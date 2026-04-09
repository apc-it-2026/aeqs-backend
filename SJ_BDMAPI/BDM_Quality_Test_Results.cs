using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SJ_BDMAPI
{
    class BDM_Quality_Test_Results
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Mat_Details(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//aql等级
                string mat_code = jarr.ContainsKey("mat_code") ? jarr["mat_code"].ToString() : "";//检验水平  
                string s_date = jarr.ContainsKey("s_date") ? jarr["s_date"].ToString() : "";//aql等级
                string e_date = jarr.ContainsKey("e_date") ? jarr["e_date"].ToString() : "";//检验水平  
                var sql = string.Empty;

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(mat_code))
                {
                    where += $@"and a1.ITEM_NO like '%{mat_code}%'";


                }
                if (!string.IsNullOrWhiteSpace(art))
                {
                    where += $@" and z1.ART_NO like '%{art}%'"; 
                }
                if (!string.IsNullOrWhiteSpace(s_date)&& !string.IsNullOrWhiteSpace(e_date))
                {
                   // where += $@"and cx.from_line like '%{mat_code}%'";
                    where += $@"and a.RCPT_DATE BETWEEN TO_date('{s_date}','yyyy-MM-dd') and TO_date('{e_date}','yyyy-MM-dd') ";

                } 
                sql = $@" 
SELECT 
  MAX(a.RCPT_DATE) RCPT_DATE,
  MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME,
  MAX(a1.RCPT_QTY) AS RCPT_QTY, --收料数量
  MAX(Z2.Shoe_No) as SHOE_NO,
  listagg(DISTINCT  z1.ART_NO, ',') WITHIN GROUP (ORDER BY z1.ART_NO) as PROD_NO, --ART 
  MAX(aw.NAME_T) as NAME_T, --材料名称
   MAX(pe.ITEM_TYPE_NO) as ITEM_TYPE_NO, --物料类型 
    MAX(a1.source_no) as ORDER_NO, --采购单号
  MAX(a.CHK_NO) as CHK_NO,--收料单号
  MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)     
  MAX(a1.CHK_SEQ) as CHK_SEQ ,
  MAX(c111.PART_NO) as PART_NO
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO

INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT  JOIN bdm_purchase_order_d z1 on a2.ORDER_NO=z1.ORDER_NO and a1.SOURCE_SEQ=z1.ORDER_SEQ --新加的
LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
LEFT JOIN base003m b1 ON a2.VEND_NO=b1.SUPPLIERS_CODE

LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT JOIN base003m b ON a.vend_no=b.SUPPLIERS_CODE
LEFT JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT JOIN base001m m1 on a.ORG_ID=m1.ORG_CODE
LEFT JOIN qcm_iqc_insp_res_m s on a.CHK_NO=s.CHK_NO and a1.CHK_SEQ=s.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on a.CHK_NO=rt.CHK_NO and a1.CHK_SEQ=rt.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON a1.SOURCE_NO=c111.ORDER_NO AND a1.CHK_SEQ=c111.ORDER_SEQ 
LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
WHERE  a.RCPT_BY='01' and A.ORG_ID ='5001' and a.STOC_NO = '1000' {where}
GROUP BY a1.CHK_NO,a1.CHK_SEQ 
 order by MAX(a.INSERT_DATE) desc,MAX(a1.ITEM_NO) desc 
 ";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Mat_Details2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data); 
                string mat_code = jarr.ContainsKey("mat_code") ? jarr["mat_code"].ToString() : "";//检验水平  
                string s_date = jarr.ContainsKey("s_date") ? jarr["s_date"].ToString() : "";//aql等级
                string e_date = jarr.ContainsKey("e_date") ? jarr["e_date"].ToString() : "";//检验水平  
                var sql = string.Empty;

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(mat_code))
                {
                    where += $@"and wb.item_no like '%{mat_code}%'";


                } 
                if (!string.IsNullOrWhiteSpace(s_date) && !string.IsNullOrWhiteSpace(e_date))
                {
                    // where += $@"and cx.from_line like '%{mat_code}%'";
                    //where += $@"and a.RCPT_DATE BETWEEN TO_date('{s_date}','yyyy-MM-dd') and TO_date('{e_date}','yyyy-MM-dd') ";
                    where += $@"and TO_CHAR(wm.RCPT_DATE,'yyyy/MM/dd') BETWEEN '{s_date}' and '{e_date}'";

                }

                sql = $@" 
SELECT 
 MAX( wm.RCPT_DATE )  as RCPT_DATE,
 MAX(c.VEND_NO_PRD) AS SCVEND_NO, 
    wb.SOURCE_NO,
    MAX(wb.ITEM_NO) AS ITEM_NO,
    MAX(c.NAME_T) AS ITEM_NAME, 
    SUM( pi.ORD_QTY ) as ORD_QTY   
      FROM
                              wms_rcpt_m wm
                            INNER JOIN wms_rcpt_d wb ON wm.CHK_NO = wb.CHK_NO
                                LEFT JOIN bdm_rd_item c on wb.item_no=c.item_no
                            INNER JOIN bdm_purchase_order_item pi ON ( pi.ORDER_NO = wb.SOURCE_NO AND pi.ORDER_SEQ = wb.SOURCE_SEQ ) 
                            WHERE
                              wm.RCPT_BY = '01' {where}
                            GROUP BY
                              wb.SOURCE_NO,
                            wb.SOURCE_SEQ ";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetVendor_Report_Main_ListFile(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string vend_no = jarr.ContainsKey("vend_no") ? jarr["vend_no"].ToString() : "";//查询条件 生产厂商
                string order_no = jarr.ContainsKey("order_no") ? jarr["order_no"].ToString() : "";//查询条件 采购单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string filetype = jarr.ContainsKey("filetype") ? jarr["filetype"].ToString() : "";//查询条件 料号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string whereSql = "";
                if (string.IsNullOrEmpty(vend_no))
                    whereSql += $@" AND vend_no is null";
                else
                    whereSql += $@" AND vend_no = '{vend_no}'";

                if (string.IsNullOrEmpty(order_no))
                    whereSql += $@" AND order_no is null";
                else
                    whereSql += $@" AND order_no = '{order_no}'";

                if (string.IsNullOrEmpty(item_no))
                    whereSql += $@" AND item_no is null";
                else
                    whereSql += $@" AND item_no = '{item_no}'"; 

                string sql = $@"														
                                SELECT
	                                t.file_url,
                                CASE
		                                v.report_type 
		                                WHEN '0' THEN
		                                'T2_Test_Report' --T2测试报告
		                                WHEN '1' THEN
		                                'T2 A-01_Report' --T2 A-01报告
	                                END file_name,
	                                'bdm_vendor_report_m' AS tablename,
	                                v.id 
                                FROM
	                                bdm_vendor_report_m v
	                                INNER JOIN BDM_UPLOAD_FILE_ITEM t ON t.guid = v.file_id and v.report_type='{filetype}'
                                WHERE 1=1 {whereSql}";

                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
    }
}
