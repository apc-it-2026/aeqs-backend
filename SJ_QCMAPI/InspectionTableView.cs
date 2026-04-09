using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class InspectionTableView
    {
        /// <summary>
        /// 实验室送检主页数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetQCM_INSPECTION_LABORATORY_D_List(object OBJ)
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

                //下拉框id更新Table值
                string general_testtype_no = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                string quality_categoryno_or_secondary_categoryno = jarr.ContainsKey("data1") ? jarr["data1"].ToString() : "";
                if (!string.IsNullOrEmpty(quality_categoryno_or_secondary_categoryno))
                 {
                    string sql = $@"SELECT
	a.testtype_name,
	a.testitem_code,
	a.testitem_name,
	a.check_item as check_item_1,
	a.check_value as check_value_1,
	a.check_item as check_item_2,
	a.check_value as check_value_2,
	a.unit,
    a.check_type,
	a.REFERENCE_LEVEL as enum_value_1,
	e.enum_value as enum_value_2,
	a.sample_num,
	a.currency_formula,
	b.ENUM_VALUE as formula_name_1,
	a.custom_formula,
	c.formula_name as formula_name_2,
	BDM_TESTITEM_M.remarks as remarks
FROM
    bdm_qualitytest_item a
		left join SYS001M b ON a.currency_formula = b.ENUM_CODE 
		and b.ENUM_TYPE = 'enum_general_formula'
    LEFT JOIN bdm_formula_m c ON a.custom_formula = c.formula_code
    AND c.FORMULA_TYPE = '1'
    LEFT JOIN sys001m d ON a.reference_level = d.ENUM_CODE
    AND d.ENUM_TYPE = 'enum_formula_type'
    LEFT JOIN sys001m e ON a.reference_level = d.ENUM_CODE
    AND d.ENUM_TYPE = 'enum_ref_level'
	left join BDM_TESTITEM_M on a.testitem_code=BDM_TESTITEM_M.TESTITEM_CODE
    where general_testtype_no = '{general_testtype_no}' and(quality_category_no ='{quality_categoryno_or_secondary_categoryno}' or  secondary_category_no= '{quality_categoryno_or_secondary_categoryno }')";
                      DataTable dt = DB.GetDataTable(sql);
                     ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

                     ret.IsSuccess = true;
                 }
                 else
                 {
                     throw new Exception("选定下拉框没有：【\"内容\"】，请检查!");
                 }
            }
            catch (Exception ex)
            {
                //DB.Rollback();
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
        /// 实验室送检扫描条码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_RD_ITEM_Select_item_no(object OBJ)
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
                //物料条码
                string item_no = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //转译
                //先找料品bom
                string PROD_NO =DB.GetString($@"select PROD_NO from BDM_RD_BOM_ITEM WHERE rownum=1 and SUBMATERIAL_NO='{item_no.Trim()}'");
                DataTable dt2 = null;
                if (!string.IsNullOrEmpty(PROD_NO))
                {
                    //如果根据条件有内容直接输出datatable
                    string sql2 = $@"select ITEM_NO,NAME_S,PARENT_ITEM_NO from BDM_RD_ITEM WHERE rownum=1 and ITEM_NO='{PROD_NO}'";
                    dt2 = DB.GetDataTable(sql2);
                }
                else
                {
                    //2表
                    //如果没有就找料品信息表直接返回datatable
                    string sql2 = $@"select ITEM_NO,NAME_S,PARENT_ITEM_NO from BDM_RD_ITEM WHERE rownum=1 and ITEM_NO='{item_no.Trim()}'";
                    dt2 = DB.GetDataTable(sql2);
                }
                dic.Add("data", dt2);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

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
        /// 扫描员工二维码
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject BDM_RD_ITEM_Select_Hr001m(object OBJ)
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

                string txt_User = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                //转译
                string sql = $@"select STAFF_NO as 账号,STAFF_NAME as 名称,staff_department as 部门 from hr001m where STAFF_NO='{txt_User}'";
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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
        /// 查询上传的文件
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GET_PROD_File_List(object OBJ)
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
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                //转译
                string sql = $@"select FILE_URL,FILE_NAME from bdm_prod_customquality_file 
                where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}' and category_no='{category_no}'  and testitem_code='{testitem_code}'";
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData1 = dt;
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
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetSYS001MDataList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(Data);

                if (jarr.Count == 0)
                    throw new Exception("接口参数不能为空，请检查！");

                Dictionary<string, object> dic = new Dictionary<string, object>();

                foreach (var item in jarr)
                {
                    string sql = $@"select enum_code,enum_value from sys001m where enum_type ='{item}' ";
                    DataTable dt = DB.GetDataTable(sql);
                    dic.Add(item, dt);
                }


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
