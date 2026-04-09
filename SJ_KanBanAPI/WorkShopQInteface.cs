using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_KanBanAPI
{
    /// <summary>
    /// 车间Q
    /// </summary>
    internal class WorkShopQInteface
    {
        public static string moudle_code = "temperatureHumidity";//温湿度
        /// <summary>
        /// 设备温湿度展示
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTempAndHumData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                Dictionary<string, object> RetDic = new Dictionary<string, object>();
                string ui_lan_type = jarr.ContainsKey("ui_lan_type") ? jarr["ui_lan_type"].ToString() : "";//
                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂商/厂区
                string stockName = jarr.ContainsKey("stockName") ? jarr["stockName"].ToString() : "";//仓库名
                string eqNo = jarr.ContainsKey("eqNo") ? jarr["eqNo"].ToString() : "";//设备编号
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间
                string devName = jarr.ContainsKey("devName") ? jarr["devName"].ToString() : "";//设备名称
                string devKey = jarr.ContainsKey("devKey") ? jarr["devKey"].ToString() : "";//设备编码

                string getType= jarr.ContainsKey("getType") ? jarr["getType"].ToString() : "0";//数据查询类型【0】温度【1】湿度


                if (string.IsNullOrEmpty(startTime)||string.IsNullOrEmpty(endTime))
                {
                    throw new Exception("【Start time】【End time】is a must!");
                }
                if (string.IsNullOrEmpty(devName))
                {
                    throw new Exception("The necessary condition [device code] is missing");
                }
                if (string.IsNullOrEmpty(devKey))
                {
                    throw new Exception("The necessary condition [device code] is missing");
                }
                string QuerySql =string.Empty;//查询SQL
                                              //Commented by Ashok on 2025/09/02
                                              //string TempDataSql = $@"SELECT
                                              //                         DEVKEY,
                                              //                         DEVNAME,
                                              //                         MAX( TEMPVALUE ) maxNum,
                                              //                         MIN( TEMPVALUE ) minNum,
                                              //                         ROUND( AVG( TEMPVALUE ), 2 ) avgNum,
                                              //                         --TO_NUMBER( nvl(SUBSTR( TEMPALARMRANGE, INSTR( TEMPALARMRANGE, '～',- 1 ) + 1 ),0) ) sopNum,
                                              //                         '21~25' sopNum,
                                              //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ) SAVEDATE
                                              //                        FROM
                                              //                         t_MSD_historydata 
                                              //                        WHERE
                                              //                         DEVKEY = '{devKey}' 
                                              //                         AND DEVNAME = '{devName}' 
                                              //                            and TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ) BETWEEN '{startTime}' AND '{endTime}'
                                              //                        GROUP BY
                                              //                         DEVKEY,
                                              //                         DEVNAME,
                                              //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ),
                                              //                         TEMPALARMRANGE 
                                              //                        ORDER BY
                                              //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ),
                                              //                         DEVNAME";

                string TempDataSql = $@"SELECT a.DEVKEY,
       a.DEVNAME,
       MAX(a.TEMPVALUE) AS maxNum,
       MIN(a.TEMPVALUE) AS minNum,
       ROUND(AVG(a.TEMPVALUE), 2) AS avgNum,
       CASE
         WHEN b.MAXTEMPALARMRANGE IS NULL THEN
          'Above ' || b.MINTEMPALARMRANGE
         ELSE
          b.MINTEMPALARMRANGE || '~' || b.MAXTEMPALARMRANGE
       END AS sopNum,
       TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'), 'yyyy-mm-dd') AS SAVEDATE
  FROM t_MSD_historydata a
 INNER JOIN T_MSD_REALTIMEDATA b
    ON a.devname = b.devname
 WHERE a.DEVKEY = '{devKey}' AND a.DEVNAME = '{devName}'  and
TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'), 'yyyy-mm-dd') BETWEEN
      '{startTime}' AND '{endTime}'
 GROUP BY a.DEVKEY,
          a.DEVNAME,
          TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'),
                  'yyyy-mm-dd'),
          b.MINTEMPALARMRANGE,
          b.MAXTEMPALARMRANGE
 ORDER BY TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'),
                  'yyyy-mm-dd'),
          a.DEVNAME";
                //Commented by Ashok on 2025/09/02
                //string HumDataSql = $@"SELECT
                //                         DEVKEY,
                //                         DEVNAME,
                //                         MAX( HUMIVALUE ) maxNum,
                //                         MIN( HUMIVALUE ) minNum,
                //                         ROUND( AVG( HUMIVALUE ), 2 ) avgNum,
                //                         --TO_NUMBER( nvl(SUBSTR(  HUMIALARMRANGE, INSTR( HUMIALARMRANGE, '～',- 1 ) + 1 ),0) ) sopNum,
                //                         '40~60' sopNum,
                //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ) SAVEDATE
                //                        FROM
                //                         t_MSD_historydata 
                //                        WHERE
                //                         DEVKEY = '{devKey}' 
                //                         AND DEVNAME = '{devName}' 
                //                            and TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ) BETWEEN '{startTime}' AND '{endTime}'
                //                        GROUP BY
                //                         DEVKEY,
                //                         DEVNAME,
                //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ),
                //                         HUMIALARMRANGE 
                //                        ORDER BY
                //                         TO_CHAR( TO_DATE( SAVETIME, 'yyyy-mm-dd hh24:mi:ss' ), 'yyyy-mm-dd' ),
                //                         DEVNAME";

                string HumDataSql = $@"SELECT a.DEVKEY,
       a.DEVNAME,
       MAX(a.HUMIVALUE) maxNum,
       MIN(a.HUMIVALUE) minNum,
       ROUND(AVG(a.HUMIVALUE), 2) avgNum,
        CASE
         WHEN b.MINHUMIALARMRANGE IS NULL THEN
          'Below ' || b.MAXHUMIALARMRANGE
         ELSE
          b.MINHUMIALARMRANGE || '~' || b.MAXHUMIALARMRANGE
       END AS sopNum,
       TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'), 'yyyy-mm-dd') AS SAVEDATE
  FROM t_MSD_historydata a
 INNER JOIN T_MSD_REALTIMEDATA b
    ON a.devname = b.devname
 WHERE  a.DEVKEY = '{devKey}' AND a.DEVNAME = '{devName}'  and
TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'), 'yyyy-mm-dd') BETWEEN
      '{startTime}' AND '{endTime}'
 GROUP BY a.DEVKEY,
          a.DEVNAME,
          TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'),
                  'yyyy-mm-dd'),
          b.MINHUMIALARMRANGE,
          b.MAXHUMIALARMRANGE
 ORDER BY TO_CHAR(TO_DATE(a.SAVETIME, 'yyyy-mm-dd hh24:mi:ss'),
                  'yyyy-mm-dd'),
          a.DEVNAME
";

                switch (getType)
                {
                    case "0":
                        QuerySql = TempDataSql;
                        break;
                    case "1":
                        QuerySql = HumDataSql;
                        break;
                }
                DataTable Dt = DB.GetDataTable(QuerySql);
                //时间数组
                List<string> TimeList = Common.ReturnData("d", startTime, Convert.ToDateTime(endTime).AddDays(1).ToString("yyyy-MM-dd"));
                List<decimal> MaxList = new List<decimal>();//最大值
                List<decimal> MinList = new List<decimal>();//最小值
                List<decimal> AvgList = new List<decimal>();//平均值
                //List<decimal> SopList = new List<decimal>();//sop标准值
                List<string> SopList = new List<string>();//sop标准值


                foreach (var item in TimeList)
                {
                    decimal maxNum = 0;
                    decimal minNum = 0;
                    decimal avgNum = 0;
                    //decimal sopNum = 0;
                    string sopNum = "";
                    DataRow[] QueryRows = Dt.Select($@"SAVEDATE='{item}'");
                    foreach (DataRow RItem in QueryRows)
                    {
                        maxNum += Convert.ToDecimal(RItem["maxNum"] !=null?RItem["maxNum"].ToString():"0");
                        minNum += Convert.ToDecimal(RItem["minNum"] !=null?RItem["minNum"].ToString():"0");
                        avgNum += Convert.ToDecimal(RItem["avgNum"] !=null?RItem["avgNum"].ToString():"0");
                        //sopNum += Convert.ToDecimal(RItem["sopNum"] !=null?RItem["sopNum"].ToString():"0");
                        sopNum += RItem["sopNum"] !=null?RItem["sopNum"].ToString():"0";
                    }
                    MaxList.Add(maxNum);
                    MinList.Add(minNum);
                    AvgList.Add(avgNum);
                    SopList.Add(sopNum);
                }

                //多语言翻译
                string name1 = "Highest Value";
                string name2 = "Lowest_Value";
                string name3 = "Average_Value";
                string name4 = "SOP Standard Value";

                //var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "最高值", "最低值", "平均值" , "SOP标准值" });
                var LanDic = Common.GetLanguagebyKanBan(ui_lan_type, moudle_code, new List<string>() { "Maximum value", "Minimum value", "Average value", "SOP standard value" });
                if (LanDic.Count > 0)
                {
                    name1 = LanDic["最高值"].ToString();
                    name2 = LanDic["最低值"].ToString();
                    name3 = LanDic["平均值"].ToString();
                    name4 = LanDic["SOP标准值"].ToString();

                }

                List<Dictionary<string, object>> DataDic = new List<Dictionary<string, object>>();

                Dictionary<string, object> maxDic = new Dictionary<string, object>();
                maxDic["name"] = $@"{devName}-{name1}";
                maxDic["data"] = MaxList;
                DataDic.Add(maxDic);

                Dictionary<string, object> minDic = new Dictionary<string, object>();
                minDic["name"] = $@"{devName}-{name2}";
                minDic["data"] = MinList;
                DataDic.Add(minDic);

                Dictionary<string, object> avgDic = new Dictionary<string, object>();
                avgDic["name"] = $@"{devName}-{name3}";
                avgDic["data"] = AvgList;
                DataDic.Add(avgDic);

                Dictionary<string, object> sopDic = new Dictionary<string, object>();
                sopDic["name"] = name4;
                sopDic["data"] = SopList;
                DataDic.Add(sopDic);


                RetDic["x"] = TimeList;//x轴
                RetDic["bar"] = DataDic;//折线数据

                ret.RetData1 = RetDic;
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
        /// 设备数据表格
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetTHData(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(ReqObj);
                string Data = ReqObj.Data.ToString();
                var jarr = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string plantType = jarr.ContainsKey("plantType") ? jarr["plantType"].ToString() : "";//厂商/厂区 类型
                string plant = jarr.ContainsKey("plant") ? jarr["plant"].ToString() : "";//厂商/厂区
                string stockName = jarr.ContainsKey("stockName") ? jarr["stockName"].ToString() : "";//仓库名
                string art = jarr.ContainsKey("art") ? jarr["art"].ToString() : "";//ART
                string po = jarr.ContainsKey("po") ? jarr["po"].ToString() : "";//PO
                string eqNo = jarr.ContainsKey("eqNo") ? jarr["eqNo"].ToString() : "";//设备编号
                string startTime = jarr.ContainsKey("startTime") ? jarr["startTime"].ToString() : "";//开始时间
                string endTime = jarr.ContainsKey("endTime") ? jarr["endTime"].ToString() : "";//结束时间

                string WhereSQL = string.Empty;
                string[] arr = new string[] { };
                if (!string.IsNullOrEmpty(plant))
                {
                    arr = plant.Split(',');
                    WhereSQL += $@"and a.ORG in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";

                    //WhereSQL = $@" and a.ORG like '{plant}%'";
                }
                if (!string.IsNullOrEmpty(stockName))
                {

                }
                if (!string.IsNullOrEmpty(eqNo))
                {
                    arr = eqNo.Split(',');
                    WhereSQL += $@"and M.DEVNAME in ({string.Join(',', arr.Select(x => $"'{x}'"))}) ";

                    //WhereSQL += $@" AND M.DEVNAME = '{eqNo}' ";
                }


                //string DataSQL = $@"SELECT
                //                     a.ORG AS plantArea,
                //                        m.DEVLOCATION as DEV_TYPE,
                //                    -- A.DEV_TYPE,
                //                     m.DEVKEY,
                //                     m.DEVNAME,
                //                     MINTEMPALARMRANGE || '~' || MAXTEMPALARMRANGE AS TEMPRANGE,
                //                     M.TEMPVALUE MAXTEMP,
                //                    CASE

                //                      WHEN TO_NUMBER( M.TEMPVALUE ) <= TO_NUMBER( MAXTEMPALARMRANGE ) THEN
                //                      '0' ELSE '1' 
                //                     END TEMPSTATUS,
                //                     MINHUMIALARMRANGE || '~' || MAXHUMIALARMRANGE HUMRANGE,
                //                     M.HUMIVALUE maxHum,
                //                    CASE

                //                     WHEN TO_NUMBER( M.HUMIVALUE ) <= TO_NUMBER( MAXHUMIALARMRANGE ) THEN
                //                     '0' ELSE '1' 
                //                     END HUMSTATUS 
                //                    FROM
                //                     T_MSD_REALTIMEDATA m
                //                     LEFT JOIN t_msd_devlist a ON m.DEVNAME = a.DEVNAME 
                //                    WHERE
                //                     1 = 1  {WhereSQL}
                //                   --and TO_CHAR(SAVETIME,'yyyy-MM-dd') BETWEEN '{startTime}'and '{endTime}'
                //                    ";


                string DataSQL = $@"SELECT a.ORG AS plantArea,
       m.DEVLOCATION AS DEV_TYPE,
       m.DEVKEY,
       m.DEVNAME,
       CASE
         WHEN MAXTEMPALARMRANGE IS NULL THEN
          'Above ' || MINTEMPALARMRANGE
         ELSE
          MINTEMPALARMRANGE || '~' || MAXTEMPALARMRANGE
       END AS TEMPRANGE,
       M.TEMPVALUE AS MAXTEMP,
       CASE
         WHEN MAXTEMPALARMRANGE IS NOT NULL THEN
            CASE
              WHEN TO_NUMBER(M.TEMPVALUE) BETWEEN TO_NUMBER(MINTEMPALARMRANGE) AND TO_NUMBER(MAXTEMPALARMRANGE) THEN '0'
              ELSE '1'
            END
         ELSE
            CASE
              WHEN TO_NUMBER(M.TEMPVALUE) > TO_NUMBER(MINTEMPALARMRANGE) THEN '0'
              ELSE '1'
            END
       END AS TEMPSTATUS,
       CASE
         WHEN MINHUMIALARMRANGE IS NULL THEN
          'Below ' || MAXHUMIALARMRANGE
         ELSE
          MINHUMIALARMRANGE || '~' || MAXHUMIALARMRANGE
       END AS HUMRANGE,
       M.HUMIVALUE AS MAXHUM,
       CASE
         WHEN MINHUMIALARMRANGE IS NOT NULL THEN
            CASE
              WHEN TO_NUMBER(M.HUMIVALUE) BETWEEN TO_NUMBER(MINHUMIALARMRANGE) AND TO_NUMBER(MAXHUMIALARMRANGE) THEN '0'
              ELSE '1'
            END
         ELSE
            CASE
              WHEN TO_NUMBER(M.HUMIVALUE) < TO_NUMBER(MAXHUMIALARMRANGE) THEN '0'
              ELSE '1'
            END
       END AS HUMSTATUS
  FROM T_MSD_REALTIMEDATA m
  LEFT JOIN T_MSD_DEVLIST a
    ON m.DEVNAME = a.DEVNAME
 WHERE 1 = 1 {WhereSQL} --and TO_CHAR(SAVETIME,'yyyy-MM-dd') BETWEEN '{startTime}'and '{endTime}'
";

                DataTable Dt = DB.GetDataTable(DataSQL);
                //foreach (DataRow item in Dt.Rows)
                //{
                //    decimal Num = 0;
                //    decimal.TryParse(item["maxHum"].ToString(), out Num);
                //    if (Num>85)
                //    {
                //        item["maxHum"] = Num - 10;
                //    }
                //}
                ret.RetData1 = Dt;
                ret.IsSuccess = true;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }
            return ret;
        }


    }
}
