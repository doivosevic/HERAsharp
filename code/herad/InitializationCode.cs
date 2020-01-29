using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace herad
{
    public static class InitializationCode
    {
        public static (List<Seq> aSeqs, Dictionary<string, List<Overlap>> allOverlaps) SetupVariables()
        {
            string[] readToContig = File.ReadAllLines(Settings.ReadToContigPath);
            string[] readToRead = File.ReadAllLines(Settings.ReadToReadPath);
            string[] readss = File.ReadAllLines(Settings.ReadsPath);
            string[] contigss = File.ReadAllLines(Settings.ContigsPath);

            readss = readss[0].StartsWith(">") == false ? readss : readss.Select(r => r.StartsWith(">") ? r.Substring(1) : r).ToArray();
            contigss = contigss[0].StartsWith(">") == false ? contigss : contigss.Select(r => r.StartsWith(">") ? r.Substring(1) : r).ToArray();

            var paf = readToContig.Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToContigOverlaps = PafToOverlap(paf);

            var readToReadPaf = readToRead.Select(s => s.Split('\t')).ToList();
            IEnumerable<Overlap> readToReadOverlaps = PafToOverlap(readToReadPaf).Where(o => o.OverlapScore * 1.0 / o.QuerySeqLen < 0.99 && o.OverlapScore * 1.0 / o.TargetSeqLen < 0.99).ToList();

            var contigReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToContigOverlaps);
            var readsReadsOverlapsDict = CreateLookupDictOfOverlapsByName(readToReadOverlaps);

            var allOverlaps = readsReadsOverlapsDict.ToDictionary(d => d.Key, d => d.Value);
            contigReadsOverlapsDict.ToList().ForEach(p => { if (allOverlaps.ContainsKey(p.Key)) { allOverlaps[p.Key].AddRange(p.Value); } else { allOverlaps[p.Key] = p.Value.ToList(); } });


            var reads = ToDict(readss);
            List<Seq> rSeqs = reads.Select(r => new Seq(r.Key, r.Value, SeqType.R)).ToList();

            var contigs = ToDict(contigss);
            List<Seq> aSeqs = contigs.Select(c => new Seq(c.Key, c.Value, SeqType.A)).ToList();

            var allSeqs = rSeqs.Concat(aSeqs).ToDictionary(s => s.Name, s => s);

            OverlapPath.AllSeqs = allSeqs;

            return (aSeqs, allOverlaps);
        }

        private static Dictionary<string, List<Overlap>> CreateLookupDictOfOverlapsByName(IEnumerable<Overlap> readToReadOverlaps)
        {
            // Create a set of (left sequence name, overlap)
            IEnumerable<(string, Overlap)> readsReadsOverlapsPairs =
                readToReadOverlaps.Select(r => (r.QuerySeqName, r))
                .Concat(readToReadOverlaps.Select(r => (r.TargetSeqName, r.GetFlipped())));

            // Create a lookup table for looking up using entering nodes
            Dictionary<string, List<Overlap>> readsReadsOverlapsDict = readsReadsOverlapsPairs.Select(s => s.Item1).Distinct().ToDictionary(s => s, s => new List<Overlap>());
            readsReadsOverlapsPairs.ToList().ForEach(p => readsReadsOverlapsDict[p.Item1].Add(p.Item2));

            return readsReadsOverlapsDict;
        }

        private static List<Overlap> PafToOverlap(List<string[]> paf)
        {
            // Mapping paf overlaps to overlap class constructor
            return paf.Select(p => new Overlap(p[0], IP(p[1]), IP(p[2]), IP(p[3]), p[4] == "+", p[5], IP(p[6]),
                IP(p[7]), IP(p[8]), IP(p[9]), IP(p[10]), IP(p[11]), p[12], p[13], p[14], p[15])).ToList();
        }

        private static string[] GetDataFile(string pattern)
        {
            var dataFolder = Directory.GetDirectories(Directory.GetCurrentDirectory()).First() + "\\ec_test2";
            var dataFiles = Directory.GetFiles(dataFolder);
            var filePath = dataFiles.FirstOrDefault(f => f.Contains(pattern));

            var contigs = File.ReadAllLines(filePath);
            return contigs;
        }

        private static Dictionary<string, string> ToDict(string[] contigs)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            for (int i = 0; i < contigs.Length; i += 2)
            {
                d.Add(contigs[i], contigs[i + 1]);
            }

            return d;
        }

        private static int IP(string s) => int.Parse(s);
    }
}
