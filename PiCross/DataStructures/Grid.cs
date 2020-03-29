using System;
using System.Collections.Generic;
using System.Linq;
using Cells;

namespace DataStructures
{
    /// <summary>
    /// Interface for grids. A grid is immutable (i.e. readonly).
    /// If you need to be able to modify items in a grid, use a <see cref="IVar" /> subtype as type parameter, e.g. <code>IGrid&lt;Cel&lt;T&gt;&gt;</code>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the grid.</typeparam>
    public interface IGrid<out T>
    {
        /// <summary>
        /// Retrieves an item from the grid at the given position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        T this[Vector2D position] { get; }

        /// <summary>
        /// Size of the grid.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Checks if <paramref name="position"/> is valid.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <returns>True if the position is valid for the grid, false otherwise.</returns>
        bool IsValidPosition( Vector2D position );

        /// <summary>
        /// Enumerates all valid positions of the grid.
        /// </summary>
        /// <returns>All valid positions of the grid. No specific order is guaranteed.</returns>
        IEnumerable<Vector2D> AllPositions { get; }

        /// <summary>
        /// Enumerates all row indices.
        /// </summary>
        /// <returns>All row indices.</returns>
        IEnumerable<int> RowIndices { get; }

        /// <summary>
        /// Enumerates all column indices.
        /// </summary>
        /// <returns>All column indices.</returns>
        IEnumerable<int> ColumnIndices { get; }

        /// <summary>
        /// Enumerates all items in the grid.
        /// </summary>
        IEnumerable<T> Items { get; }

        /// <summary>
        /// Returns a sequence representing the <paramref name="index"/>-th row.
        /// This sequence is backed by the grid: changes to the sequence
        /// will propagate to the grid and vice versa.
        /// </summary>
        /// <param name="index">Zero-based index of the row.</param>
        /// <returns>The row with index <paramref name="index"/></returns>
        ISequence<T> Row( int index );

        /// <summary>
        /// Returns a sequence representing the <paramref name="index"/>-th column.
        /// This sequence is backed by the grid: changes
        /// to the sequence will propagate to the grid and vice versa.
        /// </summary>
        /// <param name="index">Zero-based index of the column.</param>
        /// <returns>The column with index <paramref name="index"/></returns>
        ISequence<T> Column( int index );

        /// <summary>
        /// Enumerates all rows in order (increasing index).
        /// </summary>
        IEnumerable<ISequence<T>> Rows { get; }

        /// <summary>
        /// Enumerates all colums in order (increasing index).
        /// </summary>
        IEnumerable<ISequence<T>> Columns { get; }
    }

    /// <summary>
    /// Extension methods for IGrid objects.
    /// </summary>
    public static class IGridExtensions
    {
        public static IGrid<R> Map<T, R>( this IGrid<T> grid, Func<T, R> function )
        {
            return Grid.CreateVirtual( grid.Size, p => function( grid[p] ) );
        }

        public static IGrid<R> Map<T, R>( this IGrid<T> grid, Func<Vector2D, T, R> function )
        {
            return Grid.CreateVirtual( grid.Size, p => function( p, grid[p] ) );
        }

        public static IGrid<R> Map<T, R>( this IGrid<T> grid, Func<Vector2D, R> function )
        {
            return Grid.CreateVirtual( grid.Size, p => function( p ) );
        }

        public static void ForEach<T>( this IGrid<T> grid, Action<Vector2D> action )
        {
            foreach ( var position in grid.AllPositions )
            {
                action( position );
            }
        }

        public static void ForEach<T>( this IGrid<T> grid, Action<T> action )
        {
            foreach ( var position in grid.AllPositions )
            {
                action( grid[position] );
            }
        }

        public static void ForEach<T>( this IGrid<T> grid, Action<Vector2D, T> action )
        {
            foreach ( var position in grid.AllPositions )
            {
                action( position, grid[position] );
            }
        }

        public static IGrid<T> Copy<T>( this IGrid<T> grid )
        {
            return Grid.Create( grid.Size, p => grid[p] );
        }

        public static string[] AsStrings( this IGrid<char> grid )
        {
            return grid.Rows.Select( row => row.Join() ).ToArray();
        }

        public static void Overwrite<T>( this IGrid<IVar<T>> target, IGrid<T> source )
        {
            if ( target == null )
            {
                throw new ArgumentNullException( nameof( target ) );
            }
            else if ( source == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }
            else if ( target.Size != source.Size )
            {
                throw new ArgumentException( "Grids should have same size" );
            }
            else
            {
                foreach ( var position in target.AllPositions )
                {
                    target[position].Value = source[position];
                }
            }
        }

        public static void Overwrite<T>( this IGrid<Cell<T>> target, IGrid<T> source )
        {
            if ( target == null )
            {
                throw new ArgumentNullException( nameof( target ) );
            }
            else if ( source == null )
            {
                throw new ArgumentNullException( nameof( source ) );
            }
            else if ( target.Size != source.Size )
            {
                throw new ArgumentException( "Grids should have same size" );
            }
            else
            {
                foreach ( var position in target.AllPositions )
                {
                    target[position].Value = source[position];
                }
            }
        }

        public static ISequence<T> Linearize<T>( this IGrid<T> grid )
        {
            return grid.Rows.ToSequence().Flatten();
        }
    }

