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

namespace Twitchbot.Games.Trivia
{
    public class TriviaModule: IBotModule{

        private TriviaGame game;
        private Timer timer;

        private TwitchClient client;

        private string channel;

        public TriviaModule(){
            game = new TriviaGame();
            client = null;
        }
        
        public async Task<bool> ExecuteCommandIfExists(TwitchClient client, string channel, string userName, string command){
            var handled = false;
            if(this.client == null){
                this.client = client;
                this.channel = channel;
            }
            if(command.ToLower().StartsWith("!trivia")){
                if(game.isGameStarted()){
                    //ignore user trying to mess up things
                }
                var seperated = command.Split(" ");
                Question[] questions = null;
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
                    client.SendMessage(channel, "Starting trivia game");
                    game.StartGame(questions);
                    var output = game.NextQuestion();
                    client.SendMessage(channel, HttpUtility.HtmlDecode(output));
                    timer = new Timer(TimerTask, null, 60000, 60000);
                }

                handled = true;
            }
            if(command.ToLower().StartsWith("!a")){
                game.UserAnswer(userName, "a");
                handled = true;
            }
            if(command.ToLower().StartsWith("!b")){
                game.UserAnswer(userName, "b");
                handled = true;
            }
            if(command.ToLower().StartsWith("!c")){
                game.UserAnswer(userName, "c");
                handled = true;
            }
            if(command.ToLower().StartsWith("!d")){
                game.UserAnswer(userName, "d");
                handled = true;
            }
            return handled;
        }

        private void TimerTask(object timerState)
        {
            game.StopCurrentQuestion();
            var output = game.GetQuestionResultAndSave();
            output.ForEach(message => client.SendMessage(channel, message));
            if(!game.isFinished()){
                var next = game.NextQuestion();
                client.SendMessage(channel, HttpUtility.HtmlDecode(next));
            }else{
                var final = game.GetTotalScore();
                final.ForEach(message => client.SendMessage(channel, message));
                timer?.Change(Timeout.Infinite, 0);
                timer.Dispose();
            }
        }

        private async Task<Question[]> GetQuestions(int number){
            var clientHTTP = new HttpClient();
            QuestionResults questionsResults;
            HttpResponseMessage response = await clientHTTP.GetAsync($"https://opentdb.com/api.php?amount={number}&category=15&type=multiple");
            if (response.IsSuccessStatusCode)
            {
                questionsResults = await response.Content.ReadAsAsync<QuestionResults>();
                var questions = questionsResults.results;
                return questions;
            }
            return null;
        }
    }
}