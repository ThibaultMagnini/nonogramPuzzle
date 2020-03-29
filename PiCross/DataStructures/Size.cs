using System;
namespace DataStructures
{
    public class Size
    {
        public Size( int Width, int Height )
        {
            if ( Width < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( Width ) );
            }
            else if ( Height < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( Height ) );
            }
            else
            {
                this.Width = Width;
                this.Height = Height;
            }
        }

        public int Width { get; }

        public int Height { get; }

        public override bool Equals( object obj )
        {
            return Equals( obj as Size );
        }

        public bool Equals( Size that )
        {
            return that != null && this.Width == that.Width && this.Height == that.Height;
        }

        public override int GetHashCode()
        {
            return Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString()
        {
            return $"PuzzleSize[{Width}, {Height}]";
        }

        public static bool operator ==( Size s1, Size s2 )
        {
            if ( object.ReferenceEquals( s1, null ) )
            {
                return object.ReferenceEquals( s2, null );
            }
            else
            {
                return s1.Equals( s2 );
            }
        }

        public static bool operator !=( Size s1, Size s2 )
        {
            return !(s1 == s2);
        }
    }
}
