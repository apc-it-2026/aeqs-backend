using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class ChemicalcontainermBase
    {
        /// <summary>
        /// 化学品容器管理数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CONTAINER_NO = jarr.ContainsKey("CONTAINER_NO") ? jarr["CONTAINER_NO"].ToString() : "";//容器编号
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string sql = $@"select*from qcm_chemical_container_m where container_no='{CONTAINER_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg =ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 平板端扫描容器二维码接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermViewS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CONTAINER_NO = jarr.ContainsKey("CONTAINER_NO") ? jarr["CONTAINER_NO"].ToString() : "";//容器编号
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string sql = $@"select CONTAINER_NO,CONTAINER_NAME,CHEMICAL_NO,CHEMICAL_NAME,PRODUCTIONLINE_NO,PRODUCTIONLINE_NAME,EFFECTIVE_TIME from bdm_Containerinformation_m  where CONTAINER_NO='{CONTAINER_NO}'";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count>0)
                {
                    ret.IsSuccess = true;
                    ret.RetData1 = dt;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $"扫描失败，原因：容器编码【{CONTAINER_NO}】不存在";
                }
              
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 平板端扫描胶水二维码接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermViewSD(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string CHEMICAL_NO = jarr.ContainsKey("CHEMICAL_NO") ? jarr["CHEMICAL_NO"].ToString() : "";//胶水编号
                string CHEMICAL_NOS = jarr.ContainsKey("CHEMICAL_NOS") ? jarr["CHEMICAL_NOS"].ToString() : "";//材料编号
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = DB.GetDataTable($"select CHEMICAL_NO,CHEMICAL_NAME, EFFECTIVE_TIME from bdm_Chemicalglue_m where CHEMICAL_NO='{CHEMICAL_NO}'");//查询胶水信息

               /* //查询化学容器的材料清单（胶水代号）
                string CHEMICAL_NOSD = DB.GetString($"select CHEMICAL_NO from bdm_Containerinformation_m where CHEMICAL_NO='{CHEMICAL_NO}'");*/
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["CHEMICAL_NO"].ToString().Equals(CHEMICAL_NOS))
                    {
                        ret.IsSuccess = true;
                        ret.RetData1 = dt;
                        ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                      
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "胶水信息与所属化学品信息不一致，确认是否正确！！！";
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $"扫描失败，原因：胶水编码【{CHEMICAL_NO}】不存在";
                }
               

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 化学品容器管理修改调胶时间和到期时间
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermUpdate(object OBJ)
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

                string CONTAINER_NO = jarr.ContainsKey("CONTAINER_NO") ? jarr["CONTAINER_NO"].ToString() : "";//容器编号
                string CONTAINER_NAME = jarr.ContainsKey("CONTAINER_NAME") ? jarr["CONTAINER_NAME"].ToString() : "";//容器名称

                string PRODUCTIONLINE_NO = jarr.ContainsKey("PRODUCTIONLINE_NO") ? jarr["PRODUCTIONLINE_NO"].ToString() : "";//产线代号
                string PRODUCTIONLINE_NAME = jarr.ContainsKey("PRODUCTIONLINE_NAME") ? jarr["PRODUCTIONLINE_NAME"].ToString() : "";//产线名称

                string CHEMICAL_NO = jarr.ContainsKey("CHEMICAL_NO") ? jarr["CHEMICAL_NO"].ToString() : "";//材料单号（胶水代号）
                string CHEMICAL_NAME = jarr.ContainsKey("CHEMICAL_NAME") ? jarr["CHEMICAL_NAME"].ToString() : "";//材料名称（胶水名称）

                string EFFECTIVE_TIME = jarr.ContainsKey("EFFECTIVE_TIME") ? jarr["EFFECTIVE_TIME"].ToString() : "";//有效时间
                string GLUE_TIME = jarr.ContainsKey("GLUE_TIME") ? jarr["GLUE_TIME"].ToString() : "";//调胶时间
                string EXPIRATION_TIME = jarr.ContainsKey("EXPIRATION_TIME") ? jarr["EXPIRATION_TIME"].ToString() : "";//到期时间

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                string sql = string.Empty;
                string aa = DB.GetString($"select CONTAINER_NO from qcm_chemical_container_m where CONTAINER_NO='{CONTAINER_NO}'and CONTAINER_NAME='{CONTAINER_NAME}' and CHEMICAL_NO='{CHEMICAL_NO}' and CHEMICAL_NAME='{CHEMICAL_NAME}'");
                if (string.IsNullOrWhiteSpace(aa))
                {
                    sql = $"insert into qcm_chemical_container_m(CONTAINER_NO,CONTAINER_NAME,productionline_no,productionline_name,CHEMICAL_NO,CHEMICAL_NAME,EFFECTIVE_TIME,GLUE_TIME,EXPIRATION_TIME,CREATEBY,CREATEDATE,CREATETIME) VALUES('{CONTAINER_NO}','{CONTAINER_NAME}','{PRODUCTIONLINE_NO}','{PRODUCTIONLINE_NAME}','{CHEMICAL_NO}','{CHEMICAL_NAME}','{EFFECTIVE_TIME}','{GLUE_TIME}','{EXPIRATION_TIME}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
                } 
                else
                {
                     sql = $@"update qcm_chemical_container_m set GLUE_TIME='{GLUE_TIME}',EXPIRATION_TIME='{EXPIRATION_TIME}',MODIFYBY='{CreactUserId}',MODIFYDATE='{DateTime.Now.ToString("yyyy-MM-dd")}',MODIFYTIME='{DateTime.Now.ToString("HH:mm:ss")}'  where container_no='{CONTAINER_NO}'";
                }
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "提交成功";
               
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "提交失败，原因："+ ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 化学品容器管理看板
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermKbList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
              
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CONTAINER_NO = jarr.ContainsKey("CONTAINER_NO") ? jarr["CONTAINER_NO"].ToString() : "";
                string CHEMICAL_NAME = jarr.ContainsKey("CHEMICAL_NAME") ? jarr["CHEMICAL_NAME"].ToString() : "";

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(CONTAINER_NO))
                {
                    strwhere += $@" and CONTAINER_NO like '%{CONTAINER_NO}%'";
                }
                if (!string.IsNullOrEmpty(CHEMICAL_NAME))
                {
                    strwhere += $@" and CHEMICAL_NAME like '%{CHEMICAL_NAME}%'";
                }
              

                sql = $@"select CONTAINER_NO,CHEMICAL_NAME,GLUE_TIME,EFFECTIVE_TIME,EXPIRATION_TIME from  QCM_CHEMICAL_CONTAINER_M   where 1=1 {strwhere} order by ID desc";
                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 返回胶水所有信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermJSView(object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string CHEMICAL_NO = jarr.ContainsKey("CHEMICAL_NO") ? jarr["CHEMICAL_NO"].ToString() : "";//胶水编号
                string where = string.Empty;
                if (!string.IsNullOrEmpty(CHEMICAL_NO))
                    where += " and CHEMICAL_NO like '%" + CHEMICAL_NO + "%' or CHEMICAL_NAME like  '%" + CHEMICAL_NO + "%'";
                DataTable dt = DB.GetDataTable($"select CHEMICAL_NO,CHEMICAL_NAME,EFFECTIVE_TIME from BDM_CHEMICALGLUE_M where 1=1 {where} order by id desc");
                if (dt.Rows.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData1 = dt;
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查询无数据";
                }
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "查询失败，原因：" + ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 提交到打印数据信息表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ChemicalcontainermAdd(object OBJ)
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

                

                string CHEMICAL_NO = jarr.ContainsKey("CHEMICAL_NO") ? jarr["CHEMICAL_NO"].ToString() : "";//材料单号（胶水代号）
                string CHEMICAL_NAME = jarr.ContainsKey("CHEMICAL_NAME") ? jarr["CHEMICAL_NAME"].ToString() : "";//材料名称（胶水名称）

               // decimal GLUE_TIME = jarr.ContainsKey("GLUE_TIME") ? Convert.ToDecimal(jarr["GLUE_TIME"]) : 0;//调胶时间

                string EFFECTIVE_TIME = jarr.ContainsKey("EFFECTIVE_TIME") ? jarr["EFFECTIVE_TIME"].ToString() : "";//调胶时间

                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID

                //保存信息到打印表bdm_printdata
                string EXPIRATION_TIMES = DateTime.Now.AddHours(Convert.ToDouble(EFFECTIVE_TIME)).ToString("yyyy-MM-dd HH:mm:ss");//到期时间

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add("CHEMICAL_NO", CHEMICAL_NO);
                dic.Add("CHEMICAL_NAME", CHEMICAL_NAME);
                dic.Add("GLUE_TIME", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                dic.Add("EXPIRATION_TIME", EXPIRATION_TIMES);

                string jsons = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                DB.ExecuteNonQuery($"insert into BDM_PRINTDATA(PRINT_TYPE,PRINT_CONTENT,IS_PRINT,CREATEBY,CREATEDATE,CREATETIME) VALUES('1','{jsons}','N','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')");
               
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "提交成功";

            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "提交失败，原因：" + ex.Message;
            }
            return ret;
        }
    }
}
