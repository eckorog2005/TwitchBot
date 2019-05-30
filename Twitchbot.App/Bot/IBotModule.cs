using System;
using System.Threading.Tasks;
using TwitchLib.Client.Interfaces;

namespace Twitchbot.Bot
{
    public interface IBotModule{
        Task<bool> ExecuteCommandIfExists(ITwitchClient client, string channel, string userName, string command);
    }
}