﻿using System;
using System.Diagnostics;
using System.Runtime;
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
        private DllNotifier _notifier;

        /// <summary>
        /// Entry point for your mod.
        /// </summary>
        public async void Start(IModLoaderV1 loader)
        {
            #if DEBUG
            //Debugger.Launch();
            #endif
            _modLoader = (IModLoader)loader;
            _logger = (ILogger)_modLoader.GetLogger();
            _modLoader.GetController<IReloadedHooks>().TryGetTarget(out var hooks);
            _modLoader.GetController<IReloadedHooksUtilities>().TryGetTarget(out var hooksUtilities);
            string modFolder = _modLoader.GetDirectoryForModId("Riders.Tweakbox");

            /* Your mod code starts here. */
            Log.ConsoleListener = new ConsoleOutListener(_logger);
            Log.HudListener = new ShellTraceListener();
            Sewer56.SonicRiders.SDK.Init(hooks);
            Reloaded.Imgui.Hook.SDK.Init(hooks);
            _notifier = new DllNotifier(hooks);
            _tweakbox = await Tweakbox.Create(hooks, hooksUtilities, modFolder);

            // Tweak Garbage Collection.
            GC.Collect();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
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

        /* This is a dummy for R2R (ReadyToRun) deployment.
           For more details see: https://github.com/Reloaded-Project/Reloaded-II/blob/master/Docs/ReadyToRun.md
        */
        public static void Main() { }
    }
}
