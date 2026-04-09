using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QAAPI
{
    /// <summary>
    /// 品质异常
    /// </summary>
    public class Qualityabnormal
    {
        protected class file_list
        {
            public string guid { get; set; }
            public string file_name { get; set; }
            public string file_url { get; set; }

        }

        public class Polist
        {
            public string po { get; set; }
            public string se_id { get; set; }
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
        protected class qcm_abnormal_r_d
        {
            public string  id  { get; set; }
            public string task_no { get; set; }
            public string qa_cost_cate_no { get; set; }
            public string qa_cost_cate_name { get; set; }
            public string unit_price { get; set; }
            public string qa_cost_cate_u { get; set; }
            public string qty { get; set; }
        }
        protected class workshop_section
        {
            public string workshop_section_no { get; set; }
            public string workshop_section_name { get; set; }
           
        }

        protected class PO_List
        {
            public string po_list { get; set; }
            public string so_list { get; set; }
           
        }
        /// <summary>
        /// 品质异常主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Qualityabnormal_Main_getList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//关键字
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";//开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束时间
                string abnormal_level = jarr.ContainsKey("abnormal_level") ? jarr["abnormal_level"].ToString() : "";//级别
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and (a.problem_desc like '%{keyword}%' or a.prod_no like '%{keyword}%' or r.name_t like '%{keyword}%' or  a.plant_area like '%{keyword}%' or a.abnormal_level='{keyword}' or  (CASE
		when a.abnormal_level='0' then '普通品质异常'
		when a.abnormal_level='1' then '批量品质异常'
		when a.abnormal_level='2' then '重大品质异常'
	END) like '%{keyword}%'   or a.department_code IN (
SELECT
	DEPARTMENT_CODE
FROM
	(
		SELECT
			DEPARTMENT_CODE,
			DEPARTMENT_NAME
		FROM
			base005m
		UNION
			SELECT
				SUPPLIERS_CODE AS DEPARTMENT_CODE,
				SUPPLIERS_NAME AS DEPARTMENT_NAME
			FROM
				BASE003M
	)
WHERE
	DEPARTMENT_CODE LIKE '%{keyword}%'
OR DEPARTMENT_NAME LIKE '%{keyword}%'
))";
                }
                if (!string.IsNullOrWhiteSpace(abnormal_level))
                {
                    strwhere += $@" and a.abnormal_level  like '%{abnormal_level}%'";
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
              
                string sql = $@"SELECT
	a.task_no,--任务编号
	a.prod_no,--ART
    r.name_t as prod_name,--ART名称
	a.department_code,--责任部门
	'' as department_name,--责任部门
  CASE
		when a.abnormal_level='0' then 'Ordinary_Abnormal_Quality'
		when a.abnormal_level='1' then 'Abnormal_Batch_Quality'
		when a.abnormal_level='2' then 'Major_Quality_Anomalies'
	END as abnormal_level_name,--问题级别
	a.abnormal_level,--问题级别
	a.createdate,--日期
    a.createtime,
	CASE
		when a.closing_status='0' then 'Close'
		when a.closing_status='1' then 'Open'
	end as  closing_status_name,--状态
  a.closing_status,--编号
	a.problem_desc--问题描述
FROM
	qcm_abnormal_r_m a 
LEFT JOIN  bdm_rd_prod r on a.prod_no=r.prod_no
 where 1=1 {strwhere} order by a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));

                List<string> department_code_list = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    string department_code = item["department_code"].ToString();
                    if (!string.IsNullOrEmpty(department_code))
                        department_code_list.Add($@"'{department_code}'");
                }
                if (department_code_list.Count() > 0)
                {
                    sql = $@"
SELECT
	DEPARTMENT_CODE,
    DEPARTMENT_NAME
FROM
	(
		SELECT
			DEPARTMENT_CODE,
			DEPARTMENT_NAME
		FROM
			base005m
		UNION
			SELECT
				SUPPLIERS_CODE AS DEPARTMENT_CODE,
				SUPPLIERS_NAME AS DEPARTMENT_NAME
			FROM
				BASE003M
	)
WHERE
	DEPARTMENT_CODE IN({string.Join(",", department_code_list)})
";
                    DataTable dt_depart = DB.GetDataTable(sql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string department_code = item["department_code"].ToString();
                        var find = dt_depart.Select($@"DEPARTMENT_CODE='{department_code}'");
                        if (find.Length > 0)
                        {
                            item["department_name"] = find[0]["DEPARTMENT_NAME"].ToString();
                        }
                    }
                }

                int total = CommonBASE.GetPageDataTableCount(DB, sql);
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
        /// 厂区厂商数据（搜索）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_plant_area(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and plant_area like '%{keyword}%'";
                }
                string sql = $@"select plant_area from bdm_production_line_m where 1=1 {strwhere}";
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
        /// 责任部门
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_department(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and DEPARTMENT_CODE like '%{keyword}%' or DEPARTMENT_NAME like '%{keyword}%'";
                }
                string sql = $@"	SELECT * FROM (
                                    SELECT
	                                    DEPARTMENT_CODE,
	                                    DEPARTMENT_NAME
                                    FROM
	                                    base005m
                                    UNION
                                    SELECT
	                                    SUPPLIERS_CODE as DEPARTMENT_CODE,
	                                    SUPPLIERS_NAME as DEPARTMENT_NAME
                                    FROM
	                                    BASE003M ) where 1=1 {strwhere}";
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
        /// 返回异常类别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_abnormal_category(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $@" and abnormal_category_no like '%{keyword}%' or abnormal_category_name like '%{keyword}%'";
                }
                string sql = $@"select abnormal_category_no,abnormal_category_name from bdm_qa_a_cate_m where 1=1 {strwhere}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                //int total = CommonBASE.GetPageDataTableCount(DB, sql);
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
        /// ART扫描
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// 
        #region Before PO Change
        //        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArt_List(object OBJ)
        //        {
        //            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //            try
        //            {
        //                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //                string Data = ReqObj.Data.ToString();
        //                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
        //                string sql = $@"SELECT
        //    DISTINCT
        //	a.PROD_NO,--art
        //    r.name_t as prod_name,--art名称
        //	a.DEVELOP_SEASON,--季度
        //    a.SHOE_NO,--鞋型编号
        //	b.name_t,--鞋型
        //	d.IMAGE_GUID --鞋图编号
        //FROM
        //	bdm_rd_prod a
        //LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
        //LEFT JOIN  bdm_rd_prod r on a.prod_no=r.prod_no
        //LEFT JOIN QCM_SHOES_QA_RECORD_M d ON a.shoe_no = d.shoes_code
        //where a.PROD_NO='{prod_no}'";
        //                DataTable dt = DB.GetDataTable(sql);//ART带出数据
        //                if (dt.Rows.Count < 1)
        //                {
        //                    ret.ErrMsg = $@"The art[{prod_no}] has no data, please check whether it is correct";
        //                    ret.IsSuccess = false;
        //                    return ret;
        //                }
        //                dt.Columns.Add("file_list", Type.GetType("System.Object"));
        //                dt.Columns.Add("po_list", Type.GetType("System.Object"));
        //                dt.Columns.Add("po_list2", Type.GetType("System.Object"));
        //                dt.Columns.Add("workshop_section_list", Type.GetType("System.Object"));
        //                sql = $@"SELECT
        //      DISTINCT
        //	    A.SHOES_CODE,
        //      B.WORKSHOP_SECTION_NO,
        //      B.WORKSHOP_SECTION_NAME
        //  FROM
        //      QCM_DQA_MAG_M A LEFT JOIN BDM_PARAM_ITEM_D B  ON A.WORKSHOP_SECTION_NO=B.WORKSHOP_SECTION_NO";
        //                DataTable dtworkshop_section = DB.GetDataTable(sql);//工段(art下面的鞋型=》鞋型的工段有关)

        //                sql = $@"SELECT
        //                    DISTINCT
        //                	b.MER_PO,
        //                	a.PROD_NO
        //                FROM
        //                	BDM_SE_ORDER_ITEM a
        //                LEFT JOIN BDM_SE_ORDER_MASTER b on a.SE_ID=b.SE_ID";

        //                sql = $@"SELECT
        //    DISTINCT
        //  b.MER_PO,b.se_id,
        //  a.PROD_NO
        //FROM
        //  BDM_SE_ORDER_ITEM a
        //LEFT JOIN BDM_SE_ORDER_MASTER b on a.SE_ID=b.SE_ID";

        //                DataTable dt_polist = DB.GetDataTable(sql);
        //                DataRow[] dr = null;
        //                DataTable dtimg = new DataTable();
        //                List<string> po_list = new List<string>();
        //                List<file_list> files = new List<file_list>();
        //                List<workshop_section> workshop_section_list = new List<workshop_section>();
        //                List<PO_List> PO_List = new List<PO_List>();//Added for PO change project
        //                foreach (DataRow item in dt.Rows)
        //                {
        //                    sql = $"select file_url,guid From BDM_UPLOAD_FILE_ITEM where guid='{item["IMAGE_GUID"]}'";
        //                    dtimg = DB.GetDataTable(sql);
        //                    files = new List<file_list>();
        //                    item["file_list"] = files;
        //                    foreach (DataRow img in dtimg.Rows)
        //                    {
        //                        files.Add(new file_list
        //                        {
        //                            file_url = img["FILE_URL"].ToString(),
        //                            guid = img["GUID"].ToString()
        //                        });
        //                    }
        //                    item["file_list"] = files;
        //                    dr = dt_polist.Select($@"PROD_NO='{item["PROD_NO"]}'");
        //                    po_list = new List<string>();
        //                    foreach (DataRow po in dr)
        //                    {
        //                        PO_List.Add(new PO_List
        //                        {
        //                            po_list = po["MER_PO"].ToString(),//工段编号
        //                            so_list = po["se_id"].ToString()//工段名称     //After PO change Project
        //                        });

        //                        po_list.Add(po["MER_PO"].ToString());//po号码//Before PO change Project
        //                    }
        //                    item["po_list"] = PO_List;

        //                    item["po_list"] = po_list;
        //                    dr = dtworkshop_section.Select($@"SHOES_CODE='{item["SHOE_NO"]}'");
        //                    workshop_section_list = new List<workshop_section>();
        //                    foreach (DataRow workshop in dr)
        //                    {

        //                        workshop_section_list.Add(new workshop_section
        //                        {
        //                            workshop_section_no = workshop["WORKSHOP_SECTION_NO"].ToString(),//工段编号
        //                            workshop_section_name = workshop["WORKSHOP_SECTION_NAME"].ToString()//工段名称
        //                        });

        //                    }
        //                    item["workshop_section_list"] = workshop_section_list;
        //                }

        //                sql = $@"
        //SELECT
        //	production_line_code,
        //	production_line_name,
        //	plant_area
        //FROM
        //	bdm_production_line_m
        //UNION
        //	SELECT
        //		DEPARTMENT_CODE AS production_line_code,
        //		DEPARTMENT_NAME AS production_line_name,
        //		'' AS plant_area
        //	FROM
        //		BASE005M                    
        //                    ";
        //                DataTable dt4 = DB.GetDataTable(sql);//生产线

        //                ret.IsSuccess = true;
        //                Dictionary<string, object> dic = new Dictionary<string, object>();
        //                dic.Add("top", dt);
        //                dic.Add("production_line_list", dt4);
        //                ret.RetData1 = dic;

        //            }
        //            catch (Exception ex)
        //            {
        //                ret.IsSuccess = false;
        //                ret.ErrMsg = ex.Message;
        //            }
        //            return ret;
        //        }

        #endregion

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArt_List(object OBJ)
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
                string sql = $@"SELECT
    DISTINCT
	a.PROD_NO,--art
    r.name_t as prod_name,--art名称
	a.DEVELOP_SEASON,--季度
    a.SHOE_NO,--鞋型编号
	b.name_t,--鞋型
	d.IMAGE_GUID --鞋图编号
