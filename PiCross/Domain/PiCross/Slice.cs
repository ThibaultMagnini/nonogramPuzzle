using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PiCross
{
    internal class Slice
    {
        public Slice( ISequence<bool> squares )
            : this( squares.Map( x => x ? Square.FILLED : Square.EMPTY ) )
        {
            // NOP
        }

        public Slice( ISequence<Square> squares )
        {
            if ( squares == null )
            {
                throw new ArgumentNullException( nameof( squares ) );
            }
            else
            {
                this.Squares = squares;
            }
        }

        public ISequence<Square> Squares { get; }

        public override bool Equals( object obj )
        {
            return Equals( obj as Slice );
        }

        public bool Equals( Slice that )
        {
            if ( that == null )
            {
                return false;
            }
            else
            {
                return this.Squares.Equals( that.Squares );
            }
        }

        public override int GetHashCode()
        {
            return Squares.GetHashCode();
        }

        public override string ToString()
        {
            return Squares.Map( x => x.Symbol ).AsString();
        }

        public bool CompatibleWith( Slice that )
        {
            if ( that == null )
            {
                throw new ArgumentNullException( nameof( that ) );
            }
            else if ( this.Squares.Length != that.Squares.Length )
            {
                throw new ArgumentException( "Slices should have same length" );
            }
            else
            {
                return this.Squares.Indices.All( i => Squares[i].CompatibleWith( that.Squares[i] ) );
            }
        }

        public Slice Merge( Slice that )
        {
            if ( that == null )
            {
                throw new ArgumentNullException( nameof( that ) );
            }
            else if ( this.Squares.Length != that.Squares.Length )
            {
                throw new ArgumentException( "Slices should have same length" );
            }
            else
            {
                return new Slice( this.Squares.ZipWith( that.Squares, ( x, y ) => x.Merge( y ) ) );
            }
        }

        public static Slice FromString( string str )
        {
            return new Slice( Sequence.FromString( str ).Map( Square.FromSymbol ) );
        }

        public static Slice Merge( IEnumerable<Slice> slices )
        {
            return slices.Aggregate( ( x, y ) => x.Merge( y ) );
        }

        public Slice Refine( Constraints constraints )
        {
            if ( constraints == null )
            {
                throw new ArgumentNullException( nameof( constraints ) );
            }
            else
            {
                return new Slice( constraints.Superposition( Squares ) );
            }
        }

        public ISequence<Range> FindBlocks()
        {
            var blocks = new List<Range>();
            var start = -1;

            var squares = this.Squares.Concatenate( Sequence.FromItems( Square.EMPTY ) );

            for ( var i = 0; i != this.Squares.Length; ++i )
            {
                var square = this.Squares[i];

                Debug.Assert( square != null );

                if ( square == Square.UNKNOWN )
                {
                    throw new InvalidOperationException( "Slice must be fully known" );
                }
                else if ( square == Square.EMPTY )
                {
                    if ( start != -1 )
                    {
                        blocks.Add( Range.FromStartAndEndExclusive( start, i - 1 ) );
                        start = -1;
                    }
                }
                else // square == Square.FILLED
                {
                    if ( start == -1 )
                    {
                        start = i;
                    }
                }
            }

            return Sequence.FromEnumerable( blocks );
        }

        public Constraints DeriveConstraints()
        {
            var fillCount = 0;
            var constraints = new List<int>();

            for ( var i = 0; i != this.Squares.Length; ++i )
            {
                var square = this.Squares[i];

                if ( square == Square.FILLED )
                {
                    fillCount++;
                }
                else if ( square == Square.EMPTY )
                {
                    if ( fillCount > 0 )
                    {
                        constraints.Add( fillCount );
                    }

                    fillCount = 0;
                }
                else
                {
                    throw new InvalidOperationException( "Slice contained invalid square" );
                }
            }

            if ( fillCount > 0 )
            {
                constraints.Add( fillCount );
            }

            return Constraints.FromValues( constraints );
        }

        public bool IsFullyKnown
        {
            get
            {
                return Squares.Items.All( x => x != Square.UNKNOWN );
            }
        }

        public Slice KnownPrefix
        {
            get
            {
                var known = this.Squares.TakeWhile( sqr => sqr != Square.UNKNOWN );

                if ( known.Length == this.Squares.Length )
                {
                    return this;
                }
                else
                {
                    var sqrs = known.Reverse().DropWhile( sqr => sqr == Square.FILLED ).Reverse();

                    return new Slice( sqrs );
                }
            }
        }

        public Slice KnownSuffix
        {
            get
            {
                return Reverse().KnownPrefix.Reverse();
            }
        }

        public Slice Reverse()
        {
            return Lift( ns => ns.Reverse() );
        }

        public Slice Prefix( int length )
        {
            return Lift( ns => ns.Prefix( length ) );
        }

        public Slice Suffix( int length )
        {
            return Lift( ns => ns.Suffix( length ) );
        }

        public Slice Lift( Func<ISequence<Square>, ISequence<Square>> function )
        {
            return new Slice( function( Squares ) );
        }
    }
}
