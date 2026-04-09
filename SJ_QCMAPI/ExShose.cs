using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SJ_QCMAPI
{
    public class ExShose
    {
        #region 获取下拉数据源

        ///获取阶段基础信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetJDInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                var dt = DB.GetDataTable($@"select PHASE_CREATION_NO as  code ,PHASE_CREATION_NAME as name from BDM_PHASE_CREATION_M WHERE 1=1 {(string.IsNullOrEmpty(keyword) ? "" : $" AND (PHASE_CREATION_NO like '%{keyword}%' or PHASE_CREATION_NAME like '%{keyword}%')")}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取Size基础信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSizeInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                List<string> sizelist = new List<string>();
                string sql = "SELECT SHOE_SIZE FROM BDM_SHOE_SIZE_M";
                DataTable dt = DB.GetDataTable(sql);
                foreach (DataRow item in dt.Rows)
                {
                    sizelist.Add(item["SHOE_SIZE"].ToString());
                }
                //var dt = DB.GetDataTable($@"select PHASE_CREATION_NO,PHASE_CREATION_NAME from BDM_PHASE_CREATION_M WHERE 1=1 {(string.IsNullOrEmpty(keyword)?"": $" AND (PHASE_CREATION_NO like '%{keyword}%' or PHASE_CREATION_NAME like '%{keyword}%')")}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(sizelist);
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取FGT测试类型信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFGTInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                var dt = DB.GetDataTable($@"select fgt_code as  code ,fgt_name as name from bdm_fgt_m");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取Category
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCategoryInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (category_code like '%{keyword}%' or category_name like '%{keyword}%')";
                }

                string sql = $"select category_code as code,category_name as name from bdm_category_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取新旧级别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetXJJBInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (pb_type_code like '%{keyword}%' or pb_type_level like '%{keyword}%')";
                }

                string sql = $"select pb_type_code as code,pb_type_level as name from bdm_pb_type_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取年龄性别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAgeSexInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (age_gender_code like '%{keyword}%' or age_gender_name like '%{keyword}%')";
                }

                string sql = $"select age_gender_code as code,age_gender_name as name from bdm_age_gender_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取产品级别
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetProductLevel(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (PRODUCT_LEVEL_CODE like '%{keyword}%' or PRODUCT_LEVEL_VALUE like '%{keyword}%')";
                }

                string sql = $"select PRODUCT_LEVEL_CODE as code,PRODUCT_LEVEL_VALUE as name from bdm_product_level_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取成品种类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCPTypeInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (finished_product_code like '%{keyword}%' or finished_product_name like '%{keyword}%')";
                }

                string sql = $"select finished_product_code as code,finished_product_name as name from bdm_finished_product_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取部件信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPARTSInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (PARTS_CODE like '%{keyword}%' or PARTS_NAME like '%{keyword}%')";
                }

                string sql = $"select PARTS_CODE as code,PARTS_NAME as name from BDM_PARTS_M where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取部位信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPOSITIONInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (POSITION_CODE like '%{keyword}%' or POSITION_NAME like '%{keyword}%')";
                }

                string sql = $"select POSITION_CODE as code,POSITION_NAME as name from bdm_position_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取产线信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetLineInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (PRODUCTION_LINE_CODE like '%{keyword}%' or PRODUCTION_LINE_NAME like '%{keyword}%')";
                }

                string sql = $"select PRODUCTION_LINE_CODE as code, PRODUCTION_LINE_NAME as name from bdm_production_line_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取材料种类信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMaterialTypeInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (MATERIAL_TYPE_CODE like '%{keyword}%' or MATERIAL_TYPE_NAME like '%{keyword}%')";
                }

                string sql = $"select MATERIAL_TYPE_CODE as code,MATERIAL_TYPE_NAME as name from bdm_material_type_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取工艺信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetWorkmanShipInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (WORKMANSHIP_CODE like '%{keyword}%' or WORKMANSHIP_NAME like '%{keyword}%')";
                }

                string sql = $"select WORKMANSHIP_CODE as code,WORKMANSHIP_NAME as name from bdm_workmanship_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取自定义公式类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_D_formula_type(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (FORMULA_CODE like '%{keyword}%' or FORMULA_NAME like '%{keyword}%')";
                }

                string sql = $"select FORMULA_CODE as code,FORMULA_NAME as name from bdm_formula_m where 1=1 {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取通用公式类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_G_formula_type(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (ENUM_CODE like '%{keyword}%' or ENUM_VALUE like '%{keyword}%')";
                }

                string sql = $"select ENUM_CODE as code,ENUM_VALUE as name from SYS001M where enum_type='enum_general_formula' {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取判断标准
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_JUDGMENT_CRITERIA(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (ENUM_CODE like '%{keyword}%' or ENUM_VALUE like '%{keyword}%')";
                }

                string sql = $"select ENUM_CODE as code,ENUM_VALUE as name from SYS001M where enum_type='enum_judgment_criteria' {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取判断类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_JUDGE_TYPE(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (ENUM_CODE like '%{keyword}%' or ENUM_VALUE like '%{keyword}%')";
                }

                string sql = $"select ENUM_CODE as code,ENUM_VALUE as name from SYS001M where enum_type='enum_testitem_type' {where}";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取材料信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCHOICEInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (item_no like '%{keyword}%' or NAME_T like '%{keyword}%')";
                }

                string sql = $" select item_no as code,NAME_T as name from bdm_rd_item where 1=1 {where} and ROWNUM<25 ";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist.ToDataList<code_name_obj>());
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取所有下拉数据源
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetALLDDLData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                #endregion
                #region 逻辑
                Dictionary<string, List<code_name_obj>> result = new Dictionary<string, List<code_name_obj>>();
                var list_jd_data = DB.GetDataTable($@"select PHASE_CREATION_NO as  code ,PHASE_CREATION_NAME as name from BDM_PHASE_CREATION_M where PHASE_CREATION_NO is not null and PHASE_CREATION_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_jd_data", list_jd_data);

                var list_tygs_data = DB.GetDataTable("select ENUM_CODE as code,ENUM_VALUE as name from SYS001M where enum_type='enum_general_formula'").ToDataList<code_name_obj>();
                result.Add("list_tygs_data", list_tygs_data);

                var list_zdygs_data = DB.GetDataTable("select FORMULA_CODE as code,FORMULA_NAME as name from bdm_formula_m where FORMULA_CODE is not null and FORMULA_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_zdygs_data", list_zdygs_data);

                var list_category_data = DB.GetDataTable("select category_code as code,category_name as name from bdm_category_m where category_code is not null and category_name is not null").ToDataList<code_name_obj>();
                result.Add("list_category_data", list_category_data);

                var list_xjjb_data = DB.GetDataTable("select pb_type_code as code,pb_type_level as name from bdm_pb_type_m where pb_type_code is not null and pb_type_level is not null").ToDataList<code_name_obj>();
                result.Add("list_xjjb_data", list_xjjb_data);

                var list_agesex_data = DB.GetDataTable("select age_gender_code as code,age_gender_name as name from bdm_age_gender_m where age_gender_code is not null and age_gender_name is not null").ToDataList<code_name_obj>();
                result.Add("list_agesex_data", list_agesex_data);

                var list_cptype_data = DB.GetDataTable("select finished_product_code as code,finished_product_name as name from bdm_finished_product_m where finished_product_code is not null and finished_product_name is not null").ToDataList<code_name_obj>();
                result.Add("list_cptype_data", list_cptype_data);

                //var list_fgt_data = DB.GetDataTable("select finished_product_code as code,finished_product_name as name from bdm_finished_product_m").ToDataList<code_name_obj>();
                //result.Add("list_fgt_data", list_fgt_data);

                var list_parts_data = DB.GetDataTable("select PARTS_CODE as code,PARTS_NAME as name from BDM_PARTS_M where PARTS_CODE is not null and PARTS_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_parts_data", list_parts_data);

                var list_position_data = DB.GetDataTable("select POSITION_CODE as code,POSITION_NAME as name from bdm_position_m where POSITION_CODE is not null and POSITION_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_position_data", list_position_data);

                var list_line_data = DB.GetDataTable($@"
SELECT
	PRODUCTION_LINE_CODE AS code,
	PRODUCTION_LINE_NAME AS NAME
FROM
	bdm_production_line_m
WHERE
	PRODUCTION_LINE_CODE IS NOT NULL
AND PRODUCTION_LINE_NAME IS NOT NULL
UNION 
SELECT
	DEPARTMENT_CODE AS code,
	DEPARTMENT_NAME AS NAME
FROM
	base005m
WHERE
	DEPARTMENT_CODE IS NOT NULL
AND DEPARTMENT_NAME IS NOT NULL 
UNION 
SELECT
	SUPPLIERS_CODE AS code,
	SUPPLIERS_NAME AS NAME
FROM
	BASE003M 
WHERE
	SUPPLIERS_CODE IS NOT NULL
AND SUPPLIERS_NAME IS NOT NULL").ToDataList<code_name_obj>();
                result.Add("list_line_data", list_line_data);

                var list_materialtype_data = DB.GetDataTable("select MATERIAL_TYPE_CODE as code,MATERIAL_TYPE_NAME as name from bdm_material_type_m where MATERIAL_TYPE_CODE is not null and MATERIAL_TYPE_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_materialtype_data", list_materialtype_data);

                var list_workmanship_data = DB.GetDataTable("select WORKMANSHIP_CODE as code,WORKMANSHIP_NAME as name from bdm_workmanship_m where WORKMANSHIP_CODE is not null and WORKMANSHIP_NAME is not null").ToDataList<code_name_obj>();
                result.Add("list_workmanship_data", list_workmanship_data);

                var list_productlevel_data = DB.GetDataTable("select PRODUCT_LEVEL_CODE as code,PRODUCT_LEVEL_VALUE as name from bdm_product_level_m where PRODUCT_LEVEL_CODE is not null and PRODUCT_LEVEL_VALUE is not null").ToDataList<code_name_obj>();
                result.Add("list_productlevel_data", list_productlevel_data);

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


        ///获取厂商数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                string where1 = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" AND (SUPPLIERS_CODE like '%{keyword}%' or SUPPLIERS_NAME like '%{keyword}%' or UDF10 like '%{keyword}%')";
                    where1 += $" AND (DEPARTMENT_CODE like '%{keyword}%' or DEPARTMENT_NAME like '%{keyword}%')";
                }

                string sql = $@"
                 SELECT
                    '0' as data_type,
                	SUPPLIERS_CODE,
                	SUPPLIERS_NAME,
                	JC 
                FROM
                	base003m 
                WHERE
                	SUPPLIERS_CODE IS NOT NULL 
                	AND SUPPLIERS_NAME IS NOT NULL {where}
                UNION
                SELECT
                    '1' as data_type,
                	DEPARTMENT_CODE as SUPPLIERS_CODE,
                	DEPARTMENT_NAME as SUPPLIERS_NAME,
                	DEPARTMENT_CODE as JC
                FROM
                	BASE005M 
                where 1=1 {where1}
                ";

                //                string sql = $@"

                //SELECT
                //    '0' as data_type,
                //	SUPPLIERS_CODE,
                //	SUPPLIERS_NAME,
                //	SUPPLIERS_CODE as JC  
                //FROM
                //	base003m 
                //WHERE
                //	SUPPLIERS_CODE IS NOT NULL 
                //	AND SUPPLIERS_NAME IS NOT NULL {where}
                //UNION
                //SELECT
                //    '1' as data_type,
                //	DEPARTMENT_CODE as SUPPLIERS_CODE,
                //	DEPARTMENT_NAME as SUPPLIERS_NAME,
                //	DEPARTMENT_CODE as JC
                //FROM
                //	BASE005M 
                //where 1=1 {where1}
                // ";

                DataTable fgtlist = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist);
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateCSData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";//厂商代号
                string JC = jarr.ContainsKey("JC") ? jarr["JC"].ToString() : "";//简称
                #endregion
                #region 逻辑
                string sql = $@"";
                string where = "";

                string DEPARTMENT_CODE = DB.GetString($@"
SELECT 'base003m'as TABLE,SUPPLIERS_CODE as DEPARTMENT_CODE,SUPPLIERS_NAME as DEPARTMENT_NAME,JC as DEPARTMENT_CODE FROM base003m WHERE SUPPLIERS_CODE = '{SUPPLIERS_CODE}'
");
                if (!string.IsNullOrEmpty(SUPPLIERS_CODE))
                {
                    DEPARTMENT_CODE = $@"SELECT DEPARTMENT_CODE,DEPARTMENT_NAME,DEPARTMENT_CODE FROM BASE005M WHERE DEPARTMENT_CODE = '{SUPPLIERS_CODE}'";
                    sql = $@"update BASE005M set  ";
                }
                else
                {

                }

                


                ret.IsSuccess = true;
                //ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(fgtlist);
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///保存厂商检查
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveCSJc(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";//二维码
                string JC = jarr.ContainsKey("JC") ? jarr["JC"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                DB.ExecuteNonQueryOffline($@"update base003m set JC='{JC}' where SUPPLIERS_CODE='{SUPPLIERS_CODE}'");


                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取厂商数据bycode
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetCSDataByCode(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string sql = $" select SUPPLIERS_CODE,SUPPLIERS_NAME,JC from base003m where SUPPLIERS_CODE='{code}' ";

                var dic = DB.GetDictionary(sql);
                if (dic != null && dic.Count > 0)
                {
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No data found";
                }
                #endregion
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
        /// 判断是否已提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject IsSubmit(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//二维码
                string test_type = jarr.ContainsKey("test_type") ? jarr["test_type"].ToString() : "";//二维码
                #endregion
                #region 逻辑

                int value = DB.GetInt32($"select count(1) from QCM_EX_TASK_LIST_M where ART_NO='{art_no}'and TEST_TYPE='{test_type}'");
                ret.IsSuccess = true;
                ret.RetData = value.ToString();
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
        /// 提交送检登记
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveExTask(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 打开事务
                DB.Open();
                DB.BeginTransaction();
                #endregion
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string parts_code = jarr.ContainsKey("parts_code") ? jarr["parts_code"].ToString() : "";//部件编号
                string parts_name = jarr.ContainsKey("parts_name") ? jarr["parts_name"].ToString() : "";//部件名称

                string position_code = jarr.ContainsKey("position_code") ? jarr["position_code"].ToString() : "";//部位编号
                string position_name = jarr.ContainsKey("position_name") ? jarr["position_name"].ToString() : "";//部位名称

                string line_code = jarr.ContainsKey("line_code") ? jarr["line_code"].ToString() : "";//产线编号
                string line_name = jarr.ContainsKey("line_name") ? jarr["line_name"].ToString() : "";//产线名称

                string material_code = jarr.ContainsKey("material_code") ? jarr["material_code"].ToString() : "";//物料编号
                string material_name = jarr.ContainsKey("material_name") ? jarr["material_name"].ToString() : "";//物料名称
                
                string workmanship_code = jarr.ContainsKey("workmanship_code") ? jarr["workmanship_code"].ToString() : "";//工艺编号
                string workmanship_name = jarr.ContainsKey("workmanship_name") ? jarr["workmanship_name"].ToString() : "";//工艺名称

                string makings_type_code = jarr.ContainsKey("makings_type_code") ? jarr["makings_type_code"].ToString() : "";//材料编号
                string makings_type_name = jarr.ContainsKey("makings_type_name") ? jarr["makings_type_name"].ToString() : "";//材料名称

                string category_code = jarr.ContainsKey("category_code") ? jarr["category_code"].ToString() : "";//category(开发系列)
                string category_name = jarr.ContainsKey("category_name") ? jarr["category_name"].ToString() : "";//category(开发系列)

                string product_level_code = jarr.ContainsKey("product_level_code") ? jarr["product_level_code"].ToString() : "";//产品级别
                string product_level_value = jarr.ContainsKey("product_level_value") ? jarr["product_level_value"].ToString() : "";//产品级别

                string pb_type_code = jarr.ContainsKey("pb_type_code") ? jarr["pb_type_code"].ToString() : "";//新旧级别
                string pb_type_level = jarr.ContainsKey("pb_type_level") ? jarr["pb_type_level"].ToString() : "";//新旧级别

                string cp_type_code = jarr.ContainsKey("cp_type_code") ? jarr["cp_type_code"].ToString() : "";//成品种类
                string cp_type_name = jarr.ContainsKey("cp_type_name") ? jarr["cp_type_name"].ToString() : "";//成品种类

                string gender = jarr.ContainsKey("gender") ? jarr["gender"].ToString() : "";//性别
                string gender_name = jarr.ContainsKey("gender_name") ? jarr["gender_name"].ToString() : "";//性别

                string phase_creation_no = jarr.ContainsKey("phase_creation_no") ? jarr["phase_creation_no"].ToString() : "";//阶段编号
                string phase_creation_name = jarr.ContainsKey("phase_creation_name") ? jarr["phase_creation_name"].ToString() : "";//阶段名称

                string fgt_no = jarr.ContainsKey("fgt_no") ? jarr["fgt_no"].ToString() : "";//fgt编号
                string fgt_name = jarr.ContainsKey("fgt_name") ? jarr["fgt_name"].ToString() : "";//fgt名称

                string colors = jarr.ContainsKey("colors") ? jarr["colors"].ToString() : "";//颜色
                string glue = jarr.ContainsKey("glue") ? jarr["glue"].ToString() : "";//胶水信息

                string manufacturer_code = jarr.ContainsKey("manufacturer_code") ? jarr["manufacturer_code"].ToString() : "";//厂商code
                string manufacturer_name = jarr.ContainsKey("manufacturer_name") ? jarr["manufacturer_name"].ToString() : "";//厂商名称
                string manufacturer_jc = jarr.ContainsKey("manufacturer_jc") ? jarr["manufacturer_jc"].ToString() : "";//厂商缩写

                
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art
                string shoe_no = jarr.ContainsKey("shoe_no") ? jarr["shoe_no"].ToString() : "";//鞋型
                string material_way = jarr.ContainsKey("material_way") ? jarr["material_way"].ToString() : "";//material_way_id
                string season = jarr.ContainsKey("season") ? jarr["season"].ToString() : "";//季度
                string send_test_qty = jarr.ContainsKey("send_test_qty") ? jarr["send_test_qty"].ToString() : "";//送测数量
                string size = jarr.ContainsKey("size") ? jarr["size"].ToString() : "";//size
                string order_po = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";//po订单
                string order_po_qty = jarr.ContainsKey("order_po_qty") ? jarr["order_po_qty"].ToString() : "";//po数量
                string test_reason = jarr.ContainsKey("test_reason") ? jarr["test_reason"].ToString() : "";//送测原因
                string task_state = jarr.ContainsKey("task_state") ? jarr["task_state"].ToString() : "";//任务状态
                string test_type = jarr.ContainsKey("test_type") ? jarr["test_type"].ToString() : "";//测试类型

                string test_id = jarr.ContainsKey("test_id") ? jarr["test_id"].ToString() : "";//test_id
                string cmbbh = jarr.ContainsKey("cmbbh") ? jarr["cmbbh"].ToString() : "";//尺码标编号
                string model_no = jarr.ContainsKey("model_no") ? jarr["model_no"].ToString() : "";//model_no

                string staff_no = jarr.ContainsKey("staff_no") ? jarr["staff_no"].ToString() : "";//员工代号
                string staff_name = jarr.ContainsKey("staff_name") ? jarr["staff_name"].ToString() : "";//员工名称
                string staff_department = jarr.ContainsKey("staff_department") ? jarr["staff_department"].ToString() : "";//员工部门
                string staff_department_code = jarr.ContainsKey("staff_department_code") ? jarr["staff_department_code"].ToString() : "";//员工部门

                string itemlist = jarr.ContainsKey("itemlist") ? jarr["itemlist"].ToString() : "";//明细数据

                string cc_task_no = jarr.ContainsKey("cc_task_no") ? jarr["cc_task_no"].ToString() : "";

                string sldh = jarr.ContainsKey("sldh") ? jarr["sldh"].ToString() : "";//收料单号
                string makings_id = jarr.ContainsKey("makings_id") ? jarr["makings_id"].ToString() : "";//材料id
                string ccsj_date = jarr.ContainsKey("ccsj_date") ? jarr["ccsj_date"].ToString() : "";//鞋子抽测时间
                string test_part = jarr.ContainsKey("test_part") ? jarr["test_part"].ToString() : "";//试样部位名称

                string cl_qrcode_json = jarr.ContainsKey("cl_qrcode_json") ? jarr["cl_qrcode_json"].ToString() : "";//材料收料单二维码json信息



                #endregion
                #region 逻辑
                string task_no = "";
                string max_task_no = DB.GetString($@"
SELECT
	TASK_NO 
FROM
	( SELECT TASK_NO FROM QCM_EX_TASK_LIST_M WHERE TEST_TYPE = '{test_type}' AND CREATEDATE = '{DateTime.Now:yyyy-MM-dd}' ORDER BY TO_DATE( CREATEDATE || ' ' || CREATETIME, 'yyyy-mm-dd HH24:mi:ss' ) DESC ) 
WHERE
	ROWNUM = 1
");

                string lsh = "0001";
                string qzh = "";
                if (!string.IsNullOrEmpty(max_task_no))
                {
                    lsh = (Convert.ToInt32("1" + max_task_no.Substring(max_task_no.Length - 4)) + 1).ToString().Substring(1);
                }
                switch (test_type)
                {
                    case "0":
                        qzh = "P" + staff_department_code;//D//I replaced inplace of 'D'
                        break;
                    case "1":
                        qzh = "C" + staff_department_code;
                        break;
                    case "2":
                        qzh = "T" + manufacturer_jc;
                        break;
                    case "3":
                        qzh = "M" + manufacturer_jc;
                        break;
                    case "4":
                        qzh = "B" + staff_department_code;
                        break;
                }
                //物料参数插入
                if (!string.IsNullOrEmpty(makings_id))
                {
                    string Msql = $"SELECT * from BDM_RD_ITEM  WHERE ITEM_NO='{makings_id}'";
                    DataTable mdt = DB.GetDataTable(Msql);
                    material_name = mdt.Rows[0]["name_t"].ToString();
                }
               
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                task_no = qzh + DateTime.Now.ToString("yyyyMMdd") + lsh;
                if (string.IsNullOrEmpty(cc_task_no))
                {

                    //string m_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetTableSequence(DB, "QCM_EX_TASK_LIST_M");
                    var ppp = new Dictionary<string, object>();
                    ppp.Add("CL_QRCODE_JSON", cl_qrcode_json);
                    DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_M(
                                                                        TASK_NO,
                                                                        ART_NO,
                                                                        SHOE_NO,
                                                                        MATERIAL_WAY,
                                                                        CATEGORY_CODE,
                                                                        CATEGORY_NAME,
                                                                        PRODUCT_LEVEL_CODE,
                                                                        PRODUCT_LEVEL_VALUE,
                                                                        SEASON,
                                                                        PB_TYPE_CODE,
                                                                        PB_TYPE_LEVEL,
                                                                        GENDER,
                                                                        GENDER_NAME,
                                                                        PHASE_CREATION_NO,
                                                                        PHASE_CREATION_NAME,
                                                                        SEND_TEST_QTY,
                                                                        SIZES,
                                                                        ORDER_PO,
                                                                        ORDER_PO_QTY,
                                                                        FGT_NO,
                                                                        FGT_NAME,
                                                                        TEST_REASON,
                                                                        STAFF_NO,
                                                                        STAFF_NAME,
                                                                        STAFF_DEPARTMENT,
                                                                        STAFF_DEPARTMENT_CODE,
                                                                        TASK_STATE,
                                                                        TEST_TYPE,
                                                                        CREATEBY,
                                                                        CREATEDATE,
                                                                        CREATETIME,
                                                                        PARTS_CODE,
                                                                        PARTS_NAME,
                                                                        POSITION_CODE,
                                                                        POSITION_NAME,
                                                                        WORKMANSHIP_CODE,
                                                                        WORKMANSHIP_NAME,
                                                                        LINE_CODE,
                                                                        LINE_NAME,
                                                                        MATERIAL_CODE,
                                                                        MATERIAL_NAME,
                                                                        MANUFACTURER_CODE,
                                                                        MANUFACTURER_NAME,
                                                                        MANUFACTURER_JC,
                                                                        COLORS,
                                                                        GLUE,
                                                                        MAKINGS_TYPE_CODE,
                                                                        MAKINGS_TYPE_NAME,
                                                                        MAKINGS_ID,
                                                                        CP_TYPE_CODE,
                                                                        CP_TYPE_NAME,
                                                                        MODEL_NO,
                                                                        TEST_ID,
                                                                        CMBBH,
                                                                        SLDH,
                                                                        TEST_TIME,
                                                                        SY_PART_NAME,
                                                                        CL_QRCODE_JSON
                                                                        )
                                                                values(
                                                                        '{task_no}',
                                                                        '{art_no}',
                                                                        '{shoe_no}',
                                                                        '{material_way}',
                                                                        '{category_code}',
                                                                        '{category_name}',
                                                                        '{product_level_code}',
                                                                        '{product_level_value}',
                                                                        '{season}',
                                                                        '{pb_type_code}',
                                                                        '{pb_type_level}',
                                                                        '{gender}',
                                                                        '{gender_name}',
                                                                        '{phase_creation_no}',
                                                                        '{phase_creation_name}',
                                                                        '{send_test_qty}',
                                                                        '{size}',
                                                                        '{order_po}',
                                                                        '{order_po_qty}',
                                                                        '{fgt_no}',
                                                                        '{fgt_name}',
                                                                        '{test_reason}',
                                                                        '{staff_no}',
                                                                        '{staff_name}',
                                                                        '{staff_department}',
                                                                        '{staff_department_code}',
                                                                        '{task_state}',
                                                                        '{test_type}',
                                                                        '{staff_no}',
                                                                        '{date}',
                                                                        '{time}',
                                                                        '{parts_code}',
                                                                        '{parts_name}',
                                                                        '{position_code}',
                                                                        '{position_name}',
                                                                        '{workmanship_code}',
                                                                        '{workmanship_name}',
                                                                        '{line_code}',
                                                                        '{line_name}',
                                                                        '{material_code}',
                                                                        '{material_name}',
                                                                        '{manufacturer_code}',
                                                                        '{manufacturer_name}',
                                                                        '{manufacturer_jc}',
                                                                        '{colors}',
                                                                        '{glue}',
                                                                        '{makings_type_code}',
                                                                        '{makings_type_name}',
                                                                        '{makings_id}',
                                                                        '{cp_type_code}',
                                                                        '{cp_type_name}',
                                                                        '{model_no}',
                                                                        '{test_id}',
                                                                        '{cmbbh}',
                                                                        '{sldh}',
                                                                        '{ccsj_date}',
                                                                        '{test_part}',
                                                                        @CL_QRCODE_JSON
                                                                        )", ppp);

                    // string m_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_LIST_M");

                    List<Dictionary<string, object>> deteillist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(itemlist);

                    foreach (var item in deteillist)
                    {

                        // INSERT INTO bdm_workshop_section_m(product_category) VALUES('123');
                        // SELECT bdm_workshop_section_m_Sequence.CURRVAL FROM dual;

                        DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_D(
                                                                            TASK_NO,
                                                                            SOURCES,
                                                                            INSPECTION_CODE,
                                                                            INSPECTION_NAME,
                                                                            INSPECTION_TYPE,
                                                                            JUDGMENT_CRITERIA,
                                                                            JUDGE_TYPE,
                                                                            STANDARD_VALUE,
                                                                            UNIT,SAMPLE_QTY,
                                                                            G_FORMULA_CODE,
                                                                            D_FORMULA_CODE,
                                                                            ART_D_REMARK,
                                                                            CHOICE_NO,
                                                                            CHOICE_NAME,
                                                                            CREATEBY,
                                                                            CREATEDATE,
                                                                            CREATETIME
                                                                            )
                                                                values(
                                                                            '{task_no}',
                                                                            '{item["source"]}',
                                                                            '{item["inspection_code"]}',
                                                                            '{item["inspection_name"]}',
                                                                            '{item["inspection_type"]}',
                                                                            '{item["judgment_criteria"]}',
                                                                            '{item["judge_type"]}',
                                                                            '{item["standard_value"]}',
                                                                            '{item["unit"]}',
                                                                            '{item["sample_qty"]}',
                                                                            '{item["g_formula_type"]}',
                                                                            '{item["d_formula_type"]}',
                                                                            '{item["art_d_remark"]}',
                                                                            '{item["choice_no"]}',
                                                                            '{item["choice_name"]}',
                                                                            '{staff_no}',
                                                                            '{date}',
                                                                            '{time}'
                                                                            )");
                        string d_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_LIST_D");
                        for (int i = 1; i <= int.Parse(item["sample_qty"].ToString()); i++)
                        {
                            DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_QRCODE(
                                                                                    TASK_NO,
                                                                                    D_ID,
                                                                                    INSPECTION_CODE,
                                                                                    INSPECTION_NAME,
                                                                                    INSPECTION_TYPE,
                                                                                    ITEM_SEQ,
                                                                                    CREATEBY,
                                                                                    CREATEDATE,
                                                                                    CREATETIME
                                                                                    )
                                                                            values(
                                                                                '{task_no}',
                                                                                '{d_id}',
                                                                                '{item["inspection_code"]}',
                                                                                '{item["inspection_name"]}',
                                                                                '{item["inspection_type"]}',
                                                                                '{i}',
                                                                                '{staff_no}',
                                                                                '{date}',
                                                                                '{time}'
                                                                                )");
                        }
                    }
                }
                else
                {
                    var info = DB.GetDictionary($"select * from QCM_EX_TASK_LIST_M where task_no='{cc_task_no}'");
                    var ppp = new Dictionary<string, object>();
                    ppp.Add("CL_QRCODE_JSON", info["CL_QRCODE_JSON"]);
                    DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_M(
                                                                        TASK_NO,
                                                                        ART_NO,
                                                                        SHOE_NO,
                                                                        MATERIAL_WAY,
                                                                        CATEGORY_CODE,
                                                                        CATEGORY_NAME,
                                                                        PRODUCT_LEVEL_CODE,
                                                                        PRODUCT_LEVEL_VALUE,
                                                                        SEASON,
                                                                        PB_TYPE_CODE,
                                                                        PB_TYPE_LEVEL,
                                                                        GENDER,
                                                                        GENDER_NAME,
                                                                        PHASE_CREATION_NO,
                                                                        PHASE_CREATION_NAME,
                                                                        SEND_TEST_QTY,
                                                                        SIZES,
                                                                        ORDER_PO,
                                                                        ORDER_PO_QTY,
                                                                        FGT_NO,
                                                                        FGT_NAME,
                                                                        TEST_REASON,
                                                                        STAFF_NO,
                                                                        STAFF_NAME,
                                                                        STAFF_DEPARTMENT,
                                                                        STAFF_DEPARTMENT_CODE,
                                                                        TASK_STATE,
                                                                        TEST_TYPE,
                                                                        CREATEBY,
                                                                        CREATEDATE,
                                                                        CREATETIME,
                                                                        PARTS_CODE,
                                                                        PARTS_NAME,
                                                                        POSITION_CODE,
                                                                        POSITION_NAME,
                                                                        WORKMANSHIP_CODE,
                                                                        WORKMANSHIP_NAME,
                                                                        LINE_CODE,
                                                                        LINE_NAME,
                                                                        MATERIAL_CODE,
                                                                        MATERIAL_NAME,
                                                                        MANUFACTURER_CODE,
                                                                        MANUFACTURER_NAME,
                                                                        MANUFACTURER_JC,
                                                                        COLORS,
                                                                        GLUE,
                                                                        MAKINGS_TYPE_CODE,
                                                                        MAKINGS_TYPE_NAME,
                                                                        MAKINGS_ID,
                                                                        CP_TYPE_CODE,
                                                                        CP_TYPE_NAME,
                                                                        MODEL_NO,
                                                                        TEST_ID,
                                                                        CMBBH,
                                                                        RETEST_TASK_NO, 
                                                                        TEST_TIME, 
                                                                        SY_PART_NAME,
                                                                        CL_QRCODE_JSON
                                                                        )
                                                                values(
                                                                        '{task_no}',
                                                                        '{info["ART_NO"]}',
                                                                        '{info["SHOE_NO"]}',
                                                                        '{info["MATERIAL_WAY"]}',
                                                                        '{info["CATEGORY_CODE"]}',
                                                                        '{info["CATEGORY_NAME"]}',
                                                                        '{info["PRODUCT_LEVEL_CODE"]}',
                                                                        '{info["PRODUCT_LEVEL_VALUE"]}',
                                                                        '{info["SEASON"]}',
                                                                        '{info["PB_TYPE_CODE"]}',
                                                                        '{info["PB_TYPE_LEVEL"]}',
                                                                        '{info["GENDER"]}',
                                                                        '{info["GENDER_NAME"]}',
                                                                        '{info["PHASE_CREATION_NO"]}',
                                                                        '{info["PHASE_CREATION_NAME"]}',
                                                                        '{info["SEND_TEST_QTY"]}',
                                                                        '{info["SIZES"]}',
                                                                        '{info["ORDER_PO"]}',
                                                                        '{info["ORDER_PO_QTY"]}',
                                                                        '{info["FGT_NO"]}',
                                                                        '{info["FGT_NAME"]}',
                                                                        '{info["TEST_REASON"]}',
                                                                        '{staff_no}',
                                                                        '{staff_name}',
                                                                        '{staff_department}',
                                                                        '{staff_department_code}',
                                                                        '0',
                                                                        '{info["TEST_TYPE"]}',
                                                                        '{staff_no}',
                                                                        '{date}',
                                                                        '{time}',
                                                                        '{info["PARTS_CODE"]}',
                                                                        '{info["PARTS_NAME"]}',
                                                                        '{info["POSITION_CODE"]}',
                                                                        '{info["POSITION_NAME"]}',
                                                                        '{info["WORKMANSHIP_CODE"]}',
                                                                        '{info["WORKMANSHIP_NAME"]}',
                                                                        '{info["LINE_CODE"]}',
                                                                        '{info["LINE_NAME"]}',
                                                                        '{info["MATERIAL_CODE"]}',
                                                                        '{info["MATERIAL_NAME"]}',
                                                                        '{info["MANUFACTURER_CODE"]}',
                                                                        '{info["MANUFACTURER_NAME"]}',
                                                                        '{info["MANUFACTURER_JC"]}',
                                                                        '{info["COLORS"]}',
                                                                        '{info["GLUE"]}',
                                                                        '{info["MAKINGS_TYPE_CODE"]}',
                                                                        '{info["MAKINGS_TYPE_NAME"]}',
                                                                        '{info["MAKINGS_ID"]}',
                                                                        '{info["CP_TYPE_CODE"]}',
                                                                        '{info["CP_TYPE_NAME"]}',
                                                                        '{info["MODEL_NO"]}',
                                                                        '{info["TEST_ID"]}',
                                                                        '{info["CMBBH"]}',
                                                                        '{cc_task_no}',
                                                                        '{info["TEST_TIME"]}',
                                                                        '{info["SY_PART_NAME"]}',
                                                                        @CL_QRCODE_JSON
                                                                        )", ppp);

                    var list = DB.GetDataTable($"select * from QCM_EX_TASK_LIST_D where task_no='{cc_task_no}'");
                    foreach (DataRow item in list.Rows)
                    {
                        DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_D(
                                                                            TASK_NO,
                                                                            SOURCES,
                                                                            INSPECTION_CODE,
                                                                            INSPECTION_NAME,
                                                                            INSPECTION_TYPE,
                                                                            JUDGMENT_CRITERIA,
                                                                            JUDGE_TYPE,
                                                                            STANDARD_VALUE,
                                                                            UNIT,SAMPLE_QTY,
                                                                            G_FORMULA_CODE,
                                                                            D_FORMULA_CODE,
                                                                            ART_D_REMARK,
                                                                            CHOICE_NO,
                                                                            CHOICE_NAME,
                                                                            CREATEBY,
                                                                            CREATEDATE,
                                                                            CREATETIME
                                                                            )
                                                                values(
                                                                            '{task_no}',
                                                                            '{item["SOURCES"]}',
                                                                            '{item["INSPECTION_CODE"]}',
                                                                            '{item["INSPECTION_NAME"]}',
                                                                            '{item["INSPECTION_TYPE"]}',
                                                                            '{item["JUDGMENT_CRITERIA"]}',
                                                                            '{item["JUDGE_TYPE"]}',
                                                                            '{item["STANDARD_VALUE"]}',
                                                                            '{item["UNIT"]}',
                                                                            '{item["SAMPLE_QTY"]}',
                                                                            '{item["G_FORMULA_CODE"]}',
                                                                            '{item["D_FORMULA_CODE"]}',
                                                                            '{item["ART_D_REMARK"]}',
                                                                            '{item["CHOICE_NO"]}',
                                                                            '{item["CHOICE_NAME"]}',
                                                                            '{staff_no}',
                                                                            '{date}',
                                                                            '{time}'
                                                                            )");
                        string d_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_LIST_D");
                        for (int i = 1; i <= int.Parse(item["SAMPLE_QTY"].ToString()); i++)
                        {
                            DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_QRCODE(
                                                                                    TASK_NO,
                                                                                    D_ID,
                                                                                    INSPECTION_CODE,
                                                                                    INSPECTION_NAME,
                                                                                    INSPECTION_TYPE,
                                                                                    ITEM_SEQ,
                                                                                    CREATEBY,
                                                                                    CREATEDATE,
                                                                                    CREATETIME
                                                                                    )
                                                                            values(
                                                                                '{task_no}',
                                                                                '{d_id}',
                                                                                '{item["INSPECTION_CODE"]}',
                                                                                '{item["INSPECTION_NAME"]}',
                                                                                '{item["INSPECTION_TYPE"]}',
                                                                                '{i}',
                                                                                '{staff_no}',
                                                                                '{date}',
                                                                                '{time}'
                                                                                )");
                        }
                    }
                }



                #endregion
                #region 提交事务
                DB.Commit();
                ret.IsSuccess = true;
                ret.RetData = task_no;
                #endregion
            }

            catch (Exception ex)
            {
                #region 回滚事务
                DB.Rollback();
                #endregion
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                #region 关闭事务
                DB.Close();
                #endregion
            }
            return ret;
        }

        /// <summary>
        /// 扫描员工工号获取员工信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetStaffInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string qrcode = jarr.ContainsKey("qrcode") ? jarr["qrcode"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                var dic = DB.GetDictionary($@"select a.STAFF_NO,a.STAFF_NAME,b.DEPARTMENT_CODE,b.DEPARTMENT_NAME from HR001M a
                                    left join BASE005M b on a.STAFF_DEPARTMENT=b.DEPARTMENT_CODE
                                    where a.STAFF_NO='{qrcode}' and ROWNUM=1");
                if (dic == null || dic.Count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Invalid Employee ID";
                    return ret;
                }
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic); ;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取art信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArtInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string qrcode = jarr.ContainsKey("qrcode") ? jarr["qrcode"].ToString() : "";//二维码
                string type = jarr.ContainsKey("type") ? jarr["type"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                var dic = DB.GetDictionary($@"
SELECT
	A .PROD_NO,
	A .SHOE_NO,
	A .MATERIAL_WAY,
	A .DEVELOP_SEASON,
	A .SEG_NO,
	'' AS PRODUCTION_LEVEL,
	'' AS NEW_OLD_LEVEL,
	'' AS CATEGROY,
	a.PRODUCT_LEVEL,
	a.DEVELOP_TYPE,
	bb.name_t AS CATEGROY_ID,
	MOLD_NO AS MODEL_NO,
	D .name_t AS SHOE_NAME
FROM
	BDM_RD_PROD A
LEFT JOIN BDM_RD_STYLE D ON A .shoe_no = D .shoe_no
LEFT JOIN bdm_cd_code bb ON D.style_seq=bb.code_no
WHERE  a.PROD_NO='{qrcode}'");
                if (dic == null || dic.Count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = " Invalid ART barcode";//无效ART条码
                    return ret;
                }

                var po_info = DB.GetDataTable($@"
SELECT 
	I.PROD_NO,
	M.MER_PO,
	M.ORG_ID,
	M.SE_ID,
	I.SE_QTY
FROM BDM_SE_ORDER_MASTER M 
INNER JOIN BDM_SE_ORDER_ITEM I ON I.SE_ID = M.SE_ID AND I.ORG_ID=M.ORG_ID 
WHERE I.PROD_NO='{qrcode}'");

                List<string> INSPECTION_TYPE = new List<string>();
                string LCLLWhere = "";
                if (type == "cpx")
                {
                    INSPECTION_TYPE.Add("8");
                }
                if (type == "bj")
                {
                    INSPECTION_TYPE.Add("11");
                }
                if (type == "gy")
                {
                    INSPECTION_TYPE.Add("10");
                }
                if (type == "cl")
                {
                    INSPECTION_TYPE.Add("9");
                }
                //string cg_sql = "";
                string dqa_sql = "";


                if (type == "lcll")
                {
                    dqa_sql = $@"select 
                                   'conventional' AS type,
                                   '8' as inspection_type,
                                   'Finished Shoes-Testing' as inspection_type_name,
                                   '' as choice_no,
                                   '' as choice_name,
                                   a.inspection_code,
                                   a.inspection_name,
                                   to_char(a.judgment_criteria) as judgment_criteria,
                                   b.enum_value as judgment_criteria_name,
                                   to_char(a.judge_type) as judge_type,
                                   b1.enum_value as judge_type_name,
                                   to_char(a.standard_value) as standard_value,
                                   '' as unit,
                                   a.remarks
                                    from bdm_shoes_check_test_m a
                                    left join sys001m b on a.judgment_criteria=b.enum_code and b.enum_type='enum_judgment_criteria'
                                    left join sys001m b1 on a.judge_type=b1.enum_code and b1.enum_type='enum_testitem_type'
                                    where a.islcll_item=1";
                }
                else
                {
                    dqa_sql = $@"select 
                                   'DQA测试任务' as type,
                                    to_char(f.inspection_type) as inspection_type ,
                                    g.enum_value as inspection_type_name,
                                    c.choice_no,
                                    c.choice_name,
                                    c.inspection_code,
                                    c.inspection_name,
                                    to_char(c.judgment_criteria) as judgment_criteria,
                                    g1.enum_value as judgment_criteria_name,
                                    c.judge_type,
                                    g2.enum_value as judge_type_name,
                                    to_char(c.standard_value) as standard_value,
                                    c.unit,
                                    c.remark as remarks
                                    from bdm_rd_prod a
                                    join qcm_dqa_mag_m b on a.shoe_no=b.shoes_code
                                    join qcm_dqa_mag_d c on c.m_id=b.id
                                    join qcm_dqa_mag_d_art d on d.d_id=c.id and a.prod_no=d.art_code
                                    join bdm_workshop_section_m  e on b.workshop_section_no=e.workshop_section_no
                                    join bdm_workshop_section_d f on f.m_id=e.id and f.inspection_type in('{string.Join("','", INSPECTION_TYPE)}')
                                    left join sys001m g on f.inspection_type=g.enum_code and g.enum_type = 'enum_inspection_type' 
                                    left join sys001m g1 on c.judgment_criteria=g1.enum_code and g1.enum_type = 'enum_judgment_criteria' 
left join sys001m g2 on c.judge_type=g2.enum_code and g2.enum_type = 'enum_judge_type' 
                                    where  a.prod_no='{qrcode}'";

                    DataTable tableName_dt = DB.GetDataTable($"select enum_code,enum_value2,enum_value from sys001m where enum_code in('{string.Join("','", INSPECTION_TYPE)}') and enum_type='enum_inspection_type'");
                    int i = 0;
                    //foreach (DataRow item in tableName_dt.Rows)
                    //{
                    //    cg_sql += $@" union all
                    //                        select '常规' AS type,
                    //                        '{item["enum_code"]}' as inspection_type,
                    //                        '{item["enum_value"]}' as inspection_type_name,
                    //                        '' as choice_no,
                    //                        '' as choice_name,
                    //                        a.inspection_code,
                    //                        a.inspection_name,
                    //                        to_char(a.judgment_criteria) as judgment_criteria,
                    //                        b.enum_value as judgment_criteria_name,
                    //                        to_char(a.judge_type) as judge_type,
                    //                        b1.enum_value as judge_type_name,
                    //                        to_char(a.standard_value) as standard_value,
                    //                        '' as unit,
                    //                        a.remarks
                    //                 from {item["enum_value2"]} a 
                    //                 left join sys001m b on a.judgment_criteria=b.enum_code and b.enum_type='enum_judgment_criteria'
                    //                 left join sys001m b1 on a.judge_type=b1.enum_code and b1.enum_type='enum_testitem_type'";
                    //}
                }



                var check_item = DB.GetDataTable(dqa_sql);



                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("info", dic);
                result.Add("po_info", po_info);
                result.Add("check_item", check_item);

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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPoInfoByArt(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";//art
                #endregion
                #region 逻辑

                var po_info = DB.GetDataTable($@"
SELECT 
	I.PROD_NO,
	M.MER_PO,
	M.ORG_ID,
	M.SE_ID,
	I.SE_QTY
FROM BDM_SE_ORDER_MASTER M 
INNER JOIN BDM_SE_ORDER_ITEM I ON I.SE_ID = M.SE_ID AND I.ORG_ID=M.ORG_ID 
WHERE I.PROD_NO='{art_no}'");



                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("po_info", po_info);

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

        ///获取artlist
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArtList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";//二维码
                #endregion
                #region 逻辑
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" and PROD_NO like '%{keyword}%'";
                }
                var dt = DB.GetDataTable($@"select DISTINCT PROD_NO
                                                from BDM_RD_PROD 
                                                where 1=1 {where} and ROWNUM<25");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取送测清单（任务列表）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTaskList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";

                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string source_task_no = jarr.ContainsKey("source_task_no") ? jarr["source_task_no"].ToString() : "";
                string test_type = jarr.ContainsKey("test_type") ? jarr["test_type"].ToString() : "";
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";
                string model_no = jarr.ContainsKey("model_no") ? jarr["model_no"].ToString() : "";
                string test_id = jarr.ContainsKey("test_id") ? jarr["test_id"].ToString() : "";
                string sjr = jarr.ContainsKey("sjr") ? jarr["sjr"].ToString() : "";
                string sjbm = jarr.ContainsKey("sjbm") ? jarr["sjbm"].ToString() : "";
                string start_time = jarr.ContainsKey("start_time") ? jarr["start_time"].ToString() : "";
                string end_time = jarr.ContainsKey("end_time") ? jarr["end_time"].ToString() : "";
                string xxmc = jarr.ContainsKey("xxmc") ? jarr["xxmc"].ToString() : "";
                string jieduan = jarr.ContainsKey("jieduan") ? jarr["jieduan"].ToString() : "";
                string test_result = jarr.ContainsKey("test_result") ? jarr["test_result"].ToString() : "";

                string show_detail = jarr.ContainsKey("show_detail") ? jarr["show_detail"].ToString() : "";

                string where = "";

                if (!string.IsNullOrEmpty(task_no))
                {
                    where += $" and a.TASK_NO like '%{task_no}%'";
                }
                if (!string.IsNullOrEmpty(source_task_no))
                {
                    where += $" and a.RETEST_TASK_NO like '%{source_task_no}%'";
                }
                if (!string.IsNullOrEmpty(test_type))
                {
                    where += $" and TEST_TYPE='{test_type}'";
                }
                if (!string.IsNullOrEmpty(art))
                {
                    where += $" and ART_NO like '%{art}%'";
                }
                if (!string.IsNullOrEmpty(model_no))
                {
                    where += $" and MODEL_NO like '%{model_no}%'";
                }
                if (!string.IsNullOrEmpty(test_id))
                {
                    where += $" and TEST_ID like '%{test_id}%'";
                }
                if (!string.IsNullOrEmpty(sjr))
                {
                    where += $" and staff_name like '%{sjr}%'";
                }
                if (!string.IsNullOrEmpty(sjbm))
                {
                    where += $" and staff_department like '%{sjbm}%'";
                }
                if (!string.IsNullOrEmpty(xxmc))
                {
                    where += $" and (shoe_no like '%{xxmc}%')";
                }
                if (!string.IsNullOrEmpty(jieduan))
                {
                    where += $" and (phase_creation_no like '%{xxmc}%' or phase_creation_name like '%{xxmc}%')";
                }
                if (!string.IsNullOrEmpty(test_result))
                {
                    where += $" and a.TEST_RESULT ='{test_result}'";
                }
                if (!string.IsNullOrEmpty(start_time))
                {
                    where += $" and a.CREATEDATE >='{start_time}'";
                }
                if (!string.IsNullOrEmpty(end_time))
                {
                    where += $" and a.CREATEDATE <='{end_time}'";
                }

                #endregion

                #region 逻辑
                string sql = $@"SELECT
	                            a.*,
	                            b.TASK_NO as CC_TASK_NO,
	                            b.TEST_RESULT as CC_TEST_RESULT,
                                a.RETEST_TASK_NO as SOURCE_TASK_NO,
                                '' as Report_Upload_Status,
                                c.stock_code as location_code
                            FROM
	                            QCM_EX_TASK_LIST_M a
                                left join QCM_EX_TASK_ARC c on a.task_no=c.task_no
                            left join
                            (
	                            select RETEST_TASK_NO,TASK_NO,TEST_RESULT from QCM_EX_TASK_LIST_M a join
	                            (
		                            select max(id) as id from QCM_EX_TASK_LIST_M b where RETEST_TASK_NO is not null group by RETEST_TASK_NO
	                            ) b on a.id=b.id
                            ) b  on a.TASK_NO=b.RETEST_TASK_NO
                            where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), " order by CREATEDATE desc");

                foreach (DataRow dr in dt.Rows)
                {
                    string Task_No = dr["TASK_NO"].ToString();
                    string Art_No = dr["Art_No"].ToString();
                    string Test_Type = dr["Test_Type"].ToString();

                    var paramsDic = new Dictionary<string, object>();
                   // paramsDic.Add("ART_NO", Art_No);       //commented on 2025/03/28 by Ashok because of uploaded lab reports are not shown for tasks not having article number.
                    paramsDic.Add("TEST_TYPE", Test_Type);
                    paramsDic.Add("TASK_NO", Task_No);
                    string sql2 = $@"
SELECT
    QCM_EX_TASK_LIST_M_F.ID,
	QCM_EX_TASK_LIST_M_F.FILE_NAME,
	BDM_UPLOAD_FILE_ITEM.FILE_URL as file_url,
	BDM_UPLOAD_FILE_ITEM.GUID as guid,
    	QCM_EX_TASK_LIST_M_F.UPLOAD_TIME
FROM
	QCM_EX_TASK_LIST_M_F
LEFT JOIN BDM_UPLOAD_FILE_ITEM ON QCM_EX_TASK_LIST_M_F.FILE_GUID = BDM_UPLOAD_FILE_ITEM.GUID
WHERE 1=1
--AND QCM_EX_TASK_LIST_M_F.ART_NO = @ART_NO
AND QCM_EX_TASK_LIST_M_F.TEST_TYPE =@TEST_TYPE
AND QCM_EX_TASK_LIST_M_F.TASK_NO = @TASK_NO";
                    DataTable dt2 = DB.GetDataTable(sql2, paramsDic);

                    if(dt2.Rows.Count > 0)
                    {
                        dr["Report_Upload_Status"] = "Uploaded";
                    }
                    else
                    {
                        dr["Report_Upload_Status"] = "Not_Uploaded";
                    }

                }




                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                if (show_detail == "1")
                {
                    List<string> tasklist = new List<string>();
                    foreach (DataRow dr in dt.Rows)
                    {
                        tasklist.Add(dr["TASK_NO"].ToString());
                    }
                    DataTable dt_detail = DB.GetDataTable($"select task_no,inspection_code,item_test_val,item_test_result,remark from QCM_EX_TASK_LIST_D where task_no in('{string.Join("','", tasklist)}')");
                    dic.Add("Data_Detail", dt_detail);

                    if (test_type == "3")
                    {
                        DataTable dt_remarks = DB.GetDataTable($@"select m.task_no,d.inspection_code,a.remarks from QCM_EX_TASK_ITEM a
                                                                    join QCM_EX_TASK_LIST_D d on a.d_id = d.id
                                                                    join QCM_EX_TASK_LIST_M m on d.task_no = m.task_no
                                                                    where m.task_no in('{string.Join("','", tasklist)}')");
                        dic.Add("dt_remarks", dt_remarks);
                    }
                }
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取送测清单明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTaskInfo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string test_type = jarr.ContainsKey("test_type") ? jarr["test_type"].ToString() : "";
                #endregion

                #region 逻辑
                string sql = $@"SELECT * FROM QCM_EX_TASK_LIST_M WHERE TASK_NO='{task_no}'";
                Dictionary<string, object> dic = DB.GetDictionary(sql);

                if (dic == null || dic.Count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "invalid tracking number";//无效单号
                    return ret;
                }
                else if (!string.IsNullOrEmpty(test_type) && test_type != dic["TEST_TYPE"].ToString())
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The type of test delivery is inconsistent with the type of the tracking number";//送测类型与该单号类型不一致
                    return ret;
                }
                else if (!string.IsNullOrEmpty(test_type) && !string.IsNullOrEmpty(dic["RETEST_TASK_NO"].ToString()))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Retest order number, must be the original laboratory number";//重测单号，必须是原始的实验室编号
                    return ret;
                }

                sql = $@"select a.*,
                                b.enum_value as inspection_type_name,
                                b1.enum_value as JUDGMENT_CRITERIA_NAME,
                                b2.enum_value as JUDGE_TYPE_NAME
                                from QCM_EX_TASK_LIST_D a 
                                left join sys001m b on a.inspection_type=b.enum_code and b.enum_type='enum_inspection_type'
                                left join sys001m b1 on a.judgment_criteria=b1.enum_code and b1.enum_type='enum_judgment_criteria'
                                left join sys001m b2 on a.judge_type=b2.enum_code and b2.enum_type='enum_testitem_type'
                                WHERE TASK_NO='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);

                sql = $"select * from qcm_ex_task_qs WHERE TASK_NO='{task_no}'";
                DataTable qs_dt = DB.GetDataTable(sql);

                sql = $"select * from qcm_ex_task_qz WHERE TASK_NO='{task_no}'";
                DataTable qz_dt = DB.GetDataTable(sql);
                //获取物料名称              
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("info", dic);
                result.Add("itemlist", dt);
                result.Add("qslist", qs_dt);
                result.Add("qzlist", qz_dt);

                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(result);
                ret.IsSuccess = true;

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
        /// 获取测试项样品图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTaskItemImg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string d_id = jarr.ContainsKey("d_id") ? jarr["d_id"].ToString() : "";
                #endregion

                #region 逻辑
                if (string.IsNullOrEmpty(d_id))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "d_id not null";
                    return ret;
                }

               string sql = $@"
SELECT
	i.ITEM_SEQ,
	ii.IMG_GUID,
	f.FILE_URL
FROM
	QCM_EX_TASK_ITEM i 
INNER JOIN QCM_EX_TASK_ITEM_IMG ii ON ii.ITEM_ID=i.ID 
INNER JOIN BDM_UPLOAD_FILE_ITEM f ON f.GUID=ii.IMG_GUID
WHERE i.D_ID={d_id}
";
                DataTable dt = DB.GetDataTable(sql);

                //获取图片              
                Dictionary<string, DataTable> result = new Dictionary<string, DataTable>();

                foreach (DataRow item in dt.Rows)
                {
                    string ITEM_SEQ = item["ITEM_SEQ"].ToString();
                    if (result.ContainsKey(ITEM_SEQ))
                    {
                        result[ITEM_SEQ].Rows.Add(item.ItemArray);
                    }
                    else
                    {
                        result.Add(ITEM_SEQ, dt.Clone());
                        result[ITEM_SEQ].Rows.Add(item.ItemArray);
                    }
                }

                result = result.OrderBy(x => Convert.ToInt32(x.Key)).ToDictionary(p => p.Key, o => o.Value);

                ret.RetData = JsonConvert.SerializeObject(result);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///删除送测单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteTask(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                #endregion

                #region 逻辑
                string sql = $@"delete QCM_EX_TASK_LIST_M WHERE TASK_NO='{task_no}'";
                string sql1 = $@"delete QCM_EX_TASK_LIST_D WHERE TASK_NO='{task_no}'";
                string sql2 = $@"delete qcm_ex_task_qs WHERE TASK_NO='{task_no}'";
                string sql3 = $@"delete qcm_ex_task_qz WHERE TASK_NO='{task_no}'";
                string sql6 = $@"delete QCM_EX_TASK_LIST_QRCODE WHERE TASK_NO='{task_no}'";
                string sql4 = $@"delete QCM_EX_TASK_ITEM WHERE TASK_NO='{task_no}'";
                string sql5 = $@"delete QCM_EX_TASK_ITEM_IMG WHERE TASK_NO='{task_no}'";

                DB.ExecuteNonQuery(sql);
                DB.ExecuteNonQuery(sql1);
                DB.ExecuteNonQuery(sql2);
                DB.ExecuteNonQuery(sql3);
                DB.ExecuteNonQuery(sql4);
                DB.ExecuteNonQuery(sql5);
                DB.ExecuteNonQuery(sql6);
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
            finally
            {
                DB.Close();
            }
            return ret;
        }

        ///保存签收信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveQS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//二维码
                string sc_qty = jarr.ContainsKey("sc_qty") ? jarr["sc_qty"].ToString() : "";//二维码
                string qs_qty = jarr.ContainsKey("qs_qty") ? jarr["qs_qty"].ToString() : "";//二维码
                string qs_staff_no = jarr.ContainsKey("qs_staff_no") ? jarr["qs_staff_no"].ToString() : "";//二维码
                string qs_staff_name = jarr.ContainsKey("qs_staff_name") ? jarr["qs_staff_name"].ToString() : "";//二维码
                #endregion
                #region 逻辑

                string qs_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_QS(TASK_NO,SC_QTY,QS_QTY,QS_STAFF_NAME,QS_STAFF_NO,QS_TIME,CREATEBY,CREATEDATE,CREATETIME)
                                                                values('{task_no}','{sc_qty}','{qs_qty}','{qs_staff_name}','{qs_staff_no}','{qs_time}','{qs_staff_no}','{date}','{time}')");
                string qs_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_QS");

                DB.ExecuteNonQuery($"update qcm_ex_task_list_m set task_state='1' where task_no='{task_no}'");
                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = qs_id;
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

        ///保存取走信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveQZ(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//二维码
                string sc_qty = jarr.ContainsKey("sc_qty") ? jarr["sc_qty"].ToString() : "";//二维码
                string qz_qty = jarr.ContainsKey("qz_qty") ? jarr["qz_qty"].ToString() : "";//二维码
                string qz_staff_no = jarr.ContainsKey("qz_staff_no") ? jarr["qz_staff_no"].ToString() : "";//二维码
                string qz_staff_name = jarr.ContainsKey("qz_staff_name") ? jarr["qz_staff_name"].ToString() : "";//二维码
                #endregion
                #region 逻辑

                string qs_time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_QZ(TASK_NO,SC_QTY,QZ_QTY,QZ_STAFF_NAME,QZ_STAFF_NO,QZ_TIME,CREATEBY,CREATEDATE,CREATETIME)
                                                                values('{task_no}','{sc_qty}','{qz_qty}','{qz_staff_name}','{qz_staff_no}','{qs_time}','{qz_staff_no}','{date}','{time}')");
                string qz_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_QZ");

                DB.ExecuteNonQuery($"update qcm_ex_task_list_m set task_state='6' where task_no='{task_no}'");
                DB.Commit();

                ret.IsSuccess = true;
                ret.RetData = qz_id;
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

        ///保存检验设置项信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveItemCheck(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//二维码
                string dtstr = jarr.ContainsKey("list") ? jarr["list"].ToString() : "";
                string headstr = jarr.ContainsKey("head") ? jarr["head"].ToString() : "";
                string delete_list_str = jarr.ContainsKey("delete_list") ? jarr["delete_list"].ToString() : "";
                string staff_no = jarr.ContainsKey("staff_no") ? jarr["staff_no"].ToString() : "";
                string staff_name = jarr.ContainsKey("staff_name") ? jarr["staff_name"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                Dictionary<string, object> head_info = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(headstr);
                List<Dictionary<string, object>> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dtstr);
                List<string> delete_list = JsonConvert.DeserializeObject<List<string>>(delete_list_str);
                string sql = string.Empty;
                #endregion
                #region 逻辑

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                #region 更新表头信息
                if (head_info != null && head_info.Count > 0)
                {
                    string updateMSql = $@"update qcm_ex_task_list_m set {string.Join(',', head_info.Keys.Select(x => $@"{x}=@{x}"))} where task_no='{task_no}'";
                    DB.ExecuteNonQuery(updateMSql, head_info);
                }
                #endregion
                bool haveInsert = false;
                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item["d_id"].ToString()))
                    {
                        haveInsert = true;
                        DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_D(TASK_NO,SOURCES,INSPECTION_CODE,INSPECTION_NAME,INSPECTION_TYPE,judgment_criteria,judge_type,STANDARD_VALUE,UNIT,SAMPLE_QTY,G_FORMULA_CODE,D_FORMULA_CODE,ART_D_REMARK,CHOICE_NO,CHOICE_NAME,CREATEBY,CREATEDATE,CREATETIME)
                                                                values('{task_no}','{item["source"]}','{item["inspection_code"]}','{item["inspection_name"]}','{item["inspection_type"]}','{item["judgment_criteria"]}','{item["judge_type"]}','{item["standard_value"]}','{item["unit"]}','{item["sample_qty"]}','{item["g_formula_code"]}','{item["d_formula_code"]}','{item["art_d_remark"]}','{item["choice_no"]}','{item["choice_name"]}','{staff_no}','{date}','{time}')");
                        string d_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "QCM_EX_TASK_LIST_D");
                        for (int i = 1; i <= int.Parse(item["sample_qty"].ToString()); i++)
                        {
                            DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_QRCODE(TASK_NO,D_ID,INSPECTION_CODE,INSPECTION_NAME,INSPECTION_TYPE,ITEM_SEQ,CREATEBY,CREATEDATE,CREATETIME)
                                                                values('{task_no}','{d_id}','{item["inspection_code"]}','{item["inspection_name"]}','{item["inspection_type"]}','{i}','{staff_no}','{date}','{time}')");
                        }
                    }
                    else
                    {
                        sql = $"update QCM_EX_TASK_LIST_D set STANDARD_VALUE='{item["standard_value"]}',G_FORMULA_CODE='{item["g_formula_code"]}',D_FORMULA_CODE='{item["d_formula_code"]}',SAMPLE_QTY='{item["sample_qty"]}',MODIFYBY='{staff_no}',MODIFYDATE='{date}',MODIFYTIME='{time}' where id={item["d_id"]}";
                        DB.ExecuteNonQuery(sql);

                        DB.ExecuteNonQuery($"delete QCM_EX_TASK_LIST_QRCODE where d_id='{item["d_id"].ToString()}'");
                        for (int i = 1; i <= int.Parse(item["sample_qty"].ToString()); i++)
                        {
                            DB.ExecuteNonQuery($@"INSERT INTO QCM_EX_TASK_LIST_QRCODE(TASK_NO,D_ID,INSPECTION_CODE,INSPECTION_NAME,INSPECTION_TYPE,ITEM_SEQ,CREATEBY,CREATEDATE,CREATETIME)
                                                                values('{task_no}','{item["d_id"].ToString()}','{item["inspection_code"]}','{item["inspection_name"]}','{item["inspection_type"]}','{i}','{staff_no}','{date}','{time}')");
                        }
                    }
                }

                //删除 QCM_EX_TASK_LIST_D
                foreach (var item in delete_list)
                {
                    DB.ExecuteNonQuery($"delete QCM_EX_TASK_LIST_D where id={item}");
                    DB.ExecuteNonQuery($"delete QCM_EX_TASK_LIST_QRCODE where d_id={item}");
                }

                //更新检验单的结果和完结日期
                var Parameters = new Dictionary<string, object>();
                if (haveInsert || DB.GetStringline($@"select count(1) from qcm_ex_task_list_d where task_no='{task_no}'").Equals("0"))
                {
                    sql = $@"update qcm_ex_task_list_m set test_result='',completion_date='' where task_no=@task_no";
                    Parameters = new Dictionary<string, object>();
                    Parameters.Add("task_no", task_no);
                    DB.ExecuteNonQuery(sql, Parameters);
                }
                else
                {
                    sql = $@"select count(1) from qcm_ex_task_list_d where task_no='{task_no}' and  nvl(item_test_result,'1')='1'";
                    if (DB.GetStringline(sql).Equals("0"))
                    {
                        string test_result = "";
                        sql = $@"select count(1) from qcm_ex_task_list_d where task_no='{task_no}' and item_test_result='FAIL'";
                        if (!DB.GetStringline(sql).Equals("0"))
                            test_result = "FAIL";
                        else
                            test_result = "PASS";

                        sql = $@"update qcm_ex_task_list_m set test_result=@test_result,completion_date=submission_date where task_no=@task_no";
                        Parameters = new Dictionary<string, object>();
                        Parameters.Add("test_result", test_result);
                        Parameters.Add("task_no", task_no);
                        DB.ExecuteNonQuery(sql, Parameters);
                    }
                }

                //DB.ExecuteNonQuery($"update qcm_ex_task_list_m set task_state='4',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where task_no='{task_no}'");
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
            finally
            {
                DB.Close();
            }
            return ret;
        }

        ///获取成品鞋-测试定制项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_cpx_checkItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string AGE_GENDER_CODE = jarr.ContainsKey("AGE_GENDER_CODE") ? jarr["AGE_GENDER_CODE"].ToString() : "";//二维码
                string CATEGORY_CODE = jarr.ContainsKey("CATEGORY_CODE") ? jarr["CATEGORY_CODE"].ToString() : "";
                string FGT_CODE = jarr.ContainsKey("FGT_CODE") ? jarr["FGT_CODE"].ToString() : "";
                string FINISHED_PRODUCT_CODE = jarr.ContainsKey("FINISHED_PRODUCT_CODE") ? jarr["FINISHED_PRODUCT_CODE"].ToString() : "";
                string PB_TYPE_CODE = jarr.ContainsKey("PB_TYPE_CODE") ? jarr["PB_TYPE_CODE"].ToString() : "";
                string PRODUCT_LEVEL_CODE = jarr.ContainsKey("PRODUCT_LEVEL_CODE") ? jarr["PRODUCT_LEVEL_CODE"].ToString() : "";

                string AGE_GENDER_CODE_Where = $" and a.AGE_GENDER_CODE='{AGE_GENDER_CODE}'";
                if (string.IsNullOrEmpty(AGE_GENDER_CODE))
                {
                    AGE_GENDER_CODE_Where = " and (a.AGE_GENDER_CODE is null or a.AGE_GENDER_CODE=' ')";
                }
                string CATEGORY_CODE_Where = $" and a.CATEGORY_CODE='{CATEGORY_CODE}'";
                if (string.IsNullOrEmpty(CATEGORY_CODE))
                {
                    CATEGORY_CODE_Where = " and (a.CATEGORY_CODE is null or a.CATEGORY_CODE=' ')";
                }
                string FGT_CODE_Where = $" and a.FGT_CODE='{FGT_CODE}'";
                if (string.IsNullOrEmpty(FGT_CODE))
                {
                    FGT_CODE_Where = " and (a.FGT_CODE is null or a.FGT_CODE=' ')";
                }
                string FINISHED_PRODUCT_CODE_Where = $" and (a.FINISHED_PRODUCT_CODE='{FINISHED_PRODUCT_CODE}')";
                if (string.IsNullOrEmpty(FINISHED_PRODUCT_CODE))
                {
                    FINISHED_PRODUCT_CODE_Where = " and (a.FINISHED_PRODUCT_CODE is null or a.FINISHED_PRODUCT_CODE=' ')";
                }
                string PB_TYPE_CODE_Where = $" and a.PB_TYPE_CODE='{PB_TYPE_CODE}'";
                if (string.IsNullOrEmpty(PB_TYPE_CODE))
                {
                    PB_TYPE_CODE_Where = " and (a.PB_TYPE_CODE is null or a.PB_TYPE_CODE=' ')";
                }
                string PRODUCT_LEVEL_CODE_Where = $" and a.PRODUCT_LEVEL_CODE='{PRODUCT_LEVEL_CODE}'";
                if (string.IsNullOrEmpty(PRODUCT_LEVEL_CODE))
                {
                    PRODUCT_LEVEL_CODE_Where = " and (a.PRODUCT_LEVEL_CODE is null or a.PRODUCT_LEVEL_CODE=' ')";
                }
                #endregion
                #region 逻辑
                var dt = DB.GetDataTable($@"select 
                                        'conventional' as type,
                                        '8' as inspection_type ,
                                        'Finished Shoes-Testing' as inspection_type_name,
                                        '' as choice_no,
                                        '' as choice_name,
                                        c.inspection_code,
                                        c.inspection_name,
                                        to_char(c.judgment_criteria) as judgment_criteria,
									    g1.enum_value as judgment_criteria_name,
                                        to_char(c.judge_type) as judge_type,
                                        g2.enum_value as judge_type_name,
                                        to_char(c.standard_value) as standard_value,
                                        '' as unit,
                                        c.remarks
                                    from 
                                    bdm_shoes_check_test_c a 
                                    join bdm_shoes_check_test_u b on a.ID=b.C_ID
                                    join bdm_shoes_check_test_m c on b.M_ID=c.ID
                                    left join sys001m g1 on c.judgment_criteria=g1.enum_code and g1.enum_type = 'enum_judgment_criteria' 
                                    left join sys001m g2 on c.judge_type=g2.enum_code and g2.enum_type = 'enum_testitem_type' 
                                    where 1=1  {AGE_GENDER_CODE_Where}{CATEGORY_CODE_Where}{FGT_CODE_Where}{FINISHED_PRODUCT_CODE_Where}{PB_TYPE_CODE_Where}{PRODUCT_LEVEL_CODE_Where}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取部件-测试定制项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_bj_checkItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string PARTS_CODE = jarr.ContainsKey("PARTS_CODE") ? jarr["PARTS_CODE"].ToString() : "";//二维码
                //string CATEGORY_CODE = jarr.ContainsKey("CATEGORY_CODE") ? jarr["CATEGORY_CODE"].ToString() : "";
                //string POSITION_CODE = jarr.ContainsKey("POSITION_CODE") ? jarr["POSITION_CODE"].ToString() : "";
                string fgt_code = jarr.ContainsKey("fgt_code") ? jarr["fgt_code"].ToString() : "";//fgt

                //string PARTS_CODE_Where = $" and a.PARTS_CODE='{PARTS_CODE}'";
                //if (string.IsNullOrEmpty(PARTS_CODE))
                //{
                //    PARTS_CODE_Where = " and (a.PARTS_CODE is null or a.PARTS_CODE=' ')";
                //}
                //string CATEGORY_CODE_Where = $" and a.CATEGORY_CODE='{CATEGORY_CODE}'";
                //if (string.IsNullOrEmpty(CATEGORY_CODE))
                //{
                //    CATEGORY_CODE_Where = " and (a.CATEGORY_CODE is null or a.CATEGORY_CODE=' ')";
                //}
                //string POSITION_CODE_Where = $" and a.POSITION_CODE='{POSITION_CODE}'";
                //if (string.IsNullOrEmpty(POSITION_CODE))
                //{
                //    POSITION_CODE_Where = " and (a.POSITION_CODE is null or a.POSITION_CODE=' ')";
                //}

                string fgt_code_Where = $" and a.fgt_code='{fgt_code}'";
                if (string.IsNullOrEmpty(fgt_code))
                {
                    fgt_code_Where = " and (a.fgt_code is null or a.fgt_code=' ')";
                }////Component-Test
                #endregion
                #region 逻辑//Component-Test
                var dt = DB.GetDataTable($@"select 
                                        'conventional' as type,
                                        '11' as inspection_type ,
                                        'Component-Test' as inspection_type_name,
                                        '' as choice_no,
                                        '' as choice_name,
                                        c.inspection_code,
                                        c.inspection_name,
                                        to_char(c.judgment_criteria) as judgment_criteria,
									    g1.enum_value as judgment_criteria_name,
                                        to_char(c.judge_type) as judge_type,
                                        g2.enum_value as judge_type_name,
                                        to_char(c.standard_value) as standard_value,
                                        '' as unit,
                                        c.remarks
                                    from 
                                    bdm_parts_testitem_c a 
                                    join bdm_parts_testitem_u b on a.ID=b.C_ID
                                    join bdm_parts_testitem_m c on b.M_ID=c.ID
                                    left join sys001m g1 on c.judgment_criteria=g1.enum_code and g1.enum_type = 'enum_judgment_criteria' 
                                    left join sys001m g2 on c.judge_type=g2.enum_code and g2.enum_type = 'enum_testitem_type' 
                                    where 1=1  {fgt_code_Where}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取工艺-测试定制项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_gy_checkItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string WORKMANSHIP_CODE = jarr.ContainsKey("WORKMANSHIP_CODE") ? jarr["WORKMANSHIP_CODE"].ToString() : "";//二维码
                //string CATEGORY_CODE = jarr.ContainsKey("CATEGORY_CODE") ? jarr["CATEGORY_CODE"].ToString() : "";
                //string POSITION_CODE = jarr.ContainsKey("POSITION_CODE") ? jarr["POSITION_CODE"].ToString() : "";

                string fgt_code = jarr.ContainsKey("fgt_code") ? jarr["fgt_code"].ToString() : "";

                //string WORKMANSHIP_CODE_Where = $" and a.WORKMANSHIP_CODE='{WORKMANSHIP_CODE}'";
                //if (string.IsNullOrEmpty(WORKMANSHIP_CODE))
                //{
                //    WORKMANSHIP_CODE_Where = " and (a.WORKMANSHIP_CODE is null or a.WORKMANSHIP_CODE=' ')";
                //}
                //string CATEGORY_CODE_Where = $" and a.CATEGORY_CODE='{CATEGORY_CODE}'";
                //if (string.IsNullOrEmpty(CATEGORY_CODE))
                //{
                //    CATEGORY_CODE_Where = " and (a.CATEGORY_CODE is null or a.CATEGORY_CODE=' ')";
                //}
                //string POSITION_CODE_Where = $" and a.POSITION_CODE='{POSITION_CODE}'";
                //if (string.IsNullOrEmpty(POSITION_CODE))
                //{
                //    POSITION_CODE_Where = " and (a.POSITION_CODE is null or a.POSITION_CODE=' ')";
                //}

                string fgt_code_Where = $" and a.fgt_code='{fgt_code}'";
                if (string.IsNullOrEmpty(fgt_code))
                {
                    fgt_code_Where = " and (a.fgt_code is null or a.fgt_code=' ')";
                }
                #endregion
                #region 逻辑
                var dt = DB.GetDataTable($@"select 
                                        'conventional' as type,
                                        '10' as inspection_type ,
                                        'Process-Test' as inspection_type_name,
                                        '' as choice_no,
                                        '' as choice_name,
                                        c.inspection_code,
                                        c.inspection_name,
                                        to_char(c.judgment_criteria) as judgment_criteria,
									    g1.enum_value as judgment_criteria_name,
                                        to_char(c.judge_type) as judge_type,
                                        g2.enum_value as judge_type_name,
                                        to_char(c.standard_value) as standard_value,
                                        '' as unit,
                                        c.remarks
                                    from 
                                    bdm_workmanship_testitem_c a 
                                    join bdm_workmanship_testitem_u b on a.ID=b.C_ID
                                    join bdm_workmanship_testitem_m c on b.M_ID=c.ID
                                    left join sys001m g1 on c.judgment_criteria=g1.enum_code and g1.enum_type = 'enum_judgment_criteria' 
                                    left join sys001m g2 on c.judge_type=g2.enum_code and g2.enum_type = 'enum_testitem_type' 
                                    where 1=1  {fgt_code_Where}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取材料-测试定制项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_cl_checkItem(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //string MATERIAL_TYPE_CODE = jarr.ContainsKey("MATERIAL_TYPE_CODE") ? jarr["MATERIAL_TYPE_CODE"].ToString() : "";//二维码
                //string CATEGORY_CODE = jarr.ContainsKey("CATEGORY_CODE") ? jarr["CATEGORY_CODE"].ToString() : "";
                //string POSITION_CODE = jarr.ContainsKey("POSITION_CODE") ? jarr["POSITION_CODE"].ToString() : "";
                string fgt_code = jarr.ContainsKey("fgt_code") ? jarr["fgt_code"].ToString() : "";

                //string MATERIAL_TYPE_CODE_Where = $" and a.MATERIAL_TYPE_CODE='{MATERIAL_TYPE_CODE}'";
                //if (string.IsNullOrEmpty(MATERIAL_TYPE_CODE))
                //{
                //    MATERIAL_TYPE_CODE_Where = " and (a.MATERIAL_TYPE_CODE is null or a.MATERIAL_TYPE_CODE=' ')";
                //}
                //string CATEGORY_CODE_Where = $" and a.CATEGORY_CODE='{CATEGORY_CODE}'";
                //if (string.IsNullOrEmpty(CATEGORY_CODE))
                //{
                //    CATEGORY_CODE_Where = " and (a.CATEGORY_CODE is null or a.CATEGORY_CODE=' ')";
                //}
                //string POSITION_CODE_Where = $" and a.POSITION_CODE='{POSITION_CODE}'";
                //if (string.IsNullOrEmpty(POSITION_CODE))
                //{
                //    POSITION_CODE_Where = " and (a.POSITION_CODE is null or a.POSITION_CODE=' ')";
                //}
                string fgt_code_Where = $" and a.fgt_code='{fgt_code}'";
                if (string.IsNullOrEmpty(fgt_code))
                {
                    fgt_code_Where = " and (a.fgt_code is null or a.fgt_code=' ')";
                }//材料-测试
                #endregion
                #region 逻辑
                var dt = DB.GetDataTable($@"select 
                                        'conventional' as type,
                                        '9' as inspection_type ,
                                        'Material-Test' as inspection_type_name,
                                        '' as choice_no,
                                        '' as choice_name,
                                        c.inspection_code,
                                        c.inspection_name,
                                        to_char(c.judgment_criteria) as judgment_criteria,
									    g1.enum_value as judgment_criteria_name,
                                        to_char(c.judge_type) as judge_type,
                                        g2.enum_value as judge_type_name,
                                        to_char(c.standard_value) as standard_value,
                                        '' as unit,
                                        c.remarks
                                    from 
                                    bdm_material_testitem_c a 
                                    join bdm_material_testitem_u b on a.ID=b.C_ID
                                    join bdm_material_testitem_m c on b.M_ID=c.ID
                                    left join sys001m g1 on c.judgment_criteria=g1.enum_code and g1.enum_type = 'enum_judgment_criteria' 
                                    left join sys001m g2 on c.judge_type=g2.enum_code and g2.enum_type = 'enum_testitem_type' 
                                    where 1=1  {fgt_code_Where}");
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintExStockList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(Data);

                List<string> list=Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["id"].ToString());

                string where = "";

                if (list.Count >0)
                {
                    where += $" and QCM_EX_TASK_STOCK.ID in ({string.Join(',', list.Select(x => $"'{x}'"))})";
                }
                #endregion

                #region 逻辑
                string sql = $@"
SELECT
    QCM_EX_TASK_STOCK.STOCK_CODE AS 库位代号,
	QCM_EX_TASK_STOCK.STOCK_NAME 库位名称,
	QCM_EX_TASK_STOCK.WAREHOUSE_CODE 仓库代号,
	QCM_EX_TASK_WH.WAREHOUSE_NAME 仓库名称
FROM

    QCM_EX_TASK_STOCK
LEFT JOIN QCM_EX_TASK_WH ON QCM_EX_TASK_STOCK.WAREHOUSE_CODE = QCM_EX_TASK_WH.WAREHOUSE_CODE
WHERE 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
                if(dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data",dt);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    //ret.RetData1 = dt;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "No data found！";
                    ret.IsSuccess = true;
                }
                

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///获取实验室库位资料列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExStockList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";

                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";

                string where = "";

                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" and (a.STOCK_CODE like '%{keyword}%' or a.STOCK_NAME like '%{keyword}%')";
                }
                #endregion

                #region 逻辑
                //string sql = $@"SELECT
                //             *
                //            FROM
                //             QCM_EX_TASK_STOCK
                //            where 1=1 {where}";

                string sql = $@"with tmp as (select count(stock_code)as present_quantity,stock_code from QCM_EX_TASK_ARC group by stock_code)
                                select a.*,case when tmp.present_quantity is not null then tmp.present_quantity else 0 end as present_capacity,
                                (a.stock_full_capacity-case when tmp.present_quantity is not null then tmp.present_quantity else 0 end) as available_capacity 
                                from QCM_EX_TASK_STOCK a left join tmp on a.stock_code=tmp.stock_code
                              where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), " order by ID desc");
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        ///删除实验室库位
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteExStock(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ids = jarr.ContainsKey("ids") ? jarr["ids"].ToString() : "";
                #endregion

                #region 逻辑
                string Stock_Code =DB.GetString($@" select stock_code from QCM_EX_TASK_STOCK where ID in({ids})");

                string sql = $"select count(stock_code) present_quantity from QCM_EX_TASK_ARC where stock_code = '{Stock_Code}' group by stock_code";

                int Current_capacity = DB.GetInt32(sql);

                if (Current_capacity > 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Selected Location has Storge. So Cannot delete it.";
                }
                else
                {
                    string sql2 = $@"delete 
	                            QCM_EX_TASK_STOCK where ID in({ids})";
                    DB.ExecuteNonQuery(sql2);
                    ret.IsSuccess = true;
                } 
                DB.Commit();
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


        ///编辑实验室库位
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditExStock(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString() : "";
                string name = jarr.ContainsKey("name") ? jarr["name"].ToString() : "";
                string capacity = jarr.ContainsKey("capacity") ? jarr["capacity"].ToString() : "";
                string housecode = jarr.ContainsKey("housecode") ? jarr["housecode"].ToString() : "";
                #endregion
                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                if (string.IsNullOrEmpty(id))
                {
                    int count = Convert.ToInt32(DB.GetStringline($"select count(1) from QCM_EX_TASK_STOCK where STOCK_CODE='{code}'"));
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Location number already exists";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into QCM_EX_TASK_STOCK(STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,CREATEDATE,CREATETIME,STOCK_FULL_CAPACITY)
                                                    values(
                                                    '{code}',
                                                    '{name}',
                                                    '{housecode}',
                                                    '{DateTime.Now.ToString("yyyy-MM-dd")}',
                                                    '{DateTime.Now.ToString("HH:mm:ss")}',
                                                    '{capacity}'
                                                        )
                                                ");
                }
                else
                {
                    DB.ExecuteNonQuery($@"update QCM_EX_TASK_STOCK set STOCK_NAME='{name}',WAREHOUSE_CODE='{housecode}',STOCK_FULL_CAPACITY='{capacity}' where ID={id}");
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
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 获取实验室仓库资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExWhList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string where = "";
                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $" and (WAREHOUSE_NAME like '%{keyword}%' or WAREHOUSE_CODE like '%{keyword}%')";
                }
                #endregion

                #region 逻辑
                string sql = $@"SELECT
	                            *
                            FROM
	                            QCM_EX_TASK_WH
                            where 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), " order by ID desc");
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }
        ///编辑实验室仓库
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditExWh(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string code = jarr.ContainsKey("code") ? jarr["code"].ToString() : "";
                string name = jarr.ContainsKey("name") ? jarr["name"].ToString() : "";
                #endregion
                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                if (string.IsNullOrEmpty(id))
                {
                    int count = Convert.ToInt32(DB.GetStringline($"select count(1) from QCM_EX_TASK_Wh where WAREHOUSE_CODE='{code}'"));
                    if (count > 0)
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Warehouse ID already exists";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into QCM_EX_TASK_Wh(WAREHOUSE_CODE,WAREHOUSE_NAME,CREATEDATE,CREATETIME)
                                                    values(
                                                    '{code}',
                                                    '{name}',
                                                    '{DateTime.Now.ToString("yyyy-MM-dd")}',
                                                    '{DateTime.Now.ToString("HH:mm:ss")}'
                                                        )
                                                ");
                }
                else
                {
                    DB.ExecuteNonQuery($@"update QCM_EX_TASK_Wh set WAREHOUSE_NAME ='{name}',WAREHOUSE_CODE='{code}' where ID={id}");
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
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 删除实验室仓库资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteExWh(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ids = jarr.ContainsKey("ids") ? jarr["ids"].ToString() : "";
                #endregion

                #region 逻辑
                string sql = $@"delete 
	                            QCM_EX_TASK_WH where ID in({ids})";
                DB.ExecuteNonQuery(sql);
                ret.IsSuccess = true;
                DB.Commit();
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
        /// 获取实验室存档资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExARCList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";
                string taskno = jarr.ContainsKey("taskno") ? jarr["taskno"].ToString() : "";
                string stock = jarr.ContainsKey("stock") ? jarr["stock"].ToString() : "";
                string artno = jarr.ContainsKey("artno") ? jarr["artno"].ToString() : "";
                string taskname = jarr.ContainsKey("taskname") ? jarr["taskname"].ToString() : "";
                string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";
                string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(taskno))
                {
                    where += $@" and ARC.TASK_NO like '%{taskno}%' ";
                }
                if (!string.IsNullOrWhiteSpace(stock))
                {
                    where += $@" and ARC.STOCK_CODE like '%{stock}%' ";
                }
                if (!string.IsNullOrWhiteSpace(artno))
                {
                    where += $@" and LM.ART_NO like '%{artno}%' ";
                }
                if (!string.IsNullOrWhiteSpace(taskname))
                {
                    where += $@"and(( LM.TEST_TYPE = '0' AND LM.ART_NO LIKE '%{taskname}%' ) 
			                            OR ( LM.TEST_TYPE = '1' AND LM.PARTS_NAME LIKE '%{taskname}%' ) 
			                            OR ( LM.TEST_TYPE = '2' AND LM.WORKMANSHIP_NAME LIKE '%{taskname}%' ) 
			                            OR ( LM.TEST_TYPE = '3' AND LM.MAKINGS_ID LIKE '%{taskname}%' ) 
			                            OR(LM.TEST_TYPE = '4' AND LM.ART_NO LIKE '%{taskname}%')) ";
                }
                if (!string.IsNullOrWhiteSpace(wh_date_start) && !string.IsNullOrWhiteSpace(wh_date_end))
                {
                    where += $@" and ARC.WAREHOUSING_DATE >= '{wh_date_start}' and  ARC.WAREHOUSING_DATE<='{wh_date_end}'";
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(wh_date_start))
                    {
                        where += $@" and ARC.WAREHOUSING_DATE >= '{wh_date_start}' ";
                    }
                    if (!string.IsNullOrWhiteSpace(wh_date_end))
                    {
                        where += $@" ARC.WAREHOUSING_DATE <= '{wh_date_end}' ";
                    }
                }
                #endregion

                #region 逻辑
                string sql = $@"SELECT
	                            ARC.ID AS ID,
	                            ARC.TASK_NO AS TASK_NO,
	                            ARC.STOCK_CODE AS STOCK_CODE,
	                            ARC.WAREHOUSING_DATE AS WAREHOUSING_DATE,
	                            ARC.DUE_DATE AS DUE_DATE,
                                ARC.COLOUR_TYPE AS COLOUR_TYPE,
	                            ARC.NEXT_REVIEW_DATE AS NEXT_REVIEW_DATE,
	                            ARC.REMARKS AS REMARKS,
	                            LM.ART_NO AS ART_NO,
	                            (    CASE
			                            WHEN LM.TEST_TYPE = '0' THEN
			                            LM.ART_NO 
			                            WHEN LM.TEST_TYPE = '1' THEN
			                            LM.PARTS_NAME 
			                            WHEN LM.TEST_TYPE = '2' THEN
			                            LM.WORKMANSHIP_NAME 
			                            WHEN LM.TEST_TYPE = '3' THEN
			                            LM.MAKINGS_ID 
			                            WHEN LM.TEST_TYPE = '4' THEN
			                            LM.ART_NO ELSE LM.TEST_TYPE 
		                            END 
		                            ) AS TASKNAME,
                                LM.Staff_No,
                                LM.Staff_Name
	                            FROM
		                            QCM_EX_TASK_ARC ARC
		                            LEFT JOIN QCM_EX_TASK_LIST_M LM ON LM.TASK_NO = ARC.TASK_NO 
		                             
                            where 1=1 and LM.test_type !='4' {where}";//and LM.test_type !='4' is added by Ashok to remove bonding data
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), " order by ID desc");
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                //foreach(DataRow dr in dt.Rows)
                //{
                //    dr["REVIEW_DATE"] = DateTime.Parse(dr["DUE_DATE"].ToString()).AddMonths(-4);
                //}

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExARCList_Previous(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
               // string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
               // string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";
                string Task_No = jarr.ContainsKey("Task_No") ? jarr["Task_No"].ToString() : ""; 
                //string wh_date_start = jarr.ContainsKey("wh_date_start") ? jarr["wh_date_start"].ToString() : "";
                //string wh_date_end = jarr.ContainsKey("wh_date_end") ? jarr["wh_date_end"].ToString() : "";
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(Task_No))
                {
                    where += $@" and task_no like '%{Task_No}%' ";
                } 
               
                  
                string sql = $@"select 
                                task_no,
                                task_name,
                                stock_code,
                                art_no,
                                colour_type,
                                warehousing_date,
                                latest_review_date,
                                remarks,
                                createdby
                                from QCM_EX_TASK_ARC_LOG  
                            where 1=1 {where} order by latest_review_date desc";
                DataTable dt = DB.GetDataTable(sql);
                //DataTable dt = CommonBASE.GetPageDataTable(DB, sql, " order by ID desc");
                //int rowCount = CommonBASE.GetPageDataTableCount(DB, sql); 
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                //dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

               
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 编辑存档资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditARC(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string Taskcode = jarr.ContainsKey("Taskcode") ? jarr["Taskcode"].ToString() : "";
                string Stockcode = jarr.ContainsKey("Stockcode") ? jarr["Stockcode"].ToString() : "";
                #endregion
                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                int count = Convert.ToInt32(DB.GetStringline($"select count(1) from QCM_EX_TASK_STOCK where STOCK_CODE='{Stockcode}'"));
                int task_count = Convert.ToInt32(DB.GetStringline($"SELECT COUNT(1) FROM qcm_ex_task_list_m WHERE TASK_NO='{Taskcode}'"));
                DB.ExecuteNonQuery($"update QCM_EX_TASK_ARC a set a.stock_code='' where  a.due_date  < to_char(sysdate-10,'yyyy-MM-dd') and a.stock_code is not null");
                if (count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Location number does not exist";//库位编号不存在
                    return ret;
                }
                else if (task_count <= 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "lab number does not exist";//实验室编号不存在
                    return ret;
                }
                else
                {
                   int Available_Capacity = DB.GetInt32($"with tmp as (select count(stock_code)as present_quantity,stock_code from QCM_EX_TASK_ARC group by stock_code) select(a.stock_full_capacity - case when tmp.present_quantity is not null then tmp.present_quantity else 0 end) as available_capacity " +
                                                       $"from QCM_EX_TASK_STOCK a left join tmp on a.stock_code = tmp.stock_code where a.stock_code ='{Stockcode}'");
                   int Taskcount = Convert.ToInt32(DB.GetStringline($"select count(1) from QCM_EX_TASK_ARC where TASK_NO='{Taskcode}'"));
                    if (Available_Capacity > 0)
                    {
                        if (string.IsNullOrEmpty(id) && Taskcount <= 0)
                        {
                            //DB.ExecuteNonQuery($@"insert into QCM_EX_TASK_ARC (TASK_NO,STOCK_CODE,WAREHOUSING_DATE,DUE_DATE)
                            //                            values(
                            //                            '{Taskcode}',
                            //                            '{Stockcode}',
                            //                            '{DateTime.Now.ToString("yyyy-MM-dd")}',
                            //                            '{DateTime.Now.AddDays(90).ToString("yyyy-MM-dd")}'
                            //                                )
                            //                        ");  //OLD QUERY
                             

                            DB.ExecuteNonQuery($@"insert into QCM_EX_TASK_ARC (TASK_NO,STOCK_CODE,WAREHOUSING_DATE,DUE_DATE)
                                                    values(
                                                    '{Taskcode}',
                                                    '{Stockcode}',
                                                    '{DateTime.Now.ToString("yyyy-MM-dd")}',
                                                    '{DateTime.Now.AddMonths(9).ToString("yyyy-MM-dd")}'
                                                        )
                                                ");  //MODIFIED BY ASHOK
                        }
                        else if (!string.IsNullOrEmpty(id))
                        {
                            DB.ExecuteNonQuery($@"update QCM_EX_TASK_ARC set TASK_NO='{Taskcode}',STOCK_CODE='{Stockcode}' where ID='{id}'");

                        }
                        else
                        {
                            DB.ExecuteNonQuery($@"update QCM_EX_TASK_ARC set STOCK_CODE='{Stockcode}' where TASK_NO='{Taskcode}'");
                        }
                        DB.Commit();
                        ret.IsSuccess = true;
                    }
                    else
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "Selected Location has full storage Please select another one";
                    }
                  
                    
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditARC2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string TASK_NO = jarr.ContainsKey("TASK_NO") ? jarr["TASK_NO"].ToString() : "";
                string TASKNAME = jarr.ContainsKey("TASKNAME") ? jarr["TASKNAME"].ToString() : "";
                string STOCK_CODE = jarr.ContainsKey("STOCK_CODE") ? jarr["STOCK_CODE"].ToString() : "";
                string ART_NO = jarr.ContainsKey("ART_NO") ? jarr["ART_NO"].ToString() : "";
                string COLOUR_TYPE = jarr.ContainsKey("COLOUR_TYPE") ? jarr["COLOUR_TYPE"].ToString() : "";
                string WAREHOUSING_DATE = jarr.ContainsKey("WAREHOUSING_DATE") ? jarr["WAREHOUSING_DATE"].ToString() : "";
                string REVIEW_DATE = jarr.ContainsKey("REVIEW_DATE") ? jarr["REVIEW_DATE"].ToString() : "";
                string DUE_DATE = jarr.ContainsKey("DUE_DATE") ? jarr["DUE_DATE"].ToString() : "";
                string REMARKS = jarr.ContainsKey("REMARKS") ? jarr["REMARKS"].ToString() : "";
                string MAIL_STATUS = jarr.ContainsKey("MAIL_STATUS") ? jarr["MAIL_STATUS"].ToString() : "";
                string EMPNO = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string EMP_NAME = DB.GetString($@"select a.staff_name from hr001m a where a.staff_no='{EMPNO}'");
                #endregion
                #region 逻辑
                DB.Open();
                DB.BeginTransaction(); 
                string sql1 = $@"insert into QCM_EX_TASK_ARC_LOG values(QCM_EX_TASK_ARC_LOG_seq.nextval,'{TASK_NO}','{TASKNAME}','{STOCK_CODE}','{ART_NO}','{COLOUR_TYPE}','{WAREHOUSING_DATE}',sysdate,'{DUE_DATE}','{REMARKS}','{EMP_NAME}','{ID}')";
                DB.ExecuteNonQuery(sql1);
                string sql2 = $@"update QCM_EX_TASK_ARC set colour_Type='{COLOUR_TYPE}' ,next_review_date ='{REVIEW_DATE}' , Remarks='{REMARKS}',Mail_status='{MAIL_STATUS}' where Task_No ='{TASK_NO}'";
                DB.ExecuteNonQuery(sql2);

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
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 删除存档资料
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteARC(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
            DB.Open();
            DB.BeginTransaction();
            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ids = jarr.ContainsKey("ids") ? jarr["ids"].ToString() : "";
                #endregion

                #region 逻辑
                string sql = $@"delete 
	                            QCM_EX_TASK_ARC where ID in({ids})";
                DB.ExecuteNonQuery(sql);

                string sql2 = $@"delete 
	                            QCM_EX_TASK_ARC_LOG where M_ID in({ids})";
                DB.ExecuteNonQuery(sql2);

                ret.IsSuccess = true;
                DB.Commit();
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
        /// 获取设备信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExDevList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";

                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";

                string where = "";

                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $@" and (
                                        EQ_INFO_NO like '%{keyword}%' or 
                                        EQ_INFO_NAME like '%{keyword}%' or
                                        DEPARTMENT_CODE like '%{keyword}%' or
                                        DEPARTMENT_NAME like '%{keyword}%' or
                                        WORKSHOP_SECTION_NO like '%{keyword}%' or
                                        WORKSHOP_SECTION_NAME like '%{keyword}%' or
                                        EQ_NO like '%{keyword}%' or
                                        EQ_NAME like '%{keyword}%' or
                                        CONTROL_TYPE like '%{keyword}%' or
                                        WH_DATE like '%{keyword}%' or
                                        JZ_DATE like '%{keyword}%' or
                                        REMARK like '%{keyword}%')";
                }
                #endregion

                #region 逻辑
                string sql = $@"
                            SELECT
                            ID as id,
	                        EQ_INFO_NO as 编号,
                            EQ_INFO_NAME as 设备名称,
                            DEPARTMENT_CODE as 部门编号,
                            DEPARTMENT_NAME as 部门,
                            WORKSHOP_SECTION_NO as 工段编号,
                            WORKSHOP_SECTION_NAME as 工段,
                            EQ_NO as 设备类型编号,
                            EQ_NAME as 设备类型,
                            CONTROL_TYPE as 管控类型设备,
                            WH_DATE as 进仓时间,
                            JZ_DATE as 最近校正日期,
                            REMARK as 备注
                            FROM
	                            BDM_EQ_INFO_M
                            WHERE 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

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
        /// 设备信息打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintDevList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                List<string> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["id"].ToString());

                string where = "";

                if (list.Count > 0)
                {
                    where += $" and ID in ({string.Join(',', list.Select(x => $"'{x}'"))})";
                }
                #endregion

                #region 逻辑
                //                string sql = $@"
                //    SELECT
                //    ID as id,
                //	EQ_INFO_NO as 编号,
                //    EQ_INFO_NAME as 设备名称,
                //  DEPARTMENT_CODE as 部门编号,
                //  DEPARTMENT_NAME as 部门,
                //  WORKSHOP_SECTION_NO as 工段编号,
                //  WORKSHOP_SECTION_NAME as 工段,
                //  EQ_NO as 设备类型编号,
                //  EQ_NAME as 设备类型,
                //  CONTROL_TYPE as 管控类型设备,
                //  WH_DATE as 进仓时间,
                //  JZ_DATE as 最近校正日期,
                //  REMARK as 备注
                //FROM
                //    BDM_EQ_INFO_M
                //WHERE 1=1 {where}";

                string sql = $@"
    SELECT
    ID as id,
	EQ_INFO_NO,
    EQ_INFO_NAME,
  DEPARTMENT_CODE,
  DEPARTMENT_NAME,
  WORKSHOP_SECTION_NO,
  WORKSHOP_SECTION_NAME,
  EQ_NO,
  EQ_NAME,
  CONTROL_TYPE,
  WH_DATE,
  JZ_DATE,
  REMARK
FROM
    BDM_EQ_INFO_M
WHERE 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    //ret.RetData1 = dt;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "Check data！";
                    ret.IsSuccess = true;
                }


                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetInfoByTaskNo(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                #endregion

                #region 逻辑
                string sql = $@"SELECT * FROM QCM_EX_TASK_LIST_M WHERE TASK_NO='{task_no}'";
                DataTable dt = DB.GetDataTable(sql);

                if (dt.Rows.Count==0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "invalid tracking number";
                    return ret;
                }
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;

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
        /// 收料日期+料号找到对应的时间的收料单号
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Getchk_list(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string rcpt_date = jarr.ContainsKey("rcpt_date") ? jarr["rcpt_date"].ToString() : "";//收料日期
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//料号
                string org_id = jarr.ContainsKey("org_id") ? jarr["org_id"].ToString() : "";//org_id
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//chk_no
                #endregion
                #region 逻辑

                Dictionary<string, string> res = new Dictionary<string, string>();
                res.Add("chk_nolist", "");
                res.Add("part_list_str", "");
                res.Add("color_str", "");

                rcpt_date= DB.GetString($@"SELECT TO_CHAR(RCPT_DATE,'yyyy-MM-dd') FROM WMS_RCPT_M WHERE CHK_NO='{chk_no}' AND ORG_ID='{org_id}'");
                if (string.IsNullOrEmpty(rcpt_date))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $@"{chk_no}收料单 does not exist";//
                }
                else
                {
                    string sql = $@"SELECT
	{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT a.CHK_NO", "a.CHK_NO")} AS CHK_NO
FROM
	wms_rcpt_m A left join wms_rcpt_d b on a.chk_No=b.chk_No
where b.item_no='{item_no}' and a.RCPT_DATE=to_date('{rcpt_date}','yyyy-MM-dd')";
                    string chk_nolist = DB.GetString(sql);
                    res["chk_nolist"] = chk_nolist;

                    if (!string.IsNullOrWhiteSpace(chk_nolist))
                    {
                        List<string> part_list = chk_nolist.Split(',').ToList();
                        string part_sql = $@"SELECT {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT e.PART_NO", "e.PART_NO")} FROM wms_rcpt_d b 
INNER JOIN bdm_purchase_order_item e on e.order_no = b.SOURCE_NO and e.order_seq = b.SOURCE_SEQ
WHERE
	b.chk_no IN ({string.Join(",", part_list.Select(x => $@"'{x}'"))})";
                        string part_list_str = DB.GetString(part_sql);
                        res["part_list_str"] = part_list_str;

                        string color_sql = $@"SELECT {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT color_name", "color_name")} FROM bdm_rd_item WHERE item_no IN(SELECT
			b.item_no
		FROM
		wms_rcpt_d b 
		INNER JOIN bdm_purchase_order_item e on e.order_no = b.SOURCE_NO and e.order_seq = b.SOURCE_SEQ
		WHERE
			b.chk_no IN ({string.Join(",", part_list.Select(x => $@"'{x}'"))}))";
                        string color_str = DB.GetString(color_sql);
                        res["color_str"] = color_str;

                    }

                    ret.IsSuccess = true;
                    ret.RetData = JsonConvert.SerializeObject(res);
                }
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
        /// 产线列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetExLineList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "15";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";

                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";

                string where = "";

                if (!string.IsNullOrEmpty(keyword))
                {
                    where += $@" and (
                                        PRODUCTION_LINE_CODE like '%{keyword}%' or 
                                        COUNTRY like '%{keyword}%' or
                                        REGION like '%{keyword}%' or
                                        PLANT_AREA like '%{keyword}%' or
                                        DEPARTMENT like '%{keyword}%' or
                                        PRODUCTION_LINE_NAME like '%{keyword}%' or
                                        REMARKS like '%{keyword}%'
                                        )";
                }
                #endregion

                #region 逻辑
                string sql = $@"
