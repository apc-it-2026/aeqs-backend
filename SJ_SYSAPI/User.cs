using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace SJ_SYSAPI
{
    /// <summary>
    /// 用户API
    /// </summary>
    public class User
    {
        /// <summary>
        /// 获取用户权限资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserPower(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                ret.IsSuccess = true;

                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();//条件

                #endregion
                #region 逻辑


                #region 数据获取SQL
                string datasql = @"
select
*
from sysmenu01m

";
                #endregion



                DataTable dt = DBSYS.GetDataTable(datasql);

                #region 数据获取SQL
                datasql = @"
select
*
from userpower
where UserCode='" + UserCode + @"'

";
                #endregion



                DataTable dtRole = DB.GetDataTable(datasql);

                DataTable dtRole2 = dtRole.Clone();
                dtRole2.Columns.Remove("id");

                foreach (DataRow dr in dt.Rows)
                {
                    DataRow newDR = dtRole2.NewRow();

                    bool isHas = false;
                    foreach (DataRow dr2 in dtRole.Rows)
                    {
                        if (dr2["name"].ToString() == dr["name"].ToString())
                        {
                            isHas = true;

                            foreach (DataColumn dc in dtRole.Columns)
                            {
                                if (dc.ColumnName != "id")
                                {
                                    newDR[dc.ColumnName] = dr2[dc.ColumnName];
                                }
                            }
                        }

                    }


                    newDR["MenuCode"] = dr["name"].ToString();
                    newDR["MenuName"] = dr["meta_title"].ToString();


                    if (!isHas)
                    {
                        newDR["UserCode"] = UserCode;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            newDR[dc.ColumnName] = dr[dc.ColumnName];
                        }

                    }

                    dtRole2.Rows.Add(newDR);
                }




                string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dtRole2);
                Dictionary<string, object> p = new Dictionary<string, object>();




                p.Add("data", json);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }


        /// <summary>
        /// 获取系统用户列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                ret.IsSuccess = true;

                #region 接口参数
                string WHERE = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑


                #region 数据获取SQL
                string datasql = @"
select
id,user_code,user_name
from userinfo
";
                #endregion

                DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                WHERE = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, WHERE);

                datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE 1=1 " + WHERE;


                DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                    PageRow, Page, OrderBy));
                if (dt.Rows.Count > 0)
                {
                    string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    int total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");

                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    string headkey = "id,账号,名称";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    //p.Add("team_no", json);
                    //p.Add("team_name", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
                }
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }


        /// <summary>
        /// CheckUser(检验用户)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckUser(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string BarCode = string.Empty;

                string UserCode = string.Empty;

                string UserPwd = string.Empty;

                if (Data.IndexOf("UserCode") > -1)
                {
                    UserCode= jarr["UserCode"].ToString();
                    UserPwd = jarr["UserPwd"].ToString();
                }
                if (Data.IndexOf("BarCode") > -1)
                {
                    BarCode = jarr["BarCode"].ToString();
                }



                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               
                DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string sql = @"
SELECT UserCode,MaxWindow,UserPwd
FROM SJEMSSYS.dbo.SYSUSER01M 
WHERE (UserCode=@UserCode AND UserPwd=@UserPwd) OR BarCode=@BarCode
";
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("@UserCode", UserCode);
                p.Add("@UserPwd", UserPwd.ToUpper());
                p.Add("@BarCode", BarCode);

                string MaxWindow = string.Empty;

                System.Data.IDataReader idr = DB2.GetDataTableReader(sql, p);
                if (idr.Read())
                {
                    MaxWindow = idr[1].ToString();
                    UserCode = idr[0].ToString();
                    UserPwd = idr[2].ToString();
                    ret.IsSuccess = true;

                    sql = @"
SELECT staff_name FROM HR001M 
WHERE user_code=@UserCode
";
                    p["@UserCode"] = UserCode;
                    string UserName = DB.GetString(sql, p);
                    if (string.IsNullOrEmpty(UserName))
                    {
                        UserName = UserCode;
                    }
                    Dictionary<string, string> a = new Dictionary<string, string>();
                    a.Add("MaxWindow", MaxWindow);
                    a.Add("UserName", UserName);
                    a.Add("UserCode", UserCode);
                    a.Add("UserPwd", UserPwd);
                    //RetData = "<MaxWindow>" + MaxWindow + @"</MaxWindow>";
                    //RetData += "<UserName>" + UserName + @"</UserName>";
                    //RetData += "<UserCode>" + UserCode + @"</UserCode>";
                    //RetData += "<UserPWD>" + UserPwd + @"</UserPWD>";
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                }
                else
                {
                    ret.ErrMsg= "账号或密码错误";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }


        /// <summary>
        /// ChangePasswork(修改密码)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChangePasswork(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string UserCode = jarr["UserCode"].ToString();
                string UserPwd = jarr["UserPwd"].ToString();
                string UserPwdNew = jarr["UserPwdNew"].ToString();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               

                string sql = @"
