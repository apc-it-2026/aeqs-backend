using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Training_Emp_Attendance
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Remove_Trainig_Emp(object OBJ)
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
                string etime = "17:15:00";
                string sql = string.Empty;
                DataTable dt = new DataTable();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(ProcessType))
                {
                    where += $@"AND PROCESS_TYPE = '{ProcessType}'";
                }
                dt = DB.GetDataTable($@"select * from t_tsm_emp_registration a where a.emp_no='{Barcode}' and a.training_e_date >= to_char(sysdate,'yyyy/MM/dd'){where}");
                if (dt.Rows.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No such employee found";
                }
                else
                {
                    DataTable dt2 = new DataTable();

                    dt2 = DB.GetDataTable($@"select * from t_tsm_emp_attendance_m where EMP_NO ='{Barcode}' and createddate='{date}'");
                    if (dt2.Rows.Count == 0)
                    {
                        sql = $@"insert into t_tsm_emp_attendance_m(emp_no,emp_name,emp_dept,start_time,end_time,online_time,status,createdby,createddate,createdtime)
                                values('{dt.Rows[0]["emp_no"]}','{dt.Rows[0]["emp_name"]}','{dt.Rows[0]["DEPARTMENT"]}','{date} {time}','{date} {time}','0','1',
                                        '{user}','{date}','{time}')";
                        DB.ExecuteNonQuery(sql);
                        ret.IsSuccess = true;
                        ret.ErrMsg = "Removed Successfully"; 
                    }
                    else
                    {
                        dt2 = DB.GetDataTable($@"select * from t_tsm_emp_attendance_m where EMP_NO ='{Barcode}' and createddate='{date}' and status='0'"); 
                        if(dt2.Rows.Count > 0)
                        {
                            string start_time = dt2.Rows[0]["start_time"].ToString();
                            string end_time = date + " " + time;
                            DateTime start_time1 = DateTime.Parse(start_time);
                            DateTime end_time1 = DateTime.Parse(end_time);
                            TimeSpan online_time1 = end_time1 - start_time1;
                            if ((start_time1.TimeOfDay.Hours <= 12 && end_time1.TimeOfDay.Hours >= 13) || (start_time1.TimeOfDay.Hours <= 12 && end_time1.TimeOfDay.Hours >= 14) || (start_time1.TimeOfDay.Hours <= 13 && end_time1.TimeOfDay.Hours >= 14))
                            {
                                // Lunch break from 12:30 to 1:15
                                if (start_time1.TimeOfDay <= new TimeSpan(12, 30, 0) && end_time1.TimeOfDay >= new TimeSpan(13, 15, 0))
                                {
                                    online_time1 = online_time1.Subtract(new TimeSpan(0, 45, 0));
                                }
                                else
                                {
                                    online_time1 = online_time1.Subtract(new TimeSpan(0, 45, 0));
                                }

                            }
                            string online_time = Math.Round(online_time1.TotalHours,1).ToString();

                            sql = $@"update t_tsm_emp_attendance_m set end_time='{date} {time}',online_time='{online_time}',status='1',modifiedby='{user}',modifieddate='{date}',modifiedtime='{time}' where EMP_NO ='{Barcode}' and createddate='{date}'and status='0'";
                            DB.ExecuteNonQuery(sql);
                            ret.IsSuccess = false;
                            ret.ErrMsg = "Removed Sucessfully";
                        }
                        else
                        {
                            string start_time = date + " " + time;
                            string end_time = date + " " + etime;
                            DateTime start_time1 = DateTime.Parse(start_time);
                            DateTime end_time1 = DateTime.Parse(end_time);
                            TimeSpan online_time1 = end_time1 - start_time1;
                            if ((start_time1.TimeOfDay.Hours <= 12 && end_time1.TimeOfDay.Hours >= 13) || (start_time1.TimeOfDay.Hours <= 12 && end_time1.TimeOfDay.Hours >= 14) || (start_time1.TimeOfDay.Hours <= 13 && end_time1.TimeOfDay.Hours >= 14))
                            {
                                // Lunch break from 12:30 to 1:15
                                if (start_time1.TimeOfDay <= new TimeSpan(12, 30, 0) && end_time1.TimeOfDay >= new TimeSpan(13, 15, 0))
                                {
                                    online_time1 = online_time1.Subtract(new TimeSpan(0, 45, 0));
                                }
                                else
                                {
                                    online_time1 = online_time1.Subtract(new TimeSpan(0, 45, 0));
                                }

                            }
                            string online_time = Math.Round(online_time1.TotalHours,1).ToString();

                            sql = $@"insert into t_tsm_emp_attendance_m(emp_no,emp_name,emp_dept,start_time,end_time,online_time,status,createdby,createddate,createdtime)
                                values('{dt.Rows[0]["emp_no"]}','{dt.Rows[0]["emp_name"]}','{dt.Rows[0]["DEPARTMENT"]}','{date} {time}','{date} {etime}','{online_time}','0',
                                        '{user}','{date}','{time}')";
                            DB.ExecuteNonQuery(sql);
                            ret.IsSuccess = true;
                            ret.ErrMsg = "Added Successfully";
                        } 
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_TrainingEmp_Count(object OBJ)
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
                    where += $@"AND a.PROCESS_TYPE = '{ProcessType}'";
                }
                string sql = string.Empty;
                string sql1 = $@"select count(*) from t_tsm_emp_registration a where a.training_s_date<=to_char(sysdate,'yyyy/MM/dd') and a.training_e_date>=to_char(sysdate,'yyyy/MM/dd'){where}";
                int TotalCount = DB.GetInt32(sql1);
                //int OnlineCount = DB.GetInt32($@"select count(*) from t_tsm_emp_attendance_m a where a.createddate=to_char(sysdate,'yyyy/MM/dd') and status='0'"); 
                string sql2 = $@"select count(*) from t_tsm_emp_attendance_m b  inner join t_tsm_emp_registration a on a.emp_no=b.emp_no where b.createddate=to_char(sysdate,'yyyy/MM/dd') and b.status='0' and a.training_s_date<=to_char(sysdate,'yyyy/MM/dd') and a.training_e_date>=to_char(sysdate,'yyyy/MM/dd'){where}";
                int OnlineCount = DB.GetInt32(sql2);
                //int OfflineCount = TotalCount - OnlineCount;
                //int OfflineCount = DB.GetInt32($@"select count(distinct emp_no) from t_tsm_emp_attendance_m a where a.createddate=to_char(sysdate,'yyyy/MM/dd') and status='1'");
                string sql3 = $@" select count(distinct b.emp_no)
   from t_tsm_emp_attendance_m b
  inner join t_tsm_emp_registration a
     on a.emp_no = b.emp_no 
  where b.createddate = to_char(sysdate, 'yyyy/MM/dd')
    and b.status = '1'
    and a.training_s_date <= to_char(sysdate, 'yyyy/MM/dd')
    and a.training_e_date >= to_char(sysdate, 'yyyy/MM/dd')
    and a.emp_no not in
        (select b.emp_no
           from t_tsm_emp_attendance_m
          where b.createddate = to_char(sysdate, 'yyyy/MM/dd')
            and b.status = '0'
            ){where}";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOnlineOfflineList(object OBJ) 
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
                    where += $@"AND a.PROCESS_TYPE = '{ProcessType}'";
                }

                //               DataTable OnlineList = DB.GetDataTable($@"select a.emp_no, a.emp_name, a.emp_dept, b.position, a.start_time
                // from t_tsm_emp_attendance_m a
                //inner join t_tsm_emp_registration b
                //   on a.emp_no = b.emp_no
                //where a.createddate = to_char(sysdate, 'yyyy/MM/dd') and status='0' and b.training_e_date>=to_char(sysdate, 'yyyy/MM/dd')");

                //           DataTable OnlineList = DB.GetDataTable($@"select emp_no, emp_name, department, position
                // from t_tsm_emp_registration
                //where training_s_date <= to_char(sysdate, 'yyyy/MM/dd')
                //  and training_e_date >= to_char(sysdate, 'yyyy/MM/dd')");

                string sql1 = $@"select emp_no, emp_name, department, position
                 from t_tsm_emp_registration a
                where training_s_date <= to_char(sysdate, 'yyyy/MM/dd')
                  and training_e_date >= to_char(sysdate, 'yyyy/MM/dd')
                  and emp_no not in
                      (select emp_no
                         from t_tsm_emp_attendance_m
                        where createddate = to_char(sysdate, 'yyyy/MM/dd')and status = '1'
                      and emp_no  not in (select emp_no
                         from t_tsm_emp_attendance_m
                        where createddate = to_char(sysdate, 'yyyy/MM/dd')and status = '0')){where} ";
                DataTable OnlineList = DB.GetDataTable(sql1);
                string sql2 = $@"  select distinct a.emp_no,a.emp_name,b.emp_dept,a.position
   from t_tsm_emp_attendance_m b
  inner join t_tsm_emp_registration a
     on a.emp_no = b.emp_no 
  where b.createddate = to_char(sysdate, 'yyyy/MM/dd')
    and b.status = '1'
    and a.training_s_date <= to_char(sysdate, 'yyyy/MM/dd')
    and a.training_e_date >= to_char(sysdate, 'yyyy/MM/dd')
    and a.emp_no not in
        (select b.emp_no
           from t_tsm_emp_attendance_m b
          where b.createddate = to_char(sysdate, 'yyyy/MM/dd')
            and b.status = '0'
            ){where}";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Training_Attendance_Report(object OBJ)
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
                string Processtype = jarr.ContainsKey("Processtype") ? jarr["Processtype"].ToString() : "";
                string Sdate = jarr.ContainsKey("sdate") ? jarr["sdate"].ToString() : "";
                string Edate = jarr.ContainsKey("edate") ? jarr["edate"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string where = string.Empty;
                string where1 = string.Empty; 

                if (!string.IsNullOrEmpty(Barcode))
                {
                    where1 += $@" AND a.emp_no = '{Barcode}'";
                }
                if (!string.IsNullOrEmpty(Processtype))
                {
                    where1 += $@" and b.process_type='{Processtype}'";
                }
                DataTable dt = DB.GetDataTable($@" SELECT to_char(to_date('{Sdate}', 'yyyy/MM/dd') +
                                       LEVEL - 1,
                                       'yyyy/MM/dd') AS date_val
                         FROM DUAL
                       CONNECT BY LEVEL <=
                                  to_date('{Edate}', 'yyyy/MM/dd') -
                                  to_date('{Sdate}', 'yyyy/MM/dd') + 1");

                foreach (DataRow item in dt.Rows)
                {
                    where += "'"+item["date_val"].ToString()+"'" + "AS " + '"' + item["date_val"].ToString() + '"'+",";
                }

                where = where.TrimEnd(',');

                string where4 = string.Empty;
                string where5 = string.Empty;
                

                foreach (DataRow item in dt.Rows)
                {
                    where4 += "COALESCE((" + '"'+item["date_val"].ToString()+'"' + "), 0) AS " + '"' + item["date_val"].ToString() + '"' + ",";

                }

                foreach (DataRow item in dt.Rows)
                {
                    where5 += "COALESCE((" + '"' + item["date_val"].ToString() + '"' + "), 0)+";
                }
                where5 = where5.TrimEnd('+');

                string sql = $@"SELECT emp_no,emp_name,department,position,In_Date,Out_Date,Total_Training_days,Completed_Training_Days,training_type,process_name,status, {where4} 
                    {where5} as Summary  
    FROM(SELECT a.emp_no,
               a.emp_name,
               b.department,
               b.position,
               b.training_s_date as In_Date,
               b.training_e_date as Out_Date, 
               F_Training_Days(b.training_s_date,b.training_e_date) as Total_Training_days,
                case
                 when b.training_e_date > to_char(sysdate,'yyyy/MM/dd') then
                     F_Training_Days(b.training_s_date,to_char(sysdate,'yyyy/MM/dd')) 
                 else
                  F_Training_Days(b.training_s_date,b.training_e_date)
               end as Completed_Training_Days, 
               b.training_type,
               b.process_name,
               case
                 when b.training_e_date < '{date}' then 'Completed' else 'Under_Training' end as status, 
               a.createddate,
               sum(a.online_time) AS Working_Hours

          FROM t_tsm_emp_attendance_m a 
          left join t_tsm_emp_registration b on a.emp_no=b.emp_no
          and a.createddate between  b.training_s_date and b.training_e_date
         WHERE a.createddate BETWEEN '{Sdate}' AND '{Edate}'
          {where1}
         group by
         a.emp_no,
               a.emp_name,
               b.position,
               b.department,
               b.training_s_date,
               b.training_type,
               b.process_name, 
               a.createddate,
               b.training_e_date)
PIVOT(SUM(Working_Hours)
   FOR createddate IN({where}))";
                DataTable Attendance = DB.GetDataTable(sql);

                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Attendance", Attendance); 
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



        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Recieve_Training_Emp(object OBJ)
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
                string date = DateTime.Now.ToString("yyyy/MM/dd");
                string stime = "08:30:00";
                string etime = "17:15:00";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken); 
                DataTable dt = (DataTable)Newtonsoft.Json.JsonConvert.DeserializeObject(dt1_json, typeof(DataTable));
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataTable dt1 = new DataTable(); 
                        dt1 = DB.GetDataTable($@"select * from t_tsm_emp_attendance_m a where a.emp_no='{dt.Rows[i]["emp_no"]}' and a.createddate ='{date}'");
                        if(dt1.Rows.Count==0)
                        {
                            string sql = $@"insert into t_tsm_emp_attendance_m(emp_no,emp_name,emp_dept,start_time,end_time,online_time,status,createdby,createddate,createdtime)
                                values('{dt.Rows[i]["emp_no"]}','{dt.Rows[i]["emp_name"]}','{dt.Rows[i]["DEPARTMENT"]}','{date} {stime}','{date} {etime}','8','0',
                                       '{user}','{date}','{stime}')";
                            DB.ExecuteNonQuery(sql);
                        }
                        
                    }
                }
                
                ret.IsSuccess = true;
                ret.ErrMsg = "Added Successfully";
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
    }
}