FROM
	bdm_rd_prod a
LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
LEFT JOIN  bdm_rd_prod r on a.prod_no=r.prod_no
LEFT JOIN QCM_SHOES_QA_RECORD_M d ON a.shoe_no = d.shoes_code
where a.PROD_NO='{prod_no}'";
                DataTable dt = DB.GetDataTable(sql);//ART带出数据
                if (dt.Rows.Count < 1)
                {
                    ret.ErrMsg = $@"该art[{prod_no}]无数据带出，请检查是否正确";
                    ret.IsSuccess = false;
                    return ret;
                }
                dt.Columns.Add("file_list", Type.GetType("System.Object"));
                dt.Columns.Add("po_list", Type.GetType("System.Object"));
                dt.Columns.Add("po_list2", Type.GetType("System.Object"));
                dt.Columns.Add("workshop_section_list", Type.GetType("System.Object"));
                sql = $@"SELECT
      DISTINCT
	    A.SHOES_CODE,
      B.WORKSHOP_SECTION_NO,
      B.WORKSHOP_SECTION_NAME
  FROM
      QCM_DQA_MAG_M A LEFT JOIN BDM_PARAM_ITEM_D B  ON A.WORKSHOP_SECTION_NO=B.WORKSHOP_SECTION_NO";
                DataTable dtworkshop_section = DB.GetDataTable(sql);//工段(art下面的鞋型=》鞋型的工段有关)

                sql = $@"SELECT
    DISTINCT
	b.MER_PO,
	a.PROD_NO,
    b.MER_PO||','||a.SE_ID as SE_ID
