using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Range
    {
        public static Range FromStartAndLength( int start, int length )
        {
            return new Range( start, length );
        }

        public static Range FromStartAndEndExclusive( int start, int endExclusive )
        {
            return new Range( start, endExclusive - start );
        }

        private Range( int from, int length )
        {
            this.From = from;
            this.Length = length;
        }

        public int From { get; }

        public int Length { get; }

        public bool Contains( int n ) => From <= n && n < From + Length;

        public IEnumerable<int> Items => Enumerable.Range( From, Length );

        public override bool Equals( object obj )
        {
            return Equals( obj as Range );
        }

        public bool Equals( Range that )
        {
            return that != null && this.From == that.From && this.Length == that.Length;
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ Length.GetHashCode();
        }

        public override string ToString()
        {
            return $"[{From}...{From + Length})";
        }
    }
}
