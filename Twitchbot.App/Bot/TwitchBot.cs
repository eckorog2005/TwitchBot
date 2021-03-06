using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net.Http;

namespace Twitchbot.Bot
{

    public class TwitchBot : IBot
    {
        private TwitchBotClient client;
        private ConnectionCredentials credentials;

        public TwitchBot(IConfiguration iConfig, IHttpClientFactory httpFactory)
        {
            var botName = iConfig.GetValue<string>("username");
            var token = iConfig.GetValue<string>("accessToken");
            credentials = new ConnectionCredentials(botName, token);
            client = new TwitchBotClient(credentials, httpFactory);
        }

        public void JoinChannel(string userName){
            client.JoinChannel(userName);
        }

        public void DisconnectClient(){
            if(client != null){
                client.Disconnect();
            }
        }

        public void LeaveChannel(string userName){
            client.LeaveChannel(userName);
        }
    }
}