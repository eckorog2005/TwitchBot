using System;
using System.Collections.Generic;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;

namespace Twitchbot.Games.Trivia
{
    // Define a class to hold custom event info
    public class TriviaEventArgs : EventArgs
    {
        public TriviaEventArgs(ITwitchClient client, string channel, string message)
        {
            this.client = client;
            this.channel = channel;
            this.message = message;
        }
        private ITwitchClient client;
        private string channel;

        private string message;

        public ITwitchClient Client
        {
            get { return client; }
            set { client = value; }
        }

        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }
}