
using SJ_BDMAPI;
using SJ_KanBanAPI.Dto;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace SJ_KanBanAPI
{
    public class WholeLife
    {
        public static string moudle_code = "metalManagement";//金属检测
        public static string moudle_code2 = "tqcExtractionTest";//TQC
        public static string moudle_code3 = "chinaReturn";//中国市场退货分析
        public static string moudle_code4 = "a01Information";//AQL-A01
        public static string moudle_code5 = "laboratoryTests";//前段Q-实验室测试
        /// <summary>
        /// 全生命周期-头部基本信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataHead(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//Search Conditions Shoe Type
                string sql = $@"
                                SELECT 
bb.name_t as rule_no, -- calegory
MIN(A.TEST_LEVEL) as TEST_LEVEL, -- 测试级别
A .SHOE_NO AS SHOE_NO, -- 鞋型
 {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.develop_type", "A.develop_type")} develop_type, -- PBTYPE
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PRODUCT_MONTH", "A.PRODUCT_MONTH")}  PRODUCT_MONTH, -- 量产月份
 {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.DEVELOP_SEASON", "A.DEVELOP_SEASON")} DEVELOP_SEASON, -- 季度
 {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PRODUCT_LEVEL", "A.PRODUCT_LEVEL")} PRODUCT_LEVEL,-- 产品级别
 {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.COL1", "A.COL1")} COL1,-- 预计订单
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT A.PROD_NO", "A.PROD_NO")} PROD_NO,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT to_char(to_date(A.cwa_date),'yyyy-mm-dd') ", "A.cwa_date")} cwa_date,		                        
			                        q.IMAGE_GUID AS IMAGE_GUID,
			                        c.FILE_URL AS FILE_URL,  
									MAX(d.NAME_T) as name_t
	                        FROM
		                        bdm_rd_prod A 
                            LEFT JOIN BDM_RD_STYLE d on A.SHOE_NO=d.SHOE_NO
	                        LEFT JOIN qcm_shoes_qa_record_m q ON A .SHOE_NO = q.shoes_code 
	                       LEFT JOIN BDM_UPLOAD_FILE_ITEM c ON c.GUID = q.IMAGE_GUID
	                     LEFT JOIN bdm_rd_style aa ON A.shoe_no=aa.shoe_no
	                       LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                           LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=A.SHOE_NO
	                        WHERE 1=1 and A.PROD_NO = @PROD_NO
	                        GROUP BY
		                    A .SHOE_NO,q.IMAGE_GUID,c.FILE_URL,bb.name_t,e.qa_principal 
		                    order by A.SHOE_NO  
                        ";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("PROD_NO", $@"{PROD_NO}");
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
        /// 全生命周期 - QA部分
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQAData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//Search Conditions Shoe Type

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                string shoes_no = string.Empty;
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where = $@" and r.art_code like '%{PROD_NO}%'";
                    shoes_no = DB.GetString($@"select SHOE_NO from BDM_RD_PROD where PROD_NO = '{PROD_NO}' ");
                }
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    where += $@"AND ( d.createdate BETWEEN '{start_date}' AND '{end_date}') ";

                string sql = $@"
with aa as(
(SELECT
MAX(d.createdate) AS createdate,
                                MAX(d.id) as did,
	                            'DQA' AS problemsources,
	                            MAX( m.workshop_section_no ) AS workshop_section_no,
	                            MAX( m.workshop_section_name ) AS workshop_section_name,
	                           --  '' AS image_guid,
	                           --  '' AS FILE_URL,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
	                            MAX( d.choice_no ) AS choice_no,
	                            MAX( d.choice_name ) AS choice_name,
	                             MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK,
                               -- '' AS QA_RISK,
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
	                            ' — ' AS f_insp_dep,
	                            ' — ' AS f_insp_date,
	                            MAX(d.f_insp_res) AS f_insp_res,
		                        MAX( d.processing_record ) AS processing_record,
		                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT mf.file_id", "mf.file_id")} AS MQAfilelist 
	                            FROM
		                            qcm_dqa_mag_d d
		                            LEFT JOIN qcm_dqa_mag_m m ON d.m_id = m.id and m.isdelete='0'
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_dqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
		                            LEFT JOIN qcm_dqa_mag_d_mf mf ON d.id = mf.d_id and mf.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria'
                                    LEFT JOIN bdm_qa_risk_category_m x on x.qa_risk_category_code = d.qa_risk_category_code
				                where d.isdelete='0' and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( m.workshop_section_no ) IS NOT NULL 
	                            ) UNION
	                            (
	                            SELECT
MAX(d.createdate) AS createdate,
                                    MAX(d.id) as did,
		                            'MQA' AS problemsources,
		                            MAX( d.workshop_section_no ) AS workshop_section_no,
		                            MAX( d.workshop_section_name ) AS workshop_section_name,
		                           --  '' AS image_guid,
		                           --  '' AS FILE_URL,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT r.art_code", "r.art_code")} AS art_code,
		                            MAX( d.choice_no ) AS choice_no,
		                            MAX( d.choice_name ) AS choice_name,
		                            MAX( d.QA_RISK_DETAILS_DESC ) AS QA_RISK,
                                   --  '' AS QA_RISK,
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
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS MQAfilelist 
	                            FROM
		                            qcm_mqa_mag_d d
		                            -- LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON d.image_guid = u.guid
		                            LEFT JOIN qcm_mqa_mag_d_art r ON d.id = r.d_id and r.isdelete='0'
		                            LEFT JOIN qcm_mqa_mag_d_f f ON d.id = f.d_id and f.isdelete='0'
                                    LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                    LEFT JOIN bdm_qa_risk_category_m x on x.qa_risk_category_code = d.qa_risk_category_code
				                    where d.isdelete='0' and d.shoes_code=@shoe_no {where}
	                            GROUP BY d.id
                                HAVING MAX( d.workshop_section_no ) IS NOT NULL 
	                            )
)
select * from aa order BY WORKSHOP_SECTION_NO,PROBLEMSOURCES
";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("PROD_NO", $@"{PROD_NO}");
                paramTestDic.Add("shoe_no", $@"{shoes_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                List<string> d_idList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    d_idList.Add(item["did"].ToString());
                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_type"].ToString(), item["inspection_code"].ToString());
                    if (dicInspect.Count > 0 && dicInspect.ContainsKey("INSPECTION_NAME"))
                        item["inspection_name"] = dicInspect["INSPECTION_NAME"].ToString();
                }
                d_idList = d_idList.Distinct().ToList();

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
        /// 全生命周期 - 试穿
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//查询条件 鞋型

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                string where2 = string.Empty;
                string shoes_no = string.Empty;

                //试穿取值
                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }


                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( REPORTDATE BETWEEN '{start_date}' AND '{end_date}') ";
                    where2 += $@"AND ( a.ENDDATE BETWEEN '{start_date}' AND '{end_date}') ";

                }




                string sql = $@"
SELECT
    a.FITCODE as TESTID,
	a.ARTNO,
	a.TESTPHASE,
	a.REPORTDATE,
    a.SIZE,
a.FITRESULT
FROM
	fitinfo a
where 1=1 
and ARTNO = '{PROD_NO}'
{where}
order by REPORTDATE
";
                string sql2 = $@"
SELECT
	a.WEARCODE as TESTID,
	a.TESTPHASE ,
	a.ENDDATE,
    c.usercode,
	c.username,
	sum(case when b.hours1 is null then 0 else b.hours1 end) AS HOURS1,
	sum(case when b.hours2 is null then 0 else b.hours2 end) AS HOURS2,
	a.Size,
	'' as 测试结果,
	'' as 测试报告链接
FROM
	wearinfo a
LEFT JOIN wearuser b ON a.WearCode = b.WearCode
left join userinfo c on b.usercode = c.usercode
where 1=1
and ARTNO = '{PROD_NO}'
{where2}
GROUP BY
	a.TESTPHASE,
	a.ENDDATE,
	c.username,
	a.Size
order by a.ENDDATE desc";

                DataTable dt = wbscDB.GetDataTable(sql);
                DataTable dt2 = wbscDB.GetDataTable(sql2);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("FitData1", dt);
                dic.Add("WearData1", dt2);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                //DB.Close();
            }
            return ret;
        }

        /// <summary>Production Rally
        /// 全生命周期- 实验室模块
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            //SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//查询条件 鞋型

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("PROD_NO", $"%{PROD_NO}%");

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    where += $@"AND ( a.createdate BETWEEN '{start_date}' AND '{end_date}') ";

                string sql = $@"
                            SELECT
                            a.ART_NO,
	                            a.PHASE_CREATION_NAME,
                            a.TEST_TYPE,
                            a.STAFF_DEPARTMENT,
                            a.CREATEDATE,
                            a.CREATETIME,
                            a.TEST_ID,
                            a.TEST_RESULT,
                            a.TASK_NO
                            FROM
	                            QCM_EX_TASK_LIST_M a
                            left join
                            (
	                            select RETEST_TASK_NO,TASK_NO,TEST_RESULT from QCM_EX_TASK_LIST_M a join
	                            (
		                            select max(id) as id from QCM_EX_TASK_LIST_M b where RETEST_TASK_NO is not null group by RETEST_TASK_NO
	                            ) b on a.id=b.id
                            ) b  on a.TASK_NO=b.RETEST_TASK_NO
                            where 1=1 and a.ART_NO like @PROD_NO {where}
ORDER BY  CREATEDATE || CREATETIME desc
";
                DataTable dt = DB.GetDataTable(sql, param);


                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                //DB.Close();
            }
            return ret;
        }
        
        /// <summary>
        /// 生命周期看板-IQC
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetIQCData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            //SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//查询条件 ART
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 PO

                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                Dictionary<string, object> param = new Dictionary<string, object>();
                //param.Add("PROD_NO", $"%{PROD_NO}%");  c222.MER_PO

                if (!string.IsNullOrEmpty(PO))
                {

                    var arr = PO.Split(',');

                    string WherePo = "AND (  ";
                    for (int i = 0; i < arr.Length; i++)
                    {

                        if (i == arr.Length - 1)
                        {
                            WherePo += $@" c222.MER_PO like '%{arr[i]}%' ";
                        }
                        else
                        {
                            WherePo += $@"c222.MER_PO like '%{arr[i]}%' or ";
                        }
                    }

                    where += WherePo + ")";
                }

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    where += $@"AND ( s.createdate BETWEEN '{start_date}' AND '{end_date}') ";


                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@" AND z1.ART_NO like '%{PROD_NO}%'";
                }


                string sql = $@"
SELECT
  MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
  MAX(aw.NAME_T) as NAME_T, --材料名称
 MAX(aw.vend_no_prd) AS SUPPLIERS_CODE, --生产厂商编号
	MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME, --生产厂商
	MAX(b1.SUPPLIERS_CODE) as SUPPLIERS_CODE2, --采购厂商编号
	MAX(b1.SUPPLIERS_NAME) as SUPPLIERS_NAME2, --采购厂商
  '' as QTY,-- 进仓数量
  MAX(s.INSPECTIONDATE) as INSPECTIONDATE,--外观检验日期
  MAX(NVL(s.DETERMINE,2)) as DETERMINE,--外观结果
  MAX(a.CHK_NO) as CHK_NO,--收料单号 （外观结果清单）
  '' as RESULT, -- 实验室结果
	'' as task_no, -- 任务编号
MAX(s.createdate),--进仓日期
MAX(c222.MER_PO) as MER_PO, -- PO
		MAX(a.RCPT_DATE) RCPT_DATE, --进仓日期

  SUM(c111.ORD_QTY) as ORD_QTY, --采购数量
  MAX(a1.source_no) as ORDER_NO, --采购单号
   {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z1.ART_NO", "z1.ART_NO")} as PROD_NO, --ART
   {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z2.NAME_T", "z2.NAME_T")} as NAME_S2,--鞋型名称
	MAX(a1.CHK_SEQ) as CHK_SEQ,
MAX(c111.PART_NO) as PART_NO,--部位
	MAX(z2.SHOE_NO) as SHOE_NO, --鞋型
	MAX(a1.RCPT_QTY) AS RCPT_QTY, --收料数量
	MAX(nvl(s.sample_qty,0)) as sample_qty,
	MAX(s.DETERMINE) as cors,
	MAX(pe.ITEM_TYPE_NO) as ITEM_TYPE_NO, --物料类型
	MAX(a.INSERT_DATE) as INSERT_DATE,
	MAX(c111.ORDER_LEVEL) as ORDER_LEVEL
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO

INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT  JOIN bdm_purchase_order_d z1 on a2.ORDER_NO=z1.ORDER_NO and a1.SOURCE_SEQ=z1.ORDER_SEQ --新加的
LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
LEFT JOIN base003m b1 ON a2.VEND_NO=b1.SUPPLIERS_CODE

LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT JOIN base003m b ON aw.vend_no_prd=b.SUPPLIERS_CODE
LEFT JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT JOIN base001m m1 on a.ORG_ID=m1.ORG_CODE
LEFT JOIN qcm_iqc_insp_res_m s on a.CHK_NO=s.CHK_NO and a1.CHK_SEQ=s.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on a.CHK_NO=rt.CHK_NO and a1.CHK_SEQ=rt.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON a1.SOURCE_NO=c111.ORDER_NO AND a1.CHK_SEQ=c111.ORDER_SEQ
LEFT JOIN BDM_SE_ORDER_MASTER c222 on c111.SALES_ORDER = c222.SE_ID
-- LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
-- LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
WHERE  a.RCPT_BY='01' {where}
GROUP BY a1.CHK_NO,a1.CHK_SEQ 
 order by MAX(a.INSERT_DATE) desc
";

                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    DataTable dtt = DB.GetDataTable($@"SELECT task_no,TEST_RESULT from qcm_ex_task_list_m where id in(SELECT MAX (ID) idd FROM qcm_ex_task_list_m a
WHERE sldh LIKE '%{item["CHK_NO"]}%' AND makings_id = '{item["ITEM_NO"]}' and ROWNUM = 1 )");

                    if (dtt.Rows.Count > 0)
                    {
                        item["RESULT"] = dtt.Rows[0]["TEST_RESULT"].ToString();
                        item["task_no"] = dtt.Rows[0]["task_no"].ToString();
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                //DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 生命周期看板-量试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLSData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            //SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PROD_NO = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                string where2 = string.Empty;
                Dictionary<string, object> param = new Dictionary<string, object>();

                //}
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@" AND A.prod_no like '%{PROD_NO}%'";
                }


                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                    where2 += $@"AND ( createdate BETWEEN '{start_date}' AND '{end_date}') ";



                string sql = $@"
 with tmp1 as(
-- 汇总表
(
select 
a.shoes_code,
b.workshop_section_no,
b.workshop_section_name,
b.choice_no,
b.choice_name,
c.m_id,
c.createdate,
c.createtime,
c.check_res
from 
qcm_ls_list_m a
left join qcm_ls_list_d b on a.id = b.m_id 
left join qcm_ls_list_d_d c on b.id = c.m_id
where c.m_id is not null and b.isdelete = '0' and c.isdelete = '0'
)
UNION
(
select 
a.shoes_code,
a.workshop_section_no,
a.workshop_section_name,
b.choice_no,
b.choice_name,
c.m_id,
c.createdate,
c.createtime,
c.check_res
 from 
qcm_dqa_mag_m a 
left join qcm_dqa_mag_d b on a.id = b.m_id 
left join qcm_dqa_mag_d_d c on b.id = c.m_id 
where c.m_id is not null and c.isdelete = '0'
)
),
tmp2 as(
--查出每个鞋型各个工段的最早创建时间
-- 量试数据源拼接DQA数据源
select shoes_code,workshop_section_no,min(createdate) as createdate from (
(select 
a.shoes_code,
b.workshop_section_no,
c.createdate,
row_number() over(partition by a.shoes_code,b.workshop_section_no order by c.createdate asc) as rn 
from 
qcm_ls_list_m a
left join qcm_ls_list_d b on a.id = b.m_id 
left join qcm_ls_list_d_d c on b.id = c.m_id
where c.m_id is not null and b.isdelete = '0' and c.isdelete = '0'
)
UNION
(
select  
a.shoes_code,
a.workshop_section_no,
c.createdate,
row_number() over(partition by a.shoes_code,a.workshop_section_no order by c.createdate asc) as rn 
from 
qcm_dqa_mag_m a 
left join qcm_dqa_mag_d b on a.id = b.m_id 
left join qcm_dqa_mag_d_d c on b.id = c.m_id 
where c.m_id is not null and c.isdelete = '0'
)) where rn = '1' GROUP BY shoes_code,workshop_section_no
),
tmp3 as (
-- 统计每个鞋型每个工段的最终提交问题点合格、不合格、总数
-- 量试数据源拼接DQA数据源
select shoes_code,
workshop_section_no,
workshop_section_name,
sum((case when check_res = '0' then 1 else 0 end)) + sum((case when check_res = '1' then 1 else 0 end)) as all_qty,
sum((case when check_res = '0' then 1 else 0 end)) as pass_qty,
sum((case when check_res = '1' then 1 else 0 end)) as fail_qty
 from (
select 
tmp1.*,
row_number() over(partition by tmp1.shoes_code,tmp1.workshop_section_no,m_id order by createdate||createtime desc) as rn 
from tmp1) where rn = '1' 
{where2}
GROUP BY shoes_code,workshop_section_no,workshop_section_name
)

-- 所有工段左连TMP2,TMP3
select 
c.createdate as 日期,
a.workshop_section_name as 工段,
nvl(b.all_qty,'0') as 问题数量,
nvl(b.pass_qty,'0') as 通过数量,
nvl(b.fail_qty,'0') as 不通过数量,
nvl(b.PROD_NO,'') as  prod_no,
nvl(b.SHOE_NO,'') as shoes_code,
nvl(b.WORKSHOP_SECTION_NO,'')as workshop_section_no
from bdm_workshop_section_m a
left join (
-- 过滤TMP3
select 
a.prod_no,
a.shoe_no,
b.workshop_section_no,
b.workshop_section_name,
b.all_qty,b.pass_qty,b.fail_qty
from 
bdm_rd_prod a
 join tmp3 b on a.shoe_no = b.shoes_code 
where 1=1 {where}
)b on a.workshop_section_no = b.workshop_section_no
 join 
(
-- 过滤TMP2
select b.createdate,b.workshop_section_no from bdm_rd_prod a left join tmp2 b on a.shoe_no = shoes_code
where 1=1 {where}
)c on c.workshop_section_no = a.workshop_section_no
order by c.createdate desc nulls last

";

                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                //DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 生命周期看板-RQC数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 鞋型
                string PO_ORDER = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 鞋型
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 截止时间

                string where = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    throw new Exception("缺少必要参数【PROD_NO】");
                }
                if (!string.IsNullOrEmpty(start_date))
                {
                    if (!string.IsNullOrEmpty(end_date))
                    {
                        where += $@" AND m.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                    }
                    else
                    {
                        where += $@" AND m.CREATEDATE >='{start_date}'";
                    }

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@" and m.prod_no = '{PROD_NO}' ";
                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {

                    var arr = PO_ORDER.Split(',');

                    string WherePo = "AND (  ";
                    for (int i = 0; i < arr.Length; i++)
                    {

                        if (i == arr.Length - 1)
                        {
                            WherePo += $@" m.mer_po like '%{arr[i]}%' ";
                        }
                        else
                        {
                            WherePo += $@" m.mer_po like '%{arr[i]}%' or ";
                        }


                    }
                    where += WherePo + ")";
                    
                }

                string sql = $@"
SELECT
  {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.task_no", "m.task_no")} AS TASK_NO,
m.prod_no,
   
	m.mer_po,
 	(MIN (m.createdate) || '~' || MAX (m.createdate)) as createdate,
 	MAX ((
		SELECT
 			gd.workshop_section_name
 		FROM 
 			bdm_workshop_section_m gd
 		WHERE
 			gd.workshop_section_no = m.workshop_section_no)) as workshop_section_name,
m.department as Department,
m.production_line_code as Production_line_code,
nvl(sum(m.lot_qty),'0') as Inspection_quantity,
sum((select count(1) from rqc_task_detail_t t where t.task_no = m.task_no and commit_type = '0')) as Qualified_quantity,
ROUND(nvl(sum((select count(1) from rqc_task_detail_t t where t.task_no = m.task_no and commit_type = '0')) / sum(m.lot_qty) * 100,'0'),1) || '%' as Pass_rate
--,case when max(m.RESULT)>0 then 'FAIL' when max(m.RESULT) = '0' then 'PASS' end) as Critical_result
		FROM
			rqc_task_m m
		WHERE 1=1 
        and m.task_state = '2'
        {where}
			
		GROUP BY
m.task_no,
m.prod_no,
			m.mer_po,
			m.workshop_section_no,
			m.department,
			m.production_line_code
order by m.mer_po,createdate desc

";
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
        /// 生命周期看板-TQC数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 鞋型
                string PO_ORDER = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 鞋型
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 截止时间

                string where = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    throw new Exception("缺少必要参数【PROD_NO】");
                }
                if (!string.IsNullOrEmpty(start_date))
                {
                    if (!string.IsNullOrEmpty(end_date))
                    {
                        where += $@" AND m.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                    }
                    else
                    {
                        where += $@" AND m.CREATEDATE >='{start_date}'";
                    }

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@"and m.prod_no = '{PROD_NO}' ";
                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    //where += " or M.MER_PO is null ";
                    var arr = PO_ORDER.Split(',');

                    string WherePo = $"AND ( M.MER_PO is null  or";
                    for (int i = 0; i < arr.Length; i++)
                    {

                        if (i == arr.Length - 1)
                        {
                            WherePo += $@" M.MER_PO like '%{arr[i]}%' ";
                        }
                        else
                        {
                            WherePo += $@" M.MER_PO like '%{arr[i]}%' or ";
                        }
                    }

                    where += WherePo + ")";
                }
                string sql = $@"
SELECT
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.task_no", "m.task_no")} AS TASK_NO,
	M .mer_po as PO,
	(MIN (M .createdate) || '~' || MAX (createdate)) AS 日期范围,
	MAX ((SELECT gd.workshop_section_name FROM bdm_workshop_section_m gd WHERE gd.workshop_section_no = M .workshop_section_no)) AS 工段,
	M .department AS 部门,
	M .production_line_code AS 组别,
	sum(nvl(TJ.hg + TJ.bhg + tj.fxhg + tj.fxbhg + TJ.bp,0)) as 检验总数,
    sum(nvl(TJ.hg,0)) as 首检合格总数,
	sum(nvl(TJ.hg + fxhg,0)) as 合格总数,
	sum(nvl(TJ.bp,0)) as B品数量,
	(case when nvl(sum(TJ.hg + TJ.fxhg),0) != 0 then (
	round( nvl(sum(TJ.hg + TJ.fxhg),0) / nvl(sum(TJ.hg + TJ.bhg + tj.fxhg + tj.fxbhg + TJ.bp),0) * 100 ,1) || '%') else '0%' end) as 产线合格率,
	(case when nvl(sum(TJ.hg + TJ.bhg),0) != 0 then (
	round( nvl(sum(TJ.hg),0) / nvl(sum(TJ.hg + TJ.bhg),0) * 100 ,1) || '%') else '0%' end) as RFT -- 230516确认 合格/(合格+不合格)

-- round( nvl(sum(TJ.hg),0) / nvl(sum(TJ.hg + TJ.bhg + tj.fxhg + tj.fxbhg + TJ.bp),0) * 100 ,1) || '%') else '0%' end) as RFT
FROM
	tqc_task_m M
left join (select a.mer_po,a.workshop_section_no,a.department,a.production_line_code,
sum(case when b.commit_type = '0' then 1 else 0 end) as hg,
sum(case when b.commit_type = '1' then 1 else 0 end) as bhg,
sum(case when b.commit_type = '2' then 1 else 0 end) as fxhg,
sum(case when b.commit_type = '3' then 1 else 0 end) as fxbhg,
sum(case when b.commit_type = '4' then 1 else 0 end) as bp
 from tqc_task_m a left join tqc_task_commit_m b on a.task_no = b.task_no GROUP BY a .mer_po,
	a .workshop_section_no,
	a .department,
	a .production_line_code) tj on tj.mer_po = m.mer_po and TJ.workshop_section_no = m.workshop_section_no and TJ.department = m.department and TJ.production_line_code = m.production_line_code
where  m.task_state = '2' 
{where}
GROUP BY
	M .mer_po,
	M .workshop_section_no,
	M .department,
	M .production_line_code
order by m.mer_po,max(m.createdate) desc nulls last";

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
        /// 生命周期看板-合规性数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetComplianceData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 鞋型
                string PO_ORDER = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 鞋型
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 截止时间

                string where = string.Empty;
                string where2 = string.Empty;

                string where3 = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    throw new Exception("缺少必要参数【PROD_NO】");
                }
                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    end_date = Convert.ToDateTime(end_date).AddDays(1).ToString("yyyy-MM-dd");
                    where += $@" AND A.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                    where2 += $@" AND( b.CURR_UPLOAD_TIME between to_date('{start_date}','YYYY-MM-DD') and to_date('{end_date}','YYYY-MM-DD'))";

                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    where += GetPO(PO_ORDER, "bm.MER_PO");
                    where2 += GetPO(PO_ORDER, "a.prod_no");
                }
                string sql = $@"WITH TB1 AS ( SELECT SE_ID, MIN( POSTING_DATE ) postdate FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID )
                                SELECT
	                                bm.MER_PO,
	                                TO_CHAR( bi.NLT, 'yyyy-mm-dd' ) as PODD,
	                                a.prod_no,
	                                b.enum_code AS filetype_no,
	                                TO_CHAR( tb1.POSTDATE, 'yyyy-mm-dd' ) AS OUT_TIME, --出货日期,
	                                '' AS START_TIME, --开始日期,
	                                b.enum_value AS curr_file_type,--文件类型
	                                a.file_type,--文件类型代号
	                                TO_CHAR( to_date( a.UPLOAD_TIME, 'yyyy/MM/dd HH24:mi:ss' ), 'yyyy-MM-dd' ) curr_upload_time,--上传时间
	                                TO_CHAR( to_date( a.CURR_VALID_TIME, 'yyyy/MM/dd HH24:mi:ss' ), 'yyyy-MM-dd' ) CURR_VALID_TIME,--有效期
	                                a.FILE_GUID --文件GUID
                                FROM
	                                qcm_safety_compliance_file_d a
	                                LEFT JOIN sys001m b ON a.file_type = b.enum_code
	                                LEFT JOIN qcm_safety_compliance_file_m c ON c.prod_no = a.prod_no
	                                LEFT JOIN BDM_SE_ORDER_ITEM bi ON a.prod_no = bi.prod_no
	                                LEFT JOIN BDM_SE_ORDER_MASTER bm ON bm.SE_Id = bi.se_id 
	                                LEFT JOIN tb1 ON tb1.SE_ID = bm.SE_ID 
                                WHERE
	                                b.enum_type = 'enum_qcm_safety' 
	                                AND a.id IN ( SELECT MAX( ID ) FROM qcm_safety_compliance_file_d A GROUP BY PROD_NO, file_type ) 
	                                AND A.PROD_NO='{PROD_NO}' 
                                    AND bm.SE_TYPE in('ZOR1','ZOR2')
                                    {where}
                                ORDER BY
	                                a.id DESC";
                DataTable dt = DB.GetDataTable(sql);

                //联合

                
                string sql2 = $@"WITH TB1 AS ( SELECT SE_ID, MIN( POSTING_DATE ) postdate FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID )
                                SELECT
	                                bm.MER_PO AS PO,
                                    a.file_type,--文件类型代号
	                                TO_CHAR( bi.NLT, 'yyyy-mm-dd' ) AS PODD,
	                                a.prod_no AS ART,
	                                TO_CHAR( tb1.POSTDATE, 'yyyy-mm-dd' ) AS OUT_TIME,
	                                '' AS START_TIME,
	                                a.FILE_TYPE AS curr_file_type,
	                                a.curr_valid_time,
	                                a.UPLOAD_TIME AS curr_upload_time,
	                                a.FILE_GUID  
                                FROM
	                                qcm_jointly_file_d a
	                                LEFT JOIN BDM_SE_ORDER_ITEM bi ON a.prod_no = bi.prod_no
	                                LEFT JOIN BDM_SE_ORDER_MASTER bm ON bm.SE_Id = bi.se_id 
	                                LEFT JOIN tb1 ON tb1.SE_ID = bm.SE_ID 
                                WHERE
	                                1 = 1 
	                                AND id IN ( SELECT MAX( ID ) FROM qcm_jointly_file_d A GROUP BY PROD_NO, file_type ) 
	                                AND A.PROD_NO='{PROD_NO}'
                                    AND bm.SE_TYPE in('ZOR1','ZOR2')
                                    {where}
                                ORDER BY
	                                id DESC";
                
                DataTable dt2 = DB.GetDataTable(sql2);


                //A-01
                string sql3 = $@"
