using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using SJ_RQCAPI.DTO;
using SJeMES_Framework_NETCore.DBHelper;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SJ_RQCAPI
{
    public class Rqc_taskBase
    {
        private static void NullKey(Dictionary<string, object> dic)
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
        /// <summary>
        ///top主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_view(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";
                string so = jarr.ContainsKey("so") ? jarr["so"].ToString() : "";//Adedd for PO change Project
                List<string> workshop_section_no = jarr.ContainsKey("workshop_section_no") ? JsonConvert.DeserializeObject<List<string>>(jarr["workshop_section_no"].ToString()) : null;
                List<string> department = jarr.ContainsKey("department") ? JsonConvert.DeserializeObject<List<string>>(jarr["department"].ToString()) : null;
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";

                List<string> prod_no = jarr.ContainsKey("prod_no") ? JsonConvert.DeserializeObject<List<string>>(jarr["prod_no"].ToString()) : null;
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                string strwhere2 = string.Empty;
                //Array module
                if (department.Count > 0)
                {
                    strwhere += $@" and m.department in ('{string.Join("','", department)}' )";

                }
                if (workshop_section_no.Count > 0)
                {
                    strwhere += $@" and m.workshop_section_no in ('{string.Join("','", workshop_section_no)}' )";
                }

                if (prod_no.Count > 0)
                {
                    strwhere += $@" and m.prod_no in ('{string.Join("','", prod_no)}' )";
                }

                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $@" and m.production_line_code  like '%{production_line_code}%'";
                }

                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and m.createdate  like '%{putin_date}%'";
                }
                if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    strwhere += $@" and m.createdate like '%{end_date}%' ";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and m.createdate BETWEEN '{putin_date}' and '{end_date}'";
                }
                //Added by Ashok to query the data queckly   on 20250218 
                if (string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and m.createdate BETWEEN  to_char(SYSDATE - 5, 'YYYY-MM-DD') AND to_char(SYSDATE, 'YYYY-MM-DD')";
                }
                //End
                if (!string.IsNullOrWhiteSpace(shoe_no))
                {
                    strwhere += $@" and r.name_t like '%{shoe_no}%'";
                }

                if (!string.IsNullOrWhiteSpace(mer_po))
                {
                    strwhere += $@" and m.mer_po like '%{mer_po}%'";
                }
                if (!string.IsNullOrWhiteSpace(so))
                {
                    strwhere += $@" and m.se_id like '%{so}%'";
                }


                //When there is no filter condition, only the data of the most recent month is displayed by default
                if (string.IsNullOrWhiteSpace(strwhere))
                {
                    // Get the date 30 days from now
                    string thirtyDaysAgo = DateTime.Now.AddDays(-30).ToString("yyyyMMddHHmmss");
                    strwhere2 += $@"where order_time >= '{thirtyDaysAgo}'";
                }

                var sql = string.Empty;
              

                sql = $@"
select 
a.*,
CASE
  WHEN ((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)> 0) THEN
    to_char(round(((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0')/(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)),4)*100)||'%'
      ELSE
    '0%'
END AS qty_percent
from(
SELECT
	m.task_no,
	m.workshop_section_no,
	m.develop_season,
	m.shoe_no,
	r.name_t as shoe_name,
	m.prod_no,
	m.mer_po,
	m.production_line_code,
	m.se_id,
(select se_id from bdm_se_order_master where mer_po = m.mer_po) as se_id2,
	m.eq_info_no,
	m.mold_no,
	m.workorder_no,
	m.createdate,
    m.createtime,
	m.department,
	(case  
		when task_state='0' then 'In_Progress' 
		when task_state='1' then 'Stopped'
		when task_state='2' then 'Over'
	end) as task_state,
   -- (case  
    --	when check_type='0' then 'Random_Inspection' 
	--	when check_type='1' then 'Line_Patrol'
	-- end) as check_type,
     (case  
		when RESULT='0' then 'PASS' 
		when RESULT='1' then 'FAIL'
	end) as RESULT,
	ex_task_no,
	lot_qty,
	level_code,
	level_type,
	(SELECT count(1) FROM rqc_task_detail_t where task_no=m.task_no) as aql_qty,
	acre,
	(SELECT count(1) FROM rqc_task_detail_t where task_no=m.task_no and commit_type='0') AS qty,
	(
				SELECT
					COUNT (*)
				FROM
					rqc_task_detail_t
				WHERE
					task_no = ''
				AND commit_type = '1'
			) AS bad_qty,
	res_remark,
	config_no,
    (CASE  
  when M .MODIFYDATE is NOT NULL then REPLACE (M .MODIFYDATE, '-', '') || REPLACE (M .MODIFYTIME, ':', '')
  else REPLACE (M .CREATEDATE, '-', '') || REPLACE (M .CREATETIME, ':', '')
  END ) as order_time
FROM
	rqc_task_m  m
	LEFT JOIN bdm_rd_style r on m.shoe_no=r.shoe_no
		where 1=1 {strwhere} ) a {strwhere2} order by order_time desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_RFT_view(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : ""; 
                List<string> workshop_section_no = jarr.ContainsKey("workshop_section_no") ? JsonConvert.DeserializeObject<List<string>>(jarr["workshop_section_no"].ToString()) : null;
                List<string> department = jarr.ContainsKey("department") ? JsonConvert.DeserializeObject<List<string>>(jarr["department"].ToString()) : null;
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";  
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                //数组模块的
                if (department.Count > 0)
                {
                    strwhere += $@" and b.department in ('{string.Join("','", department)}' )"; 
                }
                if (workshop_section_no.Count > 0)
                {
                    strwhere += $@" and m.workshop_section_no in ('{string.Join("','", workshop_section_no)}' )";
                } 

                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $@" and b.production_line_code  like '%{production_line_code}%'";
                }

                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and b.createdate  like '%{putin_date}%'";
                }
                if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    strwhere += $@" and b.createdate like '%{end_date}%' ";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and b.createdate BETWEEN '{putin_date}' and '{end_date}'";
                } 

                var sql = string.Empty; 
                sql = $@"
 select 
 b.createdate,
b.production_line_code,
b.department,
sum(b.inspected_qty) as inspected_qty,
sum(b.pass_qty) as pass_qty, 
sum(b.inspected_qty)-sum(b.pass_qty) as defect_qty,
CASE
  WHEN (sum(b.inspected_qty)> 0) THEN
to_char(round((sum(b.pass_qty)/sum(b.inspected_qty)),4)*100)||'%' ELSE  '0%' END as RFT from 
(select 
a.createdate,
a.production_line_code,
a.department,
a.inspected_qty,
a.pass_qty,
CASE
  WHEN ((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)> 0) THEN
    to_char(round(((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0')/(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)),4)*100)||'%'
      ELSE
    '0%'
END AS qty_percent
from(
SELECT
  m.task_no, 
  m.production_line_code, 
  m.createdate, 
  m.department,  
  (SELECT count(1) FROM rqc_task_detail_t where task_no=m.task_no) as inspected_qty, 
  (SELECT count(1) FROM rqc_task_detail_t where task_no=m.task_no and commit_type='0') AS pass_qty  
FROM
  rqc_task_m  m
  --LEFT JOIN bdm_rd_style r on m.shoe_no=r.shoe_no
  where 1=1  ) a  ) b where 1=1 {strwhere}  group by b.createdate, b.production_line_code, b.department order by b.createdate desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
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
        /// ART首页的（搜索条件的）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_getart_mainlist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    strwhere += $@" and prod_no like'%{prod_no}%'";
                }
                var sql = string.Empty;
                sql = $@"select PROD_NO from bdm_rd_prod where 1=1 {strwhere}";
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
        /// 工段首页的（搜索条件的）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_getworkshop_mainlist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    strwhere += $@" and workshop_section_no like '%{workshop_section_no}%' or workshop_section_name like '%{workshop_section_no}%'";
                }
                var sql = string.Empty;
                sql = $@"SELECT
      DISTINCT
      b.workshop_section_no,
      b.workshop_section_name
  FROM
      qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no 
      where b.workshop_section_no is not null  {strwhere}";
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
        /// 删除主页任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_delete_task_no(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string sql = string.Empty;
                string id = string.Empty;
                sql = $@"delete from  rqc_task_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"select id from rqc_task_check_t_f where task_no='{task_no}'";
                var idt = DB.GetDataTable(sql);

                List<string> id_list = new List<string>();
                foreach (DataRow item in idt.Rows)
                {
                    id_list.Add(item["id"].ToString());
                }
                if (id_list.Count > 0)
                {
                    sql = $@"delete from  rqc_task_check_t_f where id in({string.Join(',', id_list.Select(x => $"'{x}'"))})";
                    DB.ExecuteNonQuery(sql);

                    sql = $@"delete from  rqc_task_check_t_ff where union_id='{id}'";
                    DB.ExecuteNonQuery(sql);
                }


                sql = $@"select id from rqc_task_item_c where task_no='{task_no}'";
                idt = DB.GetDataTable(sql);
                id_list = new List<string>();
                foreach (DataRow item in idt.Rows)
                {
                    id_list.Add(item["id"].ToString());
                }
                if (id_list.Count > 0)
                {
                    sql = $@"delete from  rqc_task_item_c where id in({string.Join(',', id_list.Select(x => $"'{x}'"))})";
                    DB.ExecuteNonQuery(sql);
                }


                sql = $@"delete from rqc_task_detail_t where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from  rqc_task_detail_t_d where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);


                sql = $@"delete from  rqc_task_xj_item where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);


                sql = $@"select id from rqc_task_xj_item_c where task_no='{task_no}'";
                idt = DB.GetDataTable(sql);

                id_list = new List<string>();
                foreach (DataRow item in idt.Rows)
                {
                    id_list.Add(item["id"].ToString());
                }

                if (id_list.Count > 0)
                {
                    sql = $@"delete from  rqc_task_xj_item_c where id in({string.Join(',', id_list.Select(x => $"'{x}'"))})";
                    DB.ExecuteNonQuery(sql);

                    sql = $@"delete from rqc_task_xj_item_c_f where union_id  in({string.Join(',', id_list.Select(x => $"'{x}'"))})";
                    DB.ExecuteNonQuery(sql);
                }



                // DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                //ret.ErrMsg = "删除失败，原因：" + ex.Message;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
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
        /// po带出主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_getpoview(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string se_id = jarr.ContainsKey("se_id") ? jarr["se_id"].ToString() : "";//changed mer_po into se_id for PO change Project
                var sql = string.Empty;
                sql = $@"
SELECT
	                    nvl(r.SHOE_NO,' ') as SHOE_NO,--鞋型编号
	                    r.DEVELOP_SEASON,--季度
	                    r.PROD_NO,--art
	                    r.user_section,--开发课
						y.style_seq, -- category
	                    r.USER_IN_SHOECHARGE,--鞋型负责人
	                    r.user_technical,--开发技术负责人
						r.develop_type,--PB TYPE
	                    s.qa_principal,--DQA负责人
	                    f.file_url,--鞋图
                        m.se_id,--制令号
                        m.workorder_no,--销售订单
                        m.mer_po,--po号
                        r.MOLD_NO,
                        l.name_t --鞋型
                    FROM
	                    BDM_SE_ORDER_MASTER m
	                    LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
                        LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
	                    LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
	                    LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
	                    LEFT JOIN QCM_SHOES_QA_RECORD_M d ON r.shoe_no = d.shoes_code
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON d.IMAGE_GUID = f.guid
                    WHERE m.se_id= '{se_id}'
                    order by f.guid asc";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    if (string.IsNullOrWhiteSpace(dt.Rows[0]["SHOE_NO"].ToString()))
                    {
                        //ret.ErrMsg = "此PO号无鞋型内容，请重新输入PO号继续此操作";
                        ret.ErrMsg = "This PO number does not contain shoe type content. Please re-enter the PO number to continue this operation.";
                        ret.IsSuccess = false;
                        return ret;
                    }
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
        /// 继续录入带出
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_task_no_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DBServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string CompanyCode = jarr.ContainsKey("CompanyCode") ? jarr["CompanyCode"].ToString() : "";//查询条件 代号【C端跳转使用】
                var sql = string.Empty;
                sql = $@"SELECT
	a.suppliers_code,--厂商代号
	a.suppliers_name,--厂商名称
	a.develop_season,--季度
	a.task_no, --任务编号
	a.workshop_section_no,--工段编号
    g.workshop_section_name,--工段名称
	a.config_no,--配置编号
	d.workmanship_code,--工艺编号
    e.workmanship_name, --工艺名称
	a.shoe_no,--鞋型编号
	b.name_t,--鞋型
	b.user_section,--开发课
	b.style_seq,--category
	b.user_in_shoecharge,--鞋型负责人
	b.user_technical,--开发技术负责人
	b.develop_type,--PB TYPE
	b.qa_principal,--DQA负责人
	b.file_url,--鞋图
	a.prod_no,--art
	a.mer_po,--po号
	a.se_id,--制令号
    (select se_id from bdm_se_order_master where mer_po = a.mer_po) as se_id2,
	a.eq_info_no,--机台
	a.department,--部门
	a.mold_no,--模号
	a.production_line_code,--组别编号
	m.production_line_name,--组别名称
	a.workorder_no,--销售订单
	a.check_type,--检验类型
	a.ex_task_no,--实验室任务编号
	a.lot_qty,--批次数量
	a.level_code,--检验水平
	a.level_type,--aql级别
	a.aql_qty,--aql抽样数量
	a.acre,--acre
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no) as test_qty,-- 已检验数量
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0') as qty,--合格提交次数
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='1') as bad_qty,-- 不合格提交次数
	nvl(a.res_remark,' ') as res_remark,--结果备注
	nvl(a.result,' ') as result, --判定结果
    CASE
  WHEN ((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)> 0) THEN
    to_char(round(((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0')/(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)),4)*100)||'%' 
      ELSE
    '0%'
END AS qty_percent, -- RFT（合格率,合格提交次数/总提交次数）
nvl((SELECT
	count(b.commit_index)
FROM
	rqc_task_item_c c
left join	rqc_task_detail_t_d b  on c.task_no=b.task_no and c.id=b.union_id where b.task_no=a.task_no  group by b.task_no),0) as commit_remark_qty -- 问题点总数
FROM
	rqc_task_m a
LEFT  JOIN (
SELECT
											m.mer_po,
	                    r.user_section,--开发课
										y.style_seq, -- category
	                    r.USER_IN_SHOECHARGE,--鞋型负责人
	                    r.user_technical,--开发技术负责人
										r.develop_type,--PB TYPE
	                    s.qa_principal,--DQA负责人
	                    f.file_url,--鞋图
                        r.MOLD_NO,
                        l.name_t --鞋型
                    FROM
	                    BDM_SE_ORDER_MASTER m
	                    LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
                      LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
	                    LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
	                    LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
	                    LEFT JOIN QCM_SHOES_QA_RECORD_M d ON r.shoe_no = d.shoes_code
	                    LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON d.IMAGE_GUID = f.guid
) b on a.mer_po=b.mer_po
LEFT JOIN bdm_production_line_m m on a.production_line_code=m.production_line_code
LEFT JOIN BDM_PARAM_ITEM_D d on a.workshop_section_no=d.WORKSHOP_SECTION_NO and a.config_no=d.config_no
LEFT JOIN bdm_workmanship_m e on d.WORKMANSHIP_CODE = e.workmanship_code
INNER JOIN (SELECT
      DISTINCT
      b.workshop_section_no,
      b.workshop_section_name
  FROM
      qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no ) g on a.workshop_section_no=g.workshop_section_no

where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);


                string url = DBServer.GetString($@"SELECT uploadurl from SYSORG01M where org = '{CompanyCode}'");
                ret.IsSuccess = true;
                ret.RetData1 = dt;
                ret.RetData = url;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 工段工艺下拉框内容（根据鞋型寻找）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_getworkshop_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";
                string strwhere = string.Empty;
                var sql = string.Empty;
                sql = $@"SELECT
      DISTINCT
      b.workshop_section_no,
      b.workshop_section_name
  FROM
      qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no 
      where b.workshop_section_no is not null and a.shoes_code='{shoe_no}'";

                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("bottom", Type.GetType("System.Object"));
                DataTable dtrow = new DataTable();
                foreach (DataRow item in dt.Rows)
                {
                    sql = $@"select
                BDM_PARAM_ITEM_D.CONFIG_NO,
				bdm_workmanship_m.WORKMANSHIP_NAME,
                BDM_PARAM_ITEM_D.WORKMANSHIP_CODE
				from
				BDM_PARAM_ITEM_D
LEFT JOIN bdm_workmanship_m on BDM_PARAM_ITEM_D.WORKMANSHIP_CODE = bdm_workmanship_m.workmanship_code
                where  BDM_PARAM_ITEM_D.WORKSHOP_SECTION_NO='{item["workshop_section_no"]}'";
                    dtrow = DB.GetDataTable(sql);
                    item["bottom"] = dtrow;
                }
                if (dt.Rows.Count < 2)
                {
                    ret.ErrMsg = "This shoe type does not maintain the content of the workmanship, please check";
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
        /// 组别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_production_line_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    strwhere += $@"production_line_code like '%{value}%' or production_line_name like '%{value}%'";
                }
                var sql = $@"select production_line_code,production_line_name from bdm_production_line_m where 1=1 {strwhere}";
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
        /// 部门
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_production_branch_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//组别
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    strwhere += $@" and  department like '%{value}%'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $@" and  production_line_code='{production_line_code}'";
                }
                if (string.IsNullOrWhiteSpace(strwhere))
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "查无数据";
                    ret.ErrMsg = "No data found";
                    return ret;
                }
                var sql = $@"select
	DISTINCT
	department
from(
SELECT
	PRODUCTION_LINE_CODE,
	PRODUCTION_LINE_NAME,
	department
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	DEPARTMENT_CODE as PRODUCTION_LINE_CODE,
	DEPARTMENT_NAME as PRODUCTION_LINE_NAME,
	UDF07 AS department
FROM
	base005m) where 1=1  {strwhere}";
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
        /// 部门
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_production_branch_list2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//组别
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    strwhere += $@" and  department like '%{value}%'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $@" and  production_line_code='{production_line_code}'";
                }
                //                var sql = $@"select
                //	DISTINCT
                //	department
                //from(
                //SELECT
                //	PRODUCTION_LINE_CODE,
                //	PRODUCTION_LINE_NAME,
                //	department
                //FROM
                //	BDM_PRODUCTION_LINE_M
                //UNION 
                //SELECT
                //	DEPARTMENT_CODE as PRODUCTION_LINE_CODE,
                //	DEPARTMENT_NAME as PRODUCTION_LINE_NAME,
                //	UDF07 AS department
                //FROM
                //	base005m) where 1=1  {strwhere}";


                var sql = $@"
SELECT DISTINCT department  from (
SELECT 
	  a.DEPARTMENT_CODE as PRODUCTION_LINE_CODE,
    a.department_code ,
    b.department_name as department
FROM
    base005m a
    LEFT JOIN base005m b ON a.udf07 = b.department_code 
)tab
WHERE 1=1
{strwhere}";
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
        /// 页面下方小盒子内容展示,history是任务下的明细，明细的第一条就是最近录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_getdiv_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString().Trim() : "";
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString().Trim() : "";
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString().Trim() : "";
                var sql = $@"SELECT
                         d.id, --id
                         d.shoes_code,
                        'DQA' as source,
                        d.choice_name, --材料
                        d.inspection_code, --检验项目编号
                        '' as inspection_name, --检验项目
                        m.enum_value, --外观检验
                        m.enum_value2,
                        d.standard_value,--标准
                        d.unit,--单位
                        d.remark, --备注
                        d.other_measures,--其他措施
                        t.file_url --图片
                        FROM
                        qcm_dqa_mag_m w 
                        LEFT JOIN qcm_dqa_mag_d d on w.id=d.m_id
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type' 
                        LEFT JOIN QCM_QA_MAG_D_IMG_S s on (s.d_id=d.id and s.QA_TYPE='0' and s.IS_MAIN='1') 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on s.image_guid=t.guid
                        WHERE 1=1 and d.shoes_code='{shoe_no}' and w.workshop_section_no='{workshop_section_no}' and d.ISDELETE='0'
                        UNION	
                         SELECT
                        d.id,
                        d.shoes_code,
                         'MQA' as source,
                        d.choice_name,
                        d.inspection_code,
                        '' as inspection_name,
                        m.enum_value,
                        m.enum_value2,
                        d.standard_value,
                        d.unit,
                        d.remark,
                        d.other_measures,
                        t.file_url
                        FROM
                        qcm_mqa_mag_d d
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type'
                        LEFT JOIN QCM_QA_MAG_D_IMG_S s on (s.d_id=d.id and s.QA_TYPE='1' and s.IS_MAIN='1') 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on s.image_guid=t.guid
                        WHERE 1=1 and d.shoes_code='{shoe_no}' and d.workshop_section_no='{workshop_section_no}'  and d.ISDELETE='0'";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("history", Type.GetType("System.Object"));
                dt.Columns.Add("top_imginfo_list", Type.GetType("System.Object"));

                sql = $@"SELECT
	A.ID,
    A.D_ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	QCM_DQA_MAG_D_F A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_id = B.GUID";
                DataTable dt_dqaimg = DB.GetDataTable(sql);//dqa文件集合

                sql = $@"SELECT
	    A.ID,
        A.D_ID,
	    B.FILE_NAME,
	    B.FILE_URL,
	    B.GUID,
	    B.SUFFIX
    FROM
	    QCM_MQA_MAG_D_F A
    INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_id = B.GUID";
                DataTable dt_mqaimg = DB.GetDataTable(sql);//mqa文件集合

                sql = $@"SELECT
    ID,
    TASK_NO,--任务编号
	UNION_ID,
	SOURCE_TYPE,--类型
	QTY,--检验总数
	Q_QTY,--合格数量
	nvl(BAD_DESC,' '),--不良问题描述
	CHECK_RES --检验结果
FROM
	RQC_TASK_CHECK_T_F where  task_no='{task_no}'  ORDER BY CREATEDATE DESC";

                DataTable dt_rqc_task_check_t_f = DB.GetDataTable(sql);
                dt_rqc_task_check_t_f.Columns.Add("imginfo_list", Type.GetType("System.Object"));//检验任务图片

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	rqc_task_check_t_ff A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";//RQC任务——核对——记录
                DataTable dt_rqcimg = DB.GetDataTable(sql);

                string source_type = string.Empty;
                DataTable drrow = new DataTable();
                List<imginfo> images = new List<imginfo>();
                List<imginfos> images2 = new List<imginfos>();
                DataRow[] dr = null;
                DataRow[] dr2 = null;



                foreach (DataRow item in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["enum_value"].ToString()))
                    {
                        item["inspection_name"] = DB.GetString($@"select inspection_name from {item["enum_value2"]} where inspection_code='{item["inspection_code"]}'");
                    }
                    images = new List<imginfo>();
                    images2 = new List<imginfos>();
                    if (item["source"].ToString() == "DQA")
                    {
                        source_type = "0";
                        //DQA附件
                        dr = dt_dqaimg.Select($"D_ID='{item["ID"]}'");
                        foreach (DataRow dr_dqaimg in dr)
                        {
                            images2.Add(new imginfos
                            {
                                guid = dr_dqaimg["GUID"].ToString(),
                                file_url = dr_dqaimg["FILE_URL"].ToString(),
                                file_name = dr_dqaimg["FILE_NAME"].ToString()
                            });
                        }
                    }
                    else
                    {
                        source_type = "1";
                        //MQA附件
                        dr = dt_mqaimg.Select($"D_ID='{item["ID"]}'");
                        foreach (DataRow dr_mqaimg in dr)
                        {
                            images2.Add(new imginfos
                            {
                                guid = dr_mqaimg["GUID"].ToString(),
                                file_url = dr_mqaimg["FILE_URL"].ToString(),
                                file_name = dr_mqaimg["FILE_NAME"].ToString()
                            });
                        }

                    }
                    item["top_imginfo_list"] = images2;//把文件装到top_imginfo_list
                    dr = dt_rqc_task_check_t_f.Select($"UNION_ID='{item["ID"]}' and SOURCE_TYPE='{source_type}'");
                    item["history"] = new List<imginfo>();

                    DataTable dtTest = dt_rqc_task_check_t_f.Clone();
                    foreach (DataRow dr_rqc_task_check_t_f in dr)
                    {
                        dtTest.Clear();
                        var search_dr2 = dt_rqcimg.Select($"UNION_ID='{dr_rqc_task_check_t_f["ID"]}'");
                        images = new List<imginfo>();
                        foreach (DataRow dr_rqcimg in search_dr2)
                        {
                            images.Add(new imginfo
                            {
                                guid = dr_rqcimg["GUID"].ToString(),
                                image_url = dr_rqcimg["FILE_URL"].ToString()
                            });
                        }
                        dr_rqc_task_check_t_f["imginfo_list"] = images;
                        dtTest.ImportRow(dr_rqc_task_check_t_f);
                    }
                    item["history"] = dtTest;
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
        /// 小盒子dqa&mqa核对查看最近提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBasediv_mx(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 dqa&mqa的id
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";//查询条件 来源类型
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 来源类型
                string swhere = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(source_type))
                {
                    if (source_type == "DQA")
                    {
                        source_type = "0";
                    }
                    else if (source_type == "MQA")
                    {
                        source_type = "1";
                    }
                }
                sql = $@"SELECT
    ID,
    TASK_NO,--任务编号
	UNION_ID,
	SOURCE_TYPE,--类型
	QTY,--检验总数
	Q_QTY,--合格数量
	BAD_DESC,--不良问题描述
	CHECK_RES --检验结果
