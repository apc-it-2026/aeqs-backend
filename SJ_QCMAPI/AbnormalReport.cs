using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 异常呈报单
    /// </summary>
    public class AbnormalReport
    {
        #region PDA 相关接口
        /// <summary>
        /// 创建异常呈报单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CreateAbnormalReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateAbnormalReportReqDto>(Data);

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string guid_img = Guid.NewGuid().ToString("N");// 保存图片
                Dictionary<string, object> insert_m_dic = new Dictionary<string, object>();
                insert_m_dic.Add("PROD_NO", requst.PROD_NO);
                insert_m_dic.Add("ART_IMG_URL", requst.ART_IMG_URL);
                insert_m_dic.Add("FW", requst.FW);
                insert_m_dic.Add("SHOE_NO", requst.SHOE_NO);
                insert_m_dic.Add("PRODUCTION_MONTH", requst.PRODUCTION_MONTH);
                insert_m_dic.Add("ORG_CODE", requst.ORG_CODE);
                insert_m_dic.Add("ORG_NAME", requst.ORG_NAME);
                insert_m_dic.Add("PRO_DEPARTMENT_NO", requst.PRO_DEPARTMENT_NO);
                insert_m_dic.Add("PRO_DEPARTMENT_NAME", requst.PRO_DEPARTMENT_NAME);
                insert_m_dic.Add("PRODUCTIONLINE_NO", requst.PRODUCTIONLINE_NO);
                insert_m_dic.Add("PRODUCTIONLINE_NAME", requst.PRODUCTIONLINE_NAME);
                insert_m_dic.Add("QUALITY_PROBLEM_LEVEL", requst.QUALITY_PROBLEM_LEVEL);
                insert_m_dic.Add("PROBLEM_DES", requst.PROBLEM_DES);
                insert_m_dic.Add("RESPONSIBLE_DEPARTMENT_NO", requst.RESPONSIBLE_DEPARTMENT_NO);
                insert_m_dic.Add("RESPONSIBLE_DEPARTMENT_NAME", requst.RESPONSIBLE_DEPARTMENT_NAME);
                insert_m_dic.Add("PROBLEM_DETAIL", requst.PROBLEM_DETAIL);
                insert_m_dic.Add("EMERGENCY_MEASURES", requst.EMERGENCY_MEASURES);
                insert_m_dic.Add("PROBLEM_REASON_STR", requst.PROBLEM_REASON_STR);
                insert_m_dic.Add("STATUS", "0");//默认未结案
                insert_m_dic.Add("PROBLEM_GUID_IMG", guid_img);

                insert_m_dic.Add("CREATEBY", userCode);
                insert_m_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                insert_m_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_ABNORMAL_REPORT_M", insert_m_dic);
                DB.ExecuteNonQuery(sql, insert_m_dic);

                if (requst.PROBLEM_IMG_LIST != null)
                {
                    foreach (string imgPath in requst.PROBLEM_IMG_LIST)
                    {
                        string fileName = imgPath.Split('/').LastOrDefault();
                        if (string.IsNullOrEmpty(fileName))
                            fileName = "";
                        DB.ExecuteNonQuery($@"insert into QCM_ABNORMAL_REPORT_IMAGEURL (img_name,img_url,guid,guid_img,createby,createdate,createtime) 
                    values('{fileName}','{imgPath}','','{guid_img}','{userCode}','{currDate:yyyy-MM-dd}','{currDate:HH:mm:ss}')");
                    }
                }

                DB.Commit();
                #endregion
                ret.ErrMsg = "保存成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "保存失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }

            return ret;

        }


        /// <summary>
        /// 异常呈报单查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchAbnormalReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string whereSql = $@" WHERE 1 = 1";

                string Data = ReqObj.Data.ToString();

                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchAbnormalReportReqDto>(Data);
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string pageIndex = requst.pageIndex;
                string pageSize = requst.pageSize;
                if (!string.IsNullOrEmpty(requst.PROBLEM_DES))
                    whereSql += $@"and PROBLEM_DES like '%{requst.PROBLEM_DES}%' ";
                if (!string.IsNullOrEmpty(requst.PROD_NO))
                    whereSql += $@"and PROD_NO like '%{requst.PROD_NO}%' ";
                if (!string.IsNullOrEmpty(requst.PRODUCTION_MONTH_START) && !string.IsNullOrEmpty(requst.PRODUCTION_MONTH_END))
                    whereSql += $@"and (PRODUCTION_MONTH >= '{requst.PRODUCTION_MONTH_START}' and PRODUCTION_MONTH <= '{requst.PRODUCTION_MONTH_END}') ";
                if (!string.IsNullOrEmpty(requst.ORG_CODE))
                    whereSql += $@"and ORG_CODE = '{requst.ORG_CODE}' ";
                if (!string.IsNullOrEmpty(requst.QUALITY_PROBLEM_LEVEL))
                    whereSql += $@"and QUALITY_PROBLEM_LEVEL = '{requst.QUALITY_PROBLEM_LEVEL}' ";
                if (!string.IsNullOrEmpty(requst.RESPONSIBLE_DEPARTMENT_NO))
                    whereSql += $@"and RESPONSIBLE_DEPARTMENT_NO = '{requst.RESPONSIBLE_DEPARTMENT_NO}' ";

                string orderSql = " ORDER BY ID DESC";

                #region 逻辑
                string sql = $@"
