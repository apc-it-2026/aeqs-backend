using Newtonsoft.Json;
using SJeMES_Framework_NETCore.DBHelper;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Production_Adjustment
    {
        #region Old MPAC Logic
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddEmployee(object OBJ)
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
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken); 
                string sql = string.Empty;
                DataTable dt = new DataTable();

                //dt = DB.GetDataTable($@"select
                //c.emp_no,c.emp_name,a.department_code as dept_name,c.work_name
                //from base005m a,t_oa_mes_department_compare b,t_oa_empmain c where c.emp_no='{Barcode}' and a.DEPARTMENT_CODE=b.MES_DEPARTMENTCODE and b.OA_DEPARTMENTCODE=c.DEPT_NO 
                //and c.STATUS='1' and a.Factory_Sap is not null   
                //");

                //Added By ManiKanta
                dt = DB.GetDataTable($@"SELECT 
    e.emp_no, 
    e.emp_name, 
    COALESCE(c.MES_DEPARTMENTCODE, e.dept_name) as dept_name, 
    e.work_name
FROM 
    t_oa_empmain e
LEFT JOIN 
    t_oa_mes_department_compare c
    ON e.dept_no = c.oa_departmentcode
LEFT JOIN 
    base005m a
    ON a.DEPARTMENT_CODE = c.MES_DEPARTMENTCODE
WHERE 
    e.emp_no = '{Barcode}'
    AND e.STATUS = '1'
   -- AND (a.Factory_Sap IS NULL OR a.Factory_Sap IS NOT NULL)      
");
                //dt = DB.GetDataTable($@"select a.emp_no,a.emp_name,a.dept_name,a.work_name,a.dept_no from t_oa_empmain a where a.emp_no='{Barcode}' and dept_name not like '%MPAC%'");
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No such employee found";
                }
               else
                {
                    DataTable dt2 = new DataTable();

                    dt2 = DB.GetDataTable($@"select * from t_tsm_prod_support_list where EMP_NO ='{Barcode}' and createddate='{date}'");
                    if (dt2.Rows.Count > 0)
                    {
                        sql = $@"delete from t_tsm_prod_support_list where EMP_NO ='{Barcode}' and createddate='{date}'";
                        DB.ExecuteNonQuery(sql);
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Removed Sucessfully";
                    }
                    else
                    {

                    sql = $@"insert into t_tsm_prod_support_list (EMP_NO,EMP_NAME,EMP_DEPT,POSITION,STATUS,CREATEDBY,
                          CREATEDDATE,CREATEDTIME) 
                          values('{dt.Rows[0]["emp_no"]}','{dt.Rows[0]["emp_name"]}','{dt.Rows[0]["dept_name"]}','{dt.Rows[0]["work_name"]}','0',
                         '{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "Added Successfully"; 
                    }
                    
                   DB.Commit();
                   
                } 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTC_InwardEmployee(object OBJ)
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
                string date = DateTime.Now.ToString("yyyy/MM/dd"); 
                string sql = string.Empty;
                DataTable dt = new DataTable();
                dt = DB.GetDataTable($@"select a.emp_no,
       a.emp_name,
       a.emp_dept,
       a.position,
       a.createddate as indate, 
       a.createdtime as intime,
       LISTAGG(b.skill_name, ',') WITHIN GROUP (ORDER BY b.skill_score) AS skill_set
  from t_tsm_prod_support_list a
 left join t_tsm_emp_skill_m b
    on a.emp_no = b.emp_no
    where a.createddate='{date}' and emp_dept not like '%MPAC%'
    GROUP BY 
     a.emp_no,a.emp_no,
       a.emp_name,
       a.emp_dept,
       a.position,
       a.createddate,a.createdtime order by a.createdtime desc");
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt); 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEmpDept(object OBJ)
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
                string Dept_name = DB.GetString($@"select dept_name from t_oa_empmain where emp_no='{Barcode}'");
                DB.Commit();
                ret.IsSuccess = true;
                ret.RetData = Dept_name;
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string sql = string.Empty;
                //DataTable dt = DB.GetDataTable($@"select distinct dept_name as department_code from t_oa_empmain");
                //DataTable dt = DB.GetDataTable($@"select distinct department_code from base005m");

                //Added by ManiKanta
                //                DataTable dt = DB.GetDataTable($@"SELECT TO_CHAR(mes_departmentcode) AS department_code 
                //FROM t_oa_mes_department_compare 
                //WHERE oa_departmentcode = 'AP808'
                //UNION ALL
                //SELECT DISTINCT a.department_code 
                //FROM base005m a");

                //Added by Ashok  on 2024/07/25
                DataTable dt = DB.GetDataTable($@"SELECT TO_CHAR(mes_departmentcode) AS department_code 
FROM t_oa_mes_department_compare 
WHERE oa_departmentcode = 'AP808'
UNION ALL
 SELECT DISTINCT 
    CASE 
        WHEN udf05 = 'MK' THEN 'MaxKing'
        WHEN udf05 = 'APEX' THEN 'APEX'
    END AS department_code
FROM base005m 
WHERE udf05 IN ('APEX', 'MK')
UNION ALL
SELECT DISTINCT a.department_code 
FROM base005m a 
WHERE a.department_code NOT LIKE '%AX%' 
AND a.department_code NOT LIKE '%MK%'");
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LoadSupport_Dept(object OBJ)
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
                //DataTable dt = DB.GetDataTable($@"select DEPARTMENT_NAME from T_TSM_SUPPORTING_DEPT");
                DataTable dt = DB.GetDataTable($@"select DEPARTMENT_NAME from T_TSM_SUPPORTING_DEPT 
union
SELECT * FROM (select distinct udf05 AS DEPARTMENT_NAME
 from base005m a where a.factory_sap in ('5001','5021','5041','5011') and udf05 not in ('MK1','APO','SPC1','MK') order by udf05)
 UNION
 SELECT DISTINCT 
    CASE 
        WHEN udf05 = 'MK' THEN 'MaxKing'
    END AS DEPARTMENT_NAME
     from base005m  where  udf05 in ('MK') ");  //Added by Ashok to show plants in supporting deptments
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProcessList(object OBJ)
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
                string Supp_Dept = jarr.ContainsKey("Supp_Dept") ? jarr["Supp_Dept"].ToString() : "";
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";
                DataTable dt = new DataTable();
                if(string.IsNullOrEmpty(Type))
                {

                if (Supp_Dept.ToLower().Contains("maxking") || Supp_Dept.ToLower().Contains("apex") ||Supp_Dept.ToLower().Contains("training"))
                {
                    dt = DB.GetDataTable($@"select name from t_tsm_processlist");
                }
                else if (Supp_Dept.ToLower().Contains('c'))
                {
                    dt = DB.GetDataTable($@"select name from t_tsm_processlist where process_type='Cutting'");
                }
                else if(Supp_Dept.ToLower().Contains('s'))
                {
                    dt = DB.GetDataTable($@"select name from t_tsm_processlist where process_type='Stitching'");
                }
                else if (Supp_Dept.ToLower().Contains('l'))
                {
                    dt = DB.GetDataTable($@"select name from t_tsm_processlist where process_type='Assembly'");
                } else
                {
                    dt = DB.GetDataTable($@"select Work_Process as Name from t_tsm_plants_process");
                }

                }
                else
                {
                    dt = DB.GetDataTable($@"select Work_Process as Name from t_tsm_plants_process");
                }
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAvailableEmployee(object OBJ)
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
                string where1 = string.Empty;
                if (!string.IsNullOrEmpty(Barcode))
                {
                    where1 = $@" and emp_no='{Barcode}'";
                }

                DataTable dt = DB.GetDataTable($@"select emp_no,emp_name,emp_dept,position,support_dept,process_name,status 
                    from t_tsm_prod_support_list
                    where 1=1 {where1} and emp_dept not like '%MPAC%' and createddate=to_char(sysdate,'yyyy/MM/dd')

");

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMPACEmployee(object OBJ)
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
                string where1 = string.Empty;
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                if (!string.IsNullOrEmpty(Barcode))
                {
                    where1 = $@" and a.emp_no='{Barcode}'";
                }

                DataTable dt = DB.GetDataTable($@" select * from (
select emp_no,
       emp_name,emp_dept,position,support_dept,process_name,status from t_tsm_prod_support_list a  where a.createddate='{date}' and emp_dept like '%MPAC%' 
       union
       
       SELECT emp_no,
       emp_name,
       dept_name AS emp_dept,
       work_name AS position,
       CAST('' AS NVARCHAR2(50)) AS support_dept,
       CAST('' AS NVARCHAR2(50)) AS process_name,
       CAST('' AS NVARCHAR2(50)) AS status
FROM t_oa_empmain
WHERE dept_name LIKE '%MPAC%' and emp_no not in (select emp_no from t_tsm_prod_support_list a  where a.createddate='{date}' and emp_dept like '%MPAC%'))a where 1=1 {where1}
                   ");

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AllocateEmployee(object OBJ)
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
                string supdept = jarr.ContainsKey("supdept") ? jarr["supdept"].ToString() : "";
                string dt_json = jarr["dt"].ToString();
                string process_name = jarr.ContainsKey("process_name") ? jarr["process_name"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss"); 
                DataTable dt1 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));
                if (dt1.Rows.Count > 0)
                {
                    for (int i = 0; i < dt1.Rows.Count; i++)
                    {
                    DataRow dr = dt1.Rows[i];
                    string Emp_No = dr["Emp_No"].ToString(); 
                    string sql1 = string.Empty;
                    string sql2 = string.Empty;
                    DataTable dt = new DataTable();

                    sql1 = $@"select * from t_tsm_prod_support_list a where a.emp_no='{Emp_No}'and createddate='{date}'";
                    dt = DB.GetDataTable(sql1);
                        if (dt.Rows.Count > 0)
                        {
                            sql2 = $@"update t_tsm_prod_support_list set support_dept='{supdept}' , process_name ='{process_name}',modifiedby='{user}',modifieddate='{date}',modifiedtime='{time}',status='1'  where emp_no='{Emp_No}' AND createddate='{date}'";
                            DB.ExecuteNonQuery(sql2);
                        }
                        else
                        {
                            sql2 = $@"insert into t_tsm_prod_support_list (EMP_NO,EMP_NAME,EMP_DEPT,POSITION,STATUS,support_dept,process_name,CREATEDBY,
                          CREATEDDATE,CREATEDTIME) 
                          values('{dt1.Rows[i]["Emp_No"]}','{dt1.Rows[i]["Emp_Name"]}','{dt1.Rows[i]["Emp_Dept"]}','{dt1.Rows[i]["Position"]}','1','{supdept}','{process_name}',
                         '{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql2);
                        }
                    }
                }
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Allocated Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject De_AllocateEmployee(object OBJ)
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
                string Emp_No = jarr.ContainsKey("Emp_No") ? jarr["Emp_No"].ToString() : ""; 
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss"); 
                string sql = $@"update t_tsm_prod_support_list set support_dept='' , process_name ='',modifiedby='{user}',modifieddate='{date}',modifiedtime='{time}',status='0'  where emp_no='{Emp_No}'";
                DB.ExecuteNonQuery(sql); 
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "De Allocated Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionSupport_Report(object OBJ)
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
                string SDate = jarr.ContainsKey("SDate") ? jarr["SDate"].ToString() : "";
                string EDate = jarr.ContainsKey("EDate") ? jarr["EDate"].ToString() : ""; 
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                string sql1 = string.Empty;
#region Excess Employee Report
                sql1 = $@"WITH TMP AS
 (SELECT source_dept,
         SUM(RECEIVED_HEAD) AS RECEIVED_Count,
         SUM(AP5) AS AP5,
         SUM(AP1) AS AP1,
         SUM(AP2) AS AP2,
         SUM(AP7) AS AP7,
         SUM(AP3) AS AP3,
         SUM(AP6) AS AP6,
         SUM(APEX) AS APEX,
         SUM(AP10) AS AP10,
         SUM(AP8) AS AP8,
         SUM(AP9) AS AP9,
         SUM(API) AS API,
         SUM(AP5) + SUM(AP1) + SUM(AP2) + SUM(AP7) + SUM(AP3) +
         SUM(AP6) + SUM(APEX) + SUM(AP10) + SUM(AP8) + SUM(AP9) +
         SUM(API) as Total_Issued
    FROM (SELECT substr(emp_dept, 5, 3) AS source_dept,
                 COUNT(*) AS RECEIVED_HEAD,
                 CASE
                   WHEN b.udf05 = 'AP5' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP5,
                 CASE
                   WHEN b.udf05 = 'AP1' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP1,
                 CASE
                   WHEN b.udf05 = 'AP2' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP2,
                 CASE
                   WHEN b.udf05 = 'AP7' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP7,
                 CASE
                   WHEN b.udf05 = 'AP3' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP3,
                 CASE
                   WHEN b.udf05 = 'AP6' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP6,
                 CASE
                   WHEN b.udf05 = 'APEX' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS APEX,
                 CASE
                   WHEN b.udf05 = 'AP10' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP10,
                 CASE
                   WHEN b.udf05 = 'AP8' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP8,
                 CASE
                   WHEN b.udf05 = 'AP9' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS AP9,
                 CASE
                   WHEN b.udf05 = 'API' THEN
                    COUNT(*)
                   ELSE
                    0
                 END AS API
            FROM t_tsm_prod_support_list a left join base005m b on a.support_dept=b.department_code
          WHERE createddate BETWEEN '{SDate}' AND '{EDate}'
             and emp_dept not in
                 (select dept_name
                    from t_oa_empmain
                   where dept_name like '%MPAC%')
           GROUP BY substr(emp_dept, 5, 3), b.udf05)
   GROUP BY source_dept)
SELECT *
  FROM TMP
UNION
SELECT CAST('SUMMARY' AS NVARCHAR2(50)) AS source_dept,
       SUM(TMP.RECEIVED_Count) AS RECEIVED_Count,
       SUM(TMP.AP5) AS AP5,
       SUM(TMP.AP1) AS AP1,
       SUM(TMP.AP2) AS AP2,
       SUM(TMP.AP7) AS AP7,
       SUM(TMP.AP3) AS AP3,
       SUM(TMP.AP6) AS AP6,
       SUM(TMP.APEX) AS APEX,
       SUM(TMP.AP10) AS AP10,
       SUM(TMP.AP8) AS AP8,
       SUM(TMP.AP9) AS AP9,
       SUM(TMP.API) AS API,
       SUM(TMP.Total_Issued) AS Total_Issued
  FROM TMP
";
                #endregion

                #region MPAC employee Report
                string sql2 = string.Empty;
                sql2 = $@"select * from (select * from (SELECT 
     CAST(b.udf05 AS NVARCHAR2(50)) AS plant,
    SUM(CASE WHEN INSTR(a.support_dept, 'C') > 0 THEN 1 ELSE 0 END) AS Cutting,
    SUM(CASE WHEN INSTR(a.support_dept, 'S') > 0 THEN 1 ELSE 0 END) AS Stitching,
    SUM(CASE WHEN INSTR(a.support_dept, 'L') > 0 THEN 1 ELSE 0 END) AS Assembly,
    COUNT(*) AS Total
FROM 
    t_tsm_prod_support_list a 
    INNER JOIN base005m b ON a.support_dept = b.department_code
WHERE 
    a.createddate  between '{SDate}' and '{EDate}'
    AND emp_dept LIKE '%MPAC%' 
    AND status = '1' and a.support_dept!='Training Center'
GROUP BY 
    b.udf05 
    union all
        SELECT 
    a.support_dept AS plant,
    SUM(CASE WHEN b.process_type='Cutting' THEN 1 ELSE 0 END) AS Cutting,
    SUM(CASE WHEN b.process_type='Stitching' THEN 1 ELSE 0 END) AS Stitching,
    SUM(CASE WHEN b.process_type='Assembly' THEN 1 ELSE 0 END) AS Assembly,
    COUNT(*) AS Total
FROM 
    t_tsm_prod_support_list a 
     left join t_tsm_processlist b on a.process_name=b.name
WHERE 
    a.createddate  between '{SDate}' and '{EDate}'
    AND emp_dept LIKE '%MPAC%' 
    AND status = '1' and a.support_dept='Training Center'
GROUP BY 
    a.support_dept)x order by x.plant)
    
    union all
    select  CAST('Summary' AS NVARCHAR2(50)) as plant ,
    sum(nvl(d.Cutting,0)) Cutting ,
    sum(nvl(d.Stitching,0)) Stitching,
    sum(nvl(d.Assembly,0)) Assembly,
    sum(nvl(d.Total,0)) Total
    from (select * from (SELECT 
     CAST(b.udf05 AS NVARCHAR2(50)) AS plant,
    SUM(CASE WHEN INSTR(a.support_dept, 'C') > 0 THEN 1 ELSE 0 END) AS Cutting,
    SUM(CASE WHEN INSTR(a.support_dept, 'S') > 0 THEN 1 ELSE 0 END) AS Stitching,
    SUM(CASE WHEN INSTR(a.support_dept, 'L') > 0 THEN 1 ELSE 0 END) AS Assembly,
    COUNT(*) AS Total
FROM 
    t_tsm_prod_support_list a 
    INNER JOIN base005m b ON a.support_dept = b.department_code
WHERE 
   a.createddate  between '{SDate}' and '{EDate}'
    AND emp_dept LIKE '%MPAC%' 
    AND status = '1' and a.support_dept!='Training Center'
GROUP BY 
    b.udf05 
    union all
        SELECT 
    a.support_dept AS plant,
    SUM(CASE WHEN b.process_type='Cutting' THEN 1 ELSE 0 END) AS Cutting,
    SUM(CASE WHEN b.process_type='Stitching' THEN 1 ELSE 0 END) AS Stitching,
    SUM(CASE WHEN b.process_type='Assembly' THEN 1 ELSE 0 END) AS Assembly,
    COUNT(*) AS Total
FROM 
    t_tsm_prod_support_list a 
     left join t_tsm_processlist b on a.process_name=b.name
WHERE 
    a.createddate  between '{SDate}' and '{EDate}'
    AND emp_dept LIKE '%MPAC%' 
    AND status = '1' and a.support_dept='Training Center'
GROUP BY 
    a.support_dept)x order by x.plant)d
    
