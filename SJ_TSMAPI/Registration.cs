using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace SJ_TSMAPI
{
    public class Registration
    {
        
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUserDetails(object OBJ)
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
                string status = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";
                string sql = string.Empty;
                string wheresql = string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"WHERE c.emp_no = '{Barcode}'";
                }
                if (status == "0")
                { 
                    sql = $@" SELECT EMP_NO, EMP_NAME, DEPARTMENT, POSITION, TRAINER, PROCESS_TYPE, PROCESS_NAME, TRAINING_TYPE
                FROM t_tsm_emp_registration c {where} order by c.training_s_date desc 
                FETCH FIRST 1 ROW ONLY";
                }
                else if (status == "1")
                {
                    sql = $@"
select
c.emp_no,c.emp_name,a.department_code as DEPARTMENT,c.work_name as POSITION
from base005m a,t_oa_mes_department_compare b,t_oa_empmain c {where} and a.DEPARTMENT_CODE=b.MES_DEPARTMENTCODE and b.OA_DEPARTMENTCODE=c.DEPT_NO 
and c.STATUS='1' --and a.Factory_Sap is not null
";
                }
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTypeOfProcess(object OBJ)
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
               
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";
                string Model = jarr.ContainsKey("Model") ? jarr["Model"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = string.Empty;
                if(string.IsNullOrEmpty(Model))
                {
                  sql = $@"select NAME from T_Tsm_processlist WHERE PROCESS_TYPE= '{TYPE}'order by name ";
                }
                else
                {
                    sql = $@"SELECT distinct b.name
  FROM T_TSM_MODELWISE_PROCESS A INNER JOIN T_TSM_PROCESSLIST B ON A.SKILL_CODE=B.SKILL_CODE
 WHERE a.MODEL_NAME = '{Model}' and b.process_type='{TYPE}'";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertDetails(object OBJ)
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
                string Name = jarr.ContainsKey("Name") ? jarr["Name"].ToString() : ""; 
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : ""; 
                string Position = jarr.ContainsKey("Position") ? jarr["Position"].ToString() : "";  
                string Trainer = jarr.ContainsKey("Trainer") ? jarr["Trainer"].ToString() : ""; 
                string Process_Type = jarr.ContainsKey("Process_Type") ? jarr["Process_Type"].ToString() : ""; 
                string Process_Name = jarr.ContainsKey("Process_Name") ? jarr["Process_Name"].ToString() : ""; 
                string Training_Types = jarr.ContainsKey("Training_Types") ? jarr["Training_Types"].ToString() : ""; 
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken); 
                string date = DateTime.Now.ToString("yyyy/MM/dd"); 
                string time = DateTime.Now.ToString("HH:mm:ss"); 
                string EndDate = jarr.ContainsKey("EndDate") ? jarr["EndDate"].ToString() : "";
                string status = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (status == "0")
                {
                    string sql = $@"UPDATE  t_tsm_emp_registration SET TRAINING_E_DATE = '{EndDate}', Training_Type = 'Re_employee Training' ,MODIFIED_BY='{user}',MODIFIED_DATE= '{date}',MODIFIED_TIME='{time}' WHERE EMP_NO = '{Barcode}' and PROCESS_NAME='{Process_Name}'";
                    DB.ExecuteNonQuery(sql);
                    DB.Commit();
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Updated Successfully"; 
                }
                else if (status == "1")
                { 
                    string sql = $@"insert into t_tsm_emp_registration(EMP_NO,EMP_NAME,DEPARTMENT,POSITION,TRAINING_TYPE,PROCESS_TYPE,PROCESS_NAME,TRAINING_S_DATE,TRAINING_E_DATE,TRAINER,CREATED_BY,CREATED_DATE,CREATED_TIME) VALUES('{Barcode}','{Name}','{Department}','{Position}','{Training_Types}','{Process_Type}','{Process_Name}','{date}','{EndDate}','{Trainer}','{user}','{date}', '{time}')"; 
                    DB.ExecuteNonQuery(sql); 
                    ret.IsSuccess = true;
                    ret.ErrMsg = "inserted Successfully"; 
                    DB.Commit();
                }
                else if (status == "2")
                {
                    string sql = $@"UPDATE  t_tsm_emp_registration SET PROCESS_TYPE='{Process_Type}',PROCESS_NAME='{Process_Name}',TRAINER='{Trainer}',TRAINING_E_DATE = '{EndDate}' ,MODIFIED_BY='{user}',MODIFIED_DATE= '{date}',MODIFIED_TIME='{time}' WHERE EMP_NO = '{Barcode}' and PROCESS_NAME IS NULL";
                    DB.ExecuteNonQuery(sql);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "Updated Successfully";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getdatadetails(object OBJ) 
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
                string Process_Name = jarr.ContainsKey("Process_Name") ? jarr["Process_Name"].ToString() : "";
                string Training_Type = jarr.ContainsKey("Training_Type") ? jarr["Training_Type"].ToString() : "";
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string sql = string.Empty;
                string wheresql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Process_Type))
                {
                    where += $@"AND PROCESS_TYPE = '{Process_Type}'";
                }
                if (!string.IsNullOrWhiteSpace(Process_Name))
                {
                    where += $@"AND PROCESS_NAME = '{Process_Name}'";
                }
                if (!string.IsNullOrWhiteSpace(Training_Type))
                {
                    where += $@"AND  Training_Type = '{Training_Type}'";
                }
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"AND  EMP_NO = '{Barcode}'";
                }


                sql = $@"SELECT EMP_NO, EMP_NAME, DEPARTMENT, POSITION, TRAINER, PROCESS_TYPE, PROCESS_NAME, TRAINING_TYPE, TRAINING_S_DATE, TRAINING_E_DATE,STATUS
                FROM t_tsm_emp_registration
                WHERE created_date BETWEEN '{fromDate}' AND '{toDate}'{where}";



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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTrainerDetails(object OBJ)
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
                string Barcode = jarr.ContainsKey("Trainer") ? jarr["Trainer"].ToString() : "";//查询条件 生产厂商编号 
                string sql = string.Empty;
                string wheresql = string.Empty;

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"WHERE EMP_NO = '{Barcode}'";
                }
                sql = $@"
