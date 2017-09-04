using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces.Files
{
    public interface IDeletedFile :IDiskFile
    {

        DateTime DeletedTime { get; }


        int LeftDays { get; }
    }
}
