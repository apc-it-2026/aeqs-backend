using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_MESAPI
{
    class FGT_Digitalization
    {
        #region Lab_Part
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MonthlyFGTReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Month = jarr["Month"].ToString().Trim();
                string FGT_Result = jarr["FGT_Result"].ToString().Trim();
                string Article = jarr["Article"].ToString().Trim();
                string where = string.Empty;
                if(!string.IsNullOrEmpty(Article))
                {
                    where += $@" and b.prod_no='{Article}'";
                }

                string sql = $@"WITH tmp2 AS
 (SELECT art_no, test_result
    FROM qcm_ex_task_list_m
   WHERE test_type = '0'
     AND createdate >= TO_CHAR(SYSDATE - 90, 'YYYY-MM-DD')),
tmp1 AS
 (SELECT d.mer_po,
         d.se_id,
         b.prod_no,
         e.name_s,
         b.cr_reqdate,
         c.work_center,
         MIN(a.date_start_plan) date_start_plan
    FROM mes010m a
    JOIN bdm_se_order_item b
      ON a.sales_order = b.se_id
   inner join bdm_rd_prod e
      on b.prod_no = e.prod_no
    JOIN mes010a1 c
      ON a.production_order = c.production_order
    JOIN bdm_se_order_master d
      on b.se_id = d.se_id
     AND c.procedure_no = 'L'
   WHERE a.order_type = 'ZP01'
     AND TO_CHAR(b.cr_reqdate, 'yyyy/MM') = '{Month}' {where}
   GROUP BY d.mer_po, e.name_s, b.prod_no, d.se_id, b.cr_reqdate, c.work_center),
final_data AS
 (SELECT t1.prod_no,
         t1.name_s Model_Name,
         t1.mer_po PO_Number,
         t1.se_id sales_order,
         t1.work_center,
         t1.date_start_plan,
         t1.cr_reqdate,
         t2.test_result,
         ROW_NUMBER() OVER(PARTITION BY t1.prod_no ORDER BY t1.cr_reqdate, t1.date_start_plan) rn
    FROM tmp1 t1
    LEFT JOIN tmp2 t2
      ON t1.prod_no = t2.art_no)
SELECT f.prod_no,
       f.Model_Name,
       f.PO_Number,
       f.sales_order ,
       f.work_center,
       f.date_start_plan,
       TO_CHAR(f.cr_reqdate, 'yyyy/MM/dd') cr_reqdate,
       d.test_type,
       d.shoe_size,
       d.quantity,
       f.test_result,
       d.lab_requested_date,
       d.prod_send_date,
       d.lab_confirmed_date
  FROM final_data f
  LEFT JOIN T_FGT_DIGITALIZATION d
    ON f.prod_no = d.prod_no
   AND f.cr_reqdate = d.cr_reqdate
 WHERE f.rn = 1
 ORDER BY f.cr_reqdate, f.date_start_plan";

                DataTable FGT_Report = DB.GetDataTable(sql);

                if (!string.IsNullOrEmpty(FGT_Result))
                {
                    var rows = FGT_Report.AsEnumerable().Where(row =>
                    {
                        var value = row["TEST_RESULT"] == DBNull.Value
                                        ? ""
                                        : row["TEST_RESULT"].ToString().Trim();

                        if (FGT_Result.Trim().Equals("REQUIRED", StringComparison.OrdinalIgnoreCase))
                        {
                            return string.IsNullOrWhiteSpace(value); // empty / null
                        }
                        else
                        {
                            return value.Equals(FGT_Result.Trim(), StringComparison.OrdinalIgnoreCase);
                        }
                    });

                    FGT_Report = rows.Any() ? rows.CopyToDataTable() : FGT_Report.Clone();
                }

                if (FGT_Report.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(FGT_Report);
                    ret.RetData = json;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;

        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveFGT_RequiredQty(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DataTable FGT_Articles = jarr.ContainsKey("FGT_Articles") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["FGT_Articles"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                if (FGT_Articles != null && FGT_Articles.Rows.Count > 0)
                {
                    DB.Open();
                    DB.BeginTransaction();

                    string sql = @"MERGE INTO T_FGT_DIGITALIZATION tgt
USING (
    SELECT 
        :cr_reqdate cr_reqdate,
        :prod_no prod_no,
        :PO_Number PO_Number,
        :work_center work_center,
        :test_type test_type,
        :shoe_size shoe_size,
        :quantity quantity,
        :requested_by requested_by,
        :lab_requested_date lab_requested_date
    FROM dual
) src
ON (tgt.prod_no = src.prod_no AND tgt.cr_reqdate = src.cr_reqdate)

WHEN MATCHED THEN
UPDATE SET
    tgt.PO_Number = src.PO_Number,
    tgt.work_center = src.work_center,
    tgt.test_type = src.test_type,
    tgt.quantity = src.quantity,
    tgt.shoe_size = src.shoe_size,
    tgt.lab_requested_date = src.lab_requested_date,
    tgt.requested_by=src.requested_by
WHEN NOT MATCHED THEN
INSERT (
    cr_reqdate,
    prod_no,
    PO_Number,
    work_center,
    test_type,
    shoe_size,
    quantity,
    lab_requested_date,
    requested_by
)
VALUES (
    src.cr_reqdate,
    src.prod_no,
    src.PO_Number,
    src.work_center,
    src.test_type,
    src.shoe_size,
    src.quantity,
    src.lab_requested_date,
    src.requested_by
)";

                    foreach (DataRow row in FGT_Articles.Rows)
                    {
                        if (row.RowState == DataRowState.Modified || row.RowState == DataRowState.Added)
                        {
                            var param = new Dictionary<string, object>
            {
                { "cr_reqdate", row["cr_reqdate"] == DBNull.Value
                    ? (object)DBNull.Value
                    : Convert.ToDateTime(row["cr_reqdate"]) },

                { "prod_no", row["prod_no"]?.ToString() },
                { "PO_Number", row["PO_Number"]?.ToString() },
                { "work_center", row["work_center"]?.ToString() },
                { "test_type", row["test_type"]?.ToString() },
                { "shoe_size", row["shoe_size"]?.ToString() },
                { "quantity", row["quantity"]?.ToString() },
                { "requested_by", user },
                { "lab_requested_date", DateTime.Now }
            };

                            DB.ExecuteNonQuery(sql, param);
                        }
                    }

                    DB.Commit();
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Data saved successfully";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No Data Found";
                }
                
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                DB.Rollback();
            }
            finally
            {
                DB.Close();
            }
            return ret;


        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeSize(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string PO_Number = jarr["PO_Number"].ToString().Trim();
                string WorkCenter = jarr["WorkCenter"].ToString().Trim();

                string sql = $@"select material_specifications size_no
  from mes010m a
 inner join mes010a1 b
    on a.production_order = b.production_order
    inner join bdm_se_order_master c on a.sales_order=c.se_id
 where c.mer_po='{PO_Number}'
   and b.procedure_no = 'L'
   and b.work_center = '{WorkCenter}'
  order by to_number(replace(material_specifications, 'K', ''))";

                DataTable FGT_Report = DB.GetDataTable(sql);

               
                if (FGT_Report.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(FGT_Report);
                    ret.RetData = json;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;

        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveLabConfirmation(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Prod_No = jarr["Prod_No"].ToString();
                string CRD = jarr["CRD"].ToString();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DB.Open();
                    DB.BeginTransaction();
                    string sql = $@"update t_fgt_digitalization
   set LAB_CONFIRMED_DATE = sysdate,
       confirmed_by='{user}'
 where PROD_NO = '{Prod_No}'
   and to_char(CR_REQDATE, 'yyyy/MM/dd') = '{CRD}'";
                     DB.ExecuteNonQuery(sql);
                     DB.Commit();
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Lab Confirmation Successful";
                
                

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                DB.Rollback();
            }
            finally
            {
                DB.Close();
            }
            return ret;


        }
        #endregion

        #region Production Part
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFGT_Requested_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string CRD_Month = jarr["CRD_Month"].ToString().Trim();
                string Plant = jarr["Plant"].ToString().Trim();
                string Status = jarr["Status"].ToString().Trim();
                string where = string.Empty;
                if(Status== "Pending")
                {
                    where += $@"and prod_send_date is null";
                }
                else if(Status == "Completed")
                {
                    where += $@"and prod_send_date is not null";
                }

                string sql = $@"select to_char(cr_reqdate, 'yyyy/MM/dd') cr_reqdate,
       prod_no,
       po_number,
       se_id sales_order,
       work_center,
       shoe_size,
       a.test_type,
       quantity,
       lab_requested_date,
       prod_send_date,
       lab_confirmed_date,
       test_result
  from t_fgt_digitalization a
 inner join bdm_se_order_master b
    on a.po_number = b.mer_po
  left join qcm_ex_task_list_m c
    on c.order_po = a.po_number
   and c.test_type = '0'
 where to_char(cr_reqdate, 'yyyy/MM') = '{CRD_Month}'
   and work_center like '%{Plant}%' {where}";

                DataTable FGT_Request = DB.GetDataTable(sql);


                if (FGT_Request.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(FGT_Request);
                    ret.RetData = json;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No Data Found";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;

        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveProductionSubmit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Prod_No = jarr["Prod_No"].ToString();
                string CRD = jarr["CRD"].ToString();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DB.Open();
                DB.BeginTransaction();
                string sql = $@"update t_fgt_digitalization
   set prod_send_date = sysdate,
       send_by='{user}'
 where PROD_NO = '{Prod_No}'
   and to_char(CR_REQDATE, 'yyyy/MM/dd') = '{CRD}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Send to Lab Successfully";
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                DB.Rollback();
            }
            finally
            {
                DB.Close();
            }
            return ret;


        }

        #endregion

    }
}
