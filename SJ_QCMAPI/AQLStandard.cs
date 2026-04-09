using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class AQLStandard
    {
        /// <summary>
        /// AQL查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GitAQL(object OBJ)
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
                string HORI_TYPE = jarr.ContainsKey("HORI_TYPE") ? jarr["HORI_TYPE"].ToString() : "1";
                string LEVEL_TYPE = jarr.ContainsKey("LEVEL_TYPE") ? jarr["LEVEL_TYPE"].ToString() : "1";
                string sql = $@"SELECT
							ID,
							START_QTY,
							END_QTY,
							SAMPLE_QTY,
							HORI_TYPE,
							AC01,
							AC02,
							AC03,
							AC04,
							AC05,
							AC06,
							AC07,
							AC08,
							AC09,
							AC10,
							AC11,
							AC12,
							AC13,
							AC14,
							AC15,
							AC16,
							AC17,
							AC18,
							AC19,
							AC20,
							AC21,
							AC22,
							AC23,
							AC24,
							AC25,
							AC26,
							AC27,
							VAL01,
							VAL02,
							VAL03,
							VAL04,
							VAL05,
							VAL06,
							VAL07,
							VAL08,
							VAL09,
							VAL10,
							VAL11,
							VAL12,
							VAL13,
							VAL14,
							VAL15,
							VAL16,
							VAL17,
							VAL18,
							VAL19,
							VAL20,
							VAL21,
							VAL22,
							VAL23,
							VAL24,
							VAL25,
							VAL26,
							VAL27,
							VALS
						FROM
							BDM_AQL_M
						WHERE HORI_TYPE='{HORI_TYPE}' AND LEVEL_TYPE='{LEVEL_TYPE}'";
                #endregion

                DataTable dt = DB.GetDataTable(sql);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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
        /// 编辑AQL
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AQLEdit(object OBJ)
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
                #region 参数
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//id
                string START_QTY = jarr.ContainsKey("START_QTY") ? jarr["START_QTY"].ToString() : "";//起始数量
                string END_QTY = jarr.ContainsKey("END_QTY") ? jarr["END_QTY"].ToString() : "";//结束数量
                string SAMPLE_QTY = jarr.ContainsKey("SAMPLE_QTY") ? jarr["SAMPLE_QTY"].ToString() : "";//样本数量
                string AC01 = jarr.ContainsKey("AC01") ? jarr["AC01"].ToString() : "";
                string AC02 = jarr.ContainsKey("AC02") ? jarr["AC02"].ToString() : "";
                string AC03 = jarr.ContainsKey("AC03") ? jarr["AC03"].ToString() : "";
                string AC04 = jarr.ContainsKey("AC04") ? jarr["AC04"].ToString() : "";
                string AC05 = jarr.ContainsKey("AC05") ? jarr["AC05"].ToString() : "";
                string AC06 = jarr.ContainsKey("AC06") ? jarr["AC06"].ToString() : "";
                string AC07 = jarr.ContainsKey("AC07") ? jarr["AC07"].ToString() : "";
                string AC08 = jarr.ContainsKey("AC08") ? jarr["AC08"].ToString() : "";
                string AC09 = jarr.ContainsKey("AC09") ? jarr["AC09"].ToString() : "";
                string AC10 = jarr.ContainsKey("AC10") ? jarr["AC10"].ToString() : "";
                string AC11 = jarr.ContainsKey("AC11") ? jarr["AC11"].ToString() : "";
                string AC12 = jarr.ContainsKey("AC12") ? jarr["AC12"].ToString() : "";
                string AC13 = jarr.ContainsKey("AC13") ? jarr["AC13"].ToString() : "";
                string AC14 = jarr.ContainsKey("AC14") ? jarr["AC14"].ToString() : "";
                string AC15 = jarr.ContainsKey("AC15") ? jarr["AC15"].ToString() : "";
                string AC16 = jarr.ContainsKey("AC16") ? jarr["AC16"].ToString() : "";
                string AC17 = jarr.ContainsKey("AC17") ? jarr["AC17"].ToString() : "";
                string AC18 = jarr.ContainsKey("AC18") ? jarr["AC18"].ToString() : "";
                string AC19 = jarr.ContainsKey("AC19") ? jarr["AC19"].ToString() : "";
                string AC20 = jarr.ContainsKey("AC20") ? jarr["AC20"].ToString() : "";
                string AC21 = jarr.ContainsKey("AC21") ? jarr["AC21"].ToString() : "";
                string AC22 = jarr.ContainsKey("AC22") ? jarr["AC22"].ToString() : "";
                string AC23 = jarr.ContainsKey("AC23") ? jarr["AC23"].ToString() : "";
                string AC24 = jarr.ContainsKey("AC24") ? jarr["AC24"].ToString() : "";
                string AC25 = jarr.ContainsKey("AC25") ? jarr["AC25"].ToString() : "";
                string AC26 = jarr.ContainsKey("AC26") ? jarr["AC26"].ToString() : "";
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间
                #endregion

                if (!string.IsNullOrEmpty(ID))
                {
                    string count = DB.GetString($@"select COUNT(1) from BDM_AQL_M where  ('{START_QTY}' BETWEEN  START_QTY AND END_QTY)  and ID <> '{ID}'");
                    if (count != "0")
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "The starting quantity cannot exist in the starting quantity and ending quantity of the stored data!";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"update BDM_AQL_M set START_QTY='{START_QTY}',END_QTY='{END_QTY}',SAMPLE_QTY='{SAMPLE_QTY}',
										AC01='{AC01}',AC02='{AC02}',AC03='{AC03}',AC04='{AC04}',AC05='{AC05}',AC06='{AC06}',AC07='{AC07}',
										AC08='{AC08}',AC09='{AC09}',AC10='{AC10}',AC11='{AC11}',AC12='{AC12}',AC13='{AC13}',AC14='{AC14}',
										AC15='{AC15}',AC16='{AC16}',AC17='{AC17}',AC18='{AC18}',AC19='{AC19}',AC20='{AC20}',AC21='{AC21}',
										AC22='{AC22}',AC23='{AC23}',AC24='{AC24}',AC25='{AC25}',AC26='{AC26}',MODIFYBY='{user}',MODIFYDATE='{date}',
										MODIFYTIME='{time}' where ID='{ID}'");
                }
                else
                {

                }
                #endregion
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

        /// <summary>
        /// 修改赋值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateGitAQL(object OBJ)
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
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//id

                string sql = $@"SELECT
								ID,
								START_QTY,
								END_QTY,
								SAMPLE_QTY,
								AC01,
								(AC01+1) AS RE01,
								AC02,
								(AC02+1) AS RE02,
								AC03,
								(AC03+1) AS RE03,
								AC04,
								(AC04+1) AS RE04,
								AC05,
								(AC05+1) AS RE05,
								AC06,
								(AC06+1) AS RE06,
								AC07,
								(AC07+1) AS RE07,
									AC08,
								(AC08+1) AS RE08,
									AC09,
								(AC09+1) AS RE09,
									AC10,
								(AC10+1) AS RE10,
									AC11,
								(AC11+1) AS RE11,
									AC12,
								(AC12+1) AS RE12,
									AC13,
								(AC13+1) AS RE13,
									AC14,
								(AC14+1) AS RE14,
									AC15,
								(AC15+1) AS RE15,
									AC16,
								(AC16+1) AS RE16,
									AC17,
								(AC17+1) AS RE17,
									AC18,
								(AC18+1) AS RE18,
									AC19,
								(AC19+1) AS RE19,
									AC20,
								(AC20+1) AS RE20,
									AC21,
								(AC21+1) AS RE21,
									AC22,
								(AC22+1) AS RE22,
									AC23,
								(AC23+1) AS RE23,
									AC24,
								(AC24+1) AS RE24,
									AC25,
								(AC25+1) AS RE25,
										AC26,
								(AC26+1) AS RE26
							FROM
								BDM_AQL_M
							where ID='{ID}'";
                #endregion
                DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// 删除AQL
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteAQL(object OBJ)
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
                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//id

                DB.ExecuteNonQuery($@"delete from BDM_AQL_M where ID='{ID}'");
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
        /// AQL查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ENUM(object OBJ)
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

                string DOC_TYPE = jarr.ContainsKey("DOC_TYPE") ? jarr["DOC_TYPE"].ToString() : "";
                string LEVEL_TYPE = jarr.ContainsKey("LEVEL_TYPE") ? jarr["LEVEL_TYPE"].ToString() : "";
                string sql = $@"select*from SYS001M where ENUM_TYPE='{DOC_TYPE}'";
                DataTable dt = DB.GetDataTable(sql);
                sql = $@"select*from SYS001M where ENUM_TYPE='{LEVEL_TYPE}'";
                DataTable dt2 = DB.GetDataTable(sql);
                #endregion
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt2);
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
        /// 修改赋值
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddAQL(object OBJ)
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


                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string Datas = jarr.ContainsKey("Data") ? jarr["Data"].ToString() : "";
                DataTable p_BDM_AQL_M = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(Datas);
                DB.Open();
                DB.BeginTransaction();
                foreach (DataRow item in p_BDM_AQL_M.Rows)
                {
                    #region 参数
                    string ID = item["ID"].ToString();//id
                    string START_QTY = item["START_QTY"].ToString();//起始数量
                    string END_QTY = item["END_QTY"].ToString();//结束数量
                    string SAMPLE_QTY = item["SAMPLE_QTY"].ToString();//样本数量
                    string HORI_TYPE = item["HORI_TYPE"].ToString();//AQL类型
                    string LEVEL_TYPE = item["LEVEL_TYPE"].ToString();//AQL类型
                    string VALUES = item["VALS"].ToString();
                    string AC01 = item["AC01"].ToString();
                    string AC02 = item["AC02"].ToString();
                    string AC03 = item["AC03"].ToString();
                    string AC04 = item["AC04"].ToString();
                    string AC05 = item["AC05"].ToString();
                    string AC06 = item["AC06"].ToString();
                    string AC07 = item["AC07"].ToString();
                    string AC08 = item["AC08"].ToString();
                    string AC09 = item["AC09"].ToString();
                    string AC10 = item["AC10"].ToString();
                    string AC11 = item["AC11"].ToString();
                    string AC12 = item["AC12"].ToString();
                    string AC13 = item["AC13"].ToString();
                    string AC14 = item["AC14"].ToString();
                    string AC15 = item["AC15"].ToString();
                    string AC16 = item["AC16"].ToString();
                    string AC17 = item["AC17"].ToString();
                    string AC18 = item["AC18"].ToString();
                    string AC19 = item["AC19"].ToString();
                    string AC20 = item["AC20"].ToString();
                    string AC21 = item["AC21"].ToString();
                    string AC22 = item["AC22"].ToString();
                    string AC23 = item["AC23"].ToString();
                    string AC24 = item["AC24"].ToString();
                    string AC25 = item["AC25"].ToString();
                    string AC26 = item["AC26"].ToString();
                    string AC27 = "";
                    string VAL01 = item["VAL01"].ToString();
                    string VAL02 = item["VAL02"].ToString();
                    string VAL03 = item["VAL03"].ToString();
                    string VAL04 = item["VAL04"].ToString();
                    string VAL05 = item["VAL05"].ToString();
                    string VAL06 = item["VAL06"].ToString();
                    string VAL07 = item["VAL07"].ToString();
                    string VAL08 = item["VAL08"].ToString();
                    string VAL09 = item["VAL09"].ToString();
                    string VAL10 = item["VAL10"].ToString();
                    string VAL11 = item["VAL11"].ToString();
                    string VAL12 = item["VAL12"].ToString();
                    string VAL13 = item["VAL13"].ToString();
                    string VAL14 = item["VAL14"].ToString();
                    string VAL15 = item["VAL15"].ToString();
                    string VAL16 = item["VAL16"].ToString();
                    string VAL17 = item["VAL17"].ToString();
                    string VAL18 = item["VAL18"].ToString();
                    string VAL19 = item["VAL19"].ToString();
                    string VAL20 = item["VAL20"].ToString();
                    string VAL21 = item["VAL21"].ToString();
                    string VAL22 = item["VAL22"].ToString();
                    string VAL23 = item["VAL23"].ToString();
                    string VAL24 = item["VAL24"].ToString();
                    string VAL25 = item["VAL25"].ToString();
                    string VAL26 = item["VAL26"].ToString();
                    string VAL27 = "";
                    string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                    string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                    string time = DateTime.Now.ToString("HH-mm-ss");//时间
                    #endregion

                    if (item["ID"].ToString() == "")
                    {
                        string count = DB.GetString($@"select COUNT(1) from BDM_AQL_M where  ({START_QTY} BETWEEN  nvl(START_QTY,0) AND nvl(END_QTY,0)) AND HORI_TYPE='{HORI_TYPE}' AND LEVEL_TYPE='{LEVEL_TYPE}'");
                        if (count != "0")
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "The starting quantity cannot exist in the starting quantity and ending quantity of the stored data!";
                            return ret;
                        }
                        DB.ExecuteNonQuery($@"insert into BDM_AQL_M (START_QTY,END_QTY,SAMPLE_QTY,HORI_TYPE,LEVEL_TYPE,AC01,AC02,AC03,AC04,AC05,
											AC06,AC07,AC08,AC09,AC10,AC11,AC12,AC13,AC14,AC15,AC16,AC17,AC18,AC19,AC20,
											AC21,AC22,AC23,AC24,AC25,AC26,AC27,VAL01,VAL02,VAL03,VAL04,VAL05,VAL06,VAL07,VAL08,VAL09,VAL10,VAL11,VAL12,VAL13,VAL14,VAL15,VAL16,VAL17,
											VAL18,VAL19,VAL20,VAL21,VAL22,VAL23,VAL24,VAL25,VAL26,VAL27,CREATEBY,CREATEDATE,CREATETIME,VALS) 
											values('{START_QTY}','{END_QTY}','{SAMPLE_QTY}','{HORI_TYPE}','{LEVEL_TYPE}','{AC01}','{AC02}','{AC03}','{AC04}','{AC05}',
											'{AC06}','{AC07}','{AC08}','{AC09}','{AC10}','{AC11}','{AC12}','{AC13}','{AC14}','{AC15}','{AC16}',
											'{AC17}','{AC18}','{AC19}','{AC20}','{AC21}','{AC22}','{AC23}','{AC24}','{AC25}','{AC26}','{AC27}','{VAL01}','{VAL02}','{VAL03}','{VAL04}','{VAL05}',
											'{VAL06}','{VAL07}','{VAL08}','{VAL09}','{VAL10}','{VAL11}','{VAL12}','{VAL13}','{VAL14}','{VAL15}','{VAL16}',
											'{VAL17}','{VAL18}','{VAL19}','{VAL20}','{VAL21}','{VAL22}','{VAL23}','{VAL24}','{VAL25}','{VAL26}','{VAL27}',
											'{user}','{date}','{time}','{VALUES}')");
                    }
                    else
                    {
                        string count = DB.GetString($@"select COUNT(1) from BDM_AQL_M where  ({START_QTY} BETWEEN  nvl(START_QTY,0) AND nvl(END_QTY,0)) AND HORI_TYPE='{HORI_TYPE}' AND LEVEL_TYPE='{LEVEL_TYPE}' AND ID NOT IN('{ID}')");
                        if (count != "0")
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "The starting quantity cannot exist in the starting quantity and ending quantity of the stored data!";
                            return ret;
                        }
                        string sql = $@"UPDATE BDM_AQL_M set  START_QTY='{START_QTY}',END_QTY='{END_QTY}',SAMPLE_QTY='{SAMPLE_QTY}',
						AC01='{AC01}',AC02='{AC02}',AC03='{AC03}',AC04='{AC04}',AC05='{AC05}',
                        AC06='{AC06}',AC07='{AC07}',AC08='{AC08}',AC09='{AC09}',AC10='{AC10}',
						AC11='{AC11}',AC12='{AC12}',AC13='{AC13}',AC14='{AC14}',AC15='{AC15}',
						AC16='{AC16}',AC17='{AC17}',AC18='{AC18}',AC19='{AC19}',AC20='{AC20}',
						AC21='{AC21}',AC22='{AC22}',AC23='{AC23}',AC24='{AC24}',AC25='{AC25}',AC26='{AC26}',AC27='{AC27}',
						VAL01='{VAL01}',VAL02='{VAL02}',VAL03='{VAL03}',VAL04='{VAL04}',VAL05='{VAL05}',
                        VAL06='{VAL06}',VAL07='{VAL07}',VAL08='{VAL08}',VAL09='{VAL09}',VAL10='{VAL10}',
						VAL11='{VAL11}',VAL12='{VAL12}',VAL13='{VAL13}',VAL14='{VAL14}',VAL15='{VAL15}',
						VAL16='{VAL16}',VAL17='{VAL17}',VAL18='{VAL18}',VAL19='{VAL19}',VAL20='{VAL20}',
						VAL21='{VAL21}',VAL22='{VAL22}',VAL23='{VAL23}',VAL24='{VAL24}',VAL25='{VAL25}',VAL26='{VAL26}',VAL27='{VAL27}',
						MODIFYBY='{user}',MODIFYDATE='{date}',MODIFYTIME='{time}',VALS='{VALUES}'
						where id={ID}";
                        DB.ExecuteNonQuery(sql);

                    }
                }

                DB.Commit();
                ret.RetData = "成功";
                ret.IsSuccess = true;
                //DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                DB.Rollback();
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
