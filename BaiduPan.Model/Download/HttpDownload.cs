using BaiduPan.Infrastructure;
using BaiduPan.Infrastructure.Events;
using BaiduPan.Model.NetDiskInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BaiduPan.Model.Download
{



    public class HttpDownload
    {
        #region Property

        /// <summary>
        ///  Multi Download
        /// </summary>
        public string[] Url { get; set; }

        /// <summary>
        ///  Download Path
        /// </summary>
        public string DownloadPath { get; set; }


        /// <summary>
        ///  Thread Number
        /// </summary>
        public int ThreadNum => 20;

        /// <summary>
        ///  Download Speed
        /// </summary>
        public long DownloadSpeed { get; set; }


        /// <summary>
        ///  Download Progress
        /// </summary>
        public float DownloadPercentage { get; set; }


        /// <summary>
        ///  Download Status
        /// </summary>
        public DownloadStateEnum DownloadState
        {
            get { return _state; }

            private set
            {
                if (_state != value)
                {
                    lock (this)
                    {
                        DownloadStateChangedEvent.Invoke(this,
                            new DownloadStateChangedEventArgs(DownloadState,
                            value,
                            TaskDatabase.GetDatabaseByUser(MountUserRepository.MountedUser).GetFileIdByDownloadInfo(Info)));
                    }
                    _state = value;
                }
            }
        }


        public DownloadInfo Info { get; set; }


        #endregion  Propeprty





        #region  Field

        private float _percentage = -1F;
        private long _speed = 0L;
        private DownloadStateEnum _state = DownloadStateEnum.Waiting;
        private List<DownloadThread> _threads = new List<DownloadThread>();


        int _completedThread = 0;


        #endregion Field



        public void Start()
        {

            if (Url == null || Url.Length == 0 || ThreadNum < 1 || Info == null)
            {
                MessageBox.Show("下载参数出错!");
                return;
            }


            try
            {

                if (Info.IsCompleted)
                {
                    DownloadState = DownloadStateEnum.Completed;
                    DownloadPercentage = 100F;
                    DownloadSpeed = 0L;
                    return;
                }

                DownloadState = DownloadStateEnum.Downloading;
                var response = GetResponse();
                if (response == null)
                {
                    DownloadState = DownloadStateEnum.Faulted;
                    return;
                }

                CreateDownloadPath(response.ContentLength);


                new Thread(ReportDownloadProgress).Start();


                var num = 0;
                var leftTaskCount = (ThreadNum + _completedThread > Info.DownloadBlockList.Count ? Info.DownloadBlockList.Count : ThreadNum + _completedThread);
                for (int i = _completedThread; i < leftTaskCount; i++)
                {
                    var block = Info.DownloadBlockList[i];

                    if (block.IsCompleted)
                    {
                        continue;
                    }

                    if (num >= Url.Length)
                    {
                        num = 0;
                    }
                    var threadItem = new DownloadThread()
                    {
                        ID = i,
                        DownloadUrl = Url[num],
                        Path = DownloadPath,
                        Block = block,
                        Info = Info,
                    };
                    threadItem.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
                    _threads.Add(threadItem);

                    num++;
                }


            }
            catch (Exception ex)
            {
                DownloadState = DownloadStateEnum.Faulted;
            }


        }

        int urlNum = 0;
        private void HttpDownload_ThreadCompletedEvent(string path)
        {
            lock (this)
            {
                _completedThread++;


                if (DownloadState == DownloadStateEnum.Paused)
                {
                    return;
                }

                Debug.WriteLine($"_completedThread : {_completedThread}    Total Task : {Info.DownloadBlockList.Count}  ");

                if (_completedThread >= Info.DownloadBlockList.Count)
                {
                    DownloadSpeed = 0L;
                    DownloadPercentage = 100F;
                    Info.IsCompleted = true;
                    Info.CompletedTime = DateTime.Now;
                    DownloadState = DownloadStateEnum.Completed;

                    DeleteTempFile();
                }
                else
                {
                    if (_completedThread + ThreadNum - 1 > Info.DownloadBlockList.Count)
                    {
                        return;
                    }

                    ManagerThread();

                }


            }
        }

        private void ManagerThread()
        {
            var block = Info.DownloadBlockList[_completedThread + ThreadNum - 1];
            if (urlNum >= Url.Length)
            {
                urlNum = 0;
            }
            var threadItem = new DownloadThread()
            {
                ID = _completedThread + ThreadNum - 1,
                DownloadUrl = Url[urlNum],
                Path = DownloadPath,
                Block = block,
                Info = Info,
            };
            threadItem.ThreadCompletedEvent += HttpDownload_ThreadCompletedEvent;
            urlNum++;
            _threads.Add(threadItem);
        }

        private void DeleteTempFile()
        {
            try
            {
                if (File.Exists(DownloadPath + ".downloading"))
                    File.Delete(DownloadPath + ".downloading");


            }
            catch (Exception ex)
            {
                Debug.WriteLine("删除文件 " + DownloadPath + ".downloading   时出现错误");
            }
        }

        private void ReportDownloadProgress()
        {
            var temp = 0L;
            while (DownloadState == DownloadStateEnum.Downloading)
            {
                Thread.Sleep(1000);
                if (temp == 0)
                {
                    temp = Info.CompletedLength;
                }
                else
                {
                    if (DownloadState == DownloadStateEnum.Downloading)
                    {
                        DownloadSpeed = Info.CompletedLength - temp;
                        DownloadPercentage = Info.CompletedLength;
                        DownloadProgressChangedEvent?.Invoke(this,
                            new DownloadPercentageChangedEventArgs(
                                new DataSize(DownloadPercentage),
                                new DataSize(DownloadSpeed)));

                        temp = Info.CompletedLength;
                    }
                }

            }
        }


        public  DownloadInfo CreateData()
        {

            var response = GetResponse();
            if (response == null)
            {
                throw new NullReferenceException("下载链接已失效");
            }

            var info = new DownloadInfo
            {
                ContentLength = response.ContentLength,
                DownloadUrl = Url,
                DownloadPath = DownloadPath,
            };

            info.Init();
            return info;

           
        }


        public async Task<DownloadInfo> CreateData(string path)
        {

            var info = CreateData();
            info.Init(path);
            return info;

        }

        public static HttpDownload GetTaskByInfo(DownloadInfo info)
        {

            return new HttpDownload()
            {
                Url = info.DownloadUrl,
                DownloadPath = info.DownloadPath,
                Info = info,
            };


        }

        public static  DownloadInfo CreateTaskInfo(string[] urls, string downloadPath,
            int threadNum = 32)
        {

            return  new HttpDownload()
            {
                Url = urls,
                DownloadPath = downloadPath,
            }.CreateData();

        }



        public DownloadInfo StopAndSave()
        {
            if (_threads != null)
            {
                DownloadState = DownloadStateEnum.Paused;

                return Info;
            }

            return null;
        }


        public override bool Equals(object obj)
        {
            if (!(obj is HttpDownload))
            {
                return false;
            }

            var download = (HttpDownload)obj;
            return download.DownloadPath == DownloadPath;
        }


        public override int GetHashCode()
        {
            return DownloadPath.GetHashCode();
        }




        private HttpWebResponse GetResponse()
        {
            foreach (var sub in Url)
            {
                try
                {
                    var request = WebRequest.Create(sub) as HttpWebRequest;
                    request.SendChunked = false;
                    request.KeepAlive = false;
                    request.ReadWriteTimeout = 800;
                    request.Timeout = 2000;

                    return (HttpWebResponse)request.GetResponse();

                }
                catch (WebException ex)
                {

                    if (ex.Message.Contains("超时") || ex.Message.Contains("Timeout"))
                    {
                        return GetResponse();
                    }

                }
            }

            return null;
        }


        private void CreateDownloadPath(long length)
        {

            if (!File.Exists(DownloadPath))
            {
                using (var stream = new FileStream(DownloadPath, FileMode.CreateNew, FileAccess.ReadWrite,
                    FileShare.ReadWrite, 1024 * 1024))
                {
                    stream.SetLength(length);
                }
            }
        }

        #region Events




        public  DownloadStateChanged DownloadStateChangedEvent;

        public  DownloadPercentageChanged DownloadProgressChangedEvent;

        #endregion Events
    }

    //public delegate void DownloadPercentageChanged(object sender, DownloadPercentageChangedEventArgs e);
    //public delegate void DownloadStateChanged(object sender, DownloadStateChangedEventArgs e);
    //public delegate void Test();
}
