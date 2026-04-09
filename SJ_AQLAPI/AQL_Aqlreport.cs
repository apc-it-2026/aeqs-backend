using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_Aqlreport
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Main(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//task_no
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string fpnum = jarr.ContainsKey("fpnum") ? jarr["fpnum"].ToString() : "";//po数量
                
   
                string sql = $@"SELECT
	a.bad_classify_code,
	a.bad_classify_name,
	b.bad_item_code,
	b.bad_item_name,
	b.problem_level,
	i.bad_standard,
	nvl(c.bad_qty,0) as bad_qty
FROM
	bdm_aql_bad_classify a 
left join bdm_aql_bad_classify_d b on a.bad_classify_code=b.bad_classify_code 
left join bdm_aql_bad_items i on i.bad_item_code=b.bad_item_code 
left join aql_cma_task_list_m_aql_e_br c on b.BAD_ITEM_CODE=c.bad_item_code and c.task_no='{task_no}'
order by a.bad_classify_code desc
";

                DataTable dt = DB.GetDataTable(sql);
                DataTable dt_head = dt.Clone();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        var dt_head_rows = dt_head.Select($@"bad_classify_code='{item["bad_classify_code"]}'");
                        if (dt_head_rows == null || (dt_head_rows != null && dt_head_rows.Length == 0))
                        {
                            DataRow addRow = dt_head.Rows.Add();
                            addRow["bad_classify_code"] = item["bad_classify_code"];
                            addRow["bad_classify_name"] = item["bad_classify_name"];
                        }
                    }
                    dt_head.DefaultView.Sort = "bad_classify_code asc";
                }

                sql = $@"SELECT 
                        a.task_no,--任务编号
                        a.po,--po号
						a.po_num,--po数量
                        a.art_no,--ART编号
                        r.name_t as art_name,--art名称
						r.SHOE_NO,--鞋型编号
                        d.name_t as shoe_name,--鞋型名称
                        a.inspection_state,--验货状态
                        a.inspection_type,--检验类型
                        a.f_inspection_time,--首次验货日期
                        a.inspection_date,--检验时间
                        a.IS_INSPECTION,--是否首次检验时间
                        b.staff_name,--工厂代表名称
                        a.factory_autograph,--工厂代表编号
                        a.customer_autograph, --客户签名
                        a.createdate, --签名日期
                        pm.Shipcountry_En as guojia,
                        cb.staff_name as checker
                        FROM
                        aql_cma_task_list_m a
                        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                        LEFT JOIN BDM_SE_ORDER_MASTER pm ON pm.MER_PO=a.po 
						left join hr001m b on a.customer_autograph=b.staff_no 
						left join hr001m cb on a.checker=cb.staff_no 
                        where a.task_no='{task_no}'";
                DataTable dt_top = DB.GetDataTable(sql);
                sql = $@"select sample_level,aql_level From aql_cma_task_list_m_aql_m a left join sys001m b on a.aql_level=b.enum_value2  where  b.enum_type='enum_aql_level' and  a.task_no='{task_no}'";
                DataTable dyac = DB.GetDataTable(sql);
             
                string level =string.Empty;
                string levar =string.Empty;
                string ac = "0";
                string ac12 = "0";
                string ac13 = "0";
                if (dyac.Rows.Count > 0)
                {
                    level = dyac.Rows[0]["sample_level"].ToString();
                    levar = dyac.Rows[0]["aql_level"].ToString();
                }
                else
                {
                    level = "2";
                    levar = "AC13";
                }

                sql = string.Empty;
                string vals ="0";
                int xnum = 0;
                string snum = "0";
                if (!string.IsNullOrWhiteSpace(level) && !string.IsNullOrWhiteSpace(fpnum) && !string.IsNullOrWhiteSpace(ac))
                {
                    if (Convert.ToDecimal(fpnum) > 0)
                    {
                        sql = $@"select VALS as snum,(round((VALS/{fpnum}),4)*100) as VALS,{levar} as ac  from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{level}' and START_QTY<={fpnum} and END_QTY>={fpnum}";
                        dyac = DB.GetDataTable(sql);
                        if (dyac.Rows.Count > 0)
                        {
                            ac = dyac.Rows[0]["ac"].ToString();
                            vals = dyac.Rows[0]["VALS"].ToString() + "%";
                            snum= dyac.Rows[0]["snum"].ToString();
                        }

                        //查询 AC12 1.5 AC13 2.5
                        sql = $@"select VALS,AC12,AC13 from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{level}' and to_number(START_QTY)<={fpnum} and to_number(END_QTY)>={fpnum}";
                        DataTable dtAC1213 = DB.GetDataTable(sql);
                        if (dtAC1213.Rows.Count > 0)
                        {
                            ac12 = dtAC1213.Rows[0]["AC12"].ToString();
                            ac13 = dtAC1213.Rows[0]["AC13"].ToString();
                        }

                    }
                   
                }

                level = DB.GetString($@"select enum_value as value from sys001m where enum_type='AQL_ENUM_RAW' AND ENUM_CODE='{level}'");
                sql = $@"
