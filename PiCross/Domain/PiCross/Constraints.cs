using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;
using System.Diagnostics;

namespace PiCross
{
    public sealed class Constraints
    {
        public static Constraints FromValues( params int[] values )
        {
            return new Constraints( values.ToSequence() );
        }

        public static Constraints FromValues( IEnumerable<int> values )
        {
            return new Constraints( values );
        }

        public static Constraints FromSequence( ISequence<int> sequence )
        {
            return new Constraints( sequence );
        }

        private Constraints( ISequence<int> values )
        {
            if ( values == null )
            {
                throw new ArgumentNullException( nameof( values ) );
            }
            else if ( values.Items.Any( n => n <= 0 ) )
            {
                throw new ArgumentOutOfRangeException( $"{nameof(values)}'s elements must all be strictly positive" );
            }
            else
            {
                this.Values = values;
            }
        }

        private Constraints( params int[] values )
            : this( Sequence.FromItems( values ) )
        {
            // NOP
        }

        private Constraints( IEnumerable<int> values )
            : this( values.ToArray() )
        {
            // NOP
        }

        internal IEnumerable<Slice> GenerateSlices( int sliceLength )
        {
            return GeneratePatterns( sliceLength, Values ).Select( x => new Slice( x ) );
        }

        private static IEnumerable<ISequence<int>> GenerateIntegers( int count, int sum )
        {
            Debug.Assert( count >= 0 );
            Debug.Assert( sum >= 0 );

            if ( count == 0 )
            {
                if ( sum == 0 )
                {
                    return new List<ISequence<int>> { Sequence.CreateEmpty<int>() };
                }
                else
                {
                    return Enumerable.Empty<ISequence<int>>();
                }
            }
            else
            {
                // TODO Possible optimization: create separate sequence prefixer class
                return from i in Enumerable.Range( 0, sum + 1 )
                       from tail in GenerateIntegers( count - 1, sum - i )
                       let prefix = Sequence.FromItems( i )
                       select prefix.Concatenate( tail );
            }
        }

        private static IEnumerable<ISequence<int>> GenerateSpacings( int length, int constraintCount, int constraintSum )
        {
            Debug.Assert( length >= 0 );
            Debug.Assert( constraintCount >= 0 );
            Debug.Assert( constraintSum >= 0 );

            var spacingCount = constraintCount + 1;
            var spacingSum = length - constraintSum - Math.Max( 0, constraintCount - 1 );

            if ( spacingSum < 0 )
            {
                return Enumerable.Empty<ISequence<int>>();
            }
            else
            {
                var numbers = GenerateIntegers( spacingCount, spacingSum );
                var deltas = Sequence.FromFunction( spacingCount, i => i == 0 || i == spacingCount - 1 ? 0 : 1 );

                return numbers.Select( ns => ns.ZipWith( deltas, ( x, y ) => x + y ) );
            }
        }

        private static IEnumerable<ISequence<Square>> GeneratePatterns( int totalSize, ISequence<int> constraints )
        {
            if ( totalSize < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( totalSize ) );
            }
            else if ( constraints == null )
            {
                throw new ArgumentNullException( nameof( constraints ) );
            }
            else
            {
                var constraintCount = constraints.Length;
                var constraintSum = constraints.Items.Sum();

                var spacings = GenerateSpacings( totalSize, constraintCount: constraintCount, constraintSum: constraintSum );

                var blocks = constraints.Map( n => Sequence.Repeat( n, Square.FILLED ) );

                foreach ( var spacing in spacings )
                {
                    var spaces = spacing.Map( n => Sequence.Repeat( n, Square.EMPTY ) );

                    yield return spaces.Intersperse( blocks ).Flatten();
                }
            }
        }

        internal int SatisfiedPrefixLength( Slice slice )
        {
            var knownPrefix = slice.KnownPrefix;
            var knownConstraints = knownPrefix.DeriveConstraints();

            return this.Values.CommonPrefixLength( knownConstraints.Values );
        }

        internal Constraints Reverse()
        {
            return new Constraints( this.Values.Reverse() );
        }

        internal int SatisfiedSuffixLength( Slice slice )
        {
            return this.Reverse().SatisfiedPrefixLength( slice.Reverse() );
        }

        public ISequence<int> Values { get; }

