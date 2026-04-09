using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;

namespace SJ_QCMAPI
{
    public class ExternalColorCard
    {
        /// <summary>
        /// 获取发外色卡列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束日期
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//厂商名称
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                #endregion


                #region 逻辑

                var sql = $@"
SELECT
	CARD_DATE,-- 日期
    VEND_NO, -- 编号
	VEND_NAME,-- 厂商名称
	FIRSTARTICLE_TYPE,-- 首件确认种类
    SHOE_NO,--鞋型
	PROD_NO, -- ART
	IS_QCCONFIRM,-- QC确认
	TEST_RESULT -- 测试状况
FROM
	QCM_COLOR_CARD_M
WHERE
	1 = 1";
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    sql += $@"AND ( QCM_COLOR_CARD_M.CARD_DATE BETWEEN '{start_date}' AND '{end_date}') ";
                if (!string.IsNullOrEmpty(VEND_NO))
                    sql += $@"AND VEND_NO LIKE '%{VEND_NO}%'";
                if (!string.IsNullOrEmpty(PROD_NO))
                    sql += $@"AND PROD_NO LIKE '%{PROD_NO}'%";

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
        /// 获取发外色卡M表头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorHead(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CARD_DATE = jarr.ContainsKey("CARD_DATE") ? jarr["CARD_DATE"].ToString() : "";//日期
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//厂商名称
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                #endregion


                #region 逻辑

                var sql = $@"
SELECT
	CARD_DATE,-- 日期
    VEND_NO, -- 编号
	VEND_NAME,-- 厂商名称
	FIRSTARTICLE_TYPE,-- 首件确认种类
    SHOE_NO,--鞋型
	PROD_NO, -- ART
	IS_QCCONFIRM,-- QC确认
	TEST_RESULT -- 测试状况
FROM
	QCM_COLOR_CARD_M
WHERE
	1 = 1 ";
                if (!string.IsNullOrEmpty(VEND_NO))
                    sql += $@"AND VEND_NO = '{VEND_NO}'";
                if (!string.IsNullOrEmpty(SHOE_NO))
                    sql += $@"AND SHOE_NO = '{SHOE_NO}'";
                if (!string.IsNullOrEmpty(CARD_DATE))
                    sql += $@"AND CARD_DATE = '{CARD_DATE}'";
                if (!string.IsNullOrEmpty(PROD_NO))
                    sql += $@"AND PROD_NO = '{PROD_NO}'";

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
        /// 获取发外色卡明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardBody(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CARD_DATE = jarr.ContainsKey("CARD_DATE") ? jarr["CARD_DATE"].ToString() : "";//日期
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//厂商编号
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                #endregion


                #region 逻辑

                var sql = $@"
SELECT
	CARD_DATE,-- 日期
	VEND_NO,-- 编号
	SHOE_NO,-- 鞋型
    PROD_NO, -- ART
	APTESTITEM_NAME, -- 检测项名称
	TEST_STANDARD, -- 检验标准
	SAMP_QTY, -- 抽样数量
	AQL_LEVEL, -- 级别
	AC,
	RE,
	CHECK_RESULT,
	REMARKS
FROM
	QCM_COLOR_CARD_D
WHERE
	1 = 1
";

                if (!string.IsNullOrEmpty(VEND_NO))
                    sql += $@"AND VEND_NO = '{VEND_NO}'";
                if (!string.IsNullOrEmpty(SHOE_NO))
                    sql += $@"AND SHOE_NO = '{SHOE_NO}'";

                if (!string.IsNullOrEmpty(CARD_DATE))
                    sql += $@"AND CARD_DATE = '{CARD_DATE}'";
                if (!string.IsNullOrEmpty(PROD_NO))
                    sql += $@"AND PROD_NO = '{PROD_NO}'";

                //查询分页数据
                DataTable dt = DB.GetDataTable(sql);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

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
        /// 根据PO带出数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardDataByPO(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                #endregion


                #region 逻辑

                var sql = $@"SELECT DEVELOP_SEASON,PRODUCT_MONTH,MATERIAL_WAY,PROD_NO FROM bdm_rd_prod WHERE 1 = 1 AND PROD_NO = '{PROD_NO}'";


