using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_KanBanAPI
{
    /// <summary>
    ///万邦中国区品质看板接口
    /// </summary>
    internal class QualityKBData
    {
        public static decimal RFTFZNumALL = 0;
        public static decimal RFTFMNumALL = 0;
        public static string moudle_code = "chinaRegion"; //中国区
        //public static string moudle_code = "mainQualityIndicators"; //主要品质指标
        //public static string moudle_code2 = "testPart";             //测试部
        //public static string moudle_code3 = "marketFeedback";       //市场反馈
        //public static string moudle_code4 = "qualityAbnormal";      //品质异常
        //public static string moudle_code5 = "forepartQ";            //前段Q
        //public static string moudle_code6 = "workshopQ";            //车间Q
        //public static string moudle_code7 = "aqLInspection";        //AQL

        /// <summary>
        /// 主要品质指标数据获取
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQualityIndex(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "en";//
                string languageType = jarr.ContainsKey("languageType") ? jarr["languageType"].ToString() : "2";//语言类别【1】中国区【2】印度区【3】越南区

                string dateYear = DateTime.Now.ToString("yyyy");

                //多语言翻译
                var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "CWA合规", "Article上线顺畅率", "测试合格率", "面料合格率" , "底料合格率", "工艺合格率", "贴底合格率", "鞋面合格率", "加工合格率" , "AQL合格率", "翻箱率", "B品率", "退货PPM", "投诉数量PPM($)", "投诉金额PPM" });
                string CWAname = string.Empty;
                string Articlename = string.Empty;
                string CSname = string.Empty;
                string MLname = string.Empty;
                string DLname = string.Empty;
                string GYname = string.Empty;
                string TDname = string.Empty;
                string XMname = string.Empty;
                string JGname = string.Empty;
                string AQLname = string.Empty;
                string FXname = string.Empty;
                string Bname = string.Empty;
                string PPMname = string.Empty;
                string TSPPMname = string.Empty;
                string TSJEMname = string.Empty;
                if (LanDic.Count > 0)
                {
                    CWAname = LanDic["CWA合规"].ToString();
                    Articlename = LanDic["Article上线顺畅率"].ToString();
                    CSname = LanDic["测试合格率"].ToString();
                    MLname = LanDic["面料合格率"].ToString();
                    DLname = LanDic["底料合格率"].ToString();
                    GYname = LanDic["工艺合格率"].ToString();
                    TDname = LanDic["贴底合格率"].ToString();
                    XMname = LanDic["鞋面合格率"].ToString();
                    JGname = LanDic["加工合格率"].ToString();
                    AQLname = LanDic["AQL合格率"].ToString();
                    FXname = LanDic["翻箱率"].ToString();
                    Bname = LanDic["B品率"].ToString();
                    PPMname = LanDic["退货PPM"].ToString();
                    TSPPMname = LanDic["投诉数量PPM($)"].ToString();
                    TSJEMname = LanDic["投诉金额PPM"].ToString();

                }



                List<Dictionary<string, object>> RetDicList = new List<Dictionary<string, object>>();
                #region CWA合规
                Dictionary<string, object> CWAPassDic = new Dictionary<string, object>();
                CWAPassDic["name"] = CWAname;
                string CWArate = "99.8%";
                CWAPassDic["percent1"] = CWArate;
                CWAPassDic["percent2"] = "100%";
                CWAPassDic["num1"] = (1093).ToString("N0");
                CWAPassDic["num2"] = (1095).ToString("N0");
                #endregion
                RetDicList.Add(CWAPassDic);
                #region Article上线顺畅率 修改分子
                Dictionary<string, object> ArticlePassDic = new Dictionary<string, object>();
                string ArtFMSQL = $@"SELECT
	                                    COUNT( DISTINCT art_no ) 
                                    FROM
	                                    SFC_TRACKIN_LIST 
                                    WHERE
	                                    to_char( scan_date, 'yyyy' ) = TO_CHAR(SYSDATE,'yyyy')";
                string ArtFZSQL = $@"SELECT
	                                    COUNT(DISTINCT PROD_NO)
                                    FROM
	                                    QCM_ABNORMAL_R_M WHERE  TO_CHAR(TO_DATE(CREATEDATE, 'yyyy-mm-dd'),'yyyy') = TO_CHAR(SYSDATE,'yyyy')";
                double ArtFM = DB.GetDouble(ArtFMSQL);
                double ArtFz = DB.GetDouble(ArtFZSQL);
                ArticlePassDic["name"] = Articlename;
                string Articlerate = "0%";
                if (ArtFM > 0)
                {
                    Articlerate = Math.Round((ArtFM - ArtFz) / ArtFM, 3) * 100 + "%";
                }
                ArticlePassDic["percent1"] = Articlerate;
                ArticlePassDic["percent2"] = "98%";
                ArticlePassDic["num1"] = (ArtFM - ArtFz).ToString("N0");
                ArticlePassDic["num2"] = ArtFM.ToString("N0");
                #endregion
                RetDicList.Add(ArticlePassDic);
                #region 测试合格率
                double TestPassNum = DB.GetDouble($@"SELECT
	                                                COUNT( 1 ) 
                                                FROM
	                                                QCM_EX_TASK_LIST_M 
                                                WHERE
	                                                NVL( TEST_RESULT, '' ) = 'PASS' 
	                                                AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = '{dateYear}' ");
                double TestAllNum = DB.GetDouble($@"SELECT
	                                                COUNT( 1 ) 
                                                FROM
	                                                QCM_EX_TASK_LIST_M 
                                                WHERE
	                                                TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM-dd' ) >= '{dateYear}' ");
                Dictionary<string, object> TestPassDic = new Dictionary<string, object>();
                TestPassDic["name"] = CSname;
                string Testrate = "0%";
                if (TestAllNum > 0)
                {
                    Testrate = (Math.Round(TestPassNum / TestAllNum, 3)) * 100 + "%";
                }
                TestPassDic["percent1"] = Testrate;
                TestPassDic["percent2"] = "90%";
                TestPassDic["num1"] = TestPassNum.ToString("N0");
                TestPassDic["num2"] = TestAllNum.ToString("N0");
                #endregion
                RetDicList.Add(TestPassDic);
                #region 面料合格率
                double FabricPassNum = DB.GetDouble($@"SELECT
	                                                        COUNT( 1 ) 
                                                        FROM
	                                                        QCM_IQC_INSP_RES_M
                                                        WHERE
                                                            DETERMINE = '0'
                                                            AND TO_CHAR(TO_DATE(CREATEDATE, 'yyyy-mm-dd'), 'yyyy') = '{dateYear}'");
                double FabricAllNum = DB.GetDouble($@"SELECT
	                                                        COUNT( 1 ) 
                                                        FROM
	                                                        QCM_IQC_INSP_RES_M
                                                        WHERE
                                                            TO_CHAR(TO_DATE(CREATEDATE, 'yyyy-mm-dd'), 'yyyy') = '{dateYear}' ");
                Dictionary<string, object> FabricPassDic = new Dictionary<string, object>();
                FabricPassDic["name"] = MLname;
                string Fabricrate = "0%";
                if (FabricAllNum > 0)
                {
                    Fabricrate = Math.Round(FabricPassNum / FabricAllNum, 3) * 100 + "%";
                }
                FabricPassDic["percent1"] = Fabricrate;
                FabricPassDic["percent2"] = "95%";
                FabricPassDic["num1"] = FabricPassNum.ToString("N0");
                FabricPassDic["num2"] = FabricAllNum.ToString("N0");
                #endregion
                RetDicList.Add(FabricPassDic);
                #region 底料合格率
                //底料工艺数据查询
                DataTable RQCDT = DB.GetDataTable($@"SELECT
                                                        a.WORKSHOP_SECTION_NO NAME,
	                                                    -- WORKSHOP_SECTION_NAME NAME,
	                                                    COUNT( 1 )  PassQty,
	                                                    sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) TestQty ,
	                                                    sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
                                                    FROM
	                                                    rqc_task_m a
	                                                    JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
	                                                    LEFT JOIN QCM_DQA_MAG_M ON QCM_DQA_MAG_M.WORKSHOP_SECTION_NO = A.WORKSHOP_SECTION_NO 
	                                                    AND A.SHOE_NO = QCM_DQA_MAG_M.SHOES_CODE 
                                                    WHERE
	                                                    substr( d.CREATEDATE, 0, 4 ) = to_char( SYSDATE, 'yyyy' ) 
	
                                                    GROUP BY
	                                                    a.WORKSHOP_SECTION_NO");
                Dictionary<string, object> GESSOPassDic = new Dictionary<string, object>();
                GESSOPassDic["name"] = DLname;
                string GESSOrate = "93%";

                GESSOPassDic["percent1"] = GESSOrate;
                GESSOPassDic["percent2"] = "95%";
                GESSOPassDic["num1"] = (55350).ToString("N0");
                GESSOPassDic["num2"] = (59500).ToString("N0");
                #endregion
                RetDicList.Add(GESSOPassDic);

                #region 工艺合格率
                Dictionary<string, object> techniquePassDic = new Dictionary<string, object>();
                techniquePassDic["name"] = GYname;
                string techniquerate = "0%";
                techniquePassDic["percent1"] = techniquerate;
                techniquePassDic["percent2"] = "95%";
                techniquePassDic["num1"] = "0";
                techniquePassDic["num2"] = "0";
                DataRow[] GYROWS = RQCDT.Select($@"NAME='GY'");//工艺
                if (GYROWS != null && GYROWS.Length > 0)
                {
                    DataRow GYRow = GYROWS[0];
                    if (Convert.ToDecimal(GYRow["PassQty"].ToString()) > 0)
                    {
                        techniquerate = Math.Round(Convert.ToDecimal(GYRow["TestQty"].ToString()) / Convert.ToDecimal(GYRow["PassQty"].ToString()) * 100, 1) + "%";
                        techniquePassDic["percent1"] = techniquerate;
                        techniquePassDic["percent2"] = "95%";
                        techniquePassDic["num1"] = Convert.ToDecimal(GYRow["TestQty"].ToString()).ToString("N0");
                        techniquePassDic["num2"] = Convert.ToDecimal(GYRow["PassQty"].ToString()).ToString("N0");
                    }
                }
                #endregion
                RetDicList.Add(techniquePassDic);
                #region 贴底合格率
                /*
                string TQCSQL = $@"WITH qtyTemp AS (
	                                SELECT
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                FROM
		                                tqc_task_commit_m 
	                                WHERE
		                                commit_type IN ( '0', '1', '2', '3', '4' ) 
	                                GROUP BY
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                ),
	                                baseDt AS (
	                                SELECT
		                                TASK_NO,
                                        a.WORKSHOP_SECTION_NO,
		                                b.WORKSHOP_SECTION_NAME 
	                                FROM
		                                TQC_TASK_M a
		                                JOIN QCM_DQA_MAG_M b ON a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		                                AND a.SHOE_NO = b.SHOES_CODE 
                                        
	                                WHERE
		                                TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
		                                AND a.WORKSHOP_SECTION_NO IN ( 'T', 'L' ) 
	                                ) SELECT
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                COUNT( 1 ) NUM,
	                                b.WORKSHOP_SECTION_NAME 
	                                a.WORKSHOP_SECTION_NO 
                                FROM
	                                qtyTemp
	                                JOIN basedt ON qtyTemp.TASK_NO = basedt.TASK_NO 
                                GROUP BY
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                a.WORKSHOP_SECTION_NO";
                */
                string TQCSQL = $@"WITH qtyTemp AS (
	                                SELECT
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                FROM
		                                tqc_task_commit_m 
	                                WHERE
		                                commit_type IN ( '0', '1', '2', '3', '4' ) 
	                                GROUP BY
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                ),
	                                baseDt AS (
	                                SELECT
		                                TASK_NO,
                                    a.WORKSHOP_SECTION_NO
		                               -- b.WORKSHOP_SECTION_NAME 
	                                FROM
		                                TQC_TASK_M a
		                              --  JOIN QCM_DQA_MAG_M b ON a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		                               -- AND a.SHOE_NO = b.SHOES_CODE 
	                                WHERE
		                                TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
		                                AND a.WORKSHOP_SECTION_NO IN ( 'T', 'L' ) 
	                                ) SELECT
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                COUNT( 1 ) NUM,
	                                -- basedt.WORKSHOP_SECTION_NAME ,
	                                basedt.WORKSHOP_SECTION_NO 
                                FROM
	                                qtyTemp
	                                JOIN basedt ON qtyTemp.TASK_NO = basedt.TASK_NO 
                                GROUP BY
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                WORKSHOP_SECTION_NO";
                DataTable TQCDT = DB.GetDataTable(TQCSQL);
                List<string> QueryStr = new List<string> { "T", "L" };//T-贴底 L-加工
                decimal bottomPNum = 0;
                decimal bottomCNum = 0;
                decimal machiningCNum = 0;
                decimal machiningPNum = 0;
                foreach (var item in QueryStr)
                {
                    DataRow[] QueryRow = TQCDT.Select($@"WORKSHOP_SECTION_NO='{item}'");
                    foreach (DataRow Ritem in QueryRow)
                    {
                        if (Ritem["COMMIT_TYPE"].ToString() == "0")
                        {
                            if (item == "T")
                            {
                                bottomCNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                            else if (item == "L")
                            {
                                machiningCNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                        }
                        if (item == "T")
                        {
                            if (Ritem["COMMIT_TYPE"].ToString() == "0" || Ritem["COMMIT_TYPE"].ToString() == "1")
                                bottomPNum += Convert.ToDecimal(Ritem["NUM"].ToString());

                        }
                        else if (item == "L")
                        {
                            if (Ritem["COMMIT_TYPE"].ToString() == "0" || Ritem["COMMIT_TYPE"].ToString() == "1")
                                machiningPNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                        }
                    }
                }


                Dictionary<string, object> BottomPassDic = new Dictionary<string, object>();
                BottomPassDic["name"] = TDname;

                string Bottomrate = "0%";
                if (bottomPNum > 0)
                {
                    Bottomrate = Math.Round(bottomCNum / bottomPNum * 100, 1) + "%";
                }
                BottomPassDic["percent1"] = Bottomrate;
                BottomPassDic["percent2"] = "95%";
                BottomPassDic["num1"] = bottomCNum.ToString("N0");
                BottomPassDic["num2"] = bottomPNum.ToString("N0");
                #endregion
                RetDicList.Add(BottomPassDic);
                #region 鞋面合格率
                Dictionary<string, object> UpperPassDic = new Dictionary<string, object>();
                UpperPassDic["name"] = XMname;
                string Upperrate = "0%";
                UpperPassDic["percent1"] = Upperrate;
                UpperPassDic["percent2"] = "95%";
                UpperPassDic["num1"] = "0";
                UpperPassDic["num2"] = "0";
                DataRow[] XMROWS = RQCDT.Select($@"NAME='S'");//针车
                if (XMROWS != null && XMROWS.Length > 0)
                {
                    DataRow XMRow = XMROWS[0];
                    if (Convert.ToDecimal(XMRow["PassQty"].ToString()) > 0)
                    {
                        Upperrate = Math.Round(Convert.ToDecimal(XMRow["TestQty"].ToString()) / Convert.ToDecimal(XMRow["PassQty"].ToString()) * 100, 1) + "%";
                        UpperPassDic["percent1"] = Upperrate;
                        UpperPassDic["percent2"] = "95%";
                        UpperPassDic["num1"] = Convert.ToDecimal(XMRow["TestQty"].ToString()).ToString("N0");
                        UpperPassDic["num2"] = Convert.ToDecimal(XMRow["PassQty"].ToString()).ToString("N0");
                    }
                }
                #endregion
                RetDicList.Add(UpperPassDic);
                #region 加工合格率
                Dictionary<string, object> machiningPassDic = new Dictionary<string, object>();
                machiningPassDic["name"] = JGname;

                string machiningrate = "0%";
                if (machiningPNum > 0)
                {
                    machiningrate = Math.Round(machiningCNum / machiningPNum * 100, 1) + "%";
                }
                RFTFMNumALL = machiningPNum;
                RFTFZNumALL = machiningCNum;
                machiningPassDic["percent1"] = machiningrate;
                machiningPassDic["percent2"] = "85%";
                machiningPassDic["num1"] = machiningCNum.ToString("N0");
                machiningPassDic["num2"] = machiningPNum.ToString("N0");
                #endregion
                RetDicList.Add(machiningPassDic);
                #region AQL合格率
                Dictionary<string, object> AQLPassDic = new Dictionary<string, object>();
                string AQLSQL = $@"WITH tmp AS (
	                                    SELECT
		                                    SUM( a.bad_qty ) AS bad_qty,
		                                    a.task_no 
	                                    FROM
		                                    aql_cma_task_list_m_aql_e_br a
		                                    LEFT JOIN aql_cma_task_list_m_aql_e_br_f b ON a.task_no = b.task_no 
		                                    AND a.bad_classify_code = b.bad_classify_code 
		                                    AND a.bad_item_code = b.bad_item_code 
	                                    WHERE
		                                    a.problem_level IN ( '0', '1', '2' ) 
	                                    GROUP BY
		                                    a.task_no 
	                                    )
                                    SELECT
	                                    a.task_no,--任务编号
	                                    a.lot_num,--分批数量
                                        a.inspection_date,
	                                    NVL( c.sample_level, '2' ) sample_level,
	                                    NVL( c.aql_level, 'AC13' ) aql_level,
	                                    NVL( TMP.BAD_QTY, 0 ) BAD_QTY,
	                                    '' inspection_state 
                                    FROM
	                                    aql_cma_task_list_m a
	                                    LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no = a.task_no
	                                    LEFT JOIN TMP ON TMP.task_no = A.TASK_NO
	                                    WHERE   TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) =TO_CHAR(SYSDATE,'yyyy')";
                DataTable AQLDt = DB.GetDataTable(AQLSQL);

                List<string> ACListr = new List<string>();
                foreach (DataRow item in AQLDt.Rows)
                {
                    if (!ACListr.Contains(item["aql_level"].ToString()))
                    {
                        ACListr.Add(item["aql_level"].ToString());
                    }
                }
                string ACSQL = $@"SELECT
	                                VALS,
                                    {string.Join(",", ACListr)},
	                                to_number(START_QTY) START_QTY,
	                                to_number(END_QTY) END_QTY,
	                                LEVEL_TYPE 
                                FROM
	                                BDM_AQL_M 
                                WHERE
	                                HORI_TYPE = '2'";
                DataTable ACDT = DB.GetDataTable(ACSQL);

                foreach (DataRow item in AQLDt.Rows)
                {
                    DataRow[] QueryName = ACDT.Select($@" LEVEL_TYPE='{item["sample_level"].ToString()}' and START_QTY<={Convert.ToDecimal(item["lot_num"].ToString())} and END_QTY>={Convert.ToDecimal(item["lot_num"].ToString())}");
                    if (QueryName.Count() > 0)
                    {
                        int acInt = 0;
                        bool cRes = int.TryParse(QueryName[0][item["aql_level"].ToString()].ToString(), out acInt);
                        if (cRes)
                        {
                            if (!string.IsNullOrEmpty(item["inspection_date"].ToString()))
                            {
                                if (Convert.ToInt32(item["BAD_QTY"].ToString()) > acInt)
                                    item["inspection_state"] = "0";
                                else
                                    item["inspection_state"] = "1";
                            }
                        }
                    }
                }
                decimal Cnum = AQLDt.Select("inspection_state='1'").Count();
                decimal pnum = AQLDt.Select("inspection_state='0'").Count();
                //AQLPassDic["name"] = "AQL合格率";
                AQLPassDic["name"] = "AQL pass rate";
                string AQLrate = "0%";
                if (pnum > 0)
                {
                    AQLrate = Math.Round((Cnum / (pnum + Cnum) * 100), 1) + "%";
                }
                AQLPassDic["percent1"] = AQLrate;
                AQLPassDic["percent2"] = "98%";
                AQLPassDic["num1"] = Cnum.ToString("N0");
                AQLPassDic["num2"] = (pnum + Cnum).ToString("N0");
                #endregion
                RetDicList.Add(AQLPassDic);

                #region 翻箱率
                Dictionary<string, object> TurnOverPassDic = new Dictionary<string, object>();
                string TurnOverSqlAll = $@"SELECT
	                                            COUNT( 1 ) 
                                            FROM
	                                            aql_cma_task_list_m 
                                            WHERE
	                                            TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM-dd' ) >= '{dateYear}' 
	                                            ";
                string TOAndSql = $@"AND inspection_type IN ( '1', '2', '3' )";
                double TOCNum = DB.GetDouble(TurnOverSqlAll + TOAndSql);
                double TOPNum = DB.GetDouble(TurnOverSqlAll);
                TurnOverPassDic["name"] = FXname;
                string TurnOverrate = "0%";
                decimal FZ = DB.GetDecimal($@"SELECT
                                                COUNT(1)
                                                FROM
	                                                QCM_ABNORMAL_R_M 
                                                WHERE
	                                                ABNORMAL_CATEGORY_NO = '008' 
	                                                AND FX_QTY > 0 
	                                                AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
	                                                ");
                if (pnum + Cnum > 0)
                {
                    TurnOverrate = Math.Round(FZ / (pnum + Cnum), 3) * 100 + "%";
                }
                TurnOverPassDic["percent1"] = TurnOverrate;
                TurnOverPassDic["percent2"] = "3%";
                TurnOverPassDic["num1"] = FZ.ToString("N0");
                TurnOverPassDic["num2"] = (pnum + Cnum).ToString("N0");
                #endregion
                RetDicList.Add(TurnOverPassDic);
                #region B品率
                Dictionary<string, object> BMPassDic = new Dictionary<string, object>();
                string BMSql = $@"SELECT
	                                    COUNT( 1 ) 
                                    FROM
	                                    (
	                                    SELECT
		                                    COMMIT_INDEX 
	                                    FROM
		                                    tqc_task_commit_m 
	                                    WHERE
		                                    TASK_NO IN ( SELECT TASK_NO FROM tqc_task_m WHERE TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM-dd' ) >= '{dateYear}' ) 
		                                    AND commit_type in ('0','1','2','3','4')
	                                    GROUP BY
		                                    COMMIT_INDEX 
	                                    ) t";
                string BMCSql = $@"	SELECT
	                                    COUNT( 1 ) 
                                    FROM
	                                    (
	                                    SELECT
		                                    COMMIT_INDEX 
	                                    FROM
		                                    tqc_task_commit_m 
	                                    WHERE
		                                    TASK_NO IN ( SELECT TASK_NO FROM tqc_task_m WHERE TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM-dd' ) >= '{dateYear}' ) 
		                                    AND commit_type in ('4')
	                                    GROUP BY
		                                    COMMIT_INDEX 
	                                    ) t";
                decimal BMCNum = DB.GetDecimal(BMCSql);
                decimal BMPNum = DB.GetDecimal(BMSql);


                BMPassDic["name"] = Bname;
                string BMrate = "0%";
                if (BMPNum > 0)
                {
                    BMrate = Math.Round(BMCNum / (machiningPNum * 2) * 100, 3) + "%"; //B品率分母修改取加工分母
                }
                BMPassDic["percent1"] = BMrate;
                BMPassDic["percent2"] = "0.03%";
                BMPassDic["num1"] = BMCNum.ToString("N0");
                //BMPassDic["num2"] = (machiningPNum * 2).ToString("N0");
                BMPassDic["num2"] = (machiningPNum * 2).ToString("N0");
                #endregion
                RetDicList.Add(BMPassDic);
                #region 退货PPM
                Dictionary<string, object> PPMPassDic = new Dictionary<string, object>();
                decimal THQty = DB.GetDecimal($@"SELECT
	                                                NVL(SUM (QTY),0) QTY 
                                                FROM
	                                                QCM_CUSTOMER_RETURN_M 
                                                WHERE
	                                                SUBSTR(RETURN_MONTH,0,4) = TO_CHAR( SYSDATE, 'yyyy' )");

                decimal AVGQty = DB.GetDecimal($@"WITH temp AS (
	                                                SELECT
		                                                NVL(ROUND( AVG( QTY ), 1 ),0) qty  
	                                                FROM
		                                                ( SELECT T_QTY qty FROM QCM_KANBAN_FINISH ORDER BY FINISH_DATE DESC ) A 
	                                                WHERE
		                                                ROWNUM <= 24 
	                                                ) SELECT
	                                                qty * TO_NUMBER( ( SELECT NVL(TO_CHAR( TO_DATE( MAX( RETURN_MONTH ), 'yyyy-mm' ), 'mm' ),0) FROM QCM_CUSTOMER_RETURN_M ) ) qty
                                                FROM
	                                                temp");//SELECT NVL(TO_CHAR( TO_DATE( MAX( RETURN_MONTH ), 'yyyy-mm' ), 'mm' ),0)
                PPMPassDic["name"] = PPMname;
                string PPMrate = "0";
                if (AVGQty > 0)
                {
                    PPMrate = Math.Round((THQty / AVGQty) * 1000000, 0).ToString();
                }
                PPMPassDic["percent1"] = PPMrate;
                PPMPassDic["percent2"] = "1300";
                PPMPassDic["num1"] = Math.Round(THQty, 0).ToString("N0");
                PPMPassDic["num2"] = Math.Round(AVGQty, 0).ToString("N0");
                #endregion
                RetDicList.Add(PPMPassDic);
                #region 投诉数量PPM
                Dictionary<string, object> QMDIC = DB.GetDictionary($@"SELECT
	                                                                        NVL(SUM(NG_QTY),0) QTY,
	                                                                        NVL(SUM(COMPLAINT_MONEY),0) MONEY
                                                                        FROM
	                                                                        QCM_CUSTOMER_COMPLAINT_M 
                                                                        WHERE
	                                                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )");
                Dictionary<string, object> WHCPassDic = new Dictionary<string, object>();
                WHCPassDic["name"] = TSPPMname;
                string WHCrate = "0";
                if (AVGQty > 0)
                {
                    WHCrate = Math.Round((Convert.ToDecimal(QMDIC["QTY"].ToString()) / AVGQty) * 1000000, 0).ToString();
                }
                WHCPassDic["percent1"] = WHCrate;
                WHCPassDic["percent2"] = "500";
                WHCPassDic["num1"] = Convert.ToDecimal(QMDIC["QTY"].ToString()).ToString("N0");
                WHCPassDic["num2"] = Math.Round(AVGQty, 0).ToString("N0");
                #endregion
                RetDicList.Add(WHCPassDic);
                #region WHC金额PPM
                Dictionary<string, object> WHCMoneyPassDic = new Dictionary<string, object>();

                decimal AVGMoney = DB.GetDecimal($@"SELECT
	                                                    NVL( ROUND(sum( T_MONEY ),0),0) qty
                                                    FROM
	                                                    QCM_KANBAN_FINISH 
                                                    WHERE
	                                                    SUBSTR( FINISH_DATE, 0, 4 ) = TO_CHAR( SYSDATE, 'yyyy' )");
                WHCMoneyPassDic["name"] = TSJEMname;
                string WHCMoneyrate = "0";
                if (AVGMoney > 0)
                {
                    WHCMoneyrate = Math.Round((Convert.ToDecimal(QMDIC["MONEY"].ToString()) / AVGMoney) * Convert.ToInt32(DateTime.Now.ToString("MM")) * 1000000, 0).ToString();
                }
                WHCMoneyPassDic["percent1"] = WHCMoneyrate;
                WHCMoneyPassDic["percent2"] = "300";
                WHCMoneyPassDic["num1"] = Convert.ToDecimal(QMDIC["MONEY"].ToString()).ToString("N0");
                WHCMoneyPassDic["num2"] = Math.Round(AVGMoney, 0).ToString("N0");
                #endregion
                RetDicList.Add(WHCMoneyPassDic);

                ret.RetData1 = RetDicList;
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
        /// 主要品质看班-实验室数-1 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestData1(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
	                                                            SELECT
	                                                            *
	                                                            FROM
	                                                            DBLINKCONFIG
	                                                            WHERE
                                                            configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(
                    dbConfigRow["dbtype"].ToString(),
                    dbConfigRow["dbserver"].ToString(),
                    dbConfigRow["dbname"].ToString(),
                    dbConfigRow["dbuser"].ToString(),
                    dbConfigRow["dbpassword"].ToString(),
                    ""
                );
                }

                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                string dataType = "0";//数据查询枚举【0】成品鞋【3】材料【4】量产拉力

                // trim(translate(send_test_qty,'0123456789',' ')) is NULL 【因数据含有+号 需要过滤】
                string DataSqlTop = $@"WITH tmp AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' ) || '-' || lpad( LEVEL, 2, 0 ) YM FROM dual CONNECT BY LEVEL < 13 )
                                    SELECT
	                                    NVL( tb1.qty, 0 ) qty 
                                    FROM
	                                    (
	                                    SELECT
		                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) AS Ymdate,
		                                    NVL( SUM( send_test_qty ), 0 ) qty 
	                                    FROM
		                                    qcm_ex_task_list_m 
	                                    WHERE
		                                    trim(translate(send_test_qty,'0123456789',' ')) is NULL and  TEST_TYPE = '";
                string DataSqlBotton = $@"' 
	                                    GROUP BY
		                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) 
	                                    ORDER BY
		                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) 
	                                    ) tb1
	                                    RIGHT JOIN tmp ON tmp.YM = tb1.Ymdate 
                                    ORDER BY
	                                    tmp.YM";

                #region X轴
                //string MonthStr = "1月,2月,3月,4月,5月,6月,7月,8月,9月,10月,11月,12月";//月份Str
                string MonthStr = "January, February, March, April, May, June, July, August, September, October, November, December";//月份Str
                List<string> MonthList = MonthStr.Split(",").ToList();
                RetDic["x"] = MonthList;
                #endregion
                #region 柱状图数据
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "成品鞋测试", "材料测试", "拉力测试", "通过率", "FGT通过率", "材料通过率", "拉力通过率", "A-01 合格率", "Fit Test通过率", "Wear Test通过率" });

                List<Dictionary<string, object>> BarList = new List<Dictionary<string, object>>();
                //成品鞋测试
                string DataSql = DataSqlTop + dataType + DataSqlBotton;
                Dictionary<string, object> CPBarDic = new Dictionary<string, object>();

                CPBarDic["name"] = lanDic["成品鞋测试"].ToString();
                DataTable CPDt = DB.GetDataTable(DataSql);
                CPBarDic["date"] = TranDate("qty", CPDt);
                BarList.Add(CPBarDic);

                //材料
                Dictionary<string, object> CLBarDic = new Dictionary<string, object>();
                CLBarDic["name"] = lanDic["材料测试"].ToString();
                dataType = "3";
                DataSql = DataSqlTop + dataType + DataSqlBotton;
                DataTable CLDt = DB.GetDataTable(DataSql);
                CLBarDic["date"] = TranDate("qty", CLDt);
                BarList.Add(CLBarDic);
                //量产拉力
                Dictionary<string, object> LLBarDic = new Dictionary<string, object>();
                LLBarDic["name"] = lanDic["拉力测试"].ToString();
                dataType = "4";
                DataSql = DataSqlTop + dataType + DataSqlBotton;
                DataTable LLDt = DB.GetDataTable(DataSql);
                LLBarDic["date"] = TranDate("qty", LLDt);
                BarList.Add(LLBarDic);
                RetDic["bar"] = BarList;
                #endregion
                #region 折线图数据
                string PercentSqlTop = $@"WITH tmp AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' ) || '-' || lpad( LEVEL, 2, 0 ) YM FROM dual CONNECT BY LEVEL < 13 )
                                        SELECT
	                                        tmp.YM,
	                                        NVL( tb1.qty, 0 ) qty 
                                        FROM
	                                        (
	                                        SELECT
		                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) Ymdate,
		                                        COUNT( 1 ) qty
	                                        FROM
		                                        qcm_ex_task_list_m 
	                                        WHERE
	                                        TEST_RESULT is NOT NULL 
	                                        ";
                string PercentSqlBotton = $@"GROUP BY
		                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) 
	                                        ORDER BY
		                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-MM' ) 
	                                        ) tb1
	                                        RIGHT JOIN tmp ON tmp.YM = tb1.Ymdate 
                                        ORDER BY
	                                        tmp.YM";
                DataTable FMDt = DB.GetDataTable(PercentSqlTop + PercentSqlBotton);

                string PercentWhereSQL = " AND TEST_RESULT='PASS'";
                DataTable FZDt = DB.GetDataTable(PercentSqlTop + PercentWhereSQL + PercentSqlBotton);
                List<Dictionary<string, object>> LineDicList = new List<Dictionary<string, object>>();

                Dictionary<string, object> lineDic1 = new Dictionary<string, object>();
                lineDic1["name"] = lanDic["通过率"].ToString();
                lineDic1["data"] = PerentDatePlan(FMDt, FZDt);
                LineDicList.Add(lineDic1);

                RetDic["line"] = LineDicList;
                #endregion
                #region 下方通过率计算
                //查询成品【FGT通过率】【枚举：0】，材料【材料通过率】【枚举：3】，量产拉力【拉力通过率：4】
                string AllRateSql = $@"SELECT
	                                        TEST_RESULT,
	                                        COUNT( 1 ) qty,
	                                        TEST_TYPE 
                                        FROM
	                                        QCM_EX_TASK_LIST_M 
                                        WHERE
	                                        TEST_RESULT IS NOT NULL 
	                                        AND TEST_TYPE IN ( '0', '3', '4' ) 
	                                        AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                        GROUP BY
	                                        TEST_TYPE,
	                                        TEST_RESULT 
                                        ORDER BY
	                                        TEST_TYPE";
                DataTable AllRateDt = DB.GetDataTable(AllRateSql);
                List<Dictionary<string, object>> RateList = new List<Dictionary<string, object>>();
                Dictionary<string, object> FGTDic = new Dictionary<string, object>();
                FGTDic["name"] = lanDic["FGT通过率"].ToString();
                FGTDic["percent"] = "0%";
                FGTDic["qtyscale"] = "0/0";
                FGTDic["TestType"] = "0";
                RateList.Add(FGTDic);
                Dictionary<string, object> CLTDic = new Dictionary<string, object>();
                CLTDic["name"] = lanDic["材料通过率"].ToString();
                CLTDic["percent"] = "0%";
                CLTDic["qtyscale"] = "0/0";
                CLTDic["TestType"] = "3";
                RateList.Add(CLTDic);
                Dictionary<string, object> LLTDic = new Dictionary<string, object>();
                LLTDic["name"] = lanDic["拉力通过率"].ToString();
                LLTDic["percent"] = "0%";
                LLTDic["qtyscale"] = "0/0";
                LLTDic["TestType"] = "4";
                RateList.Add(LLTDic);

                foreach (var item in RateList)
                {
                    decimal PASSNum = 0;
                    decimal ALLNum = 0;
                    DataRow[] Selectrows = AllRateDt.Select($@"TEST_TYPE='{item["TestType"].ToString()}'");
                   foreach (DataRow row in Selectrows)
                    {
                        ALLNum += Convert.ToDecimal(row["qty"]);
                        if (row["TEST_RESULT"].ToString().ToLower().Contains("pass"))
                        {
                            PASSNum = Convert.ToDecimal(row["qty"]);
                        }
                    }
                    if (Selectrows.Length > 0)
                    {
                        item["percent"] = Math.Round((PASSNum / ALLNum) * 100, 1) + "%";
                        item["qtyscale"] = PASSNum.ToString("N0") + "/" + ALLNum.ToString("N0");
                    }
                }

                //功能缺少有效期无法开始 【BDM_VENDOR_REPORT_M】
                Dictionary<string, object> A01DataDic = DB.GetDictionary($@"SELECT
	                                                                        SUM( CASE WHEN TO_CHAR( ADD_MONTHS( FILE_STARTTIME, 13 ), 'yyyy-mm-dd' ) >=TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) THEN 1 ELSE 0 END )  VaildNum, 
	                                                                        COUNT(1) AllNum
                                                                        FROM
	                                                                        AQL_APP_T_FILE_M");
                Dictionary<string, object> A01Dic = new Dictionary<string, object>();
                A01Dic["name"] = lanDic["A-01 合格率"].ToString();
                A01Dic["percent"] = "0%";
                A01Dic["qtyscale"] = "0/0";
                A01Dic["TestType"] = "";
                if (A01DataDic != null && A01DataDic.Count > 0)
                {
                    decimal VaildNum = Convert.ToDecimal(A01DataDic["VAILDNUM"].ToString());
                    decimal AllNum = Convert.ToDecimal(A01DataDic["ALLNUM"].ToString());
                    A01Dic["percent"] = Math.Round(VaildNum / AllNum * 100, 1) + "%";
                    A01Dic["qtyscale"] = VaildNum.ToString("N0") + "/" + AllNum.ToString("N0");
                }
                RateList.Add(A01Dic);
                //COMMENTED DUE TO MY SQL

                //string FitSql = $@"SELECT
	               //                     DATE_FORMAT( REPORTDATE, '%Y' ) ym,
	               //                     FITRESULT,
	               //                     COUNT( 1 ) Count 
                //                    FROM
	               //                     fitinfo 
                //                    WHERE
	               //                     DATE_FORMAT( REPORTDATE, '%Y' )= DATE_FORMAT( CURDATE(), '%Y' ) 
	               //                     AND FITRESULT != 'NA' 
                //                    GROUP BY
	               //                     FITRESULT 

                //                    ";
                //DataTable FitDt = wbscDB.GetDataTable(FitSql);
                //Dictionary<string, object> FitDic = new Dictionary<string, object>();
                //FitDic["name"] = lanDic["Fit Test通过率"].ToString();
                //FitDic["percent"] = "0%";
                //FitDic["qtyscale"] = "0/0";
                //FitDic["TestType"] = "";
                //decimal FitPassNum = 0;
                //decimal FitALLNum = 0;
                //foreach (DataRow item in FitDt.Rows)
                //{
                //    FitALLNum += Convert.ToDecimal(item["Count"].ToString());
                //    if (item["FITRESULT"].ToString().ToLower().Contains("pass"))
                //    {
                //        FitPassNum += Convert.ToDecimal(item["Count"].ToString());
                //    }
                //}
                //if (FitALLNum > 0)
                //{
                //    FitDic["percent"] = Math.Round((FitPassNum / FitALLNum) * 100, 1) + "%";
                //    FitDic["qtyscale"] = FitPassNum.ToString("N0") + "/" + FitALLNum.ToString("N0");
                //}
                //RateList.Add(FitDic);


                //string WearSql = $@"SELECT
	               //                     DATE_FORMAT( ENDDATE, '%Y' ) ym,
	               //                     WEARRESULT,
	               //                     COUNT( 1 ) Count 
                //                    FROM
	               //                     wearinfo 
                //                    WHERE
	               //                     DATE_FORMAT( ENDDATE, '%Y' )= DATE_FORMAT( CURDATE(), '%Y' ) 
	               //                     AND WEARRESULT != 'NA' 
                //                    GROUP BY
	               //                     WEARRESULT;
                //                    ";
                //DataTable WeartDt = wbscDB.GetDataTable(WearSql);

                //Dictionary<string, object> WareTestDic = new Dictionary<string, object>();
                //WareTestDic["name"] = lanDic["Wear Test通过率"].ToString();
                //WareTestDic["percent"] = "80.3%";
                //WareTestDic["qtyscale"] = "155/193";
                //WareTestDic["TestType"] = "";

                //decimal WearPassNum = 0;
                //decimal WearALLNum = 0;
                //foreach (DataRow item in FitDt.Rows)
                //{
                //    WearALLNum += Convert.ToDecimal(item["Count"].ToString());
                //    if (item["FITRESULT"].ToString().ToLower().Contains("pass"))
                //    {
                //        WearPassNum += Convert.ToDecimal(item["Count"].ToString());
                //    }
                //}
                //if (WearALLNum > 0)
                //{
                //    WareTestDic["percent"] = Math.Round((WearPassNum / WearALLNum) * 100, 1) + "%";
                //    WareTestDic["qtyscale"] = WearPassNum.ToString("N0") + "/" + WearALLNum.ToString("N0");
                //}
                //RateList.Add(WareTestDic);

                RetDic["rate"] = RateList;
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
        /// 主要品质看班-实验室数-2 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestData2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                //var dbConfigDt = DBSqlServer.GetDataTable($@"
	               //                                             SELECT
	               //                                             *
	               //                                             FROM
	               //                                             DBLINKCONFIG
	               //                                             WHERE
                //                                            configno='wbsc';");
                //if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                //{
                //    var dbConfigRow = dbConfigDt.Rows[0];
                //    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(
                //    dbConfigRow["dbtype"].ToString(),
                //    dbConfigRow["dbserver"].ToString(),
                //    dbConfigRow["dbname"].ToString(),
                //    dbConfigRow["dbuser"].ToString(),
                //    dbConfigRow["dbpassword"].ToString(),
                //    ""
                //);
                //}

                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                //string DataSql = $@"SELECT
	               //                     DATE_FORMAT( REPORTDATE, '%Y-%m' ) ym,
	               //                     FITRESULT,
	               //                     COUNT( 1 ) Count 
                //                    FROM
	               //                     fitinfo 
                //                    WHERE
	               //                     DATE_FORMAT( REPORTDATE, '%Y' )= DATE_FORMAT( CURDATE(), '%Y' ) 
	               //                     AND FITRESULT != 'NA' 
                //                    GROUP BY
	               //                     DATE_FORMAT( REPORTDATE, '%Y-%m' ),
	               //                     FITRESULT 
                //                    ORDER BY
	               //                     DATE_FORMAT(
		              //                      REPORTDATE,
	               //                     '%Y-%m' 
	               //                     )";
                //DataTable FitDt = DB.GetDataTable(DataSql);
                //List<string> DateYM = new List<string>();
                //string Year = DateTime.Now.ToString("yyyy");
                //for (int i = 1; i < 13; i++)
                //{
                //    DateYM.Add(Year + "-" + i.ToString().PadLeft(2, '0'));
                //}
                //List<double> QtyDataList = new List<double>();//FIT总数
                //List<double> QtyPassList = new List<double>();//FIT通过率
                //foreach (var item in DateYM)
                //{
                //    double SumNum = 0;
                //    double PassNum = 0;
                //    DataRow[] SelRows = FitDt.Select($@"ym='{item}'");
                //    foreach (DataRow SelRow in SelRows)
                //    {
                //        SumNum += Convert.ToInt32(SelRow["Count"].ToString());
                //        if (SelRow["FITRESULT"].ToString().ToLower() == "pass")
                //        {
                //            PassNum += Convert.ToInt32(SelRow["Count"].ToString());
                //        }
                //    }
                //    QtyDataList.Add(SumNum);
                //    if (SumNum > 0)
                //    {
                //        QtyPassList.Add(Math.Round((PassNum / SumNum) * 100, 2));
                //    }
                //    else
                //    {
                //        QtyPassList.Add(0);
                //    }
                //}
                //#region X轴
                ////string MonthStr = "1月,2月,3月,4月,5月,6月,7月,8月,9月,10月,11月,12月";//月份Str
                //string MonthStr = "January, February, March, April, May, June, July, August, September, October, November, December";//月份Str
                //List<string> MonthList = MonthStr.Split(",").ToList();
                //RetDic["x"] = MonthList;
                //#endregion
                //#region 柱状图数据
                //var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "Fit Test", "Wear Test", "Fit Test通过率" });

                //List<Dictionary<string, object>> BarList = new List<Dictionary<string, object>>();
                ////成品鞋测试
                //Dictionary<string, object> FitBarDic = new Dictionary<string, object>();
                //FitBarDic["name"] = lanDic["Fit Test"].ToString();
                //FitBarDic["date"] = QtyDataList;
                //BarList.Add(FitBarDic);

                //string WareSql = $@"SELECT
	               //                     DATE_FORMAT( STARTDATE, '%Y-%m' ) ym,
	               //                     COUNT( 1 ) Count 
                //                    FROM
	               //                     wearinfo 
                //                    WHERE
	               //                     DATE_FORMAT( STARTDATE, '%Y' )= DATE_FORMAT( CURDATE(), '%Y' ) 
                //                    GROUP BY
	               //                     DATE_FORMAT( STARTDATE, '%Y-%m' ) 
                //                    ORDER BY
	               //                     DATE_FORMAT(
		              //                      STARTDATE,
	               //                     '%Y-%m' 
	               //                     )";
                //DataTable WareDt = DB.GetDataTable(WareSql);
                //List<int> WareList = new List<int>();
                //foreach (var item in DateYM)
                //{
                //    int SumNum = 0;

                //    DataRow[] SelRows = WareDt.Select($@"ym='{item}'");
                //    foreach (DataRow SelRow in SelRows)
                //    {
                //        SumNum += Convert.ToInt32(SelRow["Count"].ToString());
                //    }
                //    WareList.Add(SumNum);
                //}
                //Dictionary<string, object> WearBarDic = new Dictionary<string, object>();
                //WearBarDic["name"] = lanDic["Wear Test"].ToString();
                //WearBarDic["date"] = WareList;
                //BarList.Add(WearBarDic);
                //RetDic["bar"] = BarList;
                //#endregion
                //#region 折线图数据
                //List<Dictionary<string, object>> LineDicList = new List<Dictionary<string, object>>();
                //Dictionary<string, object> lineDic1 = new Dictionary<string, object>();
                //lineDic1["name"] = lanDic["Fit Test通过率"].ToString();
                //lineDic1["data"] = QtyPassList;
                //LineDicList.Add(lineDic1);
                //RetDic["line"] = LineDicList;
                //#endregion

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
        /// 主要品质看板-市场反馈
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMarketData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                Dictionary<string, object> RetDic = new Dictionary<string, object>();

                //string MonthStr = "1月,2月,3月,4月,5月,6月,7月,8月,9月,10月,11月,12月";//月份Str
                string MonthStr = "January, February, March, April, May, June, July, August, September, October, November, December";//月份Str
                List<string> MonthList = MonthStr.Split(",").ToList();
                RetDic["x"] = MonthList;


                #region Histogram_data
                List<Dictionary<string, object>> BarList = new List<Dictionary<string, object>>();

                //柱图数据
                string DataSql = $@"SELECT
	                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy-mm' ) THDate,
	                                    d.NAME_T,
	                                    NVL( SUM( a.QTY ), 0 ) qty 
                                    FROM
	                                    QCM_CUSTOMER_RETURN_M a
	                                    LEFT JOIN bdm_rd_prod b ON a.ARTICLE = b.PROD_NO
	                                    LEFT JOIN BDM_RD_STYLE c ON b.shoe_no = c.shoe_no
	                                    LEFT JOIN bdm_cd_code d ON c.style_seq = d.CODE_NO 
	                                    AND LANGUAGE = '1' 
                                    WHERE
	                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                    GROUP BY
	                                    d.NAME_T,
	                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy-mm' )";
                DataTable ReturnBarDt = DB.GetDataTable(DataSql);
                List<string> CategoryList = Common.DistantTranData("NAME_T", ReturnBarDt);
                List<string> DateYM = Common.ReturnData("m", DateTime.Now.ToString("yyyy-01"), DateTime.Now.ToString("yyyy-12"));
                foreach (var item in CategoryList)
                {
                    List<string> NumList = new List<string>();
                    foreach (var MonthItem in DateYM)
                    {
                        int num = 0;
                        DataRow[] SelectRows = ReturnBarDt.Select($@"THDate='{MonthItem}' and NAME_T='{item}'");
                        foreach (DataRow SelectRow in SelectRows)
                        {
                            num += Convert.ToInt32(SelectRow["qty"].ToString());
                        }
                        NumList.Add(num.ToString("N0"));
                    }
                    Dictionary<string, object> CategoryDic = new Dictionary<string, object>();
                    CategoryDic["name"] = item;
                    CategoryDic["data"] = NumList;
                    BarList.Add(CategoryDic);
                }
                RetDic["bar"] = BarList;

                //折线图数据
                List<Dictionary<string, object>> LineDicList = new List<Dictionary<string, object>>();
                string LineSQL = $@"WITH MonthData AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' ) || '-' || lpad( LEVEL, 2, 0 ) MONTH FROM dual CONNECT BY LEVEL < 13 ),
                                    BaseData AS (
	                                    SELECT
		                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy-mm' ) THDate,
		                                    SUM( a.QTY ) qty 
	                                    FROM
		                                    QCM_CUSTOMER_RETURN_M a
		                                    LEFT JOIN bdm_rd_prod b ON a.ARTICLE = b.PROD_NO
		                                    LEFT JOIN BDM_RD_STYLE c ON b.shoe_no = c.shoe_no
		                                    LEFT JOIN bdm_cd_code d ON c.style_seq = d.CODE_NO 
		                                    AND LANGUAGE = '1' 
	                                    WHERE
		                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
	                                    GROUP BY
		                                    TO_CHAR( TO_DATE( a.RETURN_MONTH, 'yyyy-mm' ), 'yyyy-mm' ) 
	                                    ) SELECT
	                                    monthdata.MONTH,
	                                    NVL( basedata.qty, 0 ) qty 
                                    FROM
	                                    monthdata
	                                    LEFT JOIN BaseData ON monthdata.MONTH = BaseData.THDate";
                DataTable Dt1 = DB.GetDataTable($@"
	                                                SELECT
		                                                TO_CHAR( TO_DATE( FINISH_DATE, 'yyyy-mm' ), 'yyyy-mm' ) MONTH,
		                                                round( (select avg(T_QTY) from  (select T_QTY from QCM_KANBAN_FINISH t where t.FINISH_DATE<=t1.FINISH_DATE order by t.FINISH_DATE desc ) x where rownum<=24),0) as qty

	                                                FROM
		                                                QCM_KANBAN_FINISH t1
	                                                WHERE
		                                                SUBSTR( FINISH_DATE, 0, 4 ) = TO_CHAR( SYSDATE, 'yyyy' ) 

	                                                ORDER BY
		                                                TO_CHAR( TO_DATE( FINISH_DATE, 'yyyy-mm' ), 'yyyy-mm' ) 
                                                ");


                DataTable LineDt = Common.ReturnOrderByTable(DB.GetDataTable(LineSQL), "MONTH", "asc");
             

                List<string> PPMList = new List<string>();
                foreach (var item in DateYM)
                {
                    decimal AvgQty = 0;
                    decimal Num = 0;    
                    DataRow[] QueryRows = LineDt.Select($@"MONTH <='{item}'");

                    foreach (DataRow Ritem in QueryRows)
                    {
                        Num += Convert.ToDecimal(Ritem["qty"].ToString());

                    }
                    DataRow[] MdtRows = Dt1.Select($@"MONTH ='{item}'");
                    foreach (DataRow Mitem in MdtRows)
                    {
                        AvgQty = Convert.ToDecimal(Mitem["qty"].ToString());
                    }
                    AvgQty = AvgQty * Convert.ToInt32(Convert.ToDateTime(item).ToString("MM"));
                    if (AvgQty > 0)
                    {
                        Num = Math.Round((Num / AvgQty) * 1000000, 0);
                    }
                    else
                    {
                        Num = 0;
                    }
                    PPMList.Add(Num.ToString("N0"));
                }
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "退货PPM","结案率","确认投诉","处理中"});

                Dictionary<string, object> lineDic1 = new Dictionary<string, object>();
                lineDic1["name"] = lanDic["退货PPM"].ToString();
                lineDic1["data"] = PPMList;
                LineDicList.Add(lineDic1);

                RetDic["line"] = LineDicList;
                #endregion
                #region WHC_Complaint_Calculation
                //查询成品【FGT通过率】【枚举：0】，材料【材料通过率】【枚举：3】，量产拉力【拉力通过率：4】
                List<Dictionary<string, object>> RateList = new List<Dictionary<string, object>>();
                string AllRateSQL = $@"SELECT
	                                        PROCESSING_RESULTS_STATUS,
	                                        COUNT( 1 ) NUM
                                        FROM
	                                        (SELECT* FROM QCM_CUSTOMER_COMPLAINT_M WHERE ROWID IN (
	                                        SELECT MIN(ROWID) FROM QCM_CUSTOMER_COMPLAINT_M GROUP BY COMPLAINT_NO)
	                                        ) T 
	                                        WHERE 
	                                        TO_CHAR(TO_DATE(CREATEDATE,'yyyy-mm-dd'),'yyyy')=TO_CHAR(SYSDATE,'yyyy')
                                        GROUP BY
	                                        PROCESSING_RESULTS_STATUS
                                        ";
                DataTable AllRateDt = DB.GetDataTable(AllRateSQL);
                decimal JANum = 0;//结案
                decimal AllNum = 0;
                decimal CLZNum = 0;//处理中
                decimal YesNum = 0;//确认WHC
                foreach (DataRow item in AllRateDt.Rows)
                {
                    AllNum += Convert.ToDecimal(item["NUM"].ToString());
                    if (!string.IsNullOrEmpty(item["PROCESSING_RESULTS_STATUS"].ToString()))
                    {
                        if (item["PROCESSING_RESULTS_STATUS"].ToString() == "0")
                        {
                            YesNum += Convert.ToDecimal(item["NUM"].ToString());
                        }
                        else
                        {
                            JANum += Convert.ToDecimal(item["NUM"].ToString());
                        }
                    }
                    else
                    {
                        CLZNum += Convert.ToDecimal(item["NUM"].ToString());
                    }
                }
                Dictionary<string, object> JADic = new Dictionary<string, object>();
                JADic["name"] = lanDic["结案率"].ToString();
                JADic["percent"] = "0%";
                JADic["qtyscale"] = "0/0";
                JADic["TestType"] = "0";

                Dictionary<string, object> YESDic = new Dictionary<string, object>();
                YESDic["name"] = lanDic["确认投诉"].ToString();
                YESDic["percent"] = "0%";
                YESDic["qtyscale"] = "0/0";
                YESDic["TestType"] = "3";

                Dictionary<string, object> CLDic = new Dictionary<string, object>();
                CLDic["name"] = lanDic["处理中"].ToString();
                CLDic["percent"] = "0%";
                CLDic["qtyscale"] = "0/0";
                CLDic["TestType"] = "4";

                if (AllNum > 0)
                {
                    JADic["percent"] = Math.Round((JANum / AllNum) * 100, 0) + "%";
                    JADic["qtyscale"] = JANum.ToString("N0") + "/" + AllNum.ToString("N0");

                    YESDic["percent"] = Math.Round((YesNum / AllNum) * 100, 0) + "%";
                    YESDic["qtyscale"] = YesNum.ToString("N0") + "/" + AllNum.ToString("N0");

                    CLDic["percent"] = Math.Round((CLZNum / AllNum) * 100, 0) + "%";
                    CLDic["qtyscale"] = CLZNum.ToString("N0") + "/" + AllNum.ToString("N0");
                }

                RateList.Add(JADic);
                RateList.Add(YESDic);
                RateList.Add(CLDic);

                RetDic["rate"] = RateList;
                #endregion
                #region List_of_Top_Five_Returned_Shoes 
                string Top5SQL = $@"SELECT
	                                    SHOES_NAME,
	                                    SUM(qty) QTY
                                    FROM
	                                    QCM_CUSTOMER_RETURN_M 
                                    WHERE
	                                    SUBSTR(RETURN_MONTH,0,4) = TO_CHAR( SYSDATE, 'yyyy' )
                                    GROUP BY
	                                    SHOES_NAME ";
                RetDic["TOP5"] = Common.ReturnTopNumTable(DB.GetDataTable(Top5SQL), "QTY", "desc", 5);
                #endregion
                #region WHC_case_status_list
                //string WHCTHSQL = $@"SELECT
                //                     m.COMPLAINT_NO,
                //                     MAX( TO_CHAR( m.COMPLAINT_DATE, 'yyyy-mm-dd' ) ) AS COMPLAINT_DATE,
                //                     MAX( m.DEFECT_CONTENT ) AS DEFECT_CONTENT,
                //                     MAX( m.PROCESSING_RESULTS_STATUS ) AS STATUS
                //                     --,MAX( m.processing_results_status ) AS processing_results_status 
                //                    FROM
                //                     QCM_CUSTOMER_COMPLAINT_M m 
                //                    WHERE
                //                     1 = 1 
                //                     AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                //                    GROUP BY
                //                     m.COMPLAINT_NO 
                //                    ORDER BY
                //                     max( m.id ) DESC";//Commented by Ashok and updated the query below

                string WHCTHSQL = $@"SELECT
                                      m.COMPLAINT_NO,
                                      MAX( TO_CHAR( m.COMPLAINT_DATE, 'yyyy-mm-dd' ) ) AS COMPLAINT_DATE,
                                      m.COUNTRY_REGION, 
                                      MAX(t.prod_no) as PROD_NO,
                                      MAX( m.DEFECT_CONTENT ) AS DEFECT_CONTENT,
                                      MAX( m.PROCESSING_RESULTS_STATUS ) AS STATUS
                                      --,MAX( m.processing_results_status ) AS processing_results_status 
                                    FROM
                                      QCM_CUSTOMER_COMPLAINT_M m 
                                      LEFT JOIN BDM_SE_ORDER_MASTER b on m.PO_ORDER=b.mer_po
                                        LEFT JOIN BDM_SE_ORDER_ITEM t on b.se_id=t.se_id
                                    WHERE
                                      1 = 1 
                                      AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )  
                                    GROUP BY
                                      m.country_region,
                                      m.po_order,
                                      m.COMPLAINT_NO 
                                    ORDER BY
                                      max( m.id ) DESC";
                DataTable WHCListDt = DB.GetDataTable(WHCTHSQL);
                foreach (DataRow item in WHCListDt.Rows)
                {
                    switch (item["STATUS"].ToString())
                    {
                        case "0":
                            //item["STATUS"] = "接受投诉";
                            item["STATUS"] = "Complaints_Accepted";
                            break;
                        case "1":
                            //item["STATUS"] = "客户撤销投诉";
                            item["STATUS"] = "Customer_Withdraws_Complaint";
                            break;
                        case "2":
                            //item["STATUS"] = "退货处理";
                            item["STATUS"] = "Processing";
                            break;
                        default:
                            break;
                    }
                }
                RetDic["WHCDt"] = WHCListDt;
                #endregion

                #region CountryWiseList
                string CountryWiseQuery = $@"select a.country_region as country, count(1) as quantity from QCM_CUSTOMER_COMPLAINT_M a group by a.country_region order by quantity desc";
                DataTable CountryWiseList = DB.GetDataTable(CountryWiseQuery);  
                RetDic["CountryWise"] = CountryWiseList;
                #endregion

                #region Top5countries
                string Top5Country = $@"with tmp as(select a.salesorgan_name,sum(a.qty) as return_quantity from QCM_CUSTOMER_RETURN_M a group by a.salesorgan_name order by sum(a.qty) desc),
