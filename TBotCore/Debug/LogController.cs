using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TBotCore.Debug
{
    /// <summary>
    /// Default log controller.
    /// Just put your listeners (IDebuger) to list on initialization,
    /// and they will recieve every debug message in thread safe mode.
    /// </summary>
    public class LogController : IDebuger
    {
        public ReadOnlyCollection<IDebuger> Logs { get; }
        private Mutex Mutex;

        public LogController(params IDebuger[] debugers)
        {
            Logs = new ReadOnlyCollection<IDebuger>(debugers);
            Mutex = new Mutex();

            LogSystem(new DebugMessage($"Log controller initialized. Listeners: {Logs.Count}"));
        }


        public void LogCritical(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogCritical(msg);
            Mutex.ReleaseMutex();
        }

        public void LogError(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogError(msg);
            Mutex.ReleaseMutex();
        }

        public void LogImportant(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogImportant(msg);
            Mutex.ReleaseMutex();
        }

        public void LogMessage(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogMessage(msg);
            Mutex.ReleaseMutex();
        }

        public void LogSucces(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogSucces(msg);
            Mutex.ReleaseMutex();
        }

        public void LogSystem(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogSystem(msg);
            Mutex.ReleaseMutex();
        }

        public void LogWarning(DebugMessage msg)
        {
            Mutex.WaitOne();
            foreach (IDebuger deb in Logs)
                deb.LogWarning(msg);
            Mutex.ReleaseMutex();
        }
    }
}
