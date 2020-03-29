using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross;

namespace PiCross.Tests
{
    public class TestBase
    {
        internal static Slice CreateSlice( string str )
        {
            return Slice.FromString( str );
        }

        protected static Constraints CreateConstraints( params int[] constraints )
        {
            return Constraints.FromValues( constraints );
        }

        protected static ISequence<bool> CreateBooleans( string str )
        {
            return Sequence.FromString( str ).Map( CharToBool );
        }

        protected static IGrid<bool> CreateBooleansGrid( params string[] str )
        {
            return Grid.CreateCharacterGrid( str ).Map( CharToBool );
        }

        private static bool CharToBool(char c)
        {
            switch ( c )
            {
                case 't':
                    return true;

                case 'f':
                    return false;

                default:
                    throw new ArgumentException( "Invalid character" );
            }
        }
       

        protected static IGrid<Square> ParseFullyKnown( params string[] rows )
        {
            return Grid.CreateCharacterGrid( rows ).Map( c =>
            {
                switch ( c )
                {
                    case '.':
                    case ' ':
                        return Square.EMPTY;

                    case 'x':
                    case 'X':
                        return Square.FILLED;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        protected static IGrid<Square> ParsePartiallyKnown( params string[] rows )
        {
            return Grid.CreateCharacterGrid( rows ).Map( c =>
            {
                switch ( c )
                {
                    case 'x':
                    case '.':
                        return Square.UNKNOWN;

                    case 'X':
                        return Square.FILLED;

                    case ' ':
                        return Square.EMPTY;

                    default:
                        throw new ArgumentException( "Invalid character" );
                }
            } );
        }

        internal static EditorGrid ParseEditorGrid( params string[] rows )
        {
            var editorGridData = ParseFullyKnown( rows );

            return new EditorGrid( editorGridData );
        }

        internal static PlayGrid CreatePlayGrid( params string[] rows )
        {
            var editorGrid = ParseEditorGrid( rows );
            var playGrid = editorGrid.CreatePlayGrid();

            playGrid.Squares.Overwrite<Square>( ParsePartiallyKnown( rows ) );

            return playGrid;
        }

        internal static PlayablePuzzle CreateManualPuzzle( params string[] rows )
        {
            var playGrid = CreatePlayGrid( rows );

            return new PlayablePuzzle( playGrid );
        }

        internal static void OverwritePlayGrid( PlayGrid grid, params string[] rows)
        {
            grid.Squares.Overwrite( Square.CreateGrid( rows ) );
        }

        protected static IPuzzleEditor CreatePuzzleEditor(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );

            return new PuzzleEditor( editorGrid );
        }

        protected static Puzzle CreatePuzzle(params string[] rows)
        {
            return Puzzle.FromRowStrings( rows );
        }

        internal static SolverGrid CreateSolverGrid(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );

            return editorGrid.CreateSolverGrid();
        }

        internal static AmbiguityChecker CreateAmbiguityChecker(params string[] rows)
        {
            var puzzle = Puzzle.FromRowStrings( rows );

            return new AmbiguityChecker( columnConstraints: puzzle.ColumnConstraints, rowConstraints: puzzle.RowConstraints );
        }

        protected static IPuzzleLibrary CreateLibrary(params Puzzle[] puzzles)
        {
            var author = "test";
            var library = InMemoryDatabase.PuzzleLibrary.CreateEmpty();

            foreach ( var puzzle in puzzles )
            {
                library.Create( puzzle, author );
            }

            return new PuzzleLibraryAdapter( library );
        }
    }
}
