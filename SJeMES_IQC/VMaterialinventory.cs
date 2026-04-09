
using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SJeMES_IQC
{
    public class VMaterialinventory 
    {
        //字典去null为空
        private static void NullKeyValue(Dictionary<string, object> dic)
        {
            string[] keys = dic.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                if (dic[keys[i]] == null)
                {
                    dic[keys[i]] = "";
                }
            }
        }
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
                string DETERMINE = jarr.ContainsKey("DETERMINE") ? jarr["DETERMINE"].ToString() : "";//状态
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                DateTime datas = DateTime.Now;
                var sql = string.Empty;
                string strwhere = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
               
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                    //strwhere += $@" and a.RCPT_DATE BETWEEN TO_date(@putin_date,'yyyy-MM-dd') and TO_date(@end_date,'yyyy-MM-dd')";
                    //dic.Add("putin_date", putin_date);
                    //dic.Add("end_date", end_date);
                }
                if (!string.IsNullOrWhiteSpace(CHK_NO))
                {
                    strwhere += $@" and a.CHK_NO like '%{CHK_NO}%'";
                    //strwhere += $@" and a.CHK_NO like @CHK_NO";
                    //dic.Add("CHK_NO",$"%{CHK_NO}%");
                }
                if (!string.IsNullOrWhiteSpace(VEND_NO))
                {
                    strwhere += $@" and b.SUPPLIERS_NAME like '%{ VEND_NO}%' ";
                    //strwhere += $@" and b.SUPPLIERS_NAME like @BSUPPLIERS_NAME";
                    //dic.Add("BSUPPLIERS_NAME", $@"%{VEND_NO}%");
                }
                if (!string.IsNullOrWhiteSpace(VEND_NO2))
                {
                    strwhere += $@" and b1.SUPPLIERS_NAME like '%{VEND_NO2}%'";
                    //strwhere += $@" and b1.SUPPLIERS_NAME like @BLSUPPLIERS_NAME";
                    //dic.Add("BLSUPPLIERS_NAME", $@"%{VEND_NO2}%");
                }
                if (!string.IsNullOrWhiteSpace(bianma))
                {
                    strwhere += $@" and a1.ITEM_NO like '%{bianma}%'";
                    //strwhere += $@" and a1.ITEM_NO like @ITEM_NO";
                    //dic.Add("ITEM_NO", $@"%{bianma}%");
                }
                if (!string.IsNullOrWhiteSpace(ORG_ID))
                {
                    strwhere += $@" and A.ORG_ID ='{ORG_ID}'";
                    //strwhere += $@" and A.ORG_ID =@ORG_ID";
                    //dic.Add("ORG_ID", ORG_ID);//工厂
                }
                if (!string.IsNullOrWhiteSpace(STOC_NO))
                {
                    strwhere += $@" and a.STOC_NO = '{STOC_NO}'";
                    //strwhere += $@" and a.STOC_NO = @STOC_NO";
                    //dic.Add("STOC_NO", STOC_NO);//仓别
                }
                //if (DETERMINE == "未结案")
                if (DETERMINE == "opencase")
                {
                    strwhere += $@" and  rt.closing_status ='1'";
                    //strwhere += $@" and  rt.closing_status =@closing_status";
                    //dic.Add("closing_status","1");
                }
                //else if (DETERMINE == "已结案")
                else if (DETERMINE == "Closed")
                {
                    strwhere += $@" and rt.closing_status ='0'";
                    //strwhere += $@" and rt.closing_status =@closing_status";
                    //dic["closing_status"]="0";
                }
                //else if (DETERMINE == "无")
                else if (DETERMINE == "none")
                {
                    strwhere += $@" and rt.closing_status is null";
                }
                
                if (jieguo == "Qualified")
                {
                    strwhere += $@" and EX.TEST_RESULT ='PASS'";
                    //strwhere += $@" and EX.TEST_RESULT =@TEST_RESULT";
                    //dic.Add("TEST_RESULT", "PASS");
                }
                //else if (jieguo == "不合格")
                else if (jieguo == "Unqualified")
                {
                     strwhere += $@" and EX.TEST_RESULT = 'FAIL'";
                    //dic["TEST_RESULT"] = "FAIL";
                }
                if (wgjieguo=="Qualified")
                {
                    strwhere += $@" and s.DETERMINE ='0'";
                    //strwhere += $@" and s.DETERMINE =@DETERMINE";
                    //dic.Add("DETERMINE", "0");
                }
                //else if (wgjieguo == "不合格")
                else if (wgjieguo == "Unqualified")
                {
                    strwhere += $@" and s.DETERMINE ='1'";
                    //dic["DETERMINE"] = "1";
                }
                if (!string.IsNullOrWhiteSpace(quyang))
                {
                    strwhere += $@" AND A1.SAMPLING_STATUS='{quyang}'";
                    //strwhere += $@" AND A1.SAMPLING_STATUS=@SAMPLING_STATUS";
                    //dic.Add("SAMPLING_STATUS", quyang);
                }
                sql = $@"SELECT
    DISTINCT
    B.INSERT_DATE
FROM
    WMS_RCPT_M A
LEFT JOIN WMS_RCPT_D B ON A.CHK_NO = B.CHK_NO
WHERE A.CHK_NO ='{CHK_NO}' AND ROWNUM<3";
                DataTable dt_desc = DB.GetDataTable(sql);
                string str_desc = string.Empty;
                if (dt_desc.Rows.Count < 3)
                {
                    str_desc = $@",MAX(a1.ITEM_NO) desc";//出现多个相同的添加时间，做多一个排序的条件
                }

                string searchKeys = $@"
	MAX(a.RCPT_DATE) RCPT_DATE, --收货日期
    MAX(b.SUPPLIERS_CODE) AS SUPPLIERS_CODE, --生产厂商编号
	MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME, --生产厂商
	MAX(b1.SUPPLIERS_CODE) as SUPPLIERS_CODE2, --采购厂商编号
	MAX(b1.SUPPLIERS_NAME) as SUPPLIERS_NAME2, --采购厂商
      MAX(a.CHK_NO) as CHK_NO,--收料单号
    MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
    MAX(aw.NAME_T) as NAME_T, --材料名称
    '' as SHDW,--收货单位
    SUM(c111.ORD_QTY) as ORD_QTY, --采购数量
    MAX(a1.source_no) as ORDER_NO, --采购单号
    MAX(s.INSPECTIONDATE) as CREATEDATE,--外观检验日期
    MAX(NVL(s.DETERMINE,2)) as DETERMINE,--外观结果
    (
    CASE WHEN MAX(A1.SAMPLING_STATUS)='I' THEN '历史数据'
    ELSE
      MAX(a1.SAMPLING_STATUS) 
    END
  ) SAMPLING_STATUS,-- 测试取样状况
    '' as SYSCE_DATE,--实验室测试日期	
    	'' as TEST_RESULT,--测试结果 
	MAX(hm.staff_name) as staff_name,--检验员名称
MAX(a1.RCPT_QTY) as IV_QTY, --检验数    
MAX(s.PASS_QTY) as PASS_QTY, --合格数
MAX(rt.BAD_QTY) as BAD_QTY,--(不良数量)不合格数量
 MAX(rt.SPC_MINING) AS SPC_MINING,--特采数量
MAX(rt.actual_returned_qty) AS YTS_QTY,--实退退数
MAX(rt.supplementary_delivery_qty) as BS,--补送
''as NAME_S2, --鞋型名称
'' as PROD_NO,--ART
MAX(m1.ORG_NAME) as ORG_NAME, --工厂名称
 MAX(MM.WAREHOUSE_NAME) AS WAREHOUSE_NAME,--仓库名称
MAX(rt.closing_status) as closing_status,--结案状态


	MAX(a.INSERT_DATE) as datas, 
	MAX(a1.SOURCE_SEQ) as SOURCE_SEQ, --来源单序号
	a1.CHK_SEQ,
	'' as PART_NO,--部位
	'' as SHOE_NO, --鞋型
	MAX(a1.RCPT_QTY) AS RCPT_QTY, --收料数量
	MAX(pe.ITEM_TYPE_NO) as ITEM_TYPE_NO, --物料类型
MAX(s.staff_no) as CREATEBY, --检验员
	MAX(nvl(s.sample_qty,0)) as sample_qty,
	MAX(s.DETERMINE) as cors,
	MAX(s.isdelete) as sdisdelete,
    MAX(EX.ID) as EX_ID,-- 实验室任务ID
	MAX(a.INSERT_DATE) as INSERT_DATE,
	MAX(a.STOC_NO) as STOC_NO, --仓库代号
	MAX(nvl(rt.isdelete,0)) as isdelete --是否已经删除
";
                sql = $@"
SELECT
	searchKeys
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO
LEFT JOIN base003m b ON b.SUPPLIERS_CODE = a.VEND_NO
INNER JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT JOIN base003m b1 ON b1.SUPPLIERS_CODE = a2.VEND_NO
LEFT JOIN bdm_rd_item aw on aw.ITEM_NO=a1.ITEM_NO
LEFT JOIN BDM_RD_ITEMTYPE pe on  pe.ITEM_TYPE_NO=AW.ITEM_TYPE
LEFT JOIN bdm_rd_prod pd on pd.PROD_NO=aw.PARENT_ITEM_NO
LEFT JOIN base001m m1 on m1.ORG_CODE=a.ORG_ID
LEFT JOIN qcm_iqc_insp_res_m s on s.CHK_NO=a.CHK_NO and s.CHK_SEQ=a1.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on rt.CHK_NO=a.CHK_NO and rt.CHK_SEQ=a1.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON c111.ORDER_NO=a1.SOURCE_NO AND c111.ORDER_SEQ=a1.CHK_SEQ 
LEFT JOIN qcm_ex_task_list_m EX ON (EX.MATERIAL_CODE=a1.ITEM_NO AND instr(EX.SLDH, a1.CHK_NO) > 0)
LEFT JOIN HR001M hm on hm.staff_no=s.staff_no 
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
WHERE  a.RCPT_BY='01' {strwhere}
GROUP BY a1.CHK_NO,a1.CHK_SEQ 
 order by MAX(a.INSERT_DATE) desc{str_desc}
";
        
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"));

                DataTable dtrow = new DataTable();
                string name_t = string.Empty;
                string[] arr = null;
                List<string> qcm_ex_task_list_m_Ids = new List<string>();
                foreach (DataRow item in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(item["EX_ID"].ToString()))
                    {
                        qcm_ex_task_list_m_Ids.Add(item["EX_ID"].ToString());
                    }
                    if (!string.IsNullOrWhiteSpace(item["ORDER_NO"].ToString()))
                    {
                        dic = new Dictionary<string, object>();
                        string sql2 = $@"
SELECT
	aa.ORDER_NO,
	--ee.SALES_ORDER,
    ee.ORDER_SEQ,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ee.ART_NO", "ee.ART_NO")} as ART_NO,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT pd.SHOE_NO", "pd.SHOE_NO")} as SHOE_NO
FROM
	bdm_purchase_order_m aa
LEFT  JOIN bdm_purchase_order_d ee on aa.ORDER_NO=ee.ORDER_NO 
LEFT JOIN bdm_rd_prod pd on ee.ART_NO=pd.PROD_NO
where aa.ORDER_NO=@ORDER_NO AND ee.ORDER_SEQ=@ORDER_SEQ group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dic.Add("ORDER_NO", item["ORDER_NO"]);
                        dic.Add("ORDER_SEQ", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql2,dic);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        if (dtrow.Rows.Count > 0)
                        {
                            item["SHOE_NO"] = dtrow.Rows[0]["SHOE_NO"].ToString();
                            item["PROD_NO"] = dtrow.Rows[0]["ART_NO"].ToString();
                           
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                //包含就多条（多条带 ,）
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    name_t = string.Empty;
                                    arr = dtrow.Rows[0]["SHOE_NO"].ToString().Split(',');
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {
                                            name_t += dtrow.Rows[i]["NAME_T"].ToString() + "、";
                                        }
                                    }
                                   
                                }
                                else
                                {
                                    name_t = string.Empty;
                                    dic = new Dictionary<string, object>();
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO=@SHOE_NO";
                                    dic.Add("SHOE_NO", dtrow.Rows[0]["SHOE_NO"]);
                                    dtrow = DB.GetDataTable(sql,dic);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        name_t = dtrow.Rows[0]["NAME_T"].ToString();  
                                    }
                                }
                               
                            }
                            item["NAME_S2"] = name_t; 
                        }
                        dic = new Dictionary<string, object>();

                        sql2 = $@"
select 
	PART_NO,
    PR_UNIT