FROM
	BDM_SE_ORDER_ITEM a
LEFT JOIN BDM_SE_ORDER_MASTER b on a.SE_ID=b.SE_ID";

                DataTable dt_polist = DB.GetDataTable(sql);
                DataRow[] dr = null;
                DataTable dtimg = new DataTable();
                List<string> po_list = new List<string>();
                List<Polist> po_list2 = new List<Polist>();
                List<file_list> files = new List<file_list>();
                List<workshop_section> workshop_section_list = new List<workshop_section>();
                foreach (DataRow item in dt.Rows)
                {
                    sql = $"select file_url,guid From BDM_UPLOAD_FILE_ITEM where guid='{item["IMAGE_GUID"]}'";
                    dtimg = DB.GetDataTable(sql);
                    files = new List<file_list>();
                    item["file_list"] = files;
                    foreach (DataRow img in dtimg.Rows)
                    {
                        files.Add(new file_list
                        {
                            file_url = img["FILE_URL"].ToString(),
                            guid = img["GUID"].ToString()
                        });
                    }
                    item["file_list"] = files;
                    dr = dt_polist.Select($@"PROD_NO='{item["PROD_NO"]}'");
                    po_list = new List<string>();
                    foreach (DataRow po in dr)
                    {
                        po_list.Add(po["MER_PO"].ToString());//po号码
                        po_list2.Add(new Polist() { po = po["MER_PO"].ToString(), se_id = po["SE_ID"].ToString() });
                    }
                    item["po_list"] = po_list;
                    item["po_list2"] = po_list2;
                    // dr = dtworkshop_section.Select($@"SHOES_CODE='{item["SHOE_NO"]}'");
                    string work_no = $@"select WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME from bdm_workshop_section_m";
                    DataTable dataTable = DB.GetDataTable(work_no);
                    workshop_section_list = new List<workshop_section>();
                    foreach (DataRow workshop in dataTable.Rows)
                    {

                        workshop_section_list.Add(new workshop_section
                        {
                            workshop_section_no = workshop["WORKSHOP_SECTION_NO"].ToString(),//工段编号
                            workshop_section_name = workshop["WORKSHOP_SECTION_NAME"].ToString()//工段名称
                        });

                    }
                    item["workshop_section_list"] = workshop_section_list;
                }

                sql = $@"
SELECT
	production_line_code,
	production_line_name,
	plant_area
FROM
	bdm_production_line_m
UNION
	 SELECT
  DEPARTMENT_CODE AS production_line_code,
    DEPARTMENT_NAME AS production_line_name,
    b.ORG AS plant_area
  FROM
    BASE005M a left join SJQDMS_ORGINFO b on a.UDF05 = b.CODE  
