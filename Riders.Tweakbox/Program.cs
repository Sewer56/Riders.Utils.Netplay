﻿using System;
using System.Diagnostics;
using System.Runtime;
using System.Threading;
using Reloaded.Hooks.Definitions;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;
using Riders.Tweakbox.Misc;
using Sewer56.Imgui.Utilities;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace Riders.Tweakbox
{
    public class Program : IMod
    {
        /// <summary>
        /// Used for writing text to the console window.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Provides access to the mod loader API.
        /// </summary>
        private IModLoader _modLoader;

        private Tweakbox _tweakbox;
        
        /// <summary>
        /// Entry point for your mod.
        /// </summary>
        public async void Start(IModLoaderV1 loader)
        {
            #if DEBUG
            Debugger.Launch();
            new GCLog();
            #endif
            _modLoader = (IModLoader)loader;
            _logger = (ILogger)_modLoader.GetLogger();
            _modLoader.GetController<IReloadedHooks>().TryGetTarget(out var hooks);
            _modLoader.GetController<IReloadedHooksUtilities>().TryGetTarget(out var hooksUtilities);
            string modFolder = _modLoader.GetDirectoryForModId("Riders.Tweakbox");

            /* Your mod code starts here. */
            Trace.AutoFlush = true;
            Trace.Listeners.Add(new ConsoleOutListener(_logger));
            Trace.Listeners.Add(new ShellTraceListener());
            Sewer56.SonicRiders.SDK.Init(hooks);
            Reloaded.Imgui.Hook.SDK.Init(hooks);
            _tweakbox = await Tweakbox.Create(hooks, hooksUtilities, modFolder);

            // Garbage collect and set low latency mode.
            GC.Collect();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
            Console.WriteLine("Low Latency GC yay");
        }

        /* Mod loader actions. */
        public void Suspend() => _tweakbox.Suspend();
        public void Resume()  => _tweakbox.Resume();
        public void Unload() { Suspend(); }

        /*  If CanSuspend == false, suspend and resume button are disabled in Launcher and Suspend()/Resume() will never be called.
            If CanUnload == false, unload button is disabled in Launcher and Unload() will never be called.
        */
        public bool CanUnload()  => false;
        public bool CanSuspend() => true;

        /* Automatically called by the mod loader when the mod is about to be unloaded. */
        public Action Disposing { get; }

        /* Contains the Types you would like to share with other mods.
           If you do not want to share any types, please remove this method and the
           IExports interface.
        
           Inter Mod Communication: https://github.com/Reloaded-Project/Reloaded-II/blob/master/Docs/InterModCommunication.md
        */
        public Type[] GetTypes() => new Type[0];

        /* This is a dummy for R2R (ReadyToRun) deployment.
           For more details see: https://github.com/Reloaded-Project/Reloaded-II/blob/master/Docs/ReadyToRun.md
        */
        public static void Main() { }
    }

    /// <summary>
    /// This class is used to get a running log of the number of garbage collections that occur.
    /// </summary>
    public sealed class GCLog
    {
        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="GCLog"/> is reclaimed by garbage collection.
        /// </summary>
        ~GCLog()
        {
            Trace.WriteLine("Garbage Collection!");
            if (!AppDomain.CurrentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted)
                new GCLog();
        }
    }
}