from bdm_purchase_order_item  
where order_no ='{item["ORDER_NO"]}' and order_seq ='{item["SOURCE_SEQ"]}'
";
                        //dic.Add("order_no", item["ORDER_NO"]);
                        //dic.Add("order_seq", Convert.ToInt32(item["SOURCE_SEQ"].ToString()));
                        dtrow = DB.GetDataTable(sql2);
                        if (dtrow.Rows.Count > 0)
                        {
                            item["PART_NO"] = dtrow.Rows[0]["PART_NO"].ToString();
                            item["SHDW"] = dtrow.Rows[0]["PR_UNIT"].ToString();
                        }
                    }
                }
                if (qcm_ex_task_list_m_Ids.Count > 0)
                {
                    qcm_ex_task_list_m_Ids = qcm_ex_task_list_m_Ids.Distinct().ToList();
                    var ex_dt = DB.GetDataTable($@"SELECT ID,SUBMISSION_DATE,TEST_RESULT FROM QCM_EX_TASK_LIST_M WHERE ID IN({string.Join(",", qcm_ex_task_list_m_Ids)})");
                    foreach (DataRow item in dt.Rows)
                    {
                        string curr_id = item["EX_ID"].ToString();

                        if (!string.IsNullOrEmpty(curr_id))
                        {
                            var findRows = ex_dt.Select($@"ID={curr_id}");
                            if (findRows.Count() > 0)
                            {
                                DataRow findRow = findRows[0];
                                string ex_id = findRow["ID"].ToString();
                                if(curr_id == ex_id)
                                {
                                    object SUBMISSION_DATE = findRow["SUBMISSION_DATE"].ToString();
                                    object TEST_RESULT = findRow["TEST_RESULT"].ToString();
                                    item["SYSCE_DATE"] = SUBMISSION_DATE;
                                    item["TEST_RESULT"] = TEST_RESULT;
                                }
                            }
                        }
                    }
                }
                dic = new Dictionary<string, object>();
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
        /// 主页数据（优化过的）
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMain2(object OBJ)
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
                string PURCHASE_NO = jarr.ContainsKey("PURCHASE_NO") ? jarr["PURCHASE_NO"].ToString() : "";//采购单号
                string jieguo = jarr.ContainsKey("jieguo") ? jarr["jieguo"].ToString() : "";//物性结果
                string quyang = jarr.ContainsKey("quyang") ? jarr["quyang"].ToString() : "";//取样状况
                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//采购厂商
                string VEND_NO2 = jarr.ContainsKey("VEND_NO2") ? jarr["VEND_NO2"].ToString() : "";//生产厂商
                string bianma = jarr.ContainsKey("bianma") ? jarr["bianma"].ToString() : "";//物料编码  
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//仓别
                string wgjieguo = jarr.ContainsKey("wgjieguo") ? jarr["wgjieguo"].ToString() : "";//外观结果
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//工厂
                string DETERMINE = jarr.ContainsKey("DETERMINE") ? jarr["DETERMINE"].ToString() : "";//状态(所有，已录入，未录入)
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string Lab = jarr.ContainsKey("Lab") ? jarr["Lab"].ToString() : ""; //premika-2025/11/28
                //string Artno = jarr.ContainsKey("Lab") ? jarr["Artno"].ToString() : ""; //premika-2025/12/22
                DateTime datas = DateTime.Now;
                var sql = string.Empty;
                string strwhere = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                //string rcpt_no = "";
                //string mat_no = "";
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and a.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                }

                //premika-start-2025/12/05
                //if (!string.IsNullOrEmpty(Lab))
                //{

                //    string sql2 = $@"
                //            SELECT
                //               SLDH,MAKINGS_ID
                //            FROM
                //                QCM_EX_TASK_LIST_M 
                //            WHERE
                //                TASK_NO ='{Lab}'";
                //    if (!string.IsNullOrWhiteSpace(CHK_NO))
                //    {
                //        sql2 += $@" and SLDH like '%{CHK_NO}%'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(bianma))
                //    {
                //        sql2 += $@" and MAKINGS_ID like '%{bianma}%'";
                //    }


                //    DataTable sys_dt1 = DB.GetDataTable(sql2);
                //    // Check if table has rows
                //    if (sys_dt1 != null && sys_dt1.Rows.Count > 0)
                //    {
                //        rcpt_no = sys_dt1.Rows[0]["SLDH"]?.ToString() ?? "";
                //        mat_no = sys_dt1.Rows[0]["MAKINGS_ID"]?.ToString() ?? "";
                //    }
                //    else
                //    {

                //            ret.IsSuccess = false;
                //            ret.ErrMsg = $@"No Data Found";
                //            return ret;

                //    }


                //    string formatted = string.Join(",",
                //                        rcpt_no.Split(',')
                //                               .Select(x => $"'{x}'")); 

                //    if (!string.IsNullOrWhiteSpace(formatted))
                //    {
                //        strwhere += $@" and a.CHK_NO in ({formatted})";
                //    }
                //    if (!string.IsNullOrWhiteSpace(mat_no))
                //    {
                //        strwhere += $@" and a1.ITEM_NO like '%{mat_no}%'";
                //    }

                //}
                //else
                //{
                //    if (!string.IsNullOrWhiteSpace(CHK_NO))
                //    {
                //        strwhere += $@" and a.CHK_NO like '%{CHK_NO}%'";
                //    }
                //    if (!string.IsNullOrWhiteSpace(bianma))
                //    {
                //        strwhere += $@" and a1.ITEM_NO like '%{bianma}%'";
                //    }
                //}
                //premika-end-2025/12/05

                //premika-start-2025/12/22
                //if (!string.IsNullOrWhiteSpace(Artno))
                //{
                //    strwhere += $@" and z1.art_no='{Artno}'";
                //}
                //premika-end-2025/12/22
                if (!string.IsNullOrWhiteSpace(CHK_NO))
                {
                    strwhere += $@" and a.CHK_NO like '%{CHK_NO}%'";
                }
                if (!string.IsNullOrWhiteSpace(bianma))
                {
                    strwhere += $@" and a1.ITEM_NO like '%{bianma}%'";
                }
                if (!string.IsNullOrWhiteSpace(PURCHASE_NO))
                {
                    strwhere += $@" and a1.source_no like '%{PURCHASE_NO}%'";
                }
                if (!string.IsNullOrWhiteSpace(VEND_NO))//采购厂商
                {
                    strwhere += $@" and b1.SUPPLIERS_NAME like '%{VEND_NO}%' ";
                }
                if (!string.IsNullOrWhiteSpace(VEND_NO2))//生产厂商
                {
                    //strwhere += $@" and b.SUPPLIERS_NAME like '%{VEND_NO2}%'"; OLD
                    strwhere += $@" and b.SUPPLIERS_CODE like '%{VEND_NO2}%'";
                }
               
                if (!string.IsNullOrWhiteSpace(ORG_ID))
                {
                    strwhere += $@" and A.ORG_ID ='{ORG_ID}'";
                }
                if (!string.IsNullOrWhiteSpace(STOC_NO))
                {
                    strwhere += $@" and a.STOC_NO = '{STOC_NO}'";
                }

                //if (DETERMINE == "已检验")
                if (DETERMINE == "Checked")
                {
                    strwhere += $@" and   (s.DETERMINE='0' or s.DETERMINE='1') ";
                }
                //else if (DETERMINE == "未检验")
                else if (DETERMINE == "Not tested")
                {
                    strwhere += $@" and  NVL(s.DETERMINE,2)='2'";
                }
                if (!string.IsNullOrWhiteSpace(jieguo))
                {

                    DataTable exRes = DB.GetDataTable($@"
SELECT ID,SLDH,TEST_RESULT,MAKINGS_ID FROM QCM_EX_TASK_LIST_M WHERE SLDH IS NOT NULL AND MAKINGS_ID IS NOT NULL
");
                    if (exRes == null || exRes.Rows.Count == 0)
                    {
                        strwhere += $@" and 1=2 ";
                    }
                    else
                    {
                        List<string> sldh_makings_list = new List<string>();
                        foreach (DataRow item in exRes.Rows)
                        {
                            string SLDH = item["SLDH"].ToString();
                            string MAKINGS_ID = item["MAKINGS_ID"].ToString();

                            var SLDH_arr = SLDH.Split(',').ToList();
                            foreach (var SLDH_item in SLDH_arr)
                            {
                                if (!string.IsNullOrEmpty(SLDH_item))
                                {
                                    sldh_makings_list.Add($@"{SLDH_item}-{MAKINGS_ID}");
                                }
                            }
                        }

                        sldh_makings_list = sldh_makings_list.Distinct().ToList();

                        Dictionary<string, string> sldh_res = new Dictionary<string, string>();
                        foreach (string sldhItem in sldh_makings_list)
                        {
                            var sldhItem_arr = sldhItem.Split('-');
                            var findExRows = exRes.Select($@"SLDH like '%{sldhItem_arr[0]}%' and MAKINGS_ID='{sldhItem_arr[1]}'");
                            if (findExRows != null && findExRows.Length > 0)
                            {
                                var findResRow = findExRows.OrderByDescending(x => Convert.ToInt32(x["ID"].ToString())).FirstOrDefault();
                                if (findResRow != null)
                                {
                                    sldh_res.Add(sldhItem, findResRow["TEST_RESULT"].ToString());
                                }
                            }
                        }

                        //if (jieguo == "合格")
                        if (jieguo == "Qualified")
                        {
                            sldh_res = sldh_res.Where(x => x.Value == "PASS").ToDictionary(x => x.Key, y => y.Value);
                        }
                        else if (jieguo == "Unqualified")
                        {
                            sldh_res = sldh_res.Where(x => x.Value == "FAIL").ToDictionary(x => x.Key, y => y.Value);
                        }

                        if (sldh_res == null || sldh_res.Count() == 0)
                        {
                            strwhere += $@" and 1=2 ";
                        }
                        else
                        {
                            strwhere += $@" and ({string.Join(" or ", sldh_res.Select(x => $@"(a.CHK_NO ='{x.Key.Split('-')[0]}' and a1.ITEM_NO='{x.Key.Split('-')[1]}')"))})";
                        }
                    }
                }
                //if (wgjieguo == "合格")
                if (wgjieguo == "Qualified")
                {
                    strwhere += $@" and s.DETERMINE ='0'";
                }
               // else if (wgjieguo == "不合格")
                else if (wgjieguo == "Unqualified")
                {
                    strwhere += $@" and s.DETERMINE ='1'";
                }
                if (!string.IsNullOrWhiteSpace(quyang))
                {
                    strwhere += $@" AND A1.SAMPLING_STATUS='{quyang}'";
                }
                sql = $@"SELECT
    DISTINCT
    B.INSERT_DATE
FROM
    WMS_RCPT_M A
LEFT JOIN WMS_RCPT_D B ON A .CHK_NO = B.CHK_NO
WHERE
   a.RCPT_BY='01'
AND A.CHK_NO ='{CHK_NO}' AND ROWNUM<3";
                DataTable dt_desc = DB.GetDataTable(sql);
                string str_desc = string.Empty;
                if (dt_desc.Rows.Count < 3)
                {
                    str_desc = $@",MAX(a1.ITEM_NO) desc";//出现多个相同的添加时间，做多一个排序的条件
                }
                string searchKeys = $@"
	MAX(a.RCPT_DATE) RCPT_DATE, --收货日期
   MAX(a.vend_no) AS SUPPLIERS_CODE, --生产厂商编号
	MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME, --生产厂商
	MAX(b1.SUPPLIERS_CODE) as SUPPLIERS_CODE2, --采购厂商编号
	MAX(b1.SUPPLIERS_NAME) as SUPPLIERS_NAME2, --采购厂商
  MAX(a.CHK_NO) as CHK_NO,--收料单号
  MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
  MAX(aw.NAME_T) as NAME_T, --材料名称
  MAX(c111.PR_UNIT) as SHDW,--收货单位
  SUM(c111.ORD_QTY) as ORD_QTY, --采购数量
  MAX(a1.source_no) as ORDER_NO, --采购单号
  MAX(s.INSPECTIONDATE) as CREATEDATE,--外观检验日期
  MAX(NVL(s.DETERMINE,2)) as DETERMINE,--外观结果
  (
  CASE WHEN MAX(A1.SAMPLING_STATUS)='I' THEN '历史数据'
  ELSE
  MAX(a1.SAMPLING_STATUS) 
  END
  ) SAMPLING_STATUS,-- 测试取样状况

  '' as SYSCE_DATE,--实验室测试日期	
  '' as TEST_RESULT,--测试结果 

	MAX(hm.staff_name) as staff_name,--检验员名称
	MAX(a1.RCPT_QTY) as IV_QTY, --检验数    
	MAX(s.PASS_QTY) as PASS_QTY, --合格数
	MAX(rt.BAD_QTY) as BAD_QTY,--(不良数量)不合格数量
	MAX(rt.SPC_MINING) AS SPC_MINING,--特采数量
	MAX(rt.actual_returned_qty) AS YTS_QTY,--实退退数
	MAX(rt.supplementary_delivery_qty) as BS,--补送
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z2.NAME_T", "z2.NAME_T")} as NAME_S2,--鞋型名称
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z1.ART_NO", "z1.ART_NO")} as PROD_NO, --ART
    -- '' as NAME_S2,--鞋型名称
    -- '' as PROD_NO, --ART
    MAX(a.ORG_ID) AS ORG_ID,--工厂编号
	MAX(m1.ORG_NAME) as ORG_NAME, --工厂名称
	MAX(MM.WAREHOUSE_NAME) AS WAREHOUSE_NAME,--仓库名称
	MAX(rt.closing_status) as closing_status,--结案状态
	MAX(a.INSERT_DATE) as datas, 
	MAX(a1.SOURCE_SEQ) as SOURCE_SEQ, --来源单序号
	MAX(a1.CHK_SEQ) as CHK_SEQ,
	MAX(c111.PART_NO) as PART_NO,--部位
	-- listagg(DISTINCT z1.ART_NO, ',') WITHIN GROUP (ORDER BY z1.ART_NO)  as SHOE_NO, --鞋型
	'' as SHOE_NO, --鞋型
	MAX(a1.RCPT_QTY) AS RCPT_QTY, --收料数量
	MAX(pe.ITEM_TYPE_NO) as ITEM_TYPE_NO, --物料类型
	MAX(s.staff_no) as CREATEBY, --检验员
	MAX(nvl(s.sample_qty,0)) as sample_qty,
	MAX(s.DETERMINE) as cors,
	MAX(s.isdelete) as sdisdelete,
  '' as EX_ID,-- 实验室任务ID
	MAX(a.INSERT_DATE) as INSERT_DATE,
	MAX(a.STOC_NO) as STOC_NO, --仓库代号
    '' as task_no
";

                sql = $@"
SELECT
	searchKeys
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO

INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT  JOIN bdm_purchase_order_d z1 on a2.ORDER_NO=z1.ORDER_NO and a1.SOURCE_SEQ=z1.ORDER_SEQ --新加的
LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
LEFT JOIN base003m b1 ON a2.VEND_NO=b1.SUPPLIERS_CODE

LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT JOIN base003m b ON a.vend_no=b.SUPPLIERS_CODE
LEFT JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT JOIN base001m m1 on a.ORG_ID=m1.ORG_CODE
LEFT JOIN qcm_iqc_insp_res_m s on a.CHK_NO=s.CHK_NO and a1.CHK_SEQ=s.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on a.CHK_NO=rt.CHK_NO and a1.CHK_SEQ=rt.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON a1.SOURCE_NO=c111.ORDER_NO AND a1.CHK_SEQ=c111.ORDER_SEQ
-- LEFT JOIN qcm_ex_task_list_m EX ON (a1.ITEM_NO=EX.MATERIAL_CODE AND instr(EX.SLDH, a1.CHK_NO) > 0)
-- LEFT JOIN qcm_ex_task_list_m EX ON a1.ITEM_NO = EX.makings_id and EX.sldh = a1.CHK_NO
LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
WHERE  a.RCPT_BY='01' {strwhere}
GROUP BY a1.CHK_NO,a1.CHK_SEQ 
 order by MAX(a.INSERT_DATE) desc{str_desc}
";
                //commented by premika-2025/11/28

                //                string searchKeys = $@"
                //	MAX(a.RCPT_DATE) RCPT_DATE, --收货日期
                //   MAX(a.vend_no) AS SUPPLIERS_CODE, --生产厂商编号
                //	MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME, --生产厂商
                //	MAX(b1.SUPPLIERS_CODE) as SUPPLIERS_CODE2, --采购厂商编号
                //	MAX(b1.SUPPLIERS_NAME) as SUPPLIERS_NAME2, --采购厂商
                //  MAX(a.CHK_NO) as CHK_NO,--收料单号
                //  MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
                //  MAX(aw.NAME_T) as NAME_T, --材料名称
                //  MAX(c111.PR_UNIT) as SHDW,--收货单位
                //  SUM(c111.ORD_QTY) as ORD_QTY, --采购数量
                //  MAX(a1.source_no) as ORDER_NO, --采购单号
                //  MAX(s.INSPECTIONDATE) as CREATEDATE,--外观检验日期
                //  MAX(NVL(s.DETERMINE,2)) as DETERMINE,--外观结果
                //  (
                //  CASE WHEN MAX(A1.SAMPLING_STATUS)='I' THEN '历史数据'
                //  ELSE
                //  MAX(a1.SAMPLING_STATUS) 
                //  END
                //  ) SAMPLING_STATUS,-- 测试取样状况

                //  '' as SYSCE_DATE,--实验室测试日期	
                //  '' as TEST_RESULT,--测试结果 

                //	MAX(hm.staff_name) as staff_name,--检验员名称
                //	MAX(a1.RCPT_QTY) as IV_QTY, --检验数    
                //	MAX(s.PASS_QTY) as PASS_QTY, --合格数
                //	MAX(rt.BAD_QTY) as BAD_QTY,--(不良数量)不合格数量
                //	MAX(rt.SPC_MINING) AS SPC_MINING,--特采数量
                //	MAX(rt.actual_returned_qty) AS YTS_QTY,--实退退数
                //	MAX(rt.supplementary_delivery_qty) as BS,--补送
                //    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z2.NAME_T", "z2.NAME_T")} as NAME_S2,--鞋型名称
                //    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT  z1.ART_NO", "z1.ART_NO")} as PROD_NO, --ART
                //    -- '' as NAME_S2,--鞋型名称
                //    -- '' as PROD_NO, --ART
                //    MAX(a.ORG_ID) AS ORG_ID,--工厂编号
                //	MAX(m1.ORG_NAME) as ORG_NAME, --工厂名称
                //	MAX(MM.WAREHOUSE_NAME) AS WAREHOUSE_NAME,--仓库名称
                //	MAX(rt.closing_status) as closing_status,--结案状态
                //	MAX(a.INSERT_DATE) as datas, 
                //	MAX(a1.SOURCE_SEQ) as SOURCE_SEQ, --来源单序号
                //	MAX(a1.CHK_SEQ) as CHK_SEQ,
                //	MAX(c111.PART_NO) as PART_NO,--部位
                //	-- listagg(DISTINCT z1.ART_NO, ',') WITHIN GROUP (ORDER BY z1.ART_NO)  as SHOE_NO, --鞋型
                //	'' as SHOE_NO, --鞋型
                //	MAX(a1.RCPT_QTY) AS RCPT_QTY, --收料数量
                //	MAX(pe.ITEM_TYPE_NO) as ITEM_TYPE_NO, --物料类型
                //	MAX(s.staff_no) as CREATEBY, --检验员
                //	MAX(nvl(s.sample_qty,0)) as sample_qty,
                //	MAX(s.DETERMINE) as cors,
                //	MAX(s.isdelete) as sdisdelete,
                //  '' as EX_ID,-- 实验室任务ID
                //	MAX(a.INSERT_DATE) as INSERT_DATE,
                //	MAX(a.STOC_NO) as STOC_NO --仓库代号
                //";

                //                sql = $@"
                //SELECT
                //	searchKeys
                //FROM
                //    wms_rcpt_m a 
                //LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO

                //INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
                //LEFT  JOIN bdm_purchase_order_d z1 on a2.ORDER_NO=z1.ORDER_NO and a1.SOURCE_SEQ=z1.ORDER_SEQ --新加的
                //LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
                //LEFT JOIN base003m b1 ON a2.VEND_NO=b1.SUPPLIERS_CODE

                //LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
                //LEFT JOIN base003m b ON a.vend_no=b.SUPPLIERS_CODE
                //LEFT JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
                //LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
                //LEFT JOIN base001m m1 on a.ORG_ID=m1.ORG_CODE
                //LEFT JOIN qcm_iqc_insp_res_m s on a.CHK_NO=s.CHK_NO and a1.CHK_SEQ=s.CHK_SEQ and s.isdelete='0' 
                //LEFT JOIN qcm_iqc_insp_res_bad_report rt on a.CHK_NO=rt.CHK_NO and a1.CHK_SEQ=rt.CHK_SEQ AND rt.isdelete='0' 
                //LEFT JOIN bdm_purchase_order_item c111 ON a1.SOURCE_NO=c111.ORDER_NO AND a1.CHK_SEQ=c111.ORDER_SEQ
                //-- LEFT JOIN qcm_ex_task_list_m EX ON (a1.ITEM_NO=EX.MATERIAL_CODE AND instr(EX.SLDH, a1.CHK_NO) > 0)
                //-- LEFT JOIN qcm_ex_task_list_m EX ON a1.ITEM_NO = EX.makings_id and EX.sldh = a1.CHK_NO
                //LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
                //LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
                //WHERE  a.RCPT_BY='01' {strwhere}
                //GROUP BY a1.CHK_NO,a1.CHK_SEQ 
                // order by MAX(a.INSERT_DATE) desc{str_desc}
                //";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"));

                if (dt != null && dt.Rows.Count > 0)
                {
                    string NAME_S2_sql = $@"
select * from bdm_rd_prod z2 where 1=2
";
                   
                    foreach (DataRow item in dt.Rows)
                    {
                       

                        var sys_dt = DB.GetDataTable($@"
                        SELECT * FROM (
                            SELECT
                        	    COMPLETION_DATE,
                        	    TEST_RESULT,
                                TASK_NO--premika add column
                            FROM
                        	    QCM_EX_TASK_LIST_M 
                            WHERE
                        	    SLDH LIKE '%{item["CHK_NO"]}%' 
                        	    AND MAKINGS_ID = '{item["ITEM_NO"]}'
                            ORDER BY
                        	    ID DESC
                        	)
                        	WHERE rownum<2
                        ");


                        if (sys_dt != null && sys_dt.Rows.Count > 0)
                        {
                            item["SYSCE_DATE"] = sys_dt.Rows[0]["COMPLETION_DATE"].ToString();
                            item["TEST_RESULT"] = sys_dt.Rows[0]["TEST_RESULT"].ToString();
                            item["TASK_NO"] = sys_dt.Rows[0]["TASK_NO"].ToString();

                        }
                        else
                        {
                            DateTime rcptDt;
                            string rcptDateSql = "";
                            if (DateTime.TryParse(item[0].ToString(), out rcptDt))
                            {
                                rcptDateSql = rcptDt.ToString("yyyy-MM-dd");
                            }
                            DateTime rcptDate = Convert.ToDateTime(rcptDt);

                            // First day of the month
                            DateTime fromDate = new DateTime(rcptDate.Year, rcptDate.Month, 1);
                            // Last day of the month
                           // DateTime toDate = fromDate.AddMonths(1).AddDays(-1);
                            // Format for SQL
                            string fromdate = fromDate.ToString("yyyy-MM-dd");
                            //string todate = toDate.ToString("yyyy-MM-dd");
                            string ITEM_TYPE_NOS = item["ITEM_NO"].ToString().Substring(0, 3);
                            if (!ITEM_TYPE_NOS.Contains("401"))
                            {
                                string sql_task = $@"
                                SELECT j.task_no,j.completion_date,j.test_result
                                  FROM qcm_ex_task_list_m j
                                 WHERE j.makings_id = '{item["ITEM_NO"]}'
                                   AND TO_DATE(
                                         REGEXP_SUBSTR(j.cl_qrcode_json,
                                                       '""rcpt_date"":""([^""]+)""',
                                                       1, 1, NULL, 1),
                                         'YYYY-MM-DD'
                                       ) =
                                       (
                                         SELECT MAX(
                                                  TO_DATE(
                                                    REGEXP_SUBSTR(x.cl_qrcode_json,
                                                                  '""rcpt_date"":""([^""]+)""',
                                                                  1, 1, NULL, 1),
                                                    'YYYY-MM-DD'
                                                  )
                                                )
                                           FROM qcm_ex_task_list_m x
                                          WHERE x.makings_id = '{item["ITEM_NO"]}'
                                            AND TO_DATE(
                                                  REGEXP_SUBSTR(x.cl_qrcode_json,
                                                                '""rcpt_date"":""([^""]+)""',
                                                                1, 1, NULL, 1),
                                                  'YYYY-MM-DD'
                                                ) between DATE '{fromdate}' and  DATE '{rcptDateSql}'
                                       )";
                                DataTable task_res = DB.GetDataTable(sql_task);
                                if (task_res.Rows.Count > 0)
                                {
                                    item["SYSCE_DATE"] = task_res.Rows[0]["COMPLETION_DATE"].ToString();
                                    item["TEST_RESULT"] = task_res.Rows[0]["TEST_RESULT"].ToString();
                                    item["TASK_NO"] = task_res.Rows[0]["TASK_NO"].ToString();
                                }
                            }
                         
            
                        }

                    }
                }
             
                int num1 = 0;
                int num2 = 0;
                string keys = jarr.ContainsKey("keys") ? jarr["keys"].ToString() : "";
                if (keys == "1")
                {
                    DataTable dat = DB.GetDataTable(sql.Replace("searchKeys", " MAX(NVL(s.DETERMINE,2)) as DETERMINE"));
                    
                    foreach (DataRow item in dat.Rows)
                    {
                        if (item["DETERMINE"].ToString() == "2")
                        {
                            num2++;//Not tested
                        }
                        else
                        {
                            num1++;//Tested

                        }
                    }
                }
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                dic.Add("num1", num1.ToString());
                dic.Add("num2", num2.ToString());
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
        /// 皮料检验报告打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMainDmp_PrintPL(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
                string cscode = jarr.ContainsKey("cscode") ? jarr["cscode"].ToString() : "";
                string stoc_no = jarr.ContainsKey("stoc_no") ? jarr["stoc_no"].ToString() : "";
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string failremark = jarr.ContainsKey("failremark") ? jarr["failremark"].ToString() : "";
                var sql = string.Empty;
                string swhere = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    swhere += $@" and m.RCPT_DATE=TO_date(@putin_date,'yyyy-MM-dd')";
                    dic.Add("putin_date", putin_date);
                }
                else if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    swhere += $@" and m.RCPT_DATE=TO_date(@end_date,'yyyy-MM-dd')";
                    dic.Add("end_date", end_date);
                }
                else if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    swhere += $@" and m.RCPT_DATE BETWEEN TO_date(@putin_date,'yyyy-MM-dd') and TO_date(@end_date,'yyyy-MM-dd')";
                    dic.Add("putin_date", putin_date);
                    dic.Add("end_date", end_date);

                }
                if (!string.IsNullOrWhiteSpace(item_no))
                {
                    swhere += $@" and d.item_no=@item_no";
                    dic.Add("item_no", item_no);
                }
                if (!string.IsNullOrWhiteSpace(cscode))
                {
                    swhere += $@" and b.SUPPLIERS_CODE=@SUPPLIERS_CODE";
                    dic.Add("SUPPLIERS_CODE", cscode);
                }
                if (!string.IsNullOrWhiteSpace(stoc_no))
                {
                    swhere += $@" and (m.stoc_no=@stoc_no or  m.org_id =@org_id)";
                    dic.Add("stoc_no", stoc_no);
                    dic.Add("org_id", stoc_no);
                }
                // aa={CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT bb.SALES_ORDER", "bb.SALES_ORDER")} AS 销售订单号,



                sql = $@"
                select
                ''  AS 销售订单号,
                '{putin_date}～{end_date}' as 寄存日期,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ('['|| m.CHK_NO||'/'||d.RCPT_QTY || ']')", "m.CHK_NO")} AS 收料单号和数量,
                '' as 退货原因,
                '' as 厂商回复,
                '' as qip总检,
                '' as 会签,
                '' as qip材料助理,
                '' as 检验员,
                to_char(SYSDATE, 'yyyy/MM/dd') AS 日期,
                sum(d.RCPT_QTY ) as 寄存数量,
                sum(d.RCPT_QTY ) as 检验数量,
                CASE
                  WHEN (sum(d.RCPT_QTY ) > 0) THEN
                    (round((sum(d.RCPT_QTY )/ sum(d.RCPT_QTY )),3)*100)
                      ELSE
                    0
                END AS 抽检率,
                '' as 退货数量,
                '' AS 退货率,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.chk_no", "m.chk_no")} as 收料单号,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.SUPPLIERS_NAME", "b.SUPPLIERS_NAME")} AS 供应厂商名字,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT d.item_no", "d.item_no")} as 材料号,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT e.name_t", "e.name_t")} as  材料名字,
                {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.source_no", "m.source_no")} AS 订单号,
                ('{failremark}') AS 不合格证明,
                -- wm_concat(DISTINCT ( GF_ARTNO(m.source_no, i.order_seq)|| GF_Shoe_Name(GF_ARTNO(m.source_no, i.order_seq)))) as ArtNo和鞋型
                '' as ArtNo和鞋型
                 FROM wms_rcpt_m m
                                        LEFT JOIN wms_rcpt_d d
                                           ON m.chk_no = d.chk_no
                                        LEFT JOIN bdm_purchase_order_item i
                                           ON d.source_no = i.order_no
                                          AND d.chk_seq = i.order_seq
                                        LEFT JOIN bdm_rd_item e
                                           ON e.item_no = d.item_no
                                        LEFT JOIN base003m b
                                           ON e.vend_no_prd=b.SUPPLIERS_CODE
                                         	LEFT  JOIN BDM_RD_ITEMTYPE pe on  e.ITEM_TYPE=pe.ITEM_TYPE_NO
                                         LEFT JOIN iqc_chk_m f
                                           ON f.chk_no = d.chk_no
                                          AND f.chk_seq = d.chk_seq
                                          AND d.item_no = f.src_item_no
                						LEFT   JOIN qcm_iqc_insp_res_m mm on  d.item_no=mm.item_no and d.chk_no=mm.chk_no and d.chk_seq=mm.chk_seq
                	where  m.RCPT_BY='01' {swhere}
                ";    //COMMENTED BY ASHOK AND CHINESE WORDS CHANGED INTO ENGLISH.




                //sql = $@"
                //select
                //''  AS Sales_order_number,
                //'{putin_date}～{end_date}' as Deposit_date,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ('['|| m.CHK_NO||'/'||d.RCPT_QTY || ']')", "m.CHK_NO")} AS Receipt_number_and_quantity,
                //'' as Reasons_for_return,
                //'' as Manufacturer_reply,
                //'' as QIP_general_inspection,
                //'' as Countersign,
                //'' as QIP_material_assistant,
                //'' as Inspectors,
                //to_char(SYSDATE, 'yyyy/MM/dd') AS Today,
                //sum(d.RCPT_QTY ) as Number_of_deposits,
                //sum(d.RCPT_QTY ) as Inspection_Quantity,
                //CASE
                //  WHEN (sum(d.RCPT_QTY ) > 0) THEN
                //    (round((sum(d.RCPT_QTY )/ sum(d.RCPT_QTY )),3)*100)
                //      ELSE
                //    0
                //END AS Sampling_Rate,
                //'' as Return_Quantity,
                //'' AS Return_Rate,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.chk_no", "m.chk_no")} as Receipt_Number,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.SUPPLIERS_NAME", "b.SUPPLIERS_NAME")} AS Supplier_Name,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT d.item_no", "d.item_no")} as Material_Number,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT e.name_t", "e.name_t")} as  Material_Name,
                //{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.source_no", "m.source_no")} AS Order_Number,
                //('{failremark}') AS Certificate_of_Failure,
                //-- wm_concat(DISTINCT ( GF_ARTNO(m.source_no, i.order_seq)|| GF_Shoe_Name(GF_ARTNO(m.source_no, i.order_seq)))) as ArtNo_and_Shoe_type
                //'' as ArtNo_and_Shoe_type
                // FROM wms_rcpt_m m
                //                        LEFT JOIN wms_rcpt_d d
                //                           ON m.chk_no = d.chk_no
                //                        LEFT JOIN bdm_purchase_order_item i
                //                           ON d.source_no = i.order_no
                //                          AND d.chk_seq = i.order_seq
                //                        LEFT JOIN bdm_rd_item e
                //                           ON e.item_no = d.item_no
                //                        LEFT JOIN base003m b
                //                           ON e.vend_no_prd=b.SUPPLIERS_CODE
                //                         	LEFT  JOIN BDM_RD_ITEMTYPE pe on  e.ITEM_TYPE=pe.ITEM_TYPE_NO
                //                         LEFT JOIN iqc_chk_m f
                //                           ON f.chk_no = d.chk_no
                //                          AND f.chk_seq = d.chk_seq
                //                          AND d.item_no = f.src_item_no
                //						LEFT   JOIN qcm_iqc_insp_res_m mm on  d.item_no=mm.item_no and d.chk_no=mm.chk_no and d.chk_seq=mm.chk_seq
                //	where  m.RCPT_BY='01' {swhere}
                //";
                DataTable dt = DB.GetDataTable(sql,dic); 
                sql = $@"SELECT
    d.SOURCE_NO,
	d.SOURCE_SEQ
FROM
	wms_rcpt_m m
LEFT JOIN wms_rcpt_d d ON m.chk_no = d.chk_no
 LEFT JOIN base003m b ON b.SUPPLIERS_CODE = m.vend_no where 1=1 {swhere}";
                DataTable dtt = DB.GetDataTable(sql,dic);//查出条件

                DataTable dtrow = new DataTable();
                List<string> name_t = new List<string>();
                List<string> art_no = new List<string>();
                string[] arr = null;
                foreach (DataRow item in dtt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["SOURCE_NO"].ToString()))
                    {
                        sql = $@"
SELECT
	aa.ORDER_NO,
	--ee.SALES_ORDER,
    ee.ORDER_SEQ,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ee.ART_NO", "ee.ART_NO")} as ART_NO,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT pd.SHOE_NO", "pd.SHOE_NO")} as SHOE_NO
FROM
	bdm_purchase_order_m aa
