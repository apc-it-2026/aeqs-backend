using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class ARTFileBind
    {
        /// <summary>
        /// 获取数据绑定列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetFileList(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString().Trim() : "";//一级分类
                string TYPE1 = jarr.ContainsKey("TYPE1") ? jarr["TYPE1"].ToString().Trim() : "";//二级分类
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString().Trim() : "";//art
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = "";
                if (!string.IsNullOrEmpty(TYPE))
                {
                    where += $" AND TYPE='{TYPE}'";
                }
                if (!string.IsNullOrEmpty(TYPE1))
                {
                    where += $" AND TYPE1='{TYPE1}'";
                }
                if (!string.IsNullOrEmpty(ART))
                {
                    where += $" AND ART LIKE '%{ART}%'";
                }
                string sql = $"SELECT * FROM QCM_ART_FILE_M WHERE 1=1 {where}";
                //查询分页数据
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);

                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 添加文件绑定
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject AddFile(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString().Trim() : "";//art
                string FILE_TYPE = jarr.ContainsKey("FILE_TYPE") ? jarr["FILE_TYPE"].ToString().Trim() : "";//一级分类
                string FILE_TYPE_TEXT = jarr.ContainsKey("FILE_TYPE_TEXT") ? jarr["FILE_TYPE_TEXT"].ToString().Trim() : "";//一级分类
                string TYPE = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString().Trim() : "";//一级分类
                string TYPE1 = jarr.ContainsKey("TYPE1") ? jarr["TYPE1"].ToString().Trim() : "";//二级分类
                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString().Trim() : "";//二级分类
                string FILE_URL = jarr.ContainsKey("FILE_URL") ? jarr["FILE_URL"].ToString().Trim() : "";//二级分类
                string EFFECTIVE_DATE = jarr.ContainsKey("EFFECTIVE_DATE") ? jarr["EFFECTIVE_DATE"].ToString().Trim() : "";//art

                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                if (DB.GetInt32($"SELECT COUNT(1) FROM QCM_ART_FILE_M WHERE ART='{ART}' AND FILE_TYPE='{FILE_TYPE}'") > 0)
                {
                    DB.ExecuteNonQueryOffline($"UPDATE QCM_ART_FILE_M SET FILE_NAME='{FILE_NAME}',FILE_URL='{FILE_URL}',EFFECTIVE_DATE='{EFFECTIVE_DATE}',MODIFYBY='{usercode}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE ART='{ART}' AND FILE_TYPE='{FILE_TYPE}'");
                }
                else
                {
                    DB.ExecuteNonQueryOffline($@"INSERT INTO QCM_ART_FILE_M
                                                        (
                                                            ART,
                                                            FILE_TYPE,
                                                            FILE_TYPE_TEXT,
                                                            TYPE,
                                                            TYPE1,
                                                            FILE_NAME,
                                                            FILE_URL,
                                                            EFFECTIVE_DATE,
                                                            BIND_DATE,
                                                            CREATEBY,
                                                            CREATEDATE,
                                                            CREATETIME
                                                         )
                                                         VALUES
                                                         (
                                                            '{ART}',
                                                            '{FILE_TYPE}',
                                                            '{FILE_TYPE_TEXT}',
                                                            '{TYPE}',
                                                            '{TYPE1}',
                                                            '{FILE_NAME}',
                                                            '{FILE_URL}',
                                                            '{EFFECTIVE_DATE}',
                                                            '{date}',
                                                            '{usercode}',
                                                            '{date}',
                                                            '{time}'                                                       
                                                         )");


                }

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
        /// 修改文件绑定
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateFile(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString().Trim() : "";//art
                string FILE_NAME = jarr.ContainsKey("FILE_NAME") ? jarr["FILE_NAME"].ToString().Trim() : "";//二级分类
                string FILE_URL = jarr.ContainsKey("FILE_URL") ? jarr["FILE_URL"].ToString().Trim() : "";//二级分类
                string EFFECTIVE_DATE = jarr.ContainsKey("EFFECTIVE_DATE") ? jarr["EFFECTIVE_DATE"].ToString().Trim() : "";//art

                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                DB.ExecuteNonQueryOffline($"UPDATE QCM_ART_FILE_M SET FILE_NAME='{FILE_NAME}',FILE_URL='{FILE_URL}',EFFECTIVE_DATE='{EFFECTIVE_DATE}',MODIFYBY='{usercode}',MODIFYDATE='{date}',MODIFYTIME='{time}' WHERE ID='{ID}'");


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
        /// 删除文件绑定
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteFile(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString().Trim() : "";//art

                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                DB.ExecuteNonQueryOffline($"DELETE FROM QCM_ART_FILE_M WHERE ID='{ID}'");

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
        /// 获取数据绑定列表
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetInfoByART(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string ART = jarr.ContainsKey("ART") ? jarr["ART"].ToString().Trim() : "";//art
                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString().Trim() : "";//art



                string sql = $@"select a.ENUM_CODE,a.ENUM_VALUE,a.TYPE,a.TYPE1,b.FILE_NAME,b.FILE_URL,c.WHD,c.YHD,c.QRQM,c.SURE_USER,c.SURE_USER_NAME from SYS001M a
                                LEFT JOIN QCM_ART_FILE_M b on b.ART = '{ART}' and a.ENUM_CODE = b.FILE_TYPE and to_date(b.EFFECTIVE_DATE,'yyyy-MM-dd')>= CURRENT_DATE
																left join QCM_PO_FILE_CHECK_M c on c.PO='{PO}' and c.TYPE=a.TYPE 
																and nvl(c.TYPE1,'-1')=nvl(a.TYPE1,'-1')
                                where ENUM_TYPE = 'enum_file_type'";
                //查询分页数据
                DataTable dt = DB.GetDataTable(sql);



                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }


        /// <summary>
        /// 提交AQL品质录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SavePOFileCheck(Object OBJ)
        {

            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            string Data = string.Empty;
            string guid = string.Empty;
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                DB.Open();
                DB.BeginTransaction();
                Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string PO = jarr.ContainsKey("PO") ? jarr["PO"].ToString().Trim() : "";//art
                string SURE_USER = jarr.ContainsKey("SURE_USER") ? jarr["SURE_USER"].ToString().Trim() : "";//一级分类
                string SURE_USER_NAME = jarr.ContainsKey("SURE_USER_NAME") ? jarr["SURE_USER_NAME"].ToString().Trim() : "";//一级分类
                string CHECK_DATA = jarr.ContainsKey("CHECK_DATA") ? jarr["CHECK_DATA"].ToString().Trim() : "";//一级分类

                Dictionary<string, Dictionary<string, bool>> check_dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, bool>>>(CHECK_DATA);



                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                DB.ExecuteNonQuery($"DELETE FROM QCM_PO_FILE_CHECK_M WHERE PO='{PO}'");
                foreach (var item in check_dic)
                {

                    String TYPE = "";
                    String TYPE1 = "";
                    if (item.Key.Contains("#"))
                    {
                        TYPE = item.Key.Split('#')[0];
                        TYPE1 = item.Key.Split('#')[1];
                    }
                    else
                    {
                        TYPE = item.Key;
                    }
                    String WHD = item.Value["WHD"].ToString();
                    String YHD = item.Value["YHD"].ToString();
                    String QRQM = item.Value["QRQM"].ToString();
                    DB.ExecuteNonQuery($@"INSERT INTO QCM_PO_FILE_CHECK_M
                                            (
                                                PO,
                                                TYPE,
                                                TYPE1,
                                                SURE_USER,
                                                SURE_USER_NAME,
                                                WHD,
                                                YHD,
                                                QRQM,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                             )
                                            VALUES
                                            (
                                                '{PO}',
                                                '{TYPE}',
                                                '{TYPE1}',
                                                '{SURE_USER}',
                                                '{SURE_USER_NAME}',
                                                '{WHD}',
                                                '{YHD}',
                                                '{QRQM}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                            )
                    ");

                }

                DB.Commit();
                ret.IsSuccess = true;

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

    }
}
