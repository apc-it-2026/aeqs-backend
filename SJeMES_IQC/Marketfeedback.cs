using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJeMES_IQC
{
    /// <summary>
    /// 市场反馈
    /// </summary>
    public class Marketfeedback
    {
        /// <summary>
        /// 字典去null
        /// </summary>
        /// <param name="dic"></param>
        private static void NullKeyValue(Dictionary<string, object> dic)
        {
            string[] keys = dic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (dic[keys[i]] == null)
                {
                    dic[keys[i]] = "";
                }
            }
        }
        /*---------------------------------------------PC端-----------------------------------------------*/
        /// <summary>
        /// PC中国退货主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MianGetList(object OBJ)
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
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//art
                string name_t = jarr.ContainsKey("name_t") ? jarr["name_t"].ToString() : "";//鞋型
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(art))
                {
                    strwhere += $@" and r.prod_no like '%{art}%'";
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    strwhere += $@" and l.name_t like '%{name_t}%'";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.createdate between  '{putin_date}' and '{end_date}'";
                }
                string sql = $@"SELECT
   DISTINCT
    a.id,
	a.task_no,
	a.RETURN_MONTH,--退货月份
    -- a.DATETIME as times,--年份/月份
	'' as factory,--工厂名称
	a.region_no,--国家/地区代号
	m.shipcountry_name as region_name,--出货国家
	r.prod_no,--art
    a.po,--po
    p.name_t as style_seq,--Category
	l.name_t,--鞋型名称
	-- a.production_month,--生产月份
    (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份
	a.main_code,--主要问题代码
    a.minor_code,--次要问题代码
	b.content_cn as content_cn,--主要问题原因
    c.content_cn as content_cn2,--次要问题原因
	a.fob_price,--FOB单价
	(nvl(a.newshoes_qty,0)+nvl(a.oldshoes_qty,0)) as out_qty,--退货数量
	a.compensation_amount,--赔偿金额
	a.problem_point_desc,--问题点描述
	(a.main_code||a.minor_code) as codeincode,--合并代码
    a.status --审判状态
FROM
     qcm_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code and b.main_code=c.main_code
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
LEFT JOIN (select code_no,max(name_t) as name_t from bdm_cd_code group by code_no) p on l.style_seq=p.code_no
where 1=1 {strwhere} 
order by a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();

                if(dt!=null && dt.Rows.Count > 0)
                {
                    List<string> poList = new List<string>();
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(item["po"].ToString()))
                            poList.Add($@"'{item["po"]}'");
                    }
                    poList = poList.Distinct().ToList();

                    if (poList.Count > 0)
                    {
                        var factory_dt = DB.GetDataTable($@"
     SELECT
 	    stl.SE_ID,
 	    listagg(DISTINCT orgb.org_name, ',') WITHIN GROUP (ORDER BY orgb.org_name) as orgname
     FROM
         SFC_TRACKOUT_LIST stl
     LEFT JOIN BASE001M orgb ON orgb.org_code = stl.org_id 
			WHERE stl.SE_ID IN ({string.Join(",", poList)})
     GROUP BY stl.SE_ID
");
                        if (factory_dt != null && factory_dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                if (!string.IsNullOrEmpty(item["po"].ToString()))
                                {
                                    var findPo = factory_dt.Select($@"SE_ID='{item["po"]}'");
                                    if (findPo != null && findPo.Length > 0)
                                    {
                                        item["factory"] = findPo[0]["orgname"].ToString();
                                    }

                                }
                            }
                        }
                    }



                }

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
        /// PC客户退货主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject MianGetList2(object OBJ)
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
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//art
                string name_t = jarr.ContainsKey("name_t") ? jarr["name_t"].ToString() : "";//鞋型
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(art))
                {
                    strwhere += $@" and r.prod_no like '%{art}%'";
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    strwhere += $@" and l.name_t like '%{name_t}%'";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.createdate between  '{putin_date}' and '{end_date}'";
                }
                string sql = $@"SELECT
   DISTINCT
  a.id,
	a.task_no,
	'' as times,--年份/月份
	a.factory,--工厂名称
	a.region_no,--国家/地区代号
	'' as region_name,--国家/地区
	r.prod_no,--art
    a.po,--po
    p.name_t as style_seq,--Category
	l.name_t,--鞋型名称
	a.production_month,--生产月份
	a.main_code,--主要问题代码
    a.minor_code,--次要问题代码
	b.content_cn as content_cn,--主要问题原因
    c.content_cn as content_cn2,--次要问题原因
	a.fob_price,--FOB单价
	a.return_qty,--退货数量
	a.compensation_amount,--赔偿金额
	a.remark,--问题点描述
	(a.main_code||a.minor_code) as codeincode,--合并代码
    a.status --审判状态
   FROM
     qcm_cust_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code and b.main_code=c.main_code

