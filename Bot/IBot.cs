using System;

namespace twitchbot.Bot
{
    public interface IBot{
        void JoinChannel(string userName);
        void LeaveChannel(string userName);
    }
}