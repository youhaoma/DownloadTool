using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces.Files
{
    public interface INetDiskFile :IDiskFile
    {
        /// <summary>
        ///  Gets When the file was created
        /// </summary>
        DateTime CreatedTime { get; }

        /// <summary>
        /// Gets When the file was modyfied;
        /// </summary>
        DateTime ModifiedTime { get; }

        /// <summary>
        /// Get children file of the current file asynchronously.
        /// if current file <see cref="IDiskFile.FileType"/> is <see cref="FileTypeEnum.FolderType"/>
        /// return a instance of <see cref="IEnumerable{INetDiskFile}"/> type represents the children file,
        /// otherwise return null
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<INetDiskFile>> GetChildrenAsync();

        void DownloadAsync();


        bool DeleteAsync();

        Task<bool> RestoreAsync();


    }
}
