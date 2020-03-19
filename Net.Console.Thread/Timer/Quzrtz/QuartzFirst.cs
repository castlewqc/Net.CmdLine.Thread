using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Quartz.MisfireInstruction;

namespace Net.CmdLine.ThreadStudy.Timer.Quzrtz
{
    class QuartzFirst
    {

        public async void Run()
        {
            // construct a scheduler factory
            NameValueCollection props = new NameValueCollection() { { "quartz.serializer.type", "binary" } };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);

            // get a scheduler
            IScheduler sched = await factory.GetScheduler();
            await sched.Start();

            // define the job and tie it to our HelloJob class
            /**
             * JobDetail 是 Job的实例，每次执行都会创建一个新的
             * 那么如何为Job实例提供配置和属性 jobDataMap  它是JobDetail的一部分
             * 
             * Durability — 如果作业不耐用，一旦不再有与之关联的活动触发器Trigger，它将自动从调度程序中删除。换句话说，非持久性作业Job的寿命是由其触发器是否存在所限制。
             * RequestsRecovery — 如果一个作业“请求恢复”，并且它正在调度程序的“硬关闭”期间执行（即它在崩溃中运行的进程，或者机器被关闭），那么它将被重新执行当调度程序再次启动时。在这种情况下，JobExecutionContext.isRecovering（）方法将返回true。

             */
            IJobDetail job = JobBuilder.Create<JobFirst>()
                .WithIdentity("job1", "group1")
                //.SetJobData
                //.StoreDurably(true) //孤立存储，指即使该JobDetail没有关联的Trigger，也会进行存储
                .RequestRecovery(true) // 请求恢复，指应用崩溃后再次启动，会重新执行该作业
                .UsingJobData("key", "value")
                .Build();
            /**
             * 为作业实例提供属性和配置；在多次执行之间跟踪作业状态
             * JobDataMap map = job.JobDataMap;
             * map.Put("key", "value")
             */

            //TriggerUtils.ComputeFireTimesBetween
            // Trigger the job to run now, and then every 1 seconds
            // 全能触发器 CronTrigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                //.UsingJobData 可以为每个触发器提供属性和配置
                .WithSimpleSchedule(x => x.WithRepeatCount(3000)
                    .WithIntervalInSeconds(1)
                    .WithMisfireHandlingInstructionIgnoreMisfires() //配置哑火策略
                    )//.RepeatForever()
            .Build();
            trigger.Priority = 5; // default
            // trigger.MisfireInstruction 没有set 是根据策略来变化的
            IJobDetail job2 = JobBuilder.Create<JobFirst>()
                 .WithIdentity("job2", "group1")
                 //.SetJobData
                 //.StoreDurably(true) //孤立存储，指即使该JobDetail没有关联的Trigger，也会进行存储
                 .RequestRecovery(true) // 请求恢复，指应用崩溃后再次启动，会重新执行该作业
                 .UsingJobData("key", "value")
                 .Build();
            //instruction 指示

            ITrigger trigger2 = TriggerBuilder.Create()
              .WithIdentity("trigger2", "group1")
              .StartNow()
              //.UsingJobData 可以为每个触发器提供属性和配置
              .WithSimpleSchedule(x => x.WithRepeatCount(3)
                  .WithIntervalInSeconds(1)
                  )//.RepeatForever()
          .Build();

            //System.Collections.Generic.IReadonlyConllection
            IDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> triggersAndJobs = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
            {
                [job] = new List<ITrigger>(new List<ITrigger>() { trigger }),
                [job2] = new List<ITrigger>(new List<ITrigger>() { trigger2 })
            };

            // 注意和 System.Collections.ObjectModel.ReadOnlyDictionary 不一样
            sched.ListenerManager.AddJobListener(new JobListener(), KeyMatcher<JobKey>.KeyEquals(new JobKey("job1", "group1")));
            sched.ListenerManager.AddTriggerListener(new TriggerListener(), KeyMatcher<TriggerKey>.KeyEquals(new TriggerKey("trigger1", "group1")));
            //IReadOnlyDictionary<IJobDetail, IReadOnlyCollection<ITrigger>> dictionary = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>(triggersAndJobs);
            //await sched.ScheduleJobs(dictionary, replace: true);
            await sched.ScheduleJob(job, trigger);
            Thread.Sleep(10000);
            Console.WriteLine("standby:" + DateTimeOffset.Now.ToLocalTime());
            await sched.PauseAll();
            Thread.Sleep(TimeSpan.FromSeconds(20));
            Console.WriteLine("ResumeAll:" + DateTimeOffset.Now.ToLocalTime());
            await sched.ResumeAll();
            // standby  -  start ; 
            // shutdown 后不能再start
        }


