using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cells;

namespace DataStructures
{
    /// <summary>
    /// Interface for array-like objects. A sequence is readonly.
    /// If you need to be able to modify a sequence's elements,
    /// populate the sequence with objects with <see cref="IVar"/>.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public interface ISequence<out T> : IEnumerable<T>
    {
        /// <summary>
        /// Number of items in the sequence.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Looks up an element in the sequence.
        /// </summary>
        /// <param name="index">Zero-based index of the element.</param>
        /// <returns>Element with index <paramref name="index"/></returns>
        T this[int index] { get; }

        /// <summary>
        /// Enumerates all indices in increasing order.
        /// </summary>
        IEnumerable<int> Indices { get; }

        /// <summary>
        /// Enumerates all items in order of increasing index.
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Checks whether the given index is valid.
        /// </summary>
        /// <param name="index">Index to be checked.</param>
        /// <returns>True if valid, false otherwise.</returns>
        bool IsValidIndex( int index );
    }

    /// <summary>
    /// A series of sequence-related static factory methods.
    /// </summary>
    public static class Sequence
    {
        /// <summary>
        /// Creates an empty sequence.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <returns>An empty sequence.</returns>
        public static ISequence<T> CreateEmpty<T>()
        {
            return new EmptySequence<T>();
        }

        /// <summary>
        /// Creates a sequence containing the specified values.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="items">Items.</param>
        /// <returns>A sequence containing the given items in the given order.</returns>
        public static ISequence<T> FromItems<T>( params T[] items )
        {
            return new ArraySequence<T>( items.Length, i => items[i] );
        }

        /// <summary>
        /// Creates a sequence from a function. The function is called by-need
        /// and will be called again with each indexing.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="length">Length of the sequence.</param>
        /// <param name="function">Function determining the elements.</param>
        /// <returns>A sequence.</returns>
        public static ISequence<T> FromFunction<T>( int length, Func<int, T> function )
        {
            return new VirtualSequence<T>( length, function );
        }

        /// <summary>
        /// Converst an IEnumerable to an ISequence.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="xs">The IEnumerable.</param>
        /// <returns>Sequence containing the elements of <paramref name="xs"/> in the same order.</returns>
        public static ISequence<T> FromEnumerable<T>( IEnumerable<T> xs )
        {
            return FromItems( xs.ToArray() );
        }

        /// <summary>
        /// Creates a sequence of length <paramref name="length"/>. All items are equal to the given <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="length">Length of the sequence.</param>
        /// <param name="value">Value used to initialize the sequence.</param>
        /// <returns>Sequence containing <paramref name="value"/> repeated <paramref name="length"/> times.</returns>
        public static ISequence<T> Repeat<T>( int length, T value )
        {
            return new VirtualSequence<T>( length, _ => value );
        }

        /// <summary>
        /// Creates a character sequence.
        /// </summary>
        /// <param name="str">String whose characters will be used to initialize the sequence.</param>
        /// <returns>Sequence.</returns>
        public static ISequence<char> FromString( string str )
        {
            return FromItems( str.ToCharArray() );
        }

        /// <summary>
        /// Creates a sequence containing the values <paramref name="from"/> to <paramref name="from"/> + <paramref name="length"/>.
        /// </summary>
        /// <param name="from">Value of first item.</param>
        /// <param name="length">Length of the sequence.</param>
        /// <returns>Sequence.</returns>
        public static ISequence<int> Range( int from, int length )
        {
            return Sequence.FromFunction( length, i => from + i );
        }

        public static ISequence<bool> Bits( byte n )
        {
            return FromFunction( 8, i => ((n >> (7 - i)) & 1) == 1 );
        }
    }

    public static class SequenceExtensions
    {
        public static ISequence<T> ToSequence<T>( this IEnumerable<T> enumerable )
        {
            return Sequence.FromEnumerable( enumerable );
        }

        public static ISequence<T> Concatenate<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            return Sequence.FromFunction( xs.Length + ys.Length, i => i < xs.Length ? xs[i] : ys[i - xs.Length] );
        }

        public static ISequence<T> Flatten<T>( this ISequence<ISequence<T>> xss )
        {
            if ( xss.Length == 0 )
            {
                return Sequence.CreateEmpty<T>();
            }
            else if ( xss.Length == 1 )
            {
                return xss[0];
            }
            else
            {
                var middle = xss.Length / 2;

                var leftHalf = xss.Prefix( middle );
                var rightHalf = xss.DropPrefix( middle );

                return Flatten( leftHalf ).Concatenate( Flatten( rightHalf ) );
            }
        }

