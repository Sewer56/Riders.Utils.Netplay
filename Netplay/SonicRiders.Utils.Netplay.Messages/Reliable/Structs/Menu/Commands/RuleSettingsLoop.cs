﻿using System;
using Reloaded.Memory;
using Riders.Netplay.Messages.Reliable.Structs.Menu.Shared;
using Sewer56.SonicRiders.Structures.Tasks;
using Sewer56.SonicRiders.Structures.Tasks.Base;
using Sewer56.SonicRiders.Structures.Tasks.Enums.States;

namespace Riders.Netplay.Messages.Reliable.Structs.Menu.Commands
{
    public struct RuleSettingsLoop : IMenuSynchronizationCommand, IEquatable<RuleSettingsLoop>
    {
        public Shared.MenuSynchronizationCommand GetCommandKind() => Shared.MenuSynchronizationCommand.RuleSettingsLoop;
        public byte[] ToBytes() => Struct.GetBytes(this);

        public sbyte DeltaMenuSelectionX;
        public sbyte DeltaMenuSelectionY;
        public sbyte DeltaLapCounter;
        public sbyte DeltaAnnouncer;
        public sbyte DeltaLevel;
        public sbyte DeltaItem;
        public sbyte DeltaPit;
        public sbyte DeltaAir;
        public byte ExitingMenu;

        public bool IsDefault() => this.Equals(new RuleSettingsLoop());

        public RuleSettingsLoop Add(RuleSettingsLoop other)
        {
            return new RuleSettingsLoop()
            {
                DeltaAir = (sbyte) (DeltaAir + other.DeltaAir),
                DeltaPit = (sbyte) (DeltaPit + other.DeltaPit),
                DeltaItem = (sbyte) (DeltaItem + other.DeltaItem),
                DeltaLevel = (sbyte) (DeltaLevel + other.DeltaLevel),
                DeltaAnnouncer = (sbyte) (DeltaAnnouncer + other.DeltaAnnouncer),
                DeltaLapCounter = (sbyte) (DeltaLapCounter + other.DeltaLapCounter),
                DeltaMenuSelectionX = (sbyte) (DeltaMenuSelectionX + other.DeltaMenuSelectionX),
                DeltaMenuSelectionY = (sbyte) (DeltaMenuSelectionY + other.DeltaMenuSelectionY),
                ExitingMenu = (byte)(ExitingMenu + other.ExitingMenu)
            };
        }


        /// <summary>
        /// Undoes the cursor movement delta change made by this loop.
        /// </summary>
        public unsafe void Undo(Task<RaceRules, RaceRulesTaskState>* task)
        {
            if (task == null)
                return;

            var data = task->TaskData;
            data->AirLostAction = (data->AirLostAction - (byte) DeltaAir);
            data->Pit = (byte) (data->Pit - DeltaPit);
            data->Item = (byte) (data->Item - DeltaItem);
            data->Level = (byte)(data->Level - DeltaLevel);
            data->Announcer = (byte)(data->Announcer - DeltaAnnouncer);
            data->TotalLaps = (byte)(data->TotalLaps - DeltaLapCounter);
            data->CurrentHorizontalSelection = (byte)(data->CurrentHorizontalSelection - DeltaMenuSelectionX);
            data->CurrentVerticalSelection = (byte)(data->CurrentVerticalSelection - DeltaMenuSelectionY);
        }

        #region Autogenerated
        public bool Equals(RuleSettingsLoop other)
        {
            return DeltaMenuSelectionX == other.DeltaMenuSelectionX && DeltaMenuSelectionY == other.DeltaMenuSelectionY && DeltaLapCounter == other.DeltaLapCounter && DeltaAnnouncer == other.DeltaAnnouncer && DeltaLevel == other.DeltaLevel && DeltaItem == other.DeltaItem && DeltaPit == other.DeltaPit && DeltaAir == other.DeltaAir && ExitingMenu == other.ExitingMenu;
        }

        public override bool Equals(object obj)
        {
            return obj is RuleSettingsLoop other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DeltaMenuSelectionX.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaMenuSelectionY.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaLapCounter.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaAnnouncer.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaLevel.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaItem.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaPit.GetHashCode();
                hashCode = (hashCode * 397) ^ DeltaAir.GetHashCode();
                hashCode = (hashCode * 397) ^ ExitingMenu.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}