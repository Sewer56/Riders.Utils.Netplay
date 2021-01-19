﻿using System;
using System.IO;
using System.Linq;
using EnumsNET;
using MessagePack;
using Reloaded.Memory;
using Reloaded.Memory.Pointers;
using Reloaded.Memory.Streams;
using Riders.Netplay.Messages.Misc;
using Riders.Tweakbox.Components.Tweaks;
using Riders.Tweakbox.Definitions.Interfaces;
using Riders.Tweakbox.Misc;
using Sewer56.SonicRiders.Structures.Gameplay;
using static Riders.Tweakbox.Components.Editors.Physics.PhysicsEditorConfig.Internal;
using Player = Sewer56.SonicRiders.API.Player;

namespace Riders.Tweakbox.Components.Editors.Physics
{
    public unsafe class PhysicsEditorConfig : IConfiguration
    {
        private static FixedArrayPtr<CharacterTypeStats> _typeStats => new FixedArrayPtr<CharacterTypeStats>(0x005BD4D8, 3);
        private static PhysicsEditorConfig _default = PhysicsEditorConfig.FromGame();

        /// <summary>
        /// Internal data of the physics editor.
        /// </summary>
        public Internal Data;

        /// <summary>
        /// Creates a <see cref="PhysicsEditorConfig"/> from the values present in game memory.
        /// </summary>
        public static PhysicsEditorConfig FromGame()
        {
            return new PhysicsEditorConfig
            {
                Data =
                {
                    Contents = PhysicsEditorContents.Running | PhysicsEditorContents.TypeStats,
                    RunningPhysics1 = *Player.RunPhysics,
                    RunningPhysics2 = *Player.RunPhysics2,
                    CharacterTypeStats = _typeStats.ToArray()
                }
            };
        }

        /// <inheritdoc />
        public Action ConfigUpdated { get; set; }

        public unsafe byte[] ToBytes() => Data.ToBytes();

        public unsafe Span<byte> FromBytes(Span<byte> bytes)
        {
            Data.FromBytes(bytes, out int bytesRead);
            ConfigUpdated?.Invoke();
            return bytes.Slice(bytesRead);
        }

        public void Apply()
        {
            *Player.RunPhysics  = Data.RunningPhysics1;
            *Player.RunPhysics2 = Data.RunningPhysics2;
            Player.TypeStats.CopyFrom(Data.CharacterTypeStats, Data.CharacterTypeStats.Length);
        }

        public IConfiguration GetCurrent() => FromGame();
        public IConfiguration GetDefault() => _default;

        /* Internal representation of this config. */
        public struct Internal
        {
            /// <summary>
            /// The data stored inside this struct.
            /// </summary>
            public PhysicsEditorContents Contents;

            public RunningPhysics  RunningPhysics1;
            public RunningPhysics2 RunningPhysics2;
            public CharacterTypeStats[] CharacterTypeStats;

            public byte[] ToBytes()
            {
                using var extendedMemoryStream = new ExtendedMemoryStream();
                extendedMemoryStream.Write(Contents);
                extendedMemoryStream.Write(RunningPhysics1);
                extendedMemoryStream.Write(RunningPhysics2);
                extendedMemoryStream.Write(CharacterTypeStats);
                return extendedMemoryStream.ToArray();
            }

            public void FromBytes(Span<byte> bytes, out int numBytesRead)
            {
                fixed (byte* bytePtr = &bytes[0])
                {
                    using var stream = new UnmanagedMemoryStream(bytePtr, bytes.Length);
                    using var bufferedStreamReader = new BufferedStreamReader(stream, 512);

                    bufferedStreamReader.Read(out Contents);
                    bufferedStreamReader.ReadIfHasFlags(ref RunningPhysics1, Contents, PhysicsEditorContents.Running);
                    bufferedStreamReader.ReadIfHasFlags(ref RunningPhysics2, Contents, PhysicsEditorContents.Running);
                    bufferedStreamReader.ReadIfHasFlags(ref CharacterTypeStats, _typeStats.Count, Contents, PhysicsEditorContents.TypeStats);

                    numBytesRead = (int) bufferedStreamReader.Position();
                }
            }

            public enum PhysicsEditorContents : int
            {
                Running   = 1 << 0,
                TypeStats = 1 << 1,
            }
        }
    }
}
