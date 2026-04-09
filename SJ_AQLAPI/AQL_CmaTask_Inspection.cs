using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_CmaTask_Inspection
    {


        /// <summary>
        /// 查询SE_ID出货状态 验货室 界面
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_TaskList_Main_CHZT_I(object OBJ)
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
                        if (column2 != se_qty && se_qty == 0)
                        {
                            chzt = "Cancel_the_Order";//订单取消
                            if (!seidRes.ContainsKey(item.Key))
                            {
                                seidRes.Add(item.Key, chzt);
                            }
                            continue;
                        }

                        var find_WMS_SALES_ORDER_REPLACE = WMS_SALES_ORDER_REPLACE_dt.Select($@"SE_ID='{item.Key}'");
                        if (find_WMS_SALES_ORDER_REPLACE.Length > 0)
                        {
                            chzt = "Order_Replacement";
                            if (!seidRes.ContainsKey(item.Key))
                            {
                                seidRes.Add(item.Key, chzt);
                            }
                            continue;
                        }

                        if (column2 > se_qty)
                        {
                            if (qty > shipping_qty && qty > se_qty)
                            {
                                chzt = "Order_Reduction";//订单减少
                            }
                            else if ((se_qty >= qty && qty > shipping_qty) || (se_qty > qty && qty >= shipping_qty))
                            {

                                chzt = "Partial_Shipment";//分批出货
                            }
                            else if (qty == se_qty && shipping_qty == se_qty)
                            {
                                chzt = "Shipped";//已出货
                            }
                            else if (qty > 0 && qty <= se_qty && shipping_qty == 0)
                            {
                                chzt = "Not_Shipped";//未出货
                            }
                        }

                        if (column2 == se_qty)
                        {
                            if (shipping_qty > 0 && shipping_qty == column2)
                            {
                                chzt = "Shipped";//已出货
                            }
                            else if (shipping_qty == 0)
                            {
                                chzt = "Not_Shipped";//未出货
                            }
                            else
                            {
                                chzt = "Partial_Shipment";//分批出货
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
        /// 获取验货室订单查询 签名 按钮权限
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAutographSetting(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = $@"SELECT FACTORY_AUTOGRAPH,CUSTOMER_AUTOGRAPH FROM AQL_SIGNING_AUTOGRAPH_SETTING_M WHERE STAFF_NO='{userCode}'";
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
        /// 查询-验货室订单查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCmaTask_Inspection_Main(object OBJ)
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

                string sql = string.Empty ;
                sql = $@"SELECT 
                        to_char(a.INSPECTION_DATE,'yyyy-MM-dd') as INSPECTION_DATE,
                        a.IS_INSPECTION,
                        a.id,
                        a.task_no,
                        cx.from_line as sccq,
                        a.po,
                        a.art_no,
                        r.name_t as art_name,
                        r.SHOE_NO,
                        d.name_t as shoe_name,
(
CASE
	WHEN PG_WMS.GF_GET_FULL_RATE (pm.SE_ID) != '100%' THEN 'Not_full_box'--未满箱
    WHEN PG_WMS.GF_GET_FULL_RATE (pm.SE_ID) = '100%' THEN 'Full_box'--已满箱
	ELSE ''
END
) as full_state,-- 满箱状态

                        '' as bq,
                        pm.SE_CUSTID as kh,
                        e.c_name as guojia,
                        pi.COLUMN1 as vas,
                        a.po_num,
                        a.lot_num,
                        a.order_level,
                        -- a.full_state,
                        a.inspection_state,
                        '' as chzt,
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
                        '' as warning,
                        c.sample_level,
                        c.aql_level,
                        pi.column2,-- 订单数量
                        pi.se_qty,-- 订单有效数量
                        pm.SE_ID,
                        pm.ORG_ID,
                        a.BA_EDIT_STATE,
                        a.H_EDIT_STATE,
                        a.AQL_EDIT_STATE,
                        a.PH_EDIT_STATE,
                        d.style_seq as rule_no,
                        -- (select MAX(bb.name_t) from bdm_cd_code bb where bb.code_no=d.style_seq) as rule_no,
                        pm.SE_ID as from_line,
                        -- (select listagg(distinct from_line,',') from mms_finishedtrackin_list where se_id=pm.SE_ID) as from_line,
                        hr.staff_name as CHECKER
                        FROM
                        aql_cma_task_list_m a
                        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO 
                        LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no=a.task_no 
                        LEFT JOIN BDM_SE_ORDER_MASTER pm ON pm.MER_PO=a.po 
                        LEFT JOIN BDM_COUNTRY e on pm.descountry_code = e.c_no and e.l_no='EN'
                        LEFT JOIN BDM_SE_ORDER_ITEM pi ON pi.ORG_ID=pm.ORG_ID and pi.SE_ID=pm.SE_ID 
                        left join hr001m hr on hr.staff_no = a.CHECKER 
                        left join hr001m fhr on fhr.staff_no = a.factory_autograph 
                        left join hr001m chr on chr.staff_no = a.customer_autograph 
left join (
select 
    a.po,
    listagg(DISTINCT a.from_line, ',') WITHIN GROUP (ORDER BY a.from_line) as from_line
FROM
	mms_finishedtrackin_list a 
group by a.po
) cx on cx.po=a.po 
                        where 1=1 {where}";
                DataTable dt = new DataTable();
                if (string.IsNullOrEmpty(chzt))
                    dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                else
                    dt = DB.GetDataTable(sql, paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                List<string> poList = new List<string>();
                List<string> tasknoList = new List<string>();
                List<string> seidList = new List<string>();
                List<string> style_seqList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    poList.Add($@"'{item["po"]}'");
                    tasknoList.Add($@"'{item["task_no"]}'");
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
                //                            item["chzt"] = "订单替换";
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
                //                                item["chzt"] = "已出货";
                //                                continue;
                //                            }
                //                            else if (shipping_qty == 0)
                //                            {
                //                                item["chzt"] = "未出货";
                //                                continue;
                //                            }
                //                            else
                //                            {
                //                                item["chzt"] = "分批出货";
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
                //                                item["chzt"] = "订单减少";
                //                                continue;
                //                            }
                //                            else if ((se_qty >= qty && qty > shipping_qty) || (se_qty > qty && qty >= shipping_qty))
                //                            {

                //                                item["chzt"] = "分批出货";
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
                //                            item["chzt"] = "订单取消";
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

                #region 警告列查询
                //                if (poList.Count > 0)
                //                {
                //                    string warningColSql = $@"
                //                                            SELECT
                //                                                PO,
                //	                                            MAX(PO_NUM) AS PO_NUM,
                //	                                            SUM(LOT_NUM) AS LOT_NUM_SUM
                //                                            FROM
                //	                                            aql_cma_task_list_m
                //                                            WHERE
                //	                                            PO IN ({string.Join(",", poList)}) and effective_status = '0'
                //                                            GROUP BY PO
                //                                            ";
                //                    var poWarnDt = DB.GetDataTable(warningColSql);
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
                //                            //if (findPo.Length > 0)
                //                            //    item["sccq"] = findPo[0]["from_line"];
                //                            var find = poWarnDt.Select($@"PO='{item_po}'");
                //                            if (find.Length > 0)
                //                            {
                //                                decimal PO_NUM = Convert.ToDecimal(find[0]["PO_NUM"]);
                //                                decimal LOT_NUM_SUM = Convert.ToDecimal(find[0]["LOT_NUM_SUM"]);
                //                                if (PO_NUM != LOT_NUM_SUM)
                //                                    item["warning"] = "订单数据不正确";
                //                            }
                //                        }
                //                    }
                //                }
                #endregion

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
                                        item["inspection_results"] = "Rejected";
                                    else
                                        item["inspection_results"] = "Accepted";
                                }
                            }

                        }
                    }
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();

                //DataTable dtt = dt.Clone();
                //DataRow[] dr = new DataRow[] { };
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
        /// 新增-AQL验货任务-签名
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EidtCmaTask_TaskList_Signature(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBTest = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string autograph_state = jarr.ContainsKey("autograph_state") ? jarr["autograph_state"].ToString() : "";//签名类型 0:工厂代表签名 1:客户签名
                string account = jarr.ContainsKey("account") ? jarr["account"].ToString() : "";//查询条件 账号
                string pwd = jarr.ContainsKey("pwd") ? jarr["pwd"].ToString() : "";//查询条件 密码
                DataTable cma_yhs = jarr.ContainsKey("cma_yhs") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["cma_yhs"].ToString()) : null;//查询条件 验货室数据
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                //判断账号是否存在
                int zhCount = DBTest.GetInt32($@"select count(1) from SYSUSER01M where UserCode='{account}'");
                if (zhCount<=0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg="账号不存在!";
                    return ret;
                }

                //判断账号密码是否正确
                int ZhPwdCount = DBTest.GetInt32($@"select count(1) from " + DB.ChangeKeyWord("SYSUSER01M") + @" where (lower(UserCode) = '" + account.ToLower() + "' and UserPwd = '" + pwd + "') or (lower(UserCode) = '" + account.ToLower() + "' and UserPwd = '" + pwd.ToLower() + "')");
                if (ZhPwdCount <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "密码不正确!";
                    return ret;
                }

                switch (autograph_state)
                {
                    case "0"://工厂代表签名
                        foreach (DataRow item in cma_yhs.Rows)
                        {
                            if (item["xz"].ToString()== "True")
                            {
                                sql += $@"update aql_cma_task_list_m set factory_autograph='{account}',FACTORY_AUTOGRAPH_DATE='{DateTime.Now:yyyy-MM-dd}',modifyby='{user}',modifydate='{date}',
                                        modifytime='{time}' where id='{item["aid"]}';";
                            }
                        }
                        break;
                    case "1"://客户签名
                        foreach (DataRow item in cma_yhs.Rows)
                        {
                            if (item["xz"].ToString() == "True")
                            {
                                sql += $@"update aql_cma_task_list_m set customer_autograph='{account}',CUSTOMER_AUTOGRAPH_DATE='{DateTime.Now:yyyy-MM-dd}',modifyby='{user}',modifydate='{date}',
                                        modifytime='{time}' where id='{item["aid"]}';";
                            }
                        }
                        break;
                    default:
                        break;
                }

                DB.ExecuteNonQuery($@"BEGIN {sql} END;");

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
    }
}
