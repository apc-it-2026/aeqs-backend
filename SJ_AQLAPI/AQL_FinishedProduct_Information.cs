using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_FinishedProduct_Information
    {
        //成品仓信息
        /// <summary>
        /// 查询-成品仓信息——AQL任务清单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFinishedProduct_Information_Main(object OBJ)
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
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//po
                string radio = jarr.ContainsKey("radio") ? jarr["radio"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型
                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//进仓时间
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//进仓时间
                string sccq = jarr.ContainsKey("sccq") ? jarr["sccq"].ToString() : "";
                string zb = jarr.ContainsKey("zb") ? jarr["zb"].ToString() : "";
                string country = jarr.ContainsKey("country") ? jarr["country"].ToString() : "";
                string inspection_state = jarr.ContainsKey("inspection_state") ? jarr["inspection_state"].ToString() : "";//验货状态

                bool noFilter = Convert.ToBoolean(jarr["noFilter"].ToString());//是否无过滤条件

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(inspection_state))
                {
                    where += $@" and f.inspection_state='{inspection_state}'";
                }
                if (!string.IsNullOrWhiteSpace(sccq))
                {
                    where += " and pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) like @sccq";
                    paramTestDic.Add("sccq", $@"%{sccq}%");
                }
                if (!string.IsNullOrWhiteSpace(zb))
                {
                    where += " and pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) like @zb";
                    paramTestDic.Add("zb", $@"%{zb}%");
                }
                if (!string.IsNullOrWhiteSpace(mer_po))
                {
                    where += " and a.SE_ID IN (SELECT SE_ID FROM bdm_se_order_master  WHERE MER_PO = @mer_po)";
                    paramTestDic.Add("mer_po", $@"{mer_po}");
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += " and A .PROD_NO like @prod_no";
                    paramTestDic.Add("prod_no", $@"%{prod_no}%");
                }
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += " and GF_PROD_NAME_MG (A .org_id, A .PROD_NO, 'T') like @shoe_name";
                    paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                }
                if (!string.IsNullOrEmpty(country))
                {
                    where += " and c.Shipcountry_En like @DESCOUNTRY_NAME";
                    paramTestDic.Add("DESCOUNTRY_NAME", $@"%{country}%");
                }
                if (!string.IsNullOrWhiteSpace(starttime) && !string.IsNullOrWhiteSpace(endtime))
                {
                    paramTestDic.Add("start_date", $@"{starttime}");
                    paramTestDic.Add("endtime", $@"{endtime}");
                    where += $@" AND TO_CHAR( b.insert_date ,'yyyy-MM-dd') >= @start_date AND TO_CHAR( b.last_date ,'yyyy-MM-dd') <=  @endtime ";
                }
                else if (!string.IsNullOrWhiteSpace(starttime))
                {
                    paramTestDic.Add("start_date", $@"{starttime}");
                    where += $@" AND TO_CHAR( b.insert_date ,'yyyy-MM-dd') >= @start_date ";
                }
                else if (!string.IsNullOrWhiteSpace(endtime))
                {
                    paramTestDic.Add("endtime", $@"{endtime}");
                    where += $@" AND TO_CHAR( b.last_date ,'yyyy-MM-dd') <= @endtime ";
                }
                if (radio == "true")
                {
                    where += $@" and bb.stoc_qty >0";
                }

                string limitSql = "";
                if (noFilter)
                {
                    limitSql = $@" and rownum<=500";
                    if (Convert.ToInt32(pageSize) > 500)
                        pageSize = "500";
                }
                string pageSql = $@" and rownum<={Convert.ToInt32(pageIndex) * Convert.ToInt32(pageSize)}";

                string sql = string.Empty;
                sql = $@"
SELECT
	b.org_id,
	A .SE_ID,
	c.mer_PO,
	SYSDATE, 
    A .PROD_NO,
	A .se_QTY,
	A .nst,
	b.insert_date,
	b.insert_time,
	b.last_date,
	b.stoc_no,
	pg_wms.GF_WMS_GET_LOCATION_QTY (b.org_id, b.stoc_no, A .SE_ID) AS location_qty,
	pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) AS fromline_qty,
	b.ctn_qty,
	b.stoc_pairs,
	A .lpd,
	-- A .cr_reqdate,
	GF_PROD_NAME_MG (A .org_id, A .PROD_NO, 'T') AS shoe_name,
	A .se_qty - b.stoc_pairs AS owe_pairs,
    A.CR_REQDATE, -- CRD
	-- e.POSTINGctn_qty_DATE, --实际出货日期,
