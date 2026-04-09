using Newtonsoft.Json;
using OrderPackaging.MaterialBudget;
using Special.Packaging.Information;
using Subsection.HeightWarpage;
using Subsection.Logo;
using Subsection.ShoelacesAndOutsole;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using static SJ_AQLAPI.common.Enum;

namespace SJ_AQLAPI
{
    public class AQL_Checkthedata1
    {
        //-------------------------------------核对资料------------------------------------//

        /// <summary>
        /// 修复核对资料数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject XiuFu_data(object OBJ)
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
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                var po_dt = DB.GetDataTablebyline($@"
select PO FROM aql_cma_task_list_m GROUP BY PO
");
                foreach (DataRow po_item in po_dt.Rows)
                {
                    string po = po_item["PO"].ToString();
                    //查询 aql_cma_task_list_m_cd
                    var cd_dt = DB.GetDataTablebyline($@"select * FROM aql_cma_task_list_m_cd WHERE PO='{po}'");

                    //查询 aql_cma_task_list_m_cd_r
                    var cd_r_dt = DB.GetDataTablebyline($@"select * FROM aql_cma_task_list_m_cd_r WHERE PO='{po}'");

                    //查询 aql_cma_task_list_m
                    var m_dt = DB.GetDataTablebyline($@"select TASK_NO FROM aql_cma_task_list_m WHERE PO='{po}'");

                    //删除旧数据
                    DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_cd where po='{po}'");
                    DB.ExecuteNonQuery($@"delete from aql_cma_task_list_m_cd_r where po='{po}'");
                    foreach (DataRow m_item in m_dt.Rows)
                    {
                        string task_no = m_item["TASK_NO"].ToString();

                        StringBuilder sb = new StringBuilder();
                        foreach (DataRow cd_dt_item in cd_dt.Rows)
                        {
                            sb.Append($@"insert into aql_cma_task_list_m_cd(TASK_NO,PO,CONCLUSION,AUTOGRAPH,CONCLUSION_TYPE,CREATEBY,CREATEDATE,CREATETIME,IS_AUTOGRAPH) values('{task_no}','{cd_dt_item["PO"]}','{cd_dt_item["CONCLUSION"]}','{cd_dt_item["AUTOGRAPH"]}','{cd_dt_item["CONCLUSION_TYPE"]}','{cd_dt_item["CREATEBY"]}','{cd_dt_item["CREATEDATE"]}','{cd_dt_item["CREATETIME"]}','{cd_dt_item["IS_AUTOGRAPH"]}');");
                        }
                        if (sb.Length > 0)
                        {
                            DB.ExecuteNonQuery($@"begin {sb} end;");
                        }

                        foreach (DataRow cd_r_dt_item in cd_r_dt.Rows)
                        {
                            DB.ExecuteNonQuery($@"insert into aql_cma_task_list_m_cd_r(TASK_NO,PO,REMARK_TYPE,REMARK,CREATEBY,CREATEDATE,CREATETIME) values('{task_no}','{cd_r_dt_item["PO"]}','{cd_r_dt_item["REMARK_TYPE"]}','{cd_r_dt_item["REMARK"]}','{cd_r_dt_item["CREATEBY"]}','{cd_r_dt_item["CREATEDATE"]}','{cd_r_dt_item["CREATETIME"]}')");
                        }

                    }
                }

                ret.IsSuccess = true;
                ret.ErrMsg = "修复数据成功";
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//task_no
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                string art_no = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//art_no
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型编号
                string where = string.Empty;
                string sql = $@"SELECT
task_no,
conclusion,
is_autograph,
autograph,
STAFF_NAME,
conclusion_type
FROM
	aql_cma_task_list_m_cd 
LEFT JOIN HR001M ON STAFF_NO=autograph
where task_no='{task_no}'";

                DataTable cheked_list = DB.GetDataTable(sql);

                sql = $@"select
po,
remark_type,
remark
from aql_cma_task_list_m_cd_r where task_no='{task_no}'";
                DataTable dt_remark = DB.GetDataTable(sql);


                //成品鞋-测试0 部件1 工艺2 材料3 量产拉力4
                sql = $@"select
'1' as  yhzm,
'1' as zsdd,
'1' as ddbz,
'1' as txbz,
'1' as ypd
from dual";
                DataTable data1 = DB.GetDataTable(sql);
                sql = $@"select
'' as keyname,
(SELECT
	TASK_NO
FROM
	QCM_EX_TASK_LIST_M
WHERE
	ID IN (
		SELECT
			MAX (ID) AS ID
		FROM
			QCM_EX_TASK_LIST_M
		WHERE
			TEST_TYPE = '0'
			and ART_NO='{art_no}'
	)) as TASK_NO,
(
SELECT
	TASK_NO
FROM
	QCM_EX_TASK_LIST_M
WHERE
	ID IN (
		SELECT
			MAX (ID) AS ID
		FROM
			QCM_EX_TASK_LIST_M
		WHERE
			TEST_TYPE = '4'
	and ART_NO='{art_no}'
	)) as TASK_NO2
from dual
";
                DataTable data2 = DB.GetDataTable(sql);
                sql = $@"SELECT
    DISTINCT
    max(c.file_url) as file_url,
    max(b.enum_value) as enum_value
    FROM
    	qcm_safety_compliance_file_d A
    left join sys001m b on a.file_type=b.ENUM_CODE
    left JOIN BDM_UPLOAD_FILE_ITEM c ON A.file_guid = c.GUID
    where b.enum_type='enum_qcm_safety' and a.prod_no='{art_no}' and b.enum_code in ('4','5')  GROUP BY a.file_type";
                DataTable data3 = DB.GetDataTable(sql);
                sql = $@"SELECT
    DISTINCT
    max(c.file_url) as file_url,
    max(b.enum_value) as enum_value
    FROM
    	qcm_safety_compliance_file_d A
    left join sys001m b on a.file_type=b.ENUM_CODE
    left JOIN BDM_UPLOAD_FILE_ITEM c ON A.file_guid = c.GUID
    where b.enum_type='enum_qcm_safety' and a.prod_no='{art_no}' and b.enum_code in ('0','1','2','3') GROUP BY a.file_type";
                DataTable data3a = DB.GetDataTable(sql);
                sql = $@"select
	B.FILE_NAME,
	B.FILE_URL,
	a.valid_time
from qcm_jointly_file_d a INNER JOIN (
SELECT
	max(id) as id
FROM
	qcm_jointly_file_d where prod_no='{art_no}'  GROUP BY file_type) b  on a.id=b.id
left join  BDM_UPLOAD_FILE_ITEM b ON a.file_guid = B.GUID";
                DataTable data4 =DB.GetDataTable(sql);

                sql = $@"SELECT
	f.file_url,
	f.file_name,
	M.ID
FROM
	BDM_UPLOAD_FILE_ITEM f
INNER JOIN qcm_shoes_qa_record_file_m M ON f.guid = M .file_id
WHERE
	M.shoes_code ='{shoe_no}'";
                DataTable data5 = DB.GetDataTable(sql);

              
                
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data1", data1);
                dic.Add("data2", data2);
                dic.Add("data3", data3);
                dic.Add("data3a", data3a);
                dic.Add("data4", data4);
                dic.Add("data5", data5);
                dic.Add("cheked_list", cheked_list);
                dic.Add("data_remark", dt_remark);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                dic.Add("user_code", user);
                dic.Add("user_name", DB.GetString($@"SELECT STAFF_NAME FROM HR001M WHERE STAFF_NO='{user}'"));

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
        /// 录入数据
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//task_no
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//art_no
                List<Dictionary<string, object>> dic_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string,object>>>(jarr["dic_iist"].ToString());
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string sql = $@"SELECT PO,
CONCLUSION,
IS_AUTOGRAPH,
AUTOGRAPH,
CONCLUSION_TYPE  FROM AQL_CMA_TASK_LIST_M_CD where task_no='{task_no}'";
                DataTable dt = DB.GetDataTablebyline(sql);
                DataRow[] dr = null;
                //0：验货文件 1：成品鞋测试报告 2：产品安全测试报告CPSIA 3：产品安全测试报告文件 4：FD/VS 结果查询
                //0：验货证明 1：正式订单 2：订单包装材料预算 3：特殊包装 4：样品单<>
                //5：A-01报告 6：FGT报告 7：拉力测试结果 <>
                //8：CPSIA 9：vegan<>
                //10：客户国家特殊要求
                //11：FD/VS
                //12：MCS 13：SHAS 14：量产 15：仓库 16：CMA合格 17：UV-C处理 18：防霉包装纸 19：特殊的外观标准 20：工厂免责声明 21：FIT 22：防霉
                sql = string.Empty;
                foreach (Dictionary<string,object> dic in dic_list)
                {
                    switch (dic["conclusion_type"])
                    {
                        //0：Inspection documents
                        case "0":
                            for (int i = 0; i < 5; i++)
                            {
                                // dr = dt.Select($@"CONCLUSION_TYPE='{dic["conclusion_type"]}'");
                                dr = dt.Select($@"CONCLUSION_TYPE='{i}'");
                                if (dr.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd set conclusion='{dic["conclusion"]}',is_autograph='{dic["is_autograph"]}',autograph='{dic["user_code"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{i}' and task_no='{task_no}';";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,is_autograph,autograph,conclusion_type,createby,createdate,createtime)values('{task_no}','{dic["conclusion"]}','{dic["is_autograph"]}','{dic["user_code"]}','{i}','{userCode}','{date}','{time}');";
                                }
                            }
                            break;
                        case "1":
                            for (int i = 5; i < 8; i++)
                            {
                                // dr = dt.Select($@"CONCLUSION_TYPE='{dic["conclusion_type"]}'");
                                dr = dt.Select($@"CONCLUSION_TYPE='{i}'");
                                if (dr.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd set conclusion='{dic["conclusion"]}',is_autograph='{dic["is_autograph"]}',autograph='{dic["user_code"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{i}' and task_no='{task_no}';";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,is_autograph,autograph,conclusion_type,createby,createdate,createtime)values('{task_no}','{dic["conclusion"]}','{dic["is_autograph"]}','{dic["user_code"]}','{i}','{userCode}','{date}','{time}');";
                                }
                            }
                            break;
                        case "2":
                            for (int i = 8; i <10; i++)
                            {
                                // dr = dt.Select($@"CONCLUSION_TYPE='{dic["conclusion_type"]}'");
                                dr = dt.Select($@"CONCLUSION_TYPE='{i}'");
                                if (dr.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd set conclusion='{dic["conclusion"]}',is_autograph='{dic["is_autograph"]}',autograph='{dic["user_code"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{i}' and task_no='{task_no}';";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,is_autograph,autograph,conclusion_type,createby,createdate,createtime)values('{task_no}','{dic["conclusion"]}','{dic["is_autograph"]}','{dic["user_code"]}','{i}','{userCode}','{date}','{time}');";
                                }
                            }
                            break;
                        case "3":
                            // dr = dt.Select($@"CONCLUSION_TYPE='{dic["conclusion_type"]}'");
                            dr = dt.Select($@"CONCLUSION_TYPE='{10}'");
                            if (dr.Length > 0)
                            {
                                sql += $@" update aql_cma_task_list_m_cd set conclusion='{dic["conclusion"]}',is_autograph='{dic["is_autograph"]}',autograph='{dic["user_code"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{10}' and task_no='{task_no}';";
                            }
                            else
                            {
                                sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,is_autograph,autograph,conclusion_type,createby,createdate,createtime)values('{task_no}','{dic["conclusion"]}','{dic["is_autograph"]}','{dic["user_code"]}','{10}','{userCode}','{date}','{time}');";
                            }
                            break;
                        case "4":
                            // dr = dt.Select($@"CONCLUSION_TYPE='{dic["conclusion_type"]}'");
                            dr = dt.Select($@"CONCLUSION_TYPE='11'");
                            if (dr.Length > 0)
                            {
                                sql += $@"update aql_cma_task_list_m_cd set conclusion='{dic["conclusion"]}',is_autograph='{dic["is_autograph"]}',autograph='{dic["user_code"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='11' and task_no='{task_no}';";
                            }
                            else
                            {
                                sql += $@"insert into aql_cma_task_list_m_cd(task_no,conclusion,is_autograph,autograph,conclusion_type,createby,createdate,createtime)values('{task_no}','{dic["conclusion"]}','{dic["is_autograph"]}','{dic["user_code"]}','11','{userCode}','{date}','{time}');";
                            }
                            break;
                    }
                    
                   
                }
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    DB.ExecuteNonQuery($@"begin {sql} end;");
                }
                Dictionary<string, object> dic_data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(jarr["dic_data"].ToString());//收缩注释内容
                string[] keys = dic_data.Keys.ToArray();
                sql = $@"SELECT
task_no,
REMARK_TYPE,
REMARK
FROM
	AQL_CMA_TASK_LIST_M_CD_R where task_no='{task_no}'";
                DataTable dt2 = DB.GetDataTable(sql);
                DataRow[] dr2 = null;
                //0：验货证明 1：正式订单 2：订单包装材料预算 3：特殊包装 4：样品单 5：A-01报告 6：FGT报告 7：拉力测试结果 8：CPSIA 9：vegan 10：客户国家特殊要求 11：FD/VS 12：MCS
                //
                //
                //
                //pivot88项目核对的
                //13：SHAS 14：量产 15：仓库 16：CMA合格 17：UV-C处理 18：防霉包装纸 19：特殊的外观标准 20：工厂免责声明 21：FIT 22：防霉

