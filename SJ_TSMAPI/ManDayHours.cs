using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class ManDayHours
    {
        public static object Barcode { get; private set; }
        public static string SKILL_TYPE { get; private set; }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CountDetails(object OBJ)


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
                //转译
                string fromDate = jarr.ContainsKey("fromDate") ? jarr["fromDate"].ToString() : "";
                string toDate = jarr.ContainsKey("toDate") ? jarr["toDate"].ToString() : "";
                string Process_Type = jarr.ContainsKey("Process_Type") ? jarr["Process_Type"].ToString() : "";
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string Skill_type = jarr.ContainsKey("Skill_type") ? jarr["Skill_type"].ToString() : "";
                string sql = string.Empty;
                string wheresql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Process_Type))
                {
                    where += $@"AND r.PROCESS_TYPE = '{Process_Type}'";
                }
                if (!string.IsNullOrWhiteSpace(Skill_type))
                {
                    where += $@"AND s.SKILL_TYPE = '{Skill_type}'";
                }
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"AND  r.EMP_NO = '{ Barcode}'";
                }

                 sql = $@"
                    SELECT 
                        r.process_type, 
                        s.skill_type,
                        COUNT(DISTINCT r.emp_no) AS TOTAL_emp_count,
                        COUNT(a.createddate) AS TRAINING_DAYS,
                        SUM(a.online_time) AS TRAINING_HOURS,
                        ROUND( COUNT(a.createddate) / COUNT(DISTINCT r.emp_no),2) AS avg_total_training_days,
                        ROUND( SUM(a.online_time) / COUNT(DISTINCT r.emp_no),2) AS avg_total_training_hours
                    FROM 
                        T_TSM_EMP_REGISTRATION r
                    INNER JOIN 
                        t_tsm_processlist s ON s.name = r.process_name
                    LEFT JOIN 
                        T_TSM_EMP_ATTENDANCE_M a ON a.emp_no = r.emp_no 
                                                   AND a.createddate BETWEEN training_s_date AND r.training_e_date
                    WHERE 
                        r.created_date BETWEEN '{fromDate}' AND '{toDate}'{where}
                    GROUP BY 
                        r.process_type, s.skill_type 
                    ";


                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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


    }
}
