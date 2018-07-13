using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SwissArmyDesktop
{
    class Program
    {
        static void Main(string[] args)
        {
            //murder console window
            var handle = ExternalAPIs.GetConsoleWindow();
#if !DEBUG
            // Hide
            ExternalAPIs.ShowWindow(handle, ExternalAPIs.SW_HIDE);
#else
            // Show
            ExternalAPIs.ShowWindow(handle, ExternalAPIs.SW_SHOW);
#endif

            Console.WriteLine("It starts.");
            Application.Run(new TrayApp());

            Console.WriteLine("Press Enter to Exit...");
        }
    }
}
