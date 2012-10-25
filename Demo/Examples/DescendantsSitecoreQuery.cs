namespace Demo.Examples
{
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore.Data;
    using Sitecore.Data.Items;

    public class DescendantsSitecoreQuery : ISearchService
    {
        private readonly string contentQuery;

        public DescendantsSitecoreQuery(string contentQuery)
        {
            this.contentQuery = contentQuery;
        }

        public IEnumerable<Item> Search()
        {
            var database = Sitecore.Context.Database;
            var items = database.SelectItems(this.contentQuery).Take(20);
            return items;
        }

    }
}