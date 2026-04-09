using System;
using System.Collections.Generic;
using System.Data;
using SJ_QCMAPI.Common;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace SJ_QAAPI
{
   
    

    public class Firstarticleassurance
    {
        private class file_list
        {
            public DateTime createdate { get; set; }
            public string guid { get; set; }
            public string file_name { get; set; }
            public string file_url { get; set; }

        }
        private partial class qcm_f_art_cfm_cc_d
        {
            public string id { get; set; }
            public string checkeds { get; set; }

            public string inspection_type { get; set; }
            public string inspection_code { get; set; }
            public string inspection_name { get; set; }
            public string remark { get; set; }
            public string commit_type { get; set; }
            public string commit_inspection { get; set; }
            public List<file_list> file_list { get; set; }
        }
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
        /// <summary>
        /// 添加用的
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected static string getcreate(string token)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
            string time = DateTime.Now.ToString("HH:mm:ss");//时间
            return $@"'{token}','{date}','{time}'";
        }
        /// <summary>
        /// 修改用的
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        protected static string getupdate(string token)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
            string time = DateTime.Now.ToString("HH:mm:ss");//时间
            return $"'{token}','{date}','{time}'";
        }
        /// <summary>
        /// 主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject OutMaingetlist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
             
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";//开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束时间
                string f_type = jarr.ContainsKey("f_type") ? jarr["f_type"].ToString() : "";//首件类型
                string keyword= jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//全部条件
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and (a.prod_no like '%{keyword}%' or a.eq_info_no like '%{keyword}%' or a.production_line_code like '%{keyword}%' or d.workmanship_name like '%{keyword}%' or c.name_t like '%{keyword}%' or a.ex_task_no like '%{keyword}%' or a.physical_name like '%{keyword}%') ";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.createdate  like '%{putin_date}%'";
                }
                if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    strwhere += $@" and a.createdate like '%{end_date}%' ";
                }
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.createdate BETWEEN '{putin_date}' and '{end_date}'";
                }
                if (!string.IsNullOrWhiteSpace(f_type))
                {
                    strwhere += $@" and a.f_type like '%{f_type}%'";

                }

                #region 旧的
                /* var sql = $@"
 select
     a.f_type,--首件类型
     a.f_type_name,--首件名称
     a.task_no,--任务编号
     a.EQ_INFO_NO,--设备编号
     a.eq_info_name,--设备名称
     a.production_line_code,--产线编号
     a.production_line_name,--产线名称
     a.workshop_section_name,--工段
     a.prod_no,--ART
     a.prod_name,--ART名称
     a.workmanship_name,--工艺材料种类
     a.physical_name,--实物名称
     a.datas,--日期
      b.sign_res,--结果代号
     case 
         when b.sign_res='0' then '通过'
         when b.sign_res='1' then '未通过'
     end as sign_res_name,--结果

      b.sign_idea,--签核意见
     case 
         when b.sign_idea is null then '未签核'
         when b.sign_idea='0' then '未签核'
         when b.sign_idea='1' then '同意'
         when b.sign_idea='2' then '不同意'
     end as sign_idea_name --签核意见


 from(
 SELECT
 a.id,
 a.task_no,--任务编号
 a.mer_po,--po号
 a.prod_no,--art
 r.name_t as prod_name,--art名称
 a.shoe_no,--鞋型编号
 c.name_t,--鞋型名称
 a.mold_no,--模号
 (a.createdate ||' '|| a.createtime) as datas,--日期
 a.createdate,--日期
 a.workshop_section_no,--工段编号
 g.workshop_section_name,--工段名称
 d.workmanship_code,--工艺编号
 d.workmanship_name,--工艺名称
 a.config_no,--配置编号
 a.f_type,--首件类型
 CASE
     when a.f_type='0' then '常规'
     when a.f_type='1' then '新上线'
     when a.f_type='2' then '变更'
 end as f_type_name,--首件名称
 a.EQ_INFO_NO,--设备编号
 m.eq_info_name,--设备名称
 a.production_line_code,--产线编号
 k.production_line_name,--产线名称
 a.ex_task_no,--实验室编号
 a.physical_name--实物名称
 FROM
     qcm_f_art_cfm_m a
 LEFT JOIN BDM_PARAM_ITEM_D d on a.workshop_section_no=d.WORKSHOP_SECTION_NO and a.config_no=d.config_no
 LEFT JOIN BDM_RD_STYLE c on a.SHOE_NO=c.SHOE_NO
 LEFT JOIN BDM_EQ_INFO_M m on a.eq_info_no=m.eq_info_no
 LEFT JOIN BDM_PRODUCTION_LINE_M k ON 	a.production_line_code=k.production_line_code
 LEFT JOIN bdm_rd_prod r on a.prod_no=r.prod_no
 LEFT JOIN (SELECT
       DISTINCT
       b.workshop_section_no,
       b.workshop_section_name
   FROM
       qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no) g on a.workshop_section_no=g.workshop_section_no where 1=1  {strwhere}) a left join (select max(task_no) as task_no,max(sign_res) as sign_res,max(sign_idea) as sign_idea from qcm_f_art_cfm_s GROUP BY task_no) b on a.task_no=b.task_no
 order by a.id desc
 "; */
                #endregion
                var sql = $@"
