using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    /// <summary>
    /// 化学品信息
    /// </summary>
    public class Bdm_chemical_infomaintenanceBase
    {
        /// <summary>
        /// 化学品信息搜索
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_chemical_infomaintenanceSelect(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string chemical_name = jarr.ContainsKey("chemical_name") ? jarr["chemical_name"].ToString() : "";//化学品名称
                string medicament_name = jarr.ContainsKey("medicament_name") ? jarr["medicament_name"].ToString() : "";//化学品名称
                string wheres = string.Empty;
                if (!string.IsNullOrWhiteSpace(chemical_name))
                {
                    wheres += $"and chemical_name like '%{chemical_name}%'";
                }
                if (!string.IsNullOrWhiteSpace(medicament_name))
                {
                    wheres += $"and medicament_name like '%{medicament_name}%'";
                }
                DataTable dt = DB.GetDataTable($@"select chemical_no,chemical_name,medicament_name,reagent_proportion,corresponding_humidity,effective_time,remarks from  bdm_chemical_infomaintenance_m where  1=1 {wheres} order by id desc");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData1 = dic;
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;

        }
        /// <summary>
        /// 化学品信息新增，修改
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_chemical_infomaintenanceAddUP(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.BeginTransaction();
                DB.Open();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string chemical_category = jarr.ContainsKey("chemical_category") ? jarr["chemical_category"].ToString() : "";//化学品类别
                string chemical_no = jarr.ContainsKey("chemical_no") ? jarr["chemical_no"].ToString() : "";//化学品代号
                string chemical_name = jarr.ContainsKey("chemical_name") ? jarr["chemical_name"].ToString() : "";//化学品名称
                string medicament_name = jarr.ContainsKey("medicament_name") ? jarr["medicament_name"].ToString() : "";//药剂名称
                string reagent_proportion = jarr.ContainsKey("reagent_proportion") ? jarr["reagent_proportion"].ToString() : "";//药剂比例
                string corresponding_humidity = jarr.ContainsKey("corresponding_humidity") ? jarr["corresponding_humidity"].ToString() : "";//对应湿度
                string effective_time = jarr.ContainsKey("effective_time") ? jarr["effective_time"].ToString() : "";//有效时间(H)
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//备注
                string datas = DateTime.Now.ToString("yyyy-MM-dd");
                string times = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                sql = $"select chemical_no from  bdm_chemical_infomaintenance_m where chemical_no='{chemical_no}'";
                string chemical_nos = DB.GetString(sql);
                if (!string.IsNullOrWhiteSpace(chemical_nos))
                {
                    sql = $@"insert into bdm_chemical_infomaintenance_m(chemical_category,chemical_no,chemical_name,medicament_name,reagent_proportion,corresponding_humidity,effective_time,remarks,createby,createdate,createtime) values('{chemical_category}','{chemical_no}','{chemical_name}','{medicament_name}','{reagent_proportion}','{corresponding_humidity}','{effective_time}','{remarks}','{Usercode}','{datas}','{times}')";
                    DB.ExecuteNonQuery(sql);
                    ret.ErrMsg = "Add chemical information successfully";
                }
                else
                {
                    sql = $@"update bdm_chemical_infomaintenance_m set chemical_category='{chemical_category},chemical_name='{chemical_name}',medicament_name='{medicament_name}',reagent_proportion='{reagent_proportion}',corresponding_humidity='{corresponding_humidity}',effective_time='{effective_time}',remarks='{remarks}',modifyby='{Usercode}',modifydate='{datas}',modifytime='{times}' where chemical_no='{chemical_no}'";
                    ret.ErrMsg = "Modify chemical information successfully";
                }
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "The operation to edit chemical information failed" + ex.Message;
            }
            return ret;

        }
        /// <summary>
        /// 化学品信息删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_chemical_infomaintenanceDelete(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                DB.BeginTransaction();
                DB.Open();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string chemical_no = jarr.ContainsKey("chemical_no") ? jarr["chemical_no"].ToString() : "";//化学品代号
                string sql = string.Empty;

                sql = $@"delete from bdm_chemical_infomaintenance_m where  chemical_no='{chemical_no}'";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Delete chemical information successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Failed to delete chemical information" + ex.Message;
            }
            return ret;

        }


        public class Bdm_chemical_infomaintenanceList
        {
            public List<Bdm_chemical_infomaintenanceDto> project_datas { get; set; }
        }
        public class Bdm_chemical_infomaintenanceDto
        {
            public int id { get; set; }
            public string chemical_category { get; set;  }
            public string chemical_no { get; set; }
            public string chemical_name { get; set; }
            public string medicament_name { get; set; }
            public int reagent_proportion { get; set; }
            public string corresponding_humidity { get; set; }
            public int effective_time { get; set; }
            public string remarks{ get; set; }

        }
    }
}
