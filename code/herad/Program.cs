using System;
using System.Diagnostics;
using System.IO;

namespace herad
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new Stopwatch();
            s.Start();

            Console.WriteLine("Hello World!");
            Console.WriteLine(string.Join(",", args));

            if (args.Length != 1 || !Settings.FileToSettings(args[0]))
            {
                Console.WriteLine("Invalid settings file. Using default values");

                Settings.Folder = @"C:\git\HERAsharp\code\data\ec_test2";
                Settings.ReadsPath = "reads.fasta";
                Settings.ContigsPath = "contigs.fasta";
                Settings.ReadToReadPath = "read_to_read.paf";
                Settings.ReadToContigPath = "read_to_contig.paf";
                Settings.ResultingFileName = "complete.fasta";
            }

            var result = HeraMain.Run();

            File.WriteAllText(Settings.ResultingFileName, ">finalsequence" + Environment.NewLine + result);

            Console.WriteLine("Bye World!");
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds / 1000);


        }
    }
}
