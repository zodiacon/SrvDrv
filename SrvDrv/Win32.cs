using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SrvDrv {
    static class Win32 {
        public enum ServiceInfoLevel {
            DelayedAutoStart = 3,
            TriggerInfo = 8
        }

        [DllImport("advapi32", CharSet = CharSet.Unicode)]
        public static extern bool QueryServiceConfig2(IntPtr hService, ServiceInfoLevel infoLevel, out int autoStart, int size, out int needed);
    }
}
