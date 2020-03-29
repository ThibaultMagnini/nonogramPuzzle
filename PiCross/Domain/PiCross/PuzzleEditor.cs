using System;
using System.Linq;
using Cells;
using DataStructures;
using PiCross;
using AmbiguityEnum = PiCross.Ambiguity;

namespace PiCross
{
    internal class PuzzleEditor : IPuzzleEditor
    {
        private readonly EditorGrid editorGrid;

        private readonly IGrid<PuzzleEditorSquare> facadeGrid;

        private readonly ISequence<PuzzleEditorColumnConstraints> columnConstraints;

        private readonly ISequence<PuzzleEditorRowConstraints> rowConstraints;

        private AmbiguityChecker ambiguityChecker;

        private readonly IGrid<Cell<Ambiguity>> ambiguityGrid;

        public PuzzleEditor( EditorGrid editorGrid )
        {
            if ( editorGrid == null )
            {
                throw new ArgumentNullException( nameof( editorGrid ) );
            }
            else
            {
                this.editorGrid = editorGrid;
                ambiguityChecker = new AmbiguityChecker( columnConstraints: editorGrid.DeriveColumnConstraints(), rowConstraints: editorGrid.DeriveRowConstraints() );
                ambiguityGrid = ambiguityChecker.Ambiguities.Map( ( Ambiguity x ) => Cell.Create( x ) ).Copy();

                facadeGrid = editorGrid.Contents.Map( position => new PuzzleEditorSquare( this, position, ambiguityGrid[position] ) ).Copy();
                columnConstraints = editorGrid.Contents.ColumnIndices.Select( x => new PuzzleEditorColumnConstraints( editorGrid, x ) ).ToSequence();
                rowConstraints = editorGrid.Contents.RowIndices.Select( y => new PuzzleEditorRowConstraints( editorGrid, y ) ).ToSequence();
            }
        }

        public void ResolveAmbiguityStep()
        {
            if ( !ambiguityChecker.IsAmbiguityResolved )
            {
                ambiguityChecker.Step();

                RefreshAmbiguities();
            }
        }

        public void ResolveAmbiguity()
        {
            if ( !ambiguityChecker.IsAmbiguityResolved )
            {
                ambiguityChecker.Resolve();

                RefreshAmbiguities();
            }
        }

        public bool IsAmbiguityResolved => ambiguityChecker.IsAmbiguityResolved;

        public IGrid<IPuzzleEditorSquare> Grid => this.facadeGrid;

        public ISequence<IPuzzleEditorConstraints> ColumnConstraints => this.columnConstraints;

        public ISequence<IPuzzleEditorConstraints> RowConstraints => this.rowConstraints;

        private void OnSquareChanged( Vector2D position )
        {
            RefreshSquare( position );
            RefreshColumnConstraints( position.X );
            RefreshRowConstraints( position.Y );
            ResetAmbiguities();
        }

        private void ResetAmbiguities()
        {
            this.ambiguityChecker = new AmbiguityChecker( columnConstraints: editorGrid.DeriveColumnConstraints(), rowConstraints: editorGrid.DeriveRowConstraints() );
            RefreshAmbiguities();
        }

        private void RefreshSquare( Vector2D position )
        {
            this.facadeGrid[position].Refresh();
        }

        private void RefreshColumnConstraints( int x )
        {
            this.columnConstraints[x].Refresh();
        }

        private void RefreshRowConstraints( int x )
        {
            this.rowConstraints[x].Refresh();
        }

        private void RefreshAmbiguities()
        {
            ((IGrid<IVar<Ambiguity>>) ambiguityGrid).Overwrite( ambiguityChecker.Ambiguities );
        }

        private class PuzzleEditorSquare : IPuzzleEditorSquare
        {
            public PuzzleEditorSquare( PuzzleEditor parent, Vector2D position, Cell<Ambiguity> ambiguity )
            {
                this.IsFilled = new PuzzleEditorSquareContentsCell( parent, position );
                this.Position = position;
                this.Ambiguity = ambiguity;
            }

            public Cell<bool> IsFilled { get; }

            public Cell<Ambiguity> Ambiguity { get; }

            public Vector2D Position { get; }

            public void Refresh()
            {
                IsFilled.Refresh();
            }
        }

        private class PuzzleEditorSquareContentsCell : ManualCell<bool>
        {
            private readonly PuzzleEditor parent;

            private readonly IVar<Square> contents;

            private readonly Vector2D position;

            public PuzzleEditorSquareContentsCell( PuzzleEditor parent, Vector2D position )
                : base( SquareToBool( parent.editorGrid.Squares[position] ) )
            {
                this.parent = parent;
                this.contents = parent.editorGrid.Contents[position];
                this.position = position;
            }

            private static bool SquareToBool( Square square )
            {
                return square == Square.FILLED;
            }

            private static Square BoolToSquare( bool b )
            {
                return b ? Square.FILLED : Square.EMPTY;
            }

            protected override bool ReadValue()
            {
                return SquareToBool( parent.editorGrid.Squares[position] );
            }

            protected override void WriteValue( bool value )
            {
                var square = BoolToSquare( value );

                if ( this.contents.Value != square )
                {
                    this.contents.Value = square;

                    parent.OnSquareChanged( position );
                }
            }
        }

        private abstract class PuzzleEditorConstraints : IPuzzleEditorConstraints
        {
            protected PuzzleEditorConstraints( Func<Constraints> constraintsFetcher )
            {
                Constraints = Cell.Derived( () => constraintsFetcher() );
            }

            public Cell<Constraints> Constraints { get; }

            public void Refresh()
            {
                Constraints.Refresh();
            }
        }

        private class PuzzleEditorRowConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorRowConstraints( EditorGrid parent, int row )
                : base( () => parent.DeriveRowConstraints( row ) )
            {
                // NOP
            }
        }

        private class PuzzleEditorColumnConstraints : PuzzleEditorConstraints
        {
            public PuzzleEditorColumnConstraints( EditorGrid parent, int column )
                : base( () => parent.DeriveColumnConstraints( column ) )
            {
                // NOP
            }
        }

        public Puzzle BuildPuzzle()
        {
            return editorGrid.ToPuzzle();
        }
    }
}
