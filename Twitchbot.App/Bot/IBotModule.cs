using System;
using System.Threading.Tasks;
using TwitchLib.Client;

namespace Twitchbot.Bot
{
    public interface IBotModule{
        Task<bool> ExecuteCommandIfExists(TwitchClient client, string channel, string userName, string command);
    }
}