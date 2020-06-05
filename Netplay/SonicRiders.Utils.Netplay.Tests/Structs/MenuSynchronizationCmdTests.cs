﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Reloaded.Memory.Streams;
using Riders.Netplay.Messages.Reliable.Structs.Menu.Commands;
using Riders.Netplay.Messages.Reliable.Structs.Menu.Shared;
using Sewer56.SonicRiders.Structures.Menus.Enums;
using Xunit;
using MenuSynchronizationCommand = Riders.Netplay.Messages.Reliable.Structs.Menu.MenuSynchronizationCommand;

namespace Riders.Netplay.Messages.Tests.Structs
{
    public class MenuSynchronizationCmdTests
    {
        [Fact]
        public void SerializeStandardMessage()
        {
            var message = new CharaSelectLoop(0, 5, PlayerStatus.GearSelect);
            var bytes   = MenuSynchronizationCommand.ToBytes(message);

            using var memoryStream = new MemoryStream(bytes);
            using var streamReader = new BufferedStreamReader(memoryStream, bytes.Length);
            var command = MenuSynchronizationCommand.FromBytes(streamReader);

            Assert.Equal(message.GetCommandKind(), command.CommandKind);
            Assert.IsType<MenuSynchronizationCommand>(command);
            Assert.IsType<CharaSelectLoop>(command.Command);

            var messageCopy = (CharaSelectLoop) command.Command;
            Assert.Equal(message, messageCopy);
        }

        [Fact]
        public void SerializeMessagePack()
        {
            var message = new CharaSelectSync(new CharaSelectLoop[4]
            {
                new CharaSelectLoop(0, 1, PlayerStatus.Active),
                new CharaSelectLoop(1, 2, PlayerStatus.GearSelect),
                new CharaSelectLoop(2, 3, PlayerStatus.Inactive),
                new CharaSelectLoop(3, 4, PlayerStatus.Ready)
            });

            var bytes = MenuSynchronizationCommand.ToBytes(message);

            using var memoryStream = new MemoryStream(bytes);
            using var streamReader = new BufferedStreamReader(memoryStream, bytes.Length);
            var command = MenuSynchronizationCommand.FromBytes(streamReader);

            Assert.Equal(message.GetCommandKind(), command.CommandKind);
            Assert.IsType<MenuSynchronizationCommand>(command);
            Assert.IsType<CharaSelectSync>(command.Command);

            var messageCopy = (CharaSelectSync)command.Command;
            Assert.Equal(message, messageCopy);
        }
    }
}
