using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Aeqinfom
    {
        //----------------------------------PC--------------------------------------//
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
        /// pc主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                string eq_info_name = jarr.ContainsKey("eq_info_name") ? jarr["eq_info_name"].ToString() : "";
                string department_name = jarr.ContainsKey("department_name") ? jarr["department_name"].ToString() : "";
                string control_type = jarr.ContainsKey("control_type") ? jarr["control_type"].ToString() : "";
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";
                string eq_name = jarr.ContainsKey("eq_name") ? jarr["eq_name"].ToString() : "";

                string jiaz_date = jarr.ContainsKey("jiaz_date") ? jarr["jiaz_date"].ToString() : "";
                string jinc_date = jarr.ContainsKey("jinc_date") ? jarr["jinc_date"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string strwhere = string.Empty;
                if (!string.IsNullOrWhiteSpace(eq_info_no))
                {
                    strwhere += $" and a.eq_info_no like '%{eq_info_no}%'";
                }
                if (!string.IsNullOrWhiteSpace(eq_info_name))
                {
                    strwhere += $" and a.eq_info_name like '%{eq_info_name}%'";
                }
                if (!string.IsNullOrWhiteSpace(department_name))
                {
                    strwhere += $" and a.department_name like '%{department_name}%'";
                }
                //               if (!string.IsNullOrWhiteSpace(control_type))
                //               {
                //                   strwhere += $@" and (CASE
                //		when  a.control_type='0' then '全部'
                //		when  a.control_type='1' then '制程机器'
                //		when  a.control_type='2' then '检验工具'
                //		when  a.control_type='3' then '测试设备'
                //		when  a.control_type='4' then '其他'
                //end ) like '%{control_type}%'";
                //               }
                if (!string.IsNullOrWhiteSpace(control_type))
                {
                    strwhere += $@" and (CASE
			when  a.control_type='0' then 'All'
			when  a.control_type='1' then 'Process_Machine'
			when  a.control_type='2' then 'Validation_Tools'
			when  a.control_type='3' then 'Test_Equipment'
			when  a.control_type='4' then 'Other'
	end ) like '%{control_type}%'";
                }
                if (!string.IsNullOrWhiteSpace(remark))
                {
                    strwhere += $" and a.remark like '%{remark}%'";
                }
                if (!string.IsNullOrWhiteSpace(eq_name))
                {
                    strwhere += $" and a.eq_name like '%{eq_name}%'";
                }
                if (!string.IsNullOrWhiteSpace(jinc_date))
                {
                    strwhere += $@" and a.WH_DATE like '%{jinc_date}%'";
                }
                if (!string.IsNullOrWhiteSpace(jiaz_date))
                {
                    strwhere += $@" and a.JZ_DATE like '%{jiaz_date}%'";
                }
                //                string sql = $@"select
                //	b.id,
                //	a.eq_info_no,--编号
                //	a.eq_info_name,--设备名称
                //	a.department_code,--部门
                //	a.department_name,
                //	a.workshop_section_no,--工段
                //	a.workshop_section_name,
                //	a.eq_no,--设备类型编号
                //	a.eq_name,--设备类型
                //	CASE
                //			when  a.control_type='0' then '全部'
                //			when  a.control_type='1' then '制程机器'
                //			when  a.control_type='2' then '检验工具'
                //			when  a.control_type='3' then '测试设备'
                //			when  a.control_type='4' then '其他'
                //	end  as control_name,
                //    case
                //        when device_state='0' then '正常'
                //        when device_state='1' then '报废'
                //        when  device_state='2' then '送修'
                //    end  as device_state,--设备状态
                //	a.control_type,--管控类型设备
                //	a.wh_date,--进仓日期
                //	a.jz_date,--最近校正日期
                //    b.correction_frequency,-- 校正评率
                //	a.remark
                //from
                //	bdm_eq_info_m a 
                //left join  BDM_EQ_TYPE_M b on a.eq_no=b.EQ_NO and a.eq_name=b.EQ_NAME and a.control_type=b.control_type
                //where 1=1 {strwhere}
                //order by a.id desc
                //";
                string sql = $@"select
	b.id,
	a.eq_info_no,--编号
	a.eq_info_name,--设备名称
	a.department_code,--部门
	a.department_name,
	a.workshop_section_no,--工段
	a.workshop_section_name,
	a.eq_no,--设备类型编号
	a.eq_name,--设备类型
	CASE
			when  a.control_type='0' then 'All'
			when  a.control_type='1' then 'Process_Machine'
			when  a.control_type='2' then 'Validation_Tools'
			when  a.control_type='3' then 'Test_Equipment'
			when  a.control_type='4' then 'Other'
	end  as control_name,
    case
        when device_state='0' then 'Normal'
        when device_state='1' then 'Scrapped'
        when  device_state='2' then 'Send_for_repair'
    end  as device_state,--设备状态
	a.control_type,--管控类型设备
	a.wh_date,--进仓日期
	a.jz_date,--最近校正日期
    b.correction_frequency,-- 校正评率
	a.remark
from
	bdm_eq_info_m a 
left join  BDM_EQ_TYPE_M b on a.eq_no=b.EQ_NO and a.eq_name=b.EQ_NAME and a.control_type=b.control_type
where 1=1 {strwhere}
order by a.id desc
";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int total = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
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
        /// <summary>
        /// 明细表身数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetDataCom(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                string eq_no = jarr.ContainsKey("eq_no") ? jarr["eq_no"].ToString() : "";
             
                string sql = $@"
select 
	a.id,
	a.eq_info_no,--设备信息编号
	a.eq_no,--设备类型编号
	a.item_code,--项目编号
	a.item_name,--项目名称
	b.CALIBRATION_STANDARD,--校正标准
	case
			when a.maintain='0' then 'FAIL'
			when a.maintain='1' then 'PASS'
	end as  maintain, --检验结果
	a.remark --备注
from bdm_eq_info_eq_type_m a
LEFT JOIN bdm_correction_item_m b on b.CORRECTION_ITEM_CODE = a.ITEM_CODE
where a.EQ_INFO_NO='{eq_info_no}' and a.EQ_NO='{eq_no}' order by a.id desc
";
                DataTable dt = DB.GetDataTable(sql);
                sql = $@"select report_code,device_state from bdm_eq_info_m where EQ_INFO_NO='{eq_info_no}' and EQ_NO='{eq_no}'";
                DataTable dt2 = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("data2", dt2);
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
        /// <summary>
        /// 加载初始化表身数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject CommitData(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                string eq_no = jarr.ContainsKey("eq_no") ? jarr["eq_no"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = $@"SELECT
	ITEM_CODE,
	ITEM_NAME
FROM
	BDM_EQ_TYPE_D
WHERE
	EQ_TYPE = '0'
AND M_ID = '{id}'";
                DataTable dt = DB.GetDataTable(sql);//设备信息

                sql = $@"select 
eq_info_no,
eq_no,
item_code,
item_name,
eq_type,
maintain,
remark
 from bdm_eq_info_eq_type_m where eq_info_no='{eq_info_no}' and eq_no='{eq_no}' ";//当下要添加的表

                DataTable dt2 = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    sql = String.Empty;
                    if (dt2.Rows.Count < 1)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            sql += $@"insert into bdm_eq_info_eq_type_m(eq_info_no,eq_no,item_code,item_name,eq_type,createby,createdate,createtime) values('{eq_info_no}','{eq_no}','{item["ITEM_CODE"]}','{item["ITEM_NAME"]}','0','{Usercode}','{date}','{time}');";
                        }
                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            DB.ExecuteNonQuery($@"begin {sql} end;");
                        }
                    }
                }
                else
                {
                    ret.ErrMsg = $@"Basic maintenance equipment information number{eq_info_no},Device type number{eq_no}does not exist";
                    ret.IsSuccess = false;
                    return ret;
                }
                DB.Commit();
                ret.ErrMsg = "Successfully initialized data！";
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
        /// 保存明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject CommitDataMax(object OBJ)
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
                NullKey(jarr);
                string report_code = jarr.ContainsKey("report_code") ? jarr["report_code"].ToString() : "";//校正报告编号
                string device_state = jarr.ContainsKey("device_state") ? jarr["device_state"].ToString() : "";//设备状态
                DataTable data=jarr.ContainsKey("data")? JsonConvert.DeserializeObject<DataTable>(jarr["data"].ToString()) : null;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = $@"select 
