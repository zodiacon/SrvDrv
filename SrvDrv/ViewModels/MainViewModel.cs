using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SrvDrv.ViewModels {
    [Export]
    class MainViewModel : BindableBase, IPartImportsSatisfiedNotification {
        public ServicesViewModel ServicesViewModel { get; } = new ServicesViewModel(true);

        public ServicesViewModel DevicesViewModel { get; } = new ServicesViewModel(false);

        [Import]
        CompositionContainer _container;

        public MainViewModel() {
        }

        public void OnImportsSatisfied() {
            _container.ComposeParts(ServicesViewModel);
            _container.ComposeParts(DevicesViewModel);
        }
    }
}
