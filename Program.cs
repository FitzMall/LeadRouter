using System;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;
using System.Threading;

namespace FMLeadRouter
{
    public class Program
    {
        public const string ThisServiceName = "FMLeadRouter";
        private static Thread _thread;

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = ThisServiceName;
            }

            protected override void OnStart(string[] args)
            {
                try
                {
                    Program.Start(args);
                }
                catch (Exception ex)
                {
                    //TODO
                    throw ex;
                }
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        static void Main(string[] args)
        {
            if (!Environment.UserInteractive)
            {
                //run as a service
                using (var service = new Service())
                {
                    ServiceBase.Run(service);
                    //Logger.LogEvent(new MsmqLog() { CreateDate = DateTime.Now, StyleId = 0000, Message = "Start Service", QueueName = "All Queues" });
                }
            }
            else
            {
                //Installer Code
                string parameter = String.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;

                }

                //run as a console app
                Start(args);
                Console.WriteLine("Press any key to stop...");
                //Logger.LogEvent(new MsmqLog() { CreateDate = DateTime.Now, StyleId = 0000, Message = "Start Console", QueueName = "All Queues" });
                Console.ReadKey(true);
            }
        }

        private static void Start(string[] args)
        {
            _thread = AddWatchingThread();
            //var leads = new S22Wrapper();
            //leads.Connect();
        }

        private static void Stop()
        {
            _thread.Abort();
            _thread.Join();
            var leads = new S22Wrapper();
            leads.Disconnect();
            Console.WriteLine("Shutting Down Service...");
        }

        private static void AsyncQueueWatcher(object name)
        {
            var listener = new S22Wrapper();
            listener.Connect();
            Thread.Sleep(2000);
        }

        /* Create an Async Thread */
        public static Thread AddWatchingThread()
        {
            var watcher = new Thread(AsyncQueueWatcher);
            watcher.Start();
            return watcher;
        }
    }
}
