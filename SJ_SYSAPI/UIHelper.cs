using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SJ_SYSAPI
{
   public class UIHelper
    {
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllUIInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();


            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            try
            {
                string Data = string.Empty;
                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_tittle = jarr["ui_tittle"].ToString();//ui_tittle
                string user = string.Empty;
                if (!string.IsNullOrEmpty(ReqObj.UserToken))
                {
                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                }
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string sql = @"
select ui_code,ui_cn,ui_en,ui_yn,ui_tittle,ui_id from
SJQDMS_UILAN where ui_tittle='" + ui_tittle + @"' order by ui_code 
";
                DataTable dt = DB.GetDataTable(sql);
                //ret.RetData = "<DataTable>";where ui_tittle='"+ ui_tittle + @"'
                string data = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    data += dt.Rows[i]["ui_code"].ToString() + "@,"
                        + dt.Rows[i]["ui_cn"].ToString() + "@,"
                         + dt.Rows[i]["ui_en"].ToString() + "@,"
                        + dt.Rows[i]["ui_yn"].ToString();
                    if (i < dt.Rows.Count - 1)
                    {
                        data += "@;";
                    }
                }
                ret.RetData = data;
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }

        public SJeMES_Framework_NETCore.WebAPI.ResultObject SetUIInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;

            try
            {
              
                Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ui_cn = jarr["ui_cn"].ToString();//ui_cn
                //string ui_code = jarr["ui_code"].ToString();//ui_code
                string ui_tittle = jarr["ui_tittle"].ToString();//ui_tittle

                string user = string.Empty;
                if (!string.IsNullOrEmpty(ReqObj.UserToken))
                {
                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                }
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string sql = string.Empty;
                string[] TEXT = ui_cn.Split("@@");
                for (int i = 0; i < TEXT.Length; i++)
                {
                    string ui_code = TEXT[i].Split('#')[0];
                    string ui_id = TEXT[i].Split('#')[1];
                    string ui_text = TEXT[i].Split('#')[2];

                    #region old
                    //                    sql = "select * from SJQDMS_UILAN where ui_code='" + CODE + @"' and ui_tittle='" + ui_tittle + @"' 
                    //and ui_id='" + CN + @"'";
                    //                    DataTable dt = DB.GetDataTable(sql);
                    //                    if (dt.Rows.Count > 0)
                    //                    {
                    //                        sql = @"
                    //UPDATE SJQDMS_UILAN set ui_id='" + CN + @"'
                    //WHERE ui_code='" + CODE + @"' and ui_tittle='" + ui_tittle + @"' AND ui_id='" + CN + @"'
                    //";
                    //                        DB.ExecuteNonQueryOffline(sql);
                    //                    }
                    //                    else
                    //                    {

                    //                        sql = @"
                    //INSERT INTO SJQDMS_UILAN
                    //(ui_code,ui_id,ui_tittle)
                    //VALUES
                    //('" + CODE + @"','" + CN + @"','" + ui_tittle + @"')
                    //";
                    //}
                    #endregion

                    //sql += @"
                    //        if not Exists(select 1 from SJQDMS_UILAN where ui_code='" + CODE + @"' and ui_tittle='" + ui_tittle + @"' and ui_id='" + CN + @"')
                    //        INSERT  INTO SJQDMS_UILAN(ui_code,ui_id,ui_tittle)
                    //        VALUES
                    //        ( '" + CODE + @"','" + CN + @"','" + ui_tittle + @"')
                    //        ELSE
                    //        UPDATE SJQDMS_UILAN
                    //        SET ui_id='" + CN + @"'
                    //        WHERE ui_code='" + CODE + @"' and ui_tittle='" + ui_tittle + @"' and ui_id='" + CN + @"'
                    //        ";

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

                }
                DB.ExecuteNonQueryOffline(sql);
                ret.IsSuccess = true;


            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }

            return ret;
        }
        public static ArrayList GetSqlFile(string varFileName, string dbname)
        {
            ArrayList alSql = new ArrayList();
            if (!File.Exists(varFileName))
            {
                return alSql;
            }
            StreamReader rs = new StreamReader(varFileName, System.Text.Encoding.Default);//注意编码
            string commandText = "";
            string varLine = "";
            while (rs.Peek() > -1)
            {
                varLine = rs.ReadLine();
                if (varLine == "")
                {
                    continue;
                }
                if (varLine != "GO" && varLine != "go")
                {
                    commandText += varLine;
                    commandText = commandText.Replace("@database_name=N'dbhr'", string.Format("@database_name=N'{0}'", dbname));
                    commandText += "\r\n";
                }
                else
                {
                    alSql.Add(commandText);
                    commandText = "";
                }
            }

            rs.Close();
            return alSql;
        }
    }
}
