namespace Logger
{
    public class Logger
    {
        public static readonly object lockObj = new object();

        public enum LogType
        {
            InformationLog,
            ErrorLog
        }

        static string? LastLogFileName = null;
        static StreamWriter? lastLogWriter = null;

        public static void Write(string title, string message, string stkTrace, LogType log)
        {
            lock (lockObj)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;

                string errorDir = Path.Combine(baseDir, "Error\\");
                if (!Directory.Exists(errorDir))
                    Directory.CreateDirectory(errorDir);

                string logsDir = Path.Combine(baseDir, "Logs\\");
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);

                FileStream? fs = null;
                StreamWriter? logger = null;

                if (log == LogType.ErrorLog)
                {
                    fs = new FileStream(errorDir + DateTime.Now.ToString("MM-dd-yyyy") + ".txt", FileMode.Append, FileAccess.Write, FileShare.Write);
                    logger = new StreamWriter(fs);
                }
                else
                {
                    string filePath = logsDir + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
                    if (lastLogWriter != null && LastLogFileName == filePath)
                    {
                        logger = lastLogWriter;
                    }
                    else
                    {
                        lastLogWriter?.Close();
                        fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        logger = new StreamWriter(fs) { AutoFlush = true };
                        LastLogFileName = filePath;
                        lastLogWriter = logger;
                    }
                }

                string entry = $"{DateTime.Now:MM-dd-yyyy hh:mm:ss} - {log} : {title} - {message}"
                    + (log == LogType.ErrorLog && !string.IsNullOrEmpty(stkTrace) ? $" | Stack Trace: {stkTrace}" : "");

                logger?.WriteLine(entry);
                Console.WriteLine(entry);

                if (log == LogType.ErrorLog && logger != null && fs != null)
                {
                    logger.WriteLine("Error: " + stkTrace);
                    Console.WriteLine("Error: " + stkTrace);
                    logger.Close();
                    fs.Close();
                }
            }
        }
    }
}