";
                #endregion

                dt1 = DB.GetDataTable(sql1);
                dt2 =DB.GetDataTable(sql2);
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
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
        #endregion

        #region New MPAC Logic As per Frieheit Sir Requirement
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserLine(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                DataTable dt = new DataTable();
                string ProdLine = DB.GetString($@"select STAFF_DEPARTMENT from hr001m a where a.staff_no = '{user}'");
                DB.Commit();
                ret.IsSuccess = true;
                ret.RetData = ProdLine;
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLineEmployee(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ProdPlant = jarr.ContainsKey("ProdPlant") ? jarr["ProdPlant"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string ProdLine = jarr.ContainsKey("ProdLine") ? jarr["ProdLine"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(ProdLine))
                {
                    where += $@"and a.department_code ='{ProdLine}'";
                }
                switch (Process)
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

                    default:
                        where += $@"and a.udf01 in ('C','S','L')";
                        break;
                }

                string sql = string.Empty;
                //               DataTable dt = DB.GetDataTable($@"select c.emp_no,
                //      c.emp_name,
                //      a.department_code as DEPARTMENT,
                //      '' as Working_Skill
                // from base005m a, t_oa_mes_department_compare b, t_oa_empmain c
                //where a.udf05 = '{ProdPlant}'
                //  and a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
                //  and b.OA_DEPARTMENTCODE = c.DEPT_NO
                //  and c.STATUS = '1' {where}
                //  AND C.EMP_NO NOT IN
                //      (select EMP_NO
                //         from T_TSM_PROD_ADJUSTMENT
                //        WHERE PLANT = '{ProdPlant}'
                //          AND to_char(PROD_DATE, 'yyyy/MM/dd') = '{Prod_Date}'
                //       union
                //       select EMP_NO
                //         from t_tsm_excess_employee
                //        WHERE PLANT = '{ProdPlant}'
                //          AND to_char(PROD_DATE, 'yyyy/MM/dd') = '{Prod_Date}') order by DEPARTMENT");

                DataTable dt = DB.GetDataTable($@"SELECT c.emp_no,
       c.emp_name,
       a.department_code AS department,
       NVL(pa.working_skill, '') AS working_skill,
       CASE
         WHEN pa.status = 1 THEN 'Plant_Inchage_Applied'
         WHEN pa.status = 2 THEN 'MPAC_Allocated'
         ELSE ''
       END AS status
  FROM base005m a
  JOIN t_oa_mes_department_compare b
    ON a.department_code = b.mes_departmentcode
  JOIN t_oa_empmain c
    ON b.oa_departmentcode = c.dept_no
  LEFT JOIN t_tsm_prod_adjustment pa
    ON pa.emp_no = c.emp_no
   AND pa.plant = '{ProdPlant}'
   AND TRUNC(pa.prod_date) = TO_DATE('{Prod_Date}', 'yyyy/MM/dd')
 WHERE a.udf05 = '{ProdPlant}'
   AND c.status = '1' {where}
 ORDER BY department");
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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

        public static ResultObject GetSkillsList(object obj)
        {
            RequestObject ReqObj = (RequestObject)obj;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(ReqObj);
                var Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string sql2 = $@"select a.skill_name  from t_tsm_emp_skill_m a where a.emp_no='{Barcode}'";
                DataTable dt = DB.GetDataTable(sql2);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000: " + ex.Message;
            }

            return ret;
        }

        public static ResultObject GetNextWorkingDay(object obj)
        {
            RequestObject ReqObj = (RequestObject)obj;
            ResultObject ret = new ResultObject();
            DataBase DB = new DataBase();
            try
            {
                DB = new DataBase(ReqObj);
                var Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql2 = $@"SELECT MIN(next_day) AS next_working_day
  FROM (SELECT TRUNC(SYSDATE) + LEVEL AS next_day
          FROM dual
        CONNECT BY LEVEL <= 366)
 WHERE next_day NOT IN
       (SELECT calendar
          FROM DA_CALENDAR_S@APCHRDB
         WHERE org_id = '100'
           AND TO_CHAR(calendar, 'YYYY') = TO_CHAR(SYSDATE, 'YYYY'))";
                string date = DB.GetString(sql2);
                ret.RetData = date;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000: " + ex.Message;
            }

            return ret;
        }

     
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveAbsentEmployee(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt_json = jarr.ContainsKey("dt") ? jarr["dt"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));
                if (Type == "Withdraw_Absent")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        DB.ExecuteNonQuery($@"DELETE FROM T_TSM_PROD_ADJUSTMENT
WHERE emp_no = '{dr["EMP_NO"].ToString()}'
  AND prod_date = TO_DATE('{Prod_Date}', 'YYYY/MM/DD')");

                    }
                }
                else if (Type == "Submit_Absent")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string Plant = DB.GetString($@"select udf05 from base005m where department_code='{dr["DEPARTMENT"].ToString()}'");
                        DB.ExecuteNonQuery($@"MERGE INTO T_TSM_PROD_ADJUSTMENT t
USING (
       SELECT TO_DATE('{Prod_Date}', 'YYYY/MM/DD') AS prod_date,
              '{dr["EMP_NO"].ToString()}'          AS emp_no
         FROM dual
      ) s
ON (
       t.prod_date = s.prod_date
   AND t.emp_no    = s.emp_no
   )
WHEN MATCHED THEN
  UPDATE SET
      t.working_skill = '{dr["WORKING_SKILL"].ToString()}',
      t.status        = 1,
      t.modifiedby     = '{user}',
      t.modifiedat     = SYSDATE
WHEN NOT MATCHED THEN
  INSERT (
      prod_date,
      emp_no,
      emp_name,
      emp_dept,
      plant,
      working_skill,
      createdby,
      createdat,
      status
  )
  VALUES (
      TO_DATE('{Prod_Date}', 'YYYY/MM/DD'),
      '{dr["EMP_NO"].ToString()}',
      '{dr["EMP_NAME"].ToString()}',
      '{dr["DEPARTMENT"].ToString()}',
      '{Plant}',
      '{dr["WORKING_SKILL"].ToString()}',
      '{user}',
      SYSDATE,
      1
  )");
                    }
                }
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg="Updated Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getplant(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Plant = DB.GetString($@"select PROD_PLANT from T_TSM_PROD_Incharge_List where EMP_NO='{user}'");
                ret.IsSuccess = true;
                ret.RetData = Plant;
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ProdLines(object OBJ)
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
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string where = string.Empty;
                switch (Process)
                {
                    case "Cutting":
                        where += $@"and udf01 in ('C')";
                        break;

                    case "Stitching":
                        where += $@"and udf01 in ('S')";
                        break;

                    case "Assembly":
                        where += $@"and udf01 in ('L')";
                        break;

                    default:
                        where += $@"and udf01 in ('C','S','L')";
                        break;
                }

                DataTable dt = DB.GetDataTable($@"select department_code from base005m where udf05='{Plant}' {where}");
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAbsentReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string From_Date = jarr.ContainsKey("From_Date") ? jarr["From_Date"].ToString() : "";
                string To_Date = jarr.ContainsKey("To_Date") ? jarr["To_Date"].ToString() : "";
                string ProdPlant = jarr.ContainsKey("ProdPlant") ? jarr["ProdPlant"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string sql = string.Empty;
                string Type = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrEmpty(ProdPlant))
                {
                    where += $@"and plant = '{ProdPlant}'";
                }
               if (!string.IsNullOrEmpty(Status))
                {
                    switch (Status)
                    {
                        //case "Under_Plant_Incharge_Review":
                        //    Type = "0";
                        //    break;
                        case "Plant_Inchage_Applied":
                            Type = "1";
                            break;
                        //case "Plant_Incharge_Rejected":
                        //    Type = "7";
                        //    break;
                        case "MPAC_Allocated":
                            Type = "2";
                            break;

                    }
                       
                           
                    where += $@"and status='{Type}'";
                }
                DataTable dt = DB.GetDataTable($@"select prod_date,
       plant,
       emp_dept,
       emp_no,
       emp_name,
       working_skill,
       CASE
         WHEN status = 1 THEN
          'Plant_Inchage_Applied'
         WHEN status = 2 THEN
          'MPAC_Allocated'
       END AS status,
       support_emp_no,
       support_emp_name,
       support_emp_dept
  from t_tsm_prod_adjustment
 where to_char(prod_date, 'yyyy/MM/dd') between '{From_Date}' and
       '{To_Date}' {where}");
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupApplyList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string ProdPlant = jarr.ContainsKey("ProdPlant") ? jarr["ProdPlant"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string where = string.Empty;
                string Type = string.Empty;
                if (!string.IsNullOrEmpty(ProdPlant))
                {
                    where += $@"and plant='{ProdPlant}'";
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    switch (Status)
                    {
                        //case "Under_Plant_Incharge_Review":
                        //    Type = "0";
                        //    break;
                        case "Plant_Inchage_Applied":
                            Type = "1";
                            break;
                        //case "Plant_Incharge_Rejected":
                        //    Type = "7";
                        //    break;
                        case "MPAC_Allocated":
                            Type = "2";
                            break;

                    }


                    where += $@"and status='{Type}'";
                }
                DataTable dt = DB.GetDataTable($@"SELECT emp_no,
       emp_name,
       emp_dept AS department,
       working_skill,
       CASE
         WHEN status = 0 THEN
          'Under_Plant_Incharge_Review'
         WHEN status = 1 THEN
          'Plant_Inchage_Accepted'
         WHEN status = 2 THEN
          'MPAC_Allocated'
         WHEN status = 7 THEN
          'Plant_Incharge_Rejected'
       END AS status
  FROM t_tsm_prod_adjustment
 where to_char(prod_date, 'yyyy/MM/dd') = '{Prod_Date}' {where}");
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Submit_PlantAsst_Approval(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt_json = jarr.ContainsKey("dt") ? jarr["dt"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string Approval = jarr.ContainsKey("Approval") ? jarr["Approval"].ToString() : "";
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));
                string status = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(Approval))
                    {
                        switch (Approval)
                        {
                            case "Accept":
                                status = "1";
                                break;
                            case "Reject":
                                status = "7";
                                break;
                        }
                    }

                    DB.ExecuteNonQuery($@"UPDATE T_TSM_PROD_ADJUSTMENT
SET 
    status        = {status} ,   
    modifiedby     = '{user}',
    modifiedat     = SYSDATE
WHERE emp_no = '{dr["EMP_NO"].ToString()}'
  AND prod_date = TO_DATE('{Prod_Date}', 'YYYY/MM/DD')");
                }
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Added Successfully";
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

        #region manikanta


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserPlant(object OBJ)
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
                string ProdDay = jarr.ContainsKey("ProdDay") ? jarr["ProdDay"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = string.Empty;
                if (ProdDay == "Today")
                {
                    sql = $@"select distinct PLANT
  from T_TSM_PROD_ADJUSTMENT
 where PROD_DATE =trunc(sysdate)";
                }
                else if (ProdDay == "NxtWorkingDay")
                {
                    sql = $@"select distinct PLANT
  from T_TSM_PROD_ADJUSTMENT
 where PROD_DATE in
       (
        
        SELECT MIN(next_day) AS next_working_day
          FROM (SELECT TRUNC(SYSDATE) + LEVEL AS next_day
                   FROM dual
                 CONNECT BY LEVEL <= 366)
         WHERE next_day NOT IN
               (SELECT calendar
                  FROM DA_CALENDAR_S@APCHRDB
                 WHERE org_id = '100'
                   AND TO_CHAR(calendar, 'YYYY') = TO_CHAR(SYSDATE, 'YYYY')))";
                }


                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                //dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLine(object OBJ)
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
                string PLANT = jarr.ContainsKey("PLANT") ? jarr["PLANT"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = $@"           
select distinct DEPARTMENT_CODE from base005m where UDF05 ='{PLANT}'
        ";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                //dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Skill_Details(object OBJ)
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
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string ProdPlant = jarr.ContainsKey("ProdPlant") ? jarr["ProdPlant"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string Punch_Status = jarr.ContainsKey("Punch_Status") ? jarr["Punch_Status"].ToString() : "";
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string where = string.Empty;
                string statusList = string.Empty;
                if (!string.IsNullOrEmpty(ProdPlant))
                {
                    where += $@" and plant='{ProdPlant}'";
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    switch (Status)
                    {
                        case "Assigned":
                            statusList = "'2'";
                            break;

                        case "Not_Assigned":
                            statusList = "'1'";
                            break;
                    }
                    where += $" and status in({statusList})";
                }
                else
                {
                    statusList = "'1','2'";
                    where += $" and status in({statusList})";
                }
                if (!string.IsNullOrEmpty(Punch_Status))
                {
                    switch (Punch_Status)
                    {
                        case "Punched":
                            where += $@" and EMP_NO in (select EMP_NO
                      from ca_icdata @apchrdb
                     where CARD_DATE = TO_DATE('{Prod_Date}', 'YYYY/MM/DD'))";
                            break;

                        case "Not_Punched":
                            where += $@" and EMP_NO not in (select EMP_NO
                      from ca_icdata @apchrdb
                     where CARD_DATE = TO_DATE('{Prod_Date}', 'YYYY/MM/DD'))";
                            break;
                    }

                }
                if (!string.IsNullOrEmpty(Process))
                {
                    switch (Process)
                    {
                        case "Cutting":
                            where += $" and emp_dept like '%C%'";
                            break;

                        case "Stitching":
                            where += $" and emp_dept like '%S%'";
                            break;
                        case "Assembly":
                            where += $" and emp_dept like '%L%'";
                            break;
                    }
                    
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = $@"
            select EMP_NO, EMP_NAME, EMP_DEPT, PLANT, WORKING_SKILL,SUPPORT_EMP_NO,SUPPORT_EMP_NAME,SUPPORT_EMP_DEPT from T_TSM_PROD_ADJUSTMENT
            WHERE PROD_DATE = TO_DATE('{Prod_Date}', 'YYYY/MM/DD') {where}";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                //dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Skill_Details2(object OBJ)
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

                string Working_Skill = jarr.ContainsKey("Working_Skill") ? jarr["Working_Skill"].ToString() : "";
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = string.Empty;
                string where = string.Empty;
                
                if (Type == "MPAC")
                {
                    sql = $@"
            SELECT e.EMP_NO, e.EMP_NAME, e.DEPT_NAME,s.SKILL_NAME
FROM t_oa_empmain e
Inner JOIN t_tsm_emp_skill_m s
    ON e.EMP_NO = s.EMP_NO
WHERE e.DEPT_NAME LIKE '%MPAC%' and SKILL_NAME='{Working_Skill}'
 and e.EMP_NO not in ( select SUPPORT_EMP_NO
                          from T_TSM_PROD_ADJUSTMENT
                         where PROD_DATE in  (to_date('{Prod_Date}','yyyy/MM/dd')) and SUPPORT_EMP_NO is not null)";
                }
                else if (Type == "Excess")
                {
                    sql = $@"SELECT EMP_NO, EMP_NAME, EMP_DEPT DEPT_NAME, WORKING_SKILL SKILL_NAME
   FROM t_tsm_excess_employee
  WHERE to_char(PROD_DATE, 'yyyy/MM/dd') = to_char(sysdate, 'yyyy/MM/dd')
    and WORKING_SKILL ='{Working_Skill}'
    and EMP_NO not in (select SUPPORT_EMP_NO
                         from T_TSM_PROD_ADJUSTMENT
                        where to_char(PROD_DATE, 'yyyy/MM/dd') = '{Prod_Date}'
                          and SUPPORT_EMP_NO is not null)";
                }

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                //dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateEmployee(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt_json = jarr.ContainsKey("dt") ? jarr["dt"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));
                foreach (DataRow dr in dt.Rows)
                {
                    string Plant = DB.GetString($@"select udf05 from base005m where department_code='{dr["DEPARTMENT"].ToString()}'");
                    DB.ExecuteNonQuery($@"INSERT INTO T_TSM_PROD_ADJUSTMENT
(
    prod_date,
    emp_no,
    emp_name,
    emp_dept,
    plant,
    working_skill,  
createdby,
createdat
)
VALUES
(
    TO_DATE('{Prod_Date}', 'YYYY/MM/DD'),
    '{dr["EMP_NO"].ToString()}',                            
    '{dr["EMP_NAME"].ToString()}',                         
    '{dr["DEPARTMENT"].ToString()}',                         
    '{Plant}',                          
    '{dr["WORKING_SKILL"].ToString()}',
    '{user}',
     sysdate)");
                }
                string sql = string.Empty;

                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Added Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveAbsentEmployee2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt_json = jarr.ContainsKey("dt") ? jarr["dt"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                string Select_Type = jarr.ContainsKey("Select_Type") ? jarr["Select_Type"].ToString() : "";

                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));

                // helper to safely escape single quotes for inline SQL
                string Sq(string s) => s?.Replace("'", "''") ?? "";

                var previousRows = new System.Collections.Generic.List<System.Collections.Generic.Dictionary<string, object>>();
                int updatedCount = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    string empNo = dr.Table.Columns.Contains("EMP_NO") ? Sq(dr["EMP_NO"]?.ToString() ?? "") : "";
                    if (string.IsNullOrEmpty(empNo))
                    {
                        // invalid input row
                        DB.Rollback();
                        ret.IsSuccess = false;
                        ret.ErrMsg = "EMP_NO is missing in one of the input rows.";
                        return ret;
                    }

                    // Ensure matching row exists
                    string existCheckSql = $@"select count(1) from T_TSM_PROD_ADJUSTMENT
                                     where emp_no = '{empNo}'
                                       and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')";
                    int exists = 0;
                    try
                    {
                        var exObj = DB.ExecuteScalar(existCheckSql);
                        if (exObj != null && int.TryParse(exObj.ToString(), out int n)) exists = n;
                    }
                    catch
                    {
                        exists = 0;
                    }

                    if (exists == 0)
                    {
                        DB.Rollback();
                        ret.IsSuccess = false;
                        ret.ErrMsg = $"No existing record found for EMP_NO = {empNo} and PROD_DATE = {Prod_Date}.";
                        return ret;
                    }

                    var prev = new System.Collections.Generic.Dictionary<string, object>();
                    try
                    {
                        // read columns one-by-one via DB.GetString (safe even if null)
                        prev["EMP_NO"] = empNo;
                        prev["PROD_DATE"] = DB.GetString($@"select TO_CHAR(prod_date,'YYYY/MM/DD') from T_TSM_PROD_ADJUSTMENT 
                                                   where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["EMP_NAME"] = DB.GetString($@"select emp_name from T_TSM_PROD_ADJUSTMENT 
                                                   where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["EMP_DEPT"] = DB.GetString($@"select emp_dept from T_TSM_PROD_ADJUSTMENT 
                                                   where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["PLANT"] = DB.GetString($@"select plant from T_TSM_PROD_ADJUSTMENT 
                                                where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["WORKING_SKILL"] = DB.GetString($@"select working_skill from T_TSM_PROD_ADJUSTMENT 
                                                       where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["SUPPORT_EMP_NO"] = DB.GetString($@"select support_emp_no from T_TSM_PROD_ADJUSTMENT 
                                                        where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["SUPPORT_EMP_NAME"] = DB.GetString($@"select support_emp_name from T_TSM_PROD_ADJUSTMENT 
                                                          where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["SUPPORT_EMP_DEPT"] = DB.GetString($@"select support_emp_dept from T_TSM_PROD_ADJUSTMENT 
                                                         where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                        prev["STATUS"] = DB.GetString($@"select status from T_TSM_PROD_ADJUSTMENT 
                                                where emp_no = '{empNo}' and prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')") ?? "";
                    }
                    catch
                    {
                        // ignore read errors for individual columns; prev will contain what we could read
                    }

                    previousRows.Add(prev);

                    // Now prepare update values from incoming DataRow
                    string empName = dr.Table.Columns.Contains("EMP_NAME") ? Sq(dr["EMP_NAME"]?.ToString() ?? "") : "";
                    string empDept = dr.Table.Columns.Contains("EMP_DEPT") ? Sq(dr["EMP_DEPT"]?.ToString() ?? "") : "";
                    string workingSkill = dr.Table.Columns.Contains("WORKING_SKILL") ? Sq(dr["WORKING_SKILL"]?.ToString() ?? "") : "";
                    string supportNo = dr.Table.Columns.Contains("SUPPORT_EMP_NO") ? Sq(dr["SUPPORT_EMP_NO"]?.ToString() ?? "") : "";
                    string supportName = dr.Table.Columns.Contains("SUPPORT_EMP_NAME") ? Sq(dr["SUPPORT_EMP_NAME"]?.ToString() ?? "") : "";
                    string supportDept = dr.Table.Columns.Contains("SUPPORT_EMP_DEPT") ? Sq(dr["SUPPORT_EMP_DEPT"]?.ToString() ?? "") : "";
                    string statusValRaw = dr.Table.Columns.Contains("STATUS") ? dr["STATUS"]?.ToString() ?? "" : "";
                    string statusVal = Sq(statusValRaw);

                    // compute plant as you did (escape dept)
                    string Plant = DB.GetString($@"select udf05 from base005m where department_code='{Sq(empDept)}'") ?? "";
                    string status = string.Empty;
                    if (Select_Type == "De_Allocate")
                    {
                        status = "1";
                    }
                    else 
                    {
                        status = "2";     //Default condition is to allow employee allocation before day. written by Ashok
                    }

                    // Build SET clause parts - NOTE: do NOT update PROD_DATE (per your request)
                    var setParts = new System.Collections.Generic.List<string>
            {
                $"emp_name = '{empName}'",
                $"emp_dept = '{empDept}'",
                $"plant = '{Sq(Plant)}'",
                $"working_skill = '{workingSkill}'",
                $"support_emp_no = '{supportNo}'",
                $"support_emp_name = '{supportName}'",
                $"support_emp_dept = '{supportDept}'",
                $"modifiedby = '{Sq(user)}'",
                $"modifiedat = sysdate",
                $"status='{status}'"
            };

                    // Only include status if provided; write numeric without quotes if numeric
                    if (!string.IsNullOrEmpty(statusValRaw))
                    {
                        if (decimal.TryParse(statusValRaw, out decimal statusNum))
                        {
                            setParts.Add($"status = {statusValRaw}");
                        }
                        else
                        {
                            setParts.Add($"status = '{statusVal}'");
                        }
                    }

                    string setClause = string.Join(",\n    ", setParts);

                    string updateSql = $@"
UPDATE T_TSM_PROD_ADJUSTMENT
SET
    {setClause}
WHERE
    emp_no = '{empNo}'
    AND prod_date = TO_DATE('{Sq(Prod_Date)}','YYYY/MM/DD')";

                    // Execute update
                    DB.ExecuteNonQuery(updateSql);
                    updatedCount++;
                }

                // if we reached here all rows processed successfully
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = $"Updated: {updatedCount} row(s).";

                // return previous rows data as JSON in ret.Data (caller can inspect previous values)
                try
                {
                    // ret.Data = Newtonsoft.Json.JsonConvert.SerializeObject(previousRows);
                }
                catch
                {
                    // ignore serialization errors
                    //ret.Data = null;
                }

                return ret;
            }
            catch (Exception ex)
            {
                try { DB.Rollback(); } catch { /* ignore rollback error */ }
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                return ret;
            }
            finally
            {
                DB.Close();
            }
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMPACReport(object OBJ)
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
                string Start_Date = jarr.ContainsKey("Start_Date") ? jarr["Start_Date"].ToString() : "";
                string End_Date = jarr.ContainsKey("End_Date") ? jarr["End_Date"].ToString() : "";
                string ProdPlant = jarr.ContainsKey("ProdPlant") ? jarr["ProdPlant"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string where = string.Empty;
                string statusList = string.Empty;
                if (!string.IsNullOrEmpty(ProdPlant))
                {
                    where += $@" and plant='{ProdPlant}'";
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    switch (Status)
                    {
                        case "Assigned":
                            statusList = "'2'";
                            break;

                        case "Not_Assigned":
                            statusList = "'1'";
                            break;
                    }
                    where += $" and status in({statusList})";
                }
                else
                {
                    statusList = "'1','2'";
                    where += $" and status in({statusList})";
                }
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = $@"
            select EMP_NO, EMP_NAME, EMP_DEPT, PLANT, WORKING_SKILL,SUPPORT_EMP_NO,SUPPORT_EMP_NAME,SUPPORT_EMP_DEPT from T_TSM_PROD_ADJUSTMENT
            WHERE PROD_DATE between TO_DATE('{Start_Date}', 'YYYY/MM/DD') and TO_DATE('{End_Date}', 'YYYY/MM/DD') {where}";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                //dic.Add("data1", dt1);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveExcessEmployee(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt_json = jarr.ContainsKey("dt") ? jarr["dt"].ToString() : "";
                string Prod_Date = jarr.ContainsKey("Prod_Date") ? jarr["Prod_Date"].ToString() : "";
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt_json, (typeof(DataTable)));
                foreach (DataRow dr in dt.Rows)
                {
                    string Plant = DB.GetString($@"select udf05 from base005m where department_code='{dr["DEPARTMENT"].ToString()}'");
                    DB.ExecuteNonQuery($@"insert into t_tsm_excess_employee
(
    prod_date,
    emp_no,
    emp_name,
    emp_dept,
    plant,
    working_skill,
createdby,
createdat,
status
)
values
(
    TO_DATE('{Prod_Date}', 'YYYY/MM/DD'),
    '{dr["EMP_NO"].ToString()}',                            
    '{dr["EMP_NAME"].ToString()}',                         
    '{dr["DEPARTMENT"].ToString()}',                         
    '{Plant}',                          
    '{dr["WORKING_SKILL"].ToString()}',
    '{user}',
     sysdate,0)");
                }
                string sql = string.Empty;

                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Added Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExcessReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string From_Date = jarr.ContainsKey("From_Date") ? jarr["From_Date"].ToString() : "";
                string To_Date = jarr.ContainsKey("To_Date") ? jarr["To_Date"].ToString() : "";
                string ProdLine = jarr.ContainsKey("ProdLine") ? jarr["ProdLine"].ToString() : "";
                string sql = string.Empty;
                DataTable dt = DB.GetDataTable($@"select prod_date,
       plant,

       emp_dept,
       emp_no,
       emp_name,
       working_skill
  from t_tsm_excess_employee
 where to_char(prod_date, 'yyyy/MM/dd') between '{From_Date}' and
       '{To_Date}'
   and emp_dept = '{ProdLine}'");
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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
        #endregion


        #endregion



    }
}
