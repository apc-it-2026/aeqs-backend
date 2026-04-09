using Art.Bom;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJeMES_IQC
{
    public class IQC_APP_Compliance
    {
        /// <summary>
        /// 查询-APP2合规-主页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// 


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_ComplianceMain(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//art
                string sql = string.Empty;

                sql = $@"SELECT
                        p.PROD_NO
                        FROM
                        BDM_RD_PROD p
                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
                        where p.PROD_NO = '{PROD_NO}'";

                string prod_no = DB.GetString(sql);
                if (string.IsNullOrWhiteSpace(prod_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "This ART has no data!";
                    return ret;
                }

                sql = $@"SELECT
                        p.PROD_NO,
                        p.name_t as prod_name,
                        s.NAME_T as shoe_name,
                        '' as Astate,
                        '' as Adate
                        FROM
                        BDM_RD_PROD p
                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
                        where p.PROD_NO = '{PROD_NO}'";
                DataTable dthead = DB.GetDataTable(sql);

                sql = $@"select 
                        '' as report_no,
                        '' as POSITION,
                        '' as POSITION_CN,
                        '' as POSITION_EN,
                        '' as ITEM_NO,
                        '' as AD_ITEM_NO,
                        '' as NAME_CN,
                        '' as COLOR_NAME,
                        '' as COLOR_NO,
                        '' as VEND_NO,
                        '' as VEND_NAME,
                        '' as gys,
                        '' as Astate,
                        '' as Adate 
                        from bdm_vendor_report_m 
                        where rownum=0
                            ";

                DataTable dt = DB.GetDataTable(sql);
                dt.Rows.Clear();
                DateTime currDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                DT_PP113_REQI_REQUESTBODY d = new DT_PP113_REQI_REQUESTBODY();
                Art.Bom.DT_PP113_RSP apiRes = new Art.Bom.DT_PP113_RSP();

                d.art_no = prod_no;
                d.type = "";
                d.langu = "EN";
                d.stage = "PRS";
                d.item_no = "";
                d.werks = "5003";
                apiRes = SJ_SAPAPI.SapApiHelper.GetArtBom(d, ReqObj);
                if (!Convert.ToBoolean(apiRes.IS_SUCCESS))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = apiRes.ERR_MSG;
                    return ret;
                }
                //art的料号循环整理状态
                List<string> AstateList = new List<string>();
                List<DateTime> eDateList = new List<DateTime>();
                List<string> item_noList = new List<string>();
                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
                {
                    item_noList.Add(apiRes.RES.DATAS[i].ITEM_NO);
                }
                //T2厂商数据
                DataTable dtT2 = DB.GetDataTable($@"
SELECT
	item_no,
	report_no,
	start_date
FROM
	bdm_vendor_report_m 
WHERE report_type = '1' AND item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))}) ORDER BY start_date DESC ");
                //料品资料数据
                DataTable dtItem = DB.GetDataTable($@"
SELECT
	b.PART_NO,
	r.ITEM_NO,
	r.NAME_T,
	r.COLOR_NAME,
	r.COLOR_NO,
	r.VEND_NO,
	M .SUPPLIERS_NAME,
	r.VEND_NO AS gys
FROM
	bdm_rd_item r
LEFT JOIN bdm_rd_bom_item b ON r.item_no = b.PROD_NO
LEFT JOIN base003m M ON r.VEND_NO = M .SUPPLIERS_CODE  
WHERE r.item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))})");
                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
                {
                    DataRow[] drT2 = dtT2.Select($@"item_no='{apiRes.RES.DATAS[i].ITEM_NO}'");

                    DataRow[] drItem = dtItem.Select($@"ITEM_NO='{apiRes.RES.DATAS[i].ITEM_NO}'");

                    dt.Rows.Add(dt.NewRow());
                    int idx = dt.Rows.Count - 1;
                    //string Astate = "正常";
                    string Astate = "Normal";
                    if (drT2.Length <= 0)
                    {
                        Astate = "Expired";
                    }
                    else
                    {
                        var eDate = new DateTime();
                        if (!string.IsNullOrWhiteSpace(drT2[0]["start_date"].ToString()))
                        {
                            //1.起始日期
                            var sDate = Convert.ToDateTime(Convert.ToDateTime(drT2[0]["start_date"].ToString()).ToString("yyyy-MM-dd"));
                            //2.临期日期
                            var wDate = sDate.AddMonths(11);
                            //3.到期日期 A01到期日期等于起始日期加一年
                            eDate = sDate.AddYears(1);
                            eDateList.Add(eDate);
                            if (currDate > eDate)
                            {//已失效
                                Astate = "Expired";
                                //break;
                            }
                            else if (currDate >= wDate)
                            {//临期
                                Astate = "Arrival";
                            }
                        }
                        else
                        {
                            Astate = "Expired";
                        }

                        dt.Rows[idx]["report_no"] = drT2[0]["report_no"];
                        //dt.Rows[idx]["gys"] = "";
                        dt.Rows[idx]["Astate"] = Astate;
                        dt.Rows[idx]["Adate"] = eDate;
                    }
                    dt.Rows[idx]["POSITION"] = apiRes.RES.DATAS[i].POSITION;
                    dt.Rows[idx]["POSITION_CN"] = apiRes.RES.DATAS[i].POSITION_CN;
                    dt.Rows[idx]["POSITION_EN"] = apiRes.RES.DATAS[i].POSITION_EN;
                    dt.Rows[idx]["ITEM_NO"] = apiRes.RES.DATAS[i].ITEM_NO;
                    string AD_ITEM_NO = DB.GetString($@"select AD_ITEM_NO from bdm_rd_item where ITEM_NO='{apiRes.RES.DATAS[i].ITEM_NO}'");
                    dt.Rows[idx]["AD_ITEM_NO"] = AD_ITEM_NO;
                    dt.Rows[idx]["NAME_CN"] = apiRes.RES.DATAS[i].NAME_CN;
                    if (drItem.Length > 0)
                    {
                        //dt.Rows[idx]["COLOR_NAME"] = drItem[0]["COLOR_NAME"];
                        //dt.Rows[idx]["COLOR_NO"] = drItem[0]["COLOR_NO"];
                        //dt.Rows[idx]["VEND_NO"] = drItem[0]["VEND_NO"];
                        //dt.Rows[idx]["VEND_NAME"] = drItem[0]["SUPPLIERS_NAME"];
                        dt.Rows[idx]["gys"] = drItem[0]["SUPPLIERS_NAME"];
                    }
                    AstateList.Add(Astate);
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                int rowCount = dt.Rows.Count;
                if (AstateList.Contains("Expired"))
                    dthead.Rows[0]["Astate"] = "Expired";
                else if (AstateList.Contains("Arrival"))
                    dthead.Rows[0]["Astate"] = "Arrival";
                else if (AstateList.Contains("Normal"))
                    dthead.Rows[0]["Astate"] = "Normal";
                else
                    dthead.Rows[0]["Astate"] = "Expired";

                if (eDateList.Count > 0)
                {
                    dthead.Rows[0]["Adate"] = eDateList.Max().ToString("yyyy-MM-dd");
                }
                //dt = GetPagedTable(dt, int.Parse(pageIndex), int.Parse(pageSize));
                dic.Add("DataHead", dthead);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
//        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_ComplianceMain(object OBJ)
//        {
//            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
//            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
//            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
//            try
//            {
//                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
//                string Data = ReqObj.Data.ToString();
//                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
//                //转译
//                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
//                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
//                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//art
//                string sql = string.Empty;

//                sql = $@"SELECT
//                        p.PROD_NO
//                        FROM
//                        BDM_RD_PROD p
//                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
//                        where p.PROD_NO = '{PROD_NO}'";

//                string prod_no = DB.GetString(sql);
//                if (string.IsNullOrWhiteSpace(prod_no))
//                {
//                    ret.IsSuccess = false;
//                    ret.ErrMsg = "This ART has no data!";
//                    return ret;
//                }

//                sql = $@"SELECT
//                        p.PROD_NO,
//                        p.name_t as prod_name,
//                        s.NAME_T as shoe_name,
//                        '' as Astate,
//                        '' as Adate
//                        FROM
//                        BDM_RD_PROD p
//                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
//                        where p.PROD_NO = '{PROD_NO}'";
//                DataTable dthead = DB.GetDataTable(sql);

//                sql = $@"select 
//                        '' as report_no,
//                        '' as POSITION,
//                        '' as POSITION_CN,
//                        '' as POSITION_EN,
//                        '' as ITEM_NO,
//                        '' as AD_ITEM_NO,
//                        '' as NAME_CN,
//                        '' as COLOR_NAME,
//                        '' as COLOR_NO,
//                        '' as VEND_NO,
//                        '' as VEND_NAME,
//                        '' as gys,
//                        '' as Astate,
//                        '' as Adate 
//                        from bdm_vendor_report_m 
//                        where rownum=0
//                            ";

//                DataTable dt = DB.GetDataTable(sql);
//                dt.Rows.Clear();
//                DateTime currDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
//                DT_PP113_REQI_REQUESTBODY d = new DT_PP113_REQI_REQUESTBODY();
//                Art.Bom.DT_PP113_RSP apiRes = new Art.Bom.DT_PP113_RSP();
//                d.art_no = prod_no;
//                d.type = "";
//                d.langu = "EN";
//                d.stage = "PRS";
//                d.item_no = "";
//                d.werks = "5003"; /*APE - 1003; APH - 4003; APC - 5003*/
//                apiRes = SJ_SAPAPI.SapApiHelper.GetArtBom(d, ReqObj);
//                if (!Convert.ToBoolean(apiRes.IS_SUCCESS))
//                {
//                    ret.IsSuccess = false;
//                    ret.ErrMsg = apiRes.ERR_MSG;
//                    return ret;
//                }
//                //art的料号循环整理状态
//                List<string> AstateList = new List<string>();
//                List<DateTime> eDateList = new List<DateTime>();
//                List<string> item_noList = new List<string>();
//                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
//                {
//                    item_noList.Add(apiRes.RES.DATAS[i].ITEM_NO);
//                }
//                //T2厂商数据
//                //                DataTable dtT2 = DB.GetDataTable($@"

//                //SELECT
//                //	item_no,
//                //	report_no,
//                //	start_date
//                //FROM
//                //	bdm_vendor_report_m 
//                //WHERE report_type = '1' AND item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))}) ORDER BY start_date DESC ");

                
// sql = $@"
//SELECT
//	item_no,
//	report_no,
//	start_date
//FROM
//	bdm_vendor_report_m 
//WHERE report_type = '1' AND item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))}) ORDER BY start_date DESC ";
//                DataTable dtT2 = DB.GetDataTable(sql);
//                //料品资料数据
//                DataTable dtItem = DB.GetDataTable($@"
//SELECT
//	b.PART_NO,
//	r.ITEM_NO,
//	r.NAME_T,
//	r.COLOR_NAME,
//	r.COLOR_NO,
//	r.VEND_NO,
//	M .SUPPLIERS_NAME,
//	r.VEND_NO AS gys
//FROM
//	bdm_rd_item r
//LEFT JOIN bdm_rd_bom_item b ON r.item_no = b.PROD_NO
//LEFT JOIN base003m M ON r.VEND_NO = M .SUPPLIERS_CODE  
//WHERE r.item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))})");
//                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
//                {
//                    DataRow[] drT2 = dtT2.Select($@"item_no='{apiRes.RES.DATAS[i].ITEM_NO}'");

//                    DataRow[] drItem = dtItem.Select($@"ITEM_NO='{apiRes.RES.DATAS[i].ITEM_NO}'");

//                    dt.Rows.Add(dt.NewRow());
//                    int idx = dt.Rows.Count - 1;
//                    string Astate = "Normal";
//                    if (drT2.Length <= 0)
//                    {
//                        //Astate = "已失效";
//                        Astate = "Expired";
//                    }
//                    else
//                    {
//                        var eDate = new DateTime();
//                        if (!string.IsNullOrWhiteSpace(drT2[0]["start_date"].ToString()))
//                        {
//                            //1.起始日期
//                            var sDate = Convert.ToDateTime(Convert.ToDateTime(drT2[0]["start_date"].ToString()).ToString("yyyy-MM-dd"));
//                            //2.临期日期
//                            var wDate = sDate.AddMonths(11);
//                            //3.到期日期 A01到期日期等于起始日期加一年
//                            eDate = sDate.AddYears(1);
//                            eDateList.Add(eDate);
//                            if (currDate > eDate)
//                            {//已失效
//                                //Astate = "已失效";
//                                Astate = "Expired";
//                                //break;
//                            }
//                            else if (currDate >= wDate)
//                            {//临期
//                                //Astate = "临期";
//                                Astate = "Advent";
//                            }
//                        }
//                        else
//                        {
//                            //Astate = "已失效";
//                            Astate = "Expired";
//                        }

//                        dt.Rows[idx]["report_no"] = drT2[0]["report_no"];
//                        //dt.Rows[idx]["gys"] = "";
//                        dt.Rows[idx]["Astate"] = Astate;
//                        dt.Rows[idx]["Adate"] = eDate;
//                    }
//                    dt.Rows[idx]["POSITION"] = apiRes.RES.DATAS[i].POSITION;
//                    dt.Rows[idx]["POSITION_CN"] = apiRes.RES.DATAS[i].POSITION_CN;
//                    dt.Rows[idx]["POSITION_EN"] = apiRes.RES.DATAS[i].POSITION_EN;
//                    dt.Rows[idx]["ITEM_NO"] = apiRes.RES.DATAS[i].ITEM_NO;
//                    string AD_ITEM_NO = DB.GetString($@"select AD_ITEM_NO from bdm_rd_item where ITEM_NO='{apiRes.RES.DATAS[i].ITEM_NO}'");
//                    dt.Rows[idx]["AD_ITEM_NO"] = AD_ITEM_NO;
//                    dt.Rows[idx]["NAME_CN"] = apiRes.RES.DATAS[i].NAME_CN;
//                    if (drItem.Length > 0)
//                    {
//                        //dt.Rows[idx]["COLOR_NAME"] = drItem[0]["COLOR_NAME"];
//                        //dt.Rows[idx]["COLOR_NO"] = drItem[0]["COLOR_NO"];
//                        //dt.Rows[idx]["VEND_NO"] = drItem[0]["VEND_NO"];
//                        //dt.Rows[idx]["VEND_NAME"] = drItem[0]["SUPPLIERS_NAME"];
//                        dt.Rows[idx]["gys"] = drItem[0]["SUPPLIERS_NAME"];
//                    }
//                    AstateList.Add(Astate);
//                }

//                Dictionary<string, object> dic = new Dictionary<string, object>();
//                int rowCount = dt.Rows.Count;
//                if (AstateList.Contains("Expired"))
//                    //dthead.Rows[0]["Astate"] = "已失效";
//                    dthead.Rows[0]["Astate"] = "Expired";
//                else if (AstateList.Contains("Advent"))
//                    //dthead.Rows[0]["Astate"] = "临期";
//                    dthead.Rows[0]["Astate"] = "Advent";
//                else if (AstateList.Contains("Normal"))
//                    //dthead.Rows[0]["Astate"] = "正常";
//                    dthead.Rows[0]["Astate"] = "Normal";
//                else
//                    //dthead.Rows[0]["Astate"] = "已失效";
//                    dthead.Rows[0]["Astate"] = "Expired";

//                if (eDateList.Count > 0)
//                {
//                    dthead.Rows[0]["Adate"] = eDateList.Max().ToString("yyyy-MM-dd");
//                }
//                //dt = GetPagedTable(dt, int.Parse(pageIndex), int.Parse(pageSize));
//                dic.Add("DataHead", dthead);
//                dic.Add("Data", dt);
//                dic.Add("rowCount", rowCount);
//                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
//                ret.IsSuccess = true;

//            }
//            catch (Exception ex)
//            {
//                ret.IsSuccess = false;
//                ret.ErrMsg = ex.Message;
//            }

//            return ret;
//        }

        /// <summary>
        /// 查询-APP2合规-主页-查看报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_ComplianceMain_bg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string report_no = jarr.ContainsKey("report_no") ? jarr["report_no"].ToString() : "";//查询条件 报告编号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"														
                                SELECT
	                                t.file_url,
                                CASE
		                                v.report_type 
		                                WHEN '0' THEN
		                                'T2测试报告' 
		                                WHEN '1' THEN
		                                'T2 A-01报告' 
	                                END file_name,
	                                'bdm_vendor_report_m' AS tablename,
	                                v.id,
                                    t.guid
                                FROM
	                                bdm_vendor_report_m v
	                                INNER JOIN BDM_UPLOAD_FILE_ITEM t ON t.guid = v.file_id 
                                WHERE item_no='{item_no}' and report_no='{report_no}'";

                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查询-APP2合规-主页-下载列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_ComplianceDownloadLists(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//art
                string sql = string.Empty;

                sql = $@"SELECT
                        p.PROD_NO
                        FROM
                        BDM_RD_PROD p
                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
                        where p.PROD_NO = '{PROD_NO}'";

                string prod_no = DB.GetString(sql);
                if (string.IsNullOrWhiteSpace(prod_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "This ART has no data!";
                    return ret;
                }

                sql = $@"SELECT
                        p.PROD_NO,
                        s.NAME_T as shoe_name,
                        '' as Astate,
                        '' as Adate
                        FROM
                        BDM_RD_PROD p
                        LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
                        where p.PROD_NO = '{PROD_NO}'";
                DataTable dthead = DB.GetDataTable(sql);

                sql = $@"select 
                        '' as report_no,
                        '' as POSITION,
                        '' as ITEM_NO,
                        '' as ITEM_ID,
                        '' as NAME_CN,
                        '' as COLOR_NAME,
                        '' as COLOR_NO,
                        '' as VEND_NO,
                        '' as VEND_NAME,
                        '' as gys,
                        '' as Astate,
                        '' as Adate 
                        from bdm_vendor_report_m
                            ";

                DataTable dt = DB.GetDataTable(sql);
                dt.Rows.Clear();
                DateTime currDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                DT_PP113_REQI_REQUESTBODY d = new DT_PP113_REQI_REQUESTBODY();
                Art.Bom.DT_PP113_RSP apiRes = new Art.Bom.DT_PP113_RSP();
                //d.art_no = prod_no;
                //d.type = "1";
                //d.stage = "PRS";
                //d.item_no = "";

                d.art_no = prod_no;
                d.werks = "5003";
                d.type = "";
                d.stage = "PRS";
                d.item_no = "";
                d.langu = "EN";


                apiRes = SJ_SAPAPI.SapApiHelper.GetArtBom(d, ReqObj);
                if (!Convert.ToBoolean(apiRes.IS_SUCCESS))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = apiRes.ERR_MSG;
                    return ret;
                }
                //art的料号循环整理状态
                List<string> AstateList = new List<string>();
                List<DateTime> eDateList = new List<DateTime>();
                List<string> item_noList = new List<string>();
                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
                {
                    item_noList.Add(apiRes.RES.DATAS[i].ITEM_NO);
                }
                //T2厂商数据
                DataTable dtT2 = DB.GetDataTable($@"
                    SELECT *  FROM ( SELECT item_no,report_no,start_date FROM bdm_vendor_report_m WHERE report_type = '1' AND 
                    item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))}) ORDER BY start_date DESC ) WHERE ROWNUM =1");
                //料品资料数据
                DataTable dtItem = DB.GetDataTable($@"select b.PART_NO,r.ITEM_NO,r.NAME_T,r.COLOR_NAME,r.COLOR_NO,r.VEND_NO,m.SUPPLIERS_NAME,'' as gys
                from bdm_rd_item r inner join bdm_rd_bom_item b on r.item_no=b.PROD_NO inner join base003m m on r.VEND_NO=m.SUPPLIERS_CODE 
                where r.item_no in ({string.Join(",", item_noList.Select(x => $@"'{x}'"))})");
                for (int i = 0; i < apiRes.RES.DATAS.Length; i++)
                {
                    DataRow[] drT2 = dtT2.Select($@"item_no='{apiRes.RES.DATAS[i].ITEM_NO}'");

                    DataRow[] drItem = dtItem.Select($@"ITEM_NO='{apiRes.RES.DATAS[i].ITEM_NO}'");

                    dt.Rows.Add(dt.NewRow());
                    int idx = dt.Rows.Count - 1;
                    //string Astate = "正常";
                    string Astate = "Normal";
                    if (drT2.Length <= 0)
                    {
                        //Astate = "已失效";
                        Astate = "Expired";
                    }
                    else
                    {
                        var eDate = new DateTime();
                        if (!string.IsNullOrWhiteSpace(drT2[0]["start_date"].ToString()))
                        {
                            //1.起始日期
                            var sDate = Convert.ToDateTime(Convert.ToDateTime(drT2[0]["start_date"].ToString()).ToString("yyyy-MM-dd"));
                            //2.临期日期
                            var wDate = sDate.AddMonths(11);
                            //3.到期日期 A01到期日期等于起始日期加一年
                            eDate = sDate.AddYears(1);
                            eDateList.Add(eDate);
                            if (currDate > eDate)
                            {//已失效
                                //Astate = "已失效";
                                Astate = "Expired";
                                break;
                            }
                            else if (currDate >= wDate)
                            {//临期
                                //Astate = "临期";
                                Astate = "Advent";//Advent
                            }
                        }
                        else
                        {
                            //Astate = "已失效";
                            Astate = "Expired";
                        }

                        dt.Rows[idx]["report_no"] = drT2[0]["report_no"];
                        //dt.Rows[idx]["gys"] = "";
                        dt.Rows[idx]["Astate"] = Astate;
                        dt.Rows[idx]["Adate"] = eDate;
                    }
                    dt.Rows[idx]["POSITION"] = apiRes.RES.DATAS[i].POSITION;
                    dt.Rows[idx]["ITEM_NO"] = apiRes.RES.DATAS[i].ITEM_NO;
                    dt.Rows[idx]["ITEM_ID"] = apiRes.RES.DATAS[i].ITEM_NO;
                    dt.Rows[idx]["NAME_CN"] = apiRes.RES.DATAS[i].NAME_CN;
                    if (drItem.Length > 0)
                    {
                        dt.Rows[idx]["COLOR_NAME"] = drItem[0]["COLOR_NAME"];
                        dt.Rows[idx]["COLOR_NO"] = drItem[0]["COLOR_NO"];
                        dt.Rows[idx]["VEND_NO"] = drItem[0]["VEND_NO"];
                        dt.Rows[idx]["VEND_NAME"] = drItem[0]["SUPPLIERS_NAME"];
                    }
                    AstateList.Add(Astate);
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                int rowCount = dt.Rows.Count;
                if (AstateList.Contains("Expired"))
                    //dthead.Rows[0]["Astate"] = "已失效";
                    dthead.Rows[0]["Astate"] = "Expired";
                else if (AstateList.Contains("Advent"))
                    //dthead.Rows[0]["Astate"] = "临期";
                    dthead.Rows[0]["Astate"] = "Advent";
                else if (AstateList.Contains("Normal"))
                    //dthead.Rows[0]["Astate"] = "正常";
                    dthead.Rows[0]["Astate"] = "Normal";
                else
                    //dthead.Rows[0]["Astate"] = "已失效";
                    dthead.Rows[0]["Astate"] = "Expired";

                if (eDateList.Count > 0)
                {
                    dthead.Rows[0]["Adate"] = eDateList.Max().ToString("yyyy-MM-dd");
                }
                //dt = GetPagedTable(dt, int.Parse(pageIndex), int.Parse(pageSize));
                dic.Add("DataHead", dthead);
                dic.Add("Data", dt);
                //dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 查询-APP2合规-主页-模板维护
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_Compliance_Maintenance(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string sql = $@"select q.space_str_1,q.space_str_2,q.space_str_3,q.space_str_4,q.space_str_5,q.space_str_6,q.autograph_img_guid,f.FILE_URL from qcm_ex_app_m q
                                left join BDM_UPLOAD_FILE_ITEM f on q.autograph_img_guid=f.guid";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }


        /// <summary>
        /// 编辑-APP2合规-模板维护
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditAPP_Compliance_Maintenance(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string space_str_1 = jarr.ContainsKey("space_str_1") ? jarr["space_str_1"].ToString() : "";//查询条件 空白区1
                string space_str_2 = jarr.ContainsKey("space_str_2") ? jarr["space_str_2"].ToString() : "";//查询条件 空白区2
                string space_str_3 = jarr.ContainsKey("space_str_3") ? jarr["space_str_3"].ToString() : "";//查询条件 空白区3
                string space_str_4 = jarr.ContainsKey("space_str_4") ? jarr["space_str_4"].ToString() : "";//查询条件 空白区4
                string space_str_5 = jarr.ContainsKey("space_str_5") ? jarr["space_str_5"].ToString() : "";//查询条件 空白区5
                string space_str_6 = jarr.ContainsKey("space_str_6") ? jarr["space_str_6"].ToString() : "";//查询条件 空白区6
                string autograph_img_guid = jarr.ContainsKey("autograph_img_guid") ? jarr["autograph_img_guid"].ToString() : "";//查询条件 签字图片guid
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;

                int count = DB.GetInt32($@"select count(1) from qcm_ex_app_m");

                if (count<=0)
                    sql = $@"insert into qcm_ex_app_m (space_str_1,space_str_2,space_str_3,space_str_4,space_str_5,space_str_6,autograph_img_guid,
                            createby,createdate,createtime) 
                            values('{space_str_1}','{space_str_2}','{space_str_3}','{space_str_4}','{space_str_5}','{space_str_6}',
                            '{autograph_img_guid}','{user}','{date}','{time}')";
                else
                sql = $@"update qcm_ex_app_m set space_str_1='{space_str_1}',space_str_2='{space_str_2}',space_str_3='{space_str_3}',
                        space_str_4='{space_str_4}',space_str_5='{space_str_5}',space_str_6='{space_str_6}',autograph_img_guid='{autograph_img_guid}',
                        modifyby='{user}',modifydate='{date}',modifytime='{time}'";

                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查询-APP2合规-下载APP2报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_Compliance_Download(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string DueDateS = jarr.ContainsKey("DueDateS") ? jarr["DueDateS"].ToString() : "";//查询条件 订单到期日期开始
                string DueDateE = jarr.ContainsKey("DueDateE") ? jarr["DueDateE"].ToString() : "";//查询条件 订单到期日期结束
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art

                string where = string.Empty;
                string sql = $@"SELECT
NLT as NST,
m.MER_PO
FROM
	bdm_se_order_item i
	INNER JOIN BDM_SE_ORDER_MASTER m on i.SE_ID=m.SE_ID and i.ORG_ID=m.ORG_ID
WHERE
	NLT BETWEEN TO_DATE( @DueDateS, 'yyyy-mm-dd HH24:mi:ss' ) 
	AND TO_DATE( @DueDateE, 'yyyy-mm-dd HH24:mi:ss' )  and i.PROD_NO=@prod_no 
and m.se_type<>'ZOR8' AND m.SE_TYPE<>'ZMT1' AND m.SE_TYPE<>'ZMT2'
    ";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("DueDateS", $@"{DueDateS}");
                paramTestDic.Add("DueDateE", $@"{DueDateE}");
                paramTestDic.Add("prod_no", $@"{prod_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                DataTable newDt = new DataTable();
                newDt.Columns.Add("NST");
                newDt.Columns.Add("MER_PO");
                List<string> nstList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    if (!nstList.Contains(item["NST"].ToString()))
                    {
                        nstList.Add(item["NST"].ToString());
                    }
                    DataTable dtt = DB.GetDataTable($@"select custorder,SHIPCOUNTRY_EN from bdm_se_order_master where mer_po='0130585737'");
                }

                string mer_pos = string.Empty;
                for (int i = 0; i < nstList.Count; i++)
                {
                    DataRow[] dr = dt.Select($@"NST='{nstList[i]}'");
                    for (int a = 0; a < dr.Length; a++)
                    {
                        mer_pos += dr[a]["MER_PO"].ToString()+",";
                    }
                    newDt.Rows.Add();
                    newDt.Rows[newDt.Rows.Count - 1]["MER_PO"] = mer_pos.TrimEnd(',');

                    string date = string.Empty;
                    try { date = Convert.ToDateTime(dr[0]["NST"]).ToString("yyyy-MM-dd"); }catch(Exception ex) { }
                    newDt.Rows[newDt.Rows.Count - 1]["NST"] = date;
                    mer_pos = string.Empty; 
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", newDt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查询-APP2合规-下载APP2报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAPP_Compliance_Customer(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string DueDateS = jarr.ContainsKey("DueDateS") ? jarr["DueDateS"].ToString() : "";//查询条件 订单到期日期开始
                string DueDateE = jarr.ContainsKey("DueDateE") ? jarr["DueDateE"].ToString() : "";//查询条件 订单到期日期结束
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art

                var prod_no_list = prod_no.Split(',');
                string where = string.Empty;
                string sql = $@"select {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ACC_CUSTID", "ACC_CUSTID")}  from bdm_se_order_master where mer_po in ({string.Join(',', prod_no_list.Select(x => $"'{x}'"))})  ";
                string sql2 = $@"
select 
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT C_NAME", "C_NAME")}  
from bdm_se_order_master 
LEFT JOIN BDM_Country ON (L_NO='EN' AND C_NO=DESCOUNTRY_CODE)
where mer_po in ({string.Join(',', prod_no_list.Select(x => $"'{x}'"))})";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("mer_po", $@"{prod_no}");

                //string custorder = DB.GetString(sql, paramTestDic);
                //string shipcountry_en = DB.GetString(sql2, paramTestDic);

                string custorder = DB.GetString(sql);
                string shipcountry_en = DB.GetString(sql2);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("custorder", custorder);
                dic.Add("shipcountry_en", shipcountry_en);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)//PageIndex表示第几页，PageSize表示每页的记录数
        {
            if (PageIndex == 0)
                return dt;//0页代表每页数据，直接返回

            DataTable newdt = dt.Copy();
            newdt.Clear();//copy dt的框架

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;//源数据记录数小于等于要显示的记录，直接返回dt

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }
            return newdt;
        }

        internal static double GetMonths(DateTime from, DateTime to)
        {
            /// |-------X----|---------------|---------------|--X-----------|
            ///         ^                                       ^
            ///       from                                     to

            //change the dates if to is before from
            if (to.Ticks < from.Ticks)
            {
                DateTime temp = from;
                from = to;
                to = temp;
            }

            /// Gets the day percentage of the months = 0...1
            ///
            /// 0            1                               0              1
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^                                    ^^^^
            /// percFrom                                    percTo
            double percFrom = (double)from.Day / DateTime.DaysInMonth(from.Year, from.Month);
            double percTo = (double)to.Day / DateTime.DaysInMonth(to.Year, to.Month);

            /// get the amount of months between the two dates based on day one
            /// 
            /// |-------X----|---------------|---------------|--X-----------|
            /// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                        months
            double months = (to.Year * 12 + to.Month) - (from.Year * 12 + from.Month);

            /// Return the right parts
            /// 
            /// |-------X----|---------------|---------------|--X-----------|            
            ///         ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
            ///                      return
            return months - percFrom + percTo;
        }
    }
}
