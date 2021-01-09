﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using DearImguiSharp;
using Reloaded.Hooks.Definitions;
using Reloaded.Imgui.Hook;
using Riders.Tweakbox.Components.FixesEditor;
using Riders.Tweakbox.Components.GearEditor;
using Riders.Tweakbox.Components.Imgui;
using Riders.Tweakbox.Components.Misc;
using Riders.Tweakbox.Components.Netplay;
using Riders.Tweakbox.Components.PhysicsEditor;
using Riders.Tweakbox.Controllers;
using Riders.Tweakbox.Misc;
using Sewer56.Imgui.Shell;
using Sewer56.Imgui.Shell.Interfaces;
using Sewer56.SonicRiders.Functions;
using IReloadedHooks = Reloaded.Hooks.ReloadedII.Interfaces.IReloadedHooks;

namespace Riders.Tweakbox
{
    public class Tweakbox
    {
        /* Class Declarations */
        private ImguiHook _hook;
        private IReloadedHooks _hooks;
        private IReloadedHooksUtilities _hooksUtilities;
        
        private bool _inputsEnabled = true;
        private bool _isEnabled = true;
        private bool _isReady = false;
        private MenuBar _menuBar;
        private IHook<Functions.GetInputsFn> _blockInputsHook;

        /* Creation & Disposal */
        private Tweakbox(){}

        /// <summary>
        /// Creates a new instance of Riders Tweakbox.
        /// </summary>
        public static async Task<Tweakbox> Create(IReloadedHooks hooks, IReloadedHooksUtilities hooksUtilities,
            string modFolder)
        {
            var tweakBox = new Tweakbox();
            InitializeIoC(modFolder);
            tweakBox._hooks = hooks;
            tweakBox._hooksUtilities = hooksUtilities;
            tweakBox._blockInputsHook = Functions.GetInputs.Hook(tweakBox.BlockGameInputsIfEnabled).Activate();

            tweakBox._menuBar = new MenuBar()
            {
                Menus = new List<MenuBarItem>()
                {
                    new MenuBarItem("Netplay", new List<IComponent>()
                    {
                        IoC.GetConstant<NetplayMenu>()
                    }),
                    new MenuBarItem("Fixes", new List<IComponent>()
                    {
                        IoC.GetConstant<FixesEditor>()
                    }),
                    new MenuBarItem("Editors", new List<IComponent>()
                    {
                        IoC.GetConstant<GearEditor>(),
                        IoC.GetConstant<PhysicsEditor>()
                    }),
                    new MenuBarItem("Debug", new List<IComponent>()
                    {
                        IoC.GetConstant<DemoWindow>(),
                        IoC.GetConstant<UserGuideWindow>(),
                        IoC.GetConstant<ShellTestWindow>(),
                        IoC.GetConstant<TaskTrackerWindow>(),
                        IoC.GetConstant<MemoryDebugWindow>()
                    })
                },
                Text = new List<string>()
                {
                    "F11: Show/Hide",
                    "F10: Enable/Disable Game Input"
                }
            };

            var imguiHook = await ImguiHook.Create(tweakBox.Render);
            Shell.SetupImGuiConfig(modFolder);
            tweakBox._hook = imguiHook;
            tweakBox._isReady = true;
            return tweakBox;
        }

        /// <summary>
        /// Initializes global bindings.
        /// </summary>
        private static void InitializeIoC(string modFolder)
        {
            var io = new IO(modFolder);
            IoC.Kernel.Bind<IO>().ToConstant(io);
            IoC.Kernel.Bind<NetplayConfigFile>().ToConstant(io.GetNetplayConfig());
            IoC.GetConstant<NetplayImguiConfig>();
            IoC.GetConstant<EventController>();
        }

        private int BlockGameInputsIfEnabled()
        {
            // Skips game controller input obtain function is menu is open.
            if (_inputsEnabled)
                return _blockInputsHook.OriginalFunction();

            return 0;
        }

        /* Implementation */
        private void Render()
        { 
            if (!_isReady)
                return;

            // This works because the keys sent to imgui in WndProc follow
            // the Windows key code order.
            if (ImGui.IsKeyPressed((int) Keys.F11, false))
                _isEnabled = !_isEnabled;

            if (ImGui.IsKeyPressed((int)Keys.F10, false))
                _inputsEnabled = !_inputsEnabled;

            if (!_isEnabled) 
                return;

            // Update Menu Bar Text
            if (_inputsEnabled)
                _menuBar.Text[1] = "F10: Disable Game Input";
            else
                _menuBar.Text[1] = "F10: Enable Game Input";

            // Render MenuBar and Menus
            _menuBar.Render();

            // Render Shell
            Shell.Render();
        }

        public void Suspend()
        {
            _hook.Disable();
            _menuBar.Suspend();
        }

        public void Resume()
        {
            _hook.Enable();
            _menuBar.Resume();
        }
    }
}
