using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Infrastructure.Interfaces.Files;
using BaiduPan.Model.NetDiskInfo;
using BaiduPan.Model.ResultData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class TaskDatabase
    {
        private static readonly List<TaskDatabase> _list = new List<TaskDatabase>();

        private readonly INetDiskUser _user;
        public string Name => _user.UserName;

        private TaskList _info;


        private string taskListFile => Path.Combine(Directory.GetCurrentDirectory() + @"\Data\", Name, "TaskList.json");

        public TaskDatabase(INetDiskUser user)
        {
            _user = user;
            Reload();
        }

        public static TaskDatabase GetDatabaseByUser(INetDiskUser userMounted)
        {
            if (_list.Any(v => v.Name == userMounted.UserName))
            {
                return _list.FirstOrDefault(v => v.Name == userMounted.UserName);
            }

            var db = new TaskDatabase(userMounted);
            return db;
        }

        private void Reload()
        {
            string temp;
            if (!File.Exists(taskListFile) || string.IsNullOrEmpty(temp = File.ReadAllText(taskListFile)))
            {
                _info = new TaskList();
                Save();
                return;
            }
            else
            {
                _info = JsonConvert.DeserializeObject<TaskList>(temp);
            }

            foreach (var path in _info.Tasks.Select(v => v.DownloadPath))
            {
                if (File.Exists(path + ".downloading"))
                {
                    _info.DownloadingList.Add(DownloadingFileData.Load(path + ".downloading"));
                }
            }

        }

        public void Add(NetDiskFile file, string path)
        {
            if (Contains(path))
            {
                return;
            }


            _info.Tasks.Add(new TaskInfo()
            {
                DownloadFileInfo = file,
                DownloadPath = path,
            });

            var data = new DownloadingFileData()
            {
                Info = null,
                DownloadPath = path,
                FileInfo = file,
            };

            data.Save();
            _info.DownloadingList.Add(data);
            Save();

        }


        public bool Contains(string path)
        {
            return _info.Tasks.Any(v => v.DownloadPath == path);
        }

        public bool Contains(long id)
        {
            return _info.Tasks.Any(v => v.DownloadFileInfo.FileId == id);
        }

        public bool Contains(NetDiskFile file)
        {
            return Contains(file.FileId);
        }

        private void Save()
        {
            lock (this)
            {
                if (!File.Exists(taskListFile) || string.IsNullOrEmpty(File.ReadAllText(taskListFile)))
                {
                    var file = new FileStream(taskListFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);
                    file.Close();
                }
                File.WriteAllText(taskListFile, JObject.Parse(JsonConvert.SerializeObject(_info)).ToString());
            }
        }

        /// <summary>
        ///  Get File Path by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetFilePathById(long id)
        {
            if (!Contains(id)) return string.Empty;



            foreach (var task in _info.Tasks)
            {
                if (task.DownloadFileInfo.FileId == id)
                {
                    return task.DownloadPath;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Get FileId By Path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public long GetFileIdByPath(string path)
        {
            if (!Contains(path))
            {
                return -1L;
            }

            foreach (var task in _info.Tasks)
            {
                if (task.DownloadPath == path)
                {
                    return task.DownloadFileInfo.FileId;
                }
            }

            return -1L;
        }

        public long GetFileIdByDownloadInfo(DownloadInfo info)
        {
            return GetFileIdByPath(info.DownloadPath);
        }

        public NextResult Next()
        {
            var info = _info.DownloadingList.FirstOrDefault(fileData => fileData.Info == null);
            if (info == null)
            {
                Console.WriteLine("DEBUG:没有新任务了");
                return new NextResult(null, 209, "没有新任务了");
            }
            

            var ret =  GetDownloadFileUrls(info.FileInfo.FilePath.FullPath);



            if (ret != null && ret.Urls.Length > 0)
            {
                var resultData = CreateData(info, ret);

                return resultData;
            }
            else
            {
                Console.WriteLine("DEBUG: 获取下载链接失败");
                return new NextResult(null, -1, "获取下载链接失败");
            }







        }

        public NextResult CreateData(DownloadingFileData info, DownloadResult result)
        {
            try
            {
                var httpInfo = HttpDownload.CreateTaskInfo(result.Urls, info.DownloadPath);

                info.Info = httpInfo;
                info.Save();

               
                return new NextResult(httpInfo, 0, string.Empty);

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public TaskInfo[] GetUnCompletedList()
        {
            return _info.Tasks.ToArray();
        }

        public CompletedTask[] GetCompletedList()
        {
            return _info.CompletedTasks.ToArray();
        }



        public DownloadingFileData[] GetDownloadingTask()
        {
            return _info.DownloadingList.Where(item => item?.Info != null).ToArray();
        }



        public DownloadingFileData GetDownloadingDataByPath(string path)
        {
            return GetDownloadingTask().FirstOrDefault(item => item.DownloadPath == path);
        }

        public  DownloadResult GetDownloadFileUrls(string path)
        {
            return new ClientServer(ClientServer.AuthenticationMounted).GetDownload(path);
           
        }

        public void SetCompleted(string path)
        {
            if (!Contains(path))
            {
                return;
            }


            _info.CompletedTasks.Insert(0, new CompletedTask()
            {
                DownloadPath = path,
                Id = GetFileIdByPath(path),
                FileInfo = _info.Tasks.FirstOrDefault(item => item.DownloadPath == path).DownloadFileInfo,
                CompletedTime = DateTime.Now,
            });

            _info.Tasks.Remove(_info.Tasks.FirstOrDefault( item => item.DownloadPath == path));
            GetDownloadingDataByPath(path).DeleteFile();
            _info.DownloadingList.Remove(GetDownloadingDataByPath(path));
            Save();
        }



        public void RemoveTask(long id)
        {
            if (_info.Tasks.Any(item => item.Id == id))
            {
                var path = GetFilePathById(id);
                if (_info.DownloadingList.Any(item => item.DownloadPath == path))
                {
                    _info.DownloadingList.Remove(_info.DownloadingList.FirstOrDefault(item => item.DownloadPath == path));
                }

                if (GetDownloadingDataByPath(path) != null)
                {
                    SetCompleted(path);
                }

                var temp = _info.Tasks.FirstOrDefault(item => item.DownloadPath == path);
                _info.Tasks.Remove(temp);

                while (true)
                {
                    Thread.Sleep(300);
                    try
                    {
                        if (File.Exists(path + ".downloading")) File.Delete(path + ".downloading");

                        File.Delete(path);

                        break;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("删除文件" + path + "事出现错误:" +ex.Message );
                    }
                }
                
            }

            if(_info.CompletedTasks.Any(item => item.Id == id))
            {
                _info.CompletedTasks.Remove(_info.CompletedTasks.FirstOrDefault(item => item.Id == id));
            }


            Save();
        }



        public void UpdateTask(DownloadInfo info)
        {
            if (info == null) return;
            var data = GetDownloadingDataByPath(info.DownloadPath);
            data.Info = info;
            data.Save();
        }






    }




    public class TaskInfo
    {
        public NetDiskFile DownloadFileInfo { get; set; }

        public string DownloadPath { get; set; }

        [JsonIgnore]
        public long Id => DownloadFileInfo.FileId;
    }

    public class CompletedTask
    {
        public string DownloadPath { get; set; }
        public NetDiskFile FileInfo { get; set; }
        public long Id { get; set; }
        public DateTime CompletedTime { get; set; }
    }

    public class DownloadingFileData
    {
        public DownloadInfo Info { get; set; }
        public NetDiskFile FileInfo { get; set; }
        public string DownloadPath { get; set; }

        public void Save()
        {
            File.WriteAllText(DownloadPath + ".downloading", JObject.Parse(JsonConvert.SerializeObject(this)).ToString());
        }

        public void DeleteFile()
        {
            File.Delete(DownloadPath + ".downloading");
        }

        public static DownloadingFileData Load(string path)
        {
            return JsonConvert.DeserializeObject<DownloadingFileData>(File.ReadAllText(path));
        }
    }

    public class TaskList
    {
        public List<TaskInfo> Tasks { get; set; } = new List<TaskInfo>();

        public List<CompletedTask> CompletedTasks { get; set; } = new List<CompletedTask>();

        [JsonIgnore]
        public List<DownloadingFileData> DownloadingList { get; set; } = new List<DownloadingFileData>();
    }


    public class NextResult
    {
        public DownloadInfo Info { get; }
        public int ErrorCode { get; }
        public string ErrorMessage { get; }

        public NextResult(DownloadInfo info, int errorCode, string errorMessage)
        {
            Info = info;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
    }








}
