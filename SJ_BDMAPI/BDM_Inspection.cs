using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Inspection
    {
        /// <summary>
        /// 查询-检测项目-成品鞋-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoseTestInspection(object OBJ)
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
                string sql = $@"select ID,INSPECTION_CODE,INSPECTION_NAME,judgment_criteria,STANDARD_VALUE,REMARKS,JUDGE_TYPE,ISLCLL_ITEM from bdm_shoes_check_test_m where 1=1 {where}";
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
        /// 获取判断标准
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJudge(object OBJ)
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
                string IfSelectNull = jarr.ContainsKey("IfSelectNull") ? jarr["IfSelectNull"].ToString() : "0";

                string sql = $@"select enum_code as CODE,enum_value as VALUE from sys001m where enum_type='enum_judgment_criteria'";
                var list = DB.GetDataTable(sql).ToDataList<code_value_OBJ>();
                if (IfSelectNull == "1")
                {
                    list.Add(new code_value_OBJ { VALUE = "--please choose--", CODE = "" });
                }
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(list.OrderBy(x=>x.CODE).ToList());
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
        /// 保存-检测项目-成品鞋-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveShoseInspection(object OBJ)
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
                string is_lcll = jarr.ContainsKey("is_lcll") ? jarr["is_lcll"].ToString() : "0";//查询条件 名称
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                if (string.IsNullOrEmpty(id))
                {
                    int count = DB.GetInt32($@"select count(1) from bdm_shoes_check_test_m where INSPECTION_CODE='{code}'");
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "The number cannot be repeated ！！";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into bdm_shoes_check_test_m(INSPECTION_CODE,INSPECTION_NAME,judgment_criteria,JUDGE_TYPE,STANDARD_VALUE,REMARKS,CREATEBY,CREATEDATE,CREATETIME,ISLCLL_ITEM)
                                           values('{code}','{name}','{judge}','{judge_type}','{judge_value}','{remark}','{CreactUserId}','{date}','{time}',{is_lcll})");
                }
                else
                {
                    DB.ExecuteNonQuery($@"update bdm_shoes_check_test_m set INSPECTION_NAME='{name}',
                                                    judgment_criteria='{judge}',
                                                    JUDGE_TYPE='{judge_type}',
                                                    STANDARD_VALUE='{judge_value}',
                                                    REMARKS='{remark}',
                                                    MODIFYBY='{CreactUserId}',
                                                    MODIFYDATE='{date}',
                                                    MODIFYTIME='{time}',
                                                    ISLCLL_ITEM='{is_lcll}'
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
        /// 保存-检测项目-成品鞋-测试
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteShoseInspection(object OBJ)
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
                    DB.ExecuteNonQuery($@"delete bdm_shoes_check_test_m where id in({idlist})");
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
        /// 查询-检测项目-成品鞋-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoseTestInspection_Custom(object OBJ)
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
                string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "";//FGT测试类型
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = String.Empty;
                //if (!string.IsNullOrWhiteSpace(pb_type_level))
                //{
                //    where += $@" and pb_type_level like '%{pb_type_level}%'";
                //}
                //if (!string.IsNullOrWhiteSpace(product_level_value))
                //{
                //    where += $@" and product_level_value like '%{product_level_value}%'";
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
                                finished_product_name,
                                pb_type_level,
                                product_level_value,
                                category_name,
                                age_gender_name,
                                fgt_name
                                FROM
                                bdm_shoes_check_test_c
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
        /// 查询-检测项目-成品鞋-测试-定制类型-新增查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoseTestInspection_Custom_Insert(object OBJ)
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
                string sql = string.Empty ;

                sql = $@"select FINISHED_PRODUCT_CODE as code,FINISHED_PRODUCT_NAME as value from bdm_finished_product_m";
                DataTable dt1 = DB.GetDataTable(sql);

                sql = $@"select PB_TYPE_CODE as code,PB_TYPE_LEVEL as value from bdm_pb_type_m";
                DataTable dt2 = DB.GetDataTable(sql);

                sql = $@"select PRODUCT_LEVEL_CODE as code,PRODUCT_LEVEL_VALUE as value from bdm_product_level_m";
                DataTable dt3 = DB.GetDataTable(sql);

                sql = $@"select CATEGORY_CODE as code,CATEGORY_NAME as value from bdm_category_m";
                DataTable dt4 = DB.GetDataTable(sql);

                sql = $@"select AGE_GENDER_CODE as code,AGE_GENDER_NAME as value from bdm_age_gender_m";
                DataTable dt5 = DB.GetDataTable(sql);

                sql = $@"select fgt_code as  code ,fgt_name as value from bdm_fgt_m";
                DataTable dt6 = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);//bdm_finished_product_m 成品
                dic.Add("Data2", dt2);//bdm_pb_type_m PB Type级别
                dic.Add("Data3", dt3);//bdm_product_level_m 产品级别
                dic.Add("Data4", dt4);//bdm_category_m Category
                dic.Add("Data5", dt5);//bdm_age_gender_m 年龄性别
                dic.Add("Data6", dt6);//bdm_fgt_m fgt

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
        /// 保存-检测项目-成品鞋-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveShoseTestInspection_Custom_Insert(object OBJ)
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
                //string finished_product_code = jarr.ContainsKey("finished_product_code") ? jarr["finished_product_code"].ToString() : "";//查询条件 成品编号
                //string finished_product_name = jarr.ContainsKey("finished_product_name") ? jarr["finished_product_name"].ToString() : "";//查询条件 成品名称
                //string pb_type_code = jarr.ContainsKey("pb_type_code") ? jarr["pb_type_code"].ToString() : "";//查询条件 PB Type级别编号
                //string pb_type_level = jarr.ContainsKey("pb_type_level") ? jarr["pb_type_level"].ToString() : "";//查询条件 PB Type级别
                //string product_level_code = jarr.ContainsKey("product_level_code") ? jarr["product_level_code"].ToString() : "";//查询条件 产品级别编号
                //string product_level_value = jarr.ContainsKey("product_level_value") ? jarr["product_level_value"].ToString() : "";//查询条件 产品级别
                //string category_code = jarr.ContainsKey("category_code") ? jarr["category_code"].ToString() : "0";//查询条件 Category类别编号（开发系列）
                //string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "0";//查询条件 Category
                //string age_gender_code = jarr.ContainsKey("age_gender_code") ? jarr["age_gender_code"].ToString() : "0";//查询条件 年龄性别类别编号
                //string age_gender_name = jarr.ContainsKey("age_gender_name") ? jarr["age_gender_name"].ToString() : "0";//查询条件 年龄性别
                string fgt_code = jarr.ContainsKey("fgt_code") ? jarr["fgt_code"].ToString() : "0";//查询条件 fgt类型
                string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "0";//查询条件 fgt名称
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int countNO = DB.GetInt32($@"select count(1) from bdm_shoes_check_test_c where c_no='{c_no}'");
                if (countNO > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Number cannot be repeated!";
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from bdm_shoes_check_test_c where fgt_code='{fgt_code}'");
                if (count>0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Data cannot be consistent!";
                    return ret;
                }

                string sql = $@"insert into bdm_shoes_check_test_c 
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
        /// 删除-检测项目-成品鞋-测试-定制类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteShoseTestInspection_Custom(object OBJ)
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
                sql = $@"delete from bdm_shoes_check_test_c where id='{id}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"delete from bdm_shoes_check_test_u where c_id='{id}'";
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
        /// 查询-检测项目-成品鞋-测试-定制类型-勾选
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoseTestInspection_Custom_Check(object OBJ)
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
	                            m.islcll_item,
                            CASE
		                            WHEN u.id IS NOT NULL THEN
		                            'true' 
	                            ELSE 'false' 
	                            END AS ischeck 
                            FROM
	                            bdm_shoes_check_test_m m
	                            LEFT JOIN bdm_shoes_check_test_u u ON m.id = u.m_id and u.c_id='{cid}'
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
        /// 保存-检测项目-成品鞋-测试-定制类型-勾选
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveShoseTestInspection_Custom_Check(object OBJ)
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
                        int count = DB.GetInt32($@"select count(1) from bdm_shoes_check_test_u where m_id='{item["mid"]}' and c_id='{cid}'");
                        if (count<=0)
                        {
                            sql = $@"insert into bdm_shoes_check_test_u (m_id,c_id,createby,createdate,createtime) 
                                    values('{item["mid"]}','{cid}','{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                    if (item["xz"].ToString() == "False" || string.IsNullOrWhiteSpace(item["xz"].ToString()))
                    {
                        sql = $@"delete from bdm_shoes_check_test_u where m_id='{item["mid"]}' and c_id='{cid}'";
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

    public class code_value_OBJ
    {
        public string CODE { get; set; }
        public string VALUE { get; set; }
    }
}