LEFT JOIN  BDM_SE_ORDER_MASTER m on a.po=m.mer_po
	LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
  LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
	LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
	LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO 
    LEFT JOIN bdm_cd_code p on y.style_seq=p.code_no
where 1=1 {strwhere} order by a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
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
        /// 带出继续录入的数据(中国退货汇总)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commithisorypc(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                var sql = string.Empty;
                sql = $@"SELECT
    DISTINCT
	a.task_no,
	a.po,--po
	a.DATETIME,--年份月份
    a.region_no,--国家/地区代号
	'' as region_name, --国家/地区
    a.size_no,--码数
    a.newshoes_qty,--新鞋数量
	a.oldshoes_qty,--旧鞋数量
	l.name_t,--鞋型名称
    a.main_code,--主要问题代码
    a.minor_code,--次要问题代码
	b.content_cn as content_cn,--主要问题原因
    c.content_cn as content_cn2,--次要问题原因
    a.fob_price,--FOB单价
	a.compensation_amount,--赔偿金额
	a.problem_point_desc, --问题点描述
	a.RETURN_MONTH --退货月份
   FROM
     qcm_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code  and b.main_code=c.main_code
LEFT JOIN  BDM_SE_ORDER_MASTER m on a.po=m.mer_po
	LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
  LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 带出继续录入的数据（客户退货汇总）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commithisorypc2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                var sql = string.Empty;
                sql = $@"SELECT
    DISTINCT
	a.task_no,
	a.po,--po
   a.region_no,--国家/地区代号
	a.factory,--工厂
	a.production_month,--生产月份
	'' as region_name, --国家/地区
    a.return_qty, --退货数量
    a.main_code,--主要问题代码
    a.minor_code,--次要问题代码
		b.content_cn as content_cn,--主要问题原因
    c.content_cn as content_cn2,--次要问题原因
    a.fob_price,--FOB单价
	a.compensation_amount,--赔偿金额
	a.remark --备注
   FROM
     qcm_cust_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code  and b.main_code=c.main_code
  where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 带出下拉框(主要任务，国家代号)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getlistxlk(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                string sql = $@"SELECT
	main_code,
	content_cn
FROM
	bdm_r_main_d_code";
                DataTable dt = DB.GetDataTable(sql);
                sql = $@"select  
'' as region_no,
'' as region_name
from dual";
                DataTable dt1 = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 中国区客户退货审核
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Status(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//图片id
                string sql = $@"select status from qcm_market_feedback_m where task_no='{task_no}'";
                if (DB.GetString(sql) == "0")
                {
                    sql = $@"update qcm_market_feedback_m set status='1' where task_no='{task_no}'";
                    ret.ErrMsg = "Audit successful";
                }
                else
                {
                    sql = $@"update qcm_market_feedback_m set status='0' where task_no='{task_no}'";
                    ret.ErrMsg = "Successful cancellation of review";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 客户退货审核
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Status2(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//图片id
                string sql = $@"select status from qcm_cust_market_feedback_m where task_no='{task_no}'";
                if (DB.GetString(sql) == "0")
                {
                    sql = $@"update qcm_cust_market_feedback_m set status='1' where task_no='{task_no}'";
                    ret.ErrMsg = "Audit successful";
                }
                else
                {
                    sql = $@"update qcm_cust_market_feedback_m set status='0' where task_no='{task_no}'";
                    ret.ErrMsg = "Successful cancellation of review";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /*---------------------------------------------PDA端-----------------------------------------------*/
        /// <summary>
        /// PC主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PDA_MianGetList(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//关键字
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string sc_date_opan = jarr.ContainsKey("sc_date_opan") ? jarr["sc_date_opan"].ToString() : "";//生产日期（开始）
                string sc_date_end = jarr.ContainsKey("sc_date_end") ? jarr["sc_date_end"].ToString() : "";//生产日期（结束）
                string lr_date_opan = jarr.ContainsKey("lr_date_opan") ? jarr["lr_date_opan"].ToString() : "";//录入日期（开始）
                string lr_date_end = jarr.ContainsKey("lr_date_end") ? jarr["lr_date_end"].ToString() : "";//录入日期（结束）
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and (a.po like '%{keyword}%' or l.name_t like '%{keyword}%' or r.prod_no like '%{keyword}%' or y.style_seq like '%{keyword}%' or a.factory like '%{keyword}%' or b.content_cn like '%{keyword}%' or c.content_cn like '%{keyword}%')";
                }
                if (!string.IsNullOrWhiteSpace(sc_date_opan) && !string.IsNullOrWhiteSpace(sc_date_end))
                {
                    strwhere += $@" and a.production_month between  '{sc_date_opan}' and '{sc_date_end}'";
                }
                if (!string.IsNullOrWhiteSpace(lr_date_opan) && !string.IsNullOrWhiteSpace(lr_date_end))
                {
                    strwhere += $@" and a.createdate between  '{lr_date_opan}' and '{lr_date_end}'";
                }
                string sql = $@"SELECT
    DISTINCT
    a.id,
	(a.createdate ||' '|| a.createtime) as createdate,--录入时间
	a.task_no,--任务编号
	a.po,--po
	r.prod_no,--art
	l.name_t,--鞋型名称
	(a.main_code||a.minor_code) as codeincode,--合并代码
	(a.newshoes_qty+a.oldshoes_qty) as out_qty --退货数量


	-- '' as times,--年份/月份
	-- a.factory,--工厂名称
	-- a.region_no,--国家/地区代号
	-- '' as region_name,--国家/地区
    -- y.style_seq,--Category
	-- a.production_month,--生产月份
	-- a.main_code,--主要问题代码
    -- a.minor_code,--次要问题代码
	-- b.content_cn as content_cn,--主要问题原因
    -- c.content_cn as content_cn2,--次要问题原因
	-- a.fob_price,--FOB单价
	-- a.compensation_amount,--赔偿金额
	-- a.problem_point_desc --问题点描述
   FROM
     qcm_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code and b.main_code=c.main_code

LEFT JOIN  BDM_SE_ORDER_MASTER m on a.po=m.mer_po
	LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
  LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
	LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
	LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO where 1=1 {strwhere}  order by   a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("total", rowCount);
                ret.RetData1 = dic;
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
        /// 国家代号，国家名称
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getRegionList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                var sql = string.Empty;
                sql = $@"select  
'' as region_no,
'' as region_name
from dual";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// PO带出数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PoGetList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";
                var sql = string.Empty;
                sql = $@"
SELECT
	M.mer_po,--po号
	l.name_t, --鞋型
	r.prod_no,--art
	p.name_t as style_seq, -- category
    '' as factory,
    '' as production_month
FROM
	BDM_SE_ORDER_MASTER M
LEFT JOIN BDM_SE_ORDER_ITEM E ON M .SE_ID = E.SE_ID
LEFT JOIN bdm_rd_prod r ON E .prod_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l ON r.SHOE_NO = l.SHOE_NO
LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
LEFT JOIN bdm_cd_code p on y.style_seq=p.code_no
WHERE
	M.mer_po = '{mer_po}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count < 1)
                {
                    //ret.ErrMsg = "该po号为无效po，请检查";
                    ret.ErrMsg = "The po number is invalid po, please check";
                    ret.IsSuccess = false;
                    return ret;
                }
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 主要退货代码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCode1(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                NullKeyValue(jarr);
                string sql = $@"SELECT
	main_code,
	content_cn,
	content_en
FROM
	bdm_r_main_d_code";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 次要退货代码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCode2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                NullKeyValue(jarr);
                string main_code = jarr.ContainsKey("main_code") ? jarr["main_code"].ToString() : "";//主要不良问题代号
                string sql = $@"SELECT
	minor_code,
	content_cn,
	content_en
FROM
	bdm_r_minor_d_code where main_code='{main_code}'";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// PO继续录入带出数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commithisory(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                var sql = string.Empty;
                sql = $@"SELECT
DISTINCT
	a.task_no,
	a.po,--po
	l.name_t,--鞋型名称
	r.prod_no,--art
	p.name_t as style_seq,--Category
	a.production_month,--生产月份
	(a.newshoes_qty+a.oldshoes_qty) as out_qty,--退货数量
	a.size_no,--码数
	a.newshoes_qty,--新鞋数量
	a.oldshoes_qty,--旧鞋数量
	a.main_code,--主要问题代码
  a.minor_code,--次要问题代码
	b.content_cn as content_cn,--主要问题原因
  c.content_cn as content_cn2,--次要问题原因
	a.factory,--工厂名称
	a.fob_price,--FOB单价
	a.compensation_amount,--赔偿金额
	a.problem_point_desc,--问题点描述
	'' as times,--年份/月份
	a.region_no,--国家/地区代号
	'' as region_name--国家/地区
   FROM
     qcm_market_feedback_m a 
left join bdm_r_main_d_code b on a.main_code=b.main_code
left join bdm_r_minor_d_code c on a.minor_code=c.minor_code  and b.main_code=c.main_code

LEFT JOIN  BDM_SE_ORDER_MASTER m on a.po=m.mer_po
	LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
  LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
	LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
	LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
    LEFT JOIN bdm_cd_code p on y.style_seq=p.code_no
where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("file_list", Type.GetType("System.Object"));

                sql = $@"SELECT
	A.ID,
    A.TASK_NO,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	 qcm_market_feedback_m_f A
LEFT JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";
                DataTable dt_file = DB.GetDataTable(sql);
                DataRow[] dr = null;
                List<imginfo> file_list = new List<imginfo>();
                foreach (DataRow row in dt.Rows)
                {
                    file_list = new List<imginfo>();
                    dr = dt_file.Select($@"TASK_NO='{row["TASK_NO"]}'");
                    foreach (DataRow item in dr)
                    {
                        file_list.Add(new imginfo { file_url = item["FILE_URL"].ToString(), guid = item["GUID"].ToString() });
                    }
                    row["file_list"] = file_list;
                }
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 中国退货汇总主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Mian(object OBJ)
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
                NullKeyValue(jarr);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string region_no = jarr.ContainsKey("region_no") ? jarr["region_no"].ToString() : "";//国家/地区代号
                string production_month = jarr.ContainsKey("production_month") ? jarr["production_month"].ToString() : "";//生产月份
                string size_no = jarr.ContainsKey("size_no") ? jarr["size_no"].ToString() : "";//码数
                string newshoes_qty = jarr.ContainsKey("newshoes_qty") ? jarr["newshoes_qty"].ToString() : "";//新鞋数量
                string oldshoes_qty = jarr.ContainsKey("oldshoes_qty") ? jarr["oldshoes_qty"].ToString() : "";//旧鞋数量
                string main_code = jarr.ContainsKey("main_code") ? jarr["main_code"].ToString() : "";//主要不良代号
                string minor_code = jarr.ContainsKey("minor_code") ? jarr["minor_code"].ToString() : "";//次要不良代号
                string factory = jarr.ContainsKey("factory") ? jarr["factory"].ToString() : "";//工厂
                string fob_price = jarr.ContainsKey("fob_price") ? jarr["fob_price"].ToString() : "";//FOB单价
                string datetime = jarr.ContainsKey("datetime") ? jarr["datetime"].ToString() : "";//年份/月份
                string thyf = jarr.ContainsKey("thyf") ? jarr["thyf"].ToString() : "";//退货月份
                string compensation_amount = jarr.ContainsKey("compensation_amount") ? jarr["compensation_amount"].ToString() : "";//赔偿金额
                string problem_point_desc = jarr.ContainsKey("problem_point_desc") ? jarr["problem_point_desc"].ToString() : "";//问题点的描述

                //新鞋数量不能小于0
                if (!string.IsNullOrWhiteSpace(newshoes_qty) && Convert.ToDecimal(newshoes_qty) < 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The number of new shoes cannot be less than 0!";
                    return ret;
                }
                //旧鞋数量不能小于0
                if (!string.IsNullOrWhiteSpace(oldshoes_qty) && Convert.ToDecimal(oldshoes_qty) < 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The number of old shoes cannot be less than 0!";
                    return ret;
                }
                //新鞋数量加旧鞋数量的和不能小于0
                if (!string.IsNullOrWhiteSpace(oldshoes_qty) && !string.IsNullOrWhiteSpace(newshoes_qty) && (Convert.ToDecimal(newshoes_qty) + Convert.ToDecimal(oldshoes_qty)) <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The return quantity cannot be less than or equal to 0!";
                    return ret;
                }
                //FOB单价不能小于0
                if (!string.IsNullOrWhiteSpace(fob_price) && Convert.ToDecimal(fob_price) <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "FOB unit price cannot be less than or equal to 0!";
                    return ret;
                }
                //赔偿金额不能小于0
                if (!string.IsNullOrWhiteSpace(compensation_amount) && Convert.ToDecimal(compensation_amount) <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The compensation amount cannot be less than or equal to 0!";
                    return ret;
                }

                List<imginfo> file_list = new List<imginfo>();
                if (jarr.ContainsKey("file_list"))
                {
                    file_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<imginfo>>(jarr["file_list"].ToString());//图片集合
                }
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = $@"update qcm_market_feedback_m set region_no='{region_no}',size_no='{size_no}',newshoes_qty='{newshoes_qty}',oldshoes_qty='{oldshoes_qty}',main_code='{main_code}',minor_code='{minor_code}',fob_price='{fob_price}',compensation_amount='{compensation_amount}',problem_point_desc='{problem_point_desc}',factory='{factory}',production_month='{production_month}',DATETIME='{datetime}',modifyby='{user}',modifydate='{date}',modifytime='{time}',RETURN_MONTH='{thyf}',po='{po}' where task_no='{task_no}'";
                }
                else
                {
                    task_no = "C" + date;
                    string maxtask_no = DB.GetString($@"select max(task_no) from qcm_market_feedback_m where task_no like '{task_no}%'");
                    if (!string.IsNullOrWhiteSpace(maxtask_no))
                    {
                        string seq = maxtask_no.Replace(task_no, "");

                        int int_seq = Convert.ToInt32(seq) + 1;

                        task_no += int_seq.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        task_no += "00001";
                    }
                    sql = $@"insert into qcm_market_feedback_m(task_no,po,region_no,size_no,newshoes_qty,oldshoes_qty,main_code,minor_code,fob_price,compensation_amount,problem_point_desc,factory,production_month,DATETIME,createby,createdate,createtime,RETURN_MONTH) values('{task_no}','{po}','{region_no}','{size_no}','{newshoes_qty}','{oldshoes_qty}','{main_code}','{minor_code}','{fob_price}','{compensation_amount}','{problem_point_desc}','{factory}','{production_month}','{datetime}','{user}','{date}','{time}','{thyf}')";
                }
                DB.ExecuteNonQuery(sql);


                if (file_list.Count > 0)
                {
                    sql = $@"delete from qcm_market_feedback_m_f where task_no='{task_no}'";
                    DB.ExecuteNonQuery(sql);

                    sql = string.Empty;
                    foreach (imginfo item in file_list)
                    {
                        sql += $@"insert into qcm_market_feedback_m_f(task_no,file_guid,createby,createdate,createtime) values('{task_no}','{item.guid}','{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery($@"begin {sql} end;");
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "Saved successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Save failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 客户退货主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Mian2(object OBJ)
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
                NullKeyValue(jarr);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string region_no = jarr.ContainsKey("region_no") ? jarr["region_no"].ToString() : "";//国家/地区代号
                string production_month = jarr.ContainsKey("production_month") ? jarr["production_month"].ToString() : "";//生产月份
                string return_qty = jarr.ContainsKey("return_qty") ? jarr["return_qty"].ToString() : "";//退货数量
                string main_code = jarr.ContainsKey("main_code") ? jarr["main_code"].ToString() : "";//主要不良代号
                string minor_code = jarr.ContainsKey("minor_code") ? jarr["minor_code"].ToString() : "";//次要不良代号
                string factory = jarr.ContainsKey("factory") ? jarr["factory"].ToString() : "";//工厂
                string fob_price = jarr.ContainsKey("fob_price") ? jarr["fob_price"].ToString() : "";//FOB单价
                string compensation_amount = jarr.ContainsKey("compensation_amount") ? jarr["compensation_amount"].ToString() : "";//赔偿金额
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//备注
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                //FOB单价不能小于0
                if (!string.IsNullOrWhiteSpace(fob_price) && Convert.ToDecimal(fob_price) <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "FOB unit price cannot be less than or equal to 0!";
                    return ret;
                }
                //赔偿金额不能小于0
                if (!string.IsNullOrWhiteSpace(compensation_amount) && Convert.ToDecimal(compensation_amount) <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The compensation amount cannot be less than or equal to 0!";
                    return ret;
                }


                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = $@"update qcm_cust_market_feedback_m set region_no='{region_no}',main_code='{main_code}',minor_code='{minor_code}',fob_price='{fob_price}',return_qty='{return_qty}',compensation_amount='{compensation_amount}',remark='{remark}',factory='{factory}',production_month='{production_month}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                }
                else
                {
                    task_no = "C" + date;
                    string maxtask_no = DB.GetString($@"select max(task_no) from qcm_cust_market_feedback_m where task_no like '{task_no}%'");
                    if (!string.IsNullOrWhiteSpace(maxtask_no))
                    {
                        string seq = maxtask_no.Replace(task_no, "");

                        int int_seq = Convert.ToInt32(seq) + 1;

                        task_no += int_seq.ToString().PadLeft(5, '0');
                    }
                    else
                    {
                        task_no += "00001";
                    }
                    sql = $@"insert into qcm_cust_market_feedback_m(task_no,po,region_no,return_qty,main_code,minor_code,fob_price,compensation_amount,remark,factory,production_month,createby,createdate,createtime) values('{task_no}','{po}','{region_no}','{return_qty}','{main_code}','{minor_code}','{fob_price}','{compensation_amount}','{remark}','{factory}','{production_month}','{user}','{date}','{time}')";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "Saved successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Save failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        ///中国国客户删除主页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete_Main(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//图片id
                string sql = $@"select count(1) from qcm_market_feedback_m where task_no='{task_no}'";
                if (DB.GetInt32(sql) > 0)
                {
                    sql = $@"delete from qcm_market_feedback_m where task_no='{task_no}'";//删除表头
                    DB.ExecuteNonQuery(sql);

                    sql = $@"delete from qcm_market_feedback_m_f where task_no='{task_no}'";//删除明细图片
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        ///客户删除主页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete_Main2(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string sql = $@"delete from qcm_cust_market_feedback_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Deletion failed, reason:" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public class imginfo
        {
            public string file_name { get; set; }
            public string guid { get; set; }
            public string file_url { get; set; }
        }
    }
}
