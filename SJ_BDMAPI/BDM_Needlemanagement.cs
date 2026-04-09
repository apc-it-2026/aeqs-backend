using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Needlemanagement
    {
        public List<imginfo> image_list = new List<imginfo>();
        public class imginfo
        {
            public string guid { get; set; }
            public string image_url { get; set; }
        }
        /// <summary>
        /// 车间管理主页上半部分数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_View(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ; 
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string org_name = jarr.ContainsKey("org_name") ? jarr["org_name"].ToString() : "";
                string production_line_name = jarr.ContainsKey("production_line_name") ? jarr["production_line_name"].ToString() : "";
                string needle_category_name = jarr.ContainsKey("needle_category_name") ? jarr["needle_category_name"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(org_name))
                {
                    strwhere += $"and A.ORG_NAME='{org_name}'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_name))
                {
                    strwhere += $"and A.PRODUCTION_LINE_NAME='{production_line_name}'";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_name))
                {
                    strwhere += $"and A.NEEDLE_CATEGORY_NAME='{needle_category_name}'";
                }
                var sql = string.Empty;
                sql = $@"
SELECT
	DISTINCT
    A.ID,--ID
     A.ORG_CODE,--厂区编号
     A.ORG_NAME,--厂区
     A.PRODUCTION_LINE_CODE, --产线编号
     A.PRODUCTION_LINE_NAME, --产线
     A.NEEDLE_CATEGORY_NO, --车针类型编号
     A.NEEDLE_CATEGORY_NAME, --车针类别
	    nvl(K.ly_qty,0) as ly_qty,
		nvl(K.fz_qty,0) as fz_qty,
		nvl(K.dz_qty,0) as dz_qty,
    (nvl(K.fz_qty,0)-nvl(K.dz_qty,0)) as zx_qty,
    (nvl(K.ly_qty,0)-nvl(K.fz_qty,0)) as sy_qty
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
LEFT  JOIN (
SELECT
	 X.ORG_CODE,--厂区编号
		X.PRODUCTION_LINE_CODE,--产线
		X.NEEDLE_CATEGORY_NO,--针类型
		SUM(X.ly_qty)AS ly_qty,
SUM(X.fz_qty) AS fz_qty,
SUM(X.dz_qty) AS dz_qty
FROM (
SELECT
     A.ORG_CODE,--厂区编号
		A.PRODUCTION_LINE_CODE,--产线
		A.NEEDLE_CATEGORY_NO,--针类型
		B.ID,
		max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=0 then B.COLLAR_QTY
 END) AS ly_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=1 then B.COLLAR_QTY
 END) AS fz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
		when B.OPA_TYPE=2 then B.COLLAR_QTY
 END) AS dz_qty
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID WHERE 1=1 
GROUP BY  A.ORG_CODE,A.PRODUCTION_LINE_CODE,A.NEEDLE_CATEGORY_NO,b.ID) X GROUP BY X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.NEEDLE_CATEGORY_NO
) K on A.ORG_CODE=K.ORG_CODE AND A.PRODUCTION_LINE_CODE=K.PRODUCTION_LINE_CODE and A.NEEDLE_CATEGORY_NO=K.NEEDLE_CATEGORY_NO
where 1=1  {strwhere}";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                sql = "SELECT NEEDLE_CATEGORY_NO ,NEEDLE_CATEGORY_NAME  FROM BDM_NEEDLE_CATEGORY_M where 1=1";
                DataTable dt1 = DB.GetDataTable(sql);
                dic.Add("Data", dt);
                dic.Add("Datan", dt1);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 发针记录数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_View_fz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                /*if (!string.IsNullOrWhiteSpace(org_name))
                {
                    strwhere += $"A.ORG_NAME='{org_name}'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_name))
                {
                    strwhere += $"A.PRODUCTION_LINE_NAME='{production_line_name}'";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_name))
                {
                    strwhere += $"A.NEEDLE_CATEGORY_NAME='{needle_category_name}'";
                }*/
                var sql = string.Empty;
                sql = $@"SELECT
    A.ID,
	A.COLLAR_QTY,
	A.COLLAR_DATE,
	B.STAFF_NAME，
    A.remarks
FROM
	QCM_CAR_NEEDLE_C A
LEFT JOIN HR001M B ON A.CREATEBY=B.STAFF_NO  WHERE A.M_ID='{m_id}' AND A.OPA_TYPE='1'";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 断针记录数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_View_dz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                /*if (!string.IsNullOrWhiteSpace(org_name))
                {
                    strwhere += $"A.ORG_NAME='{org_name}'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_name))
                {
                    strwhere += $"A.PRODUCTION_LINE_NAME='{production_line_name}'";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_name))
                {
                    strwhere += $"A.NEEDLE_CATEGORY_NAME='{needle_category_name}'";
                }*/
                var sql = string.Empty;
                sql = $@"SELECT
    A.ID,
	A.COLLAR_QTY,
	A.COLLAR_DATE,
	B.STAFF_NAME,
    A.remarks
