namespace Demo.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Lucene.Net.Search;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Search;

    using global::Demo;

    /// <summary>
    /// Challenge #3: Retrieving ordered content Example 3 Numeric Range Query
    /// </summary>
    public class DateRangeNumericQuery : ISearchService
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
                return NumericRangeQuery.NewLongRange(
                    IndexCrawler.PublishedDateNumericField,
                    1,
                    Int64.Parse(DateTime.MinValue.ToString("yyyyMMddhhmmss")),
                    Int64.Parse(DateTime.MaxValue.ToString("yyyyMMddhhmmss")),
                    true,
                    true);
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