using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CallBack_Console_App
{
    class Program
    {
        private string message;
        private static Timer timer;
        private static bool complete;

        static void Main(string[] args)
        {
            Program p = new Program();
            Thread workerThread = new Thread(p.DoSomeWork);
            workerThread.Start();

            //create time with callback
            TimerCallback timerCallBack =
                new TimerCallback(p.GetState);
            timer = new Timer(timerCallBack, null,
                TimeSpan.Zero, TimeSpan.FromSeconds(2));

            // Wait for worker to complete
            do
            {
                // simply wait, do nothing
            } while (!complete);
        }

        public void GetState(object state)
        {
            // not done do retun
            if (message == string.Empty) return;
            Console.WriteLine("Worker is {0}", message);
            // is other thread completed yet, if so signal main
            // thread to stop waiting
            if (message == "Completed")
            {
                timer.Dispose();
                complete = true;
            }
        }

        public void DoSomeWork()
        {
            message = "processing";
            // simulate doing some work
            Thread.Sleep(3000);
            message = "Completed";
        }
    }
}
