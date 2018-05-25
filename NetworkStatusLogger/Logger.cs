using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace NetworkStatusLogger
{
    class Logger
    {
        private TextWriter textWriter;

        public Logger(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            if (!Directory.Exists(fileInfo.DirectoryName)) Directory.CreateDirectory(fileInfo.DirectoryName);

                textWriter = new StreamWriter(path, true);
        }

        public void logState(string msg,status state)
        {
            string logmsg = null;
            logmsg += DateTime.Now.ToString("yyyy MM dd hh:mm:ss");
            logmsg += state == status.Connect ? " [Connected]".PadRight(15):" [DisConnected]".PadRight(15);
            logmsg += msg;
            textWriter.WriteLine(logmsg);
            textWriter.Flush();
        }

        public void writeText(string msg)
        {
            string logmsg = null;
            logmsg += DateTime.Now.ToString("yyyy MM dd hh:mm:ss");
            logmsg += msg;
            textWriter.WriteLine(msg);
            textWriter.Flush();
        }
        public enum status
        {
            Connect,
            DisConnect,
        }
    }
}
