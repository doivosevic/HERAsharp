using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Strategies
    {

        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByOverlapScore =
                    (path, neighbours) =>
                            neighbours.OrderByDescending(n => (path.ToSkip[n.TargetSeqCodename] || path.Overlaps.Last().TargetStartCoord > n.QueryStartCoord) ? 0 : n.OverlapScore).First();

        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByExtensionScore =
                    // TODO: Paziti da je uvijek ulazna strana query a nikad target da ne bi bilo zabune
                    (path, neighbours) =>
                            neighbours.OrderByDescending(n => path.ToSkip[n.TargetSeqCodename] ? 0 : n.ExtensionScore1).First();

        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByMonteCarlo =
                    (next, neighbours) => {
                        var r = new Random();
                        double sum = neighbours.Sum(n => n.ExtensionScore1);
                        double selection = sum * r.NextDouble();

                        double soFar = 0;
                        Overlap selected = neighbours.SkipWhile(n => (soFar = n.ExtensionScore1) < selection).First();

                        return selected;
                    };
    }
}
