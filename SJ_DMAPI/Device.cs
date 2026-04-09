using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_DMAPI
{
    public class Device
    {
        /// <summary>
        /// 获取设备保养项目列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceMaintainItemList(object OBJ)
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
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select item_id,item_name,item_desc,hours from mac001m", Where, ReqObj.UserToken);


                string sql = @"select * from (
select item_id,item_name,item_desc,hours,@n:= @n + 1 as RN from mac001m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + "  limit " + PageRow + "";
                DataTable mac001m = DB.GetDataTable(sql);
                string count = DB.GetString("select count(1) from mac001m where 1=1 " + Where + "");
                if (mac001m.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(mac001m);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in mac001m.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "保养项目代号,保养项目名称,保养项目描述,耗时";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", count);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取设备保养项目信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceMaintainItem(object OBJ)
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
                string item_id = jarr["item_id"].ToString();//条件
               
                
                string sql = @"
select tem_id,item_name,item_desc,hours from mac001m M where  item_id='"+ item_id+@"'";
                Dictionary<string, object> p = DB.GetDictionary(sql);
            
                if (p.Count > 0)
                {
                    
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取设备维修项目列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceRepairItemList(object OBJ)
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
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select item_id,item_name,item_desc from mac002m", Where, ReqObj.UserToken);

                string sql = @"select * from (
select item_id,item_name,item_desc,@n:= @n + 1 as RN from mac002m M,(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + "  limit " + PageRow + "";
                DataTable mac002m = DB.GetDataTable(sql);
                string count = DB.GetString("select count(1) from mac002m where 1=1 " + Where + "");
                if (mac002m.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(mac002m);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in mac002m.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "维修项目代号,维修项目名称,维修项目描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", count);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取设备维修项目信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceRepairItem(object OBJ)
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
                string item_id = jarr["item_id"].ToString();//条件
                
              
                string sql = @"
select item_id,item_name,item_desc from mac002m M  where item_id='"+ item_id+@"'";
                Dictionary<string, object> p = DB.GetDictionary(sql);
                
                if (p.Count > 0)
                {
                  
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取设备台账列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceList(object OBJ)
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
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select machine_no,machine_name,description,work_center,sites,work_state,price,date_buy,processing_time,cooling_time from mes030m", Where, ReqObj.UserToken);

                string sql = @"select * from (
select machine_no,machine_name,description,work_center,sites,work_state,price,date_buy,processing_time,cooling_time,@n:= @n + 1 as RN from mes030m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + "  limit " + PageRow + "";
                DataTable mes030m = DB.GetDataTable(sql);
                string count = DB.GetString("select count(1) from mes030m where 1=1 " + Where + "");
                if (mes030m.Rows.Count > 0)
                {
                string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(mes030m);
                Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in mes030m.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "设备代号,设备名称,设备描述,所属工作中心代号,所属工站代号,工作状态,采购价格,购买日期,运行总时间,停机总时间";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    p.Add("headdata", headdata);
                    p.Add("data", json);
                p.Add("total", count);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取设备台账信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceInfo(object OBJ)
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


                string machine_no = jarr["machine_no"].ToString();//条件


               
                string sql = @"
select machine_no,machine_name,description,work_center,sites,work_state,price,date_buy,processing_time,cooling_time from mes030m M
 where machine_no='"+machine_no+@"'";

                Dictionary<string, object> p = DB.GetDictionary(sql);

                 if(p.Count>0)
                { 
                 
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 保存设备保养记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveDeviceMaintainLog(object OBJ)
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

                string keys = "machine_no,date_fix,person_fix,item_fix,item_name,hours_real,memo";

                Dictionary<string, object> mes030a4P = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                mes030a4P.Add("createby",SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));//创建人

                mes030a4P.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                mes030a4P.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));


                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes030a4", mes030a4P);
                DB.ExecuteNonQueryOffline(sql,mes030a4P);
                ret.ErrMsg = "添加成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            
            return ret;
        }
        /// <summary>
        /// 保存设备维修记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveDeviceRepairLog(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                ret.IsSuccess = true;

                string keys = "machine_no,date_fix,person_fix,item_fix,item_name,hours_real,trouble,reason,memo";

                Dictionary<string, object> mes030a3P = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);
                mes030a3P.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));//创建人

                mes030a3P.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                mes030a3P.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));


                string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes030a3", mes030a3P);
                DB.ExecuteNonQueryOffline(sql, mes030a3P);
                ret.ErrMsg = "添加成功！";
                ret.IsSuccess = true;
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
