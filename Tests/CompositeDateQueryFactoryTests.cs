namespace Tests
{
    using System;

    using Demo.CompositeField;

    using FluentAssertions;


    using NUnit.Framework;

    [TestFixture]
    public class CompositeDateQueryFactoryTests
    {
        [Test]
        // Seconds
        [TestCase("2000-01-01 00:00:01", "2000-01-01 00:00:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+Minute:00 +(+Second:[01 TO 59])))))")]
        [TestCase("2000-01-01 00:00:00", "2000-01-01 00:00:58", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+Minute:00 +(+Second:[00 TO 58])))))")]
        [TestCase("2000-01-01 00:00:01", "2000-01-01 00:00:58", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+Minute:00 +(+Second:[01 TO 58])))))")]
        [TestCase("2000-01-01 00:00:00", "2000-01-01 00:00:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+Minute:00))))")]
        
        // Minutes
        [TestCase("2000-01-01 00:01:00", "2000-01-01 00:59:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+(Minute:[01 TO 59])))))")]
        [TestCase("2000-01-01 00:00:00", "2000-01-01 00:58:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+(Minute:[00 TO 58])))))")]
        [TestCase("2000-01-01 00:01:00", "2000-01-01 00:58:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+(Minute:[01 TO 58])))))")]
        [TestCase("2000-01-01 00:00:00", "2000-01-01 00:59:59", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00)))")]
        [TestCase("2000-01-01 00:00:00", "2000-01-01 00:59:30", "+Year:2000 +(+Month:01 +(+Day:01 +(+Hour:00 +(+((+Minute:59 +(+Second:[00 TO 30])) Minute:[00 TO 58])))))")]

        // Years
        [TestCase("2000-01-01 00:00:00", "2010-12-31 23:59:59", "+(Year:[2000 TO 2010])")]

        public void ShouldReturnQuery(string startDate, string endDate, string expectedQuery)
        {
            var startDateParsed = DateTime.Parse(startDate);
            var endDateParsed = DateTime.Parse(endDate);

            var result = CompositeDateQueryFactory.GetQuery(startDateParsed, endDateParsed, string.Empty);
            result.ToString().Should().Be(expectedQuery);
        }
    }
}
