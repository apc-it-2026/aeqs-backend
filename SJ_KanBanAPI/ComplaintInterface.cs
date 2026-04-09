using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// 投诉数据分析
    /// </summary>
    internal class ComplaintInterface
    {
        static string ui_lan_type = "en";
        static string moudle_code = "complaintsAnalysis";

        public static Dictionary<string, object> RetunHisData(DataTable Dt, List<string> TimeList, string DtSelectStr, string LineStr1, string LineStr2, string  BarStr1, string BarStr2, string DSLineName1, string DSLineName2, string DSBarName1, string DSBarName2)
        {
            Dictionary<string, object> HisDic = new Dictionary<string, object>();
            HisDic["x"] = TimeList;//x轴
            HisDic["bar"] = null;//柱图
            HisDic["line"] = null;//折线


            List<decimal> LineList1 = new List<decimal>();
            List<decimal> LineList2 = new List<decimal>();
            List<decimal> BarList1 = new List<decimal>();
            List<decimal> BarList2 = new List<decimal>();
            List<Dictionary<string, object>> LabBarDic = new List<Dictionary<string, object>>();
            List<Dictionary<string, object>> LabLinerDic = new List<Dictionary<string, object>>();
            foreach (var item in TimeList)
            {
                decimal LineNum1 = 0;
                decimal LineNum2 = 0;
                decimal BarNum1 = 0;
                decimal BarNum2 = 0;
                //今年
                DataRow[] SelectRow = Dt.Select($@"{DtSelectStr}='{item}'");
                foreach (DataRow Ritem in SelectRow)
                {
                    LineNum1 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr1].ToString()) ? "0" : Ritem[LineStr1].ToString());
                    //LineNum2 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr2].ToString()) ? "0" : Ritem[LineStr2].ToString());
                    BarNum1 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[BarStr1].ToString()) ? "0" : Ritem[BarStr1].ToString());
                    //BarNum2 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[BarStr2].ToString()) ? "0" : Ritem[BarStr2].ToString());
                }
                //去年
                string lasttime = DateTime.Parse(item).AddYears(-1).ToString("yyyy-MM");
                 SelectRow = Dt.Select($@"{DtSelectStr}='{lasttime}'");
                foreach (DataRow Ritem in SelectRow)
                {
                    //LineNum1 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr1].ToString()) ? "0" : Ritem[LineStr1].ToString());
                    LineNum2 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[LineStr2].ToString()) ? "0" : Ritem[LineStr2].ToString());
                    //BarNum1 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[BarStr1].ToString()) ? "0" : Ritem[BarStr1].ToString());
                    BarNum2 += Convert.ToDecimal(string.IsNullOrEmpty(Ritem[BarStr2].ToString()) ? "0" : Ritem[BarStr2].ToString());
                }

                LineList1.Add(LineNum1);
                LineList2.Add(LineNum2);
                BarList1.Add(BarNum1);
                BarList2.Add(BarNum2);
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
            HisDic["bar"] = LabBarDic;

            //折线数据
            Dictionary<string, object> BarDic1 = new Dictionary<string, object>();
            BarDic1["name"]  =DSBarName1;
            BarDic1["data"] = BarList1;
            LabLinerDic.Add(BarDic1);
            Dictionary<string, object> BarDic2 = new Dictionary<string, object>();
            BarDic2["name"]  = DSBarName2;
            BarDic2["data"] = BarList2;
            LabLinerDic.Add(BarDic2);
            HisDic["line"] = LabLinerDic;//折线
            return HisDic;
        }
        /// <summary>
        /// 年度退货数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReturnYearData(object OBJ)
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
                ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";
                string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//ART
                string ShoeName = jarr.ContainsKey("ShoeName") ? jarr["ShoeName"].ToString() : "";//鞋型名称
                string Category = jarr.ContainsKey("Category") ? jarr["Category"].ToString() : "";//Category
                string ComplaintStr = jarr.ContainsKey("ComplaintStr") ? jarr["ComplaintStr"].ToString() : "";//投诉问题
                string ComplaintCountry = jarr.ContainsKey("ComplaintCountry") ? jarr["ComplaintCountry"].ToString() : "";//投诉国家
                string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//Po
                string PlantArea = jarr.ContainsKey("PlantArea") ? jarr["PlantArea"].ToString() : "";//厂区
                string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";//生产线
                //List<string> ProductionMonth = jarr.ContainsKey("ProductionMonth") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ProductionMonth"] == null ? "" : jarr["ProductionMonth"].ToString()) : new List<string>();   //生产月份
                string ProductionMonth = jarr.ContainsKey("ProductionMonth") ? jarr["ProductionMonth"].ToString() : "";   //生产月份
                string StartTime = jarr.ContainsKey("StartTime") ? jarr["StartTime"].ToString() : "";//开始时间
                string EndTime = jarr.ContainsKey("EndTime") ? jarr["EndTime"].ToString() : "";//结束时间
                string[] orderListart = Art.Split(',');
                string[] orderListshoeName = ShoeName.Split(',');
                string[] orderListcategory = Category.Split(',');
                string[] orderListCC = ComplaintCountry.Split(',');
                string[] orderListPo = Po.Split(',');
                string[] orderListMonth = ProductionMonth.Split(',');

                #endregion

                #region 参数判断
                if (string.IsNullOrEmpty(StartTime)||string.IsNullOrEmpty(EndTime))
                {
                    throw new Exception("缺少必要参数【开始时间】或【结束时间】！！！");
                }
                #endregion

                #region 组合查询SQL
                string WSQL = string.Empty;
                string MonthSQL = string.Empty;
                if (!string.IsNullOrEmpty(Art))
                {
                    WSQL += $@" AND o.art in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ShoeName))
                {
                    //WSQL += $@" AND o.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    WSQL += $@" AND o.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    WSQL += $@" AND o.category in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ComplaintCountry))
                {
                    WSQL += $@" AND a.COUNTRY_REGION LIKE '%{ComplaintCountry}%'";
                }
                if (!string.IsNullOrEmpty(ComplaintStr))
                {
                    WSQL += $@" AND a.DEFECT_CONTENT LIKE '%{ComplaintStr}%'";
                }
                if (!string.IsNullOrEmpty(Po))
                {
                    WSQL += $@"	AND a.PO_ORDER in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PlantArea))
                {
                    WSQL += $@"";
                }
                if (!string.IsNullOrEmpty(ProductionLine))
                {
                    WSQL += $@"";
                }
                if (!string.IsNullOrEmpty(ProductionMonth))
                {
                    if (ProductionMonth == "[]")
                    {
                        MonthSQL += $@"";
                    }
                    else
                    {
                        MonthSQL += $@"and( q.MIN_INSERT_DATE = '{ProductionMonth}' or q.MAX_INSERT_DATE = '{ProductionMonth}' )";
                    }
                    //MonthSQL += $@"and( q.MIN_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) or q.MAX_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) )";
                }
               /* if (ProductionMonth != null && ProductionMonth.Count > 0)
                {
                    MonthSQL += $@"and( q.MIN_INSERT_DATE in ('{string.Join("','", ProductionMonth)}') or q.MAX_INSERT_DATE in ('{string.Join("','", ProductionMonth)}'))";
                }*/
                #endregion

                #region 数据处理
                string StartYear = DateTime.Now.ToString("yyyy-01-01");//开始日期
                string EndYear = Convert.ToDateTime(StartYear).AddYears(1).AddMonths(-1).ToString("yyyy-MM-dd");//结束日期
                List<string> MonthList = Common.ReturnData("m", StartYear, EndYear);//今年得十二个月

                #region old 
                /*
                string DataSql = $@"WITH queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                    ,ORDERDATA AS (
	                                SELECT
		                                BM.MER_PO PO,
		                                BI.PROD_NO ART,
		                                BD.NAME_T CATEGORY,
		                                BI.SHOE_NO,
		                                bi.SE_QTY 
	                                FROM
		                                BDM_SE_ORDER_ITEM BI
		                                LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
		                                LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
		                                LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
		                                LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
		                                AND RULE_NO = 'CATEGORY_NO' 
		                                AND LANGUAGE = '1' 
                                        left join queryData q on q.se_id=BI.se_id
                                    where 1=1 {MonthSQL}
	                                ),
	                                lastyear AS (
	                                SELECT
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) COMPLAINT_DATE,
		                                count( id ) complaint_qty,
		                                SUM( o.SE_QTY ) LASTPOQTY 
	                                FROM
		                                QCM_CUSTOMER_COMPLAINT_M a
		                                JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
	                                WHERE
		                                to_char( a.COMPLAINT_DATE, 'yyyy' ) = to_char( SYSDATE, 'yyyy' ) - 1 
                                        {WSQL}
	                                GROUP BY
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) 
	                                ),
	                                thisyear AS (
	                                SELECT
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) COMPLAINT_DATE,
		                                count( id ) complaint_qty,
		                                SUM( NVL( o.SE_QTY, 0 ) ) THISPOQTY 
	                                FROM
		                                QCM_CUSTOMER_COMPLAINT_M a
		                                JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
	                                WHERE
		                                to_char( a.COMPLAINT_DATE, 'yyyy' ) = to_char( SYSDATE, 'yyyy' ) 
                                        {WSQL}
	                                GROUP BY
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) 
	                                ) SELECT
	                                thisyear.COMPLAINT_DATE,
	                                thisyear.COMPLAINT_QTY thisqty,
	                                NVL( lastyear.COMPLAINT_QTY, 0 ) lastqty,
	                                '0' thisrate,
	                                '0' lastrate,
	                                thisyear.THISPOQTY,
	                                NVL(lastyear.LASTPOQTY, 0) LASTPOQTY
                                FROM
	                                thisyear
	                                LEFT JOIN lastyear ON thisyear.COMPLAINT_DATE = lastyear.COMPLAINT_DATE 
                                ORDER BY
	                                thisyear.COMPLAINT_DATE";
                */
                #endregion

                string DataSql = $@"WITH yeardateall as (

select to_char(add_months(sysdate, -t.rn), 'yyyy-mm') datee
from dual a, (select rownum - 1 rn from dual connect by rownum <= 24) t
 where to_char(add_months(sysdate, -t.rn), 'yyyy') =to_char(sysdate, 'yyyy')-1 or to_char(add_months(sysdate, -t.rn), 'yyyy') =to_char(sysdate, 'yyyy')
order by to_char(add_months(sysdate, -t.rn), 'yyyy-mm')


),
queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                    ,ORDERDATA AS (
	                                SELECT
		                                BM.MER_PO PO,
		                                BI.PROD_NO ART,
		                                -- BD.NAME_T CATEGORY,
                                        BC.NAME_T CATEGORY,
		                                -- BI.SHOE_NO,
                                        BS.NAME_T,
		                                bi.SE_QTY 
	                                FROM
		                                BDM_SE_ORDER_ITEM BI
		                                LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
		                                LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
		                                LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
		                                LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
		                                AND RULE_NO = 'CATEGORY_NO' 
		                                AND LANGUAGE = '1' 
                                        left join queryData q on q.se_id=BI.se_id
                                    where 1=1 {MonthSQL}
	                                ),
	                                lastyear AS (
	                                SELECT
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) COMPLAINT_DATE,
		                                count( id ) complaint_qty,
		                                SUM( o.SE_QTY ) LASTPOQTY 
	                                FROM
		                                QCM_CUSTOMER_COMPLAINT_M a
		                                JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
	                                WHERE
		                                to_char( a.COMPLAINT_DATE, 'yyyy' ) = {DateTime.Now.AddYears(-1).ToString("yyyy")}
                                        {WSQL}
	                                GROUP BY
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) 
	                                ),
	                                thisyear AS (
	                                SELECT
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) COMPLAINT_DATE,
		                                count( id ) complaint_qty,
		                                SUM( NVL( o.SE_QTY, 0 ) ) THISPOQTY 
	                                FROM
		                                QCM_CUSTOMER_COMPLAINT_M a
		                                JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
	                                WHERE
		                                to_char( a.COMPLAINT_DATE, 'yyyy' ) = {DateTime.Now.ToString("yyyy")}
                                        {WSQL}
	                                GROUP BY
		                                to_char( a.COMPLAINT_DATE, 'yyyy-mm' ) 
	                                )  SELECT
									yeardateall.datee as COMPLAINT_DATE,
	                                --thisyear.COMPLAINT_DATE,
	                                nvl(thisyear.COMPLAINT_QTY,0) thisqty,
	                                NVL( lastyear.COMPLAINT_QTY, 0 ) lastqty,
	                                '0' thisrate,
	                                '0' lastrate,
	                                 nvl(thisyear.THISPOQTY,0)THISPOQTY,
	                                NVL(lastyear.LASTPOQTY, 0) LASTPOQTY
                                FROM  yeardateall 
	                                LEFT JOIN  thisyear ON thisyear.COMPLAINT_DATE = yeardateall.datee 
	                                LEFT JOIN lastyear ON yeardateall.datee = lastyear.COMPLAINT_DATE 
                               -- ORDER BY
	                               -- thisyear.COMPLAINT_DATE";

                DataTable Dt = DB.GetDataTable(DataSql);
                string ThisTempData = $@"WITH DATETEMP AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' ) || '-' || lpad( LEVEL, 2, 0 ) month FROM dual CONNECT BY LEVEL < 13 ),
                                            BASEDATA AS (
	                                            SELECT
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) month,
		                                            sum( SHIPPING_QTY ) qty 
	                                            FROM
		                                            bmd_se_shipment_d d
		                                            LEFT JOIN bmd_se_shipment_m m ON d.SHIPPING_NO = m.SHIPPING_NO 
	                                            WHERE
		                                            TO_CHAR( m.CREATEDATE, 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )
	                                            GROUP BY
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) 
	                                            ORDER BY
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) 
	                                            ) 
                                            SELECT
	                                            DATETEMP.MONTH,
	                                            NVL( BASEDATA.qty, 0 ) QTY 
                                            FROM
	                                            BASEDATA
	                                            RIGHT JOIN DATETEMP ON DATETEMP.month = BASEDATA.month
	                                            ORDER BY DATETEMP.MONTH";
                DataTable ThisTempDt=DB.GetDataTable(ThisTempData);
                string LastTempData = $@"WITH DATETEMP AS ( SELECT TO_CHAR( SYSDATE, 'yyyy' )-1 || '-' || lpad( LEVEL, 2, 0 ) month FROM dual CONNECT BY LEVEL < 13 ),
                                            BASEDATA AS (
	                                            SELECT
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) month,
		                                            sum( SHIPPING_QTY ) qty 
	                                            FROM
		                                            bmd_se_shipment_d d
		                                            LEFT JOIN bmd_se_shipment_m m ON d.SHIPPING_NO = m.SHIPPING_NO 
	                                            WHERE
		                                            TO_CHAR( m.CREATEDATE, 'yyyy' ) = TO_CHAR( SYSDATE, 'yyyy' )-1 
	                                            GROUP BY
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) 
	                                            ORDER BY
		                                            TO_CHAR( m.CREATEDATE, 'yyyy-mm' ) 
	                                            ) 
                                            SELECT
	                                            DATETEMP.MONTH,
	                                            NVL( BASEDATA.qty, 0 ) QTY 
                                            FROM
	                                            BASEDATA
	                                            RIGHT JOIN DATETEMP ON DATETEMP.month = BASEDATA.month
	                                            ORDER BY DATETEMP.MONTH";
                DataTable LastTempDt = DB.GetDataTable(LastTempData);
                foreach (DataRow item in Dt.Rows)
                {
                    decimal ThisNum = 0;
                    foreach (DataRow TempItem in ThisTempDt.Rows)
                    {
                        ThisNum += Convert.ToDecimal(TempItem["qty"].ToString());
                        if (TempItem["month"].ToString()==item["COMPLAINT_DATE"].ToString())
                        {
                            break;
                        }
                    }
                    decimal LastNum = 0;
                    foreach (DataRow TempItem in LastTempDt.Rows)
                    {
                        LastNum += Convert.ToDecimal(TempItem["qty"].ToString());
                        if (TempItem["month"].ToString() == Convert.ToDateTime(item["COMPLAINT_DATE"].ToString()).AddYears(-1).ToString("yyyy-MM"))
                        {
                            break;
                        }
                    }
                    item["thisrate"] =ThisNum>0?Math.Round((Convert.ToDecimal(item["THISPOQTY"].ToString())/ThisNum)*1000000,1):0;
                    item["lastrate"] =LastNum>0?Math.Round((Convert.ToDecimal(item["LASTPOQTY"].ToString()) / LastNum) * 1000000, 1):0;
                }
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type,moudle_code,new List<string>() {"今年投诉次数","去年投诉次数","今年投诉数量PPM","去年投诉数量PPM"});
                Dictionary<string, object> RetDic = RetunHisData(Dt,MonthList, "COMPLAINT_DATE", "thisqty", "lastqty", "thisrate", "lastrate", lanDic["今年投诉次数"].ToString(), lanDic["去年投诉次数"].ToString(), lanDic["今年投诉数量PPM"].ToString(), lanDic["去年投诉数量PPM"].ToString());
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
        /// 主要投诉原因&投诉Category占比【扇形图】
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCACCakeData(object OBJ)
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
                string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//ART
                string ShoeName = jarr.ContainsKey("ShoeName") ? jarr["ShoeName"].ToString() : "";//鞋型名称
                string Category = jarr.ContainsKey("Category") ? jarr["Category"].ToString() : "";//Category
                string ComplaintStr = jarr.ContainsKey("ComplaintStr") ? jarr["ComplaintStr"].ToString() : "";//投诉问题
                string ComplaintCountry = jarr.ContainsKey("ComplaintCountry") ? jarr["ComplaintCountry"].ToString() : "";//投诉国家
                string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//Po
                string PlantArea = jarr.ContainsKey("PlantArea") ? jarr["PlantArea"].ToString() : "";//厂区
                string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";//生产线
                //List<string> ProductionMonth = jarr.ContainsKey("ProductionMonth") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ProductionMonth"] == null ? "" : jarr["ProductionMonth"].ToString()) : new List<string>(); ;//生产月份
                string ProductionMonth = jarr.ContainsKey("ProductionMonth") ? jarr["ProductionMonth"].ToString() : "";   //生产月份
                string StartTime = jarr.ContainsKey("StartTime") ? jarr["StartTime"].ToString() : "";//开始时间
                string EndTime = jarr.ContainsKey("EndTime") ? jarr["EndTime"].ToString() : "";//结束时间
                string[] orderListart = Art.Split(',');
                string[] orderListshoeName = ShoeName.Split(',');
                string[] orderListcategory = Category.Split(',');
                string[] orderListCS = ComplaintStr.Split(',');
                string[] orderListCC = ComplaintCountry.Split(',');
                string[] orderListPo = Po.Split(',');
                string[] orderListMonth = ProductionMonth.Split(',');
                string[] orderListPA = PlantArea.Split(',');
                #endregion

                #region 参数判断
                if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime))
                {
                    throw new Exception("缺少必要参数【开始时间】或【结束时间】！！！");
                }
                #endregion

                #region 组合查询SQL
                string WSQL = string.Empty;
                string CSQL = string.Empty;
                string MonthSQL = string.Empty;
                string PAWhere = string.Empty;
                if (!string.IsNullOrEmpty(Art))
                {
                    WSQL += $@" AND o.art in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND C.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ShoeName))
                {
                    //WSQL += $@" AND o.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    //CSQL += $@" AND C.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    WSQL += $@" AND o.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    WSQL += $@" AND o.category in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND E.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ComplaintCountry))
                {
                    WSQL += $@" AND a.COUNTRY_REGION like '%{ComplaintCountry}%'";
                    CSQL += $@" AND a.COUNTRY_REGION like '%{ComplaintCountry}%'";
                }
                if (!string.IsNullOrEmpty(ComplaintStr))
                {
                    WSQL += $@" AND a.DEFECT_CONTENT like '%{ComplaintStr}%'";
                    CSQL += $@" AND a.DEFECT_CONTENT like '%{ComplaintStr}%'";
                }
                if (!string.IsNullOrEmpty(Po))
                {
                    WSQL += $@"	AND a.PO_ORDER in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                    CSQL += $@"	AND a.PO_ORDER in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PlantArea))
                {
                    PAWhere = $@" AND PLANT_AREA IN ({string.Join(',', orderListPA.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ProductionLine))
                {
                    WSQL += $@"";
                    CSQL += $@"";
                }
                if (!string.IsNullOrEmpty(ProductionMonth))
                {
                    if (ProductionMonth == "[]")
                    {
                        MonthSQL += $@"";
                        CSQL += $@"";
                    }
                    else
                    {
                        MonthSQL += $@"and( q.MIN_INSERT_DATE = '{ProductionMonth}' or q.MAX_INSERT_DATE = '{ProductionMonth}' )";
                        CSQL += $@"and( q.MIN_INSERT_DATE = '{ProductionMonth}' or q.MAX_INSERT_DATE = '{ProductionMonth}' )";
                    }
                    //MonthSQL += $@"and( q.MIN_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) or q.MAX_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) )";
                    //CSQL += $@"and( q.MIN_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) or q.MAX_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) )";
                }

                string MonthJoinSQL = string.IsNullOrEmpty(MonthSQL) ? "" : "left join queryData q on q.se_id=BI.se_id";
                #endregion

                #region 数据处理
                Dictionary<string,object> RetDic=new Dictionary<string,object>();
                RetDic["ReturnReason"] = null;//投诉原因占比
                RetDic["ReturnCategory"] = null;//Category占比
                RetDic["ReturnArt"] = null;//前十ART占比
                RetDic["ReturnResults"] = null;//投诉处理结果占比
                RetDic["ReturnPlant"] = null;//厂区投诉数量占比

                //投诉原因占比
                string ReturnReasonSQL = $@"WITH
                                        queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                            , ORDERDATA AS (
	                                        SELECT
		                                        BM.MER_PO PO,
		                                        BI.PROD_NO ART,
		                                        BC.NAME_T CATEGORY,
                                                BS.NAME_T
		                                        -- BI.SHOE_NO 
	                                        FROM
		                                        BDM_SE_ORDER_ITEM BI
		                                        LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
		                                        LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
		                                        LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
		                                        LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
		                                        AND RULE_NO = 'CATEGORY_NO' 
		                                        AND LANGUAGE = '1'  {MonthJoinSQL}                                              
                                            WHERE  1=1 {MonthSQL}
	                                        ) 
                                        SELECT
	                                        a.DEFECT_CONTENT,
	                                        sum( nvl( a.ng_qty, 0 ) ) ng_qty ,
	                                        ROW_NUMBER ( ) over ( ORDER BY sum( nvl( a.ng_qty, 0 ) ) DESC )  ranking
                                        FROM
	                                        QCM_CUSTOMER_COMPLAINT_M a
	                                        LEFT JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
                                        WHERE
	                                        a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                        AND to_date( '{EndTime}', 'yyyy-mm-dd' ) {WSQL}
                                        GROUP BY
	                                        a.DEFECT_CONTENT 
                                        ORDER BY
	                                        sum( nvl( a.ng_qty, 0 ) ) DESC";
                RetDic["ReturnReason"] = Common.ReturnCakeData(DB, ReturnReasonSQL, 2, "DEFECT_CONTENT", "ng_qty", "ranking");

                //Category占比
                string CategorySQL = $@"with queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                        SELECT
	                                        e.NAME_T category,
	                                        sum( nvl( a.ng_qty, 0 ) ) ng_qty 
                                        FROM
	                                        QCM_CUSTOMER_COMPLAINT_M a
	                                        JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                        JOIN BDM_SE_ORDER_ITEM c ON b.SE_ID = c.SE_ID
	                                        JOIN bdm_rd_style d ON c.SHOE_NO = d.SHOE_NO
	                                        LEFT JOIN BDM_CD_CODE e ON d.STYLE_SEQ = e.CODE_NO 
	                                        AND LANGUAGE = '1' 
                                            left join queryData q on q.se_id=c.se_id
                                        WHERE
	                                        a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                        AND to_date( '{EndTime}', 'yyyy-mm-dd' ) 
	                                        {CSQL}
                                        GROUP BY
	                                        e.NAME_T 
                                        ORDER BY
	                                        sum( nvl( a.ng_qty, 0 ) ) DESC";
                RetDic["ReturnCategory"] = Common.ReturnCakeData(DB, CategorySQL, 0, "category", "ng_qty", "");

                //前十ART占比
                string ArtSQL = $@" with queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                    SELECT
	                                    d.NAME_T Name,
	                                    sum( nvl( a.ng_qty, 0 ) ) value 
                                    FROM
	                                    QCM_CUSTOMER_COMPLAINT_M a
	                                    JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                    JOIN BDM_SE_ORDER_ITEM c ON b.SE_ID = c.SE_ID
	                                    JOIN bdm_rd_prod d ON c.PROD_NO = d.PROD_NO
	                                    JOIN bdm_rd_style d ON c.SHOE_NO = d.SHOE_NO
	                                    LEFT JOIN BDM_CD_CODE e ON d.STYLE_SEQ = e.CODE_NO 
	                                    AND LANGUAGE = '1' 
                                        left join queryData q on q.se_id=c.se_id
                                    WHERE
	                                    a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                    AND to_date( '{EndTime}', 'yyyy-mm-dd' ) {CSQL}
                                    GROUP BY
	                                    d.NAME_T 
                                    ORDER BY
	                                    sum( nvl( a.ng_qty, 0 ) ) DESC";
                DataTable ArtDt = DB.GetDataTable(ArtSQL);
                RetDic["ReturnArt"] = Common.ReturnTopNumTable(ArtDt, "value","DESC", 10);

                //投诉处理结果占比
                string ResultsSQL = $@"WITH  queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                        ,ORDERDATA AS (
	                                    SELECT
		                                    BM.MER_PO PO,
		                                    BI.PROD_NO ART,
		                                    -- BD.NAME_T CATEGORY,
                                            BC.NAME_T CATEGORY,
                                            BS.NAME_T
		                                    -- BI.SHOE_NO 
	                                    FROM
		                                    BDM_SE_ORDER_ITEM BI
		                                    LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
		                                    LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
		                                    LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
		                                    LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
		                                    AND RULE_NO = 'CATEGORY_NO' 
		                                    AND LANGUAGE = '1'
                                            left join queryData q on q.se_id=BI.se_id
                                        where 1=1 {MonthSQL}
	                                    ) SELECT
                                    CASE
		
	                                    WHEN
		                                    STATUS = '0' 
		                                    AND PROCESSING_RESULTS_STATUS IS NULL THEN
			                                    '处理中' 
			                                    WHEN PROCESSING_RESULTS_STATUS = '0' THEN
			                                    '接收投诉' 
			                                    WHEN PROCESSING_RESULTS_STATUS = '1' THEN
			                                    '客户撤销投诉' 
			                                    WHEN PROCESSING_RESULTS_STATUS = '2' THEN
			                                    '退货处理' 
		                                    END NAME,
	                                    SUM( NVL( NG_QTY, 0 ) ) VALUE 
                                    FROM
	                                    QCM_CUSTOMER_COMPLAINT_M a
	                                    LEFT JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
                                    WHERE
	                                    a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                    AND to_date( '{EndTime}', 'yyyy-mm-dd' ) {WSQL}
                                    GROUP BY
	                                    STATUS,
	                                    PROCESSING_RESULTS_STATUS";
                RetDic["ReturnResults"] = DB.GetDataTable(ResultsSQL);

                //厂区投诉数量占比
                string PlantSQL = $@"WITH depart_tmp AS (
	                                    SELECT
		                                    c.ORG_NAME,
		                                    PRODUCTION_LINE_CODE,
		                                    PRODUCTION_LINE_NAME,
		                                    PLANT_AREA 
	                                    FROM
		                                    bdm_production_line_m a
		                                    LEFT JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
		                                    LEFT JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE UNION ALL
	                                    SELECT
		                                    c.ORG_NAME,
		                                    DEPARTMENT_CODE,
		                                    DEPARTMENT_NAME,
		                                    org 
	                                    FROM
		                                    base005m a
		                                    LEFT JOIN SJQDMS_ORGINFO b ON a.udf05 = b.code
		                                    LEFT JOIN base001m c ON a.FACTORY_SAP = c.ORG_CODE 
	                                    ),  
                                        queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID),
	                                    ORDERDATA AS (
	                                    SELECT
		                                    BM.MER_PO PO,
		                                    BI.PROD_NO ART,
		                                    -- BD.NAME_T CATEGORY,
                                            BC.NAME_T CATEGORY,
                                            BS.NAME_T
		                                    -- BI.SHOE_NO 
	                                    FROM
		                                    BDM_SE_ORDER_ITEM BI
		                                    LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
		                                    LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
		                                    LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
		                                    LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
		                                    AND RULE_NO = 'CATEGORY_NO' 
		                                    AND LANGUAGE = '1' 
                                            {MonthJoinSQL}
                                        where 1=1 {MonthSQL}
	                                    ) SELECT
	                                    PLANT_AREA,
	                                    sum( nvl( a.NG_QTY, 0 ) ) ng_qty 
                                    FROM
	                                    QCM_CUSTOMER_COMPLAINT_M a
	                                    JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                    JOIN sfc_trackin_list e ON a.PO_ORDER = e.PO_NO
	                                    JOIN depart_tmp f ON e.GRT_DEPT = f.PRODUCTION_LINE_CODE
	                                    LEFT JOIN ORDERDATA O ON a.PO_ORDER = o.PO 
                                    WHERE
	                                    a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                    AND to_date( '{EndTime}', 'yyyy-mm-dd' )  {WSQL} {PAWhere}
                                    GROUP BY
	                                    PLANT_AREA 
                                    ORDER BY
	                                    sum( nvl( a.COMPLAINT_MONEY, 0 ) ) DESC";
                RetDic["ReturnPlant"] = Common.ReturnCakeData(DB, PlantSQL, 0, "PLANT_AREA", "ng_qty", "");
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
        /// 主要投诉数量&金额占比【条形图】
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQAMData(object OBJ)
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
                string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//ART
                string ShoeName = jarr.ContainsKey("ShoeName") ? jarr["ShoeName"].ToString() : "";//鞋型名称
                string Category = jarr.ContainsKey("Category") ? jarr["Category"].ToString() : "";//Category
                string ComplaintStr = jarr.ContainsKey("ComplaintStr") ? jarr["ComplaintStr"].ToString() : "";//投诉问题
                string ComplaintCountry = jarr.ContainsKey("ComplaintCountry") ? jarr["ComplaintCountry"].ToString() : "";//投诉国家
                string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//Po
                string PlantArea = jarr.ContainsKey("PlantArea") ? jarr["PlantArea"].ToString() : "";//厂区
                string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";//生产线
                //List<string> ProductionMonth = jarr.ContainsKey("ProductionMonth") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ProductionMonth"]==null?"":jarr["ProductionMonth"].ToString()) : new List<string>(); ;//生产月份
                string ProductionMonth = jarr.ContainsKey("ProductionMonth") ? jarr["ProductionMonth"].ToString() : "";   //生产月份
                string StartTime = jarr.ContainsKey("StartTime") ? jarr["StartTime"].ToString() : "";//开始时间
                string EndTime = jarr.ContainsKey("EndTime") ? jarr["EndTime"].ToString() : "";//结束时间

                string[] orderListart = Art.Split(',');
                string[] orderListshoeName = ShoeName.Split(',');
                string[] orderListcategory = Category.Split(',');
                string[] orderListCS = ComplaintStr.Split(',');
                string[] orderListCC = ComplaintCountry.Split(',');
                string[] orderListPo = Po.Split(',');
                //string[] orderListMonth = ProductionMonth.Split(',');
                string[] oederListPL = ProductionLine.Split(',');
                #endregion

                #region 参数判断
                if (string.IsNullOrEmpty(StartTime) || string.IsNullOrEmpty(EndTime))
                {
                    throw new Exception("缺少必要参数【开始时间】或【结束时间】！！！");
                }
                #endregion

                #region 组合查询SQL
                string WSQL = string.Empty;
                string CSQL = string.Empty;
                string PLWhere = string.Empty;
                if (!string.IsNullOrEmpty(Art))
                {
                    WSQL += $@" AND o.art in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND C.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ShoeName))
                {
                    WSQL += $@" AND o.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(Category))
                {
                    //WSQL += $@" AND o.category in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                    CSQL += $@" AND E.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ComplaintCountry))
                {
                    WSQL += $@" AND a.COUNTRY_REGION like '%{ComplaintCountry}%'";
                    CSQL += $@" AND a.COUNTRY_REGION like '%{ComplaintCountry}%'";
                }
                if (!string.IsNullOrEmpty(ComplaintStr))
                {
                    WSQL += $@" AND a.DEFECT_CONTENT like '%{ComplaintStr}%'";
                    CSQL += $@" AND a.DEFECT_CONTENT like '%{ComplaintStr}%'";
                }
                if (!string.IsNullOrEmpty(Po))
                {
                    WSQL += $@"	AND a.PO_ORDER in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                    CSQL += $@"	AND a.PO_ORDER in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PlantArea))
                {
                    WSQL += $@"";
                    CSQL += $@"";
                }
                if (!string.IsNullOrEmpty(ProductionLine))
                {
                     PLWhere = $@"AND PRODUCTION_LINE_NAME in ({string.Join(',', oederListPL.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ProductionMonth))
                {
                    if (ProductionMonth == "[]")
                    {
                        CSQL += $@"";
                    }
                    else
                    {
                        //WSQL += $@"";
                        //CSQL += $@"and( q.MIN_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) or q.MAX_INSERT_DATE in ({string.Join(',', orderListMonth.Select(x => $"'{x}'"))}) )";
                        CSQL += $@"and( q.MIN_INSERT_DATE = '{ProductionMonth}' or q.MAX_INSERT_DATE = '{ProductionMonth}' )";
                    }
                    
                }
                #endregion

                #region 数据处理
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["ShoeQty"] = null;//鞋型数量
                RetDic["ShoeMoney"] = null;//鞋型金额
                RetDic["LineQty"] = null;//产线数量

                //鞋型数量
                string ShoeQtySQL = $@"with queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                        SELECT
	                                        d.NAME_T Shoe_name,
	                                        sum( nvl( a.ng_qty, 0 ) ) ng_qty 
                                        FROM
	                                        QCM_CUSTOMER_COMPLAINT_M a
	                                        JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                        JOIN BDM_SE_ORDER_ITEM c ON b.SE_ID = c.SE_ID
	                                        JOIN bdm_rd_style d ON c.SHOE_NO = d.SHOE_NO
	                                        LEFT JOIN BDM_CD_CODE e ON d.STYLE_SEQ = e.CODE_NO 
	                                        AND LANGUAGE = '1' 
                                            left join queryData q on q.se_id=c.se_id
                                        WHERE
	                                        a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                        AND to_date( '{EndTime}', 'yyyy-mm-dd' )  {CSQL}
                                        GROUP BY
	                                        d.NAME_T 
                                        ORDER BY
	                                        sum( nvl( a.ng_qty, 0 ) ) DESC";
                DataTable ShoeQtyDt = Common.ReturnOrderByTable(DB.GetDataTable(ShoeQtySQL), "ng_qty", "desc");
                RetDic["ShoeQty"] =Common.ReturnLineData(ShoeQtyDt, "Shoe_name", "ng_qty");//鞋型数量

                //鞋型金额
                string ShoeMoneySQL = $@"with queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID)
                                        SELECT
	                                        d.NAME_T SHOE_NAME,
	                                        sum( nvl( a.COMPLAINT_MONEY, 0 ) ) ng_qty 
                                        FROM
	                                        QCM_CUSTOMER_COMPLAINT_M a
	                                        JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                        JOIN BDM_SE_ORDER_ITEM c ON b.SE_ID = c.SE_ID
	                                        JOIN bdm_rd_style d ON c.SHOE_NO = d.SHOE_NO
	                                        LEFT JOIN BDM_CD_CODE e ON d.STYLE_SEQ = e.CODE_NO 
	                                        AND LANGUAGE = '1' 
                                            left join queryData q on q.se_id=c.se_id
                                        WHERE
	                                        a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                        AND to_date( '{EndTime}', 'yyyy-mm-dd' ) {CSQL}
                                        GROUP BY
	                                        d.NAME_T 
                                        ORDER BY
	                                        sum( nvl( a.COMPLAINT_MONEY, 0 ) ) DESC";
                DataTable ShoeMoneyDt =Common.ReturnOrderByTable(DB.GetDataTable(ShoeMoneySQL), "ng_qty", "desc");
                RetDic["ShoeMoney"] = Common.ReturnLineData(ShoeMoneyDt, "Shoe_name", "ng_qty");//鞋型数量

                //产线数量
                string LineMoneySQL = $@"WITH depart_tmp AS (
	                                    SELECT
		                                    c.ORG_NAME,
		                                    PRODUCTION_LINE_CODE,
		                                    PRODUCTION_LINE_NAME,
		                                    PLANT_AREA 
	                                    FROM
		                                    bdm_production_line_m a
		                                    LEFT JOIN base005m b ON a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
		                                    LEFT JOIN base001m c ON b.FACTORY_SAP = c.ORG_CODE 
	                                    WHERE B.UDF01='L'
                                        UNION ALL
	                                    SELECT
		                                    c.ORG_NAME,
		                                    DEPARTMENT_CODE,
		                                    DEPARTMENT_NAME,
		                                    org 
	                                    FROM
		                                    base005m a
		                                    LEFT JOIN SJQDMS_ORGINFO b ON a.udf05 = b.code
		                                    LEFT JOIN base001m c ON a.FACTORY_SAP = c.ORG_CODE  
                                        WHERE A.UDF01='L' 
	                                    ),queryData as (
                                        SELECT
	                                        SE_ID,
	                                        to_char( MIN( INSERT_DATE ), 'yyyy-mm' ) AS MIN_INSERT_DATE,
	                                        to_char( MAX( INSERT_DATE ), 'yyyy-mm' ) AS MAX_INSERT_DATE 
                                        FROM
	                                        SFC_TRACKOUT_LIST 
                                        WHERE
	                                        PROCESS_NO = 'A' 
                                        GROUP BY
	                                        SE_ID) 
                                    SELECT
	                                    PRODUCTION_LINE_NAME,
	                                    sum( nvl( a.NG_QTY, 0 ) ) ng_qty 
                                    FROM
	                                    QCM_CUSTOMER_COMPLAINT_M a
	                                    JOIN bdm_se_order_master b ON a.PO_ORDER = b.MER_PO
	                                    JOIN BDM_SE_ORDER_ITEM c ON b.SE_ID = c.SE_ID
	                                    JOIN bdm_rd_style d ON c.SHOE_NO = d.SHOE_NO
	                                    LEFT JOIN BDM_CD_CODE e ON d.STYLE_SEQ = e.CODE_NO 
	                                    AND LANGUAGE = '1'
	                                    JOIN sfc_trackin_list f ON a.PO_ORDER = f.PO_NO
	                                    JOIN depart_tmp g ON f.GRT_DEPT = g.PRODUCTION_LINE_CODE    
                                        left join queryData q on q.se_id=c.se_id
                                    WHERE
	                                    a.COMPLAINT_DATE BETWEEN to_date( '{StartTime}', 'yyyy-mm-dd' ) 
	                                    AND to_date( '{EndTime}', 'yyyy-mm-dd' ) {CSQL} {PLWhere}
                                    GROUP BY
	                                    PRODUCTION_LINE_NAME 
                                    ORDER BY
	                                    sum( nvl( a.COMPLAINT_MONEY, 0 ) ) DESC";
                DataTable LineMoneyDt = Common.ReturnOrderByTable(DB.GetDataTable(LineMoneySQL), "ng_qty", "desc");
                RetDic["LineQty"] = Common.ReturnLineData(LineMoneyDt, "PRODUCTION_LINE_NAME", "ng_qty");//鞋型数量


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

    }
}
