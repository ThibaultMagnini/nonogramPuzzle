using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cells;
using DataStructures;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]
    public class Facade_Puzzle : TestBase
    {
        [TestMethod]
        [TestCategory( "Facade/Puzzle" )]
        public void ContentsChangeNotification()
        {
            var puzzle = CreateManualPuzzle( "." );
            var square = puzzle.Grid[new Vector2D( 0, 0 )];

            var flag = Flag.Create( square.Contents );

            Assert.IsFalse( flag.Status );
            Assert.AreSame( Square.UNKNOWN, square.Contents.Value );

            square.Contents.Value = Square.EMPTY;

            Assert.IsTrue( flag.Status );
            Assert.AreSame( Square.EMPTY, square.Contents.Value );
        }

        [TestMethod]
        [TestCategory( "Facade/Puzzle" )]
        public void ConstraintsSatisfactionChangeNotification()
        {
            var puzzle = CreateManualPuzzle( "x" );
            var square = puzzle.Grid[new Vector2D( 0, 0 )];
            var constraint = puzzle.RowConstraints[0];

            var flag = Flag.Create( constraint.IsSatisfied );

            Assert.IsFalse( flag.Status );
            Assert.IsFalse( constraint.IsSatisfied.Value );            

            square.Contents.Value = Square.FILLED;

            Assert.IsTrue( flag.Status );
            Assert.IsTrue( constraint.IsSatisfied.Value );
        }

        [TestMethod]
        [TestCategory( "Facade/Puzzle" )]
        public void ConstraintValueSatisfactionChangeNotification()
        {
            var puzzle = CreateManualPuzzle( "x" );
            var square = puzzle.Grid[new Vector2D( 0, 0 )];
            var constraints = puzzle.RowConstraints[0];
            var value = constraints.Values[0];

            var flag = Flag.Create( constraints.Values[0].IsSatisfied );

            Assert.IsFalse( flag.Status );
            Assert.IsFalse( value.IsSatisfied.Value );

            square.Contents.Value = Square.FILLED;

            Assert.IsTrue( flag.Status );
            Assert.IsTrue( value.IsSatisfied.Value );
        }
    }
}
