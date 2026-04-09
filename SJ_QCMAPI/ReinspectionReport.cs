using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    /// <summary>
    /// 重检报告
    /// </summary>
    public class ReinspectionReport
    {
        /***********************************************PDA start************************************************************/
        /// <summary>
        /// 模糊查询重检报告
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAllReinspectionReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string searchStr = jarr.ContainsKey("SearchStr") ? jarr["SearchStr"].ToString() : "";
                string STATUS = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";

                string whereSql = " Where 1=1 ";
                if (!string.IsNullOrEmpty(searchStr))
                {
                    whereSql += $@"and (OUTSOURCING_INSPECTION_NO like '%{searchStr}%' or SUPPLIERS_TYPE like '%{searchStr}%' or SUPPLIERS_NAME like '%{searchStr}%' or PO_ORDER like '%{searchStr}%' or PROD_NO like '%{searchStr}%') ";
                }
                if (!string.IsNullOrEmpty(STATUS))
                {
                    if (STATUS.ToUpper() == "PAIL")
                        whereSql += $@"and  STATUS='0' ";
                    else
                        whereSql += $@"and  STATUS='1' ";
                }

                #region 逻辑
                string sql = $@"
SELECT
	OUTSOURCING_INSPECTION_NO,
	SUPPLIERS_TYPE,
	SUPPLIERS_NAME,
	PO_ORDER,
	PROD_NO,
	WH_QTY,
	SPOT_CHECK_QTY,
	GUID
FROM
	QCM_REINSPECTION_REPORT_M 
";
                DataTable dt = DB.GetDataTable(sql + whereSql);
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 获取重检报告详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReinspectionReportDetail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string GUID = jarr.ContainsKey("GUID") ? jarr["GUID"].ToString() : "";


                #region 逻辑
                string sql = $@"
SELECT DISTINCT
	QCM_REINSPECTION_REPORT_M.OUTSOURCING_INSPECTION_NO,
	QCM_REINSPECTION_REPORT_M.SUPPLIERS_TYPE,
	QCM_REINSPECTION_REPORT_M.SUPPLIERS_CODE,-- 厂商代号
	QCM_REINSPECTION_REPORT_M.SUPPLIERS_NAME,-- 厂商名称
	QCM_REINSPECTION_REPORT_M.SHOE_NO,-- 鞋型
	QCM_REINSPECTION_REPORT_M.PO_ORDER,-- PO号
	QCM_REINSPECTION_REPORT_M.PROD_NO,-- ART
	QCM_REINSPECTION_REPORT_M.WH_QTY,-- 进仓数
	QCM_REINSPECTION_REPORT_M.SPOT_CHECK_QTY,-- 抽检数量
	QCM_REINSPECTION_REPORT_M.GENERAL_TESTTYPE_NO,-- 通用标准
	BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NAME, -- 通用标准名称
	QCM_REINSPECTION_REPORT_M.CATEGORY_NO,-- 检测项
	BDM_PROD_CUSTOMQUALITY_D.CATEGORY_NAME, -- 检测名称
	QCM_REINSPECTION_REPORT_M.BAD_QTY,-- 不良数
	QCM_REINSPECTION_REPORT_M.BAD_RATE,-- 不良率
	QCM_REINSPECTION_REPORT_M.NOT_ACCEPT_QTY,-- 不可接受数量
	QCM_REINSPECTION_REPORT_M.ACCEPT_QTY,-- 接受数量
	QCM_REINSPECTION_REPORT_M.GUID,
	QCM_REINSPECTION_REPORT_M.CREATEDATE
FROM
	QCM_REINSPECTION_REPORT_M
LEFT JOIN BDM_GENERAL_TESTTYPE_M ON QCM_REINSPECTION_REPORT_M.GENERAL_TESTTYPE_NO = BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NO
LEFT JOIN BDM_PROD_CUSTOMQUALITY_D ON BDM_PROD_CUSTOMQUALITY_D.CATEGORY_NO = QCM_REINSPECTION_REPORT_M.CATEGORY_NO
WHERE
	GUID = '{GUID}'
";
                DataTable dt = DB.GetDataTable(sql);
                GetReinspectionReportDetailResDto res = dt.ToDataList<GetReinspectionReportDetailResDto>().FirstOrDefault();
                if (res == null)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "查无数据";
                    return ret;
                }
                //if (!string.IsNullOrEmpty(res.BAD_RATE))
                //{
                //    decimal rate = 0;
                //    if(decimal.TryParse(res.BAD_RATE, out rate))
                //    {
                //        res.BAD_RATE = rate * 100 + "%";
                //    }
                //}
                string detailSql = $@"
