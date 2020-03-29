using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleCreationTests
    {
        [TestMethod]
        [TestCategory( "Puzzle Creation" )]
        public void FromStringConstraints()
        {
            // x.x
            // .x.
            // ...
            var puzzle = Puzzle.FromConstraints( columnConstraints: "1;1;1", rowConstraints: "1 1;1;" );
            var columnConstraints = puzzle.ColumnConstraints.Select( c => c.Values.ToArray() ).ToArray();
            var rowConstraints = puzzle.RowConstraints.Select( c => c.Values.ToArray() ).ToArray();

            Assert.AreEqual( 3, columnConstraints.Length );
            Assert.AreEqual( 3, rowConstraints.Length );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[0], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[1], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[2], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[0], new int[] { 1, 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[1], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[2], new int[] { } ) );
        }

        [TestMethod]
        [TestCategory( "Puzzle Creation" )]
        public void FromRowStrings()
        {
            var puzzle = Puzzle.FromRowStrings(
                    ".....",
                    ".x...",
                    ".xx..",
                    "x.xx.",
                    "..xxx"
                );
            var columnConstraints = puzzle.ColumnConstraints.Select( c => c.Values.ToArray() ).ToArray();
            var rowConstraints = puzzle.RowConstraints.Select( c => c.Values.ToArray() ).ToArray();

            Assert.AreEqual( 5, columnConstraints.Length );
            Assert.AreEqual( 5, rowConstraints.Length );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[0], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[1], new int[] { 2 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[2], new int[] { 3 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[3], new int[] { 2 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( columnConstraints[4], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[0], new int[] { } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[1], new int[] { 1 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[2], new int[] { 2 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[3], new int[] { 1, 2 } ) );
            Assert.IsTrue( Enumerable.SequenceEqual( rowConstraints[4], new int[] { 3 } ) );
        }
    }
}
