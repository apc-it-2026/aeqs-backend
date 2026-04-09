using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_KBAPI
{
    /// <summary>
    /// 系统看板设备接口
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Check检查授权
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Check(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                


                #region 获取参数
                string key = "DeviceGuid";
                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                string Company =DB.GetString(@"
SELECT
CompanyCode
FROM
kanbandevice
WHERE DeviceGuid=@DeviceGuid
", ReqP);
                if(string.IsNullOrEmpty(Company))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = ReqP["DeviceGuid"].ToString() + " 设备没有授权，请联系厂商处理";
                    return ret;
                }
                else
                {
                    ReqP.Add("CompanyCode", Company);
                    ReqP.Add("UserToken", Guid.NewGuid().ToString());

                    if(DB.ExecuteNonQueryOffline(@"
delete from usertoken where UserCode=@UserCode and CompanyCode=@CompanyCode;
Insert into usertoken (CompanyCode,UserCode,UserToken) VALUES (@CompanyCode,@UserCode,@UserToken)
", ReqP)>0)
                    {
                        RetData.Add("UserToken", ReqP["UserToken"].ToString());
                        RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                        ret.IsSuccess = true;
                        ret.RetData = RetDataJson;

                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "更新UserToken出错";
                        return ret;
                    }

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
        /// GetMenu获取设备的看板菜单
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMenu(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                


                #region 获取参数
            
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                DataTable dt = DB.GetDataTable(@"
SELECT * FROM kanbanmenu
");
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "没有菜单数据";
                    return ret;
                }
                else
                {
                    RetData.Add("data", SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt));
                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.RetData = RetDataJson;


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
        /// GetKanBanByDevice根据设备获取指派的看板信息
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetKanBanByDevice(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                


                #region 获取参数
                string key = "DeviceGuid";
                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                DataTable dt = DB.GetDataTable(@"
SELECT
*
FROM
kanbantodevice
WHERE DeviceGuid=@DeviceGuid
",ReqP);
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "没有指派看板";
                    return ret;
                }
                else
                {
                    RetData.Add("data", SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt));
                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
                    ret.IsSuccess = true;
                    ret.RetData = RetDataJson;


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