select * from (
SELECT
a.id,
a.task_no,--任务编号
a.mer_po,--po号
a.prod_no,--art
r.name_t as prod_name,--art名称
a.shoe_no,--鞋型编号
c.name_t,--鞋型名称
a.mold_no,--模号
(a.createdate ||' '|| a.createtime) as datas,--日期
a.createdate,--日期
a.workshop_section_no,--工段编号
g.workshop_section_name,--工段名称
d.workmanship_code,--工艺编号
d.workmanship_name,--工艺名称
a.config_no,--配置编号
a.f_type,--首件类型
k.sign_res,--结果代号
 case 
     when k.sign_res='0' then 'Pass'
     when k.sign_res='1' then 'Not_Pass'
     end as sign_res_name,--结果
p.sign_res as sign_idea,-- 签核意见
 case 
         when p.sign_res is null then 'Not_Signed_Off'
         when p.sign_res='0' then 'Agree'
         when p.sign_res='1' then 'Disagree'
     end as sign_idea_name, --签核意见
CASE
	when a.f_type='0' then 'Conventional'
	when a.f_type='1' then 'New_Online'
	when a.f_type='2' then 'Change'
end as f_type_name,--首件名称
a.EQ_INFO_NO,--设备编号
m.eq_info_name,--设备名称
a.production_line_code,--产线编号
k.production_line_name,--产线名称
a.ex_task_no,--实验室编号
a.physical_name,--实物名称
(CASE  
  when A .MODIFYDATE is NOT NULL then REPLACE (A .MODIFYDATE, '-', '') || REPLACE (A .MODIFYTIME, ':', '')
  else REPLACE (A .CREATEDATE, '-', '') || REPLACE (A .CREATETIME, ':', '')
  END ) as order_time
FROM
	qcm_f_art_cfm_m a
LEFT JOIN BDM_PARAM_ITEM_D d on a.workshop_section_no=d.WORKSHOP_SECTION_NO and a.config_no=d.config_no
LEFT JOIN BDM_RD_STYLE c on a.SHOE_NO=c.SHOE_NO
LEFT JOIN BDM_EQ_INFO_M m on a.eq_info_no=m.eq_info_no
LEFT JOIN BDM_PRODUCTION_LINE_M k ON 	a.production_line_code=k.production_line_code
LEFT JOIN bdm_rd_prod r on a.prod_no=r.prod_no
LEFT JOIN (SELECT
      DISTINCT
      b.workshop_section_no,
      b.workshop_section_name
  FROM
      qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no) g 
on a.workshop_section_no=g.workshop_section_no 
LEFT JOIN (select task_no,sign_res from qcm_f_art_cfm_s where id in (select max(id)as id from 	qcm_f_art_cfm_s GROUP BY task_no)) k on a.task_no=k.task_no
LEFT JOIN (select task_no,sign_res from qcm_f_art_cfm_s_c where id in (select max(id)as id from 	qcm_f_art_cfm_s_c GROUP BY task_no)) p on a.task_no=p.task_no
where 1=1 {strwhere}
) order by order_time desc
";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int total = CommonBASE.GetPageDataTableCount(DB, sql);

                foreach (DataRow item in dt.Rows)
                {
                    string production_line_name = item["production_line_name"].ToString();
                    if (string.IsNullOrWhiteSpace(production_line_name))
                    {
                        item["production_line_name"] = DB.GetString($@"SELECT DEPARTMENT_NAME FROM BASE005M WHERE DEPARTMENT_CODE='{item["production_line_code"]}'");
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("total", total);
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
        /// po带出数据数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Outmer_po_getlist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string so_num = jarr.ContainsKey("so_num") ? jarr["so_num"].ToString() : "";
                string strwhere = string.Empty;
                var sql = string.Empty;
                sql = $@"SELECT m.se_id as so_num,
	m.mer_po,--po号
	r.PROD_NO,--art
    r.name_t as PROD_NAME,--ART名称
	r.MOLD_NO,--模号
    r.SHOE_NO,--鞋型编号
	l.name_t,--鞋型
  f.file_url--鞋图
FROM
  BDM_SE_ORDER_MASTER m
  LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
  LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
  LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
  LEFT JOIN QCM_SHOES_QA_RECORD_M d ON r.shoe_no = d.shoes_code
  LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON d.IMAGE_GUID = f.guid
WHERE  m.se_id = '{so_num}'
order by f.guid asc";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                   
                    if (string.IsNullOrWhiteSpace(dt.Rows[0]["SHOE_NO"].ToString()))
                    {
                        ret.ErrMsg = "This PO number does not contain any shoe type. Please re-enter the PO number to continue this operation.";
                        ret.IsSuccess = false;
                        return ret;
                    }
                }
                else
                {
                    ret.ErrMsg = $@"The basic information of this PO [{so_num}] is not maintained, please check";
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
        /// 工段工艺下拉框内容（根据鞋型寻找）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getworkshop_list(object OBJ)
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
                    item["bottom"] = new string[] { };
                    sql = $@"select
                CONFIG_NO,
				WORKMANSHIP_NAME,
                WORKMANSHIP_CODE
				from
				BDM_PARAM_ITEM_D
                where  WORKSHOP_SECTION_NO='{item["workshop_section_no"]}'";
                    dtrow = DB.GetDataTable(sql);
                    item["bottom"] = dtrow;
                }
                if (dt.Rows.Count < 1)
                {
                    ret.ErrMsg = "该鞋型没有维护工段工艺内容，请检查";
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
        /// 主页添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InputMain_add_task(object OBJ)
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
                NullKeyValue(jarr);//去掉请求时带null
                //转译
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//条件 工段编号
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";//配置编号
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//条件 art
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//条件 po
                string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//条件 模号
                string ex_task_no = jarr.ContainsKey("ex_task_no") ? jarr["ex_task_no"].ToString() : "";//实验室任务编号
                string physical_name = jarr.ContainsKey("physical_name") ? jarr["physical_name"].ToString() : "";//实物名称
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";//设备编号
                string f_type = jarr.ContainsKey("f_type") ? jarr["f_type"].ToString() : "";//类型
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//产线编号
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//备注
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string item_id = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //分类的
                string sql = string.Empty;
                task_no = DB.GetString($@"select task_no from qcm_f_art_cfm_m where task_no='{task_no}'");

               
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = "update qcm_f_art_cfm_m set remark=@remark,ex_task_no=@ex_task_no,physical_name=@physical_name,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where task_no=@task_no";
                    dic = new Dictionary<string, object>();
                    dic.Add("remark", remark);//任务状态
                    dic.Add("ex_task_no", ex_task_no);
                    dic.Add("physical_name", physical_name);
                    dic.Add("modifyby", user);
                    dic.Add("modifydate", date);
                    dic.Add("modifytime", time);
                    dic.Add("task_no", task_no);
                    DB.ExecuteNonQuery(sql, dic);
                }
                else
                {
                    task_no = "R" + date;
                    string maxtask_no = DB.GetString($@"select max(task_no) from qcm_f_art_cfm_m where task_no like '{task_no}%'");
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
                    //新增的时候判断
                    if (string.IsNullOrWhiteSpace(workshop_section_no) ||
                   string.IsNullOrWhiteSpace(config_no))
                    {
                        ret.ErrMsg = "Failed to add, reason: the shoe type does not have the technical content of the maintenance section, please check";
                        ret.IsSuccess = false;
                        return ret;
                    }

                    //表头的添加
                    sql = $@"insert into  qcm_f_art_cfm_m(
                        task_no,
                        mer_po,
                        prod_no,
                        shoe_no,
                        mold_no,
                        workshop_section_no,
                        config_no,
                        f_type,
                        eq_info_no,
                        production_line_code,
                        ex_task_no,
                        physical_name,
                        remark,
                        createby,
                        createdate,
                        createtime
                    )  values(@task_no,@mer_po,@prod_no,@shoe_no,@mold_no,@workshop_section_no,@config_no,@f_type,@eq_info_no,@production_line_code,@ex_task_no,@physical_name,@remark,@createby,@createdate,@createtime)";
                    dic = new Dictionary<string, object>();
                    dic.Add("task_no", task_no);
                    dic.Add("mer_po", mer_po);
                    dic.Add("prod_no", prod_no);
                    dic.Add("shoe_no", shoe_no);
                    dic.Add("mold_no", mold_no);
                    dic.Add("workshop_section_no", workshop_section_no);
                    dic.Add("config_no", config_no);
                    dic.Add("f_type", f_type);
                    dic.Add("eq_info_no", eq_info_no);
                    dic.Add("production_line_code", production_line_code);
                    dic.Add("ex_task_no", ex_task_no);
                    dic.Add("physical_name", physical_name);
                    dic.Add("remark", remark);//任务状态
                    dic.Add("createby", user);
                    dic.Add("createdate", date);
                    dic.Add("createtime", time);
                    DB.ExecuteNonQuery(sql, dic);
                    //添加配置信息
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
                            WHERE 1=1 and QC_TYPE='4' 
                           -- and ROWNUM<21
                            ORDER BY id asc";
                        DataTable inspectiondt = DB.GetDataTable(sql);
                        sql = string.Empty;
                        dic = new Dictionary<string, object>();
                        int index = 0;
                        foreach (DataRow item in inspectiondt.Rows)
                        {
                            index++;
                            dic.Add($"task_no{index}", task_no);
                            dic.Add($"inspection_type{index}", inspection_type);
                            dic.Add($"inspection_code{index}", item["inspection_code"]);
                            dic.Add($"inspection_name{index}", item["inspection_name"]);
                            dic.Add($"createby{index}", user);
                            dic.Add($"createdate{index}", date);
                            dic.Add($"createtime{index}", time);
                            //sql += $@"insert into qcm_f_art_cfm_cc (task_no,inspection_type,inspection_code,inspection_name,createby,createdate,createtime) 
                            //    values(@task_no{index},@inspection_type{index},@inspection_code{index},@inspection_name{index},@createby{index},@createdate{index},@createtime{index});";  This is OLD ONE
                            sql += $@"insert into qcm_f_art_cfm_cc (task_no,inspection_type,inspection_code,inspection_name,createby,createdate,createtime,COMMIT_INSPECTION) 
                                values(@task_no{index},@inspection_type{index},@inspection_code{index},@inspection_name{index},@createby{index},@createdate{index},@createtime{index},'1');"; // This is edited by Ashok to show delete option for all RQC check items in First Article Confirmation.

                        }
                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            DB.ExecuteNonQuery($"begin {sql} end;",dic);
                        }
                       
                    }
                }
                ret.ErrMsg = "Submitted successfully";
                ret.IsSuccess = true;
                ret.RetData1 = task_no;
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "提交失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 任务编号所有删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_task_no_delete(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string sql = $@"select count(1) from qcm_f_art_cfm_s_c where task_no='{task_no}'";
                if (DB.GetInt32(sql) > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "已签核任务不能做删除操作";
                    return ret;
                }
                sql = $@"delete from qcm_f_art_cfm_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);//删除任务表头

                sql = $@"delete from qcm_f_art_cfm_s where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);//删除提交表头

                sql = $@"select id from qcm_f_art_cfm_c where task_no='{task_no}'";
                string id = DB.GetString(sql);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $@"delete from qcm_f_art_cfm_c where id='{id}'";
                    DB.ExecuteNonQuery(sql);//删除提交表头-提交明细
                    sql = $@"delete from qcm_f_art_cfm_cf where union_id='{id}'";
                    DB.ExecuteNonQuery(sql);//删除提交表头-提交明细-文件
                }
              

                sql = $@"delete from qcm_f_art_cfm_cc where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);//删除项目配置

                sql = $@"select id from qcm_f_art_cfm_cc_d where task_no='{task_no}'";
                id = DB.GetString(sql);
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $@"delete from qcm_f_art_cfm_cc_d where id='{id}'";
                    DB.ExecuteNonQuery(sql);//删除项目配置-提交记录
                    sql = $@"delete from qcm_f_art_cfm_cc_d_f where union_id='{id}'";
                    DB.ExecuteNonQuery(sql);//删除项目配置-提交记录-文件
                }
              
                sql = $@"delete from qcm_f_art_cfm_pg where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);//删除拍照

                sql = $@"delete from qcm_f_art_cfm_s_c where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);//删除签名确认

                ret.ErrMsg = $@"Operation delete {task_no} successful";
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "The delete operation failed, reason:" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 继续录入带出信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject OutMain_tack_no_list(object OBJ)
        
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
                string strwhere = string.Empty;
                var sql = string.Empty;
                sql = $@"SELECT