SELECT UserCode
FROM SJEMSSYS.dbo.SYSUSER01M 
WHERE UserCode=@UserCode AND UserPwd=@UserPwd
";
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("@UserCode", UserCode);
                p.Add("@UserPwd", UserPwd.ToUpper());
                p.Add("@UserPwdNew", UserPwdNew.ToUpper());

                string UserName = DB.GetString(sql, p);
                if (!string.IsNullOrEmpty(UserName))
                {
                    sql = @"
UPDATE SJEMSSYS.dbo.SYSUSER01M  set UserPwd=@UserPwdNew
where UserCode=@UserCode 
";
                    DB.ExecuteNonQueryOffline(sql, p);
                    ret.IsSuccess = true;

                }
                else
                {
                    ret.ErrMsg= "账号或密码错误";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }

        /// <summary>
        /// Login（登录)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Login(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            //SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                


                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CompanyID = jarr["CompanyCode"].ToString();//公司ID
                string CompanyName = jarr["CompanyName"].ToString();//公司名称
                string user_name = jarr["UserCode"].ToString();
                string pwd = jarr["UserPassword"].ToString();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
               
                if (!string.IsNullOrEmpty(CompanyID))
                {
                    string sql = "select * from SYSORG01M where org = '" + CompanyID + "' and orgname = '" + CompanyName + "'";
                    DataTable sqldt= DB.GetDataTable(sql);
                    if (sqldt.Rows.Count>0)
                    {
                        //DB2= new SJeMES_Framework_NETCore.DBHelper.DataBase(sqldt.Rows[0]["DBType"].ToString(), sqldt.Rows[0]["DBServer"].ToString(),sqldt.Rows[0]["DBName"].ToString(),sqldt.Rows[0]["DBUser"].ToString(),sqldt.Rows[0]["DBPassword"].ToString(),string.Empty);
                        sql = "select * from "+DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd + "'";
                        DataTable dt = DB.GetDataTable(sql);
                        string id= Guid.NewGuid().ToString();

                        if(dt.Rows.Count ==0)
                        {
                            dt = DB.GetDataTable("select * from " + DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd.ToLower() + "'");
                        }
                        if (dt == null || dt.Rows.Count == 0)
                            throw new Exception("Incorrect username or password，Check, please！");

                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("UserToken", id);
                        p.Add("userName", dt.Rows[0]["UserCode"].ToString());
                        p.Add("pwd", pwd);
                        p.Add("uploadurl", sqldt.Rows[0]["uploadurl"].ToString());

                        if (dt.Rows.Count > 0)
                        {
                           string count = DB.GetString("select count(*) from usertoken where CompanyCode='"+ CompanyID + "' and lower(UserCode)='" + user_name.ToLower() + "'");
                            if (int.Parse(count)==0)
                            {
                                sql = "insert into usertoken(CompanyCode,UserCode,UserToken) values('" + CompanyID + "','" + user_name + "','" + id + "')";
                                DB.ExecuteNonQueryOffline(sql);
                            }
                            else
                            {
                                sql = "update usertoken set UserToken='"+id+ "' where CompanyCode='" + CompanyID + "' and lower(UserCode)='" + user_name.ToLower() + "'";
                                DB.ExecuteNonQueryOffline(sql);
                            }
                            ret.IsSuccess = true;
                            ret.ErrMsg = "Landed successfully";
                            //ret.ErrMsg = "SUCCESS";
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "账号或密码错误！";
                            //ret.ErrMsg = "ERRO！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "授权失败！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "公司ID不能为空！";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAppVersionInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CompanyID = jarr["CompanyCode"].ToString();//公司ID
                string CompanyName = jarr["CompanyName"].ToString();//公司名称
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                Dictionary<string, object> p = new Dictionary<string, object>();

                string uploadurl = DB.GetString($@"SELECT TOP 1 uploadurl FROM SYSORG01M WHERE org='{CompanyID}' and orgname='{CompanyName}'");

                if (string.IsNullOrEmpty(uploadurl))
                {
                    throw new Exception("公司ID错误");
                }
                uploadurl = uploadurl.Replace("api/commoncall", "");

                long version = -1;
                string downloadUrl = "";
                string updateType = "";
                DataTable version_dt = DB.GetDataTable($@"
                                                                SELECT
	                                                                app_version 'version',
	                                                                download_url 'downloadUrl'
                                                                FROM
	                                                                APP_VERSION
                                                                ");
                if (version_dt.Rows.Count > 0)
                {
                    version = Convert.ToInt64(version_dt.Rows[0]["version"].ToString());
                    downloadUrl = version_dt.Rows[0]["downloadUrl"].ToString();
                    if (downloadUrl.ToLower().LastIndexOf(".apk") != -1)
                        updateType = "2";//全包
                    else if (downloadUrl.ToLower().LastIndexOf(".wgt") != -1)
                        updateType = "1";//增量包
                }
                p.Add("version", version);
                p.Add("downloadUrl", uploadurl + downloadUrl);
                p.Add("updateType", updateType);
                ret.IsSuccess = true;
                ret.ErrMsg = "获取成功";
                ret.RetData1 = p;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPassword(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.MySQL DB2 = new SJeMES_Framework_NETCore.DBHelper.MySQL();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user_code = jarr["user_code"].ToString();
                string pwd = jarr["pwd"].ToString();
                string UserPwdNew = jarr["UserPwdNew"].ToString();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string sql = @"
                                SELECT user_code
                                FROM userinfo 
                                WHERE user_code=@user_code AND pwd=@pwd";

                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("@user_code", user_code);
                p.Add("@pwd", pwd.ToUpper());
                p.Add("@UserPwdNew", UserPwdNew.ToUpper());

                string UserName = DB.GetString(sql, p);
                if (!string.IsNullOrEmpty(UserName))
                {
                    sql = @"
                        UPDATE  userinfo set pwd=@UserPwdNew
                        where user_code=@user_code 
                        ";
                    DB.ExecuteNonQueryOffline(sql, p);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "修改成功！";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "账号或密码错误";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ResetPassword(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.MySQL DB2 = new SJeMES_Framework_NETCore.DBHelper.MySQL();
            try
            {
                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user_code = jarr["user_code"].ToString();
                //string pwd = jarr["pwd"].ToString();
                ////Md5("pwd");
                //string tel = jarr["tel"].ToString();
                //string md1 = MD5Util.MD5(pwd);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               
                string sql = @"
SELECT tel
FROM userinfo 
WHERE user_code=@user_code";

                //string UserPasswordNews = SJeMES_Framework_NETCore.Common.Security.MD5(tel);//MD5加密
                Dictionary<string, object> p = new Dictionary<string, object>();

                p.Add("@user_code", user_code);
                //p.Add("@pwd", pwd);
               // p.Add("@tel", UserPasswordNews);
                
                string tell = DB.GetString(sql, p);
                if (!string.IsNullOrEmpty(tell))
                {
                    string tel= SJeMES_Framework_NETCore.Common.Security.MD5DoWork(tell);
                    sql = @"
UPDATE  userinfo set pwd='"+tell+@"'
where user_code='"+ user_code + "'";
                 
                    //FormsAuthentication.HashPasswordForStoringInConfigFile(pwd, "MD5").ToLower();
                    //Get_MD5(pwd, pwd);
                    DB.ExecuteNonQueryOffline(sql);
                    //Dictionary<string, object> a = new Dictionary<string, object>();
                    //a.Add("user_code", user_code);
                    //a.Add("pwd", pwd);
                    ret.ErrMsg = "重置成功！";
                    //ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                    ret.IsSuccess = true;

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "号码为空";
                }
                //else
                //{
                //    ret.ErrMsg = "重置失败！";
                //}
            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }
        //public static string Md5(string str)
        //{
        //    MD5 md5 = MD5.Create();
        //    byte[] bufstr = Encoding.GetEncoding("GBK").GetBytes(str);
        //    byte[] hashstr = md5.ComputeHash(bufstr);
        //    string md5Str = string.Empty;
        //    for (int i = 0; i < hashstr.Length; i++)
        //    {
        //        md5Str += hashstr[i].ToString("X");
        //    }
        //    return md5Str;
        //}
        /// <summary>
        /// 修改用户权限
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditPower(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
     
            try
            {
                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string UserCode = jarr["UserCode"].ToString();
                var o = JObject.Parse(Data);
                string Table = o["table"].ToString();
                

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                DB.ExecuteNonQueryOffline(@"delete from userpower where UserCode='" + UserCode + @"'");
                if (!string.IsNullOrEmpty(Table))
                {
                    string sql = string.Empty;
                    DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(Table);

                    foreach(DataRow dr in dt.Rows)
                    {
                        Dictionary<string, object> p = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByDataRow(dr);

                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "userpower", p), p);
                    }

                    ret.IsSuccess = true;
                    ret.ErrMsg = "保存成功！";

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "用户不能为空！";
                }
            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }

        /// <summary>
        /// 获取用户信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserInfo(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.MySQL DB2 = new SJeMES_Framework_NETCore.DBHelper.MySQL();
            try
            {
                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string UserCode = jarr["UserCode"].ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(UserCode))
                {
                    sql = "select * from userinfo where user_code='" + UserCode + "'";
                    DataTable dt= DB.GetDataTable(sql);
                    if (dt.Rows.Count> 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                        ret.IsSuccess = true;
                        ret.RetData = json;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "用户不能为空！";
                }
            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }

        /// <summary>
        /// 保存用户信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveUserInfo(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.MySQL DB2 = new SJeMES_Framework_NETCore.DBHelper.MySQL();
            try
            {
                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string UserCode = jarr["UserCode"].ToString();
                var o = JObject.Parse(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               
                string sql = string.Empty;
                var Table = o["Table"].ToString();
                DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(Table);
                if (dt.Rows.Count>0)
                {
                    //user_code用户代号，user_name用户名称，department部门，work_center工作中心，work_site工站，roles角色，
                    //mobile手机号码，tel联系电话，weixin微信，mail电子邮件，homepage首页模板，work_no工号，user_sex用户性别，
                    //user_guid身份证号，job职位，status账号状态，enable是否启用
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("user_code", dt.Rows[i]["user_code"].ToString());
                        dic.Add("user_name", dt.Rows[i]["user_name"].ToString());
                        dic.Add("department", dt.Rows[i]["department"].ToString());
                        dic.Add("work_center", dt.Rows[i]["work_center"].ToString());
                        dic.Add("work_site", dt.Rows[i]["work_site"].ToString());
                        dic.Add("roles", dt.Rows[i]["roles"].ToString());
                        dic.Add("mobile", dt.Rows[i]["mobile"].ToString());
                        dic.Add("tel", dt.Rows[i]["tel"].ToString());
                        dic.Add("weixin", dt.Rows[i]["weixin"].ToString());
                        dic.Add("mail", dt.Rows[i]["mail"].ToString());
                        dic.Add("home_page", dt.Rows[i]["home_page"].ToString());
                        dic.Add("work_no", dt.Rows[i]["work_no"].ToString());
                        dic.Add("user_sex", dt.Rows[i]["user_sex"].ToString());
                        dic.Add("user_guid", dt.Rows[i]["user_guid"].ToString());
                        dic.Add("job", dt.Rows[i]["job"].ToString());
                        dic.Add("status", dt.Rows[i]["status"].ToString());
                        dic.Add("enable", dt.Rows[i]["enable"].ToString());
                        sql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("userinfo"," user_code=@user_code" ,dic);
                        if(DB.ExecuteNonQueryOffline(sql, dic)==0)
                        {
                            sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "userinfo", dic);
                            DB.ExecuteNonQueryOffline(sql, dic);
                        }
                        ret.IsSuccess = true;
                        ret.ErrMsg = "保存成功！";
                    }
                  
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传参为空！";
                }
            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            

            return ret;
        }


        /// <summary>
        /// 查询用户信息——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserListData(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

            try
            { 
                Data = ReqObj.Data.ToString(); 
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string strWhere = jarr.ContainsKey("strWhere") ? jarr["strWhere"].ToString() : "";
                string strSql = string.Empty;
                if (!string.IsNullOrEmpty(strWhere))
                    strSql = " and UserCode like '%"+ strWhere + "%'";

                string sql = @" select UserCode 用户代号 from SYSUSER01M where 1=1 "+ strSql;
                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }


        /// <summary>
        /// 修改用户密码(万邦)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdatePassword(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string guid = string.Empty;
            string sql = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.MySQL DB2 = new SJeMES_Framework_NETCore.DBHelper.MySQL();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user_code = jarr["user_code"].ToString();
                string pwd = jarr["pwd"].ToString();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                if (DB != null)
                {
                    //修改密码
                    sql = @"select * from SYSUSER01M(nolock) where lower(UserCode) = '{0}'";
                    sql = string.Format(sql, user_code);
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        sql = "update SYSUSER01M set UserPwd='{0}',BarCode='{0}' where lower(UserCode) = '{1}'";
                        sql = string.Format(sql, pwd, user_code);
                        DB.ExecuteNonQueryOffline(sql);
                    }
                    else
                        throw new Exception("用户代号不存在！");
                }
                else
                    throw new Exception("500:ERROR！");

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }


        /// <summary>
        /// PDALogin（PDA登录)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PDALogin(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();



                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user_name = jarr["UserCode"].ToString();
                string pwd = jarr["UserPassword"].ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                pwd = SJeMES_Framework_NETCore.Common.Security.MD5DoWork(pwd.ToLower());
                string sql = "select top 1 * from SYSORG01M -- where org = 'apemes' and orgname = 'WB_QKPI'";
                DataTable sqldt = DB.GetDataTable(sql);
                if (sqldt.Rows.Count > 0)
                {
                    DB2= new SJeMES_Framework_NETCore.DBHelper.DataBase(sqldt.Rows[0]["DBType"].ToString(), sqldt.Rows[0]["DBServer"].ToString(),sqldt.Rows[0]["DBName"].ToString(),sqldt.Rows[0]["DBUser"].ToString(),sqldt.Rows[0]["DBPassword"].ToString(),string.Empty);
                    sql = "select * from " + DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    string id = Guid.NewGuid().ToString();

                    if (dt.Rows.Count == 0)
                    {
                        dt = DB.GetDataTable("select * from " + DB.ChangeKeyWord("SYSUSER01M") + @" where lower(UserCode) = '" + user_name.ToLower() + "' and UserPwd = '" + pwd.ToLower() + "'");
                    }
                    if (dt == null || dt.Rows.Count == 0)
                        throw new Exception("账号或密码错误，请检查！");

                    DataTable dd = DB2.GetDataTable($"select * from HR001M where USER_CODE='{user_name}'");
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("UserToken", id);
                    //p.Add("userName", dt.Rows[0]["UserCode"].ToString());
                    //p.Add("pwd", pwd);
                    //p.Add("img", "空");
                    p.Add("hr", dd);


                    if (dt.Rows.Count > 0)
                    {
                        string count = DB.GetString("select count(*) from usertoken where CompanyCode='apemes' and lower(UserCode)='" + user_name.ToLower() + "'");
                        if (int.Parse(count) == 0)
                        {
                            sql = "insert into usertoken(CompanyCode,UserCode,UserToken) values('apemes','" + user_name + "','" + id + "')";
                            DB.ExecuteNonQueryOffline(sql);
                        }
                        else
                        {
                            sql = "update usertoken set UserToken='" + id + "' where CompanyCode='apemes' and lower(UserCode)='" + user_name.ToLower() + "'";
                            DB.ExecuteNonQueryOffline(sql);
                        }

                        ret.IsSuccess = true;
                        ret.ErrMsg = "登陆成功";
                        //ret.ErrMsg = "SUCCESS";
                        ret.RetData1 = p;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "账号或密码错误！";
                        //ret.ErrMsg = "ERRO！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "授权失败！";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }



            return ret;
        }
    }
}
