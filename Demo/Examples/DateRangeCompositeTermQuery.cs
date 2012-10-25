namespace Demo.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Demo.CompositeField;

    using Lucene.Net.Search;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Search;

    using global::Demo;

    public class DateRangeCompositeTermQuery : ISearchService
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
                return CompositeDateQueryFactory.GetQuery(DateTime.MinValue, DateTime.MaxValue, IndexCrawler.PublishedDateCompositePrefix);
            }
        }

        private static Sort Sort
        {
            get
            {
                return new Sort(new SortField(IndexCrawler.PublishedDateNumericField, SortField.LONG, true));
            }
        }
    }
}