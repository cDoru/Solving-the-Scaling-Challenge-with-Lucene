namespace Demo.Examples
{
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore;
    using Sitecore.Data.Items;

    using global::Demo;

    public class OrderdContentSitecoreQuery : ISearchService
    {
        // eg. "/sitecore/content/home/section1/hour/minute/second/article" 
        private readonly string contentQuery;

        public OrderdContentSitecoreQuery(string contentQuery)
        {
            this.contentQuery = contentQuery;
        }

        public IEnumerable<Item> Search()
        {
            var items =
                Context.Database.SelectItems(contentQuery)
                    .OrderBy(item => item.Statistics.Created)
                    .Take(20);
            return items;
        }
    }
}