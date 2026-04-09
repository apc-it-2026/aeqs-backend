using Newtonsoft.Json;
using SJ_QCMAPI.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SJ_BDMAPI
{
    /// <summary>
    /// 移动端 多语言翻译
    /// </summary>
    public class UILAN_APP
    {
        /// <summary>
        /// 获取App多语言 类型选项
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAppLanguageType(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = ReqObj.Data.ToString();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string sql = $@"
SELECT
	ui_lan_type 'prop',
	ui_lan_type_name 'name'
FROM
	SJQDMS_UILAN_APP_M";
                var dt = DB.GetDataTable(sql);

                ret.IsSuccess = true;
                ret.RetData1 = dt;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 获取App多语言 翻译详情
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject GetAppLanguageTranslate(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();

            string Data = ReqObj.Data.ToString();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);

                string sql = $@"
SELECT
	dd.*
FROM
	SJQDMS_UILAN_APP_M m 
INNER JOIN SJQDMS_UILAN_APP_D d ON d.ui_lan_type=m.ui_lan_type
INNER JOIN SJQDMS_UILAN_APP_D_D dd ON dd.ui_lan_type=d.ui_lan_type AND dd.moudle_code=d.moudle_code";
                var dt = DB.GetDataTable(sql);
                var dt_list = dt.ToDataList<GetAppLanguageTranslateDto>();

                Dictionary<string, Dictionary<string, Dictionary<string, string>>> res = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
                var dt_listGroupByLanType = dt_list.GroupBy(x => x.ui_lan_type);
                foreach (var lan_type in dt_listGroupByLanType)
                {
                    Dictionary<string, Dictionary<string, string>> moudle_dic = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var moudle_item in lan_type.GroupBy(x=>x.moudle_code))
                    {
                        Dictionary<string, string> field_dic = new Dictionary<string, string>();
                        foreach (var filed_item in moudle_item)
                        {
                            field_dic.Add(filed_item.filed_code, filed_item.filed_name);
                        }
                        moudle_dic.Add(moudle_item.Key, field_dic);
                    }
                    res.Add(lan_type.Key, moudle_dic);
                }

                ret.IsSuccess = true;
                ret.RetData1 = res;
            }
            catch (Exception ex)
            {
                ret.IsSuccess = false;
                ret.ErrMsg = ex.Message;
            }

            return ret;
        }

        /// <summary>
        /// 移动端多语言查询 CS端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject SearchAppLanguageByCS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                DB.Open();
                DB.BeginTransaction();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);

                string sql = $@"
SELECT
	dd.*
FROM
	SJQDMS_UILAN_APP_M m 