LEFT  JOIN bdm_purchase_order_d ee on aa.ORDER_NO=ee.ORDER_NO 
LEFT JOIN bdm_rd_prod pd on ee.ART_NO=pd.PROD_NO
where aa.ORDER_NO=@ORDER_NO AND ee.ORDER_SEQ=@ORDER_SEQ group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dic = new Dictionary<string, object>();
                        dic.Add("ORDER_NO", item["SOURCE_NO"]);
                        dic.Add("ORDER_SEQ", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql,dic);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        if (dtrow.Rows.Count > 0)
                        {
                          
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                //item["SHOE_NO"] = dtrow.Rows[0]["SHOE_NO"].ToString();
                                art_no.Add(dtrow.Rows[0]["ART_NO"].ToString());//ART_NO
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    arr = dtrow.Rows[0]["SHOE_NO"].ToString().Split(',');
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {
                                            name_t.Add(dtrow.Rows[i]["NAME_T"].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO=@SHOE_NO";
                                    dic = new Dictionary<string, object>();
                                    dic.Add("SHOE_NO", dtrow.Rows[0]["SHOE_NO"]);
                                    dtrow = DB.GetDataTable(sql,dic);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        name_t.Add(dtrow.Rows[0]["NAME_T"].ToString()); 
                                    }
                                }
                            }
                          

                        }

                    }
                }
                

                foreach (DataRow item2 in dt.Rows)
                {
                    var strArray = art_no.Distinct().ToArray(); //字符去重
                    string art_var=string.Join("、", strArray);

                    var strArray2 = name_t.Distinct().ToArray(); //字符去重
                    string name_t_var = string.Join("、", strArray2);

                    //item2["ArtNo_and_Shoe_type"] = art_var + "\n" + name_t_var;
                    item2["ArtNo和鞋型"] = art_var + "\n" + name_t_var;
                }
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("total", dtt.Rows.Count);
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
        /// 原材料检验不良报告打印
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMainDmp_PrintYCL(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";
                string cscode = jarr.ContainsKey("cscode") ? jarr["cscode"].ToString() : "";
                string stoc_no = jarr.ContainsKey("stoc_no") ? jarr["stoc_no"].ToString() : "";
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string failremark = jarr.ContainsKey("failremark") ? jarr["failremark"].ToString() : "";
                var sql = string.Empty;

                string swhere=string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(putin_date) && string.IsNullOrWhiteSpace(end_date))
                {
                    swhere += $@" and m.RCPT_DATE=TO_date(@putin_date,'yyyy-MM-dd')";
                    dic.Add("putin_date", putin_date);
                }
                else if (!string.IsNullOrWhiteSpace(end_date) && string.IsNullOrWhiteSpace(putin_date))
                {
                    swhere += $@" and m.RCPT_DATE=TO_date(@end_date,'yyyy-MM-dd')";
                    dic.Add("end_date", end_date);
                }
                else if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    swhere += $@" and m.RCPT_DATE BETWEEN TO_date(@putin_date,'yyyy-MM-dd') and TO_date(@end_date,'yyyy-MM-dd')";
                    dic.Add("putin_date", putin_date);
                    dic.Add("end_date", end_date); 
                }
                if (!string.IsNullOrWhiteSpace(item_no))
                {
                    swhere += $@" and d.item_no=@item_no";
                    dic.Add("item_no", item_no);
                }
                if (!string.IsNullOrWhiteSpace(cscode))
                {
                    swhere += $@" and b.SUPPLIERS_CODE=@SUPPLIERS_CODE";
                    dic.Add("SUPPLIERS_CODE", cscode);
                }
                if (!string.IsNullOrWhiteSpace(stoc_no))
                {
                    swhere += $@" and (m.stoc_no=@stoc_no or  m.org_id =@org_id)";
                    dic.Add("stoc_no", stoc_no);
                    dic.Add("org_id", stoc_no);
                }
                //aa={CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT bb.SALES_ORDER", "bb.SALES_ORDER")} AS 销售订单号,
                sql = $@"
select
''  AS 销售订单号,
'{putin_date}～{end_date}' as 寄存日期,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ('['|| m.CHK_NO||'/'||d.RCPT_QTY || ']')", "m.CHK_NO")} as 收料单号和数量,
'' as 退货原因,
'' as 厂商回复,
'' as qip总检,
'' as 会签,
'' as qip材料助理,
'' as 检验员,

to_char(SYSDATE, 'yyyy/MM/dd') AS 日期,
sum(d.RCPT_QTY ) as 寄存数量,
sum(mm.sample_qty ) as 检验数量,
CASE
  WHEN (sum(d.RCPT_QTY ) > 0) THEN
    (round((sum(mm.sample_qty)/ sum(d.RCPT_QTY )),4)*100)
      ELSE
    0
END AS 抽检率,
'' as 退货数量,
'' AS 退货率,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.chk_no", "m.chk_no")} as 收料单号,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT b.SUPPLIERS_NAME", "b.SUPPLIERS_NAME")} AS 供应厂商名字,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT d.item_no", "d.item_no")} as 材料号,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT e.name_t", "e.name_t")} as  材料名字,
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT m.source_no", "m.source_no")} AS 订单号,
('{failremark}') AS 不合格证明,
-- wm_concat(DISTINCT ( GF_ARTNO(m.source_no, i.order_seq)|| GF_Shoe_Name(GF_ARTNO(m.source_no, i.order_seq)))) as ArtNo和鞋型
'' as ArtNo和鞋型

 FROM wms_rcpt_m m
                        LEFT JOIN wms_rcpt_d d
                           ON m.chk_no = d.chk_no
                        LEFT JOIN bdm_purchase_order_item i
                           ON d.source_no = i.order_no
                          AND d.chk_seq = i.order_seq
                        LEFT JOIN bdm_rd_item e
                           ON e.item_no = d.item_no
                        LEFT JOIN base003m b
                           ON e.vend_no_prd=b.SUPPLIERS_CODE
                         	LEFT  JOIN BDM_RD_ITEMTYPE pe on  e.ITEM_TYPE=pe.ITEM_TYPE_NO
                         LEFT JOIN iqc_chk_m f
                           ON f.chk_no = d.chk_no
                          AND f.chk_seq = d.chk_seq
                          AND d.item_no = f.src_item_no
						 LEFT   JOIN qcm_iqc_insp_res_m mm on  d.item_no=mm.item_no and d.chk_no=mm.chk_no and d.chk_seq=mm.chk_seq
	where  m.RCPT_BY='01' {swhere}";
                DataTable dt = DB.GetDataTable(sql, dic);
                sql = $@"SELECT
    d.SOURCE_NO,
	d.SOURCE_SEQ
FROM
	wms_rcpt_m m
LEFT JOIN wms_rcpt_d d ON m.chk_no = d.chk_no
 LEFT JOIN base003m b ON b.SUPPLIERS_CODE = m.vend_no where 1=1 {swhere}";
                DataTable dtt = DB.GetDataTable(sql,dic);//查出条件

                DataTable dtrow = new DataTable();

                List<string> name_t = new List<string>();
                List<string> art_no = new List<string>();
                string[] arr = null;
                foreach (DataRow item in dtt.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["SOURCE_NO"].ToString()))
                    {
                        sql = $@"
SELECT
	aa.ORDER_NO,
	--ee.SALES_ORDER,
    ee.ORDER_SEQ,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ee.ART_NO", "ee.ART_NO")} as ART_NO,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT pd.SHOE_NO", "pd.SHOE_NO")} as SHOE_NO
FROM
	bdm_purchase_order_m aa
LEFT  JOIN bdm_purchase_order_d ee on aa.ORDER_NO=ee.ORDER_NO 
LEFT JOIN bdm_rd_prod pd on ee.ART_NO=pd.PROD_NO
where aa.ORDER_NO=@ORDER_NO AND ee.ORDER_SEQ=@ORDER_SEQ group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dic = new Dictionary<string, object>();
                        dic.Add("ORDER_NO", item["SOURCE_NO"]);
                        dic.Add("ORDER_SEQ", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql,dic);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        if (dtrow.Rows.Count > 0)
                        {
                            //item["SHOE_NO"] = dtrow.Rows[0]["SHOE_NO"].ToString();
                            art_no.Add(dtrow.Rows[0]["ART_NO"].ToString());//ART_NO
                          
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    arr = dtrow.Rows[0]["SHOE_NO"].ToString().Split(',');
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {
                                            name_t.Add(dtrow.Rows[i]["NAME_T"].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO=@SHOE_NO";
                                    dic = new Dictionary<string, object>();
                                    dic.Add("SHOE_NO", dtrow.Rows[0]["SHOE_NO"]);
                                    dtrow = DB.GetDataTable(sql,dic);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        name_t.Add(dtrow.Rows[0]["NAME_T"].ToString());

                                    }
                                }
                                
                            }


                        }

                    }
                }


                foreach (DataRow item2 in dt.Rows)
                {
                    var strArray = art_no.Distinct().ToArray(); //字符去重
                    string art_var = string.Join("、", strArray);

                    var strArray2 = name_t.Distinct().ToArray(); //字符去重
                    string name_t_var = string.Join("、", strArray2);
                    item2["ArtNo和鞋型"] = art_var + "\n" + name_t_var;
                }
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("total", dtt.Rows.Count);
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
        /// 送测二维码打印(v0版本)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMainDmp_PrintXC(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string chk_seq = jarr.ContainsKey("chk_seq") ? jarr["chk_seq"].ToString() : "";
                string rcpt_date = jarr.ContainsKey("rcpt_date") ? jarr["rcpt_date"].ToString() : "";
                string rcpt_dates = Convert.ToDateTime(rcpt_date).ToString("yyyy-MM-dd");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var sql = string.Empty;
                sql = $@"SELECT DISTINCT  b.suppliers_code,
                 i.item_no,
                m.chk_no,
                 m.stoc_no,
                 m.source_no AS ORDER_NO,
				d.rcpt_qty Receipt_Qty,
                 d.SOURCE_SEQ,
                 m.RCPT_DATE,
                 nvl(i.ORD_QTY,0) as ORD_QTY,
                 GF_Shoe_Name(GF_ARTNO(m.source_no, i.order_seq)) AS Shoe_Name,
                 GF_Art_Name(GF_ARTNO(m.source_no, i.order_seq)) AS Art_Name,
                 GF_ARTNO(m.source_no, i.order_seq) AS ART_NO,
                 GF_Ord_Qty(d.source_no, d.chk_seq) AS Ord_Qty,
                 b.SUPPLIERS_NAME,
                 d.item_no AS Material_No,
                 e.name_t AS Material_Name,
                 i.part_no,
                 to_char(SYSDATE, 'yyyy/MM/dd') AS TodayDate,
                 m.org_id,
                 GF_Ord_Qty(d.source_no, d.chk_seq) AS QtyStr,
                 GF_ARTNO(m.source_no, i.order_seq) as xiexing,
                 GF_ARTNO(m.source_no, i.order_seq) as art_no,
                 '' AS Shoe_NameAndArt_No,
                  '' AS ORDER_NOAndQty
               FROM wms_rcpt_m m
              LEFT JOIN wms_rcpt_d d
                 ON m.chk_no = d.chk_no
              LEFT JOIN bdm_purchase_order_item i
                 ON d.source_no = i.order_no
                AND d.chk_seq = i.order_seq
              LEFT JOIN base003m b
                 ON b.SUPPLIERS_CODE = m.vend_no
              LEFT JOIN bdm_rd_item e
                 ON e.item_no = d.item_no
				where m.chk_no=@chk_no  and  d.item_no=@item_no  and d.chk_seq=@chk_seq";
                dic.Add("chk_no", chk_no);
                dic.Add("item_no",item_no);
                dic.Add("chk_seq", chk_seq);
                DataTable dt = DB.GetDataTable(sql, dic);
                dt.Columns.Add("print", Type.GetType("System.Object"));
                
                sql = $@"select
{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT (ORDER_NO || '/'||	rcpt_qty)", "ORDER_NO")} AS ORDER_NOAndQty
from (
SELECT
	DISTINCT
	y.ORDER_NO,
	t.rcpt_qty
FROM
	(
		SELECT
			b.SOURCE_NO,
			b.SOURCE_SEQ,
			b.rcpt_qty
		FROM
			wms_rcpt_m A
		LEFT JOIN wms_rcpt_d b ON A .chk_no = b.CHK_NO
		WHERE
			1 = 1
		AND b.item_no=@item_no
		AND A.RCPT_DATE=TO_date(@rcpt_dates,'yyyy-MM-dd')
	) T
INNER JOIN bdm_purchase_order_d y ON T .SOURCE_NO = y.ORDER_NO
AND T .SOURCE_SEQ = y.ORDER_SEQ)";
                dic = new Dictionary<string, object>();
                dic.Add("item_no", item_no);
                dic.Add("rcpt_dates", rcpt_dates);
                string ORDER_NOAndQty = DB.GetString(sql,dic);
                DataTable dtrow = new DataTable();
                string art = string.Empty;
                string[] arr = null;
                DataTable dr = new DataTable();

                sql = $@"SELECT
	{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT a.CHK_NO", "a.CHK_NO")} AS CHK_NO
FROM
	wms_rcpt_m A left join wms_rcpt_d b on a.chk_No=b.chk_No
where b.item_no=@item_no and a.RCPT_DATE=to_date(@rcpt_dates,'yyyy-MM-dd')";
                dic = new Dictionary<string, object>();
                dic.Add("item_no", item_no);
                dic.Add("rcpt_dates", rcpt_dates);
                string chk_nolist= DB.GetString(sql,dic);
                foreach (DataRow item in dt.Rows)
                {

                    var aa = new
                    {
                        rcpt_date = rcpt_dates,//收料日期
                        item_no = item["item_no"].ToString(),//料号
                    };
                    item["ORDER_NOAndQty"] = ORDER_NOAndQty;
                    item["print"] = Newtonsoft.Json.JsonConvert.SerializeObject(aa);
                    if (!string.IsNullOrWhiteSpace(item["ORDER_NO"].ToString()))
                    {
                        string sql2 = $@"
SELECT
	aa.ORDER_NO,
	--ee.SALES_ORDER,
    ee.ORDER_SEQ,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ee.ART_NO", "ee.ART_NO")} as ART_NO,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT pd.SHOE_NO", "pd.SHOE_NO")} as SHOE_NO
FROM
	bdm_purchase_order_m aa
LEFT  JOIN bdm_purchase_order_d ee on aa.ORDER_NO=ee.ORDER_NO 
LEFT JOIN bdm_rd_prod pd on ee.ART_NO=pd.PROD_NO
where aa.ORDER_NO=@ORDER_NO AND ee.ORDER_SEQ=@ORDER_SEQ  group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dic = new Dictionary<string, object>();
                        dic.Add("ORDER_NO", item["ORDER_NO"]);
                        dic.Add("ORDER_SEQ", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql2,dic);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        List<string> art_no = new List<string>();
                        List<string> name_t = new List<string>();
                        if (dtrow.Rows.Count > 0)
                        {
                            
                            art_no.Add(dtrow.Rows[0]["ART_NO"].ToString());//ART_NO
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    name_t = new List<string>() ;
                                    arr = dtrow.Rows[0]["SHOE_NO"].ToString().Split(',');

                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {

                                            name_t.Add(dtrow.Rows[0]["NAME_T"].ToString());
                                        }
                                    }
                                   

                                }
                                else
                                {
                                    dic = new Dictionary<string, object>();
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO=@SHOE_NO";
                                    dic.Add("SHOE_NO", dtrow.Rows[0]["SHOE_NO"]);
                                    dtrow = DB.GetDataTable(sql,dic);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        name_t.Add(dtrow.Rows[0]["NAME_T"].ToString());

                                    }
                                }
                             
                            }
                            var strArray = art_no.Distinct().ToArray(); //字符去重
                            string art_var = string.Join("、", strArray);

                            var strArray2 = name_t.Distinct().ToArray(); //字符去重
                            string name_t_var = string.Join("、", strArray2);

                            item["Shoe_NameAndArt_No"] = name_t_var + "\n" + art_var;

                        }
                        sql2 = $@"
select 
	PART_NO
