﻿using System;
using System.IO;
using Reloaded.Hooks.Definitions;

// ReSharper disable once RedundantUsingDirective
using Microsoft.Windows.Sdk;
using Reloaded.Hooks.Definitions.X86;
using Riders.Tweakbox.Misc;
using Sewer56.Imgui.Utilities;

namespace Riders.Tweakbox
{
    /// <summary>
    /// Notifies when certain DLLs are being loaded.
    /// </summary>
    public class DllNotifier
    {
        const string ErrNoTrampoline = "Hook by JMP instruction insertion. The original called by restoring instructions temporarily, which causes problems with other programs incl. Tweakbox";
        
        /// <summary>
        /// List of modules to warn against.
        /// </summary>
        public readonly NotifyEntry[] WarningList = new NotifyEntry[] 
        {
            new NotifyEntry("RTSSHooks.dll", "RivaTuner Statistics Server", $"Most commonly ships with MSI Afterburner. Does not use trampoline for function hooks. ({ErrNoTrampoline})"),
        };

        private IHook<LdrLoadDll> _ldrLoadDllHook;

        public unsafe DllNotifier(IReloadedHooks hooks)
        {
            // This log call is important, in order to prevent an endless loop in LdrLoadDllImpl
            // We are ensuring that all DLLs used by logging are activated before the hood.
            Log.WriteLine($"[{nameof(DllNotifier)}] Initialising.");

            var ntdll = PInvoke.LoadLibrary("ntdll.dll");
            var ldrLoadDll = hooks.CreateFunction<LdrLoadDll>((long)Native.GetProcAddress(ntdll, nameof(LdrLoadDll)));
            _ldrLoadDllHook = ldrLoadDll.Hook(LdrLoadDllImpl).Activate();
        }

        private unsafe int LdrLoadDllImpl(int searchPath, uint flags, Native.UNICODE_STRING* modulefilename, out IntPtr handle)
        {
            var moduleName     = modulefilename->ToString();
            var moduleFileName = Path.GetFileName(moduleName);
            var index          = WarningList.IndexOf(x => x.DllName.Equals(moduleFileName, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                var warning = WarningList[index];
                Log.WriteLine($"[WARNING!!] Unsafe library is being loaded.");
                Log.WriteLine($"DLL Name: {warning.DllName}");
                Log.WriteLine($"Name: {warning.Name}");
                Log.WriteLine($"Reason: {warning.Reason}");
                Log.WriteLine($"If Tweakbox crashes consider disabling this program or finding a way to delay its injection etc.");
            }

            return _ldrLoadDllHook.OriginalFunction(searchPath, flags, modulefilename, out handle);
        }

        // Definitions
        public struct NotifyEntry
        {
            public string DllName;
            public string Name;
            public string Reason;

            public NotifyEntry(string dllName, string name, string reason)
            {
                DllName = dllName;
                Name = name;
                Reason = reason;
            }
        }

        [Function(CallingConventions.Stdcall)]
        public unsafe delegate int LdrLoadDll(int searchPath, uint flags, Native.UNICODE_STRING* moduleFileName, out IntPtr handle);
    }
}
