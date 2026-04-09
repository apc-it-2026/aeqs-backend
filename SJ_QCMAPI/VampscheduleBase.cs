using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class VampscheduleBase
    {
        /// <summary>
        /// 查询鞋面进度（针车）接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetVampscheduleList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string PUTINTO_DATE = jarr.ContainsKey("PUTINTO_DATE") ? jarr["PUTINTO_DATE"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string SE_ID = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;
                #region 条件
                string whereSql = string.Empty;
    //            PUTINTO_DATE = ''

    //AND SHOE_NO = ''

    //AND SE_ID = ''
                if (!string.IsNullOrEmpty(PUTINTO_DATE))
                {
                    whereSql += $@"and PUTINTO_DATE like'%{PUTINTO_DATE}%'";
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    whereSql += $@"and SHOE_NO like'%{SHOE_NO}%'";
                }
                if (!string.IsNullOrEmpty(SE_ID))
                {
                    whereSql += $@"and SE_ID like'%{SE_ID}%'";
                }
                #endregion

                sql = $@"SELECT
	ID,
	WEEK_TIMES,
	PUTINTO_DATE,
	WORK_HOURS,
	ORDER_DELIVERY_DATE,
	LEAD_TIME,
	LAST_NUMBER,
	TRIP_QTY,
	VAMP_TYPE,
	SHOE_NO,
	MODULE_NO,
	SE_ID,
	ITEM_NO,
	QTY 
FROM
	qcm_vamp_machine_m 
WHERE
	1 =1{whereSql}ORDER BY PUTINTO_DATE";

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
        /// 订单资料（PO）表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrderMasterList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                var sql = string.Empty;

                sql = $@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = DB.GetDataTable(sql);

                dic.Add("Data", dt);

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



        //添加订单资料接口
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddList(object OBJ)
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

                string WEEK_TIMES = jarr.ContainsKey("WEEK_TIMES") ? jarr["WEEK_TIMES"].ToString() : "";
                string WORK_HOURS = jarr.ContainsKey("WORK_HOURS") ? jarr["WORK_HOURS"].ToString() : "";
                string LEAD_TIME = jarr.ContainsKey("LEAD_TIME") ? jarr["LEAD_TIME"].ToString() : "";
                string TRIP_QTY = jarr.ContainsKey("TRIP_QTY") ? jarr["TRIP_QTY"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string SE_ID = jarr.ContainsKey("SE_ID") ? jarr["SE_ID"].ToString() : "";
                string QTY = jarr.ContainsKey("QTY") ? jarr["QTY"].ToString() : "";
                string LAST_NUMBER = jarr.ContainsKey("LAST_NUMBER") ? jarr["LAST_NUMBER"].ToString() : "";
                string VAMP_TYPE = jarr.ContainsKey("VAMP_TYPE") ? jarr["VAMP_TYPE"].ToString() : "";
                string MODULE_NO = jarr.ContainsKey("MODULE_NO") ? jarr["MODULE_NO"].ToString() : "";
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string PUTINTO_DATE = jarr.ContainsKey("PUTINTO_DATE") ? jarr["PUTINTO_DATE"].ToString() : "";
                string ORDER_DELIVERY_DATE = jarr.ContainsKey("ORDER_DELIVERY_DATE") ? jarr["ORDER_DELIVERY_DATE"].ToString() : "";

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = $@"INSERT INTO qcm_vamp_machine_m (
	WEEK_TIMES,
	PUTINTO_DATE,
	WORK_HOURS,
	ORDER_DELIVERY_DATE,
	LEAD_TIME,
	LAST_NUMBER,
	TRIP_QTY,
	VAMP_TYPE,
	SHOE_NO,
	MODULE_NO,
	SE_ID,
	ITEM_NO,
	QTY,
	CREATEBY,
	CREATEDATE,
	CREATETIME 
)
VALUES
	(
		'{WEEK_TIMES}',
		'{PUTINTO_DATE}',
		'{WORK_HOURS}',
		'{ORDER_DELIVERY_DATE}',
		'{LEAD_TIME}',
		'{LAST_NUMBER}',
		'{TRIP_QTY}',
		'{VAMP_TYPE}',
		'{SHOE_NO}',
		'{MODULE_NO}',
		'{SE_ID}',
		'{ITEM_NO}',
		'{QTY}',
		'{CreactUserId}',
		'{DateTime.Now.ToString("yyyy-MM-dd")}',
	'{DateTime.Now.ToString("HH:mm:ss")}' 
	)";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
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
