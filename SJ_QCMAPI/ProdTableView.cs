using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class ProdTableView
    {
        /// <summary>
        /// ART定制检测标准列表页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_PROD_List(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string develop_season = jarr.ContainsKey("develop_season") ? jarr["develop_season"].ToString() : "";
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string PRODUCT_MONTH = jarr.ContainsKey("PRODUCT_MONTH") ? jarr["PRODUCT_MONTH"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string wheres = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(develop_season))
                    wheres += " and a.develop_season like '%" + develop_season + "%'";
                if (!string.IsNullOrEmpty(shoe_no))
                    wheres += "and a.shoe_no like '%" + shoe_no + "%'";
                if (!string.IsNullOrEmpty(prod_no))
                    wheres += "and a.prod_no like '%" + prod_no + "%'";
                if (!string.IsNullOrEmpty(PRODUCT_MONTH))
                    wheres += $"and a.PRODUCT_MONTH ='{PRODUCT_MONTH}'";

                //转译
                string sql = $@"select a.ORG_ID,a.develop_season,a.shoe_no,a.prod_no,a.PRODUCT_MONTH,b.img_url from bdm_rd_prod a
left join bdm_prod_m b on a.prod_no=b.prod_no where  1=1" + wheres+ "order by a.shoe_no ASC";
                string sql2 = "select DEPARTMENT_NO,DEPARTMENT_NAME,REMARKS from BDM_QUALITY_DEPARTMENT_M";
                DataTable dt1 = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("data1", dt1);
                DataTable dt2 = DB.GetDataTable(sql2);
                dic.Add("rowCount", rowCount);
                dic.Add("data2", dt2);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
    }
}
