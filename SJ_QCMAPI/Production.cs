using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class Production
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
                string sql;
                if (code == null && remarks == null)
                {
                    sql = "select id as 序号, formula_code as 公式编号 ,formula_name as 公式名称 ,ENUM_VALUE as 公式类型 ,formula_content as 公式内容 ,remarks as 备注 from BDM_FORMULA_M  a inner join SYS001M b on a.FORMULA_TYPE = b.ENUM_CODE where b.ENUM_TYPE = 'enum_formula_type'";
                }
                else
                {
                    sql = "select id as 序号, formula_code as 公式编号 ,formula_name as 公式名称 ,ENUM_VALUE as 公式类型 ,formula_content as 公式内容 ,remarks as 备注 from(select a.id, formula_code, formula_name, ENUM_VALUE, formula_content, remarks from BDM_FORMULA_M a inner join SYS001M b on FORMULA_TYPE = b.ENUM_CODE where b.ENUM_TYPE = 'enum_formula_type')  c where c.FORMULA_NAME like '%" + code + "%' OR c.REMARKS like '%" + remarks + "%'";
                }
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string sql = "delete from BDM_FORMULA_M Where id =" + id;
               
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
               // string user_code = SJeMES_Framework_NETCore.DBHelper.


                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string code = jarr.ContainsKey("txt_code") ? jarr["txt_code"].ToString() : "";
                string name = jarr.ContainsKey("txt_name") ? jarr["txt_name"].ToString() : "";
                string type = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : "";
                string results = jarr.ContainsKey("txt_content") ? jarr["txt_content"].ToString() : "";
                string remarks = jarr.ContainsKey("rtb_remarks") ? jarr["rtb_remarks"].ToString() : "";
                    
                string sql = $@"insert into BDM_FORMULA_M(formula_code ,formula_name ,formula_type ,formula_content ,remarks) values('{code}','{name}','{type}','{results}','{remarks}') ";
                
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string sql = "select id as 序号 ,formula_code as 公式编号 ,formula_name as 公式名称 ,formula_type as 公式类型 ,formula_content as 公式内容 ,remarks as 备注 from BDM_FORMULA_M Where id=" + id;
               
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string name = jarr.ContainsKey("txt_name") ? jarr["txt_name"].ToString() : "";
                string type1 = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : "";
                string type = type1 == "自定义" ? "1" : "0";

                string results = jarr.ContainsKey("txt_content") ? jarr["txt_content"].ToString() : "";
                string remarks = jarr.ContainsKey("rtb_remarks") ? jarr["rtb_remarks"].ToString() : "";

                string sql = $@"update  BDM_FORMULA_M set formula_name = '{name}',formula_type = '{type}',formula_content = '{results}',remarks = '{remarks}' where id = {id}";

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
