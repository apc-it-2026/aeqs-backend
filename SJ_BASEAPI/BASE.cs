using Org.BouncyCastle.Ocsp;
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.DBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJ_BASEAPI
{
    public class BASE
    {
        /// <summary>
        /// 获取物料信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaterielInfo(object OBJ)
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

                

                string key = "material_no";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);

                if (!string.IsNullOrEmpty(ReqP["material_no"].ToString()))
                {
                    string selectkey = @"material_no,material_name,material_specifications,material_type,process_no";
                    Dictionary<string, object> selectp = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(selectkey);
                    DataTable BASE007M = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary("base007m",
                        selectp, " material_no=@material_no ",ReqObj.UserToken), ReqP);

                    if (BASE007M.Rows.Count > 0)
                    {
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(BASE007M);
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("BASE007M", json);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "The material information under this product number does not exist！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "parameter is empty！";
                }

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;

        }

        /// <summary>
        /// 获取物料信息列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaterielList(object OBJ)
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


                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);//访问企业库



                string key = "Where,OrderBy,Page,PageRow";

                Dictionary<string, object> ReqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(key, ReqObj);


                string selectkey = @"material_no,material_name,material_specifications,material_iunit";

                Dictionary<string, object> selectp = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(selectkey);

                ReqP["Where"] = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m",selectp,string.Empty).Replace("WHERE 1=1",""), ReqP["Where"].ToString(), ReqObj.UserToken);

             
                    DataTable BASE007M = DB.GetDataTable(
                        SJeMES_Framework_NETCore.Common.StringHelper.GetSqlCutPage(DB.DataBaseType,
                        SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary("base007m",
                        selectp, ReqP["Where"].ToString(), ReqObj.UserToken),
                        ReqP["PageRow"].ToString(),
                        ReqP["Page"].ToString(),
                        ReqP["OrderBy"].ToString()));


                int total = DB.GetInt32("select count(1) from base007m where 1=1 " + ReqP["Where"].ToString() + "");

                if (BASE007M.Rows.Count > 0)
                {
                    string headdata = string.Empty;
                    foreach (DataColumn dc in BASE007M.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "品号,品名,规格,单位";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(BASE007M);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "";
                }


            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;

        }

        /// <summary>
        /// 获取ART资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GeArtListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and PROD_NO like '%{KeyWord}%'";
                }

                string sql = $@" SELECT PROD_NO from bdm_rd_prod where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data",dt);
                    dic.Add("Total",count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取PO资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GePOListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and MER_PO like '%{KeyWord}%'";
                }

                string sql = $@" SELECT MER_PO from BDM_SE_ORDER_MASTER where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取工段资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkshopSectionListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( WORKSHOP_SECTION_NO like '%{KeyWord}%' or  WORKSHOP_SECTION_NAME like '%{KeyWord}%')";
                }

                string sql = $@" SELECT WORKSHOP_SECTION_NO,WORKSHOP_SECTION_NAME from bdm_workshop_section_m where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取部件信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPartsListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( PARTS_CODE like '%{KeyWord}%' or  PARTS_NAME like '%{KeyWord}%')";
                }

                string sql = $@" SELECT PARTS_CODE,PARTS_NAME from bdm_parts_m where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 部位 2023.05.23
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBWListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( POSITION_NAME like '%{KeyWord}%' )";
                }

                string sql = $@"SELECT DISTINCT POSITION_NAME FROM BDM_POSITION_M WHERE 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取Category信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCategoryListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and  NAME_T like '%{KeyWord}%' ";
                }

                string sql = $@"select distinct NAME_T from BDM_CD_CODE where LANGUAGE ='E' {Where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取鞋型名称信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoesListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion
               // var limit = " limit " + (int.Parse(page) - 1) * int.Parse(pageRow) + "," + pageRow;
                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and  NAME_T like '%{KeyWord}%' ";
                }

                //string sql = $@"select distinct SHOE_NO,NAME_T from bdm_rd_style where 1=1 {Where}";   old one
                string sql = $@"select distinct NAME_T from bdm_rd_style where 1=1 {Where}";   //Changed by Ashok on 2025/05/06
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql,Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取工作中心列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkcenterList(object OBJ)
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

                Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(
@" select productionline_no,productionline_name,productionline_description from base016m", Where, ReqObj.UserToken);


                int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                string sql = @"select * from (
