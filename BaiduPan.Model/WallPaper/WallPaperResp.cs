using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.WallPaper
{
    public class WallPaperResp :ResponseBase
    {
        public Wallpaper[] data { get; set; }

    }


    public class Wallpaper
    {
        public string id { get; set; }

        public string class_id { get; set; }

        public string resolution { get; set; }

        public string url_mobile { get; set; }

        public string url { get; set; }

        public string url_thumb { get; set; }

        public string url_mid { get; set; }

        public string download_times { get; set; }

        public string imgcut { get; set; }

        public string tag { get; set; }

        public DateTime create_time { get; set; }

        public DateTime update_time { get; set; }

        public string utag { get; set; }

        public string tempdata { get; set; }


        //public string rdata { get; set; }

        public string img_1600_900 { get; set; }

        public string img_1440_900 { get; set; }

        public string img_1366_768 { get; set; }

        public string img_1280_800 { get; set; }


        public string img_1280_1024 { get; set; }
        public string img_1024_768 { get; set; }


    }


}
