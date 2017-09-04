using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace BaiduPan.ViewModels
{
    public abstract class ViewModelBase :BindableBase
    {

        private ICommand _loadedCommand;
        private ICommand _loadedCommandWithParam;


        public ICommand LoadedCommand
        {
            get { return _loadedCommand; }
            set { SetProperty(ref _loadedCommand, value); }
        }


        public ICommand LoadedCommandWithParam
        {
            get { return _loadedCommandWithParam; }
            set { SetProperty(ref _loadedCommandWithParam, value); }
        }



        protected IUnityContainer Container { get; }
        protected IEventAggregator EventAggregator { get; }
        protected ILoggerFacade Logger { get; }

        protected ViewModelBase()
        {
            //Container = container;
            //EventAggregator = container.Resolve<IEventAggregator>();
            //Logger = container.Resolve<ILoggerFacade>();
            LoadedCommand = new DelegateCommand(OnLoaded);
            LoadedCommandWithParam = new DelegateCommand<ContentControl>(OnLoaded);
        }

        protected virtual void OnLoaded() { }
        protected virtual void OnLoaded(ContentControl param) { }


    }
}
