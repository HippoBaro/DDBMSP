using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Entities.Enums;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace DDBMSP.CLI
{
    [Verb("generate", HelpText = "Generate data (user, articles, views, comments, etc.)")]
    class Generator {
        [Option('u', "users", Required = true, HelpText = "The total number of user to generate")]
        public int UserNumber { get; set; }
        
        [Option('a', "articles", Required = true, HelpText = "The total number of articles to generate")]
        public int ArticlesNumber { get; set; }
        
        [Option('c', "activities", Required = true, HelpText = "The total number of activities to generate")]
        public int ActivitiesNumber { get; set; }
        
        [Option('r', "random-number-of-articles-per-user", Required = false,
            HelpText = "Each user has a random number of articles. Default: false")]
        public bool RandomArticlesPerUserNumber { get; set; } = false;

        [Option("random-delta", Required = false, HelpText = "Radom delta (in % relative to average) for articles per user. Default: 25")]
        public int RandomDelta { get; set; } = 25;
        
        [Option('o', "output", Required = false, HelpText = "The output file. Default: out.ddbmsp")]
        public string Output { get; set; }

        public int TotalNumberOfElements => UserNumber + ArticlesNumber;
        
        private LinkedList<UserState> Users { get; } = new LinkedList<UserState>();
        private LinkedList<ArticleState> Articles { get;  } = new LinkedList<ArticleState>();
        private LinkedList<UserActivityState> Activities { get;  } = new LinkedList<UserActivityState>();
        private List<StorageUnit> Units { get; } = new List<StorageUnit>();

        public int Run() {
            Init();
            
            Console.WriteLine($"Starting to generate {UserNumber} users and {ArticlesNumber} articles. Total: {UserNumber + ArticlesNumber} elements");
            Console.WriteLine($"Output: {Output}");
            
            Console.WriteLine("Generating...\r");
            GenerateUsers();
            GenerateArticles();
            GenerateComments();
            Console.WriteLine("Generating... Done.");

            Console.WriteLine("Compaction...\r");
            GenerateUnits();
            Console.WriteLine("Compaction... Done.");
            
            Console.WriteLine("Writing file...\r");
            DumpData();
            Console.WriteLine("Writing file... Done.");
            
            return 0;
        }

        private void DumpData() {
            var serializer = new JsonSerializer();
            
            using (var writer = new BsonWriter(new FileStream(Output, FileMode.Create))) {
                serializer.Serialize(writer, Units);
            }
        }

        private void GenerateUnits() {
            StorageUnit New() {
                var articlePerUser = ArticlesNumber / UserNumber;
                var activitiesPerArticle = ActivitiesNumber / ArticlesNumber;
                
                var ret = new StorageUnit {
                    User = Users.First.Value,
                    Articles = Articles.Take(articlePerUser).ToList(),
                    Activities = new List<List<UserActivityState>>()
                };
                foreach (var article in ret.Articles) {
                    article.Author = ret.User.Summarize();
                    ret.User.Articles.Add(article.Summarize());
                    ret.Activities.Add(new List<UserActivityState>(Activities.Take(activitiesPerArticle).ToList()));
                    for (var i = 0; i < activitiesPerArticle; i++) {
                        Activities.RemoveFirst();
                    }
                }

                ret.EntityCount = 1 + ret.Articles.Count + ret.Articles.Count * activitiesPerArticle;
                
                Users.RemoveFirst();
                for (var i = 0; i < articlePerUser; i++) {
                    Articles.RemoveFirst();
                }
                
                return ret;
            }
            
            for (var i = 0; i < UserNumber; i++) {
                Units.Add(New());
            }
        }

        private void GenerateUsers() {
            UserState New() {
                string GenerateRandomName() =>
                    $"{RandomGenerationData.LasttNameList[RandomGenerationData.Random.Next(RandomGenerationData.LasttNameList.Count)]} {RandomGenerationData.SurNameList[RandomGenerationData.Random.Next(RandomGenerationData.SurNameList.Count)]}";

                return new UserState {
                    Id = Guid.NewGuid(),
                    Image = new Uri(RandomGenerationData.ProfileList[RandomGenerationData.Random.Next(RandomGenerationData.ProfileList.Count)]),
                    Gender = RandomGenerationData.Random.Next(2) > 0 ? Gender.Female : Gender.Male,
                    ObtainedCredits = RandomGenerationData.Random.Next(100),
                    Name = GenerateRandomName(),
                    PreferedLanguage = RandomGenerationData.Random.Next(2) > 0 ? Language.English : Language.Mandarin,
                    Region = RandomGenerationData.Random.Next(2) > 0 ? Region.HongKong : Region.MainlandChina,
                    University = RandomGenerationData.UniversityList[RandomGenerationData.Random.Next(RandomGenerationData.UniversityList.Count)],
                    Articles = new List<ArticleSummary>()
                };
            }
            
            for (var i = 0; i < UserNumber; i++) {
                Users.AddLast(New());
            }
        }

        private void GenerateArticles() {
            ArticleState New() {
                List<Tuple<string, string>> GetTopics() {
                    var ret = new List<Tuple<string, string>> {
                        RandomGenerationData.TopicList[
                            RandomGenerationData.Random.Next(RandomGenerationData.TopicList.Count)]
                    };
                    return ret;
                }

                var topic = GetTopics();
                var res = new ArticleState {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now.AddHours(-RandomGenerationData.Random.Next(1000)),
                    Abstract = RandomGenerationData.ExcerptsList[
                        RandomGenerationData.Random.Next(RandomGenerationData.ExcerptsList.Count)],
                    Content = RandomGenerationData.Contents[
                        RandomGenerationData.Random.Next(RandomGenerationData.Contents.Count)],
                    Image = new Uri(
                        RandomGenerationData.ImagesList[
                            RandomGenerationData.Random.Next(RandomGenerationData.ImagesList.Count)]),
                    Language = RandomGenerationData.Random.Next(2) > 0 ? Language.English : Language.Mandarin,
                    Tags = topic.Select(tuple => tuple.Item2).ToList(),
                    Catergory = RandomGenerationData.Random.Next(2) > 0
                        ? ArticleCategory.Science
                        : ArticleCategory.Technology,
                    Title = String.Format(
                        RandomGenerationData.TitleList[
                            RandomGenerationData.Random.Next(RandomGenerationData.TitleList.Count)],
                        topic.First().Item1),
                };
                return res;
            }
            
            for (var i = 0; i < ArticlesNumber; i++) {
                Articles.AddLast(New());
            }
        }
        
        private void GenerateComments() {
            UserActivityState New() {
                var ret = new UserActivityState {
                    Type = (UserActivityType)RandomGenerationData.Random.Next(3),
                    User = Users.ElementAt(RandomGenerationData.Random.Next(Users.Count)).Summarize(),
                    CreationDate = DateTime.Now.AddHours(-RandomGenerationData.Random.Next(1000))
                };
                switch (ret.Type) {
                    case UserActivityType.Commented:
                        ret.Comment = RandomGenerationData.CommentList[RandomGenerationData.Random.Next(RandomGenerationData.CommentList.Count)];
                        break;
                }
                return ret;
            }

            for (var i = 0; i < ActivitiesNumber; i++) {
                Activities.AddLast(New());
            }
        }

        private void Init() {
            if (string.IsNullOrEmpty(Output)) {
                Output = "/exportcli" + "/out.ddbmsp";
            }
        }
    }
}