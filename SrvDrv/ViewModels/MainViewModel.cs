using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;
using Prism.Mvvm;
using Zodiacon.WPF;

namespace SrvDrv.ViewModels {
    [Export]
    class MainViewModel : BindableBase {
        IEnumerable<ServiceViewModel> _services;

        public IEnumerable<ServiceViewModel> Services {
            get {
                if(_services == null) {
                    _services = ServiceController.GetServices().Concat(ServiceController.GetDevices()).OrderBy(svc => svc.ServiceName)
                        .Select(svc => new ServiceViewModel(svc));
                }
                return _services;
            }
        }

        [Import]
        IMessageBoxService MessageBoxService;

        public MainViewModel() {
        }

        ICommand _startCommand;
        public ICommand StartCommand => _startCommand ?? (_startCommand = new DelegateCommand<ServiceViewModel>(vm => {
            StartService(vm.Service, true);
        }, svc => svc.Service.Status == ServiceControllerStatus.Stopped));


        private void StartService(ServiceController service, bool start) {
            try {
                service.Start();
            }
            catch(Exception ex) {
                MessageBoxService.ShowMessage($"Error: {ex.Message}", Constants.AppName);
            }
        }

        private bool _showServices = true;

        public bool ShowServices {
            get { return _showServices; }
            set {
                if(SetProperty(ref _showServices, value)) {
                    UpdateFilter();
                }
            }
        }

        private bool _showDrivers = true;

        public bool ShowDrivers {
            get { return _showDrivers; }
            set {
                if(SetProperty(ref _showDrivers, value)) {
                    UpdateFilter();
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText {
            get { return _searchText; }
            set {
                if(SetProperty(ref _searchText, value)) {
                    UpdateFilter();
                }
            }
        }

        void UpdateFilter() {
            var view = CollectionViewSource.GetDefaultView(_services);
            view.Filter = obj => {
                var svc = (ServiceViewModel)obj;
                int type = (int)svc.Service.ServiceType;
                if(!ShowServices && type >= 0x10)
                    return false;

                if(!ShowDrivers && type < 0x10)
                    return false;

                var text = SearchText.ToLower();
                return svc.Name.ToLower().Contains(text) || svc.DisplayName.ToLower().Contains(text);
            };
        }
    }
}