                //查询分页数据
                DataTable dt = DB.GetDataTable(sql);
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

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
        /// 根据部门代号带出产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardLineDataByDepartmentno(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string DEPARTMENT_NO = jarr.ContainsKey("DEPARTMENT_NO") ? jarr["DEPARTMENT_NO"].ToString() : "";//部门代号
                #endregion


                #region 逻辑

                var sql = $@"SELECT PRODUCTIONLINE_NO,PRODUCTIONLINE_NAME FROM BDM_QUALITY_DEPARTMENT_D WHERE DEPARTMENT_NO = '{DEPARTMENT_NO}'";


                //查询分页数据
                DataTable dt = DB.GetDataTable(sql);
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

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
        /// 更新发外色卡
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateColor(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CARD_DATE = jarr.ContainsKey("CARD_DATE") ? jarr["CARD_DATE"].ToString() : "";//厂商名称
                string VEND_NAME = jarr.ContainsKey("VEND_NAME") ? jarr["VEND_NAME"].ToString() : "";//鞋型
                string FIRSTARTICLE_TYPE = jarr.ContainsKey("FIRSTARTICLE_TYPE") ? jarr["FIRSTARTICLE_TYPE"].ToString() : "";//鞋型
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//鞋型
                string IS_QCCONFIRM = jarr.ContainsKey("IS_QCCONFIRM") ? jarr["IS_QCCONFIRM"].ToString() : "";//鞋型
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//鞋型
                string _VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//鞋型
                #endregion


                #region 逻辑
                DB.Open();
                DB.BeginTransaction();

                var sql = $@"UPDATE QCM_COLOR_CARD_M SET  FIRSTARTICLE_TYPE = '{FIRSTARTICLE_TYPE}' , SHOE_NO = '{SHOE_NO}' , PROD_NO = '{PROD_NO}' , IS_QCCONFIRM = '{IS_QCCONFIRM}', TEST_RESULT = '{TEST_RESULT}'  WHERE VEND_NO = '{_VEND_NO}' AND SHOE_NO = '{SHOE_NO}'";
                var sql2 = $@"
SELECT
	CARD_DATE,-- 日期
    VEND_NO, -- 编号
	VEND_NAME,-- 厂商名称
	FIRSTARTICLE_TYPE,-- 首件确认种类
    SHOE_NO,--鞋型
	PROD_NO, -- ART
	IS_QCCONFIRM,-- QC确认
	TEST_RESULT -- 测试状况
FROM
	QCM_COLOR_CARD_M
WHERE
	1 = 1
AND VEND_NO = '{_VEND_NO}' AND SHOE_NO = '{SHOE_NO}'";
                DataTable dt = DB.GetDataTable(sql2);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.ErrMsg = "编辑成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "编辑失败！";

            }

            return ret;

        }

