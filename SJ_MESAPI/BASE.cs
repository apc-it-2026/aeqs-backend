using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_MESAPI
{
    public class BASE
    {
        /// <summary>
        /// 根据工单获取工艺流程信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProceduresByWorkOrde(object OBJ)
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

                
                #region 接口参数
                string production_order = jarr["production_order"].ToString();//工单
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(production_order))
                {
                    string sql = @"
SELECT 
production_order,
sorting,
procedure_no,
procedure_name,
procedure_description,
isend
FROM mes010a1 
WHERE mes010a1.production_order='" + production_order + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    int count = DB.GetInt32(@"
SELECT COUNT(1) 
FROM mes010a1 
WHERE mes010a1.production_order='" + production_order + "'");
                    if (dt.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        string headdata = string.Empty;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            headdata += dc.ColumnName + @",";
                        }


                        headdata.Remove(headdata.Length - 1);

                        string headkey = "工单,生产顺序,工序代号,工序名称,工序描述,是否最后工序";
                        headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                        p.Add("headdata", headdata);
                        p.Add("data", json);
                        p.Add("total", count);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        ret.IsSuccess = true;
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数为空！";
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
        /// 根据品号获取工艺流程信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProceduresByMateriel(object OBJ)
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

                
                #region 接口参数
                string material_no = jarr["material_no"].ToString();//品号
                #endregion
                #region 逻辑
                if (!string.IsNullOrEmpty(material_no))
                {
                    string sql = @"SELECT
* 
FROM base007d1 WHERE material_no='" + material_no + "'";
                    DataTable base007d1 = DB.GetDataTable(sql);
                    if(base007d1.Rows.Count>0)
                    {
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        string base007d1Json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(base007d1);
                        p.Add("base007d1", base007d1Json);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该品号下的工艺资料不存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数为空！";
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
        /// 获取工单资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkOrderInfo(object OBJ)
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

                
                string production_order = jarr["production_order"].ToString();//工单
                if (!string.IsNullOrEmpty(production_order))
                {
                    string sql = @"
select 
mes010m.*,
mes002m.process_name
from mes010m
left join mes002m on mes010m.process = mes002m.process_no
where production_order='" + production_order + "'";
                    DataTable EMS010M = DB.GetDataTable(sql);
                    if (EMS010M.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(EMS010M);
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("MES010M", json);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该工单下的工单资料不存在！";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数为空！";
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
        /// 计算生产耗时接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CalculationProductionHours(object OBJ)
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

                

                string keys = "material_no,procedure_no,qty";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);


                decimal hours = CalculationProductionHours2(ReqP["material_no"].ToString(),
                    ReqP["procedure_no"].ToString(),
                    ReqP["qty"].ToString(), ReqObj.UserToken);

                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("hours", hours.ToString("0.00"));
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
        /// 计算生产耗时接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static decimal CalculationProductionHours2(string material_no,string procedure_no,string qty,string usertoken)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
            ReqObj.UserToken = usertoken;

            decimal ret = 0;

            try
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Dictionary<string, object> ReqP = new Dictionary<string, object>();
                ReqP.Add("material_no", material_no);
                ReqP.Add("procedure_no", procedure_no);
                ReqP.Add("qty", qty);

                decimal takt_time = DB.GetDecimal(@"
SELECT
IFNULL(takt_time,0)
FROM
base007d1
WHERE material_no=@material_no and procedure_no=@procedure_no
", ReqP);
                ret = takt_time * Convert.ToDecimal(ReqP["qty"].ToString());

                ret = ret / 3600;

           
            }
            catch (Exception ex)
            {
               
            }
            return ret;


        }







        /// <summary>
        /// 获取工序信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProcedureInfo(object OBJ)
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

                
                string procedure_no = jarr["procedure_no"].ToString();//工序

                if (!string.IsNullOrEmpty(procedure_no))
                {
                    string sql = @"SELECT 
procedure_no,procedure_name,procedure_description 
FROM MES001M where procedure_no='" + procedure_no + "'";
                    DataTable MES002M = DB.GetDataTable(sql);
                    if (MES002M.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(MES002M);
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("MES001M", json);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该工序下的工序信息不存在！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "参数为空！";
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
        /// 获取工站信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetStation(object OBJ)
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

                
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数

                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select * from base015m", Where, ReqObj.UserToken);



                string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from base015m M
left join base016a1 on base016a1.productionlsite_no = M.productionlsite_no
,(select @n:= 0) d
" + OrderBy + @") tab where  RN >" + total + " " + Where + "  limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                string sql1 = "select * from base015m where 1=1 " + Where + "";
                DataTable dt1 = DB.GetDataTable(sql1);
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "工站代号,工站名称,工站描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", dt1.Rows.Count.ToString());
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
        /// 根据工作中心获取工站资料列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkStationListByWorkCenter(object OBJ)
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

                

                #region 接口参数
                string keys = "productionline_no";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                #endregion
                #region 逻辑

                string sql = @"select productionsite_no,productionsite_name,productionsite_description from (
select M.*,@n:= @n + 1 as RN from base016a1 
join base015m M on M.productionsite_no =base016a1.productionsite_no,(select @n:= 0) d 
where  productionline_no=@productionline_no) tab  ";
                DataTable dt = DB.GetDataTable(sql, ReqP);

                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Data", json);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 根据工序工作中心获取工站资料列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkStationListByDaW(object OBJ)
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

                

                #region 接口参数
                string keys = "procedure_no,productionline_no";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                #endregion
                #region 逻辑
                
                string sql = @"select productionsite_no,productionsite_name,productionsite_description from (
select M.*,@n:= @n + 1 as RN from base016a1 
join base015m M on M.productionsite_no =base016a1.productionsite_no,(select @n:= 0) d 
where procedure_no=@procedure_no and productionline_no=@productionline_no) tab  ";
                DataTable dt = DB.GetDataTable(sql,ReqP);
               
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Data", json);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取工站资料列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkStationList(object OBJ)
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

                

                #region 接口参数
                string keys = "Where,OrderBy,Page,PageRow";

              

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                #endregion
                #region 逻辑
                int total = (int.Parse(ReqP["Page"].ToString()) - 1) * int.Parse(ReqP["PageRow"].ToString());

                ReqP["Where"] = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select productionsite_no,productionsite_name,productionsite_description from base015m", ReqP["Where"].ToString(), ReqObj.UserToken);

                string sql = @"select  productionsite_no,productionsite_name,productionsite_description from (
select M.*,@n:= @n + 1 as RN from base015m M,(select @n:= 0) d where 1=1 "+ ReqP["Where"].ToString()+ @" 
" + ReqP["OrderBy"].ToString() + ") tab where  RN >" + total  + "  limit " + ReqP["PageRow"].ToString() + "";
                DataTable dt = DB.GetDataTable(sql);
                total  = DB.GetInt32("select count(1) from base015m where 1=1 " + ReqP["Where"].ToString() + "");
                
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "工站代号,工站名称,工站描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 根据工序获取对应工作中心列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkCenterByProcedure(object OBJ)
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

                

                #region 接口参数
                string keys = "Procedure,Where,OrderBy,Page,PageRow";


                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                #endregion
                #region 逻辑
                int total = (int.Parse(ReqP["Page"].ToString()) - 1) * int.Parse(ReqP["PageRow"].ToString());

                ReqP["Where"] = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select productionline_no,productionline_name,productionline_description from base016m", ReqP["Where"].ToString(), ReqObj.UserToken);


                string sql = @"select distinct productionline_no,productionline_name,productionline_description from (
select M.*,@n:= @n + 1 as RN from base016m M
join base016a1 on base016a1.productionline_no = M.productionline_no
and base016a1.procedure_no=@Procedure
,(select @n:= 0) d
" + ReqP["OrderBy"].ToString() + ") tab where  RN >" + total + " " + ReqP["Where"].ToString() + "  limit " + ReqP["PageRow"].ToString() + "";
                DataTable dt = DB.GetDataTable(sql,ReqP);
                
                total =DB.GetInt32(@"
select count(distinct base016m.productionline_no) from base016m
join base016a1 on base016a1.productionline_no = base016m.productionline_no
and base016a1.procedure_no=@Procedure
where 1=1 " + ReqP["Where"].ToString() + "",ReqP);
                
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }


                    headdata.Remove(headdata.Length - 1);

                    string headkey = "工作中心代号,工作中心名称,工作中心描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 根据工序获取对应工作中心列表接口(只传工序）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkCenterByProcedure2(object OBJ)
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

                

                #region 接口参数
                string keys = "Procedure";


                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);

                #endregion
                #region 逻辑
                

                string sql = @"select distinct productionline_no,productionline_name,productionline_description from (
