using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace SJ_AQLAPI
{
    public class AQL_ConfirmShoes
    {
        /// <summary>
        /// 查询-确认鞋-仓库维护-主页-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoesWarehouse_Main(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string keycode = jarr.ContainsKey("keycode") ? jarr["keycode"].ToString() : "";//查询条件
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//模块类别
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and (WAREHOUSE_CODE like @keycode or WAREHOUSE_NAME like @keycode)";
                }

                string sql = $@"select id,WAREHOUSE_CODE,WAREHOUSE_NAME from aql_confirm_shoes_wh where MODULE_TYPE='{MODULE_TYPE}' {where} order by id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("keycode", $@"%{keycode}%");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 保存-确认鞋-仓库维护-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddConfirmShoesWarehouse(object OBJ)
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
                //转译
                string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//查询条件 仓库代号
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//查询条件 仓库名称
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_wh where MODULE_TYPE='{MODULE_TYPE}' and WAREHOUSE_CODE='{WAREHOUSE_CODE}'");
                if (count > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Warehouse code cannot be repeated!";
                    return ret;
                }

                string sql = $@"insert into aql_confirm_shoes_wh (MODULE_TYPE,WAREHOUSE_CODE,WAREHOUSE_NAME,createby,createdate,createtime) 
                                values('{MODULE_TYPE}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','{user}','{date}','{time}')";
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

        /// <summary>
        /// 删除-确认鞋-仓库维护-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoesWarehouse(object OBJ)
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
                //转译
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 仓库id
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = $@"delete from aql_confirm_shoes_wh where id='{id}'";
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

        /// <summary>
        /// 查询-确认鞋-库位维护-主页-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoesLocation_Main(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//模板列表
                string search_str = jarr.ContainsKey("search_str") ? jarr["search_str"].ToString() : "";//模糊查询内容

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string whereSql = "";
                if (!string.IsNullOrEmpty(search_str))
                {
                    whereSql = " and (STOCK_CODE like @search_str1 or WAREHOUSE_NAME like @search_str2 or STOCK_NAME like @search_str3)";
                    paramTestDic.Add("search_str1", $@"%{search_str}%");
                    paramTestDic.Add("search_str2", $@"%{search_str}%");
                    paramTestDic.Add("search_str3", $@"%{search_str}%");
                }

                string sql = $@"select 
                                id,
                                STOCK_CODE,
                                WAREHOUSE_NAME,
                                STOCK_NAME,
                                REMARK
                                from aql_confirm_shoes_arc 
                                where MODULE_TYPE='{MODULE_TYPE}' {whereSql}
                                order by WAREHOUSE_NAME,STOCK_CODE";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 删除-确认鞋-库位维护-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoesLocation(object OBJ)
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
                //转译
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 库位id
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = $@"delete from aql_confirm_shoes_arc where id='{id}'";
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

        /// <summary>
        /// 查询-确认鞋-库位维护-编辑-仓库-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoesLocation_Edit_ck(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string sql = $@"select WAREHOUSE_CODE as code,WAREHOUSE_NAME as value from aql_confirm_shoes_wh where MODULE_TYPE='{MODULE_TYPE}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询-确认鞋-库位维护-编辑-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoesLocation_Edit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string sid = jarr.ContainsKey("sid") ? jarr["sid"].ToString() : "";


                string sql = $@"select 
                                id,
                                STOCK_CODE,
                                WAREHOUSE_CODE,
                                WAREHOUSE_NAME,
                                STOCK_NAME,
                                remark
                                from aql_confirm_shoes_arc 
                                where id='{sid}'";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 编辑-确认鞋-库位维护-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoesLocation(object OBJ)
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
                //转译
                string sid = jarr.ContainsKey("sid") ? jarr["sid"].ToString() : "";//查询条件 库位id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//查询条件 库位代号
                string STOCK_NAME = jarr.ContainsKey("STOCK_NAME") ? jarr["STOCK_NAME"].ToString() : "";//查询条件 库位名称
                string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//查询条件 仓库代号
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//查询条件 仓库名称
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//查询条件 备注
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_arc where MODULE_TYPE='{MODULE_TYPE}' and  STOCK_CODE='{STOCK_CODE}'");
                if (string.IsNullOrWhiteSpace(sid))
                {
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "The location code cannot be repeated!";
                        return ret;
                    }
                }

                string sql = string.Empty;

                if (!string.IsNullOrWhiteSpace(sid))
                {
                    sql = $@"update aql_confirm_shoes_arc set STOCK_CODE='{STOCK_CODE}',STOCK_NAME='{STOCK_NAME}',WAREHOUSE_CODE='{WAREHOUSE_CODE}',
                            WAREHOUSE_NAME='{WAREHOUSE_NAME}',remark='{remark}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{sid}'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"insert into aql_confirm_shoes_arc (MODULE_TYPE,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,remark,createby,createdate,createtime) 
                            values('{MODULE_TYPE}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','{remark}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                }
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

        /// <summary>
        /// 查询-确认鞋-条码打印-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_BarcodePrint(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";

                string sql = $@"SELECT
	                            r.prod_no,
                                r.name_t as prod_name,
	                            r.shoe_no,
	                            d.name_t as shoe_name,
	                            r.develop_season,
	                            bb.name_t as rule_no,
                                T.file_url
                            FROM
	                            bdm_rd_prod r
	                            LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
	                            LEFT JOIN bdm_rd_style aa ON r.shoe_no=aa.shoe_no
                                LEFT JOIN bdm_cd_code bb ON aa.style_seq=bb.code_no
                                LEFT JOIN qcm_shoes_qa_record_m q ON r.shoe_no = q.shoes_code
                                LEFT JOIN BDM_UPLOAD_FILE_ITEM T ON q.image_guid = T .guid
                                where r.prod_no=@prod_no";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("prod_no", $@"{prod_no}");
                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询-确认鞋-存放管理-主页-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_Main(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//仓库
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//确认人
                string stock_name = jarr.ContainsKey("stock_name") ? jarr["stock_name"].ToString() : "";//存放位置(库位名称)
                string wh_dateS = jarr.ContainsKey("wh_dateS") ? jarr["wh_dateS"].ToString() : "";//入库日期开始
                string wh_dateE = jarr.ContainsKey("wh_dateE") ? jarr["wh_dateE"].ToString() : "";//入库日期结束
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//模板类别
                List<string> ref_standard = jarr.ContainsKey("ref_standard") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ref_standard"].ToString()) : null;//状态

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and d.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(WAREHOUSE_NAME))
                {
                    where += $@" and c.WAREHOUSE_NAME like @WAREHOUSE_NAME";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += $@" and r.prod_no like @prod_no";
                }
                if (!string.IsNullOrWhiteSpace(confirm_by))
                {
                    where += $@" and a.confirm_by like @confirm_by";
                }
                if (!string.IsNullOrWhiteSpace(stock_name))
                {
                    where += $@" and c.STOCK_NAME like @stock_name";
                }
                if (ref_standard.Count > 0)
                {
                    where += $@" and a.ref_standard in ({string.Join(",", ref_standard)})";
                }
                if (!string.IsNullOrWhiteSpace(wh_dateS) && !string.IsNullOrWhiteSpace(wh_dateE))
                {
                    where += $@" and a.wh_date>=@wh_dateS and a.wh_date<= @wh_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_dateS))
                    {
                        where += $@" and a.wh_date >= @wh_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_dateE))
                    {
                        where += $@" and a.wh_date <= @wh_dateE ";
                    }
                }
                string sql = string.Empty;

                sql = $@"SELECT
                            a.id as aid,
                            d.name_t as shoe_name,
                            r.prod_no,
                            a.STOCK_CODE,
                            c.STOCK_NAME,
                            c.WAREHOUSE_CODE,
							c.WAREHOUSE_NAME,
                            h.staff_name as confirm_by,
                            s.enum_value as state,
                            p.enum_value as FOOt,
                            '1'  as count,
                            'Only' as unit,
                            a.wh_date,
                            a.received_time,
                            a.confirmation_time,
                            a.scrap_life,
                            a.reminder_duration,
                            a.redo_reason
                        
                            FROM
                            aql_confirm_shoes_deposit a
                            LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                            LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                            LEFT JOIN aql_confirm_shoes_arc c on a.STOCK_CODE=c.STOCK_CODE and c.MODULE_TYPE='{MODULE_TYPE}'
                           LEFT JOIN SYS001M s on a.ref_standard=s.enum_code and s.enum_type='enum_ref_standard' 
							LEFT JOIN SYS001M p on a.FOOt=p.enum_code and p.enum_type='FootType' 
                            LEFT JOIN hr001m h ON a.confirm_by = h.staff_no
                            where a.MODULE_TYPE='{MODULE_TYPE}' {where}";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_no", $@"%{prod_no}%");
                paramTestDic.Add("confirm_by", $@"%{confirm_by}%");
                paramTestDic.Add("stock_name", $@"%{stock_name}%");
                paramTestDic.Add("ref_standard", $@"{ref_standard}");
                paramTestDic.Add("wh_dateS", $@"{wh_dateS}");
                paramTestDic.Add("wh_dateE", $@"{wh_dateE}");
                paramTestDic.Add("WAREHOUSE_NAME", $@"%{WAREHOUSE_NAME}%");

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 保存-确认鞋-存放管理-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddConfirmShoes_Store(object OBJ)
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
                //转译
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//查询条件 储位编号(库位代号)
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string prod_nos = jarr.ContainsKey("prod_nos") ? jarr["prod_nos"].ToString() : "";//查询条件 鞋子二维码(art编号;报废年限;待报废提醒时长;确认鞋归属)
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string[] arts = prod_nos.Split(';');
                if (arts.Length < 4)
                {
                    ret.ErrMsg = "The shoe QR code information is not complete!";
                    ret.IsSuccess = false;
                    return ret;
                }
                string prod_no = arts[0];//art
                string scrap_life = arts[1];//报废年限
                string reminder_duration = arts[2];//待报废提醒时间
                string ascription = arts[3];//确认鞋归属
                string foot = arts[4];//左右脚

                DataTable dtArt = DB.GetDataTable($@"select prod_no,develop_season from bdm_rd_prod where prod_no='{prod_no}'");//量产日期
                if (dtArt.Rows.Count <= 0)
                {
                    ret.ErrMsg = "There is no such information in the QR code of the shoes!";
                    ret.IsSuccess = false;
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_deposit where MODULE_TYPE='{MODULE_TYPE}' and art_no='{prod_no}' and ref_standard !='3' and foot = '{foot}'");
                if (count > 0)
                {
                    ret.ErrMsg = "Shoes QR code information cannot be added repeatedly!";
                    ret.IsSuccess = false;
                    return ret;
                }
                DataTable dtkw = DB.GetDataTable($@"select STOCK_CODE from aql_confirm_shoes_arc where MODULE_TYPE='{MODULE_TYPE}' and  STOCK_CODE='{STOCK_CODE}'");
                if (dtkw.Rows.Count <= 0)
                {
                    ret.ErrMsg = "Storage number No such information found!";
                    ret.IsSuccess = false;
                    return ret;
                }
                if (ascription == "inspection room")
                    ascription = "0";
                else if (ascription == "Raw material inspection unit")
                    ascription = "1";

                //string STOCK_NAME = string.Empty;
                //string WAREHOUSE_CODE = string.Empty;
                //string WAREHOUSE_NAME = string.Empty;
                //DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '0'");//库位代号
                //if (dtt.Rows.Count > 0)
                //{
                //    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                //    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                //    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                //}

                string sql = $@"insert into aql_confirm_shoes_deposit (MODULE_TYPE,art_no,STOCK_CODE,ref_standard,scrap_life,reminder_duration,wh_date,
                                ascription,foot,createby,createdate,createtime) 
                                values('{MODULE_TYPE}','{prod_no}','{STOCK_CODE}','0','{scrap_life}','{reminder_duration}','{date}','{ascription}','{foot}','{user}','{date}','{time}')";
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

        /// <summary>
        /// 保存-确认鞋-存放管理-重做-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddConfirmShoes_Store_Redo(object OBJ)
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
                //转译
                string prod_nos = jarr.ContainsKey("prod_nos") ? jarr["prod_nos"].ToString() : "";//查询条件 鞋子二维码
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//查询条件 库位编号
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//查询条件 确认人
                string redo_reason = jarr.ContainsKey("redo_reason") ? jarr["redo_reason"].ToString() : "";//查询条件 重做原因
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string[] arts = prod_nos.Split(';');
                if (arts.Length < 4)
                {
                    ret.ErrMsg = "The information provided by the shoe QR code is incomplete.!";
                    ret.IsSuccess = false;
                    return ret;
                }
                string prod_no = arts[0];//art
                string scrap_life = arts[1];//报废年限
                string reminder_duration = arts[2];//待报废提醒时间
                string ascription = arts[3];//确认鞋归属

                DataTable dtArt = DB.GetDataTable($@"select prod_no,develop_season from bdm_rd_prod where prod_no='{prod_no}'");//量产日期
                if (dtArt.Rows.Count <= 0)
                {
                    ret.ErrMsg = "There is no such information in the shoe QR code!";
                    ret.IsSuccess = false;
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from HR001M where staff_no='{confirm_by}'");
                if (count < 0)
                {
                    ret.ErrMsg = "The confirmor does not exist!";
                    ret.IsSuccess = false;
                    return ret;
                }

                DataTable dtkw = DB.GetDataTable($@"select STOCK_CODE from aql_confirm_shoes_arc where MODULE_TYPE='{MODULE_TYPE}' and  STOCK_CODE='{STOCK_CODE}'");
                if (dtkw.Rows.Count <= 0)
                {
                    ret.ErrMsg = "No information found for storage location number!";
                    ret.IsSuccess = false;
                    return ret;
                }
                if (ascription == "验货室")
                    ascription = "0";
                else if (ascription == "原材料检验股")
                    ascription = "1";

                string sql = $@"update aql_confirm_shoes_deposit set redo_reason='{redo_reason}',confirm_by='{confirm_by}',ref_standard='0',STOCK_CODE='{STOCK_CODE}',scrap_life='{scrap_life}',reminder_duration='{reminder_duration}',
                                    modifyby='{user}',modifydate='{date}',modifytime='{time}' where art_no='{prod_no}'";
                DB.ExecuteNonQuery(sql);

                string aid = DB.GetString($@"select id from aql_confirm_shoes_deposit where art_no='{prod_no}'");

                sql = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{redo_reason}','{aid}','{prod_no}','{STOCK_CODE}','0','{user}','{user}','{date}','{time}')";
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

        /// <summary>
        /// 编辑-确认鞋-存放管理_借出/归还-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_jc_gh(object OBJ)
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
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 存放管理id
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//查询条件 状态
                string opra_by = jarr.ContainsKey("opra_by") ? jarr["opra_by"].ToString() : "";//查询条件 操作人
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//
                string STOCK_NAME = jarr.ContainsKey("STOCK_NAME") ? jarr["STOCK_NAME"].ToString() : "";//
                string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from hr001m where staff_no='{opra_by}'");
                if (count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "the person does not exist!";
                    return ret;
                }

                string sql = string.Empty;
                sql = $@"update aql_confirm_shoes_deposit set ref_standard='{ref_standard}' where id='{aid}'";
                DB.ExecuteNonQuery(sql);
                string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                //string STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{aid}'");//库位代号
                if (ref_standard == "0")
                {
                    ref_standard = "4";
                }
                sql = $@"insert into aql_confirm_shoes_deposit_d 
                                (union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{aid}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','{ref_standard}','{opra_by}','{user}','{date}','{time}')";
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

        /// <summary>
        /// 编辑-确认鞋-存放管理-批量报废-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoes_Store_plbf(object OBJ)
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
                //转译
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//查询条件 报废原因
                string staff_no = jarr.ContainsKey("staff_no") ? jarr["staff_no"].ToString() : "";
                DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sqld = string.Empty;

                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;
                foreach (DataRow item in confirm.Rows)
                {

                    if (item["xz"].ToString() == "True")
                    {
                        string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{item["aid"]}'");
                        string STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{item["aid"]}'");//库位代号
                        sql += $@"update aql_confirm_shoes_deposit set ref_standard='1',REDO_REASON ='{remark}' where id='{item["aid"]}';";

                        DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '0'");
                        if (dtt.Rows.Count > 0)
                        {
                            STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                            WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                            WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                        }
                        sqld += $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,remark,createby,createdate,createtime) 
                        values('{item["aid"]}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','1','{staff_no}','{remark}','{user}','{date}','{time}');";
                    }
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");

                if (!string.IsNullOrWhiteSpace(sqld))
                    DB.ExecuteNonQuery($@"BEGIN {sqld} END;");

                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Edited Successfully";

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

        /// <summary>
        /// 编辑-确认鞋-存放管理-确认有效期-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoes_Store_qryxq(object OBJ)
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
                //转译
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//查询条件 确认人
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 单个更新确认有效期
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 单个更新确认有效期
                DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sqld = string.Empty;
                if (confirm.Rows.Count > 0)
                {
                    foreach (DataRow item in confirm.Rows)
                    {
                        if (item["xz"].ToString() == "True")
                        {
                            string STOCK_CODE = string.Empty;
                            string STOCK_NAME = string.Empty;
                            string WAREHOUSE_CODE = string.Empty;
                            string WAREHOUSE_NAME = string.Empty;
                            string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{item["aid"]}'");
                            string ref_standard = DB.GetString($@"SELECT ref_standard FROM aql_confirm_shoes_deposit WHERE id = '{item["aid"]}'");
                            if (ref_standard == "6")
                            {
                                //throw new Exception($@"当前所选序号为【{item["序号"].ToString()}】数据为重做状态不可继续操作！");
                                throw new Exception($@"The currently selected serial number is【{item["序号"].ToString()}】The data is in redo status and operations cannot continue.！");
                            }


                            sql += $@"update aql_confirm_shoes_deposit set ref_standard='5',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{item["aid"]}';";
                            STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{item["aid"]}'");//库位代号
                            DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                            if (dtt.Rows.Count > 0)
                            {
                                STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                                WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                                WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                            }

                            sqld += $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{item["aid"]}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','5','{confirm_by}','{user}','{date}','{time}');";
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                        DB.ExecuteNonQuery($@"BEGIN {sql} END;");

                    if (!string.IsNullOrWhiteSpace(sqld))
                        DB.ExecuteNonQuery($@"BEGIN {sqld} END;");
                }
                if (!string.IsNullOrWhiteSpace(aid))
                {
                    string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                    string STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{aid}'");//库位代号
                    sql = $@"update aql_confirm_shoes_deposit set ref_standard='5',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{aid}'";
                    DB.ExecuteNonQuery(sql);
                    sqld = $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{aid}','{art_no}','{STOCK_CODE}','5','{confirm_by}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sqld);
                }

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

        /// <summary>
        /// 单一更新有效期
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoes_Store_qryxq2(object OBJ)
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
                //转译
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//查询条件 确认人
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 单个更新确认有效期
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 单个更新确认有效期
                //DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sqld = string.Empty;


                string STOCK_CODE = string.Empty;
                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;
                string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                string ref_standard = DB.GetString($@"SELECT ref_standard FROM aql_confirm_shoes_deposit WHERE id = '{aid}'");
                if (ref_standard == "6")
                {
                    throw new Exception($@"The current data is in the redo state and cannot continue to operate！");
                }

                sql += $@"update aql_confirm_shoes_deposit set ref_standard='5',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{aid}';";
                STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{aid}'");//库位代号
                DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                if (dtt.Rows.Count > 0)
                {
                    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                }

                sqld += $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{aid}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','5','{confirm_by}','{user}','{date}','{time}');";


                if (!string.IsNullOrWhiteSpace(sql))
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");

                if (!string.IsNullOrWhiteSpace(sqld))
                    DB.ExecuteNonQuery($@"BEGIN {sqld} END;");

                //if (!string.IsNullOrWhiteSpace(aid))
                //{
                //    string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                //    string STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{aid}'");//库位代号
                //    sql = $@"update aql_confirm_shoes_deposit set ref_standard='5',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{aid}'";
                //    DB.ExecuteNonQuery(sql);
                //    sqld = $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                //        values('{aid}','{art_no}','{STOCK_CODE}','5','{confirm_by}','{user}','{date}','{time}')";
                //    DB.ExecuteNonQuery(sqld);
                //}

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

        /// <summary>
        /// 编辑-确认鞋-存放管理-删除-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoes_Store(object OBJ)
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
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 存放管理id
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                DB.ExecuteNonQuery($@"delete from aql_confirm_shoes_deposit where id='{aid}'");
                DB.ExecuteNonQuery($@"delete from aql_confirm_shoes_deposit_d where union_id='{aid}'");
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

        /// <summary>
        /// 编辑-确认鞋-存放管理-出库-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteConfirmShoes_Store_ck(object OBJ)
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
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 存放管理id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 存放管理id
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";//查询条件 存放管理id
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                string STOCK_CODE = DB.GetString($@"select STOCK_CODE from aql_confirm_shoes_deposit where id='{aid}'");//库位代号
                sql = $@"update aql_confirm_shoes_deposit set ref_standard='3',REDO_REASON = '{reason}',confirmation_time='{date}',STOCK_CODE='' where id='{aid}'";
                DB.ExecuteNonQuery(sql);
                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;
                DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                if (dtt.Rows.Count > 0)
                {
                    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                }

                sql = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{reason}','{aid}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','3','{user}','{user}','{date}','{time}')";
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

        /// <summary>
        /// 查询-确认鞋-存放管理-主页-导出-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_Main_Excel(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//确认人
                string stock_name = jarr.ContainsKey("stock_name") ? jarr["stock_name"].ToString() : "";//存放位置(库位名称)
                string wh_dateS = jarr.ContainsKey("wh_dateS") ? jarr["wh_dateS"].ToString() : "";//入库日期开始
                string wh_dateE = jarr.ContainsKey("wh_dateE") ? jarr["wh_dateE"].ToString() : "";//入库日期结束
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//模板类别
                List<string> ref_standard = jarr.ContainsKey("ref_standard") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ref_standard"].ToString()) : null;//状态

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and d.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    where += $@" and r.prod_no like @prod_no";
                }
                if (!string.IsNullOrWhiteSpace(confirm_by))
                {
                    where += $@" and a.confirm_by like @confirm_by";
                }
                if (!string.IsNullOrWhiteSpace(stock_name))
                {
                    where += $@" and c.STOCK_NAME like @stock_name";
                }
                if (ref_standard.Count > 0)
                {
                    where += $@" and a.ref_standard in ({string.Join(",", ref_standard)})";
                }
                if (!string.IsNullOrWhiteSpace(wh_dateS) && !string.IsNullOrWhiteSpace(wh_dateE))
                {
                    where += $@" and a.wh_date>=@wh_dateS and a.wh_date<= @wh_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_dateS))
                    {
                        where += $@" and a.wh_date >= @wh_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_dateE))
                    {
                        where += $@" and a.wh_date <= @wh_dateE ";
                    }
                }
                string sql = string.Empty;

                sql = $@"SELECT
                            a.id as aid,
                            d.name_t as shoe_name,
                            r.prod_no,
                            c.STOCK_NAME,
                            a.confirm_by,
                            s.enum_value as state,
                            '1'  as count,
                            '只' as unit,
                            a.wh_date,
                            a.received_time,
                            a.confirmation_time,
                            a.scrap_life,
                            a.reminder_duration,
                            a.redo_reason
                            FROM
                            aql_confirm_shoes_deposit a
                            LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                            LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                            LEFT JOIN aql_confirm_shoes_arc c on a.STOCK_CODE=c.STOCK_CODE and c.MODULE_TYPE='{MODULE_TYPE}'
                            LEFT JOIN SYS001M s on a.ref_standard=s.enum_code and enum_type='enum_ref_standard' 
                            where a.ref_standard!='3' and a.MODULE_TYPE='{MODULE_TYPE}' {where}";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_no", $@"%{prod_no}%");
                paramTestDic.Add("confirm_by", $@"%{confirm_by}%");
                paramTestDic.Add("stock_name", $@"%{stock_name}%");
                paramTestDic.Add("ref_standard", $@"{ref_standard}");
                paramTestDic.Add("wh_dateS", $@"{wh_dateS}");
                paramTestDic.Add("wh_dateE", $@"{wh_dateE}");

                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 编辑-确认鞋-存放管理-更新接收日期-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_jsrq(object OBJ)
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
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 存放管理id
                string received_time = jarr.ContainsKey("received_time") ? jarr["received_time"].ToString() : "";//查询条件 接收日期
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                sql = $@"update aql_confirm_shoes_deposit set received_time='{received_time}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{aid}'";
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

        /// <summary>
        /// 查询-确认鞋-存放管理-主页-打印-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_Print(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : ""; //存放管理id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";

                string sql = string.Empty;
                //sql = $@"SELECT 
                //        a.art_no as PROD_NO,
                //        d.name_t as shoe_name,
                //        a.scrap_life,
                //        a.reminder_duration,
                //        a.ascription as ATTRIBUTION,
                //        s.ENUM_VALUE as FOOT
                //        FROM
                //        aql_confirm_shoes_deposit a
                //        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                //        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                //        LEFT JOIN SYS001M s ON a.FOOT = s.enum_code and ENUM_TYPE = 'FootType'
                //        where a.id='{aid}'"; //OLD QUERY


                //sql = $@"SELECT 
                //        a.art_no as PROD_NO,
                //        d.name_t as shoe_name,
                //        a.scrap_life,
                //        a.reminder_duration,
                //        a.ascription as ATTRIBUTION,
                //        s.ENUM_VALUE as FOOT,
                //        e.staff_name,
                //        r.develop_season, 
                //        a.wh_date,
                //        '' as review_date,
                //        a.stock_code,
                //        f.stock_name
                //        FROM
                //        aql_confirm_shoes_deposit a
                //        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                //        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                //        LEFT JOIN hr001m e on a.createby=e.staff_no
                //        LEFT JOIN aql_confirm_shoes_arc f on  f.stock_code=a.stock_code and f.MODULE_TYPE='{MODULE_TYPE}'
                //        LEFT JOIN SYS001M s ON a.FOOT = s.enum_code and ENUM_TYPE = 'FootType' 
                //        where a.id='{aid}'";//NEW QUERY BY ASHOK

                sql = $@"with tmp as(select (CASE WHEN a.confirm_by is null THEN a.createby ELSE a.confirm_by END) as inspector,a.art_no from aql_confirm_shoes_deposit a where a.MODULE_TYPE='{MODULE_TYPE}') 
                        SELECT 
                        a.id,
                        a.art_no as PROD_NO,
                        d.name_t as shoe_name,
                        a.scrap_life,
                        a.reminder_duration,
                        a.ascription as ATTRIBUTION,
                        s.ENUM_VALUE as FOOT,
                        e.staff_name,
                        r.develop_season, 
                        CASE WHEN a.confirmation_time is null THEN a.wh_date ELSE a.confirmation_time END as wh_date, 
                        '' as review_date,
                        a.stock_code,
                        f.stock_name
                        FROM
                        aql_confirm_shoes_deposit a
                        LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                        LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                       inner JOIN tmp t on t.art_no=a.art_no
                        LEFT JOIN hr001m e on t.inspector=e.staff_no
                        LEFT JOIN aql_confirm_shoes_arc f on  f.stock_code=a.stock_code and f.MODULE_TYPE='{MODULE_TYPE}'
                        LEFT JOIN SYS001M s ON a.FOOT = s.enum_code and ENUM_TYPE = 'FootType'  
                        where a.id='{aid}'";//NEW MODIFIED QUERY BY ASHOK

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询-确认鞋-存放管理-操作记录-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_State(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//存放管理id
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//状态

                string where = string.Empty;
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(ref_standard))
                {
                    where = $@" and d.ref_standard=@ref_standard";
                    paramTestDic.Add("ref_standard", $@"{ref_standard}");
                }
                string sql = string.Empty;

                //          sql = $@"SELECT
                //              d.id AS did,
                //               s.enum_value AS ref_standard_name, -- 操作
                //                   d.ref_standard ,
                //CASE
                //                d.ref_standard 
                //                WHEN '2' THEN
                //                h.staff_name ELSE '-'
                //               END AS jyr,-- 借用人
                //CASE
                //                d.ref_standard 
                //                WHEN '4' THEN
                //                h.staff_name ELSE '-'
                //               END AS ghr,-- 归还人
                //                  CASE
                //                d.ref_standard 
                //                WHEN '5' THEN
                //                h.staff_name ELSE '-'
                //               END AS qrr, -- 确认人
                //                  f.WAREHOUSE_NAME,
                //f.STOCK_NAME,
                //                  h.staff_name operator, -- 操作人
                //d.createdate,-- 时间
                //               d.remark -- 备注
                //              FROM
                //               aql_confirm_shoes_deposit_d d
                //                  LEFT JOIN aql_confirm_shoes_arc f ON d.STOCK_CODE = f.STOCK_CODE
                //               LEFT JOIN SYS001M s ON d.ref_standard = s.enum_code 
                //               AND s.enum_type = 'enum_ref_standard'
                //               LEFT JOIN hr001m h ON d.opra_by = h.staff_no 
                //              WHERE
                //               1=1 and d.union_id =@union_id {where} 
                //              ORDER BY
                //               d.id DESC";

                sql = $@"	SELECT d.union_id,
	                   d.id AS did,
	                    s.enum_value AS ref_standard_name, -- 操作
                         d.ref_standard ,
						CASE
		                    d.ref_standard 
		                    WHEN '2' THEN
		                    h.staff_name ELSE '-'
	                    END AS jyr,-- 借用人
						CASE
		                    d.ref_standard 
		                    WHEN '4' THEN
		                    h.staff_name ELSE '-'
	                    END AS ghr,-- 归还人
                        h.staff_name AS qrr, -- 确认人
                        d.WAREHOUSE_NAME,
						d.STOCK_NAME,
                        -- h.staff_name operator, -- 操作人
                        (SELECT STAFF_NAME FROM hr001m where staff_no = d.CREATEBY) as operator, -- 操作人
						d.createdate,-- 时间
	                    d.remark -- 备注
                    FROM
	                    aql_confirm_shoes_deposit_d d
                    
	                    LEFT JOIN SYS001M s ON d.ref_standard = s.enum_code 
	                    AND s.enum_type = 'enum_ref_standard'
	                    LEFT JOIN hr001m h ON d.opra_by = h.staff_no 
                    WHERE
	                    1=1 and d.union_id ='{aid}' {where} 
                    ORDER BY
	                    d.id DESC";

                //paramTestDic.Add("union_id", $@"{aid}");
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    //if (item["ref_standard"].ToString() == "2" || item["ref_standard"].ToString() == "4")
                    //{
                    //    item["operator"] = DB.GetString($@"select 
                    //                                        h.staff_name as operator
                    //                                     from 
                    //                                     aql_confirm_shoes_deposit_d d
                    //                                        LEFT JOIN hr001m h on d.opra_by=h.staff_no where d.id='{item["did"]}'");
                    //}
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 编辑-确认鞋-存放管理-确认有效期-pda-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_Confirm(object OBJ)
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
                //转译
                string qr_code = jarr.ContainsKey("qr_code") ? jarr["qr_code"].ToString() : "";//查询条件 鞋子二维码(art编号;报废年限;待报废提醒时长;确认鞋归属)
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//确认人
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string[] codes = qr_code.Split(';');
                if (codes.Length < 4)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Shoes QR code is not standardized!";
                    return ret;
                }
                int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_deposit where MODULE_TYPE='0' and art_no='{codes[0]}'");
                if (count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "This information is not maintained!";
                    return ret;
                }

                sql = $@"update aql_confirm_shoes_deposit set scrap_life='{codes[1]}',reminder_duration='{codes[2]}',ascription='{codes[3]}',confirmation_time='{date}',confirm_by='{confirm_by}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where          MODULE_TYPE='0' and art_no='{codes[0]}'";
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

        /// <summary>
        /// 查询-确认鞋-存放管理-主页-状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_Main_zt(object OBJ)//AddConfirmShoes_Store_zt
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译

                string sql = $@"select enum_code as code,enum_value as value from SYS001M where enum_type='enum_ref_standard' and enum_code <>'4' ";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 编辑-确认鞋-存放管理-入库-aql
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_rk(object OBJ)
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
                //转译
                string aid = jarr.ContainsKey("aid") ? jarr["aid"].ToString() : "";//查询条件 存放管理id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 模板类别
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//查询条件 库位代号

                //string STOCK_NAME = jarr.ContainsKey("STOCK_NAME") ? jarr["STOCK_NAME"].ToString() : "";//查询条件 库位代号
                //string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//查询条件 库位代号
                //string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//查询条件 库位代号

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;

                int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_arc where MODULE_TYPE='{MODULE_TYPE}' and STOCK_CODE='{STOCK_CODE}'");
                if (count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The location code does not exist!";
                    return ret;
                }
                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;
                DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '0'");
                if (dtt.Rows.Count > 0)
                {
                    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();

                }

                string art_no = DB.GetString($@"select art_no from aql_confirm_shoes_deposit where id='{aid}'");
                sql = $@"update aql_confirm_shoes_deposit set ref_standard='0',STOCK_CODE='{STOCK_CODE}' where id='{aid}'";
                DB.ExecuteNonQuery(sql);

                sql = $@"insert into aql_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{aid}','{art_no}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','0','{user}','{user}','{date}','{time}')";
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



        /// <summary>
        /// 重做确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateConfirmShoes_Store_rk(object OBJ)
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
                //转译
                //DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//原因
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";////0-实验室 1原材料
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sql2 = string.Empty;
                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;

                //更新该调数据状态

                string STOCK_CODE = DB.GetString($@"SELECT STOCK_CODE FROM aql_confirm_shoes_deposit WHERE id = '{id}'");
                string ART_NO = DB.GetString($@"SELECT ART_NO FROM aql_confirm_shoes_deposit WHERE id = '{id}'");
                string ref_standard = DB.GetString($@"SELECT ref_standard FROM aql_confirm_shoes_deposit WHERE id = '{id}'");

                if (ref_standard == "6")
                {
                    throw new Exception($@"The current data is in redo status and operations cannot continue.！");
                }
                sql += $@"update aql_confirm_shoes_deposit set ref_standard='6',REDO_REASON = '{reason}',MODIFYBY='{user}',MODIFYDATE = '{date}',MODIFYTIME = '{time}',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{id}'";//,CONFIRM_BY='{confirm_by}'
                DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                if (dtt.Rows.Count > 0)
                {
                    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                }

                sql2 = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                            values('{reason}','{id}','{ART_NO}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','6','{confirm_by}','{user}','{date}','{time}')";

                DB.ExecuteNonQuery(sql);
                DB.ExecuteNonQuery(sql2);

                //增加操作记录

                //if (MODULE_TYPE == "0")
                //    ascription = "0";
                //else
                //    ascription = "1";



                //foreach (DataRow item in confirm.Rows)
                //{
                //    //    string STOCK_NAME = string.Empty;
                //    //    string WAREHOUSE_CODE = string.Empty;
                //    //    string WAREHOUSE_NAME = string.Empty;
                //    if (item["xz"].ToString().ToLower()=="true")
                //    {
                //        string foot = item["FOOT"].ToString() == "左脚" ? "0" : "1";
                //        DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{item["STOCK_CODE"]}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                //        if (dtt.Rows.Count > 0)
                //        {
                //            STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                //            WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                //            WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                //        }
                //        //新增新的一条数据
                //        //sql = $@"insert into aql_confirm_shoes_deposit (MODULE_TYPE,art_no,STOCK_CODE,ref_standard,reminder_duration,scrap_life,wh_date,
                //        //        ascription,TYPE,FOOT,createby,createdate,createtime) 
                //        //        values('{MODULE_TYPE}','{item["ART"]}','{item["STOCK_CODE"]}','0','{item["reminder_duration"]}','{item["scrap_life"]}','{item["入库日期"]}','{ascription}','1','{foot}','{user}','{date}','{time}')";

                //        sql += $@"update aql_confirm_shoes_deposit set ref_standard='6',REDO_REASON = '{reason}',MODIFYBY='{user}',MODIFYDATE = '{date}',MODIFYTIME = '{time}' where id='{item["aid"]}'";//,CONFIRM_BY='{confirm_by}'

                //        sql2 = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                //            values('{reason}','{item["aid"]}','{item["ART"]}','{item["STOCK_CODE"]}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','6','{user}','{user}','{date}','{time}')";

                //        DB.ExecuteNonQuery(sql);
                //        DB.ExecuteNonQuery(sql2);
                //    }

                //}



                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully！";

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

        /// <summary>
        /// 重做批量
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateConfirmShoes_Store_rk2(object OBJ)
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
                //转译
                DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                //string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//原因
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";////0-实验室 1原材料
                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//查询条件 确认人

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sql2 = string.Empty;
                string STOCK_NAME = string.Empty;
                string WAREHOUSE_CODE = string.Empty;
                string WAREHOUSE_NAME = string.Empty;

                //更新该调数据状态

                //string STOCK_CODE = DB.GetString($@"SELECT STOCK_CODE FROM aql_confirm_shoes_deposit WHERE id = '{id}'");
                //string ART_NO = DB.GetString($@"SELECT ART_NO FROM aql_confirm_shoes_deposit WHERE id = '{id}'");

                //sql += $@"update aql_confirm_shoes_deposit set ref_standard='6',MODIFYBY='{user}',MODIFYDATE = '{date}',MODIFYTIME = '{time}' where id='{id}'";//,CONFIRM_BY='{confirm_by}'
                //DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{STOCK_CODE}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                //if (dtt.Rows.Count > 0)
                //{
                //    STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                //    WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                //    WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                //}

                //sql2 = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                //            values('{reason}','{id}','{ART_NO}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','6','{user}','{user}','{date}','{time}')";

                //DB.ExecuteNonQuery(sql);
                //DB.ExecuteNonQuery(sql2);

                //增加操作记录

                //if (MODULE_TYPE == "0")
                //    ascription = "0";
                //else
                //    ascription = "1";



                foreach (DataRow item in confirm.Rows)
                {
                    //    string STOCK_NAME = string.Empty;
                    //    string WAREHOUSE_CODE = string.Empty;
                    //    string WAREHOUSE_NAME = string.Empty;
                    if (item["xz"].ToString().ToLower() == "true")
                    {
                        string ref_standard = DB.GetString($@"SELECT ref_standard FROM aql_confirm_shoes_deposit WHERE id = '{item["aid"]}'");
                        if (ref_standard == "6")
                        {
                            //throw new Exception($@"当前所选序号为【{item["序号"].ToString()}】数据为重做状态不可继续操作！");
                            throw new Exception($@"The currently selected serial number is【{item["序号"].ToString()}】The data is in redo status and operations cannot continue.！");
                        }
                        string foot = item["FOOT"].ToString() == "左脚" ? "0" : "1";
                        DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{item["STOCK_CODE"]}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                        if (dtt.Rows.Count > 0)
                        {
                            STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                            WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                            WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                        }
                        //新增新的一条数据
                        //sql = $@"insert into aql_confirm_shoes_deposit (MODULE_TYPE,art_no,STOCK_CODE,ref_standard,reminder_duration,scrap_life,wh_date,
                        //        ascription,TYPE,FOOT,createby,createdate,createtime) 
                        //        values('{MODULE_TYPE}','{item["ART"]}','{item["STOCK_CODE"]}','0','{item["reminder_duration"]}','{item["scrap_life"]}','{item["入库日期"]}','{ascription}','1','{foot}','{user}','{date}','{time}')";

                        //修改状态
                        sql = $@"update aql_confirm_shoes_deposit set ref_standard='6',REDO_REASON = '{reason}',MODIFYBY='{user}',MODIFYDATE = '{date}',MODIFYTIME = '{time}',confirmation_time='{date}',confirm_by='{confirm_by}' where id='{item["aid"]}'";//,CONFIRM_BY='{confirm_by}'

                        //操作记录
                        sql2 = $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                            values('{reason}','{item["aid"]}','{item["ART"]}','{item["STOCK_CODE"]}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','6','{confirm_by}','{user}','{date}','{time}')";

                        DB.ExecuteNonQuery(sql);
                        DB.ExecuteNonQuery(sql2);
                    }

                }



                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully！";

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

        /// <summary>
        /// 查询重做列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_cz(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                //转译
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//


                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//

                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//

                string confirm_by = jarr.ContainsKey("confirm_by") ? jarr["confirm_by"].ToString() : "";//

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                    where += $@" and shoe_name like @shoe_name ";
                }
                if (!string.IsNullOrWhiteSpace(prod_no))
                {
                    paramTestDic.Add("prod_no", $@"%{prod_no}%");
                    where += $@" and prod_no like @prod_no ";
                }
                if (!string.IsNullOrWhiteSpace(confirm_by))
                {
                    paramTestDic.Add("confirm_by", $@"%{confirm_by}%");
                    where += $@" and confirm_by like @confirm_by ";
                }
                if (!string.IsNullOrWhiteSpace(starttime) || !string.IsNullOrWhiteSpace(endtime))
                {

                    paramTestDic.Add("starttime", $@"%{starttime + " 00:00:00"}%");
                    paramTestDic.Add("endtime", $@"%{endtime + " 23:59:59"}%");
                    where += $@" and TO_CHAR( received_time ,'yyyy-MM-dd HH24:mi:ss') BETWEEN @starttime AND @endtime ";
                }

                string sql = string.Empty;
                sql = $@"select * from (
                            SELECT
                            ROWNUM as RN,
                            a.id as aid,
                            a.STOCK_CODE,
                            c.STOCK_NAME as STOCK_NAME,
                            d.name_t as shoe_name,
                            r.prod_no,
                            s.enum_value as state,
                            -- TO_CHAR( a.received_time ,'yyyy-MM-dd HH24:mi:ss') as received_time ,
                            a.received_time as received_time ,
                            a.confirm_by,
                            a.redo_reason,
                            a.scrap_life, -- 待报废
							a.confirmation_time,--确认日期
							a.wh_date, -- 入库日期
                            a.reminder_duration,
                            a.FOOT,a.TYPE
                            FROM
                            aql_confirm_shoes_deposit a
                            LEFT JOIN bdm_rd_prod r on a.art_no=r.prod_no
                            LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
                            LEFT JOIN aql_confirm_shoes_arc c on a.STOCK_CODE=c.STOCK_CODE and c.MODULE_TYPE='{MODULE_TYPE}'
                            LEFT JOIN SYS001M s on a.ref_standard=s.enum_code and enum_type='enum_ref_standard' 
                            where a.MODULE_TYPE='{MODULE_TYPE}' AND a.ref_standard = '6')tab where 1=1 AND type IS NULL  {where}";



                DataTable dt = DB.GetDataTable(sql, paramTestDic);
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;



            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 代报废产生新art
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddConfirmShoes_Store_rk(object OBJ)
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
                //转译
                string request = jarr.ContainsKey("request") ? jarr["request"].ToString() : "";//查询条件 存放管理id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//查询条件 存放管理id
                string ascription = jarr.ContainsKey("ascription") ? jarr["ascription"].ToString() : "";//0-实验室 1原材料


                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sql2 = string.Empty;

                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(request.ToString());

                foreach (var item in data)
                {
                    string STOCK_NAME = string.Empty;
                    string WAREHOUSE_CODE = string.Empty;
                    string WAREHOUSE_NAME = string.Empty;
                    DataTable dtt = DB.GetDataTable($@"SELECT * FROM aql_confirm_shoes_arc WHERE STOCK_CODE = '{item["STOCK_CODE"]}' AND MODULE_TYPE = '{MODULE_TYPE}'");//库位代号
                    if (dtt.Rows.Count > 0)
                    {
                        STOCK_NAME = dtt.Rows[0]["STOCK_NAME"].ToString();
                        WAREHOUSE_CODE = dtt.Rows[0]["WAREHOUSE_CODE"].ToString();
                        WAREHOUSE_NAME = dtt.Rows[0]["WAREHOUSE_NAME"].ToString();
                    }
                    //新增新的一条数据
                    sql = $@"insert into aql_confirm_shoes_deposit (MODULE_TYPE,art_no,STOCK_CODE,ref_standard,scrap_life,reminder_duration,wh_date,
                                ascription,FOOT,createby,createdate,createtime) 
                                values('{MODULE_TYPE}','{item["prod_no"]}','{item["STOCK_CODE"]}','0','{item["scrap_life"]}','{item["reminder_duration"]}','{item["wh_date"]}','{ascription}','{item["FOOT"]}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);

                    //变更自身标识为已生成重做任务
                    sql2 = $@"update aql_confirm_shoes_deposit set TYPE = '1' where id = '{item["id"]}'";
                    DB.ExecuteNonQuery(sql2);
                }

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


        /// <summary>
        /// 退开发提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddConfirmShoes_Store_tkf(object OBJ)
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
                //转译
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//查询条件 存放管理id
                string MODULE_TYPE = jarr.ContainsKey("MODULE_TYPE") ? jarr["MODULE_TYPE"].ToString() : "";//原因
                string reason = jarr.ContainsKey("reason") ? jarr["reason"].ToString() : "";//原因

                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//原因

                string STOCK_NAME = jarr.ContainsKey("STOCK_NAME") ? jarr["STOCK_NAME"].ToString() : "";//原因
                string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//原因
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//原因

                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//原因


                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sql2 = string.Empty;
                string sql3 = string.Empty;

                //更新该调数据状态
                sql += $@"update aql_confirm_shoes_deposit set ref_standard='7',STOCK_CODE = '', MODIFYBY='{user}',MODIFYDATE = '{date}',MODIFYTIME = '{time}' where id='{id}'";//,CONFIRM_BY='{confirm_by}'

                //增加操作记录
                sql2 += $@"insert into aql_confirm_shoes_deposit_d (remark,union_id,art_no,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{reason}','{id}','{ART}','{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','7','{user}','{user}','{date}','{time}')";

                //更新存放位置、仓库位置
                //sql3 += $@"";



                DB.ExecuteNonQuery(sql);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_staff_name(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string STAFF_NO = jarr.ContainsKey("STAFF_NO") ? jarr["STAFF_NO"].ToString() : "";//查询条件 存放管理id

                Dictionary<string, object> param = new Dictionary<string, object>();
                param.Add("STAFF_NO", STAFF_NO);
                string sql = $@"SELECT STAFF_NO,STAFF_NAME from HR001M where 1=1 and STAFF_NO = @STAFF_NO";

                DataTable dataTable = DB.GetDataTable(sql, param);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (dataTable.Rows.Count > 0)
                {
                    dic.Add("data", dataTable);

                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "No job number found！";
                    ret.IsSuccess = false;
                }


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
