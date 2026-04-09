using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    /// <summary>
    /// 胶水危废处理
    /// </summary>
    public class SCRAP_GLUE
    {

        /// <summary>
        /// 查询 报废胶水维护
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetScrapGlue(object OBJ)
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
                string keyword = "";//查询条件
                if (jarr.ContainsKey("keyword"))
                    keyword = jarr["keyword"] == null ? "" : jarr["keyword"].ToString();
                string page = "1";
                if (jarr.ContainsKey("page"))
                    page = jarr["page"] == null ? "1" : jarr["page"].ToString();
                string pageRow = "10";
                if (jarr.ContainsKey("pageRow"))
                    pageRow = jarr["pageRow"] == null ? "10" : jarr["pageRow"].ToString();

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (SCRAP_GLUE_NO like '%{keyword}%' or SCRAP_GLUE_NAME like '%{keyword}%')";
                }
                string sql = $@"
                                SELECT
	                                SCRAP_GLUE_NO AS value,
	                                SCRAP_GLUE_NAME AS label
                                FROM
	                                BDM_SCRAP_GLUE_M 
                                WHERE 1=1 {where} 
                                ORDER BY ID DESC";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 查询 报废胶水原因维护
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetScrapGlueReason(object OBJ)
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
                bool is_cs = jarr.ContainsKey("is_cs");
                string keyword = "";//查询条件
                if (jarr.ContainsKey("keyword"))
                    keyword = jarr["keyword"] == null ? "" : jarr["keyword"].ToString();
                string page = "1";
                if (jarr.ContainsKey("page"))
                    page = jarr["page"] == null ? "1" : jarr["page"].ToString();
                string pageRow = "10";
                if (jarr.ContainsKey("pageRow"))
                    pageRow = jarr["pageRow"] == null ? "10" : jarr["pageRow"].ToString();

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and SCRAP_GLUE_REASON like '%{keyword}%'";
                }
                string sql = $@"
                                SELECT
	                                SCRAP_GLUE_REASON AS value,
	                                SCRAP_GLUE_REASON AS label
                                FROM
	                                BDM_SCRAP_GLUE_R_M 
                                WHERE 1=1 {where} 
                                ORDER BY ID DESC";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                if (is_cs)
                    ret.RetData = JsonConvert.SerializeObject(dic);
                else
                    ret.RetData1 = dic;
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
        /// 查询 生产单位
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDEPARTMENT(object OBJ)
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
                bool is_cs = jarr.ContainsKey("is_cs");//查询条件
                string keyword = "";//查询条件
                if (jarr.ContainsKey("keyword"))
                    keyword = jarr["keyword"] == null ? "" : jarr["keyword"].ToString();
                string page = "1";
                if (jarr.ContainsKey("page"))
                    page = jarr["page"] == null ? "1" : jarr["page"].ToString();
                string pageRow = "10";
                if (jarr.ContainsKey("pageRow"))
                    pageRow = jarr["pageRow"] == null ? "10" : jarr["pageRow"].ToString();

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (DEPARTMENT_CODE like '%{keyword}%' or DEPARTMENT_NAME like '%{keyword}%')";
                }
                string sql = $@"
                                SELECT
	                                DEPARTMENT_CODE AS value,
	                                MAX(DEPARTMENT_NAME) AS label
                                FROM
	                                BASE005M 
                                WHERE 1=1 {where} 
                                GROUP BY DEPARTMENT_CODE ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                if (is_cs)
                    ret.RetData = JsonConvert.SerializeObject(dic);
                else
                    ret.RetData1 = dic;
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
        /// 查询 胶水危废处理 移动端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchScrapGlueMagRecord(object OBJ)
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
                string keyword = "";//查询条件
                if (jarr.ContainsKey("keyword"))
                    keyword = jarr["keyword"] == null ? "" : jarr["keyword"].ToString();
                string startDate = "";//开始日期
                string endDate = "";//结束日期
                if (jarr.ContainsKey("start_date"))
                    startDate = jarr["start_date"] == null ? "" : jarr["start_date"].ToString();
                if (jarr.ContainsKey("end_date"))
                    endDate = jarr["end_date"] == null ? "" : jarr["end_date"].ToString();
                string page = "1";
                if (jarr.ContainsKey("page"))
                    page = jarr["page"] == null ? "1" : jarr["page"].ToString();
                string pageRow = "10";
                if (jarr.ContainsKey("pageRow"))
                    pageRow = jarr["pageRow"] == null ? "10" : jarr["pageRow"].ToString();

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (B_M.DEPARTMENT_NAME like '%{keyword}%' or M1.SCRAP_GLUE_NAME like '%{keyword}%' or M.SCRAP_GLUE_WEIGHT like '%{keyword}%' or M.SCRAP_GLUE_REASON like '%{keyword}%' or BF_M.STAFF_NAME like '%{keyword}%' or HB_M.STAFF_NAME like '%{keyword}%')";
                }
                if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
                {
                    where += $@" and (M.CREATEDATE>='{startDate}' and M.CREATEDATE<='{endDate}')";
                }
                string sql = $@"
