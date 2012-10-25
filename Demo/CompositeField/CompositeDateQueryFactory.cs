namespace Demo.CompositeField
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Lucene.Net.Index;
    using Lucene.Net.Search;

    public class CompositeDateQueryFactory
    {
        // ReSharper disable PossibleMultipleEnumeration

        private static readonly IEnumerable<Range> Ranges = new[]
            {
                new Range{Name = prefix => prefix + "Year", FloorValue = 0, CeilingValue = 9999, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(4,'0')},
                new Range{Name = prefix => prefix + "Month", FloorValue = 1, CeilingValue = 12, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(2,'0')},
                new Range{Name = prefix => prefix + "Day", FloorValue = 1, CeilingValue = 31, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(2,'0')},
                new Range{Name = prefix => prefix + "Hour", FloorValue = 0, CeilingValue = 23, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(2,'0')},
                new Range{Name = prefix => prefix + "Minute", FloorValue = 0, CeilingValue = 59, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(2,'0')},
                new Range{Name = prefix => prefix + "Second", FloorValue = 0, CeilingValue = 59, Value = i => i.ToString(CultureInfo.InvariantCulture).PadLeft(2,'0')}
            };

        public static Sort GetSort(string prefix, int sortType, bool reverse)
        {
            var sortFields = Ranges.Select(range => new SortField(range.Name(prefix), sortType, reverse)).ToArray();
            return new Sort(sortFields);
        }

        public static Query GetQuery(DateTime startDate, DateTime endDate, string prefix)
        {
            var dateParts = GetDateParts(startDate, endDate);

            return GetQuery(dateParts, prefix).First();
        }

        private static IEnumerable<Range> GetDateParts( DateTime startDate, DateTime endDate)
        {
            var startDateParts = SplitDate(startDate);
            var endDateParts = SplitDate(endDate);

            return Ranges.Select(
                (range, i) =>
                    {
                        range.Start = startDateParts[i];
                        range.End = endDateParts[i];
                        return range;
                    });
        }

        private static int[] SplitDate(DateTime date)
        {
            var dateString = date.ToString("yyy-MM-dd-HH-mm-ss");
            return dateString.Split('-').Select(int.Parse).ToArray();
        }

        private static IEnumerable<Query> GetQuery(IEnumerable<Range> parts, string prefix)
        {
            var currentPart = parts.First();
            var remainingParts = parts.Skip(1);

            if (currentPart.IsRange)
            {
                var splitQueries = Split(parts, prefix);
                if (splitQueries.Any())
                {
                    var query = new BooleanQuery();
                    query.AddClause(splitQueries.First());
                    yield return query;
                }
            }
            else
            {
                var query = new BooleanQuery();
                query.AddTermQuery(currentPart.Name(prefix), currentPart.Value(currentPart.Start));

                if (remainingParts.Any())
                {
                    foreach (var childQuery in GetQuery(remainingParts, prefix))
                    {
                        query.AddClause(childQuery);
                    }
                }

                yield return query;
            }
        }

        private static IEnumerable<Query> Split(IEnumerable<Range> parts, string prefix)
        {
            var currentPart = parts.First();
            var remainingParts = parts.Skip(1);
            
            if(remainingParts.Any())
            {
                var lowerEdgeQuery = GetLowerEdgeQuery(prefix, remainingParts);
                var upperEdgeQuery = GetUpperEdgeQuery(prefix, remainingParts);

                if (lowerEdgeQuery.Any() || upperEdgeQuery.Any() || !currentPart.IsFloorValue || !currentPart.IsCeilingValue)
                {
                    var query = new BooleanQuery();

                    var midRangeStart = currentPart.Value(currentPart.Start);
                    var midRangeEnd = currentPart.Value(currentPart.End);
                    
                    if (lowerEdgeQuery.Any())
                    {
                        midRangeStart = currentPart.Value(currentPart.Start + 1);
                        query.AddClause(GetCombinedTermAndSplitQuery(prefix, lowerEdgeQuery, currentPart), BooleanClause.Occur.SHOULD);
                    }

                    if (upperEdgeQuery.Any())
                    {
                        midRangeEnd = currentPart.Value(currentPart.End - 1);
                        query.AddClause(GetCombinedTermAndSplitQuery(prefix, upperEdgeQuery, currentPart), BooleanClause.Occur.SHOULD);
                    }
                    

                    if (currentPart.End - currentPart.Start > 1)
                    {
                        query.AddRangeQuery(currentPart.Name(prefix), midRangeStart, midRangeEnd, BooleanClause.Occur.SHOULD);
                    }

                    yield return query;
                }         
            }
            else
            {
                if (!currentPart.IsFloorValue || !currentPart.IsCeilingValue)
                {
                    yield return
                        new RangeQuery(
                            new Term(currentPart.Name(prefix), currentPart.Value(currentPart.Start)),
                            new Term(currentPart.Name(prefix), currentPart.Value(currentPart.End)),
                            true);
                }
            }
        }

        private static BooleanQuery GetCombinedTermAndSplitQuery(string prefix, IEnumerable<Query> upperSplitQuery, Range currentPart)
        {
            var upperRangeQuery = new BooleanQuery();
            upperRangeQuery.AddTermQuery(currentPart.Name(prefix), currentPart.Value(currentPart.End));
            upperRangeQuery.AddClause(upperSplitQuery.First());
            return upperRangeQuery;
        }

        private static IEnumerable<Query> GetUpperEdgeQuery(string prefix, IEnumerable<Range> remainingParts)
        {
            var newParts =
                remainingParts.Select(
                    range => 
                        {
                        range.Start = range.FloorValue;
                        return range;
                    });

            return GetQuery(newParts, prefix);
        }

        private static IEnumerable<Query> GetLowerEdgeQuery(string prefix, IEnumerable<Range> remainingParts)
        {
            var newParts = remainingParts.Select(
                range =>
                    {
                        range.End = range.CeilingValue;
                        return range;
                    });

            return GetQuery(newParts, prefix);
        }
        // ReSharper restore PossibleMultipleEnumeration
    }
}