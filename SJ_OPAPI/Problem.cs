using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{

#warning  本人未测试

    class Problem
    {




        #region ProblemType
        /// <summary>
        /// 添加常见问题分类接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddProblemType(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);



                #region 获取参数
                string keys = "TypeCode,TypeName,TypeDesc";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                int result = DB.ExecuteNonQueryOffline(@"insert into ProblemType(TypeCode,TypeName,TypeDesc,CreateDate,CreateTime) 
        values(@TypeCode,@TypeName,@TypeDesc,@CreateDate,@CreateTime)", ReqP);
                if (result > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "保存成功！";
                    //ret.RetData = null;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "插入失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }



            return ret;

        }



        /// <summary>
        /// 修改常见问题分类接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyProblemType(object OBJ)
        {



            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);



                #region 获取参数
                string keys = "TypeCode,TypeName,TypeDesc";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                string TypeCode = DB.GetString(string.Format("select TypeCode from ProblemType where TypeCode='{0}' ", ReqP["TypeCode"]));
                //判断是否存在
                if (string.IsNullOrWhiteSpace(TypeCode))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }

                int result = DB.ExecuteNonQueryOffline("UPDATE ProblemType set TypeName=@TypeName, TypeDesc=@TypeDesc where  TypeCode=@TypeCode", ReqP);
                if (result > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "修改成功！";
                    //ret.RetData = null;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "修改失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }


        /// <summary>
        /// 删除常见问题分类接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteProblemType(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 获取参数
                string keys = "TypeCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;



                #endregion




                int result = DB.ExecuteNonQueryOffline("delete from ProblemType where TypeCode=@TypeCode", ReqP);
                if (result > 0)
                {

                    //RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "删除成功！";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "删除失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }



        /// <summary>
        /// 查询常见问题分类接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryProblemType(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 获取参数
                string keys = "Where,OrderBy,PageRow,Page";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();


                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数,第几页
                string PageRow = jarr["PageRow"].ToString();//每页显示记录数,例如：每页显示5条数据
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                #endregion

                string sql = string.Format(@"select TypeCode,TypeName,TypeDesc from(
                select id, TypeCode,TypeName,TypeDesc, @n:= @n + 1 as RN from ProblemType M,
                (select @n:= 0) d  {0}) tab 
                where  RN >{1}  {2}   limit {3} ", OrderBy, total, Where, PageRow);


                DataTable dtResult = DB.GetDataTable(sql);
                if (dtResult.Rows.Count > 0)
                {
                    int total2 = DB.GetInt32("select count(*) from ProblemType where 1=1 " + Where + "");
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("data", json);
                    p.Add("total", total2);

                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }

        #endregion

        #region Problems


        /// <summary>
        /// 添加常见问题接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddProblems(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);



                #region 获取参数
                string keys = "problem,TypeCode,solution";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                int result = DB.ExecuteNonQueryOffline(@"insert into Problems(problem,TypeCode,solution,createdate,createtime) 
        values(@problem,@TypeCode,@solution,@createdate,@createtime)", ReqP);
                if (result > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "保存成功！";
                    //ret.RetData = null;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "插入失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }



            return ret;

        }



        /// <summary>
        /// 修改常见问题
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyProblems(object OBJ)
        {



            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);



                #region 获取参数
                string keys = "problem,TypeCode,solution";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                string TypeCode = DB.GetString(string.Format("select problem from Problems where problem='{0}' ", ReqP["problem"]));
                //判断是否存在
                if (string.IsNullOrWhiteSpace(TypeCode))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }

                int result = DB.ExecuteNonQueryOffline(@"UPDATE Problems set  solution=@solution,TypeCode=@TypeCode where  problem=@problem ", ReqP);
                if (result > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "修改成功！";
                    //ret.RetData = null;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "修改失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }


        /// <summary>
        /// 删除常见问题
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteProblems(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 获取参数
                string keys = "problem";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;



                #endregion




                int result = DB.ExecuteNonQueryOffline("delete from Problems where problem=@problem", ReqP);
                if (result > 0)
                {

                    //RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "删除成功！";
                    //ret.RetData = RetDataJson;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "删除失败！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }



        /// <summary>
        /// 查询常见问题接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryProblems(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 获取参数
                string keys = "Where,OrderBy,PageRow,Page";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;

                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数,第几页
                string PageRow = jarr["PageRow"].ToString();//每页显示记录数,例如：每页显示5条数据
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                #endregion

                string sql = string.Format(@"select problem,TypeCode,solution from(
                select id, problem,TypeCode,solution, @n:= @n + 1 as RN from Problems M,
                (select @n:= 0) d  {0}) tab 
                where  RN >{1}  {2}   limit {3} ", OrderBy, total, Where, PageRow);


                DataTable dtResult = DB.GetDataTable(sql);
                if (dtResult.Rows.Count > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult);
                    ret.IsSuccess = true;
                    ret.RetData = RetDataJson;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }

        #endregion 
    }
}
