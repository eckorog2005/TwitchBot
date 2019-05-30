using System;
using Xunit;

using Twitchbot.Games.BlackJack;

namespace Twitchbot.Tests.Games.BlackJack
{
    public class CardTests
    {
        [Fact]
        public void Card_Constructor(){
            var value = CardValue.King;
            var suit = CardSuit.Club;
            var card = new Card(value, suit);
            Assert.Equal(value, card.value);
            Assert.Equal(suit, card.suit);
        }
    }
}