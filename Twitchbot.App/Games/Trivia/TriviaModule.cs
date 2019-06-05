using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Twitchbot.Games.Helpers;
using System.Web;
using System.Linq;
using System.Text.RegularExpressions;
using TwitchLib.Client.Interfaces;

namespace Twitchbot.Games.Trivia
{
    public class TriviaModule: IBotModule{

        private Dictionary<string,TriviaGame> games;
        private TriviaService service;
        private Regex rx;

        public TriviaModule(IHttpClientFactory httpClientFactory){
            games = new Dictionary<string,TriviaGame>();
            rx = new Regex(@"^!([abcd])$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            service = new TriviaService(httpClientFactory);
        }
        
        public async Task<bool> ExecuteCommandIfExists(ITwitchClient client, string channel, string userName, string command){
            var handled = false;

            //find the channel's trivia game
            TriviaGame channelGame = null;
            if(games.ContainsKey(channel)){
                channelGame = games[channel];
            }

            if(command.ToLower().StartsWith("!trivia")){
                if(channelGame != null && channelGame.isGameStarted()){
                    //ignore user trying to mess up things
                }
                var seperated = command.Split(" ");
                List<Question> questions = null;
                if(seperated.Length == 2){
                    var number = 0;
                    try{
                        number = Int32.Parse(seperated[1]);
                    }catch (Exception ex) when 
                    (ex is FormatException || 
                    ex is OverflowException){
                        client.SendMessage(channel, $"{userName} Trivia start format is: !trivia [1-5]");
                    }
                    if(number > 0 && number <= 5){
                        questions = await GetQuestions(number);
                    }
                }else{
                    client.SendMessage(channel, $"{userName} Trivia start format is: !trivia [1-5]");
                }

                if(questions != null){
                    client.SendMessage(channel, "Starting trivia game. Please answer with !a !b !c or !d");
                    if(channelGame == null){
                        channelGame = new TriviaGame(client, channel);
                        channelGame.RaiseCustomEvent += HandleTriviaEvent;
                        games.Add(channel, channelGame);
                    }
                    channelGame.StartGame(questions);
                    
                }

                handled = true;
            }
            if(rx.IsMatch(command) && channelGame != null){
                var match = rx.Match(command);
                channelGame.UserAnswer(userName, match.Groups[1].Value.ToLower());
                handled = true;
            }

            return handled;
        }

        private void HandleTriviaEvent(object sender, TriviaEventArgs e)
        {
            e.Client.SendMessage(e.Channel, e.Message);
        }

        private async Task<List<Question>> GetQuestions(int number){
            var resultQuestions = new List<Question>();
            var random = new NumberGenerator();
            var value = random.RandomNumber(number);
            
            resultQuestions.AddRange(await service.GetTriviaQuestions(value, TriviaCategoryEnum.VIDEO_GAMES));

            value = number - value;

            resultQuestions.AddRange(await service.GetTriviaQuestions(value, TriviaCategoryEnum.ANIME_MANGA));
            
            return resultQuestions;
        }
    }
}