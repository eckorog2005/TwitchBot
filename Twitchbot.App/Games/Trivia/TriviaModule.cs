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

namespace Twitchbot.Games.Trivia
{
    public class TriviaModule: IBotModule{

        private Dictionary<string,TriviaGame> games;

        private Regex rx;

        public TriviaModule(){
            games = new Dictionary<string,TriviaGame>();
            rx = new Regex(@"^!([abcd])$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        
        public async Task<bool> ExecuteCommandIfExists(TwitchClient client, string channel, string userName, string command){
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
            var clientHTTP = new HttpClient();
            var random = new NumberGenerator();
            var value = random.RandomNumber(number);
            QuestionResults questionsResults;
            HttpResponseMessage response = await clientHTTP.GetAsync($"https://opentdb.com/api.php?amount={value}&category=15&type=multiple");
            if (response.IsSuccessStatusCode)
            {
                questionsResults = await response.Content.ReadAsAsync<QuestionResults>();
                questionsResults.results.ToList().ForEach(question => resultQuestions.Add(question));
            }
            value = number - value;
            response = await clientHTTP.GetAsync($"https://opentdb.com/api.php?amount={value}&category=31&type=multiple");
            if (response.IsSuccessStatusCode)
            {
                questionsResults = await response.Content.ReadAsAsync<QuestionResults>();
                questionsResults.results.ToList().ForEach(question => resultQuestions.Add(question));
            }
            return resultQuestions;
        }
    }
}