        /// <summary>
        /// 色卡录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddColorCard(object OBJ)
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
                string time = jarr.ContainsKey("time") ? jarr["time"].ToString() : "";//检验单
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";
                string VEND_NAME = jarr.ContainsKey("VEND_NAME") ? jarr["VEND_NAME"].ToString() : "";
                string FIRSTARTICLE_TYPE = jarr.ContainsKey("FIRSTARTICLE_TYPE") ? jarr["FIRSTARTICLE_TYPE"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string PART_NO = jarr.ContainsKey("PART_NO") ? jarr["PART_NO"].ToString() : "";
                string IS_QCCONFIRM = jarr.ContainsKey("IS_QCCONFIRM") ? jarr["IS_QCCONFIRM"].ToString() : "";
                #endregion

                #region 提交验证
                //判断该色卡是否已有任务
                var COUNT = DB.GetInt32($@"SELECT COUNT(1) FROM QCM_COLOR_CARD_M WHERE VEND_NO = '{VEND_NO}' AND PROD_NO = '{PROD_NO}' AND PART_NO = '{PART_NO}'");

                if(COUNT > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "该色卡已有任务清单！";
                    return ret;
                }
                    
                #endregion

                #region 逻辑

                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string CREATEDATE = DateTime.Now.ToString("yyyy-MM-dd");
                string CREATETIME = DateTime.Now.ToString("HH:mm:ss");
                //string PO_QTY = DB.GetString($@"SELECT PO_QTY FROM QCM_PO_ORDER WHERE PORD_NO = '{PO_ORDER}'");

                #region 生成检验单

                string INSPECT_NO = DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001

                string MAX_INSPECT_NO = DB.GetString($@"select max(INSPECT_NO) from QCM_COLOR_CARD_M where INSPECT_NO like '{INSPECT_NO}%'");

                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(MAX_INSPECT_NO))
                {
                    string seq = MAX_INSPECT_NO.Replace(INSPECT_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    INSPECT_NO = INSPECT_NO + int_seq.ToString().PadLeft(5, '0');

                }
                else
                {
                    INSPECT_NO = INSPECT_NO + "00001";
                }

                #endregion

                Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                insert_dic.Add("INSPECT_NO", INSPECT_NO);
                insert_dic.Add("CARD_DATE", time);
                insert_dic.Add("VEND_NO", VEND_NO);
                insert_dic.Add("VEND_NAME", VEND_NAME);
                insert_dic.Add("FIRSTARTICLE_TYPE", FIRSTARTICLE_TYPE);
                insert_dic.Add("PROD_NO", PROD_NO);
                insert_dic.Add("SHOE_NO", SHOE_NO);
                //insert_dic.Add("PROD_NO", PROD_NO);
                insert_dic.Add("PART_NO", PART_NO);
                insert_dic.Add("IS_QCCONFIRM", IS_QCCONFIRM);
                insert_dic.Add("CREATEBY", CREATEBY);
                insert_dic.Add("CREATEDATE", CREATEDATE);
                insert_dic.Add("CREATETIME", CREATETIME);

                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_COLOR_CARD_M", insert_dic);
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

        #region PB

        /// <summary>
        /// 获取发外色卡列表(PB)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardListByPB(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string StrWhere = jarr.ContainsKey("StrWhere") ? jarr["StrWhere"].ToString() : "";//
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//状态 1.(若传未完成)则查状态为空 2.若完成，则查有值的
                #endregion

                

                #region 逻辑

                var sql = $@"
SELECT
    CARD_DATE AS CARD_DATE2,
    INSPECT_NO,
    VEND_NO, -- 厂商代号
	VEND_NAME, -- 厂商名称
	SHOE_NO,-- 鞋型
	FIRSTARTICLE_TYPE, -- 首件确认种类
    PROD_NO, -- ART
	PART_NO, -- 部件
	TEST_RESULT, -- 状态
    '' AS GENERALSTANDARD,
    '' AS GENERALTYPE,
    '' AS TSRESULT
FROM
	QCM_COLOR_CARD_M
WHERE
	1 = 1
";
                #region 查询条件
                if (TEST_RESULT == "FAIL")//未完成
                    sql += $@"AND TEST_RESULT  IS null";
                else if(TEST_RESULT == "")
                {
                    sql += "";
                }
                else if(TEST_RESULT == "PASS") // 已完成
                {
                    sql += $@"AND ( TEST_RESULT IS NOT NULL OR TEST_RESULT != '')";
                }

                if (!string.IsNullOrEmpty(StrWhere))
                    sql += $@"
AND ( SHOE_NO LIKE '%{StrWhere}%'
OR VEND_NAME LIKE '%{StrWhere}%'
OR FIRSTARTICLE_TYPE LIKE '%{StrWhere}%'
OR INSPECT_NO LIKE '%{StrWhere}%'
OR PROD_NO LIKE '%{StrWhere}%'
OR TEST_RESULT LIKE '%{StrWhere}%')";
                #endregion
                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    item["GENERALSTANDARD"] = "首检检测";
                    item["GENERALTYPE"] = "斜面检测";
                    item["TSRESULT"] = "PASS";
                }

                #endregion
                ret.RetData1 = dt;
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
        /// 获取发外色卡通用标准(PB)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardListByPBBZ(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数
                string PROD_NO = string.Empty;
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                if (jarr["PROD_NO"] == null)
                    PROD_NO = "1";//未维护ART
                else
                    PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                #endregion


                #region 逻辑

                //var sql = $@"SELECT GENERAL_TESTTYPE_NO,GENERAL_TESTTYPE_NAME FROM BDM_PROD_CUSTOMQUALITY_M WHERE 1 = 1 ";
                var sql = $@"SELECT GENERAL_TESTTYPE_NO,GENERAL_TESTTYPE_NAME FROM BDM_PROD_CUSTOMQUALITY_M WHERE 1 = 1 ";

