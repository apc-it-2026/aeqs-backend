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
    public class Scheduling
    {

        
        /// <summary>
        /// 获取待排产信息接口(急单）
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetNoSchedulingEmergent(object OBJ)
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

                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                Dictionary<string, object> p = new Dictionary<string, object>();



                #region 数据获取SQL
                string datasql = @"
SELECT
*,
tmp.qty1-tmp.qty2 as 'qty3'
FROM
(
select
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no,
mes010m.material_name ,
mes010m.material_specifications ,
mes010m.date_finish_plan,
mes010m.date_start_plan,
mes010a1.sorting,
mes010a1.procedure_no ,
mes010a1.procedure_name ,
mes010a1.procedure_description,
mes010m.qty + mes010m.qty_bf as 'qty1',
ifnull(SUM(mes012m.qty_plan),0) as 'qty2'
from mes010m
LEFT JOIN mes010a1 on mes010m.production_order=mes010a1.production_order
left join MES012M on mes012m.production_order=mes010a1.production_order and mes012m.sorting = mes010a1.sorting and mes012m.procedure_no=mes010a1.procedure_no
WHERE mes010m.`status`='2' and mes010m.emergent='true'
GROUP BY
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no ,
mes010m.material_name ,
mes010m.material_specifications ,
mes010m.date_finish_plan,
mes010m.date_start_plan,
mes010a1.sorting,
mes010a1.procedure_no ,
mes010a1.procedure_name ,
mes010a1.procedure_description,
mes010m.qty
)tmp
where tmp.qty1-tmp.qty2>0 
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
                    total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");


                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "订单,工单,品号,品名,规格,工单计划完工日期,工单计划开工日期,生产顺序,工序代号,工序名称,工序描述,工单数量,已排产数量,待排产数量";
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
        /// 获取已排产信息接口(急单）
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSchedulingEmergent(object OBJ)
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

                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                Dictionary<string, object> p = new Dictionary<string, object>();



                #region 数据获取SQL
                string datasql = @"
SELECT
*
FROM
(
select 
                mes012m.id,
                mes010m.sales_order,
                mes010m.production_order ,
                mes010m.material_no ,
                mes010m.material_name ,
                mes010m.material_specifications ,
                sorting,
                procedure_no ,
                procedure_name ,
                mes012m.productionline_no,
                base016m.productionline_name,
                sorting_plan,
                qty_plan,
                mes012m.date_plan,
                hours
                from mes012m
                JOIN mes010m ON mes012m.production_order =mes010m.production_order
                LEFT JOIN base016m on mes012m.productionline_no = base016m.productionline_no
                WHERE mes010m.emergent='true'
) TMP 
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
                    total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");

                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "id,订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,工作中心代号,工作中心名称,生产顺序,排产数量,排产日期,生产耗时";
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
        /// 获取待排产信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetNoScheduling(object OBJ)
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

                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                Dictionary<string, object > p = new Dictionary<string, object>();



                #region 数据获取SQL
                string datasql = @"
SELECT
*,
tmp.qty1-tmp.qty2 as 'qty3'
FROM
(
select
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no,
mes010m.material_name ,
mes010m.material_specifications ,
mes010m.date_finish_plan,
mes010m.date_start_plan,
mes010a1.sorting,
mes010a1.procedure_no ,
mes010a1.procedure_name ,
mes010a1.procedure_description,
mes010m.qty + mes010m.qty_bf as 'qty1',
ifnull(SUM(mes012m.qty_plan),0) as 'qty2'
from mes010m
LEFT JOIN mes010a1 on mes010m.production_order=mes010a1.production_order
left join MES012M on mes012m.production_order=mes010a1.production_order and mes012m.sorting = mes010a1.sorting and mes012m.procedure_no=mes010a1.procedure_no
WHERE mes010m.`status`='2'  and mes010m.emergent='false'
GROUP BY
mes010m.sales_order,
mes010m.production_order,
mes010m.material_no ,
mes010m.material_name ,
mes010m.material_specifications ,
mes010m.date_finish_plan,
mes010m.date_start_plan,
mes010a1.sorting,
mes010a1.procedure_no ,
mes010a1.procedure_name ,
mes010a1.procedure_description,
mes010m.qty
)tmp
where tmp.qty1-tmp.qty2>0
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
                if(dt.Rows.Count >0)
                {
                    string dtJson = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");


                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "订单,工单,品号,品名,规格,工单计划完工日期,工单计划开工日期,生产顺序,工序代号,工序名称,工序描述,工单数量,已排产数量,待排产数量";
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
        /// 获取已排产信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetScheduling(object OBJ)
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

                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                Dictionary<string, object> p = new Dictionary<string, object>();

    

                #region 数据获取SQL
                string datasql = @"
SELECT
*
FROM
(
select 
                mes012m.id,
                mes010m.sales_order,
                mes010m.production_order ,
                mes010m.material_no ,
                mes010m.material_name ,
                mes010m.material_specifications ,
                sorting,
                procedure_no ,
                procedure_name ,
                mes012m.productionline_no,
                base016m.productionline_name,
                sorting_plan,
                qty_plan,
                mes012m.date_plan,
                hours
                from mes012m
                JOIN mes010m ON mes012m.production_order =mes010m.production_order
                LEFT JOIN base016m on mes012m.productionline_no = base016m.productionline_no
                WHERE mes010m.emergent='false'
) TMP 
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
                    total = DB.GetInt32(@"
select count(1) 
from(" + datasql + @") counttalbe
");

                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "id,订单,工单,品号,品名,规格,生产顺序,工序代号,工序名称,工作中心代号,工作中心名称,生产顺序,排产数量,排产日期,生产耗时";
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
        /// 删除已排产计划接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DelScheduling(object OBJ)
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
                
                string id = jarr["id"].ToString().Trim();//排产id
                Dictionary<string, string> p = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(id))
                {

                    if (DB.GetInt32(@"
SELECT
count(1)
from
mes012a1
join mes012m 
on mes012a1.production_order=mes012m.production_order
and mes012a1.sorting = mes012m.sorting
and mes012a1.procedure_no=mes012m.procedure_no
and mes012a1.productionline_no = mes012m.productionline_no
and mes012a1.date_plan=mes012m.date_plan
where mes012m.id = " + id+@"
"
) >0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "已派工的计划不能删除！";
                        return ret;
                    }

                    DB.ExecuteNonQueryOffline(@"
DELETE FROM mes012m where id="+id+@"
");

                    ret.IsSuccess = true;
                    ret.ErrMsg = "操作成功！";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg ="传参为空！";
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
        /// 获取工作中心排产顺序接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkCenterSorting(object OBJ)
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
                

                string key = "productionline_no,date_plan";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);


                Dictionary<string, object> p = new Dictionary<string, object>();


                int sorting_plan = DB.GetInt32(@"
SELECT
IFNULL(MAX(cast(sorting_plan as SIGNED)),0)
FROM
mes012m
WHERE
productionline_no=@productionline_no and date_plan=@date_plan
", ReqP);
                sorting_plan += 10;

                p.Add("sorting_plan", sorting_plan);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 保存排产计划接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject SaveScheduling(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                


                var jo = JObject.Parse(Data);
                
                
                DataTable dtp = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(jo["table"].ToString().Trim());

               
                
                string sql = string.Empty;
                Dictionary<string, string> p = new Dictionary<string, string>();

                int qa = 0;

                foreach (DataRow dr in dtp.Rows)
                {
                    string sales_order = dr["sales_order"].ToString().Trim();
                    string production_order = dr["production_order"].ToString().Trim();//生产工单
                    string sorting = dr["sorting"].ToString().Trim();
                    string material_no = dr["material_no"].ToString().Trim();
                    string material_name = dr["material_name"].ToString().Trim();
                    string material_specifications = dr["material_specifications"].ToString().Trim();
                    string procedure_no = dr["procedure_no"].ToString().Trim();
                    string procedure_name = dr["procedure_name"].ToString().Trim();
                    string productionline_no = dr["productionline_no"].ToString().Trim();
                    string sorting_plan = dr["sorting_plan"].ToString().Trim();
                    string qty_plan = dr["qty_plan"].ToString().Trim();
                    string date_plan = dr["date_plan"].ToString().Trim();

                    decimal hours = BASE.CalculationProductionHours2(dr["material_no"].ToString(),
                        dr["procedure_no"].ToString(), dr["qty_plan"].ToString().Trim(), ReqObj.UserToken);

                    
                    if (DB.GetInt32(@"
select count(1)
from
mes012m
where production_order='" + production_order + @"'
and sorting ='" + sorting + @"'
and procedure_no='" + procedure_no + @"'
and productionline_no='" + productionline_no + @"'
and date_plan ='" + date_plan + @"'
") != 0)
                    {

                        sql = string.Empty;
                        qa++;


                    }
                    else
                    {
                        sql += @"
insert into mes012m
(production_order,sorting,material_no,material_name,material_specifications,procedure_no,procedure_name,productionline_no,sorting_plan,qty_plan,date_plan,hours)
VALUES
('" + production_order + @"','" + sorting + @"','" + material_no + @"','" + material_name + @"','" + material_specifications + @"','"
 + procedure_no + @"','" + procedure_name + @"','" + productionline_no + @"','" + sorting_plan + @"','" + qty_plan + @"','" + date_plan + @"',"+ hours.ToString("0.00")+@");
";
                    }
                }

                if (qa == 0)
                {
                    DB.ExecuteNonQueryOffline(sql);

                    //更新工单计划开工日期
                    DB.ExecuteNonQueryOffline(@"
UPDATE mes010m set 
date_start_plan=(SELECT MIN(date_plan) from mes012m where production_order='" + dtp.Rows[0]["production_order"].ToString() + @"')
WHERE production_order='" + dtp.Rows[0]["production_order"].ToString() + @"'
");

                    ret.IsSuccess = true;
                    ret.ErrMsg = "排产成功";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "同一 工单/工序/工作中心/日期，不能存在重复数据！";
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
        /// 获取未来一周的工作中心负荷接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkcenterLoad(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                string date = jarr["date"].ToString().Trim();//查询日期
                string sql = string.Empty;
                Dictionary<string, string> p = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(date))
                {
                    DataTable dtData = new DataTable();



                    for (int i = 0; i < 7; i++)
                    {




                        sql = @"select 
'" + date + @"' as date_plan,
base016m.productionline_no,
base016m.productionline_name,
procedure_name,
IFNULL(SUM(hours),0) as hours
from
base016m
left join mes012m on mes012m.productionline_no=base016m.productionline_no
and mes012m.productionline_no<>'' AND mes012m.productionline_no is not null
AND date_plan ='" + date+@"' 
GROUP BY base016m.productionline_no";

                        

                        if(dtData.Rows.Count==0)
                        {
                            dtData = DB.GetDataTable(sql);
                        }
                        else
                        {
                            DataTable dt = DB.GetDataTable(sql);

                            foreach (DataRow row in dt.Rows)
                            {
                                dtData.ImportRow(row);
                            }
                        }

                        date = DateTime.Parse(date).AddDays(1).ToString("yyyy-MM-dd");
                    }

                   
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dtData);
                    ret.IsSuccess = true;
                    ret.RetData = json;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "日期不能为空！";
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
        /// 获取工作中心某天的负荷明细接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkcenterScheduling(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString().Trim();
               
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                
                string date = jarr["date"].ToString().Trim();//查询日期
                string prodctionline_no = jarr["prodctionline_no"].ToString().Trim();//工作中心代号
                string sql = string.Empty;
                Dictionary<string, string> p = new Dictionary<string, string>();
                if (!string.IsNullOrEmpty(date))
                {
                   
                    sql = @"select * from (select sales_order,m.production_order,m.material_no,m.material_name,m.material_specifications,s.sorting,procedure_no,
procedure_name,qty_plan,hours,date_plan,s.productionline_no,s.productionline_name
 from mes012m s LEFT JOIN mes010m m on s.production_order=m.production_order ) tab
where date_plan like '" + date + "%' and  productionline_no='"+ prodctionline_no + "'";
                    DataTable dt = DB.GetDataTable(sql);
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    ret.IsSuccess = true;
                    ret.RetData = json;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "日期不能为空！";
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