                sql = $@"SELECT task_no,
CONCLUSION,
IS_AUTOGRAPH,
AUTOGRAPH,
CONCLUSION_TYPE  FROM AQL_CMA_TASK_LIST_M_CD where task_no='{task_no}'";
                dt = DB.GetDataTablebyline(sql);
                sql = string.Empty;
                string remark = string.Empty;
                string status=string.Empty;//状态
                foreach (var dic in keys)
                {
                    
                    switch (dic)
                    {
                        //验货文件
                        case BotAtype.typekey:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                
                                DataTable return_data  = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if(item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='{BotAtype.typekey}'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey}','{remark}');";
                                }

                            }

                            break;
                        case BotAtype.typekey1:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='{BotAtype.typekey1}'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey1}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey1}','{remark}');";
                                }
                            }

                            break;
                        case BotAtype.typekey2:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='{BotAtype.typekey2}'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey2}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey2}','{remark}');";
                                }
                            }

                            break;
                        case BotAtype.typekey3:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='{BotAtype.typekey3}'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey3}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey3}','{remark}');";
                                }

                            }
                            break;
                        case BotAtype.typekey4:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='4'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey4}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey4}','{remark}');";
                                }

                            }
                            break;
                        case BotAtype.typekey5:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    status = string.Empty;
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype1"]}'");
                                    if (item["btn_passflag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_failflag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["naflag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype1"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype1"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype1"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='5'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey5}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey5}','{remark}');";
                                }
                            }

                            break;
                        case BotAtype.typekey6:
                            jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(dic_data[dic].ToString());
                            if (jarr != null)
                            {
                                DataTable return_data = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["returndata"].ToString());
                                foreach (DataRow item in return_data.Rows)
                                {
                                    dr = dt.Select($@"CONCLUSION_TYPE='{item["ctype2"]}'");
                                    if (item["btn_pass2flag"].ToString() == "1")
                                    {
                                        status = "1";
                                    }
                                    else if (item["btn_fail2flag"].ToString() == "1")
                                    {
                                        status = "0";
                                    }
                                    else if (item["na2flag"].ToString() == "1")
                                    {
                                        status = "2";
                                    }
                                    if (string.IsNullOrWhiteSpace(status))
                                    {
                                        //不存在就相当于取消勾选，那么就删除数据
                                        //sql += $@"delete from aql_cma_task_list_m_cd where conclusion_type='{item["ctype2"]}' and task_no='{task_no}';";
                                        continue;
                                    }
                                    if (dr.Length > 0)
                                    {
                                        sql += $@" update aql_cma_task_list_m_cd set conclusion='{status}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where conclusion_type='{item["ctype2"]}' and task_no='{task_no}';";
                                    }
                                    else
                                    {
                                        sql += $@" insert into aql_cma_task_list_m_cd(task_no,conclusion,conclusion_type,createby,createdate,createtime)values('{task_no}','{status}','{item["ctype2"]}','{userCode}','{date}','{time}');";
                                    }
                                }
                                remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                                if (string.IsNullOrWhiteSpace(remark))
                                {
                                    break;
                                }
                                dr2 = dt2.Select($@"REMARK_TYPE='{BotAtype.typekey6}'");
                                if (dr2.Length > 0)
                                {
                                    sql += $@" update aql_cma_task_list_m_cd_r set remark='{remark}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}'
 where task_no='{task_no}' and remark_type='{BotAtype.typekey6}' ;";
                                }
                                else
                                {
                                    sql += $@" insert into aql_cma_task_list_m_cd_r(task_no,remark_type,remark) values('{task_no}','{BotAtype.typekey6}','{remark}');";
                                }
                            }
                          

                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(sql))
                {
                    DB.ExecuteNonQuery($@"begin {sql} end;");
                }

                #region Added by Ashok to Update Check data status for Pivot sync
                string checkdatasql = $@"UPDATE AQL_PIVOT_DATA_STATUS