with tmp1 as (
SELECT
    A.SE_TYPE,
	b.prod_no,
	A .mer_po AS PO,
	to_char(b.nlt,'YYYY-MM-DD') AS PODD,
	d.inspection_date,
	c.posting_date,
	'' as ks_date,
	'' as yx_date
	--(select * from aql_app_t_file_m m1 where m1.file_name like b.prod_no||'&'||a.mer_po||'%' order by id desc )
FROM
	BDM_SE_ORDER_MASTER A
LEFT JOIN bdm_se_order_item b ON A .se_id = b.se_id
left join (select se_id,min(posting_date) as posting_date from bmd_se_shipment_m GROUP BY se_id) c on a.se_id = c.se_id
left join (select po,min(case when inspection_state = '1' then to_char(inspection_date,'YYYY-MM-DD') else '' end) as inspection_date from aql_cma_task_list_m GROUP BY po) d on a.mer_po = d.po
),
tmp2 as (
	select substr(m1.file_name,0,instr(m1.file_name,'&',1,1)-1) as prod_no,
	substr(m1.file_name,(INSTR(m1.file_name,'&',1,1))+1,(INSTR(m1.file_name,'&',1,2) - INSTR(m1.file_name,'&',1,1)-1)) as mer_po,
	m1.*
from aql_app_t_file_m m1  
),
tmp3 as (
-- select * from tmp2 where (key_content,CURR_UPLOAD_TIME) in (
-- select key_content,max(CURR_UPLOAD_TIME) CURR_UPLOAD_TIME from tmp2
-- group by key_content)(
select * from tmp2 where id in (
select max(id) as id from tmp2 group by prod_no,mer_po)
)
SELECT
    a.SE_TYPE,
	a.PO,
 	a.PODD,
 	a.inspection_date as 验货时间,
 	a.posting_date as 出货时间,
 	a.ks_date as 开始日期,
 	a.yx_date as 有效期,
 	b.curr_file_guid as 文件GUID,
 	b.file_name,
	b.CURR_UPLOAD_TIME
FROM
	tmp1 A LEFT JOIN tmp3 b on a.po = b.mer_po and a.prod_no = b.prod_no
-- ART
where
 1=1
AND a.SE_TYPE in('ZOR1','ZOR2')
and a.prod_no = '{PROD_NO}'
{where2}

";
                DataTable dt3 = DB.GetDataTable(sql3);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);
                dic.Add("Data3", dt3);
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
        /// 拼接PO模糊查询
        /// </summary>
        /// <param name="PO_ORDER">PO值</param>
        /// <param name="filed">查询数据库字段</param>
        /// <returns></returns>
        public static string GetPO(string PO_ORDER, string filed)
        {
            string strwhere = "";



            var arr = PO_ORDER.Split(',');

            string WherePo = "AND (  ";
            for (int i = 0; i < arr.Length; i++)
            {

                if (i == arr.Length - 1)
                {
                    WherePo += $@"{filed} like '%{arr[i]}%' ";
                }
                else
                {
                    WherePo += $@" {filed} like '%{arr[i]}%' or ";
                }
            }

            strwhere += WherePo + ")";


            return strwhere;
        }
        /// <summary>
        /// 生命周期看板-联名产品数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJointlyData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 鞋型
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 截止时间

                string where = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    throw new Exception("缺少必要参数【PROD_NO】");
                }
                if (!string.IsNullOrEmpty(start_date))
                {
                    if (!string.IsNullOrEmpty(end_date))
                    {
                        where += $@" AND A.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                    }
                    else
                    {
                        where += $@" AND A.CREATEDATE >='{start_date}'";
                    }

                }
                string sql = $@"WITH TB1 AS ( SELECT SE_ID, MIN( POSTING_DATE ) postdate FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID )
                                SELECT
	                                bm.MER_PO AS PO,
	                                TO_CHAR( bi.NLT, 'yyyy-mm-dd' ) AS PODD,
	                                a.prod_no AS ART,
	                                TO_CHAR( tb1.POSTDATE, 'yyyy-mm-dd' ) AS OUT_TIME,
	                                '' AS START_TIME,
	                                a.FILE_TYPE AS curr_file_type,
	                                a.curr_valid_time,
	                                a.UPLOAD_TIME AS curr_upload_time,
	                                a.FILE_GUID  
                                FROM
	                                qcm_jointly_file_d a
	                                LEFT JOIN BDM_SE_ORDER_ITEM bi ON a.prod_no = bi.prod_no
	                                LEFT JOIN BDM_SE_ORDER_MASTER bm ON bm.SE_Id = bi.se_id 
	                                LEFT JOIN tb1 ON tb1.SE_ID = bm.SE_ID 
                                WHERE
	                                1 = 1 
	                                AND id IN ( SELECT MAX( ID ) FROM qcm_jointly_file_d A GROUP BY PROD_NO, file_type ) 
	                                AND A.PROD_NO='{PROD_NO}'
                                    {where}
                                ORDER BY
	                                id DESC";
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
        /// 生命周期看板-AQL数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetA01Data(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 鞋型
                string PO_ORDER = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 截止时间


                string where = string.Empty;
                string AQLwhere = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    throw new Exception("缺少必要参数【PROD_NO】");
                }
                if (!string.IsNullOrEmpty(start_date))
                {
                    if (!string.IsNullOrEmpty(end_date))
                    {
                        AQLwhere += $@" AND A.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                        //where += $@" AND A.CREATEDATE BETWEEN '{start_date}'  AND '{end_date}'";
                    }
                    else
                    {
                        AQLwhere += $@" AND A.CREATEDATE >='{start_date}'";
                        //where += $@" AND A.CREATEDATE >='{start_date}'";
                    }

                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {

                    var arr = PO_ORDER.Split(',');

                    string WherePo = "AND (  ";
                    for (int i = 0; i < arr.Length; i++)
                    {

                        if (i == arr.Length - 1)
                        {
                            WherePo += $@" a.po like '%{arr[i]}%' ";
                        }
                        else
                        {
                            WherePo += $@" a.po like '%{arr[i]}%' or ";
                        }
                    }

                    AQLwhere += WherePo + ")";
                }
                #region old A-01
                /*
                string sql = $@"WITH TB1 AS ( SELECT SE_ID, MIN( POSTING_DATE ) postdate FROM BMD_SE_SHIPMENT_M GROUP BY SE_ID )
                                SELECT
	                                BDM_SE_ORDER_MASTER.MER_PO AS PO,
	                                TO_CHAR( BDM_SE_ORDER_ITEM.NLT, 'yyyy-mm-dd' ) AS PODD,
	                                '' AS Inspection_Date,
	                                TO_CHAR( TB1.POSTDATE, 'yyyy-mm-dd' ) AS PostDate,
	                                '' AS StartDate,
	                                '' AS ValidDate,
	                                '' AS GUID 
                                FROM
	                                BDM_SE_ORDER_ITEM
	                                LEFT JOIN BDM_SE_ORDER_MASTER ON BDM_SE_ORDER_ITEM.SE_ID = BDM_SE_ORDER_MASTER.SE_ID
	                                LEFT JOIN TB1 ON TB1.SE_ID = BDM_SE_ORDER_MASTER.SE_ID 
                                WHERE
	                                1 = 1 
	                                AND BDM_SE_ORDER_ITEM.PROD_NO = '{PROD_NO}'
                                ORDER BY
	                                BDM_SE_ORDER_ITEM.ORG_ID DESC";
                DataTable dt = DB.GetDataTable(sql);
                */
                #endregion
                String AQLSql = $@"WITH tmp AS (
	                                SELECT
		                                SUM( a.bad_qty ) AS bad_qty,
		                                a.task_no 
	                                FROM
		                                aql_cma_task_list_m_aql_e_br a
		                                LEFT JOIN aql_cma_task_list_m_aql_e_br_f b ON a.task_no = b.task_no 
		                                AND a.bad_classify_code = b.bad_classify_code 
		                                AND a.bad_item_code = b.bad_item_code 
	                                WHERE
		                                a.problem_level IN ( '0', '1', '2' ) 
	                                GROUP BY
		                                a.task_no 
	                                ),
	                                TB1 AS ( SELECT PO_NO, MIN( POSTING_DATE ) minpostdate, MAX( POSTING_DATE ) maxpostdate FROM BMD_SE_SHIPMENT_M GROUP BY PO_NO ) SELECT
	                                a.task_no,--任务编号
	                                a.po,--PO
	                                a.po_num,--PO数量
                                    x.DESCOUNTRY_NAME,
	                                a.inspection_date as f_inspection_time,--验货日期
	                                a.lot_num,--分批数量
	                                a.order_level,--制令等级
	                                '' AS Sampling_quantity,--抽验双数
	                                hr.staff_namE AS Inspector,--验货员
	                                '' AS inspection_results,--验货结果
	                                TO_CHAR( TB1.minpostdate, 'yyyy-mm-dd' ) AS First_ShipmentDate,--首次出货日期
	                                TO_CHAR( TB1.maxpostdate, 'yyyy-mm-dd' ) AS Last_ShipmentDate,--最后出货日期
	                                '' AS Report,--验货报告
	                                a.AQL_EDIT_STATE,
	                                NVL( c.sample_level, '2' ) sample_level,
	                                NVL( c.aql_level, 'AC13' ) aql_level,
	                                NVL( TMP.BAD_QTY, 0 ) BAD_QTY,
                                    a.inspection_state
                                FROM
	                                aql_cma_task_list_m a
	                                LEFT JOIN hr001m hr ON hr.staff_no = a.CHECKER
                                    left join bdm_se_order_master x on x.MER_PO = a.PO
	                                LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no = a.task_no
	                                LEFT JOIN TMP ON TMP.task_no = A.TASK_NO
	                                LEFT JOIN TB1 ON TB1.PO_NO = A.po 
                               WHERE
	                                1 = 1 
	                                AND a.ART_NO = '{PROD_NO}'     
                                    {AQLwhere}                               
                                        ";
                DataTable AQLDT = DB.GetDataTable(AQLSql);
                List<string> TaskList = new List<string>();

                foreach (DataRow item in AQLDT.Rows)
                {
                    string acSql = $@"select VALS, {item["aql_level"].ToString()} as AC from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{item["sample_level"].ToString()}' and to_number(START_QTY)<={item["lot_num"].ToString()} and to_number(END_QTY)>={item["lot_num"].ToString()}";
                    DataTable acDt = DB.GetDataTable(acSql);
                    if (acDt.Rows.Count > 0)
                    {
                        item["Sampling_quantity"] = acDt.Rows[0]["VALS"];
                        int acInt = 0;
                        bool cRes = int.TryParse(acDt.Rows[0]["AC"].ToString(), out acInt);
                        if (cRes)
                        {
                            if (!string.IsNullOrEmpty(item["f_inspection_time"].ToString()))
                            {
                                if (Convert.ToInt32(item["BAD_QTY"].ToString()) > acInt)
                                    item["inspection_results"] = "FAIL";
                                else
                                    item["inspection_results"] = "PASS";
                            }

                        }
                    }
                    if (item["inspection_state"].ToString() == "0")
                        item["inspection_state"] = "未验货";
                    else if (item["inspection_state"].ToString() == "1")
                        item["inspection_state"] = "已验货";
                    else
                        item["inspection_state"] = "";
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("Data", dt);
                dic.Add("AQLData", AQLDT);
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
        /// 生命周期看板-客户投诉数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomerComplaintData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;


                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    where += GetPO(PO_ORDER, "m.PO_ORDER");
                }



                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( TO_CHAR( m.COMPLAINT_DATE,'yyyy-mm-dd') BETWEEN '{start_date}' AND '{end_date}') ";
                }
                string sql = $@"select 
                        
                        MAX(m.id) as mid,
                        m.COMPLAINT_NO,
                        MAX(TO_CHAR( m.COMPLAINT_DATE,'yyyy-mm-dd')) as COMPLAINT_DATE,
                        MAX(m.COUNTRY_REGION) as COUNTRY_REGION,
                        MAX(m.PO_ORDER) as PO_ORDER,
                        MAX(t.SE_QTY) as ts_posl,
                        MAX(m.DEFECT_CONTENT) as DEFECT_CONTENT,
                        MAX(m.NG_QTY) as NG_QTY,
                        MAX(m.COMPLAINT_MONEY) as COMPLAINT_MONEY,
                        MAX(m.STATUS) as STATUS,
                        MAX(m.processing_results_status) as processing_results_status,
                        MAX(p.DEVELOP_SEASON) as DEVELOP_SEASON,
                        MAX(bb.name_t) as Category,
                        MAX(p.user_section) as user_section,
                        MAX(p.PRODUCT_MONTH) as PRODUCT_MONTH,
                        MAX(t.prod_no) as prod_no,
                        MAX(p.name_t) as prod_name,
                        MAX(t.shoe_no) as shoe_no,
                        MAX(r.name_t) as shoe_name,
                        MAX(p.COLOR_WAY) as Material_Way
                        from 
                        QCM_CUSTOMER_COMPLAINT_M m
                        LEFT JOIN BDM_SE_ORDER_MASTER b on m.PO_ORDER=b.mer_po
                        LEFT JOIN BDM_SE_ORDER_ITEM t on b.se_id=t.se_id
                        LEFT JOIN BDM_RD_PROD p on t.prod_no =p.prod_no
                        LEFT JOIN BDM_RD_STYLE r on t.shoe_no=r.shoe_no
                        LEFT JOIN bdm_cd_code bb ON r.style_seq=bb.code_no
                        where 1=1 and t.PROD_NO = '{PROD_NO}' {where} 
                        GROUP BY m.COMPLAINT_NO
                        order by max(m.id) desc";
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
        /// 生命周期看板-中国区客户投诉数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCNCustomerComplaintData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( a.RETURN_MONTH BETWEEN '{start_date}' AND '{end_date}') ";

                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    where = GetPO(PO_ORDER, "a.po ");
                }

                string sql = $@"SELECT
   DISTINCT
   MAX( a.id),
    a.RETURN_MONTH,
	MAX(a.task_no) as task_no,
	MAX(r.prod_no) as prod_no,--art
    a.po ,--po,
    nvl(sum(a.newshoes_qty+oldshoes_qty),0) as return_qty, -- 退货数量
    nvl(sum(a.compensation_amount),0) as compensation_amount,-- 退货金额
(
select nvl(sum(shipping_qty),0) from bmd_se_shipment_m m left join bmd_se_shipment_d d on m.shipping_no=d.shipping_no where m.po_no='{PROD_NO}'
) as shipping_qty, -- 出货数量
   '' as qty1, -- 第一多退货代码
    '' as qty2, -- 第二多退货代码
    '' as qty3 -- 第三多退货代码
    
FROM
     qcm_market_feedback_m a 
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
where 1=1  and r.prod_no = '{PROD_NO}' {where} 
group by a.po,a.RETURN_MONTH
order by MAX(id) desc";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    var dttcode = DB.GetDataTable($@"
SELECT
	MAX(a.po),MAX(r.prod_no),MAX(a.main_code)||'*'||sum(newshoes_qty+oldshoes_qty) as code,count(main_code)
FROM
	qcm_market_feedback_m a
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
 WHERE r.prod_no = '{item["prod_no"]}' and  a.po = '{item["po"]}'
group by a.po,r.prod_no
ORDER BY count(main_code)  desc ");
                    if (dttcode.Rows.Count > 0)
                    {
                        for (int i = 0; i < dttcode.Rows.Count; i++)
                        {
                            item[$"qty{i + 1}"] = dttcode.Rows[i]["code"].ToString();

                            if (i == 3)
                                break;
                        }

                    }


                }


                decimal sum_money = 0M; // 总退货金额
                decimal sum_return = 0M; // 总退货数量
                decimal sum_outqty = 0M;//总出货数量
                string sum_rate = "0%"; // 总退货率:总退货数量÷总出货数量X 100%  

                sum_outqty = DB.GetDecimal($@"
SELECT
	nvl(SUM (shipping_qty),0)
FROM
	bmd_se_shipment_m M
LEFT JOIN bmd_se_shipment_d D ON M .shipping_no = D .shipping_no
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN BDM_SE_ORDER_MASTER f on m.SE_ID = f.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
WHERE
	r .prod_no = '{PROD_NO}' and  f.DESCOUNTRY_CODE = 'CN'");

                DataTable dtt = DB.GetDataTable($@"
SELECT
nvl(sum(a.newshoes_qty+oldshoes_qty),0) qtyreturn,-- 退货数量
nvl(sum(a.compensation_amount),0) as compensation_amount  -- 退货金额
FROM
     qcm_market_feedback_m a 
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
where r.prod_no = '{PROD_NO}'");
                if (dtt.Rows.Count > 0)
                {
                    sum_money = Convert.ToDecimal(dtt.Rows[0]["qtyreturn"].ToString());
                    sum_return = Convert.ToDecimal(dtt.Rows[0]["compensation_amount"].ToString());
                }

                if (sum_outqty != 0)
                    sum_rate = Math.Round((sum_return / sum_outqty) * 100,1) + "%";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("SUM_MONEY", sum_money);
                dic.Add("SUM_RETURN", sum_return);
                dic.Add("SUM_OUTQTY", sum_outqty);
                dic.Add("SUM_RATE", sum_rate);
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
        /// 生命周期看板-客户退货数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCustomerReturnData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( a.RETURN_MONTH  BETWEEN '{start_date}' AND '{end_date}') ";

                }

                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                }

                string sql = $@"
SELECT
	MAX(a.RETURN_MONTH) as RETURNDATE,
	MAX(a.ARTICLE) as prod_no,
nvl(sum(a.QTY),0) as return_qty, -- 退货数量
    nvl(sum(a.MONEY),0) as compensation_amount,-- 退货金额
(
select nvl( sum(shipping_qty),0) from bmd_se_shipment_m m left join bmd_se_shipment_d d on m.shipping_no=d.shipping_no where m.po_no='{PROD_NO}'
) as shipping_qty, -- 出货数量
   '' as qty1, -- 第一多退货代码
    '' as qty2, -- 第二多退货代码
    '' as qty3 -- 第三多退货代码
	
FROM
	QCM_CUSTOMER_RETURN_M a
where 1=1  and a.ARTICLE = '{PROD_NO}' {where} 
GROUP by a.RETURN_MONTH
ORDER BY a.RETURN_MONTH";

                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    var dtt2 = DB.GetDataTable($@"
SELECT tt.* from (
SELECT MASTERCODE,count(MASTERCODE) countt from QCM_CUSTOMER_RETURN_M 
where RETURN_MONTH = '{item["RETURNDATE"]}' GROUP BY MASTERCODE
)tt ORDER BY countt desc");
                    if (dtt2.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtt2.Rows.Count; i++)
                        {
                            if (i == 3)
                                break;
                            item[$"qty{i + 1}"] = dtt2.Rows[i]["MASTERCODE"].ToString();
                            
                        }
                    }
                }


                decimal sum_money = 0M; // 总退货金额
                decimal sum_return = 0M; // 总退货数量
                decimal sum_outqty = 0M;//总出货数量
                string sum_rate = "0%"; // 总退货率:总退货数量÷总出货数量X 100%  

                sum_outqty = DB.GetDecimal($@"
SELECT
	nvl(SUM (shipping_qty),0)
FROM
	bmd_se_shipment_m M
LEFT JOIN bmd_se_shipment_d D ON M .shipping_no = D .shipping_no
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
WHERE
	r .prod_no = '{PROD_NO}'");

                DataTable dtt = DB.GetDataTable($@"
SELECT
nvl(sum(a.newshoes_qty+oldshoes_qty),0) qtyreturn,-- 退货数量
nvl(sum(a.compensation_amount),0) as compensation_amount  -- 退货金额
FROM
     qcm_market_feedback_m a 
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
where r.prod_no = '{PROD_NO}'");
                if (dtt.Rows.Count > 0)
                {
                    sum_money = Convert.ToDecimal(dtt.Rows[0]["qtyreturn"].ToString());
                    sum_return = Convert.ToDecimal(dtt.Rows[0]["compensation_amount"].ToString());
                }

                if (sum_outqty != 0)
                    sum_rate =Math.Round((sum_return / sum_outqty) * 100,1) + "%";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("SUM_MONEY", sum_money);
                dic.Add("SUM_RETURN", sum_return);
                dic.Add("SUM_OUTQTY", sum_outqty);
                dic.Add("SUM_RATE", sum_rate);
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
        /// 生命周期看板-金属检测数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJSJCtData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                string where2 = string.Empty;

                if (!string.IsNullOrEmpty(PO_ORDER))
                {

                    var arr = PO_ORDER.Split(',');

                    string WherePo = "AND (  ";
                    for (int i = 0; i < arr.Length; i++)
                    {

                        if (i == arr.Length - 1)
                        {
                            WherePo += $@" a.po like '%{arr[i]}%' ";
                        }
                        else
                        {
                            WherePo += $@" a.po like '%{arr[i]}%' or ";
                        }
                    }

                    where += WherePo + ")";
                }


                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@"AND a.art = '{PROD_NO}'";
                }

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( b.riqi  BETWEEN '{start_date}' AND '{end_date}') ";
                    where2 += $@"AND to_char(to_date(riqi,'yyyy-mm-dd'),'yyyymmdd') BETWEEN '{start_date}' AND '{end_date}') ";

                }

                //to_char(to_date(riqi,'yyyy-mm-dd'),'yyyymmdd')
                string sql = $@"
SELECT 
     MAX(a.po) as po,
    MAX(a.art) as art,
    -- sum(a.SE_SUM) as SE_SUM,
		MAX(d.se_qty) as SE_NUM,
	(MIN (b.riqi) || '~' || MAX (b.riqi)) as RIQI,
    '' as qty1, -- 检验数量
    '' as qty2, -- 检验通过数量
    '' as qty3, -- 检验不通数量
    '' as xqty1, -- x光机复测数量
    '' as xqty2, -- x光机复测通过数量
    '' as xqty3  -- x光机复测不通过数量
FROM
	ESM_MD_ORDERINFOS_LINELIST@APEIOT  a
LEFT JOIN ESM_MD_CHANLIANG_LIST@APEIOT b on a.po = b.po
LEFT JOIN BDM_SE_ORDER_MASTER c on a.po = c.mer_po
LEFT JOIN  BDM_SE_ORDER_ITEM d on c.se_id = d.se_id
WHERE 1=1 {where}

GROUP BY a.art,a.po
";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    //金属检测
                    decimal Pass_qty = DB.GetDecimal($@"select count(1) from ESM_MD_CHANLIANG_LIST@APEIOT  where  po='{item["po"]}' ");
                    decimal Fail_qty = DB.GetDecimal($@"select count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT  where  po='{item["po"]}'");
                    item["qty1"] = Pass_qty + Fail_qty;
                    item["qty2"] = Pass_qty;
                    item["qty3"] = Fail_qty;

                    //X机复测  xray 0-通过 1-不通过
                    decimal XPass_qty = DB.GetDecimal($@"select count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT where po = '{item["po"]}' and xray='0' ");

                    decimal XFail_qty = DB.GetDecimal($@"select count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT  where  po='{item["po"]}' and xray='1'");

                    item["xqty1"] = XPass_qty + XFail_qty;
                    item["xqty2"] = XPass_qty;
                    item["xqty3"] = XFail_qty;

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
        /// 生命周期看板-订单信息数据查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDDData(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string where = string.Empty;
                string where2 = string.Empty;

                if (!string.IsNullOrEmpty(PO_ORDER))
                {

                    where += GetPO(PO_ORDER, "a.mer_po");
                }



                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    where += $@"AND b.prod_no = '{PROD_NO}'";
                }

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    //where += $@"AND (c.min_date BETWEEN '{start_date}' AND '{end_date}') or ( b.prod_no = '{PROD_NO}' and c.min_date is null AND A.SE_TYPE in('ZOR1','ZOR2'))";
                    where += $@"AND (to_char(b.NLT,'yyyy-MM-dd') BETWEEN '{start_date}' AND '{end_date}') or ( b.prod_no = '{PROD_NO}' and b.NLT is null AND A.SE_TYPE in('ZOR1','ZOR2'))";

                }
                string FirstPO = DB.GetString($@"SELECT po_no FROM sfc_trackin_list WHERE art_no = '{PROD_NO}' AND process_no = 'L' ORDER BY scan_date");

                string sql = $@"
SELECT
	a.mer_po as PO号,
	c.min_date as 首次出货日期,
	c.max_date as 最后出货日期,
	a.se_custid as 客户编码,
	a.DESCOUNTRY_NAME as 出货国家,
    to_char(b.NLT,'yyyy-MM-dd') as PODD,
	-- a.shipcountry_name as 出货国家,
	b.se_qty as PO数量,
	d.SCAN_DETPT as 生产组别
	-- (case when d.scan_date is not null then '是' else '' end) as 首次上线PO
FROM
	bdm_se_order_master A
LEFT JOIN bdm_se_order_item b ON A .se_id = b.se_id
 LEFT JOIN (
	SELECT se_id,to_char(min(posting_date),'yyyy-MM-dd') as min_date,to_char(max(posting_date),'yyyy-MM-dd') as max_date  FROM bmd_se_shipment_m GROUP BY se_id
)c on c.se_id  = a.se_id
left join (select se_id,listagg(DISTINCT SCAN_DETPT,',') as SCAN_DETPT,min(scan_date) as scan_date  from sfc_trackin_list where process_no='L' GROUP BY se_id) d on d.se_id = a.se_id
where 1=1 AND A.SE_TYPE in('ZOR1','ZOR2') {where} 
";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("PO", FirstPO);
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
        /// 获取DQA文件集合
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAFilelist(object OBJ)
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
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//查询条件 
                string FileType = jarr.ContainsKey("FileType") ? jarr["FileType"].ToString() : "";
                string sql = string.Empty;

                //0-合规 1-联合
                if (FileType == "0")
                {
                    sql = $@"SELECT
                                    a.id,
	                                bm.MER_PO,
	                                a.prod_no,
	                                b.enum_code AS filetype_no,
	                                a.file_type,--文件类型代号
	                                a.FILE_GUID,--文件GUID
									cc.file_url,cc.file_name,'BDM_UPLOAD_FILE_ITEM'as tablename,cc.guid
                                FROM
	                                qcm_safety_compliance_file_d a
	                                LEFT JOIN sys001m b ON a.file_type = b.enum_code
	                                LEFT JOIN qcm_safety_compliance_file_m c ON c.prod_no = a.prod_no
	                                LEFT JOIN BDM_SE_ORDER_ITEM bi ON a.prod_no = bi.prod_no
	                                LEFT JOIN BDM_SE_ORDER_MASTER bm ON bm.SE_Id = bi.se_id 
																	LEFT JOIN BDM_UPLOAD_FILE_ITEM cc on cc.guid = a.FILE_GUID
                                WHERE
	                                b.enum_type = 'enum_qcm_safety' 
	                               
	                                AND A.PROD_NO='{PROD_NO}'  and  bm.MER_PO = '{PO}' and b.enum_code='{TYPE}'
                                ORDER BY
	                                a.id DESC";
                }
               else if(FileType == "1")
                {
                    sql = $@"SELECT
                                    a.id,
	                                bm.MER_PO,
	                                a.prod_no,
	                                a.file_type,--文件类型代号
	                                a.FILE_GUID,--文件GUID
									cc.file_url,
										cc.file_name,'BDM_UPLOAD_FILE_ITEM'as tablename,
									cc.guid
                                FROM
	                                qcm_jointly_file_d a
	                                LEFT JOIN qcm_jointly_file_m c ON c.prod_no = a.prod_no
	                                LEFT JOIN BDM_SE_ORDER_ITEM bi ON a.prod_no = bi.prod_no
	                                LEFT JOIN BDM_SE_ORDER_MASTER bm ON bm.SE_Id = bi.se_id 
																	LEFT JOIN BDM_UPLOAD_FILE_ITEM cc on cc.guid = a.FILE_GUID
                                WHERE 1=1
								AND A.PROD_NO='{PROD_NO}'  and  bm.MER_PO = '{PO}' and a.file_type='{TYPE}'
                                ORDER BY
	                                a.id DESC";
                }
                
                 
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

        #region 试穿报告接口 wearTest

        /// <summary>
        /// 获取Wear报告内容接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWearReportInfoAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            //试穿取值
            DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
            if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
            {
                var dbConfigRow = dbConfigDt.Rows[0];
                wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
            }
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string WearCode = jarr["WearCode"].ToString();
                string UserCode = jarr["UserCode"].ToString();

                //guid = GDSJ_Framework.Common.WebServiceHelper.SavePostLog(DB, ReqObj.IP4, ReqObj.MAC, ReqObj.DllName, ReqObj.ClassName, ReqObj.Method, Data);
                if (!string.IsNullOrEmpty(WearCode))
                {
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        string sql = @"select * from WearReport where WEARCODE='" + WearCode + "' and USERCODE='" + UserCode + "'";
                        DataTable dt = wbscDB.GetDataTable(sql);
                        string sql1 = @"select * from WearDateReport where WEARCODE='" + WearCode + "' and USERCODE='" + UserCode + "'";
                        DataTable dt1 = wbscDB.GetDataTable(sql1);
                        Dictionary<string, string> p = new Dictionary<string, string>();
                        // Dictionary<string, string> d = new Dictionary<string, string>();
                        if (dt.Rows.Count > 0)
                        {

                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                            }
                            string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                            p.Add("WearReport", json);
                            //ret.RetData = js.Serialize(p);
                        }
                        if (dt1.Rows.Count > 0)
                        {
                            string json = string.Empty;
                            string json2 = string.Empty;
                            for (int i = 0; i < dt1.Rows.Count; i++)
                            {
                                Dictionary<string, Object> a = new Dictionary<string, Object>();
                                string Testtype = dt1.Rows[i]["Testtype"].ToString().Trim();
                                string Testweather = dt1.Rows[i]["Testweather"].ToString().Trim();
                                string Wearcode = dt1.Rows[i]["Wearcode"].ToString().Trim();
                                string Usercode = dt1.Rows[i]["Usercode"].ToString().Trim();
                                string TestDate = dt1.Rows[i]["TestDate"].ToString().Trim();
                                string Testhours1 = dt1.Rows[i]["Testhours1"].ToString().Trim();
                                string Testhours2 = dt1.Rows[i]["Testhours2"].ToString().Trim();
                                string Testinfo = dt1.Rows[i]["Testinfo"].ToString().Trim();
                                string[] Testtype1 = new string[] { Testtype };
                                string[] Testweather1 = new string[] { Testweather };
                                a.Add("Testtype", Testtype1);
                                a.Add("Testweather", Testweather1);
                                a.Add("Wearcode", Wearcode);
                                a.Add("Usercode", Usercode);
                                a.Add("TestDate", TestDate);
                                a.Add("Testhours1", Testhours1);
                                a.Add("Testhours2", Testhours2);
                                a.Add("Testinfo", Testinfo);
                                string data = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                                //string data = js.Serialize(a);
                                if (string.IsNullOrEmpty(json))
                                {
                                    json = data;
                                }
                                else
                                {
                                    json += "," + data;
                                }

                            }
                            json2 = "[" + json + "]";
                            p.Add("WearDateReport", json2);
                        }
                        if (dt.Rows.Count == 0 && dt1.Rows.Count == 0)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "查无此数据！";
                        }
                        else
                        {
                            ret.IsSuccess = true;
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);


                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "UserCode不能为空！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "WearCode不能为空！";
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            //GDSJ_Framework.Common.WebServiceHelper.SaveRetLog(wbscDB, guid, ret.IsSuccess.ToString(), ret.RetData);

            return ret;
        }

        /// <summary>
        /// 用户资料获取接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserInfoAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            //试穿取值
            DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
            if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
            {
                var dbConfigRow = dbConfigDt.Rows[0];
                wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
            }

            try
            {

                Data = ReqObj.Data.ToString();
                //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string UserCode = jarr["UserCode"].ToString();

                //wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string sql = "select * from UserInfo where UserCode='" + UserCode + "'";

                System.Data.DataTable dt = wbscDB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                    }
                    //string dtJson = GDSJ_Framework.Common.JsonHelper.GetJsonByDataTable(dt);
                    string dtJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

                    ret.IsSuccess = true;
                    ret.RetData = dtJson;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "UserInfo表无此数据！";
                }



            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 获取Wear任务信息列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWearListAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            //试穿取值
            DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
            if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
            {
                var dbConfigRow = dbConfigDt.Rows[0];
                wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
            }

            try
            {

                Data = ReqObj.Data.ToString();
                //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Where = jarr["Where"].ToString();
                string OrderBy = jarr["OrderBy"].ToString();
                string PageRow = jarr["PageRow"].ToString();
                string Page = jarr["Page"].ToString();
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                int maxRecord = int.Parse(Page) * int.Parse(PageRow);
                string sql = @"select * from (
select @n:= @n + 1 as RN,W.*,
(select count(*) from WearUser R where R.WEARCODE = W.WEARCODE) as WearUserCount,
(select count(*) from WearReport R where R.WEARCODE = W.WEARCODE) as WearReportCount
from WearInfo W,(select @n:= 0) d  where 1=1 " + Where + " " + OrderBy + "  ) tab where RN > " + total + " AND RN <= " + maxRecord + " limit " + PageRow + "";
                string sql1 = "select count(*)AS TOTAL from WearInfo W where 1 = 1 " + Where + "";
                System.Data.DataTable dt = wbscDB.GetDataTable(sql);
                System.Data.DataTable dts = wbscDB.GetDataTable(sql1);
                var p = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                    }
                    //var dtJson = GDSJFramework_NETCore.Common.JsonHelper.TableToJsonObject(dt);
                    var dtJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    //string dtJsons = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dts);
                    string dtJsons = dts.Rows[0]["TOTAL"].ToString();
                    p.Add("json", dtJson);
                    p.Add("total", dtJsons);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            // GDSJFramework_NETCore.Common.WebServiceHelper.SaveRetLog(DB, guid, ret.IsSuccess.ToString(), ret.RetData);

            return ret;
        }


        /// <summary>
        /// 获取Wear报告列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWearReportListAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            string Data = string.Empty;

            string guid = string.Empty;

            try
            {

                //试穿取值
                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }



                Data = ReqObj.Data.ToString();
                // var js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TESTID = jarr.ContainsKey("TESTID") ? jarr["TESTID"].ToString() : "";//查询条件 ART
                string USERCODE = jarr.ContainsKey("USERCODE") ? jarr["USERCODE"].ToString() : "";//查询条件 ART
                var p = new Dictionary<string, object>();

                string sql = $@"select *,@n:= @n + 1 as RN  from (
