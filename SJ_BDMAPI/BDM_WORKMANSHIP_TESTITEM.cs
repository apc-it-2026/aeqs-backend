using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_WORKMANSHIP_TESTITEM
    {
        /// <summary>
        /// 查询-检测项目-工艺-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWORKMANSHIP_TESTITEM(object OBJ)
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
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString() : "";
                string name = jarr.ContainsKey("name") ? jarr["name"].ToString() : "";
                string judge = jarr.ContainsKey("judge") ? jarr["judge"].ToString() : "";
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = "";
                if (!string.IsNullOrEmpty(code))
                {
                    where += $" and INSPECTION_CODE like '%{code}%'";
                }
                if (!string.IsNullOrEmpty(name))
                {
                    where += $" and INSPECTION_NAME like '%{name}%'";
                }
                if (!string.IsNullOrEmpty(remark))
                {
                    where += $" and REMARKS like '%{remark}%'";
                }
                if (!string.IsNullOrEmpty(judge))
                {
                    where += $" and judgment_criteria='{judge}'";
                }
                string sql = $@"select ID,INSPECTION_CODE,INSPECTION_NAME,judgment_criteria,STANDARD_VALUE,REMARKS,JUDGE_TYPE from bdm_workmanship_testitem_m where 1=1 {where}";
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
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 保存-检测项目-工艺-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveWORKMANSHIP_TESTITEM(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 名称
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString() : "";//查询条件 名称
                string name = jarr.ContainsKey("name") ? jarr["name"].ToString() : "";//查询条件 名称
                string judge = jarr.ContainsKey("judge") ? jarr["judge"].ToString() : "";//查询条件 名称
                string judge_type = jarr.ContainsKey("judge_type") ? jarr["judge_type"].ToString() : "";//查询条件 名称
                string judge_value = jarr.ContainsKey("judge_value") ? jarr["judge_value"].ToString() : "";//查询条件 名称
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//查询条件 名称
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                if (string.IsNullOrEmpty(id))
                {
                    int count = DB.GetInt32($@"select count(1) from bdm_workmanship_testitem_m where INSPECTION_CODE='{code}'");
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Number cannot be repeated！！";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into bdm_workmanship_testitem_m(INSPECTION_CODE,INSPECTION_NAME,judgment_criteria,JUDGE_TYPE,STANDARD_VALUE,REMARKS,CREATEBY,CREATEDATE,CREATETIME)
                                           values('{code}','{name}','{judge}','{judge_type}','{judge_value}','{remark}','{CreactUserId}','{date}','{time}')");
                }
                else
                {
                    DB.ExecuteNonQuery($@"update bdm_workmanship_testitem_m set INSPECTION_NAME='{name}',
                                                    judgment_criteria='{judge}',
                                                    JUDGE_TYPE='{judge_type}',
                                                    STANDARD_VALUE='{judge_value}',
                                                    REMARKS='{remark}',
                                                    MODIFYBY='{CreactUserId}',
                                                    MODIFYDATE='{date}',
                                                    MODIFYTIME='{time}'
                                            where id={id}");
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
        /// 删除-检测项目-工艺-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteWORKMANSHIP_TESTITEM(object OBJ)
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
                string idlist = jarr.ContainsKey("idlist") ? jarr["idlist"].ToString() : "";//查询条件 名称

                if (!string.IsNullOrEmpty(idlist))
                {
                    DB.ExecuteNonQuery($@"delete bdm_workmanship_testitem_m where id in({idlist})");
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
        /// 查询-检测项目-工艺-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWORKMANSHIP_TESTITEM_Custom(object OBJ)
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
                //string position_name = jarr.ContainsKey("position_name") ? jarr["position_name"].ToString() : "";//部位
                //string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "";//开发系列
                string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "";//FGT测试类型
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = String.Empty;
                //if (!string.IsNullOrWhiteSpace(position_name))
                //{
                //    where += $@" and position_name like '%{position_name}%'";
                //}
                //if (!string.IsNullOrWhiteSpace(category_name))
                //{
                //    where += $@" and category_name like '%{category_name}%'";
                //}

                if (!string.IsNullOrWhiteSpace(fgt_name))
                {
                    where += $@" and fgt_name like '%{fgt_name}%'";
                }
                string sql = $@"SELECT 
                                id,
                                c_no,
                                workmanship_name,
                                position_name,
                                category_name,
                                fgt_name
                                FROM
                                bdm_workmanship_testitem_c
                                WHERE 1=1 {where}";
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
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询-检测项目-工艺-测试-定制类型-新增查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWORKMANSHIP_TESTITEM_Custom_Insert(object OBJ)
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
                //string pb_type_level = jarr.ContainsKey("pb_type_level") ? jarr["pb_type_level"].ToString() : "";//新旧级别
                //string product_level_value = jarr.ContainsKey("product_level_value") ? jarr["product_level_value"].ToString() : "";//产品级别
                //string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "";//用途类别
                //string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "";//FGT测试类型
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;

                sql = $@"SELECT WORKMANSHIP_CODE as code,WORKMANSHIP_NAME as value FROM bdm_workmanship_m";
                DataTable dt1 = DB.GetDataTable(sql);

                sql = $@"SELECT POSITION_CODE as code,POSITION_NAME as value FROM bdm_position_m";
                DataTable dt2 = DB.GetDataTable(sql);

                sql = $@"SELECT CATEGORY_CODE as code,CATEGORY_NAME as value from bdm_category_m";
                DataTable dt3 = DB.GetDataTable(sql);

                sql = $@"select fgt_code as  code ,fgt_name as value from bdm_fgt_m";
                DataTable dt4 = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);//bdm_workmanship_m 工艺
                dic.Add("Data2", dt2);//bdm_position_m 部位
                dic.Add("Data3", dt3);//bdm_category_m 开发系列
                dic.Add("Data4", dt4);//bdm_fgt_m fgt

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
        /// 保存-检测项目-工艺-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveWORKMANSHIP_TESTITEM_Custom_Insert(object OBJ)
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
                string c_no = jarr.ContainsKey("c_no") ? jarr["c_no"].ToString() : "";//查询条件 编号
                //string workmanship_code = jarr.ContainsKey("workmanship_code") ? jarr["workmanship_code"].ToString() : "";//查询条件 工艺编号
                //string workmanship_name = jarr.ContainsKey("workmanship_name") ? jarr["workmanship_name"].ToString() : "";//查询条件 工艺名称
                //string position_code = jarr.ContainsKey("position_code") ? jarr["position_code"].ToString() : "";//查询条件 部位编号
                //string position_name = jarr.ContainsKey("position_name") ? jarr["position_name"].ToString() : "";//查询条件 部位名称
                //string category_code = jarr.ContainsKey("category_code") ? jarr["category_code"].ToString() : "";//查询条件 Category类别编号（开发系列）
                //string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "";//查询条件 Category

                string fgt_code = jarr.ContainsKey("fgt_code") ? jarr["fgt_code"].ToString() : "";//查询条件 fgt类型
                string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "";//查询条件 fgt名称
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int countNO = DB.GetInt32($@"select count(1) from bdm_workmanship_testitem_c where c_no='{c_no}'");
                if (countNO > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Number cannot be repeated!";
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from bdm_workmanship_testitem_c where fgt_code='{fgt_code}'");
                if (count > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Data cannot be consistent!";
                    return ret;
                }

                string sql = $@"insert into bdm_workmanship_testitem_c 
                                (c_no,fgt_code,fgt_name,createby,createdate,createtime)
                                values('{c_no}','{fgt_code}','{fgt_name}',
                                '{user}','{date}','{time}')";
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
        /// 删除-检测项目-工艺-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteWORKMANSHIP_TESTITEM_Custom(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 id
                string sql = string.Empty;
                sql = $@"delete from bdm_workmanship_testitem_c where id='{id}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"delete from bdm_workmanship_testitem_u where c_id='{id}'";
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
        /// 查询-检测项目-工艺-测试-定制类型-勾选
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWORKMANSHIP_TESTITEM_Custom_Check(object OBJ)
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
                string cid = jarr.ContainsKey("cid") ? jarr["cid"].ToString() : "";//cid
                //string name = jarr.ContainsKey("name") ? jarr["name"].ToString() : "";
                //string judge = jarr.ContainsKey("judge") ? jarr["judge"].ToString() : "";
                //string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string sql = $@"SELECT
	                            m.id,
	                            m.inspection_code,
	                            m.inspection_name,
	                            m.judgment_criteria,
                                s.enum_value as judgment_criteria_name,
	                            m.STANDARD_VALUE,
	                            m.remarks,
	                            m.judge_type,
                            CASE
		                            WHEN u.id IS NOT NULL THEN
		                            'true' 
	                            ELSE 'false' 
	                            END AS ischeck 
                            FROM
	                            bdm_workmanship_testitem_m m
	                            LEFT JOIN bdm_workmanship_testitem_u u ON m.id = u.m_id and u.c_id='{cid}'
                                LEFT JOIN SYS001M s on m.judgment_criteria=s.ENUM_CODE and s.enum_type='enum_judgment_criteria'";
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
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 保存-检测项目-工艺-测试-定制类型-勾选
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveWORKMANSHIP_TESTITEM_Custom_Check(object OBJ)
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
                string cid = jarr.ContainsKey("cid") ? jarr["cid"].ToString() : "";//查询条件 cid
                DataTable mtable = jarr.ContainsKey("mtable") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["mtable"].ToString()) : null;//查询条件 m表数据

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = string.Empty;

                foreach (DataRow item in mtable.Rows)
                {
                    if (item["xz"].ToString() == "True")
                    {
                        int count = DB.GetInt32($@"select count(1) from bdm_workmanship_testitem_u where m_id='{item["mid"]}' and c_id='{cid}'");
                        if (count <= 0)
                        {
                            sql = $@"insert into bdm_workmanship_testitem_u (m_id,c_id,createby,createdate,createtime) 
                                    values('{item["mid"]}','{cid}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                    if (item["xz"].ToString() == "False" || string.IsNullOrWhiteSpace(item["xz"].ToString()))
                    {
                        sql = $@"delete from bdm_workmanship_testitem_u where m_id='{item["mid"]}' and c_id='{cid}'";
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
