using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_Order_Detail
    {
        //订单明细
        /// <summary>
        /// 查询-订单明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrder_Detail_Main(object OBJ)
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

                string SE_ID = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";//销售订单号
                string mer_po = jarr.ContainsKey("mer_po") ? jarr["mer_po"].ToString() : "";//客户po
                string SHIPCOUNTRY_NAME = jarr.ContainsKey("SHIPCOUNTRY_NAME") ? jarr["SHIPCOUNTRY_NAME"].ToString() : "";//国家
                string size_no = jarr.ContainsKey("size_no") ? jarr["size_no"].ToString() : "";//国家

                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art
                string name_t = jarr.ContainsKey("name_t") ? jarr["name_t"].ToString() : "";//鞋型
                string SE_CUSTID = jarr.ContainsKey("SE_CUSTID") ? jarr["SE_CUSTID"].ToString() : "";//客户编号
                string CUSTORDER = jarr.ContainsKey("CUSTORDER") ? jarr["CUSTORDER"].ToString() : "";//客户订单号
                string WORKORDER_NO = jarr.ContainsKey("WORKORDER_NO") ? jarr["WORKORDER_NO"].ToString() : "";//制令

                string type = jarr.ContainsKey("type") ? jarr["type"].ToString() : "";// false-未勾选 true-勾选


                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//开始日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//结束日期
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                string JoinWhere = string.Empty;
                bool haveFilter = false;
                if (!string.IsNullOrWhiteSpace(mer_po))
                {
                    where += " and m.mer_po like @mer_po";
                    paramTestDic.Add("mer_po", $@"%{mer_po}%");
                    haveFilter = true;
                }

                if (!string.IsNullOrWhiteSpace(SE_ID))
                {
                    where += " and b.SE_ID like @SE_ID";
                    paramTestDic.Add("SE_ID", $@"%{SE_ID}%");
                    haveFilter = true;
                }
                if (!string.IsNullOrWhiteSpace(SHIPCOUNTRY_NAME))
                {
                    where += " and m.Shipcountry_En like @SHIPCOUNTRY_NAME";
                    paramTestDic.Add("SHIPCOUNTRY_NAME", $@"%{SHIPCOUNTRY_NAME}%");//国家
                    haveFilter = true;
                }

                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += " and b.prod_no like @prod_no";
                    paramTestDic.Add("prod_no", $@"%{prod_no}%");
                    haveFilter = true;
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    where += " and r.name_t like @name_t";
                    paramTestDic.Add("name_t", $@"%{name_t}%");
                    haveFilter = true;
                }

                if (!string.IsNullOrWhiteSpace(SE_CUSTID))
                {
                    where += " and m.SE_CUSTID like @SE_CUSTID";
                    paramTestDic.Add("SE_CUSTID", $@"%{SE_CUSTID}%");
                    haveFilter = true;
                }
                if (!string.IsNullOrWhiteSpace(CUSTORDER))
                {
                    where += " and m.CUSTORDER like @CUSTORDER";
                    paramTestDic.Add("CUSTORDER", $@"%{CUSTORDER}%");
                    haveFilter = true;
                }

                if (!string.IsNullOrWhiteSpace(WORKORDER_NO))
                {
                    where += " and m.WORKORDER_NO like @WORKORDER_NO";
                    paramTestDic.Add("WORKORDER_NO", $@"%{WORKORDER_NO}%");
                    haveFilter = true;
                }
                if (!string.IsNullOrWhiteSpace(start_date) || !string.IsNullOrWhiteSpace(end_date))
                {

                    paramTestDic.Add("start_date", $@"{start_date + " 00:00:00"}");
                    paramTestDic.Add("end_date", $@"{end_date + " 23:59:59"}");
                    where += $@" and TO_CHAR( b.NLT ,'yyyy-MM-dd HH24:mi:ss') BETWEEN @start_date AND @end_date ";
                    haveFilter = true;
                }
                string rowWhere = " and rownum<=500 ";
                string rowPoWhere = " AND m.MER_PO IN(SELECT MER_PO FROM bdm_se_order_master WHERE rownum<=500) ";
                if (haveFilter)
                {
                    rowWhere = "";
                    rowPoWhere = "";
                }
                string sql = string.Empty;
                if (type == "false")
                {
                    sql = $@"select * from (
                            SELECT 
                        MAX(TO_CHAR( n.INSERT_TIME ,'yyyy-MM-dd'))as INSERT_TIME ,-- 进仓时间
                        LTRIM(MAX(m.SE_CUSTID),0) as SE_CUSTID,-- 客户编号
                        MAX(b.PROD_NO) PROD_NO,-- art
                        MAX(b.SE_ID) as SE_ID,-- 销售订单
						MAX(m.WORKORDER_NO) as WORKORDER_NO,--制令
                        MAX(m.mer_po) as mer_po, -- po号
                        MAX(r.name_t) as shoe_name,--鞋型
                        MAX(d.mold_no) as mold_no,-- 模号
						MAX(m.CUSTORDER) as CUSTORDER, -- 客户订单号
                        '' as size_no,-- 码数
                        MAX(b.se_qty) as ddse_qty, -- 订单数量
                        MAX(b.se_qty) as ddyxse_qty,-- 订单有效数量
                        MAX(m.Shipcountry_En) as SHIPCOUNTRY_NAME,-- 出货国家
                        MAX(d.SEG_NO)as Gender,--Gender
                        MAX(c.NAME_T) as code_no,-- 鞋型系列
                        MAX(b.column1) as column1,-- VAS
                        MAX(TO_CHAR( b.NST ,'yyyy-MM-dd')) as NST, -- PSDD
                        MAX(TO_CHAR( b.NLT ,'yyyy-MM-dd')) as NLT, -- PODD
                        MAX(TO_CHAR( b.LPD ,'yyyy-MM-dd')) as LPD,-- LPD
                        MAX(TO_CHAR(b.CR_REQDATE ,'yyyy-MM-dd')) as CR_REQDATE,-- CRD

                        MAX(d.develop_season) as develop_season,-- 开发季节
                        MAX(d.develop_type) as develop_type,-- 开发类型
                        MAX(d.color_way) as color_way, -- 鞋款配色
                        MAX(d.user_in_shoecharge) as user_in_shoecharge,-- 开发负责人
                        MAX(m.ORDER_STATUS) as ORDER_STATUS, -- 最终确认
						MAX(m.FINALCOMFIRM_DATE) FINALCOMFIRM_DATE -- 最终确认交期日期
                        FROM 
                        BDM_SE_ORDER_ITEM b
                        LEFT JOIN BDM_RD_STYLE r on b.SHOE_NO=r.SHOE_NO
                        LEFT JOIN BDM_RD_PROD d on b.prod_no=d.prod_no
                        LEFT JOIN BDM_CD_CODE c on r.style_seq=c.code_no
                        LEFT JOIN bdm_se_order_master m on b.se_id=m.se_id
						LEFT JOIN wms_finishedtrackin_orderlist n ON b.SE_ID = n.SE_ID
                        where 1=1 {rowPoWhere} {where}
                         GROUP BY m.mer_po ORDER BY m.mer_po  desc
                        ) where 1=1 {rowWhere} ";
                }
                else
                {
                    sql = $@"SELECT * FROM(
                        SELECT 
                        MAX(TO_CHAR( n.INSERT_TIME ,'yyyy-MM-dd')) as INSERT_TIME ,-- 进仓时间
                        LTRIM(MAX(m.SE_CUSTID),0) as SE_CUSTID,-- 客户编号
                        MAX(b.PROD_NO) PROD_NO,-- art
                        MAX(b.SE_ID) as SE_ID,-- 销售订单
						MAX(m.WORKORDER_NO) as WORKORDER_NO,--制令
                        MAX(m.mer_po) as mer_po, -- po号
                        MAX(r.name_t) as shoe_name,--鞋型
                        MAX(d.mold_no) as mold_no,-- 模号
						MAX(m.CUSTORDER) as CUSTORDER, -- 客户订单号
                        MAX(s.size_no) as size_no,-- 码数
                        MAX(s.se_qty) as ddse_qty, -- 订单数量
                        MAX(b.se_qty) as ddyxse_qty,-- 订单有效数量
                        MAX(m.Shipcountry_En) as SHIPCOUNTRY_NAME,-- 出货国家
                        MAX(d.SEG_NO) as Gender,--Gender
                        MAX(c.NAME_T) as code_no,-- 鞋型系列
                        MAX(b.column1) as column1,-- VAS
                         MAX(TO_CHAR( b.NST ,'yyyy-MM-dd')) as NST, -- PSDD
                        MAX(TO_CHAR( b.NLT ,'yyyy-MM-dd')) as NLT, -- PODD
                        MAX(TO_CHAR( b.LPD ,'yyyy-MM-dd')) as LPD,-- LPD
                        MAX(TO_CHAR(b.CR_REQDATE ,'yyyy-MM-dd')) as CR_REQDATE,-- CRD
                        MAX(d.develop_season) as develop_season,-- 开发季节
                        MAX(d.develop_type) as develop_type,-- 开发类型
                        MAX(d.color_way) as color_way, -- 鞋款配色
                        MAX(d.user_in_shoecharge) as user_in_shoecharge,-- 开发负责人
                        MAX(m.ORDER_STATUS) as ORDER_STATUS, -- 最终确认
						MAX(m.FINALCOMFIRM_DATE) FINALCOMFIRM_DATE -- 最终确认交期日期
                        FROM 
                        BDM_SE_ORDER_ITEM b
                        LEFT JOIN BDM_RD_STYLE r on b.SHOE_NO=r.SHOE_NO
                        LEFT JOIN BDM_SE_ORDER_SIZE s on b.se_id=s.se_id
                        LEFT JOIN BDM_RD_PROD d on b.prod_no=d.prod_no
                        LEFT JOIN BDM_CD_CODE c on r.style_seq=c.code_no
                        LEFT JOIN bdm_se_order_master m on b.se_id=m.se_id
                        LEFT JOIN wms_finishedtrackin_orderlist n ON b.SE_ID = n.SE_ID
                        where 1=1 {rowPoWhere} {where}
                        GROUP BY m.mer_po,s.size_seq ORDER BY m.mer_po,s.size_seq)
                        where 1=1 {rowWhere} ";
                }




                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                //DataTable dt = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
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
    }
}
