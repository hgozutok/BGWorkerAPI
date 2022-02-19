using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

namespace CWS.Quartz
{
    public class CustomSchedulerFactory : StdSchedulerFactory
    {
        private readonly ScopedJobFactory mJobFactory;

        public CustomSchedulerFactory(ScopedJobFactory ninjectJobFactory, NameValueCollection props) : base(props)
        {
            mJobFactory = ninjectJobFactory;
        }


        protected override IScheduler Instantiate(global::Quartz.Core.QuartzSchedulerResources rsrcs, global::Quartz.Core.QuartzScheduler qs)
        {
            qs.JobFactory = mJobFactory;
            return base.Instantiate(rsrcs, qs);
        }
    }
}
