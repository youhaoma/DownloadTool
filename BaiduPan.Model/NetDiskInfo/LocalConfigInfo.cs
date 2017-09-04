using BaiduPan.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduPan.Infrastructure;
using System.IO;
using Newtonsoft.Json;

namespace BaiduPan.Model.NetDiskInfo
{
    public class LocalConfigInfo : ILocalConfigInfo
    {

        private static readonly string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        public string Theme { get; set; }
        public string Background { get ; set ; }
        public LanguageEnum Language { get; set; } = LanguageEnum.Chinese;
        public bool IsDisplayDownloadDialog { get; set; } = true;
        public string DownloadDictionary { get; set; }
        public int ParalleTaskNum { get; set; }
        public double SpeedLimited { get; set; }

        public void Save()
        {
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(this));
        }


        public static LocalConfigInfo CreateConfigInfo()
        {
            LocalConfigInfo configInfo;
            if (File.Exists(FilePath))
            {
                var info = File.ReadAllText(FilePath);
                configInfo = JsonConvert.DeserializeObject<LocalConfigInfo>(info);
                if (configInfo != null)
                {
                    return configInfo;
                }
                                
            }

            configInfo = new LocalConfigInfo();
            configInfo.Save();
            return configInfo;
        }

    }
}
