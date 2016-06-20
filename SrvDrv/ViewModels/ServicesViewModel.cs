using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Zodiacon.WPF;

namespace SrvDrv.ViewModels {
    [Export]
    class ServicesViewModel : BindableBase {
        public bool IsServices { get; }

        [Import]
        IMessageBoxService MessageBoxService;

        public ServicesViewModel(bool services) {
            IsServices = services;
        }

        [Import]
        CompositionContainer _container;

        ServiceViewModel[] _services;
        public ServiceViewModel[] Services => _services ?? (_services = GetServices().Select(service => new ServiceViewModel(service).Compose(_container)).ToArray());

        private ServiceController[] GetServices() {
            return IsServices ? ServiceController.GetServices() : ServiceController.GetDevices();
        }
    }
}
