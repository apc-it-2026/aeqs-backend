using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    public class DQA_ShoeShape
    {
        /// <summary>
        /// DQA鞋型管理主页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAMain(object OBJ)
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
                string PRODUCT_MONTH = jarr.ContainsKey("PRODUCT_MONTH") ? jarr["PRODUCT_MONTH"].ToString() : "";//查询条件 量产月份
                string DEVELOP_SEASON = jarr.ContainsKey("DEVELOP_SEASON") ? jarr["DEVELOP_SEASON"].ToString() : "";//查询条件 季度
                string user_section = jarr.ContainsKey("user_section") ? jarr["user_section"].ToString() : "";//查询条件 开发课
                string rule_no = jarr.ContainsKey("rule_no") ? jarr["rule_no"].ToString() : "";//查询条件 Category
                string cwa_date = jarr.ContainsKey("cwa_date") ? jarr["cwa_date"].ToString() : "";//查询条件 cwa日期
                string qa_principal = jarr.ContainsKey("qa_principal") ? jarr["qa_principal"].ToString() : "";//查询条件 qa负责人
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(PROD_NO))
                {
                    where += $@" and PROD_NO like @PROD_NO ";
                }
                if (!string.IsNullOrWhiteSpace(SHOE_NO))
                {
                    where += $@" and d.NAME_T like @SHOE_NO ";
                }
                if (!string.IsNullOrWhiteSpace(PRODUCT_MONTH))
                {
                    where += $@" and PRODUCT_MONTH like @PRODUCT_MONTH ";
                }
                if (!string.IsNullOrWhiteSpace(DEVELOP_SEASON))
                {
                    where += $@" and b.DEVELOP_SEASON like @DEVELOP_SEASON ";
                }
                if (!string.IsNullOrWhiteSpace(user_section))
                {
                    where += $@" and b.user_section like @user_section ";
                }
                if (!string.IsNullOrWhiteSpace(rule_no))
                {
                    where += $@" and bb.name_t like @rule_no ";
                }
                if (!string.IsNullOrWhiteSpace(cwa_date))
                {
                    where += $@" and to_char(b.cwa_date,'YYYY/MM/dd') like @cwa_date ";
                }
                if (!string.IsNullOrWhiteSpace(qa_principal))
                {
                    where += $@" and e.qa_principal like @qa_principal ";
                }

                //string sql = $@"SELECT
                //                MAX(develop_season) AS develop_season,
                //                LISTAGG ( to_char( PROD_NO ), ',' ) WITHIN GROUP ( ORDER BY SHOE_NO ) as PROD_NO,
                //                 SHOE_NO,
                //                    MAX(user_section) AS user_section,
                //                    MAX(bom_date) AS bom_date,
                //                    MAX(cwa_date) AS cwa_date,
                //                    MAX(user_fdd) AS user_fdd,
                //                 MAX(PRODUCT_MONTH) as PRODUCT_MONTH
                //                FROM
                //                 bdm_rd_prod 
                //                 where 1=1 {where}
                //                 GROUP BY
                //                 SHOE_NO";

                string sql = $@" SELECT
	                                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.PROD_NO", "b.PROD_NO")} AS PROD_NO,
	                                b.SHOE_NO,
	                                MAX (b.PRODUCT_MONTH) AS PRODUCT_MONTH,
	                                MAX (q.image_guid) AS image_guid,
                                    MAX(T.file_url) AS file_url,
	                                MAX (b.DEVELOP_SEASON) DEVELOP_SEASON,
	                                MAX(bb.name_t) as rule_no,
	                                MAX(b.user_section) as user_section,
	                                MIN(b.TEST_LEVEL) as TEST_LEVEL,
	                                MAX(b.develop_type) as develop_type,
	                                MAX(b.COL1) as COL1,
	                                MAX(to_char(b.BOM_DATE,'YYYY/MM/dd')) as BOM_DATE,
	                                MAX(to_char(b.cwa_date,'YYYY/MM/dd')) as cwa_date,
	                                MAX(b.user_fdd) as user_fdd,
	                                MAX(b.user_technical) as user_technical,
                                    MAX(e.qa_principal) as qa_principal,
									MAX(d.NAME_T) as name_t
                                FROM
	                                bdm_rd_prod b
                                LEFT JOIN BDM_RD_STYLE d on b.SHOE_NO=d.SHOE_NO
                                LEFT JOIN qcm_shoes_qa_record_m q ON b.shoe_no = q.shoes_code
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
                                LEFT JOIN bdm_rd_style aa ON b.shoe_no=aa.shoe_no
                                LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                                LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=b.SHOE_NO
                                WHERE
	                                1 = 1 {where}
                                GROUP BY
	                                b.SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("PROD_NO", $@"%{PROD_NO}%");
                paramTestDic.Add("SHOE_NO", $@"%{SHOE_NO}%");
                paramTestDic.Add("PRODUCT_MONTH", $@"%{PRODUCT_MONTH}%");
                paramTestDic.Add("DEVELOP_SEASON", $@"%{DEVELOP_SEASON}%");
                paramTestDic.Add("user_section", $@"%{user_section}%");
                paramTestDic.Add("rule_no", $@"%{rule_no}%");
                paramTestDic.Add("cwa_date", $@"%{cwa_date}%");
                paramTestDic.Add("qa_principal", $@"%{qa_principal}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
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
        /// 跳转各阶段样品记录查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAtraitMain(object OBJ)
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
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;

                //string sql = $@"SELECT
                //                MAX(develop_season) AS develop_season,
                //                LISTAGG ( to_char( PROD_NO ), ',' ) WITHIN GROUP ( ORDER BY SHOE_NO ) as PROD_NO,
                //                 SHOE_NO,
                //                    MAX(user_section) AS user_section,
                //                    MAX(bom_date) AS bom_date,
                //                    MAX(cwa_date) AS cwa_date,
                //                 MAX(PRODUCT_MONTH) as PRODUCT_MONTH,
                //                    MAX(user_fdd) AS user_fdd,
                //                    MAX(develop_type) AS develop_type
                //                FROM
                //                 bdm_rd_prod 
                //                 where 1=1 and SHOE_NO like '%{SHOE_NO}%'
                //                 GROUP BY
                //                 SHOE_NO";
                string sql = $@"SELECT
	*
FROM
	(
		SELECT
			{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.PROD_NO", "b.PROD_NO")} AS PROD_NO,
			b.SHOE_NO,
			'' AS try_on_state,
			'' AS fgt_state,
			'' AS cma_state,
			MAX (b.PRODUCT_MONTH) AS PRODUCT_MONTH,
			MAX (q.image_guid) AS image_guid,
			MAX (T .file_url) AS file_url,
            MAX(q.production_plant) AS production_plant, -- 生产厂区
            MAX(q.process_specialist) AS process_specialist, -- 工艺专员
            MAX(q.section_chief) AS section_chief, -- 课长
            MAX(q.bottom_formwork_specialist) AS bottom_formwork_specialist,--地膜专员 
            MAX(q.master_editor) AS master_editor, --主板师 
						MAX(bb.name_t) as rule_no,
						MIN(b.TEST_LEVEL) as TEST_LEVEL,
						MAX(b.BOM_DATE) as BOM_DATE,
						MAX(b.cwa_date) as cwa_date,
						MAX(b.user_fdd) as user_fdd,
                        SUM(b.COL1) as sumcol1,
                        MAX(q.try_on_remark) as try_on_remark,
                        MAX(q.fgt_remark) as fgt_remark,
                        MAX(q.cma_remark) as cma_remark,
									MAX(d.NAME_T) as name_t,
									MAX(b.mold_no) as mold_no,
                        MAX(b.develop_type) AS develop_type
		FROM
			bdm_rd_prod b
        LEFT JOIN BDM_RD_STYLE d on b.SHOE_NO=d.SHOE_NO
		LEFT JOIN qcm_shoes_qa_record_m q ON b.shoe_no = q.shoes_code
		LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
		LEFT JOIN bdm_rd_style aa ON b.shoe_no=aa.shoe_no
    LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
		GROUP BY
			b.SHOE_NO
	) T
WHERE
	T .SHOE_NO = @SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{SHOE_NO}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Dictionary<string, object> exDic = new Dictionary<string, object>();
                    exDic.Add("SHOE_NO", SHOE_NO);

                    DataTable task_dt = DB.GetDataTable($@"
SELECT
    m.ID,
	m.TASK_NO,
	m.RETEST_TASK_NO 
FROM
	QCM_EX_TASK_LIST_M m
WHERE m.ART_NO IN (
SELECT PROD_NO FROM BDM_RD_PROD WHERE SHOE_NO=@SHOE_NO
) 
ORDER BY m.ID
", exDic);
                    List<string> task_no_list = new List<string>();
                    List<int> continue_task_id_list = new List<int>();//跳过的重测任务的id
                    foreach (DataRow item in task_dt.Rows)
                    {
                        //实验室编号
                        string TASK_NO = item["TASK_NO"].ToString();
                        int TASK_ID = Convert.ToInt32(item["ID"].ToString());
                        if (continue_task_id_list.Contains(TASK_ID))
                            continue;

                        //查询此实验室编号有没有 重测的 
                        var findNewTaskList = task_dt.Select($@"RETEST_TASK_NO='{TASK_NO}'");

                        if (findNewTaskList == null || findNewTaskList.Count() == 0)
                        {
                            task_no_list.Add(TASK_NO);
                        }
                        else
                        {
                            int maxNewId = -1;
                            foreach (DataRow newTask in findNewTaskList)
                            {
                                int currId = GetTestTaskMaxId(continue_task_id_list, task_dt, newTask["TASK_NO"].ToString(), Convert.ToInt32(newTask["ID"].ToString()));
                                if (maxNewId == -1)
                                    maxNewId = currId;
                                else if (currId > maxNewId)
                                {
                                    continue_task_id_list.Add(maxNewId);
                                    maxNewId = currId;
                                }
                            }

                            task_no_list.Add(task_dt.Select($@"ID='{maxNewId}'")[0]["TASK_NO"].ToString());
                        }


                    }

                    if (task_no_list.Count > 0)
                    {

                        DataTable exDt = DB.GetDataTable($@"
SELECT
	LOWER(d.INSPECTION_CODE) INSPECTION_CODE,
    LOWER(d.INSPECTION_NAME) INSPECTION_NAME,
	LOWER(d.ITEM_TEST_RESULT) ITEM_TEST_RESULT
FROM
	QCM_EX_TASK_LIST_M m
INNER JOIN QCM_EX_TASK_LIST_D d ON d.TASK_NO = m.TASK_NO 
WHERE m.TASK_NO IN (
{string.Join(',', task_no_list.Distinct().Select(x => $@"'{x}'"))}
) 
");

                        List<string> list = new List<string>();
                        List<string> list2 = new List<string>();
                        string fgt_remark = string.Empty;
                        string cma_remark = string.Empty;

                        //检测项目名称
                        foreach (DataRow item in exDt.Rows)
                        {
                            if (item["ITEM_TEST_RESULT"].ToString().ToLower() == "fail")
                            {
                                if (!list.Contains(item["INSPECTION_NAME"].ToString()))
                                {
                                    list.Add(item["INSPECTION_NAME"].ToString());
                                }
                            }

                            if (item["ITEM_TEST_RESULT"].ToString().ToLower() == "fail" && item["INSPECTION_CODE"].ToString().ToLower().Contains("cma"))
                            {
                                if (!list2.Contains(item["INSPECTION_NAME"].ToString()))
                                    list2.Add(item["INSPECTION_NAME"].ToString());
                            }
                        }


                        dt.Rows[0]["fgt_remark"] = string.Join(",", list);
                        dt.Rows[0]["cma_remark"] = string.Join(",", list2);
                        if (exDt != null && exDt.Rows.Count > 0)
                        {
                            var fgt_failRows = exDt.Select($@"ITEM_TEST_RESULT='fail'");
                            var fgt_passRows = exDt.Select($@"ITEM_TEST_RESULT='pass'");
                            if (fgt_failRows.Length > 0)
                                dt.Rows[0]["fgt_state"] = "FAIL";
                            else if (fgt_passRows.Length > 0)
                                dt.Rows[0]["fgt_state"] = "PASS";

                            var cma_failRows = exDt.Select($@"ITEM_TEST_RESULT='fail' and INSPECTION_CODE like 'cma%'");
                            var cma_passRows = exDt.Select($@"ITEM_TEST_RESULT='pass' and INSPECTION_CODE like 'cma%'");
                            if (cma_failRows.Length > 0)
                                dt.Rows[0]["cma_state"] = "FAIL";
                            else if (cma_passRows.Length > 0)
                                dt.Rows[0]["cma_state"] = "PASS";



                        }

                    }
                    //试穿取值
                    SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
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
                        SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");

                        DataTable artDt = DB.GetDataTable($@"
SELECT PROD_NO FROM BDM_RD_PROD WHERE SHOE_NO=@SHOE_NO
", exDic);
                        if (artDt.Rows.Count > 0)
                        {
                            List<string> artList = new List<string>();
                            foreach (DataRow item in artDt.Rows)
                            {
                                artList.Add($@"'{item[0]}'");
                            }

                            int sc_failCount = wbscDB.GetInt32($@"SELECT COUNT(1) FROM fitinfo where ARTNO IN ({string.Join(',', artList)}) AND FITRESULT='Fail';");
                            int sc_passCount = wbscDB.GetInt32($@"SELECT COUNT(1) FROM fitinfo where ARTNO IN ({string.Join(',', artList)}) AND FITRESULT='Pass';");
                            if (sc_failCount > 0)
                                dt.Rows[0]["try_on_state"] = "FAIL";
                            else if (sc_passCount > 0)
                                dt.Rows[0]["try_on_state"] = "PASS";
                        }

                    }

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

        public static int GetTestTaskMaxId(List<int> continue_task_id_list, DataTable sourceDt, string newTaskNo, int newTaskId)
        {
            var findNewTaskList = sourceDt.Select($@"RETEST_TASK_NO='{newTaskNo}'");

            if (findNewTaskList == null || findNewTaskList.Count() == 0)
            {
                return newTaskId;
            }
            else
            {
                int currMaxId = -1;
                foreach (var item in findNewTaskList)
                {
                    int currId = GetTestTaskMaxId(continue_task_id_list, sourceDt, item["TASK_NO"].ToString(), Convert.ToInt32(item["ID"].ToString()));
                    if (currMaxId == -1)
                        currMaxId = currId;
                    else if (currId > currMaxId)
                    {
                        continue_task_id_list.Add(currMaxId);
                        currMaxId = currId;
                    }
                }

                return currMaxId;
            }

        }

        /// <summary>
        /// 保存鞋型品质记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Editshoes_qa_record_m(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型编号
                string try_on_state = jarr.ContainsKey("try_on_state") ? jarr["try_on_state"].ToString() : "";//查询条件 试穿
                string fgt_state = jarr.ContainsKey("fgt_state") ? jarr["fgt_state"].ToString() : "";//查询条件 FGT
                string cma_state = jarr.ContainsKey("cma_state") ? jarr["cma_state"].ToString() : "";//查询条件 CMA
                string production_plant = jarr.ContainsKey("production_plant") ? jarr["production_plant"].ToString() : "";//查询条件 生产厂区
                string process_specialist = jarr.ContainsKey("process_specialist") ? jarr["process_specialist"].ToString() : "";//查询条件 工艺专员
                string section_chief = jarr.ContainsKey("section_chief") ? jarr["section_chief"].ToString() : "";//查询条件 课长
                string bottom_formwork_specialist = jarr.ContainsKey("bottom_formwork_specialist") ? jarr["bottom_formwork_specialist"].ToString() : "";//查询条件 底膜专员
                string master_editor = jarr.ContainsKey("master_editor") ? jarr["master_editor"].ToString() : "";//查询条件 主板师

                string try_on_remark = jarr.ContainsKey("try_on_remark") ? jarr["try_on_remark"].ToString() : "";//查询条件 试穿备注
                string fgt_remark = jarr.ContainsKey("fgt_remark") ? jarr["fgt_remark"].ToString() : "";//查询条件 FGT备注
                string cma_remark = jarr.ContainsKey("cma_remark") ? jarr["cma_remark"].ToString() : "";//查询条件 CMA备注
                //string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片guid
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from qcm_shoes_qa_record_m where shoes_code='{shoes_code}'");
                if (count > 0)
                {
                    sql = $@"update qcm_shoes_qa_record_m set try_on_state='{try_on_state}',fgt_state='{fgt_state}',
                            cma_state='{cma_state}',production_plant='{production_plant}',process_specialist='{process_specialist}',
                            section_chief='{section_chief}',bottom_formwork_specialist='{bottom_formwork_specialist}',master_editor='{master_editor}',
                            try_on_remark='{try_on_remark}',fgt_remark='{fgt_remark}',cma_remark='{cma_remark}',
                            MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where shoes_code='{shoes_code}'";
                }
                else
                {
                    sql = $@"insert into qcm_shoes_qa_record_m (shoes_code,try_on_state,fgt_state,cma_state,production_plant,process_specialist,
                             section_chief,bottom_formwork_specialist,master_editor,try_on_remark,fgt_remark,cma_remark,CREATEBY,CREATEDATE,CREATETIME) 
                             values('{shoes_code}','{try_on_state}','{fgt_state}','{cma_state}','{production_plant}','{process_specialist}',
                             '{section_chief}','{bottom_formwork_specialist}','{master_editor}','{try_on_remark}','{fgt_remark}','{cma_remark}','{user}','{date}','{time}')";
                }
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
        /// 跳转各阶段样品记录详情页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAtraitEdit(object OBJ)
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
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;

                //string sql = $@"SELECT
                //                MAX(develop_season) AS develop_season,
                //                LISTAGG ( to_char( PROD_NO ), ',' ) WITHIN GROUP ( ORDER BY SHOE_NO ) as PROD_NO,
                //                 SHOE_NO,
                //                    MAX(user_section) AS user_section,
                //                    MAX(bom_date) AS bom_date,
                //                    MAX(cwa_date) AS cwa_date,
                //                 MAX(PRODUCT_MONTH) as PRODUCT_MONTH,
                //                    MAX(user_fdd) AS user_fdd,
                //                    MAX(develop_type) AS develop_type
                //                FROM
                //                 bdm_rd_prod 
                //                 where 1=1 and SHOE_NO like '%{SHOE_NO}%'
                //                 GROUP BY
                //                 SHOE_NO";
                string sql = $@"SELECT
                                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.PROD_NO", "b.PROD_NO")} as PROD_NO,
	                                b.SHOE_NO,
	                                MAX(b.PRODUCT_MONTH) as PRODUCT_MONTH,
									MAX(q.image_guid) as image_guid,
                                    '' AS try_on_state,
                                    '' AS fgt_state,
                                    '' AS cma_state,
                                    MAX(q.production_plant) AS production_plant,
                                    MAX(q.process_specialist) AS process_specialist,
                                    MAX(q.section_chief) AS section_chief,
                                    MAX(q.bottom_formwork_specialist) AS bottom_formwork_specialist,
                                    MAX(q.master_editor) AS master_editor,
                                    MAX(bb.name_t) as rule_no,
						            MAX(b.TEST_LEVEL) as TEST_LEVEL,
						            MAX(b.BOM_DATE) as BOM_DATE,
						            MAX(b.cwa_date) as cwa_date,
						            MAX(b.user_fdd) as user_fdd,
                                    SUM(b.COL1) as sumcol1,
                                    MAX(b.DEVELOP_SEASON) AS DEVELOP_SEASON,
                                    MAX(q.try_on_remark) as try_on_remark,
                                    MAX(q.fgt_remark) as fgt_remark,
                                    MAX(q.cma_remark) as cma_remark,
                                    MAX(b.mold_no) as mold_no,
                                    MAX(b.develop_type) AS develop_type,
                                    MAX(d.NAME_T) as name_t
                                FROM
	                                bdm_rd_prod b 
                                    LEFT JOIN BDM_RD_STYLE d on b.SHOE_NO=d.SHOE_NO
                                    LEFT join qcm_shoes_qa_record_m q on b.shoe_no=q.shoes_code
                                    LEFT JOIN bdm_rd_style aa ON b.shoe_no=aa.shoe_no
                                    LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no 
	                                where 1=1 and b.SHOE_NO = @SHOE_NO
	                                GROUP BY
	                                b.SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{SHOE_NO}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                if (dt != null && dt.Rows.Count > 0)
                {
                    Dictionary<string, object> exDic = new Dictionary<string, object>();
                    exDic.Add("SHOE_NO", SHOE_NO);

                    DataTable task_dt = DB.GetDataTable($@"
SELECT
    m.ID,
	m.TASK_NO,
	m.RETEST_TASK_NO 
FROM
	QCM_EX_TASK_LIST_M m
WHERE m.ART_NO IN (
SELECT PROD_NO FROM BDM_RD_PROD WHERE SHOE_NO=@SHOE_NO
) 
ORDER BY m.ID
", exDic);
                    List<string> task_no_list = new List<string>();
                    List<int> continue_task_id_list = new List<int>();//跳过的重测任务的id
                    foreach (DataRow item in task_dt.Rows)
                    {
                        //实验室编号
                        string TASK_NO = item["TASK_NO"].ToString();
                        int TASK_ID = Convert.ToInt32(item["ID"].ToString());
                        if (continue_task_id_list.Contains(TASK_ID))
                            continue;

                        //查询此实验室编号有没有 重测的 
                        var findNewTaskList = task_dt.Select($@"RETEST_TASK_NO='{TASK_NO}'");

                        if (findNewTaskList == null || findNewTaskList.Count() == 0)
                        {
                            task_no_list.Add(TASK_NO);
                        }
                        else
                        {
                            int maxNewId = -1;
                            foreach (DataRow newTask in findNewTaskList)
                            {
                                int currId = GetTestTaskMaxId(continue_task_id_list, task_dt, newTask["TASK_NO"].ToString(), Convert.ToInt32(newTask["ID"].ToString()));
                                if (maxNewId == -1)
                                    maxNewId = currId;
                                else if (currId > maxNewId)
                                {
                                    continue_task_id_list.Add(maxNewId);
                                    maxNewId = currId;
                                }
                            }

                            task_no_list.Add(task_dt.Select($@"ID='{maxNewId}'")[0]["TASK_NO"].ToString());
                        }


                    }

                    if (task_no_list.Count > 0)
                    {
                        DataTable exDt = DB.GetDataTable($@"
SELECT
	LOWER(d.INSPECTION_CODE) INSPECTION_CODE,
    LOWER(d.INSPECTION_NAME) INSPECTION_NAME,
	LOWER(d.ITEM_TEST_RESULT) ITEM_TEST_RESULT
FROM
	QCM_EX_TASK_LIST_M m
INNER JOIN QCM_EX_TASK_LIST_D d ON d.TASK_NO = m.TASK_NO 
WHERE m.TASK_NO IN (
{string.Join(',', task_no_list.Distinct().Select(x => $@"'{x}'"))}
) 
");

                        if (exDt != null && exDt.Rows.Count > 0)
                        {
                            var fgt_failRows = exDt.Select($@"ITEM_TEST_RESULT='fail'");
                            var fgt_passRows = exDt.Select($@"ITEM_TEST_RESULT='pass'");
                            if (fgt_failRows.Length > 0)
                                dt.Rows[0]["fgt_state"] = "FAIL";
                            else if (fgt_passRows.Length > 0)
                                dt.Rows[0]["fgt_state"] = "PASS";

                            var cma_failRows = exDt.Select($@"ITEM_TEST_RESULT='fail' and INSPECTION_CODE like 'cma%'");
                            var cma_passRows = exDt.Select($@"ITEM_TEST_RESULT='pass' and INSPECTION_CODE like 'cma%'");
                            if (cma_failRows.Length > 0)
                                dt.Rows[0]["cma_state"] = "FAIL";
                            else if (cma_passRows.Length > 0)
                                dt.Rows[0]["cma_state"] = "PASS";

                        }
                    }
                    //试穿取值
                    SJeMES_Framework_NETCore.DBHelper.DataBase DBSqlServer = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
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
                        SJeMES_Framework_NETCore.DBHelper.DataBase wbscDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(dbConfigRow["dbtype"].ToString(), dbConfigRow["dbserver"].ToString(), dbConfigRow["dbname"].ToString(), dbConfigRow["dbuser"].ToString(), dbConfigRow["dbpassword"].ToString(), "");

                        DataTable artDt = DB.GetDataTable($@"
SELECT PROD_NO FROM BDM_RD_PROD WHERE SHOE_NO=@SHOE_NO
", exDic);
                        if (artDt.Rows.Count > 0)
                        {
                            List<string> artList = new List<string>();
                            foreach (DataRow item in artDt.Rows)
                            {
                                artList.Add($@"'{item[0]}'");
                            }

                            int sc_failCount = wbscDB.GetInt32($@"SELECT COUNT(1) FROM fitinfo where ARTNO IN ({string.Join(',', artList)}) AND FITRESULT='Fail';");
                            int sc_passCount = wbscDB.GetInt32($@"SELECT COUNT(1) FROM fitinfo where ARTNO IN ({string.Join(',', artList)}) AND FITRESULT='Pass';");
                            if (sc_failCount > 0)
                                dt.Rows[0]["try_on_state"] = "FAIL";
                            else if (sc_passCount > 0)
                                dt.Rows[0]["try_on_state"] = "PASS";
                        }

                    }

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
        /// 上传各阶段样品记录详情页面文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UploadtraitEditFile(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型编号
                string file_id = jarr.ContainsKey("file_id") ? jarr["file_id"].ToString() : "";//查询条件 文件关联id
                string file_type = jarr.ContainsKey("file_type") ? jarr["file_type"].ToString() : "3";//查询条件 文件类型
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;

                DB.ExecuteNonQuery($@"insert into qcm_shoes_qa_record_file_m (shoes_code,file_id,file_type,CREATEBY,CREATEDATE,CREATETIME) 
                                        values('{shoes_code}','{file_id}','{file_type}','{user}','{date}','{time}')");
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
        /// 各阶段样品记录添加页面查询阶段
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getphase_creation(object OBJ)
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
                //string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;

                string sql = $@"select phase_creation_no,phase_creation_name from bdm_phase_creation_m";
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
        /// 各阶段样品记录添加页面查询ART
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetART(object OBJ)
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
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    where = $"and SHOE_NO = @SHOE_NO";
                }
                string sql = $@"select prod_no from bdm_rd_prod where 1 = 1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{SHOE_NO}");
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
        /// 各阶段样品记录添加页面查询品质风险类别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getrisk_category(object OBJ)
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
                //string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;

                string sql = $@"SELECT qa_risk_category_code,qa_risk_category_name from bdm_qa_risk_category_m";
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
        /// 各阶段样品记录添加页面查询负责人
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getperson(object OBJ)
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
                string STAFF_NO = jarr.ContainsKey("STAFF_NO") ? jarr["STAFF_NO"].ToString() : "";//查询条件 员工代号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;

                string sql = $@"select STAFF_NO,STAFF_NAME from HR001M where STAFF_NO=@STAFF_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("STAFF_NO", $@"{STAFF_NO}");
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
        /// 查询各阶段样品记录添加页面的数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLastrecord_item(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 鞋型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = string.Empty;
                if (string.IsNullOrWhiteSpace(did))
                {
                    sql = $@"select id from qcm_shoes_qa_record_d where shoes_code='{shoe_no}' order by createdate desc,createtime desc";
                    did = DB.GetString(sql);
                }

                sql = $@"SELECT
	                        MAX(m.id) as itemid,
                            MAX(m.workshop_section_no) as workshop_section_no,
                            MAX(m.workshop_section_name) as workshop_section_name,
	                        MAX(m.d_id) as d_id,
	                        MAX(m.choice_no) as choice_no,
	                        MAX(m.choice_name) as choice_name,
	                        MAX(m.qa_risk_desc) as qa_risk_desc,
	                        MAX(m.qa_risk_category_code) as qa_risk_category_code,
	                        MAX(b.qa_risk_category_name) as qa_risk_category_name,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} as art_codes,
	                        MAX(m.bad_qty) as bad_qty,
	                        MAX(m.bad_rate) as bad_rate,
	                        MAX(m.measures) as measures,
	                        MAX(m.MEASURES_RES) as MEASURES_RES,
	                        MAX(m.person_in_charge) as person_in_charge,
                            MAX(m.remark) as remark,
                            MAX(m.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                            MAX(m.is_dqa_mqa_band) as is_dqa_mqa_band,
	                        MAX(m.image_guid) as image_guid
                        FROM
	                        qcm_shoes_qa_record_item m
	                        left join QCM_SHOES_QA_RECORD_ITEM_ART t on m.id=t.item_id
                            -- LEFT JOIN qcm_dqa_mag_m y ON t.SHOES_CODE = y.SHOES_CODE
	                        LEFT JOIN bdm_qa_risk_category_m b ON m.qa_risk_category_code = b.qa_risk_category_code
	                        where d_id=@did
	                        group by m.id";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("did", $@"{did}");
                DataTable dt1 = DB.GetDataTable(sql, paramTestDic);

                sql = $@"select phase_date,phase_creation_no,total_production from qcm_shoes_qa_record_d where id=@did";
                DataTable dt2 = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQaRiskDetails(object OBJ)
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

                var po_info = DB.GetDataTable($@"SELECT DISTINCT QA_RISK_DETAILS_DESC as PROD_NO FROM BDM_QA_RISK_DETAILS_M");

                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("art_info", po_info);

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

        /// <summary>
        /// 保存各阶段样品记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Editqa_record(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 did
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型编号
                string phase_date = jarr.ContainsKey("phase_date") ? jarr["phase_date"].ToString() : "";//查询条件 日期
                string phase_creation_no = jarr.ContainsKey("phase_creation_no") ? jarr["phase_creation_no"].ToString() : "";//查询条件 阶段编号
                string total_production = jarr.ContainsKey("total_production") ? jarr["total_production"].ToString() : "";//查询条件 生产总数
                DataTable qa_record_item = jarr.ContainsKey("qa_record_item") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["qa_record_item"].ToString()) : null;//查询条件 鞋型品质记录——品质状况——详情

                //string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片guid
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;

                if (string.IsNullOrWhiteSpace(did))
                {
                    sql = $@"insert into qcm_shoes_qa_record_d (shoes_code,phase_date,phase_creation_no,total_production,CREATEBY,CREATEDATE,CREATETIME) 
                            values('{shoes_code}','{phase_date}','{phase_creation_no}','{total_production}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    did = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_shoes_qa_record_d");
                    foreach (DataRow item in qa_record_item.Rows)
                    {
                        sql = $@"insert into qcm_shoes_qa_record_item (workshop_section_no,workshop_section_name,d_id,choice_no,choice_name,qa_risk_desc,qa_risk_category_code,
                             bad_qty,bad_rate,measures,remark,image_guid,is_dqa_mqa_band,CREATEBY,CREATEDATE,CREATETIME,QA_RISK_DETAILS_DESC,MEASURES_RES) 
                            values('{item["workshop_section_no"]}','{item["workshop_section_name"]}','{did}','{item["choice_no"]}','{item["choice_name"]}','{item["qa_risk_desc"]}','{item["qa_risk_category_code"]}',
                            '{item["bad_qty"]}','{item["bad_rate"]}','{item["measures"]}','{item["remark"]}',
                            '{item["image_guid"]}','{item["is_dqa_mqa_band_val"]}','{user}','{date}','{time}','{item["qa_risk_details_desc"]}','{item["measures_res"]}')";

                        DB.ExecuteNonQuery(sql);
                        string item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_shoes_qa_record_item");

                        if (!string.IsNullOrEmpty(item["image_guid"].ToString()))
                        {
                            sql = $@"delete from qcm_shoes_qa_record_item_f where item_id='{item_id}'";
                            DB.ExecuteNonQuery(sql);
                            string[] imgs = item["image_guid"].ToString().Split(',');
                            for (int i = 0; i < imgs.Length; i++)
                            {
                                sql = $@"insert into qcm_shoes_qa_record_item_f (item_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                values('{item_id}','{imgs[i]}','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                            }
                        }


                        string[] art_codes = item["art_codes"].ToString().Split(',');
                        for (int i = 0; i < art_codes.Length; i++)
                        {
                            sql = $@"insert into qcm_shoes_qa_record_item_art (shoes_code,item_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                values('{shoes_code}','{item_id}','{art_codes[i]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(did))
                {
                    if (qa_record_item.Rows.Count > 0)
                    {
                        sql = $@"update qcm_shoes_qa_record_d set phase_date='{phase_date}',phase_creation_no='{phase_creation_no}',total_production='{total_production}',
                            modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{did}'";
                        DB.ExecuteNonQuery(sql);
                        foreach (DataRow item in qa_record_item.Rows)
                        {
                            if (!string.IsNullOrWhiteSpace(item["itemid"].ToString()))
                            {
                                sql = $@"delete from qcm_shoes_qa_record_item_art where item_id='{item["itemid"]}'";
                                DB.ExecuteNonQuery(sql);
                                sql = $@"update qcm_shoes_qa_record_item set workshop_section_no = '{item["workshop_section_no"]}',workshop_section_name = '{item["workshop_section_name"]}', choice_no='{item["choice_no"]}',choice_name='{item["choice_name"]}',
                                    qa_risk_desc='{item["qa_risk_desc"]}',qa_risk_category_code='{item["qa_risk_category_code"]}',bad_qty='{item["bad_qty"]}',
                                    bad_rate='{item["bad_rate"]}',measures='{item["measures"]}',person_in_charge='{item["person_in_charge"]}',
                                    image_guid='{item["image_guid"]}',remark='{item["remark"]}',
                                    is_dqa_mqa_band='{item["is_dqa_mqa_band_val"]}',
                                    modifyby='{user}',modifydate='{date}',modifytime='{time}',QA_RISK_DETAILS_DESC='{item["qa_risk_details_desc"]}',MEASURES_RES='{item["measures_res"]}' where d_id='{did}' and 
                                    id='{item["itemid"]}'";
                                DB.ExecuteNonQuery(sql);
                                string[] art_codes = item["art_codes"].ToString().Split(',');
                                for (int i = 0; i < art_codes.Length; i++)
                                {
                                    sql = $@"insert into qcm_shoes_qa_record_item_art (shoes_code,item_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                values('{shoes_code}','{item["itemid"]}','{art_codes[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                                if (!string.IsNullOrEmpty(item["image_guid"].ToString()))
                                {
                                    sql = $@"delete from qcm_shoes_qa_record_item_f where item_id='{item["itemid"]}'";
                                    DB.ExecuteNonQuery(sql);
                                    string[] imgs = item["image_guid"].ToString().Split(',');
                                    for (int i = 0; i < imgs.Length; i++)
                                    {
                                        sql = $@"insert into qcm_shoes_qa_record_item_f (item_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                values('{item["itemid"]}','{imgs[i]}','{user}','{date}','{time}')";
                                        DB.ExecuteNonQuery(sql);
                                    }
                                }
                            }
                        }
                    }
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
        /// 各阶段样品记录添加页面查询材料/工序
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getchoice(object OBJ)
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
                string choice = jarr.ContainsKey("choice") ? jarr["choice"].ToString() : "";//查询条件
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 工段编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(choice))
                {
                    where = $@" and (t.choice_no like @choice or t.choice_name like @choice_name)";
                }

                string ds = DB.GetString($@"select data_source from qcm_dqa_mag_m where id='{mid}'");

                string sql = string.Empty;
                if (!string.IsNullOrEmpty(ds))
                {
                    if (ds == "0")
                    {
                        sql = $@"select choice_no,choice_name from( select item_no as choice_no,NAME_T as choice_name from bdm_rd_item)t where 1=1 {where}";
                    }
                    else
                    {
                        sql = $@"select choice_no,choice_name from(select procedure_no as choice_no,procedure_name as choice_name from base025m)t where 1=1 {where}";
                    }
                }
                else
                {
                    sql = $@"
SELECT choice_no,choice_name FROM (
select choice_no,choice_name from(select procedure_no as choice_no,procedure_name as choice_name from base025m)t 
UNION ALL
select choice_no,choice_name from( select item_no as choice_no,NAME_T as choice_name from bdm_rd_item)t 
) tab
WHERE 1=1 and choice_no like @choice OR choice_name like @choice_name ";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("choice", $@"{choice}%");
                paramTestDic.Add("choice_name", $@"{choice}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
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
        /// 根据art获取bom
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetchoiceByArt(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art_no_list_Str = jarr.ContainsKey("art_no_list") ? jarr["art_no_list"].ToString() : "";
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 工段编号

                DB.Open();
                DB.BeginTransaction();

                DataTable dt = new DataTable();
                dt.Columns.Add("choice_no", typeof(string));
                dt.Columns.Add("choice_name", typeof(string));
                dt.Columns.Add("position", typeof(string));
                dt.Columns.Add("position_en", typeof(string));

                string ds = DB.GetString($@"select data_source from qcm_dqa_mag_m where id='{mid}'");

                string sql = string.Empty;
                //if (!string.IsNullOrEmpty(ds))
                //{
                //    if (ds == "0")
                //    {//art bom
                //        InsertGetBomDtByArt(dt, art_no_list_Str, ReqObj);
                //    }
                //    else
                //    {//工序
                //        InsertProcedureDt(dt, DB);
                //    }
                //}
                //else
                //{// art bom加工序
                //    InsertGetBomDtByArt(dt, art_no_list_Str, ReqObj);
                //    InsertProcedureDt(dt, DB);
                //}
                InsertGetBomDtByArt(dt, art_no_list_Str, ReqObj);
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                int rowCount = dt.Rows.Count;

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

        //public static void InsertGetBomDtByArt(DataTable source_dt, string art_no_list_Str, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        //{
        //    if (!string.IsNullOrEmpty(art_no_list_Str))
        //    {
        //        List<string> art_no_list = art_no_list_Str.Split(",").ToList();

        //        List<Art.Bom.DT_PP113_RSPRESDATAS> resData = new List<Art.Bom.DT_PP113_RSPRESDATAS>();//当前art数据

        //        List<Art.Bom.DT_PP113_RSPRESDATAS> resDataRecord = new List<Art.Bom.DT_PP113_RSPRESDATAS>();//记录上一次循环数组

        //        int i = 0;
        //        foreach (string art_no in art_no_list)
        //        {
        //            //记录上一次art集合
        //            resDataRecord = resData;

        //            var req = new Art.Bom.DT_PP113_REQI_REQUESTBODY();
        //            req.art_no = art_no;
        //            req.item_no = "";
        //            req.stage = "PRS";
        //            req.type = "1";// 1-量产；2-开发
        //            var apires = SJ_SAPAPI.SapApiHelper.GetArtBom(req, ReqObj);
        //            if (apires.IS_SUCCESS.ToLower() == "true")
        //            {
        //                if (apires.RES.DATAS.Length > 0)
        //                    resData.AddRange(apires.RES.DATAS);
        //            }

        //            if (i > 0)
        //                resData = resData.Intersect(resDataRecord).ToList();//交集
        //            i++;
        //        }
        //        List<string> list = new List<string>();
        //        //List<string> list2 = new List<string>();
        //        //resData = resData.GroupBy(x => (x.ITEM_NO, x.NAME_CN)).Select(x => x.First());
        //        foreach (var item in resData)
        //        {
        //            if (list.Contains(item.ITEM_NO))
        //            {
        //                continue;
        //            }
        //            var addRow = source_dt.NewRow();
        //            addRow["choice_no"] = item.ITEM_NO;
        //            addRow["choice_name"] = item.NAME_EN;
        //            addRow["position"] = item.POSITION;
        //            addRow["position_en"] = item.POSITION_EN;
        //            //addRow["NAME_CN"] = item.POSITION_EN;
        //            source_dt.Rows.Add(addRow);

        //            list.Add(item.ITEM_NO);
        //        }

        //    }
        //}

        public static void InsertGetBomDtByArt(DataTable source_dt, string art_no_list_Str, SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            if (!string.IsNullOrEmpty(art_no_list_Str))
            {
                List<string> art_no_list = art_no_list_Str.Split(",").ToList();

                List<Art.Bom.DT_PP113_RSPRESDATAS> resData = new List<Art.Bom.DT_PP113_RSPRESDATAS>();//当前art数据

                List<Art.Bom.DT_PP113_RSPRESDATAS> resDataRecord = new List<Art.Bom.DT_PP113_RSPRESDATAS>();//记录上一次循环数组

                int i = 0;
                foreach (string art_no in art_no_list)
                {
                    //记录上一次art集合
                    resDataRecord = resData;

                    var req = new Art.Bom.DT_PP113_REQI_REQUESTBODY();
                    req.art_no = art_no;
                    //req.werks = "1003"; //APE-1003 APH-4003 APC-5003
                    req.werks = "5003"; //APE-1003 APH-4003 APC-5003
                    req.item_no = "";
                    req.stage = ""; //不传阶段取时间最新的样品单，传入阶段取该阶段时间最新样品单
                    req.type = "";// 1-量产；2-开发(已过时)（OUT of Date）
                    //req.langu = "1"; //1-中文 EN-英文 VI-越文
                    req.langu = "EN"; //1-中文 EN-英文 VI-越文
                    var apires = SJ_SAPAPI.SapApiHelper.GetArtBom(req, ReqObj);
                    if (apires.IS_SUCCESS.ToLower() == "true")
                    {
                        if (apires.RES.DATAS.Length > 0)
                            resData.AddRange(apires.RES.DATAS);
                    }

                    if (i > 0)
                        resData = resData.Intersect(resDataRecord).ToList();//交集
                    i++;
                }
                List<string> list = new List<string>();
                //List<string> list2 = new List<string>();
                //resData = resData.GroupBy(x => (x.ITEM_NO, x.NAME_CN)).Select(x => x.First());
                foreach (var item in resData)
                {
                    if (list.Contains(item.ITEM_NO))
                    {
                        continue;
                    }
                    //var addRow = source_dt.NewRow();
                    //addRow["choice_no"] = item.ITEM_NO;
                    //addRow["choice_name"] = item.NAME_CN;
                    //addRow["POSITION"] = item.POSITION;
                    //addRow["POSITION_CN"] = item.POSITION_CN;
                    ////addRow["NAME_CN"] = item.POSITION_EN;
                    //source_dt.Rows.Add(addRow);

                    var addRow = source_dt.NewRow();
                    addRow["choice_no"] = item.ITEM_NO;
                    //addRow["choice_name"] = item.NAME_CN;
                    addRow["choice_name"] = item.NAME_EN;
                    //addRow["POSITION"] = item.POSITION;
                    addRow["position"] = item.POSITION;
                    //addRow["POSITION_CN"] = item.POSITION_CN;
                    addRow["position_en"] = item.POSITION_EN;
                    //addRow["NAME_CN"] = item.POSITION_EN;
                    source_dt.Rows.Add(addRow);

                    list.Add(item.ITEM_NO);
                }

            }
        }

        public static void InsertProcedureDt(DataTable source_dt, SJeMES_Framework_NETCore.DBHelper.DataBase DB)
        {
            string sql = $@"select procedure_no as choice_no,procedure_name as choice_name from base025m";
            DataTable procedure_dt = DB.GetDataTable(sql);
            foreach (DataRow item in procedure_dt.Rows)
            {
                var addRow = source_dt.NewRow();
                addRow["choice_no"] = item["choice_no"].ToString();
                addRow["choice_name"] = item["choice_name"].ToString();
                source_dt.Rows.Add(addRow);
            }
        }

        /// <summary>
        /// 各阶段样品记录添加页面查询图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimage_guid(object OBJ)
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
                string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = $@"select file_name,file_url,'BDM_UPLOAD_FILE_ITEM' as tablename,'' as id,GUID from BDM_UPLOAD_FILE_ITEM where GUID in('{image_guid.Replace(",", "','")}')";
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
        /// 各阶段样品记录主页添加页面查询图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimage_guidCopy(object OBJ)
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
                string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = $@"select file_name as img_name,file_url as img_url from BDM_UPLOAD_FILE_ITEM where GUID in('{image_guid.Replace(",", "','")}')";
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
        /// 历史各阶段样品品质状况
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShapecenterView(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//鞋型编号
                string is_dqa_mqa_band = jarr.ContainsKey("is_dqa_mqa_band") ? jarr["is_dqa_mqa_band"].ToString() : "";//鞋型编号

                string where = "";
                if (is_dqa_mqa_band == "1")
                {
                    where += $" and a.is_dqa_mqa_band=@is_dqa_mqa_band";
                }
                else if (is_dqa_mqa_band == "0")
                {
                    where += $" and (a.is_dqa_mqa_band<>1 or a.is_dqa_mqa_band is null)";
                }

                string sql = $@"select id,shoes_code,phase_date,phase_creation_no,total_production,createdate,createtime  from qcm_shoes_qa_record_d where shoes_code=@shoes_code order by id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoes_code", $@"{shoes_code}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                List<string> d_id = new List<string>();
                d_id.Add("0");
                foreach (DataRow dr in dt.Rows)
                {
                    d_id.Add(dr["id"].ToString());
                }
                string sql2 = $@"SELECT
                                MAX(b.id) as did,
                                MAX(a.workshop_section_no) as workshop_section_no,
                            MAX(a.workshop_section_name) as workshop_section_name,
                                MAX(b.SHOES_CODE) as shoe_code,
	                            MAX( a.choice_no ) AS choice_no,
	                            MAX( a.choice_name ) AS choice_name,
	                            MAX( a.qa_risk_desc ) AS qa_risk_desc,
	                            MAX( a.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( m.qa_risk_category_name ) AS qa_risk_category_name,
	                            MAX( a.bad_qty ) AS bad_qty,
	                            MAX( a.bad_rate ) AS bad_rate,
	                            MAX( a.measures ) AS measures,
	                            MAX( a.MEASURES_RES ) AS MEASURES_RES,
	                            MAX( a.person_in_charge ) AS person_in_charge,
                                MAX( a.remark ) AS remark,
                                MAX( a.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
	                            MAX( a.image_guid ) AS image_guid,
                                MAX( a.is_dqa_mqa_band ) AS is_dqa_mqa_band,
	                            MAX( b.shoes_code ) AS shoes_code,
	                            MAX( b.phase_date ) AS phase_date,
	                            MAX( b.phase_creation_no ) AS phase_creation_no,
                                MAX( p.phase_creation_name ) AS phase_creation_name,
	                            MAX( b.total_production ) AS total_production,
	                            MAX( b.createdate ) AS createdate,
	                            MAX( b.createtime ) AS createtime,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_codes,
                                MAX(u.FILE_URL) as img_url,
								MAX(u.FILE_NAME) as img_name
                            FROM
	                            qcm_shoes_qa_record_item a
	                            LEFT JOIN QCM_SHOES_QA_RECORD_ITEM_ART t ON a.id = t.item_id 	
                               -- LEFT JOIN qcm_dqa_mag_m y ON t.SHOES_CODE = y.SHOES_CODE
	                            LEFT JOIN qcm_shoes_qa_record_d b ON a.d_id = b.id
                                LEFT JOIN bdm_phase_creation_m p on b.phase_creation_no=p.phase_creation_no
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM u ON a.image_guid=u.GUID
                                LEFT JOIN bdm_qa_risk_category_m m on a.qa_risk_category_code=m.qa_risk_category_code
                                AND a.CREATEDATE = b.CREATEDATE  
								AND a.CREATETIME = b.CREATETIME
                            where 1=1 and a.d_id in({string.Join(",", d_id)}) {where}
		                            GROUP BY
	                            a.id 
                            ORDER BY
	                            a.id DESC";
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("is_dqa_mqa_band", $@"{is_dqa_mqa_band}");
                DataTable dt2 = DB.GetDataTable(sql2, paramTestDic2);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1", dt2);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 各阶段样品记录查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAtraitMainFile(object OBJ)
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
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型

                string where = string.Empty;
                string sql = $@"select f.file_url,f.file_name,'qcm_shoes_qa_record_file_m' as tablename,m.id from BDM_UPLOAD_FILE_ITEM f INNER JOIN qcm_shoes_qa_record_file_m m on f.guid=m.file_id where m.shoes_code=@SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{SHOE_NO}");
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
        ///仓库详情页面历史各阶段样品品质状况
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_ShoeShapecenterViewItem(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//
                string is_dqa_mqa_band = jarr.ContainsKey("is_dqa_mqa_band") ? jarr["is_dqa_mqa_band"].ToString() : "";//鞋型编号
                List<string> art = jarr.ContainsKey("art") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["art"].ToString()) : null;//art

                string where = "";
                if (art.Count > 0)
                {
                    where = $@" and t.art_code in ({string.Join(",", art.Select(x => $@"'{x}'"))})";
                }
                if (is_dqa_mqa_band == "1")
                {
                    where += $" and a.is_dqa_mqa_band=@is_dqa_mqa_band";
                }
                else if (is_dqa_mqa_band == "0")
                {
                    where += $" and (a.is_dqa_mqa_band<>1 or a.is_dqa_mqa_band is null)";
                }
                string sql = $@"SELECT
                                b.id,
	                            MAX( b.shoes_code ) AS shoes_code,
	                            MAX( b.phase_date ) AS phase_date,
	                            MAX( b.phase_creation_no ) AS phase_creation_no,
	                            MAX( b.total_production ) AS total_production,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_codes 
                            FROM
	                            qcm_shoes_qa_record_d b
	                            LEFT JOIN qcm_shoes_qa_record_item a ON b.id = a.d_id
	                            LEFT JOIN QCM_SHOES_QA_RECORD_ITEM_ART t ON a.id = t.item_id 
                            where b.shoes_code=@shoes_code {where}
                            GROUP BY
	                            b.id 
                            ORDER by 
	                            b.id DESC";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoes_code", $@"{shoes_code}");
                paramTestDic.Add("is_dqa_mqa_band", $@"{is_dqa_mqa_band}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                List<string> d_id = new List<string>();
                d_id.Add("0");
                foreach (DataRow dr in dt.Rows)
                {
                    d_id.Add(dr["id"].ToString());
                }
                string sql2 = $@"SELECT
                                MAX( a.id ) AS itemid,
                                MAX( a.d_id ) AS d_id,
                                MAX(a.workshop_section_no) as workshop_section_no,
                            MAX(a.workshop_section_name) as workshop_section_name,
	                            MAX( a.choice_no ) AS choice_no,
	                            MAX( a.choice_name ) AS choice_name,
	                            MAX( a.qa_risk_desc ) AS qa_risk_desc,
	                            MAX( a.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( m.qa_risk_category_name ) AS qa_risk_category_name,
	                            MAX( a.bad_qty ) AS bad_qty,
	                            MAX( a.bad_rate ) AS bad_rate,
	                            MAX( a.measures ) AS measures,
	                            MAX( a.MEASURES_RES ) AS MEASURES_RES,
	                            MAX( a.person_in_charge ) AS person_in_charge,
                                MAX( a.remark ) AS remark,
                                MAX( a.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
	                            MAX( a.image_guid ) AS image_guid,
	                            MAX( b.shoes_code ) AS shoes_code,
	                            MAX( b.phase_date ) AS phase_date,
	                            MAX( b.phase_creation_no ) AS phase_creation_no,
                                MAX( p.phase_creation_name ) AS phase_creation_name,
	                            MAX( b.total_production ) AS total_production,
                                MAX( a.is_dqa_mqa_band ) AS is_dqa_mqa_band,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_codes 
                            FROM
	                            qcm_shoes_qa_record_item a
	                            LEFT JOIN QCM_SHOES_QA_RECORD_ITEM_ART t ON a.id = t.item_id 	
	                            LEFT JOIN qcm_shoes_qa_record_d b ON a.d_id = b.id 
                               -- LEFT JOIN qcm_dqa_mag_m y ON t.SHOES_CODE = y.SHOES_CODE
                                LEFT JOIN bdm_phase_creation_m p on b.phase_creation_no=p.phase_creation_no
                                LEFT JOIN bdm_qa_risk_category_m m on a.qa_risk_category_code=m.qa_risk_category_code
                                where a.d_id in({string.Join(",", d_id)}) {where}
		                            GROUP BY
	                            a.id 
                            ORDER BY
	                            a.id DESC";
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("is_dqa_mqa_band", $@"{is_dqa_mqa_band}");
                DataTable dt2 = DB.GetDataTable(sql2, paramTestDic2);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1", dt2);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 各阶段样品记录查看详情查看图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimage_guidItem(object OBJ)
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
                List<string> itemid = jarr.ContainsKey("itemid") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["itemid"].ToString()) : null;//查询条件 鞋型品质记录——品质状况——详情id
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (itemid.Count > 0)
                {
                    where = $@" q.id in ({string.Join(",", itemid.Select(x => $@"'{x}'"))})";
                }
                else
                {
                    where = $@" q.id in ('')";
                }
                string sql = string.Empty;
                DataTable dtimgs = DB.GetDataTable($@"select IMAGE_GUID from qcm_shoes_qa_record_item q where {where}");
                List<string> imgguids = new List<string>();
                foreach (DataRow item in dtimgs.Rows)
                {
                    string[] guids = item["IMAGE_GUID"].ToString().Split(',');
                    for (int i = 0; i < guids.Length; i++)
                    {
                        imgguids.Add(guids[i]);
                    }
                }

                sql = $@"select * from  BDM_UPLOAD_FILE_ITEM where guid in ({string.Join(",", imgguids.Select(x => $@"'{x}'"))})";
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
        /// 跳转DQA管理时查询信息
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
                string shoe_no = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//查询条件 鞋型
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//id
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//id
                                                                                                                                   //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                                                                                                                                   //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                //                string sql = $@"
                //SELECT
                //	LISTAGG (TO_CHAR(b.PROD_NO), ',') WITHIN GROUP (ORDER BY b.SHOE_NO) AS PROD_NO,
                //	SHOE_NO,
                //	MAX (PRODUCT_MONTH) AS PRODUCT_MONTH,
                //	MAX (DEVELOP_SEASON) AS DEVELOP_SEASON,
                //MAX (q.image_guid) AS image_guid,
                //    MAX(T.file_url) AS file_url
                //FROM
                //		bdm_rd_prod b
                //LEFT JOIN qcm_shoes_qa_record_m q ON b.shoe_no = q.shoes_code
                //LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
                //WHERE
                //	1 = 1 d.isdelete='0' AND b.SHOE_NO = '{shoe_no}' AND q.workshop_section_no = '{workshop_section_no}'
                //GROUP BY
                //	b.SHOE_NO";
                string sql = $@"
                            SELECT
	                            (
                                select {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT g.PROD_NO", "g.PROD_NO")}
                                from bdm_rd_prod g where g.SHOE_NO=b.SHOE_NO
	                            ) as PROD_NO,
	                            b.SHOE_NO,
	                            MAX (b.PRODUCT_MONTH) AS PRODUCT_MONTH,
	                            MAX (b.DEVELOP_SEASON) AS DEVELOP_SEASON,
                                MAX (q.image_guid) AS image_guid,
                                MAX(T.file_url) AS file_url,
	                                MAX(bb.name_t) as rule_no,
								MIN(b.TEST_LEVEL) as TEST_LEVEL,
								MAX(b.develop_type) as develop_type,
								MAX(b.user_section) as user_section,
								MAX(b.COL1) as COL1,
								MAX(b.USER_IN_SHOECHARGE) as USER_IN_SHOECHARGE,
								MAX(b.user_technical) as user_technical,
                                MAX(e.qa_principal) as qa_principal,
									MAX(d.NAME_T) as name_t
                            FROM
		                        bdm_rd_prod b
                                LEFT JOIN BDM_RD_STYLE d on b.SHOE_NO=d.SHOE_NO
                                LEFT JOIN qcm_shoes_qa_record_m q ON b.shoe_no = q.shoes_code
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
                                LEFT JOIN qcm_dqa_mag_m f ON b.SHOE_NO = f.shoes_code  AND f.isdelete='0' 
                                LEFT JOIN bdm_rd_style aa ON b.shoe_no=aa.shoe_no
                                LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                                LEFT JOIN bdm_shoe_extend_m e on e.shoe_no=b.SHOE_NO 
                                WHERE
	                            1 = 1  AND b.SHOE_NO = @shoe_no
                            GROUP BY
	                            b.SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);

                sql = $@"
                        SELECT
                        MAX(a.id) AS id,
                        MAX(a.SHOES_CODE) AS SHOES_CODE, -- 鞋型
                        MAX(b.workshop_section_no) AS workshop_section_no, -- 工段
                        '' as IMAGE_GUID, -- 图片guid
                        --MAX(g.FILE_URL) AS FILE_URL,-- 图片地址
                        (
                         select {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.art_code", "f.art_code")} 
                         from QCM_DQA_MAG_D_ART f where f.d_id=a.id
                        ) as art_code, 
                        MAX(a.CHOICE_NO) AS CHOICE_NO, -- 材料编号
                        MAX(a.CHOICE_NAME) AS CHOICE_NAME, 
                        MAX(a.qa_risk_desc) AS qa_risk_desc, -- 品质风险描述
                        MAX(a.inspection_code) AS inspection_code, -- 检测项目编号
                        '' as inspection_name,
                        MAX(a.inspection_type) AS inspection_type, -- 检测项目类型
                        MAX(y.enum_value) AS judge_mode,-- 判断方式
                        MAX(a.JUDGMENT_CRITERIA) as JUDGMENT_CRITERIA,
                        MAX(a.standard_value) AS standard_value, -- 标准值
                        MAX(a.judge_type) AS judge_type, -- 判断类型
                        MAX(a.unit) AS unit, -- 单位
                        MAX(a.other_measures) AS other_measures, -- 其他措施
                        MAX(a.remark) AS remark, -- 备注
                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT e.file_id", "e.file_id")} AS file_id,
                        MAX(a.processing_record) AS processing_record, -- MQA备注
                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT mf.file_id", "mf.file_id")} AS mffile_id,
                        MAX( a.f_insp_dep ) AS f_insp_dep,
                        MAX( a.f_insp_date ) AS f_insp_date,
                        MAX( a.QA_RISK_DETAILS_DESC ) AS QA_RISK_DETAILS_DESC,
                        MAX( a.qa_risk_category_code ) AS qa_risk_category_code,
                        MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
                        MAX( a.f_insp_res ) AS f_insp_res
                        FROM
	                        QCM_DQA_MAG_D a
                        LEFT JOIN QCM_DQA_MAG_M b ON a.M_ID = b.ID AND b.isdelete = '0'
                        LEFT JOIN QCM_DQA_MAG_D_F e ON e.d_id = a.id and e.isdelete='0'
                        LEFT JOIN QCM_DQA_MAG_D_MF mf ON mf.d_id = a.id and mf.isdelete='0'                            
                        --LEFT JOIN BDM_UPLOAD_FILE_ITEM g ON a.image_guid = g.guid
                        LEFT JOIN SYS001M y on a.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                        LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=a.qa_risk_category_code 
                        WHERE 1 = 1 and
                             a.SHOES_CODE = @shoe_no AND a.m_id = @m_id and a.isdelete='0'
                        GROUP BY a.id";
                //AND b.ID = '{id}'
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("shoe_no", $@"{shoe_no}");
                paramTestDic2.Add("m_id", $@"{id}");
                DataTable dt2 = DB.GetDataTable($@"SELECT * FROM ({sql}) ORDER BY id DESC", paramTestDic2);
                List<string> d_idList = new List<string>();
                foreach (DataRow item in dt2.Rows)
                {
                    d_idList.Add(item["id"].ToString());
                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_type"].ToString(), item["inspection_code"].ToString());
                    if (dicInspect.Count > 0 && dicInspect.ContainsKey("INSPECTION_NAME"))
                        item["inspection_name"] = dicInspect["INSPECTION_NAME"].ToString();
                }
                d_idList = d_idList.Distinct().ToList();

                if (d_idList.Count() > 0)
                {
                    string getDidImgSql = $@"
                        SELECT 
	                        s.D_ID,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, $@"s.IMAGE_GUID||':'||s.IS_MAIN||':'||m.FILE_URL", "s.D_ID")} as img_arr 
                        FROM QCM_QA_MAG_D_IMG_S s 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM m ON m.GUID=s.IMAGE_GUID
                        WHERE s.QA_TYPE='0' and s.D_ID IN({string.Join(",", d_idList)}) 
                        GROUP BY s.D_ID
                        ";
                    var imgDt = DB.GetDataTable(getDidImgSql);
                    foreach (DataRow item in dt2.Rows)
                    {
                        string d_id = item["id"].ToString();
                        var findImg = imgDt.Select($@"D_ID={d_id}");
                        if (findImg.Length > 0)
                        {
                            item["IMAGE_GUID"] = findImg[0]["img_arr"].ToString();
                        }
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);



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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeletePicbyGuid(object OBJ)
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
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";//

                if (string.IsNullOrEmpty(guid))
                {
                    throw new Exception("接口参数【" + guid + "】不能为空！");
                }
                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("guid", guid);
                string sql = $@" delete from BDM_UPLOAD_FILE_ITEM where guid = @guid";
                string sql2 = $@"UPDATE qcm_shoes_qa_record_m SET IMAGE_GUID = '' WHERE IMAGE_GUID = @guid";

                DB.ExecuteNonQuery(sql, param);
                DB.ExecuteNonQuery(sql2, param);

                DB.Commit();

                ret.ErrMsg = "successfully deleted！";
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "failed to delete！" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 上传DQA管理页面图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UploadShoeShape_EditImg(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型编号
                string image_guid = jarr.ContainsKey("image_guid") ? jarr["image_guid"].ToString() : "";//查询条件 图片id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from qcm_shoes_qa_record_m where shoes_code='{shoes_code}'");
                if (count > 0)
                    sql = $@"update qcm_shoes_qa_record_m set image_guid='{image_guid}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where shoes_code='{shoes_code}'";
                else
                    sql = $@"insert into qcm_shoes_qa_record_m (shoes_code,image_guid,CREATEBY,CREATEDATE,CREATETIME) values('{shoes_code}','{image_guid}','{user}','{date}','{time}')";

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
        /// 跳转DQA管理时查询页签
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
        /// DQA管理页面添加页签时查询工段
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
                string workshop_section = jarr.ContainsKey("workshop_section") ? jarr["workshop_section"].ToString() : "";//查询条件
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//查询条件 鞋型代号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(workshop_section))
                {
                    where = $@" and (workshop_section_no like @workshop_section or workshop_section_name like @workshop_section or data_source like @workshop_section)";
                }

                string sql = $@"select workshop_section_no,workshop_section_name,data_source,id from bdm_workshop_section_m 
                                where workshop_section_no NOT IN (select workshop_section_no from qcm_dqa_mag_m where isdelete='0' and SHOES_CODE=@shoe_no) {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                paramTestDic.Add("workshop_section", $@"%{workshop_section}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
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
        /// DQA管理页面添加页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Editdqa_mag_m(object OBJ)
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
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型编号
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 编号
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";//查询条件 工段名称
                string data_source = jarr.ContainsKey("data_source") ? jarr["data_source"].ToString() : "";//查询条件 材料/工序数据源
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"insert into qcm_dqa_mag_m (shoes_code,workshop_section_no,workshop_section_name,data_source,CREATEBY,CREATEDATE,CREATETIME) 
                        values('{shoes_code}','{workshop_section_no}','{workshop_section_name}','{data_source}','{user}','{date}','{time}')";

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
        /// DQA管理页面删除页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Deletedqa_mag_m(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 DQA管理id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"update qcm_dqa_mag_m set isdelete='1' where id='{mid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"select id from qcm_dqa_mag_d where m_id='{mid}'";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    sql = $@"update qcm_dqa_mag_d set isdelete='1' where id='{item["id"]}'";
                    DB.ExecuteNonQuery(sql);
                    sql = $@"select id from qcm_dqa_mag_d_f where d_id='{item["id"]}'";
                    DataTable dtt = DB.GetDataTable(sql);
                    foreach (DataRow item1 in dtt.Rows)
                    {
                        sql = $@"update qcm_dqa_mag_d_f set isdelete='1' where id='{item1["id"]}'";
                        DB.ExecuteNonQuery(sql);
                    }

                    sql = $@"select id from qcm_dqa_mag_d_art where d_id='{item["id"]}'";
                    DataTable dttt = DB.GetDataTable(sql);
                    foreach (DataRow item2 in dttt.Rows)
                    {
                        sql = $@"update qcm_dqa_mag_d_art set isdelete='1' where id='{item2["id"]}'";
                        DB.ExecuteNonQuery(sql);
                    }
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
        /// DQA管理页面添加时查询检测项
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
                string keyvalue = jarr.ContainsKey("keyvalue") ? jarr["keyvalue"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string mm = string.Empty;
                string where = string.Empty;
                if (mno == "qx")
                {
                    mm = DB.GetString($@"select workshop_section_no from qcm_dqa_mag_m");
                }
                else
                {
                    mm = DB.GetString($@"select workshop_section_no from qcm_dqa_mag_m where id ='{mno}'");
                }

                string bname = $@"SELECT
                                s.enum_code,
	                            s.enum_value2 
                            FROM
	                            bdm_workshop_section_m m
	                            INNER JOIN bdm_workshop_section_d d ON m.id = d.m_id
	                            LEFT JOIN sys001m s ON d.inspection_type = s.enum_code 
                            WHERE
	                            s.enum_type = 'enum_inspection_type' 
	                            AND m.workshop_section_no = @workshop_section_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("workshop_section_no", $@"{mm}");
                DataTable tablename = DB.GetDataTable(bname, paramTestDic);
                string sql = string.Empty;
                int i = 0;
                string field = "judge_type";
                string qc_type = "qc_type";

                if (!string.IsNullOrWhiteSpace(keyvalue))
                {
                    where = $@" and (standard_value like @standard_value or inspection_code like @standard_value or inspection_name like @standard_value)";
                }
                foreach (DataRow item in tablename.Rows)
                {
                    if (item["enum_value2"].ToString() == "bdm_shoes_check_test_m" || item["enum_value2"].ToString() == "bdm_material_testitem_m" ||
                        item["enum_value2"].ToString() == "bdm_workmanship_testitem_m" || item["enum_value2"].ToString() == "bdm_parts_testitem_m")
                        field = "TO_CHAR(i.judge_type) as  judge_type";
                    else
                        field = "'' as judge_type";

                    if (item["enum_value2"].ToString() != "bdm_shoes_check_test_m" && item["enum_value2"].ToString() != "bdm_parts_testitem_m"
                        && item["enum_value2"].ToString() != "bdm_material_testitem_m" && item["enum_value2"].ToString() != "bdm_workmanship_testitem_m"
                        && item["enum_value2"].ToString() != "bdm_shoes_check_fit_m" && item["enum_value2"].ToString() != "bdm_shoes_check_wear_m"
                        && item["enum_value2"].ToString() != "bdm_shoes_check_function_m" && item["enum_value2"].ToString() != "bdm_shoes_check_safe_m")
                        qc_type = "i.qc_type";
                    else
                        qc_type = "'-' as qc_type";

                    sql += $@"select i.standard_value,i.inspection_code,i.inspection_name,'{item["enum_code"]}' as inspection_type,TO_CHAR(i.judgment_criteria) as judgment_criteria,s.enum_value,{qc_type},{field} from {item["enum_value2"]} i
                            left join sys001m s on i.judgment_criteria=s.enum_code and s.enum_type='enum_judgment_criteria'";
                    if (i < tablename.Rows.Count - 1)
                    {
                        sql += " UNION ";
                    }
                    i++;
                }
                string sqlex = $@"select * from ({sql})t where 1=1 {where}";
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("standard_value", $@"%{keyvalue}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sqlex, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic2);
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
        /// DQA管理页面查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_EditFile(object OBJ)
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
                string[] fjguid = jarr.ContainsKey("fjguid") ? Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jarr["fjguid"].ToString()) : null;//查询条件 fjguid

                string where = string.Empty;
                string sql = $@"select file_url,file_name,'BDM_UPLOAD_FILE_ITEM' as tablename, id,GUID from BDM_UPLOAD_FILE_ITEM where GUID in ({string.Join(",", fjguid.Select(x => $@"'{x}'"))})";
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
        /// DQA管理页面添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Editdqa_mag_d(object OBJ)
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
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";//查询条件 DQA管理id
                DataTable dqa_mag_d = jarr.ContainsKey("dqa_mag_d") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["dqa_mag_d"].ToString()) : new DataTable();//查询条件 鞋型编号
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

                if (dqa_mag_d.Rows.Count > 0)
                {
                    foreach (DataRow item in dqa_mag_d.Rows)
                    {
                        string did = item["did"].ToString();
                        if (string.IsNullOrEmpty(did))
                        {
                            sql = $@"insert into QCM_DQA_MAG_D (shoes_code,m_id,choice_no,choice_name,qa_risk_desc,inspection_code,inspection_name,inspection_type,
                            JUDGMENT_CRITERIA,judge_type,standard_value,unit,other_measures,remark,CREATEBY,CREATEDATE,CREATETIME,QA_RISK_DETAILS_DESC,QA_RISK_CATEGORY_CODE) 
                            values('{shoes_code}','{m_id}','{item["choice_no"]}','{item["choice_name"]}','{item["qa_risk_desc"]}',
                            '{item["inspection_code"]}','{item["inspection_name"]}','{item["inspection_type"]}','{item["judge_modeNo"]}','{item["judge_type"]}','{item["standard_value"]}',
                            '{item["unit"]}','{item["other_measures"]}','{item["remark"]}','{user}','{date}','{time}','{item["qa_risk_details_desc"]}','{item["qa_risk_category_code"]}')";
                            DB.ExecuteNonQuery(sql);
                            did = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_dqa_mag_d");
                            if (!string.IsNullOrWhiteSpace(item["fjguid"].ToString()))
                            {
                                string[] fjguids = item["fjguid"].ToString().Split(',');
                                for (int i = 0; i < fjguids.Length; i++)
                                {
                                    sql = $@"insert into qcm_dqa_mag_d_f (d_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                 values('{did}','{fjguids[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(item["art_code"].ToString()))
                            {
                                string[] art_codes = item["art_code"].ToString().Split(',');
                                for (int i = 0; i < art_codes.Length; i++)
                                {
                                    sql = $@"insert into qcm_dqa_mag_d_art (shoes_code,d_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                    values('{shoes_code}','{did}','{art_codes[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }
                        }
                        else
                        {
                            DB.ExecuteNonQuery($@"update qcm_dqa_mag_d set choice_no='{item["choice_no"]}',
                                                                            choice_name='{item["choice_name"]}',
                                                                            qa_risk_desc='{item["qa_risk_desc"]}',
                                                                            inspection_code='{item["inspection_code"]}',
                                                                            inspection_name='{item["inspection_name"]}',
                                                                            inspection_type='{item["inspection_type"]}',
                                                                            judgment_criteria='{item["judge_modeNo"]}',
                                                                            judge_type='{item["judge_type"]}',
                                                                            standard_value='{item["standard_value"]}',
                                                                            unit='{item["unit"]}',
                                                                            other_measures='{item["other_measures"]}',
                                                                            QA_RISK_DETAILS_DESC='{item["qa_risk_details_desc"]}',QA_RISK_CATEGORY_CODE='{item["qa_risk_category_code"]}',
                                                                            remark='{item["remark"]}',
                                                                            MODIFYBY='{user}',
                                                                            MODIFYDATE='{date}',
                                                                            MODIFYTIME='{time}'
                                                where id={item["did"].ToString()}");

                            DB.ExecuteNonQuery($"delete qcm_dqa_mag_d_f where d_id='{item["did"].ToString()}'");
                            if (!string.IsNullOrWhiteSpace(item["fjguid"].ToString()))
                            {
                                string[] fjguids = item["fjguid"].ToString().Split(',');
                                for (int i = 0; i < fjguids.Length; i++)
                                {
                                    sql = $@"insert into qcm_dqa_mag_d_f (d_id,file_id,CREATEBY,CREATEDATE,CREATETIME) 
                                 values('{item["did"].ToString()}','{fjguids[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }

                            DB.ExecuteNonQuery($"delete qcm_dqa_mag_d_art where d_id='{item["did"].ToString()}'");
                            if (!string.IsNullOrWhiteSpace(item["art_code"].ToString()))
                            {
                                string[] art_codes = item["art_code"].ToString().Split(',');
                                for (int i = 0; i < art_codes.Length; i++)
                                {
                                    sql = $@"insert into qcm_dqa_mag_d_art (shoes_code,d_id,art_code,CREATEBY,CREATEDATE,CREATETIME) 
                                    values('{shoes_code}','{item["did"].ToString()}','{art_codes[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }

                        }

                        //图片处理
                        DB.ExecuteNonQuery($@"delete from QCM_QA_MAG_D_IMG_S where d_id={did}");
                        string image_guid = item["image_guid"].ToString();
                        if (!string.IsNullOrEmpty(image_guid))
                        {
                            List<string> img_list = image_guid.Split(',').ToList();
                            foreach (var img_item in img_list)
                            {
                                List<string> img_info = img_item.Split(":").ToList();
                                DB.ExecuteNonQuery($@"INSERT INTO QCM_QA_MAG_D_IMG_S(D_ID,IMAGE_GUID,IS_MAIN,CREATEBY,CREATEDATE,QA_TYPE) VALUES({did},'{img_info[0]}','{img_info[1]}','{user}',SYSDATE,'0')");
                            }
                        }
                    }
                }
                //else
                //{
                //    ret.ErrMsg = "无数据!";
                //    ret.IsSuccess = false;
                //    return ret;
                //}

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
        /// DQA查看页面查询所有文件
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
                //string[] fjguid = jarr.ContainsKey("fjguid") ? Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jarr["fjguid"].ToString()) : null;//查询条件 fjguid
                string shoes_code = jarr.ContainsKey("shoes_code") ? jarr["shoes_code"].ToString() : "";//查询条件 鞋型

                string where = string.Empty;
                string sql = $@"SELECT
	                            f.id,
	                            t.file_url,
	                            t.file_name,
	                            'qcm_dqa_mag_d_f' as tablename
                            FROM
                                qcm_dqa_mag_d d
	                            LEFT JOIN qcm_dqa_mag_d_f f on d.id=f.d_id
	                            INNER JOIN BDM_UPLOAD_FILE_ITEM t ON  f.file_id = t.guid
                            where d.shoes_code='{shoes_code}'";
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 mid
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (arts.Count > 0)
                {
                    where = $@" and t.art_code in ({string.Join(",", arts.Select(x => $@"'{x}'"))})";
                }
                string sql = string.Empty;
                if (mid == "qx")
                {
                    sql = $@"SELECT
                                d.id,
	                            MAX(m.workshop_section_name) as workshop_section_name,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_code,
	                            MAX(d.choice_no) as choice_no,
	                            MAX(d.choice_name) as choice_name,
                                MAX(d.qa_risk_desc) AS qa_risk_desc, -- 品质风险描述
	                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_name) as inspection_name,
                                '' as imageguids,
	                            '' as FILE_URL,
	                            MAX(d.inspection_type) as inspection_type,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures,
	                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS file_id
                            FROM
	                            qcm_dqa_mag_d d
	                            left join qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
	                            left join qcm_dqa_mag_d_art t on d.id=t.d_id and t.isdelete='0'
                                -- left join sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
	                            -- left join BDM_UPLOAD_FILE_ITEM u on d.image_guid=u.guid
	                            left join qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
                                where d.isdelete='0' and m.shoes_code=@shoe_no {where}
	                            GROUP BY d.id";
                }
                else
                {
                    sql = $@"SELECT
                                d.id,
	                            MAX(m.workshop_section_name) as workshop_section_name,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_code,
	                            MAX(d.choice_no) as choice_no,
	                            MAX(d.choice_name) as choice_name,
                                MAX(d.qa_risk_desc) AS qa_risk_desc, -- 品质风险描述
	                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_name) as inspection_name,
	                            '' as FILE_URL,
                                '' as imageguids,
	                            MAX(d.inspection_type) as inspection_type,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures,
	                            MAX(d.QA_RISK_DETAILS_DESC) as QA_RISK_DETAILS_DESC,
                                MAX( d.qa_risk_category_code ) AS qa_risk_category_code,
                                MAX( cm.qa_risk_category_name ) AS qa_risk_category_name,
		                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT f.file_id", "f.file_id")} AS file_id
                            FROM
	                            qcm_dqa_mag_d d
	                            left join qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
	                            left join qcm_dqa_mag_d_art t on d.id=t.d_id and t.isdelete='0'
                                -- left join sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
	                            -- left join BDM_UPLOAD_FILE_ITEM u on d.image_guid=u.guid
	                            left join qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
                                where d.isdelete='0' and d.m_id=@m_id and m.shoes_code=@shoe_no {where}
	                            GROUP BY d.id";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("m_id", $@"{mid}");
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = CommonBASE.GetPageDataTable(DB, $@"SELECT * FROM ({sql}) ORDER BY id DESC", int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                List<string> d_idList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    d_idList.Add(item["id"].ToString());
                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_type"].ToString(), item["inspection_code"].ToString());
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
                    //-------------
                    string[] file_ids = item["file_id"].ToString().Split(',');
                    string file_id = string.Empty;
                    file_ids = file_ids.GroupBy(p => p).Select(p => p.Key).ToArray();
                    for (int i = 0; i < file_ids.Length; i++)
                    {
                        file_id += file_ids[i] + ",";
                    }
                    item["file_id"] = file_id.TrimEnd(',');
                }
                d_idList = d_idList.Distinct().ToList();
                if (d_idList.Count() > 0)
                {
                    string getDidImgSql = $@"
                        SELECT 
	                        s.D_ID,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, $@"s.IMAGE_GUID||':'||s.IS_MAIN||':'||m.FILE_URL", "s.D_ID")} as img_arr 
                        FROM QCM_QA_MAG_D_IMG_S s 
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM m ON m.GUID=s.IMAGE_GUID
                        WHERE s.QA_TYPE='0' and s.D_ID IN({string.Join(",", d_idList)}) 
                        GROUP BY s.D_ID
                        ";
                    var imgDt = DB.GetDataTable(getDidImgSql);
                    foreach (DataRow item in dt.Rows)
                    {
                        string d_id = item["id"].ToString();
                        var findImg = imgDt.Select($@"D_ID={d_id}");
                        if (findImg.Length > 0)
                        {
                            item["FILE_URL"] = findImg[0]["img_arr"].ToString();
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
                        string d_id = item["id"].ToString();
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 mid
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (arts.Count > 0)
                {
                    where = $@" and t.art_code in ({string.Join(",", arts.Select(x => $@"'{x}'"))})";
                }
                string sql = string.Empty;
                if (mid == "qx")
                {
                    sql = $@"SELECT
	                            MAX(m.workshop_section_name) as workshop_section_name,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                                MAX(d.qa_risk_desc) AS qa_risk_desc, -- 品质风险描述
                                MAX(cm.qa_risk_category_name) AS qa_risk_category_name, 
                                MAX(d.QA_RISK_DETAILS_DESC) AS QA_RISK_DETAILS_DESC, 
	                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_name) as inspection_name,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.inspection_type) as inspection_type,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures
                            FROM
	                            qcm_dqa_mag_d d
	                            left join qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
	                            left join qcm_dqa_mag_d_art t on d.id=t.d_id and t.isdelete='0'
                                -- left join sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
	                            left join BDM_UPLOAD_FILE_ITEM u on d.image_guid=u.guid
	                            left join qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
                                where d.isdelete='0' and m.shoes_code=@shoe_no {where}
	                            GROUP BY d.id";
                }
                else
                {
                    sql = $@"SELECT
	                            MAX(m.workshop_section_name) as workshop_section_name,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT t.art_code", "t.art_code")} AS art_code,
	                            MAX(d.choice_name) as choice_name,
                                MAX(d.qa_risk_desc) AS qa_risk_desc, -- 品质风险描述
                                MAX(cm.qa_risk_category_name) AS qa_risk_category_name, 
                                MAX(d.QA_RISK_DETAILS_DESC) AS QA_RISK_DETAILS_DESC, 
	                            MAX(d.inspection_code) as inspection_code,
                                MAX(d.inspection_name) as inspection_name,
	                            MAX(y.enum_value) as judge_mode,
	                            MAX(d.inspection_type) as inspection_type,
	                            MAX(d.standard_value) as standard_value,
	                            MAX(d.unit) as unit,
	                            MAX(d.remark) as remark,
	                            MAX(d.other_measures) as other_measures
                            FROM
	                            qcm_dqa_mag_d d
	                            left join qcm_dqa_mag_m m on d.m_id=m.id and m.isdelete='0'
	                            left join qcm_dqa_mag_d_art t on d.id=t.d_id and t.isdelete='0'
                                -- left join sys001m s on d.inspection_type=s.enum_code and s.enum_type='enum_inspection_type'
	                            left join BDM_UPLOAD_FILE_ITEM u on d.image_guid=u.guid
	                            left join qcm_dqa_mag_d_f f on d.id=f.d_id and f.isdelete='0'
                                LEFT JOIN SYS001M y on d.JUDGMENT_CRITERIA=y.ENUM_CODE and y.ENUM_TYPE='enum_judgment_criteria' 
                                LEFT JOIN bdm_qa_risk_category_m cm on cm.qa_risk_category_code=d.qa_risk_category_code 
                                where d.isdelete='0' and d.m_id=@m_id and m.shoes_code=@shoe_no {where}
	                            GROUP BY d.id";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("m_id", $@"{mid}");
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    Dictionary<string, object> dicInspect = BASE.GetInspectionDataTable(DB, item["inspection_type"].ToString(), item["inspection_code"].ToString());
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
        /// DQA查看页面查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoeShape_ListFileguid(object OBJ)
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
                string fjguid = jarr.ContainsKey("file_id") ? jarr["file_id"].ToString() : "";//查询条件 fjguid
                string[] file_id = fjguid.Split(',');

                string where = string.Empty;
                string sql = $@"SELECT
	                            f.id,
	                            t.file_url,
	                            t.file_name,
	                            'qcm_dqa_mag_d_f' as tablename
                            FROM
	                            qcm_dqa_mag_d_f f
	                            INNER JOIN BDM_UPLOAD_FILE_ITEM t ON  f.file_id = t.guid
                                where f.file_id in ({string.Join(",", file_id.Select(x => $@"'{x}'"))})";
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
        /// DQA管理页面查询判断方式
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getjudge_mode(object OBJ)
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
                //string fjguid = jarr.ContainsKey("file_id") ? jarr["file_id"].ToString() : "";//查询条件 fjguid
                //string[] file_id = fjguid.Split(',');

                string where = string.Empty;
                string sql = $@"select ENUM_CODE,ENUM_VALUE from SYS001M where enum_type='enum_judge_symbol'";
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
        /// DQA管理页面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Deletedqa_mag_d(object OBJ)
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
                sql = $@"delete qcm_dqa_mag_d where id='{did}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"delete qcm_dqa_mag_d_f where d_id='{did}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"delete qcm_dqa_mag_d_art where d_id='{did}'";
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
        /// 编辑鞋型品质管理页面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Deleterecord_item(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 鞋型品质记录——品质状况id
                string itemid = jarr.ContainsKey("itemid") ? jarr["itemid"].ToString() : "";//查询条件 鞋型品质记录——品质状况——详情id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;

                sql = $@"delete from qcm_shoes_qa_record_item where d_id='{did}' and id='{itemid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from qcm_shoes_qa_record_item_f where item_id='{itemid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from qcm_shoes_qa_record_item_art where item_id='{itemid}'";
                DB.ExecuteNonQuery(sql);

                //判断表身是否还有数据
                string count = DB.GetStringline($@"select count(1) from qcm_shoes_qa_record_item where d_id='{did}'");
                if (Convert.ToInt32(count) <= 0)
                {
                    DB.ExecuteNonQueryOffline($@"delete from qcm_shoes_qa_record_d where id='{did}'");
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetGd(object OBJ)
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
                //转译shoes_no
                string shoes_no = jarr.ContainsKey("shoes_no") ? jarr["shoes_no"].ToString() : "";//查询条件 
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;

                sql = $@"select workshop_section_no as code,workshop_section_name as value from qcm_dqa_mag_m  where shoes_code = '{shoes_no}'";

                DataTable dt = DB.GetDataTable(sql);

                if (dt.Rows.Count == 0)
                {
                    string sql2 = $@"select workshop_section_no,workshop_section_name,data_source,id from bdm_workshop_section_m";
                    DataTable wendt = DB.GetDataTable(sql2);
                    if (wendt.Rows.Count > 0)
                    {
                        foreach (DataRow item in wendt.Rows)
                        {
                            sql2 = $@"insert into qcm_dqa_mag_m (shoes_code,workshop_section_no,workshop_section_name,data_source,CREATEBY,CREATEDATE,CREATETIME) 
                        values('{shoes_no}', '{item["workshop_section_no"]}', '{item["workshop_section_name"]}', '{item["data_source"]}', '{user}', '{date}', '{time}')";
                            DB.ExecuteNonQuery(sql2);
                        }
                        //DB.ExecuteNonQuery($@"BEGIN {sql} END;");
                        DB.Commit();
                    }

                    dt = DB.GetDataTablebyline(sql);
                }
                

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
    }
}
