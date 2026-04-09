using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SJ_SMAPI
{
    class MPAC_Allocation
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Requested_List(object OBJ)
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
                string Production_plant = jarr.ContainsKey("Production_plant") ? jarr["Production_plant"].ToString() : "";
                string where = string.Empty;
                if(!string.IsNullOrEmpty(Production_plant))
                {
                    where += $@"and a.plant = '{Production_plant}'";
                }

                string sql = $@"select a.plant,
       a.departmentcode as Source_dept,
       a.barcode,
       a.emp_name,
       a.skill_name,
       a.replaced_barcode,
       a.replaced_emp_name,
       a.replaced_department,
       a.REPLACED_SKILL_NAME,
       a.REMARKS
       from t_sm_productionrequest_d a 
       inner join t_sm_productionrequest_m b on a.DEPARTMENTCODE=b.DEPARTMENTCODE and to_char(a.createddate,'yyyy/MM/dd')=to_char(b.createddate,'yyyy/MM/dd') 
       where  1=1 {where} and b.signature4='1'
                 and a.leavedate = to_char(sysdate, 'yyyy/MM/dd') order by a.plant,a.departmentcode";

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Employee(object OBJ)
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
                string Skill_Name = jarr.ContainsKey("Skill_Name") ? jarr["Skill_Name"].ToString() : "";
                string sql = $@"select a.emp_no, a.emp_name, a.dept_name
                   from t_oa_empmain a 
                  where a.dept_name like '%MPAC%'
                    and a.emp_no not in
                        (select a.REPLACED_BARCODE
                           from t_sm_productionrequest_d a
                          where a.leavedate = to_char(sysdate, 'yyyy/MM/dd')
                            and a.REPLACED_BARCODE is not null
                            union all
                            select emp_no from t_sm_mpac_absent_m a where a.ABSENT_DATE = to_char(sysdate, 'yyyy/MM/dd'))
                    and a.emp_no = '{Barcode}'
                    ";
                DataTable dt = DB.GetDataTable(sql);
                if(dt.Rows.Count>0)
                {
                  //  string sql3 = $@"select a.emp_no, a.emp_name, a.dept_name,b.skill_name
                  // from t_oa_empmain a inner join t_tsm_emp_skill_m b on a.emp_no=b.emp_no
                  //where a.dept_name like '%MPAC%'
                  //  and a.emp_no = '{Barcode}' and  b.skill_name='{Skill_Name}'
                  //  ";
                  //  DataTable dt3 = DB.GetDataTable(sql3);
                  //  if (dt3.Rows.Count > 0)
                  //  {

                        string sql2 = $@"select a.emp_no, a.emp_name, a.dept_name
                   from t_oa_empmain a
                  where a.dept_name like '%MPAC%'
                    and a.emp_no = '{Barcode}'
                    and a.emp_no not in
                        (select a.REPLACED_BARCODE
                           from t_sm_productionrequest_d a
                          where a.leavedate = to_char(sysdate, 'yyyy/MM/dd')
                            and a.REPLACED_BARCODE is not null)";
                        DataTable dt2 = DB.GetDataTable(sql2);
                        if (dt2.Rows.Count > 0)
                        {
                            Dictionary<string, object> dic = new Dictionary<string, object>();
                            dic.Add("Data", dt2);
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                            ret.IsSuccess = true;
                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = $@"Already assigned";
                        }
                    //}
                    //else
                    //{
                    //    ret.IsSuccess = false;
                    //    ret.ErrMsg = $@"Skill Data is not matching";
                    //}
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $@"No Such Employee Available";
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
                string sql = $@"select NAME from t_tsm_processlist a where a.skill_type='Key skill'"; 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Manual_Allocate_Employee(object OBJ)
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
                string dt1_json = jarr["dt1"].ToString();
                DataTable dt1 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt1_json, (typeof(DataTable)));
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                foreach (DataRow dr in dt1.Rows)
                {
                    
                        string sql = $@"update t_sm_productionrequest_d a set a.replaced_barcode='{dr["replaced_barcode"]}',a.replaced_emp_name='{dr["replaced_emp_name"]}',
                        a.replaced_department='{dr["replaced_department"]}', a.REPLACED_SKILL_NAME='{dr["REPLACED_SKILL_NAME"]}',a.remarks='{dr["REMARKS"]}',a.REPLACEDBY='{user}',a.REPLACEDDATE='{date}',a.REPLACEDTIME='{time}' where a.barcode='{dr["barcode"]}' and a.LEAVEDATE=to_char(sysdate,'yyyy/MM/dd')";
                        DB.ExecuteNonQuery(sql);
                   
                }
                 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Auto_Allocate_Employee(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

              
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string dt1_json = jarr["dt1"].ToString();
                DataTable dt1 = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt1_json, (typeof(DataTable)));
                for (int i=0;i<dt1.Rows.Count-1;i++)
                {
                    DB.Open();
                    DB.BeginTransaction();
                    string Barcode = dt1.Rows[i]["replaced_barcode"].ToString();
                    string SKILL_NAME = dt1.Rows[i]["SKILL_NAME"].ToString();
                    if (string.IsNullOrEmpty(Barcode))
                    {
                        string sql1 = $@"select a.emp_no, a.emp_name, a.dept_name
                   from t_oa_empmain a inner join t_tsm_emp_skill_m b on a.emp_no=b.emp_no
                  where a.dept_name like '%MPAC%'
                    and a.emp_no not in
                        (select a.REPLACED_BARCODE
                           from t_sm_productionrequest_d a
                          where a.leavedate = to_char(sysdate, 'yyyy/MM/dd')
                            and a.REPLACED_BARCODE is not null
                            union all
                            select emp_no from t_sm_mpac_absent_m a where a.ABSENT_DATE = to_char(sysdate, 'yyyy/MM/dd'))
                           and b.skill_name='{SKILL_NAME}'";

                        DataTable dt = DB.GetDataTable(sql1);
                  
                        if(dt.Rows.Count>0)
                        {
                            string sql2 = $@"update t_sm_productionrequest_d a set a.replaced_barcode='{dt.Rows[0]["emp_no"]}',a.replaced_emp_name='{dt.Rows[0]["emp_name"]}',
                            a.replaced_department='{dt.Rows[0]["dept_name"]}' where a.barcode='{dt1.Rows[i]["barcode"]}' and a.LEAVEDATE=to_char(sysdate,'yyyy/MM/dd')";
                            int a = DB.ExecuteNonQuery(sql2);
                            DB.Commit();
                        }
                    }
                    DB.Close();
                }

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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_MPAC_Emp_Count(object OBJ)
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
                string ProcessType = jarr.ContainsKey("ProcessType") ? jarr["ProcessType"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {
                    where += $@"and a.dept_name like '%{ProcessType}%'";
                }
                string sql = string.Empty;
                string sql1 = $@"select count(*) from t_oa_empmain a where a.dept_name like '%MPAC%'{where}";
                int TotalCount = DB.GetInt32(sql1);
                string sql2 = $@"select count(*)
  from t_oa_empmain a
 where a.dept_name like '%MPAC%'
   and a.emp_no not in
       (select a.emp_no
          from t_sm_mpac_absent_m a
         where a.createddate = to_char(sysdate, 'yyyy/MM/dd')){where}";
                int OnlineCount = DB.GetInt32(sql2);
                string sql3 = $@" select count(*) from t_sm_mpac_absent_m a where a.createddate=to_char(sysdate,'yyyy/MM/dd'){where}";
                int OfflineCount = DB.GetInt32(sql3);
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("TotalCount", TotalCount);
                dic.Add("OnlineCount", OnlineCount);
                dic.Add("OfflineCount", OfflineCount);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_MPAC_Emp_List(object OBJ)
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
                string ProcessType = jarr.ContainsKey("ProcessType") ? jarr["ProcessType"].ToString() : "";
                string sql = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {
                    where += $@"and a.dept_name like '%{ProcessType}%'";
                }
                string sql1 = $@"select a.emp_no,a.emp_name,a.dept_name from t_oa_empmain a where a.dept_name like '%MPAC%'and a.emp_no not in
       (select a.emp_no
          from t_sm_mpac_absent_m a
         where a.createddate = to_char(sysdate, 'yyyy/MM/dd')){where} ";
                DataTable OnlineList = DB.GetDataTable(sql1);
                string sql2 = $@"select a.emp_no,a.emp_name,a.dept_name
          from t_sm_mpac_absent_m a
         where a.createddate = to_char(sysdate, 'yyyy/MM/dd'){where}";
                DataTable OfflineList = DB.GetDataTable(sql2);

                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("OnlineList", OnlineList);
                dic.Add("OfflineList", OfflineList);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Remove_MPAC_Emp(object OBJ)
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
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                DataTable dt = new DataTable();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {
                    where += $@"and a.dept_name like '%{ProcessType}%'";
                }
                dt = DB.GetDataTable($@"select a.emp_no,a.emp_name,a.dept_name from t_oa_empmain a where a.dept_name like '%MPAC%' and a.emp_no='{Barcode}' {where}");
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No such employee found";
                }
                else
                {
                    DataTable dt2 = new DataTable();

                    dt2 = DB.GetDataTable($@"select * from t_sm_mpac_absent_m where EMP_NO ='{Barcode}' and createddate='{date}'");
                    if (dt2.Rows.Count == 0)
                    {
                        sql = $@"insert into t_sm_mpac_absent_m(emp_no,emp_name,dept_name,ABSENT_DATE,createdby,createddate,createdtime)
                                values('{dt.Rows[0]["emp_no"]}','{dt.Rows[0]["emp_name"]}','{dt.Rows[0]["dept_name"]}','{date}',
                                        '{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "Removed Successfully";
                    }
                    else
                    {
                        sql = $@"delete from t_sm_mpac_absent_m a where a.emp_no='{dt.Rows[0]["emp_no"]}' and a.createddate='{date}'";
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
                string sql = $@"update t_sm_productionrequest_d
   set REPLACED_BARCODE    = '',
       REPLACED_EMP_NAME   = '',
       REPLACED_DEPARTMENT = '',
       REPLACED_SKILL_NAME = '',
       MODIFIEDBY          = '{user}',
       MODIFIEDDATE        = '{date}',
       MODIFIEDTIME        = '{time}'
 where LEAVEDATE = to_char(sysdate, 'yyyy/MM/dd')
   and BARCODE = '{Emp_No}'";
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Final_report(object OBJ)
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
                string Production_plant = jarr.ContainsKey("Production_plant") ? jarr["Production_plant"].ToString() : "";
                string SDate = jarr.ContainsKey("SDate") ? jarr["SDate"].ToString() : "";
                string EDate = jarr.ContainsKey("EDate") ? jarr["EDate"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(Production_plant))
                {
                    where += $@"and a.plant = '{Production_plant}'";
                }

                string sql = $@"select a.leavedate,a.plant,
       a.departmentcode as Source_dept,
       a.barcode,
       a.emp_name,
       a.skill_name,
       a.replaced_barcode,
       a.replaced_emp_name,
       a.replaced_department,
       a.replaced_skill_name,
       a.remarks
       from t_sm_productionrequest_d a 
       --inner join t_sm_productionrequest_m b on a.DEPARTMENTCODE=b.DEPARTMENTCODE and to_char(a.createddate,'yyyy/MM/dd')=to_char(b.createddate,'yyyy/MM/dd') 
       where  1=1 {where} --and  a.replaced_barcode is not null
                 and a.leavedate between '{SDate}' and '{EDate}' order by a.leavedate,a.plant,a.departmentcode";

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
