using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class CustomerComplaint
    {
        /// <summary>
        /// 获取投诉列表信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomerComplaintList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SPOTCHECK_DATE_START = jarr.ContainsKey("SPOTCHECK_DATE_START") ? jarr["SPOTCHECK_DATE_START"].ToString() : "";//检验开始日期
                string SPOTCHECK_DATE_END = jarr.ContainsKey("SPOTCHECK_DATE_END") ? jarr["SPOTCHECK_DATE_END"].ToString() : "";//检验结束日期
                //string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//PO
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//art
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//PO
                
                
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	COMPLAINT_NO, -- 单号
	COMPLAINT_DATE, -- 投诉日期
	COUNTRY_REGION, -- 区域
	PO_ORDER, -- po号
	DEVELOP_SEASON, -- 开发季度
	CATEGORY,
	DEVELOPMENT_COURSE, -- 开发课
	PRODUCT_MONTH, -- 量产月份
	PROD_NO, -- art
	SHOE_NO,-- 鞋型
	MATERIAL_WAY, -- MATERIAL_WAY
	PRODUCTIONLINE_NO,-- 产线代号
	PRODUCTIONLINE_NAME,-- 产线名称
	NG_QTY, -- 不良数量
	COMPLAINT_MONEY, -- 投诉金额
	DEFECT_CONTENT -- 问题点
FROM
	QCM_CUSTOMER_COMPLAINT_M
WHERE
	1 = 1 ";

                if (!string.IsNullOrEmpty(SPOTCHECK_DATE_START) && !string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                    sql += $@"AND ( SPOTCHECK_DATE BETWEEN '{SPOTCHECK_DATE_START}' AND '{SPOTCHECK_DATE_END}') ";
                if (string.IsNullOrEmpty(SPOTCHECK_DATE_START) && !string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                {
                    sql += $@"AND ( COMPLAINT_DATE BETWEEN '1970-01-01' AND '{SPOTCHECK_DATE_END}')";
                }
                if (!string.IsNullOrEmpty(SPOTCHECK_DATE_START) && string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                {
                    sql += $@"AND ( COMPLAINT_DATE BETWEEN '{SPOTCHECK_DATE_START}' AND '3000-01-01')";
                }

                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    sql += $@"AND  PROD_NO LIKE '%{PROD_NO}%'";
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    sql += $@"AND  SHOE_NO LIKE '%{SHOE_NO}%'";
                }
               
                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }

        /// <summary>
        /// 新增客户投诉内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddCustomerComplaint(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单
                string COMPLAINT_DATE = jarr.ContainsKey("COMPLAINT_DATE") ? jarr["COMPLAINT_DATE"].ToString() : "";
                string COUNTRY_REGION = jarr.ContainsKey("COUNTRY_REGION") ? jarr["COUNTRY_REGION"].ToString() : "";

                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string NG_QTY = jarr.ContainsKey("NG_QTY") ? jarr["NG_QTY"].ToString() : "";

                string COMPLAINT_MONEY = jarr.ContainsKey("COMPLAINT_MONEY") ? jarr["COMPLAINT_MONEY"].ToString() : "";
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";

                string PRODUCT_MONTH = jarr.ContainsKey("PRODUCT_MONTH") ? jarr["PRODUCT_MONTH"].ToString() : "";
                string PRODUCTIONLINE_NO = jarr.ContainsKey("PRODUCTIONLINE_NO") ? jarr["PRODUCTIONLINE_NO"].ToString() : "";
                string PRODUCTIONLINE_NAME = jarr.ContainsKey("PRODUCTIONLINE_NAME") ? jarr["PRODUCTIONLINE_NAME"].ToString() : "";
                string MATERIAL_WAY = jarr.ContainsKey("MATERIAL_WAY") ? jarr["MATERIAL_WAY"].ToString() : "";
                string DEFECT_CONTENT = jarr.ContainsKey("DEFECT_CONTENT") ? jarr["DEFECT_CONTENT"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                #endregion

                #region 逻辑

                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string CREATEDATE = DateTime.Now.ToString("yyyy-MM-dd");
                string CREATETIME = DateTime.Now.ToString("HH:mm:ss");
                //string PO_QTY = DB.GetString($@"SELECT PO_QTY FROM QCM_PO_ORDER WHERE PORD_NO = '{PO_ORDER}'");

                #region 生成检验单

                string COMPLAINT_NO = "TS" +  DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001

                string max_COMPLAINT_NO = DB.GetString($@"select max(COMPLAINT_NO) from QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO like '{COMPLAINT_NO}%'");

                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(max_COMPLAINT_NO))
                {
                    string seq = max_COMPLAINT_NO.Replace(COMPLAINT_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    COMPLAINT_NO = COMPLAINT_NO + int_seq.ToString().PadLeft(5, '0');

                }
                else
                {
                    COMPLAINT_NO = COMPLAINT_NO + "00001";
                }

                #endregion

                Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                insert_dic.Add("COMPLAINT_NO", COMPLAINT_NO);
                insert_dic.Add("COMPLAINT_DATE", COMPLAINT_DATE);

                insert_dic.Add("COUNTRY_REGION", COUNTRY_REGION);
                insert_dic.Add("PO_ORDER", PO_ORDER);
                insert_dic.Add("NG_QTY", NG_QTY);

                insert_dic.Add("COMPLAINT_MONEY", COMPLAINT_MONEY);
                insert_dic.Add("DEVELOP_SEASON", DEVELOP_SEASON);
                insert_dic.Add("PRODUCT_MONTH", PRODUCT_MONTH);

                insert_dic.Add("PRODUCTIONLINE_NO", PRODUCTIONLINE_NO);
                insert_dic.Add("PRODUCTIONLINE_NAME", PRODUCTIONLINE_NAME);

                insert_dic.Add("MATERIAL_WAY", MATERIAL_WAY);
                insert_dic.Add("DEFECT_CONTENT", DEFECT_CONTENT);
                insert_dic.Add("PROD_NO", PROD_NO);

                insert_dic.Add("CREATEBY", CREATEBY);
                insert_dic.Add("CREATEDATE", CREATEDATE);
                insert_dic.Add("CREATETIME", CREATETIME);

                

                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_CUSTOMER_COMPLAINT_M", insert_dic);
                DB.ExecuteNonQuery(sql, insert_dic);

                DB.Commit();
                #endregion
                ret.ErrMsg = "录入成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "录入失败！ " + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;

        }

        /// <summary>
        /// 查看照片(url,name)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPhotoImgList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//检验单
                //string TESTITEM_CODE = jarr.ContainsKey("TESTITEM_CODE") ? jarr["TESTITEM_CODE"].ToString() : "";//检测项编号

                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	IMG_URL,
	IMG_NAME,
	TYPE
FROM
	QCM_CUSTOMER_COMPLAINT_FILE
WHERE
	1 = 1 ";

                if (!string.IsNullOrEmpty(COMPLAINT_NO))
                    sql += $@"AND COMPLAINT_NO = '{COMPLAINT_NO}' ";

                sql += $@" AND TYPE = '1'";

                //查询分页数据
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }

        /// <summary>
        /// 保存文件/图片(url,name)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SavePhotoImgList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//检验单
                string IMG_NAME = jarr.ContainsKey("IMG_NAME") ? jarr["IMG_NAME"].ToString() : "";//图片名称
                string IMG_URL = jarr.ContainsKey("IMG_URL") ? jarr["IMG_URL"].ToString() : "";//图片路径
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";//GUID
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";// 1-图片 2-附件


                #endregion

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                var userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{ReqObj.UserToken}'");
                string createby = userDIC["UserCode"].ToString();
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");
                //DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_CUSTOMER_COMPLAINT_FILE (COMPLAINT_NO,IMG_NAME,IMG_URL,GUID,TYPE,CREATEBY,CREATEDATE,CREATETIME)VALUES('{COMPLAINT_NO}','{IMG_NAME}','{IMG_URL}','{GUID}','{TYPE}','{createby}','{createdate}','{createtime}')");
                DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_CUSTOMER_COMPLAINT_FILE (COMPLAINT_NO,IMG_NAME,IMG_URL,GUID,TYPE,CREATEBY,CREATEDATE,CREATETIME)VALUES('{COMPLAINT_NO}','{IMG_NAME}','{IMG_URL}','{GUID}','{TYPE}','{createby}','{createdate}','{createtime}')");

                DB.Commit();
                ret.ErrMsg = "保存成功";
                ret.IsSuccess = true;
                //if (DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_CUSTOMER_COMPLAINT_FILE (COMPLAINT_NO,IMG_NAME,IMG_URL,GUID,TYPE,CREATEBY,CREATEDATE,CREATETIME)VALUES('{COMPLAINT_NO}','{IMG_NAME}','{IMG_URL}','{GUID}','{TYPE}','{createby}','{createdate}','{createtime}')") > 0)
                //{
                //    DB.Commit();
                //    ret.ErrMsg = "保存成功";
                //    ret.IsSuccess = true;
                //}
                //else
                //{
                //    ret.ErrMsg = "保存失败，原因：" + ex.Message;
                //    ret.IsSuccess = false;
                //}
                #endregion

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


        #region 投诉明细

        /// <summary>
        /// 获取投诉明细表头信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomerComplaintDetail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//投诉编号

                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	COMPLAINT_NO, -- 单号
	COMPLAINT_DATE, -- 投诉日期
	COUNTRY_REGION, -- 区域
	PO_ORDER, -- po号
	DEVELOP_SEASON, -- 开发季度
	CATEGORY,
	DEVELOPMENT_COURSE, -- 开发课
	PRODUCT_MONTH, -- 量产月份
	PROD_NO, -- art
	SHOE_NO,-- 鞋型
	MATERIAL_WAY, -- MATERIAL_WAY
	PRODUCTIONLINE_NO,-- 产线代号
	PRODUCTIONLINE_NAME,-- 产线名称
	NG_QTY, -- 不良数量
	COMPLAINT_MONEY, -- 投诉金额
	DEFECT_CONTENT -- 问题点
FROM
	QCM_CUSTOMER_COMPLAINT_M
WHERE
	1 = 1 ";

                

                if (!string.IsNullOrEmpty(COMPLAINT_NO))
                {
                    sql += $@"AND  COMPLAINT_NO = '{COMPLAINT_NO}'";
                }

                DataTable dt = DB.GetDataTable(sql);
                ////查询分页数据
                //DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("rowCount", rowCount);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }

        /// <summary>
        /// 获取投诉列表明细信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomerComplaintDetailList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//投诉编号

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
CAUSE_ANALYSIS,-- 原因分析
QCM_CUSTOMER_COMPLAINT_D.RESPONSIBILITY_JUDGMENT, -- 责任判定
QCM_CUSTOMER_COMPLAINT_D.IMPROVEMENT_ACTION, -- 改善行动
QCM_CUSTOMER_COMPLAINT_D.CONCLUSION -- 结案结论
FROM
	QCM_CUSTOMER_COMPLAINT_D

WHERE
	1 = 1 ";



                if (!string.IsNullOrEmpty(COMPLAINT_NO))
                {
                    sql += $@"AND  COMPLAINT_NO = '{COMPLAINT_NO}'";
                }

                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }

        #endregion
    }
}
