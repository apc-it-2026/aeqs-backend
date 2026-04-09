using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_OOrderFlie
    {
        /// <summary>
        ///  CS主界面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMainList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string customer_no = jarr.ContainsKey("customer_no") ? jarr["customer_no"].ToString() : "";
                string update_s = jarr.ContainsKey("update_s") ? jarr["update_s"].ToString() : "";//日期开始
                string update_e = jarr.ContainsKey("update_e") ? jarr["update_e"].ToString() : "";//日期结束
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "1";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "10";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(customer_no))
                {
                    where += " and FILE_NAME like @customer_no";
                    paramTestDic.Add("customer_no", $@"%{customer_no}%");
                }
                if (!string.IsNullOrWhiteSpace(update_s) && !string.IsNullOrWhiteSpace(update_e))
                {
                    where += $@" and CURR_UPLOAD_TIME>=to_date('{update_s}','yyyy-mm-dd') and CURR_UPLOAD_TIME<= to_date('{update_e}','yyyy-mm-dd')";
                }
                string sql = $@"
SELECT
	FILE_NAME,
	CURR_UPLOAD_TIME
FROM
	AQL_O_ORDER_FILE_M   
where 1=1 {where} 
order by ID desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int total = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 文件提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Main(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");

                var paramsDic = new Dictionary<string, object>();
                string sql = string.Empty;
                sql = $@"SELECT COUNT(1) FROM AQL_O_ORDER_FILE_M WHERE FILE_NAME=@file_name";
                paramsDic.Add("file_name", file_name);
                string exist = DB.GetStringline(sql, paramsDic);
                paramsDic = new Dictionary<string, object>();
                if (Convert.ToInt32(exist) > 0)
                {
                    sql = $@"UPDATE AQL_O_ORDER_FILE_M SET CURR_UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),CURR_FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                    paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramsDic.Add("file_guid", file_guid);
                    paramsDic.Add("modifyby", user);
                    paramsDic.Add("modifydate", date);
                    paramsDic.Add("modifytime", time);
                    paramsDic.Add("file_name", file_name);
                    DB.ExecuteNonQuery(sql, paramsDic);
                }
                else
                {
                    sql = $@"INSERT INTO AQL_O_ORDER_FILE_M(FILE_NAME,CURR_UPLOAD_TIME,CURR_FILE_GUID,CREATEBY,CREATEDATE,CREATETIME) VALUES(@file_name,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@file_guid,@createby,@createdate,@createtime)";
                    paramsDic.Add("file_name", file_name);
                    paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramsDic.Add("file_guid", file_guid);
                    paramsDic.Add("createby", user);
                    paramsDic.Add("createdate", date);
                    paramsDic.Add("createtime", time);
                    DB.ExecuteNonQuery(sql, paramsDic);
                }
                paramsDic = new Dictionary<string, object>();
                sql = $"INSERT INTO AQL_O_ORDER_FILE_D(FILE_NAME,UPLOAD_TIME,FILE_GUID,CREATEBY,CREATEDATE,CREATETIME) VALUES(@file_name,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@file_guid,@createby,@createdate,@createtime)";
                paramsDic.Add("file_name", file_name);
                paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                paramsDic.Add("file_guid", file_guid);
                paramsDic.Add("createby", user);
                paramsDic.Add("createdate", date);
                paramsDic.Add("createtime", time);
                DB.ExecuteNonQuery(sql, paramsDic);

                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "提交成功";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "提交失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// CS主界面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Delete(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                string sql = $@"delete from AQL_O_ORDER_FILE_M where FILE_NAME=@file_name";
                DB.ExecuteNonQuery(sql, paramsDic);
                sql = $@"delete from AQL_O_ORDER_FILE_D where FILE_NAME=@file_name";
                DB.ExecuteNonQuery(sql, paramsDic);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "删除成功";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "删除失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查看单个的 提交明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_ListFile(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                string sql = $@"
SELECT
	a.ID,
	a.FILE_NAME,
	a.upload_time,
	a.file_guid,
	b.file_url,
	b.file_name
FROM
	AQL_O_ORDER_FILE_D a
INNER JOIN bdm_upload_file_item b ON a.file_guid = b.guid
WHERE
	a.FILE_NAME = @file_name 
ORDER BY a.UPLOAD_TIME DESC";
                DataTable dt = DB.GetDataTable(sql, paramsDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 根据id删除明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteByDId(object OBJ)
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
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("id", id);
                string sql = $@"delete from AQL_O_ORDER_FILE_D where id=@id";
                DB.ExecuteNonQuery(sql, paramsDic);

                paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                sql = $@"
SELECT
	a .upload_time,
	a .file_guid
FROM
	AQL_O_ORDER_FILE_D a
WHERE
	a.FILE_NAME = '{file_name}' 
ORDER BY a.UPLOAD_TIME DESC
";
                DataTable dt = DB.GetDataTablebyline(sql);
                paramsDic = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    paramsDic.Add("currTime", dt.Rows[0]["UPLOAD_TIME"].ToString());
                    paramsDic.Add("file_guid", dt.Rows[0]["FILE_GUID"].ToString());
                    paramsDic.Add("modifyby", user);
                    paramsDic.Add("modifydate", date);
                    paramsDic.Add("modifytime", time);
                    paramsDic.Add("file_name", file_name);
                    sql = $@"UPDATE AQL_O_ORDER_FILE_M SET CURR_UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),CURR_FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                }
                else
                {
                    paramsDic.Add("file_name", file_name);
                    sql = $@"delete from AQL_O_ORDER_FILE_M where FILE_NAME=@file_name";
                }
                DB.ExecuteNonQuery(sql, paramsDic);
                DB.Commit();

                ret.IsSuccess = true;
                ret.ErrMsg = "删除成功";
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

        /// <summary>
        /// AQL录入 查询文件guid
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileGuidByName(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";

                string file_name = po;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("file_name", file_name);
                string sql = $@"
SELECT
	b.file_url,
	b.file_name
FROM
	AQL_O_ORDER_FILE_M a
INNER JOIN bdm_upload_file_item b ON a.CURR_FILE_GUID = b.guid
WHERE
	a.FILE_NAME =@file_name";

                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("file_url", dt.Rows[0][0].ToString());
                    dic.Add("file_name", dt.Rows[0][1].ToString());
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(dic);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No official order uploaded";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
    }
}