from bdm_purchase_order_item  
where order_no =@order_no and order_seq =@order_seq";
                        dic = new Dictionary<string, object>();
                        dic.Add("order_no", item["ORDER_NO"]);
                        dic.Add("order_seq", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql2,dic);
                        if (dtrow.Rows.Count > 0)
                        {
                            item["PART_NO"] = dtrow.Rows[0]["PART_NO"].ToString();
                        }
                    }
                }
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
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
        /// 送测二维码打印(v1版本)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMainDmp_PrintXC2(object OBJ)
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
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string chk_seq = jarr.ContainsKey("chk_seq") ? jarr["chk_seq"].ToString() : "";
                string rcpt_date = jarr.ContainsKey("rcpt_date") ? jarr["rcpt_date"].ToString() : "";
                string rcpt_dates = Convert.ToDateTime(rcpt_date).ToString("yyyy-MM-dd");
                string org_id = jarr.ContainsKey("org_id") ? jarr["org_id"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                #region 旧的
                /*var sql = $@"
        SELECT DISTINCT
            TO_CHAR (SYSDATE, 'yyyy/MM/dd') AS TodayDate,--当前日期
            max(d.item_no) AS Material_No,--料号
            max(e.name_t) AS Material_Name,--材料名称
            {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT (z2.NAME_T || '/'||	z1.ART_NO)", "z1.ART_NO")} AS Shoe_NameAndArt_No, --鞋型名称+art
            (SELECT
                    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT (y.ORDER_NO || '/'||	b.rcpt_qty)", "y.ORDER_NO")}
                FROM
                    wms_rcpt_m A
                LEFT JOIN wms_rcpt_d b ON a.chk_no = b.CHK_NO
                INNER JOIN bdm_purchase_order_d y ON b.SOURCE_NO = y.ORDER_NO AND b.SOURCE_SEQ = y.ORDER_SEQ
                WHERE
                    1 = 1
                AND b.item_no=d.item_no
                AND A.RCPT_DATE=TO_date('{rcpt_dates}','yyyy-MM-dd')) AS ORDER_NOAndQty,--订单号/数量
            max(c111.part_no) as part_no,--部位
            max(b.suppliers_code) as suppliers_code, --厂商编号
            max(b.SUPPLIERS_NAME) as SUPPLIERS_NAME -- 厂商名称

        FROM
            wms_rcpt_m M
        LEFT JOIN wms_rcpt_d D ON M.chk_no = D.chk_no
        LEFT JOIN bdm_purchase_order_m  a2 on a2.ORDER_NO=d.SOURCE_NO
        LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=d.SOURCE_SEQ --新加的
        LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
        LEFT JOIN bdm_purchase_order_item c111 ON c111.ORDER_NO=d.SOURCE_NO AND c111.ORDER_SEQ=d.CHK_SEQ
        LEFT JOIN base003m b ON b.SUPPLIERS_CODE = m.vend_no
        LEFT JOIN bdm_rd_item E ON E.item_no= D.item_no
        WHERE
            M.chk_no = '{chk_no}'
        AND d.item_no ='{item_no}'
        AND d.chk_seq ='{chk_seq}'
        group by d.item_no,m.chk_no,d.chk_seq";*/
                #endregion

                string get_sld_date = DB.GetString($@"SELECT TO_CHAR(RCPT_DATE,'yyyy-MM-dd') FROM WMS_RCPT_M WHERE CHK_NO='{chk_no}' AND ORG_ID='{org_id}'");
                if (string.IsNullOrEmpty(get_sld_date))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = $@"{chk_no}Material receipt does not exist";
                }
                else
                {
                    rcpt_dates = get_sld_date;
                    var sql = $@"
SELECT DISTINCT
	TO_CHAR (SYSDATE, 'yyyy/MM/dd') AS TodayDate,--当前日期
	max(d.item_no) AS Material_No,--料号
	max(e.name_t) AS Material_Name,--材料名称
    '' Shoe_NameAndArt_No, --鞋型名称+art
	'' AS ORDER_NOAndQty,--订单号/数量
    '' as part_no,--部位
	max(e.vend_no_prd) as suppliers_code, --厂商编号
	max(b.SUPPLIERS_NAME) as SUPPLIERS_NAME, -- 厂商名称
	max(b.JC) as JC, -- 厂商简称
    '' as sumQty--premika add column
FROM
	wms_rcpt_m M
LEFT JOIN wms_rcpt_d D ON M.chk_no = D.chk_no
LEFT JOIN bdm_purchase_order_item c111 ON c111.ORDER_NO=d.SOURCE_NO AND c111.ORDER_SEQ=d.CHK_SEQ
LEFT JOIN bdm_rd_item E ON E.item_no= D.item_no
LEFT JOIN base003m b ON b.SUPPLIERS_CODE = e.vend_no_prd
WHERE
	M.chk_no = '{chk_no}'
AND d.item_no ='{item_no}'
AND d.chk_seq ='{chk_seq}'
group by d.item_no,m.chk_no,d.chk_seq";
                    DataTable dt = DB.GetDataTable(sql);
                    dt.Columns.Add("print", Type.GetType("System.Object"));
                    sql = $@"select
	{CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT (ORDER_NO || '/'||	rcpt_qty)", "ORDER_NO")}  as ORDER_NOAndQty
from (
SELECT
		b1.ITEM_NO,
		d1.ORDER_NO,
          b1.org_id,
		b1.rcpt_qty
		FROM
			wms_rcpt_m A1
		LEFT JOIN wms_rcpt_d b1 ON a1.chk_no = b1.CHK_NO
		LEFT JOIN bdm_purchase_order_m  m1 on m1.ORDER_NO=b1.SOURCE_NO
		left  JOIN bdm_purchase_order_d d1 ON m1.ORDER_NO = d1.ORDER_NO AND d1.ORDER_SEQ = b1.SOURCE_SEQ
		WHERE
			1 = 1 and  d1.ORDER_NO is not null
		AND A1.RCPT_DATE=TO_date('{rcpt_dates}','yyyy-MM-dd')
union 
SELECT
			b1.ITEM_NO,
		im.ORDER_NO,
        b1.org_id,
		b1.rcpt_qty
		FROM
			wms_rcpt_m A1
		LEFT JOIN wms_rcpt_d b1 ON a1.chk_no = b1.CHK_NO
		LEFT JOIN bdm_purchase_order_m  m1 on m1.ORDER_NO=b1.SOURCE_NO
		LEFT JOIN bdm_purchase_order_item im on m1.ORDER_NO=im.ORDER_NO AND im.ORDER_SEQ = b1.SOURCE_SEQ
		WHERE
			1 = 1 and im.ORDER_NO is not null
		AND A1.RCPT_DATE=TO_date('{rcpt_dates}','yyyy-MM-dd')
)where item_no='{item_no}' and org_id='{org_id}'";
                    string ORDER_NOAndQty = DB.GetString(sql);

                    //premika
                    double sumQty = ORDER_NOAndQty
    .Split(',')
    .Select(x => double.Parse(x.Split('/')[1], CultureInfo.InvariantCulture))
    .Sum();
                   
                    //premika

                    sql = $@"SELECT
		  {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT (z2.NAME_T || '/'||	z1.ART_NO)", "z1.ART_NO")} AS Shoe_NameAndArt_No --鞋型名称+art
		FROM
	wms_rcpt_m m
LEFT JOIN wms_rcpt_d D ON M.chk_no = D.chk_no
LEFT JOIN bdm_purchase_order_m  a2 on a2.ORDER_NO=d.SOURCE_NO
LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=d.SOURCE_SEQ --新加的
LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
		WHERE
		 m.RCPT_DATE=TO_date('{rcpt_dates}','yyyy-MM-dd') AND d.item_no ='{item_no}' and d.org_id='{org_id}'";
                    string Shoe_NameAndArt_No = DB.GetString(sql);


                    sql = $@"select
 {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT c111.part_no", "c111.part_no")} AS part_no
FROM
	wms_rcpt_m M
LEFT JOIN wms_rcpt_d D ON M.chk_no = D.chk_no
LEFT JOIN bdm_purchase_order_item c111 ON c111.ORDER_NO=d.SOURCE_NO AND c111.ORDER_SEQ=d.CHK_SEQ where  m.RCPT_DATE=TO_date('{rcpt_dates}','yyyy-MM-dd') AND d.item_no ='{item_no}' and d.org_id='{org_id}'";
                    string part_no = DB.GetString(sql);
                    foreach (DataRow item in dt.Rows)
                    {


                        item["ORDER_NOAndQty"] = ORDER_NOAndQty;
                        item["sumQty"] = sumQty;
                        item["Shoe_NameAndArt_No"] = Shoe_NameAndArt_No;
                        item["part_no"] = part_no;

                        var json = new
                        {
                            rcpt_date = rcpt_dates,//收料日期
                            item_no = item_no,//料号
                            chk_seq = chk_seq,//料号序号
                            chk_no = chk_no,//收料单号
                            org_id = org_id//工厂编号
                        };
                        item["print"] = Newtonsoft.Json.JsonConvert.SerializeObject(json);
                    }
                    dic = new Dictionary<string, object>();
                    dic.Add("Data", dt);
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    ret.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMainDmp_Chk_nolist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);  
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";
                string rcpt_date = jarr.ContainsKey("rcpt_date") ? jarr["rcpt_date"].ToString() : "";
                string rcpt_dates = Convert.ToDateTime(rcpt_date).ToString("yyyy-MM-dd");
               
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";

               string lab_no = jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//premika-2025/12/05
                //premika--start2025/12/18
                DateTime rcptDate = Convert.ToDateTime(rcpt_date);

                // First day of the month
                DateTime fromDate = new DateTime(rcptDate.Year, rcptDate.Month, 1);
                // Last day of the month
                DateTime toDate = fromDate.AddMonths(1).AddDays(-1);
                // Format for SQL
                string fromdate = fromDate.ToString("yyyy-MM-dd");
                string todate = toDate.ToString("yyyy-MM-dd");
                //premika--end
                var sql = string.Empty;
                sql = $@"select max(id)  from qcm_ex_task_list_m where sldh like '%{chk_no}%' and makings_id='{item_no}'";
                //premika--start 2025/12/05
                if (!string.IsNullOrWhiteSpace(lab_no))
                {
                    sql += $@"and task_no='{lab_no}'";
                }
                //premika-end
                string id = DB.GetString(sql);
                //if (string.IsNullOrWhiteSpace(id))
                //{
                //    ret.IsSuccess = false;
                //    ret.ErrMsg = $@"Receipt number [{chk_no}], item number [{item_no}] is not recorded in the laboratory information, please check";
                //    return ret;
                //}
                //premika--start
                string ITEM_TYPE_NOS = item_no.ToString().Substring(0, 3);
                if (!ITEM_TYPE_NOS.Contains("401"))
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        string sql_task = $@"
                                SELECT j.task_no,j.id
                                  FROM qcm_ex_task_list_m j
                                 WHERE j.makings_id = '{item_no}'
                                   AND TO_DATE(
                                         REGEXP_SUBSTR(j.cl_qrcode_json,
                                                       '""rcpt_date"":""([^""]+)""',
                                                       1, 1, NULL, 1),
                                         'YYYY-MM-DD'
                                       ) =
                                       (
                                         SELECT MAX(
                                                  TO_DATE(
                                                    REGEXP_SUBSTR(x.cl_qrcode_json,
                                                                  '""rcpt_date"":""([^""]+)""',
                                                                  1, 1, NULL, 1),
                                                    'YYYY-MM-DD'
                                                  )
                                                )
                                           FROM qcm_ex_task_list_m x
                                          WHERE x.makings_id = '{item_no}'
                                            AND TO_DATE(
                                                  REGEXP_SUBSTR(x.cl_qrcode_json,
                                                                '""rcpt_date"":""([^""]+)""',
                                                                1, 1, NULL, 1),
                                                  'YYYY-MM-DD'
                                                ) between DATE '{fromdate}' and  DATE '{rcpt_dates}'
                                       )";
                        DataTable task_res = DB.GetDataTable(sql_task);
                        if (task_res.Rows.Count > 0)
                        {
                            id = task_res.Rows[0]["ID"].ToString();
                        }
                    }



                }
                else
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = $@"Receipt number [{chk_no}], item number [{item_no}] is not recorded in the laboratory information, please check";
                        return ret;
                    }
                }
                //premika--end

                string task_no = DB.GetString($@"select task_no from qcm_ex_task_list_m where  id='{id}'");
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("task_no", task_no);//返回实验室任务编号
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
        /// DateTable的分页操作
        /// </summary>
        /// <param name="dt">要进行分页的DataTable</param>
        /// <param name="currentPageIndex">当前页数</param>
        /// <param name="pageSize">一页显示的条数</param>
        /// <returns>第pageIndex页的数据</returns>
        public static DataTable SetPage(DataTable dt, int currentPageIndex, int pageSize)
        {
            if (currentPageIndex == 0)
            {
                return dt;
            }

            DataTable newdt = dt.Clone();

            int rowbegin = (currentPageIndex - 1) * pageSize;//当前页的第一条数据在dt中的位置
            int rowend = currentPageIndex * pageSize;//当前页的最后一条数据在dt中的位置

            if (rowbegin >= dt.Rows.Count)
            {
                return newdt;
            }

            if (rowend > dt.Rows.Count)
            {
                rowend = dt.Rows.Count;
            }

            DataView dv = dt.DefaultView;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                newdt.ImportRow(dv[i].Row);
            }

            return newdt;
        }
        /// <summary>
        /// 返回分页的页数
        /// </summary>
        /// <param name="count">总条数</param>
        /// <param name="pageye">每页显示多少条</param>
        /// <returns>如果 结尾为0：则返回1</returns>
        public static int PageCount(int count, int pageye)
        {
            int page = 0;
            int sesepage = pageye;
            if (count % sesepage == 0) { page = count / sesepage; }
            else { page = (count / sesepage) + 1; }
            if (page == 0) { page += 1; }
            return page;
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
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var sql = string.Empty;
                sql = $@"SELECT
	b.sample_qty,
	a.chk_no, --收料单号
	a.test_item_no, --检测项
	a.test_item_name, --检测项名称
	a.test_standard, --检测标准
	a.determine, --检验结果
	a.image_guid, --图片关联id
	a.remark  --remark
FROM
	qcm_iqc_insp_res_d a LEFT JOIN qcm_iqc_insp_res_m b on a.chk_no=b.chk_no and a.chk_seq=b.CHK_SEQ and a.ITEM_NO=b.item_no where b.isdelete='0' and a.chk_no=@chk_no and a.chk_seq=@chk_seq and a.item_no=@item_no";
                dic = new Dictionary<string, object>();
                dic.Add("chk_no", CHK_NO);
                dic.Add("chk_seq", CHK_SEQ);
                dic.Add("item_no", ITEM_NO);
                DataTable dt = DB.GetDataTable(sql,dic);

                dic = new Dictionary<string, object>();
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckPreviousResultJYView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                //string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                //string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                var sql = string.Empty;
//                sql = $@"SELECT
//    a.createdate,
//	b.sample_qty,
//	a.chk_no, --收料单号
//	a.test_item_no, --检测项
//	a.test_item_name, --检测项名称
//	a.test_standard, --检测标准
//	a.determine, --检验结果
//	a.image_guid, --图片关联id
//	a.remark  --remark
//FROM
//	qcm_iqc_insp_res_d a LEFT JOIN qcm_iqc_insp_res_m b on a.chk_no=b.chk_no and a.chk_seq=b.CHK_SEQ and a.ITEM_NO=b.item_no where b.isdelete='0' and a.item_no=@item_no";

                sql = $@"SELECT
  a.createdate,
  b.sample_qty,
  a.chk_no, --收料单号
  a.test_item_no, --检测项
  a.test_item_name, --检测项名称
  a.test_standard, --检测标准
  a.determine, --检验结果
  a.image_guid, --图片关联id
  a.remark  --remark
FROM
  qcm_iqc_insp_res_d a LEFT JOIN qcm_iqc_insp_res_m b on a.chk_no=b.chk_no and a.chk_seq=b.CHK_SEQ and a.ITEM_NO=b.item_no where b.isdelete='0' and a.item_no=@item_no order by a.createdate  desc";

                dic = new Dictionary<string, object>();
                //dic.Add("chk_no", CHK_NO);
                //dic.Add("chk_seq", CHK_SEQ);
                dic.Add("item_no", ITEM_NO);
                DataTable dt = DB.GetDataTable(sql, dic);
                sql = $@"      select 
      sum(a.sample_qty)as total_qty ,
     sum(case when a.determine = '0' then a.sample_qty else 0 end) as pass_qty,
     sum(case when a.determine = '1' then a.sample_qty else 0 end) as fail_qty
      from qcm_iqc_insp_res_m a 
        where a.item_no =@item_no";
                DataTable dt2 = DB.GetDataTable(sql,dic);
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2", dt2);
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
              
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var p = jarr.ContainsKey("p") ? jarr["p"].ToString() : "";
                var p2 = jarr.ContainsKey("p2") ? jarr["p2"].ToString() : "";
                if (p != null && p2 != null)
                {
                    //表头内容
                    var jarr1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(p.ToString());
                    string sample_qty = jarr1.ContainsKey("sample_qty") ? jarr1["sample_qty"].ToString() : "";//抽样数量
                    string chk_no = jarr1.ContainsKey("chk_no") ? jarr1["chk_no"].ToString() : "";//来料单号
                    string id = jarr1.ContainsKey("id") ? jarr1["id"].ToString() : "";//uid
                    string determine = jarr1.ContainsKey("determine") ? jarr1["determine"].ToString() : "";//判断
                    string pass_qty = jarr1.ContainsKey("pass_qty") ? jarr1["pass_qty"].ToString() : "";//合格数
                    string testlevel = jarr1.ContainsKey("testlevel") ? jarr1["testlevel"].ToString() : "";//检验水平
                    string aql_level = jarr1.ContainsKey("aql_level") ? jarr1["aql_level"].ToString() : "";//aql级别
                    string txt_qcre = jarr1.ContainsKey("txt_acre") ? jarr1["txt_acre"].ToString() : "";//ac_re值
                    string chk_seq = jarr1.ContainsKey("chk_seq") ? jarr1["chk_seq"].ToString() : "";//材料序号
                    string item_no = jarr1.ContainsKey("item_no") ? jarr1["item_no"].ToString() : "";//料号
                    string remark = jarr1.ContainsKey("remark") ? jarr1["remark"].ToString() : "";//备注
                    string userCode2 = jarr1.ContainsKey("usercode") ? jarr1["usercode"].ToString() : "";//备注
                    string datas = jarr1.ContainsKey("datas") ? jarr1["datas"].ToString() : "";//检验日期
                    string rcpt_qty = jarr1.ContainsKey("rcpt_qty") ? jarr1["rcpt_qty"].ToString() : "";//检验数量
                    string sql = string.Empty;
                    string uid = string.Empty;
                    string date = DateTime.Now.ToString("yyyy-MM-dd");
                    string time = DateTime.Now.ToString("HH:mm:ss");
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    sql = $@"select chk_no from qcm_iqc_insp_res_m where chk_no=@chk_no  and chk_seq=@chk_seq and item_no=@item_no";
                    dic.Add("chk_no", chk_no);
                    dic.Add("chk_seq", chk_seq);
                    dic.Add("item_no", item_no);
                    if (string.IsNullOrWhiteSpace(DB.GetString(sql,dic)))
                    {
                       
                        sql = $@"insert into qcm_iqc_insp_res_m (chk_no,item_no,chk_seq,remark,pass_qty,determine,closing_status,sample_qty,aql_level,ac_re,testlevel,staff_no,inspectiondate,createby,createdate,createtime) VALUES(@chk_no,@item_no,@chk_seq,@remark,@pass_qty,@determine,@closing_status,@sample_qty,@aql_level,@ac_re,@testlevel,@staff_no,@inspectiondate,@createby,@createdate,@createtime)";
                        dic = new Dictionary<string, object>();
                        dic.Add("chk_no", chk_no);
                        dic.Add("item_no", item_no);
                        dic.Add("chk_seq", chk_seq);
                        dic.Add("remark", remark);
                        dic.Add("pass_qty", pass_qty);
                        dic.Add("determine", determine);
                        dic.Add("closing_status","");
                        dic.Add("sample_qty", sample_qty);
                        dic.Add("aql_level", aql_level);
                        dic.Add("ac_re", txt_qcre);
                        dic.Add("testlevel", testlevel);
                        dic.Add("staff_no", userCode2);
                        dic.Add("inspectiondate", datas);
                        dic.Add("createby", userCode);
                        dic.Add("createdate", date);
                        dic.Add("createtime",time);
                        DB.ExecuteNonQuery(sql,dic);
                    }
                    else
                    {
                        sql = $@"update qcm_iqc_insp_res_m
                                         set pass_qty =@pass_qty,
                                          determine =@determine,
                                          sample_qty =@sample_qty,
                                          testlevel=@testlevel,
                                          aql_level=@aql_level, 
                                            ac_re=@ac_re,
                                            remark=@remark,
                                            modifyby =@modifyby,
                                           isdelete=@isdelete,
                                          staff_no=@staff_no,
                                          inspectiondate=@inspectiondate,
                                          modifydate =@modifydate,
                                          modifytime =@modifytime
                                         where
	                                       chk_no=@chk_no and item_no=@item_no  and chk_seq=@chk_seq";
                        dic = new Dictionary<string, object>();
                        dic.Add("pass_qty", pass_qty);
                        dic.Add("determine", determine);
                        dic.Add("sample_qty", sample_qty);
                        dic.Add("testlevel", testlevel);
                        dic.Add("aql_level", aql_level);
                        dic.Add("ac_re", txt_qcre);
                        dic.Add("remark", remark);
                        dic.Add("modifyby", userCode);
                        dic.Add("isdelete", "0");
                        dic.Add("staff_no", userCode2);
                        dic.Add("inspectiondate", datas);
                        dic.Add("modifydate", date);
                        dic.Add("modifytime", time);
                        dic.Add("chk_no", chk_no);
                        dic.Add("item_no", item_no);
                        dic.Add("chk_seq", chk_seq);
                        DB.ExecuteNonQuery(sql,dic);
                       

                    }
                    var requst = Newtonsoft.Json.JsonConvert.DeserializeObject<listdic>(p2);
                    if (requst.listdic2.Count > 0)
                    {
                        foreach (var item in requst.listdic2)
                        {
                            sql = $@"select chk_no from qcm_iqc_insp_res_d where chk_no=@chk_no and test_item_no=@test_item_no and item_no=@item_no and chk_seq=@chk_seq";
                            dic = new Dictionary<string, object>();
                            dic.Add("chk_no", item.chk_no);
                            dic.Add("test_item_no", item.test_item_no);
                            dic.Add("item_no",item_no);
                            dic.Add("chk_seq",chk_seq);
                            string chk_nos = DB.GetString(sql,dic);

                           // if (item.determine == "是")
                            if (item.determine == "Yes")
                            {
                                item.determine = "0";
                            }
                            if (item.determine == "No")
                            {
                                item.determine = "1";
                            }

                            if (string.IsNullOrWhiteSpace(chk_nos))
                            {

                                sql = $@"insert into qcm_iqc_insp_res_d (chk_no,item_no,chk_seq,badproblem_code,test_item_no,test_item_name,test_standard,determine,remark,createby,createdate,createtime) values(@chk_no,@item_no,@chk_seq,@badproblem_code,@test_item_no,@test_item_name,@test_standard,@determine,@remark,@createby,@createdate,@createtime)";
                                dic = new Dictionary<string, object>();
                                dic.Add("chk_no", item.chk_no);
                                dic.Add("item_no",item_no);
                                dic.Add("chk_seq",chk_seq);
                                dic.Add("badproblem_code", item.badproblem_code);
                                dic.Add("test_item_no", item.test_item_no);
                                dic.Add("test_item_name", item.test_item_name);
                                dic.Add("test_standard", item.test_standard);
                                dic.Add("determine", item.determine);
                                dic.Add("remark", item.remark);
                                dic.Add("createby", userCode);
                                dic.Add("createdate", date);
                                dic.Add("createtime",time);
                                DB.ExecuteNonQuery(sql,dic);
                                uid = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_d");
                            }
                            else
                            {
                                sql = $@"update qcm_iqc_insp_res_d
                                           set
                                          test_item_name=@test_item_name,
                                          test_standard=@test_standard,
                                          determine=@determine,
                                          badproblem_code=@badproblem_code,
                                          remark=@remark,
                                          isdelete='0',
                                          modifyby=@modifyby,
                                          modifydate=@modifydate,
                                          modifytime=@modifytime
                                         where
	                                       chk_no =@chk_no and test_item_no=@test_item_no and item_no=@item_no and chk_seq=@chk_seq";
                                dic = new Dictionary<string, object>();
                                dic.Add("test_item_name", item.test_item_name);
                                dic.Add("test_standard", item.test_standard);
                                dic.Add("determine", item.determine);
                                dic.Add("badproblem_code", item.badproblem_code);
                                dic.Add("remark", item.remark);
                                dic.Add("modifyby", userCode);
                                dic.Add("modifydate", date);
                                dic.Add("modifytime", time);
                                dic.Add("chk_no", item.chk_no);
                                dic.Add("test_item_no", item.test_item_no);
                                dic.Add("item_no", item_no);
                                dic.Add("chk_seq", chk_seq);
                                DB.ExecuteNonQuery(sql,dic);

                                sql = $"select id from qcm_iqc_insp_res_d  where  chk_no=@chk_no and test_item_no=@test_item_no and item_no=@item_no and chk_seq=@chk_seq";
                                dic = new Dictionary<string, object>();
                                dic.Add("chk_no", chk_no);
                                dic.Add("test_item_no", item.test_item_no);
                                dic.Add("item_no", item_no);
                                dic.Add("chk_seq", chk_seq);
                                uid = DB.GetString(sql,dic);
                            }
                            //图片提交
                            if (item.image_list.Count > 0)
                            {
                                sql = string.Empty;
                                int index = 0;
                                dic = new Dictionary<string, object>();
                                string sqldel = string.Empty;
                                foreach (imginfo abc in item.image_list)
                                {
                                    index++;
                                    dic.Add($"d_id{index}", uid);
                                    dic.Add($"file_id{index}", abc.guid);
                                    dic.Add($"createby{index}", userCode);
                                    dic.Add($"createdate{index}", date);
                                    dic.Add($"createtime{index}", time);
                                    sqldel += $@"delete from qcm_iqc_insp_res_d_f where d_id='{uid}';";
                                    sql +=$@"insert into qcm_iqc_insp_res_d_f(d_id,file_id,createby,createdate,createtime) values(@d_id{index},@file_id{index},@createby{index},@createdate{index},@createtime{index});";
                                }
                                if (!string.IsNullOrWhiteSpace(sqldel))
                                {
                                    DB.ExecuteNonQuery("begin " + sqldel + " end;");
                                }
                                if (!string.IsNullOrWhiteSpace(sql))
                                {
                                    DB.ExecuteNonQuery("begin "+sql+" end;",dic);
                                }
                            }

                        }
                    }
                    //生成不良报告初始数据
                    if (determine=="1")//determine=1就是fail
                    {
                        sql = $@"SELECT ID FROM QCM_IQC_INSP_RES_BAD_REPORT WHERE CHK_NO='{chk_no}' AND ITEM_NO='{item_no}' AND CHK_SEQ='{chk_seq}' and isdelete='0'";
                        string ID = DB.GetString(sql);
                        decimal bad_qty = Convert.ToDecimal(rcpt_qty) - Convert.ToDecimal(pass_qty);//不良数量=进仓-合格
                        string bad_rate = ((bad_qty / Convert.ToDecimal(rcpt_qty)) * 100).ToString("F");
                        if (string.IsNullOrWhiteSpace(ID))
                        {
                            sql = $@"insert into qcm_iqc_insp_res_bad_report(chk_no,item_no,chk_seq,closing_status,warehousing_qty,bad_qty,bad_rate,spc_mining,actual_returned_qty,supplementary_delivery_qty
,createby,createdate,createtime) VALUES('{chk_no}','{item_no}','{chk_seq}','1','{rcpt_qty}','{bad_qty}','{bad_rate}','0','0','0','{userCode}','{datas}','{time}')";
                           
                        }
                        else
                        {
                            sql = $@"update qcm_iqc_insp_res_bad_report set bad_qty='{bad_qty}', closing_status='1',bad_rate='{bad_rate}',actual_returned_qty={bad_qty}-spc_mining,modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where chk_no='{chk_no}' and  item_no='{item_no}' and  chk_seq='{chk_seq}'";
                        }
                        DB.ExecuteNonQuery(sql);
                    }



                }
                Console.WriteLine();
                DB.Commit();
                ret.ErrMsg = "Saved successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Save failed, reason：" + ex.Message;
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


                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                var sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = $@"SELECT
   A.ID,
  A.CHK_NO,
  A.BADPROBLEM_CODE,
	A.TEST_ITEM_NO AS INSPECTION_CODE,
	A.TEST_ITEM_NAME AS INSPECTION_NAME,
	A.TEST_STANDARD AS INSPECTION_STANDARD,
	B.BADPROBLEM_NAME AS REMARKS,
	A.DETERMINE
FROM
    QCM_IQC_INSP_RES_D A LEFT JOIN QCM_IQC_BADPROBLEMS_M B ON A.BADPROBLEM_CODE=B.BADPROBLEM_CODE WHERE A.CHK_NO='{CHK_NO}' AND A.CHK_SEQ='{CHK_SEQ}' AND A.ITEM_NO='{ITEM_NO}' AND ISDELETE='0'  ORDER BY A.TEST_ITEM_NO ASC";
                dic.Add("CHK_SEQ", CHK_SEQ);
                dic.Add("ITEM_NO", ITEM_NO);
                dic.Add("CHK_NO", CHK_NO);
                DataTable dt = DB.GetDataTable(sql,dic);
                if (dt.Rows.Count == 0)
                {
                    //外观检测项目-材料
                    dic = new Dictionary<string, object>();
                    sql = $@"SELECT ''AS ID, INSPECTION_CODE,INSPECTION_NAME,QC_TYPE,JUDGMENT_CRITERIA,'' AS BADPROBLEM_CODE,'' AS CHK_NO,'' AS DETERMINE,'' AS IMAGE_GUID,STANDARD_VALUE AS INSPECTION_STANDARD,'' AS REMARKS FROM BDM_MATERIAL_INSPECTION_M WHERE QC_TYPE=@QC_TYPE ORDER BY INSPECTION_CODE ASC";
                    dic.Add("QC_TYPE", "3");
                    dt = DB.GetDataTable(sql,dic);
                }
                sql= $@"SELECT

    a.REMARK,
	a.STAFF_NO,
	b.STAFF_NAME,
	a.INSPECTIONDATE,
	a.CHK_NO,
	a.PASS_QTY,
	a.CHK_SEQ,
	a.ITEM_NO,
	a.BAD_QTY,
	a.DETERMINE,
	a.CLOSING_STATUS,
	a.SAMPLE_QTY,
	nvl(a.testlevel, 2) as testlevel,
	nvl(a.aql_level, 13) as aql_level,
	a.AC_RE
