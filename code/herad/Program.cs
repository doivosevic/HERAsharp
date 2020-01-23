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

            //HeraMain.Run(args[1], args[2], args[3], args[4]);

            string folder = @"C:\git\HERAsharp\code\data\ec_test2";
            string p1 = "ecoli_test_reads_1.fasta";
            string p2 = "ecoli_test_contigs.fasta";
            string p3 = "read_to_read.paf";
            string p4 = "read_to_contig.paf";

            HeraMain.Run(readsPath: Path.Combine(folder, p1), contigsPath: Path.Combine(folder, p2), readToReadPath: Path.Combine(folder, p3), readToContigPath: Path.Combine(folder, p4));

            Console.WriteLine("Bye World!");
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds / 1000);
        }

    }
}
