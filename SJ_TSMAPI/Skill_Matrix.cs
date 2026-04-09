using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Skill_Matrix
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSkill_Matrix(object OBJ)
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
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : ""; 
                string Process_Names = jarr.ContainsKey("Process_Names") ? jarr["Process_Names"].ToString() : "";
                string Production_Plant = jarr.ContainsKey("Production_Plant") ? jarr["Production_Plant"].ToString() : "";
                string Month = jarr.ContainsKey("Month") ? jarr["Month"].ToString() : "";
                string Model = jarr.ContainsKey("Model") ? jarr["Model"].ToString() : "";
                string where1 = string.Empty;
                string where2 = string.Empty;
                string where3 = string.Empty;
                string where7 = string.Empty;
                string model_skills = string.Empty;
                
                if (!string.IsNullOrWhiteSpace(ProductionLine))
                {
                    where2 += $@"AND a.dept_name='{ProductionLine}'";
                }
                if (!string.IsNullOrWhiteSpace(Production_Plant))
                {
                    where2 += $@"AND a.udf05='{Production_Plant}'";
                }

                //Start Department Filter
                if (Department == "Cutting")
                {
                    where3 = $@"AND a.udf01 like '%C%'";
                }
                else if (Department == "Stitching")
                {
                    where3 = $@"AND a.udf01 like '%S%'";
                }
                else if (Department == "Assembly")
                {
                    where3 = $@"AND a.udf01 like '%L%'";
                }
                else if (Department == "Packing")
                {
                    where3 = $@"AND a.udf01 like '%A%'";
                }
                else if (Department == "Stock fitting")
                {
                    where7 = $@"AND a.udf01 LIKE '%T%' 
               AND a.DEPARTMENT_NAME LIKE '%Stockfitting%' 
               AND a.DEPARTMENT_NAME NOT LIKE '%Maxking Stockfitting%'";
                }
                else if (Department == "Spray Painting")
                {
                    where7 = $@"AND a.udf01 LIKE '%SPD%'";
                }
                //End Department Filter

                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where2 += $@"AND a.emp_no='{Barcode}'";
                }
                if (!string.IsNullOrWhiteSpace(Process))
                {
                    where1 += $@"AND b.PROCESS_TYPE = '{Process}'";
                }
                if (!string.IsNullOrWhiteSpace(Process_Names))
                {
                    where1 += $@"AND b.name in ({Process_Names})";
                }


                string where = string.Empty;
                DataTable dt = new DataTable();
                int count = 0;
                if (!string.IsNullOrWhiteSpace(Model))
                {
                    string sql_skillcode = $@"SELECT distinct b.name
  FROM T_TSM_MODELWISE_PROCESS A INNER JOIN T_TSM_PROCESSLIST B ON A.SKILL_CODE=B.SKILL_CODE
 WHERE a.MODEL_NAME = '{Model}'{where1}
";
                    dt = DB.GetDataTable(sql_skillcode);
                    count = dt.Rows.Count;

                    foreach (DataRow item in dt.Rows)
                    {
                        where += "'" + item["name"].ToString().Replace(" ", "") + "'" + "AS " + '"' + item["name"].ToString().Replace(" ", "") + '"' + ",";
                    }
                }
                else
                {
                    dt = DB.GetDataTable($@" select name from t_tsm_processlist b where 1=1 {where1}");
                    count = dt.Rows.Count;
                  
                    foreach (DataRow item in dt.Rows)
                    {
                        where += "'" + item["name"].ToString().Replace(" ", "") + "'" + "AS " + '"' + item["name"].ToString().Replace(" ", "") + '"' + ",";
                    }
                }

                where = where.TrimEnd(',');
                string where4 = string.Empty;
                string where5 = string.Empty;
                string where6 = string.Empty;

                foreach (DataRow item in dt.Rows)
                { 
                    where4 += "COALESCE(("+'"'+ item["name"].ToString().Replace(" ", "") + '"'+"), 0) AS "+ item["name"].ToString().Replace(" ", "") + ",";

                }

                foreach (DataRow item in dt.Rows)
                { 
                    where5 += "COALESCE((" + '"' + item["name"].ToString().Replace(" ", "") + '"' + "), 0)+";
                }
                where5 = where5.TrimEnd('+');

                foreach (DataRow item in dt.Rows)

                { 
                    where6 += "round(SUM(" + item["name"].ToString().Replace(" ", "") + ")/(count(*) * 4) * 100, 2) AS "+ item["name"].ToString().Replace(" ", "") + ","; 
                }
                #region oldquery before adding Month in Skill Matrix
                //string sql = $@"with tmp as (select a.*,round(a.Actual_Skill_Achievement/a.Ideal_Skill_Achievement*100,2) as Skill_Index
                //                from (
                //                SELECT 
                //                    emp_no,
                //                    emp_name,
                //                    dept_name, 
                //                    {where4} 
                //                    {where5} as Actual_Skill_Achievement,
                //                    {count} * 4 as Ideal_Skill_Achievement from(select a.emp_no,
                //       a.emp_name,
                //       a.dept_name,
                //       b.skill_name,
                //       b.skill_score from (select
                //c.emp_no, c.emp_name, a.department_code as dept_name,a.udf05,udf01 
                //from base005m a, t_oa_mes_department_compare b, t_oa_empmain c where a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE and b.OA_DEPARTMENTCODE = c.DEPT_NO 
                //and c.STATUS = '1' and a.Factory_Sap is not null and c.work_name='Worker' {where7})a 
                // left
                //  join t_tsm_emp_skill_m b
                //                on a.emp_no = b.emp_no where 1 = 1 {where2} {where3} 
                //                GROUP BY
                //                     a.emp_no,
                //                       a.emp_name,
                //                       a.dept_name,
                //                        b.skill_name,
                //                       b.skill_score)
                //                       PIVOT(sum(skill_score)
                //                   FOR skill_name IN({ where})))a 
                //),
                //Total_Summary AS (
                //    SELECT 
                //        NULL AS emp_no, 
                //        NULL AS emp_name, 
                //        'Summary' AS dept_name,  
                //         {where6}
                //        SUM(Actual_Skill_Achievement) AS Actual_Skill_Achievement,
                //        SUM(Ideal_Skill_Achievement) AS Ideal_Skill_Achievement,
                //        round(AVG(Skill_Index),2) AS Skill_Index
                //    FROM 
                //        tmp
                //),orderd_summary as(  
                // select * from Total_Summary 
                // union all
                // select * from tmp)
                //select * from orderd_summary ORDER BY CASE WHEN dept_name = 'Summary' THEN 2 ELSE 1 END, emp_no NULLS LAST";
                #endregion
                #region Query After Adding Month in Skill Matrix on 20250107
                string where8 = string.Empty;
                if (!string.IsNullOrWhiteSpace(Month))
                {
                    where8 += $@" and to_char(to_date(b.createddate, 'yyyy/MM/dd'),
                                                 'yyyy/MM') <='{Month}'";
                }
                string sql = $@"with tmp as (select a.*,round(a.Actual_Skill_Achievement/a.Ideal_Skill_Achievement*100,2) as Skill_Index
                                from (
                                SELECT 
                                    emp_no,
                                    emp_name,
                                    dept_name, 
                                    {where4} 
                                    {where5} as Actual_Skill_Achievement,
                                    {count} * 4 as Ideal_Skill_Achievement from(SELECT emp_no, emp_name, dept_name, skill_name, skill_score from
 (SELECT a.emp_no,
         a.emp_name,
         a.dept_name,
         b.skill_name,
         b.skill_score,
         b.createddate,
         ROW_NUMBER() OVER(PARTITION BY a.emp_no, b.skill_name ORDER BY TO_DATE(b.createddate, 'yyyy/MM/dd') DESC) AS rn
    FROM (SELECT c.emp_no,
                 c.emp_name,
                 a.department_code AS dept_name,
                 a.udf05,
                 a.udf01
            FROM base005m a
            JOIN t_oa_mes_department_compare b
              ON a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
            JOIN t_oa_empmain c
              ON b.OA_DEPARTMENTCODE = c.DEPT_NO
           WHERE c.STATUS = '1'
            -- AND a.Factory_Sap IS NOT NULL
             AND c.work_name = 'Worker' {where7})a 
                 left
                  join t_tsm_emp_skill_d b
                                on a.emp_no = b.emp_no where 1 = 1 {where2} {where3} {where8})
                                WHERE rn = 1
 ORDER BY skill_name)
                                       PIVOT(sum(skill_score)
                                   FOR skill_name IN({ where})))a 
                ),
                Total_Summary AS (
                    SELECT 
                        NULL AS emp_no, 
                        NULL AS emp_name, 
                        'Summary' AS dept_name,  
                         {where6}
                        SUM(Actual_Skill_Achievement) AS Actual_Skill_Achievement,
                        SUM(Ideal_Skill_Achievement) AS Ideal_Skill_Achievement,
                        round(AVG(Skill_Index),2) AS Skill_Index
                    FROM 
                        tmp
                ),orderd_summary as(  
                 select * from Total_Summary 
                 union all
                 select * from tmp)
                select * from orderd_summary ORDER BY CASE WHEN dept_name = 'Summary' THEN 2 ELSE 1 END ,
          CASE
            WHEN dept_name <> 'Summary' THEN
             dept_name
            ELSE
             NULL
          END, emp_no NULLS LAST";
                #endregion
                DataTable Skill_Matrix = DB.GetDataTable(sql);
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", Skill_Matrix); 
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Prod_Plant(object OBJ)
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string sql = string.Empty;
                DataTable dt = DB.GetDataTable($@"SELECT DISTINCT udf05 AS department_code
  FROM base005m
 WHERE udf05 LIKE 'AP%'
 ORDER BY TO_NUMBER(REGEXP_SUBSTR(udf05, '\d+')) NULLS LAST, udf05");
                DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Model_Name(object OBJ)
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
                string sql = string.Empty;
                DataTable dt = DB.GetDataTable($@"SELECT DISTINCT MODEL_NAME FROM T_TSM_MODELWISE_PROCESS");
                DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Prod_line(object OBJ)
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
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string sql = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@"and a.udf05 = '{Plant}'";
                }
                switch (Department)
                {
                    case "Cutting":
                        where += $@"and a.udf01 in ('C')";
                        break;

                    case "Stitching":
                        where += $@"and a.udf01 in ('S')";
                        break;

                    case "Assembly":
                        where += $@"and a.udf01 in ('L')";
                        break;
                    case "Stock fitting":
                        where += $@"and a.udf01 in ('T')";
                        break;
                    case "Packing":
                        where += $@"and a.udf01 in ('A')";
                        break;
                    case "Spray Painting":
                        where += $@"and a.udf01 in ('SPD')";
                        break;
                    default:
                        where += $@"and udf01 in ('C','S','L','A')";
                        break;
                }
                //Added by Ashok  on 2024/07/25
                DataTable dt = DB.GetDataTable($@"select department_code
  from base005m a
 where 1=1 {where}");
                DB.Commit();
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
