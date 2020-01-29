using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Strategies
    {

        public static Func<OverlapPath, List<Overlap>, Overlap> GetBestOverlapByOverlapScore =
                    (path, neighbours) => {
                        return neighbours.OrderByDescending(n =>
                        {
                            return (path.ToSkip.Contains(n.TargetSeqName) || n.TargetStartCoord > n.QueryStartCoord) ? default : n.OverlapScore;
                        }).FirstOrDefault(n => path.ToSkip.Contains(n.TargetSeqName) == false);
                    };

        public static Func<OverlapPath, List<Overlap>, Overlap> GetBestOverlapByExtensionScore =
                    (path, neighbours) => {
                        return neighbours.OrderByDescending(n =>
                        {
                            return path.ToSkip.Contains(n.TargetSeqName) || n.TargetStartCoord > n.QueryStartCoord ? default : n.ExtensionScore1;
                        }).FirstOrDefault(n => path.ToSkip.Contains(n.TargetSeqName) == false);
                    };

        public static Func<OverlapPath, List<Overlap>, Overlap> GetBestOverlapByMonteCarlo =
                    (next, neighbours) => { 
                        var r = new Random();
                        double sum = neighbours.Sum(n => n.ExtensionScore1);
                        double selection = sum * r.NextDouble();

                        double soFar = 0;
                        return default;
                        Overlap selected = neighbours.SkipWhile(n => (soFar = n.ExtensionScore1) < selection).First();

                        return selected;
                    };
    }
}
