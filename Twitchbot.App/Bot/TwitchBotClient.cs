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
using Twitchbot.Games.Helpers;

namespace Twitchbot.Bot
{

    public class TwitchBotClient
    {
        private TwitchClient client;

        private List<IBotModule> modules;

        public TwitchBotClient(ConnectionCredentials credentials)
        {
            client = new TwitchClient();
            client.Initialize(credentials);

            client.OnLog += Client_OnLog;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnWhisperReceived += Client_OnWhisperReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;

            client.Connect();

            modules = new List<IBotModule>();

            //add modules the crappy way for now
            modules.Add(new BlackJackModule());
            modules.Add(new DiceModule());
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

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            Console.WriteLine(e.ChatMessage.Message);
            var userName = e.ChatMessage.Username;
            var message = e.ChatMessage.Message;

            foreach(var module in modules){
                var handled = await module.ExecuteCommandIfExists(client, e.ChatMessage.Channel, userName, message);
                if(handled){
                    break;
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
    }
}