SELECT 
    ID,
	PROD_NO,
	RESPONSIBLE_DEPARTMENT_NAME,
	QUALITY_PROBLEM_LEVEL,
	CREATEDATE,
	STATUS
FROM QCM_ABNORMAL_REPORT_M 
";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql + whereSql + orderSql, int.Parse(pageIndex), int.Parse(pageSize));

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int total = CommonBASE.GetPageDataTableCount(DB, sql + whereSql);
                dic.Add("data", dt);
                dic.Add("total", total);
                ret.RetData1 = dic;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 根据id获取异常呈报单详细信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAbnormalReportById(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                if (string.IsNullOrEmpty(id))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数不能为空";
                    return ret;
                }

                #region 逻辑
                string sql = $@"
SELECT 
    *
FROM QCM_ABNORMAL_REPORT_M 
where ID={id}
";
                DataTable dt = DB.GetDataTable(sql);
                GetAbnormalReportByIdResDto res = dt.ToDataList<GetAbnormalReportByIdResDto>().FirstOrDefault();
                if (res != null)
                {
                    //0普通品质问题1严重品质问题2批量/重大品质问题
                    switch (res.QUALITY_PROBLEM_LEVEL)
                    {
                        case "0":
                            res.QUALITY_PROBLEM_LEVEL_STR = "普通品质问题";
                            break;
                        case "1":
                            res.QUALITY_PROBLEM_LEVEL_STR = "严重品质问题";
                            break;
                        case "2":
                            res.QUALITY_PROBLEM_LEVEL_STR = "批量/重大品质问题";
                            break;
                        default:
                            break;
                    }
                    switch (res.STATUS)
                    {
                        case "0":
                            res.STATUS_STR = "未结案";
                            break;
                        case "1":
                            res.STATUS_STR = "结案";
                            break;
                        case "2":
                            res.STATUS_STR = "公开";
                            break;
                        default:
                            break;
                    }

                    string imgSql = $@"SELECT IMG_URL FROM QCM_ABNORMAL_REPORT_IMAGEURL WHERE GUID_IMG='{res.PROBLEM_GUID_IMG}'";
                    DataTable imgDt = DB.GetDataTable(imgSql);
                    List<string> programImgList = new List<string>();
                    foreach (DataRow imgItem in imgDt.Rows)
                    {
                        programImgList.Add(imgItem["IMG_URL"].ToString());
                    }
                    res.PROBLEM_IMG_LIST = programImgList;

                    imgSql = $@"SELECT IMG_URL FROM QCM_ABNORMAL_REPORT_IMAGEURL WHERE GUID_IMG='{res.SOLVE_GUID_IMG}'";
                    imgDt = DB.GetDataTable(imgSql);
                    List<string> solveImgList = new List<string>();
                    foreach (DataRow imgItem in imgDt.Rows)
                    {
                        solveImgList.Add(imgItem["IMG_URL"].ToString());
                    }
                    res.SOLVE_IMG_LIST = solveImgList;
                }

                #endregion
                ret.RetData1 = res;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 根据id更新异常呈报单为结案状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChangeAbnormalReportSTATUS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string STATUS_PARAMS = jarr.ContainsKey("STATUS_PARAMS") ? jarr["STATUS_PARAMS"].ToString() : "1";//不传，默认为1结案状态
                if (string.IsNullOrEmpty(id))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数不能为空";
                    return ret;
                }

                #region 逻辑

                DB.Open();
                DB.BeginTransaction();
                string sql = $@"select STATUS from QCM_ABNORMAL_REPORT_M where ID={id}";
                var dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    DataRow dataRow = dt.Rows[0];
                    string STATUS = dataRow["STATUS"].ToString();
                    List<string> statusList = new List<string>();
                    if (STATUS == "1" && STATUS == STATUS_PARAMS)
                    {
                        statusList = new List<string>()
                        {
                            "1","2"
                        };
                        if (statusList.Contains(STATUS_PARAMS))
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "异常呈报单已结案";
                            return ret;
                        }
                    }
                    else if (STATUS_PARAMS == "2")
                    {
                        statusList = new List<string>()
                        {
                            "2"
                        };
                        if (statusList.Contains(STATUS_PARAMS))
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "异常呈报单已公开";
                            return ret;
                        }
                    }
                    DateTime currDate = DateTime.Now;
                    string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                    Dictionary<string, object> update_val_dic = new Dictionary<string, object>();
                    update_val_dic.Add("STATUS", STATUS_PARAMS);

                    update_val_dic.Add("MODIFYBY", userCode);
                    update_val_dic.Add("MODIFYDATE", currDate.ToString("yyyy-MM-dd"));
                    update_val_dic.Add("MODIFYTIME", currDate.ToString("HH:mm:ss"));

                    string whereSql = $@"ID=@ID";
                    string updateSql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("QCM_ABNORMAL_REPORT_M", whereSql, update_val_dic);
                    update_val_dic.Add("ID", id);

                    DB.ExecuteNonQuery(updateSql, update_val_dic);

                    DB.Commit();
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "异常呈报单不存在";
                    return ret;
                }

                #endregion
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }

        /// <summary>
        /// 根据id更新异常呈报单 解决信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateAbnormalReportById(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateAbnormalReportByIdReqDto>(Data);

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                string sql = $@"select SOLVE_GUID_IMG from QCM_ABNORMAL_REPORT_M where ID={requst.ID}";
                var dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string guid_img = Guid.NewGuid().ToString("N");// 保存图片
                    DataRow dataRow = dt.Rows[0];
                    string SOLVE_GUID_IMG = dataRow["SOLVE_GUID_IMG"].ToString();

                    DateTime currDate = DateTime.Now;
                    string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                    Dictionary<string, object> update_val_dic = new Dictionary<string, object>();
                    update_val_dic.Add("SOLVE_GUID_IMG", guid_img);
                    update_val_dic.Add("MEASURES", requst.MEASURES);

                    update_val_dic.Add("MODIFYBY", userCode);
                    update_val_dic.Add("MODIFYDATE", currDate.ToString("yyyy-MM-dd"));
                    update_val_dic.Add("MODIFYTIME", currDate.ToString("HH:mm:ss"));

                    string whereSql = $@"ID=@ID";
                    string updateSql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("QCM_ABNORMAL_REPORT_M", whereSql, update_val_dic);
                    update_val_dic.Add("ID", requst.ID);

                    DB.ExecuteNonQuery(updateSql, update_val_dic);

                    if (requst.SOLVE_IMG_LIST != null)
                    {
                        foreach (string imgPath in requst.SOLVE_IMG_LIST)
                        {
                            string fileName = imgPath.Split('/').LastOrDefault();
                            if (string.IsNullOrEmpty(fileName))
                                fileName = "";
                            DB.ExecuteNonQuery($@"insert into QCM_ABNORMAL_REPORT_IMAGEURL (img_name,img_url,guid,guid_img,createby,createdate,createtime) 
                    values('{fileName}','{imgPath}','','{guid_img}','{userCode}','{currDate:yyyy-MM-dd}','{currDate:HH:mm:ss}')");
                        }
                    }

                    DB.Commit();
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "异常呈报单不存在";
                    return ret;
                }

                #endregion
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }
        #endregion

        #region CS端接口

        /// <summary>
        /// 异常呈报单查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchAbnormalReportCS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string whereSql = $@" WHERE 1 = 1 ";

                string Data = ReqObj.Data.ToString();

                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<SearchAbnormalReportCSReqDto>(Data);
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string pageIndex = requst.pageIndex;
                string pageSize = requst.pageSize;
                if (!string.IsNullOrEmpty(requst.PROBLEM_DES))
                    whereSql += $@"and PROBLEM_DES like '%{requst.PROBLEM_DES}%' ";
                if (!string.IsNullOrEmpty(requst.PROD_NO))
                    whereSql += $@"and PROD_NO like '%{requst.PROD_NO}%' ";
                if (!string.IsNullOrEmpty(requst.PRODUCTION_MONTH_START) && !string.IsNullOrEmpty(requst.PRODUCTION_MONTH_END))
                    whereSql += $@"and (PRODUCTION_MONTH >= '{requst.PRODUCTION_MONTH_START}' and PRODUCTION_MONTH <= '{requst.PRODUCTION_MONTH_END}') ";

                if (!string.IsNullOrEmpty(requst.PRODUCTION_MONTH_START) && string.IsNullOrEmpty(requst.PRODUCTION_MONTH_END))
                    whereSql += $@"and PRODUCTION_MONTH = '{requst.PRODUCTION_MONTH_START}' ";
                if (string.IsNullOrEmpty(requst.PRODUCTION_MONTH_START) && !string.IsNullOrEmpty(requst.PRODUCTION_MONTH_END))
                    whereSql += $@"and PRODUCTION_MONTH = '{requst.PRODUCTION_MONTH_END}' ";

                if (!string.IsNullOrEmpty(requst.ORG_CODE))
                    whereSql += $@"and ORG_CODE = '{requst.ORG_CODE}' ";
                if (!string.IsNullOrEmpty(requst.QUALITY_PROBLEM_LEVEL))
                    whereSql += $@"and QUALITY_PROBLEM_LEVEL = '{requst.QUALITY_PROBLEM_LEVEL}' ";
                if (!string.IsNullOrEmpty(requst.RESPONSIBLE_DEPARTMENT_NO))
                    whereSql += $@"and RESPONSIBLE_DEPARTMENT_NO = '{requst.RESPONSIBLE_DEPARTMENT_NO}' ";
                if (!string.IsNullOrEmpty(requst.PRO_DEPARTMENT_NO))
                    whereSql += $@"and PRO_DEPARTMENT_NO = '{requst.PRO_DEPARTMENT_NO}' ";

                string orderSql = " ORDER BY ID DESC";

                #region 逻辑
                string sql = $@"
