using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using BaiduPan.Infrastructure.Interfaces;
using System.Collections.ObjectModel;
using BaiduPan.Infrastructure.Interfaces.Files;
using BaiduPan.Infrastructure.Events;
using BaiduPan.Infrastructure;
using System.Windows.Input;
using Prism.Commands;
using BaiduPan.Model;

namespace BaiduPan.ViewModels
{
    //internal class NetDiskFileNodeViewModel : ViewModelBase
    //{
    //    public INetDiskFile _netDiskFile;

    //    private ObservableCollection<INetDiskFile> _children;
    //    private bool _isDownloading;
    //    private int downloadPercentage;


    //    public long Fileld => _netDiskFile.FileId;

    //    public FileTypeEnum FileType => _netDiskFile.FileType;

    //    public DataSize? FileSize => _netDiskFile == null ? default(DataSize): new DataSize(_netDiskFile.FileSize);

    //    public FileLocation FilePath => _netDiskFile.FilePath;

    //    public string ModifiedTime => _netDiskFile?.ModifiedTime.ToString("yyyy-MM-dd HH:mm");


    //    public NetDiskFileNodeViewModel() 
    //    {


    //        Children = new ObservableCollection<INetDiskFile>();
         
    //        //_netDiskFile = netDiskFile;

    //        //EventAggregator.GetEvent<DownloadStateChangedEvent>().Subscribe(
    //        //    OnDownloadStateChanged,
    //        //    Prism.Events.ThreadOption.UIThread,
    //        //    keepSubscriberReferenceAlive: false,
    //        //    filter: e => e.Fileld == Fileld);

    //        //EventAggregator.GetEvent<DownloadProgressChangedEvent>().Subscribe(
    //        //    OnDownloadProgressChanged,
    //        //    Prism.Events.ThreadOption.UIThread,
    //        //    keepSubscriberReferenceAlive:false,
    //        //    filter: e => e.Fileld == Fileld);

    //    }

    //    public int DownloadPercentage
    //    {
    //        get { return downloadPercentage; }
    //        set { SetProperty(ref downloadPercentage, value); }
    //    }
    //    public bool IsDownloading
    //    {
    //        get { return _isDownloading; }
    //        set { SetProperty(ref _isDownloading, value); }
    //    }

    //    public ObservableCollection<INetDiskFile> Children
    //    {
    //        get { return _children; }
    //        set { SetProperty(ref _children, value); }
    //    }

    //    //public  NetDiskFileNodeViewModel Parent
    //    //{
    //    //    get { return _parent; }
    //    //    set { SetProperty(ref _parent, value); }
    //    //}


    //    #region  Command


    //    private ICommand _deleteFileCmd;

    //    public ICommand DeleFileCmd
    //    {
    //        get
    //        {
    //            return _deleteFileCmd ?? 
    //                new DelegateCommand(DeleteFileExecuteAsync);
    //        }
           
    //    }


    //    private ICommand _downloadFileCmd;

    //    public ICommand DownloadFileCmd
    //    {
    //        get
    //        {
    //            return _downloadFileCmd ?? 
    //                new DelegateCommand(DownloadFileExecuteAsync);
    //        }
           
    //    }





    //    #endregion  Commad



    //    private async void DownloadFileExecuteAsync()
    //    {
    //        if (FileType != FileTypeEnum.FolderType)
    //            _isDownloading = true;
    //        await _netDiskFile.DownloadAsync();
    //    }


    //    private async void DeleteFileExecuteAsync()
    //    {
    //        //await _netDiskFile.DeleteAsync();
    //        Parent.RefreshChildren();
    //    }


    //    public void  RefreshChildren()
    //    {
           

    //        ClientServer client = new ClientServer(ClientServer.AuthenticationMounted);
    //        foreach (var item in _netDiskFile.GetChildrenAsync())
    //        {
    //            Children.Add(item);
    //        }
            

    //    }


    //    private void OnDownloadStateChanged(DownloadStateChangedEventArgs e)
    //    {
    //        if (e.NewState == DownloadStateEnum.Cancled
    //            || e.NewState == DownloadStateEnum.Completed
    //            || e.NewState == DownloadStateEnum.Faulted)
    //        {
    //            _isDownloading = false;
    //            downloadPercentage = 0;
    //        }
    //    }

    //    private void OnDownloadProgressChanged(DownloadProgressChangedEventArgs e)
    //    {
    //        downloadPercentage = (int)Math.Round((e.CurrentProgress / FileSize.Value) * 100);

    //        if (!_isDownloading) _isDownloading = true;
    //    }

    //}
}
