using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Collections.Generic;
using Twitchbot.Games.BlackJack;

namespace Twitchbot.Bot
{

    public class TwitchBotClient
    {
        private TwitchClient client;
        private Dictionary<string,BlackJack> blackJackGames;

        public TwitchBotClient(ConnectionCredentials credentials)
        {
            //make games
            blackJackGames = new Dictionary<string, BlackJack>();

            client = new TwitchClient();
            client.Initialize(credentials);

            client.OnLog += Client_OnLog;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();
        }

        public void Disconnect(){
            client.Disconnect();
        }

        public void JoinChannel(string userName){
            client.JoinChannel(userName);
        }

        public void LeaveChannel(String userName){
            client.LeaveChannel(userName);
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine(e.ChatMessage.Message);
            var userName = e.ChatMessage.Username;
            var message = e.ChatMessage.Message;
            if(message == "!dice"){
                RollDice(e.ChatMessage.Channel,userName);
            }

            //Blackjack game
            if(message == "!blackjack"){
                if(blackJackGames.ContainsKey(userName)){
                    client.SendMessage(e.ChatMessage.Channel,"You already started a game of blackjack");
                }else{
                    var game = new BlackJack();
                    game.NewGame();
                    var playerHand = game.GetHand(true);
                    var dealerHand = game.GetHand(false);
                    blackJackGames.Add(userName, game);
                    client.SendMessage(e.ChatMessage.Channel, $"BlackJack started - {userName} Hand : {playerHand.ToString()}, Dealer's Hand : {dealerHand.ToString()}.  Enter !hit or !stay");
                }
            }
            if(message == "!stay"){
                if(!blackJackGames.ContainsKey(userName)){
                    client.SendMessage(e.ChatMessage.Channel, $"{userName} to start a game of blackjack by using the !blackjack command");
                }else{
                    var game = blackJackGames.GetValueOrDefault(userName);
                    game.DealerTurn();
                    EndBlackJack(game, userName, e.ChatMessage.Channel);
                }
            }
            if(message == "!hit"){
                if(!blackJackGames.ContainsKey(userName)){
                    client.SendMessage(e.ChatMessage.Channel, $"{userName} to start a game of blackjack by using the !blackjack command");
                }else{
                    var game = blackJackGames.GetValueOrDefault(userName);
                    var playerHand = game.PlayerHit();
                    var dealerHand = game.GetHand(false);

                    if(playerHand.GetHandTotal() > 21){
                         EndBlackJack(game, userName, e.ChatMessage.Channel);
                    }else{
                        client.SendMessage(e.ChatMessage.Channel, $"BlackJack new status - {userName} Hand : {playerHand.ToString()}, Dealer's Hand : {dealerHand.ToString()}. Enter !hit or !stay");
                    }
                }
            }
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }

        private void RollDice(string channel, string username){
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            var byteArray = new byte[4];
            provider.GetBytes(byteArray);

            //convert 4 bytes to an integer
            var randomInteger = BitConverter.ToUInt32(byteArray, 0);
            var diceValue = (randomInteger % 6) + 1;
            client.SendMessage(channel, $"{username} roll is a {diceValue}");
        }

        private void EndBlackJack(BlackJack game, string userName, string channel){
            var playerHand = game.GetHand(true);
            var dealerHand = game.GetHand(false);
            var gameMessage = game.ScoreGame() ? $"{userName} Win" : $"{userName} Lose";
            blackJackGames.Remove(userName);
            client.SendMessage(channel, $"{gameMessage} - {userName} Hand : {playerHand.ToString()}, Dealer's Hand : {dealerHand.ToString()}.  Enter !blackjack to play again");

        }
    }
}