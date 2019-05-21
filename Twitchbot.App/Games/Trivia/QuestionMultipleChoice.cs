using System.Collections.Generic;

namespace Twitchbot.Games.Trivia
{
    public class QuestionMultipleChoice{
        public string question;
        public string correctAnswerLetter;
        public List<string> answers;

        public QuestionMultipleChoice(){
            answers = new List<string>();
        }
    }
}