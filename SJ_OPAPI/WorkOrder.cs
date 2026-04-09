using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_OPAPI
{
   public  class WorkOrder
    {
        /// <summary>
        /// 添加工单类型接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddWorkOrderType(object OBJ)
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
                string keys = "TypeCode,TypeName,TypeDesc";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string TypeCode = DB.GetString(string.Format("select TypeCode from Work_Order_Type where TypeCode='{0}'", ReqP["TypeCode"]));
                if (string.IsNullOrEmpty(TypeCode))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    ReqP.Add("createby", UserCode);
                    ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                    int result = DB.ExecuteNonQueryOffline(@"insert into Work_Order_Type(TypeCode,TypeName,TypeDesc,createby,createdate,CreateTime) 
        values(@TypeCode,@TypeName,@TypeDesc,@createby,@CreateDate,@CreateTime)", ReqP);
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
        /// 修改工单类型接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject ModifyWorkOrderType(object OBJ)
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
                string keys = "TypeCode,TypeName,TypeDesc";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string TypeCode = DB.GetString(string.Format("select TypeCode from Work_Order_Type where TypeCode='{0}'", ReqP["TypeCode"]));
                if (!string.IsNullOrEmpty(TypeCode))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    ReqP.Add("createby", UserCode);
                    ReqP.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                    ReqP.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));
                    int result = DB.ExecuteNonQueryOffline(@"update Work_Order_Type set TypeCode=@TypeCode,TypeName=@TypeName,TypeDesc=@TypeDesc,createby=@createby,createdate=@createdate,createtime=@createtime 
                     where TypeCode=@TypeCode", ReqP);
                    if (result > 0)
                    {
                        ret.ErrMsg = "修改成功！";
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "修改失败！";
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
        /// 删除工单类型接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteWorkOrderType(object OBJ)
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
                string keys = "TypeCode";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string TypeCode = DB.GetString(string.Format("select TypeCode from Work_Order_Type where TypeCode='{0}'", ReqP["TypeCode"]));
                if (!string.IsNullOrEmpty(TypeCode))
                {
                    int result = DB.ExecuteNonQueryOffline(@"delete from Work_Order_Type where TypeCode=@TypeCode", ReqP);
                    if (result > 0)
                    {
                        ret.ErrMsg = "删除成功！";
                        ret.IsSuccess = true;
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
        /// 查询工单类型接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryWorkOrderType(object OBJ)
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
                int total = DB.GetInt32("select count(*) from Work_Order_Type where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from Work_Order_Type M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
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
        /// 查询工单管理接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryWorkOrder(object OBJ)
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
                int total = DB.GetInt32("select count(*) from Work_order where 1=1 " + Where + "");
                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from Work_order M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
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
        /// 查询我的工单接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject QueryMyWorkOrder(object OBJ)
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
                DataTable dt = DB.GetDataTable(string.Format("select * from Work_order where createby='{0}'", ReqP["createby"]));
                //Dictionary<string, object> p = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    string json = GDSJFramework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    ret.IsSuccess = true;
                    ret.RetData = json;
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
        /// 添加工单接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddWorkOrder(object OBJ)
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
                string keys = "WorkOrderCode,QuestionType,Priority,CorrespondingProducts,ProblemDesc,Phone,RevisitDate,Mail,CCMail,Enclosure,Status,createby";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                string WorkOrderCode = DB.GetString(string.Format("select WorkOrderCode from Work_order where WorkOrderCode='{0}'", ReqP["WorkOrderCode"]));
                if (string.IsNullOrEmpty(WorkOrderCode))
                {
                    int result = DB.ExecuteNonQueryOffline(@"insert into Work_order(WorkOrderCode,QuestionType,Priority,CorrespondingProducts,ProblemDesc,Phone,RevisitDate,Mail,CCMail,Enclosure,Status,createby) 
        values(@WorkOrderCode,@QuestionType,@Priority,@CorrespondingProducts,@ProblemDesc,@Phone,@RevisitDate,@Mail,@CCMail,@Enclosure,@Status,@createby,@CreateDate,@CreateTime)", ReqP);
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
        /// 添加工单附件接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject AddDocInfo(object OBJ)
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
                string keys = "DocName,FileName,model,TableName,DocSize,DocType,UploadDate,UploadTime,Uploadby,path,code,format,status,pagecount,TableId";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string DocName = DB.GetString(string.Format("select DocName from DocInfo where DocName='{0}'", ReqP["DocName"]));
                if (string.IsNullOrEmpty(DocName))
                {
                    string UserCode = DB.GetString("select UserCode from UserToken where UserToken='" + UserToken + "'");
                    ReqP.Add("createby", UserCode);
                    ReqP.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    ReqP.Add("CreateTime", DateTime.Now.ToString("HH:mm:ss"));
                    int result = DB.ExecuteNonQueryOffline(@"insert into DocInfo(DocName,FileName,model,TableName,DocSize,DocType,UploadDate,UploadTime,Uploadby,path,code,format,
                    status,pagecount,TableId,createby,CreateDate,CreateTime) 
        values(@DocName,@FileName,@model,@TableName,@DocSize,@DocType,@UploadDate,@UploadTime,@Uploadby,@path,@code,@format,@status,@pagecount,@TableId,@createby,@CreateDate,@CreateTime)", ReqP);
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
        /// 删除工单附件接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static GDSJFramework_NETCore.WebAPI.ResultObject DeleteDocInfo(object OBJ)
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
                string keys = "id";
                Dictionary<string, object> ReqP = GDSJFramework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                #endregion
                #region 逻辑
                string DocName = DB.GetString(string.Format("select DocName from DocInfo where id='{0}'", ReqP["id"]));
                if (!string.IsNullOrEmpty(DocName))
                {
                    int result = DB.ExecuteNonQueryOffline(@"delete from DocInfo from id=@id", ReqP);
                    if (result > 0)
                    {
                        ret.ErrMsg = "删除成功！";
                        ret.IsSuccess = true;
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
    }
}
