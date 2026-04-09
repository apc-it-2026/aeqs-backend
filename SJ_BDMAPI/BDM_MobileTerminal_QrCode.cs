using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_MobileTerminal_QrCode
    {
        /// <summary>
        /// 查询-移动端下载
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMobileTerminal_QrCode(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBTest = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string ZhangSet = DB.DataBaseName;//账套

                //前缀
                string uploadurl = DBTest.GetString($@"select uploadurl from SYSORG01M where org='{ZhangSet}'");

                //地址
                string fd_url = DBTest.GetString($@"select full_download_url from APP_VERSION");

                uploadurl = uploadurl.Replace("api/commoncall","");

                //合并地址
                string mt_url = uploadurl + fd_url;

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("mt_url", mt_url);
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
    }
}
