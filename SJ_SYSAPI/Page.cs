using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_SYSAPI
{
   public class Page
    {
        //获取数据
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);//获取业务库
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);//获取系统库

                ret.IsSuccess = true;

                #region 接口参数
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                string sql = string.Empty;
                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sql = @"select * from (
select M.*,@n:= @n + 1 as RN from HR001M M,(select @n:= 0) d
where 1=1)
tab where  RN > " + total + "  limit " + PageRow + "";
                }
                else if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                    sql = @"
Select top(" + PageRow + @") *
from
(
select
row_number() 
over(order by id) as rownumber,*
 from HR001M
where 1=1 ) tmp
where rownumber> " + total + @"
";
                }
                else if (DB.DataBaseType.ToLower() == "oracle")
                {

                   sql = @"select * from (
select staff_no,staff_name,UDF01,UDF02,ROWNUM as RN from HR001M M
where 1=1) tab where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(PageRow)).ToString();
                }
               DataTable dt = DB.GetDataTable(sql);
                total = DB.GetInt32("select count(1) from HR001M where 1=1");



                Dictionary<string, object> p = new Dictionary<string, object>();
                string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                p.Add("HR001M", json);
                p.Add("Total", total);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }
    }
}
