using BaiduPan.Infrastructure.Interfaces.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces
{
    /// <summary>
    ///  An aggregate object represents net-disk user
    /// </summary>
    public interface INetDiskUser
    {
        /// <summary>
        ///  Gets the local path of the user's head image file
        /// </summary>
        string AvatarUrl { get; }

        /// <summary>
        ///  User Name
        /// </summary>
        string UserName { get; }

        /// <summary>
        ///  Gets the nick name of the  user
        /// </summary>
        string NickName { get; }

        /// <summary>
        ///  Gets the free size(byte) of net_disk.
        /// </summary>
        long TotalSpace { get; }


        /// <summary>
        ///  Gets Free Space
        /// </summary>
        long FreeSpace { get; }

        /// <summary>
        /// Gets the root file,which represnets root directionary of the net-disk
        /// </summary>
        INetDiskFile RootFile { get; }



        /// <summary>
        /// Gets Uncompleted Download Tasks
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDiskFile> GetUncompletedTask();


        /// <summary>
        /// Gets Completed Download Tasks
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILocalDiskFile> GetTaskCompleted();


        /// <summary>
        ///  Gets Files Shared asynchonoously
        /// </summary>
        /// <param name="files"></param>
        /// <param name="password"> password Share files need</param>
        /// <returns> Uris Shared Files</returns>
        Task<Uri> GetFilesSharedAsync(IEnumerable<INetDiskUser> files, string password = null);



        /// <summary>
        /// Updates the information of the user.
        /// </summary>
        /// <returns></returns>
        Task UpdateAsync();



    }
}
