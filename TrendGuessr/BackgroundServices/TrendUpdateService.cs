using Microsoft.Win32.TaskScheduler;

namespace TrendGuessr.BackgroundServices

{
    public class TrendUpdateService
    {
        public void RegisterTrendUpdate()
        {
            using (TaskService ts = new TaskService())
            {
                TaskDefinition td = TaskService.Instance.NewTask();
                td.RegistrationInfo.Description = "Updates trending years daily.";
                td.Triggers.Add(new DailyTrigger { StartBoundary = DateTime.Today });

                /* Run action
                td.Actions.Add(new ExecAction();
                ts.RootFolder.RegisterTaskDefinition("TrendGuessr_UpdateService", td);
                */
            }
        }

    }
}
