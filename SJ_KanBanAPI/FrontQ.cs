using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_KanBanAPI
{
	
    /// <summary>
    /// 前段Q
    /// </summary>
    internal class FrontQ
    {

		public static string moudle_code = "visualInspection";//外观检验
		public static string moudle_code2 = "equipmentParameters";//设备参数
		#region 外观检验
		/// <summary>
		/// 时间数组
		/// </summary>
		/// <param name="timeType">日期类型：【m】月，【d】日</param>
		/// <param name="startTime">开始时间</param>
		/// <param name="endTime">结束时间</param>
		/// <returns></returns>
		public static List<string> ReturnData(string timeType, string startTime, string endTime)
        {
            List<string> TimeList = new List<string>();
            switch (timeType.ToLower().Trim())
            {
                case "m":
                    startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM");
                    endTime = Convert.ToDateTime(endTime).AddMonths(1).ToString("yyyy-MM");
                    while (startTime != endTime)
                    {
                        TimeList.Add(startTime);
                        startTime = Convert.ToDateTime(startTime).AddMonths(1).ToString("yyyy-MM");
                    }
                    break;
                case "d":
                    startTime = Convert.ToDateTime(startTime).ToString("yyyy-MM-dd");
                    endTime = Convert.ToDateTime(endTime).AddDays(1).ToString("yyyy-MM-dd");
                    while (startTime != endTime)
                    {
                        TimeList.Add(startTime);
                        startTime = Convert.ToDateTime(startTime).AddDays(1).ToString("yyyy-MM-dd");
                    }
                    break;
            }
            return TimeList;
        }

        /// <summary>
        /// 外观检验--年度柱状，扇形数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllHisCakeOneData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                #region 查询条件
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//

                string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
                string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
                string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
                string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
                string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
                string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
                #endregion
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    throw new Exception("开始时间，结束时间为必填字段！");
                }

                #region 拼接条件
                string IWSQL = string.Empty;
                string RWSQL = string.Empty;
                if (!string.IsNullOrEmpty(firmType))//厂区类型
                {
					var arrfrimtype = string.Join(',', firmType.Split(',').Select(x => $"'{x}'"));
					RWSQL += $@" AND FACTORYDATA.ORG_NAME in({arrfrimtype})";
                }
                if (!string.IsNullOrEmpty(firmClass))//厂商类型
                {
                    string[] arrfirmclass = firmClass.Split(',');
                    RWSQL += $@" AND B.M_TYPE in ({string.Join(',', arrfirmclass.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(frim))//厂商
                {
                    string[] orderListf = frim.Split(',');
                    RWSQL += $@" AND B.SUPPLIERS_NAME in ({string.Join(',', orderListf.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(frimArea))//厂区 
                {
                    string[] orderListfa = frimArea.Split(',');
                    RWSQL += $@" AND sj.EN in ({string.Join(',', orderListfa.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(workshop))//工段
                {
                    string[] orderListwork = workshop.Split(',');
                    RWSQL += $@" AND QM.WORKSHOP_SECTION_NAME in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(parts))
                {

                }
                if (!string.IsNullOrEmpty(art))
                {
                    string[] orderListart = art.Split(',');
                    RWSQL += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(po))
                {
                    string[] orderListpo = po.Split(',');
                    RWSQL += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(itemNo))
                {

                }
                #endregion


                RetDic["Histogram1"] = null;//柱图1
                RetDic["cake"] = null;//饼图
                #region x轴-月份
                string startYear = DateTime.Now.ToString("yyyy-01-01");
                string endYear = Convert.ToDateTime(DateTime.Now.AddYears(1).ToString("yyyy-01-01")).AddDays(-1).ToString("yyyy-MM-dd");
                List<string> MonthList = ReturnData("m", startYear, endYear);
				#endregion

				//多语言翻译
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "外观检验合格数", "外观检验不合格数", "合格率" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{

					name1 = LanDic["外观检验合格数"].ToString();
					name2 = LanDic["外观检验不合格数"].ToString();
					name3 = LanDic["合格率"].ToString();


				}
				//柱图1
				string HistogramDataSql = $@"WITH FACTORYDATA AS ( SELECT BASE005M.DEPARTMENT_CODE, BASE005M.UDF05, ORG_CODE, ORG_NAME FROM BASE005M LEFT JOIN BASE001M ON BASE005M.FACTORY_SAP = BASE001M.ORG_CODE ),
												iqc_data AS (--C端IQC里面的进仓材料检验清单
												SELECT
													to_char( to_date( INSPECTIONDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) INSPECTIONDATE,
													sum( a.pass_qty ) pass_qty,
													sum( nvl( b.BAD_QTY, 0 ) ) bad_qty 
												FROM
													qcm_iqc_insp_res_m a
													LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
													AND a.CHK_SEQ = b.CHK_SEQ 
													AND a.ITEM_NO = b.ITEM_NO 
												WHERE
													substr( a.INSPECTIONDATE, 0, 4 ) = to_char( SYSDATE, 'yyyy' ) --写条件
		
												GROUP BY
													to_char( to_date( INSPECTIONDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
												),
												rqc_data AS (--移动端RQC
												SELECT
													to_char( to_date( a.createdate, 'yyyy-mm-dd' ), 'yyyy-mm' ) INSPECTIONDATE,
												sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
												sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
											FROM
												rqc_task_detail_t a 
                                                LEFT JOIN RQC_TASK_M rm ON a.TASK_NO = rm.TASK_NO
											    LEFT JOIN QCM_DQA_MAG_M qm ON qm.WORKSHOP_SECTION_NO = rm.WORKSHOP_SECTION_NO 
											    AND rm.SHOE_NO = qm.SHOES_CODE
												LEFT JOIN BASE003M B ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
												LEFT JOIN FACTORYDATA ON FACTORYDATA.DEPARTMENT_CODE = RM.PRODUCTION_LINE_CODE
												JOIN SJQDMS_ORGINFO sj ON FACTORYDATA.UDF05 = sj.code
											WHERE
												substr( a.createdate, 0, 4 ) = to_char( SYSDATE, 'yyyy' )  {RWSQL}
	
											GROUP BY
												to_char( to_date( a.createdate, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
												),
												total_data AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ) SELECT
												INSPECTIONDATE,
												sum( pass_qty ) pass_qty,
												sum( bad_qty ) bad_qty,
											CASE
													WHEN sum( pass_qty ) + sum( bad_qty ) > 0 THEN
													round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 1 ) ELSE 0 
												END pass_rate 
											FROM
												total_data 
											GROUP BY
												INSPECTIONDATE";
                RetDic["Histogram1"] = Common.RetunHisData(DB, MonthList, HistogramDataSql, "INSPECTIONDATE", "pass_qty", "bad_qty", "pass_rate", name1, name2, name3);//柱图1
                                                                                                                                                                                //饼图
                string CakeDataSQL = $@"WITH iqc_data AS (
	                                    SELECT
		                                    b.BADPROBLEM_NAME,
		                                    count( a.id ) bad_qty 
	                                    FROM
		                                    qcm_iqc_insp_res_d a
		                                    JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE 
	                                    WHERE
		                                    a.determine = 1 
		                                    AND a.createdate BETWEEN '{startYear}' 
		                                    AND '{endYear}' 
	                                    GROUP BY
		                                    b.BADPROBLEM_NAME 
	                                    ),
	                                    rqc_data AS (
	                                    SELECT
		                                    c.INSPECTION_NAME BADPROBLEM_NAME,
		                                    count( c.id ) bad_qty 
	                                    FROM
		                                    rqc_task_detail_t a
		                                    JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
		                                    AND a.COMMIT_INDEX = b.COMMIT_INDEX
		                                    JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
		                                    AND b.UNION_ID = c.ID 
	                                    WHERE
		                                    a.commit_type = 1 
		                                    AND a.CREATEDATE BETWEEN '{startYear}' 
		                                    AND '{endYear}' 
	                                    GROUP BY
		                                    c.INSPECTION_NAME 
	                                    ),
	                                    total_data AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ),
										sumData AS ( SELECT BADPROBLEM_NAME, sum( bad_qty ) bad_qty,ROW_NUMBER() over (ORDER BY sum(bad_qty) DESC) RANKING FROM total_data GROUP BY BADPROBLEM_NAME )
										SELECT BADPROBLEM_NAME,bad_qty FROM sumData WHERE RANKING<=5
										union ALL
										SELECT 'Other' BADPROBLEM_NAME, NVL( sum( bad_qty ), 0 ) bad_qty  FROM sumData WHERE RANKING>5 ";
                RetDic["cake"] = Common.ReturnCakeData(DB, CakeDataSQL, 0, "BADPROBLEM_NAME", "bad_qty", "");

                ret.RetData1 = RetDic;
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
        /// 外观检验--每日柱状，扇形数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllHisCakeTwoData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                #region 查询条件
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
                string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
                string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
                string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
                string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
                string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
                string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
                #endregion

                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    throw new Exception("开始时间，结束时间为必填字段！");
                }
                #region 拼接条件
                string IWSQL = string.Empty;
                string RWSQL = string.Empty;
                if (!string.IsNullOrEmpty(firmClass))//厂商类型
                {
					var arrfrimclass = string.Join(',', firmClass.Split(',').Select(x => $"'{x}'"));

					RWSQL += $@" AND B.M_TYPE in ({arrfrimclass})";
                }
                if (!string.IsNullOrEmpty(firmType))//厂区类型
                {
					string[] arrfirmType = firmType.Split(',');
                    RWSQL += $@" AND FACTORYDATA.ORG_NAME in ({string.Join(',', arrfirmType.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(frim))//厂商
                {
                    string[] orderListf = frim.Split(',');
                    RWSQL += $@" AND B.SUPPLIERS_NAME in ({string.Join(',', orderListf.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(frimArea))//厂区
                {
                    string[] orderListfa = frimArea.Split(',');
                    RWSQL += $@" AND sj.EN in ({string.Join(',', orderListfa.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(workshop))//工段
                {
                    string[] orderListwork = workshop.Split(',');
                    RWSQL += $@" AND QM.WORKSHOP_SECTION_NAME in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(parts))
                {

                }
                if (!string.IsNullOrEmpty(art))
                {
                    string[] orderListart = art.Split(',');
                    RWSQL += $@"AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(po))
                {
                    string[] orderListpo = po.Split(',');
                    RWSQL += $@"AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(itemNo))
                {

                }
                #endregion

                //柱图数据
                List<string> DayList = Common.ReturnData("d", startTime, endTime);
                string HistogramDataSql = $@"WITH FACTORYDATA AS ( SELECT BASE005M.DEPARTMENT_CODE, BASE005M.UDF05, ORG_CODE, ORG_NAME FROM BASE005M LEFT JOIN BASE001M ON BASE005M.FACTORY_SAP = BASE001M.ORG_CODE ),
												iqc_data AS (--C端IQC里面的进仓材料检验清单
													SELECT
														a.INSPECTIONDATE,
														sum( a.pass_qty ) pass_qty,
														sum( nvl( b.BAD_QTY, 0 ) ) bad_qty 
													FROM
														qcm_iqc_insp_res_m a
														LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
														AND a.CHK_SEQ = b.CHK_SEQ 
														AND a.ITEM_NO = b.ITEM_NO 
													WHERE
														a.INSPECTIONDATE BETWEEN '{startTime}' 
														AND '{endTime}' --写条件
		
													GROUP BY
														a.INSPECTIONDATE 
													),
													rqc_data AS (--移动端RQC
													SELECT
														a.createdate INSPECTIONDATE,
													sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
													sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
												FROM
													rqc_task_detail_t a
													LEFT JOIN RQC_TASK_M rm ON a.TASK_NO = rm.TASK_NO
													LEFT JOIN QCM_DQA_MAG_M qm ON qm.WORKSHOP_SECTION_NO = rm.WORKSHOP_SECTION_NO 
													AND rm.SHOE_NO = qm.SHOES_CODE
													LEFT JOIN BASE003M B ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
													LEFT JOIN FACTORYDATA ON FACTORYDATA.DEPARTMENT_CODE = RM.PRODUCTION_LINE_CODE 
													JOIN SJQDMS_ORGINFO sj ON FACTORYDATA.UDF05 = sj.code
												WHERE
													a.createdate BETWEEN '{startTime}' 
													AND '{endTime}' --写条件
													{RWSQL}
												GROUP BY
													a.createdate 
													),
													total_data AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ) SELECT
													to_char( to_date( INSPECTIONDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) insp_month,
													INSPECTIONDATE,
													sum( pass_qty ) pass_qty,
													sum( bad_qty ) bad_qty,
													round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 1 ) pass_rate 
												FROM
													total_data 
												GROUP BY
													INSPECTIONDATE";

				//多语言翻译
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "外观检验合格数", "外观检验不合格数", "合格率" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{

					name1 = LanDic["外观检验合格数"].ToString();
					name2 = LanDic["外观检验不合格数"].ToString();
					name3 = LanDic["合格率"].ToString();


				}

				RetDic["Histogram"] = Common.RetunHisData(DB, DayList, HistogramDataSql, "INSPECTIONDATE", "pass_qty", "bad_qty", "pass_rate", name1, name2, name3);//柱图1
                                                                                                                                                                             //扇形
                string CakeYearDataSQL = $@"WITH iqc_data AS (
	                                    SELECT
		                                    b.BADPROBLEM_NAME,
		                                    count( a.id ) bad_qty 
	                                    FROM
		                                    qcm_iqc_insp_res_d a
		                                    JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE 
	                                    WHERE
		                                    a.determine = 1 
		                                    AND a.createdate BETWEEN '{startTime}' 
		                                    AND '{endTime}' 
	                                    GROUP BY
		                                    b.BADPROBLEM_NAME 
	                                    ),
	                                    rqc_data AS (
	                                    SELECT
		                                    c.INSPECTION_NAME BADPROBLEM_NAME,
		                                    count( c.id ) bad_qty 
	                                    FROM
		                                    rqc_task_detail_t a
		                                    JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
		                                    AND a.COMMIT_INDEX = b.COMMIT_INDEX
		                                    JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
		                                    AND b.UNION_ID = c.ID 
	                                    WHERE
		                                    a.commit_type = 1 
		                                    AND a.CREATEDATE BETWEEN '{startTime}' 
		                                    AND '{endTime}' 
	                                    GROUP BY
		                                    c.INSPECTION_NAME 
	                                    ),
	                                    total_data AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ),
										sumData AS ( SELECT BADPROBLEM_NAME, sum( bad_qty ) bad_qty,ROW_NUMBER() over (ORDER BY sum(bad_qty) DESC) RANKING FROM total_data GROUP BY BADPROBLEM_NAME )
										SELECT BADPROBLEM_NAME,bad_qty FROM sumData WHERE RANKING<=5
										union ALL
										SELECT 'Other' BADPROBLEM_NAME, NVL( sum( bad_qty ), 0 ) bad_qty  FROM sumData WHERE RANKING>5 ";
                RetDic["cake"] = Common.ReturnCakeData(DB, CakeYearDataSQL, 0, "BADPROBLEM_NAME", "bad_qty", "");//饼图

                ret.RetData1 = RetDic;
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
		/// 外观检验--厂商数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		/// 
		#region APC_Code of GetdAllTableOneData
		//    public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableOneData(object OBJ)
		//    {
		//        SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
		//        SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
		//        SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
		//        try
		//        {
		//            DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
		//            string Data = ReqObj.Data.ToString();
		//            var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
		//            Dictionary<string, object> RetDic = new Dictionary<string, object>();

		//            #region 查询条件
		//            string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
		//            string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
		//            string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
		//            string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
		//            string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
		//            string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
		//            string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
		//            string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
		//            string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
		//            string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
		//            string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

		//bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

		//            string[] orderListfirmAC = firmType.Split(',');		//厂区分类
		//            string[] orderListfirmC = firmClass.Split(',');		//厂商分类
		//            string[] orderListfrim = frim.Split(',');			//厂商
		//            string[] orderListfrimArea = frimArea.Split(',');   //厂区
		//            string[] orderListwork = workshop.Split(',');		//工段
		//            string[] orderListparts = parts.Split(',');			//部件名称
		//            string[] orderListart = art.Split(',');				//ART
		//            string[] orderListpo = po.Split(',');               //PO
		//            string[] orderListItemno = itemNo.Split(',');       //料号
		//            #endregion

		//            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
		//            {
		//                throw new Exception("开始时间，结束时间为必填字段！");
		//            }

		//            #region 拼接条件
		//            string HisWhere = string.Empty;//柱状图条件
		//            string Frim = string.Empty;//厂商
		//            string FrimIQC = string.Empty;//厂商IQC
		//            string FrimRQC = string.Empty;//厂商RQC
		//            string PlantIQC = string.Empty;//厂区IQC
		//            string PlantRQC = string.Empty;//厂区RQC
		//            string ProductIQC = string.Empty;//产品IQC
		//            string ProductRQC = string.Empty;//产品RQC
		//            if (!string.IsNullOrEmpty(firmClass))//厂商类型
		//            {
		//                Frim += $@" AND e.M_TYPE IN ({string.Join(',',orderListfirmC.Select(x => $"'{x}'"))})";
		//            }
		//            if (!string.IsNullOrEmpty(frim))//厂商
		//            {
		//                HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//                FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//                FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//            }
		//            if (!string.IsNullOrEmpty(art))//ART
		//            {
		//                FrimIQC += $@" AND bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//                FrimRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//                PlantRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//                ProductRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//                HisWhere += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//            }
		//            if (!string.IsNullOrEmpty(po))//PO
		//            {
		//                HisWhere += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//                FrimRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//                PlantRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//                ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//            }
		//            if (!string.IsNullOrEmpty(workshop))//工段
		//            {
		//                FrimRQC += $@" AND se.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
		//            }
		//            if (!string.IsNullOrEmpty(itemNo))
		//            {
		//                FrimIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
		//                PlantIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
		//                ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";

		//            }
		//            if (!string.IsNullOrEmpty(parts))//部件
		//            {
		//                FrimIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
		//            }
		//#endregion

		//#region Manufacturer
		//string CSSQL = $@"WITH rcpt_insp_data AS (
		//			SELECT
		//				a.CHK_SEQ,
		//				a.CHK_NO,
		//				a.ITEM_NO,
		//				b1.SUPPLIERS_CODE,
		//				b1.SUPPLIERS_NAME,
		//				b1.M_TYPE
		//			FROM
		//				wms_rcpt_d a
		//				-- LEFT JOIN bdm_purchase_order_m a2 ON a.SOURCE_NO = a2.ORDER_NO
		//				left join bdm_rd_item a2 on a2.item_no = a.ITEM_NO
		//				JOIN base003m b1 ON a2.VEND_NO = b1.SUPPLIERS_CODE 
		//			),
		//			rcpt_data AS (
		//			SELECT
		//				a.SUPPLIERS_CODE,
		//				a.SUPPLIERS_NAME,
		//				count( DISTINCT b.chk_no ) rcpt_count,
		//				MAX(a.M_TYPE)  M_TYPE
		//			FROM
		//				rcpt_insp_data a
		//				JOIN wms_rcpt_m b ON a.CHK_NO = b.CHK_NO 
		//			WHERE
		//				b.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) 
		//				AND to_date( '{endTime}', 'yyyy-mm-dd' ) --write conditions

		//			GROUP BY
		//				a.SUPPLIERS_CODE,
		//				a.SUPPLIERS_NAME 
		//			),
		//			iqc_data AS (
		//			SELECT
		//				c.SUPPLIERS_NAME,
		//				sum( a.pass_qty ) pass_qty,
		//				sum( nvl( b.BAD_QTY, 0 ) ) bad_qty,
		//				count( DISTINCT a.CHK_NO ) insp_count 
		//			FROM
		//				qcm_iqc_insp_res_m a
		//				LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
		//				AND a.CHK_SEQ = b.CHK_SEQ 
		//				AND a.ITEM_NO = b.ITEM_NO
		//				JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
		//				AND a.CHK_SEQ = c.CHK_SEQ 
		//				AND a.ITEM_NO = c.ITEM_NO 
		//				LEFT JOIN WMS_RCPT_D wd ON wd.SOURCE_NO = a.CHK_NO AND wd.SOURCE_SEQ = a.CHK_SEQ
		//				left JOIN BDM_PURCHASE_ORDER_ITEM bi ON bi.ORDER_NO = wd.SOURCE_NO AND bi.ORDER_SEQ = wd.SOURCE_SEQ
		//				LEFT JOIN BDM_PURCHASE_ORDER_D bd ON bi.ORDER_NO = bd.ORDER_NO AND bi.ORDER_SEQ = bd.ORDER_SEQ
		//			WHERE
		//				a.INSPECTIONDATE BETWEEN '{startTime}' 
		//				AND '{endTime}' --写条件
		//				{FrimIQC}
		//			GROUP BY
		//				c.SUPPLIERS_NAME --,d.rcpt_count

		//			),
		//			rqc_data AS (
		//			SELECT
		//				a.SUPPLIERS_NAME,
		//			sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
		//			sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty,
		//			count( d.id ) insp_count 
		//		FROM
		//			rqc_task_m a
		//			JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO 
		//			JOIN bdm_workshop_section_m se ON a.workshop_section_no = se.workshop_section_no
		//		WHERE
		//			d.createdate BETWEEN '{startTime}' 
		//			AND '{endTime}' --写条件
		//			{FrimRQC}
		//		GROUP BY
		//			a.SUPPLIERS_NAME 
		//			),
		//			total_data_tmp AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ),
		//			total_data_tmp2 AS (--厂区基础数据
		//			SELECT
		//				SUPPLIERS_NAME,
		//						round(
		//						CASE

		//								WHEN ( sum( pass_qty ) + sum( bad_qty ) ) > 0 THEN
		//								( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100 ) ELSE 0 
		//							END,
		//							1 
		//						) pass_rate,
		//				sum( insp_count ) insp_count 
		//			FROM
		//				total_data_tmp 
		//			GROUP BY
		//				SUPPLIERS_NAME 
		//			),
		//			total_data AS ( SELECT SUPPLIERS_NAME, pass_rate, insp_count, row_number ( ) over ( ORDER BY pass_rate DESC ) total_ranking FROM total_data_tmp2 ),
		//			iqc_bad_data AS (
		//			SELECT
		//				c.SUPPLIERS_NAME,
		//				b.BADPROBLEM_NAME,
		//				count( a.id ) bad_qty 
		//			FROM
		//				qcm_iqc_insp_res_d a
		//				JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE
		//				JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
		//				AND a.CHK_SEQ = c.CHK_SEQ 
		//				AND a.ITEM_NO = c.ITEM_NO 
		//			WHERE
		//				a.determine = 1 
		//				AND a.createdate BETWEEN '{startTime}' 
		//				AND '{endTime}' 
		//			GROUP BY
		//				c.SUPPLIERS_NAME,
		//				b.BADPROBLEM_NAME 
		//			),
		//			rqc_bad_data AS (
		//			SELECT
		//				d.SUPPLIERS_NAME,
		//				c.INSPECTION_NAME BADPROBLEM_NAME,
		//				count( c.id ) bad_qty 
		//			FROM
		//				rqc_task_detail_t a
		//				JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
		//				AND a.COMMIT_INDEX = b.COMMIT_INDEX
		//				JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
		//				AND b.UNION_ID = c.ID
		//				JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO 
		//			WHERE
		//				a.commit_type = 1 
		//				AND a.CREATEDATE BETWEEN '{startTime}' 
		//				AND '{endTime}' 
		//			GROUP BY
		//				d.SUPPLIERS_NAME,
		//				c.INSPECTION_NAME 
		//			),
		//			total_bad_data_tmp AS ( --厂区不合格项目数量
		//			SELECT * FROM iqc_bad_data UNION ALL SELECT * FROM rqc_bad_data ),
		//			total_bad_data_tmp2 AS (
		//			SELECT
		//				SUPPLIERS_NAME,
		//				BADPROBLEM_NAME,
		//				sum( bad_qty ) bad_qty,
		//				ROW_NUMBER ( ) over ( partition BY SUPPLIERS_NAME ORDER BY sum( bad_qty ) DESC ) ranking 
		//			FROM
		//				total_bad_data_tmp 
		//			GROUP BY
		//				SUPPLIERS_NAME,
		//				BADPROBLEM_NAME 
		//			),
		//			total_bad_data AS ( SELECT SUPPLIERS_NAME, BADPROBLEM_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY SUPPLIERS_NAME ) totla_bad_qty FROM total_bad_data_tmp2 ) SELECT
		//			e.M_TYPE className,
		//			a.* ,
		//			e.rcpt_count,
		//			b.BADPROBLEM_NAME,
		//			round( b.bad_qty / b.totla_bad_qty * 100, 1 ) bad_item_rate,
		//			b.ranking ranking_1,
		//			c.BADPROBLEM_NAME,
		//			round( c.bad_qty / c.totla_bad_qty * 100, 1 ) bad_item_rate,
		//			c.ranking ranking_2,
		//			d.BADPROBLEM_NAME,
		//			round( d.bad_qty / d.totla_bad_qty * 100, 1 ) bad_item_rate,
		//			d.ranking ranking_3 
		//		FROM
		//			total_data a
		//			JOIN total_bad_data b ON a.SUPPLIERS_NAME = b.SUPPLIERS_NAME --left join is changed to join
		//			AND b.ranking = 1
		//			JOIN total_bad_data c ON a.SUPPLIERS_NAME = c.SUPPLIERS_NAME --left join is changed to join
		//			AND c.ranking = 2
		//			JOIN total_bad_data d ON a.SUPPLIERS_NAME = d.SUPPLIERS_NAME --left join is changed to join
		//			AND d.ranking = 3
		//			JOIN rcpt_data e ON a.SUPPLIERS_NAME = e.SUPPLIERS_NAME --left join is changed to join
		//		where 1=1 {Frim}";
		//            DataTable CSDT = DB.GetDataTable(CSSQL);
		//            #region 废弃查询显示字段
		//            /*e.M_TYPE className,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                a.PASS_RATE

		//                            ELSE

		//                                0

		//                        END) AS PASS_RATE,
		//                        a.* ,
		//			e.rcpt_count,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                b.BADPROBLEM_NAME
		//                            ELSE

		//                                NULL
		//                        END) AS BADPROBLEM_NAME,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                round(b.bad_qty / b.totla_bad_qty * 100, 1)

		//                            ELSE

		//                                NULL

		//                        END) AS bad_item_rate,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                c.BADPROBLEM_NAME

		//                            ELSE

		//                                NULL

		//                        END) AS BADPROBLEM_NAME,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                round(c.bad_qty / c.totla_bad_qty * 100, 1)

		//                            ELSE

		//                                NULL

		//                        END) AS bad_item_rate,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                d.BADPROBLEM_NAME

		//                            ELSE

		//                                NULL

		//                        END) AS BADPROBLEM_NAME,
		//			(CASE

		//                            WHEN e.rcpt_count > 0 THEN

		//                                round(d.bad_qty / d.totla_bad_qty * 100, 1)

		//                            ELSE

		//                                NULL

		//                        END) AS bad_item_rate,

		//                        --b.BADPROBLEM_NAME,
		//			--round(b.bad_qty / b.totla_bad_qty * 100, 1) bad_item_rate,
		//			b.ranking ranking_1,

		//                        --c.BADPROBLEM_NAME,
		//			--round(c.bad_qty / c.totla_bad_qty * 100, 1) bad_item_rate,
		//			c.ranking ranking_2,

		//                        --d.BADPROBLEM_NAME,
		//			--round(d.bad_qty / d.totla_bad_qty * 100, 1) bad_item_rate,
		//			d.ranking ranking_3*/
		//            #endregion
		//            List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
		//List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

		//RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CSDT,TopList, 20, IsExport);//厂商合格率前5

		//            if (!IsExport)
		//            {
		//	RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CSDT, LastList, 20);//厂商合格率后5
		//}

		//#endregion

		//ret.RetData1 = RetDic;
		//            ret.IsSuccess = true;
		//        }
		//        catch (Exception ex)
		//        {
		//            ret.IsSuccess = false;
		//            ret.ErrMsg = ex.Message;
		//        }
		//        return ret;
		//    }
		#endregion

		#region APE Code of GetdAllTableOneData
		//public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableOneData(object OBJ)
		//{
		//	SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
		//	SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
		//	SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
		//	try
		//	{
		//		DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
		//		string Data = ReqObj.Data.ToString();
		//		var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
		//		Dictionary<string, object> RetDic = new Dictionary<string, object>();

		//		#region 查询条件
		//		string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
		//		string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
		//		string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
		//		string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
		//		string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
		//		string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
		//		string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
		//		string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
		//		string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
		//		string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
		//		string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

		//		bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

		//		string[] orderListfirmAC = firmType.Split(',');     //厂区分类
		//		string[] orderListfirmC = firmClass.Split(',');     //厂商分类
		//		string[] orderListfrim = frim.Split(',');           //厂商
		//		string[] orderListfrimArea = frimArea.Split(',');   //厂区
		//		string[] orderListwork = workshop.Split(',');       //工段
		//		string[] orderListparts = parts.Split(',');         //部件名称
		//		string[] orderListart = art.Split(',');             //ART
		//		string[] orderListpo = po.Split(',');               //PO
		//		string[] orderListItemno = itemNo.Split(',');       //料号
		//		#endregion

		//		if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
		//		{
		//			throw new Exception("开始时间，结束时间为必填字段！");
		//		}

		//		#region 拼接条件
		//		string HisWhere = string.Empty;//柱状图条件
		//		string Frim = string.Empty;//厂商
		//		string FrimIQC = string.Empty;//厂商IQC
		//		string FrimRQC = string.Empty;//厂商RQC
		//		string PlantIQC = string.Empty;//厂区IQC
		//		string PlantRQC = string.Empty;//厂区RQC
		//		string ProductIQC = string.Empty;//产品IQC
		//		string ProductRQC = string.Empty;//产品RQC
		//		string IWhere = string.Empty; //IQC
		//		string RWhere = string.Empty; //RQC
		//		if (!string.IsNullOrEmpty(firmClass))//厂商类型
		//		{
		//			// Frim += $@" AND e.M_TYPE IN ({string.Join(',',orderListfirmC.Select(x => $"'{x}'"))})";
		//			IWhere += $@" AND f.M_TYPE IN ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND bp.M_TYPE IN ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(frim))//厂商
		//		{
		//			HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";

		//			IWhere += $@" AND f.SUPPLIERS_NAME IN ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND bp.SUPPLIERS_NAME IN ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(firmType))//厂区类型
		//		{
		//			// Frim += $@" AND e.M_TYPE IN ({string.Join(',',orderListfirmC.Select(x => $"'{x}'"))})";
		//			IWhere += $@" AND h.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND FACTORYDATA.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(frimArea))//厂区
		//		{
		//			HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
		//			IWhere += $@" AND h.ORG IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND sj.ORG IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(art))//ART
		//		{
		//			FrimIQC += $@" AND bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//			FrimRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//			PlantRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//			ProductRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//			HisWhere += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";

		//			IWhere += $@" AND g.ART_NO IN ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND a.PROD_NO IN ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(po))//PO
		//		{
		//			HisWhere += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//			FrimRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//			PlantRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//			ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";

		//			IWhere += $@" AND g.SALES_ORDER	 IN (select se_id from bdm_se_order_master where mer_po in({string.Join(',', orderListpo.Select(x => $"'{x}'"))}))";
		//			RWhere += $@" AND a.MER_PO IN ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(workshop))//工段
		//		{
		//			FrimRQC += $@" AND se.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
		//			RWhere += $@" AND qm.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(itemNo))//料号
		//		{
		//			FrimIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
		//			PlantIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
		//			ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";

		//			IWhere += $@" AND b.ITEM_NO IN ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
		//		}
		//		if (!string.IsNullOrEmpty(parts))//部件
		//		{
		//			FrimIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
		//		}
		//		#endregion

		//		#region 厂商
		//		#region 商基写的
		//		/*string CSSQL = $@"WITH rcpt_insp_data AS (
		//					SELECT
		//						a.CHK_SEQ,
		//						a.CHK_NO,
		//						a.ITEM_NO,
		//						b1.SUPPLIERS_CODE,
		//						b1.SUPPLIERS_NAME,
		//						b1.M_TYPE
		//					FROM
		//						wms_rcpt_d a
		//						-- LEFT JOIN bdm_purchase_order_m a2 ON a.SOURCE_NO = a2.ORDER_NO
		//						left join bdm_rd_item a2 on a2.item_no = a.ITEM_NO
		//						JOIN base003m b1 ON a2.VEND_NO = b1.SUPPLIERS_CODE 
		//					),
		//					rcpt_data AS (
		//					SELECT
		//						a.SUPPLIERS_CODE,
		//						a.SUPPLIERS_NAME,
		//						count( DISTINCT b.chk_no ) rcpt_count,
		//						MAX(a.M_TYPE)  M_TYPE
		//					FROM
		//						rcpt_insp_data a
		//						JOIN wms_rcpt_m b ON a.CHK_NO = b.CHK_NO 
		//					WHERE
		//						b.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) 
		//						AND to_date( '{endTime}', 'yyyy-mm-dd' ) --写条件
		
		//					GROUP BY
		//						a.SUPPLIERS_CODE,
		//						a.SUPPLIERS_NAME 
		//					),
		//					iqc_data AS (
		//					SELECT
		//						c.SUPPLIERS_NAME,
		//						sum( a.pass_qty ) pass_qty,
		//						sum( nvl( b.BAD_QTY, 0 ) ) bad_qty,
		//						count( DISTINCT a.CHK_NO ) insp_count 
		//					FROM
		//						qcm_iqc_insp_res_m a
		//						LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
		//						AND a.CHK_SEQ = b.CHK_SEQ 
		//						AND a.ITEM_NO = b.ITEM_NO
		//						JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
		//						AND a.CHK_SEQ = c.CHK_SEQ 
		//						AND a.ITEM_NO = c.ITEM_NO 
		//						LEFT JOIN WMS_RCPT_D wd ON wd.SOURCE_NO = a.CHK_NO AND wd.SOURCE_SEQ = a.CHK_SEQ
		//						left JOIN BDM_PURCHASE_ORDER_ITEM bi ON bi.ORDER_NO = wd.SOURCE_NO AND bi.ORDER_SEQ = wd.SOURCE_SEQ
		//						LEFT JOIN BDM_PURCHASE_ORDER_D bd ON bi.ORDER_NO = bd.ORDER_NO AND bi.ORDER_SEQ = bd.ORDER_SEQ
		//					WHERE
		//						a.INSPECTIONDATE BETWEEN '{startTime}' 
		//						AND '{endTime}' --写条件
		//						{FrimIQC}
		//					GROUP BY
		//						c.SUPPLIERS_NAME --,d.rcpt_count
		
		//					),
		//					rqc_data AS (
		//					SELECT
		//						a.SUPPLIERS_NAME,
		//					sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
		//					sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty,
		//					count( d.id ) insp_count 
		//				FROM
		//					rqc_task_m a
		//					JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO 
		//					JOIN bdm_workshop_section_m se ON a.workshop_section_no = se.workshop_section_no
		//				WHERE
		//					d.createdate BETWEEN '{startTime}' 
		//					AND '{endTime}' --写条件
		//					{FrimRQC}
		//				GROUP BY
		//					a.SUPPLIERS_NAME 
		//					),
		//					total_data_tmp AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ),
		//					total_data_tmp2 AS (--厂区基础数据
		//					SELECT
		//						SUPPLIERS_NAME,
		//								round(
		//								CASE
				
		//										WHEN ( sum( pass_qty ) + sum( bad_qty ) ) > 0 THEN
		//										( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100 ) ELSE 0 
		//									END,
		//									1 
		//								) pass_rate,
		//						sum( insp_count ) insp_count 
		//					FROM
		//						total_data_tmp 
		//					GROUP BY
		//						SUPPLIERS_NAME 
		//					),
		//					total_data AS ( SELECT SUPPLIERS_NAME, pass_rate, insp_count, row_number ( ) over ( ORDER BY pass_rate DESC ) total_ranking FROM total_data_tmp2 ),
		//					iqc_bad_data AS (
		//					SELECT
		//						c.SUPPLIERS_NAME,
		//						b.BADPROBLEM_NAME,
		//						count( a.id ) bad_qty 
		//					FROM
		//						qcm_iqc_insp_res_d a
		//						JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE
		//						JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
		//						AND a.CHK_SEQ = c.CHK_SEQ 
		//						AND a.ITEM_NO = c.ITEM_NO 
		//					WHERE
		//						a.determine = 1 
		//						AND a.createdate BETWEEN '{startTime}' 
		//						AND '{endTime}' 
		//					GROUP BY
		//						c.SUPPLIERS_NAME,
		//						b.BADPROBLEM_NAME 
		//					),
		//					rqc_bad_data AS (
		//					SELECT
		//						d.SUPPLIERS_NAME,
		//						c.INSPECTION_NAME BADPROBLEM_NAME,
		//						count( c.id ) bad_qty 
		//					FROM
		//						rqc_task_detail_t a
		//						JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
		//						AND a.COMMIT_INDEX = b.COMMIT_INDEX
		//						JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
		//						AND b.UNION_ID = c.ID
		//						JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO 
		//					WHERE
		//						a.commit_type = 1 
		//						AND a.CREATEDATE BETWEEN '{startTime}' AND '{endTime}' 
		//					GROUP BY
		//						d.SUPPLIERS_NAME,
		//						c.INSPECTION_NAME 
		//					),
		//					total_bad_data_tmp AS ( --厂区不合格项目数量
		//					SELECT * FROM iqc_bad_data UNION ALL SELECT * FROM rqc_bad_data ),
		//					total_bad_data_tmp2 AS (
		//					SELECT
		//						SUPPLIERS_NAME,
		//						BADPROBLEM_NAME,
		//						sum( bad_qty ) bad_qty,
		//						ROW_NUMBER ( ) over ( partition BY SUPPLIERS_NAME ORDER BY sum( bad_qty ) DESC ) ranking 
		//					FROM
		//						total_bad_data_tmp 
		//					GROUP BY
		//						SUPPLIERS_NAME,
		//						BADPROBLEM_NAME 
		//					),
		//					total_bad_data AS ( SELECT SUPPLIERS_NAME, BADPROBLEM_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY SUPPLIERS_NAME ) totla_bad_qty FROM total_bad_data_tmp2 ) SELECT
		//					e.M_TYPE className,
		//					a.* ,
		//					e.rcpt_count,
		//					b.BADPROBLEM_NAME,
		//					round( b.bad_qty / b.totla_bad_qty * 100, 1 ) bad_item_rate,
		//					b.ranking ranking_1,
		//					c.BADPROBLEM_NAME,
		//					round( c.bad_qty / c.totla_bad_qty * 100, 1 ) bad_item_rate,
		//					c.ranking ranking_2,
		//					d.BADPROBLEM_NAME,
		//					round( d.bad_qty / d.totla_bad_qty * 100, 1 ) bad_item_rate,
		//					d.ranking ranking_3 
		//				FROM
		//					total_data a
		//					LEFT JOIN total_bad_data b ON a.SUPPLIERS_NAME = b.SUPPLIERS_NAME 
		//					AND b.ranking = 1
		//					LEFT JOIN total_bad_data c ON a.SUPPLIERS_NAME = c.SUPPLIERS_NAME 
		//					AND c.ranking = 2
		//					LEFT JOIN total_bad_data d ON a.SUPPLIERS_NAME = d.SUPPLIERS_NAME 
		//					AND d.ranking = 3
		//					LEFT JOIN rcpt_data e ON a.SUPPLIERS_NAME = e.SUPPLIERS_NAME
		//				where 1=1 {Frim}";*/
		//		#endregion
		//		string CSSQL = $@"with 
  //                FACTORYDATA AS ( SELECT BASE005M.DEPARTMENT_CODE, BASE005M.UDF05, ORG_CODE, ORG_NAME FROM BASE005M LEFT JOIN BASE001M ON BASE005M.FACTORY_SAP = BASE001M.ORG_CODE ),
  //              sedata_org as (   select distinct a.se_id, b.UDF05,D.ORG,b.FACTORY_SAP,C.ORG_NAME
  //                 from SJQDMS_WORK_DAY_SIZE a
  //                 join base005m b
  //                   on a.D_DEPT = b.department_code
  //                  and b.UDF01 = 'L'
  //                 join base001m c on b.FACTORY_SAP = c.org_code
  //                 join SJQDMS_ORGINFO D on D.CODE = b.UDF05),

  //              iqc_data as (
  //               select distinct f.M_TYPE,
  //                  f.suppliers_name,
  //                    count(distinct b.rcpt_qty) over(partition by f.M_TYPE,f.suppliers_name) wms_num,
  //                   count(distinct c.pass_qty) over(partition by f.M_TYPE,f.suppliers_name) ins_num,
  //                  count(distinct decode(c.determine,'0',c.pass_qty,null)) over(partition by f.M_TYPE,f.suppliers_name) pass_num,
  //                  count(distinct decode(c.determine,'1',c.pass_qty,null)) over(partition by f.M_TYPE,f.suppliers_name) fail_num 
  //                 from wms_rcpt_m a
  //                 join (select rownum as rcpt_id,a.* from wms_rcpt_d a) b
  //                 on a.chk_no = b.chk_no
  //                 left join qcm_iqc_insp_res_m c
  //                 on b.chk_no = c.chk_no
  //                and b.chk_seq = c.chk_seq
  //                 left join bdm_rd_item e
  //                 on b.item_no = e.item_no
  //                 left join base003m f
  //                 on f.suppliers_code = e.VEND_NO_PRD
  //                 left join bdm_purchase_order_d g
  //                on g.order_no = b.source_no
  //                and g.order_seq = b.source_seq
  //                 left join sedata_org h on
  //                 g.sales_order = h.se_id
  //                where a.rcpt_by = '01'
  //                and 1 = 1 --写条件
		//						   and a.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) AND to_date( '{endTime}', 'yyyy-mm-dd' )
		//						{IWhere}
		//						   ),
   
		//						iqc_bad_data as (
		//						select M_TYPE,suppliers_name,BADPROBLEM_NAME,sum(bad_qty) bad_qty from (
		//						 select
		//								f.M_TYPE,
		//								f.suppliers_name,
		//								ap.id,
		//								ab.BADPROBLEM_NAME,
		//								ac.BAD_QTY bad_qty,
		//								row_number() over(partition by ap.ID order by ap.id desc) index_rn
		//						   from wms_rcpt_m a
		//						   join (select rownum as rcpt_id,a.* from wms_rcpt_d a) b
		//							 on a.chk_no = b.chk_no
		//						   left join qcm_iqc_insp_res_m c
		//							 on b.chk_no = c.chk_no
		//							and b.chk_seq = c.chk_seq
		//						   left join bdm_rd_item e
		//							 on b.item_no = e.item_no
		//						   left join base003m f
		//							 on f.suppliers_code = e.VEND_NO_PRD
		//						   left join qcm_iqc_insp_res_d ap on c.chk_no = ap.chk_no and c.chk_seq = ap.chk_seq
		//						   JOIN qcm_iqc_badproblems_m ab ON ap.BADPROBLEM_CODE = ab.BADPROBLEM_CODE 
		//						   LEFT JOIN qcm_iqc_insp_res_bad_report ac on ac.CHK_NO = ap.CHK_NO and  ac.CHK_SEQ = ap.CHK_SEQ
		//						   left join bdm_purchase_order_d g
		//							on g.order_no = b.source_no
		//							and g.order_seq = b.source_seq
		//						   left join sedata_org h on
		//							 g.sales_order = h.se_id
		//						  where a.rcpt_by = '01'
		//							and  ap.determine = 1 
		//							and 1 = 1 --写条件
		//							and a.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) AND to_date( '{endTime}', 'yyyy-mm-dd')
		//							{IWhere}
		//							) where INDEX_RN='1'
		//							 group by M_TYPE,suppliers_name,BADPROBLEM_NAME
		//						   ),
		//						 rqc_batch as
		//						 (select  ART_NO, count(distinct bill_no) as num
		//							from mms_vendor_income
		//						   group by ART_NO),
		//						rqc_data as (
		//						select distinct
		//										bp.m_Type,
		//										bp.suppliers_name,             
		//										count(decode(a.RESULT,'0',1,null,1,null)) as pass_num,
		//										count(decode(a.RESULT,'1',1,null)) as fail_num,
		//										sum(distinct d.num) wms_num,
		//										count(distinct a.TASK_NO) ins_num
		//						  from rqc_task_m a
		//						  left join base005m b
		//							on a.PRODUCTION_LINE_CODE = b.department_code
		//						  LEFT JOIN QCM_DQA_MAG_M qm
		//							 ON qm.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
		//							 AND a.SHOE_NO = qm.SHOES_CODE
		//						  left join base003m bp
		//							on a.SUPPLIERS_CODE = bp.SUPPLIERS_CODE
		//						  LEFT JOIN FACTORYDATA
		//							  ON FACTORYDATA.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
		//						  left JOIN SJQDMS_ORGINFO sj
		//							  ON FACTORYDATA.UDF05 = sj.code
		//						  left join rqc_batch d
		//							on a.prod_no = d.art_no
		//						 where 1=1 and --写条件
		//						 a.CREATEDATE BETWEEN '{startTime}' AND '{endTime}' 
		//						{RWhere}
		//						 group by bp.suppliers_name, bp.m_Type
		//						 ),
		//						rqc_bad_date as (
		//						  SELECT 
		//										BP.m_Type,
		//										BP.suppliers_name, 
		//										c.INSPECTION_NAME BADPROBLEM_NAME, 
		//										 count(c.id) bad_qty
		//							FROM rqc_task_detail_t ac
		//							JOIN rqc_task_detail_t_d b
		//							  ON ac.TASK_NO = b.TASK_NO
		//							 AND ac.COMMIT_INDEX = b.COMMIT_INDEX
		//							JOIN rqc_task_item_c c
		//							  ON b.TASK_NO = c.TASK_NO
		//							 AND b.UNION_ID = c.ID
		//							join RQC_TASK_M a
		//							  on a.task_no = ac.task_no
		//							LEFT JOIN QCM_DQA_MAG_M qm
		//							  ON qm.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
		//							 AND a.SHOE_NO = qm.SHOES_CODE
		//							LEFT JOIN BASE003M BP
		//							  ON BP.SUPPLIERS_CODE = a.SUPPLIERS_CODE
		//							LEFT JOIN FACTORYDATA
		//							  ON FACTORYDATA.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
		//							JOIN SJQDMS_ORGINFO sj
		//							  ON FACTORYDATA.UDF05 = sj.code
		//						   WHERE ac.commit_type = 1 and 1=1 --写条件
		//							AND a.CREATEDATE BETWEEN '{startTime}' AND '{endTime}' 
		//							{RWhere}
		//						   GROUP BY 
		//							BP.m_Type,
		//							 BP.suppliers_name, 
		//						   c.INSPECTION_NAME
		//						 ),
		//						 tatol_bad_data as (
		//						 select a.*,sum(a.BAD_QTY) over(partition by M_TYPE, SUPPLIERS_NAME) tatol_num from (
		//						 select M_TYPE,
		//								SUPPLIERS_NAME,
		//								BADPROBLEM_NAME,
		//								sum(BAD_QTY) BAD_QTY,
		//								row_number() over(partition by M_TYPE, SUPPLIERS_NAME order by sum(BAD_QTY) desc) index_rn
		//						   from (select * from rqc_bad_date union all select * from iqc_bad_data)
		//						  group by M_TYPE, SUPPLIERS_NAME, BADPROBLEM_NAME) a 
		//						  ),
		//						  tatol_data as (
		//									 select M_TYPE,
		//											SUPPLIERS_NAME,
		//											sum(PASS_NUM) PASS_NUM,
		//											sum(FAIL_NUM) FAIL_NUM,
		//											sum(WMS_NUM) WMS_NUM,
		//											sum(INS_NUM) INS_NUM
		//									   from (select * from iqc_data union all select * from rqc_data)
		//									   group by M_TYPE,SUPPLIERS_NAME
		//						  )
		//						select 
		//							   rm.M_TYPE CLASSNAME,
		//							   rm.SUPPLIERS_NAME SUPPLIERS_NAME,
		//							   rm.WMS_NUM INSP_COUNT,
		//							   rm.INS_NUM RCPT_COUNT,
		//							   (case when rm.INS_NUM>0 then round(rm.pass_num/rm.INS_NUM*100,2) else 0 end ) as PASS_RATE
		//							  -- a.BADPROBLEM_NAME BADPROBLEM_NAME,
		//							  -- round(a.BAD_QTY/a.TATOL_NUM*100,2) BAD_ITEM_RATE,
		//							  -- b.BADPROBLEM_NAME  BADPROBLEM_NAME1,
		//							  -- round(b.BAD_QTY/a.TATOL_NUM*100,2)   BAD_ITEM_RATE1,
		//							  -- c.BADPROBLEM_NAME  BADPROBLEM_NAME2,
		//							  -- round(c.BAD_QTY/a.TATOL_NUM*100,2) BAD_ITEM_RATE2,
		//							  -- a.TATOL_NUM
		//						  from tatol_data rm
		//						  --left join tatol_bad_data a
		//							--on rm.SUPPLIERS_NAME = a.SUPPLIERS_NAME
		//							--and a.index_rn = 1
		//						  --left join tatol_bad_data b
		//							--on rm.SUPPLIERS_NAME = b.SUPPLIERS_NAME
		//							--and b.index_rn = 2
		//						 -- left join tatol_bad_data c
		//							--on rm.SUPPLIERS_NAME = c.SUPPLIERS_NAME 
		//							--and c.index_rn = 3
		//						 where rm.SUPPLIERS_NAME is not null
		//						order by PASS_RATE desc";

		//		DataTable CSDT = DB.GetDataTable(CSSQL);
		//		#region 废弃查询显示字段
		//		/*e.M_TYPE className,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  a.PASS_RATE

  //                              ELSE

  //                                  0

  //                          END) AS PASS_RATE,
  //                          a.* ,
		//					e.rcpt_count,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  b.BADPROBLEM_NAME
  //                              ELSE

  //                                  NULL
  //                          END) AS BADPROBLEM_NAME,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  round(b.bad_qty / b.totla_bad_qty * 100, 1)

  //                              ELSE

  //                                  NULL

  //                          END) AS bad_item_rate,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  c.BADPROBLEM_NAME

  //                              ELSE

  //                                  NULL

  //                          END) AS BADPROBLEM_NAME,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  round(c.bad_qty / c.totla_bad_qty * 100, 1)

  //                              ELSE

  //                                  NULL

  //                          END) AS bad_item_rate,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  d.BADPROBLEM_NAME

  //                              ELSE

  //                                  NULL

  //                          END) AS BADPROBLEM_NAME,
		//					(CASE

  //                              WHEN e.rcpt_count > 0 THEN

  //                                  round(d.bad_qty / d.totla_bad_qty * 100, 1)

  //                              ELSE

  //                                  NULL

  //                          END) AS bad_item_rate,

  //                          --b.BADPROBLEM_NAME,
		//					--round(b.bad_qty / b.totla_bad_qty * 100, 1) bad_item_rate,
		//					b.ranking ranking_1,

  //                          --c.BADPROBLEM_NAME,
		//					--round(c.bad_qty / c.totla_bad_qty * 100, 1) bad_item_rate,
		//					c.ranking ranking_2,

  //                          --d.BADPROBLEM_NAME,
		//					--round(d.bad_qty / d.totla_bad_qty * 100, 1) bad_item_rate,
		//					d.ranking ranking_3*/
		//		#endregion
		//		List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
		//		List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

		//		RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CSDT, TopList, 20, IsExport);//厂商合格率前5

		//		if (!IsExport)
		//		{
		//			RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CSDT, LastList, 20);//厂商合格率后5
		//		}

		//		#endregion

		//		ret.RetData1 = RetDic;
		//		ret.IsSuccess = true;
		//	}
		//	catch (Exception ex)
		//	{
		//		ret.IsSuccess = false;
		//		ret.ErrMsg = ex.Message;
		//	}
		//	return ret;
		//}

		#endregion


		#region Code of GetdAllTableOneData written by Ashok
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableOneData(object OBJ)
		{
			SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
			SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
			SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
			try
			{
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
				string Data = ReqObj.Data.ToString();
				var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
				Dictionary<string, object> RetDic = new Dictionary<string, object>();

				#region 查询条件
				string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
				string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
				string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
				string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

				bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

				string[] orderListfirmAC = firmType.Split(',');     //厂区分类
				string[] orderListfirmC = firmClass.Split(',');     //厂商分类
				string[] orderListfrim = frim.Split(',');           //厂商
				string[] orderListfrimArea = frimArea.Split(',');   //厂区
				string[] orderListwork = workshop.Split(',');       //工段
				string[] orderListparts = parts.Split(',');         //部件名称
				string[] orderListart = art.Split(',');             //ART
				string[] orderListpo = po.Split(',');               //PO
				string[] orderListItemno = itemNo.Split(',');       //料号
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("Start time and end time are required fields!");
				}

				#region 拼接条件
				string HisWhere = string.Empty;//柱状图条件
				string Frim = string.Empty;//厂商
				string FrimIQC = string.Empty;//厂商IQC
				string FrimRQC = string.Empty;//厂商RQC
				string PlantIQC = string.Empty;//厂区IQC
				string PlantRQC = string.Empty;//厂区RQC
				string ProductIQC = string.Empty;//产品IQC
				string ProductRQC = string.Empty;//产品RQC
				string IWhere = string.Empty; //IQC
				string RWhere = string.Empty; //RQC
				if (!string.IsNullOrEmpty(firmClass))//厂商类型
				{
					// Frim += $@" AND e.M_TYPE IN ({string.Join(',',orderListfirmC.Select(x => $"'{x}'"))})";
					IWhere += $@" AND f.M_TYPE IN ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
					RWhere += $@" AND bp.M_TYPE IN ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frim))//厂商
				{
					HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";

					IWhere += $@" AND f.SUPPLIERS_NAME IN ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					RWhere += $@" AND bp.SUPPLIERS_NAME IN ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(firmType))//厂区类型
				{
					// Frim += $@" AND e.M_TYPE IN ({string.Join(',',orderListfirmC.Select(x => $"'{x}'"))})";
					IWhere += $@" AND h.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
					RWhere += $@" AND FACTORYDATA.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frimArea))//厂区
				{
					HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					IWhere += $@" AND h.ORG IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
					RWhere += $@" AND sj.ORG IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))//ART
				{
					FrimIQC += $@" AND bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					FrimRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					PlantRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					ProductRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					HisWhere += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";

					IWhere += $@" AND g.ART_NO IN ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					RWhere += $@" AND a.PROD_NO IN ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))//PO
				{
					HisWhere += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
					FrimRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
					PlantRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
					ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";

					IWhere += $@" AND g.SALES_ORDER	 IN (select se_id from bdm_se_order_master where mer_po in({string.Join(',', orderListpo.Select(x => $"'{x}'"))}))";
					RWhere += $@" AND a.MER_PO IN ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(workshop))//工段
				{
					FrimRQC += $@" AND se.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
					RWhere += $@" AND qm.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(itemNo))//料号
				{
					FrimIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
					PlantIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
					ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";

					IWhere += $@" AND b.ITEM_NO IN ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(parts))//部件
				{
					FrimIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
				}
                #endregion

                #region 厂商
                #region Old Query
                //string CSSQL = $@"with 
                //              FACTORYDATA AS ( SELECT BASE005M.DEPARTMENT_CODE, BASE005M.UDF05, ORG_CODE, ORG_NAME FROM BASE005M LEFT JOIN BASE001M ON BASE005M.FACTORY_SAP = BASE001M.ORG_CODE ),
                //            sedata_org as (   select distinct a.se_id, b.UDF05,D.ORG,b.FACTORY_SAP,C.ORG_NAME
                //               from SJQDMS_WORK_DAY_SIZE a
                //               join base005m b
                //                 on a.D_DEPT = b.department_code
                //                and b.UDF01 = 'L'
                //               join base001m c on b.FACTORY_SAP = c.org_code
                //               join SJQDMS_ORGINFO D on D.CODE = b.UDF05),

                //            iqc_data as (
                //             select distinct f.M_TYPE,
                //                f.suppliers_name,
                //                  count(distinct b.rcpt_qty) over(partition by f.M_TYPE,f.suppliers_name) wms_num,
                //                 count(distinct c.pass_qty) over(partition by f.M_TYPE,f.suppliers_name) ins_num,
                //                count(distinct decode(c.determine,'0',c.pass_qty,null)) over(partition by f.M_TYPE,f.suppliers_name) pass_num,
                //                count(distinct decode(c.determine,'1',c.pass_qty,null)) over(partition by f.M_TYPE,f.suppliers_name) fail_num 
                //               from wms_rcpt_m a
                //               join (select rownum as rcpt_id,a.* from wms_rcpt_d a) b
                //               on a.chk_no = b.chk_no
                //               left join qcm_iqc_insp_res_m c
                //               on b.chk_no = c.chk_no
                //              and b.chk_seq = c.chk_seq
                //               left join bdm_rd_item e
                //               on b.item_no = e.item_no
                //               left join base003m f
                //               on f.suppliers_code = e.VEND_NO_PRD
                //               left join bdm_purchase_order_d g
                //              on g.order_no = b.source_no
                //              and g.order_seq = b.source_seq
                //               left join sedata_org h on
                //               g.sales_order = h.se_id
                //              where a.rcpt_by = '01'
                //              and 1 = 1 --写条件
                //				   and a.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) AND to_date( '{endTime}', 'yyyy-mm-dd' )
                //				{IWhere}
                //				   ),

                //				iqc_bad_data as (
                //				select M_TYPE,suppliers_name,BADPROBLEM_NAME,sum(bad_qty) bad_qty from (
                //				 select
                //						f.M_TYPE,
                //						f.suppliers_name,
                //						ap.id,
                //						ab.BADPROBLEM_NAME,
                //						ac.BAD_QTY bad_qty,
                //						row_number() over(partition by ap.ID order by ap.id desc) index_rn
                //				   from wms_rcpt_m a
                //				   join (select rownum as rcpt_id,a.* from wms_rcpt_d a) b
                //					 on a.chk_no = b.chk_no
                //				   left join qcm_iqc_insp_res_m c
                //					 on b.chk_no = c.chk_no
                //					and b.chk_seq = c.chk_seq
                //				   left join bdm_rd_item e
                //					 on b.item_no = e.item_no
                //				   left join base003m f
                //					 on f.suppliers_code = e.VEND_NO_PRD
                //				   left join qcm_iqc_insp_res_d ap on c.chk_no = ap.chk_no and c.chk_seq = ap.chk_seq
                //				   JOIN qcm_iqc_badproblems_m ab ON ap.BADPROBLEM_CODE = ab.BADPROBLEM_CODE 
                //				   LEFT JOIN qcm_iqc_insp_res_bad_report ac on ac.CHK_NO = ap.CHK_NO and  ac.CHK_SEQ = ap.CHK_SEQ
                //				   left join bdm_purchase_order_d g
                //					on g.order_no = b.source_no
                //					and g.order_seq = b.source_seq
                //				   left join sedata_org h on
                //					 g.sales_order = h.se_id
                //				  where a.rcpt_by = '01'
                //					and  ap.determine = 1 
                //					and 1 = 1 --写条件
                //					and a.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) AND to_date( '{endTime}', 'yyyy-mm-dd')
                //					{IWhere}
                //					) where INDEX_RN='1'
                //					 group by M_TYPE,suppliers_name,BADPROBLEM_NAME
                //				   ),
                //				 rqc_batch as
                //				 (select  ART_NO, count(distinct bill_no) as num
                //					from mms_vendor_income
                //				   group by ART_NO),
                //				rqc_data as (
                //				select distinct
                //								bp.m_Type,
                //								bp.suppliers_name,             
                //								count(decode(a.RESULT,'0',1,null,1,null)) as pass_num,
                //								count(decode(a.RESULT,'1',1,null)) as fail_num,
                //								sum(distinct d.num) wms_num,
                //								count(distinct a.TASK_NO) ins_num
                //				  from rqc_task_m a
                //				  left join base005m b
                //					on a.PRODUCTION_LINE_CODE = b.department_code
                //				  LEFT JOIN QCM_DQA_MAG_M qm
                //					 ON qm.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
                //					 AND a.SHOE_NO = qm.SHOES_CODE
                //				  left join base003m bp
                //					on a.SUPPLIERS_CODE = bp.SUPPLIERS_CODE
                //				  LEFT JOIN FACTORYDATA
                //					  ON FACTORYDATA.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
                //				  left JOIN SJQDMS_ORGINFO sj
                //					  ON FACTORYDATA.UDF05 = sj.code
                //				  left join rqc_batch d
                //					on a.prod_no = d.art_no
                //				 where 1=1 and --写条件
                //				 a.CREATEDATE BETWEEN '{startTime}' AND '{endTime}' 
                //				{RWhere}
                //				 group by bp.suppliers_name, bp.m_Type
                //				 ),
                //				rqc_bad_date as (
                //				  SELECT 
                //								BP.m_Type,
                //								BP.suppliers_name, 
                //								c.INSPECTION_NAME BADPROBLEM_NAME, 
                //								 count(c.id) bad_qty
                //					FROM rqc_task_detail_t ac
                //					JOIN rqc_task_detail_t_d b
                //					  ON ac.TASK_NO = b.TASK_NO
                //					 AND ac.COMMIT_INDEX = b.COMMIT_INDEX
                //					JOIN rqc_task_item_c c
                //					  ON b.TASK_NO = c.TASK_NO
                //					 AND b.UNION_ID = c.ID
                //					join RQC_TASK_M a
                //					  on a.task_no = ac.task_no
                //					LEFT JOIN QCM_DQA_MAG_M qm
                //					  ON qm.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
                //					 AND a.SHOE_NO = qm.SHOES_CODE
                //					LEFT JOIN BASE003M BP
                //					  ON BP.SUPPLIERS_CODE = a.SUPPLIERS_CODE
                //					LEFT JOIN FACTORYDATA
                //					  ON FACTORYDATA.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
                //					JOIN SJQDMS_ORGINFO sj
                //					  ON FACTORYDATA.UDF05 = sj.code
                //				   WHERE ac.commit_type = 1 and 1=1 --写条件
                //					AND a.CREATEDATE BETWEEN '{startTime}' AND '{endTime}' 
                //					{RWhere}
                //				   GROUP BY 
                //					BP.m_Type,
                //					 BP.suppliers_name, 
                //				   c.INSPECTION_NAME
                //				 ),
                //				 tatol_bad_data as (
                //				 select a.*,sum(a.BAD_QTY) over(partition by M_TYPE, SUPPLIERS_NAME) tatol_num from (
                //				 select M_TYPE,
                //						SUPPLIERS_NAME,
                //						BADPROBLEM_NAME,
                //						sum(BAD_QTY) BAD_QTY,
                //						row_number() over(partition by M_TYPE, SUPPLIERS_NAME order by sum(BAD_QTY) desc) index_rn
                //				   from (select * from rqc_bad_date union all select * from iqc_bad_data)
                //				  group by M_TYPE, SUPPLIERS_NAME, BADPROBLEM_NAME) a 
                //				  ),
                //				  tatol_data as (
                //							 select M_TYPE,
                //									SUPPLIERS_NAME,
                //									sum(PASS_NUM) PASS_NUM,
                //									sum(FAIL_NUM) FAIL_NUM,
                //									sum(WMS_NUM) WMS_NUM,
                //									sum(INS_NUM) INS_NUM
                //							   from (select * from iqc_data union all select * from rqc_data)
                //							   group by M_TYPE,SUPPLIERS_NAME
                //				  )
                //				select 
                //					   rm.M_TYPE CLASSNAME,
                //					   rm.SUPPLIERS_NAME SUPPLIERS_NAME,
                //					   rm.WMS_NUM INSP_COUNT,
                //					   rm.INS_NUM RCPT_COUNT,
                //					   (case when rm.INS_NUM>0 then round(rm.pass_num/rm.INS_NUM*100,2) else 0 end ) as PASS_RATE
                //					  -- a.BADPROBLEM_NAME BADPROBLEM_NAME,
                //					  -- round(a.BAD_QTY/a.TATOL_NUM*100,2) BAD_ITEM_RATE,
                //					  -- b.BADPROBLEM_NAME  BADPROBLEM_NAME1,
                //					  -- round(b.BAD_QTY/a.TATOL_NUM*100,2)   BAD_ITEM_RATE1,
                //					  -- c.BADPROBLEM_NAME  BADPROBLEM_NAME2,
                //					  -- round(c.BAD_QTY/a.TATOL_NUM*100,2) BAD_ITEM_RATE2,
                //					  -- a.TATOL_NUM
                //				  from tatol_data rm
                //				  --left join tatol_bad_data a
                //					--on rm.SUPPLIERS_NAME = a.SUPPLIERS_NAME
                //					--and a.index_rn = 1
                //				  --left join tatol_bad_data b
                //					--on rm.SUPPLIERS_NAME = b.SUPPLIERS_NAME
                //					--and b.index_rn = 2
                //				 -- left join tatol_bad_data c
                //					--on rm.SUPPLIERS_NAME = c.SUPPLIERS_NAME 
                //					--and c.index_rn = 3
                //				 where rm.SUPPLIERS_NAME is not null
                //				order by PASS_RATE desc";
                #endregion
				//New Query

                string sql = $@"with tmp as
 (select b1.SUPPLIERS_NAME,
         MAX(a1.RCPT_QTY) as INS_QTY,
         MAX(s.PASS_QTY) as PASS_QTY,
         MAX(rt.BAD_QTY) as BAD_QTY
    from wms_rcpt_m a
    LEFT JOIN wms_rcpt_d a1
      on a1.CHK_NO = a.CHK_NO
   INNER JOIN bdm_purchase_order_m a2
      on a2.ORDER_NO = a1.SOURCE_NO
    LEFT JOIN base003m b1
      ON a2.VEND_NO = b1.SUPPLIERS_CODE
    LEFT JOIN qcm_iqc_insp_res_m s
      on a.CHK_NO = s.CHK_NO
     and a1.CHK_SEQ = s.CHK_SEQ
     and s.isdelete = '0'
    LEFT JOIN qcm_iqc_insp_res_bad_report rt
      on a.CHK_NO = rt.CHK_NO
     and a1.CHK_SEQ = rt.CHK_SEQ
     AND rt.isdelete = '0'
   WHERE a.RCPT_BY = '01'
     and a.RCPT_DATE BETWEEN TO_date('{startTime}', 'yyyy-MM-dd') and
         TO_date('{endTime}', 'yyyy-MM-dd') 
     and A.ORG_ID = '5001'
     and a.STOC_NO = '1000'
     and b1.suppliers_name like '%{frim}%'
     and a1.ITEM_NO like '%{itemNo}%' 
   GROUP BY a1.CHK_NO, a1.CHK_SEQ, b1.SUPPLIERS_NAME
   order by MAX(a.INSERT_DATE) desc, MAX(a1.ITEM_NO) desc),
   tmp2 as(
select tmp.SUPPLIERS_NAME,
       sum(tmp.INS_QTY) as received_qty,
       NVL(sum(tmp.PASS_QTY),0) as pass_qty,
       NVL(sum(tmp.BAD_QTY),0) as BAD_QTY 
       from tmp group by tmp.SUPPLIERS_NAME) 
       select 
       tmp2.SUPPLIERS_NAME,
       tmp2.received_qty as INSP_COUNT,
       tmp2.pass_qty as RCPT_COUNT,
        case when tmp2.pass_qty > 0 then round(tmp2.pass_qty/(tmp2.pass_qty+tmp2.bad_qty)*100,2) else 0 end as pass_rate from tmp2";

				DataTable CSDT = DB.GetDataTable(sql);

				 
				
				//List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
				//List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };
				List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
				List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

				RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CSDT, TopList, 20, IsExport);//厂商合格率前5

				if (!IsExport)
				{
					RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CSDT, LastList, 20);//厂商合格率后5
				}

				#endregion

				ret.RetData1 = RetDic;
				ret.IsSuccess = true;
			}
			catch (Exception ex)
			{
				ret.IsSuccess = false;
				ret.ErrMsg = ex.Message;
			}
			return ret;
		}

		#endregion
		/// <summary>
		/// 外观检验--厂区数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableTwoData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                #region 查询条件
                string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂商类型	厂区分类	
                string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
                string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
                string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
                string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
                string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

				bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

                string[] orderListfirmAC = firmType.Split(',');		//厂区分类
                string[] orderListfirmC = firmClass.Split(',');		//厂商分类
                string[] orderListfrim = frim.Split(',');			//厂商
                string[] orderListfrimArea = frimArea.Split(',');   //厂区
                string[] orderListwork = workshop.Split(',');		//工段
                string[] orderListparts = parts.Split(',');			//部件名称
                string[] orderListart = art.Split(',');				//ART
                string[] orderListpo = po.Split(',');               //PO
                string[] orderListItemno = itemNo.Split(',');		//料号
                #endregion

                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    throw new Exception("Start time and end time are required fields!");
                }

                #region 拼接条件
                string HisWhere = string.Empty;//柱状图条件
                string FrimIQC = string.Empty;//厂商IQC
                string FrimRQC = string.Empty;//厂商RQC
                string PlantIQC = string.Empty;//厂区IQC
                string PlantRQC = string.Empty;//厂区RQC
                string ProductIQC = string.Empty;//产品IQC
                string ProductRQC = string.Empty;//产品RQC
                if (!string.IsNullOrEmpty(firmClass))//厂商类型
                {
                    FrimIQC += $@" AND b1.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
                    FrimRQC += $@" AND b1.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(frim))//厂商
                {
                    HisWhere += $@" AND SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
                    FrimIQC += $@" AND c.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
                    FrimRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";

                }
                if (!string.IsNullOrEmpty(art))//ART
                {
                    PlantIQC += $@" AND bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    PlantRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    ProductRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    FrimRQC += $@" AND A.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    HisWhere += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(po))//PO
                {
                    HisWhere += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                    FrimRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                    PlantRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                    ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(workshop))//工段
                {
                    PlantRQC += $@" AND se.WORKSHOP_SECTION_NAME in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(itemNo)) 
                {
                    FrimIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
                    PlantIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
                    ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
                }
				if (!string.IsNullOrEmpty(firmType))//厂区分类
				{
					PlantIQC += $@" AND c.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
                    PlantRQC += $@" AND c.ORG_NAME IN ({string.Join(',', orderListfirmAC.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(frimArea))//厂区
                {
                    PlantIQC += $@" AND so.EN IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
                    PlantRQC += $@" AND so.EN IN ({string.Join(',', orderListfrimArea.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(parts))//部件
                {
                    PlantIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
                }

                #endregion


                #region 厂区
                string CQSQL = $@"WITH rcpt_insp_data AS (
							SELECT
								b.ORG_CODE,
								b.ORG_NAME,
								a.CHK_NO,
								a.CHK_SEQ,
								a.ITEM_NO 
							FROM
								wms_rcpt_d a
								LEFT JOIN base001m b ON a.ORG_ID = b.ORG_CODE
								JOIN qcm_iqc_insp_res_m c ON a.CHK_NO = c.CHK_NO 
								AND a.CHK_SEQ = c.CHK_SEQ 
								AND a.ITEM_NO = c.ITEM_NO
							),
							rcpt_data AS (
							SELECT
								a.ORG_CODE,
								a.ORG_NAME,
								count( DISTINCT b.chk_no ) rcpt_count 
							FROM
								rcpt_insp_data a
								JOIN wms_rcpt_m b ON a.CHK_NO = b.CHK_NO 
							WHERE
								b.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) 
								AND to_date( '{endTime}', 'yyyy-mm-dd' ) --写条件
		
							GROUP BY
								a.ORG_CODE,
								a.ORG_NAME 
							),
							iqc_data AS (
							SELECT
								c.ORG_NAME,
								sum( a.pass_qty ) pass_qty,
								sum( nvl( b.BAD_QTY, 0 ) ) bad_qty,
								count( DISTINCT a.CHK_NO ) insp_count,
								MAX( so.EN ) cq 
							FROM
								qcm_iqc_insp_res_m a
								LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
								AND a.CHK_SEQ = b.CHK_SEQ 
								AND a.ITEM_NO = b.ITEM_NO
								JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
								AND a.CHK_SEQ = c.CHK_SEQ 
								AND a.ITEM_NO = c.ITEM_NO --left join rcpt_data d on c.org_name=d.org_name and c.suppliers_name=d.SUPPLIERS_NAME
								JOIN BASE005M b5 ON b5.FACTORY_SAP = c.ORG_CODE 
								JOIN SJQDMS_ORGINFO so ON b5.UDF05 = so.CODE 
								LEFT JOIN WMS_RCPT_D wd ON wd.SOURCE_NO = a.CHK_NO AND wd.SOURCE_SEQ = a.CHK_SEQ
								left JOIN BDM_PURCHASE_ORDER_ITEM bi ON bi.ORDER_NO = wd.SOURCE_NO AND bi.ORDER_SEQ = wd.SOURCE_SEQ
								LEFT JOIN BDM_PURCHASE_ORDER_D bd ON bi.ORDER_NO = bd.ORDER_NO AND bi.ORDER_SEQ = bd.ORDER_SEQ
								LEFT JOIN wms_rcpt_m a2 ON a2.CHK_NO = wd.CHK_NO AND wd.SOURCE_NO = a2.SOURCE_NO
								JOIN base003m b1 ON a2.vend_no = b1.SUPPLIERS_CODE
							WHERE
								a.INSPECTIONDATE BETWEEN '{startTime}' 
								AND '{endTime}' --写条件
								{PlantIQC}
							GROUP BY
								c.ORG_NAME --,d.rcpt_count
		
							),
							rqc_data AS (
							SELECT
								c.ORG_NAME,
							sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
							sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty,
							count( d.id ) insp_count,
							MAX( so.EN ) plantArea 
						FROM
							rqc_task_m a
							JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
                            JOIN SJQDMS_ORGINFO so ON b.UDF05 = so.CODE 
							JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE
							JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO 
							JOIN bdm_workshop_section_m se ON a.workshop_section_no = se.workshop_section_no
							LEFT JOIN base003m b1 ON b1.SUPPLIERS_CODE = a.SUPPLIERS_CODE
						WHERE
							d.createdate BETWEEN '{startTime}' 
							AND '{endTime}' --写条件
							{PlantRQC}
						GROUP BY
							c.ORG_NAME 
							),
							total_data_tmp AS ( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ),
							total_data_tmp2 AS (--厂区基础数据
							SELECT
								org_name,
								round( ( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100 ), 1 ) pass_rate,
								sum( insp_count ) insp_count,
								MAX( cq ) cq 
							FROM
								total_data_tmp 
							GROUP BY
								org_name 
							),
							total_data AS ( SELECT org_name, pass_rate, insp_count, cq, row_number ( ) over ( ORDER BY pass_rate ASC ) total_ranking FROM total_data_tmp2 ),
							iqc_bad_data AS (
							SELECT
								c.org_name,
								b.BADPROBLEM_NAME,
								count( a.id ) bad_qty 
							FROM
								qcm_iqc_insp_res_d a
								JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE
								JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
								AND a.CHK_SEQ = c.CHK_SEQ 
								AND a.ITEM_NO = c.ITEM_NO 
							WHERE
								a.determine = 1 
								AND a.createdate BETWEEN '{startTime}' 
								AND '{endTime}' 
							GROUP BY
								c.org_name,
								b.BADPROBLEM_NAME 
							),
							rqc_bad_data AS (
							SELECT
								f.ORG_NAME,
								c.INSPECTION_NAME BADPROBLEM_NAME,
								count( c.id ) bad_qty 
							FROM
								rqc_task_detail_t a
								JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
								AND a.COMMIT_INDEX = b.COMMIT_INDEX
								JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
								AND b.UNION_ID = c.ID
								JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO
								JOIN base005m e ON d.PRODUCTION_LINE_CODE = e.DEPARTMENT_CODE
								JOIN base001m f ON e.FACTORY_SAP = f.ORG_CODE 
							WHERE
								a.commit_type = 1 
								AND a.CREATEDATE BETWEEN '{startTime}' 
								AND '{endTime}' 
							GROUP BY
								f.ORG_NAME,
								c.INSPECTION_NAME 
							),
							total_bad_data_tmp AS ( --厂区不合格项目数量
							SELECT * FROM iqc_bad_data UNION ALL SELECT * FROM rqc_bad_data ),
							total_bad_data_tmp2 AS (
							SELECT
								ORG_NAME,
								BADPROBLEM_NAME,
								sum( bad_qty ) bad_qty,
								ROW_NUMBER ( ) over ( partition BY ORG_NAME ORDER BY sum( bad_qty ) DESC ) ranking 
							FROM
								total_bad_data_tmp 
							GROUP BY
								ORG_NAME,
								BADPROBLEM_NAME 
							),
							total_bad_data AS ( SELECT ORG_NAME, BADPROBLEM_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY org_name ) totla_bad_qty FROM total_bad_data_tmp2 )
						SELECT
							a.ORG_NAME className,
							a.cq ORG_NAME,
							a.pass_rate,
							a.total_ranking,
							a.insp_count,
							e.rcpt_count,
							b.BADPROBLEM_NAME,
							round( b.bad_qty / b.totla_bad_qty * 100, 1 ) bad_item_rate_1,
							b.ranking ranking,
							c.BADPROBLEM_NAME,
							round( c.bad_qty / c.totla_bad_qty * 100, 1 ) bad_item_rate_2,
							c.ranking ranking,
							d.BADPROBLEM_NAME,
							round( d.bad_qty / d.totla_bad_qty * 100, 1 ) bad_item_rate_3,
							d.ranking ranking 
						FROM
							total_data a
							JOIN total_bad_data b ON a.org_name = b.org_name --left join is changed to join
							AND b.ranking = 1
							JOIN total_bad_data c ON a.org_name = c.org_name --left join is changed to join
							AND c.ranking = 2
							JOIN total_bad_data d ON a.org_name = d.org_name --left join is changed to join
							AND d.ranking = 3
							JOIN rcpt_data e ON a.org_name = e.org_name --left join is changed to join
						ORDER BY
							a.TOTAL_RANKING";
                DataTable CQDT = DB.GetDataTable(CQSQL);



				List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
				List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

				RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CQDT, TopList, 20, IsExport);//厂商合格率前5
                if (!IsExport)
                {
					RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CQDT, LastList, 20);//厂商合格率后5
				}
				
				#endregion


				ret.RetData1 = RetDic;
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
        /// 外观检验--产品数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// 
        #region APC Code of GetdAllTableThreeData
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableThreeData(object OBJ)
        //      {
        //          SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //          SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //          SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //          try
        //          {
        //              DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //              string Data = ReqObj.Data.ToString();
        //              var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //              Dictionary<string, object> RetDic = new Dictionary<string, object>();

        //              #region 查询条件
        //              string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
        //              string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
        //              string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
        //              string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
        //              string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
        //              string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
        //              string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
        //              string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
        //              string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
        //              string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
        //              string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

        //		bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

        //              string[] orderListfirmAC = firmType.Split(',');		//厂区分类
        //              string[] orderListfirmC = firmClass.Split(',');		//厂商分类
        //              string[] orderListfrim = frim.Split(',');			//厂商
        //              string[] orderListfrimArea = frimArea.Split(',');   //厂区
        //              string[] orderListwork = workshop.Split(',');		//工段
        //              string[] orderListparts = parts.Split(',');			//部件名称
        //              string[] orderListart = art.Split(',');				//ART
        //              string[] orderListpo = po.Split(',');               //PO
        //              string[] orderListItemno = itemNo.Split(',');       //料号
        //              #endregion

        //              if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
        //              {
        //                  throw new Exception("Start time and end time are required fields!");
        //              }

        //              #region 拼接条件

        //              string ProductIQC = string.Empty;//产品IQC
        //              string ProductRQC = string.Empty;//产品RQC
        //              if (!string.IsNullOrEmpty(firmClass))//厂商分类
        //              {
        //                  ProductRQC += $@" AND b1.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
        //                  ProductIQC += $@" and bm3.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(frim))//厂商
        //              {
        //                  ProductRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
        //                  ProductIQC += $@" and bm3.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(art))//ART
        //              {
        //                  ProductRQC += $@" AND a.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
        //                  ProductIQC += $@" and bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(po))//PO
        //              {
        //                  ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(workshop))//工段
        //              {
        //                  ProductRQC += $@" AND se.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(itemNo))//料号
        //              {
        //                  ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
        //              }
        //              if (!string.IsNullOrEmpty(parts))//部件
        //              {
        //                  ProductIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
        //              }
        //              #endregion

        //              #region 产品
        //              string CPSQL = $@"WITH rcpt_insp_data AS (
        //						select a.CHK_NO, a.CHK_SEQ,a.ITEM_NO,a.ITEM_NO ART_NO ,d.NAME_T 
        //						from wms_rcpt_d a
        //						JOIN qcm_iqc_insp_res_m b ON a.CHK_NO = b.CHK_NO 
        //						AND a.CHK_SEQ = b.CHK_SEQ 
        //						AND a.ITEM_NO = b.ITEM_NO
        //						JOIN bdm_rd_item d ON a.ITEM_NO = d.ITEM_NO 
        //					),
        //					rcpt_data AS (
        //					SELECT
        //						a.ART_NO,
        //						a.NAME_T,
        //						count( DISTINCT b.chk_no ) rcpt_count 
        //					FROM
        //						rcpt_insp_data a
        //						JOIN wms_rcpt_m b ON a.CHK_NO = b.CHK_NO 
        //					WHERE
        //						b.rcpt_date BETWEEN to_date( '{startTime}', 'yyyy-mm-dd' ) 
        //						AND to_date( '{endTime}', 'yyyy-mm-dd' ) --写条件
        //					GROUP BY
        //						a.ART_NO,
        //						a.NAME_T 
        //					),
        //					iqc_data AS (
        //					SELECT
        //						c.ART_NO,
        //						c.NAME_T,
        //						sum( a.pass_qty ) pass_qty,
        //						sum( nvl( b.BAD_QTY, 0 ) ) bad_qty,
        //						count( DISTINCT a.CHK_NO ) insp_count 
        //					FROM
        //						qcm_iqc_insp_res_m a
        //						LEFT JOIN qcm_iqc_insp_res_bad_report b ON a.CHK_NO = b.CHK_NO 
        //						AND a.CHK_SEQ = b.CHK_SEQ 
        //						AND a.ITEM_NO = b.ITEM_NO
        //						JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
        //						AND a.CHK_SEQ = c.CHK_SEQ 
        //						AND a.ITEM_NO = c.ITEM_NO 
        //						LEFT JOIN BDM_RD_ITEM bm ON bm.ITEM_NO = a.item_no
        //						JOIN BASE003M bm3 ON bm3.SUPPLIERS_CODE = bm.VEND_NO_PRD
        //						LEFT JOIN WMS_RCPT_D wd ON wd.SOURCE_NO = a.CHK_NO AND wd.SOURCE_SEQ = a.CHK_SEQ
        //						left JOIN BDM_PURCHASE_ORDER_ITEM bi ON bi.ORDER_NO = wd.SOURCE_NO AND bi.ORDER_SEQ = wd.SOURCE_SEQ
        //						LEFT JOIN BDM_PURCHASE_ORDER_D bd ON bi.ORDER_NO = bd.ORDER_NO AND bi.ORDER_SEQ = bd.ORDER_SEQ
        //					WHERE
        //						a.INSPECTIONDATE BETWEEN '{startTime}' 
        //						AND '{endTime}' --写条件
        //						{ProductIQC}
        //					GROUP BY
        //						c.ART_NO,
        //						c.NAME_T 
        //					),
        //					rqc_data AS (
        //					SELECT
        //						a.PROD_NO,
        //						b.NAME_T,
        //					sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
        //					sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty,
        //					count( d.id ) insp_count 
        //				FROM
        //					rqc_task_m a
        //					JOIN bdm_rd_prod b ON a.PROD_NO = b.PROD_NO
        //					JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
        //					JOIN bdm_workshop_section_m se ON a.workshop_section_no = se.workshop_section_no
        //					JOIN base003m b1 ON b1.SUPPLIERS_CODE = a.SUPPLIERS_CODE
        //				WHERE
        //					d.createdate BETWEEN '{startTime}' 
        //					AND '{endTime}' --写条件 
        //					{ProductRQC}
        //				GROUP BY
        //					a.PROD_NO,
        //					b.NAME_T 
        //					),
        //					total_data_tmp AS (
        //					SELECT
        //						ART_NO,
        //						NAME_T,
        //						INSP_COUNT,
        //						sum( pass_qty ) pass_qty,
        //						sum( bad_qty ) bad_qty,
        //						round(
        //							CASE
        //									WHEN ( sum( a.pass_qty ) + sum( nvl( a.BAD_QTY, 0 ) ) ) > 0 THEN
        //									sum( a.pass_qty ) / ( sum( a.pass_qty ) + sum( nvl( a.BAD_QTY, 0 ) ) ) * 100 ELSE 0 
        //								END,
        //								1 
        //							) pass_rate,
        //							row_number ( ) over (

        //							ORDER BY
        //							CASE
        //									WHEN ( sum( a.pass_qty ) + sum( nvl( a.BAD_QTY, 0 ) ) ) > 0 THEN
        //									sum( a.pass_qty ) / ( sum( a.pass_qty ) + sum( nvl( a.BAD_QTY, 0 ) ) ) ELSE 0 
        //							END DESC 
        //							) ranking  
        //					FROM
        //						( SELECT * FROM iqc_data UNION ALL SELECT * FROM rqc_data ) a 
        //					GROUP BY
        //						ART_NO,
        //						NAME_T,
        //						INSP_COUNT 
        //					),
        //					total_data AS (
        //                                         SELECT * FROM total_data_tmp 
        //					),
        //					iqc_bad_data AS (
        //					SELECT
        //						c.ART_NO,
        //						c.NAME_T,
        //						b.BADPROBLEM_NAME,
        //						count( a.id ) bad_qty 
        //					FROM
        //						qcm_iqc_insp_res_d a
        //						JOIN qcm_iqc_badproblems_m b ON a.BADPROBLEM_CODE = b.BADPROBLEM_CODE
        //						JOIN rcpt_insp_data c ON a.CHK_NO = c.CHK_NO 
        //						AND a.CHK_SEQ = c.CHK_SEQ 
        //						AND a.ITEM_NO = c.ITEM_NO 
        //					WHERE
        //						a.determine = 1 
        //						AND a.createdate BETWEEN '{startTime}' 
        //						AND '{endTime}' 
        //					GROUP BY
        //						c.ART_NO,
        //						c.NAME_T,
        //						b.BADPROBLEM_NAME 
        //					),
        //					rqc_bad_data AS (
        //					SELECT
        //						g.PROD_NO,
        //						g.NAME_T,
        //						c.INSPECTION_NAME BADPROBLEM_NAME,
        //						count( c.id ) bad_qty 
        //					FROM
        //						rqc_task_detail_t a
        //						JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
        //						AND a.COMMIT_INDEX = b.COMMIT_INDEX
        //						JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
        //						AND b.UNION_ID = c.ID
        //						JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO
        //						JOIN bdm_se_order_master e ON d.MER_PO = e.MER_PO
        //						JOIN BDM_SE_ORDER_ITEM f ON e.se_id = f.se_id
        //						JOIN bdm_rd_prod g ON f.PROD_NO = g.PROD_NO 
        //					WHERE
        //						a.commit_type = 1 
        //						AND a.CREATEDATE BETWEEN '{startTime}' 
        //						AND '{endTime}' 
        //					GROUP BY
        //						g.PROD_NO,
        //						g.NAME_T,
        //						c.INSPECTION_NAME 
        //					),
        //					total_bad_data_tmp AS (
        //					SELECT
        //						art_no,
        //						name_t,
        //						BADPROBLEM_NAME,
        //						sum( bad_qty ) bad_qty,
        //						ROW_NUMBER ( ) over ( partition BY art_no ORDER BY sum( bad_qty ) DESC ) ranking 
        //					FROM
        //						( SELECT * FROM iqc_bad_data UNION ALL SELECT * FROM rqc_bad_data ) a 
        //					GROUP BY
        //						art_no,
        //						name_t,
        //						BADPROBLEM_NAME 
        //					),
        //					total_bad_data AS ( SELECT art_no, name_t, BADPROBLEM_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY art_no ) totla_bad_qty FROM total_bad_data_tmp ) SELECT
        //					a.* ,
        //					e.rcpt_count,
        //					b.BADPROBLEM_NAME,
        //					round( CASE WHEN b.totla_bad_qty > 0 THEN b.bad_qty / b.totla_bad_qty * 100 ELSE 0 END, 1 ) bad_item_rate,
        //					b.ranking ranking_1,
        //					c.BADPROBLEM_NAME,
        //					round( CASE WHEN c.totla_bad_qty > 0 THEN c.bad_qty / c.totla_bad_qty * 100 ELSE 0 END, 1 ) bad_item_rate,
        //					c.ranking ranking_2,
        //					d.BADPROBLEM_NAME,
        //					round( CASE WHEN d.totla_bad_qty > 0 THEN d.bad_qty / d.totla_bad_qty * 100 ELSE 0 END, 1 ) bad_item_rate,
        //					d.ranking ranking_3 
        //				FROM
        //					total_data a
        //					JOIN total_bad_data b ON a.art_no = b.art_no --left join is changed to join
        //					AND b.ranking = 1
        //					JOIN total_bad_data c ON a.art_no = c.art_no --left join is changed to join
        //					AND c.ranking = 2
        //					JOIN total_bad_data d ON a.art_no = d.art_no --left join is changed to join
        //					AND d.ranking = 3
        //					JOIN rcpt_data e ON a.art_no = e.art_no";   //--left join is changed to join
        //		DataTable CPDT = DB.GetDataTable(CPSQL);



        //		List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
        //		List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

        //		RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CPDT, TopList, 20, IsExport);//厂商合格率前5
        //		if (!IsExport)
        //			RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CPDT, LastList, 20);//厂商合格率后5
        //		#endregion

        //		ret.RetData1 = RetDic;
        //              ret.IsSuccess = true;
        //          }
        //          catch (Exception ex)
        //          {
        //              ret.IsSuccess = false;
        //              ret.ErrMsg = ex.Message;
        //          }
        //          return ret;
        //      }
        #endregion

        #region Code Written By Ashok
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetdAllTableThreeData(object OBJ)
		{
			SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
			SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
			SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
			try
			{
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
				string Data = ReqObj.Data.ToString();
				var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
				Dictionary<string, object> RetDic = new Dictionary<string, object>();

				#region 查询条件
				string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
				string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
				string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
				string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

				bool IsExport = jarr.ContainsKey("IsExport") ? bool.Parse(jarr["IsExport"].ToString()) : false;//是否导出

				string[] orderListfirmAC = firmType.Split(',');     //厂区分类
				string[] orderListfirmC = firmClass.Split(',');     //厂商分类
				string[] orderListfrim = frim.Split(',');           //厂商
				string[] orderListfrimArea = frimArea.Split(',');   //厂区
				string[] orderListwork = workshop.Split(',');       //工段
				string[] orderListparts = parts.Split(',');         //部件名称
				string[] orderListart = art.Split(',');             //ART
				string[] orderListpo = po.Split(',');               //PO
				string[] orderListItemno = itemNo.Split(',');       //料号
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("Start time and end time are required fields!");
				}

				#region 拼接条件

				string ProductIQC = string.Empty;//产品IQC
				string ProductRQC = string.Empty;//产品RQC
				if (!string.IsNullOrEmpty(firmClass))//厂商分类
				{
					ProductRQC += $@" AND b1.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
					ProductIQC += $@" and bm3.M_TYPE in ({string.Join(',', orderListfirmC.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frim))//厂商
				{
					ProductRQC += $@" AND a.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
					ProductIQC += $@" and bm3.SUPPLIERS_NAME in ({string.Join(',', orderListfrim.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))//ART
				{
					ProductRQC += $@" AND a.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
					ProductIQC += $@" and bd.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))//PO
				{
					ProductRQC += $@" AND A.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(workshop))//工段
				{
					ProductRQC += $@" AND se.workshop_section_name in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(itemNo))//料号
				{
					ProductIQC += $@" AND a.ITEM_NO in ({string.Join(',', orderListItemno.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(parts))//部件
				{
					ProductIQC += $@" AND bi.PART_NO in ({string.Join(',', orderListparts.Select(x => $"'{x}'"))})";
				}
				#endregion

				#region 产品
				string CPSQL = $@"select 
a.ITEM_NO as ART_NO,
a.NAME_T as NAME_T,
sum(a.RCPT_QTY) as INSP_COUNT,
 sum(a.pass_qty) as RCPT_COUNT,
 --sum(a.bad_qty) as bad_qty,
 case when  sum(a.pass_qty) > 0 then round( sum(a.pass_qty)/( sum(a.pass_qty)+sum(a.bad_qty))*100,2) else 0 end as PASS_RATE from (
SELECT 
  MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
  MAX(aw.NAME_T) as NAME_T, --材料名称  
  NVL(MAX(a1.RCPT_QTY),0) AS RCPT_QTY, --收料数量 
  NVL(MAX(s.PASS_QTY),0) as PASS_QTY, --合格数
  NVL(MAX(rt.BAD_QTY),0) as BAD_QTY--(不良数量)不合格数量   --鞋型  
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO 
INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT JOIN base003m b1 ON a2.VEND_NO = b1.SUPPLIERS_CODE
LEFT  JOIN bdm_purchase_order_d z1 on a2.ORDER_NO=z1.ORDER_NO and a1.SOURCE_SEQ=z1.ORDER_SEQ  
LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO 
LEFT JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO 
LEFT JOIN qcm_iqc_insp_res_m s on a.CHK_NO=s.CHK_NO and a1.CHK_SEQ=s.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on a.CHK_NO=rt.CHK_NO and a1.CHK_SEQ=rt.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON a1.SOURCE_NO=c111.ORDER_NO AND a1.CHK_SEQ=c111.ORDER_SEQ  
WHERE  a.RCPT_BY='01'  and a.RCPT_DATE BETWEEN TO_date('{startTime}','yyyy-MM-dd') and TO_date('{endTime}','yyyy-MM-dd')and 
A.ORG_ID ='5001' and a.STOC_NO = '1000' and a1.ITEM_NO like '%{itemNo}%' and b1.suppliers_name like '%{frim}%'
GROUP BY a1.CHK_NO,a1.CHK_SEQ 
order by MAX(a.INSERT_DATE) desc,MAX(a1.ITEM_NO) desc

) a group by a.ITEM_NO,a.NAME_T";  
				DataTable CPDT = DB.GetDataTable(CPSQL);



				List<string> TopList = new List<string>() { "pass_rate desc", "RCPT_COUNT desc" };
				List<string> LastList = new List<string>() { "pass_rate asc", "RCPT_COUNT asc" };

				RetDic["FrimPassRateTop5"] = Common.ReturnDTOrderByRows(CPDT, TopList, 20, IsExport);//厂商合格率前5
				if (!IsExport)
					RetDic["FrimPassRateLast5"] = Common.ReturnDTOrderByRows(CPDT, LastList, 20);//厂商合格率后5
				#endregion

				ret.RetData1 = RetDic;
				ret.IsSuccess = true;
			}
			catch (Exception ex)
			{
				ret.IsSuccess = false;
				ret.ErrMsg = ex.Message;
			}
			return ret;
		}

		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllHisCakeThreeData(object OBJ)
		{
			SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
			SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
			SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
			try
			{
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
				string Data = ReqObj.Data.ToString();
				var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
				Dictionary<string, object> RetDic = new Dictionary<string, object>();

				#region 查询条件
				string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//
				string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
				string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
				string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
				string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion
				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("开始时间，结束时间为必填字段！");
				}

				#region 拼接条件
				string IWSQL = string.Empty;
				string RWSQL = string.Empty;
				if (!string.IsNullOrEmpty(firmType))//厂区类型
				{
					var arrfrimtype = string.Join(',', firmType.Split(',').Select(x => $"'{x}'"));

					//DataTable SupplierlistDt = DB.GetDataTable(HistogramDataSql);
					//List<string> stringList = new List<string>();
					//foreach (DataRow row in SupplierlistDt.Rows)
					//{
					//	string value = row["Suppliers_Name"].ToString();
					//	stringList.Add(value);
					//}
					//RWSQL += $@" AND FACTORYDATA.ORG_NAME in({arrfrimtype})";
					string[] firmTypeList = firmType.Split(',');
					string firmTypevalues = $@"select ORG_CODE from BASE001M WHERE ORG_NAME in ({string.Join(',', firmTypeList.Select(x => $"'{x}'"))})";
					DataTable firmTypevalueslistDt = DB.GetDataTable(firmTypevalues);
					List<string> firmTypevaluesList = new List<string>();
					foreach (DataRow row in firmTypevalueslistDt.Rows)
					{
						string value = row["ORG_CODE"].ToString();
						firmTypevaluesList.Add(value);
					}
					RWSQL += $@" AND A.ORG_ID in ({string.Join(',', firmTypevaluesList.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(firmClass))//厂商类型
				{
					string[] arrfirmclass = firmClass.Split(',');
					RWSQL += $@" AND B.M_TYPE in ({string.Join(',', arrfirmclass.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frim))//厂商
				{
					string[] orderListf = frim.Split(',');
					RWSQL += $@" AND b.SUPPLIERS_NAME in ({string.Join(',', orderListf.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frimArea))//厂区 
				{

					string[] orderListfa = frimArea.Split(',');
					string frimAreavalues = $@"select COMPANY from SJQDMS_ORGINFO WHERE EN in ({string.Join(',', orderListfa.Select(x => $"'{x}'"))})";
					DataTable frimAreavalueslistDt = DB.GetDataTable(frimAreavalues);
					List<string> frimAreavaluesList = new List<string>();
					foreach (DataRow row in frimAreavalueslistDt.Rows)
					{
						string value = row["COMPANY"].ToString();
						frimAreavaluesList.Add(value);
					}
					RWSQL += $@" AND A.ORG_ID in ({string.Join(',', frimAreavaluesList.Select(x => $"'{x}'"))})";

				}
				if (!string.IsNullOrEmpty(workshop))//工段
				{
					string[] orderListwork = workshop.Split(',');
					RWSQL += $@" AND QM.WORKSHOP_SECTION_NAME in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(parts))
				{

				}
				if (!string.IsNullOrEmpty(art))
				{
					string[] orderListart = art.Split(',');
					RWSQL += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					string[] orderListpo = po.Split(',');
					RWSQL += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(itemNo))
				{

				}
				#endregion


				RetDic["Histogram1"] = null;//柱图1
											//RetDic["cake"] = null;//饼图
				#region x轴-月份
				string startYear = DateTime.Now.ToString("yyyy-01-01");
				string endYear = Convert.ToDateTime(DateTime.Now.AddYears(1).ToString("yyyy-01-01")).AddDays(-1).ToString("yyyy-MM-dd");
				List<string> MonthList = ReturnData("m", startYear, endYear);
				#endregion

				//多语言翻译
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "進行了多少次測試", "經過", "失敗" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{

					name1 = LanDic["進行了多少次測試"].ToString();
					name2 = LanDic["經過"].ToString();
					name3 = LanDic["失敗"].ToString();


				}

				//string Supplierlist = $@"select distinct suppliers_name AS Suppliers_Name from wms_rcpt_m a 
				//        INNER JOIN base003m b ON a.VEND_NO = b.SUPPLIERS_CODE WHERE a.org_id = '5001' and b.suppliers_name is not null";
				//				string Supplierlist = $@"SELECT  b.suppliers_name AS Suppliers_Name
				//FROM wms_rcpt_m a
				//INNER JOIN base003m b ON a.VEND_NO = b.SUPPLIERS_CODE
				//INNER JOIN wms_rcpt_d c ON c.source_no = a.source_no
				//INNER JOIN qcm_iqc_insp_res_m d ON c.item_no = d.item_no
				//WHERE a.org_id = '5001' AND b.suppliers_name IS NOT NULL
				//GROUP BY b.suppliers_name
				//HAVING SUM(d.sample_qty) <> 0 OR SUM(CASE WHEN d.determine IN ('0', '1') THEN d.sample_qty ELSE 0 END) <> 0";
				//				DataTable SupplierlistDt = DB.GetDataTable(Supplierlist);
				//				List<string> stringList = new List<string>();
				//				foreach (DataRow row in SupplierlistDt.Rows)
				//				{
				//					string value = row["Suppliers_Name"].ToString();qcm_iqc_insp_res_bad_report
				//					stringList.Add(value);
				//				}
				List<string> DayList = Common.ReturnData("d", startTime, endTime);
				string HistogramDataSql = $@"SELECT b.suppliers_name AS Suppliers_Name,
  TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM')RCPT_DATE,
    --MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME,
    SUM(a1.RCPT_QTY) AS TAKEN_QTY,
    SUM(s.PASS_QTY) AS PASS_QTY,
    SUM(rt.BAD_QTY) AS BAD_QTY 
FROM
    wms_rcpt_m a
    LEFT JOIN wms_rcpt_d a1 ON a1.CHK_NO = a.CHK_NO
-- LEFT JOIN SJQDMS_ORGINFO sj ON a.org_id = sj.COMPANY
--LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
    LEFT JOIN base003m b ON a.vend_no = b.SUPPLIERS_CODE
    LEFT JOIN qcm_iqc_insp_res_m s ON a.CHK_NO = s.CHK_NO 
        AND a1.CHK_SEQ = s.CHK_SEQ 
        AND s.isdelete = '0' 
    LEFT JOIN qcm_iqc_insp_res_bad_report rt ON a.CHK_NO = rt.CHK_NO 
        AND a1.CHK_SEQ = rt.CHK_SEQ 
        AND rt.isdelete = '0'
LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
WHERE  
    a.RCPT_BY = '01'  
    AND a.RCPT_DATE BETWEEN TO_date('{startTime}', 'yyyy-MM-dd') 
                        AND TO_date('{endTime}', 'yyyy-MM-dd') 
   --AND A.ORG_ID = '5001' 
{RWSQL}
    AND b.suppliers_name IS NOT NULL
GROUP BY 
    b.SUPPLIERS_NAME
  HAVING 
    SUM(a1.RCPT_QTY) > 0
ORDER BY 
    MAX(a.INSERT_DATE) DESC, 
    MAX(a1.ITEM_NO) DESC";
				//柱图1
				//				string HistogramDataSql = $@"select
				//  b.suppliers_name AS Suppliers_Name,TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM')RCPT_DATE,
				// --max(to_char(to_date(a.RCPT_DATE, 'yyyy-mm-dd'), 'yyyy-mm')) RCPT_DATE,
				//  --SUM(c.rcpt_qty) as Received ,SUM(c.iv_qty) completed,SUM(c.pass_qty) as Pass_no,
				//  --MAX(NVL(d.DETERMINE, 2)) as DETERMINE,
				//   sum(d.sample_qty) as total ,
				//     sum(case when d.determine = '0' then d.sample_qty else 0 end) as pass,
				//     sum(case when d.determine = '1' then d.sample_qty else 0 end) as fail
				//  from wms_rcpt_m a
				//INNER JOIN base003m b ON a.VEND_NO = b.SUPPLIERS_CODE
				//INNER JOIN wms_rcpt_d c ON c.source_no = a.source_no
				//INNER JOIN qcm_iqc_insp_res_m d ON c.item_no = d.item_no
				//WHERE
				// a.org_id = '5001' and
				// b.suppliers_name is not null and
				//a.RCPT_DATE BETWEEN TO_date('{startYear}','yyyy-MM-dd') and TO_date('{endYear}','yyyy-MM-dd')
				//group by b.suppliers_name
				//HAVING SUM(d.sample_qty) <> 0 OR SUM(CASE WHEN d.determine IN ('0', '1') THEN d.sample_qty ELSE 0 END) <> 0";
				DataTable SupplierlistDt = DB.GetDataTable(HistogramDataSql);
				List<string> stringList = new List<string>();
				foreach (DataRow row in SupplierlistDt.Rows)
				{
					string value = row["Suppliers_Name"].ToString();
					stringList.Add(value);
				}
				//RetDic["Histogram1"] = Common.RetunHisDatagraph(DB, stringList, HistogramDataSql, "Suppliers_Name",  "total", "pass", "fail", name1, name2, name3);//柱图1
				RetDic["Histogram1"] = Common.RetunHisDatagraph(DB, stringList, HistogramDataSql, "Suppliers_Name", "TAKEN_QTY", "PASS_QTY", "BAD_QTY", name1, name2, name3);//柱图1

				//                string CakeDataSQL = $@"SELECT b.suppliers_name AS Suppliers_Name,
				//  TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM')RCPT_DATE,
				//    --MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME,
				//    SUM(a1.RCPT_QTY) AS TAKEN_QTY,
				//    SUM(s.PASS_QTY) AS PASS_QTY,
				//    SUM(rt.BAD_QTY) AS BAD_QTY 
				//FROM
				//    wms_rcpt_m a
				//    LEFT JOIN wms_rcpt_d a1 ON a1.CHK_NO = a.CHK_NO
				//-- LEFT JOIN SJQDMS_ORGINFO sj ON a.org_id = sj.COMPANY
				//--LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
				//    LEFT JOIN base003m b ON a.vend_no = b.SUPPLIERS_CODE
				//    LEFT JOIN qcm_iqc_insp_res_m s ON a.CHK_NO = s.CHK_NO 
				//        AND a1.CHK_SEQ = s.CHK_SEQ 
				//        AND s.isdelete = '0' 
				//    LEFT JOIN qcm_iqc_insp_res_bad_report rt ON a.CHK_NO = rt.CHK_NO 
				//        AND a1.CHK_SEQ = rt.CHK_SEQ 
				//        AND rt.isdelete = '0'
				//LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
				//WHERE  
				//    a.RCPT_BY = '01'  
				//    AND a.RCPT_DATE BETWEEN TO_date('{startTime}', 'yyyy-MM-dd') 
				//                        AND TO_date('{endTime}', 'yyyy-MM-dd') 
				//   --AND A.ORG_ID = '5001' 
				//{RWSQL}
				//    AND b.suppliers_name IS NOT NULL
				//GROUP BY 
				//    b.SUPPLIERS_NAME
				//  HAVING 
				//    SUM(a1.RCPT_QTY) > 0
				//ORDER BY 
				//    MAX(a.INSERT_DATE) DESC, 
				//    MAX(a1.ITEM_NO) DESC";
				string CakeDataSQL = $@"SELECT Suppliers_Name, RCPT_DATE, TAKEN_QTY, PASS_QTY, BAD_QTY
FROM (
    SELECT 
        b.suppliers_name AS Suppliers_Name,
        TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM') AS RCPT_DATE,
        SUM(a1.RCPT_QTY) AS TAKEN_QTY,
        SUM(s.PASS_QTY) AS PASS_QTY,
        SUM(rt.BAD_QTY) AS BAD_QTY,
        ROW_NUMBER() OVER (ORDER BY SUM(rt.BAD_QTY) DESC) AS BadQtyRank
    FROM
        wms_rcpt_m a
        LEFT JOIN wms_rcpt_d a1 ON a1.CHK_NO = a.CHK_NO
        LEFT JOIN base003m b ON a.vend_no = b.SUPPLIERS_CODE
        LEFT JOIN qcm_iqc_insp_res_m s ON a.CHK_NO = s.CHK_NO 
            AND a1.CHK_SEQ = s.CHK_SEQ 
            AND s.isdelete = '0' 
        LEFT JOIN qcm_iqc_insp_res_bad_report rt ON a.CHK_NO = rt.CHK_NO 
            AND a1.CHK_SEQ = rt.CHK_SEQ 
            AND rt.isdelete = '0'
        LEFT JOIN RQC_TASK_M rm ON b.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
    WHERE  
        a.RCPT_BY = '01'  
        AND a.RCPT_DATE BETWEEN TO_date('{startTime}', 'yyyy-MM-dd') 
                        AND TO_date('{endTime}', 'yyyy-MM-dd')  
        AND b.suppliers_name IS NOT NULL {RWSQL}
    GROUP BY 
        b.SUPPLIERS_NAME
    HAVING 
        SUM(a1.RCPT_QTY) > 0 AND SUM(rt.BAD_QTY) > 0 -- Adding condition for non-zero BAD_QTY
)
WHERE BadQtyRank <= 5
ORDER BY 
    BadQtyRank";

				RetDic["cake"] = Common.ReturnCakeData(DB, CakeDataSQL, 0, "Suppliers_Name", "BAD_QTY", "");//饼图


				//			string CakeDataSQL = $@"select
				// b.suppliers_name AS Suppliers_Name,TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM')RCPT_DATE,
				// --max(to_char(to_date(a.RCPT_DATE, 'yyyy-mm-dd'), 'yyyy-mm')) RCPT_DATE,
				//  --SUM(c.rcpt_qty) as Received ,SUM(c.iv_qty) completed,SUM(c.pass_qty) as Pass_no,
				//  --MAX(NVL(d.DETERMINE, 2)) as DETERMINE,
				//   sum(d.sample_qty) as total ,
				//     sum(case when d.determine = '0' then d.sample_qty else 0 end) as pass,
				//     sum(case when d.determine = '1' then d.sample_qty else 0 end) as fail
				//  from wms_rcpt_m a
				//INNER JOIN base003m b ON a.VEND_NO = b.SUPPLIERS_CODE
				//INNER JOIN wms_rcpt_d c ON c.source_no = a.source_no
				//INNER JOIN qcm_iqc_insp_res_m d ON c.item_no = d.item_no
				//WHERE
				// a.org_id = '5001' and
				// b.suppliers_name is not null and
				//a.RCPT_DATE BETWEEN TO_date('{startYear}','yyyy-MM-dd') and TO_date('{endYear}','yyyy-MM-dd')
				//group by b.suppliers_name 
				//HAVING SUM(d.sample_qty) <> 0 OR SUM(CASE WHEN d.determine IN ('0', '1') THEN d.sample_qty ELSE 0 END) <> 0";
				//			RetDic["cake"] = Common.ReturnCakeData(DB, CakeDataSQL, 0, "Suppliers_Name", "total", "");




				ret.RetData1 = RetDic;
				ret.IsSuccess = true;
			}
			catch (Exception ex)
			{
				ret.IsSuccess = false;
				ret.ErrMsg = ex.Message;
			}
			return ret;
		}
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllHisCakeFourData(object OBJ)
		{
			SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
			SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
			SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
			try
			{
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
				string Data = ReqObj.Data.ToString();
				var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
				Dictionary<string, object> RetDic = new Dictionary<string, object>();

				#region 查询条件
				string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//
				string firmType = jarr.ContainsKey("firmType") ? jarr["firmType"].ToString() : "";//厂区类型
				string firmClass = jarr.ContainsKey("firmClass") ? jarr["firmClass"].ToString() : "";//厂商分类
				string frim = jarr.ContainsKey("frim") ? jarr["frim"].ToString() : "";//厂商
				string frimArea = jarr.ContainsKey("frimArea") ? jarr["frimArea"].ToString() : "";//厂区 
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string parts = jarr.ContainsKey("parts") ? jarr["parts"].ToString() : "";//部件名称 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string itemNo = jarr.ContainsKey("itemNo") ? jarr["itemNo"].ToString() : "";//料号
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion
				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("开始时间，结束时间为必填字段！");
				}

				#region 拼接条件
				string IWSQL = string.Empty;
				string RWSQL = string.Empty;
				if (!string.IsNullOrEmpty(firmType))//厂区类型
				{
					var arrfrimtype = string.Join(',', firmType.Split(',').Select(x => $"'{x}'"));

					//DataTable SupplierlistDt = DB.GetDataTable(HistogramDataSql);
					//List<string> stringList = new List<string>();
					//foreach (DataRow row in SupplierlistDt.Rows)
					//{
					//	string value = row["Suppliers_Name"].ToString();
					//	stringList.Add(value);
					//}
					//RWSQL += $@" AND FACTORYDATA.ORG_NAME in({arrfrimtype})";
					string[] firmTypeList = firmType.Split(',');
					string firmTypevalues = $@"select ORG_CODE from BASE001M WHERE ORG_NAME in ({string.Join(',', firmTypeList.Select(x => $"'{x}'"))})";
					DataTable firmTypevalueslistDt = DB.GetDataTable(firmTypevalues);
					List<string> firmTypevaluesList = new List<string>();
					foreach (DataRow row in firmTypevalueslistDt.Rows)
					{
						string value = row["ORG_CODE"].ToString();
						firmTypevaluesList.Add(value);
					}
					RWSQL += $@" AND A.ORG_ID in ({string.Join(',', firmTypevaluesList.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(firmClass))//厂商类型
				{
					string[] arrfirmclass = firmClass.Split(',');
					RWSQL += $@" AND B.M_TYPE in ({string.Join(',', arrfirmclass.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frim))//厂商
				{
					string[] orderListf = frim.Split(',');
					RWSQL += $@" AND b.SUPPLIERS_NAME in ({string.Join(',', orderListf.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(frimArea))//厂区 
				{

					string[] orderListfa = frimArea.Split(',');
					string frimAreavalues = $@"select COMPANY from SJQDMS_ORGINFO WHERE EN in ({string.Join(',', orderListfa.Select(x => $"'{x}'"))})";
					DataTable frimAreavalueslistDt = DB.GetDataTable(frimAreavalues);
					List<string> frimAreavaluesList = new List<string>();
					foreach (DataRow row in frimAreavalueslistDt.Rows)
					{
						string value = row["COMPANY"].ToString();
						frimAreavaluesList.Add(value);
					}
					RWSQL += $@" AND A.ORG_ID in ({string.Join(',', frimAreavaluesList.Select(x => $"'{x}'"))})";

				}
				if (!string.IsNullOrEmpty(workshop))//工段
				{
					string[] orderListwork = workshop.Split(',');
					RWSQL += $@" AND QM.WORKSHOP_SECTION_NAME in ({string.Join(',', orderListwork.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(parts))
				{

				}
				if (!string.IsNullOrEmpty(art))
				{
					string[] orderListart = art.Split(',');
					RWSQL += $@" AND rm.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					string[] orderListpo = po.Split(',');
					RWSQL += $@" AND rm.MER_PO in ({string.Join(',', orderListpo.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(itemNo))
				{

				}
				#endregion


				RetDic["Histogram1"] = null;//柱图1
											//RetDic["cake"] = null;//饼图
				#region x轴-月份
				string startYear = DateTime.Now.ToString("yyyy-01-01");
				string endYear = Convert.ToDateTime(DateTime.Now.AddYears(1).ToString("yyyy-01-01")).AddDays(-1).ToString("yyyy-MM-dd");
				List<string> MonthList = ReturnData("m", startYear, endYear);
				#endregion

				//多语言翻译
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "進行了多少次測試", "經過", "失敗" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{

					name1 = LanDic["進行了多少次測試"].ToString();
					name2 = LanDic["經過"].ToString();
					name3 = LanDic["失敗"].ToString();


				}

				string HistogramDataSql = $@"SELECT b.suppliers_name AS Suppliers_Name,
  TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM')RCPT_DATE,
    --MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME,
    SUM(a1.RCPT_QTY) AS TAKEN_QTY,
    SUM(s.PASS_QTY) AS PASS_QTY,
    SUM(rt.BAD_QTY) AS BAD_QTY 
FROM
    wms_rcpt_m a
    LEFT JOIN wms_rcpt_d a1 ON a1.CHK_NO = a.CHK_NO
-- LEFT JOIN SJQDMS_ORGINFO sj ON a.org_id = sj.COMPANY
--LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
    LEFT JOIN base003m b ON a.vend_no = b.SUPPLIERS_CODE
    LEFT JOIN qcm_iqc_insp_res_m s ON a.CHK_NO = s.CHK_NO 
        AND a1.CHK_SEQ = s.CHK_SEQ 
        AND s.isdelete = '0' 
    LEFT JOIN qcm_iqc_insp_res_bad_report rt ON a.CHK_NO = rt.CHK_NO 
        AND a1.CHK_SEQ = rt.CHK_SEQ 
        AND rt.isdelete = '0'
LEFT JOIN RQC_TASK_M rm ON B.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
WHERE  
    a.RCPT_BY = '01'  
    AND a.RCPT_DATE BETWEEN TO_date('{startYear}', 'yyyy-MM-dd') 
                        AND TO_date('{endYear}', 'yyyy-MM-dd') 
   --AND A.ORG_ID = '5001' 
{RWSQL}
    AND b.suppliers_name IS NOT NULL
GROUP BY 
    b.SUPPLIERS_NAME
  HAVING 
    SUM(a1.RCPT_QTY) > 0
ORDER BY 
    MAX(a.INSERT_DATE) DESC, 
    MAX(a1.ITEM_NO) DESC";

				DataTable SupplierlistDt = DB.GetDataTable(HistogramDataSql);
				List<string> stringList = new List<string>();
				foreach (DataRow row in SupplierlistDt.Rows)
				{
					string value = row["Suppliers_Name"].ToString();
					stringList.Add(value);
				}
				//RetDic["Histogram1"] = Common.RetunHisDatagraph(DB, stringList, HistogramDataSql, "Suppliers_Name",  "total", "pass", "fail", name1, name2, name3);//柱图1
				RetDic["Histogram1"] = Common.RetunHisDatagraph(DB, stringList, HistogramDataSql, "Suppliers_Name", "TAKEN_QTY", "PASS_QTY", "BAD_QTY", name1, name2, name3);//柱图1

				string CakeDataSQL = $@"SELECT Suppliers_Name, RCPT_DATE, TAKEN_QTY, PASS_QTY, BAD_QTY
FROM (
    SELECT 
        b.suppliers_name AS Suppliers_Name,
        TO_CHAR(MAX(a.RCPT_DATE), 'YYYY-MM') AS RCPT_DATE,
        SUM(a1.RCPT_QTY) AS TAKEN_QTY,
        SUM(s.PASS_QTY) AS PASS_QTY,
        SUM(rt.BAD_QTY) AS BAD_QTY,
        ROW_NUMBER() OVER (ORDER BY SUM(rt.BAD_QTY) DESC) AS BadQtyRank
    FROM
        wms_rcpt_m a
        LEFT JOIN wms_rcpt_d a1 ON a1.CHK_NO = a.CHK_NO
        LEFT JOIN base003m b ON a.vend_no = b.SUPPLIERS_CODE
        LEFT JOIN qcm_iqc_insp_res_m s ON a.CHK_NO = s.CHK_NO 
            AND a1.CHK_SEQ = s.CHK_SEQ 
            AND s.isdelete = '0' 
        LEFT JOIN qcm_iqc_insp_res_bad_report rt ON a.CHK_NO = rt.CHK_NO 
            AND a1.CHK_SEQ = rt.CHK_SEQ 
            AND rt.isdelete = '0'
        LEFT JOIN RQC_TASK_M rm ON b.SUPPLIERS_CODE = RM.SUPPLIERS_CODE
    WHERE  
        a.RCPT_BY = '01'  
         AND a.RCPT_DATE BETWEEN TO_date('{startYear}', 'yyyy-MM-dd') 
                        AND TO_date('{endYear}', 'yyyy-MM-dd') 
        AND b.suppliers_name IS NOT NULL{RWSQL}
    GROUP BY 
        b.SUPPLIERS_NAME
    HAVING 
        SUM(a1.RCPT_QTY) > 0 AND SUM(rt.BAD_QTY) > 0 
)
WHERE BadQtyRank <= 5
ORDER BY 
    BadQtyRank";

				RetDic["cake"] = Common.ReturnCakeData(DB, CakeDataSQL, 0, "Suppliers_Name", "BAD_QTY", "");//饼图
				ret.RetData1 = RetDic;
				ret.IsSuccess = true;
			}
			catch (Exception ex)
			{
				ret.IsSuccess = false;
				ret.ErrMsg = ex.Message;
			}
			return ret;
		}

		#endregion

		#endregion

		#region 设备参数
		/// <summary>
		/// 设备参数-表格数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEquipmentData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 查询条件
                //string PlantType = jarr.ContainsKey("PlantType") ? jarr["PlantType"].ToString() : "";//厂区/厂商类型
				//List<string> PlantType = jarr.ContainsKey("PlantType") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PlantType"] == null ? "" : jarr["PlantType"].ToString()) : new List<string>(); ;//厂区类型

				//List<string> Plant = jarr.ContainsKey("Plant") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["Plant"] == null ? "" : jarr["Plant"].ToString()) : new List<string>(); //厂商/厂区

				//List<string> ProductionLine = jarr.ContainsKey("ProductionLine") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ProductionLine"] == null ? "" : jarr["ProductionLine"].ToString()) : new List<string>();//生产线


				//List<string> DevType = jarr.ContainsKey("DevType") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["DevType"] == null ? "" : jarr["DevType"].ToString()) : new List<string>();//设备种类

				//List<string> DevNo = jarr.ContainsKey("DevNo") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["DevNo"] == null ? "" : jarr["DevNo"].ToString()) : new List<string>();//设备编号

				//List<string> Art = jarr.ContainsKey("Art") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["Art"] == null ? "" : jarr["Art"].ToString()) : new List<string>();

				//List<string> Po = jarr.ContainsKey("Po") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["Po"] == null ? "" : jarr["DevNo"].ToString()) : new List<string>();

                string PlantType = jarr.ContainsKey("PlantType") ? jarr["PlantType"].ToString() : "";//厂区类型
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";//厂商/厂区
				string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";//生产线
				string DevType = jarr.ContainsKey("DevType") ? jarr["DevType"].ToString() : "";//设备种类
				string DevNo = jarr.ContainsKey("DevNo") ? jarr["DevNo"].ToString() : "";//设备编号
				string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//Art
				string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//Po






				//string DevNo = jarr.ContainsKey("DevNo") ? jarr["DevNo"].ToString() : "";//设备编号
				//string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";//生产线
				//string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//ART
				//            string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//Po

				string StartTime = jarr.ContainsKey("StartTime") ? jarr["StartTime"].ToString() : "";//开始时间
                string EndTime = jarr.ContainsKey("EndTime") ? jarr["EndTime"].ToString() : "";//结束时间
                #endregion

                #region 必要参数判定
                if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime))
                {
                    throw new Exception($@"缺少必要参数【开始时间】【结束时间】");
                }
                #endregion

                string WSQL = string.Empty;//查询条件
				string WSQL2 = string.Empty;//查询条件// written by Ashok

				#region 组合查询条件
				string[] arr = new string[] { };
				if (!string.IsNullOrEmpty(StartTime) && !string.IsNullOrEmpty(EndTime))
				{
					WSQL2 += $@"	AND createdate between '{StartTime}' and '{EndTime}'";
				}
				if (!string.IsNullOrEmpty(PlantType))
                {
					arr = PlantType.Split(',');
					WSQL += $@" AND c.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Plant))
                {
					arr = Plant.Split(',');
					//WSQL += $@"AND c.PLANT_AREA in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WSQL += $@"AND c.org in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ProductionLine))
                {
					//WSQL += $@"AND (c.PRODUCTION_LINE_CODE LIKE '{ProductionLine}%' or c.PRODUCTION_LINE_NAME LIKE '{ProductionLine}%')";
					arr = ProductionLine.Split(',');

					//WSQL += $@"AND c.PRODUCTION_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WSQL += $@"AND c.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Po))
                {
					arr = Po.Split(',');
					WSQL += $@"AND b.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Art))
                {
                    WSQL += $@"AND b.PROD_NO in ('{string.Join("','", Art)}')";
                }
                if (!string.IsNullOrEmpty(DevType))
                {
					arr = DevType.Split(',');
					//WSQL += $@"	AND (d.EQ_NO like '{DevType}%' or d.EQ_NAME like '{DevType}%')";
					WSQL += $@"	AND d.EQ_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(DevNo))
                {
					arr = DevNo.Split(',');
					WSQL += $@"	AND d.EQ_INFO_NO='{DevNo}'";
					//WSQL += $@"	AND d.EQ_INFO_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }

                #endregion
                #region old SQL
                //string DataSQL = $@"WITH depart_tmp AS (
                //					SELECT
                //						c.ORG_NAME,c.ORG_CODE,
                //						PRODUCTION_LINE_CODE,
                //						PRODUCTION_LINE_NAME,
                //						PLANT_AREA 
                //					FROM
                //						bdm_production_line_m a
                //						LEFT JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
                //						LEFT JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE UNION ALL
                //					SELECT
                //						c.ORG_NAME,c.ORG_CODE,
                //						DEPARTMENT_CODE,
                //						DEPARTMENT_NAME,
                //						org 
                //					FROM
                //						base005m a
                //						LEFT JOIN SJQDMS_ORGINFO b ON a.udf05 = b.code
                //						LEFT JOIN base001m c ON a.FACTORY_SAP = c.ORG_CODE 
                //					) SELECT
                //					c.PLANT_AREA,--厂区
                //					c.PRODUCTION_LINE_CODE,--产线编号
                //					c.PRODUCTION_LINE_NAME,--产线名称
                //					d.EQ_NO dev_type_no,--设备类型编号
                //					d.EQ_NAME dev_type_name,--设备类型名称
                //					d.EQ_INFO_NO,--设备编号
                //					d.EQ_INFO_NAME,--设备名称
                //					a.PARAM_ITEM_NO,--参数代号
                //					a.param_item_name,--参数名称
                //					a.sop_standard,--参数标准范围
                //					a.actual_standard,--实际参数
                //					a.SOP_STANDARD_END,--上限
                //					'0' status,
                //					im.REMARK -- 单位
                //				FROM
                //					rqc_task_xj_item_c a
                //					JOIN rqc_task_m b ON a.TASK_NO = b.TASK_NO
                //					LEFT JOIN depart_tmp c ON b.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE
                //					LEFT JOIN bdm_eq_info_m d ON a.EQ_INFO_NO = d.EQ_INFO_NO
                //					LEFT JOIN BDM_PARAM_ITEM_M im ON im.PARAM_ITEM_NO = a.PARAM_ITEM_NO --2023.4.25修改添加单位
                //				WHERE
                //					( a.eq_info_no, a.param_item_no, a.id ) IN ( SELECT EQ_INFO_NO, PARAM_ITEM_NO, max( id ) FROM rqc_task_xj_item_c where 1=1 {WSQL2} GROUP BY EQ_INFO_NO, PARAM_ITEM_NO ) 
                //					{WSQL}";
                #endregion
                string DataSQL = $@"WITH depart_tmp AS
 (SELECT c.ORG_NAME,
         c.ORG_CODE,
         DEPARTMENT_CODE ,
         DEPARTMENT_NAME,
         org 
    FROM base005m a
    INNER JOIN SJQDMS_ORGINFO b
      ON a.udf05 = b.code
    INNER JOIN base001m c
      ON a.FACTORY_SAP = c.ORG_CODE)
SELECT c.org PLANT_AREA, 
       c.DEPARTMENT_CODE PRODUCTION_LINE_CODE, 
       c.DEPARTMENT_NAME PRODUCTION_LINE_NAME, 
       d.EQ_NO dev_type_no, 
       d.EQ_NAME dev_type_name, 
       d.EQ_INFO_NO, 
       d.EQ_INFO_NAME, 
       a.PARAM_ITEM_NO, 
       a.param_item_name, 
       a.sop_standard,
       a.actual_standard, 
       a.SOP_STANDARD_END, 
       '0' status,
       im.REMARK 
FROM rqc_task_xj_item_c a
 inner JOIN rqc_task_m b
    ON a.TASK_NO = b.TASK_NO 
  inner JOIN depart_tmp c
    ON b.PRODUCTION_LINE_CODE = c.DEPARTMENT_CODE
  inner JOIN bdm_eq_info_m d
    ON a.EQ_INFO_NO = d.EQ_INFO_NO
  inner JOIN BDM_PARAM_ITEM_M im
    ON im.PARAM_ITEM_NO = a.PARAM_ITEM_NO
								WHERE
									( a.eq_info_no, a.param_item_no, a.id ) IN ( SELECT EQ_INFO_NO, PARAM_ITEM_NO, max( id ) FROM rqc_task_xj_item_c where 1=1 {WSQL2} GROUP BY EQ_INFO_NO, PARAM_ITEM_NO ) 
									{WSQL}";

				DataTable Dt = DB.GetDataTable(DataSQL);
                foreach (DataRow item in Dt.Rows)
                {
                    decimal Num = 0;
                    decimal HNum = 0;
                    decimal.TryParse(item["actual_standard"].ToString(), out Num);
                    decimal.TryParse(item["SOP_STANDARD_END"].ToString(), out HNum);
                    if (Num > HNum && HNum > 0)
                    {
                        item["status"] = "1";
                    }
                }
                ret.RetData1 = Dt;
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
        /// 设备参数-展开数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject OpenEquipmentData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 查询条件
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//
                string DevNo = jarr.ContainsKey("DevNo") ? jarr["DevNo"].ToString() : "";//设备编号
                string DevName = jarr.ContainsKey("DevName") ? jarr["DevName"].ToString() : "";//设备名称
                string StartTime = jarr.ContainsKey("StartTime") ? jarr["StartTime"].ToString() : "";//开始时间
                string EndTime = jarr.ContainsKey("EndTime") ? jarr["EndTime"].ToString() : "";//结束时间
                string ParaItemNo = jarr.ContainsKey("ParaItemNo") ? jarr["ParaItemNo"].ToString() : "";//数据参数代号
                string ParaItemName = jarr.ContainsKey("ParaItemName") ? jarr["ParaItemName"].ToString() : "";//数据参数名称
                #endregion

                #region 必要参数判定
                if (string.IsNullOrEmpty(DevNo))
                {
                    throw new Exception($@"缺少必要参数【设备代号】【DevNo】");
                }
                if (string.IsNullOrEmpty(ParaItemNo))
                {
                    throw new Exception($@"缺少必要参数【参数项目代号】【ParaItemNo】");
                }
                #endregion

                string WSQL = string.Empty;//查询条件

                #region 组合查询条件
                if (!string.IsNullOrEmpty(ParaItemNo))
                {
                    WSQL += $@" AND PARAM_ITEM_NO = '{ParaItemNo}' ";
                }
                if (!string.IsNullOrEmpty(DevNo))
                {
                    WSQL += $@"	AND d.EQ_INFO_NO='{DevNo}'";
                }
                #endregion

                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                string DataSQL = $@"SELECT
										EQ_INFO_NO,
										PARAM_ITEM_NO,
										CREATEDATE,--日期
										MAX( SOP_STANDARD_START ) SOP_STANDARD_START,--技术标准上限
										MAX( SOP_STANDARD ) SOP_STANDARD,--数据区间
										MAX( SOP_STANDARD_END ) SOP_STANDARD_END,--技术标准下限
										MAX( ACTUAL_STANDARD ) ACTUAL_STANDARD --实际值
									FROM
										rqc_task_xj_item_c 
									WHERE
										EQ_INFO_NO ='{DevNo}'
										AND PARAM_ITEM_NO = '{ParaItemNo}' 
										AND CREATEDATE BETWEEN '{StartTime}' 
										AND '{EndTime}' 
									GROUP BY
										EQ_INFO_NO,
										CREATEDATE,
										PARAM_ITEM_NO 
									ORDER BY
										CREATEDATE";
                DataTable Dt = DB.GetDataTable(DataSQL);
				#region 数据转换
				//foreach (DataRow item in Dt.Rows)
				//{
				//	decimal Num = 0;
				//	string Sop_str = item["SOP_STANDARD"].ToString();
				//	if (Sop_str.Contains("-"))
				//	{
				//		if (Sop_str.Contains("undefined"))
				//		{
				//			continue;
				//		}
				//		List<string> StrList = Sop_str.Split("-").ToList();
				//		if (StrList.Count >= 2)
				//		{
				//			if (string.IsNullOrEmpty(StrList[StrList.Count - 1].Trim()))
				//			{
				//				continue;
				//			}
				//			string NewSopStr = StrList[StrList.Count - 1].Replace("℃", "").Replace("°", "").Replace("kg", "").Replace("分钟", "").Replace("秒", "");
				//			//00:00:00数据格式处理
				//			if (NewSopStr.Contains(":"))
				//			{
				//				decimal HNums = 0;
				//				decimal MNums = 0;
				//				decimal SNums = 0;
				//				StrList = NewSopStr.Split(":").ToList();
				//				if (StrList.Count() == 2)
				//				{
				//					decimal.TryParse(StrList[0], out MNums);
				//					decimal.TryParse(StrList[1], out SNums);
				//					Num = MNums * 60 + SNums;
				//				}
				//				else if (StrList.Count() == 3)
				//				{
				//					decimal.TryParse(StrList[0], out HNums);
				//					decimal.TryParse(StrList[1], out MNums);
				//					decimal.TryParse(StrList[2], out SNums);
				//					Num = HNums * 60 + MNums * 60 + SNums;
				//				}
				//				else
				//				{
				//					throw new Exception(NewSopStr);
				//				}

				//			}
				//			//25' 15''数据格式处理
				//			else if (NewSopStr.Contains("'") || NewSopStr.Contains("''"))
				//			{
				//				decimal HNums = 0;
				//				decimal MNums = 0;
				//				decimal SNums = 0;
				//				StrList = NewSopStr.Split("''").ToList();
				//				if (StrList.Count() > 0)
				//				{
				//					StrList = NewSopStr.Split("'").ToList();
				//					decimal.TryParse(StrList[0], out MNums);
				//					decimal.TryParse(StrList[1], out SNums);
				//					Num = HNums * 60 + MNums * 60 + SNums;
				//				}
				//			}
				//			else if (!decimal.TryParse(NewSopStr, out Num))
				//			{
				//				throw new Exception(NewSopStr);
				//			}
				//			item["SOP_STANDARD_START"] = Num;
				//		}
				//	}
				//}
				#endregion

				//多语言翻译
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "实际参数", "SOP标准" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				if (LanDic.Count > 0)
				{
					name1 = LanDic["实际参数"].ToString();
					name2 = LanDic["SOP标准"].ToString();

				}

				List<string> MonthList = Common.ReturnData("d", StartTime, EndTime);
                RetDic = RetunHisData(Dt, MonthList, "CREATEDATE", "ACTUAL_STANDARD", "SOP_STANDARD_END", $@"{DevName}-{name1}", name2);
                ret.RetData1 = RetDic;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        public static Dictionary<string, object> RetunHisData(DataTable Dt, List<string> TimeList, string DtSelectStr, string LineStr1, string LineStr2, string DSLineName1, string DSLineName2)
        {
            Dictionary<string, object> HisDic = new Dictionary<string, object>();
            HisDic["x"] = TimeList;//x轴
            HisDic["line"] = null;//折线


            List<decimal> LineList1 = new List<decimal>();
            List<decimal> LineList2 = new List<decimal>();
            List<Dictionary<string, object>> LabBarDic = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> LabLinerDic = new List<Dictionary<string, object>>();
            foreach (var item in TimeList)
            {
                decimal LineNum1 = 0;
                decimal LineNum2 = 0;
                DataRow[] SelectRow = Dt.Select($@"{DtSelectStr}='{item}'");
                foreach (DataRow Ritem in SelectRow)
                {
                    LineNum1 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr1].ToString()) ? "0" : Ritem[LineStr1].ToString());
                    LineNum2 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr2].ToString()) ? "0" : Ritem[LineStr2].ToString());
                }
                LineList1.Add(LineNum1);
                LineList2.Add(LineNum2);
            }

            //柱状图数据
            Dictionary<string, object> LineDic1 = new Dictionary<string, object>();
            LineDic1["name"] = DSLineName1;
            LineDic1["data"] = LineList1;
            LabBarDic.Add(LineDic1);

			


			Dictionary<string, object> LineDic2 = new Dictionary<string, object>();
            LineDic2["name"] = DSLineName2;
            LineDic2["data"] = LineList2;
            LabBarDic.Add(LineDic2);
            HisDic["line"] = LabBarDic;
            return HisDic;
        }
        #endregion
    }
}