FROM
	RQC_TASK_CHECK_T_F where  task_no='{task_no}' and union_id='{id}' and source_type='{source_type}'   ORDER BY CREATEDATE DESC";
                DataTable dt = DB.GetDataTable(sql);

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	rqc_task_check_t_ff A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";//RQC任务——核对——记录
                DataTable dt_rqcimg = DB.GetDataTable(sql);

                dt.Columns.Add("imginfo_list", Type.GetType("System.Object"));
                List<imginfo> images = new List<imginfo>();
                DataRow[] dr = null;
                foreach (DataRow item in dt.Rows)
                {
                    images = new List<imginfo>();
                    dr = dt_rqcimg.Select($"UNION_ID='{item["ID"]}'");
                    item["imginfo_list"] = images;
                    foreach (DataRow dr_mqaimg in dr)
                    {
                        images.Add(new imginfo
                        {
                            guid = dr_mqaimg["GUID"].ToString(),
                            image_url = dr_mqaimg["FILE_URL"].ToString()
                        });
                    }
                    item["imginfo_list"] = images;
                }
                ret.RetData1 = dt;
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
        /// 小盒子dqa&mqa历史记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBasediv_mx_min(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 dqa&mqa的id
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";//查询条件 来源类型
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 来源类型
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                if (!string.IsNullOrWhiteSpace(source_type))
                {
                    if (source_type == "DQA")
                    {
                        source_type = "0";
                    }
                    else if (source_type == "MQA")
                    {
                        source_type = "1";
                    }
                }
                string sql = string.Empty;

                sql = $@"SELECT
    task_no,--任务编号
	union_id,
	source_type,--类型
	qty,--检验总数
	q_qty,--合格数量
	bad_desc,--不良问题描述
	check_res --检验结果
FROM
	rqc_task_check_t_f where task_no='{task_no}' and union_id='{id}' and source_type='{source_type}' order by createdate desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                dt.Columns.Add("imginfo_list", Type.GetType("System.Object"));
                DataTable dt_rowurl = new DataTable();
                foreach (DataRow item in dt.Rows)
                {
                    //多明细==>又有属于自己的图片
                    sql = $@"SELECT
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	rqc_task_check_t_ff A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID
WHERE
	A.union_id = '{item["id"]}'";
                    dt_rowurl = DB.GetDataTable(sql);
                    List<imginfo> images = new List<imginfo>();
                    if (dt_rowurl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_rowurl.Rows.Count; i++)
                        {
                            images.Add(new imginfo
                            {
                                guid = dt_rowurl.Rows[i]["GUID"].ToString(),
                                image_url = dt_rowurl.Rows[i]["FILE_URL"].ToString()
                            }); ;
                        }
                    }
                    item["imginfo_list"] = images;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("total", rowCount);
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
        /// rqc创建页面生成任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_Insert(object OBJ)
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
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//条件 工段编号
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//条件 季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//条件 art
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//条件 po
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//条件 组别
                string se_id = jarr.ContainsKey("se_id") ? jarr["se_id"].ToString() : "";//条件 制令
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";//条件 机台
                string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//条件 模号
                string workorder_no = jarr.ContainsKey("workorder_no") ? jarr["workorder_no"].ToString() : "";//条件 销售订单号
                string department = jarr.ContainsKey("department") ? jarr["department"].ToString() : "";//条件 部门
                string ex_task_no = jarr.ContainsKey("ex_task_no") ? jarr["ex_task_no"].ToString() : "";//实验室任务编号
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";//配置编号
                string suppliers_name = jarr.ContainsKey("suppliers_name") ? jarr["suppliers_name"].ToString() : "";//厂商名称
                string suppliers_code = jarr.ContainsKey("suppliers_code") ? jarr["suppliers_code"].ToString() : "";//厂商代号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //分类的
                string task_no = string.Empty;
                string sql = string.Empty;

                if (string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    //ret.ErrMsg = $@"工段不能为空";
                    ret.ErrMsg = $@"The section cannot be empty";
                    ret.IsSuccess = false;
                    return ret;
                }

                sql = $@"select
	DISTINCT
	department
