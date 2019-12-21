using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace herad
{
    public static class HeraMain
    {
        public static void Run()
        {
            List<Seq> aSeqs;
            Dictionary<string, List<Overlap>> contigReadsOverlapsDict, readsReadsOverlapsDict;

            SetupVariables(out aSeqs, out contigReadsOverlapsDict, out readsReadsOverlapsDict);
            MainLogic(aSeqs, contigReadsOverlapsDict, readsReadsOverlapsDict);
        }

        private static void SetupVariables(out List<Seq> aSeqs, out Dictionary<string, List<Overlap>> contigReadsOverlapsDict, out Dictionary<string, List<Overlap>> readsReadsOverlapsDict)
        {
            var contigs = ToDict(GetDataFile("contig"));
            aSeqs = contigs.Select(c => new Seq(c.Key, c.Value, SeqType.A)).ToList();
            var reads = ToDict(GetDataFile("reads"));
            List<Seq> rSeqs = reads.Select(r => new Seq(r.Key, r.Value, SeqType.R)).ToList();

            // The whole reference
            var refs = ToDict(GetDataFile("ref"));

            var paf = GetDataFile("read_to_contig").Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToContigOverlaps = PafToOverlap(paf);

            var readToReadPaf = GetDataFile("read_to_read").Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToReadOverlaps = PafToOverlap(readToReadPaf);

            readToContigOverlaps.ToList();

            var pafNames = paf.Select(p => (p[5] + p[0], p)).OrderBy(s => s.Item1).ToList();

            var refss = refs.First().Value;
            var readss = reads.Values;
            var contigss = contigs.Values;

            var ccc = readss.Select(i => refss.Contains(i));

            var count = ccc.Count(i => i == true);

            var c2 = contigss.Select(c123 => refss.Contains(c123));


            contigReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToContigOverlaps);
            readsReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToReadOverlaps);
        }

        private static void MainLogic(List<Seq> aSeqs, Dictionary<string, List<Overlap>> contigReadsOverlapsDict, Dictionary<string, List<Overlap>> readsReadsOverlapsDict)
        {
            var firstContig = aSeqs.First().Name;
            var firstContigOverlaps = contigReadsOverlapsDict[firstContig];


            Dictionary<string, List<Overlap>> allOverlaps = readsReadsOverlapsDict.Concat(contigReadsOverlapsDict).ToDictionary(d => d.Key, d => d.Value);

            List<Path> pathsUsingOverlapScore   = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByOverlapScore);
            List<Path> pathsUsingExtensionScore = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByExtensionScore);
            List<Path> pathsUsingMonteCarlo     = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByMonteCarlo);

            // TODO: Ovo bi trebali biti putevi dobiveni pomoću te 3 metode. Sad treba dalje odabrat najbolje i konstruirati graf samo s njima
        }

        private static List<Path> GetPathsUsingStrategy(
            Dictionary<string, List<Overlap>> overlapsDict,
            List<Overlap> firstContigOverlaps,
            Func<Path, List<Overlap>, Overlap> getBest
            )
        {
            var queueOfInterestingOverlaps = new Queue<Path>(firstContigOverlaps.Select(q => new Path(q)));

            List<Path> finalized = new List<Path>();

            while (queueOfInterestingOverlaps.Any())
            {
                Path nexty = queueOfInterestingOverlaps.Dequeue();
                var lastOver = nexty.Overlaps.Last();

                var neighbourOverlaps = overlapsDict[lastOver.TargetSeqName];
                // This time by overlap
                // Get first overlap which isn't already in NEXT
                // Order by overlap score and tie break on sequence identity
                Overlap best = getBest(nexty, neighbourOverlaps);

                // In case no valid next overlaps, backtrack
                if (best == default(Overlap))
                {
                    // If path only has 1 element, stop considering that path
                    if (nexty.Overlaps.Count() <= 1)
                    {
                        continue;
                    }

                    nexty.RemoveLastOverlapAndAddToSkipped();
                }

                nexty.AddOverlap(best);

                // If best option is to connect the read to contig
                if (best.TargetSeqName.Contains("ctg")) // TODO: Find better way to ensure the target is contig
                {
                    finalized.Add(nexty);
                    continue; // This path is complete and don't process it in the queue anymore
                }

                queueOfInterestingOverlaps.Enqueue(nexty);
            }

            return finalized;
        }

        private static Dictionary<string, List<Overlap>> CreateLookupDictOfOverlapsByName(IEnumerable<Overlap> readToReadOverlaps)
        {
            // Create a set of (left sequence name, overlap)
            IEnumerable<(string, Overlap)> readsReadsOverlapsPairs = 
                readToReadOverlaps.Select(r => (r.QuerySeqName, r))
                .Concat(readToReadOverlaps.Select(r => (r.TargetSeqName, r.GetFlipped())));

            // Create a lookup table for looking up using entering nodes
            Dictionary<string, List<Overlap>> readsReadsOverlapsDict = readsReadsOverlapsPairs.ToDictionary(r => r.Item1, r => new List<Overlap> { r.Item2 });

            return readsReadsOverlapsDict;
        }

        private static IEnumerable<Overlap> PafToOverlap(List<string[]> paf)
        {
            // Mapping paf overlaps to overlap class constructor
            return paf.Select(p => new Overlap(p[0], IP(p[1]), IP(p[2]), IP(p[3]), p[4] == "+", p[5], IP(p[6]),
                IP(p[7]), IP(p[8]), IP(p[9]), IP(p[10]), IP(p[11]), p[12], p[13], p[14], p[15]));
        }

        private static string[] GetDataFile(string what)
        {
            var dataFolder = Directory.GetDirectories(Directory.GetCurrentDirectory()).First();
            var dataFiles = Directory.GetFiles(dataFolder);

            var contigs = File.ReadAllLines(dataFiles.FirstOrDefault(f => f.Contains(what)));
            return contigs;
        }

        private static int IP(string s) => int.Parse(s);

        private static Dictionary<string, string> ToDict(string[] contigs)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            for (int i = 0; i < contigs.Length; i += 2)
            {
                d.Add(contigs[i], contigs[i + 1]);
            }

            return d;
        }
    }
}
