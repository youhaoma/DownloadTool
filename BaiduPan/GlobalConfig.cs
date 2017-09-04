using BaiduPan.Model.NetDiskInfo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan
{
    class GlobalConfig
    {
        private GlobalConfig()
        {

        }
        public static GlobalConfig Load(string configFile = "config.json")
        {
            if (!File.Exists(configFile))
                return new GlobalConfig();
            return JsonConvert.DeserializeObject<GlobalConfig>(File.ReadAllText(configFile));
        }
        public void Save(string configFile = "config.json")
        {
            File.WriteAllText(configFile, JsonConvert.SerializeObject(this));
        }

        public string DefaultDownloadPath { get; set; } = Environment.CurrentDirectory;
        public ClientCredential SavedCredential { get; set; }
        public bool AutoClose { get; set; } = false;
        public long MaxSpeed { get; set; } = 0;
        public DateTime LastSpeedTest { get; set; } = DateTime.Now;
    }
}
