using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using Twitchbot.Games.Helpers;
using TwitchLib.Client.Interfaces;

namespace Twitchbot.Games.BlackJack
{
    public class BlackJackModule: IBotModule{

        private Dictionary<string,BlackJack> blackJackGames;

        public BlackJackModule(){
            blackJackGames = new Dictionary<string, BlackJack>();
        }
        
        public async Task<bool> ExecuteCommandIfExists(ITwitchClient client, string channel, string userName, string command){
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var handled = false;
            
            //Blackjack game
            if(command == "!blackjack"){
                if(blackJackGames.ContainsKey(userName)){
                    client.SendMessage(channel,"You already started a game of blackjack");
                }else{
                    var game = new BlackJack(new NumberGenerator());
                    game.NewGame();
                    var playerHand = game.GetHand(true);
                    var dealerHand = game.GetHand(false);
                    blackJackGames.Add(userName, game);
                    client.SendMessage(channel, $"BlackJack started - \n{userName} Hand : {playerHand.ToString()}, \nDealer's Hand : {dealerHand.ToString()}.  \nEnter !hit or !stay");
                }
                handled = true;
            }
            if(command == "!stay"){
                if(!blackJackGames.ContainsKey(userName)){
                    client.SendMessage(channel, $"{userName} to start a game of blackjack by using the !blackjack command");
                }else{
                    var game = blackJackGames.GetValueOrDefault(userName);
                    game.DealerTurn();
                    EndBlackJack(client, game, userName, channel);
                }
                handled = true;
            }
            if(command == "!hit"){
                if(!blackJackGames.ContainsKey(userName)){
                    client.SendMessage(channel, $"{userName} to start a game of blackjack by using the !blackjack command");
                }else{
                    var game = blackJackGames.GetValueOrDefault(userName);
                    var playerHand = game.PlayerHit();
                    var dealerHand = game.GetHand(false);

                    if(playerHand.GetHandTotal() > 21){
                         EndBlackJack(client, game, userName, channel);
                    }else{
                        client.SendMessage(channel, $"BlackJack new status - \n{userName} Hand : {playerHand.ToString()}, \nDealer's Hand : {dealerHand.ToString()}. \nEnter !hit or !stay");
                    }
                }
                handled = true;
            }
            return handled;
        }

        private void EndBlackJack(ITwitchClient client, BlackJack game, string userName, string channel){
            var playerHand = game.GetHand(true);
            var dealerHand = game.GetHand(false);
            var gameMessage = game.ScoreGame() ? $"{userName} Win" : $"{userName} Lose";
            blackJackGames.Remove(userName);
            client.SendMessage(channel, $"{gameMessage} - {userName} Hand : {playerHand.ToString()}, Dealer's Hand : {dealerHand.ToString()}.  Enter !blackjack to play again");

        }
    }
}