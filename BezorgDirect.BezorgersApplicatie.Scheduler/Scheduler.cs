using BezorgDirect.BezorgersApplicatie.BusinessLogic.Interfaces;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BezorgDirect.BezorgersApplicatie.Scheduler
{
    /* This class was setup and written by Lennart de Waart (563079) */
    public sealed class Scheduler // Class cannot be inherited
    {
        private static Scheduler uniqueInstance;
        private readonly ISchedulerFactory _schedFact;
        private readonly IScheduler _sched;
        private readonly ILogger _logger;

        /// <summary>
        /// Private constructor, not accessible outside this class
        /// </summary>
        /// <param name="logger"></param>
        private Scheduler(ILogger logger)
        {
            _logger = logger;
            _logger.Information($"Constructing a scheduler factory...\n");
            _schedFact = new StdSchedulerFactory();
            // Get and start the scheduler
            _sched = _schedFact.GetScheduler().Result;
            _sched.Start();
        }

        /// <summary>
        /// Static method that returns a singleton instance of the Scheduler class
        /// </summary>
        /// <returns></returns>
        public static Scheduler Instance()
        {
            if (uniqueInstance == null)
                uniqueInstance = new Scheduler(new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($"../../../logs/Log-.txt", 
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}", 
                    rollingInterval: RollingInterval.Day)
                    .CreateLogger());
            return uniqueInstance;
        }

        /// <summary>
        /// Static method that creates a runnable scheduler
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            /* Deprecated */
            // Get expiration time from variables JSON-file
            //dynamic config = JsonConvert.DeserializeObject(
            //    File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            //int eon = (int)config["NotificationsService"]["ExpirationOfNotificationInMinutes"] * 60;           
            // Add a job with trigger to the scheduler
            //switch (args.Length)
            //{
            //    case 0:
            //        Instance().ScheduleJob("CheckNotificationContextForExpiration",
            //            "trigger_CheckNotificationContextForExpiration", DateTime.Now, eon, 0);
            //        break;
            //    case var _ when args.Length > 0:
            //        Instance().ScheduleJob(args[0], args[1], DateTime.Parse(args[2]), 
            //            int.Parse(args[3]), int.Parse(args[4]));
            //        break;
            //    default:
            //        break;
            //}

            // Keep the thread (and therefore the scheduler) alive
            Console.ReadKey();
            // In case a key has been pressed
            while (true) { }
        }

        /// <summary>
        /// Public method which schedules a job with trigger
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="triggerName"></param>
        /// <param name="startDateTime"></param>
        /// <param name="intervalInSeconds"></param>
        /// <param name="repeatCount"></param>
        public void ScheduleJob(string jobName, string triggerName, DateTime startDateTime, int intervalInSeconds, int repeatCount)
        {
            try
            {
                _logger.Information($"Creating a new job with identity: {jobName}...\n");
                IJobDetail job = JobBuilder.Create<Job>()
                        .WithIdentity(jobName, "ContextJobs")
                        .Build();
                job.JobDataMap["logger"] = _logger;
                ITrigger trigger;
                switch (repeatCount)
                {
                    case -1:
                        _logger.Debug($"Creating a trigger for job {jobName} " +
                            $"with identity {triggerName} and {intervalInSeconds} second(s) interval " +
                            $"that will repeat forever...\n");
                        trigger = TriggerBuilder.Create()
                       .StartNow()
                       .WithPriority(1)
                       .WithIdentity(triggerName, "ContextJobs")
                       .WithSimpleSchedule(x => x.WithIntervalInSeconds(intervalInSeconds).RepeatForever())
                       .Build();
                        break;
                    case var _ when repeatCount > -1:
                    default:
                        _logger.Debug($"Creating a trigger for job {jobName} " +
                            $"with identity {triggerName} and {intervalInSeconds} second(s) interval " +
                            $"that will repeat {repeatCount} time(s)...\n");
                        trigger = TriggerBuilder.Create()
                       .StartAt(startDateTime.AddSeconds(intervalInSeconds))
                       .WithPriority(1)
                       .WithIdentity(triggerName, "ContextJobs")
                       .WithSimpleSchedule(x => x.WithIntervalInSeconds(intervalInSeconds)
                       .WithRepeatCount(repeatCount))
                       .Build();
                        break;
                }

                // Schedule the job using the job and trigger 
                _logger.Information($"Scheduling {jobName} using the job and trigger...\n");
                _sched.ScheduleJob(job, trigger);
            }
            catch (Exception e) { _logger.Error($"An exception occured while adding job {jobName} " +
                $"and trigger {triggerName} to the scheduler. Exception message: {e.Message} " +
                $"on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.\n"); }
        }
    }

    /* This class was setup and written by Lennart de Waart (563079) */
    public class Job : IJob
    {
        private ILogger _logger;
        private readonly CancellationTokenSource cts;

        /// <summary>
        /// Creates a new instance of the Job class.
        /// Quartz requires a public empty constructor so that the scheduler can instantiate the class whenever it needs to.
        /// </summary>
        public Job()
        {
            cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            cts.CancelAfter(150000); // Cancel after 2.5 minutes
            token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Task method which executes a scheduled task
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task IJob.Execute(IJobExecutionContext context)
        {
            try
            {
                _logger = (ILogger)context.JobDetail.JobDataMap["logger"];
                // Depending on which job is triggered, choose the action...
                switch (context.JobDetail.Key.Name)
                {
                    case "CheckNotificationContextForExpiration":
                        _logger.Debug($"Executing scheduled task {context.JobDetail.Key.Name}...");
                        if (!RequestUpdateExpiredNotifications())
                            throw new Exception("RequestUpdateExpiredNotifications returned false.");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) 
            {
                _logger.Error($"An exception occured while executing job " +
                    $"{context.JobDetail.Key.Name}. Exception message: {e.Message} " +
                    $"on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.\n"); 
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// This private method calls the API UpdateExpiredNotifications endpoint. 
        /// It does so with a unique access token, because only this function should be able to call the endpoint.
        /// Unfortunately DI didn't work, context was disposed in JobBuilder.Create and therefore could not be called upon.
        /// </summary>
        /// <returns>true or false</returns>
        private bool RequestUpdateExpiredNotifications()
        {
            // Read variables from JSON file
            dynamic config = JsonConvert.DeserializeObject(File.ReadAllText("../../../../BezorgDirect.BezorgersApplicatie.API/variables.json"));
            string URL = $"{(string)config["Scheduler"]["ApiUrl"]}updateExpiredNotifications";
            string urlParameters = $"?Authorization={(string)config["Scheduler"]["AuthToken"]}";   
            // Create a client that calls the api
            HttpClient client = new HttpClient { BaseAddress = new Uri(URL) };
            try
            {     
                // List response
                HttpResponseMessage response = client.PatchAsync(urlParameters, null, cts.Token).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                    return true;
                else
                    throw new Exception("updateExpiredNotifications returned a failure...");
            }
            catch (Exception e) // Error handling
            {
                _logger.Error($"RequestUpdateExpiredNotifications says: {e.Message} Exception occured on line {new StackTrace(e, true).GetFrame(0).GetFileLineNumber()}.");
            }
            finally
            {
                // Dispose once all HttpClient calls are complete
                client.Dispose();
            }
            // Return false if any errors occured along the way
            return false;
        }
    }
}