FROM

    QCM_IQC_INSP_RES_M a LEFT JOIN HR001M b on a.STAFF_NO = b.STAFF_NO WHERE a.CHK_NO =@CHK_NO and a.CHK_SEQ =@CHK_SEQ and a.ITEM_NO =@ITEM_NO and a.ISDELETE =@ISDELETE";
                dic = new Dictionary<string, object>();
                dic.Add("CHK_NO", CHK_NO);
                dic.Add("CHK_SEQ", CHK_SEQ);
                dic.Add("ITEM_NO", ITEM_NO);
                dic.Add("ISDELETE", "0");
                DataTable dt2 = DB.GetDataTable(sql,dic);
            
                sql = $"select closing_status  from qcm_iqc_insp_res_bad_report where CHK_NO='{CHK_NO}' and CHK_SEQ='{CHK_SEQ}' and ITEM_NO='{ITEM_NO}'   and rownum=1 and isdelete!='1'";
                dic = new Dictionary<string, object>();
                dic.Add("CHK_NO", CHK_NO);
                dic.Add("CHK_SEQ", CHK_SEQ);
                dic.Add("ITEM_NO", CHK_NO);
                string status = DB.GetString(sql,dic);
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data2",dt2);
                dic.Add("status", status);
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
        /// 测试报告查看
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultCSView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";
                string TASK_NO = jarr.ContainsKey("TASK_NO") ? jarr["TASK_NO"].ToString() : "";
                string INSPECTION_CODE = jarr.ContainsKey("INSPECTION_CODE") ? jarr["INSPECTION_CODE"].ToString() : "";
                var sql = string.Empty;

                sql = $@"SELECT
                task_no,
                art_no,
                shoe_no,
                material_way,
                category_code,
                category_name,
                product_level_code,
                product_level_value,
                season,
                pb_type_code,
                pb_type_level,
                gender,
                phase_creation_no,
                phase_creation_name,
                send_test_qty,
                order_po,
                order_po_qty,
                fgt_no,
                fgt_name,
                test_reason,
                staff_no,
                staff_name,
                staff_department,
                task_state,
                test_type,
                workmanship,
                parts_code,
                position_code,
                manufacturer_code,
                parts_name,
                position_name,
                manufacturer_name,
                line_code,
                line_name,
                colors,
                glue,
                material_code,
                material_name,
                makings_type_code,
                makings_type_name,
                makings_id,
                test_result,
                submission_date,
                completion_date
                FROM
                qcm_ex_task_list_m where ID='{ID}'";
                DataTable dt = DB.GetDataTable(sql);

                sql = $@"
                SELECT
                TASK_NO,
                SOURCE,
                INSPECTION_CODE,
                INSPECTION_NAME,
                INSPECTION_TYPE,
                JUDGE_MODE,
                STANDARD_VALUE,
                UNIT,
                SAMPLE_QTY,
                G_FORMULA_CODE,
                D_FORMULA_CODE,
                ART_D_REMARK,
                SUBMISSION_STATUS,
                ITEM_TEST_VAL,
                ITEM_TEST_RESULT,
                CHOICE_NO,
                CHOICE_NAME
                FROM 
                QCM_EX_TASK_LIST_D WHERE TASK_NO='{TASK_NO}' AND INSPECTION_CODE='{INSPECTION_CODE}'";
                DataTable dt2 = DB.GetDataTable(sql);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt2);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// 查看图片
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultCSViewimg(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                //转译
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                string TEST_ITEM_NO = jarr.ContainsKey("TEST_ITEM_NO") ? jarr["TEST_ITEM_NO"].ToString() : "";

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = $@"select id from qcm_iqc_insp_res_d where CHK_NO=@CHK_NO AND ITEM_NO=@ITEM_NO AND CHK_SEQ=@CHK_SEQ AND TEST_ITEM_NO=@TEST_ITEM_NO";
                dic.Add("CHK_NO", CHK_NO);
                dic.Add("ITEM_NO", ITEM_NO);
                dic.Add("CHK_SEQ", CHK_SEQ);
                dic.Add("TEST_ITEM_NO", TEST_ITEM_NO);
                string D_ID = DB.GetString(sql,dic);

                sql = $@"SELECT 'QCM_IQC_INSP_RES_D_F' as tablename ,a.id,B.FILE_NAME,B.FILE_URL,B.GUID,B.SUFFIX FROM QCM_IQC_INSP_RES_D_F A INNER  JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_ID=B.GUID WHERE A.D_ID=@D_ID";
                dic = new Dictionary<string, object>();
                dic.Add("D_ID", D_ID);
                DataTable dt= DB.GetDataTable(sql,dic);
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultCSViewupdateqy(object OBJ)
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
                //转译
                string qy = jarr.ContainsKey("qy") ? jarr["qy"].ToString() : "";//收料单号
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//收料单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//收料单号
                string chk_seq = jarr.ContainsKey("chk_seq") ? jarr["chk_seq"].ToString() : "";//收料单号
                Dictionary<string, object> dic = new Dictionary<string, object>();
               string sql = $@"UPDATE WMS_RCPT_D SET SAMPLING_STATUS=@SAMPLING_STATUS WHERE ITEM_NO=@ITEM_NO AND CHK_NO=@CHK_NO AND CHK_SEQ=@CHK_SEQ";
                dic.Add("SAMPLING_STATUS", qy);
                dic.Add("ITEM_NO", item_no);
                dic.Add("CHK_NO", chk_no);
                dic.Add("CHK_SEQ", chk_seq);
                DB.ExecuteNonQuery(sql,dic);
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
        //----------------------------免检厂商验收--------------------------------------

        /// <summary>
        /// 免检厂商验收数据展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMJView(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
       
                string SUPPLIERS_CODE = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//工厂
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//仓库

                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";//收料日期
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//收料日期

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                DateTime datas = DateTime.Now;
                var sql = string.Empty;
                string strwhere = string.Empty;
                string org_name = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();

                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and b.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                }
                if (!string.IsNullOrEmpty(SUPPLIERS_CODE))
                {
                    strwhere += $@" and a.SUPPLIERS_CODE like '%{SUPPLIERS_CODE}%'";
                }
                if (!string.IsNullOrEmpty(SUPPLIERS_NAME))
                {
                    strwhere += $@" and  a.SUPPLIERS_NAME like '%{SUPPLIERS_NAME}%'";
                }
                if (!string.IsNullOrEmpty(STOC_NO))
                {
                    strwhere += $@" and  b.STOC_NO = '{STOC_NO}'";
                }
                if (!string.IsNullOrEmpty(ORG_ID))
                {
                    strwhere += $@" and  b.ORG_ID = '{ORG_ID}'";//工厂
                }
                else
                {
                    sql = $@"SELECT
	org_code,
    org_name
FROM
	base001m where rownum<2 order by id asc";
                    DataTable dt1 = DB.GetDataTable(sql);
                    if (dt1.Rows.Count > 0)
                    {
                        strwhere += $@" and  b.ORG_ID = '{dt1.Rows[0]["org_code"]}'";
                        //dic.Add("ORG_ID", $@"%{dt1.Rows[0]["org_code"]}%");
                        org_name = dt1.Rows[0]["org_name"].ToString();
                    }

                }
                /*   sql = $@"
  select g.id,g.SUPPLIERS_CODE,g.SUPPLIERS_NAME,g.DJ,g.JC,g.STOC_NO,MM.WAREHOUSE_NAME,g.ORG_ID,m1.ORG_NAME from (select id,SUPPLIERS_CODE,SUPPLIERS_NAME,(JC-DJ)AS DJ,JC,STOC_NO,org_id from(
  SELECT KL.id,KL.SUPPLIERS_CODE,KL.SUPPLIERS_NAME,NVL(dl.DJ,0) AS DJ,kl.JC,kl.STOC_NO,kl.org_id FROM (
  select id,SUPPLIERS_CODE,SUPPLIERS_NAME, COUNT(CHK_NO) as JC,STOC_NO,org_id  From (SELECT
    DISTINCT
    a.SUPPLIERS_CODE,--生产厂商代号
    a.SUPPLIERS_NAME,--生产厂商名称
    c.CHK_NO,
    c.item_no,
    b.STOC_NO, --仓库
    a.id,
    b.org_id
  FROM
    base003m a
  LEFT JOIN bdm_rd_item e ON a.SUPPLIERS_CODE=e.vend_no_prd
  LEFT JOIN wms_rcpt_d c on c.item_no=e.item_no
  LEFT  JOIN wms_rcpt_m b on c.chk_no=b.chk_no

  )t

  GROUP BY SUPPLIERS_CODE,SUPPLIERS_NAME,STOC_NO,org_id,id) kl LEFT JOIN(
  select id,SUPPLIERS_CODE,SUPPLIERS_NAME,COUNT(CHK_NO)as DJ,org_id,STOC_NO From (SELECT
    DISTINCT
    a.SUPPLIERS_CODE,--生产厂商代号
    a.SUPPLIERS_NAME,--生产厂商名称
    d.CHK_NO,
    c.item_no,
    B.STOC_NO,
    a.id,
    B.org_id
  FROM
    base003m a 
  LEFT JOIN bdm_rd_item e ON a.SUPPLIERS_CODE=e.vend_no_prd
  LEFT JOIN wms_rcpt_d c on c.item_no=e.item_no
  LEFT  JOIN wms_rcpt_m  b on c.CHK_NO=b.CHK_NO 
  LEFT JOIN qcm_iqc_insp_res_m d  on c.CHK_NO=d.chk_no 
  where d.CHK_NO is not  null
  )
  GROUP BY SUPPLIERS_CODE,SUPPLIERS_NAME,STOC_NO,org_id,id ) dl on kl.SUPPLIERS_CODE=dl.SUPPLIERS_CODE and kl.STOC_NO=dl.STOC_NO and  kl.org_id=dl.org_id) where  SUPPLIERS_CODE is not null AND JC!='0') g 
  left join base001m m1 on g.ORG_ID=m1.ORG_CODE 
  LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON g.STOC_NO=MM.WAREHOUSE_CODE AND g.ORG_ID=MM.ORG_ID
   where  1=1 {strwhere} order by id asc
   ";*/

                sql = $@"select id,SUPPLIERS_CODE,SUPPLIERS_NAME,STOC_NO,WAREHOUSE_NAME,org_id,ORG_NAME,(COUNT(CHK_NO)-count(CHK_NO2)) as DJ,COUNT(CHK_NO) as JC  From (SELECT
  DISTINCT
  a.SUPPLIERS_CODE,--生产厂商代号
  a.SUPPLIERS_NAME,--生产厂商名称
 (b.CHK_NO || c.chk_seq) as CHK_NO,
	(D.CHK_NO || d.chk_seq) AS CHK_NO2,
  c.item_no,
	 a.id,
  b.STOC_NO, --仓库
	MM.WAREHOUSE_NAME,
  b.org_id,
	m1.ORG_NAME
FROM
  base003m a
LEFT JOIN bdm_rd_item e ON a.SUPPLIERS_CODE=e.vend_no_prd
LEFT JOIN wms_rcpt_d c on c.item_no=e.item_no
LEFT  JOIN wms_rcpt_m b on c.chk_no=b.chk_no
LEFT JOIN qcm_iqc_insp_res_m d  on b.CHK_NO=d.chk_no and c.chk_seq=d.chk_seq and c.item_no=d.item_no
left join base001m m1 on b.ORG_ID=m1.ORG_CODE 
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON b.STOC_NO=MM.WAREHOUSE_CODE AND b.ORG_ID=MM.ORG_ID
where B.RCPT_BY='01' AND  a.SUPPLIERS_CODE is not null {strwhere}
)t
GROUP BY SUPPLIERS_CODE,SUPPLIERS_NAME,STOC_NO,WAREHOUSE_NAME,org_id,ORG_NAME,id
HAVING COUNT(CHK_NO)!=0 
order by id asc
";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql);
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("rowCount", rowCount);
                dic.Add("org_name", org_name);
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
        /// 厂商验收一键免检
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMJUpdate(object OBJ)
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
                List<Dictionary<string,object>> dic= Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(Data);
                //转译
             
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                Dictionary<string, object> dictop = new Dictionary<string, object>();
                //厂商代号找
                if (dic != null)
                {
                    
                    //sql = "select CHK_NO,ITEM_NO,CHK_SEQ from qcm_iqc_insp_res_m";
                    //DataTable dt2 = DB.GetDataTable(sql);
                    string sql2 = string.Empty;
                    Dictionary<string, object> dic2 = new Dictionary<string, object>();
                    DataRow[] dr = null;
                    DataRow[] drbot = null;
                    int index = 0;
                    string whereSql = "";
                    if (dic.Count > 0)
                    {
                        string putin_date = dic[0].ContainsKey("putin_date") ? dic[0]["putin_date"].ToString() : "";//收料日期
                        string end_date = dic[0].ContainsKey("end_date") ? dic[0]["end_date"].ToString() : "";//收料日期
                        if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                        {
                            whereSql += $@" and d.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                        }
                    }
                    string update_item_sql = "";
                    Dictionary<string, object> update_item_dic = new Dictionary<string, object>();
                    int update_item_index = 0;
                    foreach (Dictionary<string, object> item in dic)
                    {
                        sql = $@"SELECT DISTINCT
	c.CHK_NO,
	c.ITEM_NO,
	c.CHK_SEQ,
	a.SUPPLIERS_CODE,
  c.ORG_ID,
	c.STOC_NO,
  c.RCPT_QTY
FROM
	BASE003M A
LEFT JOIN bdm_rd_item b ON a.SUPPLIERS_CODE=b.vend_no_prd
LEFT JOIN wms_rcpt_d c on c.item_no=b.item_no
LEFT  JOIN wms_rcpt_m d on c.chk_no=d.chk_no
where d.RCPT_BY='01' and  a.SUPPLIERS_CODE=@SUPPLIERS_CODE and c.STOC_NO=@STOC_NO and c.ORG_ID=@ORG_ID {whereSql} ";
                        dictop = new Dictionary<string, object>();
                        dictop.Add("SUPPLIERS_CODE", item["SUPPLIERS_CODE"]);
                        dictop.Add("STOC_NO", item["STOC_NO"]);
                        dictop.Add("ORG_ID", item["ORG_ID"]);
                        DataTable dt = DB.GetDataTable(sql, dictop);
                        if (dt.Rows.Count > 0)
                        {
                            var rowList = dt.Select();
                            var dt2 = DB.GetDataTable($@"select CHK_NO,ITEM_NO,CHK_SEQ from qcm_iqc_insp_res_m where {string.Join(" or ", rowList.Select(x => $@"(CHK_NO='{ x["CHK_NO"]}'and ITEM_NO='{x["ITEM_NO"]}' and CHK_SEQ='{x["CHK_SEQ"]}')"))}");
                            foreach (DataRow dr2 in dt.Rows)
                            {
                                //获取acre和抽验数量
                                string TESTLEVEL = "2";//检验水平
                                string AQL_LEVEL = "13";//AQL级别
                                string AC_RE = "";//ac re
                                string SAMPLE_QTY = "";//抽样数量
                                GetAcReAndSampleQty(DB, AQL_LEVEL, TESTLEVEL, dr2["RCPT_QTY"].ToString(), out AC_RE, out SAMPLE_QTY);

                                //var drCount = DB.GetInt32($@"select count(1) from qcm_iqc_insp_res_m where CHK_NO='{dr2["CHK_NO"]}' and ITEM_NO='{dr2["ITEM_NO"]}' and CHK_SEQ='{dr2["CHK_SEQ"]}'");
                                //DataRow[] dr = dt.Select($"CHK_NO='{item["CHK_NO"]}' and ITEM_NO='{item["ITEM_NO"]}' and CHK_SEQ='{item["CHK_SEQ"]}'");
                                drbot = dt2.Select($"CHK_NO='{ dr2["CHK_NO"]}'and ITEM_NO='{dr2["ITEM_NO"]}' and CHK_SEQ='{dr2["CHK_SEQ"]}'");

                                index++;
                                // 头状态 更新
                                if (drbot.Length > 0)
                                {
                                    continue;
                                    sql2 += $@"update qcm_iqc_insp_res_m set isdelete='0',determine='0',pass_qty='{dr2["RCPT_QTY"]}',modifyby='{user}',modifydate='{date}',modifytime='{time}',staff_no='{user}',Inspectiondate='{date}',TESTLEVEL='{TESTLEVEL}',AQL_LEVEL='{AQL_LEVEL}',AC_RE='{AC_RE}',SAMPLE_QTY={SAMPLE_QTY} where CHK_NO='{dr2["CHK_NO"]}' and ITEM_NO='{ dr2["ITEM_NO"]}' AND CHK_SEQ='{dr2["CHK_SEQ"]}' ;";
                                }
                                else
                                {
                                    sql2 += $@" insert into qcm_iqc_insp_res_m(chk_no,item_no,chk_seq,pass_qty,determine,createby,createdate,createtime,staff_no,Inspectiondate,isdelete,TESTLEVEL,AQL_LEVEL,AC_RE,SAMPLE_QTY) values('{dr2["CHK_NO"]}','{dr2["ITEM_NO"]}','{ dr2["CHK_SEQ"]}','{dr2["RCPT_QTY"]}','{0}','{user}','{date}','{time}','{user}','{date}','0','{TESTLEVEL}','{AQL_LEVEL}','{AC_RE}',{SAMPLE_QTY});";
                                }

                                string detailSql = $@"
SELECT
	A . ID,
	A .CHK_NO,
	A .BADPROBLEM_CODE,
	A .TEST_ITEM_NO,
	A .TEST_ITEM_NAME,
	A .TEST_STANDARD,
	B.BADPROBLEM_NAME AS REMARKS,
	A .DETERMINE
FROM
	QCM_IQC_INSP_RES_D A
LEFT JOIN QCM_IQC_BADPROBLEMS_M B ON A .BADPROBLEM_CODE = B.BADPROBLEM_CODE
WHERE
	A .CHK_NO = '{dr2["CHK_NO"]}'
AND A .CHK_SEQ = '{dr2["CHK_SEQ"]}'
AND A .ITEM_NO = '{dr2["ITEM_NO"]}'
AND ISDELETE = '0'
ORDER BY
	A .TEST_ITEM_NO ASC
";
                                var detail_dt = DB.GetDataTable(detailSql);
                                if (detail_dt.Rows.Count == 0)
                                {
                                    detailSql = $@"
SELECT
	INSPECTION_CODE as TEST_ITEM_NO,
	INSPECTION_NAME as TEST_ITEM_NAME,
	QC_TYPE,
	JUDGMENT_CRITERIA,
	'' AS BADPROBLEM_CODE,
	'{dr2["CHK_NO"]}' AS CHK_NO,
	'0' AS DETERMINE,
	'' AS IMAGE_GUID,
	STANDARD_VALUE AS TEST_STANDARD,
	'' AS REMARKS
FROM
	BDM_MATERIAL_INSPECTION_M
WHERE
	QC_TYPE ='3'
ORDER BY
	INSPECTION_CODE ASC
";
                                    detail_dt = DB.GetDataTable(detailSql);
                                }

                                string CHK_NO = dr2["CHK_NO"].ToString();
                                string CHK_SEQ = dr2["CHK_SEQ"].ToString();
                                string ITEM_NO = dr2["ITEM_NO"].ToString();
                                if (CHK_NO == "SL100811064790" && CHK_SEQ == "390" && ITEM_NO == "4041301931")
                                {

                                }
                                var detail_list = detail_dt.ToDataList<qcm_iqc_insp_res_d_TOUPPER>();
                                // 明细 更新
                                foreach (var detail_item in detail_list)
                                {
                                string detail_item_sql = $@"select chk_no from qcm_iqc_insp_res_d where chk_no=@chk_no and test_item_no=@test_item_no and item_no=@item_no and chk_seq=@chk_seq";
                                  var   detail_dic = new Dictionary<string, object>();
                                    detail_dic.Add("chk_no", CHK_NO);
                                    detail_dic.Add("test_item_no", detail_item.TEST_ITEM_NO);
                                    detail_dic.Add("item_no", ITEM_NO);
                                    detail_dic.Add("chk_seq", CHK_SEQ);
                                    string chk_nos = DB.GetString(detail_item_sql, detail_dic);

                                    //if (detail_item.DETERMINE == "是")
                                    if (detail_item.DETERMINE == "Yes")
                                        detail_item.DETERMINE = "0";
                                    //if (detail_item.DETERMINE == "否")
                                    if (detail_item.DETERMINE == "No")
                                        detail_item.DETERMINE = "1";

                                    if (string.IsNullOrWhiteSpace(chk_nos))
                                    {
                                        update_item_sql += $@"insert into qcm_iqc_insp_res_d (chk_no,item_no,chk_seq,badproblem_code,test_item_no,test_item_name,test_standard,determine,remark,createby,createdate,createtime,ISDELETE) values(@chk_no{update_item_index},@item_no{update_item_index},@chk_seq{update_item_index},@badproblem_code{update_item_index},@test_item_no{update_item_index},@test_item_name{update_item_index},@test_standard{update_item_index},@determine{update_item_index},@remark{update_item_index},@createby{update_item_index},@createdate{update_item_index},@createtime{update_item_index},'0');";
                                        update_item_dic.Add($@"chk_no{update_item_index}", CHK_NO);
                                        update_item_dic.Add($@"item_no{update_item_index}", ITEM_NO);
                                        update_item_dic.Add($@"chk_seq{update_item_index}", CHK_SEQ);
                                        update_item_dic.Add($@"badproblem_code{update_item_index}", detail_item.BADPROBLEM_CODE);
                                        update_item_dic.Add($@"test_item_no{update_item_index}", detail_item.TEST_ITEM_NO);
                                        update_item_dic.Add($@"test_item_name{update_item_index}", detail_item.TEST_ITEM_NAME);
                                        update_item_dic.Add($@"test_standard{update_item_index}", detail_item.TEST_STANDARD);
                                        update_item_dic.Add($@"determine{update_item_index}", "0");
                                        update_item_dic.Add($@"remark{update_item_index}", detail_item.REMARK);
                                        update_item_dic.Add($@"createby{update_item_index}", userCode);
                                        update_item_dic.Add($@"createdate{update_item_index}", date);
                                        update_item_dic.Add($@"createtime{update_item_index}", time);
                                        //DB.ExecuteNonQuery(detail_item_sql, detail_dic);
                                        //SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_d");
                                    }
                                    else
                                    {
                                        update_item_sql += $@"update qcm_iqc_insp_res_d
                                           set
                                          test_item_name=@test_item_name{update_item_index},
                                          test_standard=@test_standard{update_item_index},
                                          determine=@determine{update_item_index},
                                          badproblem_code=@badproblem_code{update_item_index},
                                          remark=@remark{update_item_index},
                                          isdelete='0',
                                          modifyby=@modifyby{update_item_index},
                                          modifydate=@modifydate{update_item_index},
                                          modifytime=@modifytime{update_item_index}
                                         where
	                                       chk_no =@chk_no{update_item_index} and test_item_no=@test_item_no{update_item_index} and item_no=@item_no{update_item_index} and chk_seq=@chk_seq{update_item_index};";
                                        update_item_dic.Add($@"test_item_name{update_item_index}", detail_item.TEST_ITEM_NAME);
                                        update_item_dic.Add($@"test_standard{update_item_index}", detail_item.TEST_STANDARD);
                                        update_item_dic.Add($@"determine{update_item_index}", "0");
                                        update_item_dic.Add($@"badproblem_code{update_item_index}", detail_item.BADPROBLEM_CODE);
                                        update_item_dic.Add($@"remark{update_item_index}", detail_item.REMARK);
                                        update_item_dic.Add($@"modifyby{update_item_index}", userCode);
                                        update_item_dic.Add($@"modifydate{update_item_index}", date);
                                        update_item_dic.Add($@"modifytime{update_item_index}", time);
                                        update_item_dic.Add($@"chk_no{update_item_index}", CHK_NO);
                                        update_item_dic.Add($@"test_item_no{update_item_index}", detail_item.TEST_ITEM_NO);
                                        update_item_dic.Add($@"item_no{update_item_index}", ITEM_NO);
                                        update_item_dic.Add($@"chk_seq{update_item_index}", CHK_NO);
                                        //DB.ExecuteNonQuery(detail_item_sql, detail_dic);
                                    }

                                    update_item_index++;
                                }

                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sql2))
                    {
                        DB.ExecuteNonQuery($@"begin {sql2}  end;",dic2);
                    }
                    if (!string.IsNullOrWhiteSpace(update_item_sql))
                    {
                        DB.ExecuteNonQuery($@"begin {update_item_sql}  end;", update_item_dic);
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No corresponding data is passed in, please check";
                    return ret;
                }
             
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
        /// 
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="rank_code">aql等级</param>
        /// <param name="level_code">检验水平</param>
        /// <param name="qty">进仓数量</param>
        /// <param name="AC_RE_res"></param>
        /// <param name="sample_qty_res"></param>
        public static void GetAcReAndSampleQty(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string rank_code, string level_code, string qty, out string AC_RE_res, out string sample_qty_res)
        {
            if (string.IsNullOrEmpty(rank_code) || string.IsNullOrEmpty(level_code) || string.IsNullOrEmpty(qty))
            {
                throw new Exception("Query AQL level failed");
            }
            string index = Convert.ToInt32(rank_code).ToString("00");
            var sql = string.Empty;
            sql = $@"
SELECT  
	AC{index} AC,
	AC{index} RE,
	VAL{index} sample_qty
FROM BDM_AQL_M 
WHERE HORI_TYPE='1' AND LEVEL_TYPE='{level_code}' AND (START_QTY<={qty} AND END_QTY>={qty}) AND ROWNUM=1";
            DataTable dt_acre = DB.GetDataTable(sql);
            if (dt_acre.Rows.Count > 0)
            {
                DataRow firstRow = dt_acre.Rows[0];
                int ac = 0;
                int.TryParse(firstRow["AC"].ToString(), out ac);
                int sample_qty = 0;
                int.TryParse(firstRow["sample_qty"].ToString(), out sample_qty);

                var lis = new
                {
                    AC = ac,
                    RE = ac + 1,
                    AC_RE = ac + "," + (ac + 1),
                    sample_qty
                };
                AC_RE_res = ac + "," + (ac + 1);
                sample_qty_res = sample_qty.ToString();
            }
            else
            {
                throw new Exception("AQL level not maintained");
            }
        }

        /// <summary>
        /// 免检确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMJQCView(object OBJ)
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

                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//采购厂商
                string VEND_NO2 = jarr.ContainsKey("VEND_NO2") ? jarr["VEND_NO2"].ToString() : "";//生产厂商
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//仓库代号
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//工厂代号

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                DateTime datas = DateTime.Now;
                var sql = string.Empty;
                string strwhere = string.Empty;
                string swhere = string.Empty;
                Dictionary<string, object> dic= new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and A.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                    //dic.Add("putin_date", putin_date);
                    //dic.Add("end_date", end_date);
                }
             
                if (!string.IsNullOrEmpty(VEND_NO2))
                {
                    strwhere += $@" and b.SUPPLIERS_CODE = '{VEND_NO2}'";//生产厂商
                    //dic.Add("VEND_NO2", $@"%{VEND_NO2}%");
                }
                if (!string.IsNullOrEmpty(STOC_NO))
                {
                    strwhere += $@" and  A.STOC_NO = '{STOC_NO}'";
                    dic.Add("STOC_NO", $@"%{STOC_NO}%");
                }
                if (!string.IsNullOrWhiteSpace(ORG_ID))
                {
                    strwhere += $@" and  A.ORG_ID = '{ORG_ID}'";
                    //dic.Add("ORG_ID", $@"%{ORG_ID}%");
                }
                    sql = $@"SELECT
    DISTINCT
    A.INSERT_DATE AS datas
FROM
WMS_RCPT_M A 
INNER  JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
INNER  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
 WHERE a.RCPT_BY='01' {strwhere}";
                DataTable dt_desc = DB.GetDataTable(sql,dic);
                string str_desc = string.Empty;
                if (dt_desc.Rows.Count < 3)
                {
                    str_desc = $@",item_no desc";//出现多个相同的添加时间，做多一个排序的条件
                }
                if (!string.IsNullOrEmpty(VEND_NO))
                {
                    swhere += $@" and  B1.SUPPLIERS_NAME like '%{VEND_NO}%'";//采购厂商
                    //dic.Add("VEND_NO", $@"%{VEND_NO}%");
                }
                string searchKeys = $@"DISTINCT
    pe.ITEM_TYPE_NO,
	QW.JYZT,
	QW.RCPT_DATE,
    QW.datas,
	QW.SUPPLIERS_CODE,
	QW.SUPPLIERS_NAME,
	QW.CHK_NO,
	QW.ITEM_NO,
    QW.CHK_SEQ,
   qw.SOURCE_SEQ, --来源单序号
	B1.SUPPLIERS_CODE AS SUPPLIERS_CODE2, --采购厂商编号
	B1.SUPPLIERS_NAME AS SUPPLIERS_NAME2, --采购厂商
	S.INSPECTIONDATE,--外观检验日期
	'' AS SHDW,--收货单位
	C1.ORD_QTY, --采购数量
	a2.ORDER_NO, --采购单号
	S.DETERMINE,--外观结果
	qw.SAMPLING_STATUS,-- 测试取样状况
	EX.test_result AS CSJG,--测试结果
    S.staff_no, --检验员编号
	hm.staff_name,--检验员名称
    QW.RCPT_QTY, --收料数量
	 QW.RCPT_QTY as  IV_QTY,
    rt.actual_returned_qty  AS YTS,--验退数
s.PASS_QTY, --合格数
	rt.supplementary_delivery_qty as BS,--补送
	QW.STOC_NO,--仓库代号
    mm.WAREHOUSE_NAME,--仓库名称
	QW.SOURCE_NO,
	QW.ORG_ID,--工厂代号
    M1.ORG_NAME,--工厂名称
    qw.RCPT_BY";
                sql = $@"

SELECT
	{searchKeys}
FROM 
(SELECT
    A1.CHK_SEQ,
	'' AS JYZT ,--检验状态
	A.RCPT_DATE, --收货日期
    A.INSERT_DATE AS datas,
    A1.SOURCE_SEQ, --来源单序号
	B.SUPPLIERS_CODE, --生产厂商编号
	B.SUPPLIERS_NAME, --生产厂商
	A.CHK_NO,--收料单号
	A1.ITEM_NO, --料号(材料编号)
	a1.RCPT_QTY,
	A1.IV_QTY, --检验数
	A.STOC_NO, --仓库代号
    A.ORG_ID, --工厂代号
    (
    CASE WHEN A1.SAMPLING_STATUS='I' THEN '历史数据'
    ELSE
      a1.SAMPLING_STATUS 
    END
  ) SAMPLING_STATUS,-- 测试取样状况
	A1.SOURCE_NO,
    a.RCPT_BY
FROM
	WMS_RCPT_M A 
INNER  JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
INNER  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
 WHERE  1=1  {strwhere}) QW 
LEFT JOIN BDM_RD_ITEM AW ON QW.ITEM_NO=AW.ITEM_NO
LEFT   JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN BDM_RD_PROD PD ON AW.PARENT_ITEM_NO=PD.PROD_NO
LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON QW.SOURCE_NO=A2.ORDER_NO
LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE
LEFT  JOIN BASE001M M1 ON QW.ORG_ID=M1.ORG_CODE
LEFT JOIN QCM_IQC_INSP_RES_M S ON QW.CHK_NO=S.CHK_NO AND QW.ITEM_NO=S.ITEM_NO AND QW.CHK_SEQ=S.CHK_SEQ
LEFT JOIN  (SELECT ORDER_NO,PART_NO,item_no,SUM(ORD_QTY)AS ORD_QTY FROM BDM_PURCHASE_ORDER_ITEM C1 GROUP BY ORDER_NO,PART_NO,item_no) C1 ON A2.ORDER_NO=C1.ORDER_NO and qw.item_no=c1.item_no
LEFT JOIN qcm_iqc_insp_res_bad_report rt on QW.CHK_NO=rt.CHK_NO  and QW.ITEM_NO=rt.ITEM_NO and QW.CHK_SEQ=rt.CHK_SEQ
LEFT JOIN qcm_ex_task_list_m EX ON QW.ITEM_NO=EX.material_code
LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON QW.STOC_NO=MM.WAREHOUSE_CODE AND QW.ORG_ID=MM.ORG_ID
where qw.RCPT_BY='01' {swhere}  order by qw.datas desc{str_desc}";

                
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"));
                if (dt.Rows.Count > 0)
                {
                    sql = "SELECT DETERMINE,CHK_NO,ITEM_NO,CHK_SEQ FROM QCM_IQC_INSP_RES_M";
                    DataTable dt_status = DB.GetDataTable(sql);
                    DataRow[] dr = null;
                    DataTable dtrow = new DataTable();
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["ORDER_NO"].ToString()))
                        {
                            //数据量过大就条件筛选来查
                           string sql2 = $@"
select 
    DISTINCT
	PR_UNIT
from bdm_purchase_order_item  
where order_no ='{item["ORDER_NO"]}' and order_seq ='{item["SOURCE_SEQ"]}'";
                            dic = new Dictionary<string, object>();
                            dic.Add("order_no", item["ORDER_NO"]);
                            dic.Add("order_seq", item["SOURCE_SEQ"]);
                            dtrow = DB.GetDataTable(sql2);
                            if (dtrow.Rows.Count > 0)
                            {
                                item["SHDW"] = dtrow.Rows[0]["PR_UNIT"].ToString();
                            }
                        }
                        //数据量小就这个
                        dr = dt_status.Select($@"CHK_NO='{item["CHK_NO"]}' AND ITEM_NO='{item["ITEM_NO"]}' AND CHK_SEQ='{item["CHK_SEQ"]}'");
                        if (dr.Length > 0)
                        {
                            item["JYZT"] = "已检验";
                        }
                        else
                        {
                            item["JYZT"] = "未检验";
                        }
                    }
                }
                dic = new Dictionary<string, object>();
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
        /// 免检确认
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMJQCView2(object OBJ)
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

                string VEND_NO = jarr.ContainsKey("VEND_NO") ? jarr["VEND_NO"].ToString() : "";//采购厂商
                string VEND_NO2 = jarr.ContainsKey("VEND_NO2") ? jarr["VEND_NO2"].ToString() : "";//生产厂商
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//仓库代号
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//工厂代号
                string jyzt = jarr.ContainsKey("jyzt") ? jarr["jyzt"].ToString() : "";//检验状态

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                DateTime datas = DateTime.Now;
                var sql = string.Empty;
                string strwhere = string.Empty;
                string swhere = string.Empty;
                string havingWhere = "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(putin_date) && !string.IsNullOrWhiteSpace(end_date))
                {
                    strwhere += $@" and A.RCPT_DATE BETWEEN TO_date('{putin_date}','yyyy-MM-dd') and TO_date('{end_date}','yyyy-MM-dd')";
                }

                if (!string.IsNullOrEmpty(jyzt))
                {
                    if (jyzt == "1")
                        havingWhere = $@"having max(s.CHK_NO) is not null";
                    else if (jyzt == "2")
                        havingWhere = $@"having max(s.CHK_NO) is null";
                }
                if (!string.IsNullOrEmpty(VEND_NO2))
                {
                    strwhere += $@" and b.SUPPLIERS_CODE = '{VEND_NO2}'";//生产厂商
                }
                if (!string.IsNullOrEmpty(STOC_NO))
                {
                    strwhere += $@" and  A.STOC_NO = '{STOC_NO}'";
                }
                if (!string.IsNullOrWhiteSpace(ORG_ID))
                {
                    strwhere += $@" and  A.ORG_ID = '{ORG_ID}'";
                }
                sql = $@"SELECT
    DISTINCT
    A.INSERT_DATE AS datas
