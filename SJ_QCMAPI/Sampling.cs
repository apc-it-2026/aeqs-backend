using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class Sampling
    {
        /// <summary>
        /// 查询抽检主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Gitqcm_spotcheck_task_m(object OBJ)
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
                //转译
                string TestCode = jarr.ContainsKey("TestCode") ? jarr["TestCode"].ToString() : "";//查询条件
                string State = jarr.ContainsKey("State") ? jarr["State"].ToString() : "";//查询条件状态
                string where = string.Empty;
                string sql = string.Empty;

                if (!string.IsNullOrEmpty(TestCode))
                {
                    where += $@" and SPOTCHECK_NO like '%{TestCode}%' or PO_ORDER like '%{TestCode}%' or VEND_NAME like '%{TestCode}%' or
                                 SHOE_NOS like '%{TestCode}%' or PROD_NO like '%{TestCode}%' or PART_NO like '%{TestCode}%' or
                                 PROCESS_TYPE like '%{TestCode}%' or INSPECT_METHOD like '%{TestCode}%'";
                }
                if (!string.IsNullOrEmpty(State))
                {
                    where += $@" and STATUS='{State}'";
                }
                sql = $@"SELECT
	                    SPOTCHECK_NO,
	                    INSPECT_METHOD,
	                    VEND_NO,
	                    VEND_NAME,
	                    PART_NO,
	                    SHOE_NOS,
	                    PROD_NO,
	                    PO_ORDER,
	                    CODE_NUMBER,
	                    SPOTCHECK_DATE,
	                    PO_QTY,
	                    PLANSAMP_QTY,
	                    PROCESS_TYPE,
	                    BAD_QTY,
	                    NG_QTY,
	                    RFT,
	                    IS_FIRSTCONFIRM,
	                    IS_SENDTEST,
	                    IS_QUALITYAUDIT,
	                    (CASE STATUS
                      when '1' THEN '未完成'
	                    when '2' THEN '完成'
	                    ELSE ''
                      END )as STATUS
                    FROM
	                    qcm_spotcheck_task_m where 1=1 {where}";
                DataTable dt = DB.GetDataTable(sql);
                #endregion
                ret.RetData1 = dt;
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
        /// 查询通用标准和检测类型
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitUniversalStandard(object OBJ)
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
                //转译
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? (jarr["PROD_NO"] == null ? "" : jarr["PROD_NO"].ToString()) : "";//art编号
                string GTNO = jarr.ContainsKey("GTNO") ? jarr["GTNO"].ToString() : "";//通用检测类型
                string where = string.Empty;
                string sql = string.Empty;
                if (string.IsNullOrEmpty(PROD_NO))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "ART编号不能为空!";
                    return ret;
                }
                if (string.IsNullOrEmpty(GTNO))
                {
                    sql = $@"select general_testtype_no,general_testtype_name from bdm_prod_customquality_m where prod_no='{PROD_NO}'";
                }
                if (!string.IsNullOrEmpty(GTNO) && !string.IsNullOrEmpty(PROD_NO))
                {
                    sql = $@"select category_no,category_name from bdm_prod_customquality_d where prod_no='{PROD_NO}' and general_testtype_no='{GTNO}'";
                }

                DataTable dt = DB.GetDataTable(sql);
                #endregion
                ret.RetData1 = dt;
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
        /// 通过ART、通用标准和检测类型查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Gitbdm_prod_customquality_item(object OBJ)
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
                //转译
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";//art编号
                string GTNO = jarr.ContainsKey("GTNO") ? jarr["GTNO"].ToString() : "";//通用检测类型代号
                string CNO = jarr.ContainsKey("CNO") ? jarr["CNO"].ToString() : "";//类别代号
                string PLANSAMP_QTY = jarr.ContainsKey("PLANSAMP_QTY") ? jarr["PLANSAMP_QTY"].ToString() : "";//抽检数量
                string where = string.Empty;
                string sql = string.Empty;
                where = $@" and prod_no='{PROD_NO}' and general_testtype_no='{GTNO}' and category_no='{CNO}'";
                sql = $@"SELECT
	                    testitem_code,
	                    testitem_name,
	                    (CASE NVL(d_check_value,'1')
                      when '1' THEN t_check_value
                      ELSE d_check_value
                      END )as check_value,
	                    sample_num,
	                    AQL_LEVEL,
	                    '0' AC,
	                    '1' RE
                    FROM
	                    bdm_prod_customquality_item where testitem_category='2' {where}";
                DataTable dt = DB.GetDataTable(sql);

                int AC = 0;
                double RE = AC + 1;
                if (dt.Rows.Count > 0)
                {
                    DataTable a = DB.GetDataTable($@"select * from BDM_AQL_M where  ('{PLANSAMP_QTY}' BETWEEN  START_QTY AND END_QTY)");
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(item["AQL_LEVEL"].ToString()))
                        {
                            if (a.Rows.Count > 0)
                            {
                                AC = GetACRE(item["AQL_LEVEL"].ToString(), a);
                            }

                            item["AC"] = AC;
                            item["RE"] = AC + 1;

                        }
                    }
                }
                #endregion
                ret.RetData1 = dt;
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

        public static int GetACRE(string AQL_LEVEL, DataTable a)
        {
            int AC = 0;
            switch (AQL_LEVEL)
            {
                case enum_aql_level.enum_barcode_print_type_0:
                    AC = Convert.ToInt32(a.Rows[0]["AC01"].ToString());
                    break;
                case enum_aql_level.enum_barcode_print_type_1:
                    AC = Convert.ToInt32(a.Rows[0]["AC02"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_2:
                    AC = Convert.ToInt32(a.Rows[0]["AC03"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_3:
                    AC = Convert.ToInt32(a.Rows[0]["AC04"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_4:
                    AC = Convert.ToInt32(a.Rows[0]["AC05"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_5:
                    AC = Convert.ToInt32(a.Rows[0]["AC06"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_6:
                    AC = Convert.ToInt32(a.Rows[0]["AC07"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_7:
                    AC = Convert.ToInt32(a.Rows[0]["AC08"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_8:
                    AC = Convert.ToInt32(a.Rows[0]["AC09"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_9:
                    AC = Convert.ToInt32(a.Rows[0]["AC10"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_10:
                    AC = Convert.ToInt32(a.Rows[0]["AC11"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_11:
                    AC = Convert.ToInt32(a.Rows[0]["AC12"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_12:
                    AC = Convert.ToInt32(a.Rows[0]["AC13"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_13:
                    AC = Convert.ToInt32(a.Rows[0]["AC14"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_14:
                    AC = Convert.ToInt32(a.Rows[0]["AC15"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_15:
                    AC = Convert.ToInt32(a.Rows[0]["AC16"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_16:
                    AC = Convert.ToInt32(a.Rows[0]["AC17"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_17:
                    AC = Convert.ToInt32(a.Rows[0]["AC18"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_18:
                    AC = Convert.ToInt32(a.Rows[0]["AC19"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_19:
                    AC = Convert.ToInt32(a.Rows[0]["AC20"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_20:
                    AC = Convert.ToInt32(a.Rows[0]["AC21"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_21:
                    AC = Convert.ToInt32(a.Rows[0]["AC22"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_22:
                    AC = Convert.ToInt32(a.Rows[0]["AC23"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_23:
                    AC = Convert.ToInt32(a.Rows[0]["AC24"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_24:
                    AC = Convert.ToInt32(a.Rows[0]["AC25"].ToString());

                    break;
                case enum_aql_level.enum_barcode_print_type_25:
                    AC = Convert.ToInt32(a.Rows[0]["AC26"].ToString());

                    break;
                default:
                    AC = 0;
                    break;
            }
            return AC;

        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Submitqcm_spotcheck_task_d(object OBJ)
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
                string IS_FIRSTCONFIRM = jarr.ContainsKey("IS_FIRSTCONFIRM") ? jarr["IS_FIRSTCONFIRM"].ToString() : "";//首检确认否
                string IS_SENDTEST = jarr.ContainsKey("IS_SENDTEST") ? jarr["IS_SENDTEST"].ToString() : "";//送测否
                string IS_QUALITYAUDIT = jarr.ContainsKey("IS_QUALITYAUDIT") ? jarr["IS_QUALITYAUDIT"].ToString() : "";//品控审核否
                string BAD_QTY = jarr.ContainsKey("BAD_QTY") ? jarr["BAD_QTY"].ToString() : "";//报废数
                string NG_QTY = jarr.ContainsKey("NG_QTY") ? jarr["NG_QTY"].ToString() : "";//不良数
                string RFT = jarr.ContainsKey("RFT") ? jarr["RFT"].ToString() : "";//RFT

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                string SPOTCHECK_NO = jarr.ContainsKey("SPOTCHECK_NO") ? jarr["SPOTCHECK_NO"].ToString() : "";//检验单号

                List<Dictionary<string, object>> lst_TableBody = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarr["TableBody"].ToString());//返回数据






                DB.ExecuteNonQuery($@"update qcm_spotcheck_task_m set BAD_QTY='{BAD_QTY}',NG_QTY='{NG_QTY}',RFT='{RFT}',IS_FIRSTCONFIRM='{IS_FIRSTCONFIRM}',IS_SENDTEST='{IS_SENDTEST}',IS_QUALITYAUDIT='{IS_QUALITYAUDIT}',STATUS='2' 
                                       where SPOTCHECK_NO='{SPOTCHECK_NO}' ");

                string TESTITEM_CODE = string.Empty;
                foreach (Dictionary<string, object> item in lst_TableBody)
                {


                    DataTable ImgList = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(item["imageList"].ToString());//图片集合

                    TESTITEM_CODE = item["TESTITEM_CODE"].ToString();
                    double SAMPLE_NUM = item["SAMPLE_NUM"] == null ? 0 : Convert.ToDouble(item["SAMPLE_NUM"].ToString());
                    string AQL_LEVEL = item["AQL_LEVEL"] == null ? "" : item["AQL_LEVEL"].ToString();
                    string AC = item["AC"] == null ? "" : item["AC"].ToString();
                    string RE = item["RE"] == null ? "" : item["RE"].ToString();
                    double BAD_QTY2 = item["BAD_QTY"] == null ? 0 : Convert.ToDouble(item["BAD_QTY"].ToString());
                    double NG_QTY2 = item["NG_QTY"] == null ? 0 : Convert.ToDouble(item["NG_QTY"].ToString());

                    string sql = $@"insert into qcm_spotcheck_task_d (SPOTCHECK_NO,TESTITEM_CODE,TESTITEM_NAME,TEST_STANDARD, 
                                            TEST_QTY,AQL_LEVEL,AC,RE,CHECK_RESULT,REMARKS,BAD_QTY,NG_QTY,DEFECT_CONTENT,IMPROVEMENT_MODE,
                                            CREATEBY,CREATEDATE,CREATETIME) 
                                          values('{SPOTCHECK_NO}','{TESTITEM_CODE}','{item["TESTITEM_NAME"]}','{item["CHECK_VALUE"]}', 
                                            '{SAMPLE_NUM}','{AQL_LEVEL}','{AC}','{RE}','{item["CHECK_RESULT"]}',
                                            '{item["REMARKS"]}','{BAD_QTY2}','{NG_QTY2}','{item["DEFECT_CONTENT"]}','{item["IMPROVEMENT_MODE"]}',
                                            '{user}','{date}','{time}')";
                    DB.ExecuteNonQuery(sql);
                    foreach (DataRow item2 in ImgList.Rows)
                    {
                        DB.ExecuteNonQuery($@"insert into qcm_spotcheck_task_image (SPOTCHECK_NO,TESTITEM_CODE,img_name,img_url,guid,CREATEBY,CREATEDATE,CREATETIME) 
                                          values('{SPOTCHECK_NO}','{TESTITEM_CODE}','{item2["img_name"]}','{item2["img_url"]}','{Guid.NewGuid().ToString("N")}','{user}','{date}','{time}')");
                    }
                }
                #endregion
                //ret.RetData1 = dt;
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

    }
}
