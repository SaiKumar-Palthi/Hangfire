using Hangfire;
using Hangfire.Common;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace HangfireDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : Controller
    {
        [HttpPost]
        [Route("CreateBackgroudJob")]
        public ActionResult CreateBackgroudJob()
        {
            List<int> docs = new List<int>() { 1, 2, -1 };
            EmailService emailService = new EmailService();
            foreach (var item in docs)
            {
                BackgroundJob.Enqueue(() => emailService.ProcessDocument(item));
            }

            int[] value = [1, 2, 3, 4];
            int[] nums = value;
            int position = nums[0];
            int[] result = [];
            for (int i = 0; i <= nums.size; i++)
            {
                result[i] = position;
                position = nums[i] + nums[i + 1];
            }

            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            EmailService emailService = new EmailService();
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            BackgroundJob.Schedule(() => emailService.TriggerScheduledJob(), dateTimeOffset);
            return Ok();
        }
        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            var jobId = BackgroundJob.Schedule(() => Console.WriteLine(" Continued Scheduled Job Triggered"), dateTimeOffset);
            var job2Id = BackgroundJob.ContinueJobWith(jobId, () => Console.WriteLine("Continuation Job 1 Triggered"));
            var job3Id = BackgroundJob.ContinueJobWith(job2Id, () => Console.WriteLine("Continuation Job 2 Triggered"));
            return Ok();
        }
        [HttpPost]
        [Route("EmailExample")]
        public ActionResult EmailScheduler()
        {
            
            RecurringJob.AddOrUpdate<EmailService>("send-weekly-emails",
                                                    service => service.SendWeeklyEmails(),
                                                    Cron.Hourly);

            RecurringJob.AddOrUpdate<EmailService>("send-monthly-emails",
                                                    service => service.SendMonthlyEmails(),
                                                    Cron.Weekly);
            return Ok();
        }
    }
    public class EmailService
    {
        [DisplayName("Document Processing for Doc :  {0}")]
        [AutomaticRetry(Attempts = 2, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
        public void ProcessDocument(int item)
        {

            Console.WriteLine($"Document Processing is started for doc : {item}");

            Thread.Sleep(5000);
            if(item == -1)
            {
                throw new Exception($"Document failed with ID  : {item}");
            }

            Console.WriteLine($"Document Processing is complted sucessfully for doc : {item}");
        }
        [DisplayName("Scheduled Job")]
        public void TriggerScheduledJob()
        {
            Console.WriteLine("Scheduled Job Started");
            Thread.Sleep(5000);
            Console.WriteLine("Scheduled Job Complted");
        }

        [DisplayName("Sending Weekly Emails")]
        public void SendWeeklyEmails()
        {
            Console.WriteLine("Sending Weekly Email Completed");

            //throw new Exception("A custom message for an application specific exception");
        }
        [DisplayName("Sending Monthly Emails")]
        public void SendMonthlyEmails()
        {
            Console.WriteLine("Sending Montly Email Completed");
        }
    }



    //[HttpPost]
    //[Route("CreateRecurringJob")]
    //public ActionResult CreateRecurringJob()
    //{
    //    RecurringJob.AddOrUpdate("RecurringJobl", () => Console.WriteLine("Recurring Job Triggered"),Cron.Weekly);
    //    return Ok();
    //}
}
