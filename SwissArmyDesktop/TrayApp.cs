using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
        IntPtr m_hWnd;

        public TrayApp()
            {
            m_hWnd = ExternalAPIs.GetConsoleWindow();
            Console.WriteLine("Hello from the TrayApp:{0}", Thread.CurrentThread.ManagedThreadId);
            MenuItem exitMenuItem = new MenuItem("Exit", new EventHandler(Exit));
            MenuItem KickThreadItem = new MenuItem("Race!", new EventHandler(Walk));
            MenuItem GetSpriteItem = new MenuItem("Get Sprite", new EventHandler(Get));
            MenuItem PauseItem = new MenuItem("Pause", new EventHandler(Pause));
            MenuItem OrderItem = new MenuItem("List O' Windows", new EventHandler(WindowOrder));

            notifyIcon.Icon = OnIcon;
            notifyIcon.ContextMenu = new ContextMenu(new MenuItem[]
                { OrderItem, PauseItem, GetSpriteItem, KickThreadItem, exitMenuItem });
            notifyIcon.Visible = true;

            // and start it off
            scheduler.Start();
            SchedulerAlarm(scheduler);

            //Mouse Events
            MouseHook.Start();
            MouseHook.MouseAction += new EventHandler(TrayApp_Click);
            notifyIcon.Disposed += new EventHandler(Exit);
            }

        ~TrayApp()
            {
            Exit(this, null);
            }

        #region Events Handlers
        private void TrayApp_Click(object sender, EventArgs e)
            {
            MouseHook.Stop();
            IntPtr hWnd = ExternalAPIs.GetForegroundWindow();
            if (hWnd != null)
                {
                StringBuilder windowTitle = new StringBuilder(255);
                ExternalAPIs.GetWindowText(hWnd, windowTitle, 255);
                string title = windowTitle.ToString();
                Console.WriteLine("Process: {0} : Window title: {1}", hWnd.ToString(), title);
                }
            MouseHook.Start();
            }
        #endregion

        #region Menu Options
    private void WindowOrder(object sender, EventArgs e)
            {
            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
                {
                IntPtr hWnd = process.MainWindowHandle;
                StringBuilder windowTitle = new StringBuilder(255);
                if (hWnd != (IntPtr)null)
                    {
                    ExternalAPIs.GetWindowText(hWnd, windowTitle, 255);
                    string title = windowTitle.ToString();
                    if (!string.IsNullOrEmpty(title))
                        Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, title);
                    }
                }
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
            MouseHook.Stop();
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
                SpriteImporter.SPRITE s = SpriteImporter.getByNumber(i - 1);
                if (s == null) return;
                ClearForm.KickWalkerThread(s);
                }
            catch (Exception ex)
                { return; }

            //Console.WriteLine("At the front: {0}", ExternalAPIs.BringWindowToTop(hWnd).ToString());
            Console.WriteLine("At the front: {0}", ExternalAPIs.GetWindow(m_hWnd, ExternalAPIs.GW_HWNDFIRST).ToString());
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
