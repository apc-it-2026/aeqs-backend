using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Workshop_SectIon
    {
        /// <summary>
        /// 工段创建页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshop_SectIon(object OBJ)
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
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";//查询条件 名称
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 编号
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//查询条件 备注
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    where += $@" and workshop_section_no ='{workshop_section_no}' ";
                }
                if (!string.IsNullOrWhiteSpace(workshop_section_name))
                {
                    where += $@" and workshop_section_name like '%{workshop_section_name}%' ";
                }
                if (!string.IsNullOrWhiteSpace(remarks))
                {
                    where += $@" and remarks like '%{remarks}%' ";
                }

                string sql = $@"	select
		                            m.id,
		                            MAX(m.sorting) as sorting,
		                            MAX(m.data_source) as data_source,
		                            MAX(ms.ENUM_VALUE) as enum_value_data_source,
                                    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT to_char(d.inspection_type)", "d.inspection_type",";")} as inspection_type,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT to_char(ds.enum_value)", "ds.enum_value",";")} as enum_value_inspection_type,
		                            MAX(m.workshop_section_no) as workshop_section_no,
		                            MAX(m.workshop_section_name) as workshop_section_name,
		                            MAX(m.product_category) as product_category,
		                            MAX(m.remarks) as remarks
	                            from bdm_workshop_section_m m
	                            left join bdm_workshop_section_d  d on m.id=d.m_id
	                            left join SYS001M ms  ON m.data_source = ms.enum_code  AND ms.enum_type = 'enum_data_source' 
	                            left join SYS001M ds  ON d.inspection_type = ds.enum_code   AND ds.enum_type = 'enum_inspection_type' 
                                where 1=1 {where}
	                            group by
		                            m.id,
		                            d.m_id
		                            ORDER BY sorting";
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
        /// 查询检测项目类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getenum_inspection_type(object OBJ)
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
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"select enum_code,enum_value from sys001m where enum_type='enum_inspection_type'";
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
        /// 查询材料/工序数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getenum_data_source(object OBJ)
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
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"select enum_code,enum_value from sys001m where enum_type='enum_data_source'";
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
        /// 工段创建编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditWorkshop_SectIon(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 id
                string data_source = jarr.ContainsKey("data_source") ? jarr["data_source"].ToString() : "";//查询条件 材料/工序数据源
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 编号
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";//查询条件 工段名称
                string product_category = jarr.ContainsKey("product_category") ? jarr["product_category"].ToString() : "";//查询条件 产品种类
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//查询条件 备注
                List<int> inspection_type = jarr.ContainsKey("inspection_type") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(jarr["inspection_type"].ToString()) : null;//查询条件 检测项目类型
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(mid))
                {
                    sql = $@"update bdm_workshop_section_m set data_source='{data_source}',workshop_section_no='{workshop_section_no}',workshop_section_name='{workshop_section_name}',
                            product_category='{product_category}',remarks='{remarks}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where id='{mid}'";
                    DB.ExecuteNonQuery(sql);

                    sql = $@"delete from bdm_workshop_section_d where m_id='{mid}'";
                    DB.ExecuteNonQuery(sql);

                    for (int i = 0; i < inspection_type.Count; i++)
                    {
                        sql = $@"insert into bdm_workshop_section_d (m_id,inspection_type,MODIFYBY,MODIFYDATE,MODIFYTIME) values('{mid}','{inspection_type[i]}','{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(workshop_section_no))
                    {
                        int count = DB.GetInt32($@"select count(1) from bdm_workshop_section_m where workshop_section_no='{workshop_section_no}'");
                        if (count > 0)
                        {
                            ret.ErrMsg = "Number cannot be repeated!";
                            ret.IsSuccess = false;
                            return ret;
                        }
                    }
                    int sorting = DB.GetInt32($@"select NVL(MAX(sorting), 0) from bdm_workshop_section_m");
                    sql = $@"insert into bdm_workshop_section_m (sorting,data_source,workshop_section_no,workshop_section_name,product_category,remarks,CREATEBY,CREATEDATE,CREATETIME)
                            values('{sorting+1}','{data_source}','{workshop_section_no}','{workshop_section_name}','{product_category}','{remarks}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    string mmid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "bdm_workshop_section_m");
                    for (int i = 0; i < inspection_type.Count; i++)
                    {
                        sql = $@"insert into bdm_workshop_section_d (m_id,inspection_type,CREATEBY,CREATEDATE,CREATETIME) values('{mmid}','{inspection_type[i]}','{user}','{date}','{time}')";
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
        /// 修改绑值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateWorkshop_SectIon(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 id
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"	select
		                            m.id,
		                            MAX(m.sorting) as sorting,
		                            MAX(m.data_source) as data_source,
		                            MAX(ms.ENUM_VALUE) as enum_value_data_source,
                                    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT to_char(d.inspection_type)", "d.inspection_type",";")} as inspection_type,
		                            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT to_char(ds.enum_value)", "ds.enum_value", ";")} as enum_value_inspection_type,
		                            MAX(m.workshop_section_no) as workshop_section_no,
		                            MAX(m.workshop_section_name) as workshop_section_name,
		                            MAX(m.product_category) as product_category,
		                            MAX(m.remarks) as remarks
	                            from bdm_workshop_section_m m
	                            join bdm_workshop_section_d  d on m.id=d.m_id
	                            join SYS001M ms  ON m.data_source = ms.enum_code  AND ms.enum_type = 'enum_data_source' 
	                            join SYS001M ds  ON d.inspection_type = ds.enum_code   AND ds.enum_type = 'enum_inspection_type' 
                                where m.id='{mid}'
	                            group by
		                            m.id,
		                            d.m_id";
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
        /// 工段创建删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteWorkshop_SectIon(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 id

                DB.ExecuteNonQuery($@"delete from bdm_workshop_section_m where ID='{mid}'");
                DB.ExecuteNonQuery($@"delete from bdm_workshop_section_d where M_ID='{mid}'");

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
        /// 工段创建排序修改
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateGD(object OBJ)
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
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString() : "";//查询条件 id
                string sorting = jarr.ContainsKey("sorting") ? jarr["sorting"].ToString() : "";//查询条件 id

                if (!string.IsNullOrEmpty(sorting))
                {
                    int count = DB.GetInt32($@"select count(1) from bdm_workshop_section_m where sorting='{sorting}'");
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Sort cannot be repeated!";
                        return ret;
                    }
                    else
                        DB.ExecuteNonQuery($@"update bdm_workshop_section_m set sorting='{sorting}' where id='{mid}'");
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Sort cannot be empty!";
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
    }
}
