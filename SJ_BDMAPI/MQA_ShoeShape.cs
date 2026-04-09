using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    public class MQA_ShoeShape
    {
        /// <summary>
        /// MQA鞋型管理主页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMQAMain(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 art
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";//查询条件 季度
                string PRODUCT_MONTH = jarr.ContainsKey("PRODUCT_MONTH") ? jarr["PRODUCT_MONTH"].ToString() : "";//查询条件 量产月份

                string user_section = jarr.ContainsKey("user_section") ? jarr["user_section"].ToString() : "";//查询条件 开发课
                string rule_no = jarr.ContainsKey("rule_no") ? jarr["rule_no"].ToString() : "";//查询条件 Category
                string cwa_date = jarr.ContainsKey("cwa_date") ? jarr["cwa_date"].ToString() : "";//查询条件 CWA日期
                string qa_principal = jarr.ContainsKey("qa_principal") ? jarr["qa_principal"].ToString() : "";//查询条件 qa负责人
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(PROD_NO))
                {
                    where += $@" and A.PROD_NO like @PROD_NO ";
                }
                if (!string.IsNullOrWhiteSpace(SHOE_NO))
                {
                    where += $@" and d.NAME_T like @SHOE_NO ";
                }
                if (!string.IsNullOrWhiteSpace(PRODUCT_MONTH))
                {
                    where += $@" and A.PRODUCT_MONTH like @PRODUCT_MONTH ";
                }
                if (!string.IsNullOrWhiteSpace(DEVELOP_SEASON))
                {
                    where += $@" and A.DEVELOP_SEASON like @DEVELOP_SEASON ";
                }
                if (!string.IsNullOrWhiteSpace(user_section))
                {
                    where += $@" and A.user_section like @user_section ";
                }
                if (!string.IsNullOrWhiteSpace(rule_no))
                {
                    where += $@" and bb.name_t like @rule_no ";
                }
                if (!string.IsNullOrWhiteSpace(cwa_date))
                {
                    where += $@" and A.cwa_date like @cwa_date ";
                }
                if (!string.IsNullOrWhiteSpace(qa_principal))
                {
                    where += $@" and e.qa_principal like @qa_principal ";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("PROD_NO", $@"%{PROD_NO}%");
                paramTestDic.Add("SHOE_NO", $@"%{SHOE_NO}%");
                paramTestDic.Add("PRODUCT_MONTH", $@"%{PRODUCT_MONTH}%");
                paramTestDic.Add("DEVELOP_SEASON", $@"%{DEVELOP_SEASON}%");
                paramTestDic.Add("user_section", $@"%{user_section}%");
                paramTestDic.Add("rule_no", $@"%{rule_no}%");
                paramTestDic.Add("cwa_date", $@"%{cwa_date}%");
                paramTestDic.Add("qa_principal", $@"%{qa_principal}%");
                //string sql = $@"
                //            SELECT
                //             LISTAGG (TO_CHAR(A .PROD_NO), ',') WITHIN GROUP (ORDER BY A .SHOE_NO) AS PROD_NO,
                //             MAX (A .SHOE_NO) AS SHOE_NO,
                //             MAX (q.IMAGE_GUID) AS IMAGE_GUID,
                //             MAX (c.FILE_URL) AS FILE_URL,
                //             MAX (A .PRODUCT_MONTH) AS PRODUCT_MONTH,
                //             MAX (A .DEVELOP_SEASON) AS DEVELOP_SEASON,

                //             MAX(bb.rule_no) as rule_no,
                //             MAX(A.user_section) AS user_section,
                //             MAX(A.TEST_LEVEL) AS TEST_LEVEL,
                //             MAX(A.develop_type) AS develop_type,
                //             MAX(A.COL1) AS COL1,
                //             MAX(A.BOM_DATE) AS BOM_DATE,
                //             MAX(A.cwa_date) AS cwa_date,
                //             MAX(A.user_fdd) AS user_fdd,
                //             MAX(A.user_technical) AS user_technical
                //            FROM
                //             bdm_rd_prod A
                //            LEFT JOIN qcm_mqa_mag_d b ON A .SHOE_NO = b.SHOES_CODE 
                //            LEFT JOIN qcm_shoes_qa_record_m q ON b.SHOES_CODE = q.shoes_code 
                //            LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
                //            LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
                //            LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                //            WHERE
                //             1 = 1 {where}
                //            GROUP BY
                //             A .SHOE_NO";

                string sql = $@"
	                        SELECT 
			                        A .SHOE_NO AS SHOE_NO,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PROD_NO", "A.PROD_NO")} PROD_NO,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PRODUCT_MONTH", "A.PRODUCT_MONTH")} PRODUCT_MONTH,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.DEVELOP_SEASON", "A.DEVELOP_SEASON")} DEVELOP_SEASON,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_section", "A.user_section")}  user_section,
                                    MIN(A.TEST_LEVEL) as TEST_LEVEL,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.develop_type", "A.develop_type")} develop_type,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.COL1", "A.COL1")}  COL1,
			               MAX(to_char(A.BOM_DATE,'YYYY/MM/dd') ) BOM_DATE,    
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.BOM_DATE", "A.BOM_DATE")} BOM_DATE2,
			               MAX(to_char(A.cwa_date,'YYYY/MM/dd') ) cwa_date,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.cwa_date", "A.cwa_date")} cwa_date2,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_fdd", "A.user_fdd")} user_fdd,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_technical", "A.user_technical")} user_technical,
			                        q.IMAGE_GUID AS IMAGE_GUID,
			                        c.FILE_URL AS FILE_URL,  
			                        bb.name_t as rule_no,
                                    e.qa_principal as qa_principal,
									MAX(d.NAME_T) as name_t
	                        FROM
		                        bdm_rd_prod A 
                            LEFT JOIN BDM_RD_STYLE d on A.SHOE_NO=d.SHOE_NO
	                        LEFT JOIN qcm_shoes_qa_record_m q ON A .SHOE_NO = q.shoes_code 
	                        LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
	                        LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
	                        LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                            LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=A.SHOE_NO
	                        WHERE 1=1  {where}
	                        GROUP BY
		                    A .SHOE_NO,q.IMAGE_GUID,c.FILE_URL,bb.name_t, e.qa_principal
		                    order by A.SHOE_NO  
                        ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic);

                foreach (DataRow dr in dt.Rows)
                {
                    dr["PRODUCT_MONTH"] = DistinctName(dr["PRODUCT_MONTH"].ToString());
                    dr["DEVELOP_SEASON"] = DistinctName(dr["DEVELOP_SEASON"].ToString());
                    dr["user_section"] = DistinctName(dr["user_section"].ToString());
                    dr["TEST_LEVEL"] = DistinctName(dr["TEST_LEVEL"].ToString());
                    dr["develop_type"] = DistinctName(dr["develop_type"].ToString());
                    dr["COL1"] = DistinctName(dr["COL1"].ToString());
                    dr["BOM_DATE"] = DistinctName(dr["BOM_DATE"].ToString());
                    dr["cwa_date"] = DistinctName(dr["cwa_date"].ToString());
                    dr["user_fdd"] = DistinctName(dr["user_fdd"].ToString());
                    dr["user_technical"] = DistinctName(dr["user_technical"].ToString());
                }

                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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


        public static string DistinctName(string strNames)
        {
            string names = string.Empty;
            List<string> lstName = new List<string>();
            if (!string.IsNullOrEmpty(strNames))
            {
                string[] arrNames  =  strNames.Split(',');
                foreach (var name in arrNames)
                {
                    if (!lstName.Contains(name))
                    {
                        lstName.Add(name);
                    } 
                }
                names = string.Join(",", lstName);
            }
            return names;
        }

        /// <summary>
        /// MQA鞋型管理编辑页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_Edit(object OBJ)
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
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//查询条件 鞋型
                List<string> arts = jarr.ContainsKey("arts") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["arts"].ToString()) : null;//查询条件 art
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (arts.Count > 0)
                {
                    where = $@" and r.art_code in ({string.Join(",", arts.Select(x => $@"'{x}'"))})";
                }
                string sql = string.Empty;
                if (workshop_section_no == "qx")
                {
                    sql = $@"(SELECT
                                MAX(d.id) as did,
	                            'DQA' AS problemsources,
	                            MAX( m.workshop_section_no ) AS workshop_section_no,
	                            MAX( m.workshop_section_name ) AS workshop_section_name,
	                            '' AS image_guid,
	                            '' AS FILE_URL,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX( d.choice_no ) AS choice_no,
	                            MAX( d.choice_name ) AS choice_name,
	                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
	                            MAX( d.inspection_code ) AS inspection_code,
	                            MAX( d.inspection_type ) AS inspection_type, 
	                            '' AS inspection_name,
                                    MAX( d.JUDGMENT_CRITERIA) as JUDGMENT_CRITERIA,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX( d.standard_value ) AS standard_value,
	                            MAX( d.unit ) AS unit,
	                            MAX( d.other_measures ) AS other_measures,
	                            MAX( d.remark ) AS remark,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS DQAfilelist,
	                            ' — ' AS dep_attr,
		                            '' AS dep_attr_name,
	                            MAX( d.f_insp_dep ) AS f_insp_dep,
                                MAX( d.f_insp_date ) AS f_insp_date,
                                MAX( d.f_insp_res ) AS f_insp_res,
		                        MAX( d.processing_record ) AS processing_record,
		                        MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT mf.file_id", "mf.file_id")} AS MQAfilelist 
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m ON d.m_id = m.id and m.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_dqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_mf mf ON d.id = mf.d_id and mf.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
				                where d.isdelete='0' and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
                                    MAX(d.id) as did,
		                            'MQA' AS problemsources,
		                            MAX( d.workshop_section_no ) AS workshop_section_no,
		                            MAX( d.workshop_section_name ) AS workshop_section_name,
		                            '' AS image_guid,
		                            '' AS FILE_URL,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
		                            MAX( d.choice_no ) AS choice_no,
		                            MAX( d.choice_name ) AS choice_name,
		                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
		                            MAX( d.inspection_code ) AS inspection_code,
	                                MAX( d.inspection_type ) AS inspection_type, 
	                                '' AS inspection_name,
                                    MAX( d.JUDGMENT_CRITERIA) as JUDGMENT_CRITERIA,
		                            MAX(y.enum_value) as judge_mode,
		                            MAX( d.standard_value ) AS standard_value,
		                            MAX( d.unit ) AS unit,
		                            MAX( d.other_measures ) AS other_measures,
		                            MAX( d.remark ) AS remark,
		                            '' AS DQAfilelist,
		                            MAX( d.dep_attr ) AS dep_attr,
		                            '' AS dep_attr_name,
		                            MAX( d.f_insp_dep ) AS f_insp_dep,
		                            MAX( d.f_insp_date ) AS f_insp_date,
		                            MAX( d.f_insp_res ) AS f_insp_res,
		                            MAX( d.processing_record ) AS processing_record,
		                        MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS MQAfilelist 
	                            FROM
		                            qcm_mqa_mag_d d
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_mqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_mqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
				                    where d.isdelete='0' and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )
                            ";
                }
                else
                {
                    sql = $@"(SELECT
                                MAX(d.id) as did,
	                            'DQA' AS problemsources,
	                            MAX( m.workshop_section_no ) AS workshop_section_no,
	                            MAX( m.workshop_section_name ) AS workshop_section_name,
	                            '' AS image_guid,
	                            '' AS FILE_URL,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX( d.choice_no ) AS choice_no,
	                            MAX( d.choice_name ) AS choice_name,
	                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
	                            MAX( d.inspection_code ) AS inspection_code,
	                            MAX( d.inspection_type ) AS inspection_type, 
	                            '' AS inspection_name,
                                MAX( d.JUDGMENT_CRITERIA) as JUDGMENT_CRITERIA,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX( d.standard_value ) AS standard_value,
	                            MAX( d.unit ) AS unit,
	                            MAX( d.other_measures ) AS other_measures,
	                            MAX( d.remark ) AS remark,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS DQAfilelist,
	                            ' — ' AS dep_attr,
		                            '' AS dep_attr_name,
	                            MAX( d.f_insp_dep ) AS f_insp_dep,
		                            MAX( d.f_insp_date ) AS f_insp_date,
		                            MAX( d.f_insp_res ) AS f_insp_res,
		                            MAX( d.processing_record ) AS processing_record,
		                        MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT mf.file_id", "mf.file_id")} AS MQAfilelist 
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m ON d.m_id = m.id and m.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_dqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_mf mf ON d.id = mf.d_id and mf.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
				                    where d.isdelete='0' and m.workshop_section_no=@workshop_section_no and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
                                    MAX(d.id) as did,
		                            'MQA' AS problemsources,
		                            MAX( d.workshop_section_no ) AS workshop_section_no,
		                            MAX( d.workshop_section_name ) AS workshop_section_name,
		                            '' AS image_guid,
		                            '' AS FILE_URL,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
		                            MAX( d.choice_no ) AS choice_no,
		                            MAX( d.choice_name ) AS choice_name,
		                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
		                            MAX( d.inspection_code ) AS inspection_code,
	                                MAX( d.inspection_type ) AS inspection_type, 
	                                '' AS inspection_name,
                                    MAX( d.JUDGMENT_CRITERIA) as JUDGMENT_CRITERIA,
		                            MAX(y.enum_value) as judge_mode,
		                            MAX( d.standard_value ) AS standard_value,
		                            MAX( d.unit ) AS unit,
		                            MAX( d.other_measures ) AS other_measures,
		                            MAX( d.remark ) AS remark,
		                            '' AS DQAfilelist,
		                            MAX( d.dep_attr ) AS dep_attr,
		                            '' AS dep_attr_name,
		                            MAX( d.f_insp_dep ) AS f_insp_dep,
		                            MAX( d.f_insp_date ) AS f_insp_date,
		                            MAX( d.f_insp_res ) AS f_insp_res,
		                            MAX( d.processing_record ) AS processing_record,
		                        MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS MQAfilelist  
	                            FROM
		                            qcm_mqa_mag_d d
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_mqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_mqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
				                    where d.isdelete='0' and d.workshop_section_no=@workshop_section_no and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )
                            ";
                }

                DataTable dep_dt = DB.GetDataTable($@"
 select SUPPLIERS_CODE as dep_attr,SUPPLIERS_NAME as dep_attr_name from base003m 
                                                                        UNION
                                        select DEPARTMENT_CODE as dep_attr,DEPARTMENT_NAME as dep_attr_name from base005m
");
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("workshop_section_no", $@"{workshop_section_no}");
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = CommonBASE.GetPageDataTable(DB, $@"select * from ({sql}) ORDER BY problemsources DESC,did DESC", int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                List<string> d_idList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    d_idList.Add(item["did"].ToString());
                    Dictionary<string,object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_type"].ToString(), item["inspection_code"].ToString());
                    if (dicInspect.Count>0 && dicInspect.ContainsKey("INSPECTION_NAME"))
                        item["inspection_name"] = dicInspect["INSPECTION_NAME"].ToString();

                    string[] art_codes = item["art_code"].ToString().Split(',');
                    string art_code = string.Empty;
                    art_codes = art_codes.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < art_codes.Length; i++)
                    {
                        art_code += art_codes[i] + ",";
                    }
                    item["art_code"] = art_code.TrimEnd(',');
                    //------------
                    string[] DQAfilelists = item["DQAfilelist"].ToString().Split(',');
                    string DQAfilelist = string.Empty;
                    DQAfilelists = DQAfilelists.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < DQAfilelists.Length; i++)
                    {
                        DQAfilelist += DQAfilelists[i] + ",";
                    }
                    item["DQAfilelist"] = DQAfilelist.TrimEnd(',');
                    //------------
                    string[] MQAfilelists = item["MQAfilelist"].ToString().Split(',');
                    string MQAfilelist = string.Empty;
                    MQAfilelists = MQAfilelists.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < MQAfilelists.Length; i++)
                    {
                        MQAfilelist += MQAfilelists[i] + ",";
                    }
                    item["MQAfilelist"] = MQAfilelist.TrimEnd(',');

                    if (item["problemsources"].ToString() == "MQA")
                    {
                        string dep_attr = item["dep_attr"].ToString();
                        var findDt = dep_dt.Select($@"DEP_ATTR='{dep_attr}'");
                        if (findDt.Count() > 0)
                        {
                            item["dep_attr_name"] = findDt[0]["DEP_ATTR_NAME"].ToString();
                        }
                    }
                }
                d_idList = d_idList.Distinct().ToList();
                if (d_idList.Count() > 0)
                {
                    string getDidImgSql = $@"
                        SELECT 
	                        s.D_ID,
                            s.QA_TYPE,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, $@"s.IMAGE_GUID||':'||s.IS_MAIN||':'||m.FILE_URL", "s.D_ID")} as img_arr 
                        FROM QCM_QA_MAG_D_IMG_S s 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM m ON m.GUID=s.IMAGE_GUID
                        WHERE s.D_ID IN({string.Join(",", d_idList)}) 
                        GROUP BY s.D_ID,s.QA_TYPE
                        ";
                    var imgDt = DB.GetDataTable(getDidImgSql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string d_id = item["did"].ToString();
                        string problemsources = item["problemsources"].ToString().ToLower();
                        problemsources = (problemsources == "dqa") ? "0" : "1";
                        var findImg = imgDt.Select($@"D_ID={d_id} and QA_TYPE='{problemsources}'");
                        if (findImg.Length > 0)
                        {
                            item["image_guid"] = findImg[0]["img_arr"].ToString();
                        }
                    }
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// 跳转MQA编辑时查询页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_EditTab(object OBJ)
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
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                int count = DB.GetInt32($@"select count(1) from qcm_dqa_mag_m where isdelete='0' and shoes_code=@shoe_no", paramTestDic);
                if (count <= 0)
                {
                    sql = $@"select workshop_section_no,workshop_section_name,data_source,id from bdm_workshop_section_m";
                    DataTable wendt = DB.GetDataTable(sql);
                    sql = "";
                    if (wendt.Rows.Count > 0)
                    {
                        foreach (DataRow item in wendt.Rows)
                        {
                            sql += $@"insert into qcm_dqa_mag_m (shoes_code,workshop_section_no,workshop_section_name,data_source,CREATEBY,CREATEDATE,CREATETIME) 
                        values('{shoe_no}', '{item["workshop_section_no"]}', '{item["workshop_section_name"]}', '{item["data_source"]}', '{user}', '{date}', '{time}');";
                        }
                        DB.ExecuteNonQuery($@"BEGIN {sql} END;");
                        DB.Commit();
                    }
                }

                sql = $@"select id,shoes_code,workshop_section_no,workshop_section_name,data_source from qcm_dqa_mag_m where isdelete='0' and shoes_code=@shoe_no";
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

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
        /// MQA编辑页面查询DQA文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAFile(object OBJ)
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
                string DQAfilelistguid = jarr.ContainsKey("DQAfilelistguid") ? jarr["DQAfilelistguid"].ToString() : "";//查询条件 DQA文件guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string[] DQAfilelist = DQAfilelistguid.Split(',');

                string sql = $@"select file_url,file_name,'BDM_UPLOAD_FILE_ITEM' as tablename,'' as id from BDM_UPLOAD_FILE_ITEM where GUID in ({string.Join(",", DQAfilelist.Select(x => $@"'{x}'"))})";

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
        /// MQA编辑页面查询工段
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getworkshop_section(object OBJ)
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
                //string DQAfilelistguid = jarr.ContainsKey("DQAfilelistguid") ? jarr["DQAfilelistguid"].ToString() : "";//查询条件 DQA文件guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;

                var list = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                var listres = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(list.ToString());
                foreach (var item in listres)
                {
                    where += $@"'{item}'" + ",";
                }
                string sql = $@"select DISTINCT workshop_section_no,workshop_section_name from qcm_dqa_mag_m where isdelete='0' AND workshop_section_no IN({where.TrimEnd(',')})";

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
        /// MQA管理页面添加时查询检测项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getinspection(object OBJ)
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
                string mno = jarr.ContainsKey("mno") ? jarr["mno"].ToString() : "";//查询条件 工段ID
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段ID
                string KeyCode = jarr.ContainsKey("KeyCode") ? jarr["KeyCode"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                string wheregd = string.Empty;
                string bname = string.Empty;

                if (mno == "qx")
                {
                    if (!string.IsNullOrWhiteSpace(workshop_section_no))
                    {
                        wheregd += $@"AND m.workshop_section_no = @workshop_section_no";
                    }
                    bname = $@"SELECT distinct
                                s.enum_code,
	                            s.enum_value2 
                            FROM
	                            bdm_workshop_section_m m
	                            INNER JOIN bdm_workshop_section_d d ON m.id = d.m_id
	                            LEFT JOIN sys001m s ON d.inspection_type = s.enum_code 
                            WHERE
	                            s.enum_type = 'enum_inspection_type' 
                                {wheregd}
	                           ";
                }
                else
                {
                    bname = $@"SELECT distinct
                                s.enum_code,
	                            s.enum_value2 
                            FROM
	                            bdm_workshop_section_m m
	                            INNER JOIN bdm_workshop_section_d d ON m.id = d.m_id
	                            LEFT JOIN sys001m s ON d.inspection_type = s.enum_code 
                            WHERE
	                            s.enum_type = 'enum_inspection_type' 
	                            AND m.workshop_section_no = @workshop_section_no";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (mno != "qx")
                {
                    paramTestDic.Add("workshop_section_no", $@"{mno}");
                }
                else
                {
                    paramTestDic.Add("workshop_section_no", $@"{workshop_section_no}");
                }
                DataTable tablename = DB.GetDataTable(bname, paramTestDic);
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(KeyCode))
                {
                    where = $@" and (inspection_code like @KeyCode or inspection_name like @KeyCode or standard_value like @KeyCode)";
                }
                if (tablename.Rows.Count > 0)
                {
                    sql = $@"select * from(";
                    int i = 0;
                    string qc_type = "qc_type";
                    foreach (DataRow item in tablename.Rows)
                    {
                        if (item["enum_value2"].ToString() != "bdm_shoes_check_test_m" && item["enum_value2"].ToString() != "bdm_parts_testitem_m"
                        && item["enum_value2"].ToString() != "bdm_material_testitem_m" && item["enum_value2"].ToString() != "bdm_workmanship_testitem_m"
                        && item["enum_value2"].ToString() != "bdm_shoes_check_fit_m"&& item["enum_value2"].ToString() != "bdm_shoes_check_wear_m"
                        && item["enum_value2"].ToString() != "bdm_shoes_check_function_m"&& item["enum_value2"].ToString() != "bdm_shoes_check_safe_m")
                            qc_type = "qc_type";
                        else
                            qc_type = "'-' as qc_type";

                        sql += $@"select {qc_type},standard_value,inspection_code,inspection_name,'{item["enum_code"]}' as inspection_type,judgment_criteria,enum_value judgment_criteria_name
                                        from {item["enum_value2"]}
                                        left join sys001m on enum_type='enum_judgment_criteria' and judgment_criteria=enum_code ";
                        if (i < tablename.Rows.Count - 1)
                        {
                            sql += " UNION ";
                        }
                        i++;
                    }
                    sql += $@")t where 1=1 {where}";
                }
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("KeyCode", $@"%{KeyCode}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic2);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic2);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// MQA管理页面添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Editmqa_mag_d(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型
                string qa_principal = jarr.ContainsKey("qa_principal") ? jarr["qa_principal"].ToString() : "";//查询条件 qa负责人
                DataTable mqa_mag_d = jarr.ContainsKey("mqa_mag_d") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["mqa_mag_d"].ToString()) : new DataTable();//查询条件 鞋型编号

                if (string.IsNullOrEmpty(shoes_code))
                    throw new Exception("接口参数【shoes_code】不能为空，请检查！");

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from bdm_shoe_extend_m where shoe_no='{shoes_code}'");
                if (count <= 0)
                {
                    sql = $@"insert into bdm_shoe_extend_m (shoe_no,qa_principal,createby,createdate,createtime) values('{shoes_code}','{qa_principal}',
                            '{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"update bdm_shoe_extend_m set shoe_no='{shoes_code}',qa_principal='{qa_principal}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where shoe_no='{shoes_code}'";
                    DB.ExecuteNonQuery(sql);
                }

                if (mqa_mag_d.Rows.Count > 0)
                {
                    foreach (DataRow item in mqa_mag_d.Rows)
                    {
                        string curr_d_id = item["did"].ToString();
                        string problemsources = item["problemsources"].ToString();
                        if (problemsources.Equals("MQA"))
                        {
                            string data_source = DB.GetString($@"select data_source from qcm_dqa_mag_m where shoes_code='{shoes_code}' and  workshop_section_no='{item["workshop_section_no"]}'");
                            if (string.IsNullOrEmpty(item["did"].ToString()))
                            {

                                sql = $@"insert into qcm_mqa_mag_d (shoes_code,workshop_section_no,workshop_section_name,data_source,choice_no,
                                choice_name,qa_risk_desc,inspection_code,inspection_type,JUDGMENT_CRITERIA,standard_value,unit,other_measures,remark,
                                dep_attr,f_insp_dep,f_insp_date,f_insp_res,processing_record,CREATEBY,CREATEDATE,CREATETIME,QA_RISK_DETAILS_DESC,QA_RISK_CATEGORY_CODE) 
                                values('{shoes_code}','{item["workshop_section_no"]}','{item["workshop_section_name"]}','{data_source}',
                                '{item["choice_no"]}','{item["choice_name"]}','{item["qa_risk_desc"]}','{item["inspection_code"]}','{item["inspection_type"]}',
                                '{item["JUDGMENT_CRITERIA"]}','{item["standard_value"]}','{item["unit"]}','{item["other_measures"]}','{item["remark"]}',
                                '{item["dep_attr"]}','{item["f_insp_dep"]}','{item["f_insp_date"]}','{item["f_insp_res"]}','{item["processing_record"]}'
                                ,'{user}','{date}','{time}','{item["qa_risk_details_desc"]}','{item["qa_risk_category_code"]}')";
                                DB.ExecuteNonQuery(sql);

                                string did = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_mqa_mag_d");
                                curr_d_id = did;
                                string[] DQAfiles = item["MQAfilelist"].ToString().Split(',');
                                for (int i = 0; i < DQAfiles.Length; i++)
                                {
                                    sql = $@"insert into qcm_mqa_mag_d_f (d_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                     values('{did}','{DQAfiles[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }

                                string[] art_codes = item["art_code"].ToString().Split(',');
                                for (int i = 0; i < art_codes.Length; i++)
                                {
                                    sql = $@"insert into qcm_mqa_mag_d_art (shoes_code,d_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                     values('{shoes_code}','{did}','{art_codes[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }
                            else
                            {
                                sql = $@"update qcm_mqa_mag_d 
                                        set shoes_code=@shoes_code,workshop_section_no=@workshop_section_no,workshop_section_name=@workshop_section_name,data_source=@data_source,choice_no=@choice_no,choice_name=@choice_name,qa_risk_desc=@qa_risk_desc,inspection_code=@inspection_code,
                                        inspection_type=@inspection_type,JUDGMENT_CRITERIA=@JUDGMENT_CRITERIA,standard_value=@standard_value,unit=@unit,other_measures=@other_measures,remark=@remark,
                                        dep_attr=@dep_attr,f_insp_dep=@f_insp_dep,f_insp_date=@f_insp_date,f_insp_res=@f_insp_res,processing_record=@processing_record,
                                        modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime,QA_RISK_DETAILS_DESC=@qa_risk_details_desc1,QA_RISK_CATEGORY_CODE=@qa_risk_category_code1
                                        where id=@did
                                        ";
                                Dictionary<string, object> parmater = new Dictionary<string, object>();
                                parmater.Add("shoes_code", shoes_code);
                                parmater.Add("workshop_section_no", item["workshop_section_no"]);
                                parmater.Add("workshop_section_name", item["workshop_section_name"]);
                                parmater.Add("data_source", data_source);
                                parmater.Add("choice_no", item["choice_no"]);
                                parmater.Add("choice_name", item["choice_name"]);
                                parmater.Add("qa_risk_desc", item["qa_risk_desc"]);
                                parmater.Add("inspection_code", item["inspection_code"]);
                                parmater.Add("inspection_type", item["inspection_type"]);
                                parmater.Add("JUDGMENT_CRITERIA", item["JUDGMENT_CRITERIA"]);
                                parmater.Add("standard_value", item["standard_value"]);
                                parmater.Add("unit", item["unit"]);
                                parmater.Add("other_measures", item["other_measures"]);
                                parmater.Add("remark", item["remark"]);
                                parmater.Add("dep_attr", item["dep_attr"]);
                                parmater.Add("f_insp_dep", item["f_insp_dep"]);
                                parmater.Add("f_insp_date", item["f_insp_date"]);
                                parmater.Add("f_insp_res", item["f_insp_res"]);
                                parmater.Add("processing_record", item["processing_record"]);
                                parmater.Add("modifyby", user);
                                parmater.Add("modifydate", date);
                                parmater.Add("modifytime", time);
                                parmater.Add("qa_risk_details_desc1", item["qa_risk_details_desc"]);
                                parmater.Add("qa_risk_category_code1", item["qa_risk_category_code"]);
                                parmater.Add("did", item["did"].ToString());
                                DB.ExecuteNonQuery(sql, parmater);

                                DB.ExecuteNonQuery($"delete qcm_mqa_mag_d_f where d_id='{item["did"].ToString()}' and isdelete='0'");
                                DB.ExecuteNonQuery($"delete qcm_mqa_mag_d_art where d_id='{item["did"].ToString()}' and isdelete='0'");

                                string[] DQAfiles = item["MQAfilelist"].ToString().Split(',');
                                for (int i = 0; i < DQAfiles.Length; i++)
                                {
                                    sql = $@"
                                        insert into qcm_mqa_mag_d_f (d_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                        values('{item["did"].ToString()}','{DQAfiles[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }

                                string[] art_codes = item["art_code"].ToString().Split(',');
                                for (int i = 0; i < art_codes.Length; i++)
                                {
                                    sql = $@"insert into qcm_mqa_mag_d_art (shoes_code,d_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                     values('{shoes_code}','{item["did"].ToString()}','{art_codes[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }
                        }
                        else if (problemsources.Equals("DQA"))
                        {
                            //记录备注
                            DB.ExecuteNonQuery($"update qcm_dqa_mag_d set processing_record='{item["processing_record"]}' WHERE id='{item["did"].ToString()}'");

                            DB.ExecuteNonQuery($"delete qcm_dqa_mag_d_mf where d_id='{item["did"].ToString()}' and isdelete='0'");
                            string[] DQAfiles = item["MQAfilelist"].ToString().Split(',');
                            for (int i = 0; i < DQAfiles.Length; i++)
                            {
                                sql = $@"
                                        insert into qcm_dqa_mag_d_mf (d_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                        values('{item["did"].ToString()}','{DQAfiles[i]}','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                            }
                        }
                        //图片处理
                        DB.ExecuteNonQuery($@"delete from QCM_QA_MAG_D_IMG_S where d_id={curr_d_id}");
                        string image_guid = item["image_guid"].ToString();
                        if (!string.IsNullOrEmpty(image_guid))
                        {
                            List<string> img_list = image_guid.Split(',').ToList();
                            foreach (var img_item in img_list)
                            {
                                List<string> img_info = img_item.Split(":").ToList();
                                DB.ExecuteNonQuery($@"INSERT INTO QCM_QA_MAG_D_IMG_S(D_ID,IMAGE_GUID,IS_MAIN,CREATEBY,CREATEDATE,QA_TYPE) VALUES({curr_d_id},'{img_info[0]}','{img_info[1]}','{user}',SYSDATE,'{(problemsources.Equals("DQA") ? "0" : "1")}')");
                            }
                        }
                    }
                }
                else
                {
                    ret.ErrMsg = "No saved data, please check!";
                    ret.IsSuccess = false;
                    return ret;
                }

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
        /// MQA编辑页面查询材料工序数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getdqa_mag_mid(object OBJ)
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
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段名称
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";//查询条件 工段名称
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"select id from qcm_dqa_mag_m where workshop_section_no=@workshop_section_no and workshop_section_name=@workshop_section_name";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("workshop_section_no", $@"{workshop_section_no}");
                paramTestDic.Add("workshop_section_name", $@"{workshop_section_name}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

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
        /// MQA管理页面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Deletemqa_mag_d(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 鞋型
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"update qcm_mqa_mag_d set isdelete='1' where id='{did}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_mqa_mag_d_f set isdelete='1' where d_id='{did}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_mqa_mag_d_art set isdelete='1' where d_id='{did}'";
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
        /// MQA管理页面查询部门属性
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getmqa_mag_d_dep_attr(object OBJ)
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
                string da = jarr.ContainsKey("da") ? jarr["da"].ToString() : "";//查询条件 art
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(da))
                {
                    where = $@" and (t.dep_attr like @dep_attr or t.dep_attr_name like @dep_attr)";
                }

                string sql = $@"select * from (
                                select SUPPLIERS_CODE as dep_attr,SUPPLIERS_NAME as dep_attr_name from base003m
                                UNION
                                select DEPARTMENT_CODE as dep_attr,DEPARTMENT_NAME as dep_attr_name from base005m
                                )t where 1=1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("dep_attr", $@"%{da}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// 跳转MQA管理时查询表头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_EditTH(object OBJ)
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
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                                                                                               //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                                                                                               //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                
                //                string sql = $@"
                //SELECT
                //LISTAGG (TO_CHAR(A.PROD_NO), ',') WITHIN GROUP (ORDER BY A.SHOE_NO) AS PROD_NO,
                //A.SHOE_NO,
                //MAX (A.PRODUCT_MONTH) AS PRODUCT_MONTH,
                //MAX (A.DEVELOP_SEASON) AS DEVELOP_SEASON,
                //MAX (q.IMAGE_GUID) AS IMAGE_GUID,
                //MAX (c.FILE_URL) AS FILE_URL,
                //MAX(bb.rule_no) as rule_no,
                //MAX(A.user_section) AS user_section,
                //MAX(A.TEST_LEVEL) AS TEST_LEVEL,
                //MAX(A.develop_type) AS develop_type,
                //MAX(A.COL1) AS COL1,
                //MAX(A.BOM_DATE) AS BOM_DATE,
                //MAX(A.cwa_date) AS cwa_date,
                //MAX(A.user_fdd) AS user_fdd,
                //MAX(A.user_technical) AS user_technical,
                //MAX(A.USER_IN_SHOECHARGE) AS USER_IN_SHOECHARGE
                //FROM
                //	bdm_rd_prod A
                //LEFT JOIN qcm_mqa_mag_d b ON A.SHOE_NO = b.SHOES_CODE
                //LEFT JOIN qcm_shoes_qa_record_m q ON A.shoe_no = q.shoes_code
                //LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
                //LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
                //LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                //WHERE
                //	1=1 AND A.SHOE_NO = '{shoe_no}'
                //GROUP BY
                //	A.SHOE_NO";


                string sql = $@"
                                SELECT 
			                        A .SHOE_NO AS SHOE_NO,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PROD_NO", "A.PROD_NO")} PROD_NO,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PRODUCT_MONTH", "A.PRODUCT_MONTH")}  PRODUCT_MONTH,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.DEVELOP_SEASON", "A.DEVELOP_SEASON")} DEVELOP_SEASON,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_section", "A.user_section")} user_section,
			                        
                                    MIN(A.TEST_LEVEL) as TEST_LEVEL,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.develop_type", "A.develop_type")} develop_type,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.COL1", "A.COL1")} COL1,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.BOM_DATE", "A.BOM_DATE")} BOM_DATE,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.cwa_date", "A.cwa_date")} cwa_date,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_fdd", "A.user_fdd")} user_fdd,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.user_technical", "A.user_technical")} user_technical,
			                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.USER_IN_SHOECHARGE", "A.USER_IN_SHOECHARGE")} USER_IN_SHOECHARGE,
			                        q.IMAGE_GUID AS IMAGE_GUID,
			                        c.FILE_URL AS FILE_URL,  
			                        bb.name_t as rule_no,
                                    e.qa_principal ,
									MAX(d.NAME_T) as name_t
	                        FROM
		                        bdm_rd_prod A 
                            LEFT JOIN BDM_RD_STYLE d on A.SHOE_NO=d.SHOE_NO
	                        LEFT JOIN qcm_shoes_qa_record_m q ON A .SHOE_NO = q.shoes_code 
	                        LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
	                        LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
	                        LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                            LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=A.SHOE_NO
	                        WHERE 1=1 and A.SHOE_NO = @shoe_no
	                        GROUP BY
		                    A .SHOE_NO,q.IMAGE_GUID,c.FILE_URL,bb.name_t,e.qa_principal 
		                    order by A.SHOE_NO  
                        ";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                foreach (DataRow dr in dt.Rows)
                {
                    dr["PRODUCT_MONTH"] = DistinctName(dr["PRODUCT_MONTH"].ToString());
                    dr["DEVELOP_SEASON"] = DistinctName(dr["DEVELOP_SEASON"].ToString());
                    dr["user_section"] = DistinctName(dr["user_section"].ToString());
                    dr["TEST_LEVEL"] = DistinctName(dr["TEST_LEVEL"].ToString());
                    dr["develop_type"] = DistinctName(dr["develop_type"].ToString());
                    dr["COL1"] = DistinctName(dr["COL1"].ToString());
                    dr["BOM_DATE"] = DistinctName(dr["BOM_DATE"].ToString());
                    dr["cwa_date"] = DistinctName(dr["cwa_date"].ToString());
                    dr["user_fdd"] = DistinctName(dr["user_fdd"].ToString());
                    dr["user_technical"] = DistinctName(dr["user_technical"].ToString());
                }

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
        /// DQA查看页面查询数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_List(object OBJ)
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
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//查询条件 鞋型
                List<string> arts = jarr.ContainsKey("arts") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["arts"].ToString()) : null;//查询条件 art
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (arts.Count > 0)
                {
                    where = $@" and r.art_code in ({string.Join(",", arts.Select(x => $@"'{x}'"))})";
                }
                string sql = string.Empty;
                if (workshop_section_no == "qx")
                {
                    sql = $@"(SELECT
                            MAX(d.id) as did,
                            MAX(m.workshop_section_name) as workshop_section_name,
                            'DQA' as problemsources,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
                            MAX(d.inspection_code) as inspection_code,
                            '' as file_url,
                            MAX(s.enum_value) as inspection_type,
                                MAX(d.inspection_type) as inspection_typecode,
                            '' AS inspection_name,
                            '' as imageguids,
                            MAX(y.enum_value) as judge_mode,
                            MAX(d.standard_value) as standard_value,
                            MAX(d.unit) as unit,
                            MAX(d.remark) as remark,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            MAX(d.other_measures) as other_measures,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS filelistguid
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE  d.isdelete='0' and d.shoes_code=@shoe_no {where}
                                    GROUP BY d.id
	                            HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
	                            MAX(d.id) as did,
	                            MAX(d.workshop_section_name) as workshop_section_name,
	                            'MQA' as problemsources,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                                MAX( d.qa_risk_desc ) AS qa_risk_desc,
	                            MAX(d.inspection_code) as inspection_code,
	                            '' as file_url,
	                            MAX(s.enum_value) as inspection_type,
                                MAX(d.inspection_type) as inspection_typecode,
                                '' AS inspection_name,
                                '' as imageguids,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
	                            MAX(d.other_measures) as other_measures,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS filelistguid
	                            FROM
		                            qcm_mqa_mag_d d
		                            LEFT JOIN qcm_mqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_mqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE d.isdelete='0' and d.shoes_code=@shoe_no {where}
                                    GROUP BY d.id
	                            HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )";
                }
                else
                {
                    sql = $@"(SELECT
                            MAX(d.id) as did,
                            MAX(m.workshop_section_name) as workshop_section_name,
                            'DQA' as problemsources,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
                            MAX(d.inspection_code) as inspection_code,
                            '' as file_url,
                            MAX(s.enum_value) as inspection_type,
                                MAX(d.inspection_type) as inspection_typecode,
                            '' AS inspection_name,
                            '' as imageguids,
                            MAX(y.enum_value) as judge_mode,
                            MAX(d.standard_value) as standard_value,
                            MAX(d.unit) as unit,
                            MAX(d.remark) as remark,
                            MAX(d.other_measures) as other_measures,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS filelistguid
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE  d.isdelete='0' and m.workshop_section_no=@workshop_section_no and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
	                            MAX(d.id) as did,
	                            MAX(d.workshop_section_name) as workshop_section_name,
	                            'MQA' as problemsources,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                                MAX( d.qa_risk_desc ) AS qa_risk_desc,
	                            MAX(d.inspection_code) as inspection_code,
	                            '' as file_url,
	                            MAX(s.enum_value) as inspection_type,
                                MAX(d.inspection_type) as inspection_typecode,
                                '' AS inspection_name,
                                '' as imageguids,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS filelistguid
	                            FROM
		                            qcm_mqa_mag_d d
		                            LEFT JOIN qcm_mqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_mqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE d.isdelete='0' and d.workshop_section_no=@workshop_section_no and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("workshop_section_no", $@"{workshop_section_no}");
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = CommonBASE.GetPageDataTable(DB,$@"SELECT * FROM ({sql}) ORDER BY problemsources DESC,did DESC" , int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic); List<string> d_idList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {

                    d_idList.Add(item["did"].ToString());
                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_typecode"].ToString(), item["inspection_code"].ToString());
                    if (dicInspect.Count > 0 && dicInspect.ContainsKey("INSPECTION_NAME"))
                        item["inspection_name"] = dicInspect["INSPECTION_NAME"].ToString();

                    string[] art_codes = item["art_code"].ToString().Split(',');
                    string art_code = string.Empty;
                    art_codes = art_codes.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < art_codes.Length; i++)
                    {
                        art_code += art_codes[i] + ",";
                    }
                    item["art_code"] = art_code.TrimEnd(',');

                    string[] filelistguids = item["filelistguid"].ToString().Split(',');
                    string filelistguid = string.Empty;
                    filelistguids = filelistguids.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < filelistguids.Length; i++)
                    {
                        filelistguid += filelistguids[i] + ",";
                    }
                    item["filelistguid"] = filelistguid.TrimEnd(',');
                }
                d_idList = d_idList.Distinct().ToList();
                if (d_idList.Count() > 0)
                {
                    string getDidImgSql = $@"
                        SELECT 
	                        s.D_ID,
                            s.QA_TYPE,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, $@"s.IMAGE_GUID||':'||s.IS_MAIN||':'||m.FILE_URL", "s.D_ID")} as img_arr 
                        FROM QCM_QA_MAG_D_IMG_S s 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM m ON m.GUID=s.IMAGE_GUID
                        WHERE s.D_ID IN({string.Join(",", d_idList)}) 
                        GROUP BY s.D_ID,s.QA_TYPE
                        ";
                    var imgDt = DB.GetDataTable(getDidImgSql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string d_id = item["did"].ToString();
                        string problemsources = item["problemsources"].ToString().ToLower();
                        problemsources = (problemsources == "dqa") ? "0" : "1";
                        var findImg = imgDt.Select($@"D_ID={d_id} and QA_TYPE='{problemsources}'");
                        if (findImg.Length > 0)
                        {
                            item["file_url"] = findImg[0]["img_arr"].ToString();
                        }
                    }
                }
                if (d_idList.Count() > 0)
                {
                    string getDidImgSql = $@"
                        SELECT 
	                        s.D_ID,
                            s.QA_TYPE,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, $@"s.IMAGE_GUID||':'||s.IS_MAIN||':'||m.FILE_URL", "s.D_ID")} as img_arr 
                        FROM QCM_QA_MAG_D_IMG_S s 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM m ON m.GUID=s.IMAGE_GUID
                        WHERE s.D_ID IN({string.Join(",", d_idList)}) 
                        GROUP BY s.D_ID,s.QA_TYPE
                        ";
                    var imgDt = DB.GetDataTable(getDidImgSql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string d_id = item["did"].ToString();
                        //string problemsources = item["problemsources"].ToString().ToLower();
                        //problemsources = (problemsources == "dqa") ? "0" : "1";
                        var findImg = imgDt.Select($@"D_ID={d_id}");
                        if (findImg.Length > 0)
                        {
                            item["imageguids"] = findImg[0]["img_arr"].ToString();
                        }
                    }
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// DQA查看页面查询数据导出
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_List_Export(object OBJ)
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
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//查询条件 鞋型
                List<string> arts = jarr.ContainsKey("arts") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["arts"].ToString()) : null;//查询条件 art
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段

                string where = string.Empty;
                if (arts.Count > 0)
                {
                    where = $@" and r.art_code in ({string.Join(",", arts.Select(x => $@"'{x}'"))})";
                }
                string sql = string.Empty;
                if (workshop_section_no == "qx")
                {
                    sql = $@"(SELECT
                            MAX(m.workshop_section_name) as workshop_section_name,
                            'DQA' as problemsources,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc, 
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX(d.inspection_code) as inspection_code,
                            MAX(d.inspection_type) as inspection_typecode,
                            '' AS inspection_name,
                            MAX(y.enum_value) as judge_mode,
                            MAX(d.standard_value) as standard_value,
                            MAX(d.unit) as unit,
                            MAX(d.remark) as remark,
                            MAX(d.other_measures) as other_measures
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE  d.isdelete='0' and d.shoes_code=@shoe_no {where}
                                    GROUP BY d.id
	                            HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
	                            MAX(d.workshop_section_name) as workshop_section_name,
	                            'MQA' as problemsources,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
	                            MAX(d.inspection_code) as inspection_code,
                                 MAX(d.inspection_type) as inspection_typecode,
                                '' AS inspection_name,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures
	                            FROM
		                            qcm_mqa_mag_d d
		                            LEFT JOIN qcm_mqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_mqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria'
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE d.isdelete='0' and d.shoes_code=@shoe_no {where}
                                    GROUP BY d.id
	                            HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )";
                }
                else
                {
                    sql = $@"(SELECT
                            MAX(m.workshop_section_name) as workshop_section_name,
                            'DQA' as problemsources,
                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_type) as inspection_typecode,
                            '' AS inspection_name,
                            MAX(y.enum_value) as judge_mode,
                            MAX(d.standard_value) as standard_value,
                            MAX(d.unit) as unit,
                            MAX(d.remark) as remark,
                            MAX(d.other_measures) as other_measures
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE  d.isdelete='0' and d.shoes_code=@shoe_no and m.workshop_section_no=@workshop_section_no {where}
	                            GROUP BY d.id
                                HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
	                            MAX(d.workshop_section_name) as workshop_section_name,
	                            'MQA' as problemsources,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                            MAX( d.qa_risk_desc ) AS qa_risk_desc,
                            MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
	                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_type) as inspection_typecode,
                                '' AS inspection_name,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures
	                            FROM
		                            qcm_mqa_mag_d d
		                            LEFT JOIN qcm_mqa_mag_d_art r on d.id=r.d_id and r.isdelete='0'
		                            LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
		                            LEFT JOIN sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
		                            LEFT JOIN qcm_mqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
		                            WHERE d.isdelete='0' and d.shoes_code=@shoe_no and d.workshop_section_no=@workshop_section_no {where}
	                            GROUP BY d.id
                                HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                paramTestDic.Add("workshop_section_no", $@"{workshop_section_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt.Rows)
                {

                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_typecode"].ToString(), item["inspection_code"].ToString());
                    if (dicInspect.Count > 0 && dicInspect.ContainsKey("INSPECTION_NAME"))
                        item["inspection_name"] = dicInspect["INSPECTION_NAME"].ToString();

                    string[] art_codes = item["art_code"].ToString().Split(',');
                    string art_code = string.Empty;
                    art_codes = art_codes.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < art_codes.Length; i++)
                    {
                        art_code += art_codes[i] + ",";
                    }
                    item["art_code"] = art_code.TrimEnd(',');
                }

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
        /// 跳转MQA查看页面时查询表头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_ListTH(object OBJ)
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
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"
SELECT
	{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PROD_NO", "A.PROD_NO")} AS PROD_NO,
	A.SHOE_NO,
	MAX (A.PRODUCT_MONTH) AS PRODUCT_MONTH,
	MAX (A.DEVELOP_SEASON) AS DEVELOP_SEASON,
	MAX (q.image_guid) AS image_guid,
	MAX (c.FILE_URL) AS FILE_URL,
    MAX(bb.name_t) as rule_no,
	MAX(A.user_section) AS user_section,
	MIN(A.TEST_LEVEL) AS TEST_LEVEL,
	MAX(A.develop_type) AS develop_type,
	MAX(A.COL1) AS COL1,
	MAX(A.BOM_DATE) AS BOM_DATE,
	MAX(A.cwa_date) AS cwa_date,
	MAX(A.user_fdd) AS user_fdd,
	MAX(A.user_technical) AS user_technical,
    MAX(A.USER_IN_SHOECHARGE) AS USER_IN_SHOECHARGE,
MAX(e.qa_principal) as qa_principal,
									MAX(d.NAME_T) as name_t
FROM
	bdm_rd_prod A
LEFT JOIN BDM_RD_STYLE d on A.SHOE_NO=d.SHOE_NO
LEFT JOIN qcm_shoes_qa_record_m q ON A.shoe_no = q.shoes_code
LEFT JOIN qcm_mqa_mag_d b ON A .SHOE_NO = b.SHOES_CODE
LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
LEFT JOIN bdm_rd_style aa ON A .shoe_no = aa.shoe_no
LEFT JOIN bdm_cd_code bb ON aa.style_seq = bb.code_no
LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=A.SHOE_NO
WHERE
	1=1 AND A.SHOE_NO = @shoe_no
GROUP BY
	A.SHOE_NO";
                /*
                 LEFT JOIN qcm_mqa_mag_d b ON A.SHOE_NO = b.SHOES_CODE
LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = b.IMAGE_GUID
LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                 */
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                foreach (DataRow dr in dt.Rows)
                {
                    dr["PROD_NO"] = DistinctName(dr["PROD_NO"].ToString());
                }

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
        /// 跳转MQA查看页面时查询页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_ListTab(object OBJ)
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
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"select id,shoes_code,workshop_section_no,workshop_section_name,data_source from qcm_dqa_mag_m where isdelete='0' and shoes_code=@shoe_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

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
        /// MQA查看页面查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_ListFile(object OBJ)
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
                string filelistguid = jarr.ContainsKey("filelistguid") ? jarr["filelistguid"].ToString() : "";//查询条件 文件guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string[] filelist = filelistguid.Split(',');

                string sql = $@"select file_url,file_name,'BDM_UPLOAD_FILE_ITEM' as tablename,'' as id from BDM_UPLOAD_FILE_ITEM where GUID in ({string.Join(",", filelist.Select(x => $@"'{x}'"))})";

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
        /// MQA查看页面查询所有文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_ListFileALL(object OBJ)
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
                //string filelistguid = jarr.ContainsKey("filelistguid") ? jarr["filelistguid"].ToString() : "";//查询条件 文件guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型

                string sql = $@"select 
                                file_url,
                                file_name,
                                'qcm_mqa_mag_d_f' as tablename,
                                f.id as id
                                from 
                                qcm_mqa_mag_d d
                                LEFT JOIN qcm_mqa_mag_d_f f on d.id=f.d_id
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM t on f.file_id=t.guid where f.isdelete='0' and d.shoes_code='{shoes_code}'";
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
    }
}