SET check_data_status = 1,
    modified_by = {userCode},
    modified_at = SYSDATE
WHERE task_no = '{task_no}'";
                DB.ExecuteNonQuery(checkdatasql);
                #endregion


                ret.IsSuccess = true;
                ret.ErrMsg = "保存成功";
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
        /// 加载收缩框内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetviewListdata(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//art_no
                string where = string.Empty;
                string sql = $@"select";

                sql = $@"SELECT
    DISTINCT
    max(c.file_url),
    max(b.enum_value)
    FROM
    	qcm_safety_compliance_file_d A
    left join sys001m b on a.file_type=b.ENUM_CODE
    left JOIN BDM_UPLOAD_FILE_ITEM c ON A.file_guid = c.GUID
    where b.enum_type='enum_qcm_safety' and a.prod_no='{po}' and b.enum_code in ('4','5') GROUP BY a.file_type";
                DataTable data3 = DB.GetDataTable(sql);
                sql = $@"SELECT
    DISTINCT
    max(c.file_url),
    max(b.enum_value)
    FROM
    	qcm_safety_compliance_file_d A
    left join sys001m b on a.file_type=b.ENUM_CODE
    left JOIN BDM_UPLOAD_FILE_ITEM c ON A.file_guid = c.GUID
    where b.enum_type='enum_qcm_safety' and a.prod_no='{po}' and b.enum_code in ('0','1','2','3') GROUP BY a.file_type";
                DataTable data3a = DB.GetDataTable(sql);

                sql = $@"select
	B.FILE_NAME,
	B.FILE_URL,
	a.valid_time
