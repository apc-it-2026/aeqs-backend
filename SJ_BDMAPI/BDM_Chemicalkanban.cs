using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SJ_BDMAPI
{
    public class BDM_Chemicalkanban
    {
        //-----------------------化学品容器管理----------------------------------//
        /// <summary>
        /// 结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Settlealawsuit_jz(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string sql = string.Empty;
                if (DB.GetInt32($@"select count(1) from qcm_abnormal_r_m where task_no='{task_no}'") > 0)
                {
                    sql = $@"update qcm_abnormal_r_m set closing_status='1' where task_no='{task_no}'";
                    DB.ExecuteNonQuery(sql);
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "Successfully closed the case";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Case closure failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Chemicalkanban_Main_getList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string container_no = jarr.ContainsKey("container_no") ? jarr["container_no"].ToString() : "";//容器代号
                string chemical_name = jarr.ContainsKey("chemical_name") ? jarr["chemical_name"].ToString() : "";//化学品名称

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(container_no))
                {
                    strwhere += $" and a.container_no like '%{container_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(chemical_name))
                {
                    strwhere += $" and c.chemical_name like '%{chemical_name}%'";
                }
                string sql = $@"SELECT
	a.container_no,--容器编号
	b.department_code,--产线代号
	b.department_name,--产线名称
	c.chemical_no,--化学品代号
	c.chemical_name,--化学品名称
	c.medicament_name,--药剂名称
	c.reagent_proportion,--药剂比例
	c.corresponding_humidity,--对应湿度
   case
        when a.distribution_date is null then d.g_mixing_time
        when a.distribution_date is not  null then a.distribution_date
    end as g_mixing_time,--调胶时间
	a.autime,--加胶时间(配送时间)
	c.effective_time,--有效时间
	'' effective_time2 --到期时间
FROM
	bdm_container_info_m A
LEFT JOIN base005m b on a.department_code=b.department_code
inner join bdm_chemical_info_print_m d on a.distribution_id=d.id
inner join Bdm_chemical_infomaintenance_m c on d.chemical_no=c.chemical_no
where a.IS_USE='1' {strwhere} order by a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int total = CommonBASE.GetPageDataTableCount(DB, sql);
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic, Formatting.Indented, timeFormat);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        //-------------------pda---------------------//
        /// <summary>
        /// 化学品条码打印首页数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrMain_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string chemical_category = jarr.ContainsKey("chemical_category") ? jarr["chemical_category"].ToString() : "";//类别代号
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//化学品名称查询
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string strwhere = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string sql = string.Empty;
                if (!string.IsNullOrWhiteSpace(chemical_category))
                {
                    strwhere += $@" and chemical_category='{chemical_category}'";
                   
                }
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    int ids = 0;
                    int.TryParse(keyword, out ids);
                    string chemical_no = string.Empty;
                    if (ids>0)
                    {
                        sql = $@"select chemical_no from bdm_chemical_info_print_m where id='{keyword}'";
                        chemical_no = DB.GetString(sql);
                    }
                    if (!string.IsNullOrWhiteSpace(chemical_no))
                    {
                        strwhere += $@" and (chemical_name like '%{keyword}%' or chemical_no like '%{chemical_no}%')";
                    }
                    else
                    {
                        strwhere += $@" and (chemical_name like '%{keyword}%')";
                    }
                   
                }
                 sql = $@"SELECT
	chemical_category,
    chemical_no,
	chemical_name,
	medicament_name,
	corresponding_humidity,
	reagent_proportion,
	effective_time
FROM
	Bdm_chemical_infomaintenance_m    where 1=1  {strwhere} order by id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 化学品条码打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Printdata(object OBJ)
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
                string chemical_no = jarr.ContainsKey("chemical_no") ? jarr["chemical_no"].ToString() : "";//化学品代号
                string effective_time = jarr.ContainsKey("effective_time") ? jarr["effective_time"].ToString() : "";//有效时间

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = string.Empty;
                string datetime = "(select sysdate from dual)";
                string g_mixing_time = DB.GetString(datetime);//调胶时间
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string datetime2 = $@"(select sysdate+{effective_time}/24 from dual)";
                dic.Add("effective_time", effective_time);
                effective_time = DB.GetString(datetime2,dic);
                dic = new Dictionary<string, object>();
                sql = $@"insert into bdm_chemical_info_print_m(chemical_no,g_mixing_time,effective_time,createby,createdate,createtime) values(@chemical_no,{datetime},{datetime2},@createby,@createdate,@createtime)";
                dic.Add("chemical_no", chemical_no);
                dic.Add("createby", user);
                dic.Add("createdate", date);
                dic.Add("createtime", time);
                DB.ExecuteNonQuery(sql, dic);
                string id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "bdm_chemical_info_print_m");
                dic = new Dictionary<string, object>();
                //id = SJeMES_Framework_NETCore.Common.Security.AESEncrypt(id, "978/*/o65asdddd7d8d4d5d6d128****");
                dic.Add("id", id);
                dic.Add("g_mixing_time", g_mixing_time);
                dic.Add("effective_time", effective_time);
                ret.RetData1 = dic;
                ret.IsSuccess = true;
                ret.ErrMsg = "Printed successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Printing failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 扫描容器二维码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetContainer_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string container_no = jarr.ContainsKey("container_no") ? jarr["container_no"].ToString() : "";//容器代号
                string sql = $@"SELECT
	a.container_no,
	b.department_code,--产线代号
	b.department_name--产线名称
