using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_BA_Entry
    {
        /// <summary>
        /// 编辑-BA录入-精美/不精美确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditFineItem(object OBJ)
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
                string star_level = jarr.ContainsKey("star_level") ? jarr["star_level"].ToString() : "";//条件 星级
                string exquisite_qty = jarr.ContainsKey("exquisite_qty") ? jarr["exquisite_qty"].ToString() : "";//条件 精美数
                string not_exquisite_qty = jarr.ContainsKey("not_exquisite_qty") ? jarr["not_exquisite_qty"].ToString() : "";//条件 不精美数
                string c1_qty = jarr.ContainsKey("c1_qty") ? jarr["c1_qty"].ToString() : "";//条件 c1
                string c2_qty = jarr.ContainsKey("c2_qty") ? jarr["c2_qty"].ToString() : "";//条件 c2
                string c3_qty = jarr.ContainsKey("c3_qty") ? jarr["c3_qty"].ToString() : "";//条件 c3
                string c4_qty = jarr.ContainsKey("c4_qty") ? jarr["c4_qty"].ToString() : "";//条件 c4
                string c5_qty = jarr.ContainsKey("c5_qty") ? jarr["c5_qty"].ToString() : "";//条件 c5
                string c6_qty = jarr.ContainsKey("c6_qty") ? jarr["c6_qty"].ToString() : "";//条件 c6
                string c7_qty = jarr.ContainsKey("c7_qty") ? jarr["c7_qty"].ToString() : "";//条件 c7
                string c8_qty = jarr.ContainsKey("c8_qty") ? jarr["c8_qty"].ToString() : "";//条件 c8
                string c9_qty = jarr.ContainsKey("c9_qty") ? jarr["c9_qty"].ToString() : "";//条件 c9
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                //判断是否有这条数据
                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_ba where task_no='{task_no}'");
                if (count <= 0)
                    sql = $@"insert into aql_cma_task_list_m_ba (task_no,star_level,exquisite_qty,not_exquisite_qty,c1_qty,c2_qty,c3_qty,c4_qty,
                            c5_qty,c6_qty,c7_qty,c8_qty,c9_qty,createby,createdate,createtime) 
                            values('{task_no}','{star_level}','{exquisite_qty}','{not_exquisite_qty}','{c1_qty}','{c2_qty}','{c3_qty}','{c4_qty}',
                            '{c5_qty}','{c6_qty}','{c7_qty}','{c8_qty}','{c9_qty}','{user}','{date}','{time}')";
                else
                    sql = $@"update aql_cma_task_list_m_ba set star_level='{star_level}',exquisite_qty='{exquisite_qty}',not_exquisite_qty='{not_exquisite_qty}',
                            c1_qty='{c1_qty}',c2_qty='{c2_qty}',c3_qty='{c3_qty}',c4_qty='{c4_qty}',c5_qty='{c5_qty}',c6_qty='{c6_qty}',c7_qty='{c7_qty}',
                            c8_qty='{c8_qty}',c9_qty='{c9_qty}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where task_no = '{task_no}'";  


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
        /// 查询-BA录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBA_Entry(object OBJ)
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

                string sql = $@"select 
                                task_no,
                                star_level,
                                exquisite_qty,
                                not_exquisite_qty,
                                c1_qty,
                                c2_qty,
                                c3_qty,
                                c4_qty,
                                c5_qty,
                                c6_qty,
                                c7_qty,
                                c8_qty,
                                c9_qty
                                from aql_cma_task_list_m_ba where task_no='{task_no}'
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_BA_Reports(object OBJ)
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
                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//查询条件 任务编号
                string s_date = jarr.ContainsKey("s_date") ? jarr["s_date"].ToString() : "";
                string e_date = jarr.ContainsKey("e_date") ? jarr["e_date"].ToString() : "";
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string wheresql= string.Empty;
                if(!String.IsNullOrEmpty(s_date) && !String.IsNullOrEmpty(e_date))
                {
                  wheresql += $@" and a.createdate between to_char(to_date('{s_date}','yyyy/MM/dd'),'yyyy-MM-dd') and to_char(to_date('{e_date}','yyyy/MM/dd'),'yyyy-MM-dd')";
                }

                string sql = $@"select 
a.createdate as create_date,
c.name_s as model_name,
b.art_no as article,
substr(a.task_no,0,instr(a.task_no,'-')-1) as po,
cx.production_line as production_line,
b.po_num as Po_num,
(a.exquisite_qty + a.not_exquisite_qty) as pairs_inspected,
a.exquisite_qty as pairs_beautiful,
round(case
  when a.exquisite_qty = 0 then 0 else
    (a.exquisite_qty/(a.exquisite_qty + a.not_exquisite_qty)*100) end,2) as beautiful_rate,
