namespace Demo
{
    using System.Collections.Generic;
    
    using Sitecore.Data.Items;

    public interface ISearchService
    {
        IEnumerable<Item> Search();
    }
}