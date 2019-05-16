using System;
using Xunit;

using Twitchbot.Games.BlackJack;

namespace Twitchbot.Tests.Games.BlackJack
{
    public class HandTests
    {
        private Card aceOfClubs = new Card(CardValue.Ace, CardSuit.Club);
        private Card fiveOfClubs = new Card(CardValue.Five, CardSuit.Club);
        private Card fiveOfDiamonds = new Card(CardValue.Five, CardSuit.Diamond);
        private Card fiveOfHearts = new Card(CardValue.Five, CardSuit.Heart);
        private Card fiveOfSpades = new Card(CardValue.Five, CardSuit.Spade);
        private Card sevenOfClubs = new Card(CardValue.Seven, CardSuit.Club);
        private Card queenOfClubs = new Card(CardValue.Queen, CardSuit.Club);
        private Card jackOfClubs = new Card(CardValue.Jack, CardSuit.Club);
        private Card kingOfClubs = new Card(CardValue.King, CardSuit.Club);


        [Fact]
        public void Hand_Constructor()
        {
            var hand = new Hand();
            Assert.Empty(hand.cards);
            Assert.Equal(0, hand.GetHandTotal());
            Assert.False(hand.HasAce());
            Assert.Equal("", hand.ToString());
        }
        
        [Fact]
        public void GetHandTotal_NonFaceCards()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(sevenOfClubs);
            Assert.Equal(12, hand.GetHandTotal());
        }

        [Fact]
        public void GetHandTotal_FaceCard()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(queenOfClubs);
            Assert.Equal(15, hand.GetHandTotal());
        }


        [Fact]
        public void GetHandTotal_AceAsEleven()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(aceOfClubs);
            Assert.Equal(16, hand.GetHandTotal());
        }

        [Fact]
        public void GetHandTotal_AceAsOne()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(queenOfClubs);
            hand.cards.Add(aceOfClubs);
            Assert.Equal(16, hand.GetHandTotal());
        }

        [Fact]
        public void GetHandTotal_Add2Aces()
        {
            var hand = new Hand();
            hand.cards.Add(aceOfClubs);
            hand.cards.Add(aceOfClubs);
            Assert.Equal(12, hand.GetHandTotal());
        }

        [Fact]
        public void GetHandTotal_NoCard()
        {
            var hand = new Hand();
            Assert.Equal(0, hand.GetHandTotal());
        }

        [Fact]
        public void HasAce_No()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(sevenOfClubs);
            Assert.False(hand.HasAce());
        }

        [Fact]
        public void HasAce_NoCard()
        {
            var hand = new Hand();
            Assert.False(hand.HasAce());
        }

        [Fact]
        public void HasAce_Yes()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(aceOfClubs);
            Assert.True(hand.HasAce());
        }

        [Fact]
        public void ToString_OneCard()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            Assert.Equal("5\u2663", hand.ToString());
        }

        [Fact]
        public void ToString_OneCardAce()
        {
            var hand = new Hand();
            hand.cards.Add(aceOfClubs);
            Assert.Equal("A\u2663", hand.ToString());
        }

        [Fact]
        public void ToString_NoCard()
        {
            var hand = new Hand();
            Assert.Equal("", hand.ToString());
        }

        [Fact]
        public void ToString_AllSuits()
        {
            var hand = new Hand();
            hand.cards.Add(fiveOfClubs);
            hand.cards.Add(fiveOfDiamonds);
            hand.cards.Add(fiveOfSpades);
            hand.cards.Add(fiveOfHearts);
            Assert.Equal("5\u2663 5\u2662 5\u2660 5\u2661", hand.ToString());
        }

        [Fact]
        public void ToString_AllFace()
        {
            var hand = new Hand();
            hand.cards.Add(jackOfClubs);
            hand.cards.Add(queenOfClubs);
            hand.cards.Add(kingOfClubs);
            Assert.Equal("J\u2663 Q\u2663 K\u2663", hand.ToString());
        }
    }
}
