using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
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

        public string Type {
            get {
                switch(Service.ServiceType & (~ServiceType.InteractiveProcess)) {
                    case ServiceType.FileSystemDriver:
                        return "File System Driver";

                    case ServiceType.Win32OwnProcess:
                        return "Service (Own Process)";

                    case ServiceType.Win32ShareProcess:
                        return "Service (Share Process)";

                    case ServiceType.KernelDriver:
                        return "Kernel Driver";

                    case ServiceType.RecognizerDriver:
                        return "Recognizer Driver";
                }
                return Service.ServiceType.ToString();
            }
        }

        public string Status => Service.Status.ToString();

        string _description;
        public string Description => _description ?? (_description = GetDescription());

        [DllImport("advapi32", CharSet = CharSet.Unicode)]
        private static extern int RegLoadMUIString(SafeRegistryHandle hKey, string value, StringBuilder output, int count, out int size, uint flags, string directory);

        static StringBuilder _descString = new StringBuilder(1024);

        private string GetDescription() {
            using(var key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + Name)) {
                int size = _descString.Capacity;
                int error = RegLoadMUIString(key.Handle, "Description", _descString, size, out size, 0, null);
                var desc = string.Empty;
                if(error == 0)
                    desc = _descString.ToString();
                return desc;
            }
        }

        public string StartType => Service.StartType.ToString();

        public string DependsOn => string.Join(", ", Service.ServicesDependedOn.Select(svc => svc.ServiceName));

        public string DependentServices => string.Join(", ", Service.DependentServices.Select(svc => svc.ServiceName));

        public string Icon => GetIcon();

        private string GetIcon() {
            var type = Service.ServiceType & ~ServiceType.InteractiveProcess;

            switch(type) {
                case ServiceType.Win32OwnProcess:
                case ServiceType.Win32ShareProcess:
                    return Service.Status == ServiceControllerStatus.Running ? "/icons/gear_run.ico" : "/icons/gear_stop.ico";

                case ServiceType.FileSystemDriver:
                    return "/icons/data.ico";

            }
            return "/icons/driver.ico";
        }

        public string SupportedOperations {
            get {
                var ops = new List<string>(4);
                if(Service.CanStop)
                    ops.Add("Can Stop");
                if(Service.CanShutdown)
                    ops.Add("Can Shutdown");
                if(Service.CanPauseAndContinue)
                    ops.Add("Can Pause and Continue");
                return string.Join(", ", ops);
            }
        }

    }
}
