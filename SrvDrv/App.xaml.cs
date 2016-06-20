using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using SrvDrv.ViewModels;
using Zodiacon.WPF;

namespace SrvDrv {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        CompositionContainer _container;
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var catalog = new AggregateCatalog(
                new AssemblyCatalog(Assembly.GetExecutingAssembly()),
                new AssemblyCatalog(typeof(IDialogService).Assembly));

            _container = new CompositionContainer(catalog);
            _container.ComposeExportedValue(_container);
            _container.ComposeExportedValue("AppName", "Services and Drivers");
             
            var vm = _container.GetExportedValue<MainViewModel>();
            var win = new MainWindow { DataContext = vm };
            win.Show();
        }
    }
}
