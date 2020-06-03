﻿using System;
using Reloaded.Memory;
using Reloaded.Memory.Streams;

namespace Riders.Netplay.Messages.Unreliable
{
    /// <summary>
    /// Represents the header for an unreliable packet.
    /// </summary>
    public struct UnreliablePacketHeader : IEquatable<UnreliablePacketHeader>
    {
        private const short NumPlayersMask = 0x0007;

        /*
           // Header (2 bytes)
           Data Bitfields    = 13 bits
           Number of Players = 3 bits 
        */

        /// <summary>
        /// Declares the fields present in the packet to be serialized/deserialized.
        /// </summary>
        public HasData Fields { get; private set; }

        /// <summary>
        /// The number of player entries stored in this unreliable message.
        /// </summary>
        public byte NumberOfPlayers { get; private set; }

        /// <summary>
        /// Creates a packet header given a list of players to include in the packet.
        /// </summary>
        /// <param name="players">List of players to include in the packet.</param>
        public UnreliablePacketHeader(UnreliablePacketPlayer[] players)
        {
            if (players.Length < 1 || players.Length > 8)
                throw new Exception("Number of players must be in the range 1-8.");

            NumberOfPlayers = (byte) players.Length;
            Fields          = GetDataFlagsFromPlayer(players[0]);
        }

        /// <summary>
        /// Retrieves the flags declaring what data is included from a player to be sent.
        /// </summary>
        /// <param name="player">The player to get flags from.</param>
        public static HasData GetDataFlagsFromPlayer(UnreliablePacketPlayer player)
        {
            var data = HasData.Null;

            if (player.Position.HasValue)
                data |= HasData.HasPosition;

            if (player.GetRotationX().HasValue)
                data |= HasData.HasRotation;

            if (player.GetVelocityX().HasValue || player.GetVelocityY().HasValue)
                data |= HasData.HasVelocity;

            if (player.Rings.HasValue)
                data |= HasData.HasRings;

            if (player.State.HasValue)
                data |= HasData.HasState;

            return data;
        }

        /// <summary>
        /// Serializes the current instance of the packet.
        /// </summary>
        public unsafe byte[] Serialize()
        {
            // f: Fields, n: Numbers
            // ffff ffff ffff fnnn
            short fieldsPacked = (short)((short)Fields << 3);
            byte numPlayersPacked = (byte)(NumberOfPlayers - 1);

            short message = (short)((short)fieldsPacked | (short)numPlayersPacked);
            return Struct.GetBytes(message);
        }

        /// <summary>
        /// Serializes an instance of the packet.
        /// </summary>
        public static UnreliablePacketHeader Deserialize(BufferedStreamReader reader)
        {
            // f: Fields, n: Numbers
            // ffff ffff ffff fnnn
            reader.Read(out short message);
            byte numberOfPlayers = (byte)((byte)(message & NumPlayersMask) + 1);
            var fields = message >> 3;

            return new UnreliablePacketHeader
            {
                NumberOfPlayers = numberOfPlayers,
                Fields = (HasData)fields
            };
        }

        /// <summary>
        /// Declares whether the packet has a particular component of data.
        /// </summary>
        [Flags]
        public enum HasData : ushort
        {
            Null            = 0,
            HasPosition     = 1, 
            HasRotation     = 1 << 1, 
            HasVelocity     = 1 << 2, 
            HasRings        = 1 << 3, 
            HasState        = 1 << 4, 
            HasUnused0      = 1 << 5, 
            HasUnused1      = 1 << 6, 
            HasUnused2      = 1 << 7, 
            HasUnused3      = 1 << 8, 
            HasUnused4      = 1 << 9, 
            HasUnused5      = 1 << 10, 
            HasUnused6      = 1 << 11, 
            HasUnused7      = 1 << 12,
            // Last 3 bytes occupied by player count.
        }

        // Autogenerated by R#
        public bool Equals(UnreliablePacketHeader other) => Fields == other.Fields && NumberOfPlayers == other.NumberOfPlayers;
        public override bool Equals(object obj) => obj is UnreliablePacketHeader other && Equals(other);
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)Fields * 397) ^ NumberOfPlayers.GetHashCode();
            }
        }
    }
}