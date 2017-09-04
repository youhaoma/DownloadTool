using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduPan.Infrastructure.Interfaces.Files;
using System.Windows.Input;
using BaiduPan.Infrastructure;
using BaiduPan.Infrastructure.Interfaces;
using Prism.Commands;
using System.Diagnostics;
using BaiduPan.Model.Download;
using BaiduPan.Infrastructure.Events;
using System.Threading;

namespace BaiduPan.ViewModels.Item
{
    public  class DownloadingTaskItemViewModel : TaskItemViewModel
    {

        private DataSize _downloadSpeed;
        private DataSize _downloadProgerss;
        private DownloadStateEnum _downloadState = DownloadStateEnum.Waiting;

        private ICommand _pauseTaskCommand;
        private ICommand _startTaskCommand;
        private ICommand _cancelTaskCommand;


        public DownloadingTaskItemViewModel(IDiskFile diskFile) : base(diskFile)
        {

            // 方法一：
            //var downloadItem = TaskManager.GetTaskManagerByLocalDiskUser(UserMounted).GetDownloadItemById(FileId);
            //if (downloadItem == null) return;
            //downloadItem.DownloadProgressChangedEvent += OnDownloadProgressChanged;
            //downloadItem.DownloadStateChangedEvent += OnDownloadStateChanged;

            // 方法二

            //TaskManager.GetTaskManagerByLocalDiskUser(UserMounted).SubEvents(OnDownloadStateChanged,
            //     OnDownloadProgressChanged, FileId);
                        

        }

        public void OnDownloadStateChanged(object sender, DownloadStateChangedEventArgs e)
        {
            DownloadSpeed = new DataSize();
            DownloadState = e.NewState;
        }

        public void OnDownloadProgressChanged(object sender, DownloadPercentageChangedEventArgs e)
        {
            if (DownloadState != DownloadStateEnum.Downloading)
            {
                DownloadState = DownloadStateEnum.Downloading;
            }

            DownloadProgress = e.CurrentProgress;
            DownloadSpeed = e.CurrentSpeed;
            RaisePropertyChanged(nameof(RemainingTime));

        }

        public TimeSpan? RemainingTime
        {
            get
            {
                if (DownloadSpeed.BaseBValue == 0)
                    return null;
                return new TimeSpan(0, 0, (int)Math.Round((1.0 * (FileSize?.BaseBValue ?? 0) - DownloadProgress.BaseBValue) / DownloadSpeed.BaseBValue));
            }
        }

        public DataSize DownloadSpeed
        {
            get { return _downloadSpeed; }
            private set { SetProperty(ref _downloadSpeed, value); }
        }
        public DataSize DownloadProgress
        {
            get { return _downloadProgerss; }
            private set { SetProperty(ref _downloadProgerss, value); }
        }
        public DownloadStateEnum DownloadState
        {
            get { return _downloadState; }
            private set { SetProperty(ref _downloadState, value); }
        }


        public ICommand PauseTaskCommand
        {
            get
            {
                return _pauseTaskCommand ?? new DelegateCommand(PauseTaskCommandExecute, () => DownloadState == DownloadStateEnum.Downloading || DownloadState == DownloadStateEnum.Waiting);
            }
            
        }
        public ICommand StartTaskCommand
        {
            get
            {
                return _startTaskCommand ?? new DelegateCommand(StartTaskCommandExecute, () => DownloadState == DownloadStateEnum.Paused);
            }
            
        }
        public ICommand CancelTaskCommand
        {
            get
            {
                return _cancelTaskCommand ?? new DelegateCommand(CancelTaskCommandExecute);
            }
          
        }


        private async void CancelTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Cancel Task: {FilePath.FullPath} Command");

            await TaskManager.GetTaskManagerByLocalDiskUser(UserMounted).RemoveTask(this.FileId);

            RemoveItemEvent?.Invoke(this);
        }
        private void StartTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Start Task: {FilePath.FullPath} Command");

            TaskManager.GetTaskManagerByLocalDiskUser(UserMounted).ContinueTask(this.FileId);
         
        }
  
        private void PauseTaskCommandExecute()
        {
            Debug.WriteLine($"{DateTime.Now}: Pause Task: {FilePath.FullPath} Command");


            TaskManager.GetTaskManagerByLocalDiskUser(UserMounted).PauseTask(this.FileId);
           
        }


        public event Action<DownloadingTaskItemViewModel> RemoveItemEvent;

    }


}