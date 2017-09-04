using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces
{
    /// <summary>
    /// An aggregate object reprents local disk user
    /// </summary>
    public interface IMountUser
    {
        /// <summary>
        /// Gets User name
        /// </summary>
        string UserName { get; }


        /// <summary>
        ///  Gets encrypted password;
        /// </summary>
        string PasswordEncrypted { get; }

        // whether Saves password or not
        bool IsSavePassword { get; set; }

        /// <summary>
        ///  Whether Sign in Automatically or not
        /// </summary>
        bool IsAutoSignIn { get; set; }

        /// <summary>
        /// Whether connect server or not
        /// </summary>
        bool IsConnectedServer { get; }

        /// <summary>
        /// Gets All Net-Disk Users
        /// </summary>
        /// <returns></returns>
        IEnumerable<INetDiskUser> GetAllNetDiskUsers();

        /// <summary>
        /// Set Account Infomation
        /// </summary>
        /// <param name="userName"> User Name</param>
        /// <param name="password"> Password</param>
        void SetAccountInfo(string userName, string password);

        /// <summary>
        /// Sign In asynchronously
        /// </summary>
        /// <returns></returns>
        Task SignInAsync();

        /// <summary>
        ///  Sign Out
        /// </summary>
        void SignOut();

        /// <summary>
        ///  Gets Current User
        /// </summary>
        /// <returns></returns>
        INetDiskUser GetCurrentNetDiskUser();




        /// <summary>
        ///  TODO: Temporary Solution
        /// </summary>
        /// <param name="fileld"></param>
        void PasueDownloadTask(long fileld);

        void RetartDownloadTask(long fileld);

        void CancleDownloadTask(long fileld);

    }
}