tmp2 as (select tmp.*,rownum as RN from tmp),
tmp3 as( 
select 
case when tmp2.RN < 6 then tmp2.salesorgan_name  
else 'Others' end as Country,
 tmp2.return_quantity as return_quantity 
from tmp2)
select 
tmp3.country,
sum(tmp3.return_quantity) as return_quantity
from tmp3
group by tmp3.country order by tmp3.country";
                DataTable Top5CountryList = DB.GetDataTable(Top5Country);
                List<string> CountryList = Common.DistantTranData("country", Top5CountryList);
                RetDic["CountryList"] = CountryList;
                RetDic["Top5Country"] = Top5CountryList;
                #endregion

                #region Top10Articles
                string Top10Articles = $@"with tmp as(select a.article,sum(a.qty) as return_quantity from QCM_CUSTOMER_RETURN_M a group by a.article order by sum(a.qty) desc),
tmp2 as (select tmp.*,rownum as RN from tmp),
tmp3 as( 
select 
case when tmp2.RN < 11 then tmp2.article  
else 'Others' end as article,
 tmp2.return_quantity as return_quantity 
from tmp2)
select 
tmp3.article,
sum(tmp3.return_quantity) as return_quantity
from tmp3
group by tmp3.article order by return_quantity desc";
                DataTable Top10ArticlesList = DB.GetDataTable(Top10Articles);
                //List<string> CountryList = Common.DistantTranData("country", Top10ArticlesList);
                //RetDic["CountryList"] = CountryList;
                RetDic["Top10Articles"] = Top10ArticlesList;
                #endregion

                #region Top5Reasons
                string Top5Reasons = $@"with tmp as(select a.mastername,sum(a.qty) as return_quantity from QCM_CUSTOMER_RETURN_M a group by a.mastername order by sum(a.qty) desc),
