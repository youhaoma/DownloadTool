using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.WallPaper
{
    public class WallPaperService
    {

        public static IDictionary<int, string> CategoryDic;

        static HttpClient client = new HttpClient();

        const string categoryUrl = "http://cdn.apc.360.cn/index.php?c=WallPaper&a=getAllCategoriesV2&from=360chrome";
        const string wallpaperUrl = "http://wallpaper.apc.360.cn/index.php?c=WallPaper&a=getAppsByCategory&cid={0}&start={1}&count={2}&from=360chrome";

        public  async Task<IList<Category>> GetWallPaperCategory()
        {
            List<Category> allCategory;

            var json = await client.GetStringAsync(new Uri(categoryUrl));
            var category = JsonConvert.DeserializeObject<CategoryResp>(json);
            if (category == null)
            {
                return new List<Category>();
            }
            if (category.errno != "0")
            {
                throw new Exception($"错误码:{category.errno}; 错误信息:{category.errmsg}");
            }

            allCategory = category.data.ToList();



            //CategoryDic = new Dictionary<int, string>();
            //foreach (var item in allCategory)
            //{
            //    CategoryDic.Add(int.Parse(item.id),  );
            //}

            return allCategory;
        }


        public  async Task<List<Wallpaper>> GetWallPaperByCategory(string categoryId, int startNum, int endNum)
        {

            List<Wallpaper> allWallpaper;

            var json = await client.GetStringAsync(new Uri(string.Format(wallpaperUrl, categoryId, startNum, endNum)));

            var wallpaperResps = JsonConvert.DeserializeObject<WallPaperResp>(json);
            if (wallpaperResps == null)
            {
                return new List<Wallpaper>();
            }

            if (wallpaperResps.errno != "0")
            {
                throw new Exception($"错误码:{0}; 错误信息:{1}" + wallpaperResps.errno + wallpaperResps.errmsg);
            }

            allWallpaper = wallpaperResps.data.ToList();

            return allWallpaper;
        }


    }
}
