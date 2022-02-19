using BGWorkerAPI.BGJobs;
using Microsoft.AspNetCore.Mvc;
using Quartz;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BGWorkerAPI.BGWorker.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class BGWorkerController : ControllerBase
    {
        private IScheduler _scheduler;

        public BGWorkerController(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        // GET: api/<BGWorkerController>
        [HttpGet]
        public IEnumerable<string> Get()
        {

            return new string[] { "value1", "value2" };
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
            var jobKey = new JobKey("SendCWSDataToWoocommerce", "DEFAULT");
            if (await _scheduler.CheckExists(jobKey))
            {
                //do st
            }
            else
            {
                IJobDetail jobDetail = JobBuilder.Create<BasicBGJob>()

                .WithIdentity("Basic BG")
                // .UsingJobData("Username", User.Identity.Name)
                // .UsingJobData("status", "queuing..")
                .WithDescription("Send Datas From Codeswholesale to woocommere")

                .Build();
                jobDetail.JobDataMap.Put("StoreId", "1");

                ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity("GetCWSSendWooDataTrigger", "SendCWStoWoo")
                .StartNow()

                .WithSimpleSchedule(x => x.WithIntervalInSeconds(50).WithRepeatCount(1))
                .Build();

                await _scheduler.ScheduleJob(jobDetail, trigger);
                Console.WriteLine("job started");
    
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