        /**
         * DisallowConcurrentExecution： 虽然注解在Job类上，但起作用的是JobDetail 同一时间只能执行一个Job实例 但触发的还是会等待执行的
         * PersistJobDataAfterExecution：在执行完成后持久化JobData，该特性是针对Job类型生效的，意味着所有使用该Job的JobDetail都会在执行完成后持久化JobData。
         */

        [DisallowConcurrentExecution]
        [PersistJobDataAfterExecution]  // 设置了这个修改jobdatamap才有效果
        class JobFirst : IJob
        {
            /**
             * 唯一允许被抛出的异常是JobExecutionException
             * 
             */
            public Task Execute(IJobExecutionContext context)
            {
                Console.WriteLine("start------------------------>");

                //context.Trigger.JobDataMap
                //context.MergedJobDataMap 在JobDetail上找到的JobDataMap和在Trigger上找到的JobDataMap的合并，后者中的值覆盖前者中的任何同名值
                var map = context.JobDetail.JobDataMap;

                Task.Delay(2000).Wait();

                return Task.Run(() =>
                {


                    Console.WriteLine("执行" + context.JobDetail.Key.Group + "-" + context.JobDetail.Key.Name + " : " + map.GetString("key"));
                    map.Put("key", DateTimeOffset.Now.ToString());
                    Console.WriteLine("end  ------------------------>");
                });


            }
        }

        /**
         * 否决情况下
TriggerFired
VetoJobExecution
JobListener_JobExecutionVetoed
TriggerFired
VetoJobExecution
JobListener_JobExecutionVetoed
TriggerFired
VetoJobExecution
JobListener_JobExecutionVetoed
TriggerFired
VetoJobExecution
JobListener_JobExecutionVetoed

             不否决情况下
            TriggerFired
VetoJobExecution
JobListener_JobToBeExecuted
start------------------------>
执行group1-job1 : value
end  ------------------------>
JobListener_JobWasExecuted
TriggerComplete
         * 
         */
        class JobListener : IJobListener
        {
            public string Name => "JobListener";

            // 否决
            public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("JobListener_JobExecutionVetoed"));
            }

            public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("JobListener_JobToBeExecuted"));
            }

            public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("JobListener_JobWasExecuted"));
            }
        }

        class TriggerListener : ITriggerListener
        {
            public string Name => "TriggerListener";

            public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("TriggerComplete"));
            }

            // 1
            public Task TriggerFired(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("TriggerFired"));
            }

            public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => Console.WriteLine("TriggerMisfired"));
            }

            // 2 返回true 任务不执行
            public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context, CancellationToken cancellationToken = default)
            {
                return Task.Run(() => { Console.WriteLine("VetoJobExecution"); return false; });
            }
        }

        class SchedulerListener : ISchedulerListener
        {
            public Task JobAdded(IJobDetail jobDetail, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobDeleted(JobKey jobKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobInterrupted(JobKey jobKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobPaused(JobKey jobKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobResumed(JobKey jobKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobScheduled(ITrigger trigger, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobsPaused(string jobGroup, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobsResumed(string jobGroup, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task JobUnscheduled(TriggerKey triggerKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerError(string msg, SchedulerException cause, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerInStandbyMode(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerShutdown(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerShuttingdown(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerStarted(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulerStarting(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task SchedulingDataCleared(CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task TriggerFinalized(ITrigger trigger, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task TriggerPaused(TriggerKey triggerKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task TriggerResumed(TriggerKey triggerKey, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task TriggersPaused(string triggerGroup, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }

            public Task TriggersResumed(string triggerGroup, CancellationToken cancellationToken = default)
            {
                throw new NotImplementedException();
            }
        }
    }
}
