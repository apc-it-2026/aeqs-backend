using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_MESAPI
{
    public class ProductionTransfer
    {
        private decimal getMaxQty(string production_order,string sorting,string procedure_no,SJeMES_Framework_NETCore.DBHelper.DataBase DB)
        {
            decimal maxqty = 0;

            string sortingback = DB.GetString(@"
SELECT
sorting
FROM
mes010a1
WHERE cast(sorting as SIGNED)<cast('" + sorting + "' as SIGNED) and production_order='" + production_order + @"'
ORDER BY sorting desc
limit 0,1
");
            #region 获取最大转移数量
            if (string.IsNullOrEmpty(sortingback))
            {
                maxqty = Convert.ToDecimal(DB.GetString(@"
SELECT
t.gdqty-t.zyqty as 'qty'
FROM
(
SELECT
mes010m.qty as 'gdqty',
IFNULL(SUM(mes010a5.qty1), 0)+IFNULL(SUM(mes010a5.qty2), 0)+IFNULL(SUM(mes010a5.qty4), 0) as 'zyqty'
FROM
mes010a1
join mes010m on mes010a1.production_order = mes010m.production_order
left join mes010a5 on mes010a1.production_order=mes010a5.production_order and mes010a1.sorting=mes010a5.sorting and mes010a1.procedure_no=mes010a5.procedure_no
WHERE mes010a1.production_order='" + production_order + @"'
AND mes010a1.sorting='" + sorting + @"' and mes010a1.procedure_no='" + procedure_no + @"'
)t
"));
            }
            else
            {
                string maxqtysql = @"
SELECT
t2.zyqty - t.zyqty as 'qty'
FROM
(
SELECT
IFNULL(SUM(mes010a5.qty1), 0)+IFNULL(SUM(mes010a5.qty2), 0)+IFNULL(SUM(mes010a5.qty4), 0) as 'zyqty'
FROM
mes010a1
join mes010m on mes010a1.production_order = mes010m.production_order
left join mes010a5 on mes010a1.production_order = mes010a5.production_order and mes010a1.sorting = mes010a5.sorting and mes010a1.procedure_no = mes010a5.procedure_no
WHERE mes010a1.production_order = '" + production_order + @"'
AND mes010a1.sorting = '" + sorting + @"' and mes010a1.procedure_no = '" + procedure_no + @"'
)t , -- 当前工序转移数量
(
SELECT
IFNULL(SUM(mes010a5.qty1), 0)+IFNULL(SUM(mes010a5.qty2), 0)+IFNULL(SUM(mes010a5.qty4), 0) as 'zyqty'
FROM
mes010a1
join mes010m on mes010a1.production_order = mes010m.production_order
left join mes010a5 on mes010a1.production_order = mes010a5.production_order and mes010a1.sorting = mes010a5.sorting and mes010a1.procedure_no = mes010a5.procedure_no
WHERE mes010a1.production_order = '" + production_order + @"'
AND mes010a1.sorting =
(
SELECT
sorting
FROM
mes010a1
WHERE cast(sorting as SIGNED) < cast('" + sorting + @"' as SIGNED) and production_order = '" + production_order + @"'
ORDER BY sorting desc
limit 0, 1)
) t2 -- 上工序转移数量
";
                maxqty =Convert.ToDecimal(DB.GetString(maxqtysql));
            }
            #endregion

            return maxqty;
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
                string WHERE = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑


                #region 数据获取SQL
                string datasql = @"
select
production_order,sales_order,order_date,material_no,material_name,material_specifications,qty,qty_finish,qty_bad,qty_bf
from mes010m
where status='2' and qty>IFNULL(qty_finish,0)
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

                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    string headkey = "工单,订单,工单日期,品号,品名,规格,工单数量,完工数量,不良数量,报废数量";
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


        /// <summary>
        /// 获取工艺转移信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetProcedureInfo(object OBJ)
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
                

                string production_order = jarr["production_order"].ToString().Trim();
                string sorting = jarr["sorting"].ToString().Trim();
                string procedure_no = jarr["procedure_no"].ToString().Trim();
                string sql = string.Empty;

                Dictionary<string, object> p = new Dictionary<string, object>();

                sql = @"
SELECT
mes010a1.production_order,
mes010m.process,
mes002m.process_name,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes010a1.sorting,
mes010a1.procedure_no,
mes010a1.procedure_name,
mes010a1.procedure_description
FROM
mes010a1
join mes010m on mes010a1.production_order=mes010m.production_order
left join mes002m on mes002m.process_no = mes010m.process
WHERE mes010a1.production_order = '" + production_order + @"'
AND mes010a1.sorting='" + sorting + @"'
AND mes010a1.procedure_no='" + procedure_no + @"'
";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataColumn dc in dt.Columns)
                    {
                        p.Add(dc.ColumnName, dt.Rows[0][dc.ColumnName].ToString().Trim());
                    }

                    p.Add("maxqty", getMaxQty(production_order,sorting,procedure_no,DB));


                    #region 获取下工序信息
                    sql = @"
SELECT
mes010a1.sorting,
mes010a1.procedure_no,
mes010a1.procedure_name
FROM
mes010a1
join mes010m on mes010a1.production_order = mes010m.production_order
WHERE mes010a1.production_order='" + production_order + @"'
AND mes010a1.sorting=
(
SELECT
sorting
FROM
mes010a1
WHERE cast(sorting as SIGNED)>cast('" + sorting + @"' as SIGNED) and production_order='" + production_order + @"'
ORDER BY sorting asc
limit 0,1)
"; 
                    #endregion


                    string data = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(DB.GetDataTable(sql));
                    p.Add("data", data);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = true;
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
        /// 获取生产转移单信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionTransferInfo(object OBJ)
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
                

                string production_order = jarr["production_order"].ToString().Trim();
                string sorting = jarr["sorting"].ToString().Trim();
                string procedure_no = jarr["procedure_no"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                int PageRow = Convert.ToInt32(jarr["PageRow"].ToString().Trim());
                int Page = Convert.ToInt32(jarr["Page"].ToString().Trim());
                string sql = string.Empty;
                string OtherWhere = string.Empty;

                if (!string.IsNullOrEmpty(sorting) && !string.IsNullOrEmpty(procedure_no))
                {
                    OtherWhere = " AND sorting='" + sorting + @"' AND procedure_no= '" + procedure_no + @"' ";
                }
                int total = (Page - 1) * PageRow;
                Dictionary<string, object> p = new Dictionary<string, object>();


                sql = @"
SELECT
*
FROM
(
SELECT
tran_no,
tran_type2,
mes010a5.production_order,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes010m.process_name,
sorting,
procedure_no,
procedure_name,
procedure_description,
mes010a5.qty,
mes010a5.qty1,
mes010a5.qty2,
mes010a5.qty3,
mes010a5.qty4,
mes010a5.work_type,
mes010a5.work_center,
T16.productionline_name as work_center_name,
mes010a5.work_site,
T15.productionsite_name as work_site_name,
next_sorting,
next_procedure_no,
next_procedure_name,
next_work_type,
next_work_center,
T16A.productionline_name as next_work_center_name,
next_work_site,
T15A.productionsite_name as next_work_site_name,
mes010a5.team_no,
team_name,
mes030m.machine_name,
@n:= @n + 1 as RN
FROM 
mes010a5
join mes010m on mes010m.production_order = mes010a5.production_order
left join mes002m on mes010m.process=mes002m.process_no
left join hr003m on mes010a5.team_no=hr003m.team_no
left join base016m T16 on T16.productionline_no=work_center
left join base015m T15 on T15.productionsite_no=work_site
left join base016m T16A ON T16A.productionline_no=next_work_center
left join base015m T15A on T15A.productionsite_no=next_work_site
left join mes030m on mes030m.machine_no=device_no
,
(select @n:= 0) d
" + OrderBy + @") TMP
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
and RN >" + total + "  limit " + PageRow + @"
";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string data = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);

                    total = DB.GetInt32(@"
SELECT
count(1)
FROM 
mes010a5
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
");
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    p.Add("headdata", headdata);
                    p.Add("data", data);
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
        /// 获取委外转入单信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionTransferInfo3(object OBJ)
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
                

                string production_order = jarr["production_order"].ToString().Trim();
                string sorting = jarr["sorting"].ToString().Trim();
                string procedure_no = jarr["procedure_no"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                int PageRow = Convert.ToInt32(jarr["PageRow"].ToString().Trim());
                int Page = Convert.ToInt32(jarr["Page"].ToString().Trim());
                string sql = string.Empty;
                string OtherWhere = string.Empty;

                if (!string.IsNullOrEmpty(sorting) && !string.IsNullOrEmpty(procedure_no))
                {
                    OtherWhere = " AND sorting='" + sorting + @"' AND procedure_no= '" + procedure_no + @"' ";
                }
                int total = (Page - 1) * PageRow;
                Dictionary<string, object> p = new Dictionary<string, object>();


                sql = @"
SELECT
*
FROM
(
SELECT
tran_no,
tran_type2,
mes010a5.production_order,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes010m.process_name,
sorting,
procedure_no,
procedure_name,
procedure_description,
mes010a5.qty,
mes010a5.qty1,
mes010a5.qty2,
mes010a5.qty3,
mes010a5.qty4,
mes010a5.work_type,
mes010a5.work_center,
T16.suppliers_name as work_center_name,
mes010a5.work_site,
'' as work_site_name,
next_sorting,
next_procedure_no,
next_procedure_name,
next_work_type,
next_work_center,
T16A.productionline_name as next_work_center_name,
next_work_site,
T15A.productionsite_name as next_work_site_name,
mes010a5.team_no,
team_name,
mes030m.machine_name,
@n:= @n + 1 as RN
FROM 
mes010a5
join mes010m on mes010m.production_order = mes010a5.production_order
left join mes002m on mes010m.process=mes002m.process_no
left join hr003m on mes010a5.team_no=hr003m.team_no
left join base003m T16 on T16.suppliers_code=work_center
left join base016m T16A ON T16A.productionline_no=next_work_center
left join base015m T15A on T15A.productionsite_no=next_work_site
left join mes030m on mes030m.machine_no=device_no
,
(select @n:= 0) d
" + OrderBy + @") TMP
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
and RN >" + total + "  limit " + PageRow + @"
";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string data = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);

                    total = DB.GetInt32(@"
SELECT
count(1)
FROM 
mes010a5
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
");
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    p.Add("headdata", headdata);
                    p.Add("data", data);
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
        /// 获取委外转出单信息接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionTransferInfo2(object OBJ)
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
                

                string production_order = jarr["production_order"].ToString().Trim();
                string sorting = jarr["sorting"].ToString().Trim();
                string procedure_no = jarr["procedure_no"].ToString().Trim();
                string OrderBy = jarr["OrderBy"].ToString().Trim();
                int PageRow = Convert.ToInt32(jarr["PageRow"].ToString().Trim());
                int Page = Convert.ToInt32(jarr["Page"].ToString().Trim());
                string sql = string.Empty;
                string OtherWhere = string.Empty;

                if (!string.IsNullOrEmpty(sorting) && !string.IsNullOrEmpty(procedure_no))
                {
                    OtherWhere = " AND sorting='" + sorting + @"' AND procedure_no= '" + procedure_no + @"' ";
                }
                int total = (Page - 1) * PageRow;
                Dictionary<string, object> p = new Dictionary<string, object>();


                sql = @"
SELECT
*
FROM
(
SELECT
tran_no,
tran_type2,
mes010a5.production_order,
mes010m.material_no,
mes010m.material_name,
mes010m.material_specifications,
mes010m.process_name,
sorting,
procedure_no,
procedure_name,
procedure_description,
mes010a5.qty,
mes010a5.qty1,
mes010a5.qty2,
mes010a5.qty3,
mes010a5.qty4,
mes010a5.work_type,
mes010a5.work_center,
T16.productionline_name as work_center_name,
mes010a5.work_site,
T15.productionsite_name as work_site_name,
next_sorting,
next_procedure_no,
next_procedure_name,
next_work_type,
next_work_center,
T16A.suppliers_name as next_work_center_name,
next_work_site,
'' as next_work_site_name,
mes010a5.team_no,
team_name,
mes030m.machine_name,
@n:= @n + 1 as RN
FROM 
mes010a5
join mes010m on mes010m.production_order = mes010a5.production_order
left join mes002m on mes010m.process=mes002m.process_no
left join hr003m on mes010a5.team_no=hr003m.team_no
left join base016m T16 on T16.productionline_no=work_center
left join base015m T15 on T15.productionsite_no=work_site
left join base003m T16A ON T16A.suppliers_code=next_work_center
left join mes030m on mes030m.machine_no=device_no
,
(select @n:= 0) d
" + OrderBy + @") TMP
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
and RN >" + total + "  limit " + PageRow + @"
";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string data = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);

                    total = DB.GetInt32(@"
SELECT
count(1)
FROM 
mes010a5
WHERE production_order='" + production_order + @"'
" + OtherWhere + @"
");
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);
                    p.Add("headdata", headdata);
                    p.Add("data", data);
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
        /// 生产转移数据提交接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject SaveData(object OBJ)
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
                

                Dictionary<string, object> PInsert = new Dictionary<string, object>();

                #region 参数获取

                string key = "tran_type2,production_order,sorting,procedure_no,procedure_name,procedure_description,qty,qty1,qty2,qty3,qty4,work_type,work_center,work_site,next_sorting,next_procedure_no,next_procedure_name,next_work_type,next_work_center,next_work_site,team_no,device_no";

               
                PInsert = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);
                PInsert.Add("work_center_name", DB.GetString("select productionline_name from base016m where productionline_no='" + PInsert["work_center"].ToString() + "'"));
                PInsert.Add("next_work_center_name", DB.GetString("select productionline_name from base016m where productionline_no='" + PInsert["next_work_center"].ToString() + "'"));

                //如果生产转移或者委外转出
                if (PInsert["tran_type2"].ToString() == "正常转移" || PInsert["tran_type2"].ToString() == "委外转出")
                {
                    PInsert.Add("work_site_name", DB.GetString("select productionsite_name from base015m where productionsite_no='" + PInsert["work_site"].ToString()+"'"));
                }
                else
                {
                    PInsert.Add("work_site_name", DB.GetString("select suppliers_name from base003m where suppliers_code='" + PInsert["work_site"].ToString() + "'"));

                }

                //如果生产转移或者委外转入
                if (PInsert["tran_type2"].ToString() == "正常转移" || PInsert["tran_type2"].ToString() == "委外转入")
                {
                    PInsert.Add("next_work_site_name", DB.GetString("select productionsite_name from base015m where productionsite_no='" + PInsert["next_work_site"].ToString() + "'"));
                }
                else
                {
                    PInsert.Add("next_work_site_name", DB.GetString("select suppliers_name from base003m where suppliers_code='" + PInsert["next_work_site"].ToString() + "'"));
                }
                #endregion
                string sql = string.Empty;

                Dictionary<string, object> p = new Dictionary<string, object>();

                if (Convert.ToDecimal(PInsert["qty"].ToString().Trim()) !=
                    Convert.ToDecimal(PInsert["qty1"].ToString().Trim()) +
                    Convert.ToDecimal(PInsert["qty2"].ToString().Trim()) +
                    Convert.ToDecimal(PInsert["qty3"].ToString().Trim()) +
                    Convert.ToDecimal(PInsert["qty4"].ToString().Trim()))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "转移数量不等于其他数量加起来的总和";

                    return ret;
                }

                if(Convert.ToDecimal(PInsert["qty"].ToString().Trim())<=0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "转移数量必须大于0";

                    return ret;
                }

                #region 判断是否超出最大转移数量
                decimal MaxQty = getMaxQty(PInsert["production_order"].ToString(), PInsert["sorting"].ToString(), PInsert["procedure_no"].ToString(), DB);

                if (MaxQty < Convert.ToDecimal(PInsert["qty"].ToString().Trim()))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "转移数量大于最大可转移数量(" + MaxQty + @")";

                    return ret;
                }

                #endregion

                #region 插入数据


                string date = DateTime.Now.ToString("yyyyMMdd");

                string tran_no = DB.GetString(@"
SELECT
IFNULL(REPLACE(MAX(tran_no),'" + date + @"',''),'0000')
FROM
mes010a5
where tran_no like '" + date + @"%'
");
                tran_no = date + (Convert.ToInt32(tran_no) + 1).ToString("0000");

                PInsert.Add("tran_no", tran_no);
                PInsert.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                PInsert.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                PInsert.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));
                sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a5", PInsert);

                DB.ExecuteNonQueryOffline(sql, PInsert);

                #endregion

                UpdateMes010m(PInsert["production_order"].ToString(),ReqObj);

                ret.IsSuccess = true;
                ret.ErrMsg = "保存成功";

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 转移后更新功能信息
        /// </summary>
        /// <param name="production_order"></param>
        private void UpdateMes010m(string production_order, SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj)
        {
            try
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                ///更新不良数量、报废数量、完工数量
                DB.ExecuteNonQueryOffline(@"
UPDATE mes010m set
qty_bad=(select sum(IFNULL(qty2,0)) from mes010a5 where production_order='" + production_order + @"' and sorting = (SELECT MAX(cast(sorting as SIGNED)) FROM mes010a5 where production_order='" + production_order + @"')),
qty_bf =(select sum(IFNULL(qty3,0)) from mes010a5 where production_order='" + production_order + @"'),
qty_finish=(select sum(IFNULL(qty1,0)) from mes010a5 where production_order='" + production_order + @"'
and sorting=(select max(sorting) from mes010a1 where production_order='" + production_order + @"'))
WHERE production_order='" + production_order + @"' 
");


                ///更新开工日期
                DB.ExecuteNonQueryOffline(@"
UPDATE mes010m set
date_start=(SELECT IFNULL(MIN(createdate),'') FROM mes010a5 where production_order='"+production_order+@"')
WHERE production_order='"+production_order+@"'
");
                //已经完工，更新完工日期
                if(DB.GetInt32(@"select count(1) from mes010m where qty=qty_finish+qty_bad and production_order='" + production_order+@"'")==1)
                {
                    DB.ExecuteNonQueryOffline(@"
UPDATE mes010m set
date_finish=(SELECT IFNULL(MAX(createdate),'') FROM mes010a5 where production_order='" + production_order + @"')
WHERE production_order='" + production_order + @"'
");
                }

              

            }
            catch { }
        }

        /// <summary>
        /// 产生过程检验单
        /// </summary>
        /// <param name="OBJ"></param>
        private void CreateQA041(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            try
            {
                string key = "tran_type2,production_order,sorting,procedure_no,procedure_name,procedure_description,qty,qty1,qty2,qty3,qty4,work_type,work_center,work_site,next_sorting,next_procedure_no,next_procedure_name,next_work_type,next_work_center,next_work_site,team_no,device_no";


                Dictionary<string,object> PInsert = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);
            }
            catch(Exception ex) { throw ex; }
        }
    }
}
