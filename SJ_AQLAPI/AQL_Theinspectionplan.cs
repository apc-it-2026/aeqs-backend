using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    public class AQL_Theinspectionplan
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
                string plan_date = jarr.ContainsKey("plan_date") ? jarr["plan_date"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(plan_date))
                {
                    where = $@" and plan_date like '%{plan_date}%'";
                }

                string sql = $@"select plan_date,level_type,id from aql_inspection_plan_m where 1=1 {where} order by id desc";

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
        /// 计划日期明细数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_MainMin(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string gongchang = jarr.ContainsKey("gongchang") ? jarr["gongchang"].ToString() : "";
                string zubie = jarr.ContainsKey("zubie") ? jarr["zubie"].ToString() : "";

                string havingSql = " HAVING 1=1 ";
                if (!string.IsNullOrEmpty(gongchang))
                {
                    havingSql += $@" and  listagg(distinct mfl.ORG_ID,',') LIKE '%{gongchang}%' ";
                }
                if (!string.IsNullOrEmpty(zubie))
                {
                    havingSql += $@" and listagg(distinct mfl.from_line,',') LIKE '%{zubie}%' ";
                }


                string sql = $@"
SELECT
  MAX(a.id) AS id,
	listagg(distinct mfl.from_line,',') as groupo,--组别
	MAX(l.name_t) name_t, --鞋型
	M.se_id,--制令号
	m.mer_po as po,--po号
	MAX(r.prod_no) prod_no,--art
	MAX(m.Shipcountry_En) DESCOUNTRY_NAME,
	MAX(DESCOUNTRY_NAME) as 国家,
	MAX(e.se_qty) as  qty,
	'' AQL数量,
	'' 是否外观标准,
	MAX(a.is_disclaimer) is_disclaimer,--是否负责说明
	(select max(distinct a.shelf_no) from wms_stoc_location a where a.batch_no = m.se_id) as storage_area,
	--listagg(distinct mfl.ORG_ID,',') as 工厂代号,
 	listagg(distinct b.ORG_NAME,',') as 工厂
FROM
	aql_inspection_plan_d a 
LEFT JOIN BDM_SE_ORDER_MASTER m on a.po=m.mer_po
LEFT JOIN BDM_SE_ORDER_ITEM E ON M .SE_ID = E .SE_ID
LEFT JOIN bdm_rd_prod r ON E .prod_no = r.PROD_NO
LEFT JOIN BDM_RD_STYLE l ON r.SHOE_NO = l.SHOE_NO  
LEFT JOIN mms_finishedtrackin_list mfl ON mfl.PO = M.MER_PO
LEFT JOIN BASE001M b ON b.ORG_CODE = mfl.ORG_ID  
where a.union_id='{id}' 
GROUP BY m.mer_po,m.SE_ID,m.ORG_ID 
{havingSql}
order by MAX(a.id) asc";
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
        /// 弹出框数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Outdata(object OBJ)
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
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
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
        /// 保存数据
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
                string plan_date = jarr.ContainsKey("plan_date") ? jarr["plan_date"].ToString() : "";//计划时间
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//表头id
                string data = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                DataTable dt = JsonConvert.DeserializeObject<DataTable>(data);
                string sql = string.Empty;
                string m_id = string.Empty;
                if (!string.IsNullOrWhiteSpace(id))
                {
                    //存在id看是否存在要修改的时间
                    m_id = id;
                    sql = $"select count(1) from aql_inspection_plan_m where plan_date like '%{plan_date}%' and id!='{id}'";
                    if (DB.GetInt32(sql) > 0)
                    {
                        ret.ErrMsg = "There is already a plan for this time period, please re-select the plan time";//已存在该时间段计划,请重新选择计划时间
                        ret.IsSuccess = false;
                        return ret;
                    }
                    else
                    {
                        sql = $@"update aql_inspection_plan_m set plan_date='{plan_date}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where id='{id}'";
                        DB.ExecuteNonQuery(sql);
                    }
                }
                else
                {
                    sql = $"select count(1) from aql_inspection_plan_m where plan_date like '%{plan_date}%'";
                    if (DB.GetInt32(sql) > 0)
                    {
                        ret.ErrMsg = "There is already a plan for this time period, please re-select the plan time";
                        ret.IsSuccess = false;
                        return ret;
                    }
                    sql = $@"insert into aql_inspection_plan_m(plan_date,hori_type,level_type,createby,createdate,createtime)values('{plan_date}','','','{userCode}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    m_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "aql_inspection_plan_m");
                  
                }
               
                if (dt.Rows.Count > 0)
                {
                    sql = string.Empty;
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(dt.Rows[i]["d_id"].ToString()))
                        {
                            sql += $@"update  aql_inspection_plan_d set po='{dt.Rows[i]["po"]}',groupo='{dt.Rows[i]["group"]}',storage_area='{dt.Rows[i]["storage_area"]}',is_disclaimer='{dt.Rows[i]["is_disclaimer"]}',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where id='{dt.Rows[i]["d_id"]}' ;";
                        }
                        else
                        {
                            sql += $@"insert into aql_inspection_plan_d(union_id,po,groupo,storage_area,is_disclaimer,createby,createdate,createtime) values('{m_id}','{dt.Rows[i]["po"]}','{dt.Rows[i]["group"]}','{dt.Rows[i]["storage_area"]}','{dt.Rows[i]["is_disclaimer"]}','{userCode}','{date}','{time}');";
                        }
                       
                    }
                    DB.ExecuteNonQuery($@"begin {sql} end;");
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
        /// 删除数据
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

                string sql = $@"delete from aql_inspection_plan_d where id='{id}'";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                ret.ErrMsg = "删除成功";
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
        /// AQL级别样品数量
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public  SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQL_Level(object OBJ)
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
                NullKey(jarr);
                string level_type = jarr.ContainsKey("level_type") ? jarr["level_type"].ToString() : "";
                string sql = $@"SELECT
    LEVEL_TYPE,
	START_QTY,--起始数量
	END_QTY,--END_QTY
	VALS --样本量
