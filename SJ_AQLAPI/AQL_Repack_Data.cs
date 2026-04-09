using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    class AQL_Repack_Data
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Submit_repack_data(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";  
                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";   
                string repack_qty = jarr.ContainsKey("repack_qty") ? jarr["repack_qty"].ToString() : "";
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";
                string repackdate = jarr.ContainsKey("repackdate") ? jarr["repackdate"].ToString() : "";
                string update_reason = jarr.ContainsKey("update_reason") ? jarr["update_reason"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken); 
                string sql = $@"insert into  AQL_REPACK_DATA(PRODUCTION_LINE,PO,REPACK_QTY,REPACK_REASON,REPACK_DATE,CREATED_BY,UPDATE_REASON,LOCK_STATUS)
                             values ('{prod_line}','{po}','{repack_qty}','{reason}',to_date('{repackdate}','yyyy/MM/dd'),'{user}','{update_reason}',0)"; 
                DB.ExecuteNonQueryOffline(sql); 
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Repack_Data(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";
                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";
                string repack_qty = jarr.ContainsKey("repack_qty") ? jarr["repack_qty"].ToString() : "";
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";
                string repackdate = jarr.ContainsKey("repackdate") ? jarr["repackdate"].ToString() : "";
                string update_reason = jarr.ContainsKey("update_reason") ? jarr["update_reason"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql_select = $@"select REPACK_DATE, PRODUCTION_LINE, PO, REPACK_QTY, REPACK_REASON
  from aql_repack_data a
 where to_char(a.repack_date, 'yyyy/MM/dd') = '{repackdate}'
   and a.po = '{po}'";
                DataTable dt = DB.GetDataTable(sql_select);

                string insert_query = $@"INSERT INTO AQL_REPACK_DATA_LOG 
(
    REPACK_DATE,
    production_line,
    po,
    repack_qty,
    repack_reason,
    created_by,
    created_at,
    log_type
)
VALUES
(
    TO_DATE('{dt.Rows[0]["REPACK_DATE"].ToString()}', 'YYYY/MM/DD HH24:MI:SS'),
    '{dt.Rows[0]["PRODUCTION_LINE"].ToString()}',                           
    '{dt.Rows[0]["PO"].ToString()}',                            
    '{dt.Rows[0]["REPACK_QTY"].ToString()}',                           
    '{dt.Rows[0]["REPACK_REASON"].ToString()}',     
    '{user}',                             
    SYSDATE,
    'Update'
)";
                int i = DB.ExecuteNonQuery(insert_query);
                if(i>0)
                {
                    string sql = $@"UPDATE AQL_REPACK_DATA
SET 
    REPACK_QTY = '{repack_qty}',
    REPACK_REASON = '{reason}',
    PRODUCTION_LINE = '{prod_line}',
    MODIFIED_BY = '{user}',
    UPDATE_REASON = '{update_reason}',
    LOCK_STATUS = 0
WHERE 
    to_char(REPACK_DATE,'yyyy/MM/dd') = '{repackdate}' 
    AND PO = '{po}' ";
                    DB.ExecuteNonQuery(sql);
                }
                
                ret.IsSuccess = true;
                DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_repack_data(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";
                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : ""; 
                string s_date = jarr.ContainsKey("s_date") ? jarr["s_date"].ToString() : "";
                string e_date = jarr.ContainsKey("e_date") ? jarr["e_date"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(po))
                {
                    where += $@"and po = '{po}'";
                }
                if (!string.IsNullOrEmpty(prod_line))
                {
                    where += $@"and PRODUCTION_LINE = '{prod_line}'";
                }
                string sql = $@"select case
         when LOCK_STATUS = 1 then
          'Locked'
         when LOCK_STATUS = 0 then
          'UnLocked'
       end as LOCK_STATUS,
       to_char(REPACK_DATE,'yyyy/MM/dd') as REPACK_DATE,
       PRODUCTION_LINE,
       PO,
       REPACK_QTY,
       REPACK_REASON,
       UPDATE_REASON
  from AQL_REPACK_DATA
 where 1=1 {where} and to_char(REPACK_DATE,'yyyy/MM/dd') between '{s_date}' and '{e_date}'"; 
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt); 
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete_repack_data(object OBJ)
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
                string ProdLine = jarr.ContainsKey("ProdLine") ? jarr["ProdLine"].ToString() : "";
                string RepackDate = jarr.ContainsKey("RepackDate") ? jarr["RepackDate"].ToString() : "";
                string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);


                string where = string.Empty;
                if (!string.IsNullOrEmpty(Po))
                {
                    where += $@"and po = '{Po}'";
                }
                if (!string.IsNullOrEmpty(ProdLine))
                {
                    where += $@"and PRODUCTION_LINE = '{ProdLine}'";
                }
                if (!string.IsNullOrEmpty(RepackDate))
                {
                    where += $@"and to_char(REPACK_DATE,'yyyy/MM/dd') = '{RepackDate}'";
                }

                string sql_select = $@"select REPACK_DATE, PRODUCTION_LINE, PO, REPACK_QTY, REPACK_REASON
  from aql_repack_data a
 where to_char(a.repack_date, 'yyyy/MM/dd') = '{RepackDate}'
   and a.po = '{Po}'";
                DataTable dt = DB.GetDataTable(sql_select);

                string insert_query = $@"INSERT INTO AQL_REPACK_DATA_LOG 
(
    repack_date,
    production_line,
    po,
    repack_qty,
    repack_reason,
    created_by,
    created_at,
    log_type
)
VALUES
(
    TO_DATE('{dt.Rows[0]["REPACK_DATE"].ToString()}', 'YYYY/MM/DD HH24:MI:SS'),
    '{dt.Rows[0]["PRODUCTION_LINE"].ToString()}',                           
    '{dt.Rows[0]["PO"].ToString()}',                            
    '{dt.Rows[0]["REPACK_QTY"].ToString()}',                           
    '{dt.Rows[0]["REPACK_REASON"].ToString()}',     
    '{user}',                             
    SYSDATE,
    'Delete'
)";
                int i = DB.ExecuteNonQuery(insert_query);
                if (i > 0)
                {
                    string sql = $@"delete from AQL_REPACK_DATA where 1=1 {where}";
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_PO_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                sql = $@"
                        SELECT res.Customer_Po as MER_PO
  FROM (SELECT A.Customer_Po,
               MAX(b.SE_QTY) AS item_se_qty,
               SUM(c.SE_QTY) AS size_se_qty
          FROM BDM_SE_ORDER_MASTER A
         INNER JOIN BDM_SE_ORDER_ITEM b
            ON A.SE_ID = b.SE_ID
           AND A.ORG_ID = b.ORG_ID
         INNER JOIN BDM_SE_ORDER_SIZE c
            ON A.SE_ID = c.SE_ID
           AND a.ORG_ID = c.ORG_ID
         WHERE c.SE_QTY > 0
           and a.PO_AGGREGATOR is not null
         GROUP BY A.Customer_Po) res
 WHERE RES.ITEM_SE_QTY = RES.SIZE_SE_QTY";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMin_and_MaxDate(object OBJ)
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
                string sql = $@"select MAXDATE, MINDATE
  from kpi_data_lock_unlock
 where CRITERIA = 'RePacking'";
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
