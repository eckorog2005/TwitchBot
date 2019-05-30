using System;
using Xunit;

using Twitchbot.Games.Helpers;
using System.Collections.Generic;
using Moq;
using Twitchbot.Games.BlackJack;
using TwitchLib.Client.Interfaces;
using System.Threading.Tasks;

namespace Twitchbot.Tests.Games.BlackJack
{
    public class BlackJackModuleTests
    {
        [Fact]
        public void BlackJackModule_Constructor(){
            var module = new BlackJackModule();
            Assert.NotNull(module);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_blackjack_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!blackjack");
            Assert.True(handled);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_jack_not_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!jack");
            Assert.False(handled);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_hit_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!hit");
            Assert.True(handled);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_stay_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!stay");
            Assert.True(handled);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_hit_game_started_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!blackjack");
            Assert.True(handled);
            handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!hit");
            Assert.True(handled);
        }

        [Fact]
        public async Task BlackJackModule_ExecuteCommandIfExists_stay_game_started_handled(){
            var module = new BlackJackModule();
            var twitchclient = new Mock<ITwitchClient>();
            twitchclient.Setup(client => client.SendMessage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()));
            var handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!blackjack");
            Assert.True(handled);
            handled = await module.ExecuteCommandIfExists(twitchclient.Object, "test", "testUser", "!stay");
            Assert.True(handled);
        }
    }
}