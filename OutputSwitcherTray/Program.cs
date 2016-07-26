using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OutputSwitcher.Tray
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Thanks to http://odetocode.com/blogs/scott/archive/2004/08/20/the-misunderstood-mutex.aspx
            // for the robust solution of enforcing a single process instance per user.
            using (Mutex mutex = new Mutex(false, mutexId))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("OutputSwitcher already running.", "OutputSwitcher");
                    return;
                }
                else
                {
                    Application.Run(new OutputSwitcher.Tray.OutputSwitcherApplicationContext());
                }
            }
        }

        private static string mutexId = "OutputSwitcher";
    }
}