a.task_no,--任务编号
a.mer_po,--po号
x.se_id as so_num,
a.prod_no,--art
r.name_t as prod_name,--art名称
a.shoe_no,--鞋型编号
c.name_t,--鞋型名称
a.mold_no,--模号
a.workshop_section_no,--工段编号
g.workshop_section_name,--工段名称
d.workmanship_code,--工艺编号
d.workmanship_name,--工艺名称
a.config_no,--配置编号
a.f_type,--首件类型
a.EQ_INFO_NO,--设备编号
m.eq_info_name,--设备名称
a.production_line_code,--产线编号
k.production_line_name,--产线名称
a.ex_task_no,--实验室编号
a.physical_name,--实物名称
f.file_url,--鞋图
a.remark --备注
FROM
	qcm_f_art_cfm_m a
LEFT JOIN BDM_PARAM_ITEM_D d on a.workshop_section_no=d.WORKSHOP_SECTION_NO and a.config_no=d.config_no
LEFT JOIN BDM_EQ_INFO_M m on a.eq_info_no=m.eq_info_no
LEFT JOIN BDM_PRODUCTION_LINE_M k ON 	a.production_line_code=k.production_line_code
LEFT JOIN bdm_rd_prod r on a.prod_no=r.prod_no
LEFT JOIN BDM_RD_STYLE c on a.SHOE_NO=c.SHOE_NO
 LEFT JOIN QCM_SHOES_QA_RECORD_M d ON a.shoe_no = d.shoes_code
  LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON d.IMAGE_GUID = f.guid
