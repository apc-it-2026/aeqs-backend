using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    public class FilesuploadBase
    {
        private static void NullKey(Dictionary<string, object> dic)
        {
            string[] keys = dic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (dic[keys[i]] == null)
                {
                    dic[keys[i]] = "";
                }
            }
        }
        /// <summary>
        /// 主页数据（联名）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMianList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //NullKey(jarr);
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";

                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";

                string starttime1 = jarr.ContainsKey("starttime1") ? jarr["starttime1"].ToString() : "";
                string endtime1 = jarr.ContainsKey("endtime1") ? jarr["endtime1"].ToString() : "";

                string starttime2 = jarr.ContainsKey("starttime2") ? jarr["starttime2"].ToString() : "";
                string endtime2 = jarr.ContainsKey("endtime2") ? jarr["endtime2"].ToString() : "";


                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
               string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(file_type))
                {
                    strwhere += $@" and  FILE_TYPE='{file_type}'";
                }

                if (!string.IsNullOrWhiteSpace(art))
                {
                    strwhere += $@" and  prod_no like'%{art}%'";
                }
                //有效期 endtime1
                if (!string.IsNullOrWhiteSpace(starttime1))
                {
                    strwhere += $@"and to_date(curr_valid_time,'yyyy-MM-dd') BETWEEN to_date('{starttime1}','yyyy-MM-dd') AND to_date('{endtime1}','yyyy-MM-dd') ";
                }
                //上传时间 starttime2
                if (!string.IsNullOrWhiteSpace(starttime2) || !string.IsNullOrWhiteSpace(endtime2))
                {
                    strwhere += $@"and to_date(UPLOAD_TIME,'yyyy-MM-dd HH24:mi:ss') BETWEEN to_date('{starttime2}','yyyy-MM-dd HH24:mi:ss') AND to_date('{endtime2}','yyyy-MM-dd HH24:mi:ss')";
                }

                string sql = $@"
select 
prod_no
,FILE_TYPE as curr_file_type
,curr_valid_time
,UPLOAD_TIME as curr_upload_time
 from qcm_jointly_file_d   where 1=1   
and id in(SELECT MAX (ID) FROM qcm_jointly_file_d A GROUP BY PROD_NO,file_type)
{strwhere}
order by id desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int total = CommonBASE.GetPageDataTableCount(DB, sql);
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
        /// 主页数据（安全合规）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMianList2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                NullKey(jarr);
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";
                string txt_Art = jarr.ContainsKey("txt_Art") ? jarr["txt_Art"].ToString() : "";

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";


                string start_dateyxq = jarr.ContainsKey("start_dateyxq") ? jarr["start_dateyxq"].ToString() : "";
                string end_dateyxq = jarr.ContainsKey("end_dateyxq") ? jarr["end_dateyxq"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(file_type))
                {
                    strwhere += $@" and  a.file_type='{file_type}'";
                }
                if (!string.IsNullOrWhiteSpace(txt_Art))
                {
                    strwhere += $@" and  a.prod_no like'%{txt_Art}%'";
                }
                if (!string.IsNullOrWhiteSpace(start_date) || !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@"and to_date(a.UPLOAD_TIME,'yyyy/MM/dd HH24:mi:ss') BETWEEN to_date('{start_date}','yyyy-MM-dd HH24:mi:ss') AND to_date('{end_date}','yyyy-MM-dd HH24:mi:ss')";
                }
                if (!string.IsNullOrWhiteSpace(start_dateyxq) || !string.IsNullOrWhiteSpace(end_dateyxq))
                {
                    strwhere += $@"and to_date(a.CURR_VALID_TIME,'yyyy/MM/dd HH24:mi:ss') BETWEEN to_date('{start_dateyxq}','yyyy-MM-dd HH24:mi:ss') AND to_date('{end_dateyxq}','yyyy-MM-dd HH24:mi:ss')";
                }
                //                string sql = $@"
                //select 
                //a.prod_no,
                //b.enum_value as curr_file_type,
                //TO_CHAR(to_date(a.curr_upload_time,'yyyy/MM/dd HH24:mi:ss'),'yyyy-MM-dd') curr_upload_time,
                //TO_CHAR(to_date(a.CURR_VALID_TIME,'yyyy/MM/dd HH24:mi:ss'),'yyyy-MM-dd') CURR_VALID_TIME
                // from qcm_safety_compliance_file_m a left join  sys001m b on a.curr_file_type=b.enum_code
                //where b.enum_type='enum_qcm_safety'  {strwhere} order by a.id desc";
                string sql = $@"
select 
a.prod_no,
b.enum_code as filetype_no,
b.enum_value as curr_file_type,
TO_CHAR(to_date(a.UPLOAD_TIME,'yyyy/MM/dd HH24:mi:ss'),'yyyy-MM-dd') curr_upload_time,
TO_CHAR(to_date(a.CURR_VALID_TIME,'yyyy/MM/dd HH24:mi:ss'),'yyyy-MM-dd') CURR_VALID_TIME
 from qcm_safety_compliance_file_d a 
