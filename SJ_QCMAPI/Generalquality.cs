using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SJ_QCMAPI.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace SJ_QCMAPI
{
    public class Generalquality
    {
        #region 一级菜单
        /// <summary>
        /// 查询一级菜单——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetGeneralquality_m(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string typename1 = jarr.ContainsKey("typename1") ? jarr["typename1"].ToString().Trim() : "";//通用标准类型
                string typename2 = jarr.ContainsKey("typename2") ? jarr["typename2"].ToString().Trim() : "";//分类名称
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(typename1))
                    where += " and general_testtype_no='" + typename1 + "'";
                if (!string.IsNullOrEmpty(typename2))
                    where += "and quality_category_name like '%" + typename2 + "%'";

                string sql = @"  SELECT ID,general_testtype_no,quality_category_no, quality_category_name,remarks FROM bdm_generalquality_m where 1=1  " + where;
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 查询页签——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTab(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string sql = @"  SELECT general_testtype_no,general_testtype_name FROM bdm_general_testtype_m where 1=1 ";
                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 类型修改赋值——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeUpdata(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString().Trim() : "";//id编号
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(id))
                {
                    sql = @"select * from bdm_generalquality_m where id='" + id + "'";
                }
                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 类型修改,新增——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeEdit(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString().Trim() : "";//id编号
                string typeno = jarr.ContainsKey("GENERAL_TESTTYPE_NO") ? jarr["GENERAL_TESTTYPE_NO"].ToString().Trim() : "";//通用检测类型编号
                string quality_category_no = jarr.ContainsKey("quality_category_no") ? jarr["quality_category_no"].ToString().Trim() : "";//品质类别代号
                string quality_category_name = jarr.ContainsKey("quality_category_name") ? jarr["quality_category_name"].ToString().Trim() : "";//品质类别名称
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString().Trim() : "";//备注
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH-mm-ss");
                string sql = string.Empty;//编辑
                string sql2 = string.Empty;//判断 
                if (!string.IsNullOrEmpty(id))
                {
                    sql = @"update bdm_generalquality_m set quality_category_no='" + quality_category_no + "',quality_category_name='" + quality_category_name + "',remarks='" + remarks + "',modifyby='" + user + "',modifydate='" + date + "',modifytime='" + time + "' where id='" + id + "'";
                    DB.ExecuteNonQuery(sql);
                }
                else
                { 
                    sql2 = $@"select 1 from bdm_generalquality_m where general_testtype_no ='{typeno}' and quality_category_no='{quality_category_no}'";
                    DataTable dt = DB.GetDataTable(sql2);
                    if (dt.Rows.Count > 0)
                        throw new Exception("分类代号【"+ quality_category_no + "】已经存在，不能重复，请检查！");
                     
                    sql = $@"insert into bdm_generalquality_m (general_testtype_no,quality_category_no,quality_category_name,remarks,createby,createdate,createtime)
                            values('{typeno}','{quality_category_no}','{quality_category_name}','{remarks}','{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                } 
                 
                DB.Commit(); 
                ret.IsSuccess = true;
                ret.RetData = ""; 
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            { 
                DB.Close();
            }

            return ret;
        }

        /// <summary>
        /// 类型删除——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeDelete(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString().Trim() : "";//id编号
                string general_testtype_no= jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用类别编号
                string sql = string.Empty;//删除

                 
                string quality_category_no = DB.GetString($"select quality_category_no from bdm_generalquality_m where id='{id}'");//一级菜单编号
                if (!string.IsNullOrEmpty(general_testtype_no))
                {  
                    //删除单头
                    sql = $@"delete from bdm_generalquality_m where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除二级单身
                    sql = $@"delete from bdm_generalquality_d where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 测试项目
                    sql = $@"delete from bdm_qualitytest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 外观检测项目
                    sql = $@"delete from bdm_qualityaptest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 试穿检测项目
                    sql = $@"delete from bdm_qualitytntest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}'";
                    DB.ExecuteNonQuery(sql);
                }
                 
                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            { 
                DB.Close();
            }

            return ret;
        }
        #endregion

        #region 二级菜单
        /// <summary>
        /// 查询二级菜单当前分类——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTitle(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string general_testtype_no = jarr.ContainsKey("yq") ? jarr["yq"].ToString().Trim() : "";//通用类型编号

                string TYname = DB.GetString(@"select general_testtype_name from bdm_general_testtype_m where general_testtype_no='" + general_testtype_no + "'");
                string MenuOne = DB.GetString(@"select quality_category_name from bdm_generalquality_m where id ='" + qid + "'");

                List<string> dt = new List<string>();
                dt.Add(TYname);
                dt.Add(MenuOne);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查询二级菜单——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetGeneralquality_d(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string typename = jarr.ContainsKey("typename") ? jarr["typename"].ToString().Trim() : "";//分类名称
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单编号
                string general_testtype_no= jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用类别编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrEmpty(typename))
                {
                    where += " and d.secondary_category_name like '%" + typename + "%'";
                }
                if (!string.IsNullOrEmpty(qid))
                {
                    where += " and m.id = '" + qid + "'";
                }
                if (!string.IsNullOrEmpty(general_testtype_no))
                {
                    where += " and d.general_testtype_no = '" + general_testtype_no + "'";
                }
                string sql = @"select row_number() over(ORDER BY d.ID) as xh,d.id,m.quality_category_no,m.quality_category_name,d.secondary_category_no,d.secondary_category_name,d.REMARKS from bdm_generalquality_m m INNER JOIN bdm_generalquality_d d ON m.quality_category_no=d.quality_category_no where 1=1" + where;
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 二级菜单类型修改赋值——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeUpdataD(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string dno = jarr.ContainsKey("dno") ? jarr["dno"].ToString().Trim() : "";//二级菜单编号
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(dno))
                {
                    sql = @"select * from bdm_generalquality_d where secondary_category_no='" + dno + "'";
                }
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 二级菜单类型修改,新增——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeEditD(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString().Trim() : "";//二级菜单id
                string dno = jarr.ContainsKey("dno") ? jarr["dno"].ToString().Trim() : "";//二级菜单编号
                string dname = jarr.ContainsKey("dname") ? jarr["dname"].ToString().Trim() : "";//二级菜单名称
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString().Trim() : "";//二级菜单备注
                string mid = jarr.ContainsKey("mid") ? jarr["mid"].ToString().Trim() : "";//一级菜单id
                string general_testtype_no= jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用类别编号
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//操作人
                string date = DateTime.Now.ToString("yyyy-MM-dd");//年月日
                string time = DateTime.Now.ToString("HH-mm-ss");//时分秒
                string mno = DB.GetString(@"select quality_category_no from bdm_generalquality_m where id='" + mid + "'");
                string sql = string.Empty;
                int count = 0;
                if (!string.IsNullOrEmpty(did))
                {
                    sql = @"update bdm_generalquality_d set secondary_category_no='" + dno + "',secondary_category_name='" + dname + "',remarks='" + remarks + "',modifyby='" + user + "',modifydate='" + date + "',modifytime='" + time + "' where id='" + did + "'";
                }
                else
                {
                    string sql2 = DB.GetString(@"select id from bdm_generalquality_d where secondary_category_no='" + dno + "'");//判断编号是否重复
                    if (string.IsNullOrEmpty(sql2))
                    {
                        sql = @"insert into bdm_generalquality_d (general_testtype_no,quality_category_no,secondary_category_no,secondary_category_name,remarks,createby,createdate,createtime) values('"+ general_testtype_no + "','" + mno + "','" + dno + "','" + dname + "','" + remarks + "','" + user + "','" + date + "','" + time + "')";
                    }
                }
                if (!string.IsNullOrEmpty(sql))
                {
                    count = DB.ExecuteNonQuery(sql);
                    DB.Commit();
                }
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(count);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 二级菜单类型删除——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TypeDeleteD(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString().Trim() : "";//id编号
                string general_testtype_no= jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用类别编号
                string qid= jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单编号
                string sql = string.Empty;//删除
                string secondary_category_no = DB.GetString($@"select secondary_category_no from bdm_generalquality_d where id='{did}'");//二级菜单编号
                string quality_category_no = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");//一级菜单编号
                if (!string.IsNullOrEmpty(secondary_category_no))
                {
                    //删除二级单头
                    sql = $@"delete from bdm_generalquality_d where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}' and secondary_category_no='{secondary_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 测试项目
                    sql = $@"delete from bdm_qualitytest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}' and secondary_category_no='{secondary_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 外观检测项目
                    sql = $@"delete from bdm_qualityaptest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}' and secondary_category_no='{secondary_category_no}'";
                    DB.ExecuteNonQuery(sql);

                    //删除测试项明细 —— 试穿检测项目
                    sql = $@"delete from bdm_qualitytntest_item where general_testtype_no='{general_testtype_no}' and quality_category_no='{quality_category_no}' and secondary_category_no='{secondary_category_no}'";
                    DB.ExecuteNonQuery(sql);
                }

                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }
        #endregion

        /// <summary>
        /// 判断通用类别——万邦
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject JudgeType(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string gid = jarr.ContainsKey("gid") ? jarr["gid"].ToString().Trim() : "";//通用类型编号
                string sql = string.Empty;//判断
                int GC = 0;
                string Type = DB.GetString(@"select general_category from bdm_general_testtype_m where general_testtype_no='" + gid + "'");
                if (Type == "0")
                {
                    GC = 0;
                }
                else
                    GC = 1;
                DB.Commit();
                DB.Close();
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(GC);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        #region 三级菜单

        /// <summary>
        /// 查找测量标准值测试项目库
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestValue(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString().Trim() : "";//检测项编号
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(code))
                {
                    string type = DB.GetString(@"select type from bdm_testitem_m where testitem_code='" + code + "'");
                    sql = @"select value from bdm_testitem_d where testitem_code='" + code+"'";
                    string sql2 = @"select (min_value || '-' || max_value) as value  from bdm_testitem_d where testitem_code='"+code+"'";
                    DataTable dt2 = DB.GetDataTable(sql2);
                    string sql3 = @"select (value || '-' || error_value) as value from bdm_testitem_d where testitem_code='" + code + "'";
                    DataTable dt3 = DB.GetDataTable(sql3);
                    dt = DB.GetDataTable(sql);
                    if (type == enum_testitem_type.enum_testitem_type_1)
                    {
                            dic.Add("values", dt);
                    }
                    if (type == enum_testitem_type.enum_testitem_type_2)
                    {
                            dic.Add("values", dt2);
                    }
                    if (type == enum_testitem_type.enum_testitem_type_3)
                    {
                            dic.Add("values", dt3);
                    }
                }

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查找外观检测项目库
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAppearanceValue(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString().Trim() : "";//检测项编号
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(code))
                {
                    sql = @"select test_standard from bdm_aptestitem_d where aptestitem_code='" + code + "'";
                    dt = DB.GetDataTable(sql);
                    dic.Add("values", dt);
                }

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 查找试穿项目库
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTryOnValue(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString().Trim() : "";//检测项编号
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(code))
                {
                    sql = @"select test_standard from BDM_TNTESTITME_D where tntestitem_code='" + code + "'";
                    dt = DB.GetDataTable(sql);
                    dic.Add("values", dt);
                }

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 添加检测项数据
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertBDM_QUALITYTEST_ITEM(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间


                DataTable bdm_qualitytest_item = Newtonsoft.Json.JsonConvert.DeserializeObject <DataTable> (jarr["bdm_qualitytest_item"].ToString());
                string general_testtype_no= jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用检测类型代号
                string qid= jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string qno = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");
                string secondary_category_no = jarr.ContainsKey("secondary_category_no") ? jarr["secondary_category_no"].ToString().Trim() : "";//二级菜单代号
                string where = string.Empty;
                foreach (DataRow dt in bdm_qualitytest_item.Rows)
                {
                    string sql1 = $@"insert into bdm_qualitytest_item (createby,createdate,createtime,general_testtype_no,quality_category_no,secondary_category_no,testitem_code,testitem_name,AQL_LEVEL,testtype_no,testtype_name,sample_num,reference_level,check_item,check_value,unit,currency_formula,custom_formula,remarks,check_type)
                                     values('{user}','{date}','{time}','{general_testtype_no}','{qno}','{secondary_category_no}','{dt["testitem_code_1"]}','{dt["testitem_name_1"]}','{dt["AQL_LEVEL_1"]}','{dt["testtype_no_1"]}','{dt["testtype_name_1"]}','{dt["sample_num_1"]}','{dt["reference_level_1"]}','{dt["check_item_1"]}','{dt["check_value_1"]}','{dt["unit_1"]}','{dt["currency_formula_1"]}','{dt["custom_formula_1"]}','{dt["remarks_1"]}','{dt["check_type_1"]}')";
                    DB.ExecuteNonQuery(sql1);
                }

                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }

        /// <summary>
        /// 添加外观检测项数据
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertBDM_QUALITYAPTEST_ITEM(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间


                DataTable bdm_qualityaptest_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_qualityaptest_item"].ToString());
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用检测类型代号
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string qno = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");
                string secondary_category_no = jarr.ContainsKey("secondary_category_no") ? jarr["secondary_category_no"].ToString().Trim() : "";//二级菜单代号
                string where = string.Empty;
                foreach (DataRow dt in bdm_qualityaptest_item.Rows)
                {
                    string sql2 = $@"insert into bdm_qualityaptest_item (sample_num,AQL_LEVEL,createby,createdate,createtime,general_testtype_no,quality_category_no,secondary_category_no,testitem_code,testitem_name,check_item,check_value,reference_level,remarks,testtype_no,testtype_name) 
                                     values('{dt["sample_num_2"]}','{dt["AQL_LEVEL_2"]}','{user}','{date}','{time}','{general_testtype_no}','{qno}','{secondary_category_no}','{dt["testitem_code_2"]}','{dt["testitem_name_2"]}','{dt["check_item_2"]}','{dt["check_value_2"]}','{dt["reference_level_2"]}','{dt["remarks_2"]}','{dt["testtype_no_2"]}','{dt["testtype_name_2"]}')";
                    DB.ExecuteNonQuery(sql2);
                }

                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }
        
        /// <summary>
        /// 添加试穿检测项数据
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertBDM_QUALITYTNTEST_ITEM(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间


                DataTable bdm_qualitytntest_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_qualitytntest_item"].ToString());
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用检测类型代号
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string qno = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");
                string secondary_category_no = jarr.ContainsKey("secondary_category_no") ? jarr["secondary_category_no"].ToString().Trim() : "";//二级菜单代号
                string where = string.Empty;
                foreach (DataRow dt in bdm_qualitytntest_item.Rows)
                {
                    string sql3 = $@"insert into bdm_qualitytntest_item (sample_num,AQL_LEVEL,createby,createdate,createtime,general_testtype_no,quality_category_no,secondary_category_no,testitem_code,testitem_name,check_item,check_value,reference_level,remarks,testtype_no,testtype_name) 
                                     values('{dt["sample_num_3"]}','{dt["AQL_LEVEL_3"]}','{user}','{date}','{time}','{general_testtype_no}','{qno}','{secondary_category_no}','{dt["testitem_code_3"]}','{dt["testitem_name_3"]}','{dt["check_item_3"]}','{dt["check_value_3"]}','{dt["reference_level_3"]}','{dt["remarks_3"]}','{dt["testtype_no_3"]}','{dt["testtype_name_3"]}')";
                    DB.ExecuteNonQuery(sql3);
                }

                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }

        /// <summary>
        /// 修改数据
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdataSJ(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间


                DataTable bdm_qualitytest_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_qualitytest_item"].ToString());
                DataTable bdm_qualityaptest_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_qualityaptest_item"].ToString());
                DataTable bdm_qualitytntest_item = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_qualitytntest_item"].ToString());
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用检测类型代号
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string qno = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");
                string secondary_category_no = jarr.ContainsKey("secondary_category_no") ? jarr["secondary_category_no"].ToString().Trim() : "";//二级菜单代号
                string where = string.Empty;
                foreach (DataRow dt in bdm_qualitytest_item.Rows)
                {
                    if (!string.IsNullOrEmpty(general_testtype_no))
                    {
                        where += $@" and general_testtype_no='{general_testtype_no}'";
                    }
                    if (!string.IsNullOrEmpty(qno))
                    {
                        where += $@"and quality_category_no='{qno}'";
                    }
                    if (!string.IsNullOrEmpty(secondary_category_no))
                    {
                        where += $@"and secondary_category_no='{secondary_category_no}'";
                    }
                    string sql1 = $@"update bdm_qualitytest_item set check_item='{dt["check_item_1"]}',check_value='{dt["check_value_1"]}',
                                    modifyby='{user}',modifydate='{date}',modifytime='{time}' 
                                    where testitem_code='{dt["testitem_code_1"]}' {where}";
                    DB.ExecuteNonQuery(sql1);
                }
                foreach (DataRow dt in bdm_qualityaptest_item.Rows)
                {
                    if (!string.IsNullOrEmpty(general_testtype_no))
                    {
                        where += $@" and general_testtype_no='{general_testtype_no}'";
                    }
                    if (!string.IsNullOrEmpty(qno))
                    {
                        where += $@"and quality_category_no='{qno}'";
                    }
                    if (!string.IsNullOrEmpty(secondary_category_no))
                    {
                        where += $@"and secondary_category_no='{secondary_category_no}'";
                    }
                    string sql2 = $@"update bdm_qualityaptest_item set check_item='{dt["check_item_2"]}',check_value='{dt["check_value_2"]}',
                                     modifyby='{user}',modifydate='{date}',modifytime='{time}'
                                     where testitem_code='{dt["testitem_code_2"]}' {where}";
                    DB.ExecuteNonQuery(sql2);
                }
                foreach (DataRow dt in bdm_qualitytntest_item.Rows)
                {
                    if (!string.IsNullOrEmpty(general_testtype_no))
                    {
                        where += $@" and general_testtype_no='{general_testtype_no}'";
                    }
                    if (!string.IsNullOrEmpty(qno))
                    {
                        where += $@"and quality_category_no='{qno}'";
                    }
                    if (!string.IsNullOrEmpty(secondary_category_no))
                    {
                        where += $@"and secondary_category_no='{secondary_category_no}'";
                    }
                    string sql3 = $@"update bdm_qualitytntest_item set check_item='{dt["check_item_3"]}',check_value='{dt["check_value_3"]}',
                                     modifyby='{user}',modifydate='{date}',modifytime='{time}'
                                     where testitem_code='{dt["testitem_code_3"]}' {where}";
                    DB.ExecuteNonQuery(sql3);
                }

                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteSJ(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";//通用检测类型代号
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";//一级菜单id
                string qno = DB.GetString($@"select quality_category_no from bdm_generalquality_m where id='{qid}'");
                string secondary_category_no = jarr.ContainsKey("secondary_category_no") ? jarr["secondary_category_no"].ToString().Trim() : "";//二级菜单代号
                string testitem_code= jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString().Trim() : "";//检测项编号
                string TableName= jarr.ContainsKey("TableName") ? jarr["TableName"].ToString().Trim() : "";//表名
                string where = string.Empty;
                if (!string.IsNullOrEmpty(general_testtype_no))
                {
                    where += $" and general_testtype_no='{general_testtype_no}'";
                }
                if (!string.IsNullOrEmpty(qno))
                {
                    where += $" and quality_category_no='{qno}'";
                }
                if (!string.IsNullOrEmpty(secondary_category_no))
                {
                    where += $" and secondary_category_no='{secondary_category_no}'";
                }
                if (!string.IsNullOrEmpty(testitem_code))
                {
                    where += $" and testitem_code='{testitem_code}'";
                }
                
                string sql = $"delete from {TableName} where 1=1 {where}";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                DB.Close();

                ret.IsSuccess = true;
                ret.RetData = "";

            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }

        /// <summary>
        /// 查询已勾选
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCheck(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TableName = jarr.ContainsKey("TableName") ? jarr["TableName"].ToString().Trim() : "";
                string yq = jarr.ContainsKey("yq") ? jarr["yq"].ToString().Trim() : "";
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString().Trim() : "";
                string qno = DB.GetString("select quality_category_no from bdm_generalquality_m where id='" + qid + "'");
                string testitem_code= jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string testitem_name= jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";

                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";

                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(TableName))
                {
                    if (TableName== "bdm_qualitytest_item")
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
	                            testtype_no ,
	                            testitem_code,
	                            testitem_name ,
                                testtype_name,
	                            check_item ,
	                            check_value ,
	                            unit ,
                                AQL_LEVEL,
	                            reference_level ,
	                            sample_num ,
	                            currency_formula,
	                            ( SELECT ENUM_VALUE FROM SYS001M WHERE ENUM_TYPE = 'enum_general_formula' AND ENUM_CODE = currency_formula ) AS 通用公式名称， 
	                            custom_formula,
                                ( SELECT formula_name FROM bdm_formula_m WHERE formula_code = custom_formula ) AS 自定义公式名称,
	                            remarks
                            FROM
	                            {TableName} b 
                            WHERE
	                    1 =1 {where}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
                            testitem_code as 检测项编号，
                            testitem_name as 检测项名称，
                            check_item as 判断标准，
                            check_value as 测量标准，
                            reference_level as 项目引用级别，
                            remarks as 备注
                            FROM
                            {TableName}
                            WHERE
	                    1 =1 {where}";
                    }
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }
        #endregion



        /// <summary>
        /// 查询已勾选2
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCheck2(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TableName = jarr.ContainsKey("TableName") ? jarr["TableName"].ToString().Trim() : "";
                string yq = jarr.ContainsKey("yq") ? jarr["yq"].ToString().Trim() : "";
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString().Trim() : "";
                string qno = DB.GetString("select quality_category_no from bdm_generalquality_m where id='" + qid + "'");
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string testitem_name = jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(TableName))
                {
                    if (TableName == "bdm_qualitytest_item")
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
	                    testtype_no as 检测类型,
	                    testitem_code as 检测项编号,
	                    testitem_name as 检测项名称,
                        testtype_no as 检测项类型,
                        testtype_name as 检测项类型名称,
	                    check_item as 判断标准,
	                    check_value as 测量标准,
	                    unit as 单位,
	                    reference_level as 项目引用级别,
	                    sample_num as 试样数量,
	                    remarks as 备注
                    FROM
	                    {TableName} b 
                    WHERE
	                    1 =1 {where}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
                    testitem_code as 检测项编号，
                    testitem_name as 检测项名称，
                        testtype_no as 检测项类型,
                        testtype_name as 检测项类型名称,
                    check_item as 判断标准，
                    check_value as 测量标准，
                    reference_level as 项目引用级别，
                    AQL_LEVEL as AQL等级,
                    sample_num as 试样数量,
                    remarks as 备注
                    FROM
                    {TableName}
                    where 1=1
                    {where}";
                    }
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 查询已勾选3
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCheck3(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TableName = jarr.ContainsKey("TableName") ? jarr["TableName"].ToString().Trim() : "";
                string yq = jarr.ContainsKey("yq") ? jarr["yq"].ToString().Trim() : "";
                string qid = jarr.ContainsKey("qid") ? jarr["qid"].ToString().Trim() : "";
                string did = jarr.ContainsKey("did") ? jarr["did"].ToString().Trim() : "";
                string qno = DB.GetString("select quality_category_no from bdm_generalquality_m where id='" + qid + "'");
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string testitem_name = jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";


                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(TableName))
                {
                    if (TableName == "bdm_qualitytest_item")
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
	                    testtype_no as 检测类型,
	                    testitem_code as 检测项编号,
	                    testitem_name as 检测项名称,
                        testtype_no as 检测项类型,
                        testtype_name as 检测项类型名称,
	                    check_item as 判断标准,
	                    check_value as 测量标准,
	                    unit as 单位,
	                    reference_level as 项目引用级别,
	                    sample_num as 试样数量,
	                    remarks as 备注
                    FROM
	                    {TableName} b  
                    WHERE
	                    1 =1 {where}";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(yq))
                        {
                            where += $@" and general_testtype_no='{yq}'";
                        }
                        if (!string.IsNullOrEmpty(qno))
                        {
                            where += $@" and quality_category_no='{qno}'";
                        }
                        if (!string.IsNullOrEmpty(did))
                        {
                            where += $@" and secondary_category_no='{did}'";
                        }
                        if (!string.IsNullOrEmpty(testitem_code))
                        {
                            where += $@" and testitem_code like '%{testitem_code}%'";
                        }
                        if (!string.IsNullOrEmpty(testitem_name))
                        {
                            where += $@" and testitem_name like '%{testitem_name}%'";
                        }
                        sql = $@"SELECT
                    testitem_code as 检测项编号，
                    testitem_name as 检测项名称，
                        testtype_no as 检测项类型,
                        testtype_name as 检测项类型名称,
                    check_item as 判断标准，
                    check_value as 测量标准，
                    AQL_LEVEL as AQL等级,
                    sample_num as 试样数量,
                    reference_level as 项目引用级别，
                    remarks as 备注
                    FROM
                    {TableName}
                    where 1=1
                    {where}";
                    }
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }


        #region ART保存图片
        /// <summary>
        /// 图片上传
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SavePhotoImgList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string IMG_URL = jarr.ContainsKey("IMG_URL") ? jarr["IMG_URL"].ToString() : "";//图片地址
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";//guid



                #endregion

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                var userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{ReqObj.UserToken}'");
                string createby = userDIC["UserCode"].ToString();
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");
                string staticRoot = @"/wwwroot";

                //string prod_no = dic.ContainsKey("prod_no") ? dic["prod_no"].ToString() : "";

                string img = DB.GetString($"select img_url from bdm_prod_m where prod_no='{prod_no}'");
                if (!string.IsNullOrEmpty(img) || img != "")//查找旧图片，如果存在删除
                {
                    try
                    {
                        string old_imgurl = Directory.GetCurrentDirectory() + staticRoot + img;
                        System.IO.File.Delete($"{old_imgurl}");
                    }
                    catch
                    {
                    }
                }
                DB.ExecuteNonQueryOffline($@"UPDATE bdm_prod_m SET img_url='{ IMG_URL}',guid='{guid}',
                                                        MODIFYBY='{createby}',MODIFYDATE='{createdate}',MODIFYTIME='{createtime}' 
                                                     where prod_no='{prod_no}'");
                #endregion

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();
            }

            return ret;

        }


        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveFile(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase SysDB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART
                string FILE_URL = jarr.ContainsKey("FILE_URL") ? jarr["FILE_URL"].ToString() : "";//文件地址
                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString() : "";//文件名称
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";//guid


                #endregion

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                var userDIC = SysDB.GetDictionary($"select CompanyCode,UserCode from [dbo].[usertoken] where UserToken='{ReqObj.UserToken}'");
                string createby = userDIC["UserCode"].ToString();
                string createdate = DateTime.Now.ToString("yyyy-MM-dd");
                string createtime = DateTime.Now.ToString("HH:mm:ss");

                if (DB.ExecuteNonQueryOffline($@"
                                insert into bdm_prod_customquality_file(prod_no,general_testtype_no,category_no,testitem_code,file_name,file_url,guid,CREATEBY,CREATEDATE,CREATETIME)
                                VALUES('{prod_no}','{general_testtype_no}','{category_no}','{testitem_code}','{FILE_NAME}','{FILE_URL}','{guid}','{createby}','{createdate}','{createtime}')") > 0)
                {
                    DB.Commit();
                    ret.IsSuccess = true;
                    ret.ErrMsg = "上传成功！";
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "上传失败！";
                }
                #endregion

            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();
            }

            return ret;

        }

        #endregion

        /// <summary>
        /// 判断是否通用
        /// </summary>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject IFTY(Object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString().Trim() : "";
                string ifty = DB.GetString($@"select general_category from bdm_general_testtype_m where general_testtype_no='{general_testtype_no}'");
                ret.IsSuccess = true;
                ret.RetData = ifty;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }
    }
}