FROM
WMS_RCPT_M A 
INNER  JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
INNER  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
 WHERE  a.RCPT_BY='01' {strwhere} AND ROWNUM<4";
                DataTable dt_desc = DB.GetDataTable(sql);
                string str_desc = string.Empty;
                if (dt_desc.Rows.Count < 3)
                {
                    str_desc = $@",MAX(A1.item_no) desc";//出现多个相同的添加时间，做多一个排序的条件
                }
                if (!string.IsNullOrEmpty(VEND_NO))
                {
                    swhere += $@" and  B1.SUPPLIERS_NAME like '%{VEND_NO}%'";//采购厂商
                }
                string searchKeys = $@"CASE
		when max(s.CHK_NO) is not null then 'Tested'
		else 'Not_Tested'
	END  AS JYZT ,--检验状态
	MAX(a.RCPT_DATE) RCPT_DATE, --收货日期
	MAX(aw.vend_no_prd) AS SUPPLIERS_CODE, --生产厂商编号
	MAX(b.SUPPLIERS_NAME) AS SUPPLIERS_NAME, --生产厂商
	MAX(a.CHK_NO) as CHK_NO,--收料单号
	MAX(a1.ITEM_NO) as ITEM_NO, --料号(材料编号)
	MAX(a1.CHK_SEQ) as CHK_SEQ,--料号序号
	MAX(aw.NAME_T) as NAME_T, --材料名称
	MAX(b1.SUPPLIERS_CODE) as SUPPLIERS_CODE2, --采购厂商编号
	MAX(b1.SUPPLIERS_NAME) as SUPPLIERS_NAME2, --采购厂商
    MAX(s.INSPECTIONDATE) as INSPECTIONDATE,--外观检验日期
	MAX(c111.PR_UNIT) as SHDW,--收货单位
    SUM(c111.ORD_QTY) as ORD_QTY, --采购数量
	MAX(a1.source_no) as ORDER_NO, --采购单号
	MAX(NVL(s.DETERMINE,2)) as DETERMINE,--外观结果
     (
     CASE WHEN MAX(A1.SAMPLING_STATUS)='I' THEN '历史数据'
        ELSE
     MAX(a1.SAMPLING_STATUS) 
      END
    ) SAMPLING_STATUS,-- 测试取样状况
	MAX(EX.TEST_RESULT) as CSJG,--测试结果
	MAX(S.staff_no) as staff_no, --检验员编号
	MAX(hm.staff_name) as staff_name,--检验员名称
	MAX(a1.RCPT_QTY) as IV_QTY, --检验数(收料数)
	MAX(s.PASS_QTY) as PASS_QTY, --合格数
	MAX(rt.actual_returned_qty) AS YTS,--实退退数
	MAX(rt.supplementary_delivery_qty) as BS,--补送
	MAX(a.STOC_NO) as STOC_NO, --仓库代号
	MAX(MM.WAREHOUSE_NAME) AS WAREHOUSE_NAME,--仓库名称
	MAX(a.ORG_ID) as ORG_ID, --工厂编号
	MAX(m1.ORG_NAME) as ORG_NAME --工厂名称";
                sql = $@"
SELECT
	searchKeys
FROM
    wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a1.CHK_NO=a.CHK_NO

INNER  JOIN bdm_purchase_order_m a2 on a2.ORDER_NO=a1.SOURCE_NO
LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=a1.SOURCE_SEQ --新加的
LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
LEFT JOIN base003m b1 ON b1.SUPPLIERS_CODE = a2.VEND_NO

LEFT JOIN bdm_rd_item aw on aw.ITEM_NO=a1.ITEM_NO
LEFT JOIN base003m b ON aw.vend_no_prd=b.SUPPLIERS_CODE