FROM
	QCM_CAR_NEEDLE_C A
LEFT JOIN HR001M B ON A.CREATEBY=B.STAFF_NO  WHERE A.M_ID='{m_id}' AND A.OPA_TYPE='2'";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 用针领用记录数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_View_ly(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                /*if (!string.IsNullOrWhiteSpace(org_name))
                {
                    strwhere += $"A.ORG_NAME='{org_name}'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_name))
                {
                    strwhere += $"A.PRODUCTION_LINE_NAME='{production_line_name}'";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_name))
                {
                    strwhere += $"A.NEEDLE_CATEGORY_NAME='{needle_category_name}'";
                }*/
                var sql = string.Empty;
                sql = $@"SELECT
    A.ID,
	A.COLLAR_QTY,
	A.COLLAR_DATE,
	B.STAFF_NAME,
    A.remarks
FROM
	QCM_CAR_NEEDLE_C A
LEFT JOIN HR001M B ON A.CREATEBY=B.STAFF_NO  WHERE A.M_ID='{m_id}' AND A.OPA_TYPE='0'";//领用
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 车针管理添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_add(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string org_code = jarr.ContainsKey("org_code") ? jarr["org_code"].ToString() : "";
                string org_name = jarr.ContainsKey("org_name") ? jarr["org_name"].ToString() : "";
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";
                string production_line_name = jarr.ContainsKey("production_line_name") ? jarr["production_line_name"].ToString() : "";
                string needle_category_no = jarr.ContainsKey("needle_category_no") ? jarr["needle_category_no"].ToString() : "";
                string needle_category_name = jarr.ContainsKey("needle_category_name") ? jarr["needle_category_name"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";
                sql = $@"select count(*) from qcm_car_needle_m where production_line_code='{production_line_code}' AND needle_category_no='{needle_category_no}' AND org_code='{org_code}'";
                int ty = Convert.ToInt32(DB.GetString(sql));
                if (ty > 0)
                {
                    ret.ErrMsg = $"The same [factory {org_name}] [production line {production_line_name}] [bur {needle_category_name}] data already exists, please re-enter";
                    ret.IsSuccess = false;
                    return ret;
                }
                Dictionary<string, object> dic;
                sql = $@"insert into qcm_car_needle_m (
	                         org_code,
	                         org_name,
	                         production_line_code,
	                         production_line_name,
	                         needle_category_no,
	                         needle_category_name,
	                         remarks,
                             createby,
                             createdate,
                             createtime
                             )values(@org_code,@org_name,@production_line_code,@production_line_name,@needle_category_no,@needle_category_name,@remarks,@createby,@createdate,@createtime)";
                dic = new Dictionary<string, object>();
                dic.Add("org_code", org_code);
                dic.Add("org_name", org_name);
                dic.Add("production_line_code", production_line_code);
                dic.Add("production_line_name", production_line_name);
                dic.Add("needle_category_no", needle_category_no);
                dic.Add("needle_category_name", needle_category_name);
                dic.Add("remarks", remarks);
                dic.Add("createby", userCode);
                dic.Add("createdate", currDate.ToString("yyyy-MM-dd"));
                dic.Add("createtime", currDate.ToString("HH:mm:ss"));
                DB.ExecuteNonQuery(sql, dic);

                DB.Commit();
                ret.ErrMsg = "Successful operation！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "operation failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理添加===>发针添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_addfz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string collar_qty = jarr.ContainsKey("collar_qty") ? jarr["collar_qty"].ToString() : "";
                string collar_date = jarr.ContainsKey("collar_date") ? jarr["collar_date"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";
                string item_id = string.Empty;
                sql = $@"insert into qcm_car_needle_c(m_id,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Added successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "add failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理添加===>断针添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_adddz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string collar_qty = jarr.ContainsKey("collar_qty") ? jarr["collar_qty"].ToString() : "";
                string collar_date = jarr.ContainsKey("collar_date") ? jarr["collar_date"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";
                string item_id = string.Empty;
                sql = $@"insert into qcm_car_needle_c(m_id,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                DB.ExecuteNonQuery(sql);

                item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_car_needle_c");
                List<imginfo> img_url = JsonConvert.DeserializeObject<List<imginfo>>(jarr["guid_list"].ToString());
                if (img_url.Count > 0)
                {

                    if (!string.IsNullOrWhiteSpace(item_id))
                    {
                        foreach (imginfo item in img_url)
                        {
                            sql = $"insert into qcm_car_needle_f(c_id,file_guid,createby,createdate,createtime) values('{item_id}','{item.guid}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";//添加对应的guid关联图片
                            DB.ExecuteNonQuery(sql);
                        }

                    }

                }
                DB.Commit();
                ret.ErrMsg = "Added successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "add failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理添加===>发针领用添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_addly(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string m_id = jarr.ContainsKey("m_id") ? jarr["m_id"].ToString() : "";
                string collar_qty = jarr.ContainsKey("collar_qty") ? jarr["collar_qty"].ToString() : "";
                string collar_date = jarr.ContainsKey("collar_date") ? jarr["collar_date"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";
                sql = $@"insert into qcm_car_needle_c(m_id,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Added successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "add failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_delete(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                sql = $@"select id from qcm_car_needle_c where m_id='{id}'";
                DataTable c_id = DB.GetDataTable(sql);

                if (c_id.Rows.Count > 0)
                {
                    ret.ErrMsg = "A single record exists and cannot be deleted";
                    ret.IsSuccess = false;
                    return ret;
                   /* for (int i = 0; i < c_id.Rows.Count; i++)
                    {
                        sql = $"delete from qcm_car_needle_f where id='{c_id.Rows[i]["id"]}'";
                        DB.ExecuteNonQuery(sql);
                    }*/
                }
                sql = $@"delete from qcm_car_needle_m where  id='{id}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"delete from qcm_car_needle_c where m_id='{id}'";
                DB.ExecuteNonQuery(sql);

                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理===>发针删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_delete_fz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                sql = $@"delete from qcm_car_needle_c where  id='{id}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理===>断针删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_delete_dz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";

                sql = $@"delete from qcm_car_needle_c where  id='{id}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"delete from qcm_car_needle_f where  c_id='{id}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理===>断针删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_delete_ly(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";

                sql = $@"delete from qcm_car_needle_c where  id='{id}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }


        //--------------------------------PDA------------------------------------------//

        /// <summary>
        /// 车间管理主页上半部分数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getBurrList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string org_code = jarr.ContainsKey("org_code") ? jarr["org_code"].ToString() : "";
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";
                string needle_category_no = jarr.ContainsKey("needle_category_no") ? jarr["needle_category_no"].ToString() : "";
                string needle_size = jarr.ContainsKey("needle_size") ? jarr["needle_size"].ToString() : "";
                string pageSize = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string strwhere = string.Empty;
                string COLLAR_SIZE1 = string.Empty;
                string COLLAR_SIZE2 = string.Empty;

                if (!string.IsNullOrWhiteSpace(org_code))
                {
                    strwhere += $" and A.org_code='{org_code}' ";
                }
                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $" and A.production_line_code='{production_line_code}' ";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_no))
                {
                    strwhere += $" and A.needle_category_no='{needle_category_no}' ";
                }
                if (!string.IsNullOrWhiteSpace(needle_size))
                {
                    strwhere += $" and K.COLLAR_SIZE='{needle_size}' ";
                    COLLAR_SIZE1 = ",X.COLLAR_SIZE";
                    COLLAR_SIZE2 = ",B.COLLAR_SIZE";
                }
                //string permission = DB.GetString($@"SELECT NEEDLE_PERMISSION FROM QCM_APP_PERMISSION_SETTING_M WHERE STAFF_NO='{user}'");
                //if (permission != "1")
                //{
                //    strwhere += $" and A.CREATEBY='{user}' ";
                //}                                               ////// Commented by Ashok to show all needle details to all users
                var sql = string.Empty;


                #region query before adding needle size
                //                sql = $@"
                //select * from (
                //SELECT
                //	DISTINCT
                //    A.ID,--ID
                //     A.ORG_NAME,--厂区
                //     A.PRODUCTION_LINE_NAME, --产线
                //     A.NEEDLE_CATEGORY_NAME, --车针类别
                //	(nvl(K.ly_qty,0)-nvl(K.hz_qty,0)) as receive_num,
                //    nvl(K.fz_qty,0) as send_num,
                //	(nvl(K.dz_qty,0)-nvl(K.hz_qty,0)) as bad_num,
                //    (nvl(K.fz_qty,0)-nvl(K.dz_qty,0)) as useing_num,
                //    (nvl(K.ly_qty,0)-nvl(k.fz_qty,0)) as residue_num,
                //    (CASE  
                //  when A .MODIFYDATE is NOT NULL then REPLACE (A .MODIFYDATE, '-', '') || REPLACE (A .MODIFYTIME, ':', '')
                //  else REPLACE (A .CREATEDATE, '-', '') || REPLACE (A .CREATETIME, ':', '')
                //  END ) as order_time
                // FROM
                //     QCM_CAR_NEEDLE_M A
                // LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
                //LEFT  JOIN (
                //SELECT
                //	 X.ORG_CODE,--厂区编号
                //		X.PRODUCTION_LINE_CODE,--产线
                //		X.NEEDLE_CATEGORY_NO,--针类型
                //		SUM(X.ly_qty)AS ly_qty,
                //SUM(X.fz_qty) AS fz_qty,
                //SUM(X.dz_qty) AS dz_qty,
                //SUM(X.hz_qty) AS hz_qty
                //FROM (
                //SELECT
                //     A.ORG_CODE,--厂区编号
                //		A.PRODUCTION_LINE_CODE,--产线
                //		A.NEEDLE_CATEGORY_NO,--针类型
                //		B.ID,
                //		max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=0 then B.COLLAR_QTY
                // END) AS ly_qty,
                //max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=1 then B.COLLAR_QTY
                // END) AS fz_qty,
                //max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=2 then B.COLLAR_QTY
                // END) AS dz_qty,
                //max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=3 then B.COLLAR_QTY
                // END) AS hz_qty
                // FROM
                //     QCM_CAR_NEEDLE_M A
                // LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID WHERE 1=1 
                //GROUP BY  A.ORG_CODE,A.PRODUCTION_LINE_CODE,A.NEEDLE_CATEGORY_NO,b.ID) X GROUP BY X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.NEEDLE_CATEGORY_NO
                //) K on A.ORG_CODE=K.ORG_CODE AND A.PRODUCTION_LINE_CODE=K.PRODUCTION_LINE_CODE and A.NEEDLE_CATEGORY_NO=K.NEEDLE_CATEGORY_NO
                //where 1=1  {strwhere} ) order by order_time desc";

                #endregion


                sql = $@"
select * from (
SELECT
  DISTINCT
    A.ID,--ID
     A.ORG_NAME,--厂区
     A.PRODUCTION_LINE_NAME, --产线
     A.NEEDLE_CATEGORY_NAME, --车针类别
  (nvl(K.ly_qty,0)-nvl(K.hz_qty,0)) as receive_num,
    nvl(K.fz_qty,0) as send_num,
  (nvl(K.dz_qty,0)-nvl(K.hz_qty,0)) as bad_num,
    (nvl(K.fz_qty,0)-nvl(K.dz_qty,0)) as useing_num,
    (nvl(K.ly_qty,0)-nvl(k.fz_qty,0)) as residue_num,
    (CASE  
  when A .MODIFYDATE is NOT NULL then REPLACE (A .MODIFYDATE, '-', '') || REPLACE (A .MODIFYTIME, ':', '')
  else REPLACE (A .CREATEDATE, '-', '') || REPLACE (A .CREATETIME, ':', '')
  END ) as order_time
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
LEFT  JOIN (
SELECT
   X.ORG_CODE,--厂区编号
    X.PRODUCTION_LINE_CODE,--产线
    X.NEEDLE_CATEGORY_NO,
      --针类型
    SUM(X.ly_qty)AS ly_qty,
SUM(X.fz_qty) AS fz_qty,
SUM(X.dz_qty) AS dz_qty,
SUM(X.hz_qty) AS hz_qty
{COLLAR_SIZE1}
FROM (
SELECT
     A.ORG_CODE,--厂区编号
    A.PRODUCTION_LINE_CODE,--产线
    A.NEEDLE_CATEGORY_NO,
  
    max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=0 then B.COLLAR_QTY
 END) AS ly_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=1 then B.COLLAR_QTY
 END) AS fz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=2 then B.COLLAR_QTY
 END) AS dz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=3 then B.COLLAR_QTY
 END) AS hz_qty
  {COLLAR_SIZE2}
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID WHERE 1=1 
GROUP BY  A.ORG_CODE,A.PRODUCTION_LINE_CODE,A.NEEDLE_CATEGORY_NO,b.ID  {COLLAR_SIZE2} ) X GROUP BY X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.NEEDLE_CATEGORY_NO  {COLLAR_SIZE1}
) K on A.ORG_CODE=K.ORG_CODE AND A.PRODUCTION_LINE_CODE=K.PRODUCTION_LINE_CODE and A.NEEDLE_CATEGORY_NO=K.NEEDLE_CATEGORY_NO
where 1=1  {strwhere} ) order by order_time desc";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageSize), int.Parse(pageIndex));

                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// PDA领针，断针，领用的记录
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_PDAView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string m_id = jarr.ContainsKey("id") ? jarr["id"].ToString() : ""; 
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : ""; 
                string pageSize = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string strwhere = string.Empty;
                /*if (!string.IsNullOrWhiteSpace(org_name))
                {
                    strwhere += $"A.ORG_NAME='{org_name}'";
                }
                if (!string.IsNullOrWhiteSpace(production_line_name))
                {
                    strwhere += $"A.PRODUCTION_LINE_NAME='{production_line_name}'";
                }
                if (!string.IsNullOrWhiteSpace(needle_category_name))
                {
                    strwhere += $"A.NEEDLE_CATEGORY_NAME='{needle_category_name}'";
                }*/
                var sql = string.Empty;
                sql = $@"SELECT
    A.ID,
    A.COLLAR_SIZE,  --newly added for needle sizes by Ashok
	A.COLLAR_QTY, 
	A.COLLAR_DATE,
	B.STAFF_NAME,
    A.remarks,
    A.opa_type
