using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
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
                    _services = ServiceController.GetServices().Concat(ServiceController.GetDevices()).Select(svc => new ServiceViewModel(svc));
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

    }
}
