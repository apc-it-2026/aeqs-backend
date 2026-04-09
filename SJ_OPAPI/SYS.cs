using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SJ_OPAPI
{
    class SYS
    {
        /// <summary>
        /// 登录验证接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject Login(object OBJ)
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
                string keys = "UserCode,UserPwd";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion


                string id = Guid.NewGuid().ToString();

                string pwd = DB.GetString("select UserPwd from userinfo where UserCode=@UserCode", ReqP);
                string sql = string.Empty;
                if (jarr["UserPwd"].ToString() == pwd)
                {
                    string count = DB.GetString("select count(*) from usertoken where UserCode='" + jarr["UserCode"].ToString() + "'");
                    if (int.Parse(count) == 0)
                    {
                        sql = "insert into usertoken(UserCode,UserToken) values('" + jarr["UserCode"].ToString() + "','" + id + "')";
                        DB.ExecuteNonQueryOffline(sql);
                    }
                    else
                    {
                        sql = "update usertoken set UserToken='" + id + "' where UserCode='" + jarr["UserCode"].ToString() + "'";
                        DB.ExecuteNonQueryOffline(sql);
                    }
                    RetData.Add("UserToken", id);
                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.RetData = RetDataJson;
                    ret.ErrMsg = "登录成功！";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "账号或密码错误！";
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
        /// 新增系统公告接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddNotice(object OBJ)
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
                string keys = "Title,Content,ReleaseDate";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                int result = DB.ExecuteNonQueryOffline(@"insert into SystemWide(Title,content,ReleaseDate,CreateDate,CreateTime) 
        values(@Title,@Content,@ReleaseDate,@CreateDate,@CreateTime)", ReqP);
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
                    ret.ErrMsg = "保存失败！";
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
        /// 修改系统公告接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyNotice(object OBJ)
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
                string keys = "Title,Content,ReleaseDate";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                string title = DB.GetString(string.Format("select Title from SystemWide where Title='{0}' ", ReqP["Title"]));
                //判断是否存在
                if (string.IsNullOrWhiteSpace(title))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }

                int result = DB.ExecuteNonQueryOffline("UPDATE SystemWide set Content=@Content, ReleaseDate=@ReleaseDate where  Title=@Title", ReqP);
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
        /// 查询系统公告接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryNotice(object OBJ)
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
                string Page = jarr["Page"].ToString();//页数，第几页
                string PageRow = jarr["PageRow"].ToString();//每页显示记录数，例如：每页显示5条数据
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                #endregion

                string sql = string.Format(@"select Content, Title, ReleaseDate from(
                select id, Content, Title, ReleaseDate, @n:= @n + 1 as RN from SystemWide M,
                (select @n:= 0) d  {0}) tab 
                where  RN >{1}  {2}   limit {3} ", OrderBy, total, Where, PageRow);


                DataTable dtResult = DB.GetDataTable(sql);
                if (dtResult.Rows.Count > 0)
                {

                    int total2 = DB.GetInt32("select count(*) from SystemWide where 1=1 " + Where + "");
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

        /// <summary>
        /// 删除系统公告接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteNotice(object OBJ)
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
                string keys = "Title";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;



                #endregion




                int result = DB.ExecuteNonQueryOffline("delete from SystemWide where Title=@Title", ReqP);
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



    }
}
