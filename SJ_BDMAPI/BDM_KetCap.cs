using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_KetCap
    {
        /// <summary>
        /// 查询工段检测项目
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getenum_inspection_type(object OBJ)
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
                string workshop_section_no = jarr.ContainsKey("workshop_section_no") ? jarr["workshop_section_no"].ToString() : "";//工段编号
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string inspection_type = string.Empty;
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(workshop_section_no))
                {
                    string id = DB.GetString($@"select id from bdm_workshop_section_m where WORKSHOP_SECTION_NO='{workshop_section_no}'");
                    inspection_type = DB.GetString($@"select inspection_type from bdm_workshop_section_d where m_id='{id}' and ROWNUM=1 ORDER BY id asc");
                    where = $@" and enum_code='{inspection_type}'";
                }

                string sql = $@"select enum_value2,enum_value from sys001m where enum_type='enum_inspection_type' and enum_code in ('0','1','2','3','4','5','6','7') {where}";
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

        /// <summary>
        /// 根据检测项查询数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestItem(object OBJ)
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
                string tablename = jarr.ContainsKey("tablename") ? jarr["tablename"].ToString() : "";
                //string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                //string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                if (string.IsNullOrEmpty(tablename))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Please select a section!";
                    return ret;
                }
                string sql = string.Empty;
                sql = $@"select id,inspection_code,inspection_name,shortcut_key,CREATEDATE from {tablename} where ROWNUM<=20 and qc_type='1' order by id asc";
                DataTable dt = DB.GetDataTable(sql);

                string code = DB.GetString($@"select enum_code from sys001m where enum_type='enum_inspection_type' and enum_value2='{tablename}'");

                //查询有无数据无数据添加
                int count = DB.GetInt32($@"select count(1) from tqc_shortcut_key_m where inspection_type='{code}'");
                if (count<=0)
                {
                    string sqlkey = string.Empty;
                    DataTable dtkey = DB.GetDataTable($@"select enum_code from sys001m where enum_type='enum_tqc_key'");
                    foreach (DataRow item in dtkey.Rows)
                    {
                        sqlkey += $@"insert into tqc_shortcut_key_m (inspection_type,tqc_key,shortcut_key,createby,createdate,createtime) 
                                    values('{code}','{item["enum_code"]}','','{user}','{date}','{time}');";
                    }
                    if (!string.IsNullOrWhiteSpace(sqlkey))
                        DB.ExecuteNonQuery($@"BEGIN {sqlkey} END;");
                    DB.Commit();
                }

                sql = $@"SELECT
	                    m.inspection_type,
	                    m.tqc_key,
	                    s.enum_value as tqc_key_name,
	                    m.shortcut_key
                    FROM
	                    tqc_shortcut_key_m m
	                    LEFT JOIN sys001m s on m.tqc_key=s.enum_code and s.enum_type='enum_tqc_key'
                        where m.inspection_type='{code}'";
                DataTable dt2 = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);

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

        /// <summary>
        /// 检测项编辑
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditTestItem(object OBJ)
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
                DataTable TestItem = jarr.ContainsKey("TestItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TestItem"].ToString()) : null;//查询条件 检测项表数据
                string tablename = jarr.ContainsKey("tablename") ? jarr["tablename"].ToString() : "";//查询条件 表名
                DataTable TqcItem = jarr.ContainsKey("TqcItem") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TqcItem"].ToString()) : null;//查询条件 tqc功能快捷键表
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间

                string sql = string.Empty;
                if (!string.IsNullOrEmpty(tablename))
                {
                    foreach (DataRow item in TestItem.Rows)
                    {
                        sql = $@"update {tablename} set shortcut_key='{item["shortcut_key"]}',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where id='{item["id"]}'";
                        DB.ExecuteNonQuery(sql);
                    }
                    string code = DB.GetString($@"select enum_code from sys001m where enum_type='enum_inspection_type' and enum_value2='{tablename}'");
                    sql = "";
                    foreach (DataRow item in TqcItem.Rows)
                    {
                        sql += $@"update tqc_shortcut_key_m set shortcut_key='{item["对应按钮"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where inspection_type='{code}' and tqc_key='{item["操作代号"]}';";
                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                        DB.ExecuteNonQuery($@"BEGIN {sql} END;");
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No table name!!!";
                    return ret;
                }

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
