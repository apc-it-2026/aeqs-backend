using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{
    class BillManage
    {


        /// <summary>
        /// 添加账单管理接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddBillManage(object OBJ)
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
                string keys = "Title,OrderCode,TransCode,OtherParty,Money,Status,createby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                int result = DB.ExecuteNonQueryOffline(@"insert into Bill_Manage(Title,OrderCode,TransCode,OtherParty,Money,Status,createby,createdate,createtime) 
        values(@Title,@OrderCode,@TransCode,@OtherParty,@Money,@Status,@createby,@CreateDate,@CreateTime)", ReqP);
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
        /// 修改账单管理接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyBillManage(object OBJ)
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
                string keys = "Title,OrderCode,TransCode,OtherParty,Money,Status,modifyby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;
                #endregion

                string OrderCode = DB.GetString(string.Format("select OrderCode from Bill_Manage where OrderCode='{0}' ", ReqP["OrderCode"]));
                //判断是否存在
                if (string.IsNullOrWhiteSpace(OrderCode))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无此数据！";
                    return ret;
                }

                int result = DB.ExecuteNonQueryOffline("UPDATE Bill_Manage set Title=@Title,TransCode=@TransCode,OtherParty=@OtherParty,Money=@Money,Status=@Status,modifyby=@modifyby where  OrderCode=@OrderCode", ReqP);
                if (result > 0)
                {

                    RetDataJson = Newtonsoft.Json.JsonConvert.SerializeObject(RetData);
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
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }


            return ret;
        }



        /// <summary>
        /// 查询账单管理接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryBillManage(object OBJ)
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

                string sql = string.Format(@"select Title,OrderCode,TransCode,OtherParty,Money,Status from(
                select id, Title,OrderCode,TransCode,OtherParty,Money,Status, @n:= @n + 1 as RN from Bill_Manage M,
                (select @n:= 0) d  {0}) tab 
                where  RN >{1}  {2}   limit {3} ", OrderBy, total, Where, PageRow);


                DataTable dtResult = DB.GetDataTable(sql);
                if (dtResult.Rows.Count > 0)
                {
                    int total2 = DB.GetInt32("select count(*) from Bill_Manage where 1=1 " + Where + "");
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dtResult);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("data", json);
                    p.Add("total", total2);

                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = true;
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
        /// 删除账单管理接口
        /// </summary>
        /// <param name="OBJ">数据</param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteBillManage(object OBJ)
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
                string keys = "OrderCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                Dictionary<string, object> RetData = new Dictionary<string, object>();
                string RetDataJson = string.Empty;



                #endregion




                int result = DB.ExecuteNonQueryOffline("delete from Bill_Manage where OrderCode=@OrderCode", ReqP);
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
