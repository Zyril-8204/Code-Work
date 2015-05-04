// A simple console app to show how to create AppDomains and display data
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppDomain_console_app
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain domainA = AppDomain.CreateDomain("MyDomainA");
            AppDomain domainB = AppDomain.CreateDomain("MyDomainB");
            domainA.SetData("DomainKey", "Domain A value");
            domainB.SetData("DomainKey", "Domain B value");
            OutputCall();
            domainA.DoCallBack(OutputCall);
            domainB.DoCallBack(OutputCall);
            Console.ReadLine();
        }

        public static void OutputCall()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Console.WriteLine("the value {0} was found in {1}, running on thread Id {2}",
                domain.GetData("DomainKey"), domain.FriendlyName,
                Thread.CurrentThread.ManagedThreadId.ToString());
        }
    }
}
