using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SJ_SYSAPI
{
    public class Logic
    {
        #region 新增表头数据时，根据模块功能自动更新表身数据
        /// <summary>
        /// 新增表头数据时，根据模块功能自动更新表身数据
        /// </summary>
        /// <param name="App_Code"></param>
        /// <param name="Id"></param>
        public static void AddModuleOtherData(string App_Code, int Id, string token)
        {
            try
            {
                switch (App_Code)
                {
                    ///生产工单
                    case "PC_MES010":
                        AddDataMES010(Id, token);
                        break;

                    ///物料资料
                    case "PC_BASE007":
                        AddDataBASE007(Id, token);
                        break;



                    //添加角色
                    case "PC_User":
                        AddDataUser(Id, token);
                        break;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// 添加用户时添加权限信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        private static void AddDataUser(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);
                SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                Dictionary<string, object> p = DB.GetDictionary(@"select user_code,roles from userinfo where id=" + id);

                DB.ExecuteNonQueryOffline(@"update userinfo set pwd='E10ADC3949BA59ABBE56E057F20F883E' where id=" + id);

                DB.ExecuteNonQueryOffline(@"delete from userpower where UserCode=@user_code", p);

                DataTable dt = DB.GetDataTable(@"select * from rolepower where RoleName=@roles ", p);

                string sql = string.Empty;
                foreach (DataRow dr in dt.Rows)
                {
                    Dictionary<string, object> pRow = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByDataRow(dr);
                    pRow.Add("UserCode", p["user_code"].ToString());
                    pRow.Remove("RoleName");
                    pRow.Remove("id");

                    sql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "userpower", pRow) + ";";

                    DB.ExecuteNonQueryOffline(sql, pRow);

                }



            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



        /// <summary>
        /// 添加物料资料模块的表身数据
        /// </summary>
        /// <param name="id"></param>
        private static void AddDataBASE007(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string material_no = string.Empty;
                string material_name = string.Empty;
                string material_ver = string.Empty;
                string material_specifications = string.Empty;
                string process_no = string.Empty;

                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                IDataReader idr = DB.GetDataTableReader(@"
SELECT 
material_no,
material_name,
material_ver,
material_specifications,
process_no
from base007m where id =" + id + @"
");
                if (idr.Read())
                {

                    material_no = idr["material_no"].ToString();
                    material_name = idr["material_name"].ToString();
                    material_ver = idr["material_ver"].ToString();
                    material_specifications = idr["material_specifications"].ToString();

                    process_no = idr["process_no"].ToString();
                }


                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("material_no", material_no);
                p.Add("material_name", material_name);
                p.Add("material_ver", material_ver);
                p.Add("material_specifications", material_specifications);

                p.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(token));
                p.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                p.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));



                #region 插入工单工艺

                //工艺不为空
                if (!string.IsNullOrEmpty(process_no))
                {


                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND process_no='" + process_no + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes002a1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "base007d1", p, procedureDT));

                }


                #endregion


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加生产工单模块的表身数据
        /// </summary>
        /// <param name="id"></param>
        private static void AddDataMES010(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

                string production_order = string.Empty;
                string material_no = string.Empty;
                string process = string.Empty;
                string qty = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                IDataReader idr = DB.GetDataTableReader(@"
SELECT 
production_order,
material_no,
qty,
process
from mes010m where id =" + id + @"
");
                if (idr.Read())
                {
                    production_order = idr["production_order"].ToString();
                    material_no = idr["material_no"].ToString();
                    process = idr["process"].ToString();
                    qty = idr["qty"].ToString();
                }


                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("production_order", production_order);
                p.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(token));
                p.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                p.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));

                #region 根据品号更新品名、规格


                string base007Keys = "material_no,material_name,material_specifications";

                Dictionary<string, object> base007P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(base007Keys);

                string base007Where = " AND material_no='" + material_no + @"' ";

                base007P = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m", base007P, base007Where));
                if (base007P.Count > 0)
                {
                    base007P.Add("production_order", production_order);
                    DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("mes010m", " production_order=@production_order", base007P), base007P);
                }
                #endregion

                #region 插入BOM信息


                string bomKeys = "material_no,material_name,bom_ver,material_specifications,serial_number," +
                    "material_no2,material_name2,material_specifications2,unit,process_no,process_name,procedure_no,procedure_name," +
                    "procedure_description,qty_base,qty,wastage_rate";

                Dictionary<string, object> bomP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(bomKeys);

                string bomWhere = " AND material_no='" + material_no + @"' ";

                DataTable bomDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes020a1", bomP, bomWhere));
                bomDT.Columns.Add("qty2");
                if (bomDT.Rows.Count > 0)
                {

                    foreach (DataRow dr in bomDT.Rows)
                    {
                        if (dr["qty_base"].ToString() != null)
                        {
                            dr["qty2"] = Convert.ToDecimal(dr["qty"].ToString()) / Convert.ToDecimal(dr["qty_base"].ToString())
                           * Convert.ToDecimal(qty) * (1 + (Convert.ToDecimal(dr["wastage_rate"].ToString()) / 100));
                        }

                    }

                    DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a4", p, bomDT));
                }
                #endregion

                #region 插入工单工艺

                //工艺不为空
                if (!string.IsNullOrEmpty(process))
                {


                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND process_no='" + process + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes002a1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a1", p, procedureDT));

                }
                //工艺为空
                else
                {
                    DB.ExecuteNonQueryOffline(@"DELETE FROM mes010a1 where production_order='" + production_order + @"'
");

                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND material_no='" + material_no + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007d1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a1", p, procedureDT));
                }

                #endregion


            }
            catch (Exception ex)
            {

            }
        }


        #endregion


        #region 编辑表头数据时，根据模块功能自动更新表身数据
        /// <summary>
        /// 编辑表头数据时，根据模块功能自动更新表身数据
        /// </summary>
        /// <param name="App_Code"></param>
        /// <param name="Id"></param>
        public static void EditModuleOtherData(string App_Code, int Id, Dictionary<string, object> OldData, string token)
        {
            try
            {
                switch (App_Code)
                {
                    ///生产工单
                    case "PC_MES010":
                        EditDataMES010(Id, OldData, token);
                        break;
                    ///生产工单
                    case "PC_BASE007":
                        EditDataBASE007(Id, OldData, token);
                        break;

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 编辑时，修改物料资料模块的表身数据
        /// </summary>
        /// <param name="id"></param>
        private static void EditDataBASE007(int id, Dictionary<string, object> OldData, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

                string material_no = string.Empty;
                string material_name = string.Empty;
                string material_ver = string.Empty;
                string material_specifications = string.Empty;
                string process_no = string.Empty;

                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                IDataReader idr = DB.GetDataTableReader(@"
SELECT 
material_no,
material_name,
material_ver,
material_specifications,
process_no
from base007m where id =" + id + @"
");
                if (idr.Read())
                {

                    material_no = idr["material_no"].ToString();
                    material_name = idr["material_name"].ToString();
                    material_ver = idr["material_ver"].ToString();
                    material_specifications = idr["material_specifications"].ToString();

                    process_no = idr["process_no"].ToString();
                }


                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("material_no", material_no);
                p.Add("material_name", material_name);
                p.Add("material_ver", material_ver);
                p.Add("material_specifications", material_specifications);

                p.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(token));
                p.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                p.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));



                #region 插入工单工艺
                //工艺不为空，而且工艺修改了
                if (!string.IsNullOrEmpty(process_no) && OldData["process_no"].ToString() != process_no)
                {
                    DB.ExecuteNonQueryOffline(@"DELETE FROM base007d1 where material_no='" + material_no + @"'
AND material_ver ='" + material_ver + @"'
");

                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND process_no='" + process_no + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes002a1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "base007d1", p, procedureDT));

                }

                #endregion


            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 编辑时，修改生产工单模块的表身数据
        /// </summary>
        /// <param name="id"></param>
        private static void EditDataMES010(int id, Dictionary<string, object> OldData, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

                string production_order = string.Empty;
                string material_no = string.Empty;
                string process = string.Empty;
                string qty = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                IDataReader idr = DB.GetDataTableReader(@"
SELECT 
production_order,
material_no,
qty,
process
from mes010m where id =" + id + @"
");
                if (idr.Read())
                {
                    production_order = idr["production_order"].ToString();
                    material_no = idr["material_no"].ToString();
                    process = idr["process"].ToString();
                    qty = idr["qty"].ToString();
                }

                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("production_order", production_order);
                p.Add("createby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(token));
                p.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                p.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));

                #region 根据品号更新品名、规格


                string base007Keys = "material_no,material_name,material_specifications";

                Dictionary<string, object> base007P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(base007Keys);

                string base007Where = " AND material_no='" + material_no + @"' ";

                base007P = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m", base007P, base007Where));
                if (base007P.Count > 0)
                {
                    base007P.Add("production_order", production_order);
                    DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("mes010m", " production_order=@production_order", base007P), base007P);
                }
                #endregion

                #region 插入BOM信息

                if (OldData["material_no"].ToString() != material_no)
                {

                    DB.ExecuteNonQueryOffline(@"DELETE FROM mes010a4 where production_order='" + production_order + @"'
");


                    string bomKeys = "material_no,material_name,bom_ver,material_specifications,serial_number," +
                   "material_no2,material_name2,material_specifications2,process_no,process_name,procedure_no,procedure_name," +
                   "procedure_description,qty_base,qty,wastage_rate";

                    Dictionary<string, object> bomP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(bomKeys);

                    string bomWhere = " AND material_no='" + material_no + @"' ";

                    DataTable bomDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes020a1", bomP, bomWhere));
                    bomDT.Columns.Add("qty2");
                    if (bomDT.Rows.Count > 0)
                    {
                        foreach (DataRow dr in bomDT.Rows)
                        {
                            if (dr["qty_base"].ToString() != null)
                            {
                                dr["qty2"] = Convert.ToDecimal(dr["qty"].ToString()) / Convert.ToDecimal(dr["qty_base"].ToString())
                              * Convert.ToDecimal(qty) * (1 + (Convert.ToDecimal(dr["wastage_rate"].ToString()) / 100));
                            }

                        }

                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a4", p, bomDT));
                    }
                }
                #endregion

                #region 插入工单工艺
                //工艺不为空，而且工艺修改了
                if (!string.IsNullOrEmpty(process) && OldData["process"].ToString() != process)
                {
                    DB.ExecuteNonQueryOffline(@"DELETE FROM mes010a1 where production_order='" + production_order + @"'
");

                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND process_no='" + process + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes002a1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a1", p, procedureDT));

                }
                //工艺为空，物料修改了或者工艺修改了
                else if ((string.IsNullOrEmpty(process) && OldData["material_no"].ToString() != material_no) ||
                    (string.IsNullOrEmpty(process) && OldData["process"].ToString() != process))
                {
                    DB.ExecuteNonQueryOffline(@"DELETE FROM mes010a1 where production_order='" + production_order + @"'
");

                    string procedureKeys = "sorting,procedure_no,procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND material_no='" + material_no + @"' ";

                    DataTable procedureDT = DB.GetDataTable(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007d1", procedureP, procedureWhere));
                    if (procedureDT.Rows.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary(DB.DataBaseType, "mes010a1", p, procedureDT));
                }
                #endregion


            }
            catch (Exception ex)
            {

            }
        }


        #endregion


        #region 单据操作
        /// <summary>
        /// 对单据进行确认操作
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DocDoSure(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string reqKey = "App_Code,Id,DoSure";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqKey, ReqObj);
                reqP.Add("dosureby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                reqP.Add("dosuredatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                if (Convert.ToBoolean(reqP["DoSure"].ToString()))
                {
                    reqP.Add("status", "7");
                }
                else
                {
                    reqP.Add("status", "8");
                }

                string tablename = DBSys.GetString("select App_TableH from SYSAPP01M where APP_Code=@App_Code", reqP);

                DB.ExecuteNonQueryOffline(@"
UPDATE " + DB.ChangeKeyWord(tablename) + @" set
status=@status,
dosureby=@dosureby,
dosuredatetime=@dosuredatetime
where id=@Id
", reqP);
                ret.IsSuccess = true;
                ret.RetData = reqP["status"].ToString();
                if (reqP["status"].ToString() == "7")
                    ret.ErrMsg = "单据取消确认成功!";
                else
                    ret.ErrMsg = "单据确认成功!";


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

        /// <summary>
        /// 对单据进行审核操作
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DocAudit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string reqKey = "App_Code,Id,Audit";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqKey, ReqObj);
                reqP.Add("auditby", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                reqP.Add("auditdatetime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                if (Convert.ToBoolean(reqP["Audit"].ToString()))
                {
                    reqP.Add("status", "2");
                }
                else
                {
                    reqP.Add("status", "7");
                }

                string tablename = DBSys.GetString("select App_TableH from SYSAPP01M where APP_Code=@App_Code", reqP);

                DB.ExecuteNonQueryOffline(@"
UPDATE " + DB.ChangeKeyWord(tablename) + @" set
status=@status,
auditby=@auditby,
auditdatetime=@auditdatetime
where id=@Id
", reqP);
                ret.IsSuccess = true;
                ret.RetData = reqP["status"].ToString();
                if (reqP["status"].ToString() == "1")
                    ret.ErrMsg = "单据取消确认成功!";
                else
                    ret.ErrMsg = "单据确认成功!";


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }


        #endregion


        #region 保存时，自动带出表里其他关联数据
        public static void SetOtherData(string tableName, int id, string userToken)
        {
            try
            {
                switch (tableName.ToLower())
                {
                    ///工艺流程-工序表身
                    case "mes002a1":
                        SetDatames002a1(id, userToken);
                        break;
                    ///工作中心-工站表身
                    case "base016a1":
                        SetDatabase016a1(id, userToken);
                        break;
                    ///BOM-bom表身
                    case "mes020a1":
                        SetDatames020a1(id, userToken);
                        break;
                    ///订单-物料表身
                    case "erp001a1":
                        SetDataerp001a1(id, userToken);
                        break;
                    ///班组-人员表身
                    case "hr003a1":
                        SetDatahr003a1(id, userToken);
                        break;
                    ///工单-工序表身
                    case "mes010a1":
                        SetDatames010a1(id, userToken);
                        break;

                    ///工单-BOM表身
                    case "mes010a4":
                        SetDatames010a4(id, userToken);
                        break;

                    ///检验归类-检验信息
                    case "qa008a1":
                        SetDataqa008a1(id, userToken);
                        break;

                    ///检验归类表头
                    case "qa008m":
                        SetDataqa008m(id, userToken);
                        break;

                    ///BOM资料表头
                    case "mes020m":
                        SetDatames020m(id, userToken);
                        break;

                    ///过站检验单单头
                    case "qa041m":
                        SetDataqa041m(id, userToken);
                        break;


                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void SetDatames010a4(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string qty = DB.GetString(@"
SELECT qty from mes010m
where production_order=
(SELECT production_order FROM mes010a1 WHERE id=" + id + @")
");


                string bomKeys = "qty_base,qty,wastage_rate";

                Dictionary<string, object> bomP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(bomKeys);

                string bomWhere = " AND id='" + id + @"' ";

                bomP = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes010a4", bomP, bomWhere));

                if (bomP.Count > 0)
                {
                    if (bomP["qty_base"].ToString() != null)
                    {
                        bomP.Add("qty2", Convert.ToDecimal(bomP["qty"].ToString()) / Convert.ToDecimal(bomP["qty_base"].ToString())
                            * Convert.ToDecimal(qty) * (1 + (Convert.ToDecimal(bomP["wastage_rate"].ToString()) / 100)));
                    }



                    DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("mes010a4",
                        " id=" + id, bomP), bomP);
                }




            }
            catch (Exception ex)
            { }
        }


        private static void SetDataqa041m(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT
material_no,
procedure_no
from qa041m where id =" + id + @"
");



                #region 更新物料名称描述



                if (!string.IsNullOrEmpty(p["material_no"].ToString()))
                {


                    string Keys = "material_name,material_specifications";

                    Dictionary<string, object> P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(Keys);

                    string Where = " AND material_no='" + p["material_no"].ToString() + @"' ";

                    Dictionary<string, object> Data = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m,", P, Where));

                    if (Data.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "qa041m", " id=" + id, Data), Data);

                }


                #endregion

                #region 更新工序名称


                if (!string.IsNullOrEmpty(p["procedure_no"].ToString()))
                {


                    string Keys = "procedure_name";

                    Dictionary<string, object> P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(Keys);

                    string Where = " AND procedure_no='" + p["procedure_no"].ToString() + @"' ";

                    Dictionary<string, object> Data = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes001m,", P, Where));

                    if (Data.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "qa041m", " id=" + id, Data), Data);

                }


                #endregion

            }
            catch (Exception ex)
            { }
        }

        private static void SetDataqa008m(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT
group_type,
material_no
from qa008m where id =" + id + @"
");



                #region 更新物料名称描述


                if (!string.IsNullOrEmpty(p["material_no"].ToString()))
                {


                    string materialKeys = "material_name,material_description";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(materialKeys);

                    string materialWhere = " AND material_no='" + p["material_no"].ToString() + @"' ";

                    Dictionary<string, object> materialData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007,", materialP, materialWhere));
                    Dictionary<string, object> materialData2 = new Dictionary<string, object>();
                    materialData2.Add("material_name", materialData["material_name"].ToString());
                    materialData2.Add("material_desc", materialData["material_material_description"].ToString());
                    if (materialData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "qa008m", " id=" + id, materialData2), materialData2);

                }


                #endregion

                #region 更新检验类别名称


                if (!string.IsNullOrEmpty(p["group_type"].ToString()))
                {


                    string Keys = "group_name";

                    Dictionary<string, object> P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(Keys);

                    string Where = " AND group_type='" + p["group_type"].ToString() + @"' ";

                    Dictionary<string, object> Data = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "qa007m,", P, Where));

                    if (Data.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "qa008m", " id=" + id, Data), Data);

                }


                #endregion

            }
            catch (Exception ex)
            { }
        }

        private static void SetDatames020m(int id, string token)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = token;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT 
material_no
from mes020m where id =" + id + @"
");



                #region 更新工序名称描述


                if (!string.IsNullOrEmpty(p["material_no"].ToString()))
                {


                    string materialKeys = "material_name,material_description";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(materialKeys);

                    string materialWhere = " AND material_no='" + p["material_no"].ToString() + @"' ";

                    Dictionary<string, object> materialData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007,", materialP, materialWhere));
                    if (materialData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes020m", " id=" + id, materialData), materialData);

                }


                #endregion

            }
            catch (Exception ex)
            { }
        }


        private static void SetDataqa008a1(int id, string userToken)
        {

            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);


                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT
itemno
from qa008a1 where id =" + id + @"
");





                #region 更新检验类别名称


                if (!string.IsNullOrEmpty(p["itemno"].ToString()))
                {


                    string Keys = "item_type,itemname,description,check_item,qa_level,defect_leve,upper,lower,notes";

                    Dictionary<string, object> P = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(Keys);

                    string Where = " AND itemno='" + p["itemno"].ToString() + @"' ";

                    Dictionary<string, object> Data = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "qa003,", P, Where));

                    if (Data.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "qa008a1", " id=" + id, Data), Data);

                }


                #endregion

            }
            catch (Exception ex)
            { }

        }

        private static void SetDatames010a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT 
