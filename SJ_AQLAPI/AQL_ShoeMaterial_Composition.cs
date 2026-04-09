using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SJ_AQLAPI
{
    /// <summary>
    /// 鞋款材料成分维护
    /// </summary>
    public class AQL_ShoeMaterial_Composition
    {
        /// <summary>
        /// 鞋款材料成分维护 
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        //public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaintenanceOfShoeMaterialComposition(object OBJ)
        //{
        //    SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
        //    SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
        //    SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

        //    try
        //    {
        //        #region 参数
        //        string Data = ReqObj.Data.ToString();
        //        var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
        //        string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";
        //        if (string.IsNullOrEmpty(art))
        //        {
        //            ret.IsSuccess = false;
        //            ret.ErrMsg = "art Can not be empty";
        //            return ret;
        //        }
        //        #endregion

        //        #region 逻辑
        //        var artList = art.Split(',').ToList();
        //        List<MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY> reqBodyList = new List<MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY>();
        //        foreach (var item in artList)
        //        {
        //            if (!string.IsNullOrEmpty(item))
        //            {
        //                MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY add = new MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY();
        //                add.MATNR = item;
        //                reqBodyList.Add(add);
        //            }
        //        }

        //        var apires = SJ_SAPAPI.SapApiHelper.GetMaintenanceOfShoeMaterialComposition(reqBodyList.ToArray(), ReqObj);
        //        if (apires != null && apires.E_RESPONSE != null && apires.E_RESPONSE.BODY != null)
        //        {
        //            ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(apires.E_RESPONSE.BODY);
        //        }
        //        else
        //        {
        //            ret.RetData = "";
        //        }
        //        ret.IsSuccess = true;

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        ret.IsSuccess = false;
        //        ret.ErrMsg = ex.Message;
        //    }
        //    return ret;
        //}

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaintenanceOfShoeMaterialComposition(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";
                if (string.IsNullOrEmpty(art))
                {
                    ret.IsSuccess = false;
                    //ret.ErrMsg = "art 不能为空";
                    ret.ErrMsg = "Article cannot be empty";
                    return ret;
                }
                #endregion

                #region 逻辑
                var artList = art.Split(',').ToList();
                List<MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY> reqBodyList = new List<MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY>();
                foreach (var item in artList)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY add = new MaintenanceShoeMaterial.DT_SD105_REQI_REQUESTBODY();
                        add.MATNR = item;
                       // add.LANGU = "1";//1-中文 EN-英文 VI-越文
                        add.LANGU = "EN";//1-中文 EN-英文 VI-越文
                        reqBodyList.Add(add);
                    }
                }

                var apires = SJ_SAPAPI.SapApiHelper.GetMaintenanceOfShoeMaterialComposition(reqBodyList.ToArray(), ReqObj);
                if (apires != null && apires.E_RESPONSE != null && apires.E_RESPONSE.BODY != null)
                {
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(apires.E_RESPONSE.BODY);
                }
                else
                {
                    ret.RetData = "";
                }
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArt(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                #endregion
                #region 逻辑

                var po_info = DB.GetDataTable($@"SELECT DISTINCT PROD_NO FROM BDM_RD_PROD");

                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("art_info", po_info);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                #endregion
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
