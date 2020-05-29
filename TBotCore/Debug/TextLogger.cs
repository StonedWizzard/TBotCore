using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBotCore.Debug
{
    /// <summary>
    /// Default debug messages listener.
    /// Save all messages to text file
    /// </summary>
    sealed public class TextLogger : IDebuger
    {
        /// <summary>
        /// Folder, where saves logs.
        /// If property null - set default value "Logs"
        /// </summary>
        public readonly string DirectoryName;
        /// <summary>
        /// File name, where writes log.
        /// Sets automaticly by current dateTime (format is "dd-MM-yyyy_HHmmss.txt")
        /// </summary>
        public readonly string FileName;
        /// <summary>
        /// Relaitive path.
        /// Builds from DirName and FileName
        /// </summary>
        public readonly string FilePath;

        private readonly StreamWriter DebugStream;

        public TextLogger() : this(null) { }

        public TextLogger(string logFolder)
        {
            DirectoryName = String.IsNullOrEmpty(logFolder) ? "Logs" : logFolder;
            FileName = DateTime.Now.ToString("dd-MM-yyyy_HHmmss") + ".txt";
            FilePath = $"{DirectoryName}\\{FileName}";
            Directory.CreateDirectory(DirectoryName);
            DebugStream = File.CreateText(FilePath);
            WriteMessage(new DebugMessage("-=Text logger initialized!=-"));
        }

        ~TextLogger()
        {
            if (DebugStream != null)
            {
                WriteMessage(new DebugMessage("-=Text logger disposed!=-"));
                DebugStream.Close();
                DebugStream.Dispose();
            }
        }


        private void WriteMessage(DebugMessage msg)
        {
            DebugStream.WriteLine(msg.ToString());
            DebugStream.Flush();
        }

        public void LogCritical(DebugMessage msg) { WriteMessage(msg); }

        public void LogError(DebugMessage msg) { WriteMessage(msg); }

        public void LogImportant(DebugMessage msg) { WriteMessage(msg); }

        public void LogMessage(DebugMessage msg) { WriteMessage(msg); }

        public void LogSucces(DebugMessage msg) { WriteMessage(msg); }

        public void LogSystem(DebugMessage msg) { WriteMessage(msg); }

        public void LogWarning(DebugMessage msg) { WriteMessage(msg); }
    }
}
