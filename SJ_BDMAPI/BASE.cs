using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BASE
    {
        /// <summary>
        /// 通过类型
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetInspectionDataTable(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string type,string inspection_code)
        {
            Dictionary<string,object> dic = new Dictionary<string, object>();
            string tableName = DB.GetString($@"select enum_value2 from sys001m where enum_type='enum_inspection_type' and enum_code='{type}'");
            if (!string.IsNullOrEmpty(tableName))
            {
                string sql = $@"select * from {tableName} where inspection_code='{inspection_code}'  and ROWNUM=1 ";
                dic = DB.GetDictionary(sql);
            }
            return dic;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataTable(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";

                var dt = DB.GetDataTable(sql);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

    }
}
