using System;

namespace twitchbot.Bot
{
    public interface IBot{
        void AddClient(string userName);
        void RemoveClient(string userName);
    }
}