using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_MESAPI
{
    class Sterilization
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Departmentload(object OBJ)
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
                string sql = $@"SELECT DISTINCT DEPARTMENT
                           FROM T_QIP_STERILIZATION_DETAILS";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Locationload(object OBJ)
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
                string Location = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string sql = $@"SELECT DISTINCT LOCATION
                                FROM T_QIP_STERILIZATION_DETAILS
                                WHERE DEPARTMENT = '{Location}'";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Deptpicload(object OBJ)
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
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Department))
                { 
                        where += $@" AND DEPARTMENT = '{Department}'";
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                string sql = $@"SELECT Distinct DEPT_PIC  
                           FROM T_QIP_STERILIZATION_DETAILS
                           Where 1=1{where}";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Plandateload(object OBJ)
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
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Department))
                {
                    where += $@" AND DEPARTMENT = '{Department}'";
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                string sql = $@"SELECT MAX(PLAN_DATE) AS PLAN_DATE
                                FROM T_QIP_STERILIZATION_DATA
                                WHERE 1=1 {where}";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Imppicload(object OBJ)
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
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string Deptpic = jarr.ContainsKey("Deptpic") ? jarr["Deptpic"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Department))
                {
                    where += $@" AND DEPARTMENT = '{Department}'";
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                if (!string.IsNullOrEmpty(Deptpic))
                {
                    where += $@" AND DEPT_PIC = '{Deptpic}'";
                }
                string sql = $@"SELECT Distinct IMP_PIC  
                           FROM T_QIP_STERILIZATION_DETAILS
                           Where 1=1{where}";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getimageguid(object OBJ)
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
                string Img_name = jarr.ContainsKey("Img_name") ? jarr["Img_name"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Img_name))
                {
                    where += $@" AND IMAGE_NAME = '{Img_name}'";
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                if (!string.IsNullOrEmpty(Department))
                {
                    where += $@" AND DEPARTMENT = '{Department}'";
                }
                string sql = $@"SELECT IMAGE_GUID  
                           FROM T_QIP_STERILIZATION_DATA
                           Where 1=1{where}";
                DataTable dt = DB.GetDataTable(sql);
                string imageGuid = string.Empty;
                if (dt != null && dt.Rows.Count > 0)
                {
                    imageGuid = dt.Rows[0]["IMAGE_GUID"].ToString();
                }
                string sql1 = $@"
SELECT
	FILE_URL
FROM bdm_upload_file_item 
WHERE GUID = '{imageGuid}'";
                DataTable dt1 = DB.GetDataTable(sql1);
                Dictionary<string, object> dic = new Dictionary<string, object>();




                dic.Add("Data", dt1);

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getfileurl(object OBJ)
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
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string Department = jarr.ContainsKey("Dept") ? jarr["Dept"].ToString() : "";
                string Filename = jarr.ContainsKey("Filename") ? jarr["Filename"].ToString() : "";
                string where = string.Empty;
               
                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                if (!string.IsNullOrEmpty(Department))
                {
                    where += $@" AND DEPARTMENT = '{Department}'";
                }
                if (!string.IsNullOrEmpty(Filename))
                {
                    where += $@" AND FILE_NAME = '{Filename}'";
                }
                string sql = $@"SELECT FILE_GUID  
                           FROM T_QIP_STERILIZATION_DATA
                           Where 1=1{where}";
                DataTable dt = DB.GetDataTable(sql);
                string Fileguid = string.Empty;
                if (dt != null && dt.Rows.Count > 0)
                {
                    Fileguid = dt.Rows[0]["FILE_GUID"].ToString();
                }
                string sql1 = $@"
SELECT
	FILE_URL
FROM bdm_upload_file_item 
WHERE GUID = '{Fileguid}'";
                DataTable dt1 = DB.GetDataTable(sql1);
                Dictionary<string, object> dic = new Dictionary<string, object>();




                dic.Add("Data", dt1);

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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Viewsterilizationdata(object OBJ)
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
                string From = jarr.ContainsKey("From") ? jarr["From"].ToString() : "";
                string To = jarr.ContainsKey("To") ? jarr["To"].ToString() : "";
                string Dept = jarr.ContainsKey("Dept") ? jarr["Dept"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Dept))
                {
                    where += $@" AND DEPARTMENT = '{Dept}'";
                }

                if (!string.IsNullOrEmpty(Location))
                {
                    where += $@" AND LOCATION = '{Location}'";
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    where += $@" AND STATUS = '{Status}'";
                }
                string sql = $@"SELECT DEPARTMENT,LOCATION,PLAN_DATE,FINISH_DATE,NEXT_DUE_DATE,DEPT_PIC,IMP_PIC,STATUS,IMAGE_NAME,FILE_NAME,INSERTED_BY,INSERTED_AT,MODIFIED_BY,MODIFIED_AT
                                FROM T_QIP_STERILIZATION_DATA
                                WHERE PLAN_DATE >= TO_DATE('{From}', 'yyyy/MM/dd')
                                AND PLAN_DATE <  TO_DATE('{To}', 'yyyy/MM/dd') + 1 {where}";

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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Dayssubmit(object OBJ)
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
                int Days = 0;
                if (jarr.ContainsKey("Days"))
                {
                    int.TryParse(jarr["Days"].ToString(), out Days);
                }

                // Check if record exists
                string checkSql = "SELECT COUNT(1) FROM T_QIP_STERILIZATION_EMAIL_ALERT_DAYS";
                string count = DB.GetStringline(checkSql);

                string sql = string.Empty;
                if (Convert.ToInt32(count) > 0)
                {
                    // Update existing record
                    sql = $@"UPDATE T_QIP_STERILIZATION_EMAIL_ALERT_DAYS 
                 SET EMAIL_ALERT_DAYS = {Days}";
                }
                else
                {
                    // Insert new record
                    sql = $@"INSERT INTO T_QIP_STERILIZATION_EMAIL_ALERT_DAYS(EMAIL_ALERT_DAYS) 
                 VALUES({Days})";
                }

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertSterilizationdata(object OBJ)
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

                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                // Extract values from JSON
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Location = jarr.ContainsKey("Location") ? jarr["Location"].ToString() : "";
                string Plandate = jarr.ContainsKey("Plandate") ? jarr["Plandate"].ToString() : "";
                string Finishdate = jarr.ContainsKey("Finishdate") ? jarr["Finishdate"].ToString() : "";
                string Nextduedate = jarr.ContainsKey("Nextduedate") ? jarr["Nextduedate"].ToString() : "";
                string Deptpic = jarr.ContainsKey("Deptpic") ? jarr["Deptpic"].ToString() : "";
                string Imppic = jarr.ContainsKey("Imppic") ? jarr["Imppic"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";
                string Img_guid = jarr.ContainsKey("Img_guid") ? jarr["Img_guid"].ToString() : "";
                string Img_name = jarr.ContainsKey("Img_name") ? jarr["Img_name"].ToString() : "";
                string Doc_guid = jarr.ContainsKey("Doc_guid") ? jarr["Doc_guid"].ToString() : "";
                string Doc_name = jarr.ContainsKey("Doc_name") ? jarr["Doc_name"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DateTime currTime = DateTime.Now;
                string sql2 = string.Empty;
                // Check if record exists
                string sql = $@"SELECT COUNT(*) FROM T_QIP_STERILIZATION_DATA 
                        WHERE DEPARTMENT = :Department 
                          AND LOCATION = :Location 
                          AND PLAN_DATE >= TO_DATE(:Plandate, 'yyyy/MM/dd')";
                var checkParams = new Dictionary<string, object>
        {
            { "Department", Department },
            { "Location", Location },
            { "Plandate", Plandate }
        };
                string exist = DB.GetStringline(sql, checkParams);

                var paramsDic = new Dictionary<string, object>();

                if (Convert.ToInt32(exist) > 0)
                {
                    // UPDATE existing record
                    sql = @"
UPDATE T_QIP_STERILIZATION_DATA
SET 
    FINISH_DATE   = TO_DATE(:Finishdate, 'yyyy/MM/dd'),
    NEXT_DUE_DATE = TO_DATE(:Nextduedate, 'yyyy/MM/dd'),
    DEPT_PIC      = :Deptpic,
    IMP_PIC       = :Imppic,
    STATUS        = :Status,
    IMAGE_GUID    = :Img_guid,
    IMAGE_NAME    = :Img_name,
    FILE_GUID     = :Doc_guid,
    FILE_NAME     = :Doc_name,
    MODIFIED_BY   = :ModifiedBy,
    MODIFIED_AT   = :ModifiedAt
WHERE DEPARTMENT = :Department
  AND LOCATION   = :Location
  AND PLAN_DATE >= TO_DATE(:Plandate, 'yyyy/MM/dd')";

                    paramsDic.Add("Finishdate", Finishdate);
                    paramsDic.Add("Nextduedate", Nextduedate);
                    paramsDic.Add("Deptpic", Deptpic);
                    paramsDic.Add("Imppic", Imppic);
                    paramsDic.Add("Status", Status);
                    paramsDic.Add("Img_guid", Img_guid);
                    paramsDic.Add("Img_name", Img_name);
                    paramsDic.Add("Doc_guid", Doc_guid);
                    paramsDic.Add("Doc_name", Doc_name);
                    paramsDic.Add("ModifiedBy", user);
                    paramsDic.Add("ModifiedAt", currTime); // DateTime
                    paramsDic.Add("Department", Department);
                    paramsDic.Add("Location", Location);
                    paramsDic.Add("Plandate", Plandate);

                    DB.ExecuteNonQuery(sql, paramsDic);
                    if (1 == 1)
                    {
                        sql2 = @"
INSERT INTO T_QIP_STERILIZATION_DATA
(DEPARTMENT, LOCATION, PLAN_DATE,
 DEPT_PIC, IMP_PIC, STATUS)
VALUES
(:Dept, :Loc,
 TO_DATE(:Nextdue, 'yyyy/MM/dd'),
 :Deptpi, :Imppi, :State)";

                        paramsDic.Add("Dept", Department);
                        paramsDic.Add("Loc", Location);
                        paramsDic.Add("Nextdue", Nextduedate);
                        paramsDic.Add("Deptpi", Deptpic);
                        paramsDic.Add("Imppi", Imppic);
                        paramsDic.Add("State", "Pending");

                        DB.ExecuteNonQuery(sql2, paramsDic);
                    }
                }
                else
                {
                    // INSERT new record
                    sql = @"
INSERT INTO T_QIP_STERILIZATION_DATA
(DEPARTMENT, LOCATION, PLAN_DATE, FINISH_DATE, NEXT_DUE_DATE,
 DEPT_PIC, IMP_PIC, STATUS, IMAGE_GUID, IMAGE_NAME, FILE_GUID, FILE_NAME,
 INSERTED_BY, INSERTED_AT)
VALUES
(:Department, :Location,
 TO_DATE(:Plandate, 'yyyy/MM/dd'),
 TO_DATE(:Finishdate, 'yyyy/MM/dd'),
 TO_DATE(:Nextduedate, 'yyyy/MM/dd'),
 :Deptpic, :Imppic, :Status, :Img_guid, :Img_name,
 :Doc_guid, :Doc_name, :InsertedBy, :InsertedAt)";

                    paramsDic.Add("Department", Department);
                    paramsDic.Add("Location", Location);
                    paramsDic.Add("Plandate", Plandate);
                    paramsDic.Add("Finishdate", Finishdate);
                    paramsDic.Add("Nextduedate", Nextduedate);
                    paramsDic.Add("Deptpic", Deptpic);
                    paramsDic.Add("Imppic", Imppic);
                    paramsDic.Add("Status", Status);
                    paramsDic.Add("Img_guid", Img_guid);
                    paramsDic.Add("Img_name", Img_name);
                    paramsDic.Add("Doc_guid", Doc_guid);
                    paramsDic.Add("Doc_name", Doc_name);
                    paramsDic.Add("InsertedBy", user);
                    paramsDic.Add("InsertedAt", currTime); // DateTime

                    DB.ExecuteNonQuery(sql, paramsDic);
                    if (1 == 1)
                    {
                         sql2 = @"
INSERT INTO T_QIP_STERILIZATION_DATA
(DEPARTMENT, LOCATION, PLAN_DATE,
 DEPT_PIC, IMP_PIC, STATUS)
VALUES
(:Dept, :Loc,
 TO_DATE(:Nextdue, 'yyyy/MM/dd'),
 :Deptpi, :Imppi, :State)";

                        paramsDic.Add("Dept", Department);
                        paramsDic.Add("Loc", Location);
                        paramsDic.Add("Nextdue", Nextduedate);
                        paramsDic.Add("Deptpi", Deptpic);
                        paramsDic.Add("Imppi", Imppic);
                        paramsDic.Add("State", "Pending");
                        DB.ExecuteNonQuery(sql2, paramsDic);
                    }
                }

                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submission failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
    }
}
