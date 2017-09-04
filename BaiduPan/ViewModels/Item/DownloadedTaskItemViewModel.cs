using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduPan.Infrastructure.Interfaces.Files;
using System.Windows.Input;
using System.Diagnostics;
using Prism.Commands;
using System.IO;

namespace BaiduPan.ViewModels.Item
{
    internal class DownloadedTaskItemViewModel : TaskItemViewModel
    {

        private ICommand _openFileCommand;
        private ICommand _clearRecordCommand;


        public DownloadedTaskItemViewModel(ILocalDiskFile diskFile) : base(diskFile)
        {

            CompletedTime = diskFile.CompletedTime.ToString("yyyy-MM-dd HH:mm");       


        }



        public string CompletedTime { get; }

        public ICommand OpenFileCommand
        {
            get
            {
                return _openFileCommand ?? new DelegateCommand(OpenFileCommandExecuteAsync, () => File.Exists(FilePath.FullPath));
            }
           
        }
        public ICommand ClearRecordCommand
        {
            get
            {
                return _clearRecordCommand ?? new DelegateCommand(ClearRecordCommandExecute) ;
            }
            
        }

        private void ClearRecordCommandExecute()
        {
            // TODO
            Debug.WriteLine($"{DateTime.Now} : Clear Record Command: {FilePath.FullPath}");
        }
        private void OpenFileCommandExecuteAsync()
        {
            Task.Run(() =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = FilePath.FullPath });
                }
                catch { }
            });
        }



    }
}
