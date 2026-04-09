using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_MESAPI
{
   public class Feeding
    {
        /// <summary>
        /// 获取工单待投料信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkOrderNoFeeding(object OBJ)
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

                
                ret.IsSuccess = true;
                string production_order = jarr["production_order"].ToString().Trim();//工单
                if (!string.IsNullOrEmpty(production_order))
                {
                    string sql = @"
select 
production_order,
procedure_no,
procedure_name,
material_no2,
material_name2,
material_specifications2,
qty2 as qty, 
qty4,
qty2 -qty4 as qty_need
from mes010a4 where production_order = '" + production_order + @"'";
                    DataTable MES010a6 = DB.GetDataTable(sql);
                    if (MES010a6.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(MES010a6);
                        //Dictionary<string, object> p = new Dictionary<string, object>();
                        //p.Add("MES010a6", json);
                        ret.RetData = json;
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "";
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
        /// 获取工单工序待投料信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkOrderProcedureNoFeeding(object OBJ)
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


                ret.IsSuccess = true;
                string production_order = jarr["production_order"].ToString().Trim();//工单
                string procedure_no = jarr["procedure_no"].ToString().Trim();//工序
                if (!string.IsNullOrEmpty(production_order) && !string.IsNullOrEmpty(procedure_no))
                {
                    string sql = @"
select 
production_order,
procedure_no,
procedure_name,
material_no2,
material_name2,
material_specifications2,
qty2 as qty, 
qty4,
qty2 -qty4 as qty_need
from mes010a4 where production_order = '" + production_order + @"'
and procedure_no='"+ procedure_no+@"'";
                    DataTable MES010a6 = DB.GetDataTable(sql);
                    if (MES010a6.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(MES010a6);
                        //Dictionary<string, object> p = new Dictionary<string, object>();
                        //p.Add("MES010a6", json);
                        ret.RetData = json;
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "";
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
        /// 添加工单投料信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddWorkOrderFeeding(object OBJ)
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
                
                ret.IsSuccess = true;

                string reqkey = "production_order,procedure_no,procedure_name,procedure_description,material_no,material_name,material_specifications,qty,lot_no,productionline_no,productionsite_no";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqkey, ReqObj);
                reqP.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                reqP.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                reqP.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));


                if (!string.IsNullOrEmpty(reqP["production_order"].ToString().Trim()))
                {
                    IDataReader idr= DB.GetDataTableReader(@"
select
mes010a4.qty2 as qty_all,
qty2 - qty4 as qty_need,
(select productionline_name from base016m where productionline_no='" + reqP["productionline_no"].ToString().Trim()+ @"') as productionline_name,
(select productionsite_name from base015m where productionsite_no='" + reqP["productionsite_no"].ToString().Trim() + @"') as productionsite_name
from mes010a4 
where production_order = '" + reqP["production_order"].ToString().Trim() + @"' 
and procedure_no='" + reqP["procedure_no"].ToString().Trim() + @"'
and material_no2='" + reqP["material_no"].ToString().Trim() + "'");
                    if (idr.Read())
                    {
                        if (Convert.ToDecimal(reqP["qty"].ToString().Trim()) > Convert.ToDecimal(idr["qty_need"].ToString()))
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "投料数量超出待投料数量！";
                            return ret;
                        }
                        else
                        {
                            reqP.Add("qty_need", idr["qty_need"].ToString());
                            reqP.Add("qty_all", idr["qty_all"].ToString());
                            reqP.Add("productionline_name", idr["productionline_name"].ToString());
                            reqP.Add("productionsite_name", idr["productionsite_name"].ToString());


                            //插入投料记录
                            DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a6", reqP), reqP);

                            //更新工单投料数量
                            DB.ExecuteNonQueryOffline(@"
UPDATE mes010a4
SET qty4=
(select IFNULL(SUM(qty),0)
FROM
mes010a6
where mes010a6.production_order=mes010a4.production_order
AND mes010a6.procedure_no = mes010a4.procedure_no
AND mes010a6.material_no  = mes010a4.material_no2
)
WHERE mes010a4.production_order=@production_order
", reqP);

                            ret.IsSuccess = true;
                            ret.ErrMsg = "保存成功！";
                        }
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "该工单在该工序下没有该品号需要投料";
                    }

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "工单不能为空！";
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
and production_order in (SELECT production_order from mes010a4 where IFNULL(qty4,0)<qty2)
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
    }
}
