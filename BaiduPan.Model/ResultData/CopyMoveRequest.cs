using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.ResultData
{
    public class CopyMoveRequest
    {
        public string path { get; set; }
        public string dest { get; set; }
        public string newname { get; set; }
    }
}
