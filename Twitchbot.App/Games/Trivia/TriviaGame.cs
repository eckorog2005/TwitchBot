using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using Twitchbot.Games.Helpers;
using System.Text;
using System.Linq;

namespace Twitchbot.Games.Trivia
{
    public class TriviaGame{
        private Dictionary<string,int> scores;
        private Dictionary<string,int> currentQuestionResults;

        private bool isStarted;

        private int numOfQuestions;

        private bool isStopped;

        private QuestionMultipleChoice currentQuestion;

        private List<Question> questions;

        public TriviaGame(){
            scores = new Dictionary<string, int>();
            isStarted = false;
            isStopped = true;
            numOfQuestions = 0;
            questions = new List<Question>();
            currentQuestionResults = new Dictionary<string, int>();
        }

        public void StartGame(Question[] questions){
            this.questions = questions.ToList();
            scores.Clear();
            currentQuestionResults.Clear();
            isStarted = true;
            numOfQuestions = questions.Length;
        }

        public string NextQuestion(){
            if(questions.Count == 0){
                return null;
            }
            var next = questions.First();
            questions.Remove(next);
            var question = ConvertQuestion(next);
            currentQuestion = question;

            var questionNumber = numOfQuestions - questions.Count;

            var sb = new StringBuilder();
            sb.Append($"Question {questionNumber}:    "+
                currentQuestion.question+"           "); 
            
            for(var i = 0; i < currentQuestion.answers.Count; i++){
                sb.Append($"{ConvertAnswerToDisplay(Char.ToString((char)(i + 97)))}. ");
                sb.Append(currentQuestion.answers[i] +"      ");
            }

            isStopped = false;

            return sb.ToString();
        }

        public void UserAnswer(string user, string answer){
            if(isStopped){
                return;
            }
            if(currentQuestionResults.ContainsKey(user)){
                // user already answered
            }else{
                var score = 0;
                if(answer == currentQuestion.correctAnswerLetter){
                    score++;
                }
                currentQuestionResults.Add(user, score);
            }
        }

        public bool isGameStarted(){
            return isStarted;
        }

        public bool isFinished(){
            return questions.Count == 0;
        }

        public void StopCurrentQuestion(){
            isStopped = true;
        }

        public List<string> GetQuestionResultAndSave(){
            var results = new List<string>();
            results.Add($"The correct answer was {ConvertAnswerToDisplay(currentQuestion.correctAnswerLetter)}    "+
                "The following users got one point:");
            
            var sb = new StringBuilder();
            foreach(var kvp in currentQuestionResults){
                if(kvp.Value == 1){
                    //save correct score
                    if(scores.ContainsKey(kvp.Key)){
                        scores[kvp.Key]++;
                    }else{
                        scores.Add(kvp.Key, 1);
                    }

                    //add to string
                    sb.Append(kvp.Key + " ");
                }
            }
            results.Add(sb.ToString());
            return results;
        }

        public List<string> GetTotalScore(){
            var results = new List<string>();
            results.Add($"The total score is: ");
            
            var sortedDict = from entry in scores orderby entry.Value ascending select entry;

            var sb = new StringBuilder();
            foreach(var kvp in sortedDict){
                if(kvp.Value == 1){
                    //add to string
                    sb.Append(kvp.Key + " has "+ kvp.Value + " points,  ");
                }
            }
            results.Add(sb.ToString());
            isStarted = false;
            return results;
        }

        private QuestionMultipleChoice ConvertQuestion(Question question){
            var result = new QuestionMultipleChoice();

            var random = new NumberGenerator();
            var value = random.RandomNumber(4);

            result.correctAnswerLetter = Char.ToString((char)(value + 97));
            result.question = question.question;
            var adjustment = 0;
            for(var i = 0; i <= question.incorrect_answers.Length; i++){
                if(i == value){
                    result.answers.Add(question.correct_answer);
                    adjustment = 1;
                }else{
                    result.answers.Add(question.incorrect_answers[i - adjustment]);
                }
            }

            return result;
        }

        private string ConvertAnswerToDisplay(string answer){
            var unicode = "";
            switch(answer){
                case "a":
                    unicode = "\U0001F1E6";
                    break;
                case "b":
                    unicode = "\U0001F1E7";
                    break;
                case "c":
                    unicode = "\U0001F1E8";
                    break;
                case "d":
                    unicode = "\U0001F1E9";
                    break;
            }
            return unicode;
        }
    }
}