select wu.id,wu.WearCode,US.UserName,US.UserHeight,US.UserSex,US.UserCode,US.UserWeight,US.USERBIRTHDATE,US.ROLE,US.CODENUMBER,US.SHOETYPE,
US.WearUserType,US.ADDRESS,US.CATEGORY,WI.StartDate,WI.EndDate,WI.ShoesType,WI.ShoesName,WI.ArtNo,WI.Info,wu.ThroughState,
DATE_FORMAT(FROM_DAYS(TO_DAYS(NOW()) - TO_DAYS(US.USERBIRTHDATE)), '%Y') + 0 as UserAge,IFNULL(wu.TotalScore,0) as 'Score',
WI.LowHour,WI.HeightHour,wu.UsedHour,wu.TotalScore,wu.HOURS1,wu.HOURS2,WI.ATPID,WI.SEASON,WI.PBTYPE,WI.SIZE,WI.MODEL,WI.TESTLEVEL,WI.INTENDEDUSE,WI.GENDER,
WI.SAMPLETYPE,WI.TESTTYPE,WI.LAST,WI.STAGE,WI.TESTREASON,WI.WEARINGHOURSP,WI.WEARINGHOURSC,WI.TYPE,WI.COORDINATOR,WI.TESTPHASE,WI.CREATOR,WI.CREATCODE
from wearuser wu LEFT JOIN UserInfo US on wu.USERCODE = US.USERCODE 
LEFT JOIN WearInfo WI on wu.WearCode = WI.WearCode,(select @n:= 0) d
where 1 = 1  and wu.WearCode = '{TESTID}' and  US.UserCode= '{USERCODE}'
GROUP BY WearCode,UserName,UserHeight,UserSex,UserCode,UserWeight,StartDate,EndDate,ShoesType,ShoesName,ArtNo,Info,ThroughState,UserAge,Score,LowHour,HeightHour,
UsedHour,TotalScore,USERBIRTHDATE,ATPID,SEASON,PBTYPE,SIZE,MODEL,TESTLEVEL,INTENDEDUSE,GENDER,SAMPLETYPE,TESTTYPE,LAST,STAGE,TESTREASON,WEARINGHOURSP,WEARINGHOURSC
) tab where 1=1";

                DataTable dt = wbscDB.GetDataTable(sql);
                //DataTable dts = DB.GetDataTable(sql1);

                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                    }
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    p.Add("json", json);

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无报告！";
                }

                // }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWearUserInfoAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            try
            {
                

                SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
                SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

                string Data = string.Empty;
                Data = ReqObj.Data.ToString();
                string guid = string.Empty;

                //试穿取值
                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }

               
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string userCode = jarr["userCode"].ToString();
                string wearCode = jarr["wearCode"].ToString();

                string sql = "select  * from wearuser where USERCODE='" + userCode + "' and WEARCODE='" + wearCode + "' limit 1";

                System.Data.DataTable dt = wbscDB.GetDataTable(sql);

                if (dt.Rows.Count > 0)
                {
                    string dtJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                    ret.RetData = dtJson;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "wearuser表无此数据！";
                }



            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.ToString()+ " InnerException:" + ex.InnerException+ " StackTrace:" + ex.StackTrace;
            }

            //GDSJ_Framework.Common.WebServiceHelper.SaveRetLog(DB, guid, ret.IsSuccess.ToString(), ret.RetData);

            return ret;
        }

        #endregion

        #region 试穿报告接口 FitTest

        /// <summary>
        /// 获取Fit报告内容接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFitReportInfoAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
           
            string Data = string.Empty;

            string guid = string.Empty;

            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }
                //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string FitCode = jarr["FITCODE"].ToString();
                //wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                //guid = GDSJFramework_NETCore.Common.WebServiceHelper.SavePostLog(DB, ReqObj.IP4, ReqObj.MAC, ReqObj.DllName, ReqObj.ClassName, ReqObj.Method, Data);
                if (!string.IsNullOrEmpty(FitCode))
                {
                    string sql = @"select * from FitReport where  FitCode='" + FitCode + "'";

                    DataTable dt = wbscDB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        ret.IsSuccess = true;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                        }
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                        ret.RetData = json;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无此数据！";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "FitCode不能为空！";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            // GDSJFramework_NETCore.Common.WebServiceHelper.SaveRetLog(DB, guid, ret.IsSuccess.ToString(), ret.RetData);

            return ret;
        }

        /// <summary>
        /// 获取Fit任务信息列表接口111
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFitListAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }
                Data = ReqObj.Data.ToString();
                //var js = new Newtonsoft.Json.JsonConvert.Serialization();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Where = jarr["WHERE"].ToString();
                string OrderBy = jarr["ORDERBY"].ToString();
                string PageRow = jarr["PAGEROW"].ToString();
                string Page = jarr["PAGE"].ToString();
                var p = new Dictionary<string, object>();
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                int maxRecord = int.Parse(Page) * int.Parse(PageRow);
                string sql = @"select newtable.* from (SELECT *,@n:= @n + 1 as RN  
                FROM FitInfo,(select @n:= 0) d where 1=1 " + Where + " " + OrderBy + ") newtable where RN > " + total + " AND RN <= " + maxRecord + "  limit " + PageRow + " ";
                string sql1 = "select count(*)AS TOTAL from FitInfo WHERE 1=1 " + Where + "";
                System.Data.DataTable dt = wbscDB.GetDataTable(sql);
                System.Data.DataTable dts = wbscDB.GetDataTable(sql1);
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        dt.Columns[i].ColumnName = dt.Columns[i].ColumnName.ToUpper();
                    }
                    var dtJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    //string dtJsons = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dts);
                    string dtJsons = dts.Rows[0]["TOTAL"].ToString();
                    p.Add("json", dtJson);
                    p.Add("total", dtJsons);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                }



            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 获取checklist
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCheckListAPI(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var dbConfigDt = DBSqlServer.GetDataTable($@"
SELECT
	*
FROM
	DBLINKCONFIG
WHERE
	configno='wbsc';");
                if (dbConfigDt != null && dbConfigDt.Rows.Count > 0)
                {
                    var dbConfigRow = dbConfigDt.Rows[0];
                    wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");
                }

                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string fitCode = jarr["FITCODE"].ToString();
                string sql = "select * from fitchecklist where FITCODE='" + fitCode + "'";

                System.Data.DataTable dt = wbscDB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    var dtJson = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                    ret.RetData = dtJson;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                }



            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        #endregion


        /// <summary>
        /// 前端Q品质看板-实验室测试_柱状图 月
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQLaboratory(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
                string ART_NO = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//查询条件 
                string ORDER_PO = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//查询条件 
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//查询条件  厂商
                string manufacturer_type = jarr.ContainsKey("manufacturer_type") ? jarr["manufacturer_type"].ToString() : "";//查询条件  厂商类型
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//查询条件 部件
                string MAKINGS_ID = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//查询条件 料号

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//查询条件

                string Where = string.Empty;

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(ORDER_PO))
                {
                    Where += $@"and a.ORDER_PO like '%{ORDER_PO}%' ";
                    //Where += $@"and a.ORDER_PO = '{ORDER_PO}'";
                }
                if (!string.IsNullOrEmpty(manufacturer_name))
                {
                    arr = manufacturer_name.Split(',');
                    Where += $@"and ( a.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";
                    //Where += $@"and a.manufacturer_name like '%{manufacturer_name}%' ";
                }
                if (!string.IsNullOrEmpty(manufacturer_type))
                {
                    arr = manufacturer_type.Split(',');
                    Where += $@"and ( b.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";

                    //Where += $@"and b.M_TYPE like '%{manufacturer_type}%' ";
                }
                if (!string.IsNullOrEmpty(parts_name))
                {
                    arr = parts_name.Split(',');
                    Where += $@"and a.parts_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.parts_name like '%{parts_name}%' ";
                }
                if (!string.IsNullOrEmpty(MAKINGS_ID))
                {
                    arr = MAKINGS_ID.Split(',');
                    Where += $@"and a.MAKINGS_ID in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.MAKINGS_ID like '%{MAKINGS_ID}%'";
                }
                DateTime lastyear = DateTime.Now.AddYears(-1);
                DateTime start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                DateTime end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                int months = end.Month - start.Month+1;



                //int months = ts.mon;
                if (months <= 0)
                {
                    throw new Exception("请选择正确日期范围！");
                }
                if (months > 12)
                {
                    throw new Exception("所选择月份数禁止大于12个月！");
                }

                string sql = $@"
with tmp1 as (
SELECT datemonth,sum(pass_qty) pass_qty,sum(bad_qty) bad_qty from (
	select a.CREATEDATE ,to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') datemonth,
	sum(case when a.TEST_RESULT='PASS' then to_number(a.SEND_TEST_QTY) else 0 end) pass_qty,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from qcm_ex_task_list_m a
    LEFT JOIN base003m b on a.MANUFACTURER_CODE = b.SUPPLIERS_CODE
    
    LEFT JOIN HR001M c on a.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on c.UDF01 = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
	where nvl(a.TEST_RESULT,'NULL')!='NULL'
	and a.CREATEDATE between '{start.ToString("yyyy-MM-dd")}' and '{end.ToString("yyyy-MM-dd")}'
    {Where}
	group by a.CREATEDATE
)tt
GROUP BY datemonth

)
select datemonth,pass_qty,bad_qty,round(pass_qty/(pass_qty+bad_qty)*100) pass_rate from tmp1
order by datemonth ";

                //and a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code5, new List<string>() { "测试报告通过数" , "测试报告不通过数" , "合格率" });
                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = lanDic["测试报告通过数"].ToString();

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = lanDic["测试报告不通过数"].ToString();

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = lanDic["合格率"].ToString();

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                //遍历N个月
                for (int i = 0; i < months; i++)
                {
                    Xdata.Add(lastyear.AddMonths(1).AddMonths(i).ToString("yyyy-MM"));
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"datemonth = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["pass_qty"].ToString());
                        list_fail.Add(dr[0]["bad_qty"].ToString());
                        list_rate.Add(dr[0]["pass_rate"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 前端Q品质看板-实验室测试_饼状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQLaboratorybyPie(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string type = jarr.ContainsKey("type") ? jarr["type"].ToString() : "";//查询条件  0-年度 1-按查询条件

                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//查询条件 
                string order_po = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//查询条件 
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//查询条件  厂商
                string manufacturer_type = jarr.ContainsKey("manufacturer_type") ? jarr["manufacturer_type"].ToString() : "";//查询条件  厂商类型
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//查询条件 部件
                string makings_id = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//查询条件 料号

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//查询条件

                string Wheredate = string.Empty;
                string Where = string.Empty;
                decimal qtysum =0M;

                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(art_no))
                {
                    arr = art_no.Split(',');
                    Where += $@"and b.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";

                    //Where += $@" and b.ART_NO = '{art_no}'  ";
                }
                if (!string.IsNullOrEmpty(order_po))
                {
                    //arr = order_po.Split(',');
                    //Where += $@"and b.ORDER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where += $@"and b.ORDER_PO like '%{order_po}%'";
                }
                if (!string.IsNullOrEmpty(manufacturer_name))
                {
                    arr = manufacturer_name.Split(',');
                    Where += $@"and ( b.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";
                    //Where += $@"and b.manufacturer_name like '%{manufacturer_name}%' ";
                }
                if (!string.IsNullOrEmpty(manufacturer_type))
                {
                    arr = manufacturer_type.Split(',');
                    Where += $@"and ( b.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";
                    //Where += $@"and c.M_TYPE like '%{manufacturer_type}%' ";
                }
                if (!string.IsNullOrEmpty(parts_name))
                {
                    arr = parts_name.Split(',');
                    Where += $@"and b.parts_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.parts_name like '%{parts_name}%'";
                }
                if (!string.IsNullOrEmpty(makings_id))
                {
                    arr = makings_id.Split(',');
                    Where += $@"and b.makings_id in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.makings_id like '%{makings_id}%'";
                }

                if(type == "0")
                {
                    Wheredate = $@" and a.CREATEDATE between '{DateTime.Now.ToString("yyyy-01-01")}' and '{LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd")}'";

                    qtysum = DB.GetDecimal($@"select count(a.id) bad_count
from qcm_ex_task_list_d a
LEFT JOIN qcm_ex_task_list_m b on a.TASK_NO = b.TASK_NO
where a.CREATEDATE between '{DateTime.Now.ToString("yyyy-01-01")}' and '{LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd")}'
and a.ITEM_TEST_RESULT ='FAIL' {Where}
");
                }
                else
                {
                    Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);
                    Wheredate = $@" and a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'";
                    qtysum = DB.GetDecimal($@"select count(a.id) bad_count
from qcm_ex_task_list_d a
LEFT JOIN qcm_ex_task_list_m b on a.TASK_NO = b.TASK_NO
where a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'
and a.ITEM_TEST_RESULT ='FAIL' {Where}
");
                }
               

                 


                string sql = $@"
SELECT tt.INSPECTION_NAME,nvl(bad_count,0) as bad_count from (
select a.INSPECTION_NAME,count(a.id) bad_count
from qcm_ex_task_list_d a
LEFT JOIN qcm_ex_task_list_m b on a.TASK_NO = b.TASK_NO
LEFT JOIN base003m c on b.MANUFACTURER_CODE = c.SUPPLIERS_CODE

LEFT JOIN HR001M c on b.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on c.UDF01 = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE

where 1=1 {Wheredate}
and a.ITEM_TEST_RESULT ='FAIL' {Where}
group by INSPECTION_NAME ORDER BY count(a.id) desc
)tt
where rownum <=5
union all
SELECT * from (
SELECT 'Other' as INSPECTION_NAME,nvl(SUM(bad_count),0)  as bad_count from (
SELECT ROWNUM as RN,TT.* from (

select a.INSPECTION_NAME,count(a.id) bad_count
from qcm_ex_task_list_d a
LEFT JOIN qcm_ex_task_list_m b on a.TASK_NO = b.TASK_NO
where 1=1 {Wheredate} 
and a.ITEM_TEST_RESULT ='FAIL' {Where}
group by a.INSPECTION_NAME ORDER BY count(a.id) desc
)tt
)yy
where RN >5
)hh where bad_count is not null
";
                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                var dt = DB.GetDataTable(sql);



                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 前端Q品质看板-实验室测试_柱状图_日 默认查当月30天
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQLaboratoryByMonth(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
                string ART_NO = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//查询条件 
                string ORDER_PO = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//查询条件 
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//查询条件  厂商
                string manufacturer_type = jarr.ContainsKey("manufacturer_type") ? jarr["manufacturer_type"].ToString() : "";//查询条件  厂商类别
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//查询条件 部件
                string MAKINGS_ID = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//查询条件 料号

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//查询条件 

                //string starttime = DateTime.Now.ToString("yyyy-MM-01");//查询条件 
                //string endtime = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");//查询条件

                string Where = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(manufacturer_type))
                {
                    arr = manufacturer_type.Split(',');
                    Where += $@"and ( b.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";
                    //Where += $@"and b.M_TYPE like '%{manufacturer_type}%' ";
                }
                if (!string.IsNullOrEmpty(manufacturer_name))
                {
                    arr = manufacturer_name.Split(',');
                    Where += $@"and ( a.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";
                    //Where += $@"and a.manufacturer_name like '%{manufacturer_name}%' ";
                }
                if (!string.IsNullOrEmpty(parts_name))
                {
                    arr = parts_name.Split(',');
                    Where += $@"and a.parts_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.parts_name like '%{parts_name}%' ";
                }

                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(ORDER_PO))
                {
                    //Where += $@"and a.ORDER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where += $@"and a.ORDER_PO like '%{ORDER_PO}%'";
                }

                if (!string.IsNullOrEmpty(MAKINGS_ID))
                {
                    arr = MAKINGS_ID.Split(',');
                    Where += $@"and a.MAKINGS_ID in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                   
                }

                DateTime lastyear = DateTime.Now.AddYears(-1);
                DateTime start = Convert.ToDateTime(starttime);
                DateTime end = Convert.ToDateTime(endtime);

                TimeSpan ts1 = new TimeSpan(start.Ticks);

                TimeSpan ts2 = new TimeSpan(end.Ticks);

                TimeSpan ts = ts1.Subtract(ts2).Duration();
                int days = ts.Days+1;
                string sql = $@"
with tmp1 as (
SELECT datemonth,sum(pass_qty) pass_qty,sum(bad_qty) bad_qty from (
	select a.CREATEDATE ,to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm-dd') datemonth,
	sum(case when a.TEST_RESULT='PASS' then to_number(a.SEND_TEST_QTY) else 0 end) pass_qty,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from qcm_ex_task_list_m a
	LEFT JOIN base003m b on a.MANUFACTURER_CODE = b.SUPPLIERS_CODE
    
LEFT JOIN HR001M c on a.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on c.UDF01 = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
	where nvl(a.TEST_RESULT,'NULL')!='NULL' and trim(translate(send_test_qty,'0123456789',' ')) is NULL
	and a.CREATEDATE between '{starttime}' and '{endtime}'
    {Where}
	group by a.CREATEDATE
)tt
GROUP BY datemonth

)
select datemonth,pass_qty,bad_qty,round(pass_qty/(pass_qty+bad_qty)*100) pass_rate from tmp1
order by datemonth ";

                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code5, new List<string>() {"测试报告通过数","测试报告不通过数","合格率" });

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = lanDic["测试报告通过数"].ToString();

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = lanDic["测试报告不通过数"].ToString();

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = lanDic["合格率"].ToString();

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                //遍历N个月
                for (int i = 0; i < days; i++)
                {
                    Xdata.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"datemonth = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["pass_qty"].ToString());
                        list_fail.Add(dr[0]["bad_qty"].ToString());
                        list_rate.Add(dr[0]["pass_rate"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 前端Q品质看板-实验室测试_厂商 前五名 / 后五名
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQLaboratoryCs(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "1";//排序类型  0-倒序 1- 正序

                string ART_NO = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//查询条件 
                string ORDER_PO = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//查询条件 
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//查询条件  厂商
                string manufacturer_type = jarr.ContainsKey("manufacturer_type") ? jarr["manufacturer_type"].ToString() : "";//查询条件  厂商类型
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//查询条件 部件
                string MAKINGS_ID = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//查询条件 料号

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//查询条件

                string Where = string.Empty;
                string Where2 = string.Empty;
                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(manufacturer_type))
                {
                    arr = manufacturer_type.Split(',');
                    Where += $@"and ( b.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";
                    Where2 += $@"and ( c.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";

                }
                if (!string.IsNullOrEmpty(manufacturer_name))
                {
                    arr = manufacturer_name.Split(',');
                    Where += $@"and ( a.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";
                    Where2 += $@"and ( b.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";

                }
                if (!string.IsNullOrEmpty(parts_name))
                {
                    arr = parts_name.Split(',');
                    Where += $@"and a.parts_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where2 += $@"and b.parts_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where2 += $@"and b.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ORDER_PO))
                {
                    Where += $@"and a.ORDER_PO like '%{ORDER_PO}%' ";
                    Where2 += $@"and b.ORDER_PO like '%{ORDER_PO}%' ";
                }

                if (!string.IsNullOrEmpty(MAKINGS_ID))
                {
                    arr = MAKINGS_ID.Split(',');
                    Where += $@"and a.MAKINGS_ID in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where2 += $@"and b.MAKINGS_ID in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

                if (Type.Trim() == "0")
                {
                    Type = "desc";
                }
                else
                    Type = "asc";

                string sql = $@"with ex_tmp1 as (
	select 
	case when nvl(a.MANUFACTURER_NAME,'NULL') ='NULL' then a.STAFF_DEPARTMENT else a.MANUFACTURER_NAME end dept,
a.*,
case when nvl(b.M_TYPE,'NULL') ='NULL' then f.ORG_NAME else b.M_TYPE end MANUFACTURER_TYPE
-- b.M_TYPE as MANUFACTURER_TYPE
	from qcm_ex_task_list_m a
    LEFT JOIN base003m b on a.MANUFACTURER_CODE = b.SUPPLIERS_CODE

-- LEFT JOIN HR001M c on a.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on a.STAFF_DEPARTMENT_CODE = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
    LEFT JOIN BASE001M f on d.FACTORY_SAP = f.ORG_CODE
	where a.createdate between '{starttime}' and '{endtime}'
    {Where}
),
ex_bad_data_tmp1 as (
	select 
	case when nvl(b.MANUFACTURER_NAME,'NULL') ='NULL' then b.STAFF_DEPARTMENT else b.MANUFACTURER_NAME end dept,
	a.INSPECTION_NAME,c.M_TYPE as MANUFACTURER_TYPE,
	a.id
	from qcm_ex_task_list_d a
	join qcm_ex_task_list_m b on a.TASK_NO =b.TASK_NO 
    LEFT JOIN base003m c on b.MANUFACTURER_CODE = c.SUPPLIERS_CODE

LEFT JOIN HR001M c on b.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on c.UDF01 = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
	where a.CREATEDATE  between '{starttime}' and '{endtime}'
	and a.ITEM_TEST_RESULT ='FAIL'
    {Where2}
),
ex_data as (
	select 
	a.dept,
    a.MANUFACTURER_TYPE,
	count(a.id) send_test_count,
	sum(case when nvl(a.TEST_RESULT,'NULL')!='NULL' then 1 else 0 end) test_count,
	sum(case when a.TEST_RESULT='PASS' then to_number(a.SEND_TEST_QTY) else 0 end) pass_qty,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from ex_tmp1 a
	group by a.MANUFACTURER_TYPE,a.dept
),

ex_bad_data_tmp2 as (
	select 
	dept,
    MANUFACTURER_TYPE,
	INSPECTION_NAME ,
	count(id) bad_qty,
	ROW_NUMBER() over (partition by dept order by count(id) desc) ranking
	from ex_bad_data_tmp1
	group by MANUFACTURER_TYPE,dept,INSPECTION_NAME 
),
total_bad_data as (
	select dept,MANUFACTURER_TYPE,INSPECTION_NAME,bad_qty,ranking,sum(bad_qty) over(partition by dept) totla_bad_qty from ex_bad_data_tmp2
),
ex11 AS ( SELECT a.dept,a.MANUFACTURER_TYPE,CASE 
 WHEN sum(PASS_QTY)+SUM(BAD_QTY)=0 THEN
  0
 ELSE
  ROUND(SUM(pass_qty)/(SUM(pass_qty)+sum(bad_qty))*100, 1)
END AS rate
 FROM ex_data a GROUP BY a.MANUFACTURER_TYPE,a.dept ),
 ex22 as(
SELECT dept,MANUFACTURER_TYPE, ROW_NUMBER() over( ORDER BY rate desc) num  FROM ex11
) 

select a.* ,
(case 
WHEN a.PASS_QTY+a.BAD_QTY<=0 then 0
ELSE  ROUND((a.PASS_QTY / (a.PASS_QTY+a.BAD_QTY))*100,1)
end) as hgrate,
f.MANUFACTURER_TYPE,
b.INSPECTION_NAME,round(b.bad_qty/b.totla_bad_qty*100,2) bad_item_rate,b.ranking ranking_1,
c.INSPECTION_NAME,round(c.bad_qty/c.totla_bad_qty*100,2) bad_item_rate,c.ranking ranking_2,
d.INSPECTION_NAME,round(d.bad_qty/d.totla_bad_qty*100,2) bad_item_rate,d.ranking ranking_3,
(case 
WHEN a.PASS_QTY+a.BAD_QTY<=0 then 0
ELSE  ROUND((a.PASS_QTY / (a.PASS_QTY+a.BAD_QTY))*100,1)
end) as hgrate
from ex_data a
left join total_bad_data b on a.dept=b.dept and b.ranking=1
left join total_bad_data c on a.dept=c.dept and c.ranking=2
left join total_bad_data d on a.dept=d.dept and d.ranking=3
LEFT JOIN ex22 f on a.dept=f.dept
 where  a.test_count >0
ORDER BY 
case 
WHEN a.PASS_QTY+a.BAD_QTY<=0 then 0
ELSE  (a.PASS_QTY / (a.PASS_QTY+a.BAD_QTY))*100
end  {Type}, a.SEND_TEST_COUNT {Type}
 ";



                var dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();


                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 前端Q品质看板-实验室测试_产品明细 前五名 / 后五名
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQLaboratoryCP(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "1";//排序类型  0-倒序 1- 正序

                string ART_NO = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//查询条件 
                string ORDER_PO = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//查询条件 
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//查询条件  厂商
                string manufacturer_type = jarr.ContainsKey("manufacturer_type") ? jarr["manufacturer_type"].ToString() : "";//查询条件  厂商类型
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//查询条件 部位
                string MAKINGS_ID = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//查询条件 料号

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//查询条件

                string Where = string.Empty;

                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";

                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(ORDER_PO))
                {
                    //Where += $@"and a.ORDER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    Where += $@"and a.ORDER_PO like '%{ORDER_PO}%'";
                }
                if (!string.IsNullOrEmpty(manufacturer_name))
                {
                    arr = manufacturer_name.Split(',');
                    Where += $@"and ( a.manufacturer_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or e.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))})   )";

                    //Where += $@"and a.manufacturer_name like '%{manufacturer_name}%' ";
                }
                if (!string.IsNullOrEmpty(manufacturer_type))
                {
                    arr = manufacturer_type.Split(',');
                    Where += $@"and ( c.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))}) or d.FACTORY_SAP in ({string.Join(',', arr.Select(x => $"'{x}'"))})  )";

                    //Where += $@"and b.M_TYPE like '%{manufacturer_type}%' ";
                }
                if (!string.IsNullOrEmpty(parts_name))
                {
                    arr = parts_name.Split(',');
                    Where += $@"and a.POSITION_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.parts_name like '%{parts_name}%' ";
                }
                if (!string.IsNullOrEmpty(MAKINGS_ID))
                {
                    arr = MAKINGS_ID.Split(',');
                    Where += $@"and a.MAKINGS_ID in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.MAKINGS_ID like '%{MAKINGS_ID}%'";
                }

                if (Type.Trim() == "0")
                {
                    Type = "desc";
                }
                else
                    Type = "asc";
                #region old
                /*
                string sql = $@"
with rcpt_insp_data as (
	select a.CHK_NO, a.CHK_SEQ,a.ITEM_NO,c.ART_NO ,d.NAME_T 
	from wms_rcpt_d a
	join qcm_iqc_insp_res_m b on a.CHK_NO =b.CHK_NO and a.CHK_SEQ =b.CHK_SEQ and a.ITEM_NO =b.ITEM_NO 
	join bdm_purchase_order_d c on a.SOURCE_NO = c.ORDER_NO and a.SOURCE_SEQ = c.ORDER_SEQ
	join bdm_rd_prod d on c.ART_NO =d.PROD_NO 
),
rcpt_data as (
	select a.ART_NO,a.NAME_T ,count(distinct b.chk_no) rcpt_count
	from rcpt_insp_data a
	join wms_rcpt_m b on a.CHK_NO =b.CHK_NO 
	where b.rcpt_date between to_date('{starttime}','yyyy-mm-dd') and to_date('{endtime}','yyyy-mm-dd')
	--写条件
	group by a.ART_NO,a.NAME_T
),
iqc_data as (
	select c.ART_NO,c.NAME_T,
	sum(a.pass_qty) pass_qty,
	sum(nvl(b.BAD_QTY,0)) bad_qty,
	count(distinct a.CHK_NO) insp_count
	from qcm_iqc_insp_res_m a
	left join qcm_iqc_insp_res_bad_report b on a.CHK_NO=b.CHK_NO and a.CHK_SEQ=b.CHK_SEQ and a.ITEM_NO=b.ITEM_NO
	join rcpt_insp_data c on a.CHK_NO =c.CHK_NO and a.CHK_SEQ =c.CHK_SEQ and a.ITEM_NO =c.ITEM_NO 
	where a.INSPECTIONDATE between '{starttime}' and '{endtime}'
	--写条件
	group by c.ART_NO,c.NAME_T
),
rqc_data as (
	select 
	a.PROD_NO  , b.NAME_T,
	sum(case when d.commit_type=0 then 1 else 0 end) pass_qty,
	sum(case when d.commit_type=1 then 1 else 0 end) bad_qty,
	count(d.id) insp_count
	from rqc_task_m a
	join bdm_rd_prod b on a.PROD_NO =b.PROD_NO 
	join rqc_task_detail_t d on a.TASK_NO =d.TASK_NO 
	where d.createdate between '{starttime}' and '{endtime}'
	--写条件
	group by a.PROD_NO,b.NAME_T
),
total_data_tmp as (
		select ART_NO,
	NAME_T,
	INSP_COUNT,
	sum(pass_qty) pass_qty,
	sum(bad_qty) bad_qty ,
	(
case 
when sum(a.pass_qty)+sum(nvl(a.BAD_QTY,0)) = 0 then 0 
else 	round(sum(a.pass_qty)/(sum(a.pass_qty)+sum(nvl(a.BAD_QTY,0)))*100,2)
end
) pass_rate,
	row_number() over(order by (case 
when sum(a.pass_qty)+sum(nvl(a.BAD_QTY,0)) = 0 then 0 
else 	sum(a.pass_qty)/(sum(a.pass_qty)+sum(nvl(a.BAD_QTY,0)))
end) desc) ranking
	from (
	select * from iqc_data
	union all
	select * from rqc_data
	) a 
	group by ART_NO,NAME_T,INSP_COUNT
),
total_data as (
	select * from (select * from total_data_tmp order by ranking asc ) where rownum<=5
	union all
	select * from (select * from total_data_tmp order by ranking desc ) where rownum<=5
),
iqc_bad_data as (
	select c.ART_NO,c.NAME_T ,b.BADPROBLEM_NAME ,count(a.id) bad_qty
	from qcm_iqc_insp_res_d a
	join qcm_iqc_badproblems_m b on a.BADPROBLEM_CODE =b.BADPROBLEM_CODE 
	join rcpt_insp_data c on a.CHK_NO =c.CHK_NO  and a.CHK_SEQ =c.CHK_SEQ and a.ITEM_NO =c.ITEM_NO 
	where a.determine=1
	and a.createdate between '{starttime}' and '{endtime}'
	group by c.ART_NO,c.NAME_T ,b.BADPROBLEM_NAME
),
rqc_bad_data as (
	select g.PROD_NO ,g.NAME_T  ,c.INSPECTION_NAME BADPROBLEM_NAME,count(c.id) bad_qty
	from rqc_task_detail_t a
	join rqc_task_detail_t_d b on a.TASK_NO =b.TASK_NO and a.COMMIT_INDEX =b.COMMIT_INDEX 
	join rqc_task_item_c c on b.TASK_NO =c.TASK_NO and b.UNION_ID =c.ID 
	join rqc_task_m d on a.TASK_NO =d.TASK_NO 
	join bdm_se_order_master e on d.MER_PO =e.MER_PO 
	join BDM_SE_ORDER_ITEM f on e.se_id=f.se_id
	join bdm_rd_prod g on f.PROD_NO =g.PROD_NO 
	where a.commit_type=1
	and a.CREATEDATE between '{starttime}' and '{endtime}'
	group by g.PROD_NO ,g.NAME_T,c.INSPECTION_NAME 
),
total_bad_data_tmp as (
	select art_no,name_t,BADPROBLEM_NAME,
	sum(bad_qty) bad_qty,
	ROW_NUMBER() over (partition by art_no order by sum(bad_qty) desc) ranking
	from (
		select * from iqc_bad_data
		union all
		select * from rqc_bad_data
	) a group by art_no,name_t,BADPROBLEM_NAME
),
total_bad_data as (
	select art_no,name_t,BADPROBLEM_NAME,bad_qty,ranking,sum(bad_qty) over(partition by art_no) totla_bad_qty from total_bad_data_tmp
),
ex22 as(
select * from (
SELECT ART_NO,ROW_NUMBER() over( ORDER BY total_data.pass_rate {Type}) num  FROM total_data

)yy where num<6
   
) 
select a.* ,e.rcpt_count,
b.BADPROBLEM_NAME,
(
case 
when b.totla_bad_qty = 0 then 0 
else round(b.bad_qty/b.totla_bad_qty*100,2)
end
) bad_item_rate,
b.ranking ranking_1,
c.BADPROBLEM_NAME,
(
case 
when c.totla_bad_qty = 0 then 0 
else round(c.bad_qty/c.totla_bad_qty*100,2)
end
) bad_item_rate,
c.ranking ranking_2,
d.BADPROBLEM_NAME,
(
case 
when d.totla_bad_qty = 0 then 0 
else round(d.bad_qty/d.totla_bad_qty*100,2)
end
) bad_item_rate,
d.ranking ranking_3
from total_data a
left join total_bad_data b on a.art_no=b.art_no and b.ranking=1
left join total_bad_data c on a.art_no=c.art_no and c.ranking=2
left join total_bad_data d on a.art_no=d.art_no and d.ranking=3
left join rcpt_data e on a.art_no=e.art_no
 JOIN ex22 f on f.art_no  = a.ART_NO

 ";
                */
                #endregion

                string sql = $@"
with ex_tmp1 as (
	select 
	b.PROD_NO ,b.NAME_T ,a.*,c.M_TYPE
	from qcm_ex_task_list_m a
	LEFT join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
    LEFT join base003m c on c.SUPPLIERS_CODE = a.manufacturer_code

-- LEFT JOIN HR001M c on a.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on a.STAFF_DEPARTMENT_CODE = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
    LEFT JOIN BASE001M f on d.FACTORY_SAP = f.ORG_CODE
	where a.createdate between '{starttime}' and '{endtime}'
{Where}
),
ex_data as (
SELECT * from (
SELECT 
ROWNUM as RN,
(
case when pass_qty+bad_qty = 0 then 0
else ROUND(  pass_qty/(pass_qty+bad_qty )*100 ,1)
end
) as PASS_RATE,
tt.*
from (
	select 
	a.PROD_NO  as ART_NO,
	a.NAME_T,
	count(a.id) RCPT_COUNT,
    MAX(a.M_TYPE) as manufacturer_type,
	sum(case when nvl(a.TEST_RESULT,'NULL')!='NULL' then 1 else 0 end) test_count,
	sum(case when a.TEST_RESULT='PASS' then to_number(a.SEND_TEST_QTY) else 0 end) pass_qty,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from ex_tmp1 a
	group by a.PROD_NO,a.NAME_T

)tt
ORDER BY
(
case when pass_qty+bad_qty = 0 then 0
else ROUND(  pass_qty/(pass_qty+bad_qty )*100 ,1)
end
) {Type}

)tab
)

,
ex_bad_data_tmp1 as (
	select 
	c.PROD_NO as ART_NO
,c.NAME_T ,
	b.INSPECTION_NAME,
	b.id
	from qcm_ex_task_list_d b
	join qcm_ex_task_list_m a on a.TASK_NO =b.TASK_NO 
	join bdm_rd_prod c on a.ART_NO =c.PROD_NO 
  join base003m c on c.SUPPLIERS_CODE = a.manufacturer_code

-- LEFT JOIN HR001M c on a.STAFF_NO = c.STAFF_NO
	LEFT JOIN BASE005M d on a.STAFF_DEPARTMENT_CODE = d.DEPARTMENT_CODE
    LEFT JOIN SJQDMS_ORGINFO e on d.UDF05 = e.CODE
    LEFT JOIN BASE001M f on d.FACTORY_SAP = f.ORG_CODE
	where a.CREATEDATE  between '{starttime}' and '{endtime}'
{Where}
	and b.ITEM_TEST_RESULT ='FAIL'
),
ex_bad_data_tmp2 as (
	select 
	ART_NO,
	NAME_T,
	INSPECTION_NAME ,
	count(id) bad_qty,
	ROW_NUMBER() over (partition by ART_NO order by count(id) desc) ranking
	from ex_bad_data_tmp1
	group by ART_NO,NAME_T,INSPECTION_NAME 
),
total_bad_data as (
	select ART_NO,NAME_T,INSPECTION_NAME,bad_qty,ranking,sum(bad_qty) over(partition by ART_NO) totla_bad_qty from ex_bad_data_tmp2
)

select a.* ,
b.INSPECTION_NAME as BADPROBLEM_NAME,round(b.bad_qty/b.totla_bad_qty*100,1) bad_item_rate,b.ranking ranking_1,
c.INSPECTION_NAME as BADPROBLEM_NAME,round(c.bad_qty/c.totla_bad_qty*100,1) bad_item_rate,c.ranking ranking_2,
d.INSPECTION_NAME as BADPROBLEM_NAME,round(d.bad_qty/d.totla_bad_qty*100,1) bad_item_rate,d.ranking ranking_3
from ex_data a
 left join total_bad_data b on a.ART_NO=b.ART_NO and b.ranking=1
 left join total_bad_data c on a.ART_NO=c.ART_NO and c.ranking=2
  left join total_bad_data d on a.ART_NO=d.ART_NO and d.ranking=3
where a.test_count >0
ORDER BY a.PASS_RATE {Type},a.RCPT_COUNT {Type}
";


                var dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();


                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        #region AQL验货订单看板 A-01信息

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetYHData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//查询条件  0-年度 1日期
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件  Category
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件  shoe_no
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string ORDER_PO = jarr.ContainsKey("ORDER_PO") ? jarr["ORDER_PO"].ToString() : "";//查询条件 

                string DESCOUNTRY_NAME = jarr.ContainsKey("DESCOUNTRY_NAME") ? jarr["DESCOUNTRY_NAME"].ToString() : "";//查询条件  出货国家
                string INSPECTION_STATE = jarr.ContainsKey("INSPECTION_STATE") ? jarr["INSPECTION_STATE"].ToString() : "";//查询条件  验货状态


                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                //string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件  MER_PO


                string OUT_STATE = jarr.ContainsKey("OUT_STATE") ? jarr["OUT_STATE"].ToString() : "";//出货状态
                string SHELF_NO = jarr.ContainsKey("SHELF_NO") ? jarr["SHELF_NO"].ToString() : "";//储位
                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区
                //string ORG_CODE = jarr.ContainsKey("ORG_CODE") ? jarr["ORG_CODE"].ToString() : "";//厂区
                string WAREHOUSE = jarr.ContainsKey("WAREHOUSE") ? jarr["WAREHOUSE"].ToString() : "";//成品仓

                string[] orderListshoeName = SHOE_NO.Split(',');
                string[] orderListcategory = NAME_T.Split(','); //Category
                string[] orderListart = ART_NO.Split(',');
                string[] orderListPo = ORDER_PO.Split(',');
                string[] orderListgj = DESCOUNTRY_NAME.Split(',');//出货国家
                string[] orderListSheif = SHELF_NO.Split(',');  //储位
                string[] orderListorg = ORG.Split(',');         //厂区
                string[] orderListware = WAREHOUSE.Split(',');  //成品仓
                //【出货状态、储位、厂区、成品仓、】
                //c.SE_QTY
                string Where = string.Empty;
                string Where1 = string.Empty;
                if (OUT_STATE == "0")
                {
                    Where += $@" and o.qty > c.SE_QTY";//已发货
                }
                else if (OUT_STATE == "2")
                {
                    Where += $@" and i.qty<c.SE_QTY";//生产中
                }
                else if (OUT_STATE == "3")
                {
                    Where += $@" and i.qty >= c.se_qty and o.qty >0 and o.qty < c.se_qty";//部分出货
                }
                else if (OUT_STATE == "1")
                {
                    Where += $@" and i.qty >= c.se_qty";//备库存
                }

                //Category
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@" and f.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    Where += $@" and e.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
                }

                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@" and c.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ORDER_PO))
                {
                    Where += $@" AND  a.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
                }

                if (!string.IsNullOrEmpty(DESCOUNTRY_NAME))
                {
                    Where += $@" and a.DESCOUNTRY_NAME in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(INSPECTION_STATE))
                {
                    if (INSPECTION_STATE == "0")
                        Where1 += $@" AND g.ID IS null";
                    else
                        Where1 += $@" and g.INSPECTION_STATE = '{INSPECTION_STATE}'";
                }
                if (!string.IsNullOrEmpty(SHELF_NO))
                {
                    Where += $@" and s.LOCATION_NAME in ({string.Join(',', orderListSheif.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(WAREHOUSE))
                {
                    Where += $@" and s.WAREHOUSE_NAME in ({string.Join(',', orderListware.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ORG))
                {
                    Where += $@" and sj.en in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))})";
                }

                DateTime start = new DateTime();
                DateTime end = new DateTime();

                //转换为 yyyy-MM-01 ||yyyy-MM-30 
                Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);

                DateTime lastyear = DateTime.Now.AddYears(-1);
                //string starttime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-01");
                //string endtime = DateTime.Now.ToString($"yyyy-MM-{dd}");
                //DateTime start = Convert.ToDateTime(starttime);
                //DateTime end = Convert.ToDateTime(endtime);
                string starttime2;
                string endtime2;
                if (TYPE == "0")
                {
                    //format_date = "to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')";
                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                    starttime2 = start.ToString("yyyy-MM-dd");
                    endtime2 = end.ToString("yyyy-MM-dd");
                }
                else
                {
                    //format_date = "a.CREATEDATE";

                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                    starttime2 = start.ToString("yyyy-MM-dd");
                    endtime2 = end.ToString("yyyy-MM-dd");
                }

                int months = end.Month - start.Month + 1;

                string sql_content = string.Empty;//报告分部列表
                string sql_content1 = "select * from hold_report";//报告分部列表
                string sql_content2 = "select * from not_hold_art";//ART欠缺A01报告占比
                //string sql_content3 = $@"select * from not_hold_detail";//欠缺A01报告的订单列表
                string sql_content3 = $@"SELECT tab.*,
(
case 
when tab.qtyout >tab.se_qty then 'Shipped'
when tab.qtyin<tab.se_qty then 'In_Production'
when tab.qtyin>=tab.se_qty and qtyout>0 and qtyout<se_qty then 'Partial_Shipment'
when tab.qtyin>= tab.se_qty then'Stock'
else ''
end
) as  OUTSTATUS
 from (
select a.*,nvl(b.qty,0) as qtyout,nvl(c.qty,0) as qtyin from not_hold_detail a
 LEFT JOIN outdata b on a.se_id = b.se_id
left JOIN indata c on a.se_id = c.se_id
)tab";//欠缺A01报告的订单列表
                //int months = ts.mon;
                if (months <= 0)
                {
                    throw new Exception("Please select the correct date range！");
                }
                if (months > 12)
                {
                    throw new Exception("The number of months selected cannot be greater than 12 months！");
                }
                //sql_content = sql_content1;
                string sql = GetSql(sql_content1, Where, Where1, starttime2, endtime2);


                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "配备A-01报告的PO单个数", "不配备A-01报告的PO单个数", "A-01报告持有率" });

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = lanDic["配备A-01报告的PO单个数"].ToString();

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = lanDic["不配备A-01报告的PO单个数"].ToString();

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = lanDic["A-01报告持有率"].ToString();

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                //遍历N个月
                for (int i = 0; i < months; i++)
                {
                    Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"INSERT_MONTH = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["HOLD_A01_QTY"].ToString());
                        list_fail.Add(dr[0]["NOT_HOLD_A01_QTY"].ToString());
                        list_rate.Add(dr[0]["RATE"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);



                //占比
                sql = GetSql(sql_content2, Where, Where1, starttime, endtime);
                DataTable dt2 = DB.GetDataTable(sql);

                //欠缺A-01报告列表
                sql = GetSql(sql_content3, Where, Where1, starttime, endtime);
                DataTable dt3 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);// X轴
                dic.Add("Data1", res); // 报告分布
                dic.Add("Data2", dt2); // 占比
                dic.Add("Data3", dt3); // 欠缺A-01

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        public static string GetSql(string sql_content, string Where, string Where1, string starttime, string endtime)
        {
            /*
             AO1报告率：有A-01报告的PO单数÷（有A-01报告的PO单数+没有A-01报告的PO单数）
             */
            /*
            string sql = $@"
                                    with outdata as (
                                    SELECT
											M.SE_ID,
											nvl( sum( shipping_qty ), 0 ) AS qty 
										FROM
											bmd_se_shipment_m m
											LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
										WHERE
											d.factory_no = '1001' 
											AND d.warehourse_no = '2000' 
										GROUP BY
											M.SE_ID 
										),
										InData AS (
										SELECT
											se_id,
											nvl( sum( qty ), 0 ) AS qty 
										FROM
											mms_finishedtrackin_list 
										WHERE
											org_id = '1001' 
											AND stoc_no = '2000' 
										GROUP BY
											se_id 
),
po_data as (
	select a.*,b.NLT from bdm_se_order_master a
    LEFT JOIN BDM_SE_ORDER_ITEM b on a.SE_ID = b.SE_ID
	where NLT between to_date('{starttime}','yyyy-mm-dd') and to_date('{endtime}','yyyy-mm-dd')
),
a01_data as (
	select substr(m1.file_name,0,instr(m1.file_name,'&',1,1)-1) as prod_no,
	substr(m1.file_name,(INSTR(m1.file_name,'&',1,1))+1,(INSTR(m1.file_name,'&',1,2) - INSTR(m1.file_name,'&',1,1)-1)) as mer_po,
	m1.*
	from aql_app_t_file_m m1  
),
loactiondata as (SELECT BATCH_NO,shelf_no FROM WMS_STOC_LOCATION GROUP BY BATCH_NO,shelf_no),
main_data as (
	select 
    c.se_id,
    z.shelf_no as STORAGE,
	to_char(a.INSERT_DATE,'yyyy-mm') insert_month,	--月份
	a.MER_PO ,	--PO
	case when b.id is not null then 1 else 0 end hold_a01,	--有A01
	case when b.id is null then 1 else 0 end not_hold_a01,	--无A01
	d.PROD_NO ,	--编号
	d.NAME_T ,	--名称
	to_char(c.nlt,'yyyy-MM-dd') as nlt,	--交期
	c.SE_QTY,	--订单数量
	a.DESCOUNTRY_CODE,	--出货国家
	a.DESCOUNTRY_NAME,	--出货国家
	f.NAME_T Category,
	case when g.ID is null then '0' else g.INSPECTION_STATE end INSPECTION_STATE	--0：未验货 1：已验货（AQL录入功能有数据，就为已验货）
	from po_data a
	left join a01_data b on a.MER_PO =b.mer_po
	join BDM_SE_ORDER_ITEM c on a.se_id=c.se_id
	join bdm_rd_prod d on c.PROD_NO =d.PROD_NO 
	JOIN bdm_rd_style e ON d .shoe_no = e.shoe_no
    left join loactiondata z on z.BATCH_NO = a.se_id
    JOIN MMS_WAREHOUSE_SHELF_MANAGE s ON s.LOCATION_CODE = z.SHELF_NO   -- 储位基础表 2023.05.15
	JOIN bdm_cd_code f ON e .style_seq = f.code_no and f.language = '1'	-- 这个bdm_cd_code表里面有相同code_no记录，但是language不一样
	left join aql_cma_task_list_m g on a.MER_PO =g.PO 
     LEFT JOIN outdata o on c.se_id = o.se_id
    left JOIN indata i on c.se_id = i.se_id
    WHERE 1 = 1 {Where}
),
hold_report as (
	--报告分部列表
	select insert_month,sum(hold_a01) hold_a01_qty,sum(not_hold_a01) not_hold_a01_qty,
(
case 
when sum(not_hold_a01)+sum(hold_a01) = 0 then 0
 -- else  (1-(ROUND(sum(hold_a01) / (sum(not_hold_a01)+sum(hold_a01)),1)))*100
else   ROUND(sum(hold_a01) / (sum(not_hold_a01)+sum(hold_a01)),4)*100
end
) as RATE
	from main_data
	group by insert_month
),
not_hold_art_tmp as (
	select prod_no,name_t ,sum(not_hold_a01) not_hold_qty,(select sum(not_hold_a01) from main_data) not_hold_total_qty
	from main_data
	group by prod_no,name_t
),
not_hold_art as (
	--ART欠缺A01报告占比
	select PROD_NO,NAME_T,not_hold_qty,not_hold_total_qty,case when not_hold_total_qty = 0 then 0 else round(not_hold_qty/(not_hold_total_qty+not_hold_qty)*100,1) end not_hold_rate,ROW_NUMBER () over(order by not_hold_qty desc) ranking
	from not_hold_art_tmp
),
not_hold_detail as (
	--欠缺A01报告的订单列表
	select  se_id,'已生效' as not_hold_a01,MER_PO,PROD_NO,NAME_T,NLT,SE_QTY,DESCOUNTRY_CODE,DESCOUNTRY_NAME,CATEGORY,STORAGE,(case when INSPECTION_STATE = 0  then '未验货' when INSPECTION_STATE = 1 then '已验货' else '' end ) as INSPECTION_STATE
	from main_data
)
{sql_content}";
            */

            string sql = $@"
                                    with outdata as (
                                    SELECT
											M.SE_ID,
											nvl( sum( shipping_qty ), 0 ) AS qty 
										FROM
											bmd_se_shipment_m m
											LEFT JOIN bmd_se_shipment_d d ON m.shipping_no = d.shipping_no 
										WHERE
											d.factory_no = '5001' 
											AND d.warehourse_no = '2000' 
										GROUP BY
											M.SE_ID 
										),
										InData AS (
										SELECT
											se_id,
											nvl( sum( qty ), 0 ) AS qty 
										FROM
											mms_finishedtrackin_list 
										WHERE
											org_id = '5001' 
											AND stoc_no = '2000' 
										GROUP BY
											se_id 
),
po_data as (
	select a.*,b.NLT from bdm_se_order_master a
    LEFT JOIN BDM_SE_ORDER_ITEM b on a.SE_ID = b.SE_ID
	where NLT between to_date('{starttime}','yyyy-mm-dd') and to_date('{endtime}','yyyy-mm-dd')
),
a01_data as (
	select substr(m1.file_name,0,instr(m1.file_name,'&',1,1)-1) as prod_no,
	substr(m1.file_name,(INSTR(m1.file_name,'&',1,1))+1,(INSTR(m1.file_name,'&',1,2) - INSTR(m1.file_name,'&',1,1)-1)) as mer_po,
	m1.*
	from aql_app_t_file_m m1  
),
loactiondata as (SELECT BATCH_NO,shelf_no FROM WMS_STOC_LOCATION GROUP BY BATCH_NO,shelf_no),
main_data1 AS (
		SELECT
		    c.se_id,
		    z.shelf_no AS STORAGE,
		    to_char( a.INSERT_DATE, 'yyyy-mm' ) insert_month,--月份
		    a.MER_PO,--PO
		    d.PROD_NO,--编号
		    e.NAME_T,--名称
		    to_char( c.nlt, 'yyyy-MM-dd' ) AS nlt,--交期
		    c.SE_QTY,--订单数量
		    a.DESCOUNTRY_CODE,--出货国家
            x.c_name as DESCOUNTRY_NAME,
		    --a.DESCOUNTRY_NAME,--出货国家
		    f.NAME_T Category
        FROM
	        po_data a
            join bdm_country x on a.DESCOUNTRY_CODE = x.c_no and x.l_no='EN' 
	        JOIN BDM_SE_ORDER_ITEM c ON a.se_id = c.se_id
	        JOIN bdm_rd_prod d ON c.PROD_NO = d.PROD_NO
	        JOIN bdm_rd_style e ON d.shoe_no = e.shoe_no
	        LEFT JOIN loactiondata z ON z.BATCH_NO = a.se_id
	        JOIN MMS_WAREHOUSE_SHELF_MANAGE s ON s.LOCATION_CODE = z.SHELF_NO -- 储位基础表 2023.05.15
            left JOIN BASE005M m5 ON s.ORG_ID = m5.FACTORY_SAP
			left JOIN SJQDMS_ORGINFO sj ON m5.UDF05 = sj.code
	        JOIN bdm_cd_code f ON e.style_seq = f.code_no 
	        AND f.language = 'E' -- 这个bdm_cd_code表里面有相同code_no记录，但是language不一样
	        LEFT JOIN outdata o ON c.se_id = o.se_id
	        LEFT JOIN indata i ON c.se_id = i.se_id 
        WHERE
	        1 = 1 {Where}
	        GROUP BY 
		        to_char( a.INSERT_DATE, 'yyyy-mm' ),
		        c.se_id,
		        z.shelf_no,
		        a.MER_PO,
		        d.PROD_NO,
		        e.NAME_T,
		        to_char( c.nlt, 'yyyy-MM-dd' ),
		        c.SE_QTY,
		        a.DESCOUNTRY_CODE,
                x.c_name,
		        --a.DESCOUNTRY_NAME,
		        f.NAME_T
	),
	main_data AS (
		SELECT 
			a.*,
			CASE
				WHEN b.id IS NOT NULL THEN
				1 ELSE 0 
			END hold_a01,--有A01
			CASE
				WHEN b.id IS NULL THEN
			1 ELSE 0 
			END not_hold_a01,--无A01
			CASE
				WHEN g.ID IS NULL THEN
				'0' ELSE g.INSPECTION_STATE 
			END INSPECTION_STATE --0：未验货 1：已验货（AQL录入功能有数据，就为已验货）
		
		FROM main_data1 a
		LEFT JOIN a01_data b ON a.MER_PO = b.mer_po
		JOIN BDM_SE_ORDER_ITEM c ON a.se_id = c.se_id
		LEFT JOIN aql_cma_task_list_m g ON a.MER_PO = g.PO
		WHERE 1=1 {Where1}
	),
hold_report as (
	--报告分部列表
	select insert_month,sum(hold_a01) hold_a01_qty,sum(not_hold_a01) not_hold_a01_qty,
(
case 
when sum(not_hold_a01)+sum(hold_a01) = 0 then 0
 -- else  (1-(ROUND(sum(hold_a01) / (sum(not_hold_a01)+sum(hold_a01)),1)))*100
else   ROUND(sum(hold_a01) / (sum(not_hold_a01)+sum(hold_a01)),4)*100
end
) as RATE
	from main_data
	group by insert_month
),
not_hold_art_tmp as (
	select prod_no,name_t ,sum(not_hold_a01) not_hold_qty,(select sum(not_hold_a01) from main_data) not_hold_total_qty
	from main_data
	group by prod_no,name_t
),
not_hold_art as (
	--ART欠缺A01报告占比
	select PROD_NO,NAME_T,not_hold_qty,not_hold_total_qty,case when not_hold_total_qty = 0 then 0 else round(not_hold_qty/(not_hold_total_qty+not_hold_qty)*100,1) end not_hold_rate,ROW_NUMBER () over(order by not_hold_qty desc) ranking
	from not_hold_art_tmp
),
not_hold_detail as (
	--欠缺A01报告的订单列表
	select  se_id,'In_Force' as not_hold_a01,MER_PO,PROD_NO,NAME_T,NLT,SE_QTY,DESCOUNTRY_CODE,DESCOUNTRY_NAME,CATEGORY,STORAGE,(case when INSPECTION_STATE = 0  then 'Not_Inspected' when INSPECTION_STATE = 1 then 'Checked' else '' end ) as INSPECTION_STATE
	from main_data
)
{sql_content}";
            //( case when not_hold_a01 = 1 then '' else'已失效' end) 
            //-- where not_hold_a01=1

            return sql;
        }
        #endregion

        # region APE_Code
//        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetYHData(object OBJ)
//        {
//            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
//            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

//            string Data = string.Empty;

//            string guid = string.Empty;
//            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
//            try
//            {
//                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

//                Data = ReqObj.Data.ToString();

//                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

//                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
//                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//查询条件  0-年度 1日期
//                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件  Category
//                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件  shoe_no
//                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
//                string ORDER_PO = jarr.ContainsKey("ORDER_PO") ? jarr["ORDER_PO"].ToString() : "";//查询条件 

//                string DESCOUNTRY_NAME = jarr.ContainsKey("DESCOUNTRY_NAME") ? jarr["DESCOUNTRY_NAME"].ToString() : "";//查询条件  出货国家
//                string INSPECTION_STATE = jarr.ContainsKey("INSPECTION_STATE") ? jarr["INSPECTION_STATE"].ToString() : "";//查询条件  验货状态


//                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
//                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
//                //string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件  MER_PO


//                string OUT_STATE = jarr.ContainsKey("OUT_STATE") ? jarr["OUT_STATE"].ToString() : "";//出货状态
//                string AQL_RESLT = jarr.ContainsKey("AQL_RESLT") ? jarr["AQL_RESLT"].ToString() : "";//验货结果
//                string SHELF_NO = jarr.ContainsKey("SHELF_NO") ? jarr["SHELF_NO"].ToString() : "";//储位
//                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区
//                //string ORG_CODE = jarr.ContainsKey("ORG_CODE") ? jarr["ORG_CODE"].ToString() : "";//厂区
//                string WAREHOUSE = jarr.ContainsKey("WAREHOUSE") ? jarr["WAREHOUSE"].ToString() : "";//成品仓

//                string[] orderListshoeName = SHOE_NO.Split(',');
//                string[] orderListcategory = NAME_T.Split(','); //Category
//                string[] orderListart = ART_NO.Split(',');
//                string[] orderListPo = ORDER_PO.Split(',');
//                string[] orderListgj = DESCOUNTRY_NAME.Split(',');//出货国家
//                string[] orderListSheif = SHELF_NO.Split(',');  //储位
//                string[] orderListorg = ORG.Split(',');         //厂区
//                string[] orderListware = WAREHOUSE.Split(',');  //成品仓
//                //厂区的时候进行特殊处理，需要将工厂代号查询出来
//                if (!string.IsNullOrEmpty(ORG))
//                {
//                    string ORG_SQL = $@"select distinct a.FACTORY_SAP from base005m a inner join SJQDMS_ORGINFO b on a.UDF05 = b.CODE and b.ORG in({string.Join(',', orderListorg.Select(x => $"'{x}'"))})";
//                    DataTable dataTable = DB.GetDataTable(ORG_SQL);
//                    orderListorg = new string[dataTable.Rows.Count];
//                    for (int i = 0; i < dataTable.Rows.Count; i++)
//                    {
//                        orderListorg[i] = dataTable.Rows[i]["FACTORY_SAP"].ToString();
//                    }
//                }
//                //【出货状态、储位、厂区、成品仓、】
//                //c.SE_QTY
//                string Where = string.Empty;
//                string Where1 = string.Empty;
//                if (OUT_STATE == "0")
//                {
//                    Where += $@" and PG_AEQS.GET_FINISHED_STATUS(a.se_id)='已发货'";//已发货
//                }
//                else if (OUT_STATE == "2")
//                {
//                    Where += $@"  and PG_AEQS.GET_FINISHED_STATUS(a.se_id)='生产中'";//生产中
//                }
//                else if (OUT_STATE == "3")
//                {
//                    Where += $@" and PG_AEQS.GET_FINISHED_STATUS(a.se_id)='部分出货'";//部分出货
//                }
//                else if (OUT_STATE == "1")
//                {
//                    Where += $@" and PG_AEQS.GET_FINISHED_STATUS(a.se_id)='备库存'";//备库存
//                }

//                //Category
//                if (!string.IsNullOrEmpty(NAME_T))
//                {
//                    Where += $@" and e.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
//                }
//                if (!string.IsNullOrEmpty(SHOE_NO))
//                {
//                    Where += $@" and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))})";
//                }

//                if (!string.IsNullOrEmpty(ART_NO))
//                {
//                    Where += $@" and b.PROD_NO in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
//                }
//                if (!string.IsNullOrEmpty(ORDER_PO))
//                {
//                    Where += $@" AND  a.MER_PO in ({string.Join(',', orderListPo.Select(x => $"'{x}'"))})";
//                }

//                if (!string.IsNullOrEmpty(DESCOUNTRY_NAME))
//                {
//                    Where += $@" and a.DESCOUNTRY_NAME in ({string.Join(',', orderListgj.Select(x => $"'{x}'"))})";
//                }
//                if (!string.IsNullOrEmpty(INSPECTION_STATE))
//                {
//                    Where += $@" and c.AQL_EDIT_STATE = '{INSPECTION_STATE}'";
//                }
//                if (!string.IsNullOrEmpty(AQL_RESLT))
//                {
//                    Where += $@" and c.AQL_RESLT = '{AQL_RESLT}'";
//                }
//                if (!string.IsNullOrEmpty(SHELF_NO))
//                {
//                    Where += $@" and f.SHELF_NO in ({string.Join(',', orderListSheif.Select(x => $"'{x}'"))})";
//                }
//                if (!string.IsNullOrEmpty(WAREHOUSE))
//                {
//                    Where += $@" and s.WAREHOUSE_NAME in ({string.Join(',', orderListware.Select(x => $"'{x}'"))})";
//                }
//                if (!string.IsNullOrEmpty(ORG))
//                {
//                    Where += $@" and f.ORG_ID in ({string.Join(',', orderListorg.Select(x => $"'{x}'"))})";
//                }

//                DateTime start = new DateTime();
//                DateTime end = new DateTime();

//                //转换为 yyyy-MM-01 ||yyyy-MM-30 
//                Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);

//                DateTime lastyear = DateTime.Now.AddYears(-1);

//                if (TYPE == "0")
//                {
//                    //format_date = "to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')";
//                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
//                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
//                }
//                else
//                {
//                    //format_date = "a.CREATEDATE";

//                    start = DateTime.Parse(starttime);
//                    end = DateTime.Parse(endtime);
//                }
//                starttime = start.ToString("yyyy-MM-dd");
//                endtime = end.ToString("yyyy-MM-dd");
//                int months = end.Month - start.Month + 1;

//                string sql_content = string.Empty;//报告分部列表
//                string sql_content1 = "select * from hold_report";//报告分部列表
//                sql_content1 = $@"with tb1 as
// (select distinct ORG_ID, STOC_NO, BATCH_NO, SHELF_NO
//    from wms_stoc_location
//   where stoc_no = '2000'
//     and STOC_QTY > 0),
//tb2 as
// (select PROD_NO,
//         MER_PO,
//         CURR_UPLOAD_TIME,
//         ADD_MONTHS(CURR_UPLOAD_TIME, 13) as futureTime,
//         FILE_STARTTIME
//    from (select t.*,
//                 row_number() over(partition by mer_po order by id desc) rn
//            from (select substr(m1.file_name,
//                                0,
//                                instr(m1.file_name, '&', 1, 1) - 1) as prod_no,
//                         substr(m1.file_name,
//                                (INSTR(m1.file_name, '&', 1, 1)) + 1,
//                                (INSTR(m1.file_name, '&', 1, 2) -
//                                INSTR(m1.file_name, '&', 1, 1) - 1)) as mer_po,
//                         m1.*
//                    from aql_app_t_file_m m1) t)
//   where rn = '1')
//   select YH,sum(case when ASTATUS='已生效' then 1 else 0 end ) as HOLD_A01_QTY,sum(case when ASTATUS='已失效' then 1 else 0 end ) NOT_HOLD_A01_QTY,decode(count(*),0,0,round(sum(case when ASTATUS='已生效' then 1 else 0 end )/count(*),2))*100 RATE from (
//select distinct to_char(b.nlt, 'yyyy-MM') as YH,
//                a.mer_po,
//                (case when to_char(g.futureTime, 'yyyyMMdd') >
//                         to_char(sysdate, 'yyyyMMdd') then '已生效' else
//                         '已失效' end) as ASTATUS              
//  from bdm_se_order_master a
//  left join bdm_se_order_item b
//    on a.se_id = b.se_id
//  left join aql_cma_task_list_m c
//    on a.mer_po = c.po
//  left join bdm_rd_style d
//    on b.SHOE_NO = d.SHOE_NO
//  left join bdm_cd_code e
//    on d.style_seq = e.code_no
//   and e.LANGUAGE = '1'
//  left join tb1 f
//    on f.batch_no = a.se_id
//  left join tb2 g
//    on g.MER_PO = a.mer_po
// where a.SE_TYPE in ('ZOR1', 'ZOR2')
//and b.NLT between to_date('{starttime}', 'yyyy-mm-dd') and to_date('{endtime}', 'yyyy-mm-dd') and  b.se_qty !=0 and 1=1
//{Where}
//)group by YH
// ";
//                //string sql_content2 = "select * from not_hold_art";//ART欠缺A01报告占比
//                //string sql_content3 = $@"select * from not_hold_detail";//欠缺A01报告的订单列表
//                //饼图查询
//                string sql_content2 = $@"with tb1 as
// (select distinct ORG_ID, STOC_NO, BATCH_NO, SHELF_NO
//    from wms_stoc_location
//   where stoc_no = '2000'
//     and STOC_QTY > 0),
//tb2 as
// (select PROD_NO,
//         MER_PO,
//         CURR_UPLOAD_TIME,
//         ADD_MONTHS(CURR_UPLOAD_TIME, 13) as futureTime,
//         FILE_STARTTIME
//    from (select t.*,
//                 row_number() over(partition by mer_po order by id desc) rn
//            from (select substr(m1.file_name,
//                                0,
//                                instr(m1.file_name, '&', 1, 1) - 1) as prod_no,
//                         substr(m1.file_name,
//                                (INSTR(m1.file_name, '&', 1, 1)) + 1,
//                                (INSTR(m1.file_name, '&', 1, 2) -
//                                INSTR(m1.file_name, '&', 1, 1) - 1)) as mer_po,
//                         m1.*
//                    from aql_app_t_file_m m1) t)
//   where rn = '1')
//select prod_no, num
//  from (select b.prod_no, count(distinct a.mer_po) as num
//          from bdm_se_order_master a
//          left join bdm_se_order_item b
//            on a.se_id = b.se_id
//          left join aql_cma_task_list_m c
//            on a.mer_po = c.po
//          left join bdm_rd_style d
//            on b.SHOE_NO = d.SHOE_NO
//          left join bdm_cd_code e
//            on d.style_seq = e.code_no
//           and e.LANGUAGE = '1'
//          left join tb1 f
//            on f.batch_no = a.se_id
//          left join tb2 g
//            on g.MER_PO = a.mer_po
//         where a.SE_TYPE in ('ZOR1', 'ZOR2')
//           and (case
//                 when to_char(g.futureTime, 'yyyyMMdd') >
//                      to_char(sysdate, 'yyyyMMdd') then
//                  '已生效'
//                 else
//                  '已失效'
//               end) = '已失效'
//           and  b.se_qty !=0 and 1 = 1
//         {Where}
//         and b.NLT between to_date('{starttime}', 'yyyy-mm-dd') and to_date('{endtime}', 'yyyy-mm-dd') 
//         group by b.prod_no
//         order by count(distinct a.mer_po) desc)
// where ROWNUM <= 10
//";//欠缺A01报告的订单列表

//                string sql_content3 = $@"";//欠缺A01报告的订单列表
//                /*
//                 SELECT tab.*,
//                (
//                case 
//                when tab.qtyout >tab.se_qty then '已发货'
//                when tab.qtyin<tab.se_qty then '生产中'
//                when tab.qtyin>=tab.se_qty and qtyout>0 and qtyout<se_qty then '部分出货'
//                when tab.qtyin>= tab.se_qty then'备库存'
//                else ''
//                end
//                ) as  OUTSTATUS
//                 from (
//                select a.*,nvl(b.qty,0) as qtyout,nvl(c.qty,0) as qtyin from not_hold_detail a
//                 LEFT JOIN outdata b on a.se_id = b.se_id
//                left JOIN indata c on a.se_id = c.se_id
//                )tab
//                 */

//                //int months = ts.mon;
//                if (months <= 0)
//                {
//                    throw new Exception("请选择正确日期范围！");
//                }
//                if (months > 12)
//                {
//                    throw new Exception("所选择月份数禁止大于12个月！");
//                }
//                //sql_content = sql_content1;
//                string sql = GetSql(sql_content1, Where, Where1, starttime, endtime);


//                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "配备A-01报告的PO单个数", "不配备A-01报告的PO单个数", "A-01报告持有率" });

//                List<string> Xdata = new List<string>();

//                List<KanBanDtos> res = new List<KanBanDtos>();

//                KanBanDtos data_pass = new KanBanDtos();
//                data_pass.type = "bar";
//                data_pass.name = lanDic["配备A-01报告的PO单个数"].ToString();

//                KanBanDtos data_fail = new KanBanDtos();
//                data_fail.type = "bar";
//                data_fail.name = lanDic["不配备A-01报告的PO单个数"].ToString();

//                KanBanDtos data_rate = new KanBanDtos();
//                data_rate.type = "line";
//                data_rate.name = lanDic["A-01报告持有率"].ToString();

//                List<string> list_pass = new List<string>();
//                List<string> list_fail = new List<string>();
//                List<string> list_rate = new List<string>();
//                var dt = DB.GetDataTable(sql_content1);//获取柱状图数据

//                //遍历N个月
//                for (int i = 0; i < months; i++)
//                {
//                    Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));
//                }

//                foreach (var item in Xdata)
//                {
//                    var dr = dt.Select($"YH = '{item}'");
//                    if (dr.Length > 0)
//                    {
//                        list_pass.Add(dr[0]["HOLD_A01_QTY"].ToString());
//                        list_fail.Add(dr[0]["NOT_HOLD_A01_QTY"].ToString());
//                        list_rate.Add(dr[0]["RATE"].ToString());

//                    }
//                    else
//                    {
//                        list_pass.Add("0");
//                        list_fail.Add("0");
//                        list_rate.Add("0");
//                    }
//                }
//                data_pass.data = list_pass;
//                data_fail.data = list_fail;
//                data_rate.data = list_rate;

//                res.Add(data_pass);
//                res.Add(data_fail);
//                res.Add(data_rate);



//                //占比
//                //sql = GetSql(sql_content2, Where, Where1, starttime, endtime);
//                DataTable dt2 = DB.GetDataTable(sql_content2);


//                //欠缺A-01报告列表
//                sql_content3 = $@"
//with tb1 as
// (select distinct ORG_ID, STOC_NO, BATCH_NO, SHELF_NO
//    from wms_stoc_location
//   where stoc_no = '2000'
//     and STOC_QTY > 0),
//tb2 as
// (
  
//  select PROD_NO,
//          MER_PO,
//          CURR_UPLOAD_TIME,
//          ADD_MONTHS(CURR_UPLOAD_TIME, 13) as futureTime,
//          FILE_STARTTIME
//    from (select t.*,
//                  row_number() over(partition by mer_po order by id desc) rn
//             from (select substr(m1.file_name,
//                                 0,
//                                 instr(m1.file_name, '&', 1, 1) - 1) as prod_no,
//                          substr(m1.file_name,
//                                 (INSTR(m1.file_name, '&', 1, 1)) + 1,
//                                 (INSTR(m1.file_name, '&', 1, 2) -
//                                 INSTR(m1.file_name, '&', 1, 1) - 1)) as mer_po,
//                          m1.*
//                     from aql_app_t_file_m m1) t)
//   where rn = '1')
//select distinct c.task_no,
//                e.NAME_T as CATEGORY,
//                d.shoe_no,
//                d.NAME_T,
//                b.prod_no,
//                a.mer_po,
//                a.DESCOUNTRY_CODE,
//                a.DESCOUNTRY_NAME,
//                decode(c.Aql_Edit_State,'0','未验货','1','已验货','') INSPECTION_STATE,
//                decode(c.AQL_RESLT,'0','Reject','1','Accepted','') AQL_RESLT,
//                PG_AEQS.GET_FINISHED_STATUS(a.se_id) as OUTSTATUS,
//                f.SHELF_NO STORAGE,
//                to_char(b.nlt,'yyyy-MM-dd') nlt,
//                b.se_qty,
//                (case
//                  when to_char(g.futureTime, 'yyyyMMdd') >
//                       to_char(sysdate, 'yyyyMMdd') then
//                   '已生效'
//                  else
//                   '已失效'
//                end) as NOT_HOLD_A01
//  from bdm_se_order_master a
//  left join bdm_se_order_item b
//    on a.se_id = b.se_id
//  left join aql_cma_task_list_m c
//    on a.mer_po = c.po
//  left join bdm_rd_style d
//    on b.SHOE_NO = d.SHOE_NO
//  left join bdm_cd_code e
//    on d.style_seq = e.code_no
//   and e.LANGUAGE = '1'
//  left join tb1 f
//    on f.batch_no = a.se_id
//  left join tb2 g
//    on g.MER_PO = a.mer_po
// where a.SE_TYPE in ('ZOR1', 'ZOR2')
//   and b.NLT between to_date('{starttime}', 'yyyy-mm-dd') and
//       to_date('{endtime}', 'yyyy-mm-dd')
//   and (case
//         when to_char(g.futureTime, 'yyyyMMdd') >
//              to_char(sysdate, 'yyyyMMdd') then
//          '已生效'
//         else
//          '已失效'
//       end) = '已失效' and b.se_qty !=0 and 1=1 {Where} order by nlt desc";
//                //sql = GetSql(sql_content3, Where, Where1, starttime, endtime);
//                DataTable dt3 = DB.GetDataTable(sql_content3);

//                Dictionary<string, object> dic = new Dictionary<string, object>();
//                dic.Add("Xdata", Xdata);// X轴
//                dic.Add("Data1", res); // 报告分布
//                dic.Add("Data2", dt2); // 占比
//                dic.Add("Data3", dt3); // 欠缺A-01

//                ret.IsSuccess = true;
//                ret.RetData1 = dic;


//            }
//            catch (Exception ex)
//            {
//                ret.IsSuccess = false;
//                ret.ErrMsg = "00000:" + ex.Message;
//            }
//            return ret;
//        }

        #endregion
        #region 测试部品质看板-FAIL3次测试异常

        /// <summary>
        /// fail次数汇总列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSDataByFail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string SEASON = jarr.ContainsKey("SEASON") ? jarr["SEASON"].ToString() : "";//查询条件  季度
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件  Category
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件  鞋型名称
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string PHASE_CREATION_NAME = jarr.ContainsKey("PHASE_CREATION_NAME") ? jarr["PHASE_CREATION_NAME"].ToString() : "";//查询条件  阶段
                string STAFF_DEPARTMENT = jarr.ContainsKey("STAFF_DEPARTMENT") ? jarr["STAFF_DEPARTMENT"].ToString() : "";//查询条件  送测部门
                string TEST_TYPE = jarr.ContainsKey("TEST_TYPE") ? jarr["TEST_TYPE"].ToString() : "";//查询条件  送测类型 枚举（enum_test_type） 0：成品鞋；1：部件；2：工艺；3：材料；4：量产拉力；2023.05.10(enum_lab_test_type)
                string INSPECTION_NAME = jarr.ContainsKey("INSPECTION_NAME") ? jarr["INSPECTION_NAME"].ToString() : "";//查询条件  测试项目
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//查询条件  送测结果
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListSEASON = SEASON.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListShoe = NAME_T.Split(',');
                string[] orderListart = ART_NO.Split(',');
                string[] orderListJD = PHASE_CREATION_NAME.Split(',');
                string[] orderListDept = STAFF_DEPARTMENT.Split(',');
                string[] orderListtestItems = INSPECTION_NAME.Split(',');
                string Where = string.Empty;

                #region 查询多选 2023.05.10确认
                if (!string.IsNullOrEmpty(SEASON))
                {
                    Where += $@" and a.SEASON in ({string.Join(',', orderListSEASON.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@" and c.NAME_T in ({string.Join(',', orderListShoe.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@" and a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(STAFF_DEPARTMENT))
                {
                    Where += $@" and a.STAFF_DEPARTMENT in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PHASE_CREATION_NAME))
                {
                    Where += $@" and a.PHASE_CREATION_NAME in ({string.Join(',', orderListJD.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(INSPECTION_NAME))
                {
                    Where += $@" and d.INSPECTION_NAME in ({string.Join(',', orderListtestItems.Select(x => $"'{x}'"))})";
                }
                if(!string.IsNullOrEmpty(TEST_TYPE))
                {
                    Where += $@" and a.TEST_TYPE = '{TEST_TYPE}' ";
                }
                #endregion
                #region 单选（废弃）
                /*
                //Category
                if (!string.IsNullOrEmpty(SEASON))
                {
                    Where += $@"and a.SEASON like '%{SEASON}%'";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and a.CATEGORY_NAME like '%{CATEGORY}%'";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and c.NAME_T like '%{NAME_T}%'";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@"and a.art_no = '{ART_NO}'";
                }
                if (!string.IsNullOrEmpty(STAFF_DEPARTMENT))
                {
                    Where += $@"and a.STAFF_DEPARTMENT = '{STAFF_DEPARTMENT}'";
                }
                if (!string.IsNullOrEmpty(PHASE_CREATION_NAME))
                {
                    Where += $@"and a.PHASE_CREATION_NAME = '{PHASE_CREATION_NAME}'";
                }
                if (!string.IsNullOrEmpty(INSPECTION_NAME))
                {
                    Where += $@"and d.INSPECTION_NAME = '{INSPECTION_NAME}'";
                }
                */
                #endregion
                string sql = string.Empty;

                sql = $@"
with tmp as (
	select b.PROD_NO,a.SEASON ,a.CATEGORY_NAME ,c.name_t,d.INSPECTION_NAME,a.PHASE_CREATION_NAME,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from qcm_ex_task_list_m a
	join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
	LEFT JOIN bdm_rd_style c ON b .shoe_no = c.shoe_no
	join qcm_ex_task_list_d d on a.TASK_NO =d.TASK_NO 
	where a.TEST_RESULT='FAIL'
	and a.CREATEDATE between '{starttime}' and '{endtime}'
    {Where}
	group by b.PROD_NO ,b.NAME_T,a.SEASON,a.CATEGORY_NAME,c.name_t ,d.INSPECTION_NAME,a.PHASE_CREATION_NAME
)
select *
from tmp 
where bad_qty>=3
 order by bad_qty desc ";

                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// fail次数-开发季度
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSDataBySeason(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string SEASON = jarr.ContainsKey("SEASON") ? jarr["SEASON"].ToString() : "";//查询条件  季度
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件  Category
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件  Category
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string PHASE_CREATION_NAME = jarr.ContainsKey("PHASE_CREATION_NAME") ? jarr["PHASE_CREATION_NAME"].ToString() : "";//查询条件  阶段
                string STAFF_DEPARTMENT = jarr.ContainsKey("STAFF_DEPARTMENT") ? jarr["STAFF_DEPARTMENT"].ToString() : "";//查询条件  送测部门
                string TEST_TYPE = jarr.ContainsKey("TEST_TYPE") ? jarr["TEST_TYPE"].ToString() : "";//查询条件  送测类型 枚举（enum_test_type） 0：成品鞋；1：部件；2：工艺；3：材料；4：量产拉力；
                string INSPECTION_NAME = jarr.ContainsKey("INSPECTION_NAME") ? jarr["INSPECTION_NAME"].ToString() : "";//查询条件  测试项目
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//查询条件  送测结果
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListSEASON = SEASON.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListShoe = NAME_T.Split(',');
                string[] orderListart = ART_NO.Split(',');
                string[] orderListJD = PHASE_CREATION_NAME.Split(',');
                string[] orderListDept = STAFF_DEPARTMENT.Split(',');
                string[] orderListtestItems = INSPECTION_NAME.Split(',');

                string Where = string.Empty;
                //Category
                if (!string.IsNullOrEmpty(SEASON))
                {
                    Where += $@" and a.SEASON in ({string.Join(',', orderListSEASON.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@" and a.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@" and c.NAME_T in ({string.Join(',', orderListShoe.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@" and a.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(STAFF_DEPARTMENT))
                {
                    Where += $@" and a.STAFF_DEPARTMENT in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PHASE_CREATION_NAME))
                {
                    Where += $@" and a.PHASE_CREATION_NAME in ({string.Join(',', orderListJD.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(INSPECTION_NAME)) 
                {
                    Where += $@" and d.INSPECTION_NAME in ({string.Join(',', orderListtestItems.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(TEST_TYPE))
                {
                    Where += $@" and a.TEST_TYPE = '{TEST_TYPE}' ";
                }
                string sql = string.Empty;

                sql = $@"with tmp as (
	select b.PROD_NO ,b.NAME_T,a.SEASON ,a.CATEGORY_NAME ,
	sum(case when a.TEST_RESULT='FAIL' then to_number(a.SEND_TEST_QTY) else 0 end) bad_qty
	from qcm_ex_task_list_m a
	join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
    LEFT JOIN bdm_rd_style c ON b .shoe_no = c.shoe_no
    join qcm_ex_task_list_d d on a.TASK_NO =d.TASK_NO 
	where a.TEST_RESULT='FAIL'
	and a.CREATEDATE between '{starttime}' and '{endtime}'
    {Where}
	group by b.PROD_NO ,b.NAME_T,a.SEASON,a.CATEGORY_NAME
)
select NVL(SEASON,'NA') as SEASON,NVL(CATEGORY_NAME,'NA') as CATEGORY_NAME,sum(bad_qty) bad_qty
from tmp 
where bad_qty>=3
group by SEASON,CATEGORY_NAME";

                DataTable dt = DB.GetDataTable(sql);

                List<KanBanDtos> res = new List<KanBanDtos>();
                List<string> Xdata = new List<string>();
                List<string> Temp = new List<string>();
                List<string> list_Category = new List<string>();

                foreach (DataRow item in dt.Rows)
                {
                    if (Xdata.Contains(item["SEASON"].ToString()))
                        continue;
                    else
                    {
                        if (item["SEASON"].ToString() == "NA")
                        {
                            //Xdata.Add("");
                            continue;
                        }
                        else
                        {
                            //Xdata.Add(item["SEASON"].ToString());
                            if (item["SEASON"].ToString().ToUpper() == "SS23" || item["SEASON"].ToString().ToUpper() == "FW23")
                            {
                                Xdata.Add(item["SEASON"].ToString());
                            }

                        }
                        Temp.Add(item["SEASON"].ToString());
                    }
                        

                    if (list_Category.Contains(item["CATEGORY_NAME"].ToString()))
                        continue;
                    else
                    {
                            list_Category.Add(item["CATEGORY_NAME"].ToString());
                    }
                }

                foreach (var Cateitem in list_Category)
                {
                    List<string> list_qty = new List<string>();

                    KanBanDtos Dto = new KanBanDtos();
                    Dto.type = "bar";
                    Dto.name = Cateitem;

                    foreach (var Seasonitem in Temp)
                    {
                        var dr = dt.Select($"CATEGORY_NAME = '{Cateitem}' and SEASON ='{Seasonitem}' ");

                        if (dr.Length > 0)
                            list_qty.Add(dr[0]["BAD_QTY"].ToString());
                        else
                            list_qty.Add("0");
                    }
                    Dto.data = list_qty;
                    res.Add(Dto);
                }
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("Xdata", Xdata);
                p.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = p;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// fail次数-柱状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSDataBybarGraph(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string SEASON = jarr.ContainsKey("SEASON") ? jarr["SEASON"].ToString() : "";//查询条件  季度
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件  Category
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件  
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string PHASE_CREATION_NAME = jarr.ContainsKey("PHASE_CREATION_NAME") ? jarr["PHASE_CREATION_NAME"].ToString() : "";//查询条件  阶段
                string STAFF_DEPARTMENT = jarr.ContainsKey("STAFF_DEPARTMENT") ? jarr["STAFF_DEPARTMENT"].ToString() : "";//查询条件  送测部门
                string TEST_TYPE = jarr.ContainsKey("TEST_TYPE") ? jarr["TEST_TYPE"].ToString() : "";//查询条件  送测类型 枚举（enum_test_type） 0：成品鞋；1：部件；2：工艺；3：材料；4：量产拉力；
                string INSPECTION_NAME = jarr.ContainsKey("INSPECTION_NAME") ? jarr["INSPECTION_NAME"].ToString() : "";//查询条件  测试项目
                string TEST_RESULT = jarr.ContainsKey("TEST_RESULT") ? jarr["TEST_RESULT"].ToString() : "";//查询条件  送测结果
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListSEASON = SEASON.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListShoe = NAME_T.Split(',');
                string[] orderListart = ART_NO.Split(',');
                string[] orderListJD = PHASE_CREATION_NAME.Split(',');
                string[] orderListDept = STAFF_DEPARTMENT.Split(',');
                string[] orderListtestItems = INSPECTION_NAME.Split(',');

                string Where = string.Empty;
                //Category
                if (!string.IsNullOrEmpty(SEASON))
                {
                    Where += $@" and b.SEASON in ({string.Join(',', orderListSEASON.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@" and b.CATEGORY_NAME in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@" and c.NAME_T in ({string.Join(',', orderListShoe.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@" and b.art_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(STAFF_DEPARTMENT))
                {
                    Where += $@" and b.STAFF_DEPARTMENT in ({string.Join(',', orderListDept.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(PHASE_CREATION_NAME))
                {
                    Where += $@" and b.PHASE_CREATION_NAME in ({string.Join(',', orderListJD.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(INSPECTION_NAME))
                {
                    Where += $@" and a.INSPECTION_NAME in ({string.Join(',', orderListtestItems.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(TEST_TYPE))
                {
                    Where += $@" and b.TEST_TYPE = '{TEST_TYPE}' ";
                }
                string sql = $@"
with ex_bad_data_tmp1 as (
	select 
	a.INSPECTION_NAME,
	b.CATEGORY_NAME ,
	a.id
	from qcm_ex_task_list_d a
	join qcm_ex_task_list_m b on a.TASK_NO =b.TASK_NO 
	join bdm_rd_prod c on b.ART_NO =c.PROD_NO 
	LEFT JOIN bdm_rd_style d ON b .shoe_no = c.shoe_no
	where a.CREATEDATE between '{starttime}' and '{endtime}' and b.CATEGORY_NAME is not null
    {Where}
	and a.ITEM_TEST_RESULT ='FAIL'
),
ex_bad_data_tmp2 as (
	select 
	INSPECTION_NAME ,
	CATEGORY_NAME ,
	count(id) bad_qty
	from ex_bad_data_tmp1
	group by INSPECTION_NAME ,CATEGORY_NAME
),
total_bad_data as (
	select INSPECTION_NAME,CATEGORY_NAME,bad_qty from ex_bad_data_tmp2
)
select * from total_bad_data 
where bad_qty>=3
order by bad_qty desc";



                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                //KanBanDtos data_pass = new KanBanDtos();
                //data_pass.type = "bar";
                //data_pass.name = "测试报告通过数";

                //KanBanDtos data_fail = new KanBanDtos();
                //data_fail.type = "bar";
                //data_fail.name = "测试报告不通过数";

                //KanBanDtos data_rate = new KanBanDtos();
                //data_rate.type = "line";
                //data_rate.name = "合格率";

                List<string> list_Category = new List<string>();

                //List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);


                //遍历N个月
                foreach (DataRow item in dt.Rows)
                {
                    if (list_Category.Contains(item["CATEGORY_NAME"].ToString()))
                    {
                        continue;
                    }
                    else
                        list_Category.Add(item["CATEGORY_NAME"].ToString());


                    if (Xdata.Contains(item["INSPECTION_NAME"].ToString()))
                        continue;
                    else
                        //X轴
                        Xdata.Add(item["INSPECTION_NAME"].ToString());

                }


                foreach (var Cateitem in list_Category)
                {
                    List<string> list_qty = new List<string>();

                    KanBanDtos Dto = new KanBanDtos();
                    Dto.type = "bar";
                    Dto.name = Cateitem;

                    foreach (var Inspitem in Xdata)
                    {
                        var dr = dt.Select($"CATEGORY_NAME = '{Cateitem}' and INSPECTION_NAME ='{Inspitem}' ");
                        if (dr.Length > 0)
                            list_qty.Add(dr[0]["BAD_QTY"].ToString());
                        else
                            list_qty.Add("0");
                    }
                    Dto.data = list_qty;
                    res.Add(Dto);
                }
                /*
                var dr = dt.Select($"CATEGORY_NAME = '{item}' and ");
                if (dr.Length > 0)
                {
                    for (int i = 0; i < dr.Length; i++)
                    {


                        list_Category.Add(dr[i]["pass_qty"].ToString());

                    }
                    list_pass.Add(dr[0]["pass_qty"].ToString());
                    list_fail.Add(dr[0]["bad_qty"].ToString());
                    list_rate.Add(dr[0]["pass_rate"].ToString());

                }
                else
                {
                    list_pass.Add("0");
                    list_fail.Add("0");
                    list_rate.Add("0");
                }
            }
            data_pass.data = list_pass;
            data_fail.data = list_fail;
            data_rate.data = list_rate;

            res.Add(data_pass);
            res.Add(data_fail);
            res.Add(data_rate);
            */
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 获取看板地址
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetfrontUrl(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string org = jarr.ContainsKey("org") ? jarr["org"].ToString() : "";//查询条件  季度

                string url = DB.GetString($@"SELECT KANBANURL from SYSORG01M where org = '{org}'");

                ret.IsSuccess = true;
                ret.RetData = url;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        #endregion

        #region 车间Q品质看板-TQC

        /// <summary>
        /// 年度/按时间柱状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCByMonth(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//0-年度 1-按时间 
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 
                string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
                string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
                string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["EQ_INFO_NO"].ToString() : "";//查询条件 
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区
                string INSPECTION_NAME = jarr.ContainsKey("INSPECTION_NAME") ? jarr["INSPECTION_NAME"].ToString() : "";//查询条件  检验项目
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;
                string WhereInspect = string.Empty;

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and c.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    WhereInspect += $@"and c.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(MER_PO))
                {
                    arr = MER_PO.Split(',');
                    Where += $@"and a.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    arr = PLANT_AREA.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    arr = shoe_no.Split(',');
                    Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                #region  Commented by Ashok on 2025/04/22   to query the data queckly


                //if (!string.IsNullOrEmpty(workshop_section_no))
                //{
                //    arr = workshop_section_no.Split(',');
                //    Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                //if (!string.IsNullOrEmpty(mold_no))
                //{
                //    Where += $@"and a.mold_no like '%{mold_no}%'";
                //}
                //if (!string.IsNullOrEmpty(eq_info_no))
                //{
                //    Where += $@"and a.eq_info_no like '%{eq_info_no}%'";
                //}
                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}

                #endregion
                //Convert to yyyy-MM-01 ||yyyy-MM-30
                Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);
                DateTime lastyear = DateTime.Now.AddYears(-1);
                DateTime start = new DateTime();
                string Format_date = "to_char(to_date(b.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') as CREATEDATE";
              
                DateTime end = new DateTime();
                if (TYPE == "0")
                {
                    DateTime selectedDate = Convert.ToDateTime(starttime);

                    start = new DateTime(selectedDate.Year, 1, 1);
                    end = new DateTime(selectedDate.Year, 12, 31);
                    //start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    //end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                else
                {
                     Format_date = "b.CREATEDATE";
                    if (!string.IsNullOrEmpty(INSPECTION_NAME))
                    {
                        WhereInspect += $@"and u.INSPECTION_NAME like '%{INSPECTION_NAME}%'";
                    }
                    
                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                }
                int months = end.Month - start.Month + 1;

                //int months = ts.mon;
                if (months <= 0)
                {
                    throw new Exception("Please select the correct date range！");
                }
                if (months > 12)
                {
                    throw new Exception("The number of selected months is prohibited to be greater than 12 months！");
                }

                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");

                string sql = $@"
with task_temp as(
 select distinct a.task_no
    from tqc_task_m a
   inner join base005m c
      on a.production_line_code = c.department_code
   inner join tqc_task_item_c u on u.task_no=a.task_no
   where 1 = 1 and a.createdate BETWEEN '{starttime}' and '{endtime}'
{WhereInspect}
)
 ,jc_temp as (
select distinct
a.task_no ,
count(f.commit_type) as total_qty,
sum(case when f.commit_type = '0'  OR f.commit_type = '2' then 1 else 0 end) as pass_qty,
sum(case when f.commit_type = '1' then 1 else 0 end) as ng_qty
from tqc_task_commit_m f 
inner join  tqc_task_m a  on a.task_no =f.task_no

inner join base005m c on c.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
  inner join bdm_rd_style e ON a.shoe_no = e.shoe_no
 
 --LEFT join bdm_workshop_section_m x on a.workshop_section_no = x.workshop_section_no

     -- join base005m c on c.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
 join SJQDMS_ORGINFO g on c.udf05 = g.code
--LEFT JOIN base001m b1 ON c.FACTORY_SAP = b1.ORG_CODE
     --join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
where a.createdate BETWEEN '{starttime}' and '{endtime}'
{Where}  group by a.task_no

)
SELECT  createdate,
       sum(total_qty) as total_qty,
       sum(PASS_QTY) as PASS_QTY,
       sum(total_qty)-sum(PASS_QTY) as NG_QTY,
       --SUM(NG_QTY) as NG_QTY,  Commented by Ashok on 2025/04/23
       case
         when sum(total_qty) is null or sum(total_qty) = 0 then
          0
         else
          round(sum(PASS_QTY) / sum(total_qty) * 100, 2)
       end as pass_rate from (
SELECT a.*, {Format_date}
from jc_temp a 
inner join  tqc_task_m b  on a.task_no = b.task_no
where 1=1 and a.task_no in (SELECT * from task_temp)
)tt
group by tt.createdate 
";

                //多语言翻译
                var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code2, new List<string>() { "TQC首次不合格数", "TQC首次合格数", "RFT合格率" });
                string name1 = string.Empty;
                string name2 = string.Empty;
                string name3 = string.Empty;
                if (LanDic.Count > 0)
                {
                    name1 = LanDic["TQC首次不合格数"].ToString();
                    name2 = LanDic["TQC首次合格数"].ToString();
                    name3 = LanDic["RFT合格率"].ToString();

                }

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = name2;

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = name1;

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = name3;

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                //
                string rate = string.Empty;
                foreach (DataRow item in dt.Rows)
                {
                    decimal pass_qty = item["pass_qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["pass_qty"].ToString());
                    decimal ng_qty = item["ng_qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["ng_qty"].ToString());
                    decimal total_qty = item["total_qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["total_qty"].ToString());


                    if (pass_qty+ ng_qty == 0)
                        item["pass_rate"] = "0";
                    else
                    {
                        //item["pass_rate"] =Math.Round((pass_qty / (pass_qty + ng_qty)) * 100,1);
                        item["pass_rate"] =Math.Round((pass_qty / total_qty) * 100,1);  //Modified by Ashok
                    }

                }


                if (!string.IsNullOrEmpty(INSPECTION_NAME) || TYPE == "1")
                {
                    for (int i = 0; i < (end.Day - start.Day) + 1; i++)
                    {
                        Xdata.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    //遍历N个月
                    for (int i = 0; i < months; i++)
                    {
                        Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));

                    }
                    
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"CREATEDATE = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["pass_qty"].ToString());
                        list_fail.Add(dr[0]["ng_qty"].ToString());
                        list_rate.Add(dr[0]["pass_rate"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 年度/按时间_饼状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCybyPieMonth(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 
                string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
                string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
                string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区类别
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string Wheredate = string.Empty;
                string Where = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and d.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(MER_PO))
                {
                    arr = MER_PO.Split(',');
                    Where += $@"and d.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and c.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    arr = PLANT_AREA.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    arr = shoe_no.Split(',');
                    Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                #region Commneted by Ashok on 2025/04/23 to query the data queckly
                //if (!string.IsNullOrEmpty(workshop_section_no))
                //{
                //    arr = workshop_section_no.Split(',');
                //    Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                //if (!string.IsNullOrEmpty(mold_no))
                //{
                //    Where += $@"and d.mold_no like '%{mold_no}%'";
                //}
                //if (!string.IsNullOrEmpty(eq_info_no))
                //{
                //    Where += $@"and d.eq_info_no like '%{eq_info_no}%'";
                //}

                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                #endregion
                DateTime start = new DateTime();
                DateTime end = new DateTime();
                if (TYPE == "1")
                {
                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                    
                }
                else
                {
                    DateTime selectedDate = Convert.ToDateTime(starttime);

                    start = new DateTime(selectedDate.Year, 1, 1);
                    end = new DateTime(selectedDate.Year, 12, 31);
                    //start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    //end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");

                string sql = $@"
select b.INSPECTION_NAME, count(a.id) ng_qty
  from tqc_task_detail_t a
  inner join tqc_task_item_c b
    on a.UNION_ID = b.ID
  inner join tqc_task_m d
    on d.task_no = b.task_no
  inner join base005m c
    on c.DEPARTMENT_CODE = d.PRODUCTION_LINE_CODE
  --inner join base001m b1
    --ON c.FACTORY_SAP = b1.ORG_CODE
 -- inner join base003m f
    --on c.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
  inner join bdm_rd_style e
  ON d.shoe_no = e.shoe_no
  inner join SJQDMS_ORGINFO g
    on c.udf05 = g.code
 -- inner join bdm_workshop_section_m x
    --on d.workshop_section_no = x.workshop_section_no
 where a.CREATEDATE between '{starttime}' and '{endtime}' {Where}
 group by b.INSPECTION_NAME
 order by count(a.id) desc
";
                var dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 前五不良项列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCProductData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "";//TRUE走导出 
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 
                string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
                string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
                string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区类别
                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string WhereRow = string.Empty;
                string Where = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    arr = PLANT_AREA.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and d.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(MER_PO))
                {
                    arr = MER_PO.Split(',');
                    Where += $@"and d.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and c.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    arr = shoe_no.Split(',');
                    Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                #region Commneted by Ashok on 2025/04/23 to query the data queckly
                //if (!string.IsNullOrEmpty(workshop_section_no))
                //{
                //    arr = workshop_section_no.Split(',');
                //    Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                //if (!string.IsNullOrEmpty(mold_no))
                //{
                //    Where += $@"and d.mold_no like '%{mold_no}%'";
                //}
                //if (!string.IsNullOrEmpty(eq_info_no))
                //{
                //    Where += $@"and d.eq_info_no like '%{eq_info_no}%'";
                //}
                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                #endregion
                if (IsExport.ToUpper() != "TRUE")
                {
                    WhereRow = " and rownum<=5";
                }

                string sql = $@"
with ng_tmp as
 (select c.INSPECTION_NAME, count(a.id) ng_qty
    from tqc_task_detail_t a
    inner join tqc_task_item_c c
      on a.UNION_ID = c.ID
    inner join tqc_task_m d
      on d.task_no = c.task_no
    inner join base005m c
      on c.DEPARTMENT_CODE = d.PRODUCTION_LINE_CODE
    --inner join base001m b1
      --ON c.FACTORY_SAP = b1.ORG_CODE
    --inner join base003m f
      --on c.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
    inner join bdm_rd_style e
    ON d.shoe_no = e.shoe_no
    inner join SJQDMS_ORGINFO g
      on c.udf05 = g.code
    --inner join bdm_workshop_section_m x
      --on d.workshop_section_no = x.workshop_section_no
   where a.CREATEDATE between '{starttime}' and '{endtime}' {Where} 
   group by c.INSPECTION_NAME
   order by count(a.id) desc),
ng_tmp2 as
 (select * from ng_tmp where 1 = 1 {WhereRow}),
ng_data as
 (select distinct c.TASK_NO, e.INSPECTION_NAME
    from tqc_task_detail_t a
    inner join tqc_task_item_c c
      on a.UNION_ID = c.ID
    inner join ng_tmp2 e
      on c.INSPECTION_NAME = e.INSPECTION_NAME
    inner join tqc_task_m d
      on d.task_no = c.task_no
    inner join base005m c
      on c.DEPARTMENT_CODE = d.PRODUCTION_LINE_CODE
    inner join SJQDMS_ORGINFO g
      on c.udf05 = g.code
    inner join bdm_rd_style e
      ON d.shoe_no = e.shoe_no
   where a.CREATEDATE between '{starttime}' and '{endtime}' {Where}
  --select distinct c.TASK_NO,e.INSPECTION_NAME 
  --from tqc_task_detail_t c
  --left join tqc_task_item_c d on c.UNION_ID =d.ID 
  --join ng_tmp2 e on d.INSPECTION_NAME=e.INSPECTION_NAME
  ),
top5_ng_data as
 (select c.INSPECTION_NAME,
         d.ng_qty inspection_ng_qty,
         sum(case
               when b.commit_type = '0' then
                1
               else
                0
             end) as pass_qty,
         sum(case
               when b.commit_type = '1' then
                1
               else
                0
             end) as ng_qty,
         sum(case
               when b.commit_type = '2' then
                1
               else
                0
             end) as fxhg,
         sum(case
               when b.commit_type = '3' then
                1
               else
                0
             end) as fxbhg,
         sum(case
               when b.commit_type = '4' then
                1
               else
                0
             end) as bp
    from tqc_task_m a
    inner join tqc_task_commit_m b
      on a.task_no = b.task_no
    inner join ng_data c
      on a.TASK_NO = c.TASK_NO
    inner join ng_tmp d
      on c.INSPECTION_NAME = d.INSPECTION_NAME
    --left join base005m c
     -- on c.DEPARTMENT_CODE = a.PRODUCTION_LINE_CODE
    --LEFT JOIN base001m b1
      --ON c.FACTORY_SAP = b1.ORG_CODE
   -- LEFT join base003m f
      --on c.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
   -- join SJQDMS_ORGINFO g
      --on c.udf05 = g.code
   -- left join bdm_workshop_section_m x
      --on a.workshop_section_no = x.workshop_section_no
  
  --where a.CREATEDATE between '{starttime}' and '{endtime}'
  --{Where}
   GROUP BY c.INSPECTION_NAME, d.ng_qty)
select INSPECTION_NAME, --检验项目
       (pass_qty + ng_qty + fxhg + fxbhg + bp) insp_qty, --抽验总数
       inspection_ng_qty, --不合格数量
       round(((pass_qty + ng_qty + fxhg + fxbhg + bp) - inspection_ng_qty) /
             (pass_qty + ng_qty + fxhg + fxbhg + bp) * 100,
             1) pass_rate, --合格率
       ROW_NUMBER() over(order by inspection_ng_qty desc,(pass_qty + ng_qty + fxhg + fxbhg + bp) desc) ranking --排名
  from top5_ng_data
 order by inspection_ng_qty desc,
          (pass_qty + ng_qty + fxhg + fxbhg + bp) desc

";

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 生产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// <summary>
        #region GetTQCProductlineData
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCProductlineData(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

        //    string Data = string.Empty;

        //    string guid = string.Empty;
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //        Data = ReqObj.Data.ToString();

        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

        //        string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

        //        //string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "";//TRUE走导出 

        //        string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
        //        string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 

        //        string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
        //        string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
        //        string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
        //        string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
        //        string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
        //        string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["EQ_INFO_NO"].ToString() : "";//查询条件 机台 

        //        string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
        //        string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区类别
        //        string ProduceType = jarr.ContainsKey("PRODUCETYPE") ? jarr["PRODUCETYPE"].ToString() : "";//查询条件  工艺种类


        //        string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
        //        string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

        //        string GrougroupStr = string.Empty;
        //        string SelectStr = string.Empty;
        //        string Wheredate = string.Empty;
        //        string Where = string.Empty;

        //        string[] arr = new string[] { };

        //        if (!string.IsNullOrEmpty(ART_NO))
        //        {
        //            arr = ART_NO.Split(',');
        //            Where += $@"and c.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and c.prod_no = '{ART_NO}' ";
        //        }
        //        if (!string.IsNullOrEmpty(MER_PO))
        //        {
        //            arr = MER_PO.Split(',');
        //            Where += $@"and c.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and c.MER_PO = '{MER_PO}'";
        //        }

        //        if (!string.IsNullOrEmpty(workshop_section_no))
        //        {
        //            arr = workshop_section_no.Split(',');
        //            Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and x.workshop_section_name like '%{workshop_section_no}%'";
        //        }
        //        if (!string.IsNullOrEmpty(department))
        //        {
        //            arr = department.Split(',');
        //            Where += $@"and d.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and c.department like '%{department}%'";
        //        }
        //        if (!string.IsNullOrEmpty(shoe_no))
        //        {
        //            arr = shoe_no.Split(',');
        //            Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and e.NAME_T like '%{shoe_no}%'";
        //        }
        //        if (!string.IsNullOrEmpty(mold_no))
        //        {
        //            Where += $@"and c.mold_no like '%{mold_no}%'";
        //        }
        //        if (!string.IsNullOrEmpty(eq_info_no))
        //        {
        //            Where += $@"and c.eq_info_no like '%{eq_info_no}%'";
        //        }
        //        if (!string.IsNullOrEmpty(PLANT_AREA))
        //        {
        //            arr = PLANT_AREA.Split(',');
        //            Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and g.EN like '%{PLANT_AREA}%'";
        //        }
        //        if (!string.IsNullOrEmpty(M_TYPE))
        //        {
        //            arr = M_TYPE.Split(',');
        //            Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and f.M_TYPE like '%{M_TYPE}%'";
        //        }
        //        if (!string.IsNullOrEmpty(production_line_code))
        //        {
        //            arr = production_line_code.Split(',');
        //            Where += $@"and u.PRODUCTION_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
        //            //Where += $@"and c.PRODUCTION_LINE_CODE like '%{production_line_code}%'";
        //            //GrougroupStr = ",c.PRODUCTION_LINE_CODE";//
        //        }

        //        //if (string.IsNullOrEmpty(production_line_code))
        //        //{

        //        if (!string.IsNullOrEmpty(eq_info_no))
        //        {
        //            GrougroupStr = " ,c.eq_info_no";
        //            SelectStr = " ,c.eq_info_no";
        //        }

        //        if (!string.IsNullOrEmpty(mold_no))
        //        {
        //            GrougroupStr = " ,c.mold_no";
        //            SelectStr = " ,c.mold_no";
        //        }
        //        //} 

        //        string sql = $@"with depart_tmp as (
        //	select c.ORG_NAME ,PRODUCTION_LINE_CODE,PRODUCTION_LINE_NAME,PLANT_AREA 
        //	from bdm_production_line_m a
        //	left join base005m b on a.PRODUCTION_LINE_CODE =b.DEPARTMENT_CODE 
        //	left join base001m c on b.FACTORY_SAP =c.ORG_CODE 
        //	union all
        //	select c.ORG_NAME ,DEPARTMENT_CODE,DEPARTMENT_NAME,org from base005m a
        //	left join SJQDMS_ORGINFO b on a.udf05=b.code
        //	left join base001m c on a.FACTORY_SAP =c.ORG_CODE
        //),
        //tj_data as (
        //	select c.WORKSHOP_SECTION_NO,c.PRODUCTION_LINE_CODE,u.PRODUCTION_LINE_NAME ,count(a.id) tj_qty{SelectStr}
        //	from tqc_task_detail_t a
        //	join tqc_task_item_c b on a.UNION_ID =b.ID 
        //	join tqc_task_m c on a.TASK_NO =c.TASK_NO 
        //    join  bdm_production_line_m u on  u.PRODUCTION_LINE_CODE =  c.PRODUCTION_LINE_CODE
        //     join base005m d on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
        //    LEFT JOIN base001m b1 ON d.FACTORY_SAP = b1.ORG_CODE
        //	 join base003m f on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE,'0')
        //	 JOIN bdm_rd_style e ON c.shoe_no = e.shoe_no
        //left join SJQDMS_ORGINFO g on d.udf05 = g.code
        //left join bdm_workshop_section_m x on c.workshop_section_no = x.workshop_section_no

        //	where a.CREATEDATE between '{starttime}' and '{endtime}'
        //   {Where}
        //	group by c.WORKSHOP_SECTION_NO,c.PRODUCTION_LINE_CODE,u.PRODUCTION_LINE_NAME{GrougroupStr}
        //),
        //tqc_tmp as (
        //	select 
        //	a.ORG_NAME,a.PLANT_AREA,b.WORKSHOP_SECTION_NO,b.WORKSHOP_SECTION_NAME ,c.PRODUCTION_LINE_CODE ,a.PRODUCTION_LINE_NAME,
        //	sum(case when d.commit_type = '0' then 1 else 0 end) +
        //	sum(case when d.commit_type = '1' then 1 else 0 end) +
        //	sum(case when d.commit_type = '2' then 1 else 0 end) +
        //	sum(case when d.commit_type = '3' then 1 else 0 end) +
        //	sum(case when d.commit_type = '4' then 1 else 0 end) as total_qty,
        //	sum(case when d.commit_type = '0' then 1 else 0 end) as hg,
        //	sum(case when d.commit_type = '1' then 1 else 0 end) as bhg,
        //	sum(case when d.commit_type = '2' then 1 else 0 end) as fxhg,
        //	sum(case when d.commit_type = '3' then 1 else 0 end) as fxbhg,
        //	sum(case when d.commit_type = '4' then 1 else 0 end) as bp,
        //	MAX(nvl(e.tj_qty,0)) tj_qty{SelectStr}
        //	from tqc_task_m c
        // join qcm_dqa_mag_m b on c.SHOE_NO =b.SHOES_CODE and c.WORKSHOP_SECTION_NO =b.WORKSHOP_SECTION_NO 
        // join depart_tmp a on c.PRODUCTION_LINE_CODE =a.PRODUCTION_LINE_CODE
        // join tqc_task_commit_m d on c.task_no = d.task_no 
        //	 join tj_data e on c.WORKSHOP_SECTION_NO=e.WORKSHOP_SECTION_NO and c.PRODUCTION_LINE_CODE=e.PRODUCTION_LINE_CODE

        //	where c.CREATEDATE between '{starttime}' and '{endtime}'
        //	GROUP BY a.ORG_NAME,a.PLANT_AREA,b.WORKSHOP_SECTION_NO,b.WORKSHOP_SECTION_NAME ,c.PRODUCTION_LINE_CODE,a.PRODUCTION_LINE_NAME{GrougroupStr}
        //),
        //tqc_data as (
        //	select 
        //	ORG_NAME,
        //    PLANT_AREA,
        //	 WORKSHOP_SECTION_NO,
        //    WORKSHOP_SECTION_NAME,
        //     PRODUCTION_LINE_CODE,
        //    PRODUCTION_LINE_NAME, 
        //	total_qty,
        //	HG, -- 合格数
        //round(case when hg+bhg=0 then 0 else hg/(hg+bhg)*100 end,1) hg_rate,	--合格率  合格/(合格+不合格) 230516
        //	 BHG,																				--不合格数
        //	FXHG,round(case when total_qty=0 then 0 else FXHG/total_qty*100 end,5) fxhg_rate,	--返修合格数/返修合格率
        //	FXBHG,																				--返修不合格数
        //	BP,																					--B品数
        //	TJ_QTY,																				--脱胶不良
        //	round(case when total_qty=0 then 0 else TJ_QTY/total_qty*100 end,1) tj_rate			--脱胶合格率
        //	from tqc_tmp

        //),
        //tqc_ng_tmp as (
        //	select b.INSPECTION_NAME,c.WORKSHOP_SECTION_NO ,c.PRODUCTION_LINE_CODE ,u.PRODUCTION_LINE_NAME,count(a.id) ng_qty
        //	from tqc_task_detail_t a
        //	left join tqc_task_item_c b on a.UNION_ID =b.ID 
        //	left join TQC_TASK_M c on a.TASK_NO =c.TASK_NO 
        //left join  bdm_production_line_m u on  u.PRODUCTION_LINE_CODE =  c.PRODUCTION_LINE_CODE
        //left join base005m d on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
        //LEFT JOIN base001m b1 ON d.FACTORY_SAP = b1.ORG_CODE
        //left join base003m f on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE,'0')
        //left join SJQDMS_ORGINFO g on d.udf05 = g.code
        //left JOIN bdm_rd_style e ON c.shoe_no = e.shoe_no
        //left join bdm_workshop_section_m x on c.workshop_section_no = x.workshop_section_no
        //	where c.CREATEDATE between '{starttime}' and '{endtime}'
        //    {Where}
        //	group by b.INSPECTION_NAME ,c.WORKSHOP_SECTION_NO ,c.PRODUCTION_LINE_CODE,u.PRODUCTION_LINE_NAME
        //),
        //tqc_ng_data as (
        //	select INSPECTION_NAME,WORKSHOP_SECTION_NO,PRODUCTION_LINE_CODE,NG_QTY ,
        //	ROW_NUMBER () over(partition by WORKSHOP_SECTION_NO,PRODUCTION_LINE_CODE order by ng_qty desc) ranking
        //	from tqc_ng_tmp
        //)
        //select a.*,
        //b.INSPECTION_NAME INSPECTION_NAME_1,
        //round(case when total_qty=0 then 0 else b.ng_qty /a.total_qty*100 end,1) ng_qty_1,
        //c.INSPECTION_NAME INSPECTION_NAME_2,
        //round(case when total_qty=0 then 0 else c.ng_qty /a.total_qty*100 end,1) ng_qty_2,
        //d.INSPECTION_NAME INSPECTION_NAME_3,
        //round(case when total_qty=0 then 0 else d.ng_qty /a.total_qty*100 end,1) ng_qty_3,
        //e.INSPECTION_NAME INSPECTION_NAME_4,
        //round(case when total_qty=0 then 0 else e.ng_qty /a.total_qty*100 end,1) ng_qty_4,
        //f.INSPECTION_NAME INSPECTION_NAME_5,
        //round(case when total_qty=0 then 0 else f.ng_qty /a.total_qty*100 end,1) ng_qty_5
        //from tqc_data a
        //left join tqc_ng_data b on a.WORKSHOP_SECTION_NO=b.WORKSHOP_SECTION_NO and a.PRODUCTION_LINE_CODE=b.PRODUCTION_LINE_CODE and b.ranking=1
        //left join tqc_ng_data c on a.WORKSHOP_SECTION_NO=c.WORKSHOP_SECTION_NO and a.PRODUCTION_LINE_CODE=c.PRODUCTION_LINE_CODE and c.ranking=2
        //left join tqc_ng_data d on a.WORKSHOP_SECTION_NO=d.WORKSHOP_SECTION_NO and a.PRODUCTION_LINE_CODE=d.PRODUCTION_LINE_CODE and d.ranking=3
        //left join tqc_ng_data e on a.WORKSHOP_SECTION_NO=e.WORKSHOP_SECTION_NO and a.PRODUCTION_LINE_CODE=e.PRODUCTION_LINE_CODE and e.ranking=4
        //left join tqc_ng_data f on a.WORKSHOP_SECTION_NO=f.WORKSHOP_SECTION_NO and a.PRODUCTION_LINE_CODE=f.PRODUCTION_LINE_CODE and f.ranking=5

        //";
        //        //-- ORDER BY a.HG_RATE {OrderByStr}
        //        //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

        //        var dt = DB.GetDataTable(sql);

        //        DataTable RetDt = dt.Clone();
        //        DataView DV = new DataView(dt);
        //        DV.Sort = $@" HG_RATE {OrderByStr}";
        //        dt = DV.ToTable();


        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        #region 注释
        //        //                if (ISEXPORT.ToUpper() == "TRUE")
        //        //                {
        //        //                    string hedkey = @"厂区分类,厂区,工段,生产线,检验总数,首次合格总数,RFT首次合格率,返修总数,返修合格数,返修RFT,B品数,脱胶不良总数,脱胶不良率,第一问题点名称,
        //        //第一问题点占比,第二问题点名称,第二问题点占比,第三问题点名称,第三问题点占比";
        //        //                    string[] title = hedkey.Split(',');

        //        //                    List<Dto.ProductlineDto> list = new List<Dto.ProductlineDto>();
        //        //                    foreach (DataRow item in dt.Rows)
        //        //                    {

        //        //                        Dto.ProductlineDto Dto = new Dto.ProductlineDto();
        //        //                        Dto.ORG_NAME = item["ORG_NAME"].ToString();
        //        //                        Dto.PLANT_AREA = item["PLANT_AREA"].ToString();
        //        //                        Dto.WORKSHOP_SECTION_NAME = item["WORKSHOP_SECTION_NAME"].ToString();
        //        //                        Dto.PRODUCTION_LINE_NAME = item["PRODUCTION_LINE_NAME"].ToString();
        //        //                        Dto.total_qty = item["total_qty"].ToString();

        //        //                        Dto.HG = item["HG"].ToString();
        //        //                        Dto.hg_rate = item["hg_rate"].ToString();
        //        //                        Dto.FXTOTAL = (decimal.Parse(item["FXHG"].ToString())+ decimal.Parse(item["FXBHG"].ToString())).ToString();
        //        //                        Dto.FXHG = item["FXHG"].ToString();
        //        //                        Dto.fxhg_rate = item["fxhg_rate"].ToString();
        //        //                        Dto.BP = item["BP"].ToString();

        //        //                        Dto.TJ_QTY = item["TJ_QTY"].ToString();
        //        //                        Dto.tj_rate = item["tj_rate"].ToString();

        //        //                        Dto.INSPECTION_NAME_1 = item["INSPECTION_NAME_1"].ToString();
        //        //                        Dto.ng_qty_1 = item["ng_qty_1"].ToString();

        //        //                        Dto.INSPECTION_NAME_2 = item["INSPECTION_NAME_2"].ToString();
        //        //                        Dto.ng_qty_2 = item["ng_qty_2"].ToString();

        //        //                        Dto.INSPECTION_NAME_3 = item["INSPECTION_NAME_3"].ToString();
        //        //                        Dto.ng_qty_3 = item["ng_qty_3"].ToString();
        //        //                        list.Add(Dto);

        //        //                    }


        //        //                    //string url = CreateExcelFromList(list, title, "TQC生产线列表");
        //        //                    //dic.Add("url", url);
        //        //                    ret.IsSuccess = true;
        //        //                    ret.RetData1 = dic;
        //        //                }
        //        //                else
        //        //                {
        //        #endregion
        //        ret.IsSuccess = true;
        //        ret.RetData1 = dt;
        //        //}



        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = "00000:" + ex.Message;
        //    }
        //    return ret;
        //}
        #endregion

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCProductlineData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

                //string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : "";//TRUE走导出 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 

                string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
                string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
                string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["EQ_INFO_NO"].ToString() : "";//查询条件 机台 

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区类别
                string ProduceType = jarr.ContainsKey("PRODUCETYPE") ? jarr["PRODUCETYPE"].ToString() : "";//查询条件  工艺种类


                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string GrougroupStr = string.Empty;
                string SelectStr = string.Empty;
                string Wheredate = string.Empty;
                string Where = string.Empty;

                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    arr = PLANT_AREA.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and g.EN like '%{PLANT_AREA}%'";
                }
                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and d.DEPARTMENT_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and c.department like '%{department}%'";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and c.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and c.prod_no = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(MER_PO))
                {
                    arr = MER_PO.Split(',');
                    Where += $@"and c.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and c.MER_PO = '{MER_PO}'";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    arr = shoe_no.Split(',');
                    Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and e.NAME_T like '%{shoe_no}%'";
                }
                #region Commented by Ashok on 2025/04/23 to query the data queckly
                //if (!string.IsNullOrEmpty(workshop_section_no))
                //{
                //    arr = workshop_section_no.Split(',');
                //    Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //    //Where += $@"and x.workshop_section_name like '%{workshop_section_no}%'";
                //}
                //if (!string.IsNullOrEmpty(mold_no))
                //{
                //    Where += $@"and c.mold_no like '%{mold_no}%'";
                //}
                //if (!string.IsNullOrEmpty(eq_info_no))
                //{
                //    Where += $@"and c.eq_info_no like '%{eq_info_no}%'";
                //}
                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //    //Where += $@"and f.M_TYPE like '%{M_TYPE}%'";
                //}
                //if (!string.IsNullOrEmpty(production_line_code))
                //{
                //    arr = production_line_code.Split(',');
                //    Where += $@"and u.PRODUCTION_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //    //Where += $@"and c.PRODUCTION_LINE_CODE like '%{production_line_code}%'";
                //    //GrougroupStr = ",c.PRODUCTION_LINE_CODE";//
                //}
                #endregion

                if (!string.IsNullOrEmpty(eq_info_no))
                {
                    GrougroupStr = " ,c.eq_info_no";
                    SelectStr = " ,c.eq_info_no";
                }

                if (!string.IsNullOrEmpty(mold_no))
                {
                    GrougroupStr = " ,c.mold_no";
                    SelectStr = " ,c.mold_no";
                }
                
               
                //Updated by Ashok on 2025/04/23 to query the data queckly
                string sql = $@"with depart_tmp as
 (select c.ORG_NAME, PRODUCTION_LINE_CODE, PRODUCTION_LINE_NAME, PLANT_AREA
    from bdm_production_line_m a
    inner join base005m b
      on a.PRODUCTION_LINE_CODE = b.DEPARTMENT_CODE
    inner join base001m c
      on b.FACTORY_SAP = c.ORG_CODE
  union all
  select c.ORG_NAME, DEPARTMENT_CODE, DEPARTMENT_NAME, org
    from base005m a
    inner join SJQDMS_ORGINFO b
      on a.udf05 = b.code
    inner join base001m c
      on a.FACTORY_SAP = c.ORG_CODE),
tj_data as
 (select c.WORKSHOP_SECTION_NO,
         c.PRODUCTION_LINE_CODE,
         u.department_name,
         count(a.id) tj_qty{SelectStr}
    from tqc_task_detail_t a
   inner join tqc_task_item_c b
      on a.UNION_ID = b.ID
   inner join tqc_task_m c
      on a.TASK_NO = c.TASK_NO
  --join  bdm_production_line_m u on  u.PRODUCTION_LINE_CODE =  c.PRODUCTION_LINE_CODE
    inner join base005m u
      on u.department_code = c.production_line_code
   inner join base005m d
      on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
    --inner JOIN base001m b1
     -- ON d.FACTORY_SAP = b1.ORG_CODE
  -- inner join base003m f
      --on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
   inner join bdm_rd_style e
    ON c.shoe_no = e.shoe_no
    inner join SJQDMS_ORGINFO g
      on d.udf05 = g.code
    --inner join bdm_workshop_section_m x
     -- on c.workshop_section_no = x.workshop_section_no
  
   where a.CREATEDATE between '{starttime}' and '{endtime}' {Where}
   group by c.WORKSHOP_SECTION_NO,
            c.PRODUCTION_LINE_CODE,
            u.department_name{GrougroupStr}),
tqc_tmp as
 (select a.ORG_NAME,
         a.PLANT_AREA,
         b.WORKSHOP_SECTION_NO,
         b.WORKSHOP_SECTION_NAME,
         c.PRODUCTION_LINE_CODE,
         a.PRODUCTION_LINE_NAME,
         sum(case
               when d.commit_type = '0' then
                1
               else
                0
             end) + sum(case
                          when d.commit_type = '1' then
                           1
                          else
                           0
                        end) + sum(case
                                     when d.commit_type = '2' then
                                      1
                                     else
                                      0
                                   end) +
         sum(case
               when d.commit_type = '3' then
                1
               else
                0
             end) + sum(case
                          when d.commit_type = '4' then
                           1
                          else
                           0
                        end) as total_qty,
         sum(case
               when d.commit_type = '0' then
                1
               else
                0
             end) as hg,
         sum(case
               when d.commit_type = '1' then
                1
               else
                0
             end) as bhg,
         sum(case
               when d.commit_type = '2' then
                1
               else
                0
             end) as fxhg,
         sum(case
               when d.commit_type = '3' then
                1
               else
                0
             end) as fxbhg,
         sum(case
               when d.commit_type = '4' then
                1
               else
                0
             end) as bp,
         MAX(nvl(e.tj_qty, 0)) tj_qty{SelectStr}
    from tqc_task_m c
   inner join qcm_dqa_mag_m b
      on c.SHOE_NO = b.SHOES_CODE
     and c.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO
   inner join depart_tmp a
      on c.PRODUCTION_LINE_CODE = a.PRODUCTION_LINE_CODE
   inner join tqc_task_commit_m d
      on c.task_no = d.task_no
   inner join tj_data e
      on c.WORKSHOP_SECTION_NO = e.WORKSHOP_SECTION_NO
     and c.PRODUCTION_LINE_CODE = e.PRODUCTION_LINE_CODE
   inner join base005m d
      on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
      inner join SJQDMS_ORGINFO g
      on d.udf05 = g.code
  inner join bdm_rd_style e
    ON c.shoe_no = e.shoe_no
   where c.CREATEDATE between '{starttime}' and '{endtime}' {Where}
   GROUP BY a.ORG_NAME,
            a.PLANT_AREA,
            b.WORKSHOP_SECTION_NO,
            b.WORKSHOP_SECTION_NAME,
            c.PRODUCTION_LINE_CODE,
            a.PRODUCTION_LINE_NAME{GrougroupStr}),
tqc_data as
 (select ORG_NAME,
         PLANT_AREA,
         WORKSHOP_SECTION_NO,
         WORKSHOP_SECTION_NAME,
         PRODUCTION_LINE_CODE,
         PRODUCTION_LINE_NAME,
         total_qty,
         HG, -- 合格数
         round(case
                 when hg + bhg = 0 then
                  0
                 else
                  hg / (total_qty) * 100
               end,
               1) hg_rate, --合格率  合格/(合格+不合格) 230516
         BHG, --不合格数
         FXHG,
         --round(case when total_qty=0 then 0 else FXHG/total_qty*100 end,5) fxhg_rate,  --返修合格数/返修合格率
         round(case
                 when (FXHG + fxbhg) = 0 then
                  0
                 else
                  FXHG / (FXHG + fxbhg) * 100
               end,
               5) fxhg_rate, --返修合格数/返修合格率
         FXBHG, --返修不合格数
         BP, --B品数
         TJ_QTY, --脱胶不良
         round(case
                 when total_qty = 0 then
                  0
                 else
                  TJ_QTY / total_qty * 100
               end,
               1) tj_rate --脱胶合格率
    from tqc_tmp
  
  ),
tqc_ng_tmp as
 (select b.INSPECTION_NAME,
         c.WORKSHOP_SECTION_NO,
         c.PRODUCTION_LINE_CODE,
         --u.PRODUCTION_LINE_NAME,
         count(a.id) ng_qty
    from tqc_task_detail_t a
    inner join tqc_task_item_c b
      on a.UNION_ID = b.ID
    inner join TQC_TASK_M c
      on a.TASK_NO = c.TASK_NO
    --left join bdm_production_line_m u
     -- on u.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE
    inner join base005m d
      on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
   -- LEFT JOIN base001m b1
     -- ON d.FACTORY_SAP = b1.ORG_CODE
    --left join base003m f
     -- on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
    left join SJQDMS_ORGINFO g
      on d.udf05 = g.code
    inner join bdm_rd_style e
     ON c.shoe_no = e.shoe_no
    --left join bdm_workshop_section_m x
     -- on c.workshop_section_no = x.workshop_section_no
   where c.CREATEDATE between '{starttime}' and '{endtime}' {Where}
   group by b.INSPECTION_NAME,
            c.WORKSHOP_SECTION_NO,
            c.PRODUCTION_LINE_CODE
            --u.PRODUCTION_LINE_NAME
),
tqc_ng_data as
 (select INSPECTION_NAME,
         WORKSHOP_SECTION_NO,
         PRODUCTION_LINE_CODE,
         NG_QTY,
         ROW_NUMBER() over(partition by WORKSHOP_SECTION_NO, PRODUCTION_LINE_CODE order by ng_qty desc) ranking
    from tqc_ng_tmp)
select a.*,
       b.INSPECTION_NAME INSPECTION_NAME_1,
       round(case
               when total_qty = 0 then
                0
               else
                b.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_1,
       c.INSPECTION_NAME INSPECTION_NAME_2,
       round(case
               when total_qty = 0 then
                0
               else
                c.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_2,
       d.INSPECTION_NAME INSPECTION_NAME_3,
       round(case
               when total_qty = 0 then
                0
               else
                d.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_3,
       e.INSPECTION_NAME INSPECTION_NAME_4,
       round(case
               when total_qty = 0 then
                0
               else
                e.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_4,
       f.INSPECTION_NAME INSPECTION_NAME_5,
       round(case
               when total_qty = 0 then
                0
               else
                f.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_5
  from tqc_data a
 inner join tqc_ng_data b
    on a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO
   and a.PRODUCTION_LINE_CODE = b.PRODUCTION_LINE_CODE
   and b.ranking = 1 --left join is changed to join
 inner join tqc_ng_data c
    on a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO
   and a.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE
   and c.ranking = 2 --left join is changed to join
 inner join tqc_ng_data d
    on a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO
   and a.PRODUCTION_LINE_CODE = d.PRODUCTION_LINE_CODE
   and d.ranking = 3 --left join is changed to join
 inner join tqc_ng_data e
    on a.WORKSHOP_SECTION_NO = e.WORKSHOP_SECTION_NO
   and a.PRODUCTION_LINE_CODE = e.PRODUCTION_LINE_CODE
   and e.ranking = 4 --left join is changed to join
 inner join tqc_ng_data f
    on a.WORKSHOP_SECTION_NO = f.WORKSHOP_SECTION_NO
   and a.PRODUCTION_LINE_CODE = f.PRODUCTION_LINE_CODE
   and f.ranking = 5 --left join is changed to join

        ";
                //-- ORDER BY a.HG_RATE {OrderByStr}
                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                var dt = DB.GetDataTable(sql);

                DataTable RetDt = dt.Clone();
                DataView DV = new DataView(dt);
                DV.Sort = $@" HG_RATE {OrderByStr}";
                dt = DV.ToTable();


                Dictionary<string, object> dic = new Dictionary<string, object>();
                #region 注释
                //                if (ISEXPORT.ToUpper() == "TRUE")
                //                {
                //                    string hedkey = @"厂区分类,厂区,工段,生产线,检验总数,首次合格总数,RFT首次合格率,返修总数,返修合格数,返修RFT,B品数,脱胶不良总数,脱胶不良率,第一问题点名称,
                //第一问题点占比,第二问题点名称,第二问题点占比,第三问题点名称,第三问题点占比";
                //                    string[] title = hedkey.Split(',');

                //                    List<Dto.ProductlineDto> list = new List<Dto.ProductlineDto>();
                //                    foreach (DataRow item in dt.Rows)
                //                    {

                //                        Dto.ProductlineDto Dto = new Dto.ProductlineDto();
                //                        Dto.ORG_NAME = item["ORG_NAME"].ToString();
                //                        Dto.PLANT_AREA = item["PLANT_AREA"].ToString();
                //                        Dto.WORKSHOP_SECTION_NAME = item["WORKSHOP_SECTION_NAME"].ToString();
                //                        Dto.PRODUCTION_LINE_NAME = item["PRODUCTION_LINE_NAME"].ToString();
                //                        Dto.total_qty = item["total_qty"].ToString();

                //                        Dto.HG = item["HG"].ToString();
                //                        Dto.hg_rate = item["hg_rate"].ToString();
                //                        Dto.FXTOTAL = (decimal.Parse(item["FXHG"].ToString())+ decimal.Parse(item["FXBHG"].ToString())).ToString();
                //                        Dto.FXHG = item["FXHG"].ToString();
                //                        Dto.fxhg_rate = item["fxhg_rate"].ToString();
                //                        Dto.BP = item["BP"].ToString();

                //                        Dto.TJ_QTY = item["TJ_QTY"].ToString();
                //                        Dto.tj_rate = item["tj_rate"].ToString();

                //                        Dto.INSPECTION_NAME_1 = item["INSPECTION_NAME_1"].ToString();
                //                        Dto.ng_qty_1 = item["ng_qty_1"].ToString();

                //                        Dto.INSPECTION_NAME_2 = item["INSPECTION_NAME_2"].ToString();
                //                        Dto.ng_qty_2 = item["ng_qty_2"].ToString();

                //                        Dto.INSPECTION_NAME_3 = item["INSPECTION_NAME_3"].ToString();
                //                        Dto.ng_qty_3 = item["ng_qty_3"].ToString();
                //                        list.Add(Dto);

                //                    }


                //                    //string url = CreateExcelFromList(list, title, "TQC生产线列表");
                //                    //dic.Add("url", url);
                //                    ret.IsSuccess = true;
                //                    ret.RetData1 = dic;
                //                }
                //                else
                //                {
                #endregion
                ret.IsSuccess = true;
                ret.RetData1 = dt;
                //}



            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 产品明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQCProductCPData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";
                //string IsExport = jarr.ContainsKey("IsExport") ? jarr["IsExport"].ToString() : ""; // true走导出逻辑

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string MER_PO = jarr.ContainsKey("MER_PO") ? jarr["MER_PO"].ToString() : "";//查询条件 

                string workshop_section_no = jarr.ContainsKey("WORKSHOP_SECTION_NO") ? jarr["WORKSHOP_SECTION_NO"].ToString() : "";//查询条件 
                string department = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 
                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 
                string mold_no = jarr.ContainsKey("MOLD_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 
                string eq_info_no = jarr.ContainsKey("EQ_INFO_NO") ? jarr["MOLD_NO"].ToString() : "";//查询条件 

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类别
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";//查询条件  厂区类别
                string ProduceType = jarr.ContainsKey("PRODUCETYPE") ? jarr["PRODUCETYPE"].ToString() : "";//查询条件  工艺种类

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Wheredate = string.Empty;
                string Where = string.Empty;
                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    arr = PLANT_AREA.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and c.prod_no in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(MER_PO))
                {
                    arr = MER_PO.Split(',');
                    Where += $@"and c.MER_PO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and  d.department_name  in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(shoe_no))
                {
                    arr = shoe_no.Split(',');
                    Where += $@"and e.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                #region commented by Ashok on 2025/04/24 to query the data queckly
                //if (!string.IsNullOrEmpty(workshop_section_no))
                //{
                //    arr = workshop_section_no.Split(',');
                //    Where += $@"and x.workshop_section_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}


                //if (!string.IsNullOrEmpty(mold_no))
                //{
                //    Where += $@"and c.mold_no like '%{mold_no}%'";
                //}
                //if (!string.IsNullOrEmpty(eq_info_no))
                //{
                //    Where += $@"and c.eq_info_no like '%{eq_info_no}%'";
                //}
                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                //if (!string.IsNullOrEmpty(production_line_code))
                //{
                //    arr = production_line_code.Split(',');
                //    Where += $@"and u.production_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //}
                #endregion

                string sql = $@"
with tqc_tmp as
 (select c.WORKSHOP_SECTION_NO,
         x.WORKSHOP_SECTION_NAME,
         c.PROD_NO art_no,
         a.NAME_T Shoe_type_name,--art_name,
         c.shoe_no,
         e.name_t as art_name,--Shoe_type_name,
         sum(case
               when d.commit_type = '0' then
                1
               else
                0
             end) + sum(case
                          when d.commit_type = '1' then
                           1
                          else
                           0
                        end) + sum(case
                                     when d.commit_type = '2' then
                                      1
                                     else
                                      0
                                   end) +
         sum(case
               when d.commit_type = '3' then
                1
               else
                0
             end) + sum(case
                          when d.commit_type = '4' then
                           1
                          else
                           0
                        end) as total_qty,
         sum(case
               when d.commit_type = '0' then
                1
               else
                0
             end) as hg,
         sum(case
               when d.commit_type = '1' then
                1
               else
                0
             end) as bhg,
         sum(case
               when d.commit_type = '2' then
                1
               else
                0
             end) as fxhg,
         sum(case
               when d.commit_type = '3' then
                1
               else
                0
             end) as fxbhg,
         sum(case
               when d.commit_type = '4' then
                1
               else
                0
             end) as bp
    from tqc_task_m c
    inner join qcm_dqa_mag_m b
      on c.SHOE_NO = b.SHOES_CODE
     and c.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO
   inner join bdm_rd_prod a
      on a.PROD_NO = c.PROD_NO
    inner join tqc_task_commit_m d
      on c.task_no = d.task_no
    inner JOIN bdm_rd_style e
      ON c.shoe_no = e.shoe_no
    --left join bdm_production_line_m u
     -- on u.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE
    inner join base005m d
      on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
    --LEFT JOIN base001m b1
     -- ON d.FACTORY_SAP = b1.ORG_CODE
   -- join base003m f
     -- on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
    --join bdm_rd_style l
     -- ON l.shoe_no = c.shoe_no
   inner join SJQDMS_ORGINFO g
      on d.udf05 = g.code
    inner join bdm_workshop_section_m x
      on c.workshop_section_no = x.workshop_section_no
   where c.CREATEDATE between '{starttime}' and '{endtime}' {Where}
   GROUP BY c.WORKSHOP_SECTION_NO,
            x.WORKSHOP_SECTION_NAME,
            c.PROD_NO,
            a.NAME_T,
            c.shoe_no,
            e.name_t),
tqc_data as
 (select WORKSHOP_SECTION_NO,
         WORKSHOP_SECTION_NAME,
         art_no,
         art_name,
         shoe_no,
         Shoe_type_name,
         total_qty,
         HG, --合格数
         round(case
                 when hg + bhg = 0 then
                  0
                 else
                  hg / (total_qty) * 100
               end,
               1) hg_rate, -- 合格率 合格/(合格+不合格)
         BHG, --不合格数
         FXHG,
         round(case
                 when total_qty = 0 then
                  0
                 else
                  FXHG / total_qty * 100
               end,
               1) fxhg_rate, --返修合格数/返修合格率
         FXBHG, --返修不合格数
         BP --B品数
    from tqc_tmp),
tqc_ng_tmp as
 (select b.INSPECTION_NAME,
         c.WORKSHOP_SECTION_NO,
         c.SHOE_NO,
         c.PROD_NO,
         count(a.id) ng_qty
    from tqc_task_detail_t a
   inner join tqc_task_item_c b
      on a.UNION_ID = b.ID
  inner join TQC_TASK_M c
      on a.TASK_NO = c.TASK_NO
    --left join bdm_production_line_m u
     -- on u.PRODUCTION_LINE_CODE = c.PRODUCTION_LINE_CODE
    inner join base005m d
      on d.DEPARTMENT_CODE = c.PRODUCTION_LINE_CODE
   -- LEFT JOIN base001m b1
      --ON d.FACTORY_SAP = b1.ORG_CODE
    --LEFT join base003m f
     -- on d.FACTORY_SAP = ltrim(f.SUPPLIERS_CODE, '0')
  
   inner join bdm_rd_style e
      ON e.shoe_no = c.shoe_no
    inner join SJQDMS_ORGINFO g
      on d.udf05 = g.code
    --inner join bdm_workshop_section_m x
     --on c.workshop_section_no = x.workshop_section_name
   where a.CREATEDATE between '{starttime}' and '{endtime}' {Where}
   group by b.INSPECTION_NAME, c.WORKSHOP_SECTION_NO, c.SHOE_NO, c.PROD_NO),
tqc_ng_data as
 (select INSPECTION_NAME,
         WORKSHOP_SECTION_NO,
         SHOE_NO,
         PROD_NO,
         NG_QTY,
         ROW_NUMBER() over(partition by WORKSHOP_SECTION_NO, SHOE_NO, PROD_NO order by ng_qty desc) ranking
    from tqc_ng_tmp)
select a.*,
       b.INSPECTION_NAME INSPECTION_NAME_1,
       round(case
               when total_qty = 0 then
                0
               else
                b.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_1,
       c.INSPECTION_NAME INSPECTION_NAME_2,
       round(case
               when total_qty = 0 then
                0
               else
                c.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_2,
       d.INSPECTION_NAME INSPECTION_NAME_3,
       round(case
               when total_qty = 0 then
                0
               else
                d.ng_qty / a.total_qty * 100
             end,
             1) ng_qty_3
  from tqc_data a
  join tqc_ng_data b
    on a.WORKSHOP_SECTION_NO = b.WORKSHOP_SECTION_NO
   and a.SHOE_NO = b.SHOE_NO
   and a.art_no = b.PROD_NO
   and b.ranking = 1 --left join is changed to join
  join tqc_ng_data c
    on a.WORKSHOP_SECTION_NO = c.WORKSHOP_SECTION_NO
   and a.SHOE_NO = c.SHOE_NO
   and a.art_no = c.PROD_NO
   and c.ranking = 2 --left join is changed to join
  join tqc_ng_data d
    on a.WORKSHOP_SECTION_NO = d.WORKSHOP_SECTION_NO
   and a.SHOE_NO = d.SHOE_NO
   and a.art_no = d.PROD_NO
   and d.ranking = 3 --left join is changed to join
 order by a.HG_RATE {OrderByStr}

";
              
                var dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        #endregion

        #region 金属检测

        /// <summary>
        /// 柱状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// 
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMetalDetectionByYear(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//0-年度 1-按时间 
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间  

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);

                DateTime lastyear = DateTime.Now.AddYears(-1); 
                DateTime start = new DateTime();
               // string Format_date = "to_char(to_date(b.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') as CREATEDATE";

                DateTime end = new DateTime();
                if (TYPE == "0")
                {
                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                else
                {
                    //Format_date = "b.CREATEDATE"; 

                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                }
                int months = end.Month - start.Month + 1; 
                if (months <= 0)
                {
                    throw new Exception("Please select the correct date range！");
                }
                if (months > 12)
                {
                    throw new Exception("The number of selected months is prohibited to be greater than 12 months！");
                }


                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");
                string Where = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(',');
                    Where += $@"and a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.pro_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                

                string sql = $@" select to_char(to_date(a.createdate, 'yyyy-MM-dd'), 'yyyy-MM') as month,
 count(*) as Total_Qty,
 sum (case when a.processing_res='0' then 1 else 0 end) as fail_Qty,
  sum (case when a.processing_res='1' then 1 else 0 end) as Pass_Qty,
'' as pass_rate
   from qcm_insp_metal_m a where 1=1 {Where}
  group by to_char(to_date(a.createdate, 'yyyy-MM-dd'), 'yyyy-MM')
";

                //多语言翻译
                var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() {"通行证数量", "不合格数量", "通过率" });
                string name1 = string.Empty;
                string name2 = string.Empty;
                string name3 = string.Empty;
                if (LanDic.Count > 0)
                {
                    name1 = LanDic["通行证数量"].ToString();
                    name2 = LanDic["不合格数量"].ToString();
                    name3 = LanDic["通过率"].ToString();

                }

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = name1;

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = name2;

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = name3;

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                //
                string rate = string.Empty;
                foreach (DataRow item in dt.Rows)
                {
                    decimal pass_qty = item["pass_qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["pass_qty"].ToString());
                    decimal ng_qty = item["fail_Qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["fail_Qty"].ToString());
                    decimal total_qty = item["Total_Qty"].ToString() == "" ? 0 : Convert.ToDecimal(item["Total_Qty"].ToString());


                    if (pass_qty + ng_qty == 0)
                        item["pass_rate"] = "0";
                    else
                    {
                        //item["pass_rate"] =Math.Round((pass_qty / (pass_qty + ng_qty)) * 100,1);
                        item["pass_rate"] = Math.Round((pass_qty / total_qty) * 100, 1);  //Modified by Ashok
                    }

                }


                if (TYPE == "1")
                {
                    for (int i = 0; i < (end.Day - start.Day) + 1; i++)
                    {
                        Xdata.Add(start.AddDays(i).ToString("yyyy-MM-dd"));
                    }
                }
                else
                {
                    //遍历N个月
                    for (int i = 0; i < months; i++)
                    {
                        Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));

                    }

                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"month = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["pass_qty"].ToString());
                        list_fail.Add(dr[0]["fail_Qty"].ToString());
                        list_rate.Add(dr[0]["pass_rate"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMetalDetectionByPieYear(object OBJ) 
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                 

                string Wheredate = string.Empty;
                string Where = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) "; 
                }
                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(','); 
                    Where += $@"and a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) "; 
                }  
                 
                if (!string.IsNullOrEmpty(production_line_code))
                { 
                    arr = production_line_code.Split(',');
                    Where += $@"and a.pro_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) "; 
                } 
                

                DateTime start = new DateTime();
                DateTime end = new DateTime();
                if (TYPE == "1")
                {
                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);

                }
                else
                {
                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");

                string sql = $@"select a.problem_point_desc, count(*) quantity
  from qcm_insp_metal_m a
where 1=1 {Where} and a.CREATEDATE between '{starttime}' and '{endtime}'
group by a.problem_point_desc
 order by count(*) desc
";
                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                var dt = DB.GetDataTable(sql);



                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJSJCData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                /*
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("PRODUCTION_LINE_CODE") ? jarr["PRODUCTION_LINE_CODE"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                */
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//0-年度 1-按时间 
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                //string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string Where = string.Empty;

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');
                    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    //Where += $@"and d.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.PRO_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    arr = NAME_T.Split(',');
                    Where += $@"and  b.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.NAME_T = '{NAME_T}' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(',');
                    Where += $@"and a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@" and a.PO_NO = '{PO_NO}'";
                }


                /*
                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    Where += $@"and d.M_TYPE like '%{M_TYPE}%'";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    Where += $@"and g.EN like '%{UDF05}%' ";
                }
                if (!string.IsNullOrEmpty(DEPARTMENT))
                {
                    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                }

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    Where += $@"and a.PRO_LINE_NO like '%{production_line_code}%' ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and b.NAME_T like '%{NAME_T}%' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    Where += $@" and a.PO_NO = '{PO_NO}'";
                }
                */
                //转换为 yyyy-MM-01 ||yyyy-MM-30 
                Dictionary<string, object> dicTime = GetTimeDic(starttime, endtime);

                DateTime lastyear = DateTime.Now.AddYears(-1);
                //string starttime = DateTime.Now.AddYears(-1).ToString("yyyy-MM-01");
                //string endtime = DateTime.Now.ToString($"yyyy-MM-{dd}");
                DateTime start = new DateTime();
                DateTime end = new DateTime();
                string format_date = string.Empty;
                if (TYPE == "0")
                {
                    format_date = "to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')";
                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                else
                {
                    format_date = "a.CREATEDATE";
                    
                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                }
                int months = end.Month - start.Month + 1;
                int days = end.Day - start.Day + 1;




                //int months = ts.mon;
                if (months <= 0)
                {
                    throw new Exception("请选择正确日期范围！");
                }
                if (months > 12)
                {
                    throw new Exception("所选择月份数禁止大于12个月！");
                }


                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");
                #region old
                /*
                string sql = $@"
with tmp as (
	select 
	{format_date} createdate,
	sum(case when a.PROCESSING_RES =1 then 1 else 0 end) pass_qty,
	sum(case when a.PROCESSING_RES =0 then 1 else 0 end) ng_qty
	from qcm_insp_metal_m a


	LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
    LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
    LEFT JOIN base005m f on f.department_code = e.udf07

left join SJQDMS_ORGINFO g on c.udf05 = g.code
	where a.PROCESSING_RES is not NULL 
    {Where}
	and a.CREATEDATE  BETWEEN '{starttime}' AND '{endtime}'
	group by {format_date}
)
select 
createdate,pass_qty,ng_qty ,
round(pass_qty/(pass_qty+ng_qty)*100,1) pass_rate
from tmp";
                */
                #endregion

                string sql = $@"
with tmp as (
	select 
	{format_date} createdate,

	MAX((SELECT count(1) from ESM_MD_CHANLIANG_LIST@APEIOT g 
 where g.art = a.ART_NO and g.PO = a.PO_NO and g.RIQI  
-- BETWEEN '2022-12-01' and '2022-12-31'
BETWEEN '{starttime}' and '{endtime}'
)) as pass_qty,

MAX((SELECT count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT h 
where h.art =a.ART_NO  and a.PO_NO = h.PO and to_char(h.CREATE_TIME,'yyyy-MM-dd') BETWEEN '{starttime}' and '{endtime}'))as ng_qty

	from qcm_insp_metal_m a

	LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
    LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
    LEFT JOIN base005m f on f.department_code = e.udf07
    LEFT JOIN BASE001M b1 ON c.FACTORY_SAP = b1.ORG_CODE
left join SJQDMS_ORGINFO g on c.udf05 = g.code
	where 1=1
    -- a.PROCESSING_RES is not NULL 
    {Where}
	and a.CREATEDATE  BETWEEN '{starttime}' AND '{endtime}'
	group by {format_date}
)
select 
createdate,pass_qty,ng_qty ,
CASE
when pass_qty+ng_qty = 0 then 0
else round(pass_qty/(pass_qty+ng_qty)*100,1)
END as pass_rate
from tmp";


                //多语言翻译
                var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "金属检测不合格数", "金属检测合格数" , "合格率" });
                string name1 = string.Empty;
                string name2 = string.Empty;
                string name3 = string.Empty;
                if (LanDic.Count > 0)
                {
                    name1 = LanDic["金属检测不合格数"].ToString();
                    name2 = LanDic["金属检测合格数"].ToString();
                    name3 = LanDic["合格率"].ToString();

                }

                //and a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_pass = new KanBanDtos();
                data_pass.type = "bar";
                data_pass.name = name1;

                KanBanDtos data_fail = new KanBanDtos();
                data_fail.type = "bar";
                data_fail.name = name2;

                KanBanDtos data_rate = new KanBanDtos();
                data_rate.type = "line";
                data_rate.name = name3;

                List<string> list_pass = new List<string>();
                List<string> list_fail = new List<string>();
                List<string> list_rate = new List<string>();
                var dt = DB.GetDataTable(sql);

                if(TYPE == "0")
                {
                    //遍历N个月
                    for (int i = 0; i < months; i++)
                    {
                        Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));

                    }
                }
                else
                {
                    for (int i = 0; i < days; i++)
                    {
                        Xdata.Add(start.AddDays(i).ToString("yyyy-MM-dd"));

                    }
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"createdate = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_pass.Add(dr[0]["pass_qty"].ToString());
                        list_fail.Add(dr[0]["ng_qty"].ToString());
                        list_rate.Add(dr[0]["pass_rate"].ToString());

                    }
                    else
                    {
                        list_pass.Add("0");
                        list_fail.Add("0");
                        list_rate.Add("0");
                    }
                }
                data_pass.data = list_pass;
                data_fail.data = list_fail;
                data_rate.data = list_rate;

                res.Add(data_pass);
                res.Add(data_fail);
                res.Add(data_rate);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);

                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 年度/按时间_饼状图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJSJCbyPie(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//0-年度 1-按时间 

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                //string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;


                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');

                    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.PRO_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    arr = NAME_T.Split(',');
                    Where += $@"and  b.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.NAME_T = '{NAME_T}' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(',');
                    Where += $@"and  a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@" and a.PO_NO = '{PO_NO}'";
                }

                DateTime start = new DateTime();
                DateTime end = new DateTime();
                //string format_date = string.Empty;
                if (TYPE == "0")
                {
                    //format_date = "";
                    start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                    end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                }
                else
                {
                    start = DateTime.Parse(starttime);
                    end = DateTime.Parse(endtime);
                }

                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");
                #region old
                /*
                string sql = $@"
with insp_data as (
	select b.PROD_NO as NAME_T ,
	sum(case when a.PROCESSING_RES =1 then 1 else 0 end) pass_qty,
	sum(case when a.PROCESSING_RES =0 then 1 else 0 end) ng_qty
	from QCM_INSP_METAL_M a
	join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
    LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
    LEFT JOIN base005m f on f.department_code = e.udf07
left join SJQDMS_ORGINFO g on c.udf05 = g.code
	where a.PROCESSING_RES =0
    {Where}
    and a.CREATEDATE  BETWEEN '{starttime}' AND '{endtime}'
	group by b.PROD_NO
)
select 
NAME_T,pass_qty,ng_qty ,
round(pass_qty/(pass_qty+ng_qty)*100,1) pass_rate
from insp_data
order by pass_rate ,ng_qty desc
";
                */

                #endregion
                string sql = $@"
with insp_data as (
	select b.PROD_NO as NAME_T ,
		MAX((SELECT count(1) from ESM_MD_CHANLIANG_LIST@APEIOT g 
 where g.art = a.ART_NO and g.PO = a.PO_NO and g.RIQI  BETWEEN '{starttime}' AND '{endtime}')) as pass_qty,

MAX((SELECT count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT h 
where h.art =a.ART_NO  and a.PO_NO = h.PO and to_char(h.CREATE_TIME,'yyyy-MM-dd')  BETWEEN '{starttime}' AND '{endtime}'))as ng_qty
	from QCM_INSP_METAL_M a
	join bdm_rd_prod b on a.ART_NO =b.PROD_NO 
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
    LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
    LEFT JOIN base005m f on f.department_code = e.udf07
left join SJQDMS_ORGINFO g on c.udf05 = g.code
LEFT JOIN BASE001M b1 ON c.FACTORY_SAP = b1.ORG_CODE
	-- where a.PROCESSING_RES =0
    {Where}
    and a.CREATEDATE  BETWEEN '{starttime}' AND '{endtime}'
	group by b.PROD_NO
)
select 
NAME_T,pass_qty,ng_qty ,
case
when pass_qty+ng_qty = 0 then 0
else round(pass_qty/(pass_qty+ng_qty)*100,1)
END as pass_rate
from insp_data
where pass_qty>0
order by pass_rate desc";

                
                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'


                var dt = DB.GetDataTable(sql);



                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 不通过ART列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFailArtData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                //string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;


                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');

                    Where += $@"and f.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.PRO_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    arr = NAME_T.Split(',');
                    Where += $@"and b.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.NAME_T = '{NAME_T}' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(',');
                    Where += $@"and a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@" and a.PO_NO = '{PO_NO}'";
                }

                /*
                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    Where += $@"and d.M_TYPE like '%{M_TYPE}%'";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    Where += $@"and g.EN like '%{UDF05}%' ";
                }
                if (!string.IsNullOrEmpty(DEPARTMENT))
                {
                    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                }

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    Where += $@"and a.PRO_LINE_NO like '%{production_line_code}%' ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and b.NAME_T like '%{NAME_T}%' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    Where += $@" and a.PO_NO = '{PO_NO}'";
                }
                */
                string sql = $@"
SELECT DISTINCT tab.*,pass_qty+fail_qty as qty,
(
case 
when  pass_qty+fail_qty =0 then 0
else  round(pass_qty/(pass_qty+fail_qty) *100,1)
end
) as rate

from (
SELECT
  a.ART_NO as name_T,
  d.M_TYPE,
  (SELECT ORG_NAME from base001m where ORG_CODE in(
SELECT FACTORY_SAP from base005m where DEPARTMENT_CODE =a.PRO_LINE_NO
) ) as ORG_NAME,
	a.ART_NO,
	a.pro_line_no,
	a.PO_NO,
	d.SE_QTY,
(SELECT count(1) from ESM_MD_CHANLIANG_LIST@APEIOT g where g.art = a.ART_NO and g.PO = a.PO_NO) as pass_qty,
(SELECT count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT h where h.art =a.ART_NO  and a.PO_NO = h.PO) as fail_qty
FROM
	qcm_insp_metal_m a
LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
LEFT JOIN BDM_SE_ORDER_MASTER c on a.PO_NO = c.MER_PO
LEFT JOIN bdm_se_order_item d ON c .se_id = d.se_id
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
       -- LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
left join SJQDMS_ORGINFO g on c.udf05 = g.code
left join base001m f on c.FACTORY_SAP =f.ORG_CODE

where 1=1 
-- a.processing_res = '0'
and a.createdate between '{starttime}' and  '{endtime}'
{Where}

)tab
-- where pro_line_no = '10015PL02'
ORDER BY rate
";

                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 生产线列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductlineData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');

                    Where += $@"and f.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and g.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.PRO_LINE_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    arr = ART_NO.Split(',');
                    Where += $@"and a.ART_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    arr = NAME_T.Split(',');
                    Where += $@"and b.NAME_T in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@"and b.NAME_T = '{NAME_T}' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    arr = PO_NO.Split(',');
                    Where += $@"and a.PO_NO in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                    //Where += $@" and a.PO_NO = '{PO_NO}'";
                }

                /*
                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    Where += $@"and d.M_TYPE like '%{M_TYPE}%'";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    Where += $@"and g.EN like '%{UDF05}%' ";
                }
                if (!string.IsNullOrEmpty(DEPARTMENT))
                {
                    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                }

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    Where += $@"and a.PRO_LINE_NO like '%{production_line_code}%' ";
                }
                if (!string.IsNullOrEmpty(ART_NO))
                {
                    Where += $@"and a.ART_NO = '{ART_NO}' ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and b.NAME_T like '%{NAME_T}%' ";
                }

                if (!string.IsNullOrEmpty(PO_NO))
                {
                    Where += $@" and a.PO_NO = '{PO_NO}'";
                }
                */

                string sql = $@"
SELECT M_TYPE,ORG_NAME,PRO_LINE_NO,PRO_LINE_NAME,SUM(SE_QTY) as SE_QTY,SUM(PASS_QTY) as PASS_QTY,SUM(FAIL_QTY) as FAIL_QTY,SUM(QTY) as QTY,''as RATE from (

SELECT DISTINCT tab.*,pass_qty+fail_qty as qty

from (
SELECT
	b.name_T,
  d.M_TYPE,
  (SELECT ORG_NAME from base001m where ORG_CODE in(
SELECT FACTORY_SAP from base005m where DEPARTMENT_CODE =a.PRO_LINE_NO
) ) as ORG_NAME,
	a.ART_NO,
a.PRO_LINE_NO,
    a.PRO_LINE_NAME,
	a.PO_NO,
	d.SE_QTY,
(SELECT count(1) from ESM_MD_CHANLIANG_LIST@APEIOT g where g.art = a.ART_NO and g.PO = a.PO_NO and g.LINE_NAME = a.PRO_LINE_NAME) as pass_qty,
(SELECT count(1) from ESM_MD_DUANZHENXRAY_LIST@APEIOT h where h.art =a.ART_NO and a.PO_NO = h.PO and h.LINE_NAME = a.PRO_LINE_NAME) as fail_qty
FROM
	qcm_insp_metal_m a
LEFT JOIN BDM_RD_STYLE b on a.SHOE_NO=b.SHOE_NO
LEFT JOIN BDM_SE_ORDER_MASTER c on a.PO_NO = c.MER_PO
LEFT JOIN bdm_se_order_item d ON c .se_id = d.se_id
    LEFT join base005m c on c.DEPARTMENT_CODE = a.PRO_LINE_NO
    LEFT join base003m d on c.FACTORY_SAP = ltrim(d.SUPPLIERS_CODE,'0')
    LEFT JOIN base005m e on a.PRO_LINE_NO = e.udf07
left join SJQDMS_ORGINFO g on c.udf05 = g.code
left join base001m f on c.FACTORY_SAP =f.ORG_CODE
Where 1=1
{Where}
and a.createdate between '{starttime}' and  '{endtime}'
 -- where a.processing_res = '0'

)tab
where tab.PRO_LINE_NO='10015PL02'
)tt
group by ORG_NAME,M_TYPE,PRO_LINE_NO,PRO_LINE_NAME

ORDER BY rate
";
                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

              var dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    decimal pass_qty = Convert.ToDecimal(item["PASS_QTY"].ToString());
                    decimal fail_qty = Convert.ToDecimal(item["FAIL_QTY"].ToString());

                    if (pass_qty + fail_qty == 0)
                    {
                        item["RATE"] = "0";
                    }
                    else
                    {
                        item["RATE"] = Math.Round(pass_qty / (pass_qty + fail_qty) * 100, 1);
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 饼图（车针管控）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDZDataByPie(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;


                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');

                    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and d.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and b.production_line_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(ART_NO))
                //{
                //    Where += $@"and a.ART_NO = '{ART_NO}' ";
                //}
                //if (!string.IsNullOrEmpty(NAME_T))
                //{
                //    Where += $@"and b.NAME_T = '{NAME_T}' ";
                //}

                //if (!string.IsNullOrEmpty(PO_NO))
                //{
                //    Where += $@" and a.PO_NO = '{PO_NO}'";
                //}


                /*
                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    Where += $@"and e.M_TYPE like '%{M_TYPE}%'";
                }
                if (!string.IsNullOrEmpty(ORG))
                {
                    Where += $@"and  d.ORG  like '%{ORG}%' ";
                }
                if (!string.IsNullOrEmpty(PRODUCTION_LINE_NAME))
                {
                    Where += $@" and b.PRODUCTION_LINE_NAME like '%{PRODUCTION_LINE_NAME}%' ";
                }

                //and d.ORG like '%%' and b.PRODUCTION_LINE_NAME like '%%' and e.M_TYPE = ''
                */

                string sql = $@"
select b.PRODUCTION_LINE_NAME ,sum(a.COLLAR_QTY) COLLAR_QTY
from qcm_car_needle_c a
join qcm_car_needle_m b on a.M_ID =b.ID 

LEFT JOIN base005m c on c.DEPARTMENT_CODE = b.PRODUCTION_LINE_CODE
left join SJQDMS_ORGINFO d on c.udf05=d.code
LEFT join base003m e on c.FACTORY_SAP = ltrim(e.SUPPLIERS_CODE,'0')
LEFT JOIN BASE001M b1 ON c.FACTORY_SAP = b1.ORG_CODE
where a.CREATEDATE between '{starttime}' and '{endtime}'
{Where}
and a.OPA_TYPE =2

group by  b.PRODUCTION_LINE_NAME
order by sum(a.COLLAR_QTY) desc
";

                //a.CREATEDATE between '{dicTime["starttime"]}' and '{dicTime["endtime"]}'

                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 柱状图_车针管控
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDZDataByhistogram(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //string OrderByStr = jarr.ContainsKey("OrderByStr") ? jarr["OrderByStr"].ToString() : "";

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件



                string Where = string.Empty;

                //if (!string.IsNullOrEmpty(M_TYPE))
                //{
                //    Where += $@"and e.M_TYPE like '%{M_TYPE}%'";
                //}
                //if (!string.IsNullOrEmpty(ORG))
                //{
                //    Where += $@"and  d.ORG  like '%{ORG}%' ";
                //}
                //if (!string.IsNullOrEmpty(PRODUCTION_LINE_NAME))
                //{
                //    Where += $@" and b.PRODUCTION_LINE_NAME like '%{PRODUCTION_LINE_NAME}%' ";
                //}

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    arr = M_TYPE.Split(',');

                    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))
                {
                    arr = UDF05.Split(',');
                    Where += $@"and d.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and b.production_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

                //and d.ORG like '%%' and b.PRODUCTION_LINE_NAME like '%%' and e.M_TYPE = ''

                List<string> Xdata = new List<string>();

                List<KanBanDtos> res = new List<KanBanDtos>();

                KanBanDtos data_online = new KanBanDtos();
                data_online.type = "bar";
                data_online.name = "Number of online uses";

                KanBanDtos data_unline = new KanBanDtos();
                data_unline.type = "bar";
                data_unline.name = "Number of broken needles";

                KanBanDtos data_resqty = new KanBanDtos();
                data_resqty.type = "bar";
                data_resqty.name = "Remaining needles";

                KanBanDtos data_lyqty = new KanBanDtos();
                data_lyqty.type = "bar";
                data_lyqty.name = "Quantity of needle Received";

                List<string> list_online = new List<string>();
                List<string> list_unline = new List<string>();
                List<string> list_resqty = new List<string>();
                List<string> list_lyqty = new List<string>();

                string sql = $@"
with receive_tmp as (
	--领用
	select to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') createdate,
	sum(a.COLLAR_QTY) COLLAR_QTY
	from qcm_car_needle_c a
	join qcm_car_needle_m b on a.M_ID =b.ID 
LEFT JOIN base005m c on c.DEPARTMENT_CODE = b.PRODUCTION_LINE_CODE
left join SJQDMS_ORGINFO d on c.udf05=d.code
LEFT join base003m e on c.FACTORY_SAP = ltrim(e.SUPPLIERS_CODE,'0')
LEFT JOIN BASE001M b1 ON c.FACTORY_SAP = b1.ORG_CODE 
	where a.CREATEDATE between '{starttime}' and '{endtime}'
{Where}
	and a.OPA_TYPE ='0'
	group by to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')
),
send_tmp as (
	--发针
	select to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') createdate,
	sum(a.COLLAR_QTY) COLLAR_QTY
	from qcm_car_needle_c a
	join qcm_car_needle_m b on a.M_ID =b.ID 
	where a.CREATEDATE  between '{starttime}' and '{endtime}'
{Where}
	and a.OPA_TYPE ='1'
	group by to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')
),
break_tmp as (
	--断针
	select to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm') createdate,
	sum(a.COLLAR_QTY) COLLAR_QTY
	from qcm_car_needle_c a
	join qcm_car_needle_m b on a.M_ID =b.ID 
	where a.CREATEDATE  between '{starttime}' and '{endtime}'
{Where}
	and a.OPA_TYPE ='2'
	group by to_char(to_date(a.CREATEDATE,'yyyy-mm-dd'),'yyyy-mm')
)
select 
a.createdate,
nvl(a.COLLAR_QTY,0)-nvl(b.COLLAR_QTY,0) as remainder_qty,	--剩余数量
nvl(b.COLLAR_QTY,0)-nvl(c.COLLAR_QTY,0) as online_qty,		--在线数量
a.COLLAR_QTY,												--领用数量
c.COLLAR_QTY as DZQTY										--断针数量
from receive_tmp a
left join send_tmp b on a.createdate=b.CREATEDATE 
left join break_tmp c on a.createdate=c.CREATEDATE
";

                DateTime start = Convert.ToDateTime(starttime);
                DateTime end = Convert.ToDateTime(endtime);
                var dt = DB.GetDataTable(sql);
                int months = end.Month - start.Month + 1;

                //遍历N个月
                for (int i = 0; i < months; i++)
                {
                    Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));
                }

                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"createdate = '{item}'");
                    if (dr.Length > 0)
                    {
                        list_online.Add(dr[0]["online_qty"].ToString());
                        list_unline.Add(dr[0]["DZQTY"].ToString());
                        list_resqty.Add(dr[0]["remainder_qty"].ToString());
                        list_lyqty.Add(dr[0]["COLLAR_QTY"].ToString());

                    }
                    else
                    {
                        list_online.Add("0");
                        list_unline.Add("0");
                        list_resqty.Add("0");
                        list_lyqty.Add("0");
                    }
                }
                data_online.data = list_online;
                data_unline.data = list_unline;
                data_resqty.data = list_resqty;
                data_lyqty.data = list_lyqty;

                res.Add(data_online);
                res.Add(data_unline);
                res.Add(data_resqty);
                res.Add(data_lyqty);
                //res.Add(data_resqty);



                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);
                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 生产线列表(车针管控)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductlineDZData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                /*
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件  厂区类型
                string PLANT_AREA = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件  厂区
                string PRODUCTION_LINE_NAME = jarr.ContainsKey("PRODUCTION_LINE_NAME") ? jarr["PRODUCTION_LINE_NAME"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                */

                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 厂区类型
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 厂区

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty;



                /*
                if (!string.IsNullOrEmpty(M_TYPE))
                {
                    Where += $@"and A.ORG_NAME like '%{M_TYPE}%'";
                }
                if (!string.IsNullOrEmpty(PLANT_AREA))
                {
                    Where += $@"and C.EN  like '%{PLANT_AREA}%' ";
                }
                if (!string.IsNullOrEmpty(PRODUCTION_LINE_NAME))
                {
                    Where += $@" and A.PRODUCTION_LINE_NAME like '%{PRODUCTION_LINE_NAME}%' ";
                }*/

                string[] arr = new string[] { };

                if (!string.IsNullOrEmpty(M_TYPE))//厂区类型
                {
                    arr = M_TYPE.Split(',');
                    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                    //Where += $@"and e.M_TYPE in ({string.Join(',', arr.Select(x => $"'{x}'"))})";
                }
                if (!string.IsNullOrEmpty(UDF05))//厂区
                {
                    arr = UDF05.Split(',');
                    Where += $@"and c.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }
                //if (!string.IsNullOrEmpty(DEPARTMENT))
                //{
                //    Where += $@" and f.DEPARTMENT_NAME like '%{DEPARTMENT}%' ";
                //}

                if (!string.IsNullOrEmpty(production_line_code))//生产线
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.production_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

                #region old
                /*
                                string sql = $@"
                with depart_tmp as (
                        select c.ORG_NAME ,PRODUCTION_LINE_CODE,PRODUCTION_LINE_NAME,PLANT_AREA 
                            from bdm_production_line_m a
                            left join base005m b on a.PRODUCTION_LINE_CODE =b.DEPARTMENT_CODE 
                            left join base001m c on b.FACTORY_SAP =c.ORG_CODE 
                            union all
                            select c.ORG_NAME ,DEPARTMENT_CODE,DEPARTMENT_NAME,org from base005m a
                            left join SJQDMS_ORGINFO b on a.udf05=b.code
                            left join base001m c on a.FACTORY_SAP =c.ORG_CODE
                ),
                receive_tmp as (
                    --领用   
                    select 
                    b.PRODUCTION_LINE_CODE,b.PRODUCTION_LINE_NAME,
                    sum(a.COLLAR_QTY) COLLAR_QTY
                    from qcm_car_needle_c a
                    join qcm_car_needle_m b on a.M_ID =b.ID 
                    where a.CREATEDATE between '{starttime}' and '{endtime}'
                    and a.OPA_TYPE ='0'
                    group by b.PRODUCTION_LINE_CODE,b.PRODUCTION_LINE_NAME
                ),
                send_tmp as (
                    --发针
                    select
                    b.PRODUCTION_LINE_CODE,
                    b.PRODUCTION_LINE_NAME, 
                    sum(a.COLLAR_QTY) COLLAR_QTY
                    from qcm_car_needle_c a
                    join qcm_car_needle_m b on a.M_ID =b.ID 
                    where a.CREATEDATE between '{starttime}' and '{endtime}'
                    and a.OPA_TYPE ='1'
                    group by b.PRODUCTION_LINE_CODE,b.PRODUCTION_LINE_NAME
                ),
                break_tmp as (
                    --断针
                    select 
                    b.PRODUCTION_LINE_CODE,
                    b.PRODUCTION_LINE_NAME, 
                    sum(a.COLLAR_QTY) COLLAR_QTY
                    from qcm_car_needle_c a
                    join qcm_car_needle_m b on a.M_ID =b.ID 
                    where a.CREATEDATE between '{starttime}' and '{endtime}'
                    and a.OPA_TYPE ='2'
                    group by b.PRODUCTION_LINE_CODE,b.PRODUCTION_LINE_NAME
                ),
                final_data as (
                    select 
                    nvl(d.ORG_NAME,'未知') ORG_NAME,																			--厂区分类
                    nvl(d.PLANT_AREA,'未知') PLANT_AREA,																		--厂区
                    a.PRODUCTION_LINE_NAME, 																					--生产线
                    nvl(a.COLLAR_QTY,0) receive_qty,																			--领用数量
                    nvl(b.COLLAR_QTY,0) send_qty,																				--发针数量
                    nvl(c.COLLAR_QTY,0) break_qty,																				--断针数量
                    nvl(a.COLLAR_QTY,0)-nvl(b.COLLAR_QTY,0) as remainder_qty,													--剩余数量
                    nvl(b.COLLAR_QTY,0)-nvl(c.COLLAR_QTY,0) as online_qty,														--在线数量
                    round(case when nvl(a.COLLAR_QTY,0)=0 then 0 else nvl(c.COLLAR_QTY,0)/nvl(a.COLLAR_QTY,0)*100 end,1) rate	--断针比率
                    from receive_tmp a
                    left join send_tmp b on a.PRODUCTION_LINE_NAME=b.PRODUCTION_LINE_NAME 
                    left join break_tmp c on a.PRODUCTION_LINE_NAME=c.PRODUCTION_LINE_NAME 
                  join depart_tmp d on a.PRODUCTION_LINE_CODE =d.PRODUCTION_LINE_CODE
                    where 1=1 
                {Where}
                )
                SELECT 	ROWNUM as RN, tt.* FROM(
                select * from final_data
                order by rate desc
                )tt
                ";*/
                #endregion
                string sql = $@"

SELECT EN as PLANT_AREA, ORG_CODE,ORG_NAME,PRODUCTION_LINE_CODE,PRODUCTION_LINE_NAME ,
ly_qty as receive_qty, -- 领用
fz_qty as send_qty,    -- 发针
dz_qty as break_qty,    -- 断针
ly_qty-fz_qty as remainder_qty,       --剩余数量
fz_qty-dz_qty as online_qty,           -- 在线数量
case when ly_qty=0 then 0 else round((dz_qty / ly_qty)*100,1) end as rate
from (
SELECT
     X.EN,
	 X.ORG_CODE,--厂区编号
	 X.ORG_NAME,
		X.PRODUCTION_LINE_CODE,--产线
		X.PRODUCTION_LINE_NAME,
		--X.NEEDLE_CATEGORY_NO,--针类型
		SUM(X.ly_qty)AS ly_qty,
SUM(X.fz_qty) AS fz_qty,
SUM(X.dz_qty) AS dz_qty,
SUM(X.hz_qty) AS hz_qty
FROM (
SELECT
    C.EN,
     A.ORG_CODE,--厂区编号
		A.ORG_NAME,
		A.PRODUCTION_LINE_CODE,--产线
		A.PRODUCTION_LINE_NAME,--产线
		--A.NEEDLE_CATEGORY_NO,--针类型
		B.ID,
		max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=0 then B.COLLAR_QTY
 END) AS ly_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=1 then B.COLLAR_QTY
 END) AS fz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=2 then B.COLLAR_QTY
 END) AS dz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=3 then B.COLLAR_QTY
 END) AS hz_qty
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
LEFT JOIN base005m O on O.DEPARTMENT_CODE = A.production_line_code
 LEFT JOIN SJQDMS_ORGINFO C on C.CODE = O.UDF05
LEFT JOIN BASE001M b1 ON O.FACTORY_SAP = b1.ORG_CODE
WHERE 1=1  and B.CREATEDATE between '{starttime}' and '{endtime}'{Where}
GROUP BY C.EN, A.ORG_CODE,A.ORG_NAME,A.PRODUCTION_LINE_CODE,A.PRODUCTION_LINE_NAME,b.ID) X GROUP BY X.EN,X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.ORG_NAME,X.PRODUCTION_LINE_NAME

)tt
";
                

                var dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductlineMetalData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data); 
                string M_TYPE = jarr.ContainsKey("M_TYPE") ? jarr["M_TYPE"].ToString() : "";//查询条件 厂区类型
                string UDF05 = jarr.ContainsKey("UDF05") ? jarr["UDF05"].ToString() : "";//查询条件 厂区

                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//查询条件 

                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//查询条件 

                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 
                string PO_NO = jarr.ContainsKey("PO_NO") ? jarr["PO_NO"].ToString() : "";//查询条件 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string Where = string.Empty; 

                string[] arr = new string[] { };

                //if (!string.IsNullOrEmpty(M_TYPE))//厂区类型
                //{
                //    arr = M_TYPE.Split(',');
                //    Where += $@"and b1.ORG_NAME in ({string.Join(',', arr.Select(x => $"'{x}'"))})"; 
                //}
                //if (!string.IsNullOrEmpty(UDF05))//厂区
                //{
                //    arr = UDF05.Split(',');
                //    Where += $@"and c.EN in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                //} 

                if (!string.IsNullOrEmpty(production_line_code))//生产线
                {
                    arr = production_line_code.Split(',');
                    Where += $@"and a.pro_line_name in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";
                }

        
                string sql = $@"

with tmp as
 (select  a.pro_line_no,
         count(*) as Total_Qty,
         sum(case
               when a.processing_res = '1' then
                1
               else
                0
             end) as Pass_Qty,
         sum(case
               when a.processing_res = '0' then
                1
               else
                0
             end) as fail_Qty
  
    from qcm_insp_metal_m a where 1=1 {Where} and a.createdate between '{starttime}' and '{endtime}'
   group by a.pro_line_no ),

tmp2 as
 (SELECT pro_line_no,
         MAX(CASE
               WHEN ranking = 1 THEN
                problem_point_desc
             END) AS problem_point_desc_1,
         MAX(CASE
               WHEN ranking = 1 THEN
                total
             END) AS total_1,
         MAX(CASE
               WHEN ranking = 2 THEN
                problem_point_desc
             END) AS problem_point_desc_2,
         MAX(CASE
               WHEN ranking = 2 THEN
                total
             END) AS total_2,
         MAX(CASE
               WHEN ranking = 3 THEN
                problem_point_desc
             END) AS problem_point_desc_3,
         MAX(CASE
               WHEN ranking = 3 THEN
                total
             END) AS total_3
    from (select  
                 a.pro_line_no,
                 a.problem_point_desc,
                 count(1) as total,
                 ROW_NUMBER() over(partition by a.pro_line_no ORDER BY count(1) DESC) ranking
             from qcm_insp_metal_m a where 1=1 {Where} and a.createdate between '{starttime}' and '{endtime}'
           group by a.pro_line_no, a.problem_point_desc)
   group by pro_line_no)

select tmp.pro_line_no,
       tmp.total_qty,
       tmp.pass_qty,
       tmp.fail_qty,
       tmp2.problem_point_desc_1,
       tmp2.total_1,
       tmp2.problem_point_desc_2,
       tmp2.total_2,
       tmp2.problem_point_desc_3,
       tmp2.total_3
  from tmp
  join tmp2
    on tmp.pro_line_no = tmp2.pro_line_no
"; 

                var dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        #endregion

        #region 市场反馈看板-中国市场退货分析 

        /// <summary>
        /// 中国市场图标占比对比图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCNByhistogramData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//语言
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要退货原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 
                string starttime = string.Empty;
                string endtime = string.Empty;
                //string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                //string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime}) ";

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@"and c.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN LIKE '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份
                 */

                DateTime start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-01-01"));
                DateTime end = Convert.ToDateTime(LastDayOfYear(DateTime.Now).ToString("yyyy-MM-dd"));
                int months = end.Month - start.Month + 1;

                starttime = start.ToString("yyyy-MM-dd");
                endtime = end.ToString("yyyy-MM-dd");

                List<KanBanDtos> res = new List<KanBanDtos>();
                var lanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code3, new List<string>() { "去年退货数量" , "今年退货数量" });

                List<string> Xdata = new List<string>();
                KanBanDtos data_Last = new KanBanDtos();
                data_Last.type = "bar";
                data_Last.name = lanDic["去年退货数量"].ToString();
                KanBanDtos data_now = new KanBanDtos();
                data_now.type = "bar";
                data_now.name = lanDic["今年退货数量"].ToString();

                //遍历N个月
                for (int i = 0; i < months; i++)
                {
                    Xdata.Add(start.AddMonths(i).ToString("yyyy-MM"));

                }

                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
select 
a.RETURN_MONTH createdate,
sum(a.NEWSHOES_QTY)+sum(a.OLDSHOES_QTY) return_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1'
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
LEFT JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by a.RETURN_MONTH
order by a.RETURN_MONTH
";
                var dt = DB.GetDataTable(sql);//今年

                starttime =start.AddYears(-1).ToString("yyyy-01-01");
                endtime = LastDayOfYear(end.AddYears(-1)).ToString("yyyy-MM-dd");
                sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
select 
a.RETURN_MONTH as createdate,
sum(a.NEWSHOES_QTY)+sum(a.OLDSHOES_QTY) return_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1'
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
LEFT JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by a.RETURN_MONTH
order by a.RETURN_MONTH";

                var dt2 = DB.GetDataTable(sql);//去年


                List<string> datalast = new List<string>();
                foreach (var item in Xdata)
                {
                    var dr = dt2.Select($"createdate = '{item}'");
                    if (dr.Length > 0)
                    {
                        datalast.Add(dr[0]["return_qty"].ToString());
                    }
                    else
                    {
                        datalast.Add("0");
                    }
                }
                data_Last.data = datalast;

                List<string> dataNow = new List<string>();//今年
                foreach (var item in Xdata)
                {
                    var dr = dt.Select($"createdate = '{item}'");
                    if (dr.Length > 0)
                    {
                        dataNow.Add(dr[0]["return_qty"].ToString());
                    }
                    else
                    {
                        dataNow.Add("0");
                    }
                }
                data_now.data = dataNow;

                res.Add(data_Last);
                res.Add(data_now);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", res);
                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 退货原因主要占比
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCNReturnReasonData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;
                //string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                //string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');

                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }

                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and l.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and b.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                //if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }

                //【厂区、生产线、生产月份】


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
select tt.* from (
select  nvl(b.CONTENT_CN,'未知') CONTENT_CN ,
sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
from qcm_market_feedback_m a
 join bdm_r_main_d_code b on a.MAIN_CODE =b.MAIN_CODE 

 JOIN BDM_SE_ORDER_MASTER c on a.po=c.mer_po
 JOIN BDM_SE_ORDER_ITEM d ON c.SE_ID = d.SE_ID
 JOIN bdm_rd_prod r ON d.prod_no = r.PROD_NO
 JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
 JOIN bdm_cd_code bb ON l.style_seq=bb.code_no and language = '1'
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and b.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = d.SE_ID

where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by b.CONTENT_CN
order by sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) desc
)tt
where return_qty is not null
";


                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// CATEGORY占比
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCNCategoryData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }

                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
select e.NAME_T category,
sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
LEFT JOIN bdm_rd_prod r ON c.prod_no = r.PROD_NO
join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
left join BDM_CD_CODE e on d.STYLE_SEQ =e.CODE_NO and LANGUAGE ='1'

LEFT JOIN bdm_cd_code bb ON d.style_seq=bb.code_no

 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by e.NAME_T
order by sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY, 0)) desc";


                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// art占比
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCNARTData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
SELECT * from (
SELECT  ROWNUM as RN ,tt.* from (
select r.PROD_NO category,
sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
LEFT JOIN bdm_rd_prod r ON c.prod_no = r.PROD_NO

 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1'
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by r.PROD_NO
order by sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) desc
)tt
)yy
where yy.RN<11
";


                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 新旧鞋占比
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetNeworOldData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件

                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and d.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and l.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%' ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%' ";
                }
                string str1 = "sum(NEW_QTY) as new_qty,sum(OLD_QTY) as old_qty";
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        str1 = "sum(NEW_QTY) as new_qty ";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        str1 = "sum(OLD_QTY) as old_qty ";
                    }
                }


                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份

                 */



                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)

SELECT {str1} from (
select DISTINCT
nvl(NEWSHOES_QTY,0) new_qty,
nvl(OLDSHOES_QTY,0) old_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
join bdm_rd_prod d on c.PROD_NO  =d.PROD_NO  
join bdm_rd_style l on c.SHOE_NO =d.SHOE_NO 

 JOIN bdm_cd_code bb ON l.style_seq=bb.code_no and language = '1'
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
)tt
";


                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 厂区退货数量占比
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCQReturnData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;


                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'   ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'   ";
                }
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }

                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份

                 */



                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
),
 depart_tmp as (
	select c.ORG_NAME ,PRODUCTION_LINE_CODE,PRODUCTION_LINE_NAME,PLANT_AREA 
	from bdm_production_line_m a
	left join base005m b on a.PRODUCTION_LINE_CODE =b.DEPARTMENT_CODE 
	left join base001m c on b.FACTORY_SAP =c.ORG_CODE 
	union all
	select c.ORG_NAME ,DEPARTMENT_CODE,DEPARTMENT_NAME,org from base005m a
	left join SJQDMS_ORGINFO b on a.udf05=b.code
	left join base001m c on a.FACTORY_SAP =c.ORG_CODE
)
	select PLANT_AREA,
	sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
	from qcm_market_feedback_m a
	join bdm_se_order_master b on a.PO =b.MER_PO 
	join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
    join bdm_rd_prod r on c.PROD_NO  =r.PROD_NO 
	join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
	join sfc_trackin_list e on a.PO =e.PO_NO 
	join depart_tmp f on e.GRT_DEPT=f.PRODUCTION_LINE_CODE
    
 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1' 
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
	group by PLANT_AREA
	order by return_qty desc
";


                var dt = DB.GetDataTable(sql);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                ret.IsSuccess = true;
                ret.RetData1 = dt;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 鞋型退货数量展示图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoesData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;

                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }

                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@"and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份

                 */
                List<string> Xdata = new List<string>();
                KanBanDtos data_res = new KanBanDtos();
                data_res.type = "bar";
                data_res.name = "退货数量";


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
)
select d.NAME_T category,
sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
from qcm_market_feedback_m a
join bdm_se_order_master b on a.PO =b.MER_PO 
join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
join bdm_rd_prod r on c.PROD_NO  =r.PROD_NO
join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 

 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1'
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
LEFT JOIN SEID_tmp z on z.se_id = c.SE_ID
where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
group by d.NAME_T
order by sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) desc
";


                var dt = DB.GetDataTable(sql);
                List<string> data = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    Xdata.Add(item["category"].ToString());
                    data.Add(item["return_qty"].ToString());

                }
                data_res.data = data;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata",Xdata);
                dic.Add("Data", data_res);
                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 退货率
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoesReturnRateData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');

                string Where = string.Empty;
                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if (orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }


                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@" and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份

                 */
                List<string> Xdata = new List<string>();
                KanBanDtos data_res = new KanBanDtos();
                data_res.type = "bar";
                data_res.name = "退货率";


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
),
 tmp as (
	select d.NAME_T category,
	sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty,
	sum(nvl(f.SHIPPING_QTY ,0)) SHIPPING_QTY
	from qcm_market_feedback_m a
	join bdm_se_order_master b on a.PO =b.MER_PO 
	join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
    join bdm_rd_prod r on c.PROD_NO  =r.PROD_NO
	join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
	left join bmd_se_shipment_m e on a.PO =e.PO_NO 
	left join bmd_se_shipment_d f on e.SHIPPING_NO =f.SHIPPING_NO 

 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1' 
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = c.SE_ID

where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
	group by d.NAME_T
)
select CATEGORY,RETURN_QTY,SHIPPING_QTY ,
round(case when nvl(a.SHIPPING_QTY,0)=0 then 0 else nvl(a.RETURN_QTY,0)/nvl(a.SHIPPING_QTY,0)*100 end,1) rate
from tmp a
";


                var dt = DB.GetDataTable(sql);

                DataTable RetDt = dt.Clone();
                DataView DV = new DataView(dt);
                DV.Sort = $@" rate desc";
                dt = DV.ToTable();

                List<string> data = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    Xdata.Add(item["CATEGORY"].ToString());
                    data.Add(item["rate"].ToString());

                }
                data_res.data = data;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", data_res);
                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 生产线退货数
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductlineReturnData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 
                string NAME_T = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//查询条件 鞋型名称 
                string CATEGORY = jarr.ContainsKey("CATEGORY") ? jarr["CATEGORY"].ToString() : "";//查询条件 

                string CONTENT_CN = jarr.ContainsKey("CONTENT_CN") ? jarr["CONTENT_CN"].ToString() : "";//主要原因 

                string CONTENT_CN_C = jarr.ContainsKey("CONTENT_CN_C") ? jarr["CONTENT_CN_C"].ToString() : "";//主要原因 
                string SHOE_TYPE = jarr.ContainsKey("SHOE_TYPE") ? jarr["SHOE_TYPE"].ToString() : "";//鞋类型 0-新 1-旧 
                //List<string> PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["PRODUCTION_MONTH"].ToString()) : new List<string>();//生产月份 
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString() : "";//生产月份
                string PRODUCTION_MONTH_START = string.Empty;
                string PRODUCTION_MONTH_END = string.Empty;

                string ORG = jarr.ContainsKey("ORG") ? jarr["ORG"].ToString() : "";//厂区 
                string PRO_LINE_NAME = jarr.ContainsKey("PRO_LINE_NAME") ? jarr["PRO_LINE_NAME"].ToString() : "";//生产线 

                string starttime = jarr.ContainsKey("STARTTIME") ? jarr["STARTTIME"].ToString() : "";//查询条件 
                string endtime = jarr.ContainsKey("ENDTIME") ? jarr["ENDTIME"].ToString() : "";//查询条件
                string[] orderListart = PROD_NO.Split(',');
                string[] orderListshoeName = NAME_T.Split(',');
                string[] orderListcategory = CATEGORY.Split(',');
                string[] orderListMonth = PRODUCTION_MONTH.Split(',');
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    if(orderListMonth.Length > 1)
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = orderListMonth[1];
                    }
                    else
                    {
                        PRODUCTION_MONTH_START = orderListMonth[0];
                        PRODUCTION_MONTH_END = DateTime.Now.ToString("yyyy-MM");
                    }



                    //选择日期中 包含 生产时间范围
                    string WhereTime = $@"or (  z.MIN_INSERT_DATE<= '{PRODUCTION_MONTH_START}' and  z.MAX_INSERT_DATE >= '{PRODUCTION_MONTH_END}')";


                    //交集
                    Where += $@"and (  ( z.MIN_INSERT_DATE>='{PRODUCTION_MONTH_END}' or z.MAX_INSERT_DATE <='{PRODUCTION_MONTH_START}')  {WhereTime})";

                }

                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    Where += $@"and r.prod_no in ({string.Join(',', orderListart.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(NAME_T))
                {
                    Where += $@"and d.NAME_T in ({string.Join(',', orderListshoeName.Select(x => $"'{x}'"))}) ";
                }
                if (!string.IsNullOrEmpty(CATEGORY))
                {
                    Where += $@"and bb.NAME_T in ({string.Join(',', orderListcategory.Select(x => $"'{x}'"))}) ";
                }

                if (!string.IsNullOrEmpty(CONTENT_CN))
                {
                    Where += $@"and h.CONTENT_CN like '%{CONTENT_CN}%'  ";
                }
                if (!string.IsNullOrEmpty(CONTENT_CN_C))
                {
                    Where += $@"and g.CONTENT_CN like '%{CONTENT_CN_C}%'  ";
                }

                //if (!string.IsNullOrEmpty(CONTENT_CN_C))
                //{
                //    Where += $@"and g.CONTENT_CN = '{CONTENT_CN_C}'  ";
                //}
                if (!string.IsNullOrEmpty(SHOE_TYPE))
                {
                    //新
                    if (SHOE_TYPE == "0")
                    {
                        Where += $@" and a.newshoes_qty >0";
                    }
                    if (SHOE_TYPE == "1")
                    {
                        Where += $@" and a.oldshoes_qty >0";
                    }
                }
                //【厂区、生产线、生产月份】
                /*
                  (SELECT to_char(MIN(INSERT_DATE), 'yyyy-mm')||' '||to_char(MAX(INSERT_DATE), 'yyyy-mm') FROM SFC_TRACKOUT_LIST where SE_ID = m.SE_ID and PROCESS_NO = 'A') as production_month, --生产月份

                 */
                List<string> Xdata = new List<string>();
                KanBanDtos data_res = new KanBanDtos();
                data_res.type = "bar";
                data_res.name = "退货数量";


                string sql = $@"
