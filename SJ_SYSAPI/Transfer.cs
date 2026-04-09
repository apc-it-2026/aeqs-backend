using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SJeMES_Framework_NETCore.Common;

namespace SJ_SYSAPI
{
    /// <summary>
    /// 转单操作
    /// </summary>
    public class Transfer
    {


        #region 客户订单转生产工单

        /// <summary>
        /// 获取没产生工单的订单信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTranSalesOrder(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {
               
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string reqKey = "Where,OrderBy,Page,PageRow";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqKey,ReqObj);


                #region 数据获取SQL
                string datasql = @"
SELECT
    id,
	sales_order,
	order_type,
	order_date,
	customer_name,
	(
	SELECT 
		GROUP_CONCAT(CONCAT('品号:',IFNULL(a1.material_no,''),'(',IFNULL(a1.material_name,''),',',IFNULL(a1.material_specifications,''),') 数量:',IFNULL(a1.qty,''),' 交货日期:',IFNULL(a1.delivery_date,'')) separator '; ')
	FROM 
		erp001a1 a1 
	WHERE
        a1.sales_order = m.sales_order 

	)  materials,
	note 
FROM
	erp001m m where  m.`status` = '2' and (m.production_order='' or m.production_order is null)
";
                #endregion

                DataTable dtColum = DB.GetDataTable(@"
SELECT
*
FROM (" + datasql + @") tmp1 where 1=2");


                reqP["Where"] = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(DB.DataBaseType, dtColum, reqP["Where"].ToString());

                datasql = @"
SELECT
* FROM (" + datasql + @") tmp1 WHERE 1=1 " + reqP["Where"].ToString();


                DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType, datasql,
                    reqP["PageRow"].ToString(), reqP["Page"].ToString(), reqP["OrderBy"].ToString()));
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

                    string headkey = "id,客户订单,订单类型,订单日期,客户名称,订单信息,备注";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    Dictionary<string, object> p = new Dictionary<string, object>();

                    p.Add("headdata", headdata);
                    p.Add("data", dtJson);
                    p.Add("total", total);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "无符合条件的客户订单数据!";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }


        /// <summary>
        /// 客户订单转生产工单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SalesOrderTranProductionOrder(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = ReqObj.Data.ToString();

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {
                string id = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data)["id"].ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);



                string erp001a1Key = "sales_order,serial_number,material_no,material_name,material_specifications,unit,delivery_date,qty";

                Dictionary<string, object> erp001a1P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(erp001a1Key);

                System.Data.DataTable dt = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType,
                    "erp001a1", erp001a1P, " AND sales_order = (SELECT sales_order FROM erp001m where id=" + id+")"));

                string sales_type= DB.GetString(@"SELECT order_type from erp001m where id=" + id + @"");


                if(dt.Rows.Count == 0)
                {
                    string sales_order = DB.GetString(@"SELECT sales_order from erp001m where id=" + id + @"");

                    throw new Exception(sales_order + " 订单信息没有维护品号信息");
                }

                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> drP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByDataRow(dr);

                    drP.Add("sales_type", sales_type);
                   

                    #region 确保没有产生过工单
                    if (DB.GetInt32(@"
SELECT count(1) from mes010m
WHERE sales_order=@sales_order and sales_seq=@serial_number and material_no=@material_no", drP) > 0)
                    {
                        continue;
                    }
                    #endregion


                    CreateMES010(ReqObj, drP);


                    CreateBomMES010(ReqObj, drP);



                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }

        private static void CreateBomMES010(SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj, Dictionary<string, object> P)
        {
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

            System.Data.DataTable dt = DB.GetDataTable(@"
SELECT
'" + P["sales_order"].ToString() + @"' as 'sales_order',
'" + P["serial_number"].ToString() + @"' as 'serial_number',
'" + P["delivery_date"].ToString() + @"' as 'delivery_date',
'" + P["sales_type"].ToString() + @"' as 'sales_type',
material_no2 as 'material_no',
material_name2 as 'material_name',
material_specifications2 as 'material_specifications',
unit,
IFNULL(mes020a1.qty,1)/ifnull(qty_base,1) * (1+ifnull(wastage_rate,0)/100) * " + P["qty"].ToString() + @" as 'qty'
FROM 
mes020a1
join base007m on mes020a1.material_no2=base007m.material_no
where mes020a1.material_no=@material_no
and base007m.material_systype='1'
", P);
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                Dictionary<string, object> drP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByDataRow(dr);


                CreateMES010(reqObj, drP);

                CreateBomMES010(reqObj, drP);
            }
        }

        private static void CreateMES010(SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj, Dictionary<string, object> P)
        {
            string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

            #region 获取工单单头数据
            string mes010mKey =
@"production_order,sales_order,sales_seq,order_date,material_no,material_name,material_specifications,unit,qty,process,process_name,status,date_finish_plan";

            mes010mKey += ",createby,createdate,createtime";

            Dictionary<string, object> mes010mP = StringHelper.GetDictionaryByString(mes010mKey);
            mes010mP = StringHelper.CopyDataDictionary(P, mes010mP);
            mes010mP["sales_seq"] = P["serial_number"];
            if(string.IsNullOrEmpty(P["delivery_date"].ToString()))
            {
                throw new Exception(P["sales_order"].ToString()+ " 订单信息没有维护交货日期");
            }
            mes010mP["date_finish_plan"] = Convert.ToDateTime(P["delivery_date"].ToString()).AddDays(-1).ToString("yyyy-MM-dd");
            mes010mP["createdate"] = mes010mP["order_date"] = DateTime.Now.ToString("yyyy-MM-dd");
            mes010mP["status"] = "8";//待确认
            mes010mP["createby"] = usercode;
            mes010mP["createtime"] = DateTime.Now.ToString("HH:mm:ss");
            mes010mP["order_type"] = "1";//工单类型：生产工单

            if(P["sales_type"].ToString()=="1")
            {
                mes010mP.Add("emergent", "false");
            }
            else if(P["sales_type"].ToString() == "2")
            {
                mes010mP.Add("emergent", "true");
            }


                ///获取工艺流程
                Dictionary<string, object> base007P = DB.GetDictionary(StringHelper.GetSelectSqlByDictionary(DB.DataBaseType,
                "base007m", StringHelper.GetDictionaryByString("process_no,process_name"), " AND material_no=@material_no"), mes010mP);
            if(base007P.Count==0)
            {
                throw new Exception(P["sales_order"].ToString() + " 订单的品号 " + mes010mP["material_no"].ToString() + " 没有维护工艺流程");
            }
            base007P.Add("process", base007P["process_no"]);
            mes010mP = StringHelper.CopyDataDictionary(base007P, mes010mP);

            mes010mP["production_order"] = Logic.GetDocNo("production_order", "mes010m", reqObj.UserToken);

            #endregion

            //添加工单表头
            DB.ExecuteNonQueryOffline(StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010m", mes010mP),mes010mP);
            int mes010mID = DB.GetInt32(@"
SELECT id from mes010m where production_order=@production_order
", mes010mP);
            ///添加工单表身
            SJ_SYSAPI.Logic.AddModuleOtherData("PC_MES010", mes010mID, reqObj.UserToken);

            DB.ExecuteNonQueryOffline(@"
update erp001m
set production_order=CONCAT(ifnull(production_order,''),@production_order,',')
where sales_order=@sales_order
", mes010mP);
        } 


        #endregion
    }
}
