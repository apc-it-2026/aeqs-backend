using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class InspectionResult
    {
        /// <summary>
        /// 获取送测结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCheckResult(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//ART
                string STAFF_NO = jarr.ContainsKey("STAFF_NO") ? jarr["STAFF_NO"].ToString() : "";//送检人员代号
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//送检开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//送检结束日期
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";//类型  0-检测中  1-已完成
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                #endregion

                #region 接口验证
                
                var Is_staff_no = DB.GetString($@"SELECT * FROM HR001M WHERE STAFF_NO = '{STAFF_NO}'AND rownum =1");

                if (Is_staff_no == "")
                {
                    ret.IsSuccess = false;
                    throw new Exception("不存在该员工，请检查！");
                }


                #endregion

                #region 逻辑
                DataTable dt = new DataTable();
                int rowCount = 0;
                string WHERE = string.Empty;

                if (!string.IsNullOrEmpty(INSPECTION_NO))
                    WHERE += $@"AND QCM_INSPECTION_LABORATORY_M.INSPECTION_NO LIKE '%{INSPECTION_NO}%'";

                if(!string.IsNullOrEmpty(ART))
                    WHERE += $@"AND QCM_INSPECTION_LABORATORY_M.ART_CODE LIKE '%{ART}%'";

                if (!string.IsNullOrEmpty(STAFF_NO))
                    WHERE += $@"AND QCM_INSPECTION_LABORATORY_M.STAFF_NO LIKE '%{STAFF_NO}%'";

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    WHERE += $@"AND ( QCM_INSPECTION_LABORATORY_M.INSPECTION_DATE BETWEEN '{start_date}' AND '{end_date}') ";
                 
                var sql = "";
                if (Type == "0")
                {
                    sql = $@"
                            SELECT DISTINCT
	                            QCM_INSPECTION_LABORATORY_M.INSPECTION_NO,-- 送检单号
	                            BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NAME, --送检类型
	                            QCM_INSPECTION_LABORATORY_M.ART_CODE, -- ART
	                            QCM_INSPECTION_LABORATORY_M.DEPARTMENT_NO, -- 阶段名称
	                            QCM_INSPECTION_LABORATORY_M.CATEGORY_NAME,-- 试样种类名称
	                            QCM_INSPECTION_LABORATORY_M.STAFF_NAME, -- 送测人员
	                            QCM_INSPECTION_LABORATORY_M.DEPARTMENT_NAME,-- 送测部门
	                            QCM_INSPECTION_LABORATORY_M.PLANTAREA_NAME,-- 厂区
	                            QCM_INSPECTION_LABORATORY_M.PRODUCTIONLINE_NAME,-- 产线
	                            '' as CHECK_RESULT,  -- 检测结果
	                            QCM_INSPECTION_LABORATORY_M.INSPECTION_DATE -- 送检日期
                            FROM
	                            QCM_INSPECTION_LABORATORY_M 
                            LEFT JOIN BDM_GENERAL_TESTTYPE_M ON BDM_GENERAL_TESTTYPE_M.general_testtype_no = QCM_INSPECTION_LABORATORY_M.general_testtype_no
                            WHERE 
	                            1 = 1 
                            {WHERE}
                            AND STATUS <> '2' ";//检测中

                    //查询分页数据
                    dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                    rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                    foreach (DataRow item in dt.Rows)
                    {
                        int flag = 0; //0-未开始 1-检测中 2-已完成
                        int StatusRecord = 0;//记录表身明细检验状态 （空）--未检测
                        DataTable dt2 = DB.GetDataTable($@"SELECT CHECK_RESULT FROM QCM_INSPECTION_LABORATORY_D WHERE INSPECTION_NO = '{item["INSPECTION_NO"].ToString()}'");
                        int count = dt2.Rows.Count;//计算检验项目
                        foreach (DataRow itemdetail in dt2.Rows)
                        {

                            if (itemdetail["CHECK_RESULT"].ToString() == "")
                            {
                                StatusRecord++;
                            }

                        }
                        if (count == StatusRecord)//检测项目数 = 记录检测项目为空的数
                            item["CHECK_RESULT"] = "未开始";
                        if (StatusRecord < count && StatusRecord != 0)//检测项目为空的数 < 检测项目数 且  检测项目为空的数不等于0
                            item["CHECK_RESULT"] = "测试中";
                        if (StatusRecord == 0)
                            item["CHECK_RESULT"] = "已完成";

                    }
                }
                else
                {
                    sql = $@"
                            SELECT DISTINCT
	                            QCM_INSPECTION_LABORATORY_M.INSPECTION_NO,-- 送检单号
	                            BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NAME, --送检类型
	                            QCM_INSPECTION_LABORATORY_M.ART_CODE, -- ART
	                            QCM_INSPECTION_LABORATORY_M.DEPARTMENT_NO, -- 阶段名称
	                            QCM_INSPECTION_LABORATORY_M.CATEGORY_NAME,-- 试样种类名称
	                            QCM_INSPECTION_LABORATORY_M.STAFF_NAME, -- 送测人员
	                            QCM_INSPECTION_LABORATORY_M.DEPARTMENT_NAME,-- 送测部门
	                            QCM_INSPECTION_LABORATORY_M.PLANTAREA_NAME,-- 厂区
	                            -- QCM_INSPECTION_LABORATORY_M.PRODUCTIONLINE_NAME,-- 产线
	                            '' as CHECK_RESULT,  -- 检测结果
	                            QCM_INSPECTION_LABORATORY_M.INSPECTION_DATE -- 送检日期
                            FROM
	                            QCM_INSPECTION_LABORATORY_M 
                            LEFT JOIN BDM_GENERAL_TESTTYPE_M ON BDM_GENERAL_TESTTYPE_M.general_testtype_no = QCM_INSPECTION_LABORATORY_M.general_testtype_no
                            WHERE 
	                            1 = 1 
                            {WHERE}
                            AND STATUS = '2' ";//已完成

                    //查询分页数据
                    dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                    rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                    foreach (DataRow item in dt.Rows)
                    {
                        item["CHECK_RESULT"] = "已完成";
                    }
                }

                //DataTable dt = DB.GetDataTable(sql);

                

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data",dt); 
                dic.Add("rowCount", rowCount);


                ret.IsSuccess = true; 
                ret.RetData =Newtonsoft.Json.JsonConvert.SerializeObject(dic) ;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            return ret;

        }

        /// <summary>
        /// 获取查看表头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReportHead(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单

                #endregion

                #region 逻辑
                string WHERE = string.Empty;

                if (!string.IsNullOrEmpty(INSPECTION_NO))
                    WHERE += $@"AND QCM_INSPECTION_LABORATORY_M.INSPECTION_NO LIKE '%{INSPECTION_NO}%'";

                string sql = $@"