                if(PROD_NO == "1")
                {
                    sql += $@"AND PROD_NO = 'null'";
                }

                if (!string.IsNullOrEmpty(PROD_NO))
                    sql += $@"AND PROD_NO = '{PROD_NO}'";
                List<string> List = new List<string>();

                var dt = DB.GetDataTable(sql);

                //foreach (DataRow item in dt.Rows)
                //{
                //    List.Add(item["GENERAL_TESTTYPE_NO"].ToString());
                //}

                #endregion
                ret.RetData1 = dt;
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
        /// 获取发外色卡检测类型(PB)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardListByPBLX(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                string GENERAL_TESTTYPE_NO= jarr.ContainsKey("GENERAL_TESTTYPE_NO") ? jarr["GENERAL_TESTTYPE_NO"].ToString() : "";//检测类型代号
                #endregion


                #region 逻辑

                var sql = $@"SELECT CATEGORY_NO,CATEGORY_NAME FROM BDM_PROD_CUSTOMQUALITY_D WHERE 1 = 1 ";
                if (!string.IsNullOrEmpty(PROD_NO))
                    sql += $@"AND PROD_NO = '{PROD_NO}'";

                if (!string.IsNullOrEmpty(GENERAL_TESTTYPE_NO))
                    sql += $@"AND GENERAL_TESTTYPE_NO = '{GENERAL_TESTTYPE_NO}'";
                List<string> List = new List<string>();
                var dt = DB.GetDataTable(sql);
                //foreach (DataRow item in dt.Rows)
                //{
                //    List.Add(item["CATEGORY_NAME"].ToString());
                //}

                #endregion
                ret.RetData1 = dt;
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
        /// 获取发外色卡明细(PB)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetColorCardDetailByPB(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                string GENERAL_TESTTYPE_NO = jarr.ContainsKey("GENERAL_TESTTYPE_NO") ? jarr["GENERAL_TESTTYPE_NO"].ToString() : "";//检测类型代号
                string CATEGORY_NO = jarr.ContainsKey("CATEGORY_NO") ? jarr["CATEGORY_NO"].ToString() : "";//类别（检测类型）
                #endregion


                #region 逻辑

                var sql = $@"
SELECT
	D_CHECK_VALUE, -- 检验标准(定制)
    TESTITEM_CODE,-- 检测项编号
    TESTITEM_NAME,-- 名称
	SAMPLE_NUM, -- 检验数量
	ART_REMARKS, -- 备注
	'' AS AQL_LEVEL, -- AQL级别
	'' AS AC,
	'' AS RE
FROM
	BDM_PROD_CUSTOMQUALITY_ITEM
WHERE
	1 = 1
AND TESTITEM_CATEGORY = '2'
AND PROD_NO = '{PROD_NO}'
AND GENERAL_TESTTYPE_NO = '{GENERAL_TESTTYPE_NO}'
AND CATEGORY_NO = '{CATEGORY_NO}'";

