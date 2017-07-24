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
    sealed class ServiceViewModel : BindableBase, IDisposable {
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

        public ServiceControllerStatus Status => Service.Status;

        string _description;
        public string Description => _description ?? (_description = GetDescription());

        [DllImport("advapi32", CharSet = CharSet.Unicode)]
        private static extern int RegLoadMUIString(SafeRegistryHandle hKey, string value, StringBuilder output, int count, out int size, uint flags, string directory);

        public void Refresh() {
            Service.Refresh();
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(Icon));
            RaisePropertyChanged(nameof(DependentServices));
            RaisePropertyChanged(nameof(DependsOn));
        }

        static StringBuilder _descString = new StringBuilder(1024);

        RegistryKey _key;
        private string GetDescription() {
            var key = GetRegistryKey();
            int size = _descString.Capacity;
            int error = RegLoadMUIString(key.Handle, "Description", _descString, size, out size, 0, null);
            var desc = string.Empty;
            if(error == 0)
                desc = _descString.ToString();
            return desc;
        }

        private RegistryKey GetRegistryKey() {
            if(_key == null)
                _key = Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Services\" + Name);
            return _key;
        }

        public string RunAs {
            get {
                var key = GetRegistryKey();
                return key.GetValue("ObjectName", string.Empty).ToString();
            }
        }

        public string ImagePath => GetRegistryKey().GetValue("ImagePath", string.Empty, RegistryValueOptions.None).ToString();

        public bool IsDelayStart {
            get {
				try {
					int delayStart, size;
					Win32.QueryServiceConfig2(Service.ServiceHandle.DangerousGetHandle(), Win32.ServiceInfoLevel.DelayedAutoStart, out delayStart, sizeof(int), out size);
					return delayStart > 0;
				}
				catch {
					return false;
				}
            }
        }

        public string DelayedStart => Service.StartType == ServiceStartMode.Automatic && IsDelayStart ? " (Delay Start)" : string.Empty;

        public string StartType => $"{Service.StartType} ({(int)Service.StartType}) {DelayedStart}";

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

        public void Dispose() {
            Service.Dispose();
            _key.Dispose();
        }

        public bool IsPaused => Status == ServiceControllerStatus.Paused;

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
