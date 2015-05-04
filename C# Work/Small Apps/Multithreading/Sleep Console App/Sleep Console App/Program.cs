using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sleep_Console_App
{
    class Program
    {
        public static Thread T1;
        public static Thread T2;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter Main Method");

            T1 = new Thread(new ThreadStart(Count1));
            T2 = new Thread(new ThreadStart(Count2));
            T1.Start();
            T2.Start();
            Console.WriteLine("Exit Main Method");
            Console.ReadLine();
        }

        // Thread T1 ThreadStart
        private static void Count1()
        {
            Console.WriteLine("Enter T1 Counter");
            for (int i = 0; i <50; i++)
            {
                Console.Write(i + " ");
                if (i == 10)
                {
                    Thread.Sleep(1000);
                }
            }
            Console.WriteLine("Exit T1 Counter");
        }

        // Thread T2 ThreadStart
        private static void Count2()
        {
            Console.WriteLine("Enter T2 Counter");
            for (int i = 51; i < 100; i++)
            {
                Console.Write(i + " ");
                if (i == 70)
                {
                    Thread.Sleep(5000);
                }
            }
            Console.WriteLine("Exit T2 Counter");
        }
    }
}
