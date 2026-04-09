using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJeMES_IQC
{
    public class IQC_ConfirmShoes
    {
        /// <summary>
        /// 查询-确认鞋-仓库维护-主页
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
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(keycode))
                {
                    where = $@" and (WAREHOUSE_CODE like @keycode or WAREHOUSE_NAME like @keycode)";
                }

                string sql = $@"select id,WAREHOUSE_CODE,WAREHOUSE_NAME from qcm_confirm_shoes_wh where 1=1 {where} order by id desc";
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
        /// 保存-确认鞋-仓库维护
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
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from qcm_confirm_shoes_wh where WAREHOUSE_CODE='{WAREHOUSE_CODE}'");
                if (count > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "仓库代号不能重复!";
                    return ret;
                }

                string sql = $@"insert into qcm_confirm_shoes_wh (WAREHOUSE_CODE,WAREHOUSE_NAME,createby,createdate,createtime) 
                                values('{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','{user}','{date}','{time}')";
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
        /// 删除-确认鞋-仓库维护
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

                string sql = $@"delete from qcm_confirm_shoes_wh where id='{id}'";
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
        /// 查询-确认鞋-库位维护-主页
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


                string sql = $@"select 
                                id,
                                STOCK_CODE,
                                WAREHOUSE_NAME,
                                STOCK_NAME,
                                remark,
                                ref_standard,
                                expire_day,
                                remind_day
                                from qcm_confirm_shoes_arc 
                                order by id desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
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
        /// 删除-确认鞋-库位维护
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

                string sql = $@"delete from qcm_confirm_shoes_arc where id='{id}'";
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
        /// 查询-确认鞋-库位维护-编辑-仓库
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

                string sql = $@"select WAREHOUSE_CODE as code,WAREHOUSE_NAME as value from qcm_confirm_shoes_wh";
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
        /// 查询-确认鞋-库位维护-编辑
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
                                remark,
                                ref_standard,
                                expire_day,
                                remind_day
                                from qcm_confirm_shoes_arc 
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
        /// 编辑-确认鞋-库位维护
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
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";//查询条件 库位代号
                string STOCK_NAME = jarr.ContainsKey("STOCK_NAME") ? jarr["STOCK_NAME"].ToString() : "";//查询条件 库位名称
                string WAREHOUSE_CODE = jarr.ContainsKey("WAREHOUSE_CODE") ? jarr["WAREHOUSE_CODE"].ToString() : "";//查询条件 仓库代号
                string WAREHOUSE_NAME = jarr.ContainsKey("WAREHOUSE_NAME") ? jarr["WAREHOUSE_NAME"].ToString() : "";//查询条件 仓库名称
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//查询条件 备注
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//查询条件 参照标准
                string expire_day = jarr.ContainsKey("expire_day") ? jarr["expire_day"].ToString() : "";//查询条件 到期时间
                string remind_day = jarr.ContainsKey("remind_day") ? jarr["remind_day"].ToString() : "";//查询条件 提醒时间
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from qcm_confirm_shoes_arc where STOCK_CODE='{STOCK_CODE}'");
                if (string.IsNullOrWhiteSpace(sid))
                {
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "库位代号不能重复!";
                        return ret;
                    }
                }

                string sql = string.Empty;

                if (!string.IsNullOrWhiteSpace(sid))
                {
                    sql = $@"update qcm_confirm_shoes_arc set STOCK_CODE='{STOCK_CODE}',STOCK_NAME='{STOCK_NAME}',WAREHOUSE_CODE='{WAREHOUSE_CODE}',
                            WAREHOUSE_NAME='{WAREHOUSE_NAME}',remark='{remark}',ref_standard='{ref_standard}',expire_day='{expire_day}',
                            remind_day='{remind_day}',modifyby='{user}',modifydate='{date}',modifytime='{time}' where id='{sid}'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                {
                    sql = $@"insert into qcm_confirm_shoes_arc (STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,remark,ref_standard,
                           expire_day,remind_day,createby,createdate,createtime) 
                            values('{STOCK_CODE}','{STOCK_NAME}','{WAREHOUSE_CODE}','{WAREHOUSE_NAME}','{remark}','{ref_standard}','{expire_day}',
                            '{remind_day}','{user}','{date}','{time}')";
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
        /// 查询-确认鞋-条码打印
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
        /// 查询-确认鞋-存放管理-主页
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
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型名称
                string prod_name = jarr.ContainsKey("prod_name") ? jarr["prod_name"].ToString() : "";//art名称
                string stock_name = jarr.ContainsKey("stock_name") ? jarr["stock_name"].ToString() : "";//存放位置(库位名称)
                string wh_dateS = jarr.ContainsKey("wh_dateS") ? jarr["wh_dateS"].ToString() : "";//入库日期开始
                string wh_dateE = jarr.ContainsKey("wh_dateE") ? jarr["wh_dateE"].ToString() : "";//入库日期结束
                string output_dateS = jarr.ContainsKey("output_dateS") ? jarr["output_dateS"].ToString() : "";//量产日期开始
                string output_dateE = jarr.ContainsKey("output_dateE") ? jarr["output_dateE"].ToString() : "";//量产日期结束
                List<string> ref_standard = jarr.ContainsKey("ref_standard") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ref_standard"].ToString()) : null;//状态

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and d.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(prod_name))
                {
                    where += $@" and r.prod_no like @prod_name";
                }
                if (!string.IsNullOrWhiteSpace(stock_name))
                {
                    where += $@" and a.stock_name like @stock_name";
                }
                if (!string.IsNullOrWhiteSpace(wh_dateS) && !string.IsNullOrWhiteSpace(wh_dateE))
                {
                    where += $@" and q.wh_date>=@wh_dateS and q.wh_date<= @wh_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_dateS))
                    {
                        where += $@" and q.wh_date >= @wh_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_dateE))
                    {
                        where += $@" and q.wh_date <= @wh_dateE ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(output_dateS) && !string.IsNullOrWhiteSpace(output_dateE))
                {
                    where += $@" and q.output_date>=@output_dateS and q.output_date<= @output_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(output_dateS))
                    {
                        where += $@" and q.output_date >= @output_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(output_dateE))
                    {
                        where += $@" and q.output_date <= @output_dateE ";
                    }
                }
                if (ref_standard.Count > 0)
                {
                    where += $@" and q.ref_standard in ({string.Join(",", ref_standard)})";
                }


                string sql = $@"	select 
	                                q.id as qid,
	                                d.shoe_no,
	                                d.name_t as shoe_name,
	                                r.prod_no,
	                                r.name_t as prod_name,
	                                a.stock_code,
	                                a.stock_name,
                                    q.ref_standard,
	                                s.enum_value as ref_standard_name,
	                                q.wh_date,
	                                q.output_date,
	                                q.reconfirmation_time,
	                                q.expected_maturity_date,
                                    c.remind_day,
                                    c.ref_standard as czbz
	                                from 
	                                qcm_confirm_shoes_deposit q
	                                LEFT JOIN bdm_rd_prod r on q.art_no=r.prod_no
	                                LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
	                                LEFT JOIN qcm_confirm_shoes_arc a on q.stock_code=a.stock_code
	                                LEFT JOIN SYS001M s on q.ref_standard=s.enum_code and s.enum_type='enum_ref_standard'
                                    LEFT JOIN qcm_confirm_shoes_arc c on q.STOCK_CODE=c.STOCK_CODE
                                    where q.ref_standard!='3' {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_name", $@"%{prod_name}%");
                paramTestDic.Add("stock_name", $@"%{stock_name}%");
                paramTestDic.Add("wh_dateS", $@"{wh_dateS}");
                paramTestDic.Add("wh_dateE", $@"{wh_dateE}");
                paramTestDic.Add("output_dateS", $@"{output_dateS}");
                paramTestDic.Add("output_dateE", $@"{output_dateE}");

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
        /// 查询-确认鞋-存放管理-主页-状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetConfirmShoes_Store_Main_zt(object OBJ)
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

                string sql = $@"select enum_code as code,enum_value as value from SYS001M where enum_type='enum_ref_standard' and enum_code !='3'";
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
        /// 保存-确认鞋-存放管理
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
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//查询条件 鞋子二维码(art编号)
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                DataTable dtArt = DB.GetDataTable($@"select prod_no,develop_season from bdm_rd_prod where prod_no='{prod_no}'");//量产日期
                if (dtArt.Rows.Count <= 0)
                {
                    ret.ErrMsg = "There is no such information in the QR code of the shoes!";
                    ret.IsSuccess = false;
                    return ret;
                }

                int count = DB.GetInt32($@"select count(1) from qcm_confirm_shoes_deposit where art_no='{prod_no}' and ref_standard !='3'");
                if (count > 0)
                {
                    ret.ErrMsg = "Shoes QR code information cannot be added repeatedly!";//鞋子二维码信息不能重复添加
                    ret.IsSuccess = false;
                    return ret;
                }

                DataTable dtkw = DB.GetDataTable($@"select ref_standard,expire_day,STOCK_CODE from qcm_confirm_shoes_arc where STOCK_CODE='{STOCK_CODE}'");
                if (dtkw.Rows.Count <= 0)
                {
                    ret.ErrMsg = "Storage number No such information found!";//储位编号查无此信息
                    ret.IsSuccess = false;
                    return ret;
                }

                string union_id = DB.GetString($@"select id from qcm_confirm_shoes_deposit where art_no='{prod_no}' and ref_standard='3'");
                if (!string.IsNullOrWhiteSpace(union_id))
                {
                    string sqlde = $@"delete from qcm_confirm_shoes_deposit where art_no='{prod_no}'";
                    DB.ExecuteNonQuery(sqlde);
                }
                string qid = string.Empty;
                string sql = string.Empty;
                if (dtkw.Rows[0]["ref_standard"].ToString() == "0")
                {
                    string expected_maturity_date = (Convert.ToDateTime(date).AddDays(Convert.ToInt32(dtkw.Rows[0]["expire_day"].ToString()))).ToString("yyyy-MM-dd");//预计到期时间
                    sql = $@"insert into qcm_confirm_shoes_deposit (art_no,STOCK_CODE,ref_standard,wh_date,expected_maturity_date,createby,
                            createdate,createtime) 
                            values('{prod_no}','{STOCK_CODE}','0','{date}','{expected_maturity_date}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    qid= SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_confirm_shoes_deposit");
                }
                if (dtkw.Rows[0]["ref_standard"].ToString() == "1")
                {
                    string expected_maturity_date = (Convert.ToDateTime(date).AddDays(Convert.ToInt32(dtkw.Rows[0]["expire_day"].ToString()))).ToString("yyyy-MM-dd");//预计到期时间
                    sql = $@"insert into qcm_confirm_shoes_deposit (art_no,STOCK_CODE,ref_standard,output_date,expected_maturity_date,createby,
                            createdate,createtime) 
                            values('{prod_no}','{STOCK_CODE}','0','{date}','{expected_maturity_date}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    qid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_confirm_shoes_deposit");
                }

                if (!string.IsNullOrWhiteSpace(qid)&& !string.IsNullOrWhiteSpace(union_id))
                {
                    sql = $@"update qcm_confirm_shoes_deposit_d set union_id = '{qid}' where union_id='{union_id}'";
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
        /// 编辑-确认鞋-存放管理_报废/出库
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_bf_ck(object OBJ)
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
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString() : "";
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string sql = string.Empty;
                sql = $@"update qcm_confirm_shoes_deposit set ref_standard='{ref_standard}' where id='{qid}'";
                DB.ExecuteNonQuery(sql);
                string art_no = DB.GetString($@"select art_no from qcm_confirm_shoes_deposit where id='{qid}'");
                string STOCK_CODE = DB.GetString($@"select STOCK_CODE from qcm_confirm_shoes_deposit where id='{qid}'");
                sql = $@"insert into qcm_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{qid}','{art_no}','{STOCK_CODE}','{ref_standard}','{user}','{user}','{date}','{time}')";
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
        /// 编辑-确认鞋-存放管理_再确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditConfirmShoes_Store_zqr(object OBJ)
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
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString() : "";//查询条件 存放管理id
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//查询条件 状态
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string STOCK_CODE = DB.GetString($@"select STOCK_CODE from qcm_confirm_shoes_deposit where id='{qid}'");//库位代号
                string expire_day = DB.GetString($@"select expire_day from qcm_confirm_shoes_arc where STOCK_CODE='{STOCK_CODE}'");//到期时间

                string expected_maturity_date = (Convert.ToDateTime(date).AddDays(Convert.ToInt32(expire_day))).ToString("yyyy-MM-dd");//预计到期时间
                string sql = string.Empty;
                sql = $@"update qcm_confirm_shoes_deposit set ref_standard='{ref_standard}',reconfirmation_time='{date}',expected_maturity_date='{expected_maturity_date}' 
                        where id='{qid}'";
                DB.ExecuteNonQuery(sql);

                string art_no = DB.GetString($@"select art_no from qcm_confirm_shoes_deposit where id='{qid}'");
                sql = $@"insert into qcm_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{qid}','{art_no}','{STOCK_CODE}','{ref_standard}','{user}','{user}','{date}','{time}')";
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
        /// 删除-确认鞋-存放管理
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
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString() : "";//查询条件 存放管理id
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                DB.ExecuteNonQuery($@"delete from qcm_confirm_shoes_deposit where id='{qid}'");

                DB.ExecuteNonQuery($@"delete from qcm_confirm_shoes_deposit_d where union_id='{qid}'");

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
        /// 编辑-确认鞋-存放管理-批量报废
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
                DataTable confirm = jarr.ContainsKey("confirm") ? Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["confirm"].ToString()) : null;//查询条件 存放管理数据
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                string sql = string.Empty;
                string sqld = string.Empty;
                foreach (DataRow item in confirm.Rows)
                {
                    if (item["xz"].ToString() == "True")
                    {
                        string art_no = DB.GetString($@"select art_no from qcm_confirm_shoes_deposit where id='{item["qid"]}'");
                        string STOCK_CODE = DB.GetString($@"select STOCK_CODE from qcm_confirm_shoes_deposit where id='{item["qid"]}'");//库位代号
                        sql += $@"update qcm_confirm_shoes_deposit set ref_standard='1' where id='{item["qid"]}';";

                        sqld += $@"insert into qcm_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{item["qid"]}','{art_no}','{STOCK_CODE}','1','{user}','{user}','{date}','{time}');";
                    }
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    DB.ExecuteNonQuery($@"BEGIN {sql} END;");

                if (!string.IsNullOrWhiteSpace(sqld))
                    DB.ExecuteNonQuery($@"BEGIN {sqld} END;");

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
        /// 编辑-确认鞋-存放管理_借出/归还
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
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString() : "";//查询条件 存放管理id
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//查询条件 状态
                string opra_by= jarr.ContainsKey("opra_by") ? jarr["opra_by"].ToString() : "";//查询条件 操作人
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                int count = DB.GetInt32($@"select count(1) from hr001m where staff_no='{opra_by}'");
                if (count<=0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "该人员不存在!";
                    return ret;
                }

                string sql = string.Empty;
                sql = $@"update qcm_confirm_shoes_deposit set ref_standard='{ref_standard}' where id='{qid}'";
                DB.ExecuteNonQuery(sql);
                string art_no = DB.GetString($@"select art_no from qcm_confirm_shoes_deposit where id='{qid}'");
                string STOCK_CODE = DB.GetString($@"select STOCK_CODE from qcm_confirm_shoes_deposit where id='{qid}'");//库位代号
                if (ref_standard=="0")
                {
                    ref_standard = "4";
                }
                sql = $@"insert into qcm_confirm_shoes_deposit_d (union_id,art_no,STOCK_CODE,ref_standard,opra_by,createby,createdate,createtime) 
                        values('{qid}','{art_no}','{STOCK_CODE}','{ref_standard}','{opra_by}','{user}','{date}','{time}')";
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
        /// 查询-确认鞋-存放管理-操作记录
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
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString() : "";//存放管理id
                string ref_standard = jarr.ContainsKey("ref_standard") ? jarr["ref_standard"].ToString() : "";//状态

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(ref_standard))
                {
                    where = $@" and d.ref_standard=@ref_standard";
                }
                string sql = string.Empty;

                sql = $@"	select 
                                    d.id as did,
	                                s.enum_value as ref_standard_name,
                                    h.staff_name as operator,
                                    d.ref_standard
	                                from 
	                                qcm_confirm_shoes_deposit_d d
	                                LEFT JOIN SYS001M s on d.ref_standard=s.enum_code and s.enum_type='enum_ref_standard'
                                    LEFT JOIN hr001m h on d.opra_by=h.staff_no
                                    where d.union_id='{qid}' {where}
                                    order by d.id desc";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("ref_standard", $@"{ref_standard}");

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                foreach (DataRow item in dt.Rows)
                {
                    if (item["ref_standard"].ToString()=="2"|| item["ref_standard"].ToString() == "4")
                    {
                        item["operator"] = DB.GetString($@"select 
                                                            h.staff_name as operator
	                                                        from 
	                                                        qcm_confirm_shoes_deposit_d d
                                                            LEFT JOIN hr001m h on d.opra_by=h.staff_no where d.id='{item["did"]}'");
                    }
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
        /// 查询-确认鞋-存放管理-主页-导出
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
                string shoe_name = jarr.ContainsKey("shoe_name") ? jarr["shoe_name"].ToString() : "";//鞋型名称
                string prod_name = jarr.ContainsKey("prod_name") ? jarr["prod_name"].ToString() : "";//art名称
                string stock_name = jarr.ContainsKey("stock_name") ? jarr["stock_name"].ToString() : "";//存放位置(库位名称)
                string wh_dateS = jarr.ContainsKey("wh_dateS") ? jarr["wh_dateS"].ToString() : "";//入库日期开始
                string wh_dateE = jarr.ContainsKey("wh_dateE") ? jarr["wh_dateE"].ToString() : "";//入库日期结束
                string output_dateS = jarr.ContainsKey("output_dateS") ? jarr["output_dateS"].ToString() : "";//量产日期开始
                string output_dateE = jarr.ContainsKey("output_dateE") ? jarr["output_dateE"].ToString() : "";//量产日期结束
                List<string> ref_standard = jarr.ContainsKey("ref_standard") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["ref_standard"].ToString()) : null;//状态

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(shoe_name))
                {
                    where += $@" and d.name_t like @shoe_name";
                }
                if (!string.IsNullOrWhiteSpace(prod_name))
                {
                    where += $@" and r.prod_no like @prod_name";
                }
                if (!string.IsNullOrWhiteSpace(stock_name))
                {
                    where += $@" and a.stock_name like @stock_name";
                }
                if (!string.IsNullOrWhiteSpace(wh_dateS) && !string.IsNullOrWhiteSpace(wh_dateE))
                {
                    where += $@" and q.wh_date>=@wh_dateS and q.wh_date<= @wh_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_dateS))
                    {
                        where += $@" and q.wh_date >= @wh_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_dateE))
                    {
                        where += $@" and q.wh_date <= @wh_dateE' ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(output_dateS) && !string.IsNullOrWhiteSpace(output_dateE))
                {
                    where += $@" and q.output_date>=@output_dateS and q.output_date<= @output_dateE";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(output_dateS))
                    {
                        where += $@" and q.output_date >= @output_dateS ";
                    }
                    if (!string.IsNullOrWhiteSpace(output_dateE))
                    {
                        where += $@" and q.output_date <= @output_dateE' ";
                    }
                }
                if (ref_standard.Count > 0)
                {
                    where += $@" and q.ref_standard in ({string.Join(",", ref_standard)})";
                }


                string sql = $@"	select 
	                                d.name_t as shoe_name,
	                                r.prod_no,
	                                a.stock_name,
	                                s.enum_value as ref_standard_name,
	                                q.wh_date,
	                                q.output_date,
	                                q.reconfirmation_time,
	                                q.expected_maturity_date
	                                from 
	                                qcm_confirm_shoes_deposit q
	                                LEFT JOIN bdm_rd_prod r on q.art_no=r.prod_no
	                                LEFT JOIN BDM_RD_STYLE d on r.SHOE_NO=d.SHOE_NO
	                                LEFT JOIN qcm_confirm_shoes_arc a on q.stock_code=a.stock_code
	                                LEFT JOIN SYS001M s on q.ref_standard=s.enum_code and s.enum_type='enum_ref_standard'
                                    LEFT JOIN qcm_confirm_shoes_arc c on q.STOCK_CODE=c.STOCK_CODE
                                    where q.ref_standard!='3' {where}";
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                paramTestDic.Add("shoe_name", $@"%{shoe_name}%");
                paramTestDic.Add("prod_name", $@"%{prod_name}%");
                paramTestDic.Add("stock_name", $@"%{stock_name}%");
                paramTestDic.Add("wh_dateS", $@"{wh_dateS}");
                paramTestDic.Add("wh_dateE", $@"{wh_dateE}");
                paramTestDic.Add("output_dateS", $@"{output_dateS}");
                paramTestDic.Add("output_dateE", $@"{output_dateE}");

                DataTable dt = DB.GetDataTable(sql,paramTestDic);
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

    }
}
