using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SJ_KanBanAPI.Dto;
using System.Reflection;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// AQL验货订单看板
    /// </summary>
    internal class AQLInterface
    {
        //static string ui_lan_type = "zh";
        static string ui_lan_type = "en";
        static string moudle_code = "poInformation";
		/// <summary>
		/// 获取PO信息列表
		/// </summary>
		/// <param name="OBJ"></param>
		/// <returns></returns>
		/// 


		private static DateTime LastDayOfYear(DateTime datetime)
		{
			DateTime AssemblDate = Convert.ToDateTime(datetime.AddYears(1).Year + "-" + "01" + "-" + "01");  // 组装当前指定月份
			return AssemblDate.AddDays(-1);
		}

		public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShipmentByMonth(object OBJ)
		{
			SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
			SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
			SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
			try
			{
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
				string Data = ReqObj.Data.ToString();
				var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

				#region PO运输
				ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "en";
				string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 
				string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : DateTime.Now.AddMonths(-11).ToString("yyyy-MM-dd");//开始时间
				string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : DateTime.Now.ToString("yyyy-MM-dd");//结束时间
				string category = jarr.ContainsKey("category") ? jarr["category"].ToString() : "";//Category
				string shoeName = jarr.ContainsKey("shoeName") ? jarr["shoeName"].ToString() : "";//鞋型名称
				string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//Art
				string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
				string shipCountryName = jarr.ContainsKey("shipCountryName") ? jarr["shipCountryName"].ToString() : "";//出货国家
				string inspection = jarr.ContainsKey("inspection") ? jarr["inspection"].ToString() : "";//验货状态
				string shipment = jarr.ContainsKey("shipment") ? jarr["shipment"].ToString() : "";//出货状态
				string stockDull = jarr.ContainsKey("stockDull") ? jarr["stockDull"].ToString() : "";//库存状态
																									 //string plantArea = jarr.ContainsKey("plantArea") ?string.IsNullOrEmpty(jarr["plantArea"].ToString())?"1001": jarr["plantArea"].ToString() : "1001";//厂区
				string plantArea = jarr.ContainsKey("plantArea") ? jarr["plantArea"].ToString() : "";//厂区
				string finshed = jarr.ContainsKey("finshed") ? jarr["finshed"].ToString() : "";//成品仓
				string risk = jarr.ContainsKey("risk") ? jarr["risk"].ToString() : "";//品质风险
				string site = jarr.ContainsKey("site") ? jarr["site"].ToString() : "";//储位

				string[] orderListshoeName = shoeName.Split(','); //鞋型
				string[] orderListcategory = category.Split(','); //Category
				string[] orderListart = art.Split(',');           //art
				string[] orderListPo = po.Split(',');             //po
				string[] orderListgj = shipCountryName.Split(',');//出货国家
				string[] orderListSheif = site.Split(',');        //储位
				string[] orderListorg = plantArea.Split(',');     //厂区
				string[] orderListware = finshed.Split(',');

				string ShipmentWhere = string.Empty;//验货

				if (!string.IsNullOrEmpty(shipCountryName))
				{
					ShipmentWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
				}

					DateTime start = new DateTime();
				DateTime end = new DateTime(); 

				if (TYPE == "0")
				{
					start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
					end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
				}
				else
				{ 
					start = DateTime.Parse(startTime);
					end = DateTime.Parse(endTime);
				}
				int months = end.Month - start.Month + 1; 
				if (months <= 0)
				{
					throw new Exception("Please select the correct date range！");
				}


				startTime = start.ToString("yyyy-MM-dd");
				endTime = end.ToString("yyyy-MM-dd"); 
				string shipmentsql = $@"with tmp as (select* from (select

		 a.po_no,
         d.nst as planned_date, 
         d.CR_REQDATE as CR_date,
       (select max(posting_date) from bmd_se_shipment_m where se_id = a.se_id) as shipping_date

	from BDM_SE_SHIPMENT_NOTIFICATION_M a

	left
	join BDM_SE_ORDER_ITEM d on a.se_id = d.se_id
    left join BDM_SE_ORDER_MASTER c on a.se_id = c.se_id
    LEFT JOIN BDM_COUNTRY e on c.descountry_code = e.c_no and e.l_no='EN'
	where a.status = '7' {ShipmentWhere}

	order by shipping_date desc nulls last )tab where 1 = 1 and TO_CHAR(tab.shipping_date  ,'yyyy-MM-dd HH24:mi:ss') BETWEEN '{startTime}' AND '{endTime}'),
    tmp2 as (select tmp.*,CASE WHEN TO_CHAR(shipping_date, 'yyyy-mm-dd') <= TO_CHAR(CR_date, 'yyyy-mm-dd')  THEN 0 ELSE 1 END as status from tmp) 
    select TO_CHAR( tmp2.shipping_date  ,'yyyy-MM') as shipping_date,
    sum(CASE WHEN tmp2.status = '0' THEN 1 else 0 end ) as Ontime,
    sum (CASE WHEN tmp2.status = '1' THEN 1 else 0 end ) as Delay,
    '' as Rate
    from tmp2 group by TO_CHAR( tmp2.shipping_date  ,'yyyy-MM')";
				

				#endregion

				var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "准时发货 数量", "延迟交货数量", "准时出货率" });
				string name1 = string.Empty;
				string name2 = string.Empty;
				string name3 = string.Empty;
				if (LanDic.Count > 0)
				{
					name1 = LanDic["准时发货 数量"].ToString();
					name2 = LanDic["延迟交货数量"].ToString();
					name3 = LanDic["准时出货率"].ToString();
				}

				List<string> Xdata = new List<string>();

				List<KanBanDtos> res = new List<KanBanDtos>();

				KanBanDtos data_ontime = new KanBanDtos();
				data_ontime.type = "bar";
				data_ontime.name = name1;

				KanBanDtos data_delay = new KanBanDtos();
				data_delay.type = "bar";
				data_delay.name = name2;

				KanBanDtos data_rate = new KanBanDtos();
				data_rate.type = "line";
				data_rate.name = name3;

				List<string> OnTime = new List<string>();
				List<string> Delay = new List<string>();
				List<string> OnTimeRate = new List<string>();
				var dt = DB.GetDataTable(shipmentsql);

				string rate = string.Empty;
				foreach (DataRow item in dt.Rows)
				{
					decimal Ontime = item["Ontime"].ToString() == "" ? 0 : Convert.ToDecimal(item["Ontime"].ToString());
					decimal delay = item["Delay"].ToString() == "" ? 0 : Convert.ToDecimal(item["Delay"].ToString());


					if (Ontime + delay == 0)
						item["Rate"] = "0";
					else
					{
						item["Rate"] = Math.Round((Ontime / (Ontime + delay)) * 100, 1);
					}

				}

				

				

				if (TYPE == "1")
				{
					for (int i = 0; i < (end.Day - start.Day) + 1; i++)
					{
						Xdata.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
					}
				}
				else
				{
					//遍历N个月
					for (int i = 0; i < months; i++)
					{
						Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));

					}

				}

				foreach (var item in Xdata)
				{
					var dr = dt.Select($"shipping_date = '{item}'");
					if (dr.Length > 0)
					{
						OnTime.Add(dr[0]["Ontime"].ToString());
						Delay.Add(dr[0]["Delay"].ToString());
						OnTimeRate.Add(dr[0]["Rate"].ToString());

					}
					else
					{
						OnTime.Add("0");
						Delay.Add("0");
						OnTimeRate.Add("0");
					}
				}
				data_ontime.data = OnTime;
				data_delay.data = Delay;
				data_rate.data = OnTimeRate;

				res.Add(data_ontime);
				res.Add(data_delay);
				res.Add(data_rate);

				Dictionary<string, object> dic = new Dictionary<string, object>();
				dic.Add("Xdata", Xdata);
				dic.Add("Data", res);

				ret.IsSuccess = true;
				ret.RetData1 = dic; 
			}
			catch (Exception ex)
			{
				ret.IsSuccess = false;
				ret.ErrMsg = ex.Message;
			}
            return ret;
		}

	    public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPOData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
				DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 条件查询
                ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "en";
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : DateTime.Now.AddMonths(-11).ToString("yyyy-MM-dd");//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : DateTime.Now.ToString("yyyy-MM-dd");//结束时间
                string category = jarr.ContainsKey("category") ? jarr["category"].ToString() : "";//Category
                string shoeName = jarr.ContainsKey("shoeName") ? jarr["shoeName"].ToString() : "";//鞋型名称
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//Art
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                string shipCountryName = jarr.ContainsKey("shipCountryName") ? jarr["shipCountryName"].ToString() : "";//出货国家
                string inspection = jarr.ContainsKey("inspection") ? jarr["inspection"].ToString() : "";//验货状态
                string shipment = jarr.ContainsKey("shipment") ? jarr["shipment"].ToString() : "";//出货状态
				string stockDull = jarr.ContainsKey("stockDull") ? jarr["stockDull"].ToString() : "";//库存状态
                //string plantArea = jarr.ContainsKey("plantArea") ?string.IsNullOrEmpty(jarr["plantArea"].ToString())?"1001": jarr["plantArea"].ToString() : "1001";//厂区
                string plantArea = jarr.ContainsKey("plantArea") ? jarr["plantArea"].ToString() : "";//厂区
                string finshed = jarr.ContainsKey("finshed") ? jarr["finshed"].ToString() : "";//成品仓
                string risk=jarr.ContainsKey("risk") ? jarr["risk"].ToString() : "";//品质风险
                string site = jarr.ContainsKey("site") ? jarr["site"].ToString() : "";//储位

                string[] orderListshoeName = shoeName.Split(','); //鞋型
                string[] orderListcategory = category.Split(','); //Category
                string[] orderListart = art.Split(',');			  //art
                string[] orderListPo = po.Split(',');			  //po
                string[] orderListgj = shipCountryName.Split(',');//出货国家
                string[] orderListSheif = site.Split(',');		  //储位
                string[] orderListorg = plantArea.Split(',');	  //厂区
                string[] orderListware = finshed.Split(',');	  //成品仓

                string InspectionWhere = string.Empty;//验货
				string RiskWhere = string.Empty;//质量异常
				string TableWhere = string.Empty;//表格
				string StockDullWhere = string.Empty;// 库存
				string CountryWhere = string.Empty;// 国家
				string DullWhere = string.Empty;// 呆滞品
				string StockWhere = string.Empty;// 库存
				string CategoryWhere = string.Empty;// 呆滞品
				string InstockWhere = string.Empty;// 呆滞品
                string InstockWhere1 = string.Empty;
                string InstockWhere2 = string.Empty;
				string kcdullWhere = string.Empty;
				string SHILWhere1 = string.Empty;//厂区、成品仓联合查询
                string SHILWhere2 = string.Empty;//厂区、成品仓联合查询

                if (!string.IsNullOrEmpty(art))
				{
                    InspectionWhere += $@" AND bi.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					RiskWhere += $@" AND BM.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					TableWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					StockDullWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					CountryWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					DullWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					StockWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					CategoryWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
					InstockWhere += $@" AND BI.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
				if (!string.IsNullOrEmpty(po))
				{
					InspectionWhere += $@" and bm.mer_po in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					RiskWhere += $@" and B.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					TableWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					StockDullWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					CountryWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					DullWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					StockWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					CategoryWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
					InstockWhere += $@" and BM.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))}) ";
				}
				if (!string.IsNullOrEmpty(shipCountryName))
				{
					InspectionWhere += $@" AND bm.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					//RiskWhere += $@" AND B.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					RiskWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					//TableWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					TableWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					StockDullWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					//CountryWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					CountryWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					DullWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					StockWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					//CategoryWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					CategoryWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					//InstockWhere += $@" AND BM.descountry_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
					InstockWhere += $@" AND e.c_name in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))}) ";
				}
                if (!string.IsNullOrEmpty(category))
                {
					CategoryWhere = $@"AND BC.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
					TableWhere = $@"AND CATEData.CATEGORY in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";

                }
                if (!string.IsNullOrEmpty(shoeName))
                {
					InspectionWhere += $@" AND bi.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					RiskWhere += $@" AND BM.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					TableWhere += $@" AND CATEData.NAME_S in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					StockDullWhere += $@" AND BI.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					DullWhere += $@" AND BI.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					StockWhere += $@" AND BI.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					CategoryWhere += $@" AND BI.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
					InstockWhere += $@" AND BI.SHOE_NO in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
				}
                if (!string.IsNullOrEmpty(plantArea))
                {
                    InstockWhere1 += $@" AND udf05 in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))}) ";
                    InstockWhere2 += $@" AND B.udf05 in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))}) ";
                    TableWhere += $@" AND m5.udf05 in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))}) ";
                    //SHILWhere1 += $@" AND ORG_ID in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))}) ";
                    //SHILWhere2 += $@" AND d.factory_no in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(finshed))
                {
                    SHILWhere1 += $@" AND STOC_NO in ({string.Join(',', orderListware.Select(x => $"'{x}'"))}) ";
                    SHILWhere2 += $@" AND d.warehourse_no in ({string.Join(',', orderListware.Select(x => $"'{x}'"))}) ";
                }

                string InspStr = string.Empty;
                if (!string.IsNullOrEmpty(inspection))
                {
                    switch (inspection)
                    {
						case "0":
							TableWhere += " and AQLStatus.inspection_state='0'";
							InspStr += " and INSPECTION_STATE='0'";
							break;
						case "1":
							TableWhere += " and AQLStatus.inspection_state='1'";
							InspStr += " and INSPECTION_STATE='1'";
							break;
						case "2":
							TableWhere += " and AQLStatus.inspection_state='1'";
							InspStr += " and INSPECTION_STATE='1' AND RESULT='Accept'";
							break;
						case "3":
							TableWhere += " and AQLStatus.inspection_state='1'";
							InspStr += " and INSPECTION_STATE='1' AND RESULT='Reject'";
							break;
						case "4":
							TableWhere += " and AQLStatus.inspection_state='1'";
							InspStr += " and INSPECTION_STATE='1' AND RESULT='Accept'";
							break;
                    }
                }
                if (!string.IsNullOrEmpty(shipment))
                {
					switch (shipment)
					{
						case "0":
							TableWhere += " AND NVL( OutData.qty, 0 ) >= BI.SE_QTY";
							break;
						case "2":
							TableWhere += " AND NVL( InData.qty, 0 ) < BI.SE_QTY AND NVL(OutData.qty, 0 ) = 0";
							break;
						case "1":
							TableWhere += " AND NVL( InData.qty, 0 ) >= BI.SE_QTY AND NVL(OutData.qty, 0 ) = 0";
							break;
					}
				}
                if (!string.IsNullOrEmpty(site))
                {
					TableWhere += $@"and wl.SHELF_NO in ({string.Join(',', orderListSheif.Select(x => $"'{x}'"))}) ";
				}

				string top5where = string.Empty;

				if (!string.IsNullOrEmpty(risk))
                {
					//top5where += $@" and (qa_risk_desc='{risk}' or qa_risk_desc='{risk}' or qa_risk_desc='{risk}' or qa_risk_desc='{risk}' or qa_risk_desc='{risk}') ";
                    top5where += $@" and QA_RISK_CATEGORY_CODE='{risk}' ";
                    RiskWhere += $@" AND QD.QA_RISK_CATEGORY_CODE = '{risk}' ";
                }
				if (!string.IsNullOrEmpty(stockDull))
				{
                    string dull = string.Empty;
                    if (stockDull == "0")
                    {
                        dull += "一个月以上呆滞品,";
                    }
                    else if (stockDull == "1")
                    {
                        dull += "两个月以上呆滞品,";
                    }
                    else if (stockDull == "2")
                    {
                        dull += "三个月以上呆滞品,";
                    }
                    else if (stockDull == "3")
                    {
                        dull += "四个月以上呆滞品,";
                    }
                    else if (stockDull == "4")
                    {
                        dull += "一个月以内呆滞品";
                    }
                    string[] orderListdull = dull.Split(',');
					kcdullWhere = $@" and dull in ({string.Join(',', orderListdull.Select(x => $"'{x}'"))}) ";
                }
				//库存呆滞
				

                #endregion
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                RetDic["PoInformationList"] = null;//Po信息列表数据
                RetDic["OrderList"] = null;//订单信息列表 

                #region PO信息列表
                Dictionary<string, object> RateDic = new Dictionary<string, object>();
                RateDic["Inspection"] = null;//验货占比
                RateDic["QualityRisk"] = null;//品质风险占比
                RateDic["Dull"] = null;//呆滞品占比
                RateDic["Stock"] = null;//仓储及生产情况展示
                RateDic["Category"] = null;//Category占比
                RateDic["Shipment"] = null;//国家出货
                RateDic["StockDull"] = null;//库存呆滞占比


				#region 国家出货占比
				string CountyRateSQL = $@"SELECT
												NVL( e.c_name, 'Other' ) DESCOUNTRY_NAME,
												NVL( SUM( SE_QTY ), 0 ) qty 
											FROM
												BDM_SE_ORDER_ITEM BI
												LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID 
                                                left join BDM_COUNTRY e on BM.descountry_code = e.c_no and e.l_no='EN'
											WHERE
												TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
												AND '{endTime}' {CountryWhere}
											GROUP BY
												e.c_name";
				RateDic["Shipment"]= Common.ReturnCakeData(DB, CountyRateSQL,0, "DESCOUNTRY_NAME", "qty","",ui_lan_type, moudle_code);
				#endregion

				#region  Category占比
				string CategoryRateSQL = $@"SELECT
										nvl( BC.NAME_T, 'Other' ) CATEGORY,
										NVL( SUM( SE_QTY ), 0 ) qty 
									FROM
										BDM_SE_ORDER_ITEM BI
										LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID 
                                        LEFT JOIN BDM_COUNTRY e on BM.descountry_code = e.c_no and e.l_no='EN'
										LEFT JOIN BDM_RD_PROD BD ON BI.PROD_NO = BD.PROD_NO
										LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
										LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
										AND RULE_NO = 'CATEGORY_NO' 
										AND LANGUAGE = '1' 
									WHERE
										TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
										AND '{endTime}' {CategoryWhere}
									GROUP BY
										BC.NAME_T";
				RateDic["Category"] = Common.ReturnCakeData(DB, CategoryRateSQL, 0, "CATEGORY", "qty", "",ui_lan_type, moudle_code);//Category占比
				#endregion

				#region 呆滞品占比
				/*
呆滞库存：当前月份无出货计划的库存总数

待出货库存：当前月份计划出货的库存总数

呆滞率：呆滞库存÷总库存 X 100%
				 */
				string DullRateSql = $@"SELECT
											'Sluggish_Stock' AS name,
											NVL(
												SUM( CASE WHEN TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN wb.STOC_QTY ELSE 0 END ),
												0 
											) qty 
										FROM
											BDM_SE_ORDER_ITEM BI
											LEFT JOIN WMS_STOC_BATCH WB ON BI.SE_ID = WB.BATCH_NO
											LEFT JOIN BDM_SE_ORDER_MASTER BM ON BM.SE_ID = BI.SE_ID 
										WHERE
											WB.OUT_QTY < BI.SE_QTY
										UNION ALL
										SELECT
											'Inventory_to_be_shipped' AS name,
											NVL( SUM( wb.STOC_QTY ), 0 )- NVL(SUM( CASE WHEN TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN wb.STOC_QTY ELSE 0 END ),0 ) qty 
										FROM
											BDM_SE_ORDER_ITEM BI
											LEFT JOIN WMS_STOC_BATCH WB ON BI.SE_ID = WB.BATCH_NO
											LEFT JOIN BDM_SE_ORDER_MASTER BM ON BM.SE_ID = BI.SE_ID 
										WHERE
											1=1
											AND TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
											AND '{endTime}' {DullWhere}";
				Dictionary<string, object> DullDic = new Dictionary<string, object>();
				string DullRateSQL = $@"WITH STOCKDATA AS (
										SELECT
											TO_CHAR( BI.NLT, 'yyyy-mm-DD' ) YMD,
											TO_CHAR( BI.NLT, 'yyyy-mm' ) YM,
											NVL( SUM( wb.STOC_QTY ), 0 ) qty 
										FROM
											BDM_SE_ORDER_ITEM BI
											LEFT JOIN WMS_STOC_BATCH WB ON BI.SE_ID = WB.BATCH_NO
											LEFT JOIN BDM_SE_ORDER_MASTER BM ON BM.SE_ID = BI.SE_ID 
										WHERE
											WB.OUT_QTY < BI.SE_QTY 
											AND  wb.STOC_QTY>0
										GROUP BY
											BI.NLT 
										),
										QTYDATA AS (
										SELECT
											NVL(SUM( QTY ), 0)  ALLQTY 
										FROM
											STOCKDATA 
										WHERE
											YM = TO_CHAR( SYSDATE, 'yyyy-mm' ) 
											 UNION ALL
										SELECT
											NVL(SUM( QTY ), 0)  DULLQTY 
										FROM
											STOCKDATA 
										WHERE
											YM != TO_CHAR( SYSDATE, 'yyyy-mm' ) 
										) 
										SELECT
											* 
										FROM
											QTYDATA";
				DataTable DullRateDt = DB.GetDataTable(DullRateSQL);
				DullDic["rate"] ="0%";
				if (DullRateDt.Rows.Count>0&&DullRateDt.Rows.Count==2)
                {
					decimal OutQty = Convert.ToDecimal(DullRateDt.Rows[0]["ALLQTY"].ToString());
					decimal DullQty = Convert.ToDecimal(DullRateDt.Rows[1]["ALLQTY"].ToString());
					DullDic["rate"] = Math.Round(DullQty/(DullQty+OutQty)*100,1)+"%";
				}

				DullDic["data"]= Common.ReturnCakeData(DB, DullRateSql, 0, "name", "qty", "",ui_lan_type,moudle_code);
				RateDic["Dull"] = DullDic;//呆滞品占比
				#endregion

				#region 仓储情况以及生产情况
				string stockAndProductionSql = $@"WITH STOCKDT AS ( SELECT BATCH_NO, SUM( STOC_QTY ) STOC_QTY, SUM( IN_QTY ) IN_QTY FROM WMS_STOC_BATCH GROUP BY BATCH_NO ) SELECT
													'PO_Production' AS name,
													 NVL(SUM( CASE WHEN WB.IN_QTY >= BI.SE_QTY THEN SE_QTY ELSE 0 END ),0) KC 
													FROM
														BDM_SE_ORDER_ITEM BI
														LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
														JOIN STOCKDT WB ON WB.BATCH_NO = BI.SE_ID 
													WHERE
														1=1 UNION ALL
													SELECT
														'PO_Total_Inventory' AS name,
														NVL(SUM( STOC_QTY ),0) KC1q 
													FROM
														BDM_SE_ORDER_ITEM BI
														LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID
														JOIN STOCKDT WB ON WB.BATCH_NO = BI.SE_ID 
													WHERE
														1=1 {StockWhere}";
				decimal PlanQty = Math.Round(Convert.ToDecimal(DB.GetString($@"	SELECT
														NVL( SUM( SE_QTY ), 0 ) allqty 
													FROM
														BDM_SE_ORDER_ITEM BI
														LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID 
													WHERE
														TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
														AND '{endTime}' {StockWhere} ")),0);
				decimal HavingQty = Math.Round(Convert.ToDecimal(DB.GetString($@"	SELECT
						nvl( sum( shipping_qty ), 0 ) AS shipping_qty 
					FROM
						bmd_se_shipment_m m
						LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no
						LEFT JOIN BDM_SE_ORDER_ITEM BI ON BI.SE_ID = M.SE_ID
						LEFT JOIN BDM_SE_ORDER_MASTER BM ON BI.SE_ID = BM.SE_ID 
					WHERE
						d.factory_no = '1001' 
						AND d.warehourse_no = '2000' AND TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
					  AND '{endTime}' {StockWhere} ")), 0);
				Dictionary<string,object> QtyDic=new Dictionary<string,object>();
				//QtyDic["outName"] = "发货率";
				QtyDic["outName"] = "Shipment_Rate";
				QtyDic["outRate"] = PlanQty==0?"0":Math.Round(HavingQty / PlanQty, 3).ToString("p");
				//QtyDic["ratioName"] = "已发货数量/计划发货数量";
				QtyDic["ratioName"] = "Shipped Quantity/Planned Shipment Quantity";
				QtyDic["ratio"] =HavingQty+"/"+PlanQty;

				Dictionary<string, object> SPDic = new Dictionary<string, object>();
				SPDic["rate"] = QtyDic;
				SPDic["data"] = Common.ReturnCakeData(DB, stockAndProductionSql, 0, "name", "KC", "",ui_lan_type, moudle_code);//Category占比
				RateDic["ratio"] = SPDic;//呆滞品占比

				#endregion

				#region 验货占比
				string InspectionDataSQL = $@"with bad_data as (
	                                            SELECT
													a.TASK_NO,
													a.LOT_NUM,
													a.INSPECTION_STATE,
													sum( nvl( b.bad_qty, 0 ) ) bad_qty 
												FROM
													aql_cma_task_list_m a
													LEFT JOIN aql_cma_task_list_m_aql_e_br b ON b.TASK_NO = a.TASK_NO 
													LEFT JOIN BDM_SE_ORDER_MASTER bm on a.po =bm.mer_po
													LEFT JOIN BDM_SE_ORDER_ITEM bi on bm.se_id =bi.se_id
												WHERE
													TO_CHAR(bi.NLT,'yyyy-mm-dd') BETWEEN '{startTime}' 
													AND '{endTime}' {InspectionWhere}
												GROUP BY
													a.TASK_NO,
													a.LOT_NUM,
													a.INSPECTION_STATE
                                            ),
                                            aql_spec_tmp as (
	                                            select
	                                            a.TASK_NO ,
	                                            a.INSPECTION_STATE,
	                                            a.lot_num,
	                                            a.bad_qty,
	                                            nvl(b.sample_level,2) sample_level,
	                                            nvl(b.aql_level ,'AC13') aql_level
	                                            from bad_data a
	                                            left join  aql_cma_task_list_m_aql_m b on a.TASK_NO =b.TASK_NO 
                                            ),
                                            aql_spec_tmp2 as (
                                                select * from (
                                                select HORI_TYPE,LEVEL_TYPE,START_QTY,END_QTY,AC01,AC02,AC03,AC04,AC05,AC06,AC07,AC08,AC09,AC10,AC11,AC12,AC13,AC14,AC15,AC16,AC17,AC18,AC19,AC20,AC21,AC22,AC23,AC24,AC25,AC26,AC27 from BDM_AQL_M
                                                where HORI_TYPE='2'
                                                )
                                                unpivot(
                                                    aql_val for JOB in (
                                                    AC01,AC02,AC03,AC04,AC05,AC06,AC07,AC08,AC09,AC10,AC11,AC12,AC13,AC14,AC15,AC16,AC17,AC18,AC19,AC20,AC21,AC22,AC23,AC24,AC25,AC26,AC27
                                                    )
                                                )
                                            ),
                                            aql_data as (
	                                            select a.TASK_NO ,
	                                            a.INSPECTION_STATE,--已检未检
	                                            a.lot_num,--批数量
	                                            a.bad_qty,
	                                            b.aql_val ,
	                                            case when a.bad_qty>b.aql_val then 'Reject' else 'Accept' end result--AQL结果
	                                            from aql_spec_tmp a
	                                            join aql_spec_tmp2 b on a.sample_level=b.LEVEL_TYPE and to_number(b.START_QTY)<=a.LOT_NUM and to_number(b.END_QTY)>=a.LOT_NUM and a.aql_level=b.job
                                            )
                                            select 
                                            INSPECTION_STATE ,result,sum(lot_num) lot_num
                                            from aql_data where 1=1 {InspStr}
                                            group by INSPECTION_STATE,result";
                DataTable InspectionDt = DB.GetDataTable(InspectionDataSQL);

                List<Dictionary<string, object>> InspeCtionList = new List<Dictionary<string, object>>();
                decimal passNum = 0;
                decimal failNum = 0;

                #region 初始化类型
                //多语言

               var LanDic = Common.GetLanguagebyKanBan(ui_lan_type,moudle_code,new List<string>(){"已验货数量(PASS)","已验货数量(FAIL)", "未验货数量" });
                //var LanDic = Common.GetLanguagebyKanBan(ui_lan_type,moudle_code,new List<string>(){ "Quantity inspected (PASS)", "Quantity inspected (FAIL)", "Non-inspected quantity" });
                for (int i = 0; i < 3; i++)
                {
					Dictionary<string, object> Dic = new Dictionary<string, object>();

                    switch (i)
                    {
						case 0:
							
							Dic["inspection_state"] = "1";//检验状态 是否验货
							Dic["result"] = "accept";//
							Dic["name"] = LanDic["已验货数量(PASS)"] ;
							Dic["value"] = "0";
							break;
						case 1:
							
							Dic["inspection_state"] = "1";
							Dic["result"] = "reject";//
							Dic["name"] = LanDic["已验货数量(FAIL)"];
							Dic["value"] = "0";

							break;
						case 2:
							Dic["inspection_state"] = "0";
							Dic["result"] = "accept";//
							Dic["name"] = LanDic["未验货数量"];
							Dic["value"] = "0";
							break;

						default:
                            break;
                    }
					InspeCtionList.Add(Dic);
				}

				foreach (var item in InspeCtionList)
				{
					var dr = InspectionDt.Select($"INSPECTION_STATE ={item["inspection_state"]}  and RESULT = '{item["result"]}'");


					if (dr.Length > 0)
					{
						if (item["inspection_state"].ToString() == "1" && item["result"].ToString().ToLower().Contains("accept"))
						{
							passNum += Convert.ToDecimal(dr[0]["lot_num"].ToString());
						}
						else
						{
							failNum += Convert.ToDecimal(dr[0]["lot_num"].ToString());
						}
						item["value"] = Convert.ToDecimal(dr[0]["lot_num"].ToString());
					}

				}

                #endregion
                #region
                //           foreach (DataRow item in InspectionDt.Rows)
                //           {
                //               Dictionary<string, object> Dic = new Dictionary<string, object>();
                //               //Dic["name"] = null;
                //               Dic["name"] = null;
                //               Dic["value"] = null;


                //               switch (item["INSPECTION_STATE"].ToString())
                //               {
                //                   case "1":

                //                       if (item["RESULT"].ToString().ToLower().Contains("accept"))
                //                       {
                //                           passNum += Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                           Dic["name"] = "已验货数量(PASS)";
                //                           Dic["value"] = Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                       }

                //                       //if (item["RESULT"].ToString().ToLower().Contains("reject"))
                //                       //                     {
                //                       //	failNum += Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                       //	Dic["name"] = "已验货数量(FAIL)";
                //                       //	Dic["value"] = Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                       //}
                //                       else
                //                       {
                //                           failNum += Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                           Dic["name"] = "已验货数量(FAIL)";
                //                           Dic["value"] = Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                       }
                //                       break;
                //                   case "0":
                //                       Dic["name"] = "未验货数量";
                //                       Dic["value"] = Convert.ToDecimal(item["LOT_NUM"].ToString());
                //                       break;
                //                   default:
                //                       break;
                //               }
                //decimal Num = Convert.ToDecimal(string.IsNullOrEmpty(Dic["value"].ToString()) ? "0" : Dic["value"].ToString());
                //               if (Num>0)
                //               {
                //	InspeCtionList.Add(Dic);
                //}
                //           }
                #endregion
                Dictionary<string, object> InDic = new Dictionary<string, object>();
                if (passNum+failNum>0)
                {
					InDic["rate"] = Math.Round(passNum / (passNum + failNum)*100,1)+"%";//验货通过率
				}
                else
                {
					InDic["rate"] = "0%";//验货通过率
				}
                InDic["data"] = InspeCtionList;
                RateDic["Inspection"] = InDic;//验货占比
				#endregion

				#region 品质风险占比
				string ErrorData = $@"WITH DATA AS
									(SELECT
										QD.qa_risk_desc,
										COUNT( QD.qa_risk_desc ) num ,
										ROW_NUMBER() OVER (ORDER BY COUNT( QD.qa_risk_desc ) DESC) RANKING
									FROM
										qcm_dqa_mag_d QD
										LEFT JOIN BDM_SE_ORDER_ITEM BM ON QD.SHOES_CODE=BM.SHOE_NO
										LEFT JOIN BDM_SE_ORDER_MASTER B ON BM.SE_ID=B.SE_ID
                                        LEFT JOIN BDM_COUNTRY e on B.descountry_code = e.c_no and e.l_no='EN'
									WHERE
										QD.isdelete = '0' 
										AND TO_CHAR(BM.NLT,'yyyy-mm-dd') BETWEEN '{startTime}' AND  '{endTime}'  {RiskWhere}
									GROUP BY
										QD.QA_RISK_DESC)
										SELECT qa_risk_desc, num FROM DATA WHERE RANKING<=5
										UNION ALL
										SELECT 'Other', NVL(SUM(NUM),0)  FROM DATA WHERE RANKING<=5";
				DataTable ErrorTb = DB.GetDataTable(ErrorData);
				List<Dictionary<string, object>> ErrorList = new List<Dictionary<string, object>>();
                foreach (DataRow item in ErrorTb.Rows)
                {
					decimal Num = Convert.ToDecimal(string.IsNullOrEmpty(item["NUM"].ToString()) ? "0" : item["NUM"].ToString());
					if (Num>0)
                    {
						Dictionary<string, object> Dic = new Dictionary<string, object>();
						Dic["name"] = item["QA_RISK_DESC"].ToString();
						Dic["value"] = item["NUM"].ToString();
						ErrorList.Add(Dic);
					}
                }
				RateDic["QualityRisk"] = ErrorList;//品质风险占比
				#endregion

				#region 库存呆滞占比 
				string DullSQL = $@"WITH DayData AS (
									SELECT
										t1.SE_ID,
										TO_CHAR( t1.startDate, 'yyyy-mm-dd' ) startDate,
										NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ) endDate,
										TO_DATE( NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ), 'yyyy-mm-dd' ) - t1.startDate days 
									FROM
										( SELECT SE_ID, max( insert_date ) AS startDate FROM mms_finishedtrackin_list WHERE org_id = '5001' AND stoc_no = '2000' GROUP BY SE_ID ) t1
										LEFT JOIN (
										SELECT
											m.SE_ID,
											max( posting_date ) AS endDate 
										FROM
											bmd_se_shipment_m m
											LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
										WHERE
											d.factory_no = '5001' 
											AND d.warehourse_no = '2000' 
										GROUP BY
											m.SE_ID 
										) t2 ON t1.SE_ID = t2.SE_ID 
									),
									temp1 AS (
									SELECT
									CASE
			
										WHEN
											DayData.DAYS < 30 
											AND TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN
												'Sluggish goods within one month' 
												WHEN DayData.DAYS > 30 
												AND DayData.DAYS < 60 
												AND TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN
													'Over one month sluggish goods' 
													WHEN DayData.DAYS > 60 
													AND DayData.DAYS < 90 
													AND TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN
														'Sluggish goods more than two months old' 
														WHEN DayData.DAYS > 90 
														AND DayData.DAYS < 120 
														AND TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN
															'Sluggish goods more than three months old' 
															WHEN DayData.DAYS > 120 
															AND TO_CHAR( BI.NLT, 'yyyy-mm' ) != TO_CHAR( SYSDATE, 'yyyy-mm' ) THEN
																'Over four months sluggish goods' 
																END AS Dull 
														FROM
															BDM_SE_ORDER_MASTER BM
															LEFT JOIN BDM_SE_ORDER_ITEM BI ON BM.SE_ID = BI.SE_ID
															JOIN DayData ON DayData.se_id = bi.se_id 
														--WHERE
															--TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
															--AND '{endTime}' {StockDullWhere}
														) SELECT
														Dull name,
														COUNT( 1 ) VALUE 
													FROM
														temp1 
													WHERE
														dull IS NOT NULL {kcdullWhere}
												GROUP BY
									Dull";
				RateDic["StockDull"] = Common.ReturnCakeData(DB, DullSQL, 0, "name", "value", "",ui_lan_type, moudle_code);//库存呆滞占比
				#endregion

				#region 各厂区进仓数量
				string InstockSql = $@"WITH BASEDATA AS (
										SELECT
											CASE 
											WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'1PA')>0 AND  SJ.EN IS NULL THEN
												'Fifth Factory'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'2PA')>0 AND  SJ.EN IS NULL THEN
												'Fifth Factory'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'3PA')>0 AND  SJ.EN IS NULL THEN
												'Fifth Factory'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'T01')>0 AND  SJ.EN IS NULL THEN
												'Fifth Factory'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'T02')>0 AND  SJ.EN IS NULL THEN
												'Fifth Factory'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'A')>0 AND  SJ.EN IS NULL THEN
												'Other'
												WHEN instr(SUBSTR(B.DEPARTMENT_CODE, 5, 3),'T06')>0  THEN
												'Other'
											WHEN instr( SUBSTR( B.DEPARTMENT_CODE, 5, 3 ), 'T10' ) > 0 
											 THEN
												'Other'
											ELSE
												SJ.EN
										END FrimName,
											B.DEPARTMENT_CODE,
											SUM( qty ) qty,
											B.udf05
										FROM
											MMS_FINISHEDTRACKIN_LIST ML
											LEFT JOIN BDM_SE_ORDER_MASTER BM ON BM.SE_ID=ML.SE_ID
                                            LEFT JOIN BDM_COUNTRY e on BM.descountry_code = e.c_no and e.l_no='EN'
											LEFT JOIN BDM_SE_ORDER_ITEM BI ON BI.SE_ID=ML.SE_ID
											JOIN BASE005M B ON B.DEP_SAP = ML.FROM_LINE 
											AND B.DEP_SAP
											IS NOT NULL LEFT JOIN BASE001M F ON F.ORG_CODE = B.FACTORY_SAP
											LEFT JOIN SJQDMS_ORGINFO SJ ON SJ.CODE = B.UDF05 
										WHERE
										  1=1 
										  -- AND ORG_CODE = '{plantArea}' -- 2023.05.15
										  AND TO_CHAR(BI.NLT,'yyyy-mm-dd') BETWEEN '{startTime}' and '{endTime}'
										  {InstockWhere}
										GROUP BY
											SJ.EN,
											B.DEPARTMENT_CODE,
											B.udf05)
										SELECT
											nvl( FRIMNAME, 'Other' ) Name,
											SUM( qty ) VALUE 
										FROM
											BASEDATA 
										where 1=1 {InstockWhere1}
										GROUP BY
											nvl( FRIMNAME, 'Other' )
										UNION ALL 
											SELECT
											F.ORG_NAME NAME,
											SUM( qty ) VALUE
										FROM
											MMS_FINISHEDTRACKIN_LIST ML
											LEFT JOIN BDM_SE_ORDER_MASTER BM ON BM.SE_ID=ML.SE_ID
                                            LEFT JOIN BDM_COUNTRY e on BM.descountry_code = e.c_no and e.l_no='EN'
											LEFT JOIN BDM_SE_ORDER_ITEM BI ON BI.SE_ID=ML.SE_ID
											JOIN BASE005M B ON B.DEP_SAP = ML.FROM_LINE 
										  LEFT JOIN BASE001M F ON F.ORG_CODE = B.FACTORY_SAP 
										WHERE
											1=1
											-- FACTORY_SAP != '{plantArea}' -- 2023.05.15
											AND TO_CHAR(BI.NLT,'yyyy-mm-dd') BETWEEN '{startTime}' and '{endTime}' {InstockWhere} {InstockWhere2}
										GROUP BY
											F.ORG_NAME";
				DataTable IndT = DB.GetDataTable(InstockSql);
				RateDic["InStock"] = IndT;//库存呆滞占比
				#endregion

				#endregion
				RetDic["PoInformationList"] = RateDic;//Po信息列表数据

				#region 订单表格信息
				//				string DataSql = $@"WITH AQLStatus AS ( SELECT PO,inspection_state, TO_CHAR(inspection_date,'yyyy-mm-dd') f_inspection_time FROM AQL_CMA_TASK_LIST_M ),
				//											HELF AS (SELECT BATCH_NO, SHELF_NO FROM WMS_STOC_LOCATION WHERE 1=1 {SHILWhere1} GROUP BY BATCH_NO, SHELF_NO ),
				//											OutData AS (
				//												SELECT
				//													M.SE_ID,
				//													nvl( sum( shipping_qty ), 0 ) AS qty 
				//												FROM
				//													bmd_se_shipment_m m
				//													LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
				//												WHERE
				//													1=1 {SHILWhere2}
				//												GROUP BY
				//													M.SE_ID 
				//												),
				//												InData AS (
				//												SELECT
				//													se_id,
				//													nvl( sum( qty ), 0 ) AS qty 
				//												FROM
				//													mms_finishedtrackin_list 
				//												WHERE
				//													1=1 {SHILWhere1}
				//												GROUP BY
				//													se_id 
				//												),
				//												CATEData AS (
				//												SELECT
				//													BD.PROD_NO,
				//													BC.NAME_T CATEGORY,
				//													BS.NAME_T
				//													-- BD.NAME_T 
				//												FROM
				//													bdm_rd_prod BD
				//													LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
				//													LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
				//													AND RULE_NO = 'CATEGORY_NO' 
				//													AND LANGUAGE = '1' 
				//												),
				//												PostDate AS ( SELECT SE_ID,TO_CHAR(MAX( posting_date ),'yyyy-mm-dd')  posting_date FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID ),
				//												DayData AS (
				//												SELECT
				//													t1.SE_ID,
				//													TO_CHAR( t1.startDate, 'yyyy-mm-dd' ) startDate,
				//													NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ) endDate,
				//												 TO_DATE( NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ), 'yyyy-mm-dd' )-t1.startDate  days 
				//												FROM
				//													( 
				//														SELECT SE_ID, min( insert_date ) AS startDate 
				//														FROM mms_finishedtrackin_list 
				//	WHERE 1=1 {SHILWhere1}  GROUP BY SE_ID 
				//) t1
				//													LEFT JOIN (
				//													SELECT
				//														m.SE_ID,
				//														max( posting_date ) AS endDate 
				//													FROM
				//														bmd_se_shipment_m m
				//														LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
				//													WHERE
				//														1=1 {SHILWhere2} 
				//													GROUP BY
				//														m.SE_ID 
				//													) t2 ON t1.SE_ID = t2.SE_ID 
				//												),
				//												Top5Data AS (
				//												SELECT
				//													SHOES_CODE,
				//													QD.qa_risk_desc,
				//													COUNT( QD.qa_risk_desc ) num,
				//													ROW_NUMBER ( ) OVER ( PARTITION BY SHOES_CODE ORDER BY COUNT( QD.qa_risk_desc ) DESC ) RANKING 
				//												FROM
				//													qcm_dqa_mag_d QD 
				//												WHERE
				//													QD.isdelete = '0' 
				//													{top5where}
				//												GROUP BY
				//													SHOES_CODE,
				//													QD.QA_RISK_DESC 
				//												ORDER BY
				//													SHOES_CODE 
				//												),
				//												tabledata as (SELECT
				//												TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) NLT,--PODD
				//												BM.MER_PO,--PO
				//												BI.SE_QTY,--PO双数
				//												BM.descountry_name SHIPCOUNTRY_NAME,--出货国家
				//												CATEData.CATEGORY,--category
				//												( CATEData.NAME_T ) SHOE_NO,--鞋型
				//												BI.PROD_NO ART,--ART
				//												wl.SHELF_NO AS SITE,--储位
				//												AQLStatus.inspection_state INSPECTION_STATE,--验货状态
				//												AQLStatus.F_INSPECTION_TIME,--验货日期
				//											--NVL( OutData.qty, 0 ) AS OUT_QTY,--出货数量
				//											--NVL( InData.qty, 0 ) AS IN_QTY,--入库数量
				//											CASE
				//													WHEN NVL( OutData.qty, 0 ) <= 0 THEN
				//													'' 
				//													WHEN NVL( OutData.qty, 0 ) >= BI.SE_QTY THEN
				//													'已出货' 
				//													WHEN NVL( InData.qty, 0 ) < BI.SE_QTY 
				//													AND NVL( OutData.qty, 0 ) = 0 THEN
				//														'生产中' 
				//														WHEN NVL( InData.qty, 0 ) >= BI.SE_QTY 
				//														AND BI.SE_QTY > NVL( OutData.qty, 0 ) 
				//														AND NVL( OutData.qty, 0 ) > 0 THEN
				//															'部分出货' 
				//															WHEN NVL( InData.qty, 0 ) >= BI.SE_QTY 
				//															AND NVL( OutData.qty, 0 ) = 0 THEN
				//																'备库存' 
				//																WHEN NVL( InData.qty, 0 ) < BI.SE_QTY 
				//																AND NVL( OutData.qty, 0 ) < BI.SE_QTY THEN
				//																	'生产中且部分出货' 
				//																	END AS OutState,--出货状态
				//																PostDate.POSTING_DATE POSTING_DATE,--实际出货日期
				//															CASE

				//																	WHEN DayData.DAYS < 30 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
				//																	'一个月以内呆滞品' 
				//																	WHEN DayData.DAYS > 30 
				//																	AND DayData.DAYS < 60 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
				//																		'一个月以上呆滞品' 
				//																		WHEN DayData.DAYS > 60 
				//																		AND DayData.DAYS < 90 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
				//																			'两个月以上呆滞品' 
				//																			WHEN DayData.DAYS > 90 
				//																			AND DayData.DAYS < 120 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
				//																				'三个月以上呆滞品' 
				//																				WHEN DayData.DAYS > 120 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
				//																				'四个月以上呆滞品' 
				//								END AS Dull,
				//								t1.qa_risk_desc AS RISK_1,
				//								t2.qa_risk_desc AS RISK_2,
				//								t3.qa_risk_desc AS RISK_3,
				//								t4.qa_risk_desc AS RISK_4,
				//								t5.qa_risk_desc AS RISK_5 
				//							FROM
				//								BDM_SE_ORDER_MASTER BM
				//								JOIN BDM_SE_ORDER_ITEM BI ON BM.SE_ID = BI.SE_ID
				//								JOIN OutData ON OutData.SE_ID = BI.SE_ID
				//								JOIN InData ON InData.SE_ID = BI.SE_ID
				//								JOIN CATEData ON CATEData.PROD_NO = BI.PROD_NO
				//								JOIN AQLStatus ON AQLStatus.PO =  Bm.mer_po
				//								JOIN PostDate ON PostDate.SE_ID = BI.SE_ID
				//								JOIN DayData ON DayData.SE_ID = BI.SE_ID
				//								JOIN HELF wl ON wl.BATCH_NO = BI.SE_ID 
				//								JOIN MMS_WAREHOUSE_SHELF_MANAGE sh ON wl.SHELF_NO = sh.LOCATION_CODE
				//								JOIN BASE005M m5 ON sh.ORG_ID = m5.FACTORY_SAP
				//								LEFT JOIN Top5Data t1 ON t1.SHOES_CODE = BI.SHOE_NO 
				//								AND t1.RANKING = '1'
				//								LEFT JOIN Top5Data t2 ON t2.SHOES_CODE = BI.SHOE_NO 
				//								AND t2.RANKING = '2'
				//								LEFT JOIN Top5Data t3 ON t3.SHOES_CODE = BI.SHOE_NO 
				//								AND t3.RANKING = '3'
				//								LEFT JOIN Top5Data t4 ON t4.SHOES_CODE = BI.SHOE_NO 
				//								AND t4.RANKING = '4'
				//								LEFT JOIN Top5Data t5 ON t5.SHOES_CODE = BI.SHOE_NO 
				//								AND t5.RANKING = '5' 
				//							WHERE
				//								1 = 1 
				//							AND TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
				//							AND '{endTime}' {TableWhere}
				//) SELECT * FROM tabledata where 1=1 {kcdullWhere}
				//GROUP BY NLT,MER_PO,SE_QTY,SHIPCOUNTRY_NAME,CATEGORY,SHOE_NO,ART,SITE,INSPECTION_STATE,F_INSPECTION_TIME,OUTSTATE,POSTING_DATE,DULL,RISK_1,RISK_2,RISK_3,RISK_4,RISK_5";
				#endregion
				#region 订单表格信息 //updated 01/09/2023//
				string DataSql = $@"WITH AQLStatus AS ( SELECT PO,inspection_state, TO_CHAR(inspection_date,'yyyy-mm-dd') f_inspection_time FROM AQL_CMA_TASK_LIST_M ),
											HELF AS (SELECT BATCH_NO, SHELF_NO FROM WMS_STOC_LOCATION WHERE 1=1 {SHILWhere1} GROUP BY BATCH_NO, SHELF_NO ),
											OutData AS (
												SELECT
													M.SE_ID,
													nvl( sum( shipping_qty ), 0 ) AS qty 
												FROM
													bmd_se_shipment_m m
													LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
												WHERE
													1=1 {SHILWhere2}
												GROUP BY
													M.SE_ID 
												),
												InData AS (
												SELECT
													se_id,
													nvl( sum( qty ), 0 ) AS qty 
												FROM
													mms_finishedtrackin_list 
												WHERE
													1=1 {SHILWhere1}
												GROUP BY
													se_id 
												),
												CATEData AS (
												SELECT
													BD.PROD_NO,
													BC.NAME_T CATEGORY,
													BS.NAME_T
													-- BD.NAME_T 
												FROM
													bdm_rd_prod BD
													LEFT JOIN BDM_RD_STYLE BS ON BS.SHOE_NO = BD.SHOE_NO
													LEFT JOIN BDM_CD_CODE BC ON BC.CODE_NO = BS.STYLE_SEQ 
													AND RULE_NO = 'CATEGORY_NO' 
													AND LANGUAGE = '1' 
												),
												PostDate AS ( SELECT SE_ID,TO_CHAR(MAX( posting_date ),'yyyy-mm-dd')  posting_date FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID ),
												DayData AS (
												SELECT
													t1.SE_ID,
													TO_CHAR( t1.startDate, 'yyyy-mm-dd' ) startDate,
													NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ) endDate,
												 TO_DATE( NVL( TO_CHAR( t2.endDate, 'yyyy-mm-dd' ), TO_CHAR( SYSDATE, 'yyyy-mm-dd' ) ), 'yyyy-mm-dd' )-t1.startDate  days 
												FROM
													( 
														SELECT SE_ID, min( insert_date ) AS startDate 
														FROM mms_finishedtrackin_list 
	WHERE 1=1 {SHILWhere1}  GROUP BY SE_ID 
) t1
													LEFT JOIN (
													SELECT
														m.SE_ID,
														max( posting_date ) AS endDate 
													FROM
														bmd_se_shipment_m m
														LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
													WHERE
														1=1 {SHILWhere2} 
													GROUP BY
														m.SE_ID 
													) t2 ON t1.SE_ID = t2.SE_ID 
												),
												Top5Data AS (
												SELECT
													SHOES_CODE,
													QD.qa_risk_desc,
													COUNT( QD.qa_risk_desc ) num,
													ROW_NUMBER ( ) OVER ( PARTITION BY SHOES_CODE ORDER BY COUNT( QD.qa_risk_desc ) DESC ) RANKING 
												FROM
													qcm_dqa_mag_d QD 
												WHERE
													QD.isdelete = '0' 
													{top5where}
												GROUP BY
													SHOES_CODE,
													QD.QA_RISK_DESC 
												ORDER BY
													SHOES_CODE 
												),
												tabledata as (SELECT
												TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) NLT,--PODD
												BM.MER_PO,--PO
												BI.SE_QTY,--PO双数
												e.c_name as SHIPCOUNTRY_NAME,--出货国家
												CATEData.CATEGORY,--category
												( CATEData.NAME_T ) SHOE_NO,--鞋型
												BI.PROD_NO ART,--ART
												wl.SHELF_NO AS SITE,--储位
												AQLStatus.inspection_state INSPECTION_STATE,--验货状态
												AQLStatus.F_INSPECTION_TIME,--验货日期
											--NVL( OutData.qty, 0 ) AS OUT_QTY,--出货数量
											--NVL( InData.qty, 0 ) AS IN_QTY,--入库数量
											CASE
													WHEN NVL( OutData.qty, 0 ) <= 0 THEN
													'' 
													WHEN NVL( OutData.qty, 0 ) >= BI.SE_QTY THEN
													'Shipped' 
													WHEN NVL( InData.qty, 0 ) < BI.SE_QTY 
													AND NVL( OutData.qty, 0 ) = 0 THEN
														'In_Production' 
														WHEN NVL( InData.qty, 0 ) >= BI.SE_QTY 
														AND BI.SE_QTY > NVL( OutData.qty, 0 ) 
														AND NVL( OutData.qty, 0 ) > 0 THEN
															'Partial_Shipment' 
															WHEN NVL( InData.qty, 0 ) >= BI.SE_QTY 
															AND NVL( OutData.qty, 0 ) = 0 THEN
																'Keep_Inventory' 
																WHEN NVL( InData.qty, 0 ) < BI.SE_QTY 
																AND NVL( OutData.qty, 0 ) < BI.SE_QTY THEN
																	'In_Production_and_Partially_Shipped' 
																	END AS OutState,--出货状态
																PostDate.POSTING_DATE POSTING_DATE,--实际出货日期
															CASE
						
																	WHEN DayData.DAYS < 30 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
																	'Sluggish_goods_within_one_month' 
																	WHEN DayData.DAYS > 30 
																	AND DayData.DAYS < 60 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
																		'Over_one_month_sluggish_goods' 
																		WHEN DayData.DAYS > 60 
																		AND DayData.DAYS < 90 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
																			'Over_two_months_sluggish_goods' 
																			WHEN DayData.DAYS > 90 
																			AND DayData.DAYS < 120 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
																				'Sluggish_goods_more_than_three_months_old' 
																				WHEN DayData.DAYS > 120 AND TO_CHAR(bi.NLT,'yyyy-mm')!=TO_CHAR(SYSDATE,'yyyy-mm') THEN
																				'Over_four_months_sluggish_goods' 
								END AS Dull,
								t1.qa_risk_desc AS RISK_1,
								t2.qa_risk_desc AS RISK_2,
								t3.qa_risk_desc AS RISK_3,
								t4.qa_risk_desc AS RISK_4,
								t5.qa_risk_desc AS RISK_5 
							FROM
								BDM_SE_ORDER_MASTER BM
                                JOIN BDM_COUNTRY e on BM.descountry_code = e.c_no and e.l_no='EN'
								JOIN BDM_SE_ORDER_ITEM BI ON BM.SE_ID = BI.SE_ID
								JOIN OutData ON OutData.SE_ID = BI.SE_ID
								JOIN InData ON InData.SE_ID = BI.SE_ID
								JOIN CATEData ON CATEData.PROD_NO = BI.PROD_NO
								JOIN AQLStatus ON AQLStatus.PO =  Bm.mer_po
								JOIN PostDate ON PostDate.SE_ID = BI.SE_ID
								JOIN DayData ON DayData.SE_ID = BI.SE_ID
								JOIN HELF wl ON wl.BATCH_NO = BI.SE_ID 
								JOIN MMS_WAREHOUSE_SHELF_MANAGE sh ON wl.SHELF_NO = sh.LOCATION_CODE
								JOIN BASE005M m5 ON sh.ORG_ID = m5.FACTORY_SAP
								LEFT JOIN Top5Data t1 ON t1.SHOES_CODE = BI.SHOE_NO 
								AND t1.RANKING = '1'
								LEFT JOIN Top5Data t2 ON t2.SHOES_CODE = BI.SHOE_NO 
								AND t2.RANKING = '2'
								LEFT JOIN Top5Data t3 ON t3.SHOES_CODE = BI.SHOE_NO 
								AND t3.RANKING = '3'
								LEFT JOIN Top5Data t4 ON t4.SHOES_CODE = BI.SHOE_NO 
								AND t4.RANKING = '4'
								LEFT JOIN Top5Data t5 ON t5.SHOES_CODE = BI.SHOE_NO 
								AND t5.RANKING = '5' 
							WHERE
								1 = 1 
							AND TO_CHAR( BI.NLT, 'yyyy-mm-dd' ) BETWEEN '{startTime}' 
							AND '{endTime}' {TableWhere}
) SELECT * FROM tabledata where 1=1 {kcdullWhere}
GROUP BY NLT,MER_PO,SE_QTY,SHIPCOUNTRY_NAME,CATEGORY,SHOE_NO,ART,SITE,INSPECTION_STATE,F_INSPECTION_TIME,OUTSTATE,POSTING_DATE,DULL,RISK_1,RISK_2,RISK_3,RISK_4,RISK_5";
				DataTable TableDt = DB.GetDataTable(DataSql);

                foreach (DataRow item in TableDt.Rows)
                {
                    switch (item["INSPECTION_STATE"].ToString())
                    {
						case "0":
							//item["INSPECTION_STATE"] = "未验货";
							item["INSPECTION_STATE"] = "Not inspected";
							break;
						case "1":
							//item["INSPECTION_STATE"] = "已验货";
							item["INSPECTION_STATE"] = "Inspected";
							break;
                    }
                    
                }
                
                RetDic["OrderList"] = TableDt;
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
