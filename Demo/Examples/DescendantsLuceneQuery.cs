namespace Demo.Examples
{
    using System.Collections.Generic;
    using System.Linq;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Search;

    public class DescendantsLuceneQuery : ISearchService
    {
        public IEnumerable<Item> Search()
        {
            var database = Context.Database;

            var index = SearchManager.GetIndex("stories");
            var searcher = index.CreateSearchContext().Searcher;

            TopDocs topDocs = searcher.Search(Query, 20);

            var results =
                topDocs.ScoreDocs.Select(
                    scoreDoc =>
                    database.GetItem(
                        new ID(searcher.Doc(scoreDoc.doc).GetField(IndexCrawler.IdField).StringValue())));

            return results;
        }

        private static Query Query
        {
            get
            {
                var ancestor = Context.Database.GetItem("/sitecore/content/Home");

                return new TermQuery(new Term(IndexCrawler.AncestorsField, ancestor.ID.ToGuid().ToString("N")));
            }
        }
    }
}