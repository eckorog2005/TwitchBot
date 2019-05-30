using System;
using System.Threading.Tasks;
using Moq;
using Twitchbot.Games.Dice;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using Xunit;

namespace Twitchbot.Tests.Games.Dice
{
    public class DiceModuleTests
    {
        [Fact]
        public void DiceModule_Constructor(){
            var module = new DiceModule();
            Assert.NotNull(module);
        }

        [Fact]
        public async Task DiceModule_ExecuteCommandIfExists_GoodPath(){
            var module = new DiceModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!dice");
            Assert.True(handled);
        }

        [Fact]
        public async Task DiceModule_ExecuteCommandIfExists_BadPath(){
            var module = new DiceModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!badDice");
            Assert.False(handled);
        }
    }
}