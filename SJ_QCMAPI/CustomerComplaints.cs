using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class CustomerComplaints
    {
        /// <summary>
        /// 查询客户投诉
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitQCM_CUSTOMER_COMPLAINT_M(object OBJ)
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
                string where = string.Empty;
                string sql = string.Empty;

                if (!string.IsNullOrEmpty(TestCode))
                {
                    where += $@" and COMPLAINT_NO like '%{TestCode}%' or COMPLAINT_NO like '%{TestCode}%' or COUNTRY_REGION like '%{TestCode}%' or 
                                  PO_ORDER like '%{TestCode}%'";
                }
                sql = $@"SELECT
	                    COMPLAINT_NO,
	                    COMPLAINT_DATE,
	                    COUNTRY_REGION,
	                    PO_ORDER,
                        NG_QTY,
                        COMPLAINT_MONEY
                    FROM
	                    QCM_CUSTOMER_COMPLAINT_M where 1=1 {where}";
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
        /// 点击选择查询数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ClickSelect(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString() : "";//投诉单号
                Dictionary<string, object> dic = new Dictionary<string, object>();

                DataTable dt1 = DB.GetDataTable($@"SELECT
	                                                COMPLAINT_NO,
	                                                COMPLAINT_DATE,
	                                                COUNTRY_REGION,
	                                                PO_ORDER,
	                                                NG_QTY,
	                                                COMPLAINT_MONEY,
                                                    DEFECT_CONTENT，
                                                    DEVELOP_SEASON,
                                                    CATEGORY,
                                                    DEVELOPMENT_COURSE,
                                                    PRODUCT_MONTH,
                                                    PLANTAREA_NO,
                                                    PLANTAREA_NAME,
                                                    PRODUCTIONLINE_NO,
                                                    PRODUCTIONLINE_NAME,
                                                    SHOE_NO,
                                                    MATERIAL_WAY,
                                                    PROD_NO
                                                FROM
	                                                QCM_CUSTOMER_COMPLAINT_M where COMPLAINT_NO='{COMPLAINT_NO}'");

                DataTable dt2 = DB.GetDataTable($@"select IMG_NAME,IMG_URL,TYPE from QCM_CUSTOMER_COMPLAINT_FILE where COMPLAINT_NO='{COMPLAINT_NO}'");

                DataTable dt3 = DB.GetDataTable($@"select PROCES_DATE,CAUSE_ANALYSIS,RESPONSIBILITY_JUDGMENT,IMPROVEMENT_ACTION,CONCLUSION,'1' as Dlogo from QCM_CUSTOMER_COMPLAINT_D where COMPLAINT_NO='{COMPLAINT_NO}'");
                dic.Add("Data",dt1);
                dic.Add("ImgList", dt2);
                dic.Add("manage", dt3);
                #endregion
                ret.RetData1 = dic;
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
        /// 提交数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubmitQCM_CUSTOMER_COMPLAINT_D(object OBJ)
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
                string COMPLAINT_NO = jarr.ContainsKey("COMPLAINT_NO") ? jarr["COMPLAINT_NO"].ToString():"";
                DataTable TableBody = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TableBody"].ToString());//表身数据

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                foreach (DataRow item in TableBody.Rows)
                {
                    DB.ExecuteNonQuery($@"insert into QCM_CUSTOMER_COMPLAINT_D (COMPLAINT_NO,PROCES_DATE,CAUSE_ANALYSIS,RESPONSIBILITY_JUDGMENT,
                                           IMPROVEMENT_ACTION,CONCLUSION,CREATEBY,CREATEDATE,CREATETIME ) 
                                          values('{COMPLAINT_NO}','{item["PROCES_DATE"]}','{item["CAUSE_ANALYSIS"]}','{item["RESPONSIBILITY_JUDGMENT"]}',
                                           '{item["IMPROVEMENT_ACTION"]}','{item["CONCLUSION"]}','{user}','{date}','{time}' )");
                }

                #endregion
                //ret.RetData1 = dic;
                ret.IsSuccess = true;
                DB.Commit();//提交事务
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
    }
}