a.star_level as star_rating,
a.c1_qty as c1,
a.c2_qty as c2,
a.c3_qty as c3,
a.c4_qty as c4,
a.c5_qty as c5,
a.c6_qty as c6,
a.c7_qty as c7,
a.c8_qty as c8,
a.c9_qty as c9,
tmp.bad_item_names
from aql_cma_task_list_m_ba a
left join aql_cma_task_list_m b on a.task_no = b.task_no 
left join bdm_rd_prod c on c.prod_no = b.art_no
left join (
select a.mer_po,
listagg(DISTINCT a.production_line_code, ',') WITHIN GROUP (ORDER BY a.production_line_code) as production_line
 from tqc_task_m a where a.workshop_section_no='L' group by a.mer_po 
) cx on cx.mer_po = substr(a.task_no,0,instr(a.task_no,'-')-1) 
left join (select substr(a.task_no,0,instr(a.task_no,'-')-1) as task_no,
listagg(a.bad_item_name, ',') WITHIN GROUP (ORDER BY a.bad_item_name) as bad_item_names
 from aql_cma_task_list_m_aql_e_br a  group by substr(a.task_no,0,instr(a.task_no,'-')-1) 
) tmp on tmp.task_no = substr(a.task_no,0,instr(a.task_no,'-')-1)
where 1=1 {wheresql} order by a.createdate" ;
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
        /// 编辑-BA录入-BA星级/不精美数/精美数
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Edit_BABjmJm(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string state = jarr.ContainsKey("state") ? jarr["state"].ToString() : "";//条件 判断 BA星级/不精美数/精美数
                string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";//条件 数据
                string star_level = jarr.ContainsKey("star_level") ? jarr["star_level"].ToString() : "";//条件 BA星级
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//time
                string sql = string.Empty;

                //Determine whether this data exists
                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_ba where task_no='{task_no}'");
                if (count <= 0)
                {
                    switch (state)
                    {
                        case "BA":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,star_level,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "bjm":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,star_level,not_exquisite_qty,createby,createdate,createtime) 
                                    values('{task_no}','{star_level}','{value}','{user}','{date}','{time}')";
                            break;
                        case "jm":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,star_level,exquisite_qty,createby,createdate,createtime) 
                                    values('{task_no}','{star_level}','{value}','{user}','{date}','{time}')";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch(state)
                    {
                        case "BA":
                            sql = $@"update aql_cma_task_list_m_ba set star_level='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                        break;
                        case "bjm":
                            sql = $@"update aql_cma_task_list_m_ba set star_level='{star_level}',not_exquisite_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                        break;
                        case "jm":
                            sql = $@"update aql_cma_task_list_m_ba set star_level='{star_level}',exquisite_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                        break;
                        default:
                            break;
                    }
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
        /// 编辑-BA录入-c数
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Edit_qty(object OBJ)
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
                string state = jarr.ContainsKey("state") ? jarr["state"].ToString() : "";//条件 判断 c几
                string value = jarr.ContainsKey("value") ? jarr["value"].ToString() : "";//条件 数据
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                //判断是否有这条数据
                int count = DB.GetInt32($@"select count(1) from aql_cma_task_list_m_ba where task_no='{task_no}'");
                if (count <= 0)
                {
                    switch (state)
                    {
                        case "C1":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c1_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C2":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c2_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C3":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c3_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C4":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c4_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C5":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c5_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C6":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c6_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C7":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c7_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C8":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c8_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        case "C9":
                            sql = $@"insert into aql_cma_task_list_m_ba (task_no,c9_qty,createby,createdate,createtime) 
                                    values('{task_no}','{value}','{user}','{date}','{time}')";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case "C1":
                            sql = $@"update aql_cma_task_list_m_ba set c1_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C2":
                            sql = $@"update aql_cma_task_list_m_ba set c2_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C3":
                            sql = $@"update aql_cma_task_list_m_ba set c3_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C4":
                            sql = $@"update aql_cma_task_list_m_ba set c4_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C5":
                            sql = $@"update aql_cma_task_list_m_ba set c5_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C6":
                            sql = $@"update aql_cma_task_list_m_ba set c6_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C7":
                            sql = $@"update aql_cma_task_list_m_ba set c7_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C8":
                            sql = $@"update aql_cma_task_list_m_ba set c8_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        case "C9":
                            sql = $@"update aql_cma_task_list_m_ba set c9_qty='{value}',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                            break;
                        default:
                            break;
                    }
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
        /// 更新ba录入编辑状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditBaState(object OBJ)
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

                sql = $@"update aql_cma_task_list_m set BA_EDIT_STATE='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                #region Added by Ashok to Update BA Star Rating status for Pivot sync
                string BAdatasql = $@"UPDATE AQL_PIVOT_DATA_STATUS
SET ba_entry_status  = 1,
    modified_by = {user},
    modified_at = SYSDATE
WHERE task_no = '{task_no}'";
                DB.ExecuteNonQuery(BAdatasql);
                #endregion

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
}
