using System;
using System.Collections.Generic;
using TwitchLib.Client;

namespace Twitchbot.Games.Trivia
{
    // Define a class to hold custom event info
    public class TriviaEventArgs : EventArgs
    {
        public TriviaEventArgs(TwitchClient client, string channel, string message)
        {
            this.client = client;
            this.channel = channel;
            this.message = message;
        }
        private TwitchClient client;
        private string channel;

        private string message;

        public TwitchClient Client
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