using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SwissArmyDesktop
    {
    class ClearForm
        {
        const int TASKBAR_HEIGHT = 40;
        static Point location;
        static IntPtr hWnd;
        static void locationChangeEvent(object sender, EventArgs e)
            {
            Form form = (Form)sender;
            form.LocationChanged -= locationChangeEvent;
            form.Location = location;
            }

        static public void KickWalkerThread(SpriteImporter.SPRITE sprite)
            {
            Console.WriteLine("Before the Thread starts.");
            new Thread(() =>
            {//The sprites run on this thread.
                Console.WriteLine("Hello from Thread:{0}", Thread.CurrentThread.ManagedThreadId);
                Form form = new Form();
                form.StartPosition = FormStartPosition.Manual;
                hWnd = ExternalAPIs.GetConsoleWindow();

                #region Finding Screen Area
                form.Bounds = GetFormRectangle(sprite.bitmap);
                List<Screen> screens = GetScreenArea();
                int leftSide = MostLeft(screens);//Strong side
                int rightSide = MostRight(screens);
                #endregion

                #region Adding PictureBox
                form.FormBorderStyle = FormBorderStyle.None;
                form.BackColor = Color.DodgerBlue;
                form.TransparencyKey = Color.FromArgb(0, Color.DodgerBlue);
                form.Icon = TrayApp.OnIcon;

                PictureBox pic = new PictureBox() { Left = form.Right, SizeMode = PictureBoxSizeMode.AutoSize };
                if (sprite.bitmap == null)
                    pic.Load(sprite.filename);
                else
                    pic.Image = sprite.bitmap;
                pic.Top = 0;
                pic.Left = 0;
                form.Controls.Add(pic);
                #endregion

                #region Animation
                System.Windows.Forms.Timer timeout = new System.Windows.Forms.Timer();

                var top = ExternalAPIs.GetTopWindow((IntPtr)null);
                var local = hWnd;

                System.Windows.Forms.Timer animation = new System.Windows.Forms.Timer();
                animation.Tick += new System.EventHandler((sender, e) =>
                {
                    var templocal = ExternalAPIs.GetConsoleWindow();
                    if (templocal != local)
                        {
                        local = templocal;
                        Console.WriteLine("\tTop most window: {0}", top);
                        Console.WriteLine("\tLocal Window local: {0}", local);
                        }
                    var temptop = ExternalAPIs.GetTopWindow((IntPtr)null);
                    if (temptop != top)
                        {
                        top = temptop;
                        Console.WriteLine("\tTop most window: {0}", top);
                        Console.WriteLine("\tLocal Window local: {0}", local);
                        }
                    Thread.BeginCriticalRegion();
                    try
                        {
                        if (pic.Right < leftSide || form.Right < leftSide)
                            { form.TopMost = false; form.TopLevel = false; form.Visible = false; form.Close(); }
                        form.Left -= pic.Width / sprite.jumpDiv;
                        }
                    catch (Exception ex) { Debug.WriteLine(ex.Message); }
                    Thread.EndCriticalRegion();
                });
                animation.Interval = sprite.tickSpeed;
                animation.Enabled = true;
                #endregion

                location = new Point(rightSide, Screen.PrimaryScreen.Bounds.Bottom - pic.Height - TASKBAR_HEIGHT);
                form.LocationChanged += new System.EventHandler(locationChangeEvent);

                form.TopMost = true;
                form.FormClosing += new FormClosingEventHandler((sender, e)
                    => { animation.Stop(); timeout.Stop(); });
                Thread.BeginCriticalRegion();
                Console.WriteLine("Show Dialog");
                form.ShowDialog();
                Thread.EndCriticalRegion();
                //Application.Run(f);

            }).Start();
            }

        #region finding_screen_bounds
        static int MostRight(List<Screen> screens)
            {
            int max = screens.FirstOrDefault().Bounds.Right;
            foreach (var s in screens)
                {
                max = s.Bounds.Right > max ? s.Bounds.Right : max;
                }
            return max;
            }
        static int MostLeft(List<Screen> screens)
            {
            int max = screens.FirstOrDefault().Bounds.Left;
            foreach (var s in screens)
                {
                max = s.Bounds.Left < max ? s.Bounds.Left : max;
                }
            return max;
            }

        static List<Screen> GetScreenArea()
            {
            List<Screen> sList = new List<Screen>();
            Screen[] screens = Screen.AllScreens;
            foreach (var s in screens)
                {
                if (s.Bounds.Bottom == Screen.PrimaryScreen.Bounds.Bottom)
                    sList.Add(s);
                }
            return sList;
            }

        static Rectangle GetFormRectangle(Image img)
            {
            // task bar height is 40
            int margin = img.Height;
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            Rectangle frame = new Rectangle(
                x: screen.Right,
                y: screen.Bottom - margin - 40,
                width: img.Width,
                height: margin);// screen.Height);
            return frame;

            }

        #endregion

        }
    }
