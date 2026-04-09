using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class RawMaterialInspection
    {
        /// <summary>
        /// 原材料检验接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SelectWms_rcpt(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//物料编号
                Dictionary<string, object> wms_rcpt_d = new Dictionary<string, object>();//物料数据
                Dictionary<string, object> wms_rcpt = new Dictionary<string, object>();//物料信息
                Dictionary<string, object> ProjectInspection = new Dictionary<string, object>();//项目检验列表
                Dictionary<string, object> WWPlist = new Dictionary<string, object>();//合并返回数据

                //判断收料表是否存在此收料单号
                int count = Convert.ToInt32(DB.GetString($@"select count(1) from wms_rcpt_m where CHK_NO='{CHK_NO}'"));
                if (count == 0)
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "收料单号不存在!";
                    return ret;
                }

                //查询品号信息
                if (string.IsNullOrEmpty(ITEM_NO))
                {
                    string sql = $@"	
	select ITEM_NO,NAME_T from bdm_rd_item where ITEM_NO in(
	select ITEM_NO from wms_rcpt_d where CHK_NO='{CHK_NO}')";
                    DataTable dt = DB.GetDataTable(sql);
                    WWPlist.Add("wms_rcpt_d", dt);
                    ret.IsSuccess = true;
                    ret.RetData1 = WWPlist;
                    return ret;
                }

                //物料编号不为空才能进来
                if (!string.IsNullOrEmpty(ITEM_NO))
                {
                    #region 物料信息
                    //根据料品代号拿进仓日期和进仓数量
                    string asql = $@"select m.RCPT_DATE,d.RCPT_QTY from wms_rcpt_m m inner JOIN wms_rcpt_d d on m.CHK_NO=d.CHK_NO where d.ITEM_NO='{ITEM_NO}'";
                    DataTable adt = DB.GetDataTable(asql);
                    string RCPT_DATE = string.Empty;
                    string RCPT_QTY = string.Empty;
                    foreach (DataRow item in adt.Rows)
                    {
                        RCPT_DATE = item["RCPT_DATE"].ToString();
                        RCPT_QTY = item["RCPT_QTY"].ToString();
                    }
                    wms_rcpt.Add("RCPT_DATE", RCPT_DATE);
                    wms_rcpt.Add("RCPT_QTY", RCPT_QTY);

                    //根据料品代号拿ART
                    string bsql = $@"select PARENT_ITEM_NO from bdm_rd_item where ITEM_NO in(
	                        select PROD_NO from bdm_rd_bom_item  WHERE SUBMATERIAL_NO = '{ITEM_NO}')";
                    DataTable bdt = DB.GetDataTable(bsql);
                    string art = string.Empty;
                    foreach (DataRow item in bdt.Rows)
                    {
                        art += item["PARENT_ITEM_NO"] + ",";
                    }
                    wms_rcpt.Add("PARENT_ITEM_NO", art.TrimEnd(','));

                    //根据ART拿鞋型
                    string gsql = $@"select SHOE_NO from bdm_rd_prod where PROD_NO in (
	                                 select PARENT_ITEM_NO from bdm_rd_item where ITEM_NO in(
	                                 select PROD_NO from bdm_rd_bom_item  WHERE SUBMATERIAL_NO = '{ITEM_NO}'))";
                    DataTable gdt = DB.GetDataTable(gsql);
                    string SHOE_NO = string.Empty;
                    foreach (DataRow item in gdt.Rows)
                    {
                        SHOE_NO += item["SHOE_NO"] + ",";
                    }
                    wms_rcpt.Add("SHOE_NO", SHOE_NO.TrimEnd(','));

                    //根据料品代号拿皮料
                    string hsql = $@"select item_type_name from bdm_rd_itemtype where item_type_no in (										
                            select ITEM_TYPE from bdm_rd_item where ITEM_NO in(
	                        select PROD_NO from bdm_rd_bom_item  WHERE SUBMATERIAL_NO = '{ITEM_NO}'))";
                    DataTable hdt = DB.GetDataTable(hsql);
                    string Leather = "FALSE";
                    foreach (DataRow item in hdt.Rows)
                    {
                        if (item["item_type_name"].ToString().Equals("皮") || item["item_type_name"].ToString().Equals("里"))
                        {
                            Leather = "TRUE";
                        }
                        else
                            Leather = "FALSE";
                    }
                    wms_rcpt.Add("IS_LEATHER", Leather);

                    //根据料品代号拿部位编号
                    string csql = $@"select PART_NO from bdm_rd_bom_item where PROD_NO='{ITEM_NO}'";
                    DataTable cdt = DB.GetDataTable(csql);
                    string part_no = string.Empty;
                    foreach (DataRow item in cdt.Rows)
                    {
                        part_no += item["PART_NO"] + ",";
                    }
                    wms_rcpt.Add("PART_NO", part_no.TrimEnd(','));

                    //根据料品代号拿厂商名称
                    string dsql = $@"select bm.SUPPLIERS_CODE,bm.SUPPLIERS_NAME from wms_rcpt_m m inner JOIN wms_rcpt_d d on m.CHK_NO=d.CHK_NO INNER JOIN base003m bm ON m.VEND_NO=bm.SUPPLIERS_CODE where d.ITEM_NO='{ITEM_NO}'";
                    DataTable ddt = DB.GetDataTable(dsql);
                    string SUPPLIERS_NAME = string.Empty;
                    string SUPPLIERS_CODE = string.Empty;
                    foreach (DataRow item in ddt.Rows)
                    {
                        SUPPLIERS_NAME = item["SUPPLIERS_NAME"].ToString();
                        SUPPLIERS_CODE = item["SUPPLIERS_CODE"].ToString();
                    }
                    wms_rcpt.Add("SUPPLIERS_NAME", SUPPLIERS_NAME);
                    wms_rcpt.Add("SUPPLIERS_CODE", SUPPLIERS_CODE);

                    wms_rcpt.Add("TagCard", "标签卡不清楚暂时空着!");

                    #endregion

                    #region 检验项目列表
                    string esql = $@"SELECT
                                      testitem_code,
                                      testitem_name,
                                      sample_num ,
                                      (CASE NVL(d_check_value,'1')
                                      when '1' THEN t_check_value
                                      ELSE d_check_value
                                      END )as check_value
                                    FROM
                                      bdm_prod_customquality_item where category_no='{ITEM_NO}' and testitem_category='2'";
                    DataTable edt = DB.GetDataTable(esql);
                    edt.Columns.Add("AQL");
                    edt.Columns.Add("AC,RE");
                    edt.Columns.Add("InspectionResult");
                    edt.Columns.Add("remarks");
                    if (edt.Rows.Count > 0)
                    {
                        //如果ART定制表中有数据就进来
                        for (int i = 0; i < edt.Rows.Count; i++)
                        {
                            edt.Rows[i]["AQL"] = "值暂时为空,未确定!";
                            edt.Rows[i]["AC,RE"] = "值暂时为空,未确定!";
                            edt.Rows[i]["InspectionResult"] = "PASS";
                            edt.Rows[i]["remarks"] = "";
                        }
                        WWPlist.Add("ProjectInspection", edt);
                    }
                    else
                    {
                        //如果ART定制表没数据进来
                        string fsql = $@"select testitem_code,testitem_name,sample_num,check_value from bdm_qualityaptest_item where secondary_category_no in (
	                                     select br.ITEM_TYPE from wms_rcpt_d d INNER JOIN bdm_rd_item br ON d.ITEM_NO=br.ITEM_NO INNER JOIN wms_rcpt_m m ON m.chk_no=d.chk_no where m.chk_no='{CHK_NO}' and d.ITEM_NO='{ITEM_NO}')";
                        DataTable fdt = DB.GetDataTable(fsql);
                        fdt.Columns.Add("AQL");
                        fdt.Columns.Add("AC,RE");
                        fdt.Columns.Add("InspectionResult");
                        fdt.Columns.Add("remarks");
                        if (fdt.Rows.Count > 0)
                        {
                            for (int i = 0; i < fdt.Rows.Count; i++)
                            {
                                fdt.Rows[i]["AQL"] = "值暂时为空,未确定!";
                                fdt.Rows[i]["AC,RE"] = "值暂时为空,未确定!";
                                fdt.Rows[i]["InspectionResult"] = "PASS";
                                fdt.Rows[i]["remarks"] = "";
                            }
                            WWPlist.Add("ProjectInspection", fdt);
                        }
                        else
                        {
                            //如果都没有数据就提示维护
                            ret.IsSuccess = false;
                            ret.ErrMsg = "请维护该品号的通用品质标准数据!";
                            return ret;
                        }
                    }
                    #endregion
                }
                #endregion
                //数据合并返回
                WWPlist.Add("wms_rcpt", wms_rcpt);

                ret.RetData1 = WWPlist;
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
        /// 原材料检验提交接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubmitWms_rcpt(object OBJ)
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
                DataTable TableBody = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TableBody"].ToString());//表身数据
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//收料单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//料品代号
                string item_name = jarr.ContainsKey("NAME_T") ? jarr["NAME_T"].ToString() : "";//料品名称
                string is_leather = jarr.ContainsKey("IS_LEATHER") ? jarr["IS_LEATHER"].ToString() : "";//皮料
                string vend_no = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";//生产厂商代号
                string vend_name = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//生产厂商名称
                string rcpt_date = jarr.ContainsKey("RCPT_DATE") ? jarr["RCPT_DATE"].ToString() : "";//进仓日期
                int rcpt_qty = jarr.ContainsKey("RCPT_QTY") ? (jarr["RCPT_QTY"].ToString() == "" ? 0 : Convert.ToInt32(jarr["RCPT_QTY"].ToString())) : 0;//进仓数量
                string shoe_nos = jarr.ContainsKey("SHOE_NO") ? jarr["SHOE_NO"].ToString() : "";//鞋型
                string prod_nos = jarr.ContainsKey("PARENT_ITEM_NO") ? jarr["PARENT_ITEM_NO"].ToString() : "";//ART
                string part_nos = jarr.ContainsKey("PART_NO") ? jarr["PART_NO"].ToString() : "";//部位
                int ng_qty = jarr.ContainsKey("ng_qty") ? (jarr["ng_qty"].ToString()==""?0: Convert.ToInt32(jarr["ng_qty"].ToString())) : 0;//不合格数

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                //送检单号M
                string inspection_noM = "RM" + DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(inspection_no) from QCM_RM_INSPECTION_M where inspection_no like '{inspection_noM}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询送检单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(inspection_noM, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    inspection_noM = inspection_noM + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    inspection_noM = inspection_noM + "00001";
                }

                //送检单号D
                string inspection_noD = "RM" + DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int statusD = 1;
                string sqlD = $@"select max(inspection_no) from QCM_RM_INSPECTION_D where inspection_no like '{inspection_noD}%'";
                string max_inspection_noD = DB.GetString(sqlD);
                //查询送检单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_noD))
                {
                    string seq = max_inspection_noD.Replace(inspection_noD, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    inspection_noD = inspection_noD + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("送检单号：【" + inspection_no + "】重复，请检查!");
                }
                else
                {
                    inspection_noD = inspection_noD + "00001";
                }

                string result = string.Empty;//结果
                string ac = string.Empty;//AC
                string re = string.Empty;//RE

                //提交头部数据
                if (string.IsNullOrEmpty(chk_no) || string.IsNullOrEmpty(item_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "收料单号和物料品号不能为空!";
                    return ret;
                }
                foreach (DataRow dd in TableBody.Rows)
                {
                    if (dd["InspectionResult"].ToString() == "PASS")
                    {
                        result = "PASS";
                    }
                    else
                    {
                        result = "FAIL";
                        break;
                    }   
                }
                DB.ExecuteNonQuery($@"delete from qcm_rm_inspection_m where chk_no='{chk_no}' and item_no='{item_no}'");
                DB.ExecuteNonQuery($@"delete from qcm_rm_inspection_d where chk_no='{chk_no}' and item_no='{item_no}'");

                DB.ExecuteNonQuery($@"insert into qcm_rm_inspection_m (createby,createdate,createtime,inspection_no,chk_no,item_no,item_name,is_leather,vend_no,vend_name,rcpt_date,rcpt_qty,shoe_nos,prod_nos,part_nos,ng_qty,result) 
                                          values('{user}','{date}','{time}','{inspection_noM}','{chk_no}','{item_no}','{item_name}','{is_leather}','{vend_no}','{vend_name}','{rcpt_date}', 
                                          '{rcpt_qty}','{shoe_nos}','{prod_nos}','{part_nos}','{ng_qty}','{result}')
                                         ");
                //提交检验项目信息
                foreach (DataRow item in TableBody.Rows)
                {
                    var AR = item["AC,RE"].ToString().Split(',');
                    if (AR.Length == 1)
                        ac = AR[0];
                    else
                    {
                        ac = AR[0];
                        re = AR[1];
                    }
                    int samp_qty = item["SAMPLE_NUM"].ToString() == "" ? 0 : Convert.ToInt32(item["SAMPLE_NUM"].ToString());
                    DB.ExecuteNonQuery($@"insert into qcm_rm_inspection_d (createby,createdate,createtime,inspection_no,chk_no,item_no,aptestitem_code,aptestitem_name,test_standard,samp_qty,AQL_level,AC,RE,check_result,remarks)
                                          values('{user}','{date}','{time}','{inspection_noD}','{chk_no}','{item_no}','{item["testitem_code"]}','{item["testitem_name"]}','{item["CHECK_VALUE"]}','{samp_qty}','{item["AQL"]}','{ac}','{re}','{item["InspectionResult"]}','{item["remarks"]}')
                                        ");
                }
                #endregion
                ret.RetData1 = result;
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
        /// 原材料检验单查询接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SelectQcmRmInspection(object OBJ)
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
                string CHK_NO = jarr.ContainsKey("CHK_NO") ? jarr["CHK_NO"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//物料编号
                Dictionary<string, object> ALLlist = new Dictionary<string, object>();//合并返回数据

                string isdatacount = DB.GetString($@"select count(1) from qcm_rm_inspection_m where chk_no='{CHK_NO}'");
                if (isdatacount == "0")
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "未找到该收料单号!";
                    return ret;
                }

                //查询返回下拉框数据
                if (string.IsNullOrEmpty(ITEM_NO))
                {
                    string sql = $@"select ITEM_NO,NAME_T from bdm_rd_item where ITEM_NO in(
	select ITEM_NO from qcm_rm_inspection_d where CHK_NO='{CHK_NO}')";
                    DataTable dt = DB.GetDataTable(sql);
                    ALLlist.Add("wms_rcpt_d", dt);
                    ret.IsSuccess = true;
                    ret.RetData1 = ALLlist;
                    return ret;
                }

                string asql = $@"SELECT
	                            is_leather,
	                            vend_no,
	                            vend_name,
	                            rcpt_date,
	                            rcpt_qty,
	                            shoe_nos,
	                            prod_nos,
	                            part_nos,
	                            ng_qty
                                FROM
	                            qcm_rm_inspection_m where chk_no='{CHK_NO}' and item_no='{ITEM_NO}'";
                DataTable adt = DB.GetDataTable(asql);
                ALLlist.Add("qcm_rm_inspection_m", adt);

                string FAILsql = $@"	SELECT
	                                aptestitem_code,
	                                aptestitem_name,
	                                test_standard,
	                                'FAIL' as check_result,
	                                remarks
                                    FROM
	                                qcm_rm_inspection_d where chk_no='{CHK_NO}' and item_no='{ITEM_NO}' and check_result='FAIL'";
                DataTable FAILdt = DB.GetDataTable(FAILsql);
                ALLlist.Add("qcm_rm_inspection_FAIL", FAILdt);

                string PASSsql = $@"	SELECT
	                                aptestitem_code,
	                                aptestitem_name,
	                                test_standard,
	                                'FAIL' as check_result,
	                                remarks
                                    FROM
	                                qcm_rm_inspection_d where chk_no='{CHK_NO}' and item_no='{ITEM_NO}' and check_result='PASS'";
                DataTable PASSdt = DB.GetDataTable(PASSsql);
                ALLlist.Add("qcm_rm_inspection_PASS", PASSdt);

                #endregion
                ret.RetData1 = ALLlist;
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
        /// CLIMA单提交创建接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubmitQcmClimaorder(object OBJ)
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
                DataTable TableBody = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TableBody"].ToString());//表身数据
                string chk_no = jarr.ContainsKey("chk_no") ? jarr["chk_no"].ToString() : "";//收料单号
                string item_no = jarr.ContainsKey("item_no") ? jarr["item_no"].ToString() : "";//料品代号
                string vend_no = jarr.ContainsKey("SUPPLIERS_CODE") ? jarr["SUPPLIERS_CODE"].ToString() : "";//生产厂商代号
                string vend_name = jarr.ContainsKey("SUPPLIERS_NAME") ? jarr["SUPPLIERS_NAME"].ToString() : "";//生产厂商名称
                string rcpt_date = jarr.ContainsKey("RCPT_DATE") ? jarr["RCPT_DATE"].ToString() : "";//进仓日期
                int inspect_qty = jarr.ContainsKey("inspect_qty") ? (jarr["inspect_qty"].ToString() == "" ? 0 : Convert.ToInt32(jarr["inspect_qty"].ToString())) : 0;//clima数量
                int ok_qty = jarr.ContainsKey("ok_qty") ? (jarr["ok_qty"].ToString() == "" ? 0 : Convert.ToInt32(jarr["ok_qty"].ToString())) : 0;//合规数量
                int ng_qty = jarr.ContainsKey("ng_qty") ? (jarr["ng_qty"].ToString() == "" ? 0 : Convert.ToInt32(jarr["ng_qty"].ToString())) : 0;//退货数量
                int repair_qty = jarr.ContainsKey("repair_qty") ? (jarr["repair_qty"].ToString() == "" ? 0 : Convert.ToInt32(jarr["repair_qty"].ToString())) : 0;//补送数量

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                //提交头部数据
                if (string.IsNullOrEmpty(chk_no) || string.IsNullOrEmpty(item_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "收料单号和物料品号不能为空!";
                    return ret;
                }

                //CLIMA单号M
                string climaorder_noM = DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(climaorder_no) from qcm_climaorder_m where climaorder_no like '{climaorder_noM}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询CLIMA单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(climaorder_noM, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    climaorder_noM = climaorder_noM + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("CLIMA单号：【" + climaorder_no + "】重复，请检查!");
                }
                else
                {
                    climaorder_noM = climaorder_noM + "00001";
                }

                //CLIMA单号D
                string climaorder_noD = DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int statusD = 1;
                string sqlD = $@"select max(climaorder_no) from qcm_climaorder_d where climaorder_no like '{climaorder_noD}%'";
                string max_inspection_noD = DB.GetString(sqlD);
                //查询CLIMA单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_noD))
                {
                    string seq = max_inspection_noD.Replace(climaorder_noD, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    climaorder_noD = climaorder_noD + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("CLIMA单号：【" + climaorder_no + "】重复，请检查!");
                }
                else
                {
                    climaorder_noD = climaorder_noD + "00001";
                }


                //判断收料单号和物料品号不能为空
                if (string.IsNullOrEmpty(chk_no) || string.IsNullOrEmpty(item_no))
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "收料单号和物料品号不能为空!";
                    return ret;
                }

                //新增CLIMA单
                DB.ExecuteNonQuery($@"delete from qcm_climaorder_m where chk_no='{chk_no}' and item_no='{item_no}'");
                DB.ExecuteNonQuery($@"delete from qcm_climaorder_d where chk_no='{chk_no}' and item_no='{item_no}'");

                string pr_unit = string.Empty;//收货单位
                string purchase_no = DB.GetString($@"select m.SOURCE_NO from wms_rcpt_m m inner join wms_rcpt_d d on m.CHK_NO=d.CHK_NO where rownum=1 and d.CHK_NO='{chk_no}' and d.ITEM_NO='{item_no}' and m.RCPT_BY='01' ORDER BY m.rcpt_date desc ");//采购单号
                string pur_vend_no = string.Empty;//采购厂商编号
                string pur_vend_name = string.Empty;//采购厂商名称
                string ord_qty = string.Empty;//采购量
                string samp_condition = string.Empty;//取样状况  空
                string tscheck_result = string.Empty;//测试结果
                string tscheck_date = string.Empty;//实验室检验日期
                string inspector = string.Empty;//检验人员
                string warehouse_qty = string.Empty;//仓库  空
                string result = DB.GetString($@"select result from qcm_rm_inspection_m where chk_no='{chk_no}' and item_no='{item_no}'");

                //送检登记表数据
                DataTable ffdt = DB.GetDataTable($@"select staff_no,inspection_enddate,result from qcm_inspection_laboratory_m where rownum=1 and doc_no='{chk_no}' and material_no='{item_no}' and document_type='{enum_document_type.enum_document_type_1}' ORDER BY ID desc");
                foreach (DataRow item in ffdt.Rows)
                {
                    inspector = item["staff_no"].ToString();
                    tscheck_result = item["result"].ToString();
                    tscheck_date = item["inspection_enddate"].ToString();
                }

                //收料单表中数据
                string rcsql = $@"	select * from wms_rcpt_m m inner join wms_rcpt_d d on m.CHK_NO=d.CHK_NO where rownum=1 and  d.CHK_NO='{chk_no}' and d.ITEM_NO='{item_no}'  ORDER BY m.rcpt_date desc ";
                DataTable redt = DB.GetDataTable(rcsql);
                foreach (DataRow item in redt.Rows)
                {
                    pr_unit = item["PR_UNIT"].ToString();
                }

                //采购单表数据
                string wrsql = $@"select m.VEND_NO,i.ORD_QTY from bdm_purchase_order_m m inner join bdm_purchase_order_item i on m.ORDER_NO=i.ORDER_NO where m.ORDER_NO = (
	select m.SOURCE_NO from wms_rcpt_m m inner join wms_rcpt_d d on m.CHK_NO=d.CHK_NO where rownum=1 and  d.CHK_NO='{chk_no}' and d.ITEM_NO='{item_no}'  )ORDER BY m.INSERT_DATE desc";
                DataTable wrdt = DB.GetDataTable(wrsql);
                foreach (DataRow item in wrdt.Rows)
                {
                    pur_vend_no = item["VEND_NO"].ToString();
                    pur_vend_name = DB.GetString($@"select suppliers_name from base003m where suppliers_code='{item["VEND_NO"]}'");
                    ord_qty = item["ORD_QTY"].ToString();
                }

                //新增CLIMA表
                DB.ExecuteNonQuery($@"insert into qcm_climaorder_m (createby,createdate,createtime,climaorder_no,chk_no,rcpt_date,pro_vend_no,pro_vend_name,item_no,pr_unit,purchase_no,pur_vend_no,pur_vend_name,ord_qty, 
                                      samp_condition,apcheck_result,apcheck_date,tscheck_result,tscheck_date,inspector,inspect_qty,ok_qty,ng_qty,repair_qty,warehouse_qty,status) 
                                      values('{user}','{date}','{time}','{climaorder_noM}','{chk_no}','{rcpt_date}','{vend_no}','{vend_name}','{item_no}','{pr_unit}','{purchase_no}','{pur_vend_no}','{pur_vend_name}','{ord_qty}',
                                      '{samp_condition}','{result}','{rcpt_date}','{tscheck_result}','{tscheck_date}','{inspector}','{inspect_qty}','{ok_qty}','{ng_qty}','{repair_qty}','{warehouse_qty}','1')");


                //新增CLIMA单明细
                foreach (DataRow item in TableBody.Rows)
                {
                    DB.ExecuteNonQuery($@"insert into qcm_climaorder_d (createby,createdate,createtime,climaorder_no,chk_no,item_no,aptestitem_code,aptestitem_name,check_result,remarks) 
                                          values('{user}','{date}','{time}','{climaorder_noD}','{chk_no}','{item_no}','{item["aptestitem_code"]}','{item["aptestitem_name"]}','{item["check_result"]}','{item["remarks"]}')");
                }

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
        /// 原材料检验画皮查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SelectPaintedSkin(object OBJ)
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
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//物料编号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";
                Dictionary<string, object> ALLlist = new Dictionary<string, object>();//返回数据
                string sql = $@"SELECT
	                            d.CHK_NO 
                            FROM
	                            wms_rcpt_d d
	                            INNER JOIN 
	                            wms_rcpt_m m
	                            ON d.CHK_NO=m.CHK_NO
                            WHERE
	                            ITEM_NO = '{ITEM_NO}' 
	                            AND m.RCPT_BY=1
	                            AND d.CHK_NO NOT IN ( SELECT CHK_NO FROM qcm_rm_paintedskin_d )";
                DataTable dt = DB.GetDataTable(sql);
                string SUPPLIERS_NAME = string.Empty;//厂商名称
                string SUPPLIERS_CODE = string.Empty;//厂商代号
                string NAME_T = string.Empty;//料品名称

                #region 根据料品代号拿数据

                //根据料品代号拿厂商名称
                string dsql = $@"select bm.SUPPLIERS_CODE,bm.SUPPLIERS_NAME from wms_rcpt_m m inner JOIN wms_rcpt_d d on m.CHK_NO=d.CHK_NO INNER JOIN base003m bm ON m.VEND_NO=bm.SUPPLIERS_CODE where d.ITEM_NO='{ITEM_NO}'";
                DataTable ddt = DB.GetDataTable(dsql);
                foreach (DataRow item in ddt.Rows)
                {
                    SUPPLIERS_NAME = item["SUPPLIERS_NAME"].ToString();
                    SUPPLIERS_CODE = item["SUPPLIERS_CODE"].ToString();
                }

                //根据料品代号拿料品名称
                string nsql = $@"select NAME_T from bdm_rd_item where ITEM_NO='{ITEM_NO}'";
                DataTable ndt = DB.GetDataTable(nsql);
                foreach (DataRow item in ndt.Rows)
                {
                    NAME_T = item["NAME_T"].ToString();
                }

                string ss = $@"select PAINT_NO,ITEM_NO,ITEM_NAME,QTY,PAINT_DATE from qcm_rm_paintedskin_m where ITEM_NO='{ITEM_NO}'";

                #endregion
                DataTable sdt = CommonBASE.GetPageDataTable(DB, ss, int.Parse(pageIndex), int.Parse(pageSize));
                int rowCount = CommonBASE.GetPageDataTableCount(DB, ss);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                ALLlist.Add("SUPPLIERS_NAME", SUPPLIERS_NAME);
                ALLlist.Add("SUPPLIERS_CODE", SUPPLIERS_CODE);
                ALLlist.Add("NAME_T", NAME_T);
                ALLlist.Add("item_no", ITEM_NO);
                ALLlist.Add("CHK_NO", dt);
                ALLlist.Add("QCM_RM_PAINTEDSKIN_M", sdt);
                ALLlist.Add("rowCount", rowCount);
                #endregion
                ret.RetData1 = ALLlist;
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
                //DB.Close();//关闭事务
            }
            return ret;

        }

        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject TaskCreation(object OBJ)
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
                string CHK_NOS = jarr.ContainsKey("CHK_NOS") ? jarr["CHK_NOS"].ToString() : "";//收料单号
                string ITEM_NO = jarr.ContainsKey("ITEM_NO") ? jarr["ITEM_NO"].ToString() : "";//物料编号
                if (string.IsNullOrEmpty(CHK_NOS)||string.IsNullOrEmpty(ITEM_NO))
                {
                    ret.ErrMsg = "收料单号和物料编号不能为空!";
                    ret.IsSuccess = false;
                    return ret;
                }
                Dictionary<string, object> ALLlist = new Dictionary<string, object>();//返回数据
                string[] CHK_NO = CHK_NOS.Split(',');
                string RCPT_DATE = string.Empty;//进仓日期
                double RCPT_QTY = 0;//进仓数量
                string SUPPLIERS_NAME = string.Empty;//厂商名称
                string SUPPLIERS_CODE = string.Empty;//厂商代号
                string SHOE_NO = string.Empty;//鞋型
                string art = string.Empty;//ART
                string part_no = string.Empty;//部位编号
                string NAME_T = string.Empty;//料品名称

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间


                //画皮单号M
                string PAINT_NO = DateTime.Now.ToString("yyyyMMdd");//QA20211016  00001
                int status = 1;
                string sql = $@"select max(PAINT_NO) from qcm_rm_paintedskin_m where PAINT_NO like '{PAINT_NO}%'";
                string max_inspection_no = DB.GetString(sql);
                //查询画皮单号有没有相同的
                if (!string.IsNullOrEmpty(max_inspection_no))
                {
                    string seq = max_inspection_no.Replace(PAINT_NO, "");//00002

                    int int_seq = Convert.ToInt32(seq) + 1;//3   00111

                    PAINT_NO = PAINT_NO + int_seq.ToString().PadLeft(5, '0');
                    //throw new Exception("画皮单号：【" + PAINT_NO + "】重复，请检查!");
                }
                else
                {
                    PAINT_NO = PAINT_NO + "00001";
                }

                #region 根据料品,收料单号拿数据
                for (int i = 0; i < CHK_NO.Length; i++)
                {
                    //根据料品代号拿进仓日期和进仓数量
                    string asql = $@"select m.RCPT_DATE,d.RCPT_QTY from wms_rcpt_m m inner JOIN wms_rcpt_d d on m.CHK_NO=d.CHK_NO where d.CHK_NO='{CHK_NO[i]}' and d.ITEM_NO='{ITEM_NO}'";
                    DataTable adt = DB.GetDataTable(asql);
                    foreach (DataRow item in adt.Rows)
                    {
                        RCPT_DATE += item["RCPT_DATE"].ToString() + ",";
                        RCPT_QTY = Convert.ToDouble(item["RCPT_QTY"].ToString()) + RCPT_QTY;
                        DB.ExecuteNonQuery($@"insert into qcm_rm_paintedskin_d (PAINT_NO,CHK_NO,RCPT_DATE,RCPT_QTY,CREATEBY,CREATEDATE,CREATETIME)
                                              values('{PAINT_NO}','{CHK_NO[i]}','{item["RCPT_DATE"]}','{item["RCPT_QTY"]}','{user}','{date}','{time}')");
                    }
                }
                #endregion

                #region 根据料品代号拿数据

                //根据料品代号拿厂商名称
                string dsql = $@"select bm.SUPPLIERS_CODE,bm.SUPPLIERS_NAME from wms_rcpt_m m inner JOIN wms_rcpt_d d on m.CHK_NO=d.CHK_NO INNER JOIN base003m bm ON m.VEND_NO=bm.SUPPLIERS_CODE where d.ITEM_NO='{ITEM_NO}'";
                DataTable ddt = DB.GetDataTable(dsql);
                foreach (DataRow item in ddt.Rows)
                {
                    SUPPLIERS_NAME = item["SUPPLIERS_NAME"].ToString();
                    SUPPLIERS_CODE = item["SUPPLIERS_CODE"].ToString();
                }

                //根据ART拿鞋型
                string gsql = $@"select SHOE_NO from bdm_rd_prod where PROD_NO in (
	                                 select PARENT_ITEM_NO from bdm_rd_item where ITEM_NO in(
	                                 select PROD_NO from bdm_rd_bom_item  WHERE SUBMATERIAL_NO = '{ITEM_NO}'))";
                DataTable gdt = DB.GetDataTable(gsql);
                foreach (DataRow item in gdt.Rows)
                {
                    SHOE_NO += item["SHOE_NO"] + ",";
                }

                //根据料品代号拿ART
                string bsql = $@"select PARENT_ITEM_NO from bdm_rd_item where ITEM_NO in(
	                        select PROD_NO from bdm_rd_bom_item  WHERE SUBMATERIAL_NO = '{ITEM_NO}')";
                DataTable bdt = DB.GetDataTable(bsql);
                foreach (DataRow item in bdt.Rows)
                {
                    art += item["PARENT_ITEM_NO"] + ",";
                }

                //根据料品代号拿部位编号
                string csql = $@"select PART_NO from bdm_rd_bom_item where SUBMATERIAL_NO='{ITEM_NO}'";
                DataTable cdt = DB.GetDataTable(csql);
                foreach (DataRow item in cdt.Rows)
                {
                    part_no += item["PART_NO"] + ",";
                }

                //根据料品代号拿料品名称
                string nsql = $@"select NAME_T from bdm_rd_item where ITEM_NO='{ITEM_NO}'";
                DataTable ndt = DB.GetDataTable(nsql);
                foreach (DataRow item in ndt.Rows)
                {
                    NAME_T = item["NAME_T"].ToString();
                }
                #endregion
                SHOE_NO = SHOE_NO == "" ? "" : SHOE_NO.TrimEnd(',');
                art = art == "" ? "" : art.TrimEnd(',');
                part_no = part_no == "" ? "" : part_no.TrimEnd(',');
                RCPT_DATE = RCPT_DATE == "" ? "" : RCPT_DATE.TrimEnd(',');

                string ss = $@"insert into QCM_RM_PAINTEDSKIN_M(PAINT_NO, PAINT_DATE, ITEM_NO, ITEM_NAME, vend_no,
                                       vend_name, RCPT_DATES, QTY, SHOE_NOS, PROD_NOS, part_nos, CREATEBY, CREATEDATE, CREATETIME)
                                      values('{PAINT_NO}', '{date}', '{ITEM_NO}', '{NAME_T}', '{SUPPLIERS_CODE}',
                                       '{SUPPLIERS_NAME}', '{RCPT_DATE}', '{RCPT_QTY}', '{SHOE_NO}', '{art}', '{part_no}', '{user}', '{date}', '{time}')";
                DB.ExecuteNonQuery(ss);
                #endregion
                DB.Commit();//提交事务
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int rowCount = CommonBASE.GetPageDataTableCount(DB, "select PAINT_NO,ITEM_NO,ITEM_NAME,QTY,PAINT_DATE from qcm_rm_paintedskin_m");
                DataTable dd = new DataTable();
                dd.Rows.Add();
                dd.Columns.Add("PAINT_NO");
                dd.Columns.Add("ITEM_NO");
                dd.Columns.Add("ITEM_NAME");
                dd.Columns.Add("QTY");
                dd.Columns.Add("PAINT_DATE");
                dd.Rows[0]["PAINT_NO"] = PAINT_NO;
                dd.Rows[0]["ITEM_NO"] = ITEM_NO;
                dd.Rows[0]["ITEM_NAME"] = NAME_T;
                dd.Rows[0]["QTY"] = RCPT_QTY;
                dd.Rows[0]["PAINT_DATE"] = date;
                dic.Add("NowData", dd);
                dic.Add("rowCount", rowCount);
                ret.IsSuccess = true;
                ret.RetData1 = dic;
                
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
        /// 点击操作查询
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SelectOperation(object OBJ)
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
                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";//画皮任务单号

                DataTable dt = DB.GetDataTable($@"SELECT
	                                            PAINT_NO,
	                                            ITEM_NO,
	                                            ITEM_NAME,
	                                            vend_no,
	                                            vend_name,
	                                            RCPT_DATES,
	                                            QTY,
	                                            SHOE_NOS,
	                                            PROD_NOS,
	                                            part_nos 
                                            FROM
	                                            QCM_RM_PAINTEDSKIN_M
	                                            where PAINT_NO ='{PAINT_NO}'");
                string CHK_NO = string.Empty;
                DataTable ss = DB.GetDataTable($@"select CHK_NO from qcm_rm_paintedskin_d where PAINT_NO='{PAINT_NO}'");
                foreach (DataRow item in ss.Rows)
                {
                    CHK_NO += item["CHK_NO"].ToString()+",";
                }
                dt.Columns.Add("CHK_NO");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["CHK_NO"] = CHK_NO.TrimEnd(',');
                }
                #endregion
                ret.IsSuccess = true;
                ret.RetData1 = dt;
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
        /// 提交
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubmitPaintedSkin(object OBJ)
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
                DataTable TableBody = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(jarr["TableBody"].ToString());//等级数据
                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";//画皮任务单号

                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//获取的登陆人信息
                string date = DateTime.Now.ToString("yyyy-MM-dd");//日期
                string time = DateTime.Now.ToString("HH-mm-ss");//时间

                foreach (DataRow item in TableBody.Rows)
                {
                    if (string.IsNullOrEmpty(item["SUPPLIER_AREA"].ToString())|| string.IsNullOrEmpty(item["ACTUAL_AREA"].ToString())|| string.IsNullOrEmpty(item["PAINT_LEVEL"].ToString()))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = "面积或等级不能为空!";
                        return ret;
                    }
                    DB.ExecuteNonQuery($@"insert into qcm_rm_paintedskin_item (PAINT_NO,PAINT_LEVEL,SUPPLIER_AREA,ACTUAL_AREA, CREATEBY, CREATEDATE, CREATETIME) 
                                          values('{PAINT_NO}','{item["PAINT_LEVEL"]}','{item["SUPPLIER_AREA"]}','{item["ACTUAL_AREA"]}', '{user}', '{date}', '{time}')");
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
        /// 原材料检验画皮查看进度
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject PlanPaintedSkin(object OBJ)
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
                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";//画皮单号

                Dictionary<string, object> dic = new Dictionary<string, object>();
                string sql = string.Empty;
                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(PAINT_NO))
                {
                    sql = $@"
                                SELECT
	                                m.PAINT_LEVEL,
	                                SUM( ACTUAL_AREA ) summary,
	                                s.enum_value2 ,
		                                SUM( ACTUAL_AREA ) *enum_value2 as multiple
                                FROM
	                                qcm_rm_paintedskin_item m
	                                INNER JOIN SYS001M s ON s.ENUM_CODE = m.PAINT_LEVEL 
                                WHERE
	                                s.ENUM_TYPE = 'enum_paintedskin_level' and PAINT_NO='{PAINT_NO}'
                                GROUP BY
	                                m.PAINT_LEVEL ,
	                                s.enum_value2
									ORDER BY m.PAINT_LEVEL";
                    dt = DB.GetDataTable(sql);
                }
                double summary = 0;
                double multiple = 0;
                double qualityfactor = 0;
                string aa = string.Empty;
                foreach (DataRow item in dt.Rows)
                {
                    summary = Convert.ToDouble(item["summary"].ToString()==""?0: item["summary"]) + summary;
                    multiple = Convert.ToDouble(item["multiple"].ToString() == "" ? 0 : item["multiple"]) + multiple;
                }
                if (summary!=0 || multiple!=0)
                {
                    qualityfactor = multiple / summary;
                }
                if (qualityfactor!=0)
                {
                    aa = Math.Round(Convert.ToDecimal(qualityfactor), 2, MidpointRounding.AwayFromZero).ToString();
                }
                dt.Rows.Add();
                dt.Rows[dt.Rows.Count - 1]["PAINT_LEVEL"] = "总数";
                dt.Rows[dt.Rows.Count - 1]["summary"] = summary;
                dt.Rows[dt.Rows.Count - 1]["enum_value2"] = "—";
                dt.Rows[dt.Rows.Count - 1]["multiple"] = multiple;
                dic.Add("qcm_rm_paintedskin_item", dt);
                dic.Add("qualityfactor", aa);
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
        /// 任务删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject DeleteTask(object OBJ)
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
                string PAINT_NO = jarr.ContainsKey("PAINT_NO") ? jarr["PAINT_NO"].ToString() : "";//画皮任务单号

                string count = DB.GetString($@"select count(1) from qcm_rm_paintedskin_item where PAINT_NO='{PAINT_NO}'");

                if (count!="0")
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "该任务已有录入数据,不能删除!";
                    return ret;
                }

                DB.ExecuteNonQuery($@"delete from QCM_RM_PAINTEDSKIN_M where PAINT_NO='{PAINT_NO}'");
                DB.ExecuteNonQuery($@"delete from qcm_rm_paintedskin_d where PAINT_NO='{PAINT_NO}'");
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
    }
}