SELECT
GENERAL_TESTTYPE_NAME,--检测类型
INSPECTION_DATE,-- 检测提交日期
ART_CODE,
CATEGORY_NAME,-- 试样种类名称
INSPECTION_ENDDATE, -- 完成日期
DEPARTMENT_NO, -- 阶段
PLANTAREA_NAME, -- 厂区
GENERAL_TESTTYPE_NO -- 检测类型


FROM
	QCM_INSPECTION_LABORATORY_M
WHERE
	1 = 1
{WHERE}
";
                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    item["general_testtype_no"] =  DB.GetString($@"SELECT  GENERAL_TESTTYPE_NAME FROM BDM_GENERAL_TESTTYPE_M WHERE GENERAL_TESTTYPE_NO = '{item["general_testtype_no"].ToString()}'");
                }

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            return ret;

        }

        /// <summary>
        /// 获取查看内容明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReportBody(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string INSPECTION_NO = jarr.ContainsKey("INSPECTION_NO") ? jarr["INSPECTION_NO"].ToString() : "";//送检单

                #endregion

                #region 逻辑
                string WHERE = string.Empty;

                if (!string.IsNullOrEmpty(INSPECTION_NO))
                    WHERE += $@"AND QCM_INSPECTION_LABORATORY_D.INSPECTION_NO LIKE '%{INSPECTION_NO}%'";

                string sql = $@"
SELECT
    QCM_INSPECTION_LABORATORY_D.INSPECTION_NO,-- 送检单号
    QCM_INSPECTION_LABORATORY_D.TESTITEM_CODE, -- 编号
    QCM_INSPECTION_LABORATORY_D.TESTITEM_NAME, -- 名称
    QCM_INSPECTION_LABORATORY_D.REFERENCE_LEVEL, -- 引用级别
    QCM_INSPECTION_LABORATORY_D.RESULT_VALUE, -- 测试结果值
    QCM_INSPECTION_LABORATORY_D.T_CHECK_ITEM, -- 判断标准(通用)
    QCM_INSPECTION_LABORATORY_D.T_CHECK_VALUE, -- 测量标准(通用)
    QCM_INSPECTION_LABORATORY_D.D_CHECK_ITEM, -- 判断标准(定制)
    QCM_INSPECTION_LABORATORY_D.D_CHECK_VALUE, -- 测量标准(定制)
    QCM_INSPECTION_LABORATORY_D.UNIT, -- 单位i
    QCM_INSPECTION_LABORATORY_D.SAMPLE_NUM, --试样数量
    QCM_INSPECTION_LABORATORY_D.RESULT_VALUE,-- 检测结果值
    QCM_INSPECTION_LABORATORY_D.CHECK_RESULT, -- 检测结果 (pass/fail)
    QCM_INSPECTION_LABORATORY_D.TEST_REMARKS,-- 测试项备注
    BDM_FORMULA_M.FORMULA_CONTENT -- 公式内容
FROM
	QCM_INSPECTION_LABORATORY_D
LEFT JOIN BDM_FORMULA_M ON QCM_INSPECTION_LABORATORY_D.CUSTOM_FORMULA = BDM_FORMULA_M.FORMULA_CODE

WHERE
    1 = 1
{WHERE}
";
                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    if (item["CHECK_RESULT"].ToString() == "")
                        item["CHECK_RESULT"] = "FAIL";
                }

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

                #endregion

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