with SEID_tmp as(
SELECT 
	SE_ID,
to_char(MIN(INSERT_DATE), 'yyyy-mm') as MIN_INSERT_DATE,
to_char(MAX(INSERT_DATE), 'yyyy-mm') as MAX_INSERT_DATE
FROM SFC_TRACKOUT_LIST where  PROCESS_NO = 'A'
GROUP BY SE_ID
),
 depart_tmp as (
	select c.ORG_NAME ,PRODUCTION_LINE_CODE,PRODUCTION_LINE_NAME,PLANT_AREA 
	from bdm_production_line_m a
	left join base005m b on a.PRODUCTION_LINE_CODE =b.DEPARTMENT_CODE 
	left join base001m c on b.FACTORY_SAP =c.ORG_CODE 
	union all
	select c.ORG_NAME ,DEPARTMENT_CODE,DEPARTMENT_NAME,org from base005m a
	 join SJQDMS_ORGINFO b on a.udf05=b.code
	 join base001m c on a.FACTORY_SAP =c.ORG_CODE
)

	select PRODUCTION_LINE_NAME,
	sum(nvl(NEWSHOES_QTY,0)+nvl(OLDSHOES_QTY,0)) return_qty
	from qcm_market_feedback_m a
	join bdm_se_order_master b on a.PO =b.MER_PO 
	join BDM_SE_ORDER_ITEM c on b.SE_ID =c.SE_ID 
    join bdm_rd_prod r on c.PROD_NO  =r.PROD_NO
	join bdm_rd_style d on c.SHOE_NO =d.SHOE_NO 
	join sfc_trackin_list e on a.PO =e.PO_NO 
	join depart_tmp f on e.GRT_DEPT=f.PRODUCTION_LINE_CODE


 JOIN bdm_cd_code bb ON d.style_seq=bb.code_no and language = '1' 
 JOIN bdm_r_main_d_code h on a.main_code = h.MAIN_CODE
 join bdm_r_minor_d_code g on a.minor_code=g.minor_code and h.main_code=g.main_code
 JOIN SEID_tmp z on z.se_id = b.SE_ID