                var dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    if (item["D_CHECK_VALUE"].ToString() == "") // 如果检验标准(定制)无值，则测查检验标准(标准)
                    {
                        string T_CHECK_ITEM = DB.GetString($@"
SELECT
	T_CHECK_ITEM -- 检验标准(标准)
FROM
	BDM_PROD_CUSTOMQUALITY_ITEM
WHERE
	1 = 1
AND TESTITEM_CATEGORY = '2'
AND PROD_NO = '{PROD_NO}'
AND GENERAL_TESTTYPE_NO = '{GENERAL_TESTTYPE_NO}'
AND CATEGORY_NO = '{CATEGORY_NO}'");

                        item["D_CHECK_VALUE"] = T_CHECK_ITEM;
                    }
                }

                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();
            }

            return ret;

        }

        /// <summary>
        /// 提交发外色卡明细(PB)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddColorCardContainerByPB(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string IS_QCCONFIRM = jarr.ContainsKey("IS_QCCONFIRM") ? jarr["IS_QCCONFIRM"].ToString() : "";//QC确认
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//测试状况

                string CARD_DATE = jarr.ContainsKey("CARD_DATE") ? jarr["CARD_DATE"].ToString() : "";//需要保存到表身的日期
                string CARD_DATE2 = jarr.ContainsKey("CARD_DATE2") ? jarr["CARD_DATE2"].ToString() : "";//日期2表头录入的日期
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//厂商编号
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                string DEFECT_CONTENT = jarr.ContainsKey("qsInfo") ? jarr["qsInfo"].ToString() : "";//问题点
                string IMPROVEMENT_MODE = jarr.ContainsKey("ways") ? jarr["ways"].ToString() : "";//改善方式

                List<ExternalColorCardDto> request = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ExternalColorCardDto>>(jarr["Detail"].ToString());
                

                #endregion


                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                string sql = string.Empty;
                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");
                string UpdateSql = $@"UPDATE QCM_COLOR_CARD_M SET IS_QCCONFIRM = '{IS_QCCONFIRM}' ,TEST_RESULT = '{TEST_RESULT}' WHERE CARD_DATE = '{CARD_DATE2}' AND VEND_NO = '{VEND_NO}' AND SHOE_NO = '{SHOE_NO}'";
                var sqlIMG = "";
                string guid = Guid.NewGuid().ToString("N");
                string APSTESTITEM_CODE = string.Empty;
                List<DetailImg> ImageList = new List<DetailImg>();

                if (request.Count > 0)
                {
                    //提交数据明细
                    foreach (var item in request)
                    {
                        ImageList = item.imageList;
                        foreach (var item2 in ImageList)
                        {
                            //string image_name = item2.Substring(item2.LastIndexOf('/'),item2.Length-1);
                            //string name = System.IO.Path.GetFileName(item2);

                            sqlIMG += $@" INTO QCM_COLOR_CARD_IMAGE 
                (
                CARD_DATE,
                VEND_NO,
                SHOE_NO,
                PROD_NO,
                APTESTITEM_CODE,
                IMG_NAME,
                IMG_URL,
                GUID,
                CREATEBY,
                CREATEDATE,
                CREATETIME

                )
                VALUES
                (
                '{CARD_DATE}',
                '{VEND_NO}',
                '{SHOE_NO}',
                '{PROD_NO}',
                '{APSTESTITEM_CODE}',
                '{item2.img_name}',
                '{item2.img_url}',
                '{guid}',
                '{CREATEBY}',
                '{createdate}',
                '{createtime}'
                )";
                        }

                        APSTESTITEM_CODE = item.TESTITEM_CODE;
                        sql += $@"
                 INTO QCM_COLOR_CARD_D (
                    CARD_DATE,
                	VEND_NO,
                	PROD_NO,
                	SHOE_NO,
                	APTESTITEM_CODE,
                    APTESTITEM_NAME,
                	TEST_STANDARD, -- 检测标准
                	SAMP_QTY,
                	AQL_LEVEL,
                	AC,
                	RE,
                	CHECK_RESULT,
                	REMARKS,
                    DEFECT_CONTENT,
                    IMPROVEMENT_MODE,
                    CREATEBY,
                    CREATEDATE,
                    CREATETIME
                )
                VALUES
                (
                '{CARD_DATE}',
                '{VEND_NO}',
                '{item.PROD_NO}',
                '{SHOE_NO}',
                '{item.TESTITEM_CODE}', -- 检测项编号
                '{item.TESTITEM_NAME}',
                '{item.TEST_STANDARD}',
                '{item.SAMP_QTY}',
                '{item.AQL_LEVEL}',
                '{item.AC}',
                '{item.RE}',
                '{item.CHECK_RESULT}',
                '{item.REMARKS}',
                '{item.qsInfo}',
                '{item.ways}',
                '{CREATEBY}',
                '{createdate}',
                '{createtime}'
                )";

                    }
                    if (!string.IsNullOrEmpty(sql))
                        DB.ExecuteNonQuery("INSERT ALL" + sql + "SELECT * FROM DUAL");
                    if (!string.IsNullOrEmpty(sqlIMG))
                        DB.ExecuteNonQuery("INSERT ALL" + sqlIMG + "SELECT * FROM DUAL");
                    DB.ExecuteNonQuery(UpdateSql);
                    DB.Commit();

                    ret.ErrMsg = "提交成功！";
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "提交失败！";
                    ret.IsSuccess = false;
                }
                #endregion

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

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
