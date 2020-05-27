using SampleTgBot.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using TBotCore;
using TBotCore.Core;
using TBotCore.Debug;

namespace SampleTgBot
{
    public partial class MainForm : Form
    {
        private LogController Log;
        private Repository Repository;
        private FormLoger Console;
        private BotManager Bot;

        public MainForm()
        {
            InitializeComponent();
            Console = new FormLoger(this);
        }

        private async Task InitializeBot()
        {
            await Task.Delay(100);

            try
            {
                Log = new LogController(new TextLogger("Logs"), Console);
                Program.SetLog(Log);

                Repository = new Repository();
                DbInitializer dbInitializer = new DbInitializer();

                // if db ok - activate bot core
                // otherwise do nothing, keep error messages on console
                if (dbInitializer.InitializeConnection())
                    Bot = new BotManager(Repository, Log).StartBot();

                this.Invoke((Action)(() => { Text = $"@{Bot.BotApiManager.BotName}, online"; }));
            }
            catch(Exception e)
            {
                Log.LogCritical(new DebugMessage("Bot initialization fails", "InitializeBot()", e));
            }
        }

        #region Console
        class FormLoger : IDebuger
        {
            public delegate void InvokeDebugDelegate(string txt, MsgColor color, bool bold);
            private readonly RichTextBox LogBox;

            public FormLoger(MainForm owner)
            {
                LogBox = owner.LogBox;
            }

            public void Print(string msg, MsgColor color, bool bold = false)
            {
                try
                {
                    LogBox.BeginInvoke(new InvokeDebugDelegate(BeginPrint), new object[] { msg, color, bold });
                }
                catch { }
            }

            private void BeginPrint(string txt, MsgColor color, bool bold)
            {
                int lastSymbol = LogBox.Text.Length;
                LogBox.AppendText(txt);
                LogBox.Select(lastSymbol, LogBox.Text.Length - lastSymbol);
                switch (color)
                {
                    case MsgColor.Black:
                        LogBox.SelectionColor = Color.Black;
                        break;
                    case MsgColor.Red:
                        LogBox.SelectionColor = Color.Red;
                        break;
                    case MsgColor.Green:
                        LogBox.SelectionColor = Color.Green;
                        break;
                    case MsgColor.Purple:
                        LogBox.SelectionColor = Color.Purple;
                        break;
                    case MsgColor.Orange:
                        LogBox.SelectionColor = Color.Orange;
                        break;
                    case MsgColor.Blue:
                        LogBox.SelectionColor = Color.Navy;
                        break;
                    default:
                        LogBox.SelectionColor = Color.Black;
                        break;
                }
                LogBox.SelectionFont = new Font(LogBox.SelectionFont, FontStyle.Bold);

                LogBox.Select(LogBox.Text.Length, 0);

                LogBox.SelectionFont = new Font(LogBox.SelectionFont, FontStyle.Regular);
                LogBox.ScrollToCaret();
            }

            public void LogMessage(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Black);
            }

            public void LogImportant(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Purple);
            }

            public void LogSystem(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Blue, true);
            }

            public void LogSucces(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Green);
            }

            public void LogWarning(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Orange);
            }

            public void LogError(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Red);
            }

            public void LogCritical(DebugMessage msg)
            {
                Print(msg.ToString(), MsgColor.Red, true);
                // do reboot or something...
            }

            public enum MsgColor
            {
                Black,
                Red,
                Green,
                Purple,
                Orange,
                Blue,
            }
        }
        #endregion

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // start iniitializing after full form display
            Task.Run(async () => await InitializeBot());
        }
    }
}
