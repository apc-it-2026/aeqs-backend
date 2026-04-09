using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class VampQualityAudit
    {
        /// <summary>
        /// 鞋面品质标准查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Selectbdm_vamp_quality_m(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> dd = new Dictionary<string, object>();
                string sql = $@"select * from bdm_vamp_quality_m 
                                order by  quality_type_code asc，quality_item_code desc";

                string zs = DB.GetString($@"select SUM(BASE_SOCRE) zs from bdm_vamp_quality_m where type=1");
                DataTable dt = DB.GetDataTable(sql);

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("zs", zs);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
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
        /// 鞋面品质标准保存接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Insertbdm_vamp_quality_m(object OBJ)
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
                Dictionary<string, object> dd = new Dictionary<string, object>();
                DataTable bdm_vamp_quality_m = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["bdm_vamp_quality_m"].ToString());//表身数据
                string where = string.Empty;
                string sql = string.Empty;
                DataRow[] drr = bdm_vamp_quality_m.Select("TYPE=0");
                foreach (DataRow item in drr)
                {
                    
                    DataRow[] drr2 = bdm_vamp_quality_m.Select("TYPE=1 and QUALITY_TYPE_CODE = '"+item["QUALITY_TYPE_CODE"] + "'");
                    double socre = 0;
                    foreach (var item2 in drr2)
                    {
                        socre += Convert.ToDouble(item2["SOCRE"].ToString()==""?0: item2["SOCRE"]);
                    }
                    DB.ExecuteNonQuery($@"update bdm_vamp_quality_m set BASE_SOCRE='{socre}' where type=0 and QUALITY_TYPE_CODE='{item["QUALITY_TYPE_CODE"]}'");
                }

                foreach (DataRow item in bdm_vamp_quality_m.Rows)
                {
                    where = $" and QUALITY_TYPE_CODE='{item["QUALITY_TYPE_CODE"]}' and QUALITY_ITEM_CODE='{item["QUALITY_ITEM_CODE"]}'";
                    sql = $@"update bdm_vamp_quality_m set QUALITY_TYPE_NAME='{item["QUALITY_TYPE_NAME"]}',QUALITY_ITEM_NAME='{item["QUALITY_ITEM_NAME"]}',BASE_SOCRE='{item["SOCRE"]}' where 1=1 {where}";
                    DB.ExecuteNonQuery(sql);
                }
                #endregion
                ret.IsSuccess = true;
                DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
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
        /// 鞋面品质审核列表查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitM_VAMP_QUALITY_M(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//厂商名称
                string QUALITY_DATE = jarr.ContainsKey("QUALITY_DATE") ? jarr["QUALITY_DATE"].ToString() : "";//日期
                string where = string.Empty;
                if (!string.IsNullOrEmpty(SUPPLIERS_NAME))
                {
                    where += $@" and SUPPLIERS_NAME like '%{SUPPLIERS_NAME}%'";
                }
                if (!string.IsNullOrEmpty(QUALITY_DATE))
                {
                    where += $@" and QUALITY_DATE='{QUALITY_DATE}'";
                }

                string sql = $@"select * from QCM_VAMP_QUALITY_M where 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
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
        /// 鞋面品质审核列表明细查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitQCM_VAMP_QUALITY_D(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string M_GUID = jarr.ContainsKey("guid_m") ? jarr["guid_m"].ToString() : "";//主表guid
                string where = string.Empty;
                if (!string.IsNullOrEmpty(M_GUID))
                {
                    where += $" and M_GUID='{M_GUID}'";
                }
                string sql = $@"select * from QCM_VAMP_QUALITY_D where 1=1 {where}
                                order by  QUALITY_ITEM_CODE asc，QUALITY_TYPE_CODE desc ";
                DataTable dt = DB.GetDataTable(sql);

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
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
        /// 鞋面品质审核列表明细照片查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitQCM_VAMP_QUALITY_IMAGEURL(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();

                //打开事务
                //DB.Open();
                //DB.BeginTransaction();
                #region 逻辑

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string D_GUID = jarr.ContainsKey("D_GUID") ? jarr["D_GUID"].ToString() : "";//主表guid
                string where = string.Empty;
                if (!string.IsNullOrEmpty(D_GUID))
                {
                    where += $" and D_GUID='{D_GUID}'";
                }
                string sql = $@"select IMG_NAME,IMG_URL from QCM_VAMP_QUALITY_IMAGEURL where 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);

                #endregion
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                /*DB.Rollback();*///回滚事务
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;

        }
    }
}
