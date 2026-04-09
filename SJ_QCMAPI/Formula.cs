using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class Formula
    {
        /// <summary>
        /// 获取公式计算维护数据接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFormulaList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string code = jarr.ContainsKey("txt_code") ? jarr["txt_code"].ToString() : "";
                string remarks = jarr.ContainsKey("txt_remarks") ? jarr["txt_remarks"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
               
                string where = string.Empty;
                if (!string.IsNullOrEmpty(code))
                    where += " and formula_name_1 like '%" + code + "%'";
                if (!string.IsNullOrEmpty(remarks))
                    where += "and remarks_1 like '%" + remarks + "%'";

                string sql = $@"
SELECT

    formula_code_1,
	formula_name_1,
	formula_type_1,
    formula_content_1,
	remarks_1
FROM
    (

    SELECT

        formula_code as formula_code_1,
        formula_name AS formula_name_1,
        'Customize' AS formula_type_1,
        formula_content AS formula_content_1,
        remarks AS remarks_1
    FROM
        BDM_FORMULA_M
        UNION
        select ENUM_CODE formula_code_1, ENUM_VALUE formula_name_1,
        'Universal' AS formula_type_1,
        ENUM_VALUE AS formula_content_1,
        '' AS remarks_1
        from sys001m where enum_type = 'enum_general_formula'
    )
where 1=1 {where}
    order by formula_type_1 desc";

                //DataTable dt = DB.GetDataTable(sql);

                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            //finally
            //{
            //    DB.Close();//关闭事务
            //}
            return ret;
           
        }

        /// <summary>
        /// 删除公式计算数据接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DelFormulaList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string formula_code = jarr.ContainsKey("formula_code") ? jarr["formula_code"].ToString() : "";
                string sql = $@"delete from BDM_FORMULA_M Where formula_code ='{formula_code}'";
               
                int dt = DB.ExecuteNonQuery(sql);

                #endregion

                ret.RetData = "";
                ret.IsSuccess = true;
                DB.Commit();//提交事务
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
        /// 添加公式计算数据接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsFormulaList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj); 


                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string code = jarr.ContainsKey("txt_code") ? jarr["txt_code"].ToString() : "";
                string name = jarr.ContainsKey("txt_name") ? jarr["txt_name"].ToString() : "";
                string type = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : "";
                string content = jarr.ContainsKey("txt_content") ? jarr["txt_content"].ToString() : "";
                string remarks = jarr.ContainsKey("rtb_remarks") ? jarr["rtb_remarks"].ToString() : "";

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sqls = $"select 1 from BDM_FORMULA_M where formula_code='{code}'";
                string sql = string.Empty;
                DataTable dt = DB.GetDataTable(sqls);

                if (dt.Rows.Count > 0)
                {
                    throw new Exception("Formula number: 【" + code + "】repeated, please re-enter!");
                }

                
                //sql = $@"insert into BDM_FORMULA_M(formula_code ,formula_name ,formula_type ,formula_content ,remarks ,CREATEBY ,CREATEDATE ,CREATETIME) 
                //           values('{code}','{name}','{type}','{results}','{remarks}','{user}','{date}','{time}') ";


                sql = $@"insert into BDM_FORMULA_M(formula_code ,formula_name ,formula_type ,formula_content ,remarks ,CREATEBY ,CREATEDATE ,CREATETIME) 
                                            values(@formula_code,@formula_name,@formula_type,@formula_content,@remarks,@CREATEBY,@CREATEDATE,@CREATETIME) ";
                
                Dictionary<string, object> p_FORMULA_M = new Dictionary<string, object>();
                p_FORMULA_M.Add("formula_code", code);
                p_FORMULA_M.Add("formula_name", name);
                p_FORMULA_M.Add("formula_type", type);
                p_FORMULA_M.Add("formula_content", content);
                p_FORMULA_M.Add("remarks", remarks);
                p_FORMULA_M.Add("CREATEBY", user);
                p_FORMULA_M.Add("CREATEDATE", date);
                p_FORMULA_M.Add("CREATETIME", time); 
                DB.ExecuteNonQuery(sql, p_FORMULA_M);


                #endregion
                ret.RetData = "Saved successfully！";
                ret.IsSuccess = true;
                DB.Commit();//提交事务
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
        /// （用于修改）获取一条公式计算维护数据接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetByIdFormulaList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string formula_code = jarr.ContainsKey("formula_code") ? jarr["formula_code"].ToString() : "";
                string sql = $@"select formula_code,formula_name,formula_type,formula_content,remarks from BDM_FORMULA_M Where formula_code='{formula_code}'" ;
               
                DataTable dt = DB.GetDataTable(sql);

                #endregion

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            //finally
            //{
            //    DB.Close();//关闭事务
            //}
            return ret;

        }
        /// <summary>
        /// 修改一条公式计算维护数据接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateByIdFormulaList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string formula_code = jarr.ContainsKey("formula_code") ? jarr["formula_code"].ToString() : "";
                string name = jarr.ContainsKey("txt_name") ? jarr["txt_name"].ToString() : "";
                string type1 = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : "";
                string type = type1 == "自定义" ? "1" : "0";
                string results = jarr.ContainsKey("txt_content") ? jarr["txt_content"].ToString() : "";
                string remarks = jarr.ContainsKey("rtb_remarks") ? jarr["rtb_remarks"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("d");//日期
                string time = DateTime.Now.ToString("t");//时间

                string sql = $@"update  BDM_FORMULA_M set formula_name = '{name.Trim()}',formula_type = '{type.Trim()}',formula_content = '{results.Trim()}',remarks = '{remarks.Trim()}', MODIFYBY = '{user}' ,MODIFYDATE = '{date}',MODIFYTIME = '{time}' where formula_code = '{formula_code}'";

                int dt = DB.ExecuteNonQuery(sql);

                #endregion

                ret.RetData = "";
                ret.IsSuccess = true;
                DB.Commit();//提交事务
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
    }
}