left join  sys001m b on a.file_type=b.enum_code
LEFT JOIN qcm_safety_compliance_file_m c on c.prod_no = a.prod_no
where b.enum_type='enum_qcm_safety'   
{strwhere}
and a.id in(SELECT MAX (ID) FROM qcm_safety_compliance_file_d A GROUP BY PROD_NO,file_type)
order by a.id desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int total = CommonBASE.GetPageDataTableCount(DB, sql);

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
        /// 下拉框内容(联合)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CobList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string strwhere = string.Empty;

                string sql = $@"select file_type from qcm_jointly_file_type_m";
                DataTable dt = DB.GetDataTable(sql);
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
        /// 下拉框内容（安全合规）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CobList2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string strwhere = string.Empty;

                string sql = $@"select enum_code,enum_value from sys001m where enum_type='enum_qcm_safety'";
                DataTable dt = DB.GetDataTable(sql);
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
        /// 上传文件（联名）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Mian(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string curr_file_type = jarr.ContainsKey("curr_file_type") ? jarr["curr_file_type"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = string.Empty;

                var listprod = prod_no.Split(',');


                foreach (var itempro in listprod)
                {
                    sql = $@"select count(1) from qcm_jointly_file_m where   prod_no='{itempro}'";
                    if (DB.GetInt32(sql) > 0)
                    {
                        sql = $@"update qcm_jointly_file_m set curr_file_type='{curr_file_type}',curr_valid_time='{putin_date}',curr_upload_time='{date + " " + time}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where prod_no='{itempro}'";
                    }
                    else
                    {

                        sql = $@"insert into qcm_jointly_file_m(prod_no,curr_file_type,curr_valid_time,curr_upload_time,createby,createdate,createtime)values('{itempro}','{curr_file_type}','{putin_date}','{date + " " + time}','{user}','{date}','{time}')";
                    }
                    DB.ExecuteNonQuery(sql);

                    List<string> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<string>>(jarr["file_list"].ToString()) : null;
                    if (list.Count > 0)
                    {
                        sql = string.Empty;
                        foreach (string item in list)
                        {
                            sql += $"insert into qcm_jointly_file_d(prod_no,file_type,valid_time,upload_time,file_guid,createby,createdate,createtime,curr_valid_time) values('{itempro}','{curr_file_type}','{putin_date}','{date + " " + time}','{item}','{user}','{date}','{time}','{putin_date}');";

                        }
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                    }
                }

                
                ret.IsSuccess = true;
                //ret.ErrMsg = "提交成功";
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "提交失败，原因：" + ex.Message;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 上传文件（安全合规）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Mian2(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string curr_file_type = jarr.ContainsKey("curr_file_type") ? jarr["curr_file_type"].ToString() : "";
                string curr_valid_time = jarr.ContainsKey("curr_valid_time") ? jarr["curr_valid_time"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = string.Empty;
                var listprod = prod_no.Split(',');
                foreach (var itemprod in listprod)
                {

                    sql = $@"select count(1) from qcm_safety_compliance_file_m where   prod_no='{itemprod}' ";
                    if (DB.GetInt32(sql) > 0)
                    {
                        sql = $@"update qcm_safety_compliance_file_m set curr_file_type='{curr_file_type}',curr_upload_time='{date + " " + time}',curr_valid_time = '{curr_valid_time + " 00:00:00"}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where prod_no='{itemprod}'";
                    }
                    else
                    {

                        sql = $@"insert into qcm_safety_compliance_file_m(prod_no,curr_file_type,curr_upload_time,curr_valid_time,createby,createdate,createtime)values('{itemprod}','{curr_file_type}','{date + " " + time}','{curr_valid_time + " 00:00:00"}','{user}','{date}','{time}')";
                    }
                    DB.ExecuteNonQuery(sql);

                    List<string> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<string>>(jarr["file_list"].ToString()) : null;
                    if (list.Count > 0)
                    {
                        sql = string.Empty;
                        foreach (string item in list)
                        {
                            sql += $"insert into qcm_safety_compliance_file_d(prod_no,file_type,upload_time,file_guid,createby,createdate,createtime,CURR_VALID_TIME) values('{itemprod}','{curr_file_type}','{date + " " + time}','{item}','{user}','{date}','{time}','{curr_valid_time}');";

                        }
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                    }


                }

                
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "提交失败，原因：" + ex.Message;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 删除（联名）
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
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//file_type
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";//

                string sql = string.Empty;

                if (string.IsNullOrEmpty(file_type))
                {
                    sql = $@"delete from qcm_jointly_file_d where prod_no='{prod_no}' ";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"delete from qcm_jointly_file_d where prod_no='{prod_no}' and file_type = '{file_type}'";
                    DB.ExecuteNonQuery(sql);
                }

                string count = DB.GetStringline($@"select count(1) from qcm_jointly_file_d where prod_no='{prod_no}' ");
                if (count == "") count = "0";

                if(int.Parse(count) == 0)
                {
                    sql = $@"delete from qcm_jointly_file_m where prod_no='{prod_no}'";
                    DB.ExecuteNonQuery(sql);
                }

                
               
                ret.IsSuccess = true;
                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
               // ret.ErrMsg = "删除失败，原因：" + ex.Message;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 删除（安全合规）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Delete2(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string filetype_no = jarr.ContainsKey("filetype_no") ? jarr["filetype_no"].ToString() : "";
                string sql = string.Empty;
                sql = $@"delete from qcm_safety_compliance_file_d where prod_no='{prod_no}' and FILE_TYPE = '{filetype_no}'";
                DB.ExecuteNonQuery(sql);

                string count = DB.GetStringline($@"select count(1) from qcm_safety_compliance_file_d where prod_no='{prod_no}'");
                if (count == "") count = "0";
                if (int.Parse(count) == 0)
                {
                    sql = $@"delete from qcm_safety_compliance_file_m where prod_no='{prod_no}' ";
                    DB.ExecuteNonQuery(sql);
                }
                
                
                ret.IsSuccess = true;
                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "删除失败，原因：" + ex.Message;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        ///图片(联合的)
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
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";

                string sql = $@"SELECT
	a.id,
    a.prod_no,
	a.valid_time,
	a.upload_time,
	a.file_guid,
    b.file_url,
	b.file_name
FROM
	qcm_jointly_file_d a inner join   bdm_upload_file_item b on a.file_guid = b.guid where a.prod_no='{prod_no}' and a.file_type = '{file_type}'";
                DataTable dt = DB.GetDataTable(sql);
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
        ///图片(安全合规)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_ListFile2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";

                string sql = $@"SELECT
	a.id,
    a.prod_no,
	'' as valid_time,
	a.upload_time,
	a.file_guid,
    b.file_url,
	b.file_name
FROM
	qcm_safety_compliance_file_d a inner join   bdm_upload_file_item b on a.file_guid = b.guid  where a.prod_no='{prod_no}' and file_type = '{file_type}'";
                DataTable dt = DB.GetDataTable(sql);
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
        /// 删除文件（联名和安全合规）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpDelete(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string keyname = jarr.ContainsKey("keyname") ? jarr["keyname"].ToString() : "";
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                //1联名,2合规
                if (keyname == "1")
                {
                    string sql = $@"delete from qcm_jointly_file_d where id='{id}'";
                    DB.ExecuteNonQuery(sql);
                    sql = $@"select valid_time,prod_no,file_type,upload_time from qcm_jointly_file_d where id in (
select max(id) from qcm_jointly_file_d where id!='{id}')";
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        sql = $@"update qcm_jointly_file_m set curr_file_type='{dt.Rows[0]["FILE_TYPE"]}',curr_valid_time='{dt.Rows[0]["VALID_TIME"]}',curr_upload_time='{dt.Rows[0]["UPLOAD_TIME"]}' where prod_no='{dt.Rows[0]["PROD_NO"]}'";
                    }
                    else
                    {
                        sql = $@"delete from qcm_jointly_file_m where prod_no='{prod_no}'";
                    }
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    string sql = $@"delete from qcm_safety_compliance_file_d where id='{id}'";
                    DB.ExecuteNonQuery(sql);
                    sql = $@"select prod_no,file_type,upload_time from qcm_safety_compliance_file_d where id in (
select max(id) from qcm_safety_compliance_file_d where id!='{id}')";
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        sql = $@"update qcm_safety_compliance_file_m set curr_file_type='{dt.Rows[0]["FILE_TYPE"]}',curr_upload_time='{dt.Rows[0]["UPLOAD_TIME"]}' where prod_no='{dt.Rows[0]["PROD_NO"]}'";
                    }
                    else
                    {
                        sql = $@"delete from qcm_safety_compliance_file_m where prod_no='{prod_no}'";
                    }
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg =ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();

                string where = string.Empty;
                string OrderBy = "ORDER BY tab.rn";
                Dictionary<string, object> param = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(keyword))
                {
                    sql += $" and 鞋型 like @鞋型 or  ART like @ART ";

                    param.Add("鞋型", $@"%{keyword}%");
                    param.Add("ART", $@"%{ keyword}%");

                }

                DataTable dt = DB.GetDataTable(sql+ OrderBy, param);

                if (dt.Rows.Count > 0)
                {
                    dic.Add("data", dt);
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(dic); ;
                }
                else
                {
                    ret.IsSuccess = false;
                   // ret.ErrMsg = "查无数据！";
                    ret.ErrMsg = "No data found！";
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
