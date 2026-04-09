using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI
{
    public class ProdBASE
    {


        /// <summary>
        /// ART定制检测项明细获取一级页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTitle(object OBJ)
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
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART编号

                string count = DB.GetString($@"select count(1) from bdm_prod_m where prod_no='{prod_no}'");//判断是否存在
                DataTable dd = DB.GetDataTable($@"select * from bdm_general_testtype_m");//通用品质类型表总数据
                DataTable tt = DB.GetDataTable($@"select * from bdm_prod_customquality_m where general_testtype_no not IN (select general_testtype_no from bdm_general_testtype_m) and prod_no='{prod_no}'");
                string sql = string.Empty;//查询页签
                DataTable dt = new DataTable();
                string where = string.Empty;//条件
                                            //判断是否第一次进来

                string sep1 = string.Empty;
                string sep2 = string.Empty;
                if (count == "0")
                {
                   
                    DB.ExecuteNonQuery($@"insert into bdm_prod_m (prod_no) values('{prod_no}')");
                    foreach (DataRow item in dd.Rows)
                    {
                        sep1+= $@"into bdm_prod_customquality_m (prod_no,general_testtype_no,general_testtype_name) values('{prod_no}','{item["general_testtype_no"]}','{item["general_testtype_name"]}')";
                    } 
                }
                //判断数据是否同步
                if (count != "0")
                {
                    foreach (DataRow item in dd.Rows)
                    {
                        string gg = DB.GetString($@"select count(1) from bdm_prod_customquality_m where general_testtype_no='{item["general_testtype_no"]}' and prod_no='{prod_no}'");
                        if (gg == "0")
                        {
                            sep2 += $@"into bdm_prod_customquality_m (prod_no,general_testtype_no,general_testtype_name) values('{prod_no}','{item["general_testtype_no"]}','{item["general_testtype_name"]}')";
                         
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sep1))
                {
                    string sep11 = "insert all " + sep1 + " select*from dual";
                    DB.ExecuteNonQuery(sep11);
                }
                if (!string.IsNullOrEmpty(sep2))
                {
                    string sep22 = "insert all " + sep2 + " select*from dual";
                    DB.ExecuteNonQuery(sep22);
                }
                DB.Commit();//提交事务
                sql = $@"select general_testtype_no,general_testtype_name from bdm_prod_customquality_m where 1=1 and prod_no='{prod_no}'";
                dt = DB.GetDataTable(sql);
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
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
        /// ART定制检测项明细获取二级页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTitle2(object OBJ)
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
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART编号
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";//ART通用检测编号

                //拿到通用类型数据
                string sql = $@"select p.*,g.GENERAL_CATEGORY from bdm_prod_customquality_m p INNER JOIN BDM_GENERAL_TESTTYPE_M g ON p.GENERAL_TESTTYPE_NO=g.general_testtype_no where p.PROD_NO='{prod_no}'";
                DataTable dt = DB.GetDataTable(sql);
                //通过bom主表里的料品编号和物料清单编号拿到料品bom明细表里的类别代号
                string sql2 = $@"SELECT
	                            SUBMATERIAL_NO 
                            FROM
	                            bdm_rd_bom_item 
                            WHERE
	                            prod_no IN (SELECt PROD_NO FROM bdm_rd_bom_m WHERE PROD_NO IN ( SELECT ITEM_NO FROM bdm_rd_item WHERE PARENT_ITEM_NO = '{prod_no}' ) )
	                            AND BOM_NO IN (SELECt BOM_NO FROM bdm_rd_bom_m WHERE PROD_NO IN ( SELECT ITEM_NO FROM bdm_rd_item WHERE PARENT_ITEM_NO = '{prod_no}' ) )
	                            AND SUBMATERIAL_NO not in (select category_no from bdm_prod_customquality_d)
                            GROUP BY
	                            SUBMATERIAL_NO";
                DataTable dt2 = DB.GetDataTable(sql2);
                //拿到料品信息表里的类别名称
                string sql4 = $@"select NAME_T from bdm_rd_item WHERE ITEM_NO in ({sql2})";
                DataTable dt4 = DB.GetDataTable(sql4);
                //
                string sep1 = string.Empty;
                string sep2 = string.Empty;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //通用
                    if (dt.Rows[i]["GENERAL_CATEGORY"].ToString() == enum_formula_type.enum_formula_type_0)
                    {
                        if (dt2.Rows.Count > 0 && dt4.Rows.Count > 0)
                        {
                            for (int a = 0; a < dt2.Rows.Count; a++)
                            {
                                sep1 += $@"into bdm_prod_customquality_d (prod_no,general_testtype_no,category_no,category_name)
                                                  values('{prod_no}','{dt.Rows[i]["general_testtype_no"]}','{dt2.Rows[a]["SUBMATERIAL_NO"]}','{dt4.Rows[a]["NAME_T"]}')";
                              
                            }
                        }
                    }
                    //自定义
                    else if (dt.Rows[i]["GENERAL_CATEGORY"].ToString() == enum_formula_type.enum_formula_type_1)
                    {
                        //查询通用品质二级类别
                        string sql5 = $@"select quality_category_no,quality_category_name from bdm_generalquality_m where general_testtype_no='{dt.Rows[i]["general_testtype_no"]}'";
                        DataTable dt5 = DB.GetDataTable(sql5);
                        for (int a = 0; a < dt5.Rows.Count; a++)
                        {
                            string tt = DB.GetString($@"select COUNT(1) from bdm_prod_customquality_d where prod_no='{prod_no}' and general_testtype_no='{dt.Rows[i]["general_testtype_no"]}' and category_no='{dt5.Rows[a]["quality_category_no"]}'");
                            if (tt == "0")
                            {
                                sep2 += $@"into bdm_prod_customquality_d (prod_no,general_testtype_no,category_no,category_name) 
                                                  values('{prod_no}','{dt.Rows[i]["general_testtype_no"]}','{dt5.Rows[a]["quality_category_no"]}','{dt5.Rows[a]["quality_category_name"]}')";
                                
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sep1))
                {
                    string sep11 = "insert all " + sep1 + " select*from dual";
                    DB.ExecuteNonQuery(sep11);
                }
                if (!string.IsNullOrEmpty(sep2))
                {
                    string sep22 = "insert all " + sep2 + " select*from dual";
                    DB.ExecuteNonQuery(sep22);
                }
               
                DB.Commit();//提交事务
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
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
        /// ART定制检测项明细加载二级页签
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTitle3(object OBJ)
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
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART编号
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";//ART通用检测编号
                //查询品质检验分类
                string sql6 = $@"select category_no,category_name from bdm_prod_customquality_d where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}'";
                DataTable dt = DB.GetDataTable(sql6);

                DB.Commit();//提交事务
                #endregion
                ret.RetData1 = dt;
                ret.IsSuccess = true;
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
        /// 判断数据还是否存在不存在就删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject IsDelete(object OBJ)
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
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//art编号

                //判断没有二级菜单的删除
                DataTable one = DB.GetDataTable($@"
select GENERAL_TESTTYPE_NO from bdm_prod_customquality_m where prod_no='{prod_no}' and GENERAL_TESTTYPE_NO not in (
select GENERAL_TESTTYPE_NO from bdm_prod_customquality_d where prod_no='{prod_no}' )");
                string sep1 = string.Empty;
                foreach (DataRow item in one.Rows)
                {
                    DB.ExecuteNonQuery($@"delete from bdm_prod_customquality_m where prod_no='{prod_no}' and GENERAL_TESTTYPE_NO='{item["GENERAL_TESTTYPE_NO"]}'");
                }
                //查询art编号下的页签
                DataTable dt = DB.GetDataTable($@"select PROD_NO,GENERAL_TESTTYPE_NO,category_no from bdm_prod_customquality_d where prod_no='{prod_no}' ");

                //循环art下的页签
                foreach (DataRow item in dt.Rows)
                {
                    //判断是否有数据
                    string count = DB.GetString($@"select count(1) from bdm_prod_customquality_item where prod_no='{prod_no}' and general_testtype_no='{item["category_no"]}' and category_no='{item["category_no"]}'");
                    if (count == "0")
                    {
                        //判断在通用品质标准中是否存在
                        string countTY = DB.GetString($@"select count(1) from bdm_generalquality_d where general_testtype_no='{item["general_testtype_no"]}' and (quality_category_no='{item["category_no"]}' or secondary_category_no='{item["category_no"]}')");
                        if (countTY == "0")
                        {
                            DB.ExecuteNonQuery($@"delete from bdm_prod_customquality_m where prod_no='{prod_no}' and general_testtype_no='{item["general_testtype_no"]}'");
                            DB.ExecuteNonQuery($@"delete from bdm_prod_customquality_d where prod_no='{prod_no}' and general_testtype_no='{item["general_testtype_no"]}' and category_no='{item["category_no"]}'");
                        }
                    }
                }


                DB.Commit();//提交事务
                #endregion
                //ret.RetData1 = dt;
                ret.IsSuccess = true;
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
        /// ART定制检测项明细给通用类型加上数据
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject InsertTY(object OBJ)
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
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART编号
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";//ART通用检测编号
                //string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";//料品/类别代号
                //子件品号
                string Submaterial_noSQL = $@"SELECT
	                            SUBMATERIAL_NO 
                            FROM
	                            bdm_rd_bom_item 
                            WHERE
	                            prod_no IN (SELECt PROD_NO FROM bdm_rd_bom_m WHERE PROD_NO IN ( SELECT ITEM_NO FROM bdm_rd_item WHERE PARENT_ITEM_NO = '{prod_no}' ) )
	                            AND BOM_NO IN (SELECt BOM_NO FROM bdm_rd_bom_m WHERE PROD_NO IN ( SELECT ITEM_NO FROM bdm_rd_item WHERE PARENT_ITEM_NO = '{prod_no}' ) )
                            GROUP BY
	                            SUBMATERIAL_NO";
                DataTable dt1 = DB.GetDataTable(Submaterial_noSQL);



                //品质检验项目
                string BPCI = $@"select * from bdm_prod_customquality_item";
                DataTable dt3 = DB.GetDataTable(BPCI);

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    string ItemType = DB.GetString($@"select ITEM_TYPE from bdm_rd_item where ITEM_NO='{dt1.Rows[i]["SUBMATERIAL_NO"]}'");
                    if (!string.IsNullOrEmpty(ItemType))
                    {
                        //测试项,外观检测项,试穿项三表数据
                        string cws = $@"
SELECT
	'1' AS TYPENAME,
	TESTITEM_NAME AS 检测类型，
	TESTITEM_CODE  AS 检测项编号，
	TESTITEM_NAME AS 检测项名称,
    testtype_no as 检测项类型,
    testtype_name as 检测项类型名称,
    check_type as 检验项目类型,
	CHECK_ITEM AS	判断标准通用,
	CHECK_VALUE AS 测量标准通用,
	'' AS 判断标准定制,
	'' AS 测量标准定制 ,
	UNIT AS 单位,
	REFERENCE_LEVEL AS 项目引用级别通用 ，
	'' AS 项目引用级别标准 ,
	SAMPLE_NUM AS 试样数量,
	CURRENCY_FORMULA AS 通用公式类型,
	custom_formula AS 自定义公式类型，
	'' AS ART定制备注
FROM
	BDM_QUALITYTEST_ITEM 
	WHERE general_testtype_no='{general_testtype_no}' and secondary_category_no='{ItemType}'
	UNION
SELECT
	'2'  AS TYPENAME ,
	testtype_name AS  检测类型 ,
	testitem_code  AS  检测项编号 ,
	testitem_name AS  检测项名称 ,
    testtype_no as 检测项类型,
    testtype_name as 检测项类型名称,
    '' as 检验项目类型,
	CHECK_ITEM AS	 判断标准通用  ,
		CHECK_VALUE AS  测量标准通用  ,
	'' AS  判断标准定制  ,
	'' AS  测量标准定制  ,
	'' AS  单位 ,
	REFERENCE_LEVEL AS  项目引用级别通用  ,
	'' AS  项目引用级别标准  ,
	SAMPLE_NUM AS  试样数量 ,
	'' AS  通用公式类型 ,
	'' AS  自定义公式类型 ,
	'' AS  ART定制备注 
FROM
	bdm_qualityaptest_item 
		WHERE general_testtype_no='{general_testtype_no}' and secondary_category_no='{ItemType}'
	UNION
	SELECT
	'3' AS TYPENAME,
	TESTITEM_NAME AS  检测类型 ，
	TESTITEM_CODE  AS  检测项编号 ，
	TESTITEM_NAME AS  检测项名称 ,
    testtype_no as 检测项类型,
    testtype_name as 检测项类型名称,
    '' as 检验项目类型,
	CHECK_ITEM AS	 判断标准通用  ,
	CHECK_VALUE AS  测量标准通用  ,
	'' AS  判断标准定制  ,
	'' AS  测量标准定制  ,
	'' AS  单位 ,
	REFERENCE_LEVEL AS  项目引用级别通用  ，
	'' AS  项目引用级别标准  ,
	SAMPLE_NUM AS  试样数量 ,
	'' AS  通用公式类型 ,
	'' AS  自定义公式类型 ，
	'' AS  ART定制备注 
FROM
	bdm_qualitytntest_item
		WHERE general_testtype_no='{general_testtype_no}' and secondary_category_no='{ItemType}'
	";
                        DataTable dt2 = DB.GetDataTable(cws);
                        //通用类别
                        string general_category = DB.GetString($@"select general_category from bdm_general_testtype_m where general_testtype_no='{general_testtype_no}'");
                        //通用
                        if (general_category == enum_formula_type.enum_formula_type_0)
                        {
                            string sep1 = string.Empty;
                            string sep2 = string.Empty;
                            for (int t = 0; t < dt2.Rows.Count; t++)
                            {
                                if (dt3.Rows.Count > 0)
                                {
                                    DataRow[] dt3row = dt3.Select($"testitem_code='{dt2.Rows[t]["检测项编号"].ToString()}'");
                                    if (dt3row.Length == 0)
                                    {
                                        sep1 += $@" into bdm_prod_customquality_item 
                                                          (prod_no,general_testtype_no,category_no,testitem_category,testitem_code,testitem_name,testtype_no,testtype_name,
                                                          sample_num,reference_level,check_type,t_check_item,t_check_value,d_check_item,d_check_value,currency_formula,custom_formula,unit,art_remarks)
                                                          values('{prod_no}','{general_testtype_no}','{dt1.Rows[i]["SUBMATERIAL_NO"]}','{dt2.Rows[t]["TYPENAME"]}','{dt2.Rows[t]["检测项编号"]}',
                                                          '{dt2.Rows[t]["检测项名称"]}','{dt2.Rows[t]["检测项类型"]}','{dt2.Rows[t]["检测项类型名称"]}','{dt2.Rows[t]["试样数量"]}',
                                                          '{dt2.Rows[t]["项目引用级别标准"]}','{dt2.Rows[t]["检验项目类型"]}','{dt2.Rows[t]["判断标准通用"]}','{dt2.Rows[t]["测量标准通用"]}',
                                                          '{dt2.Rows[t]["判断标准定制"]}','{dt2.Rows[t]["测量标准定制"]}','{dt2.Rows[t]["通用公式类型"]}','{dt2.Rows[t]["自定义公式类型"]}','{dt2.Rows[t]["单位"]}','{dt2.Rows[t]["ART定制备注"]}')";
                                       
                                    }
                                }
                                else
                                {
                                    sep2 += $@" into bdm_prod_customquality_item 
                                                          (prod_no,general_testtype_no,category_no,testitem_category,testitem_code,testitem_name,testtype_no,testtype_name,
                                                          sample_num,reference_level,check_type,t_check_item,t_check_value,d_check_item,d_check_value,currency_formula,custom_formula,unit,art_remarks)
                                                          values('{prod_no}','{general_testtype_no}','{dt1.Rows[i]["SUBMATERIAL_NO"]}','{dt2.Rows[t]["TYPENAME"]}','{dt2.Rows[t]["检测项编号"]}',
                                                          '{dt2.Rows[t]["检测项名称"]}','{dt2.Rows[t]["检测项类型"]}','{dt2.Rows[t]["检测项类型名称"]}','{dt2.Rows[t]["试样数量"]}',
                                                          '{dt2.Rows[t]["项目引用级别标准"]}','{dt2.Rows[t]["检验项目类型"]}','{dt2.Rows[t]["判断标准通用"]}','{dt2.Rows[t]["测量标准通用"]}',
                                                          '{dt2.Rows[t]["判断标准定制"]}','{dt2.Rows[t]["测量标准定制"]}','{dt2.Rows[t]["通用公式类型"]}','{dt2.Rows[t]["自定义公式类型"]}','{dt2.Rows[t]["单位"]}','{dt2.Rows[t]["ART定制备注"]}')";
                                }
                               
                            }
                            if (!string.IsNullOrEmpty(sep1))
                            {
                                string sep11 = "insert all " + sep1 + " select*from dual";
                                DB.ExecuteNonQuery(sep11);
                            }
                            if (!string.IsNullOrEmpty(sep2))
                            {
                                string sep22 = "insert all " + sep2 + " select*from dual";
                                DB.ExecuteNonQuery(sep22);
                            }
                          
                           
                        }
                    }
                }

                DB.Commit();//提交事务
                #endregion
                //ret.RetData1 = dt;
                ret.IsSuccess = true;
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
        /// ART上传图片处基本信息
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Information(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                //转译
                string sql = $@"select*from bdm_rd_prod where prod_no='{prod_no}'";

                DataTable dt = DB.GetDataTable(sql);
                DataTable dt2 = DB.GetDataTable($"select img_url from bdm_prod_m where prod_no='{prod_no}'");
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dt);
                ret.RetData1 = Newtonsoft.Json.JsonConvert.SerializeObject(dt2);
                ret.IsSuccess = true;

            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }
        //修改视图的信息
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject UpdateList(object OBJ)
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
                string prod_nos = jarr.ContainsKey("prod_nos") ? jarr["prod_nos"].ToString() : "";
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";
                string general_testtype_nos = jarr.ContainsKey("general_testtype_nos") ? jarr["general_testtype_nos"].ToString() : "";
                string CreactUserId = SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(ReqObj.UserToken);//查询登录人UserID
                var data = jarr.ContainsKey("DgvAll") ? jarr["DgvAll"].ToString() : "";
                DataTable table = Newtonsoft.Json.JsonConvert.DeserializeObject<DataTable>(data);
               
                string sql3 = string.Empty;
                foreach (DataRow item in table.Rows)
                {
                    string sql1 = $"select prod_no from BDM_PROD_CUSTOMQUALITY_ITEM where prod_no='{item["prod_nos"]}' and general_testtype_no='{item["general_testtype_nos"]}' and category_no='{item["category_nos"]}' and testitem_code='{item["testitem_code"]}'";
                    DataTable dt = DB.GetDataTable(sql1);
                    if (dt.Rows.Count > 0)
                    {
                        string sql2 = $@"update BDM_PROD_CUSTOMQUALITY_ITEM set AQL_LEVEL='{item["AQL_LEVEL"]}',testitem_category='{item["testitem_category"]}',testitem_name='{item["testitem_name"]}',testtype_no='{item["testtype_no"]}',testtype_name='{item["testtype_name"]}',reference_level='{item["reference_levelTYS"]}',d_reference_level='{item["reference_levelTY"]}',t_check_item='{item["check_itemTY"]}',t_check_value='{item["check_valueTY"]}',d_check_item='{item["check_itemDZ"]}',d_check_value='{item["check_valueDZ"]}',currency_formula='{item["currency_formula"]}',custom_formula='{item["custom_formula"]}',unit='{item["unit"]}',art_remarks='{item["art_remarks"]}',modifyby='{CreactUserId}',modifydate='{DateTime.Now.ToString("yyyy-MM-dd")}',modifytime='{DateTime.Now.ToString("HH:mm:ss")}'  where prod_no='{item["prod_nos"]}' and general_testtype_no='{item["general_testtype_nos"]}' and category_no='{item["category_nos"]}' and testitem_code='{item["testitem_code"]}'";
                        DB.ExecuteNonQuery(sql2);
                    }
                    else if(string.IsNullOrEmpty(prod_nos)||
                        string.IsNullOrEmpty(general_testtype_nos) ||
                          string.IsNullOrEmpty(category_no)
                        )
                    {
                        throw new Exception("缺少关键字段，请联系管理员！");
                    }
                    else
                    {
                         sql3+=$@" into BDM_PROD_CUSTOMQUALITY_ITEM(AQL_LEVEL,prod_no,general_testtype_no,category_no,testitem_category,testitem_code,testitem_name,testtype_no,testtype_name,sample_num,reference_level,d_reference_level,check_type,t_check_item,t_check_value,d_check_item,d_check_value,currency_formula,custom_formula,unit,art_remarks,createby,createdate,createtime) values('{item["AQL_LEVEL"]}','{prod_nos}','{general_testtype_nos}','{category_no}','{item["testitem_category"]}','{item["testitem_code"]}','{item["testitem_name"]}','{item["testtype_no"]}','{item["testtype_name"]}','{item["sample_num"]}','{item["reference_levelTYS"]}','{item["reference_levelTY"]}','','{item["check_itemTY"]}','{item["check_valueTY"]}','{item["check_itemDZ"]}','{item["check_valueDZ"]}','{item["currency_formula"]}','{item["custom_formula"]}','{item["unit"]}','{item["art_remarks"]}','{CreactUserId}','{DateTime.Now.ToString("yyyy-MM-dd")}','{DateTime.Now.ToString("HH:mm:ss")}')";
                    }
                }
                if (!string.IsNullOrEmpty(sql3))
                {
                    string sql33 = "insert ALL " + sql3 + " select*from dual";
                    DB.ExecuteNonQuery(sql33);
                }
              
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
        /// <summary>
        /// 页面视图展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetList(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);

            try
            {
                string Data = ReqObj.Data.ToString();
                //转译
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";//ART编号
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";//ART编号
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";//ART编号  


                string pageSize = jarr.ContainsKey("pageSize") ? jarr["pageSize"].ToString() : "";
                string pageIndex = jarr.ContainsKey("pageIndex") ? jarr["pageIndex"].ToString() : "";


                string sql = $@"SELECT
		prod_no,
		category_no,
    general_testtype_no,
		testitem_category,
        testtype_no,
        testtype_name,
		sys001m.enum_value as testtype_name_1,
		testitem_code_1,
		testitem_name_1,
		check_itemTY_1,
		check_valueTY_1,
		check_itemDZ_1,
		check_valueDZ_1,
        AQL_LEVEL_1,
        unit_1,
		reference_levelTY_1,
    	d_reference_level_1,
		sample_num_1,
		currency_formula_1,
		custom_formula_1,
		art_remarks_1,
		ap.enum_value as currency_formulaName,
		ad.formula_name as  custom_formulaName 
FROM
	(
	SELECT
		PROD_NO,
		category_no,
		general_testtype_no,
		testitem_category,
		testitem_code testitem_code_1,
        testtype_no,
        testtype_name,
		testitem_name testitem_name_1,
		t_check_item check_itemTY_1,
		t_check_value check_valueTY_1,
		d_check_item check_itemDZ_1,
		d_check_value check_valueDZ_1,
		unit unit_1,
        AQL_LEVEL AQL_LEVEL_1,
		reference_level reference_levelTY_1,
    d_reference_level d_reference_level_1,
		sample_num sample_num_1,
		currency_formula currency_formula_1,
		custom_formula custom_formula_1,
		art_remarks art_remarks_1 
	FROM
		BDM_PROD_CUSTOMQUALITY_ITEM 
	) tab
	LEFT JOIN sys001m ON sys001m.enum_code = tab.TESTITEM_CATEGORY and 	ENUM_TYPE='enum_testitem_category'
	LEFT JOIN sys001m ap ON ap.enum_code = tab.currency_formula_1 and 	ap.ENUM_TYPE='enum_general_formula'
	left join bdm_formula_m ad on tab.custom_formula_1=ad.formula_code where  prod_no = '{prod_no}' and general_testtype_no = '{general_testtype_no}'  and category_no = '{category_no}'";



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
                ret.ErrMsg = ex.Message;

            }
            finally
            {
                DB.Close();//关闭事务
            }
            return ret;

        }
        /// <summary>
        /// 取消勾选就删除
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject ARTDelete(object OBJ)
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

                string prod_no = jarr.ContainsKey("prod_no") ? jarr["prod_no"].ToString() : "";
                string general_testtype_no = jarr.ContainsKey("general_testtype_no") ? jarr["general_testtype_no"].ToString() : "";
                string category_no = jarr.ContainsKey("category_no") ? jarr["category_no"].ToString() : "";
                string testitem_code = jarr.ContainsKey("testitem_code") ? jarr["testitem_code"].ToString() : "";

                string sql1 = $"select prod_no from BDM_PROD_CUSTOMQUALITY_ITEM where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}' and category_no='{category_no}' and testitem_code='{testitem_code}'";
                DataTable dt = DB.GetDataTable(sql1);
                string sql2 = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    sql2 = $@"delete from BDM_PROD_CUSTOMQUALITY_ITEM where prod_no='{prod_no}' and general_testtype_no='{general_testtype_no}' and category_no='{category_no}' and testitem_code='{testitem_code}'";
                }
                DB.ExecuteNonQuery(sql2);
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

    }
}
