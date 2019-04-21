using System;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace twitchbot.Games.BlackJack{
    public class BlackJack{
        private List<int> deck;
        private Hand dealerHand;
        private Hand playerHand; 

        public BlackJack(){
            deck = new List<int>();
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
            playerHand.handValue = 0;
            dealerHand.handValue = 0;
            playerHand.hasAce = false;
            dealerHand.hasAce = false;

            NewGame();
        }

        public void DealerTurn(){
            DealCard(false);

            // horrible AI
            if(dealerHand.handValue < 16){
                DealCard(false);
            }
            if(dealerHand.handValue > 21 && dealerHand.hasAce){
                dealerHand.handValue -= 10;
                if(dealerHand.handValue < 16){
                    DealCard(false);
                }
            }
        }

        public bool ScoreGame(){
            if(playerHand.handValue > dealerHand.handValue && playerHand.handValue <= 21){
                return true;
            }else if(dealerHand.handValue >= 21 && playerHand.handValue <= 21){
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

        public bool PlayerStay(){
            DealerTurn();
            return ScoreGame();
        }

        private void DealCard(bool isPlayer){
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);

            //convert 4 bytes to an integer
            var randomInteger = BitConverter.ToUInt16(byteArray, 0);
            var index = randomInteger % deck.Count;
            var card = deck[index];
            deck.Remove(card);
            var isAce = card == 11;
            if(isPlayer){
                playerHand.handValue += card;
                playerHand.hasAce = isAce;
            }else{
                dealerHand.handValue += card;
                playerHand.hasAce = isAce;
            }
        }

        private void CreateDeck(){
            deck.Clear();
            for(var i = 2; i < 12; i++){
                var amount = 4;
                if(i == 10) 
                    amount = 16;
                for(var j = 0; j < amount; j++){
                    deck.Add(i);
                }
            }
        }
    }
}