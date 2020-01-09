using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace herad
{
    public static class HeraMain
    {
        public static void Run()
        {
            (List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps) = SetupVariables();

            MainLogic(aSeqs, allOverlaps);
        }

        private static void Deserialize()
        {
            List<Seq> aSeqs;
            Dictionary<int, List<Overlap>> allOverlaps;

            string aSeqsJsonFileName = "aSeqsJson.txt";
            string allOverlapsFileName = "allOverlapsJson.txt";

            aSeqs = (List<Seq>)JsonConvert.DeserializeObject<List<Seq>>(File.ReadAllText(aSeqsJsonFileName));
            allOverlaps = JsonConvert.DeserializeObject<Dictionary<int, List<Overlap>>>(File.ReadAllText(allOverlapsFileName));

            MainLogic(aSeqs, allOverlaps);
        }

        private static void SerializeSetupObjects()
        {
            (List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps) = SetupVariables();
            string aSeqsJsonFileName = "aSeqsJson.txt";
            string allOverlapsFileName = "allOverlapsJson.txt";

            SerializeToFile(aSeqs, aSeqsJsonFileName);
            SerializeToFile(allOverlaps, allOverlapsFileName);
        }

        private static void SerializeToFile(object objectToSerialize, string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //serializer.NullValueHandling = NullValueHandling.Ignore;

            using (StreamWriter sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, objectToSerialize);
                // {"ExpiryDate":new Date(1230375600000),"Price":0}
            }
        }

        private static (List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps) SetupVariables()
        {
            var contigs = ToDict(GetDataFile("contig"));
            var aSeqs = contigs.Select(c => new Seq(c.Key, c.Value, SeqType.A)).ToList();
            var reads = ToDict(GetDataFile("reads"));
            List<Seq> rSeqs = reads.Select(r => new Seq(r.Key, r.Value, SeqType.R)).ToList();

            // The whole reference
            var refs = ToDict(GetDataFile("ref"));

            var paf = GetDataFile("read_to_contig").Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToContigOverlaps = PafToOverlap(paf);

            var readToReadPaf = GetDataFile("read_to_read").Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToReadOverlaps = PafToOverlap(readToReadPaf);

            var contigReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToContigOverlaps);
            var readsReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToReadOverlaps);

            var allOverlaps = readsReadsOverlapsDict.ToDictionary(d => d.Key, d => d.Value);
            contigReadsOverlapsDict.ToList().ForEach(p => { if (allOverlaps.ContainsKey(p.Key)) { allOverlaps[p.Key].AddRange(p.Value); } else { allOverlaps[p.Key] = p.Value.ToList(); } });

            return (aSeqs, allOverlaps);
        }

        private static void MainLogic(List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps)
        {

            var firstContig = int.Parse(aSeqs.First().Name.Substring(1 + "ctg".Length));

            var firstContigOverlaps = allOverlaps[firstContig];

            List<Path> pathsUsingOverlapScore   = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByOverlapScore);

            var byLen = pathsUsingOverlapScore.Select(o => (o.GetPathLength(), o)).OrderByDescending(p => p.Item1).ToList();

            var groupSizes = byLen.Count() / 100;
            var groups = new List<List<Path>>();
            for (int i = 0; i < byLen.Count; i += groupSizes)
            {
                groups.Add(new List<Path>());
                for (int j = 0; j < groupSizes && i + j < byLen.Count; j++)
                {
                    groups.Last().Add(byLen[i + j].Item2);
                }
            }

            List<Path> pathsUsingExtensionScore = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByExtensionScore);
            List<Path> pathsUsingMonteCarlo     = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByMonteCarlo);

            // TODO: Ovo bi trebali biti putevi dobiveni pomoću te 3 metode. Sad treba dalje odabrat najbolje i konstruirati graf samo s njima
        }

        private static List<Path> GetPathsUsingStrategy(
            Dictionary<int, List<Overlap>> overlapsDict,
            List<Overlap> firstContigOverlaps,
            Func<Path, List<Overlap>, Overlap> getBest
            )
        {
            var queueOfInterestingOverlaps = new Queue<Path>(firstContigOverlaps.Select(q => new Path(q)));

            Debug.Assert(queueOfInterestingOverlaps.First().ToSkip.Count() > overlapsDict.Max(o => o.Value.Count));

            List<Path> finalized = new List<Path>();

            while (queueOfInterestingOverlaps.Any())
            {
                Path nexty = queueOfInterestingOverlaps.Dequeue();
                var lastOver = nexty.Overlaps.Last();

                var neighbourOverlaps = overlapsDict[lastOver.TargetSeqCodename];
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
                if (best.TargetSeqCodename < 10) // TODO: Find better way to ensure the target is contig
                {
                    finalized.Add(nexty);
                    continue; // This path is complete and don't process it in the queue anymore
                }

                queueOfInterestingOverlaps.Enqueue(nexty);
            }

            return finalized;
        }

        private static Dictionary<int, List<Overlap>> CreateLookupDictOfOverlapsByName(IEnumerable<Overlap> readToReadOverlaps)
        {
            // Create a set of (left sequence name, overlap)
            IEnumerable<(int, Overlap)> readsReadsOverlapsPairs = 
                readToReadOverlaps.Select(r => (r.QuerySeqCodename, r))
                .Concat(readToReadOverlaps.Select(r => (r.TargetSeqCodename, r.GetFlipped())));

            // Create a lookup table for looking up using entering nodes
            Dictionary<int, List<Overlap>> readsReadsOverlapsDict = readsReadsOverlapsPairs.Select(s => s.Item1).Distinct().ToDictionary(s => s, s => new List<Overlap>());
            readsReadsOverlapsPairs.ToList().ForEach(p => readsReadsOverlapsDict[p.Item1].Add(p.Item2));

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
