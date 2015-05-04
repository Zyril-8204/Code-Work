using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tread_Console_App
{
    class Program
    {
        static void Main(string[] args)
        {
            // no paramaters
            Thread workerThread = new Thread(StartThread);
            Console.WriteLine("Main Thread Id {0}", 
                Thread.CurrentThread.ManagedThreadId.ToString());
            workerThread.Start();

            // using parameter
            Thread workerThread2 = new Thread(ParameterizedStartThread);
            // the anser to life, the universe, and everything is?
            workerThread2.Start(42);
            Console.ReadLine();
        }

        public static void StartThread()
        {
            for (int i =0; i<10; i++)
            {
                Console.WriteLine("Thread value {0} running on Thread Id {1}",
                    i.ToString(), Thread.CurrentThread.ManagedThreadId.ToString());
            }
        }

        public static void ParameterizedStartThread(object value)
        {
            Console.WriteLine("Thread passed value{0} running on Thread{1}",
                value.ToString(),
                Thread.CurrentThread.ManagedThreadId.ToString());
        }
    }
}
