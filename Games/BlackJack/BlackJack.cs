using System;
using System.Security.Cryptography;
using System.Collections.Generic;

using twitchbot.Games.Helpers;

namespace twitchbot.Games.BlackJack{
    public class BlackJack{
        private Hand deck;
        private Hand dealerHand;
        private Hand playerHand; 

        public BlackJack(){
            deck = new Hand();
            dealerHand = new Hand();
            playerHand = new Hand();
        }

        public void NewGame(){
            //create deck
            CreateDeck();

            //deal cards
            for(var i = 0; i < 2; i++){
                DealCard(true);
            }
            DealCard(false);
        }

        public void resetGame(){
            dealerHand = new Hand();
            playerHand = new Hand();

            NewGame();
        }

        public void DealerTurn(){
            //simulate only seeing one dealer card. add card for the second.
            DealCard(false);

            /** 
                Dealer Rules :
                    hit if value is less than 17 or soft 17,
                    else stay on hard 17 or higher
            */ 
            while(playerHand.GetHandTotal() < 21 && 
                    (dealerHand.GetHandTotal() < 17 || 
                    (dealerHand.GetHandTotal() == 17 && dealerHand.hasAce()))){
                DealCard(false);
            }
        }

        public bool ScoreGame(){
            var playerScore = playerHand.GetHandTotal();
            var dealerScore = dealerHand.GetHandTotal();
            if(playerScore == dealerScore && playerScore <= 21){
                return false;
            }
            if((playerScore > dealerScore && playerScore <= 21) || 
                    (dealerScore > 21 && playerScore <= 21)){
                return true;
            }else{
                return false;
            }
        }

        public Hand PlayerHit(){
            DealCard(true);
            return playerHand;
        }

        public Hand GetHand(bool isPlayer){
            if(isPlayer){
                return playerHand;
            }else{
                return dealerHand;
            }
        }

        public bool PlayerStayed(){
            DealerTurn();
            return ScoreGame();
        }

        private bool DealCard(bool isPlayer){
            var index = RandomNumber.RandomInteger(deck.cards.Count);
            var card = deck.cards[index];
            deck.cards.Remove(card);
            if(isPlayer){
                playerHand.cards.Add(card);
                if(playerHand.GetHandTotal() > 21){
                    DealerTurn();
                    return true;
                }
            }else{
                dealerHand.cards.Add(card);
            }

            return false;
        }

        private void CreateDeck(){
            deck.cards.Clear();
            var cardSuits = Enum.GetValues(typeof(CardSuit));
            var cardValues = Enum.GetValues(typeof(CardValue));
            foreach(var value in cardValues){
                foreach(var suit in cardSuits){
                    deck.cards.Add(new Card((CardValue)value, (CardSuit)suit));
                }
            }
        }
    }
}