using System;

namespace twitchbot.Games.BlackJack{
    public class Card{
        public CardValue value {get;}
        public CardSuit suit {get;}

        public Card(CardValue value, CardSuit suit){
            this.value = value;
            this.suit = suit;
        }
    }
}