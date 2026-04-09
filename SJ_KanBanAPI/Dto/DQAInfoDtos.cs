using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_KanBanAPI.Dto
{
    public class DQAInfoDtos
    {
        /// <summary>
        /// 季度
        /// </summary>
        public string Develop_Season { get; set; }
        /// <summary>
        /// 鞋型数量
        /// </summary>
        public string ShoesNum { get; set; }
        /// <summary>
        /// Art数量
        /// </summary>
        public string Prod_No { get; set; }
        /// <summary>
        /// 改善合计数量
        /// </summary>
        public string GsNum { get; set; }
        /// <summary>
        /// 改善比率
        /// </summary>
        public string GsNumRate { get; set; }
        /// <summary>
        /// 待改善
        /// </summary>
        public string DGsNum { get; set; }
        /// <summary>
        /// 待改善比率
        /// </summary>
        public string DGsNumRate { get; set; }
        /// <summary>
        /// FD数量
        /// </summary>
        public string FD { get; set; }
        /// <summary>
        /// FD比率
        /// </summary>
        public string FDRate { get; set; }

        /// <summary>
        /// VS数量
        /// </summary>
        public string VS { get; set; }
        /// <summary>
        /// VS比率
        /// </summary>
        public string VSRate { get; set; }
    }
}
