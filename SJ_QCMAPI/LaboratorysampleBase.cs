using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class LaboratorysampleBase
    {

        /// <summary>
        /// 新增实验室样品存放管理
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Addlaboratorysample(object OBJ)
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

                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string item_name = jarr.ContainsKey("item_name") ? jarr["item_name"].ToString() : "";
                string vend_name = jarr.ContainsKey("vend_name") ? jarr["vend_name"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string location_name = jarr.ContainsKey("location_name") ? jarr["location_name"].ToString() : "";
                string location_no = jarr.ContainsKey("location_no") ? jarr["location_no"].ToString() : "";
                string vend_no = jarr.ContainsKey("vend_no") ? jarr["vend_no"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                if (string.IsNullOrEmpty(prod_no.Trim()) ||
                   string.IsNullOrEmpty(item_no.Trim()) ||
                   string.IsNullOrEmpty(item_name.Trim()) ||
                   string.IsNullOrEmpty(vend_no.Trim()) ||
                   string.IsNullOrEmpty(vend_name.Trim()) ||
                   string.IsNullOrEmpty(location_name.Trim()) ||
                   string.IsNullOrEmpty(location_no.Trim())
                   )
                {
                    throw new Exception("输入数据不全!请检验");
                }

                string putin_date = DateTime.Now.ToString("yyyy-MM-dd");
                DateTime end_dates= DateTime.Now.AddDays(90);
                string end_date = end_dates.ToString("yyyy-MM-dd");
                string sql = $@"select*from QCM_LABORATORYSAMPLE_STORAGE_M where item_no='{item_no}'";
                DataTable dt = DB.GetDataTable(sql);
                string sql2 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    sql2 = $"update QCM_LABORATORYSAMPLE_STORAGE_M set location_no='{location_no}',location_name='{location_name}' where item_no='{item_no}'";
                  
                }
                else
                {
                    sql2 = $"insert into QCM_LABORATORYSAMPLE_STORAGE_M(location_no,location_name,item_no,item_name,vend_no,vend_name,prod_no,putin_date,end_date,createby,createdate,createtime) values('{location_no}','{location_name}','{item_no}','{item_name}','{vend_no}','{vend_name}','{prod_no}','{putin_date}','{end_date}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
                }
                DB.ExecuteNonQuery(sql2);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "入库成功";

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "入库失败，原因:"+ ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 实验室样品存放管理修改存放位置(包括删除)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Updatelaboratorysample(object OBJ)
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

                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string location_no = jarr.ContainsKey("location_no") ? jarr["location_no"].ToString() : "";
                string location_name = jarr.ContainsKey("location_name") ? jarr["location_name"].ToString() : "";
                string CAO = jarr.ContainsKey("CAO") ? jarr["CAO"].ToString() : "";
                string sql = string.Empty;
                if (CAO == "Delete")
                {
                    sql = $"delete QCM_LABORATORYSAMPLE_STORAGE_M  where item_no='{ITEM_NO}'";
                }
                if (CAO == "Update")
                {
                    sql = $"update QCM_LABORATORYSAMPLE_STORAGE_M set location_no='{location_no}',location_name='{location_name}' where item_no='{ITEM_NO}'";
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
      
    }
}