select productionline_no,productionline_name,productionline_description,@n:= @n + 1 as RN from base016m M,(select @n:= 0) d
" + OrderBy + @") tab where  RN >" + total + " " + Where + "  limit " + PageRow + "";



                DataTable dt = DB.GetDataTable(sql);


                total = DB.GetInt32("select count(1) from base016m where 1=1 " + Where + "");

                if (dt.Rows.Count > 0)
                {
                    string headdata = string.Empty;
                    foreach (DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "工作中心,工作中心名称,工作中心描述";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);


                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
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
        /// 获取供应商列表接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupplierList(object OBJ)
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
@" select suppliers_code,suppliers_name from base003m", Where, ReqObj.UserToken);


                string sql = @"select * from (
select suppliers_code,suppliers_name,@n:= @n + 1 as RN from base003m M,(select @n:= 0) d
" + OrderBy + @"
) tab where  RN >" + total + " " + Where + "  limit " + PageRow + "";
                DataTable dt = DB.GetDataTable(sql);
                total  = DB.GetInt32("select count(1) from base003m where 1=1 " + Where + "");

                if (dt.Rows.Count > 0)
                {
                    string headdata = string.Empty;
                    foreach(DataColumn dc in dt.Columns)
                    {
                        headdata += dc.ColumnName + @",";
                    }
                    headdata.Remove(headdata.Length - 1);

                    string headkey = "供应商代号,供应商名称";
                    headdata = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonKeyValue(headkey, headdata);

                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("headdata", headdata);
                    p.Add("data", json);
                    p.Add("total", total);
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
        /// 获取厂商分类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrgList(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";

                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and M_TYPE like '%{KeyWord}%' ";
                }

                string sql = $@"SELECT DISTINCT M_TYPE  from BASE003M where 1=1 and M_TYPE is not null {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 厂区类型/厂商类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GeCQLandorgtypelistData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( tt.code like '%{KeyWord}%' or tt.name like '%{KeyWord}%')";
                }
                //厂商+厂区
                string sql = $@"SELECT * from (

SELECT DISTINCT M_TYPE as code, M_TYPE as name  from BASE003M  where 1=1 and M_TYPE is not null
union all
SELECT DISTINCT ORG_CODE as code,ORG_NAME as name FROM BASE001M  WHERE 1=1

)tt where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取厂商资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrgInfoList(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";

                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    //Where += $@" and M_TYPE like '%{KeyWord}%' ";
                    Where += $@" and ( SUPPLIERS_CODE LIKE '%{KeyWord}%' OR SUPPLIERS_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT SUPPLIERS_CODE,SUPPLIERS_NAME  from BASE003M where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 开发季度    2023.05.12
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetKFJDListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and SEASON like '%{KeyWord}%' ";
                }

                string sql = $@" SELECT DISTINCT SEASON FROM QCM_EX_TASK_LIST_M WHERE SEASON IS NOT NULL {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取阶段资料 2023.05.10
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJDListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and PHASE_CREATION_NAME like '%{KeyWord}%' ";
                }

                string sql = $@" SELECT DISTINCT PHASE_CREATION_NAME FROM QCM_EX_TASK_LIST_M WHERE PHASE_CREATION_NAME IS NOT NULL {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取厂区类型资料    2023.05.10
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCQTYPEListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion
                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( ORG_CODE LIKE '%{KeyWord}%' OR ORG_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT ORG_CODE,ORG_NAME FROM BASE001M WHERE 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 厂区资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GeCQListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( a.UDF05 like '%{KeyWord}%' or b.EN like '%{KeyWord}%')";
                }

                string sql = $@"
SELECT DISTINCT  a.UDF05,b.EN from BASE005M a
 left join SJQDMS_ORGINFO b on a.udf05 = b.code