    public static class Grid
    {
        /// <summary>
        /// Creates a new grid with the given size where each element is determined by <paramref name="initializer"/>.
        /// </summary>
        /// <typeparam name="T">Type of the elements of the grid.</typeparam>
        /// <param name="size">Size of the grid.</param>
        /// <param name="initializer">Function that determines what value the grid should take at each position.</param>
        /// <returns>A new grid.</returns>
        public static IGrid<T> Create<T>( Size size, Func<Vector2D, T> initializer )
        {
            return new Grid<T>( size, initializer );
        }

        public static IGrid<T> Create<T>( Size size, T initialValue = default( T ) )
        {
            return Create( size, _ => initialValue );
        }

        public static IGrid<T> CreateVirtual<T>( Size size, Func<Vector2D, T> function )
        {
            return new VirtualGrid<T>( size, function );
        }

        public static IGrid<char> CreateCharacterGrid( params string[] strings )
        {
            var height = strings.Length;
            var width = strings[0].Length;
            var size = new Size( width, height );

            if ( !strings.All( s => s.Length == width ) )
            {
                throw new ArgumentException( "All strings must have equal length" );
            }
            else
            {
                return new Grid<char>( size, p => strings[p.Y][p.X] );
            }
        }

        internal static bool EqualItems<T>( IGrid<T> xss, IGrid<T> yss )
        {
            if ( xss == null )
            {
                throw new ArgumentNullException( nameof( xss ) );
            }
            else if ( yss == null )
            {
                throw new ArgumentNullException( nameof( yss ) );
            }
            else
            {
                if ( xss.Size == yss.Size )
                {
                    return xss.AllPositions.All( p => xss[p] == null ? yss[p] == null : xss[p].Equals( yss[p] ) );
                }
                else
                {
                    return false;
                }
            }
        }

        internal static int HashCode<T>( IGrid<T> grid )
        {
            return grid.Items.Select( x => x.GetHashCode() ).Aggregate( 0, ( x, y ) => x ^ y );
        }

        public static IGrid<T> FromRows<T>( ISequence<ISequence<T>> rows )
        {
            var width = rows[0].Length;
            var height = rows.Length;
            var size = new Size( width, height );

            return Grid.Create( size, p => rows[p.Y][p.X] );
        }
    }

    internal abstract class GridBase<T> : IGrid<T>
    {
        public override bool Equals( object obj )
        {
            return Equals( obj as IGrid<T> );
        }

        public bool Equals( IGrid<T> grid )
        {
            if ( grid == null )
            {
                return false;
            }
            else
            {
                return Grid.EqualItems( this, grid );
            }
        }

        public override int GetHashCode()
        {
            return Grid.HashCode( this );
        }

        public abstract T this[Vector2D position] { get; }

        public abstract Size Size { get; }

        public bool IsValidPosition( Vector2D position )
        {
            return 0 <= position.X && position.X < this.Size.Width && 0 <= position.Y && position.Y < this.Size.Height;
        }

        public IEnumerable<Vector2D> AllPositions
        {
            get
            {
                return from y in Enumerable.Range( 0, this.Size.Height )
                       from x in Enumerable.Range( 0, this.Size.Width )
                       select new Vector2D( x, y );
            }
        }

        public IEnumerable<int> RowIndices => Enumerable.Range( 0, this.Size.Height );

        public IEnumerable<int> ColumnIndices => Enumerable.Range( 0, this.Size.Width );

        public IEnumerable<T> Items => AllPositions.Select( p => this[p] );

        public ISequence<T> Row( int y )
        {
            return Sequence.FromFunction( Size.Width, x => this[new Vector2D( x, y )] );
        }

        public ISequence<T> Column( int x )
        {
            return Sequence.FromFunction( Size.Height, y => this[new Vector2D( x, y )] );
        }

        public IEnumerable<ISequence<T>> Rows => RowIndices.Select( Row );

        public IEnumerable<ISequence<T>> Columns => ColumnIndices.Select( Column );
    }

    internal class Grid<T> : GridBase<T>
    {
        private readonly T[,] items;

        private readonly Size size;

        public Grid( Size size, Func<Vector2D, T> initializer )
        {
            this.size = size;

            var width = size.Width;
            var height = size.Height;

            items = new T[width, height];

            foreach ( var x in Enumerable.Range( 0, width ) )
            {
                foreach ( var y in Enumerable.Range( 0, height ) )
                {
                    var position = new Vector2D( x, y );

                    items[x, y] = initializer( position );
                }
            }
        }

        public Grid( Size size, T initialValue = default( T ) )
            : this( size, p => initialValue )
        {
            // NOP
        }

        public override Size Size => new Size( items.GetLength( 0 ), items.GetLength( 1 ) );

        public override T this[Vector2D position] => items[position.X, position.Y];
    }

    internal class VirtualGrid<T> : GridBase<T>
    {
        private readonly Func<Vector2D, T> function;

        private readonly Size size;

        public VirtualGrid( Size size, Func<Vector2D, T> function )
        {
            if ( size == null )
            {
                throw new ArgumentNullException( nameof( size ) );
            }
            else if ( function == null )
            {
                throw new ArgumentNullException( nameof( function ) );
            }
            else
            {
                this.function = function;
                this.size = size;
            }
        }

        public override Size Size => size;

        public override T this[Vector2D position]
        {
            get
            {
                if ( !this.IsValidPosition( position ) )
                {
                    throw new ArgumentOutOfRangeException( nameof( position ) );
                }
                else
                {
                    return function( position );
                }
            }
        }
    }
}
