using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Interrupt_Console_App
{
    class Program
    {
        public static Thread sleeper;
        public static Thread waker;
        
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Main Method");
            sleeper = new Thread(new ThreadStart(PutThreadToSleep));
            waker = new Thread(new ThreadStart(WakeThread));
            sleeper.Start();
            waker.Start();
            Console.WriteLine("Exiting Main Method");
            Console.ReadLine();
        }

        // Thread sleeper ThreadStart
        private static void PutThreadToSleep()
        {
            for (int i = 0; i < 50; i++)
            {
                Console.Write(i + " ");
                if (i == 10 || i == 20 || i == 30)
                {
                    try
                    {
                        Console.WriteLine("Sleep, Going to sleep {0}",
                            i.ToString());
                        Thread.Sleep(20);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine("Foribly ");
                    }
                    Console.WriteLine("woken");
                }
            }
        }

        // Thread wake ThreadStart
        private static void WakeThread()
        {
            for (int i = 51; i < 100; i++)
            {
                Console.Write(i + " ");

                if (sleeper.ThreadState == ThreadState.WaitSleepJoin)
                {
                    Console.WriteLine("Interrupting sleeper");
                    sleeper.Interrupt();
                }
            }
        }
    }
}