from(
SELECT
	PRODUCTION_LINE_CODE,
	PRODUCTION_LINE_NAME,
	department
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	DEPARTMENT_CODE as PRODUCTION_LINE_CODE,
	DEPARTMENT_NAME as PRODUCTION_LINE_NAME,
	UDF07 AS department
FROM
	base005m) where  production_line_code='{production_line_code}'";

                string departments = DB.GetString(sql);

                if (string.IsNullOrWhiteSpace(departments))
                {
                    //ret.ErrMsg = $@"该组别无此部门";
                    ret.ErrMsg = $@"There is no such department in this group";
                    ret.IsSuccess = false;
                    return ret;
                }
                if (!string.IsNullOrWhiteSpace(ex_task_no))
                {
                    sql = $@"select task_no from qcm_ex_task_list_m where task_no='{ex_task_no}'";
                    string ex_task_nos = DB.GetString(sql);
                    if (string.IsNullOrWhiteSpace(ex_task_nos))
                    {
                       // ret.ErrMsg = $@"无此【{ex_task_no}】实验室编号，请确认";
                        ret.ErrMsg = $@"There is no such [{ex_task_no}] laboratory number, please confirm";
                        ret.IsSuccess = false;
                        return ret;
                    }
                }


                #region 表头（抽检）（巡线）添加
                task_no = "R" + date;
                string maxtask_no = DB.GetString($@"select max(task_no) from rqc_task_m where task_no like '{task_no}%'");
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
                //表头的添加
                sql = $@"insert into  rqc_task_m(
                        task_no,
                        workshop_section_no,
                        develop_season,
                        shoe_no,
                        prod_no,
                        mer_po,
                        production_line_code,
                        se_id,
                        eq_info_no,
                        mold_no,
                        workorder_no,
                        department,
                        task_state,
                        ex_task_no,
                        config_no,
                        createby,
                        createdate,
                        createtime,
                        SUPPLIERS_CODE,
                        SUPPLIERS_NAME
                    )  values(@task_no,@workshop_section_no,@develop_season,@shoe_no,@prod_no,@mer_po,@production_line_code,@se_id,@eq_info_no,@mold_no,@workorder_no,@department,@task_state,@ex_task_no,@config_no,@createby,@createdate,@createtime,@suppliers_code,@suppliers_name)";
                dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);
                dic.Add("workshop_section_no", workshop_section_no);
                dic.Add("develop_season", develop_season);
                dic.Add("shoe_no", shoe_no);
                dic.Add("prod_no", prod_no);
                dic.Add("mer_po", mer_po);
                dic.Add("production_line_code", production_line_code);
                dic.Add("se_id", se_id);
                dic.Add("eq_info_no", eq_info_no);
                dic.Add("mold_no", mold_no);
                dic.Add("workorder_no", workorder_no);
                dic.Add("department", department);
                dic.Add("task_state", "0");//任务状态
                dic.Add("ex_task_no", ex_task_no);
                dic.Add("config_no", config_no);
                dic.Add("createby", user);
                dic.Add("createdate", date);
                dic.Add("createtime", time);
                dic.Add("suppliers_code", suppliers_code);
                dic.Add("suppliers_name", suppliers_name);
                DB.ExecuteNonQuery(sql, dic);
                #endregion
                string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");
                string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                string tabname = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{inspection_type}' and enum_code in ('0','1','2','3','4','5','6')");
                if (!string.IsNullOrWhiteSpace(tabname))
                {
                    //sql = $@"SELECT 
                    //        inspection_code,
                    //        inspection_name,
                    //        qc_type,
                    //        judgment_criteria,
                    //        standard_value,
                    //        shortcut_key
                    //        FROM
                    //        {tabname}
                    //        WHERE qc_type='2' and ROWNUM<21
                    //        ORDER BY id asc";

                    sql = $@"SELECT 
                            inspection_code,
                            inspection_name,
                            qc_type,
                            judgment_criteria,
                            standard_value,
                            shortcut_key
                            FROM
                            {tabname}
                            WHERE qc_type='2'
                            ORDER BY id asc";
                    //qc_type='2'等于2是RQC类别
                    DataTable inspectiondt = DB.GetDataTable(sql);
                    sql = string.Empty;
                    int index = 0;
                    dic = new Dictionary<string, object>();
                    foreach (DataRow item in inspectiondt.Rows)
                    {
                        index++;
                        dic.Add($"task_no{index}", task_no);
                        dic.Add($"inspection_type{index}", inspection_type);
                        dic.Add($"inspection_code{index}", item["inspection_code"]);
                        dic.Add($"inspection_name{index}", item["inspection_name"]);
                        dic.Add($"qc_type{index}", item["qc_type"]);
                        dic.Add($"judgment_criteria{index}", item["judgment_criteria"]);
                        dic.Add($"standard_value{index}", item["standard_value"]);
                        dic.Add($"shortcut_key{index}", item["shortcut_key"]);
                        dic.Add($"createby{index}", user);
                        dic.Add($"createdate{index}", date);
                        dic.Add($"createtime{index}", time);
                        sql += $@"insert into rqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,standard_value,shortcut_key,createby,createdate,createtime) values(@task_no{index},@inspection_type{index},@inspection_code{index},@inspection_name{index},@qc_type{index},@judgment_criteria{index},@standard_value{index},@shortcut_key{index},@createby{index},@createdate{index},@createtime{index});";
                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        DB.ExecuteNonQuery("begin " + sql + " end;", dic);
                    }

                }
                sql = $@"SELECT
    m.param_item_no, --参数项目编号
    m.param_item_name, --参数项目名称
    m.workshop_section_no --工段编号
FROM
    BDM_PARAM_ITEM_UNION U 