        public static ISequence<R> ZipWith<T1, T2, R>( this ISequence<T1> xs, ISequence<T2> ys, Func<T1, T2, R> zipper )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( ys == null )
            {
                throw new ArgumentNullException( nameof( ys ) );
            }
            else if ( xs.Length != ys.Length )
            {
                throw new ArgumentException( "xs and ys should have same length" );
            }
            else if ( zipper == null )
            {
                throw new ArgumentNullException( nameof( zipper ) );
            }
            else
            {
                return Sequence.FromFunction( xs.Length, i => zipper( xs[i], ys[i] ) );
            }
        }

        public static ISequence<R> Map<T, R>( this ISequence<T> xs, Func<T, R> function )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( nameof( function ) );
            }
            else
            {
                return Sequence.FromFunction( xs.Length, i => function( xs[i] ) );
            }
        }

        public static ISequence<R> Map<T, R>( this ISequence<T> xs, Func<int, T, R> function )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( nameof( function ) );
            }
            else
            {
                return Sequence.FromFunction( xs.Length, i => function( i, xs[i] ) );
            }
        }

        public static ISequence<T> Intersperse<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( ys == null )
            {
                throw new ArgumentNullException( nameof( ys ) );
            }
            else if ( xs.Length != ys.Length + 1 )
            {
                throw new ArgumentException( $"xs.Length (={xs.Length}) should be equal to ys.Length + 1 (={ys.Length} + 1)" );
            }
            else
            {
                return Sequence.FromFunction( xs.Length + ys.Length, i => i % 2 == 0 ? xs[i / 2] : ys[(i - 1) / 2] );
            }
        }

        public static string AsString( this ISequence<char> xs )
        {
            return new string( xs.Items.ToArray() );
        }

        public static ISequence<T> Subsequence<T>( this ISequence<T> xs, int from, int count )
        {
            if ( !xs.IsValidIndex( from ) )
            {
                throw new ArgumentOutOfRangeException( nameof( from ) );
            }
            else if ( count < 0 || (count > 0 && from + count - 1 >= xs.Length) )
            {
                throw new ArgumentOutOfRangeException( nameof( count ) );
            }
            else
            {
                return Sequence.FromFunction( count, i => xs[i + from] );
            }
        }

        private static ISequence<T> SafeSubsequence<T>( this ISequence<T> xs, int from, int count )
        {
            if ( !xs.IsValidIndex( from ) )
            {
                throw new ArgumentOutOfRangeException( nameof( from ) );
            }
            else if ( count < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( count ) );
            }
            else
            {
                count = Math.Min( count, xs.Length - from );

                return Sequence.FromFunction( count, i => xs[i + from] );
            }
        }

        public static ISequence<T> Prefix<T>( this ISequence<T> xs, int length )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( length > xs.Length )
            {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }
            else
            {
                return xs.Subsequence( 0, length );
            }
        }

        public static ISequence<T> Suffix<T>( this ISequence<T> xs, int from )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( from > xs.Length )
            {
                throw new ArgumentOutOfRangeException( "from", $"from = {from}, length = {xs.Length}" );
            }
            else
            {
                if ( from == xs.Length )
                {
                    return Sequence.CreateEmpty<T>();
                }
                else
                {
                    return xs.Subsequence( from, xs.Length - from );
                }
            }
        }

        public static ISequence<T> DropPrefix<T>( this ISequence<T> xs, int length )
        {
            return xs.Suffix( length );
        }

        public static ISequence<T> DropSuffix<T>( this ISequence<T> xs, int length )
        {
            return xs.Prefix( xs.Length - length - 1 );
        }

        public static int FindFirstIndexOf<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var i = 0;

            while ( i < xs.Length )
            {
                var current = xs[i];

                if ( predicate( current ) )
                {
                    return i;
                }

                ++i;
            }

            return -1;
        }

        public static ISequence<T> TakeWhile<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var index = xs.FindFirstIndexOf( x => !predicate( x ) );

            if ( index == -1 )
            {
                return xs;
            }
            else
            {
                return xs.Prefix( index );
            }
        }

        public static ISequence<T> DropWhile<T>( this ISequence<T> xs, Func<T, bool> predicate )
        {
            var index = xs.FindFirstIndexOf( x => !predicate( x ) );

            if ( index == -1 )
            {
                return Sequence.CreateEmpty<T>();
            }
            else
            {
                return xs.Suffix( index );
            }
        }

        public static ISequence<T> Reverse<T>( this ISequence<T> xs )
        {
            return Sequence.FromFunction( xs.Length, i => xs[xs.Length - i - 1] );
        }

        public static int CommonPrefixLength<T>( this ISequence<T> xs, ISequence<T> ys )
        {
            var i = 0;

            while ( i < xs.Length && i < ys.Length && xs[i].Equals( ys[i] ) )
            {
                ++i;
            }

            return i;
        }

        public static ISequence<T> Copy<T>( this ISequence<T> xs )
        {
            return new ArraySequence<T>( xs.Length, i => xs[i] );
        }

        public static string Join( this ISequence<char> cs, string infix = "" )
        {
            return string.Join( infix, cs.Items.ToArray() );
        }

        public static string Join( this ISequence<string> cs, string infix = "" )
        {
            return string.Join( infix, cs.Items.ToArray() );
        }

        public static void Each<T>( this ISequence<T> xs, Action<T> action )
        {
            foreach ( var x in xs.Items )
            {
                action( x );
            }
        }

        public static bool Overwrite<T>( this ISequence<IVar<T>> xs, ISequence<T> ys )
        {
            if ( xs == null )
            {
                throw new ArgumentNullException( nameof( xs ) );
            }
            else if ( ys == null )
            {
                throw new ArgumentNullException( nameof( ys ) );
            }
            else if ( xs.Length != ys.Length )
            {
                throw new ArgumentException( "xs and ys must have same length" );
            }
            else
            {
                var overwriteDetected = false;

                foreach ( var i in xs.Indices )
                {
                    var cell = xs[i];
                    var value = ys[i];

                    if ( !Util.AreEqual( cell.Value, value ) )
                    {
                        cell.Value = value;
                        overwriteDetected = true;
                    }
                }

                return overwriteDetected;
            }
        }

        public static ISequence<ISequence<T>> Group<T>( this ISequence<T> xs, int groupSize )
        {
            return Sequence.FromFunction( (xs.Length + groupSize - 1) / groupSize, i => xs.SafeSubsequence( i * groupSize, groupSize ) );
        }

        public static ISequence<byte> GroupBits( this ISequence<bool> bits )
        {
            var byteGroups = bits.Group( 8 );

            return byteGroups.Map( group => group.ToByte() );
        }

        public static byte ToByte( this ISequence<bool> bits )
        {
            if ( bits == null )
            {
                throw new ArgumentNullException( nameof( bits ) );
            }
            else if ( bits.Length > 8 )
            {
                throw new ArgumentOutOfRangeException( "Maximum eight bits allowed" );
            }
            else
            {
                return (byte) (bits.Map( ( i, b ) => (b ? 1 : 0) << (7 - i) ).Items.Aggregate( 0, ( x, y ) => x | y ));
            }
        }

        public static T[] ToArray<T>( this ISequence<T> seq )
        {
            var array = new T[seq.Length];

            seq.Each( i => array[i] = seq[i] );

            return array;
        }

        public static void Each<T>( this ISequence<T> xs, Action<int> action )
        {
            foreach ( var i in xs.Indices )
            {
                action( i );
            }
        }
    }

    internal abstract class SequenceBase<T> : ISequence<T>
    {
        public abstract int Length { get; }

        public abstract T this[int index] { get; }

        public IEnumerable<int> Indices
        {
            get
            {
                return Enumerable.Range( 0, this.Length );
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<T> Items => Indices.Select( i => this[i] );

        public bool IsValidIndex( int index )
        {
            return 0 <= index && index < Length;
        }

        public override string ToString()
        {
            var str = string.Join( ", ", Items.Select( x => x.ToString() ) );

            return str;
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as ISequence<T> );
        }

        public bool Equals( ISequence<T> that )
        {
            if ( that == null )
            {
                return false;
            }
            else if ( this.Length != that.Length )
            {
                return false;
            }
            else
            {
                return Indices.All( i => EqualItems( this[i], that[i] ) );
            }
        }

        private bool EqualItems( T x, T y )
        {
            if ( x == null )
            {
                return y == null;
            }
            else
            {
                return x.Equals( y );
            }
        }

        public override int GetHashCode()
        {
            return Items.Select( x => x.GetHashCode() ).Aggregate( 0, ( x, y ) => x ^ y );
        }
    }

    internal class EmptySequence<T> : SequenceBase<T>
    {
        public override int Length => 0;

        public override T this[int index] => throw new ArgumentOutOfRangeException( nameof( index ) );
    }

    internal class ArraySequence<T> : SequenceBase<T>
    {
        private readonly T[] items;

        public ArraySequence( int length, Func<int, T> initializer )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }
            else if ( initializer == null )
            {
                throw new ArgumentNullException( nameof( initializer ) );
            }
            else
            {
                items = Enumerable.Range( 0, length ).Select( initializer ).ToArray();
            }
        }

        public override int Length
        {
            get
            {
                return items.Length;
            }
        }

        public override T this[int index]
        {
            get
            {
                return items[index];
            }
        }
    }

    internal class VirtualSequence<T> : SequenceBase<T>
    {
        private readonly int length;

        private readonly Func<int, T> function;

        public VirtualSequence( int length, Func<int, T> function )
        {
            if ( length < 0 )
            {
                throw new ArgumentOutOfRangeException( nameof( length ) );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( nameof( function ) );
            }
            else
            {
                this.length = length;
                this.function = function;
            }
        }

        public override int Length
        {
            get
            {
                return length;
            }
        }

        public override T this[int index]
        {
            get
            {
                if ( !IsValidIndex( index ) )
                {
                    throw new ArgumentOutOfRangeException( nameof( index ) );
                }
                else
                {
                    return function( index );
                }
            }
        }
    }
}
