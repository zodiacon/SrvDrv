using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
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
    }
}
