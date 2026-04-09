using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.DBHelper;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SJ_TQCAPI.DTO;

namespace SJ_TQCAPI
{
    public class TQC_Task
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckTQC_Task_Main_OP(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_id = jarr.ContainsKey("task_id") ? jarr["task_id"].ToString() : "";//task_id
                string user_code = jarr.ContainsKey("user_code") ? jarr["user_code"].ToString() : "";//user_code
                #endregion

                #region 逻辑

                var exist = DB.GetInt32($@"
SELECT 
	count(1)
FROM tqc_task_m 
WHERE ID={task_id} and CREATEBY='{user_code}'");

                bool isOK = exist > 0 ? true : false;

                ret.IsSuccess = true;
                ret.RetData = isOK.ToString().ToLower();
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }
          //ToTest the TFS updating code or not while check in and check out
        /// <summary>
        /// tqc主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Main(object OBJ)
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
                string task_state = jarr.ContainsKey("task_state") ? jarr["task_state"].ToString() : "";//任务状态
                string datestart = jarr.ContainsKey("datestart") ? jarr["datestart"].ToString() : "";//查询条件 日期开始
                string dateend = jarr.ContainsKey("dateend") ? jarr["dateend"].ToString() : "";//查询条件 日期结束
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//查询条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//查询条件 mer_po

                string se_id = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";// Newly Added to SE_ID 

                //string workshop_section = jarr.ContainsKey("workshop_section") ? jarr["workshop_section"].ToString() : "";//查询条件 工段 
                string department = jarr.ContainsKey("department") ? jarr["department"].ToString() : "";//查询条件 部门
                string production_line = jarr.ContainsKey("production_line") ? jarr["production_line"].ToString() : "";//查询条件 组别
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(datestart) && !string.IsNullOrWhiteSpace(dateend))
                {
                    where += $@" and t.createdate>=@datestart and t.createdate<=@dateend ";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(datestart))
                    {
                        where += $@" and t.createdate>='{datestart}'";
                    }
                    if (!string.IsNullOrWhiteSpace(dateend))
                    {
                        where += $@" and t.createdate<='{dateend}'";
                    }
                }
                if (!string.IsNullOrWhiteSpace(shoe_no))
                {
                    where += $@" and d.name_t like @shoe_no";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += $@" and t.prod_no like @prod_no";
                }
                //if (!string.IsNullOrWhiteSpace(workshop_section))
                //{
                //    where += $@" and (t.workshop_section_no like '%{workshop_section}%' or  w.workshop_section_name like '%{workshop_section}%')";
                //}
                if (!string.IsNullOrWhiteSpace(department))
                {
                    where += $@" and t.department like @department";
                }
                if (!string.IsNullOrWhiteSpace(mer_po))
                {
                    where += $@" and t.mer_po like @mer_po";
                }
                if (!string.IsNullOrWhiteSpace(production_line))
                {
                    where += $@" and (t.production_line_code like @production_line or  c.department_name like @production_line)";
                }
                //edit on 6/26(PO Change)
                if (!string.IsNullOrWhiteSpace(se_id))
                {
                    where += $@" and (t.se_id like @se_id )";
                }
                List<string> task_statelist = task_state.Split(',').ToList();// Added  t.se_id
                string sql = $@"SELECT
                                t.ID,
                                t.mer_po,
	                            t.task_no,
	                            t.createdate,
	                            t.department,
	                            t.production_line_code,
	                            --b.production_line_name,
                                c.department_name as production_line_name,
	                            t.shoe_no,
	                            t.prod_no,
                                t.se_id, --Edit on 6/26(PO Change)
	                            r.name_t,                              
	                            '' as total,
	                            '' as qualified,
	                            '' as FirstQualifiedNum,
	                            '' as bnum,
	                            '' as totalpass,
	                            '' as rftpass,
                                '' as problems,
                                '' as badNum, 
                                '' as aqlresult,
	                            CASE t.task_state
	                            WHEN '0' THEN
		                            'In progress'
		                            WHEN '1' THEN
		                            'Stop line'
		                            WHEN '2' THEN
		                            'Over'
                                    WHEN '3' THEN
		                            'Re_Inspection_In progress'
                                     WHEN '4' THEN
		                            'Re_Inspection_Stopped'
                            END as task_state,
                            t.workshop_section_no,
                            w.workshop_section_name,
                            d.name_t as name_tt
                            FROM 
	                            tqc_task_m t
                                LEFT JOIN BDM_RD_STYLE d on t.SHOE_NO=d.SHOE_NO
	                            --LEFT JOIN bdm_production_line_m b on t.production_line_code=b.production_line_code
                                LEFT JOIN BASE005M c on t.production_line_code = c.department_code
	                            LEFT JOIN bdm_rd_prod r on t.prod_no=r.prod_no
	                            LEFT JOIN bdm_workshop_section_m w on t.workshop_section_no=w.workshop_section_no
                            where t.task_state in ({string.Join(',', task_statelist.Select(x => $@"'{x}'"))})  {where}  order by REPLACE (t.CREATEDATE, '-', '') || REPLACE (t.CREATETIME, ':', '') desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("datestart", $@"{datestart}");
                paramTestDic.Add("dateend", $@"{dateend}");
                paramTestDic.Add("shoe_no", $@"%{shoe_no}%");
                paramTestDic.Add("prod_no", $@"%{prod_no}%");
                paramTestDic.Add("department", $@"%{department}%");
                paramTestDic.Add("mer_po", $@"%{mer_po}%");
                paramTestDic.Add("production_line", $@"%{production_line}%");
               // paramTestDic.Add("se_id", $@"%{se_id}%");//Edit on 6/26(PO Change)
                paramTestDic.Add("se_id", $@"%{se_id}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                List<string> PoList = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    int First_Time_Qualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{item["task_no"]}' 	AND commit_type = '0'  GROUP BY	COMMIT_INDEX)t");
                    int First_Time_UnQualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{item["task_no"]}' 	AND commit_type = '1'  GROUP BY	COMMIT_INDEX)t");
                    int Rework_Qualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{item["task_no"]}' 	AND commit_type = '2'  GROUP BY	COMMIT_INDEX)t");
                    int BGrade = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{item["task_no"]}' 	AND commit_type = '4'  GROUP BY	COMMIT_INDEX)t");
                    int Rework_UnQualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{item["task_no"]}' 	AND commit_type = '3'  GROUP BY	COMMIT_INDEX)t");
                    item["total"] = First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified;
                    item["qualified"] = First_Time_Qualified + Rework_Qualified;
                    item["bnum"] = BGrade;
                    item["FirstQualifiedNum"] = First_Time_Qualified;
                    item["badNum"] = First_Time_UnQualified;
                    try
                    {
                        item["totalpass"] = Convert.ToDecimal((First_Time_Qualified + Rework_Qualified)) / Convert.ToDecimal((First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified));
                    }
                    catch (Exception)
                    {

                        item["totalpass"] = 0;
                    }
                    try
                    {
                        item["rftpass"] = Convert.ToDecimal(First_Time_Qualified) / Convert.ToDecimal(First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified);
                    }
                    catch (Exception)
                    {
                        item["rftpass"] = 0;
                    }
                    #region Top3Issues
                    //string top3Sql = $@" with tmp as (select * from (select  a.task_no,b.inspection_name ,count(1) as total, ROW_NUMBER ( ) over ( partition by a.task_no ORDER BY count(1) DESC ) ranking 
                    //                                from tqc_task_detail_t a 
                    //                                left join tqc_task_item_c b on a.union_id = b.id
                    //                                group by b.inspection_name,a.task_no) a 
                    //                                 where a.ranking <4 ) 
                    //  SELECT 
                    //  distinct
                    //  --a.task_no,
                    //  b.inspection_name INSPECTION_NAME_1,
                    //  b.total Fail_Quantity_1,
                    //  c.inspection_name INSPECTION_NAME_2,
                    //  c.total Fail_Quantity_2,
                    //  d.inspection_name INSPECTION_NAME_3,
                    //  d.total Fail_Quantity_3 
                    //  FROM
                    //  tqc_task_detail_t a  
                    //  LEFT JOIN tmp b ON  a.task_no= b.task_no
                    //  AND b.ranking = 1
                    //  LEFT JOIN tmp c ON  a.task_no= c.task_no
                    //  AND c.ranking = 2
                    //  LEFT JOIN tmp d ON a.task_no= d.task_no
                    //  AND d.ranking =3 where a.task_no='{item["task_no"]}'";


                    ////string top3Sql = $@"select inspection_name 
                    ////                    from (select b.inspection_name ,count(1) as count 
                    ////                            from tqc_task_detail_t a 
                    ////                            left join tqc_task_item_c b on a.union_id = b.id
                    ////                            where a.task_no = '{item["task_no"]}' 
                    ////                            group by b.inspection_name
                    ////                            order by count desc)
                    ////                    where rownum<=3";
                    //DataTable problemTable = DB.GetDataTable(top3Sql);
                    //foreach (DataRow row in problemTable.Rows)
                    //{
                    //    item["INSPECTION_NAME_1"] = row["INSPECTION_NAME_1"].ToString();
                    //    item["INSPECTION_NAME_2"] = row["INSPECTION_NAME_2"].ToString();
                    //    item["INSPECTION_NAME_3"] = row["INSPECTION_NAME_3"].ToString();
                    //    item["Fail_Quantity_1"] = row["Fail_Quantity_1"].ToString();
                    //    item["Fail_Quantity_2"] = row["Fail_Quantity_2"].ToString();
                    //    item["Fail_Quantity_3"] = row["Fail_Quantity_3"].ToString();
                    //}
                    #endregion
                    #region AQLResultAdd
                    //if (item["workshop_section_no"].ToString() == "L")
                    //{
                    //    string PO = item["mer_po"].ToString();

                    //    string SQL = $@" SELECT  
                    //                        a.task_no,
                    //                        a.po, 
                    //                        a.lot_num, 
                    //                        a.inspection_state,
                    //                        '' as inspection_results,
                    //                        c.sample_level,
                    //                        c.aql_level
                    //                        FROM
                    //                        aql_cma_task_list_m a  
                    //                        LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no=a.task_no  
                    //left join (
                    //select 
                    //    a.po,
                    //    listagg(DISTINCT a.from_line, ',') WITHIN GROUP (ORDER BY a.from_line) as from_line
                    //FROM
                    //  mms_finishedtrackin_list a 
                    //group by a.po
                    //) cx on cx.po=a.po 
                    //                        where a.po  ='{PO}' ";

                    //    DataTable dt2 = DB.GetDataTable(SQL);

                    //    string insResSql = $@"
                    //                SELECT
                    //                    a.task_no,
                    //                    b.po,
                    //                    MAX(a.bad_classify_code) as bad_classify_code,
                    //                    MAX(a.bad_item_code) as bad_item_code,
                    //                    MAX(a.bad_item_name) as bad_item_name,
                    //                    MAX(a.problem_level) as problem_level,
                    //                    MAX(a.bad_qty) as bad_qty
                    //                from 
                    //                    aql_cma_task_list_m_aql_e_br a 
                    //                    left join aql_cma_task_list_m b on b.task_no=a.task_no
                    //                Where b.po ='{PO}'
                    //                GROUP BY a.task_no,b.po,a.bad_classify_code,a.bad_item_code";
                    //    DataTable insResDt = DB.GetDataTable(insResSql);

                    //    foreach (DataRow items in dt2.Rows)
                    //    {
                    //        if (items["inspection_state"].ToString() == "1")
                    //        {
                    //            string task_no = items["task_no"].ToString();
                    //            if (!string.IsNullOrEmpty(task_no))
                    //            {
                    //                int zy = 0;//主要
                    //                int cy = 0;//次要
                    //                int yz = 0;//严重
                    //                var findTaskList = insResDt.Select($@"task_no='{task_no}'");
                    //                if (findTaskList.Length > 0)
                    //                {
                    //                    foreach (var task_item in findTaskList)
                    //                    {
                    //                        switch (task_item["problem_level"])
                    //                        {
                    //                            case "0":
                    //                                zy += Convert.ToInt32(task_item["bad_qty"].ToString());
                    //                                break;
                    //                            case "1":
                    //                                cy += Convert.ToInt32(task_item["bad_qty"].ToString());
                    //                                break;
                    //                            case "2":
                    //                                yz += Convert.ToInt32(task_item["bad_qty"].ToString());
                    //                                break;
                    //                            default:
                    //                                break;
                    //                        }
                    //                    }
                    //                }

                    //                int hjbl = cy + zy + yz;//合计不良

                    //                //foreach (DataRow item2 in dt2.Rows)
                    //                //{
                    //                string sample_level = items["sample_level"].ToString();
                    //                string aql_level = items["aql_level"].ToString();
                    //                if (string.IsNullOrEmpty(sample_level) || string.IsNullOrEmpty(sample_level))
                    //                {
                    //                    sample_level = "2";
                    //                    aql_level = "AC13";
                    //                }
                    //                string ac = aql_level;//查询条件 ac
                    //                string num = items["lot_num"].ToString();//查询条件 任务数量(分批数量)
                    //                string LEVEL_TYPE = sample_level;//查询条件 样本级别
                    //                string acSql = $@"select VALS, {ac} as AC from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                    //                DataTable acDt = DB.GetDataTable(acSql);

                    //                if (acDt.Rows.Count > 0)
                    //                {
                    //                    int acInt = 0;
                    //                    bool cRes = int.TryParse(acDt.Rows[0]["AC"].ToString(), out acInt);
                    //                    if (cRes)
                    //                    {
                    //                        if (item["aqlresult"].ToString() == "Rejected")
                    //                        {

                    //                        }
                    //                        else
                    //                        {
                    //                            if (hjbl > acInt)
                    //                                item["aqlresult"] = "Rejected";
                    //                            else
                    //                                item["aqlresult"] = "Accepted";
                    //                        }
                    //                    }
                    //                }

                    //                //}
                    //            }

                    //        }
                    //        else
                    //        {
                    //            if (string.IsNullOrEmpty(item["aqlresult"].ToString()))
                    //            {
                    //                item["aqlresult"] = "Not_Inspected";
                    //            }
                    //        }
                    //    }
                    //}
                    #endregion;
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("data2", Dt2);
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
        /// tqc创建页面查询产线和部门（多选）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductLineAndDepartment(object OBJ)
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
                string productLine = jarr.ContainsKey("productLine") ? jarr["productLine"].ToString() : "";//查询条件 产线
                string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";//查询条件 产线

                string sql = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrEmpty(productLine))
                {
                    where += $@" and (department_code like '%{productLine}%' or department_name like '%{productLine}%')";
                }
                if (!string.IsNullOrEmpty(depart))
                {
                    where += $@" and udf07 like '%{depart}%'";
                }

                sql = $@"select department_code as productLine,department_name as productLineName,udf07 as depart from base005m where 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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
        /// tqc创建页面查询下拉框数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Edit_Com_List(object OBJ)
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

                string sql = string.Empty;

                sql = $@"select eq_info_no,eq_info_name from bdm_eq_info_m";
                DataTable dt1 = DB.GetDataTable(sql);

                sql = $@"select WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME from bdm_workshop_section_m";
                DataTable dt2 = DB.GetDataTable(sql);

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

        /// <summary>
        /// tqc创建页面查询po数据
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
                string se_id = jarr.ContainsKey("se_id") ? jarr["se_id"].ToString() : "";//art
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
                if (!string.IsNullOrWhiteSpace(se_id))  //Edit on 6/26(PO Change)
                {
                    List<string> se_idList = se_id.Split(',').ToList();
                    se_id = string.Join(',', se_idList.Select(x => $@"'{x}'"));

                    where += $@" and m.se_id in ({se_id})";
                }
                string sql = string.Empty;
                sql = $@"	select m.mer_po,r.prod_no,m.se_id from BDM_SE_ORDER_MASTER m
                        LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO 
						where r.prod_no is not null {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keycode", $@"%{keycode}%");
                paramTestDic.Add("prod_no", $@"{art}");
                paramTestDic.Add("se_id", $@"{se_id}");
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
        /// tqc创建页面查询ART数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Edit_ART(object OBJ)
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
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and (prod_no like @keycode or name_t like @keycode)";
                }

                string sql = string.Empty;
                sql = $@"SELECT
	                    prod_no,
	                    name_t
                    FROM
	                    BDM_RD_PROD where 1=1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keycode", $@"%{keycode}%");
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
        /// tqc创建页面根据art查询鞋型和季节
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoe_no_jijie(object OBJ)
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
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//查询条件 mer_po
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//查询条件 art

                string sql = string.Empty;
                string prod_no = "";
                if (!string.IsNullOrEmpty(mer_po))
                {
                    string[] pos = mer_po.Split(',');
                    sql = $@"select r.prod_no from BDM_SE_ORDER_MASTER m
                        LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
						where r.prod_no is not null and m.mer_po in ({string.Join(",", pos.Select(x => $@"'{x}'"))}) GROUP BY r.prod_no";
                    DataTable count = DB.GetDataTable(sql);
                    if (count.Rows.Count > 1)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "The selected PO contains multiple ARTs and cannot be created!";
                        return ret;
                    }
                    prod_no = DB.GetString($@"select r.prod_no from BDM_SE_ORDER_MASTER m
                        LEFT JOIN BDM_SE_ORDER_ITEM e ON m.SE_ID = e.SE_ID
	                    LEFT JOIN bdm_rd_prod r ON e.prod_no = r.PROD_NO
						where r.prod_no is not null and m.mer_po in ({string.Join(",", pos.Select(x => $@"'{x}'"))}) GROUP BY r.prod_no");
                }
                else
                {
                    prod_no = art;
                }

                sql = $@"SELECT
	                    r.SHOE_NO,
	                    r.DEVELOP_SEASON,
	                    r.PROD_NO,
	                    r.user_section,
	                    r.USER_IN_SHOECHARGE,
	                    r.user_technical,
	                    s.qa_principal,
	                    y.style_seq,
	                    r.develop_type,
	                    f.file_url,
                        m.se_id,
                        m.workorder_no,
                        r.MOLD_NO,
                        l.name_t
                    FROM
	                    bdm_rd_prod r
                    LEFT JOIN BDM_SE_ORDER_ITEM E ON r .PROD_NO = E .PROD_NO
                    LEFT JOIN BDM_SE_ORDER_MASTER M ON E .SE_ID = M.SE_ID
                    LEFT JOIN BDM_RD_STYLE l ON r.SHOE_NO = l.SHOE_NO
                    LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
                    LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
                    LEFT JOIN qcm_shoes_qa_record_m D ON r.shoe_no = D .shoes_code
                    LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON D .image_guid = f.guid
                    WHERE
	                    r.prod_no = @prod_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("prod_no", $@"{prod_no}");
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
        /// tqc创建页面根据art查询鞋型和季节
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoe_no_jijiebyART(object OBJ)
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 art

                string sql = string.Empty;
                

                sql = $@"
SELECT
	r.SHOE_NO,
	r.DEVELOP_SEASON,
	r.PROD_NO,
	r.user_section,
	r.USER_IN_SHOECHARGE,
	r.user_technical,
	s.qa_principal,
	y.style_seq,
	r.develop_type,
	f.file_url,
	m.se_id,
	m.workorder_no,
	r.MOLD_NO,
	l.name_t
FROM
	bdm_rd_prod r
