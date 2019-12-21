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
    public class Path
    {
        public IEnumerable<Overlap> Overlaps => _overlaps;

        public IEnumerable<HashSet<Overlap>> LastSkips => _lastSkips;

        private List<Overlap> _overlaps;
        private List<HashSet<Overlap>> _lastSkips;

        public Path(Overlap initialOverlap)
        {
            this._overlaps = new List<Overlap> { initialOverlap };
            this._lastSkips = new List<HashSet<Overlap>> { new HashSet<Overlap>() };
        }

        public Overlap RemoveLastOverlapAndAddToSkipped()
        {
            var last = this._overlaps.Last();
            this._overlaps.RemoveAt(this._overlaps.Count - 1);
            this._lastSkips.RemoveAt(this._lastSkips.Count - 1);

            // Update the hashmap for the new last overlap's skip list
            // (new meaning the last last was removed so this is the new last)
            this.LastSkips.Last().Add(last);

            return last;
        }

        public void AddOverlap(Overlap o)
        {
            Debug.Assert(this.LastSkips.Last().Contains(o) == false && this._overlaps.Contains(o) == false);
            this._overlaps.Add(o);
            this._lastSkips.Add(new HashSet<Overlap> { });
        }
    }
}
