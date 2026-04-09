using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Training_Status
    {
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
                string Training_Type = jarr.ContainsKey("Training_Type") ? jarr["Training_Type"].ToString() : "";
                string Process_Type = jarr.ContainsKey("Process_Type") ? jarr["Process_Type"].ToString() : "";
                string sql = string.Empty;
                string wheresql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Training_Type))
                {
                    where += $@"AND  Training_Type = '{ Training_Type}'";
                }

                sql = $@"
                    SELECT 
    COUNT(emp_no) AS Total_employees,
    COUNT(training_e_date) AS Training_completed,
    COUNT(CASE WHEN status = 'drop' THEN 1 END) AS Training_notcompleted
FROM 
    t_tsm_emp_registration WHERE created_date BETWEEN '{fromDate}' AND '{toDate}'{where}";
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