        internal bool IsSatisfied( Slice slice )
        {
            if ( slice.IsFullyKnown )
            {
                var derivedConstraints = slice.DeriveConstraints();

                return derivedConstraints.Equals( this );
            }
            else
            {
                return false;
            }
        }

        internal Range UnsatisfiedValueRange( Slice slice )
        {
            var knownPrefix = slice.KnownPrefix;
            var rest = slice.Lift( xs => xs.DropPrefix( knownPrefix.Squares.Length ) );
            var knownSuffix = rest.KnownSuffix;

            var left = this.SatisfiedPrefixLength( knownPrefix );
            var right = this.Values.Length - this.Lift( x => x.DropPrefix( left ) ).SatisfiedSuffixLength( knownSuffix );

            return Range.FromStartAndEndExclusive( left, right );
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Constraints );
        }

        public bool Equals( Constraints that )
        {
            return that != null && this.Values.Equals( that.Values );
        }

        public override int GetHashCode()
        {
            return this.Values.GetHashCode();
        }

        public override string ToString()
        {
            var str = this.Values.Map( x => x.ToString() ).Join( "-" );

            return $"[{str}]";
        }

        internal Constraints Lift( Func<ISequence<int>, ISequence<int>> function )
        {
            return new Constraints( function( Values ) );
        }

        internal void Generate( Action<bool[]> receiver, ISequence<Square> compatibleWith )
        {
            new Generator( receiver, compatibleWith, this.Values ).Generate();
        }

        internal ISequence<Square> Superposition( ISequence<Square> compatibleWith )
        {
            Square[] result = null;

            Generate( ( bool[] bs ) =>
            {
                if ( result == null )
                {
                    result = bs.Select( b => b ? Square.FILLED : Square.EMPTY ).ToArray();
                }
                else
                {
                    for ( var i = 0; i != result.Length; ++i )
                    {
                        var sqr = bs[i] ? Square.FILLED : Square.EMPTY;

                        result[i] = result[i].Merge( sqr );
                    }
                }
            }, compatibleWith );

            return Sequence.FromItems( result );
        }

        private class Generator
        {
            private readonly Action<bool[]> receiver;

            private readonly bool[] slice;

            private ISequence<Square> compatibleWith;

            private ISequence<int> constraints;

            private int[] cumulative;

            public Generator( Action<bool[]> receiver, ISequence<Square> compatibleWith, ISequence<int> constraints )
            {
                this.receiver = receiver;
                this.slice = new bool[compatibleWith.Length];
                this.compatibleWith = compatibleWith;
                this.constraints = constraints;
                this.cumulative = ComputeCumulative( constraints );
            }

            private static int[] ComputeCumulative( ISequence<int> constraints )
            {
                var result = new int[constraints.Length];

                if ( result.Length > 0 )
                {
                    result[result.Length - 1] = constraints[constraints.Length - 1];

                    for ( var i = result.Length - 2; i >= 0; --i )
                    {
                        result[i] = constraints[i] + 1 + result[i + 1];
                    }
                }

                return result;
            }

            public void Generate()
            {
                Generate( 0, 0 );
            }

            private void Yield()
            {
                receiver( slice );
            }

            private void Generate( int constraintIndex, int index )
            {
                if ( constraintIndex == constraints.Length )
                {
                    for ( var i = index; i != slice.Length; ++i )
                    {
                        if ( compatibleWith[i] == Square.FILLED )
                        {
                            return;
                        }

                        slice[i] = false;
                    }

                    Yield();
                }
                else if ( slice.Length - index < cumulative[constraintIndex] )
                {
                    return;
                }
                else
                {
                    if ( compatibleWith[index] != Square.FILLED )
                    {
                        slice[index] = false;

                        Generate( constraintIndex, index + 1 );
                    }

                    var size = constraints[constraintIndex];
                    for ( var i = 0; i != size; ++i )
                    {
                        if ( compatibleWith[index] == Square.EMPTY )
                        {
                            return;
                        }

                        slice[index] = true;

                        ++index;
                    }

                    ++constraintIndex;

                    if ( constraintIndex < constraints.Length )
                    {
                        if ( compatibleWith[index] == Square.FILLED )
                        {
                            return;
                        }

                        slice[index] = false;

                        index++;
                    }

                    Generate( constraintIndex, index );
                }
            }
        }
    }
}
