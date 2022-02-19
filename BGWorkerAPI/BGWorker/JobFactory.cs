using BGWorkerAPI.BGJobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace BGWorkerAPI.Quartz
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public JobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return _serviceProvider.GetRequiredService<BasicBGJob>();
        }

        public void ReturnJob(IJob job) { }
    }
}
