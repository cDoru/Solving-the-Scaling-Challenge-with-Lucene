namespace Demo.CompositeField
{
    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public static class QueryExtensions
    {
        public static void AddClause(this BooleanQuery query, Query queryToAdd)
        {
            query.Add(new BooleanClause(queryToAdd, BooleanClause.Occur.MUST));

        }

        public static void AddClause(this BooleanQuery query, Query queryToAdd, BooleanClause.Occur occur)
        {
            query.Add(new BooleanClause(queryToAdd, occur));
        }

        public static void AddTermQuery(this BooleanQuery query, string field, string value)
        {
            var termQuery = new TermQuery(new Term(field, value));
            query.AddClause(termQuery);
        }

        public static void AddRangeQuery(
            this BooleanQuery query, string field, string lowerValue, string upperValue, BooleanClause.Occur occur)
        {
            var rangeQuery = new RangeQuery(new Term(field, lowerValue), new Term(field, upperValue), true);
            query.AddClause(rangeQuery, occur);
        }
    }
}