left join BDM_SE_ORDER_MASTER x on x.mer_po=a.mer_po
LEFT JOIN (SELECT
      DISTINCT
      b.workshop_section_no,
      b.workshop_section_name
  FROM
      qcm_dqa_mag_m a LEFT JOIN BDM_PARAM_ITEM_D b  on a.workshop_section_no=b.workshop_section_no) g on a.workshop_section_no=g.workshop_section_no  where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("ISSUC", Type.GetType("System.Object"));
                //check_res(0pass，1fail),sign_res(0 同意，1 不同意),commit_type(0 pass，1 fail)
                bool button = true;
                sql = $@"select count(*) from qcm_f_art_cfm_c where task_no='{task_no}'and check_res='1'";
                int failsum1 = DB.GetInt32(sql);//检验项目提交明细

                sql = $@"select count(*) from  qcm_f_art_cfm_s_c where task_no='{task_no}' and sign_res='1'";
                int failsum2 = DB.GetInt32(sql);//DQA/MQA提交明细

                sql = $@"select count(*) from qcm_f_art_cfm_cc_d where task_no='{task_no}' and commit_type='1'";
                int failsum3 = DB.GetInt32(sql);//项目配置——提交记录

                if (failsum1 > 0 || failsum2 > 0 || failsum3 > 0)
                {
                    button = false;
                }
                foreach (DataRow item in dt.Rows)
                {
                    item["ISSUC"] = button;
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
        /// 卡片盒子数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Outhistory_getList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString().Trim() : "";
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString().Trim() : "";
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString().Trim() : "";
                NullKeyValue(jarr);
                var sql = $@"select*from (SELECT
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
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE 1=1 and d.shoes_code like '%{shoe_no}%' and w.workshop_section_no='{workshop_section_no}' and d.ISDELETE='0'
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
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE 1=1 and d.shoes_code like '%{shoe_no}%' and d.workshop_section_no='{workshop_section_no}'  and d.ISDELETE='0') order by id desc";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("history", Type.GetType("System.Object"));
                dt.Columns.Add("top_file_list", Type.GetType("System.Object"));

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

                sql = $@"
SELECT
    ID,
    TASK_NO,--任务编号
	UNION_ID,--关联id
    sorting,--提交序号
	SOURCE_TYPE,--类型
	QTY,--检验总数
	Q_QTY,--合格数量
	BAD_DESC,--不良问题描述
	CHECK_RES --检验结果
