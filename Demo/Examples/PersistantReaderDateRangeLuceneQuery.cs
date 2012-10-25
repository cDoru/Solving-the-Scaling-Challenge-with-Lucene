namespace Demo.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    using Sitecore;
    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.Search;

    using global::Demo;

    /// <summary>
    /// Challenge #3: Retrieving ordered content Example 2 a Range Query
    /// </summary>
    public class PersistantReaderDateRangeLuceneQuery : ISearchService
    {
        private static IndexReader reader;

        public IEnumerable<Item> Search()
        {
            var database = Context.Database;

            TopFieldDocs topFieldDocs = Searcher.Search(Query, null, 20, Sort);

            var results =
                topFieldDocs.ScoreDocs.Select(
                    scoreDoc =>
                    database.GetItem(
                        new ID(Searcher.Doc(scoreDoc.doc).GetField(IndexCrawler.IdField).StringValue())));

            return results;
        }

        private static IndexSearcher Searcher
        {
            get
            {
                reader = reader!=null ? reader.Reopen() : IndexReader.Open(SearchManager.GetIndex("stories").Directory, true);

                return new IndexSearcher(reader);
            }
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
                return new Sort(new SortField(IndexCrawler.PublishedDateTermField, SortField.LONG, true));
            }
        }
    }
}