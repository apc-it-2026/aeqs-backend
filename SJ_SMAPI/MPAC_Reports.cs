using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_SMAPI
{
    class MPAC_Reports
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Plant_wise_Absent_Report(object OBJ)
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
                string SkillType = jarr.ContainsKey("SkillType") ? jarr["SkillType"].ToString() : "";
                string SkillName = jarr.ContainsKey("SkillName") ? jarr["SkillName"].ToString() : ""; 
                string sdate1 = jarr.ContainsKey("sdate") ? jarr["sdate"].ToString() : "";
                string edate1 = jarr.ContainsKey("edate") ? jarr["edate"].ToString() : ""; 
                string sdate = !string.IsNullOrEmpty(sdate1)?Convert.ToDateTime(sdate1).ToString("yyyy/MM/dd"):"";
                string edate = !string.IsNullOrEmpty(edate1) ? Convert.ToDateTime(edate1).ToString("yyyy/MM/dd") : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@" and a.plant='{Plant}'";
                }
                if (!string.IsNullOrEmpty(SkillType))
                {
                    where += $@" and a.skill_type='{SkillType}'";
                }
                if (!string.IsNullOrEmpty(SkillName))
                {
                    where += $@" and a.skill_name='{SkillName}'";
                }
                if (!string.IsNullOrEmpty(sdate) && !string.IsNullOrEmpty(edate))
                {
                    where += $@" and a.leavedate BETWEEN '{sdate}' AND '{edate}'";
                }

                string sql = $@"WITH PlantCounts AS (
  SELECT 
    a.plant AS label, 
    COUNT(a.plant) AS value
  FROM 
    t_sm_productionrequest_d a
  WHERE 1=1 
   {where}
  GROUP BY 
    a.plant
),
TotalCount AS (
  SELECT 
    SUM(value) AS total_value
  FROM 
    PlantCounts
)
SELECT 
  label AS LABEL,
  value AS VALUE,
  ROUND((value * 100.0 / total_value), 2) AS percentage
FROM 
  PlantCounts, 
  TotalCount
ORDER BY 
  value DESC";

                DataTable dt = DB.GetDataTable(sql);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Skill_wise_Absent_Report(object OBJ)
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
                string SkillType = jarr.ContainsKey("SkillType") ? jarr["SkillType"].ToString() : "";
                string SkillName = jarr.ContainsKey("SkillName") ? jarr["SkillName"].ToString() : "";
                string sdate1 = jarr.ContainsKey("sdate") ? jarr["sdate"].ToString() : "";
                string edate1 = jarr.ContainsKey("edate") ? jarr["edate"].ToString() : "";
                string sdate = !string.IsNullOrEmpty(sdate1) ? Convert.ToDateTime(sdate1).ToString("yyyy/MM/dd") : "";
                string edate = !string.IsNullOrEmpty(edate1) ? Convert.ToDateTime(edate1).ToString("yyyy/MM/dd") : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Plant))
                {
                    where += $@" and a.plant='{Plant}'";
                }
                if (!string.IsNullOrEmpty(SkillType))
                {
                    where += $@" and a.skill_type='{SkillType}'";
                }
                if (!string.IsNullOrEmpty(SkillName))
                {
                    where += $@" and a.skill_name='{SkillName}'";
                }
                if (!string.IsNullOrEmpty(sdate) && !string.IsNullOrEmpty(edate))
                {
                    where += $@" and a.leavedate BETWEEN '{sdate}' AND '{edate}'";
                }


                string sql = $@"WITH skill_nameCounts AS (
  SELECT 
    a.skill_name AS label, 
    COUNT(a.skill_name) AS value
  FROM 
    t_sm_productionrequest_d a 
   WHERE 1=1 
   {where}
  GROUP BY 
    a.skill_name
),
TotalCount AS (
  SELECT 
    SUM(value) AS total_value
  FROM 
    skill_nameCounts
)
SELECT 
  label AS LABEL,
  value AS VALUE,
  ROUND((value * 100.0 / total_value), 2) AS percentage
FROM 
  skill_nameCounts, 
  TotalCount
ORDER BY 
  value DESC
";

                DataTable dt = DB.GetDataTable(sql);
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Load_Plant(object OBJ)
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
                string sql = $@"select distinct udf05 as Plant from base005m where 1=1 and udf05 is not null order by udf05 ";
                DataTable dt = DB.GetDataTable(sql);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Load_SkillName(object OBJ)
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
                string SkillType = jarr.ContainsKey("SkillType") ? jarr["SkillType"].ToString() : "";
                string where = string.Empty;
                if(!string.IsNullOrEmpty(SkillType))
                {
                    where += $@"and Process_type = '{SkillType}'";
                }
                string sql = $@"select Name as SkillName from t_tsm_processlist where 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Load_SkillType(object OBJ)
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
                string sql = $@"select distinct process_type as SkillType  from t_tsm_processlist";
                DataTable dt = DB.GetDataTable(sql);
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
