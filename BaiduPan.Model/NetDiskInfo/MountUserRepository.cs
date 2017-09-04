using BaiduPan.Infrastructure.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiduPan.Model.NetDiskInfo
{
    public class MountUserRepository : ModelBase, IMountUserRepository
    {
        private List<NetDiskUser> _netDiskUserList;

        public static NetDiskUser MountedUser
        {
            get
            {
                return ClientServer.AuthenticationMounted == null ? null :  new ClientServer(ClientServer.AuthenticationMounted).GetUserInfo().records?.FirstOrDefault() ;
            }
        }


        public MountUserRepository(IUnityContainer container) : base(container)
        {
            _netDiskUserList = new List<NetDiskUser>();
            ReadLocalUserData();

        }



        public INetDiskUser FirstOrDefault()
        {
            return _netDiskUserList.FirstOrDefault();
        }
        public INetDiskUser FindById(string username)
        {
            return _netDiskUserList.Where(item => item.UserName == username).FirstOrDefault();
        }
        public IEnumerable<INetDiskUser> GetAll()
        {
            return _netDiskUserList.AsReadOnly();
        }
        public bool Contains(string id)
        {
            return _netDiskUserList.Any(item => item.UserName == id);
        }
        public void Remove(string id)
        {
            var temp = _netDiskUserList.Where(item => item.UserName == id).FirstOrDefault();
            if (temp != null) _netDiskUserList.Remove(temp);



        }
        //public async Task<INetDiskUser> CreateAsync(string username, string password, bool force = false)
        //{
        //    //if (Contains(username))
        //    //{
        //    //    // 1.在已有列表中查找；
        //    //    var existedObj = FindById(username);
        //    //    //if (!existedObj.IsRememberPassword)
        //    //    existedObj.SetAccountInfo(username, password);
        //    //    if (!existedObj.IsConnectedServer) await existedObj.SignInAsync();
        //    //    return existedObj;
        //    //}
        //    //// 2.如果force = true，则向服务器注册。
        //    //if (force) await RegisterAsync(username, password);
        //    //// 3.添加并连接至服务器；
        //    //Add(username, password);
        //    //var temp = FindById(username);
        //    //await temp.SignInAsync();
            
        //}
        public void Save()
        {

        }


        private void ReadLocalUserData()
        {

            foreach (var item in GlobalConfig.Load().Users)
            {
         
                ClientServer client = new ClientServer(item.SavedCredential);
                var user = client.GetUserInfo();
                _netDiskUserList.AddRange(user.records);
            }

            
        }
        private async Task RegisterAsync(string username, string password)
        {

        }
        private void Add(string username, string password)
        {

        }
    }
}
