using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    class BDM_PARAM_ITEM_M
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
                string param_item_no = jarr.ContainsKey("param_item_no") ? jarr["param_item_no"].ToString() : "";//查询条件 名称
                string param_item_name = jarr.ContainsKey("param_item_name") ? jarr["param_item_name"].ToString() : "";//查询条件 编号
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//查询条件 备注
                string check_standard = jarr.ContainsKey("check_standard") ? jarr["check_standard"].ToString() : "";//查询条件标准
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";//查询条件 编号
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//查询条件 编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(param_item_no))
                {
                    where += $@" and BDM_PARAM_ITEM_M.PARAM_ITEM_NO LIKE '%{param_item_no}%' ";
                }
                if (!string.IsNullOrWhiteSpace(param_item_name))
                {
                    where += $@" and BDM_PARAM_ITEM_M.PARAM_ITEM_NAME  LIKE '%{param_item_name}%' ";
                }
                if (!string.IsNullOrWhiteSpace(check_standard))
                {
                    where += $@" and BDM_PARAM_ITEM_M.CHECK_STANDARD  LIKE '%{check_standard}%' ";
                }
                if (!string.IsNullOrWhiteSpace(workshop_section_name))
                {
                    where += $@" and BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NAME like '%{workshop_section_name}%' ";
                }
                if (!string.IsNullOrWhiteSpace(remarks))
                {
                    where += $@" and BDM_PARAM_ITEM_M.REMARK like '%{remarks}%' ";
                }
                if (!string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    where += $@" and BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NO like '%{workshop_section_no}%' ";
                }
                string sql = string.Empty;
                if (string.IsNullOrEmpty(config_no)|| config_no=="")
                {
                    sql = $@"
                SELECT
                BDM_PARAM_ITEM_M.ID,
                BDM_PARAM_ITEM_M.PARAM_ITEM_NO,
                BDM_PARAM_ITEM_M.PARAM_ITEM_NAME,
                BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NO,
                BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NAME,
                BDM_PARAM_ITEM_M.JUDGMENT_CRITERIA as JUDGMENT_CRITERIA_CODE,
                SYS001M.ENUM_VALUE as JUDGMENT_CRITERIA,
                BDM_PARAM_ITEM_M.CHECK_STANDARD,
                BDM_PARAM_ITEM_M.REMARK
                FROM 
                BDM_PARAM_ITEM_M
				LEFT JOIN SYS001M ON BDM_PARAM_ITEM_M.JUDGMENT_CRITERIA=SYS001M.ENUM_CODE and SYS001M.ENUM_TYPE='enum_PARAM_Eunm'
                where 1=1 {where}";
                }
                else 
                {
                    sql = $@"	
				SELECT
				CASE  NVL(BDM_PARAM_ITEM_UNION.ID,-1)  
				WHEN -1 THEN '0' 
				ELSE '1' 
				END AS Checked,
                BDM_PARAM_ITEM_M.ID,
                BDM_PARAM_ITEM_M.PARAM_ITEM_NO,
                BDM_PARAM_ITEM_M.PARAM_ITEM_NAME,
                BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NO,
                BDM_PARAM_ITEM_M.WORKSHOP_SECTION_NAME,
                BDM_PARAM_ITEM_M.JUDGMENT_CRITERIA as JUDGMENT_CRITERIA_CODE,
                SYS001M.ENUM_VALUE as JUDGMENT_CRITERIA,
                BDM_PARAM_ITEM_M.CHECK_STANDARD,
                BDM_PARAM_ITEM_M.REMARK
                FROM 
                BDM_PARAM_ITEM_M
				LEFT JOIN SYS001M ON BDM_PARAM_ITEM_M.JUDGMENT_CRITERIA=SYS001M.ENUM_CODE and SYS001M.ENUM_TYPE='enum_PARAM_Eunm'
				LEFT JOIN BDM_PARAM_ITEM_UNION ON BDM_PARAM_ITEM_UNION.PARAM_ITEM_NO=BDM_PARAM_ITEM_M.PARAM_ITEM_NO AND BDM_PARAM_ITEM_UNION.CONFIG_NO='{config_no}'
                where 1 = 1 {where}";
                }
               
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
        /// 参数项目删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteParamItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string param_item_no = jarr.ContainsKey("param_item_no") ? jarr["param_item_no"].ToString() : "";//param_item_no
                int unionCount = Convert.ToInt32(DB.GetStringline($@"SELECT count(1) FROM BDM_PARAM_ITEM_UNION WHERE PARAM_ITEM_NO='{param_item_no}'"));
                if (unionCount > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Parameter item is already associated, deletion failed";
                }
                else
                {
                    DB.ExecuteNonQuery($@"delete from BDM_PARAM_ITEM_M where param_item_no='{param_item_no}'");
                    DB.Commit();//提交事务
                    ret.IsSuccess = true;
                }
                #endregion
            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;

        }

        /// <summary>
        /// 工段种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshop(object OBJ)
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
		        SELECT
                WORKMANSHIP_CODE,
                WORKMANSHIP_NAME,
                WORKSHOP_SECTION_NO,
                WORKSHOP_SECTION_NAME
                FROM bdm_workmanship_m";
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
        ///  检查类型Eunm
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEunm(object OBJ)
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
                SELECT
                ENUM_CODE，ENUM_VALUE
                FROM 
                SYS001M
                WHERE ENUM_TYPE='enum_PARAM_Eunm'";
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
        /// 工段种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetParam_AddOrUpdate(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string param_item_no = jarr.ContainsKey("param_item_no") ? jarr["param_item_no"].ToString() : "";
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";
                string param_item_name = jarr.ContainsKey("param_item_name") ? jarr["param_item_name"].ToString() : "";
                string judgment_criteria = jarr.ContainsKey("judgment_criteria") ? jarr["judgment_criteria"].ToString() : "";
                string check_standard = jarr.ContainsKey("check_standard") ? jarr["check_standard"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CreateData = DateTime.Now.ToString("yyyy-MM-dd");
                string CreateTime = DateTime.Now.ToString("HH:mm:ss");
                Dictionary<string, object> p_BDM_PARAM_ITEM_M = new Dictionary<string, object>();

                if (string.IsNullOrEmpty(id))
                {
                    p_BDM_PARAM_ITEM_M.Add("PARAM_ITEM_NO", param_item_no);//编号
                    p_BDM_PARAM_ITEM_M.Add("WORKSHOP_SECTION_NAME", workshop_section_name);//工段种类
                    p_BDM_PARAM_ITEM_M.Add("WORKSHOP_SECTION_NO", workshop_section_no);//工段种类
                    p_BDM_PARAM_ITEM_M.Add("PARAM_ITEM_NAME", param_item_name);//名称
                    p_BDM_PARAM_ITEM_M.Add("JUDGMENT_CRITERIA", judgment_criteria);//判断标准
                    p_BDM_PARAM_ITEM_M.Add("CHECK_STANDARD", check_standard);//检测项目标准
                    p_BDM_PARAM_ITEM_M.Add("REMARK", remarks);//备注
                    p_BDM_PARAM_ITEM_M.Add("CREATEBY", usercode);
                    p_BDM_PARAM_ITEM_M.Add("CREATEDATE", CreateData);
                    p_BDM_PARAM_ITEM_M.Add("CREATETIME", CreateTime);
                    int count = DB.GetInt32("select count(1) from BDM_PARAM_ITEM_M where PARAM_ITEM_NO=@PARAM_ITEM_NO", p_BDM_PARAM_ITEM_M);
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Number cannot be repeated！！";
                        return ret;
                    }
                    DB.ExecuteNonQuery(
                        SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "BDM_PARAM_ITEM_M", p_BDM_PARAM_ITEM_M),
                        p_BDM_PARAM_ITEM_M);
                }
                else
                {
                    string old_WORKSHOP_SECTION_NO = DB.GetStringline($@"SELECT WORKSHOP_SECTION_NO FROM BDM_PARAM_ITEM_M where ID={id}");//原 工段种类
                    //有修改过 工段种类
                    if (old_WORKSHOP_SECTION_NO != workshop_section_no)
                    {
                        p_BDM_PARAM_ITEM_M.Add("PARAM_ITEM_NO", param_item_no);//编号
                        int count = DB.GetInt32("select count(1) from BDM_PARAM_ITEM_UNION where PARAM_ITEM_NO=@PARAM_ITEM_NO", p_BDM_PARAM_ITEM_M);
                        if (count > 0)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "The parameter item has been associated, and the section type cannot be modified";
                            return ret;
                        }
                        p_BDM_PARAM_ITEM_M.Clear();
                    }

                    p_BDM_PARAM_ITEM_M.Add("PARAM_ITEM_NAME", param_item_name);//名称
                    p_BDM_PARAM_ITEM_M.Add("WORKSHOP_SECTION_NO", workshop_section_no);//工段种类
                    p_BDM_PARAM_ITEM_M.Add("WORKSHOP_SECTION_NAME", workshop_section_name);//工段种类
                    p_BDM_PARAM_ITEM_M.Add("JUDGMENT_CRITERIA", judgment_criteria);//判断标准
                    p_BDM_PARAM_ITEM_M.Add("CHECK_STANDARD", check_standard);//检测项目标准
                    p_BDM_PARAM_ITEM_M.Add("REMARK", remarks);//备注
                    p_BDM_PARAM_ITEM_M.Add("MODIFYBY", usercode);
                    p_BDM_PARAM_ITEM_M.Add("MODIFYDATE", CreateData);
                    p_BDM_PARAM_ITEM_M.Add("MODIFYTIME", CreateTime);
                    p_BDM_PARAM_ITEM_M.Add("ID", id);//编号
                    string sql = $@"UPDATE BDM_PARAM_ITEM_M SET PARAM_ITEM_NAME=@PARAM_ITEM_NAME,WORKSHOP_SECTION_NO=@WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME=@WORKSHOP_SECTION_NAME,JUDGMENT_CRITERIA=@JUDGMENT_CRITERIA,CHECK_STANDARD=@CHECK_STANDARD,REMARK=@REMARK,MODIFYBY=@MODIFYBY,MODIFYDATE=@MODIFYDATE,MODIFYTIME=@MODIFYTIME WHERE ID=@ID";
                    DB.ExecuteNonQuery(sql, p_BDM_PARAM_ITEM_M);
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
        /// 配置页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshopConfig_SectIon(object OBJ)
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
                string workmanship_name = jarr.ContainsKey("workmanship_name") ? jarr["workmanship_name"].ToString() : "";//查询条件 编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(workmanship_name))
                {
                    where += $@" and BDM_PARAM_ITEM_D.WORKMANSHIP_NAME LIKE '%{workmanship_name}%'";
                }
                if (!string.IsNullOrWhiteSpace(workshop_section_name))
                {
                    where += $@" and BDM_PARAM_ITEM_D.WORKSHOP_SECTION_NAME LIKE '%{workshop_section_name}%' ";
                }
                string sql = $@"
                select
				BDM_PARAM_ITEM_D.ID,
                BDM_PARAM_ITEM_D.CONFIG_NO,
				BDM_PARAM_ITEM_D.WORKMANSHIP_CODE,
				-- BDM_PARAM_ITEM_D.WORKMANSHIP_NAME,
                bdm_workmanship_m.WORKMANSHIP_NAME,
				BDM_PARAM_ITEM_D.WORKSHOP_SECTION_NO,
                BDM_PARAM_ITEM_D.WORKSHOP_SECTION_NAME,
				BDM_PARAM_ITEM_D.REMARK
				from
				BDM_PARAM_ITEM_D
                LEFT JOIN bdm_workmanship_m on BDM_PARAM_ITEM_D.WORKMANSHIP_CODE = bdm_workmanship_m.workmanship_code
                where 1=1 {where}";
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshopConfig_Add(object OBJ)
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
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";
                string workshop_section_name = jarr.ContainsKey("workshop_section_name") ? jarr["workshop_section_name"].ToString() : "";
                string workmanship_code = jarr.ContainsKey("workmanship_code") ? jarr["workmanship_code"].ToString() : "";
                string workmanship_name = jarr.ContainsKey("workmanship_name") ? jarr["workmanship_name"].ToString() : "";
                string note = jarr.ContainsKey("note") ? jarr["note"].ToString() : "";
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CreateData = DateTime.Now.ToString("yyyy-MM-dd");
                string CreateTime = DateTime.Now.ToString("HH:mm:ss");
                Dictionary<string, object> p_BDM_PARAM_ITEM_D = new Dictionary<string, object>();
                p_BDM_PARAM_ITEM_D.Add("CONFIG_NO", config_no);//编号
                p_BDM_PARAM_ITEM_D.Add("WORKSHOP_SECTION_NAME", workshop_section_name);//工段种类
                p_BDM_PARAM_ITEM_D.Add("WORKSHOP_SECTION_NO", workshop_section_no);//工段种类
                p_BDM_PARAM_ITEM_D.Add("WORKMANSHIP_CODE", workmanship_code);//工艺编号
                p_BDM_PARAM_ITEM_D.Add("WORKMANSHIP_NAME", workmanship_name);//工艺名称
                p_BDM_PARAM_ITEM_D.Add("REMARK", note);//工艺名称
                p_BDM_PARAM_ITEM_D.Add("CREATEBY", usercode);
                p_BDM_PARAM_ITEM_D.Add("CREATEDATE", CreateData);
                p_BDM_PARAM_ITEM_D.Add("CREATETIME", CreateTime);
                int count = DB.GetInt32("select count(1) from BDM_PARAM_ITEM_D where CONFIG_NO=@CONFIG_NO", p_BDM_PARAM_ITEM_D);
                if (count>0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Number cannot be repeated！！";
                    return ret;
                }
                int countww = DB.GetInt32($@"select count(1) from BDM_PARAM_ITEM_D where WORKSHOP_SECTION_NO='{workshop_section_no}' and WORKMANSHIP_CODE='{workmanship_code}'");
                if (countww > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Sections and processes cannot be duplicated with existing data！！";
                    return ret;
                }
                DB.ExecuteNonQuery(
                SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "BDM_PARAM_ITEM_D", p_BDM_PARAM_ITEM_D),
                p_BDM_PARAM_ITEM_D);
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
        /// 删除AQL
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//id
                DataTable dt = DB.GetDataTable($@"select
								BDM_PARAM_ITEM_UNION.ID
								from 
								BDM_PARAM_ITEM_UNION
								LEFT JOIN BDM_PARAM_ITEM_D ON BDM_PARAM_ITEM_UNION.CONFIG_NO =BDM_PARAM_ITEM_D.CONFIG_NO");
                if (dt.Rows.Count>0&&dt!=null)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        DB.ExecuteNonQuery($@"delete from BDM_PARAM_ITEM_UNION where ID='{item["ID"].ToString()}'");
                    }
                }
                DB.ExecuteNonQuery($@"delete from BDM_PARAM_ITEM_D where ID='{ID}'");
                #endregion
                ret.IsSuccess = true;
                DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;

        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshopConfigUnion_Add(object OBJ)
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
                string config_no = jarr.ContainsKey("config_no") ? jarr["config_no"].ToString() : "";
                string no = jarr.ContainsKey("param_item_no") ? jarr["param_item_no"].ToString() : "";
                string dele_no = jarr.ContainsKey("delete_param_item_no") ? jarr["delete_param_item_no"].ToString() : "";
                List<string> param_item_no = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(no);
                List<string> delete_param_item_no = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(dele_no);
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CreateData = DateTime.Now.ToString("yyyy-MM-dd");
                string CreateTime = DateTime.Now.ToString("HH:mm:ss");
                //勾选绑定
                foreach (var item in param_item_no)
                {
                    Dictionary<string, object> p_BDM_PARAM_ITEM_UNION = new Dictionary<string, object>();
                    p_BDM_PARAM_ITEM_UNION.Add("CONFIG_NO", config_no);//编号
                    p_BDM_PARAM_ITEM_UNION.Add("PARAM_ITEM_NO", item);//工段种类
                    p_BDM_PARAM_ITEM_UNION.Add("CREATEBY", usercode);
                    p_BDM_PARAM_ITEM_UNION.Add("CREATEDATE", CreateData);
                    p_BDM_PARAM_ITEM_UNION.Add("CREATETIME", CreateTime);
                    DB.ExecuteNonQuery(
                    SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "BDM_PARAM_ITEM_UNION", p_BDM_PARAM_ITEM_UNION),
                    p_BDM_PARAM_ITEM_UNION);
                }
                //取消绑定
                foreach (var item in delete_param_item_no)
                {
                    DB.ExecuteNonQuery($@"DELETE FROM BDM_PARAM_ITEM_UNION WHERE CONFIG_NO='{config_no}'AND PARAM_ITEM_NO='{item}'");
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
    }
}
