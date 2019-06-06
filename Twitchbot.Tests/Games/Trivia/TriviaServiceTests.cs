using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NSubstitute;
using Twitchbot.Games.Trivia;
using Twitchbot.Tests.Helpers;
using TwitchLib.Client;
using TwitchLib.Client.Interfaces;
using Xunit;

namespace Twitchbot.Tests.Games.Trivia
{
    public class TriviaServiceTests
    {

        private enum HttpTriviaResponseType
        {
            GOOD,
            NO_RESULT,
            INVALID,
            TOKEN_NOT_FOUND,
            EMPTY_TOKEN
        }

        private QuestionResults goodResponse = new QuestionResults
        {
            response_code = 0,
            results = new Question[]{
                 new Question{
                    category = "Entertainment: Video Games",
                    type = "multiple",
                    difficulty = "medium",
                    question = "The city of Rockport is featured in which of the following video games?",
                    correct_answer = "Need for Speed: Most Wanted (2005)",
                    incorrect_answers = new String[]{
                        "Infamous 2",
                        "Saints Row: The Third",
                        "Burnout Revenge"
                    }
                 }
             }
        };

        private QuestionResults noResultResponse = new QuestionResults
        {
            response_code = 1,
            results = new Question[]{

             }
        };

        private QuestionResults invalidResponse = new QuestionResults
        {
            response_code = 2,
            results = new Question[]{

            }
        };

        private QuestionResults tokenNotFoundResponse = new QuestionResults
        {
            response_code = 1,
            results = new Question[]{

            }
        };

        private QuestionResults emptyTokenResponse = new QuestionResults
        {
            response_code = 1,
            results = new Question[]{

            }
        };

        [Fact]
        public void TriviaService_Constructor()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.GOOD, HttpStatusCode.OK));
            Assert.NotNull(service);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsGood()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.GOOD, HttpStatusCode.OK));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(1, results.Count);
            CompareQuestion(results[0], goodResponse.results[0]);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsNoResult()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.NO_RESULT, HttpStatusCode.OK));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsInvalid()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.INVALID, HttpStatusCode.OK));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsNotFound()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.TOKEN_NOT_FOUND, HttpStatusCode.OK));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsEmptyToken()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.EMPTY_TOKEN, HttpStatusCode.OK));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(0, results.Count);
        }

        [Fact]
        public async void TriviaService_GetTriviaQuestionsBadRequest()
        {
            var service = new TriviaService(CreateFactory(HttpTriviaResponseType.GOOD, HttpStatusCode.BadRequest));
            var results = await service.GetTriviaQuestions(1, TriviaCategoryEnum.VIDEO_GAMES);
            Assert.Equal(0, results.Count);
        }

        private IHttpClientFactory CreateFactory(HttpTriviaResponseType type, HttpStatusCode code)
        {
            string json = "";

            switch (type)
            {
                case HttpTriviaResponseType.GOOD:
                    json = JsonConvert.SerializeObject(goodResponse);
                    break;
                case HttpTriviaResponseType.NO_RESULT:
                    json = JsonConvert.SerializeObject(noResultResponse);
                    break;
                case HttpTriviaResponseType.INVALID:
                    json = JsonConvert.SerializeObject(invalidResponse);
                    break;
                case HttpTriviaResponseType.TOKEN_NOT_FOUND:
                    json = JsonConvert.SerializeObject(tokenNotFoundResponse);
                    break;
                case HttpTriviaResponseType.EMPTY_TOKEN:
                    json = JsonConvert.SerializeObject(emptyTokenResponse);
                    break;
            }

            var httpClientFactoryMock = Substitute.For<IHttpClientFactory>();
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = code,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });
            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);

            httpClientFactoryMock.CreateClient().Returns(fakeHttpClient);

            return httpClientFactoryMock;
        }

        private void CompareQuestion(Question result, Question template)
        {
            Assert.Equal(result.category, template.category);
            Assert.Equal(result.type, template.type);
            Assert.Equal(result.difficulty, template.difficulty);
            Assert.Equal(result.question, template.question);
            Assert.Equal(result.correct_answer, template.correct_answer);

            Assert.Equal(result.incorrect_answers, template.incorrect_answers);
            for (int i = 0; i < result.incorrect_answers.Length; i++)
            {
                Assert.Equal(result.incorrect_answers[i], template.incorrect_answers[i]);
            }
        }
    }
}