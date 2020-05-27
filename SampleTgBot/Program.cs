using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBotCore.Debug;

namespace SampleTgBot
{
    static class Program
    {
        static IDebuger _Log;
        public static IDebuger Log { get => _Log; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        { 
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        public static void SetLog(IDebuger log)
        {
            if (log == null)
                throw new ArgumentException("log");

            if(_Log == null) _Log = log;
        }
    }
}
