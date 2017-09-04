using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces.Files
{
     public  interface IFile
    {
        /// <summary>
        ///  Gets the Id of File
        /// </summary>
        long FileId { get; }

        /// <summary>
        ///  Gets the type of the file
        /// </summary>
        FileTypeEnum FileType { get; }
    }
}
