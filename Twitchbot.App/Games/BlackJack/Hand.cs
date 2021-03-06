using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Twitchbot.Games.BlackJack{
    public class Hand{
        public IList<Card> cards {get;}

        public Hand(){
            cards = new List<Card>();
        }

        public int GetHandTotal(){
            int total = 0;
            int numberOfAcesMin = 0;
            int numberOfAcesMax = 0;
            foreach(var card in cards){
                if(card.value == CardValue.Ace){
                    numberOfAcesMin ++;
                    if(numberOfAcesMax == 0){
                        numberOfAcesMax = 11;
                    }else{
                        numberOfAcesMax++;
                    }
                }else{
                    var value = FindCardValue(card.value);
                    total += value;
                }
            }
            if(total + numberOfAcesMax <= 21){
                total += numberOfAcesMax;
            }else{
                total += numberOfAcesMin;
            }

            return total;
        }

        public bool HasAce(){
            if(cards.FirstOrDefault(card => card.value == CardValue.Ace) != null){
                return true;
            }
            return false;
        }

        public override string ToString(){
            var spade = '\u2660';
            var club = '\u2663';
            var heart = '\u2661';
            var diamond = '\u2662';
            var sb = new StringBuilder();

            foreach(var card in cards){
                string cardValue;
                string cardSuit;
                var cardValueEnum = card.value;
                var cardSuitEnum = card.suit;
                
                if(cardValueEnum == CardValue.Ace){
                    cardValue = "A";
                }else if(((int)cardValueEnum) > 10){
                    if(cardValueEnum == CardValue.Jack){
                        cardValue = "J";
                    }
                    else if(cardValueEnum == CardValue.Queen){
                        cardValue = "Q";
                    }
                    else if(cardValueEnum == CardValue.King){
                        cardValue = "K";
                    }
                    else{
                        cardValue = ((int)cardValueEnum).ToString();
                    }
                }else{
                    cardValue = ((int)cardValueEnum).ToString();
                }

                if(cardSuitEnum == CardSuit.Spade){
                    cardSuit = spade.ToString();
                }else if(cardSuitEnum == CardSuit.Club){
                    cardSuit = club.ToString();
                }else if(cardSuitEnum == CardSuit.Heart){
                    cardSuit = heart.ToString();
                }else{
                    cardSuit = diamond.ToString();
                }
                sb.Append($"{cardValue}{cardSuit} ");
            }
            return sb.ToString().Trim();
        }

        private int FindCardValue(CardValue card){
            var cardValue = 10;
            var numberValue = (int)card;
            if(numberValue < 10){
                cardValue = numberValue;
            }
            return cardValue;
        }
    }
}