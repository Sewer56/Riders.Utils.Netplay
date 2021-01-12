﻿using DearImguiSharp;
using Sewer56.Imgui.Shell.Interfaces;

namespace Riders.Tweakbox.Components.Debug
{
    public class DemoWindow : IComponent
    {
        public string Name { get; set; } = "Demo Window";
        private bool _isEnabled;

        public ref bool IsEnabled() => ref _isEnabled;
        public void Disable() { }
        public void Enable()  { }
        
        public void Render()
        {
            if (_isEnabled)
                ImGui.ShowDemoWindow(ref _isEnabled);
        }
    }
}