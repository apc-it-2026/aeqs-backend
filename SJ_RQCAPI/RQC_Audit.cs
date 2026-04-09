using SJ_BASEAPI.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_RQCAPI
{
    public class RQC_Audit
    {
        /// <summary>
        /// rqc工厂审核主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        /// To Test TFS source code
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Main(object OBJ)
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
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 结束时间
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string state = jarr.ContainsKey("state") ? jarr["state"].ToString() : "";//查询条件 审核状态0:待改善;1:已结案
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(start_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@" and (to_date(m.createdate,'yyyy-mm-dd,hh24:mi:ss') between to_date('{start_date}','yyyy-mm-dd,hh24:mi:ss') and to_date('{end_date}','yyyy-mm-dd,hh24:mi:ss'))";
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (t.audit_type_name like '%{keyword}%' or m.task_no like '%{keyword}%' or b.suppliers_name like '%{keyword}%' or t.audit_type_name like '{keyword}')";
                }
                string sql = string.Empty;
                sql = $@"SELECT
	                        TASK_NO
                        FROM
	                        qcm_audit_task_d 
                        GROUP BY TASK_NO 
                        HAVING COUNT(1)=COUNT(CASE IS_END WHEN '1' THEN '1' ELSE NULL END) ";
                if (state == "0")
                {
                    where += $@" and m.task_no not in ({sql})";
                }
                else if (state == "1")
                {
                    where += $@" and m.task_no in ({sql})";
                }
                sql = $@"SELECT
                        m.task_no,
                        m.createdate,
                        m.suppliers_code,
                        b.suppliers_name,
                        m.audit_type_code,
                        t.AUDIT_TYPE_F_NAME as audit_type_name,
                        '' as item_total,
                        '' as audit_score,
                        '' as stay_perfect_count,
                        '' as audit_state,
                        m.createby
                        FROM
                        qcm_audit_task_m m
                        LEFT JOIN base003m b on m.SUPPLIERS_CODE=b.SUPPLIERS_CODE
                        LEFT JOIN BDM_AUDIT_TYPE_F_M t on m.audit_type_code=t.AUDIT_TYPE_F_CODE
                        where 1=1 {where} order by m.id desc
                        ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                foreach (DataRow item in dt.Rows)
                {
                    string item_total = DB.GetString($@"select sum(score_limit) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["item_total"] = item_total == "" ? "0" : item_total;
                    string audit_score = DB.GetString($@"select sum(score) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["audit_score"] = audit_score == "" ? "0" : audit_score;
                    string stay_perfect_count = DB.GetString($@"select count(1) from qcm_audit_task_d where is_end='0' and task_no='{item["task_no"]}'");
                    item["stay_perfect_count"] = stay_perfect_count;
                    DataTable task_d_dt = DB.GetDataTable($@"select is_end from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["audit_state"] = "Closed";
                    foreach (DataRow item1 in task_d_dt.Rows)
                    {
                        if (item1["is_end"].ToString() == "0")
                        {
                            item["audit_state"] = "To_be_Improved";
                        }
                    }
                }

                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核查询供应商
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Edit_suppliers(object OBJ)
        
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (suppliers_code like '%{keyword}%' or suppliers_name like '%{keyword}%')";
                }
                string sql = $@"select suppliers_code as value,suppliers_name as label from base003m where (suppliers_code is not null or suppliers_name is not null) {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核查询审核类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Edit_audit_type(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (audit_type_f_code like '%{keyword}%' or audit_type_f_name like '%{keyword}%')";
                }
                string sql = $@"select audit_type_f_code as value,audit_type_f_name as label from bdm_audit_type_f_m where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核查询审核项目分类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Edit_audit_item_cate(object OBJ)
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
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (audit_item_cate_code like '%{keyword}%')";
                }
                string sql = $@"select audit_item_cate_code as value,audit_item_cate_code as label from bdm_audit_item_cate_m where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核生成任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertRQCAudit_Edit(object OBJ)
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
                string suppliers_code = jarr.ContainsKey("suppliers_code") ? jarr["suppliers_code"].ToString() : "";//查询条件 供应商编号
                string audit_type_code = jarr.ContainsKey("audit_type_code") ? jarr["audit_type_code"].ToString() : "";//查询条件 审核类型编号
                string overall_desc = jarr.ContainsKey("overall_desc") ? jarr["overall_desc"].ToString() : "";//查询条件 整体描述
                string imp_suggest = jarr.ContainsKey("imp_suggest") ? jarr["imp_suggest"].ToString() : "";//查询条件 改善建议
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                if (string.IsNullOrWhiteSpace(suppliers_code) || string.IsNullOrWhiteSpace(audit_type_code))
                {
                    //ret.ErrMsg = "供应商或审核类型不能为空!";
                    ret.ErrMsg = "Supplier or audit type cannot be empty!";
                    ret.IsSuccess = false;
                    return ret;
                }

                string task_no = string.Empty;
                task_no = DB.GetString($@"SELECT task_no FROM(select task_no from qcm_audit_task_m where task_no like '{"E" + DateTime.Now.ToString("yyyyMMdd")}%'  order by id desc)t WHERE ROWNUM <= 1");
                if (string.IsNullOrWhiteSpace(task_no))
                {
                    task_no = "E" + DateTime.Now.ToString("yyyyMMdd") + "1";
                }
                else
                {
                    task_no = $@"E{DateTime.Now.ToString("yyyyMMdd")}" + (Convert.ToInt32(task_no.Replace($@"E{DateTime.Now.ToString("yyyyMMdd")}", "")) + 1);
                }
                string sql = string.Empty;
                sql = $@"insert into qcm_audit_task_m (task_no,suppliers_code,audit_type_code,overall_desc,imp_suggest,sub_state,
                        createby,createdate,createtime) 
                        values('{task_no}','{suppliers_code}','{audit_type_code}','{overall_desc}','{imp_suggest}','0','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                sql = $@"
SELECT
	d.audit_item_cate_code,
	d.audit_item_cate_content,
	d.audit_type_code,
	d.score_limit,
	d.remarks
FROM
	bdm_audit_type_f_m fm
INNER JOIN bdm_audit_type_f_d fd ON FD.AUDIT_TYPE_F_CODE= FM.AUDIT_TYPE_F_CODE
INNER JOIN bdm_audit_type_m m ON m.AUDIT_TYPE_CODE = FD.AUDIT_TYPE_CODE 
INNER JOIN bdm_audit_type_d d ON d.AUDIT_TYPE_CODE = m.AUDIT_TYPE_CODE
WHERE
	fm.AUDIT_TYPE_F_CODE ='{audit_type_code}'";
                DataTable dt = DB.GetDataTable(sql);
                sql = "";
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> paramsDic = new Dictionary<string, object>();
                    int paramIndex = 1;
                    foreach (DataRow item in dt.Rows)
                    {
                        sql += $@"insert into qcm_audit_task_d (task_no,audit_item_cate_code,audit_item_cate_content,AUDIT_TYPE_F_CODE,audit_type_code,score_limit,remarks,
                            is_end,createby,createdate,createtime) 
                            values(@task_no_{paramIndex},@audit_item_cate_code_{paramIndex},@audit_item_cate_content_{paramIndex},@audit_type_f_code_{paramIndex},@audit_type_code_{paramIndex},
                            @score_limit_{paramIndex},@remarks_{paramIndex},'0','{user}','{date}','{time}');";
                        paramsDic.Add($@"task_no_{paramIndex}", task_no);
                        paramsDic.Add($@"audit_item_cate_code_{paramIndex}", item["audit_item_cate_code"]);
                        paramsDic.Add($@"audit_item_cate_content_{paramIndex}", item["audit_item_cate_content"]);
                        paramsDic.Add($@"audit_type_f_code_{paramIndex}", audit_type_code);
                        paramsDic.Add($@"audit_type_code_{paramIndex}", item["audit_type_code"]);
                        paramsDic.Add($@"score_limit_{paramIndex}", item["score_limit"]);
                        paramsDic.Add($@"remarks_{paramIndex}", item["remarks"]);

                        paramIndex++;
                    }
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;", paramsDic);
                }
                ret.IsSuccess = true;
                DB.Commit();
                ret.RetData1 = task_no;
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
        /// rqc工厂审核编辑页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Edit(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string audit_item_cate_code = jarr.ContainsKey("audit_item_cate_code") ? jarr["audit_item_cate_code"].ToString() : "";//查询条件 审核项目分类
                //转译
                string sql = string.Empty;
                sql = $@"SELECT
                        q.task_no,
                        q.suppliers_code,
                        m.suppliers_name,
                        q.audit_type_code,
                        b.audit_type_f_name as audit_type_name,
                        q.overall_desc,
                        q.imp_suggest,
                        q.sub_state,
                        '' as item_total,
                        '' as audit_score,
                        '' as audit_state
                        FROM
                        qcm_audit_task_m q
                        LEFT JOIN bdm_audit_type_f_m b on q.audit_type_code=b.audit_type_f_code
                        left join base003m m on q.suppliers_code=m.suppliers_code
                        where q.task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    string item_total = DB.GetString($@"select sum(score_limit) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["item_total"] = item_total == "" ? "0" : item_total;
                    string audit_score = DB.GetString($@"select sum(score) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["audit_score"] = audit_score == "" ? "0" : audit_score;
                    DataTable task_d_dt = DB.GetDataTable($@"select is_end from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    //item["audit_state"] = "已结案";
                    item["audit_state"] = "Closed";
                    foreach (DataRow item1 in task_d_dt.Rows)
                    {
                        if (item1.ToString() == "0")
                        {
                           // item["audit_state"] = "待改善";
                            item["audit_state"] = "To_be_Improved";
                        }
                    }
                }
                Dictionary<string, object> didt = new Dictionary<string, object>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    didt.Add(dt.Columns[i].ColumnName, dt.Rows[0][dt.Columns[i].ColumnName]);
                }

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(audit_item_cate_code))
                {
                    where = $@" and audit_item_cate_code='{audit_item_cate_code}'";
                }
                sql = $@"
                        SELECT
                            d.id,
                            d.audit_item_cate_code,
                            d.audit_item_cate_content,
                            d.remarks,
                            d.score_limit,
                            d.score,
                            d.examine_res,
                            d.pb_point,
                            d.improve_res,
                            d.is_end,
                            d.audit_type_code,
							TM.AUDIT_TYPE_NAME
                        FROM
                        qcm_audit_task_d d 
                        INNER JOIN BDM_AUDIT_TYPE_M tm ON TM.AUDIT_TYPE_CODE=d.AUDIT_TYPE_CODE
                        where task_no='{task_no}' {where}";
                DataTable Bodydt = DB.GetDataTable(sql);
                Bodydt.Columns.Add("file_list_wtd", typeof(object));//问题点图片
                foreach (DataRow item in Bodydt.Rows)
                {
                    DataTable fl = new DataTable();
                    fl.Columns.Add("file_name");
                    fl.Columns.Add("file_url");
                    item["file_list_wtd"] = fl;
                    sql = $@"SELECT 
                        t.file_name,
                        t.guid,
                        t.file_url
                        FROM
                        qcm_audit_task_d_f q
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_guid=t.guid
                        where source_type='0' and q.union_id='{item["id"]}'";
                    fl = DB.GetDataTable(sql);
                    if (fl.Rows.Count > 0)
                    {
                        item["file_list_wtd"] = fl;
                    }
                }

                Bodydt.Columns.Add("file_list_gsjg", typeof(object));//改善结果图片
                foreach (DataRow item in Bodydt.Rows)
                {
                    DataTable gs = new DataTable();
                    gs.Columns.Add("file_name");
                    gs.Columns.Add("file_url");
                    item["file_list_gsjg"] = gs;
                    sql = $@"SELECT 
                        t.file_name,
                        t.guid,
                        t.file_url
                        FROM
                        qcm_audit_task_d_f q
                        LEFT JOIN BDM_UPLOAD_FILE_ITEM t on q.file_guid=t.guid
                        where source_type='1' and q.union_id='{item["id"]}'";
                    gs = DB.GetDataTable(sql);
                    if (gs.Rows.Count > 0)
                    {
                        item["file_list_gsjg"] = gs;
                    }
                }

                //审核中分类 分组
                List<RQCDataBody> BodyList = new List<RQCDataBody>();
                foreach (DataRow item in Bodydt.Rows)
                {
                    var findObj = BodyList.FirstOrDefault(x => x.audit_type_f_code == item["audit_type_code"].ToString());
                    if (findObj == null)
                    {
                        RQCDataBody addBody = new RQCDataBody();
                        addBody.audit_type_f_code = item["audit_type_code"].ToString();
                        addBody.audit_type_f_name = item["AUDIT_TYPE_NAME"].ToString();
                        addBody.details = Bodydt.Clone();
                        addBody.details.ImportRow(item);

                        BodyList.Add(addBody);
                    }
                    else
                    {
                        findObj.details.ImportRow(item);
                    }
                }

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("DataHead", didt);
                dic.Add("DataBody", BodyList);

                ret.RetData1 = dic;
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
        /// rqc工厂审核编辑页保存草稿
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit(object OBJ)
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
                DataSet ds = Json2DataSetConverter.Convert(Data);

                //转译
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string overall_desc = jarr.ContainsKey("overall_desc") ? jarr["overall_desc"].ToString() : "";//查询条件 整体描述
                string imp_suggest = jarr.ContainsKey("imp_suggest") ? jarr["imp_suggest"].ToString() : "";//查询条件 改善建议
                DataTable datahead = ds.Tables["details"];//查询条件 卡片数据
                if (datahead==null||datahead.Rows.Count<=0)
                {
                    //ret.ErrMsg = "无审核项内容!";
                    ret.ErrMsg = "No review items!";
                    ret.IsSuccess = false;
                    return ret;
                }
                DataTable wtdguid = ds.Tables["wtdguid"];//查询条件 问题点图片guid数据
                DataTable gsjgguid = ds.Tables["gsjgguid"];//查询条件 改善结果图片guid数据
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间


                string sql = string.Empty;

                sql = $@"update qcm_audit_task_m set overall_desc='{overall_desc}',imp_suggest='{imp_suggest}',
                        modifyby='{user}',modifydate='{date}',modifytime='{time}'
                        where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                if (datahead.Rows.Count > 0)
                {
                    string sqllist = string.Empty;
                    foreach (DataRow item in datahead.Rows)
                    {
                        sql = $@"select improve_res from qcm_audit_task_d where task_no='{task_no}' and id='{item["id"]}'";
                        string improve_res = DB.GetString(sql);
                        if (item["improve_res"].ToString() != improve_res)
                        {
                            sqllist += $@"insert into qcm_audit_task_d_h (union_id,old_val,new_val,createby,createdate,createtime) 
                            values('{item["id"]}','{"改善结果:" + improve_res}','{"改善结果:" + item["improve_res"]}','{user}','{date}','{time}');";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sqllist))
                    {
                        DB.ExecuteNonQuery($@"BEGIN {sqllist} END;");
                    }
                }

                sql = "";
                string sqlguid = string.Empty;
                //DataTable wtdguid = new DataTable();
                //DataTable gsjgguid = new DataTable();
                if (datahead.Rows.Count > 0)
                {
                    foreach (DataRow item in datahead.Rows)
                    {
                        //wtdguid = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(item["wtdguid"].ToString());
                        //gsjgguid = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(item["gsjgguid"].ToString());

                        sql += $@"update qcm_audit_task_d set score='{item["score"]}',examine_res='{item["examine_res"]}',pb_point='{item["pb_point"]}',
                            improve_res='{item["improve_res"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{item["id"]}';";
                        sqlguid = "";
                        DB.ExecuteNonQuery($@"delete from qcm_audit_task_d_f where union_id='{item["id"]}' and source_type='0'");
                        if (wtdguid != null && wtdguid.Rows.Count > 0)
                        {
                            foreach (DataRow item1 in wtdguid.Rows)
                            {
                                if (item["_id"].ToString() == item1["_parent_id"].ToString())
                                {
                                    sqlguid += $@"insert into qcm_audit_task_d_f (union_id,file_guid,source_type,createby,createdate,createtime) 
                                            values('{item["id"]}','{item1["guid"]}','0','{user}','{date}','{time}');";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(sqlguid))
                                DB.ExecuteNonQuery($@"BEGIN {sqlguid} END;");
                        }
                        sqlguid = "";
                        DB.ExecuteNonQuery($@"delete from qcm_audit_task_d_f where union_id='{item["id"]}' and source_type='1'");
                        if (gsjgguid != null && gsjgguid.Rows.Count > 0)
                        {
                            foreach (DataRow item2 in gsjgguid.Rows)
                            {
                                if (item["_id"].ToString() == item2["_parent_id"].ToString())
                                {
                                    sqlguid += $@"insert into qcm_audit_task_d_f (union_id,file_guid,source_type,createby,createdate,createtime) 
                                            values('{item["id"]}','{item2["guid"]}','1','{user}','{date}','{time}');";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(sqlguid))
                                DB.ExecuteNonQuery($@"BEGIN {sqlguid} END;");
                        }
                    }
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");
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
        /// rqc工厂审核编辑页提交报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit_sub(object OBJ)
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
                
                DataSet ds = Json2DataSetConverter.Convert(Data);

                //转译
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string overall_desc = jarr.ContainsKey("overall_desc") ? jarr["overall_desc"].ToString() : "";//查询条件 整体描述
                string imp_suggest = jarr.ContainsKey("imp_suggest") ? jarr["imp_suggest"].ToString() : "";//查询条件 改善建议
                DataTable datahead = ds.Tables["details"];//查询条件 卡片数据
                if (datahead == null || datahead.Rows.Count <= 0)
                {
                    //ret.ErrMsg = "无审核项内容!";
                    ret.ErrMsg = "No review items!";
                    ret.IsSuccess = false;
                    return ret;
                }
                DataTable wtdguid = ds.Tables["wtdguid"];//查询条件 问题点图片guid数据
                DataTable gsjgguid = ds.Tables["gsjgguid"];//查询条件 改善结果图片guid数据
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;

                sql = $@"update qcm_audit_task_m set overall_desc='{overall_desc}',imp_suggest='{imp_suggest}',sub_state='1',
                        modifyby='{user}',modifydate='{date}',modifytime='{time}'
                        where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                if (datahead.Rows.Count > 0)
                {
                    string sqllist = string.Empty;
                    foreach (DataRow item in datahead.Rows)
                    {
                        sql = $@"select improve_res from qcm_audit_task_d where task_no='{task_no}' and id='{item["id"]}'";
                        string improve_res = DB.GetString(sql);
                        if (item["improve_res"].ToString() != improve_res)
                        {
                            sqllist += $@"insert into qcm_audit_task_d_h (union_id,old_val,new_val,createby,createdate,createtime) 
                            values('{item["id"]}','{"改善结果:" + improve_res}','{"改善结果:" + item["improve_res"]}','{user}','{date}','{time}');";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sqllist))
                    {
                        DB.ExecuteNonQuery($@"BEGIN {sqllist} END;");
                    }
                }

                sql = "";
                string sqlguid = string.Empty;
                //DataTable wtdguid = new DataTable();
                //DataTable gsjgguid = new DataTable();
                if (datahead.Rows.Count > 0)
                {
                    foreach (DataRow item in datahead.Rows)
                    {
                        //wtdguid = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(item["wtdguid"].ToString());
                        //gsjgguid = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(item["gsjgguid"].ToString());

                        sql += $@"update qcm_audit_task_d set score='{item["score"]}',examine_res='{item["examine_res"]}',pb_point='{item["pb_point"]}',
                            improve_res='{item["improve_res"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{item["id"]}';";
                        sqlguid = "";
                        if (wtdguid != null && wtdguid.Rows.Count > 0)
                        {
                            DB.ExecuteNonQuery($@"delete from qcm_audit_task_d_f where union_id='{item["id"]}' and source_type='0'");
                            foreach (DataRow item1 in wtdguid.Rows)
                            {
                                if (item["_id"].ToString() == item1["_parent_id"].ToString())
                                {
                                    sqlguid += $@"insert into qcm_audit_task_d_f (union_id,file_guid,source_type,createby,createdate,createtime) 
                                            values('{item["id"]}','{item1["guid"]}','0','{user}','{date}','{time}');";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(sqlguid))
                                DB.ExecuteNonQuery($@"BEGIN {sqlguid} END;");
                        }
                        sqlguid = "";
                        if (gsjgguid != null && gsjgguid.Rows.Count > 0)
                        {
                            DB.ExecuteNonQuery($@"delete from qcm_audit_task_d_f where union_id='{item["id"]}' and source_type='1'");
                            foreach (DataRow item2 in gsjgguid.Rows)
                            {
                                if (item["_id"].ToString() == item2["_parent_id"].ToString())
                                {
                                    sqlguid += $@"insert into qcm_audit_task_d_f (union_id,file_guid,source_type,createby,createdate,createtime) 
                                            values('{item["id"]}','{item2["guid"]}','1','{user}','{date}','{time}');";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(sqlguid))
                                DB.ExecuteNonQuery($@"BEGIN {sqlguid} END;");
                        }
                    }
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");
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
        /// rqc工厂审核编辑页结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit_over(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 工厂审核详情表id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"update qcm_audit_task_d set is_end='1' where id='{id}'";
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
        /// rqc工厂审核编辑页更新改善结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit_gsjg(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 工厂审核详情表id
                string improve_res = jarr.ContainsKey("improve_res") ? jarr["improve_res"].ToString() : "";//查询条件 改善结果
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"select improve_res from qcm_audit_task_d where id='{id}'";
                string improve_res_test = DB.GetString(sql);
                sql = $@"update qcm_audit_task_d set improve_res='{improve_res}' where id='{id}'";
                DB.ExecuteNonQuery(sql);
                if (improve_res != improve_res_test)
                {
                    sql = $@"insert into qcm_audit_task_d_h (union_id,old_val,new_val,createby,createdate,createtime) 
                            values('{id}','{"改善结果:" + improve_res_test}','{"改善结果:" + improve_res}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
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
        /// rqc工厂审核编辑页查询历史记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Edit_history(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 工厂审核详情表id
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (createdate like '%{keyword}%' or old_val like '%{keyword}%' or new_val like '%{keyword}%' or createby like '%{keyword}%')";
                }
                string sql = $@"select createdate,old_val,new_val,createby from qcm_audit_task_d_h where union_id='{id}' {where} order by id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核主页查询-工厂端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRQCAudit_Main_factory(object OBJ)
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
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 开始时间
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//查询条件 结束时间
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string state = jarr.ContainsKey("state") ? jarr["state"].ToString() : "";//查询条件 审核状态0:待改善;1:已结案
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //转译
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(start_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@" and (to_date(m.createdate,'yyyy-mm-dd,hh24:mi:ss') between to_date('{start_date}','yyyy-mm-dd,hh24:mi:ss') and to_date('{end_date}','yyyy-mm-dd,hh24:mi:ss'))";
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    where += $@" and (t.audit_type_name like '%{keyword}%' or m.task_no like '%{keyword}%' or b.suppliers_name like '%{keyword}%' or t.audit_type_name like '{keyword}')";
                }
                string sql = string.Empty;
                sql = $@"SELECT
	                        TASK_NO
                        FROM
	                        qcm_audit_task_d 
                        GROUP BY TASK_NO 
                        HAVING COUNT(1)=COUNT(CASE IS_END WHEN '1' THEN '1' ELSE NULL END) ";
                if (state == "0")
                {
                    where += $@" and m.task_no not in ({sql})";
                }
                else if (state == "1")
                {
                    where += $@" and m.task_no in ({sql})";
                }
                sql = $@"SELECT
                        m.task_no,
                        m.createdate,
                        m.suppliers_code,
                        b.suppliers_name,
                        m.audit_type_code,
                        t.AUDIT_TYPE_F_NAME as audit_type_name,
                        '' as item_total,
                        '' as audit_score,
                        '' as stay_perfect_count,
                        '' as audit_state,
                        m.createby
                        FROM
                        qcm_audit_task_m m
                        LEFT JOIN base003m b on m.SUPPLIERS_CODE=b.SUPPLIERS_CODE
                        LEFT JOIN BDM_AUDIT_TYPE_F_M t on m.audit_type_code=t.AUDIT_TYPE_F_CODE
                        where m.sub_state='1' {where} order by m.id desc
                        ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                foreach (DataRow item in dt.Rows)
                {
                    string item_total = DB.GetString($@"select sum(score_limit) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["item_total"] = item_total == "" ? "0" : item_total;
                    string audit_score = DB.GetString($@"select sum(score) from qcm_audit_task_d where task_no='{item["task_no"]}'");
                    item["audit_score"] = audit_score == "" ? "0" : audit_score;
                    string stay_perfect_count = DB.GetString($@"select count(1) from qcm_audit_task_d where is_end='0' and task_no='{item["task_no"]}'");
                    item["stay_perfect_count"] = stay_perfect_count;
                    DataTable task_d_dt = DB.GetDataTable($@"select is_end from qcm_audit_task_d where task_no='{item["task_no"]}'");
                   // item["audit_state"] = "已结案";
                    item["audit_state"] = "Closed";
                    foreach (DataRow item1 in task_d_dt.Rows)
                    {
                        if (item1["is_end"].ToString() == "0")
                        {
                            item["audit_state"] = "To_be_Improved";
                        }
                    }
                }

                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData1 = dic;
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
        /// rqc工厂审核编辑页取消结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit_cancel_over(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 工厂审核详情表id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"update qcm_audit_task_d set is_end='0' where id='{id}'";
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
        /// rqc工厂审核删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteRQCAudit(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                //string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                //string time = DateTime.Now.ToString("HH:mm:ss");//时间

                if (string.IsNullOrWhiteSpace(task_no))
                {
                    //ret.ErrMsg = "没有任务编号!";
                    ret.ErrMsg = "No task number!";
                    ret.IsSuccess = false;
                    return ret;
                }

                string sql = string.Empty;
                sql = $@"delete from qcm_audit_task_m where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from qcm_audit_task_d where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                string did = DB.GetString($@"select id from qcm_audit_task_d where task_no='{task_no}'");

                sql = $@"delete from qcm_audit_task_d_f where union_id='{did}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from qcm_audit_task_d_h where union_id='{did}'";
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
        /// rqc工厂审核编辑页提交-工厂端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditRQCAudit_Edit_factory_gsjg(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 工厂审核详情表id
                string improve_res = jarr.ContainsKey("improve_res") ? jarr["improve_res"].ToString() : "";//查询条件 改善结果
                DataTable gsjgguid = jarr.ContainsKey("gsjgguid") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["gsjgguid"].ToString()) : null;//查询条件 改善结果图片guid数据
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"select improve_res from qcm_audit_task_d where id='{id}'";
                string improve_res_test = DB.GetString(sql);
                sql = $@"update qcm_audit_task_d set improve_res='{improve_res}' where id='{id}'";
                DB.ExecuteNonQuery(sql);
                if (improve_res != improve_res_test)
                {
                    sql = $@"insert into qcm_audit_task_d_h (union_id,old_val,new_val,createby,createdate,createtime) 
                            values('{id}','{"改善结果:" + improve_res_test}','{"改善结果:" + improve_res}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }

                DB.ExecuteNonQuery($@"delete from qcm_audit_task_d_f where union_id='{id}' and source_type='1'");
                if (gsjgguid.Rows.Count > 0)
                {
                    sql = "";
                    foreach (DataRow item in gsjgguid.Rows)
                    {
                        sql += $@"insert into qcm_audit_task_d_f (union_id,file_guid,source_type,createby,createdate,createtime) 
                                            values('{id}','{item["guid"]}','1','{user}','{date}','{time}');";
                    }
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");
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
    }

    #region Dto
    public class RQCDataBody
    {
        public string audit_type_f_code { get; set; }
        public string audit_type_f_name { get; set; }
        public DataTable details { get; set; }
    }
    #endregion
}
