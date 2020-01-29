using System;
using System.Diagnostics;

namespace herad
{
    [DebuggerDisplay("OL {QuerySeqName} {TargetSeqName} qs:{QueryStartCoord} qe:{QueryEndCoord}")]
    // https://lh3.github.io/minimap2/minimap2.html#10
    public class Overlap
    {
        // 1	string	Query sequence name
        public string QuerySeqName;

        public int QuerySeqCodename;

        // 2	int	Query sequence length
        public int QuerySeqLen;

        // 3	int	Query start coordinate (0-based)
        public int QueryStartCoord;

        // 4	int	Query end coordinate (0-based)
        public int QueryEndCoord;

        // 5	char	‘+’ if query/target on the same strand; ‘-’ if opposite
        public bool SameStrand;

        // 6	string	Target sequence name
        public string TargetSeqName;

        public int TargetSeqCodename;

        // 7	int	Target sequence length
        public int TargetSeqLen;

        // 8	int	Target start coordinate on the original strand
        public int TargetStartCoord;

        // 9	int	Target end coordinate on the original strand
        public int TargetEndCoord;

        // 10	int	Number of matching bases in the mapping
        public int NumMatching;

        // 11	int	Number bases, including gaps, in the mapping
        public int NumAll;

        // 12	int	Mapping quality (0-255 with 255 for missing)
        public int Quality;

        public double Identity;

        public Overlap(string n, int ql, int qs, int qe, bool ss, string tn,
            int tl, int ts, int te, int nm, int na, int q, string tp, string cm, string s1, string dv)
        {
            this.QuerySeqName = n; this.QuerySeqLen = ql; this.QueryStartCoord = qs; this.QueryEndCoord = qe;
            this.SameStrand = ss; this.TargetSeqName = tn; this.TargetSeqLen = tl; this.TargetStartCoord = ts;
            this.TargetEndCoord = te; this.NumMatching = nm; this.NumAll = na; this.Quality = q;

            this.QuerySeqCodename = GetCodename(this.QuerySeqName);
            this.TargetSeqCodename = GetCodename(this.TargetSeqName);

            this.Identity = 1.0 * this.NumMatching / this.NumAll;

            this.OverlapScore = (this.QueryEndCoord - this.QueryStartCoord + this.TargetEndCoord - this.TargetStartCoord) * 1.0 / 2.0;
            this.ExtensionScore1 = this.OverlapScore + this.QueryStartCoord / 2.0 + (this.QuerySeqLen - this.QueryEndCoord + this.TargetStartCoord) / 2.0;
            this.ExtensionScore2 = this.OverlapScore + (this.TargetSeqLen - this.TargetEndCoord) / 2.0 + (this.QuerySeqLen - this.QueryEndCoord + this.TargetStartCoord) / 2.0;
        }

        private static int GetCodename(string name)
        {
            if (name.StartsWith("ctg", StringComparison.CurrentCultureIgnoreCase)) return int.Parse(name.Substring("ctg".Length));
            else return int.Parse(name.Substring("read".Length)) + 10;
        }


        /// <summary>
        /// 
        /// $RefLeftOverhang  = $tStart;
        /// $RefRightOverhang = ( $tLength - $tEnd );
        /// $QryLeftOverhang  = $qStart;
        /// $QryRightOverhang = ( $qLength - $qEnd );
        /// 
        /// </summary>
        /// 
        //public double ExtensionScore(bool es1) => this.OverlapScore + (es1 ? this.QueryStartCoord : this.TargetSeqLen - this.TargetEndCoord) / 2.0 + (this.QuerySeqLen - this.QueryEndCoord + this.TargetStartCoord) / 2.0;

        public double ExtensionScore1;
        public double ExtensionScore2;

        public double OverlapScore;

        public int Flipped = 0;

        public Overlap GetFlipped()
        {
            Flipped++;

            return new Overlap(
            this.TargetSeqName, this.TargetSeqLen, this.TargetStartCoord, this.TargetEndCoord,
            this.SameStrand,
            this.QuerySeqName, this.QuerySeqLen, this.QueryStartCoord, this.QueryEndCoord,
            this.NumMatching, this.NumAll, this.Quality, null, null, null, null);
        }
    }
}
