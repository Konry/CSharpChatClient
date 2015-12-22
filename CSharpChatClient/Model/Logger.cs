using System;
using System.Diagnostics;
using System.IO;

namespace CSharpChatClient.Controller
{
    public static class Logger
    {
        /// <summary>
        /// LogState to filter the Messages to their informations
        /// From Trace, Info over Warning, Error to Fatal
        /// </summary>
        public enum LogState
        {
            TRACE = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3,
            FATAL = 4
        }

        private static StreamWriter writer;

        static Logger()
        {
            try
            {
                var globalStartTime = DateTime.Now;
                writer = File.AppendText("log" + globalStartTime.ToString("-yyyy-MM-dd") + ".txt");
                writer.Write("CSharpChat Tool Log: ");
                writer.WriteLine("  :");
                writer.WriteLine("-------------------------------");
                writer.AutoFlush = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Writer could not created.");
            }
        }

        /// <summary>
        /// Logs automatically a trace level message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogTrace(string logMessage)
        {
            Log(logMessage, LogState.TRACE);
        }

        /// <summary>
        /// Logs automatically an info level message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogInfo(string logMessage)
        {
            Log(logMessage, LogState.INFO);
        }

        /// <summary>
        /// Logs automatically a warning level message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogWarning(string logMessage)
        {
            Log(logMessage, LogState.WARNING);
        }

        /// <summary>
        /// Logs automatically an error level message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogError(string logMessage)
        {
            Log(logMessage, LogState.ERROR);
        }

        /// <summary>
        /// Logs automatically a fatal level message
        /// </summary>
        /// <param name="logMessage"></param>
        public static void LogFatal(string logMessage)
        {
            Log(logMessage, LogState.FATAL);
        }

        /// <summary>
        /// Logs directly in the from the logger created file
        /// Ignores Logs which are unter the LogState which is defined in the configuration.
        /// </summary>
        /// <param name="logMessage">The message to show in the log</param>
        /// <param name="state">The state to classify the information level</param>
        public static void Log(string logMessage, LogState state = LogState.INFO)
        {
            if (Configuration.logLevel <= state)
            {
                try
                {
                    string logLevel = string.Empty;
                    logLevel = GetStateString(state);
                    string temp = "\r\n" + DateTime.Now.ToString("s") + " \t- " + logLevel + " - " + logMessage;
                    Debug.WriteLine(temp);
                    writer.WriteLine(temp);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Writer could not created.");
                }
                //writer.FlushAsync();
            }
        }

        /// <summary>
        /// The logger for Exceptions, normally a caught exception is just a warning
        /// </summary>
        /// <param name="logMessage">A message to put before the exception text</param>
        /// <param name="exception">The exception to analyise</param>
        /// <param name="state">Optional, normal LogState.WARNING</param>
        public static void LogException(string logMessage, Exception exception, LogState state = LogState.WARNING)
        {
            Log(logMessage + "\r\n" + exception.Message + "\r\n\r\n" + exception.StackTrace + "\r\n", state);
        }

        /// <summary>
        /// Get the String for the enum of LogState
        /// </summary>
        /// <param name="state"></param>
        /// <returns>The string of the state</returns>
        private static string GetStateString(LogState state)
        {
            string logLevel;
            switch (state)
            {
                case LogState.INFO:
                    logLevel = "Info";
                    break;

                case LogState.TRACE:
                    logLevel = "Trace";
                    break;

                case LogState.WARNING:
                    logLevel = "Warning";
                    break;

                case LogState.ERROR:
                    logLevel = "Error";
                    break;

                case LogState.FATAL:
                    logLevel = "Fatal";
                    break;

                default:
                    logLevel = "DEFAULT";
                    break;
            }

            return logLevel;
        }
    }
}