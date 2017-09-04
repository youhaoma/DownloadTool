using BaiduPan.Infrastructure.Interfaces.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaiduPan.Infrastructure;

namespace BaiduPan.Model.NetDiskInfo
{
    public class LocalDiskFile : ILocalDiskFile
    {
        public DateTime CompletedTime { get; private set; }

        public FileLocation FilePath { get; private set; }

        public long FileSize { get; private set; }

        public long FileId { get; private set; }

        public FileTypeEnum FileType { get; private set; }

        public LocalDiskFile(long id, string path, FileTypeEnum type, long size, DateTime time)
        {

            FileId = id;
            FilePath = new FileLocation(path);
            FileType = type;
            FileSize = size;
            CompletedTime = time;



        }
    }
}
