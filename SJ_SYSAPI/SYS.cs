using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace SJ_SYSAPI
{
    public class SYS
    {

        #region 通用页面方法
        /// <summary>
        /// Web获取模块配置信息接口(表头列表模式)
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject WebGetModuleConfigPanelHList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string APP_Code = jarr["APP_Code"].ToString();

                bool ShowSYS = true;
                try
                {
                    ShowSYS = Convert.ToBoolean(jarr["ShowSYS"].ToString());
                }
                catch { }
                if (!string.IsNullOrEmpty(APP_Code))
                {
                    string App_Json = DB.GetString("select App_JsonHList from SYSAPP01M where APP_Code='" + APP_Code + "'");
                    if (!string.IsNullOrEmpty(App_Json))
                    {
                        SJeMES_Framework_NETCore.Web.JSONPanelClassHList jchl = Newtonsoft.Json.JsonConvert.DeserializeObject<SJeMES_Framework_NETCore.Web.JSONPanelClassHList>(App_Json);

                        List<SJeMES_Framework_NETCore.Web.JSONPanelClassHListItem> Now = new List<SJeMES_Framework_NETCore.Web.JSONPanelClassHListItem>();

                        foreach (SJeMES_Framework_NETCore.Web.JSONPanelClassHListItem jchli in jchl.tableHead)
                        {
                            if (!ShowSYS)
                            {
                                //if (jchli.prop == "enable" || jchli.prop == "createby" ||
                                //    jchli.prop == "createdate" || jchli.prop == "createtime" ||
                                //    jchli.prop == "modifyby" || jchli.prop == "modifydate" ||
                                //    jchli.prop == "modifytime" || jchli.prop == "deleteby" ||
                                //    jchli.prop == "deletedate" || jchli.prop == "deletetime" ||
                                //    jchli.prop == "isdelete" || jchli.prop == "guid" ||
                                //    jchli.prop == "timestamp" || jchli.prop =="org" ||
                                //    jchli.prop == "dosureby" || jchli.prop =="dosuredate" ||
                                //    jchli.prop == "dosuretime" || jchli.prop =="aduitby" ||
                                //    jchli.prop == ")
                                //{
                                bool IsSys = false;

                                foreach (string s in SJeMES_Framework_NETCore.Web.System.SystemFields.Split(','))
                                {
                                    if (s == jchli.prop)
                                    {
                                        IsSys = true;
                                        break;
                                    }
                                }




                                if (!IsSys && jchli.prop != "org")
                                {
                                    Now.Add(jchli);
                                }
                            }
                            else
                            {
                                if (jchli.prop == "enable" || jchli.prop == "deleteby" ||
                                    jchli.prop == "deletedate" || jchli.prop == "deletetime" ||
                                    jchli.prop == "isdelete" || jchli.prop == "guid" ||
                                    jchli.prop == "timestamp" || jchli.prop == "org")

                                {
                                }
                                else
                                {
                                    Now.Add(jchli);
                                }
                            }


                        }



                        jchl.tableHead = Now;



                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(jchl);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        //ret.ErrMsg = "获取数据失败！";
                        ret.ErrMsg = "Failed to get data！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "Code为空！";
                    ret.ErrMsg = "Code is empty！";
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
        /// 获取表头主键信息 --多个
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetTableHeadsValue(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string TableName = jarr["TableName"].ToString();//表名
                string AppCode = jarr["APP_Code"].ToString();//模块code
                string HeadId = jarr["HeadId"].ToString();//id;
                if (!string.IsNullOrEmpty(HeadId))
                {
                    #region 获取和表头绑定关系
                    string headtable = DBSys.GetString(@"select App_TableH from SYSAPP01M where APP_Code='" + AppCode + @"'");
                    if (TableName.Contains('\''))
                    {
                        TableName = TableName.Replace("'", "\''");
                    }
                    string sheadkeys = DBSys.GetString(@"Select HeadKeys FROM SYSAPP01A1 where TableB='" + TableName + @"' AND APP_Code='" + AppCode + @"'");
                    sheadkeys = sheadkeys.Replace("[", "").Replace("]", "").Replace("\"", "");
                    string[] jo;
                    if (sheadkeys.IndexOf(",") > -1)
                    {
                        jo = sheadkeys.Split(',');
                    }
                    else
                    {
                        jo = new string[1] { sheadkeys };
                    }
                    sheadkeys = string.Empty;
                    foreach (string s in jo)
                    {
                        if (DB.DataBaseType.ToLower() != "oracle")
                        {
                            sheadkeys += s.Split(':')[0] + ",";
                        }
                        else
                        {
                            sheadkeys += s.Split(':')[0].ToUpper() + ",";
                        }
                    }
                    sheadkeys = sheadkeys.Remove(sheadkeys.Length - 1);
                    string sql = @"select " + sheadkeys + @" from " + DB.ChangeKeyWord(headtable) + @" where id=" + HeadId;
                    DataTable dHeadKeys = DB.GetDataTable(sql);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dHeadKeys);
                    #endregion


                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "没有传入HeadId参数，无法自动添加关联数据";
                    ret.ErrMsg = "If the HeadId parameter is not passed in, associated data cannot be automatically added";
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
        /// Web获取模块配置信息接口(表身)
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject WebGetModuleConfigPanelB(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string APP_Code = jarr["APP_Code"].ToString();
                if (!APP_Code.StartsWith("PC_"))
                {
                    APP_Code = "PC_" + APP_Code;
                }
                bool ShowSYS = true;
                try
                {
                    ShowSYS = Convert.ToBoolean(jarr["ShowSYS"].ToString());
                }
                catch { }
                if (!string.IsNullOrEmpty(APP_Code))
                {
                    string App_Json = DB.GetString("select App_Json from SYSAPP01M where APP_Code='" + APP_Code + "'");
                    if (!string.IsNullOrEmpty(App_Json))
                    {
                        SJeMES_Framework_NETCore.Web.JSONFormClass jfc = Newtonsoft.Json.JsonConvert.DeserializeObject<SJeMES_Framework_NETCore.Web.JSONFormClass>(App_Json);

                        #region 是否显示系统字段
                        if (!ShowSYS)
                        {
                            string[] sSys = SJeMES_Framework_NETCore.Web.System.SystemFields.Split(',');

                            foreach (SJeMES_Framework_NETCore.Web.JSONPanelClassB pb in jfc.PanelB)
                            {
                                List<SJeMES_Framework_NETCore.Web.JSONControlB> NewJCB = new List<SJeMES_Framework_NETCore.Web.JSONControlB>();

                                foreach (SJeMES_Framework_NETCore.Web.JSONControlB jcb in pb.tableHead)
                                {
                                    bool isSys = false;
                                    foreach (string s in sSys)
                                    {
                                        if (jcb.prop == s)
                                        {
                                            isSys = true;
                                        }

                                    }

                                    if (!isSys)
                                    {
                                        NewJCB.Add(jcb);
                                    }
                                }

                                pb.tableHead = NewJCB;
                            }
                        }
                        else
                        {

                        }
                        #endregion

                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(jfc.PanelB);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Failed to get data！";
                        //ret.ErrMsg = "获取数据失败！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "Code为空！";
                    ret.ErrMsg = "Code is empty！";
                }

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
        /// 获取单据状态
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetDocStatus(object OBJ)
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

                string reqKey = "APP_Code,Id";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqKey, ReqObj);

                string tablename = DBSys.GetString("select App_TableH from SYSAPP01M where APP_Code=@APP_Code", reqP);

                string status = DB.GetString(@"
SELECT
status
FROM
" + DB.ChangeKeyWord(tablename) + @"
where id=@Id
", reqP);
                ret.IsSuccess = true;
                ret.RetData = status;

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
        /// Web获取模块配置信息接口(表头)
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject WebGetModuleConfigPanelH(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string APP_Code = jarr["APP_Code"].ToString();
                if (!string.IsNullOrEmpty(APP_Code))
                {
                    string App_Json = DB.GetString("select App_JsonH from SYSAPP01M where APP_Code='" + APP_Code + "'");

                    DataTable dt = DB.GetDataTable("select * from SYSAPP01A2 where App_Code='" + APP_Code + "'");

                    if (!string.IsNullOrEmpty(App_Json))
                    {

                        ret.IsSuccess = true;
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("App_Json", App_Json);
                        p.Add("OtherMenu", SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt));
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        //ret.ErrMsg = "获取数据失败！";
                        ret.ErrMsg = "Failed to get data！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "Code为空！";
                    ret.ErrMsg = "Code is empty！";
                }

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
        /// Web获取模块配置信息接口(整个模块)
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject WebGetModuleConfig(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string APP_Code = jarr["APP_Code"].ToString();
                if (!string.IsNullOrEmpty(APP_Code))
                {
                    string App_Json = DB.GetString("select App_Json from SYSAPP01M where APP_Code='" + APP_Code + "'");

                    DataTable dt = DB.GetDataTable("select * from SYSAPP01A2 where App_Code='" + APP_Code + "'");

                    if (!string.IsNullOrEmpty(App_Json))
                    {

                        ret.IsSuccess = true;
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("App_Json", App_Json);
                        p.Add("OtherMenu", dt);
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        //ret.ErrMsg = "获取数据失败！";
                        ret.ErrMsg = "Failed to get data！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "Code为空！";
                    ret.ErrMsg = "Code is empty！";
                }

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
        /// 删除模块数据接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject DelModuleData(object OBJ)
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


                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(Data);

                foreach (DataRow dr in dt.Rows)
                {
                    string TableName = dr["TableName"].ToString();//表名
                    string Id = dr["Id"].ToString();//ID
                    string sql = string.Empty;

                    if (!string.IsNullOrEmpty(TableName))
                    {
                        #region 是否表头，是的话删除对应表身数据
                        string APP_Code = DBSys.GetString(@"select APP_Code from SYSAPP01M where App_TableH='" + TableName + @"'");
                        if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql += @"begin
";

                        }
                        ///是表头
                        if (!string.IsNullOrEmpty(APP_Code))
                        {
                            DataTable dtBodyInfo = DBSys.GetDataTable(@"select * from SYSAPP01A1 where APP_Code='" + APP_Code + @"'");

                            foreach (DataRow dr2 in dtBodyInfo.Rows)
                            {
                                string tablenameb = dr2["TableB"].ToString();
                                string sheadkeys = dr2["headkeys"].ToString();
                                string whereB = string.Empty;
                                sheadkeys = sheadkeys.Replace("[", "").Replace("]", "").Replace("\"", "");
                                string[] jo;
                                if (sheadkeys.IndexOf(",") > -1)
                                {
                                    jo = sheadkeys.Split(',');
                                }
                                else
                                {
                                    jo = new string[1] { sheadkeys };
                                }

                                Dictionary<string, string> djo = new Dictionary<string, string>();
                                sheadkeys = string.Empty;
                                foreach (string s in jo)
                                {
                                    if (DB.DataBaseType.ToLower() != "oracle")
                                    {
                                        djo.Add(s.Split(':')[0], s.Split(':')[1]);
                                        sheadkeys += s.Split(':')[0] + ",";
                                    }
                                    else
                                    {
                                        djo.Add(s.Split(':')[0].ToUpper(), s.Split(':')[1]);
                                        sheadkeys += s.Split(':')[0].ToUpper() + ",";
                                    }
                                }


                                

                                sheadkeys = sheadkeys.Remove(sheadkeys.Length - 1);

                                Dictionary<string, string> dHeadKeys = new Dictionary<string, string>();

                                DataTable dtHeadKeys = new DataTable();

                                if (DB.DataBaseType.ToLower() == "mysql")
                                {
                                    dtHeadKeys = DB.GetDataTable(@"select " + sheadkeys + @" from `" + TableName + @"` where id=" + Id + @"");
                                }
                                else if (DB.DataBaseType.ToLower() == "sqlserver")
                                {
                                    dtHeadKeys = DB.GetDataTable(@"select " + sheadkeys + @" from [" + TableName + @"] where id=" + Id + @"");
                                }
                                else if (DB.DataBaseType.ToLower() == "oracle")
                                {
                                    dtHeadKeys = DB.GetDataTable(@"select " + sheadkeys + " from " + TableName + " where id=" + Id + @"");
                                    // dtHeadKeys = DB.GetDataTable(@"select " + sheadkeys + " from " + TableName + " where "+ sheadkeys + "=" + Id + @"");
                                }

                                if (dtHeadKeys.Rows.Count > 0)
                                {
                                    foreach (DataColumn dc in dtHeadKeys.Columns)
                                    {
                                        whereB += " AND " + DB.ChangeKeyWord(djo[dc.ColumnName]) + @"='" + dtHeadKeys.Rows[0][dc.ColumnName].ToString() + @"' ";
                                    }
                                }

                                if (!string.IsNullOrEmpty(whereB))
                                {
                                    if (!tablenameb.Trim().Contains("SELECT"))
                                    {
                                        sql += @"
DELETE FROM " + DB.ChangeKeyWord(tablenameb.Trim()) + @" WHERE 1=1 " + whereB + @";
";
                                    }


                                }
                            }
                        }


                        #region 通用检测类型删除

                        if (TableName == "BDM_GENERAL_TESTTYPE_M")
                        {
                            DB.Open();
                            DB.BeginTransaction();
                            string gereral_testtype_no = DB.GetString($@"SELECT GENERAL_TESTTYPE_NO from BDM_GENERAL_TESTTYPE_M WHERE ID = '{Id}'");

                            //删除通用品质标准5个表相应检测类型代号数据
                            DB.ExecuteNonQuery($@"DELETE FROM BDM_GENERALQUALITY_M WHERE GENERAL_TESTTYPE_NO = '{gereral_testtype_no}'");
                            DB.ExecuteNonQuery($@"DELETE FROM BDM_GENERALQUALITY_D WHERE GENERAL_TESTTYPE_NO = '{gereral_testtype_no}'");
                            DB.ExecuteNonQuery($@"DELETE FROM BDM_QUALITYTEST_ITEM WHERE GENERAL_TESTTYPE_NO = '{gereral_testtype_no}'");
                            DB.ExecuteNonQuery($@"DELETE FROM BDM_QUALITYAPTEST_ITEM WHERE GENERAL_TESTTYPE_NO = '{gereral_testtype_no}'");
                            DB.ExecuteNonQuery($@"DELETE FROM BDM_QUALITYTNTEST_ITEM WHERE GENERAL_TESTTYPE_NO = '{gereral_testtype_no}'");

                            DB.Commit();
                            DB.Close();
                        }

                        #endregion

                        #endregion

                        sql += "delete from " + TableName.Trim() + " where id=" + Id + ";";

                        if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql += "end;";
                        }
                        string sql2 = "insert into SJCS(title) values('" + sql.Replace("'","\"") + "')";
                        DBSys.ExecuteNonQueryOffline(sql2);
                        DB.ExecuteNonQueryOffline(sql);

                    }


                }
                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted！";
                //ret.ErrMsg = "删除成功！";

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }



        #region old
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject AddModuleData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                var o = JObject.Parse(Data);
                string TableName = jarr["TableName"].ToString();//表名
                string AppCode = jarr["APP_Code"].ToString();//模块code
                string HeadId = string.Empty;
                string sql;
                try
                {
                    HeadId = jarr["HeadId"].ToString();
                }
                catch { }

                string createby = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);



                bool IsHeadData = false;
                bool IsDoc = false;

                foreach (string s in SJeMES_Framework_NETCore.Web.System.DocModules.Split(","))
                {
                    if (AppCode.ToLower() == s.ToLower())
                    {
                        IsDoc = true;
                    }
                }


                if (!string.IsNullOrEmpty(TableName))
                {
                    for (int i = 0; i < TableName.Split(',').Length; i++)
                    {
                        string keys = DBSys.GetString("select App_TableKeysH from SYSAPP01M where APP_Code='" + AppCode + "' and App_TableH='" + TableName + "'");
                        if (string.IsNullOrEmpty(keys))
                        {
                            keys = DBSys.GetString("select TableKeysB from SYSAPP01A1 where APP_Code='" + AppCode + "' and TableB='" + TableName + "'");
                        }
                        if (!string.IsNullOrEmpty(keys))
                        {
                            //Dictionary<string,object> Dkeys =SJeMES_Framework_NETCore.Common.StringHelper.GetDictionaryByString(keys.Replace('[', ' ').Replace(']', ' ').Replace('"', ' ').Replace(" ",""));
                            string Name = o[TableName.Split(',')[i]].ToString();
                            int n = 0;


                            ArrayList arrayList = Newtonsoft.Json.JsonConvert.DeserializeObject<ArrayList>(Name);

                            string retId = string.Empty;

                            if (arrayList.Count > 0)
                            {
                                foreach (object arr in arrayList)
                                {
                                    Dictionary<string, object> dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(arr.ToString());

                                    ///检查数据合法性
                                    Logic.CheckData(TableName, HeadId, dictionary, ReqObj.UserToken);


                                    string key = string.Empty;
                                    string value = string.Empty;
                                    string where = string.Empty;

                                    #region 是否表头
                                    string APP_Code = DBSys.GetString(@"select APP_Code from SYSAPP01M where App_TableH='" + TableName.Split(',')[i] + @"'");

                                    ///是否表头
                                    if (!string.IsNullOrEmpty(APP_Code))
                                    {
                                        IsHeadData = true;
                                    }
                                    #endregion

                                    Dictionary<string, object> dHeadKeys = new Dictionary<string, object>();
                                    Dictionary<string, string> djo = new Dictionary<string, string>();
                                    #region 如果是表身的话，添加表头自动关联
                                    if (!IsHeadData)
                                    {
                                        if (!string.IsNullOrEmpty(HeadId))
                                        {
                                            #region 获取和表头绑定关系

                                            string headtable = DBSys.GetString(@"select App_TableH from SYSAPP01M where APP_Code='" + AppCode + @"'");
                                            string sheadkeys = DBSys.GetString(@"Select HeadKeys FROM SYSAPP01A1 where TableB='" + TableName + @"' AND APP_Code='" + AppCode + @"'");

                                            sheadkeys = sheadkeys.Replace("[", "").Replace("]", "").Replace("\"", "");
                                            string[] jo;
                                            if (sheadkeys.IndexOf(",") > -1)
                                            {
                                                jo = sheadkeys.Split(',');
                                            }
                                            else
                                            {
                                                jo = new string[1] { sheadkeys };
                                            }



                                            sheadkeys = string.Empty;
                                            foreach (string s in jo)
                                            {
                                                if (DB.DataBaseType.ToLower() != "oracle")
                                                {
                                                    djo.Add(s.Split(':')[0], s.Split(':')[1]);
                                                    sheadkeys += s.Split(':')[0] + ",";
                                                }
                                                else
                                                {
                                                    djo.Add(s.Split(':')[0].ToUpper(), s.Split(':')[1]);
                                                    sheadkeys += s.Split(':')[0].ToUpper() + ",";
                                                }
                                            }

                                            sheadkeys = sheadkeys.Remove(sheadkeys.Length - 1);


                                            sql = @"select " + sheadkeys + @" from " + DB.ChangeKeyWord(headtable) + @" where id=" + HeadId;
                                            dHeadKeys = DB.GetDictionary(sql);



                                            #endregion


                                        }
                                        else
                                        {
                                            ret.IsSuccess = false;
                                            //ret.ErrMsg = "没有传入HeadId参数，无法自动添加关联数据";
                                            ret.ErrMsg = "If the HeadId parameter is not passed in, associated data cannot be automatically added";
                                            return ret;
                                        }
                                    }


                                    #endregion


                                    foreach (string current in dictionary.Keys)
                                    {


                                        bool ishas = false;
                                        foreach (string s in dHeadKeys.Keys)
                                        {
                                            if (s == current)
                                                ishas = true;
                                        }

                                        if (!ishas && current.ToLower() != "seq" && current.ToLower() != "sorting"
                                            && current.ToLower() != "serial_number" && current.ToLower() != "status")
                                        {
                                            if (DB.DataBaseType.ToLower() != "oracle")
                                            {
                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    key = current;
                                                }
                                                else
                                                {
                                                    key += "," + current;
                                                }
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    key = "\"" + current.ToUpper() + "\"";
                                                }
                                                else if (!key.Contains(current.ToUpper()))
                                                {
                                                    key += "," + current;
                                                }
                                            }


                                            if (DB.DataBaseType.ToLower() != "oracle")
                                            {
                                                if (string.IsNullOrEmpty(value))
                                                {

                                                    value = "@" + current;
                                                }
                                                else
                                                {
                                                    value += ",@" + current;
                                                }
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(value))
                                                {

                                                    value = ":" + current;
                                                }
                                                else if (!value.Contains(current))
                                                {
                                                    value += ",:" + current;
                                                }
                                            }


                                        }
                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {

                                            if (!ishas && keys.Contains(current) && current.ToLower() != "seq"
                                            && current.ToLower() != "sorting" && current.ToLower() != "serial_number")
                                            {
                                                where += " and " + current + "=@" + current;
                                            }
                                        }
                                        else
                                        {
                                            if (!ishas && keys.Contains(current) && current.ToLower() != "seq"
                                            && current.ToLower() != "sorting" && current.ToLower() != "serial_number")
                                            {
                                                where += " and \"" + current.ToUpper() + "\"=:" + current;
                                            }
                                        }


                                    }

                                    #region 表身关联字段
                                    if (dHeadKeys.Count > 0)
                                    {
                                        foreach (string dc in dHeadKeys.Keys)
                                        {
                                            if (DB.DataBaseType.ToLower() != "oracle")
                                            {
                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    key = djo[dc];
                                                }
                                                else
                                                {
                                                    key += "," + djo[dc];
                                                }
                                                if (string.IsNullOrEmpty(value))
                                                {
                                                    value = "@" + djo[dc];
                                                }
                                                else
                                                {
                                                    value += ",@" + djo[dc];
                                                }
                                                where += " and " + dc + "=@" + djo[dc];
                                            }
                                            else
                                            {
                                                if (string.IsNullOrEmpty(key))
                                                {
                                                    key = "\"" + djo[dc].ToUpper() + "\"";
                                                }
                                                else if (!key.Contains(djo[dc].ToUpper()))
                                                {
                                                    key += ",\"" + djo[dc].ToUpper() + "\"";
                                                }
                                                if (string.IsNullOrEmpty(value))
                                                {
                                                    value = ":" + djo[dc];
                                                }
                                                else if (!value.Contains(djo[dc]))
                                                {
                                                    value += ",:" + djo[dc];
                                                }
                                                if (!where.Contains(djo[dc].ToUpper()))
                                                {
                                                    where += " and \"" + djo[dc].ToUpper() + "\"=:" + djo[dc];
                                                }
                                               
                                            }


                                            if (dictionary.ContainsKey(djo[dc]))
                                            {
                                                dictionary[djo[dc]] = dHeadKeys[dc].ToString().Trim();
                                            }
                                            else
                                            {
                                                dictionary.Add(djo[dc], dHeadKeys[dc].ToString().Trim());
                                            }

                                        }


                                    }
                                    #endregion

                                    Dictionary<string, object> tmp = new Dictionary<string, object>();
                                    foreach (string current in dictionary.Keys)
                                    {
                                        tmp.Add(current, dictionary[current].ToString().Trim());
                                    }



                                    if (keys.Contains("seq"))
                                    {
                                        int seq = 0;

                                        if (DB.DataBaseType.ToLower() == "mysql")
                                        {
                                            seq = DB.GetInt32(@"
                select IFNULL(MAX(cast(seq as SIGNED)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                                        {
                                            seq = DB.GetInt32(@"
                select ISNULL(MAX(cast(seq as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (
                                            DB.DataBaseType.ToLower() == "oracle")
                                        {
                                            seq = DB.GetInt32(@"
                select NVL(MAX(cast(seq as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }

                                        seq = ((seq / 10) + 1) * 10;

                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {
                                            key += ",seq";
                                            value += ",@seq";
                                            where += " AND seq=@seq";
                                        }
                                        else
                                        {
                                            key += ",seq";
                                            value += ",:seq";
                                            where += " AND seq=:seq";
                                        }

                                        if (tmp.ContainsKey("seq"))
                                        {
                                            tmp["seq"] = seq.ToString().Trim();
                                        }
                                        else
                                        {
                                            tmp.Add("seq", seq.ToString().Trim());
                                        }

                                    }




                                    int sorting = 0;
                                    if (keys.Contains("sorting"))
                                    {
                                        

                                        if (DB.DataBaseType.ToLower() == "mysql")
                                        {
                                            sorting = DB.GetInt32(@"
                select IFNULL(MAX(cast(sorting as SIGNED)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                                        {
                                            sorting = DB.GetInt32(@"
                select ISNULL(MAX(cast(sorting as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (
                                            DB.DataBaseType.ToLower() == "oracle")
                                        {
                                            sorting = DB.GetInt32(@"
                select NVL(MAX(cast(sorting as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        sorting = ((sorting / 10) + 1) * 10;

                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {
                                            key += ",sorting";
                                            value += ",@sorting";
                                            where += " AND sorting=@sorting";
                                        }
                                        else
                                        {
                                            key += ",sorting";
                                            value += ",:sorting";
                                            where += " AND sorting=:sorting";
                                        }

                                        if (tmp.ContainsKey("sorting"))
                                        {
                                            tmp["sorting"] = sorting.ToString().Trim();
                                        }
                                        else
                                        {
                                            tmp.Add("sorting", sorting.ToString().Trim());
                                        }

                                    }

                                    if (keys.Contains("serial_number"))
                                    {
                                        int serial_number = 0;

                                        if (DB.DataBaseType.ToLower() == "mysql")
                                        {
                                            serial_number = DB.GetInt32(@"
                select IFNULL(MAX(cast(serial_number as SIGNED)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                                        {
                                            serial_number = DB.GetInt32(@"
                select ISNULL(MAX(cast(serial_number as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        else if (
                                            DB.DataBaseType.ToLower() == "oracle")
                                        {
                                            serial_number = DB.GetInt32(@"
                select NVL(MAX(cast(serial_number as int)),0) from " + TableName.Split(',')[i] + " where 1=1 " + where + @"
                ", tmp);
                                        }
                                        serial_number = ((serial_number / 10) + 1) * 10;

                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {
                                            key += ",serial_number";
                                            value += ",@serial_number";
                                            where += " AND serial_number=@serial_number";
                                        }
                                        else
                                        {
                                            key += ",serial_number";
                                            value += ",serial_number";
                                            where += " AND serial_number=serial_number";
                                        }

                                        if (tmp.ContainsKey("serial_number"))
                                        {
                                            tmp["serial_number"] = serial_number.ToString().Trim();
                                        }
                                        else
                                        {
                                            tmp.Add("serial_number", serial_number.ToString().Trim());
                                        }

                                    }


                                    if (DB.DataBaseType.ToLower() == "mysql")
                                    {
                                        if (DB.GetInt32(@"select count(1) from information_schema.columns where table_name = '" + TableName.Split(',')[i] + "' and column_name = 'guid' AND TABLE_SCHEMA ='" + DB.DataBaseName + @"'") == 1)
                                        {
                                            key += ",guid";
                                            value += ",@guid";
                                            tmp.Add("guid", Guid.NewGuid().ToString());
                                        }

                                        if (DB.GetInt32(@"select count(1) from information_schema.columns where table_name = '" + TableName.Split(',')[i] + "' and column_name = 'timestamp' AND TABLE_SCHEMA ='" + DB.DataBaseName + @"'") == 1)
                                        {
                                            key += ",timestamp";
                                            value += ",@timestamp";
                                            tmp.Add("timestamp", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString());
                                        }
                                    }
                                    //else if (DB.DataBaseType.ToLower() == "sqlserver")
                                    //{
                                    //    if (DB.GetInt32(@"select   count(1)  from   syscolumns   where   id=object_id('" + TableName.Split(',')[i] + "')   and   name='guid'") == 1)
                                    //    {
                                    //        key += ",guid";
                                    //        value += ",@guid";
                                    //        tmp.Add("guid", Guid.NewGuid().ToString());
                                    //    }

                                    //    if (DB.GetInt32(@"select   count(1)  from   syscolumns   where   id=object_id('" + TableName.Split(',')[i] + "')   and   name='timestamp'") == 1)
                                    //    {
                                    //        key += ",timestamp";
                                    //        value += ",@timestamp";
                                    //        tmp.Add("timestamp", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString());
                                    //    }


                                    //}

                                    else if (DB.DataBaseType.ToLower() == "oracle")
                                    {
                                        if (DB.GetInt32(@"SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + TableName.Split(',')[i].ToUpper() + "' AND COLUMN_NAME = 'GUID'") == 1)
                                        {
                                            key += ",guid";
                                            value += ",:guid";
                                            tmp.Add("guid", Guid.NewGuid().ToString());
                                        }

                                        //if (DB.GetInt32(@"SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + TableName.Split(',')[i].ToUpper() + "' AND COLUMN_NAME = 'TIMESTAMP'") == 1)
                                        //{
                                        //    key += ",\"TIMESTAMP\"";
                                        //    value += ",:timestamp";
                                        //    tmp.Add("timestamp", DateTime.Now.ToString("yyyyMMddHHmmssss"));
                                        //}


                                    }







                                    sql = "select * from " + TableName.Split(',')[i] + " where 1=1 " + where + "";
                                    string sql1 = "insert into SJCS(title) values('" + sql + "')";
                                    DBSys.ExecuteNonQueryOffline(sql1);
                                    DataTable dt = DB.GetDataTable(sql, tmp);
                                    if (dt.Rows.Count == 0)
                                    {

                                        if (IsHeadData && IsDoc)
                                        {
                                            keys = keys.Replace("[", "").Replace("]", "").Replace("\"", "");
                                            tmp[keys] = Logic.GetDocNo(keys, TableName.Split(',')[i], ReqObj.UserToken);

                                            key += ",status";
                                            value += ",'8'";

                                        }



                                        //if(DB.DataBaseType.ToLower() == "oracle")
                                        //{
                                        //    key += ",\"ID\"";
                                        //    value += ",(select NVL(MAX(\"ID\"),0)+1 from "+ TableName.Split(',')[i]+@")";
                                        //}

                                        sql = "insert into " + TableName.Split(',')[i] + "(" + key + ") VALUES (" + value + ") ";
                                        sql1 = "insert into SJCS(title) values('" + sql + "')";
                                        DBSys.ExecuteNonQueryOffline(sql1);
                                        DB.ExecuteNonQueryOffline(sql, tmp);
                                        int id = 0;
                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {
                                            id = DB.GetInt32("select id from " + TableName.Split(',')[i] + " where 1=1 " + where + "", tmp);
                                        }
                                        else
                                        {
                                            id = DB.GetInt32("select \"ID\" from " + TableName.Split(',')[i] + " where 1=1 " + where + "", tmp);
                                        }

                                        if (string.IsNullOrEmpty(retId))
                                        {
                                            retId += id.ToString();
                                        }
                                        else
                                        {
                                            retId += "," + id.ToString();
                                        }

                                        tmp.Add("createby", createby);
                                        tmp.Add("createdate", DateTime.Now.ToString("yyyy-MM-dd"));
                                        tmp.Add("createtime", DateTime.Now.ToString("HH:mm:ss"));

                                        if (DB.DataBaseType.ToLower() != "oracle")
                                        {
                                            DB.ExecuteNonQueryOffline(@"
                UPDATE " + DB.ChangeKeyWord(TableName.Split(',')[i]) + @"
                SET createby=@createby,createdate=@createdate,createtime=@createtime
                WHERE id =" + id + @"
                ", tmp);
                                        }
                                        else
                                        {
                                            DB.ExecuteNonQueryOffline(@"
                UPDATE " + DB.ChangeKeyWord(TableName.Split(',')[i]) + @"
                SET createby=:createby,createdate=:createdate,createtime=:createtime " +
"WHERE id =" + id + @"
                ", tmp);
                                        }

                                        if (IsHeadData)
                                        {

                                            Logic.AddModuleOtherData(AppCode, id, ReqObj.UserToken);

                                        }


                                        Logic.SetOtherData(TableName, id, ReqObj.UserToken);

                                        n++;
                                    }
                                    else
                                    {
                                        ret.IsSuccess = false;
                                        ret.ErrMsg = "Data duplication！";
                                        return ret;
                                    }
                                }

                            }
                            if (n == 0)
                            {
                                ret.IsSuccess = false;
                                //ret.ErrMsg = "添加失败！";
                                ret.ErrMsg = "Add failed！";
                            }
                            else
                            {
                                ret.IsSuccess = true;
                                ret.RetData = retId;
                                //ret.ErrMsg = "添加成功！";
                                ret.ErrMsg = "Added successfully！";
                            }

                        }
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "The primary key data of the table is not set！";
                            //ret.ErrMsg = "没有设置表的主键数据！";
                            return ret;
                        }

                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "传参为空！";
                    ret.ErrMsg = "Pass parameter is empty！";
                }

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
        /// <summary>
        /// 修改模块数据接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject EditModuleData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DBSys = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                var o = JObject.Parse(Data);
                string TableName = jarr["TableName"].ToString();//表名
                string APP_Code = jarr["APP_Code"].ToString();
                string modfiyby = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                bool IsHeadData = false;

                bool IsDoc = false;

                foreach (string s in SJeMES_Framework_NETCore.Web.System.DocModules.Split(","))
                {
                    if (APP_Code.ToLower() == s.ToLower())
                    {
                        IsDoc = true;


                    }
                }


                if (!string.IsNullOrEmpty(TableName))
                {
                    for (int i = 0; i < TableName.Split(',').Length; i++)
                    {

                        string Name = o[TableName.Split(',')[i]].ToString();
                        int n = 0;


                        ArrayList arrayList = Newtonsoft.Json.JsonConvert.DeserializeObject<ArrayList>(Name);
                        if (arrayList.Count > 0)
                        {
                            foreach (object arr in arrayList)
                            {
                                Dictionary<string, object> tmp = JsonConvert.DeserializeObject<Dictionary<string, object>>(arr.ToString());

                                ///检查数据合法性
                                Logic.CheckData(TableName, string.Empty, tmp, ReqObj.UserToken);

                                string key = string.Empty;
                                string value = string.Empty;
                                string where = string.Empty;

                                #region 是否表头
                                string code = DBSys.GetString(@"select APP_Code from SYSAPP01M where App_TableH='" + TableName.Split(',')[i] + @"'");

                                ///是否表头
                                if (!string.IsNullOrEmpty(code))
                                {
                                    IsHeadData = true;
                                }
                                #endregion

                                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                                foreach (string current in tmp.Keys)
                                {
                                    dictionary.Add(current, tmp[current].ToString().Trim());
                                }

                                foreach (string current in dictionary.Keys)
                                {


                                    if (current != "RN" && current != "timestamp" && current != "id")
                                    {
                                        if (IsDoc && current.ToLower() == "status")
                                        {

                                        }
                                        else

                                        {
                                            if (string.IsNullOrEmpty(key))
                                            {
                                                //key = current + "='" + dictionary[current].ToString().Trim() + "'";
                                                key = " " + DB.ChangeKeyWord(current) + " " + "=@" + current;
                                            }
                                            else
                                            {
                                                //key += "," + current + "='" + dictionary[current].ToString().Trim() + "'";
                                                key += "," + DB.ChangeKeyWord(current) + "=@" + current;
                                            }
                                        }
                                    }

                                }

                                key += ",modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime";

                                Dictionary<string, object> OldData = DB.GetDictionary(@"SELECT * FROM " + DB.ChangeKeyWord(TableName.Split(',')[i]) + @" where id=" + dictionary["id"].ToString());

                                #region 添加修改信息
                                if (dictionary.ContainsKey("modifyby"))
                                {
                                    dictionary["modifyby"] = modfiyby;
                                }
                                else
                                {
                                    dictionary.Add("modifyby", modfiyby);
                                }
                                if (dictionary.ContainsKey("modifydate"))
                                {
                                    dictionary["modifydate"] = DateTime.Now.ToString("yyyy-MM-dd");
                                }
                                else
                                {
                                    dictionary.Add("modifydate", DateTime.Now.ToString("yyyy-MM-dd"));
                                }
                                if (dictionary.ContainsKey("modifytime"))
                                {
                                    dictionary["modifytime"] = DateTime.Now.ToString("HH:mm:ss");
                                }
                                else
                                {
                                    dictionary.Add("modifytime", DateTime.Now.ToString("HH:mm:ss"));
                                }
                                #endregion




                                //UPDATE 表名称 SET 列名称 = 新值 WHERE 列名称 = 某值
                                string sql = "UPDATE " + TableName.Split(',')[i] + " SET " + key + " WHERE id=" + dictionary["id"].ToString() + "";
                                //WriteProgramLog(sql);
                                //string sql1 = "insert into SJCS(title) values('" + sql + "')";
                                //DBSys.ExecuteNonQueryOffline(sql1);
                                DB.ExecuteNonQueryOffline(sql, dictionary);


                                if (IsHeadData)
                                {
                                    Logic.EditModuleOtherData(APP_Code, Convert.ToInt32(dictionary["id"].ToString()), OldData, ReqObj.UserToken);
                                }

                                Logic.SetOtherData(TableName.Split(',')[i], Convert.ToInt32(dictionary["id"].ToString()), ReqObj.UserToken);

                                n++;

                            }

                        }
                        if (n == 0)
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "Fail to edit！";
                           // ret.ErrMsg = "修改失败！";
                        }
                        else
                        {
                            ret.IsSuccess = true;
                           // ret.ErrMsg = "修改成功！";
                            ret.ErrMsg = "Successfully modified！";
                        }



                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "传参为空！";
                    ret.ErrMsg = "Pass parameter is empty！";
                }

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
        /// 获取模块数据接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetModuleData22(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();
                string usercode = string.Empty;
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                usercode = SJeMES_Framework_NETCore.Web.System.GetUserCode(ReqObj);
                var o = JObject.Parse(Data);
                string TableName = jarr["TableName"].ToString();//表名
                //string title = jarr["title"].ToString();
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                string APP_Code = jarr["APP_Code"].ToString();//CODE


                string pj = string.Empty;
                Where = Where + " and createby in (" + usercode + ")";
                if (!string.IsNullOrEmpty(TableName))
                {
                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();
                    int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                    if (Where.Contains("@ALL"))
                    {
                        var index = Where.LastIndexOf('@');
                        Where = Where.Substring(index + 1, Where.Length - index - 1);

                        string sql2 = string.Empty;

                        if (DB.DataBaseType.ToLower() == "mysql")
                        {
                            sql2 = "select * from " + TableName.Trim() + " where 1=1 limit 1";
                        }
                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                        {
                            sql2 = "select top(1) * from " + TableName.Trim() + " where 1=1";
                        }
                        else if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql2 = "select * from " + TableName.Trim() + " where rownum=1";
                        }


                        DataTable dt2 = DB.GetDataTable(sql2);

                        foreach (DataColumn item in dt2.Columns)
                        {
                            if (item.ColumnName.ToLower() == "guid" ||
                                item.ColumnName.ToLower() == "timestamp" ||
                                item.ColumnName.ToLower() == "id")
                            {

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(pj))
                                {
                                    if (DB.DataBaseType.ToLower() == "mysql")
                                    {
                                        pj = " and (`" + item.ColumnName + "` like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "sqlserver")
                                    {
                                        pj = " and ([" + item.ColumnName + "] like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "oracle")
                                    {
                                        pj = " and (" + item.ColumnName + " like " + "'%" + Where + "%' ";
                                    }
                                }
                                else
                                {
                                    if (DB.DataBaseType.ToLower() == "mysql")
                                    {
                                        pj += "or `" + item.ColumnName + "` like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "sqlserver")
                                    {
                                        pj += "or [" + item.ColumnName + "] like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "oracle")
                                    {
                                        pj += "or " + item.ColumnName + " like " + "'%" + Where + "%' ";
                                    }


                                }
                            }

                        }
                        pj += ") ";

                    }
                    else
                    {
                        pj = Where;
                    }

                    if (string.IsNullOrEmpty(OrderBy))
                    {
                        OrderBy = " order by id desc ";
                    }

                    string sql = string.Empty;
                    if (!string.IsNullOrWhiteSpace(PageRow))
                    {
                        if (DB.DataBaseType.ToLower() == "mysql")
                        {
                            sql = @"select * from (
select M.*,@n:= @n + 1 as RN from " + TableName + @" M,(select @n:= 0) d
where 1=1 " + pj + @"
" + OrderBy + @")
tab where  RN > " + total + "  limit " + PageRow + "";
                        }
                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                        {
                            sql = @"
Select top(" + PageRow + @") *
from
(
select
row_number() 
over(" + OrderBy + @") as rownumber,*
 from " + TableName + @"
where 1=1 " + pj + @") tmp
where rownumber> " + total + @"
";
                        }
                        else if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql = @"select * from (
select M.*,ROWNUM as RN from " + TableName + @" M
where 1=1 " + pj + @"
" + OrderBy + @"
)
tab where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(PageRow)).ToString();
                        }


                        dt = DB.GetDataTable(sql);
                        total = DB.GetInt32("select count(1) from " + TableName + " where 1=1 " + pj + "");
                    }
                    else
                    {
                        sql = @"
SELECT * FROM " + TableName + @" where 1=1 " + pj + @" " + OrderBy + @" 
";
                    }
                    //if (dt.Rows.Count > 0)
                    //{
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    //json = json.Replace("TRUE", "是").Replace("True", "是").Replace("true", "是");
                    //json = json.Replace("FALSE", "否").Replace("False", "否").Replace("false", "否");
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add(TableName, json);
                    p.Add("Total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                    //}
                    //else
                    //{
                    //    ret.IsSuccess = false;
                    //    ret.ErrMsg = "";
                    //}
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Pass parameter is empty！";
                    //ret.ErrMsg = "传参为空！";
                }

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
        /// 获取模块数据接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetModuleData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
               
                var o = JObject.Parse(Data);
                string TableName = jarr["TableName"].ToString();//表名
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                string APP_Code = jarr["APP_Code"].ToString();//CODE
                string pj = string.Empty;
                if (!string.IsNullOrEmpty(TableName))
                {
                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();
                    int total = (int.Parse(Page) - 1) * int.Parse(PageRow);
                    if (Where.Contains("@ALL"))
                    {
                        var index = Where.LastIndexOf('@');
                        Where = Where.Substring(index + 1, Where.Length - index - 1);

                        string sql2 = string.Empty;

                        if (DB.DataBaseType.ToLower() == "mysql")
                        {
                            sql2 = "select * from " + TableName.Trim() + " where 1=1 limit 1";
                        }
                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                        {
                            sql2 = "select top(1) * from " + TableName.Trim() + " where 1=1";
                        }
                        else if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql2 = "select * from " + TableName.Trim() + " where rownum=1";
                        }


                        DataTable dt2 = DB.GetDataTable(sql2);

                        foreach (DataColumn item in dt2.Columns)
                        {
                            if (item.ColumnName.ToLower() == "guid" ||
                                item.ColumnName.ToLower() == "timestamp" ||
                                item.ColumnName.ToLower() == "id")
                            {

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(pj))
                                {
                                    if (DB.DataBaseType.ToLower() == "mysql")
                                    {
                                        pj = " and (`" + item.ColumnName + "` like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "sqlserver")
                                    {
                                        pj = " and ([" + item.ColumnName + "] like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "oracle")
                                    {
                                        pj = " and (" + item.ColumnName + " like " + "'%" + Where + "%' ";
                                    }
                                }
                                else
                                {
                                    if (DB.DataBaseType.ToLower() == "mysql")
                                    {
                                        pj += "or `" + item.ColumnName + "` like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "sqlserver")
                                    {
                                        pj += "or [" + item.ColumnName + "] like " + "'%" + Where + "%' ";
                                    }
                                    else if (DB.DataBaseType.ToLower() == "oracle")
                                    {
                                        pj += "or " + item.ColumnName + " like " + "'%" + Where + "%' ";
                                    }


                                }
                            }

                        }
                        pj += ") ";

                    }
                    else
                    {
                        pj = Where;
                    }

                    if (string.IsNullOrEmpty(OrderBy))
                    {
                        OrderBy = " order by id desc ";
                    }

                    string sql = string.Empty;
                    if (!string.IsNullOrWhiteSpace(PageRow))
                    {
                        if (DB.DataBaseType.ToLower() == "mysql")
                        {
                            sql = @"select * from (
select M.*,@n:= @n + 1 as RN from " + TableName + @" M,(select @n:= 0) d
where 1=1 " + pj + @"
" + OrderBy + @")
tab where  RN > " + total + "  limit " + PageRow + "";
                        }
                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                        {
                            sql = @"
Select top(" + PageRow + @") *
from
(
select
row_number() 
over(" + OrderBy + @") as rownumber,*
 from " + TableName + @"
where 1=1 " + pj + @") tmp
where rownumber> " + total + @"
";
                        }
                        else if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            sql = @"select * from (
select M.*,ROWNUM as RN from " + TableName + @" M
where 1=1 " + pj + @"
" + OrderBy + @"
)
tab where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(PageRow)).ToString();
                        }


                        dt = DB.GetDataTable(sql);
                        total = DB.GetInt32("select count(1) from " + TableName + " where 1=1 " + pj + "");
                    }
                    else
                    {
                        sql = @"
SELECT * FROM " + TableName + @" where 1=1 " + pj + @" " + OrderBy + @" 
";
                    }
                    //if (dt.Rows.Count > 0)
                    //{
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                    //json = json.Replace("TRUE", "是").Replace("True", "是").Replace("true", "是");
                    //json = json.Replace("FALSE", "否").Replace("False", "否").Replace("false", "否");
                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add(TableName, json);
                    p.Add("Total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;
                    //}
                    //else
                    //{
                    //    ret.IsSuccess = false;
                    //    ret.ErrMsg = "";
                    //}
                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "传参为空！";
                    ret.ErrMsg = "Pass parameter is empty！";
                }

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
        /// 获取模块对应表身数据接口
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetModuleBodyData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                var o = JObject.Parse(Data);

                string reqkey = "TableName,Seq,Where,OrderBy,Page,PageRow,APP_Code,HeadId";

                Dictionary<string, object> reqP = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(reqkey, ReqObj);


                string pj = string.Empty;
                if (!string.IsNullOrEmpty(reqP["TableName"].ToString()))
                {
                    DataTable dt = new DataTable();
                    DataTable dt1 = new DataTable();
                    string Where = string.Empty;

                    #region where条件
                    if (reqP["Where"].ToString().Contains("@ALL"))
                    {
                        var index = reqP["Where"].ToString().LastIndexOf('@');
                        Where = reqP["Where"].ToString().Substring(index + 1, reqP["Where"].ToString().Length - index - 1);
                        string sql2 = "select * from " + reqP["TableName"].ToString().Trim() + " where 1=1 limit 1";
                        DataTable dt2 = DB.GetDataTable(sql2);

                        foreach (DataColumn item in dt2.Columns)
                        {
                            if (item.ColumnName.ToLower() == "guid" ||
                                item.ColumnName.ToLower() == "timestamp" ||
                                item.ColumnName.ToLower() == "id")
                            {

                            }
                            else
                            {
                                if (string.IsNullOrEmpty(pj))
                                {
                                    pj = " and (" + item.ColumnName + " like " + "'%" + Where + "%' ";
                                }
                                else
                                {
                                    pj += "or " + item.ColumnName + " like " + "'%" + Where + "%' ";
                                }
                            }

                        }
                        pj += ") ";

                    }
                    else
                    {
                        pj = reqP["Where"].ToString();
                    }
                    #endregion

                    #region 获取和表头绑定关系


                    string headTable = DB2.GetString(@"select App_TableH from SYSAPP01M where APP_Code=@APP_Code", reqP);

                    string sheadkeys = DB2.GetString(@"Select HeadKeys FROM SYSAPP01A1 where Seq=@Seq AND APP_Code=@APP_Code", reqP);

                    sheadkeys = sheadkeys.Replace("[", "").Replace("]", "").Replace("\"", "");
                    string[] jo;
                    if (sheadkeys.IndexOf(",") > -1)
                    {
                        jo = sheadkeys.Split(',');
                    }
                    else
                    {
                        jo = new string[1] { sheadkeys };
                    }





                    Dictionary<string, string> djo = new Dictionary<string, string>();
                    sheadkeys = string.Empty;
                    foreach (string s in jo)
                    {
                        if (DB.DataBaseType.ToLower() != "oracle")
                        {
                            djo.Add(s.Split(':')[0], s.Split(':')[1]);
                            sheadkeys += s.Split(':')[0] + ",";
                        }
                        else
                        {
                            djo.Add(s.Split(':')[0].ToUpper(), s.Split(':')[1]);
                            sheadkeys += s.Split(':')[0].ToUpper() + ",";
                        }

                    }

                    sheadkeys = sheadkeys.Remove(sheadkeys.Length - 1);

                    Dictionary<string, string> dHeadKeys = new Dictionary<string, string>();

                    DataTable dtHeadKeys = DB.GetDataTable(@"select " + sheadkeys + @" from " + DB.ChangeKeyWord(headTable) + @" where id=@HeadId", reqP);

                    if (dtHeadKeys.Rows.Count > 0)
                    {
                        foreach (DataColumn dc in dtHeadKeys.Columns)
                        {
                            if (!reqP["TableName"].ToString().ToLower().Contains("select"))
                            {
                                pj += " AND " + DB.ChangeKeyWord(djo[dc.ColumnName]) + @"='" + dtHeadKeys.Rows[0][dc.ColumnName].ToString() + @"' ";
                                //string sql1 = "insert into SJCS(title) values('ColumnName:" + DB.ChangeKeyWord(djo[dc.ColumnName]) + "')";
                                //DB2.ExecuteNonQueryOffline(sql1);
                                //sql1 = "insert into SJCS(title) values('dtHeadKeys:" + dtHeadKeys.Rows[0][dc.ColumnName].ToString() + "')";
                                //DB2.ExecuteNonQueryOffline(sql1);
                            }
                            else
                            {
                                if (!reqP.ContainsKey(djo[dc.ColumnName]))
                                {
                                    reqP["TableName"] = reqP["TableName"].ToString().Replace(djo[dc.ColumnName], dtHeadKeys.Rows[0][dc.ColumnName].ToString());
                                }
                            }
                        }
                    }

                    #endregion


                    #region 单据表身排序

                    string sysOrderBy = string.Empty;

                    if (!reqP["TableName"].ToString().ToLower().Contains("select"))
                    {
                        if (DB.DataBaseType.ToLower() == "mysql")
                        {
                            if (DB.GetInt32("select count(1) from information_schema.COLUMNS WHERE COLUMN_NAME='sorting' and TABLE_SCHEMA='" + DB.DataBaseName + "' and TABLE_NAME='" + reqP["TableName"].ToString() + "'") == 1)
                            {
                                sysOrderBy = " ORDER BY sorting asc ";
                            }
                            if (DB.GetInt32("select count(1) from information_schema.COLUMNS WHERE COLUMN_NAME='serial_number' and TABLE_SCHEMA='" + DB.DataBaseName + "' and TABLE_NAME='" + reqP["TableName"].ToString() + "'") == 1)
                            {
                                sysOrderBy = " ORDER BY serial_number asc ";
                            }
                            if (DB.GetInt32("select count(1) from information_schema.COLUMNS WHERE COLUMN_NAME='seq' and TABLE_SCHEMA='" + DB.DataBaseName + "' and TABLE_NAME='" + reqP["TableName"].ToString() + "'") == 1)
                            {
                                sysOrderBy = " ORDER BY seq asc ";
                            }
                        }
                        else if (DB.DataBaseType.ToLower() == "sqlserver")
                        {
                            if (DB.GetInt32("select   count(1)  from   syscolumns   where   id=object_id('" + reqP["TableName"].ToString() + "')   and   name='sorting'") == 1)
                            {
                                sysOrderBy = " ORDER BY sorting asc ";
                            }
                            if (DB.GetInt32("select   count(1)  from   syscolumns   where   id=object_id('" + reqP["TableName"].ToString() + "')   and   name='serial_number'") == 1)
                            {
                                sysOrderBy = " ORDER BY serial_number asc ";
                            }
                            if (DB.GetInt32("select   count(1)  from   syscolumns   where   id=object_id('" + reqP["TableName"].ToString() + "')   and   name='seq'") == 1)
                            {
                                sysOrderBy = " ORDER BY seq asc ";
                            }
                        }

                        else if (DB.DataBaseType.ToLower() == "oracle")
                        {
                            if (DB.GetInt32("SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + reqP["TableName"].ToString().ToUpper() + "' AND COLUMN_NAME = 'SORTING'") == 1)
                            {
                                sysOrderBy = " ORDER BY sorting asc ";
                            }
                            if (DB.GetInt32("SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + reqP["TableName"].ToString().ToUpper() + "' AND COLUMN_NAME = 'SERIAL_NUMBER'") == 1)
                            {
                                sysOrderBy = " ORDER BY serial_number asc ";
                            }
                            if (DB.GetInt32("SELECT COUNT(1) FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '" + reqP["TableName"].ToString().ToUpper() + "' AND COLUMN_NAME = 'SEQ'") == 1)
                            {
                                sysOrderBy = " ORDER BY seq asc ";
                            }
                        }
                    }

                    if (reqP["OrderBy"].ToString().ToLower().Trim() == "order by id desc")
                    {
                        reqP["OrderBy"] = sysOrderBy;
                    }

                    #endregion
                    int total = 0;
                    string sql = string.Empty;
                    string orderby = string.Empty;
                    if (!reqP["TableName"].ToString().ToLower().Contains("select"))
                    {
                        #region 有表名
                        if (!string.IsNullOrWhiteSpace(reqP["PageRow"].ToString()))
                        {
                            total = (int.Parse(reqP["Page"].ToString()) - 1) * int.Parse(reqP["PageRow"].ToString());
                            if (DB.DataBaseType.ToLower() == "mysql")
                            {
                                sql = @"select * from (
select M.*,@n:= @n + 1 as RN from " + reqP["TableName"].ToString() + @" M,(select @n:= 0) d
where 1=1 " + pj + @"
" + reqP["OrderBy"].ToString() + @")
tab where  RN > " + total + "  limit " + reqP["PageRow"].ToString() + "";
                            }
                            else if (DB.DataBaseType.ToLower() == "sqlserver")
                            {
                                sql = @"select top(" + reqP["PageRow"].ToString() + @") * from (
select M.*,row_number() over (" + reqP["OrderBy"].ToString() + @") as rn from " + reqP["TableName"].ToString() + @" M
where 1=1 " + pj + @"
)
tab where rn >" + total;
                            }
                            else if (DB.DataBaseType.ToLower() == "oracle")
                            {
                                sql = @"select * from (
select M.*,ROWNUM  as RN from " + reqP["TableName"].ToString() + @" M
where 1=1 " + pj + @"
" + reqP["OrderBy"].ToString() + @"
)
tab where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(reqP["PageRow"].ToString())).ToString();
                            }
                            dt = DB.GetDataTable(sql);

                            //string sql1 = "insert into SJCS(title) values('1：" + dt.Rows.Count + "')";
                            //DB2.ExecuteNonQueryOffline(sql1);

                            total = DB.GetInt32("select count(1) from " + reqP["TableName"].ToString().Trim() + " where 1=1 " + pj + "");
                        }
                        else
                        {

                            if (reqP["TableName"].ToString().Trim()== "MES030A4")
                            {
                                orderby = " order by date_fix";
                            }
                            else if (reqP["TableName"].ToString().Trim() == "MES030A5")
                            {
                                orderby = " order by date_fix";
                            }
                            else
                            {
                                orderby = reqP["OrderBy"].ToString();
                            }
                            sql = @"
SELECT * FROM " + reqP["TableName"].ToString().Trim() + @" where 1=1 " + pj + @" " + orderby + @" 
";
                            dt = DB.GetDataTable(sql);

                            //string sql1 = "insert into SJCS(title) values('TableName:" + reqP["TableName"].ToString().Trim() + "')";
                            //DB2.ExecuteNonQueryOffline(sql1);
                          
                            //sql1 = "insert into SJCS(title) values('OrderBy:" + reqP["OrderBy"].ToString() + "')";
                            //DB2.ExecuteNonQueryOffline(sql1);

                            total = DB.GetInt32("select count(1) from " + reqP["TableName"].ToString().Trim() + " where 1=1 " + pj + "");
                        }
                        #endregion
                    }
                    else
                    {
                        #region 没表名
                        if (!string.IsNullOrWhiteSpace(reqP["PageRow"].ToString()))
                        {
                            total = (int.Parse(reqP["Page"].ToString()) - 1) * int.Parse(reqP["PageRow"].ToString());
                            if (DB.DataBaseType.ToLower() == "mysql")
                            {
                                sql = @"select * from (
select M.*,@n:= @n + 1 as RN from (" + reqP["TableName"].ToString() + @") M,(select @n:= 0) d
where 1=1 " + pj + @"
" + reqP["OrderBy"].ToString() + @")
tab where  RN > " + total + "  limit " + reqP["PageRow"].ToString() + "";
                            }
                            else if (DB.DataBaseType.ToLower() == "sqlserver")
                            {
                                sql = @"select top(" + reqP["PageRow"].ToString() + @") * from (
select M.*,row_number() over (" + reqP["OrderBy"].ToString() + @") as rn from (" + reqP["TableName"].ToString() + @") M
where 1=1 " + pj + @"
)
tab where rn >" + total;
                            }
                            else if (DB.DataBaseType.ToLower() == "oracle")
                            {
                                sql = @"select * from (
select M.*,ROWNUM  as RN from (" + reqP["TableName"].ToString() + @") M
where 1=1 " + pj + @"
" + reqP["OrderBy"].ToString() + @"
)
tab where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(reqP["PageRow"].ToString())).ToString();
                            }
                            dt = DB.GetDataTable(sql);

                            //string sql1 = "insert into SJCS(title) values('3:" + dt.Rows.Count + "')";
                            //DB2.ExecuteNonQueryOffline(sql1);

                            total = DB.GetInt32("select count(1) from (" + reqP["TableName"].ToString().Trim() + ") m where 1=1 " + pj + "");
                        }
                        else
                        {
                            if (reqP["TableName"].ToString().Trim() == "MES030A4")
                            {
                                orderby = " order by date_fix";
                            }
                            else if (reqP["TableName"].ToString().Trim() == "MES030A5")
                            {
                                orderby = " order by date_fix";
                            }
                            else
                            {
                                orderby = reqP["OrderBy"].ToString();
                            }
                            sql = @"
SELECT * FROM (" + reqP["TableName"].ToString().Trim() + @") m where 1=1 " + pj + @" " + orderby + @" 
";
                            dt = DB.GetDataTable(sql);

                            //string sql1 = "insert into SJCS(title) values('4:" + dt.Rows.Count + "')";
                            //DB2.ExecuteNonQueryOffline(sql1);

                            total = DB.GetInt32("select count(1) from (" + reqP["TableName"].ToString().Trim() + ") m where 1=1 " + pj + "");
                        }
                        #endregion
                    }

                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                    json = json.Replace("TRUE", "是").Replace("True", "是").Replace("true", "是");
                    json = json.Replace("FALSE", "否").Replace("False", "否").Replace("false", "否");
                    List<string> heads = new List<string>();
                    foreach (DataColumn dc in dt.Columns)
                    {
                        heads.Add(dc.ColumnName);
                    }

                    Dictionary<string, object> p = new Dictionary<string, object>();
                    p.Add("Heads", heads);
                    p.Add("Data", json);
                    p.Add("Total", total);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    ret.IsSuccess = true;

                }
                else
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "传参为空！";
                    ret.ErrMsg = "Pass parameter is empty！";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }
        #endregion



        /// <summary>
        /// 获取视图数据（看板、报表）
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetViewData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string ViewName = jarr["View"].ToString();//看板/报表名称
                string Where = jarr["Where"].ToString();//条件
                string OrderBy = jarr["OrderBy"].ToString();//排序
                string Page = jarr["Page"].ToString();//页数
                string PageRow = jarr["PageRow"].ToString();//行数
                if (!string.IsNullOrEmpty(ViewName))
                {
                    Where = SJeMES_Framework_NETCore.Common.StringHelper.GetWhereWithAll(@"
SELECT * from `" + ViewName + @"` ", Where, ReqObj.UserToken);


                    int total2 = (int.Parse(Page) - 1) * int.Parse(PageRow);
                    int total = DB.GetInt32("select count(1) from `" + ViewName + "` where 1=1 " + Where + "");




                    if (total > 0)
                    {
                        string sql = @"select * from (
select M.*,@n:= @n + 1 as RN from `" + ViewName + "` M,(select @n:= 0) d) tab where  RN > " + total2 + " " + Where + " " + OrderBy + " limit " + PageRow + "";
                        DataTable dt = DB.GetDataTable(sql);
                        string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                        json = json.Replace("TRUE", "是").Replace("True", "是").Replace("true", "是");
                        json = json.Replace("FALSE", "否").Replace("False", "否").Replace("false", "否");
                        json = json.Replace(".000000", "").Replace(".0000", "").Replace(".00", "");
                        string headdata = string.Empty;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            headdata += dc.ColumnName + @",";
                        }
                        headdata.Remove(headdata.Length - 1);

                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("headdata", headdata);
                        p.Add("data", json);
                        p.Add("total", total);
                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Kanban/report name cannot be empty！";
                    //ret.ErrMsg = "看板/报表名称不能为空！";
                }

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
        /// 获取服务器公钥
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetServerRASKey(SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj)
        {
            SJeMES_Framework_NETCore.WebAPI.ResultObject Ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            try
            {
                SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string RASKey = DB.GetString("SELECT Key1 FROM rasinfo");

                if (string.IsNullOrEmpty(RASKey))
                {
                    string[] RASKeys = SJeMES_Framework_NETCore.Common.RASHelper.CreateRAS();

                    string sql = @"
INSERT INTO rasinfo
(Key1,Key2)
VALUES
('" + RASKeys[0] + @"','" + RASKeys[1] + @"')
";
                    DB.ExecuteNonQueryOffline(sql);

                    RASKey = RASKeys[0];
                }

                Ret.IsSuccess = true;

                //Dictionary<string, string> p = new Dictionary<string, string>();
                //p.Add("RasKey", RASKey);
                //RASKey = SJeMES_Framework_NETCore.Common.RASHelper.RSAPublicKeyDotNet2Java(RASKey);
                Ret.RetData = RASKey;

            }
            catch (Exception ex)
            {
                Ret.IsSuccess = false;
                Ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", Ret);
            }

            return Ret;
        }

        /// <summary>
        /// GetOrg(获取账套信息)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetOrg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {


                #region 参数


                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);



                string sql = string.Empty;


                sql = @"
SELECT * FROM SYSORG01M
";


                DataTable dt = DB.GetDataTable(sql);



                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }


        /// <summary>
        /// 根据用户获取PDA权限
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPDAPermissionsNew(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string UserCode = jarr["UserCode"].ToString();
                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                Dictionary<string, string> p = new Dictionary<string, string>();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string sql = @"SELECT
SYSAPP03M.AppCode AS '模块代号',
SYSAPP03M.AppName AS '模块名称',
SYSAPP03M.url ,
SYSAPP03M.tag ,
IFNULL(Select,'False') AS '权限'
FROM SYSAPP03M left join SYSUSER06M on SYSUSER06M.UserCode = '" + UserCode + @"' and SYSAPP03M.AppCode = SYSUSER06M.AppCode  collate Chinese_PRC_90_CI_AI
where SYSAPP03M.AppCode like '%PDA%'";

                DataTable dt = DB.GetDataTable(sql);
                string json = "{\"category\": [\"bmsrk\",\" bmsck\",  \"bmsmg\",\" bmszz\", \" bmszl\",\"  bmsbj\", \" bmsjh\",\" bmsms\",\"bmsjit\"] ,\"results\": {";
                string bmsmg = "\"bmsmg\":[";
                string bmsck = "\"bmsck\":[";
                string bmsrk = "\"bmsrk\":[";
                string bmsjit = "\"bmsjit\":[";
                string bmsms = "\"bmsms\":[";
                string bmsjh = "\"bmsjh\":[";
                string bmsbj = "\"bmsbj\":[";
                string bmszl = "\"bmszl\":[";
                string bmszz = "\"bmszz\":[";
                bool aa = false;
                bool bb = false;
                bool cc = false;
                bool dd = false;
                bool ee = false;
                bool ff = false;
                bool gg = false;
                bool hh = false;
                bool ii = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (Convert.ToBoolean(dt.Rows[i]["权限"].ToString()) == true)
                    {
                        if (dt.Rows[i]["模块代号"].ToString().Contains("PDA_"))
                        {
                            string[] list = dt.Rows[i]["模块代号"].ToString().Split('_');
                            string ordername = list[1];
                            string name = dt.Rows[i]["模块名称"].ToString();
                            string id = list[2];
                            string url = dt.Rows[i]["url"].ToString();
                            string tag = dt.Rows[i]["tag"].ToString();
                            if (ordername == "data")//bmsmg
                            {
                                aa = true;


                                bmsmg += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/Code.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"data" + id + "\"},";


                            }
                            if (ordername == "out")//bmsck
                            {
                                bb = true;

                                bmsck += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/PlanOutStock.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"out" + id + "\"},";



                            }
                            if (ordername == "storage")//bmsrk
                            {
                                cc = true;


                                bmsrk += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/PlanInStock.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"storage" + id + "\"},";

                            }

                            if (ordername == "jit")//bmsjit
                            {
                                dd = true;


                                bmsjit += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/AllotIn.png\",\"type\":\"IN\",\"tag\":\"\"" + tag + ",\"url\":\"" + url + "\",\"mark\":\"jit" + id + "\"},";

                            }
                            if (ordername == "equipment")//bmsms
                            {
                                ee = true;


                                bmsms += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/default.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"equipment" + id + "\"},";

                            }
                            if (ordername == "picking")//bmsjh
                            {
                                ff = true;

                                bmsjh += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/PlanOutStock.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"picking" + id + "\"},";

                            }
                            if (ordername == "warn")//bmsbj
                            {
                                gg = true;

                                bmsbj += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/default.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"warn" + id + "\"},";


                            }
                            if (ordername == "qc")//bmszl
                            {
                                hh = true;


                                bmszl += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/AllotIn.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"qc" + id + "\"},";

                            }
                            if (ordername == "production")//bmszz
                            {
                                ii = true;

                                bmszz += "{\"name\":\"" + name + "\",\"img_url\":\"app/icon/PlanOutStock.png\",\"type\":\"IN\",\"tag\":\"" + tag + "\",\"url\":\"" + url + "\",\"mark\":\"production" + id + "\"},";

                            }
                        }
                    }
                }
                if (aa)
                {
                    bmsmg = bmsmg.Substring(0, bmsmg.Length - 1);
                }
                if (bb)
                {
                    bmsck = bmsck.Substring(0, bmsck.Length - 1);
                }
                if (cc)
                {
                    bmsrk = bmsrk.Substring(0, bmsrk.Length - 1);
                }
                if (dd)
                {
                    bmsjit = bmsjit.Substring(0, bmsjit.Length - 1);
                }
                if (ee)
                {
                    bmsms = bmsms.Substring(0, bmsms.Length - 1);
                }
                if (ff)
                {
                    bmsjh = bmsjh.Substring(0, bmsjh.Length - 1);
                }
                if (gg)
                {
                    bmsbj = bmsbj.Substring(0, bmsbj.Length - 1);
                }
                if (hh)
                {
                    bmszl = bmszl.Substring(0, bmszl.Length - 1);
                }
                if (ii)
                {
                    bmszz = bmszz.Substring(0, bmszz.Length - 1);
                }
                bmsmg += "],";
                bmsck += "],";
                bmsrk += "],";
                bmsjit += "],";
                bmsjh += "],";
                bmsbj += "],";
                bmszl += "],";
                bmsms += "],";
                bmszz += "]";
                json += bmsmg + bmsck + bmsrk + bmsjit + bmsjh + bmsbj + bmszl + bmsmg + bmszz + "}}";
                p.Add("JSON", json);
                ret.RetData = "<JSON>" + json + "</JSON>";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }



            return ret;
        }

        /// <summary>
        /// GetPDAVer(获取PDA版本)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPDAVer(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数


                #endregion

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                Dictionary<string, string> p = new Dictionary<string, string>();


                List<SJeMES_Framework_NETCore.Common.IOHelper.File> files = new List<SJeMES_Framework_NETCore.Common.IOHelper.File>();
                files = SJeMES_Framework_NETCore.Common.IOHelper.GetAllFile(System.AppDomain.CurrentDomain.BaseDirectory + @"\bin\App", files, 0);

                string Var1 = string.Empty;
                string Var2 = string.Empty;

                foreach (SJeMES_Framework_NETCore.Common.IOHelper.File f in files)
                {
                    if (f.Name.StartsWith("pda_wms"))
                    {
                        try
                        {
                            string[] s = f.Name.Replace(".apk", "").Split('_');

                            Var1 = s[2];
                            Var2 = s[3];
                            ret.IsSuccess = true;
                            p.Add("Var1", Var1);
                            p.Add("Var2", Var2);
                            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                        }
                        catch
                        {
                            ret.ErrMsg = "PDA program naming is not standardized";
                            //ret.ErrMsg = "PDA程序命名不规范";
                            string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
                        }
                    }
                }

                if (string.IsNullOrEmpty(Var1))
                {
                    ret.ErrMsg = "No PDA program";
                    //ret.ErrMsg = "没有PDA程序";
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }


        /// <summary>
        /// SaveMenuLog(记录模块使用)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveMenuLog(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string MenuName = jarr["MenuName"].ToString();
                string UserCode = jarr["UserCode"].ToString();
                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                string sql = @"
DELETE FROM SYSUSER03M WHERE AppCode='" + MenuName + @"' AND UserCode='" + UserCode + @"'
INSERT INTO SYSUSER03M
(UserCode,AppCode,DateTime)
Values
('" + UserCode + @"','" + MenuName + @"','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"')
";
                DB.ExecuteNonQueryOffline(sql);

                sql = @"
UPDATE SYSUSER04M SET Times=Times+1 WHERE AppCode='" + MenuName + @"' AND UserCode='" + UserCode + @"' 
";
                if (DB.ExecuteNonQueryOffline(sql) == 0)
                {
                    sql = @"INSERT INTO SYSUSER04M
(UserCode, AppCode, Times)
Values
('" + UserCode + @"', '" + MenuName + @"', 1)
";
                    DB.ExecuteNonQueryOffline(sql);
                }

                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }

        /// <summary>
        /// SaveMenuFav(收藏)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveMenuFav(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string UserCode = jarr["UserCode"].ToString();
                string AppCode = jarr["AppCode"].ToString();
                #endregion

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);



                string sql = @"
DELETE FROM SYSUSER05M WHERE AppCode='" + AppCode + @"' AND UserCode='" + UserCode + @"'  
";
                if (DB.ExecuteNonQueryOffline(sql) == 0)
                {
                    sql = @"INSERT INTO SYSUSER05M
(UserCode, AppCode)
Values
('" + UserCode + @"', '" + AppCode + @"')
";
                    DB.ExecuteNonQueryOffline(sql);
                }

                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }


        /// <summary>
        /// SaveUILanguage(保存多语言记录)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveUILanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string UserCode = jarr["UserCode"].ToString();
                string MenuName = jarr["MenuName"].ToString();
                string MenuText = jarr["MenuText"].ToString();
                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);



                string sql = @"
SELECT * FROM SYSLAN01M (NOLOCK) 
WHERE contorl ='" + MenuName + @"'
";
                if (string.IsNullOrEmpty(DB.GetString(sql)))
                {
                    sql = @"INSERT INTO SYSLAN01M
(contorl, cn,hk,en)
Values
('" + MenuName + @"', '" + MenuText + @"','" + MenuText + "','" + MenuText + @"')
";
                    DB.ExecuteNonQueryOffline(sql);
                }

                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }

        /// <summary>
        /// GetLanguagesType(获取多语言类型)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLanguagesType(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {


                #region 参数


                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);



                string sql = string.Empty;

                if (DB.DataBaseType.ToLower() == "mysql")
                    sql = @"
SELECT * FROM SYSLAN01M limit 1
";
                if (DB.DataBaseType.ToLower() == "sqlserver")
                    sql = @"
SELECT TOP 1 * FROM SYSLAN01M
";
                if (DB.DataBaseType.ToLower() == "oracle")
                    sql = @"
SELECT * FROM SYSLAN01M where rownum=1
";

                DataTable dt = DB.GetDataTable(sql);

                List<string> data = new List<string>();
                foreach (DataColumn dc in dt.Columns)
                {
                    if (dc.ColumnName != "contorl")
                    {
                        data.Add(dc.ColumnName);
                    }
                }

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }

        /// <summary>
        /// GetUILanguage(获取多语言记录)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUILanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string UserCode = jarr["UserCode"].ToString();
                string MenuName = jarr["MenuName"].ToString();
                string MenuText = jarr["MenuText"].ToString();

                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);



                string sql = @"
SELECT * FROM SYSLAN01M 
WHERE contorl ='" + MenuName + @"'
";
                if (string.IsNullOrEmpty(DB.GetString(sql)))
                {
                    sql = @"INSERT INTO SYSLAN01M
(contorl, cn,hk,en)
Values
('" + MenuName + @"', '" + MenuText + @"','" + MenuText + "','" + MenuText + @"')
";
                    DB.ExecuteNonQueryOffline(sql);
                }

                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;
        }

        /// <summary>
        /// 写入PDA多语言信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SetLanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string Type = jarr["Type"].ToString();
                string Key = jarr["Key"].ToString();
                string cn = jarr["cn"].ToString();
                string hk = jarr["hk"].ToString();
                string en = jarr["en"].ToString();
                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                DataTable dt;
                string sql = string.Empty;
                string sql2 = string.Empty;

                if (Type == "1")
                {
                    sql = @"
SELECT contorl  FROM SYSLAN03M   where contorl = '" + Key + "'";
                    dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        sql = @"
update SYSLAN03M  set cn='" + cn + "',hk='" + hk + "',en='" + en + "' where contorl='" + Key + "'";
                    }
                    else
                    {
                        sql = @"
insert into SYSLAN03M(contorl,cn,hk,en) values('" + Key + "','" + cn + "','" + hk + "','" + en + "')";
                    }

                }
                else if (Type == "2")
                {
                    sql = @"
SELECT msg  FROM SYSLAN04M  where msg = '" + Key + "'";
                    dt = DB.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        sql = @"
update SYSLAN04M  set cn='" + cn + "',hk='" + hk + "',en='" + en + "' where msg='" + Key + "'";
                    }
                    else
                    {
                        sql = @"
insert into SYSLAN04M(msg,cn,hk,en) values('" + Key + "','" + cn + "','" + hk + "','" + en + "')";
                    }

                }

                DB.ExecuteNonQueryOffline(sql);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }




            return ret;

        }


        /// <summary>
        /// 获取PDA多语言信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {

                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                string Type = jarr["Type"].ToString();
                string Key = jarr["Key"].ToString();
                string Language = "," + jarr["Language"].ToString();
                if (Language == ",")
                {
                    Language = "";
                }


                #endregion
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                Dictionary<string, string> p = new Dictionary<string, string>();

                System.Data.DataTable dt;

                if (Type == "1")
                {
                    string sql1 = @"
SELECT contorl " + Language + " FROM SYSLAN03M ";
                    dt = DB.GetDataTable(sql1);
                }
                else
                {
                    string sql2 = @"
SELECT msg" + Language + " FROM SYSLAN04M  ";

                    dt = DB.GetDataTable(sql2);
                }


                // string dtXML1 = SJeMES_Framework_NETCore.Common.StringHelper.GetXMLFromDataTable(dt);
                string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(dt);
                if (Type == "1")
                {
                    p.Add("SYSLAN03M", json);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else if (Type == "2")
                {
                    p.Add("SYSLAN04M", json);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                ret.IsSuccess = true;




            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }



            return ret;

        }

        /// <summary>
        /// 获取图表信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetChartsData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string select_SQL = jarr["select_SQL"].ToString();
                string sumField = jarr["sumField"].ToString();
                string groupField = jarr["groupField"].ToString();

                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string sql = string.Empty;

                #region old code
                //                switch (tableName)
                //                {
                //                    case "WMS012A1":
                //                        switch (fieldName)
                //                        {
                //                            case "物料名称":
                //                                sql = @"SELECT BASE007M.material_name as 'name',sum(WMS012A1.qty)as qty 
                //FROM WMS012A1 inner join BASE007M ON WMS012A1.material_no=BASE007M.material_no where WMS012A1.material_no='" + barCode.Trim() + "' GROUP BY BASE007M.material_name";
                //                                break;
                //                            case "物料规格":
                //                                sql = @"SELECT BASE007M.material_specifications  as 'name',sum(WMS012A1.qty) as qty 
                //FROM WMS012A1 inner join BASE007M ON WMS012A1.material_no=BASE007M.material_no where WMS012A1.material_no='" + barCode.Trim() + "' GROUP BY BASE007M.material_specifications";
                //                                break;
                //                            case "库位名称":
                //                                sql = @"select BASE011M.location_name  as 'name' ,SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE011M ON WMS012A1.location=BASE011M.location_no
                //WHERE WMS012A1.material_no='" + barCode.Trim() + @"'
                //GROUP BY BASE011M.location_name";
                //                                break;
                //                            case "库位编号":
                //                                sql = @"select BASE011M.location_no as 'name',SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE011M ON WMS012A1.location=BASE011M.location_no
                //where WMS012A1.material_no='" + barCode.Trim() + @"'
                //GROUP BY BASE011M.location_no";
                //                                break;
                //                            case "仓库编号":
                //                                //                                sql = @"select BASE010M.warehouse_no as 'name',SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE010M ON WMS012A1.warehouse=BASE010M.warehouse_no
                //                                //where WMS012A1.material_no='" + barCode.Trim() + @"'
                //                                //GROUP BY BASE010M.warehouse_no";
                //                                sql = @"select BASE007M.material_no as 'name',SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE010M ON WMS012A1.warehouse=BASE010M.warehouse_no
                //INNER JOIN BASE007M ON WMS012A1.material_no=BASE007M.material_no
                //WHERE WMS012A1.warehouse='" + barCode.Trim() + @"'
                //GROUP BY BASE007M.material_no";
                //                                break;
                //                            case "仓库名称":
                //                                //                                sql = @"select BASE010M.warehouse_name as 'name',SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE010M ON WMS012A1.warehouse=BASE010M.warehouse_no
                //                                //where WMS012A1.material_no='" + barCode.Trim() + @"'
                //                                //GROUP BY BASE010M.warehouse_name";
                //                                sql = @"select BASE010M.warehouse_name as 'name',SUM(WMS012A1.qty) AS qty from WMS012A1 INNER JOIN BASE010M ON WMS012A1.warehouse=BASE010M.warehouse_no
                //where WMS012A1.warehouse='" + barCode.Trim() + @"'
                //GROUP BY BASE010M.warehouse_name";
                //                                break;
                //                        }
                //                        break;
                //                } 
                #endregion

                sql = "exec sp_GetChartsData '" + select_SQL.Replace("'", "''") + "','" + sumField + "','" + groupField + "'";
                var tab = DB.GetDataTable(sql);

                if (tab.Rows.Count > 0)
                {
                    string json = SJeMES_Framework_NETCore.Common.JsonHelper.GetJsonByDataTable(tab);
                    Dictionary<string, string> p = new Dictionary<string, string>();
                    p.Add("tab_Chart", json);
                    //string tab_XML = SJeMES_Framework_NETCore.Common.StringHelper.GetXMLFromDataTable(tab);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                }
                else
                {
                    //ret.ErrMsg = "不存在数据！";
                    ret.ErrMsg = "No data！";
                }


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }


            return ret;
        }

        static object locker = new object();
        /// <summary>
        /// 重要信息写入日志
        /// </summary>
        /// <param name="logs">日志列表，每条日志占一行</param>
        public static void WriteProgramLog(params string[] logs)
        {
            lock (locker)
            {
                string LogAddress = Environment.CurrentDirectory + "\\Log";
                if (!Directory.Exists(LogAddress + "\\PRG"))
                {
                    Directory.CreateDirectory(LogAddress + "\\PRG");
                }

                LogAddress = string.Concat(LogAddress, "\\PRG\\",
                    DateTime.Now.Year, '-', DateTime.Now.Month, '-',
                    DateTime.Now.Day, "_program.log");

                StreamWriter sw = new StreamWriter(LogAddress, true);
                foreach (string log in logs)
                {
                    sw.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(), log));
                }
                sw.Close();
            }
        }




    }
}
