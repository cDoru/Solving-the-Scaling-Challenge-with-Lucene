using System;

namespace Demo
{
    using System.Diagnostics;

    using Sitecore.Jobs;
    using Sitecore.Search;
    using Sitecore.Web.UI.HtmlControls;

    using Action = System.Action;

    public partial class RebuildIndex : Page
    {

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var job = new Job(new JobOptions("indexrebuild","indexing", "site",SearchManager.GetIndex("stories"), "Rebuild"));
            JobManager.Start(job);

            //var timeTaken = TimeAction(() => SearchManager.GetIndex("stories").Rebuild());
            //this.timeTakenLiteral.Text = timeTaken.TotalMilliseconds.ToString(CultureInfo.InvariantCulture);


        }

        private static TimeSpan TimeAction(Action action)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            action.Invoke();
            stopWatch.Stop();

            return stopWatch.Elapsed;
        }
    }
}