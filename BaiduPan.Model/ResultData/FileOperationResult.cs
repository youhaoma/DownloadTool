using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.ResultData
{
    public class FileOperationResult
    {
        public int errno { get; set; }
        public Info[] info { get; set; }
        public long request_id { get; set; }
    }

    public class Info
    {
        public int errno { get; set; }
        public string path { get; set; }
    }
}
