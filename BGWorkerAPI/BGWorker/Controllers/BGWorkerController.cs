using BGWorkerAPI.BGJobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;
using Quartz.Listener;
using System.Text.Json;
using System.Text.Json.Serialization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BGWorkerAPI.BGWorker.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class BGWorkerController : ControllerBase
    {
        //private IScheduler _scheduler;
        private readonly IServiceProvider _serviceProvider;


        public BGWorkerController(
           // IScheduler scheduler, 
            IServiceProvider serviceProvider)
        {
           // _scheduler = scheduler;
            _serviceProvider = serviceProvider;
        }

            // GET: api/<BGWorkerController>
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {

            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];
            List<IJobDetail> list = await BGJobManager.JobsListAsync(scheduler);


            string jsonString="";
            foreach (var job in list)
            {
                jsonString += "Key"+ job.Key + ", Durable" + job.Durable + ", JobDataMap" + job.JobDataMap + ", JobType" 
                    + job.JobType + ", Description" + job.Description +",";

            }

            return new string[] { jsonString };
        }

        // GET api/<BGWorkerController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<BGWorkerController>
        [HttpPost]
        public async Task PostAsync([FromBody] string value)
        {

            //JobKey jobKeySMS = new JobKey("SMSJob", "Groupe1");
            //IJobDetail jobDetailSMS = JobBuilder.Create().newJob(SendSMS.class).withIdentity(jobKeySMS).build();

            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];                //= new StdSchedulerFactory().GetScheduler("Scheduler-Core").GetAwaiter().GetResult();
        //scheduler.Clear();
   // scheduler.Start();

   // scheduler.ScheduleJob(jobDetailSMS, DYNAMIC_TRIGGER);


            string jobName = "BGJOB," ;
            var jobKey = new JobKey(jobName, "DEFAULT");
            // var activejobs = await _scheduler.GetCurrentlyExecutingJobs();
            if (await BGJobManager.CheckExists(scheduler, jobName) == true)
            {
                Console.WriteLine("job already running");
            }
            else
            {
              //  var user = await _userManager.FindByEmailAsync(User.Identity.Name);

                IJobDetail jobDetail = JobBuilder.Create<BasicBGJob>()

                .WithIdentity(jobName)
                .UsingJobData("Username","HugoSpring")
                .UsingJobData("status", "Queued..")

                .UsingJobData("jwttoken", "TOKEN HERE")


                .WithDescription("Send Datas  parameters: " )

                .Build();

                ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity(jobName + "Trigger", "SendCWStoWoo")
                .StartNow()

                .WithSimpleSchedule(x => x.WithIntervalInHours(24)
                                      .WithMisfireHandlingInstructionNextWithExistingCount()
                                      .WithRepeatCount(1)
                //                    .WithMisfireHandlingInstructionFireNow()
                )
                .Build();



                // BGJobManager.ScheduleJob(, jobDetail, trigger);
                var result = scheduler.ScheduleJob(jobDetail, trigger).GetAwaiter().GetResult();
       


                Console.WriteLine(result);
    
            }
        }

            // PUT api/<BGWorkerController>/5
            [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BGWorkerController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
