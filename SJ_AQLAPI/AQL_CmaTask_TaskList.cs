using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    /// <summary>
    /// AQL任务清单
    /// </summary>
    public class AQL_CmaTask_TaskList
    {

        /// <summary>
        /// 查询SE_ID出货状态 AQL任务清单 界面
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_Main_CHZT_T(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(Data);

                Dictionary<string, string> seidRes = new Dictionary<string, string>();
                if (dataList.Count() > 0)
                {
                    var seidList = dataList.Select(x => $@"'{x.Key}'");
                    //进仓dt
                    DataTable mms_finishedtrackin_list_dt = DB.GetDataTable($@"
SELECT 
    SE_ID,SUM(qty) as qty 
FROM 
mms_finishedtrackin_list 
WHERE SE_ID IN ({string.Join(',', seidList)}) 
GROUP BY SE_ID");
                    //出货dt
                    DataTable bmd_se_shipment_dt = DB.GetDataTable($@"
SELECT 
	m.SE_ID,
	SUM(d.SHIPPING_QTY) AS SHIPPING_QTY
FROM 
bmd_se_shipment_m m 
INNER JOIN bmd_se_shipment_d d ON d.SHIPPING_NO=m.SHIPPING_NO 
WHERE m.SE_ID IN({string.Join(',', seidList)}) 
GROUP BY m.SE_ID");
                    DataTable WMS_SALES_ORDER_REPLACE_dt = DB.GetDataTable($@"
SELECT 
    SE_ID 
FROM 
WMS_SALES_ORDER_REPLACE 
WHERE SE_ID IN({string.Join(',', seidList)})  
GROUP BY SE_ID
");
                    foreach (var item in dataList)
                    {
                        string chzt = "";

                        decimal column2 = 0;//订单数量
                        bool column2_convert = decimal.TryParse(item.Value["column2"].ToString(), out column2);
                        decimal se_qty = 0;//订单有效数量
                        bool se_qty_convert = decimal.TryParse(item.Value["se_qty"].ToString(), out se_qty);
                        decimal qty = 0;//进仓数量
                        var find_mms_finishedtrackin_list = mms_finishedtrackin_list_dt.Select($@"SE_ID='{item.Key}'");
                        if (find_mms_finishedtrackin_list.Length > 0)
                        {
                            bool qty_convert = decimal.TryParse(find_mms_finishedtrackin_list[0]["qty"].ToString(), out qty);
                        }
                        decimal shipping_qty = 0;//出货数量
                        var bmd_se_shipment_list = bmd_se_shipment_dt.Select($@"SE_ID='{item.Key}'");
                        if (bmd_se_shipment_list.Length > 0)
                        {
                            bool shipping_qty_convert = decimal.TryParse(bmd_se_shipment_list[0]["SHIPPING_QTY"].ToString(), out shipping_qty);
                        }

                        if (column2 > se_qty)
                        {
                            if (qty == se_qty && shipping_qty == se_qty)
                            {
                                chzt = "Shipped";//已出货
                            }
                        }

                        if (column2 == se_qty)
                        {
                            if (shipping_qty > 0 && shipping_qty == column2)
                            {
                                chzt = "Shipped";//已出货
                            }
                        }

                        if (string.IsNullOrEmpty(chzt))
                            chzt = "Not_Shipped";//未出货

                        if (!seidRes.ContainsKey(item.Key))
                        {
                            seidRes.Add(item.Key, chzt);
                        }
                    }

                }

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(seidRes);
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
        /// 查询Art图片接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_Main_IMG(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 获取图片
                SJeMES_Framework_NETCore.DBHelper.DataBase sqlseverDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);
                var nfspath = sqlseverDB.GetString($@"
                SELECT
                	nfspath
                FROM
                	[dbo].[SYSORG01M]
                WHERE
                	UPPER (org) = '{companyCode.ToUpper()}';");
                Dictionary<string, string> artImgDic = new Dictionary<string, string>();
                Dictionary<string, string> res = new Dictionary<string, string>();
                foreach (var item in dataList)
                {
                    string currArtNo = item.Value.ToString();
                    if (!artImgDic.ContainsKey(currArtNo))
                    {
                        var sapApiReq = new Art.Image.DT_PP117_REQI_REQUESTBODY();
                        sapApiReq.art_no = currArtNo;
                        var sapApiRes = SJ_SAPAPI.SapApiHelper.GetArtImage(sapApiReq, ReqObj);
                        if (sapApiRes.IS_SUCCESS.ToLower() == "true")
                        {
                            if (sapApiRes.RES.DATAS != null && sapApiRes.RES.DATAS.Length > 0)
                            {
                                string ZPIC_DES = "";
                                string ZMATNR_FILENAME = "";
                                foreach (var sapImg in sapApiRes.RES.DATAS)
                                {
                                    if (sapImg.ZPIC_DES.ToLower() == "u")
                                    {
                                        ZPIC_DES = sapImg.ZPIC_DES;
                                        ZMATNR_FILENAME = sapImg.ZMATNR_FILENAME;
                                        break;
                                    }
                                }

                                string fileSavePath = System.IO.Path.Combine("sapArtImg", currArtNo, ZPIC_DES, ZMATNR_FILENAME);
                                string nfsFilePath = Path.Combine(nfspath, ZMATNR_FILENAME);
                                if (string.IsNullOrEmpty(ZPIC_DES))
                                {

                                    fileSavePath = "";
                                }
                                else
                                {
                                    fileSavePath = "/" + ZMATNR_FILENAME;
                                    //if (File.Exists(nfsFilePath))
                                    //{
                                    //    fileSavePath = "/" + ZMATNR_FILENAME;
                                    //    //var filepath = Directory.GetCurrentDirectory() + @"\wwwroot";
                                    //    ////不存在就创建文件夹
                                    //    //string dicPath = Path.Combine(filepath, fileSavePath.Substring(0, fileSavePath.LastIndexOf(@"\") + 1));
                                    //    //if (!Directory.Exists(dicPath))
                                    //    //{
                                    //    //    System.IO.Directory.CreateDirectory(dicPath);
                                    //    //}
                                    //    //string file = Path.Combine(filepath, fileSavePath);
                                    //    //if (System.IO.File.Exists(file))
                                    //    //{
                                    //    //    try
                                    //    //    {

                                    //    //        File.Delete(file);
                                    //    //        System.IO.File.Copy(nfsFilePath, file);
                                    //    //    }
                                    //    //    catch
                                    //    //    {

                                    //    //    }
                                    //    //}
                                    //    //else
                                    //    //{
                                    //    //    System.IO.File.Copy(nfsFilePath, file);
                                    //    //}

                                    //}
                                    //else
                                    //{
                                    //    fileSavePath = "";
                                    //}
                                }
                                artImgDic.Add(currArtNo, fileSavePath);
                            }
                            else
                            {
                                artImgDic.Add(currArtNo, "");
                            }
                        }
                        else
                        {
                            artImgDic.Add(currArtNo, "");
                        }
                    }

                    res.Add(item.Key, artImgDic[currArtNo]);
                }
                #endregion

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(res);
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
        /// 查询-AQL任务清单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_Main(object OBJ)//new
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
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sccq = jarr.ContainsKey("sccq") ? jarr["sccq"].ToString() : "";//生产厂区
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string art_name = jarr.ContainsKey("art_name") ? jarr["art_name"].ToString() : "";//art
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型
                string f_inspection_timeS = jarr.ContainsKey("f_inspection_timeS") ? jarr["f_inspection_timeS"].ToString() : "";//进仓日期开始
                string f_inspection_timeE = jarr.ContainsKey("f_inspection_timeE") ? jarr["f_inspection_timeE"].ToString() : "";//进仓日期结束
                string zhubie = jarr.ContainsKey("zhubie") ? jarr["zhubie"].ToString() : "";//组别
                string guojia = jarr.ContainsKey("guojia") ? jarr["guojia"].ToString() : "";//国家
                string chzt = jarr.ContainsKey("chzt") ? jarr["chzt"].ToString() : "";//出货状态
             
                string inspection_state = jarr.ContainsKey("inspection_state") ? jarr["inspection_state"].ToString() : "";//验货状态

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(sccq))
                {
                    where += " and cx.from_line like @sccq";
                    paramTestDic.Add("sccq", $@"%{sccq}%");
                }
              
                if (!string.IsNullOrWhiteSpace(zhubie))
                {
                    where += " and cx.from_line like @zhubie";
                    paramTestDic.Add("zhubie", $@"%{zhubie}%");
                }

                if (!string.IsNullOrWhiteSpace(po))
                {
                    where += " and a.po like @po";
                    paramTestDic.Add("po", $@"%{po}%");
                }
                if (!string.IsNullOrWhiteSpace(art_name))
                {
                    where += " and a.art_no like @art_name";
                    paramTestDic.Add("art_name", $@"%{art_name}%");
                }
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += " and d.name_t like @shoe_name";
                    paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                }
                if (!string.IsNullOrWhiteSpace(guojia))
                {
                    where += " and e.c_name like @guojia";
                    paramTestDic.Add("guojia", $@"%{guojia}%");
                }
                if (!string.IsNullOrWhiteSpace(inspection_state))
                {
                    where += " and a.inspection_state=@inspection_state";
                    paramTestDic.Add("inspection_state", $@"{inspection_state}");
                }
                if (!string.IsNullOrWhiteSpace(f_inspection_timeS) && !string.IsNullOrWhiteSpace(f_inspection_timeE))
                {
                    where += $@" and a.f_inspection_time>=@f_inspection_timeS and a.f_inspection_time<= @f_inspection_timeE";
                    paramTestDic.Add("f_inspection_timeS", $@"{f_inspection_timeS}");
                    paramTestDic.Add("f_inspection_timeE", $@"{f_inspection_timeE}");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(f_inspection_timeS))
                    {
                        where += $@" and a.f_inspection_time >= @f_inspection_timeS ";
                        paramTestDic.Add("f_inspection_timeS", $@"{f_inspection_timeS}");
                    }
                    if (!string.IsNullOrWhiteSpace(f_inspection_timeE))
                    {
                        where += $@" and a.f_inspection_time <= @f_inspection_timeE ";
                        paramTestDic.Add("f_inspection_timeE", $@"{f_inspection_timeE}");
                    }
                }
                if (string.IsNullOrWhiteSpace(where))
                {
                    where += @"and a.createdate between
                       to_char(sysdate - 3, 'yyyy-MM-dd') and
                       to_char(sysdate, 'yyyy-MM-dd')";
                }

                string sql = string.Empty;
                sql = $@"SELECT 
                        to_char(INSPECTION_DATE,'yyyy-MM-dd') as INSPECTION_DATE,
                        a.IS_INSPECTION,
                        a.id,
                        a.task_no,
                        cx.from_line as sccq,
                        a.po,
                        a.art_no,
                        r.shoe_no,
                        r.name_t as art_name,
                        d.name_t as shoe_name,
                        '' as bq,
                        pm.SE_CUSTID as kh,
                        e.c_name as guojia,
                        pi.COLUMN1 as vas,
                        a.po_num,
                        a.lot_num,
                        a.order_level,
(
CASE
	WHEN PG_WMS.GF_GET_FULL_RATE (pm.SE_ID) != '100%' THEN 'Not_Full_Box'--未满箱
    WHEN PG_WMS.GF_GET_FULL_RATE (pm.SE_ID) = '100%' THEN 'Full_Box'--已满箱
	ELSE ''
END
) as full_state,-- 满箱状态
                        -- a.full_state,
                        a.inspection_state,
                        '' as chzt,-- 出货状态
                        a.f_inspection_time,
                        fhr.staff_name as factory_autograph,
                        a.factory_autograph_date,
                        chr.staff_name as customer_autograph,
                        a.customer_autograph_date,
                        a.task_type,
                        a.pb_state,
                        a.inspection_type,
                        a.effective_status,
                        '' as inspection_results,
                        c.sample_level,
                        c.aql_level,
                        '' as art_img_url,
                        a.BA_EDIT_STATE,
                        a.H_EDIT_STATE,
                        a.AQL_EDIT_STATE,
                        a.PH_EDIT_STATE,
                        pi.column2,-- 订单数量
                        pi.se_qty,-- 订单有效数量
                        pm.SE_ID,
                        pm.ORG_ID,
                        d.style_seq as rule_no,
                        -- (select MAX(bb.name_t) from bdm_cd_code bb where bb.code_no=d.style_seq) as rule_no,
                        pm.SE_ID as from_line,
                        -- (select listagg(distinct from_line,',') from mms_finishedtrackin_list where se_id=pm.SE_ID) as from_line,
                        hr.staff_name as CHECKER
                        FROM
                        aql_cma_task_list_m a
                        inner JOIN bdm_rd_prod r on a.art_no=r.prod_no
                        inner JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                        -- LEFT JOIN bdm_cd_code bb ON d.style_seq=bb.code_no
                        LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no=a.task_no 
                        inner JOIN BDM_SE_ORDER_MASTER pm ON pm.MER_PO=a.po 
                        inner JOIN BDM_COUNTRY e on pm.descountry_code = e.c_no and e.l_no='EN'
                        inner JOIN BDM_SE_ORDER_ITEM pi ON pi.ORG_ID=pm.ORG_ID and pi.SE_ID=pm.SE_ID 
                        left join hr001m hr on hr.staff_no = a.CHECKER 
                        left join hr001m fhr on fhr.staff_no = a.factory_autograph 
                        left join hr001m chr on chr.staff_no = a.customer_autograph 
                        inner join (
select 
    a.po,
    listagg(DISTINCT a.from_line, ',') WITHIN GROUP (ORDER BY a.from_line) as from_line
FROM
	mms_finishedtrackin_list a 
group by a.po
) cx on cx.po=a.po 
                        where 1=1 {where} order by a.id desc";
                DataTable dt = new DataTable();
                if (string.IsNullOrEmpty(chzt))
                    dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                else
                    dt = DB.GetDataTable(sql, paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, string> artImgDic = new Dictionary<string, string>();
                List<string> tasknoList = new List<string>();
                List<string> poList = new List<string>();
                List<string> seidList = new List<string>();
                List<string> style_seqList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    tasknoList.Add($@"'{item["task_no"]}'");
                    poList.Add($@"'{item["po"]}'");
                    if (!string.IsNullOrEmpty(item["SE_ID"].ToString()))
                        seidList.Add($@"'{item["SE_ID"]}'");
                    if (!string.IsNullOrEmpty(item["rule_no"].ToString()))
                        style_seqList.Add($@"'{item["rule_no"]}'");
                }
                poList = poList.Distinct().ToList();
                seidList = seidList.Distinct().ToList();
                style_seqList = style_seqList.Distinct().ToList();

                DataTable from_line_dt = null;
                DataTable name_t_dt = null;
                if (seidList.Count() > 0)
                {
                    string from_line_sql = $@"
select 
	se_id,
	listagg(distinct from_line,',') AS from_line 
from mms_finishedtrackin_list 
WHERE SE_ID IN({string.Join(",", seidList)}) 
GROUP BY se_id
";
                    from_line_dt = DB.GetDataTable(from_line_sql);
                }
                if (style_seqList.Count() > 0)
                {
                    string name_t_sql = $@"
select 
	bb.code_no,
	MAX(bb.name_t) as name_t 
from bdm_cd_code bb 
WHERE bb.CODE_NO IN({string.Join(",", style_seqList)}) 
GROUP BY bb.code_no
";
                    name_t_dt = DB.GetDataTable(name_t_sql);
                }

                if ((from_line_dt != null && from_line_dt.Rows.Count > 0) || (name_t_dt != null && name_t_dt.Rows.Count > 0))
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        if (from_line_dt != null && from_line_dt.Rows.Count != 0)
                        {
                            var find_from_line = from_line_dt.Select($@"se_id='{item["from_line"]}'");
                            if (find_from_line != null && find_from_line.Length > 0)
                            {
                                item["from_line"] = find_from_line[0]["from_line"].ToString();
                            }
                            else
                            {
                                item["from_line"] = "";
                            }
                        }
                        else
                        {
                            item["from_line"] = "";
                        }
                        if (name_t_dt != null && name_t_dt.Rows.Count != 0)
                        {
                            var find_name_t = name_t_dt.Select($@"code_no='{item["rule_no"]}'");
                            if (find_name_t != null && find_name_t.Length > 0)
                            {
                                item["rule_no"] = find_name_t[0]["name_t"].ToString();
                            }
                            else
                            {
                                item["rule_no"] = "";
                            }
                        }
                        else
                        {
                            item["rule_no"] = "";
                        }
                    }
                }
                else
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        item["from_line"] = "";
                        item["rule_no"] = "";
                    }
                }

                #region 出货状态
                //                if (seidList.Count() > 0)
                //                {
                //                    //进仓dt
                //                    DataTable mms_finishedtrackin_list_dt = DB.GetDataTable($@"
                //SELECT 
                //    SE_ID,SUM(qty) as qty 
                //FROM 
                //mms_finishedtrackin_list 
                //WHERE SE_ID IN ({string.Join(',', seidList)}) 
                //GROUP BY SE_ID");
                //                    //出货dt
                //                    DataTable bmd_se_shipment_dt = DB.GetDataTable($@"
                //SELECT 
                //	m.SE_ID,
                //	SUM(d.SHIPPING_QTY) AS SHIPPING_QTY
                //FROM 
                //bmd_se_shipment_m m 
                //INNER JOIN bmd_se_shipment_d d ON d.SHIPPING_NO=m.SHIPPING_NO 
                //WHERE m.SE_ID IN({string.Join(',', seidList)}) 
                //GROUP BY m.SE_ID");
                //                    DataTable WMS_SALES_ORDER_REPLACE_dt = DB.GetDataTable($@"
                //SELECT 
                //    SE_ID 
                //FROM 
                //WMS_SALES_ORDER_REPLACE 
                //WHERE SE_ID IN({string.Join(',', seidList)})  
                //GROUP BY SE_ID
                //");
                //                    foreach (DataRow item in dt.Rows)
                //                    {
                //                        var find_WMS_SALES_ORDER_REPLACE = WMS_SALES_ORDER_REPLACE_dt.Select($@"SE_ID='{item["SE_ID"]}'");
                //                        if (find_WMS_SALES_ORDER_REPLACE.Length > 0)
                //                        {
                //                            //item["chzt"] = "订单替换";
                //                            item["chzt"] = "未出货";
                //                            continue;
                //                        }

                //                        decimal column2 = 0;//订单数量
                //                        bool column2_convert = decimal.TryParse(item["column2"].ToString(), out column2);
                //                        decimal se_qty = 0;//订单有效数量
                //                        bool se_qty_convert = decimal.TryParse(item["se_qty"].ToString(), out se_qty);
                //                        decimal qty = 0;//进仓数量
                //                        var find_mms_finishedtrackin_list = mms_finishedtrackin_list_dt.Select($@"SE_ID='{item["SE_ID"]}'");
                //                        if (find_mms_finishedtrackin_list.Length > 0)
                //                        {
                //                            bool qty_convert = decimal.TryParse(find_mms_finishedtrackin_list[0]["qty"].ToString(), out qty);
                //                        }
                //                        decimal shipping_qty = 0;//出货数量
                //                        var bmd_se_shipment_list = bmd_se_shipment_dt.Select($@"SE_ID='{item["SE_ID"]}'");
                //                        if (bmd_se_shipment_list.Length > 0)
                //                        {
                //                            bool shipping_qty_convert = decimal.TryParse(bmd_se_shipment_list[0]["SHIPPING_QTY"].ToString(), out shipping_qty);
                //                        }
                //                        if (column2 == se_qty)
                //                        {
                //                            if (shipping_qty > 0 && shipping_qty == column2)
                //                            {
                //                                //item["chzt"] = "已出货";
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                            else if (shipping_qty == 0)
                //                            {
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                            else
                //                            {
                //                                //item["chzt"] = "分批出货";
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                        }
                //                        if (column2 > se_qty)
                //                        {
                //                            if (qty == se_qty && shipping_qty == se_qty)
                //                            {
                //                                item["chzt"] = "已出货";
                //                                continue;
                //                            }
                //                            else if (qty > shipping_qty && qty > se_qty)
                //                            {
                //                                //item["chzt"] = "订单减少";
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                            else if ((se_qty >= qty && qty > shipping_qty) || (se_qty > qty && qty >= shipping_qty))
                //                            {

                //                                //item["chzt"] = "分批出货";
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                            else if (qty > 0 && qty <= se_qty && shipping_qty == 0)
                //                            {
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                        }
                //                        if (column2 != se_qty && se_qty == 0)
                //                        {
                //                            //item["chzt"] = "订单取消";
                //                            item["chzt"] = "未出货";
                //                            continue;
                //                        }

                //                        item["chzt"] = "未出货";
                //                    }

                //                    if (!string.IsNullOrEmpty(chzt))
                //                    {
                //                        var findDt = dt.Select($@"chzt='{chzt}'");
                //                        if (findDt != null && findDt.Length > 0)
                //                        {
                //                            DataTable dt_new = dt.Clone();
                //                            for (int i = 0; i < findDt.Length; i++)
                //                            {
                //                                dt_new.ImportRow(findDt[i]);
                //                            }
                //                            dt = dt_new;
                //                        }
                //                        else
                //                        {
                //                            dt = new DataTable();
                //                        }
                //                    }
                //                }
                #endregion

                //if (poList.Count() > 0)
                //{
                //                    var from_line_list = DB.GetDataTable($@"
                //select 
                //    a.po,
                //    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT a.from_line", "a.from_line")} as from_line
                //FROM
                //	mms_finishedtrackin_list a 
                //WHERE a.po IN ({string.Join(",", poList)}) 
                //group by a.po
                //");
                //                    foreach (DataRow item in dt.Rows)
                //                    {
                //                        string item_po = item["po"].ToString();
                //                        if (!string.IsNullOrEmpty(item_po))
                //                        {
                //                            var findPo = from_line_list.Select($@"po='{item_po}'");
                //                            if (findPo.Length > 0)
                //                                item["sccq"] = findPo[0]["from_line"];
                //                        }
                //                    }
                //}

                #region 验货结果
                if (tasknoList.Count > 0)
                {
                    string insResSql = $@"
                SELECT
                    a.task_no,
                    MAX(a.bad_classify_code) as bad_classify_code,
                    MAX(a.bad_item_code) as bad_item_code,
                    MAX(a.bad_item_name) as bad_item_name,
                    MAX(a.problem_level) as problem_level,
                    MAX(a.bad_qty) as bad_qty
                from 
                    aql_cma_task_list_m_aql_e_br a 
                Where a.task_no IN ({string.Join(",", tasknoList)})
                GROUP BY a.task_no,a.bad_classify_code,a.bad_item_code";
                    DataTable insResDt = DB.GetDataTable(insResSql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string task_no = item["task_no"].ToString();
                        if (!string.IsNullOrEmpty(task_no))
                        {
                            int zy = 0;//主要
                            int cy = 0;//次要
                            int yz = 0;//严重
                            var findTaskList = insResDt.Select($@"task_no='{task_no}'");
                            if (findTaskList.Length > 0)
                            {
                                foreach (var task_item in findTaskList)
                                {
                                    switch (task_item["problem_level"])
                                    {
                                        case "0":
                                            zy += Convert.ToInt32(task_item["bad_qty"].ToString());
                                            break;
                                        case "1":
                                            cy += Convert.ToInt32(task_item["bad_qty"].ToString());
                                            break;
                                        case "2":
                                            yz += Convert.ToInt32(task_item["bad_qty"].ToString());
                                            break;
                                        default:
                                            break;
                                    }
                                }
                            }

                            int hjbl = cy + zy + yz;//合计不良

                            string sample_level = item["sample_level"].ToString();
                            string aql_level = item["aql_level"].ToString();
                            if (string.IsNullOrEmpty(sample_level) || string.IsNullOrEmpty(sample_level))
                            {
                                sample_level = "2";
                                aql_level = "AC13";
                            }
                            string ac = aql_level;//查询条件 ac
                            string num = item["lot_num"].ToString();//查询条件 任务数量(分批数量)
                            string LEVEL_TYPE = sample_level;//查询条件 样本级别
                            string acSql = $@"select VALS, {ac} as AC from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                            DataTable acDt = DB.GetDataTable(acSql);
                            if (acDt.Rows.Count > 0)
                            {
                                int acInt = 0;
                                bool cRes = int.TryParse(acDt.Rows[0]["AC"].ToString(), out acInt);
                                if (cRes)
                                {
                                    if (hjbl > acInt)
                                        item["inspection_results"] = "Reject";
                                    else
                                        item["inspection_results"] = "Accepted";
                                }
                            }

                        }
                    }
                }
                #endregion

                #region 获取图片
                SJeMES_Framework_NETCore.DBHelper.DataBase sqlseverDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);
                var nfspath = sqlseverDB.GetString($@"
                SELECT
                	nfspath
                FROM
                	[dbo].[SYSORG01M]
                WHERE
                	UPPER (org) = '{companyCode.ToUpper()}';");
                foreach (DataRow item in dt.Rows)
                {
                    string currArtNo = item["art_no"].ToString();
                    if (!artImgDic.ContainsKey(currArtNo))
                    {
                        var sapApiReq = new Art.Image.DT_PP117_REQI_REQUESTBODY();
                        sapApiReq.art_no = currArtNo;
                        var sapApiRes = SJ_SAPAPI.SapApiHelper.GetArtImage(sapApiReq, ReqObj);
                        if (sapApiRes.IS_SUCCESS.ToLower() == "true")
                        {
                            if (sapApiRes.RES.DATAS != null && sapApiRes.RES.DATAS.Length > 0)
                            {

                                string fileSavePath = System.IO.Path.Combine("sapArtImg", currArtNo, sapApiRes.RES.DATAS[0].ZPIC_DES, sapApiRes.RES.DATAS[0].ZMATNR_FILENAME);

                                //string nfsFilePath = Path.Combine(nfspath, sapApiRes.RES.DATAS[0].ZMATNR_FILENAME);//OLD PATH 

                                string nfsFilePath = $@"\\10.2.198.3" + sapApiRes.RES.DATAS[0].ZMATNR_DIR + sapApiRes.RES.DATAS[0].ZMATNR_FILENAME; //NEW PATH

                                //string nfsFilePath = Path.Combine(nfspath, sapApiRes.RES.DATAS[0].ZMATNR_DIR, sapApiRes.RES.DATAS[0].ZMATNR_FILENAME);//NEW PATH

                                if (File.Exists(nfsFilePath))
                                {
                                    var filepath = Directory.GetCurrentDirectory() + @"\wwwroot";
                                    //不存在就创建文件夹
                                    string dicPath = Path.Combine(filepath, fileSavePath.Substring(0, fileSavePath.LastIndexOf(@"\") + 1));
                                    if (!Directory.Exists(dicPath))
                                    {
                                        System.IO.Directory.CreateDirectory(dicPath);
                                    }
                                    string file = Path.Combine(filepath, fileSavePath);
                                    if (System.IO.File.Exists(file))
                                    {
                                        try
                                        {

                                            File.Delete(file);
                                            System.IO.File.Copy(nfsFilePath, file);
                                        }
                                        catch
                                        {

                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.Copy(nfsFilePath, file);
                                    }

                                }
                                else
                                {
                                    fileSavePath = "";
                                }
                                artImgDic.Add(currArtNo, fileSavePath);
                            }
                            else
                            {
                                artImgDic.Add(currArtNo, "");
                            }
                        }
                        else
                        {
                            artImgDic.Add(currArtNo, "");
                        }
                    }
                    item["art_img_url"] = artImgDic[currArtNo];
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                //DataRow[] dr = new DataRow[] { };
                //string Wherestr = string.Empty;
                //DataTable dtt = dt.Clone();
                //if (string.IsNullOrEmpty(sccq) && string.IsNullOrEmpty(zhubie))
                //{
                //    dic.Add("Data", dt);
                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(sccq))
                //        dr = dt.Select($@"sccq like '%{sccq}%' ");//or sccq like '%{zhubie}%'
                //    if (!string.IsNullOrEmpty(zhubie))
                //        dr = dt.Select($@" sccq like '%{zhubie}%'");//or sccq like '%{zhubie}%'

                //    //dic.Add("Data", dr);

                //    foreach (DataRow row in dr)
                //    {
                //        dtt.ImportRow(row);
                //    }
                //    dic.Add("Data", dtt);
                //}

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                #region 查询是否有权限清除数据
                string getPHSql = $@"SELECT aql_clear_autograph FROM AQL_SIGNING_AUTOGRAPH_SETTING_M WHERE STAFF_NO='{user}'";
                var phRes = DB.GetString(getPHSql);
                phRes = string.IsNullOrEmpty(phRes) ? "" : phRes;

                dic.Add("aql_clear_autograph", phRes);
                #endregion
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

        /// <summary>
        /// 查询-新增AQL验货任务-PO
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_InsertPo(object OBJ)
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
                string sql = string.Empty;
                
                sql = $@"
                        SELECT res.MER_PO FROM (
                        SELECT
                         A .MER_PO,
                         MAX(b.SE_QTY) AS item_se_qty,
                         SUM(c.SE_QTY) AS size_se_qty
                        FROM
                         BDM_SE_ORDER_MASTER A
                        INNER JOIN BDM_SE_ORDER_ITEM b ON A .SE_ID = b.SE_ID AND A.ORG_ID=b.ORG_ID 
                        INNER JOIN BDM_SE_ORDER_SIZE c ON A .SE_ID = c.SE_ID AND a.ORG_ID=c.ORG_ID 
                        WHERE c.SE_QTY>0 and a.PO_AGGREGATOR is not null
                        GROUP BY A .MER_PO
                        ) res 
                        WHERE RES.ITEM_SE_QTY=RES.SIZE_SE_QTY";
              

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
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
        /// 新增-新增AQL验货任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// 

        #region Before Po Change2 on 2025/02/13
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertCmaTask_TaskList_Insert(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//查询条件 po
        //        List<string> lot_nums = jarr.ContainsKey("lot_nums") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["lot_nums"].ToString()) : new List<string>();//查询条件 分批数量
        //        string date = DateTime.Now.ToString("yyyy-MM-dd");
        //        string time = DateTime.Now.ToString("HH:mm:ss");
        //        string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
        //        string sql = string.Empty;
        //        string task_no = string.Empty;//任务编号

        //        int countpo = DB.GetInt32($@"select count(1) from BDM_SE_ORDER_MASTER where MER_PO='{po}'");
        //        if (countpo <= 0)
        //        {
        //            ret.IsSuccess = false;
        //            ret.ErrMsg = "PO number does not exist!";//PO号不存在
        //            return ret;
        //        }

        //        int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m where po='{po}'");
        //        if (count > 0)
        //        {
        //            ret.IsSuccess = false;
        //            ret.ErrMsg = "PO number cannot be created repeatedly!";
        //            return ret;
        //        }

        //        // 先查出此po的码数明细
        //        DataTable dt = DB.GetDataTable($@"SELECT
        //                                     bb.CR_SIZE,
        //                                     SUM( bb.SE_QTY ) SE_QTY 
        //                                    FROM
        //                                     (
        //                                     SELECT
        //                                      s.SIZE_NO as CR_SIZE,
        //                                      s.SE_QTY,
        //                                       s.SIZE_SEQ
        //                                     FROM
        //                                      BDM_SE_ORDER_MASTER m
        //                                      INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
        //                                      AND m.SE_ID = s.SE_ID 
        //                                     WHERE
        //                                      m.MER_PO = '{po}' 
        //                                     ) bb 
        //                                    GROUP BY bb.CR_SIZE 
        //                                    ORDER BY  MAX(bb.SIZE_SEQ)
        //                                    ");
        //        DataTable dt_copy = dt.Copy();

        //        for (int i = 0; i < lot_nums.Count; i++)
        //        {

        //            task_no = po + "-" + (i + 1).ToString();

        //            //art
        //            string art_no = DB.GetString($@"select b.PROD_NO from BDM_SE_ORDER_MASTER a left join BDM_SE_ORDER_ITEM b on a.SE_ID=b.SE_ID where a.MER_PO='{po}'");

        //            //PO数量
        //            string po_num = DB.GetString($@"select b.SE_QTY from BDM_SE_ORDER_MASTER a left join BDM_SE_ORDER_ITEM b on a.SE_ID=b.SE_ID where a.MER_PO='{po}'");

        //            if (string.IsNullOrWhiteSpace(po_num))
        //            {
        //                ret.IsSuccess = false;
        //                ret.ErrMsg = "PO quantity is not maintained!";
        //                return ret;
        //            }

        //            //制令级别
        //            string order_level = string.Empty;
        //            if (lot_nums.Count > 1)
        //                order_level = "0";
        //            else if (lot_nums.Count <= 1)
        //                order_level = "1";
        //            else
        //                order_level = "";

        //            sql = $@"insert into aql_cma_task_list_m (inspection_date,task_no,po,art_no,po_num,lot_num,order_level,full_state,inspection_state,
        //                task_type,createby,createdate,createtime) 
        //                values('','{task_no}','{po}','{art_no}','{po_num}','{lot_nums[i]}','{order_level}','0','0','1','{user}','{date}','{time}')";
        //            DB.ExecuteNonQuery(sql);

        //            #region 点箱
        //            var calDt = dt_copy.Select($@"SE_QTY>0");
        //            string sqlbox = string.Empty;
        //            decimal valfp = 0;//分批数量
        //            foreach (DataRow item in calDt)
        //            {
        //                var findBaseSeQtyRows = dt.Select($@"CR_SIZE='{item["CR_SIZE"]}'");
        //                decimal baseSeQty = Convert.ToDecimal(findBaseSeQtyRows[0]["SE_QTY"]);
        //                decimal currSeQty = Convert.ToDecimal(item["SE_QTY"]);
        //                //Only one task logic
        //                if (lot_nums.Count == 1)
        //                {
        //                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
        //                    values('{task_no}','{item["CR_SIZE"]}','{currSeQty}','{user}','{date}','{time}');";
        //                    valfp += currSeQty;
        //                }
        //                //Logic for more than one task and not the last task
        //                if (i < lot_nums.Count - 1 && lot_nums.Count != 1)
        //                {
        //                    decimal se_qty = Math.Floor((Convert.ToDecimal(lot_nums[i]) / Convert.ToDecimal(po_num)) * baseSeQty);
        //                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
        //                    values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}');";
        //                    valfp += se_qty;
        //                    item["SE_QTY"] = currSeQty - se_qty;
        //                }
        //                //More than one task and the logic of the last task
        //                if (i == lot_nums.Count - 1 && lot_nums.Count != 1)
        //                {
        //                    decimal se_qty = currSeQty;

        //                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
        //                    values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}');";
        //                    valfp += se_qty;
        //                }
        //            }
        //            //Modify the actual even number
        //            DB.ExecuteNonQuery($@"update aql_cma_task_list_m set lot_num='{valfp}' where task_no='{task_no}'");
        //            if (!string.IsNullOrWhiteSpace(sqlbox))
        //                DB.ExecuteNonQuery($@"BEGIN {sqlbox} END;");
        //            #endregion
        //        }


        //        DB.Commit();
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}
        #endregion

        #region After Po Change2 on 2025/02/13
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertCmaTask_TaskList_Insert(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//查询条件 po
                List<string> lot_nums = jarr.ContainsKey("lot_nums") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["lot_nums"].ToString()) : new List<string>();//查询条件 分批数量
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string task_no = string.Empty;//任务编号

                int countpo = DB.GetInt32($@"select count(1) from BDM_SE_ORDER_MASTER where MER_PO='{po}'");
                if (countpo <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "PO number does not exist!";//PO号不存在
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m where po='{po}'");
                if (count > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "PO number cannot be created repeatedly!";
                    return ret;
                }

                string merge_sql = $@"select so_mergr_mark from bdm_se_order_master where mer_po = '{po}'";
                string merge_mark = DB.GetString(merge_sql);
                DataTable dt = null;


                if (!string.IsNullOrEmpty(merge_mark) && merge_mark.Equals("Y"))//If it is a combined order, execute the logic of combining orders
                {
                    // First find out the code details of this po
                    dt = DB.GetDataTable($@"select
                                                        bb.cr_size as CR_SIZE,
                                                        bb.po_line_item ,
                                                        max(bb.se_qty) as SE_QTY
                                                      from
                                                        (
                                                        select
                                                          s.po_size as cr_size,
                                                          s.po_line_qty as se_qty,
                                                            s.po_line_item
                                                        from
                                                          bdm_se_order_master m
                                                          inner join t_bdm_se_order_reference s on  m.se_id = s.se_id 
                                                        where
                                                          m.mer_po = '{po}' 
                                                          and nvl(s.is_delete,'N') != 'Y' 
                                                        ) bb 
                                                      group by bb.cr_size ,bb.po_line_item
                                                      order by  max(bb.po_line_item)
                                                      ");
                }
                else
                {
                    // First find out the code details of this po
                    dt = DB.GetDataTable($@"SELECT
	                                            bb.CR_SIZE,
	                                            SUM( bb.SE_QTY ) SE_QTY 
                                            FROM
                                             (
                                             SELECT
                                              s.SIZE_NO as CR_SIZE,
                                              s.SE_QTY,
                                               s.SIZE_SEQ
                                             FROM
                                              BDM_SE_ORDER_MASTER m
                                              INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
                                              AND m.SE_ID = s.SE_ID 
                                             WHERE
                                              m.MER_PO = '{po}' 
                                             ) bb 
                                            GROUP BY bb.CR_SIZE 
                                            ORDER BY  MAX(bb.SIZE_SEQ)
                                            ");
                }
                    DataTable dt_copy = dt.Copy();

                    for (int i = 0; i < lot_nums.Count; i++)
                    {

                        task_no = po + "-" + (i + 1).ToString();

                        //art
                        string art_no = DB.GetString($@"select b.PROD_NO from BDM_SE_ORDER_MASTER a left join BDM_SE_ORDER_ITEM b on a.SE_ID=b.SE_ID where a.MER_PO='{po}'");

                        //PO数量
                        string po_num = DB.GetString($@"select b.SE_QTY from BDM_SE_ORDER_MASTER a left join BDM_SE_ORDER_ITEM b on a.SE_ID=b.SE_ID where a.MER_PO='{po}'");

                        if (string.IsNullOrWhiteSpace(po_num))
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "PO quantity is not maintained!";
                            return ret;
                        }

                        //制令级别
                        string order_level = string.Empty;
                        if (lot_nums.Count > 1)
                            order_level = "0";
                        else if (lot_nums.Count <= 1)
                            order_level = "1";
                        else
                            order_level = "";

                        sql = $@"insert into aql_cma_task_list_m (inspection_date,task_no,po,art_no,po_num,lot_num,order_level,full_state,inspection_state,
                        task_type,createby,createdate,createtime) 
                        values('','{task_no}','{po}','{art_no}','{po_num}','{lot_nums[i]}','{order_level}','0','0','1','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                       //Added by Ashok on 2026/01/28 to check all required data upload status before AQL final submit
                        sql = $@"insert into aql_pivot_data_status(task_no,created_by,created_at)values( '{task_no}', '{user}',sysdate)";
                        DB.ExecuteNonQuery(sql);



                    #region Point Box
                    var calDt = dt_copy.Select($@"SE_QTY>0");
                        string sqlbox = string.Empty;
                        decimal valfp = 0;//分批数量
                        foreach (DataRow item in calDt)
                        {

                            var findBaseSeQtyRows = dt.Select($@"CR_SIZE='{item["CR_SIZE"]}'");
                            decimal baseSeQty = Convert.ToDecimal(findBaseSeQtyRows[0]["SE_QTY"]);
                            decimal currSeQty = Convert.ToDecimal(item["SE_QTY"]);
                            if (merge_mark.Equals("Y"))
                            {
                                //Only one task logic
                                if (lot_nums.Count == 1)
                                {
                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime,po_line_item) 
                            values('{task_no}','{item["CR_SIZE"]}','{currSeQty}','{user}','{date}','{time}','{item["po_line_item"]}');";
                                    valfp += currSeQty;
                                }
                                //Logic for more than one task and not the last task
                                if (i < lot_nums.Count - 1 && lot_nums.Count != 1)
                                {
                                    decimal se_qty = Math.Floor((Convert.ToDecimal(lot_nums[i]) / Convert.ToDecimal(po_num)) * baseSeQty);
                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime,po_line_item) 
                            values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}','{item["po_line_item"]}');";
                                    valfp += se_qty;
                                    item["SE_QTY"] = currSeQty - se_qty;
                                }
                                //The logic of more than one task and the last task
                                if (i == lot_nums.Count - 1 && lot_nums.Count != 1)
                                {
                                    decimal se_qty = currSeQty;

                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime,po_line_item) 
                            values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}','{item["po_line_item"]}');";
                                    valfp += se_qty;
                                }
                            }
                            else
                            {

                                //Only one task logic
                                if (lot_nums.Count == 1)
                                {
                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
                            values('{task_no}','{item["CR_SIZE"]}','{currSeQty}','{user}','{date}','{time}');";
                                    valfp += currSeQty;
                                }
                                //Logic for more than one task and not the last task
                                if (i < lot_nums.Count - 1 && lot_nums.Count != 1)
                                {
                                    decimal se_qty = Math.Floor((Convert.ToDecimal(lot_nums[i]) / Convert.ToDecimal(po_num)) * baseSeQty);
                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
                            values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}');";
                                    valfp += se_qty;
                                    item["SE_QTY"] = currSeQty - se_qty;
                                }
                                //More than one task and the logic of the last task
                                if (i == lot_nums.Count - 1 && lot_nums.Count != 1)
                                {
                                    decimal se_qty = currSeQty;

                                    sqlbox += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
                            values('{task_no}','{item["CR_SIZE"]}','{se_qty}','{user}','{date}','{time}');";
                                    valfp += se_qty;
                                }
                            }
                        }
                        //Modify the actual even number
                        DB.ExecuteNonQuery($@"update aql_cma_task_list_m set lot_num='{valfp}' where task_no='{task_no}'");
                        if (!string.IsNullOrWhiteSpace(sqlbox))
                            DB.ExecuteNonQuery($@"BEGIN {sqlbox} END;");
                        #endregion
                    }


                    DB.Commit();
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
        #endregion

        /// <summary>
        /// 删除-AQL验货任务-清除数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DelelteCmaTask_TaskList_All(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//查询条件 po
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                #region 校验是否有权限清除数据
                string getPHSql = $@"SELECT aql_clear_autograph FROM AQL_SIGNING_AUTOGRAPH_SETTING_M WHERE STAFF_NO='{user}'";
                var phRes = DB.GetString(getPHSql);
                phRes = string.IsNullOrEmpty(phRes) ? "" : phRes;
                if (phRes != "1")
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "您无权限清除数据";
                    DB.Close();
                    return ret;
                }
                #endregion

                #region 核对资料
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_cd where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_cd_r where task_no='{task_no}'");
                #endregion

                #region 拍照
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_pg_d where task_no='{task_no}'");
                #endregion

                #region AQL录入
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_aql_e_br where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_aql_e_br_f where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_aql_m where task_no='{task_no}'");
                #endregion

                #region 湿度录入
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_h where task_no='{task_no}'");
                #endregion

                #region BA录入
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_ba where task_no='{task_no}'");
                #endregion

                #region 点箱
                DB.ExecuteNonQuery($@"update aql_cma_task_list_m_pb set CASE_NO='' where task_no='{task_no}'");
                #endregion
                //清除编辑状态
                DB.ExecuteNonQuery($@"update aql_cma_task_list_m set inspection_date = '', inspection_state = '0',f_inspection_time = '',PB_STATE='0',BA_EDIT_STATE='0',H_EDIT_STATE='0',AQL_EDIT_STATE='0',PH_EDIT_STATE='0',CHECKER='',IS_INSPECTION='0',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'");

                DB.Commit();
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
        /// 查询-制令分界设置-分界数量
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_ParamValue(object OBJ)
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
                string sql = string.Empty;

                sql = $@"select param_value from aql_cma_task_list_s";

                string param_value = DB.GetString(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("param_value", param_value);
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
        /// 编辑-制令分界设置-分界数量
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditCmaTask_TaskList_ParamValue(object OBJ)
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
                string param_value = jarr.ContainsKey("param_value") ? jarr["param_value"].ToString() : "";//查询条件 制令数   
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_s");
                if (count <= 0)
                {
                    DB.ExecuteNonQuery($@"insert into aql_cma_task_list_s (param_type,param_value,createby,createdate,createtime) 
                                            values('TaskSplitCount','{param_value}','{user}','{date}','{time}') ");
                }
                else
                {
                    DB.ExecuteNonQuery($@"update aql_cma_task_list_s set param_value='{param_value}',modifyby='{user}',modifydate='{date}',
                                        modifytime='{time}'");
                }

                DB.Commit();
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
        /// 查询-选择AQL验货任务-PO数量
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_Insert_ponum(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//查询条件 po
                string sql = string.Empty;

                //PO数量
                string po_num = DB.GetString($@"
SELECT
	SUM(b.SE_QTY) AS SE_QTY
FROM
	BDM_SE_ORDER_MASTER A
INNER JOIN BDM_SE_ORDER_SIZE b ON A .SE_ID = b.SE_ID AND a.ORG_ID=b.ORG_ID

where a.MER_PO='{po}'");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("PO_NUM", po_num);
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
        /// 查询PO列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
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
                //转译
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件 po
                string sql = string.Empty;
                string Where = string.Empty;

                
                Dictionary<string, object> dic = new Dictionary<string, object>();
                

                if (!string.IsNullOrEmpty(keyword)) {

                    Where += $@"and ( a.MER_PO like @keyword or b.PROD_NO like @keyword or c.name_t like @keyword)";
                    dic.Add("keyword",$@"%{keyword}%" );

                }

                sql = $@"
SELECT
ROWNUM as RN,
a.MER_PO,-- PO
b.SE_QTY, -- 订单数量
b.PROD_NO,--art
c.name_t
FROM
BDM_SE_ORDER_MASTER a
	LEFT JOIN BDM_SE_ORDER_ITEM b ON a.SE_ID = b.SE_ID AND a.ORG_ID= b.ORG_ID
	LEFT JOIN BDM_RD_STYLE c ON b.shoe_no = c.shoe_no
where 1=1 and b.SE_QTY >0 {Where}";

                DataTable dt = DB.GetDataTable(sql,dic);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("data", dt);

                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "查无数据！";
                    ret.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDatalist(object OBJ)
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
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";//查询条件 po

                string Where = string.Empty;

                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {

                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "No data found！";
                    ret.IsSuccess = false;
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEnum(object OBJ)
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
                string ENUM_CODE = jarr.ContainsKey("ENUM_CODE") ? jarr["ENUM_CODE"].ToString() : "";//查询条件 po
                string ENUM_TYPE = jarr.ContainsKey("ENUM_TYPE") ? jarr["ENUM_TYPE"].ToString() : "";//查询条件 po
                string LANGUAGE = jarr.ContainsKey("LANGUAGE") ? jarr["LANGUAGE"].ToString() : "";//查询条件 po

                string Where = string.Empty;
                string sql = $@"SELECT {LANGUAGE} as ENUM_VALUE,UI_EN,UI_YN,UI_CN FROM sys001m where ENUM_TYPE = '{ENUM_TYPE}' and ENUM_CODE ='{ENUM_CODE}'";

                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {

                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "查无数据！";
                    ret.IsSuccess = false;
                }

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
