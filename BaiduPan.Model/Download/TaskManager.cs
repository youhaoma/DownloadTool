using BaiduPan.Infrastructure;
using BaiduPan.Infrastructure.Events;
using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Infrastructure.Interfaces.Files;
using BaiduPan.Model.NetDiskInfo;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BaiduPan.Model.Download
{
    public class TaskManager
    {
        private static readonly Dictionary<string, TaskManager> Manager = new Dictionary<string, TaskManager>();

        private readonly TaskDatabase _database;
        private readonly string _dataFolder;
        private readonly List<HttpDownload> _downloadingTasks = new List<HttpDownload>();
        private bool _running;


        public string TaskListFile => Path.Combine(_dataFolder, "TaskList.json");

        private  INetDiskUser userMounted
        {
            get
            {
                return MountUserRepository.MountedUser;
            }
        }


        public TaskManager()
        {
            _dataFolder = Path.Combine(Directory.GetCurrentDirectory() + @"\Data\", userMounted.UserName);

            if (!(Directory.Exists(_dataFolder)))
            {
                Directory.CreateDirectory(_dataFolder);
            }

            _database = TaskDatabase.GetDatabaseByUser(userMounted);
            _running = true;


        }

        public static TaskManager GetTaskManagerByLocalDiskUser(INetDiskUser user)
        {
            lock (Manager)
            {
                if (!Manager.ContainsKey(user.UserName))
                {
                    Manager.Add(user.UserName,
                        new TaskManager());
                } 
            }

            return Manager[user.UserName];
        }


        public void AddDownloadTask(DownloadInfo info)
        {
            var download = HttpDownload.GetTaskByInfo(info);
            download.DownloadStateChangedEvent += Download_DownloadStateChangedEvent;
            download.DownloadProgressChangedEvent += Download_DownloadProgressChangedEvent;



            //if (downloadingPageStateChanged != null)
            //{
            //    download.DownloadStateChangedEvent += downloadingPageStateChanged;

            //}
            //if (downloadingPageProgressChanged != null)
            //{
            //    download.DownloadProgressChangedEvent += downloadingPageProgressChanged;
            //}

            _downloadingTasks.Add(download);

            download.Start();
            

            TaskCollectionChanged?.Invoke(download);
        }

        private void Download_DownloadProgressChangedEvent(object sender, DownloadPercentageChangedEventArgs e)
        {
            var info = (HttpDownload)sender;

            Debug.WriteLine($"PercentageChanged: Task: {info.DownloadPath} Speed: {e.CurrentSpeed} Progress: {e.CurrentProgress}");


        }

        private async void Download_DownloadStateChangedEvent(object sender, DownloadStateChangedEventArgs e)
        {
            var info = (HttpDownload)sender;

            Debug.WriteLine($"PercentageChanged: Task: {info.DownloadPath} State: {e.OldState} -> {e.NewState}");


            var fileId = _database.GetFileIdByPath(info.DownloadPath);
            switch (e.NewState)
            {
                case DownloadStateEnum.Completed:
                    _database.SetCompleted(info.DownloadPath);
                    _downloadingTasks.Remove(info);
                    GetNextTask();
                    break;
                case DownloadStateEnum.Waiting:
                    _downloadingTasks.Remove(info);
                    //暂停所做操作
                    break;
            }
        }


        public LocalDiskFile[] GetUnCompletedList()
        {
            return _database.GetUnCompletedList().Select(item => new LocalDiskFile(item.Id,
                item.DownloadPath,
                item.DownloadFileInfo.FileType,
                item.DownloadFileInfo.FileSize,
                DateTime.Now)).ToArray();
        }
        public LocalDiskFile[] GetCompletedList()
        {
            return _database.GetCompletedList().Select(item => new LocalDiskFile(item.Id,
                item.DownloadPath,
                item.FileInfo.FileType,
                item.FileInfo.FileSize,
                item.CompletedTime)).ToArray();
        }


        public List<HttpDownload> GetHttpDownloads()
        {
            return _downloadingTasks;
        }

        public void StopAndSave()
        {
            _running = false;
            _downloadingTasks.ForEach(item =>
            {
                var info = item.StopAndSave();
                _database.UpdateTask(info);
            });

        }


        public void PauseTask(long id)
        {
            if (!_database.Contains(id))
            {
                return;
            }

            if (_downloadingTasks.Any(item => item.DownloadPath == _database.GetFilePathById(id)))
            {
                _downloadingTasks.ForEach(item =>
                {
                    if (item.DownloadPath == _database.GetFilePathById(id))
                    {
                        var info = item.StopAndSave();
                        if (info != null)
                        {
                            _database.UpdateTask(info);
                        }
                    }
                });
            }

        }


        public void ContinueTask(long id)
        {

            if (!_database.Contains(id)) return;

            if (_downloadingTasks.Any(item => item.DownloadPath == _database.GetFilePathById(id)
            && item.DownloadState != DownloadStateEnum.Downloading))
            {

                _downloadingTasks.ForEach(item =>
                {

                    if (item.DownloadPath == _database.GetFilePathById(id)
                    && item.DownloadState != DownloadStateEnum.Downloading)
                    {
                        item.Start();
                    }

                });
            }

        }


        public async Task RemoveTask(long id)
        {

            await Task.Run( () =>
            {
                if (_database.GetFilePathById(id) != string.Empty)
                {
                    var path = _database.GetFilePathById(id);
                    if (_downloadingTasks.Any(item => item.DownloadPath == path))
                    {
                        var Task = _downloadingTasks.FirstOrDefault(item => item.DownloadPath == path);
                        Task.StopAndSave();
                        _downloadingTasks.Remove(Task);
                    }
                }

                _database.RemoveTask(id);
            });





        }


        public bool CreateTask(NetDiskFile file, string path)
        {
            if (_database.Contains(path))
            {
                return false;
            }

            
            _database.Add(file, path);

            return true;
        }

        public  void StartTask()
        {

            TaskAddedCompleted?.Invoke();

            CheckTask();
            //ThreadPool.QueueUserWorkItem( async(para) => await CheckTask());

           




        }




        private  void CheckTask()
        {
            int count = _downloadingTasks.Count(item => item.DownloadState == DownloadStateEnum.Downloading)
            < GlobalConfig.TaskNum ? _downloadingTasks.Count(item => item.DownloadState == DownloadStateEnum.Downloading) : GlobalConfig.TaskNum;

            for (int i = 0; i < GlobalConfig.TaskNum; i++)
            {

                GetNextTask();


                //if (_database.GetDownloadingTask().Length == _downloadingTasks.Count)
                //{


                //}

                //var data = _database.GetDownloadingTask().FirstOrDefault(
                //    item => _downloadingTasks.All(p => item.DownloadPath != p.DownloadPath));

                //if (data == null)
                //{
                //    AddDownloadTask(data.Info);
                //}
            }

         
        }


        private  void GetNextTask()
        {
            var result =  _database.Next();
            if (result == null)
            {
                return;
            }

            if (result.ErrorCode != 0)
            {
                if (result.ErrorCode == 209)
                {

                    //没有任务了
                }
                return;
                // 出现错误

            }

            AddDownloadTask(result.Info);
        }


        public HttpDownload GetDownloadItemById(long id)
        {
            return _downloadingTasks.FirstOrDefault(taskItem => taskItem.DownloadPath == _database.GetFilePathById(id
                ));
        }


        //private DownloadStateChanged downloadingPageStateChanged;
        //private DownloadPercentageChanged downloadingPageProgressChanged;

        //public void SubEvents(DownloadStateChanged state ,
        //    DownloadPercentageChanged progress = null,
        //    long fileId = -1)
        //{

        //    if (fileId == -1)
        //    {
        //        downloadingPageProgressChanged = downloadingPageProgressChanged ?? progress;
        //        downloadingPageStateChanged = downloadingPageStateChanged ?? state;

        //        foreach (var item in _downloadingTasks)
        //        {
        //            if (progress != null)
        //            {                        

        //                item.DownloadProgressChangedEvent -= progress;
        //                item.DownloadProgressChangedEvent += progress; 
        //            }
        //            if (state != null)
        //            {                      
        //                item.DownloadStateChangedEvent -= state;
        //                item.DownloadStateChangedEvent += state; 
        //            }
        //        }
        //    }
        //    else
        //    {

        //        var task = _downloadingTasks.FirstOrDefault(item => item.DownloadPath == _database.GetFilePathById(fileId));


        //        if (task == null) return;

        //        task.DownloadProgressChangedEvent += progress;
        //        task.DownloadStateChangedEvent += state;
        //    }


        //}




        public Action TaskAddedCompleted;
        public Action<HttpDownload> TaskCollectionChanged;
    }


    

    public delegate void DownloadPercentageChanged(object sender, DownloadPercentageChangedEventArgs e);
    public delegate void DownloadStateChanged(object sender, DownloadStateChangedEventArgs e);


}
