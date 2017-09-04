using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.WallPaper
{
    public class CategoryResp : ResponseBase
    {

        public Category[] data { get; set; }

    }


    public class Category
    {

        public string id { get; set; }

        public string name { get; set; }

        public string order_num { get; set; }

        public string tag { get; set; }

        public string create_time { get; set; }




    }







}
