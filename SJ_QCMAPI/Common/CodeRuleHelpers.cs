using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI.Common
{
    /// <summary>
    /// 编码规则 辅助类 by hedx 2021.05.28
    /// </summary>
    public class CodeRuleHelpers
    {
        /// <summary>
        /// 根据编码规则生产新条码
        /// </summary>
        /// <param name="token"></param>
        /// <param name="table_name">表名</param>
        /// <param name="keys">兼容旧数据使用，如果是新的编码规则不用传入</param>
        /// <returns></returns>
        public static string GetNewCodeByRule(string RULE_NO, SJeMES_Framework_NETCore.DBHelper.DataBase DB = null)
        {

            string SerialCode = string.Empty;
            try
            {
                #region 兼容旧方法产生编码
                var sql = $@"select RULE_NO from bdm_code_rule_m where RULE_NO='{RULE_NO}'";
                var ruleNo = DB.GetString(sql);
                if (string.IsNullOrWhiteSpace(ruleNo))
                {
                    throw new Exception("请先维护代号【" + ruleNo.ToLower() + "】的编码规则，请检查！");
                }
                #endregion

                #region 根据编码规则产生编码
                DataTable bdm_code_rule_md = DB.GetDataTable($@"
SELECT
	a.RULE_NO,
    a.INITIAL_DATE,
	a.RULE_NAME,
	a.RULE_INSTANCE,
	a.INITIAL_RULE,
	a.REMARKS,
	b.RULE_SEQ,
	b.RULE_ITEM,
	b.RULE_VALUE,
	b.COMPLEMENT,
	b.COMPLEMENT_CHAR,
	b.LENGTH 
FROM
	bdm_code_rule_m a
	LEFT JOIN bdm_code_rule_d b ON a.RULE_NO = b.RULE_NO
WHERE
    a.rule_no='{ruleNo}'");
                if (bdm_code_rule_md == null || bdm_code_rule_md.Rows.Count == 0)
                    throw new Exception("编码规则代号【" + ruleNo + "】信息不存在，请检查！");

                //流水号初始化日期
                string str_initial_date = bdm_code_rule_md.Rows[0]["INITIAL_DATE"].ToString();
                DateTime initial_date = DateTime.Now;
                if (!string.IsNullOrEmpty(str_initial_date))
                    initial_date = Convert.ToDateTime(str_initial_date);

                //初始规律 // 每日@:每日@;每月@:每月@;每年@:每年@;一直累加@:一直累加@;
                string init_law = bdm_code_rule_md.Rows[0]["INITIAL_RULE"].ToString();

                //查询编码规则 bdm_code_rule_d
                DataTable bdm_code_rule_d = DB.GetDataTable($@"select RULE_NO,RULE_SEQ,RULE_ITEM,RULE_VALUE,COMPLEMENT,COMPLEMENT_CHAR,LENGTH from bdm_code_rule_d  where RULE_NO='{ruleNo}'");
                if (bdm_code_rule_d == null || bdm_code_rule_d.Rows.Count == 0)
                    throw new Exception("编码规则代号【" + ruleNo + "】信息为空，请先维护编码规则！");


                #region 1.判断是否需要初始化流水号
                //每日
                if (DateTime.Now.Date >= initial_date.Date && init_law == enum_initial_rule.enum_initial_date_0)
                {
                    InitializeSerialNumber(DB, ruleNo);
                    initial_date = DateTime.Now.AddDays(1);
                    UpdateInitialDate(DB, ruleNo, initial_date.ToString("yyyy-MM-dd"));
                }
                //每月
                else if (DateTime.Now.Month >= initial_date.Date.Month && DateTime.Now.Year == initial_date.Date.Year && init_law == enum_initial_rule.enum_initial_date_1)
                {
                    InitializeSerialNumber(DB, ruleNo);
                    initial_date = DateTime.Now.AddMonths(1);
                    UpdateInitialDate(DB, ruleNo, initial_date.ToString("yyyy-MM-dd"));
                }
                //每年
                else if (DateTime.Now.Year == initial_date.Date.Year && init_law.Equals(enum_initial_rule.enum_initial_date_2))
                {
                    InitializeSerialNumber(DB, ruleNo);
                    initial_date = DateTime.Now.AddYears(1);
                    UpdateInitialDate(DB, ruleNo, initial_date.ToString("yyyy-MM-dd"));
                }
                //一直累加
                else if (init_law.Equals(enum_initial_rule.enum_initial_date_3))
                {
                }
                #endregion
                #region 2.根据设定的规则 提取编码，生成编码 
                foreach (DataRow dr in bdm_code_rule_d.Rows)
                {
                    string TempCode = string.Empty;
                    //补码位数
                    int digits = 0;
                    //规则项内容
                    int.TryParse(dr["RULE_VALUE"].ToString(), out digits);

                    //补位规则  covering
                    string COMPLEMENT = dr["COMPLEMENT"].ToString();

                    //补位字符 covering_char
                    string COMPLEMENT_CHAR = dr["COMPLEMENT_CHAR"].ToString();


                    /* 固定字符 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_0)
                        TempCode = dr["rule_item_character"].ToString().Trim();

                    /* 年[yyyy] 4位年 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_1)
                        TempCode = DateTime.Now.Year.ToString();

                    /* 年[yy] 2位年 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_2)
                        TempCode = DateTime.Now.ToString("yy");

                    /* 月[MM] 月份 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_3)
                        TempCode = DateTime.Now.Month.ToString().PadLeft(2, '0');

                    /* 日[dd] 日期 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_4)
                        TempCode = DateTime.Now.Day.ToString().PadLeft(2, '0');

                    /* 流水号 */
                    if (dr["RULE_ITEM"].ToString() == enum_rule_item.enum_rule_item_5)
                        TempCode = GetSerialNumber(DB, ruleNo).ToString();

                    /* 补码 */
                    if (digits > 0 && !string.IsNullOrEmpty(COMPLEMENT) && !string.IsNullOrEmpty(COMPLEMENT_CHAR))
                    {
                        if (COMPLEMENT.Equals(enum_complement.enum_complement_num_1))
                        {
                            TempCode = TempCode.PadLeft(digits, Convert.ToChar(COMPLEMENT_CHAR));
                        }
                        else if (COMPLEMENT.Equals(enum_complement.enum_complement_num_2))
                        {
                            TempCode = TempCode.PadRight(digits, Convert.ToChar(COMPLEMENT_CHAR));
                        }
                        else
                        {
                            //不补位
                        }
                    }

                    SerialCode += TempCode;
                }

                if (string.IsNullOrEmpty(SerialCode))
                    throw new Exception("根据编码规则提取编码失败，请检查！");
                #endregion
                //RULE_INSTANCE
                #region 3.更新回写编码规则的实例
                DB.ExecuteNonQueryOffline($"update  bdm_code_rule_m set rule_instance='{SerialCode}' where rule_no='{ruleNo}'");
                #endregion

                #endregion

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return SerialCode;
        }


        /// <summary>
        /// 初始化流水号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SMT"></param>
        /// <returns></returns>
        public static void InitializeSerialNumber(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string rule_no)
        {
            try
            {
                string sql = $"select 1 from bdm_code_rule_item where rule_no='{rule_no}'";
                if (!string.IsNullOrEmpty(DB.GetString(sql)))
                {
                    //重置流水号
                    DB.ExecuteNonQueryOffline($@"   
                                               select * from bdm_code_rule_item  where rule_no='{rule_no}' for update; 
                                               update bdm_code_rule_item set current_num=0 where rule_no='{rule_no}';
                    ");
                }
                else
                {
                    #region 兼容客户已经产生使用的流水单号(使用一段时间后可以注销掉这部分代码) 2021.06.01 by hedx
                    //string barcode = CODE.GetDocNo(DB, keys, table_name);
                    //if (!string.IsNullOrEmpty(barcode))
                    //{
                    //    int num = 1;
                    //    Int32.TryParse(barcode.Substring(barcode.Length - 4, 4), out num);
                    //    if (num == 1)
                    //    {
                    //        //如果查询不到，新增
                    //        DB.ExecuteNonQueryOffline($@"insert into serial_number(rule_no,current_num)values('{rule_no}','0')");
                    //    }
                    //    else
                    //    {
                    //        DB.ExecuteNonQueryOffline($@"insert into serial_number(rule_no,current_num)values('{rule_no}','{num}')");
                    //    }
                    //}
                    //else
                    //{ 
                    //    //如果查询不到，新增
                    //    DB.ExecuteNonQueryOffline($@"insert into serial_number(rule_no,current_num)values('{rule_no}','0')");
                    //} 
                    #endregion

                    //如果查询不到，新增
                    DB.ExecuteNonQueryOffline($@"insert into bdm_code_rule_item(rule_no,current_num)values('{rule_no}','0')");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="SMT">流水号类型</param>
        /// <param name="Count">分配数量</param>
        /// <returns></returns>
        public static int GetSerialNumber(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string rule_no)
        {
            int num = 0;
            try
            {
                string sql = $"select 1 from bdm_code_rule_item where rule_no='{rule_no}'";
                if (!string.IsNullOrEmpty(DB.GetString(sql)))
                {
                    //流水号+1  行锁，修改数据
                    string str = DB.GetString($@"
                                               select current_num from bdm_code_rule_item  where rule_no='{rule_no}' for update; 
                                               update bdm_code_rule_item set current_num=current_num+1 where rule_no='{rule_no}';
                                            ");

                    //string str = DB.GetString($"select current_num from serial_number where rule_no ='{rule_no}'");
                    if (!string.IsNullOrEmpty(str))
                    {
                        num = Convert.ToInt32(str) + 1;
                    }
                }
                else
                {
                    #region 兼容客户已经产生使用的流水单号(使用一段时间后可以注销掉这部分代码) 2021.06.01 by hedx
                    //string barcode = CODE.GetDocNo(DB, keys, table_name); 
                    //int index = 0;
                    //Int32.TryParse(barcode.Substring(barcode.Length - 4, 4), out index);
                    //if (index == 0)
                    //{
                    //    //如果查询不到，新增
                    //    DB.ExecuteNonQueryOffline($@"insert into serial_number(rule_no,current_num)values('{rule_no}','1')");
                    //    num = 1;
                    //}
                    //else
                    //{
                    //    DB.ExecuteNonQueryOffline($@"insert into serial_number(rule_no,current_num)values('{rule_no}','{index}')");
                    //    num = index;
                    //} 
                    #endregion

                    //如果查询不到，新增
                    DB.ExecuteNonQueryOffline($@"insert into bdm_code_rule_item(rule_no,current_num)values('{rule_no}','1')");
                    num = 1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return num;
        }

        /// <summary>
        /// 更新初始化日期
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="rule_no"></param>
        /// <returns></returns>
        public static long UpdateInitialDate(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string rule_no, string initial_date)
        {
            try
            {
                string sqlString = $@"UPDATE bdm_code_rule_m set initial_date='{initial_date}' where rule_no='{rule_no}'";
                return DB.ExecuteNonQueryOffline(sqlString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
