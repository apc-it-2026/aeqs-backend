using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class TQCBase
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArtImageUrl(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"SELECT
	IMG_URL,
	( SELECT mer_po FROM BDM_SE_ORDER_MASTER WHERE SE_ID IN ( SELECT SE_ID FROM BDM_SE_ORDER_ITEM WHERE PROD_NO LIKE '%{PROD_NO}%' ) ) AS MER_PO 
FROM
	bdm_prod_m 
WHERE
	PROD_NO LIKE '%{PROD_NO}%'";

                DataTable dt = DB.GetDataTable(sql);

                dic.Add("Data", dt);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
