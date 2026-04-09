using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class LaboratorysampleTable
    {
        /// <summary>
        /// 首页实验室样品存放管理展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LaboratorysampleGetList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string NAME_S = jarr.ContainsKey("NAME_S") ? jarr["NAME_S"].ToString() : "";
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";
                string PARENT_ITEM_NO = jarr.ContainsKey("PARENT_ITEM_NO") ? jarr["PARENT_ITEM_NO"].ToString() : "";
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
                string putin_expect = jarr.ContainsKey("putin_expect") ? jarr["putin_expect"].ToString() : "";
                string end_expect = jarr.ContainsKey("end_expect") ? jarr["end_expect"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(ITEM_NO))
                {
                    strwhere += $@" and item_no like '%{ITEM_NO}%'";
                }
                if (!string.IsNullOrEmpty(NAME_S))
                {
                    strwhere += $@" and item_name like '%{NAME_S}%'";
                }
                if (!string.IsNullOrEmpty(SUPPLIERS_NAME))
                {
                    strwhere += $@" and vend_name like '%{SUPPLIERS_NAME}%'";
                }
                if (!string.IsNullOrEmpty(PARENT_ITEM_NO))
                {
                    strwhere += $@" and prod_no like '%{PARENT_ITEM_NO}%'";
                }
                if (!string.IsNullOrEmpty(putin_date) && !string.IsNullOrEmpty(end_date))
                {
                    strwhere += $@"AND ( putin_date  BETWEEN '{putin_date}' AND '{end_date}') ";
                }
                if (!string.IsNullOrEmpty(putin_expect) && !string.IsNullOrEmpty(end_expect))
                {
                    strwhere += $@"AND ( end_date  BETWEEN '{putin_expect}' AND '{end_expect}') ";
                }
                sql = $@"select item_no,item_name,location_name,vend_name,prod_no,putin_date,end_date from  qcm_laboratorysample_storage_m  where 1=1 {strwhere} order by id desc";
                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 新增页面带出
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LaboratorysampleGetView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string sql = $@"select ITEM_NO,ITEM_NO,NAME_S,PARENT_ITEM_NO from bdm_rd_item where ITEM_NO='{item_no}'";
                DataTable dt = DB.GetDataTable(sql);
                string sql2 = $@"select SUPPLIERS_CODE,SUPPLIERS_NAME from BASE003M where SUPPLIERS_CODE=(
(select VEND_NO from WMS_RCPT_M a left
                join WMS_RCPT_D b on a.CHK_NO = b.CHK_NO where a.RCPT_DATE = (
select max(a.RCPT_DATE) from WMS_RCPT_M aa left
                        join WMS_RCPT_D bb on aa.CHK_NO = bb.CHK_NO
)  and b.Item_no = '{item_no}' and ROWNUM = 1))";
                string sql3 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    sql3 = $@"select PARENT_ITEM_NO from bdm_rd_item where ITEM_NO='{dt.Rows[0]["ITEM_NO"]}'";
                    DataTable dt2 = DB.GetDataTable(sql3);
                    dic.Add("data2", dt2);
                }
                DataTable dt1 = DB.GetDataTable(sql2);
                dic.Add("data", dt);
                dic.Add("data1", dt1);
               ;
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 新增页面带出,平板端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LaboratorysampleGetViews(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                Dictionary<string,object> dic = new Dictionary<string,object>();
                //转译
                //先找料品bom parent_item_no suppliers_cide suppliers_cide
                string sql = $@"select item_no,name_s,parent_item_no from bdm_rd_item where ITEM_NO='{item_no}' and ROWNUM=1";
                DataTable dt = DB.GetDataTable(sql);

                string sql2 = $@"select suppliers_name,suppliers_code from BASE003M where SUPPLIERS_CODE=(
(select VEND_NO from WMS_RCPT_M a left
                join WMS_RCPT_D b on a.CHK_NO = b.CHK_NO where a.RCPT_DATE = (
select max(a.RCPT_DATE) from WMS_RCPT_M aa left
                        join WMS_RCPT_D bb on aa.CHK_NO = bb.CHK_NO
)  and b.Item_no = '{item_no}' and ROWNUM = 1))";
                DataTable dt2 = DB.GetDataTable(sql2);
                if (dt2.Rows.Count > 0)
                {
                    dic.Add("item_no", dt.Rows[0]["ITEM_NO"] != null ? dt.Rows[0]["ITEM_NO"].ToString() : "");
                    dic.Add("item_name", dt.Rows[0]["NAME_S"] != null ? dt.Rows[0]["NAME_S"].ToString() : "");
                    dic.Add("prod_no", dt.Rows[0]["PARENT_ITEM_NO"] != null ? dt.Rows[0]["PARENT_ITEM_NO"].ToString() : "");
                    dic.Add("vend_no", dt2.Rows[0]["SUPPLIERS_CODE"] != null ? dt2.Rows[0]["SUPPLIERS_CODE"].ToString() : "");
                    dic.Add("vend_name", dt2.Rows[0]["SUPPLIERS_NAME"] != null ? dt2.Rows[0]["SUPPLIERS_NAME"].ToString() : "");
                }
                else
                {
                    dic.Add("item_no", dt.Rows[0]["ITEM_NO"] != null ? dt.Rows[0]["ITEM_NO"].ToString() : "");
                    dic.Add("item_name", dt.Rows[0]["NAME_S"] != null ? dt.Rows[0]["NAME_S"].ToString() : "");
                    dic.Add("prod_no", dt.Rows[0]["PARENT_ITEM_NO"] != null ? dt.Rows[0]["PARENT_ITEM_NO"].ToString() : "");
                    dic.Add("vend_no", "");
                    dic.Add("vend_name", "");
                }
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 平板端库位扫描
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LaboratorysampleGetKuView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string location_no = jarr.ContainsKey("location_no") ? jarr["location_no"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string sql = $@"select location_no,location_name from BDM_LABORATORYSAMPLE_LOCATION where location_no='{location_no}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("该库位不存在，请重新扫描!");
                }
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 修改前展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LaboratorysampleUpdateView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string sql = $@"select item_no,item_name,vend_name,prod_no,location_name,location_no from  qcm_laboratorysample_storage_m where item_no='{item_no}' and ROWNUM = 1";
                DataTable dt = DB.GetDataTable(sql);
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
    }
}
