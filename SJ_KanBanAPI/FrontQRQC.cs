using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// 车间Q——RQC抽检
    /// </summary>
    internal class FrontQRQC
    {
		public static string moudle_code = "rqcExtractionTest";//rqc
        /// <summary>
        /// RQC年度抽验次数数据,扇形
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetHisYearRQCData(object OBJ)
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
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言

                string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
                string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
                string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
                string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
                string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
                string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
                string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                #endregion

                string startYear = DateTime.Now.ToString("yyyy-01-01");
                string endYear = Convert.ToDateTime(startYear).AddYears(1).AddDays(-1).ToString("yyyy-MM-dd");
                List<string> monthList = Common.ReturnData("m", startYear, endYear);

				string HisWhereSQL = string.Empty;//柱图条件
				string CakeWhereSQL = string.Empty;//饼图条件
               
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["HisData"] = null;
                RetDic["CakeData"] = null;

				string[] arr = new string[] { };



                #region Combination conditions
                #region Commented by ashok on 2025/05/09 to query the data queckly

    //            if (!string.IsNullOrEmpty(plantType))//Plant 
				//{
				//	arr = plantType.Split(',');
				//	HisWhereSQL += $@"and B1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

				//	//HisWhereSQL += $@" AND B1.ORG_NAME LIKE '{plantType}%'";
				//}
				//if (!string.IsNullOrEmpty(workshop))//工段
				//{
				//	arr = workshop.Split(',');
				//	HisWhereSQL += $@"and qm.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	//HisWhereSQL += $@" AND qm.WORKSHOP_SECTION_NAME like '{workshop}%'";

				//}
				//if (!string.IsNullOrEmpty(depart))//部门
				//{
				//	arr = depart.Split(',');
				//	HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

				//	//HisWhereSQL += $@" AND a.DEPARTMENT LIKE '{depart}%'";
				//	//CakeWhereSQL += $@" AND rc.DEPARTMENT LIKE '{depart}%'";
				//}
				//if (!string.IsNullOrEmpty(technology))//工艺/材料种类
				//{
				//	arr = technology.Split(',');
				//	HisWhereSQL += $@"and t.WORKMANSHIP_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKMANSHIP_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

				//	//HisWhereSQL += $@" AND a.CONFIG_NO like '{technology}%'";
				//	//CakeWhereSQL += $@" AND rc.CONFIG_NO like '{technology}%'";
				//}
				//if (!string.IsNullOrEmpty(machine))//机台
				//{
				//	HisWhereSQL += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	CakeWhereSQL += $@" AND rc.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))//模号
				//{
				//	HisWhereSQL += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	CakeWhereSQL += $@" AND rc.MOLD_NO  LIKE '{mold_no}%'";
				//}

                #endregion

                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
					HisWhereSQL += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    CakeWhereSQL += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
				
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					HisWhereSQL += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					HisWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					HisWhereSQL += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				#endregion

				//RQC抽验柱状
				//string LeftJoinSQL = $@"		LEFT JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.SUPPLIERS_CODE
				//								LEFT JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
				//								LEFT JOIN BASE001M B1 ON B1.ORG_CODE=BM.FACTORY_SAP
				//								LEFT JOIN QCM_DQA_MAG_M qm on qm.workshop_section_no= a.WORKSHOP_SECTION_NO and  a.SHOE_NO=qm.SHOES_CODE"; //original One

				string LeftJoinSQL = $@"		inner JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.production_line_code
                                                inner JOIN bdm_rd_style x ON X.SHOE_NO=A.SHOE_NO
												inner JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
												--inner JOIN BASE001M B1 ON B1.ORG_CODE=BM.FACTORY_SAP
												--inner JOIN QCM_DQA_MAG_M qm on qm.workshop_section_no= a.WORKSHOP_SECTION_NO and  a.SHOE_NO=qm.SHOES_CODE"; //changed by Ashok
				string JoinSql = string.IsNullOrEmpty(HisWhereSQL) ? "" : LeftJoinSQL;
				string RQCYearDataSQL = $@"WITH rqc_data AS (
												SELECT
													to_char( to_date( d.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) createdate,
												sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
												sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
											FROM
												rqc_task_m a
												inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO 
												 --left join BDM_PARAM_ITEM_D t on t.WORKMANSHIP_CODE = a.CONFIG_NO
												{JoinSql}
											WHERE
												substr( d.CREATEDATE, 0, 4 ) = to_char( SYSDATE, 'yyyy' ) 
												{HisWhereSQL}
											GROUP BY
												to_char( to_date( d.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
												) SELECT
												createdate,
												pass_qty,
												bad_qty,
											CASE
		
													WHEN pass_qty + bad_qty > 0 THEN
													round( pass_qty / ( pass_qty + bad_qty ) * 100, 1 ) ELSE 0 
												END pass_rate 
											FROM
												rqc_data";

				//Multi-language translation
				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "RQC抽验合格数", "RQC抽验不合格数", "合格率" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{
					name1 = LanDic["RQC抽验合格数"].ToString();
					name2 = LanDic["RQC抽验不合格数"].ToString();
					name3 = LanDic["合格率"].ToString();

				}

				RetDic["HisData"] = Common.RetunHisData(DB,monthList,RQCYearDataSQL, "createdate", "pass_qty", "bad_qty", "pass_rate", name1, name2, name3);
                #region old
                //RQC抽验饼图
                //    string RQCCakeYearDataSql = $@"WITH rqc_bad_data_tmp AS (
                //	SELECT
                //		c.INSPECTION_NAME,--count(c.id) bad_qty
                //	sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
                //	sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
                //FROM
                //	rqc_task_detail_t a
                //	JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
                //	AND a.COMMIT_INDEX = b.COMMIT_INDEX
                //	JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
                //	AND b.UNION_ID = c.ID
                //	LEFT JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
                //	LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=rc.PRODUCTION_LINE_CODE
                //	LEFT JOIN BASE005M bm on bm.DEPARTMENT_CODE=rc.DEPARTMENT
                //	LEFT JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
                //       LEFT join BDM_PARAM_ITEM_D t on t.WORKMANSHIP_CODE = rc.CONFIG_NO
                //WHERE
                //	a.CREATEDATE BETWEEN '{startYear}' 
                //	AND '{endYear}' 
                //	{CakeWhereSQL}
                //GROUP BY
                //	c.INSPECTION_NAME 
                //	),
                //	rqc_bad_data AS (
                //	SELECT
                //		INSPECTION_NAME,
                //		pass_qty,
                //		bad_qty,
                //	CASE
                //			WHEN pass_qty + bad_qty > 0 THEN
                //			round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
                //		END pass_rate,
                //	ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC ) ranking 
                //FROM
                //	rqc_bad_data_tmp 
                //	) --不良问题点占比
                //SELECT
                //	* 
                //FROM
                //	rqc_bad_data";      ////THIS IS ORIGINAL QUERY
                #endregion

                string RQCCakeYearDataSql = $@"WITH rqc_bad_data_tmp AS (
													SELECT
														c.INSPECTION_NAME,--count(c.id) bad_qty
													sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
													sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
												FROM
													rqc_task_detail_t a
													inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
													AND a.COMMIT_INDEX = b.COMMIT_INDEX
													inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
													AND b.UNION_ID = c.ID
													inner JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
                                                    inner JOIN bdm_rd_style x ON X.SHOE_NO=rc.SHOE_NO  
													--LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=rc.PRODUCTION_LINE_CODE
													inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=rc.DEPARTMENT
													inner JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
											        --LEFT join BDM_PARAM_ITEM_D t on t.CONFIG_NO = rc.CONFIG_NO
												WHERE
													a.CREATEDATE BETWEEN '{startYear}' 
													AND '{endYear}' 
													{CakeWhereSQL}
												GROUP BY
													c.INSPECTION_NAME 
													),
													rqc_bad_data AS (
													SELECT
														INSPECTION_NAME,
														pass_qty,
														bad_qty,
													CASE
															WHEN pass_qty + bad_qty > 0 THEN
															round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
														END pass_rate,
													ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC ) ranking 
												FROM
													rqc_bad_data_tmp 
													) --不良问题点占比
												SELECT
													* 
												FROM
													rqc_bad_data";  //This is changed by Ashok
				RetDic["CakeData"] = Common.ReturnCakeData(DB, RQCCakeYearDataSql, 1, "INSPECTION_NAME", "bad_qty", "ranking");

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
		/// RQC时间区间内抽验次数数据,扇形
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetHisDayRQCData(object OBJ)
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
				string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
				string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
				string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
				string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
				string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
				string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion

				if (string.IsNullOrEmpty(startTime)||string.IsNullOrEmpty(endTime))
                {
					throw new Exception("必填条件【开始时间】【结束时间】不能为空！");
                }

				List<string> dayList = Common.ReturnData("d", startTime, endTime);

				string HisWhereSQL = string.Empty;//柱图条件
				string CakeWhereSQL = string.Empty;//饼图条件
				string[] arr = new string[] { };

				Dictionary<string, object> RetDic = new Dictionary<string, object>();
				RetDic["HisData"] = null;
				RetDic["CakeData"] = null;


                #region 组合查询【多选】
                #region commneted by Ashok on 2025/05/09 to query the data queckly
    //            if (!string.IsNullOrEmpty(plantType))
				//{
				//	arr = plantType.Split(',');
				//	HisWhereSQL += $@"and B1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(workshop))
				//{
				//	arr = workshop.Split(',');
				//	HisWhereSQL += $@"and qm.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(depart))
				//{
				//	arr = depart.Split(',');
				//	HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(technology))
				//{
				//	arr = technology.Split(',');
				//	HisWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(machine))
				//{
				//	HisWhereSQL += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	CakeWhereSQL += $@" AND rc.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))
				//{
				//	HisWhereSQL += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	CakeWhereSQL += $@" AND rc.MOLD_NO  LIKE '{mold_no}%'";
				//}
                #endregion 
                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
					HisWhereSQL += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    CakeWhereSQL += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					HisWhereSQL += $@"and bm.DEPARTMENT_NAME  in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    CakeWhereSQL += $@"and bm.DEPARTMENT_NAME  in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					HisWhereSQL += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					HisWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					HisWhereSQL += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				#endregion

				//RQC抽验柱状
				//string LeftJoinSQL = $@"		LEFT JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.SUPPLIERS_CODE
				//								LEFT JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
				//								LEFT JOIN BASE001M B1 ON B1.ORG_CODE=BM.FACTORY_SAP
				//								LEFT JOIN QCM_DQA_MAG_M qm on qm.workshop_section_no= a.WORKSHOP_SECTION_NO AND a.SHOE_NO=qm.SHOES_CODE";

				string LeftJoinSQL = $@"		inner JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.production_line_code
                                                inner JOIN bdm_rd_style x  ON X.SHOE_NO=A.SHOE_NO
												inner JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
												--inner JOIN BASE001M B1 ON B1.ORG_CODE=BM.FACTORY_SAP
												--inner JOIN QCM_DQA_MAG_M qm on qm.workshop_section_no= a.WORKSHOP_SECTION_NO and  a.SHOE_NO=qm.SHOES_CODE";
				string JoinSql = string.IsNullOrEmpty(HisWhereSQL) ? "" : LeftJoinSQL;
				string RQCDayDataSQL = $@"WITH rqc_data AS (
												SELECT
													 d.CREATEDATE createdate,
												sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
												sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
											FROM
												rqc_task_m a
												inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO 
												--inner join BDM_PARAM_ITEM_D t on t.WORKMANSHIP_CODE = a.CONFIG_NO
												{JoinSql}
											WHERE
												d.CREATEDATE between '{startTime}' and '{endTime}' 
												{HisWhereSQL}
											GROUP BY
												d.createdate 
												) SELECT
												createdate,
												pass_qty,
												bad_qty,
											CASE
		
													WHEN pass_qty + bad_qty > 0 THEN
													round( pass_qty / ( pass_qty + bad_qty ) * 100, 1 ) ELSE 0 
												END pass_rate 
											FROM
												rqc_data";
				//RetDic["HisData"] = Common.RetunHisData(DB, dayList, RQCDayDataSQL, "createdate", "pass_qty", "bad_qty", "pass_rate", "RQC抽验合格数", "RQC抽验不合格数", "合格率");
				RetDic["HisData"] = Common.RetunHisData(DB, dayList, RQCDayDataSQL, "createdate", "pass_qty", "bad_qty", "pass_rate", "RQC random inspection qualified number", "RQC random inspection failed number", "pass rate");

				//RQC抽验饼图
				//string RQCCakeDayDataSql = $@"WITH rqc_bad_data_tmp AS (
				//									SELECT
				//										c.INSPECTION_NAME,--count(c.id) bad_qty
				//									sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
				//									sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
				//								FROM
				//									rqc_task_detail_t a
				//									JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
				//									AND a.COMMIT_INDEX = b.COMMIT_INDEX
				//									JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
				//									AND b.UNION_ID = c.ID
				//									LEFT JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
				//									LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=rc.PRODUCTION_LINE_CODE
				//									LEFT join BDM_PARAM_ITEM_D t on t.WORKMANSHIP_CODE = rc.CONFIG_NO
				//									LEFT JOIN BASE005M bm on bm.DEPARTMENT_CODE=rc.DEPARTMENT
				//									LEFT JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
				//								WHERE
				//									a.CREATEDATE BETWEEN '{startTime}' 
				//									AND '{endTime}' 
				//									{CakeWhereSQL}
				//								GROUP BY
				//									c.INSPECTION_NAME 
				//									),
				//									rqc_bad_data AS (
				//									SELECT
				//										INSPECTION_NAME,
				//										pass_qty,
				//										bad_qty,
				//									CASE
				//											WHEN pass_qty + bad_qty > 0 THEN
				//											round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
				//										END pass_rate,
				//									ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC ) ranking 
				//								FROM
				//									rqc_bad_data_tmp 
				//									) --不良问题点占比
				//								SELECT
				//									* 
				//								FROM
				//									rqc_bad_data";   //////THIS IS ORIGINAL QUERY

				string RQCCakeDayDataSql = $@"WITH rqc_bad_data_tmp AS (
													SELECT
														c.INSPECTION_NAME,--count(c.id) bad_qty
													sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
													sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
												FROM
													rqc_task_detail_t a
													inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
													AND a.COMMIT_INDEX = b.COMMIT_INDEX
													inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
													AND b.UNION_ID = c.ID
													inner JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
                                                    inner JOIN bdm_rd_style x ON X.SHOE_NO=rc.SHOE_NO 
													--LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=rc.PRODUCTION_LINE_CODE
													--LEFT join BDM_PARAM_ITEM_D t on t.CONFIG_NO = rc.CONFIG_NO
													inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=rc.DEPARTMENT
													inner JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
												WHERE
													a.CREATEDATE BETWEEN '{startTime}' 
													AND '{endTime}' 
													{CakeWhereSQL}
												GROUP BY
													c.INSPECTION_NAME 
													),
													rqc_bad_data AS (
													SELECT
														INSPECTION_NAME,
														pass_qty,
														bad_qty,
													CASE
															WHEN pass_qty + bad_qty > 0 THEN
															round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
														END pass_rate,
													ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC ) ranking 
												FROM
													rqc_bad_data_tmp 
													) --不良问题点占比
												SELECT
													* 
												FROM
													rqc_bad_data";  //This is changed by Ashok
				RetDic["CakeData"] = Common.ReturnCakeData(DB, RQCCakeDayDataSql, 1, "INSPECTION_NAME", "bad_qty", "ranking");

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
		/// 前5名不通过项目列表
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTop5FailData(object OBJ)
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
				string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "false";//厂区类型

				string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
				string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
				string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
				string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
				string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
				string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("必填条件【开始时间】【结束时间】不能为空！");
				}
				string WhereRow = string.Empty;
                if (!bool.Parse(IsExport))
                {
					WhereRow = " and ranking<=5";

				}

				List<string> dayList = Common.ReturnData("d", startTime, endTime);

				string HisWhereSQL = string.Empty;//柱图条件
				string CakeWhereSQL = string.Empty;//饼图条件

				Dictionary<string, object> RetDic = new Dictionary<string, object>();
				RetDic["Top5FailData"] = null;



				#region Combined query [multiple selection]
				string[] arr = new string[] { };

                #region commented by Ashok to query the data queckly
    //            if (!string.IsNullOrEmpty(plantType))
				//{
				//	arr = plantType.Split(',');
				//	HisWhereSQL += $@"and B1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(workshop))
				//{
				//	arr = workshop.Split(',');
				//	CakeWhereSQL += $@"and t.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(depart))
				//{
				//	arr = depart.Split(',');
				//	HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(technology))
				//{
				//	arr = technology.Split(',');
				//	HisWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(machine))
				//{
				//	HisWhereSQL += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	CakeWhereSQL += $@" AND rc.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))
				//{
				//	HisWhereSQL += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	CakeWhereSQL += $@" AND rc.MOLD_NO  LIKE '{mold_no}%'";
				//}
                #endregion

                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
                    CakeWhereSQL += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					HisWhereSQL += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					HisWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					HisWhereSQL += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				#endregion

				//RQC抽验饼图
				string RQCTableDataSql = $@"WITH rqc_bad_data_tmp AS (
													SELECT
														c.INSPECTION_NAME,--count(c.id) bad_qty
													sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
													sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
												FROM
													rqc_task_detail_t a
													inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
													AND a.COMMIT_INDEX = b.COMMIT_INDEX
													inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
													AND b.UNION_ID = c.ID
													inner JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
                                                    inner JOIN bdm_rd_style x ON X.SHOE_NO=rc.SHOE_NO 
													--LEFT join BDM_PARAM_ITEM_D t on t.CONFIG_NO = rc.CONFIG_NO
													--LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=rc.PRODUCTION_LINE_CODE
													inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=rc.DEPARTMENT
													inner JOIN SJQDMS_ORGINFO sj ON bm.udf05 = sj.code
												WHERE
													a.CREATEDATE BETWEEN '{startTime}' 
													AND '{endTime}' 
													{CakeWhereSQL}
												GROUP BY
													c.INSPECTION_NAME 
													),
													rqc_bad_data AS (
													SELECT
														INSPECTION_NAME,
														pass_qty,
														bad_qty,
													CASE
															WHEN pass_qty + bad_qty > 0 THEN
															round( pass_qty / ( pass_qty + bad_qty ) * 100, 1 ) ELSE 0 
														END pass_rate,
													ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC ) ranking 
												FROM
													rqc_bad_data_tmp 
													) --不良问题点占比
												SELECT
													INSPECTION_NAME,
													( PASS_QTY + BAD_QTY ) testqty,
													bad_qty,
													pass_rate,
													ranking 
												FROM
													rqc_bad_data where 1=1 {WhereRow}";
				RetDic["Top5FailData"] = DB.GetDataTable(RQCTableDataSql);

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
		/// 前5名不通过项目列表
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTop5FailHisDataData(object OBJ)
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
				string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
				string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
				string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
				string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
				string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
				string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				string INSPECTION_NAME = jarr.ContainsKey("INSPECTION_NAME") ? jarr["INSPECTION_NAME"].ToString() : "";//结束时间
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("必填条件【开始时间】【结束时间】不能为空！");
				}

				if (string.IsNullOrEmpty(INSPECTION_NAME))
				{
					throw new Exception("必填条件【检验项目】不能为空！");
				}
				string INSPECTION_NAMESTR = string.Empty;

				if (!string.IsNullOrEmpty(INSPECTION_NAME))
				{
					INSPECTION_NAMESTR = $@" AND C.INSPECTION_NAME='{INSPECTION_NAME}'";

				}
				List<string> dayList = Common.ReturnData("d", startTime, endTime);

				string HisWhereSQL = string.Empty;//柱图条件
				string CakeWhereSQL = string.Empty;//饼图条件

				Dictionary<string, object> RetDic = new Dictionary<string, object>();
				RetDic["HisData"] = null;
				RetDic["CakeData"] = null;

			
				#region 组合查询【多选】
				string[] arr = new string[] { };

                #region commented by Ashok on 2025/05/09 to query the data queckly
    //            if (!string.IsNullOrEmpty(plantType))
				//{
				//	arr = plantType.Split(',');
				//	HisWhereSQL += $@"and B1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(workshop))
				//{
				//	arr = workshop.Split(',');
				//	HisWhereSQL += $@"and qm.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(depart))
				//{
				//	arr = depart.Split(',');
				//	HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(technology))
				//{
				//	arr = technology.Split(',');
				//	HisWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	CakeWhereSQL += $@"and t.WORKMANSHIP_CODE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(machine))
				//{
				//	HisWhereSQL += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	CakeWhereSQL += $@" AND rc.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))
				//{
				//	HisWhereSQL += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	CakeWhereSQL += $@" AND rc.MOLD_NO  LIKE '{mold_no}%'";
				//}
                #endregion

                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
					HisWhereSQL += $@"and bm.UDF05 in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					HisWhereSQL += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					HisWhereSQL += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and rc.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					HisWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					CakeWhereSQL += $@"and X.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					HisWhereSQL += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				#endregion

				//RQC抽验柱状
				string LeftJoinSQL = $@"		inner JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.PRODUCTION_LINE_CODE   
												--LEFT JOIN BASE001M B1 ON B1.ORG_CODE=BM.FACTORY_SAP
												--LEFT JOIN QCM_DQA_MAG_M qm on qm.workshop_section_no= a.WORKSHOP_SECTION_NO   AND a.SHOE_NO=qm.SHOES_CODE
												--LEFT JOIN BDM_PRODUCTION_LINE_M bl on bl.PRODUCTION_LINE_CODE=a.PRODUCTION_LINE_CODE";  //--here a.SUPPLIERS_CODE is changed to a.PRODUCTION_LINE_CODE by Ashok
				string JoinSql = string.IsNullOrEmpty(HisWhereSQL) ? "" : LeftJoinSQL;
				#region old commneted on 2025/05/09 by Ashok

				//string HisTop5FailDataSQL = $@"WITH rqc_bad_data_tmp AS (
				//									SELECT
				//										c.INSPECTION_NAME,
				//									sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
				//									sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
				//								FROM
				//									rqc_task_detail_t a
				//									inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
				//									AND a.COMMIT_INDEX = b.COMMIT_INDEX
				//									inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
				//									AND b.UNION_ID = c.ID 
				//									inner JOIN RQC_TASK_M rc ON rc.task_no = a.task_no 
				//									inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=rc.DEPARTMENT
				//									--LEFT join BDM_PARAM_ITEM_D t on t.WORKMANSHIP_CODE = rc.CONFIG_NO
				//								WHERE
				//									a.CREATEDATE BETWEEN '{startTime}' 
				//									AND '{endTime}' {CakeWhereSQL}  {INSPECTION_NAMESTR}
				//								GROUP BY
				//									c.INSPECTION_NAME 
				//									),
				//									rqc_bad_data AS (
				//									SELECT
				//										INSPECTION_NAME,
				//										pass_qty,
				//										bad_qty,
				//										round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) pass_rate,
				//										ROW_NUMBER ( ) over ( ORDER BY bad_qty DESC, pass_qty + bad_qty DESC ) ranking 
				//									FROM
				//										rqc_bad_data_tmp 
				//									),
				//									rqc_data AS (
				//									SELECT
				//										a.createdate,
				//									sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
				//									sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
				//								FROM
				//									rqc_task_m a
				//									inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
				//									inner JOIN rqc_task_detail_t_d b ON d.TASK_NO = b.TASK_NO 
				//									AND d.COMMIT_INDEX = b.COMMIT_INDEX
				//									inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
				//									AND b.UNION_ID = c.ID
				//									inner JOIN rqc_bad_data e ON e.INSPECTION_NAME = c.INSPECTION_NAME 
				//									{JoinSql}
				//								WHERE
				//									d.createdate BETWEEN '{startTime}' 
				//									AND '{endTime}'  
				//									AND e.ranking <= 5 
				//									{HisWhereSQL}  {INSPECTION_NAMESTR}
				//								GROUP BY
				//									a.createdate 
				//									) SELECT
				//									createdate,
				//									pass_qty,
				//									bad_qty,
				//									round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) pass_rate 
				//								FROM
				//									rqc_data 
				//								ORDER BY
				//									createdate";

				#endregion

				string HisTop5FailDataSQL = $@"with rqc_data AS (
													SELECT
														a.createdate,
													sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
													sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
												FROM
													rqc_task_m a
													inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
													inner JOIN rqc_task_detail_t_d b ON d.TASK_NO = b.TASK_NO 
													AND d.COMMIT_INDEX = b.COMMIT_INDEX
													inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
													AND b.UNION_ID = c.ID
													{JoinSql}
												WHERE
													a.createdate BETWEEN '{startTime}' 
													AND '{endTime}' 
													{HisWhereSQL}  {INSPECTION_NAMESTR}
												GROUP BY
													a.createdate 
													) SELECT
													createdate,
													pass_qty,
													bad_qty,
													round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) pass_rate 
												FROM
													rqc_data 
												ORDER BY
													createdate";
				//RetDic["HisData"] = Common.RetunHisData(DB, dayList, HisTop5FailDataSQL, "createdate", "pass_qty", "bad_qty", "pass_rate", "RQC抽验合格数", "RQC抽验不合格数", "合格率");
				RetDic["HisData"] = Common.RetunHisData(DB, dayList, HisTop5FailDataSQL, "createdate", "pass_qty", "bad_qty", "pass_rate", "RQC random inspection qualified number", "RQC random inspection failed number", "pass rate");

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
		/// 生产线数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		//public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionLineData(object OBJ)
		//{
		//	SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
		//	SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
		//	SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
		//	try
		//	{
		//		DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
		//		string Data = ReqObj.Data.ToString();
		//		var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


		//		#region 查询条件
		//		string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "false";//

		//		string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
		//		string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
		//		string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
		//		string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
		//		string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
		//		string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
		//		string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
		//		string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
		//		string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
		//		string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
		//		string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
		//		string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
		//		string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
		//		#endregion

		//		if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
		//		{
		//			throw new Exception("必填条件【开始时间】【结束时间】不能为空！");
		//		}

		//              #region 根据查询字段动态组合查询

		//              List<string> dayList = Common.ReturnData("d", startTime, endTime);

		//		string WhereStr1 = string.Empty;//条件1
		//		string WhereStr2 = string.Empty;//条件2

		//		Dictionary<string, object> RetDic = new Dictionary<string, object>();
		//		RetDic["Top5DT"] = null;
		//		RetDic["Last5DT"] = null;
		//		string conditionStr = "workshop,productionLine,machine,mold_no";
		//		List<string> conditionList = new List<string>();
		//              foreach (var item in jarr)
		//              {
		//                  if (conditionStr.Contains(item.Key))
		//                  {
		//                      if (!string.IsNullOrEmpty(item.Value.ToString()))
		//                      {
		//					conditionList.Add(item.Key);
		//				}
		//			}
		//              }
		//		string CaseStr = string.Empty;

		//		if (conditionList.Contains("workshop"))
		//              {
		//			CaseStr = "workshop";
		//		}
		//		if (conditionList.Contains("productionLine"))
		//		{
		//			CaseStr = "productionLine";
		//		}
		//		if (conditionList.Contains("machine"))
		//		{
		//			CaseStr = "machine";
		//		}
		//		if (conditionList.Contains("mold_no"))
		//		{
		//			CaseStr = "mold_no";
		//		}

		//		Dictionary<string, object> QueryDic = new Dictionary<string, object>();
		//		QueryDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//								e.WORKSHOP_SECTION_NAME,--工段
		//								a.PRODUCTION_LINE_CODE,
		//								b.PRODUCTION_LINE_NAME,--产线
		//								g.EQ_INFO_NO,
		//								g.EQ_INFO_NAME,--机台
		//								a.mold_no,--模号";
		//		QueryDic["str2"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								--以下展示字段需要根据传入参数动态拼接
		//								NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
		//								nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
		//								nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
		//								nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
		//								nvl( mold_no, 'NULL' ) mold_no,--模号";
		//		QueryDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//								g.WORKSHOP_SECTION_NAME,--工段
		//						        --以下展示字段需要根据传入参数动态拼接
		//								e.PRODUCTION_LINE_CODE,
		//								e.PRODUCTION_LINE_NAME,--产线
		//								i.EQ_INFO_NO,
		//								i.EQ_INFO_NAME,--机台
		//								d.mold_no,--模号";
		//		QueryDic["str4"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
		//								nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
		//								nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
		//								nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
		//								nvl( mold_no, 'NULL' ) mold_no,--模号";
		//		QueryDic["str5"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME,--产线
		//								EQ_INFO_NO,
		//								EQ_INFO_NAME,--机台
		//								mold_no,--模号";
		//		QueryDic["str6"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME,--产线
		//								EQ_INFO_NO,
		//								EQ_INFO_NAME,--机台
		//								mold_no,--模号";

		//		Dictionary<string, object> GroupDic = new Dictionary<string, object>();
		//		GroupDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//								e.WORKSHOP_SECTION_NAME,
		//								a.PRODUCTION_LINE_CODE,
		//								b.PRODUCTION_LINE_NAME,
		//								g.EQ_INFO_NO,
		//								g.EQ_INFO_NAME,
		//								a.mold_no";
		//		GroupDic["str2"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME,--产线
		//								EQ_INFO_NO,
		//								EQ_INFO_NAME,--机台
		//								mold_no --模号";
		//		GroupDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//								g.WORKSHOP_SECTION_NAME,
		//								e.PRODUCTION_LINE_CODE,
		//								e.PRODUCTION_LINE_NAME,
		//								i.EQ_INFO_NO,
		//								i.EQ_INFO_NAME,
		//								d.mold_no";
		//		GroupDic["str4"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME,
		//								EQ_INFO_NO,
		//								EQ_INFO_NAME,
		//								mold_no ";
		//		GroupDic["str5"] = @"ORG_NAME, PLANT_AREA, WORKSHOP_SECTION_NO, PRODUCTION_LINE_CODE, EQ_INFO_NO, mold_no ";
		//		GroupDic["str6"] = @"AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE 
		//								AND a.EQ_INFO_NO = b.EQ_INFO_NO 
		//								AND a.mold_no = b.mold_no ";
		//		GroupDic["str7"] = @"AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE 
		//								AND a.EQ_INFO_NO = c.EQ_INFO_NO 
		//								AND a.mold_no = c.mold_no  ";
		//		GroupDic["str8"] = @"AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE 
		//								AND a.EQ_INFO_NO = d.EQ_INFO_NO 
		//								AND a.mold_no = d.mold_no ";

		//		if (!string.IsNullOrEmpty(CaseStr))
		//              {
		//                  switch (CaseStr)
		//                  {
		//				case "workshop":
		//					QueryDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//											e.WORKSHOP_SECTION_NAME,--工段
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str1"] = @"e.WORKSHOP_SECTION_NO,e.WORKSHOP_SECTION_NAME";
		//					QueryDic["str2"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str2"] = @"WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME";
		//					QueryDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//											g.WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str3"] = @"g.WORKSHOP_SECTION_NO,g.WORKSHOP_SECTION_NAME";
		//					QueryDic["str4"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str4"] = @"WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME";
		//					QueryDic["str5"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str5"] = @"ORG_NAME, PLANT_AREA, WORKSHOP_SECTION_NO";
		//					QueryDic["str6"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											'' PRODUCTION_LINE_CODE,
		//											'' PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					GroupDic["str6"] = @"AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO";
		//					GroupDic["str7"] = @"AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO ";
		//					GroupDic["str8"] = @"AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO";
		//					break;
		//				case "productionLine":
		//					QueryDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//											e.WORKSHOP_SECTION_NAME,--工段
		//											a.PRODUCTION_LINE_CODE,
		//											b.PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str2"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											NVL(PRODUCTION_LINE_CODE, 'NULL') PRODUCTION_LINE_CODE,
		//											nvl(PRODUCTION_LINE_NAME, 'NULL') PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//											g.WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											e.PRODUCTION_LINE_CODE,
		//											e.PRODUCTION_LINE_NAME,--产线
		//											EQ_INFO_NO,
		//											EQ_INFO_NAME,--机台
		//											mold_no,--模号";
		//					QueryDic["str4"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
		//											nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str5"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str6"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,--产线
		//											'' EQ_INFO_NO,
		//											'' EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";

		//					GroupDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//								e.WORKSHOP_SECTION_NAME,
		//								a.PRODUCTION_LINE_CODE,
		//								b.PRODUCTION_LINE_NAME";
		//					GroupDic["str2"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,--工段
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME";
		//					GroupDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//								g.WORKSHOP_SECTION_NAME,
		//								e.PRODUCTION_LINE_CODE,
		//								e.PRODUCTION_LINE_NAME";
		//					GroupDic["str4"] = @"WORKSHOP_SECTION_NO,
		//								WORKSHOP_SECTION_NAME,
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME ";
		//					GroupDic["str5"] = @"ORG_NAME, PLANT_AREA, WORKSHOP_SECTION_NO, PRODUCTION_LINE_CODE";
		//					GroupDic["str6"] = @"AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE ";
		//					GroupDic["str7"] = @"AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE ";
		//					GroupDic["str8"] = @"AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
		//								AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE ";
		//					break;
		//				case "machine":
		//					QueryDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//											e.WORKSHOP_SECTION_NAME,--工段
		//											a.PRODUCTION_LINE_CODE,
		//											b.PRODUCTION_LINE_NAME,--产线
		//											g.EQ_INFO_NO,
		//											g.EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str2"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											NVL(PRODUCTION_LINE_CODE, 'NULL') PRODUCTION_LINE_CODE,
		//											nvl(PRODUCTION_LINE_NAME, 'NULL') PRODUCTION_LINE_NAME,--产线
		//											nvl(EQ_INFO_NO, 'NULL') EQ_INFO_NO,
		//											nvl(EQ_INFO_NAME, 'NULL') EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//											g.WORKSHOP_SECTION_NAME,--工段
		//											--以下展示字段需要根据传入参数动态拼接
		//											e.PRODUCTION_LINE_CODE,
		//											e.PRODUCTION_LINE_NAME,--产线
		//											i.EQ_INFO_NO,
		//											i.EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str4"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
		//											nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
		//											nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
		//											nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str5"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,--产线
		//											EQ_INFO_NO,
		//											EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";
		//					QueryDic["str6"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,--产线
		//											EQ_INFO_NO,
		//											EQ_INFO_NAME,--机台
		//											'' mold_no,--模号";

		//					GroupDic["str1"] = @"e.WORKSHOP_SECTION_NO,
		//											e.WORKSHOP_SECTION_NAME,
		//											a.PRODUCTION_LINE_CODE,
		//											b.PRODUCTION_LINE_NAME,
		//											g.EQ_INFO_NO,
		//											g.EQ_INFO_NAME";
		//					GroupDic["str2"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,--工段
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,--产线
		//											EQ_INFO_NO,
		//											EQ_INFO_NAME";
		//					GroupDic["str3"] = @"g.WORKSHOP_SECTION_NO,
		//											g.WORKSHOP_SECTION_NAME,
		//											e.PRODUCTION_LINE_CODE,
		//											e.PRODUCTION_LINE_NAME,
		//											i.EQ_INFO_NO,
		//											i.EQ_INFO_NAME";
		//					GroupDic["str4"] = @"WORKSHOP_SECTION_NO,
		//											WORKSHOP_SECTION_NAME,
		//											PRODUCTION_LINE_CODE,
		//											PRODUCTION_LINE_NAME,
		//											EQ_INFO_NO,
		//											EQ_INFO_NAME";
		//					GroupDic["str5"] = @"ORG_NAME, PLANT_AREA, WORKSHOP_SECTION_NO, PRODUCTION_LINE_CODE, EQ_INFO_NO";
		//					GroupDic["str6"] = @"AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		//											AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE 
		//											AND a.EQ_INFO_NO = b.EQ_INFO_NO ";
		//					GroupDic["str7"] = @"AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
		//											AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE 
		//											AND a.EQ_INFO_NO = c.EQ_INFO_NO ";
		//					GroupDic["str8"] = @"AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
		//											AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE 
		//											AND a.EQ_INFO_NO = d.EQ_INFO_NO ";
		//					break;
		//				case "mold_no":
		//					break;
		//			}
		//              }

		//		#endregion

		//		#region 组合条件
		//		string[] arr = new string[] { };
		//		if (!string.IsNullOrEmpty(plantType))
		//		{
		//			arr = plantType.Split(',');
		//			WhereStr1 += $@"and B.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and e.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND B.ORG_NAME LIKE '{plantType}%'";
		//			//WhereStr2 += $@" AND e.ORG_NAME LIKE '{plantType}%'";
		//		}
		//		if (!string.IsNullOrEmpty(plant))
		//		{
		//                  arr = plant.Split(',');
		//                  WhereStr1 += $@"and B.PLANT_AREA in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and e.PLANT_AREA in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND B.PLANT_AREA ='{plant}'";
		//			//WhereStr2 += $@" AND E.PLANT_AREA ='{plant}'";
		//		}
		//		if (!string.IsNullOrEmpty(workshop))
		//		{
		//                  arr = workshop.Split(',');
		//                  WhereStr1 += $@"and e.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and g.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND E.WORKSHOP_SECTION_NAME like '{workshop}%'";
		//			//WhereStr2 += $@" AND G.WORKSHOP_SECTION_NAME like '{workshop}%'";
		//		}
		//		if (!string.IsNullOrEmpty(depart))
		//		{
		//                  arr = depart.Split(',');
		//                  WhereStr1 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND a.DEPARTMENT LIKE '{depart}%'";
		//			//WhereStr2 += $@" AND D.DEPARTMENT LIKE '{depart}%'";
		//		}
		//		if (!string.IsNullOrEmpty(productionLine))
		//		{
		//                  arr = productionLine.Split(',');
		//                  WhereStr1 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND (b.PRODUCTION_LINE_CODE like '{productionLine}%' OR b.PRODUCTION_LINE_NAME like '{productionLine}%')";
		//			//WhereStr2 += $@" AND (E.PRODUCTION_LINE_CODE like '{productionLine}%' OR E.PRODUCTION_LINE_NAME like '{productionLine}%')";
		//		}
		//		if (!string.IsNullOrEmpty(art))
		//		{
		//                  arr = art.Split(',');
		//                  WhereStr1 += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and D.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND a.PROD_NO ='{art}'";
		//			//WhereStr2 += $@" AND D.PROD_NO ='{art}'";
		//		}
		//		if (!string.IsNullOrEmpty(sheoName))
		//		{
		//                  arr = sheoName.Split(',');
		//                  WhereStr1 += $@"and a.SHOE_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and D.SHOE_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND a.SHOE_NO like '{sheoName}%'";
		//			//WhereStr2 += $@" AND D.SHOE_NO like '{sheoName}%'";
		//		}
		//		if (!string.IsNullOrEmpty(technology))
		//		{
		//                  WhereStr1 += $@" AND a.CONFIG_NO like '{technology}%'";
		//			WhereStr2 += $@" AND D.CONFIG_NO like '{technology}%'";
		//		}
		//		if (!string.IsNullOrEmpty(machine))
		//		{
		//                  arr = machine.Split(',');
		//                  WhereStr1 += $@" AND a.EQ_INFO_NO  like '{machine}%'";
		//			WhereStr2 += $@" AND D.EQ_INFO_NO  like '{machine}%'";
		//		}
		//		if (!string.IsNullOrEmpty(mold_no))
		//		{
		//                  WhereStr1 += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
		//			WhereStr2 += $@" AND D.MOLD_NO  LIKE '{mold_no}%'";
		//		}
		//		if (!string.IsNullOrEmpty(po))
		//		{
		//                  arr = po.Split(',');
		//                  WhereStr1 += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
		//			WhereStr2 += $@"and D.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";

		//			//WhereStr1 += $@" AND a.MER_PO='{po}'";
		//			//WhereStr2 += $@" AND D.MER_PO='{po}'";
		//		}
		//		#endregion

		//		string DataSQL = $@"WITH depart_tmp AS (
		//							SELECT
		//								c.ORG_NAME,
		//								PRODUCTION_LINE_CODE,
		//								PRODUCTION_LINE_NAME,
		//								PLANT_AREA 
		//							FROM
		//								bdm_production_line_m a
		//								LEFT JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
		//								LEFT JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE UNION ALL
		//							SELECT
		//								c.ORG_NAME,
		//								DEPARTMENT_CODE,
		//								DEPARTMENT_NAME,
		//								org 
		//							FROM
		//								base005m a
		//								LEFT JOIN SJQDMS_ORGINFO b ON a.udf05 = b.code
		//								LEFT JOIN base001m c ON a.FACTORY_SAP = c.ORG_CODE 
		//							),
		//							rqc_data_tmp AS (
		//							SELECT
		//								a.TASK_NO,
		//								b.ORG_NAME,
		//								b.PLANT_AREA,--厂区
		//						        --以下展示字段需要根据传入参数动态拼接
		//								{QueryDic["str1"].ToString()}
		//							sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
		//							sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
		//						FROM
		//							rqc_task_m a
		//							JOIN depart_tmp b ON a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE
		//							JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
		//							LEFT JOIN qcm_dqa_mag_m e ON a.SHOE_NO = e.SHOES_CODE 
		//							AND e.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
		//							LEFT JOIN bdm_eq_info_m g ON a.EQ_INFO_NO = g.EQ_INFO_NO 
		//							LEFT JOIN BASE005M bm on bm.DEPARTMENT_CODE=a.DEPARTMENT
		//						WHERE
		//							a.createdate BETWEEN '{startTime}' 
		//							AND '{endTime}'  {WhereStr1} --写条件
		//						--以下group by字段需要根据传入参数动态拼接

		//						GROUP BY
		//							a.TASK_NO,
		//							b.ORG_NAME,
		//							b.PLANT_AREA,
		//							{GroupDic["str1"].ToString()}
		//							),
		//							rqc_data AS (
		//							SELECT
		//								ORG_NAME,
		//								PLANT_AREA,
		//								{QueryDic["str2"].ToString()}
		//								sum( pass_qty ) pass_qty,
		//								sum( bad_qty ) bad_qty,
		//								( sum( pass_qty ) + sum( bad_qty ) ) total_qty,
		//								round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 2 ) pass_rate 
		//							FROM
		//								rqc_data_tmp --以下group by字段需要根据传入参数动态拼接

		//							GROUP BY
		//								ORG_NAME,
		//								PLANT_AREA,
		//								{GroupDic["str2"].ToString()}
		//							),
		//							rqc_bad_data_tmp AS (
		//							SELECT
		//								c.INSPECTION_NAME,
		//								e.ORG_NAME,
		//								e.PLANT_AREA,--厂区
		//								{QueryDic["str3"].ToString()}
		//							sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
		//							sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
		//						FROM
		//							rqc_task_detail_t a
		//							JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
		//							AND a.COMMIT_INDEX = b.COMMIT_INDEX
		//							JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
		//							AND b.UNION_ID = c.ID
		//							JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO
		//							JOIN depart_tmp e ON d.PRODUCTION_LINE_CODE = e.PRODUCTION_LINE_CODE
		//							LEFT JOIN qcm_dqa_mag_m g ON d.SHOE_NO = g.SHOES_CODE 
		//							AND g.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO
		//							LEFT JOIN bdm_eq_info_m i ON d.EQ_INFO_NO = i.EQ_INFO_NO
		//							JOIN rqc_data_tmp j ON d.TASK_NO = j.task_no
		//							JOIN BASE005M bm on bm.DEPARTMENT_CODE=d.DEPARTMENT
		//						where a.CREATEDATE between '{startTime}' and '{endTime}'
		//						{WhereStr2}
		//						--写条件
		//						--以下group by字段需要根据传入参数动态拼接

		//						GROUP BY
		//							c.INSPECTION_NAME,
		//							e.ORG_NAME,
		//							e.PLANT_AREA,
		//							{GroupDic["str3"].ToString()}
		//							),
		//							rqc_bad_data_tmp2 AS (--以下展示字段需要根据传入参数动态拼接
		//							SELECT
		//								INSPECTION_NAME,
		//								ORG_NAME,
		//								PLANT_AREA,--厂区
		//								{QueryDic["str4"].ToString()}
		//								sum( pass_qty ) pass_qty,
		//								sum( bad_qty ) bad_qty --round(sum(pass_qty)/(sum(pass_qty)+sum(bad_qty))*100,2) pass_rate

		//							FROM
		//								rqc_bad_data_tmp 
		//							GROUP BY
		//								INSPECTION_NAME,
		//								ORG_NAME,
		//								PLANT_AREA,
		//								{GroupDic["str4"].ToString()}
		//							),
		//							rqc_bad_data_tmp3 AS (--以下展示字段需要根据传入参数动态拼接
		//							SELECT
		//								INSPECTION_NAME,
		//								ORG_NAME,
		//								PLANT_AREA,--厂区
		//								{QueryDic["str5"].ToString()}
		//								pass_qty,
		//								bad_qty,
		//						--pass_rate,
		//						--以下partition by 内容需要根据传入参数动态拼接
		//								sum( bad_qty ) over ( partition BY {GroupDic["str5"].ToString()} ) total_qty,
		//								ROW_NUMBER ( ) over ( partition BY {GroupDic["str5"].ToString()} ORDER BY bad_qty DESC, pass_qty + bad_qty DESC ) ranking 
		//							FROM
		//								rqc_bad_data_tmp2 
		//							),
		//							rqc_bad_data AS (--以下展示字段需要根据传入参数动态拼接
		//							SELECT
		//								INSPECTION_NAME,
		//								ORG_NAME,
		//								PLANT_AREA,--厂区
		//								{QueryDic["str6"].ToString()}
		//								pass_qty,
		//								bad_qty,
		//								round( bad_qty / total_qty * 100, 2 ) fail_rate,
		//								total_qty,
		//								ranking 
		//							FROM
		//								rqc_bad_data_tmp3 
		//							) SELECT
		//							a.* ,
		//							b.INSPECTION_NAME INSPECTION_NAME_1,
		//							b.fail_rate fail_rate_1,
		//							c.INSPECTION_NAME INSPECTION_NAME_2,
		//							c.fail_rate fail_rate_2,
		//							d.INSPECTION_NAME INSPECTION_NAME_3,
		//							d.fail_rate fail_rate_3 
		//						FROM
		//							rqc_data a --以下条件需要根据传入参数动态拼接
		//							LEFT JOIN rqc_bad_data b ON a.PLANT_AREA = b.PLANT_AREA 
		//							{GroupDic["str6"].ToString()}
		//							AND b.ranking = 1
		//							LEFT JOIN rqc_bad_data c ON a.PLANT_AREA = c.PLANT_AREA 
		//							{GroupDic["str7"].ToString()}
		//							AND c.ranking = 2
		//							LEFT JOIN rqc_bad_data d ON a.PLANT_AREA = d.PLANT_AREA 
		//							{GroupDic["str8"].ToString()}
		//							AND d.ranking =3";
		//		DataTable AllDt = DB.GetDataTable(DataSQL);
		//              foreach (DataRow item in AllDt.Rows)
		//              {
		//			item["EQ_INFO_NAME"] = item["EQ_INFO_NAME"].ToString().Replace("NULL", "");
		//			item["PRODUCTION_LINE_NAME"] = item["PRODUCTION_LINE_NAME"].ToString().Replace("NULL", "");
		//			item["PRODUCTION_LINE_CODE"] = item["PRODUCTION_LINE_CODE"].ToString().Replace("NULL", "");
		//			item["EQ_INFO_NO"] = item["EQ_INFO_NO"].ToString().Replace("NULL", "");
		//			item["MOLD_NO"] = item["MOLD_NO"].ToString().Replace("NULL", "");
		//              }

		//		List<string> TopList = new List<string>() { "PASS_RATE Desc", "total_qty Desc" };
		//		List<string> LastList = new List<string>() { "PASS_RATE asc", "total_qty asc" };

		//		RetDic["Top5DT"] = Common.ReturnDTOrderByRows(AllDt, TopList, 20, bool.Parse(IsExport));
		//		if (!bool.Parse(IsExport))
		//              {
		//			RetDic["Last5DT"] = Common.ReturnDTOrderByRows(AllDt, LastList, 20);
		//		}



		//		ret.RetData1 = RetDic;
		//		ret.IsSuccess = true;
		//	}
		//	catch (Exception ex)
		//	{
		//		ret.IsSuccess = false;
		//		ret.ErrMsg = ex.Message;
		//	}
		//	return ret;
		//}     //Commented by Ashok //Commented by Ashok


		/// <summary>
		/// 生产线数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionLineData(object OBJ)
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
				string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "false";//

				string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
				string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
				string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
				string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
				string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
				string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("Required conditions [Start Time] [End Time] cannot be empty!");
				}

				#region Dynamically combine queries based on query fields

				List<string> dayList = Common.ReturnData("d", startTime, endTime);

				string WhereStr1 = string.Empty;//条件1
				string WhereStr2 = string.Empty;//条件2

				Dictionary<string, object> RetDic = new Dictionary<string, object>();
				RetDic["Top5DT"] = null;
				RetDic["Last5DT"] = null;
				string conditionStr = "workshop,productionLine,machine,mold_no";
				List<string> conditionList = new List<string>();
				foreach (var item in jarr)
				{
					if (conditionStr.Contains(item.Key))
					{
						if (!string.IsNullOrEmpty(item.Value.ToString()))
						{
							conditionList.Add(item.Key);
						}
					}
				}
				string CaseStr = string.Empty;

				if (conditionList.Contains("workshop"))
				{
					CaseStr = "workshop";
				}
				if (conditionList.Contains("productionLine"))
				{
					CaseStr = "productionLine";
				}
				if (conditionList.Contains("machine"))
				{
					CaseStr = "machine";
				}
				if (conditionList.Contains("mold_no"))
				{
					CaseStr = "mold_no";
				}

				Dictionary<string, object> QueryDic = new Dictionary<string, object>();
				QueryDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
										--e.WORKSHOP_SECTION_NAME,--工段
										a.PRODUCTION_LINE_CODE,
										b.PRODUCTION_LINE_NAME,--产线
										--g.EQ_INFO_NO,
										--g.EQ_INFO_NAME,--机台
										a.mold_no,--模号";
				QueryDic["str2"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										--以下展示字段需要根据传入参数动态拼接
										NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
										nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
										--nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
										--nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
										nvl( mold_no, 'NULL' ) mold_no,--模号";
				QueryDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
										--g.WORKSHOP_SECTION_NAME,--工段
								        --以下展示字段需要根据传入参数动态拼接
										e.PRODUCTION_LINE_CODE,
										e.PRODUCTION_LINE_NAME,--产线
										--i.EQ_INFO_NO,
										--i.EQ_INFO_NAME,--机台
										d.mold_no,--模号";
				QueryDic["str4"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
										nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
										--nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
										--nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
										nvl( mold_no, 'NULL' ) mold_no,--模号";
				QueryDic["str5"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME,--产线
										--EQ_INFO_NO,
										--EQ_INFO_NAME,--机台
										mold_no,--模号";
				QueryDic["str6"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME,--产线
										--EQ_INFO_NO,
										--EQ_INFO_NAME,--机台
										mold_no,--模号";

				Dictionary<string, object> GroupDic = new Dictionary<string, object>();
				GroupDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
										--e.WORKSHOP_SECTION_NAME,
										a.PRODUCTION_LINE_CODE,
										b.PRODUCTION_LINE_NAME,
										--g.EQ_INFO_NO,
										--g.EQ_INFO_NAME,
										a.mold_no";
				GroupDic["str2"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME,--产线
										--EQ_INFO_NO,
										--EQ_INFO_NAME,--机台
										mold_no --模号";
				GroupDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
										--g.WORKSHOP_SECTION_NAME,
										e.PRODUCTION_LINE_CODE,
										e.PRODUCTION_LINE_NAME,
										--i.EQ_INFO_NO,
										--i.EQ_INFO_NAME,
										d.mold_no";
				GroupDic["str4"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME,
										--EQ_INFO_NO,
										--EQ_INFO_NAME,
										mold_no ";
				GroupDic["str5"] = @"ORG_NAME,
PLANT_AREA,
--WORKSHOP_SECTION_NO,
PRODUCTION_LINE_CODE,
--EQ_INFO_NO, 
mold_no ";
				GroupDic["str6"] = @"--AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE 
										--AND a.EQ_INFO_NO = b.EQ_INFO_NO 
										AND a.mold_no = b.mold_no ";
				GroupDic["str7"] = @"--AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE 
										--AND a.EQ_INFO_NO = c.EQ_INFO_NO 
										AND a.mold_no = c.mold_no  ";
				GroupDic["str8"] = @"--AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE 
										--AND a.EQ_INFO_NO = d.EQ_INFO_NO 
										AND a.mold_no = d.mold_no ";

				if (!string.IsNullOrEmpty(CaseStr))
				{
					switch (CaseStr)
					{
						case "workshop":
							QueryDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
													--e.WORKSHOP_SECTION_NAME,--工段
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str1"] = @"--e.WORKSHOP_SECTION_NO,e.WORKSHOP_SECTION_NAME";
							QueryDic["str2"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str2"] = @"--WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME";
							QueryDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
													--g.WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str3"] = @"--g.WORKSHOP_SECTION_NO,g.WORKSHOP_SECTION_NAME";
							QueryDic["str4"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str4"] = @"--WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME";
							QueryDic["str5"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str5"] = @"ORG_NAME, PLANT_AREA,-- WORKSHOP_SECTION_NO";
							QueryDic["str6"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													'' PRODUCTION_LINE_CODE,
													'' PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							GroupDic["str6"] = @"--AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO";
							GroupDic["str7"] = @"--AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO ";
							GroupDic["str8"] = @"--AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO";
							break;
						case "productionLine":
							QueryDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
													--e.WORKSHOP_SECTION_NAME,--工段
													a.PRODUCTION_LINE_CODE,
													b.PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str2"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													NVL(PRODUCTION_LINE_CODE, 'NULL') PRODUCTION_LINE_CODE,
													nvl(PRODUCTION_LINE_NAME, 'NULL') PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
													--g.WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													e.PRODUCTION_LINE_CODE,
													e.PRODUCTION_LINE_NAME,--产线
													--EQ_INFO_NO,
													--EQ_INFO_NAME,--机台
													mold_no,--模号";
							QueryDic["str4"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
													nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str5"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str6"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,--产线
													--'' EQ_INFO_NO,
													--'' EQ_INFO_NAME,--机台
													'' mold_no,--模号";

							GroupDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
										--e.WORKSHOP_SECTION_NAME,
										a.PRODUCTION_LINE_CODE,
										b.PRODUCTION_LINE_NAME";
							GroupDic["str2"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,--工段
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME";
							GroupDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
										--g.WORKSHOP_SECTION_NAME,
										e.PRODUCTION_LINE_CODE,
										e.PRODUCTION_LINE_NAME";
							GroupDic["str4"] = @"--WORKSHOP_SECTION_NO,
										--WORKSHOP_SECTION_NAME,
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME ";
							GroupDic["str5"] = @"ORG_NAME, PLANT_AREA, 
--WORKSHOP_SECTION_NO, 
PRODUCTION_LINE_CODE";
							GroupDic["str6"] = @"--AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE ";
							GroupDic["str7"] = @"--AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE ";
							GroupDic["str8"] = @"--AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
										AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE ";
							break;
						case "machine":
							QueryDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
													--e.WORKSHOP_SECTION_NAME,--工段
													a.PRODUCTION_LINE_CODE,
													b.PRODUCTION_LINE_NAME,--产线
													--g.EQ_INFO_NO,
													--g.EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str2"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													NVL(PRODUCTION_LINE_CODE, 'NULL') PRODUCTION_LINE_CODE,
													nvl(PRODUCTION_LINE_NAME, 'NULL') PRODUCTION_LINE_NAME,--产线
													--nvl(EQ_INFO_NO, 'NULL') EQ_INFO_NO,
													--nvl(EQ_INFO_NAME, 'NULL') EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
													--g.WORKSHOP_SECTION_NAME,--工段
													--以下展示字段需要根据传入参数动态拼接
													e.PRODUCTION_LINE_CODE,
													e.PRODUCTION_LINE_NAME,--产线
													--i.EQ_INFO_NO,
													--i.EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str4"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													NVL( PRODUCTION_LINE_CODE, 'NULL' ) PRODUCTION_LINE_CODE,
													nvl( PRODUCTION_LINE_NAME, 'NULL' ) PRODUCTION_LINE_NAME,--产线
													--nvl( EQ_INFO_NO, 'NULL' ) EQ_INFO_NO,
													--nvl( EQ_INFO_NAME, 'NULL' ) EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str5"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,--产线
													--EQ_INFO_NO,
													--EQ_INFO_NAME,--机台
													'' mold_no,--模号";
							QueryDic["str6"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,--产线
													--EQ_INFO_NO,
													--EQ_INFO_NAME,--机台
													'' mold_no,--模号";

							GroupDic["str1"] = @"--e.WORKSHOP_SECTION_NO,
													--e.WORKSHOP_SECTION_NAME,
													a.PRODUCTION_LINE_CODE,
													b.PRODUCTION_LINE_NAME,
													--g.EQ_INFO_NO,
													--g.EQ_INFO_NAME";
							GroupDic["str2"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,--工段
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,--产线
													--EQ_INFO_NO,
													--EQ_INFO_NAME";
							GroupDic["str3"] = @"--g.WORKSHOP_SECTION_NO,
													--g.WORKSHOP_SECTION_NAME,
													e.PRODUCTION_LINE_CODE,
													e.PRODUCTION_LINE_NAME,
													--i.EQ_INFO_NO,
													--i.EQ_INFO_NAME";
							GroupDic["str4"] = @"--WORKSHOP_SECTION_NO,
													--WORKSHOP_SECTION_NAME,
													PRODUCTION_LINE_CODE,
													PRODUCTION_LINE_NAME,
													--EQ_INFO_NO,
													--EQ_INFO_NAME";
							GroupDic["str5"] = @"ORG_NAME, PLANT_AREA,
--WORKSHOP_SECTION_NO,
PRODUCTION_LINE_CODE,
--EQ_INFO_NO";
							GroupDic["str6"] = @"--AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
													AND a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE 
													--AND a.EQ_INFO_NO = b.EQ_INFO_NO ";
							GroupDic["str7"] = @"--AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
													AND a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE 
													--AND a.EQ_INFO_NO = c.EQ_INFO_NO ";
							GroupDic["str8"] = @"--AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
													AND a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE 
													--AND a.EQ_INFO_NO = d.EQ_INFO_NO ";
							break;
						case "mold_no":
							break;
					}
				}

				#endregion

				#region 组合条件
				string[] arr = new string[] { };

                #region Commented by Ashok to query the data queckly on 2025/05/10
    //            if (!string.IsNullOrEmpty(plantType))
				//{
				//	arr = plantType.Split(',');
				//	WhereStr1 += $@"and B.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and e.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(workshop))
				//{
				//	arr = workshop.Split(',');
				//	WhereStr1 += $@"and e.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and g.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(depart))
				//{
				//	arr = depart.Split(',');
				//	WhereStr1 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(technology))
				//{
				//	WhereStr1 += $@" AND a.CONFIG_NO like '{technology}%'";
				//	WhereStr2 += $@" AND D.CONFIG_NO like '{technology}%'";
				//}
				//if (!string.IsNullOrEmpty(machine))
				//{
				//	arr = machine.Split(',');
				//	WhereStr1 += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	WhereStr2 += $@" AND D.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))
				//{
				//	WhereStr1 += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	WhereStr2 += $@" AND D.MOLD_NO  LIKE '{mold_no}%'";
				//}
                #endregion

                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
					WhereStr1 += $@"and B.PLANT_AREA in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and e.PLANT_AREA in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					WhereStr1 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and bm.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					WhereStr1 += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and D.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					WhereStr1 += $@"and x.name_t in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and x.name_t in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					WhereStr1 += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and D.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				#endregion

				string DataSQL = $@"WITH depart_tmp AS (
									SELECT
										c.ORG_NAME,
										PRODUCTION_LINE_CODE,
										PRODUCTION_LINE_NAME,
										PLANT_AREA 
									FROM
										bdm_production_line_m a
										inner JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
										inner JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE UNION ALL
									SELECT
										c.ORG_NAME,
										DEPARTMENT_CODE,
										DEPARTMENT_NAME,
										org 
									FROM
										base005m a
										inner JOIN SJQDMS_ORGINFO b ON a.udf05 = b.code
										inner JOIN base001m c ON a.FACTORY_SAP = c.ORG_CODE 
									),
									rqc_data_tmp AS (
									SELECT
										a.TASK_NO,
										b.ORG_NAME,
										b.PLANT_AREA,
										{QueryDic["str1"].ToString()}
									sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
									sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
								FROM
									rqc_task_m a
									inner JOIN depart_tmp b ON a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE
									inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
									--LEFT JOIN qcm_dqa_mag_m e ON a.SHOE_NO = e.SHOES_CODE 
									--AND e.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
									--LEFT JOIN bdm_eq_info_m g ON a.EQ_INFO_NO = g.EQ_INFO_NO 
									inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=a.DEPARTMENT
                                    inner join bdm_rd_style x on x.shoe_no=a.SHOE_NO  
								WHERE
									a.createdate BETWEEN '{startTime}' 
									AND '{endTime}'  {WhereStr1} 
								GROUP BY
									a.TASK_NO,
									b.ORG_NAME,
									b.PLANT_AREA,
									{GroupDic["str1"].ToString()}
									),
									rqc_data AS (
									SELECT
										ORG_NAME,
										PLANT_AREA,
										{QueryDic["str2"].ToString()}
										sum( pass_qty ) pass_qty,
										sum( bad_qty ) bad_qty,
										( sum( pass_qty ) + sum( bad_qty ) ) total_qty,
										round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 2 ) pass_rate 
									FROM
										rqc_data_tmp 
									GROUP BY
										ORG_NAME,
										PLANT_AREA,
										{GroupDic["str2"].ToString()}
									),
									rqc_bad_data_tmp AS (
									SELECT
										c.INSPECTION_NAME,
										e.ORG_NAME,
										e.PLANT_AREA,
										{QueryDic["str3"].ToString()}
									sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
									sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
								FROM
									rqc_task_detail_t a
									inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
									AND a.COMMIT_INDEX = b.COMMIT_INDEX
									inner JOIN rqc_task_item_c c ON b.TASK_NO = c.TASK_NO 
									AND b.UNION_ID = c.ID
									inner JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO
									inner JOIN depart_tmp e ON d.PRODUCTION_LINE_CODE = e.PRODUCTION_LINE_CODE
									--LEFT JOIN qcm_dqa_mag_m g ON d.SHOE_NO = g.SHOES_CODE 
									--AND g.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO
									--LEFT JOIN bdm_eq_info_m i ON d.EQ_INFO_NO = i.EQ_INFO_NO
									inner JOIN rqc_data_tmp j ON d.TASK_NO = j.task_no
									inner JOIN BASE005M bm on bm.DEPARTMENT_NAME=d.DEPARTMENT
                                    inner join bdm_rd_style x on x.shoe_no=d.SHOE_NO  
								where a.CREATEDATE between '{startTime}' and '{endTime}'
								{WhereStr2}
								GROUP BY
									c.INSPECTION_NAME,
									e.ORG_NAME,
									e.PLANT_AREA,
									{GroupDic["str3"].ToString()}
									),
									rqc_bad_data_tmp2 AS (
									SELECT
										INSPECTION_NAME,
										ORG_NAME,
										PLANT_AREA,
										{QueryDic["str4"].ToString()}
										sum( pass_qty ) pass_qty,
										sum( bad_qty ) bad_qty
									FROM
										rqc_bad_data_tmp 
									GROUP BY
										INSPECTION_NAME,
										ORG_NAME,
										PLANT_AREA,
										{GroupDic["str4"].ToString()}
									),
									rqc_bad_data_tmp3 AS (
									SELECT
										INSPECTION_NAME,
										ORG_NAME,
										PLANT_AREA,
										{QueryDic["str5"].ToString()}
										pass_qty,
										bad_qty,
										sum( bad_qty ) over ( partition BY {GroupDic["str5"].ToString()} ) total_qty,
										ROW_NUMBER ( ) over ( partition BY {GroupDic["str5"].ToString()} ORDER BY bad_qty DESC, pass_qty + bad_qty DESC ) ranking 
									FROM
										rqc_bad_data_tmp2 
									),
									rqc_bad_data AS (
									SELECT
										INSPECTION_NAME,
										ORG_NAME,
										PLANT_AREA,
										{QueryDic["str6"].ToString()}
										pass_qty,
										bad_qty,
										 case
           when total_qty = 0 then
            0
           else
            round(bad_qty / total_qty * 100, 2)
         end as fail_rate,
										total_qty,
										ranking 
									FROM
										rqc_bad_data_tmp3 
									) SELECT
									a.* ,
									b.INSPECTION_NAME INSPECTION_NAME_1,
									b.fail_rate fail_rate_1,
									c.INSPECTION_NAME INSPECTION_NAME_2,
									c.fail_rate fail_rate_2,
									d.INSPECTION_NAME INSPECTION_NAME_3,
									d.fail_rate fail_rate_3 
								FROM
									rqc_data a 
									left JOIN rqc_bad_data b ON a.PLANT_AREA = b.PLANT_AREA 
									{GroupDic["str6"].ToString()}
									AND b.ranking = 1
									left JOIN rqc_bad_data c ON a.PLANT_AREA = c.PLANT_AREA 
									{GroupDic["str7"].ToString()}
									AND c.ranking = 2
									left JOIN rqc_bad_data d ON a.PLANT_AREA = d.PLANT_AREA 
									{GroupDic["str8"].ToString()}
									AND d.ranking =3";
				DataTable AllDt = DB.GetDataTable(DataSQL);
				foreach (DataRow item in AllDt.Rows)
				{
					//item["EQ_INFO_NAME"] = item["EQ_INFO_NAME"].ToString().Replace("NULL", "");
					item["PRODUCTION_LINE_NAME"] = item["PRODUCTION_LINE_NAME"].ToString().Replace("NULL", "");
					item["PRODUCTION_LINE_CODE"] = item["PRODUCTION_LINE_CODE"].ToString().Replace("NULL", "");
					//item["EQ_INFO_NO"] = item["EQ_INFO_NO"].ToString().Replace("NULL", "");
					item["MOLD_NO"] = item["MOLD_NO"].ToString().Replace("NULL", "");
				}

				List<string> TopList = new List<string>() { "PASS_RATE Desc", "total_qty Desc" };
				List<string> LastList = new List<string>() { "PASS_RATE asc", "total_qty asc" };

				RetDic["Top5DT"] = Common.ReturnDTOrderByRows(AllDt, TopList, 20, bool.Parse(IsExport));
				if (!bool.Parse(IsExport))
				{
					RetDic["Last5DT"] = Common.ReturnDTOrderByRows(AllDt, LastList, 20);
				}



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
		/// 产品列表数据
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProdutionDetailData(object OBJ)
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
				string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "false";//

				string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂区类型
				string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂区
				string workshop = jarr.ContainsKey("workshop") ? jarr["workshop"].ToString() : "";//工段
				string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//部门
				string productionLine = jarr.ContainsKey("productionLine") ? jarr["productionLine"].ToString() : "";//产线 
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
				string sheoName = jarr.ContainsKey("sheoName") ? jarr["sheoName"].ToString() : "";//鞋型名称
				string technology = jarr.ContainsKey("technology") ? jarr["technology"].ToString() : "";//工艺/材料种类
				string machine = jarr.ContainsKey("machine") ? jarr["machine"].ToString() : "";//机台
				string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//模号
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
				#endregion

				if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
				{
					throw new Exception("Required conditions [Start time] [End time] cannot be empty!");
				}

				List<string> dayList = Common.ReturnData("d", startTime, endTime);

				string WhereStr1 = string.Empty;//条件1
				string WhereStr2 = string.Empty;//条件2

				Dictionary<string, object> RetDic = new Dictionary<string, object>();
				RetDic["Top5DT"] = null;
				RetDic["Last5DT"] = null;
				string[] arr = new string[] { };

                #region commneted by Ashok on 2025/05/09 to query the data queckly
    //            if (!string.IsNullOrEmpty(plantType))
				//{
				//	arr = plantType.Split(',');
				//	WhereStr1 += $@"and c.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and f.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(workshop))
				//{
				//	arr = workshop.Split(',');
				//	WhereStr1 += $@"and e.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and G.WORKSHOP_SECTION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(depart))
				//{
				//	arr = depart.Split(',');
				//	WhereStr1 += $@"and b.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and e.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(technology))
				//{
				//	arr = technology.Split(',');
				//	WhereStr1 += $@"and BD.WORKMANSHIP_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//	WhereStr2 += $@"and BD.WORKMANSHIP_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				//}
				//if (!string.IsNullOrEmpty(machine))
				//{
				//	WhereStr1 += $@" AND a.EQ_INFO_NO  like '{machine}%'";
				//	WhereStr2 += $@" AND D.EQ_INFO_NO  like '{machine}%'";
				//}
				//if (!string.IsNullOrEmpty(mold_no))
				//{
				//	WhereStr1 += $@" AND a.MOLD_NO  LIKE '{mold_no}%'";
				//	WhereStr2 += $@" AND D.MOLD_NO  LIKE '{mold_no}%'";
				//}
                #endregion 

                if (!string.IsNullOrEmpty(plant))
				{
					arr = plant.Split(',');
					WhereStr1 += $@"and sj.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and sj.EN  in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(productionLine))
				{
					arr = productionLine.Split(',');
					WhereStr1 += $@"and b.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and e.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(art))
				{
					arr = art.Split(',');
					WhereStr1 += $@"and a.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and D.PROD_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(sheoName))
				{
					arr = sheoName.Split(',');
					WhereStr1 += $@"and g.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and i.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
				if (!string.IsNullOrEmpty(po))
				{
					arr = po.Split(',');
					WhereStr1 += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
					WhereStr2 += $@"and D.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
				}
		

				string DataSQL = $@"WITH rqc_data_tmp AS (
										SELECT
											a.TASK_NO,
											f.PROD_NO art_no,
											f.NAME_T art_name,
											g.name_t AS Shoe_type_name ,
											c.ORG_CODE,
											c.ORG_NAME,
											e.WORKSHOP_SECTION_NO,
											e.WORKSHOP_SECTION_NAME,
											a.CONFIG_NO,
											BD.WORKMANSHIP_NAME CONFIG_NAME,		
										sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
										sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
									FROM
										rqc_task_m a
										inner JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
										inner JOIN SJQDMS_ORGINFO sj ON b.udf05 = sj.code 
										inner JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE
										inner JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
										inner JOIN qcm_dqa_mag_m e ON a.SHOE_NO = e.SHOES_CODE 
										AND e.WORKSHOP_SECTION_NO = a.WORKSHOP_SECTION_NO
										inner JOIN bdm_rd_prod f ON a.PROD_NO = f.PROD_NO
										inner JOIN bdm_rd_style g ON a.shoe_no = g.shoe_no 
                                        inner JOIN BDM_PARAM_ITEM_D BD ON BD.CONFIG_NO=A.CONFIG_NO
										-- LEFT JOIN BDM_PRODUCTION_LINE_M b1 on b1.PRODUCTION_LINE_CODE=a.PRODUCTION_LINE_CODE
									WHERE
										a.createdate BETWEEN '{startTime}' 
										AND '{endTime}' 
										{WhereStr1}
									GROUP BY
										a.TASK_NO,
										f.PROD_NO,
										f.NAME_T,
										g.name_t,
										c.ORG_CODE,
										c.ORG_NAME,
										e.WORKSHOP_SECTION_NO,
										e.WORKSHOP_SECTION_NAME,
										a.CONFIG_NO,
										BD.WORKMANSHIP_NAME 
										),
										rqc_data AS (
										SELECT
											ORG_CODE,
											ORG_NAME,
											art_no,
											art_name, Shoe_type_name  ,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO,
											CONFIG_NAME,
											sum( pass_qty ) pass_qty,
											sum( bad_qty ) bad_qty,
											( sum( pass_qty ) + sum( bad_qty ) ) total_qty,
											(CASE 
														WHEN ( sum( pass_qty ) + sum( bad_qty) )=0 THEN
															0
														ELSE
															round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 2 )
													END) pass_rate
											-- round( sum( pass_qty ) / ( sum( pass_qty ) + sum( bad_qty ) ) * 100, 2 ) pass_rate 
										FROM
											rqc_data_tmp 
		
										GROUP BY
											ORG_CODE,
											ORG_NAME,
											art_no,
											art_name, Shoe_type_name ,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO, 
											CONFIG_NAME
										),
										rqc_bad_data_tmp AS (
										SELECT
											c.INSPECTION_NAME,
											d.PROD_NO art_no,
											h.NAME_T art_name,
											i.name_t AS Shoe_type_name ,
											f.ORG_CODE,
											f.ORG_NAME,
											g.WORKSHOP_SECTION_NO,
											g.WORKSHOP_SECTION_NAME,
											d.CONFIG_NO,
										sum( CASE WHEN a.commit_type = 0 THEN 1 ELSE 0 END ) pass_qty,
										sum( CASE WHEN a.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
									FROM
										rqc_task_detail_t a
										inner JOIN rqc_task_detail_t_d b ON a.TASK_NO = b.TASK_NO 
										AND a.COMMIT_INDEX = b.COMMIT_INDEX
										inner JOIN rqc_task_item_c c ON a.TASK_NO = c.TASK_NO 
					AND b.UNION_ID = c.ID
										inner JOIN rqc_task_m d ON a.TASK_NO = d.TASK_NO
										inner JOIN base005m e ON d.PRODUCTION_LINE_CODE = e.DEPARTMENT_CODE
										inner JOIN SJQDMS_ORGINFO sj ON e.udf05 = sj.code
										inner JOIN base001m f ON e.FACTORY_SAP = f.ORG_CODE
										inner JOIN qcm_dqa_mag_m g ON d.SHOE_NO = g.SHOES_CODE 
										AND g.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO
										inner JOIN bdm_rd_prod h ON d.PROD_NO = h.PROD_NO
										inner JOIN bdm_rd_style i ON d.shoe_no = i.shoe_no
										inner JOIN BDM_PARAM_ITEM_D BD ON BD.CONFIG_NO=d.CONFIG_NO
										--inner JOIN BDM_PRODUCTION_LINE_M b1 on b1.PRODUCTION_LINE_CODE=d.PRODUCTION_LINE_CODE
										-- JOIN rqc_data_tmp j ON d.TASK_NO = j.task_no 
									WHERE
										d.CREATEDATE BETWEEN '{startTime}' 
										AND '{endTime}' 
										{WhereStr2} 
									GROUP BY
										c.INSPECTION_NAME,
										f.ORG_CODE,
										f.ORG_NAME,
										d.PROD_NO,
										h.NAME_T,
										i.name_t,
										g.WORKSHOP_SECTION_NO,
										g.WORKSHOP_SECTION_NAME,
										d.mold_no,
										d.CONFIG_NO 
										),
										rqc_bad_data_tmp2 AS (
										SELECT
											INSPECTION_NAME,
											art_no,
											art_name, Shoe_type_name ,
											ORG_CODE,
											ORG_NAME,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO,
											sum( pass_qty ) pass_qty,
											sum( bad_qty ) bad_qty 
		
										FROM
											rqc_bad_data_tmp 
										GROUP BY
											INSPECTION_NAME,
											art_no,
											art_name, Shoe_type_name ,
											ORG_CODE,
											ORG_NAME,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO 
										),
										rqc_bad_data_tmp3 AS (
										SELECT
											INSPECTION_NAME,
											art_no,
											art_name, Shoe_type_name ,
											ORG_CODE,
											ORG_NAME,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO,
											pass_qty,
											bad_qty,
											sum( bad_qty ) over ( partition BY ORG_NAME, WORKSHOP_SECTION_NO,CONFIG_NO, art_no, art_name, Shoe_type_name ) total_qty,
											ROW_NUMBER ( ) over ( partition BY ORG_NAME, WORKSHOP_SECTION_NO,CONFIG_NO, art_no, art_name, Shoe_type_name ORDER BY bad_qty DESC, pass_qty + bad_qty DESC ) ranking 
										FROM
											rqc_bad_data_tmp2 
										),
										rqc_bad_data AS (
										SELECT
											INSPECTION_NAME,
											art_no,
											art_name, Shoe_type_name ,
											ORG_CODE,
											ORG_NAME,
											WORKSHOP_SECTION_NO,
											WORKSHOP_SECTION_NAME,
											CONFIG_NO,
											pass_qty,
											bad_qty,
											(CASE 
												WHEN total_qty=0 THEN
													0
												ELSE
													round( bad_qty / total_qty * 100, 2 ) 
											END) fail_rate,
											-- round( bad_qty / total_qty * 100, 2 ) fail_rate,
											total_qty,
											ranking 
										FROM
											rqc_bad_data_tmp3 
										) SELECT
										a.* ,
										b.INSPECTION_NAME INSPECTION_NAME_1,
										b.fail_rate fail_rate_1,
										c.INSPECTION_NAME INSPECTION_NAME_2,
										c.fail_rate fail_rate_2,
										d.INSPECTION_NAME INSPECTION_NAME_3,
										d.fail_rate fail_rate_3 
									FROM
										rqc_data a 
										left JOIN rqc_bad_data b ON a.ORG_CODE = b.ORG_CODE 
										AND a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
										AND a.art_no = b.art_no 
										AND a.CONFIG_NO = b.CONFIG_NO 
										AND b.ranking = 1
										left JOIN rqc_bad_data c ON a.ORG_CODE = c.ORG_CODE 
										AND a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO 
										AND a.art_no = c.art_no 
										AND a.CONFIG_NO = c.CONFIG_NO 
										AND c.ranking = 2
										left JOIN rqc_bad_data d ON a.ORG_CODE = d.ORG_CODE 
										AND a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO 
										AND a.art_no = d.art_no 
										AND a.CONFIG_NO = d.CONFIG_NO 
										AND d.ranking =3";  //APE Query
				DataTable AllDt = DB.GetDataTable(DataSQL);

				List<string> TopList = new List<string>() { "PASS_RATE Desc", "total_qty Desc" };
				List<string> LastList = new List<string>() { "PASS_RATE asc", "total_qty asc" };
				RetDic["Top5DT"] = Common.ReturnDTOrderByRows(AllDt, TopList, 20, bool.Parse(IsExport));
				if (!bool.Parse(IsExport))
                {
					RetDic["Last5DT"] = Common.ReturnDTOrderByRows(AllDt, LastList, 20);
				}
				
				

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

	}
}
