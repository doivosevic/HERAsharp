using System;
using System.Diagnostics;

namespace herad
{
    public enum SeqType
    {
        A, R
    }

    [DebuggerDisplay("Seq {SeqType} {Name}")]
    public class Seq
    {
        private static int Counter = 0;

        public Seq(string name, string content, SeqType seqType)
        {
            this.Name = name;
            this.Content = content;
            this.SeqType = seqType;

            this.Codename = Counter++;
        }

        public string Name { get; }
        public string Content { get; }
        public SeqType SeqType { get; }

        public int Codename { get; }
    }
}
