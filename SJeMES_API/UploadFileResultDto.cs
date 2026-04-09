using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SJeMES_API
{
    public class UploadFileResultDto
    {
        public string ErrMsg { get; set; }
        public bool IsSuccess { get; set; }
        public object ReturnObj { get; set; }
    }
}
