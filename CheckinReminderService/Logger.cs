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

                string logsDir = Path.Combine(baseDir, "Logs\\");
                if (!Directory.Exists(logsDir))
                    Directory.CreateDirectory(logsDir);

                string entry = $"{DateTime.Now:MM-dd-yyyy hh:mm:ss} - {log} : {title} - {message}"
                    + (log == LogType.ErrorLog && !string.IsNullOrEmpty(stkTrace) ? $" | Stack Trace: {stkTrace}" : "");

                string filePath = logsDir + DateTime.Now.ToString("MM-dd-yyyy") + ".txt";
                if (lastLogWriter == null || LastLogFileName != filePath)
                {
                    lastLogWriter?.Close();
                    FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                    lastLogWriter = new StreamWriter(fs) { AutoFlush = true };
                    LastLogFileName = filePath;
                }

                lastLogWriter.WriteLine(entry);
                Console.WriteLine(entry);
            }
        }
    }
}
