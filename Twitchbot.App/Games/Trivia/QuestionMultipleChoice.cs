using System.Collections.Generic;

namespace Twitchbot.Games.Trivia
{
    public class QuestionMultipleChoice{
        public string Question;
        public string CorrectAnswerLetter;
        public List<string> Answers;

        public QuestionMultipleChoice(){
            Answers = new List<string>();
        }
    }
}