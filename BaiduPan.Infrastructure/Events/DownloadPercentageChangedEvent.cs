using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Events
{


    public class DownloadPercentageChangedEventArgs :EventArgsBase
    {

        public long Fileld { get; }

        public DataSize CurrentProgress { get; }


        public DataSize CurrentSpeed { get;  }


        public DownloadPercentageChangedEventArgs(DataSize currentProgress, DataSize currentSpeed, long fileld = -1)
        {
            Fileld = fileld;
            CurrentProgress = currentProgress;
            CurrentSpeed = currentSpeed;

        }
    }
}
