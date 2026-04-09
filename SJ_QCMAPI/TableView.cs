using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
   public class TableView
    {
        /// <summary>
        /// 首页检查数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMList(object OBJ)
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
                //转译
                string testtype_no = jarr.ContainsKey("testtype_no") ? jarr["testtype_no"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";
                string testitem_name = jarr.ContainsKey("testitem_name") ? jarr["testitem_name"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                //取值
                var sql = string.Empty;

                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(testtype_no) )
                {
                    strwhere += $@" and (a.testtype_no like '%{testtype_no.Trim()}%' or a.testtype_name like '%{testtype_no.Trim()}%')";
                }
                if (!string.IsNullOrEmpty(testitem_code))
                {
                    strwhere += $@" and a.testitem_code like '%{testitem_code.Trim()}%' ";
                }
                if (!string.IsNullOrEmpty(testitem_name))
                {
                    strwhere += $@" and a.testitem_name like '%{testitem_name.Trim()}%'";
                }
                 
                sql = $@"SELECT 
 a.testtype_no，
	a.testtype_name，
	a.testitem_code,
	a.testitem_name,
    a.AQL_LEVEL,
	a.sample_num,
	c.enum_value as formula_name_1,
	e.formula_name as formula_name_2,
	b.enum_value as enum_value_1,
	a.remarks
 FROM
 BDM_TESTITEM_M a
 left JOIN SYS001M b ON a.reference_level = b.ENUM_CODE and b.ENUM_TYPE = 'enum_ref_level'
 left join SYS001M c ON a.currency_formula = c.ENUM_CODE and c.ENUM_TYPE = 'enum_general_formula'
 left join BDM_FORMULA_M e on a.CUSTOM_FORMULA = e.formula_code   
                        where 1=1 {strwhere} order by a.id desc";

                //DataTable dt = DB.GetDataTable(sql);
                //查询分页数据
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
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            } 
            return ret;
        }
        /// <summary>
        /// 修改前数据显示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMUpdatebyId(object OBJ)
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
                string sql = $@"select id as 序号,testtype_no as 测试类型,testitem_code as 检测项编号,reference_level as 结果引用级别,testitem_name as 检测项名称, unit as 单位,testitem_count as 检测标准数量,sample_num as 试样数量,currency_formula as 通用公式类型,custom_formula as 自定义公式类型,type as 类型,remarks as 备注,AQL_LEVEL as AQL级别,AC as AC值,RE as RE值 from BDM_TESTITEM_M where testitem_code='{id}'";

                DataTable dt = DB.GetDataTable(sql);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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
        /// 编辑公式测试视图
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBDM_TESTITEMAddList(object OBJ)
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

                string id = jarr.ContainsKey("data") ? jarr["data"].ToString() : "";
                string sql = string.Empty;
                string types =DB.GetString($"select type from BDM_TESTITEM_M where TESTITEM_CODE='{id}'");//1 2 3
                //转译
                if (types.Equals(enum_testitem_type.enum_testitem_type_1))
                {
                    sql = $@"select a.id,a.value as 标准测量,b.unit as 单位,a.remarks as 备注
                                FROM  BDM_TESTITEM_D a
                                left join BDM_TESTITEM_M b on a.testitem_code=b.testitem_code
                                where a.testitem_code='{id}' order by id desc";
                }
                else if(types.Equals(enum_testitem_type.enum_testitem_type_2))
                {
                    sql = $@"select a.id,CONCAT(CONCAT(a.MIN_VALUE,'~'), a.MAX_VALUE) as 标准测量,b.unit 单位,a.remarks as 备注 
                                from BDM_TESTITEM_D a
                                left join BDM_TESTITEM_M b on a.testitem_code=b.testitem_code
                                where a.testitem_code='{id}' order by id desc";
                }
                else if (types.Equals(enum_testitem_type.enum_testitem_type_3))
                {
                    sql = $@"select a.id,CONCAT(CONCAT(a.value,'±'), a.error_value) as 标准测量,b.unit 单位,a.remarks as 备注
                                from BDM_TESTITEM_D a
                                left join BDM_TESTITEM_M b on a.testitem_code=b.testitem_code
                                where a.testitem_code='{id}' order by id desc";
                }
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.IsSuccess = true;
                //DB.Commit();
            }
            catch (Exception ex)
            {
                //DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            //finally
            //{
            //    DB.Close();
            //}
            return ret;
        }

        /// <summary>
        /// 绑定公式下拉框
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFormulaData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);


                Dictionary<string, object> dic = new Dictionary<string, object>();

                string sql = $@"select formula_name,formula_code from bdm_formula_m";
                DataTable dt = DB.GetDataTable(sql);
                dic.Add("data", dt);
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
