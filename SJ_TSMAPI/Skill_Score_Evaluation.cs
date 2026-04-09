//using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Skill_Score_Evaluation
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEmp_RegistrationDetails(object OBJ)
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
                string Type = jarr.ContainsKey("Type") ? jarr["Type"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"WHERE c.emp_no = '{Barcode}'";
                }

                if (Type=="0")
                {
                    string sql = string.Empty;
                    string process = DB.GetString($@"SELECT PROCESS_NAME FROM T_TSM_EMP_REGISTRATION WHERE EMP_NO='{Barcode}' AND TRAINING_E_DATE>=TO_CHAR(SYSDATE,'YYYY/MM/DD')");
                    if (string.IsNullOrEmpty(process))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "No such employee found";
                    }
                    else
                    {
                        DataTable dt1 = new DataTable();
                        dt1 = DB.GetDataTable($@"select * from t_tsm_skill_score_evaluation_m where barcode='{Barcode}' and process='{process}'");
                        if (dt1.Rows.Count > 0)
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("Data1", dt1);
                            dic.Add("Status", "1"); // 1 means Already evaluated employee
                            ret.IsSuccess = true;
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        }
                        else
                        {
                            DataTable dt2 = new DataTable();
                            dt2 = DB.GetDataTable($@"select emp_name, department, process_name, trainer, training_e_date
  from t_tsm_emp_registration 
 where emp_no = '{Barcode}'
   AND TRAINING_E_DATE>=TO_CHAR(SYSDATE,'YYYY/MM/DD')");

                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("Data1", dt2);
                            dic.Add("Status", "0");//0 means still not yet evaluated employee
                            ret.IsSuccess = true;
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        }
                    }
                }
                else if (Type == "1")
                {
                    string sql = string.Empty; 
                    sql = $@"";
                    DataTable dt2 = new DataTable();
                    dt2 = DB.GetDataTable(sql);
                    if (dt2.Rows.Count == 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "No such employee found";
                    }
                    else
                    {
                        DataTable dt3 = new DataTable();
                        dt3 = DB.GetDataTable($@"select skill_name from t_tsm_emp_skill_m c {where}");

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("Data2", dt2);
                        dic.Add("Data3", dt3);
                        dic.Add("Status", "0");
                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    }
                    
                }

                else if (Type == "2")
                {
                    string sql = string.Empty;
                    sql = $@"
select
c.emp_name,a.department_code as DEPARTMENT
from base005m a,t_oa_mes_department_compare b,t_oa_empmain c {where} and a.DEPARTMENT_CODE=b.MES_DEPARTMENTCODE and b.OA_DEPARTMENTCODE=c.DEPT_NO 
and c.STATUS='1' --and a.Factory_Sap is not null";
                    DataTable dt2 = new DataTable();
                    dt2 = DB.GetDataTable(sql);



                    if (dt2.Rows.Count == 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "No such employee found";
                    }
                    else
                    {
                        DataTable dt3 = new DataTable();
                        dt3 = DB.GetDataTable($@"select name from t_tsm_processlist");
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("Data2", dt2);
                        dic.Add("Data3", dt3);
                        dic.Add("Status", "0");
                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    }
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
                DataTable dt = DB.GetDataTable($@"select name from t_tsm_processlist");
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckSkillName(object OBJ)
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
                string SkillName = jarr.ContainsKey("SkillName") ? jarr["SkillName"].ToString() : "";
                string sql = string.Empty;
                int count = DB.GetInt32($@"select count(name) from t_tsm_processlist where name='{SkillName}'");
                DB.Commit();
                if(count>0)
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
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Print_SkillScore(object OBJ)
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
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string sql = string.Empty;     
               DataTable dt2 = DB.GetDataTable($@"select a.*, b.EXAMINATION_STANDARD,b.EVALUATION_STANDARD,to_char(sysdate,'yyyy/MM/dd') as now,c.training_s_date,c.training_e_date
  from t_tsm_skill_score_evaluation_m a
  left join t_tsm_skill_standard b   on a.process = b.skillname
  left join t_tsm_emp_registration c on a.barcode=c.emp_no and a.process=c.process_name 
 where a.barcode = '{Barcode}' and a.process='{Process}'");

                DataTable dt = dt2.Clone();

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("Data", dt2); 
                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic); 
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertSignature(object OBJ)
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
                string username = jarr.ContainsKey("username") ? jarr["username"].ToString() : "";
                string password = jarr.ContainsKey("password") ? jarr["password"].ToString() : "";
                string Designation_Code = jarr.ContainsKey("Designation_Code") ? jarr["Designation_Code"].ToString() : "";
                string Designation_Name = jarr.ContainsKey("Designation_Name") ? jarr["Designation_Name"].ToString() : "";
                byte[] image = jarr.ContainsKey("image") ? Convert.FromBase64String(jarr["image"].ToString()) : new byte[0];
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy/MM/dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                sql = "INSERT INTO T_TSM_SIG (USERNAME, PWD, IMAGE, Designation_Code, Designation_Name, CREATEDBY, CREATEDDATE, CREATEDTIME)" +
                    "VALUES(:V_user, :V_pwd, :V_image, :V_Designation_Code, :V_Designation_Name, :V_User, :V_CreatedDate, :V_CreatedTime)"; 
                //sql = "INSERT INTO T_TSM_SIG (USERNAME, PWD, Designation_Code,Designation_Name,IMAGE) VALUES (:V_user, :V_pwd, :V_image,:Designation_Code,:Designation_Name)"; 
                var insertParamsDic = new Dictionary<string, object>();
                insertParamsDic.Add("V_user", username);
                insertParamsDic.Add("V_pwd", password);
                insertParamsDic.Add("V_image", image);
                insertParamsDic.Add("V_Designation_Code", Designation_Code);
                insertParamsDic.Add("V_Designation_Name", Designation_Name);
                insertParamsDic.Add("V_User", user);
                insertParamsDic.Add("V_CreatedDate", date);
                insertParamsDic.Add("V_CreatedTime", time);
                DB.ExecuteNonQuery(sql, insertParamsDic);
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ValidateCredentials(object OBJ)
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
                string username = jarr.ContainsKey("username") ? jarr["username"].ToString() : "";
                string password = jarr.ContainsKey("password") ? jarr["password"].ToString() : "";
                string receivedIndex = jarr.ContainsKey("receivedIndex") ? jarr["receivedIndex"].ToString() : "";
                string sql = string.Empty;
                DataTable dt = new DataTable();
                dt = DB.GetDataTable($@"select * from t_tsm_sig where username='{username}' and pwd = '{password}' and designation_code='{receivedIndex}'");
                DB.Commit();
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No such employee found";
                }
                else
                {  
                    ret.IsSuccess = true; 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Update_password(object OBJ)
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
                string username = jarr.ContainsKey("username") ? jarr["username"].ToString() : "";
                string password = jarr.ContainsKey("password") ? jarr["password"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy/MM/dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;
                DataTable dt = new DataTable();
                sql=$@"update t_tsm_sig set pwd='{password}',modifiedby='{user}',modifieddate='{date}' , modifiedtime='{time}' where username='{username}'";
                DB.ExecuteNonQuery(sql);
                DB.Commit();  
                ret.IsSuccess = true;
                ret.ErrMsg = "Updated Successfully";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetImageFromDatabase(object OBJ)
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
                string username = jarr.ContainsKey("username") ? jarr["username"].ToString() : "";
                string password = jarr.ContainsKey("password") ? jarr["password"].ToString() : "";
                string sql = string.Empty;
                byte[] imageData = null; 
                sql = $@"select image from t_tsm_sig where username='{username}' and  pwd='{password}' ";
                DataTable dt = DB.GetDataTable(sql);
                DB.Commit();
                if (dt != null && dt.Rows.Count > 0)
                {
                    object imageObj = dt.Rows[0]["IMAGE"];

                    if (imageObj != DBNull.Value)
                    {
                        imageData = (byte[])imageObj;
                    }
                    ret.IsSuccess = true;
                    ret.RetData = Convert.ToBase64String(imageData);


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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Add_SkillScoreData(object OBJ)
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
                string Dept = jarr.ContainsKey("Dept") ? jarr["Dept"].ToString() : "";
                string Process = jarr.ContainsKey("Process") ? jarr["Process"].ToString() : "";
                string Trainer = jarr.ContainsKey("Trainer") ? jarr["Trainer"].ToString() : "";
                string Model = jarr.ContainsKey("Model") ? jarr["Model"].ToString() : "";
                string IE_st_time = jarr.ContainsKey("IE_st_time") ? jarr["IE_st_time"].ToString() : "";
                string FirststCycle = jarr.ContainsKey("FirststCycle") ? jarr["FirststCycle"].ToString() : "";
                string SecondCycle = jarr.ContainsKey("SecondCycle") ? jarr["SecondCycle"].ToString() : "";
                string ThirdCycle = jarr.ContainsKey("ThirdCycle") ? jarr["ThirdCycle"].ToString() : "";
                string FourthCycle = jarr.ContainsKey("FourthCycle") ? jarr["FourthCycle"].ToString() : "";
                string FifthCycle = jarr.ContainsKey("FifthCycle") ? jarr["FifthCycle"].ToString() : "";
                string TCT = jarr.ContainsKey("TCT") ? jarr["TCT"].ToString() : "";
                string AvgCycleTime = jarr.ContainsKey("AvgCycleTime") ? jarr["AvgCycleTime"].ToString() : "";
                string IEScore = jarr.ContainsKey("IEScore") ? jarr["IEScore"].ToString() : "";
                string QIPScore = jarr.ContainsKey("QIPScore") ? jarr["QIPScore"].ToString() : "";
                string Qualitypairs = jarr.ContainsKey("Qualitypairs") ? jarr["Qualitypairs"].ToString() : "";
                string Totalpairs = jarr.ContainsKey("Totalpairs") ? jarr["Totalpairs"].ToString() : "";
                string TotalScore = jarr.ContainsKey("TotalScore") ? jarr["TotalScore"].ToString() : "";
                string SkillLevel = jarr.ContainsKey("SkillLevel") ? jarr["SkillLevel"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy/MM/dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间 

                #region Old Logic Dont delete It.
                //byte[] image1 = jarr["image1"] != null ? Convert.FromBase64String(jarr["image1"].ToString()) : new byte[0];
                //byte[] image2 = jarr["image2"] != null ? Convert.FromBase64String(jarr["image2"].ToString()) : new byte[0];
                //byte[] image3 = jarr["image3"] != null ? Convert.FromBase64String(jarr["image3"].ToString()) : new byte[0];
                //byte[] image4 = jarr["image4"] != null ? Convert.FromBase64String(jarr["image4"].ToString()) : new byte[0];
                //byte[] image5 = jarr["image5"] != null ? Convert.FromBase64String(jarr["image5"].ToString()) : new byte[0];
                //byte[] image6 = jarr["image6"] != null ? Convert.FromBase64String(jarr["image6"].ToString()) : new byte[0];
                //byte[] image7 = jarr["image7"] != null ? Convert.FromBase64String(jarr["image7"].ToString()) : new byte[0];
                //byte[] image8 = jarr["image8"] != null ? Convert.FromBase64String(jarr["image8"].ToString()) : new byte[0];

                //string TRAINER_SIG_DATE = jarr.ContainsKey("TRAINER_SIG_DATE") ? jarr["TRAINER_SIG_DATE"].ToString() : "";
                //string OPERATOR_SIG_DATE = jarr.ContainsKey("OPERATOR_SIG_DATE") ? jarr["OPERATOR_SIG_DATE"].ToString() : "";
                //string IE_SPECIALIST_SIG_DATE = jarr.ContainsKey("IE_SPECIALIST_SIG_DATE") ? jarr["IE_SPECIALIST_SIG_DATE"].ToString() : "";
                //string QIP_INCHARGE_SIG_DATE = jarr.ContainsKey("QIP_INCHARGE_SIG_DATE") ? jarr["QIP_INCHARGE_SIG_DATE"].ToString() : "";
                //string LINE_SUPERVISOR_SIG_DATE = jarr.ContainsKey("LINE_SUPERVISOR_SIG_DATE") ? jarr["LINE_SUPERVISOR_SIG_DATE"].ToString() : "";
                //string PLANT_INCHARGE_SIG_DATE = jarr.ContainsKey("PLANT_INCHARGE_SIG_DATE") ? jarr["PLANT_INCHARGE_SIG_DATE"].ToString() : "";
                //string ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE = jarr.ContainsKey("ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE") ? jarr["ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE"].ToString() : "";
                //string SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE = jarr.ContainsKey("SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE") ? jarr["SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE"].ToString() : "";
                //string IESCORE_DATE = jarr.ContainsKey("IESCORE_DATE") ? jarr["IESCORE_DATE"].ToString() : "";
                //string QIPSCORE_DATE = jarr.ContainsKey("QIPSCORE_DATE") ? jarr["QIPSCORE_DATE"].ToString() : "";
                //string PRODUCTIONSCORE_DATE = jarr.ContainsKey("PRODUCTIONSCORE_DATE") ? jarr["PRODUCTIONSCORE_DATE"].ToString() : "";
                //string TRAININGCENTRESCORE_DATE = jarr.ContainsKey("TRAININGCENTRESCORE_DATE") ? jarr["TRAININGCENTRESCORE_DATE"].ToString() : "";



                //string datetime = DateTime.Now.ToString();//时间 
                //if (string.IsNullOrEmpty(IEScore))
                //{
                //    IESCORE_DATE = ""; 
                //}
                //else if(string.IsNullOrEmpty(IESCORE_DATE))
                //{
                //    IESCORE_DATE = datetime;
                //}
                //if (string.IsNullOrEmpty(Qualitypairs))
                //{
                //    PRODUCTIONSCORE_DATE = "";
                //}
                //else if (string.IsNullOrEmpty(PRODUCTIONSCORE_DATE))
                //{
                //    PRODUCTIONSCORE_DATE = datetime;
                //}
                //if (string.IsNullOrEmpty(QIPScore))
                //{
                //    QIPSCORE_DATE = "";
                //}
                //else if (string.IsNullOrEmpty(QIPSCORE_DATE))
                //{
                //    QIPSCORE_DATE = datetime;
                //}
                //if (string.IsNullOrEmpty(Totalpairs))
                //{
                //    TRAININGCENTRESCORE_DATE = "";
                //}
                //else if (string.IsNullOrEmpty(TRAININGCENTRESCORE_DATE))
                //{
                //    TRAININGCENTRESCORE_DATE = datetime;
                //} 
                //string sql1 = string.Empty;
                //string sql2 = string.Empty;
                //string sql3 = string.Empty;
                //sql1 = $@"select * from t_tsm_skill_score_evaluation_m where barcode='{Barcode}' and process='{Process}'";
                //DataTable dt1 = DB.GetDataTable(sql1);
                //if(dt1.Rows.Count>0)
                //{
                //    sql2 = "update t_tsm_skill_score_evaluation_m set Name=:V_Name, Dept=:V_Dept,Trainer=:V_Trainer, Model=:V_Model, IE_st_time=:V_IE_st_time, FirstCycle=:V_FirststCycle,SecondCycle=:V_SecondCycle,ThirdCycle=:V_ThirdCycle," +
                //  "FourthCycle=:V_FourthCycle,FifthCycle=:V_FifthCycle,TCT=:V_TCT,AvgCycleTime=:V_AvgCycleTime,IEScore=:V_IEScore,QIPScore=:V_QIPScore,Qualitypairs=:V_Qualitypairs,Totalpairs=:V_Totalpairs,TotalScore=:V_TotalScore,SkillLevel=:V_SkillLevel," +
                //  "TRAINER_SIG_DATE=:V_TRAINER_SIG_DATE,OPERATOR_SIG_DATE=:V_OPERATOR_SIG_DATE,IE_SPECIALIST_SIG_DATE=:V_IE_SPECIALIST_SIG_DATE,QIP_INCHARGE_SIG_DATE=:V_QIP_INCHARGE_SIG_DATE,LINE_SUPERVISOR_SIG_DATE=:V_LINE_SUPERVISOR_SIG_DATE,PLANT_INCHARGE_SIG_DATE=:V_PLANT_INCHARGE_SIG_DATE," +
                //  "ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE=:V_ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE,SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE=:V_SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE,IESCORE_DATE=:V_IESCORE_DATE,QIPSCORE_DATE=:V_QIPSCORE_DATE,PRODUCTIONSCORE_DATE=:V_PRODUCTIONSCORE_DATE,TRAININGCENTRESCORE_DATE=:V_TRAININGCENTRESCORE_DATE," +
                //  "TRAINER_SIG=:V_image1,OPERATOR_SIG=:V_image2,IE_SPECIALIST_SIG=:V_image3,QIP_INCHARGE_SIG=:V_image4,LINE_SUPERVISOR_SIG=:V_image5,PLANT_INCHARGE_SIG=:V_image6,ASSEMBLY_TRAINING_SUPERVISOR_SIG=:V_image7,SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG=:V_image8,MODIFIEDBY=:V_user," +
                //  "MODIFIEDDATE=:V_date,MODIFIEDTIME=:V_time WHERE barcode= :V_Barcode and process=:V_Process";
                //    var insertParamsDic = new Dictionary<string, object>(); 
                //    insertParamsDic.Add("V_Name", Name);
                //    insertParamsDic.Add("V_Dept", Dept); 
                //    insertParamsDic.Add("V_Trainer", Trainer);
                //    insertParamsDic.Add("V_Model", Model);
                //    insertParamsDic.Add("V_IE_st_time", IE_st_time);
                //    insertParamsDic.Add("V_FirststCycle", FirststCycle);
                //    insertParamsDic.Add("V_SecondCycle", SecondCycle);
                //    insertParamsDic.Add("V_ThirdCycle", ThirdCycle);
                //    insertParamsDic.Add("V_FourthCycle", FourthCycle);
                //    insertParamsDic.Add("V_FifthCycle", FifthCycle);
                //    insertParamsDic.Add("V_TCT", TCT);
                //    insertParamsDic.Add("V_AvgCycleTime", AvgCycleTime);
                //    insertParamsDic.Add("V_IEScore", IEScore);
                //    insertParamsDic.Add("V_QIPScore", QIPScore);
                //    insertParamsDic.Add("V_Qualitypairs", Qualitypairs);
                //    insertParamsDic.Add("V_Totalpairs", Totalpairs);
                //    insertParamsDic.Add("V_TotalScore", TotalScore);
                //    insertParamsDic.Add("V_SkillLevel", SkillLevel);
                //    insertParamsDic.Add("V_TRAINER_SIG_DATE", TRAINER_SIG_DATE);
                //    insertParamsDic.Add("V_OPERATOR_SIG_DATE", OPERATOR_SIG_DATE);
                //    insertParamsDic.Add("V_IE_SPECIALIST_SIG_DATE", IE_SPECIALIST_SIG_DATE);
                //    insertParamsDic.Add("V_QIP_INCHARGE_SIG_DATE", QIP_INCHARGE_SIG_DATE);
                //    insertParamsDic.Add("V_LINE_SUPERVISOR_SIG_DATE", LINE_SUPERVISOR_SIG_DATE);
                //    insertParamsDic.Add("V_PLANT_INCHARGE_SIG_DATE", PLANT_INCHARGE_SIG_DATE);
                //    insertParamsDic.Add("V_ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE", ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE);
                //    insertParamsDic.Add("V_SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE", SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE);
                //    insertParamsDic.Add("V_IESCORE_DATE", IESCORE_DATE);
                //    insertParamsDic.Add("V_QIPSCORE_DATE", QIPSCORE_DATE);
                //    insertParamsDic.Add("V_PRODUCTIONSCORE_DATE", PRODUCTIONSCORE_DATE);
                //    insertParamsDic.Add("V_TRAININGCENTRESCORE_DATE", TRAININGCENTRESCORE_DATE);
                //    insertParamsDic.Add("V_image1", image1);
                //    insertParamsDic.Add("V_image2", image2);
                //    insertParamsDic.Add("V_image3", image3);
                //    insertParamsDic.Add("V_image4", image4);
                //    insertParamsDic.Add("V_image5", image5);
                //    insertParamsDic.Add("V_image6", image6);
                //    insertParamsDic.Add("V_image7", image7);
                //    insertParamsDic.Add("V_image8", image8);
                //    insertParamsDic.Add("V_user", user);
                //    insertParamsDic.Add("V_date", date);
                //    insertParamsDic.Add("V_time", time);
                //    insertParamsDic.Add("V_Barcode", Barcode);
                //    insertParamsDic.Add("V_Process", Process); 


                //    int count = DB.ExecuteNonQuery(sql2, insertParamsDic);

                //}
                //else
                //{
                //    sql3 = "INSERT INTO t_tsm_skill_score_evaluation_m (Barcode, Name, Dept, Process, Trainer, Model, IE_st_time, FirstCycle,SecondCycle,ThirdCycle," +
                //   "FourthCycle,FifthCycle,TCT,AvgCycleTime,IEScore,QIPScore,Qualitypairs,Totalpairs,TotalScore,SkillLevel," +
                //   "TRAINER_SIG,OPERATOR_SIG,IE_SPECIALIST_SIG,QIP_INCHARGE_SIG,LINE_SUPERVISOR_SIG,PLANT_INCHARGE_SIG,ASSEMBLY_TRAINING_SUPERVISOR_SIG,SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG,CREATEDBY,CREATEDDATE,CREATEDTIME," +
                //   "TRAINER_SIG_DATE,OPERATOR_SIG_DATE,IE_SPECIALIST_SIG_DATE,QIP_INCHARGE_SIG_DATE,LINE_SUPERVISOR_SIG_DATE,PLANT_INCHARGE_SIG_DATE,ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE,SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE," +
                //   "IESCORE_DATE,QIPSCORE_DATE,PRODUCTIONSCORE_DATE,TRAININGCENTRESCORE_DATE)" +
                //   "VALUES(:V_Barcode, :V_Name, :V_Dept, :V_Process, :V_Trainer, :V_Model, :V_IE_st_time, :V_FirststCycle,:V_SecondCycle, :V_ThirdCycle, :V_FourthCycle, :V_FifthCycle, :V_TCT, " +
                //   ":V_AvgCycleTime, :V_IEScore, :V_QIPScore,:V_Qualitypairs, :V_Totalpairs, :V_TotalScore, :V_SkillLevel, :V_image1, :V_image2, :V_image3, :V_image4,:V_image5, :V_image6, :V_image7, :V_image8,:V_user,:V_date,:V_time," +
                //   ":V_TRAINER_SIG_DATE,:V_OPERATOR_SIG_DATE,:V_IE_SPECIALIST_SIG_DATE,:V_QIP_INCHARGE_SIG_DATE,:V_LINE_SUPERVISOR_SIG_DATE,:V_PLANT_INCHARGE_SIG_DATE,:V_ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE,:V_SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE," +
                //   ":V_IESCORE_DATE,:V_QIPSCORE_DATE,:V_PRODUCTIONSCORE_DATE,:V_TRAININGCENTRESCORE_DATE)"; 
                //    var insertParamsDic = new Dictionary<string, object>();
                //    insertParamsDic.Add("V_Barcode", Barcode);
                //    insertParamsDic.Add("V_Name", Name);
                //    insertParamsDic.Add("V_Dept", Dept);
                //    insertParamsDic.Add("V_Process", Process);
                //    insertParamsDic.Add("V_Trainer", Trainer);
                //    insertParamsDic.Add("V_Model", Model);
                //    insertParamsDic.Add("V_IE_st_time", IE_st_time);
                //    insertParamsDic.Add("V_FirststCycle", FirststCycle);
                //    insertParamsDic.Add("V_SecondCycle", SecondCycle);
                //    insertParamsDic.Add("V_ThirdCycle", ThirdCycle);
                //    insertParamsDic.Add("V_FourthCycle", FourthCycle);
                //    insertParamsDic.Add("V_FifthCycle", FifthCycle);
                //    insertParamsDic.Add("V_TCT", TCT);
                //    insertParamsDic.Add("V_AvgCycleTime", AvgCycleTime);
                //    insertParamsDic.Add("V_IEScore", IEScore);
                //    insertParamsDic.Add("V_QIPScore", QIPScore);
                //    insertParamsDic.Add("V_Qualitypairs", Qualitypairs);
                //    insertParamsDic.Add("V_Totalpairs", Totalpairs);
                //    insertParamsDic.Add("V_TotalScore", TotalScore);
                //    insertParamsDic.Add("V_SkillLevel", SkillLevel);
                //    insertParamsDic.Add("V_image1", image1);
                //    insertParamsDic.Add("V_image2", image2);
                //    insertParamsDic.Add("V_image3", image3);
                //    insertParamsDic.Add("V_image4", image4);
                //    insertParamsDic.Add("V_image5", image5);
                //    insertParamsDic.Add("V_image6", image6);
                //    insertParamsDic.Add("V_image7", image7);
                //    insertParamsDic.Add("V_image8", image8);
                //    insertParamsDic.Add("V_user", user);
                //    insertParamsDic.Add("V_date", date);
                //    insertParamsDic.Add("V_time", time);
                //    insertParamsDic.Add("V_TRAINER_SIG_DATE", TRAINER_SIG_DATE);
                //    insertParamsDic.Add("V_OPERATOR_SIG_DATE", OPERATOR_SIG_DATE);
                //    insertParamsDic.Add("V_IE_SPECIALIST_SIG_DATE", IE_SPECIALIST_SIG_DATE);
                //    insertParamsDic.Add("V_QIP_INCHARGE_SIG_DATE", QIP_INCHARGE_SIG_DATE);
                //    insertParamsDic.Add("V_LINE_SUPERVISOR_SIG_DATE", LINE_SUPERVISOR_SIG_DATE);
                //    insertParamsDic.Add("V_PLANT_INCHARGE_SIG_DATE", PLANT_INCHARGE_SIG_DATE);
                //    insertParamsDic.Add("V_ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE", ASSEMBLY_TRAINING_SUPERVISOR_SIG_DATE);
                //    insertParamsDic.Add("V_SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE", SENIOR_SUPERVISOR_OF_TRAINING_DEPT_SIG_DATE);
                //    insertParamsDic.Add("V_IESCORE_DATE", IESCORE_DATE);
                //    insertParamsDic.Add("V_QIPSCORE_DATE", QIPSCORE_DATE);
                //    insertParamsDic.Add("V_PRODUCTIONSCORE_DATE", PRODUCTIONSCORE_DATE);
                //    insertParamsDic.Add("V_TRAININGCENTRESCORE_DATE", TRAININGCENTRESCORE_DATE);
                //    int count = DB.ExecuteNonQuery(sql3, insertParamsDic);

                //} 
                //if(!string.IsNullOrEmpty(SkillLevel))
                //{
                //    //string sql4 = $@"select * from t_tsm_emp_skill_m where emp_no = '{Barcode}' and skill_name = '{Process}'";
                //    //DataTable dt = DB.GetDataTable(sql4);
                //    //if(dt.Rows.Count>0)
                //    //{
                //    //    string sql5 = $@"update t_tsm_emp_skill_m set skill_score='{SkillLevel}',modifiedby='{user}',modifieddate='{date}',modifiedtime='{time}' where emp_no = '{Barcode}' and skill_name = '{Process}'";
                //    //    DB.ExecuteNonQuery(sql5); 
                //    //}
                //    //else
                //    //{
                //    //    string sql6 = $@"insert into t_tsm_emp_skill_m (emp_no,skill_name,skill_score,createdby,createddate,createdtime) values('{Barcode}','{Process}','{SkillLevel}','{user}','{date}','{time}')";
                //    //    DB.ExecuteNonQuery(sql6); 
                //    //} 

                //        string sql6 = $@"insert into t_tsm_emp_skill_d (emp_no,skill_name,skill_score,createdby,createddate,createdtime) values('{Barcode}','{Process}','{SkillLevel}','{user}','{date}','{time}')";
                //        DB.ExecuteNonQuery(sql6);

                //}

                #endregion;

                DB.AddProcedureParameter("P_Barcode", Barcode,DbType.String, ParameterDirection.Input);
                DB.AddProcedureParameter("P_Name", Name, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Dept", Dept, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Process", Process, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Trainer", Trainer, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Model", Model, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_IE_st_time", IE_st_time, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_FirststCycle", FirststCycle, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_SecondCycle", SecondCycle, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_ThirdCycle", ThirdCycle, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_FourthCycle", FourthCycle, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_FifthCycle", FifthCycle, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_TCT", TCT, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_AvgCycleTime", AvgCycleTime, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_IEScore", IEScore, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_QIPScore", QIPScore, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Qualitypairs", Qualitypairs, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_Totalpairs", Totalpairs, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("P_SkillLevel", SkillLevel, DbType.String, ParameterDirection.Input);
                DB.AddProcedureParameter("P_TotalScore", TotalScore, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("p_createdby", user, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("p_createddate", date, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("p_createdtime", time, DbType.String,ParameterDirection.Input);
                DB.AddProcedureParameter("p_count1", "", DbType.String,ParameterDirection.Output);
                DB.AddProcedureParameter("p_count2", "", DbType.String, ParameterDirection.Output);
              
                DB.ExecuteProcedure("sp_emp_skill_score");
               
                #region test
                //string constrerp = "Data Source=(DESCRIPTION=" + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.3.0.240)(PORT=1521)))" + "(CONNECT_DATA=(SERVICE_NAME = MESTEST01)));" + "User Id=mes00;Password=dbmes00;";


                //    OracleConnection con = new OracleConnection(constrerp);
                //    con.Open();

                //    OracleCommand cmd = new OracleCommand();
                //    cmd.Connection = con;
                //    cmd.CommandType = CommandType.StoredProcedure;
                //    cmd.CommandText = "sp_emp_skill_score";
                //    cmd.Parameters.Add("P_Barcode", OracleDbType.Varchar2).Value = Barcode;
                //    cmd.Parameters.Add("P_Name", OracleDbType.Varchar2).Value = Name;
                //cmd.Parameters.Add("P_Dept", OracleDbType.Varchar2).Value = Dept;
                //cmd.Parameters.Add("P_Process", OracleDbType.Varchar2).Value = Process;
                //cmd.Parameters.Add("P_Trainer", OracleDbType.Varchar2).Value = Trainer;
                //cmd.Parameters.Add("P_Model", OracleDbType.Varchar2).Value = Model;
                //cmd.Parameters.Add("P_IE_st_time", OracleDbType.Varchar2).Value = IE_st_time;
                //cmd.Parameters.Add("P_FirststCycle", OracleDbType.Varchar2).Value = FirststCycle;
                //cmd.Parameters.Add("P_SecondCycle", OracleDbType.Varchar2).Value = SecondCycle;
                //cmd.Parameters.Add("P_ThirdCycle", OracleDbType.Varchar2).Value = ThirdCycle;
                //cmd.Parameters.Add("P_FourthCycle", OracleDbType.Varchar2).Value = FourthCycle;
                //cmd.Parameters.Add("P_FifthCycle", OracleDbType.Varchar2).Value = FifthCycle;
                //cmd.Parameters.Add("P_TCT", OracleDbType.Varchar2).Value = TCT;
                //cmd.Parameters.Add("P_AvgCycleTime", OracleDbType.Varchar2).Value = AvgCycleTime;
                //cmd.Parameters.Add("P_IEScore", OracleDbType.Varchar2).Value = IEScore;
                //cmd.Parameters.Add("P_QIPScore", OracleDbType.Varchar2).Value = QIPScore;
                //cmd.Parameters.Add("P_Qualitypairs", OracleDbType.Varchar2).Value = Qualitypairs;
                //cmd.Parameters.Add("P_Totalpairs", OracleDbType.Varchar2).Value = Totalpairs;
                //cmd.Parameters.Add("P_TotalScore", OracleDbType.Varchar2).Value = TotalScore;
                //cmd.Parameters.Add("P_SkillLevel", OracleDbType.Varchar2).Value = SkillLevel;
                //cmd.Parameters.Add("p_createdby", OracleDbType.Varchar2).Value = user;
                //cmd.Parameters.Add("p_createddate", OracleDbType.Varchar2).Value = date;
                //cmd.Parameters.Add("p_createdtime", OracleDbType.Varchar2).Value = time;
                //cmd.Parameters.Add("p_count", OracleDbType.Varchar2).Direction = ParameterDirection.Output;
                //    OracleDataAdapter da = new OracleDataAdapter(cmd); 
                #endregion;

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEmp_SkillDetails(object OBJ)
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
                string ProcessType = jarr.ContainsKey("ProcessType") ? jarr["ProcessType"].ToString() : "";
                string ProcessName = jarr.ContainsKey("ProcessName") ? jarr["ProcessName"].ToString() : "";
                string StartDate = jarr.ContainsKey("StartDate") ? jarr["StartDate"].ToString() : "";
                string EndDate = jarr.ContainsKey("EndDate") ? jarr["EndDate"].ToString() : "";
                string Training_Type = jarr.ContainsKey("Training_Type") ? jarr["Training_Type"].ToString() : "";
                string sql = string.Empty;
                string where = string.Empty;

                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"AND e.Barcode = '{Barcode}'";
                }

                if (!string.IsNullOrWhiteSpace(ProcessName))
                {
                    where += $@"AND e.PROCESS = '{ProcessName}'";
                }
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {
                    where += $@"AND p.PROCESS_TYPE = '{ProcessType}'";
                }
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {  
                    where += $@"AND p.PROCESS_TYPE = '{ProcessType}'";
                }
                if (!string.IsNullOrWhiteSpace(Training_Type))
                {
                    where += $@"AND s.Training_Type = '{Training_Type}'";
                }
                if (Training_Type == "New Employee Training")
                {
                    string sql3 = $@"
        SELECT 
            e.barcode,         
            e.name,         
            e.dept,         
            e.process,         
            '5' AS pbs,
            e.model,
            e.firstcycle,         
            e.secondcycle,         
            e.thirdcycle,         
            e.fourthcycle,         
            e.fifthcycle,         
            e.TCT,         
            e.avgcycletime,         
            e.ie_st_time,         
            e.iescore,         
            e.qipscore,         
            e.Qualitypairs,         
            e.Totalpairs,         
            e.totalscore,         
            e.skilllevel,
            p.process_type,
            s.training_type
        FROM 
            t_tsm_skill_score_evaluation_d
e
        JOIN 
            t_tsm_processlist p ON e.process = p.name
        JOIN 
            t_tsm_emp_registration s ON e.barcode = s.emp_no
        WHERE 1=1 
            {where} 
            AND e.createddate BETWEEN '{StartDate}' AND '{EndDate}'";

                    DataTable dt1 = DB.GetDataTable(sql3);
                    Dictionary<string, object> dic = new Dictionary<string, object>
    {
                         { "Data", dt1 }
    };
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    DB.Commit();
                }
                else
                {
                    string sql3 = $@"
        SELECT 
            e.barcode,         
            e.name,         
            e.dept,         
            e.process,         
            '5' AS pbs,
            e.model,
            e.firstcycle,         
            e.secondcycle,         
            e.thirdcycle,         
            e.fourthcycle,         
            e.fifthcycle,         
            e.TCT,         
            e.avgcycletime,         
            e.ie_st_time,         
            e.iescore,         
            e.qipscore,         
            e.Qualitypairs,         
            e.Totalpairs,         
            e.totalscore,         
            e.skilllevel,
            p.process_type
        FROM 
            t_tsm_skill_score_evaluation_d e
        JOIN 
            t_tsm_processlist p ON e.process = p.name
        WHERE 1=1 
            {where} 
            AND e.createddate BETWEEN '{StartDate}' AND '{EndDate}'";

                    DataTable dt2 = DB.GetDataTable(sql3);
                    Dictionary<string, object> dic = new Dictionary<string, object>
    {
        { "Data", dt2 }
    };
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEmp_SkillScoreDetails(object OBJ)
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
                string ProcessName = jarr.ContainsKey("ProcessName") ? jarr["ProcessName"].ToString() : ""; 
                string sql = string.Empty;
                string where = string.Empty;

                if (!string.IsNullOrWhiteSpace(Barcode))
                {
                    where += $@"AND Barcode = '{Barcode}'";
                }
                if (!string.IsNullOrWhiteSpace(ProcessName))
                {
                    where += $@"AND PROCESS = '{ProcessName}'";
                }

                //                DataTable dt2 = DB.GetDataTable($@"select *
                //  from t_tsm_skill_score_evaluation_d where 
                //1=1 {where}  and createddate > to_char(sysdate-30, 'yyyy/MM/dd')");

                DataTable dt2 = DB.GetDataTable($@"select * from t_tsm_skill_score_evaluation_m where 1=1 {where}");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt2);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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

