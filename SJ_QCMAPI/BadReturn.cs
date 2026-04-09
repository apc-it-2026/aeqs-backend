using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class BadReturn
    {
        /// <summary>
        /// 查询不良退货
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Gitqcm_bad_return_m(object OBJ)
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
                    where += $@" and RETURN_NO like '%{TestCode}%' or PLANT_AREA like '%{TestCode}%' or ORDER_QTY like '%{TestCode}%' or 
                                  TURNOVER_QTY like '%{TestCode}%' or B_QTY like '%{TestCode}%' or RETURN_FREQUENCY like '%{TestCode}%' or 
                                AFFECT_HOURS like '%{TestCode}%' or SHOE_NO like '%{TestCode}%' or PROD_NO like '%{TestCode}%'";
                }
                sql = $@"SELECT
	                    RETURN_NO,
	                    PLANT_AREA,
	                    ORDER_QTY,
	                    TURNOVER_QTY,
	                    B_QTY,
	                    RETURN_FREQUENCY,
	                    AFFECT_HOURS,
	                    SHOE_NO,
	                    PROD_NO
                    FROM
	                    qcm_bad_return_m where 1=1 {where}";
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
        /// 提交不良退货
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubmitBadReturn(object OBJ)
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
                string RETURN_NO = jarr.ContainsKey("RETURN_NO") ? jarr["RETURN_NO"].ToString() : "";//退货单号
                string BAD_REASON = jarr.ContainsKey("BAD_REASON") ? jarr["BAD_REASON"].ToString() : "";//不良原因
                string TREATMENT_METHOD = jarr.ContainsKey("TREATMENT_METHOD") ? jarr["TREATMENT_METHOD"].ToString() : "";//处理方法
                string TREATMENT_RESULT = jarr.ContainsKey("TREATMENT_RESULT") ? jarr["TREATMENT_RESULT"].ToString() : "";//处理结果

                DataTable ImgListBL = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["ImgListBL"].ToString());//不良原因图片

                DataTable ImgListJG= Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["ImgListJG"].ToString());//结果录入图片

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                string guid = Guid.NewGuid().ToString("N");

                DB.ExecuteNonQuery($@"insert into qcm_bad_return_d (F_GUID,RETURN_NO,BAD_REASON,TREATMENT_METHOD,TREATMENT_RESULT,
                                        CREATEBY,CREATEDATE,CREATETIME) 
                                      values('{guid}','{RETURN_NO}','{BAD_REASON}','{TREATMENT_METHOD}','{TREATMENT_RESULT}',
                                        '{user}','{date}','{time}')");

                foreach (DataRow item in ImgListBL.Rows)
                {
                    DB.ExecuteNonQuery($@"insert into qcm_bad_return_image (F_GUID,RETURN_NO,IMG_NAME,IMG_URL,GUID,
                                           TYPE,CREATEBY,CREATEDATE,CREATETIME )
                                        values('{guid}','{RETURN_NO}','{item["IMG_NAME"]}','{item["IMG_URL"]}','{Guid.NewGuid().ToString("N")}',
                                           '1','{user}','{date}','{time}' )");
                }
                foreach (DataRow item in ImgListJG.Rows)
                {
                    DB.ExecuteNonQuery($@"insert into qcm_bad_return_image (F_GUID,RETURN_NO,IMG_NAME,IMG_URL,GUID,
                                           TYPE,CREATEBY,CREATEDATE,CREATETIME )
                                        values('{guid}','{RETURN_NO}','{item["IMG_NAME"]}','{item["IMG_URL"]}','{Guid.NewGuid().ToString("N")}',
                                           '2','{user}','{date}','{time}' )");
                }

                #endregion
                //ret.RetData1 = dt;
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
