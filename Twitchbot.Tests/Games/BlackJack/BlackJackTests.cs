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
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
        }

        [Fact]
        public void BlackJack_ResetGame()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(1)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
            game.ResetGame();
            Assert.Equal("A\u2663 A\u2661", game.GetHand(true).ToString());
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
        }

        [Fact]
        public void BlackJack_DealerTurn()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
            game.DealerTurn();
            Assert.Equal("A\u2660 A\u2662 2\u2661 2\u2663 2\u2660", game.GetHand(false).ToString());
        }

        [Fact]
        public void BlackJack_DealerTurn_player21()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(51)
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("K\u2662 A\u2661", game.GetHand(true).ToString());
            Assert.Equal("A\u2663", game.GetHand(false).ToString());
            game.DealerTurn();
            Assert.Equal("A\u2663 A\u2660", game.GetHand(false).ToString());
        }

        [Fact]
        public void BlackJack_ScoreGame_Tie_DealerWins()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(51)
                .Returns(50)
                .Returns(49)
                .Returns(48);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("K\u2662 K\u2660", game.GetHand(true).ToString());
            Assert.Equal("K\u2663", game.GetHand(false).ToString());
            game.DealerTurn();
            Assert.Equal("K\u2663 K\u2661", game.GetHand(false).ToString());
            Assert.False(game.ScoreGame());
        }

        [Fact]
        public void BlackJack_ScoreGame_DealerWins()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(51)
                .Returns(50)
                .Returns(49)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("K\u2662 K\u2660", game.GetHand(true).ToString());
            Assert.Equal("K\u2663", game.GetHand(false).ToString());
            game.DealerTurn();
            Assert.Equal("K\u2663 A\u2661", game.GetHand(false).ToString());
            Assert.False(game.ScoreGame());
        }

        [Fact]
        public void BlackJack_ScoreGame_PlayerWins()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(51)
                .Returns(0)
                .Returns(48)
                .Returns(47);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("K\u2662 A\u2661", game.GetHand(true).ToString());
            Assert.Equal("K\u2663", game.GetHand(false).ToString());
            game.DealerTurn();
            Assert.Equal("K\u2663 K\u2661", game.GetHand(false).ToString());
            Assert.True(game.ScoreGame());
        }

        [Fact]
        public void BlackJack_PlayerHit()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
            game.PlayerHit();
            Assert.Equal("A\u2661 A\u2663 A\u2662", game.GetHand(true).ToString());
        }

        [Fact]
        public void BlackJack_GetPlayerHand()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2661 A\u2663", game.GetHand(true).ToString());
        }

        [Fact]
        public void BlackJack_GetDealerHand()
        {
            var random = new Mock<IRandomNumberGenerator>();
            random.SetupSequence(randomNumber => randomNumber.RandomNumber(It.IsAny<int>()))
                .Returns(0)
                .Returns(0)
                .Returns(0);
            var game = new Twitchbot.Games.BlackJack.BlackJack(random.Object);
            game.NewGame();
            Assert.Equal("A\u2660", game.GetHand(false).ToString());
        }
    }
}
