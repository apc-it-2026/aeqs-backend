using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace SJ_SYSAPI
{
    public class DataBase
    {

        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDT(object OBJ)
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





                string sql = jarr["sql"].ToString();
                
               

                System.Data.DataTable dt = DB.GetDataTable(sql);

            

           
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataTable(object OBJ)
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

              


               
                string[] s = new string[1];
                s[0] = ",";
             
                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string where = jarr["where"].ToString();
                string orderby = jarr["orderby"].ToString();
               
                string spname = jarr["pname"].ToString();
                Dictionary<string, object> p = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(spname) && !string.IsNullOrEmpty(sqlp.Replace("{", "").Replace("}", "")))
                {

                    var jarrr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(sqlp);

                    string[] pname = spname.Split(s, StringSplitOptions.RemoveEmptyEntries);


                   
                    foreach (string pn in pname)
                    {
                        p.Add(pn, jarrr[pn].ToString());
                    }
                }

                string pj = string.Empty;

                if (where.Contains("@ALL"))
                {
                    var index = where.LastIndexOf('@');
                    where = where.Substring(index + 1, where.Length - index - 1);
                    
                    DataTable dt2 = DB.GetDataTable(@"
SELECT
*
FROM ("+sql+@"
) tmp
");

                    foreach (DataColumn item in dt2.Columns)
                    {
                        if (string.IsNullOrEmpty(pj))
                        {
                            pj = " and (" + item.ColumnName + " like " + "'%" + where + "%' ";
                        }
                        else
                        {
                            pj += "or " + item.ColumnName + " like " + "'%" + where + "%' ";
                        }

                    }
                    pj += ") ";
                    where = pj;
                }

                
                int total = DB.GetInt32(@"
SELECT
COUNT(1)
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
", p);
                
                string sqldata = string.Empty;

                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sqldata = @"
SELECT
* FROM
(
SELECT
tmp.*
FROM
(" + sql + @") tmp
" + orderby + @") T WHERE " + where + @"  
";
                }
                else if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                   
                        sqldata = @"

SELECT
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @" "+orderby+@"
";

                   
                }
                

                    System.Data.DataTable dt = DB.GetDataTable(sqldata, p);

                string headdata = string.Empty;

                foreach(System.Data.DataColumn dc in dt.Columns)
                {
                    headdata += dc.ColumnName + ",";
                }

                headdata = headdata.Remove(headdata.Length - 1);

                string dtJosn = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);


                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("headdata", headdata);
                d.Add("data", dtJosn);
                d.Add("total", total);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                ret.IsSuccess = true;

               
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }


        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataTableCupyPage(object OBJ)
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





                string[] s = new string[1];
                s[0] = ",";

                string sql = jarr["sql"].ToString();
                if (sql.Contains("(NOLOCK)"))
                {
                    sql = sql.Replace("(NOLOCK)", string.Empty);
                }
                if (sql.Contains("(nolock)"))
                {
                    sql = sql.Replace("(nolock)", string.Empty);
                }
                //if (sql.Contains("'"))
                //{
                //    sql = sql.Replace("'", string.Empty);
                //}
                string sqlp = jarr["sqlp"].ToString();
                string where = jarr["where"].ToString();
                string orderby = jarr["orderby"].ToString();
                string pagerow = jarr["pagerow"].ToString();
                string page = jarr["page"].ToString();
                string spname = jarr["pname"].ToString();
                Dictionary<string, object> p = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(spname) && !string.IsNullOrEmpty(sqlp.Replace("{", "").Replace("}", "")))
                {

                    var jarrr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(sqlp);

                    string[] pname = spname.Split(s, StringSplitOptions.RemoveEmptyEntries);



                    foreach (string pn in pname)
                    {
                        p.Add(pn, jarrr[pn].ToString());
                    }
                }

                string pj = string.Empty;

                if (where.Contains("@ALL"))
                {
                    var index = where.LastIndexOf('@');
                    where = where.Substring(index + 1, where.Length - index - 1);

                    DataTable dt2 = DB.GetDataTable(@"
SELECT
*
FROM (" + sql + @"
) tmp
");

                    foreach (DataColumn item in dt2.Columns)
                    {
                        if (string.IsNullOrEmpty(pj))
                        {
                            pj = " and (" + item.ColumnName + " like " + "'%" + where + "%' ";
                        }
                        else
                        {
                            pj += "or " + item.ColumnName + " like " + "'%" + where + "%' ";
                        }

                    }
                    pj += ") ";
                    where = pj;
                }


                int total = DB.GetInt32(@"
SELECT
COUNT(1)
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
", p);
                if (total < (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow))
                {
                    page = "1";
                }
                string sqldata = string.Empty;

                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sqldata = @"
SELECT
* FROM
(
SELECT
@n:=@n+1 as '行号',
tmp.*
FROM
(" + sql + @") tmp,
(select @n:=0) R
" + orderby + @") T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @" " + where + @"  limit " + pagerow + @" 
";
                }
                else if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sqldata = @"
SELECT
top(" + pagerow + @") * FROM
(
SELECT
row_number() over (" + orderby + @") as '行号',
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @"   
";
                    }
                    else
                    {
                        sqldata = @"
SELECT
top(" + pagerow + @") * FROM
(
SELECT
row_number()  as '行号',
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @"   
";
                    }
                }
                else if (DB.DataBaseType.ToLower() == "oracle")
                {
                    if (sql.Contains("(NOLOCK)"))
                    {
                        sql = sql.Replace("(NOLOCK)", string.Empty);
                    }
                    if (sql.Contains("(nolock)"))
                    {
                        sql = sql.Replace("(nolock)", string.Empty);
                    }
                    //if (sql.Contains("'"))
                    //{
                    //    sql = sql.Replace("'", string.Empty);
                    //}
                    sqldata = @"
SELECT
 * FROM
(
SELECT
rownum r,
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.r>=" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @" and rownum<=" + Convert.ToInt32(pagerow) + " " + orderby + @"   
";
                }


                System.Data.DataTable dt = DB.GetDataTable(sqldata, p);

                string headdata = string.Empty;

                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    headdata += dc.ColumnName + ",";
                }

                headdata = headdata.Remove(headdata.Length - 1);

                string dtJosn = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);


                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("headdata", headdata);
                d.Add("data", dtJosn);
                d.Add("total", total);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SYSGetDT(object OBJ)
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
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);





                string sql = jarr["sql"].ToString();



                System.Data.DataTable dt = DB.GetDataTable(sql);




                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SYSGetDataTable(object OBJ)
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
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);





                string[] s = new string[1];
                s[0] = ",";

                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string where = jarr["where"].ToString();
                string orderby = jarr["orderby"].ToString();

                string spname = jarr["pname"].ToString();
                Dictionary<string, object> p = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(spname) && !string.IsNullOrEmpty(sqlp.Replace("{", "").Replace("}", "")))
                {

                    var jarrr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(sqlp);

                    string[] pname = spname.Split(s, StringSplitOptions.RemoveEmptyEntries);



                    foreach (string pn in pname)
                    {
                        p.Add(pn, jarrr[pn].ToString());
                    }
                }

                string pj = string.Empty;
                
                if (where.Contains("@ALL"))
                {
                    var index = where.LastIndexOf('@');
                    where = where.Substring(index + 1, where.Length - index - 1);

                    DataTable dt2 = DB.GetDataTable(@"
SELECT
*
FROM (" + sql + @"
) tmp
");

                    foreach (DataColumn item in dt2.Columns)
                    {
                        if (string.IsNullOrEmpty(pj))
                        {
                            pj = " and (" + item.ColumnName + " like " + "'%" + where + "%' ";
                        }
                        else
                        {
                            pj += "or " + item.ColumnName + " like " + "'%" + where + "%' ";
                        }

                    }
                    pj += ") ";
                    where = pj;
                }


                int total = DB.GetInt32(@"
SELECT
COUNT(1)
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
", p);
                string sqldata = string.Empty;

                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sqldata = @"
SELECT
* FROM
(
SELECT
tmp.*
FROM
(" + sql + @") tmp
" + orderby + @") T WHERE " + where + @" 
";
                }
                else if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                   
                        sqldata = @"
SELECT
*
FROM
(
SELECT
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @" "+orderby+@"
) T 
";
                    }
                    
                    
                

                System.Data.DataTable dt = DB.GetDataTable(sqldata, p);

                string headdata = string.Empty;

                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    headdata += dc.ColumnName + ",";
                }

                headdata = headdata.Remove(headdata.Length - 1);

                string dtJosn = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);


                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("headdata", headdata);
                d.Add("data", dtJosn);
                d.Add("data_dt", dt);
                d.Add("total", total);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                ret.RetData1 = d;
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetDataTable(获取DataTable)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SYSGetDataTableCupyPage(object OBJ)
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
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);





                string[] s = new string[1];
                s[0] = ",";

                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string where = jarr["where"].ToString();
                string orderby = jarr["orderby"].ToString();
                string pagerow = jarr["pagerow"].ToString();
                string page = jarr["page"].ToString();
                string spname = jarr["pname"].ToString();
                Dictionary<string, object> p = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(spname) && !string.IsNullOrEmpty(sqlp.Replace("{", "").Replace("}", "")))
                {

                    var jarrr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(sqlp);

                    string[] pname = spname.Split(s, StringSplitOptions.RemoveEmptyEntries);



                    foreach (string pn in pname)
                    {
                        p.Add(pn, jarrr[pn].ToString());
                    }
                }

                string pj = string.Empty;

                if (where.Contains("@ALL"))
                {
                    var index = where.LastIndexOf('@');
                    where = where.Substring(index + 1, where.Length - index - 1);

                    DataTable dt2 = DB.GetDataTable(@"
SELECT
*
FROM (" + sql + @"
) tmp
");

                    foreach (DataColumn item in dt2.Columns)
                    {
                        if (string.IsNullOrEmpty(pj))
                        {
                            pj = " and (" + item.ColumnName + " like " + "'%" + where + "%' ";
                        }
                        else
                        {
                            pj += "or " + item.ColumnName + " like " + "'%" + where + "%' ";
                        }

                    }
                    pj += ") ";
                    where = pj;
                }


                int total = DB.GetInt32(@"
SELECT
COUNT(1)
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
", p);
                string sqldata = string.Empty;

                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sqldata = @"
SELECT
* FROM
(
SELECT
@n:=@n+1 as '行号',
tmp.*
FROM
(" + sql + @") tmp,
(select @n:=0) R
" + orderby + @") T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @" " + where + @"  limit " + pagerow + @" 
";
                }
                else if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                    if (!string.IsNullOrEmpty(orderby))
                    {
                        sqldata = @"
SELECT
top(" + pagerow + @") * FROM
(
SELECT
row_number() over (" + orderby + @") as '行号',
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @"   
";
                    }
                    else
                    {
                        sqldata = @"
SELECT
top(" + pagerow + @") * FROM
(
SELECT
row_number() as '行号',
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.行号>" + (Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + @"   
";
                    }
                }
                else if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sqldata = @"
SELECT
* FROM
(
SELECT
row_number() over (" + orderby + @") as '行号',
tmp.*
FROM
(" + sql + @") tmp
where 1=1 " + where + @"
) T WHERE T.行号 between " + ((Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + 1) + " and " + ((Convert.ToInt32(page) - 1) * Convert.ToInt32(pagerow) + Convert.ToInt32(pagerow)).ToString();

                }

                System.Data.DataTable dt = DB.GetDataTable(sqldata, p);

                string headdata = string.Empty;

                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    headdata += dc.ColumnName + ",";
                }

                headdata = headdata.Remove(headdata.Length - 1);

                string dtJosn = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);


                Dictionary<string, object> d = new Dictionary<string, object>();
                d.Add("headdata", headdata);
                d.Add("data", dtJosn);
                d.Add("total", total);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }


        /// <summary>
        /// GetString(获取String)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SYSGetString(object OBJ)
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
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);


                //string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<sql>", "</sql>");
                //string sqlp = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<sqlp>", "</sqlp>");
                string[] s = new string[1];
                s[0] = ",";
                //string[] pname = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<pname>", "</pname>").Split(s, StringSplitOptions.RemoveEmptyEntries);
                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string[] pname = jarr["pname"].ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);

                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(XML);

                //guid = SJeMES_Framework_NETCore.Common.WebServiceHelper.SavePostLog(DB, IP4, MAC, DllName, ClassName, Method, Data);

                Dictionary<string, object> p = new Dictionary<string, object>();
                foreach (string pn in pname)
                {
                    p.Add(pn, SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(sqlp, "<" + pn + ">", "</" + pn + ">"));
                }

                string r = DB.GetString(sql, p);



 
                ret.RetData = r;
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetString(获取String)
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetString(object OBJ)
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

        
                //string sql = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<sql>", "</sql>");
                //string sqlp = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<sqlp>", "</sqlp>");
                string[] s = new string[1];
                s[0] = ",";
                //string[] pname = SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(Data, "<pname>", "</pname>").Split(s, StringSplitOptions.RemoveEmptyEntries);
                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string[] pname = jarr["pname"].ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);

                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(XML);

                //guid = SJeMES_Framework_NETCore.Common.WebServiceHelper.SavePostLog(DB, IP4, MAC, DllName, ClassName, Method, Data);

                Dictionary<string, object> p = new Dictionary<string, object>();
                foreach (string pn in pname)
                {
                    p.Add(pn, SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(sqlp, "<" + pn + ">", "</" + pn + ">"));
                }

                string r = DB.GetString(sql, p);




                ret.RetData = r;
                ret.IsSuccess = true;

                
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// GetDataColumn
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataColumn(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;


            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                
               
             
                string sql = jarr["sql"].ToString();

                if (sql.Contains("(NOLOCK)"))
                {
                    sql = sql.Replace("(NOLOCK)",string.Empty);
                }
                if (sql.Contains("(nolock)"))
                {
                    sql = sql.Replace("(nolock)", string.Empty);
                }
                //if (sql.Contains("'"))
                //{
                //    sql = sql.Replace("'", string.Empty);
                //}
                //if (sql.Contains("'"))
                //{
                //    sql = sql.Replace("'", string.Empty);
                //}

                DataTable dt = DB.GetDataTable(@"select * from (" + sql + ") tmp where 1=1");

                List<string> d = new List<string>();
                foreach(DataColumn dc in dt.Columns)
                {
                    d.Add(dc.ColumnName);
                }

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(d);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }


        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ExecuteNonQuery(object OBJ)
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

           
               
                string[] s = new string[1];
                s[0] = ",";
              
                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string[] pname = jarr["pname"].ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);




                Dictionary<string, object> p = new Dictionary<string, object>();
                foreach (string pn in pname)
                {
                    p.Add(pn, SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(sqlp, "<" + pn + ">", "</" + pn + ">"));
                }

                string r = "";
                if (p != null && p.Count > 0)
                    r = DB.ExecuteNonQueryOffline(sql, p).ToString();
                else
                    r = DB.ExecuteNonQueryOffline(sql).ToString();





                ret.RetData = r;
                ret.IsSuccess = true;

 
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SYSExecuteNonQuery(object OBJ)
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
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);



                string[] s = new string[1];
                s[0] = ",";

                string sql = jarr["sql"].ToString();
                string sqlp = jarr["sqlp"].ToString();
                string[] pname = jarr["pname"].ToString().Split(s, StringSplitOptions.RemoveEmptyEntries);




                Dictionary<string, object> p = new Dictionary<string, object>();
                foreach (string pn in pname)
                {
                    p.Add(pn, SJeMES_Framework_NETCore.Common.StringHelper.GetDataFromFirstTag(sqlp, "<" + pn + ">", "</" + pn + ">"));
                }

                string r = DB.ExecuteNonQueryOffline(sql, p).ToString();





                ret.RetData = r;
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }


        /// <summary>
        /// 根据连接获取数据表
        /// </summary>
        /// <param name="OBJ">XML数据</param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTable(object OBJ)
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

                #region 
                DataTable dt = new DataTable();
                string sql = string.Empty;
                if (DB.DataBaseType.ToLower() == "sqlserver")
                {
                    sql = "select name from sys.tables ";
                    dt = DB.GetDataTable(sql);
                }
                if (DB.DataBaseType.ToLower() == "oracle")
                {
                    sql = "select table_name as name from user_tables";
                    dt = DB.GetDataTable(sql);
                }
                if (DB.DataBaseType.ToLower() == "mysql")
                {
                    sql = "select table_name as name from information_schema.tables where TABLE_SCHEMA = '" + DB.DataBaseName + "';";
                    dt = DB.GetDataTable(sql);
                }
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;


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
