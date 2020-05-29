using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Debug
{
    /// <summary>
    /// Debugger interface, it's all, really!
    /// </summary>
    public interface IDebuger
    {
        /// <summary>
        /// Logs regular message.
        /// </summary>
        public void LogMessage(DebugMessage msg);

        /// <summary>
        /// Logs important message.
        /// </summary>
        public void LogImportant(DebugMessage msg);

        /// <summary>
        /// Logs system message like initialization process.
        /// </summary>
        public void LogSystem(DebugMessage msg);

        /// <summary>
        /// Logs succesful message.
        /// </summary>
        public void LogSucces(DebugMessage msg);

        /// <summary>
        /// Logs warning message.
        /// </summary>
        public void LogWarning(DebugMessage msg);

        /// <summary>
        /// Logs error message.
        /// </summary>
        public void LogError(DebugMessage msg);

        /// <summary>
        /// Log critical error, witch forced app to close.
        /// </summary>
        public void LogCritical(DebugMessage msg);
    }
}