UNION 
    select SUPPLIERS_CODE,SUPPLIERS_NAME,'' as plant_area from base003m  
                    ";
                DataTable dt4 = DB.GetDataTable(sql);//生产线

                ret.IsSuccess = true;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("top", dt);
                dic.Add("production_line_list", dt4);
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
        /// 保存草稿
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Input_Main_Data(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型编号
                string pro_month = jarr.ContainsKey("pro_month") ? jarr["pro_month"].ToString() : "";//生产月份
                string plant_area = jarr.ContainsKey("plant_area") ? jarr["plant_area"].ToString() : "";//产区
                string abnormal_level = jarr.ContainsKey("abnormal_level") ? jarr["abnormal_level"].ToString() : "";//异常等级
                string abnormal_category_no = jarr.ContainsKey("abnormal_category_no") ? jarr["abnormal_category_no"].ToString() : "";//异常类别
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//工段
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//生产线
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string po_list = jarr.ContainsKey("po_list") ? jarr["po_list"].ToString() : "";//po号码
                string se_list = jarr.ContainsKey("se_list") ? jarr["se_list"].ToString() : "";//po号码
                string fx_qty = jarr.ContainsKey("fx_qty") ? jarr["fx_qty"].ToString() : "";//翻箱数量
                string problem_desc = jarr.ContainsKey("problem_desc") ? jarr["problem_desc"].ToString() : "";//问题描述
                string department_code = jarr.ContainsKey("department_code") ? jarr["department_code"].ToString() : "";//责任部门
                string department_codecc = jarr.ContainsKey("department_codecc") ? jarr["department_codecc"].ToString() : "";//问题详情
                string f_measures = jarr.ContainsKey("f_measures") ? jarr["f_measures"].ToString() : "";//救火措施
                string cause_analysis = jarr.ContainsKey("cause_analysis") ? jarr["cause_analysis"].ToString() : "";//原因分析
                string preventive_measure = jarr.ContainsKey("preventive_measure") ? jarr["preventive_measure"].ToString() : "";//预防措施
                string improve_results = jarr.ContainsKey("improve_results") ? jarr["improve_results"].ToString() : "";//改善结果
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string closing_status = jarr.ContainsKey("closing_status") ? jarr["closing_status"].ToString() : "";//结案状态
                string top_disabled = jarr.ContainsKey("top_disabled") ? jarr["top_disabled"].ToString() : "";//结案状态
                
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string m_id = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                
                sql = $"select task_no from qcm_abnormal_r_m where task_no='{task_no}'";
                task_no = DB.GetString(sql);
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = $@"select id from qcm_abnormal_r_m where task_no='{task_no}'";
                    m_id = DB.GetString(sql);
                    if (!Convert.ToBoolean(top_disabled))//false就是不禁用 True是本人修改的就可以修改上部分
                    {
                     
                        sql = "update qcm_abnormal_r_m set abnormal_level=@abnormal_level,abnormal_category_no=@abnormal_category_no,prod_no=@prod_no,shoe_no=@shoe_no,develop_season=@develop_season,pro_month=@pro_month,workshop_section_no=@workshop_section_no,production_line_code=@production_line_code,plant_area=@plant_area,po_list=@po_list,fx_qty=@fx_qty,problem_desc=@problem_desc,department_code=@department_code,department_codecc=@department_codecc,f_measures=@f_measures,cause_analysis=@cause_analysis,preventive_measure=@preventive_measure,improve_results=@improve_results,closing_status=@closing_status,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime,se_list=@SE_LIST where task_no=@task_no";
                        dic.Add("abnormal_level", abnormal_level);
                        dic.Add("abnormal_category_no", abnormal_category_no);
                        dic.Add("prod_no", prod_no);
                        dic.Add("shoe_no", shoe_no);
                        dic.Add("develop_season", develop_season);
                        dic.Add("pro_month", pro_month);
                        dic.Add("workshop_section_no", workshop_section_no);
                        dic.Add("production_line_code", production_line_code);
                        dic.Add("plant_area", plant_area);
                        dic.Add("po_list", po_list);
                        dic.Add("fx_qty", fx_qty);
                        dic.Add("problem_desc", problem_desc);
                        dic.Add("department_code", department_code);
                        dic.Add("department_codecc", department_codecc);
                        dic.Add("f_measures", f_measures);
                        dic.Add("cause_analysis", cause_analysis);
                        dic.Add("preventive_measure", preventive_measure);
                        dic.Add("improve_results", improve_results);
                        dic.Add("closing_status", closing_status);
                        dic.Add("modifyby", user);
                        dic.Add("modifydate", date);
                        dic.Add("modifytime", time);
                        dic.Add("SE_LIST", se_list.TrimEnd(','));
                        dic.Add("task_no", task_no);//条件
                        DB.ExecuteNonQuery(sql, dic);

                        List<file_list> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["file_list"].ToString()) : null;//问题图片
                        if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='0' ") > 0)
                        {
                            sql = $@"delete from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='0'";
                            DB.ExecuteNonQuery(sql);
                        }
                        if (list.Count > 0)
                        {
                            sql = string.Empty;
                            foreach (file_list item in list)
                            {
                                sql += $@"insert into qcm_abnormal_r_m_f(union_id,file_guid,source_type,createby,createdate,createtime)values('{m_id}','{item.guid}','0','{user}','{date}','{time}');";
                            }
                            DB.ExecuteNonQuery("begin " + sql + " end;");
                        }
                    }
                    else
                    {
                        //禁用的时候,只能修改一部分
                        sql = "update qcm_abnormal_r_m set f_measures=@f_measures,cause_analysis=@cause_analysis,preventive_measure=@preventive_measure,improve_results=@improve_results,closing_status=@closing_status,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where task_no=@task_no";
                        dic.Add("f_measures", f_measures);
                        dic.Add("cause_analysis", cause_analysis);
                        dic.Add("preventive_measure", preventive_measure);
                        dic.Add("improve_results", improve_results);
                        dic.Add("closing_status", closing_status);
                        dic.Add("modifyby", user);
                        dic.Add("modifydate", date);
                        dic.Add("modifytime", time);
                        dic.Add("task_no", task_no);//条件
                        DB.ExecuteNonQuery(sql, dic);

                    }
                }
                else
                {
                    task_no = "R" + date;
                    string maxtask_no = DB.GetString($@"select max(task_no) from qcm_abnormal_r_m where task_no like '{task_no}%'");
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

                    sql = $@"insert into qcm_abnormal_r_m(task_no,abnormal_level,abnormal_category_no,prod_no,shoe_no,develop_season,pro_month,workshop_section_no,production_line_code,plant_area,po_list,fx_qty,problem_desc,department_code,department_codecc,f_measures,cause_analysis,preventive_measure,improve_results,closing_status,createby,createdate,createtime,se_list) values(@task_no,@abnormal_level,@abnormal_category_no,@prod_no,@shoe_no,@develop_season,@pro_month,@workshop_section_no,@production_line_code,@plant_area,@po_list,@fx_qty,@problem_desc,@department_code,@department_codecc,@f_measures,@cause_analysis,@preventive_measure,@improve_results,@closing_status,@createby,@createdate,@createtime,@SE_LIST)";
                    dic = new Dictionary<string, object>();
                    dic.Add("task_no", task_no);
                    dic.Add("abnormal_level", abnormal_level);
                    dic.Add("abnormal_category_no", abnormal_category_no);
                    dic.Add("prod_no", prod_no);
                    dic.Add("shoe_no", shoe_no);
                    dic.Add("develop_season", develop_season);
                    dic.Add("pro_month", pro_month);
                    dic.Add("workshop_section_no", workshop_section_no);
                    dic.Add("production_line_code", production_line_code);
                    dic.Add("plant_area", plant_area);
                    dic.Add("po_list", po_list);
                    dic.Add("fx_qty", fx_qty);
                    dic.Add("problem_desc", problem_desc);
                    dic.Add("department_code", department_code);
                    dic.Add("department_codecc", department_codecc);
                    dic.Add("f_measures", f_measures);
                    dic.Add("cause_analysis", cause_analysis);
                    dic.Add("preventive_measure", preventive_measure);
                    dic.Add("improve_results", improve_results);
                    dic.Add("closing_status", closing_status);
                    dic.Add("createby", user);
                    dic.Add("createdate", date);
                    dic.Add("createtime", time);
                    dic.Add("SE_LIST", se_list.TrimEnd(','));

                    DB.ExecuteNonQuery(sql, dic);

                    m_id= SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_abnormal_r_m");


                    List<file_list> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["file_list"].ToString()) : null;//问题图片
                    if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='0' ") > 0)
                    {
                        sql = $@"delete from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='0'";
                        DB.ExecuteNonQuery(sql);
                    }
                    if (list.Count > 0)
                    {
                        sql = string.Empty;
                        foreach (file_list item in list)
                        {
                            sql += $@"insert into qcm_abnormal_r_m_f(union_id,file_guid,source_type,createby,createdate,createtime)values('{m_id}','{item.guid}','0','{user}','{date}','{time}');";
                        }
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                    }
                }
               
                List<file_list> yes_list = jarr.ContainsKey("yes_file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["yes_file_list"].ToString()) : null;//解决后的图片
                List<qcm_abnormal_r_d> bittom_list = jarr.ContainsKey("bittom_list") ? JsonConvert.DeserializeObject<List<qcm_abnormal_r_d>>(jarr["bittom_list"].ToString()) : null;//品质异常成本
                //问题图片
                //解决后图片
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='1' ") > 0)
                {
                    sql = $@"delete from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='1'";
                    DB.ExecuteNonQuery(sql);
                }
                if (yes_list.Count > 0)
                {
                    sql = string.Empty;
                    foreach (file_list item in yes_list)
                    {
                        sql += $@"insert into qcm_abnormal_r_m_f(union_id,file_guid,source_type,createby,createdate,createtime)values('{m_id}','{item.guid}','1','{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }
                //底部品质异常成本
                if (bittom_list.Count > 0)
                {
                    
                    sql= string.Empty;
                    foreach (qcm_abnormal_r_d item in bittom_list)
                    {
                        //存在就修改
                        if (!string.IsNullOrWhiteSpace(item.id))
                        {
                            sql += $@"update qcm_abnormal_r_d set qa_cost_cate_no='{item.qa_cost_cate_no}',qa_cost_cate_name='{item.qa_cost_cate_name}',unit_price='{item.unit_price}',qa_cost_cate_u='{item.qa_cost_cate_u}',qty='{item.qty}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{item.id}';";
                        }
                        else
                        {
                            sql += $@"insert into qcm_abnormal_r_d(task_no,qty,qa_cost_cate_no,qa_cost_cate_name,unit_price,qa_cost_cate_u,createby,createdate,createtime)values('{task_no}','{item.qty}','{item.qa_cost_cate_no}','{item.qa_cost_cate_name}','{item.unit_price}','{item.qa_cost_cate_u}','{user}','{date}','{time}');";
                        }

                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "保存成功";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "保存失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 返回保存草稿的数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetInput_Datea(object OBJ)
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
                string CompanyCode = jarr.ContainsKey("CompanyCode") ? jarr["CompanyCode"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = $@"SELECT
  a.ID,--ID
	a.task_no,--任务编号
	a.abnormal_level,--品质异常级别
     CASE
		when a.abnormal_level='0' then '普通品质异常'
		when a.abnormal_level='1' then '批量品质异常'
		when a.abnormal_level='2' then '重大品质异常'
	END as abnormal_level_name,--问题级别
	a.abnormal_category_no,--异常类型编号
    e.abnormal_category_name,--异常类型
	a.prod_no,--ART
    r.name_t as prod_name,--ART名称
	a.shoe_no,--鞋型
    f.name_t,--鞋型
	a.develop_season,--季度
	a.pro_month,--生产年月
	a.workshop_section_no,--工段编号
	b.workshop_section_name,--工段名称
	a.production_line_code,--生产线编号
	d.production_line_name,--生产线名称
	a.plant_area,--产区
	a.po_list,--po
	 a.SE_LIST,--销售订单
	a.fx_qty,--翻箱数量
	a.problem_desc,--问题描述
	a.department_code,--责任部门编号
	c.department_name,--责任部门
	a.department_codecc,--问题详情
	a.f_measures,--救活措施
	a.cause_analysis,--原因分析
	a.preventive_measure,--预防措施
	a.improve_results,--改善结果
    a.createby,
    k.IMAGE_GUID, --鞋图编号
    a.closing_status --状态
FROM
	qcm_abnormal_r_m a 
left join BDM_PARAM_ITEM_D b on a.workshop_section_no=b.workshop_section_no
LEFT JOIN  bdm_rd_prod r on a.prod_no=r.prod_no
left join base005m c on a.department_code=c.department_code
left join bdm_production_line_m d on a.production_line_code=d.production_line_code
left join bdm_qa_a_cate_m e on a.abnormal_category_no=e.abnormal_category_no
LEFT JOIN QCM_SHOES_QA_RECORD_M k ON a.shoe_no = k.shoes_code
LEFT JOIN BDM_RD_STYLE f on a.SHOE_NO=f.SHOE_NO
where a.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    DataRow dtRow = dt.Rows[0];
                    string department_code = dtRow["department_code"].ToString();
                    string department_name = dtRow["department_name"].ToString();
                    if (!string.IsNullOrEmpty(department_code) && string.IsNullOrEmpty(department_name))
                    {
                        var p_dic = new Dictionary<string, object>();
                        p_dic.Add("SUPPLIERS_CODE", department_code);
                        dtRow["department_name"] = DB.GetString($@"
                                    SELECT
	                                    SUPPLIERS_NAME 
                                    FROM
	                                    BASE003M WHERE SUPPLIERS_CODE=@SUPPLIERS_CODE
                                    ", p_dic);
                    }
                    string production_line_code = dtRow["production_line_code"].ToString();
                    string production_line_name = dtRow["production_line_name"].ToString();
                    if (!string.IsNullOrEmpty(production_line_code) && string.IsNullOrEmpty(production_line_name))
                    {
                        var p_dic = new Dictionary<string, object>();
                        p_dic.Add("DEPARTMENT_CODE", production_line_code);
                        dtRow["production_line_name"] = DB.GetString($@"
                                    SELECT DEPARTMENT_NAME FROM BASE005M WHERE DEPARTMENT_CODE=@DEPARTMENT_CODE
                                    ", p_dic);
                    }
                }

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID
FROM
	QCM_ABNORMAL_R_M_F A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_GUID = B.GUID where A.SOURCE_TYPE='0'";
                DataTable file_dt = DB.GetDataTable(sql);//问题图片

                sql = $@"SELECT
    A.UNION_ID,
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID
FROM
	QCM_ABNORMAL_R_M_F A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_GUID = B.GUID where A.SOURCE_TYPE='1'";
                DataTable yes_file_dt = DB.GetDataTable(sql);//解决后的图片

                sql = $@"SELECT
	ID,
	TASK_NO,
	QA_COST_CATE_NO,
    QA_COST_CATE_NAME,
	UNIT_PRICE,
	QA_COST_CATE_U,
    QTY
FROM
	QCM_ABNORMAL_R_D where task_no='{task_no}'";
                DataTable bittom_dt = DB.GetDataTable(sql);//品质异常成本数据
                dt.Columns.Add("top_disabled", Type.GetType("System.Object"));
                dt.Columns.Add("art_file_list", Type.GetType("System.Object"));
                dt.Columns.Add("file_list", Type.GetType("System.Object"));
                dt.Columns.Add("yes_file_list", Type.GetType("System.Object"));
                dt.Columns.Add("bittom_list", Type.GetType("System.Object"));
                DataRow [] dr = null;
                DataTable dtimg = new DataTable();
                List<file_list> files = new List<file_list>();
                 List<file_list> images = new List<file_list>();
                foreach (DataRow item in dt.Rows)
                {
                    sql = $"select file_url,guid From BDM_UPLOAD_FILE_ITEM where guid='{item["IMAGE_GUID"]}'";
                    dtimg = DB.GetDataTable(sql);
                    files = new List<file_list>();
                    item["art_file_list"] = files;
                    foreach (DataRow img in dtimg.Rows)
                    {
                        files.Add(new file_list
                        {
                            file_url = img["FILE_URL"].ToString(),
                            guid = img["GUID"].ToString()
                        });
                    }
                    item["art_file_list"] = files;

                    if (item["CREATEBY"].ToString().Equals(user))//按钮禁用，如果是本人不需要禁用
                    {
                        item["top_disabled"] = false;
                    }
                    else
                    {
                        item["top_disabled"] = true;
                    }

                    //问题图片
                    dr = file_dt.Select($"UNION_ID='{item["ID"]}'");
                    images = new List<file_list>();
                    foreach (DataRow dr_file in dr)
                    {
                        images.Add(new file_list
                        {
                            guid = dr_file["GUID"].ToString(),
                            file_url = dr_file["FILE_URL"].ToString(),
                            file_name = dr_file["FILE_NAME"].ToString()
                        });
                    }
                    item["file_list"] = images;

                    //解决后图片
                    dr = yes_file_dt.Select($"UNION_ID='{item["ID"]}'");
                    images = new List<file_list>();
                    foreach (DataRow dr_yes_file in dr)
                    {
                        images.Add(new file_list
                        {
                            guid = dr_yes_file["GUID"].ToString(),
                            file_url = dr_yes_file["FILE_URL"].ToString(),
                            file_name = dr_yes_file["FILE_NAME"].ToString()
                        });
                    }
                    item["yes_file_list"] = images;

                    //底部的品质异常成本
                    item["bittom_list"] = bittom_dt;

                }

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
        /// 结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Settlealawsuit_jz(object OBJ)
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
                string sql = string.Empty;
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m where task_no='{task_no}'") > 0)
                {
                    sql = $@"update qcm_abnormal_r_m set closing_status='1' where task_no='{task_no}'";
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "结案成功";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "结案失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Settlealawsuit(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型编号
                string pro_month = jarr.ContainsKey("pro_month") ? jarr["pro_month"].ToString() : "";//生产月份
                string plant_area = jarr.ContainsKey("plant_area") ? jarr["plant_area"].ToString() : "";//产区
                string abnormal_level = jarr.ContainsKey("abnormal_level") ? jarr["abnormal_level"].ToString() : "";//异常等级
                string abnormal_category_no = jarr.ContainsKey("abnormal_category_no") ? jarr["abnormal_category_no"].ToString() : "";//异常类别
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//工段
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//生产线
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string po_list = jarr.ContainsKey("po_list") ? jarr["po_list"].ToString() : "";//po号码
                string fx_qty = jarr.ContainsKey("fx_qty") ? jarr["fx_qty"].ToString() : "";//翻箱数量
                string problem_desc = jarr.ContainsKey("problem_desc") ? jarr["problem_desc"].ToString() : "";//问题描述
                string department_code = jarr.ContainsKey("department_code") ? jarr["department_code"].ToString() : "";//责任部门
                string department_codecc = jarr.ContainsKey("department_codecc") ? jarr["department_codecc"].ToString() : "";//问题详情
                string f_measures = jarr.ContainsKey("f_measures") ? jarr["f_measures"].ToString() : "";//救火措施
                string cause_analysis = jarr.ContainsKey("cause_analysis") ? jarr["cause_analysis"].ToString() : "";//原因分析
                string preventive_measure = jarr.ContainsKey("preventive_measure") ? jarr["preventive_measure"].ToString() : "";//预防措施
                string improve_results = jarr.ContainsKey("improve_results") ? jarr["improve_results"].ToString() : "";//改善结果
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                
                string closing_status= jarr.ContainsKey("closing_status") ? jarr["closing_status"].ToString() : "";//结案
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string m_id = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(task_no))
                {
                    sql = $@"select count(1) from qcm_abnormal_r_m where task_no='{task_no}' and createby='{user}'";
                    if (DB.GetInt32(sql) < 1)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "除填报人以外人员不可编辑修改";
                        return ret;
                    }
                    sql = "update qcm_abnormal_r_m set abnormal_level=@abnormal_level,abnormal_category_no=@abnormal_category_no,prod_no=@prod_no,shoe_no=@shoe_no,develop_season=@develop_season,pro_month=@pro_month,workshop_section_no=@workshop_section_no,production_line_code=@production_line_code,plant_area=@plant_area,po_list=@po_list,fx_qty=@fx_qty,problem_desc=@problem_desc,department_code=@department_code,department_codecc=@department_codecc,f_measures=@f_measures,cause_analysis=@cause_analysis,preventive_measure=@preventive_measure,improve_results=@improve_results,closing_status=@closing_status,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where task_no=@task_no";
                    dic.Add("abnormal_level", abnormal_level);
                    dic.Add("abnormal_category_no", abnormal_category_no);
                    dic.Add("prod_no", prod_no);
                    dic.Add("shoe_no", shoe_no);
                    dic.Add("develop_season", develop_season);
                    dic.Add("pro_month", pro_month);
                    dic.Add("workshop_section_no", workshop_section_no);
                    dic.Add("production_line_code", production_line_code);
                    dic.Add("plant_area", plant_area);
                    dic.Add("po_list", po_list);
                    dic.Add("fx_qty", fx_qty);
                    dic.Add("problem_desc", problem_desc);
                    dic.Add("department_code", department_code);
                    dic.Add("department_codecc", department_codecc);
                    dic.Add("f_measures", f_measures);
                    dic.Add("cause_analysis", cause_analysis);
                    dic.Add("preventive_measure", preventive_measure);
                    dic.Add("improve_results", improve_results);
                    dic.Add("closing_status", closing_status);//结案
                    dic.Add("modifyby", user);
                    dic.Add("modifydate", date);
                    dic.Add("modifytime", time);
                    dic.Add("task_no", task_no);//条件
                    DB.ExecuteNonQuery(sql, dic);

                    sql = $@"select id from qcm_abnormal_r_m where task_no='{task_no}'";
                    m_id = DB.GetString(sql);
                }
                else
                {
                    task_no = "R" + date;
                    string maxtask_no = DB.GetString($@"select max(task_no) from qcm_abnormal_r_m where task_no like '{task_no}%'");
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

                    sql = $@"insert into qcm_abnormal_r_m(task_no,abnormal_level,abnormal_category_no,prod_no,shoe_no,develop_season,pro_month,workshop_section_no,production_line_code,plant_area,po_list,fx_qty,problem_desc,department_code,department_codecc,f_measures,cause_analysis,preventive_measure,improve_results,closing_status,createby,createdate,createtime) values(@task_no,@abnormal_level,@abnormal_category_no,@prod_no,@shoe_no,@develop_season,@pro_month,@workshop_section_no,@production_line_code,@plant_area,@po_list,@fx_qty,@problem_desc,@department_code,@department_codecc,@f_measures,@cause_analysis,@preventive_measure,@improve_results,@closing_status,@createby,@createdate,@createtime)";
                    dic = new Dictionary<string, object>();
                    dic.Add("task_no", task_no);
                    dic.Add("abnormal_level", abnormal_level);
                    dic.Add("abnormal_category_no", abnormal_category_no);
                    dic.Add("prod_no", prod_no);
                    dic.Add("shoe_no", shoe_no);
                    dic.Add("develop_season", develop_season);
                    dic.Add("pro_month", pro_month);
                    dic.Add("workshop_section_no", workshop_section_no);
                    dic.Add("production_line_code", production_line_code);
                    dic.Add("plant_area", plant_area);
                    dic.Add("po_list", po_list);
                    dic.Add("fx_qty", fx_qty);
                    dic.Add("problem_desc", problem_desc);
                    dic.Add("department_code", department_code);
                    dic.Add("department_codecc", department_codecc);
                    dic.Add("f_measures", f_measures);
                    dic.Add("cause_analysis", cause_analysis);
                    dic.Add("preventive_measure", preventive_measure);
                    dic.Add("improve_results", improve_results);
                    dic.Add("closing_status", closing_status);//结案
                    dic.Add("createby", user);
                    dic.Add("createdate", date);
                    dic.Add("createtime", time);
                    DB.ExecuteNonQuery(sql, dic);

                    m_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_abnormal_r_m");
                }
                List<file_list> list = jarr.ContainsKey("file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["file_list"].ToString()) : null;//问题图片
                List<file_list> yes_list = jarr.ContainsKey("yes_file_list") ? JsonConvert.DeserializeObject<List<file_list>>(jarr["yes_file_list"].ToString()) : null;//解决后的图片
                List<qcm_abnormal_r_d> bittom_list = jarr.ContainsKey("bittom_list") ? JsonConvert.DeserializeObject<List<qcm_abnormal_r_d>>(jarr["bittom_list"].ToString()) : null;//品质异常成本
                //问题图片
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='0' ") > 0)
                {
                    sql = $@"delete from qcm_abnormal_r_m_f where union_id='{m_id}'";
                    DB.ExecuteNonQuery(sql);
                }
                if (list.Count > 0)
                {
                    sql = string.Empty;
                    foreach (file_list item in list)
                    {
                        sql += $@"insert into qcm_abnormal_r_m_f(union_id,file_guid,source_type,createby,createdate,createtime)values('{m_id}','{item.guid}','0','{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }

                //解决后图片
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m_f where union_id='{m_id}' and source_type='1' ") > 0)
                {
                    sql = $@"delete from qcm_abnormal_r_m_f where union_id='{m_id}'";
                    DB.ExecuteNonQuery(sql);
                }
                if (yes_list.Count > 0)
                {
                    sql = string.Empty;
                    foreach (file_list item in list)
                    {
                        sql += $@"insert into qcm_abnormal_r_m_f(union_id,file_guid,source_type,createby,createdate,createtime)values('{m_id}','{item.guid}','1','{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }
                //底部品质异常成本
                if (bittom_list.Count > 0)
                {

                    sql = string.Empty;
                    foreach (qcm_abnormal_r_d item in bittom_list)
                    {
                        //存在就修改
                        if (!string.IsNullOrWhiteSpace(item.id))
                        {
                            sql += $@"update qcm_abnormal_r_d set qa_cost_cate_no='{item.qa_cost_cate_no}',qa_cost_cate_name='{item.qa_cost_cate_name}',unit_price='{item.unit_price}',qa_cost_cate_u='{item.qa_cost_cate_u}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{item.id}';";
                        }
                        else
                        {
                            sql += $@"insert into qcm_abnormal_r_d(task_no,qa_cost_cate_no,qa_cost_cate_name,unit_price,qa_cost_cate_u,createby,createdate,createtime)values('{task_no}','{item.qa_cost_cate_no}','{item.qa_cost_cate_name}','{item.unit_price}','{item.qa_cost_cate_u}','{user}','{date}','{time}');";
                        }

                    }
                    DB.ExecuteNonQuery("begin " + sql + " end;");
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "保存成功";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "保存失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 返回异常成本类别数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getabnormal_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string sql = $@"SELECT
	qa_cost_cate_no,--品质成本类别编号
	qa_cost_cate_name,--品质成本类别名称
	unit_price,--单价
	qa_cost_cate_u,--单位
    '0'as qty --数量
FROM
	bdm_qa_cost_cate_m";
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
        /// 删除品质成本
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject deletepzcb(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string sql = string.Empty;
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_d where id='{id}'") > 0)
                {
                    sql = $@"delete from qcm_abnormal_r_d where id='{id}'";
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "删除成功";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "删除失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_PONUM(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string po_list = jarr.ContainsKey("poList") ? jarr["poList"].ToString() : "";
                List<string> list = JsonConvert.DeserializeObject<List<string>>(po_list);
                string listStr = "";
                foreach (var item in list)
                {
                    listStr += "'" + item + "',";
                }
                listStr = listStr.TrimEnd(',');
                string sql = $@"select sum(b.se_qty) as ponum
  from bdm_se_order_master a
  join bdm_se_order_item b
 using (se_id)
 where a.mer_po in({listStr})";
                DataTable dataTable = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dataTable;
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
