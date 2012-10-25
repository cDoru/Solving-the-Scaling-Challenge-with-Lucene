namespace Demo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using Sitecore.Data;
    using Sitecore.Data.Items;
    using Sitecore.SecurityModel;

    using global::Demo.Examples;

    public partial class RunExamples : Page
    {
        private readonly TemplateID sampleItemTemplateId =
            new TemplateID(new ID("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}"));

        private readonly TemplateID folderItemTemplateId =
            new TemplateID(new ID("{A87A00B1-E6DB-45AB-8B54-636FEC3B5523}"));

        private const int NumberOfSections = 5;

        private const int ItemsPerSectionPerRun = 1000;

        private readonly IEnumerable<ISearchService> searchServices = new List<ISearchService>
            {
                //new DescendantsSitecoreQuery(@"/sitecore/content/home/*/*/*/*/*"),
                new DescendantsLuceneQuery(),
                new OrderedContentLuceneTermSort(),
                new OrderedContentLuceneCompositeTermSort(),
                new OrderedContentLuceneNumericTermSort(),
                new DateRangeTermQuery(),
                new DateRangeCompositeTermQuery(),
                new DateRangeNumericQuery(),
                new PersistantReaderDateRangeLuceneQuery(),
                new PersistantSearchContextDateRangeLuceneQuery()
            };

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                this.numberOfItems.Text = 0.ToString(CultureInfo.InvariantCulture);
                IEnumerable<string> types =
                    this.searchServices.Select(service => service.GetType().Name.ToString(CultureInfo.InvariantCulture));
                var resultsLine = string.Join(",", types);
                this.results.Text += resultsLine;
                Log(resultsLine);

                this.runTestsTimeInterval.Text = 30.ToString(CultureInfo.InvariantCulture);
                this.addItemsTimeInterval.Text = 30.ToString(CultureInfo.InvariantCulture);

            }
        }

        protected void AddItems_OnClick(object sender, EventArgs e)
        {
            var items = int.Parse(this.numberOfItems.Text);
            items += ItemsPerSectionPerRun * NumberOfSections;

            var database = Sitecore.Context.Database;

            var homeItem = database.GetItem("/sitecore/content/home");

            this.PopulateSections(homeItem);

            this.numberOfItems.Text = items.ToString(CultureInfo.InvariantCulture);
        }

        protected void RunTests_OnClick(object sender, EventArgs e)
        {
            var runResults = (from searchService in searchServices
                              let timeTaken = TimeAction(() => searchService.Search())
                              select Tuple.Create(searchService.GetType(), timeTaken)).ToList();

            IEnumerable<string> resultText = runResults.Select(tuple => tuple.Item2.TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
            var resultsLine = this.numberOfItems.Text + ","
                              +
                              String.Join(
                                  ",",
                                  resultText);
            this.results.Text += resultsLine + Environment.NewLine;

            Log(resultsLine);
        }

        protected void RunAutomatedTests_OnClick(object sender, EventArgs e)
        {
            this.SetTimeIntervals();
            this.AddItemsTimers.Enabled = true;
            this.RunExamplesTimer.Enabled = false;
        }

        protected void AddItems_Tick(object sender, EventArgs e)
        {
            this.SetTimeIntervals();
            this.AddItemsTimers.Enabled = false;
            this.RunExamplesTimer.Enabled = true;
            this.AddItems_OnClick(sender, e);
        }

        protected void RunTests_Tick(object sender, EventArgs e)
        {
            this.SetTimeIntervals();
            this.AddItemsTimers.Enabled = true;
            this.RunExamplesTimer.Enabled = false;
            this.RunTests_OnClick(sender, e);
        }

        private void SetTimeIntervals()
        {
            this.AddItemsTimers.Interval =
                (int)TimeSpan.FromSeconds(int.Parse(this.addItemsTimeInterval.Text)).TotalMilliseconds;
            this.RunExamplesTimer.Interval =
                (int)TimeSpan.FromSeconds(int.Parse(this.runTestsTimeInterval.Text)).TotalMilliseconds;
        }

        private static void Log(string resultsLine)
        {
            var logPath = HttpContext.Current.Server.MapPath("~/log.txt");

            var writer = File.AppendText(logPath);
            writer.WriteLine(resultsLine);
            writer.Close();
        }

        private static TimeSpan TimeAction(Action action)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            action.Invoke();
            stopWatch.Stop();

            return stopWatch.Elapsed;
        }

        private void PopulateSections(Item homeItem)
        {
            using (new SecurityDisabler())
            {


                var sections = homeItem.GetChildren().ToArray();

                if (sections.Length == 0)
                {
                    sections =
                        Enumerable.Range(1, NumberOfSections).Select(
                            i => homeItem.Add("Section" + i, this.folderItemTemplateId)).ToArray();
                }

                foreach (var section in sections)
                {
                    Enumerable.Range(0, ItemsPerSectionPerRun).Select(
                        i =>
                            {
                                var now = DateTime.Now;
                                var hour = now.ToString("HH");
                                var minute = now.ToString("mm");
                                var second = now.ToString("ss");

                                var hourFolder = section.GetChildren().FirstOrDefault(item => item.Name == hour)
                                                 ?? section.Add(hour, this.folderItemTemplateId);

                                var minuteFolder = hourFolder.GetChildren().FirstOrDefault(item => item.Name == minute)
                                                   ?? hourFolder.Add(minute, this.folderItemTemplateId);

                                var secondFolder = minuteFolder.GetChildren().FirstOrDefault(
                                    item => item.Name == second) ?? minuteFolder.Add(second, this.folderItemTemplateId);

                                return secondFolder.Add(
                                    i.ToString(CultureInfo.InvariantCulture), this.sampleItemTemplateId);
                            }).ToArray();
                }
            }
        }
    }
}