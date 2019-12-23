using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Strategies
    {


        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByExtensionScore =
                    // TODO: Paziti da je uvijek ulazna strana query a nikad target da ne bi bilo zabune
                    (next, neighbours) =>
                            neighbours.OrderByDescending(n => n.ExtensionScore(true) * 1000 + n.Identity).FirstOrDefault(o => !next.ToSkip.Contains(o.TargetSeqName));

        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByOverlapScore =
                    (next, neighbours) =>
                            neighbours.OrderByDescending(n => n.OverlapScore * 1000 + n.Identity).FirstOrDefault(o => !next.ToSkip.Contains(o.TargetSeqName));

        public static Func<Path, List<Overlap>, Overlap> GetBestOverlapByMonteCarlo =
                    (next, neighbours) => {
                        var r = new Random();
                        double sum = neighbours.Sum(n => n.ExtensionScore(true));
                        double selection = sum * r.NextDouble();

                        double soFar = 0;
                        Overlap selected = neighbours.SkipWhile(n => (soFar = n.ExtensionScore(true)) < selection).First();

                        return selected;
                    };
    }
}
