using Quartz;
using Quartz.Simpl;
using Quartz.Spi;
using System;

namespace CWS.Quartz
{
    public class IoCJobFactory : SimpleJobFactory
    {
        private readonly IServiceProvider _factory;

        public IoCJobFactory(IServiceProvider factory)
        {
            _factory = factory;
        }
        public override IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _factory.GetService(bundle.JobDetail.JobType) as IJob;
        }

        public override void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
    }
}
