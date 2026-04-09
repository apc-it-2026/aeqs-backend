using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    public class BatchProduction
    {
        /// <summary>
        /// 获取数据绑定列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBatchProductionList(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string BATCH_DATE = jarr.ContainsKey("BATCH_DATE") ? jarr["BATCH_DATE"].ToString().Trim() : "";
                string SHOE_NAME = jarr.ContainsKey("SHOE_NAME") ? jarr["SHOE_NAME"].ToString().Trim() : "";
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString().Trim() : "";//art
                string DEVELOP_QUARTER = jarr.ContainsKey("DEVELOP_QUARTER") ? jarr["DEVELOP_QUARTER"].ToString().Trim() : "";//art
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString().Trim() : "";
                string BATCH_MONTH = jarr.ContainsKey("BATCH_MONTH") ? jarr["BATCH_MONTH"].ToString().Trim() : "";
                string PRODUCTION_MONTH = jarr.ContainsKey("PRODUCTION_MONTH") ? jarr["PRODUCTION_MONTH"].ToString().Trim() : "";

                string is_pda = jarr.ContainsKey("is_pda") ? jarr["is_pda"].ToString().Trim() : "1";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = "";
                if (!string.IsNullOrEmpty(BATCH_DATE))
                {
                    where += $" AND BATCH_DATE='{BATCH_DATE}'";
                }
                if (!string.IsNullOrEmpty(SHOE_NAME))
                {
                    where += $" AND SHOE_NAME LIKE '%{SHOE_NAME}%'";
                }
                if (!string.IsNullOrEmpty(ART))
                {
                    where += $" AND ART LIKE '%{ART}%'";
                }
                if (!string.IsNullOrEmpty(TYPE))
                {
                    where += $" AND TYPE='{TYPE}'";
                }
                if (!string.IsNullOrEmpty(BATCH_MONTH))
                {
                    string stime = BATCH_MONTH.Split('|')[0];
                    string etime = BATCH_MONTH.Split('|')[1];
                    where += $" AND BATCH_DATE  between '{stime}' and '{etime}'";
                }
                if (!string.IsNullOrEmpty(PRODUCTION_MONTH))
                {
                    string stime = PRODUCTION_MONTH.Split('|')[0];
                    string etime = PRODUCTION_MONTH.Split('|')[1];
                    where += $" AND PRODUCTION_DATE  between '{stime}' and '{etime}'";
                }
                string sql = $"SELECT * FROM QCM_BATCH_PRODUCTION_M WHERE 1=1 {where}";
                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.IsSuccess = true;
                if(is_pda=="1")
                {
                    ret.RetData1 = dic;
                }
                else
                {
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 添加
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Edit(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                var ID= jarr.ContainsKey("ID")?jarr["ID"].ToString():"";
                var BATCH_CODE = jarr.ContainsKey("BATCH_CODE") ? jarr["BATCH_CODE"].ToString() : "";
                var DEVELOP_QUARTER = jarr.ContainsKey("DEVELOP_QUARTER") ? jarr["DEVELOP_QUARTER"].ToString() : "";
                var TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";
                var ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";
                var BATCH_DATE = jarr.ContainsKey("BATCH_DATE") ? jarr["BATCH_DATE"].ToString() : "";
                var PRODUCTION_DATE = jarr.ContainsKey("PRODUCTION_DATE") ? jarr["PRODUCTION_DATE"].ToString() : "";
                var SHOE_NAME = jarr.ContainsKey("SHOE_NAME") ? jarr["SHOE_NAME"].ToString() : "";
                var BIG_MOLD_NO = jarr.ContainsKey("BIG_MOLD_NO") ? jarr["BIG_MOLD_NO"].ToString() : "";
                var SIZE_DOUBLE = jarr.ContainsKey("SIZE_DOUBLE") ? jarr["SIZE_DOUBLE"].ToString() : "";
                var COLOR = jarr.ContainsKey("COLOR") ? jarr["COLOR"].ToString() : "";
                var DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";
                var PROCEDURE = jarr.ContainsKey("PROCEDURE") ? jarr["PROCEDURE"].ToString() : "";
                var LEADER_AUTOGRAPH = jarr.ContainsKey("LEADER_AUTOGRAPH") ? jarr["LEADER_AUTOGRAPH"].ToString() : "";
                var SHOE_LAST = jarr.ContainsKey("SHOE_LAST") ? jarr["SHOE_LAST"].ToString() : "";

                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                if (!string.IsNullOrEmpty(ID))
                {
                    DB.ExecuteNonQueryOffline($@"update QCM_BATCH_PRODUCTION_M set
                                                            DEVELOP_QUARTER='{DEVELOP_QUARTER}',
                                                            TYPE='{TYPE}',
                                                            ART='{ART}',
                                                            BATCH_DATE='{BATCH_DATE}',
                                                            PRODUCTION_DATE='{PRODUCTION_DATE}',
                                                            SHOE_NAME='{SHOE_NAME}',
                                                            BIG_MOLD_NO='{BIG_MOLD_NO}',
                                                            SIZE_DOUBLE='{SIZE_DOUBLE}',
                                                            COLOR='{COLOR}',
                                                            DEPARTMENT='{DEPARTMENT}',
                                                            PROCEDURE='{PROCEDURE}',
                                                            LEADER_AUTOGRAPH='{LEADER_AUTOGRAPH}',
                                                            SHOE_LAST='{SHOE_LAST}',
                                                            MODIFYBY='{usercode}',
                                                            MODIFYDATE='{date}',
                                                            MODIFYTIME='{time}'
                                                        where ID={ID}");
                    ret.IsSuccess = true;
                }
                else
                {
                    if(DB.GetInt32($"select count(1) from QCM_BATCH_PRODUCTION_M where BATCH_CODE='{BATCH_CODE}'")>0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "量试编号已存在";
                    }
                    else
                    {
                        DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_BATCH_PRODUCTION_M
                                                        (
                                                            BATCH_CODE,
                                                            DEVELOP_QUARTER,
                                                            TYPE,
                                                            ART,
                                                            BATCH_DATE,
                                                            PRODUCTION_DATE,
                                                            SHOE_NAME,
                                                            BIG_MOLD_NO,
                                                            SIZE_DOUBLE,
                                                            COLOR,
                                                            DEPARTMENT,
                                                            PROCEDURE,
                                                            LEADER_AUTOGRAPH,
                                                            SHOE_LAST,
                                                            CREATEBY,
                                                            CREATEDATE,
                                                            CREATETIME,
                                                            STATUS
                                                         )
                                                         VALUES
                                                         (
                                                            '{BATCH_CODE}',
                                                            '{DEVELOP_QUARTER}',
                                                            '{TYPE}',
                                                            '{ART}',
                                                            '{BATCH_DATE}',
                                                            '{PRODUCTION_DATE}',
                                                            '{SHOE_NAME}',
                                                            '{BIG_MOLD_NO}',
                                                            '{SIZE_DOUBLE}',
                                                            '{COLOR}',
                                                            '{DEPARTMENT}',
                                                            '{PROCEDURE}',
                                                            '{LEADER_AUTOGRAPH}',
                                                            '{SHOE_LAST}',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}',
                                                            '0'  
                                                         )");
                        ret.IsSuccess = true;
                    }
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
        /// 删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Delete(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string BATCH_CODE = jarr.ContainsKey("BATCH_CODE") ? jarr["BATCH_CODE"].ToString().Trim() : "";//art

                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                DB.Open();
                DB.BeginTransaction();

                DB.ExecuteNonQueryOffline($"DELETE FROM QCM_BATCH_PRODUCTION_M WHERE BATCH_CODE='{BATCH_CODE}'");
                DB.ExecuteNonQueryOffline($"DELETE FROM QCM_BATCH_PRODUCTION_D WHERE BATCH_CODE='{BATCH_CODE}'");
                DB.ExecuteNonQueryOffline($"DELETE FROM QCM_BATCH_PRODUCTION_IMAGEURL WHERE BATCH_CODE='{BATCH_CODE}'");

                DB.Commit();
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {

                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 添加问题
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddProblem(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string BATCH_CODE = jarr.ContainsKey("BATCH_CODE") ? jarr["BATCH_CODE"].ToString().Trim() : "";
                string PROBLEM = jarr.ContainsKey("PROBLEM") ? jarr["PROBLEM"].ToString().Trim() : "";
                List<Dictionary<string, object>> PROBLEM_LIST = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(PROBLEM);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:MM:ss");
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                foreach (var item in PROBLEM_LIST)
                {
                    string PROBLEM_IMG_GUID = Guid.NewGuid().ToString("N");
                    string SOLUTION_IMG_GUID = Guid.NewGuid().ToString("N");
                    string ID = item.ContainsKey("ID") ? item["ID"].ToString() : "";
                    if (string.IsNullOrEmpty(ID))
                    {
                        DB.ExecuteNonQuery($@"insert into qcm_batch_production_d(
                                                            BATCH_CODE,
                                                            CHECK_ITEM,
                                                            CHECK_STANDARD,
                                                            CHECK_NUM,
                                                            GOOD_NUM,
                                                            BAD_NUM,
                                                            CHECK_RESULT,
                                                            PROBLEM,
                                                            PROBLEM_IMG_GUID,
                                                            SOLUTION_IMG_GUID,
                                                            CREATEBY,
                                                            CREATEDATE,
                                                            CREATETIME
                                                            )
                                                            values(
                                                            '{BATCH_CODE}',
                                                            '{item["CHECK_ITEM"]}',
                                                            '{item["CHECK_STANDARD"]}',
                                                            '{item["CHECK_NUM"]}',
                                                            '{item["GOOD_NUM"]}',
                                                            '{item["BAD_NUM"]}',
                                                            '{item["CHECK_RESULT"]}',
                                                            '{item["PROBLEM"]}',
                                                            '{PROBLEM_IMG_GUID}',
                                                            '{SOLUTION_IMG_GUID}',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}'
                                                            )");
                        List<Dictionary<string, object>> PROBLEM_IMG_LIST = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["PROBLEM_IMG"].ToString());
                        foreach (var img in PROBLEM_IMG_LIST)
                        {
                            DB.ExecuteNonQuery($@"insert into QCM_BATCH_PRODUCTION_IMAGEURL(
                                                            BATCH_CODE,
                                                            GUID,
                                                            IMG_NAME,
                                                            IMG_URL,
                                                            TYPE,
                                                            CREATEBY,
                                                            CREATEDATE,
                                                            CREATETIME
                                                            )
                                                            values(
                                                            '{BATCH_CODE}',
                                                            '{PROBLEM_IMG_GUID}',
                                                            '{img["IMG_NAME"]}',
                                                            '{img["IMG_URL"]}',
                                                            '0',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}'
                                                            )");
                        }
                    }
                    else
                    {
                        PROBLEM_IMG_GUID = DB.GetString($"select PROBLEM_IMG_GUID from qcm_batch_production_d where ID={ID}");
                        DB.ExecuteNonQuery($@"update qcm_batch_production_d set
                                                            CHECK_ITEM='{item["CHECK_ITEM"]}',
                                                            CHECK_STANDARD='{item["CHECK_STANDARD"]}',
                                                            CHECK_NUM='{item["CHECK_NUM"]}',
                                                            GOOD_NUM='{item["GOOD_NUM"]}',
                                                            BAD_NUM='{item["BAD_NUM"]}',
                                                            CHECK_RESULT='{item["CHECK_RESULT"]}',
                                                            PROBLEM='{item["PROBLEM"]}'
                                                            WHERE ID={ID}");
                        DB.ExecuteNonQuery($"DELETE FROM QCM_BATCH_PRODUCTION_IMAGEURL WHERE BATCH_CODE='{BATCH_CODE}' AND GUID='{PROBLEM_IMG_GUID}' AND TYPE='0'");
                        List<Dictionary<string, object>> PROBLEM_IMG_LIST = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["PROBLEM_IMG"].ToString());
                        foreach (var img in PROBLEM_IMG_LIST)
                        {
                            DB.ExecuteNonQuery($@"insert into QCM_BATCH_PRODUCTION_IMAGEURL(
                                                            BATCH_CODE,
                                                            GUID,
                                                            IMG_NAME,
                                                            IMG_URL,
                                                            TYPE,
                                                            CREATEBY,
                                                            MODIFYDATE,
                                                            MODIFYTIME
                                                            )
                                                            values(
                                                            '{BATCH_CODE}',
                                                            '{PROBLEM_IMG_GUID}',
                                                            '{img["IMG_NAME"]}',
                                                            '{img["IMG_URL"]}',
                                                            '0',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}'
                                                            )");
                        }

                    }
                }
                DB.ExecuteNonQuery($@"UPDATE qcm_batch_production_m  a set a.status=
                                            (
                                            select case when count(1) > 0 then 0 else 1 end from qcm_batch_production_d where nvl(solution, '-1') = '-1' and batch_code = '{BATCH_CODE}'
                                            )
                                            where batch_code = '{BATCH_CODE}'");
                DB.Commit();
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProblemDetail(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string BATCH_CODE = jarr.ContainsKey("BATCH_CODE") ? jarr["BATCH_CODE"].ToString().Trim() : "";
                string type = jarr.ContainsKey("type") ? jarr["type"].ToString().Trim() : "0";
                DataTable dt = DB.GetDataTable($@"select a.ID,a.BATCH_CODE,a.CHECK_ITEM,a.CHECK_STANDARD,a.CHECK_NUM,a.GOOD_NUM,a.BAD_NUM,a.CHECK_RESULT,a.PROBLEM,a.SOLUTION,a.SOLUTION_SURE,
                                                (select {CommonBASE.GetGroupConcatByOracleVersion(DB, "concat(concat(b.IMG_NAME,'|'),b.IMG_URL)", "b.ID")}  from QCM_BATCH_PRODUCTION_IMAGEURL b where a.BATCH_CODE=b.BATCH_CODE and a.PROBLEM_IMG_GUID=b.GUID) PROBLEM_IMG,
                                                (select {CommonBASE.GetGroupConcatByOracleVersion(DB, "concat(concat(c.IMG_NAME,'|'),c.IMG_URL)", "c.ID")}  from QCM_BATCH_PRODUCTION_IMAGEURL c where a.BATCH_CODE=c.BATCH_CODE and a.SOLUTION_IMG_GUID=c.GUID) SOLUTION_IMG
                                                from QCM_BATCH_PRODUCTION_D a 
                                                where a.BATCH_CODE='{BATCH_CODE}'");
                List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> drDic = new Dictionary<string, object>();
                    drDic["ID"] = dr["ID"].ToString();
                    drDic["BATCH_CODE"] = dr["BATCH_CODE"].ToString();
                    drDic["CHECK_ITEM"] = dr["CHECK_ITEM"].ToString();
                    drDic["CHECK_STANDARD"] = dr["CHECK_STANDARD"].ToString();
                    drDic["CHECK_NUM"] = dr["CHECK_NUM"].ToString();
                    drDic["GOOD_NUM"] = dr["GOOD_NUM"].ToString();
                    drDic["BAD_NUM"] = dr["BAD_NUM"].ToString();
                    drDic["CHECK_RESULT"] = dr["CHECK_RESULT"].ToString();
                    drDic["PROBLEM"] = dr["PROBLEM"].ToString();
                    drDic["SOLUTION"] = dr["SOLUTION"].ToString();
                    drDic["SOLUTION_SURE"] = dr["SOLUTION_SURE"].ToString();

                    List<Dictionary<string, object>> PROBLEM_IMG_DIC = new List<Dictionary<string, object>>();
                    if (!string.IsNullOrEmpty(dr["PROBLEM_IMG"].ToString()))
                    {
                        List<string> list = dr["PROBLEM_IMG"].ToString().Split(',').ToList();
                        foreach (var item in list)
                        {
                            Dictionary<string, object> ITEM_DIC = new Dictionary<string, object>();
                            ITEM_DIC["IMG_NAME"] = item.Split("|")[0];
                            ITEM_DIC["IMG_URL"] = item.Split("|")[1];
                            PROBLEM_IMG_DIC.Add(ITEM_DIC);
                        }
                    }
                    drDic["PROBLEM_IMG"] = PROBLEM_IMG_DIC;
                    List<Dictionary<string, object>> SOLUTION_IMG_DIC = new List<Dictionary<string, object>>();
                    if (!string.IsNullOrEmpty(dr["SOLUTION_IMG"].ToString()))
                    {
                        List<string> list = dr["SOLUTION_IMG"].ToString().Split(',').ToList();
                        foreach (var item in list)
                        {
                            Dictionary<string, object> ITEM_DIC = new Dictionary<string, object>();
                            ITEM_DIC["IMG_NAME"] = item.Split("|")[0];
                            ITEM_DIC["IMG_URL"] = item.Split("|")[1];
                            SOLUTION_IMG_DIC.Add(ITEM_DIC);
                        }
                    }
                    drDic["SOLUTION_IMG"] = SOLUTION_IMG_DIC;
                    result.Add(drDic);
                }

                if(type=="1")
                {
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                }
                else
                {
                    ret.IsSuccess = true;
                    ret.RetData1 = result;
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
        /// 解决问题提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveSolution(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SOLUTION_DATA = jarr.ContainsKey("SOLUTION_DATA") ? jarr["SOLUTION_DATA"].ToString().Trim() : "";
                List<Dictionary<string, object>> SOLUTION_LIST = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(SOLUTION_DATA);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:MM:ss");
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string BATCH_CODE = "";
                foreach (var item in SOLUTION_LIST)
                {
                    string ID = item.ContainsKey("ID") ? item["ID"].ToString() : "";
                    string SOLUTION = item.ContainsKey("SOLUTION") ? item["SOLUTION"].ToString() : "";
                    string SOLUTION_SURE = item.ContainsKey("SOLUTION_SURE") ? item["SOLUTION_SURE"].ToString() : "";

                    string SOLUTION_GUID = DB.GetString($"select SOLUTION_IMG_GUID from qcm_batch_production_d where ID={ID}");
                    BATCH_CODE = DB.GetString($"select BATCH_CODE from qcm_batch_production_d where ID={ID}");
                    DB.ExecuteNonQuery($@"update qcm_batch_production_d set
                                                            SOLUTION_SURE='{item["SOLUTION_SURE"]}',
                                                            SOLUTION='{item["SOLUTION"]}'
                                                            WHERE ID={ID}");
                    DB.ExecuteNonQuery($"DELETE FROM QCM_BATCH_PRODUCTION_IMAGEURL WHERE BATCH_CODE='{BATCH_CODE}' AND GUID='{SOLUTION_GUID}' AND TYPE='1'");
                    List<Dictionary<string, object>> SOLUTION_IMG_LIST = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(item["SOLUTION_IMG"].ToString());
                    foreach (var img in SOLUTION_IMG_LIST)
                    {
                        DB.ExecuteNonQuery($@"insert into QCM_BATCH_PRODUCTION_IMAGEURL(
                                                            BATCH_CODE,
                                                            GUID,
                                                            IMG_NAME,
                                                            IMG_URL,
                                                            TYPE,
                                                            CREATEBY,
                                                            MODIFYDATE,
                                                            MODIFYTIME
                                                            )
                                                            values(
                                                            '{BATCH_CODE}',
                                                            '{SOLUTION_GUID}',
                                                            '{img["IMG_NAME"]}',
                                                            '{img["IMG_URL"]}',
                                                            '1',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}'
                                                            )");
                    }
                }
                DB.ExecuteNonQuery($@"UPDATE qcm_batch_production_m  a set a.status=
                                            (
                                            select case when count(1) > 0 then 0 else 1 end from qcm_batch_production_d where nvl(solution, '-1') = '-1' and batch_code = '{BATCH_CODE}'
                                            )
                                            where batch_code = '{BATCH_CODE}'");
                DB.Commit();
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();
            }

            return ret;
        }
    }
}
