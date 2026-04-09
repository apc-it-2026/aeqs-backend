using Org.BouncyCastle.Crypto.Agreement.Srp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// 测试品质看板
    /// </summary>
    internal class TestQuality
    {
        static string ui_lan_type = "en";
        static string moudle_code = "testNalysis";
        /// <summary>
        /// 测试工作量分析
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestWorkloadData(object OBJ)
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
                ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "en";
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
                string quarter = jarr.ContainsKey("quarter") ? jarr["quarter"].ToString() : "";//季度
                string category = jarr.ContainsKey("category") ? jarr["category"].ToString() : "";//category
                string shoeName = jarr.ContainsKey("shoeName") ? jarr["shoeName"].ToString() : "";//鞋型名称
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string testDepartment = jarr.ContainsKey("testDepartment") ? jarr["testDepartment"].ToString() : "";//送测部门
                string stage = jarr.ContainsKey("stage") ? jarr["stage"].ToString() : "";//阶段
                string testType = jarr.ContainsKey("testType") ? jarr["testType"].ToString() : "";//实验室送测类型
                string testItems = jarr.ContainsKey("testItems") ? jarr["testItems"].ToString() : "";//测试项目
                string testResult = jarr.ContainsKey("testResult") ? jarr["testResult"].ToString() : "";//测试结果
                string[] orderListquarter = quarter.Split(',');
                string[] orderListcategory = category.Split(',');
                string[] orderListshoeName = shoeName.Split(',');
                string[] orderListart = art.Split(',');
                string[] orderListDept = testDepartment.Split(',');
                string[] orderListstage = stage.Split(',');
                string[] orderListtestItems = testItems.Split(',');
                #endregion

                #region 参数验证
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                {
                    throw new Exception("Missing prerequisites: start time and end time!");
                }
                #endregion


                #region 组合查询SQL
                string LabLineWhereSql = string.Empty;
                string LabCakeWhereSql = string.Empty;
                string WareLineWhereSql = string.Empty;
                string WareCakeWhereSql = string.Empty;
                string DepartLineWhereSql = string.Empty;
                string ArtLineWhereSql = string.Empty;
                string DepartTbWhereSql = string.Empty;
                string ArtTbWhereSql = string.Empty;
                string testWhere = string.Empty; 

                //多选 2023.5.10
                if (!string.IsNullOrEmpty(art))
                {
                    LabLineWhereSql = $@" AND a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    LabCakeWhereSql = $@" AND M.ART_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    WareLineWhereSql = $@" AND a.ARTNO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    WareCakeWhereSql = $@" AND b.ARTNO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    DepartLineWhereSql = $@" AND a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    ArtLineWhereSql = $@" AND a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql = $@" AND a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql = $@" AND a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(testDepartment))
                {
                    DepartLineWhereSql += $@" and a.staff_department in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql += $@" and a.staff_department in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql += $@" and a.staff_department in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(stage))//阶段
                {
                    DepartLineWhereSql += $@" and a.PHASE_CREATION_NAME in ({string.Join(',', orderListstage.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql += $@" and a.PHASE_CREATION_NAME in ({string.Join(',', orderListstage.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql += $@" and a.PHASE_CREATION_NAME in ({string.Join(',', orderListstage.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(quarter))
                {
                    LabLineWhereSql += $@" and a.SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                    LabCakeWhereSql += $@" and m.SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                    DepartLineWhereSql += $@" and a.SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                    ArtLineWhereSql += $@" and b.DEVELOP_SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql += $@" and a.SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql += $@" and a.SEASON in ({string.Join(',', orderListquarter.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(category))
                {
                    LabLineWhereSql += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    LabCakeWhereSql += $@" and m.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    DepartLineWhereSql += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    ArtLineWhereSql += $@" and BC.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }

                if (!string.IsNullOrEmpty(shoeName))
                {
                    LabLineWhereSql += $@" and a.shoe_no in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    LabCakeWhereSql += $@" and m.shoe_no in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    DepartLineWhereSql += $@" and a.shoe_no in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    ArtLineWhereSql += $@" and B.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    DepartTbWhereSql += $@" and a.shoe_no in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    ArtTbWhereSql += $@" and a.shoe_no in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(testType))
                {
                    LabLineWhereSql += $@" and a.TEST_TYPE = '{testType}'";
                    LabCakeWhereSql += $@" and m.TEST_TYPE = '{testType}'";
                    DepartLineWhereSql += $@" and a.TEST_TYPE = '{testType}'";
                    ArtLineWhereSql += $@" and a.TEST_TYPE = '{testType}'";
                    DepartTbWhereSql += $@" and a.TEST_TYPE = '{testType}'";
                    ArtTbWhereSql += $@" and a.TEST_TYPE = '{testType}'";
                }
                if (!string.IsNullOrEmpty(testResult))
                {
                    switch (testResult)
                    {
                        case "0":
                            testWhere += $@" AND PASS_RATE >= 100";
                            break;
                        case "1":
                            testWhere += $@" AND PASS_RATE < 100";
                            break;
                    }
                }

                #endregion
                #region 单选where条件（废弃）
                /*
                if (!string.IsNullOrEmpty(art))
                {
                    LabLineWhereSql = $@" AND a.art_no='{art}'";
                    LabCakeWhereSql = $@" AND M.ART_NO ='{art}'";
                    WareLineWhereSql = $@" AND a.ARTNO ='{art}'";
                    WareCakeWhereSql = $@" AND b.ARTNO ='{art}'";
                    DepartLineWhereSql = $@" AND a.art_no ='{art}'";
                    ArtLineWhereSql = $@" AND a.art_no ='{art}'";
                    DepartTbWhereSql = $@" AND a.art_no ='{art}'";
                    ArtTbWhereSql = $@" AND a.art_no ='{art}'";
                }
                if (!string.IsNullOrEmpty(testDepartment))
                {
                    DepartLineWhereSql += $@" and a.staff_department ='{testDepartment}'";
                    DepartTbWhereSql += $@" and a.staff_department ='{testDepartment}'";
                }
                if (!string.IsNullOrEmpty(quarter))
                {
                    LabLineWhereSql += $@" and a.SEASON like '{quarter}%'";
                    LabCakeWhereSql += $@" and m.SEASON like '{quarter}%'";
                    DepartLineWhereSql += $@" and a.SEASON like '{quarter}%'";
                    ArtLineWhereSql += $@" and b.DEVELOP_SEASON like '{quarter}%'";
                    DepartTbWhereSql += $@" and a.SEASON like '{quarter}%'";
                    ArtTbWhereSql += $@" and a.SEASON like '{quarter}%'";
                }
                if (!string.IsNullOrEmpty(category))
                {
                    LabLineWhereSql += $@" and a.CATEGORY_NAME like '{category}%'";
                    LabCakeWhereSql += $@" and m.CATEGORY_NAME like '{category}%'";
                    DepartLineWhereSql += $@" and a.CATEGORY_NAME like '{category}%'";
                    ArtLineWhereSql += $@" and BC.NAME_T like '{category}%'";
                    DepartTbWhereSql += $@" and a.CATEGORY_NAME like '{category}%'";
                    ArtTbWhereSql += $@" and a.CATEGORY_NAME like '{category}%'";
                }

                if (!string.IsNullOrEmpty(shoeName))
                {
                    LabLineWhereSql += $@" and a.shoe_no like '{shoeName}%'";
                    LabCakeWhereSql += $@" and m.shoe_no like '{shoeName}%'";
                    DepartLineWhereSql += $@" and a.shoe_no like '{shoeName}%'";
                    ArtLineWhereSql += $@" and B.NAME_T like '{shoeName}%'";
                    DepartTbWhereSql += $@" and a.shoe_no like '{shoeName}%'";
                    ArtTbWhereSql += $@" and a.shoe_no like '{shoeName}%'";
                }*/
                #endregion


                string startYear = DateTime.Now.ToString("yyyy-01-01");
                string endYear = Convert.ToDateTime(DateTime.Now.AddYears(1).ToString("yyyy-01-01")).AddDays(-1).ToString("yyyy-MM-dd");

                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["Laboratory"] = null;//试穿报告—实验室
                RetDic["WearingTest"] = null;//试穿报告—试穿测试
                RetDic["OtherData"] = null;//送测部门以及ART
                RetDic["AnalysisList"] = null;//分析列表

                #region 试穿报告—实验室
                Dictionary<string, object> labDic = new Dictionary<string, object>();
                labDic["month"] = null;//月份数据
                labDic["day"] = null;//每日数据
                //柱状数据（固定年）
                string LabMonthSql = $@"WITH tmp1 AS (
	                                        SELECT
		                                        to_char( to_date( CREATEDATE, 'yyyy-MM-dd' ), 'yyyy-mm' ) CREATEDATE,
	                                        sum( CASE WHEN a.TEST_RESULT = 'PASS' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) pass_qty,
	                                        sum( CASE WHEN a.TEST_RESULT = 'FAIL' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) bad_qty 
                                        FROM
	                                        qcm_ex_task_list_m a 
                                        WHERE
	                                        nvl( a.TEST_RESULT, 'NULL' ) != 'NULL' 
	                                        AND a.CREATEDATE BETWEEN '{startYear}' 
	                                        AND '{endYear}'{LabLineWhereSql}
                                        GROUP BY
	                                        to_char( to_date( CREATEDATE, 'yyyy-MM-dd' ), 'yyyy-mm' ) 
	                                        ) SELECT
	                                        CREATEDATE,
	                                        pass_qty,
	                                        bad_qty,
	                                        CASE
		
		                                        WHEN pass_qty + bad_qty > 0 THEN
		                                        round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
	                                        END AS pass_rate 
                                        FROM
	                                        tmp1";
                
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code,new List<string>(){ "PASS测试报告份数", "FAIL测试报告份数", "PASS比率" });

                labDic["month"] = RetunHisData(DB, ReturnData("m", startYear, endYear), LabMonthSql, "CREATEDATE", "pass_qty", "bad_qty", "pass_rate", lanDic["PASS测试报告份数"].ToString() , lanDic["FAIL测试报告份数"].ToString(), lanDic["PASS比率"].ToString());
               
                //柱状数据（日）
                string LabDaySql = $@"WITH tmp1 AS (
	                                        SELECT
		                                        a.CREATEDATE,
	                                        NVL(sum( CASE WHEN a.TEST_RESULT = 'PASS' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ),0) pass_qty,
	                                        NVL(sum( CASE WHEN a.TEST_RESULT = 'FAIL' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ),0) bad_qty 
                                        FROM
	                                        qcm_ex_task_list_m a 
                                        WHERE
	                                        nvl( a.TEST_RESULT, 'NULL' ) != 'NULL' 
	                                        AND a.CREATEDATE BETWEEN '{startTime}'
	                                        AND '{endTime}' {LabLineWhereSql} 
                                        GROUP BY
	                                        a.CREATEDATE 
	                                        ) SELECT
	                                        CREATEDATE,
	                                        pass_qty,
	                                        bad_qty,
                                        CASE
		
		                                        WHEN pass_qty + bad_qty > 0 THEN
		                                        round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
	                                        END AS pass_rate 
                                        FROM
	                                        tmp1";
                labDic["day"] = RetunHisData(DB, ReturnData("d", startTime, endTime), LabDaySql, "CREATEDATE", "pass_qty", "bad_qty", "pass_rate", lanDic["PASS测试报告份数"].ToString(), lanDic["FAIL测试报告份数"].ToString(), lanDic["PASS比率"].ToString());
                
                //扇形(固定年)
                string LabCakeYearSQL = $@"WITH temp AS (
	                                    SELECT
		                                    a.INSPECTION_NAME,
		                                    count( a.id ) bad_count,
		                                    ROW_NUMBER ( ) over ( ORDER BY count( a.id ) DESC ) AS Ranking 
	                                    FROM
		                                    qcm_ex_task_list_d a 
                                            LEFT JOIN QCM_EX_TASK_LIST_M M ON M.TASK_NO = A.TASK_NO 
	                                    WHERE
		                                    a.CREATEDATE BETWEEN '{startYear}' 
		                                    AND '{endYear}' 
		                                    AND a.ITEM_TEST_RESULT = 'FAIL' 
                                            {LabCakeWhereSql}
	                                    GROUP BY
		                                    a.INSPECTION_NAME 
	                                    ) 
                                    SELECT
	                                    INSPECTION_NAME,
	                                    bad_count 
                                    FROM
	                                    temp 
                                    WHERE
	                                    Ranking <= 5 UNION ALL
                                    SELECT
	                                    'Other',
	                                    NVL( SUM( bad_count ), 0 ) 
                                    FROM
	                                    temp 
                                    WHERE
	                                    Ranking >5";
                labDic["yearCake"] = ReturnCakeData(DB, LabCakeYearSQL, 0, "INSPECTION_NAME", "bad_count","");//扇形数据
                
                //时间区间                                                                                     //扇形(固定年)
                string LabCakeDaySQL = $@"WITH temp AS (
	                                        SELECT
		                                        A.INSPECTION_NAME,
		                                        count( A.id ) bad_count,
		                                        ROW_NUMBER ( ) over ( ORDER BY count( A.id ) DESC ) AS Ranking 
	                                        FROM
		                                        qcm_ex_task_list_d a
		                                        LEFT JOIN QCM_EX_TASK_LIST_M M ON M.TASK_NO = A.TASK_NO 
	                                        WHERE
		                                        a.CREATEDATE BETWEEN '{startTime}' 
		                                        AND '{endTime}' 
		                                        AND A.ITEM_TEST_RESULT = 'FAIL' 
		                                        {LabCakeWhereSql}
	                                        GROUP BY
		                                        A.INSPECTION_NAME 
	                                        ) SELECT
	                                        INSPECTION_NAME,
	                                        bad_count 
                                        FROM
	                                        temp 
                                        WHERE
	                                        Ranking <= 5 UNION ALL
                                        SELECT
	                                        'Other',
	                                        NVL( SUM( bad_count ), 0 ) 
                                        FROM
	                                        temp 
                                        WHERE
	                                        Ranking >5";
                labDic["dayCake"] = ReturnCakeData(DB, LabCakeDaySQL, 0, "INSPECTION_NAME", "bad_count", "");//扇形数据

                RetDic["Laboratory"] = labDic;//试穿报告—实验室
                #endregion

                #region 试穿报告—试穿测试
                SJeMES_Framework_NETCore.DBHelper.DataBase WearDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
                //试穿MYSQLDB
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
                    WearDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(
                    dbConfigRow["dbtype"].ToString(),
                    dbConfigRow["dbserver"].ToString(),
                    dbConfigRow["dbname"].ToString(),
                    dbConfigRow["dbuser"].ToString(),
                    dbConfigRow["dbpassword"].ToString(),
                    ""
                );
                }

                Dictionary<string, object> WearDic = new Dictionary<string, object>();
                WearDic["month"] = null;//月份数据
                WearDic["day"] = null;//每日数据

                //本年每月数据
                string WearMonthSql = $@"SELECT
	                                            DATE_FORMAT( reportdate, '%Y-%m' ) reportdate,
	                                            sum( CASE WHEN fitresult = 'Pass' THEN qty ELSE 0 END ) pass_qty,
	                                            sum( CASE WHEN fitresult = 'Fail' THEN qty ELSE 0 END ) fail_qty 
                                            FROM
	                                            (
	                                            SELECT
		                                            a.REPORTDATE,
		                                            a.FITRESULT,
		                                            count( a.id ) qty 
	                                            FROM
		                                            fitinfo a 
	                                            WHERE
		                                            a.FITRESULT != 'NA' 
		                                            AND a.REPORTDATE BETWEEN '{startYear}' 
		                                            AND '{endYear}'
	                                            GROUP BY
		                                            a.REPORTDATE,
		                                            a.FITRESULT 
	                                            ) t 
                                            GROUP BY
	                                             DATE_FORMAT( reportdate, '%Y-%m' )";
                WearDic["month"] = RetunHisData(WearDB, ReturnData("m", startYear, endYear), WearMonthSql, "reportdate", "pass_qty", "fail_qty", "", lanDic["PASS测试报告份数"].ToString(), lanDic["FAIL测试报告份数"].ToString(), lanDic["PASS比率"].ToString());
                //本年扇形
                string WearYearCakeSQL = $@"select item_name,fail_count ,(@rownum := @rownum + 1) as ranking
                                        from (
                                        select 'Length' as item_name ,count(TEST1_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST1_1!='c' {WareCakeWhereSql}
                                        union all 
                                        select 'Toe box height' as item_name ,count(TEST1_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST1_2!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Toe box width' as item_name ,count(TEST1_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST1_3!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Ball girth volume' as item_name ,count(TEST1_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST1_4!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Instep volume' as item_name ,count(TEST2_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST2_1!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Strap Length' as item_name ,count(TEST2_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST2_2!='b'{WareCakeWhereSql}
                                        union all 
                                        select 'Lace Length' as item_name ,count(TEST2_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST2_3!='b'{WareCakeWhereSql}
                                        union all 
                                        select 'Throat opening' as item_name ,count(TEST2_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST2_4!='b'{WareCakeWhereSql}
                                        union all 
                                        select 'Collar heights medial' as item_name ,count(TEST3_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST3_1!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Collar heights lateral' as item_name ,count(TEST3_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST3_2!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Collar heights heel' as item_name ,count(TEST3_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST3_3!='c'{WareCakeWhereSql}
                                        union all 
                                        select 'Heel slip' as item_name ,count(TEST3_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST3_4!='a'{WareCakeWhereSql}
                                        union all 
                                        select 'Heel width' as item_name ,count(TEST3_5) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startYear}' and '{endYear}' and b.FITRESULT !='NA'
                                        where TEST3_5!='c'{WareCakeWhereSql}
                                        ) t,
                                        (select @rownum := 0) t2
                                        order by fail_count desc";
                WearDic["yearCake"] = ReturnCakeData(WearDB, WearYearCakeSQL, 1, "item_name", "fail_count", "ranking");//扇形数据
                //区间扇形数据                                                                                           
                string WearDayCakeSQL = $@"select item_name,fail_count ,(@rownum := @rownum + 1) as ranking
                                        from (
                                        select 'Length' as item_name ,count(TEST1_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST1_1!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Toe box height' as item_name ,count(TEST1_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST1_2!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Toe box width' as item_name ,count(TEST1_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST1_3!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Ball girth volume' as item_name ,count(TEST1_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST1_4!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Instep volume' as item_name ,count(TEST2_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST2_1!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Strap Length' as item_name ,count(TEST2_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST2_2!='b'
                                        {WareCakeWhereSql} union all 
                                        select 'Lace Length' as item_name ,count(TEST2_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST2_3!='b'
                                        {WareCakeWhereSql} union all 
                                        select 'Throat opening' as item_name ,count(TEST2_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST2_4!='b'
                                        {WareCakeWhereSql} union all 
                                        select 'Collar heights medial' as item_name ,count(TEST3_1) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST3_1!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Collar heights lateral' as item_name ,count(TEST3_2) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST3_2!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Collar heights heel' as item_name ,count(TEST3_3) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST3_3!='c'
                                        {WareCakeWhereSql} union all 
                                        select 'Heel slip' as item_name ,count(TEST3_4) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST3_4!='a'
                                        {WareCakeWhereSql} union all 
                                        select 'Heel width' as item_name ,count(TEST3_5) fail_count from fitreport a
                                        join fitinfo b on b.REPORTDATE between '{startTime}' and '{endTime}' and b.FITRESULT !='NA'
                                        where TEST3_5!='c' {WareCakeWhereSql}
                                        ) t,
                                        (select @rownum := 0) t2
                                        order by fail_count desc";
                WearDic["dayCake"] = ReturnCakeData(WearDB, WearDayCakeSQL, 1, "item_name", "fail_count", "ranking");//扇形数据
                //每日数据
                string WearDaySql = $@"SELECT
	                                        DATE_FORMAT( reportdate, '%Y-%m-%d' ) reportdate,
	                                        sum( CASE WHEN fitresult = 'Pass' THEN qty ELSE 0 END ) pass_qty,
	                                        sum( CASE WHEN fitresult = 'Fail' THEN qty ELSE 0 END ) fail_qty 
                                        FROM
	                                        (
	                                        SELECT
		                                        a.REPORTDATE,
		                                        a.FITRESULT,
		                                        count( a.id ) qty 
	                                        FROM
		                                        fitinfo a 
	                                        WHERE
		                                        a.FITRESULT != 'NA' 
		                                        AND a.REPORTDATE BETWEEN '{startTime}' 
		                                        AND '{endTime}' {WareLineWhereSql}
	                                        GROUP BY
		                                        a.REPORTDATE,
		                                        a.FITRESULT 
	                                        ) t 
                                        GROUP BY
	                                        DATE_FORMAT(reportdate,'%Y-%m-%d')";
                WearDic["day"] = RetunHisData(WearDB, ReturnData("d", startTime, endTime), WearDaySql, "reportdate", "pass_qty", "fail_qty", "", lanDic["PASS测试报告份数"].ToString(), lanDic["FAIL测试报告份数"].ToString(), lanDic["PASS比率"].ToString());
                RetDic["WearingTest"] = WearDic;//试穿报告—试穿测试
                #endregion

                #region 送测部门以及ART
                Dictionary<string,object> OtherDic=new Dictionary<string,object>();
                OtherDic["depart"] = null;
                OtherDic["art"] = null;
                string departDataSql = $@"WITH tmp1 AS (
	                                            SELECT
		                                            a.staff_department,
	                                            sum( CASE WHEN a.TEST_RESULT = 'PASS' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) pass_qty,
	                                            sum( CASE WHEN a.TEST_RESULT = 'FAIL' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) bad_qty 
                                            FROM
	                                            qcm_ex_task_list_m a 
                                            WHERE
	                                            nvl( a.TEST_RESULT, 'NULL' ) != 'NULL' 
	                                            AND a.CREATEDATE BETWEEN '{startTime}' 
	                                            AND '{endTime}' {DepartLineWhereSql} 
                                            GROUP BY
	                                            a.staff_department 
	                                            ) SELECT
	                                            staff_department,
	                                            pass_qty,
	                                            bad_qty,
                                            CASE
		
		                                            WHEN pass_qty + bad_qty > 0 THEN
		                                            round( pass_qty / ( pass_qty + bad_qty ) * 100, 2 ) ELSE 0 
	                                            END pass_rate 
                                            FROM
	                                            tmp1";
                DataTable DepartDt = DB.GetDataTable(departDataSql);
                OtherDic["depart"] = ReturnOtherHisData(DepartDt, "staff_department", "bad_qty", "pass_qty", "pass_rate",ui_lan_type);
                string artDataSql = $@"with tmp1 as (
	                                        select b.PROD_NO ,b.NAME_T,
	                                        sum(case when a.TEST_RESULT='PASS' then to_number(a.SEND_TEST_QTY) else 0 end) pass_qty,
	                                        sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	                                        from qcm_ex_task_list_m a
	                                        join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
										    LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = B.SHOE_NO
										    LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
										    AND RULE_NO = 'CATEGORY_NO' 
										    AND LANGUAGE = '1' 
	                                        where nvl(a.TEST_RESULT,'NULL')!='NULL'
	                                        and a.CREATEDATE between '{startTime}' and '{endTime}' {ArtLineWhereSql}
	                                        group by b.PROD_NO ,b.NAME_T
                                        )
                                        select PROD_NO art_no,NAME_T art_name,pass_qty,bad_qty,round(pass_qty/(pass_qty+bad_qty)*100,2) pass_rate from tmp1";
                DataTable artDt = DB.GetDataTable(artDataSql);
                OtherDic["art"] = ReturnOtherHisData(artDt, "art_no", "bad_qty", "pass_qty", "pass_rate",ui_lan_type);
                RetDic["OtherData"] = OtherDic;//送测部门以及ART
                #endregion

                #region 试穿报告—分析列表表格
                string DepartAnalysisSQL = $@"WITH tmp1 AS (
	                                                SELECT
		                                                a.staff_department,
	                                                sum( CASE WHEN a.TEST_RESULT = 'PASS' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) pass_qty,
	                                                sum( CASE WHEN a.TEST_RESULT = 'FAIL' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) bad_qty 
                                                FROM
	                                                qcm_ex_task_list_m a 
                                                WHERE
	                                                nvl( a.TEST_RESULT, 'NULL' ) != 'NULL' 
	                                                AND a.CREATEDATE BETWEEN '{startTime}' 
	                                                AND '{endTime}' {DepartTbWhereSql}
                                                GROUP BY
	                                                a.staff_department 
	                                                ),
	                                                ex_bad_data_tmp1 AS (
	                                                SELECT
		                                                b.staff_department,
		                                                a.INSPECTION_NAME,
		                                                a.id 
	                                                FROM
		                                                qcm_ex_task_list_d a
		                                                JOIN qcm_ex_task_list_m b ON a.TASK_NO = b.TASK_NO 
	                                                WHERE
		                                                b.CREATEDATE BETWEEN '{startTime}' 
		                                                AND '{endTime}' 
                                                        and b.TEST_RESULT is not NULL
		                                                AND a.ITEM_TEST_RESULT = 'FAIL' 
	                                                ),
	                                                ex_bad_data_tmp2 AS (
	                                                SELECT
		                                                staff_department,
		                                                INSPECTION_NAME,
		                                                count( id ) bad_qty,
		                                                ROW_NUMBER ( ) over ( partition BY staff_department ORDER BY count( id ) DESC ) ranking 
	                                                FROM
		                                                ex_bad_data_tmp1 
	                                                GROUP BY
		                                                staff_department,
		                                                INSPECTION_NAME 
	                                                ),
	                                                total_bad_data AS ( SELECT staff_department, INSPECTION_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY staff_department ) totla_bad_qty FROM ex_bad_data_tmp2 ),
                                    tabledata AS (SELECT
	                                                a.staff_department,
	                                                ( a.pass_qty + a.bad_qty ) AS num,
                                                CASE
		
		                                                WHEN a.pass_qty + a.bad_qty > 0 THEN
		                                                trunc( a.pass_qty / ( a.pass_qty + a.bad_qty ) * 100, 2 ) ELSE 0 
	                                                END AS pass_rate,
	                                                b.INSPECTION_NAME INSPECTION_NAME,
	                                                trunc( b.bad_qty / b.totla_bad_qty * 100, 2 ) bad_item_rate,
	                                                b.ranking ranking_1,
	                                                c.INSPECTION_NAME INSPECTION_NAME1,
	                                                trunc( c.bad_qty / c.totla_bad_qty * 100, 2 ) bad_item_rate1,
	                                                c.ranking ranking_2,
	                                                d.INSPECTION_NAME INSPECTION_NAME2,
	                                                trunc( d.bad_qty / d.totla_bad_qty * 100, 2 ) bad_item_rate2,
	                                                d.ranking ranking_3 
                                                FROM
	                                                tmp1 a
	                                                LEFT JOIN total_bad_data b ON a.staff_department = b.staff_department 
	                                                AND b.ranking = 1
	                                                LEFT JOIN total_bad_data c ON a.staff_department = c.staff_department 
	                                                AND c.ranking = 2
	                                                LEFT JOIN total_bad_data d ON a.staff_department = d.staff_department 
	                                                AND d.ranking =3
                                       )SELECT * FROM tabledata WHERE 1=1 {testWhere}";
                DataTable DepartAnalyDt = DB.GetDataTable(DepartAnalysisSQL);
                string ArtAnalysisSQL = $@"WITH tmp1 AS (
	                                            SELECT
		                                            b.PROD_NO,
		                                            b.NAME_T,
	                                            sum( CASE WHEN a.TEST_RESULT = 'PASS' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) pass_qty,
	                                            sum( CASE WHEN a.TEST_RESULT = 'FAIL' THEN to_number( a.SEND_TEST_QTY ) ELSE 0 END ) bad_qty 
                                            FROM
	                                            qcm_ex_task_list_m a
	                                            JOIN bdm_rd_prod b ON a.ART_NO = b.PROD_NO 
                                            WHERE
	                                            nvl( a.TEST_RESULT, 'NULL' ) != 'NULL' 
	                                            AND a.CREATEDATE BETWEEN '{startTime}' 
	                                            AND '{endTime}' {ArtTbWhereSql}
                                            GROUP BY
	                                            b.PROD_NO,
	                                            b.NAME_T 
	                                            ),
	                                            ex_bad_data_tmp1 AS (
	                                            SELECT
		                                            c.PROD_NO,
		                                            c.NAME_T,
		                                            a.INSPECTION_NAME,
		                                            a.id 
	                                            FROM
		                                            qcm_ex_task_list_d a
		                                            JOIN qcm_ex_task_list_m b ON a.TASK_NO = b.TASK_NO
		                                            JOIN bdm_rd_prod c ON b.ART_NO = c.PROD_NO 
	                                            WHERE
		                                            b.CREATEDATE BETWEEN '{startTime}' 
		                                            AND '{endTime}' 
                                                    AND b.TEST_RESULT is NOT null
		                                            AND a.ITEM_TEST_RESULT = 'FAIL' 
	                                            ),
	                                            ex_bad_data_tmp2 AS (
	                                            SELECT
		                                            PROD_NO,
		                                            NAME_T,
		                                            INSPECTION_NAME,
		                                            count( id ) bad_qty,
		                                            ROW_NUMBER ( ) over ( partition BY PROD_NO ORDER BY count( id ) DESC ) ranking 
	                                            FROM
		                                            ex_bad_data_tmp1 
	                                            GROUP BY
		                                            PROD_NO,
		                                            NAME_T,
		                                            INSPECTION_NAME 
	                                            ),
	                                            total_bad_data AS ( SELECT PROD_NO, NAME_T, INSPECTION_NAME, bad_qty, ranking, sum( bad_qty ) over ( partition BY PROD_NO ) totla_bad_qty FROM ex_bad_data_tmp2 ),
                                        tabledata as (SELECT
	                                            a.PROD_NO,
	                                            a.NAME_T,
	                                            ( a.pass_qty + a.bad_qty ) AS num,
                                            CASE
		
		                                            WHEN a.pass_qty + a.bad_qty > 0 THEN
		                                            trunc( a.pass_qty / ( a.pass_qty + a.bad_qty ) * 100, 3 ) ELSE 0 
	                                            END AS pass_rate,
	                                            b.INSPECTION_NAME INSPECTION_NAME,
	                                            trunc( b.bad_qty / b.totla_bad_qty * 100, 2 ) bad_item_rate,
	                                            b.ranking ranking_1,
	                                            c.INSPECTION_NAME INSPECTION_NAME1,
	                                            trunc( c.bad_qty / c.totla_bad_qty * 100, 2 ) bad_item_rate1,
	                                            c.ranking ranking_2,
	                                            d.INSPECTION_NAME INSPECTION_NAME2,
	                                            trunc( d.bad_qty / d.totla_bad_qty * 100, 2 ) bad_item_rate2,
	                                            d.ranking ranking_3 
                                            FROM
	                                            tmp1 a
	                                            LEFT JOIN total_bad_data b ON a.PROD_NO = b.PROD_NO 
	                                            AND b.ranking = 1
	                                            LEFT JOIN total_bad_data c ON a.PROD_NO = c.PROD_NO 
	                                            AND c.ranking = 2
	                                            LEFT JOIN total_bad_data d ON a.PROD_NO = d.PROD_NO 
	                                            AND d.ranking =3
                                        ) SELECT * FROM tabledata WHERE 1=1 {testWhere}";
                DataTable ArtAnalyDt = DB.GetDataTable(ArtAnalysisSQL);
                Dictionary<string, object> AnalysDic = new Dictionary<string, object>();

                DataView dv = new DataView(DepartAnalyDt);
                dv.Sort = "pass_rate DESC";
                AnalysDic["Depart"] = dv.ToTable();

                //AnalysDic["Depart"] = DepartAnalyDt;
                 dv = new DataView(ArtAnalyDt);
                dv.Sort = "pass_rate ASC";
                AnalysDic["Art"] = dv.ToTable();

                RetDic["AnalysisList"] = AnalysDic;//分析列表
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
        /// 封装柱状图数据，仅适用与两个柱状数据一条折线数据，且必须保证匹配字段唯一
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="TimeList">x轴数组</param>
        /// <param name="Sql">数据查询sql</param>
        /// <param name="DtSelectStr">Dt中匹配x轴字段</param>
        /// <param name="DataStr1">取值字段1</param>
        /// <param name="DataStr2">取值字段2</param>
        /// <param name="DataStr3">取值字段3,当此值为空时采取计算方式</param>
        /// <param name="DSName1">数据标题1</param>
        /// <param name="DSName2">数据标题2</param>
        /// <param name="DSName3">数据标题3</param>
        /// <returns></returns>
        public static Dictionary<string, object> RetunHisData(SJeMES_Framework_NETCore.DBHelper.DataBase DB, List<string> TimeList, string Sql, string DtSelectStr, string DataStr1, string DataStr2, string DataStr3, string DSName1, string DSName2, string DSName3)
        {
            Dictionary<string, object> HisDic = new Dictionary<string, object>();
            HisDic["x"] = TimeList;//x轴
            HisDic["bar"] = null;//柱图
            HisDic["line"] = null;//折线


            DataTable LabMonthDt = DB.GetDataTable(Sql);
            List<decimal> LabPassList = new List<decimal>();//PassList
            List<decimal> LabFailList = new List<decimal>();//FailList
            List<decimal> LabRateList = new List<decimal>();//RateList
            List<Dictionary<string, object>> LabBarDic = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> LabLinerDic = new List<Dictionary<string, object>>();
            foreach (var item in TimeList)
            {
                decimal PassNum = 0;
                decimal failNum = 0;
                decimal RateNum = 0;
                DataRow[] SelectRow = LabMonthDt.Select($@"{DtSelectStr}='{item}'");
                foreach (DataRow Ritem in SelectRow)
                {
                    PassNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr1].ToString()) ? "0" : Ritem[DataStr1].ToString());
                    failNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr2].ToString()) ? "0" : Ritem[DataStr2].ToString());
                    if (string.IsNullOrEmpty(DataStr3))
                    {
                        if (PassNum + failNum > 0)
                        {
                            RateNum += Math.Round(PassNum / (PassNum + failNum) * 100, 2);
                        }
                    }
                    else
                    {
                        RateNum += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[DataStr3].ToString()) ? "0" : Ritem[DataStr3].ToString());
                    }

                }
                LabPassList.Add(PassNum);
                LabFailList.Add(failNum);
                LabRateList.Add(RateNum);
            }

            //柱状图数据
            Dictionary<string, object> LabPassDic = new Dictionary<string, object>();
            LabPassDic["name"] = DSName1;
            LabPassDic["data"] = LabPassList;
            LabBarDic.Add(LabPassDic);
            Dictionary<string, object> LabFailDic = new Dictionary<string, object>();
            LabFailDic["name"] = DSName2;
            LabFailDic["data"] = LabFailList;
            LabBarDic.Add(LabFailDic);
            HisDic["bar"] = LabBarDic;

            //折线数据
            Dictionary<string, object> RateDic = new Dictionary<string, object>();
            RateDic["name"] = DSName3;
            RateDic["data"] = LabRateList;
            LabLinerDic.Add(RateDic);
            HisDic["line"] = LabLinerDic;//折线
            return HisDic;
        }
        /// <summary>
        /// 返回饼图数据
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="SQL">sql</param>
        /// <param name="Type">数据处理方式【0】:sql处理【1】:程序处理</param>
        /// <param name="NameStr">name取值字段</param>
        /// <param name="ValueStr">value取值字段</param>
        /// <param name="RankingStr">程序序号处理字段</param>
        /// <returns></returns>
        public static List<Dictionary<string, object>> ReturnCakeData(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string SQL,int Type, string NameStr, string ValueStr,string RankingStr)
        {
            List<Dictionary<string, object>> CakeDic = new List<Dictionary<string, object>>();
            DataTable CakeDt = DB.GetDataTable(SQL);
            decimal otherNum = 0;
            foreach (DataRow item in CakeDt.Rows)
            {
                Dictionary<string, object> Dic = new Dictionary<string, object>();
                if (Type>0)
                {
                    if (Convert.ToDecimal(item["ranking"].ToString()) <5)
                    {
                        decimal Num= Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                        if (Num>0)
                        {
                            Dic["name"] = item[NameStr];
                            Dic["value"] = Num;
                            CakeDic.Add(Dic);
                        }
                    }
                    else
                    {
                        otherNum+= Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                    }
                }
                else
                {
                    Dic["name"] = item[NameStr];
                    Dic["value"] = Convert.ToDecimal(string.IsNullOrEmpty(item[ValueStr].ToString()) ? "0" : item[ValueStr].ToString());
                    CakeDic.Add(Dic);
                }

            }
            if (otherNum>0)
            {
                Dictionary<string, object> Dic = new Dictionary<string, object>();
                Dic["name"] = "Other";
                Dic["value"] = otherNum;
                CakeDic.Add(Dic);
            }
            return CakeDic;
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
                RetList.Add(item[RowName].ToString());
            }
            return RetList;
        }
        /// <summary>
        /// 送测柱状图
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowName1">取值字段：x轴</param>
        /// <param name="rowName2">取值字段：送测不合格</param>
        /// <param name="rowName3">取值字段：送测合格</param>
        /// <param name="rowName4">取值字段：送测合格率</param>
        /// <returns></returns>
        public static Dictionary<string,object> ReturnOtherHisData(DataTable dt,string rowName1, string rowName2, string rowName3, string rowName4,string ui_lan_type)
        {
            List<string> DepartXList = TranDate(rowName1, dt);
            List<string> DepartFailist = TranDate(rowName2, dt);
            List<string> DepartPassList = TranDate(rowName3, dt);
            List<string> DepartRateList = TranDate(rowName4, dt);
            var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "送测合格数", "送测不合格数", "送测合格率" });
            List<Dictionary<string, object>> DepartBarList = new List<Dictionary<string, object>>();
            Dictionary<string, object> PassDic = new Dictionary<string, object>();
            PassDic["name"] = lanDic["送测合格数"];
            PassDic["data"] = DepartPassList;
            DepartBarList.Add(PassDic);

            Dictionary<string, object> FailDic = new Dictionary<string, object>();
            FailDic["name"] = lanDic["送测不合格数"];
            FailDic["data"] = DepartFailist;
            DepartBarList.Add(FailDic);

            List<Dictionary<string, object>> DepartLineList = new List<Dictionary<string, object>>();
            Dictionary<string, object> RateDic = new Dictionary<string, object>();
            RateDic["name"] = lanDic["送测合格率"];
            RateDic["data"] = DepartRateList;
            DepartLineList.Add(RateDic);

            Dictionary<string, object> DepartDic = new Dictionary<string, object>();
            DepartDic["x"] = DepartXList;
            DepartDic["bar"] = DepartBarList;
            DepartDic["line"] = DepartLineList;
            return DepartDic;
        }
    }
}