(select MAX(POSTING_DATE) from bmd_se_shipment_m p LEFT JOIN bdm_se_order_item l ON p.se_id = l.se_id where l.SE_ID = A.SE_ID) as POSTING_DATE,
    c.Shipcountry_En as SHIPCOUNTRY_NAME, -- 国家
to_char(f.inspection_date,'yyyy-MM-dd') as inspection_date, -- 验货日期
 pg_wms.GF_GET_FINISHED_CARTON(a.se_id)-b.ctn_qty as se_ctn_qty, -- 所欠箱数
 pg_wms.GF_GET_FINISHED_CARTON(a.se_id) as owe_ctn_qty, -- 订单总箱数
(
	CASE
	WHEN PG_WMS.GF_GET_FULL_RATE (B.SE_ID) != '100%' THEN
		''
	WHEN t .se_id IS NOT NULL THEN
		TO_CHAR (t.a_date, 'yyyy/MM/dd')
	ELSE
		TO_CHAR (b.last_date, 'yyyy/MM/dd')
	END
) AS a_date, -- 满箱日期
-- t.nail_pz, -- 测钉
(case when t.se_id is not null then t.nail_pz else 'Y' end) as nail_pz, -- 测钉
-- t.recheck_pz, -- 重新验货
(case when t.se_id is not null then t.recheck_pz else 'N' end) as recheck_pz, -- 重新验货
PG_WMS.GF_GET_FULL_RATE(a.SE_ID) as full_rate, -- 满箱率
f.inspection_state
FROM
	bdm_se_order_item A