FROM
	QCM_CAR_NEEDLE_C A
LEFT JOIN HR001M B ON A.CREATEBY=B.STAFF_NO  WHERE A.M_ID='{m_id}' AND A.OPA_TYPE='{opa_type}' ORDER BY A.COLLAR_DATE DESC";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageSize), int.Parse(pageIndex));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                switch (opa_type)
                {
                    case "2":
                        sql = $@"SELECT
    A.ID,
    A.COLLAR_SIZE,  --newly added for needle sizes by Ashok
	A.COLLAR_QTY,
	A.COLLAR_DATE,
	B.STAFF_NAME,
    A.remarks
FROM
	QCM_CAR_NEEDLE_C A
LEFT JOIN HR001M B ON A.CREATEBY=B.STAFF_NO  WHERE A.M_ID='{m_id}' AND A.OPA_TYPE='{opa_type}' ORDER BY A.COLLAR_DATE DESC";
                        dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageSize), int.Parse(pageIndex));
                        rowCount= CommonBASE.GetPageDataTableCount(DB, sql);
                        dt.Columns.Add("guid_list", Type.GetType("System.Object"));
                        if (dt.Rows.Count > 0)
                        {
                            foreach (DataRow item in dt.Rows)
                            {
                                sql = $@"SELECT
	A.ID, 
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	qcm_car_needle_f A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID
WHERE
	A.c_ID = '{item["ID"]}'";
                                DataTable dt_img = DB.GetDataTable(sql);
                                List<imginfo> images = new List<imginfo>();
                                if (dt_img.Rows.Count > 0)
                                {
                                    for (int i = 0; i < dt_img.Rows.Count; i++)
                                    {
                                        images.Add(new imginfo
                                        {
                                            guid = dt_img.Rows[i]["GUID"].ToString(),
                                            image_url = dt_img.Rows[i]["FILE_URL"].ToString()
                                        }); ;
                                    }
                                }
                                item["guid_list"] = images;
                            }
                        }
                        break;
                }
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 车针管理添加===>发针,领用，断针添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_PDAadd(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string m_id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//表头ID
                string collar_qty = jarr.ContainsKey("collar_qty") ? jarr["collar_qty"].ToString() : "";
                string collar_size = jarr.ContainsKey("collar_size") ? jarr["collar_size"].ToString() : "";
                string collar_date = jarr.ContainsKey("collar_date") ? jarr["collar_date"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : ""; 
                string sql2 = string.Empty;
                sql = $@"select NEEDLE_SIZE as value from bdm_needle_size_m where NEEDLE_SIZE='{collar_size}'";
                DataTable dt2 = DB.GetDataTable(sql);
                if (dt2.Rows.Count==0)
                {
                    ret.ErrMsg = "Please select Needle Size first";
                    ret.IsSuccess = false;
                    return ret;
                }

                sql = $@"UPDATE QCM_CAR_NEEDLE_M SET MODIFYBY='{userCode}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE ID={m_id}";
                DB.ExecuteNonQuery(sql);

                string item_id = string.Empty;

                #region query before needle size added
                //                sql = $@"
                //SELECT
                //	DISTINCT
                //    A.ID,--ID
                //     A.ORG_CODE,--厂区编号
                //     A.ORG_NAME,--厂区
                //     A.PRODUCTION_LINE_CODE, --产线编号
                //     A.PRODUCTION_LINE_NAME, --产线
                //     A.NEEDLE_CATEGORY_NO, --车针类型编号
                //     A.NEEDLE_CATEGORY_NAME, --车针类别
                //     A.REMARKS, --备注
                //     B.COLLAR_DATE, --日期
                //	   nvl(K.ly_qty,0) as ly_qty,
                //	    nvl(K.fz_qty,0) as fz_qty,
                //		nvl(K.dz_qty,0) as dz_qty,
                //    nvl((K.fz_qty-K.dz_qty),0) as zx_qty,
                //    nvl((K.ly_qty-fz_qty),0) as sy_qty
                // FROM
                //     QCM_CAR_NEEDLE_M A
                // LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
                //LEFT  JOIN (
                //SELECT
                //	 X.ORG_CODE,--厂区编号
                //		X.PRODUCTION_LINE_CODE,--产线
                //		X.NEEDLE_CATEGORY_NO,--针类型
                //		SUM(X.ly_qty)AS ly_qty,
                //SUM(X.fz_qty) AS fz_qty,
                //SUM(X.dz_qty) AS dz_qty
                //FROM (
                //SELECT
                //     A.ORG_CODE,--厂区编号
                //		A.PRODUCTION_LINE_CODE,--产线
                //		A.NEEDLE_CATEGORY_NO,--针类型
                //		B.ID,
                //		max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=0 then B.COLLAR_QTY
                // END) AS ly_qty,
                //max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=1 then B.COLLAR_QTY
                // END) AS fz_qty,
                //max(
                //CASE
                //   WHEN  B.OPA_TYPE is  null THEN 0
                //		when B.OPA_TYPE=2 then B.COLLAR_QTY
                // END) AS dz_qty
                // FROM
                //     QCM_CAR_NEEDLE_M A
                // LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID WHERE 1=1 
                //GROUP BY  A.ORG_CODE,A.PRODUCTION_LINE_CODE,A.NEEDLE_CATEGORY_NO,b.ID) X GROUP BY X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.NEEDLE_CATEGORY_NO
                //) K on A.ORG_CODE=K.ORG_CODE AND A.PRODUCTION_LINE_CODE=K.PRODUCTION_LINE_CODE and A.NEEDLE_CATEGORY_NO=K.NEEDLE_CATEGORY_NO
                //where 1=1 and A.ID='{m_id}'";
                #endregion

                sql = $@"

SELECT
  DISTINCT
    A.ID,--ID
     A.ORG_CODE,--厂区编号
     A.ORG_NAME,--厂区
     A.PRODUCTION_LINE_CODE, --产线编号
     A.PRODUCTION_LINE_NAME, --产线
     A.NEEDLE_CATEGORY_NO, --车针类型编号
     A.NEEDLE_CATEGORY_NAME, --车针类别
     A.REMARKS, --备注
     B.COLLAR_DATE,
     K.COLLAR_SIZE ,--日期
     nvl(K.ly_qty,0) as ly_qty,
      nvl(K.fz_qty,0) as fz_qty,
    nvl(K.dz_qty,0) as dz_qty,
    nvl((K.fz_qty-K.dz_qty),0) as zx_qty,
    nvl((K.ly_qty-fz_qty),0) as sy_qty
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID 
LEFT  JOIN (
SELECT
   X.ORG_CODE,--厂区编号
    X.PRODUCTION_LINE_CODE,--产线
    X.NEEDLE_CATEGORY_NO,
    X.COLLAR_SIZE,--针类型
    SUM(X.ly_qty)AS ly_qty,
SUM(X.fz_qty) AS fz_qty,
SUM(X.dz_qty) AS dz_qty
FROM (
SELECT
     A.ORG_CODE,--厂区编号
    A.PRODUCTION_LINE_CODE,--产线
    A.NEEDLE_CATEGORY_NO,--针类型
    B.ID,
    B.Collar_Size,
    max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=0 then B.COLLAR_QTY
 END) AS ly_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=1 then B.COLLAR_QTY
 END) AS fz_qty,
max(
CASE
   WHEN  B.OPA_TYPE is  null THEN 0
    when B.OPA_TYPE=2 then B.COLLAR_QTY
 END) AS dz_qty
 FROM
     QCM_CAR_NEEDLE_M A
 LEFT JOIN QCM_CAR_NEEDLE_C B ON A.ID = B.M_ID WHERE 1=1 and A.ID='{m_id}'
GROUP BY  A.ORG_CODE,A.PRODUCTION_LINE_CODE,A.NEEDLE_CATEGORY_NO,b.ID, B.COLLAR_SIZE) X GROUP BY X.ORG_CODE,X.PRODUCTION_LINE_CODE,X.NEEDLE_CATEGORY_NO,X.COLLAR_SIZE
) K on A.ORG_CODE=K.ORG_CODE AND A.PRODUCTION_LINE_CODE=K.PRODUCTION_LINE_CODE and A.NEEDLE_CATEGORY_NO=K.NEEDLE_CATEGORY_NO
where 1=1 and A.ID='{m_id}'and K.COLLAR_SIZE='{collar_size}'";
                DataTable dt = DB.GetDataTable(sql);
                decimal fz_qty = 0;
                decimal dz_qty = 0;
                decimal ly_qty = 0;
                decimal collar_qtys = Convert.ToDecimal(collar_qty);//录入的数量
                if (dt.Rows.Count > 0)
                {
                    fz_qty = Convert.ToDecimal(dt.Rows[0]["fz_qty"].ToString());
                    dz_qty = Convert.ToDecimal(dt.Rows[0]["dz_qty"].ToString());
                    ly_qty = Convert.ToDecimal(dt.Rows[0]["ly_qty"].ToString());
                }
                switch (opa_type)
                {
                    //领用的
                    case "0":
                        
                        //sql = $@"insert into qcm_car_needle_c(m_id,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        sql = $@"insert into qcm_car_needle_c(m_id,collar_size,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_size}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        DB.ExecuteNonQuery(sql);
                        break;
                    //发针
                    case "1":
                        //领用的-发针的<输入的”
                        if ((ly_qty - fz_qty) < collar_qtys)
                        {
                            ret.ErrMsg = "The number of hair pins cannot be greater than the number of receipts";
                            ret.IsSuccess = false;
                            return ret;
                        }
                       // sql = $@"insert into qcm_car_needle_c(m_id,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        sql = $@"insert into qcm_car_needle_c(m_id,collar_size,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_size}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        DB.ExecuteNonQuery(sql);
                        break;
                    //断针
                    case "2":
                        if ((fz_qty - dz_qty) < collar_qtys)
                        {
                            ret.ErrMsg = "The number of broken needles cannot be greater than the number of issued needles";
                            ret.IsSuccess = false;
                            return ret;
                        }
                        sql = $@"insert into qcm_car_needle_c(m_id,collar_size,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_size}','{collar_qty}','{collar_date}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        DB.ExecuteNonQuery(sql);
                        item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_car_needle_c");
                        List<imginfo> img_url = JsonConvert.DeserializeObject<List<imginfo>>(jarr["guid_list"].ToString());
                        if (img_url.Count > 0)
                        {

                            if (!string.IsNullOrWhiteSpace(item_id))
                            {
                                foreach (imginfo item in img_url)
                                {
                                    sql = $"insert into qcm_car_needle_f(c_id,file_guid,createby,createdate,createtime) values('{item_id}','{item.guid}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";//添加对应的guid关联图片
                                    DB.ExecuteNonQuery(sql);
                                }

                            }

                        }
                        break;
                    //换针
                    case "3":
                        sql = $@"insert into qcm_car_needle_c(m_id,collar_size,collar_qty,collar_date,opa_type,remarks,createby,createdate,createtime) values('{m_id}','{collar_size}','{collar_qty}','{currDate:yyyy-MM-dd}','{opa_type}','{remarks}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                        DB.ExecuteNonQuery(sql);
                        break;
                }

               
                DB.Commit();
                ret.ErrMsg = "Added successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "add failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理删除、没有层级的删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_PDAdelete(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";

                string mId = DB.GetString($@"select M_ID from qcm_car_needle_c where id='{id}'");
                if (!string.IsNullOrEmpty(mId))
                {
                    sql = $@"UPDATE QCM_CAR_NEEDLE_M SET MODIFYBY='{userCode}',MODIFYDATE='{currDate.ToString("yyyy-MM-dd")}',MODIFYTIME='{currDate.ToString("HH:mm:ss")}' WHERE ID={mId}";
                    DB.ExecuteNonQuery(sql);
                }

                switch (opa_type)
                {
                    //断针的有图片
                    case "2":
                        sql = $@"select id from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}'";
                        DataTable c_id = DB.GetDataTable(sql);

                        if (c_id.Rows.Count > 0)
                        {
                            for (int i = 0; i < c_id.Rows.Count; i++)
                            {
                                sql = $"delete from qcm_car_needle_f where id='{c_id.Rows[i]["id"]}'";
                                DB.ExecuteNonQuery(sql);
                            }
                        }
                   break;
                }
                sql = $@"delete from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}' ";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 车针管理删除，带层级判断删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_PDAdeletepower(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string opa_type = jarr.ContainsKey("opa_type") ? jarr["opa_type"].ToString() : "";
                string m_id = DB.GetString($@"select m_id from qcm_car_needle_c where id='{id}'");
                DataTable c_id =new DataTable();
                switch (opa_type)
                {
                    //领用的
                    case "0":
                        sql = $@"select id from qcm_car_needle_c where m_id='{m_id}' and opa_type not in '{opa_type}'";
                        c_id = DB.GetDataTable(sql);
                        if (c_id.Rows.Count > 0)
                        {
                            ret.ErrMsg = "Deletion failed, because there is a record of send needle,broken needle, please delete the corresponding record before proceeding";
                            ret.IsSuccess = false;
                            return ret;
                        }
                        sql = $@"delete from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}' ";
                        DB.ExecuteNonQuery(sql);
                        break;
                    //发针
                    case "1":
                        sql = $@"select id from qcm_car_needle_c where m_id='{m_id}' and opa_type='2'";
                        c_id = DB.GetDataTable(sql);
                        if (c_id.Rows.Count > 0)
                        {
                            ret.ErrMsg = "Deletion failed, because there is a record of broken needle, please delete the corresponding record before proceeding";
                            ret.IsSuccess = false;
                            return ret;
                        }
                        sql = $@"delete from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}' ";
                        DB.ExecuteNonQuery(sql);
                        break;
                    //断针的
                    case "2":
                        sql = $@"select id from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}'";
                        c_id = DB.GetDataTable(sql);
                        if (c_id.Rows.Count > 0)
                        {
                            for (int i = 0; i < c_id.Rows.Count; i++)
                            {
                                sql = $"delete from qcm_car_needle_f where id='{c_id.Rows[i]["id"]}'";
                                DB.ExecuteNonQuery(sql);
                            }
                        }
                        sql = $@"delete from qcm_car_needle_c where id='{id}' and opa_type='{opa_type}' ";
                        DB.ExecuteNonQuery(sql);
                        break;
     
                }
              
                DB.Commit();
                ret.ErrMsg = "Operation deleted successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation delete failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 产区搜索
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getOrgList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string label = jarr.ContainsKey("label") ? jarr["label"].ToString() : "";
                string strwhere = "";
                if (!string.IsNullOrWhiteSpace(label))
                {
                    strwhere += $"org_name like '%{label}%'";
                }
                var sql = string.Empty;
                sql = $@"select org_code  as  value,org_name as label from base001m where 1=1 {strwhere}";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getProduction_lineList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string org_code = jarr.ContainsKey("org_code") ? jarr["org_code"].ToString() : "";
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//模糊搜索

                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = "";
                Dictionary<string, object> whereParamDic = new Dictionary<string, object>();
                //if (!string.IsNullOrWhiteSpace(org_code))
                //{
                //    strwhere += $" and VALUE like @org_code";
                //    whereParamDic.Add("org_code", $@"{org_code}");
                //}
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $" and ( label like @keyword or VALUE like @keyword )";

                    whereParamDic.Add("keyword", $@"%{keyword}%");
                }

                var sql = string.Empty;
                sql = $@"select*from (
SELECT
	PRODUCTION_LINE_CODE AS VALUE,
	PRODUCTION_LINE_NAME AS label
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	DEPARTMENT_CODE as VALUE,
	DEPARTMENT_NAME as label
FROM
	base005m)
