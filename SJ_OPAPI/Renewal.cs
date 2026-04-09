using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{
   public  class Renewal
    {
        /// <summary>
        /// 查询续费管理接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryRenewalManage(object OBJ)
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
                int total = DB.GetInt32("select count(*) from Renewal_Manage where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from Renewal_Manage M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
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
        /// 添加续费接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddRenewalManage(object OBJ)
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
                string keys = "ProductCode,ProductNum,qty,ProductDesc,Payment,DueDate,RenewalTime,RenewDate,Money,createby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string CODE = DB.GetString(string.Format("select ProductCode from Renewal_Manage where ProductCode='{0}'", ReqP["ProductCode"]));
                if (string.IsNullOrEmpty(CODE))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                    int result = DB.ExecuteNonQueryOffline(@"insert into Renewal_Manage(ProductCode,ProductNum,qty,ProductDesc,Payment,DueDate,RenewalTime,RenewDate,Money,createby,
                    createdate,CreateTime) 
                   values(@ProductCode,@ProductNum,@qty,@ProductDesc,@Payment,@DueDate,@RenewalTime,@RenewDate,@Money,@createby,@CreateDate,@CreateTime)", ReqP);
                    if (result > 0)
                    {
                        ret.ErrMsg = "保存成功！";
                        ret.IsSuccess = true;
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
                    ret.ErrMsg = "已存在该数据！";
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
        /// 删除续费接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyRenewalManage(object OBJ)
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
                string keys = "ProductCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string CODE = DB.GetString(string.Format("select ProductCode from Renewal_Manage where ProductCode='{0}'", ReqP["ProductCode"]));
                if (string.IsNullOrEmpty(CODE))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                    int result = DB.ExecuteNonQueryOffline(@"insert into Renewal_Manage(ProductCode,ProductNum,qty,ProductDesc,Payment,DueDate,RenewalTime,RenewDate,Money,createby,
                    createdate,CreateTime) 
                   values(@ProductCode,@ProductNum,@qty,@ProductDesc,@Payment,@DueDate,@RenewalTime,@RenewDate,@Money,@createby,@CreateDate,@CreateTime)", ReqP);
                    if (result > 0)
                    {
                        ret.ErrMsg = "保存成功！";
                        ret.IsSuccess = true;
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
                    ret.ErrMsg = "已存在该数据！";
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
    }
}
