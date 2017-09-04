using BaiduPan.Infrastructure.Interfaces;
using BaiduPan.Model;
using BaiduPan.Model.NetDiskInfo;
using BaiduPan.Model.WallPaper;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BaiduPan.ViewModels
{
    internal class UserAvatarViewModel : BindableBase
    {
        public INetDiskUser UserMounted => MountUserRepository.MountedUser;


        private double _used;

        public double Used
        {
            get { return _used; }
            set { SetProperty(ref _used, value); }
        }


        private double _total;

        public double Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }





        private NetDiskQuota DiskQuota { get; set; }

        private string _wallPaper;

        public string WallPaper
        {
            get { return _wallPaper; }
            set { SetProperty(ref _wallPaper, value); }
        }
        




        public DelegateCommand ChangeWallPaperCmd     {  get;private set;  }

        public DelegateCommand SignOutCmd { get; private set; }


        private async void InitialWallPaper()
        {
            var service = new WallPaperService();
            var category = await service.GetWallPaperCategory();
            Random ra = new Random();
            var categorySelected = category[ra.Next(0, category.Count)];
            int num = ra.Next(1, Convert.ToInt16(categorySelected.order_num) - 4);

            List<Wallpaper> wallpaper;

            try
            {
               wallpaper = await service.GetWallPaperByCategory(categorySelected.id, num ,  num+ 3 );
            }
            catch (Exception ex)
            {
                wallpaper = new List<Wallpaper>();
                
            }

            WallPaper = wallpaper.Count > 0 ? wallpaper.FirstOrDefault().img_1024_768 : "/Assets/Images/Bob Brents.jpg";

           

        }



        public UserAvatarViewModel()
        {
            DiskQuota = new ClientServer(ClientServer.AuthenticationMounted).GetQuotaInfo();
            _used = DiskQuota.used / 1024 /1024 / 1024;
            _total = DiskQuota.total / 1024 / 1024 / 1024;

            ChangeWallPaperCmd = new DelegateCommand(() => InitialWallPaper());

            InitialWallPaper();

            SignOutCmd = new DelegateCommand(() =>
            {
                var signout = new ClientServer(ClientServer.AuthenticationMounted);
                if (signout.Logout())
                {
                    Application.Current.Shutdown();
                }
            });
        }






    }
}
