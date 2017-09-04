using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces.Files
{
    public interface IDiskFile : IFile
    {
        /// <summary>
        ///  Gets a instance of <see cref="FileLocation"/> which represents the file location information
        /// </summary>
        FileLocation FilePath { get; }

        /// <summary>
        ///  Gets the size of the file based on byte. if the<see cref="FileType"/> is <see cref="FileTypeEnum.FolderType"/>, return 0.
        /// </summary>
        long FileSize { get; }

    }
}
