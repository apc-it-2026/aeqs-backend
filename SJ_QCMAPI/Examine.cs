using SJ_QCMAPI;
using SJ_QCMAPI.Common;
using SJeMES_Framework_NETCore.Common;
using SJeMES_Framework_NETCore.WebAPI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SJ_QCMAPI
{
    public class Examine
    {
        /// <summary>
        /// 根据检测项获取检验单和检测项公式内容信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTestItemDetailsData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();

            try
            {
                string sql = string.Empty;
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);




                #region 逻辑
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                string d_id = jarr.ContainsKey("d_id") ? jarr["d_id"].ToString() : "";
                if (string.IsNullOrEmpty(d_id))
                    throw new Exception("接口参数不能为空，请检查！");

                sql = @"select a.*,b.enum_value JUDGE_MODE_NAME,a.remark as item_remark
                                from qcm_ex_task_list_d a
                                left join sys001m b on a.judgment_criteria=b.enum_code and b.enum_type='enum_judgment_criteria'
                                where a.id=@d_id";
                Parameters.Add("d_id", d_id);
                Dictionary<string, object> dic_qcm_ex_task_list_d = DB.GetDictionary(sql, Parameters);
                if (dic_qcm_ex_task_list_d == null || dic_qcm_ex_task_list_d.Count == 0)
                    throw new Exception("数据不存在，请返回刷新界面后重试！");
                string task_no = dic_qcm_ex_task_list_d["TASK_NO"].ToString();
                decimal SAMPLE_QTY = Convert.ToDecimal(dic_qcm_ex_task_list_d["SAMPLE_QTY"].ToString());



                Dictionary<string, object> retData = new Dictionary<string, object>();
                List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

                #region 1、返回头部信息
                sql = @"select a.task_no,                     -- 检验单编号
                                a.art_no,                     -- ART编号
                                b.enum_value test_type_name,-- 送检类型
                                a.staff_name,                 -- 送检人员
                                a.phase_creation_name,        -- 阶段
                                a.MANUFACTURER_NAME as workmanship,                -- 厂商
                                a.createdate,                 -- 送检时间
                                a.test_reason,                -- 送测原因
                                a.submission_date,            -- 检测提交日期
                                a.completion_date             -- 检测完成日期
                        from qcm_ex_task_list_m a
                            left join sys001m b on a.test_type=b.enum_code and b.enum_type='enum_test_type' 
                        where a.task_no=@task_no";
                Parameters.Add("task_no", task_no);
                Dictionary<string, object> dic_qcm_ex_task_list_m = DB.GetDictionary(sql, Parameters);
                if (dic_qcm_ex_task_list_m == null || dic_qcm_ex_task_list_m.Count == 0)
                    throw new Exception("扫描的检测单号不存在，请检查！");



                dic_qcm_ex_task_list_m.Add("INSPECTION_NAME", dic_qcm_ex_task_list_d["INSPECTION_NAME"].ToString());//测试项内容
                dic_qcm_ex_task_list_m.Add("JUDGE_MODE", dic_qcm_ex_task_list_d["JUDGE_MODE_NAME"].ToString());//判断标准
                dic_qcm_ex_task_list_m.Add("STANDARD_VALUE", dic_qcm_ex_task_list_d["STANDARD_VALUE"].ToString());//测量标准
                dic_qcm_ex_task_list_m.Add("ART_D_REMARK", dic_qcm_ex_task_list_d["ART_D_REMARK"].ToString());//检验项备注
                #endregion


                #region 2、返回自定义计算公式  
                Dictionary<string, object> strFormula = new Dictionary<string, object>();
                string formula_content = string.Empty;
                string formula_name = string.Empty;

                sql = $@"SELECT formula_content,formula_name FROM bdm_formula_m  WHERE  FORMULA_CODE = @FORMULA_CODE ";
                Parameters.Clear();
                Parameters.Add("FORMULA_CODE", dic_qcm_ex_task_list_d["D_FORMULA_CODE"].ToString());
                Dictionary<string, object> dic_bdm_formula_m = DB.GetDictionary(sql, Parameters);

                if (dic_bdm_formula_m != null && dic_bdm_formula_m.Count > 0)
                {
                    string charF = string.Empty;
                    formula_name = dic_bdm_formula_m["FORMULA_NAME"].ToString();
                    formula_content = dic_bdm_formula_m["FORMULA_CONTENT"].ToString();

                }
                #endregion

                #region 查看公式信息
                Dictionary<string, string> FormulaContent = new Dictionary<string, string>();
                sql = $@"select enum_value from sys001m where enum_type = 'enum_general_formula' and enum_code = @g_formula_code";
                Parameters.Clear();
                Parameters.Add("g_formula_code", dic_qcm_ex_task_list_d["G_FORMULA_CODE"].ToString());
                string g_formula_name = DB.GetString(sql, Parameters);
                FormulaContent.Add("g_formula_name", g_formula_name);//通用公式名称
                FormulaContent.Add("d_formula_code", formula_name);//自定义公式名称
                FormulaContent.Add("d_formula_content", formula_content);//自定义公式内容
                #endregion 

                //4、检验项设备，备注信息，公式参数   
                sql = @"select id,item_seq,eq_info_no,eq_info_name,remarks,formula_parameters,calculation_result  from qcm_ex_task_item where d_id=@d_id";
                Parameters.Clear();
                Parameters.Add("d_id", d_id);
                DataTable dt_qcm_ex_task_item = DB.GetDataTable(sql, Parameters);

                //5、检验项图片明细列表 
                sql = @"select a.item_id,a.img_guid,b.file_url 
                            from qcm_ex_task_item_img a
                            left join BDM_UPLOAD_FILE_ITEM b on a.img_guid = b.guid
                            where task_no=@task_no";
                Parameters.Clear();
                Parameters.Add("task_no", task_no);
                DataTable dt_item_img = DB.GetDataTable(sql, Parameters);



                for (int i = 1; i <= SAMPLE_QTY; i++)
                {
                    Dictionary<string, object> lstData = new Dictionary<string, object>();
                    List<Dictionary<string, object>> CustomFormula = new List<Dictionary<string, object>>();

                    #region 公式内容
                    string charF = string.Empty;
                    for (int j = 0; j < formula_content.Length; j++)
                    {
                        if (formula_content[j].ToString() == "N")
                        {
                            if (charF.Length > 0)
                            {
                                strFormula = new Dictionary<string, object>();
                                strFormula.Add("type", 0);
                                strFormula.Add("text", charF);
                                CustomFormula.Add(strFormula);
                                charF = string.Empty;
                            }
                            charF = formula_content[j].ToString();
                            strFormula = new Dictionary<string, object>();
                            strFormula.Add("type", 1);
                            strFormula.Add("text", "");
                            CustomFormula.Add(strFormula);
                            charF = string.Empty;
                        }
                        else
                            charF += formula_content[j].ToString();
                    }
                    if (charF.Length > 0)
                    {
                        strFormula = new Dictionary<string, object>();
                        strFormula.Add("type", 0);
                        strFormula.Add("text", charF);
                        CustomFormula.Add(strFormula);
                    }
                    #endregion

                    //每项结果
                    decimal result = 0;
                    string eq_info_no = string.Empty;
                    string eq_info_name = string.Empty;
                    string remarks = string.Empty;
                    List<Dictionary<string, string>> lst_img = new List<Dictionary<string, string>>();

                    DataRow[] dr_item = dt_qcm_ex_task_item.Select($"item_seq='{i}'");
                    if (dr_item.Length > 0)
                    {
                        decimal.TryParse(dr_item[0]["calculation_result"].ToString(), out result);
                        var arr = dr_item[0]["formula_parameters"].ToString().Split(',').ToList();
                        int index = 0;
                        if (!string.IsNullOrEmpty(dr_item[0]["formula_parameters"].ToString()) && arr.Count > 0)
                        {
                            foreach (Dictionary<string, object> dicCustom in CustomFormula)
                            {
                                if (dicCustom["type"].ToString().Equals("1"))
                                {
                                    dicCustom["text"] = arr[index];
                                    index++;
                                }
                            }
                        }

                        eq_info_no = dr_item[0]["eq_info_no"].ToString();
                        eq_info_name = dr_item[0]["eq_info_name"].ToString();
                        remarks = dr_item[0]["remarks"].ToString();

                        DataRow[] drs = dt_item_img.Select($"item_id='{dr_item[0]["id"].ToString()}'");
                        if (drs.Length > 0)
                        {
                            foreach (DataRow dri in drs)
                            {
                                Dictionary<string, string> dicImg = new Dictionary<string, string>();
                                dicImg.Add("url", dri["file_url"].ToString());
                                dicImg.Add("guid", dri["img_guid"].ToString());
                                lst_img.Add(dicImg);
                            }
                        }
                    }

                    lstData.Add("result", result);
                    lstData.Add("CustomFormula", CustomFormula);
                    lstData.Add("eq_info_no", eq_info_no);
                    lstData.Add("eq_info_name", eq_info_name);
                    lstData.Add("remarks", remarks);
                    lstData.Add("lstImg", lst_img);

                    list.Add(lstData);
                }
                #endregion

                retData.Add("HeadData", dic_qcm_ex_task_list_m);//头部数据 
                retData.Add("FormulaContent", FormulaContent);//查看公式内容
                retData.Add("StandardValue", dic_qcm_ex_task_list_d["JUDGE_MODE_NAME"].ToString() + " " + dic_qcm_ex_task_list_d["STANDARD_VALUE"].ToString());//判断标准
                retData.Add("Result", dic_qcm_ex_task_list_d["ITEM_TEST_VAL"].ToString());//综合计算结果
                retData.Add("TestResult", dic_qcm_ex_task_list_d["ITEM_TEST_RESULT"].ToString());//检测结果（PASS/FAIL）

                retData.Add("list", list);//内容 
                retData.Add("item_remark", dic_qcm_ex_task_list_d["ITEM_REMARK"].ToString());//内容 

                ret.RetData1 = retData;
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
        /// 获取检验单编号接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetInspectionOrder(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string sql = string.Empty;
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 参数
                Dictionary<string, object> Parameters = new Dictionary<string, object>();

                string task_no = string.Empty;//jarr.ContainsKey("task_no") ? jarr["task_no"].ToString() : "";//检验单编号 
                string barcode = jarr.ContainsKey("barcode") ? jarr["barcode"].ToString() : "";//条码：检验单编号@类型@检测项编号@序号
                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "10";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "1";
                string inspection_type = string.Empty;//类型
                string inspection_code = string.Empty;//检测项编号
                string inspection_seq = string.Empty;//序号

                if (string.IsNullOrEmpty(barcode))
                    throw new Exception("Please scan the QR code of the inspection form or the QR code of the test item first！");

                //扫描的是检测条码
                if (barcode.Contains("@"))
                {
                    string[] arrbarcode = barcode.Split('@');
                    if (arrbarcode.Length != 4)
                        throw new Exception("The scanned barcode is incorrect, please check！");
                    task_no = arrbarcode[0].ToString();
                    inspection_type = arrbarcode[1].ToString();
                    inspection_code = arrbarcode[2].ToString();
                    inspection_seq = arrbarcode[3].ToString();
                }
                else
                    task_no = barcode;
                #endregion

                #region 逻辑
                Dictionary<string, object> retData = new Dictionary<string, object>();
                Dictionary<string, object> dic_item = new Dictionary<string, object>();
                List<Dictionary<string, object>> lst_dic_item = new List<Dictionary<string, object>>();

                sql = @"select a.task_no,                     -- 检验单编号
                                a.art_no,                     -- ART编号
                                b.enum_value test_type_name,  -- 送检类型
                                a.staff_name,                 -- 送检人员
                                a.phase_creation_name,        -- 阶段
                                a.MANUFACTURER_NAME as workmanship,                -- 厂商
                                a.createdate,                 -- 送检时间
                                a.test_reason,                -- 送测原因
                                a.submission_date,            -- 检测提交日期
                                a.completion_date             -- 检测完成日期
                        from qcm_ex_task_list_m a
                            left join sys001m b on a.test_type=b.enum_code and b.enum_type='enum_test_type' 
                        where a.task_no=@task_no";
                Parameters.Add("task_no", task_no);
                Dictionary<string, object> dic_qcm_ex_task_list_m = DB.GetDictionary(sql, Parameters);
                if (dic_qcm_ex_task_list_m == null || dic_qcm_ex_task_list_m.Count == 0)
                    throw new Exception("The scanned test number does not exist, please check！");

                sql = @"
                            select 
                            a.id,
                            a.inspection_code,                                                        -- 检验项编号
                            a.inspection_name,                                                        -- 检验内容
                            a.judgment_criteria,                                                      -- 测量标准(通用)
                            b.enum_value,
                            a.standard_value,                                                         -- 测量标准(定制)
                            a.sample_qty,                                                             -- 试样数量
                            nvl(a.item_test_result,'') item_test_result,                              -- 检验结果
                            (case a.submission_status 
                                    when '0' then '未提交' else '已提交' end) submission_status,      -- 提交状态
                            a.art_d_remark,
                            a.remark as item_remark
                            from qcm_ex_task_list_d a
                            left join sys001m b on a.judgment_criteria=b.enum_code and b.enum_type='enum_judgment_criteria'
                            where a.task_no=@task_no and judgment_criteria<>6";
                if (!string.IsNullOrEmpty(inspection_code) && !string.IsNullOrEmpty(inspection_type))
                {
                    sql = sql + " and a.inspection_type=@inspection_type and a.inspection_code=@inspection_code";

                    Parameters.Add("inspection_type", inspection_type);
                    Parameters.Add("inspection_code", inspection_code);
                }
                int rowCount = CommonBASE.GetPageDataTableCount(DB, sql, Parameters);
                DataTable dt_qcm_ex_task_list_d = CommonBASE.GetPageDataTable(DB, sql, int.Parse(pageIndex), int.Parse(pageSize), "", Parameters);
                foreach (DataRow dr in dt_qcm_ex_task_list_d.Rows)
                {
                    dic_item = new Dictionary<string, object>();
                    dic_item.Add("d_id", dr["id"].ToString());
                    dic_item.Add("inspection_code", dr["inspection_code"].ToString());
                    dic_item.Add("inspection_name", dr["inspection_name"].ToString());
                    dic_item.Add("judge_mode", dr["enum_value"].ToString());
                    dic_item.Add("judgment_criteria", dr["judgment_criteria"].ToString());
                    dic_item.Add("standard_value", dr["standard_value"].ToString());
                    dic_item.Add("sample_qty", dr["sample_qty"].ToString());
                    dic_item.Add("item_test_result", dr["item_test_result"] == null ? "" : dr["item_test_result"]);
                    dic_item.Add("submission_status", dr["submission_status"].ToString());
                    dic_item.Add("art_d_remark", dr["art_d_remark"].ToString());
                    dic_item.Add("item_remark", dr["item_remark"].ToString());
                    lst_dic_item.Add(dic_item);
                }
                #endregion

                retData.Add("HeadData", dic_qcm_ex_task_list_m);
                retData.Add("ItemData", lst_dic_item);
                retData.Add("ItemRowCount", rowCount);
                ret.RetData1 = retData;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            return ret;
        }


        public static DataTable GetInspectionDataTable(SJeMES_Framework_NETCore.DBHelper.DataBase DB)
        {
            string sql = @"
                            select 0 as ty,bdm_insp_fin_shoes_m.* from bdm_insp_fin_shoes_m
                            union all 
                            select 1 as ty,bdm_inspection_backing_parts_m.* from bdm_inspection_backing_parts_m
                            union all 
                            select 2 as ty,bdm_sewing_machine_upper_m.* from bdm_sewing_machine_upper_m
                            union all 
                            select 3 as ty,bdm_cutting_fabric_parts_m.* from bdm_cutting_fabric_parts_m
                            union all 
                            select 4 as ty,bdm_packing_inspection_m.* from bdm_packing_inspection_m
                            union all 
                            select 5 as ty,bdm_workmanship_inspection_m.* from bdm_workmanship_inspection_m
                            union all 
                            select 6 as ty,bdm_material_inspection_m.* from bdm_material_inspection_m
                            union all 
                            select 7 as ty,bdm_finished_shoes_aql_m.* from bdm_finished_shoes_aql_m
                            ";
            DataTable dt = DB.GetDataTable(sql);
            return dt;
        }

        /// <summary>
        /// 运行计算公式接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CalculationFormula_old(object OBJ)
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
                string TestItemCode = jarr.ContainsKey("TestItemCode") ? jarr["TestItemCode"].ToString() : "";
                string FormulaVal = jarr.ContainsKey("FormulaVals") ? jarr["FormulaVals"].ToString() : "";
                List<List<string>> FormulaVals = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(FormulaVal);

                //截取字符
                string[] arr = TestItemCode.Split("@");

                //通用公式代号
                string Currency_formula = string.Empty;
                //自定义公式代号
                string Custom_formula = string.Empty;
                //通用判断标准
                string TYPD = string.Empty;
                //通用测量标准值
                string TYCL = string.Empty;
                //定制判断标准
                string DZPD = string.Empty;
                //定制测量标准值
                string DZCL = string.Empty;
                string sql = string.Empty;
                if (arr.Length > 0)
                {
                    sql = "select * from qcm_inspection_laboratory_d where inspection_no='" + arr[0] + "' and testitem_code='" + arr[1] + "'";
                }
                DataTable dd = DB.GetDataTable(sql);
                foreach (DataRow item in dd.Rows)
                {
                    Currency_formula = item["Currency_formula"].ToString();
                    Custom_formula = item["Custom_formula"].ToString();
                    TYPD = item["t_check_item"].ToString();
                    TYCL = item["t_check_value"].ToString();
                    DZPD = item["d_check_item"].ToString();
                    DZCL = item["d_check_value"].ToString();
                }

                //公式内容为空弹出
                if (string.IsNullOrEmpty(Currency_formula) || string.IsNullOrEmpty(Custom_formula))
                {
                    ret.ErrMsg = "计算公式无数据!";
                    ret.IsSuccess = false;
                    return ret;
                }
                //通用公式内容
                string Formula_contentTY = DB.GetString(@"select formula_content from bdm_formula_m where formula_code='" + Currency_formula + "'");
                //自定义公式内容
                string Formula_contentZDY = DB.GetString(@"select formula_content from bdm_formula_m where formula_code='" + Custom_formula + "'");

                //结果
                List<string> T_check_item = new List<string>();
                string ZDYcontent = string.Empty;//自定义公式替换
                string demo = string.Empty;
                int aa = 0;
                int s = 1;
                for (int i = 0; i < FormulaVals.Count; i++)
                {
                    for (int b = 0; b < Formula_contentZDY.Length; b++)
                    {
                        string aaa = Formula_contentZDY.Substring(b, 1);
                        if (aaa == "N")
                        {
                            demo += FormulaVals[i][aa];
                            aa++;
                        }
                        else
                        {
                            demo += aaa;
                        }

                    }
                    aa = 0;
                    #region 放数据到内容表
                    string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//操作人
                    string date = DateTime.Now.ToString("yyyy-MM-dd");//年月日
                    string time = DateTime.Now.ToString("HH-mm-ss");//时分秒
                    string IsExist = DB.GetString(@"select count(1) from qcm_inspection_laboratory_item where inspection_no='" + arr[0] + "' and testitem_code='" + arr[1] + "' and testitem_code_seq='" + s + "'");
                    string sqlItem = string.Empty;
                    if (IsExist != "0")
                    {
                        sqlItem = @"update qcm_inspection_laboratory_item set formula_content='" + demo + "',modifyby='" + user + "',modifydate='" + date + "',modifytime='" + time + "' where inspection_no='" + arr[0] + "' and testitem_code='" + arr[1] + "' and testitem_code_seq='" + s + "'";
                        DB.ExecuteNonQuery(sqlItem);
                        s++;
                    }
                    else
                    {
                        sqlItem = @"insert into qcm_inspection_laboratory_item (inspection_no,testitem_code,testitem_code_seq,formula_content,createby,createdate,createtime) values('" + arr[0] + "','" + arr[1] + "','" + s + "','" + demo + "','" + user + "','" + date + "','" + time + "')";
                        DB.ExecuteNonQuery(sqlItem);
                        s++;
                    }
                    #endregion
                    if (demo != "")
                    {
                        T_check_item.Add(CalcByDataTable(demo).ToString());
                    }
                    demo = string.Empty;
                }
                string mm = T_check_item[0];
                string Result = string.Empty;//结果
                double a = 0;
                for (int i = 0; i < T_check_item.Count; i++)
                {
                    switch (Currency_formula)
                    {
                        //最大值
                        case enum_general_formula.enum_general_formula_1:
                            Result = Convert.ToDouble(T_check_item[i]) > Convert.ToDouble(mm) ? T_check_item[i] : mm;
                            break;
                        //最小值
                        case enum_general_formula.enum_general_formula_2:
                            Result = Convert.ToDouble(T_check_item[i]) < Convert.ToDouble(mm) ? T_check_item[i] : mm;
                            break;
                        //平均值
                        case enum_general_formula.enum_general_formula_0:
                            a = Convert.ToDouble(T_check_item[i]) + a;
                            Result = (a / T_check_item.Count).ToString();
                            break;
                        //极差值
                        case enum_general_formula.enum_general_formula_3:
                            double DA = Convert.ToDouble(T_check_item[i]) > Convert.ToDouble(mm) ? Convert.ToDouble(T_check_item[i]) : Convert.ToDouble(mm);
                            double XIAO = Convert.ToDouble(T_check_item[i]) < Convert.ToDouble(mm) ? Convert.ToDouble(T_check_item[i]) : Convert.ToDouble(mm);
                            Result = (DA - XIAO).ToString();
                            break;
                        default:
                            break;
                    }
                }

                //检测标准
                string Standard = string.Empty;//检测标准
                string TestPD = string.Empty;//检测结果
                if (!string.IsNullOrEmpty(DZCL))
                {
                    switch (DZPD)
                    {
                        //大于
                        case enum_judge_symbol.enum_judge_symbol_1:
                            Standard = ">" + DZCL;
                            TestPD = Convert.ToDouble(Result) > Convert.ToDouble(DZCL) ? "PASS" : "FAIL";
                            break;
                        //小于
                        case enum_judge_symbol.enum_judge_symbol_3:
                            Standard = "<" + DZCL;
                            TestPD = Convert.ToDouble(Result) < Convert.ToDouble(DZCL) ? "PASS" : "FAIL";
                            break;
                        //等于
                        case enum_judge_symbol.enum_judge_symbol_5:
                            Standard = "=" + DZCL;
                            TestPD = Convert.ToDouble(Result) == Convert.ToDouble(DZCL) ? "PASS" : "FAIL";
                            break;
                        //大于等于
                        case enum_judge_symbol.enum_judge_symbol_2:
                            Standard = ">=" + DZCL;
                            TestPD = Convert.ToDouble(Result) >= Convert.ToDouble(DZCL) ? "PASS" : "FAIL";
                            break;
                        //小于等于
                        case enum_judge_symbol.enum_judge_symbol_4:
                            Standard = "<=" + DZCL;
                            TestPD = Convert.ToDouble(Result) <= Convert.ToDouble(DZCL) ? "PASS" : "FAIL";
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (TYPD)
                    {
                        //大于
                        case enum_judge_symbol.enum_judge_symbol_1:
                            Standard = ">" + TYCL;
                            TestPD = Convert.ToDouble(Result) > Convert.ToDouble(TYCL) ? "PASS" : "FAIL";
                            break;
                        //小于
                        case enum_judge_symbol.enum_judge_symbol_3:
                            Standard = "<" + TYCL;
                            TestPD = Convert.ToDouble(Result) < Convert.ToDouble(TYCL) ? "PASS" : "FAIL";
                            break;
                        //等于
                        case enum_judge_symbol.enum_judge_symbol_5:
                            Standard = "=" + TYCL;
                            TestPD = Convert.ToDouble(Result) == Convert.ToDouble(TYCL) ? "PASS" : "FAIL";
                            break;
                        //大于等于
                        case enum_judge_symbol.enum_judge_symbol_2:
                            Standard = ">=" + TYCL;
                            TestPD = Convert.ToDouble(Result) >= Convert.ToDouble(TYCL) ? "PASS" : "FAIL";
                            break;
                        //小于等于
                        case enum_judge_symbol.enum_judge_symbol_4:
                            Standard = "<=" + TYCL;
                            TestPD = Convert.ToDouble(Result) <= Convert.ToDouble(TYCL) ? "PASS" : "FAIL";
                            break;
                        default:
                            break;
                    }
                }
                #endregion
                Dictionary<string, object> ReturnValue = new Dictionary<string, object>();
                Dictionary<string, object> RR = new Dictionary<string, object>();
                RR.Add("Result", Result);
                RR.Add("Standard", Standard);
                RR.Add("TestPD", TestPD);
                ReturnValue.Add("T_check_item", T_check_item);
                ReturnValue.Add("ItemData", RR);

                ret.RetData1 = ReturnValue;
                ret.IsSuccess = true;
                DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = true;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }
        //字符串计算结果
        internal static decimal CalcByDataTable(string expression)
        {
            object result = new DataTable().Compute(expression, "");
            return decimal.Parse(result + "");
        }


        /// <summary>
        /// 运行计算公式接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject CalculationFormula(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string sql = string.Empty;
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                DB.Open();
                DB.BeginTransaction();

                string d_id = jarr.ContainsKey("d_id") ? jarr["d_id"].ToString() : "";
                string FormulaVal = jarr.ContainsKey("LstFormulaParameters") ? jarr["LstFormulaParameters"].ToString() : "";
                if (string.IsNullOrEmpty(d_id))
                    throw new Exception("接口参数不能为空，请检查！");


                string date = DateTime.Now.ToString("yyyy-MM-dd");//年月日
                string time = DateTime.Now.ToString("HH:mm:ss");//时分秒
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//操作人

                List<List<string>> FormulaVals = Newtonsoft.Json.JsonConvert.DeserializeObject<List<List<string>>>(FormulaVal);
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                sql = @"select * from qcm_ex_task_list_d where id=@d_id";
                Parameters.Add("d_id", d_id);
                Dictionary<string, object> dic_qcm_ex_task_list_d = DB.GetDictionary(sql, Parameters);
                if (dic_qcm_ex_task_list_d == null || dic_qcm_ex_task_list_d.Count == 0)
                    throw new Exception("数据不存在，请返回刷新界面后重试！");
                string task_no = dic_qcm_ex_task_list_d["TASK_NO"].ToString();


                //查询自定义公式
                sql = $@"SELECT formula_content,formula_name FROM bdm_formula_m  WHERE  FORMULA_CODE = @FORMULA_CODE ";
                Parameters.Clear();
                Parameters.Add("FORMULA_CODE", dic_qcm_ex_task_list_d["D_FORMULA_CODE"].ToString());
                Dictionary<string, object> dic_bdm_formula_m = DB.GetDictionary(sql, Parameters);

                if (dic_bdm_formula_m == null || dic_bdm_formula_m.Count == 0)
                    throw new Exception($"公式代号【{dic_qcm_ex_task_list_d["D_FORMULA_CODE"].ToString()}】信息不存在，请检查！");

                //自定义的公式内容
                string formula_content = dic_bdm_formula_m["FORMULA_CONTENT"].ToString();

                //公式
                string formulaTxt = string.Empty;
                //计算结果
                decimal calculation_result = 0;
                //运算结果集
                List<decimal> T_check_item = new List<decimal>();

                for (int i = 0; i < FormulaVals.Count; i++)
                {
                    formulaTxt = string.Empty;
                    if (FormulaVals[i].Count > 0)
                    {
                        int index = 0;
                        for (int b = 0; b < formula_content.Length; b++)
                        {
                            string charF = formula_content[b].ToString();
                            if (charF == "N")
                            {
                                formulaTxt += decimal.Parse(FormulaVals[i][index]);
                                index++;
                            }
                            else
                            {
                                formulaTxt += charF;
                            }
                        }

                        if (!string.IsNullOrEmpty(formulaTxt))
                            calculation_result = CalcByDataTable(formulaTxt);

                        #region 放数据到内容表
                        sql = @"select count(1) from qcm_ex_task_item where d_id=@d_id and item_seq=@item_seq";
                        Parameters = new Dictionary<string, object>();
                        Parameters.Add("d_id", d_id);
                        Parameters.Add("item_seq", i + 1);
                        string IsExist = DB.GetString(sql, Parameters);
                        if (IsExist != "0")
                        {
                            sql = @"
                                    update 
                                        qcm_ex_task_item 
                                    set 
                                        formula_content=@formula_content,
                                        modifyby=@modifyby,
                                        modifydate=@modifydate,
                                        modifytime=@modifytime,
                                        formula_parameters=@formula_parameters,
                                        calculation_result=@calculation_result
                                    where 
                                        d_id=@d_id and item_seq=@item_seq";
                            Parameters = new Dictionary<string, object>();
                            Parameters.Add("formula_content", formulaTxt);
                            Parameters.Add("modifyby", user);
                            Parameters.Add("modifydate", date);
                            Parameters.Add("modifytime", time);
                            Parameters.Add("formula_parameters", string.Join(",", FormulaVals[i]));
                            Parameters.Add("calculation_result", calculation_result);
                            Parameters.Add("d_id", d_id);

                            Parameters.Add("item_seq", i + 1);
                            DB.ExecuteNonQuery(sql, Parameters);
                        }
                        else
                        {
                            sql = @"insert into qcm_ex_task_item (task_no,d_id,item_seq,formula_content,formula_parameters,calculation_result,createby,createdate,createtime) 
                                    values(@task_no,@d_id,@item_seq,@formula_content,@formula_parameters,@calculation_result,@createby,@createdate,@createtime)";
                            Parameters = new Dictionary<string, object>();
                            Parameters.Add("task_no", task_no);
                            Parameters.Add("d_id", d_id);
                            Parameters.Add("item_seq", i + 1);
                            Parameters.Add("formula_content", formulaTxt);
                            Parameters.Add("formula_parameters", string.Join(",", FormulaVals[i]));
                            Parameters.Add("calculation_result", calculation_result);
                            Parameters.Add("createby", user);
                            Parameters.Add("createdate", date);
                            Parameters.Add("createtime", time);
                            DB.ExecuteNonQuery(sql, Parameters);
                        }
                        #endregion

                        T_check_item.Add(decimal.Parse(calculation_result.ToString("0.##")));
                    }
                    else
                    {
                        T_check_item.Add(0);
                    }
                }

                //运算结果
                decimal Result = 0;
                string G_FORMULA_CODE = dic_qcm_ex_task_list_d["G_FORMULA_CODE"].ToString();
                switch (G_FORMULA_CODE)
                {
                    //最大值
                    case enum_general_formula.enum_general_formula_1:
                        for (int i = 0; i < T_check_item.Count; i++)
                        {
                            if (i == 0)
                                Result = Convert.ToDecimal(T_check_item[i]);
                            else
                                Result = Convert.ToDecimal(T_check_item[i]) > Result ? Convert.ToDecimal(T_check_item[i]) : Result;
                        }
                        break;
                    //最小值
                    case enum_general_formula.enum_general_formula_2:
                        for (int i = 0; i < T_check_item.Count; i++)
                        {
                            if (i == 0)
                                Result = Convert.ToDecimal(T_check_item[i]);
                            else
                                Result = Convert.ToDecimal(T_check_item[i]) < Result ? Convert.ToDecimal(T_check_item[i]) : Result;
                        }
                        break;
                    //平均值
                    case enum_general_formula.enum_general_formula_0:
                        for (int i = 0; i < T_check_item.Count; i++)
                        {
                            Result = Result + Convert.ToDecimal(T_check_item[i]);
                        }
                        Result = (Result / T_check_item.Count);
                        break;
                    //极差值
                    case enum_general_formula.enum_general_formula_3:
                        decimal minval = 0;
                        decimal maxval = 0;

                        #region 旧的
                        /* for (int i = 0; i < T_check_item.Count; i++)
                                        {
                                            if (i == 0)
                                                Result = Convert.ToDecimal(T_check_item[i]);

                                            if (Convert.ToDecimal(T_check_item[i]) > Result)
                                            {
                                                maxval = Convert.ToDecimal(T_check_item[i]);
                                                minval = Result;
                                            }
                                            else
                                            {
                                                maxval = Result;
                                                minval = Convert.ToDecimal(T_check_item[i]);
                                            } 
                                        } */
                        #endregion
                        //2022.3.30ltj
                        if (T_check_item.Count > 0)
                        {
                            T_check_item.Sort();
                            Result = T_check_item[T_check_item.Count - 1];
                            minval = T_check_item.Count > 1 ? T_check_item[0] : 0;
                            maxval = Result;
                        }
                        Result = maxval - minval;
                        break;
                    default:
                        break;
                }

                //判定结果
                string TestResult = string.Empty;
                //判断方式
                string JUDGMENT_CRITERIA = dic_qcm_ex_task_list_d["JUDGMENT_CRITERIA"].ToString();
                //判断类型
                string JUDGE_TYPE = dic_qcm_ex_task_list_d["JUDGE_TYPE"].ToString();
                if(string.IsNullOrEmpty(dic_qcm_ex_task_list_d["JUDGE_TYPE"].ToString()))
                {
                    JUDGE_TYPE = enum_testitem_type.enum_testitem_type_1;
                }
                //判断标准值
                string STANDARD_VALUE = dic_qcm_ex_task_list_d["STANDARD_VALUE"].ToString();
                decimal value1 = 0;
                decimal value2 = 0;
                var standard_list = STANDARD_VALUE.Split('~').ToList();
                decimal.TryParse(standard_list[0], out value1);
                if (standard_list.Count > 1)
                {
                    decimal.TryParse(standard_list[1], out value2);
                }

                switch (JUDGMENT_CRITERIA)
                {
                    //大于
                    case enum_judgment_criteria.enum_judgment_criteria_1:
                        switch (JUDGE_TYPE)
                        {
                            case enum_testitem_type.enum_testitem_type_1:
                                TestResult = Convert.ToDecimal(Result) > Convert.ToDecimal(STANDARD_VALUE) ? "PASS" : "FAIL";
                                break;
                            case enum_testitem_type.enum_testitem_type_2:
                                TestResult = Convert.ToDecimal(Result) > value2 ? "PASS" : "FAIL";  //>上限
                                break;
                            case enum_testitem_type.enum_testitem_type_3:
                                TestResult = Convert.ToDecimal(Result) > value1 + value2 ? "PASS" : "FAIL";//>上限
                                break;
                        }

                        break;
                    //小于
                    case enum_judgment_criteria.enum_judgment_criteria_2:
                        switch (JUDGE_TYPE)
                        {
                            case enum_testitem_type.enum_testitem_type_1:
                                TestResult = Convert.ToDecimal(Result) < Convert.ToDecimal(STANDARD_VALUE) ? "PASS" : "FAIL";
                                break;
                            case enum_testitem_type.enum_testitem_type_2:
                                TestResult = Convert.ToDecimal(Result) < value1 ? "PASS" : "FAIL";  //>下限
                                break;
                            case enum_testitem_type.enum_testitem_type_3:
                                TestResult = Convert.ToDecimal(Result) < value1 - value2 ? "PASS" : "FAIL";//>上限
                                break;
                        }

                        break;
                    //等于
                    case enum_judgment_criteria.enum_judgment_criteria_5:
                        switch (JUDGE_TYPE)
                        {
                            case enum_testitem_type.enum_testitem_type_1:
                                TestResult = Convert.ToDecimal(Result) == Convert.ToDecimal(STANDARD_VALUE) ? "PASS" : "FAIL";
                                break;
                            case enum_testitem_type.enum_testitem_type_2:
                                TestResult = (Convert.ToDecimal(Result) >= value1 && Convert.ToDecimal(Result) <= value2) ? "PASS" : "FAIL";  //>下限
                                break;
                            case enum_testitem_type.enum_testitem_type_3:
                                TestResult = (Convert.ToDecimal(Result) >= value1-value2 && Convert.ToDecimal(Result) <= value1+ value2) ? "PASS" : "FAIL";  //>下限
                                break;
                        }

                        break;
                    //大于等于
                    case enum_judgment_criteria.enum_judgment_criteria_3:
                        switch (JUDGE_TYPE)
                        {
                            case enum_testitem_type.enum_testitem_type_1:
                                TestResult = Convert.ToDecimal(Result) >= Convert.ToDecimal(STANDARD_VALUE) ? "PASS" : "FAIL";
                                break;
                            case enum_testitem_type.enum_testitem_type_2:
                                TestResult = Convert.ToDecimal(Result) >= value2 ? "PASS" : "FAIL";  //>上限
                                break;
                            case enum_testitem_type.enum_testitem_type_3:
                                TestResult = Convert.ToDecimal(Result) >= value1 + value2 ? "PASS" : "FAIL";//>上限
                                break;
                        }
                        break;
                    //小于等于
                    case enum_judgment_criteria.enum_judgment_criteria_4:
                        switch (JUDGE_TYPE)
                        {
                            case enum_testitem_type.enum_testitem_type_1:
                                TestResult = Convert.ToDecimal(Result) <= Convert.ToDecimal(STANDARD_VALUE) ? "PASS" : "FAIL";
                                break;
                            case enum_testitem_type.enum_testitem_type_2:
                                TestResult = Convert.ToDecimal(Result) <= value1 ? "PASS" : "FAIL";  //>下限
                                break;
                            case enum_testitem_type.enum_testitem_type_3:
                                TestResult = Convert.ToDecimal(Result) <= value1 - value2 ? "PASS" : "FAIL";//>上限
                                break;
                        }
                        break;
                    default:
                        break;
                }

                Dictionary<string, object> ReturnValue = new Dictionary<string, object>();
                ReturnValue.Add("Result", Result.ToString("0.##"));
                ReturnValue.Add("TestResult", TestResult);
                ReturnValue.Add("TestItems", T_check_item);
                ret.RetData1 = ReturnValue;
                ret.IsSuccess = true;
                DB.Commit();//提交事务
            }
            catch (Exception ex)
            {
                DB.Rollback();//回滚事务
                ret.IsSuccess = true;
                ret.ErrMsg = "00000:" + ex.Message;
            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;
        }


        /// <summary>
        /// 检验内容提交接口
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SubimitInspectionData(object OBJ)
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
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 逻辑
                string sql = string.Empty;
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//操作人
                string date = DateTime.Now.ToString("yyyy-MM-dd");//年月日
                string time = DateTime.Now.ToString("HH:mm:ss");//时分秒

                string d_id = jarr.ContainsKey("d_id") ? jarr["d_id"].ToString() : "";
                string item_remark = jarr.ContainsKey("item_remark") ? jarr["item_remark"].ToString() : "";
                string strDetailedData = jarr.ContainsKey("list") ? jarr["list"].ToString() : "";
                string Result = jarr.ContainsKey("Result") ? jarr["Result"].ToString() : "";//综合计算结果
                string TestResult = jarr.ContainsKey("TestResult") ? jarr["TestResult"].ToString() : "";//检测结果（PASS/FAIL）

                if (string.IsNullOrEmpty(d_id))
                    throw new Exception("接口参数不能为空，请检查！");
                List<Dictionary<string, object>> dicDetailedData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(strDetailedData);
                if (dicDetailedData.Count == 0)
                    throw new Exception("提交检验明细数据信息不能为空，请检查！");
                if (string.IsNullOrEmpty(Result))
                    throw new Exception("提交的综合计算结果不能为空，请检查！");
                if (string.IsNullOrEmpty(TestResult))
                    throw new Exception("提交的检测结果不能为空，请检查！");

                sql = @"select * from qcm_ex_task_list_d where id=@d_id";
                Parameters.Add("d_id", d_id);
                Dictionary<string, object> dic_qcm_ex_task_list_d = DB.GetDictionary(sql, Parameters);
                if (dic_qcm_ex_task_list_d == null || dic_qcm_ex_task_list_d.Count == 0)
                    throw new Exception("数据不存在，请返回刷新界面后重试！");
                string task_no = dic_qcm_ex_task_list_d["TASK_NO"].ToString();



                //首次提交检测日期  qcm_ex_task_list_m
                sql = $@"update qcm_ex_task_list_m set submission_date='{date}' where task_no='{task_no}'";
                DB.ExecuteNonQuery(sql);

                //修改检验项的提交状态，检测结果
                sql = @"update qcm_ex_task_list_d set submission_status=@submission_status,item_test_val=@item_test_val,item_test_result=@item_test_result,remark=@remark where id = @id";
                Parameters = new Dictionary<string, object>();
                Parameters.Add("submission_status", "1");
                Parameters.Add("item_test_val", Result);
                Parameters.Add("item_test_result", TestResult);
                Parameters.Add("remark", item_remark);
                Parameters.Add("id", d_id);
                DB.ExecuteNonQuery(sql, Parameters);

                //记录设备，图片信息等
                int seq = 0;
                foreach (Dictionary<string, object> item in dicDetailedData)
                {
                    seq++;
                    sql = @"select id from qcm_ex_task_item where d_id=@d_id and item_seq=@item_seq";
                    Parameters = new Dictionary<string, object>();
                    Parameters.Add("d_id", d_id);
                    Parameters.Add("item_seq", seq);
                    string item_id = DB.GetString(sql, Parameters);
                    if (string.IsNullOrEmpty(item_id))
                    {
                        //新增 qcm_ex_task_item 
                        sql = @"insert into qcm_ex_task_item(task_no,d_id,item_seq,eq_info_no,eq_info_name,remarks)
                                    values(@task_no,@d_id,@item_seq,@eq_info_no,@eq_info_name,@remarks)";
                        Parameters = new Dictionary<string, object>();
                        Parameters.Add("task_no", task_no);
                        Parameters.Add("d_id", d_id);
                        Parameters.Add("item_seq", seq);
                        Parameters.Add("eq_info_no", item["eq_info_no"].ToString());
                        Parameters.Add("eq_info_name", item["eq_info_name"].ToString());
                        Parameters.Add("remarks", item["remarks"].ToString());
                        DB.ExecuteNonQuery(sql, Parameters);
                        item_id = SJeMES_Framework_NETCore.DBHelper.DataBase.GetCurId(DB, "qcm_ex_task_item");
                    }
                    else
                    {
                        //修改 qcm_ex_task_item
                        sql = @"update qcm_ex_task_item set eq_info_no=@eq_info_no,eq_info_name=@eq_info_name,remarks=@remarks
                                        where id=@item_id";
                        Parameters = new Dictionary<string, object>();
                        Parameters.Add("eq_info_no", item["eq_info_no"].ToString());
                        Parameters.Add("eq_info_name", item["eq_info_name"].ToString());
                        Parameters.Add("remarks", item["remarks"].ToString());
                        Parameters.Add("item_id", item_id);
                        DB.ExecuteNonQuery(sql, Parameters);
                    }

                    //图片信息[{guid:"",url:""},{}]
                    List<Dictionary<string, string>> lstImg = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(item["lstImg"].ToString());

                    //删除
                    sql = @"delete from qcm_ex_task_item_img where item_id=@item_id";
                    Parameters = new Dictionary<string, object>();
                    Parameters.Add("item_id", item_id);
                    DB.ExecuteNonQuery(sql, Parameters);
                    if (lstImg.Count > 0)
                    {

                        foreach (Dictionary<string, string> dicImg in lstImg)
                        {
                            sql = @"insert into qcm_ex_task_item_img(task_no,item_id,img_guid,createby,createdate,createtime)
                                        values(@task_no,@item_id,@img_guid,@createby,@createdate,@createtime)";
                            Parameters = new Dictionary<string, object>();
                            Parameters.Add("task_no", task_no);
                            Parameters.Add("item_id", item_id);
                            Parameters.Add("img_guid", dicImg["guid"].ToString());
                            Parameters.Add("createby", user);
                            Parameters.Add("createdate", date);
                            Parameters.Add("createtime", time);
                            DB.ExecuteNonQuery(sql, Parameters);
                        }
                    }
                }



                //更新检验单的结果和完结日期  test_result   completion_date
                sql = $@"select count(1) from qcm_ex_task_list_d where task_no='{task_no}' and  nvl(item_test_result,'1')='1'";
                Parameters = new Dictionary<string, object>();
                Parameters.Add("d_id", d_id);
                if (DB.GetStringline(sql).Equals("0"))
                {
                    string test_result = "";
                    sql = $@"select count(1) from qcm_ex_task_list_d where task_no='{task_no}' and item_test_result='FAIL'";
                    if (!DB.GetStringline(sql).Equals("0"))
                        test_result = "FAIL";
                    else
                        test_result = "PASS";

                    sql = $@"update qcm_ex_task_list_m set test_result=@test_result,completion_date=@completion_date where task_no=@task_no";
                    Parameters = new Dictionary<string, object>();
                    Parameters.Add("test_result", test_result);
                    Parameters.Add("completion_date", date);
                    Parameters.Add("task_no", task_no);
                    DB.ExecuteNonQuery(sql, Parameters);
                }

                #endregion

                ret.IsSuccess = true;
                ret.ErrMsg = "提交成功！";
                DB.Commit();//提交事务
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

        /// <summary>
        /// 发送邮件通知
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SendInspectionDataEmail(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                #region 逻辑
                string sql = string.Empty;
                Dictionary<string, object> Parameters = new Dictionary<string, object>();
                string user = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//操作人
                string date = DateTime.Now.ToString("yyyy-MM-dd");//年月日
                string time = DateTime.Now.ToString("HH:mm:ss");//时分秒

                string d_id = jarr.ContainsKey("d_id") ? jarr["d_id"].ToString() : "";

                if (string.IsNullOrEmpty(d_id))
                    throw new Exception("接口参数不能为空，请检查！");

                var dt = DB.GetDataTable($@"
SELECT
	A.TASK_NO,-- 实验室编号
	A .inspection_code,-- 检验项编号
	NVL (A .item_test_result, '') item_test_result,-- 检验结果
	b.enum_value,-- 判断标准
	A .standard_value,-- 测量标准(定制)
	A .art_d_remark,--测试项备注
	A .sample_qty,-- 试样数量
    A .remark-- 备注
FROM
	qcm_ex_task_list_d A
LEFT JOIN sys001m b ON A .judgment_criteria = b.enum_code AND b.enum_type = 'enum_judgment_criteria'
WHERE
	A .id={d_id}
");
                if (dt.Rows.Count > 0)
                {
                    var fRow = dt.Rows[0];
                    string body = $@"实验室编号为：{fRow["TASK_NO"]}的{fRow["inspection_code"]}检验项，已经产生结果，实验结果为{fRow["item_test_result"]}<br />
判断标准：{fRow["enum_value"]}<br />
测量标准：{fRow["standard_value"]}<br />
测试项备注：{fRow["art_d_remark"]}<br />
试样数量：{fRow["sample_qty"]}<br />
测试结果备注：{fRow["remark"]}
";
                    var email = DB.GetString($@"
SELECT
	HR.STAFF_EMAIL
FROM
	QCM_EX_TASK_LIST_M m
INNER JOIN HR001M hr ON HR.STAFF_NO=m.STAFF_NO
WHERE
	m.TASK_NO = '{fRow["TASK_NO"]}'");
                    var staff_name = DB.GetString($@"
SELECT
	m.STAFF_NAME
FROM
	QCM_EX_TASK_LIST_M m
WHERE
	m.TASK_NO = '{fRow["TASK_NO"]}'
");
                    if (String.IsNullOrEmpty(email))
                    {
                        ret.IsSuccess = false;
                        ret.ErrMsg = $@"送测人【{staff_name}】未设置邮箱";
                    }
                    else
                    {

                        var sendToList = new List<string>();
                        sendToList.Add(email);
                        var sendCCList = new List<string>();
                        string errorMsg = "";
                        bool res = MailUtil.SendMessage(sendToList, sendCCList, "实验室单项结果推送", body, null, out errorMsg);
                        if (res)
                            ret.IsSuccess = true;
                        else
                        {
                            ret.IsSuccess = false;
                            ret.ErrMsg = errorMsg;
                        }
                    }
                }
                else
                {
                    ret.IsSuccess = false;
                    ret.ErrMsg = "测试项不存在";
                }

                #endregion
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = "00000:" + ex.Message;

            }
            finally
            {
            }
            return ret;
        }

    }
}