procedure_no
from mes010a1 where id =" + id + @"
");



                #region 更新工序名称描述


                if (!string.IsNullOrEmpty(p["procedure_no"].ToString()))
                {


                    string procedureKeys = "procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND procedure_no='" + p["procedure_no"].ToString() + @"' ";

                    Dictionary<string, object> procedureData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes001m", procedureP, procedureWhere));
                    if (procedureData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes010a1", " id=" + id, procedureData), procedureData);

                }


                #endregion

            }
            catch (Exception ex)
            { }
        }

        private static void SetDatahr003a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT 
staff_no
from hr003a1 where id =" + id + @"
");






                #region 更新人名


                if (!string.IsNullOrEmpty(p["staff_no"].ToString()))
                {


                    string staffKeys = "staff_name";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(staffKeys);

                    string staffWhere = " AND staff_no='" + p["staff_no"].ToString() + @"' ";

                    Dictionary<string, object> staffData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "hr001m", materialP, staffWhere));


                    if (staffData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "hr003a1", " id=" + id, staffData), staffData);

                }


                #endregion




            }
            catch (Exception ex)
            {

            }
        }

        private static void SetDataerp001a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT 
material_no
from erp001a1 where id =" + id + @"
");






                #region 更新品名描述


                if (!string.IsNullOrEmpty(p["material_no"].ToString()))
                {


                    string materialKeys = "material_name,material_specifications,material_iunit";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(materialKeys);

                    string materialWhere = " AND material_no='" + p["material_no"].ToString() + @"' ";

                    Dictionary<string, object> materialData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m", materialP, materialWhere));
                    Dictionary<string, object> materialData2 = new Dictionary<string, object>();
                    materialData2.Add("material_name", materialData["material_name"].ToString());
                    materialData2.Add("material_specifications", materialData["material_specifications"].ToString());
                    materialData2.Add("udf01", materialData["material_iunit"].ToString());

                    if (materialData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "erp001a1", " id=" + id, materialData2), materialData2);

                }


                #endregion




            }
            catch (Exception ex)
            {

            }
        }

        private static void SetDatames020a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();

                Dictionary<string, object> p = DB.GetDictionary(@"
SELECT 
process_no,
material_no,
material_no2,
procedure_no
from mes020a1 where id =" + id + @"
");




                #region 更新工艺流程名称描述


                if (!string.IsNullOrEmpty(p["process_no"].ToString()))
                {


                    string processKeys = "process_name";

                    Dictionary<string, object> processP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(processKeys);

                    string processWhere = " AND process_no='" + p["process_no"].ToString() + @"' ";

                    Dictionary<string, object> processData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes002m", processP, processWhere));
                    if (processData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes020a1", " id=" + id, processData), processData);

                }


                #endregion

                #region 更新主件品名描述


                if (!string.IsNullOrEmpty(p["material_no"].ToString()))
                {


                    string materialKeys = "material_name,material_specifications";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(materialKeys);

                    string materialWhere = " AND material_no='" + p["material_no"].ToString() + @"' ";

                    Dictionary<string, object> materialData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m", materialP, materialWhere));
                    Dictionary<string, object> materialData2 = new Dictionary<string, object>();
                    materialData2.Add("material_name", materialData["material_name"].ToString());
                    materialData2.Add("material_specifications", materialData["material_specifications"].ToString());

                    if (materialData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes020a1", " id=" + id, materialData2), materialData2);

                }


                #endregion

                #region 更新子件品名描述


                if (!string.IsNullOrEmpty(p["material_no2"].ToString()))
                {


                    string materialKeys = "material_name,material_specifications";

                    Dictionary<string, object> materialP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(materialKeys);

                    string materialWhere = " AND material_no='" + p["material_no2"].ToString() + @"' ";

                    Dictionary<string, object> materialData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base007m", materialP, materialWhere));
                    Dictionary<string, object> materialData2 = new Dictionary<string, object>();
                    materialData2.Add("material_name2", materialData["material_name"].ToString());
                    materialData2.Add("material_specifications2", materialData["material_specifications"].ToString());

                    if (materialData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes020a1", " id=" + id, materialData2), materialData2);

                }


                #endregion

                #region 更新工序名称描述


                if (!string.IsNullOrEmpty(p["procedure_no"].ToString()))
                {


                    string procedureKeys = "procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND procedure_no='" + p["procedure_no"].ToString() + @"' ";

                    Dictionary<string, object> procedureData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes001m", procedureP, procedureWhere));
                    if (procedureData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes020a1", " id=" + id, procedureData), procedureData);

                }


                #endregion


            }
            catch (Exception ex)
            {

            }
        }

        private static void SetDatabase016a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();


                #region 更新工站名称描述

                string productionsite_no = string.Empty;
                productionsite_no = DB.GetString(@"
SELECT 
productionsite_no
from base016a1 where id =" + id + @"
");


                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("productionsite_no", productionsite_no);

                //工站不为空
                if (!string.IsNullOrEmpty(productionsite_no))
                {


                    string productionsiteKeys = "productionsite_name,productionsite_description";

                    Dictionary<string, object> productionsiteP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(productionsiteKeys);

                    string productionsiteWhere = " AND productionsite_no='" + productionsite_no + @"' ";

                    Dictionary<string, object> productionsiteData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "base015m", productionsiteP, productionsiteWhere));
                    if (productionsiteData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "base016a1", " id=" + id, productionsiteData), productionsiteData);

                }


                #endregion


            }
            catch (Exception ex)
            {

            }
        }

        private static void SetDatames002a1(int id, string userToken)
        {
            try
            {
                SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                reqObj.UserToken = userToken;

                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);





                string sql = string.Empty;
                Dictionary<string, object> InsertData = new Dictionary<string, object>();


                #region 更新工序名称，工序描述

                string procedure_no = string.Empty;
                procedure_no = DB.GetString(@"
SELECT 
procedure_no
from mes002a1 where id =" + id + @"
");


                Dictionary<string, object> p = new Dictionary<string, object>();
                p.Add("procedure_no", procedure_no);

                //工序不为空
                if (!string.IsNullOrEmpty(procedure_no))
                {


                    string procedureKeys = "procedure_name,procedure_description";

                    Dictionary<string, object> procedureP = SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(procedureKeys);

                    string procedureWhere = " AND procedure_no='" + procedure_no + @"' ";

                    Dictionary<string, object> procedureData = DB.GetDictionary(SJeMES_Framework_NETCore.Common.StringHelper.GetSelectSqlByDictionary(DB.DataBaseType, "mes001m", procedureP, procedureWhere));
                    if (procedureData.Count > 0)
                        DB.ExecuteNonQueryOffline(SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary(
                            "mes002a1", " id=" + id, procedureData), procedureData);

                }


                #endregion


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 修改数据时检验数据的合格性

        public static void CheckData(string tableName, string HeadId, Dictionary<string, object> Data, string token)
        {
           

            try
            {
                switch (tableName.ToLower())
                {
                    ///订单-明细
                    case "erp001a1":
                        CheckDataERP001A1(Data);
                        break;
                    ///生产工单
                    case "mes010m":
                        CheckDataMES010M(Data);
                        break;
                    ///生产工单-工艺
                    case "mes010a1":
                        CheckDataMES010A1(Data);
                        break;
                    ///生产工单-BOM
                    case "mes010a4":
                        CheckDataMES010A4(Data);
                        break;
                    ///过程检验单
                    case "qa041m":
                        CheckDataQA041M(Data);
                        break;
                    ///BOM-表身
                    case "mes020a1":
                        CheckDataMES020A1(HeadId, Data, token);
                        break;
                    //外观检测项目库
                    //bdm_aptestitem_m
                    case "bdm_aptestitem_m":
                        CheckDataBDM_APTESTITEM_M(HeadId, Data, token);
                        break;
                    //通用测试类型
                    //bdm_general_testtype_m
                    case "bdm_general_testtype_m":
                        CheckDataBDM_GENERAL_TESTTYPE_M(HeadId, Data, token);
                        break;
                    //开发阶段
                    //BDM_DPSTAGE_M
                    case "bdm_dpstage_m":
                        CheckDataBDM_DPSTAGE_M(HeadId, Data, token);
                        break;
                    //外观检测项目库
                    //BDM_TNTESTITEM_M
                    case "bdm_tntestitem_m":
                        CheckDataBDM_TNTESTITEM_M(HeadId, Data, token);
                        break;
                    //化学品容器管理
                    case "qcm_chemical_container_m":
                        CheckDataBDM_HXP_M(HeadId, Data, token);
                        break;
                    //容器胶水
                    case "bdm_Chemicalglue_m":
                        CheckDataBDM_HXP_M(HeadId, Data, token);
                        break;
                    //化学品容器
                    case "bdm_Containerinformation_m":
                        CheckDataBDM_HXP_M(HeadId, Data, token);
                        break;
                    //2022.2.23化学品信息维护
                    case "bdm_chemical_infomaintenance_m":
                        bdm_chemical_infomaintenance_m(HeadId, Data, token);
                        break;
                    //成品鞋功能性(判断标准)
                    case "bdm_shoes_check_function":
                        BDM_SHOES_CHECK_FUNCTIONS(HeadId, Data, token);
                        break;
                    //工艺测试(判断标准)
                    case "bdm_workmanship_testitem_m":
                        BDM_SHOES_CHECK_FUNCTIONS(HeadId, Data, token);
                        break;
                    //材料测试(判断标准)
                    case "bdm_material_testitem_m":
                        BDM_SHOES_CHECK_FUNCTIONS(HeadId, Data, token);
                        break;
                    //成品鞋功能性(判断标准)
                    case "bdm_shoes_check_function_m":
                        BDM_SHOES_CHECK_FUNCTIONS(HeadId, Data, token);
                        break;
                    //检测项目-工艺-测试-定制（由于选择弹框，配置限制不了，表身不能重复）
                    case "bdm_ws_ti_unique_d":
                        bdm_ws_ti_unique_dS(HeadId, Data, token);
                        break;

                    case "bdm_audit_type_d":
                        bdm_audit_type_dS(HeadId, Data, token);
                        break;
                    //审核项目分类修改不能重复数据
                   /* case "bdm_audit_item_cate_m":
                        bdm_audit_item_cate_M(HeadId, Data, token);
                        break;*/
                    //阶段创建，修改的时候不能相同
                    //case "bdm_phase_creation_m":
                    //    bdm_phase_creation_M(HeadId, Data, token);
                    //    break;



                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }





        #endregion

        #region 检验数据合法性

        private static void CheckDataMES020A1(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {

                //子件物料品号不和很产品品号一样
                //新增
                string material_no = string.Empty;
                if (!string.IsNullOrEmpty(HeadId))
                {
                    SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                    reqOBJ.UserToken = token;

                    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);
                    material_no = DB.GetString("select material_no from mes020m where id=" + HeadId);

                }
                else
                {
                    material_no = data["material_no"].ToString();
                }
                if (material_no == data["material_no2"].ToString())
                {
                    throw new Exception("子件品号不能和产品品号一样");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataBDM_APTESTITEM_M(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {

                if (Convert.ToInt32(data["SAMPLE_NUM"].ToString()) <= 0)
                {
                    throw new Exception("试样数量必须为正整数，请检查！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 化学品管理
        /// </summary>
        /// <param name="HeadId"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        private static void CheckDataBDM_HXP_M(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {

                if (Convert.ToInt32(data["EFFECTIVE_TIME"].ToString()) <= 0)
                {
                    throw new Exception("有效时间必须为正整数，请检查！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 化学品信息维护
        /// </summary>
        /// <param name="HeadId"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        private static void bdm_chemical_infomaintenance_m(string HeadId, Dictionary<string, object> data, string token)
        {

            try
            {
                string sd = string.Empty;
                try
                {
                    sd = "药剂比例必须为正整数,且不能大于100，请检查！";
                    Convert.ToInt32(data["REAGENT_PROPORTION"].ToString());
                    if (Convert.ToDecimal(data["REAGENT_PROPORTION"].ToString()) < 0)
                        throw new Exception("药剂比例必须为正整数,且不能大于100，请检查！");
                    if (Convert.ToDecimal(data["REAGENT_PROPORTION"].ToString()) > 100)
                        throw new Exception("药剂比例必须为正整数,且不能大于100，请检查！");
                    sd = "有效时间必须为正整数，请检查！";
                    Convert.ToInt32(data["EFFECTIVE_TIME"].ToString());
                    if (Convert.ToDecimal(data["EFFECTIVE_TIME"].ToString()) < 0)
                        throw new Exception("有效时间必须为正整数，请检查！");
                    sd = "化学品类别不能为空，请检查！";
                    if (string.IsNullOrWhiteSpace(data["CHEMICAL_CATEGORY"].ToString()))
                        throw new Exception("化学品类别不能为空，请检查！");
                }

                catch { throw new Exception(sd); }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 判断标准
        /// </summary>
        /// <param name="HeadId"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        private static void BDM_SHOES_CHECK_FUNCTIONS(string HeadId, Dictionary<string, object> data, string token)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(data["JUDGMENT_CRITERIA"].ToString()))
                {
                    throw new Exception("判断标准不能为空!");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 检测项目-工艺-测试-测试
        /// </summary>
        /// <param name="HeadId"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        private static void bdm_ws_ti_unique_dS(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {
                //新增
                if (data.ContainsKey("id"))
                {
                    string INSPECTION_CODE = string.Empty;
                    if (!string.IsNullOrWhiteSpace(data["INSPECTION_CODE"].ToString()))
                    {
                        SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                        reqOBJ.UserToken = token;

                        SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);
                        INSPECTION_CODE = DB.GetString("select INSPECTION_CODE from BDM_WS_TI_UNIQUE_D where INSPECTION_CODE=" + data["INSPECTION_CODE"].ToString());
                        if (!string.IsNullOrWhiteSpace(INSPECTION_CODE))
                        {
                            throw new Exception("数据检测项序号重复，修改失败!");
                        }
                    }
                    else
                    {
                        throw new Exception("缺少条件检测项序号INSPECTION_CODE请检测");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void bdm_audit_type_dS(string HeadId, Dictionary<string, object> data, string token)
        {

            try
            {
                string sd = string.Empty;
                try
                {
                    sd = "分数必须为正整数,且不能大于100，请检查！";
                    Convert.ToInt32(data["SCORE_LIMIT"].ToString());
                    if (Convert.ToDecimal(data["SCORE_LIMIT"].ToString()) < 0)
                        throw new Exception("分数必须为正整数,且不能大于100，请检查！");
                    if (Convert.ToDecimal(data["SCORE_LIMIT"].ToString()) > 100)
                        throw new Exception("分数必须为正整数,且不能大于100，请检查！");
                }

                catch { throw new Exception(sd); }
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
        }
        private static void bdm_audit_item_cate_M(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {
                string AUDIT_ITEM_CATE_CODE = string.Empty;
                if (!string.IsNullOrWhiteSpace(data["AUDIT_ITEM_CATE_CODE"].ToString()))
                {
                  
                    SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                    reqOBJ.UserToken = token;

                    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);

                    string sql = $"SELECT AUDIT_ITEM_CATE_CODE FROM bdm_audit_item_cate_m where AUDIT_ITEM_CATE_CODE='{data["AUDIT_ITEM_CATE_CODE"].ToString()}'"; 
                    AUDIT_ITEM_CATE_CODE = DB.GetString(sql);
                    if (!string.IsNullOrWhiteSpace(AUDIT_ITEM_CATE_CODE))
                    {
                        throw new Exception("审核项目分类重复，操作失败!");
                    }
                }
                else
                {
                    throw new Exception("缺少条件检测项序号INSPECTION_CODE请检测");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private static void bdm_phase_creation_M(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {
                string PHASE_CREATION_NO = string.Empty;
                if (!string.IsNullOrWhiteSpace(data["PHASE_CREATION_NO"].ToString()))
                {

                    SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
                    reqOBJ.UserToken = token;

                    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);

                    string sql = $"select PHASE_CREATION_NO from bdm_phase_creation_m where PHASE_CREATION_NO='{data["PHASE_CREATION_NO"]}'";
                    PHASE_CREATION_NO = DB.GetString(sql);
                    if (!string.IsNullOrWhiteSpace(PHASE_CREATION_NO))
                    {
                        throw new Exception("审核项目分类重复，操作失败!");
                    }
                }
                else
                {
                    throw new Exception("缺少条件检测项序号PHASE_CREATION_NO请检测");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private static void CheckDataBDM_TNTESTITEM_M(string HeadId, Dictionary<string, object> data, string token)
        {
            try
            {

                if (Convert.ToInt32(data["SAMPLE_NUM"].ToString()) <= 0)
                {
                    throw new Exception("试样数量必须为正整数，请检查！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataBDM_GENERAL_TESTTYPE_M(string HeadId, Dictionary<string, object> data, string token)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
            reqOBJ.UserToken = token;

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);
            try
            {
                if (data["GENERAL_CATEGORY"].ToString() == "0")
                {
                    string Ishave = DB.GetString($@"SELECT GENERAL_CATEGORY FROM BDM_GENERAL_TESTTYPE_M WHERE GENERAL_CATEGORY = '0' AND  GENERAL_TESTTYPE_NO NOT IN('{data["GENERAL_TESTTYPE_NO"].ToString()}') ");//检验是否还存在类型为0(通用)
                    if (Ishave != "")
                    {
                        throw new Exception("通用类别【通用】已存在，请检查！");
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataBDM_DPSTAGE_M(string HeadId, Dictionary<string, object> data, string token)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject reqOBJ = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
            reqOBJ.UserToken = token;

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqOBJ);
            try
            {
                if (string.IsNullOrEmpty(data["DPSTAGE_CODE"].ToString().Trim()) || string.IsNullOrEmpty(data["DPSTAGE_NAME"].ToString().Trim()))
                    throw new Exception("请确认输入的内容正确");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataQA041M(Dictionary<string, object> data)
        {
            try
            {
                //送检数量
                try
                {
                    Convert.ToDecimal(data["qty"].ToString());
                }

                catch { throw new Exception("送检必须为数字"); }
                if (Convert.ToDecimal(data["qty"].ToString()) <= 0)
                {
                    throw new Exception("送检不能少于或等于0");
                }



                ///抽样数量
                try
                {
                    Convert.ToInt32(data["qty_qa"].ToString());
                }
                catch { throw new Exception("抽样数量必须为正整数"); }
                if (Convert.ToInt32(data["qty_qa"].ToString()) <= 0)
                {
                    throw new Exception("抽样数量不能少于或等于0");
                }



                ///良品数量
                try
                {
                    Convert.ToDecimal(data["qty_ok"].ToString());
                }
                catch { throw new Exception("良品数量必须为数字"); }
                if (Convert.ToDecimal(data["qty_ok"].ToString()) < 0)
                {
                    throw new Exception("良品数量不能少于0");
                }

                ///不良数量
                try
                {
                    Convert.ToDecimal(data["qty_bad"].ToString());
                }
                catch { throw new Exception("不良数量必须为数字"); }
                if (Convert.ToDecimal(data["qty_bad"].ToString()) < 0)
                {
                    throw new Exception("不良数量不能少于0");
                }

                ///报废数量
                try
                {
                    Convert.ToDecimal(data["qty_scrap"].ToString());
                }
                catch { throw new Exception("报废数量必须为数字"); }
                if (Convert.ToDecimal(data["qty_scrap"].ToString()) < 0)
                {
                    throw new Exception("报废数量不能少于0");
                }

                if (Convert.ToDecimal(data["qty_qa"].ToString()) !=
                    Convert.ToDecimal(data["qty_ok"].ToString()) +
                    Convert.ToDecimal(data["qty_bad"].ToString()) +
                        Convert.ToDecimal(data["qty_scrap"].ToString()))
                {
                    throw new Exception("良品数量+不良数量+报废数量 不等于 抽样数量");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataERP001A1(Dictionary<string, object> data)
        {
            try
            {
                //数量
                try
                {

                    Convert.ToDecimal(data["qty"].ToString());
                }
                catch { throw new Exception("数量必须为数字类型"); }




                ///计划开工
                try
                {

                    Convert.ToDateTime(data["delivery_date"].ToString() + " 23:59:59");
                }
                catch { throw new Exception("交货日期必须为日期类型"); }

                if (Convert.ToDateTime(data["delivery_date"].ToString() + " 23:59:59") < DateTime.Now.AddDays(1))
                {
                    throw new Exception("交货日期不能早于当前日期（+1天）");
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataMES010M(Dictionary<string, object> data)
        {
            try
            {
                //计划数量
                try
                {
                    Convert.ToDecimal(data["qty"].ToString());
                }
                catch { throw new Exception("计划数量必须为数字"); }
                if (Convert.ToDecimal(data["qty"].ToString()) <= 0)
                {
                    throw new Exception("计划数量不能少于或等于0");
                }


                DateTime time;
                ///计划开工
                if (!string.IsNullOrEmpty(data["date_start_plan"].ToString()))
                {
                    try
                    {
                        time = Convert.ToDateTime(data["date_start_plan"].ToString() + " 23:59:59");
                    }

                    catch { throw new Exception("计划开工日期必须为日期类型"); }
                    if (time < DateTime.Now)
                    {
                        throw new Exception("计划开工日期不能早于当前日期");
                    }
                }



                ///计划完工
                try
                {

                    time = Convert.ToDateTime(data["date_finish_plan"].ToString() + " 23:59:59");
                }
                catch { throw new Exception("计划完工日期必须为日期类型"); }

                if (Convert.ToDateTime(data["date_finish_plan"].ToString() + " 23:59:59") < DateTime.Now)
                {
                    throw new Exception("计划完工日期不能早于当前日期");
                }

                if (!string.IsNullOrEmpty(data["date_start_plan"].ToString()))
                {
                    if (Convert.ToDateTime(data["date_finish_plan"].ToString() + " 23:59:59") < Convert.ToDateTime(data["date_start_plan"].ToString() + " 23:59:59"))
                    {
                        throw new Exception("计划完工日期不能早于计划开工日期");
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataMES010A4(Dictionary<string, object> data)
        {
            try
            {
                ///用量
                try
                {
                    Convert.ToDecimal(data["qty"].ToString());
                }
                catch { throw new Exception("用量必须为数字"); }
                if (Convert.ToDecimal(data["qty"].ToString()) <= 0)
                {
                    throw new Exception("用量不能少于或等于0");
                }



                ///底数
                if (data.ContainsKey("qty_base"))
                {
                    try
                    {
                        Convert.ToInt32(data["qty_base"].ToString());
                    }
                    catch { throw new Exception("底数必须为正整数"); }

                    if (Convert.ToInt32(data["qty_base"].ToString()) <= 0)
                    {
                        throw new Exception("底数不能少于或等于0");
                    }

                }




                ///损耗率
                if (data.ContainsKey("wastage_rate"))
                    try
                    {
                        Convert.ToDecimal(data["wastage_rate"].ToString());
                    }
                    catch { throw new Exception("损耗率必须为数字"); }
                if (Convert.ToDecimal(data["wastage_rate"].ToString()) < 0)
                {
                    throw new Exception("损耗率不能少于0");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void CheckDataMES010A1(Dictionary<string, object> data)
        {
            try
            {
                ///生产顺序
                try
                {
                    Convert.ToInt32(data["sorting"].ToString());
                }
                catch { throw new Exception("生产顺序只能为正整数"); }
                if (Convert.ToInt32(data["sorting"].ToString()) < 1)
                {
                    throw new Exception("生产顺序只能为正整数");
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 获取单号

        public static string GetDocNo(string keys, string tablename, string token)
        {
            string DocNo = string.Empty;

            SJeMES_Framework_NETCore.WebAPI.RequestObject reqObj = new SJeMES_Framework_NETCore.WebAPI.RequestObject();
            reqObj.UserToken = token;

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(reqObj);

            switch (tablename.ToLower())
            {
                ///工单
                case "mes010m":
                    DocNo = "MO";
                    break;
                ///客户订单
                case "erp001m":
                    DocNo = "CO";
                    break;
                ///过站检验单
                case "qa041m":
                    DocNo = "IPQC";
                    break;
            }

            if (DB.DataBaseType.ToLower() == "mysql")
            {
                DocNo = DocNo + DateTime.Now.ToString("yyMMdd") + (Convert.ToInt32(DB.GetString(@"
SELECT IFNULL(MAX(" + keys + "),'0000') FROM " + DB.ChangeKeyWord(tablename) + @" where " + keys + @" like '" + DocNo + DateTime.Now.ToString("yyMMdd") + @"%'").Replace(
                 DocNo + DateTime.Now.ToString("yyMMdd"), "")) + 1).ToString("0000");
            }
            else if (DB.DataBaseType.ToLower() == "sqlserver")
            {
                DocNo = DocNo + DateTime.Now.ToString("yyMMdd") + (Convert.ToInt32(DB.GetString(@"
SELECT ISNULL(MAX(" + keys + "),'0000') FROM " + DB.ChangeKeyWord(tablename) + @" where " + keys + @" like '" + DocNo + DateTime.Now.ToString("yyMMdd") + @"%'").Replace(
                 DocNo + DateTime.Now.ToString("yyMMdd"), "")) + 1).ToString("0000");
            }
            else if (DB.DataBaseType.ToLower() == "oracle")
            {
                DocNo = DocNo + DateTime.Now.ToString("yyMMdd") + (Convert.ToInt32(DB.GetString(@"
SELECT ISNULL(MAX(" + keys + "),'0000') FROM " + DB.ChangeKeyWord(tablename) + @" where " + keys + @" like '" + DocNo + DateTime.Now.ToString("yyMMdd") + @"%'").Replace(
                 DocNo + DateTime.Now.ToString("yyMMdd"), "")) + 1).ToString("0000");
            }



            return DocNo;
        }

        #endregion
    }
}
