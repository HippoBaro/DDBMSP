﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
 using CommandLine;
 using DDBMSP.Entities.Article;
 using DDBMSP.Entities.Article.Components;
 using DDBMSP.Entities.Enums;
 using DDBMSP.Entities.User;
 using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
 using ShellProgressBar;

namespace DDBMSP.CLI
{
    [Verb("generate", HelpText = "Generate data (user, articles, views, comments, etc.)")]
    class Generator {
        [Option('u', "users", Required = true, HelpText = "The total number of user to generate")]
        public int UserNumber { get; set; }
        
        [Option('a', "articles", Required = true, HelpText = "The total number of articles to generate")]
        public int ArticlesNumber { get; set; }
        
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
        private List<StorageUnit> Units { get; } = new List<StorageUnit>();

        public int Run() {
            Init();
            
            Console.WriteLine($"Starting to generate {UserNumber} users and {ArticlesNumber} articles. Total: {UserNumber + ArticlesNumber} elements");
            Console.WriteLine($"Output: {Output}");
            
            GenerateUsers();
            GenerateArticles();

            GenerateUnits();
            
            DumpData();
            
            return 0;
        }

        private void DumpData() {
            var serializer = new JsonSerializer();

            var t = Stopwatch.StartNew();
            using (var writer = new BsonWriter(new FileStream(Output, FileMode.Create))) {
                serializer.Formatting = Formatting.Indented;
                writer.WriteStartArray();
                for (var i = 0; i < Units.Count; i++) {
                    serializer.Serialize(writer, Units[i]);
                    if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                    WritingPB.Tick(i);
                    t.Restart();
                }
                writer.WriteEndArray();
                WritingPB.Tick(UserNumber);
                Program.ProgressBar.Tick();
            }
        }

        private void GenerateUnits() {
            StorageUnit New() {
                var articlePerUser = ArticlesNumber / UserNumber;
                
                var ret =  new StorageUnit {
                    User = Users.First.Value,
                    Articles = Articles.Take(articlePerUser).ToList()
                };
                foreach (var article in ret.Articles) {
                    article.Author = ret.User.Summarize();
                    ret.User.Articles.Add(article.Summarize());
                }
                
                ret.EntityCount = 1 + ret.Articles.Count;
                
                Users.RemoveFirst();
                for (var i = 0; i < articlePerUser; i++) {
                    Articles.RemoveFirst();
                }
                
                return ret;
            }

            var t = Stopwatch.StartNew();
            for (var i = 0; i < UserNumber; i++) {
                Units.Add(New());
                if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                UnitsPB.Tick(i);
                t.Restart();
            }
            UnitsPB.Tick(UserNumber);
            Program.ProgressBar.Tick();
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

            var t = Stopwatch.StartNew();
            for (var i = 0; i < UserNumber; i++) {
                Users.AddLast(New());
                if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                UserPB.Tick(i);
                t.Restart();
            }
            UserPB.Tick(UserNumber);
            Program.ProgressBar.Tick();
        }

        private void GenerateArticles() {
            ArticleState New() {
                List<string> GetTagList() {
                    var ret = new List<string>();
                    if (RandomGenerationData.Random.Next(10) > 1) {
                        ret.Add(RandomGenerationData.TagsList[RandomGenerationData.Random.Next(RandomGenerationData.TagsList.Count)]);
                        ret.Add(RandomGenerationData.TagsList[RandomGenerationData.Random.Next(RandomGenerationData.TagsList.Count)]);
                        return ret;
                    }
                    ret.Add(RandomGenerationData.TagsList[RandomGenerationData.Random.Next(RandomGenerationData.TagsList.Count)]);
                    return ret;
                }

                return new ArticleState {
                    Id = Guid.NewGuid(),
                    CreationDate = DateTime.Now.AddHours(-RandomGenerationData.Random.Next(1000)),
                    Abstract = RandomGenerationData.ExcerptsList[RandomGenerationData.Random.Next(RandomGenerationData.ExcerptsList.Count)],
                    Content = RandomGenerationData.Contents[RandomGenerationData.Random.Next(RandomGenerationData.Contents.Count)],
                    Image = new Uri(RandomGenerationData.ImagesList[RandomGenerationData.Random.Next(RandomGenerationData.ImagesList.Count)]),
                    Language = RandomGenerationData.Random.Next(2) > 0 ? Language.English : Language.Mandarin,
                    Tags = GetTagList(),
                    Title = RandomGenerationData.TitleList[RandomGenerationData.Random.Next(RandomGenerationData.TitleList.Count)],
                    Catergory = RandomGenerationData.Random.Next(2) > 0 ? ArticleCategory.Science : ArticleCategory.Technology,
                };
            }
            
            var t = Stopwatch.StartNew();
            for (var i = 0; i < ArticlesNumber; i++) {
                Articles.AddLast(New());
                if (t.ElapsedMilliseconds <= Program.ProgressBarRefreshDelay) continue;
                ArticlePB.Tick(i);
                t.Restart();
            }
            ArticlePB.Tick(ArticlesNumber);
            Program.ProgressBar.Tick();
        }

        private ChildProgressBar UserPB { get; set; }
        private ChildProgressBar ArticlePB { get; set; }
        private ChildProgressBar UnitsPB { get; set; }
        private ChildProgressBar WritingPB { get; set; }

        private void Init() {
            if (string.IsNullOrEmpty(Output)) {
                Output = Environment.CurrentDirectory + "/out.ddbmsp";
            }
            Program.ProgressBar = new ProgressBar(4, "Generating data", Program.ProgressBarOption);

            UserPB = Program.ProgressBar.Spawn(UserNumber, "Users", Program.ProgressBarOption);
            ArticlePB = Program.ProgressBar.Spawn(ArticlesNumber, "Articles", Program.ProgressBarOption);
            UnitsPB = Program.ProgressBar.Spawn(UserNumber, "Compaction", Program.ProgressBarOption);
            WritingPB = Program.ProgressBar.Spawn(UserNumber, "Writting", Program.ProgressBarOption);
            
        }
    }
}