tmp2 as (select tmp.*,rownum as RN from tmp),
tmp3 as( 
select 
case when tmp2.RN < 6 then tmp2.mastername  
else 'Others' end as mastername,
 tmp2.return_quantity as return_quantity 
from tmp2)
select 
tmp3.mastername as Reason,
sum(tmp3.return_quantity) as return_quantity
from tmp3
group by tmp3.mastername order by return_quantity desc";
                DataTable Top5ReasonsList = DB.GetDataTable(Top5Reasons);
                //List<string> CountryList = Common.DistantTranData("country", Top10ArticlesList);
                //RetDic["CountryList"] = CountryList;
                RetDic["Top5Reasons"] = Top5ReasonsList;
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
        /// 主要品质看板-DQA-未完成
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAData(object OBJ)
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
                RetDic["DQA"] = null;
                RetDic["MQA"] = null;

                #region DQA
                string DataSql = $@"SELECT
	                                            DEVELOP_SEASON,--季度
	                                            COUNT( DISTINCT BD.SHOE_NO ) shoeNum,--鞋型
	                                            COUNT( DISTINCT BD.PROD_NO ) artNum,--ART
	                                            SUM( NVL( QIT.BAD_QTY, 0 ) ) badQty,--风险数量
	                                            '0' AS improveQty,--已改善数量
	                                            '0' AS NotImproveQty,--已改善数量
	                                            '0' AS FD,--FD数量
	                                            '0' AS VS,--VS数量
	                                            '0' AS LR,--LR数量
	                                            '0' AS QT, --QT数量
	                                            '0.00%' AS improveQtyRate,--已改善数量
	                                            '0.00%' AS NotImproveQtyRate,--已改善数量
	                                            '0.00%' AS FDRate,--FD数量
	                                            '0.00%' AS VSRate,--VS数量
	                                            '0.00%' AS LRRate,--LR数量
	                                            '0.00%' AS QTRate --QT数量
                                            FROM
	                                            BDM_RD_PROD BD
	                                            LEFT JOIN QCM_SHOES_QA_RECORD_D QD ON QD.SHOES_CODE = BD.SHOE_NO
	                                            LEFT JOIN QCM_SHOES_QA_RECORD_ITEM QIT ON QIT.D_ID = QD.ID 
                                            WHERE
	                                            1 = 1 and  trim(translate(QIT.BAD_QTY,'0123456789',' ')) is NULL
