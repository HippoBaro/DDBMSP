using System;
using System.Threading.Tasks;
using DDBMSP.Common.PODs.Article;
using DDBMSP.Common.PODs.Article.Components;
using DDBMSP.Common.PODs.User;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using Orleans;
using Orleans.Streams;

namespace DDBMSP.Grains
{
    public class User : StatefulGrain<UserState>, IUser
    {
        private IAsyncStream<IArticle> ArticleStream { get; set; }
        
        public override Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("Default");
            ArticleStream = streamProvider.GetStream<IArticle>(Guid.ParseExact("00000000000000000000000000000001", "N"), "Articles");
            return base.OnActivateAsync();
        }

        public async Task<Guid> AuthorNewArticle(IArticleData articleData)
        {
            Console.WriteLine("here");
            var article = GrainFactory.GetGrain<IArticle>(Guid.NewGuid());
            Console.WriteLine("here");
            articleData.CreationDate = DateTime.UtcNow;
            Console.WriteLine("here");
            articleData.AuthorId = this.GetPrimaryKey();
            Console.WriteLine("here");
            var state = (ArticleState) articleData;
            Console.WriteLine("here");
            state.Exists = true;
            Console.WriteLine("here");
            State.Articles.Add(article.GetPrimaryKey());
            Console.WriteLine("here");
            await article.SetState(state);
            Console.WriteLine("here");
            await ArticleStream.OnNextAsync(article);
            Console.WriteLine("here");
            return article.GetPrimaryKey();
        }
    }
}