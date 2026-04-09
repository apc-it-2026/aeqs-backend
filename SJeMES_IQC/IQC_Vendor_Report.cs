using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SJeMES_IQC
{
    public class IQC_Vendor_Report
    {
        /// <summary>
        /// T2厂商上传主页查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetVendor_Report_Main(object OBJ)
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
                string SOURCE_NO = jarr.ContainsKey("SOURCE_NO") ? jarr["SOURCE_NO"].ToString() : "";//查询条件 采购单号
                string CGSUPPLIERS_NAME = jarr.ContainsKey("CGSUPPLIERS_NAME") ? jarr["CGSUPPLIERS_NAME"].ToString() : "";//查询条件 采购厂商
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//查询条件 料号
                string ITEM_NAME = jarr.ContainsKey("ITEM_NAME") ? jarr["ITEM_NAME"].ToString() : "";//查询条件 材料名称
                string SCSUPPLIERS_NAME = jarr.ContainsKey("SCSUPPLIERS_NAME") ? jarr["SCSUPPLIERS_NAME"].ToString() : "";//查询条件 生产厂商
                string RCPT_DATES = jarr.ContainsKey("RCPT_DATES") ? jarr["RCPT_DATES"].ToString() : "";//查询条件 收料日期开始
                string RCPT_DATEE = jarr.ContainsKey("RCPT_DATEE") ? jarr["RCPT_DATEE"].ToString() : "";//查询条件 收料日期结束
                string cssc = jarr.ContainsKey("cssc") ? jarr["cssc"].ToString() : "";//查询条件 测试报告上传状态
                string bgsc = jarr.ContainsKey("bgsc") ? jarr["bgsc"].ToString() : "";//查询条件 A-01报告上传状态
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrEmpty(SOURCE_NO))
                {
                    where += $@" and wb.SOURCE_NO like '%{SOURCE_NO}%'";
                }
                if (!string.IsNullOrEmpty(CGSUPPLIERS_NAME))
                {
                    where += $@" and bm.SUPPLIERS_NAME like '%{CGSUPPLIERS_NAME}%'";
                }
                if (!string.IsNullOrEmpty(ITEM_NO))
                {
                    where += $@" and wb.ITEM_NO like '%{ITEM_NO}%'";
                }
                if (!string.IsNullOrEmpty(ITEM_NAME))
                {
                    where += $@" and c.NAME_T like '%{ITEM_NAME}%'";
                }
                if (!string.IsNullOrEmpty(SCSUPPLIERS_NAME))
                {
                    where += $@" and bm2.SUPPLIERS_NAME like '%{SCSUPPLIERS_NAME}%'";
                }
                if (!string.IsNullOrWhiteSpace(RCPT_DATES) && !string.IsNullOrWhiteSpace(RCPT_DATEE))
                {
                    where += $@"  and TO_CHAR(wm.RCPT_DATE,'yyyy-MM-dd') BETWEEN '{RCPT_DATES}' and '{RCPT_DATEE}'";
                }
                //if (!string.IsNullOrWhiteSpace(RCPT_DATES) && string.IsNullOrWhiteSpace(RCPT_DATEE))
                //{
                //    where += $@" and TO_CHAR(wm.RCPT_DATE,'yyyy-MM-dd')  like '%{RCPT_DATES}%'";
                //}
                //if (!string.IsNullOrWhiteSpace(RCPT_DATEE) && string.IsNullOrWhiteSpace(RCPT_DATES))
                //{
                //    where += $@" and TO_CHAR(wm.RCPT_DATE,'yyyy-MM-dd') like '%{RCPT_DATEE}%' ";
                //}
                if (cssc == "uploaded")//已上传
                {
                    where += $@" and wm.VEND_NO IN (SELECT vend_no from bdm_vendor_report_m WHERE report_type='0')
	                            AND wb.SOURCE_NO IN (SELECT order_no from bdm_vendor_report_m WHERE report_type='0')
	                            AND wb.ITEM_NO IN (SELECT item_no from bdm_vendor_report_m WHERE report_type='0')";
                }
                else if (cssc == "Not uploaded")//未上传
                {
                    where += $@" -- and wm.VEND_NO NOT IN (SELECT vend_no from bdm_vendor_report_m WHERE report_type='0')
	                            -- AND wb.SOURCE_NO NOT IN (SELECT order_no from bdm_vendor_report_m WHERE report_type='0')
	                            AND wb.ITEM_NO NOT IN (SELECT item_no from bdm_vendor_report_m WHERE report_type='0')";
                }
                if (bgsc == "uploaded")
                {
                    where += $@" and wm.VEND_NO IN (SELECT vend_no from bdm_vendor_report_m WHERE report_type='1')
	                            AND wb.SOURCE_NO IN (SELECT order_no from bdm_vendor_report_m WHERE report_type='1')
	                            AND wb.ITEM_NO IN (SELECT item_no from bdm_vendor_report_m WHERE report_type='1')";
                }
                else if (bgsc == "Not uploaded")//未上传
                {
                    where += $@" -- and wm.VEND_NO NOT IN (SELECT vend_no from bdm_vendor_report_m WHERE report_type='1')
	                            -- AND wb.SOURCE_NO NOT IN (SELECT order_no from bdm_vendor_report_m WHERE report_type='1')
	                            AND  wb.ITEM_NO NOT IN (SELECT item_no from bdm_vendor_report_m WHERE report_type='1')";
                }
                string searchKeys = $@"
	                            MAX(c.VEND_NO_PRD) AS SCVEND_NO,
		MAX(bm2.SUPPLIERS_NAME ) AS SCSUPPLIERS_NAME,
		wb.SOURCE_NO,
		MAX(wb.ITEM_NO) AS ITEM_NO,
		MAX(c.NAME_T) AS ITEM_NAME,
		MAX( pm.VEND_NO ) AS CGVEND_NO,
		MAX( bm.SUPPLIERS_NAME ) AS CGSUPPLIERS_NAME,
		SUM( pi.ORD_QTY ) as ORD_QTY,
		MAX( wm.RCPT_DATE )  as RCPT_DATE
";
                sql = $@"SELECT
                                /*+index(wb IDX_WMS_RCPT_DA) index(pi PK_BDM_PURCHASE_ORDER_ITEM)*/
                                searchKeys
                            FROM
	                            wms_rcpt_m wm
		                        INNER JOIN wms_rcpt_d wb ON wm.CHK_NO = wb.CHK_NO
                                LEFT JOIN bdm_rd_item c on wb.item_no=c.item_no
		                        INNER JOIN bdm_purchase_order_item pi ON ( pi.ORDER_NO = wb.SOURCE_NO AND pi.ORDER_SEQ = wb.SOURCE_SEQ )
		                        INNER JOIN bdm_purchase_order_m pm ON pm.ORDER_NO  = wb.SOURCE_NO    
		                        LEFT JOIN BASE003M bm on bm.SUPPLIERS_CODE=pm.VEND_NO 
		                        LEFT JOIN BASE003M bm2 on c.VEND_NO_PRD= bm2.SUPPLIERS_CODE
                            WHERE
	                            wm.RCPT_BY = '01' {where} 
                            GROUP BY
	                            wb.SOURCE_NO,
		                        wb.SOURCE_SEQ  ";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"));

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

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
        /// <summary>
        /// T2厂商上传主页上传操作T2测试报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Vendor_Report_Main_EditT2(object OBJ)
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
                string vend_no = jarr.ContainsKey("vend_no") ? jarr["vend_no"].ToString() : "";//查询条件 生产厂商编号
                string order_no = jarr.ContainsKey("order_no") ? jarr["order_no"].ToString() : "";//查询条件 采购单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string report_type = jarr.ContainsKey("report_type") ? jarr["report_type"].ToString() : "";//查询条件 报告类型
                string file_id = jarr.ContainsKey("file_id") ? jarr["file_id"].ToString() : "";//查询条件 文件关联id
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string whereSql = "";
                if (string.IsNullOrEmpty(vend_no))
                    whereSql += $@" AND vend_no is null";
                else
                    whereSql += $@" AND vend_no = '{vend_no}'";

                if (string.IsNullOrEmpty(order_no))
                    whereSql += $@" AND order_no is null";
                else
                    whereSql += $@" AND order_no = '{order_no}'";

                if (string.IsNullOrEmpty(item_no))
                    whereSql += $@" AND item_no is null";
                else
                    whereSql += $@" AND item_no = '{item_no}'";

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from bdm_vendor_report_m where 1=1 {whereSql} and report_type='{report_type}'");
                if (count > 0)
                {
                    sql = $@"update bdm_vendor_report_m set file_id='{file_id}',modifyby='{user}',modifydate='{date}',modifytime='{time}'
                             where 1=1 {whereSql} and report_type='{report_type}'";
                }
                else
                {
                    sql = $@"insert into bdm_vendor_report_m (vend_no,order_no,item_no,report_type,file_id,createby,createdate,createtime)
                             values('{vend_no}','{order_no}','{item_no}','{report_type}','{file_id}','{user}','{date}','{time}')";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();

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

        /// <summary>
        /// T2厂商上传主页上传操作A01报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Vendor_Report_Main_EditA01(object OBJ)
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
                string vend_no = jarr.ContainsKey("vend_no") ? jarr["vend_no"].ToString() : "";//查询条件 生产厂商编号
                string order_no = jarr.ContainsKey("order_no") ? jarr["order_no"].ToString() : "";//查询条件 采购单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                string report_type = jarr.ContainsKey("report_type") ? jarr["report_type"].ToString() : "";//查询条件 报告类型
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";//查询条件 A-01起始日期  
                string file_id = jarr.ContainsKey("file_id") ? jarr["file_id"].ToString() : "";//查询条件 文件关联id
                string report_no = jarr.ContainsKey("report_no") ? jarr["report_no"].ToString() : "";//查询条件 报告编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string whereSql = "";
                if (string.IsNullOrEmpty(vend_no))
                    whereSql += $@" AND vend_no is null";
                else
                    whereSql += $@" AND vend_no = '{vend_no}'";

                if (string.IsNullOrEmpty(order_no))
                    whereSql += $@" AND order_no is null";
                else
                    whereSql += $@" AND order_no = '{order_no}'";

                if (string.IsNullOrEmpty(item_no))
                    whereSql += $@" AND item_no is null";
                else
                    whereSql += $@" AND item_no = '{item_no}'";

                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(1) from bdm_vendor_report_m where 1=1 {whereSql} and report_type='{report_type}'");
                if (count > 0)
                {
                    sql = $@"update bdm_vendor_report_m set report_no='{report_no}',file_id='{file_id}',start_date='{start_date}',modifyby='{user}',modifydate='{date}',modifytime='{time}'
                             where 1=1 {whereSql} and report_type='{report_type}'";
                }
                else
                {
                    sql = $@"insert into bdm_vendor_report_m (report_no,vend_no,order_no,item_no,report_type,start_date,file_id,createby,createdate,createtime)
                             values('{report_no}','{vend_no}','{order_no}','{item_no}','{report_type}','{start_date}','{file_id}','{user}','{date}','{time}')";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();

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

        /// <summary>
        /// T2厂商上传主页查询文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetVendor_Report_Main_ListFile(object OBJ)
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
                string vend_no = jarr.ContainsKey("vend_no") ? jarr["vend_no"].ToString() : "";//查询条件 生产厂商
                string order_no = jarr.ContainsKey("order_no") ? jarr["order_no"].ToString() : "";//查询条件 采购单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料号
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string whereSql = "";
                if (string.IsNullOrEmpty(vend_no))
                    whereSql += $@" AND vend_no is null";
                else
                    whereSql += $@" AND vend_no = '{vend_no}'";

                if (string.IsNullOrEmpty(order_no))
                    whereSql += $@" AND order_no is null";
                else
                    whereSql += $@" AND order_no = '{order_no}'";

                if (string.IsNullOrEmpty(item_no))
                    whereSql += $@" AND item_no is null";
                else
                    whereSql += $@" AND item_no = '{item_no}'";

                string sql = $@"														
                                SELECT
	                                t.file_url,
                                CASE
		                                v.report_type 
		                                WHEN '0' THEN
		                                'T2_Test_Report' --T2测试报告
		                                WHEN '1' THEN
		                                'T2 A-01_Report' --T2 A-01报告
	                                END file_name,
	                                'bdm_vendor_report_m' AS tablename,
	                                v.id 
                                FROM
	                                bdm_vendor_report_m v
	                                INNER JOIN BDM_UPLOAD_FILE_ITEM t ON t.guid = v.file_id 
                                WHERE 1=1 {whereSql}";

                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

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

        public class ModelHelper<T> where T : new()  // 此处一定要加上new()
        {

            public static IList<T> DataTableToModel(DataTable dt)
            {

                IList<T> list = new List<T>();// 定义集合
                Type type = typeof(T); // 获得此模型的类型
                string tempName = "";
                foreach (DataRow dr in dt.Rows)
                {
                    T t = new T();
                    PropertyInfo[] propertys = t.GetType().GetProperties();// 获得此模型的公共属性
                    foreach (PropertyInfo pro in propertys)
                    {
                        tempName = pro.Name;
                        if (dt.Columns.Contains(tempName))
                        {
                            if (!pro.CanWrite) continue;
                            object value = dr[tempName];
                            if (value != DBNull.Value)
                                pro.SetValue(t, value, null);
                        }
                    }
                    list.Add(t);
                }
                return list;
            }
        }
    }
}
