using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Twitchbot.Games.Trivia
{
    public class TriviaService {
        private IHttpClientFactory httpClientFactory;
        
        public TriviaService(IHttpClientFactory factory){
            httpClientFactory = factory;
        }

        public async Task<List<Question>> GetTriviaQuestions(int amount, TriviaCategoryEnum category){
            List<Question> results = new List<Question>();
            HttpClient clientHTTP = httpClientFactory.CreateClient();

            HttpResponseMessage response = await clientHTTP.GetAsync($"https://opentdb.com/api.php?amount={amount}&category={((int)category)}&type=multiple");
            if (response.IsSuccessStatusCode)
            {
                QuestionResults questionsResults = await response.Content.ReadAsAsync<QuestionResults>();
                questionsResults.results.ToList().ForEach(question => results.Add(question));
            }

            return results;
        }
    }
}