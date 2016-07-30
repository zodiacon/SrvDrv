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
    class ServiceViewModel : BindableBase {
        public ServiceController Service { get; }

        public ServiceViewModel(ServiceController service) {
            Service = service;
        }

        public string Name => Service.ServiceName;
        public string DisplayName => Service.DisplayName;
    }
}
