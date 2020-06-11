﻿using System.IO;
using System.Numerics;
using DearImguiSharp;
using Sewer56.Imgui.Misc;
using Sewer56.Imgui.Utilities;

// ReSharper disable once CheckNamespace
namespace Sewer56.Imgui.Shell
{
    public static partial class Shell
    {
        public static unsafe void SetupTheme(string configFolder)
        {
            var io = ImGui.GetIO();
            io.BackendFlags |= (int)ImGuiBackendFlags.ImGuiBackendFlagsHasGamepad;
            io.BackendFlags |= (int)ImGuiBackendFlags.ImGuiBackendFlagsHasSetMousePos;
            io.ConfigFlags |= (int)ImGuiConfigFlags.ImGuiConfigFlagsNavEnableGamepad;
            io.ConfigFlags |= (int)ImGuiConfigFlags.ImGuiConfigFlagsNavEnableKeyboard;
            io.ConfigFlags &= ~(int)ImGuiConfigFlags.ImGuiConfigFlagsNavEnableSetMousePos;
            io.IniFilename = Path.Combine(configFolder, "imgui.ini");

            var fontPath = Path.Combine(configFolder, "Assets/Fonts/Ruda-Bold.ttf");
            var font = ImGui.ImFontAtlasAddFontFromFileTTF(io.Fonts, fontPath, 15.0f, null, ref Constants.NullReference<ushort>());
            if (font != null)
                io.FontDefault = font;

            var style = ImGui.GetStyle();
            style.FrameRounding = 4.0f;
            style.WindowBorderSize = 0.0f;
            style.PopupBorderSize = 0.0f;
            style.GrabRounding = 4.0f;

            var colors = style.Colors;
            colors[(int)ImGuiCol.ImGuiColText] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTextDisabled] = new Vector4(0.73f, 0.75f, 0.74f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColWindowBg] = new Vector4(0.09f, 0.09f, 0.09f, 0.94f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColChildBg] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColPopupBg] = new Vector4(0.08f, 0.08f, 0.08f, 0.94f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColBorder] = new Vector4(0.20f, 0.20f, 0.20f, 0.50f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColBorderShadow] = new Vector4(0.00f, 0.00f, 0.00f, 0.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColFrameBg] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColFrameBgHovered] = new Vector4(0.84f, 0.66f, 0.66f, 0.40f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColFrameBgActive] = new Vector4(0.84f, 0.66f, 0.66f, 0.67f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTitleBg] = new Vector4(0.47f, 0.22f, 0.22f, 0.67f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTitleBgActive] = new Vector4(0.47f, 0.22f, 0.22f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTitleBgCollapsed] = new Vector4(0.47f, 0.22f, 0.22f, 0.67f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColMenuBarBg] = new Vector4(0.34f, 0.16f, 0.16f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColScrollbarBg] = new Vector4(0.02f, 0.02f, 0.02f, 0.53f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColScrollbarGrab] = new Vector4(0.31f, 0.31f, 0.31f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColScrollbarGrabHovered] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColScrollbarGrabActive] = new Vector4(0.51f, 0.51f, 0.51f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColCheckMark] = new Vector4(1.00f, 1.00f, 1.00f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColSliderGrab] = new Vector4(0.71f, 0.39f, 0.39f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColSliderGrabActive] = new Vector4(0.84f, 0.66f, 0.66f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColButton] = new Vector4(0.47f, 0.22f, 0.22f, 0.65f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColButtonHovered] = new Vector4(0.71f, 0.39f, 0.39f, 0.65f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColButtonActive] = new Vector4(0.20f, 0.20f, 0.20f, 0.50f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColHeader] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColHeaderHovered] = new Vector4(0.84f, 0.66f, 0.66f, 0.65f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColHeaderActive] = new Vector4(0.84f, 0.66f, 0.66f, 0.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColSeparator] = new Vector4(0.43f, 0.43f, 0.50f, 0.50f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColSeparatorHovered] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColSeparatorActive] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColResizeGrip] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColResizeGripHovered] = new Vector4(0.84f, 0.66f, 0.66f, 0.66f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColResizeGripActive] = new Vector4(0.84f, 0.66f, 0.66f, 0.66f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTab] = new Vector4(0.71f, 0.39f, 0.39f, 0.54f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTabHovered] = new Vector4(0.84f, 0.66f, 0.66f, 0.66f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTabActive] = new Vector4(0.84f, 0.66f, 0.66f, 0.66f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTabUnfocused] = new Vector4(0.07f, 0.10f, 0.15f, 0.97f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTabUnfocusedActive] = new Vector4(0.14f, 0.26f, 0.42f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColPlotLines] = new Vector4(0.61f, 0.61f, 0.61f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColPlotLinesHovered] = new Vector4(1.00f, 0.43f, 0.35f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColPlotHistogram] = new Vector4(0.90f, 0.70f, 0.00f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColPlotHistogramHovered] = new Vector4(1.00f, 0.60f, 0.00f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColTextSelectedBg] = new Vector4(0.26f, 0.59f, 0.98f, 0.35f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColDragDropTarget] = new Vector4(1.00f, 1.00f, 0.00f, 0.90f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColNavHighlight] = new Vector4(0.41f, 0.41f, 0.41f, 1.00f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColNavWindowingHighlight] = new Vector4(1.00f, 1.00f, 1.00f, 0.70f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColNavWindowingDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.20f).ToImVec();
            colors[(int)ImGuiCol.ImGuiColModalWindowDimBg] = new Vector4(0.80f, 0.80f, 0.80f, 0.35f).ToImVec();
            style.Colors = colors;
        }
    }
}