where a.RETURN_MONTH between '{DateTime.Parse(starttime).ToString("yyyy-MM")}' and '{DateTime.Parse(endtime).ToString("yyyy-MM")}'
{Where}
	group by PRODUCTION_LINE_NAME
	order by return_qty desc
";


                var dt = DB.GetDataTable(sql);
                List<string> data = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    Xdata.Add(item["PRODUCTION_LINE_NAME"].ToString());
                    data.Add(item["return_qty"].ToString());

                }
                data_res.data = data;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Xdata", Xdata);
                dic.Add("Data", data_res);
                ret.IsSuccess = true;
                ret.RetData1 = dic;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        #endregion
        /// <summary>
        /// 品质异常
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject QualityFailReported(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//查询条件 ART
                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//查询条件 PO
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                
                //string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息

                string where = string.Empty;

                if (!string.IsNullOrEmpty(start_date) && !string.IsNullOrEmpty(end_date))
                {
                    where += $@"AND ( a.createdate  BETWEEN '{start_date}' AND '{end_date}') ";

                }
                if (!string.IsNullOrEmpty(PO_ORDER))
                {
                    where += $@"AND a.po_list like '%{PO_ORDER}%' ";

                }

                //0：结案；1：未结案
                string sql = $@"SELECT DISTINCT
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
	a.fx_qty,--翻箱数量
	a.problem_desc,--问题描述
	a.department_code,--责任部门编号
	c.department_name,--责任部门
	a.department_codecc,--问题详情
    a.createby,
    ( 
case 
when a.closing_status = 0 then '结案'
when a.closing_status = 1 then '未结案'
end
) as closing_status
FROM
	qcm_abnormal_r_m a 
left join BDM_PARAM_ITEM_D b on a.workshop_section_no=b.workshop_section_no
LEFT JOIN  bdm_rd_prod r on a.prod_no=r.prod_no
left join base005m c on a.department_code=c.department_code
left join bdm_production_line_m d on a.production_line_code=d.production_line_code
left join bdm_qa_a_cate_m e on a.abnormal_category_no=e.abnormal_category_no
LEFT JOIN BDM_RD_STYLE f on a.SHOE_NO=f.SHOE_NO
where 1=1
and a.prod_no = '{PROD_NO}'
{where}";
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
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data",dt);

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


        /// <summary>
        /// PO信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Edit_PO(object OBJ)
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
                string keycode = jarr.ContainsKey("keycode") ? jarr["keycode"].ToString() : "";//查询条件 
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//art
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and m.mer_po like @keycode";
                }
                if (!string.IsNullOrWhiteSpace(art))
                {
                    where += $@" and r.prod_no=@prod_no";
                }

                string sql = string.Empty;
                sql = $@"	select m.mer_po,r.prod_no from BDM_SE_ORDER_MASTER m
                        LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
						where r.prod_no is not null and M.SE_TYPE in('ZOR1','ZOR2')  {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keycode", $@"%{keycode}%");
                paramTestDic.Add("prod_no", $@"{art}");
                DataTable dt1 = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt1);
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
        /// 转换 获取 月初和月末日期
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetTimeDic(string starttime, string endtime)
        {
            List<string> list = new List<string>();

            DateTime s = DateTime.Parse(endtime + " 23:59:59");

            var dd = DateTime.Parse(endtime + " 23:59:59").AddDays(1 - s.Day).AddMonths(1).AddDays(-1).ToString("dd");

            starttime = Convert.ToDateTime(starttime).ToString("yyyy-MM-01");
            endtime = Convert.ToDateTime(endtime).ToString($"yyyy-MM-{dd}");

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("starttime", starttime);
            dic.Add("endtime", endtime);
            return dic;
        }
        /// <summary>  
        /// 取得当前年的最后天  
        /// </summary>  
        /// <param name="datetime">要取得月份第一天的时间</param>  
        /// <returns></returns>  
        private static DateTime LastDayOfYear(DateTime datetime)
        {
            DateTime AssemblDate = Convert.ToDateTime(datetime.AddYears(1).Year + "-" + "01" + "-" + "01");  // 组装当前指定月份
            return AssemblDate.AddDays(-1);
        }

        /*
        /// <summary>
        /// 导出EXCEL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataList">数据集</param>
        /// <param name="headers">表头</param>
        /// <param name="FileName">文件名</param>
        /// <returns></returns>
        public static string CreateExcelFromList<T>(List<T> dataList, string[] headers, string FileName)
        {

            string sWebRootFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot\\tempExcel");//如果用浏览器url下载的方式  存放excel的文件夹一定要建在网站首页的同级目录下！！！
            if (!Directory.Exists(sWebRootFolder))
            {
                Directory.CreateDirectory(sWebRootFolder);
            }
            string sFileName = $@"{FileName}{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;//5.0之后的epplus需要指定 商业证书 或者非商业证书。低版本不需要此行代码
            using (ExcelPackage package = new ExcelPackage(file))
            {

                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                worksheet.Cells.LoadFromCollection(dataList, true);

                //表头字段
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                //for (int i = 0; i < headers.Length + 1; i++)
                //{//删除不需要的列
                //    string aa = worksheet.Cells[1, i + 1].Value.ToString();
                //    if (aa == "总行数")
                //    {
                //        worksheet.DeleteColumn(i + 1);
                //    }
                //}

                package.Save();
            }
            return path;//这是返回文件的方式
                        //return sFileName ;    //如果用浏览器url下载的方式  这里直接返回生成的文件名就可以了
        }

        */
    }
}

