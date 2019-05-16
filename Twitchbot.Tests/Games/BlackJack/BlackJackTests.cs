using System;
using Xunit;

using Twitchbot.Games.Helpers;
using System.Collections.Generic;
using Moq;

namespace Twitchbot.Tests.Games.BlackJack
{
    public class BlackJackTests
    {
        [Fact]
        public void BlackJack_Constructor()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.Setup(randomNumber => randomNumber.RandomNumber(1)).Returns(2);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            Assert.Empty(game.GetHand(true).cards);
            Assert.Empty(game.GetHand(false).cards);
        }

        [Fact]
        public void BlackJack_NewGame()
        {
            var random = new Mock<IRandomNumberGenerator>();
            var randomArray = new Stack<int>();
            randomArray.Push(1);
            randomArray.Push(1);
            randomArray.Push(1);
            random.Setup(randomNumber => randomNumber.RandomNumber(1)).Returns(randomArray.Pop());
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
        }
    }
}
