using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;
using PiCross;
using IO;

namespace PiCross
{
    internal class PuzzleSerializer : ISerializer<Puzzle>
    {
        public void Write( StreamWriter writer, Puzzle puzzle )
        {
            new Writer( writer, puzzle ).Write();
        }

        public Puzzle Read( StreamReader reader )
        {
            return new Reader( reader ).Read();
        }

        private class Reader : ReaderBase
        {
            internal Reader( StreamReader streamReader )
                : base( streamReader )
            {
                // NOP
            }

            internal Puzzle Read()
            {
                var size = ReadDimensions();
                var grid = ReadGrid( size );

                return Puzzle.FromGrid( grid );
            }

            private Size ReadDimensions()
            {
                var ns = ReadIntegers( expectedCount: 2 );

                return new Size( ns[0], ns[1] );
            }

            private IGrid<bool> ReadGrid( Size size )
            {
                var rows = ReadLines( size.Height );

                return Grid.Create<bool>( size, p => rows[p.Y][p.X] == 'x' );
            }
        }

        private class Writer : WriterBase
        {
            private readonly Puzzle puzzle;

            internal Writer( StreamWriter streamWriter, Puzzle puzzle )
                : base( streamWriter )
            {
                if ( puzzle == null )
                {
                    throw new ArgumentNullException( nameof( puzzle ) );
                }
                else
                {
                    this.puzzle = puzzle;
                }
            }

            public void Write()
            {
                WriteDimensions();
                WriteGrid();
            }

            private void WriteDimensions()
            {
                streamWriter.WriteLine( "{0} {1}", puzzle.Size.Width, puzzle.Size.Height );
            }

            private void WriteGrid()
            {
                foreach ( var row in puzzle.Grid.Rows )
                {
                    WriteRow( row );
                }
            }

            private void WriteRow( ISequence<bool> row )
            {
                foreach ( var square in row.Items )
                {
                    WriteSquare( square );
                }

                streamWriter.WriteLine();
            }

            private void WriteSquare( bool isFilled )
            {
                streamWriter.Write( isFilled ? 'x' : '.' );
            }
        }
    }
}