where 1=1 and a.UDF05 is not null {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 厂区/厂商资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GeCQLandorgistData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( tt.code like '%{KeyWord}%' or tt.name like '%{KeyWord}%')";
                }
                //厂区+厂商
                string sql = $@" SELECT * from (
SELECT DISTINCT  a.UDF05 as code,b.EN as name from BASE005M a
 left join SJQDMS_ORGINFO b on a.udf05 = b.code
where 1=1 and a.UDF05 is not null
union all
SELECT DISTINCT SUPPLIERS_CODE as code,SUPPLIERS_NAME as name from BASE003M
)tt where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取部门资料  2023.05.10
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDepartListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( DEPARTMENT_CODE LIKE '%{KeyWord}%' OR DEPARTMENT_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT DEPARTMENT_CODE,DEPARTMENT_NAME FROM BASE005M where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取生产线资料 2023.05.10
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSCXListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( DEPARTMENT_CODE like '%{KeyWord}%' or DEPARTMENT_NAME like '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT DEPARTMENT_CODE PRODUCTION_LINE_CODE,DEPARTMENT_NAME PRODUCTION_LINE_NAME FROM BASE005M where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 成品仓资料 2023.05.15
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCPCListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( WAREHOUSE_CODE like '%{KeyWord}%' or WAREHOUSE_NAME like '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT WAREHOUSE_CODE,WAREHOUSE_NAME from mms_warehouse_manage  where wms_warehouse='Y' {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 储位资料 2023.05.15
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCWListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( LOCATION_CODE like '%{KeyWord}%' or LOCATION_NAME like '%{KeyWord}%')";
                }

                string sql = $@"select DISTINCT LOCATION_CODE,LOCATION_NAME from MMS_WAREHOUSE_SHELF_MANAGE where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 出货国家
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetGJistData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( C_NO like '%{KeyWord}%' or C_NAME like '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT C_NO,C_NAME FROM BDM_COUNTRY WHERE L_NO = 'EN' {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取设备类型资料 /设备种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEQTYPEListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( EQ_NO LIKE '%{KeyWord}%' or EQ_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT EQ_NO,EQ_NAME FROM bdm_eq_type_m where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取设备信息编号
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEQINFONOListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( EQ_INFO_NO LIKE '%{KeyWord}%' or EQ_INFO_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"SELECT DISTINCT EQ_INFO_NO,EQ_INFO_NAME from bdm_eq_info_m where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取测试项目
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetINSPECTIONistData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( INSPECTION_CODE LIKE '%{KeyWord}%' or INSPECTION_NAME LIKE '%{KeyWord}%')";
                }

                string sql = $@"
SELECT DISTINCT * from (
SELECT INSPECTION_CODE,INSPECTION_NAME from bdm_shoes_check_test_m 
union all 
SELECT INSPECTION_CODE,INSPECTION_NAME from bdm_material_testitem_m 
union all 
SELECT INSPECTION_CODE,INSPECTION_NAME from bdm_workmanship_testitem_m
union all 
SELECT INSPECTION_CODE,INSPECTION_NAME from bdm_parts_testitem_m 
) where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取设备编号 20230510确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetNETADRRListData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( NETADDR LIKE '%{KeyWord}%' or DEVNAME LIKE '%{KeyWord}%')";//Original One
                    //Where += $@" and ( a.eq_info_no LIKE '%{KeyWord}%' or a.eq_info_name LIKE '%{KeyWord}%')"; //Changed by Ashok 
                }

                string sql = $@"select DISTINCT NETADDR,DEVNAME from T_MSD_DEVLIST where 1=1 {Where}"; //Original Code

                //string sql = $@"  select a.eq_info_no,a.eq_info_name from bdm_eq_info_m a where 1=1 {Where}"; //Changed by Ashok 

                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 获取验货状态下拉框
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetYHStatusList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='enum_yh_status'";
                Dictionary<string, object> dic = GetDic(DB, sql);

                /*Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("0","未验货");
                dic.Add("1","已验货");
                dic.Add("2","已验货PASS");
                dic.Add("3","已验货FAIL");
                dic.Add("4","再次验货PASS");*/

                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 获取出货状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOutStatusList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='CHStatus'";
                Dictionary<string, object> dic = GetDic(DB, sql);

               /* Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("0", "已发货");
                dic.Add("1", "备库存");
                dic.Add("2", "生产中");*/


                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 新旧鞋
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetShoesTypesList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='ShoesType'";
                Dictionary<string, object> dic = GetDic(DB, sql);

                /*Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("0", "新鞋");
                dic.Add("1", "旧鞋");*/

                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 测试结果下拉
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestResList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='TestResult'";
                Dictionary<string, object> dic = GetDic(DB,sql);

                /*Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("0", "PASS");
                dic.Add("1", "FAIL");
                dic.Add("2", "全部");*/



                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 品质风险下拉
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetRiskOfQualityList(object OBJ)
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

                Dictionary<string, string> dic = new Dictionary<string, string>();
                DataTable dt = DB.GetDataTable($@"SELECT QA_RISK_CATEGORY_CODE,QA_RISK_CATEGORY_NAME from bdm_qa_risk_category_m");



                ret.IsSuccess = true;
                ret.RetData1 = dt;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 出货状态下拉框
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCHstatusList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='CHStatus'";
                Dictionary<string, object> dic = GetDic(DB,sql);

                /*Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("0", "已发货");
                dic.Add("1", "备库存");
                dic.Add("2", "生产中");*/

                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 实验室送测类型下拉   2023.05.10
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLABTestTypeList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='enum_lab_test_type'";
                Dictionary<string, object> dic = GetDic(DB, sql);

                

                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 库存呆滞状态（下拉） 2023.05.15
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDZPStatusList(object OBJ)
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

                string sql = $@"SELECT ENUM_CODE,ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE ='enum_dzp_status'";
                Dictionary<string, object> dic = GetDic(DB, sql);



                ret.IsSuccess = true;
                ret.RetData1 = dic;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        /// 工艺/材料种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPARAMData(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ( WORKMANSHIP_CODE like '%{KeyWord}%' or WORKMANSHIP_NAME like '%{KeyWord}%')";
                }

                string sql = $@"SELECT WORKMANSHIP_CODE,WORKMANSHIP_NAME FROM BDM_PARAM_ITEM_D where 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 料号资料    2023.05.16
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMATERIALList(object OBJ)
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
                string KeyWord = jarr.ContainsKey("KeyWord") ? jarr["KeyWord"].ToString() : "";//查询条件 
                string Page = jarr.ContainsKey("Page") ? jarr["Page"].ToString() : "1";//页数
                string PageRow = jarr.ContainsKey("PageRow") ? jarr["PageRow"].ToString() : "15";//行数

                #endregion

                //string limit = $@"AND ROWNUM >= {(Convert.ToInt32(Page) - 1) * Convert.ToInt32(PageRow)} and ROWNUM <=  {Convert.ToInt32(PageRow)}";
                string Where = string.Empty;
                if (!string.IsNullOrEmpty(KeyWord))
                {
                    Where += $@" and ITEM_NO like '%{KeyWord}%' ";
                }

                string sql = $@"SELECT DISTINCT ITEM_NO FROM BDM_RD_ITEM WHERE 1=1 {Where}";
                //DataTable dt = DB.GetDataTable(sql + limit);
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, Convert.ToInt32(Page), Convert.ToInt32(PageRow));
                int count = DB.GetInt32($"select count(1) from ({sql})tt");
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    dic.Add("Total", count);
                    ret.IsSuccess = true;
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }
                #region 逻辑

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
        /// 返回字典
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Dictionary<string, object> GetDic(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string sql)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            DataTable dt = DB.GetDataTablebyline(sql);

            if (dt.Rows.Count > 0)
            {
                for(int i=0; i<dt.Rows.Count;i++)
                {
                    ret.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
                }
            }
            return ret;
        }

    }
}