SELECT DISTINCT
	QCM_REINSPECTION_REPORT_D.PROD_NO,
	QCM_REINSPECTION_REPORT_D.GENERAL_TESTTYPE_NO,
	BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NAME, -- 通用标准名称
	QCM_REINSPECTION_REPORT_D.CATEGORY_NO,
	BDM_PROD_CUSTOMQUALITY_D.CATEGORY_NAME, -- 检测名称
	QCM_REINSPECTION_REPORT_D.TESTITEM_CATEGORY,
	QCM_REINSPECTION_REPORT_D.TESTITEM_CODE,
	QCM_REINSPECTION_REPORT_D.TESTITEM_NAME,
	QCM_REINSPECTION_REPORT_D.TESTTYPE_NO,
	QCM_REINSPECTION_REPORT_D.TESTTYPE_NAME,
	QCM_REINSPECTION_REPORT_D.SAMPLE_NUM,
	QCM_REINSPECTION_REPORT_D.T_CHECK_ITEM,
	QCM_REINSPECTION_REPORT_D.T_CHECK_VALUE,
	QCM_REINSPECTION_REPORT_D.AQL_LEVEL,
	QCM_REINSPECTION_REPORT_D.AC,
	QCM_REINSPECTION_REPORT_D.RE,
	QCM_REINSPECTION_REPORT_D.INS_RES,
	QCM_REINSPECTION_REPORT_D.BAD_QTY,
	QCM_REINSPECTION_REPORT_D.PROBLEM_POINT,
	QCM_REINSPECTION_REPORT_D.WH_RETURN,
	QCM_REINSPECTION_REPORT_D.REMARK,
  QCM_REINSPECTION_REPORT_D.GUID_IMG
FROM
	QCM_REINSPECTION_REPORT_D
