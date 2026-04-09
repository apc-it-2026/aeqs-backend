using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class SendTestFrequency
    {
        /// <summary>
        /// 送检频率主页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSendTestFrequency(object OBJ)
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
                string data = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";//查询条件
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(data))
                {
                    where += $@" and (INSPECTION_FREQUENCY_VALUE like '%{data}%' or INSPECTION_FREQUENCY_TIME_UNIT like '%{data}%' or REMARKS like '%{data}%')";
                }

                string sql = $@"select b.ID,b.INSPECTION_FREQUENCY_VALUE,b.INSPECTION_FREQUENCY_TIME_UNIT,s.ENUM_VALUE,b.REMARKS from BDM_INSPECTION_FREQUENCY_M b inner join sys001m s 
                                on b.INSPECTION_FREQUENCY_TIME_UNIT=s.ENUM_CODE where s.ENUM_TYPE='enum_SendTestFrequency' {where}";
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
        /// 送检频率修改赋值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUpdataValue(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件id

                string sql = $@"select ID,INSPECTION_FREQUENCY_VALUE,INSPECTION_FREQUENCY_TIME_UNIT,REMARKS from BDM_INSPECTION_FREQUENCY_M
                                where 1=1 and ID='{id}'";

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
        /// 送检频率获取下拉框数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetComValue(object OBJ)
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
                //string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件id

                string sql = $@"select enum_code,enum_value from sys001m where enum_type='enum_SendTestFrequency'";

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
        /// 送检频率编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditSendTestFrequency(object OBJ)
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
                string id = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//查询条件 id
                string INSPECTION_FREQUENCY_VALUE = jarr.ContainsKey("INSPECTION_FREQUENCY_VALUE") ? jarr["INSPECTION_FREQUENCY_VALUE"].ToString() : "";//查询条件 值
                string INSPECTION_FREQUENCY_TIME_UNIT = jarr.ContainsKey("INSPECTION_FREQUENCY_TIME_UNIT") ? jarr["INSPECTION_FREQUENCY_TIME_UNIT"].ToString() : "";//查询条件 单位
                string REMARKS = jarr.ContainsKey("REMARKS") ? jarr["REMARKS"].ToString() : "";//查询条件 备注
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    sql = $@"update BDM_INSPECTION_FREQUENCY_M set INSPECTION_FREQUENCY_VALUE='{INSPECTION_FREQUENCY_VALUE}',INSPECTION_FREQUENCY_TIME_UNIT='{INSPECTION_FREQUENCY_TIME_UNIT}',
                            REMARKS='{REMARKS}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where ID='{id}'";
                }
                else
                {
                    sql = $@"insert into BDM_INSPECTION_FREQUENCY_M (INSPECTION_FREQUENCY_VALUE,INSPECTION_FREQUENCY_TIME_UNIT,REMARKS,CREATEBY,CREATEDATE,CREATETIME) 
                            values('{INSPECTION_FREQUENCY_VALUE}','{INSPECTION_FREQUENCY_TIME_UNIT}','{REMARKS}','{user}','{date}','{time}')";
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
        /// 送检频率删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteSendTestFrequency(object OBJ)
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
                string id = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//查询条件 id

                DB.ExecuteNonQuery($@"delete from BDM_INSPECTION_FREQUENCY_M where ID='{id}'");
                DB.ExecuteNonQuery($@"delete from BDM_INSPECTION_FREQUENCY_D where M_ID='{id}'");
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
        /// 查询未绑定材料种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject WBindingType(object OBJ)
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
                string item_type_name = jarr.ContainsKey("item_type_name") ? jarr["item_type_name"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(item_type_name))
                {
                    where = $@" and item_type_name like '%{item_type_name}%'";
                }

                string sql = $@"select item_type_no,item_type_name from BDM_RD_ITEMTYPE where ITEM_TYPE_NO not in (select ITEM_TYPE_NO from BDM_INSPECTION_FREQUENCY_D) {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                DataTable dt2 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);
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
        /// 查询已绑定材料种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject YBindingType(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string item_type_name = jarr.ContainsKey("item_type_name") ? jarr["item_type_name"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(item_type_name))
                {
                    where = $@" and item_type_name like '%{item_type_name}%'";
                }

                string sql = $@"select d.id,d.item_type_no,r.item_type_name from BDM_INSPECTION_FREQUENCY_D d inner join BDM_RD_ITEMTYPE r on d.ITEM_TYPE_NO=r.ITEM_TYPE_NO where d.m_id='{id}' {where}";
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
        /// 绑定材料种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BindingType(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 mid
                string item_type_no_list = jarr.ContainsKey("item_type_no_list") ? jarr["item_type_no_list"].ToString() : "";//表身条件
                string delete = jarr.ContainsKey("delete") ? jarr["delete"].ToString() :"";//表身条件
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                List<string> type_no_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(item_type_no_list);
                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(delete) && !string.IsNullOrWhiteSpace(mid))
                {
                    if (type_no_list.Count>0)
                    {
                      

                        for (int i = 0; i < type_no_list.Count; i++)
                        {
                            DB.ExecuteNonQuery($@"delete from BDM_INSPECTION_FREQUENCY_D where M_ID='{mid}' and ITEM_TYPE_NO='{type_no_list[i]}'");
                        }
                    }
                   
                }
                else
                {
                    if(type_no_list.Count>0)
                    for (int i = 0; i < type_no_list.Count; i++)
                    {
                        int count = DB.GetInt32($@"select count(1) from BDM_INSPECTION_FREQUENCY_D where M_ID='{mid}' and ITEM_TYPE_NO='{type_no_list[i]}'");
                        if (count == 0)
                        {
                            sql = $@"insert into BDM_INSPECTION_FREQUENCY_D(M_ID,ITEM_TYPE_NO,CREATEBY,CREATEDATE,CREATETIME) 
                                    values('{mid}','{type_no_list[i]}','{user}','{date}','{time}')";
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
}
