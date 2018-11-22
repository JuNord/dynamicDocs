using System;
using System.Runtime.InteropServices;
using RestService;

namespace WebServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("RUNNING");
            Host.Run();
            Console.WriteLine("STOPPED");
        }
    }
}