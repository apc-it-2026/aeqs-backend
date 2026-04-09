using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.DBHelper;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SJ_AQLAPI.DTO;
using System.Linq;

namespace SJ_AQLAPI
{
    public class F_AQL_Entry
    {
        /// <summary>
        /// 查询-AQL录入-样本级别/AQL级别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEntry_RawLevel(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string sql = string.Empty;

                sql = $@"select enum_code as code,enum_value as value from sys001m where enum_type='AQL_ENUM_RAW'";
                DataTable dt1 = DB.GetDataTable(sql);

                sql = $@"select enum_value2 as code,enum_value as value from sys001m where enum_type='enum_aql_level'";
                DataTable dt2 = DB.GetDataTable(sql);

                sql = $@"select sample_level,aql_level from aql_cma_task_list_m_aql_m where task_no='{task_no}'";
                DataTable dt3 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
                dic.Add("Data3", dt3);
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
        /// 查询-AQL录入-不良分类/不良项目
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEntry_Classify(object OBJ)
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
                string bad_classify_code = jarr.ContainsKey("bad_classify_code") ? jarr["bad_classify_code"].ToString() : "";//不良分类代号
                string sql = string.Empty;

                if (string.IsNullOrWhiteSpace(bad_classify_code))
                {
                    sql = $@"select bad_classify_code,bad_classify_name from bdm_aql_bad_classify";
                }
                else
                {
                    sql = $@"select bad_classify_code,bad_item_code,bad_item_name,problem_level from bdm_aql_bad_classify_d where bad_classify_code='{bad_classify_code}'";
                }
                DataTable dt = DB.GetDataTable(sql);
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

        /// <summary>
        /// 查询-AQL录入-不良排序-图片
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
	                            'aql_cma_task_list_m_aql_e_br_f' AS tablename,
	                            f.id,
	                            GUID 
                            FROM
	                            BDM_UPLOAD_FILE_ITEM t 
	                            LEFT JOIN aql_cma_task_list_m_aql_e_br_f f on f.file_guid= t.guid
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
        /// 查询-AQL录入-根据AQL级别获取抽样比例
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEntry_SamplingRate(object OBJ)
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
                string ac = jarr.ContainsKey("ac") ? jarr["ac"].ToString() : "";//查询条件 ac
                string num = jarr.ContainsKey("num") ? jarr["num"].ToString() : "";//查询条件 任务数量
                string LEVEL_TYPE = jarr.ContainsKey("LEVEL_TYPE") ? jarr["LEVEL_TYPE"].ToString() : "";//查询条件 样本级别
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";
                string task_type = jarr.ContainsKey("task_type") ? jarr["task_type"].ToString() : "";

                string where = string.Empty;
                string sql = $@"select VALS, {ac} as AC from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                DataTable dt = DB.GetDataTable(sql);

                //查询 AC12 1.5 AC13 2.5
                sql = $@"select VALS,AC12,AC13 from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                DataTable dtAC1213 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1213", dtAC1213);

                //计算点箱
                if (task_type == "Manually_Created")//手动创建
                {
                    //int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_pb where task_no='{task_no}'");
                    //if (count <= 0)
                    //{
                    //    InsertPointBox(OBJ);
                    //}
                    sql = $@"SELECT
	MAX(p.id) as id,
	MAX(p.task_no) as task_no,
	MAX(p.case_no) as case_no
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
	ORDER BY MAX(s.SIZE_SEQ)";
                }
                else
                {
                    sql = $@"SELECT
	MAX(p.id) as id,
	MAX(p.task_no) as task_no,
	MAX(p.case_no) as case_no
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
	ORDER BY MAX(s.SIZE_SEQ)";
                }
                DataTable dtdx = DB.GetDataTable(sql);
                dic.Add("Datadx", dtdx);
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
        /// 查询-AQL录入-不良排序
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEntry_Sorting(object OBJ)
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
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                #region Commented for new defect codes in AQL
                //string sql = $@"SELECT
                //                MAX(a.bad_classify_code) as bad_classify_code,
                //             MAX(a.bad_item_code) as bad_item_code,
                //             MAX(a.bad_item_name) as bad_item_name,
                //             MAX(a.problem_level) as problem_level,
                //             MAX(a.bad_qty) as bad_qty,
                //              {CommonBASE.GetGroupConcatByOracleVersion(DB, "to_char( b.file_guid )", "a.id")} as imglist
                //             from 
                //             aql_cma_task_list_m_aql_e_br a
                //             LEFT JOIN aql_cma_task_list_m_aql_e_br_f b on a.task_no=b.task_no and a.bad_classify_code=b.bad_classify_code and a.bad_item_code=b.bad_item_code
                //                where a.task_no='{task_no}' GROUP BY a.task_no,a.bad_classify_code,a.bad_item_code";
                //DataTable dt = DB.GetDataTable(sql);

                #endregion

                string sqlNew = $@"
SELECT
    a.bad_classify_code,
    a.bad_item_code,
    a.bad_item_name,
    a.problem_level,
    SUM(a.bad_qty) AS bad_qty,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "to_char(b.file_guid)", "a.id")} AS imglist
FROM aql_cma_task_list_m_aql_e_br a
LEFT JOIN aql_cma_task_list_m_aql_e_br_f b
    ON a.task_no = b.task_no
   AND a.bad_classify_code = b.bad_classify_code
   AND a.bad_item_code = b.bad_item_code
