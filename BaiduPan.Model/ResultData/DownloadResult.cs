using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.ResultData
{
    public class DownloadResult
    {
        public string client_ip { get; set; }
        public Url[] urls { get; set; }
        public RankParam rank_param { get; set; }
        public int sl { get; set; }
        public int max_timeout { get; set; }
        public int min_timeout { get; set; }
        public long request_id { get; set; }


        [JsonIgnore]
        public string[] Urls
        {
            get
            {
                List<string> allUrl = new List<string>();
                foreach (var url in urls)
                {
                    allUrl.Add(url.url);
                }
                return allUrl.ToArray();
            }
        }
    }

    public class RankParam
    {
        public int max_continuous_failure { get; set; }
        public int bak_rank_slice_num { get; set; }
    }

    public class Url
    {
        public string url { get; set; }
        public int rank { get; set; }
    }
}
