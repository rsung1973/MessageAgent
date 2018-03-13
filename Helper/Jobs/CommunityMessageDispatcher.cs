using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Helper;
using MessageAgent.Properties;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Utility;
using WebHome.DataPort;

namespace MessageAgent.Helper.Jobs
{
    public class CommunityMessageDispatcher : IJob
    {
        public void Dispose()
        {

        }

        public void DoJob()
        {
            try
            {
                Console.WriteLine("[" + DateTime.Now + "]工作任務: " + this.GetType().FullName);
                JArray result = MessageOutbound.Instance.GetResidentMessage("");
                if (result != null && result.Count>0)
                {
                    Logger.Debug("讀取資料:");
                    Logger.Debug(JsonConvert.SerializeObject(result));
                    var items = result.Where(r => r.Value<int>("C01_msg_key") > Settings.Default.CurrentMsgKey);
                    if (items.Count() > 0)
                    {
                        Logger.Debug("傳送資料:");
                        Logger.Debug(JsonConvert.SerializeObject(items));

                        using (WebClient client = new WebClient())
                        {
                            client.Encoding = Encoding.UTF8;
                            client.UploadString(Settings.Default.PushMessageUrl, JsonConvert.SerializeObject(items));
                            Settings.Default["CurrentMsgKey"] = items.Max(r => r.Value<int>("C01_msg_key"));
                            Settings.Default.Save();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }                 

        }

        public DateTime GetScheduleToNextTurn(DateTime current)
        {
            return current.AddMinutes(Settings.Default.CheckMessageIntervalInMinutes);
        }
    }

    public static class JobLauncher
    {
        public static void StartUp()
        {

            Console.WriteLine("Daemon Job launches ...");

            JobScheduler.StartUp(Settings.Default.CheckMessageIntervalInMinutes * 60000);

            var jobList = JobScheduler.JobList;

            if (jobList == null || !jobList.Any(j => j.AssemblyQualifiedName == typeof(CommunityMessageDispatcher).AssemblyQualifiedName))
            {
                var jobItem = new JobItem
                {
                    AssemblyQualifiedName = typeof(CommunityMessageDispatcher).AssemblyQualifiedName,
                    Description = "檢查BA訊息公告",
                    Schedule = DateTime.Today.Add(new TimeSpan(0, 10, 0))
                };
                JobScheduler.AddJob(jobItem);
                JobScheduler.LaunchJob(jobItem);
            }

        }
    }

}
