using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Quality_Documents
    {
        /// <summary>
        /// 查询-万邦品质文件/客户品质文件-主页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQuality_Documents_Main(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString() : "";//文件名称
                string REMARK = jarr.ContainsKey("REMARK") ? jarr["REMARK"].ToString() : "";//备注
                string sUPLOAD_TIME = jarr.ContainsKey("sUPLOAD_TIME") ? jarr["sUPLOAD_TIME"].ToString() : "";//上传时间开始
                string eUPLOAD_TIME = jarr.ContainsKey("eUPLOAD_TIME") ? jarr["eUPLOAD_TIME"].ToString() : "";//上传时间结束
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";//文件分类
                string role_no = jarr.ContainsKey("role_no") ? jarr["role_no"].ToString() : "";//角色
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = "";
                if (!string.IsNullOrEmpty(FILE_NAME))
                {
                    where += $" and u.FILE_NAME like @FILE_NAME";
                }
                if (!string.IsNullOrEmpty(REMARK))
                {
                    where += $" and m.REMARK like @REMARK";
                }
                if (!string.IsNullOrWhiteSpace(sUPLOAD_TIME) && !string.IsNullOrWhiteSpace(eUPLOAD_TIME))
                {
                    where += $@" and (m.UPLOAD_TIME between to_date(@sUPLOAD_TIME,'yyyy-mm-dd,hh24:mi:ss') and to_date(@eUPLOAD_TIME,'yyyy-mm-dd,hh24:mi:ss'))";
                }
                if (!string.IsNullOrEmpty(role_no))
                {
                    where += $" and m.role_no = @role_no";
                }
                string sql = $@"SELECT 
                                m.id as mid,
                                u.FILE_NAME,
                                m.REMARK,
                                m.UPLOAD_TIME,
                                u.FILE_URL,
                                m.FILE_STATE,
                                m.role_no,
                                '' as role_name
                                FROM
                                bdm_quality_documents_m m
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM u on m.FILE_GUID=u.GUID
                                where file_type=@file_type {where}
                                order by m.id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("file_type", $@"{file_type}");
                paramTestDic.Add("FILE_NAME", $@"%{FILE_NAME}%");
                paramTestDic.Add("REMARK", $@"%{REMARK}%");
                paramTestDic.Add("sUPLOAD_TIME", $@"{sUPLOAD_TIME}");
                paramTestDic.Add("eUPLOAD_TIME", $@"{eUPLOAD_TIME}");
                paramTestDic.Add("role_no", $@"{role_no}");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    string role_name = DB2.GetString($@"SELECT Role_Name FROM SYSROLE01M where Role_No='{item["role_no"]}'");
                    item["role_name"] = role_name;
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
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

        /// <summary>
        /// 查询-万邦品质文件/客户品质文件-编辑页-角色
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRole_Edit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译

                string sql = $@"SELECT
	                            Role_No as 'code',
	                            Role_Name as 'value'
                            FROM
	                            SYSROLE01M";
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

        /// <summary>
        /// 保存-万邦品质文件/客户品质文件-编辑页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditQuality_Documents(object OBJ)
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
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//查询条件 备注
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "";//查询条件 文件分类
                string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";//查询条件 文件关联id
                string role_no = jarr.ContainsKey("role_no") ? jarr["role_no"].ToString() : "";//查询条件 角色
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                if (file_type == "2")
                {
                    sql = $@"insert into bdm_quality_documents_m (remark,upload_time,file_type,file_guid,role_no,createby,createdate,createtime) 
                            values('{remark}',to_date('{date}','yyyy-mm-dd hh24:mi:ss'),'{file_type}','{file_guid}','{role_no}','{user}','{date}','{time}')";
                }
                else
                {
                    sql = $@"insert into bdm_quality_documents_m (remark,upload_time,file_type,file_guid,createby,createdate,createtime) 
                            values('{remark}',to_date('{date}','yyyy-mm-dd hh24:mi:ss'),'{file_type}','{file_guid}','{user}','{date}','{time}')";
                }

                DB.ExecuteNonQuery(sql);

                DB.Commit();
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

        /// <summary>
        /// 修改-万邦品质文件/客户品质文件-编辑页-改状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditQuality_Documents_State(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 mid
                string file_state = jarr.ContainsKey("file_state") ? jarr["file_state"].ToString() : "";//查询条件 文件状态
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                if (file_state == "0")
                    sql = $@"update bdm_quality_documents_m set file_state='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{mid}'";
                else
                    sql = $@"update bdm_quality_documents_m set file_state='0',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{mid}'";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
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

        /// <summary>
        /// 删除-万邦品质文件/客户品质文件-编辑页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteQuality_Documents(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 mid
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                sql = $@"delete from bdm_quality_documents_m where id='{mid}'";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
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
