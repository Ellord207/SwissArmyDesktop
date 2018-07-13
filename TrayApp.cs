using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Quartz;
using Quartz.Impl;

using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Threading;

//Windows API Documentation
// https://msdn.microsoft.com/en-us/library/ff468919(v=vs.85).aspx
// https://msdn.microsoft.com/en-us/library/windows/desktop/ms682073(v=vs.85).aspx
namespace SwissArmyDesktop
{
    public class TrayApp : ApplicationContext
    {
        public static Icon OnIcon = new Icon(@"\\NAOU24186\Users\Nathan.Elrod.BENTLEY\Documents\Shared\Walker\Resources\magical_shield_HtR_icon.ico");
        public static Icon OffIcon = new Icon(@"\\NAOU24186\Users\Nathan.Elrod.BENTLEY\Documents\Shared\Walker\Resources\shield_VaX_icon.ico");

        IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();

        static Random r = new Random();
        NotifyIcon notifyIcon = new NotifyIcon();
        IntPtr hWnd;

        static DateTime eventHorizon = new DateTime();
        public TrayApp()
            {
            hWnd = ExternalAPIs.GetConsoleWindow();
            Console.WriteLine("Hello from the TrayApp:{0}", Thread.CurrentThread.ManagedThreadId);
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            MenuItem KickThreadItem = new MenuItem("Race!", new EventHandler(Walk));
            MenuItem GetSpriteItem = new MenuItem("Get Sprite", new EventHandler(Get));
            MenuItem PauseItem = new MenuItem("Pause", new EventHandler(Pause));
            MenuItem EventItem = new MenuItem("When?", new EventHandler(NextEvent));
            //MenuItem OrderItem = new MenuItem("WindowOrder", new EventHandler(WindowOrder));

            notifyIcon.Icon = OnIcon;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { EventItem, PauseItem, GetSpriteItem, KickThreadItem, exitMenuItem });
            notifyIcon.Visible = true;

            // and start it off
            scheduler.Start();
            SchedulerAlarm(scheduler);
            }

        #region Menu Options
        private void WindowOrder(object sender, EventArgs e)
            {

            }

        private void NextEvent(object sender, EventArgs e)
            {
            MessageBox.Show("The next runner will be by at " + eventHorizon.ToShortTimeString());
            }

        void Pause(object sender, EventArgs e)
            {
            if (scheduler.IsTriggerGroupPaused("group1"))
                {
                notifyIcon.Icon = OnIcon;
                scheduler.ResumeAll();
                }
            else
                {
                notifyIcon.Icon = OffIcon;
                scheduler.PauseAll();
                }
            }

        void Exit(object sender, EventArgs e)
            {
            scheduler.Shutdown();
            // We must manually tidy up and remove the icon before we exit.
            // Otherwise it will be left behind until the user mouses over.
            notifyIcon.Visible = false;
            Application.Exit();
            }

        void Walk(object sender, EventArgs e)
            {
            SpriteImporter.SPRITE s;
            for (int j = 0; j < 8; j++)
                {
                s = SpriteImporter.getByNumber(j);
                ClearForm.KickWalkerThread(s);
                }
            }

        void Get(object sender, EventArgs e)
            {
            int i;
            try
                {
                i = int.Parse(Interaction.InputBox("Select a number between 1 and " + SpriteImporter.FieldCount + ".", "Select Sprite", ""));
                }
            catch (Exception ex)
                { return; }
            SpriteImporter.SPRITE s = SpriteImporter.getByNumber(i - 1);
            if (s == null) return;
            ClearForm.KickWalkerThread(s);
            Console.WriteLine("At the front: {0}", ExternalAPIs.BringWindowToTop(hWnd).ToString());
            }
        #endregion

        #region Scheduler Methods
        private static void SchedulerAlarm(IScheduler scheduler)
            {
            scheduler.Clear();
#if DEBUG
            int minutesTillEvent = 1;
#else
            int minutesTillEvent = r.Next(20, 70);
#endif
            eventHorizon = DateTime.Now.AddMinutes(minutesTillEvent);

            IJobDetail job = JobBuilder.Create<TrayJobs>()
                .WithIdentity("myJob", "group1") // name "myJob", group "group1"
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(minutesTillEvent)
                    .WithRepeatCount(1)
                    )
                .Build();
            // Tell quartz to schedule the job using our trigger
            scheduler.ScheduleJob(job, trigger);
            }
        public class TrayJobs : IJob
            {
            static int fireCount = 0;
            static Random r = TrayApp.r;
            public void Execute(IJobExecutionContext context)
                {
                if (fireCount++ == 0)
                    return;
                fireCount = 0;
                TrayApp.SchedulerAlarm(context.Scheduler);
                int i = r.Next() % SpriteImporter.FieldCount;
                SpriteImporter.SPRITE s = SpriteImporter.getByNumber(i);
                ClearForm.KickWalkerThread(s);
                }
            }
        #endregion

        }
    }
