using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 不良退货接口
    /// </summary>
    public class BadReturnBase
    {
        /// <summary>
        /// 不良退货列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadReturnList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string RETURN_DATE = jarr.ContainsKey("RETURN_DATE") ? jarr["RETURN_DATE"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(RETURN_DATE))
                {
                    whereSql += $@"and RETURN_DATE='" + RETURN_DATE + "'";
                }
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    whereSql += $@"and PROD_NO like '%" + PROD_NO + "%'";
                }
                if (!string.IsNullOrEmpty(SHOE_NO))
                {
                    whereSql += $@"and SHOE_NO like '%" + SHOE_NO + "%'";
                }

                sql = $@"SELECT
	ID,
	RETURN_NO,
	RETURN_DATE,
	PLANT_AREA,
	ORDER_QTY,
	TURNOVER_QTY,
	B_QTY,
	RETURN_FREQUENCY,
	AFFECT_HOURS,
	SHOE_NO,
	PROD_NO 
FROM
	qcm_bad_return_m 
WHERE
	1 = 1 { whereSql}";

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadReturnDetailList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string RETURN_NO = jarr.ContainsKey("RETURN_NO") ? jarr["RETURN_NO"].ToString() : "";

                string whereSql = string.Empty;

                Dictionary<string, object> dic = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(RETURN_NO))
                {
                    whereSql += $@"and RETURN_NO like'%" + RETURN_NO + "%'";
                }

               string sql = $@"SELECT
	ID,
	RETURN_NO,
	BAD_REASON,
	TREATMENT_METHOD,
	TREATMENT_RESULT,
	F_GUID 
FROM
	qcm_bad_return_d
WHERE
    1=1
    { whereSql}";

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


        //录入退货单信息接口
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

                string RETURN_NO = "QA" + DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(RETURN_NO) from qcm_bad_return_m where RETURN_NO like '{RETURN_NO}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(RETURN_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    RETURN_NO = RETURN_NO + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    RETURN_NO = RETURN_NO + "00001";
                }

                string RETURN_DATE = jarr.ContainsKey("RETURN_DATE") ? jarr["RETURN_DATE"].ToString() : "";
                string PLANT_AREA = jarr.ContainsKey("PLANT_AREA") ? jarr["PLANT_AREA"].ToString() : "";
                string ORDER_QTY = jarr.ContainsKey("ORDER_QTY") ? jarr["ORDER_QTY"].ToString() : "";
                string TURNOVER_QTY = jarr.ContainsKey("TURNOVER_QTY") ? jarr["TURNOVER_QTY"].ToString() : "";
                string B_QTY = jarr.ContainsKey("B_QTY") ? jarr["B_QTY"].ToString() : "";
                string RETURN_FREQUENCY = jarr.ContainsKey("RETURN_FREQUENCY") ? jarr["RETURN_FREQUENCY"].ToString() : "";
                string AFFECT_HOURS = jarr.ContainsKey("AFFECT_HOURS") ? jarr["AFFECT_HOURS"].ToString() : "";
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string AddSql = $@"	insert into qcm_bad_return_m(RETURN_NO,RETURN_DATE,PLANT_AREA,ORDER_QTY,TURNOVER_QTY,B_QTY,RETURN_FREQUENCY,AFFECT_HOURS,SHOE_NO,PROD_NO,CREATEBY,CREATEDATE,CREATETIME)VALUES('{RETURN_NO}','{RETURN_DATE}','{PLANT_AREA}','{ORDER_QTY}','{TURNOVER_QTY}','{B_QTY}','{RETURN_FREQUENCY}','{AFFECT_HOURS}','{SHOE_NO}','{PROD_NO}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";

                DB.ExecuteNonQuery(AddSql);
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

        /// <summary>GetFileViewS
        /// 不良退货图片接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileViewS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string RETURN_NO = jarr.ContainsKey("RETURN_NO") ? jarr["RETURN_NO"].ToString() : "";
                string F_GUID = jarr.ContainsKey("F_GUID") ? jarr["F_GUID"].ToString() : "";
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";



                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(RETURN_NO))
                {
                    whereSql += $@"and RETURN_DATE='" + RETURN_NO + "'";
                }
                if (!string.IsNullOrEmpty(F_GUID))
                {
                    whereSql += $@"and F_GUID='" + F_GUID + "'";
                }
                if (TYPE=="1")
                {
                    whereSql += $@"and TYPE='" + TYPE + "'";
                }
                if (TYPE == "2")
                {
                    whereSql += $@"and TYPE='" + TYPE + "'";
                }

                string sql = $@"select ID,RETURN_NO,IMG_NAME,IMG_URL,F_GUID from qcm_bad_return_image where RETURN_NO='{RETURN_NO}' and TYPE='{TYPE}' and F_GUID='{F_GUID}'";

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
    }
}
