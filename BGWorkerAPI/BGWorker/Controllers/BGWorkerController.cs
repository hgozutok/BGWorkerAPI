using BGWorkerAPI.BGJobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;
using Quartz.Listener;
using Newtonsoft.Json;
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

        // [Route("/ChangeOrderStatus/{storeid}/{orderid}/{status}/")]
        [Route("GetJobList/")]
        public async Task<IEnumerable<string>> GetAsync()
        {

            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];
            List<IJobDetail> list = await BGJobManager.JobsListAsync(scheduler);
            string jsonString = "";
            if (list != null)
                 jsonString= JsonConvert.SerializeObject(list);

            return new string[] { jsonString };
        }

        [HttpGet]
        [Route("GetActiveJobList/")]
        public async Task<IEnumerable<string>> GetActiveJobListAsync()
        {

            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];
            List<IJobDetail> list = await BGJobManager.JobsActiveListAsync(scheduler);
            string jsonString = "";
            if (list != null)
                jsonString = JsonConvert.SerializeObject(list);


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
        [Route("CreateBasicJob/")]
        public async Task<string> PostAsync([FromBody] string value)
        {

            string result = "";

            var schedulers = new StdSchedulerFactory().GetAllSchedulers().Result;

            var scheduler = schedulers[0];             
  



            string jobName = "BGJOB" ;
            var jobKey = new JobKey(jobName, "DEFAULT");
            // var activejobs = await _scheduler.GetCurrentlyExecutingJobs();
            if (await BGJobManager.CheckExists(scheduler, jobName) == true)
            {
                Console.WriteLine("job already running");
                result= "job already running";
            }
            else
            {
            

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



               var results = BGJobManager.ScheduleJob(jobDetail, trigger);
   

                result= JsonConvert.SerializeObject(results);

         
    
            }
            return result;
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
