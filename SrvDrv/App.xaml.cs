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
        public App() {
            LoadAssemblies();
        }

        readonly Dictionary<string, Assembly> _assemblies = new Dictionary<string, Assembly>();

        private void LoadAssemblies() {
            var appAssembly = typeof(App).Assembly;
            foreach(var resourceName in appAssembly.GetManifestResourceNames()) {
                if(resourceName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)) {
                    using(var stream = appAssembly.GetManifestResourceStream(resourceName)) {
                        var assemblyData = new byte[(int)stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        var assembly = Assembly.Load(assemblyData);
                        _assemblies.Add(assembly.GetName().Name, assembly);
                    }
                }
            }
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }

        Assembly OnAssemblyResolve(object sender, ResolveEventArgs args) {
            var shortName = new AssemblyName(args.Name).Name;
            Assembly assembly;
            if(_assemblies.TryGetValue(shortName, out assembly)) {
                return assembly;
            }
            return null;
        }

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