LEFT JOIN BDM_RD_ITEMTYPE pe on  pe.ITEM_TYPE_NO=AW.ITEM_TYPE
LEFT JOIN bdm_rd_prod pd on pd.PROD_NO=aw.PARENT_ITEM_NO
LEFT JOIN base001m m1 on m1.ORG_CODE=a.ORG_ID
LEFT JOIN qcm_iqc_insp_res_m s on s.CHK_NO=a.CHK_NO and s.CHK_SEQ=a1.CHK_SEQ and s.isdelete='0' 
LEFT JOIN qcm_iqc_insp_res_bad_report rt on rt.CHK_NO=a.CHK_NO and rt.CHK_SEQ=a1.CHK_SEQ AND rt.isdelete='0' 
LEFT JOIN bdm_purchase_order_item c111 ON c111.ORDER_NO=a1.SOURCE_NO AND c111.ORDER_SEQ=a1.CHK_SEQ
LEFT JOIN qcm_ex_task_list_m EX ON (EX.MATERIAL_CODE=a1.ITEM_NO AND instr(EX.SLDH, a1.CHK_NO) > 0)
LEFT JOIN HR001M hm on hm.staff_no=s.staff_no 
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
WHERE  a.RCPT_BY='01' {strwhere}
GROUP BY a1.CHK_NO,a1.CHK_SEQ {havingWhere} 
 order by MAX(a.INSERT_DATE) desc{str_desc}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"));
                dic = new Dictionary<string, object>();
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
        /// 免检确认一键免检
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultMJQUpdate(object OBJ)
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
                List<Dictionary<string, object>> dic_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(Data);
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH:mm:ss");//时间
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string sql = string.Empty;
                //sql= "select CHK_NO,ITEM_NO,CHK_SEQ from qcm_iqc_insp_res_m";
                //DataTable dt = DB.GetDataTable(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (dic_list != null && dic_list.Count() > 0)
                {
                    int index = 0;
                    sql = string.Empty;
                    string excute_item_sql = "";
                    var excute_item_dic = new Dictionary<string, object>();
                    int excute_item_index = 0;

                    // 查询下面循环需要用到的数据
//                    var count_dt = DB.GetDataTable($@"
//select 
//    CHK_NO,ITEM_NO,CHK_SEQ,count(1) as existcount
//from qcm_iqc_insp_res_m 
//where {string.Join(" or ", dic_list.Select(x=>$@"(CHK_NO='{x["CHK_NO"]}' and ITEM_NO='{x["ITEM_NO"]}' and CHK_SEQ='{x["CHK_SEQ"]}')"))} 
//group by CHK_NO,ITEM_NO,CHK_SEQ
//");

                    foreach (Dictionary<string, object> item in dic_list)
                    {
                        index++;
                        string RCPT_QTY = item["RCPT_QTY"] != null ? item["RCPT_QTY"].ToString() : "0";
                        var drCount = DB.GetInt32($@"select count(1) from qcm_iqc_insp_res_m where CHK_NO='{item["CHK_NO"]}' and ITEM_NO='{item["ITEM_NO"]}' and CHK_SEQ='{item["CHK_SEQ"]}'");

                        //获取acre和抽验数量
                        string TESTLEVEL = "2";//检验水平
                        string AQL_LEVEL = "13";//AQL级别
                        string AC_RE = "";//ac re
                        string SAMPLE_QTY = "";//抽样数量
                        GetAcReAndSampleQty(DB, AQL_LEVEL, TESTLEVEL, RCPT_QTY, out AC_RE, out SAMPLE_QTY);

                        //int drCount = 0;
                        //var find_count = count_dt.Select($@"CHK_NO='{item["CHK_NO"]}' and ITEM_NO='{item["ITEM_NO"]}' and CHK_SEQ='{item["CHK_SEQ"]}'");
                        //if (find_count != null && find_count.Length > 0)
                        //{
                        //    drCount= find_count
                        //}
                        if (drCount > 0)
                        {
                            continue;
                            dic.Add($@"determine{index}", "0");
                            dic.Add($@"pass_qty{index}", RCPT_QTY);
                            dic.Add($@"modifyby{index}", user);
                            dic.Add($@"modifydate{index}", date);
                            dic.Add($@"staff_no{index}", user);
                            dic.Add($@"Inspectiondate{index}", date);
                            dic.Add($@"modifytime{index}", time);
                            dic.Add($@"CHK_NO{index}", item["CHK_NO"]);
                            dic.Add($@"ITEM_NO{index}", item["ITEM_NO"]);
                            dic.Add($@"CHK_SEQ{index}", item["CHK_SEQ"]);
                            sql += $@"update qcm_iqc_insp_res_m set determine=@determine{index},pass_qty=@pass_qty{index},modifyby=@modifyby{index},modifydate=@modifydate{index},staff_no=@staff_no{index},Inspectiondate=@Inspectiondate{index},modifytime=@modifytime{index},isdelete='0' where CHK_NO=@CHK_NO{index}  AND ITEM_NO=@ITEM_NO{index} AND CHK_SEQ=@CHK_SEQ{index} ;";
                        }
                        else
                        {
                            dic.Add($@"chk_no{index}", item["CHK_NO"]);
                            dic.Add($@"item_no{index}", item["ITEM_NO"]);
                            dic.Add($@"chk_seq{index}", item["CHK_SEQ"]);
                            dic.Add($@"pass_qty{index}", RCPT_QTY);
                            dic.Add($@"determine{index}", "0");
                            dic.Add($@"createby{index}", user);
                            dic.Add($@"createdate{index}", date);
                            dic.Add($@"createtime{index}", time);
                            dic.Add($@"staff_no{index}", user);
                            dic.Add($@"Inspectiondate{index}", date);
                            sql += $@"insert into qcm_iqc_insp_res_m(chk_no,item_no,chk_seq,pass_qty,determine,createby,createdate,createtime,staff_no,Inspectiondate,isdelete,TESTLEVEL,AQL_LEVEL,AC_RE,SAMPLE_QTY) values(@chk_no{index},@item_no{index},@chk_seq{index},@pass_qty{index},@determine{index},@createby{index},@createdate{index},@createtime{index},@staff_no{index},@Inspectiondate{index},'0','{TESTLEVEL}','{AQL_LEVEL}','{AC_RE}',{SAMPLE_QTY});";
                        }

                        string detailSql = $@"
SELECT
	A . ID,
	A .CHK_NO,
	A .BADPROBLEM_CODE,
	A .TEST_ITEM_NO,
	A .TEST_ITEM_NAME,
	A .TEST_STANDARD,
	B.BADPROBLEM_NAME AS REMARKS,
	A .DETERMINE
FROM
	QCM_IQC_INSP_RES_D A
LEFT JOIN QCM_IQC_BADPROBLEMS_M B ON A .BADPROBLEM_CODE = B.BADPROBLEM_CODE
WHERE
	A .CHK_NO = '{item["CHK_NO"]}'
AND A .CHK_SEQ = '{item["CHK_SEQ"]}'
AND A .ITEM_NO = '{item["ITEM_NO"]}'
AND ISDELETE = '0'
ORDER BY
	A .TEST_ITEM_NO ASC
";
                        var detail_dt = DB.GetDataTable(detailSql);
                        if (detail_dt.Rows.Count == 0)
                        {
                            detailSql = $@"
SELECT
	INSPECTION_CODE as TEST_ITEM_NO,
	INSPECTION_NAME as TEST_ITEM_NAME,
	QC_TYPE,
	JUDGMENT_CRITERIA,
	'' AS BADPROBLEM_CODE,
	'{item["CHK_NO"]}' AS CHK_NO,
	'0' AS DETERMINE,
	'' AS IMAGE_GUID,
	STANDARD_VALUE AS TEST_STANDARD,
	'' AS REMARKS
FROM
	BDM_MATERIAL_INSPECTION_M
WHERE
	QC_TYPE ='3'
ORDER BY
	INSPECTION_CODE ASC
";
                            detail_dt = DB.GetDataTable(detailSql);
                        }

                        string CHK_NO = item["CHK_NO"].ToString();
                        string CHK_SEQ = item["CHK_SEQ"].ToString();
                        string ITEM_NO = item["ITEM_NO"].ToString();
                        var detail_list = detail_dt.ToDataList<qcm_iqc_insp_res_d_TOUPPER>();

                        if (detail_list.Count() > 0)
                        {

                            string detail_item_sql = $@"select chk_no,test_item_no,chk_seq,item_no from qcm_iqc_insp_res_d where chk_no=@chk_no and test_item_no IN ({string.Join(",", detail_list.Where(x => !string.IsNullOrEmpty(x.TEST_ITEM_NO)).Select(x => $@"'{x.TEST_ITEM_NO}'"))}) and chk_seq=@chk_seq and item_no=@item_no  ";
                            var detail_dic = new Dictionary<string, object>();
                            detail_dic.Add("chk_no", CHK_NO);
                            //detail_dic.Add("test_item_no", detail_item.TEST_ITEM_NO);
                            detail_dic.Add("chk_seq", CHK_SEQ);
                            detail_dic.Add("item_no", ITEM_NO);
                            var chk_nos_dt = DB.GetDataTable(detail_item_sql, detail_dic);
                            // 明细 更新
                            foreach (var detail_item in detail_list)
                            {
                                //string detail_item_sql = $@"select chk_no from qcm_iqc_insp_res_d where chk_no=@chk_no and test_item_no=@test_item_no and item_no=@item_no and chk_seq=@chk_seq ";
                                //var detail_dic = new Dictionary<string, object>();
                                //detail_dic.Add("chk_no", CHK_NO);
                                //detail_dic.Add("test_item_no", detail_item.TEST_ITEM_NO);
                                //detail_dic.Add("item_no", ITEM_NO);
                                //detail_dic.Add("chk_seq", CHK_SEQ);
                                string chk_nos = "";
                                var find_chk_nos = chk_nos_dt.Select($@"chk_no='{CHK_NO}' and test_item_no='{detail_item.TEST_ITEM_NO}' and item_no='{ITEM_NO}' and chk_seq='{CHK_SEQ}'");
                                if (find_chk_nos != null && find_chk_nos.Length > 0)
                                {
                                    chk_nos = find_chk_nos[0]["chk_no"].ToString();
                                }
                                //DB.GetString(detail_item_sql, detail_dic);

                               // if (detail_item.DETERMINE == "是")
                                if (detail_item.DETERMINE == "Yes")
                                    detail_item.DETERMINE = "0";
                                //if (detail_item.DETERMINE == "否")
                                if (detail_item.DETERMINE == "No")
                                    detail_item.DETERMINE = "1";

                                if (string.IsNullOrWhiteSpace(chk_nos))
                                {
                                    excute_item_sql += $@"insert into qcm_iqc_insp_res_d (chk_no,item_no,chk_seq,badproblem_code,test_item_no,test_item_name,test_standard,determine,remark,createby,createdate,createtime,ISDELETE) values(@chk_no{excute_item_index},@item_no{excute_item_index},@chk_seq{excute_item_index},@badproblem_code{excute_item_index},@test_item_no{excute_item_index},@test_item_name{excute_item_index},@test_standard{excute_item_index},@determine{excute_item_index},@remark{excute_item_index},@createby{excute_item_index},@createdate{excute_item_index},@createtime{excute_item_index},'0');";
                                    excute_item_dic.Add($@"chk_no{excute_item_index}", CHK_NO);
                                    excute_item_dic.Add($@"item_no{excute_item_index}", ITEM_NO);
                                    excute_item_dic.Add($@"chk_seq{excute_item_index}", CHK_SEQ);
                                    excute_item_dic.Add($@"badproblem_code{excute_item_index}", detail_item.BADPROBLEM_CODE);
                                    excute_item_dic.Add($@"test_item_no{excute_item_index}", detail_item.TEST_ITEM_NO);
                                    excute_item_dic.Add($@"test_item_name{excute_item_index}", detail_item.TEST_ITEM_NAME);
                                    excute_item_dic.Add($@"test_standard{excute_item_index}", detail_item.TEST_STANDARD);
                                    excute_item_dic.Add($@"determine{excute_item_index}", "0");
                                    excute_item_dic.Add($@"remark{excute_item_index}", detail_item.REMARK);
                                    excute_item_dic.Add($@"createby{excute_item_index}", userCode);
                                    excute_item_dic.Add($@"createdate{excute_item_index}", date);
                                    excute_item_dic.Add($@"createtime{excute_item_index}", time);
                                    //DB.ExecuteNonQuery(detail_item_sql, detail_dic);
                                    //SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_d");
                                }
                                else
                                {
                                    excute_item_sql += $@"update qcm_iqc_insp_res_d
                                           set
                                          test_item_name=@test_item_name{excute_item_index},
                                          test_standard=@test_standard{excute_item_index},
                                          determine=@determine{excute_item_index},
                                          badproblem_code=@badproblem_code{excute_item_index},
                                          remark=@remark{excute_item_index},
                                          isdelete='0',
                                          modifyby=@modifyby{excute_item_index},
                                          modifydate=@modifydate{excute_item_index},
                                          modifytime=@modifytime{excute_item_index}
                                         where
	                                       chk_no =@chk_no{excute_item_index} and test_item_no=@test_item_no{excute_item_index} and item_no=@item_no{excute_item_index} and chk_seq=@chk_seq{excute_item_index};";
                                    excute_item_dic.Add($@"test_item_name{excute_item_index}", detail_item.TEST_ITEM_NAME);
                                    excute_item_dic.Add($@"test_standard{excute_item_index}", detail_item.TEST_STANDARD);
                                    excute_item_dic.Add($@"determine{excute_item_index}", "0");
                                    excute_item_dic.Add($@"badproblem_code{excute_item_index}", detail_item.BADPROBLEM_CODE);
                                    excute_item_dic.Add($@"remark{excute_item_index}", detail_item.REMARK);
                                    excute_item_dic.Add($@"modifyby{excute_item_index}", userCode);
                                    excute_item_dic.Add($@"modifydate{excute_item_index}", date);
                                    excute_item_dic.Add($@"modifytime{excute_item_index}", time);
                                    excute_item_dic.Add($@"chk_no{excute_item_index}", CHK_NO);
                                    excute_item_dic.Add($@"test_item_no{excute_item_index}", detail_item.TEST_ITEM_NO);
                                    excute_item_dic.Add($@"item_no{excute_item_index}", ITEM_NO);
                                    excute_item_dic.Add($@"chk_seq{excute_item_index}", CHK_NO);
                                    //DB.ExecuteNonQuery(detail_item_sql, detail_dic);
                                }

                                excute_item_index++;
                            }
                        }

                    }
                    if (!string.IsNullOrWhiteSpace(sql))
                    {
                        DB.ExecuteNonQuery($@"begin {sql} end;", dic);
                    }
                    if (!string.IsNullOrWhiteSpace(excute_item_sql))
                    {
                        DB.ExecuteNonQuery($@"begin {excute_item_sql} end;", excute_item_dic);
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "No corresponding data is passed in, please check";
                    return ret;
                }
              
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

        //-------------------------------------PDA原材料进仓清单----------------------------------------------------------------------------------------//
        /// <summary>
        /// 扫描原材料进仓清单
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultPDAYCLViewA(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);


                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//料号
                string PUTIN_DATE = jarr.ContainsKey("PUTIN_DATE") ? jarr["PUTIN_DATE"].ToString() : "";//开始时间
                string END_DATE = jarr.ContainsKey("END_DATE") ? jarr["END_DATE"].ToString() : "";//结束时间
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                var sql = string.Empty;
                string strwhere = string.Empty;

                /*  if (!string.IsNullOrWhiteSpace(PUTIN_DATE) && string.IsNullOrWhiteSpace(END_DATE))
                  {
                      strwhere += $@" and a.RCPT_DATE=TO_date('{PUTIN_DATE}','yyyy-MM-dd')";
                  }
                  if (!string.IsNullOrWhiteSpace(END_DATE) && string.IsNullOrWhiteSpace(PUTIN_DATE))
                  {
                      strwhere += $@" and a.RCPT_DATE=TO_date('{END_DATE}','yyyy-MM-dd')";
                  }*/
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string str_desc = string.Empty;
                if (!string.IsNullOrWhiteSpace(PUTIN_DATE) && !string.IsNullOrWhiteSpace(END_DATE))
                {
                    strwhere += $@" and a.RCPT_DATE BETWEEN TO_date(@PUTIN_DATE,'yyyy-MM-dd') and TO_date(@END_DATE,'yyyy-MM-dd')";
                    dic.Add("PUTIN_DATE", PUTIN_DATE);
                    dic.Add("END_DATE", END_DATE);
                }
                if (!string.IsNullOrWhiteSpace(CHK_NO))
                {
                    strwhere += $@" and a.CHK_NO=@CHK_NO";
                    dic.Add("CHK_NO", CHK_NO);
                    sql = $@"SELECT
    DISTINCT
    b.INSERT_DATE
FROM
    wms_rcpt_m A
LEFT JOIN wms_rcpt_d b ON A .chk_no = b.chk_no
WHERE  a.RCPT_BY='01' and  A.chk_no =@CHK_NO and a.RCPT_BY='01'  and rownum<3";
                    DataTable dt_desc = DB.GetDataTable(sql, dic);

                    if (dt_desc.Rows.Count < 3 && dt_desc.Rows.Count > 0)
                    {
                        str_desc = $@",a1.ITEM_NO desc";//出现多个相同的添加时间，做多一个排序的条件
                    }
                }
                if (!string.IsNullOrWhiteSpace(ITEM_NO))
                {
                    strwhere += $@" and a1.ITEM_NO like @ITEM_NO";
                    dic.Add("ITEM_NO", $@"%{ITEM_NO}%");
                }
               
                sql = $@"select rownum as XH,c.* FROM (
SELECT
	DISTINCT
	a.RCPT_DATE as JCDATE, --收货日期
	a1.INSERT_DATE,
	a.CHK_NO,--收料单号
	a1.ITEM_NO, --料号(材料编号)
    a1.CHK_SEQ --材料序号
FROM
	wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a.CHK_NO=a1.CHK_NO
 where   a.RCPT_BY='01' and a.CHK_NO is not null and  a1.ITEM_NO  is not null and  a1.CHK_SEQ is not null   {strwhere}  order by a1.INSERT_DATE desc{str_desc}) c";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize),"",dic);
                string total = DB.GetString($@"SELECT
	COUNT(1)
FROM
	wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a.CHK_NO=a1.CHK_NO
where a.RCPT_BY='01' and a.CHK_NO is not null and  a1.ITEM_NO  is not null and  a1.CHK_SEQ is not null  {strwhere}
",dic);

                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("total", total);

                ret.IsSuccess = true;
                ret.RetData1 = dic;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 录入检验结果带出数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultPDAYCLViewB(object OBJ)
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
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                var sql = string.Empty;
                sql = $@"SELECT
	DISTINCT
	aw.vend_no_prd as SUPPLIERS_CODE, --生产厂商编号
	b.SUPPLIERS_NAME, --生产厂商
	a1.ITEM_NO, --料号(材料编号)
    a1.CHK_SEQ,--材料序号
    a1.SOURCE_SEQ, --来源单序号
	aw.NAME_T, --材料名称
	a.RCPT_DATE as JCDATE, --收货日期
	a1.RCPT_QTY, --收料数量
    A1.source_no as ORDER_NO,--采购单号
	'' AS SHOE_NO_LS,--鞋型
	'' as PROD_NO_LS,--ART
	'' AS BW,--部位
   a.INSERT_DATE,--添加时间
	a.CHK_NO--收料单号
FROM
	wms_rcpt_m a 
LEFT JOIN wms_rcpt_d a1 on a.CHK_NO=a1.CHK_NO
LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT JOIN base003m b ON aw.vend_no_prd = b.SUPPLIERS_CODE
 where a.RCPT_BY='01'  and  a.chk_no='{CHK_NO}' and  a1.ITEM_NO='{ITEM_NO}' and A1.CHK_SEQ='{CHK_SEQ}'  order by a.INSERT_DATE desc";
                DataTable dt1 = DB.GetDataTable(sql);//物料信息,top部分
                DataTable dtrow = new DataTable();
                string name_t = string.Empty;
                string[] arr = null;
                foreach (DataRow item in dt1.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["ORDER_NO"].ToString()))
                    {
                        sql = $@"
SELECT
	aa.ORDER_NO,
	--ee.SALES_ORDER,
    ee.ORDER_SEQ,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT ee.ART_NO", "ee.ART_NO")} as ART_NO,
    {CommonBASE.GetGroupConcatByOracleVersion(DB, "DISTINCT pd.SHOE_NO", "pd.SHOE_NO")} as SHOE_NO
FROM
	bdm_purchase_order_m aa
LEFT  JOIN bdm_purchase_order_d ee on aa.ORDER_NO=ee.ORDER_NO 
LEFT JOIN bdm_rd_prod pd on ee.ART_NO=pd.PROD_NO
where aa.ORDER_NO='{item["ORDER_NO"]}' AND ee.ORDER_SEQ='{item["SOURCE_SEQ"]}' group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dtrow = DB.GetDataTable(sql);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        if (dtrow.Rows.Count > 0)
                        {
                            //item["SHOE_NO"] = dtrow.Rows[0]["SHOE_NO"].ToString();
                            item["PROD_NO_LS"] = dtrow.Rows[0]["ART_NO"].ToString();
                          
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    name_t = string.Empty;
                                    arr = dtrow.Rows[0]["SHOE_NO"].ToString().Split(',');
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {
                                            name_t += dtrow.Rows[0]["NAME_T"].ToString() + "、";
                                        }
                                    }
                                }
                                else
                                {
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO='{dtrow.Rows[0]["SHOE_NO"].ToString()}'";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        name_t = dtrow.Rows[0]["NAME_T"].ToString();

                                    }
                                }
                                
                            }
                            item["SHOE_NO_LS"] = name_t;

                        }
                        sql = $@"
select 
	PART_NO
from bdm_purchase_order_item  
where order_no = '{item["ORDER_NO"]}' and order_seq = '{item["SOURCE_SEQ"]}'";
                        dtrow = DB.GetDataTable(sql);
                        if (dtrow.Rows.Count > 0)
                        {
                            item["BW"] = dtrow.Rows[0]["PART_NO"].ToString();
                        }
                    }
                }

                sql = $@"SELECT
	a.REMARK,
	a.STAFF_NO,
	b.STAFF_NAME,
	a.INSPECTIONDATE as  MODIFYDATE,
	a.CHK_NO,
	a.PASS_QTY,
	a.CHK_SEQ,
	a.ITEM_NO,
	a.BAD_QTY,
	a.DETERMINE,
	a.CLOSING_STATUS,
	a.SAMPLE_QTY,
	nvl(a.testlevel,2) as testlevel,
	nvl(a.aql_level,13)as aql_level,
	a.AC_RE
FROM
	QCM_IQC_INSP_RES_M a LEFT JOIN HR001M b on a.staff_no=b.STAFF_NO WHERE a.isdelete!='1' and  a.CHK_NO='{CHK_NO}'and a.CHK_SEQ='{CHK_SEQ}' and  a.ITEM_NO='{ITEM_NO}'";

                DataTable dt2 = DB.GetDataTable(sql);//抽样数量中间模块
                sql = $@"SELECT
   a.ID,
  a.CHK_NO,
  A.BADPROBLEM_CODE,
	a.TEST_ITEM_NO as INSPECTION_CODE,
	a.TEST_ITEM_NAME as INSPECTION_NAME,
	a.TEST_STANDARD as INSPECTION_STANDARD,
	b.badproblem_name as REMARKS,
	a.DETERMINE
