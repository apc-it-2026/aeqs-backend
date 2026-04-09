using Oracle.ManagedDataAccess.Client;
using SJ_BASEHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace SJ_SYSAPI
{
    public class SendMESSAGETest
    {

        ///// <summary>
        ///// 测试发送邮件
        ///// </summary>
        ///// <param name="OBJ"></param>
        ///// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject SendEmailTest(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject(); 

        //    try
        //    {
        //        #region 调用 
        //        List<Dictionary<string, string>> lstData = new List<Dictionary<string, string>>();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        dic["USERCODE"] = "何德镟";//用户名称
        //        dic["EMAIL"] = "1040601252@qq.com";//收件人邮箱
        //        dic["TITLE"] = "这是邮件标题！"+"["+DateTime.Now.ToString("yyyy-MM-dd") + "]";//标题
        //        dic["SENDTEXT"] = "这是一封测试邮件！["+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";//消息内容
        //        lstData.Add(dic);//可添加多人信息
        //        SJeMES_Framework_NETCore.WebAPI.ResultObject res = SendMessageHelper.SendEMAIL(lstData);
        //        if (!res.IsSuccess)
        //            throw new Exception(res.ErrMsg);
        //        #endregion
                
        //        ret.IsSuccess = true;
        //        ret.RetData = "操作成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = "00000:" + ex.Message;
        //        string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
        //    }

        //    return ret;
        //}

        ///// <summary>
        ///// 测试发送短信
        ///// </summary>
        ///// <param name="OBJ"></param>
        ///// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject SendSMSTest(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

        //    try
        //    {
        //        #region 调用 
        //        List<Dictionary<string, string>> lstData = new List<Dictionary<string, string>>();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        dic["USERCODE"] = "何德镟";//用户名称
        //        dic["PHONE"] = "17875726315";//手机号码
        //        dic["SENDTEXT"] = "【商基】这是一条测试短信！";//消息内容
        //        lstData.Add(dic);//可添加多人信息
        //        SJeMES_Framework_NETCore.WebAPI.ResultObject res = SendMessageHelper.SendSMS(lstData);
        //        if (!res.IsSuccess)
        //            throw new Exception(res.ErrMsg);
        //        #endregion

        //        ret.IsSuccess = true;
        //        ret.RetData = "操作成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = "00000:" + ex.Message;
        //        string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
        //    }

        //    return ret;
        //}

        /// <summary>
        /// 测试推送微信消息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject SendWEIXINTest(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

        //    try
        //    {
        //        #region 调用 
        //        List<Dictionary<string, string>> lstData = new List<Dictionary<string, string>>();
        //        Dictionary<string, string> dic = new Dictionary<string, string>();
        //        dic["USERCODE"] = "何德镟";//用户名称
        //        dic["WEIXINCODE"] = "oZGI2uKq6CbbDxl8UUSeDd8NrA4c";//微信 OPENID
        //        dic["SENDTEXT"] = "【商基】这是一条微信推送信息！";//消息内容
        //        lstData.Add(dic);//可添加多人信息
        //        SJeMES_Framework_NETCore.WebAPI.ResultObject res = SendMessageHelper.SendWEIXIN(lstData);
        //        if (!res.IsSuccess)
        //            throw new Exception(res.ErrMsg);
        //        #endregion

        //        ret.IsSuccess = true;
        //        ret.RetData = "操作成功！";
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = "00000:" + ex.Message;
        //        string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
        //    }

        //    return ret;
        //}



        /// <summary>
        /// SQLConnectCountTest
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SQLConnectCountTest(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
             

            for (int i = 0; i < 1000; i++)
            { 
                using (SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj))
                {
                    DB.Open();
                    DB.ExecuteNonQuery(@"insert into xxx");
                }
            } 

            return ret;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveImg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;
            try
            {

                string cnnstr = "data source=FY;User Id=FY;Password=123;"; 
                SJeMES_Framework_NETCore.DBHelper.GDSJOracle DB = new SJeMES_Framework_NETCore.DBHelper.GDSJOracle(cnnstr);

                int i = 0;
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                object PHOTO = jarr["PHOTO"].ToString();
                string PNAME = jarr["PNAME"].ToString();
                string PURL = jarr["PURL"].ToString();
                DB.Open();
                byte[] buff;
                using (MemoryStream ms = new MemoryStream())
                {
                    IFormatter iFormatter = new BinaryFormatter();
                    iFormatter.Serialize(ms, PHOTO);
                    buff = ms.GetBuffer();
                }
                string sql = "select * from WB_BLOBDEMO";
                DataTable dt = DB.GetDataTable(sql);


                string sqlStr = "insert into WB_BLOBDEMO(PHOTO,PNAME,PURL)values(@PHOTO,@PNAME,@PURL)";
                OracleParameter[] parameterValue = {
                        new OracleParameter("@PHOTO",OracleDbType.Blob),
                        new OracleParameter("@PNAME",OracleDbType.Varchar2),
                        new OracleParameter("@PURL",OracleDbType.Varchar2),
                        };
                parameterValue[0].Value = buff;
                parameterValue[1].Value = PNAME;
                parameterValue[2].Value = PURL;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("@PHOTO", buff);
                dic.Add("@PNAME", PNAME);
                dic.Add("@PURL", PURL);
                
                i = DB.ExecuteNonQueryOffline(sqlStr, dic);

                // sql = "";
                //sql = string.Format(sql, buff, PNAME, PURL);
                //int i = DB.ExecuteNonQueryOffline(sql);
                if (i <= 0)
                    throw new Exception("保存图片失败！");

                DB.Close();
                ret.IsSuccess = true;
                ret.RetData = "操作成功！";
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
