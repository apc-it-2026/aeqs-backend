using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace SJ_SYSAPI
{
    public class Language
    {

        /// <summary>
        /// 获取消息多语言
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
           
            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);//获取业务库
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);//获取系统库

                ret.IsSuccess = true;

                #region 接口参数
                string msg = jarr["msg"].ToString();//条件
                string language = jarr["language"].ToString();//条件
                #endregion
                #region 逻辑


                #region 数据获取SQL
                string datasql = @"
select
*
from SYSLAN05M where msg=N'" + msg + @"'
";
                #endregion
                DataTable dt = DBSYS.GetDataTable(datasql);
                if (dt.Rows.Count > 0)
                {
                    if (language == "en" && !string.IsNullOrEmpty(dt.Rows[0]["ui_en"].ToString()))
                    {
                        ret.IsSuccess = true;
                        ret.RetData = dt.Rows[0]["ui_en"].ToString();
                    }
                    else if (language == "hk" && !string.IsNullOrEmpty(dt.Rows[0]["ui_yn"].ToString()))
                    {
                        ret.IsSuccess = true;
                        ret.RetData = dt.Rows[0]["ui_yn"].ToString();
                    }
                    else if (language == "cn" && !string.IsNullOrEmpty(dt.Rows[0]["ui_cn"].ToString()))
                    {
                        ret.IsSuccess = true;
                        ret.RetData = dt.Rows[0]["ui_cn"].ToString();
                    }
                    else
                    {
                        ret.IsSuccess = true;
                        ret.RetData = dt.Rows[0]["msg"].ToString();
                    }
                }
                else
                {
                    string pattern = "[\u4e00-\u9fbb]";
                    if (Regex.IsMatch(msg, pattern))
                    {
                        datasql = "insert into SYSLAN05M(msg,ui_en,ui_yn,ui_cn) values(N'" + msg + "','','',N'"+ msg + "')";
                    }
                    else
                    {
                        datasql = "insert into SYSLAN05M(msg,ui_en,ui_yn,ui_cn) values(N'" + msg + "','','','')";
                    }
                    DBSYS.ExecuteNonQueryOffline(datasql);
                    ret.IsSuccess = true;
                    ret.RetData = msg;
                }
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetListLanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);//获取业务库
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);//获取系统库

                ret.IsSuccess = true;

                #region 接口参数
                List<string> lstKey = Newtonsoft.Json.JsonConvert.DeserializeObject <List<string>>(jarr["lstKey"].ToString());//条件
                string language = jarr["language"].ToString().ToLower();//条件
                #endregion


                #region 逻辑
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string strName = string.Empty;
                foreach (string msg in lstKey)
                {
                    strName = msg;
                    string datasql = @"
                                select
                                *
                                from SYSLAN05M where msg=N'" + msg + @"'
                                "; 
                    DataTable dt = DBSYS.GetDataTable(datasql);
                    if (dt.Rows.Count > 0)
                    {
                        if (language == "en" && !string.IsNullOrEmpty(dt.Rows[0]["ui_en"].ToString()))
                        {
                            strName = dt.Rows[0]["ui_en"].ToString();
                        }
                        else if (language == "hk" && !string.IsNullOrEmpty(dt.Rows[0]["ui_yn"].ToString()))
                        { 
                            strName = dt.Rows[0]["ui_yn"].ToString();
                        }
                        else if (language == "cn" && !string.IsNullOrEmpty(dt.Rows[0]["ui_cn"].ToString()))
                        { 
                            strName = dt.Rows[0]["ui_cn"].ToString();
                        }
                        else
                        { 
                            strName = dt.Rows[0]["msg"].ToString();
                        }
                    }
                    else
                    {
                        string pattern = "[\u4e00-\u9fbb]";
                        if (Regex.IsMatch(msg, pattern))
                        {
                            datasql = "insert into SYSLAN05M(msg,ui_en,ui_yn,ui_cn) values(N'" + msg + "','','',N'" + msg + "')";
                        }
                        else
                        {
                            datasql = "insert into SYSLAN05M(msg,ui_en,ui_yn,ui_cn) values(N'" + msg + "','','','')";
                        }
                        DBSYS.ExecuteNonQueryOffline(datasql);
                    }
                    dic.Add(msg, strName);
                }
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                #endregion
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
        /// 获取消息多语言
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDgv(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                ret.IsSuccess = true;

                #region 接口参数
                string ui_code = jarr["ui_code"].ToString();
                string ui_tittle = jarr["ui_tittle"].ToString();
                string Language = jarr["Language"].ToString();
                var o = JObject.Parse(Data);
                string Table = o["dt"].ToString();
                #endregion
                 
                #region 数据获取SQL
                if (!string.IsNullOrEmpty(Table))
                {
                    string sql = string.Empty;
                    DataTable dt = SJeMES_Framework_NETCore.Common.JsonHelper.GetDataTableByJson(Table);
                    if (dt.Rows.Count > 0)
                    {
                        DataTable tblDatas = new DataTable("Datas");
                        DataColumn dc = null;
                        tblDatas.Columns.Add("name", Type.GetType("System.String"));
                        dc = tblDatas.Columns.Add("title", Type.GetType("System.String"));
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string ui_id = dt.Rows[i]["title"].ToString().Split('&')[0];
                            string ui_text = dt.Rows[i]["title"].ToString().Split('&')[1];
                            DataTable count = DBSYS.GetDataTable("select * from SJQDMS_UILAN where ui_tittle='" + ui_tittle + "' " +
                                "and ui_code='" + ui_code + "' and ui_id='" + ui_id + "'");
                            if (count.Rows.Count == 0)
                            {  
                                #region 不存在的新增
                                //判断是否中文
                                string pattern = "[\u4e00-\u9fbb]";
                                if (Regex.IsMatch(ui_text, pattern))
                                {
                                    sql += @"
                                            if not Exists(select 1 from SJQDMS_UILAN where ui_code='{0}' and ui_tittle='{2}' and ui_id='{1}')
                                            INSERT  INTO SJQDMS_UILAN(ui_code,ui_id,ui_tittle,ui_cn)
                                            VALUES ( '{0}','{1}','{2}','{3}');
                                             ";
                                    sql = string.Format(sql, ui_code, ui_id, ui_tittle, ui_text);
                                }
                                //如果不是中文的就存在英文的列
                                else
                                {
                                    sql += @"
                                            if not Exists(select 1 from SJQDMS_UILAN where ui_code='{0}' and ui_tittle='{2}' and ui_id='{1}')
                                            INSERT  INTO SJQDMS_UILAN(ui_code,ui_id,ui_tittle,ui_cn,ui_en)
                                            VALUES ( '{0}','{1}','{2}','{3}','{3}');
                                            ";
                                    sql = string.Format(sql, ui_code, ui_id, ui_tittle, ui_text);
                                }
                                #endregion 
                            }
                        }
                        if (!string.IsNullOrEmpty(sql))
                            DBSYS.ExecuteNonQueryOffline(sql);

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string ui_id = dt.Rows[i]["title"].ToString().Split('&')[0];
                            DataRow newRow;
                            newRow = tblDatas.NewRow();
                            DataTable count = DBSYS.GetDataTable("select * from SJQDMS_UILAN where ui_tittle='" + ui_tittle + "' " +
                               "and ui_code='" + ui_code + "' and ui_id='" + ui_id + "'");
                            if (count.Rows.Count > 0)
                            {
                                newRow["name"] = count.Rows[0]["ui_id"].ToString();
                                if (Language == "en")
                                {
                                    if (!string.IsNullOrEmpty(count.Rows[0]["ui_en"].ToString()))
                                        newRow["title"] = count.Rows[0]["ui_en"].ToString();
                                    else
                                        newRow["title"] = count.Rows[0]["ui_cn"].ToString();
                                }
                                else if (Language == "hk")
                                {
                                    if (!string.IsNullOrEmpty(count.Rows[0]["ui_yn"].ToString()))
                                        newRow["title"] = count.Rows[0]["ui_yn"].ToString();
                                    else
                                        newRow["title"] = count.Rows[0]["ui_cn"].ToString();
                                }
                                else if(Language == "cn")
                                    newRow["title"] = count.Rows[0]["ui_cn"].ToString();
                                else
                                    newRow["title"] = count.Rows[0]["ui_id"].ToString();

                                tblDatas.Rows.Add(newRow);
                                
                            }
                        }
                        Dictionary<string, object> p = new Dictionary<string, object>();
                        p.Add("dt", tblDatas);
                        ret.IsSuccess = true;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(p);
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "数据不能为空！";
                }
                #endregion
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
        /// 测试接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getcs(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            try
            {
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                //string msg = jarr["msg"].ToString();
                ret.IsSuccess = false;
                #region 数据获取SQL
                ret.ErrMsg = "品号12345不存在";
                #endregion
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
        /// 获取弹窗消息多语言
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetUILanguage(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DBSYS;
            
            string Data = string.Empty;
            string sql = string.Empty;

            try
            {
                #region 接口参数
                DBSYS = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                Data = ReqObj.Data.ToString();
                if (string.IsNullOrEmpty(Data))
                    throw new Exception("传入参数不能为空，请检查！");
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                if (jarr.Count == 0)
                    throw new Exception("传入参数不能为空，请检查！");
                string lan = jarr["Language"].ToString().ToLower();
                if (string.IsNullOrEmpty(lan))
                    throw new Exception("参数【Language】不能为空，请检查！");
                List<string> lstKeys = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["Data"].ToString());
                #endregion

                #region 逻辑
                StringBuilder sb = new StringBuilder();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                foreach (var item in lstKeys)
                {
                    sb.Append("'" + item + "',");
                }
                string strwhere = sb.ToString().TrimEnd(',');

                string strSelect = string.Empty;
                if (lan.ToLower().Equals("cn"))
                    strSelect = " ui_cn as lanText ";
                else if (lan.ToLower().Equals("en"))
                    strSelect = " ui_en as lanText ";
                else if (lan.ToLower().Equals("hk"))
                    strSelect = " ui_yn as lanText ";

                sql = @"select msg," + strSelect + " from SYSLAN05M(nolock) where msg in (" + strwhere + ")";
                DataTable dt = DBSYS.GetDataTable(sql);
                if (dt != null && dt.Rows.Count > 0 && lstKeys.Count > 0)
                {
                    foreach (var item in lstKeys)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            if (dr["msg"].ToString().Equals(item))
                            {
                                if (dic.ContainsKey(item))
                                    dic[item] = dr["lanText"].ToString();
                                else
                                    dic.Add(item, dr["lanText"].ToString());
                            }
                        }
                    }
                }
                #endregion

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;

            string Data = string.Empty;
            string sql = string.Empty;

            try
            {
                #region 接口参数
                DB= new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();
                //获取用户权限
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCode(ReqObj);
                #endregion
                #region 逻辑
                sql = @"select material_no as '物料代号',material_name as '物料名称',
createby as '创建人' from BASE007M WHERE 1=1 ";
                if (!string.IsNullOrEmpty(usercode))
                {
                    sql += " AND createby in (" + usercode + ")";
                }
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("json",dt);
                #endregion
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DelData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;

            string Data = string.Empty;
            string sql = string.Empty;

            try
            {
                #region 接口参数
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id= jarr["id"].ToString();
                #endregion
                #region 逻辑
                    sql = @"delete from base007M where material_no in ('" + id+"')";
                    DB.ExecuteNonQueryOffline(sql);
                    ret.IsSuccess = true;
                    ret.ErrMsg = "删除成功！";
                #endregion
                //ret.IsSuccess = true;
                //ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;

            string Data = string.Empty;
            string sql = string.Empty;

            try
            {
                #region 接口参数
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                #endregion
                #region 逻辑
                    ret.IsSuccess = true;
                    ret.ErrMsg = "编辑成功！";
                #endregion
                //ret.IsSuccess = true;
                //ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

    }
}
