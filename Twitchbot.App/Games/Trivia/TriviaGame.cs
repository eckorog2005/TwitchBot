using System;
using Twitchbot.Bot;
using TwitchLib.Client;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Twitchbot.Games.Helpers;
using System.Web;
using System.Text;
using System.Linq;

namespace Twitchbot.Games.Trivia
{
    public class TriviaGame
    {
        private Dictionary<string, int> scores;
        private Dictionary<string, int> currentQuestionResults;

        private const int time = 30000;

        private Timer timer;

        private TwitchClient client;

        private string channel;

        private bool isStarted;

        private int numOfQuestions;

        private bool isStopped;

        private QuestionMultipleChoice currentQuestion;

        private List<Question> questions;

        public event EventHandler<TriviaEventArgs> RaiseCustomEvent;

        public TriviaGame(TwitchClient client, string channel)
        {
            scores = new Dictionary<string, int>();
            isStarted = false;
            isStopped = true;
            numOfQuestions = 0;
            questions = new List<Question>();
            currentQuestionResults = new Dictionary<string, int>();
            this.client = client;
            this.channel = channel;
        }

        public void StartGame(List<Question> questions)
        {
            this.questions = questions;
            scores.Clear();
            currentQuestionResults.Clear();
            isStarted = true;
            numOfQuestions = questions.Count;
            var output = NextQuestion();
            OnRaiseTriviaEvent(new TriviaEventArgs(client, channel, HttpUtility.HtmlDecode(output)));
            timer = new Timer(TimerTask, null, time, time);
        }

        public string NextQuestion()
        {
            if (questions.Count == 0)
            {
                return null;
            }
            var next = questions.First();
            questions.Remove(next);
            var question = ConvertQuestion(next);
            currentQuestion = question;

            var questionNumber = numOfQuestions - questions.Count;

            var sb = new StringBuilder();
            sb.Append($"Question {questionNumber}:    " +
                currentQuestion.question + "           ");

            for (var i = 0; i < currentQuestion.answers.Count; i++)
            {
                sb.Append($"{ConvertAnswerToDisplay(Char.ToString((char)(i + 97)))} ");
                sb.Append(currentQuestion.answers[i] + "      ");
            }

            isStopped = false;

            return sb.ToString();
        }

        public void UserAnswer(string user, string answer)
        {
            if (isStopped)
            {
                return;
            }
            if (currentQuestionResults.ContainsKey(user))
            {
                // user already answered
            }
            else
            {
                var score = 0;
                if (answer == currentQuestion.correctAnswerLetter)
                {
                    score++;
                }
                currentQuestionResults.Add(user, score);
            }
        }

        public bool isGameStarted()
        {
            return isStarted;
        }

        public bool isFinished()
        {
            return questions.Count == 0;
        }

        public void StopCurrentQuestion()
        {
            isStopped = true;
        }

        public List<string> GetQuestionResultAndSave()
        {
            var results = new List<string>();
            var correctCount = 0;

            foreach (var kvp in currentQuestionResults)
            {
                var points = kvp.Value;
                //save correct score
                if (scores.ContainsKey(kvp.Key))
                {
                    scores[kvp.Key] += points;
                }
                else
                {
                    scores.Add(kvp.Key, points);
                }

                if (points == 1)
                {
                    correctCount++;
                }
            }

            results.Add($"The correct answer was {ConvertAnswerToDisplay(currentQuestion.correctAnswerLetter)} | " +
                $"{correctCount} user(s) got the question correct.");

            currentQuestionResults.Clear();
            return results;
        }

        public List<string> GetTotalScore()
        {
            var results = new List<string>();
            results.Add($"Top 10 final socres: ");

            var sortedDict = (from entry in scores orderby entry.Value descending select entry).Take(10);

            var sb = new StringBuilder();
            foreach (var kvp in sortedDict)
            {
                var pointString = "point";
                if(kvp.Value > 1 || kvp.Value == 0){
                    pointString = pointString + "s";
                }
                //add to string
                sb.Append($"{kvp.Key} has {kvp.Value} {pointString},  ");
            }
            results.Add(sb.ToString().TrimEnd(", ".ToCharArray()));
            isStarted = false;
            return results;
        }

        protected virtual void OnRaiseTriviaEvent(TriviaEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<TriviaEventArgs> handler = RaiseCustomEvent;

            // Event will be null if there are no subscribers
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        private QuestionMultipleChoice ConvertQuestion(Question question)
        {
            var result = new QuestionMultipleChoice();

            var random = new NumberGenerator();
            var value = random.RandomNumber(4);
            string[] otherAnswers = question.incorrect_answers;

            result.correctAnswerLetter = Char.ToString((char)(value + 97));
            result.question = question.question;

            //randomize incorrect answers
            for (var i = question.incorrect_answers.Length - 1; i > 0; i--)
            {
                var answerIndex = random.RandomNumber(i + 1);
                var temp = otherAnswers[answerIndex];
                otherAnswers[answerIndex] = otherAnswers[i];
                otherAnswers[i] = temp;
            }

            var adjustment = 0;
            for (var i = 0; i <= question.incorrect_answers.Length; i++)
            {
                if (i == value)
                {
                    result.answers.Add(question.correct_answer);
                    adjustment = 1;
                }
                else
                {
                    result.answers.Add(otherAnswers[i - adjustment]);
                }
            }

            return result;
        }

        private string ConvertAnswerToDisplay(string answer)
        {
            var unicode = "";
            switch (answer)
            {
                case "a":
                    unicode = "\U0001F170";
                    break;
                case "b":
                    unicode = "\U0001F171";
                    break;
                case "c":
                    unicode = "\U0001F172";
                    break;
                case "d":
                    unicode = "\U0001F173";
                    break;
            }
            return unicode;
        }

        private void TimerTask(object timerState)
        {
            StopCurrentQuestion();
            var output = GetQuestionResultAndSave();
            output.ForEach(message => 
                OnRaiseTriviaEvent(new TriviaEventArgs(client, channel, HttpUtility.HtmlDecode(message))));
            if(!isFinished()){
                var next = NextQuestion();
                OnRaiseTriviaEvent(new TriviaEventArgs(client, channel, HttpUtility.HtmlDecode(next)));
            }else{
                var final = GetTotalScore();
                final.ForEach(message => 
                    OnRaiseTriviaEvent(new TriviaEventArgs(client, channel, HttpUtility.HtmlDecode(message))));
                timer?.Change(Timeout.Infinite, 0);
                timer.Dispose();
            }
        }
    }
}