WHERE a.task_no = '{task_no}'
GROUP BY
    a.task_no,
    a.bad_classify_code,
    a.bad_item_code,
    a.bad_item_name,
    a.problem_level";

                DataTable dt = DB.GetDataTable(sqlNew);


              

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
        /// 提交-AQL录入-不良排序
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditAQLEntry_Sorting(object OBJ)
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
                string inspection_date = jarr.ContainsKey("inspection_date") ? jarr["inspection_date"].ToString() : "";//检验日期
                string sample_level = jarr.ContainsKey("sample_level") ? jarr["sample_level"].ToString() : "";//条件 样本级别
                string aql_level = jarr.ContainsKey("aql_level") ? jarr["aql_level"].ToString() : "";//条件 AQL级别
                DataTable cma_task_br = jarr.ContainsKey("cma_task_br") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["cma_task_br"].ToString()) : null;//条件 不良项目记录
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                string sqlimg = string.Empty;

                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_aql_e_br where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_aql_e_br_f where task_no='{task_no}'");

                DB.ExecuteNonQuery($@"update aql_cma_task_list_m set inspection_state='1',AQL_EDIT_STATE='1',is_inspection='1',inspection_date=to_date('{inspection_date}','yyyy-mm-dd') where task_no='{task_no}' ");//改变AQL清单验货状态

                //aql录入无数据验货日期为空有数据取最近日期
                if (cma_task_br.Rows.Count > 0)
                    DB.ExecuteNonQuery($@"update aql_cma_task_list_m set f_inspection_time='{date}' where task_no='{task_no}' ");
                else
                    DB.ExecuteNonQuery($@"update aql_cma_task_list_m set f_inspection_time='' where task_no='{task_no}' ");

                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_aql_m where task_no='{task_no}'");
                if (count > 0)
                    DB.ExecuteNonQuery($@"update aql_cma_task_list_m_aql_m set sample_level='{sample_level}',aql_level='{aql_level}',
                                        modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'");
                else
                    DB.ExecuteNonQuery($@"insert into aql_cma_task_list_m_aql_m (task_no,sample_level,aql_level,createby,createdate,createtime) 
                                          values('{task_no}','{sample_level}','{aql_level}','{user}','{date}','{time}')");

                int index_i = 0;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                Dictionary<string, object> paramTestDicimg = new Dictionary<string, object>();
                foreach (DataRow item in cma_task_br.Rows)
                {
                    sql += $@"insert into aql_cma_task_list_m_aql_e_br (task_no,bad_classify_code,bad_item_code,bad_item_name,problem_level,bad_qty,
                                createby,createdate,createtime) 
                            values(@task_no{index_i},@bad_classify_code{index_i},@bad_item_code{index_i},@bad_item_name{index_i},@problem_level{index_i},@bad_qty{index_i},
                            @createby{index_i},@createdate{index_i},@createtime{index_i});";
                    paramTestDic.Add($@"task_no{index_i}", $@"{task_no}");
                    paramTestDic.Add($@"bad_classify_code{index_i}", $@"{item["不良分类代号3"]}");
                    paramTestDic.Add($@"bad_item_code{index_i}", $@"{item["不良项目代号2"]}");
                    paramTestDic.Add($@"bad_item_name{index_i}", $@"{item["不良项目名称2"]}");
                    paramTestDic.Add($@"problem_level{index_i}", $@"{item["问题级别2"]}");
                    paramTestDic.Add($@"bad_qty{index_i}", $@"{item["不良数量"]}");
                    paramTestDic.Add($@"createby{index_i}", $@"{user}");
                    paramTestDic.Add($@"createdate{index_i}", $@"{date}");
                    paramTestDic.Add($@"createtime{index_i}", $@"{time}");
                    string[] imgs = item["imglist"].ToString().Split(',');

                    int index_j = 0;
                    for (int i = 0; i < imgs.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(imgs[i]))
                        {
                            sqlimg += $@"insert into aql_cma_task_list_m_aql_e_br_f (task_no,bad_classify_code,bad_item_code,file_guid,createby,createdate,createtime) 
                                        values(@task_no_{index_i}_{index_j},@bad_classify_code_{index_i}_{index_j},@bad_item_code_{index_i}_{index_j},@file_guid_{index_i}_{index_j},@createby_{index_i}_{index_j},@createdate_{index_i}_{index_j},@createtime_{index_i}_{index_j});";
                            paramTestDicimg.Add($@"task_no_{index_i}_{index_j}", $@"{task_no}");
                            paramTestDicimg.Add($@"bad_classify_code_{index_i}_{index_j}", $@"{item["不良分类代号3"]}");
                            paramTestDicimg.Add($@"bad_item_code_{index_i}_{index_j}", $@"{item["不良项目代号2"]}");
                            paramTestDicimg.Add($@"file_guid_{index_i}_{index_j}", $@"{imgs[i]}");
                            paramTestDicimg.Add($@"createby_{index_i}_{index_j}", $@"{user}");
                            paramTestDicimg.Add($@"createdate_{index_i}_{index_j}", $@"{date}");
                            paramTestDicimg.Add($@"createtime_{index_i}_{index_j}", $@"{time}");
                            index_j++;
                        }
                    }

                    index_i++;
                }

                if (!string.IsNullOrEmpty(sql))
                {
                    String eSql = $@"begin {sql} end;";
                    DB.ExecuteNonQuery(eSql, paramTestDic);
                }

                if (!string.IsNullOrEmpty(sqlimg))
                {
                    String eSql = $@"begin {sqlimg} end;";
                    DB.ExecuteNonQuery(eSql, paramTestDicimg);
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
        /// 获取样本级别/AQL级别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEntry_Level(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//任务编号
                string sql = string.Empty;

                sql = $@"select enum_code as code,enum_value as value from sys001m where enum_type='AQL_ENUM_RAW'";
                DataTable dt1 = DB.GetDataTable(sql);

                sql = $@"select enum_value2 as code,enum_value as value from sys001m where enum_type='enum_aql_level'";
                DataTable dt2 = DB.GetDataTable(sql);

                sql = $@"select sample_level,aql_level from aql_cma_task_list_m_aql_m where task_no='{task_no}'";
                DataTable dt3 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
                dic.Add("Data3", dt3);
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
        /// 是否为最终任务
        /// </summary>
        public ResultObject CheckIsFinalTask(object OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                string Data = reqObj.Data.ToString();
                string task_no = JsonConvert.DeserializeObject<string>(Data);

                //必须为最终任务
                string sql = $@"select inspection_type from aql_cma_task_list_m where task_no = '{task_no}' ";
                if (DB.GetString(sql) == "0")
                {
                    ret.IsSuccess = true;
                    return ret;
                }
                ret.IsSuccess = false;
            }
            catch (Exception ex)
            {
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;
        }

        /// <summary>
        /// 检验PV88提交前的必填验证项
        /// </summary>
        public ResultObject CheckRequiredField(object OBJ)
        {
            RequestObject ReqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string checkListStr = jarr["checkList"].ToString();
                List<string> checkList = JsonConvert.DeserializeObject<List<string>>(checkListStr);

                // 点箱状态
                string sql = $@"select pb_state from aql_cma_task_list_m where task_no = '{task_no}'";
                string pointBoxState = DB.GetString(sql);

                // 核对项目状态
                for (int i = 0; i < checkList.Count; i++)
                {
                    checkList[i] = "'" + checkList[i] + "'";
                }
                sql = $@"select count(*) from aql_cma_task_list_m_cd where task_no = '{task_no}' and conclusion is not null and conclusion_type in({string.Join(",", checkList)})";
                int checkCount = DB.GetInt32(sql);

                // 相片(产品、测量)
                sql = $@"select distinct(image_type) from aql_cma_task_list_m_pg_d where task_no = '{task_no}' group by image_type";
                DataTable dataTable = DB.GetDataTable(sql);
                int photoCount = dataTable.Rows.Count;

                if (pointBoxState.Equals("1") && checkCount >= checkList.Count && photoCount == 2)
                {
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "请核对Pivot88项目、点箱、照片等资料是否填写完整!";
                    ret.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 获取p88核对项否定数量
        /// </summary>
        public ResultObject GetNegativeItemNums(object OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                string Data = reqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                List<string> typeList = JsonConvert.DeserializeObject<List<string>>(jarr["typeList"].ToString());

                for (int i = 0; i < typeList.Count; i++)
                {
                    typeList[i] = "'" + typeList[i] + "'";
                }
                string sql = $@"select count(*) from aql_cma_task_list_m_cd where task_no = '{task_no}' and conclusion = '0' and conclusion_type in ({string.Join(",", typeList)})";
                int count = DB.GetInt32(sql);

                ret.RetData = JsonConvert.SerializeObject(count);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;
        }

        /// <summary>
        ///Transfer data to p88 intermediate table
        /// </summary>
        public ResultObject TransferDataToPivot88(object OBJ)
        {
            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();

            try
            {
                DB = new DataBase(reqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = reqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                var aeqsMainJson = jarr.ContainsKey("aeqsMain") ? jarr["aeqsMain"] : "";
                var assignmentJson = jarr.ContainsKey("aeqsAssignment") ? jarr["aeqsAssignment"] : "";
                var boxListJson = jarr.ContainsKey("box") ? jarr["box"] : "";
                var productListJson = jarr.ContainsKey("product") ? jarr["product"] : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);

                AEQS_TO_P88_LIST aeqsMain = JsonConvert.DeserializeObject<AEQS_TO_P88_LIST>(aeqsMainJson.ToString());
                AEQS_TO_P88_ASSIGNMENT aeqsAssignment = JsonConvert.DeserializeObject<AEQS_TO_P88_ASSIGNMENT>(assignmentJson.ToString());

                List<AEQS_TO_P88_SECTIONS> boxList = JsonConvert.DeserializeObject<List<AEQS_TO_P88_SECTIONS>>(boxListJson.ToString());
                List<AEQS_TO_P88_SECTIONS> productList = JsonConvert.DeserializeObject<List<AEQS_TO_P88_SECTIONS>>(productListJson.ToString());

                string mid = string.Empty;
                try
                {
                    //Get id from sequence
                    string id = DB.GetString("SELECT T_AEQS_TO_P88_LIST_ID_SEQ.nextval FROM dual");
                    mid = "apache5_" + id.PadLeft(3, '0');
                }
                catch (Exception ex)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = ex.Message;
                    return ret;
                }
                #region Master data section
                //The start time is the earliest submission time of the task.
                string sql = $@"select concat(concat(min(CREATEDATE), ' '), min(createtime)) from aql_cma_task_list_m_cd
                                 where CREATEDATE = (select min(CREATEDATE) from aql_cma_task_list_m_cd where task_no in ('{task_no}'))
                                 and task_no = '{task_no}'";
                //Remove duplicate box number
                //string caseNo = DB.GetString($@"select listagg( distinct case_no,'/') as case_no  from aql_cma_task_list_m_pb where  task_no = '{task_no}'");
                //caseNo = string.Join("/", caseNo.Split('/').Distinct().ToArray());
                string caseNo = DB.GetString($@"select listagg(distinct case_no, '/') within group(order by to_number(case_no) asc) as case_no
                                  from (select distinct REGEXP_SUBSTR(case_no, '[^/]+', 1, level) case_no
                                          from aql_cma_task_list_m_pb
                                         where task_no = '{task_no}' and case_no !='/' and case_no is not null
                                        connect by level <= REGEXP_COUNT(case_no, '[^/]+')
                                               and rowid = prior rowid
                                               and prior dbms_random.value is not null)");


                aeqsMain.date_started = Convert.ToDateTime(DB.GetDateTime(sql).ToString("yyyy-MM-dd HH:mm:ss"));
                aeqsMain.passfails_0_listvalues_value = caseNo;
                aeqsMain.assignment_items_assignment_report_type_id = 9;

                sql = $@"insert into t_aeqs_to_p88_list
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
                                    '{mid}',
                                    '{aeqsMain.status}',
                                    to_date('{aeqsMain.date_started.ToString("yyyy-MM-dd HH:mm:ss")}', 'yyyy-mm-dd hh24:mi:ss'),
                                    '{aeqsMain.defective_parts}',
                                    '{aeqsMain.passfails_0_title}',
                                    '{aeqsMain.passfails_0_type}',
                                    '{aeqsMain.passfails_0_subsection}',
                                    '{aeqsMain.passfails_0_listvalues_value}',
                                    '{aeqsMain.assignment_items_assignment_report_type_id}'
                                    )";
                DB.ExecuteNonQuery(sql);
                #endregion

                if (!string.IsNullOrEmpty(mid))
                {
                    #region assignment部分
                    aeqsAssignment.assignment_items_assignment_inspector_username = DB.GetString($"select p88_checker from t_aeqs_to_p88_inspector where checker_code = '{user}'");
                    string dateStartSql = $@"select concat(concat(min(CREATEDATE),' '),min(createtime)) from aql_cma_task_list_m_cd where  CREATEDATE in(
                                                select min(CREATEDATE)  from aql_cma_task_list_m_cd where task_no = '{task_no}')";
                    DateTime dateStart = Convert.ToDateTime(DB.GetDateTime(dateStartSql).ToString("yyyy-MM-dd HH:mm:ss"));

                    string art = DB.GetString($@"select art_no from aql_cma_task_list_m  where task_no = '{task_no}'");
                    string po = DB.GetString($@"select po from aql_cma_task_list_m where task_no ='{task_no}'");
                    //时间间隔分钟数
                    aeqsAssignment.assignment_items_inspection_completed_date = Convert.ToDateTime(date + " " + time);//提交时间
                    TimeSpan ts = aeqsAssignment.assignment_items_inspection_completed_date - aeqsMain.date_started;
                    aeqsAssignment.assignment_items_total_inspection_minutes = Math.Round(Convert.ToDecimal(ts.TotalMinutes));
                    aeqsAssignment.assignment_items_assignment_date_inspection = aeqsMain.date_started;



                    //商品名稱
                    string itemName = DB.GetString($@"select s.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '{art}'");
                    if (string.IsNullOrEmpty(itemName))
                    {
                        itemName = DB.GetString($@"select p.name_s from bdm_rd_style s inner join bdm_rd_prod p on s.shoe_no = p.shoe_no where p.prod_no = '{art}'");
                    }
                    //aeqsAssignment.assignment_items_assignment_date_inspection = dateStart;
                    //aeqsAssignment.assignment_items_po_line_po_number = po.ToString().Split('&')[0];// Edit on 7/12(Po Change)
                   // aeqsAssignment.assignment_items_po_line_po_number = DB.GetString($@"select customer_po from bdm_se_order_master where mer_po = '{po}'"); //Edit on 2/8(PO Change2)
                    aeqsAssignment.assignment_items_po_line_po_number = po;//Edit on 2/21(PO Change2)

                    aeqsAssignment.assignment_items_po_line_sku_item_name = itemName;

                    //按码数个数传递，有多少个码数就传递多少条记录。
                    //DataTable sizeTable = DB.GetDataTable($@"select distinct concat('{art}_',size_no) as size_no,se_qty from bdm_se_order_size where se_id in (
                    //                                                select se_id from bdm_se_order_master where mer_po = '{po}')");
                    #region Before PO Change2 on 2025/02/13
                    //码数，按"ART_SIZE"格式组合，SIZE转换后的码数从base097m取
                    //DataTable sizeTable = DB.GetDataTable($@"select max(concat('{art}_', d.ad_size)) as ad_size,
                    //                                               max(a.se_qty) as se_qty,
                    //                                               sum(b.se_qty) as po_se_qty,
                    //                                              (select PO_SEQ from bdm_se_order_size A, bdm_se_order_master B  where A.SE_ID=B.SE_ID AND MER_PO= '{po}' AND  SIZE_NO=b.size_no) AS PO_SEQ
                    //                                          from aql_cma_task_list_m_pb a
                    //                                          left join bdm_se_order_size b
                    //                                            on a.cr_size = b.size_no
                    //                                          left join bdm_se_order_master c
                    //                                            on b.se_id = c.se_id
                    //                                          left join base097m d
                    //                                            on b.size_no = FACTORY_SIZE
                    //                                         where a.task_no = '{task_no}'
                    //                                           and a.se_qty > 0 
                    //                                           and c.mer_po = '{po}'
                    //                                         group by ad_size, a.se_qty, b.size_no
                    //                                         order by max(b.size_seq)");

                    #endregion

                    #region After PO change2  on 2025/02/13
                    //The number of codes is combined in the "ART_SIZE" format. The number of codes after SIZE conversion is taken from base097m (the order before POChange is combined in ART_SIZE, and after the change is combined in ART_SIZE_LineItem)） Edit on POChange :2024/6/20
                    DataTable sizeTable = null;
                    //Non-merged orders (multiple size lines, or merged orders) follow the old logic
                    string merge_mark = DB.GetString($@"select so_mergr_mark from bdm_se_order_master where mer_po = '{po}'");
                    if (merge_mark.Equals("Y"))
                    {
                        sizeTable = DB.GetDataTable($@"select max(concat('{art}_', d.ad_size)) as ad_size,
                                                            a.cr_size ,
                                                            r.po_line_item as po_seq,
                                                            max(a.se_qty) as se_qty, --Batch quantity
                                                            max(r.po_line_qty) as po_se_qty --The total quantity of this code in the batch order
                                                        from aql_cma_task_list_m_pb a
                                                        left join base097m d on a.cr_size = factory_size
                                                        left join t_bdm_se_order_reference r on a.cr_size = r.po_size and a.po_line_item = r.po_line_item
                                                        where a.task_no = '{task_no}'
                                                        and a.se_qty > 0 
                                                        and nvl(r.is_delete,'N') != 'Y'
                                                        group by ad_size,a.cr_size,r.po_line_item
                                                        order by max(a.cr_size) asc ");
                    }
                    else
                    {
                        sizeTable = DB.GetDataTable($@"select max(concat('{art}_', d.ad_size)) as ad_size,
                                                                    max(a.se_qty) as se_qty,
                                                                    sum(b.se_qty) as po_se_qty,b.size_no ,
                                                    (select PO_SEQ from bdm_se_order_size A, bdm_se_order_master B  where A.SE_ID=B.SE_ID AND MER_PO= '{po}' AND  SIZE_NO=b.size_no) AS PO_SEQ
                                                                from aql_cma_task_list_m_pb a
                                                                left join bdm_se_order_size b
                                                                on a.cr_size = b.size_no
                                                                left join bdm_se_order_master c
                                                                on b.se_id = c.se_id
                                                                left join base097m d
                                                                on b.size_no = FACTORY_SIZE
                                                                where a.task_no = '{task_no}'
                                                                and a.se_qty > 0 
                                                                and c.mer_po = '{po}'
                                                                group by ad_size, a.se_qty, b.size_no
                                                                order by max(b.size_seq)");
                    }
                    #endregion

                    if (sizeTable.Rows.Count > 0)
                    {

                        #region 每个码数的双数
                        //添加双数列
                        DataColumn column = new DataColumn("shuangshu");
                        sizeTable.Columns.Add(column);
                        //计算双数
                        decimal sampleSize = aeqsAssignment.assignment_items_sampling_size;//样本量（抽样后的双数）
                        decimal actualEvenNumber = aeqsAssignment.assignment_items_sampled_inspected;//实际双数

                        if (sampleSize > 0 && actualEvenNumber > 0)
                        {
                            Dictionary<int, decimal> evenNumbersDic = new Dictionary<int, decimal>();
                            if (sizeTable != null && sizeTable.Rows.Count > 0)
                            {
                                int datatable_index = 0;
                                foreach (DataRow item in sizeTable.Rows)
                                {
                                    //当前行的双数
                                    decimal curr_evenNumber = (Convert.ToDecimal(item["se_qty"].ToString()) / actualEvenNumber) * sampleSize;
                                    evenNumbersDic.Add(datatable_index, curr_evenNumber);
                                    datatable_index++;
                                }
                            }

                            //计算余数差值
                            int addOne_count = Convert.ToInt32(sampleSize - evenNumbersDic.Sum(x => Math.Floor(x.Value)));
                            //排序，打乱顺序，方便分配余数
                            evenNumbersDic = evenNumbersDic.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, y => y.Value);

                            int[] keys = evenNumbersDic.Keys.ToArray();
                            for (int i = 0; i < keys.Length; i++)
                            {
                                if (i < addOne_count)
                                {
                                    evenNumbersDic[keys[i]] = Math.Floor(evenNumbersDic[keys[i]]) + 1;
                                }
                                else
                                {
                                    evenNumbersDic[keys[i]] = Math.Floor(evenNumbersDic[keys[i]]);
                                }
                            }

                            foreach (var item in evenNumbersDic)
                            {
                                sizeTable.Rows[item.Key]["shuangshu"] = item.Value;
                            }
                        }
                        #endregion
                        //Project code: PO The changed orders are all FTWTRANS4M
                        string project_code = string.Empty;
                        string report_type_name = string.Empty;
                        if (IsOrderNewFormat(po, DB))
                        {
                            project_code = "FTWTRANS4M";
                            report_type_name = "FTW - AQL Outbound";
                            aeqsAssignment.assignment_items_po_line_po_number = DB.GetString($@"select customer_po from bdm_se_order_master where mer_po = '{po}'");
                        }
                        foreach (DataRow dr in sizeTable.Rows)
                        {

                            if (!string.IsNullOrEmpty(dr["ad_size"].ToString()))
                            {
                                aeqsAssignment.assignment_items_po_line_sku_sku_number = dr["ad_size"].ToString();
                            }
                            if (!string.IsNullOrEmpty(dr["ad_size"].ToString()) && !string.IsNullOrEmpty(project_code))
                            {
                                //查询行号 
                                aeqsAssignment.assignment_items_po_line_sku_sku_number = dr["ad_size"].ToString() + "_" + dr["PO_SEQ"]; //Edit on POChange: 2024 /07/12
                            }
                            if (!string.IsNullOrEmpty(dr["po_se_qty"].ToString()))
                            {
                                aeqsAssignment.assignment_items_po_line_qty = Convert.ToDecimal(dr["po_se_qty"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["se_qty"].ToString()))
                            {
                                aeqsAssignment.assignment_items_qty_inspected = Convert.ToDecimal(dr["se_qty"].ToString());
                                aeqsAssignment.assignment_items_qty_to_inspect = Convert.ToDecimal(dr["se_qty"].ToString());
                            }
                            if (!string.IsNullOrEmpty(dr["shuangshu"].ToString()))
                            {
                                aeqsAssignment.assignment_items_sampling_size = Convert.ToDecimal(dr["shuangshu"].ToString());
                                aeqsAssignment.assignment_items_sampled_inspected = Convert.ToDecimal(dr["shuangshu"].ToString());
                            }


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
                                    assignment_items_po_line_qty,
                                    assignment_items_assignment_report_type_id,
                                    po_line_project_code,
                                    report_type_name)
                                values
                                    (
                                    '{mid}',
                                    '{aeqsAssignment.assignment_items_sampled_inspected}',
                                    '{aeqsAssignment.assignment_items_inspection_result_id}',
                                    '{aeqsAssignment.assignment_items_inspection_status_id}',
                                    '{aeqsAssignment.assignment_items_qty_inspected}',
                                    to_date('{aeqsAssignment.assignment_items_inspection_completed_date.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss'),
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
                                    to_date('{aeqsAssignment.assignment_items_assignment_date_inspection.ToString("yyyy-MM-dd HH:mm:ss")}','yyyy-mm-dd hh24:mi:ss'),
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
                                    '{aeqsAssignment.assignment_items_po_line_qty}',
                                    '{aeqsAssignment.assignment_items_assignment_report_type_id}',
                                    '{project_code}',
                                    '{report_type_name}'
                                    )"; //Edit on POChange: 2024/06/12
                                   
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                    #endregion

                    #region 包装部分
                    foreach (AEQS_TO_P88_SECTIONS box in boxList)
                    {
                        string boxId = string.Empty;
                        try
                        {
                            //从序列获取id
                            boxId = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                        }
                        catch (Exception ex)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = ex.Message;
                            return ret;
                        }

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
                                  ('{boxId}',
                                   '{mid}',
                                   '{box.sections_type}',
                                   '{box.sections_title}',
                                   '{box.sections_result_id}',
                                   '{box.sections_qty_inspected}',
                                    {box.sections_sampled_inspected},
                                    {box.sections_defective_parts},
                                   '{box.sections_inspection_level}',
                                   '{box.sections_inspection_method}',
                                   '{box.sections_aql_minor}',
                                   '{box.sections_aql_major}',
                                   '{box.sections_aql_critical}',
                                   '{box.sections_barcodes_value}',
                                   '{box.sections_qty_type}',
                                   '{box.sections_max_minor_defects}',
                                   '{box.sections_max_major_defects}',
                                   '{box.sections_max_major_a_defects}',
                                   '{box.sections_max_major_b_defects}',
                                   '{box.sections_max_critical_defects}',
                                   '{box.sections_defects_label}',
                                   '{box.sections_defects_subsection}',
                                   '{box.sections_defects_code}',
                                   '{box.sections_defects_critical_level}',
                                   '{box.sections_defects_major_level}',
                                   '{box.sections_defects_minor_level}',
                                   '{box.sections_defects_comments}')";
                        DB.ExecuteNonQuery(sql);

                        //保存图片
                        if (!string.IsNullOrEmpty(box.defect_image))
                        {
                            string[] imageList = box.defect_image.Split(",");

                            int i = 1;
                            string imgSql = string.Empty;
                            foreach (string guid in imageList)
                            {
                                AEQS_TO_P88_SECTIONS_F boxPicture = new AEQS_TO_P88_SECTIONS_F();
                                boxPicture.sections_defects_pictures_title = "Photos " + i;
                                boxPicture.sections_defects_pictures_full_filename = guid;
                                boxPicture.sections_defects_pictures_number = i;
                                //boxPicture.sections_defects_pictures_comment = "N/A";

                                imgSql += $@"insert into T_aeqs_to_p88_sections_f (UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, sections_defects_pictures_full_filename, sections_defects_pictures_number, sections_defects_pictures_comment)
                                               values ('{boxId}', '{boxPicture.sections_defects_pictures_title}', '{boxPicture.sections_defects_pictures_full_filename}','{boxPicture.sections_defects_pictures_number}', '{boxPicture.sections_defects_pictures_comment}');";
                                i++;
                            }
                            DB.ExecuteNonQuery("begin " + imgSql + " end;");
                        }

                    }
                    #endregion

                    #region 产品
                    foreach (AEQS_TO_P88_SECTIONS product in productList)
                    {
                        string pid = string.Empty;
                        try
                        {
                            //从序列获取id
                            pid = DB.GetString("SELECT T_AEQS_TO_P88_SECTIONS_SEQ.nextval FROM dual");
                        }
                        catch (Exception ex)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = ex.Message;
                            return ret;
                        }

                        if (!string.IsNullOrEmpty(product.sections_defects_label))
                        {
                            product.sections_defects_label = product.sections_defects_label.Replace("'", "''");
                        }
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
                                  ('{pid}',
                                   '{mid}',
                                   '{product.sections_type}',
                                   '{product.sections_title}',
                                   '{product.sections_result_id}',
                                   '{product.sections_qty_inspected}',
                                    {product.sections_sampled_inspected},
                                    {product.sections_defective_parts},
                                   '{product.sections_inspection_level}',
                                   '{product.sections_inspection_method}',
                                   '{product.sections_aql_minor}',
                                   '{product.sections_aql_major}',
                                   '{product.sections_aql_critical}',
                                   '{product.sections_barcodes_value}',
                                   '{product.sections_qty_type}',
                                   '{product.sections_max_minor_defects}',
                                   '{product.sections_max_major_defects}',
                                   '{product.sections_max_major_a_defects}',
                                   '{product.sections_max_major_b_defects}',
                                   '{product.sections_max_critical_defects}',
                                   '{product.sections_defects_label}',
                                   '{product.sections_defects_subsection}',
                                   '{product.sections_defects_code}',
                                   '{product.sections_defects_critical_level}',
                                   '{product.sections_defects_major_level}',
                                   '{product.sections_defects_minor_level}',
                                   '{product.sections_defects_comments}')";
                        DB.ExecuteNonQuery(sql);

                        //保存图片
                        if (!string.IsNullOrEmpty(product.defect_image))
                        {
                            string[] imageList = product.defect_image.Split(",");

                            int i = 1;
                            string imgSql = string.Empty;
                            foreach (string guid in imageList)
                            {
                                AEQS_TO_P88_SECTIONS_F productPicture = new AEQS_TO_P88_SECTIONS_F();
                                productPicture.sections_defects_pictures_title = "Photos " + i;
                                productPicture.sections_defects_pictures_full_filename = guid;
                                productPicture.sections_defects_pictures_number = i;
                                //boxPicture.sections_defects_pictures_comment = "N/A";

                                imgSql += $@"insert into T_aeqs_to_p88_sections_f (UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, sections_defects_pictures_full_filename, sections_defects_pictures_number, sections_defects_pictures_comment)
                                               values ('{pid}', '{productPicture.sections_defects_pictures_title}', '{productPicture.sections_defects_pictures_full_filename}','{productPicture.sections_defects_pictures_number}', '{productPicture.sections_defects_pictures_comment}');";
                                i++;
                            }
                            DB.ExecuteNonQuery("begin " + imgSql + " end;");
                        }

                    }
                    #endregion

                    #region PassFail
                    List<AEQS_TO_P88_PASSFAIL> passFailList = new List<AEQS_TO_P88_PASSFAIL>();
                    sql = $@"select * from aql_cma_task_list_m_cd where task_no = '{task_no}'";
                    DataTable dataTable = DB.GetDataTable(sql);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            if (dr["conclusion_type"] != null)
                            {
                                AEQS_TO_P88_PASSFAIL passfail = new AEQS_TO_P88_PASSFAIL();
                                sql = string.Empty;

                                switch (dr["conclusion_type"].ToString())
                                {
                                    case "12"://Valid and signed MCS
                                        passfail.passfails_title = "mcs_availability_signature_compliance";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//Not checked
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//Checked
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//Not checked
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "1_general_compliance";
                                        sql = $@"insert into T_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "13"://SHAS
                                        passfail.passfails_title = "shas_compliance";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "1_general_compliance";
                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "5"://A-01
                                        passfail.passfails_title = "a_01_compliance";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "1_general_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    //case "8"://cpsi
                                    case "25"://cpsi
                                        passfail.passfails_title = "cpsia_compliance";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "1_general_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "10"://Customer/Country Special Requirements
                                        passfail.passfails_title = "customer_country_specific_compliance";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "1_general_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "14"://Mass production (finished shoes)
                                        passfail.passfails_title = "production_finish_goods";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "2_metal_detection_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "15"://Warehouse (outer box)
                                        passfail.passfails_title = "warehouse_outer_carton";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "2_metal_detection_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "6"://Mass production FGT qualified
                                        passfail.passfails_title = "finished_goods_testing_pass";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "3_fgt_compliance";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "17"://UVC processing
                                        passfail.passfails_title = "uv_c_treatment";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "4_mold_prevention";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "18"://Anti-mildew wrapping paper
                                        passfail.passfails_title = "anti_mold_wrapping_paper";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "4_mold_prevention";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "23"://moisture_control_box
                                        passfail.passfails_title = "moisture_control_box";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "4_mold_prevention";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";
                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "24"://moisture_control_product
                                        passfail.passfails_title = "moisture_control_product";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "4_mold_prevention";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "19"://exceptional_visual_standard
                                        passfail.passfails_title = "exceptional_visual_standard";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "5_exceptional_management";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "20"://工厂免责声明
                                        passfail.passfails_title = "factory_disclaimer";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "validation";
                                        passfail.passfails_checklistsubsection = "5_exceptional_management";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                    case "21"://一脚蹬
                                        passfail.passfails_title = "slip_on_inspection_pass_step_in_tool";
                                        if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("0"))
                                        {
                                            passfail.passfails_value = "NO";//未核对
                                            passfail.passfails_status = "fail";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("1"))
                                        {
                                            passfail.passfails_value = "YES";//已核对
                                            passfail.passfails_status = "pass";
                                        }
                                        else if (dr["conclusion"] != null && dr["conclusion"].ToString().Equals("2"))
                                        {
                                            passfail.passfails_value = "N/A";//未核对
                                            passfail.passfails_status = "na";
                                        }
                                        passfail.passfails_type = "check-list";
                                        passfail.passfails_subsection = "checklist";
                                        passfail.passfails_checklistsubsection = "1_fit";

                                        sql = $@"insert into t_aeqs_to_p88_passfail (UNION_ID, PASSFAILS_TITLE,PASSFAILS_VALUE, PASSFAILS_TYPE, PASSFAILS_SUBSECTION, PASSFAILS_CHECKLISTSUBSECTION, PASSFAILS_STATUS)
                                            values ('{mid}', '{passfail.passfails_title}', '{passfail.passfails_value}', '{passfail.passfails_type}', '{passfail.passfails_subsection}', '{passfail.passfails_checklistsubsection}', '{passfail.passfails_status}')";

                                        DB.ExecuteNonQuery(sql);
                                        break;
                                }

                            }

                        }

                    }
                    #endregion

                    #region 照片(产品0 测量1)
                    sql = $@"select image_type,image_index,file_guid,b.file_url
                                from aql_cma_task_list_m_pg_d a
                                left join bdm_upload_file_item b on a.file_guid = b.guid
                                where task_no = '{task_no}'";
                    DataTable pictureTable2 = DB.GetDataTable(sql);
                    if (pictureTable2.Rows.Count > 0)
                    {
                        sql = string.Empty;
                        foreach (DataRow dr in pictureTable2.Rows)
                        {
                            AEQS_TO_P88_SECTIONS_F picture2 = new AEQS_TO_P88_SECTIONS_F();
                            picture2.section_type = "pictures";
                            picture2.section_title = "photos";

                            //产品的第一张为“样本与实物对比照片”
                            if (dr["image_type"].ToString() == "0" && dr["image_index"].ToString() == "1")
                            {
                                picture2.sections_defects_pictures_title = "Compare Sample vs. Actual";
                            }
                            else
                            {
                                picture2.sections_defects_pictures_title = "Photos " + dr["image_index"].ToString();
                            }

                            if (dr["file_guid"] != null && !string.IsNullOrEmpty(dr["file_guid"].ToString()))
                            {
                                picture2.sections_defects_pictures_full_filename = dr["file_guid"].ToString();
                            }
                            picture2.sections_defects_pictures_number = Convert.ToInt32(dr["image_index"].ToString());
                            //picture2.sections_defects_pictures_comment = "N/A";

                            sql += $@"insert into t_aeqs_to_p88_sections_f ( UNION_ID, SECTIONS_DEFECTS_PICTURES_TITLE, SECTIONS_DEFECTS_PICTURES_FULL_FILENAME, SECTIONS_DEFECTS_PICTURES_NUMBER, SECTIONS_DEFECTS_PICTURES_COMMENT, SECTION_TYPE, SECTION_TITLE)
                                            values('{mid}', '{picture2.sections_defects_pictures_title}', '{picture2.sections_defects_pictures_full_filename}', '{picture2.sections_defects_pictures_number}', '{picture2.sections_defects_pictures_comment}', '{picture2.section_type}','{picture2.section_title}');";

                        }
                        DB.ExecuteNonQuery("begin " + sql + " end;");
                    }
                    #endregion
                }

                // 同步到记录表
                sql = $@"insert into t_aql_cma_task_p88_relation(task_no,p88_unique_key,createtime,creator) values('{task_no}','{mid}','{date + " " + time}','{user}')";
                DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
                DB.Commit();
            }
            catch (Exception ex)
            {
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
                DB.Rollback();
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
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

        public ResultObject GetClassifyNameByBadItemCode(object obj)
        {
            RequestObject reqObj = (RequestObject)obj;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();

            string badItemCode = JsonConvert.DeserializeObject<string>(reqObj.Data.ToString());

            try
            {
                DB = new DataBase(reqObj);
                string sql = $@"select c.bad_classify_name from  bdm_aql_bad_classify_d d
                                left join bdm_aql_bad_classify c on d.bad_classify_code = c.bad_classify_code
                                where d.bad_item_code = '{badItemCode}'";
                ret.RetData = DB.GetString(sql);
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
        /// 设置cpsia提交过的值给新的枚举值25
        /// </summary>
        /// <returns></returns>
        public ResultObject SetCpsiaData(object obj)
        {
            RequestObject reqObj = (RequestObject)obj;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();

            string Data = reqObj.Data.ToString();
            var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
            string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
            string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);//获取的登陆人信息
            string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
            string time = DateTime.Now.ToString("HH:mm:ss");//时间

            try
            {
                DB = new DataBase(reqObj);
                DB.Open();
                DB.BeginTransaction();

                //新枚举值25是否有数据
                string sql = $@"select count(1) from aql_cma_task_list_m_cd where task_no = '{task_no}' and  conclusion_type = '25' and conclusion is not null";
                int count = DB.GetInt32(sql);
                if (count <= 0)
                {
                    //如果新枚举值没有数据，并且就枚举值曾经提交过，则插入一行新枚举值的记录。
                    sql = $@"select task_no,conclusion,conclusion_type from aql_cma_task_list_m_cd where task_no = '{task_no}' and conclusion_type = '8' and conclusion is not null";
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        string conclusion = dt.Rows[0]["conclusion"].ToString();
                        sql = $@"insert into aql_cma_task_list_m_cd(conclusion,  conclusion_type, createby, createdate, createtime, task_no)
values ('{conclusion}',  '25', '{user}', '{date}', '{time}', '{task_no}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                DB.Commit();
            }
            catch (Exception ex)
            {
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
        /// 获取该订单的合并标记，以及对照表信息，以及点箱记录源数据。
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static ResultObject CheckIsMergeOrder(object OBJ)
        {

            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                string Data = reqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";

                string sql = $@"select so_mergr_mark from bdm_se_order_master where mer_po = '{mer_po}'";
                string merge_mark = DB.GetString(sql);

                //string reference_sql = $@"select r.mer_po,size_no,po_line_item,po_line_qty 
                //                            from t_bdm_se_order_reference r
                //                            inner join bdm_se_order_master m on r.mer_po = m.mer_po
                //                            where m.mer_po = '{mer_po}' --and m.so_mergr_mark = 'Y'
                //                            order by size_no,po_line_item";
                //DataTable referenceTable = DB.GetDataTable(reference_sql);

                string pointbox_sql = $@"select task_no, case_no, cr_size, se_qty, createby, po_line_item 
                                            from aql_cma_task_list_m_pb where task_no  = '{task_no}'
                                            order by cr_size";
                DataTable pbReferenceTable = DB.GetDataTable(pointbox_sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("merge_mark", merge_mark);
                dic.Add("pbReferenceTable", pbReferenceTable);
                //dic.Add("referenceTable", referenceTable);

                ret.RetData = JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;

        }

        public static ResultObject CheckFinalTask(object OBJ)
        {

            RequestObject reqObj = (RequestObject)OBJ;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(reqObj);
                string Data = reqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string sql = $@"select check_data_status,
       carton_number_status,
       photo_upload_status,
       humidity_entry_status,
       ba_entry_status,
       aql_entry_status
  from aql_pivot_data_status
 where task_no = '{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                List<string> pending = new List<string>();
                DataRow row = dt.Rows[0];
                if (row["check_data_status"].ToString() == "0")
                    pending.Add("First Page Data is Not Saved");

                if (row["carton_number_status"].ToString() == "0")
                    pending.Add("Carton Data is Not Submitted");

                if (row["photo_upload_status"].ToString() == "0")
                    pending.Add("Photos are not Submitted");

                //if (row["humidity_entry_status"].ToString() == "0")
                //    pending.Add("Humidity data not enterd");

                //if (row["ba_entry_status"].ToString() == "0")
                //    pending.Add("BA Entry not submitted");
               
                if (pending.Count > 0)
                {
                    ret.ErrMsg = "Please complete the following before submission:\n• "
                          + string.Join("\n• ", pending);
                    ret.IsSuccess = false;
                }
                else
                {
                    ret.IsSuccess = true;
                }
                   
            }
            catch (Exception ex)
            {
                ret.ErrMsg = ex.Message;
                ret.IsSuccess = false;
            }
            return ret;

        }
    }
}
