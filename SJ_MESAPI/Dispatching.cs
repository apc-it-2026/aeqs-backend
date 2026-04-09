using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SJ_MESAPI
{
    public class Dispatching
    {

        ///生产派工

        /// <summary>
        /// 获取已派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkcenterDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string WHERE = jarr["WHERE"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                string PageRow = jarr["PageRow"].ToString().Trim();
                string Page = jarr["Page"].ToString().Trim();

                Dictionary<string, object> p = new Dictionary<string, object>();

             
                #region 数据获取SQL
                string datasql = @"
SELECT
                mes012a1.id,
                mes010m.sales_order,
                mes010m.production_order,
                mes010m.material_no,
                mes010m.material_name,
                mes010m.material_specifications,
                mes012a1.sorting,
                mes012a1.procedure_no,
                mes012a1.procedure_name,
                mes012a1.productionline_no,
                base016m.productionline_name,
                mes012m.sorting_plan,
                mes012m.qty_plan,
                mes012m.date_plan,
                mes012a1.productionsite_no,
                base015m.productionsite_name,
                mes012a1.date_start,
                mes012a1.qty
                FROM
                mes012a1
                join mes010m on mes012a1.production_order=mes010m.production_order
                join mes012m on mes012a1.production_order=mes012m.production_order and mes012a1.sorting=mes012m.sorting and mes012a1.procedure_no=mes012m.procedure_no and mes012a1.productionline_no=mes012m.productionline_no and mes012m.date_plan=mes012a1.date_plan
                join base016m on base016m.productionline_no=mes012a1.productionline_no
                join base015m on base015m.productionsite_no = mes012a1.productionsite_no
                where mes012a1.productionline_no<>'' and mes012a1.productionline_no is not null
";
                #endregion

                DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                WHERE = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, WHERE);

                datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE 1=1 " + WHERE;


                DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                    PageRow, Page, OrderBy));
                if (dt.Rows.Count > 0)
                {
                    string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    int total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");

                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "id,订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,工作中心代号,工作中心名称,排产顺序,排产数量,排产日期,工站代号,工站名称,派工日期,派工数量";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", dtJson);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 获取待派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkcenterNoDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string WHERE = jarr["WHERE"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                string PageRow = jarr["PageRow"].ToString().Trim();
                string Page = jarr["Page"].ToString().Trim();
                Dictionary<string, object> p = new Dictionary<string, object>();

              

                #region 数据获取SQL
                string datasql = @"
SELECT  
*,
TMP.qty_plan-TMP.qty2 as 'qty3'
FROM(
SELECT
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes012m.sorting,
mes012m.procedure_no,
mes012m.procedure_name,
mes012m.productionline_no,
base016m.productionline_name,
mes012m.sorting_plan,
mes012m.qty_plan,
ifnull(SUM(mes012a1.qty),0) as qty2,
mes012m.date_plan
FROM
mes012m
join mes010m on mes012m.production_order=mes010m.production_order
left join mes012a1 on mes012a1.production_order=mes012m.production_order and mes012a1.sorting=mes012m.sorting and mes012a1.procedure_no=mes012m.procedure_no and mes012a1.productionline_no=mes012m.productionline_no and mes012m.date_plan=mes012a1.date_plan
join base016m on base016m.productionline_no=mes012m.productionline_no
where mes012m.productionline_no<>'' and mes012m.productionline_no is not null
GROUP BY
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes012m.sorting,
mes012m.procedure_no,
mes012m.procedure_name,
mes012m.productionline_no,
base016m.productionline_name,
mes012m.sorting_plan,
mes012m.qty_plan,
mes012m.date_plan) TMP
";
                    #endregion

                    DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                    WHERE = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, WHERE);

                    datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE qty3>0 " + WHERE;


                    DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                        PageRow, Page, OrderBy));
                    if (dt.Rows.Count > 0)
                    {
                        string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                        int total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");
                        string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,工作中心代号,工作中心名称,排产顺序,排产数量,已派工数量,排产日期,待派工数量";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    p.Add("headdata", headdata);
                    p.Add("data", dtJson);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 删除已派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DelWorkcenterDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string id = jarr["id"].ToString().Trim();
                
                Dictionary<string, string> p = new Dictionary<string, string>();
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(id))
                {
                    sql = "select * from mes012a1 where id='"+id+"'";
                    DataTable dt = DB.GetDataTable(sql);
                    DateTime t1 = DateTime.Now;
                    DateTime t2 = Convert.ToDateTime(dt.Rows[0]["date_start"].ToString().Trim());
                    if (DateTime.Compare(t1, t2) > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该派工任务已到生产日期不能删除";
                    }
                    else
                    {
                        sql = "delete from mes012a1 where id='" + id + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "删除成功";
                    }
                }
                else {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "id不能为空！";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 保存派工信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject SaveWorkcenterDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                var o = JObject.Parse(Data);
                string table = o["table"].ToString().Trim();
              
                DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(table);
                if (dt.Rows.Count>0)
                {
                    int n = 0;
                    int a = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string sqlqty3 = @"
select
IFNULL(qty_plan - qty2, 0) as qty3
from
(
select
ifnull(SUM(mes012a1.qty), 0) as qty2,
mes012m.qty_plan
from mes012m
left join mes012a1 on mes012m.production_order = mes012a1.production_order
and mes012a1.sorting = mes012m.sorting and mes012a1.procedure_no = mes012m.procedure_no
and mes012a1.productionline_no = mes012m.productionline_no and mes012m.date_plan = mes012a1.date_plan
WHERE mes012m.production_order = '" + dt.Rows[i]["production_order"].ToString().Trim() + @"'
and mes012m.sorting = '" + dt.Rows[i]["sorting"].ToString().Trim() + @"'
and mes012m.procedure_no = '" + dt.Rows[i]["procedure_no"].ToString().Trim() + @"'
AND mes012m.productionline_no = '" + dt.Rows[i]["productionline_no"].ToString().Trim() + @"'
and mes012m.date_plan = '" + dt.Rows[i]["date_plan"].ToString().Trim() + @"'
GROUP BY
mes012a1.sorting,
mes012a1.procedure_no,
mes012a1.procedure_name,
mes012a1.productionline_no,
mes012m.sorting_plan,
mes012m.qty_plan,
mes012m.date_plan) tab";
                        string qty3 = DB.GetString(sqlqty3);
                        if (Convert.ToDecimal(dt.Rows[i]["qty"].ToString().Trim())>Convert.ToDecimal(qty3))
                        {
                            n ++;
                            ret.IsSuccess = false;
                            ret.ErrMsg = dt.Rows[i]["production_order"].ToString().Trim()+"派工数量大于待派工数量！";
                            return ret;
                        }
                        if (n==0)
                        {
                            int seq =Convert.ToInt32(DB.GetString("select count(*) from mes012a1 where production_order='"+ dt.Rows[i]["production_order"].ToString().Trim() + "' and sorting='"+ dt.Rows[i]["sorting"].ToString().Trim() + "' and procedure_no='"+ dt.Rows[i]["procedure_no"].ToString().Trim() + "'"));
                            int seqq = seq + 1;
                            string sql = @"insert into mes012a1(production_order,sorting,material_no,material_name,material_specifications,
                                procedure_no,procedure_name,productionline_no,date_plan,
productionsite_no,date_start,qty,seq) values('" + dt.Rows[i]["production_order"].ToString().Trim() + "','"+ dt.Rows[i]["sorting"].ToString().Trim() + @"',
'" + dt.Rows[i]["material_no"].ToString().Trim() + "','" + dt.Rows[i]["material_name"].ToString().Trim() + "','" + dt.Rows[i]["material_specifications"].ToString().Trim() + @"',
'"+ dt.Rows[i]["procedure_no"].ToString().Trim() + "','" + dt.Rows[i]["procedure_name"].ToString().Trim() + "','" + dt.Rows[i]["productionline_no"].ToString().Trim() + @"'
,'" + dt.Rows[i]["date_plan"].ToString().Trim() + @"','" + dt.Rows[i]["productionsite_no"].ToString().Trim() + @"',
'" + dt.Rows[i]["date_start"].ToString().Trim() + @"','"+ dt.Rows[i]["qty"].ToString().Trim() + "','"+ seqq + "')";
                            DB.ExecuteNonQueryOffline(sql);
                            a++;
                        }
                    }
                    if (a>0)
                    {
                        ret.IsSuccess = true;
                        ret.ErrMsg = "派工成功！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数为空！";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        ///委外派工

        /// <summary>
        /// 获取已派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupplierDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string WHERE = jarr["WHERE"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                string PageRow = jarr["PageRow"].ToString().Trim();
                string Page = jarr["Page"].ToString().Trim();

                Dictionary<string, object> p = new Dictionary<string, object>();

       


                #region 数据获取SQL
                string datasql = @"
SELECT
                mes012a1.id,
                mes010m.sales_order,
                mes010m.production_order,
                mes010m.material_no,
                mes010m.material_name,
                mes010m.material_specifications,
                mes012a1.sorting,
                mes012a1.procedure_no,
                mes012a1.procedure_name,
                mes012m.sorting_plan,
                mes012m.qty_plan,
                mes012m.date_plan,
                mes012a1.productionsite_no,
                base003m.suppliers_name as productionsite_name,
                mes012a1.date_start,
                mes012a1.qty
                FROM
                mes012a1
                join mes010m on mes012a1.production_order=mes010m.production_order
                join mes012m on mes012a1.production_order=mes012m.production_order and mes012a1.sorting=mes012m.sorting and mes012a1.procedure_no=mes012m.procedure_no and mes012a1.productionline_no=mes012m.productionline_no and mes012m.date_plan=mes012a1.date_plan
                join base003m on base003m.suppliers_code = mes012a1.productionsite_no
                where mes012a1.productionline_no='' or mes012a1.productionline_no is null
";
                #endregion

                DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                WHERE = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, WHERE);

                datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE 1=1 " + WHERE;


                DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                    PageRow, Page, OrderBy));
                if (dt.Rows.Count > 0)
                {
                    string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    int total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "id,订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,排产顺序,排产数量,排产日期,供应商代号,供应商名称,派工日期,派工数量";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);
                    p.Add("headdata", headdata);
                    p.Add("data", dtJson);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 获取待派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupplierNoDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string WHERE = jarr["WHERE"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                string PageRow = jarr["PageRow"].ToString().Trim();
                string Page = jarr["Page"].ToString().Trim();

                Dictionary<string, object> p = new Dictionary<string, object>();

            

                #region 数据获取SQL
                string datasql = @"
SELECT  
                *,
                TMP.qty_plan-TMP.qty2 as 'qty3'
                FROM(
                SELECT
                mes010m.sales_order,
                mes010m.production_order,
                mes010m.material_no,
                mes010m.material_name,
                mes010m.material_specifications,
                mes012m.sorting,
                mes012m.procedure_no,
                mes012m.procedure_name,
                mes012m.sorting_plan,
                mes012m.qty_plan,
                ifnull(SUM(mes012a1.qty),0) as qty2,
                mes012m.date_plan
                FROM
                mes012m
                join mes010m on mes012m.production_order=mes010m.production_order
                left join mes012a1 on mes012a1.production_order=mes012m.production_order and mes012a1.sorting=mes012m.sorting and mes012a1.procedure_no=mes012m.procedure_no and mes012a1.productionline_no=mes012m.productionline_no and mes012m.date_plan=mes012a1.date_plan
                where mes012m.productionline_no='' or mes012m.productionline_no is  null
                GROUP BY
                mes010m.sales_order,
                mes010m.production_order,
                mes010m.material_no,
                mes010m.material_name,
                mes010m.material_specifications,
                mes012m.sorting,
                mes012m.procedure_no,
                mes012m.procedure_name,
                mes012m.productionline_no,
                mes012m.sorting_plan,
                mes012m.qty_plan,
                mes012m.date_plan) TMP
";
                #endregion

                DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                WHERE = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, WHERE);

                datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE qty3>0 " + WHERE;


                DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                    PageRow, Page, OrderBy));
                if (dt.Rows.Count > 0)
                {
                    string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    int total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,排产顺序,排产数量,已派工数量,排产日期,待派工数量";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", dtJson);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
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
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 删除已派工信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DelSupplierDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                //var o = JObject.Parse(Data);
                string id = jarr["id"].ToString().Trim();

                Dictionary<string, string> p = new Dictionary<string, string>();
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(id))
                {
                    sql = "select * from mes012a1 where id='" + id + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    DateTime t1 = DateTime.Now;
                    DateTime t2 = Convert.ToDateTime(dt.Rows[0]["date_start"].ToString().Trim());
                    if (DateTime.Compare(t1, t2) > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该派工任务已到生产日期不能删除";
                    }
                    else
                    {
                        sql = "delete from mes012a1 where id='" + id + "'";
                        DB.ExecuteNonQueryOffline(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "删除成功";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "id不能为空！";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 保存派工信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject SaveSupplierDispatching(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);//查sjemessys库
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                var o = JObject.Parse(Data);
                string table = o["table"].ToString().Trim();
            
                DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(table);
                if (dt.Rows.Count > 0)
                {
                    int n = 0;
                    int a = 0;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string qty3 = DB.GetString(@"
select 
qty_plan-qty2 as qty3 
from 
(
select 
ifnull(SUM(mes012a1.qty),0) as qty2,
mes012m.qty_plan 
from mes012m 
left join mes012a1 on mes012m.production_order=mes012a1.production_order 
and mes012a1.sorting = mes012m.sorting and mes012a1.procedure_no = mes012m.procedure_no
and mes012a1.productionline_no = mes012m.productionline_no and mes012m.date_plan = mes012a1.date_plan
WHERE mes012m.production_order = '"+dt.Rows[i]["production_order"].ToString().Trim()+@"' 
and mes012m.sorting = '"+dt.Rows[i]["sorting"].ToString().Trim()+@"' 
and mes012m.procedure_no = '"+dt.Rows[i]["procedure_no"].ToString().Trim()+@"'
AND mes012m.productionline_no='"+dt.Rows[i]["productionline_no"].ToString().Trim()+@"'
and mes012m.date_plan='"+dt.Rows[i]["date_plan"].ToString().Trim()+@"'
GROUP BY
mes012a1.sorting,
mes012a1.procedure_no,
mes012a1.procedure_name,
mes012a1.productionline_no,
mes012m.sorting_plan,
mes012m.qty_plan,
mes012m.date_plan) tab
");
                        if (Convert.ToDecimal(dt.Rows[i]["qty"].ToString().Trim()) > Convert.ToDecimal(qty3))
                        {
                            n++;
                            ret.IsSuccess = false;
                            ret.ErrMsg = dt.Rows[i]["production_order"].ToString().Trim() + "派工数量大于待派工数量！";
                            return ret;
                        }
                        if (n == 0)
                        {
                            int seq = Convert.ToInt32(DB.GetString("select count(*) from mes012a1 where production_order='" + dt.Rows[i]["production_order"].ToString().Trim() + "' and sorting='" + dt.Rows[i]["sorting"].ToString().Trim() + "' and procedure_no='" + dt.Rows[i]["procedure_no"].ToString().Trim() + "'"));
                            int seqq = seq + 1;
                            string sql = @"insert into mes012a1(production_order,sorting,material_no,material_name,material_specifications,
                                procedure_no,procedure_name,productionline_no,date_plan,
productionsite_no,date_start,qty,seq) values('" + dt.Rows[i]["production_order"].ToString().Trim() + "','" + dt.Rows[i]["sorting"].ToString().Trim() + @"',
'" + dt.Rows[i]["material_no"].ToString().Trim() + "','" + dt.Rows[i]["material_name"].ToString().Trim() + "','" + dt.Rows[i]["material_specifications"].ToString().Trim() + @"',
'" + dt.Rows[i]["procedure_no"].ToString().Trim() + "','" + dt.Rows[i]["procedure_name"].ToString().Trim() + "','" + dt.Rows[i]["productionline_no"].ToString().Trim() + @"'
,'" + dt.Rows[i]["date_plan"].ToString().Trim() + @"','" + dt.Rows[i]["productionsite_no"].ToString().Trim() + @"',
'" + dt.Rows[i]["date_start"].ToString().Trim() + @"','" + dt.Rows[i]["qty"].ToString().Trim() + "','" + seqq + "')";
                            DB.ExecuteNonQueryOffline(sql);
                            a++;
                        }
                    }
                    if (a > 0)
                    {
                        ret.IsSuccess = true;
                        ret.ErrMsg = "派工成功！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "传入参数为空！";
                }
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
