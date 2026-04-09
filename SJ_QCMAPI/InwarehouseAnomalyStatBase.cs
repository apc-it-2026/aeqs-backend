using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class InwarehouseAnomalyStatBase
    {
        /// <summary>
        /// 查询进仓异常材料统计接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetInwarehouseAnomalyStatList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 窗口传的参数
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";
                string ITEM_TYPE_NAME = jarr.ContainsKey("ITEM_TYPE_NAME") ? jarr["ITEM_TYPE_NAME"].ToString() : "";
                string Reject_ratioD = jarr.ContainsKey("Reject_ratioD") ? jarr["Reject_ratioD"].ToString() : "";
                string Reject_ratioS = jarr.ContainsKey("Reject_ratioS") ? jarr["Reject_ratioS"].ToString() : "";
                string acceptableD = jarr.ContainsKey("acceptableD") ? jarr["acceptableD"].ToString() : "";
                string acceptableS = jarr.ContainsKey("acceptableS") ? jarr["acceptableS"].ToString() : "";
                string Special_miningBLD = jarr.ContainsKey("Special_miningBLD") ? jarr["Special_miningBLD"].ToString() : "";
                string Special_miningBLS = jarr.ContainsKey("Special_miningBLS") ? jarr["Special_miningBLS"].ToString() : "";
                string Emergency_releaseBLD = jarr.ContainsKey("Emergency_releaseBLD") ? jarr["Emergency_releaseBLD"].ToString() : "";
                string Emergency_releaseBLS = jarr.ContainsKey("Emergency_releaseBLS") ? jarr["Emergency_releaseBLS"].ToString() : "";
                #endregion

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;
                #region 条件
                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(SUPPLIERS_NAME))
                {
                    whereSql += $@"and SUPPLIERS_NAME like'%" + SUPPLIERS_NAME + "%'";
                }
                if (!string.IsNullOrEmpty(ITEM_TYPE_NAME))
                {
                    whereSql += $@"and ITEM_TYPE_NAME like'%" + ITEM_TYPE_NAME + "%'";
                }
                if (!string.IsNullOrEmpty(Reject_ratioD) && !string.IsNullOrEmpty(Reject_ratioS))
                {
                    whereSql += $@"and TH>='{Reject_ratioD}' and TH<='{Reject_ratioS}'";
                }
                if (!string.IsNullOrEmpty(acceptableD) && !string.IsNullOrEmpty(acceptableS))
                {
                    whereSql += $@"and acceptable>='{acceptableD}' AND acceptable<='{acceptableS}'";
                }
                if (!string.IsNullOrEmpty(Special_miningBLD) && !string.IsNullOrEmpty(Special_miningBLS))
                {
                    whereSql += $@"and Special_miningBL>='{Special_miningBLD}' AND Special_miningBL<='{Special_miningBLS}'";
                }
                if (!string.IsNullOrEmpty(Emergency_releaseBLD) && !string.IsNullOrEmpty(Emergency_releaseBLS))
                {
                    whereSql += $@"and Emergency_releaseBL>='{Emergency_releaseBLD}' AND Emergency_releaseBL<='{Emergency_releaseBLS}'";
                }
                #endregion

                sql = $@"
	select * from (
	SELECT
	SUPPLIERS_NAME,
	ITEM_TYPE_NAME,
	sum( num ) COUNT,
	( SELECT COUNT( * ) FROM qcm_climaorder_m WHERE CHK_NO = M.CHK_NO ) AS Bad_batch,
	ROUND( ( ( SELECT COUNT( * ) FROM qcm_climaorder_m WHERE CHK_NO = M.CHK_NO ) / sum( num ) )*100, 2 ) AS Reject_ratio ,
	COUNT(DOC_NO) as PhysicalProperties,
	(select count(1) from qcm_rm_inspection_d q where q.chk_no=M.CHK_NO and q.ITEM_NO=M.ITEM_NO and q.aptestitem_name='颜色') as ys,
	(select count(1) from qcm_rm_inspection_d q where q.chk_no=M.CHK_NO and q.ITEM_NO=M.ITEM_NO and q.aptestitem_name='规格') as gg,
	(select count(1) from qcm_rm_inspection_d q where q.chk_no=M.CHK_NO and q.ITEM_NO=M.ITEM_NO and q.aptestitem_name='材质不良') as czbl,
  (select count(1) from qcm_rm_inspection_d q where q.chk_no=M.CHK_NO and q.ITEM_NO=M.ITEM_NO and q.aptestitem_name NOT in ('颜色','规格','材质不良')) as qt,
	( SELECT COUNT( * ) FROM qcm_climaorder_m WHERE CHK_NO = M.CHK_NO and  status=2)  as SBad_batch,
	ROUND( ( ( SELECT COUNT( * ) FROM qcm_climaorder_m WHERE CHK_NO = M.CHK_NO and  status=2 ) / sum( num ) )*100, 2 ) AS TH ,
	(100-ROUND( ( ( SELECT COUNT( * ) FROM qcm_climaorder_m WHERE CHK_NO = M.CHK_NO and  status=2 ) / sum( num ) )*100, 2 )) AS acceptable,
	1 as ranking,
	0 as Special_mining,
	0 as Special_miningBL,
	0 as Emergency_release,
	0 as Emergency_releaseBL
FROM
	(
	SELECT
		m.VEND_NO,
		m2.SUPPLIERS_NAME AS SUPPLIERS_NAME,
		d.CHK_NO AS CHK_NO,
		d.ITEM_NO AS ITEM_NO,
		m4.ITEM_TYPE_NAME AS ITEM_TYPE_NAME,
		1 num ,
		m5.DOC_NO,
		q.aptestitem_name
	FROM
		wms_rcpt_d d
		LEFT JOIN wms_rcpt_m m ON d.CHK_NO = m.CHK_NO
		JOIN BASE003M m2 ON m2.SUPPLIERS_CODE = m.VEND_NO
		LEFT JOIN BDM_RD_ITEM m3 ON m3.ITEM_NO = d.ITEM_NO
		LEFT JOIN BDM_RD_ITEMTYPE m4 ON m4.ITEM_TYPE_NO = m3.ITEM_TYPE 
		LEFT JOIN qcm_inspection_laboratory_m m5 ON	m5.doc_no=d.CHK_NO AND m5.material_no=d.ITEM_NO
		LEFT JOIN qcm_rm_inspection_d q ON q.chk_no=d.CHK_NO and q.ITEM_NO=d.ITEM_NO
	) M 
GROUP BY
	SUPPLIERS_NAME,
	ITEM_TYPE_NAME,
	CHK_NO,
	ITEM_NO,
	DOC_NO,
aptestitem_name)
where 1=1
{whereSql}";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
