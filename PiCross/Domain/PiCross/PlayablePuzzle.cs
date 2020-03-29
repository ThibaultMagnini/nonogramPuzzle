using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    internal class PlayablePuzzle : IPlayablePuzzle
    {
        private readonly PlayGrid playGrid;

        private readonly ISequence<PlayablePuzzleConstraints> columnConstraints;

        private readonly ISequence<PlayablePuzzleConstraints> rowConstraints;

        public PlayablePuzzle( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( new PlayGrid( columnConstraints: columnConstraints, rowConstraints: rowConstraints ) )
        {
            // NOP
        }

        public PlayablePuzzle( PlayGrid playGrid )
        {
            if ( playGrid == null )
            {
                throw new ArgumentNullException( nameof( playGrid ) );
            }
            else
            {
                this.playGrid = playGrid;
                this.Grid = playGrid.Squares.Map( ( position, var ) => new PlayablePuzzleSquare( this, var, position ) ).Copy();
                this.columnConstraints = this.playGrid.ColumnConstraints.Map( constraints => new PlayablePuzzleConstraints( constraints ) ).Copy();
                this.rowConstraints = this.playGrid.RowConstraints.Map( constraints => new PlayablePuzzleConstraints( constraints ) ).Copy();
                this.IsSolved = Cell.Derived( DeriveIsSolved );
                this.UnknownCount = Cell.Derived( DeriveUnknownCount );
                this.ContainsUnknowns = Cell.Derived( DeriveContainsUnknowns );
            }
        }

        private bool DeriveIsSolved()
        {
            return columnConstraints.Items.All( x => x.IsSatisfied.Value ) && rowConstraints.Items.All( x => x.IsSatisfied.Value );
        }

        private int DeriveUnknownCount()
        {
            return Grid.Items.Count( playablePuzzleSquare => playablePuzzleSquare.Contents.Value == Square.UNKNOWN );
        }

        private bool DeriveContainsUnknowns()
        {
            return Grid.Items.Any( playablePuzzleSquare => playablePuzzleSquare.Contents.Value == Square.UNKNOWN );
        }

        public Cell<bool> IsSolved { get; }

        public Cell<int> UnknownCount { get; }

        public Cell<bool> ContainsUnknowns { get; }

        public IGrid<IPlayablePuzzleSquare> Grid { get; }

        public ISequence<IPlayablePuzzleConstraints> ColumnConstraints => columnConstraints;

        public ISequence<IPlayablePuzzleConstraints> RowConstraints => rowConstraints;

        private void Refresh( Vector2D position )
        {
            RefreshSquare( position );
            RefreshColumnConstraints( position.X );
            RefreshRowConstraints( position.Y );
            RefreshIsSolved();
            RefreshUnknownCount();
            RefreshContainsUnknowns();
        }

        private void Refresh()
        {
            RefreshSquares();
            RefreshConstraints();
            RefreshIsSolved();
            RefreshUnknownCount();
            RefreshContainsUnknowns();
        }

        private void RefreshIsSolved()
        {
            IsSolved.Refresh();
        }

        private void RefreshUnknownCount()
        {
            UnknownCount.Refresh();
        }

        private void RefreshContainsUnknowns()
        {
            ContainsUnknowns.Refresh();
        }

        private void RefreshSquares()
        {
            foreach ( var square in this.Grid.Items )
            {
                square.Contents.Refresh();
            }
        }

        private void RefreshConstraints()
        {
            columnConstraints.Each( RefreshConstraints );
            rowConstraints.Each( RefreshConstraints );
        }

        private void RefreshSquare( Vector2D position )
        {
            this.Grid[position].Contents.Refresh();
        }

        private void RefreshColumnConstraints( int x )
        {
            RefreshConstraints( this.columnConstraints[x] );
        }

        private void RefreshRowConstraints( int y )
        {
            RefreshConstraints( this.rowConstraints[y] );
        }

        private static void RefreshConstraints( PlayablePuzzleConstraints constraints )
        {
            constraints.IsSatisfied.Refresh();

            foreach ( var value in constraints.Values.Items )
            {
                value.IsSatisfied.Refresh();
            }
        }

        private class PlayablePuzzleSquare : IPlayablePuzzleSquare
        {
            public PlayablePuzzleSquare( PlayablePuzzle parent, IVar<Square> contents, Vector2D position )
            {
                this.Contents = new PlayablePuzzleSquareContentsCell( parent, contents, position );
                this.Position = position;
            }

            Cell<Square> IPlayablePuzzleSquare.Contents => Contents;

            public PlayablePuzzleSquareContentsCell Contents { get; }

            public Vector2D Position { get; }

         
        }

        private class PlayablePuzzleSquareContentsCell : ManualCell<Square>
        {
            private readonly PlayablePuzzle parent;

            private readonly IVar<Square> contents;

            private readonly Vector2D position;

            public PlayablePuzzleSquareContentsCell( PlayablePuzzle parent, IVar<Square> contents, Vector2D position )
                : base( contents.Value )
            {
                this.parent = parent;
                this.contents = contents;
                this.position = position;
            }

            protected override Square ReadValue()
            {
                return this.contents.Value;
            }

            protected override void WriteValue( Square value )
            {
                this.contents.Value = value;

                parent.Refresh( position );
            }
        }

        private class PlayablePuzzleConstraints : IPlayablePuzzleConstraints
        {
            public PlayablePuzzleConstraints( PlayGridConstraints constraints )
            {
                this.Values = constraints.Values.Map( constraint => new PlayablePuzzleConstraintsValue( constraint ) ).Copy();
                this.IsSatisfied = new ReadonlyManualCell<bool>( () => constraints.IsSatisfied );
            }

            ISequence<IPlayablePuzzleConstraintsValue> IPlayablePuzzleConstraints.Values => Values;

            public ISequence<PlayablePuzzleConstraintsValue> Values { get; }

            Cell<bool> IPlayablePuzzleConstraints.IsSatisfied => IsSatisfied;

            public ReadonlyManualCell<bool> IsSatisfied { get; }
        }

        private class PlayablePuzzleConstraintsValue : IPlayablePuzzleConstraintsValue
        {

            private readonly PlayGridConstraintValue constraint;

            public PlayablePuzzleConstraintsValue( PlayGridConstraintValue constraint )
            {
                this.constraint = constraint;
                this.IsSatisfied = new ReadonlyManualCell<bool>( () => constraint.IsSatisfied );
            }

            public int Value => constraint.Value;

            Cell<bool> IPlayablePuzzleConstraintsValue.IsSatisfied => IsSatisfied;

            public ReadonlyManualCell<bool> IsSatisfied { get; }
        }
    }
}