id,
eq_info_no,--设备信息编号
eq_no,--设备类型编号
item_code,--项目编号
item_name,--项目名称
eq_type,--校正标准
maintain, --检验结果
remark --备注
 from bdm_eq_info_eq_type_m";
                DataTable dt = DB.GetDataTable(sql);



                DataRow[] dr = null;
                if (data.Rows.Count > 0)
                {
                    sql = String.Empty;
                    int i = 1;
                    string eq_info_no = string.Empty;
                    string eq_no = string.Empty;
                    string status = string.Empty;
                    foreach (DataRow item in data.Rows)
                    {
                        dr = dt.Select($@"EQ_INFO_NO='{item["Column5"]}' and EQ_NO='{item["Column6"]}' and ID='{item["Column7"]}'");
                        if (dr.Length > 0)
                        {
                            if (item["Column3"].ToString() == "PASS")
                            {
                                status = "1";
                            }
                            else
                            {
                                status = "0";
                            }
                            sql += $@"update bdm_eq_info_eq_type_m set maintain='{status}',remark='{item["Column4"]}' where eq_info_no='{item["Column5"]}' and eq_no='{item["Column6"]}' and id='{item["Column7"]}';";
                        }
                        i++;
                        if (i == 2)
                        {
                            eq_info_no = item["Column5"].ToString();
                            eq_no= item["Column6"].ToString();
                        }
                        
                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        DB.ExecuteNonQuery($@"begin {sql} end;");
                    }
                    sql = $@"select (1) from bdm_eq_info_m where eq_info_no='{eq_info_no}' and eq_no='{eq_no}'";
                    int num = DB.GetInt32(sql);
                    if (num > 0)
                    {
                        sql = $@"update bdm_eq_info_m set report_code='{report_code}',device_state='{device_state}',jz_date='{DateTime.Now.ToString("yyyy-MM-dd")}' where eq_info_no='{eq_info_no}' and eq_no='{eq_no}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                
                DB.Commit();
                ret.ErrMsg = "Saved successfully！";
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
        //-------------------------------------PDA--------------------------------------//
        /// <summary>
        /// 扫描工具条码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetList(object OBJ)
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
              
                string eq_info_no = jarr.ContainsKey("eq_info_no") ? jarr["eq_info_no"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string sql = $@"
select
    a.eq_no,
	b.id,
	a.eq_info_no,--编号
    a.eq_info_name --设备信息名称
from
	bdm_eq_info_m a 
left join  BDM_EQ_TYPE_M b on a.eq_no=b.EQ_NO and a.eq_name=b.EQ_NAME and a.control_type=b.control_type
where eq_info_no='{eq_info_no}'";
                DataTable dt = DB.GetDataTable(sql);//根据条码查看表头
                if (dt.Rows.Count > 0)
                {
                    sql = $@"SELECT
	ITEM_CODE,
	ITEM_NAME
FROM
	BDM_EQ_TYPE_D
WHERE
	EQ_TYPE = '0'
AND M_ID = '{dt.Rows[0]["id"]}'";
                    DataTable dt1 = DB.GetDataTable(sql);//有数据查明细表内容

                    if (dt1.Rows.Count > 0)
                    {
                        sql = $@"
select 
a.eq_info_no,
a.item_code,--项目编号
a.item_name,--项目名称
a.maintain,--结果
b.CALIBRATION_STANDARD remark --标准说明
from bdm_eq_info_eq_type_m a
LEFT JOIN bdm_correction_item_m b on b.CORRECTION_ITEM_CODE = a.ITEM_CODE 
where a.eq_type='0' and a.eq_info_no='{eq_info_no}'";
                        DataTable dt2 = DB.GetDataTable(sql);//查询是否初始化过数据
                        if (dt2.Rows.Count < 1)
                        {
                            //没有就添加初始化数据
                            sql = String.Empty;
                            foreach (DataRow item in dt1.Rows)
                            {
                                sql += $@"insert into bdm_eq_info_eq_type_m(eq_info_no,eq_no,item_code,item_name,eq_type,createby,createdate,createtime) values('{eq_info_no}','{dt.Rows[0]["eq_no"]}','{item["ITEM_CODE"]}','{item["ITEM_NAME"]}','0','{Usercode}','{date}','{time}');";
                            }
                            if (!string.IsNullOrWhiteSpace(sql))
                            {
                                DB.ExecuteNonQueryOffline($@"begin {sql} end;");
                            }

                        }
                     
                    }
                    //添加完初始化数据就返回数据
                    sql = $@"
select 
    a.id,
    a.item_code,--项目编号
    a.item_name,--项目名称
    b.CALIBRATION_STANDARD remark, --标准说明
    a.maintain,--检验结果
    case
        when a.maintain='0' then 'FAIL'
        when a.maintain='1' then 'PASS'
    end as  maintain2 --检验结果
from bdm_eq_info_eq_type_m a
LEFT JOIN bdm_correction_item_m b on b.CORRECTION_ITEM_CODE = a.ITEM_CODE 
where a.EQ_INFO_NO='{eq_info_no}' and a.EQ_NO='{dt.Rows[0]["eq_no"]}' 
order by a.id desc";
                    DataTable data = DB.GetDataTable(sql);
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("eq_info_no", dt.Rows[0]["eq_info_no"]);
                    dic.Add("eq_info_name", dt.Rows[0]["eq_info_name"]);
                    dic.Add("data", data);
                    ret.RetData1 = dic;

                }
                else
                {
                    ret.ErrMsg = $@"Basic maintenance without tool barcode[{eq_info_no}]";
                    ret.IsSuccess = false;
                    return ret;
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
        /// <summary>
        /// 保存明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject Commitdatalv(object OBJ)
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
                NullKey(jarr);
                List<data> datalist = jarr.ContainsKey("data") ? JsonConvert.DeserializeObject<List<data>>(jarr["data"].ToString()) : null;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string Usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = $@"select 
id,
eq_info_no,--设备信息编号
eq_no,--设备类型编号
item_code,--项目编号
item_name,--项目名称
eq_type,--校正标准
maintain, --检验结果
remark --备注
 from bdm_eq_info_eq_type_m";
                DataTable dt = DB.GetDataTable(sql);



                DataRow[] dr = null;
                if (datalist.Count > 0)
                {
                    sql = String.Empty;
                    foreach (data item in datalist)
                    {
                        dr = dt.Select($@"ID='{item.id}'");
                        if (dr.Length > 0)
                        {
                           
                            sql += $@"update bdm_eq_info_eq_type_m set maintain='{item.maintain}' where id='{item.id}';";
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        DB.ExecuteNonQuery($@"begin {sql} end;");
                    }

                    sql = $@"update bdm_eq_info_m set jz_date='{DateTime.Now.ToString("yyyy-MM-dd")}' where eq_info_no='{dr[0]["eq_info_no"]}' and eq_no='{dr[0]["eq_no"]}'";
                    DB.ExecuteNonQuery(sql);
                }

                DB.Commit();
                ret.ErrMsg = "Saved successfully！";
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
        public class data
        {
            public int id { get; set; }
            public string item_code { get; set; }
            public string item_name { get; set; }
            public string remark { get; set; }
            public string maintain { get; set; }
            public string maintain2 { get; set; }
        }
    }
}