INNER JOIN BDM_PARAM_ITEM_M M ON M.PARAM_ITEM_NO=U.PARAM_ITEM_NO
WHERE
    U.CONFIG_NO = '{config_no}' and m.workshop_section_no='{workshop_section_no}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    dic = new Dictionary<string, object>();
                    int index = 0;
                    sql = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        index++;
                        dic.Add($"task_no{index}", task_no);
                        dic.Add($"param_item_no{index}", dt.Rows[i]["param_item_no"]);
                        dic.Add($"param_item_name{index}", dt.Rows[i]["param_item_name"]);
                        sql += $@"insert into rqc_task_xj_item(task_no,param_item_no,param_item_name,createby,createdate,createtime) values(@task_no{index},@param_item_no{index},@param_item_name{index},'{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;", dic);
                }
                //ret.ErrMsg = "提交成功";
                ret.ErrMsg = "Submitted successfully";
                ret.IsSuccess = true;
                ret.RetData1 = task_no;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "提交失败，原因：" + ex.Message;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// rqc创建页面生成任务(v0版本)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_InserDemo(object OBJ)
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
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//条件 工段编号
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//条件 季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//条件 art
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//条件 po
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//条件 组别
                string se_id = jarr.ContainsKey("se_id") ? jarr["se_id"].ToString() : "";//条件 制令
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";//条件 机台
                string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//条件 模号
                string workorder_no = jarr.ContainsKey("workorder_no") ? jarr["workorder_no"].ToString() : "";//条件 销售订单号
                string department = jarr.ContainsKey("department") ? jarr["department"].ToString() : "";//条件 部门
                string ex_task_no = jarr.ContainsKey("ex_task_no") ? jarr["ex_task_no"].ToString() : "";//实验室任务编号
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";//配置编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //分类的
                string check_type = jarr.ContainsKey("check_type") ? jarr["check_type"].ToString() : "";//枚举 0抽验,1巡线
                string task_no = string.Empty;
                string sql = string.Empty;

                sql = $@"select department from bdm_production_line_m where production_line_code='{production_line_code}'";

                string departments = DB.GetString(sql);

                if (string.IsNullOrWhiteSpace(departments))
                {
                    //ret.ErrMsg = $@"该组别无此部门";
                    ret.ErrMsg = $@"There is no such department in this group";
                    ret.IsSuccess = false;
                    return ret;
                }
                if (!string.IsNullOrWhiteSpace(ex_task_no))
                {
                    sql = $@"select task_no from qcm_ex_task_list_m where task_no='{ex_task_no}'";
                    string ex_task_nos = DB.GetString(sql);
                    if (string.IsNullOrWhiteSpace(ex_task_nos))
                    {
                        //ret.ErrMsg = $@"无此【{ex_task_no}】实验室编号，请确认";
                        ret.ErrMsg = $@"There is no such [{ex_task_no}] laboratory number, please confirm";
                        ret.IsSuccess = false;
                        return ret;
                    }
                }

                if (string.IsNullOrWhiteSpace(check_type))
                {
                    //ret.ErrMsg = "检验类型不能为空";
                    ret.ErrMsg = "Test type cannot be empty";
                    ret.IsSuccess = false;
                    return ret;
                }
                #region 表头（抽检）（巡线）添加
                task_no = "R" + date;
                string maxtask_no = DB.GetString($@"select max(task_no) from rqc_task_m where task_no like '{task_no}%'");
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
                //Adding a header
                sql = $@"insert into  rqc_task_m(
                        task_no,
                        workshop_section_no,
                        develop_season,
                        shoe_no,
                        prod_no,
                        mer_po,
                        production_line_code,
                        se_id,
                        eq_info_no,
                        mold_no,
                        workorder_no,
                        department,
                        task_state,
                        check_type,
                        ex_task_no,
                        config_no,
                        createby,
                        createdate,
                        createtime
                    )  values(@task_no,@workshop_section_no,@develop_season,@shoe_no,@prod_no,@mer_po,@production_line_code,@se_id,@eq_info_no,@mold_no,@workorder_no,@department,@task_state,@check_type,@ex_task_no,@config_no,@createby,@createdate,@createtime)";
                dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);
                dic.Add("workshop_section_no", workshop_section_no);
                dic.Add("develop_season", develop_season);
                dic.Add("shoe_no", shoe_no);
                dic.Add("prod_no", prod_no);
                dic.Add("mer_po", mer_po);
                dic.Add("production_line_code", production_line_code);
                dic.Add("se_id", se_id);
                dic.Add("eq_info_no", eq_info_no);
                dic.Add("mold_no", mold_no);
                dic.Add("workorder_no", workorder_no);
                dic.Add("department", department);
                dic.Add("task_state", "0");//任务状态
                dic.Add("check_type", check_type);
                dic.Add("ex_task_no", ex_task_no);
                dic.Add("config_no", config_no);
                dic.Add("createby", user);
                dic.Add("createdate", date);
                dic.Add("createtime", time);
                DB.ExecuteNonQuery(sql, dic);
                #endregion
                switch (check_type)
                {
                    case "0":
                        string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");
                        string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                        string tabname = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{inspection_type}' and enum_code in ('0','1','2','3','4','5','6')");
                        if (!string.IsNullOrWhiteSpace(tabname))
                        {
                            sql = $@"SELECT 
                            inspection_code,
                            inspection_name,
                            qc_type,
                            judgment_criteria,
                            standard_value,
                            shortcut_key
                            FROM
                            {tabname}
                            WHERE qc_type='2' and ROWNUM<21
                            ORDER BY id asc";
                            DataTable inspectiondt = DB.GetDataTable(sql);
                            sql = string.Empty;
                            int index = 0;
                            dic = new Dictionary<string, object>();
                            foreach (DataRow item in inspectiondt.Rows)
                            {
                                index++;
                                dic.Add($"task_no{index}", task_no);
                                dic.Add($"inspection_type{index}", inspection_type);
                                dic.Add($"inspection_code{index}", item["inspection_code"]);
                                dic.Add($"inspection_name{index}", item["inspection_name"]);
                                dic.Add($"qc_type{index}", item["qc_type"]);
                                dic.Add($"judgment_criteria{index}", item["judgment_criteria"]);
                                dic.Add($"standard_value{index}", item["standard_value"]);
                                dic.Add($"shortcut_key{index}", item["shortcut_key"]);
                                dic.Add($"createby{index}", user);
                                dic.Add($"createdate{index}", date);
                                dic.Add($"createtime{index}", time);
                                sql += $@"insert into rqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,standard_value,shortcut_key,createby,createdate,createtime) values(@task_no{index},@inspection_type{index},@inspection_code{index},@inspection_name{index},@qc_type{index},@judgment_criteria{index},@standard_value{index},@shortcut_key{index},@createby{index},@createdate{index},@createtime{index});";
                            }
                            if (!string.IsNullOrWhiteSpace(sql))
                            {
                                DB.ExecuteNonQuery("begin " + sql + " end;", dic);
                            }

                        }
                        break;
                    case "1":
                        sql = $@"SELECT
    m.param_item_no, --参数项目编号
    m.param_item_name, --参数项目名称
    m.workshop_section_no --工段编号
FROM
    BDM_PARAM_ITEM_UNION U 
INNER JOIN BDM_PARAM_ITEM_M M ON M.PARAM_ITEM_NO=U.PARAM_ITEM_NO
WHERE
    U.CONFIG_NO = '{config_no}' and m.workshop_section_no='{workshop_section_no}'";
                        DataTable dt = DB.GetDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            dic = new Dictionary<string, object>();
                            int index = 0;
                            sql = string.Empty;
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                index++;
                                dic.Add($"task_no{index}", task_no);
                                dic.Add($"param_item_no{index}", dt.Rows[i]["param_item_no"]);
                                dic.Add($"param_item_name{index}", dt.Rows[i]["param_item_name"]);
                                sql += $@"insert into rqc_task_xj_item(task_no,param_item_no,param_item_name,createby,createdate,createtime) values(@task_no{index},@param_item_no{index},@param_item_name{index},'{user}','{date}','{time}');";
                            }
                            DB.ExecuteNonQuery("begin " + sql + " end;", dic);
                        }
                        break;
                }
                //ret.ErrMsg = "提交成功";
                ret.ErrMsg = "Submitted successfully";
                ret.IsSuccess = true;
                ret.RetData1 = task_no;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
               // ret.ErrMsg = "提交失败，原因：" + ex.Message;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// rqc创建页面数据录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_Insert2(object OBJ)
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
                NullKey(jarr);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string union_id = jarr.ContainsKey("union_id") ? jarr["union_id"].ToString() : "";
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";
                string q_qty = jarr.ContainsKey("q_qty") ? jarr["q_qty"].ToString() : "";
                string bad_desc = jarr.ContainsKey("bad_desc") ? jarr["bad_desc"].ToString() : "";
                string check_res = jarr.ContainsKey("check_res") ? jarr["check_res"].ToString() : "";
                List<imginfo> list = jarr.ContainsKey("imginfo_list") ? JsonConvert.DeserializeObject<List<imginfo>>(jarr["imginfo_list"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                string sql = string.Empty;

                sql = $@"UPDATE RQC_TASK_M SET MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE TASK_NO='{task_no}'";
                DB.ExecuteNonQuery(sql);

                if (!string.IsNullOrWhiteSpace(source_type))
                {
                    if (source_type == "DQA")
                    {
                        source_type = "0";
                    }
                    else if (source_type == "MQA")
                    {
                        source_type = "1";
                    }
                }
                sql = $@"insert into rqc_task_check_t_f(task_no,union_id,source_type,qty,q_qty,bad_desc,check_res,createby,createdate,createtime) values('{task_no}','{union_id}','{source_type}','{qty}','{q_qty}','{bad_desc}','{check_res}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);
                item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "rqc_task_check_t_f");

                //图片明细
                // sql = $"delete from rqc_task_check_t_ff where union_id='{item_id}'";
                //DB.ExecuteNonQuery(sql);
                if (list.Count > 0)
                {
                    sql = string.Empty;
                    foreach (imginfo item in list)
                    {
                        sql += $"insert into rqc_task_check_t_ff(union_id,file_guid,createby,createdate,createtime) values('{item_id}','{item.guid}','{user}','{date}','{time}');";//添加对应的guid关联图片

                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }

                #region 更新首次检验
                if (!string.IsNullOrWhiteSpace(source_type))
                {
                    string check_res_res = "Fail";
                    if (check_res == "0")
                    {
                        check_res_res = "Pass";
                    }
                    string dp_name = DB.GetStringline($@"
SELECT
	a.DEPARTMENT_NAME
FROM
	HR001M m
INNER JOIN BASE005M a ON a.DEPARTMENT_CODE=m.STAFF_DEPARTMENT
WHERE m.STAFF_NO='{user}'
");
                    string updateTableName = "";
                    if (source_type == "0")
                    {
                        updateTableName = "QCM_DQA_MAG_D";
                    }
                    else if (source_type == "1")
                    {
                        updateTableName = "QCM_MQA_MAG_D";
                    }
                    sql = $@"SELECT F_INSP_RES FROM {updateTableName} WHERE ID={union_id}";
                    string exist_res = DB.GetStringline(sql);
                    //无首次检测
                    if (string.IsNullOrWhiteSpace(exist_res))
                    {
                        sql = $@"UPDATE {updateTableName} SET F_INSP_DEP='{dp_name}',F_INSP_DATE='{date}',F_INSP_RES='{check_res_res}' WHERE ID={union_id}";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                #endregion

                ret.IsSuccess = true;
                //ret.ErrMsg = "提交成功";
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "提交失败，原因：" + ex.Message;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// RQC创建页面右下撤回一次
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_recall(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"UPDATE RQC_TASK_M SET MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE TASK_NO='{task_no}'";
                DB.ExecuteNonQuery(sql);

                string id = DB.GetString($@"SELECT max(id) FROM RQC_TASK_DETAIL_T WHERE TASK_NO='{task_no}'");
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $@"delete from rqc_task_detail_t where task_no='{task_no}' and id={id}";
                    DB.ExecuteNonQuery(sql);
                }
                string commit_index = DB.GetString($@"select max(commit_index) from rqc_task_detail_t_d where task_no='{task_no}'");

                //Remove the problem points only when the submission is unqualified
                string commit_type = DB.GetString($@"select commit_type from rqc_task_detail_t where id = '{id}'");
                if (commit_type != "0" && commit_type != "2")
                {
                    sql = $@"delete from rqc_task_detail_t_d where task_no='{task_no}' and commit_index='{commit_index}'";
                }
                DB.ExecuteNonQuery(sql);

                DB.Commit();
                ret.IsSuccess = true;
               // ret.ErrMsg = "撤销成功";
                ret.ErrMsg = "Revoked successfully";


            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "撤销失败，原因是：" + ex.Message;
                ret.ErrMsg = "Undo failed because：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 检验项目数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_jyxm_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string sql = $@"SELECT
			'' AS checkeds,
			MAX(c.ID) AS ID,
			MAX(c.inspection_code) AS inspection_code,
			MAX(c.inspection_name) AS inspection_name,
			'RQC' AS qc_type,
			MAX(c.judgment_criteria) AS judgment_criteria,
			MAX(c.standard_value) AS standard_value,
			0 num
		FROM
			rqc_task_item_c c
		WHERE
			c.task_no = '{task_no}'
		GROUP BY
			c.ID";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("imginfo_list", Type.GetType("System.Object"));

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	 rqc_task_detail_t_f A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";
                DataTable dt_img = DB.GetDataTable(sql);

                DataRow[] dr = null;
                List<imginfo> images = new List<imginfo>();
                foreach (DataRow item in dt.Rows)
                {
                    dr = dt_img.Select($"UNION_ID='{item["ID"]}'");
                    images = new List<imginfo>();
                    foreach (DataRow dr_img in dr)//假如多个图片
                    {
                        images.Add(new imginfo
                        {
                            guid = dr_img["GUID"].ToString(),
                            image_url = dr_img["FILE_URL"].ToString()
                        });
                    }
                    item["imginfo_list"] = images;
                    decimal num = DB.GetDecimal($@"select count(1) from rqc_task_detail_t_d where union_id='{item["id"]}' group by task_no");
                    item["num"] = num;
                }
                sql = $@"select
    a.lot_qty,--批次数量
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no) as test_qty,-- 已检验数量
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0') as qty,--合格提交次数
	(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='1') as bad_qty,-- 不合格提交次数
	
    CASE
  WHEN ((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)> 0) THEN
    to_char(round(((SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no and commit_type='0')/(SELECT count(1) FROM rqc_task_detail_t where task_no=a.task_no)),4)*100)||'%'
      ELSE
    '0%'
END AS qty_percent, -- RFT（合格率,合格提交次数/总提交次数）
nvl((SELECT
	count(b.commit_index)
FROM
	rqc_task_item_c c
left join	rqc_task_detail_t_d b  on c.task_no=b.task_no and c.id=b.union_id where b.task_no=a.task_no  group by b.task_no),0) as commit_remark_qty,-- 问题点总数
nvl(a.res_remark,' ') as res_remark,--结果备注
nvl(a.result,' ') as result --判定结果
From rqc_task_m a where a.task_no='{task_no}'";
                DataTable dt1 = DB.GetDataTable(sql);
                sql = $@"select*from (
SELECT
			a.inspection_code,
			a.inspection_name,
			count(b.task_no) as qty
		FROM
			rqc_task_item_c a
left join rqc_task_detail_t_d  b on a.task_no=b.task_no and a.id=b.UNION_ID
		WHERE a.task_no='{task_no}'
		GROUP BY
			a.inspection_code,a.inspection_name
order by count(b.task_no) desc) where rownum<4";
                DataTable dt2 = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("rqc_task_detail_t", dt);
                dic.Add("center_list", dt1);//检验的局部刷新数据
                dic.Add("top_test", dt2);//top数据展示
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
        /// <summary>
        /// rqc创建页面提交检验项目
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_jyxm_Add(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string commit_type = jarr.ContainsKey("commit_type") ? jarr["commit_type"].ToString() : "";
                List<rqc_task_detail_t> list = jarr.ContainsKey("rqc_task_detail_t") ? JsonConvert.DeserializeObject<List<rqc_task_detail_t>>(jarr["rqc_task_detail_t"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"UPDATE RQC_TASK_M SET MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE TASK_NO='{task_no}'";
                DB.ExecuteNonQuery(sql);

                string commit_count = DB.GetString($@"select max(commit_index) from rqc_task_detail_t where task_no='{task_no}' and commit_type='{commit_type}'");
                if (string.IsNullOrWhiteSpace(commit_count))
                {
                    commit_count = "0";
                }
                else
                {
                    commit_count = (Convert.ToDecimal(commit_count) + 1).ToString();
                }
                sql = $@"insert into rqc_task_detail_t (task_no,commit_index,commit_type,createby,createdate,createtime) 
                                    values('{task_no}','{commit_count}','{commit_type}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                if (list.Count > 0)
                {
                    foreach (rqc_task_detail_t item in list)
                    {
                        if (item.checkeds.HasValue && item.checkeds.Value == 1)
                        {
                            sql = $@"insert into rqc_task_detail_t_d (task_no,union_id,commit_index,createby,createdate,createtime) 
                                    values('{task_no}','{item.id}','{commit_count}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);

                            sql = $@"delete from rqc_task_detail_t_f where union_id='{item.id}'";
                            DB.ExecuteNonQuery(sql);
                            if (item.imginfo_list.Count > 0)
                            {
                                sql = string.Empty;
                                foreach (imginfo item2 in item.imginfo_list)
                                {
                                    sql += $@"insert into rqc_task_detail_t_f (task_no,union_id,file_guid,createby,createdate,createtime) 
                                        values('{task_no}','{item.id}','{item2.guid}','{user}','{date}','{time}');";

                                }
                                DB.ExecuteNonQuery("begin " + sql + " end;");
                            }

                        }


                    }

                }
               // ret.ErrMsg = "保存成功";
                ret.ErrMsg = "Saved successfully";
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "保存失败，原因是：" + ex.Message;
                ret.ErrMsg = "Save failed because：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 右边数据录入操作
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_Main_Insert_mx(object OBJ)
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

                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string lot_qty = jarr.ContainsKey("lot_qty") ? jarr["lot_qty"].ToString() : "";//批次数量
                string level_code = jarr.ContainsKey("level_code") ? jarr["level_code"].ToString() : "";//检验水平
                string level_type = jarr.ContainsKey("level_type") ? jarr["level_type"].ToString() : "";//aql维护值
                string aql_qty = jarr.ContainsKey("aql_qty") ? jarr["aql_qty"].ToString() : "";//aql抽检数量
                string acre = jarr.ContainsKey("acre") ? jarr["acre"].ToString() : "";//acre
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//合格数量
                string bad_qty = jarr.ContainsKey("bad_qty") ? jarr["bad_qty"].ToString() : "";//不合格数量
                string result = jarr.ContainsKey("result") ? jarr["result"].ToString() : "";//判定结果
                string res_remark = jarr.ContainsKey("res_remark") ? jarr["res_remark"].ToString() : "";//备注
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                string sql = string.Empty;
                //分类的
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = $@"update rqc_task_m set
                            lot_qty='{lot_qty}',
                            commit_remark_qty=nvl(commit_remark_qty,0)+1,
                            level_code='{level_code}',
                            level_type='{level_type}',
                            aql_qty='{aql_qty}',
                            acre='{acre}',
                            qty='{qty}',
                            result='{result}',
                            bad_qty='{bad_qty}',
                            res_remark='{res_remark}',
                            modifyby='{user}',
                            modifydate='{date}',
                            modifytime='{time}'
                 where task_no='{task_no}'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    //ret.ErrMsg = "缺少任务编号,请检查";
                    ret.ErrMsg = "Missing task number, please check";
                    ret.IsSuccess = false;
                    return ret;
                }
                ret.IsSuccess = true;
                //ret.ErrMsg = "保存成功";
                ret.ErrMsg = "Saved successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "保存失败，原因：" + ex.Message;
                ret.ErrMsg = "Save failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// rqc创建页面设置停线或者重新开线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_state(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 关联记录表id
                string task_state = jarr.ContainsKey("task_state") ? jarr["task_state"].ToString() : "";//0开线，1是关线
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update rqc_task_m set task_state='{task_state}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// rqc创建页面实验室样品编号录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_ex_taskadd(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 关联记录表id
                string ex_task_no = jarr.ContainsKey("ex_task_no") ? jarr["ex_task_no"].ToString() : "";//条件 关联记录表id

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = $@"select task_no from qcm_ex_task_list_m where task_no='{ex_task_no}'";
                string ex_task_nos = DB.GetString(sql);
                if (string.IsNullOrWhiteSpace(ex_task_nos))
                {
                    //ret.ErrMsg = $@"无此【{ex_task_no}】实验室编号，请确认";
                    ret.ErrMsg = $@"There is no such [{ex_task_no}] laboratory number, please confirm";
                    ret.IsSuccess = false;
                    return ret;
                }

                sql = $@"update rqc_task_m set ex_task_no='{ex_task_no}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// 结束检验
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject over_inspection(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string isFinish = jarr.ContainsKey("isFinish") ? jarr["isFinish"].ToString() : "";//条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = $@"UPDATE RQC_TASK_M SET MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE TASK_NO='{task_no}'";
                DB.ExecuteNonQuery(sql);
                if (isFinish.ToLower() == "true")
                {
                    sql = $@"update rqc_task_m set task_state='2' where task_no='{task_no}'";
                }
                else
                {
                    sql = $@"update rqc_task_m set task_state='0' where task_no='{task_no}'";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                //ret.ErrMsg = "操作成功";
                ret.ErrMsg = "Successful operation";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                //ret.ErrMsg = "操作失败，原因：" + ex.Message;
                ret.ErrMsg = "Operation failed, reason：" + ex.Message;
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        //Added for Pivot88

        /// <summary>
        /// 同步RQC任务到pivot88
        /// </summary>
        /// <param name="requestObject"></param>
        /// <returns></returns>
        public ResultObject TransferDataToPivot88(RequestObject requestObject)
        {
            RequestObject reqObj = requestObject;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();

            try
            {
                DB = new DataBase(reqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = reqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string test_qty = jarr.ContainsKey("test_qty") ? jarr["test_qty"].ToString() : "";//已检验数量
                string group = jarr.ContainsKey("group") ? jarr["group"].ToString() : "";
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";
                List<rqc_task_detail_t> rqc_detail_list = jarr.ContainsKey("rqc_task_detail_t") ? JsonConvert.DeserializeObject<List<rqc_task_detail_t>>(jarr["rqc_task_detail_t"].ToString()) : null;
                string completeDate = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string completeTime = DateTime.Now.ToString("HH:mm:ss");//时间
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);

                #region 主数据
                string uniqueKey = string.Empty;
                try
                {
                    //从序列获取id
                    string id = DB.GetString("SELECT T_AEQS_TO_P88_LIST_ID_SEQ.nextval FROM dual");
                    uniqueKey = "apache5_" + id.PadLeft(3, '0');
                }
                catch (Exception ex)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = ex.Message;
                    return ret;
                }
                //总不良数
                int defective_parts = DB.GetInt32($@"select count(union_id) from rqc_task_detail_t_d d inner join rqc_task_item_c c on d.union_id = c.id inner join  bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code
                                                     where d.task_no = '{task_no}' and m.bad_item_code is not null and m.bad_item_name is not null and m.qc_type = 2");


                //表头
                AEQS_TO_P88_LIST aeqsMain = new AEQS_TO_P88_LIST();
                aeqsMain.status = "Submitted";
                aeqsMain.defective_parts = defective_parts;
                DateTime startTime = DB.GetDateTime($"select concat(concat(createdate,' '),createtime) from rqc_task_m where task_no = '{task_no}'");
                aeqsMain.date_started = startTime;

                aeqsMain.passfails_0_title = "inspected_carton_numbers";
                aeqsMain.passfails_0_type = "list";
                aeqsMain.passfails_0_listvalues_value = "N/A";
                aeqsMain.passfails_0_subsection = "actual_inspection";
                aeqsMain.assignment_items_assignment_report_type_id = 27;
                aeqsMain.assignment_items_fields_string_12 = "C2B (aSC)";


                //如果已检验双数为0,不传
                if (Convert.ToInt32(test_qty) == 0)
                {
                    ret.IsSuccess = false;
                    return ret;
                }

                string sql = $@"insert into t_aeqs_to_p88_list
                                    (
                                    unique_key,
                                    status,
                                    date_started,
                                    defective_parts,
                                    assignment_items_fields_string_12,
                                    assignment_items_assignment_report_type_id,
                                    passfails_0_title,
                                    passfails_0_type,
                                    passfails_0_subsection,
                                    passfails_0_listvalues_value)
                                values
                                    (
                                    '{uniqueKey}',
                                    '{aeqsMain.status}',
                                    to_date('{aeqsMain.date_started}','yyyy-mm-dd hh24:mi:ss'),
                                    '{aeqsMain.defective_parts}',
                                    '{aeqsMain.assignment_items_fields_string_12}',
                                    '{aeqsMain.assignment_items_assignment_report_type_id}',
                                    '{aeqsMain.passfails_0_title}',
                                    '{aeqsMain.passfails_0_type}',
                                    '{aeqsMain.passfails_0_subsection}',
                                    '{aeqsMain.passfails_0_listvalues_value}'
                                    )";
                DB.ExecuteNonQuery(sql);
                #endregion

                #region 包装
                string packageId = string.Empty;
                try
                {
                    //从序列获取id
                    packageId = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                }
                catch (Exception ex)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = ex.Message;
                    return ret;
                }
                AEQS_TO_P88_SECTIONS aeqsPackage = new AEQS_TO_P88_SECTIONS();
                aeqsPackage.sections_type = "aqlDefects";
                aeqsPackage.sections_title = "packing_packaging_labelling";
                aeqsPackage.sections_result_id = 1;//1:pass
                ////取PO所有码数订单量。
                //aeqsPackage.sections_qty_inspected = DB.GetDecimal($@"select sum(se_qty) as se_qty
                //                                                        from bdm_se_order_size 
                //                                                        where se_id in (select se_id from bdm_se_order_master where mer_po = '{po}')");
                aeqsPackage.sections_qty_inspected = Convert.ToDecimal(test_qty);
                aeqsPackage.sections_sampled_inspected = Convert.ToDecimal(test_qty);
                aeqsPackage.sections_defective_parts = 0;
                aeqsPackage.sections_inspection_level = "100%inspection";
                aeqsPackage.sections_inspection_method = "normal";
                aeqsPackage.sections_aql_minor = 4.0m;
                aeqsPackage.sections_aql_major = 2.5m;
                aeqsPackage.sections_aql_critical = 1.0m;
                aeqsPackage.sections_max_minor_defects = 0;
                aeqsPackage.sections_max_major_defects = 0;
                aeqsPackage.sections_max_critical_defects = 0;

                sql = $@"insert into T_AEQS_TO_P88_SECTIONS
                                  (ID,
                                   UNION_ID,
                                   SECTIONS_TYPE,
                                   SECTIONS_TITLE,
                                   SECTIONS_RESULT_ID,
                                   SECTIONS_QTY_INSPECTED,
                                   SECTIONS_SAMPLED_INSPECTED,
                                   SECTIONS_DEFECTIVE_PARTS,
                                   SECTIONS_INSPECTION_LEVEL,
                                   SECTIONS_INSPECTION_METHOD,
                                   SECTIONS_AQL_MINOR,
                                   SECTIONS_AQL_MAJOR,
                                   SECTIONS_AQL_CRITICAL,
                                   SECTIONS_BARCODES_VALUE,
                                   SECTIONS_QTY_TYPE,
                                   SECTIONS_MAX_MINOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_A_DEFECTS,
                                   SECTIONS_MAX_MAJOR_B_DEFECTS,
                                   SECTIONS_MAX_CRITICAL_DEFECTS,
                                   SECTIONS_DEFECTS_LABEL,
                                   SECTIONS_DEFECTS_SUBSECTION,
                                   SECTIONS_DEFECTS_CODE,
                                   SECTIONS_DEFECTS_CRITICAL_LEVEL,
                                   SECTIONS_DEFECTS_MAJOR_LEVEL,
                                   SECTIONS_DEFECTS_MINOR_LEVEL,
                                   SECTIONS_DEFECTS_COMMENTS)
                                values
                                  ('{packageId}',
                                   '{uniqueKey}',
                                   '{aeqsPackage.sections_type}',
                                   '{aeqsPackage.sections_title}',
                                   '{aeqsPackage.sections_result_id}',
                                   '{aeqsPackage.sections_qty_inspected}',
                                    {aeqsPackage.sections_sampled_inspected},
                                    {aeqsPackage.sections_defective_parts},
                                   '{aeqsPackage.sections_inspection_level}',
                                   '{aeqsPackage.sections_inspection_method}',
                                   '{aeqsPackage.sections_aql_minor}',
                                   '{aeqsPackage.sections_aql_major}',
                                   '{aeqsPackage.sections_aql_critical}',
                                   '{aeqsPackage.sections_barcodes_value}',
                                   '{aeqsPackage.sections_qty_type}',
                                   '{aeqsPackage.sections_max_minor_defects}',
                                   '{aeqsPackage.sections_max_major_defects}',
                                   '{aeqsPackage.sections_max_major_a_defects}',
                                   '{aeqsPackage.sections_max_major_b_defects}',
                                   '{aeqsPackage.sections_max_critical_defects}',
                                   '{aeqsPackage.sections_defects_label}',
                                   '{aeqsPackage.sections_defects_subsection}',
                                   '{aeqsPackage.sections_defects_code}',
                                   '{aeqsPackage.sections_defects_critical_level}',
                                   '{aeqsPackage.sections_defects_major_level}',
                                   '{aeqsPackage.sections_defects_minor_level}',
                                   '{aeqsPackage.sections_defects_comments}')";
                DB.ExecuteNonQuery(sql);

                #endregion 包装


                #region 产品  
                string productId = string.Empty;
                sql = string.Empty;

                //产品,只传输与P88不良项目有对应关系的项目,且如有多个不良项对应一个p88不良项，则以一个p88不良项传输，不良项数量相加。
                string productSql = $@"with bad_item_table as(
                                            select c.inspection_name,count(union_id) as badnum ,m.bad_item_code,m.bad_item_name,s.bad_classify_code,y.bad_classify_name,m.problem_level
                                            from rqc_task_detail_t_d d inner join rqc_task_item_c c on d.union_id = c.id 
                                            left join bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code 
                                            left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code 
                                            left join bdm_aql_bad_classify y on s.bad_classify_code = y.bad_classify_code
                                            where d.task_no = '{task_no}' and m.bad_item_code is not null and m.bad_item_name is not null and m.qc_type = 2
                                            group by c.inspection_code,c.inspection_name, m.problem_level,m.bad_item_code,m.bad_item_name,s.bad_classify_code,m.problem_level,y.bad_classify_name
                                          )
                                          select listagg( distinct inspection_name,',') as inspection_name,bad_item_code,bad_item_name,bad_classify_code,bad_classify_name,problem_level,sum(badnum) as badnum from bad_item_table
                                          group by bad_item_code,bad_item_name,bad_classify_code,bad_classify_name,problem_level";

                DataTable productTable = DB.GetDataTable(productSql);



                //严重，主要，次要最大允收数
                int max_critical_defects = 0;
                int max_major_defects = 0;
                int max_minor_defects = 0;

                //总检验结果：问题点总数（与p88有关联的问题点）/已检验双数*100%<=15%时传“1=Pass ”，否则传“2=Fail”
                int total_inspection_result = 0;
                if (Convert.ToDouble(defective_parts) / Convert.ToDouble(test_qty) * 100 <= 15)
                {
                    total_inspection_result = 1;//Pass
                }
                else
                {
                    total_inspection_result = 2;//Fail
                }

                //产品结果
                int productResultId = total_inspection_result;

                // 计算允收数
                string aqlSql = $@"select AC11,AC13,AC14,vals from (
                                            select AC11,AC13,AC14,vals  from BDM_AQL_M 
                                            where HORI_TYPE='2' and LEVEL_TYPE='2' and vals <= CAST('{test_qty}' AS int )
                                            order by CAST(vals AS int ) desc ) where rownum = 1";
                DataTable dataTableAC = DB.GetDataTable(aqlSql);
                if (dataTableAC.Rows.Count > 0)
                {
                    max_critical_defects = Convert.ToInt32(dataTableAC.Rows[0]["AC11"].ToString());//严重
                    max_major_defects = Convert.ToInt32(dataTableAC.Rows[0]["AC13"].ToString());//主要
                    max_minor_defects = Convert.ToInt32(dataTableAC.Rows[0]["AC14"].ToString());//次要
                }

                if (productTable.Rows.Count > 0)
                {


                    //总次要/主要/严重不良数
                    int minorLevel = DB.GetInt32($@"select count(*) from rqc_task_detail_t_d d inner join rqc_task_item_c c on d.union_id = c.id inner join  bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code
                                        where d.task_no = '{task_no}' and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '次要'");
                    int majorLevel = DB.GetInt32($@"select count(*) from rqc_task_detail_t_d d inner join rqc_task_item_c c on d.union_id = c.id inner join  bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code
                                        where d.task_no = '{task_no}' and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '主要'");
                    int criticalLevel = DB.GetInt32($@"select count(*) from rqc_task_detail_t_d d inner join rqc_task_item_c c on d.union_id = c.id inner join  bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code
                                        where d.task_no = '{task_no}' and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '严重'");


                    //产品检验结果 ，对比允收数与实际不良数的结果。
                    //if (minorLevel > max_minor_defects || majorLevel > max_major_defects || criticalLevel > max_critical_defects)
                    //{
                    //    productResultId = 2;//Fail
                    //}
                    //else
                    //{
                    //    productResultId = 1;//Pass
                    //}

                    //插入数据
                    foreach (DataRow dr in productTable.Rows)
                    {
                        AEQS_TO_P88_SECTIONS aeqsProduct = new AEQS_TO_P88_SECTIONS();
                        try
                        {
                            //从序列获取id
                            productId = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                        }
                        catch (Exception ex)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = ex.Message;
                            return ret;
                        }

                        aeqsProduct.sections_type = "aqlDefects";
                        aeqsProduct.sections_title = "product";
                        aeqsProduct.sections_defective_parts = defective_parts;
                        //aeqsProduct.sections_qty_inspected = DB.GetDecimal($@"select sum(se_qty) as se_qty from bdm_se_order_size where se_id in (select se_id from bdm_se_order_master where mer_po = '{po}')");
                        aeqsProduct.sections_qty_inspected = Convert.ToDecimal(test_qty);
                        aeqsProduct.sections_sampled_inspected = Convert.ToInt32(test_qty);
                        aeqsProduct.sections_inspection_level = "100%inspection";
                        aeqsProduct.sections_inspection_method = "normal";
                        aeqsProduct.sections_aql_minor = 4.0m;
                        aeqsProduct.sections_aql_major = 2.5m;
                        aeqsProduct.sections_aql_critical = 1.0m;
                        aeqsProduct.sections_defects_label = dr["bad_item_name"].ToString();
                        aeqsProduct.sections_defects_subsection = dr["bad_classify_name"].ToString();
                        aeqsProduct.sections_defects_code = "FTW" + dr["bad_item_code"].ToString();
                        aeqsProduct.sections_max_critical_defects = max_critical_defects;
                        aeqsProduct.sections_max_major_defects = max_major_defects;
                        aeqsProduct.sections_max_minor_defects = max_minor_defects;
                        aeqsProduct.sections_result_id = productResultId;
                        if (dr["problem_level"].ToString().Equals("主要"))
                        {
                            aeqsProduct.sections_defects_major_level = Convert.ToInt32(dr["badnum"].ToString());
                        }
                        else if (dr["problem_level"].ToString().Equals("次要"))
                        {
                            aeqsProduct.sections_defects_minor_level = Convert.ToInt32(dr["badnum"].ToString());
                        }
                        else if (dr["problem_level"].ToString().Equals("严重"))
                        {
                            aeqsProduct.sections_defects_critical_level = Convert.ToInt32(dr["badnum"].ToString());
                        }

                        sql += $@"insert into T_AEQS_TO_P88_SECTIONS
                                  (ID,
                                   UNION_ID,
                                   SECTIONS_TYPE,
                                   SECTIONS_TITLE,
                                   SECTIONS_RESULT_ID,
                                   SECTIONS_QTY_INSPECTED,
                                   SECTIONS_SAMPLED_INSPECTED,
                                   SECTIONS_DEFECTIVE_PARTS,
                                   SECTIONS_INSPECTION_LEVEL,
                                   SECTIONS_INSPECTION_METHOD,
                                   SECTIONS_AQL_MINOR,
                                   SECTIONS_AQL_MAJOR,
                                   SECTIONS_AQL_CRITICAL,
                                   SECTIONS_BARCODES_VALUE,
                                   SECTIONS_QTY_TYPE,
                                   SECTIONS_MAX_MINOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_A_DEFECTS,
                                   SECTIONS_MAX_MAJOR_B_DEFECTS,
                                   SECTIONS_MAX_CRITICAL_DEFECTS,
                                   SECTIONS_DEFECTS_LABEL,
                                   SECTIONS_DEFECTS_SUBSECTION,
                                   SECTIONS_DEFECTS_CODE,
                                   SECTIONS_DEFECTS_CRITICAL_LEVEL,
                                   SECTIONS_DEFECTS_MAJOR_LEVEL,
                                   SECTIONS_DEFECTS_MINOR_LEVEL,
                                   SECTIONS_DEFECTS_COMMENTS)
                                values
                                  ('{productId}',
                                   '{uniqueKey}',
                                   '{aeqsProduct.sections_type}',
                                   '{aeqsProduct.sections_title}',
                                   '{aeqsProduct.sections_result_id}',
                                   '{aeqsProduct.sections_qty_inspected}',
                                    {aeqsProduct.sections_sampled_inspected},
                                    {aeqsProduct.sections_defective_parts},
                                   '{aeqsProduct.sections_inspection_level}',
                                   '{aeqsProduct.sections_inspection_method}',
                                   '{aeqsProduct.sections_aql_minor}',
                                   '{aeqsProduct.sections_aql_major}',
                                   '{aeqsProduct.sections_aql_critical}',
                                   '{aeqsProduct.sections_barcodes_value}',
                                   '{aeqsProduct.sections_qty_type}',
                                   '{aeqsProduct.sections_max_minor_defects}',
                                   '{aeqsProduct.sections_max_major_defects}',
                                   '{aeqsProduct.sections_max_major_a_defects}',
                                   '{aeqsProduct.sections_max_major_b_defects}',
                                   '{aeqsProduct.sections_max_critical_defects}',
                                   '{aeqsProduct.sections_defects_label}',
                                   '{aeqsProduct.sections_defects_subsection}',
                                   '{aeqsProduct.sections_defects_code}',
                                   '{aeqsProduct.sections_defects_critical_level}',
                                   '{aeqsProduct.sections_defects_major_level}',
                                   '{aeqsProduct.sections_defects_minor_level}',
                                   '{aeqsProduct.sections_defects_comments}');";


                        //产品图片(相同项目的图片也要合并计算。例如不良项1 ，不良项2对应的都是一个p88不良，那么它们合并为一个p88不良时相同的图片也要合并过去。)
                        int index = 1;
                        string imgsql = string.Empty;
                        //拿到p88项目中包含的检验项
                        string[] inspectionList = dr["inspection_name"].ToString().Split(",");
                        foreach (var inpsection in inspectionList)
                        {
                            foreach (rqc_task_detail_t item in rqc_detail_list)
                            {
                                if (item.inspection_name == inpsection && item.imginfo_list.Count > 0)
                                {
                                    foreach (imginfo img in item.imginfo_list)
                                    {
                                        AEQS_TO_P88_SECTIONS_F productPicture = new AEQS_TO_P88_SECTIONS_F();
                                        productPicture.sections_defects_pictures_title = "IMAGE " + index;
                                        productPicture.sections_defects_pictures_full_filename = img.guid;
                                        productPicture.sections_defects_pictures_number = index;
                                        imgsql += $@"insert into T_aeqs_to_p88_sections_f (UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, sections_defects_pictures_full_filename, sections_defects_pictures_number, sections_defects_pictures_comment)
                                               values ('{productId}', '{productPicture.sections_defects_pictures_title}', '{productPicture.sections_defects_pictures_full_filename}','{productPicture.sections_defects_pictures_number}', '{productPicture.sections_defects_pictures_comment}');";
                                        index++;
                                    }
                                    break;//当前项目图片添加完毕立刻退出循环
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(imgsql))
                        {
                            DB.ExecuteNonQuery("begin " + imgsql + " end;");
                        }
                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }
                else //产品没有不良项时也需要传一行产品的数据（合格）。
                {

                    AEQS_TO_P88_SECTIONS aeqsProduct = new AEQS_TO_P88_SECTIONS();
                    try
                    {
                        //从序列获取id
                        productId = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                    }
                    catch (Exception ex)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = ex.Message;
                        return ret;
                    }

                    productResultId = 1;//pass
                    aeqsProduct.sections_type = "aqlDefects";
                    aeqsProduct.sections_title = "product";
                    aeqsProduct.sections_defective_parts = defective_parts;
                    aeqsProduct.sections_qty_inspected = Convert.ToDecimal(test_qty);
                    aeqsProduct.sections_sampled_inspected = Convert.ToInt32(test_qty);
                    aeqsProduct.sections_inspection_level = "100%inspection";
                    aeqsProduct.sections_inspection_method = "normal";
                    aeqsProduct.sections_aql_minor = 4.0m;
                    aeqsProduct.sections_aql_major = 2.5m;
                    aeqsProduct.sections_aql_critical = 1.0m;
                    aeqsProduct.sections_max_critical_defects = max_critical_defects;
                    aeqsProduct.sections_max_major_defects = max_major_defects;
                    aeqsProduct.sections_max_minor_defects = max_minor_defects;
                    aeqsProduct.sections_result_id = productResultId;

                    sql = $@"insert into T_AEQS_TO_P88_SECTIONS
                                  (ID,
                                   UNION_ID,
                                   SECTIONS_TYPE,
                                   SECTIONS_TITLE,
                                   SECTIONS_RESULT_ID,
                                   SECTIONS_QTY_INSPECTED,
                                   SECTIONS_SAMPLED_INSPECTED,
                                   SECTIONS_DEFECTIVE_PARTS,
                                   SECTIONS_INSPECTION_LEVEL,
                                   SECTIONS_INSPECTION_METHOD,
                                   SECTIONS_AQL_MINOR,
                                   SECTIONS_AQL_MAJOR,
                                   SECTIONS_AQL_CRITICAL,
                                   SECTIONS_BARCODES_VALUE,
                                   SECTIONS_QTY_TYPE,
                                   SECTIONS_MAX_MINOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_DEFECTS,
                                   SECTIONS_MAX_MAJOR_A_DEFECTS,
                                   SECTIONS_MAX_MAJOR_B_DEFECTS,
                                   SECTIONS_MAX_CRITICAL_DEFECTS,
                                   SECTIONS_DEFECTS_LABEL,
                                   SECTIONS_DEFECTS_SUBSECTION,
                                   SECTIONS_DEFECTS_CODE,
                                   SECTIONS_DEFECTS_CRITICAL_LEVEL,
                                   SECTIONS_DEFECTS_MAJOR_LEVEL,
                                   SECTIONS_DEFECTS_MINOR_LEVEL,
                                   SECTIONS_DEFECTS_COMMENTS)
                                values
                                  ('{productId}',
                                   '{uniqueKey}',
                                   '{aeqsProduct.sections_type}',
                                   '{aeqsProduct.sections_title}',
                                   '{aeqsProduct.sections_result_id}',
                                   '{aeqsProduct.sections_qty_inspected}',
                                    {aeqsProduct.sections_sampled_inspected},
                                    {aeqsProduct.sections_defective_parts},
                                   '{aeqsProduct.sections_inspection_level}',
                                   '{aeqsProduct.sections_inspection_method}',
                                   '{aeqsProduct.sections_aql_minor}',
                                   '{aeqsProduct.sections_aql_major}',
                                   '{aeqsProduct.sections_aql_critical}',
                                   '{aeqsProduct.sections_barcodes_value}',
                                   '{aeqsProduct.sections_qty_type}',
                                   '{aeqsProduct.sections_max_minor_defects}',
                                   '{aeqsProduct.sections_max_major_defects}',
                                   '{aeqsProduct.sections_max_major_a_defects}',
                                   '{aeqsProduct.sections_max_major_b_defects}',
                                   '{aeqsProduct.sections_max_critical_defects}',
                                   '{aeqsProduct.sections_defects_label}',
                                   '{aeqsProduct.sections_defects_subsection}',
                                   '{aeqsProduct.sections_defects_code}',
                                   '{aeqsProduct.sections_defects_critical_level}',
                                   '{aeqsProduct.sections_defects_major_level}',
                                   '{aeqsProduct.sections_defects_minor_level}',
                                   '{aeqsProduct.sections_defects_comments}')";
                    DB.ExecuteNonQuery(sql);

                }
                #endregion 产品

                #region assignment部分


                if (!string.IsNullOrEmpty(uniqueKey))
                {

                    string art = DB.GetString($@"select prod_no from rqc_task_m  where task_no = '{task_no}'");

                    //商品名稱
                    string itemName = DB.GetString($@"select s.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '{art}'");
                    if (string.IsNullOrEmpty(itemName))
                    {
                        itemName = DB.GetString($@"select p.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '{art}'");
                    }

                    AEQS_TO_P88_ASSIGNMENT aeqsAssignment = new AEQS_TO_P88_ASSIGNMENT();
                    aeqsAssignment.assignment_items_sampled_inspected = Convert.ToDecimal(test_qty);
                    aeqsAssignment.assignment_items_inspection_completed_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                    TimeSpan ts = aeqsAssignment.assignment_items_inspection_completed_date - aeqsMain.date_started;
                    aeqsAssignment.assignment_items_total_inspection_minutes = Math.Round(Convert.ToDecimal(ts.TotalMinutes)); 
                    aeqsAssignment.assignment_items_sampling_size = Convert.ToDecimal(test_qty);
                    aeqsAssignment.assignment_items_aql_minor = 4.0m;
                    aeqsAssignment.assignment_items_aql_major = 2.5m;
                    aeqsAssignment.assignment_items_aql_critical = 1.0m;

                    aeqsAssignment.assignment_items_assignment_inspector_username = DB.GetString($"select p88_checker from t_aeqs_to_p88_inspector where checker_code = '{group}'");
                    aeqsAssignment.assignment_items_assignment_date_inspection = startTime;
                    aeqsAssignment.assignment_items_assignment_inspection_level = "100%inspection";
                    aeqsAssignment.assignment_items_assignment_inspection_method = "normal";
                    aeqsAssignment.assignment_items_assignment_report_type_id = 27;
                    aeqsAssignment.assignment_items_po_line_po_exporter_id = 233; //"219 APE 233 APC
                    aeqsAssignment.assignment_items_po_line_po_exporter_erp_business_id = "011";
                    aeqsAssignment.assignment_items_po_line_po_number = po;
                    aeqsAssignment.assignment_items_po_line_importer_id = 215;
                    aeqsAssignment.assignment_items_po_line_importer_erp_business_id = "Adidas001";
                    aeqsAssignment.assignment_items_po_line_importer_project_id = 2062;
                    aeqsAssignment.assignment_items_po_line_sku_item_name = itemName;
                    //aeqsAssignment.assignment_items_inspection_result_id = productResultId;
                    //aeqsAssignment.assignment_items_inspection_status_id = aeqsAssignment.assignment_items_inspection_result_id == 1 ? 3 : 1;//检验结果为pass时，传“3=已接受”,检验结果为fail时，传“1=待审批”

                    //总检验结果：问题点总数（与p88有关联的问题点）/已检验双数*100%<=15%时传“1=Pass ”，否则传“2=Fail”
                    //if (Convert.ToDouble(defective_parts) / Convert.ToDouble(test_qty) * 100 <= 15)
                    //{
                    //    aeqsAssignment.assignment_items_inspection_result_id = 1;//Pass
                    //    aeqsAssignment.assignment_items_inspection_status_id = 3;//已接受
                    //}
                    //else
                    //{
                    //    aeqsAssignment.assignment_items_inspection_result_id = 2;//fail
                    //    aeqsAssignment.assignment_items_inspection_status_id = 1;//待审批
                    //}
                    if (total_inspection_result == 1)
                    {
                        aeqsAssignment.assignment_items_inspection_result_id = 1;//Pass
                        aeqsAssignment.assignment_items_inspection_status_id = 3;//已接受
                    }
                    else if (total_inspection_result == 2)
                    {
                        aeqsAssignment.assignment_items_inspection_result_id = 2;//fail
                        aeqsAssignment.assignment_items_inspection_status_id = 1;//待审批
                    }

                    #region before PO change 2 on 20250218
                    //All the codes of this task PO are combined in the "ART_SIZE" format. The converted code of SIZE is taken from base097m
                    //DataTable sizeTable = DB.GetDataTable($@"select max('{art}_' || b.ad_size || '_' || a.po_seq) as ad_size,sum(a.se_qty) as se_qty
                    //                                           from bdm_se_order_size a
                    //                                           left join base097m b on a.size_no = b.FACTORY_SIZE
                    //                                           where se_id in(
                    //                                                  select se_id from bdm_se_order_master where mer_po = '{po}'
                    //                                           )
                    //                                           and a.se_qty >0
                    //                                           group by ad_size,a.se_qty
                    //                                           order by dbms_random.value()");//Edit on 6/28(PO Change)

                    #endregion

                    #region After PO change 2 on 20250218
                    DataTable sizeTable = null;
                    //Non-merged orders (multiple size lines, or merged orders) follow the old logic
                    string merge_mark = DB.GetString($@"select so_mergr_mark from bdm_se_order_master where mer_po = '{po}'");
                    if (merge_mark.Equals("Y"))
                    {
                        //All the codes of this task PO are combined in the "ART_SIZE" format. The converted code of SIZE is taken from base097m. The SKU format after PO change becomes ART_SIZE_LineItem
                        sizeTable = DB.GetDataTable($@"select max('{art}_' || b.ad_size ) as ad_size,
                                                       sum(r.PO_LINE_QTY) as se_qty,
                                                       max(r.po_line_item) as po_seq
                                                       from bdm_se_order_size a
                                                       left join t_bdm_se_order_reference r on a.se_id = r.se_id and a.cr_size = r.po_size --and a.po_line_item = r.po_line_item
                                                       left join base097m b on a.size_no = b.FACTORY_SIZE
                                                       where a.se_id in(
                                                              select se_id from bdm_se_order_master where mer_po = '{po}'
                                                       )
                                                       and a.se_qty >0
                                                       group by ad_size,a.se_qty,r.po_line_item
                                                       order by dbms_random.value()");
                    }
                    else
                    {
                        //All the codes of this task PO are combined in the "ART_SIZE" format. The converted code of SIZE is taken from base097m. The SKU format after PO change becomes ART_SIZE_LineItem
                        sizeTable = DB.GetDataTable($@"select max('{art}_' || b.ad_size ) as ad_size,
                                                               sum(a.se_qty) as se_qty,
                                                               max(po_seq) as po_seq
                                                               from bdm_se_order_size a
                                                               left join base097m b on a.size_no = b.FACTORY_SIZE
                                                               where se_id in(
                                                                      select se_id from bdm_se_order_master where mer_po = '{po}'
                                                               )
                                                               and a.se_qty >0
                                                               group by ad_size,a.se_qty
                                                               order by dbms_random.value()"); //Edit on 6/28(PO Change)
                    }
                    #endregion

                    if (sizeTable.Rows.Count > 0)
                    {
                        #region Extraction code
                        //Randomly draw the code number based on the checked even number. When the checked even number is <= 10, randomly draw a code number (the order quantity of the code number needs to be greater than the sampling number allocated to the code number).
                        //When 10 < the checked even number <= 20, draw two codes and divide them equally. If there are not enough to be allocated, randomly draw the third code number, and so on.
                        //When the checked even number is >20, randomly draw three codes and divide them equally. If there are not enough to be allocated, draw the fourth code number, and so on.

                        Dictionary<int, string> pairs = new Dictionary<int, string>();
                        List<ShoeSize> shoeList = new List<ShoeSize>();
                        int sampleCount = 0;//Number of draws
                        int sampleQty = Convert.ToInt32(test_qty);//Sampling quantity (inspected quantity)

                        //1. Determine how many pairs you want to draw in total
                        if (sampleQty <= 10)
                        {
                            int se_qty = Convert.ToInt32(sizeTable.Rows[0]["se_qty"].ToString());
                            if (se_qty < sampleQty)//If the order quantity of a code number is insufficient, you need to continue to draw code numbers until the total order quantity is greater than the inspected quantity.
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    sampleCount++;
                                    int seQtys = 0;
                                    for (int i = 0; i <= sampleCount; i++)
                                    {
                                        seQtys += Convert.ToInt32(sizeTable.Rows[sampleCount]["se_qty"].ToString());
                                    }
                                    if (seQtys >= sampleQty)
                                    {
                                        flag = false;
                                    }
                                }
                            }
                            else
                            {
                                sampleCount = 0;//Draw a code
                            }
                        }
                        else if (sampleQty > 10 && sampleQty <= 20)
                        {
                            int se_qty = Convert.ToInt32(sizeTable.Rows[0]["se_qty"].ToString());
                            int se_qty2 = Convert.ToInt32(sizeTable.Rows[1]["se_qty"].ToString());

                            if ((se_qty + se_qty2) < sampleQty)//If the order quantity of two code numbers is insufficient, it is necessary to continue to draw code numbers until the total order quantity is greater than the inspected quantity.
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    sampleCount++;
                                    int seQtys = 0;
                                    for (int i = 0; i <= sampleCount; i++)
                                    {
                                        seQtys += Convert.ToInt32(sizeTable.Rows[sampleCount]["se_qty"].ToString());
                                    }
                                    if (seQtys >= sampleQty)
                                    {
                                        flag = false;
                                    }
                                }
                            }
                            else
                            {
                                sampleCount = 1;//Draw two numbers 0,1
                            }
                        }
                        else if (sampleQty > 20)
                        {
                            int se_qty = Convert.ToInt32(sizeTable.Rows[0]["se_qty"].ToString());
                            int se_qty2 = Convert.ToInt32(sizeTable.Rows[1]["se_qty"].ToString());
                            int se_qty3 = Convert.ToInt32(sizeTable.Rows[2]["se_qty"].ToString());

                            int seQtys = se_qty + se_qty2 + se_qty3;
                            sampleCount = 2;//抽三个码数 0,1，2

                            if (seQtys <= sampleQty)//如果三个码数的订单数量不够，需要再继续抽新的码数，直到总订单数大于已检验数量。
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    seQtys += Convert.ToInt32(sizeTable.Rows[sampleCount++]["se_qty"].ToString());
                                    if (seQtys >= sampleQty)
                                    {
                                        flag = false;
                                    }
                                }
                            }
                        }

                        //2.平均分配数量给每一双码数，一双直接分，多双平均分。
                        int sampleQty2 = sampleQty;
                        if (sampleCount == 0)
                        {
                            ShoeSize shoeSize = new ShoeSize();
                            shoeSize.assign_qty = sampleQty2;

                            shoeSize.size = sizeTable.Rows[sampleCount]["ad_size"].ToString();
                            shoeList.Add(shoeSize);
                            //pairs.Add(sampleQty2, sizeTable.Rows[sampleCount]["ad_size"].ToString());
                        }
                        else
                        {
                            int avgNumInt = sampleQty2 / (sampleCount + 1);//取平均数的整数
                            int remainNum = sampleQty2 % (sampleCount + 1);//模运算，取余数
                            int extraNum = 0;//未分配完的数量

                            for (int i = 0; i <= sampleCount; i++)
                            {
                                int seNum = Convert.ToInt32(sizeTable.Rows[i]["se_qty"].ToString());

                                //如果码数订单数大于等于平均数，则分配平均数
                                if (seNum >= avgNumInt)
                                {
                                    //如果上一个码数没分完，且订单数足够，则一起分到这个码数
                                    if (extraNum > 0 && seNum >= avgNumInt + extraNum)
                                    {
                                        ShoeSize shoeSize = new ShoeSize();
                                        shoeSize.assign_qty = avgNumInt + extraNum;
                                        shoeSize.se_qty = seNum;
                                        shoeSize.size = sizeTable.Rows[i]["ad_size"].ToString();
                                        shoeList.Add(shoeSize);
                                        extraNum = 0;
                                    }
                                    else
                                    {
                                        ShoeSize shoeSize = new ShoeSize();
                                        shoeSize.assign_qty = avgNumInt;
                                        shoeSize.se_qty = seNum;
                                        shoeSize.size = sizeTable.Rows[i]["ad_size"].ToString();
                                        shoeList.Add(shoeSize);
                                    }
                                }
                                //码数订单数不足平均数，只分配改码数的最大订单数，同时将没分完的数量保存传递给下一个码数
                                else
                                {
                                    ShoeSize shoeSize = new ShoeSize();
                                    shoeSize.assign_qty = seNum;
                                    shoeSize.se_qty = seNum;
                                    shoeSize.size = sizeTable.Rows[i]["ad_size"].ToString();
                                    shoeList.Add(shoeSize);
                                    extraNum += avgNumInt - seNum;
                                }
                            }

                            //3.分配余数和剩余数量（）
                            if (extraNum > 0)
                            {
                                remainNum += extraNum;
                            }
                            if (remainNum > 0)
                            {
                                bool flag = true;
                                while (flag)
                                {
                                    for (int i = 0; i < shoeList.Count; i++)
                                    {
                                        if (remainNum > 0)
                                        {
                                            if (shoeList[i].assign_qty < shoeList[i].se_qty)
                                            {
                                                shoeList[i].assign_qty++;
                                                remainNum--;
                                            }
                                        }
                                        else
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                }

                            }


                        }
                        #endregion


                        //Project code: PO The changed orders are all FTWTRANS4M
                        string project_code = string.Empty;
                        string report_type_name = string.Empty;
                        if (IsOrderNewFormat(po, DB))
                        {
                            project_code = "FTWTRANS4M";
                            report_type_name = "FTW - Inline";
                            aeqsAssignment.assignment_items_po_line_po_number = DB.GetString($@"select customer_po from bdm_se_order_master where mer_po = '{po}'");
                        }

                        //DataRow[] rows = sizeTable.Select()
                        //插入数据
                        foreach (DataRow dr in sizeTable.Rows)
                        {
                            foreach (ShoeSize item in shoeList)
                            {
                                if (dr["ad_size"].ToString().Equals(item.size))
                                {
                                    aeqsAssignment.assignment_items_po_line_sku_sku_number = dr["ad_size"].ToString();
                                    //aeqsAssignment.assignment_items_qty_inspected = Convert.ToDecimal(dr["se_qty"].ToString());
                                    //aeqsAssignment.assignment_items_qty_to_inspect = Convert.ToDecimal(dr["se_qty"].ToString());
                                    aeqsAssignment.assignment_items_qty_inspected = item.assign_qty;
                                    aeqsAssignment.assignment_items_qty_to_inspect = item.assign_qty;
                                    aeqsAssignment.assignment_items_po_line_qty = Convert.ToDecimal(dr["se_qty"].ToString());

                                    aeqsAssignment.assignment_items_sampling_size = item.assign_qty;
                                    aeqsAssignment.assignment_items_sampled_inspected = item.assign_qty;

                                    sql = $@"insert into t_aeqs_to_p88_assignment
                                    (
                                    union_id,
                                    assignment_items_sampled_inspected,
                                    assignment_items_inspection_result_id,
                                    assignment_items_inspection_status_id,
                                    assignment_items_qty_inspected,
                                    assignment_items_inspection_completed_date,
                                    assignment_items_total_inspection_minutes,
                                    assignment_items_sampling_size,
                                    assignment_items_qty_to_inspect,
                                    assignment_items_aql_minor,
                                    assignment_items_aql_major,
                                    assignment_items_aql_major_a,
                                    assignment_items_aql_major_b,
                                    assignment_items_aql_critical,
                                    assignment_items_supplier_booking_msg,
                                    assignment_items_conclusion_remarks,
                                    assignment_items_assignment_inspector_username,
                                    assignment_items_assignment_date_inspection,
                                    assignment_items_assignment_inspection_level,
                                    assignment_items_assignment_inspection_method,
                                    assignment_items_po_line_po_exporter_id,
                                    assignment_items_po_line_po_exporter_erp_business_id,
                                    assignment_items_po_line_po_number,
                                    assignment_items_po_line_importer_id,
                                    assignment_items_po_line_importer_erp_business_id,
                                    assignment_items_po_line_importer_project_id,
                                    assignment_items_po_line_sku_sku_number,
                                    assignment_items_po_line_sku_item_name,
                                    assignment_items_assignment_report_type_id,
                                    assignment_items_po_line_qty,
                                    po_line_project_code,
                                    report_type_name)
                                values
                                    (
                                    '{uniqueKey}',
                                    '{aeqsAssignment.assignment_items_sampled_inspected}',
                                    '{aeqsAssignment.assignment_items_inspection_result_id}',
                                    '{aeqsAssignment.assignment_items_inspection_status_id}',
                                    '{aeqsAssignment.assignment_items_qty_inspected}',
                                    to_date('{aeqsAssignment.assignment_items_inspection_completed_date}','yyyy-mm-dd hh24:mi:ss'),
                                    '{aeqsAssignment.assignment_items_total_inspection_minutes}',
                                    '{aeqsAssignment.assignment_items_sampling_size}',
                                    '{aeqsAssignment.assignment_items_qty_to_inspect}',
                                    '{aeqsAssignment.assignment_items_aql_minor}',
                                    '{aeqsAssignment.assignment_items_aql_major}',
                                    '{aeqsAssignment.assignment_items_aql_major_a}',
                                    '{aeqsAssignment.assignment_items_aql_major_b}',
                                    '{aeqsAssignment.assignment_items_aql_critical}',
                                    '{aeqsAssignment.assignment_items_supplier_booking_msg}',
                                    '{aeqsAssignment.assignment_items_conclusion_remarks}',
                                    '{aeqsAssignment.assignment_items_assignment_inspector_username}',
                                    to_date('{aeqsAssignment.assignment_items_assignment_date_inspection}','yyyy-mm-dd hh24:mi:ss'),
                                    '{aeqsAssignment.assignment_items_assignment_inspection_level}',
                                    '{aeqsAssignment.assignment_items_assignment_inspection_method}',
                                    '{aeqsAssignment.assignment_items_po_line_po_exporter_id}',
                                    '{aeqsAssignment.assignment_items_po_line_po_exporter_erp_business_id}',
                                    '{aeqsAssignment.assignment_items_po_line_po_number}',
                                    '{aeqsAssignment.assignment_items_po_line_importer_id}',
                                    '{aeqsAssignment.assignment_items_po_line_importer_erp_business_id}',
                                    '{aeqsAssignment.assignment_items_po_line_importer_project_id}',
                                    '{aeqsAssignment.assignment_items_po_line_sku_sku_number}',
                                    '{aeqsAssignment.assignment_items_po_line_sku_item_name}',
                                    '{aeqsAssignment.assignment_items_assignment_report_type_id}',
                                    '{aeqsAssignment.assignment_items_po_line_qty}',
                                    '{project_code}',
                                    '{report_type_name}'
                                    )";//Edit on 6/28(PO Change)
                                    DB.ExecuteNonQuery(sql);
                                    break;
                                }
                            }
                        }


                    }
                }



                #endregion

                #region PassFail
                sql = string.Empty;
                AEQS_TO_P88_PASSFAIL passfail = new AEQS_TO_P88_PASSFAIL();
                //mcs
                passfail.passfails_title = "mcs_confirmed_component_is_available_signature_compliant";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "1_general_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "quality_working_instruction_flow_chart";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "2_document_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "operation_all_sops_bpfc";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "2_document_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "broken_needle_procedure_control_record";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "3_metal_detection_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "metal_tools_detection_control_record";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "3_metal_detection_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "metal_detector_calibration_with_test_stick_approved_record";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "3_metal_detection_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "calibration_maintenance_records";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "4_machinery_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "daily_machine_setting_records";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "4_machinery_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "workplace_safety_standards_machinery_chemicals_ppe_etc";
                passfail.passfails_subsection = "validation";
                passfail.passfails_checklistsubsection = "5_occupational_health_safety_compliance";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "machine_condition";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "time";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "temperature";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "pressure";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "energy_level";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "speed";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "programming_automated_processing";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "1_machine_setting_according_to_standard";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";



                passfail.passfails_title = "mold";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "cutting_knife_laser_die_conveyor_board";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "needle";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "last";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "pressing_pad";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "gauges";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "2_tools";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";


                passfail.passfails_title = "chemical_management";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "3_process";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "process_compliance_to_sop";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "3_process";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "basic_6s";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "3_process";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                passfail.passfails_title = "final_evaluation";
                passfail.passfails_subsection = "checklist";
                passfail.passfails_checklistsubsection = "validation_checklist_final_evaluation";
                sql += $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{uniqueKey}', '{passfail.passfails_title}', 'Yes', 'check-list', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', 'pass');";

                DB.ExecuteNonQuery("begin " + sql + " end;");

                #endregion

                #region 图片
                //sql = string.Empty;
                //AEQS_TO_P88_SECTIONS_F picture2 = new AEQS_TO_P88_SECTIONS_F();
                //picture2.section_type = "pictures";
                //picture2.section_title = "photos";
                //sql = $@"insert into t_aeqs_to_p88_sections_f ( UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, SECTIONS_DEFECTS_PICTURES_FULL_FILENAME, SECTIONS_DEFECTS_PICTURES_NUMBER, SECTIONS_DEFECTS_PICTURES_COMMENT, SECTION_TYPE, SECTION_TITLE)
                //                            values('{uniqueKey}', '{picture2.sections_defects_pictures_title}', '{picture2.sections_defects_pictures_full_filename}', '{picture2.sections_defects_pictures_number}', '{picture2.sections_defects_pictures_comment}', '{picture2.section_type}','{picture2.section_title}')";
                //DB.ExecuteNonQuery(sql);
                #endregion

                // 同步关联
                sql = $@"insert into t_aql_cma_task_p88_relation(task_no,p88_unique_key,createtime,creator) values('{task_no}','{uniqueKey}','{completeDate + " " + completeTime}','{user}')";
                DB.ExecuteNonQuery(sql);

                DB.Commit();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查询AQL样本级别
        /// </summary>
        /// <param name="sampelNum"></param>
        /// <param name="AQLLevel"></param>
        public Dictionary<string, string> GetAQL(string sampelNum, string AQLLevel)
        {
            return null;
        }


        ///////////////-----------------------巡线-------------------//////////////////
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshop_SectIon(object OBJ)
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
                /* sql = $@"select
 DISTINCT
     a.TASK_NO,
 a.param_item_no,
 a.param_item_name,
 (case

         when num>0 then '已录入' 
         ELSE
       '未录入'
     end) as status,
  num,
 sop_standard,
 actual_standard
 from rqc_task_xj_item a LEFT JOIN(
 SELECT
     COUNT(id) as num,
     param_item_no,
     task_no,
 COUNT(sop_standard) as sop_standard, --sop标准
 COUNT(actual_standard) as actual_standard --实际数据
 FROM
     rqc_task_xj_item_c
 GROUP BY param_item_no,task_no) b on a.task_no=b.task_no and a.param_item_no=b.param_item_no WHERE a.task_no='{task_no}'";*/
                string sql = $@"select
DISTINCT
a.id, 
a.TASK_NO,
a.param_item_no,
a.param_item_name,
(case
		
		when (select count(id) from  rqc_task_xj_item_c where task_no=a.task_no and param_item_no=a.param_item_no)>0 then 'Already_Entered' 
		ELSE
      'Not_Entered'
	end) as status,
  '' as sop_standard,
(select actual_standard from (select actual_standard from  rqc_task_xj_item_c where task_no=a.task_no and param_item_no=a.param_item_no order by id desc) where ROWNUM<2) as actual_standard
from rqc_task_xj_item a  WHERE a.task_no='{task_no}'  order by a.param_item_no 
";
                DataTable dt = DB.GetDataTable(sql);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        string param_item_no = item["param_item_no"].ToString();
                        var c_dt = DB.GetDataTable($@"
SELECT
	SOP_STANDARD_START,
	SOP_STANDARD_END,
	SOP_STANDARD_TYPE
FROM
	(
		SELECT
			SOP_STANDARD_START,
			SOP_STANDARD_END,
			SOP_STANDARD_TYPE
		FROM
			rqc_task_xj_item_c
		WHERE
			task_no = '{task_no}'
		AND param_item_no = '{param_item_no}'
		ORDER BY
			ID DESC
	)
WHERE
	ROWNUM < 2
");
                        if (c_dt != null && c_dt.Rows.Count > 0)
                        {
                            string SOP_STANDARD_TYPE = c_dt.Rows[0]["SOP_STANDARD_TYPE"].ToString();
                            string SOP_STANDARD_START = c_dt.Rows[0]["SOP_STANDARD_START"].ToString();
                            string SOP_STANDARD_END = c_dt.Rows[0]["SOP_STANDARD_END"].ToString();

                            if (SOP_STANDARD_TYPE == "0")
                            {
                                SOP_STANDARD_START = SOP_STANDARD_TODATE_Str(SOP_STANDARD_START);
                                SOP_STANDARD_END = SOP_STANDARD_TODATE_Str(SOP_STANDARD_END);
                            }

                            item["sop_standard"] = $@"{SOP_STANDARD_START} - {SOP_STANDARD_END}";
                        }
                    }
                }

                ret.RetData1 = dt;
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        public static string SOP_STANDARD_TODATE_Str(string second_str)
        {
            try
            {
                int second = Convert.ToInt32(second_str);
                int h = second / (24 * 60 * 60);
                int min = (second - (h * (24 * 60 * 60))) / 60;
                int ss = second - h * (24 * 60 * 60) - min * 60;
                string date_str = $@"{h:00}:{min:00}:{ss:00}";
                return date_str;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string SOP_STANDARD_TOSECOND_Str(string date_str)
        {
            try
            {
                int second = 0;
                var date_str_arr = date_str.Split(':');
                if (date_str_arr.Length == 3)
                {
                    second = Convert.ToInt32(date_str_arr[0]) * 24 * 60 * 60 + Convert.ToInt32(date_str_arr[1]) * 60 + Convert.ToInt32(date_str_arr[2]);
                }

                return second.ToString();
            }
            catch (Exception)
            {
                return "0";
            }
        }

        /// <summary>
        /// 参数项目数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshop_Sect(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string eq_no = jarr.ContainsKey("eq_no") ? jarr["eq_no"].ToString() : "";
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";

                string sql = $@"SELECT
                          eqinfo.EQ_INFO_NO,--设备编号
                          eqtype_m.EQ_NO,--设备类型编号
                          eqtype_d.item_code as param_item_no,--参数项目编号
                          eqtype_d.item_name as param_item_name --参数项目名称
                        FROM
                          bdm_eq_info_m eqinfo--设备表
                        INNER JOIN BDM_EQ_TYPE_M eqtype_m ON eqtype_m.EQ_NO=eqinfo.EQ_NO -- 内关联设备类型
                        INNER JOIN BDM_EQ_TYPE_D eqtype_d ON (eqtype_d.m_id=eqtype_m.id AND eqtype_d.eq_type='1')-- 内关联设备类型参数明细
                        WHERE 
                        eqinfo.EQ_INFO_NO='{eq_no}' -- 扫描的设备编号
                        AND eqtype_d.item_code IN (
                          SELECT
                            param_item_no
                          FROM
                            rqc_task_xj_item
                          WHERE
                            task_no = '{task_no}'
                        )";
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData1 = dt;
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
        ///RQC任务——巡线——机台扫描检测项目录入——提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_xxadd(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                List<rqc_task_xj_item_c> list = jarr.ContainsKey("list") ? JsonConvert.DeserializeObject<List<rqc_task_xj_item_c>>(jarr["list"].ToString()) : null;

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                string sql = string.Empty;

                sql = $@"UPDATE RQC_TASK_M SET MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE TASK_NO='{task_no}'";
                DB.ExecuteNonQuery(sql);

                if (list.Count > 0)
                {
                    Dictionary<string, object> paramDic = new Dictionary<string, object>();
                    foreach (rqc_task_xj_item_c item in list)
                    {
                        paramDic = new Dictionary<string, object>();
                        sql = $@"insert into rqc_task_xj_item_c(task_no,eq_info_no,param_item_no,param_item_name,sop_standard,actual_standard,createby,createdate,createtime,sop_standard_type,sop_standard_start,sop_standard_end) values(@task_no1,@eq_info_no1,@param_item_no1,@param_item_name1,@sop_standard1,@actual_standard1,@user1,@date1,@time1,@sop_standard_type1,@sop_standard_start1,@sop_standard_end1)";
                        paramDic.Add($@"task_no1", task_no);
                        paramDic.Add($@"eq_info_no1", eq_info_no);
                        paramDic.Add($@"param_item_no1", item.param_item_no);
                        paramDic.Add($@"param_item_name1", item.param_item_name);
                        paramDic.Add($@"sop_standard1", item.sop_standard);
                        paramDic.Add($@"actual_standard1", item.actual_standard);
                        paramDic.Add($@"user1", user);
                        paramDic.Add($@"date1", date);
                        paramDic.Add($@"time1", time);

                        var sop_standard_arr = item.sop_standard.Split(" - ");
                        if (item.isTime.ToLower().Trim() == "true")
                        {
                            paramDic.Add($@"sop_standard_type1", "0");
                            paramDic.Add($@"sop_standard_start1", SOP_STANDARD_TOSECOND_Str(sop_standard_arr[0]));
                            paramDic.Add($@"sop_standard_end1", SOP_STANDARD_TOSECOND_Str(sop_standard_arr[1]));
                        }
                        else
                        {
                            paramDic.Add($@"sop_standard_type1", "1");
                            paramDic.Add($@"sop_standard_start1", sop_standard_arr[0]);
                            paramDic.Add($@"sop_standard_end1", sop_standard_arr[1]);
                        }

                        DB.ExecuteNonQuery(sql, paramDic);
                        item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "rqc_task_xj_item_c");

                        if (item.imginfo_list.Count > 0)
                        {
                            foreach (imginfo item2 in item.imginfo_list)
                            {
                                paramDic = new Dictionary<string, object>();
                                sql = $"insert into rqc_task_xj_item_c_f(union_id,file_guid,createby,createdate,createtime) values(@union_id1,@file_guid1,@createby1,@createdate1,@createtime1)";//添加对应的guid关联图片
                                paramDic.Add($@"union_id1", item_id);
                                paramDic.Add($@"file_guid1", item2.guid);
                                paramDic.Add($@"createby1", user);
                                paramDic.Add($@"createdate1", date);
                                paramDic.Add($@"createtime1", time);
                                DB.ExecuteNonQuery(sql, paramDic);
                            }
                        }
                    }
                }
               // ret.ErrMsg = "提交成功";
                ret.ErrMsg = "Submitted successfully";
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                //ret.ErrMsg = "提交失败，原因是:" + ex.Message;
                ret.ErrMsg = "Submission failed because:" + ex.Message;
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_deleteguid(object OBJ)
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
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";//条件 任务编号
                string sql = $@"delete from rqc_task_detail_t_f where file_guid='{guid}'";
                DB.ExecuteNonQuery(sql);

                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted"; 
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                //ret.ErrMsg = "删除失败，原因是:" + ex.Message;
                ret.ErrMsg = "Delete failed because:" + ex.Message;
                ret.IsSuccess = false;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 巡线历史记录查看
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Rqc_taskBase_xxview(object OBJ)
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
                string param_item_no = jarr.ContainsKey("param_item_no") ? jarr["param_item_no"].ToString() : "";
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(param_item_no))
                {
                    where += $@"and a.param_item_no like '%{param_item_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(eq_info_no))
                {
                    where += $@"and b.eq_info_no like '%{eq_info_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@" and b.CREATEDATE like '%{putin_date}%'";
                }
                if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    where += $@" and b.CREATEDATE like '%{end_date}%' ";
                }

                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@" and b.CREATEDATE BETWEEN '{putin_date}' and '{end_date}'";
                }
                string sql = string.Empty;
                sql = $@"SELECT
    b.id,
	a.TASK_NO,
	a.param_item_no,--参数项目编号
	a.param_item_name,--参数项目名称
	(b.CREATEDATE ||'  ' || b.CREATETIME) as CREATEDATE,--采集时间
	'' as sop_standard,--SOP数据
	SOP_STANDARD_START,
	SOP_STANDARD_END,
	SOP_STANDARD_TYPE,
	b.actual_standard, --参数实际数据
    b.eq_info_no --机台序列号
FROM
	rqc_task_xj_item A
LEFT JOIN rqc_task_xj_item_c b on a.TASK_NO=b.TASK_NO and a.param_item_no=b.param_item_no where a.task_no='{task_no}' {where} order by a.id desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));

                dt.Columns.Add("imginfo_list", Type.GetType("System.Object"));
                DataTable dt_rowurl = new DataTable();
                foreach (DataRow item in dt.Rows)
                {
                    string SOP_STANDARD_TYPE = item["SOP_STANDARD_TYPE"].ToString();
                    string SOP_STANDARD_START = item["SOP_STANDARD_START"].ToString();
                    string SOP_STANDARD_END = item["SOP_STANDARD_END"].ToString();

                    if (SOP_STANDARD_TYPE == "0")
                    {
                        SOP_STANDARD_START = SOP_STANDARD_TODATE_Str(SOP_STANDARD_START);
                        SOP_STANDARD_END = SOP_STANDARD_TODATE_Str(SOP_STANDARD_END);
                    }

                    item["sop_standard"] = $@"{SOP_STANDARD_START} - {SOP_STANDARD_END}";

                    //多明细==>又有属于自己的图片
                    sql = $@"SELECT
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	rqc_task_xj_item_c_f A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID
WHERE
	A.union_id = '{item["id"]}'";
                    dt_rowurl = DB.GetDataTable(sql);
                    List<imginfo> images = new List<imginfo>();

                    if (dt_rowurl.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt_rowurl.Rows.Count; i++)
                        {
                            images.Add(new imginfo
                            {
                                guid = dt_rowurl.Rows[i]["GUID"].ToString(),
                                image_url = dt_rowurl.Rows[i]["FILE_URL"].ToString()
                            }); ;
                        }
                    }
                    item["imginfo_list"] = images;
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
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
        /// 查看当前订单是否为PO变更后的新格式订单
        /// </summary>
        /// <param name="mer_po"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool IsOrderNewFormat(string mer_po, DataBase db)
        {
            bool flag = false;
            string sql = $@"select count(*) from bdm_se_order_master where mer_po = '{mer_po}' and  po_aggregator is not null and customer_po is not null";
            int n = db.GetInt32(sql);
            if (n > 0)
            {
                flag = true;
            }
            return flag;
        }

    }
    public class rqc_task_detail_t
    {
        public int? checkeds { get; set; }
        public string id { get; set; }
        public string inspection_code { get; set; }
        public string inspection_name { get; set; }
        public string qc_type { get; set; }
        public string judgment_criteria { get; set; }
        public string standard_value { get; set; }
        public string num { get; set; }
        public List<imginfo> imginfo_list { get; set; }
    }
    /// <summary>
    /// RQC任务——巡检——参数项目——提交明细--参数
    /// </summary>
    public class rqc_task_xj_item_c
    {
        /// <summary>
        /// 参数项目编号
        /// </summary>
        public string param_item_no { get; set; }
        /// <summary>
        /// 参数项目名称
        /// </summary>
        public string param_item_name { get; set; }
        /// <summary>
        /// sop标准
        /// </summary>
        public string sop_standard { get; set; }
        /// <summary>
        /// 参数实际数据
        /// </summary>
        public string actual_standard { get; set; }
        /// <summary>
        /// 是否时间控件
        /// </summary>
        public string isTime { get; set; }
        public List<imginfo> imginfo_list { get; set; }
    }
    public class rqc_task_check_t_f
    {
        /// <summary>
        /// id
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 任务编号
        /// </summary>
        public string task_no { get; set; }
        /// <summary>
        /// 关联id
        /// </summary>
        public string union_id { get; set; }
        /// <summary>
        /// 来源类型
        /// </summary>
        public string source_type { get; set; }
        /// <summary>
        /// 检验总数
        /// </summary>
        public string qty { get; set; }
        /// <summary>
        /// 合格数量
        /// </summary>
        public string q_qty { get; set; }
        /// <summary>
        /// 不良问题点描述
        /// </summary>
        public string bad_desc { get; set; }
        /// <summary>
        /// 检验结果
        /// </summary>
        public string check_res { get; set; }
        public List<imginfo> imginfo_list { get; set; }
    }
    public class imginfo
    {
        public string image_name { get; set; }
        public string guid { get; set; }
        public string image_url { get; set; }
    }
    public class imginfos
    {
        public string file_name { get; set; }
        public string guid { get; set; }
        public string file_url { get; set; }
    }
    //Added for pivot88
    public class ShoeSize
    {
        public int se_qty;
        public int assign_qty;
        public string size;
    }
}
