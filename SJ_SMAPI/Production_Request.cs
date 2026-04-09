using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SJ_SMAPI
{
    public class Production_Request
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Plants(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                //string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                DataTable dt = new DataTable();
                dt = DB.GetDataTable($@"SELECT DISTINCT UDF05 FROM base005m WHERE UDF05 IS NOT NULL");
                //DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ProcessType(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                //string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                DataTable dt = new DataTable();
                dt = DB.GetDataTable($@"select distinct process_type from t_tsm_processlist");
                //DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Production_Lines(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                DB.Open();
                DB.BeginTransaction();
                DataTable dt = new DataTable();
                //dt = DB.GetDataTable($@"SELECT DISTINCT(UDF07) FROM base005m WHERE UDF07 IS NOT NULL and udf05 = '"+Plant+"'");
                dt = DB.GetDataTable($@"select distinct(Department_code) from base005m where udf05 = '" + Plant + "' order by department_code");
                // DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Emp_Deatils(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                DB.Open();
                DB.BeginTransaction();
                DataTable dt = new DataTable();
                //dt = DB.GetDataTable($@"select c.emp_no, c.emp_name, a.department_code as dept_name, c.work_name from base005m a join t_oa_mes_department_compare b on a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE join t_oa_empmain c on b.OA_DEPARTMENTCODE = c.DEPT_NO where c.emp_no = " + Barcode + " and c.STATUS = '1' and a.Factory_Sap is not null");
                string sql = "SELECT c.emp_name" +
                             "  FROM t_oa_empmain c" +
                             "  JOIN t_oa_mes_department_compare b" +
                             "    ON b.OA_DEPARTMENTCODE = c.DEPT_NO " +
                             "  JOIN base005m a" +
                             "    ON a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE" +
                             "   AND a.udf05 = '" + Plant + "'" +
                             " WHERE c.emp_no = '" + Barcode + "'" +
                             "   AND c.STATUS = '1'" +
                             "   AND a.Factory_Sap IS NOT NULL";
                dt = DB.GetDataTable(sql);

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_SkillName(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Process_type = jarr.ContainsKey("Process_type") ? jarr["Process_type"].ToString() : "";
                DB.Open();
                DB.BeginTransaction();
                DataTable dt = new DataTable();
                //dt = DB.GetDataTable($@"SELECT DISTINCT(UDF07) FROM base005m WHERE UDF07 IS NOT NULL and udf05 = '"+Plant+"'");
                dt = DB.GetDataTable($@"select distinct name from t_tsm_processlist where Process_type = '" + Process_type + "'");
                // DB.Commit();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Checkdept(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string Dept_code = jarr.ContainsKey("Dept_code") ? jarr["Dept_code"].ToString() : "";
                string User = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                DB.Open();
                DB.BeginTransaction();

                string sql = $@"
            SELECT COUNT(*) as dept_count
            FROM T_SM_ProductionRequest_m 
            WHERE CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
            AND plant = '{Plant}' 
            AND DEPARTMENTCODE = '{Dept_code}'";

                DataTable dt = DB.GetDataTable(sql);
                int deptCount = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["dept_count"]) : 0;

                if (deptCount > 0)
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "success";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No records found";
                }

                DB.Commit();
                // Retrieve data based on the current date and plant
                string selectSql = $@"
            SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME
            FROM T_SM_ProductionRequest_d
            WHERE PLANT = '{Plant}'
            AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
             AND  DEPARTMENTCODE ='" + Dept_code + "' ";
                DataTable dt1 = DB.GetDataTable(selectSql);

                // Handle result
                if (dt1.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt1, Count = dt1.Rows.Count });
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for the specified criteria.";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteData(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";
                string barcode = jarr.ContainsKey("barcode") ? jarr["barcode"].ToString() : "";

                db.Open();
                db.BeginTransaction();

                // Delete the employee data based on the given barcode
                string deleteSql = $@"
                DELETE FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}' AND DEPARTMENTCODE = '{deptCode}' AND BARCODE = '{barcode}'";
                db.ExecuteNonQuery(deleteSql);

                db.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Deleted successfully";
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteSelectedData(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";

                // Ensure jarr["barcodes"] is of type JArray
                var barcodes = jarr.ContainsKey("barcodes")
                    ? ((JArray)jarr["barcodes"]).ToObject<List<string>>()
                    : new List<string>();

                db.Open();
                db.BeginTransaction();

                // Delete the employee data based on the barcodes and today's date
                string barcodeList = string.Join("','", barcodes);
                string deleteSql = $@"
                                    DELETE FROM T_SM_ProductionRequest_d
                                    WHERE PLANT = '{plant}' 
                                    AND DEPARTMENTCODE = '{deptCode}' 
                                    AND BARCODE IN ('{barcodeList}')
                                    AND CREATEDDATE = TRUNC(SYSDATE)";  // Add date check for today's date

                db.ExecuteNonQuery(deleteSql);

                db.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Deleted successfully";
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Submit_Plant_Data(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string Dept_code = jarr.ContainsKey("Dept_code") ? jarr["Dept_code"].ToString() : "";
                string Head_count = jarr.ContainsKey("Head_count") ? jarr["Head_count"].ToString() : "";
                string User = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currentDateTime = DateTime.Now;
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = currentDateTime.ToString("HH:mm:ss");

                // Define the cut-off time
                // Check if current time is before 2:00 PM
                TimeSpan cutOffTime = new TimeSpan(14, 00, 0); // 2:00 PM
                //TimeSpan cutOffTime = new TimeSpan(16, 30, 0); // 4:30 PM

                if (currentDateTime.TimeOfDay > cutOffTime)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "4:30 PM";
                    return ret;
                }

                DB.Open();

                // Check the push status
                string statusCheckSql = $@"
                SELECT CONFIRM_STATUS 
                FROM T_SM_ProductionRequest_m 
                WHERE PLANT = '{Plant}' AND DEPARTMENTCODE = '{Dept_code}' AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD') ";
                DataTable statusDt = DB.GetDataTable(statusCheckSql);

                if (statusDt.Rows.Count > 0 && statusDt.Rows[0]["CONFIRM_STATUS"].ToString() == "1")
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Confirmed.";
                    return ret;
                }

                DB.BeginTransaction();
                string sql = $@"
                INSERT INTO T_SM_ProductionRequest_m(
                    PLANT,
                    DEPARTMENTCODE,
                    HEADCOUNT,
                    CREATEDBY,
                    CREATEDTIME
                ) 
                VALUES(
                    '{Plant}',
                    '{Dept_code}',
                    '{Head_count}',  
                    '{User}',
                    '{currentTime}'
                )";

                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "success";
                DB.Commit();
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Constraint";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static (int? HeadCount, string Message) GetHeadCountFromTable(SJeMES_Framework_NETCore.DBHelper.DataBase db, string currentDate, string plant, string deptCode)
        {
            // Check for headcount in T_SM_ProductionRequest_m
            string checkSql = $@"
             SELECT MAX(headcount) AS headcount
             FROM T_SM_ProductionRequest_m
             WHERE CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
             AND PLANT = '{plant}'
             AND DEPARTMENTCODE = '{deptCode}'";

            DataTable dtCheck = db.GetDataTable(checkSql);

            int? headCount = dtCheck.Rows.Count > 0 && dtCheck.Rows[0]["headcount"] != DBNull.Value
                              ? Convert.ToInt32(dtCheck.Rows[0]["headcount"])
                              : (int?)null;

            if (headCount.HasValue)
            {
                return (headCount, "Headcount NOT NULL");
            }

            // If no headcount in T_SM_ProductionRequest_m, get it from the fallback query
            string fallbackSql = $@"
             SELECT COUNT(*) AS HEADCOUNT
             FROM base005m a
             JOIN t_oa_mes_department_compare b ON a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
             JOIN t_oa_empmain c ON b.OA_DEPARTMENTCODE = c.DEPT_NO
             WHERE c.STATUS = '1'
             AND a.Factory_Sap IS NOT NULL
             AND a.DEPARTMENT_CODE = '{deptCode}'
             AND c.work_name = 'Worker'
             GROUP BY a.DEPARTMENT_CODE";

            DataTable dtFallback = db.GetDataTable(fallbackSql);

            int? fallbackHeadCount = dtFallback.Rows.Count > 0 && dtFallback.Rows[0]["HEADCOUNT"] != DBNull.Value
                                     ? Convert.ToInt32(dtFallback.Rows[0]["HEADCOUNT"])
                                     : (int?)null;

            if (fallbackHeadCount.HasValue)
            {
                return (fallbackHeadCount, "Headcount NULL");
            }

            return (null, "No headcount data found.");
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetHeadCount(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("Dept_code") ? jarr["Dept_code"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                // Check if current time is before 2:00 PM
                TimeSpan cutoffTime = new TimeSpan(14, 00, 0); // 2:00 PM
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                // TimeSpan cutoffTime = new TimeSpan(16, 30, 0); // 4:30 PM
                TimeSpan currentTime = DateTime.Now.TimeOfDay;
                db.Open();
                db.BeginTransaction();

                // Check if the current time is past 4:30 PM
                if (currentTime > cutoffTime)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "4:30 PM";

                    var (headCount1, message) = GetHeadCountFromTable(db, currentDate, plant, deptCode);

                    if (headCount1.HasValue)
                    {
                        ret.RetData = JsonConvert.SerializeObject(new { HeadCount = headCount1, Message = message });
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = message;
                    }

                    return ret;
                }

                // Existing logic for before 4:30 PM

                string checkStatusSql = $@"
                SELECT
                     (SELECT DISTINCT CONFIRM_STATUS
                      FROM T_SM_ProductionRequest_m
                      WHERE PLANT = '{plant}' 
                        AND DEPARTMENTCODE = '{deptCode}' 
                        AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD') 
                        AND CONFIRM_STATUS = 1) AS m_status,
                     (SELECT DISTINCT CONFIRM_STATUS
                      FROM T_SM_ProductionRequest_d
                      WHERE PLANT = '{plant}' 
                        AND DEPARTMENTCODE = '{deptCode}' 
                        AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD') 
                        AND CONFIRM_STATUS = 1) AS d_status
                FROM dual";

                DataTable dtConfirmed = db.GetDataTable(checkStatusSql);
                bool isAlreadyConfirmed = false;

                if (dtConfirmed.Rows.Count > 0)
                {
                    string mStatus = dtConfirmed.Rows[0]["m_status"].ToString();
                    string dStatus = dtConfirmed.Rows[0]["d_status"].ToString();

                    bool isMConfirmed = mStatus == "1";
                    bool isDConfirmed = dStatus == "1";

                    isAlreadyConfirmed = isMConfirmed && isDConfirmed;
                }

                if (isAlreadyConfirmed)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Confirmed.";

                    var (headCount2, message) = GetHeadCountFromTable(db, currentDate, plant, deptCode);

                    if (headCount2.HasValue)
                    {
                        ret.RetData = JsonConvert.SerializeObject(new { HeadCount = headCount2, Message = message });
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = message;
                    }

                    return ret;
                }

                // Use the new method to get headcount
                var (deptCount, msg) = GetHeadCountFromTable(db, currentDate, plant, deptCode);

                if (deptCount.HasValue)
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Success";
                    ret.RetData = JsonConvert.SerializeObject(new { HeadCount = deptCount, Message = msg });
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = msg;
                }

                string selectSql = $@"
                SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME, LEAVEDATE
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                AND DEPARTMENTCODE = '{deptCode}'";

                DataTable dt = db.GetDataTable(selectSql);

                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt, Count = dt.Rows.Count, HeadCount = deptCount });
                }
                else
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "No records found";
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckAndGetData(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("Dept_code") ? jarr["Dept_code"].ToString() : "";
                string headCount = jarr.ContainsKey("head") ? jarr["head"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                //string currentDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

                db.Open();
                db.BeginTransaction();

                // Check department existence
                string checkSql = $@"
                SELECT COUNT(*) as dept_count
                FROM T_SM_ProductionRequest_m 
                WHERE CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                AND plant = '{plant}' 
                AND DEPARTMENTCODE = '{deptCode}'";

                DataTable dtCheck = db.GetDataTable(checkSql);
                int deptCount = dtCheck.Rows.Count > 0 ? Convert.ToInt32(dtCheck.Rows[0]["dept_count"]) : 0;

                if (deptCount > 0)
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "success";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No records found";
                    return ret;
                }

                // Retrieve data if department exists
                string selectSql = $@"
                SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME,LEAVEDATE
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                AND DEPARTMENTCODE = '{deptCode}'";

                DataTable dt = db.GetDataTable(selectSql);

                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt, Count = dt.Rows.Count });
                }
                else
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "No data found for the specified criteria.";
                }

                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static async Task<string> SendApiRequestAsync(string usertoken, string subject, string body, string sendAll, string empnopz, string orgidpz, string deptnopz, string otherspz, string Reciever)
        {
            try
            {
                //Create an object to hold your request parameters
                var requestPayload = new
                {
                    subject,
                    body,
                    sendAll,
                    empnopz,
                    orgidpz,
                    deptnopz,
                    otherspz,
                    userList = new[] { Reciever }
                };


                //string jsonPayload = JsonSerializer.Serialize(requestPayload);
                string jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(requestPayload);

                // Create an HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Set the API endpoint URL//Replace with your actual API URL
                    string apiUrl = "https://apc.apachefootwear.com/Platform/message/EscalateAppMessgae";

                    // Add the usertoken to the HTTP headers
                    client.DefaultRequestHeaders.Add("Token", usertoken);

                    // Create a JSON content from the serialized payload
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Send a POST request to the API
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read and return the response content
                        string responseContent = await response.Content.ReadAsStringAsync();
                        return responseContent;
                    }
                    else
                    {
                        // Handle error responses here
                        return "API request failed: " + response.ReasonPhrase;
                    }
                }
            }
            catch (Exception e)
            {
                // Handle exceptions here
                return "Error: " + e.Message;
            }
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExistingdata(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                db.Open();
                db.BeginTransaction();

                // Retrieve data based on the current date and plant
                string selectSql = $@"
                SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                 AND  DEPARTMENTCODE ='" + deptCode + "' ";
                DataTable dt = db.GetDataTable(selectSql);

                // Handle result
                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt, Count = dt.Rows.Count });
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for the specified criteria.";
                }
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Absent_List(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string barcode = jarr.ContainsKey("barcode") ? jarr["barcode"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";
                string empName = jarr.ContainsKey("empName") ? jarr["empName"].ToString() : "";
                string processType = jarr.ContainsKey("processType") ? jarr["processType"].ToString() : "";
                string skillName = jarr.ContainsKey("skillName") ? jarr["skillName"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                string leavedate = jarr.ContainsKey("leavedate") ? jarr["leavedate"].ToString() : "";
                //string inserttDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                db.Open();
                db.BeginTransaction();

                // Update data in the database
                string updateSql = $@"
                UPDATE T_SM_ProductionRequest_d
                SET 
                    EMP_NAME = '{empName}',
                    DEPARTMENTCODE = '{deptCode}',
                    SKILL_TYPE = '{processType}',
                    SKILL_NAME = '{skillName}',
                    LEAVEDATE = '{leavedate}',
                    MODIFIEDBY = '{user}',
                    MODIFIEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD'),
                    MODIFIEDTIME = '{currentTime}'
                WHERE 
                    PLANT = '{plant}' AND 
                    BARCODE = '{barcode}' AND 
                    CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')";

                db.ExecuteNonQuery(updateSql);

                db.Commit();

                // Retrieve data based on the current date and plant
                string selectSql = $@"
                SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME,LEAVEDATE
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                AND  DEPARTMENTCODE ='" + deptCode + "' ";
                DataTable dt = db.GetDataTable(selectSql);

                // Handle result
                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt, Count = dt.Rows.Count });
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for the specified criteria.";
                }
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Insert_Absent_List(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string barcode = jarr.ContainsKey("barcode") ? jarr["barcode"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";
                string empName = jarr.ContainsKey("empName") ? jarr["empName"].ToString() : "";
                string processType = jarr.ContainsKey("processType") ? jarr["processType"].ToString() : "";
                string skillName = jarr.ContainsKey("skillName") ? jarr["skillName"].ToString() : "";
                string leaveDateString = jarr.ContainsKey("leaveDate") ? jarr["leaveDate"].ToString() : "";
                string leaveDate = leaveDateString.Replace('-', '/');
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                DateTime currentDateTime = DateTime.Now;
                string currentDate = currentDateTime.ToString("yyyy-MM-dd");
                string currentTime = currentDateTime.ToString("HH:mm:ss");
                // Define the cut-off time
                // Check if current time is before 2:00 PM
                TimeSpan cutOffTime = new TimeSpan(14, 00, 0); // 2:00 PM
                //TimeSpan cutOffTime = new TimeSpan(16, 30, 0); // 4:30 PM
                if (currentDateTime.TimeOfDay > cutOffTime)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "4:30 PM";
                    return ret;
                }

                db.Open();

                // Check the push status before inserting
                string statusCheckSql = $@"
                SELECT CONFIRM_STATUS 
                FROM T_SM_ProductionRequest_m 
                WHERE PLANT = '{plant}' AND DEPARTMENTCODE = '{deptCode}' AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')";
                DataTable statusDt = db.GetDataTable(statusCheckSql);

                if (statusDt.Rows.Count > 0 && statusDt.Rows[0]["CONFIRM_STATUS"].ToString() == "1")
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Confirmed.";
                    return ret;
                }

                db.BeginTransaction();

                // Insert data into the database
                string insertSql = $@"
                INSERT INTO T_SM_ProductionRequest_d(
                    PLANT,
                    BARCODE,
                    EMP_NAME,
                    DEPARTMENTCODE,
                    SKILL_TYPE,
                    SKILL_NAME,
                    LEAVEDATE,
                    CREATEDBY,
                    CREATEDDATE,
                    CREATEDTIME
                ) 
                 VALUES(
                    '{plant}',
                    '{barcode}',
                    '{empName}',
                    '{deptCode}',
                    '{processType}',
                    '{skillName}',
                    '{leaveDate}',
                    '{user}',
                    TO_DATE('{currentDate}', 'YYYY-MM-DD'),
                    '{currentTime}'
                    )";
                db.ExecuteNonQuery(insertSql);

                db.Commit();

                // Retrieve data based on the current date and plant
                string selectSql = $@"
                SELECT PLANT, BARCODE, EMP_NAME, DEPARTMENTCODE, SKILL_TYPE, SKILL_NAME, LEAVEDATE
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                AND DEPARTMENTCODE = '{deptCode}'";
                DataTable dt = db.GetDataTable(selectSql);

                // Handle result
                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(new { Data = dt, Count = dt.Rows.Count });
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found for the specified criteria.";
                }
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ConfirmData(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");
                UpdateAbsentCountByDepartment(db, plant, currentDate);

                // Check if current time is before 2:00 PM
                TimeSpan cutoffTime = new TimeSpan(14, 00, 0); // 2:00 PM
                TimeSpan now = DateTime.Now.TimeOfDay;

                if (now > cutoffTime)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "4:30 PM";
                    return ret;
                }

                db.Open();
                db.BeginTransaction();

                // Check count of records in T_SM_ProductionRequest_d
                string countSql = $@"
                SELECT COUNT(*)
                FROM T_SM_ProductionRequest_d
                WHERE PLANT = '{plant}'                       
                AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')";

                int recordCount = Convert.ToInt32(db.GetScalar(countSql));

                // Proceed only if there are records to update
                if (recordCount > 0)
                {
                    string checkStatusSql = $@"
                    SELECT
                        (SELECT DISTINCT CONFIRM_STATUS
                         FROM T_SM_ProductionRequest_m
                         WHERE PLANT = '{plant}'                        
                           AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD') 
                           AND CONFIRM_STATUS = 1) AS m_status,
                        (SELECT DISTINCT CONFIRM_STATUS
                         FROM T_SM_ProductionRequest_d
                         WHERE PLANT = '{plant}'                       
                           AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD') 
                           AND CONFIRM_STATUS = 1) AS d_status
                    FROM dual";

                    DataTable statusTable = db.GetDataTable(checkStatusSql);

                    bool isAlreadyConfirmed = false;

                    if (statusTable.Rows.Count > 0)
                    {
                        string mStatus = statusTable.Rows[0]["m_status"].ToString();
                        string dStatus = statusTable.Rows[0]["d_status"].ToString();

                        bool isMConfirmed = mStatus == "1";
                        bool isDConfirmed = dStatus == "1";

                        isAlreadyConfirmed = isMConfirmed && isDConfirmed;
                    }

                    if (isAlreadyConfirmed)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Confirmed.";
                    }
                    else
                    {
                        string updateSql = $@"
                    BEGIN                  
                        UPDATE T_SM_ProductionRequest_m
                        SET CONFIRM_STATUS = 1, SIGNATURE1 = 1, CONFIRMED_BY = '{user}', CONFIRMED_TIME = '{currentTime}', SIGN1BY = 'plantAssistant'
                        WHERE PLANT = '{plant}'
                          AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD');

                        UPDATE T_SM_ProductionRequest_d
                        SET CONFIRM_STATUS = 1, CONFIRMED_BY = '{user}', CONFIRMED_TIME = '{currentTime}'
                        WHERE PLANT = '{plant}'
                          AND CREATEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD');
                    END;";

                        db.ExecuteNonQuery(updateSql);
                        db.Commit();
                        //UpdateAbsentCountByDepartment(plant, currentDate);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "Data confirmed successfully.";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No records found to update.";
                }
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }
        public static void UpdateAbsentCountByDepartment(SJeMES_Framework_NETCore.DBHelper.DataBase db, string plant, string createdDate)
        {
            try
            {
                db.Open(); // Open the connection once
                db.BeginTransaction(); // Begin the transaction

                // Retrieve all department codes from the details table for the specified plant and date
                string getDepartmentsSql = $@"
                SELECT DISTINCT DEPARTMENTCODE
                FROM t_sm_productionrequest_d
                WHERE plant = '{plant}'
                AND createddate = TO_DATE('{createdDate}', 'YYYY-MM-DD')";

                DataTable departmentsTable = db.GetDataTable(getDepartmentsSql);

                // Iterate through each department and update the absent count
                foreach (DataRow row in departmentsTable.Rows)
                {
                    string departmentCode = row["DEPARTMENTCODE"].ToString();

                    // Get the count of records in the details table for this department
                    string getCountSql = $@"
                    SELECT COUNT(*) 
                    FROM t_sm_productionrequest_d
                    WHERE plant = '{plant}'
                    AND departmentcode = '{departmentCode}'
                    AND createddate = TO_DATE('{createdDate}', 'YYYY-MM-DD')";

                    int absentCount = Convert.ToInt32(db.ExecuteScalar(getCountSql));

                    // Update the absent count in the main table
                    string updateMainTableSql = $@"
                    UPDATE t_sm_productionrequest_m
                    SET absentcount = {absentCount}
                    WHERE plant = '{plant}'
                    AND departmentcode = '{departmentCode}'
                    AND createddate = TO_DATE('{createdDate}', 'YYYY-MM-DD')";

                    db.ExecuteNonQuery(updateMainTableSql);
                }

                db.Commit(); // Commit the transaction after all updates are done
            }
            catch (Exception ex)
            {
                db.Rollback(); // Rollback the transaction in case of an error
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                db.Close(); // Close the connection after the loop
            }
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_Plant_Data(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                string data = reqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";
                string deptCode = jarr.ContainsKey("deptCode") ? jarr["deptCode"].ToString() : "";
                string headCount = jarr.ContainsKey("headCount") ? jarr["headCount"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(reqObj.UserToken);
                //string inserttDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                string currentTime = DateTime.Now.ToString("HH:mm:ss");

                db.Open();
                db.BeginTransaction();

                // Update query
                string updateSql = $@"
                UPDATE T_SM_ProductionRequest_m
                SET HEADCOUNT = '{headCount}',
                    MODIFIEDBY = '{user}',
                    MODIFIEDTIME = '{currentTime}',
                    MODIFIEDDATE = TO_DATE('{currentDate}', 'YYYY-MM-DD')
                WHERE PLANT = '{plant}' 
                AND DEPARTMENTCODE = '{deptCode}' AND CREATEDDATE  = TO_DATE('{currentDate}', 'YYYY-MM-DD') ";

                int rowsAffected = db.ExecuteNonQuery(updateSql);
                db.Commit();
            }
            catch (Exception ex)
            {
                db.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEmployeeList(object OBJ)
        {
            var ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            var reqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            var db = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                db = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                db.Open();
                db.BeginTransaction();
                string sql = @"SELECT 
                            c.emp_no as barcode, 
                            c.emp_name, 
                            a.udf05 AS plant
                        FROM 
                            base005m a
                        JOIN 
                            t_oa_mes_department_compare b 
                            ON a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
                        JOIN 
                            t_oa_empmain c 
                            ON b.OA_DEPARTMENTCODE = c.DEPT_NO
                        WHERE 
                            c.STATUS = '1'
                            AND a.Factory_Sap IS NOT NULL
                        ORDER BY 
                            a.udf05";

                DataTable dt = db.GetDataTable(sql);
                var result = new List<Dictionary<string, object>>();

                foreach (DataRow row in dt.Rows)
                {
                    var record = new Dictionary<string, object>
                {
                    { "BARCODE", row["BARCODE"].ToString() },
                    { "EMP_NAME", row["EMP_NAME"].ToString() },
                    { "PLANT", row["PLANT"].ToString() }
                };
                    result.Add(record);
                }

                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            finally
            {
                db.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Aprroved_Status(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                DB.Open();
                DB.BeginTransaction();

                string sql = $@"
                SELECT confirm_status, signature1, signature2, signature3, signature4 
                FROM t_sm_productionrequest_m 
                WHERE plant = '{Plant}' 
                AND createddate = TO_DATE('{DateTime.Now.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')";
                DataTable dt = DB.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    bool allConfirmed = true;

                    foreach (DataRow row in dt.Rows)
                    {
                        int confirmStatus = Convert.ToInt32(row["confirm_status"]);
                        if (confirmStatus == 0)
                        {
                            allConfirmed = false;
                            break; // Exit loop as soon as we find one not confirmed
                        }
                    }

                    string statusMessage = allConfirmed ? "Confirmed" : "Not Confirmed";

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("ConfirmedStatus", statusMessage);
                    dic.Add("Signature1", dt.Rows[0]["Signature1"].ToString());
                    dic.Add("Signature2", dt.Rows[0]["Signature2"].ToString());
                    dic.Add("Signature3", dt.Rows[0]["Signature3"].ToString());
                    dic.Add("Signature4", dt.Rows[0]["Signature4"].ToString());

                    ret.IsSuccess = true;
                    ret.ErrMsg = statusMessage;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                }

                else
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "No records";
                }

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Approve_Status(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string Plant = jarr.ContainsKey("Plant") ? jarr["Plant"].ToString() : "";
                string StatusKey = jarr.ContainsKey("StatusKey") ? jarr["StatusKey"].ToString() : "";
                string Username = jarr.ContainsKey("Username") ? jarr["Username"].ToString() : "";
                string Password = jarr.ContainsKey("Password") ? jarr["Password"].ToString() : "";

                if (string.IsNullOrEmpty(StatusKey) || string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Missing";
                    return ret;
                }

                DB.Open();
                DB.BeginTransaction();

                string roleCondition = GetRoleForStatusKey(StatusKey);

                // Check if the username and password match the required role
                string roleValidationSql;
                if (roleCondition == "Rita" || roleCondition == "Production Manager")
                {
                    // For Rita and Production In-Charge, no need to check Plant
                    roleValidationSql = $@"
                    SELECT COUNT(*) 
                    FROM check_login
                    WHERE username = '{Username}' 
                    AND password = '{Password}' 
                    AND role = '{roleCondition}'";
                }
                else
                {
                    // For other roles, check Plant
                    roleValidationSql = $@"
                        SELECT COUNT(*) 
                        FROM check_login 
                        WHERE username = '{Username}' 
                        AND password = '{Password}' 
                        AND Plant = '{Plant}'
                        AND role = '{roleCondition}'";
                }

                int userCount = Convert.ToInt32(DB.ExecuteScalar(roleValidationSql));
                if (userCount == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Invalid";
                    DB.Rollback();
                    return ret;
                }

                // Update signature status
                string updateStatusSql = $@"
                UPDATE t_sm_productionrequest_m
                SET {GetSignatureField(StatusKey)} = '1', {GetSignedField(StatusKey)} = '{GetRoleSignStatusKey(StatusKey)}'
                WHERE plant = '{Plant}' 
                AND createddate = TO_DATE('{DateTime.Now.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')";


                int rowsAffected = DB.ExecuteNonQuery(updateStatusSql);
                if (rowsAffected > 0)
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Updated";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No records";
                }

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


        // Helper method to get the correct signature field based on StatusKey
        private static string GetSignatureField(string StatusKey)
        {
            switch (StatusKey)
            {
                case "plantAssistant":
                    return "signature1";
                case "plantInCharge":
                    return "signature2";
                case "productionManager":
                    return "signature3";
                case "rita":
                    return "signature4";
                default:
                    throw new ArgumentException("Invalid StatusKey");
            }
        }
        private static string GetSignedField(string StatusKey)
        {
            switch (StatusKey)
            {
                case "plantAssistant":
                    return "SIGN1BY";
                case "plantInCharge":
                    return "SIGN2BY";
                case "productionManager":
                    return "SIGN3BY";
                case "rita":
                    return "SIGN4BY";
                default:
                    throw new ArgumentException("Invalid StatusKey");
            }
        }
        private static string GetRoleSignStatusKey(string StatusKey)
        {
            switch (StatusKey)
            {
                case "plantAssistant":
                    return "Plant Assistant";
                case "plantInCharge":
                    return "Plant In-Charge";
                case "productionManager":
                    return "Sailaja-v";
                case "rita":
                    return "Rita";
                default:
                    throw new ArgumentException("Invalid StatusKey");
            }
        }

        // Helper method to get the required role based on StatusKey
        private static string GetRoleForStatusKey(string StatusKey)
        {
            switch (StatusKey)
            {
                case "plantAssistant":
                    return "Plant Assistant";
                case "plantInCharge":
                    return "Plant In-Charge";
                case "productionManager":
                    return "Production Manager";
                case "rita":
                    return "Rita";
                default:
                    throw new ArgumentException("Invalid StatusKey");
            }
        }



    }
}