AND 
( SUBSTR( BD.DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))

or   SUBSTR( BD.DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))+1
)
	
                                            GROUP BY
	                                            BD.DEVELOP_SEASON
                                            ORDER BY SUBSTR(DEVELOP_SEASON,3,2) desc,SUBSTR(DEVELOP_SEASON,1,2) asc
                                            -- ORDER BY DEVELOP_SEASON  DESC
                                             ";
                DataTable DataDt = DB.GetDataTable(DataSql);

                //改善，未改善，FD,VS,LR,其他数据查询处理
                string ItemDataSQL = $@"SELECT
	                                            develop_season DEVELOP_SEASON,
	                                            file_type UPLOADTYPE,
	                                            NVL( CHANGE_RESULT, '0' ) CHANGE_RESULT ,
	                                            sum( cnt ) AS qty 
                                            FROM
	                                            (
	                                            SELECT
		                                            shoes_code,
		                                            file_type,
		                                            count( * ) AS cnt,
		                                            (
		                                            SELECT
			                                            bd.develop_season 
		                                            FROM
			                                            BDM_RD_PROD BD 
		                                            WHERE
			                                            bd.shoe_no = t.shoes_code 
			                                           AND 
( 
SUBSTR( DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))

or   
SUBSTR( DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))+1
)
			                                            AND ROWNUM = 1 
		                                            ) AS develop_season,
		                                            (
		                                            SELECT
			                                            NVL( QIT.CHANGE_RESULT, '0' ) 
		                                            FROM
			                                            QCM_SHOES_QA_RECORD_ITEM QIT,
			                                            QCM_SHOES_QA_RECORD_D QD 
		                                            WHERE
			                                            QIT.D_ID = QD.ID 
			                                            AND QD.Shoes_Code = t.shoes_code 
			                                            AND ROWNUM = 1 
		                                            ) AS CHANGE_RESULT 
	                                            FROM
		                                            QCM_SHOES_QA_RECORD_FILE_M t  -- where t.SHOES_CODE = 'LZP26'
	                                            GROUP BY
		                                            shoes_code,
		                                            file_type 
	                                            ) 
                                            WHERE
	                                           1=1 
