using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using Lucene.Net.Analysis.Core;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Search.Spans;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DDBMSP.Grains.Aggregators.Articles.Search
{
    [Reentrant]
    [StorageProvider(ProviderName = "RedisStore")]
    class GlobalSearchArticleAggregator : ScheduledPersistedGrain<RAMDirectory>, IGlobalSearchArticleAggregator
    {
        private IndexWriter Writer { get; set; }

        private IndexSearcher _searcher;

        private IndexSearcher Searcher
        {
            get {
                if (State == null)
                    State = new RAMDirectory();
                return _searcher ?? (_searcher = new IndexSearcher(DirectoryReader.Open(State)));
            }
        }

        public override Task OnActivateAsync() {
            Writer = new IndexWriter(State,
                new IndexWriterConfig(LuceneVersion.LUCENE_48, new SimpleAnalyzer(LuceneVersion.LUCENE_48)) {
                    MergeScheduler = new SerialMergeScheduler()
                });
            return base.OnActivateAsync();
        }

        public Task Aggregate(Immutable<ArticleSummary> article) {
            Task Aggregate() {
                var doc = new Document {
                    new TextField("abstract", article.Value.Abstract, Field.Store.YES),
                    new TextField("title", article.Value.Title, Field.Store.YES),
                    new StringField("tag", article.Value.Tags.First(), Field.Store.YES),
                    new TextField("author", article.Value.Author.Name, Field.Store.YES),
                    new TextField("id", article.Value.Id.ToString(), Field.Store.YES)
                };
                Writer.AddDocument(doc);
                Writer.Commit();
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(Aggregate);
        }

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            Task AggregateRange() {
                foreach (var article in articles.Value) {
                    var doc = new Document {
                        new TextField("abstract", article.Abstract, Field.Store.YES),
                        new TextField("title", article.Title, Field.Store.YES),
                        new StringField("tag", article.Tags.First(), Field.Store.YES),
                        new TextField("author", article.Author.Name, Field.Store.YES),
                        new TextField("id", article.Id.ToString(), Field.Store.YES)
                    };
                    Writer.AddDocument(doc);
                }
                Writer.Commit();
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(AggregateRange);
        }

        public Task<Immutable<List<Dictionary<string, string>>>> GetSearchResult(Immutable<string> keywords) {
            var srch = keywords.Value.ToLower();
            var queryAnd = new BooleanQuery {
                {
                    new SpanMultiTermQueryWrapper<PrefixQuery>(new PrefixQuery(new Term("title", srch))) {Boost = 1.5f},
                    Occur.SHOULD
                }, {
                    new SpanMultiTermQueryWrapper<PrefixQuery>(new PrefixQuery(new Term("abstract", srch))),
                    Occur.SHOULD
                }
            };

            var terms = srch.Split(' ');
            foreach (var term in terms) {
                queryAnd.Add(new BooleanClause(new TermQuery(new Term("title", term)) {Boost = 2}, Occur.SHOULD));
                queryAnd.Add(new BooleanClause(new TermQuery(new Term("abstract", term)), Occur.SHOULD));
                queryAnd.Add(new BooleanClause(new TermQuery(new Term("tag", term)), Occur.SHOULD));
                queryAnd.Add(new BooleanClause(new TermQuery(new Term("author", term)), Occur.SHOULD));
            }

            queryAnd.MinimumNumberShouldMatch = 1;

            var hits = Searcher.Search(new BooleanQuery {{queryAnd, Occur.MUST}}, 5).ScoreDocs;

            var res = new List<Dictionary<string, string>>(hits.Length);
            res.AddRange(hits.Select(t => new Dictionary<string, string> {
                {"title", Searcher.Doc(t.Doc).Get("title")},
                {"id", "/post/" + Searcher.Doc(t.Doc).Get("id")},
                {"description", Searcher.Doc(t.Doc).Get("abstract")}
            }));
            return Task.FromResult(res.AsImmutable());
        }
    }
}