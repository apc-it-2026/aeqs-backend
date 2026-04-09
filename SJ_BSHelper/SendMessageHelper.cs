
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SJ_BASEHelper
{
    /// <summary>
    /// insert 发送消息类
    /// </summary>
    public class SendMessageHelper
    {
       
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <param name="USERCODE">用户名称</param>
        /// <param name="EMAIL">邮箱</param>
        /// <param name="SENDTEXT">邮件内容</param>
        /// <returns></returns>
        //public static ResultObject SendEMAIL( List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断

        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

        //        if (lstData.Count == 0)
        //            throw new Exception("请传入收件人信息！");

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string EMAIL = dic.ContainsKey("EMAIL") ? dic["EMAIL"].ToString() : "";//邮箱
        //            string TITLE = dic.ContainsKey("TITLE") ? dic["TITLE"].ToString() : "";//邮件标题
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容
        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(EMAIL))
        //                throw new Exception("传入【EMAIL】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(TITLE))
        //                throw new Exception("传入【TITLE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");

        //            Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
        //            if (!r.IsMatch(EMAIL))
        //                throw new Exception("邮箱号【" + EMAIL + "】格式不正确，请检查！");
        //        }

        //        #endregion

        //        #region 逻辑处理
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();//用户名称
        //            string EMAIL = dic["EMAIL"].ToString();//邮箱
        //            string TITLE = dic.ContainsKey("TITLE") ? dic["TITLE"].ToString() : "";//邮件标题
        //            string SENDTEXT = dic["SENDTEXT"].ToString();//内容
        //            sql += @"insert into ORDER_EMAIL(USERCODE,EMAIL,SENDTEXT,DATE,TIME,TITLE)
        //                        values('{0}','{1}','{2}','{3}','{4}','{5}');
        //                      ";
        //            sql = string.Format(sql, USERCODE, EMAIL, SENDTEXT, YMD, TIME, TITLE);
        //        }
        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}
         
        ///// <summary>
        ///// 发送手机信息
        ///// </summary>
        ///// <param name="ReqObj"></param>
        ///// <returns></returns>
        //public static ResultObject SendSMS( List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断

        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string PHONE = dic.ContainsKey("PHONE") ? dic["PHONE"].ToString() : "";//手机号码
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容
        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(PHONE))
        //                throw new Exception("传入【PHONE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");

        //            //Regex rx = new Regex(@"^0{0,1}(13[4-9]|15[7-9]|15[0-2]|18[7-8])[0-9]{8}$");
        //            //if (!rx.IsMatch(PHONE))
        //            //    throw new Exception("手机号【" + PHONE + "】格式不对，请检查！");
        //        }

        //        #endregion

        //        #region 逻辑处理
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();
        //            string PHONE = dic["PHONE"].ToString();
        //            string SENDTEXT = dic["SENDTEXT"].ToString();
        //            sql += @"insert into ORDER_SMS(USERCODE,PHONE,SENDTEXT,DATE,TIME)
        //                        values('{0}','{1}','{2}','{3}','{4}')";
        //            sql = string.Format(sql, USERCODE, PHONE, SENDTEXT, YMD, TIME);
        //        }
        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}
         
        ///// <summary>
        ///// 发送微信消息
        ///// </summary>
        ///// <param name="ReqObj"></param>
        ///// <returns></returns>
        //public static ResultObject SendWEIXIN( List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断
        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string WEIXINCODE = dic.ContainsKey("WEIXINCODE") ? dic["WEIXINCODE"].ToString() : "";//微信 OPENID
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容

        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(WEIXINCODE))
        //                throw new Exception("传入【WEIXINCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");
        //        }
        //        #endregion

        //        #region 逻辑处理  
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();//用户名称
        //            string WEIXINCODE = dic["WEIXINCODE"].ToString();//微信 OPENID
        //            string SENDTEXT = dic["SENDTEXT"].ToString();//内容

        //            sql += @"insert into ORDER_WEIXIN(USERCODE,WEIXINCODE,SENDTEXT,DATE,TIME)
        //                        values('{0}','{1}','{2}','{3}','{4}')";
        //            sql = string.Format(sql, USERCODE, WEIXINCODE, SENDTEXT, YMD, TIME);
        //        }

        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}


        ///// <summary>
        ///// 发送邮件
        ///// </summary>
        ///// <param name="ReqObj"></param>
        ///// <param name="USERCODE">用户名称</param>
        ///// <param name="EMAIL">邮箱</param>
        ///// <param name="SENDTEXT">邮件内容</param>
        ///// <returns></returns>
        //public static ResultObject SendEMAIL(RequestObject ReqObj, List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断

        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //        if (lstData.Count == 0)
        //            throw new Exception("请传入收件人信息！");

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string EMAIL = dic.ContainsKey("EMAIL") ? dic["EMAIL"].ToString() : "";//邮箱
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容
        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(EMAIL))
        //                throw new Exception("传入【EMAIL】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");

        //            Regex r = new Regex("^\\s*([A-Za-z0-9_-]+(\\.\\w+)*@(\\w+\\.)+\\w{2,5})\\s*$");
        //            if (!r.IsMatch(EMAIL))
        //                throw new Exception("邮箱号【" + EMAIL + "】格式不正确，请检查！");
        //        }

        //        #endregion

        //        #region 逻辑处理
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();//用户名称
        //            string EMAIL = dic["EMAIL"].ToString();//邮箱
        //            string SENDTEXT = dic["SENDTEXT"].ToString();//内容
        //            sql += @"insert into ORDER_EMAIL(USERCODE,EMAIL,SENDTEXT,DATE,TIME)
        //                        values('{0}','{1}','{2}','{3}','{4}');
        //                      ";
        //            sql = string.Format(sql, USERCODE, EMAIL, SENDTEXT, YMD, TIME);
        //        }
        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}


        ///// <summary>
        ///// 发送手机信息
        ///// </summary>
        ///// <param name="ReqObj"></param>
        ///// <returns></returns>
        //public static ResultObject SendSMS(RequestObject ReqObj, List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断

        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string PHONE = dic.ContainsKey("PHONE") ? dic["PHONE"].ToString() : "";//手机号码
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容
        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(PHONE))
        //                throw new Exception("传入【PHONE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");

        //            //Regex rx = new Regex(@"^0{0,1}(13[4-9]|15[7-9]|15[0-2]|18[7-8])[0-9]{8}$");
        //            //if (!rx.IsMatch(PHONE))
        //            //    throw new Exception("手机号【" + PHONE + "】格式不对，请检查！");
        //        }

        //        #endregion

        //        #region 逻辑处理
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();
        //            string PHONE = dic["PHONE"].ToString();
        //            string SENDTEXT = dic["SENDTEXT"].ToString();
        //            sql += @"insert into ORDER_SMS(USERCODE,PHONE,SENDTEXT,DATE,TIME)
        //                        values('{0}','{1}','{2}','{3}','{4}')";
        //            sql = string.Format(sql, USERCODE, PHONE, SENDTEXT, YMD, TIME);
        //        }
        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}

        ///// <summary>
        ///// 发送微信消息
        ///// </summary>
        ///// <param name="ReqObj"></param>
        ///// <returns></returns>
        //public static ResultObject SendWEIXIN(RequestObject ReqObj, List<Dictionary<string, string>> lstData)
        //{
        //    ResultObject res = new ResultObject();
        //    try
        //    {
        //        string sql = string.Empty;
        //        string YMD = DateTime.Now.ToString("yyyy-MM-dd");
        //        string TIME = DateTime.Now.ToString("HH:mm:ss");

        //        #region 参数判断
        //        SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic.ContainsKey("USERCODE") ? dic["USERCODE"].ToString() : "";//用户名称
        //            string WEIXINCODE = dic.ContainsKey("WEIXINCODE") ? dic["WEIXINCODE"].ToString() : "";//微信 OPENID
        //            string SENDTEXT = dic.ContainsKey("SENDTEXT") ? dic["SENDTEXT"].ToString() : "";//内容

        //            if (string.IsNullOrEmpty(USERCODE))
        //                throw new Exception("传入【USERCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(WEIXINCODE))
        //                throw new Exception("传入【WEIXINCODE】参数不能为空，请检查！");
        //            if (string.IsNullOrEmpty(SENDTEXT))
        //                throw new Exception("传入【SENDTEXT】参数不能为空，请检查！");
        //        }
        //        #endregion

        //        #region 逻辑处理  
        //        foreach (Dictionary<string, string> dic in lstData)
        //        {
        //            string USERCODE = dic["USERCODE"].ToString();//用户名称
        //            string WEIXINCODE = dic["WEIXINCODE"].ToString();//微信 OPENID
        //            string SENDTEXT = dic["SENDTEXT"].ToString();//内容

        //            sql += @"insert into ORDER_WEIXIN(USERCODE,WEIXINCODE,SENDTEXT,DATE,TIME)
        //                        values('{0}','{1}','{2}','{3}','{4}')";
        //            sql = string.Format(sql, USERCODE, WEIXINCODE, SENDTEXT, YMD, TIME);
        //        }

        //        DB2.ExecuteNonQueryOffline(sql);
        //        #endregion

        //        res.IsSuccess = true;
        //        res.RetData = "成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        res.IsSuccess = false;
        //        res.ErrMsg = ex.Message;
        //    }
        //    return res;
        //}

    }
}