SELECT
EMP_NAME
FROM
	t_oa_empmain   {where}";

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProcessName(object OBJ)
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string processname = jarr.ContainsKey("process name") ? jarr["process name"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                 
                string sql = $@"select * from t_tsm_emp_skill_m a where a.emp_no='{Barcode}' and a.skill_name='{processname}'"; 

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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Hidingdetails(object OBJ)
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>(); 
                string sql = "SELECT * FROM t_tsm_emp_registration WHERE TRAINING_E_DATE >= '" + date + "' AND EMP_NO = '" + Barcode + "'"; 
                DataTable dt = DB.GetDataTable(sql); 
                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Savedetails(object OBJ)
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
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string Process_Name = jarr.ContainsKey("Process_Name") ? jarr["Process_Name"].ToString() : "";

                string EndDate = jarr.ContainsKey("EndDate") ? jarr["EndDate"].ToString() : "";
                string Status = jarr.ContainsKey("Status") ? jarr["Status"].ToString() : "";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();


                string sql = $@"UPDATE  t_tsm_emp_registration SET TRAINING_E_DATE = '{EndDate}',status='{Status}' WHERE EMP_NO = '{Barcode}' AND PROCESS_NAME = '{Process_Name}'";

                DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckModifybyUser(object OBJ)
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
                string User = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                String sql = $@"select WORK_NAME from t_oa_empmain where EMP_NO='{User}'";

                DataTable dt = DB.GetDataTable(sql);
                DataRow dr2 = dt.Rows[0];
                string WORK_NAME = dr2["WORK_NAME"].ToString();
                if (WORK_NAME == "Supervisor")
                {



                    ret.IsSuccess = true;

                }
                else
                {
                    ret.IsSuccess = false;
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
                string Process_Name = jarr.ContainsKey("Process_Name") ? jarr["Process_Name"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = string.Empty;
                sql = $@"select * from t_tsm_emp_registration where EMP_NO = '{Barcode}' and PROCESS_NAME = '{Process_Name}'";
                DataTable dt = DB.GetDataTable(sql);
                DataRow dr2 = dt.Rows[0];
                string Barcode2 = dr2["EMP_NO"].ToString();
                string Process_Name2 = dr2["Process_Name"].ToString();
                string Training_Types = dr2["Training_Type"].ToString();
                string TRAINING_S_DATE = dr2["TRAINING_S_DATE"].ToString();
                string PROCESS_TYPE = dr2["PROCESS_TYPE"].ToString();
                string EMP_NAME = dr2["EMP_NAME"].ToString();
                string POSITION = dr2["POSITION"].ToString();
                string Department = dr2["DEPARTMENT"].ToString();
                string Trainer = dr2["TRAINER"].ToString();
                string TRAINING_E_DATE = dr2["TRAINING_E_DATE"].ToString();
                string User_Account = user;
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql2 = string.Empty;
                sql2 = $@"insert into t_tsm_emp_registration_log(EMP_NO,EMP_NAME,DEPARTMENT,POSITION,TRAINING_TYPE,PROCESS_TYPE,PROCESS_NAME,TRAINING_S_DATE,TRAINING_E_DATE,TRAINER,CREATEDBY,CREATED_DATE,CREATED_TIME) VALUES('{Barcode2}','{EMP_NAME}','{Department}','{POSITION}','{Training_Types}','{PROCESS_TYPE}','{Process_Name}','{TRAINING_S_DATE}','{TRAINING_E_DATE}','{Trainer}','{User_Account}','{date}', '{time}')";
                DB.ExecuteNonQuery(sql2);
                string sql3 = string.Empty;
                sql3 = $@"DELETE  t_tsm_emp_registration WHERE  EMP_NO = '{Barcode2}' and PROCESS_NAME = '{Process_Name}'";
                DB.ExecuteNonQuery(sql3);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_NewUser_Details(object OBJ)
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
                string wheresql = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"WHERE c.emp_no = '{Barcode}'";
                }
               sql = $@" SELECT EMP_NO, EMP_NAME, DEPARTMENT, POSITION,TRAINING_TYPE
                FROM t_tsm_emp_registration c {where} AND  C.PROCESS_NAME IS NULL order by c.training_s_date desc 
                FETCH FIRST 1 ROW ONLY"; 
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Insert_New_User_Details(object OBJ)
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
                string Name = jarr.ContainsKey("Name") ? jarr["Name"].ToString() : "";
                string Department = jarr.ContainsKey("Department") ? jarr["Department"].ToString() : "";
                string Position = jarr.ContainsKey("Position") ? jarr["Position"].ToString() : "";  
                string Training_Types = jarr.ContainsKey("Training_Types") ? jarr["Training_Types"].ToString() : "";
                string Process_Type = jarr.ContainsKey("Process_Type") ? jarr["Process_Type"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss"); 
                string EndDate = jarr.ContainsKey("EndDate") ? jarr["EndDate"].ToString() : "";  
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>(); 
                    string sql = $@"insert into t_tsm_emp_registration(EMP_NO,EMP_NAME,DEPARTMENT,POSITION,TRAINING_TYPE,PROCESS_TYPE,TRAINING_S_DATE,TRAINING_E_DATE,CREATED_BY,CREATED_DATE,CREATED_TIME) VALUES('{Barcode}','{Name}','{Department}','{Position}','{Training_Types}','{Process_Type}','{date}','{EndDate}','{user}','{date}', '{time}')";
                    DB.ExecuteNonQuery(sql);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "inserted Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject LoadProcessType(object OBJ)
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
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string sql = "select distinct process_type from T_Tsm_processlist";
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
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(TYPE))
                {
                    where += $@" and a.process_type='{TYPE}'";
                }
                string sql =$@"select * from T_Tsm_processlist a where 1=1 {where} ";
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
    }
}