SELECT * from (
SELECT
	PRODUCTION_LINE_CODE, -- 产线代号
	COUNTRY, -- 国家
REGION, -- 地区
PLANT_AREA,-- 厂区
DEPARTMENT, -- 部门
PRODUCTION_LINE_NAME,-- 产线
REMARKS-- 备注
FROM
	BDM_PRODUCTION_LINE_M
UNION all
SELECT
	a.DEPARTMENT_CODE AS PRODUCTION_LINE_CODE,
	'' as COUNTRY,
	'' as REGION,
	b.ORG as PLANT_AREA,
		a.UDF07 AS DEPARTMENT,
	a.DEPARTMENT_NAME AS PRODUCTION_LINE_NAME,
	'' as REMARKS
FROM
	base005m a
LEFT JOIN SJQDMS_ORGINFO b ON a.UDF05 = b.CODE
)tab WHERE 1=1 {where}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                ret.IsSuccess = true;

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
        /// 产线打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetPrintLineList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                #region 参数
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                List<string> list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(jarr["line"].ToString());

                string where = "";

                if (list.Count > 0)
                {
                    where += $" and PRODUCTION_LINE_CODE in ({string.Join(',', list.Select(x => $"'{x}'"))})";
                }
                #endregion

                #region 逻辑
                string sql = $@"
