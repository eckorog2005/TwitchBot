using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace twitchbot.Bot
{

    public class TwitchBot : IBot
    {
        private Dictionary<string, TwitchBotClient> clients;
        private ConnectionCredentials credentials;

        public TwitchBot(IConfiguration iConfig)
        {
            var botName = iConfig.GetValue<string>("username");
            var token = iConfig.GetValue<string>("accessToken");
            credentials = new ConnectionCredentials(botName, token);
            clients = new Dictionary<string, TwitchBotClient>();
        }

        public void AddClient(string userName){
            var client = new TwitchBotClient(userName, credentials);
            clients.Add(userName, client);
        }

        public void DisconnectClient(string userName){
            var client = GetClient(userName);
            if(client != null){
                client.Disconnect();
            }
        }

        public void RemoveClient(string userName){
            DisconnectClient(userName);
            clients.Remove(userName);
        }

        private TwitchBotClient GetClient(string userName){
            TwitchBotClient client;
            clients.TryGetValue(userName, out client);
            return client;
        }
    }
}