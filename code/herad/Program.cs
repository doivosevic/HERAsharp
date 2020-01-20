using System;
using System.Diagnostics;

namespace herad
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Stopwatch();
            s.Start();
            Console.WriteLine("Hello World!");

            HeraMain.Run();

            Console.WriteLine("Bye World!");
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds / 1000);
        }

    }
}
