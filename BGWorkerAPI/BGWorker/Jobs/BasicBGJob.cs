

using Quartz;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace BGWorkerAPI.BGJobs
{
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class BasicBGJob : IJob
    {
      

        public BasicBGJob(
        
            )
        {
           
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Thread.Sleep(100);
            string[] param = context.JobDetail.Key.Name.Split(',');
            string userName = param[1];
           
              await Task.CompletedTask;
              Console.WriteLine("Basic job finished");
                

          
        }


   
    }
}