FROM
	qcm_f_art_cfm_c  where  task_no='{task_no}'   ORDER BY id desc";

                DataTable qcm_f_art_cfm_c = DB.GetDataTable(sql);
                qcm_f_art_cfm_c.Columns.Add("file_list", Type.GetType("System.Object"));//检验任务图片

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	qcm_f_art_cfm_cf A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";//首件任务——核对——记录
                DataTable qcm_f_art_cfm_cf = DB.GetDataTable(sql);
                DataTable dtTest = new DataTable();
                string source_type = string.Empty;
                DataTable drrow = new DataTable();
                List<file_list> images = new List<file_list>();
                DataRow[] dr = null;
                DataRow[] dr2 = null;
                
                foreach (DataRow item in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["enum_value"].ToString()))
                    {
                        item["inspection_name"] = DB.GetString($@"select inspection_name from {item["enum_value2"]} where inspection_code='{item["inspection_code"]}'");
                    }
                    images = new List<file_list>();
                    if (item["source"].ToString() == "DQA")
                    {
                        source_type = "0";
                        //DQA附件
                        dr = dt_dqaimg.Select($"D_ID='{item["ID"]}'");
                        foreach (DataRow dr_dqaimg in dr)
                        {
                            images.Add(new file_list
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
                            images.Add(new file_list
                            {
                                guid = dr_mqaimg["GUID"].ToString(),
                                file_url = dr_mqaimg["FILE_URL"].ToString(),
                                file_name = dr_mqaimg["FILE_NAME"].ToString()
                            });
                        }

                    }
                    item["top_file_list"] = images;//把文件装到top_imginfo_list
                    dr = qcm_f_art_cfm_c.Select($"UNION_ID='{item["ID"]}' and SOURCE_TYPE='{source_type}'");

                    dtTest = qcm_f_art_cfm_c.Clone();
                    item["history"] = dtTest;
                    foreach (DataRow dr_qcm_f_art_cfm_c in dr)
                    {
                        dr2 = qcm_f_art_cfm_cf.Select($"UNION_ID='{dr_qcm_f_art_cfm_c["ID"]}'");
                        images = new List<file_list>();
                        foreach (DataRow dr_qcm_f_art_cfm_cf in dr2)
                        {
                            images.Add(new file_list
                            {
                                guid = dr_qcm_f_art_cfm_cf["GUID"].ToString(),
                                file_url = dr_qcm_f_art_cfm_cf["FILE_URL"].ToString(),
                                file_name = dr_qcm_f_art_cfm_cf["FILE_NAME"].ToString()
                            });
                        }
                        dr_qcm_f_art_cfm_c["file_list"] = images;
                        dtTest.ImportRow(dr_qcm_f_art_cfm_c);
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
        /// DQA/MQA录入（卡片盒子的录入）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Div_input(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string union_id = jarr.ContainsKey("union_id") ? jarr["union_id"].ToString() : "";
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";
                string q_qty = jarr.ContainsKey("q_qty") ? jarr["q_qty"].ToString() : "";
                string bad_desc = jarr.ContainsKey("bad_desc") ? jarr["bad_desc"].ToString() : "";
                string check_res = jarr.ContainsKey("check_res") ? jarr["check_res"].ToString() : "";
                List<file_list> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["file_list"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string input = getcreate(user);
                DateTime currDate = DateTime.Now;
                string sql = string.Empty;

                sql = $@"UPDATE QCM_F_ART_CFM_M SET MODIFYBY='{user}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE TASK_NO='{task_no}'";
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
                int maxsorting = DB.GetInt32($"select nvl(max(SORTING),0) from qcm_f_art_cfm_c where task_no='{task_no}'");
                sql = $@"insert into qcm_f_art_cfm_c(task_no,union_id,sorting,source_type,qty,q_qty,bad_desc,check_res,createby,createdate,createtime) values('{task_no}','{union_id}','{maxsorting+1}','{source_type}','{qty}','{q_qty}','{bad_desc}','{check_res}',{input})";
                DB.ExecuteNonQuery(sql);
                string item_id  = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_f_art_cfm_c");

                if (list.Count > 0)
                {
                    sql = string.Empty;
                    foreach (file_list item in list)
                    {
                        sql += $"insert into qcm_f_art_cfm_cf(union_id,file_guid,createby,createdate,createtime) values('{item_id}','{item.guid}',{input});";//添加对应的guid关联图片
                       
                    }
                    DB.ExecuteNonQuery("begin "+sql+" end;");
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
                        sql = $@"UPDATE {updateTableName} SET F_INSP_DEP='{dp_name}',F_INSP_DATE='{currDate:yyyy-MM-dd}',F_INSP_RES='{check_res_res}' WHERE ID={union_id}";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                #endregion

                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 返回历史数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Outhistory_getList2(object OBJ)
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
	UNION_ID,--关联id
    sorting,--提交序号
	SOURCE_TYPE,--类型
	QTY,--检验总数
	Q_QTY,--合格数量
	BAD_DESC,--不良问题描述
	CHECK_RES --检验结果
FROM
	qcm_f_art_cfm_c  where   task_no='{task_no}' and union_id='{id}' and source_type='{source_type}'   ORDER BY CREATEDATE DESC";
                DataTable dt = DB.GetDataTable(sql);

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	qcm_f_art_cfm_cf A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";//RQC任务——核对——记录
                DataTable dt_rqcimg = DB.GetDataTable(sql);

                dt.Columns.Add("file_list", Type.GetType("System.Object"));
                List<file_list> images = new List<file_list>();
                DataRow[] dr = null;
                foreach (DataRow item in dt.Rows)
                {
                    images = new List<file_list>();
                    dr = dt_rqcimg.Select($"UNION_ID='{item["ID"]}'");
                    item["file_list"] = images;
                    foreach (DataRow dr_mqaimg in dr)
                    {
                        images.Add(new file_list
                        {
                            guid = dr_mqaimg["GUID"].ToString(),
                            file_url = dr_mqaimg["FILE_URL"].ToString()
                        });
                    }
                    item["file_list"] = images;
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
        /// 检验项目数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getinspectionitem_list(object OBJ)
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
                string sql = string.Empty;
                sql = $@"SELECT
    id,
     '' AS checkeds,
	task_no,
	inspection_type,
	inspection_code,
	inspection_name,
    commit_inspection
FROM
	qcm_f_art_cfm_cc where task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("commit_type", Type.GetType("System.Object"));
                foreach (DataRow item in dt.Rows)
                {
                    item["commit_type"] = true;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("qcm_f_art_cfm_cc_d", dt);
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
        /// 检验项目提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commitinspectionitem(object OBJ)
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
                List<qcm_f_art_cfm_cc_d> list = jarr.ContainsKey("qcm_f_art_cfm_cc_d") ? JsonConvert.DeserializeObject<List<qcm_f_art_cfm_cc_d>>(jarr["qcm_f_art_cfm_cc_d"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var currDate = DateTime.Now;
                string sql = string.Empty;
                int maxsorting = DB.GetInt32($"select nvl(max(SORTING),0) from qcm_f_art_cfm_cc_d where task_no='{task_no}'");
                string sign_res = jarr.ContainsKey("sign_res") ? jarr["sign_res"].ToString() : "";//结果
                string input = getcreate(user);
                sql = $@"select count(1) from qcm_f_art_cfm_m where task_no='{task_no}'";
                if (DB.GetInt32(sql) < 1)
                {
                    ret.ErrMsg = $@"Task number [{task_no}] does not exist, please check";
                    ret.IsSuccess = false;
                    return ret;
                }

                sql = $@"UPDATE QCM_F_ART_CFM_M SET MODIFYBY='{user}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"select nvl(max(sorting),0) from  qcm_f_art_cfm_s where task_no='{task_no}'";
                int maxsorting2 = DB.GetInt32(sql);
                sql = $@"insert into qcm_f_art_cfm_s(task_no,sorting,sign_res,createby,createdate,createtime) values('{task_no}','{maxsorting2 + 1}','{sign_res}',{input})";
                DB.ExecuteNonQuery(sql);
                string item_id = string.Empty;
                if (list.Count > 0)
                {
                    maxsorting += 1;
                    foreach (qcm_f_art_cfm_cc_d item in list)
                    {

                        if (item.commit_inspection == "1")
                        {
                            if (!string.IsNullOrWhiteSpace(item.id))
                            {
                                //1为修改的
                                sql = $@"update qcm_f_art_cfm_cc set  inspection_name='{item.inspection_name}' where commit_inspection='1' and id='{item.id}'";
                                DB.ExecuteNonQuery(sql);
                            }

                        }
                        sql = $@"insert into qcm_f_art_cfm_cc_d (task_no,sorting,inspection_type,inspection_code,inspection_name,remark,commit_type,createby,createdate,createtime) 
                                    values('{task_no}','{maxsorting}','{item.inspection_type}','{item.inspection_code}','{item.inspection_name}','{item.remark}','{item.commit_type}',{input})";
                        DB.ExecuteNonQuery(sql);
                        item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_f_art_cfm_cc_d");

                        sql = $@"delete from rqc_task_detail_t_f where task_no='{task_no}'";
                        DB.ExecuteNonQuery(sql);
                        sql = string.Empty;
                        if (item.file_list.Count > 0)
                        {
                            foreach (file_list item2 in item.file_list)
                            {
                                sql += $@"insert into qcm_f_art_cfm_cc_d_f(union_id,file_guid,createby,createdate,createtime) 
                                        values('{item_id}','{item2.guid}',{input});";

                            }
                            DB.ExecuteNonQuery("begin " + sql + " end;");
                        }
                        sql = string.Empty;
                        if (item.checkeds == "2")
                        {
                            //等于2的就是新增的
                            sql = $@"insert into qcm_f_art_cfm_cc(task_no,inspection_type,inspection_code,inspection_name,commit_inspection,createby,createdate,createtime) values('{task_no}','{item.inspection_type}','{item.inspection_code}','{item.inspection_name}','1',{input})";
                            DB.ExecuteNonQuery(sql);
                        }


                    }

                }
                ret.ErrMsg = "Saved successfully";
                ret.IsSuccess = true;
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Save failed due to：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 首检记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getinspectionitem_record(object OBJ)
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
	{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT id", "id")} as id,--id,用来拼接图片的
	('第'||sorting ||'次') as sorting,--首检次数
    sorting as sorting2,
	(createdate||' '|| createtime) as createdate, --日期
	-- listagg(DISTINCT inspection_name, ',') WITHIN GROUP (ORDER BY inspection_name) as inspection_name --不良项明细
	'' as inspection_name
FROM
	qcm_f_art_cfm_cc_d where task_no='{task_no}' and commit_type='1'
	GROUP BY task_no,sorting,(createdate||' '|| createtime) order by sorting asc";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("file_list", Type.GetType("System.Object"));

                sql = $@"SELECT INSPECTION_NAME,SORTING,COMMIT_TYPE,COMMIT_TYPE,INSPECTION_NAME FROM QCM_F_ART_CFM_CC_D WHERE TASK_NO='{task_no}' ";
                DataTable dt_2 = DB.GetDataTable(sql);//用于查之后序号对应的是否检验拼接

                sql = $@"select check_res from qcm_f_art_cfm_c where id in (select max(id) from qcm_f_art_cfm_c where task_no='{task_no}')";
                string check_res = DB.GetString(sql);//查看最新的检验结果是否pass,fail

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	  qcm_f_art_cfm_cc_d_f A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID";
                DataTable dt_img = DB.GetDataTable(sql);

                string[] arrid = null;
                DataRow[] dr = null;
                DataRow[] dr2 = null;
                List<file_list> images = new List<file_list>();
                foreach (DataRow item in dt.Rows)
                {
                    //0 pass,1 fail 
                    dr2 = dt_2.Select($@"SORTING='{item["SORTING2"]}'and COMMIT_TYPE='1'");//查询每次提交是否存在全部pass(存在全部pass显示首检验通过，存在没有通过的则拼接fial情况的不良数据)
                    if (dr2.Length > 0)//说明存在fail的情况，则取fail
                    {
                        foreach (DataRow d in dr2)
                        {
                            item["INSPECTION_NAME"] +=  d["INSPECTION_NAME"].ToString() + "、";//取fail的检验项名称
                        }
                       
                    }
                    else if(check_res=="1")//最新的盒子（DQA,MQA）提交存在fail
                    {
                        item["INSPECTION_NAME"] = "DQA/MQA项目不通过";
                    }
                    else
                    {
                        //在上面两个条件都不存在就显示
                        item["INSPECTION_NAME"] = "检验通过";
                    }
                    //处理首件未通过的数据拼接到dt.INSPECTION_NAME里面
                    if (item["id"].ToString().Contains(','))
                    {
                        arrid = item["id"].ToString().Split(',');
                        images = new List<file_list>();
                        for (int i = 0; i < arrid.Length; i++)
                        {
                            dr = dt_img.Select($"UNION_ID='{arrid[i]}'");
                            
                            foreach (DataRow dr_img in dr)//假如多个图片
                            {
                                images.Add(new file_list
                                {
                                    guid = dr_img["GUID"].ToString(),
                                    file_url = dr_img["FILE_URL"].ToString()
                                });
                            }
                           
                        }
                        item["file_list"] = images;
                    }
                    else
                    {
                        dr = dt_img.Select($"UNION_ID='{item["id"]}'");
                        images = new List<file_list>();
                        foreach (DataRow dr_img in dr)//假如多个图片
                        {
                            images.Add(new file_list
                            {
                                guid = dr_img["GUID"].ToString(),
                                file_url = dr_img["FILE_URL"].ToString()
                            });
                        }
                        item["file_list"] = images;
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
        /// 首检记录明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getinspectionitem_recordhois(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id_str = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//id拼接字符串
                string[] arr = id_str.Split(',');
                string sql = $@"select INSPECTION_NAME,REMARK from qcm_f_art_cfm_cc_d where id in ('{string.Join("','", arr)}')";
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
        /// 拍照上传
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Input_file_list(object OBJ)
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
                List<file_list> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["file_list"].ToString()) : null;
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                var currDate = DateTime.Now;
                string sql = string.Empty;

                sql = $@"UPDATE QCM_F_ART_CFM_M SET MODIFYBY='{user}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                string commit = getcreate(user);
                if (list.Count > 0)
                {
                    sql = $@"select max(sorting) from qcm_f_art_cfm_pg where task_no='{task_no}'";
                  
                    string max_sorting = DB.GetString(sql);
                    if (string.IsNullOrWhiteSpace(max_sorting))
                    {
                        max_sorting = "0";
                    }
                    int indexs = Convert.ToInt32(max_sorting);
                    sql = string.Empty;
                    foreach (file_list item in list)
                    {
                        indexs++;
                        sql += $"insert into qcm_f_art_cfm_pg   (task_no,sorting,file_guid,createby,createdate,createtime) values('{task_no}','{indexs}','{item.guid}',{commit});";//添加对应的guid关联图片
                       
                    }
                    DB.ExecuteNonQuery("begin "+sql+" end;");
                }

                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 拍照上传-照片记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Out_file_list(object OBJ)
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
	ID,
    FILE_GUID,
	(createdate ||' '|| createtime)as data
FROM
	qcm_f_art_cfm_pg where task_no='{task_no}' order by id asc";
                DataTable dt = DB.GetDataTable(sql);
                dt.Columns.Add("file_list", Type.GetType("System.Object"));
                sql = $@"SELECT
	FILE_URL,
	FILE_NAME,
  GUID
FROM
	BDM_UPLOAD_FILE_ITEM";
                DataTable dt_files = DB.GetDataTable(sql);
                List<file_list> files = new List<file_list>();
                DataRow[] dr = null;
                foreach (DataRow item in dt.Rows)
                {
                    dr = dt_files.Select($@"GUID='{item["FILE_GUID"]}'");
                    files = new List<file_list>();
                    foreach (DataRow dr_file in dr)
                    {
                        files.Add(new file_list { file_url = dr_file["FILE_URL"].ToString(), guid = dr_file["GUID"].ToString(),file_name=dr_file["FILE_NAME"].ToString() });
                    }
                    item["file_list"] = files;
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
        /// 删除图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject delete_file(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//图片id
                string task_no = DB.GetString($@"select task_no from qcm_f_art_cfm_pg where id='{id}'");
                if (!string.IsNullOrEmpty(task_no))
                {
                    string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                    var currDate = DateTime.Now;
                    string Usql = $@"UPDATE QCM_F_ART_CFM_M SET MODIFYBY='{user}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE task_no='{task_no}'";
                    DB.ExecuteNonQuery(Usql);
                }
                string sql = $@"select count(1) from qcm_f_art_cfm_pg where id='{id}'";
                if (DB.GetInt32(sql) > 0)
                {
                    sql = $@"delete from qcm_f_art_cfm_pg where id='{id}'";
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
                ret.ErrMsg = "Deletion failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject delete_inspectionitem(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//图片id
                string sql = $@"select commit_inspection from qcm_f_art_cfm_cc where id='{id}'";
                if (!string.IsNullOrWhiteSpace(DB.GetString(sql)))
                {
                    sql = $@"delete from qcm_f_art_cfm_cc where id='{id}'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                   //ret.ErrMsg = "删除失败，不是新建的检验项不能删除";
                    ret.ErrMsg = "Deletion failed. Items that are not newly created cannot be deleted.";
                    ret.IsSuccess = false;
                    return ret;
                }
                ret.IsSuccess = true;
                //ret.ErrMsg = "删除成功";
                ret.ErrMsg = "successfully deleted";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                //ret.ErrMsg = "删除失败，原因：" + ex.Message;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 回车带出姓名
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_staff_name(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string staff_no = jarr.ContainsKey("staff_no") ? jarr["staff_no"].ToString() : "";//任务编号
                string sql = $@"select staff_name from hr001m where staff_no='{staff_no}'";
                string staff_name = DB.GetString(sql);
                if (string.IsNullOrWhiteSpace(staff_name))
                {
                    //ret.ErrMsg = $@"编号[{staff_no}]没有相关人员姓名，请确认是否填写正确";
                    ret.ErrMsg = $@"The number [{staff_no}] does not have the name of the relevant person, please confirm whether it is filled in correctly.";
                    ret.IsSuccess= false;
                    return ret;
                }

                ret.IsSuccess = true;
                ret.RetData1 = staff_name;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        ///提交签名
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Signature(object OBJ)
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
                string sign_res = jarr.ContainsKey("sign_res") ? jarr["sign_res"].ToString() : "";//签核结果
                string sign_idea = jarr.ContainsKey("sign_idea") ? jarr["sign_idea"].ToString() : "";//签核意见
                string staff_no = jarr.ContainsKey("staff_no") ? jarr["staff_no"].ToString() : "";//签名人编号
                string staff_name = jarr.ContainsKey("staff_name") ? jarr["staff_name"].ToString() : "";//签名人
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string input = getcreate(user);
                string sql = string.Empty;

                sql = $@"select staff_name from hr001m where staff_no='{staff_no}'";//先确认签名人是否为账号填写的签名人
                string staff_names = DB.GetString(sql);
                if (staff_names!= staff_name)
                {
                    //ret.ErrMsg = $@"人员姓名[{staff_name}]的账号不正确，请正确操作";
                    ret.ErrMsg = $@"The account number of staff name [{staff_name}] is incorrect, please operate correctly.";
                    ret.IsSuccess = false;
                    return ret;
                }
                var currDate = DateTime.Now;
                sql = $@"UPDATE QCM_F_ART_CFM_M SET MODIFYBY='{user}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"select nvl(max(sorting),0) from qcm_f_art_cfm_s_c where task_no='{task_no}'";
                int maxsorting = DB.GetInt32(sql);
                sql = $@"insert into qcm_f_art_cfm_s_c(task_no,sorting,sign_res,sign_idea,staff_no,createby,createdate,createtime)values('{task_no}','{maxsorting+ 1}','{sign_res}','{sign_idea}','{staff_no}',{input})";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            
            return ret;
        }
        /// <summary>
        ///首件检验未通过/首件检验通过
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Checkwhetheritpasses(object OBJ)
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
                string sign_res = jarr.ContainsKey("sign_res") ? jarr["sign_res"].ToString() : "";//结果
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = string.Empty;
                string input = getcreate(user);
                sql = $@"select nvl(max(sorting),0) from  qcm_f_art_cfm_s where task_no='{task_no}'";
                int maxsorting = DB.GetInt32(sql);
                sql = $@"insert into qcm_f_art_cfm_s(task_no,sorting,sign_res,createby,createdate,createtime) values('{task_no}','{maxsorting + 1}','{sign_res}',{input})";
                DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 返回按钮文字
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_buttontext(object OBJ)
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

                //check_res(0pass，1fail),sign_res(0 同意，1 不同意),commit_type(0 pass，1 fail)
                string button = string.Empty;
                string sql = $@"select count(*) from qcm_f_art_cfm_c where task_no='{task_no}'and check_res='1'";
                int failsum1 = DB.GetInt32(sql);//检验项目提交明细
              
                sql = $@"select count(*) from  qcm_f_art_cfm_s_c where task_no='{task_no}' and sign_res='1'";
                int failsum2  = DB.GetInt32(sql);//DQA/MQA提交明细

                sql = $@"select count(*) from qcm_f_art_cfm_cc_d where task_no='{task_no}' and commit_type='1'";
                int failsum3 = DB.GetInt32(sql);//项目配置——提交记录

                if(failsum1>0 || failsum2>0 || failsum3 > 0)
                {
                    button = "button_fail";
                }
                else
                {
                    button = "button_pass";
                }
                ret.IsSuccess = true;
                ret.RetData1 = button;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 返回签名记录数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Thesignaturerecord(object OBJ)
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
	b.staff_name,
	b.staff_post as jobtitle,
	case 
		when a.sign_res='0' then 'Agree' 
		when a.sign_res='1' then 'Disagree' 
	end as sign_res_name,
    a.sign_res,
	a.sign_idea,
	(a.createdate ||' '|| a.createtime) as createdate
FROM
	qcm_f_art_cfm_s_c a
LEFT JOIN hr001m  b on a.staff_no=b.staff_no where a.task_no='{task_no}'";
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
        /// 扫描返回设备
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_eq_info(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";//任务编号
                string sql = $@"select eq_info_name From bdm_eq_info_m where eq_info_no='{eq_info_no}'";
                string eq_info_name = DB.GetString(sql);
                ret.IsSuccess = true;
                ret.RetData1 = eq_info_name;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 扫描返回产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_production_line(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//产线编号

                string sql = $@"select production_line_name from bdm_production_line_m where production_line_code='{production_line_code}'";
                string production_line_name = DB.GetString(sql);
                ret.IsSuccess = true;
                ret.RetData1 = production_line_name;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        ///有换页的
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject OutMain_getlist2(object OBJ)
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

                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string where = string.Empty;

                string sql = string.Empty;
                sql = $@"SELECT
    b.id,
	a.TASK_NO,
	a.param_item_no,--参数项目编号
	a.param_item_name,--参数项目名称
	(b.CREATEDATE ||'  ' || b.CREATETIME) as CREATEDATE,--采集时间
	b.sop_standard,--SOP数据
	b.actual_standard--参数实际数据
FROM
	rqc_task_xj_item A
LEFT JOIN rqc_task_xj_item_c b on a.TASK_NO=b.TASK_NO and a.param_item_no=b.param_item_no where a.task_no='{task_no}' {where}";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));

                dt.Columns.Add("imginfo_list", Type.GetType("System.Object"));
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
    }
}