LEFT JOIN BDM_SE_ORDER_ITEM e ON e.PROD_NO = r.PROD_NO
LEFT JOIN BDM_SE_ORDER_MASTER m ON m.SE_ID = e.SE_ID
LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO
LEFT JOIN bdm_shoe_extend_m s ON r.SHOE_NO = s.SHOE_NO
LEFT JOIN bdm_rd_style y ON s.SHOE_NO = y.SHOE_NO
LEFT JOIN qcm_shoes_qa_record_m d ON r.shoe_no = d.shoes_code
LEFT JOIN BDM_UPLOAD_FILE_ITEM f ON d.IMAGE_GUID = f.guid
WHERE r.prod_no = @prod_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("prod_no", $@"{prod_no}");
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
        /// tqc创建页面根据产线查询部门
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDepartment(object OBJ)
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
                string department_code = jarr.ContainsKey("department_code") ? jarr["department_code"].ToString() : "";//查询条件 产线

                string sql = string.Empty;

                sql = $@"select department_code,udf07 from base005m where department_code=@department_code";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("department_code", $@"{department_code}");
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
        /// tqc创建页面生成任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Task_Edit_Insert(object OBJ)
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
                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";//条件 季度
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//条件 鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//条件 art
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//条件 工段编号
                string department = jarr.ContainsKey("department") ? jarr["department"].ToString() : "";//条件 部门
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";//条件 产线
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";//条件 机台
                string mold_no = jarr.ContainsKey("mold_no") ? jarr["mold_no"].ToString() : "";//条件 模号
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//条件 po
                string se_id = jarr.ContainsKey("se_id") ? jarr["se_id"].ToString() : "";//条件 制令
                string workorderno = jarr.ContainsKey("workorderno") ? jarr["workorderno"].ToString() : "";//条件 销售订单号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string task_no = string.Empty;//任务编号

                task_no = DB.GetString($@"SELECT task_no FROM(select task_no from tqc_task_m where CREATEDATE = '{DateTime.Now:yyyy-MM-dd}'  order by TO_NUMBER(REPLACE(task_no, 'T', '')) DESC)t WHERE ROWNUM = 1");
                if (string.IsNullOrWhiteSpace(task_no))
                {
                    task_no = "T" + DateTime.Now.ToString("yyyyMMdd") + "1";
                }
                else
                {
                    task_no = $@"T{DateTime.Now.ToString("yyyyMMdd")}" + (Convert.ToInt32(task_no.Replace($@"T{DateTime.Now.ToString("yyyyMMdd")}", "")) + 1);
                }

                string sql = string.Empty;
                sql = $@"insert into tqc_task_m (mer_po,se_id,workorderno,task_no,workshop_section_no,develop_season,shoe_no,prod_no,production_line_code,eq_info_no,mold_no,
                        department,task_state,createby,createdate,createtime) 
                        values('{mer_po}','{se_id}','{workorderno}','{task_no}','{workshop_section_no}','{develop_season}','{shoe_no}','{prod_no}','{production_line_code}',
                        '{eq_info_no}','{mold_no}','{department}','0','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");
                string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                string tabname = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{inspection_type}' and enum_code in ('0','1','2','3','4','5','6','7')");
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
                            WHERE qc_type='1' and ROWNUM<=20
                            ORDER BY id asc";
                    DataTable inspectiondt = DB.GetDataTable(sql);
                    foreach (DataRow item in inspectiondt.Rows)
                    {
                        sql = $@"insert into tqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,
                                standard_value,shortcut_key,createby,createdate,createtime) 
                                values('{task_no}','{inspection_type}','{item["inspection_code"]}','{item["inspection_name"]}','{item["qc_type"]}',
                                '{item["judgment_criteria"]}','{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// tqc生成任务之后或继续录入查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTask_Edit(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string sql = string.Empty;
                sql = $@"SELECT
	                            task_no,
	                            '' as total,
	                            '' as qualified,
	                            '' as bnum,
	                            '' as totalpass,
	                            '' as rftpass,
                                '' as fx,
                                '' as Rework_UnQualified,
                                '' as fxrft,
                                '' as First_Time_UnQualified,
                                '' as schgzs,
                                '' as scbhhzs,
                                '' as fxhgs
                            FROM 
	                            tqc_task_m where task_no=@task_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt1 = DB.GetDataTable(sql, paramTestDic);
                //Enum: 0 Normal and qualified, 1 Normal and unqualified, 2 Repaired and qualified, 3 Repaired and unqualified, 4 B-grade product

                foreach (DataRow item in dt1.Rows)
                {
                    int First_Time_Qualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{task_no}' 	AND commit_type = '0'  GROUP BY	COMMIT_INDEX)t");
                    int First_Time_UnQualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{task_no}' 	AND commit_type = '1'  GROUP BY	COMMIT_INDEX)t");
                    int Rework_Qualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{task_no}' 	AND commit_type = '2'  GROUP BY	COMMIT_INDEX)t");
                    int BGrade = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{task_no}' 	AND commit_type = '4'  GROUP BY	COMMIT_INDEX)t");
                    int Rework_UnQualified = DB.GetInt32($@"SELECT COUNT(1) FROM( SELECT COMMIT_INDEX FROM	tqc_task_commit_m WHERE	TASK_NO = '{task_no}' 	AND commit_type = '3'  GROUP BY	COMMIT_INDEX)t");
                    item["total"] = First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified;
                    item["qualified"] = First_Time_Qualified + Rework_Qualified;
                    item["bnum"] = BGrade;
                    item["First_Time_UnQualified"] = First_Time_UnQualified+ Rework_UnQualified;//Number of unacceptable submissions
                    item["Rework_UnQualified"] = Rework_UnQualified;
                    item["schgzs"] = First_Time_Qualified;// Total number of first-time passes
                    item["scbhhzs"] = First_Time_UnQualified;// Total number of initial failures
                    item["fxhgs"] = Rework_Qualified;// Number of repaired items that passed inspection
                    try
                    {
                        //item["fxrft"] = (Convert.ToDecimal(Rework_Qualified + First_Time_UnQualified) - Convert.ToDecimal(Rework_UnQualified)) / Convert.ToDecimal(Rework_Qualified + Rework_UnQualified);
                        item["fxrft"] = (Convert.ToDecimal(Rework_Qualified)) / Convert.ToDecimal(Rework_Qualified + First_Time_UnQualified);
                    }
                    catch (Exception)
                    {

                        item["fxrft"] = 0;
                    }
                    try
                    {
                        item["totalpass"] = Convert.ToDecimal((First_Time_Qualified + Rework_Qualified)) / Convert.ToDecimal((First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified));
                    }
                    catch (Exception)
                    {

                        item["totalpass"] = 0;
                    }

                    item["fx"] = Rework_Qualified + First_Time_UnQualified;
                    try
                    {
                        item["rftpass"] = Convert.ToDecimal(First_Time_Qualified) / Convert.ToDecimal((First_Time_Qualified + Rework_Qualified + BGrade + First_Time_UnQualified + Rework_UnQualified));
                    }
                    catch (Exception)
                    {

                        item["rftpass"] = 0;
                    }
                }

                sql = $@"SELECT
	                    m.workshop_section_no,
	                    m.develop_season,
	                    m.shoe_no,
	                    m.prod_no,
	                    m.production_line_code,
                        b.department_name,
	                    m.eq_info_no,
	                    m.mold_no,
	                    m.department,
	                    m.task_state,
	                    m.mer_po,
	                    m.se_id,
	                    m.stage,
	                    m.workorderno,
                        m.aql_task_no,
                        m.aql_rework_quantity,
                        d.name_t
                    FROM
	                    tqc_task_m m
                        LEFT JOIN BDM_RD_STYLE d on m.SHOE_NO=d.SHOE_NO
                        LEFT JOIN base005m b on m.production_line_code=b.department_code
                        where task_no=@task_no";
                DataTable dt2 = DB.GetDataTable(sql, paramTestDic);

                sql = $@"SELECT
	                        c.id,
	                        MAX(c.inspection_type) as inspection_type,
	                        MAX(c.inspection_code) as inspection_code,
	                        MAX(c.inspection_name) as inspection_name,
	                        MAX(c.qc_type) as qc_type,
	                        MAX(c.judgment_criteria) as judgment_criteria,
	                        MAX(c.standard_value) as standard_value,
	                        MAX(c.shortcut_key) as shortcut_key,
	                        0 num,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "to_char( f.file_guid )", "c.id")} as imglist
                        FROM
	                        tqc_task_item_c c
							LEFT JOIN tqc_task_detail_t t on c.id=t.union_id
	                        LEFT JOIN tqc_task_detail_t_f f on t.id=f.union_id
                            where c.task_no=@task_no and use_type='1'
	                        GROUP BY c.id";
                DataTable dt3 = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt3.Rows)
                {
                    decimal num = DB.GetDecimal($@"select count(1) from tqc_task_detail_t where union_id='{item["id"]}' group by task_no");
                    item["num"] = num;
                }

                sql = $@"SELECT
	                        c.id,
	                        MAX(c.inspection_type) as inspection_type,
	                        MAX(c.inspection_code) as inspection_code,
	                        MAX(c.inspection_name) as inspection_name,
	                        MAX(c.qc_type) as qc_type,
	                        MAX(c.judgment_criteria) as judgment_criteria,
	                        MAX(c.standard_value) as standard_value,
	                        MAX(c.shortcut_key) as shortcut_key,
	                        0 num,
	                        {CommonBASE.GetGroupConcatByOracleVersion(DB, "to_char( f.file_guid )", "c.id")} as imglist
                        FROM
	                        tqc_task_item_c c
							LEFT JOIN tqc_task_detail_t t on c.id=t.union_id
	                        LEFT JOIN tqc_task_detail_t_f f on t.id=f.union_id
                            where c.task_no=@task_no and use_type='2'
	                        GROUP BY c.id ";
                DataTable dtut = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dtut.Rows)
                {
                    decimal num = DB.GetDecimal($@"select count(1) from tqc_task_detail_t where union_id='{item["id"]}' group by task_no");
                    item["num"] = num;
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = $@"SELECT
                        d.id,
                        '0' as source
                        FROM
                        qcm_dqa_mag_d d
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type'
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE d.shoes_code='{dt2.Rows[0]["shoe_no"].ToString()}' and d.ISDELETE='0'
                        UNION	
                        SELECT
                        d.id,
                        '1' as source
                        FROM
                        qcm_mqa_mag_d d
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type'
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE d.shoes_code='{dt2.Rows[0]["shoe_no"].ToString()}' and d.ISDELETE='0'";
                DataTable dmids = DB.GetDataTable(sql);
                string check_res = "FAIL";
                foreach (DataRow item in dmids.Rows)
                {
                    sql = $@"select check_res from tqc_task_check_t_f where task_no='{task_no}' and union_id='{item["id"]}' and source_type='{item["source"]}' order by id desc";
                    DataTable resdt = DB.GetDataTable(sql);
                    if (resdt.Rows.Count > 0)
                    {
                        if (resdt.Rows[0]["check_res"].ToString() == "0")
                        {
                            check_res = "PASS";
                        }
                        else if (resdt.Rows[0]["check_res"].ToString() == "1")
                        {
                            check_res = "FAIL";
                            dic.Add("Data1", dt1);//当前数据
                            dic.Add("Data2", dt2);//基本信息
                            dic.Add("Data3", dt3);//键帽信息
                            dic.Add("check_res", check_res);//检验结果
                            dic.Add("dtut", dtut);//不常见项目

                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                            ret.IsSuccess = true;
                            return ret;
                        }
                    }
                }


                //The Rework task displays the box data
                string task_state = DB.GetString($@"select task_state  from tqc_task_m where task_no = '{task_no}'");
                if (task_state.Equals("3") || task_state.Equals("4") || task_state.Equals("5"))
                {
                    string sql2 = $@"SELECT
  MAX(p.id) as id,
  MAX(p.task_no) as task_no,
  MAX(p.case_no) as case_no,
  MAX(p.cr_size) as cr_size,
  MAX(p.se_qty) as se_qty,--分批订单数量
  max(s.se_qty) AS po_qty --PO订单总数
FROM
  aql_cma_task_list_m_pb p
  INNER JOIN tqc_task_m a ON p.TASK_NO = a.TASK_NO
  INNER JOIN BDM_SE_ORDER_MASTER m ON m.mer_po = a.mer_po
  INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
  AND m.SE_ID = s.SE_ID 
  AND p.cr_size = s.SIZE_NO 
WHERE
  p.TASK_NO = '{task_no}'
  GROUP BY s.SIZE_NO
  ORDER BY MAX(s.SIZE_SEQ)";
                    DataTable dt4 = DB.GetDataTable(sql2);
                    dic.Add("Data4", dt4);//Point box data
                }



                dic.Add("Data1", dt1);//当前数据
                dic.Add("Data2", dt2);//基本信息
                dic.Add("Data3", dt3);//键帽信息
                dic.Add("check_res", check_res);//检验结果
                dic.Add("dtut", dtut);//不常见项目

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
        /// tqc创建页面键帽设置上传照片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Task_Edit_Upload(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string union_id = jarr.ContainsKey("union_id") ? jarr["union_id"].ToString() : "";//条件 关联记录表id
        //        string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";//条件 文件关联id
        //        string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
        //        string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
        //        string time = DateTime.Now.ToString("HH:mm:ss");//时间
        //        string sql = string.Empty;

        //        sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
        //                values('{union_id}','{file_guid}','{user}','{date}','{time}')";
        //        DB.ExecuteNonQuery(sql);

        //        ret.IsSuccess = true;
        //        DB.Commit();

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}

        /// <summary>
        /// tqc创建页面键帽设置查询图片
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
                string sql = $@"SELECT
	                            file_name,
	                            file_url,
	                            'tqc_task_detail_t_f' AS tablename,
	                            f.id,
	                            GUID 
                            FROM
	                            BDM_UPLOAD_FILE_ITEM t 
	                            LEFT JOIN tqc_task_detail_t_f f on f.file_guid= t.guid
                                where  t.guid in('{image_guid.Replace(",", "','")}')";
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
        /// tqc创建页面键帽设置查询b品图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimage_guidB(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 图片guid
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = $@"SELECT
	                            MAX(b.file_name) as file_name,
	                            MAX(b.file_url) as file_url,
	                            'tqc_task_detail_m_f' AS tablename,
	                            MAX(f.id) as id,
	                            MAX(b.guid) as guid
                            FROM
	                            tqc_task_commit_m t
	                            INNER JOIN tqc_task_detail_m_f f on t.COMMIT_INDEX=f.union_id
	                            LEFT JOIN BDM_UPLOAD_FILE_ITEM b on f.file_guid=b.guid
	                            where t.TASK_NO='{task_no}' and t.commit_type='4'
	                            GROUP BY t.COMMIT_INDEX ";
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
        /// tqc创建页面设置停线或者重新开线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Task_Edit_state(object OBJ)
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
                string task_state = jarr.ContainsKey("task_state") ? jarr["task_state"].ToString() : "";//条件 停线状态
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";//条件 停线原因
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update tqc_task_m set task_state='{task_state}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"insert into tqc_task_stopline_r (task_no,reason,stop_type,createby,createdate,createtime) 
                         values('{task_no}','{reason}','{task_state}','{user}','{date}','{time}')";
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
        /// tqc创建页面提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Task_Edit(object OBJ)
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
                string commit_type = jarr.ContainsKey("commit_type") ? jarr["commit_type"].ToString() : "";//条件 提交类型
                DataTable TestItem = jarr.ContainsKey("TestItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TestItem"].ToString()) : null;//条件 键帽信息
                DataTable Uncommon_TestItem = jarr.ContainsKey("Uncommon_TestItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["Uncommon_TestItem"].ToString()) : null;//条件 不常见项目
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                string commit_index = DB.GetString($@"select max(commit_index) from tqc_task_commit_m where task_no='{task_no}'");
                if (string.IsNullOrWhiteSpace(commit_index))
                {
                    commit_index = "0";
                }
                sql = $@"insert into tqc_task_commit_m (task_no,commit_index,commit_type,createby,createdate,createtime) 
                                    values('{task_no}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                if (TestItem.Rows.Count > 0)
                {
                    foreach (DataRow item in TestItem.Rows)
                    {
                        if (item["ifclick"].ToString() == "true")
                        {
                            sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{item["id"]}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                            string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                            string[] imgs = item["imglist"].ToString().Split(',');
                            //DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                            DataTable imgdt = DB.GetDataTable($@"select file_guid from tqc_task_detail_t_f where union_id in (select id from tqc_task_detail_t where task_no='{task_no}')");
                            string[] imgdts = new string[imgdt.Rows.Count];
                            for (int i = 0; i < imgdt.Rows.Count; i++)
                            {
                                imgdts[i] = imgdt.Rows[i]["file_guid"].ToString();
                            }
                            for (int i = 0; i < imgs.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                {
                                    if (!imgdts.Contains(imgs[i]))
                                    {
                                        sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
                                        DB.ExecuteNonQuery(sql);
                                    }
                                }
                            }
                        }
                    }
                }

                //不常见项目
                if (Uncommon_TestItem.Rows.Count > 0)
                {
                    string workshop_section_no = DB.GetString($@"select workshop_section_no from tqc_task_m where task_no='{task_no}'");
                    foreach (DataRow item in Uncommon_TestItem.Rows)
                    {
                        if (item["id"] == null || string.IsNullOrWhiteSpace(item["id"].ToString()))
                        {
                            if (Convert.ToDecimal(item["num"]) > 0)
                            {
                                string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");//工段编号获取工段id
                                string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");//工段id获取检测项目类型
                                sql = $@"insert into tqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,
                                    standard_value,shortcut_key,use_type,createby,createdate,createtime) 
                                    values('{task_no}','{inspection_type}','{item["inspection_code"]}','{item["inspection_name"]}','{item["qc_type"]}',
                                    '{item["judgment_criteria"]}','{item["standard_value"]}','{item["shortcut_key"]}','2','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                                string cid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_item_c");
                                if (item["ifclick"].ToString() == "true")
                                {
                                    sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{cid}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                    string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                                    string[] imgs = item["imglist"].ToString().Split(',');
                                    //DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                                    DataTable imgdt = DB.GetDataTable($@"select file_guid from tqc_task_detail_t_f where union_id in (select id from tqc_task_detail_t where task_no='{task_no}')");
                                    string[] imgdts = new string[imgdt.Rows.Count];
                                    for (int i = 0; i < imgdt.Rows.Count; i++)
                                    {
                                        imgdts[i] = imgdt.Rows[i]["file_guid"].ToString();
                                    }
                                    for (int i = 0; i < imgs.Length; i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                        {
                                            if (!imgdts.Contains(imgs[i]))
                                            {
                                                sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
                                                DB.ExecuteNonQuery(sql);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item["ifclick"].ToString() == "true")
                            {
                                sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{item["id"]}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                                string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                                string[] imgs = item["imglist"].ToString().Split(',');
                                //DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                                DataTable imgdt = DB.GetDataTable($@"select file_guid from tqc_task_detail_t_f where union_id in (select id from tqc_task_detail_t where task_no='{task_no}')");
                                string[] imgdts = new string[imgdt.Rows.Count];
                                for (int i = 0; i < imgdt.Rows.Count; i++)
                                {
                                    imgdts[i] = imgdt.Rows[i]["file_guid"].ToString();
                                }
                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                    {
                                        if (!imgdts.Contains(imgs[i]))
                                        {
                                            sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
                                            DB.ExecuteNonQuery(sql);
                                        }
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
        /// tqc创建页面b品提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditTestItemB(object OBJ)
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
                string commit_type = jarr.ContainsKey("commit_type") ? jarr["commit_type"].ToString() : "";//条件 提交类型
                DataTable TestItem = jarr.ContainsKey("TestItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TestItem"].ToString()) : null;//条件 键帽信息
                DataTable Uncommon_TestItem = jarr.ContainsKey("Uncommon_TestItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["Uncommon_TestItem"].ToString()) : null;//条件 不常见项目
                string file_guidB = jarr.ContainsKey("file_guidB") ? jarr["file_guidB"].ToString() : "";//条件 b品guid
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                string commit_index = DB.GetString($@"select max(commit_index) from tqc_task_commit_m where task_no='{task_no}'");
                if (string.IsNullOrWhiteSpace(commit_index))
                {
                    commit_index = "0";
                }
                if (!string.IsNullOrEmpty(file_guidB))
                {
                    sql = $@"insert into tqc_task_detail_m_f (union_id,file_guid,createby,createdate,createtime) 
                         values('{Convert.ToDecimal(commit_index) + 1}','{file_guidB}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }
                sql = $@"insert into tqc_task_commit_m (task_no,commit_index,commit_type,createby,createdate,createtime) 
                                    values('{task_no}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                if (TestItem.Rows.Count > 0)
                {
                    foreach (DataRow item in TestItem.Rows)
                    {

                        if (item["ifclick"].ToString() == "true")
                        {
                            sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{item["id"]}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                            string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                            string[] imgs = item["imglist"].ToString().Split(',');
                            DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                            for (int i = 0; i < imgs.Length; i++)
                            {
                                if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                {
                                    sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                }
                            }
                        }
                    }
                }

                //Uncommon items
                if (Uncommon_TestItem.Rows.Count > 0)
                {
                    string workshop_section_no = DB.GetString($@"select workshop_section_no from tqc_task_m where task_no='{task_no}'");
                    foreach (DataRow item in Uncommon_TestItem.Rows)
                    {
                        if (item["id"] == null || string.IsNullOrWhiteSpace(item["id"].ToString()))
                        {
                            if (Convert.ToDecimal(item["num"]) > 0)
                            {
                                string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");//工段编号获取工段id
                                string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");//工段id获取检测项目类型
                                sql = $@"insert into tqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,
                                    standard_value,shortcut_key,use_type,createby,createdate,createtime) 
                                    values('{task_no}','{inspection_type}','{item["inspection_code"]}','{item["inspection_name"]}','{item["qc_type"]}',
                                    '{item["judgment_criteria"]}','{item["standard_value"]}','{item["shortcut_key"]}','2','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                                string cid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_item_c");
                                if (item["ifclick"].ToString() == "true")
                                {
                                    sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{cid}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                                    DB.ExecuteNonQuery(sql);
                                    string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                                    string[] imgs = item["imglist"].ToString().Split(',');
                                    DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                                    for (int i = 0; i < imgs.Length; i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                        {
                                            sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
                                            DB.ExecuteNonQuery(sql);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (item["ifclick"].ToString() == "true")
                            {
                                sql = $@"insert into tqc_task_detail_t (task_no,union_id,commit_index,commit_type,qc_type,judgment_criteria,standard_value,
                                    shortcut_key,createby,createdate,createtime) 
                                    values('{task_no}','{item["id"]}','{Convert.ToDecimal(commit_index) + 1}','{commit_type}','{item["qc_type"]}','{item["judgment_criteria"]}',
                                    '{item["standard_value"]}','{item["shortcut_key"]}','{user}','{date}','{time}')";
                                DB.ExecuteNonQuery(sql);
                                string tid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_detail_t");
                                string[] imgs = item["imglist"].ToString().Split(',');
                                DB.ExecuteNonQuery($@"delete from tqc_task_detail_t_f where union_id='{tid}'");
                                for (int i = 0; i < imgs.Length; i++)
                                {
                                    if (!string.IsNullOrWhiteSpace(imgs[i].ToString()))
                                    {
                                        sql = $@"insert into tqc_task_detail_t_f (union_id,file_guid,createby,createdate,createtime) 
                                        values('{tid}','{imgs[i]}','{user}','{date}','{time}')";
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
        /// tqc创建页面撤回提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Task_Edit_recall(object OBJ)
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

                string commit_index = DB.GetString($@"select max(commit_index) from tqc_task_commit_m where task_no='{task_no}'");
                //Newly Added by Ashok to recall BGrade Data on 20260112
                string commit_type = DB.GetString($@"select commit_type from tqc_task_commit_m where task_no='{task_no}' and commit_index='{commit_index}'");
                if(commit_type=="4")
                {
                    sql = $@"DELETE FROM tqc_bgrade_reason
 WHERE ROWID IN (SELECT ROWID
                   FROM tqc_bgrade_reason
                  WHERE task_no = '{task_no}'
                  ORDER BY created_at DESC
                  FETCH FIRST 1 ROW ONLY)";
                    DB.ExecuteNonQuery(sql);
                }
                sql = $@"delete from tqc_task_commit_m where task_no='{task_no}' and commit_index='{commit_index}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_detail_t where task_no='{task_no}' and commit_index='{commit_index}'";
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
        /// tqc创建页面根据鞋型查dqa&mqa
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAMQA(object OBJ)
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

                string sql = string.Empty;

                sql = $@"SELECT
                        d.id,
                        d.choice_name,
                        d.inspection_code,
                        '' as inspection_name,
                        m.enum_value,
                        m.enum_value2,
                        d.standard_value,
                        d.unit,
                        d.remark,
                        d.other_measures,
                        d.shoes_code,
                        'DQA' as source,
                        t.file_url
                        FROM
                        qcm_dqa_mag_d d
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type'
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE d.shoes_code=@shoe_no and d.ISDELETE='0'
                        UNION	
                        SELECT
                        d.id,
                        d.choice_name,
                        d.inspection_code,
                        '' as inspection_name,
                        m.enum_value,
                        m.enum_value2,
                        d.standard_value,
                        d.unit,
                        d.remark,
                        d.other_measures,
                        d.shoes_code,
                        'MQA' as source,
                        t.file_url
                        FROM
                        qcm_mqa_mag_d d
                        LEFT JOIN SYS001M m on d.inspection_type=m.enum_code and enum_type='enum_inspection_type'
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on d.image_guid=t.guid
                        WHERE d.shoes_code=@shoe_no and d.ISDELETE='0'";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["enum_value"].ToString()))
                    {
                        item["inspection_name"] = DB.GetString($@"select inspection_name from {item["enum_value2"]} where inspection_code='{item["inspection_code"]}'");
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetClaimData(object OBJ)
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
                string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//查询条件 鞋型 
                string sql = string.Empty;

                sql = $@"select 
                        to_char(m.complaint_date,'yyyy-MM-dd') as complaint_date,
                        m.COMPLAINT_NO,  
                         t.prod_no as prod_no,
                        m.PO_ORDER as PO_ORDER, 
                        m.STATUS as STATUS,
                        '' as imglist
                        from 
                        QCM_CUSTOMER_COMPLAINT_M m
                        LEFT JOIN BDM_SE_ORDER_MASTER b on m.PO_ORDER=b.mer_po
                        LEFT JOIN BDM_SE_ORDER_ITEM t on b.se_id=t.se_id 
                       where 1=1 and t.prod_no='{Art}'";
                //Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                //paramTestDic.Add("shoe_no", $@"{shoe_no}");
                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    string COMPLAINT_NO = item["COMPLAINT_NO"].ToString();

                    DataTable dtimgs = DB.GetDataTable($@"select file_guid from QCM_CUSTOMER_COMPLAINT_M_F where file_type='0' and COMPLAINT_NO='{COMPLAINT_NO}' ");
                    for (int i = 0; i < dtimgs.Rows.Count; i++)
                    {
                        item["imglist"] += dtimgs.Rows[i]["file_guid"].ToString() + ',';
                    }
                    item["imglist"] = item["imglist"].ToString().TrimEnd(',');
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReturnDatalist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                string where = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string ARTICLE = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() :"1";

                #region 条件
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(ARTICLE))
                {
                    where += "ARTICLE = @ARTICLE";
                    paramTestDic.Add("ARTICLE", $@"{ARTICLE}");
                }
                #endregion
                sql = $@"

SELECT 
    ID,
    RETURN_MONTH,
    REGION,
    FACTORY_NO,
    FACTORY_NAME,
    SALESORGAN_NO,
    SALESORGAN_NAME,
    ARTICLE,
    SHOES_NAME,
    TO_CHAR (PRODUCTION_DATE,'MM-yyyy') PRODUCTION_DATE,
    MASTERCODE,
    MASTERNAME,
    SECONDCODE,
    SECONDNAME,
    FOB,
    QTY,
    MONEY,
    PRICE
FROM QCM_CUSTOMER_RETURN_M WHERE {where} order by id desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                // DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int total = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "Query failed！" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// dqa&mqa核对页面编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditDQAMQA(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//条件 dqa&mqa的id
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";//条件 来源类型
                DataTable tqc_task_check_t_f = jarr.ContainsKey("tqc_task_check_t_f") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["tqc_task_check_t_f"].ToString()) : null;//条件 TQC任务——核对——记录
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                string check_res_res = "Pass";
                foreach (DataRow item in tqc_task_check_t_f.Rows)
                {
                    if (source_type == "DQA")
                    {
                        source_type = "0";
                    }
                    else if (source_type == "MQA")
                    {
                        source_type = "1";
                    }
                    sql = $@"insert into tqc_task_check_t_f (task_no,union_id,source_type,qty,q_qty,bad_desc,check_res,createby,createdate,createtime) 
                            values('{task_no}','{id}','{source_type}','{item["检验总数"]}','{item["合格数量"]}','{item["不良问题描述"]}',
                            '{item["检验结果代号"]}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);

                    if (item["检验结果代号"].ToString() == "1")
                    {
                        check_res_res = "Fail";
                    }

                    string fid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "tqc_task_check_t_f");
                    string[] imgguids = item["图片集合"].ToString().Split(',');
                    for (int i = 0; i < imgguids.Length; i++)
                    {
                        sql = $@"insert into tqc_task_check_t_ff (union_id,file_guid,createby,createdate,createtime) 
                                values('{fid}','{imgguids[i]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }

                }

                if (tqc_task_check_t_f.Rows.Count > 0)
                {
                    #region 更新首次检验
                    if (!string.IsNullOrWhiteSpace(source_type))
                    {
                        string union_id = id;
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
        /// dqa&mqa核对查看历史
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAMQA_history(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 dqa&mqa的id
                string source_type = jarr.ContainsKey("source_type") ? jarr["source_type"].ToString() : "";//查询条件 来源类型
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string keycode = jarr.ContainsKey("keycode") ? jarr["keycode"].ToString() : "";//查询条件
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
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

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and (qty like @keycode or q_qty like @keycode or bad_desc like @keycode or check_res like @keycode)";
                }
                string sql = string.Empty;

                sql = $@"SELECT * FROM(
                            SELECT
	                            MAX( t.qty ) AS qty,
	                            MAX( t.q_qty ) AS q_qty,
	                            MAX( t.bad_desc ) AS bad_desc,
                            CASE
		                            MAX( t.check_res ) 
		                            WHEN '0' THEN
		                            'PASS' 
		                            WHEN '1' THEN
		                            'FAIL' 
	                            END check_res,
	                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "to_char( f.file_guid )", "t.id")} AS imglist 
                            FROM
	                            tqc_task_check_t_f t
	                            LEFT JOIN tqc_task_check_t_ff f ON t.id = f.union_id 
                        WHERE t.union_id=@union_id and t.task_no=@task_no and t.source_type=@source_type 
                        GROUP BY t.id
                        ORDER BY t.id desc)T where 1=1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("union_id", $@"{id}");
                paramTestDic.Add("task_no", $@"{task_no}");
                paramTestDic.Add("source_type", $@"{source_type}");
                paramTestDic.Add("keycode", $@"%{keycode}%");
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
        /// dqa&mqa核对查看历史查询图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDQAMQA_history_img(object OBJ)
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
                string sql = $@"SELECT
	                            file_name,
	                            file_url,
	                            'tqc_task_check_t_ff' AS tablename,
	                            f.id,
	                            GUID 
                            FROM
	                            BDM_UPLOAD_FILE_ITEM t 
	                            INNER JOIN tqc_task_check_t_ff f on t.guid=f.file_guid 
                                where f.file_guid in('{image_guid.Replace(",", "','")}')";
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = $@"update tqc_task_m set task_state='2' where task_no='{task_no}'";
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
        /// tqc主页删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete_TQC_Task_Main(object OBJ)
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
                sql = $@"delete from tqc_task_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_item_c where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_commit_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_stopline_r where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_detail_t where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                string tid = DB.GetString($@"select id from tqc_task_detail_t where task_no='{task_no}'");
                sql = $@"delete from tqc_task_detail_t_f where union_id='{tid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from tqc_task_check_t_f where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                DataTable dt = DB.GetDataTable($@"select commit_index from tqc_task_detail_t where task_no='{task_no}'");
                foreach (DataRow item in dt.Rows)
                {
                    sql = $@"delete from tqc_task_detail_m_f where union_id='{item["commit_index"]}'";
                    DB.ExecuteNonQuery(sql);
                }

                string fid = DB.GetString($@"select id from tqc_task_check_t_f where task_no='{task_no}'");
                sql = $@"delete from tqc_task_check_t_ff where union_id='{fid}'";
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
        /// tqc编辑页面查看停线记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Edit_Stopline_Record(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"SELECT
	                            stop_type,
	                            CONCAT( CONCAT( createdate, ' ' ), createtime ) AS createdatetime,
	                            reason
                            FROM
	                            tqc_task_stopline_r where task_no=@task_no order by id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
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
        /// tqc编辑页面查询提交快捷键
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Task_Edit_shortcut_key(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号

                string sql = string.Empty;
                string inspection_type = DB.GetString($@"select inspection_type from tqc_task_item_c where task_no='{task_no}'");
                sql = $@"select tqc_key,shortcut_key from tqc_shortcut_key_m where inspection_type=@inspection_type";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("inspection_type", $@"{inspection_type}");
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
        /// tqc编辑页面不常见项目查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTQC_Uncommon_TestItem(object OBJ)
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
                string keyvalue = jarr.ContainsKey("keyvalue") ? jarr["keyvalue"].ToString() : "";//查询条件
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                DataTable NotInDt = jarr.ContainsKey("NotInDt") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["NotInDt"].ToString()) : null;//查询条件 不常见选项过滤
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 工段编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;
                string where = string.Empty;
                string whereIC = string.Empty;
                List<string> inspection_codes = new List<string>();
                if (NotInDt != null && NotInDt.Rows.Count > 0)
                {
                    foreach (DataRow item in NotInDt.Rows)
                    {
                        inspection_codes.Add(item["inspection_code"].ToString());
                    }
                }
                if (inspection_codes.Count > 0)
                {
                    whereIC = $@" and inspection_code not in ({string.Join(",", inspection_codes.Select(x => $@"'{x}'"))})";
                }
                string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");
                string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                string tabname = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{inspection_type}' and enum_code in ('0','1','2','3','4','5','6')");
                if (!string.IsNullOrWhiteSpace(keyvalue))
                {
                    where = $@" and inspection_name like '%{keyvalue}%'";
                }
                string sqlin = $@"SELECT 
                            id
                            FROM
                            {tabname}
                            WHERE qc_type='1' and ROWNUM<=20";
                sql = $@"SELECT 
                            inspection_code,
                            inspection_name,
                            qc_type,
                            judgment_criteria,
                            standard_value,
                            shortcut_key
                            FROM
                            {tabname}
                            WHERE qc_type='1' and id not in ({sqlin}) {whereIC} {where}
                            ORDER BY id asc";
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
        /// tqc编辑页查询测试结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEx_LookResult(object OBJ)
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
                string tqc_task_no = jarr.ContainsKey("tqc_task_no") ? jarr["tqc_task_no"].ToString() : "";//查询条件 tqc任务编号

                string sql = string.Empty;

                sql = $@"select prod_no from tqc_task_m where task_no='{tqc_task_no}'";
                string ART = DB.GetString(sql);

                sql = $@"SELECT * FROM(
                        SELECT
	                        task_no
                        FROM
	                        qcm_ex_task_list_m 
                        WHERE
	                        test_type = '4' 
	                        AND art_no = '{ART}' 
	                        ORDER BY
	                        id DESC
	                        )
	                        WHERE  ROWNUM = 1  ";

                string task_no = DB.GetString(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);

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
        /// tqc编辑页查询安全合规文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCompliance_File(object OBJ)
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
                string tqc_task_no = jarr.ContainsKey("tqc_task_no") ? jarr["tqc_task_no"].ToString() : "";//查询条件 tqc任务编号

                string sql = string.Empty;

                sql = $@"select prod_no from tqc_task_m where task_no='{tqc_task_no}'";
                string ART = DB.GetString(sql);

                sql = $@"SELECT
                        MAX(b.FILE_URL) as file_url,
                        MAX(b.FILE_NAME) as file_name,
                        'qcm_safety_compliance_file_d' as tablename,
                        MAX(q.id) as id,
                        MAX(b.guid) as guid
                        FROM
	                        qcm_safety_compliance_file_d q
	                        LEFT JOIN BDM_UPLOAD_FILE_ITEM b on q.FILE_GUID=b.GUID
                        WHERE
	                        PROD_NO = '{ART}'
	                        GROUP BY FILE_TYPE";

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
        /// tqc编辑页查询DQA文件
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
                string tqc_task_no = jarr.ContainsKey("tqc_task_no") ? jarr["tqc_task_no"].ToString() : "";//查询条件 tqc任务编号

                string sql = string.Empty;

                sql = $@"select shoe_no from tqc_task_m where task_no='{tqc_task_no}'";
                string shoe_no = DB.GetString(sql);

                sql = $@"select f.file_url,f.file_name,'qcm_shoes_qa_record_file_m' as tablename,m.id,f.guid from BDM_UPLOAD_FILE_ITEM f INNER JOIN qcm_shoes_qa_record_file_m m on f.guid=m.file_id where m.shoes_code=@SHOE_NO";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("SHOE_NO", $@"{shoe_no}");
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Quality_Bonus(object OBJ)
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

                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//任务状态
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//查询条件 日期开始
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";//查询条件 日期结束
                string Group = jarr.ContainsKey("Group") ? jarr["Group"].ToString() : "";//查询条件 鞋型
                string s_date = jarr.ContainsKey("s_date") ? jarr["s_date"].ToString() : "";//查询条件 art
                string e_date = jarr.ContainsKey("e_date") ? jarr["e_date"].ToString() : "";//查询条件 mer_po
               // string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : ""; 
                string where = string.Empty;
                //List<string> task_statelist = task_state.Split(',').ToList();

                if (!string.IsNullOrWhiteSpace(s_date) && !string.IsNullOrWhiteSpace(e_date))
                {
                    where += $@" and t.createdate>=@datestart and t.createdate<=@dateend ";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(s_date))
                    {
                        where += $@" and t.createdate>='{s_date}'";
                    }
                    if (!string.IsNullOrWhiteSpace(e_date))
                    {
                        where += $@" and t.createdate<='{e_date}'";
                    }
                }


                string sql = $@"select b.production_line_code,
       count(a.commit_type) as toal_inspected,
       sum(case when a.commit_type = 0 THEN 1 else 0 end) as first_pass,
       sum(case when a.commit_type = 4 THEN 1 else 0 end) as b_grade,
       '' as rft_pass_percent,
       '' as b_grade_percentage   
       from tqc_task_commit_m a  left join 
       tqc_task_m b 
       on a.task_no = b.task_no
        where b.production_line_code like '%L%' and b.createdate between to_char(to_date('{s_date}','yyyy/MM/dd'),'yyyy-MM-dd') and to_char(to_date('{e_date}','yyyy/MM/dd'),'yyyy-MM-dd')
        group by b.production_line_code order by b.production_line_code"; 

                DataTable dt = DB.GetDataTable(sql); 
                foreach (DataRow item in dt.Rows)
                { 
                    try
                    {
                        item["b_grade_percentage"] = Math.Round(Convert.ToDecimal(item["b_grade"]) / (Convert.ToDecimal(item["toal_inspected"]) / 2),2);
                    }
                    catch (Exception)
                    {

                        item["b_grade_percent"] = 0;
                    }
                    try
                    {
                        item["rft_pass_percent"] = Math.Round(Convert.ToDecimal(item["first_pass"]) / Convert.ToDecimal(item["toal_inspected"]),2); 
                    }
                    catch (Exception)
                    {
                        item["rft_pass_percent"] = 0;
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Hourly_Top3Issues(object OBJ) 
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

                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";//任务状态
                string startdate = jarr.ContainsKey("startdate") ? jarr["startdate"].ToString() : "";//查询条件 日期开始
                string enddate = jarr.ContainsKey("enddate") ? jarr["enddate"].ToString() : "";//查询条件 日期结束 
                string where = string.Empty;
                string where1 = string.Empty;
                string where2 = string.Empty;
                //List<string> task_statelist = task_state.Split(',').ToList();

                if (!string.IsNullOrWhiteSpace(startdate) && !string.IsNullOrWhiteSpace(enddate))
                {
                    where += $@"  and to_char(to_date(a.createdate,'yyyy-MM-dd'),'yyyy/MM/dd') between '{startdate}' and '{enddate}'";
                }
                if(!string.IsNullOrWhiteSpace(prod_line))
                {
                    where2 += $@" and a.production_line_code = '{prod_line}'";
                }
                if (!string.IsNullOrWhiteSpace(prod_line))
                {
                    where1 += $@" and c.production_line_code = '{prod_line}'";
                }

                #region TQCHourlyDataQuery
                string sql = $@" with tmp as
  (SELECT a.createdate,
          production_line_code,
          timeslot,
          MAX(CASE
                WHEN ranking = 1 THEN
                 inspection_name
              END) AS inspection_name_1,
          MAX(CASE
                WHEN ranking = 1 THEN
                 total
              END) AS total_1,
          round(MAX(CASE
                      WHEN ranking = 1 THEN
                       total
                    END) / sum(total) * 100,
                2) as percentage_1,
          MAX(CASE
                WHEN ranking = 2 THEN
                 inspection_name
              END) AS inspection_name_2,
          MAX(CASE
                WHEN ranking = 2 THEN
                 total
              END) AS total_2,
          round(MAX(CASE
                      WHEN ranking = 2 THEN
                       total
                    END) / sum(total) * 100,
                2) as percentage_2,
          MAX(CASE
                WHEN ranking = 3 THEN
                 inspection_name
              END) AS inspection_name_3,
          MAX(CASE
                WHEN ranking = 3 THEN
                 total
              END) AS total_3,
          round(MAX(CASE
                      WHEN ranking = 3 THEN
                       total
                    END) / sum(total) * 100,
                2) as percentage_3
     FROM (
                  select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '06:30-07:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1 {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0630' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'0730'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
                  select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '07:30-08:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0730' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'0830'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union

                  select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '08:30-09:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0830' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'0930'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
               select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '09:30-10:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0930' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1030'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
           select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '10:30-11:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1030' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1130'
            group by b.inspection_name, c.production_line_code, a.createdate
           union
           select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '11:30-12:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
             and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1130' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1230'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
           select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '12:30-13:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1230' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1330'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
                 select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '13:30-14:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
             and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1330' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1430'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
           select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '14:30-15:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
             and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1430' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1530'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
                 select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '15:30-16:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1530' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1630'
            group by b.inspection_name, c.production_line_code, a.createdate
           
           union
           select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '16:30-17:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1630' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1730'
            group by b.inspection_name, c.production_line_code, a.createdate
             union 
                 select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '17:30-18:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
             and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1730' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1830'
            group by b.inspection_name, c.production_line_code, a.createdate 
           union 
                 select a.createdate,
                  c.production_line_code,
                  b.inspection_name,
                  count(1) as total,
                   ROW_NUMBER() over(partition by c.production_line_code,a.createdate ORDER BY count(1) DESC) ranking,
                  '18:30-19:30' as timeslot
             from tqc_task_detail_t a
             inner join tqc_task_item_c b
               on a.union_id = b.id
             inner join tqc_task_m c
               on c.task_no = a.task_no
            where 1 = 1  {where1}{where}
              and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1830' and to_char(TO_DATE(a.createtime, 'hh24:mi:ss'), 'hh24mi')<'1930'
            group by b.inspection_name, c.production_line_code, a.createdate 
) a
    where 1=1 
    GROUP BY production_line_code, timeslot, a.timeslot, a.createdate),
 
 tmp2 as
  (select a.createdate,a.production_line_code,
          '06:30-07:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on a.task_no = b.task_no
    where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0630' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'0730'
    group by a.production_line_code,a.createdate 

union 

select a.createdate,a.production_line_code,
          '07:30-08:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on a.task_no = b.task_no
    where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0730' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'0830'
    group by a.production_line_code,a.createdate 

union
          select a.createdate,a.production_line_code,
          '08:30-09:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on a.task_no = b.task_no
    where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0830' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'0930'
    group by a.production_line_code,a.createdate 
   
   union
select a.createdate,a.production_line_code,
          '09:30-10:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
       and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '0930' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1030'
    group by a.production_line_code,a.createdate 
   
   union
select a.createdate,a.production_line_code,
          '10:30-11:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1030' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1130'
    group by a.production_line_code,a.createdate 
   
   union
   select a.createdate,a.production_line_code,
          '11:30-12:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no 
      where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1130' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1230'
   group by a.production_line_code,a.createdate 
   
   union
   select a.createdate,a.production_line_code,
          '12:30-13:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no 
     where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1230' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1330'
    group by a.production_line_code,a.createdate 
   union
select a.createdate,a.production_line_code,
          '13:30-14:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1330' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1430'
    group by a.production_line_code,a.createdate 
   
   union
   select a.createdate,a.production_line_code,
          '14:30-15:30' as timeslot,
          count(commit_type) as total_insp,
         count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
      where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1430' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1530'
    group by a.production_line_code,a.createdate 
   union
select a.createdate,a.production_line_code,
          '15:30-16:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
     and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1530' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1630'
    group by a.production_line_code,a.createdate 
   
   union
   select a.createdate,a.production_line_code,
          '16:30-17:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
     where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1630' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1730'
    group by a.production_line_code,a.createdate   
union
select a.createdate,a.production_line_code,
          '17:30-18:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1730' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1830'
    group by a.production_line_code,a.createdate 

union
select a.createdate,a.production_line_code,
          '18:30-19:30' as timeslot,
          count(commit_type) as total_insp,
          count(CASE
                  WHEN commit_type = '0' THEN
                   commit_type
                END) +  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END) AS total_pass,
          
          round((count(CASE
                        WHEN commit_type = '0' THEN
                         commit_type
                      END)+  count(CASE
                  WHEN commit_type = '2' THEN
                   commit_type
                END))/ count(commit_type) * 100,
                2) as passrate
     from tqc_task_commit_m b
     inner join tqc_task_m a
       on b.task_no = a.task_no
    where 1=1 {where} 
      {where2}
      and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')>=
          '1830' and to_char(TO_DATE(b.createtime, 'hh24:mi:ss'), 'hh24mi')<'1930'
    group by a.production_line_code,a.createdate 
   )
 
 select tmp2.createdate,
        tmp2.production_line_code,
        tmp2.timeslot,
        tmp2.total_insp,
        tmp2.total_pass,
        tmp2.total_insp-tmp2.total_pass as total_defects,
        tmp2.passrate,
        tmp.inspection_name_1,
        tmp.total_1,
        tmp.percentage_1,
        tmp.inspection_name_2,
        tmp.total_2,
        tmp.percentage_2,
        tmp.inspection_name_3,
        tmp.total_3,
        tmp.percentage_3
   from tmp2
   inner join tmp
     on tmp2.production_line_code = tmp.production_line_code
    and tmp2.timeslot = tmp.timeslot and tmp2.createdate=tmp.createdate order by tmp2.createdate desc,tmp2.timeslot
";
                #endregion

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Prod_line(object OBJ)
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
 

               
                string sql = $@"select m.department_code from base005m m";
               

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_TQC_Stopline_Record(object OBJ)
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

                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";//任务状态
                string startdate = jarr.ContainsKey("startdate") ? jarr["startdate"].ToString() : "";//查询条件 日期开始
                string enddate = jarr.ContainsKey("enddate") ? jarr["enddate"].ToString() : "";//查询条件 日期结束 
                string where = string.Empty;
                string where2 = string.Empty;
                //List<string> task_statelist = task_state.Split(',').ToList();

                if (!string.IsNullOrWhiteSpace(startdate) && !string.IsNullOrWhiteSpace(enddate))
                {
                    where += $@"  and to_char(to_date(a.createdate,'yyyy-MM-dd'),'yyyy/MM/dd') between '{startdate}' and '{enddate}'";
                }
                if (!string.IsNullOrWhiteSpace(prod_line))
                {
                    where2 += $@" and b.production_line_code like '%{prod_line}%'";
                }
                #region old
    //            string sql = $@"select b.createdate,b.production_line_code,
    //     b.Line_Stop_Time,
    //     b.Line_Start_Time,
    //     round(to_number(to_date(b.Line_Start_Time, 'yyyy-MM-dd HH24:MI:SS') -
    //                     to_date(b.Line_Stop_Time, 'yyyy-MM-dd HH24:MI:SS')) * 24 * 6,
    //           2) as Stop_Time,
    //     b.Stop_Reason
    //from (select a.production_line_code,
    //             Max(a.time_s) as Line_Stop_Time,
    //             Max(a.time_e) as Line_Start_Time,
    //             Max(a.reason) as Stop_Reason
    //        from (select b.production_line_code,
    //                     a.task_no,
    //                     case
    //                       when a.stop_type = '1' then
    //                        CONCAT(CONCAT(a.createdate, ' '), a.createtime)
    //                     end as time_s,
    //                     case
    //                       when a.stop_type = '0' then
    //                        CONCAT(CONCAT(a.createdate, ' '), a.createtime)
    //                     end as time_e,
    //                     a.reason,
    //                     ROW_NUMBER() OVER(PARTITION BY a.stop_type ORDER BY CONCAT(CONCAT(a.createdate, ' '), a.createtime)) AS ranking
    //                from tqc_task_stopline_r a
    //                join tqc_task_m b
    //                  on a.task_no = b.task_no
    //               where 1=1 {where} {where2}
    //               order by CONCAT(CONCAT(a.createdate, ' '), a.createtime) desc) a
    //       group by b.createdate,a.production_line_code,a.ranking)b";
                #endregion
                string sql = $@"select b.createdate,b.production_line_code,
         b.Line_Stop_Time,
         b.Line_Start_Time,
         round(to_number(to_date(b.Line_Start_Time, 'HH24:MI:SS') -
                         to_date(b.Line_Stop_Time, 'HH24:MI:SS')) * 24*60,
               0) as Stop_Time,
         b.Stop_Reason
    from (select a.createdate,a.production_line_code,
                 Max(a.time_s) as Line_Stop_Time,
                 Max(a.time_e) as Line_Start_Time,
                 Max(a.reason) as Stop_Reason
            from (select a.createdate,b.production_line_code,
                         a.task_no,
                         case
                           when a.stop_type = '1' then
                            a.createtime
                         end as time_s,
                         case
                           when a.stop_type = '0' then
                            a.createtime
                         end as time_e,
                         a.reason,
                         ROW_NUMBER() OVER(PARTITION BY a.stop_type,a.createdate,b.production_line_code ORDER BY CONCAT(CONCAT(a.createdate, ' '), a.createtime)) AS ranking
                    from tqc_task_stopline_r a
                    join tqc_task_m b
                      on a.task_no = b.task_no
                   where 1=1 {where} {where2}
                   order by CONCAT(CONCAT(a.createdate, ' '), a.createtime) desc) a
           group by a.createdate,a.production_line_code,a.ranking)b order by  b.createdate desc,b.production_line_code";


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


        //Added for Pivot88

        /// <summary>
        /// 同步TQC数据到pivot88
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static ResultObject TransferDataToPivot88(object OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject resultObject = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                DB.Open();


                string sql = string.Empty;
                //获取已报工完成的Po
                //Get the completed Po
                DataTable dataTable = GetFinishWorkPO();
                if (dataTable.Rows.Count > 0)
                {

                    string where = string.Empty;

                    foreach (DataRow row in dataTable.Rows)
                    {

                        DB.BeginTransaction();

                        string unique_key = string.Empty;
                        string po = row["po"].ToString();
                        string rout_no = row["rout_no"].ToString();
                        string inspection_table = string.Empty;
                        int total_inspection_result = 0;//Total inspection results

                        //additional allocation identifier
                        bool hasAssignment = false;
                        //The total number of reported defects is allocated by TQC and corresponds to defect_parts in the List table.
                        decimal defect_parts = 0;

                        if (rout_no.Equals("L")) inspection_table = "bdm_insp_fin_shoes_m";//加工
                        else if (rout_no.Equals("T")) inspection_table = "bdm_inspection_backing_parts_m";//贴底

                        GetDefectParts(po, rout_no, out hasAssignment, inspection_table, out defect_parts);


                        #region aeqs_main
                        try
                        {
                            string id = DB.GetString("SELECT T_AEQS_TO_P88_LIST_ID_SEQ.nextval FROM dual");
                            unique_key = "apache5_" + id.PadLeft(3, '0');
                        }
                        catch (Exception ex)
                        {
                            resultObject.IsSuccess = false;
                            resultObject.ErrMsg = ex.Message;
                            return resultObject;
                        }
                        AEQS_TO_P88_LIST aEQS_TO_P88_LIST = new AEQS_TO_P88_LIST();
                        aEQS_TO_P88_LIST.status = "Submitted";
                        aEQS_TO_P88_LIST.date_started = DB.GetDateTime($@"select createtime from (select concat(concat(createdate, ' '), createtime) as createtime from tqc_task_m where mer_po like '%{po}%' and workshop_section_no = '{rout_no}' and task_state in ('0','1','2') order by createtime asc)where rownum = 1");
                        aEQS_TO_P88_LIST.defective_parts = defect_parts;
                        aEQS_TO_P88_LIST.passfails_0_title = "inspected_carton_numbers";
                        aEQS_TO_P88_LIST.passfails_0_type = "list";
                        aEQS_TO_P88_LIST.passfails_0_subsection = "actual_inspection";
                        aEQS_TO_P88_LIST.assignment_items_assignment_report_type_id = 31m;
                        aEQS_TO_P88_LIST.passfails_0_listvalues_value = "N/A";
                        string sql2 = $@"insert into t_aeqs_to_p88_list
                                          (unique_key,
                                           status,
                                           date_started,
                                           defective_parts,
                                           assignment_items_assignment_report_type_id,
                                           passfails_0_title,
                                           passfails_0_type,
                                           passfails_0_subsection,
                                           passfails_0_listvalues_value)
                                        values
                                          ('{unique_key}',
                                           '{aEQS_TO_P88_LIST.status}', 
                                           to_date('{aEQS_TO_P88_LIST.date_started}', 'yyyy-mm-dd hh24:mi:ss'),
                                           '{aEQS_TO_P88_LIST.defective_parts}',
                                           '{aEQS_TO_P88_LIST.assignment_items_assignment_report_type_id}',
                                           '{aEQS_TO_P88_LIST.passfails_0_title}',
                                           '{aEQS_TO_P88_LIST.passfails_0_type}',
                                           '{aEQS_TO_P88_LIST.passfails_0_subsection}',
                                           '{aEQS_TO_P88_LIST.passfails_0_listvalues_value}')";
                        DB.ExecuteNonQuery(sql2);

                        #endregion 


                        #region aeqs_package
                        string package_id = string.Empty;
                        try
                        {
                            package_id = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                        }
                        catch (Exception ex2)
                        {
                            resultObject.IsSuccess = false;
                            resultObject.ErrMsg = ex2.Message;
                            return resultObject;
                        }
                        int order_qty = DB.GetInt32("select nvl(sum(se_qty),0)  as se_qty from bdm_se_order_size where se_id in (select se_id from bdm_se_order_master where mer_po = '" + po + "')");
                        AEQS_TO_P88_SECTIONS aeqs_package = new AEQS_TO_P88_SECTIONS();
                        aeqs_package.sections_type = "aqlDefects";
                        aeqs_package.sections_title = "packing_packaging_labelling";
                        aeqs_package.sections_result_id = 1m;
                        aeqs_package.sections_qty_inspected = order_qty;
                        aeqs_package.sections_sampled_inspected = order_qty;
                        aeqs_package.sections_defective_parts = 0;
                        aeqs_package.sections_inspection_level = "100%inspection";
                        aeqs_package.sections_inspection_method = "normal";
                        aeqs_package.sections_aql_minor = 4.0m;
                        aeqs_package.sections_aql_major = 2.5m;
                        aeqs_package.sections_aql_critical = 1.0m;
                        aeqs_package.sections_barcodes_value = "001";
                        string acSql = $@"select AC11, AC13, AC14, vals
                                          from (select AC11, AC13, AC14, vals
                                                  from BDM_AQL_M
                                                 where HORI_TYPE = '2'
                                                   and LEVEL_TYPE = '2'
                                                   and vals <= CAST('{order_qty}' AS int)
                                                 order by CAST(vals AS int) desc)
                                         where rownum = 1";
                        DataTable dataTable2 = DB.GetDataTable(acSql);
                        if (dataTable2.Rows.Count > 0)
                        {
                            aeqs_package.sections_max_critical_defects = Convert.ToInt32(dataTable2.Rows[0]["AC11"].ToString());

                            aeqs_package.sections_max_major_defects = Convert.ToInt32(dataTable2.Rows[0]["AC13"].ToString());
                            aeqs_package.sections_max_minor_defects = Convert.ToInt32(dataTable2.Rows[0]["AC14"].ToString());
                        }
                        aeqs_package.sections_max_major_a_defects = 0m;
                        aeqs_package.sections_max_major_b_defects = 0m;
                        aeqs_package.sections_defects_minor_level = 0m;
                        aeqs_package.sections_defects_major_level = 0m;
                        aeqs_package.sections_defects_critical_level = 0m;

                        #region package_sql
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
                                   SECTIONS_DEFECTS_MINOR_LEVEL)
                                values
                                  ('{package_id}',
                                   '{unique_key}',
                                   '{aeqs_package.sections_type}',
                                   '{aeqs_package.sections_title}',
                                   '{aeqs_package.sections_result_id}',
                                   '{aeqs_package.sections_qty_inspected}',
                                   '{aeqs_package.sections_sampled_inspected}',
                                   '{aeqs_package.sections_defective_parts}',
                                   '{aeqs_package.sections_inspection_level}',
                                   '{aeqs_package.sections_inspection_method}',
                                   '{aeqs_package.sections_aql_minor}',
                                   '{aeqs_package.sections_aql_major}',
                                   '{aeqs_package.sections_aql_critical}',
                                   '{aeqs_package.sections_barcodes_value}',
                                   '{aeqs_package.sections_qty_type}',
                                   '{aeqs_package.sections_max_minor_defects}',
                                   '{aeqs_package.sections_max_major_defects}',
                                   '{aeqs_package.sections_max_major_a_defects}',
                                   '{aeqs_package.sections_max_major_b_defects}',
                                   '{aeqs_package.sections_max_critical_defects}',
                                   '{aeqs_package.sections_defects_label}',
                                   '{aeqs_package.sections_defects_subsection}',
                                   '{aeqs_package.sections_defects_code}',
                                   '{aeqs_package.sections_defects_critical_level}',
                                   '{aeqs_package.sections_defects_major_level}',
                                   '{aeqs_package.sections_defects_minor_level}')";
                        #endregion

                        DB.ExecuteNonQuery(sql);
                        #endregion


                        #region aeqs_product
                        //product results
                        int product_result = 0;
                        int max_critical_defects = Convert.ToInt32(aeqs_package.sections_max_critical_defects);
                        int max_major_defects = Convert.ToInt32(aeqs_package.sections_max_major_defects);
                        int max_minor_defects = Convert.ToInt32(aeqs_package.sections_max_minor_defects);
                        //Record all assigned detail_ids
                        List<string> detailIdList = new List<string>();

                        #region 旧代码
                        //int minor_nums = DB.GetInt32("select count(*) from tqc_task_detail_t d inner join tqc_task_item_c c on d.union_id = c.id inner join  bdm_inspection_backing_parts_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code\r\n                                        where d.task_no in( select task_no from tqc_task_m where mer_po = '" + po + "' and workshop_section_no = '" + rout_no + "' ) and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '次要'");
                        //int major_nums = DB.GetInt32("select count(*) from tqc_task_detail_t d inner join tqc_task_item_c c on d.union_id = c.id inner join  bdm_inspection_backing_parts_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code\r\n                                        where d.task_no in( select task_no from tqc_task_m where mer_po = '" + po + "' and workshop_section_no = '" + rout_no + "' ) and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '主要'");
                        //int critical_nums = DB.GetInt32("select count(*) from tqc_task_detail_t d inner join tqc_task_item_c c on d.union_id = c.id inner join  bdm_inspection_backing_parts_m m on c.inspection_code = m.inspection_code left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code\r\n                                        where d.task_no in( select task_no from tqc_task_m where mer_po = '" + po + "' and workshop_section_no = '" + rout_no + "' ) and m.bad_item_code is not null and m.bad_item_name is not null and m.problem_level = '严重'");

                        //产品检验结果
                        //if (minor_nums > max_minor_defects || major_nums > max_major_defects || critical_nums > max_critical_defects)
                        //{
                        //    product_result = 2;//fail
                        //}
                        //else
                        //{
                        //    product_result = 1;//success
                        //}
                        #endregion

                        //Total inspection results: total number of problem points (problem points related to p88)/even number of inspected * 100% <= 15%. Pass "1=Pass", otherwise pass "2=Fail"
                        if (Convert.ToDouble(defect_parts) / Convert.ToDouble(order_qty) * 100 <= 15)
                        {
                            total_inspection_result = 1;//Pass
                        }
                        else
                        {
                            total_inspection_result = 2;//fail
                        }
                        product_result = total_inspection_result;
                        DB.Commit();


                        DB.BeginTransaction();

                        //Get the bad items of PO
                        //The query is divided into two parts. The first step is to check the defects of tasks that only belong to this po. The second step is to query the defects assigned to this po.
                        DataTable badItemTable = GetBadItemTable(po, rout_no, inspection_table);

                        //if (detailIdList.Count > 0)
                        //{
                        //    //分配时需要去掉已分配过的detail_id
                        //    where = $@" and d.id not in({ string.Join(',', detailIdList.Select(x => $"'{x}'"))})";
                        //}

                        //Does po have additional bad items allocated?
                        if (hasAssignment)
                        {
                            string quantitySql = $@"select nvl(sum(assigned_quantity),0) from t_aeqs_to_p88_po_record where po = '{po}'";
                            int assigned_quantity = DB.GetInt32(quantitySql);//分配数量

                            //Get assigned bad items
                            DataTable dtAssignment = GetAssignBadItem(po, rout_no, assigned_quantity);

                            if (dtAssignment.Rows.Count > 0)
                            {
                                //Compare the same items in the bad table and the bad allocation table
                                bool defect_exist = false;
                                foreach (DataRow dr in dtAssignment.Rows)
                                {
                                    foreach (DataRow dr2 in badItemTable.Rows)
                                    {
                                        string inspection_name = dr["INSPECTION_NAME"].ToString();
                                        if (inspection_name.Equals(dr2["INSPECTION_NAME"].ToString()))
                                        {
                                            dr2["badnum"] = (Convert.ToInt32(dr2["badnum"].ToString()) + Convert.ToInt32(dr["badnum"].ToString())).ToString();
                                            defect_exist = true;
                                            break;
                                        }
                                    }

                                    //If this bad item does not exist, add it to the existing table.。
                                    if (!defect_exist)
                                    {
                                        DataRow newRow = badItemTable.NewRow();
                                        newRow["detail_id"] = dr["detail_id"].ToString();
                                        newRow["inspection_name"] = dr["inspection_name"].ToString();
                                        newRow["bad_item_code"] = dr["bad_item_code"].ToString();
                                        newRow["bad_item_name"] = dr["bad_item_name"].ToString();

                                        newRow["bad_classify_code"] = dr["bad_classify_code"].ToString();
                                        newRow["bad_classify_name"] = dr["bad_classify_name"].ToString();
                                        newRow["problem_level"] = dr["problem_level"].ToString();
                                        newRow["badnum"] = dr["badnum"].ToString();
                                        badItemTable.Rows.Add(newRow.ItemArray);

                                    }

                                }

                                //Save the assigned bad point Detail_id to the t_aeqs_to_p88_po_record table
                                SaveDetailId(po, rout_no, assigned_quantity);


                            }


                        }

                        if (badItemTable.Rows.Count > 0)
                        {
                            foreach (DataRow row2 in badItemTable.Rows)
                            {
                                string product_id = string.Empty;
                                try
                                {
                                    product_id = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                                }
                                catch (Exception ex3)
                                {
                                    resultObject.IsSuccess = false;
                                    resultObject.ErrMsg = ex3.Message;
                                    return resultObject;
                                }
                                AEQS_TO_P88_SECTIONS aeqs_product = new AEQS_TO_P88_SECTIONS();
                                aeqs_product.sections_type = "aqlDefects";
                                aeqs_product.sections_title = "product";
                                aeqs_product.sections_result_id = product_result;
                                aeqs_product.sections_defective_parts = defect_parts;
                                aeqs_product.sections_qty_inspected = order_qty;
                                aeqs_product.sections_sampled_inspected = order_qty;
                                aeqs_product.sections_inspection_level = "100%inspection";
                                aeqs_product.sections_inspection_method = "normal";
                                aeqs_product.sections_aql_minor = 4.0m;
                                aeqs_product.sections_aql_major = 2.5m;
                                aeqs_product.sections_aql_critical = 1.0m;
                                aeqs_product.sections_max_critical_defects = max_critical_defects;
                                aeqs_product.sections_max_major_defects = max_major_defects;
                                aeqs_product.sections_max_minor_defects = max_minor_defects;
                                aeqs_product.sections_max_major_a_defects = 0m;
                                aeqs_product.sections_max_major_b_defects = 0m;
                                aeqs_product.sections_defects_label = row2["bad_item_name"].ToString();
                                aeqs_product.sections_defects_subsection = row2["bad_classify_name"].ToString();
                                aeqs_product.sections_defects_code = "FTW" + row2["bad_item_code"].ToString();
                                if (row2["problem_level"].ToString().Equals("主要"))
                                {
                                    aeqs_product.sections_defects_major_level = Convert.ToInt32(row2["badnum"].ToString());
                                }
                                else if (row2["problem_level"].ToString().Equals("次要"))
                                {
                                    aeqs_product.sections_defects_minor_level = Convert.ToInt32(row2["badnum"].ToString());
                                }
                                else if (row2["problem_level"].ToString().Equals("严重"))
                                {
                                    aeqs_product.sections_defects_critical_level = Convert.ToInt32(row2["badnum"].ToString());
                                }

                                #region product_sql
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
                                          ('{product_id}',
                                           '{unique_key}',
                                           '{aeqs_product.sections_type}',
                                           '{aeqs_product.sections_title}',
                                           '{aeqs_product.sections_result_id}',
                                           '{aeqs_product.sections_qty_inspected}',
                                           {aeqs_product.sections_sampled_inspected},
                                           {aeqs_product.sections_defective_parts},
                                           '{aeqs_product.sections_inspection_level}',
                                           '{aeqs_product.sections_inspection_method}',
                                           '{aeqs_product.sections_aql_minor}',
                                           '{aeqs_product.sections_aql_major}',
                                           '{aeqs_product.sections_aql_critical}',
                                           '{aeqs_product.sections_barcodes_value}',
                                           '{aeqs_product.sections_qty_type}',
                                           '{aeqs_product.sections_max_minor_defects}',
                                           '{aeqs_product.sections_max_major_defects}',
                                           '{aeqs_product.sections_max_major_a_defects}',
                                           '{aeqs_product.sections_max_major_b_defects}',
                                           '{aeqs_product.sections_max_critical_defects}',
                                           '{aeqs_product.sections_defects_label}',
                                           '{aeqs_product.sections_defects_subsection}',
                                           '{aeqs_product.sections_defects_code}',
                                           '{aeqs_product.sections_defects_critical_level}',
                                           '{aeqs_product.sections_defects_major_level}',
                                           '{aeqs_product.sections_defects_minor_level}',
                                           '{aeqs_product.sections_defects_comments}')";
                                #endregion product_sql

                                DB.ExecuteNonQuery(sql);
                                if (string.IsNullOrEmpty(row2["detail_id"].ToString()))
                                {
                                    continue;
                                }
                                List<string> source = row2["detail_id"].ToString().Split(',').ToList();
                                string text3 = string.Join(',', source.Select((string x) => "'" + x + "'"));
                                string sql5 = "select file_guid from tqc_task_detail_t_f  where union_id in (" + text3 + ")";
                                DataTable dataTable4 = DB.GetDataTable(sql5);
                                if (dataTable4.Rows.Count <= 0)
                                {
                                    continue;
                                }
                                int num5 = 1;
                                string text4 = string.Empty;
                                foreach (DataRow row3 in dataTable4.Rows)
                                {
                                    string sections_defects_pictures_full_filename = row3["file_guid"].ToString();
                                    AEQS_TO_P88_SECTIONS_F aEQS_TO_P88_SECTIONS_F = new AEQS_TO_P88_SECTIONS_F();
                                    aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_title = "IMAGE " + num5;
                                    aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_full_filename = sections_defects_pictures_full_filename;
                                    aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_number = num5;
                                    text4 += $"insert into T_aeqs_to_p88_sections_f (UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, sections_defects_pictures_full_filename, sections_defects_pictures_number, sections_defects_pictures_comment)\r\n                                               values ('{product_id}', '{aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_title}', '{aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_full_filename}','{aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_number}', '{aEQS_TO_P88_SECTIONS_F.sections_defects_pictures_comment}');";
                                    num5++;
                                }
                                if (!string.IsNullOrEmpty(text4))
                                {
                                    DB.ExecuteNonQuery("begin " + text4 + " end;");
                                }
                            }
                        }
                        else
                        {
                            string product_id = string.Empty;
                            try
                            {
                                product_id = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                            }
                            catch (Exception ex3)
                            {
                                resultObject.IsSuccess = false;
                                resultObject.ErrMsg = ex3.Message;
                                return resultObject;
                            }

                            #region product_sql
                            AEQS_TO_P88_SECTIONS aeqs_product = new AEQS_TO_P88_SECTIONS();
                            aeqs_product.sections_result_id = 1;
                            aeqs_product.sections_type = "aqlDefects";
                            aeqs_product.sections_title = "product";
                            aeqs_product.sections_result_id = product_result;
                            aeqs_product.sections_defective_parts = defect_parts;
                            aeqs_product.sections_qty_inspected = order_qty;
                            aeqs_product.sections_sampled_inspected = order_qty;
                            aeqs_product.sections_inspection_level = "100%inspection";
                            aeqs_product.sections_inspection_method = "normal";
                            aeqs_product.sections_aql_minor = 4.0m;
                            aeqs_product.sections_aql_major = 2.5m;
                            aeqs_product.sections_aql_critical = 1.0m;
                            aeqs_product.sections_max_critical_defects = max_critical_defects;
                            aeqs_product.sections_max_major_defects = max_major_defects;
                            aeqs_product.sections_max_minor_defects = max_minor_defects;
                            aeqs_product.sections_max_major_a_defects = 0m;
                            aeqs_product.sections_max_major_b_defects = 0m;

                            aeqs_package.sections_defects_minor_level = 0m;
                            aeqs_package.sections_defects_major_level = 0m;
                            aeqs_package.sections_defects_critical_level = 0m;


                            total_inspection_result = 1;

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
                                            sections_defects_minor_level,
                                            sections_defects_major_level,
                                            sections_defects_critical_level
                                            )
                                        values
                                          ('{product_id}',
                                           '{unique_key}',
                                           '{aeqs_product.sections_type}',
                                           '{aeqs_product.sections_title}',
                                           '{aeqs_product.sections_result_id}',
                                           '{aeqs_product.sections_qty_inspected}',
                                           {aeqs_product.sections_sampled_inspected},
                                           {aeqs_product.sections_defective_parts},
                                           '{aeqs_product.sections_inspection_level}',
                                           '{aeqs_product.sections_inspection_method}',
                                           '{aeqs_product.sections_aql_minor}',
                                           '{aeqs_product.sections_aql_major}',
                                           '{aeqs_product.sections_aql_critical}',
                                           '{aeqs_product.sections_barcodes_value}',
                                           '{aeqs_product.sections_qty_type}',
                                           '{aeqs_product.sections_max_minor_defects}',
                                           '{aeqs_product.sections_max_major_defects}',
                                           '{aeqs_product.sections_max_major_a_defects}',
                                           '{aeqs_product.sections_max_major_b_defects}',
                                            '{aeqs_product.sections_defects_minor_level}',
                                            '{aeqs_product.sections_defects_major_level}',
                                            '{aeqs_product.sections_defects_critical_level}')";

                            #endregion product_sql

                            DB.ExecuteNonQuery(sql);


                        }
                        #endregion


                        #region aeqs_assignment
                        string string2 = DB.GetString($@"select distinct prod_no from tqc_task_m  where task_no in (select task_no from tqc_task_m where mer_po like '%{po}%' and workshop_section_no = '" + rout_no + "')");
                        string string3 = DB.GetString("select s.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '" + string2 + "'");
                        if (string.IsNullOrEmpty(string3))
                        {
                            string3 = DB.GetString("select p.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '" + string2 + "'");
                        }
                        if (string.IsNullOrEmpty(string3))
                        {
                            string3 = DB.GetString("select p.name_s from bdm_rd_prod p where p.prod_no = '" + string2 + "'");
                        }
                        AEQS_TO_P88_ASSIGNMENT aEQS_TO_P88_ASSIGNMENT = new AEQS_TO_P88_ASSIGNMENT();
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_status_id = (aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_result_id == 1) ? 3 : 1;


                        aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_completed_date = Convert.ToDateTime(DB.GetString($@"select create_time
                                                                          from (select concat(concat(createdate, ' '), createtime) as create_time
                                                                                  from tqc_task_m
                                                                                 where mer_po like '%{po}%'
                                                                                   and workshop_section_no = '{rout_no}'
                                                                                 order by create_time desc)
                                                                         where rownum = 1"));

                        TimeSpan timeSpan = aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_completed_date - aEQS_TO_P88_LIST.date_started;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_date_inspection = aEQS_TO_P88_LIST.date_started;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_total_inspection_minutes = Math.Round(Convert.ToDecimal(timeSpan.TotalMinutes));
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_minor = 4.0m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major = 2.5m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_critical = 1.0m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major_a = 0m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major_b = 0m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_report_type_id = "31"; 
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspection_level = "100%inspection";
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspection_method = "normal";
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_exporter_erp_business_id = "011"; //APC Value 011, APE value 779
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_number = po;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_exporter_id = 233m;//APC Value 233,APE value 219
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_id = 215m;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_erp_business_id = "Adidas001";
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_project_id = 2062m;

                        string sqlUserName = $@"select p88_checker
  from t_aeqs_to_p88_inspector
 where checker_code in
       (select distinct production_line_code
           from (
              select *
                from tqc_task_m
               where mer_po like '%{po}%'
                 and WORKSHOP_SECTION_NO = '{rout_no}'
               order by concat(concat(createdate, ' '), createtime) asc)
               where rownum = 1
        )";
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspector_username = DB.GetString(sqlUserName);
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_sku_item_name = string3;
                        aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_result_id = total_inspection_result;
                        if (total_inspection_result == 1)
                        {
                            aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_status_id = 3;//accept
                        }
                        else if (total_inspection_result == 2)
                        {
                            aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_status_id = 1;//待批准
                        }

                        #region before PO change 2 on 20250218

                        //string sql6 = $@"select max(concat('{string2}_', b.ad_size)) as ad_size,
                        //                       sum(a.se_qty) as se_qty
                        //                  from bdm_se_order_size a
                        //                  left
                        //                  join base097m b

                        //                on a.size_no = b.FACTORY_SIZE
                        //                 where se_id in
                        //                       (select se_id from bdm_se_order_master where mer_po = '{po}')
                        //                   and a.se_qty > 0
                        //                 group by ad_size, a.se_qty";
                        //DataTable dataTable5 = DB.GetDataTable(sql6);
                        #endregion
                        #region After PO change 2 on 20250218
                        //码数部分
                        DataTable dataTable5 = null;
                        //If it is a merge order, then follow the merge logic
                        string merge_mark = DB.GetString($@"select so_mergr_mark from bdm_se_order_master where mer_po = '{po}'");
                        if (merge_mark.Equals("Y"))
                        {
                            string merge_sql = $@"select max('{string2}_' || b.ad_size) as ad_size,
                                                       a.po_line_item as po_seq,
                                                       max(a.po_line_qty)as se_qty
                                                from t_bdm_se_order_reference a
                                                left join base097m b on a.po_size = b.factory_size
                                                where a.se_id in
                                                     (select se_id from bdm_se_order_master where mer_po = '{po}')
                                                 and a.PO_LINE_QTY > 0
                                                group by ad_size, a.po_line_item";
                            dataTable5 = DB.GetDataTable(merge_sql);
                        }
                        else
                        {
                            //Edit on 7/1(PO Change)
                            string sql6 = $@"select max('{string2}_' || b.ad_size) as ad_size,
                                               sum(a.se_qty) as se_qty,
                                               max(po_seq) as po_seq
                                          from bdm_se_order_size a
                                          left join base097m b on a.size_no = b.FACTORY_SIZE
                                         where se_id in
                                               (select se_id from bdm_se_order_master where mer_po = '{po}')
                                           and a.se_qty > 0
                                         group by ad_size, a.se_qty";
                            dataTable5 = DB.GetDataTable(sql6);
                        }

                        #endregion

                        if (dataTable5.Rows.Count > 0)
                        {
                            foreach (DataRow row4 in dataTable5.Rows)
                            {
                                decimal se_qty = Convert.ToDecimal(row4["se_qty"].ToString());

                                aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_qty = se_qty;
                                aEQS_TO_P88_ASSIGNMENT.assignment_items_qty_to_inspect = se_qty;
                                aEQS_TO_P88_ASSIGNMENT.assignment_items_sampling_size = se_qty;
                                aEQS_TO_P88_ASSIGNMENT.assignment_items_sampled_inspected = se_qty;
                                aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_sku_sku_number = row4["ad_size"].ToString();
                                aEQS_TO_P88_ASSIGNMENT.assignment_items_qty_inspected = se_qty;
                                sql = $@"insert into t_aeqs_to_p88_assignment
                                              (union_id,
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
                                               assignment_items_po_line_qty)
                                            values
                                              ('{unique_key}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_sampled_inspected}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_result_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_status_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_qty_inspected}',
                                               to_date('{aEQS_TO_P88_ASSIGNMENT.assignment_items_inspection_completed_date}',
                                                       'yyyy-mm-dd hh24:mi:ss'),
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_total_inspection_minutes}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_sampling_size}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_qty_to_inspect}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_minor}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major_a}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_major_b}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_aql_critical}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_supplier_booking_msg}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_conclusion_remarks}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspector_username}',
                                               to_date('{aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_date_inspection}',
                                                       'yyyy-mm-dd hh24:mi:ss'),
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspection_level}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_inspection_method}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_exporter_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_exporter_erp_business_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_po_number}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_erp_business_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_importer_project_id}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_sku_sku_number}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_sku_item_name}',
                                               '{aEQS_TO_P88_ASSIGNMENT.assignment_items_assignment_report_type_id}',
                                              '{aEQS_TO_P88_ASSIGNMENT.assignment_items_po_line_qty}')";

                                DB.ExecuteNonQuery(sql);
                            }
                        }
                        #endregion


                        #region aeqs_passfail
                        sql = string.Empty;
                        AEQS_TO_P88_PASSFAIL aEQS_TO_P88_PASSFAIL = new AEQS_TO_P88_PASSFAIL();
                        aEQS_TO_P88_PASSFAIL.passfails_title = "mcs_availability_signature_compliance";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_general_compliance";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "grading_sheet_availability_signature_compliance";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_general_compliance";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "finished_uppers_shoes_pass_through_md";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_metal_detection_compliance";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "calibration_with_test_stick_approved_supplier";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_metal_detection_compliance";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "uv_c_treatment";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "3_mold_prevention";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "anti_mold_wrapping_paper";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "3_mold_prevention";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "line_stoppage_due_to_quality_incident";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "4_exceptional_management";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'no', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                        aEQS_TO_P88_PASSFAIL.passfails_title = "slip_on_inspection_pass_step_in_tool";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "checklist";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_fit";
                        string text5 = "yes";
                        string text6 = "pass";
                        int int5 = DB.GetInt32("select count(*) from qcm_dqa_mag_d m left join bdm_rd_prod p on m.shoes_code = p.shoe_no where p.prod_no = '" + string2 + "' and  m.QA_RISK_DETAILS_DESC = '入脚-袜套结构'");
                        if (int5 <= 0)
                        {
                            text5 = "N/A";
                            text6 = "na";
                        }
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', '" + text5 + "', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', '" + text6 + "');";
                        aEQS_TO_P88_PASSFAIL.passfails_title = "moisture_test_aquaboy_pass";
                        aEQS_TO_P88_PASSFAIL.passfails_subsection = "checklist";
                        aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_mold_prevention";
                        sql = sql + "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                        #endregion


                        //Check whether this po has been added to the record table. If not, add it. If it does, change the status.
                        int record_count = DB.GetInt32($@"select count(*) from t_aeqs_to_p88_po_record where po = '{po}' and workshop_section_no='{rout_no}'");
                        string sqlRecord2 = string.Empty;
                        if (record_count == 0 && hasAssignment == false)
                        {
                            string task_no = DB.GetString($@"select task_no from tqc_task_m where mer_po = '{po}' and workshop_section_no = '{rout_no}'");
                            sqlRecord2 = $@"insert into t_aeqs_to_p88_po_record(po,task_no,workshop_section_no,is_sync,creattime) values('{po}','{task_no}','{rout_no}', 'S','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                        }
                        else//PO is set to synchronized
                        {
                            sqlRecord2 = $"update t_aeqs_to_p88_po_record set is_sync = 'S' where po = '{po}' and workshop_section_no='{rout_no}'";
                        }
                        DB.ExecuteNonQuery(sqlRecord2);


                        //Set this TQC task ID to Scanned
                        string tqc_sync_sql = $@"update tqc_task_m set is_scan = 'Y' where mer_po like '%{po}%' and workshop_section_no = '{rout_no}'";
                        DB.ExecuteNonQuery(tqc_sync_sql);

                        DB.Commit();
                    }
                }


                //获取TQC任务重满足报工条件的po (1.报工完成，2.工段为贴底或加工，3.排除取消的订单,4.此PO要存在对应工段和po的TQC任务)
                //Obtain the PO whose TQC task satisfies the work reporting conditions (1. The work reporting is completed, 2. The work section is for bottoming or processing, 3. Canceled orders are excluded, 4. This PO must have TQC tasks corresponding to the work section and po)
                DataTable GetFinishWorkPO()
                {
                    string po_sql = $@"with po_data as
                                 ( --TQC PO deduplication of untransmitted tasks, if there are multiple ones, separate them into multiple lines
                                  select distinct REGEXP_SUBSTR(mer_po, '[^,]+', 1, level) po--,task_no
                                    from tqc_task_m
                                   where nvl(is_scan, 'N') != 'Y'      and
	                                  mer_po is not null
                                  connect by level <= REGEXP_COUNT(mer_po, '[^,]+')
                                         and rowid = prior rowid
                                         and prior dbms_random.value is not null),
                                work_po as
                                 ( --The related work report form shows that the work report is completed, the work section is bottom-up and processing, and the output PO
                                  select s.se_id as se_id,
                                          s.po as po,
                                          sum(finish_qty) as finish_qty,
                                          rout_no,
                                          inout_pz,
                                          i.se_qty as se_qty,
                                          max(i.status) as status
                                    from sjqdms_work_day s
                                   inner join po_data p
                                      on s.po = p.po
                                   inner join bdm_se_order_item i
                                      on s.se_id = i.se_id
                                   where inout_pz = 'OUT'
                                     and (rout_no = 'L' or rout_no = 'T')
                                     and i.status != 99 --Exclude canceled orders
--and (s.insert_date >= to_date('2023/12/27 22:00:00','yyyy-mm-dd hh24:mi:ss'))--TQC is synchronized to pivot88 online time  Real
and (s.insert_date >= to_date('2024/01/15 22:00:00','yyyy-mm-dd hh24:mi:ss'))--TQC is synchronized to pivot88 online time  Test
                                     and exists (
					                       select 1 from tqc_task_m t where  t.mer_po like '%' || s.po || '%' and s.rout_no = t.workshop_section_no and task_state in ('0', '1', '2') --It is required that the tasks corresponding to the work section exist in tqc
					                     )
                                    and not exists(
                                        select 1 from t_aeqs_to_p88_po_record t where s.po = t.po and s.rout_no = t.workshop_section_no and t.is_sync = 'S'  --Exclude POs that have been synchronized
                                    )
                                   group by s.se_id, s.po, rout_no, inout_pz,se_qty
                                  having sum(finish_qty) > se_qty or sum(finish_qty) = se_qty)
                                select se_id, po, finish_qty, se_qty,rout_no, inout_pz, status
                                  from work_po 
--where po in('0132377427','A132464195','0132599317','0132467146')			 
";

                    return DB.GetDataTable(po_sql);
                }

                //Get the total number of defects reported in the List main table and assign the number of defects to the PO
                void GetDefectParts(string po, string rout_no, out bool hasAssignment, string inspection_table, out decimal defect_parts)
                {
                    hasAssignment = false;
                    defect_parts = 0;

                    //Calculate the number of POs included in the TQC task of this Po
                    string count_sql = $@"select count(distinct REGEXP_SUBSTR(mer_po,'[^,]+',1,level)) po
                                            from(select task_no, mer_po
                                                   from tqc_task_m
                                                   where mer_po like '%{po}%' and workshop_section_no = '{rout_no}')
                                            connect by level <= REGEXP_COUNT(mer_po, '[^,]+')
                                            and rowid = prior rowid
                                            and prior dbms_random.value is not null
                                            order by po";
                    int containPoCount = DB.GetInt32(count_sql);


                    //The TQC task only contains one PO, and all defective numbers are assigned to this PO.
                    if (containPoCount == 1)
                    {
                        defect_parts = DB.GetDecimal($@"select count(*)
                                                                from tqc_task_detail_t d
                                                                inner join tqc_task_item_c c
                                                                on d.union_id = c.id
                                                                inner join {inspection_table} m
                                                                on c.inspection_code = m.inspection_code
                                                                left join bdm_aql_bad_classify_d s
                                                                on m.bad_item_code = s.bad_item_code
                                                                inner join bdm_aql_bad_classify y
                                                                on s.bad_classify_code = y.bad_classify_code
                                                                where d.task_no in (select task_no
                                                                                        from tqc_task_m
                                                                                        where mer_po = '{po}'
                                                                                        and workshop_section_no = '{rout_no}'
                                                                                        and task_state in ('0','1','2'))
                                                                and m.bad_item_code is not null
                                                                and m.bad_item_name is not null
                                                                and m.qc_type = 1 --TQC
                                                                and d.commit_type = 1");
                    }
                    //The TQC task contains multiple POs, and the defective numbers need to be evenly distributed to each PO.
                    else if (containPoCount > 1)
                    {
                        string taskSql = $@"select  t.task_no,t.mer_po,t.workshop_section_no,count(*) as defect
		                                            from tqc_task_m t
		                                            left join tqc_task_detail_t d on t.task_no = d.task_no
					                                            left join tqc_task_item_c c
					                                            on d.union_id = c.id
					                                            left join bdm_insp_fin_shoes_m m
					                                            on c.inspection_code = m.inspection_code
					                                            left join bdm_aql_bad_classify_d s
					                                            on m.bad_item_code = s.bad_item_code
					                                            left join bdm_aql_bad_classify y
					                                            on s.bad_classify_code = y.bad_classify_code
					                                            where t.mer_po like '%{po}%'
					                                            and t.WORKSHOP_SECTION_NO = '{rout_no}'
					                                            and m.bad_item_code is not null
					                                            and m.bad_item_name is not null
					                                            and m.qc_type = 1 --TQC
					                                            and d.commit_type = 1
		                                            group by t.task_no,t.mer_po,t.workshop_section_no
                                                    having count(*) > 0 -- 过滤掉不良数为0";
                        DataTable taskTable = DB.GetDataTable(taskSql);
                        if (taskTable.Rows.Count > 0)
                        {
                            //All tasks for this po
                            foreach (DataRow dr in taskTable.Rows)
                            {

                                // Has this task been assigned? If assigned, fetch it directly and skip this cycle.
                                string taskSql2 = $@"select task_no,assigned_quantity from t_aeqs_to_p88_po_record where task_no = '{dr["task_no"].ToString()}' and po = '{dr["mer_po"].ToString()}'";
                                DataTable taskTable2 = DB.GetDataTable(taskSql2);
                                if (taskTable2.Rows.Count > 0)
                                {
                                    string assigned_quantity = taskTable2.Rows[0]["assigned_quantity"].ToString();
                                    if (!string.IsNullOrEmpty(assigned_quantity))
                                    {
                                        defect_parts += Convert.ToDecimal(assigned_quantity);
                                    }
                                    continue;
                                }


                                int task_defects = Convert.ToInt32(dr["defect"].ToString());
                                List<string> polist = dr["mer_po"].ToString().Split(",").ToList();

                                //if (task_defects == 0 || polist.Count < 1) continue;//Interrupt when there are no bad numbers or no po
                                if (polist.Count == 1)// If there is only one po, all is given to that po
                                {
                                    defect_parts += task_defects;
                                    continue;
                                }
                                else if (polist.Count > 1)
                                {
                                    //1.When there are multiple POs, it is necessary to distribute the bad numbers evenly.
                                    int avgNumInt = task_defects / polist.Count();//average integer
                                    int remainNum = task_defects % polist.Count();//Modulo operation, taking remainder
                                    List<PoEntity> poEntityList = new List<PoEntity>();
                                    foreach (string item in polist)
                                    {
                                        PoEntity poEntity = new PoEntity();
                                        poEntity.mer_po = item;
                                        poEntity.task_no = dr["task_no"].ToString();
                                        poEntity.defect_nums = avgNumInt;
                                        poEntity.workshop_section_no = dr["workshop_section_no"].ToString();

                                        poEntityList.Add(poEntity);
                                    }


                                    //2.Distribute remainder
                                    if (remainNum > 0)
                                    {
                                        bool flag = true;
                                        while (flag)
                                        {
                                            for (int i = 0; i < poEntityList.Count; i++)
                                            {
                                                if (remainNum > 0)
                                                {
                                                    poEntityList[i].defect_nums++;
                                                    remainNum--;
                                                }
                                                else
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    //3.Save the assignment results of each task
                                    foreach (PoEntity item in poEntityList)
                                    {
                                        if (item.mer_po.Equals(po))
                                        {
                                            defect_parts += item.defect_nums;
                                        }

                                        //Query whether this task has been assigned to PO
                                        int n = DB.GetInt32($@"select count(*) from t_aeqs_to_p88_po_record where po = '{item.mer_po}' and task_no = '{item.task_no}' and workshop_section_no = '{item.workshop_section_no}'");
                                        if (n == 0)
                                        {
                                            string sqlRecord = $@"insert into t_aeqs_to_p88_po_record(po,task_no,assigned_quantity,workshop_section_no,creattime) values('{item.mer_po}', '{item.task_no}', {item.defect_nums},'{item.workshop_section_no}','{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}')";
                                            DB.ExecuteNonQuery(sqlRecord);
                                        }
                                    }
                                    hasAssignment = true;

                                }



                            }
                        }


                    }

                }

                //Obtain the defective items of PO corresponding to a single TQC task (there is a configuration relationship and it is the first inspection)
                DataTable GetBadItemTable(string po, string rout_no, string inspection_table)
                {
                    string badItemSql = $@"with bad_item_table as
		                                     (select d.id as detail_id,
						                                     c.inspection_name,
						                                     count(union_id) as badnum,
						                                     m.bad_item_code,
						                                     m.bad_item_name,
						                                     s.bad_classify_code,
						                                     y.bad_classify_name,
						                                     m.problem_level
				                                    from tqc_task_detail_t d 
			                                     inner join tqc_task_item_c c 
					                                    on d.union_id = c.id 
			                                     inner join {inspection_table} m 
					                                    on c.inspection_code = m.inspection_code 
				                                    left join bdm_aql_bad_classify_d s
					                                    on m.bad_item_code = s.bad_item_code 
			                                     inner join bdm_aql_bad_classify y 
					                                    on s.bad_classify_code = y.bad_classify_code 
			                                     where d.task_no in
						                                     (select task_no from tqc_task_m where mer_po = '{po}' and workshop_section_no = '{rout_no}' and task_state in ('0','1','2'))
				                                     and m.bad_item_code is not null
				                                     and m.bad_item_name is not null
				                                     and m.qc_type = 1 -- TQC
                                                     and d.commit_type = 1
			                                     group by d.id,
								                            c.inspection_code,
								                            c.inspection_name,
								                            m.problem_level,
								                            m.bad_item_code,
								                            m.bad_item_name,
								                            s.bad_classify_code,
								                            m.problem_level,
								                            y.bad_classify_name) 
		                                    select listagg(distinct detail_id, ',') as detail_id,
					                                listagg(distinct inspection_name, ',') as inspection_name,
					                                bad_item_code,
					                                bad_item_name,
					                                bad_classify_code,
					                                bad_classify_name,
					                                problem_level,
					                                sum(badnum) as badnum
			                                    from bad_item_table
		                                     group by bad_item_code,
							                            bad_item_name,
							                            bad_item_name,
							                            bad_classify_code,
							                            bad_classify_name,
							                            problem_level";

                    return DB.GetDataTable(badItemSql);
                }

                //Obtain the defective items allocated by PO from the TQC of multiple POs (excluding defective points assigned to other POs)
                DataTable GetAssignBadItem(string po, string rout_no, int assigned_quantity)
                {
                    #region 分配不良数SQL
                    string assignmentCountsSql = $@"with bad_item_table as
                                                             (select task_no,--查询出需要分配的不良项目和数量
                                                                     detail_id,
                                                                     inspection_name,
                                                                     union_id,
                                                                     bad_item_code,
                                                                     bad_item_name,
                                                                     bad_classify_code,
                                                                     bad_classify_name,
                                                                     problem_level
                                                                from (
                                                                      --tqc_defect_detail
                                                                      select t.task_no,
                                                                              d.id as detail_id,
                                                                              c.inspection_name,
                                                                              c.inspection_code,
                                                                              union_id,
                                                                              m.bad_item_code,
                                                                              m.bad_item_name,
                                                                              s.bad_classify_code,
                                                                              y.bad_classify_name,
                                                                              m.problem_level
                                                                        from tqc_task_detail_t d
                                                                       left join tqc_task_item_c c on d.union_id = c.id
                                                                       left join tqc_task_m t on d.task_no = t.task_no
                                                                       left join bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code
                                                                       left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code
                                                                       left join bdm_aql_bad_classify y  on s.bad_classify_code = y.bad_classify_code
                                                                       where t.task_no in ( --查询此po有多少需要分配不良的任务和数量
                                                                                            select distinct task_no  from t_aeqs_to_p88_po_record where po = '{po}' and workshop_section_no = '{rout_no}')
									                                        --and t.workshop_section_no = '{rout_no}'
									                                        and t.task_state in ('0', '1', '2')
									                                        and m.bad_item_code is not null
									                                        and m.bad_item_name is not null
									                                        and m.qc_type = 1 -- TQC
                                                                            and d.commit_type = 1
                                                                            and d.id not in(
                                                                                --排除对应TQC任务已经分配过的不良点
                                                                                select distinct REGEXP_SUBSTR(defect_detail_id,'[^,]+',1,level) detail_id
                                                                                        from t_aeqs_to_p88_po_record
                                                                                        where defect_detail_id is not null
                                                                                    and task_no in(
		                                                                                select task_no from 	t_aeqs_to_p88_po_record
		                                                                                where po =  '{po}' and  WORKSHOP_SECTION_NO = '{rout_no}'                                 
                                                                                    )   
                                                                                connect by level <= REGEXP_COUNT(defect_detail_id,'[^,]+')
                                                                                        and rowid = prior rowid
                                                                                        and prior dbms_random.value is not null)
--where
                                                                            and d.id not in
                                                                                        (
                                                                                        --排除这个po已经分配到的tqc不良项目
                                                                                        select distinct REGEXP_SUBSTR(defect_detail_id,'[^,]+',1,level) detail_id
                                                                                                from t_aeqs_to_p88_po_record
                                                                                                where defect_detail_id is not null
                                                                                            and po = '{po}'
                                                                                            and workshop_section_no = '{rout_no}'
                                                                                        connect by level <= REGEXP_COUNT(defect_detail_id,'[^,]+')
                                                                                                and rowid = prior rowid
                                                                                                and prior dbms_random.value is not null)                                                                                                    
                                                                        order by concat(concat(d.createdate,','),d.createtime))

                                                               where rownum <= {assigned_quantity})
                                                            select max(task_no) task_no,
                                                                   listagg(distinct detail_id, ',') as detail_id,
                                                                   listagg(distinct inspection_name, ',') as inspection_name,
                                                                   bad_item_code,
                                                                   bad_item_name,
                                                                   bad_classify_code,
                                                                   bad_classify_name,
                                                                   problem_level,
                                                                   count(union_id) as badnum
                                                              from bad_item_table
                                                             group by bad_item_code,
                                                                      bad_item_name,
                                                                      bad_classify_code,
                                                                      bad_classify_name,
                                                                      problem_level";
                    #endregion

                    return DB.GetDataTable(assignmentCountsSql);
                }

                //Save the allocated bad points to the t_aeqs_to_P88_po_record table
                void SaveDetailId(string po, string rout_no, int assigned_quantity)
                {
                    string detailIdSql = $@"--记录到record表
                                                        select task_no, listagg(detail_id, ',') as detail_id from (
                                                               with bad_item_table as (
			                                                                    select task_no, --查询出需要分配的不良项目和数量
                                                                                    detail_id
                                                                                from (
                                                                                    --tqc_defect_detail
                                                                                    select t.task_no,
                                                                                            d.id as detail_id                                                       
                                                                                        from tqc_task_detail_t d
                                                                                        left join tqc_task_item_c c on d.union_id = c.id
                                                                                        left join tqc_task_m t on d.task_no = t.task_no
                                                                                        left join bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code
                                                                                        left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code
                                                                                        left join bdm_aql_bad_classify y on s.bad_classify_code = y.bad_classify_code
                                                                                        where t.task_no in
                                                                                            ( --查询此po有多少需要分配不良的任务和数量
                                                                                            select distinct task_no  from t_aeqs_to_p88_po_record where po = '{po}' and workshop_section_no = '{rout_no}')
                                                                                        --and t.workshop_section_no = '{rout_no}'
                                                                                        and t.task_state in ('0', '1', '2')
                                                                                        and m.bad_item_code is not null
                                                                                        and m.bad_item_name is not null
                                                                                        and m.qc_type = 1 -- TQC  
                                                                                        and d.commit_type = 1
                                                                                        and d.id not in(
                                                                                            --排除对应TQC任务已经分配过的不良点
                                                                                            select distinct REGEXP_SUBSTR(defect_detail_id,'[^,]+',1,level) detail_id
                                                                                                  from t_aeqs_to_p88_po_record
                                                                                                  where defect_detail_id is not null
                                                                                              and task_no in(
		                                                                                            select task_no from 	t_aeqs_to_p88_po_record
		                                                                                            where po =  '{po}' and  WORKSHOP_SECTION_NO = '{rout_no}'                                 
                                                                                                )   
                                                                                            connect by level <= REGEXP_COUNT(defect_detail_id,'[^,]+')
                                                                                                  and rowid = prior rowid
                                                                                                  and prior dbms_random.value is not null)
                                                                                        and d.id not in
                                                                                            (
                                                                                            --排除这个po已经分配到的tqc不良点
                                                                                            select distinct REGEXP_SUBSTR(defect_detail_id,'[^,]+',1,level) detail_id
                                                                                                    from t_aeqs_to_p88_po_record
                                                                                                    where defect_detail_id is not null
                                                                                                and po = '{po}'
                                                                                                and workshop_section_no = '{rout_no}'
                                                                                            connect by level <= REGEXP_COUNT(defect_detail_id,'[^,]+')
                                                                                                    and rowid = prior rowid
                                                                                                    and prior dbms_random.value is not null)
                                                                    order by concat(concat(d.createdate,','),d.createtime))
                                                                    where rownum <= {assigned_quantity})
                                                                 select task_no,detail_id from bad_item_table) 
				                                                 group by task_no";


                    DataTable task_detailid_table = DB.GetDataTable(detailIdSql);
                    if (task_detailid_table.Rows.Count > 0)
                    {
                        foreach (DataRow dr in task_detailid_table.Rows)
                        {
                            DB.ExecuteNonQuery($@"update t_aeqs_to_p88_po_record set defect_detail_id = '{dr["detail_id"].ToString()}' where po = '{po}' and task_no = '{dr["task_no"].ToString()}' and workshop_section_no = '{rout_no}'");
                        }
                    }

                }

                resultObject.IsSuccess = true;

            }
            catch (Exception ex4)
            {
                resultObject.IsSuccess = false;
                resultObject.ErrMsg = ex4.Message;
            }
            finally
            {

                DB.Close();
            }
            return resultObject;
        }

        /// <summary>
        /// Generate TQC rummage tasks based on the results of AQL inspection tasks
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static ResultObject GenerateTQCReworkTaskByAQL(RequestObject OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject result = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                DB.Open();

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(OBJ.UserToken);//获取的登陆人信息
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string createtime = DateTime.Now.ToString("HH:mm:ss");//时间

                string aqlTaskSql = $@"select m.unique_key, s.sections_qty_inspected, r.task_no, report_type, po, reprot_result 
                                          from t_aeqs_to_p88_list m
                                          left join (select distinct union_id,
                                                                     assignment_items_inspection_result_id      as reprot_result,
                                                                     assignment_items_assignment_report_type_id as report_type,
                                                                     assignment_items_po_line_po_number         as po
                                                       from t_aeqs_to_p88_assignment) a
                                            on m.unique_key = a.union_id
                                          left join (select distinct union_id, sections_qty_inspected
                                                       from t_aeqs_to_p88_sections) s
                                            on m.unique_key = s.union_id
                                          left join t_aql_cma_task_p88_relation r
                                            on m.unique_key = r.p88_unique_key
                                         where m.is_sync = 'S'
                                           and nvl(m.is_rework,'N') != 'Y'
                                           and reprot_result = 2
                                           and report_type = 9";
                DataTable reworkTable = DB.GetDataTable(aqlTaskSql);

                //Create a TQC rummage task
                if (reworkTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in reworkTable.Rows)
                    {
                        string aql_task_no = dr["task_no"].ToString();
                        string po = dr["po"].ToString();
                        string aql_rework_quantitiy = dr["sections_qty_inspected"].ToString();
                        string task_no = string.Empty;//Task number
                        string unique_key = dr["unique_key"].ToString();


                        task_no = DB.GetString($@"SELECT task_no FROM(select task_no from tqc_task_m where CREATEDATE = '{DateTime.Now:yyyy-MM-dd}'  order by TO_NUMBER(REPLACE(task_no, 'T', '')) DESC)t WHERE ROWNUM = 1");
                        if (string.IsNullOrWhiteSpace(task_no))
                        {
                            task_no = "T" + DateTime.Now.ToString("yyyyMMdd") + "1";
                        }
                        else
                        {
                            task_no = $@"T{DateTime.Now.ToString("yyyyMMdd")}" + (Convert.ToInt32(task_no.Replace($@"T{DateTime.Now.ToString("yyyyMMdd")}", "")) + 1);
                        }

                        string artInfoSql = $@"select art_no,p.shoe_no,develop_season,s.name_t,r.se_id,r.workorder_no
	                                            from aql_cma_task_list_m m
	                                            left join bdm_rd_prod p on m.art_no = p.prod_no
	                                            left join bdm_rd_style s on s.shoe_no =p.shoe_no
	                                            left join bdm_se_order_item i on p.prod_no = i.prod_no
	                                            left join bdm_se_order_master r on i.se_id = r.se_id
                                                where task_no = '{dr["task_no"].ToString()}'";
                        DataTable artInfoTable = DB.GetDataTable(artInfoSql);
                        if (artInfoTable.Rows.Count > 0)
                        {
                            string art_no = artInfoTable.Rows[0]["art_no"].ToString();
                            string shoe_no = artInfoTable.Rows[0]["shoe_no"].ToString();
                            string develop_season = artInfoTable.Rows[0]["develop_season"].ToString();
                            string name_t = artInfoTable.Rows[0]["name_t"].ToString();
                            string se_id = artInfoTable.Rows[0]["se_id"].ToString();
                            string workorder_no = artInfoTable.Rows[0]["workorder_no"].ToString();

                            string sql = $@"insert into tqc_task_m(task_no,workshop_section_no,develop_season,shoe_no,prod_no,createby,createdate,createtime,task_state,mer_po,aql_task_no, aql_rework_quantity,se_id,workorderno)
                                            values('{task_no}','L','{develop_season}','{shoe_no}','{art_no}','{user}','{createdate}','{createtime}','3','{po}','{aql_task_no}','{aql_rework_quantitiy}','{se_id}','{workorder_no}')";
                            DB.ExecuteNonQuery(sql);

                            //Generate section information
                            string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='L'");
                            string inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                            string tabname = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{inspection_type}' and enum_code in ('0','1','2','3','4','5','6','7')");
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
                                            WHERE qc_type='1' and ROWNUM<=20
                                         ORDER BY id asc";
                                DataTable inspectiondt = DB.GetDataTable(sql);
                                foreach (DataRow item in inspectiondt.Rows)
                                {
                                    sql = $@"insert into tqc_task_item_c (task_no,inspection_type,inspection_code,inspection_name,qc_type,judgment_criteria,
                                standard_value,shortcut_key,createby,createdate,createtime) 
                                values('{task_no}','{inspection_type}','{item["inspection_code"]}','{item["inspection_name"]}','{item["qc_type"]}',
                                '{item["judgment_criteria"]}','{item["standard_value"]}','{item["shortcut_key"]}','{user}','{createdate}','{createtime}')";
                                    DB.ExecuteNonQuery(sql);

                                }
                            }

                            //Generate point bin data
                            string pointBoxSql = $@"select cr_size,se_qty from aql_cma_task_list_m_pb where task_no = '{aql_task_no}'";
                            DataTable piontBoxTable = DB.GetDataTable(pointBoxSql);
                            if (piontBoxTable.Rows.Count > 0)
                            {
                                string sql2 = string.Empty;
                                foreach (DataRow dr2 in piontBoxTable.Rows)
                                {
                                    sql2 += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,se_qty,createby,createdate,createtime) 
                                                 values('{task_no}','{dr2["cr_size"]}','{dr2["se_qty"]}','{user}','{createdate}','{createtime}');";
                                }
                                DB.ExecuteNonQuery("begin " + sql2 + " end;");
                            }

                            //Update the aql task to the created rummage task
                            string reworkSql = $@"update t_aeqs_to_p88_list set is_rework = 'Y' where unique_key = '{unique_key}'";
                            DB.ExecuteNonQuery(reworkSql);


                        }
                    }
                }
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = true;
                result.ErrMsg = ex.Message;
            }

            return result;
        }

        /// <summary>
        ///Save rummaging datasss
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static ResultObject SavePointBoxData(RequestObject OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject result = new ResultObject();
            DataBase DB = new DataBase();
            string Data = reqObj.Data.ToString();
            var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
            string productline = jarr.ContainsKey("productline") ? jarr["productline"].ToString() : "";
            string depart = jarr.ContainsKey("depart") ? jarr["depart"].ToString() : "";
            string stage = jarr.ContainsKey("stage") ? jarr["stage"].ToString() : "";
            string _task_no = jarr.ContainsKey("_task_no") ? jarr["_task_no"].ToString() : "";
            DataTable pointBoxTable = jarr.ContainsKey("pointBoxTable") ? JsonConvert.DeserializeObject<DataTable>(jarr["pointBoxTable"].ToString()) : new DataTable();

            try
            {
                DB = new DataBase(reqObj);
                DB.Open();
                string sql = $@"update tqc_task_m set production_line_code = '{productline}',department = '{depart}',stage = '{stage}' where task_no = '{_task_no}'";
                DB.ExecuteNonQuery(sql);

                string caseNoSql = string.Empty;
                if (pointBoxTable.Rows.Count > 0)
                {
                    foreach (DataRow dr in pointBoxTable.Rows)
                    {
                        caseNoSql += $@"update aql_cma_task_list_m_pb set case_no = '{dr["case_no"].ToString()}' where task_no = '{_task_no}' and cr_size = '{dr["size"].ToString()}';";
                    }
                    DB.ExecuteNonQuery("begin " + caseNoSql + " end;");
                }
            }
            catch (Exception ex)
            {
                result.ErrMsg = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// TQC rummaging tasks are synchronized to pivot88
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static ResultObject TranserTQCReworkDataToPivot88(RequestObject OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject result = new ResultObject();
            DataBase DB = new DataBase();
            string Data = reqObj.Data.ToString();
            var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
            string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//date
            //string time = DateTime.Now.ToString("HH:mm:ss");//时间

            try
            {
                DB = new DataBase(reqObj);
                DB.Open();
                DB.BeginTransaction();
                string po = DB.GetString($@"select mer_po from tqc_task_m where task_no = '{task_no}'");
                string workshop_section_no = DB.GetString($@"select workshop_section_no from tqc_task_m where task_no = '{task_no}'");


                string unique_key = string.Empty;
                try
                {
                    string id = DB.GetString("SELECT T_AEQS_TO_P88_LIST_ID_SEQ.nextval FROM dual");
                    unique_key = "apache5_" + id.PadLeft(3, '0');
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = ex.Message;
                    return result;
                }
                #region aeqs_main
                AEQS_TO_P88_LIST aeqs_main = new AEQS_TO_P88_LIST();
                aeqs_main.unique_key = unique_key;
                aeqs_main.status = "Submitted";
                aeqs_main.date_started = DB.GetDateTime($@"select concat(concat(createdate,' '),createtime) as createtime from tqc_task_m where task_no = '{task_no}'");
                aeqs_main.defective_parts = GetDefective_parts(task_no);
                aeqs_main.assignment_items_assignment_report_type_id = 35;
                aeqs_main.passfails_0_title = "inspected_carton_numbers";
                aeqs_main.passfails_0_type = "list";
                aeqs_main.passfails_0_subsection = "actual_inspection";
                aeqs_main.passfails_0_listvalues_value = DB.GetString($@"select listagg(distinct case_no, '/') within group(order by to_number(case_no) asc) as case_no from (select distinct REGEXP_SUBSTR(case_no, '[^/]+', 1, level) case_no from aql_cma_task_list_m_pb where task_no = '{task_no}' connect by level <= REGEXP_COUNT(case_no, '[^/]+') and rowid = prior rowid and prior dbms_random.value is not null)");

                string sql = $@"insert into t_aeqs_to_p88_list
                                    (
                                    unique_key,
                                    status,
                                    date_started,
                                    defective_parts,
                                    passfails_0_title,
                                    passfails_0_type,
                                    passfails_0_subsection,
                                    passfails_0_listvalues_value,
                                    assignment_items_assignment_report_type_id)
                                values
                                    (
                                    '{unique_key}',
                                    '{aeqs_main.status}',
                                    to_date('{aeqs_main.date_started}','yyyy-mm-dd hh24:mi:ss'),
                                    '{aeqs_main.defective_parts}',
                                    '{aeqs_main.passfails_0_title}',
                                    '{aeqs_main.passfails_0_type}',
                                    '{aeqs_main.passfails_0_subsection}',
                                    '{aeqs_main.passfails_0_listvalues_value}',
                                    '{aeqs_main.assignment_items_assignment_report_type_id}'
                                    )";
                DB.ExecuteNonQuery(sql);
                #endregion


                #region aeqs_package
                string package_id = string.Empty;
                string se_qty = DB.GetString($@"select nvl(aql_rework_quantity,'0') as se_qty from tqc_task_m where task_no = '{task_no}'");
                try
                {
                    package_id = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                }
                catch (Exception ex2)
                {
                    result.IsSuccess = false;
                    result.ErrMsg = ex2.Message;
                    return result;
                }

                AEQS_TO_P88_SECTIONS aeqs_package = new AEQS_TO_P88_SECTIONS();
                aeqs_package.sections_type = "aqlDefects";
                aeqs_package.sections_title = "packing_packaging_labelling";
                aeqs_package.sections_result_id = 1;
                aeqs_package.sections_qty_inspected = DB.GetInt32($@"select NVL(aql_rework_quantity,0) from tqc_task_m where task_no = '{task_no}'");
                aeqs_package.sections_sampled_inspected = aeqs_package.sections_qty_inspected;
                aeqs_package.sections_defective_parts = 0;
                aeqs_package.sections_inspection_level = "100%inspection";
                aeqs_package.sections_inspection_method = "normal";
                aeqs_package.sections_aql_minor = 2.5m;//AC13
                aeqs_package.sections_aql_major = 1.5m;//AC12
                aeqs_package.sections_aql_critical = 0.01m;//AC01
                aeqs_package.sections_barcodes_value = "001";
                aeqs_package.sections_qty_type = "carton";

                Dictionary<string, int> maxReceptDic = GetMaxReceptNumDic(se_qty);
                aeqs_package.sections_max_minor_defects = maxReceptDic["minor"];
                aeqs_package.sections_max_major_defects = maxReceptDic["major"];
                aeqs_package.sections_max_critical_defects = maxReceptDic["critical"];
                aeqs_package.sections_max_major_a_defects = 0;
                aeqs_package.sections_max_major_b_defects = 0;

                aeqs_package.sections_defects_critical_level = 0;
                aeqs_package.sections_defects_major_level = 0;
                aeqs_package.sections_defects_minor_level = 0;
                aeqs_package.sections_defects_comments = "aqlDefects";

                #region aeqs_package_sql
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
                                   SECTIONS_DEFECTS_MINOR_LEVEL)
                                values
                                  ('{package_id}',
                                   '{unique_key}',
                                   '{aeqs_package.sections_type}',
                                   '{aeqs_package.sections_title}',
                                   '{aeqs_package.sections_result_id}',
                                   '{aeqs_package.sections_qty_inspected}',
                                   '{aeqs_package.sections_sampled_inspected}',
                                   '{aeqs_package.sections_defective_parts}',
                                   '{aeqs_package.sections_inspection_level}',
                                   '{aeqs_package.sections_inspection_method}',
                                   '{aeqs_package.sections_aql_minor}',
                                   '{aeqs_package.sections_aql_major}',
                                   '{aeqs_package.sections_aql_critical}',
                                   '{aeqs_package.sections_barcodes_value}',
                                   '{aeqs_package.sections_qty_type}',
                                   '{aeqs_package.sections_max_minor_defects}',
                                   '{aeqs_package.sections_max_major_defects}',
                                   '{aeqs_package.sections_max_major_a_defects}',
                                   '{aeqs_package.sections_max_major_b_defects}',
                                   '{aeqs_package.sections_max_critical_defects}',
                                   '{aeqs_package.sections_defects_label}',
                                   '{aeqs_package.sections_defects_subsection}',
                                   '{aeqs_package.sections_defects_code}',
                                   '{aeqs_package.sections_defects_critical_level}',
                                   '{aeqs_package.sections_defects_major_level}',
                                   '{aeqs_package.sections_defects_minor_level}')";
                #endregion

                DB.ExecuteNonQuery(sql);
                #endregion

                int totalResult = GetTotalResult(se_qty);//总结果

                #region aeqs_product
                DataTable productTable = GetProductDefectitem(task_no);
                if (productTable.Rows.Count > 0)
                {
                    sql = string.Empty;
                    foreach (DataRow dr in productTable.Rows)
                    {
                        string productId = string.Empty;
                        try
                        {
                            //从序列获取id
                            productId = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                        }
                        catch (Exception ex)
                        {
                            result.IsSuccess = false;
                            result.ErrMsg = ex.Message;
                            return result;
                        }

                        AEQS_TO_P88_SECTIONS aeqs_product = new AEQS_TO_P88_SECTIONS();
                        aeqs_product.sections_type = "aqlDefects";
                        aeqs_product.sections_title = "product";
                        aeqs_product.sections_result_id = totalResult;//与总结果一致。
                        aeqs_product.sections_defective_parts = aeqs_main.defective_parts;
                        aeqs_product.sections_qty_inspected = Convert.ToInt32(se_qty);
                        aeqs_product.sections_sampled_inspected = Convert.ToInt32(se_qty);
                        aeqs_product.sections_inspection_level = "100%inspection";
                        aeqs_product.sections_inspection_method = "normal";
                        aeqs_product.sections_aql_minor = 2.5m;
                        aeqs_product.sections_aql_major = 1.5m;
                        aeqs_product.sections_aql_critical = 0.01m;
                        aeqs_product.sections_max_minor_defects = maxReceptDic["minor"];
                        aeqs_product.sections_max_major_defects = maxReceptDic["major"];
                        aeqs_product.sections_max_critical_defects = maxReceptDic["critical"];
                        aeqs_product.sections_max_major_a_defects = 0;
                        aeqs_product.sections_max_major_b_defects = 0;

                        aeqs_product.sections_defects_label = dr["bad_item_name"].ToString();
                        aeqs_product.sections_defects_subsection = dr["bad_classify_name"].ToString();
                        aeqs_product.sections_defects_code = "FTW" + dr["bad_item_code"].ToString();
                        if (dr["problem_level"].ToString().Equals("主要")) aeqs_product.sections_defects_major_level = Convert.ToInt32(dr["badnum"].ToString());
                        else if (dr["problem_level"].ToString().Equals("次要")) aeqs_product.sections_defects_minor_level = Convert.ToInt32(dr["badnum"].ToString());
                        else if (dr["problem_level"].ToString().Equals("严重")) aeqs_product.sections_defects_critical_level = Convert.ToInt32(dr["badnum"].ToString());

                        #region aeqs_product_sql
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
                                   '{unique_key}',
                                   '{aeqs_product.sections_type}',
                                   '{aeqs_product.sections_title}',
                                   '{aeqs_product.sections_result_id}',
                                   '{aeqs_product.sections_qty_inspected}',
                                    {aeqs_product.sections_sampled_inspected},
                                    {aeqs_product.sections_defective_parts},
                                   '{aeqs_product.sections_inspection_level}',
                                   '{aeqs_product.sections_inspection_method}',
                                   '{aeqs_product.sections_aql_minor}',
                                   '{aeqs_product.sections_aql_major}',
                                   '{aeqs_product.sections_aql_critical}',
                                   '{aeqs_product.sections_barcodes_value}',
                                   '{aeqs_product.sections_qty_type}',
                                   '{aeqs_product.sections_max_minor_defects}',
                                   '{aeqs_product.sections_max_major_defects}',
                                   '{aeqs_product.sections_max_major_a_defects}',
                                   '{aeqs_product.sections_max_major_b_defects}',
                                   '{aeqs_product.sections_max_critical_defects}',
                                   '{aeqs_product.sections_defects_label}',
                                   '{aeqs_product.sections_defects_subsection}',
                                   '{aeqs_product.sections_defects_code}',
                                   '{aeqs_product.sections_defects_critical_level}',
                                   '{aeqs_product.sections_defects_major_level}',
                                   '{aeqs_product.sections_defects_minor_level}',
                                   '{aeqs_product.sections_defects_comments}');";


                        #endregion


                        #region aeqs_product_f

                        List<string> idList = dr["detail_id"].ToString().Split(',').ToList();
                        string detail_ids = string.Join(',', idList.Select(x => "'" + x + "'"));
                        string product_img_sql = $@"select file_guid from tqc_task_detail_t_f  where union_id in ({detail_ids})";

                        DataTable product_img_table = DB.GetDataTable(product_img_sql);
                        if (product_img_table.Rows.Count > 0)
                        {
                            int num = 1;
                            string img_sql = string.Empty;
                            foreach (DataRow dr_img in product_img_table.Rows)
                            {
                                AEQS_TO_P88_SECTIONS_F aeqs_section_f = new AEQS_TO_P88_SECTIONS_F();
                                aeqs_section_f.sections_defects_pictures_title = "IMAGE" + num;
                                aeqs_section_f.sections_defects_pictures_full_filename = dr_img["file_guid"].ToString();
                                aeqs_section_f.sections_defects_pictures_number = num;

                                img_sql += $@"insert into t_aeqs_to_p88_sections_f
                                              (union_id,
                                               sections_defects_pictures_title,
                                               sections_defects_pictures_full_filename,
                                               sections_defects_pictures_number,
                                               sections_defects_pictures_comment)
                                            values
                                              ('{productId}',
                                               '{aeqs_section_f.sections_defects_pictures_title}',
                                               '{aeqs_section_f.sections_defects_pictures_full_filename}',
                                               '{aeqs_section_f.sections_defects_pictures_number}',
                                               '{aeqs_section_f.sections_defects_pictures_comment}');";
                            }
                            if (!string.IsNullOrEmpty(img_sql))
                            {
                                DB.ExecuteNonQuery("begin " + img_sql + " end;");
                            }
                        }
                        #endregion
                    }
                    if (!string.IsNullOrEmpty(sql))
                    {
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                    }
                }
                else
                {
                    totalResult = 1;
                    string product_id = string.Empty;
                    try
                    {
                        product_id = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                    }
                    catch (Exception ex)
                    {
                        result.IsSuccess = false;
                        result.ErrMsg = ex.Message;
                        return result;
                    }

                    #region aeqs_product_sql2
                    AEQS_TO_P88_SECTIONS aeqs_product = new AEQS_TO_P88_SECTIONS();
                    aeqs_product.sections_type = "aqlDefects";
                    aeqs_product.sections_title = "product";
                    aeqs_product.sections_result_id = totalResult;
                    aeqs_product.sections_defective_parts = 0;
                    aeqs_product.sections_qty_inspected = Convert.ToInt32(se_qty);
                    aeqs_product.sections_sampled_inspected = Convert.ToInt32(se_qty);
                    aeqs_product.sections_inspection_level = "100%inspection";
                    aeqs_product.sections_inspection_method = "normal";
                    aeqs_product.sections_aql_minor = 4.0m;
                    aeqs_product.sections_aql_major = 2.5m;
                    aeqs_product.sections_aql_critical = 1.0m;
                    aeqs_product.sections_max_minor_defects = maxReceptDic["minor"];
                    aeqs_product.sections_max_major_defects = maxReceptDic["major"];
                    aeqs_product.sections_max_critical_defects = maxReceptDic["critical"];
                    aeqs_product.sections_max_major_a_defects = 0m;
                    aeqs_product.sections_max_major_b_defects = 0m;

                    aeqs_package.sections_defects_minor_level = 0m;
                    aeqs_package.sections_defects_major_level = 0m;
                    aeqs_package.sections_defects_critical_level = 0m;

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
                                            sections_defects_minor_level,
                                            sections_defects_major_level,
                                            sections_defects_critical_level
                                            )
                                        values
                                          ('{product_id}',
                                           '{unique_key}',
                                           '{aeqs_product.sections_type}',
                                           '{aeqs_product.sections_title}',
                                           '{aeqs_product.sections_result_id}',
                                           '{aeqs_product.sections_qty_inspected}',
                                           {aeqs_product.sections_sampled_inspected},
                                           {aeqs_product.sections_defective_parts},
                                           '{aeqs_product.sections_inspection_level}',
                                           '{aeqs_product.sections_inspection_method}',
                                           '{aeqs_product.sections_aql_minor}',
                                           '{aeqs_product.sections_aql_major}',
                                           '{aeqs_product.sections_aql_critical}',
                                           '{aeqs_product.sections_barcodes_value}',
                                           '{aeqs_product.sections_qty_type}',
                                           '{aeqs_product.sections_max_minor_defects}',
                                           '{aeqs_product.sections_max_major_defects}',
                                           '{aeqs_product.sections_max_major_a_defects}',
                                           '{aeqs_product.sections_max_major_b_defects}',
                                            '{aeqs_product.sections_max_critical_defects}',
                                            '{aeqs_product.sections_defects_minor_level}',
                                            '{aeqs_product.sections_defects_major_level}',
                                            '{aeqs_product.sections_defects_critical_level}')";

                    #endregion product_sql
                    DB.ExecuteNonQuery(sql);
                }
                #endregion


                #region aeqs_assignment
                Dictionary<string, string> art_dic = GetArtAndShoeName(task_no);
                string art = art_dic.ContainsKey("art") ? art_dic["art"].ToString() : "";
                string art_name = art_dic.ContainsKey("art_name") ? art_dic["art_name"].ToString() : "";

                DataTable sizeTable = GetAssigmentSize(art, po);
                if (sizeTable.Rows.Count > 0)
                {
                    AEQS_TO_P88_ASSIGNMENT aeqs_assignment = new AEQS_TO_P88_ASSIGNMENT();
                    aeqs_assignment.assignment_items_inspection_completed_date = Convert.ToDateTime(date);//检验完成时间
                    TimeSpan timeSpan = aeqs_assignment.assignment_items_inspection_completed_date - aeqs_main.date_started;
                    aeqs_assignment.assignment_items_assignment_date_inspection = aeqs_main.date_started;//检验计划开始时间
                    aeqs_assignment.assignment_items_total_inspection_minutes = Math.Round(Convert.ToDecimal(timeSpan.TotalMinutes));
                    aeqs_assignment.assignment_items_aql_minor = 2.5m;
                    aeqs_assignment.assignment_items_aql_major = 1.5m;
                    aeqs_assignment.assignment_items_aql_critical = 0.01m;
                    aeqs_assignment.assignment_items_aql_major_a = 0m;
                    aeqs_assignment.assignment_items_aql_major_b = 0m;
                    aeqs_assignment.assignment_items_inspection_result_id = GetTotalResult(se_qty);
                    aeqs_assignment.assignment_items_inspection_status_id = (aeqs_assignment.assignment_items_inspection_result_id == 1m) ? 3 : 1;
                    aeqs_assignment.assignment_items_assignment_report_type_id = "35"; 
                    aeqs_assignment.assignment_items_assignment_inspection_level = "100%inspection";
                    aeqs_assignment.assignment_items_assignment_inspection_method = "normal";
                    aeqs_assignment.assignment_items_assignment_inspector_username = GetInspectorName(task_no);
                    aeqs_assignment.assignment_items_po_line_po_exporter_erp_business_id = "011"; //APC Value 011, APE value 779
                    aeqs_assignment.assignment_items_po_line_po_number = po;
                    aeqs_assignment.assignment_items_po_line_po_exporter_id = 233m; //APC Value 233,APE value 219
                    aeqs_assignment.assignment_items_po_line_importer_id = 215m;
                    aeqs_assignment.assignment_items_po_line_importer_erp_business_id = "Adidas001";
                    aeqs_assignment.assignment_items_po_line_importer_project_id = 2062m;
                    aeqs_assignment.assignment_items_po_line_sku_item_name = art_name;

                    string assignment_sql = string.Empty;
                    foreach (DataRow dr in sizeTable.Rows)
                    {
                        int size_se_qty = Convert.ToInt32(dr["se_qty"].ToString());

                        aeqs_assignment.assignment_items_po_line_qty = size_se_qty;
                        aeqs_assignment.assignment_items_qty_to_inspect = size_se_qty;
                        aeqs_assignment.assignment_items_sampling_size = size_se_qty;
                        aeqs_assignment.assignment_items_sampled_inspected = size_se_qty;
                        aeqs_assignment.assignment_items_qty_inspected = size_se_qty;
                        aeqs_assignment.assignment_items_po_line_sku_sku_number = dr["ad_size"].ToString();

                        #region aeqs_assignment_sql
                        assignment_sql += $@"insert into t_aeqs_to_p88_assignment
                                              (union_id,
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
                                               assignment_items_po_line_qty)
                                            values
                                              ('{unique_key}',
                                               '{aeqs_assignment.assignment_items_sampled_inspected}',
                                               '{aeqs_assignment.assignment_items_inspection_result_id}',
                                               '{aeqs_assignment.assignment_items_inspection_status_id}',
                                               '{aeqs_assignment.assignment_items_qty_inspected}',
                                               to_date('{aeqs_assignment.assignment_items_inspection_completed_date}','yyyy-mm-dd hh24:mi:ss'),
                                               '{aeqs_assignment.assignment_items_total_inspection_minutes}',
                                               '{aeqs_assignment.assignment_items_sampling_size}',
                                               '{aeqs_assignment.assignment_items_qty_to_inspect}',
                                               '{aeqs_assignment.assignment_items_aql_minor}',
                                               '{aeqs_assignment.assignment_items_aql_major}',
                                               '{aeqs_assignment.assignment_items_aql_major_a}',
                                               '{aeqs_assignment.assignment_items_aql_major_b}',
                                               '{aeqs_assignment.assignment_items_aql_critical}',
                                               '{aeqs_assignment.assignment_items_supplier_booking_msg}',
                                               '{aeqs_assignment.assignment_items_conclusion_remarks}',
                                               '{aeqs_assignment.assignment_items_assignment_inspector_username}',
                                               to_date('{aeqs_assignment.assignment_items_assignment_date_inspection}','yyyy-mm-dd hh24:mi:ss'),
                                               '{aeqs_assignment.assignment_items_assignment_inspection_level}',
                                               '{aeqs_assignment.assignment_items_assignment_inspection_method}',
                                               '{aeqs_assignment.assignment_items_po_line_po_exporter_id}',
                                               '{aeqs_assignment.assignment_items_po_line_po_exporter_erp_business_id}',
                                               '{aeqs_assignment.assignment_items_po_line_po_number}',
                                               '{aeqs_assignment.assignment_items_po_line_importer_id}',
                                               '{aeqs_assignment.assignment_items_po_line_importer_erp_business_id}',
                                               '{aeqs_assignment.assignment_items_po_line_importer_project_id}',
                                               '{aeqs_assignment.assignment_items_po_line_sku_sku_number}',
                                               '{aeqs_assignment.assignment_items_po_line_sku_item_name}',
                                               '{aeqs_assignment.assignment_items_assignment_report_type_id}',
                                               '{aeqs_assignment.assignment_items_po_line_qty}');";
                        #endregion

                    }
                    if (!string.IsNullOrEmpty(assignment_sql))
                    {
                        DB.ExecuteNonQuery("begin " + assignment_sql + " end;");
                    }
                }
                #endregion


                #region aeqs_passfail
                string passfail_sql = string.Empty;

                AEQS_TO_P88_PASSFAIL aEQS_TO_P88_PASSFAIL = new AEQS_TO_P88_PASSFAIL();
                aEQS_TO_P88_PASSFAIL.passfails_title = "mcs_availability_signature_compliance";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_general_compliance";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "grading_sheet_availability_signature_compliance";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_general_compliance";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "finished_uppers_shoes_pass_through_md";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_metal_detection_compliance";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "calibration_with_test_stick_approved_supplier";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_metal_detection_compliance";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "uv_c_treatment";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "3_mold_prevention";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "anti_mold_wrapping_paper";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "3_mold_prevention";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "line_stoppage_due_to_quality_incident";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "validation";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "4_exceptional_management";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'no', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "slip_on_inspection_pass_step_in_tool";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "checklist";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "1_fit";
                string passfail_value = "yes";
                string passfail_status = "pass";
                int count = DB.GetInt32("select count(*) from qcm_dqa_mag_d m left join bdm_rd_prod p on m.shoes_code = p.shoe_no where p.prod_no = '" + art + "' and  m.QA_RISK_DETAILS_DESC = '入脚-袜套结构'");
                if (count <= 0)
                {
                    passfail_value = "N/A";
                    passfail_status = "na";
                }
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', '" + passfail_value + "', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', '" + passfail_status + "');";

                aEQS_TO_P88_PASSFAIL.passfails_title = "moisture_test_aquaboy_pass";
                aEQS_TO_P88_PASSFAIL.passfails_subsection = "checklist";
                aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection = "2_mold_prevention";
                passfail_sql += "insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)\r\n                                            values ('" + unique_key + "', '" + aEQS_TO_P88_PASSFAIL.passfails_title + "', 'yes', 'check-list', '" + aEQS_TO_P88_PASSFAIL.passfails_subsection + "', '" + aEQS_TO_P88_PASSFAIL.passfails_checklistsubsection + "', 'pass');";

                DB.ExecuteNonQuery("begin " + passfail_sql + " end;");

                #endregion

                //The total number of problem points that failed for the first time
                int GetDefective_parts(string taskNo)
                {
                    string commits_sql = $@"select count(COMMIT_INDEX) as commits
	                                            from tqc_task_detail_t d
	                                            inner join tqc_task_item_c c
	                                            on d.union_id = c.id
	                                            left join bdm_insp_fin_shoes_m m
	                                            on c.inspection_code = m.inspection_code
	                                            left join bdm_aql_bad_classify_d s
	                                            on m.bad_item_code = s.bad_item_code
	                                            left join bdm_aql_bad_classify y
	                                            on s.bad_classify_code = y.bad_classify_code
	                                            where d.task_no = '{taskNo}'
	                                            and m.bad_item_code is not null
	                                            and m.bad_item_name is not null
	                                            and m.qc_type = 1 --TQC
	                                            and d.commit_type = 1";
                    return DB.GetInt32(commits_sql);
                }

                //Maximum number allowed Sampling based on batch quantity
                Dictionary<string, int> GetMaxReceptNumDic(string num)
                {
                    Dictionary<string, int> dic = new Dictionary<string, int>();
                    string acSql = $@"select AC13, AC12, AC01, vals
                                          from (select AC13, AC12, AC01, vals
                                                  from BDM_AQL_M
                                                 where HORI_TYPE = '2'
                                                   and LEVEL_TYPE = '2'
                                                   and vals <= CAST('{num}' AS int)
                                                 order by CAST(vals AS int) desc)
                                         where rownum = 1";
                    DataTable dataTable2 = DB.GetDataTable(acSql);
                    if (dataTable2.Rows.Count > 0)
                    {
                        dic.Add("minor", Convert.ToInt32(dataTable2.Rows[0]["AC13"].ToString()));
                        dic.Add("major", Convert.ToInt32(dataTable2.Rows[0]["AC12"].ToString()));
                        dic.Add("critical", Convert.ToInt32(dataTable2.Rows[0]["AC01"].ToString()));
                    }
                    return dic;
                }

                //Total inspection result: total number of problem points (problem points related to p88) / inspected even number * 100% <= 15%. When 100% <= 15%, pass "1 = Pass ", otherwise pass " 2 = Fail "
                int GetTotalResult(string seQty)
                {
                    int defect_parts = GetDefective_parts(task_no);

                    if (Convert.ToDouble(defect_parts) / Convert.ToDouble(seQty) * 100 <= 5)
                    {
                        return 1;//Pass
                    }
                    else
                    {
                        return 2;//fail
                    }
                }

                //Get all configured defective items of the product
                DataTable GetProductDefectitem(string taskNo)
                {
                    string productSql = $@"with bad_item_table as(
			                                select listagg(d.id,',') as detail_id, c.inspection_name,count(union_id) as badnum ,m.bad_item_code,m.bad_item_name,s.bad_classify_code,y.bad_classify_name,m.problem_level
			                                from tqc_task_detail_t d 
			                                inner join tqc_task_item_c c on d.union_id = c.id 
			                                left join bdm_insp_fin_shoes_m m on c.inspection_code = m.inspection_code 
			                                left join bdm_aql_bad_classify_d s on m.bad_item_code = s.bad_item_code 
			                                left join bdm_aql_bad_classify y on s.bad_classify_code = y.bad_classify_code
			                                where d.task_no in ('{taskNo}')
			                                and m.bad_item_code is not null 
			                                and m.bad_item_name is not null
                                            and d.commit_type = '1'
			                                and m.qc_type = 1
			                                group by c.inspection_code,c.inspection_name, m.problem_level,m.bad_item_code,m.bad_item_name,s.bad_classify_code,m.problem_level,y.bad_classify_name
		                                )
		                                select max(detail_id) as detail_id, listagg( distinct inspection_name,',') as inspection_name,bad_item_code,bad_item_name,bad_classify_code,bad_classify_name,problem_level,sum(badnum) as badnum from bad_item_table
		                                group by bad_item_code,bad_item_name,bad_classify_code,bad_classify_name,problem_level";

                    return DB.GetDataTable(productSql);
                }

                //Get the ART and shoe model name of the assignment part
                Dictionary<string, string> GetArtAndShoeName(string taksNo)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    string art_sql = $@"select m.prod_no,s.name_s,p.name_s as name_s2
                                        from tqc_task_m m
                                        left join bdm_rd_prod p on m.prod_no = p.prod_no
                                        left join bdm_rd_style s on p.shoe_no = s.shoe_no
                                        where m.task_no = '{taksNo}'";

                    DataTable dt = DB.GetDataTable(art_sql);
                    if (dt.Rows.Count > 0)
                    {
                        dic.Add("art", dt.Rows[0]["prod_no"].ToString());
                        if (!string.IsNullOrEmpty(dt.Rows[0]["name_s"].ToString()))
                        {
                            dic.Add("art_name", dt.Rows[0]["name_s"].ToString());
                        }
                        else
                        {
                            dic.Add("art_name", dt.Rows[0]["name_s2"].ToString());
                        }
                    }
                    return dic;
                }

                //Get inspector name
                string GetInspectorName(string taskNo)
                {
                    List<string> list = DB.GetString($"select production_line_code from tqc_task_m  where task_no = '{taskNo}'").Split(',').ToList();
                    string inspectorName = string.Empty;
                    foreach (string item in list)
                    {
                        inspectorName = DB.GetString($@"select p88_checker from t_aeqs_to_p88_inspector where checker_code = '{item}'");
                        if (!string.IsNullOrEmpty(inspectorName))
                        {
                            break;
                        }
                    }
                    return inspectorName;
                }

                //Get all code numbers inspected in the assignment part
                DataTable GetAssigmentSize(string _art, string _po)
                {
                    #region before PO change 2 on 20250218

                    //string sizeSql = $@"select max(concat('{_art}_', b.ad_size)) as ad_size,
                    //                           sum(a.se_qty) as se_qty
                    //                      from bdm_se_order_size a
                    //                      left join base097m b
                    //                        on a.size_no = b.FACTORY_SIZE
                    //                     where se_id in
                    //                           (select se_id from bdm_se_order_master where mer_po = '{_po}')
                    //                       and a.se_qty > 0
                    //                     group by ad_size, a.se_qty";
                    //return DB.GetDataTable(sizeSql);
                    #endregion


                    #region After PO change 2 on 20250218
                    //码数部分
                    DataTable dataTable = null;
                    //If it is a merge order, then follow the merge logic
                    string merge_mark = DB.GetString($@"select so_mergr_mark from bdm_se_order_master where mer_po = '{po}'");
                    if (merge_mark.Equals("Y"))
                    {
                        string merge_sql = $@"select max('{_art}_' || b.ad_size) as ad_size,
                                                       a.po_line_item as po_seq,
                                                       max(a.po_line_qty)as se_qty
                                                from t_bdm_se_order_reference a
                                                left join base097m b on a.po_size = b.factory_size
                                                where a.se_id in
                                                     (select se_id from bdm_se_order_master where mer_po = '{po}')
                                                 and a.PO_LINE_QTY > 0
                                                group by ad_size, a.po_line_item";
                        dataTable = DB.GetDataTable(merge_sql);
                    }
                    else
                    {
                        string unmerge_sql = $@"select max('{_art}_' || b.ad_size) as ad_size,
                                               sum(a.se_qty) as se_qty,
                                               max(po_seq) as po_seq
                                          from bdm_se_order_size a
                                          left join base097m b on a.size_no = b.FACTORY_SIZE
                                         where se_id in
                                               (select se_id from bdm_se_order_master where mer_po = '{po}')
                                           and a.se_qty > 0
                                         group by ad_size, a.se_qty";
                        dataTable = DB.GetDataTable(unmerge_sql);
                    }
                    return dataTable;
                    #endregion

                }

                DB.Commit();
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.ErrMsg = ex.Message;
                result.IsSuccess = false;
            }

            return result;
        }




        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertAndFetchData(object OBJ)
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

                string prod_date = jarr.ContainsKey("prod_date") ? jarr["prod_date"].ToString() : "";
                string prod_line = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";
                int inspection_qty = jarr.ContainsKey("inspection_qty") ? Convert.ToInt32(jarr["inspection_qty"]) : 0;
                int total_pass_qty = jarr.ContainsKey("total_pass_qty") ? Convert.ToInt32(jarr["total_pass_qty"]) : 0;
                double rft = jarr.ContainsKey("rft") ? Convert.ToDouble(jarr["rft"]) : 0.0;
                string rft_type = jarr.ContainsKey("rft_type") ? jarr["rft_type"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                string sql = string.Empty;
                DataTable dt = new DataTable();
                if(rft_type=="TQC")
                {
                    string Prod_Line = DB.GetString($@"select m.department_code from base005m m where m.udf01 in ('S','L') and department_code='{prod_line}'");
                    if(!string.IsNullOrEmpty(Prod_Line))
                    {
                        sql = $@"INSERT INTO TQC_MANUAL_RFT (PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT, CREATEBY, CREATEDATE, CREATETIME)
                    VALUES (TO_DATE('{prod_date}', 'yyyy-MM-dd'), '{prod_line}', '{inspection_qty}', '{total_pass_qty}', '{rft.ToString("0.00")}', '{user}', '{date}', '{time}')";

                        DB.ExecuteNonQuery(sql);

                        dt = DB.GetDataTable($@"SELECT PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT FROM TQC_MANUAL_RFT ORDER BY PROD_DATE DESC");

                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Incorrect production Line";
                    }

                    
                } 
                else if (rft_type == "RQC")
                {
                    string Prod_Line = DB.GetString($@"select m.department_code from base005m m where m.udf01 in ('C','S') and department_code='{prod_line}'");
                    if (!string.IsNullOrEmpty(Prod_Line))
                    {
                        sql = $@"INSERT INTO RQC_MANUAL_RFT (PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT, CREATEBY, CREATEDATE, CREATETIME)
                          VALUES (TO_DATE('{prod_date}', 'yyyy-MM-dd'), '{prod_line}', '{inspection_qty}', '{total_pass_qty}', '{rft.ToString("0.00")}', '{user}', '{date}', '{time}')";

                        DB.ExecuteNonQuery(sql);

                        dt = DB.GetDataTable($@"SELECT PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT FROM RQC_MANUAL_RFT ORDER BY PROD_DATE DESC");

                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Incorrect production Line";
                    }
                }

               
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Fetch_RFT(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DataTable dt = new DataTable();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string prodDate = jarr.ContainsKey("prod_date") && jarr["prod_date"] != null
                                    ? jarr["prod_date"].ToString()
                                    : DateTime.Now.ToString("yyyy-MM-dd");
                string RFT_Type = jarr.ContainsKey("RFT_Type") && jarr["RFT_Type"] != null
                                 ? jarr["RFT_Type"].ToString()
                                 : "";
                if (RFT_Type == "TQC")
                {
                    string sql = $@"SELECT PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT,CREATEDATE
                                FROM TQC_MANUAL_RFT 
                                WHERE TRUNC(PROD_DATE) = TO_DATE(:prodDate, 'YYYY-MM-DD')
                                ORDER BY PROD_DATE DESC";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"prodDate", prodDate }
                };

                      dt = DB.GetDataTable(sql, parameters);
                }
                else if (RFT_Type == "RQC")
                {

                    string sql = $@"SELECT PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT,CREATEDATE
                                FROM RQC_MANUAL_RFT 
                                WHERE TRUNC(PROD_DATE) = TO_DATE(:prodDate, 'YYYY-MM-DD')
                                ORDER BY PROD_DATE DESC";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"prodDate", prodDate }
                };

                     dt = DB.GetDataTable(sql, parameters);
                }
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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



        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Fetch_RFT_Data(object OBJ)
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
                DataTable dt = new DataTable();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string fromDate = jarr.ContainsKey("FROM_DATE") ? jarr["FROM_DATE"]?.ToString() : null;
                string toDate = jarr.ContainsKey("TO_DATE") ? jarr["TO_DATE"]?.ToString() : null;
                string prodLine = jarr.ContainsKey("PROD_LINE") ? jarr["PROD_LINE"]?.ToString() : null;
                string RFT_type = jarr.ContainsKey("RFT_type") ? jarr["RFT_type"]?.ToString() : null;

                if (string.IsNullOrEmpty(fromDate) || string.IsNullOrEmpty(toDate))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Both FROM_DATE and TO_DATE are required.";
                    return ret;
                }

                if (RFT_type == "TQC")
                {
                    string sql = @"SELECT TO_CHAR(PROD_DATE, 'YYYY-MM-DD') AS PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT 
                       FROM TQC_MANUAL_RFT 
                       WHERE PROD_DATE BETWEEN TO_DATE(:FROM_DATE, 'YYYY-MM-DD') AND TO_DATE(:TO_DATE, 'YYYY-MM-DD')";


                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "FROM_DATE", fromDate },
                    { "TO_DATE", toDate }
                };


                    if (!string.IsNullOrEmpty(prodLine))
                    {
                        sql += " AND PROD_LINE = :PROD_LINE";
                        parameters.Add("PROD_LINE", prodLine);
                    }

                    sql += " ORDER BY PROD_DATE ";

                    dt = DB.GetDataTable(sql, parameters);
                }
                else if (RFT_type == "RQC")
                {
                    string sql = @"SELECT TO_CHAR(PROD_DATE, 'YYYY-MM-DD') AS PROD_DATE, PROD_LINE, INSPECTION_QTY, TOTAL_PASS_QTY, RFT 
                       FROM RQC_MANUAL_RFT 
                       WHERE PROD_DATE BETWEEN TO_DATE(:FROM_DATE, 'YYYY-MM-DD') AND TO_DATE(:TO_DATE, 'YYYY-MM-DD')";


                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "FROM_DATE", fromDate },
                    { "TO_DATE", toDate }
                };


                    if (!string.IsNullOrEmpty(prodLine))
                    {
                        sql += " AND PROD_LINE = :PROD_LINE";
                        parameters.Add("PROD_LINE", prodLine);
                    }

                    sql += " ORDER BY PROD_DATE ";

                     dt = DB.GetDataTable(sql, parameters);
                }
                // Return results
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_RFT_Data_ForEdit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();

                var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                if (!requestData.ContainsKey("prod_date") || !requestData.ContainsKey("prod_line"))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Missing Prod Date or Prod Line.";
                    return ret;
                }

                string prodDate = requestData["prod_date"].ToString();
                string prodLine = requestData["prod_line"].ToString();
                string rft_type = requestData["rft_type"].ToString();
                string sql = string.Empty;
                DataTable dt = new DataTable();
                if(rft_type=="TQC")
                {
                    sql = @"SELECT INSPECTION_QTY, TOTAL_PASS_QTY, RFT 
                                FROM TQC_MANUAL_RFT 
                                WHERE TRUNC(PROD_DATE) = TO_DATE(:prodDate, 'YYYY-MM-DD')
                                AND PROD_LINE = :prodLine";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "prodDate", prodDate },
                    { "prodLine", prodLine }
                };

                   dt = DB.GetDataTable(sql, parameters);
                }
                else if(rft_type=="RQC")
                {
                    sql = @"SELECT INSPECTION_QTY, TOTAL_PASS_QTY, RFT 
                                FROM RQC_MANUAL_RFT 
                                WHERE TRUNC(PROD_DATE) = TO_DATE(:prodDate, 'YYYY-MM-DD')
                                AND PROD_LINE = :prodLine";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "prodDate", prodDate },
                    { "prodLine", prodLine }
                };

                    dt = DB.GetDataTable(sql, parameters);
                }
               

                if (dt.Rows.Count > 0)
                {
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for the given Prod Date and Prod Line.";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "Error fetching data: " + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }



        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Prod_RFT(object OBJ)
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

                string prodDate = jarr.ContainsKey("prod_date") ? jarr["prod_date"].ToString() : "";
                string prodLine = jarr.ContainsKey("prod_line") ? jarr["prod_line"].ToString() : "";
                string rft_type = jarr.ContainsKey("rft_type") ? jarr["rft_type"].ToString() : "";
                int inspectionQty = jarr.ContainsKey("inspection_qty") ? Convert.ToInt32(jarr["inspection_qty"]) : 0;
                int totalPassQty = jarr.ContainsKey("total_pass_qty") ? Convert.ToInt32(jarr["total_pass_qty"]) : 0;
                double rft = jarr.ContainsKey("rft") ? Convert.ToDouble(jarr["rft"]) : 0.0;

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = string.Empty;
                if (rft_type=="TQC")
                {
                    string select_query = $@"select prod_date, prod_line, inspection_qty, total_pass_qty, rft
  from tqc_manual_rft a
 where to_char(a.prod_date, 'yyyy-MM-dd') = '{prodDate}'
   and a.prod_line = '{prodLine}'";

                    DataTable dt = DB.GetDataTable(select_query);

                    string insert_query = $@"insert into tqc_manual_rft_log
  (prod_date,
   prod_line,
   inspection_qty,
   total_pass_qty,
   rft,
   createdby,
   log_type)
values
  (to_date('{dt.Rows[0]["PROD_DATE"]}', 'YYYY/MM/DD HH24:MI:SS'),
  '{dt.Rows[0]["PROD_LINE"]}', 
  '{dt.Rows[0]["INSPECTION_QTY"]}',
  '{dt.Rows[0]["TOTAL_PASS_QTY"]}', 
  '{dt.Rows[0]["RFT"]}', 
  '{user}', 
   'Update' 
   )";
                    int i = DB.ExecuteNonQuery(insert_query);

                    if (i > 0)
                    {
                        sql = $@"
                    UPDATE TQC_MANUAL_RFT
                    SET INSPECTION_QTY = '{inspectionQty}',
                        TOTAL_PASS_QTY = '{totalPassQty}',
                        RFT = '{rft.ToString("0.00")}',
                        MODIFYBY = '{user}',
                        MODIFYDATE = '{date}',
                        MODIFYTIME = '{time}'
                    WHERE PROD_DATE = TO_DATE('{prodDate}', 'YYYY-MM-DD')
                      AND PROD_LINE = '{prodLine}'";
                        DB.ExecuteNonQuery(sql);
                    }

                    
                }
                else if(rft_type=="RQC")
                {
                    string select_query = $@"select prod_date, prod_line, inspection_qty, total_pass_qty, rft
  from rqc_manual_rft a
 where to_char(a.prod_date, 'yyyy-MM-dd') = '{prodDate}'
   and a.prod_line = '{prodLine}'";

                    DataTable dt = DB.GetDataTable(select_query);

                    string insert_query = $@"insert into rqc_manual_rft_log
  (prod_date,
   prod_line,
   inspection_qty,
   total_pass_qty,
   rft,
   createdby,
   log_type)
values
  (to_date('{dt.Rows[0]["PROD_DATE"]}', 'YYYY/MM/DD HH24:MI:SS'),
  '{dt.Rows[0]["PROD_LINE"]}', 
  '{dt.Rows[0]["INSPECTION_QTY"]}',
  '{dt.Rows[0]["TOTAL_PASS_QTY"]}', 
  '{dt.Rows[0]["RFT"]}', 
  '{user}', 
   'Update' 
   )";
                    int i = DB.ExecuteNonQuery(insert_query);

                    if (i > 0)
                    {
                        sql = $@"
                    UPDATE RQC_MANUAL_RFT
                    SET INSPECTION_QTY = '{inspectionQty}',
                        TOTAL_PASS_QTY = '{totalPassQty}',
                        RFT = '{rft.ToString("0.00")}',
                        MODIFYBY = '{user}',
                        MODIFYDATE = '{date}',
                        MODIFYTIME = '{time}'
                    WHERE PROD_DATE = TO_DATE('{prodDate}', 'YYYY-MM-DD')
                      AND PROD_LINE = '{prodLine}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }

               
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Updated Successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Database error: " + ex.Message;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Fetch_PO_Qty(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                //DB.BeginTransaction();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string customerPO = jarr.ContainsKey("CUSTOMER_PO") ? jarr["CUSTOMER_PO"].ToString() : "";

                if (string.IsNullOrWhiteSpace(customerPO))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Customer PO cannor be empty.";
                    return ret;
                }
                string po = customerPO.Split('&')[0];
                string sql = $@" SELECT round(SSS.SE_QTY,0) PO_Quantity
                                FROM BDM_SE_ORDER_MASTER SS
                                INNER JOIN BDM_SE_ORDER_ITEM SSS
                                    ON SS.SE_ID = SSS.SE_ID
                                INNER JOIN BDM_RD_PROD D
                                    ON SSS.PROD_NO = D.PROD_NO
                                WHERE SS.CUSTOMER_PO = :CUSTOMER_PO or ss.mer_po=:CUSTOMER_PO
                                GROUP BY SS.SE_ID, SS.CUSTOMER_PO, SSS.PROD_NO, D.NAME_T, SSS.SE_QTY";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"CUSTOMER_PO", po }
                };

                DataTable dt = DB.GetDataTable(sql, parameters);
                if (dt.Rows.Count > 0)
                {
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for Customer PO: " + customerPO;
                }

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Databae error: " + ex.Message;
            }

            finally
            {
                DB.Close();
            }
            return ret;

        }



        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Fetch_PO_Country(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                //DB.BeginTransaction();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string customerPO = jarr.ContainsKey("CUSTOMER_PO") ? jarr["CUSTOMER_PO"].ToString() : "";

                if (string.IsNullOrWhiteSpace(customerPO))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Customer PO cannor be empty.";
                    return ret;
                }
                string po = customerPO.Split('&')[0];

                string sql = $@" select b.c_name
                                from bdm_se_order_master a
                                    inner join bdm_country b
                                    on a.descountry_code = b.c_no
                                and b.l_no = 'EN'
                                where a.customer_po= :CUSTOMER_PO or  a.mer_po=:CUSTOMER_PO";

                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"CUSTOMER_PO", po }
                };

                DataTable dt = DB.GetDataTable(sql, parameters);
                if (dt.Rows.Count > 0)
                {
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for Customer PO: " + customerPO;
                }

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Databae error: " + ex.Message;
            }

            finally
            {
                DB.Close();
            }
            return ret;

        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Prod_line_For_Manual_RFT(object OBJ)
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
                string RFT_Type = jarr.ContainsKey("RFT_Type") ? jarr["RFT_Type"].ToString() : "";
                string sql = string.Empty;
                if (RFT_Type=="RQC")
                {
                    sql = $@"select m.department_code from base005m m where m.udf01 in ('C','S')";
                }
                else if(RFT_Type=="TQC")
                {
                    sql = $@"select m.department_code from base005m m where m.udf01 in ('S','L')";
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


        //Adding New method here for inserting the Bgrade Reason by Srinath N
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Bgrade_Reason(object OBJ)
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
                string PO = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";//采购订单
                string ART = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";//艺术
                string PRODLINE = jarr.ContainsKey("Prodline") ? jarr["Prodline"].ToString() : "";//产品线
                string TASK_NO = jarr.ContainsKey("Task_No") ? jarr["Task_No"].ToString() : "";//任务号
                string BGRADE_REASON = jarr.ContainsKey("Bgrade_Reason") ? jarr["Bgrade_Reason"].ToString() : "";//不良原因
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = $@"insert into TQC_BGRADE_REASON(PRODDATE,PRODLINE,PO_NUMBER,TASK_NO,ART_NUMBER,BGRADE_REASON,CREATED_BY,CREATED_AT) 
                         values(TO_DATE('{date}', 'YYYY-MM-DD'),'{PRODLINE}','{PO}','{TASK_NO}','{ART}','{BGRADE_REASON}','{user}',TO_DATE('{time}', 'HH24:MI:SS'))";
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

        // Adding Few Bgrade Reasons from database to bind in frontend combobox when loading the data 
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bgrade_Reasons(object OBJ)
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
                string sql = $@"select BGRADE_REASON from TQC_TASK_EDIT_BGRADE_REASONS";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PlantLoad(object OBJ)
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
                string sql = $@"SELECT DISTINCT UDF05  
                           FROM Base005m
                           Where FACTORY_SAP='5001'";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Prodlineload(object OBJ)
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
                string sql = $@"SELECT DISTINCT UDF05  
                           FROM Base005m
                           Where FACTORY_SAP='5001'";
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
        //To get Org for Bgrade reason view
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Orgload(object OBJ)
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
                string sql = $@"
           SELECT DISTINCT FACTORY_SAP
           FROM Base005m";
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
        // to get plant details to bgrade view details
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PlantViewLoad(object OBJ)
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
                string Org = jarr.ContainsKey("Org") ? jarr["Org"].ToString() : "";
                string sql = $@"SELECT DISTINCT UDF05
                                FROM Base005m
                                WHERE FACTORY_SAP = '{Org}'";
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
        // to get Prodline details to bgrade view details
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ProdlineViewload(object OBJ)
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
                string Org = jarr.ContainsKey("Org") ? jarr["Org"].ToString() : "";
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Org))
                {
                    if (Org == "5001")
                    {
                        where += $@" AND UDF01 = 'L'";
                    }

                }
                string sql = $@"SELECT Distinct DEPARTMENT_CODE  
                           FROM Base005m
                           Where FACTORY_SAP='{Org}' AND UDF05='{Plant}'{where}";
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
        //To get Po details to bgrade view data
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPoData(object OBJ)
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
                string sql = $@"SELECT SS.CUSTOMER_PO
  FROM BDM_SE_ORDER_MASTER SS
 INNER JOIN BDM_SE_ORDER_ITEM SSS
    ON SS.SE_ID = SSS.SE_ID
 INNER JOIN BDM_RD_PROD D
    ON SSS.PROD_NO = D.PROD_NO";
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
        // to view count based on all filter conditions
       
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_BgradeCount(object OBJ)
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
                // string Date = jarr.ContainsKey("Date") ? jarr["Date"].ToString() : ""; 
                string FromDate = jarr.ContainsKey("FromDate") ? jarr["FromDate"].ToString() : ""; //premika 2025/11/18
                string ToDate = jarr.ContainsKey("ToDate") ? jarr["ToDate"].ToString() : "";  //premika 2025/11/18
                string Org = jarr.ContainsKey("Org") ? jarr["Org"].ToString() : "";
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string Line = jarr.ContainsKey("Line") ? jarr["Line"].ToString() : "";
                string Po = jarr.ContainsKey("Po") ? jarr["Po"].ToString() : "";
                string Bgrade_Reason = jarr.ContainsKey("Bgrade_Reason") ? jarr["Bgrade_Reason"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Org))
                {
                    where += $@" AND PRODLINE LIKE '%{Org}%'";
                }

                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@" AND PRODLINE LIKE '____{Plant}%'";
                }
                if (!string.IsNullOrEmpty(Line))
                {
                    where += $@" AND PRODLINE = '{Line}'";
                }
                if (!string.IsNullOrEmpty(Po))
                {
                    where += $@" AND PO_NUMBER = '{Po}'";
                }
                if (!string.IsNullOrEmpty(Bgrade_Reason))
                {
                    where += $@" AND BGRADE_REASON = '{Bgrade_Reason}'";
                }
                //premika--2025/11/19 start
                string sql = $@"SELECT 
    PRODDATE,
    PRODLINE,
    PO_NUMBER,
    TASK_NO,
    BGRADE_REASON,
    COUNT(*) AS Quantity
