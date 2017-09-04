using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Infrastructure.Interfaces
{
    public interface IMountUserRepository : IRepository<string, INetDiskUser>
    {
        
        /// <summary>
        ///  Remove a mount account;
        /// </summary>
        /// <param name="id"></param>
        void Remove(string id);

        /// <summary>
        /// Creates a mount account or return an existing account , and try to sign in
        /// if force is true, try to sign up.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="force">Specifies whether registration is required if the instance does not exist.</param>
        /// <returns></returns>
        //Task<INetDiskUser> CreateAsync(string userName, string password, bool force = false);
    }
}
