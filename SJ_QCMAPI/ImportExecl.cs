using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_QCMAPI
{
    public class ImportExecl
    {
        /// <summary>
        /// 量产试作导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_batch_production(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                string flag = DB.GetString($"select nvl(status,'-1') from qcm_batch_production_m where batch_code='{dr["量试编号"]}'");

                if (!string.IsNullOrEmpty(flag))
                {
                    if (flag != "1" && flag != "2")
                    {
                        DB.ExecuteNonQuery($@"update qcm_batch_production_m set
                                            DEVELOP_QUARTER='{dr["开发季度"]}',
                                            TYPE='{dr["类别"]}',
                                            ART='{dr["ART"]}',
                                            BATCH_DATE='{dr["量试日期"]}',
                                            PRODUCTION_DATE='{dr["生产日期"]}',
                                            SHOE_NAME='{dr["鞋型名称"]}',
                                            BIG_MOLD_NO='{dr["大底模号"]}',
                                            SIZE_DOUBLE='{dr["试作SIZE、双数"]}',
                                            COLOR='{dr["配色"]}',
                                            DEPARTMENT='{dr["执行部门"]}',
                                            PROCEDURE='{dr["工艺"]}',
                                            SHOE_LAST='{dr["楦头"]}',
                                            MODIFYBY='{usercode}',
                                            MODIFYDATE='{date}',
                                            MODIFYTIME='{time}'
                                            where batch_code='{dr["量试编号"]}'");
                    }
                }
                else
                {
                    DB.ExecuteNonQuery($@"insert into qcm_batch_production_m(
                                                BATCH_CODE,
                                                DEVELOP_QUARTER,
                                                TYPE,
                                                ART,
                                                BATCH_DATE,
                                                PRODUCTION_DATE,
                                                SHOE_NAME,
                                                BIG_MOLD_NO,
                                                SIZE_DOUBLE,
                                                COLOR,
                                                DEPARTMENT,
                                                PROCEDURE,
                                                SHOE_LAST,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME,
                                                STATUS
                                                )
                                         values(
                                                '{dr["量试编号"]}',
                                                '{dr["开发季度"]}',
                                                '{dr["类别"]}',
                                                '{dr["ART"]}',
                                                '{dr["量试日期"]}',
                                                '{dr["生产日期"]}',
                                                '{dr["鞋型名称"]}',
                                                '{dr["大底模号"]}',
                                                '{dr["试作SIZE、双数"]}',
                                                '{dr["配色"]}',
                                                '{dr["执行部门"]}',
                                                '{dr["工艺"]}',
                                                '{dr["楦头"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}',
                                                '{0}'
                                                )
                                ");
                }
            }
        }


        /// <summary>
        /// 客户投诉导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_Customer(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {

                DB.ExecuteNonQuery($@"insert into QCM_CUSTOMER_COMPLAINT_M(
                                                COMPLAINT_NO,
                                                COMPLAINT_DATE,
                                                COUNTRY_REGION,
                                                PO_ORDER,
                                                DEVELOP_SEASON,
                                                CATEGORY,
                                                DEVELOPMENT_COURSE,
                                                PRODUCT_MONTH,
                                                PROD_NO,
                                                SHOE_NO,
                                                MATERIAL_WAY,
                                                PRODUCTIONLINE_NO,
                                                PRODUCTIONLINE_NAME,
                                                NG_QTY,
                                                COMPLAINT_MONEY,
                                                DEFECT_CONTENT,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                                )
                                         values(
                                                '{dr["投诉编号"]}',
                                                '{dr["投诉日期"]}',
                                                '{dr["国家区域"]}',
                                                '{dr["PO"]}',
                                                '{dr["开发季度"]}',
                                                '{dr["CATEGORY"]}',
                                                '{dr["开发课"]}',
                                                '{dr["量产月份"]}',
                                                '{dr["ART"]}',
                                                '{dr["鞋型"]}',
                                                '{dr["MATERIAL_WAY"]}',
                                                '{dr["产线代号"]}',
                                                '{dr["产线名称"]}',
                                                '{dr["不良数量"]}',
                                                '{dr["投诉金额"]}',
                                                '{dr["问题点"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )
                                ");

            }
        }

        /// <summary>
        /// 客户投诉导入2022-06-06新
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_CustomerEx(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                //string datet = dr["投诉日期"].ToString();
                string datet = dr["Complaint date"].ToString();
                try
                {
                   // datet =Convert.ToDateTime(dr["投诉日期"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") ;
                    datet =Convert.ToDateTime(dr["Complaint date"].ToString()).ToString("yyyy/MM/dd HH:mm:ss") ;
                }
                catch(Exception ex)
                {
                   // datet = Convert.ToDateTime(dr["投诉日期"].ToString()).ToString("yyyy/MM/dd") + " 00:00:00";
                }

                string po = "";
               // if (dr["投诉PO号"] != null)
                if (dr["Complaint PO number"] != null)
                   // po = dr["投诉PO号"].ToString();
                    po = dr["Complaint PO number"].ToString();

                DB.ExecuteNonQuery($@"insert into QCM_CUSTOMER_COMPLAINT_M(
                                                COMPLAINT_NO,
                                                COMPLAINT_DATE,
                                                COUNTRY_REGION,
                                                PO_ORDER,
                                                NG_QTY,
                                                COMPLAINT_MONEY,
                                                DEFECT_CONTENT,
                                                FOB,
                                                STATUS,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                                )
                                         values(
                                                '{dr["Complaint No"]}',                                             
                                                to_date('{datet}','yyyy-mm-dd hh24:mi:ss'),
                                                '{dr["country/area"]}',
                                                '{po}',
                                                '{dr["a poor amount"]}',
                                                '{dr["Complaint amount"]}',
                                                '{dr["Problems"]}',
                                                '{dr["FOB"]}',
                                                '0',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )
                                ");

            }
        }

        /// <summary>
        /// 确认鞋库位维护导入2022-06-07新
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_ShoesArc(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                string WAREHOUSE_NAME = DB.GetString($@"select WAREHOUSE_NAME from qcm_confirm_shoes_wh where WAREHOUSE_CODE='{dr["仓库代号"]}'");
                DB.ExecuteNonQuery($@"insert into qcm_confirm_shoes_arc (STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,remark,
                                    ref_standard,expire_day,remind_day,createby,createdate,createtime) 
                                    values('{dr["库位代号"]}','{dr["库位名称"]}','{dr["仓库代号"]}','{WAREHOUSE_NAME}',
                                    '{dr["备注"]}','{dr["参照标准"]}','{dr["到期时间"]}','{dr["提醒时间"]}','{usercode}',
                                    '{date}','{time}')");

            }
        }

        /// <summary>
        /// 确认鞋仓库维护导入2022-06-07新
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_ShoesWh(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                DB.ExecuteNonQuery($@"insert into qcm_confirm_shoes_wh (WAREHOUSE_CODE,WAREHOUSE_NAME,createby,createdate,createtime) 
                                    values('{dr["仓库代号"]}','{dr["仓库名称"]}','{usercode}','{date}','{time}') ");

            }
        }

        /// <summary>
        /// 出货仓库维护导入2022-06-08新
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_ShoesWh_AQL(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                DB.ExecuteNonQuery($@"insert into aql_confirm_shoes_wh (MODULE_TYPE,WAREHOUSE_CODE,WAREHOUSE_NAME,createby,createdate,createtime) 
                                    values('{dt.Rows[0]["MODULE_TYPE"].ToString()}','{dr["Warehouse_Code"]}','{dr["Warehouse_Name"]}','{usercode}','{date}','{time}') ");

            }
        }

        /// <summary>
        /// 出货库位维护导入2022-06-08新
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_ShoesArc_AQL(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {
                string WAREHOUSE_NAME = DB.GetString($@"select WAREHOUSE_NAME from aql_confirm_shoes_wh where WAREHOUSE_CODE='{dr["仓库代号"]}'");
                DB.ExecuteNonQuery($@"insert into aql_confirm_shoes_arc (MODULE_TYPE,STOCK_CODE,STOCK_NAME,WAREHOUSE_CODE,WAREHOUSE_NAME,remark,
                                    createby,createdate,createtime) 
                                    values('{dt.Rows[0]["MODULE_TYPE"].ToString()}','{dr["Location_Code"]}','{dr["Location_Name"]}','{dr["Warehouse_Code"]}','{WAREHOUSE_NAME}',
                                    '{dr["备注"]}','{usercode}',
                                    '{date}','{time}')");

            }
        }

        /// <summary>
        /// 首件确认导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Input_REINSPECTION(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            foreach (DataRow dr in dt.Rows)
            {
                string INSPECT_NO = DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                string sql = $@"select max(INSPECT_NO) from qcm_firstarticle_confirm_m where INSPECT_NO like '{INSPECT_NO}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询检验单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(INSPECT_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    INSPECT_NO += int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    INSPECT_NO += "00001";
                }
                string sql2 = $@"insert into qcm_firstarticle_confirm_m(
                                                INSPECT_NO,
                                                PO_ORDER,
                                                PROD_NO,
                                                SHOE_NO,
                                                MODULE_NO,
                                                PHYSICAL_NAME,
                                                MACHINE,
                                                CODE_NUMBER,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                                )
                                               values(
                                                '{INSPECT_NO}',
                                                '{dr["PO单号"]}',
                                                '{dr["ART"]}',
                                                '{dr["鞋型"]}',
                                                '{dr["模号"]}',
                                                '{dr["实物名称"]}',
                                                '{dr["机台"]}',
                                                '{dr["码数"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )";
                DB.ExecuteNonQuery(sql2);

            }
        }
        /// <summary>
        /// 发外厂商品质体系项目日志
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Input_REINSPECTION_REPORT_M(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            foreach (DataRow dr in dt.Rows)
            {
                string OUTSOURCING_INSPECTION_NO = DateTime.Now.ToString("yyyyMMdd");//20211016  00001
                string sql = $@"select max(OUTSOURCING_INSPECTION_NO) from QCM_REINSPECTION_REPORT_M where OUTSOURCING_INSPECTION_NO like '{OUTSOURCING_INSPECTION_NO}%'";
                string max_OUTSOURCING_INSPECTION_NO = DB.GetString(sql);
                //查询外包检验编号有没有相同的
                if (!string.IsNullOrEmpty(max_OUTSOURCING_INSPECTION_NO))
                {
                    string seq = max_OUTSOURCING_INSPECTION_NO.Replace(OUTSOURCING_INSPECTION_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    OUTSOURCING_INSPECTION_NO += int_seq.ToString().PadLeft(5, '0');
                }
                else
                {
                    OUTSOURCING_INSPECTION_NO += "00001";
                }
                string sql2 = $@"insert into QCM_REINSPECTION_REPORT_M(
                                                 OUTSOURCING_INSPECTION_NO,
                                                 SUPPLIERS_CODE,
                                                 SUPPLIERS_NAME,
                                                 SUPPLIERS_TYPE,
                                                 PO_ORDER,
                                                 PROD_NO,
                                                 WH_QTY,
                                                 SPOT_CHECK_QTY,
                                                 BAD_QTY,
                                                 BAD_RATE,
                                                 NOT_ACCEPT_QTY,
                                                 SHOE_NO,
                                                 ACCEPT_QTY,
                                                 CREATEBY,
                                                 CREATEDATE,
                                                 CREATETIME

                                                 )
                                         values(
                                                '{OUTSOURCING_INSPECTION_NO}',
                                                '{dr["厂商代号"]}',
                                                '{dr["厂商名称"]}',
                                                '{dr["厂商类型"]}',
                                                '{dr["制令号"]}',
                                                '{dr["ART"]}',
                                                '{dr["进仓数"]}',
                                                '{dr["抽检数"]}',
                                                '{dr["不良数"]}',
                                                '{dr["不良率"]}',
                                                '{dr["不接受数量"]}',
                                                '{dr["鞋型"]}',
                                                '{dr["接受数量"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )
                                ";
                DB.ExecuteNonQuery(sql2);

            }
        }

        /// <summary>
        /// 发外厂商色卡
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_ExternalColorCard(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {

                DB.ExecuteNonQuery($@"insert into QCM_COLOR_CARD_M(
                                                INSPECT_NO,
                                                CARD_DATE,
                                                VEND_NO,
                                                VEND_NAME,
                                                FIRSTARTICLE_TYPE,
                                                SHOE_NO,
                                                PROD_NO,
                                                PART_NO，
                                                IS_QCCONFIRM,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                                )
                                         values(
                                                '{dr["检验单号"]}',
                                                '{dr["日期"]}',
                                                '{dr["厂商代号"]}',
                                                '{dr["厂商名称"]}',
                                                '{dr["首检确认种类"]}',
                                                '{dr["鞋型"]}',
                                                '{dr["ART"]}',
                                                '{dr["部件"]}',
                                                '{dr["QC确认"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )
                                ");

            }
        }

        /// <summary>
        /// 抽检监督报表导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_Inspection_Supervision_report(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {

                DB.ExecuteNonQuery($@"insert into QCM_SPOTCHECK_TASK_M(
                                                SPOTCHECK_NO,
                                                INSPECT_METHOD,
                                                VEND_NO,
                                                VEND_NAME,
                                                PART_NO,
                                                SHOE_NOS,
                                                PROD_NO,
                                                PO_ORDER，
                                                CODE_NUMBER,
                                                SPOTCHECK_DATE,
                                                PO_QTY,
                                                PLANSAMP_QTY,
                                                PROCESS_TYPE,
                                                NG_QTY,
                                                STATUS,
                                                CREATEBY,
                                                CREATEDATE,
                                                CREATETIME
                                                )
                                         values(
                                                '{dr["检验单号"]}',
                                                '{dr["检验方式"]}',
                                                '{dr["厂商代号"]}',
                                                '{dr["厂商"]}',
                                                '{dr["部件"]}',
                                                '{dr["鞋型名称"]}',
                                                '{dr["Article"]}',
                                                '{dr["PO"]}',
                                                '{dr["码数"]}',
                                                '{dr["检验日期"]}',
                                                '{dr["生产数量(双)"]}',
                                                '{dr["抽检数(双)"]}',
                                                '{dr["工艺类型"]}',
                                                '{dr["总不良数(件)"]}',
                                                '{dr["状态"]}',
                                                '{usercode}',
                                                '{date}',
                                                '{time}'
                                                )
                                ");

            }
        }


        /// <summary>
        /// 不良退货表导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_BadReturn_Report(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            foreach (DataRow dr in dt.Rows)
            {

                DB.ExecuteNonQuery($@"INSERT INTO qcm_bad_return_m ( RETURN_NO, RETURN_DATE, PLANT_AREA, ORDER_QTY, TURNOVER_QTY, B_QTY, RETURN_FREQUENCY, AFFECT_HOURS, SHOE_NO, PROD_NO, CREATEBY, CREATEDATE, CREATETIME )
VALUES
	( '{dr["退货单号"]}', '{dr["退货日期"]}', '{dr["厂区"]}', '{dr["订单数"]}', '{dr["翻箱数（双）"]}', '{dr["B品（只）"]}', '{dr["退库（次）"]}', '{dr["品质影响后段工时"]}', '{dr["鞋型"]}', '{dr["ART"]}', '{usercode}', '{date}', '{time}' )
                                ");
            }
        }
        /// <summary>
        /// 客户退货数据导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_Marketfeedback(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode,Action<bool> rex)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            string sql = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                string po = "";
                if (dr["PO"] != null)
                    po = dr["PO"].ToString();
                //sql = $@"select count(1) from bdm_se_order_master where mer_po='{dr["PO"]}'";
                //if (DB.GetInt32(sql) < 1)
                //{
                //    rex(true);
                //    break;
                //}
                string task_no = "C" + date;
                sql = $@"select max(task_no) from qcm_market_feedback_m where task_no like '{task_no}%'";
                string maxtask_no = DB.GetStringline(sql);
                if (!string.IsNullOrWhiteSpace(maxtask_no))
                {
                    string seq = maxtask_no.Replace(task_no, "");

                    int int_seq = Convert.ToInt32(seq) + 1;

                    task_no += int_seq.ToString().PadLeft(5, '0');
                }
                else
                {
                    task_no += "00001";
                }
                //DateTime 日期 = new DateTime();
                //try
                //{
                //    //string 生产日期_str = dr["生产日期"].ToString();
                //    var 生产日期_list = dr["年份/月份(MM-yyyy)"].ToString().Split('-').Select(x => Convert.ToInt32(x).ToString("00")).ToList();
                //    string 生产日期_str = string.Join('-', 生产日期_list);

                //    //IFormatProvider ifp = new System.Globalization.CultureInfo("zh-CN", true);
                //    //DateTime.Now.ToString("MM/dd/yy");
                //    //DateTime 生产日期 = DateTime.ParseExact(生产日期_str, "MM/dd/yy", ifp);

                //    日期 = DateTime.ParseExact(生产日期_str, "MM-yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal);
                //}
                //catch (Exception ex)
                //{
                //    throw new Exception("生产日期格式错误，请检查！");
                //}
                string thyf_str = "";
                if(!string.IsNullOrWhiteSpace(dr["Return_Month"].ToString()))
                {
                    DateTime is_date;
                    if (!DateTime.TryParse(dr["Return_Month"].ToString(), out is_date))
                    {
                        throw new Exception("Return month format error, please check！");
                    }
                    thyf_str = is_date.ToString("yyyy-MM");
                }

                sql =$@"insert into qcm_market_feedback_m(task_no,po,region_no,size_no,newshoes_qty,oldshoes_qty,main_code,minor_code,fob_price,compensation_amount,problem_point_desc,DATETIME,createby,createdate,createtime,RETURN_MONTH) values('{task_no}','{po}','{dr["Country_Code"]}','{dr["Yardage"]}','{dr["New_Shoes_Quantity"]}','{dr["Old_Shoes_Quantity"]}','{dr["Main_Bad_Code"]}','{dr["Minor_Bad_Code"]}','{dr["FOB_Unit_Price($)"]}','{dr["Amount_Of_Compensation($)"]}','{dr["Problem_Description"]}','','{usercode}','{date}','{time}','{thyf_str}')";
                DB.ExecuteNonQueryOffline(sql);
            }
         
        }
        /// <summary>
        /// 客户退货数据导入
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="dt"></param>
        public static void Import_Marketfeedback2(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode,Action<bool> rex)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd");
            string time = DateTime.Now.ToString("HH:mm:ss");
            string sql = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                sql = $@"select count(1) from bdm_se_order_master where mer_po='{dr["PO"]}'";
                if (DB.GetInt32(sql) < 1)
                {
                    rex(true);
                    break;
                }
                string task_no = "C" + date;
                sql = $@"select max(task_no) from qcm_cust_market_feedback_m where task_no like '{task_no}%'";
                string maxtask_no = DB.GetStringline(sql);
                if (!string.IsNullOrWhiteSpace(maxtask_no))
                {
                    string seq = maxtask_no.Replace(task_no, "");

                    int int_seq = Convert.ToInt32(seq) + 1;

                    task_no += int_seq.ToString().PadLeft(5, '0');
                }
                else
                {
                    task_no += "00001";
                }
              
                sql = $@"insert into qcm_cust_market_feedback_m(task_no,po,region_no,return_qty,main_code,minor_code,fob_price,compensation_amount,remark,createby,createdate,createtime) values('{task_no}','{dr["PO"]}','{dr["国家/地区代号"]}','{dr["退货数量"]}','{dr["主要不良代码"]}','{dr["次要不良代码"]}','{dr["FOB单价($)"]}','{dr["赔偿金额($)"]}','{dr["备注"]}','{usercode}','{date}','{time}')";
                DB.ExecuteNonQuery(sql);
            }

        }

        public static void Import_CustomerReturn(SJeMES_Framework_NETCore.DBHelper.DataBase DB, DataTable dt, string usercode)
        {
            try
            {
                DateTime Production_Date = new DateTime();
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                string time = DateTime.Now.ToString("HH:mm:ss");
                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        //string 生产日期_str = dr["生产日期"].ToString();
                        //var 生产日期_list = dr["Production_Date(MM-yyyy)"].ToString().Split('-').Select(x => Convert.ToInt32(x).ToString("00")).ToList();
                        var Production_date_list = dr["Production_Date(MM-yyyy)"].ToString().Split('-').Select(x => Convert.ToInt32(x).ToString("00")).ToList();
                        //string 生产日期_str = string.Join('-', 生产日期_list);
                        string Production_date_str = string.Join('-', Production_date_list);

                        //IFormatProvider ifp = new System.Globalization.CultureInfo("zh-CN", true);
                        //DateTime.Now.ToString("MM/dd/yy");
                        //DateTime 生产日期 = DateTime.ParseExact(生产日期_str, "MM/dd/yy", ifp);

                        Production_Date = DateTime.ParseExact(Production_date_str, "MM-yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"), System.Globalization.DateTimeStyles.AssumeLocal);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("The production date format is wrong, please check！");
                    }

                    try
                    {
                        decimal.Parse(dr["FOB"].ToString());
                        decimal.Parse(dr["Quantity"].ToString());
                        decimal.Parse(dr["Amount"].ToString());
                        decimal.Parse(dr["Additional_Fees"].ToString());
                    }
                    catch(Exception ex)
                    {
                        throw new Exception("FOB, quantity, amount, additional fee must be numeric type, please check!");
                    }

                    string thyf_str = "";
                    if (!string.IsNullOrWhiteSpace(dr["Return_Month"].ToString()))
                    {
                        DateTime is_date;
                        if (!DateTime.TryParse(dr["Return_Month"].ToString(), out is_date))
                        {
                            throw new Exception("The return month format is wrong, please check!");
                        }
                        thyf_str = is_date.ToString("yyyy-MM");
                    }


                    DB.ExecuteNonQuery($@"insert into QCM_CUSTOMER_RETURN_M(
                                                REGION,
                                                FACTORY_NO,
                                                FACTORY_NAME,
                                                SALESORGAN_NO,
                                                SALESORGAN_NAME,
                                                ARTICLE,
                                                SHOES_NAME,
                                                PRODUCTION_DATE,
                                                MASTERCODE,
                                                MASTERNAME,
                                                SECONDCODE,
                                                SECONDNAME,
                                                FOB,
                                                QTY,
                                                MONEY,
                                                PRICE,
		                                            CREATEBY,
		                                            CREATEDATE,
		                                            CREATETIME,
		                                            RETURN_MONTH
                                                )
                                         values(
                                                '{dr["Area"]}','{dr["Factory_Code"]}', '{dr["Factory_Name"]}','{dr["Sales_Org_Code"]}','{dr["Sales_Org_Name"]}','{dr["ARTICLE"]}',
                                                '{dr["Shoe_Type_Name"]}',to_date('{Production_Date:yyyy-MM-dd}','yyyy-mm-dd hh24:mi:ss'),'{dr["Main_Code"]}','{dr["Main_Code_Name"]}','{dr["Minor_Code"]}',
                                                '{dr["Minor_Code_Name"]}',{dr["FOB"]},{dr["Quantity"]}, {dr["Amount"]},{dr["Additional_Fees"]},
                                                '{usercode}', '{date}','{time}','{thyf_str}'
                                                )
                                ");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
    }
}
