using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces.Files
{
    public interface ISharedFile : IDiskFile
    {
        Uri ShareLink { get; }

        string Password { get; }

        DateTime ShareTime { get; }

        long VisitedNumber { get; }

        long DownloadedNumber { get; }

        long SavedNumber { get; }
    }
}
