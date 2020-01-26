using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Utilities
    {
        public static string Flip(this string v, bool firstStrand)
        {
            return firstStrand ? v : Reverse(v).GC_AT();
        }

        public static string GC_AT(this string v) => string.Join(string.Empty, v.Select(c =>
        {
            switch (c)
            {
                case 'G': return 'C';
                case 'C': return 'G';
                case 'T': return 'A';
                case 'A': return 'T';
                default:
                    throw new NullReferenceException();
            }
        }));

        public static string Reverse(string v) => string.Join("", v.Reverse());
    }
}