LEFT JOIN BDM_GENERAL_TESTTYPE_M ON QCM_REINSPECTION_REPORT_D.GENERAL_TESTTYPE_NO = BDM_GENERAL_TESTTYPE_M.GENERAL_TESTTYPE_NO
LEFT JOIN BDM_PROD_CUSTOMQUALITY_D ON BDM_PROD_CUSTOMQUALITY_D.CATEGORY_NO = QCM_REINSPECTION_REPORT_D.CATEGORY_NO
WHERE
	GUID = '{GUID}'";
                DataTable dtDetail = DB.GetDataTable(detailSql);
                var detailList = dtDetail.ToDataList<GetReinspectionReportDetailItemResDto>();
                foreach (var item in detailList)
                {
                    string imgSql = $@"SELECT IMG_URL FROM QCM_REINSPECTION_REPORT_IMAGEURL WHERE GUID_IMG='{item.GUID_IMG}'";
                    DataTable imgDt = DB.GetDataTable(imgSql);
                    List<string> imgList = new List<string>();
                    foreach (DataRow imgItem in imgDt.Rows)
                    {
                        imgList.Add(imgItem["IMG_URL"].ToString());
                    }
                    item.IMG_LIST = imgList;
                }
                res.Details = detailList;

                #endregion
                ret.RetData1 = res;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 获取atr检验类型和分类
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetARTMDetails(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";

                #region 逻辑
                string sql = $@"
SELECT
	m.PROD_NO,
	m.GENERAL_TESTTYPE_NO,
	m.GENERAL_TESTTYPE_NAME,
	d.CATEGORY_NO,
	d.CATEGORY_NAME
FROM
BDM_PROD_CUSTOMQUALITY_M m
INNER JOIN BDM_PROD_CUSTOMQUALITY_D d ON (m.PROD_NO=d.PROD_NO and m.GENERAL_TESTTYPE_NO=d.GENERAL_TESTTYPE_NO)
WHERE
	m.PROD_NO = '{PROD_NO}'
";
                DataTable dt = DB.GetDataTable(sql);
                var dtList = dt.ToDataList<GetARTMDetailsDto>();

                var res = dtList.GroupBy(x => x.GENERAL_TESTTYPE_NO).Select(x => new
                {
                    GENERAL_TESTTYPE_NO = x.Key,
                    GENERAL_TESTTYPE_NAME = x.Max(y=>y.GENERAL_TESTTYPE_NAME),
                    OptionItem = x.Select(y => new
                    {
                        y.CATEGORY_NO,
                        y.CATEGORY_NAME
                    })
                });

                #endregion
                ret.RetData1 = res;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 获取art检验项目
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetARTItemDetails(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string PROD_NO = jarr.ContainsKey("PROD_NO") ? jarr["PROD_NO"].ToString() : "";
                string GENERAL_TESTTYPE_NO = jarr.ContainsKey("GENERAL_TESTTYPE_NO") ? jarr["GENERAL_TESTTYPE_NO"].ToString() : "";
                string CATEGORY_NO = jarr.ContainsKey("CATEGORY_NO") ? jarr["CATEGORY_NO"].ToString() : "";

                #region 逻辑
                string sql = $@"
SELECT
	PROD_NO,
	GENERAL_TESTTYPE_NO,
	CATEGORY_NO,
	TESTITEM_CATEGORY,
	TESTITEM_CODE,
	TESTITEM_NAME,-- 检验项名称
	TESTTYPE_NO,-- 检测项类型
	TESTTYPE_NAME,-- 检测项类型名称
	SAMPLE_NUM,-- 检验数量
	T_CHECK_ITEM,-- 判断标准
	T_CHECK_VALUE,-- 测量标准值
	D_CHECK_ITEM,-- 定制判断标准
	D_CHECK_VALUE,-- 定制测量标准值
	AQL_LEVEL,
	AC,
	RE
FROM
	BDM_PROD_CUSTOMQUALITY_ITEM
WHERE
	PROD_NO = '{PROD_NO}'
AND GENERAL_TESTTYPE_NO = '{GENERAL_TESTTYPE_NO}'
AND CATEGORY_NO = '{CATEGORY_NO}'
";
                DataTable dt = DB.GetDataTable(sql);

                foreach (DataRow item in dt.Rows)
                {
                    //优先取定制标准，如果无设置，则取通用标准
                    string D_CHECK_ITEM = item["D_CHECK_ITEM"].ToString();
                    string D_CHECK_VALUE = item["D_CHECK_VALUE"].ToString();
                    if (!string.IsNullOrEmpty(D_CHECK_ITEM) || !string.IsNullOrEmpty(D_CHECK_VALUE))
                    {
                        item["T_CHECK_ITEM"] = D_CHECK_ITEM;
                        item["T_CHECK_VALUE"] = D_CHECK_VALUE;
                    }
                }

                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
            }

            return ret;
        }

        /// <summary>
        /// 保存重检报告详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SaveReinspectionReport(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<SaveReinspectionReportReqDto>(Data);

                #region 逻辑
                DB.Open();
                DB.BeginTransaction();
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string OUTSOURCING_INSPECTION_NO = "";
                string PO_ORDER = "";
                string sql = $@"select OUTSOURCING_INSPECTION_NO,PO_ORDER from QCM_REINSPECTION_REPORT_M where guid='{requst.GUID}'";
                var mDt = DB.GetDataTablebyline(sql);
                foreach (DataRow item in mDt.Rows)
                {
                    OUTSOURCING_INSPECTION_NO = item["OUTSOURCING_INSPECTION_NO"].ToString();
                    PO_ORDER = item["PO_ORDER"].ToString();
                }

                decimal convertD = 1;
                //表头
                Dictionary<string, object> update_val_dic = new Dictionary<string, object>();
                update_val_dic.Add("BAD_QTY", requst.BAD_QTY);
                update_val_dic.Add("GENERAL_TESTTYPE_NO", requst.GENERAL_TESTTYPE_NO);
                update_val_dic.Add("CATEGORY_NO", requst.CATEGORY_NO);
                update_val_dic.Add("NOT_ACCEPT_QTY", requst.NOT_ACCEPT_QTY);
                update_val_dic.Add("ACCEPT_QTY", requst.ACCEPT_QTY);
                update_val_dic.Add("BAD_RATE", (requst.BAD_QTY * convertD / requst.SPOT_CHECK_QTY).ToString("0.00"));

                update_val_dic.Add("MODIFYBY", userCode);
                update_val_dic.Add("MODIFYDATE", currDate.ToString("yyyy-MM-dd"));
                update_val_dic.Add("MODIFYTIME", currDate.ToString("HH:mm:ss"));

                string whereSql = $@"GUID=@GUID";
                sql = SJeMES_Framework_NETCore.Common.StringHelper.GetUpdateSqlByDictionary("QCM_REINSPECTION_REPORT_M", whereSql, update_val_dic);
                update_val_dic.Add("GUID", requst.GUID);

                DB.ExecuteNonQuery(sql, update_val_dic);

                // 每次进来将所有表身相关记录删除
                string delSql = $@"DELETE FROM QCM_REINSPECTION_REPORT_D Where GUID='{requst.GUID}'";
                DB.ExecuteNonQuery(delSql);
                delSql = $@"DELETE FROM QCM_REINSPECTION_REPORT_IMAGEURL Where GUID='{requst.GUID}'";
                DB.ExecuteNonQuery(delSql);
                // 重新新增表身
                foreach (var item in requst.Details)
                {
                    string guid_img = Guid.NewGuid().ToString("N");
                    Dictionary<string, object> insert_dic = new Dictionary<string, object>();
                    insert_dic.Add("PROD_NO", item.PROD_NO);
                    insert_dic.Add("GENERAL_TESTTYPE_NO", item.GENERAL_TESTTYPE_NO);
                    insert_dic.Add("CATEGORY_NO", item.CATEGORY_NO);
                    insert_dic.Add("TESTITEM_CATEGORY", item.TESTITEM_CATEGORY);
                    insert_dic.Add("TESTITEM_CODE", item.TESTITEM_CODE);
                    insert_dic.Add("TESTITEM_NAME", item.TESTITEM_NAME);
                    insert_dic.Add("TESTTYPE_NO", item.TESTTYPE_NO);
                    insert_dic.Add("TESTTYPE_NAME", item.TESTTYPE_NAME);
                    insert_dic.Add("SAMPLE_NUM", item.SAMPLE_NUM);
                    insert_dic.Add("T_CHECK_ITEM", item.T_CHECK_ITEM);
                    insert_dic.Add("T_CHECK_VALUE", item.T_CHECK_VALUE);
                    insert_dic.Add("AQL_LEVEL", item.AQL_LEVEL);
                    insert_dic.Add("AC", item.AC);
                    insert_dic.Add("RE", item.RE);
                    insert_dic.Add("INS_RES", item.INS_RES);
                    insert_dic.Add("BAD_QTY", item.BAD_QTY);
                    insert_dic.Add("PROBLEM_POINT", item.PROBLEM_POINT);
                    insert_dic.Add("WH_RETURN", item.WH_RETURN);
                    insert_dic.Add("REMARK", item.REMARK);
                    insert_dic.Add("OUTSOURCING_INSPECTION_NO", OUTSOURCING_INSPECTION_NO);
                    insert_dic.Add("PO_ORDER", PO_ORDER);

                    insert_dic.Add("CREATEBY", userCode);
                    insert_dic.Add("CREATEDATE", currDate.ToString("yyyy-MM-dd"));
                    insert_dic.Add("CREATETIME", currDate.ToString("HH:mm:ss"));
                    insert_dic.Add("GUID", requst.GUID);
                    insert_dic.Add("GUID_IMG", guid_img);

                    string insertSql = SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_REINSPECTION_REPORT_D", insert_dic);
                    DB.ExecuteNonQuery(insertSql, insert_dic);

                    if (item.IMG_LIST != null)
                    {
                        // 保存图片
                        foreach (string imgPath in item.IMG_LIST)
                        {
                            string fileName = imgPath.Split('/').LastOrDefault();
                            if (string.IsNullOrEmpty(fileName))
                                fileName = "";
                            DB.ExecuteNonQuery($@"insert into QCM_REINSPECTION_REPORT_IMAGEURL (img_name,img_url,guid,guid_img,createby,createdate,createtime) 
                    values('{fileName}','{imgPath}','{requst.GUID}','{guid_img}','{userCode}','{currDate:yyyy-MM-dd}','{currDate:HH:mm:ss}')");
                        }
                    }
                }

                DB.Commit();
                #endregion
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
        /***********************************************PDA end************************************************************/
    }

    #region Dto
    public class GetARTMDetailsDto
    {
        /// <summary>
        /// art
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// 检测类型
        /// </summary>
        public string GENERAL_TESTTYPE_NO { get; set; }
        public string GENERAL_TESTTYPE_NAME { get; set; }
        /// <summary>
        /// 检测类型类别
        /// </summary>
        public string CATEGORY_NO { get; set; }
        public string CATEGORY_NAME { get; set; }
    }

    public class SaveReinspectionReportReqDto
    {
        /// <summary>
        /// 不良数
        /// </summary>
        public int BAD_QTY { get; set; }
        /// <summary>
        /// 抽检数量
        /// </summary>
        public int SPOT_CHECK_QTY { get; set; }
        /// <summary>
        /// 不可接受数量
        /// </summary>
        public int NOT_ACCEPT_QTY { get; set; }
        /// <summary>
        /// 接受数量
        /// </summary>
        public int ACCEPT_QTY { get; set; }
        /// <summary>
        /// 通用检测类型代号
        /// </summary>
        public string GENERAL_TESTTYPE_NO { get; set; }
        /// <summary>
        /// 检测类别
        /// </summary>
        public string CATEGORY_NO { get; set; }
        public string GUID { get; set; }
        public List<SaveReinspectionReportItemReqDto> Details { get; set; }
    }

    public class SaveReinspectionReportItemReqDto
    {
        /// <summary>
        /// art
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// 通用检测类型代号
        /// </summary>
        public string GENERAL_TESTTYPE_NO { get; set; }
        /// <summary>
        /// 通用检测类型名称
        /// </summary>
        public string GENERAL_TESTTYPE_NAME { get; set; }
        /// <summary>
        /// 检测类别
        /// </summary>
        public string CATEGORY_NO { get; set; }
        /// <summary>
        /// 检测类别名称
        /// </summary>
        public string CATEGORY_NAME { get; set; }
        /// <summary>
        /// 检验类别
        /// </summary>
        public string TESTITEM_CATEGORY { get; set; }
        /// <summary>
        /// 检测项编号
        /// </summary>
        public string TESTITEM_CODE { get; set; }
        /// <summary>
        /// 检测项名称
        /// </summary>
        public string TESTITEM_NAME { get; set; }
        /// <summary>
        /// 检验项类型
        /// </summary>
        public string TESTTYPE_NO { get; set; }
        /// <summary>
        /// 检验项类型名称
        /// </summary>
        public string TESTTYPE_NAME { get; set; }
        /// <summary>
        /// 试验数量(检验数量)
        /// </summary>
        public int? SAMPLE_NUM { get; set; }
        /// <summary>
        /// 判断标准
        /// </summary>
        public string T_CHECK_ITEM { get; set; }
        public string T_CHECK_VALUE { get; set; }
        /// <summary>
        /// AQL级别
        /// </summary>
        public string AQL_LEVEL { get; set; }
        public string AC { get; set; }
        public string RE { get; set; }
        /// <summary>
        /// 检验结果 0:false;1:true;
        /// </summary>
        public string INS_RES { get; set; }
        /// <summary>
        /// 不良数
        /// </summary>
        public int BAD_QTY { get; set; }
        /// <summary>
        /// 问题点
        /// </summary>
        public string PROBLEM_POINT { get; set; }
        /// <summary>
        /// 是否退库 0:false;1:true;
        /// </summary>
        public string WH_RETURN { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARK { get; set; }
        /// <summary>
        /// 图片集合
        /// </summary>
        public List<string> IMG_LIST { get; set; }
    }

    public class GetReinspectionReportDetailResDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string OUTSOURCING_INSPECTION_NO { get; set; }
        /// <summary>
        /// 厂商代号
        /// </summary>
        public string SUPPLIERS_CODE { get; set; }
        /// <summary>
        /// 厂商名称
        /// </summary>
        public string SUPPLIERS_NAME { get; set; }
        /// <summary>
        /// 鞋型
        /// </summary>
        public string SHOE_NO { get; set; }
        /// <summary>
        /// PO号
        /// </summary>
        public string PO_ORDER { get; set; }
        /// <summary>
        /// ART
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// 进仓数
        /// </summary>
        public int WH_QTY { get; set; }
        /// <summary>
        /// 抽检数量
        /// </summary>
        public int SPOT_CHECK_QTY { get; set; }
        /// <summary>
        /// 通用标准
        /// </summary>
        public string GENERAL_TESTTYPE_NO { get; set; }
        /// <summary>
        /// 通用标准名称
        /// </summary>
        public string GENERAL_TESTTYPE_NAME { get; set; }
        /// <summary>
        /// 检测项
        /// </summary>
        public string CATEGORY_NO { get; set; }
        /// <summary>
        /// 检测项名称
        /// </summary>
        public string CATEGORY_NAME { get; set; }
        /// <summary>
        /// 不良数
        /// </summary>
        public int BAD_QTY { get; set; }
        /// <summary>
        /// 不良率
        /// </summary>
        public string BAD_RATE { get; set; }
        /// <summary>
        /// 不可接受数量
        /// </summary>
        public int NOT_ACCEPT_QTY { get; set; }
        /// <summary>
        /// 接受数量
        /// </summary>
        public int ACCEPT_QTY { get; set; }
        public string GUID { get; set; }
        public string CREATEDATE { get; set; }
        public List<GetReinspectionReportDetailItemResDto> Details { get; set; }
    }

    public class GetReinspectionReportDetailItemResDto: SaveReinspectionReportItemReqDto
    {
        public string GUID_IMG { get; set; }
    }
    #endregion
}
