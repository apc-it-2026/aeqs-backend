using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class BrokenNeedle
    {

        /// <summary>
        /// 获取产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionLineList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 逻辑

                var sql = $@"SELECT PRODUCTIONLINE_NO,PRODUCTIONLINE_NAME FROM BDM_QUALITY_DEPARTMENT_D ";

                DataTable dt = DB.GetDataTable(sql);
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }

            return ret;

        }


        /// <summary>
        /// 根据PO号带出详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBrokenNeedleDetail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PO_Order = jarr.ContainsKey("PO_Order") ? jarr["PO_Order"].ToString() : "";//PO_Order号

                #endregion


                #region 逻辑
                //,CODE_NUMBER,LEFT_OR_RIGHT,PRODUCTIONLINE_NAME

                var SE_ID = DB.GetString($@"
SELECT
	SE_ID  -- 订单号
FROM
	BDM_SE_ORDER_MASTER
WHERE 
	MER_PO = '{PO_Order}'");

                var sql = $@"SELECT SHOE_NO,PROD_NO FROM BDM_SE_ORDER_ITEM WHERE SE_ID = '{SE_ID}'";

                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    ret.RetData1 = dt;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "查无数据";
                    ret.IsSuccess = false;
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
        /// 根据部门带出产线
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBrokenNeedleLineByDepartment(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string DEPARTMENT_NO = jarr.ContainsKey("DEPARTMENT_NO") ? jarr["DEPARTMENT_NO"].ToString() : "";//部门

                #endregion


                #region 逻辑
                //,CODE_NUMBER,LEFT_OR_RIGHT,PRODUCTIONLINE_NAME

                var sql = $@"SELECT PRODUCTIONLINE_NO,PRODUCTIONLINE_NAME FROM BDM_QUALITY_DEPARTMENT_D WHERE 1 = 1 ";


                sql += $@"AND DEPARTMENT_NO = '{DEPARTMENT_NO}'";


                DataTable dt = DB.GetDataTable(sql);

                if(dt.Rows.Count > 0)
                {
                    ret.RetData1 = dt;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "查无数据";
                    ret.IsSuccess = false;
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
        /// 断针提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddBrokenNeedle(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //CODE_NUMBER,LEFT_OR_RIGHT,PRODUCTIONLINE_NAME
                //SHOE_NO,PROD_NO

                string PO_ORDER = jarr.ContainsKey("PO_ORDER") ? jarr["PO_ORDER"].ToString() : "";//PO_Order号
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//ART
                string CODE_NUMBER = jarr.ContainsKey("CODE_NUMBER") ? jarr["CODE_NUMBER"].ToString() : "";//码数
                string LEFT_OR_RIGHT = jarr.ContainsKey("LEFT_OR_RIGHT") ? jarr["LEFT_OR_RIGHT"].ToString() : "";//R/L
                string PRODUCTIONLINE_NO = jarr.ContainsKey("PRODUCTIONLINE_NO") ? jarr["PRODUCTIONLINE_NO"].ToString() : "";//产线
                string PRODUCTIONLINE_NAME = jarr.ContainsKey("PRODUCTIONLINE_NAME") ? jarr["PRODUCTIONLINE_NAME"].ToString() : "";//产线

                string DEPARTMENT_NO = jarr.ContainsKey("DEPARTMENT_NO") ? jarr["DEPARTMENT_NO"].ToString() : "";//部门
                string DEPARTMENT_NAME = jarr.ContainsKey("DEPARTMENT_NAME") ? jarr["DEPARTMENT_NAME"].ToString() : "";//
                
                //string PRODUCTIONLINE_NAME = jarr.ContainsKey("PRODUCTIONLINE_NAME") ? jarr["PRODUCTIONLINE_NAME"].ToString() : "";

                #endregion
                DB.Open();
                DB.BeginTransaction();

                #region 接口验证

                if (string.IsNullOrEmpty(PO_ORDER.Trim(' ')))
                    throw new Exception("接口参数【PO_ORDER】不能为空,请检查！");

                if (string.IsNullOrEmpty(SHOE_NO.Trim(' ')))
                    throw new Exception("接口参数【SHOE_NO】不能为空,请检查！");
                #endregion


                #region 逻辑

                string CREATEBY = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");

//                string PRODUCTIONLINE_NO = DB.GetString($@"SELECT PRODUCTIONLINE_NO FROM BDM_QUALITY_DEPARTMENT_D WHERE PRODUCTIONLINE_NAME = '{PRODUCTIONLINE_NAME}'");

//                string DEPARTMENT_NO = DB.GetString($@"
//SELECT
//	BDM_QUALITY_DEPARTMENT_M.DEPARTMENT_NO
//FROM
//	BDM_QUALITY_DEPARTMENT_D
//LEFT JOIN BDM_QUALITY_DEPARTMENT_M ON BDM_QUALITY_DEPARTMENT_D.DEPARTMENT_NO = BDM_QUALITY_DEPARTMENT_M.DEPARTMENT_NO 
//WHERE BDM_QUALITY_DEPARTMENT_D.PRODUCTIONLINE_NO = '{PRODUCTIONLINE_NO}'");


//                string DEPARTMENT_NAME = DB.GetString($@"
//SELECT
//	BDM_QUALITY_DEPARTMENT_M.DEPARTMENT_NAME
//FROM
//	BDM_QUALITY_DEPARTMENT_D
//LEFT JOIN BDM_QUALITY_DEPARTMENT_M ON BDM_QUALITY_DEPARTMENT_D.DEPARTMENT_NO = BDM_QUALITY_DEPARTMENT_M.DEPARTMENT_NO 
//WHERE BDM_QUALITY_DEPARTMENT_D.PRODUCTIONLINE_NO = '{PRODUCTIONLINE_NO}'");

                var sql = $@"
INSERT INTO QCM_BROKEN_NEEDLE_M (
	PO_ORDER,
	SHOE_NO,
	PROD_NO,
	CODE_NUMBER,
	LEFT_OR_RIGHT,
	DEPARTMENT_NO,
	DEPARTMENT_NAME,
	PRODUCTIONLINE_NO,
	PRODUCTIONLINE_NAME,
CREATEBY,
CREATEDATE,
CREATETIME
)VALUES(
'{PO_ORDER}',
'{SHOE_NO}',
'{PROD_NO}',
'{CODE_NUMBER}',
'{LEFT_OR_RIGHT}',
'{DEPARTMENT_NO}',
'{DEPARTMENT_NAME}',
'{PRODUCTIONLINE_NO}',
'{PRODUCTIONLINE_NAME}',
'{CREATEBY}',
'{createdate}',
'{createtime}'
)";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                #endregion
                ret.ErrMsg = "提交成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "提交失败！" + ex.Message;

            }
            finally
            {
                DB.Close();
            }

            return ret;

        }
    }
}
