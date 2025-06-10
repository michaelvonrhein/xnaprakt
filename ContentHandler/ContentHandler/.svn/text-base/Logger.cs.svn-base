using System;
using System.Collections.Generic;
using System.Text;

namespace PraktWS0708.ContentHandler
{
    static class Logger
    {
        private static System.IO.StreamWriter logWriter = null;

        public static void LogIt(String message)
        {
            if (logWriter != null)
            {
                logWriter.WriteLine(message);
            }
        }

        public static void NewLine()
        {
            if (logWriter != null)
            {
                logWriter.WriteLine();
            }
        }

        public static void StartLog(String filename)
        {
            logWriter = new System.IO.StreamWriter(filename);
        }

        public static void EndLog()
        {
            if (logWriter != null)
            {
                logWriter.Close();
            }
        }
    }
}
