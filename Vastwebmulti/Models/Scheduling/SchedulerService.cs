using Quartz;
using Quartz.Impl;
using System;
using System.Net;
using static org.apache.commons.codec.language.bm.Rule;

namespace Vastwebmulti.Models.Scheduling
{
    public class SchedulerService
    {
        private static readonly string ScheduleCronExpression = "0 0 3 * * ?";
        private static readonly string ScheduleCronExpression1 = "0 */10 * ? * *";
        private static readonly string ScheduleCronExpression2 = "0 */2 * ? * *"; // Updated cron expression

        public static async System.Threading.Tasks.Task StartAsync()
        {
            try
            {
                var islocal = IsLocalhost();
                if (islocal)
                {
                    var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                    if (!scheduler.IsStarted)
                    {
                        await scheduler.Start();
                    }
                    var job1 = JobBuilder.Create<Task_Delete_data>()
                        .WithIdentity("ExecuteTaskServiceCallJob1", "group1")
                        .Build();
                    var trigger1 = TriggerBuilder.Create()
                        .WithIdentity("ExecuteTaskServiceCallTrigger1", "group1")
                        .WithCronSchedule(ScheduleCronExpression)
                        .Build();

                    await scheduler.ScheduleJob(job1, trigger1);

                    var job2 = JobBuilder.Create<Task_Delete_data>()
                       .WithIdentity("ExecuteTaskServiceCallJob2", "group2")
                       .Build();
                    var trigger2 = TriggerBuilder.Create()
                        .WithIdentity("ExecuteTaskServiceCallTrigger2", "group2")
                        .WithCronSchedule(ScheduleCronExpression)
                        .Build();
                    await scheduler.ScheduleJob(job2, trigger2);

                    var job3 = JobBuilder.Create<Phonepe>()
          .WithIdentity("ExecuteTaskServiceCallJob3", "group3")
          .Build();
                    var trigger3 = TriggerBuilder.Create()
                        .WithIdentity("ExecuteTaskServiceCallTrigger3", "group3")
                        .WithCronSchedule(ScheduleCronExpression1)
                        .Build();

                    await scheduler.ScheduleJob(job3, trigger3);

                    //            var job4 = JobBuilder.Create<Phonepe>()
                    //.WithIdentity("ExecuteTaskServiceCallJob4", "group4")
                    //.Build();
                    //            var trigger4 = TriggerBuilder.Create()
                    //                .WithIdentity("ExecuteTaskServiceCallTrigger4", "group4")
                    //                .WithCronSchedule(ScheduleCronExpression1)
                    //                .Build();

                    //            await scheduler.ScheduleJob(job4, trigger4); 


                    var job5 = JobBuilder.Create<upiresponsetask>()
        .WithIdentity("ExecuteTaskServiceCallJob5", "group5")
        .Build();
                    var trigger5 = TriggerBuilder.Create()
                        .WithIdentity("ExecuteTaskServiceCallTrigger5", "group5")
                        .WithCronSchedule(ScheduleCronExpression2)
                        .Build();

                    await scheduler.ScheduleJob(job5, trigger5);


                    var job6 = JobBuilder.Create<Radaint>()
                               .WithIdentity("ExecuteTaskServiceCallJob6", "group6")
                               .Build();
                    var trigger6 = TriggerBuilder.Create()
                        .WithIdentity("ExecuteTaskServiceCallTrigger6", "group6")
                        .WithCronSchedule(ScheduleCronExpression2)
                        .Build();

                    await scheduler.ScheduleJob(job6, trigger6);
                }
            }
            catch (Exception ex)
            {
                upiresponsetask.WriteLogUPI("The Error Message : " + ex + " Error Time - " + DateTime.Now);
            }
        }
        private static bool IsLocalhost()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                
                if (ip.ToString() == "10.5.1.24"
                    || ip.ToString() == "10.5.1.36"
                    || ip.ToString() == "10.5.1.8"
                    || ip.ToString() == "10.5.1.10"
                    || ip.ToString() == "10.5.1.12"
                    || ip.ToString() == "10.5.1.14"
                    || ip.ToString() == "10.5.1.25"
                    )
                {
                    return true;

                }
            }
            return false;
        }
    }
}