LEFT JOIN bdm_se_order_master c ON A .se_id = c.se_id
LEFT JOIN bmd_se_shipment_m e ON e.se_id = A.se_id
LEFT JOIN aql_cma_task_list_m f ON f.po = c.MER_PO
LEFT JOIN (
	SELECT
		SUM (qty) AS stoc_pairs,
		MIN (insert_date) AS insert_date,
		COUNT (*) AS ctn_qty,
		MIN (insert_date) AS insert_time,
		MAX (insert_date) AS last_date,
		se_id,
		stoc_no,
		org_id
	FROM
		mms_finishedtrackin_list 
	GROUP BY
		org_id,
		stoc_no,
		se_id
) b ON A .se_id = b.se_id
LEFT JOIN WMS_KANBAN_INFORMATION  t ON b.se_ID = t.SE_ID and t.PO = c.mer_PO
LEFT JOIN vw_finish_stoc_qty_by_batch bb ON bb.batch_no = b.SE_ID and bb.org_id = b.org_id and bb.stoc_no=b.stoc_no
where 1=1 {where} ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql + pageSql + " order by TO_NUMBER(REPLACE(PG_WMS.GF_GET_FULL_RATE(a.SE_ID), '%', '')) desc", int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql + limitSql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();

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
        /// 查询-成品仓信息——验货室
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFinishedProduct_Information_Main_I(object OBJ)
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
                string inspection_state = jarr.ContainsKey("inspection_state") ? jarr["inspection_state"].ToString() : "";//验货状态
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//po
                string radio = jarr.ContainsKey("radio") ? jarr["radio"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型
                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//进仓时间
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//进仓时间
                string xsddzt = jarr.ContainsKey("xsddzt") ? jarr["xsddzt"].ToString() : "";//销售订单状态
                string zb = jarr.ContainsKey("zb") ? jarr["zb"].ToString() : "";//组别 
                string sccq = jarr.ContainsKey("sccq") ? jarr["sccq"].ToString() : "";
                string country = jarr.ContainsKey("country") ? jarr["country"].ToString() : "";

                bool noFilter = Convert.ToBoolean(jarr["noFilter"].ToString());//是否无过滤条件
                string pageSize_SelectedIndex = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;

                if (!string.IsNullOrWhiteSpace(inspection_state))
                {
                    where += $@" and f.inspection_state='{inspection_state}'";
                }
                if (!string.IsNullOrWhiteSpace(sccq))
                {
                    where += " and pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) like @sccq";
                    paramTestDic.Add("sccq", $@"%{sccq}%");
                }
                if (!string.IsNullOrWhiteSpace(zb))
                {
                    where += " and pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) like @zb";
                    paramTestDic.Add("zb", $@"%{zb}%");
                }


                if (!string.IsNullOrWhiteSpace(mer_po))
                {
                    where += " and a.SE_ID IN (SELECT SE_ID FROM bdm_se_order_master  WHERE MER_PO = @mer_po)";
                    paramTestDic.Add("mer_po", $@"{mer_po}");
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += " and A .PROD_NO like @prod_no";
                    paramTestDic.Add("prod_no", $@"%{prod_no}%");
                }
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += " and GF_PROD_NAME_MG (A .org_id, A .PROD_NO, 'T') like @shoe_name";
                    paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                }
                if (!string.IsNullOrWhiteSpace(starttime) && !string.IsNullOrWhiteSpace(endtime))
                {


                    paramTestDic.Add("start_date", $@"{starttime}");
                    paramTestDic.Add("endtime", $@"{endtime}");
                    where += $@" AND TO_CHAR(b.insert_date ,'yyyy-MM-dd') >= @start_date AND TO_CHAR( b.insert_date ,'yyyy-MM-dd') <= @endtime ";


                }
                else if (!string.IsNullOrWhiteSpace(starttime))
                {
                    paramTestDic.Add("start_date", $@"{starttime}");
                    where += $@" AND TO_CHAR( b.insert_date ,'yyyy-MM-dd') >= @start_date ";
                }
                else if (!string.IsNullOrWhiteSpace(endtime))
                {
                    paramTestDic.Add("endtime", $@"{endtime}");
                    where += $@" AND TO_CHAR( b.insert_date ,'yyyy-MM-dd') <= @endtime ";
                }
                if (radio == "true")
                {
                    where += $@" and bb.stoc_qty >0";
                }
                if (!string.IsNullOrEmpty(country))
                {
                    where += " and c.Shipcountry_En like @DESCOUNTRY_NAME";
                    paramTestDic.Add("DESCOUNTRY_NAME", $@"%{country}%");
                }
                if (!string.IsNullOrWhiteSpace(xsddzt))
                {
                    where += $@" and c.STATUS = '{xsddzt}'";
                    //where += " and c.STATUS = @xsddzt";
                    //paramTestDic.Add("xsddzt", $@"{xsddzt}");
                }
                //if (!string.IsNullOrWhiteSpace(zb))
                //{
                //    string fromline_list = string.Join(',', zb.Split(',').Select(x => $@"'{x}'"));
                //    where += $@" and A .SE_ID IN (SELECT DISTINCT SE_ID FROM MMS_FINISHEDTRACKIN_LIST WHERE FROM_LINE IN ({fromline_list}))";
                //}

                string limitSql = "";
                if (noFilter)
                {
                    limitSql = $@" and rownum<=500";
                    if (Convert.ToInt32(pageSize) > 500)
                        pageSize = "500";
                }
                string pageSql = $@" and rownum<={Convert.ToInt32(pageIndex) * Convert.ToInt32(pageSize)}";
                string sql = string.Empty;
                sql = $@"
SELECT
    A.column2,-- 订单数量
	b.org_id,
	A .SE_ID,
	c.mer_PO,
	SYSDATE, 
    A .PROD_NO,
	A .se_QTY,-- 订单有效数量
	A .nst,
	b.insert_date,
	b.insert_time,
	b.last_date,
	b.stoc_no,
	pg_wms.GF_WMS_GET_LOCATION_QTY (b.org_id, b.stoc_no, A .SE_ID) AS location_qty,
	pg_wms.GF_WMS_GET_WORKCENTER_QTY (b.org_id, b.stoc_no, A .SE_ID) AS fromline_qty,
	b.ctn_qty,
	b.stoc_pairs,
	A .lpd,
	-- A .cr_reqdate,
	GF_PROD_NAME_MG (A .org_id, A .PROD_NO, 'T') AS shoe_name,
	A .se_qty - b.stoc_pairs AS owe_pairs,
    A.CR_REQDATE, -- CRD
	-- e.POSTING_DATE, --实际出货日期,
