using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace z1
{
    public interface ILogger
    {
        void Log(string Message);
    }

    public interface ILoggerFactoryWorker
    {
        ILogger Create(params object[] parameters);
    }



    public class ConsoleLogger : ILogger
    {
        private ConsoleLogger() { }
        public void Log(string msg)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {msg}");
        }

        public class LoggerCreator : ILoggerFactoryWorker
        {
            public ILogger Create(params object[] parameters)
            {
                return new ConsoleLogger();
            }
        }
    }

    public class FileLogger : ILogger
    {
        private string path;
        private FileLogger() { }
        private FileLogger(string path)
        {
            this.path = path;
            if (!File.Exists(path))
                File.OpenWrite(path).Close();
        }
        public void Log(string msg)
        {
            
            var f = File.AppendText(path);
            f.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {msg}");
            f.Close();
        }

        public class LoggerCreator : ILoggerFactoryWorker
        {
            public ILogger Create(params object[] parameters)
            {
                if (parameters.Length > 0 && parameters[0] is string)
                    return new FileLogger((string)parameters[0]);
                return new LoggerFactory.NullLogger();
            }
        }

    }

    public enum LogType { None, Console, File }

    public class LoggerFactory
    {
        private Dictionary<LogType, ILoggerFactoryWorker> workers = new Dictionary<LogType, ILoggerFactoryWorker>();

        public ILogger GetLogger(LogType logType, params object[] parameters)
        {
            if (workers.ContainsKey(logType))
                return workers[logType].Create(parameters);
            return new NullLogger();
        }

        public void Register(LogType logType, ILoggerFactoryWorker worker)
        {
            workers[logType] = worker;
        }

        private static LoggerFactory _instance;
        private static object _lock = new object();
        public static LoggerFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new LoggerFactory();
                            _instance.Register(LogType.Console, new ConsoleLogger.LoggerCreator());
                            _instance.Register(LogType.File, new FileLogger.LoggerCreator());
                        }
                    }
                }
                return _instance;
            }
        }

        public class NullLogger : ILogger
        {
            public void Log(string _)
            {

            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger1 = LoggerFactory.Instance.GetLogger(LogType.File, "foo.txt");
            logger1.Log("foo bar"); // logowanie do pliku
            ILogger logger2 = LoggerFactory.Instance.GetLogger(LogType.None);
            logger2.Log("qux"); // brak logowania
            Console.ReadKey();
        }
    }
}
