using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SJ_QCMAPI
{
    public class BASE
    {

        /// <summary>
        /// 查询系统枚举
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYS001MDataList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string enum_type = jarr.ContainsKey("enum_type") ? jarr["enum_type"].ToString() : "";
                string where = jarr.ContainsKey("where") ? jarr["where"].ToString() : "";
                if (string.IsNullOrEmpty(enum_type))
                    throw new Exception("接口参数【enum_type】不能为空，请检查！");

                string sql = $@"select enum_code,enum_value from sys001m where enum_type ='{enum_type}' {where} ";
                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 查询多个系统枚举
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYS001MDataListS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(Data);

                if (jarr.Count == 0)
                    throw new Exception("接口参数不能为空，请检查！");

                Dictionary<string, object> dic = new Dictionary<string, object>();

                foreach (var item in jarr)
                {
                    string sql = $@"select enum_code,enum_value from sys001m where enum_type ='{item}' ";
                    DataTable dt = DB.GetDataTable(sql);
                    dic.Add(item, dt);
                }


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
        /// 文件查看视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileView(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string tablename = jarr.ContainsKey("tablename") ? jarr["tablename"].ToString() : "";//存储文件表名
                string parms = jarr.ContainsKey("parms") ? jarr["parms"].ToString() : "";//过滤参数
                string fileds = jarr.ContainsKey("fileds") ? jarr["fileds"].ToString() : "";//查询字段
                Dictionary<string, object> parmsDic = null;
                if (!string.IsNullOrEmpty(parms))
                {
                    parmsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(parms);
                }
                Dictionary<string, object> filedsDic = null;
                if (!string.IsNullOrEmpty(fileds))
                {
                    filedsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(fileds);
                }
                //string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋号
                string sqlfiled = "";
                foreach (var item in filedsDic)
                {
                    sqlfiled += item.Key + " as " + item.Value + ",";
                }
                string sqlparms = "";
                foreach (var item in parmsDic)
                {
                    sqlparms += $" and {item.Key}='{item.Value}'";
                }
                string sql = $@"select id,{sqlfiled.Trim(',')},'{tablename}' as tablename from {tablename} where 1=1 {sqlparms}";

                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("Data1", dt2);
                //dic.Add("Data2", dt3);
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
        /// 文件查看视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteFile(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string tablename = jarr.ContainsKey("tablename") ? jarr["tablename"].ToString() : "";//存储文件表名
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//过滤参数
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";//新删除参数
                string delWhereKey = jarr.ContainsKey("del_key") ? jarr["del_key"].ToString() : "";//新删除参数
                string file_url = jarr.ContainsKey("file_url") ? jarr["file_url"].ToString() : "";//过滤参数


                string Path = $"{Directory.GetCurrentDirectory()}/wwwroot/{file_url}";
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
                if (!string.IsNullOrEmpty(guid))
                {
                    DB.ExecuteNonQueryOffline($"delete {tablename} where {delWhereKey}='{id}'");
                    DB.ExecuteNonQueryOffline($"delete BDM_UPLOAD_FILE_ITEM where GUID='{guid}'");
                }
                else
                    DB.ExecuteNonQueryOffline($"delete {tablename} where id={id}");
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
        /// 获取厂商下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSupplier(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keywork = jarr.ContainsKey("keywork") ? jarr["keywork"].ToString() : "";//关键字
                DataTable dt = DB.GetDataTable($"SELECT SUPPLIERS_CODE,SUPPLIERS_NAME FROM BASE003M WHERE 1=1 {(String.IsNullOrEmpty(keywork) ? "" : $" AND (SUPPLIERS_CODE LIKE '%{keywork}%' OR SUPPLIERS_NAME LIKE '%{keywork}%')")}");
                ret.RetData1 = dt;
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
        /// 导入模板数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ImportData(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SOURCE = jarr.ContainsKey("SOURCE") ? jarr["SOURCE"].ToString() : "";//关键字
                int import_type = jarr.ContainsKey("import_type") ? int.Parse(jarr["import_type"].ToString()) : -1;//关键字
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(SOURCE);
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                switch (import_type)
                {
                    case (int)enum_import_type.enum_import_type_1:
                        //dosomething
                        ImportExecl.Import_batch_production(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_2:
                        //客户投诉
                        //客户投诉导入条件
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //int count = DB.GetInt32($@"select count(1) from BDM_SE_ORDER_MASTER where mer_po='{dt.Rows[i]["投诉PO号"]}'");
                            //if (count <= 0)
                            //{
                            //    ret.IsSuccess = false;
                            //    ret.ErrMsg = $@"未找到PO数据!第{i + 1}行";
                            //    return ret;
                            //}
                           // int countno = DB.GetInt32($@"select count(1) from QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO='{dt.Rows[i]["投诉编号"]}'");
                            int countno = DB.GetInt32($@"select count(1) from QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO='{dt.Rows[i]["Complaint No"]}'");
                            if (countno > 0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"The complaint number cannot be repeated! No.{i + 1}行";
                                return ret;
                            }
                        }
                        ImportExecl.Import_CustomerEx(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_3:
                        //首件确认
                        ImportExecl.Input_REINSPECTION(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_4:
                        //发外厂商品质体系项目日志(重检报告)导入模板
                        ImportExecl.Input_REINSPECTION_REPORT_M(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_5:
                        //发外厂商色卡导入模板
                        ImportExecl.Import_ExternalColorCard(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_6:
                        //抽检监督导入模板
                        ImportExecl.Import_Inspection_Supervision_report(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_7:
                        //不良退货导入模板
                        ImportExecl.Import_BadReturn_Report(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_8:
                        //中国区域客户退货数据导入模板
                        bool flag = false;
                        ImportExecl.Import_Marketfeedback(DB, dt, usercode, (k) => { flag = k; });
                        if (flag)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "There are po items without basic information maintenance in the imported data, and the import fails";
                            return ret;
                        }
                        break;
                    case (int)enum_import_type.enum_import_type_9:
                        //确认鞋库位维护导入
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["参照标准"].ToString() == "入库时间")
                            {
                                dt.Rows[i]["参照标准"] = "0";
                            }
                            else if (dt.Rows[i]["参照标准"].ToString() == "量产时间")
                            {
                                dt.Rows[i]["参照标准"] = "1";
                            }

                            if (dt.Rows[i]["参照标准"].ToString() == "0" || dt.Rows[i]["参照标准"].ToString() == "1"
                                    || dt.Rows[i]["参照标准"].ToString() == "入库时间" || dt.Rows[i]["参照标准"].ToString() == "量产时间")
                            {

                            }
                            else
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"Incorrect reference standard! Line {i + 1}";
                                return ret;
                            }

                            int count = DB.GetInt32($@"select count(1) from qcm_confirm_shoes_arc where STOCK_CODE='{dt.Rows[i]["库位代号"]}'");
                            if (count > 0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"Location code cannot be repeated! Line {i + 1}!";
                                return ret;
                            }

                            int countck= DB.GetInt32($@"select count(1) from qcm_confirm_shoes_wh where WAREHOUSE_CODE='{dt.Rows[i]["仓库代号"]}'");
                            if (countck<=0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"Warehouse code does not exist! Line {i + 1}!";
                                return ret;
                            }
                        }
                        DataView dv = new DataView(dt);
                        if (dv.Count != dv.ToTable(true, "库位代号").Rows.Count)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "There is duplicate data in the location code, please check!";
                            return ret;
                        }
                        ImportExecl.Import_ShoesArc(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_10:
                        //确认鞋仓库维护导入
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int count = DB.GetInt32($@"select count(1) from qcm_confirm_shoes_wh where WAREHOUSE_CODE='{dt.Rows[i]["仓库代号"]}'");
                            if (count>0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "Warehouse number cannot be repeated!";
                                return ret;
                            }
                        }
                        DataView dv1 = new DataView(dt);
                        if (dv1.Count != dv1.ToTable(true, "仓库代号").Rows.Count)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "Warehouse code has duplicate data, please check!";
                            return ret;
                        }
                        ImportExecl.Import_ShoesWh(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_11:
                        //出货仓库维护导入
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_wh where MODULE_TYPE = '{dt.Rows[0]["MODULE_TYPE"].ToString()}' and WAREHOUSE_CODE='{dt.Rows[i]["仓库代号"]}'");
                            if (count > 0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = "Warehouse number cannot be repeated!";
                                return ret;
                            }
                        }
                        DataView dv2 = new DataView(dt);
                        if (dv2.Count != dv2.ToTable(true, "仓库代号").Rows.Count)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "The warehouse code has duplicate data, please check!";
                            return ret;
                        }
                        ImportExecl.Import_ShoesWh_AQL(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_12:
                        //出货库位维护导入
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            int count = DB.GetInt32($@"select count(1) from aql_confirm_shoes_arc where MODULE_TYPE = '{dt.Rows[0]["MODULE_TYPE"].ToString()}' and STOCK_CODE='{dt.Rows[i]["库位代号"]}'");
                            if (count > 0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"Location code cannot be repeated! Line {i + 1}!";
                                return ret;
                            }

                            int countck = DB.GetInt32($@"select count(1) from aql_confirm_shoes_wh where MODULE_TYPE = '{dt.Rows[0]["MODULE_TYPE"].ToString()}' and WAREHOUSE_CODE='{dt.Rows[i]["仓库代号"]}'");
                            if (countck <= 0)
                            {
                                ret.IsSuccess = false;
                                ret.ErrMsg = $@"Warehouse code does not exist! Line {i + 1}!";
                                return ret;
                            }
                        }
                        DataView dv3 = new DataView(dt);
                        if (dv3.Count != dv3.ToTable(true, "库位代号").Rows.Count)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "There is duplicate data in the location code, please check!";
                            return ret;
                        }
                        ImportExecl.Import_ShoesArc_AQL(DB, dt, usercode);
                        break;
                    case (int)enum_import_type.enum_import_type_13:
                        //客户退货导入模板
                        bool flag2=false;
                        ImportExecl.Import_Marketfeedback2(DB, dt, usercode, (k) =>{ flag2 = k;});
                        if (flag2)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "There are po items without basic information maintenance in the imported data, and the import fails";
                            return ret;
                        }
                        break;
                    case (int)enum_import_type.enum_import_type_14:
                        //客户退货导入模板
                        ImportExecl.Import_CustomerReturn(DB, dt, usercode);

                        break;
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
        /// 获取ART下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetART(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keywork = jarr.ContainsKey("keywork") ? jarr["keywork"].ToString() : "";//关键字

                string whereSql = $@" WHERE 1 = 1";
                if (!string.IsNullOrEmpty(keywork))
                {
                    whereSql += $@" AND d.PROD_NO LIKE '%{keywork}%'";
                }
                string sql = $@"
