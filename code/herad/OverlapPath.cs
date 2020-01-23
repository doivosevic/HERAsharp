using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace herad
{
    /// <summary>
    /// Overlap path containing list of overlaps and backtracks
    /// </summary>
    [DebuggerDisplay("P {Length}")]
    public class OverlapPath
    {
        public IEnumerable<Overlap> Overlaps => _overlaps;

        //public IEnumerable<HashSet<Overlap>> LastSkips => _lastSkips;

        public int Length => this._overlaps.Count;

        public HashSet<string> ToSkip;

        private List<Overlap> _overlaps;
        internal static Dictionary<string, Seq> AllSeqs;

        //private List<HashSet<Overlap>> _lastSkips;

        public OverlapPath(Overlap initialOverlap)
        {
            this._overlaps = new List<Overlap> { initialOverlap };
            //this._lastSkips = new List<HashSet<Overlap>> { new HashSet<Overlap>() };
            this.ToSkip = new HashSet<string>();
            //new HashSet<int> { initialOverlap.QuerySeqCodename, initialOverlap.TargetSeqCodename };
            this.ToSkip.Add(initialOverlap.QuerySeqName);
            this.ToSkip.Add(initialOverlap.TargetSeqName);
        }

        public int GetPathLength()
        {
            int len = this._overlaps[0].NumMatching;

            for (int i = 1; i < this._overlaps.Count; i++)
            {
                var last = this._overlaps[i - 1];
                var now = this._overlaps[i];

                // Overlap of overlaps calculating logic
                //  | |
                //  V V

                if (last.TargetStartCoord < now.QueryStartCoord && last.TargetEndCoord > now.QueryEndCoord)
                {
                    //Skip because inside
                }
                else if (last.TargetStartCoord < now.QueryStartCoord && last.TargetEndCoord < now.QueryEndCoord)
                {
                    if (last.TargetEndCoord > now.QueryStartCoord)
                    {
                        // No overlap between overlaps
                        len += now.NumMatching;
                    }
                    else
                    {
                        len += (now.QueryEndCoord - last.QueryEndCoord);
                    }
                }
                else if (last.TargetStartCoord > now.QueryStartCoord && last.TargetEndCoord > now.QueryStartCoord)
                {
                    if (last.TargetStartCoord > now.QueryEndCoord)
                    {
                        // No overlap between overlaps
                        len += now.NumMatching;
                    }
                    else
                    {
                        len += (last.TargetStartCoord - now.QueryStartCoord);
                    }
                }
                else
                {
                    // This overlap consumes the last overlap
                    len += now.NumMatching - last.NumMatching;
                }
            }

            return len;
        }

        public string AssembleSequence()
        {
            StringBuilder sb = new StringBuilder();

            var firstOverlap = this._overlaps.First();
            var firstRead = AllSeqs[firstOverlap.TargetSeqName];
            sb.Append(firstRead.Content.Substring(firstOverlap.QueryEndCoord));

            for (int i = 0; i < this.Length - 1; i++)
            {
                var overlap = this._overlaps[i];
                var leftPath = AllSeqs[overlap.QuerySeqName];
                var rightPath = AllSeqs[overlap.TargetSeqName];

                var nextOverlap = this._overlaps[i + 1];
                var nextLeft = AllSeqs[nextOverlap.QuerySeqName];
                var nextRight = AllSeqs[nextOverlap.TargetSeqName];

                Debug.Assert(rightPath == nextLeft);

                var newAddendum = nextLeft.Content.Substring(overlap.TargetEndCoord, nextOverlap.TargetEndCoord - overlap.TargetEndCoord);
                sb.Append(newAddendum);
            }

            return sb.ToString();
        }

        public Overlap RemoveLastOverlapAndAddToSkipped()
        {
            var last = this._overlaps.Last();
            this._overlaps.RemoveAt(this._overlaps.Count - 1);
            //this._lastSkips.RemoveAt(this._lastSkips.Count - 1);

            // Update the hashmap for the new last overlap's skip list
            // (new meaning the last last was removed so this is the new last)
            //this.LastSkips.Last().Add(last);
            //this.ToSkip.Add(last.TargetSeqName);

            return last;
        }

        public void AddOverlap(Overlap o)
        {
            //Debug.Assert(this.LastSkips.Last().Contains(o) == false && this._overlaps.Contains(o) == false);
            this._overlaps.Add(o);
            this.ToSkip.Add(o.TargetSeqName);
            //this._lastSkips.Add(new HashSet<Overlap> { });
        }
    }
}
