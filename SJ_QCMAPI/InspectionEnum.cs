using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class InspectionEnum
    {
        /// <summary>
        /// 通用检测枚举
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getbdm_general_testtype_m_Meun(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"select general_testtype_no,general_testtype_name from bdm_general_testtype_m";
                DataTable dt1 = DB.GetDataTable(sql);
                dic.Add("data1", dt1);
               

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 通用公式编号
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getbdm_general_testtype_m_No(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string general_testtype_no = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";


                Dictionary<string, object> dic = new Dictionary<string, object>();

                string GENERAL_CATEGORY = DB.GetString($@"select GENERAL_CATEGORY from bdm_general_testtype_m where general_testtype_no='{general_testtype_no}'");
                string sql = string.Empty;
                if (GENERAL_CATEGORY =="0")
                {
                    sql = $@"select SECONDARY_CATEGORY_NO as aa,SECONDARY_CATEGORY_NAME as bb from bdm_generalquality_d where general_testtype_no='{general_testtype_no}'";
                }
                if (GENERAL_CATEGORY == "1")
                {
                    sql = $@"select QUALITY_CATEGORY_NO as aa,QUALITY_CATEGORY_NAME as bb from bdm_generalquality_m where general_testtype_no='{general_testtype_no}'";
                }

                DataTable dt1 = DB.GetDataTable(sql);
                dic.Add("data1",dt1);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getbdm_general_testtype_m_Nos(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PARENT_ITEM_NO = jarr.ContainsKey("PARENT_ITEM_NO") ? jarr["PARENT_ITEM_NO"].ToString() : "";


                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"select GENERAL_TESTTYPE_NO,GENERAL_TESTTYPE_NAME,Prod_NO from BDM_PROD_customquality_m where  prod_no='{PARENT_ITEM_NO}'";
               
                DataTable dt1 = DB.GetDataTable(sql);
                dic.Add("data1", dt1);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getbdm_general_testtype_m_Nosd(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                //DB.Open();
                //DB.BeginTransaction();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";


                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"select CATEGORY_NO,CATEGORY_NAME From BDM_PROD_CUSTOMQUALITY_D where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}'";
                DataTable dt1 = DB.GetDataTable(sql);
                dic.Add("data1", dt1);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

    }
}
