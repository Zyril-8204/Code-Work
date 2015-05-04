using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Thread_Join_Console_App
{
    class Program
    {
        public static Thread T1;
        public static Thread T2;

        static void Main(string[] args)
        {
            T1 = new Thread(new ThreadStart(First));
            T2 = new Thread(new ThreadStart(Second));
            T1.Name = "T1";
            T2.Name = "T2";
            T1.Start();
            T2.Start();
            Console.ReadLine();
        }

        // Thread T1 ThreadStart
        private static void First()
        {
            for (int i = 0; i<5; i++)
            {
                Console.WriteLine(
                    "T1 state[{0}], T1 showing {1}",
                    T1.ThreadState, i.ToString());
            }
        }

        // Thread T2 ThreadStart
        private static void Second()
        {
            // What is the state of both threads
            Console.WriteLine(
                "T2 state [{0}] just about to Join, T1 state[{1}], CurrentThreadName={2}",
                T2.ThreadState, T1.ThreadState,
                Thread.CurrentThread.Name);

            //join T1
            T1.Join();

            Console.WriteLine("T2 state[{0}] T2 just joined T1, T1 state[{1}], CurrentThreadName={2}",
                T2.ThreadState, T1.ThreadState,
                Thread.CurrentThread.Name);

            for (int i = 5; i < 10; i++)
            {
                Console.WriteLine(
                    "T2 state[{0}], T1 state[{1}], CurrentThreadName={2} showing {3}",
                    T2.ThreadState, T1.ThreadState,
                    Thread.CurrentThread.Name, i.ToString());
            }

            Console.WriteLine(
                "T2 state[{0}], T1 state[{1}], CurrentThreadName={2}",
                T2.ThreadState, T1.ThreadState,
                Thread.CurrentThread.Name);
        }

    }
}