SELECT
	MAX(p.id) as id,
	MAX(p.task_no) as task_no,
	MAX(p.case_no) as case_no,
	MAX(p.cr_size) as cr_size,
	MAX(p.se_qty) as qty,
	SUM(s.se_qty) AS se_qty
FROM
	aql_cma_task_list_m_pb p
	INNER JOIN aql_cma_task_list_m a ON p.TASK_NO = a.TASK_NO
	INNER JOIN BDM_SE_ORDER_MASTER m ON m.mer_po = a.po
	INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
	AND m.SE_ID = s.SE_ID 
	AND p.cr_size = s.SIZE_NO 
WHERE
	a.po = '{po}' 
	AND a.TASK_NO = '{task_no}'
	GROUP BY s.SIZE_NO
	ORDER BY MAX(s.SIZE_SEQ)
";
                //DataTable dts = DB.GetDataTable(sql);
                List<string> xsList = new List<string>();
                DataTable dts = CommonBASE.GetPageDataTable(DB, sql, int.Parse("1"), int.Parse("9"));
                if (dts != null && dts.Rows.Count > 0)
                {
                    
                    //int xnum = 0;
                    foreach (DataRow item in dts.Rows)
                    {
                        string case_no = item["case_no"].ToString();
                        if (!string.IsNullOrEmpty(case_no))
                        {
                            List<string> caselist = case_no.ToString().Split('/').ToList();
                            foreach (var item2 in caselist)
                            {
                                if (!xsList.Contains(item2) && !string.IsNullOrEmpty(item2))
                                {
                                    xsList.Add(item2);
                                    xnum++;
                                }
                            }
                            //xnum += case_no.Split('/').Length;
                        }
                    }
                }
                if (dts.Rows.Count != 9)
                {
                    int addCount = 9 - dts.Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        dts.Rows.Add(dts.NewRow());
                    }
                }
                DataTable dts2 = CommonBASE.GetPageDataTable(DB, sql, int.Parse("2"), int.Parse("9"));
                if (dts2 != null && dts2.Rows.Count > 0)
                {
                    foreach (DataRow item in dts2.Rows)
                    {
                        string case_no = item["case_no"].ToString();
                        if (!string.IsNullOrEmpty(case_no))
                        {
                            List<string> caselist = case_no.ToString().Split('/').ToList();
                            foreach (var item2 in caselist)
                            {
                                if (!xsList.Contains(item2) && !string.IsNullOrEmpty(item2))
                                {
                                    xsList.Add(item2);
                                    xnum++;
                                }
                            }
                        }
                    }
                }
                if (dts2.Rows.Count != 9)
                {
                    int addCount = 9 - dts2.Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        dts2.Rows.Add(dts2.NewRow());
                    }
                }
                DataTable dts3 = CommonBASE.GetPageDataTable(DB, sql, int.Parse("3"), int.Parse("9"));
                if (dts3 != null && dts3.Rows.Count > 0)
                {
                    foreach (DataRow item in dts3.Rows)
                    {
                        string case_no = item["case_no"].ToString();
                        if (!string.IsNullOrEmpty(case_no))
                        {
                            xnum += case_no.Split('/').Length;
                        }
                    }
                }
                if (dts3.Rows.Count != 9)
                {
                    int addCount = 9 - dts3.Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        dts3.Rows.Add(dts3.NewRow());
                    }
                }
                DataTable dts4 = CommonBASE.GetPageDataTable(DB, sql, int.Parse("4"), int.Parse("9"));
                if (dts4 != null && dts4.Rows.Count > 0)
                {
                    foreach (DataRow item in dts4.Rows)
                    {
                        string case_no = item["case_no"].ToString();
                        if (!string.IsNullOrEmpty(case_no))
                        {
                            xnum += case_no.Split('/').Length;
                        }
                    }
                }
                if (dts4.Rows.Count != 9)
                {
                    int addCount = 9 - dts4.Rows.Count;
                    for (int i = 0; i < addCount; i++)
                    {
                        dts4.Rows.Add(dts4.NewRow());
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);//动态视图内容
                dic.Add("data_head", dt_head);//动态视图内容头部
                dic.Add("dt_top", dt_top);//表头内容
                dic.Add("vals", vals);//抽样比例
                dic.Add("level", level);//抽样样本级别
                dic.Add("ac", ac);//ac级别
                dic.Add("ac12", ac12);//ac级别 1.5
                dic.Add("ac13", ac13);//ac级别 2.5
                dic.Add("xnum", xnum);//每箱数量
                dic.Add("snum", snum);//双数
                dic.Add("dts", dts);//点箱内容
                dic.Add("dts2", dts2);//点箱内容
                dic.Add("dts3", dts3);//点箱内容
                dic.Add("dts4", dts4);//点箱内容
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
    }
}