SELECT
	d.PROD_NO,
	m.IMG_URL,
    d.SHOE_NO
FROM
	BDM_RD_PROD d 
LEFT JOIN BDM_PROD_M m ON m.PROD_NO=d.PROD_NO
";
                DataTable dt = DB.GetDataTable(sql + whereSql);
                ret.RetData1 = dt;
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
        /// 获取厂区下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrg(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keywork = jarr.ContainsKey("keywork") ? jarr["keywork"].ToString() : "";//关键字

                string whereSql = $@" WHERE 1 = 1";
                if (!string.IsNullOrEmpty(keywork))
                {
                    whereSql += $@" and (ORG_CODE LIKE '%{keywork}%' OR ORG_NAME LIKE '%{keywork}%')";
                }
                string sql = $@"
SELECT
	ORG_CODE,
	ORG_NAME
FROM
	BASE001M
";
                DataTable dt = DB.GetDataTable(sql + whereSql);
                ret.RetData1 = dt;
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
        /// 获取品管部门下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDepartment(object OBJ)
        { 
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keywork = jarr.ContainsKey("keywork") ? jarr["keywork"].ToString() : "";//关键字

                string whereSql = $@" WHERE 1 = 1";
                if (!string.IsNullOrEmpty(keywork))
                {
                    whereSql += $@" AND (DEPARTMENT_NO LIKE '%{keywork}%' OR DEPARTMENT_NAME LIKE '%{keywork}%')";
                }
                string sql = $@"
SELECT
	DEPARTMENT_NO,
	DEPARTMENT_NAME
FROM
	BDM_QUALITY_DEPARTMENT_M
";
                DataTable dt = DB.GetDataTable(sql + whereSql);
                ret.RetData1 = dt;
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
        /// 根据品管部门获取生产线下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductionLineByDepartment(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string department = jarr.ContainsKey("department") ? jarr["department"].ToString() : "";//部门
                string keywork = jarr.ContainsKey("keywork") ? jarr["keywork"].ToString() : "";//关键字

                string whereSql = $@" WHERE DEPARTMENT_NO = '{department}'";
                if (!string.IsNullOrEmpty(keywork))
                {
                    whereSql += $@"
AND (
	PRODUCTIONLINE_NO LIKE '%{keywork}%'
	OR PRODUCTIONLINE_NAME LIKE '%{keywork}%'
)
";
                }
                string sql = $@"
SELECT
	PRODUCTIONLINE_NO,
	PRODUCTIONLINE_NAME
FROM
	BDM_QUALITY_DEPARTMENT_D
";
                DataTable dt = DB.GetDataTable(sql + whereSql);
                ret.RetData1 = dt;
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
        /// 根据PDA返回语言类型获取对应语言字段数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPdaMultilingual(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string sql = $@"SELECT ui_id,ui_cn,ui_en,ui_yn
                            FROM SJQDMS_UILAN(nolock) WHERE ui_tittle = 'PDA'";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                Dictionary<string, object> dic_cn = new Dictionary<string, object>();
                Dictionary<string, object> dic_en = new Dictionary<string, object>();
                Dictionary<string, object> dic_yn = new Dictionary<string, object>();

                DataTable dt = DB.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dic_cn.Add(dr["ui_id"].ToString(), dr["ui_cn"].ToString());
                        dic_en.Add(dr["ui_id"].ToString(), string.IsNullOrEmpty(dr["ui_en"].ToString()) ? dr["ui_cn"].ToString() : dr["ui_en"].ToString());
                        dic_yn.Add(dr["ui_id"].ToString(), string.IsNullOrEmpty(dr["ui_yn"].ToString()) ? dr["ui_cn"].ToString() : dr["ui_yn"].ToString());
                    }
                }

                dic.Add("cn", dic_cn);
                dic.Add("en", dic_en);
                dic.Add("yn", dic_yn);


                ret.IsSuccess = true;
                ret.RetData1 = dic;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }


        /// <summary>
        /// 查询设备信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEquipmentData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                string Data = ReqObj.Data.ToString();
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyWord = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("page") ? jarr["page"].ToString() : "1";

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string sql = @"select eq_info_no,eq_info_name from bdm_eq_info_m";

                if (!string.IsNullOrEmpty(keyWord))
                {
                    sql += " where eq_info_no like @eq_info_no or eq_info_name like @eq_info_name";
                    Parameters.Add("eq_info_no", "%" + keyWord + "%");
                    Parameters.Add("eq_info_name", "%" + keyWord + "%");
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", Parameters);

                ret.IsSuccess = true;
                ret.RetData1 = dt;

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
