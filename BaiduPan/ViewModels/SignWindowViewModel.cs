using BaiduPan.Model;
using BaiduPan.Views;
using MahApps.Metro.Controls;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BaiduPan.ViewModels
{
    internal class SignWindowViewModel :BindableBase
    {





        //private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get;
            set;
        }

        private User _selectedItemUser;
        public User SelectedItemUser
        {
            get { return _selectedItemUser; }
            set {
                SetProperty(ref _selectedItemUser, value);
                SignIn.RaiseCanExecuteChanged();              
            }
        }
                    


        public SignWindowViewModel()
        {
            Users = new ObservableCollection<User>();

            SignIn = new DelegateCommand<MetroWindow>(
                   (window) => Sign(SelectedItemUser,window),
                   (window) => SelectedItemUser != null);

            foreach (var item in GlobalConfig.Load().Users)
            {
                var user = new User { UserConfig = item };
                user.RemoveEvent += User_RemoveEvent;

                Users.Add(user);
            }
            
            

        }

        private void User_RemoveEvent(User user)
        {
            Users.Remove(user);

            GlobalConfig cnfg = new GlobalConfig();

            cnfg.RemoveUser(user.UserConfig);

        }





        public DelegateCommand<MetroWindow> SignIn {  get;  private set;    }

        private void Sign(User user, MetroWindow window)
        {
            ClientServer client = new ClientServer(user?.UserConfig?.SavedCredential);

            if (user?.UserConfig?.SavedCredential != null)
            {

                var indicator = new IndicatorWindow();
                window.Hide();
                indicator.Show();
                if (client.CheckIsLogined())
                {

                    
                    ClientServer.AuthenticationMounted = user?.UserConfig?.SavedCredential;
                    new MainWindow().Show();
                    indicator.Close();
                    window.Close();
                   
                }
            }
        }

        private DelegateCommand<MetroWindow> _newAccountCmd;

        public DelegateCommand<MetroWindow> NewAccountCmd
        {
            get
            {
                return _newAccountCmd ?? new DelegateCommand<MetroWindow>((window) =>
                {
                    if ( new ClientServer(null).Login())
                    {
                        
                        new MainWindow().Show();
                        window.Close();
                    }
                });
            }

        }



    }

    internal class User :BindableBase
    {

        public string UserName => UserConfig.UserName;

        public string AvatarUrl => new ClientServer(UserConfig.SavedCredential).GetUserInfo()?.records?.FirstOrDefault().AvatarUrl;

        public UserConfig UserConfig { get; set; }

        private ICommand _removeUserCmd;

        public ICommand RemoveUserCmd
        {
            get
            {
                return _removeUserCmd ?? new DelegateCommand( () =>
                {
                    RemoveEvent?.Invoke(this);
                });
            }
            
        }

        public event Action<User> RemoveEvent;



    }


}
