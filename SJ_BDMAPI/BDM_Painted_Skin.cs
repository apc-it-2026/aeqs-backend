using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Painted_Skin
    {
        /// <summary>
        /// 画皮主页查询状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Main_State(object OBJ)
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

                string sql = $@"SELECT * FROM( 
                                SELECT
	                                enum_code,
	                                enum_value 
                                FROM
	                                sys001m 
                                WHERE
	                                enum_type = 'enum_task_state' UNION
                                SELECT
	                                'qb' AS enum_code,
	                                'All' AS enum_value --all--全部
                                FROM
	                                sys001m)t ORDER BY enum_code desc";
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
        /// 画皮主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Main(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string task_state = jarr.ContainsKey("task_state") ? jarr["task_state"].ToString() : "";//查询条件 状态
                string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";//查询条件 范围开始时间
                string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";//查询条件 范围结束时间
                string vend_name = jarr.ContainsKey("vend_name") ? jarr["vend_name"].ToString() : "";//查询条件 生产厂商
                string item_name = jarr.ContainsKey("item_name") ? jarr["item_name"].ToString() : "";//查询条件 材料名称
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(item_no))
                {
                    where += $@" and m.item_no like @item_no ";
                }
                if (!string.IsNullOrWhiteSpace(vend_name))
                {
                    where += $@" and m.vend_name like @vend_name ";
                }
                if (!string.IsNullOrWhiteSpace(item_name))
                {
                    where += $@" and m.item_name like @item_name ";
                }

                if (!string.IsNullOrWhiteSpace(wh_date_start) && !string.IsNullOrWhiteSpace(wh_date_end))
                {
                    where += $@" and m.wh_date_start >= @wh_date_start and  m.wh_date_end<=@wh_date_end";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_date_start))
                    {
                        where += $@" and m.wh_date_start >= @wh_date_start ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_date_end))
                    {
                        where += $@" and m.wh_date_end <= @wh_date_end' ";
                    }
                }

                if (!string.IsNullOrWhiteSpace(task_state))
                {
                    if (task_state != "qb")
                    {
                        where += $@" and m.task_state=@task_state";
                    }
                }
                string sql = $@"SELECT
                                m.task_no,
                                MAX(m.item_no) as item_no,
                                MAX(m.item_name) as item_name,
                                MAX(m.vend_no) as vend_no,
                                MAX(m.vend_name) as vend_name,
                                TO_CHAR(MAX(m.wh_date_start)) ||'-'|| TO_CHAR(MAX(m.wh_date_end)) as wh_date,
                                MAX(NVL( m.mtl_qty, 0)) as mtl_qty,
                                SUM(NVL(d.qty, 0) )as yhp_qty,
                                MAX(s.enum_value) as task_state
                                FROM
	                                qcm_hp_task_m m
	                                LEFT JOIN qcm_hp_task_d d on m.task_no=d.task_no and d.isdelete='0'
	                                LEFT JOIN SYS001M s on m.task_state=s.enum_code and enum_type='enum_task_state'
                                    where m.isdelete='0' {where}
	                                GROUP BY m.task_no
                                    order by MAX(to_date(m.CREATEDATE||' '||m.CREATETIME,'yyyy-MM-dd HH24:mi:ss')) desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("item_no", $@"%{item_no}%");
                paramTestDic.Add("vend_name", $@"%{vend_name}%");
                paramTestDic.Add("item_name", $@"%{item_name}%");
                paramTestDic.Add("wh_date_start", $@"{wh_date_start}");
                paramTestDic.Add("wh_date_end", $@"{wh_date_end}");
                paramTestDic.Add("task_state", $@"{task_state}");
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
        /// 新增画皮查询材料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Insert_item(object OBJ)
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
                string keyWord = jarr.ContainsKey("keyWord") ? jarr["keyWord"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(keyWord))
                {
                    where += $@" and (b.ITEM_NO like @keyWord or b.NAME_T like @keyWord or m.SUPPLIERS_NAME like @keyWord)";
                }

                string sql = $@"select	                            
                             b.ITEM_NO,
	                         b.NAME_T,
	                         m.SUPPLIERS_NAME
                         FROM
	                         bdm_rd_item b
                          LEFT JOIN BASE003M m on b.VEND_NO_PRD=m.SUPPLIERS_CODE
                                where 1=1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keyWord", $@"%{keyWord}%");
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
        /// 画皮新增
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPainted_Skin_Insert(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";//查询条件 进仓时间（开始）
                string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";//查询条件 进仓时间（结束）
                string mtl_qty = jarr.ContainsKey("mtl_qty") ? jarr["mtl_qty"].ToString() : "";//查询条件 材料数量
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                string dmd = DateTime.Now.ToString("yyyyMMdd");
                string task_no = DB.GetString($@"select MAX(task_no) as task_no from qcm_hp_task_m where task_no like '{dmd}%'");
                task_no = task_no == "" ? dmd + "0001" : (Convert.ToDouble(task_no) + 1).ToString();
                if (string.IsNullOrWhiteSpace(item_no))
                {
                    ret.ErrMsg = "Item number is empty!";
                    ret.IsSuccess = false;
                    return ret;
                }

                DataTable valuedt = DB.GetDataTable($@"SELECT
	                                                b.VEND_NO,
	                                                b.NAME_T,
	                                                m.SUPPLIERS_NAME
                                                FROM
	                                                bdm_rd_item b
                                                 LEFT JOIN BASE003M m on b.VEND_NO=m.SUPPLIERS_CODE
	                                                where b.ITEM_NO='{item_no}'");
                if (valuedt.Rows.Count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "There is no data under this item number!";//该料号下无数据
                    return ret;
                }
                foreach (DataRow item in valuedt.Rows)
                {
                    sql = $@"insert into qcm_hp_task_m (task_no,item_no,item_name,vend_no,vend_name,wh_date_start,wh_date_end,mtl_qty,task_state,createby,createdate,createtime) 
                        values('{task_no}','{item_no}','{item["NAME_T"]}','{item["VEND_NO"]}','{item["SUPPLIERS_NAME"]}','{wh_date_start}','{wh_date_end}','{mtl_qty}','0','{user}','{date}','{time}')";
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
        /// 画皮删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPainted_Skin_Delete(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = $@"update qcm_hp_task_m set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_d set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra_l set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra_r set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// 画皮操作查询等级
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_level(object OBJ)
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
                //string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"select enum_code,enum_value from sys001m where enum_type='enum_pl_level' order by enum_code";
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
        /// 画皮操作页面头查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_Head(object OBJ)
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
                string sql = $@"SELECT
                                m.task_no,
                                MAX(m.item_no) as item_no,
                                MAX(m.item_name) as item_name,
                                MAX(m.vend_no) as vend_no,
                                MAX(m.vend_name) as vend_name,
                                TO_CHAR(MAX(m.wh_date_start)) ||'-'|| TO_CHAR(MAX(m.wh_date_end)) as wh_date,
                                MAX(NVL( m.mtl_qty, 0)) as mtl_qty,
                                SUM(NVL(d.qty, 0) )as yhp_qty,
                                MAX(s.enum_value) as task_state
                                FROM
	                                qcm_hp_task_m m
	                                LEFT JOIN qcm_hp_task_d d on m.task_no=d.task_no and d.isdelete='0'
	                                LEFT JOIN SYS001M s on m.task_state=s.enum_code and enum_type='enum_task_state'
                                    where m.isdelete='0' and m.task_no=@task_no
	                                GROUP BY m.task_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
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
        /// 画皮操作页面个人汇总查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_HZ(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //转译
                //string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = $@"SELECT
	                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty
                            FROM
	                            SYS001M s
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.createby =@createby and d.task_no=@task_no
                            WHERE
	                            ENUM_TYPE = 'enum_pl_level' 
	                            GROUP BY s.ENUM_CODE
                                ORDER BY s.ENUM_CODE asc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("createby", user);
                paramTestDic.Add("task_no", task_no);
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
        /// 画皮操作页面个人画皮记录查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_JL(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = $@"SELECT
                                d.id as did,
                                s.enum_value as pl_level,
                                d.qty,
                                d.createdate
                                FROM
	                                qcm_hp_task_d d 
	                                JOIN SYS001M s on d.pl_level=s.ENUM_CODE and s.ENUM_TYPE = 'enum_pl_level'
	                                where d.createby =@createby and d.isdelete='0' and d.task_no=@task_no
                                    ORDER BY d.id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("createby", $@"{user}");
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
        /// 画皮操作页面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Edit_Delete(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 d表id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update qcm_hp_task_d set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{did}'";
                DB.ExecuteNonQuery(sql);
                string count = DB.GetStringline($@"SELECT
	                                                count( 1 ) 
                                                FROM
	                                                qcm_hp_task_d d 
	                                                JOIN qcm_hp_task_m m on d.TASK_NO=m.TASK_NO
                                                WHERE
	                                                d.isdelete = '0' 
	                                                AND d.TASK_NO=(SELECT TASK_NO from qcm_hp_task_d where id='{did}')");
                if (count == "0")
                {
                    string task_no = DB.GetString($@"select task_no from qcm_hp_task_d where id='{did}'");
                    sql = $@"update qcm_hp_task_m set task_state='0' where task_no='{task_no}'";
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
        /// 画皮操作页面新增
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Edit_Insert(object OBJ)
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
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//查询条件 数量（面积）
                string pl_level = jarr.ContainsKey("pl_level") ? jarr["pl_level"].ToString() : "";//查询条件 皮料等级
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"insert into qcm_hp_task_d (task_no,qty,pl_level,createby,createdate,createtime) 
                        values('{task_no}','{qty}','{pl_level}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                sql = $@"update qcm_hp_task_m set task_state='1' where task_no='{task_no}'";
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
        /// 画皮查看进度页面画皮汇总查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_HZ(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息

                string sql = string.Empty;

                sql = $@"SELECT * FROM (
                                SELECT
                                MAX(s.enum_code) as enum_code,
	                                MAX(s.enum_value) as enum_value,
	                                SUM(NVL( d.qty, 0)) as qty,
	                                MAX(s.enum_value2) as coefficient,
	                                (SUM(NVL( d.qty, 0))*MAX(s.enum_value2)) as multiple
                                FROM
	                                SYS001M s 
	                                LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                                WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                                GROUP BY s.ENUM_CODE
	                                UNION
	                                SELECT
	                                '5' as enum_code,
	                                'I~V总和' as enum_value,
	                                SUM(NVL( d.qty, 0)) as qty,
	                                '-' as coefficient,
	                                SUM((NVL( d.qty, 0)*s.enum_value2)) as multiple
	                                FROM
	                                SYS001M s 
	                                LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                                WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                                )t ORDER BY t.enum_code";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt1 = DB.GetDataTable(sql, paramTestDic);

                sql = $@"SELECT
	                    MAX(s.enum_value) as enum_value,
	                    SUM(NVL( d.qty, 0)) as qty
                    FROM
	                    SYS001M s 
	                    LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                    WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('5','6')
	                    GROUP BY s.ENUM_CODE
	                    ORDER BY s.enum_code";
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("task_no", $@"{task_no}");
                DataTable dt2 = DB.GetDataTable(sql, paramTestDic2);

                Dictionary<string, object> paramTestDic3 = new Dictionary<string, object>();
                paramTestDic3.Add("task_no", $@"{task_no}");
                DataTable pecftdt = DB.GetDataTable($@"
	                                            SELECT
		                                            SUM( NVL( d.qty, 0 ) ) AS qty,
		                                            SUM( ( NVL( d.qty, 0 ) * s.enum_value2 ) ) AS multiple 
	                                            FROM
		                                            SYS001M s
		                                            LEFT JOIN qcm_hp_task_d d ON s.ENUM_CODE = d.pl_level 
		                                            AND d.isdelete = '0' 
		                                            AND d.task_no = @task_no
	                                            WHERE
		                                            ENUM_TYPE = 'enum_pl_level' 
	                                            AND s.enum_code IN ( '0', '1', '2', '3', '4' ) 
	                                            ", paramTestDic3);

                string pecft = string.Empty;
                foreach (DataRow item in pecftdt.Rows)
                {
                    if (item["qty"].ToString() == "0" || item["multiple"].ToString() == "0")
                    {
                        pecft = "0";
                    }
                    else
                    {
                        pecft = Math.Round(Convert.ToDecimal(item["multiple"]) / Convert.ToDecimal(item["qty"]) * 100, 2).ToString();
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt1);
                dic.Add("Data1", dt2);
                dic.Add("pecft", pecft);

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
        /// 画皮查看进度页面页面头查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_Head(object OBJ)
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
                string sql = $@"SELECT
                                m.task_no,
                                MAX(m.item_no) as item_no,
                                MAX(m.item_name) as item_name,
                                MAX(m.vend_no) as vend_no,
                                MAX(m.vend_name) as vend_name,
                                TO_CHAR(MAX(m.wh_date_start)) ||'-'|| TO_CHAR(MAX(m.wh_date_end)) as wh_date,
                                MAX(NVL( m.mtl_qty, 0)) as mtl_qty,
                                SUM(NVL(d.qty, 0) )as yhp_qty,
                                MAX(s.enum_value) as task_state
                                FROM
	                                qcm_hp_task_m m
	                                LEFT JOIN qcm_hp_task_d d on m.task_no=d.task_no and d.isdelete='0'
	                                LEFT JOIN SYS001M s on m.task_state=s.enum_code and enum_type='enum_task_state'
                                    where m.isdelete='0' and m.task_no=@task_no
	                                GROUP BY m.task_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
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
        /// 画皮查看进度页面画皮记录者查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_Staff(object OBJ)
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
                //string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = $@"SELECT
	                            d.createby ,
	                            MAX(h.staff_name) as staff_name
                            FROM
	                            qcm_hp_task_d d
                             LEFT JOIN HR001M h on d.CREATEBY=h.STAFF_NO
                                where d.isdelete='0'
                            GROUP BY
	                            d.createby";
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
        /// 画皮查看进度页面画皮记录查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_task_d(object OBJ)
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
                string createby = jarr.ContainsKey("createby") ? jarr["createby"].ToString() : "";//查询条件 画皮记录者
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(createby))
                {
                    where = $@" and d.CREATEBY=@createby";
                }
                if (createby == "全部0318")
                {
                    sql = $@"SELECT
                                s.enum_value as pl_level,
                                d.qty,
                                d.createby,
                                h.staff_name,
                                TO_CHAR(d.createdate)||' '|| TO_CHAR(d.createtime) as wh_date
                                FROM
	                                qcm_hp_task_d d
	                                LEFT JOIN HR001M h on d.CREATEBY=h.staff_no
                                    LEFT JOIN SYS001M s on d.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
	                                where d.isdelete='0' and d.task_no=@task_no
	                                ORDER BY d.id desc";
                }
                else
                {
                    sql = $@"SELECT
                                s.enum_value as pl_level,
                                d.qty,
                                d.createby,
                                h.staff_name,
                                TO_CHAR(d.createdate)||' '|| TO_CHAR(d.createtime) as wh_date
                                FROM
	                                qcm_hp_task_d d
	                                LEFT JOIN HR001M h on d.CREATEBY=h.staff_no
                                    LEFT JOIN SYS001M s on d.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
	                                where d.isdelete='0' and d.task_no=@task_no {where}
	                                ORDER BY d.id desc";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                paramTestDic.Add("createby", $@"{createby}");
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
        /// 画皮查看进度页完成画皮
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_List_Complete(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update qcm_hp_task_m set task_state='2',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// 画皮查看进度页取消完成画皮
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_List_CancelComplete(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update qcm_hp_task_m set task_state='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                DB.ExecuteNonQuery($@"delete from qcm_hp_task_satra where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from qcm_hp_task_satra_l where task_no='{task_no}'");
                DB.ExecuteNonQuery($@"delete from qcm_hp_task_satra_r where task_no='{task_no}'");
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
        /// 皮料评估报表页面头查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Report_Head(object OBJ)
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
                string sql = string.Empty;
                sql = $@"SELECT 
                        MAX(m.item_no) as item_no,
                        MAX(m.item_name) as item_name,
                        SUM(NVL(d.qty, 0)) as qty,
                        MAX(m.CREATEDATE) as CREATEDATE,
                        MAX(r.ITEM_TYPE_NAME) as ITEM_TYPE_NAME,
                        MAX(m.mtl_qty) as mtl_qty,
                        MAX(m.vend_name) as vend_name   
                        FROM
                        qcm_hp_task_m m
                        LEFT JOIN qcm_hp_task_d d on m.TASK_NO=d.TASK_NO and d.isdelete='0'
                        LEFT JOIN BDM_RD_ITEM t on m.ITEM_NO=t.ITEM_NO
                        LEFT JOIN BDM_RD_ITEMTYPE r on t.ITEM_TYPE=r.ITEM_TYPE_NO
                        WHERE m.TASK_NO=@task_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                sql = $@"select pl_qty,supplier,approver,tabulator,area_diff_cft,pur_qty_cft,avg_use_rate,assessment from qcm_hp_task_satra where isdelete='0' and task_no=@task_no";
                DataTable dt1 = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1", dt1);

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
        /// 皮料评估报表页面画皮记录查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Report_task_d(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                int count = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_r where isdelete='0' and task_no=@task_no", paramTestDic);
                if (count <= 0)
                {
                    sql = $@"SELECT * FROM (
                            SELECT
                            0 as id,
                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            MAX(s.enum_value2) as coefficient,
	                            (SUM(NVL( d.qty, 0))*MAX(s.enum_value2)) as multiple,
                                'false' as isinput,
	                            'false' as istotal
                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO=@task_no
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                            GROUP BY s.ENUM_CODE
	                            UNION
	                            SELECT
                                0 as id,
	                            '4.5' as enum_code,
	                            'I~V总和' as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            '-' as coefficient,
	                            SUM((NVL( d.qty, 0)*s.enum_value2)) as multiple,
                                'false' as isinput,
	                            'true' as istotal
	                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO=@task_no
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                            UNION
	                            SELECT
                                0 as id,
	                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            MAX(NVL(s.enum_value2, '-'))  as coefficient,
	                            0 as multiple,
                                'true' as isinput,
	                            'false' as istotal
	                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO=@task_no
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('5','6')
	                            GROUP BY s.ENUM_CODE
	                            )t ORDER BY t.enum_code";
                }
                else
                {
                    sql = $@"SELECT * FROM(
                                SELECT 
                                MAX(r.id) as id,
                                MAX(s.enum_value) as enum_value,
                                MAX(r.multiple) as multiple,
                                MAX(r.coefficient) as coefficient,
                                MAX(r.qty) as qty,
                                MAX(r.pl_level) as enum_code,
                                'false' as isinput,
                                'false' as istotal
                                FROM
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('0','1','2','3','4') and r.isdelete='0' and r.TASK_NO=@task_no
																GROUP BY r.pl_level
                                UNION
                                SELECT 
                                MAX(r.id) as id,
                                MAX(s.enum_value) as enum_value,
                                MAX(r.multiple) as multiple,
                                MAX(r.coefficient) as coefficient,
                                MAX(r.qty) as qty,
                                MAX(r.pl_level) as enum_code,
                                'true' as isinput,
                                'false' as istotal
                                FROM
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('5','6')  and r.isdelete='0' and r.TASK_NO=@task_no
																GROUP BY r.pl_level
                                UNION
                                SELECT 
                                0 as id,
                                'I~V总和' as ENUM_VALUE,
                                SUM((NVL( r.qty, 0)*s.enum_value2)) as multiple,
                                '-' as coefficient,
                                SUM(NVL( r.qty, 0)) as qty,
                                '4.5' as enum_code,
                                'false' as isinput,
                                'true' as istotal
                                FROM 
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('0','1','2','3','4') and r.isdelete='0' and r.TASK_NO=@task_no
                                 )t ORDER BY enum_code";
                }


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
        /// 皮料评估报表页面面积抽检查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Report_satra_l(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = string.Empty;

                sql = $@"
select sorting,MAX(supplier_area) as supplier_area,MAX(actual_area) as actual_area from qcm_hp_task_satra_l where isdelete='0' and task_no=@task_no GROUP BY sorting ORDER BY  sorting";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
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
        /// 皮料评估报表页面保存
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Report_Edit(object OBJ)
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
                Dictionary<string, string> tabHead = jarr.ContainsKey("tabHead") ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jarr["tabHead"].ToString()) : new Dictionary<string, string>();//表头数据
                DataTable qcm_hp_task_satra_l = jarr.ContainsKey("qcm_hp_task_satra_l") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["qcm_hp_task_satra_l"].ToString()) : new DataTable();//qcm_hp_task_satra_l
                DataTable qcm_hp_task_satra_r = jarr.ContainsKey("qcm_hp_task_satra_r") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["qcm_hp_task_satra_r"].ToString()) : new DataTable();//qcm_hp_task_satra_r
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                int countHead = DB.GetInt32($@"select count(1) from qcm_hp_task_satra where task_no='{task_no}' and isdelete ='0'");
                if (countHead > 0)
                {
                    sql = $@"update qcm_hp_task_satra set pl_qty='{tabHead["pl_qty"]}',supplier='{tabHead["supplier"]}',approver='{tabHead["approver"]}',
                             tabulator='{tabHead["tabulator"]}',area_diff_cft='{tabHead["area_diff_cft"]}',pur_qty_cft='{tabHead["pur_qty_cft"]}',
                             avg_use_rate='{tabHead["avg_use_rate"]}',assessment='{tabHead["assessment"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                            where task_no='{task_no}' and isdelete='0'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"insert into qcm_hp_task_satra (task_no,pl_qty,supplier,approver,tabulator,area_diff_cft,pur_qty_cft,avg_use_rate,assessment,
                             createby,createdate,createtime)
                             values('{task_no}','{tabHead["pl_qty"]}','{tabHead["supplier"]}','{tabHead["approver"]}','{tabHead["tabulator"]}','{tabHead["area_diff_cft"]}',
                             '{tabHead["pur_qty_cft"]}','{tabHead["avg_use_rate"]}','{tabHead["assessment"]}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }

                int countsatra_l = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_l where task_no='{task_no}' and isdelete ='0'");
                if (countsatra_l > 0)
                {
                    foreach (DataRow item in qcm_hp_task_satra_l.Rows)
                    {
                        if (item["xh"].ToString() != "合计")
                        {
                            sql = $@"update qcm_hp_task_satra_l set sorting='{item["xh"]}',supplier_area='{item["gys_area"]}',actual_area='{item["sj_area"]}',
                                    modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}' and sorting='{item["xh"]}' and isdelete='0'";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }
                else
                {
                    foreach (DataRow item in qcm_hp_task_satra_l.Rows)
                    {
                        if (item["xh"].ToString() != "合计")
                        {
                            sql = $@"insert into qcm_hp_task_satra_l (task_no,sorting,supplier_area,actual_area,createby,createdate,createtime)
                                     values('{task_no}','{item["xh"]}','{item["gys_area"]}','{item["sj_area"]}',
                                     '{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }

                int countsatra_r = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_r where task_no='{task_no}' and isdelete='0'");
                if (countsatra_r > 0)
                {
                    foreach (DataRow item in qcm_hp_task_satra_r.Rows)
                    {
                        if (Convert.ToDecimal(item["id"].ToString()) > 0)
                        {
                            string coefficient = item["coefficient"].ToString() == "-" ? "0" : item["coefficient"].ToString().TrimEnd('%');
                            coefficient = (Convert.ToDecimal(coefficient) / 100).ToString();
                            sql = $@"update qcm_hp_task_satra_r set multiple='{item["multiple"]}',coefficient='{coefficient}',
                                     qty='{item["qty"]}',pl_level='{item["enum_code"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}'
                                     where isdelete='0' and task_no='{task_no}' and id='{item["id"]}'";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }
                else
                {
                    foreach (DataRow item in qcm_hp_task_satra_r.Rows)
                    {
                        if (item["istotal"].ToString() != "true")
                        {
                            string coefficient = item["coefficient"].ToString() == "-" ? "0" : item["coefficient"].ToString().TrimEnd('%');
                            coefficient = (Convert.ToDecimal(coefficient) / 100).ToString();
                            sql = $@"insert into qcm_hp_task_satra_r (task_no,multiple,coefficient,qty,pl_level,createby,createdate,createtime)
                                     values('{task_no}','{item["multiple"]}','{coefficient}','{item["qty"]}','{item["enum_code"]}',
                                     '{user}','{date}','{time}' )";
                            DB.ExecuteNonQuery(sql);
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

        //---------------------------------------------

        /// <summary>
        /// 画皮主页查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Main_PDA(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";//查询条件 范围开始时间
                string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";//查询条件 范围结束时间
                string keyWord = jarr.ContainsKey("keyWord") ? jarr["keyWord"].ToString() : "";//查询条件 keyWord
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(item_no))
                {
                    where += $@" and item_no like @item_no ";
                    paramTestDic.Add("item_no", $@"%{item_no}%");
                }

                if (!string.IsNullOrEmpty(keyWord))
                {
                    where += $@" and (task_no like @keyWord_task_no or item_name like @keyWord_item_name or vend_name like @keyWord_vend_name or 
                               mtl_qty like @keyWord_mtl_qty or yhp_qty like @keyWord_yhp_qty or task_state like @keyWord_task_state )";
                    paramTestDic.Add("keyWord_task_no", $@"%{keyWord}%");
                    paramTestDic.Add("keyWord_item_name", $@"%{keyWord}%");
                    paramTestDic.Add("keyWord_vend_name", $@"%{keyWord}%");
                    paramTestDic.Add("keyWord_mtl_qty", $@"%{keyWord}%");
                    paramTestDic.Add("keyWord_yhp_qty", $@"%{keyWord}%");
                    paramTestDic.Add("keyWord_task_state", $@"%{keyWord}%");
                }

                if (!string.IsNullOrWhiteSpace(wh_date_start) && !string.IsNullOrWhiteSpace(wh_date_end))
                {
                    where += $@" and wh_date_start >= @wh_date_start and  wh_date_end<=@wh_date_end";
                    paramTestDic.Add("wh_date_start", $@"{wh_date_start}");
                    paramTestDic.Add("wh_date_end", $@"{wh_date_end}");
                }
                string sql = $@"SELECT * FROM(
                                SELECT
                                MAX(to_date(m.CREATEDATE||' '||m.CREATETIME,'yyyy-MM-dd HH24:mi:ss')) as createdate,
                                MAX(m.id) as id,
                                m.task_no,
                                MAX(m.item_no) as item_no,
                                MAX(m.item_name) as item_name,
                                MAX(m.vend_no) as vend_no,
                                MAX(m.vend_name) as vend_name,
								MAX(m.wh_date_start) as wh_date_start,
								MAX(m.wh_date_end) as wh_date_end,
                                TO_CHAR(MAX(m.wh_date_start)) ||'-'|| TO_CHAR(MAX(m.wh_date_end)) as wh_date,
                                MAX(NVL( m.mtl_qty, 0)) as mtl_qty,
                                SUM(NVL(d.qty, 0) )as yhp_qty,
                                MAX(s.enum_value) as task_state,
																CASE MAX(s.enum_value)
																WHEN '已完成' THEN
																	0
																	WHEN '初始化' THEN
																	1
																	WHEN '进行中' THEN
																	1
																ELSE
																	1
															END isbaogao,
															CASE MAX(s.enum_value)
																WHEN '已完成' THEN
																	1
																	WHEN '初始化' THEN
																	0
																	WHEN '进行中' THEN
																	1
																ELSE
																	1
															END isdelete
                                FROM
	                                qcm_hp_task_m m
	                                LEFT JOIN qcm_hp_task_d d on m.task_no=d.task_no and d.isdelete='0'
	                                LEFT JOIN SYS001M s on m.task_state=s.enum_code and enum_type='enum_task_state'
                                    where m.isdelete='0'
	                                GROUP BY m.task_no)t where 1=1 {where} ORDER BY createdate desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

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
        /// 画皮主页删除PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Main_Delete_PDA(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                if (string.IsNullOrWhiteSpace(task_no))
                {
                    ret.ErrMsg = "Task number is empty!";
                    ret.IsSuccess = false;
                    return ret;
                }

                sql = $@"update qcm_hp_task_m set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_d set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra_l set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"update qcm_hp_task_satra_r set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// 新增画皮查询材料PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Insert_item_PDA(object OBJ)
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
                string keyWord = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "1";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(keyWord))
                {
                    where += $@" and (b.ITEM_NO like @keyWord or b.NAME_T like @keyWord or m.SUPPLIERS_NAME like @keyWord)";
                }

                string sql = $@"select	                            
                             b.ITEM_NO,
	                         b.NAME_T,
	                         m.SUPPLIERS_NAME
                         FROM
	                         bdm_rd_item b
                          LEFT JOIN BASE003M m on b.VEND_NO_PRD=m.SUPPLIERS_CODE
                                where 1=1 {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keyWord", $@"%{keyWord}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

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
        /// 画皮新增PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertPainted_Skin_Insert_PDA(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";//查询条件 进仓时间（开始）
                string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";//查询条件 进仓时间（结束）
                string mtl_qty = jarr.ContainsKey("mtl_qty") ? jarr["mtl_qty"].ToString() : "";//查询条件 材料数量
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                string dmd = DateTime.Now.ToString("yyyyMMdd");
                string task_no = DB.GetString($@"select MAX(task_no) as task_no from qcm_hp_task_m where task_no like '{dmd}%'");
                task_no = task_no == "" ? dmd + "0001" : (Convert.ToDouble(task_no) + 1).ToString();
                if (string.IsNullOrWhiteSpace(item_no))
                {
                    ret.ErrMsg = "Item number is empty!";
                    ret.IsSuccess = false;
                    return ret;
                }

                DataTable valuedt = DB.GetDataTable($@"SELECT
	                                                b.VEND_NO,
	                                                b.NAME_T,
	                                                m.SUPPLIERS_NAME
                                                FROM
	                                                bdm_rd_item b
                                                 LEFT JOIN BASE003M m on b.VEND_NO=m.SUPPLIERS_CODE
	                                                where b.ITEM_NO='{item_no}'");
                if (valuedt.Rows.Count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "There is no data under this item number!";
                    return ret;
                }
                foreach (DataRow item in valuedt.Rows)
                {
                    sql = $@"insert into qcm_hp_task_m (task_no,item_no,item_name,vend_no,vend_name,wh_date_start,wh_date_end,mtl_qty,task_state,createby,createdate,createtime) 
                        values('{task_no}','{item_no}','{item["NAME_T"]}','{item["VEND_NO"]}','{item["SUPPLIERS_NAME"]}','{wh_date_start}','{wh_date_end}','{mtl_qty}','0','{user}','{date}','{time}')";
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
        /// 画皮操作页面个人汇总查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_HZ_PDA(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //转译
                //string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = $@"SELECT
	                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty
                            FROM
	                            SYS001M s
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.createby =@createby and d.task_no=@task_no
                            WHERE
	                            ENUM_TYPE = 'enum_pl_level' 
	                            GROUP BY s.ENUM_CODE
                                ORDER BY s.ENUM_CODE asc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("createby", $@"{user}");
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt = DB.GetDataTable(sql,paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

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
        /// 画皮操作页面个人画皮记录查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Edit_JL_PDA(object OBJ)
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
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string sql = $@"SELECT
                                d.id as did,
                                s.enum_value as pl_level,
                                d.qty,
                                d.createdate
                                FROM
	                                qcm_hp_task_d d 
	                                JOIN SYS001M s on d.pl_level=s.ENUM_CODE and s.ENUM_TYPE = 'enum_pl_level'
	                                where d.createby =@createby and d.isdelete='0' and d.task_no=@task_no
                                    ORDER BY d.id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("createby", $@"{user}");
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

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
        /// 画皮操作页面新增PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Edit_Insert_PDA(object OBJ)
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
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//查询条件 数量（面积）
                string pl_level = jarr.ContainsKey("pl_level") ? jarr["pl_level"].ToString() : "";//查询条件 皮料等级
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                sql = $@"insert into qcm_hp_task_d (task_no,qty,pl_level,createby,createdate,createtime) 
                        values('{task_no}','{qty}','{pl_level}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);

                sql = $@"update qcm_hp_task_m set task_state='1' where task_no='{task_no}'";
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
        /// 画皮操作页面删除PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Edit_Delete_PDA(object OBJ)
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
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString() : "";//查询条件 d表id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update qcm_hp_task_d set isdelete='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{did}'";
                DB.ExecuteNonQuery(sql);
                string count = DB.GetStringline($@"SELECT
	                                                count( 1 ) 
                                                FROM
	                                                qcm_hp_task_d d 
	                                                JOIN qcm_hp_task_m m on d.TASK_NO=m.TASK_NO
                                                WHERE
	                                                d.isdelete = '0' 
	                                                AND d.TASK_NO=(SELECT TASK_NO from qcm_hp_task_d where id='{did}')");
                if (count == "0")
                {
                    string task_no = DB.GetString($@"select task_no from qcm_hp_task_d where id='{did}'");
                    sql = $@"update qcm_hp_task_m set task_state='0' where task_no='{task_no}'";
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
        /// 画皮查看进度页面画皮汇总查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_HZ_PDA(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息

                string sql = string.Empty;

                sql = $@"SELECT * FROM (
                                SELECT
                                MAX(s.enum_code) as enum_code,
	                                MAX(s.enum_value) as enum_value,
	                                SUM(NVL( d.qty, 0)) as qty,
	                                MAX(s.enum_value2) as coefficient,
	                                (SUM(NVL( d.qty, 0))*MAX(s.enum_value2)) as multiple
                                FROM
	                                SYS001M s 
	                                LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                                WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                                GROUP BY s.ENUM_CODE
	                                UNION
	                                SELECT
	                                '5' as enum_code,
	                                'I~V总和' as enum_value,
	                                SUM(NVL( d.qty, 0)) as qty,
	                                '-' as coefficient,
	                                SUM((NVL( d.qty, 0)*s.enum_value2)) as multiple
	                                FROM
	                                SYS001M s 
	                                LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                                WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                                )t ORDER BY t.enum_code";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                DataTable dt1 = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt1.Rows)
                {
                    if (item["enum_code"].ToString() != "5")
                    {
                        item["coefficient"] = (Convert.ToDecimal(item["coefficient"]) * 100).ToString() + "%";
                    }
                }


                sql = $@"SELECT
	                    MAX(s.enum_value) as enum_value,
	                    SUM(NVL( d.qty, 0)) as qty
                    FROM
	                    SYS001M s 
	                    LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                    WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('5','6')
	                    GROUP BY s.ENUM_CODE
	                    ORDER BY s.enum_code";
                DataTable dt2 = DB.GetDataTable(sql, paramTestDic);
                DataTable pecftdt = DB.GetDataTable($@"SELECT
                                                SUM( NVL( d.qty, 0 ) ) AS qty,
		                                            SUM( ( NVL( d.qty, 0 ) * s.enum_value2 ) ) AS multiple 
                                                FROM
	                                                SYS001M s 
	                                                LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.task_no=@task_no
	                                                WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')", paramTestDic);
                string pecft = string.Empty;
                foreach (DataRow item in pecftdt.Rows)
                {
                    if (item["qty"].ToString() == "0" || item["multiple"].ToString() == "0")
                    {
                        pecft = "0";
                    }
                    else
                    {
                        pecft = Math.Round(Convert.ToDecimal(item["multiple"]) / Convert.ToDecimal(item["qty"]) * 100, 2).ToString();
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt1);
                dic.Add("Data1", dt2);
                dic.Add("pecft", pecft);

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
        /// 画皮查看进度页面画皮记录查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_List_task_d_PDA(object OBJ)
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
                string createby = jarr.ContainsKey("createby") ? jarr["createby"].ToString() : "";//查询条件 画皮记录者
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//查询条件 任务编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(createby))
                {
                    where = $@" and d.CREATEBY=@createby";
                }
                if (string.IsNullOrEmpty(createby))
                {
                    sql = $@"SELECT
                                s.enum_value as pl_level,
                                d.qty,
                                d.createby,
                                h.staff_name,
                                TO_CHAR(d.createdate)||' '|| TO_CHAR(d.createtime) as wh_date
                                FROM
	                                qcm_hp_task_d d
	                                LEFT JOIN HR001M h on d.CREATEBY=h.staff_no
                                    LEFT JOIN SYS001M s on d.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
	                                where d.isdelete='0' and d.task_no=@task_no
	                                ORDER BY d.id desc";
                }
                else
                {
                    sql = $@"SELECT
                                s.enum_value as pl_level,
                                d.qty,
                                d.createby,
                                h.staff_name,
                                TO_CHAR(d.createdate)||' '|| TO_CHAR(d.createtime) as wh_date
                                FROM
	                                qcm_hp_task_d d
	                                LEFT JOIN HR001M h on d.CREATEBY=h.staff_no
                                    LEFT JOIN SYS001M s on d.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
	                                where d.isdelete='0' and d.task_no=@task_no {where}
	                                ORDER BY d.id desc";
                }

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                paramTestDic.Add("createby", $@"{createby}");
                DataTable dtstaff = DB.GetDataTable($@"SELECT
	                            d.createby ,
	                            MAX(h.staff_name) as staff_name
                            FROM
	                            qcm_hp_task_d d
                             LEFT JOIN HR001M h on d.CREATEBY=h.STAFF_NO
                                where d.isdelete='0' and d.TASK_NO=@task_no
                            GROUP BY
	                            d.createby", paramTestDic);

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("DataStaff", dtstaff);
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
        /// 画皮查看进度页完成画皮PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_List_Complete_PDA(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update qcm_hp_task_m set task_state='2',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}'";
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
        /// 皮料评估报表页面查询PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPainted_Skin_Report_task_d_PDA(object OBJ)
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
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("task_no", $@"{task_no}");
                paramTestDic.Add("createby", $@"{user}");
                int count = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_r where isdelete='0' and task_no='{paramTestDic["task_no"]}'", paramTestDic);
                if (count <= 0)
                {
                    sql = $@"SELECT * FROM (
                            SELECT
                            0 as id,
                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            MAX(s.enum_value2) as coefficient,
	                            (SUM(NVL( d.qty, 0))*MAX(s.enum_value2)) as multiple,
                                'false' as isinput,
	                            'false' as istotal
                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO='{paramTestDic["task_no"]}' 
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                            GROUP BY s.ENUM_CODE
	                            UNION
	                            SELECT
                                0 as id,
	                            '4.5' as enum_code,
	                            'I~V总和' as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            '-' as coefficient,
	                            SUM((NVL( d.qty, 0)*s.enum_value2)) as multiple,
                                'false' as isinput,
	                            'true' as istotal
	                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO='{paramTestDic["task_no"]}' 
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
	                            UNION
	                            SELECT
                                0 as id,
	                            MAX(s.enum_code) as enum_code,
	                            MAX(s.enum_value) as enum_value,
	                            SUM(NVL( d.qty, 0)) as qty,
	                            MAX(NVL(s.enum_value2, '-'))  as coefficient,
	                            0 as multiple,
                                'true' as isinput,
	                            'false' as istotal
	                            FROM
	                            SYS001M s 
	                            LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0' and d.TASK_NO='{paramTestDic["task_no"]}' 
	                            WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('5','6')
	                            GROUP BY s.ENUM_CODE
	                            )t ORDER BY t.enum_code";
                }
                else
                {
                    sql = $@"SELECT * FROM(
                                SELECT 
                                MAX(r.id) as id,
                                MAX(s.enum_value) as enum_value,
                                MAX(r.multiple) as multiple,
                                MAX(r.coefficient) as coefficient,
                                MAX(r.qty) as qty,
                                MAX(r.pl_level) as enum_code,
                                'false' as isinput,
                                'false' as istotal
                                FROM
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('0','1','2','3','4') and r.isdelete='0' and r.TASK_NO='{paramTestDic["task_no"]}'
                                GROUP BY r.pl_level
                                UNION
                                SELECT 
                                MAX(r.id) as id,
                                MAX(s.enum_value) as enum_value,
                                MAX(r.multiple) as multiple,
                                MAX(r.coefficient) as coefficient,
                                MAX(r.qty) as qty,
                                MAX(r.pl_level) as enum_code,
                                'true' as isinput,
                                'false' as istotal
                                FROM
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('5','6')  and r.isdelete='0' and r.TASK_NO='{paramTestDic["task_no"]}'
                                GROUP BY r.pl_level
                                UNION
                                SELECT 
                                0 as id,
                                'I~V总和' as ENUM_VALUE,
                                SUM((NVL( r.qty, 0)*s.enum_value2)) as multiple,
                                '-' as coefficient,
                                SUM(NVL( r.qty, 0)) as qty,
                                '4.5' as enum_code,
                                'false' as isinput,
                                'true' as istotal
                                FROM 
                                qcm_hp_task_satra_r r LEFT JOIN SYS001M s on r.PL_LEVEL=s.enum_code and s.enum_type='enum_pl_level'
                                WHERE PL_LEVEL in ('0','1','2','3','4') and r.isdelete='0' and r.TASK_NO='{paramTestDic["task_no"]}'
                                 )t ORDER BY enum_code";
                }
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    if (item["coefficient"].ToString() != "-")
                    {
                        item["coefficient"] = (Convert.ToDecimal(item["coefficient"]) * 100).ToString() + "%";
                    }
                }

                DataTable dtlist = new DataTable();
                int countl = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_l where task_no='{paramTestDic["task_no"]}'", paramTestDic);
                if (countl > 0)
                {
                    sql = $@"select sorting,MAX(supplier_area) as supplier_area,MAX(actual_area) as actual_area from qcm_hp_task_satra_l where isdelete='0' and task_no='{paramTestDic["task_no"]}' GROUP BY sorting ORDER BY sorting";
                    dtlist = DB.GetDataTable(sql, paramTestDic);
                }
                else
                {
                    dtlist.Columns.Add("SORTING");
                    dtlist.Columns.Add("SUPPLIER_AREA");
                    dtlist.Columns.Add("ACTUAL_AREA");
                    for (int i = 0; i < 20; i++)
                    {
                        dtlist.Rows.Add();
                        dtlist.Rows[i]["SORTING"] = i + 1;
                        dtlist.Rows[i]["SUPPLIER_AREA"] = "";
                        dtlist.Rows[i]["ACTUAL_AREA"] = "";
                    }
                }

                decimal count1 = 0;
                foreach (DataRow item in dtlist.Rows)
                {
                    count1 += item["supplier_area"].ToString() == "" ? 0 : Convert.ToDecimal(item["supplier_area"].ToString());
                }
                decimal count2 = 0;
                foreach (DataRow item in dtlist.Rows)
                {
                    count2 += item["actual_area"].ToString() == "" ? 0 : Convert.ToDecimal(item["actual_area"].ToString());
                }

                dtlist.Rows.Add();
                dtlist.Rows[dtlist.Rows.Count - 1]["sorting"] = "21";
                dtlist.Rows[dtlist.Rows.Count - 1]["supplier_area"] = count1;
                dtlist.Rows[dtlist.Rows.Count - 1]["actual_area"] = count2;


                sql = $@"SELECT 
                        MAX(m.item_no) as item_no,
                        MAX(m.item_name) as item_name,
                        SUM(NVL(d.qty, 0)) as qty,
                        MAX(m.CREATEDATE) as CREATEDATE,
                        MAX(r.ITEM_TYPE_NAME) as ITEM_TYPE_NAME
                        FROM
                        qcm_hp_task_m m
                        LEFT JOIN qcm_hp_task_d d on m.TASK_NO=d.TASK_NO and d.isdelete='0'
                        LEFT JOIN BDM_RD_ITEM t on m.ITEM_NO=t.ITEM_NO
                        LEFT JOIN BDM_RD_ITEMTYPE r on t.ITEM_TYPE=r.ITEM_TYPE_NO
                        WHERE m.TASK_NO='{paramTestDic["task_no"]}'";
                Dictionary<string, object> paramTestDic2 = new Dictionary<string, object>();
                paramTestDic2.Add("task_no", $@"{task_no}");
                DataTable dtHead1 = DB.GetDataTable(sql, paramTestDic2);
                sql = $@"select pl_qty,supplier,approver,tabulator,area_diff_cft,pur_qty_cft,avg_use_rate,assessment from qcm_hp_task_satra where isdelete='0' and task_no='{paramTestDic["task_no"]}'";
                DataTable dtHead2 = DB.GetDataTable(sql, paramTestDic2);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("DataHZ", dt);
                dic.Add("DataCY", dtlist);
                dic.Add("DataHead1", dtHead1);
                dic.Add("DataHead2", dtHead2);

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
        /// 皮料评估报表页面保存PDA
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Painted_Skin_Report_Edit_PDA(object OBJ)
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
                Dictionary<string, string> tabHead = jarr.ContainsKey("tabHead") ? Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jarr["tabHead"].ToString()) : new Dictionary<string, string>();//表头数据
                List<qcm_hp_task_satra_l_dto> qcm_hp_task_satra_l_list = new List<qcm_hp_task_satra_l_dto>();
                if (jarr.ContainsKey("qcm_hp_task_satra_l"))
                {
                    qcm_hp_task_satra_l_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<qcm_hp_task_satra_l_dto>>(jarr["qcm_hp_task_satra_l"].ToString());
                }
                List<qcm_hp_task_satra_r_dto> qcm_hp_task_satra_r_list = new List<qcm_hp_task_satra_r_dto>();
                if (jarr.ContainsKey("qcm_hp_task_satra_r"))
                {
                    qcm_hp_task_satra_r_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<qcm_hp_task_satra_r_dto>>(jarr["qcm_hp_task_satra_r"].ToString());
                }
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                int countHead = DB.GetInt32($@"select count(1) from qcm_hp_task_satra where task_no='{task_no}' and isdelete ='0'");
                if (countHead > 0)
                {
                    sql = $@"update qcm_hp_task_satra set pl_qty='{tabHead["pl_qty"]}',supplier='{tabHead["supplier"]}',approver='{tabHead["approver"]}',
                             tabulator='{tabHead["tabulator"]}',area_diff_cft='{tabHead["area_diff_cft"]}',pur_qty_cft='{tabHead["pur_qty_cft"]}',
                             avg_use_rate='{tabHead["avg_use_rate"]}',assessment='{tabHead["assessment"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                            where task_no='{task_no}' and isdelete='0'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"insert into qcm_hp_task_satra (task_no,pl_qty,supplier,approver,tabulator,area_diff_cft,pur_qty_cft,avg_use_rate,assessment,
                             createby,createdate,createtime)
                             values('{task_no}','{tabHead["pl_qty"]}','{tabHead["supplier"]}','{tabHead["approver"]}','{tabHead["tabulator"]}','{tabHead["area_diff_cft"]}',
                             '{tabHead["pur_qty_cft"]}','{tabHead["avg_use_rate"]}','{tabHead["assessment"]}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }

                int countsatra_l = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_l where task_no='{task_no}' and isdelete ='0'");
                if (countsatra_l > 0)
                {
                    foreach (var item in qcm_hp_task_satra_l_list)
                    {
                        if (item.SORTING != "21")
                        {
                            sql = $@"update qcm_hp_task_satra_l set sorting='{item.SORTING}',supplier_area='{item.SUPPLIER_AREA}',actual_area='{item.ACTUAL_AREA}',
                                    modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no='{task_no}' and sorting='{item.SORTING}' and isdelete='0'";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }
                else
                {
                    foreach (var item in qcm_hp_task_satra_l_list)
                    {
                        if (item.SORTING != "21")
                        {
                            sql = $@"insert into qcm_hp_task_satra_l (task_no,sorting,supplier_area,actual_area,createby,createdate,createtime)
                                     values('{task_no}','{item.SORTING}','{item.SUPPLIER_AREA}','{item.ACTUAL_AREA}',
                                     '{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }

                int countsatra_r = DB.GetInt32($@"select count(1) from qcm_hp_task_satra_r where task_no='{task_no}' and isdelete='0'");
                if (countsatra_r > 0)
                {
                    foreach (var item in qcm_hp_task_satra_r_list)
                    {
                        if (item.ID > 0)
                        {
                            if (string.IsNullOrEmpty(item.MULTIPLE.ToString()) || string.IsNullOrEmpty(item.QTY.ToString()))
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "Multiple or amount cannot be empty!";
                                return ret;
                            }
                            string coefficient = item.COEFFICIENT == "-" ? "0" : item.COEFFICIENT.TrimEnd('%');
                            coefficient = (Convert.ToDecimal(coefficient) / 100).ToString();
                            sql = $@"update qcm_hp_task_satra_r set multiple='{item.MULTIPLE}',coefficient='{coefficient}',
                                     qty='{item.QTY}',pl_level='{item.ENUM_CODE}',modifyby='{user}',modifydate='{date}',modifytime='{time}'
                                     where isdelete='0' and task_no='{task_no}' and id='{item.ID}'";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                }
                else
                {
                    foreach (var item in qcm_hp_task_satra_r_list)
                    {
                        if (item.ENUM_CODE.ToString() != "4.5")
                        {
                            if (string.IsNullOrEmpty(item.MULTIPLE.ToString()) || string.IsNullOrEmpty(item.QTY.ToString()))
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "Multiple or amount cannot be empty!";
                                return ret;
                            }
                            string coefficient = item.COEFFICIENT == "-" ? "0" : item.COEFFICIENT.TrimEnd('%');
                            coefficient = (Convert.ToDecimal(coefficient) / 100).ToString();
                            sql = $@"insert into qcm_hp_task_satra_r (task_no,multiple,coefficient,qty,pl_level,createby,createdate,createtime)
                                     values('{task_no}','{item.MULTIPLE}','{coefficient}','{item.QTY}','{item.ENUM_CODE}',
                                     '{user}','{date}','{time}' )";
                            DB.ExecuteNonQuery(sql);
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
    }

    public class qcm_hp_task_satra_l_dto
    {
        public string SORTING { get; set; }
        public string SUPPLIER_AREA { get; set; }
        public string ACTUAL_AREA { get; set; }
    }

    public class qcm_hp_task_satra_r_dto
    {
        /// <summary>
        /// 
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// I级
        /// </summary>
        public string ENUM_VALUE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MULTIPLE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string COEFFICIENT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QTY { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ENUM_CODE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ISINPUT { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ISTOTAL { get; set; }
    }

}
