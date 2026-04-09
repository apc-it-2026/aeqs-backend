using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Skill_matrix_Report
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPlantData(object OBJ)
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
                string sql = $@"select distinct UDF05 
                                from Base005m where FACTORY_SAP='5001'";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProcessData(object OBJ)
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
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@" AND UDF05 = '{Plant}'";
                }
                string sql = $@"select distinct UDF01 from Base005m where FACTORY_SAP='5001'{where}";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLineData(object OBJ)
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
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@" AND UDF05 = '{Plant}'";
                }
                if (!string.IsNullOrEmpty(Process))
                {
                    where += $@" AND UDF01 = '{Process}'";
                }

                string sql = $@"select distinct DEP_SAP from Base005m where FACTORY_SAP='5001'{where}";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetskilltypeData(object OBJ)
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
                string Line = jarr.ContainsKey("Line") ? jarr["Line"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Line))
                {
                    where += $@" AND DEPT_NO = '{Line}'";
                }
                string sql = $@"SELECT distinct D.SKILL_NAME
                                FROM T_OA_EMPMAIN C
                                INNER JOIN T_TSM_EMP_SKILL_M D
                                ON C.EMP_NO = D.EMP_NO
                                WHERE 1=1{where}";
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

        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetskillReportDetails(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        DB.Open();
        //        DB.BeginTransaction();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
        //        string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
        //        string Line = jarr.ContainsKey("Line") ? jarr["Line"].ToString() : "";
        //        string Skill_type = jarr.ContainsKey("Skill_type") ? jarr["Skill_type"].ToString() : "";
        //        string where = string.Empty;
        //        if (!string.IsNullOrEmpty(Plant))
        //        {
        //            where += $@" AND T.UDF05 = '{Plant}'";
        //        }

        //        if (!string.IsNullOrEmpty(Process))
        //        {
        //            where += $@" AND T.UDF01 = '{Process}'";
        //        }
        //        if (!string.IsNullOrEmpty(Line))
        //        {
        //            where += $@" AND T.HR_DEPT_NO = '{Line}'";
        //        }
        //        if (!string.IsNullOrEmpty(Skill_type))
        //        {
        //            where += $@" AND S.SKILL_NAME = '{Skill_type}'";
        //        }
        //        string sql = $@"{where}";

        //        DataTable dt = DB.GetDataTable(sql);
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        dic.Add("Data", dt);

        //        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        DB.Rollback();
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    finally
        //    {
        //        DB.Close();
        //    }
        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetskillReportDetails(object OBJ)
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
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string Line = jarr.ContainsKey("Line") ? jarr["Line"].ToString() : "";
                string Skill_type = jarr.ContainsKey("Skill_type") ? jarr["Skill_type"].ToString() : "";

                // Build dynamic filters
                string filter = "";
                if (!string.IsNullOrEmpty(Plant))
                    filter += $" AND T.UDF05 = '{Plant}'";
                if (!string.IsNullOrEmpty(Process))
                    filter += $" AND T.UDF01 = '{Process}'";
                if (!string.IsNullOrEmpty(Line))
                    filter += $" AND T.HR_DEPT_NO = '{Line}'";
                if (!string.IsNullOrEmpty(Skill_type))
                    filter += $" AND S.SKILL_NAME = '{Skill_type}'";

                // Complete SQL query
                string sql = $@"
WITH TEMP AS (
    SELECT DISTINCT 
        SS.FACTORY_SAP, SS.UDF05, SS.DEPARTMENT_CODE AS MES_DEPT_NO,
        SS.UDF01,
        D.DEPT_NO AS HR_DEPT_NO,
        D.FACT_MANS AS HR_MP
    FROM BASE005M SS
    INNER JOIN dp_dept_auth@APCHRDB D
        ON SS.DEP_SAP = D.DEPT_NO
    LEFT JOIN dp_dept@APCHRDB F
        ON F.DEPT_NO = D.DEPT_NO 
       AND D.org_id = 100
    WHERE D.AUTH_NO = 'G01'
),
TEMP2 AS (
    SELECT
        A.DEPT_NO AS OA_DEPT_NO,
        COUNT(*) AS ACTUAL_MP
    FROM ep_main@apchrdb A 
    WHERE A.status = 1 
      AND A.org_id = 100 
      AND A.work_no = 'G01'
    GROUP BY A.DEPT_NO
),
SKILL AS (
    SELECT 
        C.DEPT_NO AS OA_DEPT_NO,
        D.SKILL_NAME,
        COUNT(D.SKILL_NAME) AS SKILL_COUNT
    FROM ep_main@apchrdb C
    INNER JOIN T_TSM_EMP_SKILL_M D
        ON C.EMP_NO = D.EMP_NO
    WHERE C.DEPT_NO LIKE 'AP%' 
      AND C.status = 1
    GROUP BY C.DEPT_NO, D.SKILL_NAME
)
SELECT 
     T.FACTORY_SAP AS FACTORY_ID,
     T.UDF05 AS PLANT,
     CMP.MES_DEPARTMENTCODE AS MES_DEPT_NO,
     T.HR_DEPT_NO,
     T.UDF01 AS PROCESS,
     T.HR_MP,
     NVL(T2.ACTUAL_MP, 0) AS ACTUAL_MP,
     NVL(S.SKILL_COUNT, 0) AS SKILL_COUNT,
     CASE 
         WHEN (NVL(S.SKILL_COUNT, 0) - NVL(T2.ACTUAL_MP, 0)) < 0 THEN 
             '-' || TO_CHAR(ABS(NVL(S.SKILL_COUNT, 0) - NVL(T2.ACTUAL_MP, 0)))
         ELSE 
             TO_CHAR(NVL(S.SKILL_COUNT, 0) - NVL(T2.ACTUAL_MP, 0))
     END AS DIFFERENCE,
     S.SKILL_NAME
FROM T_OA_MES_DEPARTMENT_COMPARE CMP
LEFT JOIN TEMP T
    ON CMP.MES_DEPARTMENTCODE = T.MES_DEPT_NO
LEFT JOIN TEMP2 T2
    ON CMP.OA_DEPARTMENTCODE = T2.OA_DEPT_NO
LEFT JOIN SKILL S
    ON CMP.OA_DEPARTMENTCODE = S.OA_DEPT_NO
WHERE CMP.MES_DEPARTMENTCODE LIKE '5001AP%' {filter}
ORDER BY CMP.MES_DEPARTMENTCODE, S.SKILL_NAME";

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

    }
}
