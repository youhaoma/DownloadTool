using BaiduPan.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.NetDiskInfo
{
    public class NetDiskQuota
    {
        public int errno { get; set; }
        public long total { get; set; }
        public long free { get; set; }
        public long request_id { get; set; }
        public bool expire { get; set; }
        public long used { get; set; }

        [JsonIgnore]
        public DataSize Used => new DataSize(used);


        [JsonIgnore]
        public DataSize Total => new DataSize(total);
    }
}