(select MAX(POSTING_DATE) from bmd_se_shipment_m p LEFT JOIN bdm_se_order_item l ON p.se_id = l.se_id where l.SE_ID = A.SE_ID) as POSTING_DATE,
    c.Shipcountry_En as SHIPCOUNTRY_NAME, -- 国家
to_char(f.inspection_date,'yyyy-MM-dd') as inspection_date, -- 验货日期
 pg_wms.GF_GET_FINISHED_CARTON(a.se_id)-b.ctn_qty as se_ctn_qty, -- 所欠箱数
 pg_wms.GF_GET_FINISHED_CARTON(a.se_id) as owe_ctn_qty, -- 订单总箱数
(
	CASE
	WHEN PG_WMS.GF_GET_FULL_RATE (B.SE_ID) != '100%' THEN
		''
	WHEN t .se_id IS NOT NULL THEN
		TO_CHAR (t.a_date, 'yyyy/MM/dd')
	ELSE
		TO_CHAR (b.last_date, 'yyyy/MM/dd')
	END
) AS a_date, -- 满箱日期
-- t.nail_pz, -- 测钉
(case when t.se_id is not null then t.nail_pz else 'Y' end) as nail_pz, -- 测钉
-- t.recheck_pz, -- 重新验货
(case when t.se_id is not null then t.recheck_pz else 'N' end) as recheck_pz, -- 重新验货
PG_WMS.GF_GET_FULL_RATE(a.SE_ID) as full_rate, -- 满箱率
'' as chzt, -- 出货状态
c.STATUS,
f.inspection_state
FROM
	bdm_se_order_item A
LEFT JOIN bdm_se_order_master c ON A .se_id = c.se_id
LEFT JOIN bmd_se_shipment_m e ON e.se_id = A.se_id
LEFT JOIN aql_cma_task_list_m f ON f.po = c.MER_PO
LEFT JOIN (
	SELECT
		SUM (qty) AS stoc_pairs,
		MIN (insert_date) AS insert_date,
		COUNT (*) AS ctn_qty,
		MIN (insert_date) AS insert_time,
		MAX (insert_date) AS last_date,
		se_id,
		stoc_no,
		org_id
	FROM
		mms_finishedtrackin_list 
	GROUP BY
		org_id,
		stoc_no,
		se_id
) b ON A .se_id = b.se_id
LEFT JOIN WMS_KANBAN_INFORMATION  t ON b.se_ID = t.SE_ID and t.PO = c.mer_PO 
LEFT JOIN vw_finish_stoc_qty_by_batch bb ON bb.batch_no = b.SE_ID and bb.org_id = b.org_id and bb.stoc_no=b.stoc_no
where 1=1 {where} ";
                DataTable dt = null;
                string orderBySql = $@" order by TO_NUMBER(REPLACE(PG_WMS.GF_GET_FULL_RATE(a.SE_ID), '%', '')) desc";
                if (noFilter)
                {//无过滤条件，只查前500条
                    dt = DB.GetDataTable(sql + limitSql + orderBySql, paramTestDic);
                }
                else
                {//有过滤条件，全查
                    dt = DB.GetDataTable(sql+ orderBySql, paramTestDic);
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql + limitSql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //DataTable dtt = dt.Clone();
                //DataRow[] dr = new DataRow[] { };
                //if (string.IsNullOrEmpty(sccq) && string.IsNullOrEmpty(zb))
                //{
                //    dic.Add("Data", dt);
                //}
                //else
                //{
                //    if (!string.IsNullOrEmpty(sccq))
                //        dr = dt.Select($@"fromline_qty like '%{sccq}%' ");//or sccq like '%{zhubie}%'
                //    if (!string.IsNullOrEmpty(zb))
                //        dr = dt.Select($@" fromline_qty like '%{zb}%'");//or sccq like '%{zhubie}%'

                //    //dic.Add("Data", dr);

                //    foreach (DataRow row in dr)
                //    {
                //        dtt.ImportRow(row);
                //    }
                //    dic.Add("Data", dtt);
                //}


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
        /// 获取组别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFromLine(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                #endregion
                #region 逻辑

                var fromline_info = DB.GetDataTable($@"
SELECT DISTINCT FROM_LINE FROM MMS_FINISHEDTRACKIN_LIST");



                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("fromline_info", fromline_info);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
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
