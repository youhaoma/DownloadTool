using BaiduPan.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BaiduPan.Infrastructure.Interfaces.Files;
using Newtonsoft.Json;

namespace BaiduPan.Model.NetDiskInfo
{
    public class UserInfoResult
    {
        public int errno { get; set; }
        public long request_id { get; set; }
        public NetDiskUser[] records { get; set; }
    }
    public class NetDiskUser :  INetDiskUser
    {
        public NetDiskUser()
        {

            RootFile = new NetDiskFile(this);

        }

        public long uk { get; set; }
        //public string uname { get; set; }
        public string nick_name { get; set; }
        public string intro { get; set; }
        //public string avatar_url { get; set; }
        public int follow_flag { get; set; }
        public int black_flag { get; set; }
        public string follow_source { get; set; }
        public string display_name { get; set; }
        public string remark { get; set; }
        public string priority_name { get; set; }

        #region Public properties
        /// <summary>
        /// 用户名
        /// </summary>

        [JsonProperty("uname")]
        public string UserName { get; private set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [JsonIgnore]
        public string NickName { get; private set; }

        /// <summary>
        /// 头像URL
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; private set; }

        /// <summary>
        /// 总容量
        /// </summary>
        [JsonIgnore]
        public long TotalSpace { get; private set; }

        /// <summary>
        /// 剩余容量
        /// </summary>
        [JsonIgnore]
        public long FreeSpace { get; private set; }

        /// <summary>
        /// 已用容量
        /// </summary>
        [JsonIgnore]
        public long UsedSpace { get; private set; }

        /// <summary>
        /// Gets the root file, which represents root directory of the net-disk.
        /// </summary>
        [JsonIgnore]
        public INetDiskFile RootFile { get; }


        [JsonIgnore]
        public bool IsOnline { get; set; }


        [JsonIgnore]
        internal string DownloadDirectory => @"D:\Test\";


        #endregion
        public Task<Uri> GetFilesSharedAsync(IEnumerable<INetDiskUser> files, string password = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ILocalDiskFile> GetTaskCompleted()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDiskFile> GetUncompletedTask()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
