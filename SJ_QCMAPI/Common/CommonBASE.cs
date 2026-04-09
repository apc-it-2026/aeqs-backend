using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_QCMAPI.Common
{
    public static class CommonBASE
    {
        /// <summary>
        /// 当前oracle版本是否支持lisagg
        /// </summary>
        public static bool? ORACLE_LISTAGG_USE = null;

        /// <summary>
        /// 根据oracle版本获取拼接函数
        /// </summary>
        /// <param name="DB"></param>
        /// <param name="concatStr">需要拼接的字段</param>
        /// <param name="lisaggOrderStr">lisagg顺序</param>
        /// <param name="joinStr">拼接符号</param>
        /// <returns></returns>
        public static string GetGroupConcatByOracleVersion(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string concatStr, string lisaggOrderStr, string joinStr = ",")
        {
            if (!ORACLE_LISTAGG_USE.HasValue)
            {
                DataTable oracleVerDt = DB.GetDataTable($@"select * from v$version");
                foreach (DataRow item in oracleVerDt.Rows)
                {
                    string version = item[0].ToString();
                    if (version.ToLower().Contains("19"))
                        ORACLE_LISTAGG_USE = true;
                    else
                        ORACLE_LISTAGG_USE = false;
                    break;
                }
            }
            string res = "";
            if (ORACLE_LISTAGG_USE.Value)
            {
                res = $@"listagg({concatStr}, '{joinStr}') WITHIN GROUP (ORDER BY {lisaggOrderStr})";
            }
            else
            {
                if (joinStr == ",")
                    res = $@"wm_concat({concatStr})";
                else
                    res = $@"replace(wm_concat({concatStr}),',', '{joinStr}' )";
            }
            return res;
        }

        /// <summary>
        /// 获取当前用户代号
        /// </summary>
        /// <param name="UserToken"></param>
        /// <returns></returns>
        public static string GetUserCode(string UserToken)
        { 
            return SJeMES_Framework_NETCore.Web.System.GetUserCodeByToken(UserToken);
        }


        /// <summary>
        /// 获取当前日期
        /// </summary>
        /// <returns></returns>
        public static string GetYMD()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 查询分页数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static DataTable GetPageDataTable(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string sqlContent, int pageIndex, int pageSize,string orderBy="", Dictionary<string, object> Parameters = null)
        {
            DataTable dt = new DataTable();
            int total = (pageIndex - 1) * pageSize; 

            string sql = $@"select * from (
                                    select M.*,ROWNUM as RN from ({sqlContent}) M
                                    where 1=1 {orderBy}
                                    ) tab
                      where RN between " + (total + 1).ToString() + " and " + (total + Convert.ToInt32(pageSize)).ToString();

            if (Parameters != null && Parameters.Count > 0)
            {
                dt = DB.GetDataTable(sql,Parameters);
            }
            else
            {
                dt = DB.GetDataTable(sql);
            }
            return dt;
        
        }

        /// <summary>
        /// 查询SQL总行数
        /// </summary>
        /// <param name="sqlContent"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public static int GetPageDataTableCount(SJeMES_Framework_NETCore.DBHelper.DataBase DB, string sqlContent,Dictionary<string,object> Parameters=null)
        { 
            string sql = $@"   select count(1) from ({sqlContent}) M ";
            if (Parameters !=null &&  Parameters.Count>0)
            {
               return DB.GetInt32(sql,Parameters);
            }
            else
            {
                return DB.GetInt32(sql);
            }
        }

    }
}