WHERE
	1 = 1 {strwhere}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", whereParamDic);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getProduction_lineList2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string label = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(label))
                {
                    strwhere += $" and (VALUE like '%{label}%' or LABEL like '%{label}%')";
                }

                var sql = string.Empty;
                sql = $@"select*from (
SELECT
	PRODUCTION_LINE_CODE AS VALUE,
	PRODUCTION_LINE_NAME AS LABEL
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	DEPARTMENT_CODE as VALUE,
	DEPARTMENT_NAME as LABEL
FROM
	base005m)
WHERE
	1 = 1 {strwhere}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getProduction_lineList3(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string org_code = jarr.ContainsKey("org_code") ? jarr["org_code"].ToString() : "";
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//模糊搜索

                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = "";
                Dictionary<string, object> whereParamDic = new Dictionary<string, object>();
                //if (!string.IsNullOrWhiteSpace(org_code))
                //{
                //    strwhere += $" and VALUE like @org_code";
                //    whereParamDic.Add("org_code", $@"{org_code}");
                //}
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    strwhere += $" and production_line_name like @label";
                    whereParamDic.Add("label", $@"%{keyword}%");
                }

                var sql = string.Empty;
                sql = $@"select*from (
SELECT
	PRODUCTION_LINE_CODE AS production_line_code,
	PRODUCTION_LINE_NAME AS production_line_name,
    PLANT_AREA as plant_area
