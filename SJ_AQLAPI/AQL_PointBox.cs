using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_PointBox
    {
        /// <summary>
        /// 查询-点箱-根据AQL级别获取抽样比例
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLPointBox_SamplingRate(object OBJ)
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

                string where = string.Empty;
                string sql = $@"select VALS, {ac} as AC from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                DataTable dt = DB.GetDataTable(sql);

                //查询 AC12 1.5 AC13 2.5
                sql = $@"select VALS,AC12,AC13 from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}' and to_number(START_QTY)<={num} and to_number(END_QTY)>={num}";
                DataTable dtAC1213 = DB.GetDataTable(sql);

                sql = $@"select VALS,to_number(START_QTY) as START_QTY,to_number(END_QTY) as END_QTY from BDM_AQL_M where HORI_TYPE='2' and LEVEL_TYPE='{LEVEL_TYPE}'";
                DataTable dt2 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);
                dic.Add("Data1213", dtAC1213);

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
        /// 查询-点箱
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPointBox(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string task_type = jarr.ContainsKey("task_type") ? jarr["task_type"].ToString() : "";//任务类型
                string sql = string.Empty;
                if (task_type == "Manually_Created")
                {
                    //int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_pb where task_no='{task_no}'");
                    //if (count <= 0)
                    //{
                    //    InsertPointBox(OBJ);
                    //}
                    #region Before PO change2 on 2025/02/13
                    //                    sql = $@"SELECT
                    //	MAX(p.id) as id,
                    //	MAX(p.task_no) as task_no,
                    //	MAX(p.case_no) as case_no,
                    //	MAX(p.cr_size) as cr_size,
                    //	MAX(p.se_qty) as se_qty,
                    //	SUM(s.se_qty) AS size_qty
                    //FROM
                    //	aql_cma_task_list_m_pb p
                    //	INNER JOIN aql_cma_task_list_m a ON p.TASK_NO = a.TASK_NO
                    //	INNER JOIN BDM_SE_ORDER_MASTER m ON m.mer_po = a.po
                    //	INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
                    //	AND m.SE_ID = s.SE_ID 
                    //	AND p.cr_size = s.SIZE_NO 
                    //WHERE
                    //	a.po = '{po}' 
                    //	AND a.TASK_NO = '{task_no}'
                    //	GROUP BY s.SIZE_NO
                    //	ORDER BY MAX(s.SIZE_SEQ)";
                    //                }
                    //                else
                    //                {
                    //                    sql = $@"SELECT
                    //	MAX(p.id) as id,
                    //	MAX(p.task_no) as task_no,
                    //	MAX(p.case_no) as case_no,
                    //	MAX(p.cr_size) as cr_size,
                    //	MAX(p.se_qty) as se_qty,
                    //	SUM(s.se_qty) AS size_qty
                    //FROM
                    //	aql_cma_task_list_m_pb p
                    //	INNER JOIN aql_cma_task_list_m a ON p.TASK_NO = a.TASK_NO
                    //	INNER JOIN BDM_SE_ORDER_MASTER m ON m.mer_po = a.po
                    //	INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
                    //	AND m.SE_ID = s.SE_ID 
                    //	AND p.cr_size = s.SIZE_NO 
                    //WHERE
                    //	a.po = '{po}' 
                    //	AND a.TASK_NO = '{task_no}'
                    //	GROUP BY s.SIZE_NO
                    //	ORDER BY MAX(s.SIZE_SEQ)";
                    //                }
                    #endregion
                    #region After PO change2 on 2025/02/13
                    sql = $@"SELECT
	MAX(p.id) as id,
	MAX(p.task_no) as task_no,
	MAX(p.case_no) as case_no,
	MAX(p.cr_size) as cr_size,
	sum(p.se_qty) as se_qty,
	max(s.se_qty) AS size_qty
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
	MAX(p.case_no) as case_no,
	MAX(p.cr_size) as cr_size,
	sum(p.se_qty) as se_qty,
	max(s.se_qty) AS size_qty
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
                #endregion
                DataTable dt = DB.GetDataTable(sql);

                sql = $@"SELECT
pb_state
FROM
aql_cma_task_list_m 
WHERE TASK_NO='{task_no}'";
                DataTable dt2 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);
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
        /// 新增-初始化点箱
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPointBox(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                DataTable dt = DB.GetDataTable($@"SELECT
	                                            bb.CR_SIZE,
	                                            SUM( bb.SE_QTY ) SE_QTY 
                                            FROM
	                                            (
	                                            SELECT
		                                            s.SIZE_NO as CR_SIZE,
		                                            s.SE_QTY,
			                                            s.SIZE_SEQ
	                                            FROM
		                                            BDM_SE_ORDER_MASTER m
		                                            INNER JOIN BDM_SE_ORDER_SIZE s ON m.ORG_ID = s.ORG_ID 
		                                            AND m.SE_ID = s.SE_ID 
	                                            WHERE
		                                            m.MER_PO = '{po}' 
	                                            ) bb 
                                            GROUP BY bb.CR_SIZE 
                                            ORDER BY  MAX(bb.SIZE_SEQ)
                                            ");
                foreach (DataRow item in dt.Rows)
                {
                    sql += $@"insert into aql_cma_task_list_m_pb (task_no,cr_size,createby,createdate,createtime) 
                            values('{task_no}','{item["CR_SIZE"]}','{user}','{date}','{time}');";
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");
                DB.Commit();
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
        /// 保存-点箱-样本/aql级别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPointBox_level(object OBJ)
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
                string sample_level = jarr.ContainsKey("sample_level") ? jarr["sample_level"].ToString() : "";//样本级别
                string aql_level = jarr.ContainsKey("aql_level") ? jarr["aql_level"].ToString() : "";//aql级别
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_aql_m where task_no='{task_no}'");
                if (count <= 0)
                    sql = $@"insert into aql_cma_task_list_m_aql_m (task_no,sample_level,aql_level,createby,createdate,createtime) 
                        values('{task_no}','{sample_level}','{aql_level}','{user}','{date}','{time}')";
                else
                    sql = $@"update aql_cma_task_list_m_aql_m set sample_level='{sample_level}',aql_level='{aql_level}',modifyby='{user}',
                            modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
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
        /// 保存-点箱-点箱完成/取消点箱完成
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPointBox_Complete(object OBJ)
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
                string state = jarr.ContainsKey("state") ? jarr["state"].ToString() : "";//点箱完成状态
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                sql = $@"update aql_cma_task_list_m set pb_state='{state}',CHECKER='{user}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                #region Added by Ashok to Update Carton Entry status for Pivot sync
                string cartonsql = $@"UPDATE AQL_PIVOT_DATA_STATUS
SET carton_number_status  = 1,
    modified_by = {user},
    modified_at = SYSDATE
WHERE task_no = '{task_no}'";
                DB.ExecuteNonQuery(cartonsql);
                #endregion

                DB.Commit();
                string pb_state = DB.GetString($@"select pb_state from aql_cma_task_list_m where task_no='{task_no}'");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("pb_state", pb_state);
                dic.Add("CHECKER", DB.GetString($@"select staff_name from hr001m where staff_no='{user}'"));
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
        /// 保存-点箱
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPointBox(object OBJ)
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
                string inspection_type = jarr.ContainsKey("inspection_type") ? jarr["inspection_type"].ToString() : "";//检验类型
                string shoe_type = jarr.ContainsKey("shoe_type") ? jarr["shoe_type"].ToString() : "";//新旧鞋型
                string inspection_date = jarr.ContainsKey("inspection_date") ? jarr["inspection_date"].ToString() : "";//检验时间
                string lot_num = jarr.ContainsKey("lot_num") ? jarr["lot_num"].ToString() : "";//分批数量
                DataTable list_m_pb_l = jarr.ContainsKey("list_m_pb_l") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["list_m_pb_l"].ToString()) : null;//条件 点箱左边
                DataTable list_m_pb_r = jarr.ContainsKey("list_m_pb_r") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["list_m_pb_r"].ToString()) : null;//条件 点箱右边
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sqll = string.Empty;
                string sqlr = string.Empty;

                DB.ExecuteNonQuery($@"update aql_cma_task_list_m set inspection_type='{inspection_type}',shoe_type='{shoe_type}',lot_num='{lot_num}',inspection_date=to_date('{inspection_date}','yyyy-mm-dd'),is_inspection='1' where task_no='{task_no}'");

                if (list_m_pb_l!=null && list_m_pb_l.Rows.Count>0)
                {
                    foreach (DataRow item in list_m_pb_l.Rows)
                    {   
                        //Commented for PO Change2 on 2025/02/13
                        //sqll += $@"update aql_cma_task_list_m_pb set case_no='{item["箱号"]}',se_qty='{item["订单量"]}',modifyby='{user}',
                        //        modifydate='{date}',modifytime='{time}' where id='{item["id"]}';";
                        sqll += $@"update aql_cma_task_list_m_pb set case_no='{item["箱号"]}',modifyby='{user}',
                                modifydate='{date}',modifytime='{time}' where id='{item["id"]}';";
                    }
                    if (!string.IsNullOrWhiteSpace(sqll))
                        DB.ExecuteNonQuery($@"BEGIN {sqll} END;");
                }
                if (list_m_pb_r!= null && list_m_pb_r.Rows.Count > 0)
                {
                    foreach (DataRow item in list_m_pb_r.Rows)
                    {    
                        //Commented for PO Change2 on 2025/02/13
                        //sqlr += $@"update aql_cma_task_list_m_pb set case_no='{item["箱号2"]}',se_qty='{item["订单量2"]}',modifyby='{user}',
                        //        modifydate='{date}',modifytime='{time}' where id='{item["id2"]}';";
                        sqlr += $@"update aql_cma_task_list_m_pb set case_no='{item["箱号2"]}',modifyby='{user}',
                                modifydate='{date}',modifytime='{time}' where id='{item["id2"]}';";
                    }
                    if (!string.IsNullOrWhiteSpace(sqlr))
                        //DB.ExecuteNonQuery($@"BEGIN {sqlr} END;");  commented by Ashok on 20250217
                    DB.ExecuteNonQuery($@"BEGIN {sqlr} END;");
                }

                DB.Commit();
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
        /// 打印-查询-点箱
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPointBox_Print(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string sql = string.Empty;
                    sql = $@"
SELECT
	MAX(p.id) as id,
	MAX(p.task_no) as task_no,
	MAX(p.case_no) as case_no,
	MAX(p.cr_size) as cr_size,
	MAX(p.se_qty) as se_qty,
	SUM(s.se_qty) AS size_qty
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
        /// 保存-点箱-头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPointBox_title(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string inspection_type = jarr.ContainsKey("inspection_type") ? jarr["inspection_type"].ToString() : "";//检验类型
                string inspection_date = jarr.ContainsKey("inspection_date") ? jarr["inspection_date"].ToString() : "";//检验时间
                string shoe_type = jarr.ContainsKey("shoe_type") ? jarr["shoe_type"].ToString() : "";//新旧鞋型
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                //判断po是否存在
                int count = DB.GetInt32($@"SELECT
                        count(1)
                    FROM

                        BDM_SE_ORDER_MASTER a

                        LEFT JOIN BDM_SE_ORDER_ITEM b ON a.SE_ID = b.SE_ID

                        LEFT JOIN BDM_RD_STYLE c on b.shoe_no = c.shoe_no
                        where a.mer_po = '{po}'");
                if (count<=0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "PO号不存在!";
                    return ret;
                }

                int counttask = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_d_m where task_no='{task_no}'");
                if (counttask<=0)
                    sql = $@"insert into aql_cma_task_list_m_d_m (task_no,po,inspection_type,inspection_date,shoe_type,createby,createdate,createtime) 
                         values('{task_no}','{po}','{inspection_type}','{inspection_date}','{shoe_type}','{user}','{date}','{time}')";
                else
                    sql = $@"update aql_cma_task_list_m_d_m set po='{po}',inspection_type='{inspection_type}',inspection_date='{inspection_date}',
                            shoe_type='{shoe_type}',modifyby='{user}',modifydate='{date}',modifytime='{time}'";
                DB.ExecuteNonQuery(sql);

                DB.Commit();
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
        /// 查询-点箱-头
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPointBox_title(object OBJ)
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

                sql = $@"SELECT
m.inspection_type,
m.inspection_date,
m.is_inspection,
m.shoe_type,
	a.MER_PO,
	b.PROD_NO,
	b.SE_QTY,
	c.name_t AS shoe_name,
    m.lot_num
FROM
	aql_cma_task_list_m m
	LEFT JOIN BDM_SE_ORDER_MASTER a ON m.po = a.MER_PO
	LEFT JOIN BDM_SE_ORDER_ITEM b ON a.SE_ID = b.SE_ID
	LEFT JOIN BDM_RD_STYLE c ON b.shoe_no = c.shoe_no
	WHERE m.TASK_NO='{task_no}'";

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
        /// 生成校验任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GenerateVerificationTask(object OBJ)
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
                string original_task_no = jarr.ContainsKey("original_task_no") ? jarr["original_task_no"].ToString() : "";//原任务编号
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string INSPECTION_TYPE = jarr.ContainsKey("INSPECTION_TYPE") ? jarr["INSPECTION_TYPE"].ToString() : "";//点箱完成状态

                #region 非空校验
                if (string.IsNullOrEmpty(original_task_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“原任务编号”不能为空";
                    return ret;
                }
                if (string.IsNullOrEmpty(INSPECTION_TYPE))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“检验类型”不能为空";
                    return ret;
                }
                if (string.IsNullOrEmpty(po))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“po”不能为空";
                    return ret;
                }
                #endregion

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");//日期
                string time = currTime.ToString("HH:mm:ss");//时间
                DB.Open();
                DB.BeginTransaction();

                string new_task_no = DB.GetStringline($@"
SELECT * FROM (
SELECT TASK_NO FROM AQL_CMA_TASK_LIST_M WHERE PO='{po}' ORDER BY ID DESC
) TEMP_AQL_CMA_TASK_LIST_M 
WHERE ROWNUM=1
");
                if (string.IsNullOrEmpty(original_task_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "“原任务编号”不存在";
                    return ret;
                }
                else
                {
                    int index = 0;
                    if (new_task_no.Contains("-"))
                    {
                        index = Convert.ToInt32(new_task_no.Replace(po + "-", "")) + 1;
                    }
                    else
                    {
                        index = Convert.ToInt32(new_task_no.Replace(po, "")) + 1;
                    }
                    

                    new_task_no = string.Concat(po,"-", index);
                }

                // aql_cma_task_list_m 复制
                //Dictionary<string, object> aql_cma_task_list_m_dic_select = new Dictionary<string, object>();
                //aql_cma_task_list_m_dic_select.Add("TASK_NO", original_task_no);
                string aql_cma_task_list_m_sql = $@"SELECT * FROM aql_cma_task_list_m WHERE TASK_NO='{original_task_no}'";
                var aql_cma_task_list_m_dt = DB.GetDataTablebyline(aql_cma_task_list_m_sql);

                if (aql_cma_task_list_m_dt.Rows.Count > 0)
                {
                    int index = 0;
                    Dictionary<string, object> aql_cma_task_list_m_dic_insert = new Dictionary<string, object>();
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow item in aql_cma_task_list_m_dt.Rows)
                    {
                        aql_cma_task_list_m_dic_insert.Add($@"TASK_NO_{index}", new_task_no);
                        aql_cma_task_list_m_dic_insert.Add($@"PO_{index}", item["PO"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"ART_NO_{index}", item["ART_NO"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"PO_NUM_{index}", item["PO_NUM"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"LOT_NUM_{index}", item["LOT_NUM"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"ORDER_LEVEL_{index}", item["ORDER_LEVEL"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"FULL_STATE_{index}", item["FULL_STATE"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"INSPECTION_STATE_{index}", "0");//验货状态 未验货：0
                        aql_cma_task_list_m_dic_insert.Add($@"TASK_TYPE_{index}", "1");// 任务类型 0：自动生成 1：手动创建
                        aql_cma_task_list_m_dic_insert.Add($@"CREATEBY_{index}", user);
                        aql_cma_task_list_m_dic_insert.Add($@"CREATEDATE_{index}", date);
                        aql_cma_task_list_m_dic_insert.Add($@"CREATETIME_{index}", time);
                        aql_cma_task_list_m_dic_insert.Add($@"PB_STATE_{index}", "0");//点箱完成状态 0：可编辑 1：不可编辑
                        aql_cma_task_list_m_dic_insert.Add($@"EFFECTIVE_STATUS_{index}", "0");//生效状态 0：生效 1：失效
                        aql_cma_task_list_m_dic_insert.Add($@"INSPECTION_TYPE_{index}", INSPECTION_TYPE);
                        aql_cma_task_list_m_dic_insert.Add($@"SHOE_TYPE_{index}", item["SHOE_TYPE"].ToString());
                        aql_cma_task_list_m_dic_insert.Add($@"ORIGINAL_TASK_NO_{index}", original_task_no);

                        string INSPECTION_DATE_STR = "";
                        DateTime INSPECTION_DATE_Date = new DateTime();
                        if (DateTime.TryParse(item["INSPECTION_DATE"].ToString(), out INSPECTION_DATE_Date))
                        {
                            INSPECTION_DATE_STR = INSPECTION_DATE_Date.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        sb.AppendLine($@"
INSERT INTO AQL_CMA_TASK_LIST_M (
	TASK_NO,
	PO,
	ART_NO,
	PO_NUM,
	LOT_NUM,
	ORDER_LEVEL,
	FULL_STATE,
	INSPECTION_STATE,
	TASK_TYPE,
	CREATEBY,
	CREATEDATE,
	CREATETIME,
	PB_STATE,
	EFFECTIVE_STATUS,
	INSPECTION_TYPE,
	INSPECTION_DATE,
	SHOE_TYPE,
	ORIGINAL_TASK_NO
)
VALUES
	(
		@TASK_NO_{index},
		@PO_{index},
		@ART_NO_{index},
		@PO_NUM_{index},
		@LOT_NUM_{index},
		@ORDER_LEVEL_{index},
		@FULL_STATE_{index},
		@INSPECTION_STATE_{index},
		@TASK_TYPE_{index},
		@CREATEBY_{index},
		@CREATEDATE_{index},
		@CREATETIME_{index},
		@PB_STATE_{index},
		@EFFECTIVE_STATUS_{index},
		@INSPECTION_TYPE_{index},
		TO_DATE (
			'{INSPECTION_DATE_STR}',
			'SYYYY-MM-DD HH24:MI:SS'
		),
		@SHOE_TYPE_{index},
		@ORIGINAL_TASK_NO_{index}
	);
");

                        index++;
                    }
                    sb.AppendLine($@"
UPDATE aql_cma_task_list_m SET EFFECTIVE_STATUS='1' WHERE TASK_NO='{original_task_no}';
");
                    DB.ExecuteNonQuery($@"
begin
{sb}
end;
", aql_cma_task_list_m_dic_insert);
                }

                // aql_cma_task_list_m_aql_m 复制
                //Dictionary<string, object> aql_cma_task_list_m_aql_m_dic_select = new Dictionary<string, object>();
                //aql_cma_task_list_m_aql_m_dic_select.Add("TASK_NO", original_task_no);
                string aql_cma_task_list_m_aql_m_sql = $@"SELECT * FROM aql_cma_task_list_m_aql_m WHERE TASK_NO='{original_task_no}'";
                var aql_cma_task_list_m_aql_m_dt = DB.GetDataTablebyline(aql_cma_task_list_m_aql_m_sql);
                if (aql_cma_task_list_m_aql_m_dt.Rows.Count > 0)
                {
                    int index = 0;
                    Dictionary<string, object> aql_cma_task_list_m_aql_m_dic_insert = new Dictionary<string, object>();
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow item in aql_cma_task_list_m_aql_m_dt.Rows)
                    {
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"TASK_NO_{index}", new_task_no);
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"SAMPLE_LEVEL_{index}", item["SAMPLE_LEVEL"].ToString());
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"AQL_LEVEL_{index}", item["AQL_LEVEL"].ToString());
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"CREATEBY_{index}", user);
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"CREATEDATE_{index}", date);
                        aql_cma_task_list_m_aql_m_dic_insert.Add($@"CREATETIME_{index}", time);
                        sb.AppendLine($@"
INSERT INTO AQL_CMA_TASK_LIST_M_AQL_M (
	TASK_NO,
	SAMPLE_LEVEL,
	AQL_LEVEL,
	CREATEBY,
	CREATEDATE,
	CREATETIME
)
VALUES
	(
		@TASK_NO_{index},
		@SAMPLE_LEVEL_{index},
		@AQL_LEVEL_{index},
		@CREATEBY_{index},
		@CREATEDATE_{index},
		@CREATETIME_{index}
	);
");

                        index++;
                    }
                    if (sb.Length > 0)
                    {
                        DB.ExecuteNonQuery($@"begin {sb} end;", aql_cma_task_list_m_aql_m_dic_insert);
                    }
                }

                // aql_cma_task_list_m_pb 复制
                //Dictionary<string, object> aql_cma_task_list_m_pb_dic_select = new Dictionary<string, object>();
                //aql_cma_task_list_m_pb_dic_select.Add("TASK_NO", original_task_no);
                string aql_cma_task_list_m_pb_sql = $@"SELECT * FROM aql_cma_task_list_m_pb WHERE TASK_NO='{original_task_no}'";
                var aql_cma_task_list_m_pb_dt = DB.GetDataTablebyline(aql_cma_task_list_m_pb_sql);
                if (aql_cma_task_list_m_pb_dt.Rows.Count > 0)
                {
                    int index = 0;
                    Dictionary<string, object> aql_cma_task_list_m_pb_dic_insert = new Dictionary<string, object>();
                    StringBuilder sb = new StringBuilder();
                    foreach (DataRow item in aql_cma_task_list_m_pb_dt.Rows)
                    {
                        aql_cma_task_list_m_pb_dic_insert.Add($@"TASK_NO_{index}", new_task_no);
                        aql_cma_task_list_m_pb_dic_insert.Add($@"CASE_NO_{index}", "");
                        aql_cma_task_list_m_pb_dic_insert.Add($@"CR_SIZE_{index}", item["CR_SIZE"].ToString());
                        aql_cma_task_list_m_pb_dic_insert.Add($@"SE_QTY_{index}", item["SE_QTY"].ToString());
                        aql_cma_task_list_m_pb_dic_insert.Add($@"CREATEBY_{index}", user);
                        aql_cma_task_list_m_pb_dic_insert.Add($@"CREATEDATE_{index}", date);
                        aql_cma_task_list_m_pb_dic_insert.Add($@"CREATETIME_{index}", time);
                        sb.AppendLine($@"
INSERT INTO AQL_CMA_TASK_LIST_M_PB (
	TASK_NO,
	CASE_NO,
	CR_SIZE,
	SE_QTY,
	CREATEBY,
	CREATEDATE,
	CREATETIME
)
VALUES
	(
		@TASK_NO_{index},
		@CASE_NO_{index},
		@CR_SIZE_{index},
		@SE_QTY_{index},
		@CREATEBY_{index},
		@CREATEDATE_{index},
		@CREATETIME_{index}
	);
");

                        index++;
                    }
                    if (sb.Length > 0)
                    {
                        DB.ExecuteNonQuery($@"begin {sb} end;", aql_cma_task_list_m_pb_dic_insert);
                    }
                }

                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = "生成成功";
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
        /// 获取检验状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPointBox_AQLEDITSTATE(object OBJ)
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

                sql = $@"SELECT AQL_EDIT_STATE FROM AQL_CMA_TASK_LIST_M WHERE TASK_NO = '{task_no}'";

                string AQL_EDIT_STATE = DB.GetString(sql);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(AQL_EDIT_STATE);
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
