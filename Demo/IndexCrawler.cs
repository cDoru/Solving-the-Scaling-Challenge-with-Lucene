namespace Demo
{
    using System;
    using System.Globalization;
    using System.Linq;

    using Lucene.Net.Documents;

    using Sitecore.Data.Items;
    using Sitecore.Search.Crawlers;

    using Field = Lucene.Net.Documents.Field;

    public class IndexCrawler : DatabaseCrawler
    {
        #region fieldNames
        public const string IdField = "_id";

        public const string AncestorsField = "stories_ancestors";

        public const string PublishedDateNumericField = "stories_publishedDate";
        public const string PublishedDateTermField = "stories_publishedDateTerm";

        public const string PublishedDateCompositePrefix = "stories_publishedDate";
        public const string PublishedDateCompositeYear = "stories_publishedDateYear";
        public const string PublishedDateCompositeMonth = "stories_publishedDateMonth";
        public const string PublishedDateCompositeDay = "stories_publishedDateDay";
        public const string PublishedDateCompositeHour = "stories_publishedDateHour";
        public const string PublishedDateCompositeMinute = "stories_publishedDateMinute";
        public const string PublishedDateCompositeSecond = "stories_publishedDateSecond";

        #endregion

        protected override void AddAllFields(Document document, Item item, bool versionSpecific)
        {
            //base.AddAllFields(document, item, versionSpecific);      
            if (item.Versions.IsLatestVersion())
            {
                document.Add(
                    new Field(IdField, item.ID.ToGuid().ToString("N"), Field.Store.YES, Field.Index.NOT_ANALYZED));

                document.Add(
                    new Field(
                        AncestorsField,
                        string.Join(" ", item.Axes.GetAncestors().Select(item1 => item1.ID.ToGuid().ToString("N"))),
                        Field.Store.YES,
                        Field.Index.ANALYZED));


                var publishedDate = item.Statistics.Created;

                AddNumericPublishedDate(document, publishedDate);
                AddTermPublishedDate(document, publishedDate);
                AddCompositePublishedDate(document, publishedDate);
            }

        }

        protected override void AddSpecialFields(Document document, Item item)
        {
            //base.AddSpecialFields(document, item);
        }

        private static void AddNumericPublishedDate(Document document, DateTime publishedDate)
        {
            var publishedDateString = publishedDate.ToString("yyyyMMddhhmmss");
            var publishedDateNumeric = new NumericField(PublishedDateNumericField, Field.Store.YES, true);
            publishedDateNumeric.SetLongValue(long.Parse(publishedDateString));
            document.Add(publishedDateNumeric);
        }

        private static void AddTermPublishedDate(Document document, DateTime publishedDate)
        {
            var publishedDateString = publishedDate.ToString("yyyyMMddhhmmss");

            document.Add(new Field(PublishedDateTermField, publishedDateString, Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        private static void AddCompositePublishedDate(Document document, DateTime publishedDate)
        {
            document.Add(new Field(PublishedDateCompositeYear, publishedDate.Year.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(PublishedDateCompositeMonth, publishedDate.Month.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(PublishedDateCompositeDay, publishedDate.Day.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(PublishedDateCompositeHour, publishedDate.Hour.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(PublishedDateCompositeMinute, publishedDate.Minute.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
            document.Add(new Field(PublishedDateCompositeSecond, publishedDate.Second.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), Field.Store.YES, Field.Index.NOT_ANALYZED));
        }
    }
}