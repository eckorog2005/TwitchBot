using System;

namespace Twitchbot.Bot
{
    public interface IBot{
        void JoinChannel(string userName);
        void LeaveChannel(string userName);
    }
}