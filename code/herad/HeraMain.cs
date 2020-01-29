using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace herad
{
    public static class HeraMain
    {
        public static string Run()
        {
            (List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps) = InitializationCode.SetupVariables();

            return MainLogic(aSeqs, allOverlaps);
        }

        private static string MainLogic(List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps)
        {
            // GENERATION OF CONSENSUS SEQUENCES
            List<(int, int, List<Path>, Path)> listOfConsensuses = GetConsensusSequences(aSeqs, allOverlaps);
            Console.WriteLine("Got consensuses");

            // CONSTRUCTION OF OVERLAP GRAPH
            List<(int, int)> graph = GetConnectionGraph(listOfConsensuses);
            Console.WriteLine("Got connection graph");

            // FINAL SEQUENCE ASSEMBLY
            List<Overlap> completePath = GetFinalPathOverlaps(listOfConsensuses, graph);

            Console.WriteLine("Got final path overlaps");

            Console.WriteLine(string.Join("", completePath.Select(t => "(" + t.QuerySeqName + "," + t.TargetSeqName + ")")));

            string final = BuildSequenceFromOverlaps(completePath);
            return final;
        }

        private static string BuildSequenceFromOverlaps(List<Overlap> completePath)
        {
            StringBuilder sb = new StringBuilder();
            bool firstStrand = true;

            // QUERY 0 ITERATOR
            int q0It = 0;

            // OVERLAP END IT
            int oEIt = 0;

            for (int i = 0; i < completePath.Count; ++i)
            {
                var ol = completePath[i];
                var leftSeq = Path.AllSeqs[ol.QuerySeqName];

                var queryStart = ol.QueryStartCoord;
                var targetStart = ol.TargetStartCoord;
                var queryEnd = ol.QueryEndCoord;
                var sameStrand = ol.SameStrand;
                //var targetEnd = ol.TargetEndCoord;

                if (!sameStrand && !firstStrand)
                {
                    queryStart = ol.QuerySeqLen - ol.QueryEndCoord;
                    queryEnd = ol.QuerySeqLen - ol.QueryStartCoord;
                }else if (sameStrand && !firstStrand) {

                    queryStart = ol.QuerySeqLen - ol.QueryEndCoord;
                    queryEnd = ol.QuerySeqLen - ol.QueryStartCoord;
                    targetStart = ol.TargetSeqLen - ol.TargetEndCoord;
                }
                else if (!sameStrand && firstStrand)
                {
                    targetStart = ol.TargetSeqLen - ol.TargetEndCoord;
                }

                int startDiff = queryStart - targetStart;


                //if (ol.SameStrand && !firstStrand)
                //{
                //    startDiff = -startDiff;
                //    ol = ol.GetFlipped();
                //}


                var absQEnd = q0It + queryEnd;

                if (absQEnd > oEIt)
                {
                    int addedLen = absQEnd - oEIt;
                    int posOnLeftStrand = oEIt - q0It;
                    var flipped = leftSeq.Content.Flip(firstStrand);
                    sb.Append(flipped.Substring(posOnLeftStrand, addedLen));
                    oEIt = absQEnd;
                }

                if (sameStrand == false) firstStrand = !firstStrand;
                q0It += startDiff;
            }

            var lastOl = completePath.Last();
            var lastTargetStart = q0It + lastOl.QueryStartCoord - lastOl.TargetStartCoord;
            var lastTargetTotalEnd = lastTargetStart + lastOl.TargetSeqLen;
            if (lastTargetTotalEnd - oEIt > 0)
            {
                var rightSeq = Path.AllSeqs[lastOl.TargetSeqName];
                sb.Append(rightSeq.Content.Substring(lastOl.TargetEndCoord));
            }

            string final = sb.ToString();
            return final;
        }

        private static List<(int, int, List<Path>, Path)> GetConsensusSequences(List<Seq> aSeqs, Dictionary<int, List<Overlap>> allOverlaps)
        {
            List<(int, int, List<Path>, Path)> listOfConsensuses = new List<(int, int, List<Path>, Path)>();

            var options = new ParallelOptions();
            options.MaxDegreeOfParallelism = 1;

            Parallel.ForEach(aSeqs, options, ctg =>
            {
                var ctgName = int.Parse(ctg.Name.Substring("ctg".Length));

                var firstContigOverlaps = allOverlaps[ctgName];

                // FINDING PATHS BETWEEN ANCHORING NODES IN HERA
                List<Path> pathsUsingOverlapScore = GetPathsUsingStrategy(allOverlaps, firstContigOverlaps, Strategies.GetBestOverlapByOverlapScore);

                List<(int Key, List<Path>)> byEndContig = pathsUsingOverlapScore.GroupBy(p => p.Overlaps.Last().TargetSeqCodename).Select(g => (g.Key, g.ToList())).Where(g => g.Key != ctgName).ToList();

                foreach (var keyCtg in byEndContig)
                {
                    var consensus = GetConsensusSequenceAndItsGroup(keyCtg.Item2);

                    listOfConsensuses.Add((ctgName, keyCtg.Key, consensus.Item2, consensus.Item1));
                }
            });

            return listOfConsensuses;
        }

        private static List<Overlap> GetFinalPathOverlaps(List<(int, int, List<Path>, Path)> listOfConsensuses, List<(int, int)> graph)
        {
            int firstCtg = graph.First().Item1;
            int realFirst = firstCtg;

            while (true)
            {
                var newFirst = graph.FirstOrDefault(n => n.Item2 == firstCtg);
                if (newFirst == default || realFirst == newFirst.Item1) break;
                firstCtg = newFirst.Item1;
            }

            var ordered = new List<int> { firstCtg };

            while (true)
            {
                var next = graph.FirstOrDefault(n => n.Item1 == ordered.Last());
                if (next == default || next.Item2 == firstCtg) break;
                ordered.Add(next.Item2);
            }

            List<(int, int, List<Path>, Path)> orderedPairs = ordered.SkipLast(1).Zip(ordered.Skip(1)).Select(p => listOfConsensuses.First(c => c.Item1 == p.First && c.Item2 == p.Second)).ToList();

            List<Overlap> completePath = new List<Overlap>();

            foreach (var c in orderedPairs)
            {
                completePath.AddRange(c.Item4.Overlaps);
            }

            return completePath;
        }

        private static List<(int, int)> GetConnectionGraph(List<(int, int, List<Path>, Path)> listOfConsensuses)
        {
            var graph = new List<(int, int)>();

            foreach ((int leftCtg, int rightCtg, List<Path> paths, Path conensus) in listOfConsensuses.OrderByDescending(g => g.Item3.Count))
            {

                if (graph.Contains((leftCtg, rightCtg)) || graph.Contains((leftCtg, rightCtg))) continue;
                if (leftCtg == rightCtg) continue;

                bool areConnected = false;
                var connected = new HashSet<int> { leftCtg };
                foreach (var node in graph)
                {
                    if (connected.Contains(node.Item1) && node.Item2 == rightCtg || connected.Contains(node.Item2) && node.Item1 == rightCtg)
                    {
                        areConnected = true;
                        break;
                    }

                    if (connected.Contains(node.Item1) && !connected.Contains(node.Item2)) connected.Add(node.Item2);
                    if (connected.Contains(node.Item2) && !connected.Contains(node.Item1)) connected.Add(node.Item1);
                }

                if (areConnected) continue;

                if (graph.Any(n => n.Item1 == leftCtg || n.Item2 == rightCtg)) continue;

                graph.Add((leftCtg, rightCtg));
            }

            return graph;
        }

        private static (Path, List<Path>) GetConsensusSequenceAndItsGroup(List<Path> pathsUsingOverlapScore)
        {
            var byLen = pathsUsingOverlapScore.Select(o => (o.GetPathLength(), o)).OrderByDescending(p => p.Item1).ToList();

            var count = byLen.Count();
            var groupSizes = count < 1000 ? 1 : count/100;
            var groups = new List<List<Path>>();
            for (int i = 0; i < byLen.Count; i += groupSizes)
            {
                groups.Add(new List<Path>());
                for (int j = 0; j < groupSizes && i + j < byLen.Count; j++)
                {
                    groups.Last().Add(byLen[i + j].Item2);
                }
            }

            var by1000 = byLen.GroupBy(lp => lp.Item1 / (1 * 1000 * 1000)).Select(g => (g.Count(), g.ToList())).ToList();

            (int, List<(int, Path o)>) bestGroup = by1000.OrderByDescending(g => g.Item1).First();

            int indexOfBest = by1000.IndexOf(bestGroup);


            var inBestByScore = bestGroup.Item2.Select(p => (p.o.Overlaps.Sum(oo => oo.OverlapScore) / p.o.Overlaps.Count(), p.o)).OrderByDescending(p => p.Item1).ToList();
            var consensusSequence = inBestByScore.First().o;
            return (consensusSequence, bestGroup.Item2.Select(g => g.o).ToList());
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
                    queueOfInterestingOverlaps.Enqueue(nexty);
                    continue;
                }

                nexty.ToSkip[best.TargetSeqCodename] = true;
                nexty.AddOverlap(best);

                // If best option is to connect the read to contig
                if (best.TargetSeqCodename < 10) // TODO: Find better way to ensure the target is contig
                {
                    finalized.Add(nexty);
                    continue; // This path is complete and don't process it in the queue anymore
                }

                queueOfInterestingOverlaps.Enqueue(nexty);
            }

            return finalized.Where(p => p.Length > 2).ToList();
        }
    }
}
