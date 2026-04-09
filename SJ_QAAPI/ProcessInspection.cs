using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_QAAPI
{

    /// <summary>
    /// 过程检验
    /// </summary>
    public class ProcessInspection
    {
        /// <summary>
        /// 获取过程检验单列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestDocList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                
                #region 接口参数
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                #endregion
                #region 逻辑
                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@"
select id,quality_inspection,bill_from,bill_no,sorting,procedure_no,procedure_name,products_barcode,lot_barcode,packing_barcode,material_no,
material_name,material_specifications,qty,quality_type,qty_qa,qty_ok,qty_bad,qty_scrap,qty_back,quality_date,quality_saff,
inspection_results,results,notes from qa041m",
Where, ReqObj.UserToken);


                string count = DB.GetString("SELECT count(1) FROM qa041m WHERE 1=1 " + Where);
                string sql = @"select * from (
select id,quality_inspection,bill_from,bill_no,sorting,procedure_no,procedure_name,products_barcode,lot_barcode,packing_barcode,material_no,
material_name,material_specifications,qty,quality_type,qty_qa,qty_ok,qty_bad,qty_scrap,qty_back,quality_date,quality_saff,
inspection_results,results,notes,@n:= @n + 1 as RN from qa041m M,
(select @n:= 0) d) tab where  RN > " + total + " " + Where + " limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }

                    headdata.Remove(headdata.Length - 1);

                    string headkey = "id,检验单号,单据来源,来源单号,生产顺序,工序代号,工序名称,产品条码,批号,包装条码,品号,品名,规格,送检数量,检验类型,检验数量,验收数量,不良数量,报废数量,验退数量,检验日期,检验人员,检验结果,判定结果,备注";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", count);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 获取过程检验单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestDocInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                
                #region 接口参数
                string quality_inspection = jarr["quality_inspection"].ToString();
            
                #endregion
                #region 逻辑


                string sql = @"
SELECT
id,quality_inspection,bill_from,bill_no,sorting,procedure_no,procedure_name,products_barcode,lot_barcode,packing_barcode,material_no,
material_name,material_specifications,qty,quality_type,qty_qa,qty_ok,qty_bad,qty_scrap,qty_back,quality_date,quality_saff,
inspection_results,results,notes
FROM qa041m
where quality_inspection='" + quality_inspection + @"'
";
                DataTable dt = DB.GetDataTable(sql);

                sql = @"
SELECT
id,quality_inspection,sorting,item_type,itemno,itemname,description,qty,defect_level,AC,RE,qty_bad,results,check_item,BadNo,note
FROM qa041a1
where quality_inspection='" + quality_inspection + @"'
order by sorting asc
";

                DataTable dt1 = DB.GetDataTable(sql);

                sql = @"
SELECT
id,quality_inspection,sorting,item_type,itemno,itemname,description,qty,upper,lower,qty_bad,results,check_item,BadNo,note
FROM qa041a2
where quality_inspection='" + quality_inspection + @"'
order by sorting asc
";

                DataTable dt2 = DB.GetDataTable(sql);

                Dictionary<string, object> p = new Dictionary<string, object>();
                string result = string.Empty;
                string result1 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    string json1 = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt1);
                    string json2 = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt2);
                    Dictionary<string, object> a = new Dictionary<string, object>();
                    a.Add("QA041M", json);
                    a.Add("QA041A1", json1);
                    a.Add("QA041A2", json2);
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(a);

                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(a);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "没有找到对应的数据";
                }
                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 修改过程检验单信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveTestDocInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {

                Data = ReqObj.Data.ToString();
                
             
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                
                #region 接口参数

           

                DataTable dtdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(Data);

                #endregion

            

                string sql = string.Empty;

                #region 逻辑
                foreach (DataRow dr in dtdata.Rows)
                {
                    string tablename = dr["tablename"].ToString();
                    string id = dr["id"].ToString();
                    Dictionary<string,object> dtabledata =Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(dr["tabledata"].ToString());

                    sql += @"
update "+tablename+@" set 
";
                    foreach(string key in dtabledata.Keys)
                    {
                        sql += key + "='" + dtabledata[key].ToString() + @"',";
                    }
                    sql = sql.Remove(sql.Length - 1);

                    sql += " where id=" + id+";";
                }

                if(DB.ExecuteNonQueryOffline(sql) >0)
                {
                    ret.IsSuccess = true;
                    ret.ErrMsg = "修改成功";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "修改失败";
                }

                #endregion

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }
    }
}
