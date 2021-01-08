﻿using System;
using Reloaded.Memory;
using Riders.Tweakbox.Controllers;
using Riders.Tweakbox.Definitions.Interfaces;
using Riders.Tweakbox.Misc;

namespace Riders.Tweakbox.Components.FixesEditor
{
    public class FixesEditorConfig : IConfiguration
    {
        public Internal Data = Internal.GetDefault();

        // Serialization
        public byte[] ToBytes() => Struct.GetBytes(Data);
        public Span<byte> FromBytes(Span<byte> bytes)
        {
            Struct.FromArray(bytes, out Data);
            return bytes.Slice(Struct.GetSize<Internal>());
        }

        // Apply
        public void Apply() { }
        public IConfiguration GetCurrent() => this;
        public IConfiguration GetDefault() => new FixesEditorConfig();

        #region Internal
        public struct Internal
        {
            public bool BootToMenu;
            public bool FramePacing;
            public bool FramePacingSpeedup; // Speed up game to compensate for lag.

            internal static Internal GetDefault() => new Internal
            {
                BootToMenu = true,
                FramePacingSpeedup = true,
                FramePacing = true,
            };
        }
        #endregion
    }
}