SELECT * from (
SELECT
	PRODUCTION_LINE_CODE, -- 产线代号
	COUNTRY, -- 国家
REGION, -- 地区
PLANT_AREA,-- 厂区
DEPARTMENT, -- 部门
PRODUCTION_LINE_NAME,-- 产线
REMARKS-- 备注
FROM
	BDM_PRODUCTION_LINE_M
UNION all
SELECT
	a.DEPARTMENT_CODE AS PRODUCTION_LINE_CODE,
	'' as COUNTRY,
	'' as REGION,
	b.ORG as PLANT_AREA,
		a.UDF07 AS DEPARTMENT,
	a.DEPARTMENT_NAME AS PRODUCTION_LINE_NAME,
	'' as REMARKS
FROM
	base005m a
LEFT JOIN SJQDMS_ORGINFO b ON a.UDF05 = b.CODE
)tab WHERE 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    //ret.RetData1 = dt;
                    ret.IsSuccess = true;
                }
                else
                {
                    ret.ErrMsg = "No data found！";
                    ret.IsSuccess = true;
                }


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
        /// 文件查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetMainList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString() : "";
                string start_date = jarr.ContainsKey("start_date") ? jarr["start_date"].ToString() : "";
                string end_time = jarr.ContainsKey("end_time") ? jarr["end_time"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "1";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "10";

                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                string where = string.Empty;
                if (!string.IsNullOrWhiteSpace(FILE_NAME))
                {

                    where += " and FILE_NAME like @FILE_NAME";
                    paramTestDic.Add("FILE_NAME", $@"%{FILE_NAME}%");
                    //paramTestDic.Add("CURR_UPLOAD_TIME", $@"%{keyword}%");
                }
                if (!string.IsNullOrWhiteSpace(start_date) || !string.IsNullOrWhiteSpace(end_time))
                {

                    where += $@" and TO_CHAR( CURR_UPLOAD_TIME ,'yyyy/MM/dd HH24:mi:ss') BETWEEN @start_date AND @end_time";
                    paramTestDic.Add("start_date ", $@"{start_date} 00:00:00");
                    paramTestDic.Add("end_time ", $@"{end_time} 23:59:59");
                }
                string sql = $@"
SELECT
    ID,
	FILE_NAME,
    substr(FILE_NAME,1,instr(FILE_NAME,'.')-1),
	TO_CHAR( CURR_UPLOAD_TIME ,'yyyy/MM/dd HH24:mi:ss') CURR_UPLOAD_TIME,
    EFFECT
FROM
	AQL_APP_T_FILE_M   
where 1=1 
{where} 
order by ID desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", paramTestDic);
                int total = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 文件提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_Main(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //var diclist = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarr["file"].ToString());
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";
                string file_starttime = jarr.ContainsKey("time") ? jarr["time"].ToString() : "";//开始日期
                string effect = jarr.ContainsKey("effect") ? jarr["effect"].ToString() : "0";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");

                if (string.IsNullOrEmpty(file_starttime))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Please select a start time！";
                    return ret;
                }


                var paramsDic = new Dictionary<string, object>();
                string sql = string.Empty;
                sql = $@"SELECT COUNT(1) FROM AQL_APP_T_FILE_M WHERE FILE_NAME=@file_name";
                paramsDic.Add("file_name", file_name);
                string exist = DB.GetStringline(sql, paramsDic);
                paramsDic = new Dictionary<string, object>();
                if (Convert.ToInt32(exist) > 0)
                {
                    sql = $@"UPDATE AQL_APP_T_FILE_M SET  CURR_UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),CURR_FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                    paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramsDic.Add("file_guid", file_guid);
                    paramsDic.Add("modifyby", user);
                    paramsDic.Add("modifydate", date);
                    paramsDic.Add("modifytime", time);
                    paramsDic.Add("file_name", file_name);

                    DB.ExecuteNonQuery(sql, paramsDic);
                }
                else
                {
                    sql = $@"INSERT INTO AQL_APP_T_FILE_M(FILE_NAME,CURR_UPLOAD_TIME,CURR_FILE_GUID,CREATEBY,CREATEDATE,CREATETIME,EFFECT,FILE_STARTTIME) VALUES(@file_name,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@file_guid,@createby,@createdate,@createtime,@effect,to_date(@FILE_STARTTIME, 'yyyy-MM-dd HH24:mi:ss'))";
                    paramsDic.Add("file_name", file_name);
                    paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                    paramsDic.Add("file_guid", file_guid);
                    paramsDic.Add("createby", user);
                    paramsDic.Add("createdate", date);
                    paramsDic.Add("createtime", time);
                    paramsDic.Add("effect", effect);
                    paramsDic.Add("FILE_STARTTIME", file_starttime);
                    DB.ExecuteNonQuery(sql, paramsDic);
                }
                paramsDic = new Dictionary<string, object>();
                sql = $"INSERT INTO AQL_APP_T_FILE_D(FILE_NAME,UPLOAD_TIME,FILE_GUID,CREATEBY,CREATEDATE,CREATETIME) VALUES(@file_name,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@file_guid,@createby,@createdate,@createtime)";
                paramsDic.Add("file_name", file_name);
                paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                paramsDic.Add("file_guid", file_guid);
                paramsDic.Add("createby", user);
                paramsDic.Add("createdate", date);
                paramsDic.Add("createtime", time);
                DB.ExecuteNonQuery(sql, paramsDic);



                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submit failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 查看单个的 提交明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_ListFile(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                string sql = $@"
SELECT 
	a.ID,
	a.FILE_NAME,
	a.upload_time,
	a.file_guid,
	b.file_url,
	b.file_name
FROM
	AQL_APP_T_FILE_D a
INNER JOIN bdm_upload_file_item b ON a.file_guid = b.guid
WHERE
	a.FILE_NAME = @file_name 
ORDER BY a.UPLOAD_TIME DESC";
                DataTable dt = DB.GetDataTable(sql, paramsDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// CS主界面删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Delete(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                string sql = $@"delete from AQL_APP_T_FILE_M where FILE_NAME=@file_name";
                DB.ExecuteNonQuery(sql, paramsDic);
                sql = $@"delete from AQL_APP_T_FILE_D where FILE_NAME=@file_name";
                DB.ExecuteNonQuery(sql, paramsDic);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Delete failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }



        /// <summary>
        /// app2文件修改为生效
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Update_EFFECT(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                //EFFECT
                string sql = $@"UPDATE AQL_APP_T_FILE_M SET EFFECT='0' where ID={id}";
                DB.ExecuteNonQuery(sql);
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Effective successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Validation failed, reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// app2批量生效
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Update_EFFECT_Batch(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ids = jarr.ContainsKey("ids") ? jarr["ids"].ToString() : "";
                List<string> ids_list = ids.Split(',').ToList();
                foreach (var item in ids_list)
                {
                    //EFFECT
                    string sql = $@"UPDATE AQL_APP_T_FILE_M SET EFFECT='0' where ID={item}";
                    DB.ExecuteNonQuery(sql);
                }
                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Batch validation succeeded";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Batch validation failed, the reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        /// <summary>
        /// 根据id删除明细
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteByDId(object OBJ) 
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
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("id", id);
                string sql = $@"delete from AQL_APP_T_FILE_D where id=@id";
                DB.ExecuteNonQuery(sql, paramsDic);

                paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                sql = $@"
SELECT
	a .upload_time,
	a .file_guid
FROM
	AQL_APP_T_FILE_D a
WHERE
	a.FILE_NAME = '{file_name}' 
ORDER BY a.UPLOAD_TIME DESC
";
                DataTable dt = DB.GetDataTablebyline(sql);
                paramsDic = new Dictionary<string, object>();
                if (dt.Rows.Count > 0)
                {
                    paramsDic.Add("currTime", dt.Rows[0]["UPLOAD_TIME"].ToString());
                    paramsDic.Add("file_guid", dt.Rows[0]["FILE_GUID"].ToString());
                    paramsDic.Add("modifyby", user);
                    paramsDic.Add("modifydate", date);
                    paramsDic.Add("modifytime", time);
                    paramsDic.Add("file_name", file_name);
                    sql = $@"UPDATE AQL_APP_T_FILE_M SET CURR_UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),CURR_FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                }
                else
                {
                    paramsDic.Add("file_name", file_name);
                    sql = $@"delete from AQL_APP_T_FILE_M where FILE_NAME=@file_name";
                }
                DB.ExecuteNonQuery(sql, paramsDic);
                DB.Commit();

                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteByDId2(object OBJ)
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
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string id = jarr.ContainsKey("id") ? jarr["id"].ToString() : "";
                string guid = jarr.ContainsKey("guid") ? jarr["guid"].ToString() : "";

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");

                var paramsDic = new Dictionary<string, object>();
                paramsDic.Add("guid", guid);
                string sql = $@"delete from BDM_UPLOAD_FILE_ITEM where guid=@guid";
                DB.ExecuteNonQuery(sql, paramsDic);

                paramsDic = new Dictionary<string, object>();
                paramsDic.Add("file_name", file_name);
                sql = $@"
SELECT
	a .upload_time,
	a .file_guid
FROM
	QCM_EX_TASK_LIST_M_F a
WHERE
	a.id = '{id}' 
";
                DataTable dt = DB.GetDataTablebyline(sql);
                paramsDic = new Dictionary<string, object>();
                //if (dt.Rows.Count > 0)
                //{
                //    paramsDic.Add("currTime", dt.Rows[0]["UPLOAD_TIME"].ToString());
                //    paramsDic.Add("file_guid", dt.Rows[0]["FILE_GUID"].ToString());
                //    paramsDic.Add("modifyby", user);
                //    paramsDic.Add("modifydate", date);
                //    paramsDic.Add("modifytime", time);
                //    paramsDic.Add("file_name", file_name);
                //    sql = $@"UPDATE QCM_EX_TASK_LIST_M_F SET UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                //}
                //else
                ////{
                paramsDic.Add("id", id);
                sql = $@"delete from QCM_EX_TASK_LIST_M_F where id=@id";
                //}
                DB.ExecuteNonQuery(sql, paramsDic);
                DB.Commit();

                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
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
        /// 送测文件提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Commit_MainSc(object OBJ)
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
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                //var diclist = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarr["file"].ToString());
                string file_name = jarr.ContainsKey("file_name") ? jarr["file_name"].ToString() : "";
                string file_guid = jarr.ContainsKey("file_guid") ? jarr["file_guid"].ToString() : "";

                string task_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";
                string sclx = jarr.ContainsKey("sclx") ? jarr["sclx"].ToString() : "";
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                DateTime currTime = DateTime.Now;
                string date = currTime.ToString("yyyy-MM-dd");
                string time = currTime.ToString("HH:mm:ss");



                var paramsDic = new Dictionary<string, object>();
                string sql = string.Empty;
                //sql = $@"SELECT COUNT(1) FROM QCM_EX_TASK_LIST_M_F WHERE FILE_NAME=@file_name";
                //paramsDic.Add("file_name", file_name);
                //string exist = DB.GetStringline(sql, paramsDic);
                paramsDic = new Dictionary<string, object>();
                //if (Convert.ToInt32(exist) > 0)
                //{
                //    sql = $@"UPDATE QCM_EX_TASK_LIST_M_F SET CURR_UPLOAD_TIME=TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),CURR_FILE_GUID=@file_guid,MODIFYBY=@modifyby,MODIFYDATE=@modifydate,MODIFYTIME=@modifytime WHERE FILE_NAME=@file_name";
                //    paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));

                //    paramsDic.Add("file_guid", file_guid);
                //    paramsDic.Add("task_no", task_no);
                //    paramsDic.Add("sclx", sclx);
                //    paramsDic.Add("art", art);

                //    paramsDic.Add("modifyby", user);
                //    paramsDic.Add("modifydate", date);
                //    paramsDic.Add("modifytime", time);
                //    paramsDic.Add("file_name", file_name);
                //    DB.ExecuteNonQuery(sql, paramsDic);
                //}
                //else
                //{
                sql = $@"INSERT INTO QCM_EX_TASK_LIST_M_F(TASK_NO,FILE_NAME,ART_NO,UPLOAD_TIME,TEST_TYPE,FILE_GUID,CREATEBY,CREATEDATE,CREATETIME) 
                                                  VALUES(@TASK_NO,@FILE_NAME,@ART_NO,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@TEST_TYPE,@FILE_GUID,@createby,@createdate,@createtime)";

                paramsDic.Add("TASK_NO", task_no);
                paramsDic.Add("FILE_NAME", file_name);
                paramsDic.Add("ART_NO", art);
                paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                paramsDic.Add("TEST_TYPE", sclx);
                paramsDic.Add("FILE_GUID", file_guid);
                paramsDic.Add("createby", user);
                paramsDic.Add("createdate", date);
                paramsDic.Add("createtime", time);
                DB.ExecuteNonQuery(sql, paramsDic);
                //}
                //paramsDic = new Dictionary<string, object>();
                //sql = $"INSERT INTO AQL_APP_T_FILE_D(FILE_NAME,UPLOAD_TIME,FILE_GUID,CREATEBY,CREATEDATE,CREATETIME) VALUES(@file_name,TO_DATE(@currTime, 'yyyy-MM-dd HH24:mi:ss'),@file_guid,@createby,@createdate,@createtime)";
                //paramsDic.Add("file_name", file_name);
                //paramsDic.Add("currTime", currTime.ToString("yyyy-MM-dd HH:mm:ss"));
                //paramsDic.Add("file_guid", file_guid);
                //paramsDic.Add("createby", user);
                //paramsDic.Add("createdate", date);
                //paramsDic.Add("createtime", time);
                //DB.ExecuteNonQuery(sql, paramsDic);



                DB.Commit();
                ret.IsSuccess = true;
                ret.ErrMsg = "Submitted successfully";
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Submit failed, reason：" + ex.Message; 
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_ListFileSc(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TASK_NO = jarr.ContainsKey("TASK_NO") ? jarr["TASK_NO"].ToString() : "";
                string sclx = jarr.ContainsKey("sclx") ? jarr["sclx"].ToString() : "";
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";

                var paramsDic = new Dictionary<string, object>();
                //paramsDic.Add("ART_NO", art);
                paramsDic.Add("TEST_TYPE", sclx);
                paramsDic.Add("TASK_NO", TASK_NO);
                string sql = $@"
SELECT
    QCM_EX_TASK_LIST_M_F.ID,
	QCM_EX_TASK_LIST_M_F.FILE_NAME,
	BDM_UPLOAD_FILE_ITEM.FILE_URL as file_url,
	BDM_UPLOAD_FILE_ITEM.GUID as guid,
    	QCM_EX_TASK_LIST_M_F.UPLOAD_TIME
FROM
	QCM_EX_TASK_LIST_M_F
LEFT JOIN BDM_UPLOAD_FILE_ITEM ON QCM_EX_TASK_LIST_M_F.FILE_GUID = BDM_UPLOAD_FILE_ITEM.GUID
WHERE 1=1
--AND QCM_EX_TASK_LIST_M_F.ART_NO = @ART_NO
AND QCM_EX_TASK_LIST_M_F.TEST_TYPE =@TEST_TYPE
AND QCM_EX_TASK_LIST_M_F.TASK_NO = @TASK_NO";
                DataTable dt = DB.GetDataTable(sql, paramsDic);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Check_Duplicate(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string art_no = jarr.ContainsKey("art_no") ? jarr["art_no"].ToString() : "";
                string order_po = jarr.ContainsKey("order_po") ? jarr["order_po"].ToString() : "";
                string test_type = jarr.ContainsKey("test_type") ? jarr["test_type"].ToString() : "";
                string size = jarr.ContainsKey("size") ? jarr["size"].ToString() : "";
                string line_name = jarr.ContainsKey("line_name") ? jarr["line_name"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                order_po = order_po.Replace("&", "_");

                string sql = $@"select CREATEDATE, ART_NO, SIZES, ORDER_PO, LINE_NAME, count(*)
  from QCM_EX_TASK_LIST_M
 where TEST_TYPE = '{test_type}'
   and CREATEDATE = '{date}'
   and ART_NO = '{art_no}'
   and ORDER_PO like '{order_po}'
   and LINE_NAME = '{line_name}'
 group by CREATEDATE, ART_NO, SIZES, ORDER_PO, LINE_NAME
having count(*) >= 1
 ";
                DataTable dt = DB.GetDataTable(sql);
                ret.IsSuccess = dt.Rows.Count>0;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }
            return ret;
        }
    }
}
