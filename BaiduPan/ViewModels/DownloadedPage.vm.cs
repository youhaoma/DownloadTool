using BaiduPan.Infrastructure.Events;
using BaiduPan.Model.Download;
using BaiduPan.Model.NetDiskInfo;
using BaiduPan.ViewModels.Item;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BaiduPan.ViewModels
{
    internal class DownloadedPageViewModel :ViewModelBase
    {



        //private ObservableCollection<DownloadedTaskItemViewModel> DownloadTaskList
        //{ get; set; } = new ObservableCollection<DownloadedTaskItemViewModel>();


        public DownloadedPageViewModel()
        {

        }

        private ObservableCollection<DownloadedTaskItemViewModel> _downloadTaskList =
            new ObservableCollection<DownloadedTaskItemViewModel>();

        public ObservableCollection<DownloadedTaskItemViewModel> DownloadTaskList
        {
            get { return _downloadTaskList; }
            set { SetProperty(ref _downloadTaskList, value); }
        }


        private ICommand _clearAllRecordCommand;

        public ICommand ClearAllRecordCommand
        {
            get
            {
                return _clearAllRecordCommand ?? new DelegateCommand(ClearAllRecordCommandExecute,
                    () => DownloadTaskList?.Any() ?? false);
            }

        }

        private void ClearAllRecordCommandExecute()
        {
            foreach (var item in DownloadTaskList)
            {
                if (item.ClearRecordCommand.CanExecute(null))
                    item.ClearRecordCommand.Execute(null);
            }
        }


        protected override void OnLoaded()
        {

            TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).TaskCollectionChanged += (httpdownload) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {

                });

            };


            foreach (var item in TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).GetCompletedList())
            {
                if (DownloadTaskList.Any(element => element.FileId == item.FileId)) continue;
                DownloadTaskList.Add(new DownloadedTaskItemViewModel(item));
            }
        }


        private void OnDownloadCompleted(object sender, DownloadStateChangedEventArgs e)
        {
            var temp = TaskManager.GetTaskManagerByLocalDiskUser(MountUserRepository.MountedUser).
                GetCompletedList().
                FirstOrDefault(element => element.FileId == e.FileId);
            if (temp == null) return;
            var localFile = new DownloadedTaskItemViewModel(temp);
            DownloadTaskList.Insert(0, localFile);
        }


    }
}