FROM
	BDM_AQL_M
WHERE
	HORI_TYPE = '1' AND LEVEL_TYPE='{level_type}'";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Outdata_New(object OBJ)
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
                string sql = jarr.ContainsKey("sql") ? jarr["sql"].ToString() : "";
                //string sql2 = jarr.ContainsKey("sql2") ? jarr["sql2"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";


                DataTable dt = DB.GetDataTable(sql);

//                string zb_sql = $@"
//SELECT
//	f.se_id as key,
//	f.from_line AS value
//FROM
//	mms_finishedtrackin_list f
//WHERE
//	f.SE_ID IN ({sql2})
//GROUP BY
//	f.se_id,
//	f.from_line
//";
//                DataTable zb_dt = DB.GetDataTable(zb_sql);
//                var zb_list = zb_dt.ToDataList<DtKey>();
//                var zb_list_group = zb_list.GroupBy(x => x.KEY).Select(x => new DtKey
//                {
//                    KEY = x.Key,
//                    VALUE = string.Join(",", x.Select(y => y.VALUE).Distinct().ToList())
//                });

//                string cfwz_sql = $@"
//SELECT
//	A .batch_no as key,
//	A .shelf_no as value
//FROM
//	wms_stoc_location A
//WHERE
//	A .batch_no IN (
//		SELECT DISTINCT
//			M .se_id
//		FROM
//			BDM_SE_ORDER_MASTER M
//	)
//GROUP BY
//	A .batch_no,
//	A .shelf_no
//";
//                DataTable cfwz_dt = DB.GetDataTable(cfwz_sql);
//                var fwz_list = cfwz_dt.ToDataList<DtKey>();
//                var fwz_list_group = fwz_list.GroupBy(x => x.KEY).Select(x => new DtKey
//                {
//                    KEY = x.Key,
//                    VALUE = string.Join(",", x.Select(y => y.VALUE).Distinct().ToList())
//                });

//                foreach (DataRow item in dt.Rows)
//                {
//                    string se_id = item["制令号"].ToString();
//                    var find_zb = zb_list_group.FirstOrDefault(x => x.KEY == se_id);
//                    if (find_zb != null)
//                        item["组别"] = find_zb.VALUE;
//                    var find_cfwz = fwz_list_group.FirstOrDefault(x => x.KEY == se_id);
//                    if (find_cfwz != null)
//                        item["存放位置"] = find_cfwz.VALUE;
//                }

                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
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



    }

    public class DtKey
    {
        public string KEY { get; set; }
        public string VALUE { get; set; }
    }

}
