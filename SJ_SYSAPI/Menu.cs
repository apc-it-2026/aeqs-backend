using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_SYSAPI
{
    public class Menu
    {
        #region 旧
        /// <summary>
        /// 获取系统菜单配置
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        //        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSMenu(object OBJ)
        //        {
        //            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();


        //            string guid = string.Empty;
        //            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
        //            try
        //            {
        //                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
        //                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();

        //                string user = string.Empty;
        //                string sql = string.Empty;
        //                DataTable dt = new DataTable();
        //                DataTable dtSYS = new DataTable();
        //                if (!string.IsNullOrEmpty(ReqObj.UserToken))
        //                {
        //                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
        //                }

        //                if (string.IsNullOrEmpty(user) || user.ToLower() == "admin")
        //                {

        //                    sql = @"
        //SELECT * from sysmenu01m order by menu_level,menu_parent,menu_seq
        //";
        //                    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
        //                    dt = DB.GetDataTable(sql);
        //                    dtSYS = DB.GetDataTable(sql);
        //                }
        //                else
        //                {
        //                    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //                    if (DB.DataBaseType.ToLower() == "mysql")
        //                    {
        //                        sql = @"
        //SELECT * from userpower where `Select`='True' and UserCode='" + user + @"' order by menu_level,menu_parent,menu_seq
        //";
        //                    }
        //                    else if (DB.DataBaseType.ToLower() == "sqlserver")
        //                    {
        //                        sql = @"
        //SELECT * from userpower where [Select]='True' and UserCode='" + user + @"' order by menu_level,menu_parent,menu_seq
        //";
        //                    }

        //                    else if (DB.DataBaseType.ToLower() == "oracle")
        //                    {
        //                        sql =
        //"SELECT * from userpower where \"Select\"='True' and \"UserCode\"='" + user + "' order by \"menu_level\",\"menu_parent\",\"menu_seq\"";
        //                    }
        //                    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
        //                    dt = DB.GetDataTable(sql);

        //                    sql = @"
        //SELECT * from sysmenu01m_tmp order by menu_level,menu_parent,menu_seq
        //";
        //                    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
        //                    dtSYS = DB.GetDataTable(sql);
        //                }




        //                if (dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr2 in dt.Rows)
        //                    {

        //                        DataRow[] drs = dtSYS.Select("name='" + dr2["name"].ToString() + @"'");

        //                        if (drs.Length > 0)
        //                        {
        //                            DataRow dr = drs[0];

        //                            SJeMES_Framework_NETCore.Web.JSONMenu menu = new SJeMES_Framework_NETCore.Web.JSONMenu();
        //                            menu.path = dr["path"].ToString();
        //                            menu.component = dr["component"].ToString();
        //                            menu.redirect = dr["redirect"].ToString();
        //                            menu.name = dr["name"].ToString();
        //                            menu.action = dr["action"].ToString();
        //                            menu.dllname = dr["dllname"].ToString();
        //                            menu.classname = dr["classname"].ToString();
        //                            menu.method = dr["method"].ToString();
        //                            menu.url = dr["url"].ToString();
        //                            menu.module = dr["module"].ToString();
        //                            try
        //                            {
        //                                menu.hidden = Convert.ToBoolean(dr["hidden"].ToString());
        //                            }
        //                            catch { }
        //                            menu.query = dr["query"].ToString();

        //                            SJeMES_Framework_NETCore.Web.JSONMenuMeta menumeta = new SJeMES_Framework_NETCore.Web.JSONMenuMeta();
        //                            menumeta.title = dr["meta_title"].ToString();
        //                            menumeta.icon = dr["meta_icon"].ToString();
        //                            try
        //                            {
        //                                menumeta.noCache = Convert.ToBoolean(dr["meta_noCache"].ToString());
        //                            }
        //                            catch { }

        //                            menu.meta = menumeta;

        //                            if (string.IsNullOrEmpty(dr["menu_parent"].ToString()))
        //                            {

        //                                MENUS.Add(menu.name, menu);

        //                            }
        //                            else
        //                            {
        //                                if (dr["menu_level"].ToString() == "2")
        //                                {
        //                                    MENUS[dr["menu_parent"].ToString()].children.Add(menu);
        //                                    MENUS2.Add(menu.name, dr["menu_parent"].ToString() + "," + (MENUS[dr["menu_parent"].ToString()].children.Count - 1).ToString());
        //                                }
        //                                else if (dr["menu_level"].ToString() == "3")
        //                                {
        //                                    string[] tmp = MENUS2[dr["menu_parent"].ToString()].Split(',');

        //                                    MENUS[tmp[0]].children[Convert.ToInt32(tmp[1])].children.Add(menu);
        //                                }
        //                            }
        //                        }

        //                    }

        //                    List<SJeMES_Framework_NETCore.Web.JSONMenu> MenuList = new List<SJeMES_Framework_NETCore.Web.JSONMenu>();

        //                    foreach (string key in MENUS.Keys)
        //                    {
        //                        MenuList.Add(MENUS[key]);
        //                    }

        //                    ret.IsSuccess = true;
        //                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(MenuList);
        //                }
        //                else
        //                {
        //                    ret.IsSuccess = false;
        //                    ret.ErrMsg = "该用户没有任何权限";
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                ret.IsSuccess = false;
        //                ret.ErrMsg = ex.Message;
        //            }

        //            return ret;
        //        } 
        #endregion


        /// <summary>
        /// 获取系统菜单配置
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        #region old wenqq
        //        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSMenu(object OBJ)
        //        {
        //            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();


        //            string guid = string.Empty;
        //            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
        //            try
        //            {
        //                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
        //                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();

        //                string user = string.Empty;
        //                string sql = string.Empty;
        //                DataTable dt1 = new DataTable();
        //                DataTable dtSYS1 = new DataTable();
        //                if (!string.IsNullOrEmpty(ReqObj.UserToken))
        //                {
        //                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
        //                }


        //                    sql = @"
        //SELECT * from SYSMENU01M order by menu_seq
        //";
        //                    DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
        //                    dt1 = DB.GetDataTable(sql);


        //                if (dt1.Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr in dt1.Rows)
        //                    {

        //                        SJeMES_Framework_NETCore.Web.JSONMenu menu = new SJeMES_Framework_NETCore.Web.JSONMenu();
        //                        menu.menu_name = dr["menu_name"].ToString();
        //                        menu.menu_seq = Convert.ToInt32(dr["menu_seq"].ToString());
        //                        menu.menu_info = dr["menu_info"].ToString();

        //                        MENUS.Add(menu.menu_name, menu);

        //                        sql = @"
        //SELECT * FROM SYSMENU02M WHERE menu_parent='"+menu.menu_name+ @"'
        //order by menu_seq
        //";                      DataTable dt2 = DB.GetDataTable(sql);

        //                        foreach(DataRow dr2 in dt2.Rows)
        //                        {
        //                            SJeMES_Framework_NETCore.Web.JSONMenu menu2 = new SJeMES_Framework_NETCore.Web.JSONMenu();
        //                            menu2.menu_name = dr2["menu_name"].ToString();
        //                            menu2.menu_seq = Convert.ToInt32(dr2["menu_seq"].ToString());
        //                            menu2.menu_info = dr2["menu_info"].ToString();
        //                            menu2.menu_parent = menu.menu_name;
        //                            MENUS[menu.menu_name].children.Add(menu2.menu_name, menu2);

        //                            sql = @"
        //SELECT * FROM SYSMENU03M WHERE menu_parent='" + menu2.menu_name + @"'
        //order by menu_seq
        //"; DataTable dt3 = DB.GetDataTable(sql);

        //                            foreach (DataRow dr3 in dt3.Rows)
        //                            {
        //                                SJeMES_Framework_NETCore.Web.JSONMenu menu3 = new SJeMES_Framework_NETCore.Web.JSONMenu();
        //                                menu3.menu_name = dr3["menu_name"].ToString();
        //                                menu3.menu_seq = Convert.ToInt32(dr3["menu_seq"].ToString());
        //                                menu3.menu_info = dr3["menu_info"].ToString();
        //                                menu3.menu_parent = menu2.menu_name;
        //                                menu3.menu_action = dr3["menu_action"].ToString();
        //                                menu3.menu_url = dr3["menu_url"].ToString();
        //                                menu3.menu_dll = dr3["menu_dll"].ToString();
        //                                menu3.menu_class = dr3["menu_class"].ToString();
        //                                menu3.menu_method = dr3["menu_method"].ToString();
        //                                menu3.menu_module = dr3["menu_module"].ToString();
        //                                MENUS[menu.menu_name].children[menu2.menu_name].children.Add(menu3.menu_name,menu3);


        //                            }
        //                        }

        //                    }

        //                    List<SJeMES_Framework_NETCore.Web.JSONMenu> MenuList = new List<SJeMES_Framework_NETCore.Web.JSONMenu>();

        //                    foreach (string key in MENUS.Keys)
        //                    {
        //                        MenuList.Add(MENUS[key]);
        //                    }

        //                    ret.IsSuccess = true;
        //                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(MenuList);
        //                }
        //                else
        //                {
        //                    ret.IsSuccess = false;
        //                    ret.ErrMsg = "该用户没有任何权限";
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                ret.IsSuccess = false;
        //                ret.ErrMsg = ex.Message;
        //            }

        //            return ret;
        //        }
        #endregion

        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSMenu(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string ss = SJeMES_Framework_NETCore.WebAPI.RequestObject.CurrentLanguage;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2;
            try
            {
                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();

                string user = string.Empty;
                string sql = string.Empty;
                DataTable dt1 = new DataTable();
                DataTable dtSYS1 = new DataTable();
                DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                if (!string.IsNullOrEmpty(ReqObj.UserToken))
                {
                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                    if (user == "ADMIN" || user == "admin")
                    {
                        
                    }
                    else
                    {
                        user = DB2.GetString("select distinct UDF01 from HR001M where user_code='" + user + "'");
                    }
                }
                else
                {
                    user = "ADMIN";
                }
                if (user == "ADMIN" || user == "admin")
                {
                    sql = @"
                    SELECT * from SYSMENU01M order by menu_seq
                    ";
                }
                else
                {
                    sql = @"select distinct SYSMENU01M.* from SYSMENU01M inner join SYSROLE03M on 
SYSMENU01M.menu_name=SYSROLE03M.parent where Role_No='" + user + "' order by menu_seq";
                }
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                dt1 = DB.GetDataTable(sql);
                if (dt1.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt1.Rows)
                    {

                        SJeMES_Framework_NETCore.Web.JSONMenu menu = new SJeMES_Framework_NETCore.Web.JSONMenu();
                        menu.menu_name = dr["menu_name"].ToString();
                        menu.menu_seq = Convert.ToInt32(dr["menu_seq"].ToString());
                        menu.menu_info = dr["menu_info"].ToString();
                        menu.ui_en= dr["ui_en"].ToString();
                        menu.ui_yn = dr["ui_yn"].ToString();
                        menu.menu_main = dr["menu_name"].ToString();
                        MENUS.Add(menu.menu_name, menu);
                        if (user=="ADMIN" || user=="admin")
                        {

                            sql = @"  SELECT * FROM SYSMENU02M
                            WHERE menu_parent='" + menu.menu_name + @"'
                            order by menu_seq
                            ";
                        }
                        else
                        {
                            sql = @"  SELECT distinct SYSMENU02M.* FROM SYSMENU02M inner join SYSROLE03M on 
  SYSMENU02M.menu_name = SYSROLE03M.parentsecond where Role_No = '" + user + "' and menu_parent = '" + menu.menu_name + "' order by menu_seq";
                        }
                        DataTable dt2 = DB.GetDataTable(sql);

                        foreach (DataRow dr2 in dt2.Rows)
                        {
                            SJeMES_Framework_NETCore.Web.JSONMenu menu2 = new SJeMES_Framework_NETCore.Web.JSONMenu();
                            menu2.menu_name = dr2["menu_name"].ToString();
                            menu2.menu_seq = Convert.ToInt32(dr2["menu_seq"].ToString());
                            menu2.menu_info = dr2["menu_info"].ToString();
                            menu2.menu_parent = menu.menu_name;
                            menu2.ui_en= dr2["ui_en"].ToString();
                            menu2.ui_yn = dr2["ui_yn"].ToString();
                            menu2.menu_main = dr["menu_name"].ToString();
                            MENUS[menu.menu_name].children.Add(menu2.menu_name, menu2);
                            if (user == "ADMIN" || user == "admin")
                            {
                                sql = @"
                                SELECT * FROM SYSMENU03M WHERE menu_parent='" + menu2.menu_name + @"'
                                order by menu_seq
                                ";
                            }
                            else
                            {
                                sql = @"SELECT distinct SYSMENU03M.* FROM SYSMENU03M   inner join SYSROLE03M on 
  SYSMENU03M.menu_name = SYSROLE03M.menuname where Role_No = '" + user + "' and menu_parent = '" + menu2.menu_name + @"' order by menu_seq";
                            }
                                DataTable dt3 = DB.GetDataTable(sql);

                                foreach (DataRow dr3 in dt3.Rows)
                                {
                                    SJeMES_Framework_NETCore.Web.JSONMenu menu3 = new SJeMES_Framework_NETCore.Web.JSONMenu();
                                    menu3.menu_name = dr3["menu_name"].ToString();
                                    menu3.menu_seq = Convert.ToInt32(dr3["menu_seq"].ToString());
                                    menu3.menu_info = dr3["menu_info"].ToString();
                                    menu3.menu_parent = menu2.menu_name;
                                    menu3.menu_action = dr3["menu_action"].ToString();
                                    menu3.menu_url = dr3["menu_url"].ToString();
                                    menu3.menu_dll = dr3["menu_dll"].ToString();
                                    menu3.menu_class = dr3["menu_class"].ToString();
                                    menu3.menu_method = dr3["menu_method"].ToString();
                                    menu3.menu_module = dr3["menu_module"].ToString();
                                    menu3.ui_en = dr3["ui_en"].ToString();
                                    menu3.ui_yn = dr3["ui_yn"].ToString();
                                    menu3.menu_main = dr["menu_name"].ToString();
                                    MENUS[menu.menu_name].children[menu2.menu_name].children.Add(menu3.menu_name, menu3);


                                }   
                            
                        }

                    }

                    List<SJeMES_Framework_NETCore.Web.JSONMenu> MenuList = new List<SJeMES_Framework_NETCore.Web.JSONMenu>();

                    foreach (string key in MENUS.Keys)
                    {
                        MenuList.Add(MENUS[key]);
                    }

                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(MenuList);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "This user does not have any permissions";
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
        /// 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSMenu2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string ss = SJeMES_Framework_NETCore.WebAPI.RequestObject.CurrentLanguage;

            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB2;
            try
            {
                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();

                string user = string.Empty;
                string sql = string.Empty;
                
                DataTable dt1 = new DataTable();
                DataTable dtSYS1 = new DataTable();
                DB2 = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string name = jarr["name"].ToString();//二级菜单名称
                if (!string.IsNullOrEmpty(ReqObj.UserToken))
                {
                    user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                    if (user == "ADMIN" || user == "admin")
                    {

                    }
                    else
                    {
                        user = DB2.GetString("select distinct UDF01 from HR001M where user_code='" + user + "'");
                    }
                }
                else
                {
                    user = "ADMIN";
                }

                if (user == "ADMIN" || user == "admin")
                {
                    sql = @"
                                SELECT * FROM SYSMENU03M WHERE menu_parent='" + name + @"'
                                order by menu_seq
                                ";
                }
                else
                {
                    sql = @"SELECT distinct SYSMENU03M.* FROM SYSMENU03M   inner join SYSROLE03M on 
  SYSMENU03M.menu_name = SYSROLE03M.menuname where Role_No = '" + user + "' and menu_parent = '" + name + @"' order by menu_seq";
                }
                DataTable dt3 = DB.GetDataTable(sql);
                string mainName = DB.GetString("SELECT menu_parent FROM SYSMENU02M where menu_name='"+ name + "'");
                foreach (DataRow dr3 in dt3.Rows)
                {
                    SJeMES_Framework_NETCore.Web.JSONMenu menu3 = new SJeMES_Framework_NETCore.Web.JSONMenu();
                    menu3.menu_name = dr3["menu_name"].ToString();
                    menu3.menu_seq = Convert.ToInt32(dr3["menu_seq"].ToString());
                    menu3.menu_info = dr3["menu_info"].ToString();
                    menu3.menu_parent = name;
                    menu3.menu_action = dr3["menu_action"].ToString();
                    menu3.menu_url = dr3["menu_url"].ToString();
                    menu3.menu_dll = dr3["menu_dll"].ToString();
                    menu3.menu_class = dr3["menu_class"].ToString();
                    menu3.menu_method = dr3["menu_method"].ToString();
                    menu3.menu_module = dr3["menu_module"].ToString();
                    menu3.ui_en = dr3["ui_en"].ToString();
                    menu3.ui_yn = dr3["ui_yn"].ToString();
                    menu3.menu_main = mainName;
                    MENUS.Add(menu3.menu_name, menu3);
                }
                List<SJeMES_Framework_NETCore.Web.JSONMenu> MenuList = new List<SJeMES_Framework_NETCore.Web.JSONMenu>();

                foreach (string key in MENUS.Keys)
                {
                    MenuList.Add(MENUS[key]);
                }
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(MenuList);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
                string insetrt = SJeMES_Framework_NETCore.Web.System.AddCatch(ReqObj, "out", ret);
            }
            return ret;
        }
        #region 旧
        /// <summary>
        /// 获取用户菜单配置
        /// </summary>
        /// <param name="ReqObj"></param>
        /// <returns></returns>
        //        public SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYSUserMenu(object OBJ)
        //        {
        //            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();


        //            string guid = string.Empty;
        //            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
        //            try
        //            {
        //                Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu> MENUS = new Dictionary<string, SJeMES_Framework_NETCore.Web.JSONMenu>();
        //                Dictionary<string, string> MENUS2 = new Dictionary<string, string>();

        //                string keys = "user_code";
        //                Dictionary<string, object> p = SJeMES_Framework_NETCore.WebAPI.WebAPIHelper.GetWebParameters(keys, ReqObj);


        //                string sql = @"
        //SELECT * from userpower where `Select`='True' and user_code=@user_code order by menu_level,menu_parent,menu_seq
        //";


        //                DataTable dt = DB.GetDataTable(sql, p);

        //                if (dt.Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr in dt.Rows)
        //                    {
        //                        SJeMES_Framework_NETCore.Web.JSONMenu menu = new SJeMES_Framework_NETCore.Web.JSONMenu();
        //                        menu.path = dr["path"].ToString();
        //                        menu.component = dr["component"].ToString();
        //                        menu.redirect = dr["redirect"].ToString();
        //                        menu.name = dr["name"].ToString();
        //                        menu.hidden = Convert.ToBoolean(dr["hidden"].ToString());
        //                        menu.query = dr["query"].ToString();

        //                        SJeMES_Framework_NETCore.Web.JSONMenuMeta menumeta = new SJeMES_Framework_NETCore.Web.JSONMenuMeta();
        //                        menumeta.title = dr["meta_title"].ToString();
        //                        menumeta.icon = dr["meta_icon"].ToString();
        //                        menumeta.noCache = Convert.ToBoolean(dr["meta_noCache"].ToString());


        //                        menu.meta = menumeta;

        //                        if (string.IsNullOrEmpty(dr["menu_parent"].ToString()))
        //                        {

        //                            MENUS.Add(menu.name, menu);

        //                        }
        //                        else
        //                        {
        //                            if (dr["menu_level"].ToString() == "2")
        //                            {
        //                                MENUS[dr["menu_parent"].ToString()].children.Add(menu);
        //                                MENUS2.Add(menu.name, dr["menu_parent"].ToString() + "," + (MENUS[dr["menu_parent"].ToString()].children.Count - 1).ToString());
        //                            }
        //                            else if (dr["menu_level"].ToString() == "3")
        //                            {
        //                                string[] tmp = MENUS2[dr["menu_parent"].ToString()].Split(',');

        //                                MENUS[tmp[0]].children[Convert.ToInt32(tmp[1])].children.Add(menu);
        //                            }
        //                        }


        //                    }

        //                    List<SJeMES_Framework_NETCore.Web.JSONMenu> MenuList = new List<SJeMES_Framework_NETCore.Web.JSONMenu>();

        //                    foreach (string key in MENUS.Keys)
        //                    {
        //                        MenuList.Add(MENUS[key]);
        //                    }

        //                    ret.IsSuccess = true;
        //                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(MenuList);
        //                }
        //                else
        //                {
        //                    ret.IsSuccess = false;
        //                    ret.ErrMsg = "";
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                ret.IsSuccess = false;
        //                ret.ErrMsg = ex.Message;
        //            }

        //            return ret;
        //        } 
        #endregion




    }
}
