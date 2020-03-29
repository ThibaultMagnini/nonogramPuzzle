using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using PiCross;
using System.Text;


namespace PiCross.Tests
{
    [TestClass]    
    public class PuzzleIOTests
    {
        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle1()
        {
            CheckIO( "." );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle2()
        {
            CheckIO( ".." );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle3()
        {
            CheckIO( ".", "." );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle4()
        {
            CheckIO( "x" );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle5()
        {
            CheckIO( "xx" );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle6()
        {
            CheckIO( "x", "x" );
        }

        [TestMethod]
        [TestCategory( "Puzzle IO" )]
        public void WriteThenReadYieldsEqualPuzzle7()
        {
            CheckIO( ".", "x" );
        }

        private void CheckIO(params string[] rowStrings)
        {
            CheckIO( Puzzle.FromRowStrings( rowStrings ) );
        }

        private void CheckIO(Puzzle puzzle)
        {
            Puzzle result;

            using ( var memoryStream = new MemoryStream() )
            {
                var puzzleIO = new PuzzleSerializer();

                using ( var streamWriter = new StreamWriter( memoryStream, Encoding.UTF8, 32768, true ) )
                {
                    puzzleIO.Write( streamWriter, puzzle );
                }

                memoryStream.Seek( 0, SeekOrigin.Begin );

                using ( var streamReader = new StreamReader( memoryStream, Encoding.UTF8, false, 32768, true ) )
                {
                    result = puzzleIO.Read( streamReader );
                }
            }

            Assert.AreEqual<Puzzle>( puzzle, result );
        }
    }
}
