using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class MaterialInspection
    {
        /// <summary>
        /// 进仓材料检验清单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaterialInspectionList(object OBJ)
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
                //CHK_NO    PUR_VEND_NAME   ITEM_NO start_date  TSCHECK_RESULT  PRO_VEND_NAME   WAREHOUSE_QTY   SAMP_CONDITION  STATUS  APCHECK_RESULT
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string PUR_VEND_NAME = jarr.ContainsKey("PUR_VEND_NAME") ? jarr["PUR_VEND_NAME"].ToString() : "";//采购厂商
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//物料编码
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束日期
                string TSCHECK_RESULT = jarr.ContainsKey("TSCHECK_RESULT") ? jarr["TSCHECK_RESULT"].ToString() : "";//测试结果/物性结果
                string PRO_VEND_NAME = jarr.ContainsKey("PRO_VEND_NAME") ? jarr["PRO_VEND_NAME"].ToString() : "";//生产厂商
                string WAREHOUSE_QTY = jarr.ContainsKey("WAREHOUSE_QTY") ? jarr["WAREHOUSE_QTY"].ToString() : "";//仓库
                string SAMP_CONDITION = jarr.ContainsKey("SAMP_CONDITION") ? jarr["SAMP_CONDITION"].ToString() : "";//取样状况
                //string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";//状态 1-检测中 2-已完成
                string APCHECK_RESULT = jarr.ContainsKey("APCHECK_RESULT") ? jarr["APCHECK_RESULT"].ToString() : "";//外观结果


                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//1-待处理 2-已完成

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                #endregion

                #region 查询条件验证

                //if (STATUS == "待处理")
                //    STATUS = "1";
                //if (STATUS == "已完成")
                //    STATUS = "2";

                #endregion

                #region 逻辑
                var sql = "";
                string WHERE = "";
                //if (STATUS == "1")
                //    WHERE += $@" and status <> '2'";
                //else
                //    WHERE += $@" and status = '2' ";//已完成

                if(TYPE == "1")
                {
                    sql = $@"
SELECT
	RCPT_DATE, -- 收货日期
	PRO_VEND_NO, -- 生产厂商编号
	PRO_VEND_NAME,
	CHK_NO, -- 收料单号
	ITEM_NO,-- 料号
	PUR_VEND_NO, -- 采购厂商编号
	PUR_VEND_NAME,--
	APCHECK_DATE, -- 外观检验日期
	TSCHECK_DATE,-- 实验室检验日期
	PR_UNIT,--收货单位
	ORD_QTY,--采购量
	PURCHASE_NO,-- 采购单号
	APCHECK_RESULT,-- 外观结果
	SAMP_CONDITION,-- 取样状况
	TSCHECK_RESULT, -- 检测结果
	INSPECTOR,-- 检验员
	INSPECT_QTY,-- 检验数
	OK_QTY,--合格数
	NG_QTY,--验退数
	REPAIR_QTY,--补送
	WAREHOUSE_QTY, -- 仓库
	STATUS -- 状态
FROM
	QCM_CLIMAORDER_M
WHERE 
	1 = 1
AND STATUS = '1'
{WHERE}
";
                }
                else
                {
                    sql = $@"
SELECT
	RCPT_DATE, -- 收货日期
	PRO_VEND_NO, -- 生产厂商编号
	PRO_VEND_NAME,
	CHK_NO, -- 收料单号
	ITEM_NO,-- 料号
	PUR_VEND_NO, -- 采购厂商编号
	PUR_VEND_NAME,--
	APCHECK_DATE, -- 外观检验日期
	TSCHECK_DATE,-- 实验室检验日期
	PR_UNIT,--收货单位
	ORD_QTY,--采购量
	PURCHASE_NO,-- 采购单号
	APCHECK_RESULT,-- 外观结果
	SAMP_CONDITION,-- 取样状况
	TSCHECK_RESULT, -- 检测结果
	INSPECTOR,-- 检验员
	INSPECT_QTY,-- 检验数
	OK_QTY,--合格数
	NG_QTY,--验退数
	REPAIR_QTY,--补送
	WAREHOUSE_QTY, -- 仓库
	STATUS -- 状态
FROM
	QCM_CLIMAORDER_M
WHERE 
	1 = 1
AND STATUS = '2'
{WHERE}
";
                }

                //CHK_NO    PUR_VEND_NAME   ITEM_NO start_date  TSCHECK_RESULT  PRO_VEND_NAME   WAREHOUSE_QTY   SAMP_CONDITION  STATUS  APCHECK_RESULT
                if (!string.IsNullOrEmpty(CHK_NO))
                    sql += $@" AND CHK_NO LIKE '%{CHK_NO}%'";

                if (!string.IsNullOrEmpty(PUR_VEND_NAME))
                    sql += $@" AND PUR_VEND_NAME LIKE '%{PUR_VEND_NAME}%'";

                if (!string.IsNullOrEmpty(ITEM_NO))
                    sql += $@" AND ITEM_NO LIKE '%{ITEM_NO}%'";

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    sql += $@"AND ( RCPT_DATE BETWEEN '{start_date}' AND '{end_date}') ";

                if (!string.IsNullOrEmpty(TSCHECK_RESULT))
                    sql += $@" AND TSCHECK_RESULT LIKE '%{TSCHECK_RESULT}%'";

                if (!string.IsNullOrEmpty(PRO_VEND_NAME))
                    sql += $@" AND PRO_VEND_NAME LIKE '%{PRO_VEND_NAME}%'";

                if (!string.IsNullOrEmpty(WAREHOUSE_QTY))
                    sql += $@" AND WAREHOUSE_QTY LIKE '%{WAREHOUSE_QTY}%'";

                if (!string.IsNullOrEmpty(SAMP_CONDITION))
                    sql += $@" AND SAMP_CONDITION LIKE '%{SAMP_CONDITION}%'";

                //if (!string.IsNullOrEmpty(STATUS))
                //    sql += $@" AND STATUS LIKE '%{STATUS}%'";

                if (!string.IsNullOrEmpty(APCHECK_RESULT))
                    sql += $@" AND APCHECK_RESULT LIKE '%{APCHECK_RESULT}%'";




                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                foreach (DataRow item in dt.Rows)
                {

                    //item["SUPPLIERS_NAME2"] = DB.GetString($@"SELECT SUPPLIERS_NAME FROM BASE003M WHERE SUPPLIERS_CODE = '{item["VEND_NO2"].ToString()}'");
                    //TestResultTemp = DB.GetString($@"SELECT CHECK_RESULT FROM QCM_INSPECTION_LABORATORY_M WHERE ");
                    //item["TestResult"] = !string.IsNullOrEmpty(TestResultTemp) ? TestResultTemp :"FAIL";
                    //var DetailCount = DB.GetInt32($@"SELECT COUNT(1) FROM QCM_INSPECTION_LABORATORY_M WHERE MATERIAL_NO = '{item["ITEM_NO"].ToString()}' AND PLANTAREA_NO = '{item["VEND_NO"].ToString()}'");
                    //item["PERSON"] = DB.GetString($@"SELECT STAFF_NAME FROM QCM_INSPECTION_LABORATORY_M WHERE MATERIAL_NO = '{item["ITEM_NO"].ToString()}' AND PLANTAREA_NO = '{item["VEND_NO"].ToString()}'
                    //");
                    //if (DetailCount > 0)
                    //{

                    //INSPECTION_NO = DB.GetString($@"SELECT STAFF_NAME FROM QCM_INSPECTION_LABORATORY_M WHERE MATERIAL_NO = '{item["ITEM_NO"].ToString()}' AND PLANTAREA_NO = '{item["VEND_NO"].ToString()}'");

                    

                    //var dtDetail = DB.GetDataTable($@"SELECT RESULT_VALUE FROM QCM_INSPECTION_LABORATORY_D WHERE INSPECTION_NO = '{item["INSPECTION_NO"].ToString()}'"); // 检测中
                    //string isflag = "0";// 0-都是PASS 1-包含其他结果

                    //foreach (DataRow itemdetail in dtDetail.Rows)
                    //{

                    //    if (itemdetail["RESULT_VALUE"].ToString() != "PASS")
                    //    {
                    //        isflag = "1";
                    //        item["TESTRESULT"] = "FAIL";
                    //        break;
                    //    }
                    //}
                    //if (isflag == "0")
                    //    item["TESTRESULT"] = "PASS";
                }
                    
                //}
                //DataRow[] dt2 = null;
                //DataTable dataTable = new DataTable();
                //if (!string.IsNullOrEmpty(TestResult))
                //{
                //    dt2 = dt.Select($@"RCPT_BY = '{TestResult}'");
                //    dataTable = dt2[0].Table.Clone(); //复制需要更新的表的结构

                //    foreach (var dr in dt2)
                //    {
                //        dataTable.ImportRow(dr);
                //    }
                //}





                //DataRow[] rows = dt.Select($@"TestResult = {TestResult}");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);


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
