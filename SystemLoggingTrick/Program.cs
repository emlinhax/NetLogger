using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SystemLoggingTrick
{
    internal class Program
    {
        public static string logFile = "System.Net.log";
        static void Main(string[] args)
        {
            if (File.Exists(logFile))
                File.Delete(logFile);

            Stream logFile = File.Create(logFile);

            //Get private logging class
            Assembly asm = typeof(System.Net.WebClient).Assembly;
            Type logging = asm.GetType("System.Net.Logging");

            //Create a new trace listener
            TraceListener listener = new TextWriterTraceListener(logFile);
            Trace.Listeners.Add(listener);

            //initialize logging for system.net and enable it
            logging.GetMethod("InitializeLogging", BindingFlags.NonPublic | BindingFlags.Static)
                .Invoke(null, new object[] { });

            logging.GetField("s_LoggingEnabled", BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic)
                .SetValue(null, true);

            //loop through each tracesource and add our listener (we have to do it with reflection because they are private)
            foreach (FieldInfo fi in logging.GetFields(BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic))
            {
                if (fi.FieldType.Name == "TraceSource")
                {
                    TraceSource ts = (TraceSource)fi.GetValue(null);
                    ts.Listeners.Add(listener);
                    ts.Switch.Level = SourceLevels.All;
                }
            }

            //Loading another assembly into our appdomain will make it use our already loaded logging class.
            //that will allow us to log the other assembly's activity without
            //having to modify it's source-code.
            if (args[0].Length > 1)
                Assembly.LoadFile(args[0]).EntryPoint.Invoke(null, new object[] { new string[] { } });
        }
    }
}
