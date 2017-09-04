using BaiduPan.Model.NetDiskInfo;
using BaiduPan.Model.ResultData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model
{
    public class ClientServer
    {

        public static ClientCredential AuthenticationMounted;

 

        const string MainServer = "http://pan.baidu.com";
        const string PCSServer = "http://d.pcs.baidu.com";
        const string DiskAPIServer = "http://diskapi.baidu.com";
        const string PasswordServer = "https://passport.baidu.com";
        const string PCSDataServer = "http://pcsdata.baidu.com";

        public ClientCredential Authentication { get; private  set; }

        public ClientServer(ClientCredential authentication)
        {
            Authentication = authentication;
        }
        private HttpWebRequest GetRequest(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = "netdisk;5.5.4.1;PC;PC-Windows;10.0.14393;WindowsBaiduYunGuanJia";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(new Cookie("STOKEN", Authentication.STOKEN, "/", "baidu.com"));
            request.CookieContainer.Add(new Cookie("BDUSS", Authentication.BDUSS, "/", "baidu.com"));
            request.SendChunked = false;
            return request;
        }


        /// <summary>
        /// 非API接口，弹出网页抓取Cookies保存在当前实例中。
        /// </summary>
        /// <returns></returns>

        public bool Login()
        {
            NewAccountForm loginWin = new NewAccountForm();

            if (loginWin.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Authentication = loginWin.Authentication;
                AuthenticationMounted = this.Authentication;

                var user = GetUserInfo().records.FirstOrDefault();

                GlobalConfig config = new GlobalConfig();
                config.Users.Add(new UserConfig {
                    UserName = user.UserName,
                    AvatarUrl = user.AvatarUrl,
                    SavedCredential = this.Authentication,
                    MaxSpeed = 10240000,
                });
                config.Save();

                return CheckIsLogined();

            }
            else
            {
                return false;
            }
        }

        public bool Logout()
        {
            try
            {
                HttpWebRequest request = GetRequest($"{PasswordServer}/?logout");
                request.GetResponse().GetResponseStream().Close();
                Authentication = new ClientCredential();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        public bool CheckIsLogined()
        {
            return GetQuotaInfo().errno == 0;
        }

        public NetDiskQuota GetQuotaInfo()
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/quota?checkexpire=1&checkfree=1");
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                var quotaInfo = JsonConvert.DeserializeObject<NetDiskQuota>(json);
                return quotaInfo;
            }
            catch (Exception)
            {
                return new NetDiskQuota();
            }
        }


        public UserInfoResult GetUserInfo()
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/user/getinfo");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write($"user_list=[{Authentication.UK}]");
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                var userInfo = JsonConvert.DeserializeObject<UserInfoResult>(json);

                return userInfo;
            }
            catch (Exception ex)
            {
                return new UserInfoResult();
            }
        }


        public NetDiskFileResult GetFileList(string path)
        {
            try
            {

                HttpWebRequest request = GetRequest($"{MainServer}/api/list?dir={Uri.EscapeDataString(path)}&page=1&num=100000");
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                var fileList = JsonConvert.DeserializeObject<NetDiskFileResult>(json);
                return fileList;


            }
            catch (Exception ex)
            {
                return new NetDiskFileResult();
            }

        }

        public NetDiskFileResult Search(string keyword, string path = "/")
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/search?recursion=1&dir={Uri.EscapeDataString(path)}&key={Uri.EscapeDataString(keyword)}&page=1&num=1000");
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<NetDiskFileResult>(json);
            }
            catch (Exception)
            {
                return new NetDiskFileResult();
            }
        }



        public DownloadResult GetDownload(string path)
        {
            try
            {
                HttpWebRequest request = GetRequest($"{PCSServer}/rest/2.0/pcs/file?app_id=250528&method=locatedownload&ver=4.0&path={Uri.EscapeDataString(path)}");
                request.Method = "POST";
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<DownloadResult>(json);
            }
            catch (Exception)
            {
                return new DownloadResult();
            }
        }





        public FileOperationResult DeleteFiles(string[] files)
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/filemanager?opera=delete");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write($"filelist={JsonConvert.SerializeObject(files)}");
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<FileOperationResult>(json);
            }
            catch (Exception)
            {
                return new FileOperationResult();
            }
        }
        public FileOperationResult CopyFiles(CopyMoveRequest[] requests)
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/filemanager?opera=copy");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write($"filelist={JsonConvert.SerializeObject(requests)}");
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<FileOperationResult>(json);
            }
            catch (Exception)
            {
                return new FileOperationResult();
            }
        }
        public FileOperationResult MoveFiles(CopyMoveRequest[] requests)
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/filemanager?opera=move");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write($"filelist={JsonConvert.SerializeObject(requests)}");
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<FileOperationResult>(json);
            }
            catch (Exception)
            {
                return new FileOperationResult();
            }
        }
        public FileOperationResult RenameFiles(RenameRequest[] requests)
        {
            try
            {
                HttpWebRequest request = GetRequest($"{MainServer}/api/filemanager?opera=rename");
                request.Method = "POST";
                StreamWriter writer = new StreamWriter(request.GetRequestStream());
                writer.Write($"filelist={JsonConvert.SerializeObject(requests)}");
                writer.Close();
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream());
                string json = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<FileOperationResult>(json);
            }
            catch (Exception)
            {
                return new FileOperationResult();
            }
        }









    }
}
