using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
     public class QAShoeShapeBASE
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ARTDelete(object OBJ)
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

                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";

                string sql1 = $"select prod_no from BDM_PROD_CUSTOMQUALITY_ITEM where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}' and category_no='{category_no}' and testitem_code='{testitem_code}'";
                DataTable dt = DB.GetDataTable(sql1);
                string sql2 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    sql2 = $@"delete from BDM_PROD_CUSTOMQUALITY_ITEM where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}' and category_no='{category_no}' and testitem_code='{testitem_code}'";
                }
                DB.ExecuteNonQuery(sql2);
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
