using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.Download
{

    /// <summary>
    /// Download File Info 
    /// </summary>
    public class DownloadInfo
    {
        /// <summary>
        ///  Total File Length
        /// </summary>
        public long ContentLength { get; set; }

        /// <summary>
        ///  Length Completed
        /// </summary>
        public long CompletedLength { get; set; }

        /// <summary>
        ///  whether download task is done;
        /// </summary>
        public bool IsCompleted { get; set; }

        /// <summary>
        /// Download url
        /// </summary>
        public string[] DownloadUrl { get; set; }

        /// <summary>
        ///  Download Path
        /// </summary>
        public string DownloadPath { get; set; }

        /// <summary>
        ///  Block Length
        /// </summary>
        public const long BlockLength = 1024 * 512;

        /// <summary>
        /// Block download
        /// </summary>
        public List<DownloadBlock> DownloadBlockList { get; } = new List<DownloadBlock>();


        /// <summary>
        ///  When Downloaded
        /// </summary>
        public DateTime CompletedTime { get; set; }

        public void Init(string path)
        {
           Init();
            Save(path);
        }

        public void Init()
        {
            var temp = 0L;
            DownloadBlockList.Clear();
            while (temp + BlockLength < ContentLength)
            {
                DownloadBlockList.Add(new DownloadBlock
                {
                    From = temp,
                    To = temp + BlockLength - 1,
                    IsCompleted = false,
                });
                temp += BlockLength;
            }

            DownloadBlockList.Add(new DownloadBlock
            {
                From = temp,
                To = ContentLength,
                IsCompleted = false,
            });

            CompletedTime = DateTime.Now;
        }

        public void Save(string path)
        {

            File.WriteAllText(path, JObject.Parse(JsonConvert.SerializeObject(this)).ToString());


        }

    }


    public class DownloadBlock
    {
        /// <summary>
        /// Start downloading position;
        /// </summary>
        public long From { get; set; }

        /// <summary>
        ///  End downloading position
        /// </summary>
        public long To { get; set; }

        /// <summary>
        /// length downloaded;
        /// </summary>
        public long CompletedLength { get; set; } = 0L;

        /// <summary>
        ///  Whether downloaded or not
        /// </summary>
        public bool IsCompleted { get; set; }

    }
}