FROM tqc_bgrade_reason
 WHERE PRODDATE BETWEEN 
      TO_DATE('{FromDate}', 'yyyy/mm/dd') 
  AND TO_DATE('{ToDate}', 'yyyy/mm/dd') {where}
 group by PRODDATE,
    PRODLINE,
    PO_NUMBER,
    TASK_NO,
    BGRADE_REASON";
                //premika--2025/11/19 end
                //                string sql = $@"SELECT 
                //    PRODDATE,
                //    PRODLINE,
                //    PO_NUMBER,
                //    TASK_NO,
                //    BGRADE_REASON,
                //    COUNT(*) AS Quantity
                //FROM tqc_bgrade_reason
                // WHERE TO_CHAR(PRODDATE, 'yyyy/MM/dd') = '{Date}' {where}
                // group by PRODDATE,
                //    PRODLINE,
                //    PO_NUMBER,
                //    TASK_NO,
                //    BGRADE_REASON";

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

        // To view count and reason for Bgrade based on Task NO
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TQC_Bgrade_View(object OBJ)
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
                string TASK_NO = jarr.ContainsKey("Task_No") ? jarr["Task_No"].ToString() : "";//任务号
                string sql = $@"Select BGRADE_REASON, count(*) as Quantity
  from tqc_bgrade_reason a
 where a.task_no = '{TASK_NO}'
 group by BGRADE_REASON";

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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete_RFT_Data(object OBJ)
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
                var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prodDate = requestData["prod_date"].ToString();
                string prodLine = requestData["prod_line"].ToString();
                string rft_type = requestData["rft_type"].ToString();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                DataTable dt = new DataTable();
                if (rft_type == "TQC")
                {
                    string select_query = $@"select prod_date, prod_line, inspection_qty, total_pass_qty, rft
  from tqc_manual_rft a
 where to_char(a.prod_date, 'yyyy/MM/dd') = '{prodDate}'
   and a.prod_line = '{prodLine}'";

                    DataTable dt1 = DB.GetDataTable(select_query);

                    string insert_query = $@"insert into tqc_manual_rft_log
  (prod_date,
   prod_line,
   inspection_qty,
   total_pass_qty,
   rft,
   createdby,
   log_type)
values
  (to_date('{dt1.Rows[0]["PROD_DATE"]}', 'YYYY/MM/DD HH24:MI:SS'),
  '{dt1.Rows[0]["PROD_LINE"]}', 
  '{dt1.Rows[0]["INSPECTION_QTY"]}',
  '{dt1.Rows[0]["TOTAL_PASS_QTY"]}', 
  '{dt1.Rows[0]["RFT"]}', 
  '{user}', 
   'Delete' 
   )";
                    int i = DB.ExecuteNonQuery(insert_query);

                    if (i > 0)
                    {
                        sql = $@"delete from TQC_MANUAL_RFT a where to_char(a.prod_date,'yyyy/MM/dd')='{prodDate}'
                                AND PROD_LINE = '{prodLine}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                else if (rft_type == "RQC")
                {
                    string select_query = $@"select prod_date, prod_line, inspection_qty, total_pass_qty, rft
  from rqc_manual_rft a
 where to_char(a.prod_date, 'yyyy/MM/dd') = '{prodDate}'
   and a.prod_line = '{prodLine}'";

                    DataTable dt1 = DB.GetDataTable(select_query);

                    string insert_query = $@"insert into rqc_manual_rft_log
  (prod_date,
   prod_line,
   inspection_qty,
   total_pass_qty,
   rft,
   createdby,
   log_type)
values
  (to_date('{dt1.Rows[0]["PROD_DATE"]}', 'YYYY/MM/DD HH24:MI:SS'),
  '{dt1.Rows[0]["PROD_LINE"]}', 
  '{dt1.Rows[0]["INSPECTION_QTY"]}',
  '{dt1.Rows[0]["TOTAL_PASS_QTY"]}', 
  '{dt1.Rows[0]["RFT"]}', 
  '{user}', 
   'Delete' 
   )";
                    int i = DB.ExecuteNonQuery(insert_query);

                    if (i > 0)
                    {
                        sql = $@"delete from RQC_MANUAL_RFT a where to_char(a.prod_date,'yyyy/MM/dd')='{prodDate}'
                                AND PROD_LINE = '{prodLine}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                DB.Commit();
                ret.IsSuccess = true;
                
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Error fetching data: " + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

    }

    class PoEntity
    {
        public string mer_po;
        public string task_no;
        public string workshop_section_no;
        public int defect_nums;
    }

}

