using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class Bdm_phase_creation_Base
    {
        /// <summary>
        /// 阶段创建搜索
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_phase_creationSelect(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string phase_creation_no = jarr.ContainsKey("phase_creation_no") ? jarr["phase_creation_no"].ToString() : "";//编号
                string phase_creation_name = jarr.ContainsKey("phase_creation_name") ? jarr["phase_creation_name"].ToString() : "";//名称
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//备注
                string wheres = string.Empty;
                if (!string.IsNullOrWhiteSpace(phase_creation_no))
                {
                    wheres += $"and phase_creation_no like '%{phase_creation_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(phase_creation_name))
                {
                    wheres += $"and phase_creation_name like '%{phase_creation_name}%'";
                }
                DataTable dt = DB.GetDataTable($@"select phase_creation_no,phase_creation_name,remarks from  bdm_phase_creation_m where  1=1 {wheres} order by id desc");
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
        /// 阶段创建新增，修改
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_phase_creationAddUP(object OBJ)
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
                string phase_creation_no = jarr.ContainsKey("phase_creation_no") ? jarr["phase_creation_no"].ToString() : "";//编号
                string phase_creation_name = jarr.ContainsKey("phase_creation_name") ? jarr["phase_creation_name"].ToString() : "";//名称
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";//备注
                string datas = DateTime.Now.ToString("yyyy-MM-dd");
                string times = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                sql = $"select phase_creation_no from  bdm_phase_creation_m where phase_creation_no='{phase_creation_no}'";
                string phase_creation_nos = DB.GetString(sql);
                if (!string.IsNullOrWhiteSpace(phase_creation_nos))
                {
                    sql = $@"insert into bdm_phase_creation_m(phase_creation_no,phase_creation_name,remarks,createby,createdate,createtime) values('{phase_creation_no}','{phase_creation_name}','{remarks}','{Usercode}','{datas}','{times}')";
                    DB.ExecuteNonQuery(sql);
                    ret.ErrMsg = "New stage succeeded";
                }
                else
                {
                    sql = $@"update bdm_phase_creation_m set phase_creation_no='{phase_creation_no}',phase_creation_name='{phase_creation_name}',remarks='{remarks}',modifyby='{Usercode}',modifydate='{datas}',modifytime='{times}' where phase_creation_no='{phase_creation_no}'";
                    ret.ErrMsg = "Modify phase succeeded";
                }
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Operation edit stage failed" + ex.Message;
            }
            return ret;

        }
        /// <summary>
        /// 阶段创建删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Bdm_phase_creationDelete(object OBJ)
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
                string phase_creation_no = jarr.ContainsKey("phase_creation_no") ? jarr["phase_creation_no"].ToString() : "";//编号
               
               
                string sql = string.Empty;

                sql = $@"delete from bdm_phase_creation_m where  phase_creation_no='{phase_creation_no}'";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Delete phase succeeded";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Delete phase failed" + ex.Message;
            }
            return ret;

        }

        public class Bdm_phase_creation_BaseList
        {
            public List<Bdm_phase_creation_BaseDto> project_datas { get; set; }
        }
       public class Bdm_phase_creation_BaseDto
        {
            public int id { get; set; }
            public string phase_creation_no { get; set; }
            public string phase_creation_name { get; set; }
            public string remarks { get; set; }
        }
    }
}