select M.*,@n:= @n + 1 as RN from base016m M
join base016a1 on base016a1.productionline_no = M.productionline_no
and base016a1.procedure_no=@Procedure
,(select @n:= 0) d) tab ";
                DataTable dt = DB.GetDataTable(sql, ReqP);

               

                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Data", json);
                    
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取班组资料列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkGroupList(object OBJ)
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

                
                #region 接口参数
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select team_no,team_name,team_description,team_times,team_timee from hr003m", Where, ReqObj.UserToken);


                string sql = @"select team_no,team_name,team_description,team_times,team_timee from (
select M.*,@n:= @n + 1 as RN from hr003m M,(select @n:= 0) d
" + OrderBy + ") tab where  RN >" + total + " " + Where + "  limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                total =DB.GetInt32( "select count(1) from hr003m where 1=1 " + Where + "");
               
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    string headkey = "班组代号,班组名称,班组描述,上班时间,下班时间";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取班组资料列表接口(所有）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkGroupListALL(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
               
                
              
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                
                #region 接口参数
              
                #endregion
                #region 逻辑



                string sql = @"select team_no,team_name,team_description,team_times,team_timee from (
select M.*,@n:= @n + 1 as RN from hr003m M,(select @n:= 0) d) tab";
                DataTable dt = DB.GetDataTable(sql);
                int total = DB.GetInt32("select count(1) from hr003m");

                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "班组代号,班组名称,班组描述,上班时间,下班时间";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
        /// 获取工单列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkOrderList(object OBJ)
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

                
                #region 接口参数
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"select production_order,sales_order,order_date,material_no,material_name,material_specifications,qty,qty_finish,qty_bad,qty_bf,process,productionline_no,qaprocess_no,status from mes010m", Where, ReqObj.UserToken);


                string sql = @"select
production_order,sales_order,order_date,material_no,material_name,material_specifications,qty,qty_finish,qty_bad,qty_bf,process,productionline_no,qaprocess_no,status
from (
select M.*,@n:= @n + 1 as RN from mes010m M,(select @n:= 0) d
" + OrderBy + ") tab where  RN >" + total + " " + Where + "  limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                total =DB.GetInt32("select count(1) from mes010m where 1=1 " + Where + "");
               
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    string headkey = "工单,订单,工单日期,品号,品名,规格,工单数量,完工数量,不良数量,报废数量,工艺流程,工作中心,品检流程,状态";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    //p.Add("team_no", json);
                    //p.Add("team_name", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
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
  
