using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class SpotCheck
    {
        /// <summary>
        /// 获取抽检列表内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSpotCheckList(object OBJ)
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
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	SPOTCHECK_NO, -- 检验单号
	INSPECT_METHOD,	-- 检验方式
	VEND_NO, -- 厂商代号
	VEND_NAME, -- 厂商名称
	PART_NO, -- 部件
	SHOE_NOS, -- 鞋型名称
	PROD_NO, -- ART
	PO_ORDER, -- PO单号
	CODE_NUMBER, -- 码数
	SPOTCHECK_DATE, -- 检验日期
	PO_QTY, -- 生产数量
	PLANSAMP_QTY, -- 计划抽检数
	PROCESS_TYPE, -- 工艺类型
	NG_QTY,-- 不良数
	STATUS -- 状态
FROM
	QCM_SPOTCHECK_TASK_M
WHERE 
    1 =1 ";

                if (!string.IsNullOrEmpty(SPOTCHECK_DATE_START) && !string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                    sql += $@"AND ( SPOTCHECK_DATE BETWEEN '{SPOTCHECK_DATE_START}' AND '{SPOTCHECK_DATE_END}') ";
                if (string.IsNullOrEmpty(SPOTCHECK_DATE_START) && !string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                {
                    sql += $@"AND ( SPOTCHECK_DATE BETWEEN '1970-01-01' AND '{SPOTCHECK_DATE_END}')";
                }
                if (!string.IsNullOrEmpty(SPOTCHECK_DATE_START) && string.IsNullOrEmpty(SPOTCHECK_DATE_END))
                {
                    sql += $@"AND ( SPOTCHECK_DATE BETWEEN '{SPOTCHECK_DATE_START}' AND '3000-01-01')";
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
        /// 获取抽检列表明细 (头)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSpotCheckDetail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	QCM_SPOTCHECK_TASK_M.VEND_NO, -- 厂商代号
	QCM_SPOTCHECK_TASK_M.VEND_NAME, -- 厂商名称
	QCM_SPOTCHECK_TASK_M.PART_NO, -- 部件
	QCM_SPOTCHECK_TASK_M.SHOE_NOS, -- 鞋型名称
	QCM_SPOTCHECK_TASK_M.PROD_NO, -- ART
	QCM_SPOTCHECK_TASK_M.PO_ORDER, -- PO单号
	QCM_SPOTCHECK_TASK_M.SPOTCHECK_DATE, -- 检验日期
	QCM_SPOTCHECK_TASK_M.PO_QTY, -- 生产数量
	QCM_SPOTCHECK_TASK_M.PLANSAMP_QTY, -- 计划抽检数
	QCM_SPOTCHECK_TASK_M.PROCESS_TYPE -- 工艺类型
FROM
	QCM_SPOTCHECK_TASK_M
-- LEFT JOIN QCM_SPOTCHECK_TASK_D ON QCM_SPOTCHECK_TASK_M.SPOTCHECK_NO = QCM_SPOTCHECK_TASK_D.SPOTCHECK_NO
WHERE 
	1 = 1  ";

                if (!string.IsNullOrEmpty(SPOTCHECK_NO))
                    sql += $@"AND SPOTCHECK_NO = '{SPOTCHECK_NO}' ";

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
        /// 获取抽检列表明细 (列表)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSpotCheckDetailList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion

                #region 逻辑

                var sql = $@"
SELECT
	QCM_SPOTCHECK_TASK_D.TESTITEM_CODE,-- 检测项编号
	QCM_SPOTCHECK_TASK_D.TESTITEM_NAME, -- 检测项名称
	QCM_SPOTCHECK_TASK_D.TEST_STANDARD, -- 检验标准
	QCM_SPOTCHECK_TASK_D.TEST_QTY, -- 抽检数量
	QCM_SPOTCHECK_TASK_D.AQL_LEVEL, -- QAL级别
	QCM_SPOTCHECK_TASK_D.DEFECT_CONTENT, -- 问题点
    QCM_SPOTCHECK_TASK_D.BAD_QTY,-- 报废数量
	QCM_SPOTCHECK_TASK_D.NG_QTY, -- 不良数量
	QCM_SPOTCHECK_TASK_D.CHECK_RESULT, --检验结果
	QCM_SPOTCHECK_TASK_D.REMARKS -- 备注
FROM
	QCM_SPOTCHECK_TASK_D
WHERE 
	1 = 1  ";

                if (!string.IsNullOrEmpty(SPOTCHECK_NO))
                    sql += $@"AND SPOTCHECK_NO = '{SPOTCHECK_NO}' ";

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
        /// 新增抽检内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddSpotCheck(object OBJ)
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
                string VEND_NAME = jarr.ContainsKey("VEND_NAME") ? jarr["VEND_NAME"].ToString() : "";
                string INSPECT_METHOD = jarr.ContainsKey("INSPECT_METHOD") ? jarr["INSPECT_METHOD"].ToString() : "";
                string PART_NO = jarr.ContainsKey("PART_NO") ? jarr["PART_NO"].ToString() : "";
                string SHOE_NOS = jarr.ContainsKey("SHOE_NOS") ? jarr["SHOE_NOS"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string CODE_NUMBER = jarr.ContainsKey("CODE_NUMBER") ? jarr["CODE_NUMBER"].ToString() : "";
                string PO_QTY = jarr.ContainsKey("PO_QTY") ? jarr["PO_QTY"].ToString() : "";
                string PLANSAMP_QTY = jarr.ContainsKey("PLANSAMP_QTY") ? jarr["PLANSAMP_QTY"].ToString() : "";
                string PROCESS_TYPE = jarr.ContainsKey("PROCESS_TYPE") ? jarr["PROCESS_TYPE"].ToString() : "";
                #endregion

                #region 逻辑

                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string CREATEDATE = DateTime.Now.ToString("yyyy-MM-dd");
                string CREATETIME = DateTime.Now.ToString("HH:mm:ss");
                //string PO_QTY = DB.GetString($@"SELECT PO_QTY FROM QCM_PO_ORDER WHERE PORD_NO = '{PO_ORDER}'");

                #region 生成检验单

                string SPOTCHECK_NO =  DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001

                string max_inspection_no =DB.GetString($@"select max(SPOTCHECK_NO) from QCM_SPOTCHECK_TASK_M where SPOTCHECK_NO like '{SPOTCHECK_NO}%'") ;

                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(SPOTCHECK_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    SPOTCHECK_NO = SPOTCHECK_NO + int_seq.ToString().PadLeft(5, '0');

                }
                else
                {
                    SPOTCHECK_NO = SPOTCHECK_NO + "00001";
                }

                #endregion

                Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                insert_dic.Add("SPOTCHECK_NO", SPOTCHECK_NO);
                insert_dic.Add("VEND_NAME", VEND_NAME);
                insert_dic.Add("INSPECT_METHOD", INSPECT_METHOD);
                insert_dic.Add("PART_NO", PART_NO);
                insert_dic.Add("SHOE_NOS", SHOE_NOS);
                insert_dic.Add("PROD_NO", PROD_NO);
                insert_dic.Add("PO_ORDER", PO_ORDER);
                insert_dic.Add("SPOTCHECK_DATE", DateTime.Now.ToString("yyyy-MM-dd"));
                insert_dic.Add("CODE_NUMBER", CODE_NUMBER);
                insert_dic.Add("PO_QTY", PO_QTY);
                insert_dic.Add("CREATEBY", CREATEBY);
                insert_dic.Add("CREATEDATE", CREATEDATE);
                insert_dic.Add("CREATETIME", CREATETIME);

                insert_dic.Add("PLANSAMP_QTY", PLANSAMP_QTY);
                insert_dic.Add("PROCESS_TYPE", PROCESS_TYPE);

                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_SPOTCHECK_TASK_M", insert_dic);
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
        /// 编辑抽检内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateSpotCheck(object OBJ)
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
                string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";
                string VEND_NAME = jarr.ContainsKey("VEND_NAME") ? jarr["VEND_NAME"].ToString() : "";

                string PART_NO = jarr.ContainsKey("PART_NO") ? jarr["PART_NO"].ToString() : "";//部件
                string SHOE_NOS = jarr.ContainsKey("SHOE_NOS") ? jarr["SHOE_NOS"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";
                string SPOTCHECK_DATE = jarr.ContainsKey("SPOTCHECK_DATE") ? jarr["SPOTCHECK_DATE"].ToString() : "";//检验日期
                
                string PO_QTY = jarr.ContainsKey("PO_QTY") ? jarr["PO_QTY"].ToString() : "";
                string PLANSAMP_QTY = jarr.ContainsKey("PLANSAMP_QTY") ? jarr["PLANSAMP_QTY"].ToString() : "";
                string PROCESS_TYPE = jarr.ContainsKey("PROCESS_TYPE") ? jarr["PROCESS_TYPE"].ToString() : "";
                #endregion

                #region 逻辑

                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string CREATEDATE = DateTime.Now.ToString("yyyy-MM-dd");
                string CREATETIME = DateTime.Now.ToString("HH:mm:ss");

               

                Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                insert_dic.Add("SPOTCHECK_NO", SPOTCHECK_NO);
                insert_dic.Add("VEND_NO", VEND_NO);
                insert_dic.Add("VEND_NAME", VEND_NAME);

                insert_dic.Add("PART_NO", PART_NO);
                insert_dic.Add("SHOE_NOS", SHOE_NOS);
                insert_dic.Add("PROD_NO", PROD_NO);

                insert_dic.Add("PO_ORDER", PO_ORDER);
                insert_dic.Add("SPOTCHECK_DATE", SPOTCHECK_DATE);
                insert_dic.Add("PO_QTY", PO_QTY);
                insert_dic.Add("PLANSAMP_QTY", PLANSAMP_QTY);
                insert_dic.Add("PROCESS_TYPE", PROCESS_TYPE);

                insert_dic.Add("CREATEBY", CREATEBY);
                insert_dic.Add("CREATEDATE", CREATEDATE);
                insert_dic.Add("CREATETIME", CREATETIME);


                string WHERE = $@" SPOTCHECK_NO = '{SPOTCHECK_NO}'";
                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("QCM_SPOTCHECK_TASK_M", WHERE, insert_dic);
                DB.ExecuteNonQuery(sql, insert_dic);

                DB.Commit();
                #endregion
                ret.ErrMsg = "编辑成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "编辑失败！ " + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;

        }

        /// <summary>
        /// 查看图片(url,name)
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
                string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单
                string TESTITEM_CODE = jarr.ContainsKey("TESTITEM_CODE") ? jarr["TESTITEM_CODE"].ToString() : "";//检测项编号
                
                #endregion

                #region 逻辑

                var sql = $@"SELECT IMG_NAME,IMG_URL FROM QCM_SPOTCHECK_TASK_IMAGE WHERE 1 = 1  ";

                if (!string.IsNullOrEmpty(SPOTCHECK_NO))
                    sql += $@"AND SPOTCHECK_NO = '{SPOTCHECK_NO}' ";
                if (!string.IsNullOrEmpty(TESTITEM_CODE))
                    sql += $@"AND TESTITEM_CODE = '{TESTITEM_CODE}' ";

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
    }
}
