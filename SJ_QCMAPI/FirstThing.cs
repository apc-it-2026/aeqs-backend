using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class FirstThing
    {
        /// <summary>
        /// 首件确认主页查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitHomepage(object OBJ)
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
                string State = jarr.ContainsKey("State") ? jarr["State"].ToString() : "";//查询状态
                string where = string.Empty;
                string sql = string.Empty;
                if (!string.IsNullOrEmpty(TestCode))
                {
                    where += $@" and INSPECT_NO like '%{TestCode}%' or PROD_NO like '%{TestCode}%' or SHOE_NO like '%{TestCode}%' or 
                                MODULE_NO like '%{TestCode}%' or PHYSICAL_NAME like '%{TestCode}%' or MACHINE like '%{TestCode}%' 
                                or CODE_NUMBER like '%{TestCode}%'";
                }
                if (!string.IsNullOrEmpty(State))
                {
                    where += $@" and STATUS='{State}'";
                }

                sql = $@"select INSPECT_NO,PROD_NO,SHOE_NO,MODULE_NO,PO_ORDER,PHYSICAL_NAME,MACHINE,CODE_NUMBER,STATUS from qcm_firstarticle_confirm_m where 1=1 {where}";

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
        /// 查询首件确认录入界面产线下拉框接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitProductionLine(object OBJ)
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
                string sql = $@"SELECT
	department_no || '@' || productionline_no AS dp_no,
	department_name || '@' || productionline_name AS dp_name 
FROM
	bdm_productionline_defects_m";

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
        /// 查询首件确认录入界面不良问题记录接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitBadProblems(object OBJ)
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
                string DP_NO = jarr.ContainsKey("DP_NO") ? jarr["DP_NO"].ToString() : "";//部门和产线代号
                string[] dp = DP_NO.Split('@');
                //转译
                string sql = $@"select defect_no,defect_name from bdm_productionline_defects_m where department_no='{dp[0]}' and productionline_no='{dp[1]}'";

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
        /// 首件确认录入界面首检未通过接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstInspection(object OBJ)
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
                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";//首检单号
                var defect = jarr.ContainsKey("defect") ? jarr["defect"].ToString() : "";//不良问题
                DataTable table = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(defect);

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间
                string inspect_seq = DB.GetString($"select max(inspect_seq) from qcm_firstarticle_confirm_d where inspect_no='{inspect_no}'");
                if (string.IsNullOrEmpty(inspect_seq))
                    inspect_seq = "1";
                else
                    inspect_seq = (Convert.ToInt32(inspect_seq) + 1).ToString();
                foreach (DataRow item in table.Rows)
                {
                    DB.ExecuteNonQuery($@"insert into qcm_firstarticle_confirm_d (inspect_no,inspect_seq,defect_no,defect_name,CREATEBY,CREATEDATE,CREATETIME) 
                                          values('{inspect_no}','{inspect_seq}','{item["defect_no"]}','{item["defect_name"]}','{user}','{date}','{time}')");
                }
                DB.ExecuteNonQuery($"update qcm_firstarticle_confirm_m set STATUS='FAIL',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where INSPECT_NO='{inspect_no}'");
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
        /// 首件确认录入界面首检通过接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstInspectionTrue(object OBJ)
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
                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";//首检单号

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                DB.ExecuteNonQuery($"update qcm_firstarticle_confirm_m set STATUS='PASS',MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}' where INSPECT_NO='{inspect_no}'");
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
        /// 首件确认录入界面首检记录查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject FirstInspectionRecord(object OBJ)
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
                string inspect_no = jarr.ContainsKey("inspect_no") ? jarr["inspect_no"].ToString() : "";//首检单号
                string where = string.Empty;
                string sql = string.Empty;


                sql = $@"	select 
	                        inspect_no,
	                        inspect_seq,
													defect_no,
													defect_name,
													(CREATEDATE || '-' || CREATETIME) as datetime
	                        from 
	                        qcm_firstarticle_confirm_d
		                        where inspect_no='{inspect_no}'
														ORDER BY inspect_seq desc";

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
        /// 首件确认录入界面查看报告查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ViewReport(object OBJ)
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
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString() : "";//ART
                string SHOE_NO = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string where = string.Empty;
                string sql = string.Empty;

                if (!string.IsNullOrWhiteSpace(ART) && !string.IsNullOrWhiteSpace(SHOE_NO))
                {
                    string DEVELOP_SEASON = DB.GetString($@"select DEVELOP_SEASON from bdm_rd_prod where PROD_NO='{ART}' and SHOE_NO='{SHOE_NO}'");
                    sql = $@"select file_name,file_url from qcm_qa_shoeshape_file where develop_season='{DEVELOP_SEASON}' and shoe_no='{SHOE_NO}'";
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
    }
}
