using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 鞋面品质审核类
    /// </summary>
    public class VampQuality
    {
        /// <summary>
        /// 获取鞋面品质审核基础信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetVampQualityInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string QUALITY_TYPE = jarr.ContainsKey("QUALITY_TYPE") ? jarr["QUALITY_TYPE"].ToString() : "";//
                

                #endregion

                #region 接口验证
                #endregion

                #region 逻辑
                string WHERE = string.Empty;

                if (!string.IsNullOrEmpty(QUALITY_TYPE))
                    WHERE += $@" AND (QUALITY_TYPE_CODE='{QUALITY_TYPE}' or QUALITY_TYPE_NAME LIKE='{QUALITY_TYPE}')";

                string sql = $"SELECT ID,QUALITY_TYPE_CODE,QUALITY_TYPE_NAME,QUALITY_ITEM_CODE,QUALITY_ITEM_NAME,TYPE,BASE_SOCRE,'' as SOCRE,'' as URL FROM BDM_VAMP_QUALITY_M WHERE 1=1 {WHERE} ORDER BY QUALITY_TYPE_CODE ASC,NVL(QUALITY_ITEM_CODE,'0') ASC  ";


                DataTable dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData1 = dt;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            return ret;

        }

        /// <summary>
        /// Uni App 鞋面品质检验提交接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveVampQuality(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                #region 接口参数

                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";//
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//
                string TOTAL_SOCRE = jarr.ContainsKey("TOTAL_SOCRE") ? jarr["TOTAL_SOCRE"].ToString() : "";//
                DataTable QUALITY_DATA = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["QUALITY_DATA"].ToString());
                #endregion
                #region 接口验证
                #endregion

                #region 逻辑

                string guid_m = Guid.NewGuid().ToString("N");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DB.Open();
                DB.BeginTransaction();
                DB.ExecuteNonQuery($@"INSERT INTO QCM_VAMP_QUALITY_M
                                    (
                                       GUID,
                                       SUPPLIERS_CODE,
                                       SUPPLIERS_NAME,
                                       QUALITY_DATE,
                                       SOCRE,
                                       CREATEBY,
                                       CREATEDATE,
                                       CREATETIME
                                    ) 
                                    VALUES
                                    (
                                       '{guid_m}',
                                       '{SUPPLIERS_CODE}',
                                       '{SUPPLIERS_NAME}',
                                       '{date}',
                                       '{TOTAL_SOCRE}',
                                       '{usercode}',
                                       '{date}',
                                       '{time}'
                                    )");

                foreach (DataRow dr in QUALITY_DATA.Rows)
                {
                    string guid_d = Guid.NewGuid().ToString("N");
                    DB.ExecuteNonQuery($@"INSERT INTO QCM_VAMP_QUALITY_D
                                        (
                                           M_GUID,
                                           GUID,
                                           QUALITY_TYPE_CODE,
                                           QUALITY_TYPE_NAME,
                                           QUALITY_ITEM_CODE,
                                           QUALITY_ITEM_NAME,
                                           BASE_SOCRE,
                                           SOCRE,
                                           TYPE,
                                           CREATEBY,
                                           CREATEDATE,
                                           CREATETIME
                                        ) 
                                        VALUES
                                        (
                                           '{guid_m}',
                                           '{guid_d}',
                                           '{dr["QUALITY_TYPE_CODE"]}',
                                           '{dr["QUALITY_TYPE_NAME"]}',
                                           '{dr["QUALITY_ITEM_CODE"]}',
                                           '{dr["QUALITY_ITEM_NAME"]}',
                                           '{dr["BASE_SOCRE"]}',
                                           '{dr["SOCRE"]}',
                                           '{dr["TYPE"]}',
                                           '{usercode}',
                                           '{date}',
                                           '{time}'
                                        )");

                    if(!string.IsNullOrEmpty(dr["url"].ToString()))
                    {
                        foreach (var item in dr["url"].ToString().Split())
                        {
                            string guid = Guid.NewGuid().ToString("N");
                            DB.ExecuteNonQuery($@"INSERT INTO QCM_VAMP_QUALITY_IMAGEURL
                                    (
                                       D_GUID,
                                       GUID,
                                       IMG_NAME,
                                       IMG_URL,
                                       CREATEBY,
                                       CREATEDATE,
                                       CREATETIME
                                    ) 
                                    VALUES
                                    (
                                       '{guid_d}',
                                       '{guid}',
                                       '{item.Substring(item.LastIndexOf(@"/") + 1)}',
                                       '{item}',
                                       '{usercode}',
                                       '{date}',
                                       '{time}'
                                    )");
                        }
                    }
                    
                }
                DB.Commit();
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally {
                DB.Close();
            }
            return ret;

        }
    }
}
