using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;


namespace SJ_TSMAPI
{
    class Termination_Emp_List
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Termination_Emp_Skill_Data(object OBJ)
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string where1 = string.Empty;
                string where2 = string.Empty;
                if (!string.IsNullOrWhiteSpace(Process))
                {
                    where1 += $@"AND PROCESS_TYPE = '{Process}'";
                }
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where2 += $@"AND a.emp_no='{Barcode}'";
                }
                DataTable dt = DB.GetDataTable($@" select name from t_tsm_processlist where 1=1 {where1} order by skill_type desc");
                int count = dt.Rows.Count;
                string where = string.Empty;
                foreach (DataRow item in dt.Rows)
                {
                    where += "'" + item["name"].ToString() + "'" + "AS " + '"' + item["name"].ToString() + '"' + ",";
                }

                where = where.TrimEnd(',');

                string where4 = string.Empty;
                string where5 = string.Empty;
                string where6 = string.Empty;

                foreach (DataRow item in dt.Rows)
                {
                    where4 += "COALESCE((" + '"' + item["name"].ToString() + '"' + "), 0) AS " + item["name"].ToString() + ",";
                }

                foreach (DataRow item in dt.Rows)
                {
                    where5 += "COALESCE((" + '"' + item["name"].ToString() + '"' + "), 0)+";
                }
                where5 = where5.TrimEnd('+');

                foreach (DataRow item in dt.Rows)
                {
                    where6 += "round(SUM(" + item["name"].ToString() + ")/(count(*) * 4) * 100, 2) AS " + item["name"].ToString() + ",";
                }


                string sql = $@"
                select * from (SELECT 
                    emp_no,
                    emp_name,
                    dept_name, 
                    {where4} 
                    {where5} as Actual_Skill_Achievement,
                    {count} * 4 as Ideal_Skill_Achievement from(select a.emp_no,
       a.emp_name,
       a.dept_name,
       b.skill_name,
       b.skill_score from (select emp_no, emp_name, dept_name from T_TSM_TERMINATED_EMP)a 
 left
  join t_tsm_emp_skill_m b
                on a.emp_no = b.emp_no where 1 = 1 
                GROUP BY
                     a.emp_no,
                       a.emp_name,
                       a.dept_name,
                        b.skill_name,
                       b.skill_score)
                       PIVOT(sum(skill_score)
                   FOR skill_name IN({ where}))) a where a.Actual_Skill_Achievement>0 {where2}";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Import_Termination_Emp_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string Source = jarr["Data"].ToString();
                DataTable dt = (DataTable)JsonConvert.DeserializeObject(Source, typeof(DataTable));
                DB.Open();
                DB.BeginTransaction();
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        int count = DB.GetInt32($@"select count(*) from T_TSM_TERMINATED_EMP where emp_no='{dr["EMP_NO"]}'");
                        if (count > 0)
                        {
                            string sql2 = $@"UPDATE T_TSM_TERMINATED_EMP
SET
    EMP_NAME = '{dr["EMP_NAME"]}',
    DEPT_NO = '{dr["DEPT_NO"]}',
    DEPT_NAME = '{dr["DEPT_NAME"]}',
    WORK_NAME = '{dr["POSITION"]}',
    UPDATED_AT = SYSDATE
WHERE EMP_NO = '{dr["EMP_NO"]}'
";
                            DB.ExecuteNonQuery(sql2);
                        }
                        else
                        {
                            string sql2 = $@"insert into T_TSM_TERMINATED_EMP( EMP_NO,
       EMP_NAME,
       DEPT_NO,
       DEPT_NAME,        
       WORK_NAME,
       UPDATED_AT
        ) VALUES
             (
               '{dr["EMP_NO"]}',
               '{dr["EMP_NAME"]}',
               '{dr["DEPT_NO"]}', 
               '{dr["DEPT_NAME"]}',
               '{dr["POSITION"]}',
                 SYSDATE)";
                            DB.ExecuteNonQuery(sql2);
                        }

                    }
                }

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
