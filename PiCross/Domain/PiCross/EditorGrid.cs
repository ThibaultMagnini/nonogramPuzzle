using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    internal class EditorGrid
    {
        private EditorGrid( IGrid<IVar<Square>> grid )
        {
            this.Contents = grid;
        }

        public EditorGrid( IGrid<Square> grid )
            : this( grid.Map( x => new Var<Square>( x ) ).Copy() )
        {
            // NOP
        }

        private EditorGrid( Size size )
            : this( Grid.Create( size, _ => new Var<Square>( Square.UNKNOWN ) ) )
        {
            // NOP
        }

        public static EditorGrid FromSize( Size size )
        {
            return new EditorGrid( size );
        }

        public static EditorGrid FromStrings( params string[] rows )
        {
            return new EditorGrid( Square.CreateGrid( rows ) );
        }

        public static EditorGrid FromPuzzle( Puzzle puzzle )
        {
            return new EditorGrid( puzzle.Grid.Map( b => b ? Square.FILLED : Square.EMPTY ) );
        }

        public IGrid<IVar<Square>> Contents { get; }

        public IGrid<Square> Squares => Contents.Map( var => var.Value );

        public Slice Column( int x )
        {
            return new Slice( Squares.Column( x ) );
        }

        public Slice Row( int y )
        {
            return new Slice( Squares.Row( y ) );
        }

        public IEnumerable<Slice> Columns => Contents.ColumnIndices.Select( Column );

        public IEnumerable<Slice> Rows => Contents.RowIndices.Select( Row );

        public Constraints DeriveColumnConstraints( int column )
        {
            return Column( column ).DeriveConstraints();
        }

        public Constraints DeriveRowConstraints( int row )
        {
            return Row( row ).DeriveConstraints();
        }

        public ISequence<Constraints> DeriveColumnConstraints()
        {
            return Sequence.FromEnumerable( Columns.Select( column => column.DeriveConstraints() ) );
        }

        public ISequence<Constraints> DeriveRowConstraints()
        {
            return Sequence.FromEnumerable( Rows.Select( row => row.DeriveConstraints() ) );
        }

        public PlayGrid CreatePlayGrid()
        {
            return new PlayGrid( columnConstraints: DeriveColumnConstraints(), rowConstraints: DeriveRowConstraints() );
        }

        public SolverGrid CreateSolverGrid()
        {
            return new SolverGrid( columnConstraints: DeriveColumnConstraints(), rowConstraints: DeriveRowConstraints() );
        }

        public Size Size => Contents.Size;

        public Puzzle ToPuzzle()
        {
            return Puzzle.FromGrid( this.Contents.Map( cell => cell.Value == Square.FILLED ) );
        }
    }
}
