using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using Twitchbot.Games.Helpers;
using TwitchLib.Client.Interfaces;

namespace Twitchbot.Games.Dice
{
    public class DiceModule: IBotModule{

        private int sidesOfDice = 20;

        public DiceModule(){
        }
        
        public async Task<bool> ExecuteCommandIfExists(ITwitchClient client, string channel, string userName, string command){
            var handled = false;
            if(command == "!dice"){
                RollDice(client, channel, userName);
                handled = true;
            }
            return handled;
        }

        private void RollDice(ITwitchClient client, string channel, string username){
            var random = new NumberGenerator();
            var diceValue = random.RandomNumber(sidesOfDice) + 1;
            client.SendMessage(channel, $"{username} roll is a {diceValue}");
        }
    }
}