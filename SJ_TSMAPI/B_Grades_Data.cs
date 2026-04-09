using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class B_Grades_Data
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Insert_BGrade_Repairs_Data(object OBJ)
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
                string ProductionLine = jarr.ContainsKey("ProductionLine") ? jarr["ProductionLine"].ToString() : "";
                string TotalReceived = jarr.ContainsKey("TotalReceived") ? jarr["TotalReceived"].ToString() : "";
                string TotalRepaired = jarr.ContainsKey("TotalRepaired") ? jarr["TotalRepaired"].ToString() : "";
                string TotalUnRepaired = jarr.ContainsKey("TotalUnRepaired") ? jarr["TotalUnRepaired"].ToString() : "";
                string IssueType = jarr.ContainsKey("IssueType") ? jarr["IssueType"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                string sql = $@"insert into T_TSM_B_GRADE_REPAIRS_DATA_M(PRODUCTION_LINE,RECEIVED_QUANTITY,REPAIRED_QUANTITY,UNREPAIRED_QUANTITY,REPAIR_REASON,createdby,createddate,createdtime) 
                               values('{ProductionLine}','{TotalReceived}','{TotalRepaired}','{TotalUnRepaired}','{IssueType}','{user}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_BGrade_Repairs_Data(object OBJ)
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
                string fromDate = jarr.ContainsKey("fromDate") ? jarr["fromDate"].ToString() : "";
                string toDate = jarr.ContainsKey("toDate") ? jarr["toDate"].ToString() : "";
                string Issue_Type = jarr.ContainsKey("Issue_Type") ? jarr["Issue_Type"].ToString() : "";
                string Production_Line = jarr.ContainsKey("Production_Line") ? jarr["Production_Line"].ToString() : "";
                string where = string.Empty;

                if (!string.IsNullOrWhiteSpace(fromDate))
                {
                    where += $@"AND createddate >= '{fromDate}'";
                }
                if (!string.IsNullOrWhiteSpace(toDate))
                {
                    where += $@"AND createddate <= '{toDate}'";
                }
                if (!string.IsNullOrWhiteSpace(Issue_Type))
                {
                    where += $@"AND  REPAIR_REASON = '{Issue_Type}'";
                }
                if (!string.IsNullOrWhiteSpace(Production_Line))
                {
                    where += $@"AND  PRODUCTION_LINE = '{Production_Line}'";
                }
                string sql = $@"select PRODUCTION_LINE,RECEIVED_QUANTITY,REPAIRED_QUANTITY,UNREPAIRED_QUANTITY,REPAIR_REASON from T_TSM_B_GRADE_REPAIRS_DATA_M
                               where 1=1 {where}";
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
