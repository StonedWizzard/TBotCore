using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Debug
{
    /// <summary>
    /// Basic debug message. 
    /// Contains debug info and overrides ToString() method
    /// </summary>
    public class DebugMessage
    {
        /// <summary>
        /// DateTime when message was created
        /// </summary>
        public DateTime Time { get; protected set; }
        /// <summary>
        /// Basic error message info
        /// </summary>
        public string Message { get; protected set; }
        /// <summary>
        /// Place/Stage/etc. where error occured
        /// </summary>
        public string Location { get; protected set; }
        /// <summary>
        /// Additional info
        /// </summary>
        public string AddInfo { get; protected set; }


        public DebugMessage(string msg)
        {
            Message = msg;
            Time = DateTime.Now;
        }
        public DebugMessage(string msg, string location) : this(msg)
        {
            Location = "\r\nLocation ->" + (String.IsNullOrEmpty(location) ? "[???]" : $"[{location}]");
        }
        public DebugMessage(string msg, string location, string addInfo) : this (msg, location)
        {
            AddInfo = $"<<Additional information>>\r\n" + addInfo;
        }
        public DebugMessage(string msg, string location, Exception addInfo) : this(msg, location)
        {
            if (addInfo != null)
            {
                AddInfo = $"<<Additional information>>\r\n" +
                    $"Exception: {addInfo.Message}\r\nInnerExeption: {addInfo.InnerException}\r\n" +
                    $"Source: {addInfo.Source}\r\n->TargetSite: {addInfo.TargetSite}\r\n" +
                    $"Stack trace:\r\n{addInfo.StackTrace}\r\n";
            }
        }

        public override string ToString()
        {
            return $"[{Time}] {Message}\r\n{Location}\r\n{AddInfo}";
        }
    }
}