FROM
    QCM_IQC_INSP_RES_D a LEFT JOIN qcm_iqc_badproblems_m b on a.badproblem_code=b.badproblem_code where a.isdelete!='1' and   a.chk_no='{CHK_NO}' and a.CHK_SEQ='{CHK_SEQ}' and a.ITEM_NO='{ITEM_NO}'  ORDER BY A.TEST_ITEM_NO ASC";
                DataTable dt3 = DB.GetDataTable(sql);
                dt3.Columns.Add("IMAGE_LIST", Type.GetType("System.Object"));
                if (dt3.Rows.Count > 0)
                {
                    //拼接图片路径，还有图片guid穿上去 =>一个检验项目可能多个图片
                    foreach (DataRow item in dt3.Rows)
                    {
                        sql = $"SELECT a.id,B.FILE_NAME,B.FILE_URL,B.GUID,B.SUFFIX FROM QCM_IQC_INSP_RES_D_F A INNER  JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_ID=B.GUID WHERE A.D_ID='{item["ID"]}'";
                        DataTable dt_img = DB.GetDataTable(sql);//每个检测项目的图片集合
                        List<imginfo> images = new List<imginfo>();
                        if (dt_img.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt_img.Rows.Count; i++)
                            {
                                images.Add(new imginfo()
                                {
                                    guid = dt_img.Rows[i]["GUID"].ToString(),
                                    image_url = dt_img.Rows[i]["FILE_URL"].ToString()
                                });
                            }
                        }
                        item["IMAGE_LIST"] = images;
                    }
                }
                if (dt3.Rows.Count == 0)
                {
                    //外观检测项目-材料
                    sql = $@"SELECT
	INSPECTION_CODE,
	INSPECTION_NAME,
	QC_TYPE,
	JUDGMENT_CRITERIA,
	'{CHK_NO}' AS CHK_NO,
	'' AS DETERMINE,
    '' as BADPROBLEM_CODE,
	STANDARD_VALUE AS  INSPECTION_STANDARD,
	REMARKS
FROM
	BDM_MATERIAL_INSPECTION_M
WHERE
	QC_TYPE = '3'
ORDER BY INSPECTION_CODE ASC";
                    dt3 = DB.GetDataTable(sql);
                    dt3.Columns.Add("IMAGE_LIST", Type.GetType("System.Object"));
                    List<imginfo> images = new List<imginfo>();
                    foreach (DataRow item in dt3.Rows)
                    {
                        item["IMAGE_LIST"] = images;
                    }
                }//表身信息
                if (dt3.Rows.Count == 0)
                {
                    ret.ErrMsg = "No data yet, please check the appearance inspection items - whether the material is maintained, please check";
                    ret.IsSuccess = false;
                    return ret;
                }
              
                
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("top", dt1);
                dic.Add("center", dt2);
                dic.Add("bottom", dt3);

                ret.IsSuccess = true;
                ret.RetData1 = dic;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        /// <summary>
        /// PDA检查员编号扫描
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultPDAYCLViewUser(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string STAFF_NO = jarr.ContainsKey("STAFF_NO") ? jarr["STAFF_NO"].ToString() : "";
                DataTable  DT = DB.GetDataTable($"select STAFF_NAME,STAFF_NO From HR001M where STAFF_NO='{STAFF_NO}' and rownum=1");
                if (DT.Rows.Count==0)
                {
                    ret.ErrMsg = $"Inspector [{STAFF_NO}] does not exist, please check";
                    ret.IsSuccess = false;
                    return ret;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("USER_SYS", DT);
                ret.IsSuccess = true;
                ret.RetData1 = dic;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultPDAYCLViewUser2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string STAFF_NO = jarr.ContainsKey("STAFF_NO") ? jarr["STAFF_NO"].ToString() : "";
                DataTable DT = DB.GetDataTable($"select STAFF_NAME,STAFF_NO From HR001M where STAFF_NO='{STAFF_NO}' and rownum=1");
                if (DT.Rows.Count == 0)
                {
                    ret.ErrMsg = $"Inspector [{STAFF_NO}] does not exist, please check";
                    ret.IsSuccess = false;
                    return ret;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("USER_SYS", DT);
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
        /// PDA不良问题
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultBadproblems(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string swheres = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    swheres += $"and BADPROBLEM_CODE LIKE @keyword OR BADPROBLEM_NAME LIKE @keyword";
                    dic.Add("keyword", $@"%{keyword}%");
                }
                string total = DB.GetString("SELECT COUNT(1) FROM QCM_IQC_BADPROBLEMS_M");
                string sql = $"SELECT BADPROBLEM_NAME,BADPROBLEM_CODE FROM QCM_IQC_BADPROBLEMS_M WHERE 1=1 {swheres}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow),"",dic);
                dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("total",total);
                ret.IsSuccess = true;
                ret.RetData1 = dic;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultBadproblems2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string keyword = jarr.ContainsKey("keyword") ? jarr["keyword"].ToString() : "";
                string page = jarr.ContainsKey("page") ? jarr["page"].ToString() : "";
                string pageRow = jarr.ContainsKey("pageRow") ? jarr["pageRow"].ToString() : "";
                string swheres = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    swheres += $"AND BADPROBLEM_CODE LIKE @keyword OR BADPROBLEM_NAME LIKE @keyword";
                    dic.Add("keyword", $@"%{keyword}%");

                }
                string sql = $"SELECT BADPROBLEM_NAME,BADPROBLEM_CODE FROM QCM_IQC_BADPROBLEMS_M WHERE 1=1 {swheres}";
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(page), int.Parse(pageRow),"",dic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql,dic);

                dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", rowCount);
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
        /// 录入检验结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CheckResultPDAYCLAdd(object OBJ)
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
              
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = String.Empty;
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                #region center

                Dictionary<string, object> center_jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(jarr["center"].ToString());
                NullKeyValue(center_jarr);
                string userCode2 = center_jarr.ContainsKey("STAFF_NO") ? center_jarr["STAFF_NO"].ToString() : "";//检验员
                string STAFF_NAME = center_jarr.ContainsKey("STAFF_NAME") ? center_jarr["STAFF_NAME"].ToString() : "";//检验员
                string chk_no = center_jarr.ContainsKey("CHK_NO") ? center_jarr["CHK_NO"].ToString():"";//收料单号
                string chk_seq = center_jarr.ContainsKey("CHK_SEQ")?  center_jarr["CHK_SEQ"].ToString() : "";//料号
                string item_no = center_jarr.ContainsKey("ITEM_NO")? center_jarr["ITEM_NO"].ToString() : "";//材料序号
                string bad_qty = center_jarr.ContainsKey("BAD_QTY")? center_jarr["BAD_QTY"].ToString():"";//不合格数量
                string sample_qty = center_jarr.ContainsKey("SAMPLE_QTY")? center_jarr["SAMPLE_QTY"].ToString():"";//抽样数量
                string determine = center_jarr.ContainsKey("DETERMINE") ?  center_jarr["DETERMINE"].ToString():"";//判定
                string aql_level = center_jarr.ContainsKey("AQL_LEVEL") ?  center_jarr["AQL_LEVEL"].ToString():"";//aql级别
                string ac_re = center_jarr.ContainsKey("AC_RE") ? center_jarr["AC_RE"].ToString():"";//判定
                string rcpt_qty = center_jarr.ContainsKey("RCPT_QTY") ? center_jarr["RCPT_QTY"].ToString() : "";//检验数量
                string testlevel = center_jarr.ContainsKey("TESTLEVEL") ? center_jarr["TESTLEVEL"].ToString():"";//检验水平
                string closing_status = center_jarr.ContainsKey("CLOSING_STATUS") ? center_jarr["CLOSING_STATUS"].ToString():"";//结案状态
                string remark = center_jarr.ContainsKey("REMARK") ?  center_jarr["REMARK"].ToString() : "";//备注
                string modifydate = center_jarr.ContainsKey("MODIFYDATE") ?center_jarr["MODIFYDATE"].ToString() : "";//检验时间
                string pass_qty = center_jarr.ContainsKey("PASS_QTY") ? center_jarr["PASS_QTY"].ToString() : "";//合格数量
               
                if (!string.IsNullOrWhiteSpace(userCode))
                {
                    sql = $"select STAFF_NAME from HR001M where STAFF_NO='{userCode}'and rownum=1";
                    string username = DB.GetString(sql);
                    if (string.IsNullOrWhiteSpace(username))
                    {
                        ret.ErrMsg = $@"inspector number【{userCode}】Inspector for does not exist";
                        ret.IsSuccess = false;
                        return ret;
                    }
                }
                else
                {
                    ret.ErrMsg = $@"Inspector option cannot be empty";
                    ret.IsSuccess = false;
                    return ret;
                }
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = $@"select chk_no from qcm_iqc_insp_res_m where chk_no=@chk_no  and item_no=@item_no and chk_seq=@chk_seq";
                dic.Add("chk_no", chk_no);
                dic.Add("item_no", item_no);
                dic.Add("chk_seq", chk_seq);
                if (string.IsNullOrWhiteSpace(DB.GetString(sql, dic)))
                {
                    dic = new Dictionary<string, object>();
                    dic.Add("CHK_NO", chk_no);
                    dic.Add("ITEM_NO", item_no);
                    dic.Add("CHK_SEQ", chk_seq);
                    dic.Add("PASS_QTY", pass_qty);
                    dic.Add("REMARK", remark);
                    dic.Add("BAD_QTY", bad_qty);
                    dic.Add("DETERMINE", determine);
                    dic.Add("CLOSING_STATUS", closing_status);
                    dic.Add("SAMPLE_QTY", sample_qty);
                    dic.Add("AQL_LEVEL", aql_level);
                    dic.Add("AC_RE", ac_re);
                    dic.Add("TESTLEVEL", testlevel);
                    dic.Add("STAFF_NO", userCode2);
                    dic.Add("INSPECTIONDATE", date);
                    dic.Add("CREATEBY", userCode);
                    dic.Add("CREATEDATE", date);
                    dic.Add("CREATETIME", time);
                    sql = $@"INSERT INTO QCM_IQC_INSP_RES_M (CHK_NO,ITEM_NO,CHK_SEQ,PASS_QTY,REMARK,BAD_QTY,DETERMINE,CLOSING_STATUS,SAMPLE_QTY,AQL_LEVEL,AC_RE,TESTLEVEL,STAFF_NO,INSPECTIONDATE,CREATEBY,CREATEDATE,CREATETIME) VALUES(@CHK_NO,@ITEM_NO,@CHK_SEQ,@PASS_QTY,@REMARK,@BAD_QTY,@DETERMINE,@CLOSING_STATUS,@SAMPLE_QTY,@AQL_LEVEL,@AC_RE,@TESTLEVEL,@STAFF_NO,@INSPECTIONDATE,@CREATEBY,@CREATEDATE,@CREATETIME)";
                }
                else
                {
                    dic = new Dictionary<string, object>();
                    dic.Add("BAD_QTY", bad_qty);
                    dic.Add("REMARK", remark);
                    dic.Add("ISDELETE", "0");
                    dic.Add("DETERMINE", determine);
                    dic.Add("SAMPLE_QTY", sample_qty);
                    dic.Add("AQL_LEVEL", aql_level);
                    dic.Add("AC_RE", ac_re);
                    dic.Add("PASS_QTY", pass_qty);
                    dic.Add("TESTLEVEL", testlevel);
                    dic.Add("STAFF_NO", userCode2);
                    dic.Add("INSPECTIONDATE", date);
                    dic.Add("MODIFYBY", userCode);
                    dic.Add("MODIFYDATE", date);
                    dic.Add("MODIFYTIME", time);
                    dic.Add("CHK_NO", chk_no);
                    dic.Add("ITEM_NO", item_no);
                    dic.Add("CHK_SEQ", chk_seq);
                    sql = $@"UPDATE QCM_IQC_INSP_RES_M
                                         SET BAD_QTY =@BAD_QTY,
                                          REMARK=@REMARK,
                                          ISDELETE=@ISDELETE,
                                          DETERMINE =@DETERMINE,
                                          SAMPLE_QTY =@SAMPLE_QTY,
                                          AQL_LEVEL=@AQL_LEVEL,
                                          AC_RE=@AC_RE,
                                          PASS_QTY=@PASS_QTY,
                                          TESTLEVEL=@TESTLEVEL,
                                          STAFF_NO=@STAFF_NO,
                                          INSPECTIONDATE=@INSPECTIONDATE,
                                          MODIFYBY =@MODIFYBY,
                                          MODIFYDATE =@MODIFYDATE,
                                          MODIFYTIME =@MODIFYTIME
                                         WHERE
	                                       CHK_NO =@CHK_NO  AND  ITEM_NO=@ITEM_NO AND CHK_SEQ=@CHK_SEQ";
                }
                DB.ExecuteNonQuery(sql,dic);
                #endregion

                #region bottom
                var bottom_jarr = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jarr["bottom"].ToString());
              
                if (bottom_jarr.Count > 0)
                {
                    string item_id = string.Empty;
                    string m_id = string.Empty;
                    sql = $@"SELECT CHK_NO,TEST_ITEM_NO,CHK_SEQ,ITEM_NO FROM QCM_IQC_INSP_RES_D";
                    DataTable dtdr = DB.GetDataTable(sql);
                    DataRow[] dr = null;
                    foreach (Dictionary<string, object> item in bottom_jarr)
                    {
                        NullKeyValue(item);
                        string id = item.ContainsKey("ID") ? item["ID"].ToString() : "";
                        string determines = item.ContainsKey("DETERMINE") ?  item["DETERMINE"].ToString() : "";
                        string test_item_no = item.ContainsKey("INSPECTION_CODE") ? item["INSPECTION_CODE"].ToString() : "";
                        string test_item_name = item.ContainsKey("INSPECTION_NAME") ? item["INSPECTION_NAME"].ToString() : "";
                        string test_standard = item.ContainsKey("INSPECTION_STANDARD") ? item["INSPECTION_STANDARD"].ToString() : "";
                        string badproblem_code = item.ContainsKey("BADPROBLEM_CODE") ? item["BADPROBLEM_CODE"].ToString() : "";//不良问题编号
                        string remarks = item.ContainsKey("REMARKS") ? item["REMARKS"].ToString() : "";//备注
                        qcm_iqc_insp_res_d currItem = new qcm_iqc_insp_res_d()
                        {
                            id = id,
                            chk_no = chk_no,
                            determine = determines,
                            test_item_no = test_item_no,
                            test_item_name = test_item_name,
                            test_standard = test_standard,
                            badproblem_code = badproblem_code,
                            remark = remarks,
                            image_list = JsonConvert.DeserializeObject<List<imginfo>>(item["imgList"].ToString())
                        };
                        dr = dtdr.Select($@"CHK_NO='{currItem.chk_no}' AND TEST_ITEM_NO='{currItem.test_item_no}' AND CHK_SEQ='{chk_seq}' AND  ITEM_NO='{item_no}'");
                        if (dr.Length<1)
                        {
                            dic = new Dictionary<string, object>();
                            dic.Add("CHK_NO", currItem.chk_no);
                            dic.Add("ITEM_NO", item_no);
                            dic.Add("CHK_SEQ", chk_seq);
                            dic.Add("BADPROBLEM_CODE", currItem.badproblem_code);
                            dic.Add("TEST_ITEM_NO", currItem.test_item_no);
                            dic.Add("TEST_ITEM_NAME", currItem.test_item_name);
                            dic.Add("TEST_STANDARD", currItem.test_standard);
                            dic.Add("DETERMINE", currItem.determine);
                            dic.Add("REMARK", currItem.remark);
                            dic.Add("CREATEBY", userCode);
                            dic.Add("CREATEDATE", date);
                            dic.Add("CREATETIME", time);
                            sql = $@"INSERT INTO QCM_IQC_INSP_RES_D (CHK_NO,ITEM_NO,CHK_SEQ,BADPROBLEM_CODE,TEST_ITEM_NO,TEST_ITEM_NAME,TEST_STANDARD,DETERMINE,REMARK,CREATEBY,CREATEDATE,CREATETIME) VALUES(@CHK_NO,@ITEM_NO,@CHK_SEQ,@BADPROBLEM_CODE,@TEST_ITEM_NO,@TEST_ITEM_NAME,@TEST_STANDARD,@DETERMINE,@REMARK,@CREATEBY,@CREATEDATE,@CREATETIME)";
                            DB.ExecuteNonQuery(sql,dic);
                            item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_d");
                        }
                        else
                        {
                            dic = new Dictionary<string, object>();
                            dic.Add("TEST_ITEM_NO", currItem.test_item_no);
                            dic.Add("TEST_ITEM_NAME", currItem.test_item_name);
                            dic.Add("TEST_STANDARD", currItem.test_standard);
                            dic.Add("DETERMINE", currItem.determine);
                            dic.Add("REMARK", currItem.remark);
                            dic.Add("MODIFYBY", userCode);
                            dic.Add("BADPROBLEM_CODE", badproblem_code);
                            dic.Add("MODIFYDATE", date);
                            dic.Add("MODIFYTIME", time);
                            dic.Add("CHK_NO", currItem.chk_no);
                            dic.Add("ITEM_NO", item_no);
                            dic.Add("CHK_SEQ", chk_seq);
                            sql = $@"UPDATE QCM_IQC_INSP_RES_D
                                           SET
                                          TEST_ITEM_NO =@TEST_ITEM_NO,
                                          TEST_ITEM_NAME =@TEST_ITEM_NAME,
                                          TEST_STANDARD =@TEST_STANDARD,
                                          DETERMINE =@DETERMINE,
                                          REMARK =@REMARK,
                                          MODIFYBY =@MODIFYBY,
                                          BADPROBLEM_CODE=@BADPROBLEM_CODE,
                                          MODIFYDATE =@MODIFYDATE,
                                          MODIFYTIME =@MODIFYTIME
                                         WHERE
	                                       CHK_NO =@CHK_NO AND  ITEM_NO=@ITEM_NO AND CHK_SEQ=@CHK_SEQ  AND TEST_ITEM_NO=@TEST_ITEM_NO";
                            DB.ExecuteNonQuery(sql,dic);
                          

                            sql = $"SELECT ID FROM QCM_IQC_INSP_RES_D WHERE  CHK_NO =@CHK_NO AND TEST_ITEM_NO=@TEST_ITEM_NO AND CHK_SEQ=@CHK_SEQ AND ITEM_NO=@ITEM_NO";
                            dic = new Dictionary<string, object>();
                            dic.Add("CHK_NO", currItem.chk_no);
                            dic.Add("TEST_ITEM_NO", currItem.test_item_no);
                            dic.Add("CHK_SEQ", chk_seq);
                            dic.Add("ITEM_NO", item_no);
                            string ids=DB.GetString(sql,dic);
                            item_id = ids;
                        }
                        //对应检查项图片集合
                        //先删除全部关联图片
                        sql = $"delete from qcm_iqc_insp_res_d_f where d_id='{item_id}'";
                        DB.ExecuteNonQuery(sql);
                        if (currItem.image_list.Count()>0)
                        {
                            dic = new Dictionary<string, object>();
                            sql = string.Empty;
                            for (int i = 0; i < currItem.image_list.Count; i++)
                            {
                                dic.Add($@"d_id{i}", item_id);
                                dic.Add($@"file_id{i}", currItem.image_list[i].guid);
                                sql += $@"insert into qcm_iqc_insp_res_d_f(d_id,file_id) values(@d_id{i},@file_id{i});";//添加对应的guid关联图片
                               
                            }
                            if (!string.IsNullOrWhiteSpace(sql))
                            {
                                DB.ExecuteNonQuery($@"begin {sql}  end;",dic);
                            }
                        }

                    }
                }
                #endregion
                //生成不良报告初始数据
                if (determine == "1")//determine=1就是fail
                {



                    sql = $@"SELECT ID FROM QCM_IQC_INSP_RES_BAD_REPORT WHERE CHK_NO='{chk_no}' AND ITEM_NO='{item_no}' AND CHK_SEQ='{chk_seq}' and isdelete='0'";
                    string ID = DB.GetString(sql);


                     bad_qty = (Convert.ToDecimal(rcpt_qty) - Convert.ToDecimal(pass_qty)).ToString();//不良数量=进仓-合格
                    string bad_rate = ((Convert.ToDecimal(bad_qty) / Convert.ToDecimal(rcpt_qty)) * 100).ToString("F");
                    if (string.IsNullOrWhiteSpace(ID))
                    {
                        sql = $@"insert into qcm_iqc_insp_res_bad_report(chk_no,item_no,chk_seq,closing_status,warehousing_qty,bad_qty,bad_rate,spc_mining,actual_returned_qty,supplementary_delivery_qty
,createby,createdate,createtime) VALUES('{chk_no}','{item_no}','{chk_seq}','1','{rcpt_qty}','{bad_qty}','{bad_rate}','0','0','0','{userCode}','{date}','{time}')";

                    }
                    else
                    {
                        sql = $@"update qcm_iqc_insp_res_bad_report set bad_qty='{bad_qty}', closing_status='1',bad_rate='{bad_rate}',actual_returned_qty={bad_qty}-spc_mining,modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where chk_no='{chk_no}' and  item_no='{item_no}' and  chk_seq='{chk_seq}'";
                    }
                    DB.ExecuteNonQuery(sql);
                }

                DB.Commit();
                ret.ErrMsg = "Saved successfully！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "Save failed! ,reason：" + ex.Message;
            }
            finally
            {
                DB.Close();
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

        public class qcm_iqc_insp_res_d
        {
            public string id { get; set; }
            public string chk_no { get; set; }
            public string test_item_no { get; set; }
            public string test_item_name { get; set; }
            public string test_standard { get; set; }
            public string determine { get; set; }
            public string badproblem_code { get; set; }
            public List<imginfo> image_list { get; set; }
            public string remark { get; set; }
        }

        public class qcm_iqc_insp_res_d_TOUPPER
        {
            public string ID { get; set; }
            public string CHK_NO { get; set; }
            public string TEST_ITEM_NO { get; set; }
            public string TEST_ITEM_NAME { get; set; }
            public string TEST_STANDARD { get; set; }
            public string DETERMINE { get; set; }
            public string BADPROBLEM_CODE { get; set; }
            public string REMARK { get; set; }
        }
        public class guid_list
        {
            public string guid { get; set; }
            public int d_id { get; set; }
            public int file_url { get; set; }
        }

        public class imginfo
        {
            public string guid { get; set; }
            public string image_url { get; set; }
            public string chk_no { get; set; }
            public string test_item_no { get; set; }
        }

        /// <summary>
        /// 获取AQL相关枚举
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLEnum(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var sql = string.Empty;
                sql = $@"
SELECT
	ENUM_CODE,ENUM_VALUE
FROM
	SYS001M
WHERE
	ENUM_TYPE = 'AQL_ENUM_RAW'
";
                DataTable dt_level = DB.GetDataTable(sql);

                sql = $@"
SELECT
	ENUM_CODE,ENUM_VALUE
FROM
	SYS001M
WHERE
	ENUM_TYPE = 'enum_aql_level'
";
                DataTable dt_rank = DB.GetDataTable(sql);
                int rankIndex = 1;
                foreach (DataRow item in dt_rank.Rows)
                {
                    item["ENUM_CODE"] = rankIndex;
                    rankIndex++;
                }
               
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("AQL_Level", dt_level);
                dic.Add("AQL_Rank", dt_rank);
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
        /// 根据校验水平、AQL等级、数量 获取AcRe
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLAcRe(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string rank_code = jarr.ContainsKey("rank_code") ? jarr["rank_code"].ToString() : "";//aql等级
                string level_code = jarr.ContainsKey("level_code") ? jarr["level_code"].ToString() : "";//检验水平
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//进仓数量
                if (string.IsNullOrEmpty(rank_code) || string.IsNullOrEmpty(level_code) || string.IsNullOrEmpty(qty))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Parameter error";
                    return ret;
                }
                string index = Convert.ToInt32(rank_code).ToString("00");
                var sql = string.Empty;
                sql = $@"
SELECT  
	AC{index} AC,
	AC{index} RE,
	VAL01 sample_qty
FROM BDM_AQL_M 
WHERE HORI_TYPE='1' AND LEVEL_TYPE='{level_code}' AND (START_QTY<={qty} AND END_QTY>={qty}) AND ROWNUM=1";
                DataTable dt_acre = DB.GetDataTable(sql);
                if (dt_acre.Rows.Count > 0)
                {
                    DataRow firstRow = dt_acre.Rows[0];
                    int ac = 0;
                    int.TryParse(firstRow["AC"].ToString(), out ac);
                    int sample_qty = 0;
                    int.TryParse(firstRow["sample_qty"].ToString(), out sample_qty);
                    ret.IsSuccess = true;
                    ret.RetData1 = new
                    {
                        AC = ac,
                        RE = ac + 1,
                        AC_RE= ac + ","+ (ac + 1),
                        sample_qty
                    };
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "AQL level not maintained";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAQLAcRe2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string rank_code = jarr.ContainsKey("rank_code") ? jarr["rank_code"].ToString() : "";//aql等级
                string level_code = jarr.ContainsKey("level_code") ? jarr["level_code"].ToString() : "";//检验水平
                string qty = jarr.ContainsKey("qty") ? jarr["qty"].ToString() : "";//进仓数量
                if (string.IsNullOrEmpty(rank_code) || string.IsNullOrEmpty(level_code) || string.IsNullOrEmpty(qty))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "Parameter error";
                    return ret;
                }
                string index = Convert.ToInt32(rank_code).ToString("00");
                var sql = string.Empty;
                sql = $@"
SELECT  
	AC{index} AC,
	AC{index} RE,
	VAL{index} sample_qty
FROM BDM_AQL_M 
WHERE HORI_TYPE='1' AND LEVEL_TYPE='{level_code}' AND (START_QTY<={qty} AND END_QTY>={qty}) AND ROWNUM=1";
                DataTable dt_acre = DB.GetDataTable(sql);
                if (dt_acre.Rows.Count > 0)
                {
                    DataRow firstRow = dt_acre.Rows[0];
                    int ac = 0;
                    int.TryParse(firstRow["AC"].ToString(), out ac);
                    int sample_qty = 0;
                    int.TryParse(firstRow["sample_qty"].ToString(), out sample_qty);
                   
                    var lis = new
                    {
                        AC = ac,
                        RE = ac + 1,
                        AC_RE = ac + "," + (ac + 1),
                        sample_qty
                    };
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("AC_RE", ac + "," + (ac + 1));
                    dic.Add("sample_qty", sample_qty);
                    ret.IsSuccess = true;
                    ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "AQL level not maintained";
                }

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 计算测试取样状况
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CalSamplingStatus(object OBJ)
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

                string sql = $@"
                                SELECT
	                                B.ITEM_TYPE_NO,
	                                A.INSPECTION_FREQUENCY_TIME_UNIT TIME_UNIT,--1 day, 2 months, 3 years
	                                A.INSPECTION_FREQUENCY_VALUE VALUE
                                FROM
	                                BDM_INSPECTION_FREQUENCY_D B
                                INNER JOIN BDM_INSPECTION_FREQUENCY_M A ON A .ID = B.M_ID
                                ";
                // 获取物料分类的送测频率
                DataTable itemType_dt = DB.GetDataTable(sql);

                sql = $@"
                        SELECT
	                        M.RCPT_DATE,
	                        D.CHK_NO,
	                        D.CHK_SEQ,
                            D.ITEM_NO,
	                        D.INSERT_DATE,
	                        aw.ITEM_TYPE
                        FROM
	                        WMS_RCPT_D D 
                        INNER JOIN WMS_RCPT_M M ON M.CHK_NO=D.CHK_NO 
                        LEFT JOIN bdm_rd_item aw ON D.ITEM_NO=aw.ITEM_NO
                        WHERE 
	                        D.SAMPLING_STATUS IS NULL 
                        ORDER BY D.INSERT_DATE 
                        ";
                DataTable rcpt_dt = DB.GetDataTable(sql);
                StringBuilder updateSS_SB = new StringBuilder();
                
                foreach (DataRow item in rcpt_dt.Rows)
                {
                    string ITEM_TYPE = item["ITEM_TYPE"].ToString();// 物料分类
                    string CHK_NO = item["CHK_NO"].ToString();
                    string CHK_SEQ = item["CHK_SEQ"].ToString();
                    //var existItemType = itemType_dt.Select($@"物料分类='{ITEM_TYPE}'");
                    var existItemType = itemType_dt.Select($@"ITEM_TYPE_NO='{ITEM_TYPE}'");
                    if (existItemType.Count() > 0)
                    {
                        string INSERT_DATE = Convert.ToDateTime(item["INSERT_DATE"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        string RCPT_DATE = Convert.ToDateTime(item["RCPT_DATE"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        string ITEM_NO = item["ITEM_NO"].ToString();// 物料编码

                        DataRow findType = existItemType[0];
                        string date_unit = findType["时间单位"].ToString();
                        string date_value = findType["值"].ToString();
                        string where_INSERT_DATE = $@"TO_DATE('{INSERT_DATE}','yyyy-mm-dd,hh24:mi:ss')";
                        string where_RCPT_DATE_start = $@"TO_DATE('{RCPT_DATE}','yyyy-mm-dd,hh24:mi:ss')";
                        string where_RCPT_DATE_end = $@"TO_DATE('{RCPT_DATE}','yyyy-mm-dd,hh24:mi:ss')";
                        switch (date_unit)
                        {
                            case "1":
                                where_RCPT_DATE_start = $@"(TO_DATE('{RCPT_DATE}','yyyy-mm-dd,hh24:mi:ss')-{date_value})";
                                break;
                            case "2":
                                where_RCPT_DATE_start = $@"add_months(TO_DATE('{RCPT_DATE}','yyyy-mm-dd,hh24:mi:ss'),-{date_value})";
                                break;
                            case "3":
                                where_RCPT_DATE_start = $@"add_months(TO_DATE('{RCPT_DATE}','yyyy-mm-dd,hh24:mi:ss'),-{date_value}*12)";
                                break;
                            default:
                                break;
                        }
                        string findLogSql = $@"
                                            SELECT
	                                            count(1)
                                            FROM
	                                            WMS_RCPT_S_LOG
                                            WHERE
	                                            RCPT_DATE >= {where_RCPT_DATE_start}
                                            AND RCPT_DATE < {where_RCPT_DATE_end}
                                            AND INSERT_DATE < {where_INSERT_DATE} 
                                            AND ITEM_NO='{ITEM_NO}'
                                            ";
                        int logFindCount = Convert.ToInt32(DB.GetStringline(findLogSql));
                        if(logFindCount>0)
                            updateSS_SB.Append($@"UPDATE WMS_RCPT_D SET SAMPLING_STATUS = 'N'  WHERE CHK_NO='{CHK_NO}' AND CHK_SEQ='{CHK_SEQ}';");
                        else
                        {
                            string findRcptSql = $@"
                                            SELECT
	                                            count(1)
                                            FROM
	                                            WMS_RCPT_D D 
                                            INNER JOIN WMS_RCPT_M M ON M.CHK_NO=D.CHK_NO 
                                            WHERE
	                                            M.RCPT_DATE >= {where_RCPT_DATE_start}
                                            AND M.RCPT_DATE < {where_RCPT_DATE_end}
                                            AND D.INSERT_DATE < {where_INSERT_DATE} 
                                            AND D.ITEM_NO='{ITEM_NO}'
                                            AND D.SAMPLING_STATUS is null";
                            int findRcptCount = Convert.ToInt32(DB.GetStringline(findRcptSql));
                            if(findRcptCount>0)
                                updateSS_SB.Append($@"UPDATE WMS_RCPT_D SET SAMPLING_STATUS = 'N'  WHERE CHK_NO='{CHK_NO}' AND CHK_SEQ='{CHK_SEQ}';");
                            else
                            {
                                updateSS_SB.Append($@"UPDATE WMS_RCPT_D SET SAMPLING_STATUS = 'Y'  WHERE CHK_NO='{CHK_NO}' AND CHK_SEQ='{CHK_SEQ}';");
                                string insertLogSql = $@"INSERT INTO WMS_RCPT_S_LOG (CHK_NO, CHK_SEQ, INSERT_DATE, RCPT_DATE, SAMPLING_STATUS, ITEM_NO) VALUES ('{CHK_NO}', '{CHK_SEQ}', {where_INSERT_DATE}, {where_RCPT_DATE_start}, 'Y', '{ITEM_NO}')";
                                DB.ExecuteNonQuery(insertLogSql);
                            }
                        }
                    }
                    else
                    {
                        updateSS_SB.Append($@"UPDATE WMS_RCPT_D SET SAMPLING_STATUS = 'N'  WHERE CHK_NO='{CHK_NO}' AND CHK_SEQ='{CHK_SEQ}';");
                    }
                }
                if (updateSS_SB.Length > 0)
                    DB.ExecuteNonQuery($@"BEGIN {updateSS_SB} END;");
                DB.Commit();

                ret.IsSuccess = true;
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


        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetArticleDetailsData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string data = ReqObj.Data.ToString();

                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

                string art_no = jarr.ContainsKey("art_name")
                                ? jarr["art_name"].ToString().Trim()
                                : "";

                List<string> poList = new List<string>();

                if (jarr.ContainsKey("orders") && jarr["orders"] != null)
                {
                    poList = JsonConvert.DeserializeObject<List<string>>(jarr["orders"].ToString())
                             .Where(x => !string.IsNullOrWhiteSpace(x))
                             .Select(x => x.Trim())
                             .Distinct()
                             .ToList();
                }

                string sql_txt = @"
select  t.art_no,
        w.shoe_no,
        t.order_no,
        t.item_no,
        p.name_t,
        t.ord_qty,
        y.vend_no,
        n.suppliers_name,
        j.part_no
from bdm_purchase_order_d t
left join bdm_rd_item p
    on p.item_no = t.item_no
left join bdm_se_order_item w
    on w.se_id = t.sales_order
   and w.prod_no = t.art_no
left join bdm_purchase_order_m y
    on y.order_no = t.order_no
left join base003m n
    on n.suppliers_code = y.vend_no
left join bdm_purchase_order_item j
    on j.order_no = t.order_no
   and j.item_no = t.item_no
where t.art_no = '" + art_no.Replace("'", "''") + "'";

               
                if (poList.Count > 0)
                {
                    string inClause = string.Join(",",
                        poList.Select(x => $"'{x.Replace("'", "''")}'"));

                    sql_txt += $" and t.order_no in ({inClause})";
                }

              
                DataTable result = DB.GetDataTable(sql_txt);

                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(result);
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }

            return ret;
        }



    }
}
