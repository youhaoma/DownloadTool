using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPan.Model.Download
{
    public class DownloadThread
    {


        #region  Property
        public int ID { get; set; }
        public string DownloadUrl { get; set; }
        public string Path { get; set; }

        public string PathTmp => Path + ".tmp";
        public DownloadBlock Block { get; set; }
        public DownloadInfo Info { get; set; }


        public System.Threading.ThreadState State => _workThread.ThreadState;



        #endregion  Property

        public event Action<string> ThreadCompletedEvent;

        bool _stoped;

        private Thread _workThread;
        private HttpWebRequest _request;
        private HttpWebResponse _response;


        private Dictionary<string, int> urlStatistic = new Dictionary<string, int>();
        private List<string> validUrl => Info.DownloadUrl.ToList();
        internal DownloadThread()
        {
            ServicePointManager.DefaultConnectionLimit = 1000;

            _workThread = new Thread(Start) { IsBackground = true };
            
            _workThread.Start();

            
        }


        public void Start()
        {            
            
            try
            {

                Thread.Sleep(300);
                _request?.Abort();
                _response?.Close();

                if (_stoped)
                {
                    return;
                }

                if (Block.IsCompleted)
                {
                    ThreadCompletedEvent?.Invoke(Path);
                    return;
                }

                 _request = WebRequest.Create(DownloadUrl) as HttpWebRequest;
                _request.SendChunked = false;
                _request.KeepAlive = false;
                _request.ReadWriteTimeout = 2000;
                _request.Timeout = 2000;
                _request.AddRange("bytes", Block.From, Block.To);


                _response = _request.GetResponse() as HttpWebResponse;

                if (!File.Exists(Path))
                {
                    return;
                }

                using (var responseStream = _response.GetResponseStream())
                {


                    using (var stream = new FileStream(Path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, 1024 * 1024))
                    {
                        stream.Seek(Block.From, SeekOrigin.Begin);
                        var array = new byte[1024];
                        var i = responseStream.Read(array, 0, array.Length);
                        while (true)
                        {
                            if (i <= 0 && Block.From - 1 != Block.To && Block.From != Block.To)
                            {
                                //发送空数据,放弃这个链接重试
                                _workThread = new Thread(Start) { IsBackground = true };
                                _workThread.Start();
                                return;
                            }
                            if (i <= 0)
                            {
                                break;
                            }
                            stream.Write(array, 0, i);
                            Block.From += i;
                            Block.CompletedLength += i;
                            Info.CompletedLength += i;
                            Info.DownloadBlockList[ID] = Block;

                            i = responseStream.Read(array, 0, array.Length);

                        }
                        Block.IsCompleted = true;
                        ThreadCompletedEvent?.Invoke(Path);
                    }
                }


                _response?.Close();
                _request?.Abort();
                

            }
            catch (Exception ex)
            {

                GC.Collect();

                if (ex.Message.Contains("操作"))
                {
                    Debug.WriteLine(ex.StackTrace);

                    //NextUrl();
                    //goto Retry;
                }


                if (ex is ThreadAbortException)
                {
                    return;
                }

                if (ex.Message.Contains("终止") || ex.Message.Contains("取消"))
                {
                    return;
                }

                


                NextUrl();
                _workThread = new Thread(Start) { IsBackground = true };
                _workThread.Start();



            }
        }

        private int _num;
        private int _orderNum;
        private void NextUrl()
        {

            if (urlStatistic.ContainsKey(DownloadUrl))
            {
                if (++(urlStatistic[DownloadUrl]) > 3)
                {
                    validUrl.Remove(DownloadUrl);
                }
            }
            else
            {
                urlStatistic.Add(DownloadUrl, 1);
            }



            _num++;
            if (_num >= validUrl.Count)
            {
                _num = 0;
            }

            DownloadUrl = validUrl[_num];


        }


        public void Stop()
        {
            if (Block.IsCompleted)
            {
                return;
            }

            _stoped = true;
            _workThread.Abort();
            _request?.Abort();
            _response?.Close();
        }


         
    }
}