FROM
	BDM_PRODUCTION_LINE_M
UNION 
SELECT
	base005m.DEPARTMENT_CODE as production_line_code,
	base005m.DEPARTMENT_NAME as production_line_name,
  SJQDMS_ORGINFO.ORG as plant_area
FROM
	base005m
LEFT JOIN SJQDMS_ORGINFO ON base005m.UDF05 = SJQDMS_ORGINFO.CODE
)
WHERE
	1 = 1 {strwhere}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", whereParamDic);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 车针
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getNeedle_categoryList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string NEEDLE_CATEGORY_NO = jarr.ContainsKey("NEEDLE_CATEGORY_NO") ? jarr["NEEDLE_CATEGORY_NO"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(NEEDLE_CATEGORY_NO))
                {
                    strwhere += $@"and NEEDLE_CATEGORY_NO='{NEEDLE_CATEGORY_NO}'";
                }
                string sql = string.Empty;
                sql = $@"SELECT NEEDLE_CATEGORY_NO as value,NEEDLE_CATEGORY_NAME as label FROM BDM_NEEDLE_CATEGORY_M where 1=1 {strwhere}";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getNeedle_sizeList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string NEEDLE_CATEGORY_NO = jarr.ContainsKey("NEEDLE_CATEGORY_NO") ? jarr["NEEDLE_CATEGORY_NO"].ToString() : "";
                string strwhere = string.Empty;
                //if (!string.IsNullOrWhiteSpace(NEEDLE_CATEGORY_NO))
                //{
                //    strwhere += $@"and NEEDLE_CATEGORY_NO='{NEEDLE_CATEGORY_NO}'";
                //}
                string sql = string.Empty;
                sql = $@"select NEEDLE_SIZE as value from bdm_needle_size_m";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 扫描带出四个
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject getProduction_lineInfoByScan(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string production_line_code = jarr.ContainsKey("production_line_code") ? jarr["production_line_code"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(production_line_code))
                {
                    strwhere += $"and production_line_code='{production_line_code}'";
                }
                string sql = string.Empty;
                sql = $@"
SELECT * from (
SELECT
	org_code,
	org_name,
	production_line_code,
	production_line_name
FROM
	qcm_car_needle_m
UNION ALL
SELECT
 b.org_code,
 b.org_name,
 a.department_code,
 a.department_name
FROM
 base005m A
LEFT JOIN base001m b ON A .FACTORY_SAP = b.ORG_CODE
)tab where 1=1 {strwhere}";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_Needlemanagement_getimg(object OBJ)
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
              
                string id = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;

                sql = $@"SELECT
	A.ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	qcm_car_needle_f A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.file_guid = B.GUID
WHERE
	A.c_ID = '{id}'";
                DataTable dt = DB.GetDataTable(sql);
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
    }
}
