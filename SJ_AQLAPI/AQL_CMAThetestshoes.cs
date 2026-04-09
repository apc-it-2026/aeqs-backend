using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_CMAThetestshoes
    {
        private static void NullKey(Dictionary<string, object> dic)
        {
            string[] keys = dic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (dic[keys[i]] == null)
                {
                    dic[keys[i]] = "";
                }
            }
        }
        /// <summary>
        /// 主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Main(object OBJ)
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
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
                string putin_date1 = jarr.ContainsKey("putin_date1") ? jarr["putin_date1"].ToString() : "";
                string end_date2 = jarr.ContainsKey("end_date2") ? jarr["end_date2"].ToString() : "";
                string status = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";
                string name_t = jarr.ContainsKey("name_t") ? jarr["name_t"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    where += $@" and a.external_test_date between '{putin_date}' and '{end_date}'";
                }
                if (!string.IsNullOrWhiteSpace(putin_date1) && !string.IsNullOrWhiteSpace(end_date2))
                {
                    where += $@" and a.import_date between '{putin_date1}' and '{end_date2}'";
                }
                if (!string.IsNullOrWhiteSpace(art_no))
                {
                    where += $@" and a.art_no like '%{art_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(name_t))
                {
                    where += $@" and l.name_t like '%{name_t}%'";
                }
                if (!string.IsNullOrWhiteSpace(status))
                {
                    if (status == "0")
                        where += $@" and a.external_test_res is not null ";
                    else
                        where += $@" and a.external_test_res is null ";
                }


                /*
                string sql = $@"
WITH aa as(
SELECT ART_NO,MAX(ID) id FROM aql_cma_test_shoes_m GROUP BY ART_NO
)
SELECT
a.id as ID,
l.name_t,--鞋型
a.art_no as prod_no,
a.internal_test_date,
CASE
	when a.internal_test_res='0' then 'PASS'
	when a.internal_test_res='1' then 'FAIL'
end as internal_test_res,
a.external_test_date,
CASE
	WHEN a.external_test_res='0' then 'PASS'
	WHEN a.external_test_res='1' then 'FAIL'
END as external_test_res,
CASE
	WHEN a.RE_TEST_RES = '0' THEN 'PASS'
	WHEN a.RE_TEST_RES = '1' THEN 'FAIL'
end AS RE_TEST_RES,
a.re_delivery_date,
a.import_date
FROM
	aql_cma_test_shoes_m a 
LEFT JOIN  bdm_rd_prod r ON a.art_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO 
JOIN aa n ON a.id = n.id
where 1=1 {where}  order by a.id desc";
                */


                string sql = $@"
WITH aa as(
SELECT ART_NO,MAX(ID) id FROM aql_cma_test_shoes_m GROUP BY ART_NO
)
SELECT
a.id as ID,
l.name_t,--鞋型
a.art_no as prod_no,
'' as internal_test_date,
'' as internal_test_res,
a.external_test_date,
CASE
	WHEN a.external_test_res='0' then 'PASS'
	WHEN a.external_test_res='1' then 'FAIL'
END as external_test_res,
CASE
	WHEN a.RE_TEST_RES = '0' THEN 'PASS'
	WHEN a.RE_TEST_RES = '1' THEN 'FAIL'
end AS RE_TEST_RES,
a.re_delivery_date,
a.import_date
FROM
	aql_cma_test_shoes_m a 
LEFT JOIN  bdm_rd_prod r ON a.art_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l on r.SHOE_NO=l.SHOE_NO 
JOIN aa n ON a.id = n.id
where 1=1 {where}  order by a.id desc";


                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));

                //art所有测试清单中最新的结果和日期
                foreach (DataRow item in dt.Rows)
                {
                    DataTable dtt = DB.GetDataTable($@"
SELECT 
TASK_NO,ART_NO,
completion_date,
RETEST_TASK_NO,
test_result
 from QCM_EX_TASK_LIST_M where ART_NO = '{item["prod_no"]}' ORDER BY  concat(createdate,createtime)  desc");
                    dtt = DB.GetDataTable($@"
SELECT
	m.ART_NO,
	m.completion_date,
	m.test_result
FROM
	QCM_EX_TASK_LIST_D d
INNER JOIN QCM_EX_TASK_LIST_M m ON m.TASK_NO=d.TASK_NO	
WHERE
	(d.INSPECTION_CODE LIKE 'CMA%' 
	OR d.INSPECTION_CODE LIKE 'cma%') AND m.ART_NO = '{item["prod_no"]}' 
ORDER BY  TO_DATE( m.CREATEDATE||' '||m.CREATETIME, 'yyyy-mm-dd HH24:mi:ss') desc");
                    if (dtt.Rows.Count > 0)
                    {
                        item["internal_test_date"] = dtt.Rows[0]["completion_date"].ToString();
                        item["internal_test_res"] = dtt.Rows[0]["test_result"].ToString();
                    }

                }


                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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
        /// 主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_MainHistory(object OBJ)
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
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string sql = $@"
SELECT
a.art_no,
a.id,
a.internal_test_date,
case
    when a.internal_test_res='0' then 'PASS'
    when a.internal_test_res='1' then 'FAIL'
end as internal_test_res,
a.external_test_date,
case
    when a.external_test_res='0' then 'PASS'
    when a.external_test_res='1' then 'FAIL'
end as external_test_res,
CASE
	WHEN a.RE_TEST_RES = '0' THEN 'PASS'
	WHEN a.RE_TEST_RES = '1' THEN 'FAIL'
end AS RE_TEST_RES,
a.re_delivery_date,
a.import_date
FROM
    aql_cma_test_shoes_d a
where  a.art_no='{art_no}' order by a.ID desc";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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
        /// 保存数据添加的art数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_artdata(object OBJ)
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
                List<string> art_list = jarr.ContainsKey("list") ? JsonConvert.DeserializeObject<List<string>>(jarr["list"].ToString()) : null;//表头id
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                art_list = art_list.Where(x=>x.Count()>0).ToList();
                DateTime datetime = DateTime.Now;
                string date = datetime.ToString("yyyy-MM-dd");
                string time = datetime.ToString("HH:mm:ss");
                string sql = $@"SELECT
DISTINCT
	PROD_NO --art
FROM
	BDM_RD_PROD";
                DataTable dt = DB.GetDataTable(sql);

                sql = "select*from aql_cma_test_shoes_m";
                DataTable dt_m = DB.GetDataTable(sql);
                DataRow [] drstr = null;
                DataRow [] dr_m=null;
                string messg = string.Empty;//报错提示用的
                string messg2 = string.Empty;//报错提示用的
                sql = string.Empty;
                foreach (string item in art_list)
                {
                    drstr = dt.Select($@"PROD_NO='{item}'");
                    //说明基础维护不存在该art
                    if (drstr.Length < 1)
                    {
                        messg += item + "、";
                        continue;
                    }
                    foreach (DataRow dr in drstr)
                    {
                        dr_m=dt_m.Select($@"ART_NO='{dr["PROD_NO"]}'");//查询表头是否有art值
                        if (dr_m.Length > 0)
                        {
                            foreach (DataRow dr2 in dr_m)
                            {
                                //查看导入时间是否为期满三个月，如果不满就添加，否则不给添加
                                if(Convert.ToDateTime(dr2["IMPORT_DATE"]).AddMonths(3)>datetime.AddMonths(3))
                                {
                                    sql += $@"insert into aql_cma_test_shoes_m(art_no,import_date,createby,createdate,createtime) values('{dr["ART_NO"]}','{date+" "+time}','{userCode}','{date}','{time}');";
                                }
                                else
                                {
                                    messg2 += dr["PROD_NO"] + "、";
                                    
                                }
                            }
                        }
                        else
                        {
                            sql += $@"insert into aql_cma_test_shoes_m(art_no,import_date,createby,createdate,createtime) values('{item}','{date + " " + time}','{userCode}','{date}','{time}');";
                        }
                    }


                }
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    DB.ExecuteNonQueryOffline($@"begin {sql} end;");
                }
               
                if (!string.IsNullOrWhiteSpace(messg))
                {
                    ret.ErrMsg = "ART:" + messg + "does not exist, cannot import";
                }
                else if (!string.IsNullOrWhiteSpace(messg2))
                {
                    ret.ErrMsg += "\t\t"+$@"ART:{messg2}There is data within three months and cannot be imported";
                }
                if (!string.IsNullOrWhiteSpace(ret.ErrMsg))
                {
                    ret.IsSuccess = false;
                    return ret;
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "Added successfully";
                DB.Commit();

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
        /// 根据导入art拿到鞋型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_artlist(object OBJ)
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
                List<string> art_list = jarr.ContainsKey("list") ? JsonConvert.DeserializeObject<List<string>>(jarr["list"].ToString()) : null;//初始的数据源

                for (int i = 0; i < art_list.Count; i++)
                {
                    int count = DB.GetInt32($@"select count(1) from BDM_RD_PROD where PROD_NO='{art_list[i]}'");
                    if (count<=0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = $@"{art_list[i]}Information does not exist!";
                        return ret;
                    }
                }

                string sql = $@"SELECT
	A.PROD_NO,
	B.NAME_T
FROM
	BDM_RD_PROD A
LEFT JOIN BDM_RD_STYLE B ON A.SHOE_NO = B.SHOE_NO WHERE A.PROD_NO in ('{string.Join("','", art_list)}')";

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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
        /// 保存之后操作的数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_data(object OBJ)
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
                DataTable dt_data = jarr.ContainsKey("dt_data") ? JsonConvert.DeserializeObject<DataTable>(jarr["dt_data"].ToString()) : null;//表头id
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime datetime = DateTime.Now;
                string date = datetime.ToString("yyyy-MM-dd");
                string time = datetime.ToString("HH:mm:ss");
                string sql = string.Empty;
                sql = $@"
WITH aa as(
SELECT ART_NO,MAX(ID) id FROM aql_cma_test_shoes_m GROUP BY ART_NO
)
SELECT
	aql_cma_test_shoes_m.*
FROM
	aql_cma_test_shoes_m
JOIN  aa on aa.id = aql_cma_test_shoes_m.ID";
                DataTable dt_m = DB.GetDataTable(sql);
                DataRow [] dr_m = null;
                if (dt_data.Rows.Count > 0)
                {
                    string messg = string.Empty;//报错提示用的
                    
                    sql = string.Empty;
                    bool flag; 
                    foreach (DataRow item in dt_data.Rows)
                    {
                        string internal_test_res = String.Empty;
                        string external_test_res = String.Empty;
                        string RE_TEST_RES = String.Empty;
                        if (item["prod_no"].ToString() == "AC7627")
                        {

                        }
                        flag = true;
                        if (item["internal_test_res"].ToString() == "PASS")internal_test_res = "0";
                        else if(item["internal_test_res"].ToString() == "FAIL") internal_test_res = "1";

                        if (item["external_test_res"].ToString() == "PASS") external_test_res = "0";
                        else if(item["external_test_res"].ToString() == "FAIL") external_test_res = "1";

                        if (item["RE_TEST_RES"].ToString() == "PASS") RE_TEST_RES = "0";
                        else if (item["RE_TEST_RES"].ToString() == "FAIL") RE_TEST_RES = "1";

                        dr_m =dt_m.Select($@"ART_NO='{item["prod_no"]}'");
                        if (dr_m.Length > 0)
                        {
                            //查看导入时间是否为期满三个月，如果不满就添加，否则不给添加
                            //                            if (Convert.ToDateTime(dr_m[0]["import_date"]).AddMonths(3) > datetime.AddMonths(3))
                            //                            {
                            //                                /*sql += $@"insert into aql_cma_test_shoes_m(art_no,internal_test_date,internal_test_res,external_test_date,external_test_res,re_delivery_date,import_date,createby,createdate,createtime
                            //) values('{item["prod_no"]}','{item["internal_test_date"]}','{internal_test_res}','{item["external_test_date"]}','{external_test_res}','{item["re_delivery_date"]}','{date}','{userCode}','{date}','{time}');";*/
                            //                               sql += $@"update  aql_cma_test_shoes_m set internal_test_date='{item["internal_test_date"]}',internal_test_res='{internal_test_res}',external_test_date='{item["external_test_date"]}',external_test_res='{external_test_res}',re_delivery_date='{item["re_delivery_date"]}',import_date='{date}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where art_no='{item["prod_no"]}';";
                            //
                            //                            }
                            //                            else
                            //                            {
                            //                                messg += item["prod_no"] + "、";
                            //                                flag = false;
                            //                            }

                            sql += $@"update  aql_cma_test_shoes_m set RE_TEST_RES = '{RE_TEST_RES}',internal_test_date='{item["internal_test_date"]}',internal_test_res='{internal_test_res}',external_test_date='{item["external_test_date"]}',external_test_res='{external_test_res}',re_delivery_date='{item["re_delivery_date"]}',import_date='{item["import_date"].ToString()}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where art_no='{item["prod_no"]}' and ID = '{item["ID"]}';";

                        }
                        else
                        {
                            sql += $@"insert into aql_cma_test_shoes_m(art_no,RE_TEST_RES,internal_test_date,internal_test_res,external_test_date,external_test_res,re_delivery_date,import_date,createby,createdate,createtime
) values('{item["prod_no"]}','{RE_TEST_RES}','{item["internal_test_date"]}','{internal_test_res}','{item["external_test_date"]}','{external_test_res}','{item["re_delivery_date"]}','{item["import_date"].ToString()}','{userCode}','{date}','{time}');";
                        }

                        if (flag)
                        {
                            sql += $@"insert into aql_cma_test_shoes_d(art_no,RE_TEST_RES,internal_test_date,internal_test_res,external_test_date,external_test_res,re_delivery_date,import_date,createby,createdate,createtime
) values('{item["prod_no"]}','{RE_TEST_RES}','{item["internal_test_date"]}','{internal_test_res}','{item["external_test_date"]}','{external_test_res}','{item["re_delivery_date"]}','{item["import_date"].ToString()}','{userCode}','{date}','{time}');";
                        }

                    }
                  
                    if (!string.IsNullOrWhiteSpace(messg))
                    {
                        //ret.ErrMsg +=$@"ART:{messg}三个月内已有数据，无法导入";
                        ret.ErrMsg +=$@"ART:{messg}There is data within three months and cannot be imported";
                    }
                    if (!string.IsNullOrWhiteSpace(ret.ErrMsg))
                    {
                        ret.IsSuccess = false;
                        return ret;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            DB.ExecuteNonQuery($@"begin {sql} end;");
                        }
                    }
                }   
                ret.IsSuccess = true;
                //ret.ErrMsg = "添加成功";
                ret.ErrMsg = "Added successfully";
                DB.Commit();

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
        /// 添加确认鞋数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Add_data(object OBJ)
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
                List<string> art_list = jarr.ContainsKey("list") ? JsonConvert.DeserializeObject<List<string>>(jarr["list"].ToString()) : null;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime datetime = DateTime.Now;
                string date = datetime.ToString("yyyy-MM-dd");
                string time = datetime.ToString("HH:mm:ss");
                string sql = string.Empty;
                DataTable dtt = DB.GetDataTable($@"
WITH aa as(
SELECT ART_NO,MAX(ID) id FROM aql_cma_test_shoes_m GROUP BY ART_NO
)
SELECT
	aql_cma_test_shoes_m.*
FROM
	aql_cma_test_shoes_m
JOIN  aa on aa.id = aql_cma_test_shoes_m.ID");


                foreach (var item in art_list)
                {
                    if(DB.GetInt16($@"select count(1) from BDM_RD_PROD where PROD_NO='{item}'") == 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = $@"【{item}】 information does not exist!";
                        //ret.ErrMsg = $@"【{item}】 信息不存在!";
                        return ret;
                    }

                }
                DataTable dt = DB.GetDataTable($@"
SELECT
	A.PROD_NO,
	B.NAME_T
FROM
	BDM_RD_PROD A
LEFT JOIN BDM_RD_STYLE B ON A.SHOE_NO = B.SHOE_NO WHERE A.PROD_NO in ('{string.Join("','", art_list)}')");



                foreach (DataRow item in dt.Rows)
                {
                    //如果表头存在art
                    var dr_m = dtt.Select($@"ART_NO='{item["prod_no"]}'");
                    if (dr_m.Length > 0)
                    {
                        //将表头已存在的更新到表身

                        /*表头更新*/
                        sql += $@"update aql_cma_test_shoes_m SET internal_test_date= '',internal_test_res='',external_test_date='',external_test_res='',re_delivery_date= '',RE_TEST_RES='',import_date ='{date}',createby = '{userCode}',createdate = '{date}',createtime = '{time}' WHERE ART_NO = '{dr_m[0]["ART_NO"]}';";

                        /*更新至历史记录*/
                        sql += $@"insert into aql_cma_test_shoes_d(art_no,internal_test_date,internal_test_res,external_test_date,external_test_res,re_delivery_date,RE_TEST_RES,import_date,createby,createdate,createtime
) values('{dr_m[0]["ART_NO"]}','{dr_m[0]["internal_test_date"]}','{dr_m[0]["internal_test_res"]}','{dr_m[0]["external_test_date"]}','{dr_m[0]["external_test_res"]}','{dr_m[0]["re_delivery_date"]}','{dr_m[0]["RE_TEST_RES"]}','{dr_m[0]["import_date"]}','{userCode}','{date}','{time}');";
                    }
                    else
                    {
                        //新增至表头
                        sql += $@"insert into aql_cma_test_shoes_m(art_no,import_date,createby,createdate,createtime
) values('{item["prod_no"]}','{date}','{userCode}','{date}','{time}');";
                    }

                   
                }

                if (!string.IsNullOrWhiteSpace(sql))
                {
                    DB.ExecuteNonQuery($@"begin {sql} end;");
                }

                ret.IsSuccess = true;
                ret.ErrMsg = "Added_Successfully";
                DB.Commit();

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
        /// 删除明细数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Delete(object OBJ)

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

                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//表头id
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art

                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime datetime = DateTime.Now;
                string date = datetime.ToString("yyyy-MM-dd");
                string time = datetime.ToString("HH:mm:ss");

                string sql = $@"delete from aql_cma_test_shoes_d where id='{id}'";
                DB.ExecuteNonQuery(sql);
                sql = $@"select art_no,internal_test_date,internal_test_res,external_test_date,external_test_res,re_delivery_date,import_date
                         from aql_cma_test_shoes_d  where id=(select max(id) from aql_cma_test_shoes_d where id!='{id}' and  art_no='{prod_no}')";
                DataTable dt = DB.GetDataTablebyline(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        sql = $@"update  aql_cma_test_shoes_m set internal_test_date='{item["internal_test_date"]}',internal_test_res='{item["internal_test_res"]}',external_test_date='{item["external_test_date"]}',external_test_res='{item["external_test_res"]}',re_delivery_date='{item["re_delivery_date"]}',import_date='{item["import_date"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where art_no='{item["art_no"]}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                else
                {
                    DB.ExecuteNonQuery($@"delete from aql_cma_test_shoes_m where ART_NO='{prod_no}'");
                }
               

                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
                //ret.ErrMsg = "删除成功";
                DB.Commit();

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
