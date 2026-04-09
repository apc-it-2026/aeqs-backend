using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_QAAPI
{
    public class BASE
    {
        /// <summary>
        /// 获取检验项目资料接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestItemInfo(object OBJ)
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
SELECT
itemno,itemname,description,check_item,qa_level,defect_level,item_type,upper,lower,FirstStandard,notes from qa003m ",
Where, ReqObj.UserToken);

                string sql = @"select * from (
select itemno,itemname,description,check_item,qa_level,defect_level,item_type,upper,lower,FirstStandard,notes,
@n:= @n + 1 as RN from QA003M M,(select @n:= 0) d
" + OrderBy + @") tab where  RN >" + total + " " + Where + " limit " + PageRow + "";
                    DataTable dt = DB.GetDataTable(sql);
                    string count = DB.GetString("select count(1) from QA003M where 1=1 "+Where+"");
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

                    string headkey = "检验项目代号,检验项目名称,检验项目描述,是否参与判定,检验等级,缺陷等级,检验类型,最高值,最低值,是否首检项,备注";
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
        /// 获取不良分类接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadType(object OBJ)
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
SELECT
badno,badname,description from qa004m ",
Where, ReqObj.UserToken);

                string count =DB.GetString("SELECT count(1) FROM qa004m where (badtype='' or badtype is null) "+Where);
                string sql = @"select * from (
select badno,badname,description,@n:= @n + 1 as RN from qa004m M,
(select @n:= 0) d
where badtype='' or badtype is null
" + OrderBy + @") tab where  RN > " + total + " "+Where+" limit " + PageRow + "";
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

                    string headkey = "不良分类代号,不良分类名称,不良分类描述";
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
        /// 获取不良信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadInfo(object OBJ)
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
SELECT
bad_no,bad_info,bad_description from qa010m ",
Where, ReqObj.UserToken);

                string count = DB.GetString("SELECT count(1) FROM qa010m where 1=1 " + Where);
                string sql = @"select * from (
select M.bad_no,M.bad_info,M.bad_description,@n:= @n + 1 as RN from qa010m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + " limit " + PageRow + "";
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

                    string headkey = "不良信息代号,不良信息名称,不良信息描述";
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
        /// 获取不良原因分类接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadReasonType(object OBJ)
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
SELECT
reasonno,reasonname,description from qa005m ",
Where, ReqObj.UserToken);

                string COUNT = DB.GetString("SELECT count(1) FROM qa005m where reasontype='' or reasontype is null "+Where);
                string sql = @"select * from (
select reasonno,reasonname,description,@n:= @n + 1 as RN from qa005m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total+" "+Where +" limit "+PageRow+"";
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
                    string headkey = "不良原因分类代号,不良原因分类名称,不良原因分类描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", COUNT);
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
        /// 获取不良原因接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBadReason(object OBJ)
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
SELECT
reasonno,reasonname,description from qa005m ",
Where, ReqObj.UserToken);

                string COUNT = DB.GetString("SELECT count(1) FROM qa005m where reasontype<>'' and reasontype is not null " + Where);
                string sql = @"select * from (
select M.reasonno,M.reasonname,M.description,M.reasontype,M1.reasonname as reasontypename,@n:= @n + 1 as RN from qa005m M
LEFT JOIN qa005m M1 ON M1.reasonno =M.reasontype
,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + " limit " + PageRow + "";
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
                    string headkey = "不良原因代号,不良原因名称,不良原因描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", COUNT);
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
        /// 获取检验类别接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TestType(object OBJ)
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
SELECT
group_type,group_name,group_desc,qa_mode,qa_type,qty,rate from qa007m ",
Where, ReqObj.UserToken);

                string count = DB.GetString("SELECT count(1) FROM qa007m WHERE 1=1 "+Where);
                string sql = @"select * from (
select group_type,group_name,group_desc,qa_mode,qa_type,qty,rate,@n:= @n + 1 as RN from qa007m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total+" "+Where+" limit "+PageRow+"";
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

                    string headkey = "检验类别代号,检验类别名称,检验类别描述,检验方式,检验数量,送检数量";
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
        /// 获取检验归类列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestItemGroupList(object OBJ)
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
SELECT
group_type,group_name,process_no,material_no,material_name,material_desc,unit,notes from qa008m ",
Where, ReqObj.UserToken);

                string count = DB.GetString("SELECT count(1) FROM qa008m WHERE 1=1 " + Where);
                string sql = @"select * from (
select group_type,group_name,process_no,material_no,material_name,material_desc,unit,notes,@n:= @n + 1 as RN from qa008m M,
(select @n:= 0) d
" + OrderBy + @") tab where  RN > " + total + " " + Where + " limit " + PageRow + "";
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

                    string headkey = "检验归类代号,检验归类名称,检验流程,品号,品名,规格,单位,备注";
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
        /// 获取检验归类信息接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestItemGroup(object OBJ)
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
                string material_no = jarr["material_no"].ToString();
                string process_no = jarr["process_no"].ToString();
                #endregion
                #region 逻辑


                string sql = @"
SELECT
group_type,group_name,process_no,material_no,material_name,material_desc,unit,notes
FROM qa008m
where material_no='"+material_no+@" and process_no='"+process_no+@"'
";
                DataTable dt = DB.GetDataTable(sql);

                sql = @"
SELECT
material_no,process_no,sorting,item_type,itemno,itemname,
description,check_item,qa_level,AC,RE,defect_level,upper,lower,qa_mode,qa_type,qty,rate,notes
FROM qa008a1
where material_no='" + material_no + @" and process_no='" + process_no + @"'
orderby sorting asc
";

                DataTable dt1 = DB.GetDataTable(sql);
                Dictionary<string, object> p = new Dictionary<string, object>();
                string result = string.Empty;
                string result1 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    string json1 = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt1);
                    Dictionary<string, object> a = new Dictionary<string, object>();
                    a.Add("QA008M", json);
                    a.Add("QA008A1", json1);
                    result = Newtonsoft.Json.JsonConvert.SerializeObject(a);

                    ret.IsSuccess = true;
                    ret.ErrMsg = Newtonsoft.Json.JsonConvert.SerializeObject(a);
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
    }
}