from qcm_jointly_file_d a INNER JOIN (
SELECT
	max(id) as id
FROM
	qcm_jointly_file_d where prod_no='{po}'  GROUP BY file_type) b  on a.id=b.id
left join  BDM_UPLOAD_FILE_ITEM b ON a.file_guid = B.GUID";
                DataTable data4 = DB.GetDataTable(sql);

                DataTable data1 = new DataTable();
                DataTable data2 = new DataTable();
                DataTable data5 = new DataTable();


                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data1", data1);
                dic.Add("data2", data2);
                dic.Add("data3", data3);
                dic.Add("data3a", data3a);
                dic.Add("data4", data4);
                dic.Add("data5", data5);
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
        /// 订单包装材料预算
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUrlDDBZCL(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//art_no
                var req = new OrderPackaging.MaterialBudget.DT_AEQS2_REQI_REQUESTBODY();
                req.PO =DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");
                req.LANGU = "EN";
                var apires = SJ_SAPAPI.SapApiHelper.GetOrderPackagingMaterialBudget(req, ReqObj);
                DT_AEQS2_RSP pdf = new DT_AEQS2_RSP();
                string errmes = apires.ERR_MSG;
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {
                    pdf.PDF_URL= apires.PDF_URL;

                    //1.Determine whether the nfs N disk file exists
                    SJeMES_Framework_NETCore.DBHelper.DataBase sqlseverDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                    var companyCode = SJeMES_Framework_NETCore.Web.System.GetCompanyCodeByToken(ReqObj.UserToken);
                    var nfspath = sqlseverDB.GetString($@"
SELECT
	nfspath
FROM
	[dbo].[SYSORG01M]
WHERE
	UPPER (org) = '{companyCode.ToUpper()}'");
                    string nfsFilePath = $@"{nfspath}{pdf.PDF_URL}".Replace("/", @"\");

                    if (!File.Exists(nfsFilePath))
                    {
                        ret.ErrMsg = $@"File [{nfsFilePath}] does not exist";
                        ret.IsSuccess = false;
                        return ret;
                    }
                    else
                    {

                        //var filepath = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot";
                        //if (!System.IO.Directory.Exists(filepath + pdf.PDF_URL.Substring(0, pdf.PDF_URL.LastIndexOf(@"/") + 1)))
                        //{
                        //    Create the file if it does not exist
                        //    System.IO.Directory.CreateDirectory(filepath + pdf.PDF_URL.Substring(0, pdf.PDF_URL.LastIndexOf(@"/") + 1));
                        //}
                        //string file = filepath + pdf.PDF_URL.Replace("/", @"\");

                        //Treat it as a directory first. If the directory exists, recursively copy the files under the directory.
                        //if (!System.IO.File.Exists(file))
                        //{
                        //    File.Delete(file);
                        //    System.IO.File.Copy(nfsFilePath, file);
                        //}
                        //else
                        //{
                        //    System.IO.File.Copy(nfsFilePath, file);
                        //}

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("PDF_URL", pdf.PDF_URL);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        ret.IsSuccess = true;

                    }
                    

                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = errmes;
                    return ret;
                }
              
               

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 特殊包装材料预算
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUrlTSBZCL(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//art_no
                var req = new Special.Packaging.Information.DT_SD023_REQI_REQUESTBody();
                req.BSTNK =po;
                var apires = SJ_SAPAPI.SapApiHelper.GetSpecialPackagingInformation(req, ReqObj);
                DT_SD023_RSP data = new DT_SD023_RSP();
                string errmes = apires.ERP_MSG;
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {
                    data.E_RESPONSE = apires.E_RESPONSE;
                    //1.判断nfs N盘文件存不存在
                    string nfsFilePath = $@"N:/{data.E_RESPONSE}";
                    if (!File.Exists(nfsFilePath))
                    {
                        ret.ErrMsg = $@"{nfsFilePath}文件不存在";
                        ret.IsSuccess = false;
                        return ret;
                    }
                    else
                    {

                        var filepath = System.IO.Directory.GetCurrentDirectory() + @"\wwwroot\AEQS";
                        if (!System.IO.Directory.Exists(filepath))
                        {
                            //不存在就创建文件
                            System.IO.Directory.CreateDirectory(filepath);
                        }
                        string path = nfsFilePath.Substring(0, nfsFilePath.LastIndexOf(@"/") + 1);
                        string[] fileList = System.IO.Directory.GetFileSystemEntries(path);
                        foreach (string file in fileList)
                        {
                            // 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                            if (System.IO.Directory.Exists(file))
                            {
                                System.IO.File.Copy(file, filepath + @"\\" + System.IO.Path.GetFileName(file));
                            }
                            else
                            {
                                System.IO.File.Copy(file, filepath + @"\\" + System.IO.Path.GetFileName(file), true);
                            }
                        }

                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("E_RESPONSE", data.E_RESPONSE);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                        ret.IsSuccess = true;

                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = errmes;
                    return ret;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 获取sap样品单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLSampleList(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art_no
        //        var req = new Art.Bom.DT_PP113_REQI_REQUESTBODY();
        //        req.art_no = art_no;
        //        req.item_no = "";
        //        req.stage = "PRS";
        //        req.type = "1";// 1-量产；2-开发
        //        var apires = SJ_SAPAPI.SapApiHelper.GetArtBom(req, ReqObj);
        //        if (apires.IS_SUCCESS.ToLower() == "true")
        //        {
        //            ret.IsSuccess = true;
        //            ret.RetData = JsonConvert.SerializeObject(apires.RES.DATAS);
        //        }
        //        else
        //        {
        //            ret.IsSuccess = false;
        //            ret.ErrMsg = apires.ERR_MSG;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }

        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLSampleList(object OBJ)
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
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art_no
                var req = new Art.Bom.DT_PP113_REQI_REQUESTBODY();
                req.art_no = art_no;
                //req.werks = "1003"; //APE-1003 APH-4003 APC-5003
                req.werks = "5003"; //APE-1003 APH-4003 APC-5003
                req.item_no = "";
                req.stage = "PRS";//In the non-transmission stage, the latest sample list is taken; in the transmission stage, the latest sample list of that stage is taken
                req.type = "";//1-Mass production; 2-Development (out of date)
                //req.langu = "1";//1-中文 EN-英文 VI-越文
                req.langu = "EN";//1-Chinese EN-English VI-Vietnamese
                var apires = SJ_SAPAPI.SapApiHelper.GetArtBom(req, ReqObj);
                if (apires.IS_SUCCESS.ToLower() == "true")
                {
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(apires.RES.DATAS);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = apires.ERR_MSG;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ArtInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string po = jarr["po"].ToString();
                string art = jarr["art"].ToString();

                Dictionary<string, object> res = new Dictionary<string, object>();
                res.Add("art_name", "");
                res.Add("shoe_name", "");
                res.Add("po_date", "");

                DB.Open();
                DB.BeginTransaction();
                string sql = $@"
SELECT
	p.name_t as prod_name,
	s.NAME_T as shoe_name
FROM
BDM_RD_PROD p
LEFT JOIN BDM_RD_STYLE s on p.SHOE_NO=s.SHOE_NO
where p.PROD_NO = '{art}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    res["art_name"] = dt.Rows[0]["prod_name"].ToString();
                    res["shoe_name"] = dt.Rows[0]["shoe_name"].ToString();
                }

                sql = $@"
SELECT
	i.NST
FROM
	bdm_se_order_item i
INNER JOIN BDM_SE_ORDER_MASTER m on i.SE_ID=m.SE_ID and i.ORG_ID=m.ORG_ID
WHERE
	m.MER_PO='{po}'
";
                dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    res["po_date"] = dt.Rows[0]["NST"].ToString();
                }

                ret.RetData = JsonConvert.SerializeObject(res);
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


        //-------------------------------------分段资料------------------------------------//

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatasMax(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po

                #region 鞋带or大底
                var req = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY();
                List<DT_PP114_RSPRESDATAS> list = new List<DT_PP114_RSPRESDATAS>();
                req.PO = po;
                req.tab = "A";
                var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {

                    list = apires.RES.DATAS.ToList();
                }
                List<DT_PP114_RSPRESDATAS> list1 = new List<DT_PP114_RSPRESDATAS>();
                req.tab = "B";
                apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {
                    list1 = apires.RES.DATAS.ToList();
                }
                #endregion

                #region 高度/翘度
                var req1 = new Subsection.HeightWarpage.DT_PP115_REQI_REQUESTBODY();
                req1.PO = po;
                var apires1 = SJ_SAPAPI.SapApiHelper.GetSubsectionHeightWarpage(req1, ReqObj);
                List<DT_PP115_RSPRESDATAS> list2 = new List<DT_PP115_RSPRESDATAS>();
                if (Convert.ToBoolean(apires1.IS_SUCCESS))
                {

                    list2 = apires1.RES.DATAS.ToList();
                }
                #endregion

                #region lOGO
                var req2 = new Subsection.Logo.DT_PP116_REQI_REQUESTBODY();
                req2.PO = po;
                var apires2 = SJ_SAPAPI.SapApiHelper.GetSubsectionLogo(req2, ReqObj);
                List<DT_PP116_RSPRESDATAS> list3 = new List<DT_PP116_RSPRESDATAS>();
                if (Convert.ToBoolean(apires2.IS_SUCCESS))
                {
                    list3 = apires2.RES.DATAS.ToList();
                }
                
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = TableExtension.ToDataTable(list);
                DataTable dt1 = TableExtension.ToDataTable(list1);
                DataTable dt2 = TableExtension.ToDataTable(list2);
                DataTable dt3 = TableExtension.ToDataTable(list3);
                dic.Add("list", dt);//鞋带
                dic.Add("list1", dt1);//大底
                dic.Add("list2", dt2);//高度/翘度
                dic.Add("list3", dt3);//LOGO
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
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
        //        po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");

        //        #region 鞋带
        //        var req = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY();
        //        List<DT_PP114_RSPRESDATAS> list = new List<DT_PP114_RSPRESDATAS>();
        //        req.PO = po;
        //        req.tab = "A";
        //        var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
        //        if (Convert.ToBoolean(apires.IS_SUCCESS))
        //        {

        //            list = apires.RES.DATAS.ToList();
        //        } 
        //        #endregion

        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        DataTable dt = TableExtension.ToDataTable(list);
        //        dic.Add("list", dt);//鞋带
        //        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }

        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");

                #region 鞋带
                var req = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY();
                List<DT_PP114_RSPRESDATAS> list = new List<DT_PP114_RSPRESDATAS>();
                req.PO = po;
                req.tab = "A"; //A-鞋带 B-大底
                //req.LANGU = "1";//1-中文 EN-英文 VI-越文
                req.LANGU = "EN";//1-中文 EN-英文 VI-越文
                var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {

                    list = apires.RES.DATAS.ToList();
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = TableExtension.ToDataTable(list);
                dic.Add("list", dt);//鞋带
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

        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas1(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
        //        po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");

        //        #region 大底
        //        List<DT_PP114_RSPRESDATAS> list1 = new List<DT_PP114_RSPRESDATAS>();
        //        var req = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY();
        //        req.PO = po;
        //        req.tab = "B";
        //        var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
        //        if (Convert.ToBoolean(apires.IS_SUCCESS))
        //        {
        //            list1 = apires.RES.DATAS.ToList();
        //        }
        //        #endregion

        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        DataTable dt1 = TableExtension.ToDataTable(list1);
        //        dic.Add("list1", dt1);//大底
        //        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }

        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas1(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");

                #region 大底
                List<DT_PP114_RSPRESDATAS> list1 = new List<DT_PP114_RSPRESDATAS>();
                var req = new Subsection.ShoelacesAndOutsole.DT_PP114_REQI_REQUESTBODY();
                req.PO = po;
                req.tab = "B";
                //req.LANGU = "1";//1-中文 EN-英文 VI-越文
                req.LANGU = "EN";//1-中文 EN-英文 VI-越文
                var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionShoelacesAndOutsole(req, ReqObj);
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {
                    list1 = apires.RES.DATAS.ToList();
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt1 = TableExtension.ToDataTable(list1);
                dic.Add("list1", dt1);//大底
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas2(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");

                #region 高度/翘度
                var req = new Subsection.HeightWarpage.DT_PP115_REQI_REQUESTBODY();
                req.PO = po;
                var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionHeightWarpage(req, ReqObj);
                List<DT_PP115_RSPRESDATAS> list2 = new List<DT_PP115_RSPRESDATAS>();
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {

                    list2 = apires.RES.DATAS.ToList();
                }
                #endregion

                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = TableExtension.ToDataTable(list2);
                dic.Add("list2", dt);//高度/翘度
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
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas3(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
        //    try
        //    {
        //        DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //        string Data = ReqObj.Data.ToString();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        //转译
        //        string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
        //        po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");
        //        #region lOGO
        //        var req = new Subsection.Logo.DT_PP116_REQI_REQUESTBODY();
        //        req.PO = po;
        //        var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionLogo(req, ReqObj);
        //        List<DT_PP116_RSPRESDATAS> list3 = new List<DT_PP116_RSPRESDATAS>();
        //        if (Convert.ToBoolean(apires.IS_SUCCESS))
        //        {
        //            list3 = apires.RES.DATAS.ToList();
        //        }
        //        #endregion
        //        Dictionary<string, object> dic = new Dictionary<string, object>();
        //        DataTable dt = TableExtension.ToDataTable(list3);
        //        dic.Add("list3", dt);//LOGO
        //        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
        //        ret.IsSuccess = true;

        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }

        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDfdDatas3(object OBJ)
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
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//po
                po = DB.GetString($@"SELECT SE_ID FROM BDM_SE_ORDER_MASTER WHERE MER_PO='{po}'");
                #region lOGO
                var req = new Subsection.Logo.DT_PP116_REQI_REQUESTBODY();
                req.PO = po;
                //req.WERKS = "1001"; //APE 1001 APH-4001 APC-5001
                req.WERKS = "5001"; //APE 1001 APH-4001 APC-5001

                //req.LANGU = "1";//1-中文 EN-英文 VI-越文
                req.LANGU = "EN";//1-中文 EN-英文 VI-越文
                var apires = SJ_SAPAPI.SapApiHelper.GetSubsectionLogo(req, ReqObj);
                List<DT_PP116_RSPRESDATAS> list3 = new List<DT_PP116_RSPRESDATAS>();
                if (Convert.ToBoolean(apires.IS_SUCCESS))
                {
                    list3 = apires.RES.DATAS.ToList();
                }
                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = TableExtension.ToDataTable(list3);
                dic.Add("list3", dt);//LOGO
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




        //-----------------------湿度录入----------------//
        /// <summary>
        /// 数据展示（湿度录入）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SduyMain(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";

                string sql = $@"SELECT
BAD_ITEM_CODE,
BAD_ITEM_NAME,
PROBLEM_LEVEL
FROM AQL_CMA_TASK_LIST_M_H where task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.RetData = JsonConvert.SerializeObject(dic);
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
        /// 数据展示（湿度录入）备注
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SduyMainReamrk(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string sql = $@"select
meterial,
standard_value,
inspection,
measurement,
corrected_action
from aql_cma_task_list_m_h_r";
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.RetData = JsonConvert.SerializeObject(dic);
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
        /// 湿度录入数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public  SJeMES_Framework_NETCore.WebAPI.ResultObject SduyCommit(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                List<Dictionary<string, object>> dic_list = jarr.ContainsKey("diclist") ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarr["diclist"].ToString()) : null ;//条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = $@"SELECT
BAD_ITEM_CODE,
BAD_ITEM_NAME,
PROBLEM_LEVEL
FROM AQL_CMA_TASK_LIST_M_H where task_no='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);
                sql = String.Empty;
                if (dic_list.Count > 0)
                {
                    DataRow[] dr = null;
                    foreach (Dictionary<string,object> item in dic_list)
                    {
                        dr = dt.Select($@"BAD_ITEM_CODE='{item["bad_item_code"]}' and BAD_ITEM_NAME='{item["bad_item_name"]}'");
                        if (dr.Length > 0)
                        {
                            sql += $@"update AQL_CMA_TASK_LIST_M_H set PROBLEM_LEVEL='{item["problem_level"]}' where BAD_ITEM_CODE='{item["bad_item_code"]}' and BAD_ITEM_NAME='{item["bad_item_name"]}' and task_no='{task_no}';";
                        }
                        else
                        {
                            sql += $@"insert into AQL_CMA_TASK_LIST_M_H(TASK_NO,BAD_ITEM_CODE,BAD_ITEM_NAME,PROBLEM_LEVEL) VALUES('{task_no}','{item["bad_item_code"]}','{item["bad_item_name"]}','{item["problem_level"]}');";
                        }
                     
                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        DB.ExecuteNonQuery($@"begin {sql} end;");
                    }
                }
                ret.IsSuccess = true;
                ret.ErrMsg = "Edited_Successfully";
                DB.Commit();

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.ErrMsg = "Editing_Failed, Reason：" + ex.Message;
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
        /// 更新湿度录入编辑状态
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditHState(object OBJ)
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
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//条件 任务编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                string sql = string.Empty;

                sql = $@"update aql_cma_task_list_m set H_EDIT_STATE='1',modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
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
        /// 获取APP2报告文件路径
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ArtFileInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string po = jarr["po"].ToString();
                string art = jarr["art"].ToString();
                // A-01 Report Naming Rules [art&po&original file name]
                string sql = $@"
SELECT 
substr(a.file_name,1,instr(a.file_name,'&')-1) art,
substr(a.file_name,instr(a.file_name,'&')+1,instr(a.file_name,'&',instr(a.file_name,'&')+1)-instr(a.file_name,'&')-1) as po,
	a.ID,
	a.FILE_NAME,
	a.CURR_UPLOAD_TIME upload_time,
	a.CURR_FILE_GUID file_guid,
	b.file_url
FROM
	AQL_APP_T_FILE_M a
INNER JOIN bdm_upload_file_item b ON a.CURR_FILE_GUID = b.guid
where  
substr(a.file_name,1,instr(a.file_name,'&')-1) =  '{art}'
and substr(a.file_name,instr(a.file_name,'&')+1,instr(a.file_name,'&',instr(a.file_name,'&')+1)-instr(a.file_name,'&')-1) = '{po}' 
and a.EFFECT='0' 
ORDER BY a.ID DESC";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Data", dt);
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_ArtReportFileInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TEST_TYPE = jarr["TEST_TYPE"].ToString();
                string art = jarr["art"].ToString();

                string sql = $@"
SELECT
    QCM_EX_TASK_LIST_M_F.ID,
	QCM_EX_TASK_LIST_M_F.TASK_NO,
	QCM_EX_TASK_LIST_M_F.ART_NO,
	QCM_EX_TASK_LIST_M_F.FILE_NAME,
	QCM_EX_TASK_LIST_M_F.TEST_TYPE,
	BDM_UPLOAD_FILE_ITEM.FILE_URL net_file_url,
	BDM_UPLOAD_FILE_ITEM.FILE_URL file_url,
	QCM_EX_TASK_LIST_M_F.UPLOAD_TIME
FROM
	QCM_EX_TASK_LIST_M_F
LEFT JOIN BDM_UPLOAD_FILE_ITEM ON QCM_EX_TASK_LIST_M_F.FILE_GUID = BDM_UPLOAD_FILE_ITEM.GUID
WHERE 1=1
AND QCM_EX_TASK_LIST_M_F.ART_NO = '{art}'
AND QCM_EX_TASK_LIST_M_F.TEST_TYPE ='{TEST_TYPE}'
ORDER BY QCM_EX_TASK_LIST_M_F.id desc
";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Data", dt);
                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(p);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found！";
                }

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sclx = jarr.ContainsKey("TEST_TYPE") ? jarr["TEST_TYPE"].ToString() : "";
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString() : "";
                //string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";

                DataTable dtres = new DataTable();

                dtres.Columns.Add("task_no", typeof(string));
                dtres.Columns.Add("file_name", typeof(string));
                dtres.Columns.Add("file_url", typeof(string));
                dtres.Columns.Add("net_file_url", typeof(string));
                dtres.Columns.Add("UPLOAD_TIME", typeof(string));


                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("order_po", $@"'%{PO}%'");
                paramsDic.Add("TEST_TYPE", sclx);
                //                string sql = $@"
                //SELECT
                //    QCM_EX_TASK_LIST_M_F.ID,
                //	QCM_EX_TASK_LIST_M_F.FILE_NAME,
                //	BDM_UPLOAD_FILE_ITEM.FILE_URL as file_url,
                //	BDM_UPLOAD_FILE_ITEM.GUID as guid,
                //    	QCM_EX_TASK_LIST_M_F.UPLOAD_TIME
                //FROM
                //	QCM_EX_TASK_LIST_M_F
                //LEFT JOIN BDM_UPLOAD_FILE_ITEM ON QCM_EX_TASK_LIST_M_F.FILE_GUID = BDM_UPLOAD_FILE_ITEM.GUID
                //WHERE 1=1
                //AND QCM_EX_TASK_LIST_M_F.ART_NO = @ART_NO
                //AND QCM_EX_TASK_LIST_M_F.TEST_TYPE =@TEST_TYPE";
                string sql = $@"SELECT DISTINCT TASK_NO from QCM_EX_TASK_LIST_M WHERE order_po like '%{PO}%'";

                DataTable dt = DB.GetDataTable(sql, paramsDic);
                //PO包含任务编号集合
                int i = 0;
                foreach (DataRow item in dt.Rows)
                {
                    sql = $@"
SELECT 
QCM_EX_TASK_LIST_M_F.TASK_NO,
QCM_EX_TASK_LIST_M_F.FILE_NAME,
	BDM_UPLOAD_FILE_ITEM.FILE_URL as file_url,
	BDM_UPLOAD_FILE_ITEM.GUID as guid,
	QCM_EX_TASK_LIST_M_F.UPLOAD_TIME
 FROM
 QCM_EX_TASK_LIST_M_F 
LEFT JOIN QCM_EX_TASK_LIST_M ON QCM_EX_TASK_LIST_M_F.TASK_NO = QCM_EX_TASK_LIST_M.TASK_NO
LEFT JOIN BDM_UPLOAD_FILE_ITEM  ON QCM_EX_TASK_LIST_M_F.FILE_GUID = BDM_UPLOAD_FILE_ITEM.GUID
WHERE 1=1 AND QCM_EX_TASK_LIST_M.TASK_NO = '{item["TASK_NO"]}' AND QCM_EX_TASK_LIST_M_F.TEST_TYPE ='{sclx}' ORDER BY UPLOAD_TIME desc";

                    DataTable dtt = DB.GetDataTable(sql);

                    foreach (DataRow item2 in dtt.Rows)
                    {
                        //3、添加数据行
                        DataRow dr = dtres.NewRow();
                        dr["task_no"] = item2["TASK_NO"]; //
                        dr["file_name"] = item2["FILE_NAME"];
                        dr["file_url"] = item2["file_url"];//
                        dr["net_file_url"] = item2["file_url"];//
                        dr["UPLOAD_TIME"] = item2["UPLOAD_TIME"];//
                        dtres.Rows.Add(dr);
                        //break;
                    }

                    i++;
                }
                dtres.DefaultView.Sort = "UPLOAD_TIME desc";
                dtres = dtres.DefaultView.ToTable();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dtres);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
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
