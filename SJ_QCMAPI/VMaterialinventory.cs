using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    //

    public class VMaterialinventory
    {

        //-------------------------材料进仓检验清单-------------------------------------------------//
        /// <summary>
        /// 主页数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMain(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";//收料日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//收料日期
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string jieguo = jarr.ContainsKey("jieguo") ? jarr["jieguo"].ToString() : "";//物性结果
                string quyang = jarr.ContainsKey("quyang") ? jarr["quyang"].ToString() : "";//取样状况
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//采购厂商
                string VEND_NO2 = jarr.ContainsKey("VEND_NO2") ? jarr["VEND_NO2"].ToString() : "";//生产厂商
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";//状态
                string bianma = jarr.ContainsKey("bianma") ? jarr["bianma"].ToString() : "";//物料编码  
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//仓别
                string wgjieguo = jarr.ContainsKey("wgjieguo") ? jarr["wgjieguo"].ToString() : "";//外观结果
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//工厂

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                var sql = string.Empty;
                string strwhere = string.Empty;
                if (!string.IsNullOrEmpty(putin_date))
                {
                    strwhere += $@" and PROD_NO like '%{putin_date}%'";
                }
                if (!string.IsNullOrEmpty(putin_date))
                {
                    strwhere += $@" and SHOE_NO like '%{putin_date}%' ";
                }
                if (!string.IsNullOrEmpty(putin_date))
                {
                    strwhere += $@" and MODULE_NO like '%{putin_date}%'";
                }
                if (!string.IsNullOrEmpty(putin_date))//搜索时间
                {
                    strwhere += $@" and CREATEDATE='{putin_date.Trim()}'";
                }
                sql = $@"SELECT
	CHK_NO, --收料单号
	RCPT_DATE, --收料日期
	SOURCE_NO, --来源单号
	RCPT_BY, --收料来源
	VEND_NO, --厂商代号
	DELIVER_NO, --送货单号
	TRANS_TYPE, --移动类型
	STOC_NO, --仓库代号
	RCPT_TYPE --移动原因 
FROM
	wms_rcpt_m where  1=1 {strwhere}";

                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
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
        ///查看检验结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultJYView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";

                var sql = string.Empty;
                #region 条件
                string whereSql = string.Empty;
                if (!string.IsNullOrEmpty(CHK_NO))
                {
                    whereSql += $@"and CHK_NO like'%{CHK_NO}%'";
                }
             
                #endregion
                sql = $@"select chk_no,test_item_no,test_item_name,test_standard,determine,image_guid,remark from qcm_iqc_insp_res_d where 1=1 {whereSql}";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = DB.GetDataTable(sql);
             
                dic.Add("Data", dt);

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
        ///外观检验结果录入
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultAdd(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                DB.Open();
                DB.BeginTransaction();
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var p = jarr.ContainsKey("p")? jarr["p"].ToString():"";
                var diclist = jarr.ContainsKey("diclist")?jarr["diclist"].ToString():"";
                if(p!=null && diclist != null)
                {
                    //表头内容
                    var jarr1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(p.ToString());
                    string sample_qty = jarr1.ContainsKey("sample_qty") ? jarr1["sample_qty"].ToString() : "";//抽样数量
                    string chk_no = jarr1.ContainsKey("chk_no") ? jarr1["chk_no"].ToString() : "";//来料单号
                    string bad_qty = jarr1.ContainsKey("bad_qty") ? jarr1["bad_qty"].ToString() : "";//不合格数量
                    string determine = jarr1.ContainsKey("determine") ? jarr1["determine"].ToString() : "";//判断
                    string sql = String.Empty;


                    var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<listdic>(diclist);
                    if (requst.listdic2.Count > 0)
                    {
                        var IDList = requst.listdic2.Where(x => x.ID.HasValue).Select(x => x.ID);
                        if (IDList.Count() > 0)
                        {
                            string IDS = string.Join(',', IDList);
                            //先删除请求里不存在的ID数据
                            string delSql = $@"DELETE FROM BDM_OUT_QUALITY_LIST_M WHERE ID NOT IN({IDS})";
                            DB.ExecuteNonQuery(delSql);
                        }
                        foreach (var item in requst.listdic2)
                        {
                            if (!item.ID.HasValue)
                            {//新增
                                Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                                insert_dic.Add("chk_no", item.chk_no);
                                insert_dic.Add("test_item_no", item.test_item_no);
                                insert_dic.Add("test_item_name", item.test_item_name);
                                insert_dic.Add("test_standard", item.test_standard);
                                insert_dic.Add("determine", item.determine);
                                insert_dic.Add("image_guid", item.image_guid);
                                insert_dic.Add("remark", item.remark);
                                insert_dic.Add("CREATEBY", userCode);
                                insert_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                                insert_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));

                                 sql= SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "qcm_iqc_insp_res_m", insert_dic);
                                DB.ExecuteNonQuery(sql, insert_dic);
                            }
                            else
                            {//修改
                                Dictionary<string, object> update_val_dic = new Dictionary<string, object>();
                                update_val_dic.Add("chk_no", item.chk_no);
                                update_val_dic.Add("test_item_no", item.test_item_no);
                                update_val_dic.Add("test_item_name", item.test_item_name);
                                update_val_dic.Add("test_standard", item.test_standard);
                                update_val_dic.Add("determine", item.determine);
                                update_val_dic.Add("image_guid", item.image_guid);
                                update_val_dic.Add("remark", item.remark);

                                update_val_dic.Add("MODIFYBY", userCode);
                                update_val_dic.Add("MODIFYDATE", currDate.ToString("yyyy-MM-dd"));
                                update_val_dic.Add("MODIFYTIME", currDate.ToString("HH:mm:ss"));

                                string whereSql = $@"ID=@ID";
                                sql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("BDM_OUT_QUALITY_LIST_M", whereSql, update_val_dic);
                                update_val_dic.Add("ID", item.ID);
                                DB.ExecuteNonQuery(sql, update_val_dic);
                            }
                        }
                    }
                   

                }
                DB.Commit();
                ret.ErrMsg = "保存成功！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "保存失败，原因：" + ex.Message;
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 检验结果表身录入展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultLRView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";
                var sql = string.Empty;
                #region 条件
                string whereSql = string.Empty;
                if (!string.IsNullOrEmpty(chk_no))
                {
                    whereSql += $@"and a.chk_no ='%{chk_no}%'";
                }
                #endregion
                sql = $@"SELECT
	a.bad_qty,
	a.determine,
	a.closing_status,
	b.chk_no,
	b.test_item_no,
	b.test_item_name,
	b.test_standard,
	b.determine,
	b.image_guid,
	b.remark
FROM
	qcm_iqc_insp_res_m a
LEFT JOIN qcm_iqc_insp_res_d b ON a.chk_no = b.chk_no where 1=1 {whereSql}";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                DataTable dt = DB.GetDataTable(sql);

                dic.Add("Data", dt);

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


        public class listdic
        {
            /// <summary>
            /// 项目集合
            /// </summary>
            public List<qcm_iqc_insp_res_d> listdic2 = new List<qcm_iqc_insp_res_d>();
        }
      
        public  class qcm_iqc_insp_res_d
        {
            public int? ID { get; set; }
            public string chk_no { get; set; }
            public string test_item_no { get; set; }
            public string test_item_name { get; set; }
            public string test_standard { get; set; }
            public string determine { get; set; }
            public string image_guid{ get; set; }
            public string remark{ get; set; }
        }
    }
}
