using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SJ_TSMAPI
{
    class Multi_Skill_Bonus_Calculation
    {
        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Skill_Bonus(object OBJ)
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
                string ProcessType = jarr.ContainsKey("ProcessType") ? jarr["ProcessType"].ToString() : "";
                string ProdLine = jarr.ContainsKey("ProdLine") ? jarr["ProdLine"].ToString() : "";
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string Month = jarr.ContainsKey("Month") ? jarr["Month"].ToString() : "";
                string Att_Date = Month.Replace("/", "");
                string sql = string.Empty;
                string where1 = string.Empty;

                if (!string.IsNullOrEmpty(ProcessType))
                {
                    where1 += $@" and c.process_type = '{ProcessType}'";
                }
                if (!string.IsNullOrEmpty(ProdLine))
                {
                    where1 += $@" AND a.dept_name like '%{ProdLine}%'";
                }
                if (!string.IsNullOrEmpty(Barcode))
                {
                    where1 += $@" AND a.emp_no = '{Barcode}'";
                }
                if (!string.IsNullOrEmpty(Month))
                {
                    where1 += $@" and b.createddate like'%{Month}%'";
                }


                sql = $@"WITH tmp AS (
    SELECT
        c.emp_no,
        c.emp_name,
        a.department_code AS dept_name,
        c.work_name
    FROM
        base005m a,
        t_oa_mes_department_compare b,
        t_oa_empmain c
    WHERE
        a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
        AND b.OA_DEPARTMENTCODE = c.DEPT_NO
        AND c.STATUS = '1'
        --AND a.Factory_Sap IS NOT NULL
),tmp2 as (
SELECT
    a.emp_no,
    a.emp_name,
    a.dept_name,
    a.work_name,
    COUNT(CASE WHEN c.skill_type2 = 'Unskill' THEN 1 END) AS Unskill,
    COUNT(CASE WHEN c.skill_type2 = 'Semi Skill' THEN 1 END) AS SemiSkill,
    COUNT(CASE WHEN c.skill_type2 = 'Key skill' THEN 1 END) AS KeySkill,
    LISTAGG(b.skill_name, ',') WITHIN GROUP (ORDER BY b.skill_score) AS skill_set
FROM
    tmp a
LEFT JOIN t_tsm_emp_skill_d b ON a.emp_no = b.emp_no
LEFT JOIN t_tsm_processlist c ON c.name = b.skill_name
WHERE
    1 = 1{where1}
GROUP BY
    a.emp_no,
    a.emp_name,
    a.dept_name,
    a.work_name) ,tmp3 as 
    (select
    b.emp_no,
    b.emp_name,
    b.dept_name,
    b.work_name,
    b.skill_set
    ,F_Skill_Grade_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) as Grade
    ,F_Skill_Bonus_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) as Bonus
    from 
    tmp2 b  where F_Skill_Grade_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) is not null)
,tmp4 as 
      ( select A.BARCODE,round(avg((a.iescore / 70) * 30), 0) as IE_Score,
           round(avg((a.qipscore / 30) * 30), 0) as QIP_Score,
           round(avg((a.iescore / 70) * 30)*0.7, 0)+round(avg((a.qipscore / 30) * 30)*0.3, 0) as Skill_Score,
           F_Att_Bonus_Calc(A.BARCODE,'{Att_Date}') as Att_Bonus
      from t_tsm_skill_score_evaluation_d a
     where a.createddate like '%{Month}%'
        or a.modifieddate like '%{Month}%' group by A.BARCODE) 
        select  a.emp_no,
    a.emp_name,
    a.dept_name,
    a.work_name,
    a.skill_set,
   a.Grade,
   a.Bonus,
   b.IE_Score,
   b.QIP_Score, 
   b.Skill_Score,
   b.Att_Bonus,
  ((b.IE_Score/100)+(b.QIP_Score/100)+(b.Skill_Score/100)+(b.Att_Bonus/100))*100 as Percentage,
    ((b.IE_Score/100)+(b.QIP_Score/100)+(b.Skill_Score/100)+(b.Att_Bonus/100))*a.Bonus as actualbonus
     from tmp3 a inner join tmp4 b on a.emp_no=b.barcode order by a.dept_name
       ";
                DataTable dt = DB.GetDataTable(sql); 
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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

        public static SJeMES_Framework_NETCore.WebAPI.ResultObject Get_Skill_Bonus_Eligible_List(object OBJ)
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
                string ProcessType = jarr.ContainsKey("ProcessType") ? jarr["ProcessType"].ToString() : "";
                string ProdLine = jarr.ContainsKey("ProdLine") ? jarr["ProdLine"].ToString() : "";
                string Barcode = jarr.ContainsKey("Barcode") ? jarr["Barcode"].ToString() : "";
                string Month = jarr.ContainsKey("Month") ? jarr["Month"].ToString() : "";
                string Att_Date = Month.Replace("/", "");
                string sql = string.Empty;
                string where1 = string.Empty;
                if (!string.IsNullOrEmpty(ProcessType))
                {
                    where1 += $@" and c.process_type = '{ProcessType}'";
                }
                if (!string.IsNullOrEmpty(ProdLine))
                {
                    where1 += $@" AND a.dept_name like '%{ProdLine}%'";
                }
                if (!string.IsNullOrEmpty(Barcode))
                {
                    where1 += $@" AND a.emp_no = '{Barcode}'";
                }
                //if (!string.IsNullOrEmpty(Month))
                //{
                //    where1 += $@" and b.createddate like'%{Month}%'";
                //}


                sql = $@"WITH tmp AS (
    SELECT
        c.emp_no,
        c.emp_name,
        a.department_code AS dept_name,
        c.work_name
    FROM
        base005m a,
        t_oa_mes_department_compare b,
        t_oa_empmain c
    WHERE
        a.DEPARTMENT_CODE = b.MES_DEPARTMENTCODE
        AND b.OA_DEPARTMENTCODE = c.DEPT_NO
        AND c.STATUS = '1'
        --AND a.Factory_Sap IS NOT NULL
),tmp2 as (
SELECT
    a.emp_no,
    a.emp_name,
    a.dept_name,
    a.work_name,
    COUNT(CASE WHEN c.skill_type2 = 'Unskill' THEN 1 END) AS Unskill,
    COUNT(CASE WHEN c.skill_type2 = 'Semi Skill' THEN 1 END) AS SemiSkill,
    COUNT(CASE WHEN c.skill_type2 = 'Key skill' THEN 1 END) AS KeySkill,
    LISTAGG(b.skill_name, ',') WITHIN GROUP (ORDER BY b.skill_score) AS skill_set
FROM
    tmp a
LEFT JOIN t_tsm_emp_skill_m b ON a.emp_no = b.emp_no
LEFT JOIN t_tsm_processlist c ON c.name = b.skill_name
WHERE
    1 = 1 {where1}
GROUP BY
    a.emp_no,
    a.emp_name,
    a.dept_name,
    a.work_name)   
    ,tmp3 as 
    (select
    b.emp_no,
    b.emp_name,
    b.dept_name,
    b.work_name,
    b.skill_set
    ,F_Skill_Grade_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) as Grade
    ,F_Skill_Bonus_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) as Bonus
    from 
    tmp2 b  where F_Skill_Grade_Calc('{ProcessType}',b.KeySkill,b.SemiSkill,b.Unskill) is not null) select * from tmp3 c  order by c.dept_name
       ";
                DataTable dt = DB.GetDataTable(sql);
                DB.Commit();
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("Data", dt);
                ret.IsSuccess = true;
                ret.RetData = Newtonsoft.Json.JsonConvert.SerializeObject(dic);
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
