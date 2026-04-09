using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_BDMAPI
{
    public class BDM_Equipment
    {
        /// <summary>
        /// 获取设备类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEquipment(object OBJ)
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
                string EQ_NO = jarr.ContainsKey("EQ_NO") ? jarr["EQ_NO"].ToString() : "";//编号
                string EQ_NAME = jarr.ContainsKey("EQ_NAME") ? jarr["EQ_NAME"].ToString() : "";//名称
                string CORRECTION_FREQUENCY = jarr.ContainsKey("CORRECTION_FREQUENCY") ? jarr["CORRECTION_FREQUENCY"].ToString() : "";//校正频率
                string CONTROL_TYPE = jarr.ContainsKey("CONTROL_TYPE") ? jarr["CONTROL_TYPE"].ToString() : "";//类型
                string REMARK = jarr.ContainsKey("REMARK") ? jarr["REMARK"].ToString() : "";//备注
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(EQ_NO))
                {
                    where += $@" and EQ_NO like '%{EQ_NO}%' ";
                }
                if (!string.IsNullOrWhiteSpace(EQ_NAME))
                {
                    where += $@" and EQ_NAME like '%{EQ_NAME}%' ";
                }
                if (!string.IsNullOrWhiteSpace(CORRECTION_FREQUENCY))
                {
                    where += $@" and CORRECTION_FREQUENCY like '%{CORRECTION_FREQUENCY}%' ";
                }
                if (!string.IsNullOrWhiteSpace(CONTROL_TYPE) && CONTROL_TYPE != "0")
                {
                    where += $@" and CONTROL_TYPE like '%{CONTROL_TYPE}%' ";
                }
                if(CONTROL_TYPE == "4")
                {
                    where += $@" and CONTROL_TYPE NOT IN ('1','2','3') ";
                }
                if (!string.IsNullOrWhiteSpace(REMARK))
                {
                    where += $@" and REMARK like '%{REMARK}%' ";
                }
                string sql = $@"SELECT id AS 唯一ID,rownum AS LINE,EQ_NO,EQ_NAME,CORRECTION_FREQUENCY,CONTROL_TYPE,REMARK FROM BDM_EQ_TYPE_M WHERE 1 = 1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                //foreach (DataRow item in dt.Rows)
                //{
                //    switch (item["CONTROL_TYPE"])
                //    {
                //        case "0":
                //            item["CONTROL_TYPE"] = "全部";
                //            break;
                //        case "1":
                //            item["CONTROL_TYPE"] = "制程机器";
                //            break;
                //        case "2":
                //            item["CONTROL_TYPE"] = "检验工具";
                //            break;
                //        case "3":
                //            item["CONTROL_TYPE"] = "测试设备";
                //            break;
                //        case "4":
                //            item["CONTROL_TYPE"] = "其他";
                //            break;
                //        default:
                //            break;
                //    }
                //}
                foreach (DataRow item in dt.Rows)
                {
                    switch (item["CONTROL_TYPE"])
                    {
                        case "0":
                            item["CONTROL_TYPE"] = "All";
                            break;
                        case "1":
                            item["CONTROL_TYPE"] = "Process_Machine";
                            break;
                        case "2":
                            item["CONTROL_TYPE"] = "Validation_Tools";
                            break;
                        case "3":
                            item["CONTROL_TYPE"] = "Test_Equipment";
                            break;
                        case "4":
                            item["CONTROL_TYPE"] = "Other";
                            break;
                        default:
                            break;
                    }
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 过去编辑设备类型信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEquipmentInfo(object OBJ)
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
                string EQ_NO = jarr.ContainsKey("EQ_NO") ? jarr["EQ_NO"].ToString() : "";//编号

                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(EQ_NO))
                {
                    where += $@" and EQ_NO = '{EQ_NO}' ";
                }

                string sql = $@"SELECT EQ_NO,EQ_NAME,CORRECTION_FREQUENCY,control_type,REMARK FROM BDM_EQ_TYPE_M WHERE 1 = 1 {where}";
                DataTable dt = DB.GetDataTable(sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 新增设备类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddEquipment(object OBJ)
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
                string EQ_NO = jarr.ContainsKey("EQ_NO") ? jarr["EQ_NO"].ToString() : "";//编号
                string EQ_NAME = jarr.ContainsKey("EQ_NAME") ? jarr["EQ_NAME"].ToString() : "";//名称
                string CORRECTION_FREQUENCY = jarr.ContainsKey("CORRECTION_FREQUENCY") ? jarr["CORRECTION_FREQUENCY"].ToString() : "";//校正频率
                string control_type = jarr.ContainsKey("control_type") ? jarr["control_type"].ToString() : "";//管控类型（设备）
                string remark = jarr.ContainsKey("remark") ? jarr["remark"].ToString() : "";//备注

                int isHave = DB.GetInt32($@"SELECT * FROM BDM_EQ_TYPE_M WHERE EQ_NO = '{EQ_NO}'");
                if(isHave >0)
                {
                    ret.ErrMsg = "same number already exists！";
                    ret.IsSuccess = false;
                    return ret;
                }

                Dictionary<string, object> ReqP = new Dictionary<string, object>();
                ReqP.Add("EQ_NO", EQ_NO);
                ReqP.Add("EQ_NAME", EQ_NAME);
                ReqP.Add("CORRECTION_FREQUENCY", CORRECTION_FREQUENCY);
                ReqP.Add("control_type", control_type);
                ReqP.Add("remark", remark);

                ReqP.Add("CREATEBY",SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                ReqP.Add("CREATEDATE", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CREATETIME", DateTime.Now.ToString("HH:mm:ss"));

                string sql = $@"INSERT INTO BDM_EQ_TYPE_M (EQ_NO,EQ_NAME,CORRECTION_FREQUENCY,control_type,remark,CREATEBY,CREATEDATE,CREATETIME) 
											        VALUES(@EQ_NO,@EQ_NAME,@CORRECTION_FREQUENCY,@control_type,@remark,@CREATEBY,@CREATEDATE,@CREATETIME)";

                int count = DB.ExecuteNonQuery(sql, ReqP);
                //if (count > 0)
                //{
                    ret.ErrMsg = "Saved successfully！";
                    DB.Commit();
                    ret.IsSuccess = true;
                //}
                //else
                //{
                //    ret.ErrMsg = "保存失败，原因：" + ex.Message;
                //    DB.Commit();
                //    ret.IsSuccess = false;
                //}

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
        /// 设备类型删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteEquipmentType(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                DB.Open();
                DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                DB.ExecuteNonQuery($@"delete from bdm_eq_type_m where id={id}");
                DB.ExecuteNonQuery($@"delete from bdm_eq_type_d where id={id}");
                DB.Commit();//提交事务
                ret.IsSuccess = true;
                #endregion
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

        #region 编辑

        /// <summary>
        /// 更新设备类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateEquipment(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string CORRECTION_FREQUENCY = jarr.ContainsKey("CORRECTION_FREQUENCY") ? jarr["CORRECTION_FREQUENCY"].ToString() : "";

                //Dictionary<string, object> ReqP = new Dictionary<string, object>();
                //ReqP.Add("ID", id);
                //ReqP.Add("CORRECTION_FREQUENCY", CORRECTION_FREQUENCY);

                string sql = $@"UPDATE BDM_EQ_TYPE_M SET CORRECTION_FREQUENCY='{CORRECTION_FREQUENCY}' WHERE ID = '{id}'";

                DB.ExecuteNonQuery(sql);
                ret.ErrMsg = "Update completed！";
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
        /// 编辑更新设备类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateEquipmentInfo(object OBJ)
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
                string EQ_NO = jarr.ContainsKey("EQ_NO") ? jarr["EQ_NO"].ToString() : "";
                string EQ_NAME = jarr.ContainsKey("EQ_NAME") ? jarr["EQ_NAME"].ToString() : "";
                string CORRECTION_FREQUENCY = jarr.ContainsKey("CORRECTION_FREQUENCY") ? jarr["CORRECTION_FREQUENCY"].ToString() : "";
                string REMARK = jarr.ContainsKey("REMARK") ? jarr["REMARK"].ToString() : "";
                string control_type = jarr.ContainsKey("control_type") ? jarr["control_type"].ToString() : "";


                string sql = $@"UPDATE BDM_EQ_TYPE_M SET EQ_NAME='{EQ_NAME}',CORRECTION_FREQUENCY='{CORRECTION_FREQUENCY}',control_type='{control_type}',REMARK='{REMARK}' WHERE EQ_NO = '{EQ_NO}'";

                DB.ExecuteNonQuery(sql);
                ret.ErrMsg = "Update completed！";
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

        #endregion

        #region 关联项目

        /// <summary>
        /// 获取项目内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetEquipment_type_d(object OBJ)
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
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";//
                string EQ_TYPE = jarr.ContainsKey("EQ_TYPE") ? jarr["EQ_TYPE"].ToString() : "";//0：校正项目；1：参数项目

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;


                string sql = $@"SELECT rownum AS LINE ,ID AS 唯一ID,ITEM_CODE,ITEM_NAME FROM BDM_EQ_TYPE_D  WHERE EQ_TYPE = '{EQ_TYPE}' AND M_ID = '{id}'";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 新增项目内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddEquipment_type_d(object OBJ)
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
                string M_ID = jarr.ContainsKey("M_ID") ? jarr["M_ID"].ToString() : "";//
                string ITEM_CODE = jarr.ContainsKey("ITEM_CODE") ? jarr["ITEM_CODE"].ToString() : "";//
                string ITEM_NAME = jarr.ContainsKey("ITEM_NAME") ? jarr["ITEM_NAME"].ToString() : "";//
                string EQ_TYPE = jarr.ContainsKey("EQ_TYPE") ? jarr["EQ_TYPE"].ToString() : "";//0：校正项目；1：参数项目



                Dictionary<string, object> ReqP = new Dictionary<string, object>();
                ReqP.Add("M_ID", M_ID);
                ReqP.Add("ITEM_CODE", ITEM_CODE);
                ReqP.Add("ITEM_NAME", ITEM_NAME);
                ReqP.Add("EQ_TYPE", EQ_TYPE);

                ReqP.Add("CREATEBY", SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken));
                ReqP.Add("CREATEDATE", DateTime.Now.ToString("yyyy-MM-dd"));
                ReqP.Add("CREATETIME", DateTime.Now.ToString("HH:mm:ss"));

                //string sql = $@"INSERT INTO BDM_EQ_TYPE_D (M_ID,ITEM_CODE,ITEM_NAME,EQ_TYPE,CREATEBY,CREATEDATE,CREATETIME) 
                //   VALUES(@M_ID,@ITEM_CODE,@ITEM_NAME,@EQ_TYPE,@CREATEBY,@CREATEDATE,@CREATETIME)";
                string sql = $@"INSERT INTO BDM_EQ_TYPE_D (M_ID,ITEM_CODE,ITEM_NAME,EQ_TYPE,CREATEBY,CREATEDATE,CREATETIME) 
											        VALUES('{M_ID}','{ITEM_CODE}','{ITEM_NAME}','{EQ_TYPE}','{SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken)}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Saved successfully！";
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
        /// 删除项目内容
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteEquipment_type_d(object OBJ)
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
                string M_ID = jarr.ContainsKey("M_ID") ? jarr["M_ID"].ToString() : "";//主表id
                string D_ID = jarr.ContainsKey("D_ID") ? jarr["D_ID"].ToString() : "";//子表id

                string sql = $@"delete from bdm_eq_type_d where m_id='{M_ID}' and id='{D_ID}'";

                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.ErrMsg = "Successfully deleted！";
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

        #endregion

        /// <summary>
        /// 获取参数内容或者校正数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetDeviceType_CP_Item(object OBJ)
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
                string EQ_TYPE = jarr.ContainsKey("EQ_TYPE") ? jarr["EQ_TYPE"].ToString() : "";//0：校正项目；1：参数项目
                string KeyCode= jarr.ContainsKey("KeyCode") ? jarr["KeyCode"].ToString() : "";//条件

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                string sql = string.Empty;
                if (EQ_TYPE=="0")
                {
                    if (!string.IsNullOrEmpty(KeyCode))
                    {
                        where = $@" and (CORRECTION_ITEM_CODE like '%{KeyCode}%' or CORRECTION_ITEM_NAME like '%{KeyCode}%')";
                    }
                    sql = $@"SELECT
	                            CORRECTION_ITEM_CODE as code,
	                            CORRECTION_ITEM_NAME as name
                            FROM
	                            bdm_correction_item_m where 1=1 {where}";
                }
                if (EQ_TYPE == "1")
                {
                    if (!string.IsNullOrEmpty(KeyCode))
                    {
                        where = $@" and (PARAM_ITEM_NO like '%{KeyCode}%' or PARAM_ITEM_NAME like '%{KeyCode}%')";
                    }
                    sql = $@"SELECT
	                            PARAM_ITEM_NO as code,
	                            PARAM_ITEM_NAME as name
                            FROM
	                            bdm_param_item_m where 1=1 {where}";
                }

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
    }
}
