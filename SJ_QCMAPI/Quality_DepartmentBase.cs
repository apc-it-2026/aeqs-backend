using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class Quality_DepartmentBase
    {
        /// <summary>
        /// 查询部门产线接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQuality_DepartmentList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string department_no = jarr.ContainsKey("department_no") ? jarr["department_no"].ToString() : "";
                string department_name = jarr.ContainsKey("department_name") ? jarr["department_name"].ToString() : "";
                string productionline_no = jarr.ContainsKey("productionline_no") ? jarr["productionline_no"].ToString() : "";
                string productionline_name = jarr.ContainsKey("productionline_name") ? jarr["productionline_name"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(department_no))
                {
                    whereSql += $@"and m.department_no like'%" +department_no+"%'";
                }
                if (!string.IsNullOrEmpty(department_name))
                {
                    whereSql += $@"and m.department_name like'%" + department_name + "%'";
                }
                if (!string.IsNullOrEmpty(productionline_no))
                {
                    whereSql += $@"and d.productionline_no like'%" + productionline_no + "%'";
                }
                if (!string.IsNullOrEmpty(productionline_name))
                {
                    whereSql += $@"and d.productionline_name like'%" + productionline_name + "%'";
                }


                sql = $@"SELECT
	d.department_no,
	m.department_name,
	d.productionline_no,
	d.productionline_name 
FROM
	BDM_QUALITY_DEPARTMENT_M m
	JOIN BDM_QUALITY_DEPARTMENT_D d ON d.DEPARTMENT_NO = m.DEPARTMENT_NO 
WHERE
	1 = 1 { whereSql}";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 不良问题点newAdd
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ProductionlineDefectsM_NewAdd(object OBJ)
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

                string department_no = jarr.ContainsKey("department_no") ? jarr["department_no"].ToString() : "";
                string department_name = jarr.ContainsKey("department_name") ? jarr["department_name"].ToString() : "";
                string productionline_no = jarr.ContainsKey("productionline_no") ? jarr["productionline_no"].ToString() : "";
                string productionline_name = jarr.ContainsKey("productionline_name") ? jarr["productionline_name"].ToString() : "";

                string defect_no = jarr.ContainsKey("defect_no") ? jarr["defect_no"].ToString() : "";//不良问题代号
                string defect_name = jarr.ContainsKey("defect_name") ? jarr["defect_name"].ToString() : "";//不良问题

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                var sql = string.Empty;

                string sql1 = $@"SELECT
	* 
FROM
	bdm_productionline_defects_m 
WHERE
	defect_no = '{defect_no}'";
                DataTable dd = DB.GetDataTable(sql1);
                if (dd.Rows.Count > 0)
                {
                    throw new Exception($"不良问题代号【{defect_no}】已存在，请重新输入!");
                }

                sql = $@"INSERT INTO bdm_productionline_defects_m ( department_no, department_name, productionline_no, productionline_name, defect_no, defect_name, createby, createdate, createtime )
VALUES
	( '{department_no}', '{department_name}', '{productionline_no}', '{productionline_name}', '{defect_no}', '{defect_name}', '{CreactUserId}', '{DateTime.Now.ToString("yyyy-MM-dd")}', '{DateTime.Now.ToString("HH:mm:ss")}' )";


                DB.ExecuteNonQuery(sql);
                    DB.Commit();
                    ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
        /// 不良问题点update\delete
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ProductionlineDefectsM_Operation(object OBJ)
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

                string defect_no = jarr.ContainsKey("defect_no") ? jarr["defect_no"].ToString() : "";//不良问题代号
                string defect_name= jarr.ContainsKey("defect_name") ? jarr["defect_name"].ToString() : "";//不良问题
                string Operation=jarr.ContainsKey("Operation") ? jarr["Operation"].ToString() : "";

                string sql = string.Empty;
                if (Operation=="Delete")
                {
                    sql = $@"delete from bdm_productionline_defects_m where defect_no='{defect_no}'";
                }
                if (Operation=="Modify")
                {
                    sql = $@"update bdm_productionline_defects_m set defect_name='{defect_name}' where defect_no='{defect_no}'";
                }

                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
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
        /// 查询新增不良问题点接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQuality_DepartmentNewAdd(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string department_no = jarr.ContainsKey("department_no") ? jarr["department_no"].ToString() : "";
                string department_name = jarr.ContainsKey("department_name") ? jarr["department_name"].ToString() : "";
                string productionline_no = jarr.ContainsKey("productionline_no") ? jarr["productionline_no"].ToString() : "";
                string productionline_name = jarr.ContainsKey("productionline_name") ? jarr["productionline_name"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string whereSql = string.Empty;

                if (!string.IsNullOrEmpty(department_no))
                {
                    whereSql += $@"and department_no='" + department_no + "'";
                }
                if (!string.IsNullOrEmpty(department_name))
                {
                    whereSql += $@"and department_name='" + department_name + "'";
                }
                if (!string.IsNullOrEmpty(productionline_no))
                {
                    whereSql += $@"and productionline_no='" + productionline_no + "'";
                }
                if (!string.IsNullOrEmpty(productionline_name))
                {
                    whereSql += $@"and productionline_name='" + productionline_name + "'";
                }

                string sql1 = $@"SELECT
	department_no,
	department_name,
	productionline_no,
	productionline_name,
	defect_no,
	defect_name 
FROM
	BDM_PRODUCTIONLINE_DEFECTS_M where 1=1 {whereSql}";

                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql1, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql1);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
