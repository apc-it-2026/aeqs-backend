using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{
   public  class Order
    {
        /// <summary>
        /// 添加订单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddOrderInfo(object OBJ)
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
                string OrderCode = jarr["OrderCode"].ToString();
                string OrderDate = jarr["OrderDate"].ToString();
                string OrderType = jarr["OrderType"].ToString();
                string OrderSource = jarr["OrderSource"].ToString();
                string PaymentStatus = jarr["PaymentStatus"].ToString();
                string PaymentDate = jarr["PaymentDate"].ToString();
                string PaymentTime = jarr["PaymentTime"].ToString();
                string createby = jarr["createby"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(OrderCode))
                {
                    string Order = DB.GetString("select OrderCode from OrderInfo where OrderCode='" + OrderCode + "'");
                    if (string.IsNullOrEmpty(Order))
                    {
                        string sql = "insert into OrderInfo(OrderCode,OrderDate,OrderType,OrderSource,PaymentStatus,PaymentDate,PaymentTime,createby,createdate,createtime) " +
                            "values('" + OrderCode + "','" + OrderDate + "','" + OrderType + @"',
                     '" + OrderSource + "','" + PaymentStatus + "','" + PaymentDate + "','" + PaymentTime + "','"+ createby + "','"+ DateTime.Now.ToString("yyyy-MM-dd") + "','"+DateTime.Now.ToString("HH:mm:ss")+"')";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "保存成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "已存在该订单！";
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
        /// 修改订单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyOrderInfo(object OBJ)
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
                string OrderCode = jarr["OrderCode"].ToString();
                string OrderDate = jarr["OrderDate"].ToString();
                string OrderType = jarr["OrderType"].ToString();
                string OrderSource = jarr["OrderSource"].ToString();
                string PaymentStatus = jarr["PaymentStatus"].ToString();
                string PaymentDate = jarr["PaymentDate"].ToString();
                string PaymentTime = jarr["PaymentTime"].ToString();
                string modifyby = jarr["modifyby"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(OrderCode))
                {
                    string Order = DB.GetString("select OrderCode from OrderInfo where OrderCode='" + OrderCode + "'");
                    if (!string.IsNullOrEmpty(Order))
                    {
                        string sql = "update OrderInfo set OrderCode='"+ OrderCode + "',OrderDate='"+ OrderDate + "',OrderType='"+ OrderType + "',OrderSource='"+ OrderSource + "',PaymentStatus='"+ PaymentStatus + "'," +
                            "PaymentDate='"+ PaymentDate + "',PaymentTime='"+ PaymentTime + "',modifyby='"+ modifyby + "',modifydate='"+ DateTime.Now.ToString("yyyy-MM-dd") + "'," +
                            "modifytime='"+DateTime.Now.ToString("HH:mm:ss")+ "' where OrderCode='" + OrderCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "修改成功！";
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
        /// 删除订单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteOrderInfo(object OBJ)
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
                string OrderCode = jarr["OrderCode"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(OrderCode))
                {
                    string Order = DB.GetString("select OrderCode from OrderInfo where OrderCode='" + OrderCode + "'");
                    if (!string.IsNullOrEmpty(Order))
                    {
                        string sql = "delete from OrderInfo where OrderCode='" + OrderCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        //ret.IsSuccess = true;
                        //ret.RetData = "修改成功！";
                    }
                    string code = DB.GetString("select ifnull(count(*),0) from Order_Details where OrderCode='" + OrderCode + "'");
                    if (code!="0")
                    {
                        string sql = "delete from Order_Details where OrderCode='" + OrderCode + "'";
                        DB.ExecuteNonQueryOffline(sql);
                    }
                    ret.IsSuccess = true;
                    ret.ErrMsg = "删除成功！";
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
        /// 查询订单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryOrderInfo(object OBJ)
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
                int total = DB.GetInt32("select count(*) from OrderInfo where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from OrderInfo M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count>0)
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
                    ret.IsSuccess = true;
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
        /// 添加订单明细接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddOrderDetails(object OBJ)
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
                string ProductCode = jarr["ProductCode"].ToString();
                string ProductName = jarr["ProductName"].ToString();
                string qty = jarr["qty"].ToString();
                string ProductDesc = jarr["ProductDesc"].ToString();
                string Payment = jarr["Payment"].ToString();
                string ExpiryDate = jarr["ExpiryDate"].ToString();
                string Money = jarr["Money"].ToString();
                string OrderCode = jarr["OrderCode"].ToString();
                string createby = jarr["createby"].ToString();
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(ProductCode))
                {
                    string sql = "select * from Order_Details where ProductCode='"+ ProductCode + "' and ProductName='"+ ProductName + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count==0)
                    {
                        sql = "insert into Order_Details(ProductCode,ProductName,qty,ProductDesc,Payment,ExpiryDate,Money,OrderCode,createby,createdate,createtime) values('"+ ProductCode + "'" +
                            ",'"+ ProductName + "','"+ qty + "','"+ ProductDesc + "','"+ Payment + "','"+ ExpiryDate + "','"+ Money + "','"+ OrderCode + "','"+ createby + "'," +
                            "'"+DateTime.Now.ToString("yyyy-MM-dd")+"','"+DateTime.Now.ToString("HH:mm:ss")+"')";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "保存成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该数据已存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数为空！";
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
        /// 修改订单明细接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyOrderDetails(object OBJ)
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
                string keys = "ProductCode,ProductName,qty,ProductDesc,Payment,ExpiryDate,Money,OrderCode,modifyby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string CODE = DB.GetString(string.Format("select ProductCode from Order_Details where ProductCode='{0}' and ProductName='{1}'", ReqP["ProductCode"], ReqP["ProductName"]));
               
                if (!string.IsNullOrEmpty(CODE))
                {
                  
                    int result = DB.ExecuteNonQueryOffline("UPDATE Order_Details set ProductCode=@ProductCode, ProductName=@ProductName, qty=@qty" +
                        ",ProductDesc=@ProductDesc,Payment=@Payment,ExpiryDate=@ExpiryDate,Money=@Money,OrderCode=@OrderCode,modifyby=@modifyby where  ProductCode=@ProductCode", ReqP);
                    if (result > 0)
                    {
                        ret.IsSuccess = true;
                        ret.ErrMsg = "修改成功！";
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "修改失败！";
                        return ret;
                    }
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
        /// 删除订单明细接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteOrderDetails(object OBJ)
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
                string keys = "ProductCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string CODE = DB.GetString(string.Format("select * from Order_Details where ProductCode='{0}'", ReqP["ProductCode"]));

                if (!string.IsNullOrEmpty(CODE))
                {

                    int result = DB.ExecuteNonQueryOffline("delete from Order_Details where  ProductCode=@ProductCode", ReqP);
                    if (result > 0)
                    {
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
        /// 查询订单明细接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryOrderDetails(object OBJ)
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
                int total = DB.GetInt32("select count(*) from Order_Details where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from Order_Details M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
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
                    ret.IsSuccess = true;
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
        /// 查询我的订单接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryMyOrderDetails(object OBJ)
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
                string keys = "createby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                DataTable dt = DB.GetDataTable(string.Format("select * from OrderInfo where createby='{0}'", ReqP["createby"]));
                DataTable dt1 = DB.GetDataTable(string.Format("select * from Order_Details where createby='{0}'", ReqP["createby"]));
                Dictionary<string, object> p = new Dictionary<string, object>();
                int n = 0;
                int a = 0;
                if (dt.Rows.Count>0)
                {
                    string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    p.Add("OrderInfo", json);
                }
                else
                {
                    n++;
                }
                if (dt1.Rows.Count > 0)
                {
                    string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt1);
                    p.Add("Order_Details", json);
                }
                if (n>0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                }
                else
                {
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
