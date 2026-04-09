using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_Shipment_Notice
    {
        //出货通知
        /// <summary>
        /// 查询-出货通知
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShipment_Notice_Main(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//客户po
                string SE_ID = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";//销售订单号
                string SHIPCOUNTRY_NAME = jarr.ContainsKey("SHIPCOUNTRY_NAME") ? jarr["SHIPCOUNTRY_NAME"].ToString() : "";//国家

                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//art
                string name_t = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//鞋型


                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束日期
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(PO))
                {
                    where += " and PO_NO like @PO_NO";
                    paramTestDic.Add("PO_NO", $@"%{PO}%");
                }
                if (!string.IsNullOrWhiteSpace(SE_ID))
                {
                    where += " and SE_ID like @SE_ID";
                    paramTestDic.Add("SE_ID", $@"%{SE_ID}%");
                }
                if (!string.IsNullOrWhiteSpace(SHIPCOUNTRY_NAME))
                {
                    where += " and SHIPCOUNTRY_NAME like @SHIPCOUNTRY_NAME";
                    paramTestDic.Add("SHIPCOUNTRY_NAME", $@"%{SHIPCOUNTRY_NAME}%");//国家
                }

                if (!string.IsNullOrWhiteSpace(ART))
                {
                    where += " and prod_no like @ART";
                    paramTestDic.Add("ART", $@"%{ART}%");
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    where += " and shoe_name like @NAME_T";
                    paramTestDic.Add("NAME_T", $@"%{name_t}%");
                }

                if (!string.IsNullOrWhiteSpace(start_date) || !string.IsNullOrWhiteSpace(end_date))
                {

                    paramTestDic.Add("start_date", $@"{start_date + " 00:00:00"}");
                    paramTestDic.Add("end_date", $@"{end_date + " 23:59:59"}");
                    where += $@" and TO_CHAR( tab.aa  ,'yyyy-MM-dd HH24:mi:ss') BETWEEN @start_date AND @end_date ";
                }


                string sql = string.Empty;
                sql = $@"
select 
    tab.*,
    TO_CHAR( tab.aa ,'yyyy-MM-dd') as posting_date 
from (
    select 
       a.se_id,
       r.name_t as shoe_name,
       d.shoe_no,
       c.se_custid,
       c.WORKORDER_NO,
       a.po_no,
       d.prod_no,
       d.se_qty,
       a.boxs_numbers,
       a.status,
       a.delivery_no,
       --c.descountry_name as SHIPCOUNTRY_NAME,
       e.c_name as SHIPCOUNTRY_NAME,
       a.container_truck,
       TO_CHAR( d.nst ,'yyyy-MM-dd') as nst, 
       TO_CHAR( d.CR_REQDATE ,'yyyy-MM-dd') as CR_REQDATE, 
       (select max(posting_date) from bmd_se_shipment_m where se_id=a.se_id) as aa,
       (select listagg(distinct from_line,',') from mms_finishedtrackin_list where se_id=a.SE_ID) as Lines_List,
       a.train_number
    from BDM_SE_SHIPMENT_NOTIFICATION_M a
    left join bmd_se_shipment_m b on a.delivery_no = b.delivery_no
    left join BDM_SE_ORDER_MASTER c on a.se_id = c.se_id
    left join BDM_COUNTRY e on c.descountry_code = e.c_no and e.l_no='EN'
    left join BDM_SE_ORDER_ITEM d on a.se_id = d.se_id
    LEFT JOIN BDM_RD_STYLE r on d.shoe_no=r.shoe_no
    where a.status='7' 
    order by aa desc nulls last
)tab 
where 1=1 {where} ";



               
                //DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShipment_Notice_Main_Export(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//客户po
                string SE_ID = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";//销售订单号
                string SHIPCOUNTRY_NAME = jarr.ContainsKey("SHIPCOUNTRY_NAME") ? jarr["SHIPCOUNTRY_NAME"].ToString() : "";//国家

                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//art
                string name_t = jarr.ContainsKey("name_t") ? jarr["name_t"].ToString() : "";//鞋型


                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束日期

                string where = string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(PO))
                {
                    where += " and PO_NO like @PO_NO";
                    paramTestDic.Add("PO_NO", $@"%{PO}%");
                }
                if (!string.IsNullOrWhiteSpace(SE_ID))
                {
                    where += " and SE_ID like @SE_ID";
                    paramTestDic.Add("SE_ID", $@"%{SE_ID}%");
                }
                if (!string.IsNullOrWhiteSpace(SHIPCOUNTRY_NAME))
                {
                    where += " and SHIPCOUNTRY_NAME like @SHIPCOUNTRY_NAME";
                    paramTestDic.Add("SHIPCOUNTRY_NAME", $@"%{SHIPCOUNTRY_NAME}%");//国家
                }

                if (!string.IsNullOrWhiteSpace(ART))
                {
                    where += " and prod_no like @ART";
                    paramTestDic.Add("ART", $@"%{ART}%");
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    where += " and name_t like @name_t";
                    paramTestDic.Add("name_t", $@"%{name_t}%");
                }

                if (!string.IsNullOrWhiteSpace(start_date) || !string.IsNullOrWhiteSpace(end_date))
                {

                    paramTestDic.Add("start_date", $@"{start_date + " 00:00:00"}");
                    paramTestDic.Add("end_date", $@"{end_date + " 23:59:59"}");
                    where += $@" and TO_CHAR( tab.aa  ,'yyyy-MM-dd HH24:mi:ss') BETWEEN @start_date AND @end_date ";
                }



                string sql = string.Empty;
                sql = $@"select tab.*,TO_CHAR( tab.aa ,'yyyy-MM-dd')  as posting_date from (
                 SELECT
	                     -- m.DELIVERY_NO, -- 出货单号
                        m.SE_ID, -- 销售订单号
						r.name_t as shoe_name,
						o.se_custid, -- 客户号
						o.WORKORDER_NO, -- 制令
						o.mer_po as PO_NO, -- 客户订单号
						b.prod_no, -- art
                         b.se_qty,
						m.BOXS_NUMBERS,-- 箱数
						e.c_name as SHIPCOUNTRY_NAME,-- 国家
						m.CONTAINER_TRUCK,-- 柜车
                        (select listagg(distinct from_line,',') from mms_finishedtrackin_list where se_id=m.SE_ID) as Lines_List,
						(select max(posting_date) from bmd_se_shipment_m where se_id=b.se_id) aa,
						m.TRAIN_NUMBER ,
                        b.nst, 
                        b.CR_REQDATE
                FROM
	                BDM_SE_ORDER_MASTER o
                LEFT JOIN BDM_SE_SHIPMENT_NOTIFICATION_M m on m.se_id=o.se_id
                left join BDM_COUNTRY e on o.descountry_code = e.c_no and e.l_no='EN'
                LEFT JOIN BDM_SE_ORDER_ITEM b on m.SE_ID=b.SE_ID
                LEFT JOIN BDM_RD_STYLE r on b.shoe_no=r.shoe_no
                )tab where 1=1 {where}";






                //DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                dt.Columns.Remove("AA");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEx_LookResult(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 tqc任务编号

                string sql = string.Empty;

                //sql = $@"select prod_no from tqc_task_m where mer_po='{PO}'";
                //string ART = DB.GetString(sql);

                sql = $@"SELECT * FROM(
                        SELECT
	                        task_no
                        FROM
	                        qcm_ex_task_list_m 
                        WHERE
	                        test_type = '4'  
                            AND order_po like '%{PO}%'
	                        ORDER BY
	                        id DESC
	                        )
	                        WHERE  ROWNUM = 1  ";

                string task_no = DB.GetString(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_AQL_Result(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";//查询条件 tqc任务编号
                //string Art = jarr.ContainsKey("Art") ? jarr["Art"].ToString() : "";
                string sql = string.Empty;

                //sql = $@"select prod_no from tqc_task_m where mer_po='{PO}'";
                //string ART = DB.GetString(sql);

                sql = $@"SELECT  
                        a.task_no, 
                        a.po, 
                        a.po_num,
                        a.lot_num,
                        a.inspection_state
                        FROM
                        aql_cma_task_list_m a
                        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                        -- LEFT JOIN bdm_cd_code bb ON d.style_seq=bb.code_no
                        LEFT JOIN aql_cma_task_list_m_aql_m c ON c.task_no=a.task_no 
                        LEFT JOIN BDM_SE_ORDER_MASTER pm ON pm.MER_PO=a.po 
                        LEFT JOIN BDM_SE_ORDER_ITEM pi ON pi.ORG_ID=pm.ORG_ID and pi.SE_ID=pm.SE_ID 
                        left join hr001m hr on hr.staff_no = a.CHECKER 
                        left join hr001m fhr on fhr.staff_no = a.factory_autograph 
                        left join hr001m chr on chr.staff_no = a.customer_autograph 
                        left join (
select 
    a.po,
    listagg(DISTINCT a.from_line, ',') WITHIN GROUP (ORDER BY a.from_line) as from_line
FROM
  mms_finishedtrackin_list a 
group by a.po
) cx on cx.po=a.po 
                        where 1=1 and a.po like '%{PO}%'  order by a.id desc ";

                DataTable dt = DB.GetDataTable(sql);
                string task_no = dt.Rows[0]["task_no"].ToString();
                string po = dt.Rows[0]["po"].ToString();
                string num = dt.Rows[0]["po_num"].ToString();
                string fpnum = dt.Rows[0]["lot_num"].ToString();
                string yhstatus;
                if (dt.Rows[0]["inspection_state"].ToString() == "0")
                {
                    yhstatus = "Not_Inspected";
                } 
                else if (dt.Rows[0]["inspection_state"].ToString() == "1")
                {
                    yhstatus = "Inspected";
                }
                else
                {
                    yhstatus = ""; 
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);
                dic.Add("po", po);
                dic.Add("num", num);
                dic.Add("fpnum", fpnum);
                dic.Add("yhstatus", yhstatus);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true; 
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
    }
}
