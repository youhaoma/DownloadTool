using BaiduPan.Infrastructure;
using BaiduPan.Infrastructure.Events;
using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Model.Download;
using BaiduPan.Model.NetDiskInfo;
using BaiduPan.ViewModels.Item;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BaiduPan.ViewModels
{
    internal class DownloadingPageViewModel :ViewModelBase
    {

        private INetDiskUser _netDiskUser
        {
            get
            {
                return MountUserRepository.MountedUser;
            }
        }


        public DownloadingPageViewModel()
        {


            InitialDownloadList();


            TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).TaskAddedCompleted += () =>
            {
                Application.Current.Dispatcher.Invoke( () =>
                {
                    InitialDownloadList();
                });
            };

            TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).TaskCollectionChanged  += (httpdownload) =>
            {
                Application.Current.Dispatcher.Invoke( () => InitialDownloadEvents(httpdownload));
               
            };



            var https = TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).GetHttpDownloads();
            foreach (var item in https)
            {

                if (item.DownloadState == DownloadStateEnum.Downloading)
                {
                    var downloadingItem = DownloadTaskList.FirstOrDefault(p => p.FilePath.FullPath == item.DownloadPath);
                    if (downloadingItem != null)
                    {
                        downloadingItem.OnDownloadStateChanged(null,
                        new DownloadStateChangedEventArgs(DownloadStateEnum.Waiting, DownloadStateEnum.Downloading));

                        item.DownloadStateChangedEvent += downloadingItem.OnDownloadStateChanged;
                        item.DownloadProgressChangedEvent += downloadingItem.OnDownloadProgressChanged;
                    }


                }

                item.DownloadStateChangedEvent += OnDownloadStateChanged;
                item.DownloadProgressChangedEvent += OnDownloadProgressChanged;
            }



        }


        private ObservableCollection<DownloadingTaskItemViewModel> _downloadTaskList = new ObservableCollection<DownloadingTaskItemViewModel>();

        public ObservableCollection<DownloadingTaskItemViewModel> DownloadTaskList
        {
            get { return _downloadTaskList; }
            set { SetProperty(ref _downloadTaskList, value); }
        }


        public DataSize TotalDownloadProgress => DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.DownloadProgress);


        public DataSize TotalDownloadSpeed => DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.DownloadSpeed);


        public DataSize TotalDownloadQuantity => !DownloadTaskList.Any() ? new DataSize(1) : DownloadTaskList.Aggregate(new DataSize(), (current, item) => current + item.FileSize ?? default(DataSize));
        public bool IsStartAll => DownloadTaskList == null ? false: DownloadTaskList.All(temp => temp.StartTaskCommand.CanExecute(null));






        #region Commands and their logic
        private ICommand _pauseAllCommand;
        private ICommand _startAllCommand;
        private ICommand _cancelAllCommand;

        public ICommand PauseAllCommand
        {
            get
            {
                return _pauseAllCommand ?? new DelegateCommand(PauseAllCommandExecute, 
                    () => DownloadTaskList?.Any() ?? false);
            }
            
        }
        public ICommand StartAllCommand
        {
            get
            {
                return _startAllCommand ?? new DelegateCommand(StartAllCommandExecute, 
                    () => DownloadTaskList?.Any() ?? false);
            }
            
        }
        public ICommand CancelAllCommand
        {
            get
            {
                return _cancelAllCommand ?? new DelegateCommand(CancelAllCommandExecute, () => DownloadTaskList?.Any() ?? false);
            }
          
        }

        private void CancelAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Cancel All Command");
            foreach (var item in DownloadTaskList)
            {
                if (item.CancelTaskCommand.CanExecute(null))
                {
                    item.CancelTaskCommand.Execute(null);
                }
            }
        }
        private void StartAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Start All Command, IsStartAll = {IsStartAll}");
            foreach (var item in DownloadTaskList)
            {
                if (item.StartTaskCommand.CanExecute(null))
                {
                    item.StartTaskCommand.Execute(null);
                }
            }
            RaisePropertyChanged(nameof(IsStartAll));
        }
        private void PauseAllCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Pause All Command, IsStartAll = {IsStartAll}");
            foreach (var item in DownloadTaskList)
            {
                if (item.PauseTaskCommand.CanExecute(null))
                {
                    item.PauseTaskCommand.Execute(null);
                }
            }
            RaisePropertyChanged(nameof(IsStartAll));
        }
        #endregion




        protected override void OnLoaded()
        {

            if (_netDiskUser == null)
            {
                DownloadTaskList.Clear();
                return;
            }

            InitialDownloadList();
        }


        private void InitialDownloadList()
        {
            foreach (var item in TaskManager.GetTaskManagerByLocalDiskUser(_netDiskUser).GetUnCompletedList())
            {
                if (DownloadTaskList.Any(element => element.FileId == item.FileId)) continue;

                var viewmodel = new DownloadingTaskItemViewModel(item);
                viewmodel.RemoveItemEvent += (task) =>
                {
                    DownloadTaskList.Remove(task);
                };
                DownloadTaskList.Add(viewmodel);
            }
        }

        private void InitialDownloadEvents(HttpDownload downloadInfo)
        {

            var taskInfo = DownloadTaskList.FirstOrDefault(item => item.FilePath.FullPath == downloadInfo.DownloadPath);

            if (taskInfo != null)
            {
                downloadInfo.DownloadProgressChangedEvent += taskInfo.OnDownloadProgressChanged;
                downloadInfo.DownloadStateChangedEvent += taskInfo.OnDownloadStateChanged;
            }

            downloadInfo.DownloadProgressChangedEvent += OnDownloadProgressChanged;
            downloadInfo.DownloadStateChangedEvent += OnDownloadStateChanged;

        }




        private void OnDownloadProgressChanged(object sender, DownloadPercentageChangedEventArgs e)
        {      
            RaisePropertyChanged(nameof(TotalDownloadProgress));
            RaisePropertyChanged(nameof(TotalDownloadQuantity));
            RaisePropertyChanged(nameof(TotalDownloadSpeed));
        }
        private void OnDownloadStateChanged(object sender, DownloadStateChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsStartAll));
            if (e.NewState == DownloadStateEnum.Completed || e.NewState == DownloadStateEnum.Canceld)
                OnDownloadCompletedOrCanceled(e);

            if (e.NewState == DownloadStateEnum.Downloading)
            {
                var temp = DownloadTaskList.FirstOrDefault(item => item.FileId == e.FileId);
                var indexThis = DownloadTaskList.IndexOf(temp);
                if (indexThis != 0)
                {

                    Application.Current.Dispatcher.Invoke( () =>
                    {
                        DownloadTaskList.Insert(0, temp);
                        DownloadTaskList.Remove(temp);
                    });
                }
            }
            Debug.WriteLine($"{DateTime.Now}: FileId={e.FileId}, OldState={e.OldState}, NewState={e.NewState}, IsStartAll={IsStartAll}");
        }
        private void OnDownloadCompletedOrCanceled(DownloadStateChangedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke( () =>
            {
                DownloadTaskList.Remove(DownloadTaskList.FirstOrDefault(item => item.FileId == e.FileId));
            });
            RaisePropertyChanged(nameof(TotalDownloadQuantity));
            OnDownloadProgressChanged(null, null);
        }




  


    }
}