AND 
( 
SUBSTR( DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))

or   
SUBSTR( DEVELOP_SEASON, 3, 2 ) =TO_NUMBER(SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ))+1
)


                                            GROUP BY
	                                            develop_season,
	                                            file_type,
	                                            CHANGE_RESULT";
                DataTable ItemDt = DB.GetDataTable(ItemDataSQL);

                DataTable ALLDt = ReturnDt(DB, "");
                DataTable ENDDt = ReturnDt(DB, " and  a.bad_qty='0'");
                string YEAR = DateTime.Now.ToString("yy");


                foreach (DataRow item in DataDt.Rows)
                {
                    #region 变量定义
                    decimal improveQty = 0;//改善
                    decimal NotImproveQty = 0;//未改善
                    decimal FDQty = 0;//FD
                    decimal VSQty = 0;//VS
                    decimal LRQty = 0;//LR
                    decimal QTQty = 0;//其他
                    #endregion

                    DataRow[] SelectRows = ItemDt.Select($@"DEVELOP_SEASON='{item["DEVELOP_SEASON"]}'");
                    foreach (DataRow Ritem in SelectRows)
                    {


                        decimal NUM = Convert.ToDecimal(Ritem["QTY"].ToString());
                        if (NUM <= 0)
                        {
                            continue;
                        }
                        switch (Ritem["CHANGE_RESULT"].ToString())
                        {
                            case "0":
                                NotImproveQty += NUM;
                                break;
                            case "1":
                                improveQty += NUM;
                                break;
                        }
                        switch (Ritem["UPLOADTYPE"].ToString())
                        {
                            case "0":
                                FDQty += NUM;
                                break;
                            case "1":
                                VSQty += NUM;
                                break;
                            case "2":
                                LRQty += NUM;
                                break;
                            case "3":
                                QTQty += NUM;
                                break;
                        }

                    }
                    decimal AllNum = Convert.ToDecimal(item["artNum"].ToString());
                    if (AllNum > 0)
                    {
                        item["improveQty"] = improveQty.ToString("N0");
                        item["NotImproveQty"] = NotImproveQty.ToString("N0");
                        item["FD"] = FDQty.ToString("N0");
                        item["VS"] = VSQty.ToString("N0");
                        item["LR"] = LRQty.ToString("N0");
                        item["QT"] = QTQty.ToString("N0");
                        item["FDRate"] = Math.Round(FDQty / AllNum * 100, 1) + "%";
                        item["VSRate"] = Math.Round(VSQty / AllNum * 100, 1) + "%";
                        item["LRRate"] = Math.Round(LRQty / AllNum * 100, 1) + "%";
                    }
                    

                      DataRow[] QUERYROW = ALLDt.Select($@"DEVELOP_SEASON='{item["DEVELOP_SEASON"]}'");
                    foreach (DataRow Qitem in QUERYROW)
                    {
                        AllNum = Convert.ToDecimal(Qitem["CNT"].ToString());
                        item["badQty"] = Convert.ToDecimal(Qitem["CNT"].ToString()).ToString("N0");
                    }
                    DataRow[] QUERY2ROW = ENDDt.Select($@"DEVELOP_SEASON='{item["DEVELOP_SEASON"]}'");
                    foreach (DataRow Qitem in QUERY2ROW)
                    {
                        improveQty = Convert.ToDecimal(Qitem["CNT"].ToString());
                        item["improveQty"] = Convert.ToDecimal(Qitem["CNT"].ToString());
                    }
                    item["NotImproveQty"] = (AllNum - improveQty).ToString("N0");
                    if (AllNum > 0)
                    {
                        item["improveQtyRate"] = Math.Round(improveQty / AllNum * 100, 1) + "%";
                        item["NotImproveQtyRate"] = Math.Round((AllNum-(improveQty)) / AllNum * 100, 1)  + "%";
                    }

                    item["SHOENUM"] = decimal.Parse(item["SHOENUM"].ToString()).ToString("N0");
                    item["ARTNUM"] = decimal.Parse(item["ARTNUM"].ToString()).ToString("N0");
                    item["BADQTY"] = decimal.Parse(item["BADQTY"].ToString()).ToString("N0");

                }
                #endregion
                RetDic["DQA"] = DataDt;

                #region MQA
                string DataMQAsql = $@"WITH TMPDQA AS (
	                        SELECT
		                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
		                        COUNT( 1 ) shoeNum 
	                        FROM
		                        QCM_DQA_MAG_M QM
		                        LEFT JOIN QCM_DQA_MAG_D QD ON QM.ID = QD.M_ID 
	                        WHERE
		                        1 = 1 --AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
		
	                        GROUP BY
		                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                        ),
	                        TMP1 AS (
	                        SELECT
		                        TB.DY,
		                        SUM( TB.SHOENUM ) SHOENUM 
	                        FROM
		                        (
		                        SELECT
			                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
			                        COUNT( 1 ) shoeNum 
		                        FROM
			                        QCM_DQA_MAG_D QM 
		                        WHERE
			                        processing_record IS NOT NULL --AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
			
		                        GROUP BY
			                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) UNION ALL
		                        SELECT
			                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
			                        COUNT( 1 ) shoeNum 
		                        FROM
			                        QCM_MQA_MAG_D QM 
		                        WHERE
			                        processing_record IS NOT NULL --AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
			
		                        GROUP BY
			                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
		                        ) TB 
	                        GROUP BY
		                        TB.DY 
	                        ),
	                        TMP2 AS (
	                        SELECT
		                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
		                        COUNT( DISTINCT PROD_NO ) NUM 
	                        FROM
		                        QCM_ABNORMAL_R_M 
	                        WHERE
		                        1 = 1 --TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
		
	                        GROUP BY
		                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                        ),
	                        TMP3 AS (
	                        SELECT
		                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
		                        COUNT( 1 ) NUM 
	                        FROM
		                        QCM_ABNORMAL_R_M 
	                        WHERE
		                        1 = 1 --AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
		
		                        AND CLOSING_STATUS = '0' 
		                        AND ABNORMAL_LEVEL IN ( '1', '2' ) 
	                        GROUP BY
		                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                        ),
	                        yearData AS (
	                        SELECT
		                        TO_CHAR( ADD_MONTHS( TO_DATE( to_char( SYSDATE, 'yyyymm' ), 'yyyyMM' ), ROWNUM - 12 ), 'yyyy-MM' ) AS MONTH 
	                        FROM
		                        DUAL CONNECT BY ROWNUM <= 13 
	                        ),
	                        basedt AS (
	                        SELECT
		                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) AS month,
		                        COUNT( DISTINCT QM.SHOES_CODE ) shoeNum,
		                        COUNT( DISTINCT BD.PROD_NO ) artNum,
		                        MAX( NVL( TMPDQA.shoeNum, 0 ) ) mqaNum,
		                        MAX( NVL( TMP1.SHOENUM, 0 ) ) lcNum,
		                        MAX( NVL( TMP2.NUM, 0 ) ) artBadNum,
		                        '0' janum,
		                        '0' wjanum,
		                        MAX( NVL( TMP3.NUM, 0 ) ) badnum,
		                        '0%' Rate1,
		                        '0%' Rate2,
		                        '0%' Rate3,
		                        '0%' Rate4,
		                        '0%' Rate5 
	                        FROM
		                        QCM_MQA_MAG_D QM
		                        LEFT JOIN BDM_RD_PROD BD ON QM.SHOES_CODE = BD.SHOE_NO
		                        LEFT JOIN TMPDQA ON TMPDQA.DY = TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' )
		                        LEFT JOIN TMP1 ON TMP1.DY = TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' )
		                        LEFT JOIN TMP2 ON TMP2.DY = TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' )
		                        LEFT JOIN TMP3 ON TMP3.DY = TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                        WHERE
		                        1 = 1 --AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
		
	                        GROUP BY
		                        TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                        ) SELECT
	                        yearData.MONTH,
	                        NVL( shoeNum, 0 ) shoeNum,
	                        NVL( artNum, 0 ) artNum,
	                        NVL( mqaNum, 0 ) mqaNum,
	                        NVL( lcNum, 0 ) lcNum,
	                        NVL( artBadNum, 0 ) artBadNum,
	                        NVL( janum, 0 ) janum,
	                        NVL( wjanum, 0 ) wjanum,
	                        NVL( badnum, 0 ) badnum,
                        CASE
		
		                        WHEN NVL( mqaNum, 0 ) > 0 THEN
		                        ROUND( NVL( lcNum, 0 ) / NVL( mqaNum, 0 ) * 100, 0 ) || '%' ELSE '0%' 
	                        END Rate1,
                        CASE
	
	                        WHEN NVL( artNum, 0 ) > 0 THEN
	                        ROUND( NVL( artBadNum, 0 ) / NVL( artNum, 0 ) * 100, 0 ) || '%' ELSE '0%' 
	                        END Rate2,
                          NVL( Rate3, '0%' ) Rate3,
	                        NVL( Rate4, '0%' ) Rate4,
	                        NVL( Rate5, '0%' ) Rate5 
                        FROM
	                        yearData
	                        LEFT JOIN basedt ON basedt.month = yearData.MONTH 
                        ORDER BY
	                        MONTH DESC";
                DataTable DQADT = DB.GetDataTable(DataMQAsql);
                string abmoSQL = $@"SELECT
	                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DY,
	                                    CLOSING_STATUS,
	                                    COUNT( 1 ) NUM 
                                    FROM
	                                    QCM_ABNORMAL_R_M 
                                    WHERE
	                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                    GROUP BY
	                                    TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ),
	                                    CLOSING_STATUS";
                DataTable ABMODT = DB.GetDataTable(abmoSQL);
                foreach (DataRow item in DQADT.Rows)
                {
                    decimal JANUM = 0;
                    decimal WJANUM = 0;
                    DataRow[] selectRow = ABMODT.Select($@"DY='{item["month"].ToString()}'");
                    foreach (DataRow RItem in selectRow)
                    {
                        switch (RItem["CLOSING_STATUS"].ToString())
                        {
                            case "0":
                                WJANUM += Convert.ToDecimal(RItem["NUM"].ToString());
                                break;
                            case "1":
                                JANUM += Convert.ToDecimal(RItem["NUM"].ToString());
                                break;
                        }
                        item["janum"] = JANUM.ToString("N0");
                        item["wjanum"] = WJANUM.ToString("N0");
                        decimal Allnum = JANUM + WJANUM + Convert.ToDecimal(item["BADNUM"].ToString());
                        if (Allnum > 0)
                        {
                            item["RATE3"] = Math.Round(JANUM / Allnum * 100, 0) + "%";
                            item["RATE4"] = Math.Round(WJANUM / Allnum * 100, 0) + "%";
                            item["RATE5"] = Math.Round(Convert.ToDecimal(item["BADNUM"].ToString()) / Allnum * 100, 0) + "%";
                        }
                        

                    }

                    item["SHOENUM"] = decimal.Parse(item["SHOENUM"].ToString()).ToString("N0");
                    item["ARTNUM"] = decimal.Parse(item["ARTNUM"].ToString()).ToString("N0");
                    item["MQANUM"] = decimal.Parse(item["MQANUM"].ToString()).ToString("N0");
                    item["LCNUM"] = decimal.Parse(item["LCNUM"].ToString()).ToString("N0");
                    item["ARTBADNUM"] = decimal.Parse(item["ARTBADNUM"].ToString()).ToString("N0");


                }
                #endregion
                RetDic["MQA"] = DQADT;

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
        /// 主要品质看板-品质异常
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQualityData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase SQLSERVERDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                Dictionary<string, object> RetDic = new Dictionary<string, object>();//返回数据对象
                RetDic["x"] = string.Empty;//柱状数据源
                RetDic["bar"] = string.Empty;//柱状数据源
                RetDic["RateData"] = string.Empty;//概率计算
                RetDic["TableData"] = string.Empty;//表格数据

                #region 柱状区数据
                List<string> YBList = new List<string>();//一般
                List<string> ZDList = new List<string>();//重大
                List<string> FXList = new List<string>();//翻箱
                //SQL
                string ZZDataSql = $@"SELECT
	                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) YMDate,
	                                        ABNORMAL_LEVEL,
	                                        COUNT( 1 ) NUM,
                                            '1' FxStatus
                                        FROM
	                                        QCM_ABNORMAL_R_M 
                                        WHERE
	                                        ABNORMAL_LEVEL IN ( '0', '1', '2' ) 
	                                        AND TO_CHAR(TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
                                        GROUP BY
	                                        ABNORMAL_LEVEL,
	                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
                                        ORDER BY
	                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' )";
                DataTable Dt = DB.GetDataTable(ZZDataSql);
                // 月份
                List<string> DateYM = new List<string>();
                string Year = DateTime.Now.ToString("yyyy");
                for (int i = 1; i < 13; i++)
                {
                    DateYM.Add(Year + "-" + i.ToString().PadLeft(2, '0'));
                }
                //赋值
                foreach (var item in DateYM)
                {
                    int YBNum = 0;
                    int ZDNum = 0;
                    int FXNum = 0;
                    DataRow[] SelectRows = Dt.Select($@"YMDate='{item}'");
                    foreach (var RowItem in SelectRows)
                    {
                        //0:翻箱数为0的数据，1:翻箱数不为0
                        switch (RowItem["FxStatus"].ToString())
                        {
                            case "0":
                                FXNum += Convert.ToInt32(RowItem["NUM"].ToString());
                                break;
                            case "1":
                                //0:一般异常  2:重大异常
                                if (RowItem["ABNORMAL_LEVEL"].ToString() == "0")
                                {
                                    YBNum += Convert.ToInt32(RowItem["NUM"].ToString());
                                }
                                else if (RowItem["ABNORMAL_LEVEL"].ToString() == "1")
                                {
                                    FXNum += Convert.ToInt32(RowItem["NUM"].ToString());
                                }
                                else if (RowItem["ABNORMAL_LEVEL"].ToString() == "2")
                                {
                                    ZDNum += Convert.ToInt32(RowItem["NUM"].ToString());
                                }
                                break;
                        }
                    }
                    YBList.Add(YBNum.ToString("N0"));
                    ZDList.Add(ZDNum.ToString("N0"));
                    FXList.Add(FXNum.ToString("N0"));
                }
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "一般异常", "批量异常", "重大异常" });

                List<Dictionary<string, object>> BarList = new List<Dictionary<string, object>>();
                //一般异常
                Dictionary<string, object> YBDic = new Dictionary<string, object>();
                YBDic["name"] = lanDic["一般异常"].ToString();
                YBDic["data"] = YBList;
                BarList.Add(YBDic);
                //翻箱异常
                Dictionary<string, object> FXDic = new Dictionary<string, object>();
                FXDic["name"] = lanDic["批量异常"].ToString();
                FXDic["data"] = FXList;
                BarList.Add(FXDic);
                //重大异常
                Dictionary<string, object> ZDDic = new Dictionary<string, object>();
                ZDDic["name"] = lanDic["重大异常"].ToString();
                ZDDic["data"] = ZDList;
                BarList.Add(ZDDic);

                #endregion
                RetDic["x"] = DateYM;//柱状数据源
                RetDic["bar"] = BarList;

                #region 结案率
                string RateDataSQL = $@"SELECT
	                                        ABNORMAL_LEVEL,
	                                        TO_CHAR(COUNT( 1 ),'FM999,999,999,999,990.00') NUM,
	                                        CLOSING_STATUS 
                                        FROM
	                                        QCM_ABNORMAL_R_M 
                                        WHERE
	                                        TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                        GROUP BY
	                                        ABNORMAL_LEVEL,
	                                        CLOSING_STATUS ";
                DataTable RateDt = DB.GetDataTable(RateDataSQL);
                decimal AllYbNum = 0;//所有一般异常数据
                decimal EndYbNum = 0;//已结案的一般异常数据

                decimal AllPlNum = 0;//所有批量异常数据
                decimal EndPlNum = 0;//已结案的批量异常数据


                decimal AllZdNum = 0;//所有重大异常数据
                decimal EndZdNum = 0;//已结案的重大异常数据


                foreach (DataRow item in RateDt.Rows)
                {
                    switch (item["ABNORMAL_LEVEL"].ToString())
                    {
                        case "0":
                            AllYbNum += Convert.ToDecimal(item["NUM"].ToString());
                            if (item["CLOSING_STATUS"].ToString() == "1")
                            {
                                EndYbNum += Convert.ToDecimal(item["NUM"].ToString());
                            }
                            break;
                        case "1":
                            AllPlNum += Convert.ToDecimal(item["NUM"].ToString());
                            if (item["CLOSING_STATUS"].ToString() == "1")
                            {
                                EndPlNum += Convert.ToDecimal(item["NUM"].ToString());
                            }
                            break;
                        case "2":
                            AllZdNum += Convert.ToDecimal(item["NUM"].ToString());
                            if (item["CLOSING_STATUS"].ToString() == "1")
                            {
                                EndZdNum += Convert.ToDecimal(item["NUM"].ToString());
                            }
                            break;
                    }
                }


                List<Dictionary<string, object>> RateList = new List<Dictionary<string, object>>();
                Dictionary<string, object> YBRateDic = new Dictionary<string, object>();
                YBRateDic["name"] = lanDic["一般异常"].ToString();
                YBRateDic["percent"] = AllYbNum > 0 ? Math.Round(EndYbNum / AllYbNum * 100, 0) + "%" : "0%";
                YBRateDic["num"] = EndYbNum.ToString("N0") + "/" + AllYbNum.ToString("N0");
                RateList.Add(YBRateDic);

                Dictionary<string, object> FXRateDic = new Dictionary<string, object>();
                FXRateDic["name"] = lanDic["批量异常"].ToString();
                FXRateDic["percent"] = AllPlNum > 0 ? Math.Round(EndPlNum / AllPlNum * 100, 0) + "%" : "0%";
                FXRateDic["num"] = EndPlNum.ToString("N0") + "/" + AllPlNum.ToString("N0");
                RateList.Add(FXRateDic);

                Dictionary<string, object> ZDRateDic = new Dictionary<string, object>();
                ZDRateDic["name"] = lanDic["重大异常"].ToString();
                ZDRateDic["percent"] = AllZdNum > 0 ? Math.Round(EndZdNum / AllZdNum * 100, 0) + "%" : "0%";
                ZDRateDic["num"] = EndZdNum.ToString("N0") + "/" + AllZdNum.ToString("N0");
                RateList.Add(ZDRateDic);

               
                #endregion
                RetDic["RateData"] = RateList;

                #region 表格数据
                string TableDataDt = $@"SELECT
	                                        A.CREATEDATE,
	                                        A.ABNORMAL_LEVEL,
	                                        A.SHOE_NO,
	                                        A.PROD_NO,
	                                        A.DEPARTMENT_CODECC,
	                                        A.PLANT_AREA,
	                                        DECODE( NVL( A.CLOSING_STATUS, '0' ), '0', 'Closed_case', 'Open_case' ) CLOSING_STATUS,
	                                        BT.FILE_URL 
                                        FROM
	                                        QCM_ABNORMAL_R_M A
	                                        LEFT JOIN QCM_ABNORMAL_R_M_F AF ON  A.ID=AF.UNION_ID AND SOURCE_TYPE='0'
	                                        LEFT JOIN BDM_UPLOAD_FILE_ITEM BT ON AF.FILE_GUID=BT.GUID ORDER BY
	                                    a.CREATEDATE DESC ";
                DataTable TableDT = DB.GetDataTable(TableDataDt);
                string imageUrl = SQLSERVERDB.GetString($@"SELECT TOP 1 uploadurl FROM SYSORG01M ").Replace("/api/commoncall", "").Trim();
                foreach (DataRow item in TableDT.Rows)
                {
                    switch (item["ABNORMAL_LEVEL"].ToString())
                    {
                        case "0":
                            item["ABNORMAL_LEVEL"] = "General_exception";
                            break;
                        case "1":
                            item["ABNORMAL_LEVEL"] = "Batch_exception";
                            break;
                        case "2":
                            item["ABNORMAL_LEVEL"] = "Major_abnormality";
                            break;
                    }
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        item["FILE_URL"] = imageUrl + item["FILE_URL"].ToString();
                    }
                }
                #endregion
                RetDic["TableData"] = TableDT;

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
        /// 主要品质看板-前段Q
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFrontQData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["RateData"] = null;
                RetDic["PassTable"] = null;


                #region 概率计算
                List<Dictionary<string, object>> RateList = new List<Dictionary<string, object>>();
                //检验合格率
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "检验合格率", "测试合格率", "审核成绩" });
                Dictionary<string, object> newdic = DB.GetDictionary($@"SELECT
	                                                                        nvl( sum( m.lot_qty ), '0' ) AS 检验数量,
	                                                                        nvl( sum( ( SELECT count( 1 ) FROM rqc_task_detail_t t WHERE t.task_no = m.task_no AND commit_type = '0' ) ), '0' ) AS 合格数量 
                                                                        FROM
	                                                                        rqc_task_m m
                                                                        WHERE
	                                                                        WORKSHOP_SECTION_NO != 'L' 
	                                                                        AND TO_CHAR( TO_DATE( m.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) ");
                Dictionary<string, object> JYDic = new Dictionary<string, object>();
                JYDic["name"] = lanDic["检验合格率"].ToString();
                //JYDic["percentTitle"] = "平均值";
                JYDic["percentTitle"] = "Average value";
                JYDic["percent"] = "0%";// Math.Round(PassNum / AllNum, 3).ToString("P")
                JYDic["num"] = "0" + "/" + "0";
                if (newdic != null)
                {
                    if (Convert.ToDecimal(newdic["检验数量"].ToString()) > 0)
                    {
                        JYDic["percent"] = Math.Round(Convert.ToDecimal(newdic["合格数量"].ToString()) / Convert.ToDecimal(newdic["检验数量"].ToString()) * 100, 1) + "%";
                        JYDic["num"] = Convert.ToDecimal(newdic["合格数量"].ToString()).ToString("N0") + "/" + Convert.ToDecimal(newdic["检验数量"].ToString()).ToString("N0");
                    }
                }

                RateList.Add(JYDic);
                //测试合格率
                string RateSql = $@"SELECT
	                                    ITEM_TEST_RESULT,
	                                    COUNT( 1 ) Num
                                    FROM
	                                    QCM_EX_TASK_LIST_M QM
	                                    LEFT JOIN QCM_EX_TASK_LIST_D QD ON QM.TASK_NO = QD.TASK_NO 
                                    WHERE
	                                    TEST_TYPE IN ( '1', '2', '3' ) 
	                                    AND QD.ITEM_TEST_RESULT IS NOT NULL 
                                        AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                    GROUP BY
	                                    ITEM_TEST_RESULT";
                DataTable RateDt = DB.GetDataTable(RateSql);
                double AllNum = 0;//数量总和
                double PassNum = 0;//Pass数量
                foreach (DataRow item in RateDt.Rows)
                {
                    AllNum += Convert.ToDouble(item["Num"].ToString());
                    if (item["ITEM_TEST_RESULT"].ToString().ToLower() == "pass")
                    {
                        PassNum += Convert.ToDouble(item["Num"].ToString());
                    }
                }
                Dictionary<string, object> TestDic = new Dictionary<string, object>();
                TestDic["name"] = lanDic["测试合格率"].ToString();
                //TestDic["percentTitle"] = "平均值";
                TestDic["percentTitle"] = "Average value";
                TestDic["percent"] = Math.Round(PassNum / AllNum * 100, 1) + "%";
                TestDic["num"] = PassNum.ToString("N0") + "/" + AllNum.ToString("N0");

                RateList.Add(TestDic);

                //审核成绩
                string scoreSql = $@"WITH temp AS ( SELECT MAX( id ) id FROM QCM_AUDIT_TASK_M GROUP BY TASK_NO )
                                    SELECT
	                                    QM.TASK_NO,
	                                    MAX( BASE003M.SUPPLIERS_NAME ) name,
	                                    SUM( NVL( SCORE, 0 ) ) rate 
                                    FROM
	                                    QCM_AUDIT_TASK_D QD
	                                    LEFT JOIN ( SELECT * FROM QCM_AUDIT_TASK_M WHERE id IN ( SELECT id FROM temp ) ) QM ON QD.TASK_NO = QM.TASK_NO
	                                    LEFT JOIN BASE003M ON BASE003M.SUPPLIERS_CODE = QM.SUPPLIERS_CODE 
                                    WHERE
	                                    TO_CHAR( TO_DATE( QD.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                    GROUP BY
	                                    QM.TASK_NO,
	                                    QM.SUPPLIERS_CODE 
                                    ORDER BY
	                                    SUM( NVL( SCORE, 0 ) ) ";
                DataTable ScoreDt = DB.GetDataTable(scoreSql);
                decimal num = 0;
                foreach (DataRow item in ScoreDt.Rows)
                {
                    num += Convert.ToDecimal(item["rate"].ToString());
                }
                Dictionary<string, object> EXDic = new Dictionary<string, object>();
                EXDic["name"] = lanDic["审核成绩"].ToString();
               // EXDic["percentTitle"] = "平均分";
                EXDic["percentTitle"] = "The average score";
                if (ScoreDt.Rows.Count > 0)
                {
                    EXDic["percent"] = Math.Round(num / ScoreDt.Rows.Count, 2).ToString("N0");
                }
                else {
                    EXDic["percent"] =0;
                }
               
                EXDic["num"] = ScoreDt.Rows.Count;

                RateList.Add(EXDic);
                RetDic["RateData"] = RateList;
                #endregion

                #region 表格数据
                //string TestSql = $@"WITH temp AS (
                //                     SELECT
                //                      a.SUPPLIERS_NAME NAME,
                //                     TO_CHAR(COUNT( 1 ),'FM999,999,999,999,990.00') as PassQty,
                //                     sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) TestQty,
                //                     sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
                //                    FROM
                //                     rqc_task_m a
                //                     JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO

                //                    WHERE
                //                     substr( d.CREATEDATE, 0, 4 ) = to_char( SYSDATE, 'yyyy' ) 
                //                      AND WORKSHOP_SECTION_NO != 'L' 
                //                        AND a.SUPPLIERS_NAME is not null
                //                    GROUP BY
                //                     a.SUPPLIERS_NAME 
                //                     ) SELECT
                //                     NAME,
                //                    CASE

                //                      WHEN PassQty > 0 THEN
                //                      ROUND( testqty / PassQty * 100, 1 ) ELSE 0 
                //                     END  ||'%' rate 
                //                    FROM
                //                     temp";
                string TestSql = $@"WITH temp AS (
	                                    SELECT
		                                    a.SUPPLIERS_NAME NAME,
		                                   TO_CHAR(COUNT( 1 )) as PassQty,
	                                    sum( CASE WHEN d.commit_type = 0 THEN 1 ELSE 0 END ) TestQty,
	                                    sum( CASE WHEN d.commit_type = 1 THEN 1 ELSE 0 END ) bad_qty 
                                    FROM
	                                    rqc_task_m a
	                                    JOIN rqc_task_detail_t d ON a.TASK_NO = d.TASK_NO
	                                   
                                    WHERE
	                                    substr( d.CREATEDATE, 0, 4 ) = to_char( SYSDATE, 'yyyy' ) 
	                                     AND WORKSHOP_SECTION_NO != 'L' 
                                        AND a.SUPPLIERS_NAME is not null
                                    GROUP BY
	                                    a.SUPPLIERS_NAME 
	                                    ) SELECT
	                                    NAME,
                                    CASE
		
		                                    WHEN PassQty > 0 THEN
		                                    ROUND( testqty / PassQty * 100, 1 ) ELSE 0 
	                                    END  ||'%' rate 
                                    FROM
	                                    temp";
                DataTable TestDt = DB.GetDataTable(TestSql);

                String SuppSQL = $@"SELECT
	                                    SUPPLIERS_NAME NAME,
	                                    ROUND(
	                                    CASE
			                                    WHEN COUNT( 1 ) > 0 THEN
		                                    sum( CASE WHEN ITEM_TEST_RESULT = 'PASS' THEN 1 ELSE 0 END ) / COUNT( 1 ) * 100 ELSE 0 
	                                    END,
	                                    1 
	                                    )  ||'%' RATE 
                                    FROM
	                                    QCM_EX_TASK_LIST_M QM
	                                    LEFT JOIN QCM_EX_TASK_LIST_D QD ON QM.TASK_NO = QD.TASK_NO
	                                    LEFT JOIN BASE003M ON BASE003M.SUPPLIERS_CODE = QM.MANUFACTURER_CODE 
                                    WHERE
	                                    TEST_TYPE IN ( '1', '2', '3' ) 
	                                    AND QD.ITEM_TEST_RESULT IS NOT NULL 
	                                    AND TO_CHAR( TO_DATE( QM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                    GROUP BY
	                                    SUPPLIERS_NAME";
                DataTable SuppDt = DB.GetDataTable(SuppSQL);

                String ScoreSQL = $@"WITH temp AS ( SELECT MAX( id ) id FROM QCM_AUDIT_TASK_M GROUP BY TASK_NO ) SELECT
                                        BASE003M.SUPPLIERS_NAME name,
                                       TO_CHAR(SUM( NVL( SCORE, 0 ) ),'FM999,999,999,999,990.00') rate 
                                        FROM
	                                        QCM_AUDIT_TASK_D QD
	                                        LEFT JOIN ( SELECT * FROM QCM_AUDIT_TASK_M WHERE id IN ( SELECT id FROM temp ) ) QM ON QD.TASK_NO = QM.TASK_NO
	                                        LEFT JOIN BASE003M ON BASE003M.SUPPLIERS_CODE = QM.SUPPLIERS_CODE 
                                        WHERE
	                                        TO_CHAR( TO_DATE( QD.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                        GROUP BY
	                                        SUPPLIERS_NAME";
                DataTable SDt = DB.GetDataTable(ScoreSQL);


                Dictionary<string, object> DtList = new Dictionary<string, object>();
                DtList["DTTestTop3"] = Common.ReturnTopNumTable(TestDt, "RATE", "DESC", 3);
                DtList["DTTestlast3"] = Common.ReturnTopNumTable(TestDt, "RATE", "ASC", 3);
                DtList["DtTop3"] = Common.ReturnTopNumTable(SuppDt, "RATE", "DESC", 3);
                DtList["DtLast3"] = Common.ReturnTopNumTable(SuppDt, "RATE", "ASC", 3);
                DtList["DTScoreTop3"] = Common.ReturnTopNumTable(SDt, "RATE", "DESC", 3);
                DtList["DTScorelast3"] = Common.ReturnTopNumTable(SDt, "RATE", "ASC", 3);
                RetDic["PassTable"] = DtList;
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
        /// 主要品质看板-车间Q
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshopQData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["HistogramData1"] = null;//柱状图1
                RetDic["HistogramData2"] = null;//柱状图2
                RetDic["RateData"] = null;//概率
                RetDic["TableDT1"] = null;//表格数据1
                RetDic["TableDT2"] = null;//表格数据2

                decimal AllStopHours = 0;
                decimal AllRateAvg = 0;

                #region 柱状图1
                Dictionary<string, object> HISDic = new Dictionary<string, object>();
                HISDic["x"] = null;
                HISDic["bar"] = null;
                HISDic["line"] = null;

                List<string> DateYM = new List<string>();
                string Year = DateTime.Now.ToString("yyyy");
                for (int i = 1; i < 13; i++)
                {
                    DateYM.Add(Year + "-" + i.ToString().PadLeft(2, '0'));
                }
                //string MonthStr = "1月,2月,3月,4月,5月,6月,7月,8月,9月,10月,11月,12月";//月份Str
                string MonthStr = "January, February, March, April, May, June, July, August, September, October, November, December";//月份Str
                List<string> MonthList = MonthStr.Split(",").ToList();
                HISDic["x"] = MonthList;//X轴数据
                string Histogtam1BarSql = $@"WITH tmp AS (
	                                        SELECT
		                                        TASK_NO,
		                                        MIN( DateYm ) mintime,
		                                        MAX( DateYm ) maxtime 
	                                        FROM
		                                        (
		                                        SELECT
			                                        TASK_NO,
			                                        STOP_TYPE,
			                                        (
			                                        CASE
					
					                                        WHEN STOP_TYPE = '1' THEN
					                                        MIN( CREATEDATE ) || ' ' || MIN( CREATETIME ) ELSE MAX( CREATEDATE ) || ' ' || MAX( CREATETIME ) 
				                                        END 
				                                        ) AS DateYm 
			                                        FROM
				                                        TQC_TASK_STOPLINE_R 
			                                        GROUP BY
				                                        TASK_NO,
				                                        STOP_TYPE 
			                                        ) 
		                                        GROUP BY
			                                        TASK_NO 
		                                        ) SELECT
		                                        tm.TASK_NO,
		                                        TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) DateYM,
		                                        tmp.mintime,
		                                        tmp.maxtime 
	                                        FROM
		                                        TQC_TASK_M tm
		                                        LEFT JOIN tmp ON tmp.TASK_NO = tm.TASK_NO 
	                                        WHERE
	                                        tmp.maxtime IS NOT NULL 
	                                        AND tmp.mintime IS NOT NULL";
                DataTable TimeDt = DB.GetDataTable(Histogtam1BarSql);
                List<decimal> TimeList = new List<decimal>();

                List<string> TaskList = new List<string>();
                foreach (var item in DateYM)
                {
                    decimal Num = 0;
                    DataRow[] selectRow = TimeDt.Select($@"DateYM='{item}'");
                    foreach (DataRow row in selectRow)
                    {
                        TimeSpan Ts = Convert.ToDateTime(row["maxtime"].ToString()).Subtract(Convert.ToDateTime(row["mintime"].ToString()));
                        Num += Math.Round(Convert.ToDecimal(Ts.TotalSeconds / 3600), 3);
                        //if (TaskList.Contains(row["TASK_NO"].ToString()))
                        {
                            TaskList.Add(row["TASK_NO"].ToString());
                        }
                    }
                    TimeList.Add(Num);
                    AllStopHours += Num;
                }
                //多语言翻译
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "停线时长", "停线次数", "TQC首次合格数", "TQC首次不合格数", "RFT首次通过率", "品质异常停线", "加工RFT" });

                Dictionary<string, object> BarDic = new Dictionary<string, object>();
                BarDic["name"] = lanDic["停线时长"].ToString();
                BarDic["data"] = TimeList;
                HISDic["bar"] = BarDic;//折线图数据
                string Histogtam1LineSql = $@"WITH tmp1 AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' ) || '-' || lpad( LEVEL, 2, 0 ) YM FROM dual CONNECT BY LEVEL < 13 ),
                                                tmp2 AS (
	                                                SELECT
		                                                TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) dateYM,
		                                                COUNT( 1 ) NUM 
	                                                FROM
		                                                TQC_TASK_STOPLINE_R 
	                                                WHERE
		                                                STOP_TYPE = '1' 
		                                                AND CREATEDATE IS NOT NULL 
		                                                AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
	                                                GROUP BY
		                                                TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) 
	                                                ) SELECT
	                                                tmp1.YM,
	                                                NVL( tmp2.NUM, 0 ) NUM 
                                                FROM
	                                                tmp1
	                                                LEFT JOIN tmp2 ON tmp1.YM = tmp2.dateYM
	                                                ORDER BY tmp1.YM";
                DataTable HisLineDT = DB.GetDataTable(Histogtam1LineSql);
                Dictionary<string, object> LineDic = new Dictionary<string, object>();
                LineDic["name"] = lanDic["停线次数"].ToString();
                LineDic["data"] = TranDate("NUM", HisLineDT);
                HISDic["line"] = LineDic;//折线图数据

                #endregion
                RetDic["HistogramData1"] = HISDic;

                #region 柱状图2
                Dictionary<string, object> HISDataDic = new Dictionary<string, object>();
                HISDataDic["x"] = MonthList;
                HISDataDic["bar"] = null;
                HISDataDic["line"] = null;

                string Histogtam2DataSql = $@"WITH tb1 AS (
	                                            SELECT
		                                            TASK_NO,
		                                            COMMIT_TYPE,
		                                            COUNT( 1 ) NUM 
	                                            FROM
		                                            ( SELECT TASK_NO, COMMIT_INDEX, COMMIT_TYPE FROM TQC_TASK_COMMIT_M GROUP BY TASK_NO, COMMIT_INDEX, COMMIT_TYPE ) t 
	                                            GROUP BY
		                                            TASK_NO,
		                                            COMMIT_TYPE 
	                                            ) SELECT
	                                            TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) AS YMDate,
	                                            NVL( tb1.COMMIT_TYPE, 0 ) Ctype,
	                                            NVL(
		                                            SUM( tb1.NUM ),
		                                            0） Num 
	                                            FROM
		                                            TQC_TASK_M TM
		                                            LEFT JOIN tb1 ON tb1.TASK_NO = TM.TASK_NO 
	                                            WHERE
		                                            TASK_STATE IN ( '0','1','2','3','4' ) 
                                                    AND tm.WORKSHOP_SECTION_NO='L'
		                                            AND TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
	                                            GROUP BY
		                                            TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ),
		                                            tb1.COMMIT_TYPE 
                                            ORDER BY
	                                            YMDate";
                DataTable HisogtamDataDT = DB.GetDataTable(Histogtam2DataSql);

                List<decimal> GPNumList = new List<decimal>();// 首次检验合格
                List<decimal> BPNumList = new List<decimal>();// 首检不合格
                List<decimal> RFTPNumList = new List<decimal>(); //RFT首次通过率

                decimal RFTPassNum = 0;
                decimal RFTAllNum = 0;

                foreach (var item in DateYM)
                {
                    decimal AllNum = 0;
                    decimal FirstPassNum = 0;
                    decimal FirstFAILNum = 0;
                    DataRow[] SelectRows = HisogtamDataDT.Select($@"YMDate='{item}'");
                    foreach (DataRow RItem in SelectRows)
                    {
                        AllNum += Convert.ToDecimal(RItem["Num"].ToString());
                        RFTAllNum += Convert.ToDecimal(RItem["Num"].ToString());
                        if (RItem["Ctype"].ToString() == "0")
                        {
                            FirstPassNum += Convert.ToDecimal(RItem["Num"].ToString());
                            RFTPassNum += Convert.ToDecimal(RItem["Num"].ToString());
                        }
                        else if (RItem["Ctype"].ToString() == "1")
                        {
                            FirstFAILNum += Convert.ToDecimal(RItem["Num"].ToString());
                        }
                    }
                    GPNumList.Add(FirstPassNum);
                    BPNumList.Add(FirstFAILNum);//2023.05.18
                    //BPNumList.Add(AllNum - FirstPassNum);
                    if (AllNum > 0)
                    {
                        //RFTPNumList.Add(Math.Round((FirstPassNum / AllNum) * 100, 1));
                        RFTPNumList.Add(Math.Round((FirstPassNum / (FirstPassNum+ FirstFAILNum) ) * 100, 1));
                        AllRateAvg += Math.Round((FirstPassNum / AllNum) * 100, 1);
                    }
                    else
                    {
                        RFTPNumList.Add(0);

                    }

                }
                List<Dictionary<string, object>> barList = new List<Dictionary<string, object>>();
                List<Dictionary<string, object>> lineList = new List<Dictionary<string, object>>();
                //首次合格数
                Dictionary<string, object> GPDic = new Dictionary<string, object>();
                GPDic["name"] = lanDic["TQC首次合格数"].ToString();
                GPDic["data"] = GPNumList;
                //首次不合格数
                Dictionary<string, object> BPDic = new Dictionary<string, object>();
                BPDic["name"] = lanDic["TQC首次不合格数"].ToString();
                BPDic["data"] = BPNumList;

                barList.Add(GPDic);
                barList.Add(BPDic);
                HISDataDic["bar"] = barList;

                Dictionary<string, object> RFTDic = new Dictionary<string, object>();
                RFTDic["name"] = lanDic["RFT首次通过率"].ToString();
                RFTDic["data"] = RFTPNumList;
                lineList.Add(RFTDic);
                HISDataDic["line"] = lineList;

                #endregion
                RetDic["HistogramData2"] = HISDataDic;

                #region 概率
                List<IDictionary<string, object>> ListDic = new List<IDictionary<string, object>>();
                Dictionary<string, object> NewDic1 = new Dictionary<string, object>();
                NewDic1["name"] = lanDic["品质异常停线"].ToString();
                NewDic1["avg"] = "";
               // NewDic1["data"] = Math.Round(AllStopHours /73, 1) + "H/线";
                NewDic1["data"] = Math.Round(AllStopHours /73, 1) + "H/Line";
                NewDic1["Ratiodata"] = AllStopHours + "/" + 73;
                ListDic.Add(NewDic1);
                Dictionary<string, object> NewDic2 = new Dictionary<string, object>();

                #region MyRegion
                string TQCSQL = $@"WITH qtyTemp AS (
	                                SELECT
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                FROM
		                                tqc_task_commit_m 
	                                WHERE
		                                commit_type IN ( '0', '1', '2', '3', '4' ) 
	                                GROUP BY
		                                TASK_NO,
		                                COMMIT_INDEX,
		                                COMMIT_TYPE 
	                                ),
	                                baseDt AS (
	                                SELECT
		                                TASK_NO,
		                                a.WORKSHOP_SECTION_NO 
	                                FROM
		                                TQC_TASK_M a
		                                JOIN QCM_DQA_MAG_M b ON a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO 
		                                AND a.SHOE_NO = b.SHOES_CODE 
	                                WHERE
		                                TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
		                                -- AND WORKSHOP_SECTION_NAME IN ( '加工', '贴底' ) 
                                           and a.WORKSHOP_SECTION_NO in ('L','T')
	                                ) SELECT
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                COUNT( 1 ) NUM,
	                                basedt.WORKSHOP_SECTION_NO 
                                FROM
	                                qtyTemp
	                                JOIN basedt ON qtyTemp.TASK_NO = basedt.TASK_NO 
                                GROUP BY
	                                qtyTemp.TASK_NO,
	                                qtyTemp.COMMIT_TYPE,
	                                basedt.WORKSHOP_SECTION_NO";
                DataTable TQCDT = DB.GetDataTable(TQCSQL);

                int j = TQCDT.Rows.Count;
                List<string> QueryStr = new List<string> { "T", "L" };
                decimal bottomPNum = 0;
                decimal bottomCNum = 0;
                decimal machiningCNum = 0;
                decimal machiningPNum = 0;
                foreach (var item in QueryStr)
                {
                    DataRow[] QueryRow = TQCDT.Select($@"WORKSHOP_SECTION_NO='{item}'");
                    foreach (DataRow Ritem in QueryRow)
                    {
                        if (Ritem["COMMIT_TYPE"].ToString() == "0")
                        {
                            if (item == "T")
                            {
                                bottomCNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                            else if (item == "L")
                            {
                                machiningCNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                        }
                       if (Ritem["COMMIT_TYPE"].ToString() == "0" || Ritem["COMMIT_TYPE"].ToString() == "1")   
                       
                        {
                            if (item == "T")
                            {
                                bottomPNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                            else if (item == "L")
                            {
                                machiningPNum += Convert.ToDecimal(Ritem["NUM"].ToString());
                            }
                        }

                    }
                }
                #endregion

                NewDic2["name"] = lanDic["加工RFT"].ToString();
                NewDic2["avg"] = "";
                NewDic2["data"] = RFTAllNum > 0 ? Math.Round(machiningCNum / machiningPNum * 100, 1) + "%" : "0%";
                NewDic2["Ratiodata"] = machiningCNum.ToString("N0") + "/" + machiningPNum.ToString("N0");
                ListDic.Add(NewDic2);
                RetDic["RateData"] = ListDic;
                #endregion


                #region 表格数据1（产线）
                Dictionary<string, Object> dt1Dic = new Dictionary<string, Object>();
                dt1Dic["linehours"] = null;
                dt1Dic["lineRate"] = null;
                string TableLineHoursDataDTSQL = $@"SELECT
	                                            DEPARTMENT_NAME,
	                                            SUM(allHours) allHours 
                                            FROM
	                                            (
		                                            WITH tmp AS (
		                                            SELECT
			                                            TASK_NO,
			                                            MIN( DateYm ) mintime,
			                                            MAX( DateYm ) maxtime 
		                                            FROM
			                                            (
			                                            SELECT
				                                            TASK_NO,
				                                            STOP_TYPE,
				                                            (
				                                            CASE
						
						                                            WHEN STOP_TYPE = '1' THEN
						                                            MIN( CREATEDATE ) || ' ' || MIN( CREATETIME ) ELSE MAX( CREATEDATE ) || ' ' || MAX( CREATETIME ) 
					                                            END 
					                                            ) AS DateYm 
				                                            FROM
					                                            TQC_TASK_STOPLINE_R 
				                                            GROUP BY
					                                            TASK_NO,
					                                            STOP_TYPE 
				                                            ) 
			                                            GROUP BY
				                                            TASK_NO 
			                                            ) SELECT
			                                            tm.TASK_NO,
			                                            TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ),
			                                            tmp.mintime,
			                                            tmp.maxtime,
			                                            tm.PRODUCTION_LINE_CODE,
			                                            BASE005M.DEPARTMENT_NAME,
			                                            tm.SHOE_NO,
			                                            ROUND( TO_NUMBER( TO_DATE( tmp.maxtime, 'yyyy-mm-dd HH24:mi:ss' ) - TO_DATE( tmp.mintime, 'yyyy-mm-dd HH24:mi:ss' ) ) * 24 ,1 ) allHours
		                                            FROM
			                                            TQC_TASK_M tm
			                                            LEFT JOIN tmp ON tmp.TASK_NO = tm.TASK_NO 
			                                            LEFT JOIN BASE005M ON BASE005M.DEPARTMENT_CODE=TM.PRODUCTION_LINE_CODE
		                                            WHERE
			                                            tmp.maxtime IS NOT NULL 
			                                            AND tmp.mintime IS NOT NULL 
			                                            AND TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) = TO_CHAR( SYSDATE, 'yyyy-mm' ) 
		                                            ) 
                                            GROUP BY DEPARTMENT_NAME";
                dt1Dic["linehours"] = Common.ReturnOrderByTable(DB.GetDataTable(TableLineHoursDataDTSQL), "allHours", "desc");

                dt1Dic["lineRate"] = ReturnRateData("DEPARTMENT_NAME", DB);
                #endregion
                RetDic["TableDT1"] = dt1Dic;
                #region 表格数据2（鞋型）
                Dictionary<string, Object> dt2Dic = new Dictionary<string, Object>();
                dt2Dic["linehours"] = null;
                dt2Dic["lineRate"] = null;
                string TableShoeHourDataDTSQL = $@"SELECT
	                                            SHOE_NO,
	                                            SUM(allHours) allHours 
                                            FROM
	                                            (
		                                            WITH tmp AS (
		                                            SELECT
			                                            TASK_NO,
			                                            MIN( DateYm ) mintime,
			                                            MAX( DateYm ) maxtime 
		                                            FROM
			                                            (
			                                            SELECT
				                                            TASK_NO,
				                                            STOP_TYPE,
				                                            (
				                                            CASE
						
						                                            WHEN STOP_TYPE = '1' THEN
						                                            MIN( CREATEDATE ) || ' ' || MIN( CREATETIME ) ELSE MAX( CREATEDATE ) || ' ' || MAX( CREATETIME ) 
					                                            END 
					                                            ) AS DateYm 
				                                            FROM
					                                            TQC_TASK_STOPLINE_R 
				                                            GROUP BY
					                                            TASK_NO,
					                                            STOP_TYPE 
				                                            ) 
			                                            GROUP BY
				                                            TASK_NO 
			                                            ) SELECT
			                                            tm.TASK_NO,
			                                            TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ),
			                                            tmp.mintime,
			                                            tmp.maxtime,
			                                            tm.PRODUCTION_LINE_CODE,
			                                            BASE005M.DEPARTMENT_NAME,
			                                            tm.SHOE_NO,
			                                            ROUND( TO_NUMBER( TO_DATE( tmp.maxtime, 'yyyy-mm-dd HH24:mi:ss' ) - TO_DATE( tmp.mintime, 'yyyy-mm-dd HH24:mi:ss' ) ) * 24 ,1) allHours
		                                            FROM
			                                            TQC_TASK_M tm
			                                            LEFT JOIN tmp ON tmp.TASK_NO = tm.TASK_NO 
			                                            LEFT JOIN BASE005M ON BASE005M.DEPARTMENT_CODE=TM.PRODUCTION_LINE_CODE
		                                            WHERE
			                                            tmp.maxtime IS NOT NULL 
			                                            AND tmp.mintime IS NOT NULL 
			                                            AND TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) = TO_CHAR( SYSDATE, 'yyyy-mm' ) 
		                                            ) 
                                            GROUP BY SHOE_NO";
                dt2Dic["linehours"] = Common.ReturnOrderByTable(DB.GetDataTable(TableShoeHourDataDTSQL), "allHours", "desc");
                dt2Dic["lineRate"] = ReturnRateData("SHOE_NO", DB);
                #endregion
                RetDic["TableDT1"] = dt1Dic;
                RetDic["TableDT2"] = dt2Dic;
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
        /// 主要品质看板-AQL验货
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLTestData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["ColumnarData"] = null;//柱状数据
                RetDic["RateData"] = null;//概率数据
                RetDic["TableData"] = null;//表格数据

                //多语言翻译
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "PASS数量", "FAIL数量", "未验货数量" });

                #region 柱状数据
                Dictionary<string, object> ZZDic = new Dictionary<string, object>();
                ZZDic["x"] = null;//x轴
                ZZDic["bar"] = null;//柱状图
                ZZDic["line"] = null;//折线图

                //X轴
                List<string> DateYM = new List<string>();
                string Year = DateTime.Now.ToString("yyyy");
                for (int i = 1; i < 13; i++)
                {
                    DateYM.Add(Year + "-" + i.ToString().PadLeft(2, '0'));
                }
                //string MonthStr = "1月,2月,3月,4月,5月,6月,7月,8月,9月,10月,11月,12月";//月份Str
                string MonthStr = "January, February, March, April, May, June, July, August, September, October, November, December";//月份Str
                List<string> MonthList = MonthStr.Split(",").ToList();
                ZZDic["x"] = MonthList;

                string ZZDataSql = $@"WITH tmp AS (
	                                    SELECT
		                                    SUM( a.bad_qty ) AS bad_qty,
		                                    a.task_no 
	                                    FROM
		                                    aql_cma_task_list_m_aql_e_br a
		                                    LEFT JOIN aql_cma_task_list_m_aql_e_br_f b ON a.task_no = b.task_no 
		                                    AND a.bad_classify_code = b.bad_classify_code 
		                                    AND a.bad_item_code = b.bad_item_code 
	                                    WHERE
		                                    a.problem_level IN ( '0', '1', '2' ) 
	                                    GROUP BY
		                                    a.task_no 
	                                    )
                                    SELECT
	                                    a.inspection_date,--验货日期
	                                    a.lot_num,--分批数量
	                                    '' AS Sampling_quantity,--抽验双数
	                                    '' AS inspection_results,--验货结果
	                                    NVL( c.sample_level, '2' ) sample_level,
	                                    NVL( c.aql_level, 'AC13' ) aql_level,
	                                    NVL( TMP.BAD_QTY, 0 ) BAD_QTY,
	                                    a.inspection_state,
	                                    TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) dateYm 
                                    FROM
	                                    aql_cma_task_list_m a
	                                    LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no = a.task_no
	                                    LEFT JOIN TMP ON TMP.task_no = A.TASK_NO
                                    WHERE
	                                    TO_CHAR( TO_DATE( a.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )";
                DataTable AQLDT = DB.GetDataTable(ZZDataSql);
                List<string> ACList = new List<string>();//查询的AC数组
                foreach (DataRow item in AQLDT.Rows)
                {
                    if (!ACList.Contains(item["aql_level"].ToString()))
                    {
                        ACList.Add(item["aql_level"].ToString());
                    }
                }
                string ACSQL = $@"SELECT
	                                VALS,
                                    {string.Join(",", ACList)},
	                                to_number(START_QTY) START_QTY,
	                                to_number(END_QTY) END_QTY,
	                                LEVEL_TYPE 
                                FROM
	                                BDM_AQL_M 
                                WHERE
	                                HORI_TYPE = '2'";
                DataTable ACDT = DB.GetDataTable(ACSQL);

                foreach (DataRow item in AQLDT.Rows)
                {
                    DataRow[] QueryName = ACDT.Select($@" LEVEL_TYPE='{item["sample_level"].ToString()}' and START_QTY<={Convert.ToDecimal(item["lot_num"].ToString())} and END_QTY>={Convert.ToDecimal(item["lot_num"].ToString())}");
                    if (QueryName.Count() > 0)
                    {
                        int acInt = 0;
                        bool cRes = int.TryParse(QueryName[0][item["aql_level"].ToString()].ToString(), out acInt);
                        if (cRes)
                        {
                            if (!string.IsNullOrEmpty(item["inspection_date"].ToString()))
                            {
                                if (Convert.ToInt32(item["BAD_QTY"].ToString()) > acInt)
                                    item["inspection_results"] = "FAIL";
                                else
                                    item["inspection_results"] = "PASS";
                            }
                        }
                    }
                }


                #region 变量定义
                List<decimal> PassList = new List<decimal>();//检验合格
                List<decimal> FailList = new List<decimal>();//检验不合格
                List<decimal> NullList = new List<decimal>();//检验空值
                List<decimal> PassRateList = new List<decimal>();//通过率
                decimal PassNum = 0;//PASS总数
                decimal FailNum = 0;//FAIL总数
                decimal NullNum = 0;//空值总数
                decimal AllNum = 0;//总数
                #endregion



                foreach (var DateItem in DateYM)
                {
                    decimal PNum = 0;//PASS总数
                    decimal FNum = 0;//FAIL总数
                    decimal NNum = 0;//空值总数
                    decimal ANum = 0;//总数
                    DataRow[] DRS = AQLDT.Select($@"dateYm='{DateItem}'");
                    foreach (DataRow item in DRS)
                    {
                        ANum++;
                        switch (item["inspection_results"].ToString().ToLower().Trim())
                        {
                            case "fail":
                                FNum++;
                                break;
                            case "pass":
                                PNum++;
                                break;
                            case "":
                                NNum++;
                                break;
                        }
                    }
                    PassNum += PNum;
                    FailNum += FNum;
                    NullNum += NNum;
                    AllNum += ANum;
                    PassList.Add(PNum);
                    FailList.Add(FNum);
                    NullList.Add(NNum);
                    if (PNum + FNum > 0)
                    {
                        PassRateList.Add(Math.Round(PNum / (FNum + PNum) * 100, 2));
                    }
                    else
                    {
                        PassRateList.Add(0);
                    }

                }
                //柱状
                List<Dictionary<string, object>> barList = new List<Dictionary<string, object>>();
                Dictionary<string, object> PassBar = new Dictionary<string, object>();
                PassBar["name"] = lanDic["PASS数量"].ToString();
                PassBar["data"] = PassList;
                barList.Add(PassBar);
                Dictionary<string, object> FailBar = new Dictionary<string, object>();
                FailBar["name"] = lanDic["FAIL数量"].ToString();
                FailBar["data"] = FailList;
                barList.Add(FailBar);
                Dictionary<string, object> NullBar = new Dictionary<string, object>();
                NullBar["name"] = lanDic["未验货数量"].ToString();
                NullBar["data"] = NullList;
                barList.Add(NullBar);
                ZZDic["bar"] = barList;

                //折线
                List<Dictionary<string, object>> lineList = new List<Dictionary<string, object>>();
                Dictionary<string, object> PassRateBar = new Dictionary<string, object>();
                //PassRateBar["name"] = "PASS数量占比";
                PassRateBar["name"] = "Proportion of PASS quantity";
                PassRateBar["data"] = PassRateList;
                lineList.Add(PassRateBar);
                ZZDic["line"] = lineList;
                #endregion
                RetDic["ColumnarData"] = ZZDic;//柱状数据

                #region 概率
                List<Dictionary<string, object>> RateList = new List<Dictionary<string, object>>();
                Dictionary<string, object> FinshData = new Dictionary<string, object>();
               // FinshData["name"] = "验货完成率";
                FinshData["name"] = "Inspection completion rate";
                if (AllNum > 0)
                {
                    FinshData["percent"] = Math.Round((PassNum + FailNum) / AllNum * 100, 1) + "%";
                    FinshData["num"] = (PassNum + FailNum).ToString("N0") + "/" + AllNum.ToString("N0");
                }
                else
                {
                    FinshData["percent"] = "0.00%";
                    FinshData["num"] = "0/0";
                }
                RateList.Add(FinshData);

                Dictionary<string, object> PassData = new Dictionary<string, object>();
                //PassData["name"] = "验货通过率";
                PassData["name"] = "Inspection pass rate";
                if (AllNum > 0)
                {
                    PassData["percent"] = Math.Round(PassNum / (PassNum + FailNum) * 100, 1) + "%";
                    PassData["num"] = PassNum.ToString("N0") + "/" + (PassNum + FailNum).ToString("N0");
                }
                else
                {
                    PassData["percent"] = "0.00%";
                    PassData["num"] = "0/0";
                }
                RateList.Add(PassData);
                #endregion
                RetDic["RateData"] = RateList;

                #region 表格数据
                Dictionary<string, object> TableList = new Dictionary<string, object>();
                DataTable TableData1 = DB.GetDataTable($@"SELECT
	                                                            BASE005M.DEPARTMENT_NAME AS NAME,
	                                                            SUM( FX_QTY ) AS NUM 
                                                            FROM
	                                                            QCM_ABNORMAL_R_M 
	                                                            JOIN BASE005M ON BASE005M.DEPARTMENT_CODE=QCM_ABNORMAL_R_M.PRODUCTION_LINE_CODE
                                                            WHERE
	                                                            ABNORMAL_CATEGORY_NO = '008' 
	                                                            AND FX_QTY > 0 
	                                                            AND TO_CHAR( TO_DATE( QCM_ABNORMAL_R_M.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                                            GROUP BY
	                                                            BASE005M.DEPARTMENT_NAME");
                TableData1 = Common.ReturnOrderByTable(TableData1, "NUM", "DESC");
                DataTable TableData2 = DB.GetDataTable($@"SELECT
	                                                        PROBLEM_DESC AS NAME,
	                                                        SUM( FX_QTY ) AS NUM 
                                                        FROM
	                                                        QCM_ABNORMAL_R_M 
                                                        WHERE
	                                                        ABNORMAL_CATEGORY_NO = '008' 
                                                            and FX_QTY>0 
	                                                        AND TO_CHAR( TO_DATE( CREATEDATE, 'yyyy-mm-dd' ), 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' ) 
                                                        GROUP BY
	                                                        PROBLEM_DESC");
                TableData2 = Common.ReturnOrderByTable(TableData2, "NUM", "DESC");
                TableList["tb1"] = TableData1;
                TableList["tb2"] = TableData2;

                #endregion
                RetDic["TableData"] = TableList;

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
        /// 取dataTable中其中一列转换为List
        /// </summary>
        /// <param name="RowName"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<string> TranDate(string RowName, DataTable dt)
        {
            List<string> RetList = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                RetList.Add(Convert.ToDecimal(item[RowName].ToString()).ToString("N0"));
            }
            return RetList;
        }
        public static List<decimal> PerentDatePlan(DataTable FMDicList, DataTable FZDicList)
        {
            List<decimal> RetList = new List<decimal>();
            foreach (DataRow FMitem in FMDicList.Rows)
            {
                foreach (DataRow FZitem in FZDicList.Rows)
                {
                    if (FMitem["YM"].ToString() == FZitem["YM"].ToString())
                    {
                        decimal FMQty = Convert.ToDecimal(FMitem["qty"].ToString());
                        decimal FZQty = Convert.ToDecimal(FZitem["qty"].ToString());
                        if (FMQty <= 0)
                        {
                            RetList.Add(0);
                        }
                        else
                        {
                            RetList.Add(Math.Round((FZQty / FMQty) * 100, 2));
                        }
                        break;
                    }
                }
            }
            return RetList;
        }

        /// <summary>
        /// 封装数据返回
        /// </summary>
        /// <param name="RowName">列名【BASE005M.DEPARTMENT_NAME】【SHOE_NO】</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static DataTable ReturnRateData(string RowName, SJeMES_Framework_NETCore.DBHelper.DataBase DB)
        {
            string TableLineRateDataDTSQL = $@"WITH tb1 AS (
	                                                SELECT
		                                                TASK_NO,
		                                                COMMIT_TYPE,
		                                                COUNT( 1 ) NUM 
	                                                FROM
		                                                ( SELECT TASK_NO, COMMIT_INDEX, COMMIT_TYPE FROM TQC_TASK_COMMIT_M GROUP BY TASK_NO, COMMIT_INDEX, COMMIT_TYPE ) t 
	                                                GROUP BY
		                                                TASK_NO,
		                                                COMMIT_TYPE 
	                                                ) SELECT
	                                               {RowName},
	                                                NVL( tb1.COMMIT_TYPE, 0 ) Ctype,
	                                                NVL(
		                                                SUM( tb1.NUM ),
		                                                0） Num 
	                                                FROM
		                                                TQC_TASK_M TM
		                                                LEFT JOIN tb1 ON tb1.TASK_NO = TM.TASK_NO 
		                                                LEFT JOIN BASE005M on  BASE005M.DEPARTMENT_CODE=TM.PRODUCTION_LINE_CODE
	                                                WHERE
		                                                TASK_STATE IN ( '0', '2' )  
                                                        AND TM.WORKSHOP_SECTION_NO='L'
		                                                AND TO_CHAR( TO_DATE( TM.CREATEDATE, 'yyyy-mm-dd' ), 'yyyy-mm' ) = TO_CHAR( SYSDATE, 'yyyy-mm' ) 
	                                                GROUP BY
		                                                tb1.COMMIT_TYPE ,{RowName}";
            DataTable LineRate = Common.ReturnOrderByTable(DB.GetDataTable(TableLineRateDataDTSQL), "NUM", "asc");
            List<string> lineRateList = new List<string>();
            List<Dictionary<string, object>> LineRateDicList = new List<Dictionary<string, object>>();
            foreach (DataRow item in LineRate.Rows)
            {
                if (!lineRateList.Contains(item[RowName].ToString()))
                {
                    lineRateList.Add(item[RowName].ToString());
                }
            }
            DataTable Dt = new DataTable();
            Dt.Columns.Add(RowName);
            Dt.Columns.Add("RATE");
            foreach (var item in lineRateList)
            {
                DataRow NewRow = Dt.Rows.Add();
                NewRow[RowName] = item;
                decimal paseRate = 0;
                decimal allRate = 0;
                DataRow[] Select = LineRate.Select($@"{RowName}='{item}'");
                foreach (DataRow Ritem in Select)
                {
                    allRate += Convert.ToDecimal(Ritem["Num"].ToString());
                    if (Ritem["Ctype"].ToString() == "0")
                    {
                        paseRate += Convert.ToDecimal(Ritem["Num"].ToString());
                    }
                }
                if (allRate > 0)
                {
                    NewRow["RATE"] = Math.Round(paseRate / allRate * 100, 1) + "%";
                }
                else
                {
                    NewRow["RATE"] = "0%";
                }
            }
            DataView DV = new DataView(Dt);
            DV.Sort = "RATE ASC";
            return DV.ToTable();
        }

        public static DataTable ReturnDt(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string WhereSql)
        {
            string sql = $@"SELECT
	                            DEVELOP_SEASON,
	                            COUNT( * ) AS CNT 
                            FROM
	                            (
	                            SELECT
		                            (
		                            SELECT
			                            DEVELOP_SEASON 
		                            FROM
			                            BDM_RD_PROD bd 
		                            WHERE
			                            bd.SHOE_NO = b.SHOES_CODE 
			                            AND SUBSTR( DEVELOP_SEASON, 3, 2 ) = TO_NUMBER( SUBSTR( TO_CHAR( SYSDATE, 'yyyy' ), 3, 2 ) ) + 1 
			                            AND ROWNUM = 1 
		                            ) AS DEVELOP_SEASON,
		                            a.* 
	                            FROM
		                            qcm_shoes_qa_record_item a
		                            JOIN ( SELECT max( id ) AS id, shoes_code FROM qcm_shoes_qa_record_d  GROUP BY shoes_code ) b ON a.d_id = b.id 
		                            {WhereSql}
	                            ) 
                            GROUP BY
	                            DEVELOP_SEASON";
            return DB.GetDataTable(sql);
        }
    }
}
