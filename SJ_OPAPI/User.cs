using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SJ_OPAPI
{
    
   public  class User
    {
        /// <summary>
        /// 注册页
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject Register(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();//用户名
                string UserPwd = jarr["UserPwd"].ToString();//密码
                string UserName = jarr["UserName"].ToString();//真实姓名
                string Phone = jarr["Phone"].ToString();//电话
                string Mail = jarr["Mail"].ToString();//邮箱
                //string CompanyName = jarr["CompanyName"].ToString();//公司名称
                //string UserType = jarr["UserType"].ToString();//用户类型
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(UserCode) && !string.IsNullOrEmpty(UserPwd))
                {
                    string User = DB.GetString("select UserCode from UserInfo where UserCode='"+ UserCode + "'");
                    if (string.IsNullOrEmpty(User))
                    {
                       string sql = "insert into UserInfo(UserCode,UserPwd,UserName,Phone,Mail) values('" + UserCode + "','" + UserPwd + "','" + UserName + @"',
                     '" + Phone + "','" + Mail + "')";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "注册成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该用户已注册！";
                    }
                   
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "用户名及密码不能为空！";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        ///修改个人基础资料接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyUser(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();//用户名
                //string UserPwd = jarr["UserPwd"].ToString();//密码
                string UserName = jarr["UserName"].ToString();//真实姓名
                string Phone = jarr["Phone"].ToString();//电话
                string Mail = jarr["Mail"].ToString();//邮箱
                string CompanyCode = jarr["CompanyCode"].ToString();
                string CompanyName = jarr["CompanyName"].ToString();//公司名称
                string Address = jarr["Address"].ToString();
                string DocumentType = jarr["DocumentType"].ToString();
                string DocumentCode = jarr["DocumentCode"].ToString();
                //string DocumentTime = Convert.ToString(jarr["DocumentTime"]);
                string DocumentTime = "";
                string PersonIdCard = jarr["PersonIdCard"].ToString();
                string NationalIdCard = jarr["NationalIdCard"].ToString();
                string status = jarr["status"].ToString();
                string UserType = jarr["UserType"].ToString();//用户类型
                
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(UserCode))
                {
                    string User = DB.GetString("select UserCode from UserInfo where UserCode='" + UserCode + "'");
                    if (!string.IsNullOrEmpty(User))
                    {
                        string sql = "update UserInfo set UserCode='"+ UserCode + "',UserName='"+ UserName + "',Phone='"+ Phone + "',Mail='" + Mail + @"',
                        CompanyCode='"+ CompanyCode + "',CompanyName='"+ CompanyName + "',Address='"+ Address + "',DocumentType='"+ DocumentType + @"',
                        DocumentCode='"+ DocumentCode + "',DocumentTime='"+ DocumentTime + "',PersonIdCard='"+ PersonIdCard + "',NationalIdCard='"+ NationalIdCard + "',status='"+ status + @"',
                        UserType='"+ UserType + "' where UserCode='"+ UserCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "修改成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该用户不存在！";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "用户名不能为空！";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        ///查询个人基础资料接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryUser(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();//用户名
                
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(UserCode))
                {
                    string sql = "select * from UserInfo where UserCode='"+ UserCode + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count>0)
                    {
                        string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                        ret.RetData = json;
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无此数据！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 查询个人认证接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryUserList(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string Where = jarr["Where"].ToString();
                string OrderBy = jarr["OrderBy"].ToString();
                string PageRow = jarr["PageRow"].ToString();
                string Page = jarr["Page"].ToString();
                #endregion
                #region 逻辑
                int total2 = (int.Parse(Page) - 1) * int.Parse(PageRow);
                int total = DB.GetInt32("select count(*) from UserInfo where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from UserInfo M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("data", json);
                p.Add("total", total);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 重置密码接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ResetPwd(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(UserCode))
                {
                    DataTable dt = DB.GetDataTable("select * from UserInfo where UserCode='" + UserCode + "'");
                    if (dt.Rows.Count>0)
                    {
                        string pwd = GDSJFramework_NETCore.Common.Security.MD5DoWork(dt.Rows[0]["Phone"].ToString());
                        string sql = "update UserInfo set UserPwd='"+ pwd + "' where UserCode='" + UserCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "重置密码成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该用户不存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 修改密码接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyPwd(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);


                #region 接口参数
                string UserCode = jarr["UserCode"].ToString();
                string UserPasswordOld = jarr["UserPasswordOld"].ToString();
                string UserPasswordNew = jarr["UserPasswordNew"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(UserCode))
                {
                    DataTable dt = DB.GetDataTable("select * from UserInfo where UserCode='" + UserCode + "' and UserPwd='"+ UserPasswordOld + "'");
                    if (dt.Rows.Count > 0)
                    {
                        string sql = "update UserInfo set UserPwd='" + UserPasswordNew + "' where UserCode='" + UserCode + "' and UserPwd='" + UserPasswordOld + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "修改密码成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该用户不存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        /// <summary>
        /// 忘记密码接口（手机）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetPasswordMobile(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            GDSJFramework_NETCore.DBHelper.DataBase DMP = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "Phone,UserCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["Phone"].ToString()))
                {
                    #region 获取短信数据库
                    string Server = DB.GetString("select Server from systeminfo");
                    string DBType = DB.GetString("select DBType from systeminfo");
                    DMP=new GDSJFramework_NETCore.DBHelper.DataBase(DBType, Server);
                    #endregion
                   // string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(ReqP["UserCode"].ToString()))
                    {
                        Regex regex = new Regex("^1[34578]\\d{9}$");
                        if (regex.IsMatch(ReqP["Phone"].ToString()))
                        {
                            string phone= DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + ReqP["UserCode"].ToString() + "' and Phone='"+ ReqP["Phone"].ToString() + "'");
                            if (phone!="0")
                            {
                                System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);
                                Random rd = new Random();
                                for (int i = 0; i < 4; i++)
                                {
                                    newRandom.Append(constant[rd.Next(10)]);
                                }
                                //return newRandom.ToString();
                                string sendtext = "【商基】验证码：" + newRandom.ToString() + "，用于平台登录。验证码提供给他人可能导致账号被盗，请勿泄露，谨防被骗。";
                                string sql = "insert into ORDER_SMS(usercode,phone,date,time,sendtext) VALUES('" + ReqP["UserCode"].ToString() + "','" + ReqP["Phone"].ToString() + "'," +
                                    "'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + sendtext + "')";
                                DMP.ExecuteNonQueryOffline(sql);
                                sql = "update UserInfo set VerificationCode='" + newRandom.ToString() + "' where UserCode='" + ReqP["UserCode"].ToString() + "'";
                                DB.ExecuteNonQueryOffline(sql);
                                ret.IsSuccess = true;
                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "该手机号并不是该用户注册的手机号！";
                            }
                           
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "手机号格式不正确！";
                        } 
                        
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 忘记密码接口（验证用户名及手机）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetMobile(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            GDSJFramework_NETCore.DBHelper.DataBase DMP = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "PhoneOrMail,UserCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["PhoneOrMail"].ToString()))
                {
                    #region 获取短信数据库
                    //string Server = DB.GetString("select Server from systeminfo");
                    //string DBType = DB.GetString("select DBType from systeminfo");
                    //DMP = new GDSJFramework_NETCore.DBHelper.DataBase(DBType, Server);
                    #endregion
                    // string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(ReqP["UserCode"].ToString()))
                    {
                        Regex regex = new Regex("^1[34578]\\d{9}$");
                        if (regex.IsMatch(ReqP["Phone"].ToString()))
                        {
                            string phone = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + ReqP["UserCode"].ToString() + "' " +
                                "and (Mail='" + ReqP["PhoneOrMail"].ToString() + "' or Phone='" + ReqP["PhoneOrMail"].ToString() + "')");
                            if (phone != "0")
                            {
                                ret.IsSuccess = true;
                                ret.ErrMsg = "验证成功！";
                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "该手机号并不是该用户注册的手机号！";
                            }

                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "手机号格式不正确！";
                        }

                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 忘记密码接口（邮箱）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetPasswordMail(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            GDSJFramework_NETCore.DBHelper.DataBase DMP = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "Mail,UserCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["Mail"].ToString()))
                {
                    #region 获取短信数据库
                    string Server = DB.GetString("select Server from systeminfo");
                    string DBType = DB.GetString("select DBType from systeminfo");
                    DMP = new GDSJFramework_NETCore.DBHelper.DataBase(DBType, Server);
                    #endregion
                    //string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(ReqP["UserCode"].ToString()))
                    {
                        Regex regex = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
                        if (regex.IsMatch(ReqP["Mail"].ToString()))
                        {
                            string Mail = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + ReqP["UserCode"].ToString() + "' and Mail='" + ReqP["Mail"].ToString() + "'");
                            if (Mail != "0")
                            {
                                System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);
                                Random rd = new Random();
                                for (int i = 0; i < 4; i++)
                                {
                                    newRandom.Append(constant[rd.Next(10)]);
                                }
                                //return newRandom.ToString();
                                string sendtext = "【商基】验证码：" + newRandom.ToString() + "，用于平台登录。验证码提供给他人可能导致账号被盗，请勿泄露，谨防被骗。";
                                string sql = "insert into ORDER_EMAIL(usercode,email,date,time,sendtext) VALUES('" + ReqP["UserCode"].ToString() + "','" + ReqP["Mail"].ToString() + "'," +
                                    "'" + DateTime.Now.ToString("yyyy-MM-dd") + "','" + DateTime.Now.ToString("HH:mm:ss") + "','" + sendtext + "')";
                                DMP.ExecuteNonQueryOffline(sql);
                                sql = "update UserInfo set VerificationCode='" + newRandom.ToString() + "' where UserCode='" + ReqP["UserCode"].ToString() + "'";
                                DB.ExecuteNonQueryOffline(sql);
                                ret.IsSuccess = true;
                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "该邮箱并不是该用户注册的邮箱！";
                            }

                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "邮箱格式不正确！";
                        }

                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 忘记密码接口（手机验证）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetPasswordPhoneVer(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "VerificationCode,Phone";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["VerificationCode"].ToString()))
                {

                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                            string count = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + UserCode + "' and VerificationCode='" + ReqP["VerificationCode"].ToString() + "' " +
                                "and Phone='"+ ReqP["Phone"].ToString() + "'");
                        if (count!="0")
                        {
                            ret.IsSuccess = true;
                            ret.ErrMsg = "验证通过！";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "验证码无效！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 忘记密码接口（邮箱验证）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetPasswordMailVer(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "VerificationCode,Mail";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["VerificationCode"].ToString()))
                {

                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(UserCode))
                    {
                        string count = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + UserCode + "' and VerificationCode='" + ReqP["VerificationCode"].ToString() + "' " +
                            "and Mail='" + ReqP["Mail"].ToString() + "'");
                        if (count != "0")
                        {
                            ret.IsSuccess = true;
                            ret.ErrMsg = "验证通过！";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "验证码无效！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 忘记密码接口（修改）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ForgetPassword(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "PassPwdNew,PhoneOrMail,UserCode,VerificationCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["PassPwdNew"].ToString()))
                {

                    //string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    if (!string.IsNullOrEmpty(ReqP["UserCode"].ToString()))
                    {
                        string count = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + ReqP["UserCode"].ToString() + "'");
                        if (count != "0")
                        {
                            string count1 = DB.GetString("select ifnull(count(*),0) from UserInfo where UserCode='" + ReqP["UserCode"].ToString() + "' and VerificationCode='" + ReqP["VerificationCode"].ToString() + "' " +
                           "and (Mail='" + ReqP["PhoneOrMail"].ToString() + "' or Phone='"+ ReqP["PhoneOrMail"].ToString() + "')");
                            if (count1!="0")
                            {
                                string sql = "update UserInfo set UserPwd='" + ReqP["PassPwdNew"].ToString() + "' where UserCode='" + ReqP["UserCode"].ToString() + "'";
                                DB.ExecuteNonQueryOffline(sql);
                                ret.IsSuccess = true;
                                ret.ErrMsg = "修改密码成功！";
                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "验证码无效！";
                            } 
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "查找不到该用户！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查找不到该用户！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 添加企业用户接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddCompanyUser(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "CompanyCode,UserCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["CompanyCode"].ToString()))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    string count = DB.GetString("select ifnull(count(*),0) from CompanyUser where UserCode='" + ReqP["UserCode"].ToString() + "' and CompanyCode='" + ReqP["CompanyCode"].ToString() + "'");
                    if (count == "0")
                    {
                        ReqP.Add("createby", UserCode);
                        ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                        ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                        int result = DB.ExecuteNonQueryOffline(@"insert into CompanyUser(CompanyCode,UserCode,createby,CreateDate,CreateTime) 
        values(@CompanyCode,@UserCode,@createby,@CreateDate,@CreateTime)", ReqP);
                        if (result > 0)
                        {
                            ret.IsSuccess = true;
                            ret.ErrMsg = "保存成功！";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "保存失败！";
                            return ret;
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该用户已存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 移除企业用户接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteCompanyUser(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string keys = "CompanyCode,UserCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ReqP["CompanyCode"].ToString()))
                {
                    string count = DB.GetString("select ifnull(count(*),0) from CompanyUser where UserCode='" + ReqP["UserCode"].ToString() + "' and CompanyCode='" + ReqP["CompanyCode"].ToString() + "'");
                    if (count != "0")
                    {
                        int result = DB.ExecuteNonQueryOffline(@"delete from CompanyUser where UserCode=@UserCode and CompanyCode=@CompanyCode", ReqP);
                        if (result > 0)
                        {
                            ret.IsSuccess = true;
                            ret.ErrMsg = "删除成功！";
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "删除失败！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "查无此数据！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数不能为空！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 查询企业用户接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryCompanyUser(object OBJ)
        {
            GDSJFramework_NETCore.WebAPI.RequestObject ReqObj = (GDSJFramework_NETCore.WebAPI.RequestObject)OBJ;
            GDSJFramework_NETCore.WebAPI.ResultObject ret = new GDSJFramework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            string UserToken = string.Empty;
            string guid = string.Empty;
            GDSJFramework_NETCore.DBHelper.DataBase DB = new GDSJFramework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                UserToken = ReqObj.UserToken.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new GDSJFramework_NETCore.DBHelper.DataBase(string.Empty);

                #region 接口参数
                string CompanyCode = jarr["CompanyCode"].ToString();
                string OrderBy = jarr["OrderBy"].ToString();
                string PageRow = jarr["PageRow"].ToString();
                string Page = jarr["Page"].ToString();
                #endregion
                #region 逻辑
                int total2 = (int.Parse(Page) - 1) * int.Parse(PageRow);
                int total = DB.GetInt32("select count(*) from CompanyUser where CompanyCode='"+ CompanyCode + "'");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from CompanyUser M,(select @n:= 0) d) tab where  RN > " + total2 + " and CompanyCode='" + CompanyCode + "' " + OrderBy + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("data", json);
                p.Add("total", total);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                #endregion

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
