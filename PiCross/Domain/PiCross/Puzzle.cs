using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace PiCross
{
    public class InvalidConstraintsException : PiCrossException
    {
        public InvalidConstraintsException()
            : base( "Invalid constraints" )
        {
            // NOP
        }
    }

    /// <summary>
    /// A Puzzle object contains all information about a PiCross puzzle,
    /// i.e. it contains the row and column constraints as well as the actual solution.
    /// </summary>
    public sealed class Puzzle
    {
        /// <summary>
        /// Creates a Puzzle from the constraints.
        /// Internally, the puzzle is automatically solved.
        /// If the constraints are ambiguous or contradictory,
        /// an exception is thrown.
        /// </summary>
        /// <param name="columnConstraints">Column constraints.</param>
        /// <param name="rowConstraints">Row constraints.</param>
        /// <returns>A Puzzle with the given constraints.</returns>
        /// <exception cref="InvalidConstraintsException">Thrown when the constraints
        /// don't lead to a single solution.</exception>
        public static Puzzle FromConstraints( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
        {
            var solverGrid = new SolverGrid( columnConstraints, rowConstraints );
            solverGrid.Refine();

            if ( !solverGrid.IsSolved )
            {
                throw new InvalidConstraintsException();
            }
            else
            {
                var grid = ConvertSquareGridToBoolGrid( solverGrid.Squares );

                return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: grid );
            }
        }

        /// <summary>
        /// Creates a puzzle from constraints in string format.
        /// Constraints on the same row/column should be separated by a single space,
        /// while rows/columns should be separated by a semicolon. For example,
        /// "2 1;1;;4 1".
        /// </summary>
        /// <param name="columnConstraints">Column constraints.</param>
        /// <param name="rowConstraints">Row constraints.</param>
        /// <returns></returns>
        public static Puzzle FromConstraints( string columnConstraints, string rowConstraints )
        {
            var parsedColumnConstraints = ParseConstraints( columnConstraints );
            var parsedRowConstraints = ParseConstraints( rowConstraints );

            return FromConstraints( columnConstraints: parsedColumnConstraints, rowConstraints: parsedRowConstraints );
        }

        private static int[][] ParseConstraints(string constraints)
        {
            return constraints.Split( ';' ).Select( part =>
              {
                  if ( part == "" )
                  {
                      return new int[0];
                  }
                  else
                  {
                      return part.Split( ' ' ).Select( int.Parse ).ToArray();
                  }
              } ).ToArray();
        }

        /// <summary>
        /// Creates a Puzzle from the constraints.
        /// Internally, the puzzle is automatically solved.
        /// If the constraints are ambiguous or contradictory,
        /// an exception is thrown.
        /// </summary>
        /// <param name="columnConstraints">Column constraints.</param>
        /// <param name="rowConstraints">Row constraints.</param>
        /// <returns>A Puzzle with the given constraints.</returns>
        /// <exception cref="InvalidConstraintsException">Thrown when the constraints
        /// don't lead to a single solution.</exception>
        public static Puzzle FromConstraints( int[][] columnConstraints, int[][] rowConstraints )
        {
            var columnConstraintsAsSequence = Sequence.FromItems( columnConstraints.Select( Constraints.FromValues ).ToArray() );
            var rowConstraintsAsSequence = Sequence.FromItems( rowConstraints.Select( Constraints.FromValues ).ToArray() );

            return FromConstraints( columnConstraints: columnConstraintsAsSequence, rowConstraints: rowConstraintsAsSequence );
        }

        /// <summary>
        /// Creates a Puzzle from a solution. The constraints
        /// will be inferred.
        /// </summary>
        /// <param name="grid">Solution represented by a grid of Squares.</param>
        /// <returns>A Puzzle with the given solution.</returns>
        public static Puzzle FromGrid( IGrid<Square> grid )
        {
            var editorGrid = new EditorGrid( grid );

            var columnConstraints = editorGrid.DeriveColumnConstraints();
            var rowConstraints = editorGrid.DeriveRowConstraints();

            var boolGrid = ConvertSquareGridToBoolGrid( grid );

            return new Puzzle( columnConstraints: columnConstraints, rowConstraints: rowConstraints, grid: boolGrid );
        }

        /// <summary>
        /// Creates a Puzzle from a solution. The constraints
        /// will be inferred.
        /// </summary>
        /// <param name="grid">Solution represented by a grid of bools.</param>
        /// <returns>A Puzzle with the given solution.</returns>
        public static Puzzle FromGrid( IGrid<bool> grid )
        {
            return FromGrid( grid.Map( Square.FromBool ) );
        }

        /// <summary>
        /// Creates a Puzzle from a sequence of strings representing
        /// the rows of the solution of a puzzle. A 'x' represents
        /// a filled square, a '.' corresponds to an empty square.
        /// </summary>
        /// <param name="rows">Strings representing rows.</param>
        /// <returns>Puzzle.</returns>
        public static Puzzle FromRowStrings( params string[] rows )
        {
            return FromGrid( Square.CreateGrid( rows ) );
        }

        /// <summary>
        /// Creates an empty puzzle with the given size.
        /// The constraints are all zero, the solution is the empty grid.
        /// </summary>
        /// <param name="size">Size of the puzzle.</param>
        /// <returns>Empty puzzle.</returns>
        public static Puzzle CreateEmpty( Size size )
        {
            return FromGrid( DataStructures.Grid.Create( size, false ) );
        }

        private static IGrid<bool> ConvertSquareGridToBoolGrid( IGrid<Square> squares )
        {
            return squares.Map( x => (bool) x ).Copy();
        }

        private Puzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, IGrid<bool> grid )
        {
            if ( columnConstraints == null )
            {
                throw new ArgumentNullException( nameof( columnConstraints ) );
            }
            else if ( rowConstraints == null )
            {
                throw new ArgumentNullException( nameof( rowConstraints ) );
            }
            else if ( grid == null )
            {
                throw new ArgumentNullException( nameof( grid ) );
            }
            else if ( columnConstraints.Length != grid.Size.Width )
            {
                throw new ArgumentException( $"{nameof(columnConstraints)} and grid do not agree on width" );
            }
            else if ( rowConstraints.Length != grid.Size.Height )
            {
                throw new ArgumentException( $"{nameof(rowConstraints)} and grid do not agree on height" );
            }
            else
            {
                this.ColumnConstraints = columnConstraints;
                this.RowConstraints = rowConstraints;
                this.Grid = grid;
            }
        }

        /// <summary>
        /// Grid representing the solution of the puzzle.
        /// </summary>
        public IGrid<bool> Grid { get; }

        /// <summary>
        /// Row constraints.
        /// </summary>
        public ISequence<Constraints> RowConstraints { get; }

        /// <summary>
        /// Column constraints.
        /// </summary>
        public ISequence<Constraints> ColumnConstraints { get; }

        /// <summary>
        /// Size of the puzzle.
        /// </summary>
        public Size Size => this.Grid.Size;

        /// <summary>
        /// Checks whether the puzzle is solveable based on the constraints.
        /// </summary>
        public bool IsSolvable
        {
            get
            {
                var solverGrid = new SolverGrid( columnConstraints: ColumnConstraints, rowConstraints: RowConstraints );

                solverGrid.Refine();

                return solverGrid.IsSolved;
            }
        }

        public override bool Equals( object obj )
        {
            return Equals( obj as Puzzle );
        }

        public bool Equals( Puzzle that )
        {
            return that != null && this.ColumnConstraints.Equals( that.ColumnConstraints ) && this.RowConstraints.Equals( that.RowConstraints ) && this.Grid.Equals( that.Grid );
        }

        public override int GetHashCode()
        {
            return Size.GetHashCode() ^ ColumnConstraints.GetHashCode() ^ RowConstraints.GetHashCode();
        }

        public override string ToString()
        {
            var rowStrings = from row in this.Grid.Rows
                             select row.Map( x => Square.FromBool( x ).Symbol ).Join();

            return rowStrings.ToSequence().Join( "\n" );
        }
    }
}
