using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace SJeMES_IQC
{
    public class IQC_Bad_Report
    {
        /// <summary>
        /// IQC不良报告主页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_Main(object OBJ)
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
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//查询条件 收料单号
                string VENDOR_TYPE_NAME = jarr.ContainsKey("VENDOR_TYPE_NAME") ? jarr["VENDOR_TYPE_NAME"].ToString() : "";//查询条件 采购厂商
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//查询条件 料品编码
                string 外观结果 = jarr.ContainsKey("外观结果") ? jarr["外观结果"].ToString() : "";//查询条件 外观结果
                string 测试结果 = jarr.ContainsKey("测试结果") ? jarr["测试结果"].ToString() : "";//查询条件 测试结果
                string 生产厂商 = jarr.ContainsKey("生产厂商") ? jarr["生产厂商"].ToString() : "";//查询条件 生产厂商
                string rcpt_dateS = jarr.ContainsKey("rcpt_dateS") ? jarr["rcpt_dateS"].ToString() : "";//查询条件 收货日期开始
                string rcpt_dateE = jarr.ContainsKey("rcpt_dateE") ? jarr["rcpt_dateE"].ToString() : "";//查询条件 收货日期结束
                string stoc_no = jarr.ContainsKey("stoc_no") ? jarr["stoc_no"].ToString() : "";//查询条件 仓库
                string 取样状况 = jarr.ContainsKey("取样状况") ? jarr["取样状况"].ToString() : "";//查询条件 取样状况
                string closing_status = jarr.ContainsKey("closing_status") ? jarr["closing_status"].ToString() : "";//查询条件 状态
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                string where = string.Empty;
                if (!string.IsNullOrEmpty(chk_no))
                {
                    where += $@" and m.chk_no like '%{chk_no}%'";
                }
                if (!string.IsNullOrEmpty(VENDOR_TYPE_NAME))
                {
                    where += $@" and bm.VENDOR_TYPE_NAME like '%{VENDOR_TYPE_NAME}%'";
                }
                if (!string.IsNullOrEmpty(item_no))
                {
                    where += $@" and d.item_no like '%{item_no}%'";
                }
                if (!string.IsNullOrEmpty(外观结果))
                {
                    where += $@" and 外观结果 like '%{外观结果}%'";
                }
                if (!string.IsNullOrEmpty(测试结果))
                {
                    where += $@" and 测试结果 like '%{测试结果}%'";
                }
                if (!string.IsNullOrEmpty(生产厂商))
                {
                    where += $@" and 生产厂商 like '%{生产厂商}%'";
                }
                if (!string.IsNullOrEmpty(rcpt_dateS)&& !string.IsNullOrEmpty(rcpt_dateE))
                {
                    where += $@" and m.rcpt_date <= '{rcpt_dateS}'  and m.rcpt_date >= '{rcpt_dateE}'";
                }
                else
                {
                    if (!string.IsNullOrEmpty(rcpt_dateS))
                    {
                        where += $@" and m.rcpt_date <= '{rcpt_dateS}'";
                    }
                    if (!string.IsNullOrEmpty(rcpt_dateE))
                    {
                        where += $@" and m.rcpt_date >= '{rcpt_dateE}'";
                    }
                }
                if (!string.IsNullOrEmpty(stoc_no))
                {
                    where += $@" and d.stoc_no like '%{stoc_no}%'";
                }
                if (!string.IsNullOrEmpty(取样状况))
                {
                    where += $@" and 取样状况 like '%{取样状况}%'";
                }
                if (!string.IsNullOrEmpty(closing_status))
                {
                    where += $@" and closing_status like '%{closing_status}%'";
                }

                string sql = string.Empty;
                sql = $@"SELECT
	                    m.rcpt_date,
	                    CASE 
	                    WHEN t.ITEM_TYPE_NO like '401%' THEN
	                    '皮料不良报告' 
	                    ELSE
	                    '原材料检验报告'
	                    END as 报告类型,
	                    t.ITEM_TYPE_NO,
	                    t.ITEM_TYPE_NAME,
	                    '生产厂商编号' as 生产厂商编号,
	                    '生产厂商' as 生产厂商,
	                    m.chk_no,
	                    d.item_no,
	                    m.vend_no,
	                    bm.VENDOR_TYPE_NAME,
	                    '检验日期' as 检验日期,
	                    '收货单位' as 收货单位,
	                    '采购量' as 采购量,
	                    '采购单号' as 采购单号,
	                    '外观结果' as 外观结果,
	                    '取样状况' as 取样状况,
	                    '测试结果' as 测试结果,
	                    '检验员' as 检验员,
	                    '检验数' as 检验数,
	                    d.pass_qty,
	                    d.normal_qty,
	                    '补送' as 补送,
	                    d.stoc_no,
	                    		CASE 
	                WHEN NVL(i.closing_status, '1') =1 THEN
	                '未结案' 
	                ELSE
	                '结案'
	                END as closing_status
                    FROM
	                    WMS_RCPT_D d
	                    JOIN WMS_RCPT_M m on d.CHK_NO=m.CHK_NO
	                    LEFT JOIN base003m bm on m.vend_no=bm.VENDOR_TYPE_NO
	                    LEFT JOIN bdm_rd_item r on d.ITEM_NO=r.ITEM_NO
	                    LEFT JOIN bdm_rd_itemtype t on r.ITEM_TYPE=t.ITEM_TYPE_NO
	                    LEFT JOIN qcm_iqc_insp_res_bad_report i on d.CHK_NO=i.CHK_NO
                    where 1=1 {where}";

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

        /// <summary>
        /// IQC不良报告主页面查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_Main2(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                NullKeyValue(jarr);
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//查询条件 收料单号
                string VENDOR_TYPE_NAME = jarr.ContainsKey("SUPPLIERS_NAME2") ? jarr["SUPPLIERS_NAME2"].ToString() : "";//查询条件 采购厂商
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//查询条件 料品编码
                string DETERMINE = jarr.ContainsKey("DETERMINE") ? jarr["DETERMINE"].ToString() : "";//查询条件 外观结果
                string CSJG = jarr.ContainsKey("CSJG") ? jarr["CSJG"].ToString() : "";//查询条件 测试结果
                string SUPPLIERS_NAME = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//查询条件 生产厂商
                string rcpt_dateS = jarr.ContainsKey("rcpt_dateS") ? jarr["rcpt_dateS"].ToString() : "";//查询条件 收货日期开始
                string rcpt_dateE = jarr.ContainsKey("rcpt_dateE") ? jarr["rcpt_dateE"].ToString() : "";//查询条件 收货日期结束
                string ORG_ID = jarr.ContainsKey("ORG_ID") ? jarr["ORG_ID"].ToString() : "";//查询条件 工厂
                string STOC_NO = jarr.ContainsKey("STOC_NO") ? jarr["STOC_NO"].ToString() : "";//查询条件 仓库
                string CSQYZK = jarr.ContainsKey("CSQYZK") ? jarr["CSQYZK"].ToString() : "";//查询条件 取样状况
                string closing_status = jarr.ContainsKey("closing_status") ? jarr["closing_status"].ToString() : "";//查询条件 状态
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                Dictionary<string, object> dic = new Dictionary<string, object>();
                string sql = string.Empty;
                string strwhere = string.Empty;
                if (closing_status == "Closed")//已结案
                {
                    closing_status = "0";
                }
                 else if (closing_status == "opencase")//未结案
                {
                    closing_status = "1";
                }
                if (DETERMINE == "qualified")//合格
                {
                    DETERMINE = "0";
                }
                else if (DETERMINE == "unqualified")//不合格
                {
                    DETERMINE = "1";
                }
                if (!string.IsNullOrWhiteSpace(CHK_NO))
                {
                    strwhere += $@" and CHK_NO like @CHK_NO";
                    dic.Add("CHK_NO", $@"%{CHK_NO}%");
                }
                if (!string.IsNullOrWhiteSpace(VENDOR_TYPE_NAME))//采购厂商
                {
                    strwhere += $@" and SUPPLIERS_NAME2 like @VENDOR_TYPE_NAME ";
                    dic.Add("VENDOR_TYPE_NAME", $@"%{VENDOR_TYPE_NAME}%");
                }
                if (!string.IsNullOrWhiteSpace(ITEM_NO))//料品编号
                {
                    strwhere += $@" and ITEM_NO like @ITEM_NO";
                    dic.Add("ITEM_NO", $@"%{ITEM_NO}%");
                }
                if (!string.IsNullOrWhiteSpace(DETERMINE))
                {
                    strwhere += $@" and DETERMINE like @DETERMINE";
                    dic.Add("DETERMINE", $@"%{DETERMINE}%");
                }
                if (!string.IsNullOrWhiteSpace(CSJG))
                {
                    strwhere += $@" and CSJG like @CSJG";
                    dic.Add("CSJG", $@"%{CSJG}%");
                }
                if (!string.IsNullOrWhiteSpace(SUPPLIERS_NAME))//生产厂商
                {
                    strwhere += $@" and SUPPLIERS_NAME like @SUPPLIERS_NAME";
                    dic.Add("SUPPLIERS_NAME", $@"%{SUPPLIERS_NAME}%");
                }
                if (!string.IsNullOrWhiteSpace(STOC_NO))
                {
                    strwhere += $@" and STOC_NO = @STOC_NO";
                    dic.Add("STOC_NO", $@"{STOC_NO}");
                }
                if (!string.IsNullOrWhiteSpace(ORG_ID))
                {
                    strwhere += $@" and ORG_ID = @ORG_ID";
                    dic.Add("ORG_ID", $@"{ORG_ID}");
                }
                if (!string.IsNullOrWhiteSpace(closing_status))
                {
                    strwhere += $@" and closing_status like @closing_status";
                    dic.Add("closing_status", $@"%{closing_status}%");
                }
                if (!string.IsNullOrWhiteSpace(rcpt_dateS) && !string.IsNullOrWhiteSpace(rcpt_dateE))
                {
                    strwhere += $@" and RCPT_DATE  BETWEEN TO_date(@rcpt_dateS,'yyyy-MM-dd') and TO_date(@rcpt_dateE,'yyyy-MM-dd')";
                    dic.Add("rcpt_dateS", $@"{rcpt_dateS}");
                    dic.Add("rcpt_dateE", $@"{rcpt_dateE}");
                }
                if (!string.IsNullOrWhiteSpace(CSQYZK))
                {
                    strwhere += $@" and SAMPLING_STATUS=@CSQYZK";
                    dic.Add("CSQYZK", $@"{CSQYZK}");
                }
                sql = $@"SELECT
    DISTINCT
    b.INSERT_DATE
FROM
    wms_rcpt_m A
LEFT JOIN wms_rcpt_d b ON A .chk_no = b.chk_no
WHERE   a.RCPT_BY='01' and   A.chk_no = '{CHK_NO}'  and rownum<3";
                DataTable dt_desc = DB.GetDataTable(sql);
                string str_desc = string.Empty;
                if (dt_desc.Rows.Count < 3)
                {
                    str_desc = ",item_no desc";//出现多个相同的添加时间，做多一个排序的条件
                }
                string searchKeys = $@"DISTINCT
    JA.CHK_NO AS JACHK_NO,--结案数据
	A.RCPT_DATE, --收货日期
    A.INSERT_DATE as datas, 
	'' as REPORT_TYPE, --报告类型
	aw.vend_no_prd as  SUPPLIERS_CODE, --生产厂商编号
	B.SUPPLIERS_NAME, --生产厂商
	A.CHK_NO,--收料单号
	A1.ITEM_NO, --料号(材料编号)
    A1.CHK_SEQ, --料号序号(材料编号)
    pe.ITEM_TYPE_NO, --物料类型
	B1.SUPPLIERS_CODE AS SUPPLIERS_CODE2, --采购厂商编号
	B1.SUPPLIERS_NAME AS SUPPLIERS_NAME2, --采购厂商
	S.INSPECTIONDATE as CREATEDATE,--外观检验日期
	C1.PR_UNIT, --收货单位
	C1.ORD_QTY, --采购数量
	C1.ORDER_NO, --采购单号
   S.DETERMINE,--外观结果
  S.SAMPLE_QTY,--抽检数量,
   (
    CASE WHEN A1.SAMPLING_STATUS='I' THEN '历史数据'
    ELSE
      a1.SAMPLING_STATUS 
    END
  ) SAMPLING_STATUS,-- 测试取样状况
	EX.test_result AS CSJG,--测试结果
	S.STAFF_NO, --检验员编号
    a1.RCPT_QTY, --收料数量
    hm.STAFF_NAME,--检验员名称
   	a1.RCPT_QTY as IV_QTY, --检验数
    s.PASS_QTY, --合格数
    JA.actual_returned_qty AS RETURN_QTY,--验退数
   JA.SUPPLEMENTARY_DELIVERY_QTY AS BS,--补送
	A.STOC_NO, --仓库代号
	A.ORG_ID, --工厂代号
    MM.WAREHOUSE_NAME,--仓库名称
	JA.CLOSING_STATUS --结案状态";
                sql = $@"
select*from (
SELECT
	{searchKeys}
FROM
	(select*from WMS_RCPT_M where RCPT_BY='01') A 
LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
LEFT JOIN MMS_WAREHOUSE_MANAGE MM ON a.STOC_NO=MM.WAREHOUSE_CODE AND a.ORG_ID=MM.ORG_ID
LEFT JOIN BDM_RD_ITEM AW ON A1.ITEM_NO=AW.ITEM_NO
LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN BDM_RD_PROD PD ON AW.PARENT_ITEM_NO=PD.PROD_NO
LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO

LEFT JOIN BASE003M B ON aw.vend_no_prd = B.SUPPLIERS_CODE

LEFT JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE
LEFT JOIN (select ORDER_NO,PR_UNIT,item_no,sum(ORD_QTY)as ORD_QTY from  bdm_purchase_order_item GROUP BY PR_UNIT,ORDER_NO,item_no) C1 ON A2.ORDER_NO=C1.ORDER_NO AND a1.ITEM_NO=c1.item_no
LEFT JOIN BASE001M M1 ON A1.ORG_ID=M1.ORG_CODE
LEFT JOIN QCM_IQC_INSP_RES_M S ON A.CHK_NO=S.CHK_NO and a1.ITEM_NO=s.ITEM_NO and a1.CHK_SEQ=s.CHK_SEQ
LEFT JOIN BDM_VENDOR_REPORT_M RE ON A.VEND_NO=RE.VEND_NO
LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO and a1.item_no=ja.item_no and a1.chk_seq=ja.chk_seq  and ja.isdelete='0'
LEFT JOIN qcm_ex_task_list_m EX ON a1.ITEM_NO=EX.material_code
LEFT JOIN HR001M hm on s.staff_no=hm.staff_no
)
 WHERE  1=1 {strwhere}    and (DETERMINE='0' or DETERMINE='1') and  JACHK_NO  is  not null    ORDER BY datas DESC{str_desc}";

                
                DataTable dt = CommonBASE.GetPageDataTable(DB, sql.Replace("searchKeys", searchKeys), int.Parse(pageIndex), int.Parse(pageSize),"",dic);
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql.Replace("searchKeys", "1"),dic);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        if (!string.IsNullOrWhiteSpace(item["ITEM_TYPE_NO"].ToString()))
                        {
                            string ITEM_TYPE_NO = item["ITEM_TYPE_NO"].ToString().Substring(0, 3);

                            if (ITEM_TYPE_NO.Contains("401"))
                            {
                               // item["REPORT_TYPE"] = "Leather report";//皮料报告
                                item["REPORT_TYPE"] = "Bad leather report";//皮料报告
                            }
                            else
                            {
                                item["REPORT_TYPE"] = "Non-Leather Report";//非皮料报告
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
        /// 不良报告处理信息展示,检验结果，皮料的
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_view(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//查询条件 采购厂商
                string ORDER_NO = jarr.ContainsKey("ORDER_NO") ? jarr["ORDER_NO"].ToString() : "";//查询条件 料品编码
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                sql = $@"SELECT
	DISTINCT
	B.SUPPLIERS_CODE, --生产厂商编号
	B.SUPPLIERS_NAME, --生产厂商
	A.CHK_NO,--收料单号
	   aw.NAME_T, --材料名称
	A1.ITEM_NO, --料号(材料编号)
   pe.ITEM_TYPE_NO,--物料类型（皮料，非皮料）
   A1.RCPT_QTY,--收料数量
	C1.ORDER_NO, --采购单号
  A1.IV_QTY, --检验数
	A1.PASS_QTY, --合格数
    pd.SHOE_NO,--鞋型
	JA.CLOSING_STATUS, --结案状态
	qc.test_item_no, --检验项编号
	qc.test_item_name, --检验项名称
	qc.determine,--检验结果
	qc.remark --检验结果备注
FROM
	WMS_RCPT_M A 
LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE
LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A2.ORDER_NO=C1.ORDER_NO
LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO
LEFT JOIN   qcm_iqc_insp_res_d qc on a.chk_no=qc.chk_no

 where  a.RCPT_BY='01' and a.CHK_NO=@CHK_NO and a1.ITEM_NO=@ITEM_NO  and   (pe.ITEM_TYPE_NO like '401%' or pe.ITEM_TYPE_NO like '402%')";//皮料数据
                dic.Add("CHK_NO", CHK_NO);
                dic.Add("ITEM_NO", ITEM_NO);
                DataTable dt = DB.GetDataTable(sql,dic);
                sql = $@"
select
t.task_no,
t.item_no,
t.item_name,
t.vend_no,
t.vend_name,
t.wh_date_start,
t.wh_date_end,
nvl(t.mtl_qty,0)as mtl_qty,
t.task_state,
y.enum_code,
y.enum_value

from qcm_hp_task_m t RIGHT   JOIN(
SELECT
	MAX(s.enum_code) as enum_code,
  MAX(s.enum_value) as enum_value,
  SUM(NVL( d.qty, 0)) as qty,
  MAX(s.enum_value2) as coefficient,
	d.task_no,
  (SUM(NVL( d.qty, 0))*MAX(s.enum_value2)) as multiple
	
FROM
  SYS001M s 
  LEFT JOIN qcm_hp_task_d d on s.ENUM_CODE=d.pl_level and d.isdelete='0'
  WHERE ENUM_TYPE = 'enum_pl_level' and s.enum_code in ('0','1','2','3','4')
  GROUP BY s.ENUM_CODE,d.task_no) y on t.task_no=y.task_no where t.task_no='{CHK_NO}'  order by y.ENUM_CODE asc";
              
                DataTable dt1 = DB.GetDataTable(sql,dic);
                dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                dic.Add("Data1", dt1);
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// <summary>
        ///皮料保存
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_add(object OBJ)
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
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//来料单号
                string warehousing_qty = jarr.ContainsKey("warehousing_qty") ? jarr["warehousing_qty"].ToString() : "";
                string bad_qty = jarr.ContainsKey("bad_qty") ? jarr["bad_qty"].ToString() : "";
                string bad_rate = jarr.ContainsKey("bad_rate") ? jarr["bad_rate"].ToString() : "";
                string spc_mining = jarr.ContainsKey("spc_mining") ? jarr["spc_mining"].ToString() : "";
                string actual_returned_qty = jarr.ContainsKey("actual_returned_qty") ? jarr["actual_returned_qty"].ToString() : "";
                string supplementary_delivery_qty = jarr.ContainsKey("supplementary_delivery_qty") ? jarr["supplementary_delivery_qty"].ToString() : "";
                string insp_report = jarr.ContainsKey("insp_report") ? jarr["insp_report"].ToString() : "";
                string return_reason = jarr.ContainsKey("return_reason") ? jarr["return_reason"].ToString() : "";
                string manufacturer_reply = jarr.ContainsKey("manufacturer_reply") ? jarr["manufacturer_reply"].ToString() : "";
                string pdj = jarr.ContainsKey("pdj") ? jarr["pdj"].ToString() : ""; //等级划分的
                string guid_list = jarr.ContainsKey("guid_list") ? jarr["guid_list"].ToString() : ""; //等级划分的
                if (pdj != null)
                {
                    var jarr2 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string,object>>(pdj);
                    string a = jarr.ContainsKey("a") ? jarr["a"].ToString() : "0";//I级
                    string b = jarr.ContainsKey("b") ? jarr["b"].ToString() : "0";//II级
                    string c = jarr.ContainsKey("c") ? jarr["c"].ToString() : "0";//III级
                    string d = jarr.ContainsKey("d") ? jarr["d"].ToString() : "0";//IV级
                    string e = jarr.ContainsKey("e") ? jarr["e"].ToString() : "0";//V级
                    string f = jarr.ContainsKey("f") ? jarr["f"].ToString() : "";//Ⅵ级,填写
                    string fail = jarr.ContainsKey("fail") ? jarr["fail"].ToString() : "";//不合格,填写
                    string pjsyl = jarr.ContainsKey("pjsyl") ? jarr["pjsyl"].ToString() : "";//平均使用率，填写
                    if(!string.IsNullOrWhiteSpace(DB.GetString($"select chk_no from qcm_iqc_insp_res_bad_r_a where chk_no='{chk_no}'"))) 
                    {
                        sql = $"insert into qcm_iqc_insp_res_bad_r_a(chk_no,insp1,insp2,insp3,insp4,insp5,insp6,createby,createdate,createtime) values('{chk_no}''{a}','{b}','{c}','{d}','{e}','{f}','{fail}','{pjsyl}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                    }
                    else
                    {
                        sql = $"update  qcm_iqc_insp_res_bad_r_a set chk_no='{chk_no}',insp1='{a}',insp2='{b}',insp3='{c}',insp4='{d}',insp5='{e}',insp6='{f}',modifyby='{userCode}',modifydate='{currDate.ToString("yyyy-MM-dd")}',modifytime='{currDate.ToString("HH:mm:ss")}' where chk_no='{chk_no}'";
                    }
                    DB.ExecuteNonQuery(sql);
                }
                string item_id = string.Empty;
                if (string.IsNullOrWhiteSpace(DB.GetString($"select chk_no From qcm_iqc_insp_res_bad_report where chk_no='{chk_no}'")))
                {
                    sql = $@"INSERT INTO qcm_iqc_insp_res_bad_report(chk_no,warehousing_qty,bad_qty,bad_rate,spc_mining,actual_returned_qty,supplementary_delivery_qty,insp_report,return_reason,manufacturer_reply,CREATEBY,CREATEDATE,CREATETIME) VALUES('{chk_no}','{warehousing_qty}','{bad_qty}','{bad_rate}','{spc_mining}','{actual_returned_qty}','{supplementary_delivery_qty}','{insp_report}','{return_reason}','{manufacturer_reply}','{userCode}','{currDate.ToString("yyyy-MM-dd")}','{currDate.ToString("HH:mm:ss")}')";
                    DB.ExecuteNonQuery(sql);//不良报告
                    item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_bad_report");
                }
                else
                {
                    sql = $@"update  qcm_iqc_insp_res_bad_report set chk_no='{chk_no}',warehousing_qty='{warehousing_qty}',bad_qty='{bad_qty}',bad_rate='{bad_rate}',spc_mining='{spc_mining}',actual_returned_qty='{actual_returned_qty}',supplementary_delivery_qty='{supplementary_delivery_qty}',insp_report='{insp_report}',return_reason='{return_reason}',manufacturer_reply='{manufacturer_reply}',modifyby='{userCode}',modifydate='{currDate.ToString("yyyy-MM-dd")}',modifytime='{currDate.ToString("HH:mm:ss")}' where chk_no='{chk_no}'";
                    DB.ExecuteNonQuery(sql);//不良报告
                }

                if (!string.IsNullOrWhiteSpace(guid_list))
                {
                    string[] guid_ls = guid_list.Split('?');
                    string m_id = DB.GetString($"select id from qcm_iqc_insp_res_bad_report where chk_no='{chk_no}'");
                    if (!string.IsNullOrWhiteSpace(item_id))
                    {
                        for (int i = 0; i < guid_ls.Length - 1; i++)
                        {
                            sql = $"insert into qcm_iqc_insp_res_bad_report_f(union_id,file_id) values('{item_id}','{guid_ls[i]}')";//添加对应的guid关联图片
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(m_id))
                    {
                        for (int i = 0; i < guid_ls.Length - 1; i++)
                        {
                            sql = $"insert into qcm_iqc_insp_res_bad_report_f(union_id,file_id) values('{m_id}','{guid_ls[i]}')";//添加对应的guid关联图片
                            DB.ExecuteNonQuery(sql);
                        }
                    }
                    if(string.IsNullOrWhiteSpace(item_id) && string.IsNullOrWhiteSpace(m_id))
                    {
                        ret.ErrMsg = "There is a bug in the photo upload, please check！";
                        ret.IsSuccess = false;
                        return ret;
                    }
                  
                }

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
        /// 不良报告处理信息展示,检验结果
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_view2(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//料号
                string ORDER_NO = jarr.ContainsKey("ORDER_NO") ? jarr["ORDER_NO"].ToString() : "";//
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//物料序号
                string status = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";//皮料的0，非皮料的1
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                #region 统一的数据源
                sql = $@"SELECT
	a.chk_no,
	a.item_no,
	a.chk_seq,
	a.test_item_no,
  a.test_item_name,
	a.test_standard,
	a.determine,
	a.remark,
	b.BADPROBLEM_CODE,
	b.BADPROBLEM_NAME
FROM
	qcm_iqc_insp_res_d a LEFT   JOIN qcm_iqc_badproblems_m b on a.badproblem_code=b.badproblem_code
WHERE
	a.CHK_NO=@CHK_NO and a.ITEM_NO=@ITEM_NO and a.CHK_SEQ=@CHK_SEQ";
                dic.Add("CHK_NO", CHK_NO);
                dic.Add("ITEM_NO", ITEM_NO);
                dic.Add("CHK_SEQ", CHK_SEQ);
                DataTable dt1 = DB.GetDataTable(sql, dic);//中间检验项目的
                string insp_report = DB.GetString($"select insp_report from qcm_iqc_insp_res_bad_report where chk_no=@CHK_SEQ and ITEM_NO=@ITEM_NO  and CHK_SEQ=@CHK_SEQ AND isdelete='0'  AND rownum=1",dic);//检验报告不合格说明
                
                sql = $@"SELECT
    A.ID,
	A.CHK_NO,
    A.ITEM_NO,
    A.CHK_SEQ,
	A.CLOSING_STATUS,
	A.WAREHOUSING_QTY,
	nvl(A.BAD_QTY,0)as BAD_QTY,
	A.BAD_RATE,
	nvl(A.SPC_MINING,0) as SPC_MINING,
	nvl(A.ACTUAL_RETURNED_QTY,0) as ACTUAL_RETURNED_QTY,
	nvl(A.SUPPLEMENTARY_DELIVERY_QTY,0) as SUPPLEMENTARY_DELIVERY_QTY,
	A.INSP_REPORT,
	A.RETURN_REASON,
	A.MANUFACTURER_REPLY
FROM
	QCM_IQC_INSP_RES_BAD_REPORT A WHERE A.CHK_NO=@CHK_NO  and  A.ITEM_NO=@ITEM_NO and A.CHK_SEQ=@CHK_SEQ and a.isdelete='0'";
                DataTable dt2 = DB.GetDataTable(sql, dic);//页面下半的数据

                dt2.Columns.Add("IMAGE_LIST", Type.GetType("System.Object")); 
                #endregion
                if (dt2.Rows.Count > 0)
                {
                    foreach (DataRow item in dt2.Rows)
                    {
                        sql = $@"SELECT
	W.ID,
	Q.FILE_NAME,
	Q.FILE_URL,
	Q.GUID,
	Q.SUFFIX
FROM
	BDM_UPLOAD_FILE_ITEM Q
INNER JOIN (
	SELECT
		A.ID,
		B.FILE_ID
	FROM
		qcm_iqc_insp_res_bad_report A
	INNER JOIN qcm_iqc_insp_res_bad_report_f B ON A.ID = B.UNION_ID where A.id='{item["id"]}'
) W ON Q.GUID = W.FILE_ID";
                        DataTable dt_img = DB.GetDataTable(sql);
                        List<imginfo> images = new List<imginfo>();
                        if (dt_img.Rows.Count > 0)
                        {
                            for (int i = 0; i <dt_img.Rows.Count; i++)
                            {
                                images.Add(new imginfo
                                {
                                    guid = dt_img.Rows[i]["GUID"].ToString(),
                                    image_url = dt_img.Rows[i]["FILE_URL"].ToString()
                                }); ;
                            }
                        }
                        item["IMAGE_LIST"] = images;
                    }
                }
                sql = $@"SELECT
	DISTINCT
	B.SUPPLIERS_CODE, --生产厂商编号
	B.SUPPLIERS_NAME, --生产厂商
	A.CHK_NO,--收料单号
	aw.NAME_T, --材料名称
	A1.ITEM_NO, --料号(材料编号)
   pe.ITEM_TYPE_NO,--物料类型（皮料，非皮料）
    nvl( A1.RCPT_QTY,0)as RCPT_QTY,--收料数量
    A1.CHK_SEQ,--收料序号
	a1.source_no as ORDER_NO, --采购单号
     a1.SOURCE_SEQ, --来源单序号
    nvl(A1.IV_QTY,0)as IV_QTY, --检验数
	nvl(QC.PASS_QTY,0)as PASS_QTY , --合格数
    '' as  SHOE_NO,--鞋型
     a.RCPT_DATE,--收料日期
	JA.CLOSING_STATUS --结案状态
FROM
	WMS_RCPT_M A 
LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE
LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A2.ORDER_NO=C1.ORDER_NO
LEFT JOIN qcm_iqc_insp_res_m QC  ON A.CHK_NO=QC.CHK_NO  AND A1.ITEM_NO=QC.ITEM_NO AND A1.CHK_SEQ=QC.CHK_SEQ
LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO  AND A1.ITEM_NO=JA.ITEM_NO AND A1.CHK_SEQ=JA.CHK_SEQ
 where   a.RCPT_BY='01' and   a.CHK_NO=@CHK_NO and a1.ITEM_NO=@ITEM_NO and A1.CHK_SEQ=@CHK_SEQ  and  pe.ITEM_TYPE_NO not  like '401%'";//非皮料数据
                DataTable dt_top = DB.GetDataTable(sql,dic);//没有保存前的数据展示
                #region 皮料的单独等级数据
                if (status == "0")
                {
                    sql = $@"SELECT
	DISTINCT
	B.SUPPLIERS_CODE, --生产厂商编号
	B.SUPPLIERS_NAME, --生产厂商
	A.CHK_NO,--收料单号
	aw.NAME_T, --材料名称
	A1.ITEM_NO, --料号(材料编号)
    a1.SOURCE_SEQ, --来源单序号
   pe.ITEM_TYPE_NO,--物料类型（皮料，非皮料）
    nvl(A1.RCPT_QTY,0)as RCPT_QTY,--收料数量
    A1.CHK_SEQ,--收料序号
	a1.source_no as  ORDER_NO, --采购单号
    nvl(A1.IV_QTY,0)as IV_QTY, --检验数
	nvl(QC.PASS_QTY,0)as PASS_QTY , --合格数
    '' as  SHOE_NO,--鞋型
    a.RCPT_DATE,--收料日期
	JA.CLOSING_STATUS --结案状态
FROM
	WMS_RCPT_M A 
LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO
LEFT  JOIN BASE003M B ON A.VEND_NO = B.SUPPLIERS_CODE
LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE
LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A2.ORDER_NO=C1.ORDER_NO
LEFT JOIN qcm_iqc_insp_res_m QC  ON A.CHK_NO=QC.CHK_NO  AND A1.ITEM_NO=QC.ITEM_NO AND A1.CHK_SEQ=QC.CHK_SEQ
LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO  AND A1.ITEM_NO=JA.ITEM_NO AND A1.CHK_SEQ=JA.CHK_SEQ
 where    a.RCPT_BY='01' and  a.CHK_NO=@CHK_NO and a1.ITEM_NO=@ITEM_NO  and A1.CHK_SEQ=@CHK_SEQ  and  pe.ITEM_TYPE_NO   like '401%'";//皮料数据
                    dt_top = DB.GetDataTable(sql,dic);
                   
                }
                //claim_no号码
                sql = $@"SELECT A.CLAIM_NO,A.CHK_NO,B.CHK_SEQ,B.SRC_ITEM_NO FROM  PO_CLAIM_M A LEFT JOIN PO_CLAIM_D B ON A.CLAIM_NO=B.CLAIM_NO WHERE A.CHK_NO=@CHK_NO AND SRC_ITEM_NO=@ITEM_NO AND B.CHK_SEQ=@CHK_SEQ ";
                DataTable dt_claim_no = DB.GetDataTable(sql,dic);

                DataTable dtrow = new DataTable();
                string name_t = string.Empty;
                string[] arr = null;
                foreach (DataRow item in dt_top.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(item["ORDER_NO"].ToString()))
                    {
                        dic = new Dictionary<string, object>();
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
where aa.ORDER_NO=@ORDER_NO  AND ee.ORDER_SEQ=@ORDER_SEQ  group by AA.ORDER_NO, ee.ORDER_SEQ";
                        dic.Add("ORDER_NO", item["ORDER_NO"]);
                        dic.Add("ORDER_SEQ", item["SOURCE_SEQ"]);
                        dtrow = DB.GetDataTable(sql,dic);
                        //ART_NO会出现 HQ1877 或  HQ1877,HQ1880,HQ1884
                        //SHOE_NO会出现 MDD33 或  MDD33,MDD34,MDD35
                        if (dtrow.Rows.Count > 0)
                        {
                            //item["SHOE_NO"] = dtrow.Rows[0]["SHOE_NO"].ToString();
                            //item["PROD_NO"] = dtrow.Rows[0]["ART_NO"].ToString();
                          
                            //不包含就只有一条或者没有
                            if (!string.IsNullOrWhiteSpace(dtrow.Rows[0]["SHOE_NO"].ToString()))
                            {
                                if (dtrow.Rows[0]["SHOE_NO"].ToString().Contains(','))
                                {
                                    name_t = string.Empty;
                                    sql = $"select NAME_T from BDM_RD_STYLE where SHOE_NO in ('{string.Join("','", arr)}')";
                                    dtrow = DB.GetDataTable(sql);
                                    if (dtrow.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < dtrow.Rows.Count; i++)
                                        {
                                            name_t+=dtrow.Rows[i]["NAME_T"].ToString();
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
                                        name_t = dtrow.Rows[0]["NAME_T"].ToString();

                                    }
                                }
                               
                            }
                            item["SHOE_NO"] = name_t;

                        }

                    }
                }
               
                #endregion

                dic.Add("Data_top", dt_top);
                dic.Add("Data1", dt1);
                dic.Add("Data2", dt2);
                dic.Add("insp_report", insp_report);
                dic.Add("dt_claim_no", dt_claim_no);
                ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
        /// <summary>
        /// 不良报告处理信息展示,检验结果(优化三)
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_view3(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//料号
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//物料序号
                string status = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";//皮料的0，非皮料的1
                string Type = jarr.ContainsKey("TYPE") ? jarr["TYPE"].ToString() : "";//皮料的0，非皮料的1
                string where = string.Empty;
                string sql = string.Empty;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                #region 统一的数据源
                if (status == "0")//皮料
                {
                    status = string.Empty;
                }
                else
                {
                    status = "not";
                }
                //if (Type == "Present")
                //{


                #region 2022/6/24前的
                //premika-start-2025/10/17
                sql = $@"SELECT
                    DISTINCT
                    max(pa.CLAIM_NO) as CLAIM_NO,--cimal_no单号
                    MAX(B.SUPPLIERS_NAME) as SUPPLIERS_NAME, --供应商
                    listagg(DISTINCT z2.NAME_T, ',') WITHIN GROUP (ORDER BY z2.NAME_T)  as NAME_S2, --鞋型名称
                    MAX(A1.ITEM_NO) as ITEM_NO, --料号(材料编号)
                    MAX(C1.ORDER_NO) as ORDER_NO, --采购单号
                  MAX(S.sample_qty) as sample_qty,--抽样数量
                    max(a.CHK_NO) as CHK_NO,--收料单号
                    max(a.RCPT_DATE) as RCPT_DATE,--收料日期（进仓日期）
                  CASE
                  WHEN (MAX(A1.RCPT_QTY)> 0) THEN
                    (round((MAX(S.sample_qty)/MAX(A1.RCPT_QTY)),3)*100)
                      ELSE
                     null 
                    END AS CJ_RATE,-- 抽检率
                    MAX(aw.NAME_T) as NAME_T, --材料名称
                    MAX(A1.RCPT_QTY) as RCPT_QTY,--进仓数量
                    MAX(JA.bad_qty) as bad_qty, --不良数量
                    MAX(JA.bad_rate) as bad_rate,--不良率
                    MAX(ja.spc_mining) as spc_mining,--特采数量
                    MAX(ja.actual_returned_qty) as actual_returned_qty,--实退数量
                    MAX(ja.supplementary_delivery_qty) as supplementary_delivery_qty,--补送数量
                    MAX(ja.insp_report) as insp_report,--检验报告,不合格说明
                    MAX(JA.return_reason) as return_reason,--退货原因
                    MAX(ja.manufacturer_reply) as manufacturer_reply,--厂商回复
                    MAX(ja.ID) as ID, --ID
                    MAX(JA.CLOSING_STATUS) AS CLOSING_STATUS, --结案状态
                    MAX(gh.grade_i) as G1,
                MAX(gh.grade_ii) as G2,
                MAX(gh.grade_iii) as G3,
                MAX(gh.grade_iv) as G4,
                MAX(gh.grade_v) as G5,
                MAX(gh.grade_vi) as G6,
                MAX(gh.un_qualified) as unqualified,
                MAX(gh.avg_util_rate) as avg_util_rate
                FROM
                    WMS_RCPT_M A 
                LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
                LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
                LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=a1.SOURCE_SEQ --新加的
                LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
                LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE

                LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
                LEFT  JOIN BASE003M B ON aw.vend_no_prd = B.SUPPLIERS_CODE

                LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
                LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO

                LEFT JOIN QCM_IQC_INSP_RES_M S ON A.CHK_NO=S.CHK_NO and a1.ITEM_NO=s.ITEM_NO and a1.CHK_SEQ=s.CHK_SEQ
                LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A1.SOURCE_NO=C1.ORDER_NO AND  a1.CHK_SEQ=c1.ORDER_SEQ
                LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO  AND A1.ITEM_NO=JA.ITEM_NO AND A1.CHK_SEQ=JA.CHK_SEQ and ja.isdelete='0'
                LEFT JOIN PO_CLAIM_M pm on pm.chk_no=ja.chk_no --结案查询clamin_单
                LEFT JOIN PO_CLAIM_D pa on pm.CLAIM_NO=pa.CLAIM_NO and pm.CHK_NO=ja.chk_no AND pa.SRC_ITEM_NO=ja.ITEM_NO AND pa.CHK_SEQ=ja.CHK_SEQ
                LEFT JOIN qcm_iqc_insp_res_bad_report_leather_grades gh on gh.id=ja.id and gh.chk_no=ja.chk_no and gh.item_no=ja.item_no
                 where  a.RCPT_BY='01' and   a1.CHK_NO='{CHK_NO}' and a1.chk_seq='{CHK_SEQ}'  and a1.ITEM_NO='{ITEM_NO}'  and  pe.ITEM_TYPE_NO {status}  like '401%' ";




                //premika-end-2025/10/17
                //previous query commented
                //sql = $@"SELECT
                //    DISTINCT
                //    max(pa.CLAIM_NO) as CLAIM_NO,--cimal_no单号
                //    MAX(B.SUPPLIERS_NAME) as SUPPLIERS_NAME, --供应商
                //    listagg(DISTINCT z2.NAME_T, ',') WITHIN GROUP (ORDER BY z2.NAME_T)  as NAME_S2, --鞋型名称
                //    MAX(A1.ITEM_NO) as ITEM_NO, --料号(材料编号)
                //    MAX(C1.ORDER_NO) as ORDER_NO, --采购单号
                //  MAX(S.sample_qty) as sample_qty,--抽样数量
                //    max(a.CHK_NO) as CHK_NO,--收料单号
                //    max(a.RCPT_DATE) as RCPT_DATE,--收料日期（进仓日期）
                //  CASE
                //  WHEN (MAX(A1.RCPT_QTY)> 0) THEN
                //    (round((MAX(S.sample_qty)/MAX(A1.RCPT_QTY)),3)*100)
                //      ELSE
                //     null 
                //    END AS CJ_RATE,-- 抽检率
                //    MAX(aw.NAME_T) as NAME_T, --材料名称
                //    MAX(A1.RCPT_QTY) as RCPT_QTY,--进仓数量
                //    MAX(JA.bad_qty) as bad_qty, --不良数量
                //    MAX(JA.bad_rate) as bad_rate,--不良率
                //    MAX(ja.spc_mining) as spc_mining,--特采数量
                //    MAX(ja.actual_returned_qty) as actual_returned_qty,--实退数量
                //    MAX(ja.supplementary_delivery_qty) as supplementary_delivery_qty,--补送数量
                //    MAX(ja.insp_report) as insp_report,--检验报告,不合格说明
                //    MAX(JA.return_reason) as return_reason,--退货原因
                //    MAX(ja.manufacturer_reply) as manufacturer_reply,--厂商回复
                //    MAX(ja.ID) as ID, --ID
                //    MAX(JA.CLOSING_STATUS) AS CLOSING_STATUS --结案状态
                //FROM
                //    WMS_RCPT_M A 
                //LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
                //LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
                //LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=a1.SOURCE_SEQ --新加的
                //LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
                //LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE

                //LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
                //LEFT  JOIN BASE003M B ON aw.vend_no_prd = B.SUPPLIERS_CODE

                //LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
                //LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO

                //LEFT JOIN QCM_IQC_INSP_RES_M S ON A.CHK_NO=S.CHK_NO and a1.ITEM_NO=s.ITEM_NO and a1.CHK_SEQ=s.CHK_SEQ
                //LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A1.SOURCE_NO=C1.ORDER_NO AND  a1.CHK_SEQ=c1.ORDER_SEQ
                //LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO  AND A1.ITEM_NO=JA.ITEM_NO AND A1.CHK_SEQ=JA.CHK_SEQ and ja.isdelete='0'
                //LEFT JOIN PO_CLAIM_M pm on pm.chk_no=ja.chk_no --结案查询clamin_单
                //LEFT JOIN PO_CLAIM_D pa on pm.CLAIM_NO=pa.CLAIM_NO and pm.CHK_NO=ja.chk_no AND pa.SRC_ITEM_NO=ja.ITEM_NO AND pa.CHK_SEQ=ja.CHK_SEQ 
                // where  a.RCPT_BY='01' and   a1.CHK_NO='{CHK_NO}' and a1.chk_seq='{CHK_SEQ}'  and a1.ITEM_NO='{ITEM_NO}'  and  pe.ITEM_TYPE_NO {status}  like '401%'
                #endregion


                DataTable dt = DB.GetDataTable(sql);//皮料/非皮料
                   
                    List<imginfo> images = new List<imginfo>();
                    if (dt.Rows.Count > 0)
                    {
                        sql = $@"SELECT
	A.ID,
  A.UNION_ID,
	B.FILE_NAME,
	B.FILE_URL,
	B.GUID,
	B.SUFFIX
FROM
	QCM_IQC_INSP_RES_BAD_REPORT_F A
INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_ID = B.GUID";
                        DataTable dt_img = DB.GetDataTable(sql);
                        DataRow[] dr = null;

                        foreach (DataRow item in dt.Rows)
                        {
                            dr = dt_img.Select($@"UNION_ID='{item["ID"]}'");
                            images = new List<imginfo>();
                            foreach (DataRow dr_img in dr)
                            {
                                images.Add(new imginfo
                                {
                                    id = dr_img["ID"].ToString(),
                                    guid = dr_img["GUID"].ToString(),
                                    image_url = dr_img["FILE_URL"].ToString()
                                }); ;
                            }
                        }
                    }
                    sql = $@"SELECT
	a.test_item_no,
    a.test_item_name,
	a.test_standard,
	a.determine,
	a.remark,
	b.BADPROBLEM_CODE,
	b.BADPROBLEM_NAME
FROM
	qcm_iqc_insp_res_d a LEFT   JOIN qcm_iqc_badproblems_m b on a.badproblem_code=b.badproblem_code
WHERE
	a.CHK_NO='{CHK_NO}' and a.ITEM_NO='{ITEM_NO}' and a.CHK_SEQ='{CHK_SEQ}'";
                    DataTable dt_txt = DB.GetDataTable(sql);
                    dic.Add("Data", dt);
                    dic.Add("img_list", images);
                    dic.Add("Dt_txt", dt_txt);
                    ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
                    ret.IsSuccess = true;

//                }
//                else
//                {
//                    #region 2022/6/24前的
//                    sql = $@"SELECT
//            DISTINCT
//            max(pa.CLAIM_NO) as CLAIM_NO,--cimal_no单号
//            MAX(B.SUPPLIERS_NAME) as SUPPLIERS_NAME, --供应商
//            listagg(DISTINCT z2.NAME_T, ',') WITHIN GROUP (ORDER BY z2.NAME_T)  as NAME_S2, --鞋型名称
//            MAX(A1.ITEM_NO) as ITEM_NO, --料号(材料编号)
//            MAX(C1.ORDER_NO) as ORDER_NO, --采购单号
//          sum(S.sample_qty) as sample_qty,--抽样数量
//            max(a.CHK_NO) as CHK_NO,--收料单号
//            max(a.RCPT_DATE) as RCPT_DATE,--收料日期（进仓日期）
//          CASE
//          WHEN (SUM(S.Pass_Qty)> 0) THEN
//            (round((sum(S.sample_qty)/SUM(S.Pass_Qty)),3)*100)
//              ELSE
//             null 
//            END AS CJ_RATE,-- 抽检率
//            MAX(aw.NAME_T) as NAME_T, --材料名称
//            SUM(S.Pass_Qty) as RCPT_QTY,--进仓数量
//            MAX(JA.bad_qty) as bad_qty, --不良数量
//             CASE
//          WHEN (SUM(S.Pass_Qty)> 0) THEN 
//            (round((SUM(JA.bad_qty)/sum(S.Pass_Qty)),3)*100) 
//              ELSE
//             null 
//            END AS bad_rate,--不良率
//            MAX(ja.spc_mining) as spc_mining,--特采数量
//            MAX(ja.actual_returned_qty) as actual_returned_qty,--实退数量
//            MAX(ja.supplementary_delivery_qty) as supplementary_delivery_qty,--补送数量
//            MAX(ja.insp_report) as insp_report,--检验报告,不合格说明
//            MAX(JA.return_reason) as return_reason,--退货原因
//            MAX(ja.manufacturer_reply) as manufacturer_reply,--厂商回复
//            MAX(ja.ID) as ID, --ID
//            MAX(JA.CLOSING_STATUS) AS CLOSING_STATUS --结案状态
//        FROM
//            WMS_RCPT_M A 
//        LEFT JOIN WMS_RCPT_D A1 ON A.CHK_NO=A1.CHK_NO
//        LEFT JOIN BDM_PURCHASE_ORDER_M A2 ON A1.SOURCE_NO=A2.ORDER_NO
//        LEFT  JOIN bdm_purchase_order_d z1 on z1.ORDER_NO=a2.ORDER_NO and z1.ORDER_SEQ=a1.SOURCE_SEQ --新加的
//        LEFT JOIN bdm_rd_prod z2 on z1.ART_NO=z2.PROD_NO 
//        LEFT  JOIN BASE003M B1 ON A2.VEND_NO = B1.SUPPLIERS_CODE

//        LEFT JOIN bdm_rd_item aw on a1.ITEM_NO=aw.ITEM_NO
//        LEFT  JOIN BASE003M B ON aw.vend_no_prd = B.SUPPLIERS_CODE

//        LEFT  JOIN BDM_RD_ITEMTYPE pe on  AW.ITEM_TYPE=pe.ITEM_TYPE_NO
//        LEFT JOIN bdm_rd_prod pd on aw.PARENT_ITEM_NO=pd.PROD_NO

//        LEFT JOIN QCM_IQC_INSP_RES_M S ON A.CHK_NO=S.CHK_NO and a1.ITEM_NO=s.ITEM_NO and a1.CHK_SEQ=s.CHK_SEQ
//        LEFT JOIN BDM_PURCHASE_ORDER_ITEM C1 ON A1.SOURCE_NO=C1.ORDER_NO AND  a1.CHK_SEQ=c1.ORDER_SEQ
//        LEFT JOIN QCM_IQC_INSP_RES_BAD_REPORT JA ON A.CHK_NO=JA.CHK_NO  AND A1.ITEM_NO=JA.ITEM_NO AND A1.CHK_SEQ=JA.CHK_SEQ and ja.isdelete='0'
//        LEFT JOIN PO_CLAIM_M pm on pm.chk_no=ja.chk_no --结案查询clamin_单
//        LEFT JOIN PO_CLAIM_D pa on pm.CLAIM_NO=pa.CLAIM_NO and pm.CHK_NO=ja.chk_no AND pa.SRC_ITEM_NO=ja.ITEM_NO AND pa.CHK_SEQ=ja.CHK_SEQ 
//         where  a.RCPT_BY='01' and   a1.ITEM_NO='{ITEM_NO}'  and  pe.ITEM_TYPE_NO {status}  like '401%' ";
//#endregion



//                    DataTable dt = DB.GetDataTable(sql);//皮料/非皮料
#endregion
//                    List<imginfo> images = new List<imginfo>();
//                    if (dt.Rows.Count > 0)
//                    {
//                        sql = $@"SELECT
//	A.ID,
//  A.UNION_ID,
//	B.FILE_NAME,
//	B.FILE_URL,
//	B.GUID,
//	B.SUFFIX
//FROM
//	QCM_IQC_INSP_RES_BAD_REPORT_F A
//INNER JOIN BDM_UPLOAD_FILE_ITEM B ON A.FILE_ID = B.GUID";
//                        DataTable dt_img = DB.GetDataTable(sql);
//                        DataRow[] dr = null;

//                        foreach (DataRow item in dt.Rows)
//                        {
//                            dr = dt_img.Select($@"UNION_ID='{item["ID"]}'");
//                            images = new List<imginfo>();
//                            foreach (DataRow dr_img in dr)
//                            {
//                                images.Add(new imginfo
//                                {
//                                    id = dr_img["ID"].ToString(),
//                                    guid = dr_img["GUID"].ToString(),
//                                    image_url = dr_img["FILE_URL"].ToString()
//                                }); ;
//                            }
//                        }
//                    }
//                    sql = $@"SELECT
//	a.test_item_no,
//    a.test_item_name,
//	a.test_standard,
//	a.determine,
//	a.remark,
//	b.BADPROBLEM_CODE,
//	b.BADPROBLEM_NAME
//FROM
//	qcm_iqc_insp_res_d a LEFT   JOIN qcm_iqc_badproblems_m b on a.badproblem_code=b.badproblem_code
//WHERE
//	a.CHK_NO='{CHK_NO}' and a.ITEM_NO='{ITEM_NO}' and a.CHK_SEQ='{CHK_SEQ}'";
//                    DataTable dt_txt = DB.GetDataTable(sql);
//                    dic.Add("Data", dt);
//                    dic.Add("img_list", images);
//                    dic.Add("Dt_txt", dt_txt);
//                    ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
//                    ret.IsSuccess = true;
//                }
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

//        
        /// <summary>
        ///不良报告提交保存
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_add2(object OBJ)
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
                string sql = string.Empty;
             
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//来料单号
                string warehousing_qty = jarr.ContainsKey("warehousing_qty") ? jarr["warehousing_qty"].ToString() : "";
                string bad_qty = jarr.ContainsKey("bad_qty") ? jarr["bad_qty"].ToString() : "";
                string bad_rate = jarr.ContainsKey("bad_rate") ? jarr["bad_rate"].ToString() : "";
                string spc_mining = jarr.ContainsKey("spc_mining") ? jarr["spc_mining"].ToString() : "";
                string actual_returned_qty = jarr.ContainsKey("actual_returned_qty") ? jarr["actual_returned_qty"].ToString() : "";
                string supplementary_delivery_qty = jarr.ContainsKey("supplementary_delivery_qty") ? jarr["supplementary_delivery_qty"].ToString() : "";
                string insp_report = jarr.ContainsKey("insp_report") ? jarr["insp_report"].ToString() : "";
                string return_reason = jarr.ContainsKey("return_reason") ? jarr["return_reason"].ToString() : "";
                string manufacturer_reply = jarr.ContainsKey("manufacturer_reply") ? jarr["manufacturer_reply"].ToString() : "";
                string closing_status = jarr.ContainsKey("closing_status") ? jarr["closing_status"].ToString() : "";//结案操作的
                string item_id = string.Empty;
                string ITEM_NO= jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//结案操作的
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");

                //premika-start
                string ST_GRADE_I = jarr.ContainsKey("GRADE_I") ? jarr["GRADE_I"].ToString() : "";
                string ST_GRADE_II = jarr.ContainsKey("GRADE_II") ? jarr["GRADE_II"].ToString() : "";
                string ST_GRADE_III = jarr.ContainsKey("GRADE_III") ? jarr["GRADE_III"].ToString() : "";
                string ST_GRADE_IV = jarr.ContainsKey("GRADE_IV") ? jarr["GRADE_IV"].ToString() : "";
                string ST_GRADE_V = jarr.ContainsKey("GRADE_V") ? jarr["GRADE_V"].ToString() : "";
                string ST_GRADE_VI = jarr.ContainsKey("GRADE_VI") ? jarr["GRADE_VI"].ToString() : "";
                string ST_UNQUALIFIED = jarr.ContainsKey("UNQUALIFIED") ? jarr["UNQUALIFIED"].ToString() : "";
                string ST_AVG_UTIL_RATE = jarr.ContainsKey("AVG_UTIL_RATE") ? jarr["AVG_UTIL_RATE"].ToString() : "";
                double GRADE_I = jarr.ContainsKey("GRADE_I") && double.TryParse(jarr["GRADE_I"]?.ToString(), out double g1) ? g1 : 0;
                double GRADE_II = jarr.ContainsKey("GRADE_II") && double.TryParse(jarr["GRADE_II"]?.ToString(), out double g2) ? g2 : 0;
                double GRADE_III = jarr.ContainsKey("GRADE_III") && double.TryParse(jarr["GRADE_III"]?.ToString(), out double g3) ? g3 : 0;
                double GRADE_IV = jarr.ContainsKey("GRADE_IV") && double.TryParse(jarr["GRADE_IV"]?.ToString(), out double g4) ? g4 : 0;
                double GRADE_V = jarr.ContainsKey("GRADE_V") && double.TryParse(jarr["GRADE_V"]?.ToString(), out double g5) ? g5 : 0;
                double GRADE_VI = jarr.ContainsKey("GRADE_VI") && double.TryParse(jarr["GRADE_VI"]?.ToString(), out double g6) ? g6 : 0;
                double UNQUALIFIED = jarr.ContainsKey("UNQUALIFIED") && double.TryParse(jarr["UNQUALIFIED"]?.ToString(), out double u) ? u : 0;
                double AVG_UTIL_RATE = jarr.ContainsKey("AVG_UTIL_RATE") && double.TryParse(jarr["AVG_UTIL_RATE"]?.ToString(), out double avg) ? avg : 0.0;

                string ITEM_NAME = jarr.ContainsKey("ITEM_NAME") ? jarr["ITEM_NAME"].ToString() : "";
                string material_type = jarr.ContainsKey("status") ? jarr["status"].ToString() : "";
                //premika-end


                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("CHK_NO", chk_no);
                dic.Add("ITEM_NO", ITEM_NO);
                dic.Add("CHK_SEQ", CHK_SEQ);

                sql = $@"SELECT ID FROM QCM_IQC_INSP_RES_BAD_REPORT WHERE CHK_NO=@CHK_NO AND ITEM_NO=@ITEM_NO AND CHK_SEQ=@CHK_SEQ";

                string ID = DB.GetString(sql, dic);
                if (string.IsNullOrWhiteSpace(ID))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "The bad report does not exist or has been deleted, please refresh the page and try again";//该不良报告不存在或已被删除，请刷新页面重试
                    DB.Close();
                    return ret;
                    //dic = new Dictionary<string, object>();
                    //dic.Add("chk_no", chk_no);
                    //dic.Add("item_no", ITEM_NO);
                    //dic.Add("chk_seq", CHK_SEQ);
                    //if (closing_status == "0")
                    //{
                    //    sql = $@"insert into qcm_iqc_insp_res_bad_report(chk_no,item_no,chk_seq,closing_status,warehousing_qty,bad_qty,bad_rate,spc_mining,actual_returned_qty,supplementary_delivery_qty,insp_report,return_reason,manufacturer_reply,createby,createdate,createtime) VALUES(@chk_no,@item_no,@chk_seq,@closing_status,@warehousing_qty,@bad_qty,@bad_rate,@spc_mining,@actual_returned_qty,@supplementary_delivery_qty,@insp_report,@return_reason,@manufacturer_reply,@createby,@createdate,@createtime)";
                    //    dic.Add("closing_status", closing_status);
                    //}
                    //else
                    //{
                    //    sql = $@"insert into qcm_iqc_insp_res_bad_report(chk_no,item_no,chk_seq,warehousing_qty,bad_qty,bad_rate,spc_mining,actual_returned_qty,supplementary_delivery_qty,insp_report,return_reason,manufacturer_reply,createby,createdate,createtime) VALUES(@chk_no,@item_no,@chk_seq,@warehousing_qty,@bad_qty,@bad_rate,@spc_mining,@actual_returned_qty,@supplementary_delivery_qty,@insp_report,@return_reason,@manufacturer_reply,@createby,@createdate,@createtime)";
                    //}
                    //dic.Add("warehousing_qty", warehousing_qty);
                    //dic.Add("bad_qty", bad_qty);
                    //dic.Add("bad_rate", bad_rate);
                    //dic.Add("spc_mining", spc_mining);
                    //dic.Add("actual_returned_qty", actual_returned_qty);
                    //dic.Add("supplementary_delivery_qty", supplementary_delivery_qty);
                    //dic.Add("insp_report", insp_report);
                    //dic.Add("return_reason", return_reason);
                    //dic.Add("manufacturer_reply", manufacturer_reply);
                    //dic.Add("createby", userCode);
                    //dic.Add("createdate", date);
                    //dic.Add("createtime", time);
                    //DB.ExecuteNonQuery(sql,dic);//不良报告
                    //item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_iqc_insp_res_bad_report");
                }
                else
                {
                  
                    sql = $@"update  qcm_iqc_insp_res_bad_report set closing_status=@closing_status,isdelete=@isdelete,warehousing_qty=@warehousing_qty,bad_qty=@bad_qty,bad_rate=@bad_rate,spc_mining=@spc_mining,actual_returned_qty=@actual_returned_qty,supplementary_delivery_qty=@supplementary_delivery_qty,insp_report=@insp_report,return_reason=@return_reason,manufacturer_reply=@manufacturer_reply,modifyby=@modifyby,modifydate=@modifydate,modifytime=@modifytime where chk_no=@chk_no and item_no=@item_no AND chk_seq=@chk_seq";
                    dic = new Dictionary<string, object>();
                    dic.Add("closing_status", closing_status);
                    dic.Add("isdelete", "0");
                    dic.Add("warehousing_qty", warehousing_qty);
                    dic.Add("bad_qty", bad_qty);
                    dic.Add("bad_rate", bad_rate);
                    dic.Add("spc_mining", spc_mining);
                    dic.Add("actual_returned_qty", actual_returned_qty);
                    dic.Add("supplementary_delivery_qty", supplementary_delivery_qty);
                    dic.Add("insp_report", insp_report);
                    dic.Add("return_reason", return_reason);
                    dic.Add("manufacturer_reply", manufacturer_reply);
                    dic.Add("modifyby", userCode);
                    dic.Add("modifydate", date);
                    dic.Add("modifytime", time);
                    dic.Add("chk_no", chk_no);
                    dic.Add("item_no", ITEM_NO);
                    dic.Add("chk_seq", CHK_SEQ);
                    DB.ExecuteNonQuery(sql,dic);//不良报告
                    item_id = ID;
                    //premika-start-2025/10/17
                    if (material_type == "0")
                    {
                        if (!string.IsNullOrWhiteSpace(ST_GRADE_I) && !string.IsNullOrWhiteSpace(ST_GRADE_II) && !string.IsNullOrWhiteSpace(ST_GRADE_III) && !string.IsNullOrWhiteSpace(ST_GRADE_IV) && !string.IsNullOrWhiteSpace(ST_GRADE_V) && !string.IsNullOrWhiteSpace(ST_GRADE_VI) && !string.IsNullOrWhiteSpace(ST_UNQUALIFIED) && !string.IsNullOrWhiteSpace(ST_AVG_UTIL_RATE))
                        {
                            Dictionary<string, object> dics = new Dictionary<string, object>();
                            dics.Add("CHK_NO", chk_no);
                            dics.Add("ITEM_NO", ITEM_NO);
                            dics.Add("ID", ID);
                            sql = $@"SELECT COUNT(*) as GRADE_COUNT from qcm_iqc_insp_res_bad_report_leather_grades WHERE CHK_NO=@CHK_NO AND ITEM_NO=@ITEM_NO AND ID=@ID";

                            string GRADE_COUNT = DB.GetString(sql, dics);
                            if (GRADE_COUNT == "0")
                            {
                                sql = $@"INSERT INTO qcm_iqc_insp_res_bad_report_leather_grades(ID,CHK_NO,ITEM_NO,ITEM_NAME,Grade_I,Grade_II,Grade_III,Grade_IV,Grade_V,Grade_VI,Un_Qualified,AVG_UTIL_RATE,INSERT_DATE,INSERT_USER,CHK_SEQ,STATUS)values('{item_id}','{chk_no}','{ITEM_NO}','{ITEM_NAME}','{GRADE_I}',{GRADE_II},'{GRADE_III}',{GRADE_IV},{GRADE_V},'{GRADE_VI}','{UNQUALIFIED}','{AVG_UTIL_RATE}','{date}','{userCode}','{CHK_SEQ}','{0}')";
                                DB.ExecuteNonQuery(sql);
                            }
                            else
                            {
                                sql = $@"update qcm_iqc_insp_res_bad_report_leather_grades set STATUS='{0}',Grade_I='{GRADE_I}',Grade_II='{GRADE_II}',Grade_III='{GRADE_III}',Grade_IV='{GRADE_IV}',Grade_V='{GRADE_V}',Grade_VI='{GRADE_VI}',Un_Qualified='{UNQUALIFIED}',AVG_UTIL_RATE='{AVG_UTIL_RATE}',UPDATE_DATE='{date}',UPDATE_USER='{userCode}' where chk_no='{chk_no}' and item_no='{ITEM_NO}' AND ID='{ID}'";
                                DB.ExecuteNonQuery(sql);
                            }
                        }
                     
                       
                    }

                    //premika-end-2025/10/17

                }
                sql = "SELECT 'C0'||PO_CLAIM_D_SEQ.Nextval AS claim_no FROM dual";
                DataTable dt = DB.GetDataTable(sql);
                if (closing_status == "0")//结案就操作cliam_no单
                {
                    if (dt.Rows.Count > 0)
                    {
                        sql = $@"
	SELECT
	b.CHK_NO,
	b.item_no,
	b.chk_seq,
	b.ORG_ID,
    a.VEND_NO,
	c1.ORDER_NO
FROM
	WMS_RCPT_M   A
LEFT JOIN WMS_RCPT_d b ON A.CHK_NO = b.CHK_NO
LEFT JOIN bdm_purchase_order_m c on b.SOURCE_NO=c.ORDER_NO
LEFT JOIN bdm_purchase_order_item c1 on c.ORDER_NO=c1.ORDER_NO AND b.ITEM_NO=c1.item_no
where  a.RCPT_BY='01' and  a.CHK_NO=@CHK_NO and b.ITEM_NO=@ITEM_NO and b.CHK_SEQ=@CHK_SEQ";
                        dic = new Dictionary<string, object>();
                        dic.Add("CHK_NO", chk_no);
                        dic.Add("ITEM_NO", ITEM_NO);
                        dic.Add("CHK_SEQ", CHK_SEQ);
                        DataTable dt_row = DB.GetDataTable(sql,dic);

                        string claim = dt.Rows[0]["claim_no"].ToString();
                        sql = $@"SELECT A.CLAIM_NO,A.CHK_NO,B.CHK_SEQ,B.SRC_ITEM_NO FROM  PO_CLAIM_M A LEFT JOIN PO_CLAIM_D B ON A.CLAIM_NO=B.CLAIM_NO WHERE A.CHK_NO='{chk_no}' AND B.CHK_SEQ='{CHK_SEQ}' AND SRC_ITEM_NO='{ITEM_NO}'";
                        DataTable sd = DB.GetDataTable(sql);
                        sql = $@"select STAFF_DEPARTMENT from hr001m where staff_no='{userCode}'";
                        string bar = DB.GetString(sql);//部门
                        if (string.IsNullOrEmpty(bar))
                        {
                            ret.ErrMsg = $@"The operator {userCode} has not been assigned a department, cannot generate a claim form, and cannot close the case";
                            ret.IsSuccess = false;
                            return ret;
                        }
                        if (sd.Rows.Count < 1)
                        {

                            if (dt_row.Rows.Count > 0)
                            {
                                sql = $@"INSERT INTO po_claim_m(org_id,claim_no,vend_no,cla_type,chk_no,cla_date,status,last_date,insert_date,GRT_DEPT,GRT_USER,LAST_USER)values('{dt_row.Rows[0]["ORG_ID"]}','{claim}','{dt_row.Rows[0]["VEND_NO"]}','1','{chk_no}',TRUNC(SYSDATE),'7',sysdate,sysdate,'{bar}','{userCode}','{userCode}')";
                                DB.ExecuteNonQuery(sql);
                                sql = $@"
INSERT INTO po_claim_d(org_id,CLAIM_SEQ,claim_no,chk_seq,src_item_no,ng_qty,order_no,is_status,insert_date,last_date,src_order_no) values('{dt_row.Rows[0]["ORG_ID"]}','1','{claim}','{CHK_SEQ}','{ITEM_NO}','{actual_returned_qty}','{dt_row.Rows[0]["ORDER_NO"]}','Y',sysdate,sysdate,'{dt_row.Rows[0]["ORDER_NO"]}')";
                                DB.ExecuteNonQuery(sql);
                            }
                        }
                    }
                }
                List<imginfo> img_url = JsonConvert.DeserializeObject<List<imginfo>>(jarr["guid_list"].ToString());
                
                if (img_url.Count>0)
                {

                    if (!string.IsNullOrWhiteSpace(item_id))
                    {
                        dic = new Dictionary<string, object>();
                        int index = 0;
                        sql = string.Empty;
                        string sql2 = string.Empty;
                        List<string> id_list = new List<string>();
                        foreach (imginfo item in img_url)
                        {
                            if (!string.IsNullOrWhiteSpace(item.id))
                            {
                                id_list.Add(item.id);
                            }
                            if (!string.IsNullOrWhiteSpace(item.guid))
                            {
                                index++;
                                dic.Add($@"union_id{index}", item_id);
                                dic.Add($@"file_id{index}", item.guid);
                                dic.Add($@"createby{index}", userCode);
                                dic.Add($@"createdate{index}", date);
                                dic.Add($@"createtime{index}", time);
                                sql += $"insert into qcm_iqc_insp_res_bad_report_f(union_id,file_id,createby,createdate,createtime) values(@union_id{index},@file_id{index},@createby{index},@createdate{index},@createtime{index});";//添加对应的guid关联图片
                            }
                            
                          
                        }
                        if (id_list.Count > 0)
                        {
                            sql2 = $"delete from qcm_iqc_insp_res_bad_report_f where union_id='{item_id}' and id not  in ('{string.Join("','", id_list)}')";
                           
                        }
                        else
                        {
                            sql2 = $@"delete from qcm_iqc_insp_res_bad_report_f where  union_id='{item_id}'";
                        }
                        DB.ExecuteNonQuery(sql2);
                        if (!string.IsNullOrWhiteSpace(sql))
                        {
                            DB.ExecuteNonQuery($@"begin {sql}  end;",dic);
                        }
                        
                    }

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
        /// <summary>
        /// 不良报告软删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_delete(object OBJ)
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
                string sql = string.Empty;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() :"0";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//收料单号
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                //sql = $"update qcm_iqc_insp_res_m  set isdelete='1',modifyby='{userCode}',modifydate='{date}',modifytime='{time}' where  chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";//初始化检验结果
                //DB.ExecuteNonQuery(sql);
                sql = $"delete from qcm_iqc_insp_res_m where chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";//初始化检验结果 硬删
                DB.ExecuteNonQuery(sql);

                //sql = $"update qcm_iqc_insp_res_d set isdelete='1' where chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";//初始化检验结果
                //DB.ExecuteNonQuery(sql);
                sql = $"delete from qcm_iqc_insp_res_d  where chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";//初始化检验结果 硬删
                DB.ExecuteNonQuery(sql);

                sql = $"delete from QCM_IQC_INSP_RES_BAD_R_A where  chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";
                DB.ExecuteNonQuery(sql);//删除签名确认

                //取消还未结案的改状态为1,相当于取消
                //sql = $@"update qcm_iqc_insp_res_bad_report set isdelete='1' where  chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}' and closing_status='1'";
                //DB.ExecuteNonQuery(sql);
                sql = $"delete from qcm_iqc_insp_res_bad_report where chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}' and closing_status='1'";//硬删
                DB.ExecuteNonQuery(sql);

                sql = $"select closing_status  From qcm_iqc_insp_res_bad_report where chk_no='{chk_no}' and ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";
                string closing_status = DB.GetString(sql);
                if (closing_status=="0")
                {
                    ret.ErrMsg = $"[Receipt No. {chk_no}*Item No. {ITEM_NO}*Item No. {CHK_SEQ}] has been closed, and the operation of canceling the bad report cannot be performed";
                    ret.IsSuccess = false;
                    return ret;
                }
               

                DB.Commit();
                ret.ErrMsg = "Successful operation！";//操作成功
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "operation failed！";//操作失败
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }
        /// <summary>
        /// 取消结案
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Report_jiean(object OBJ)
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
                string sql = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "0";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//料号
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//料号序号
                string CLAIM_NO = jarr.ContainsKey("CLAIM_NO") ? jarr["CLAIM_NO"].ToString() : "";//claim_no单号

                string MTYPE = jarr.ContainsKey("STATUS") ? jarr["STATUS"].ToString() : "";//premika


                sql = $"SELECT CLOSING_STATUS FROM QCM_IQC_INSP_RES_BAD_REPORT WHERE   CHK_NO='{CHK_NO}' AND ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";
                string CLOSING_STATUS = DB.GetString(sql);
                sql = $@"select source_no from wms_issue_m where source_no like '{CLAIM_NO}%'";
                if (!string.IsNullOrWhiteSpace(CLAIM_NO))
                {
                    if (DB.GetDataTable(sql).Rows.Count > 0)
                    {
                        ret.ErrMsg = $"The Claim_no: {CLAIM_NO} has been returned and cannot be canceled";
                        ret.IsSuccess = false;
                        return ret;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(ITEM_NO))
                        {
                            sql = $@"delete from po_claim_d where claim_no='{CLAIM_NO}' and SRC_ITEM_NO='{ITEM_NO}'";
                            DB.ExecuteNonQuery(sql);

                            sql = $@"delete from po_claim_m where claim_no='{CLAIM_NO}' and CHK_NO='{CHK_NO}'";
                            DB.ExecuteNonQuery(sql);

                        }
                        else
                        {
                            ret.ErrMsg = $"The order number to be deleted: {CLAIM_NO} lacks the SRC_ITEM_NO condition [{ITEM_NO}], please check";
                            ret.IsSuccess = false;
                            return ret;
                        }
                    }
                    

                }
                if (string.IsNullOrWhiteSpace(CLOSING_STATUS))
                {
                    ret.ErrMsg = $"The document of [material receipt number {CHK_NO}*material number {ITEM_NO}*material number sequence number {CHK_SEQ}] has not been closed yet, and the cancel closing operation cannot be performed";
                    ret.IsSuccess = false;
                    return ret;
                }
                sql = $@"select ID from QCM_IQC_INSP_RES_BAD_REPORT where  CHK_NO ='{CHK_NO}' AND ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";
                int ID = DB.GetInt32(sql);
                if (ID> 0)
                {
                    sql = $"UPDATE QCM_IQC_INSP_RES_BAD_REPORT SET CLOSING_STATUS='1',insp_report='',return_reason='',manufacturer_reply='',spc_mining='',actual_returned_qty='',supplementary_delivery_qty='',modifyby='{userCode}',modifydate='{currDate.ToString("yyyy-MM-dd")}',modifytime='{currDate.ToString("HH:mm:ss")}' WHERE ID='{ID}'";
                    DB.ExecuteNonQuery(sql);
                    sql = $@"DELETE FROM qcm_iqc_insp_res_bad_report_f WHERE union_id='{ID}'";
                    DB.ExecuteNonQuery(sql);
                }
                //premika start
                if (MTYPE == "0")
                {

                    sql = $@"update qcm_iqc_insp_res_bad_report_leather_grades set STATUS='1',GRADE_I='',GRADE_II='',GRADE_III='',GRADE_IV='',GRADE_V='',GRADE_VI='',UN_QUALIFIED='',AVG_UTIL_RATE='',UPDATE_USER='{userCode}',UPDATE_DATE='{currDate.ToString("yyyy-MM-dd")}' where chk_no='{CHK_NO}' and chk_seq='{CHK_SEQ}' and item_no='{ITEM_NO}'";
                    DB.ExecuteNonQuery(sql);
                }
                //premika end
                sql = $"delete from QCM_IQC_INSP_RES_BAD_R_A where chk_no='{CHK_NO}' and chk_seq='{CHK_SEQ}' and item_no='{ITEM_NO}'";//硬删
                DB.ExecuteNonQuery(sql);

                DB.Commit();
                ret.ErrMsg = "Successful operation！";
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                DB.Rollback();
                ret.IsSuccess = false;
                ret.ErrMsg = "operation failed！";
            }
            finally
            {
                DB.Close();
            }
            return ret;
        }

        #region 客户退货

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReturnDatalist(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                string where = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string REGION = jarr.ContainsKey("REGION") ? jarr["REGION"].ToString() : "";//

                string FACTORY_NO = jarr.ContainsKey("FACTORY_NO") ? jarr["FACTORY_NO"].ToString() : "";//
                string FACTORY_NAME = jarr.ContainsKey("FACTORY_NAME") ? jarr["FACTORY_NAME"].ToString() : "";//

                string SALESORGAN_NO = jarr.ContainsKey("SALESORGAN_NO") ? jarr["SALESORGAN_NO"].ToString() : "";//
                string SALESORGAN_NAME = jarr.ContainsKey("SALESORGAN_NAME") ? jarr["SALESORGAN_NAME"].ToString() : "";//

                string ARTICLE = jarr.ContainsKey("ARTICLE") ? jarr["ARTICLE"].ToString() : "";//
                string SHOES_NAME = jarr.ContainsKey("SHOES_NAME") ? jarr["SHOES_NAME"].ToString() : "";//

                string MASTERCODE = jarr.ContainsKey("MASTERCODE") ? jarr["MASTERCODE"].ToString() : "";//
                string MASTERNAME = jarr.ContainsKey("MASTERNAME") ? jarr["MASTERNAME"].ToString() : "";//

                string SECONDCODE = jarr.ContainsKey("SECONDCODE") ? jarr["SECONDCODE"].ToString() : "";//
                string SECONDNAME = jarr.ContainsKey("SECONDNAME") ? jarr["SECONDNAME"].ToString() : "";//

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//

                string putin_date = jarr.ContainsKey("putin_date") ? jarr["putin_date"].ToString() : "";//
                string end_date = jarr.ContainsKey("end_date") ? jarr["end_date"].ToString() : "";//

                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";

                #region 条件
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(REGION))
                {
                    where += " and REGION like @REGION";
                    paramTestDic.Add("REGION", $@"%{REGION}%");
                }
                if (!string.IsNullOrEmpty(FACTORY_NO))
                {
                    where += " and FACTORY_NO like @FACTORY_NO";
                    paramTestDic.Add("FACTORY_NO", $@"%{FACTORY_NO}%");
                }
                if (!string.IsNullOrEmpty(FACTORY_NAME))
                {
                    where += " and FACTORY_NAME like @FACTORY_NAME";
                    paramTestDic.Add("FACTORY_NAME", $@"%{FACTORY_NAME}%");
                }

                if (!string.IsNullOrEmpty(SALESORGAN_NO))
                {
                    where += " and SALESORGAN_NO like @SALESORGAN_NO";
                    paramTestDic.Add("SALESORGAN_NO", $@"%{SALESORGAN_NO}%");
                }
                if (!string.IsNullOrEmpty(SALESORGAN_NAME))
                {
                    where += " and SALESORGAN_NAME like @SALESORGAN_NAME";
                    paramTestDic.Add("SALESORGAN_NAME", $@"%{SALESORGAN_NAME}%");
                }

                if (!string.IsNullOrEmpty(ARTICLE))
                {
                    where += " and ARTICLE like @ARTICLE";
                    paramTestDic.Add("ARTICLE", $@"%{ARTICLE}%");
                }

                if (!string.IsNullOrEmpty(SHOES_NAME))
                {
                    where += " and SHOES_NAME like @SHOES_NAME";
                    paramTestDic.Add("SHOES_NAME", $@"%{SHOES_NAME}%");
                }

                if (!string.IsNullOrEmpty(MASTERCODE))
                {
                    where += " and MASTERCODE like @MASTERCODE";
                    paramTestDic.Add("MASTERCODE", $@"%{MASTERCODE}%");
                }

                if (!string.IsNullOrEmpty(MASTERNAME))
                {
                    where += " and MASTERNAME like @MASTERNAME";
                    paramTestDic.Add("MASTERNAME", $@"%{MASTERNAME}%");
                }

                if (!string.IsNullOrEmpty(SECONDCODE))
                {
                    where += " and SECONDCODE like @SECONDCODE";
                    paramTestDic.Add("SECONDCODE", $@"%{SECONDCODE}%");
                }
                if (!string.IsNullOrEmpty(SECONDNAME))
                {
                    where += " and SECONDNAME like @SECONDNAME";
                    paramTestDic.Add("SECONDNAME", $@"%{SECONDNAME}%");
                }
                if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                {
                    //where += $@"AND ( PRODUCTION_DATE BETWEEN '{starttime}' AND '{endtime}') ";
                    starttime = DateTime.Parse(starttime).ToString("yyyy-MM-01");
                    endtime = DateTime.Parse(DateTime.Parse(endtime).ToString("yyyy-MM-01")).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
                    where += $@" and TO_CHAR( PRODUCTION_DATE ,'yyyy-MM-dd') BETWEEN @start_date AND @end_time";
                    paramTestDic.Add("start_date", $@"{starttime}");
                    paramTestDic.Add("end_time", $@"{endtime}");
                }
                if (!string.IsNullOrEmpty(putin_date) && !string.IsNullOrEmpty(end_date))
                { 
                    where += $@" and return_month  BETWEEN @putin_date AND @end_date";
                    paramTestDic.Add("putin_date", $@"{putin_date}");
                    paramTestDic.Add("end_date", $@"{end_date}");
                }
                #endregion
                sql = $@"

SELECT 
    ID,
    RETURN_MONTH,
    REGION,
    FACTORY_NO,
    FACTORY_NAME,
    SALESORGAN_NO,
    SALESORGAN_NAME,
    ARTICLE,
    SHOES_NAME,
    TO_CHAR (PRODUCTION_DATE,'MM-yyyy') PRODUCTION_DATE,
    MASTERCODE,
    MASTERNAME,
    SECONDCODE,
    SECONDNAME,
    FOB,
    QTY,
    MONEY,
    PRICE
FROM QCM_CUSTOMER_RETURN_M WHERE 1=1 {where} order by id desc";

                DataTable dt = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "",paramTestDic);
                int total = CommonBASE.GetPageDataTableCount(DB, sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                dic.Add("rowCount", total);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "Query failed！" + ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 导出dt
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetReturnDatalist_dc(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string sql = string.Empty;
                string where = string.Empty;
                DateTime currDate = DateTime.Now;
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);

                string REGION = jarr.ContainsKey("REGION") ? jarr["REGION"].ToString() : "";//

                string FACTORY_NO = jarr.ContainsKey("FACTORY_NO") ? jarr["FACTORY_NO"].ToString() : "";//
                string FACTORY_NAME = jarr.ContainsKey("FACTORY_NAME") ? jarr["FACTORY_NAME"].ToString() : "";//

                string SALESORGAN_NO = jarr.ContainsKey("SALESORGAN_NO") ? jarr["SALESORGAN_NO"].ToString() : "";//
                string SALESORGAN_NAME = jarr.ContainsKey("SALESORGAN_NAME") ? jarr["SALESORGAN_NAME"].ToString() : "";//

                string ARTICLE = jarr.ContainsKey("ARTICLE") ? jarr["ARTICLE"].ToString() : "";//
                string SHOES_NAME = jarr.ContainsKey("SHOES_NAME") ? jarr["SHOES_NAME"].ToString() : "";//

                string MASTERCODE = jarr.ContainsKey("MASTERCODE") ? jarr["MASTERCODE"].ToString() : "";//
                string MASTERNAME = jarr.ContainsKey("MASTERNAME") ? jarr["MASTERNAME"].ToString() : "";//

                string SECONDCODE = jarr.ContainsKey("SECONDCODE") ? jarr["SECONDCODE"].ToString() : "";//
                string SECONDNAME = jarr.ContainsKey("SECONDNAME") ? jarr["SECONDNAME"].ToString() : "";//

                string starttime = jarr.ContainsKey("starttime") ? jarr["starttime"].ToString() : "";//
                string endtime = jarr.ContainsKey("endtime") ? jarr["endtime"].ToString() : "";//


                #region 条件
                Dictionary<string, object> paramTestDic = new Dictionary<string, object>();
                if (!string.IsNullOrEmpty(REGION))
                {
                    where += " and REGION like @REGION";
                    paramTestDic.Add("REGION", $@"%{REGION}%");
                }
                if (!string.IsNullOrEmpty(FACTORY_NO))
                {
                    where += " and FACTORY_NO like @FACTORY_NO";
                    paramTestDic.Add("FACTORY_NO", $@"%{FACTORY_NO}%");
                }
                if (!string.IsNullOrEmpty(FACTORY_NAME))
                {
                    where += " and FACTORY_NAME like @FACTORY_NAME";
                    paramTestDic.Add("FACTORY_NAME", $@"%{FACTORY_NAME}%");
                }

                if (!string.IsNullOrEmpty(SALESORGAN_NO))
                {
                    where += " and SALESORGAN_NO like @SALESORGAN_NO";
                    paramTestDic.Add("SALESORGAN_NO", $@"%{SALESORGAN_NO}%");
                }
                if (!string.IsNullOrEmpty(SALESORGAN_NAME))
                {
                    where += " and SALESORGAN_NAME like @SALESORGAN_NAME";
                    paramTestDic.Add("SALESORGAN_NAME", $@"%{SALESORGAN_NAME}%");
                }

                if (!string.IsNullOrEmpty(ARTICLE))
                {
                    where += " and ARTICLE like @ARTICLE";
                    paramTestDic.Add("ARTICLE", $@"%{ARTICLE}%");
                }

                if (!string.IsNullOrEmpty(SHOES_NAME))
                {
                    where += " and SHOES_NAME like @SHOES_NAME";
                    paramTestDic.Add("SHOES_NAME", $@"%{SHOES_NAME}%");
                }

                if (!string.IsNullOrEmpty(MASTERCODE))
                {
                    where += " and MASTERCODE like @MASTERCODE";
                    paramTestDic.Add("MASTERCODE", $@"%{MASTERCODE}%");
                }

                if (!string.IsNullOrEmpty(MASTERNAME))
                {
                    where += " and MASTERNAME like @MASTERNAME";
                    paramTestDic.Add("MASTERNAME", $@"%{MASTERNAME}%");
                }

                if (!string.IsNullOrEmpty(SECONDCODE))
                {
                    where += " and SECONDCODE like @SECONDCODE";
                    paramTestDic.Add("SECONDCODE", $@"%{SECONDCODE}%");
                }
                if (!string.IsNullOrEmpty(SECONDNAME))
                {
                    where += " and SECONDNAME like @SECONDNAME";
                    paramTestDic.Add("SECONDNAME", $@"%{SECONDNAME}%");
                }
                if (!string.IsNullOrEmpty(starttime) && !string.IsNullOrEmpty(endtime))
                {
                    //where += $@"AND ( PRODUCTION_DATE BETWEEN '{starttime}' AND '{endtime}') ";
                    where += $@"and TO_CHAR( PRODUCTION_DATE ,'yyyy-MM-dd') BETWEEN @start_date AND @end_time";
                    paramTestDic.Add("start_date", $@"{starttime}");
                    paramTestDic.Add("end_time", $@"{endtime}");
                }

                #endregion
                sql = $@"

SELECT 
    RETURN_MONTH,
    REGION,
    FACTORY_NO,
    FACTORY_NAME,
    SALESORGAN_NO,
    SALESORGAN_NAME,
    ARTICLE,
    SHOES_NAME,
    TO_CHAR (PRODUCTION_DATE,'yyyy-MM-dd') PRODUCTION_DATE,
    MASTERCODE,
    SECONDCODE,
    MASTERNAME,
    SECONDNAME,
    FOB,
    QTY,
    MONEY,
    PRICE
FROM QCM_CUSTOMER_RETURN_M WHERE 1=1 {where} order by id desc";

                DataTable dt = DB.GetDataTable( sql, paramTestDic);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("data", dt);
                ret.IsSuccess = true;
                ret.RetData = JsonConvert.SerializeObject(dic);

            }
            catch (Exception ex)
            {

                ret.IsSuccess = false;
                ret.ErrMsg = "Query failed！" + ex.Message;
            }

            return ret;
        }
        /// <summary>
        /// 删除列表数据【客户退货】
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Main_Delete(object OBJ)
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

                string ID = jarr.ContainsKey("ID") ? jarr["ID"].ToString() : "";//表头id
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art
                string userCode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                DateTime datetime = DateTime.Now;
                string date = datetime.ToString("yyyy-MM-dd");
                string time = datetime.ToString("HH:mm:ss");

                string sql = $@"delete from QCM_CUSTOMER_RETURN_M where id='{ID}'";
                DB.ExecuteNonQuery(sql);

                ret.IsSuccess = true;
                ret.ErrMsg = "successfully deleted";
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

        #endregion
        public class listdic
        {

            /// <summary>
            /// 项目集合
            /// </summary>
            public List<qcm_iqc_insp_res_bad_report> listdic2 = new List<qcm_iqc_insp_res_bad_report>();

        }

        public class qcm_iqc_insp_res_bad_report
        {
            /// <summary>
            /// ID
            /// </summary>
            public string id { get; set; }
            /// <summary>
            /// 收料单号
            /// </summary>
            public string chk_no { get; set; }
            /// <summary>
            /// 结案状态
            /// </summary>
            public string closing_status { get; set; }
            /// <summary>
            /// 进仓数量
            /// </summary>
            public string warehousing_qty { get; set; }
            /// <summary>
            /// 不良数量
            /// </summary>
            public string bad_qty { get; set; }
            /// <summary>
            /// 不良率
            /// </summary>
            public string bad_rate { get; set; }
            /// <summary>
            /// 特采数量
            /// </summary>
            public string spc_mining { get; set; }
            /// <summary>
            /// 实退数量
            /// </summary>
            public string actual_returned_qty { get; set; }
            /// <summary>
            /// 补送数量
            /// </summary>
            public string supplementary_delivery_qty { get; set; }
            /// <summary>
            /// 检验报告/不合格说明
            /// </summary>
            public string insp_report { get; set; }
            /// <summary>
            /// 退货原因
            /// </summary>
            public string return_reason { get; set; }
            /// <summary>
            /// 厂商回复
            /// </summary>
            public string manufacturer_reply { get; set; }
            /// <summary>
            /// 图片集合
            /// </summary>
            public List<imginfo> image_list { get; set; }
        }


        /// <summary>
        /// 签名人
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_Autograph(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//料号
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//料号
                string where = string.Empty;
                string sql = string.Empty;
                sql = $@"
                SELECT
                a.ID,
                a.CHK_NO,
                a.DEPARTMENT,
                a.CONFIRM_BY,
				b.STAFF_NAME,
                a.CREATEDATE,
                a.CREATETIME,
                a.MODIFYDATE,
                a.MODIFYTIME,
                a.ISDELETE,
                a.DELETEDATE,
                a.DELETETIME
                FROM
                 QCM_IQC_INSP_RES_BAD_R_A a LEFT JOIN HR001M b on a.CONFIRM_BY=b.staff_no WHERE a.CHK_NO='{CHK_NO}'AND a.ITEM_NO='{ITEM_NO}' AND a.CHK_SEQ='{CHK_SEQ}' and a.isdelete='0'";
                DataTable dt = DB.GetDataTable(sql);
                ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
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

        /// <summary>
        /// 签名人
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_AutographAdd(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//部门
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//部门
                string USERCODES = jarr.ContainsKey("USERCODE") ? jarr["USERCODE"].ToString() : "";//检验员判定
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//料号
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CreateData = DateTime.Now.ToString("yyyy-MM-dd");
                string CreateTime = DateTime.Now.ToString("HH:mm:ss");

                Dictionary<string, object> p_QCM_IQC_INSP_RES_BAD_R_A = new Dictionary<string, object>();
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CHK_NO", CHK_NO);//收料单号
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("DEPARTMENT", DEPARTMENT);//部门
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CREATEBY", usercode);
                if (DEPARTMENT == "3" && !string.IsNullOrWhiteSpace(USERCODES))
                {
                    usercode = USERCODES;//检验员
                }
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CONFIRM_BY", usercode);//签名人
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CREATEDATE", CreateData);
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CREATETIME", CreateTime);
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("ISDELETE", "0");
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("ITEM_NO", ITEM_NO);
                p_QCM_IQC_INSP_RES_BAD_R_A.Add("CHK_SEQ", CHK_SEQ);

                // premika added code 2025-09-20----start
               
               ValidateNoDuplicateSign(DB, CHK_NO, ITEM_NO, DEPARTMENT, usercode);
                ValidatePreviousDepartmentSigned(DB, CHK_NO, ITEM_NO, int.Parse(DEPARTMENT));//2025-09-22

                // premika added code 2025-09-20----End


                DB.ExecuteNonQuery(
                SJeMES_Framework_NETCore.Common.StringHelper.GetInsertSqlByDictionary("oracle", "QCM_IQC_INSP_RES_BAD_R_A", p_QCM_IQC_INSP_RES_BAD_R_A),
                p_QCM_IQC_INSP_RES_BAD_R_A);
                ret.IsSuccess = true;
                DB.Commit();
                // premika added code 2025-09-22----start
                //if (ret.IsSuccess)
                //{
                //    DataTable Result = SendMailToNxtDepartment(DB, CHK_NO, ITEM_NO, DEPARTMENT, usercode);

                //    foreach (DataRow dr in Result.Rows)
                //    {
                //        if (dr["Status"].ToString() != "Message sent")
                //        {
                //            throw new Exception("Error Found at Mail send");
                //        }
                //    }
                //}
                //whatsapp alert--2025-12-20
                if (ret.IsSuccess)
                {

                    var (groupId, phoneNumbers) =
                     GetDeptContacts(DB, DEPARTMENT);

                    string msg = BuildMessage(DB,
                        CHK_NO,
                        ITEM_NO,
                        usercode,
                        DEPARTMENT
                    );

                    if (phoneNumbers == null || phoneNumbers.Count == 0)
                    {
                        if (DEPARTMENT!="0")
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = "No phone numbers found.";
                        }
                    }
                    bool sent = SendWhatsAppGroupMessage(msg, groupId, phoneNumbers);
                    if (!sent)
                    {
                        throw new Exception("WhatsApp message sending failed");
                    }
                    else
                    {
                        ret.IsSuccess = true;
                    }
                   

                }


                //DataTable Result = SendMailToNxtDepartment(DB, CHK_NO, ITEM_NO, DEPARTMENT, usercode);
                //foreach (DataRow dr in Result.Rows)
                //{
                //    if (dr["Status"].ToString() != "Message sent")
                //    {
                //        throw new Exception("Error Found at Mail send");
                //    }

                //}

                // premika added code 2025-09-22----End
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

        //Premika whatsapp and mail alerts --2025-12-20
        public static (string groupId, List<string> phoneNumbers) GetDeptContacts(
SJeMES_Framework_NETCore.DBHelper.DataBase DB,
string Department)
        {

            string ApprovalOrder = @"
             SELECT APPROVAL_ORDER
        FROM TBL_AEQS_CLAIMS_WHATSAPP_ALETS
        WHERE DEPT_CODE = :DEPT_CODE";


            var Params = new Dictionary<string, object>
        {
            { "DEPT_CODE", Department}

        };
            DataTable dts = DB.GetDataTable(ApprovalOrder, Params);
            string Order = dts.Rows[0]["APPROVAL_ORDER"].ToString();
            int approvalOrders1 = int.Parse(Order) + 1;
            string sqlPrev = @"
             SELECT group_id, phone_number
        FROM TBL_AEQS_CLAIMS_WHATSAPP_ALETS
        WHERE approval_order = :approvalOrder";


            var prevParams = new Dictionary<string, object>
        {
          
            { "approvalOrder", approvalOrders1}

        };
            DataTable dt = DB.GetDataTable(sqlPrev, prevParams);
            if (dt.Rows.Count == 0)
                return (string.Empty, new List<string>());

            // Get group id (same for all rows)
            string groupId = dt.Rows[0]["group_id"].ToString();

            List<string> phoneNumbers = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string phone = row["phone_number"]?.ToString();

                if (!string.IsNullOrWhiteSpace(phone))
                {
                    phoneNumbers.Add(phone);
                }
            }
            return (groupId, phoneNumbers);


        }
        private static string BuildMessage(SJeMES_Framework_NETCore.DBHelper.DataBase DB,
    string CHK_NO,
    string ITEM_NO,
    string usercode,
    string department
    )
        {
          
            string Msg_chk_no = "";
            string Msg_item_no = "";
            string Msg_confirm_by = "";
            string Msg_staff_name = "";
            string Msg_createdate = "";
            string Msg_createtime = "";
            string Msg_item_name = "";

            string MsgData = @"
               select yt.chk_no,yt.item_no,e.name_t,yt.confirm_by,h.staff_name,yt.createdate,yt.createtime
        from QCM_IQC_INSP_RES_BAD_R_A yt left join hr001m h on h.staff_no=yt.confirm_by
        left join bdm_rd_item e on e.item_no=yt.item_no
       where yt.chk_no = :CHK_NO
         and yt.item_no =:ITEM_NO 
         and yt.confirm_by =:usercode
         and yt.department =:department";


            var Params = new Dictionary<string, object>
        {
            { "CHK_NO", CHK_NO},
            { "ITEM_NO", ITEM_NO},
             { "usercode",  usercode},
            { "department", department}

        };
            DataTable dts = DB.GetDataTable(MsgData, Params);
            if (dts.Rows.Count == 0)
             { 
            throw new Exception("No WhatsApp contacts found");
            }
            else
            {
                 Msg_chk_no = dts.Rows[0]["chk_no"].ToString();
                Msg_item_no = dts.Rows[0]["item_no"].ToString();
                Msg_item_name = dts.Rows[0]["name_t"].ToString();
                Msg_confirm_by = dts.Rows[0]["confirm_by"].ToString();
                Msg_staff_name = dts.Rows[0]["staff_name"].ToString();
                 Msg_createdate = dts.Rows[0]["createdate"].ToString();
                 Msg_createtime = dts.Rows[0]["createtime"].ToString();
              

            }
            Dictionary<string, string> depts = new Dictionary<string, string>()
                {
                    { "3", "Inspector" },
                    { "2", "QIP Raw Material Department" },
                    { "1", "Warehouse Department" },
                     { "4", "Business Department" },
                      { "0", "QIP Incharge" }
                };
            string Msg_dept = depts[department]; 

            return
        $@"
{Msg_dept} signed Sucessfully.Kindly Review and Confirm Your Signature ASAP. 🤝

Signature Details:
------------------
Receipt No:*{Msg_chk_no}*
Material Code:*{Msg_item_no}*
Material Name:*{Msg_item_name}*
Signed By:*{Msg_staff_name}*
Barcode:*{Msg_confirm_by}*
Signed Department:*{Msg_dept}*
Date:*{Msg_createdate}*
Time:*{Msg_createtime}*
";
        }

        public static bool SendWhatsAppGroupMessage(
    string message,
    string groupId, List<String> phoneNumbers)
        {
            string phNumbers = phoneNumbers[0];

            string[] MobileNumbers = phNumbers
    .Split(',', StringSplitOptions.RemoveEmptyEntries)
    .Select(x => x.Trim())
    .ToArray();

            var payload = new
            {
                  
               tagNumbers = MobileNumbers.ToArray(),
            groups = new[] { groupId },
                textMsg = message,
                mediaurl = "",
                filename = ""
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            var result = SendMessageAsync(jsonPayload).GetAwaiter().GetResult();

            return result.success;
        }
        private static async Task<(bool success, string message)> SendMessageAsync(string jsonPayload)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                   
                    string url = "http://10.3.0.208:9090/whatsapp/WhatsappApi/TagPersonsInGroup";
                    var response = await httpClient.PostAsync(url, content);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    JArray parsedMessage = JArray.Parse(responseBody);

                    string retdata = "Fail";

                    foreach (var item in parsedMessage)
                    {
                        string sendStatus = item["sendStatus"]?.ToString();

                        if (sendStatus != "Failed")
                        {
                            retdata = "Success";
                            break;
                        }
                    }

                    if (response.IsSuccessStatusCode)
                    {
                        return (true, retdata);
                    }
                    else
                    {
                        return (false, $"Failed with status {response.StatusCode}: {responseBody}");
                    }
                }
                catch (Exception ex)
                {
                    return (false, $"Exception: {ex.Message}");
                }
            }
        }

        // premika added code 2025-10-14----End










        // premika added code 2025-09-20----start
        private static void ValidateNoDuplicateSign(SJeMES_Framework_NETCore.DBHelper.DataBase DB,
                                            string chkNo, string itemNo, string department, string userCode)
        {
           
                string sqlPrev = @"
            SELECT f.confirm_by, f.department
            FROM QCM_IQC_INSP_RES_BAD_R_A f
            WHERE f.chk_no = :CHK_NO
              AND f.item_no = :ITEM_NO
              AND f.department <> :DEPARTMENT
              AND f.isdelete = 0
            ORDER BY CAST(f.department AS INT) DESC";

                
                var prevParams = new Dictionary<string, object>
        {
            { "CHK_NO", chkNo },
            { "ITEM_NO", itemNo },
            { "DEPARTMENT", department }
        };

               
                DataTable prevRecord = DB.GetDataTable(sqlPrev, prevParams);

            
                foreach (DataRow row in prevRecord.Rows)
                {
                    string prevUser = row["confirm_by"].ToString();
                    string prevDept = row["department"].ToString();

                    if (prevUser == userCode)
                    {
                        throw new Exception("Cannot sign in Other Department");
                    }
                }
            
        }

        // premika added code 2025-09-20----END
        // premika added code 2025-09-22----start
        private static void ValidatePreviousDepartmentSigned(
    SJeMES_Framework_NETCore.DBHelper.DataBase DB,
    string chkNo,
    string itemNo,
    int currentDept)
        {
            // Define signing order (from first to last)
            List<int> departmentFlow = new List<int> { 3, 2, 1, 4, 0 };//flow:3-inspector,2-qip supervisor,1-warehouse,4-business,0-Qip section Head 

            int currentIndex = departmentFlow.IndexOf(currentDept);

            // If department not found or it's the first signer, no validation needed
            if (currentIndex <= 0)
                return;

            int prevDept = departmentFlow[currentIndex - 1];

            string sql = @"
        SELECT COUNT(*)
        FROM QCM_IQC_INSP_RES_BAD_R_A
        WHERE CHK_NO = :CHK_NO
          AND ITEM_NO = :ITEM_NO
          AND DEPARTMENT = :PrevDept
          AND ISDELETE = 0";

            var parameters = new Dictionary<string, object>
    {
        { "CHK_NO", chkNo },
        { "ITEM_NO", itemNo },
        { "PrevDept", prevDept }
    };

            int countPrev = Convert.ToInt32(DB.ExecuteScalar(sql, parameters));

            if (countPrev == 0)
            {
                throw new Exception($"Previous department not signed");
            }
        }

       


        // premika added code 2025-09-22----END


        /// <summary>
        /// 签名人
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetBad_AutographDelete(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string DEPARTMENT = jarr.ContainsKey("DEPARTMENT") ? jarr["DEPARTMENT"].ToString() : "";//部门
                string CHK_SEQ = jarr.ContainsKey("CHK_SEQ") ? jarr["CHK_SEQ"].ToString() : "";//料号
                string usercode = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);
                string CreateData = DateTime.Now.ToString("yyyy-MM-dd");
                string CreateTime = DateTime.Now.ToString("HH:mm:ss");
                string ITEM_NO= jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";
                //DB.ExecuteNonQuery($@"Update QCM_IQC_INSP_RES_BAD_R_A set ISDELETE='1',DELETEDATE='{CreateData}',DELETEBY='{usercode}',DELETETIME='{CreateTime}' WHERE CHK_NO='{CHK_NO}' AND DEPARTMENT='{DEPARTMENT}' AND ISDELETE='0' AND ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'");


                // premika added code 2025-09-19----start

                if (!IsAuthorizedToCancel(DB, CHK_NO, DEPARTMENT, ITEM_NO, usercode))
                {
                    throw new Exception("Not allow to cancel this signature");
                }
                // premika added code 2025-09-19----End

                string sql = $"delete from QCM_IQC_INSP_RES_BAD_R_A WHERE CHK_NO='{CHK_NO}' AND DEPARTMENT='{DEPARTMENT}'  AND ITEM_NO='{ITEM_NO}' AND CHK_SEQ='{CHK_SEQ}'";//硬删
                DB.ExecuteNonQuery(sql);
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
        // premika added code 2025-09-19----start
        private static bool IsAuthorizedToCancel(
    SJeMES_Framework_NETCore.DBHelper.DataBase DB,
    string chkNo,
    string department,
    string itemNo,
    string currentUser)
        {
            string sqlBase = @"
        SELECT COUNT(*)
        FROM QCM_IQC_INSP_RES_BAD_R_A
        WHERE CHK_NO = @CHK_NO
          AND ITEM_NO = @ITEM_NO
          AND DEPARTMENT = @DEPARTMENT
          AND ISDELETE = 0
    ";

            string sqlCondition;

            switch (department)
            {
                case "3": // Inspector
                    sqlCondition = @"
                AND (
                        CONFIRM_BY = @CURRENT_USER
                        OR CREATEBY = @CURRENT_USER
                    )
            ";
                    break;

                case "4": // Business (NEW)
                case "2": // QIP Supervisor
                case "1": // Warehouse
                case "0": // QIP Section Head
                default:
                    sqlCondition = @"
                AND CONFIRM_BY = @CURRENT_USER
                AND CREATEBY = @CURRENT_USER
            ";
                    break;
            }

            string sql = sqlBase + sqlCondition;

            var parameters = new Dictionary<string, object>
    {
        { "CHK_NO", chkNo },
        { "ITEM_NO", itemNo },
        { "DEPARTMENT", department },
        { "CURRENT_USER", currentUser }
    };

            object result = DB.ExecuteScalar(sql, parameters);
            int approveCounts = result == null ? 0 : Convert.ToInt32(result);

            return approveCounts > 0;
        }

        // premika added code 2025-09-19----End



        // premika added code 2025-09-22----start


        //private static DataTable SendMailToNxtDepartment(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string CHK_NO, string ITEM_NO, string DEPARTMENT, string usercode)
        //{

        //    List<int> DepartmentFlow = new List<int> { 3, 2, 1, 4, 0 };

        //    // 2️⃣ Calculate next department safely
        //    int currentDept = int.Parse(DEPARTMENT);
        //    int currentIndex = DepartmentFlow.IndexOf(currentDept);

        //    if (currentIndex == -1 || currentIndex == DepartmentFlow.Count - 1)
        //    {
        //        return null; // last approver → no email
        //    }

        //    string NxtDept = DepartmentFlow[currentIndex + 1].ToString();

        //    string subject = "";
        //   // int PrevDept = int.Parse(DEPARTMENT) - 1;
        //   // string NxtDept = PrevDept.ToString();
        //    string CurrentDeptName = "";
        //    string SignedUserName = "";
        //    string SignedDate = "";
        //    string SignedTime = "";
        //    string SignedUser = "";



        //    string sql = @"   Select 
        //       z.confirm_by,
        //       v.staff_name,
        //       z.createdate,
        //       z.createtime,
        //       y.department
        //  from QCM_IQC_INSP_RES_BAD_R_A z
        //  left join TBL_AEQS_MAIL_CONFIG_ALERTS y
        //    on y.dept = z.department
        //    left join hr001m v on v.staff_no=z.confirm_by
        //     WHERE 1=1 ";

        //    if (!string.IsNullOrEmpty(DEPARTMENT))
        //    {
        //        sql += $@" AND z.DEPARTMENT = '{DEPARTMENT.Trim()}' ";
        //    }
        //    if (!string.IsNullOrEmpty(CHK_NO))
        //    {
        //        sql += $@" AND z.CHK_NO = '{CHK_NO.Trim()}' ";
        //    }
        //    if (!string.IsNullOrEmpty(ITEM_NO))
        //    {
        //        sql += $@" AND z.ITEM_NO = '{ITEM_NO.Trim()}' ";
        //    }
        //    sql += $@"AND z.ISDELETE = 0";

        //    DataTable dt1 = DB.GetDataTable(sql);

        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        CurrentDeptName = dr["DEPARTMENT"].ToString();
        //    }
        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        SignedUserName = dr["staff_name"].ToString();
        //    }
        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        SignedDate = dr["createdate"].ToString();
        //    }
        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        SignedTime = dr["createtime"].ToString();
        //    }
        //    foreach (DataRow dr in dt1.Rows)
        //    {
        //        SignedUser = dr["confirm_by"].ToString();
        //    }
        //    string sql1 = @"SELECT  TO_LIST, CC_LIST, ERROR_LIST, MAIL_SUBJECT FROM Tbl_Aeqs_Mail_Config_Alerts where 1=1 ";
        //    if (!string.IsNullOrEmpty(NxtDept))
        //    {
        //        sql1 += $@"and DEPT='{NxtDept}'";
        //    }
        //    DataTable dt = DB.GetDataTable(sql1);

        //    List<string> recipientEmails = new List<string>();
        //    List<string> ccEmails = new List<string>();


        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        subject = dr["MAIL_SUBJECT"].ToString();
        //    }


        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        string recipientEmail = dr["TO_LIST"].ToString();
        //        if (!string.IsNullOrEmpty(recipientEmail))
        //        {
        //            recipientEmails.AddRange(recipientEmail.Split(','));
        //        }
        //    }


        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        string ccEmail = dr["CC_LIST"].ToString();
        //        if (!string.IsNullOrEmpty(ccEmail))
        //        {
        //            ccEmails.AddRange(ccEmail.Split(','));
        //        }
        //    }

        //    string body = $@"
        //<html>
        //  <body style='font-family: Arial, sans-serif; font-size: 14px; line-height: 1.6;'>
        //    <p>Dear ALL,</p>

        //    <p>The Employee signed successfully with the following details:</p>

        //    <table style='border-collapse: collapse; width: 100%; max-width: 400px; font-size: 14px;'>
        //      <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left; '>Check Number</th>
        //        <td style='border: 1px solid #ddd; padding: 8px;  color: #007BFF; font-weight: bold;'>{CHK_NO}</td>
        //      </tr>
        //      <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Material Code</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{ITEM_NO}</td>
        //      </tr>
        //      <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Signed By</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{SignedUserName}</td>
        //      </tr>
        //      <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Barcode</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{SignedUser}</td>
        //      </tr>
        //      <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Department/Position</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{CurrentDeptName}</td>
        //      </tr>
        //         <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Signed Date</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{SignedDate}</td>
        //      </tr>
        //     <tr>
        //        <th style='border: 1px solid #ddd; padding: 8px; text-align: left;'>Signed Time</th>
        //        <td style='border: 1px solid #ddd; padding: 8px; color: #007BFF; font-weight: bold;'>{SignedTime}</td>
        //      </tr>
        //    </table>

        //    <p style='margin-top: 20px;'>
        //     The department has signed off. Please proceed with your Nextsteps as soon as possible.
        //    </p>

        //    <p>Thank you,</p>
        //    <p><strong>System Alert</strong></p>
        //  </body>
        //</html>";

        //    using (var client = new SmtpClient())
        //    {
        //        string userEmailAddress = "IT-announcement@in.apachefootwear.com";
        //        string userName = "Remainder Mail";
        //        string password = "it-123456";
        //        // string host = "10.3.0.254"; // Mail server
        //        string host = "apcmx1.apachefootwear.com";
        //        int port = 25;


        //        DataTable resultTable = new DataTable();
        //        resultTable.Columns.Add("Status", typeof(string));

        //        try
        //        {
        //            MailMessage msg = new MailMessage();
        //            msg.From = new MailAddress(userEmailAddress, userName);
        //            msg.Subject = subject;
        //            msg.Body = body;
        //            msg.BodyEncoding = Encoding.UTF8;
        //            msg.IsBodyHtml = true;
        //            msg.Priority = MailPriority.High;

        //            // Add recipient emails
        //            foreach (string email in recipientEmails)
        //            {
        //                msg.To.Add(new MailAddress(email.Trim()));
        //            }

        //            // Add CC emails
        //            foreach (string email in ccEmails)
        //            {
        //                msg.CC.Add(new MailAddress(email.Trim()));
        //            }

        //            client.Host = host;
        //            client.Port = port;
        //            client.UseDefaultCredentials = false;
        //            client.EnableSsl = false;
        //            client.Credentials = new NetworkCredential(userEmailAddress, password);
        //            client.DeliveryMethod = SmtpDeliveryMethod.Network;



        //            client.Send(msg);
        //            resultTable.Rows.Add("Message sent");
        //        }
        //        catch (SmtpException ex)
        //        {
        //            resultTable.Rows.Add("Message failed: " + ex.Message);
        //        }

        //        return resultTable;
        //    }
        //}
        // premika added code 2025-09-22----End


  

        public class imginfo
        {
            public string id { get; set; }
            public string guid { get; set; }
            public string image_url { get; set; }
        }
    }
}