FROM
	bdm_container_info_m A
LEFT JOIN base005m b on a.department_code=b.department_code 
where a.container_no=@container_no";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("container_no", container_no);
                DataTable dt = DB.GetDataTable(sql,dic);
                if (dt.Rows.Count < 1)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $@"The container number {container_no} does not exist, please check whether the basic information of the container is maintained";
                    return ret;
                }
                ret.IsSuccess = true;
                ret.RetData1 = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 扫描化学品信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetChemical_List(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//化学品id
                #region 解密
                /* bool flagse64 = (id.Length % 4 == 0) && Regex.IsMatch(id, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
                        if (!flagse64)
                        {
                            int ids = 0;
                            int.TryParse(id, out ids);
                            if (ids < 1)
                            {
                                ret.ErrMsg = "该二维码为无效二维码";
                                ret.IsSuccess = false;
                                return ret;
                            }
                        }*/
                //id = SJeMES_Framework_NETCore.Common.Security.AESDecrypt(id, "978/*/o65asdddd7d8d4d5d6d128****"); 
                #endregion
                int ids = 0;
                int.TryParse(id, out ids);
                if (ids < 1)
                {
                    ret.ErrMsg = "This QR code is an invalid QR code";
                    ret.IsSuccess = false;
                    return ret;
                }
                string sql = $@"SELECT
	a.chemical_no,--化学品代号
	a.chemical_name,--化学品名称
	a.corresponding_humidity,--对应温度
    to_char(b.g_mixing_time,'yyyy-mm-dd hh24:mi:ss') as g_mixing_time,--调胶时间
	a.medicament_name,--药剂名称
	a.reagent_proportion,--药剂比例
	a.effective_time --有效时间
FROM
	Bdm_chemical_infomaintenance_m A
LEFT JOIN bdm_chemical_info_print_m b on a.chemical_no=b.chemical_no where b.id=@id";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("id",id);
                DataTable dt = DB.GetDataTable(sql,dic);
                if (dt.Rows.Count < 1)
                {
                    ret.ErrMsg = "The QR code is invalid, please confirm whether it is a print QR code";
                    ret.IsSuccess = false;
                    return ret;
                }
                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 化学品配送信息提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_data(object OBJ)
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
                string container_no = jarr.ContainsKey("container_no") ? jarr["container_no"].ToString() : "";//容器编号
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//二维码id
                string g_mixing_time = jarr.ContainsKey("g_mixing_time") ? jarr["g_mixing_time"].ToString() : "";//调配时间
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                //id = SJeMES_Framework_NETCore.Common.Security.AESDecrypt(id, "978/*/o65asdddd7d8d4d5d6d128****");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = "select container_no from bdm_container_info_m where container_no=@container_no";
                dic.Add("container_no", container_no);
                DataTable dt = DB.GetDataTable(sql, dic);
                if (dt.Rows.Count < 1)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Please maintain the basic information container data before operating";
                    return ret;
                }
                else
                {

                    sql = $@"select chemical_no from bdm_chemical_info_print_m where id='{id}'";
                    string chemical_no = DB.GetString(sql);
                    if (string.IsNullOrWhiteSpace(chemical_no))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Lack of conditions, please check the chemical barcode printing function";
                        return ret;
                    }
                    else
                    {
                        sql = "select CHEMICAL_NO from bdm_container_info_c where container_no=@container_no";
                        dic = new Dictionary<string, object>();
                        dic.Add("container_no", container_no);

                        DataTable dt_c = DB.GetDataTable(sql, dic);
                        var chemicals = dt_c.Select($@"CHEMICAL_NO='{chemical_no}'");
                        if (chemicals.Count() <= 0)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = $@"The scanned chemical does not match the chemical under the basic information of the container, please scan again";
                            return ret;
                        }
                        dic = new Dictionary<string, object>();
                        sql = $@"update  bdm_container_info_m set distribution_id=@distribution_id,distribution_date=to_date('{g_mixing_time}','yyyy-mm-dd hh24:mi:ss'),autime=sysdate,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where container_no=@container_no";
                        dic.Add("distribution_id", id);
                        dic.Add("modifyby", user);
                        dic.Add("modifydate", date);
                        dic.Add("modifytime", time);
                        dic.Add("container_no", container_no);
                        DB.ExecuteNonQuery(sql, dic);
                    }
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submit failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /* public static bool IsBase64(string base64Str, out byte[] bytes)
         {

             bytes = null;
             if (string.IsNullOrEmpty(base64Str))
                 return false;
             else
             {
                 if (base64Str.Contains(","))
                     base64Str = base64Str.Split(',')[1];
                 if (base64Str.Length % 4 != 0)
                     return false;
                 if (base64Str.Any(c => !base64CodeArray.Contains(c)))
                     return false;
             }
             try
             {
                 bytes = Convert.FromBase64String(base64Str);
                 return true;
             }
             catch (FormatException)
             {
                 return false;
             }
         }*/

        /// <summary>
        /// 容器使用管控查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchContainerUse(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//模糊搜索

                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";

                string whereStr = string.Empty;
                Dictionary<string, object> whereParamDic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    whereStr += $" and (a.container_no like @container_no or b.department_name like @department_name)";
                    whereParamDic.Add("container_no", $@"%{keyword}%");
                    whereParamDic.Add("department_name", $@"%{keyword}%");
                }
                string sql = $@"
