
using BGWorkerAPI.Quartz;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace BGWorkerAPI.BGJobs
{
    public class BGJobDetail
    {
        [Key]
        public int ID { get; set; }
        public string JobName { get; set; }
        public string userName { get; set; }
        [ForeignKey("storeID")]
        public int storeID { get; set; }
        public string JobStatus { get; set; }
        public string LastItem { get; set; }
        public DateTime startDate { get; set; }
        public bool active { get; set; }
   
    }
    public class BGJobManager
    {
        private readonly IServiceProvider _serviceProvider;

        public BGJobManager(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public static async Task<DateTimeOffset> ScheduleJob( IJobDetail job, ITrigger trigger)
        {
            //IServiceProvider serviceProvider,
            //   var props = new NameValueCollection
            //   {
            //       { "quartz.scheduler.instanceName", "LocalServer" },
            //   { "quartz.scheduler.instanceId", "LocalServer" },
            //   { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
            //   { "quartz.jobStore.useProperties", "true" },
            //   { "quartz.jobStore.dataSource", "default" },
            //   { "quartz.jobStore.tablePrefix", "QRTZ_" },
            //   { "quartz.jobStore.clustered", "true" },
            //   { "quartz.jobStore.dontSetAutoCommitFalse", "True" },
            //   { "quartz.dataSource.default.provider", "SqlServer" },
            ////   { "serilog.logger.org.quartz", "ERROR,CONSOLE" },
            //    { "quartz.jobStore.lockHandler.type", "Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz" },
            //  // {  "quartz.dataSource.default.connectionString",
            //     //Configuration.GetConnectionString("QuartzConn") },
            //   //  @"Server=GOZUTOK\SQLEXPRESS;Initial Catalog=Quartz;Integrated Security=True" },
            //  // @"Server=HUGO\SQLEXPRESS;Initial Catalog=Quartz;Integrated Security=True" },
            //   { "quartz.threadPool.threadCount", "15" },
            //   { "quartz.serializer.type", "json" },
            //   };
            //   var factory = new StdSchedulerFactory(props);
            //   var sched = await factory.GetScheduler();
            //   sched.JobFactory = new JobFactory(serviceProvider);

            //   await sched.Start();

         


            var result= await GetScheduler().ScheduleJob(job, trigger);
            return result;
        }

        public static IScheduler GetScheduler()
        {
            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];
            return scheduler;
        }

        public static async Task<List<IJobDetail>> JobsListAsync()
        {
            List<IJobDetail> jobs = new List<IJobDetail>();

            foreach (JobKey jobKey in await GetScheduler().GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
            {
                jobs.Add(await GetScheduler().GetJobDetail(jobKey));
            }

            return jobs;
        }

        public static async Task<IJobDetail> GetJobDetailAsync(string jobName)
        {
            //IJobDetail jobs = IJobDetail();
            JobKey jkey = new JobKey(jobName);

            var result = await GetScheduler().GetJobDetail(jkey);

          
                return result;
           

          
        }



        public static async Task<List<IJobDetail>> JobsListAsync(IScheduler scheduler, string username)
        {
            List<IJobDetail> jobs = new List<IJobDetail>();

            foreach (JobKey jobKey in await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
            {
                IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
                string userName = jobDetail.JobDataMap.GetString("Username");

                if (userName == username)
                {
                    jobs.Add(jobDetail);
                }
            }

            return jobs;
        }

        public static async Task<List<IJobDetail>> JobsListAsync(IScheduler scheduler, string username, int? storeid)
        {
            List<IJobDetail> jobs = new List<IJobDetail>();

            foreach (JobKey jobKey in await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup()))
            {
                IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
                string userName = jobDetail.JobDataMap.GetString("Username");
                int StoreId = jobDetail.JobDataMap.GetInt("StoreId");
                JobDataMap dataMap = jobDetail.JobDataMap;
                if (userName == username)
                {
                    if (StoreId == storeid)
                    {
                        string status = await JobsStatusAsync( jobDetail.Key.Name);
                        if (status != null)
                        {
                            jobDetail.JobDataMap.Put("status", status);
                        }

                        //   jobDetail.JobDataMap.Add("lastItem", await JobsLastItemAsync(scheduler, jobDetail.Key.Name));
                        jobs.Add(jobDetail);
                    }
                }
            }

            return jobs;
        }

        public static async Task<List<IJobDetail>> JobsActiveListAsync(IScheduler scheduler, string username, int? storeid)
        {
            List<IJobDetail> jobs = new List<IJobDetail>();
            var jobslist = await scheduler.GetCurrentlyExecutingJobs();
            foreach (var item in jobslist)
            {
                //  IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
                string userName = item.JobDetail.JobDataMap.GetString("Username");
                int StoreId = item.JobDetail.JobDataMap.GetInt("StoreId");
                if (userName == username)
                {
                    if (StoreId == storeid)
                    {
                        jobs.Add(item.JobDetail);
                    }
                }
            }

            return jobs;
        }

        public static async Task<List<IJobDetail>> JobsActiveListAsync(IScheduler scheduler)
        {
            List<IJobDetail> jobs = new List<IJobDetail>();
            var jobslist = await scheduler.GetCurrentlyExecutingJobs();
            foreach (var item in jobslist)
            {
               
                        jobs.Add(item.JobDetail);
          
            }

            return jobs;
        }

        public static async Task<bool> JobCancelAsync( string jobName)
        {
            var jobKey = new JobKey(jobName);

            if (await BGJobManager.CheckExists( jobName))
            {
                await GetScheduler().DeleteJob(jobKey);
                return true;
            }
            else
                return false;
        }

        public static async Task<bool> CheckExists( string jobName)
        {
            var jobKey = new JobKey(jobName);

            if (await GetScheduler().CheckExists(jobKey))
            {
                return true;
            }
            else
                return false;
        }

        public static async Task<string> JobsStatusAsync( string jobName)
        {
            string status = null;
            if (await CheckExists( jobName) == true)
            {
                //var jobKey = new JobKey(jobName);
                //List<IJobDetail> jobs = new List<IJobDetail>();
                var jobslist = await GetScheduler().GetCurrentlyExecutingJobs();

                foreach (var item in jobslist)
                {
                    //  IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
                    if (item.JobDetail.Key.Name == jobName)
                    {
                        try
                        {
                            status = item.JobDetail.JobDataMap["status"].ToString();
                            break;
                        }
                        catch (Exception)
                        {
                            status = null;
                        }

                        break;
                    }
                }
            }
            return status;
        }
        public static async Task<string> JobsLastItemAsync( string jobName)
        {
            string lastItem = null;
            if (await CheckExists( jobName) == true)
            {
                //var jobKey = new JobKey(jobName);
                //List<IJobDetail> jobs = new List<IJobDetail>();
                var jobslist = await GetScheduler().GetCurrentlyExecutingJobs();

                foreach (var item in jobslist)
                {
                    //  IJobDetail jobDetail = await scheduler.GetJobDetail(jobKey);
                    if (item.JobDetail.Key.Name == jobName)
                    {
                        try
                        {
                            lastItem = item.JobDetail.JobDataMap["lastItem"].ToString();
                        }
                        catch (Exception ex)
                        {
                            lastItem = "";
                        }

                        break;
                    }
                }
            }
            return lastItem;
        }
        public static async Task<bool> JobRescheduleAsync(string jobName, DateTimeOffset startTime)
        {
            var jobKey = new JobKey(jobName);

            if (await BGJobManager.CheckExists( jobName))
            {
                TriggerKey triggerkey = new TriggerKey("RescheduleTrigger", "Reschedule");

                ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity("JobCWSToWoocommerceTrigger", "SendCWStoWoo")
                .StartAt(startTime)

                .WithSimpleSchedule(x => x.WithIntervalInSeconds(5)

                                    .WithRepeatCount(1)
                                    .WithMisfireHandlingInstructionFireNow())
                .Build();
                await GetScheduler().RescheduleJob(triggerkey, trigger);
                return true;
            }
            else
                return false;
        }


    }
}