SELECT
	M.ID,
	M.DEPARTMENT_CODE,
	B_M.DEPARTMENT_NAME,
	M.SCRAP_GLUE_NO,
	M1.SCRAP_GLUE_NAME,
	M.SCRAP_GLUE_WEIGHT,
	M.SCRAP_GLUE_REASON,
	M.BF_AUTOGRAPH,
	BF_M.STAFF_NAME AS BF_AUTOGRAPH_NAME,
	M.HB_AUTOGRAPH,
	HB_M.STAFF_NAME AS HB_AUTOGRAPH_NAME,
	M.CREATEDATE
FROM
	BDM_SCRAP_GLUE_MAG_M M
LEFT JOIN HR001M BF_M ON BF_M.STAFF_NO = M.BF_AUTOGRAPH
LEFT JOIN HR001M HB_M ON HB_M.STAFF_NO = M.HB_AUTOGRAPH 
LEFT JOIN BASE005M B_M ON B_M.DEPARTMENT_CODE = M.DEPARTMENT_CODE 
LEFT JOIN BDM_SCRAP_GLUE_M M1 ON M1.SCRAP_GLUE_NO = M.SCRAP_GLUE_NO 
WHERE 1=1 {where}
ORDER BY M.ID DESC";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// 新增或修改 胶水危废处理
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddOrEditScrapGlueMagRecord(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<AddOrEditScrapGlueMagRecordReq>(Data);

                #region 非空校验
                if (string.IsNullOrEmpty(request.scrap_glue_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“报废胶水”不能为空";
                    return ret;
                }
                if (string.IsNullOrEmpty(request.scrap_glue_reason))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“报废胶水原因”不能为空";
                    return ret;
                }
                if (string.IsNullOrEmpty(request.department_code))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“生产单位”不能为空";
                    return ret;
                }
                if (string.IsNullOrEmpty(request.scrap_glue_weight))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“报废胶水重量”不能为空";
                    return ret;
                }
                #endregion

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");//日期
                string time = currTime.ToString("HH:mm:ss");//时间
                DB.Open();
                DB.BeginTransaction();

                Dictionary<string, object> paramsDic = new Dictionary<string, object>();
                paramsDic.Add("SCRAP_GLUE_NO", request.scrap_glue_no);
                paramsDic.Add("SCRAP_GLUE_REASON", request.scrap_glue_reason);
                paramsDic.Add("SCRAP_GLUE_WEIGHT", request.scrap_glue_weight);
                paramsDic.Add("DEPARTMENT_CODE", request.department_code);
                paramsDic.Add("BF_AUTOGRAPH", request.bf_autograph);
                paramsDic.Add("HB_AUTOGRAPH", request.hb_autograph);
                if (!request.id.HasValue)
                {
                    paramsDic.Add("CREATEBY", user);
                    paramsDic.Add("CREATEDATE", date);
                    paramsDic.Add("CREATETIME", time);
                    string sql = $@"
                                    INSERT INTO BDM_SCRAP_GLUE_MAG_M (
	                                    SCRAP_GLUE_NO,
	                                    SCRAP_GLUE_REASON,
	                                    SCRAP_GLUE_WEIGHT,
	                                    DEPARTMENT_CODE,
	                                    BF_AUTOGRAPH,
	                                    HB_AUTOGRAPH,
	                                    CREATEBY,
	                                    CREATEDATE,
	                                    CREATETIME
                                    )
                                    VALUES
	                                    (
                                        @SCRAP_GLUE_NO,
                                        @SCRAP_GLUE_REASON,
                                        @SCRAP_GLUE_WEIGHT,
                                        @DEPARTMENT_CODE,
                                        @BF_AUTOGRAPH,
                                        @HB_AUTOGRAPH,
                                        @CREATEBY,
                                        @CREATEDATE,
                                        @CREATETIME
                                    )
                                    ";
                    DB.ExecuteNonQuery(sql, paramsDic);
                }
                else
                {
                    paramsDic.Add("MODIFYBY", user);
                    paramsDic.Add("MODIFYDATE", date);
                    paramsDic.Add("MODIFYTIME", time);
                    paramsDic.Add("ID", request.id.Value);
                    string sql = $@"
                                UPDATE BDM_SCRAP_GLUE_MAG_M
                                SET SCRAP_GLUE_NO=@SCRAP_GLUE_NO,
                                    SCRAP_GLUE_REASON=@SCRAP_GLUE_REASON,
                                    SCRAP_GLUE_WEIGHT=@SCRAP_GLUE_WEIGHT,
                                    DEPARTMENT_CODE=@DEPARTMENT_CODE,
                                    BF_AUTOGRAPH=@BF_AUTOGRAPH,
                                    HB_AUTOGRAPH=@HB_AUTOGRAPH,
                                    MODIFYBY = @MODIFYBY,
                                    MODIFYDATE = @MODIFYDATE,
                                    MODIFYTIME = @MODIFYTIME
                                WHERE
	                                ID = @ID
                                ";
                    DB.ExecuteNonQuery(sql, paramsDic);
                }

                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = "保存成功";
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
        /// 根据id获取 胶水危废处理 明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetScrapGlueMagRecordById(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = "";
                if (jarr.ContainsKey("id"))
                    id = jarr["id"] == null ? "" : jarr["id"].ToString();
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“id”Can not be empty";
                    return ret;
                }
                DB.Open();
                DB.BeginTransaction();

                string sql = $@"
                            SELECT
	                            M.ID,
	                            M.DEPARTMENT_CODE,
	                            B_M.DEPARTMENT_NAME,
	                            M.SCRAP_GLUE_NO,
	                            M1.SCRAP_GLUE_NAME,
	                            M.SCRAP_GLUE_WEIGHT,
	                            M.SCRAP_GLUE_REASON,
	                            M.BF_AUTOGRAPH,
	                            BF_M.STAFF_NAME AS BF_AUTOGRAPH_NAME,
	                            M.HB_AUTOGRAPH,
	                            HB_M.STAFF_NAME AS HB_AUTOGRAPH_NAME
                            FROM
	                            BDM_SCRAP_GLUE_MAG_M M
                            LEFT JOIN HR001M BF_M ON BF_M.STAFF_NO = M.BF_AUTOGRAPH
                            LEFT JOIN HR001M HB_M ON HB_M.STAFF_NO = M.HB_AUTOGRAPH
                            LEFT JOIN BASE005M B_M ON B_M.DEPARTMENT_CODE = M.DEPARTMENT_CODE 
                            LEFT JOIN BDM_SCRAP_GLUE_M M1 ON M1.SCRAP_GLUE_NO = M.SCRAP_GLUE_NO 
                            WHERE M.ID={id}";
                DataTable dt = DB.GetDataTablebyline(sql);

                if (dt.Rows.Count > 0)
                {
                    ret.RetData1 = dt.ToDataList<GetScrapGlueMagRecordByIdRsp>()[0];
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "Data does not exist";
                    ret.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
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
        /// 根据id删除 胶水危废处理 明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DelScrapGlueMagRecordById(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = "";
                if (jarr.ContainsKey("id"))
                    id = jarr["id"] == null ? "" : jarr["id"].ToString();
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“id”Can not be empty";
                    return ret;
                }
                DB.Open();
                DB.BeginTransaction();

                string sql = $@"