SELECT
	a.container_no,--容器编号
	b.department_name,--产线名称
	a.IS_USE
FROM
	bdm_container_info_m a
LEFT JOIN base005m b on a.department_code=b.department_code
where 1=1 {whereStr} 
order by a.id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", whereParamDic);
                dt.Columns.Add("IS_USE_BOOL",typeof(bool));
                foreach (DataRow item in dt.Rows)
                {
                    item["IS_USE_BOOL"] = item["IS_USE"].ToString() == "1" ? true : false;
                }

                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 更新使用状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateContainerUse(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string container_no = jarr.ContainsKey("container_no") ? jarr["container_no"].ToString() : "";//容器编号
                bool is_use_bool = jarr.ContainsKey("is_use_bool") ? Convert.ToBoolean(jarr["is_use_bool"].ToString()) : false;//是否使用

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                DateTime currDate = DateTime.Now;
                string date = currDate.ToString("yyyy-MM-dd");
                string time = currDate.ToString("HH:mm:ss");

                DB.Open();
                DB.BeginTransaction();
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = "select container_no from bdm_container_info_m where container_no=@container_no";
                dic.Add("container_no", container_no);
                DataTable dt = DB.GetDataTable(sql, dic);
                if (dt.Rows.Count < 1)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Please maintain the basic information container data before operating";
                    return ret;
                }
                else
                {
                    dic = new Dictionary<string, object>();
                    sql = $@"update bdm_container_info_m set IS_USE=@is_use,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where container_no=@container_no";

                    dic.Add("is_use", is_use_bool ? "1" : "0");
                    dic.Add("modifyby", user);
                    dic.Add("modifydate", date);
                    dic.Add("modifytime", time);
                    dic.Add("container_no", container_no);
                    DB.ExecuteNonQuery(sql, dic);
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
