using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Infrastructure.Interfaces.Files;
using BaiduPan.Model;
using BaiduPan.Model.NetDiskInfo;
using Microsoft.Practices.Unity;
using Prism.Commands;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace BaiduPan.ViewModels
{
    internal class HomeViewModel : ViewModelBase
    {

        private readonly IMountUserRepository _mountUserRepository;
        private NetDiskUser _netDiskUser = new NetDiskUser();
        //private NetDiskFileNodeViewModel _currentFile;
        private bool _isRefreshing;

        private ClientServer _client;

   

        public INetDiskFile CurrentFile { get; set; }

        private ObservableCollection<INetDiskFile> _childrenFile;

        public ObservableCollection<INetDiskFile> ChildrenFile
        {
            get { return _childrenFile; }
            set { SetProperty(ref _childrenFile, value); }
        }



        public HomeViewModel() 
        {
            
            _client = new ClientServer(ClientServer.AuthenticationMounted);
            ChildrenFile = new ObservableCollection<INetDiskFile>();

            var user = _client.GetUserInfo().records.FirstOrDefault();

            if (_client.Authentication != null )
            {
                CurrentFile = (_netDiskUser.RootFile);

                GetFileList("/");
            }


        }

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set { SetProperty(ref _isRefreshing, value); }
        }
        //internal NetDiskFileNodeViewModel CurrentFile
        //{
        //    get { return _currentFile; }
        //    set { SetProperty(ref _currentFile, value); }
        //}



        #region  Command

        private ICommand _returnFolderCmd;

        private ICommand _enterFolderCmd;

        private ICommand _refreshFileCmd;


        private ICommand _batchDownloadFilesCmd;


        private ICommand _batchDeleteFilesCmd;

        public ICommand BatchDeleteFilesCmd
        {
            get
            {
                return _batchDeleteFilesCmd ??
                    new DelegateCommand<IList>((files) => { });
            }

        }



        public ICommand BatchDownloadFilesCmd
        {
            get
            {
                return _batchDownloadFilesCmd ??
                    new DelegateCommand<IList>( (s) => { } );
            }

        }


        public ICommand RefreshFileCmd
        {
            get
            {
                return _refreshFileCmd ??
                    new DelegateCommand( () =>
                    {
                        RefreshFileCmdExecuteAsync();
                    });
            }

        }


        public ICommand EnterFolderCmd
        {
            get
            {
                return _enterFolderCmd ??
                    new DelegateCommand<INetDiskFile>( async(file) =>
                    {
                        CurrentFile = file;

                        ChildrenFile.Clear();


                         await GetFileList(file.FilePath.FullPath);


                    }, file => file?.FileType == Infrastructure.FileTypeEnum.FolderType);
            }

        }



        public ICommand ReturnFolderCmd
        {
            get
            {
                return _returnFolderCmd ?? new DelegateCommand( async() =>
                {
                    string path = CurrentFile.FilePath.FullPath.Clone().ToString();

                    string temp = CurrentFile.FilePath.FullPath.Remove(CurrentFile.FilePath.FullPath.LastIndexOf('/'));

                    if (string.IsNullOrEmpty(temp))
                    {
                        temp = "/";
                    }

                    await GetFileList(temp);

                    string currentPath = temp.Remove(temp.LastIndexOf('/'));
                    if (string.IsNullOrEmpty(currentPath))
                    {
                        currentPath = "/";
                    }

                    NetDiskFileResult flst = _client.GetFileList(currentPath);

                    CurrentFile = flst.list.Where(item => item.FilePath.FullPath == temp).FirstOrDefault();

                    _isRefreshing = false;
                }, () => CurrentFile != null && CurrentFile.FilePath.FullPath != "/");
            }

        }

        private async Task<NetDiskFileResult> GetFileList(string temp)
        {

            return await Task.Run( () =>
            {
                var flst = _client.GetFileList(temp);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChildrenFile.Clear();

                    if (flst != null && flst.list.Length > 0)
                    {
                        ChildrenFile.AddRange(flst.list);
                    }
                });

                return flst;
            }) ;
        }


        #endregion  Command





        private void  RefreshFileCmdExecuteAsync()
        {
            _isRefreshing = true;

            GetFileList(CurrentFile.FilePath.FullPath);

            _isRefreshing = false;
        }

        //private void BatchDownloadFilesCmdExecute(IList files)
        //{
        //    foreach (var item in GetSelectedItems(files))
        //    {
        //        item.DownloadFileCmd.Execute(null);
        //    }
        //}

        //private void BatchDeleteFilesCmdExecute(IList files)
        //{
        //    foreach (var item in GetSelectedItems(files))
        //    {
        //        item.DeleFileCmd.Execute(null);
        //    }
        //}

        protected override void OnLoaded()
        {
            //if (SetProperty(ref _netDiskUser, _mountUserRepository?.FirstOrDefault()?.GetCurrentNetDiskUser())
            //    && _netDiskUser != null)





        }




    }
}