SELECT 
    ID,
	PROD_NO,
	PRO_DEPARTMENT_NAME,
	RESPONSIBLE_DEPARTMENT_NAME,
	QUALITY_PROBLEM_LEVEL,
	PRODUCTION_MONTH,
	PROBLEM_DES,
	STATUS,
    PROBLEM_GUID_IMG,
    SOLVE_GUID_IMG
FROM QCM_ABNORMAL_REPORT_M 
";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql + whereSql + orderSql, int.Parse(pageIndex), int.Parse(pageSize));

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int total = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("data", dt);
                dic.Add("total", total);
                ret.RetData1 = dic;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }


        /// <summary>
        /// 员工列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchHR001CS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;

                sql = $@"select STAFF_NO,STAFF_NAME,STAFF_DEPARTMENT from hr001m";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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

        /// <summary>
        /// 问题等级接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchProblemLevel(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string sql = $@"select ENUM_CODE,ENUM_VALUE from sys001m where ENUM_TYPE='enum_quality_problem_level'";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = DB.GetDataTable(sql);
                dic.Add("Data", dt);

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

        #endregion

    }
    #region dto
    public class CreateAbnormalReportReqDto
    {
        /// <summary>
        /// ART
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// ART图片路径
        /// </summary>
        public string ART_IMG_URL { get; set; }
        /// <summary>
        /// FW21
        /// </summary>
        public string FW { get; set; }
        /// <summary>
        /// 鞋型
        /// </summary>
        public string SHOE_NO { get; set; }
        /// <summary>
        /// 生产月份
        /// </summary>
        public string PRODUCTION_MONTH { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public string ORG_CODE { get; set; }
        /// <summary>
        /// 厂区名称
        /// </summary>
        public string ORG_NAME { get; set; }
        /// <summary>
        /// 生产工段
        /// </summary>
        public string PRO_DEPARTMENT_NO { get; set; }
        /// <summary>
        /// 生产工段名称
        /// </summary>
        public string PRO_DEPARTMENT_NAME { get; set; }
        /// <summary>
        /// 生产线代号
        /// </summary>
        public string PRODUCTIONLINE_NO { get; set; }
        /// <summary>
        /// 生产线名称
        /// </summary>
        public string PRODUCTIONLINE_NAME { get; set; }
        /// <summary>
        /// 品质问题级别 0普通品质问题1严重品质问题2批量/重大品质问题
        /// </summary>
        public string QUALITY_PROBLEM_LEVEL { get; set; }
        /// <summary>
        /// 问题描述
        /// </summary>
        public string PROBLEM_DES { get; set; }
        /// <summary>
        /// 责任部门代号
        /// </summary>
        public string RESPONSIBLE_DEPARTMENT_NO { get; set; }
        /// <summary>
        /// 责任部门名称
        /// </summary>
        public string RESPONSIBLE_DEPARTMENT_NAME { get; set; }
        /// <summary>
        /// 问题详情
        /// </summary>
        public string PROBLEM_DETAIL { get; set; }
        /// <summary>
        /// 紧急处理措施
        /// </summary>
        public string EMERGENCY_MEASURES { get; set; }
        /// <summary>
        /// 问题图片集合
        /// </summary>
        public List<string> PROBLEM_IMG_LIST { get; set; }
        /// <summary>
        /// 问题原因
        /// </summary>
        public string PROBLEM_REASON_STR { get; set; }
    }

    public class GetAbnormalReportByIdResDto: CreateAbnormalReportReqDto
    {
        /// <summary>
        /// 解决后图片
        /// </summary>
        public List<string> SOLVE_IMG_LIST { get; set; }
        /// <summary>
        /// 措施
        /// </summary>
        public string MEASURES { get; set; }
        /// <summary>
        /// 问题图片关联键
        /// </summary>
        public string PROBLEM_GUID_IMG { get; set; }
        /// <summary>
        /// 解决后图片关联键
        /// </summary>
        public string SOLVE_GUID_IMG { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string STATUS { get; set; }
        /// <summary>
        /// 状态转义
        /// </summary>
        public string STATUS_STR { get; set; }
        /// <summary>
        /// 品质问题级别转义 0普通品质问题1严重品质问题2批量/重大品质问题
        /// </summary>
        public string QUALITY_PROBLEM_LEVEL_STR { get; set; }
    }

    public class UpdateAbnormalReportByIdReqDto
    {
        public string ID { get; set; }
        /// <summary>
        /// 解决后图片
        /// </summary>
        public List<string> SOLVE_IMG_LIST { get; set; }
        /// <summary>
        /// 措施
        /// </summary>
        public string MEASURES { get; set; }
    }

    public class SearchAbnormalReportReqDto
    {
        /// <summary>
        /// 页码
        /// </summary>
        public string pageIndex { get; set; }
        /// <summary>
        /// 每页行数
        /// </summary>
        public string pageSize { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string PROBLEM_DES { get; set; }
        /// <summary>
        /// ART
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// 日期范围 开始
        /// </summary>
        public string PRODUCTION_MONTH_START { get; set; }
        /// <summary>
        /// 日期范围 结束
        /// </summary>
        public string PRODUCTION_MONTH_END { get; set; }
        /// <summary>
        /// 厂区
        /// </summary>
        public string ORG_CODE { get; set; }
        /// <summary>
        /// 问题级别
        /// </summary>
        public string QUALITY_PROBLEM_LEVEL { get; set; }
        /// <summary>
        /// 责任部门
        /// </summary>
        public string RESPONSIBLE_DEPARTMENT_NO { get; set; }
    }

    public class SearchAbnormalReportCSReqDto: SearchAbnormalReportReqDto
    {
        /// <summary>
        /// 发生部门（生产工段）
        /// </summary>
        public string PRO_DEPARTMENT_NO { get; set; }
    }

    #endregion

}
