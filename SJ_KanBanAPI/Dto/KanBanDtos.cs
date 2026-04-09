using System;
using System.Collections.Generic;
using System.Text;

namespace SJ_KanBanAPI.Dto
{
    /// <summary>
    /// 柱状图
    /// </summary>
    public class KanBanDtos
    {
        public string type { get; set; }
        public string name { get; set; }
        public List<string> data { get; set; }
    }
}