INNER JOIN SJQDMS_UILAN_APP_D d ON d.ui_lan_type=m.ui_lan_type
INNER JOIN SJQDMS_UILAN_APP_D_D dd ON dd.ui_lan_type=d.ui_lan_type AND dd.moudle_code=d.moudle_code ";
                DataTable dt = DB.GetDataTablebyline(sql);
                var list = dt.ToDataList<GetAppLanguageTranslateDto>();

                var listGroupByMoudleAndField = list.GroupBy(x => new { x.moudle_code, x.filed_code });

                List<SearchAppLanguageByCSRes> res = new List<SearchAppLanguageByCSRes>();
                foreach (var item in listGroupByMoudleAndField)
                {
                    SearchAppLanguageByCSRes add = new SearchAppLanguageByCSRes()
                    {
                        moudle_code = item.Key.moudle_code,
                        filed_code = item.Key.filed_code
                    };
                    var find_cn = item.FirstOrDefault(x => x.ui_lan_type == "zh");
                    if (find_cn != null)
                        add.filed_name_cn = find_cn.filed_name;
                    var find_en = item.FirstOrDefault(x => x.ui_lan_type == "en");
                    if (find_en != null)
                        add.filed_name_en = find_en.filed_name;
                    var find_yn = item.FirstOrDefault(x => x.ui_lan_type == "yn");
                    if (find_yn != null)
                        add.filed_name_yn = find_yn.filed_name;

                    res.Add(add);
                }

                int rowCount = res.Count();

                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", res);
                dic.Add("rowCount", rowCount);

                ret.RetData = JsonConvert.SerializeObject(dic);
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
        /// 移动端多语言编辑 CS端
        /// </summary>
        /// <param name="OBJ"></param>
        /// <returns></returns>
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject EditAppLanguageByCS(object OBJ)
        {
            SJeMES_Framework_NETCore.WebAPI.RequestObject ReqObj = (SJeMES_Framework_NETCore.WebAPI.RequestObject)OBJ;
            SJeMES_Framework_NETCore.WebAPI.ResultObject ret = new SJeMES_Framework_NETCore.WebAPI.ResultObject();
            SJeMES_Framework_NETCore.DBHelper.DataBase DB = new SJeMES_Framework_NETCore.DBHelper.DataBase();
            try
            {
                DB = new SJeMES_Framework_NETCore.DBHelper.DataBase(string.Empty);
                string Data = ReqObj.Data.ToString();
                var jarr = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                string moudle_code = jarr["moudle_code"].ToString();
                string field_code = jarr["field_code"].ToString();
                string cn = jarr["cn"].ToString();
                string en = jarr["en"].ToString();
                string yn = jarr["yn"].ToString();

                DB.Open();
                DB.BeginTransaction();

                // cn
                Dictionary<string, object> paramDic = new Dictionary<string, object>();
                paramDic.Add("filed_name", cn);
                paramDic.Add("moudle_code", moudle_code);
                paramDic.Add("filed_code", field_code);
                paramDic.Add("ui_lan_type", "zh");
                string updateSql = $@"UPDATE SJQDMS_UILAN_APP_D_D SET filed_name=@filed_name WHERE moudle_code=@moudle_code AND filed_code=@filed_code AND ui_lan_type=@ui_lan_type";
                int updateCount = DB.ExecuteNonQuery(updateSql, paramDic);
                if (updateCount == 0)
                {
                    paramDic = new Dictionary<string, object>();
                    paramDic.Add("ui_lan_type", "zh");
                    paramDic.Add("moudle_code", moudle_code);
                    paramDic.Add("filed_code", field_code);
                    paramDic.Add("filed_name", cn);
                    string insertSql = $@"INSERT INTO SJQDMS_UILAN_APP_D_D (ui_lan_type, moudle_code, filed_code, filed_name) VALUES (@ui_lan_type, @moudle_code, @filed_code, @filed_name)";
                    DB.ExecuteNonQuery(insertSql, paramDic);
                }

                // en
                paramDic = new Dictionary<string, object>();
                paramDic.Add("filed_name", en);
                paramDic.Add("moudle_code", moudle_code);
                paramDic.Add("filed_code", field_code);
                paramDic.Add("ui_lan_type", "en");
                updateSql = $@"UPDATE SJQDMS_UILAN_APP_D_D SET filed_name=@filed_name WHERE moudle_code=@moudle_code AND filed_code=@filed_code AND ui_lan_type=@ui_lan_type";
                updateCount = DB.ExecuteNonQuery(updateSql, paramDic);
                if (updateCount == 0)
                {
                    paramDic = new Dictionary<string, object>();
                    paramDic.Add("ui_lan_type", "en");
                    paramDic.Add("moudle_code", moudle_code);
                    paramDic.Add("filed_code", field_code);
                    paramDic.Add("filed_name", en);
                    string insertSql = $@"INSERT INTO SJQDMS_UILAN_APP_D_D (ui_lan_type, moudle_code, filed_code, filed_name) VALUES (@ui_lan_type, @moudle_code, @filed_code, @filed_name)";
                    DB.ExecuteNonQuery(insertSql, paramDic);
                }

                // yn
                paramDic = new Dictionary<string, object>();
                paramDic.Add("filed_name", yn);
                paramDic.Add("moudle_code", moudle_code);
                paramDic.Add("filed_code", field_code);
                paramDic.Add("ui_lan_type", "yn");
                updateSql = $@"UPDATE SJQDMS_UILAN_APP_D_D SET filed_name=@filed_name WHERE moudle_code=@moudle_code AND filed_code=@filed_code AND ui_lan_type=@ui_lan_type";
                updateCount = DB.ExecuteNonQuery(updateSql, paramDic);
                if (updateCount == 0)
                {
                    paramDic = new Dictionary<string, object>();
                    paramDic.Add("ui_lan_type", "yn");
                    paramDic.Add("moudle_code", moudle_code);
                    paramDic.Add("filed_code", field_code);
                    paramDic.Add("filed_name", yn);
                    string insertSql = $@"INSERT INTO SJQDMS_UILAN_APP_D_D (ui_lan_type, moudle_code, filed_code, filed_name) VALUES (@ui_lan_type, @moudle_code, @filed_code, @filed_name)";
                    DB.ExecuteNonQuery(insertSql, paramDic);
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

    }

    #region dto
    public class GetAppLanguageTranslateDto
    {
        public string ui_lan_type { get; set; }
        public string moudle_code { get; set; }
        public string filed_code { get; set; }
        public string filed_name { get; set; }
    }
    public class SearchAppLanguageByCSRes
    {
        public string moudle_code { get; set; }
        public string filed_code { get; set; }
        public string filed_name_cn { get; set; }
        public string filed_name_en { get; set; }
        public string filed_name_yn { get; set; }
    }
    #endregion

}
