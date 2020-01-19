using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace herad
{
    public static class Utilities
    {
        public static string Flip(bool firstStrand, string v)
        {
            if (firstStrand) return v;
            else return string.Join(string.Empty, v.Select(c =>
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
        }
    }
}