DELETE FROM BDM_SCRAP_GLUE_MAG_M WHERE ID={id}";
                DB.ExecuteNonQuery(sql);
                DB.Commit();

                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
        /// 签名校验
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ScrapGlueMagAutograph(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = ReqObj.Data.ToString();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase DBCompany = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CompanyID = DB.GetString($@"
SELECT
	CompanyCode
FROM
	usertoken u
WHERE
	u.UserToken = '{ReqObj.UserToken}'
");//套账id
                string user_name = jarr["UserCode"].ToString();
                string pwd = jarr["UserPassword"].ToString();

                if (!string.IsNullOrEmpty(CompanyID))
                {
                    string sql = "select * from SYSORG01M where org = '" + CompanyID + "'";
                    DataTable sqldt = DB.GetDataTable(sql);
                    if (sqldt.Rows.Count > 0)
                    {
                        sql = "select * from " + DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd + "'";
                        DataTable dt = DB.GetDataTable(sql);
                        string id = Guid.NewGuid().ToString();

                        if (dt.Rows.Count == 0)
                        {
                            dt = DB.GetDataTable("select * from " + DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd.ToLower() + "'");
                        }
                        if (dt == null || dt.Rows.Count == 0)
                            throw new Exception("账号或密码错误，请检查！");


                        if (dt.Rows.Count > 0)
                        {
                            string user_name_name = DBCompany.GetString($@"SELECT STAFF_NAME FROM HR001M WHERE STAFF_NO='{user_name}'");
                            ret.RetData1 = new
                            {
                                code = user_name,
                                name = user_name_name
                            };
                            ret.IsSuccess = true;
                            ret.ErrMsg = "correct password";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "Incorrect username or password！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Set account does not exist";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Set account does not exist";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询 胶水危废处理 CS端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchScrapGlueMagRecordByCS(object OBJ)
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

                string where = string.Empty;
                string dp_code = "";//生产单位 编号
                if (jarr.ContainsKey("dp_code"))
                    dp_code = jarr["dp_code"] == null ? "" : jarr["dp_code"].ToString();
                if (!string.IsNullOrWhiteSpace(dp_code))
                {
                    where += $@" and M.DEPARTMENT_CODE='{dp_code}'";
                }
                string sg_reason = "";//报废原因
                if (jarr.ContainsKey("sg_reason"))
                    sg_reason = jarr["sg_reason"] == null ? "" : jarr["sg_reason"].ToString();
                if (!string.IsNullOrWhiteSpace(sg_reason))
                {
                    where += $@" and M.SCRAP_GLUE_REASON = '{sg_reason}'";
                }
                string bf_staff_name = "";//报废单位签名
                if (jarr.ContainsKey("bf_staff_name"))
                    bf_staff_name = jarr["bf_staff_name"] == null ? "" : jarr["bf_staff_name"].ToString();
                if (!string.IsNullOrWhiteSpace(bf_staff_name))
                {
                    where += $@" and BF_M.STAFF_NAME like '%{bf_staff_name}%'";
                }
                string hb_staff_name = "";//环保股回收签名
                if (jarr.ContainsKey("hb_staff_name"))
                    hb_staff_name = jarr["hb_staff_name"] == null ? "" : jarr["hb_staff_name"].ToString();
                if (!string.IsNullOrWhiteSpace(hb_staff_name))
                {
                    where += $@" and HB_M.STAFF_NAME like '%{hb_staff_name}%'";
                }

                string startDate = "";//开始日期
                string endDate = "";//结束日期
                if (jarr.ContainsKey("start_date"))
                    startDate = jarr["start_date"] == null ? "" : jarr["start_date"].ToString();
                if (jarr.ContainsKey("end_date"))
                    endDate = jarr["end_date"] == null ? "" : jarr["end_date"].ToString();
                string page = "1";
                if (jarr.ContainsKey("page"))
                    page = jarr["page"] == null ? "1" : jarr["page"].ToString();
                string pageRow = "10";
                if (jarr.ContainsKey("pageRow"))
                    pageRow = jarr["pageRow"] == null ? "10" : jarr["pageRow"].ToString();

                if (!string.IsNullOrWhiteSpace(startDate) && !string.IsNullOrWhiteSpace(endDate))
                {
                    where += $@" and (M.CREATEDATE>='{startDate}' and M.CREATEDATE<='{endDate}')";
                }
                string sql = $@"
SELECT
	M.ID,
	M.DEPARTMENT_CODE,
	B_M.DEPARTMENT_NAME,
	M.SCRAP_GLUE_NO,
	M1.SCRAP_GLUE_NAME,
	M.SCRAP_GLUE_WEIGHT,
	M.SCRAP_GLUE_REASON,
	M.BF_AUTOGRAPH,
	BF_M.STAFF_NAME AS BF_AUTOGRAPH_NAME,
	M.HB_AUTOGRAPH,
	HB_M.STAFF_NAME AS HB_AUTOGRAPH_NAME,
	M.CREATEDATE
FROM
	BDM_SCRAP_GLUE_MAG_M M
LEFT JOIN HR001M BF_M ON BF_M.STAFF_NO = M.BF_AUTOGRAPH
LEFT JOIN HR001M HB_M ON HB_M.STAFF_NO = M.HB_AUTOGRAPH 
LEFT JOIN BASE005M B_M ON B_M.DEPARTMENT_CODE = M.DEPARTMENT_CODE 
LEFT JOIN BDM_SCRAP_GLUE_M M1 ON M1.SCRAP_GLUE_NO = M.SCRAP_GLUE_NO 
WHERE 1=1 {where}
ORDER BY M.ID DESC";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = JsonConvert.SerializeObject(dic);
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

    #region dto
    public class AddOrEditScrapGlueMagRecordReq
    {
        public int? id { get; set; }
        /// <summary>
        /// 报废胶水代号
        /// </summary>
        public string scrap_glue_no { get; set; }
        /// <summary>
        /// 报废胶水原因
        /// </summary>
        public string scrap_glue_reason { get; set; }
        /// <summary>
        /// 报废胶水重量
        /// </summary>
        public string scrap_glue_weight { get; set; }
        /// <summary>
        /// 生产单位
        /// </summary>
        public string department_code { get; set; }
        /// <summary>
        /// 报废单位签名
        /// </summary>
        public string bf_autograph { get; set; }
        /// <summary>
        /// 环保股回收签名
        /// </summary>
        public string hb_autograph { get; set; }
    }

    //如果好用，请收藏地址，帮忙分享。
    public class GetScrapGlueMagRecordByIdRsp
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SCRAP_GLUE_NO { get; set; }
        public string SCRAP_GLUE_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SCRAP_GLUE_WEIGHT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SCRAP_GLUE_REASON { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BF_AUTOGRAPH { get; set; }
        /// <summary>
        /// 罗伟花
        /// </summary>
        public string BF_AUTOGRAPH_NAME { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string HB_AUTOGRAPH { get; set; }
        /// <summary>
        /// 谢志安
        /// </summary>
        public string HB_AUTOGRAPH_NAME { get; set; }
    }


    #endregion

}
