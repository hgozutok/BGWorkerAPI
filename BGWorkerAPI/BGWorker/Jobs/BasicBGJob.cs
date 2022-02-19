

using Quartz;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace BGWorkerAPI.BGJobs
{
    //public class CWSToWoocommerceJobSettings
    //{
    //    public CWSToWoocommerceJobSettings(string userName, int storeId)
    //    {
    //        this.userName = userName;
    //        StoreId = storeId;
    //      //  CWSWooTaskList.AddSetting(userName, storeId);
    //    }

    //    public string userName { get; set; }
    //    public int StoreId { get; set; }
    //    public int status { get; set; }
    //    public int totalItem { get; set; }
    //    public string statusMessage { get; set; }

    //}

    //public class CWSWooTaskList
    //{
    //    public static List<CWSToWoocommerceJobSettings> allSettings = new List<CWSToWoocommerceJobSettings>();

    //    public static bool AddSetting(CWSToWoocommerceJobSettings settings)
    //    {
    //        //CWSToWoocommerceJobSettings settings = new CWSToWoocommerceJobSettings(userName, storeId);
    //        allSettings.Add(settings);
    //        return true;
    //    }

    //    public static CWSToWoocommerceJobSettings FindSetting(string userName, int storeId)
    //    {
    //        CWSToWoocommerceJobSettings itemTofind;
    //        foreach (var item in CWSWooTaskList.allSettings)
    //        {
    //            if (item.userName == userName)
    //            {
    //                if (item.StoreId == storeId)
    //                {
    //                    itemTofind = item;
    //                    return item;
    //                }
    //                else continue;
    //            }
    //            else continue;
    //        }
    //        return null;
    //    }
    //}
    [PersistJobDataAfterExecution]
    [DisallowConcurrentExecution]
    public class BasicBGJob : IJob
    {
        //private readonly IUserProperties _userProperties;

        public BasicBGJob(
            //IUserProperties userProperties
            )
        {
            //_userProperties = userProperties;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            Thread.Sleep(100);
            string[] param = context.JobDetail.Key.Name.Split(',');
            string userName = param[1];
            //  context.JobDetail.JobDataMap.GetString("Username");
           

                        await Task.CompletedTask;
                        Console.WriteLine("CWSToWoocommerce job finished");
                

          
        }


   
    }
}