using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class SatraLeatherEvaluationBase
    {
        /// <summary>
        /// 获取SATRA皮料评估列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSatraList(object OBJ)
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
                string ITEM_NAME = jarr.ContainsKey("ITEM_NAME") ? jarr["ITEM_NAME"].ToString() : "";
                string PAINT_DATE = jarr.ContainsKey("PAINT_DATE") ? jarr["PAINT_DATE"].ToString() : "";
                string ITEM_TYPE_NAME = jarr.ContainsKey("ITEM_TYPE_NAME") ? jarr["ITEM_TYPE_NAME"].ToString() : "";
                string vend_name = jarr.ContainsKey("vend_name") ? jarr["vend_name"].ToString() : "";
                string CREATEBY = jarr.ContainsKey("CREATEBY") ? jarr["CREATEBY"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(ITEM_NO))
                {
                    whereSql += $@"and item_no like'%" + ITEM_NO + "%'";
                }
                if (!string.IsNullOrEmpty(ITEM_NAME))
                {
                    whereSql += $@"and item_name like'%" + ITEM_NAME + "%'";
                }
                if (!string.IsNullOrEmpty(PAINT_DATE))
                {
                    whereSql += $@"and paint_date like'%" + PAINT_DATE + "%'";
                }
                if (!string.IsNullOrEmpty(vend_name))
                {
                    whereSql += $@"and vend_name like'%" + vend_name + "%'";
                }
                if (!string.IsNullOrEmpty(CREATEBY))
                {
                    whereSql += $@"and createby like'%" + CREATEBY + "%'";
                }
                if (!string.IsNullOrEmpty(ITEM_TYPE_NAME))
                {
                    whereSql += $@"and item_no in(select item_no from bdm_rd_item where ITEM_TYPE in(select item_type_no from BDM_RD_ITEMTYPE where ITEM_TYPE_NAME LIKE'%" + ITEM_TYPE_NAME + "%'))";
                }



                sql = $@"SELECT
	m.ID,
	m.PAINT_NO,
	m.ITEM_NO,
	m.ITEM_NAME,
	m.vend_no,
	m.vend_name,
	m.PAINT_DATE,
	( SELECT sum( QTY ) FROM QCM_RM_PAINTEDSKIN_M WHERE PAINT_NO = m.PAINT_NO ) AS QTY,
	m.SHOE_NOS,
	m.PROD_NOS,
	m.part_nos,
	m.createby,
	m.AVERAGE_USE_RATE,
	m.DIFFERENCE_COEFFICIENT,
	m.ASSESSMENT,
	((SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL NOT IN ('6','6B'))+(SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL IN ('6B','6') AND TYPE='1')) AS ACTUAL_AREA,
	( SELECT ITEM_TYPE_NAME FROM BDM_RD_ITEMTYPE WHERE ITEM_TYPE_NO IN ( SELECT item_type FROM bdm_rd_item WHERE ITEM_NO = m.ITEM_NO ) ) AS ITEM_TYPE_NAME,
	(
		( ((SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL NOT IN ('6','6B'))+(SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL IN ('6B','6') AND TYPE='1')) ) / ( SELECT sum( QTY ) FROM QCM_RM_PAINTEDSKIN_M WHERE PAINT_NO = m.PAINT_NO ) * 100 
	) AS usage_rate,
	d.chk_no
FROM
	QCM_RM_PAINTEDSKIN_M m LEFT JOIN QCM_RM_PAINTEDSKIN_D d on d.paint_no=m.paint_no
WHERE
	1 =1  {whereSql}";

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
        /// 根据Id获取SATRA皮料评估列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSatraListById(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";

                string sql = $@"SELECT
	m.ID,
	m.PAINT_NO,
	m.ITEM_NO,
	m.ITEM_NAME,
	m.vend_no,
	m.vend_name,
	m.PAINT_DATE,
	( SELECT sum( QTY ) FROM QCM_RM_PAINTEDSKIN_M WHERE PAINT_NO = m.PAINT_NO ) AS QTY,
	m.SHOE_NOS,
	m.PROD_NOS,
	m.part_nos,
	m.createby,
	m.AVERAGE_USE_RATE,
	m.DIFFERENCE_COEFFICIENT,
	m.ASSESSMENT,
	((SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL NOT IN ('6','6B'))+(SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL IN ('6B','6') AND TYPE='1')) AS ACTUAL_AREA,
	( SELECT ITEM_TYPE_NAME FROM BDM_RD_ITEMTYPE WHERE ITEM_TYPE_NO IN ( SELECT item_type FROM bdm_rd_item WHERE ITEM_NO = m.ITEM_NO ) ) AS ITEM_TYPE_NAME,
	(
		( ((SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL NOT IN ('6','6B'))+(SELECT SUM(ACTUAL_AREA) FROM QCM_RM_PAINTEDSKIN_ITEM WHERE PAINT_NO=m.PAINT_NO  AND PAINT_LEVEL IN ('6B','6') AND TYPE='1')) ) / ( SELECT sum( QTY ) FROM QCM_RM_PAINTEDSKIN_M WHERE PAINT_NO = m.PAINT_NO ) * 100 
	) AS usage_rate,
	d.chk_no
FROM
	QCM_RM_PAINTEDSKIN_M m LEFT JOIN QCM_RM_PAINTEDSKIN_D d on d.paint_no=m.paint_no
WHERE
	1 =1 AND m.PAINT_NO= '{PAINT_NO}'";

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


        /// <summary>
        /// SATRA皮料评估列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSatraList2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PAINT_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(PAINT_NO))
                {
                    whereSql += $@"and item_no like'%" + PAINT_NO + "%'";
                }



                sql = $@"SELECT
	ACTUAL_AREA,
	MULTIPLE 
FROM
	qcm_rm_paintedskin_item 
WHERE
	AND PAINT_LEVEL = '6'{whereSql}";

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
        /// 获取供应商面积、实际面积总和接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupllyAndActualSUM(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";

                string sql = $@"select COUNT(*) as COUNT,sum(SUPPLIER_AREA) as SUPPLIER_AREA,sum(ACTUAL_AREA) as ACTUAL_AREA from qcm_rm_paintedskin_item where PAINT_NO='{PAINT_NO}'";


                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 分组获取供应商、实际面积接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupllyAndActual(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";

                string sql = $@"SELECT
	ID,
	PAINT_LEVEL,
	SUPPLIER_AREA,
	ACTUAL_AREA 
FROM
	qcm_rm_paintedskin_item 
WHERE
	PAINT_NO = '{PAINT_NO}' 
	AND PAINT_LEVEL NOT IN ( '6', '6B' ) UNION
SELECT
	ID,
	PAINT_LEVEL,
	SUPPLIER_AREA,
	ACTUAL_AREA 
FROM
	qcm_rm_paintedskin_item 
WHERE
	PAINT_NO = '{PAINT_NO}' 
	AND PAINT_LEVEL IN ( '6', '6B' ) 
	AND TYPE = '1' 
ORDER BY
	PAINT_LEVEL";


                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupllyAndActual2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"SELECT
	PAINT_LEVEL,
	ACTUAL_AREA 
FROM
	(
	SELECT
		* 
	FROM
		(
		SELECT
			PAINT_LEVEL,
			sum( ACTUAL_AREA ) AS ACTUAL_AREA 
		FROM
			qcm_rm_paintedskin_item 
		WHERE
			PAINT_NO = '{PAINT_NO}' 
			AND PAINT_LEVEL NOT IN ( '6', '6B' ) 
		GROUP BY
			PAINT_LEVEL 
		ORDER BY
			PAINT_LEVEL ASC 
		) m UNION
	SELECT
		PAINT_LEVEL,
		SUM( ACTUAL_AREA ) AS ACTUAL_AREA 
	FROM
		qcm_rm_paintedskin_item 
	WHERE
		PAINT_LEVEL IN ( '6', '6B' ) 
		AND TYPE = '1' 
		AND PAINT_NO = '{PAINT_NO}' 
	GROUP BY
		PAINT_LEVEL 
	) m";

                string sql1 = $@"SELECT
	PAINT_LEVEL,
	SUM( ACTUAL_AREA ) AS ACTUAL_AREA,
	SUM( MULTIPLE ) AS MULTIPLE 
FROM
	qcm_rm_paintedskin_item 
WHERE
	PAINT_NO = '{PAINT_NO}' 
	AND PAINT_LEVEL = '6' 
	AND TYPE = '1' 
GROUP BY
	PAINT_LEVEL";

                string sql2 = $@"SELECT
	PAINT_LEVEL,
	SUM( ACTUAL_AREA ) AS ACTUAL_AREA,
	SUM( MULTIPLE ) AS MULTIPLE 
FROM
	qcm_rm_paintedskin_item 
WHERE
	PAINT_NO = '{PAINT_NO}' 
	AND PAINT_LEVEL = '6B' 
	AND TYPE = '1' 
GROUP BY
	PAINT_LEVEL";



                DataTable dt = DB.GetDataTable(sql);
                DataTable dt1 = DB.GetDataTable(sql1);
                DataTable dt2 = DB.GetDataTable(sql2);
                dic.Add("Data", dt);
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);

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

        //修改视图的信息
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateList(object OBJ)
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
                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";
                string PURCHASE_COEFFICIENT = jarr.ContainsKey("PURCHASE_COEFFICIENT") ? jarr["PURCHASE_COEFFICIENT"].ToString() : "";
                string AVERAGE_USE_RATE = jarr.ContainsKey("AVERAGE_USE_RATE") ? jarr["AVERAGE_USE_RATE"].ToString() : "";
                string DIFFERENCE_COEFFICIENT = jarr.ContainsKey("DIFFERENCE_COEFFICIENT") ? jarr["DIFFERENCE_COEFFICIENT"].ToString() : "";
                string ASSESSMENT = jarr.ContainsKey("ASSESSMENT") ? jarr["ASSESSMENT"].ToString() : "";

                string level = jarr.ContainsKey("level") ? jarr["level"].ToString() : "";
                string level2 = jarr.ContainsKey("level2") ? jarr["level2"].ToString() : "";
                string VI = jarr.ContainsKey("VI") ? jarr["VI"].ToString() : "";
                string VI2 = jarr.ContainsKey("VI2") ? jarr["VI2"].ToString() : "";
                string M = jarr.ContainsKey("M") ? jarr["M"].ToString() : "";
                string M2 = jarr.ContainsKey("M2") ? jarr["M2"].ToString() : "";

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = $@"UPDATE QCM_RM_PAINTEDSKIN_M 
SET PURCHASE_COEFFICIENT = '{PURCHASE_COEFFICIENT}',
AVERAGE_USE_RATE = '{AVERAGE_USE_RATE}',
DIFFERENCE_COEFFICIENT = '{DIFFERENCE_COEFFICIENT}',
ASSESSMENT = '{ASSESSMENT}',
MODIFYBY='{CreactUserId}',
MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',
MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}'
WHERE
	PAINT_NO = '{PAINT_NO}'";

                string sql2 = $@"select PAINT_NO from qcm_rm_paintedskin_item where PAINT_NO='{PAINT_NO}' AND PAINT_LEVEL='{level}' AND TYPE='1'";
                string sql3 = $@"select PAINT_NO from qcm_rm_paintedskin_item where PAINT_NO='{PAINT_NO}' AND PAINT_LEVEL='{level2}' AND TYPE='1'";

                DataTable dt = DB.GetDataTable(sql2);
                DataTable dt2 = DB.GetDataTable(sql3);
                string updateSql = string.Empty;
                string updateSql2 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(VI) && !string.IsNullOrEmpty(M))
                    {
                        updateSql = $@"update qcm_rm_paintedskin_item set SUPPLIER_AREA='0',ACTUAL_AREA='{VI}',MULTIPLE='{M}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}' WHERE PAINT_NO='{PAINT_NO}' AND PAINT_LEVEL='{level}' AND TYPE='1'";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(VI) && !string.IsNullOrEmpty(M))
                    {
                        updateSql = $@"	insert into qcm_rm_paintedskin_item(PAINT_NO,PAINT_LEVEL,SUPPLIER_AREA,ACTUAL_AREA,CREATEBY,CREATEDATE,CREATETIME,TYPE,MULTIPLE) VALUES('{PAINT_NO}','{level}','0','{VI}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','1','{M}')";
                    }

                }
                if (dt2.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(VI2) && !string.IsNullOrEmpty(M2))
                    {
                        updateSql2 = $@"update qcm_rm_paintedskin_item set SUPPLIER_AREA='0',ACTUAL_AREA='{VI2}',MULTIPLE='{M2}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}' WHERE PAINT_NO='{PAINT_NO}' AND PAINT_LEVEL='{level2}' AND TYPE='1'";
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(VI2) && !string.IsNullOrEmpty(M2))
                    {
                        updateSql2 = $@"insert into qcm_rm_paintedskin_item(PAINT_NO,PAINT_LEVEL,SUPPLIER_AREA,ACTUAL_AREA,CREATEBY,CREATEDATE,CREATETIME,TYPE,MULTIPLE) VALUES('{PAINT_NO}','{level2}','0','{VI2}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}','1','{M2}')";
                    }
                }


                DB.ExecuteNonQuery(sql);
                DB.ExecuteNonQuery(updateSql);
                DB.ExecuteNonQuery(updateSql2);
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
