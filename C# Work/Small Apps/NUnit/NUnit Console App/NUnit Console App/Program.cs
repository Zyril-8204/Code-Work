﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NUnit_Console_App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter Two Number\n");
            int number1, number2;
            number1 = int.Parse(Console.ReadLine());
            number2 = int.Parse(Console.ReadLine());

            MathsHelper helper = new MathsHelper();
            int x = helper.Add(number1, number2);
            Console.WriteLine("\nThe sum of " + number1 + " and " + number2 + " is " + x);
            Console.ReadKey();
            int y = helper.Subtract(number1, number2);
            Console.WriteLine("\nThe differene between " + number1 + " and " +number2 + " is " + y);
            Console.ReadKey();
        }
    }

    public class MathsHelper
    {
        public MathsHelper() { }
        public int Add(int a, int b)
        {
            int x = a + b;
            return x;
        }
        public int Subtract(int a, int b)
        {
            int x = a - b;
            return x;
        }
    }
}
