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

    using global::Demo;

    public class OrderedContentLuceneTermSort : ISearchService
    {
        public IEnumerable<Item> Search()
        {
            var database = Context.Database;

            var index = SearchManager.GetIndex("stories");
            var searcher = index.CreateSearchContext().Searcher;

            TopFieldDocs topFieldDocs = searcher.Search(Query, null, 20, Sort);

            var results =
                topFieldDocs.ScoreDocs.Select(
                    scoreDoc =>
                    database.GetItem(
                        new ID(searcher.Doc(scoreDoc.doc).GetField(IndexCrawler.IdField).StringValue())));

            return results;
        }

        private static Query Query
        {
            get
            {
                var ancestor = Context.Database.GetItem("/sitecore/content/home");

                return new TermQuery(new Term(IndexCrawler.AncestorsField, ancestor.ID.ToGuid().ToString("N")));
            }
        }

        private static Sort Sort
        {
            get
            {
                return new Sort(new SortField(IndexCrawler.PublishedDateTermField, SortField.STRING, true));
            }
        }
    }
}