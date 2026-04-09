using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_QCMAPI
{
    public class ExternalColorCardDto
    {
        /// <summary>
        /// 改善方法
        /// </summary>
        public string ways { get; set; }
        /// <summary>
        /// 问题点
        /// </summary>
        public string qsInfo { get; set; }
        /// <summary>
        /// 检验数量
        /// </summary>
        public string SAMP_QTY { get; set; }
        /// <summary>
        /// 鞋型
        /// </summary>
        //public string SHOE_NO { get; set; }
        /// <summary>
        /// ART
        /// </summary>
        public string PROD_NO { get; set; }
        /// <summary>
        /// 检测项编号
        /// </summary>
        public string TESTITEM_CODE { get; set; }
        /// <summary>
        /// 检测项名称
        /// </summary>
        public string TESTITEM_NAME { get; set; }
        /// <summary>
        /// AC
        /// </summary>
        public string AC { get; set; }
        /// <summary>
        /// 检验结果
        /// </summary>
        public string CHECK_RESULT { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string REMARKS { get; set; }
        /// <summary>
        /// 检验标准
        /// </summary>
        public string TEST_STANDARD { get; set; }
        /// <summary>
        /// AQL级别
        /// </summary>
        public string AQL_LEVEL { get; set; }
        /// <summary>
        /// RE
        /// </summary>
        public string RE { get; set; }
        public List<DetailImg> imageList { get; set; }
    }
    public class DetailImg  {

        /// <summary>
        /// 图片名称
        /// </summary>
        public string img_name { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string img_url { get; set; }

    }

    public class code_name_obj
    {
        public string CODE { get; set; }
        public string NAME { get; set; }
    }

}
