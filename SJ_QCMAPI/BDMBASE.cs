using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class BDMBASE
    {
        /// <summary>
        /// 首页检查数据展示之删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMDelect(object OBJ)
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
                string id = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";

                //删除表头
                string sql = $@"delete from BDM_TESTITEM_M where TESTITEM_CODE='{id}'";
                DB.ExecuteNonQuery(sql);
                //删除表身
                string  sql2 = $@"delete from BDM_TESTITEM_D where TESTITEM_CODE='{id}'";
                DB.ExecuteNonQuery(sql2);
                ret.IsSuccess = true;
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
        /// 新增测试项数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMAdd(object OBJ)
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
                string testitem_name = jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";
                string reference_level = jarr.ContainsKey("reference_level") ? jarr["reference_level"].ToString() : "";
                string currency_formula = jarr.ContainsKey("currency_formula") ? jarr["currency_formula"].ToString() : "";
                string testtype_no = jarr.ContainsKey("testtype_no") ? jarr["testtype_no"].ToString() : "";
                string unit = jarr.ContainsKey("unit") ? jarr["unit"].ToString() : "";
                string custom_formula = jarr.ContainsKey("custom_formula") ? jarr["custom_formula"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string sample_num = jarr.ContainsKey("sample_num") ? jarr["sample_num"].ToString() : "";
                string type = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";

                string AQL_LEVEL = jarr.ContainsKey("AQL_LEVEL") ? jarr["AQL_LEVEL"].ToString() : "";
               /* string AC = jarr.ContainsKey("AC") ? jarr["AC"].ToString() : "";
                string RE = jarr.ContainsKey("RE") ? jarr["RE"].ToString() : "";*/
                //取值
                string sql = string.Empty;

                //判断添加的testitem_code有没有重复 
                sql = $@"select*from BDM_TESTITEM_M where testitem_code='{testitem_code}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                    throw new Exception("检验项目编号【" + testitem_code + "】已经存在，请检查！");
                 
                string testtype_name = DB.GetString($"select testtype_name from bdm_testtype_m where testtype_no='{testtype_no}'");

                sql = $@"insert into BDM_TESTITEM_M(testitem_code,testitem_name,reference_level,currency_formula,testtype_no,testtype_name,AQL_LEVEL,unit,
                                                        custom_formula,sample_num,remarks,type,createdate,createtime,createby)
                            values('{testitem_code}','{testitem_name}','{reference_level}','{currency_formula}','{testtype_no}','{testtype_name}','{AQL_LEVEL}','{unit}','{custom_formula}','{sample_num}','{remarks}','{type}','{CommonBASE.GetYMD()}','{CommonBASE.GetTime()}','{CommonBASE.GetUserCode(ReqObj.UserToken)}')";
                DB.ExecuteNonQuery(sql); 
                
                ret.IsSuccess = true; 
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
        /// 修改测试项数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMUpdate(object OBJ)
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

                string user_code = CommonBASE.GetUserCode(ReqObj.UserToken);

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string testitem_name = jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";
                string reference_level = jarr.ContainsKey("reference_level") ? jarr["reference_level"].ToString() : "";
                string currency_formula = jarr.ContainsKey("currency_formula") ? jarr["currency_formula"].ToString() : "";
                string testtype_no = jarr.ContainsKey("testtype_no") ? jarr["testtype_no"].ToString() : "";
                string unit = jarr.ContainsKey("unit") ? jarr["unit"].ToString() : "";
                string custom_formula = jarr.ContainsKey("custom_formula") ? jarr["custom_formula"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string sample_num = jarr.ContainsKey("sample_num") ? jarr["sample_num"].ToString() : "";
                string type = jarr.ContainsKey("cbo_type") ? jarr["cbo_type"].ToString() : ""; 
                string codeid = jarr.ContainsKey("Codeid") ? jarr["Codeid"].ToString() : "";
                string testtype_name = jarr.ContainsKey("testtype_name") ? jarr["testtype_name"].ToString() : "";
                string remarks = jarr.ContainsKey("remarks") ? jarr["remarks"].ToString() : "";

                string AQL_LEVEL = jarr.ContainsKey("AQL_LEVEL") ? jarr["AQL_LEVEL"].ToString() : "";
              /*  string AC = jarr.ContainsKey("AC") ? jarr["AC"].ToString() : "";
                string RE = jarr.ContainsKey("RE") ? jarr["RE"].ToString() : "";*/

                string sql = $@"select * from BDM_TESTITEM_M where testitem_code='{codeid}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count == 0)
                    throw new Exception("检测项编号【"+ codeid + "】不存在，请检查！");
                string old_type = dt.Rows[0]["type"].ToString();

                //类型跟原来的不一样，要删除原类型的明细数据
                if (!type.Equals(old_type))
                { 
                    //删除原本
                    string sqldelete = $@"delete from BDM_TESTITEM_D where testitem_code='{testitem_code}'";  
                    DB.ExecuteNonQuery(sqldelete);
                }
                if (string.IsNullOrEmpty(testtype_name))
                {
                    testtype_name = dt.Rows[0]["testtype_name"].ToString();
                }
                sql = $@"UPDATE BDM_TESTITEM_M SET testitem_name='{testitem_name}',reference_level='{reference_level}',
                                currency_formula='{currency_formula}',testtype_no='{testtype_no}',unit='{unit}',
                                custom_formula='{custom_formula}',testitem_code='{testitem_code}',testtype_name='{testtype_name}',
                                sample_num='{sample_num}',type='{type}',remarks='{remarks}',AQL_LEVEL='{AQL_LEVEL}',
                                modifyby='{user_code}',modifydate='{CommonBASE.GetYMD()}',modifytime='{CommonBASE.GetTime()}'
                                WHERE testitem_code='{codeid}'"; 
                DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
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
        /// 新增测试项标准
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMAddEx(object OBJ)
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
                //
                string value = jarr.ContainsKey("Tx_text") ? jarr["Tx_text"].ToString() : "";
                string min_value = jarr.ContainsKey("Left_text") ? jarr["Left_text"].ToString() : "";
                string max_value = jarr.ContainsKey("Right_text") ? jarr["Right_text"].ToString() : "";
                string remarks = jarr.ContainsKey("richTextBox_remarks") ? jarr["richTextBox_remarks"].ToString() : "";
                string FAGG = jarr.ContainsKey("FAGG") ? jarr["FAGG"].ToString() : "";
                string testitem_codeby = jarr.ContainsKey("testitem_codeby") ? jarr["testitem_codeby"].ToString() : "";
                 
                //取值
                string sql = string.Empty;
                string sql2 = string.Empty;
                string sql3 = string.Empty;
                string testitem_seq = string.Empty;
                sql = $"select max(testitem_seq) from BDM_TESTITEM_D where testitem_code='{testitem_codeby}'";
                string str_seq = DB.GetString(sql);
                if (string.IsNullOrEmpty(str_seq))
                {
                    testitem_seq = "001";
                }
                else
                {
                    testitem_seq = (Convert.ToInt32(str_seq)+10).ToString().PadLeft(3,'0');
                }

                switch (FAGG)
                {
                    //固定值
                    case enum_testitem_type.enum_testitem_type_1:
                        string emy1 = $"select*from BDM_TESTITEM_D where value='{value}' and testitem_code='{testitem_codeby}'";
                        DataTable dt1 = DB.GetDataTable(emy1);
                        if (dt1.Rows.Count > 0)
                        {
                            throw new Exception("已处在该明细，请添加其他新的明细！");
                        }
                        else
                        {
                            sql = $@"insert into  BDM_TESTITEM_D(testitem_code,testitem_seq,value,remarks,createdate,createtime,createby)
                                values('{testitem_codeby}','{testitem_seq}','{value}','{remarks}','{CommonBASE.GetYMD()}','{CommonBASE.GetTime()}','{CommonBASE.GetUserCode(ReqObj.UserToken)}')";
                       /*     sql2 = $@"delete from BDM_TESTITEM_D where min_value is not null and max_value is not null";
                            sql3 = $@"delete from BDM_TESTITEM_D where value is not null and max_value is not null";*/
                        }
                        break;
                    //上下限
                    case enum_testitem_type.enum_testitem_type_2:
                        string emy2 = $"select*from BDM_TESTITEM_D where min_value='{min_value}'and max_value='{max_value}' and testitem_code='{testitem_codeby}'";
                        DataTable dt2 = DB.GetDataTable(emy2);
                        if (dt2.Rows.Count > 0)
                        {
                            throw new Exception("已处在该明细，请添加其他新的明细！");
                        }
                        else
                        {
                            sql = $@"insert into  BDM_TESTITEM_D(testitem_code,testitem_seq,min_value,max_value,remarks,createdate,createtime,createby)
                                    values('{testitem_codeby}','{testitem_seq}','{min_value}','{max_value}','{remarks}','{CommonBASE.GetYMD()}','{CommonBASE.GetTime()}','{CommonBASE.GetUserCode(ReqObj.UserToken)}')";
                          /*  sql2 = $@"delete from BDM_TESTITEM_D where value is not null and max_value is not null";
                            sql3 = $@"delete from BDM_TESTITEM_D where value is not null";*/
                        }
                      
                        break;
                    //误差值
                    case enum_testitem_type.enum_testitem_type_3:
                        string emy3 = $"select*from BDM_TESTITEM_D where value='{value}'and max_value='{max_value}' and testitem_code='{testitem_codeby}'";
                        DataTable dt3 = DB.GetDataTable(emy3);
                        if (dt3.Rows.Count > 0)
                        {
                            throw new Exception("已处在该明细，请添加其他新的明细！");
                        }
                        else
                        {
                            sql = $@"insert into  BDM_TESTITEM_D(testitem_code,testitem_seq,value,error_value,remarks,createdate,createtime,createby) 
                                    values('{testitem_codeby}','{testitem_seq}','{min_value}','{max_value}','{remarks}','{CommonBASE.GetYMD()}','{CommonBASE.GetTime()}','{CommonBASE.GetUserCode(ReqObj.UserToken)}')";
                           /* sql2 = $@"delete from BDM_TESTITEM_D where min_value is not null and max_value is not null";
                            sql3 = $@"delete from BDM_TESTITEM_D where value is not null";*/
                        }
                       
                        break;
                    default:
                        break;
                }
                DB.ExecuteNonQuery(sql);
                /*DB.ExecuteNonQuery(sql2);
                DB.ExecuteNonQuery(sql3);*/
                ret.IsSuccess = true;
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
        /// 删除测试项标准
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMDelete(object OBJ)
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
                string value = jarr.ContainsKey("data") ? jarr["value"].ToString() : "";
                string min_valu = jarr.ContainsKey("data1") ? jarr["txt_min_value"].ToString() : "";
                string max_value = jarr.ContainsKey("data2") ? jarr["txt_max_value"].ToString() : "";
                //取值
                string sql = string.Empty;
                if (string.IsNullOrEmpty(value))
                {
                    sql = $@"insert into  BDM_TESTITEM_D(min_value,max_value,createdate,createby) values({min_valu},{max_value},{005},{DateTime.Now})";
                }
                else
                {
                    sql = $@"insert into  BDM_TESTITEM_D(value,createdate,createby) values({min_valu},{max_value},{005},{DateTime.Now})";
                }

                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMCodeDelect(object OBJ)
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
                string id = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                //转译


                string sql = string.Empty;
              
                if (!string.IsNullOrEmpty(id))
                {
                    sql = $"delete from BDM_TESTITEM_D where id='{id}'";
                }
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
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


    }
}
