using BaiduPan.Infrastructure;
using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Infrastructure.Interfaces.Files;
using BaiduPan.Model.NetDiskInfo;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaiduPan.ViewModels
{
    public  abstract class TaskItemViewModel :ViewModelBase
    {

        public INetDiskUser UserMounted
        {
            get
            {
                return MountUserRepository.MountedUser;
            }
        }

        private readonly IDiskFile _diskFile;
        private ICommand _openFolderCommand;

        public TaskItemViewModel(IDiskFile diskFile)
        {
            _diskFile = diskFile;
             

        }

        public long FileId => _diskFile.FileId;

        public FileTypeEnum FileType => _diskFile.FileType;
        public FileLocation FilePath => _diskFile.FilePath;
        public DataSize? FileSize => new DataSize(_diskFile.FileSize);

        public ICommand OpenFolderCommand
        {
            get
            {
                return _openFolderCommand ?? new DelegateCommand(
                OpenFolderCommandExecuteAsync,
                () => Directory.Exists(FilePath.FolderPath));
            }
            
        }


        private void OpenFolderCommandExecuteAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start("explorer.exe", FilePath.FolderPath);
                }
                catch { }
            });
        }
    }
}
