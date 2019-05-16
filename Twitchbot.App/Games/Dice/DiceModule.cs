using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using Twitchbot.Games.Helpers;

namespace Twitchbot.Games.BlackJack
{
    public class DiceModule: IBotModule{

        private Dictionary<string,BlackJack> blackJackGames;

        public DiceModule(){
            blackJackGames = new Dictionary<string, BlackJack>();
        }
        
        public async Task<bool> ExecuteCommandIfExists(TwitchClient client, string channel, string userName, string command){
            var handled = false;
            if(command == "!dice"){
                RollDice(client, channel, userName);
                handled = true;
            }
            return handled;
        }

        private void RollDice(TwitchClient client, string channel, string username){
            var random = new NumberGenerator();
            var diceValue = random.RandomNumber(6) + 1;
            client.SendMessage(channel, $"{username} roll is a {diceValue}");
        }
    }
}