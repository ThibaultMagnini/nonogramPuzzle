using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;
using DataStructures;
using System.Linq;

namespace PiCross.Tests
{
    [TestClass]
    public class PlayGridTests : TestBase
    {
        [TestMethod]
        [TestCategory( "PlayGrid" )]
        [Description( "All squares are set to Square.UNKNOWN during initialization" )]
        public void AllSquaresUnknownAfterCreation()
        {
            var playGrid = CreatePlayGrid(
                "x.",
                "x." );

            Assert.IsTrue( playGrid.Squares.Items.All( x => x.Value == Square.UNKNOWN ) );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        [Description( "All constraints must be unsatisfied" )]
        public void AllConstraintsAreUnsatisfiedAfterCreation()
        {
            var playGrid = CreatePlayGrid(
                "x.",
                "x." );

            foreach ( var constraint in playGrid.ColumnConstraints.Items.Concat( playGrid.RowConstraints.Items ) )
            {
                Assert.IsFalse( constraint.IsSatisfied );

                foreach ( var value in constraint.Values.Items )
                {
                    Assert.IsFalse( value.IsSatisfied );
                }
            }
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation1()
        {
            var playGrid = CreatePlayGrid(
                "X ",
                "x." );

            Assert.IsFalse( playGrid.ColumnConstraints[0].IsSatisfied );
            Assert.IsFalse( playGrid.ColumnConstraints[1].IsSatisfied );

            Assert.IsTrue( playGrid.RowConstraints[0].IsSatisfied );
            Assert.IsFalse( playGrid.RowConstraints[1].IsSatisfied );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation2()
        {
            var playGrid = CreatePlayGrid(
                "X XX xxx.x",
                "x........." );

            CheckSatisfaction( playGrid.ColumnConstraints, "ffffffffff" );
            CheckSatisfaction( playGrid.RowConstraints, "ff" );

            CheckSatisfactions( playGrid.RowConstraints, "ttff", "f" );
            CheckSatisfactions( playGrid.ColumnConstraints, "f", "", "f", "f", "", "f", "f", "f", "", "f" );
        }        

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation3()
        {
            var playGrid = CreatePlayGrid(
                "x. XX",
                "x.. .",
                "...  ",
                "X..X ",
                "X..XX" );

            CheckSatisfaction( playGrid.ColumnConstraints, "ffftf" );
            CheckSatisfaction( playGrid.RowConstraints, "fffff" );

            CheckSatisfactions( playGrid.RowConstraints, "ft", "f", "", "ff", "ff" );
            CheckSatisfactions( playGrid.ColumnConstraints, "ff", "", "", "tt", "ft" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation4()
        {
            var playGrid = CreatePlayGrid(
                "x"
                );

            CheckSatisfaction( playGrid.ColumnConstraints, "f" );
            CheckSatisfaction( playGrid.RowConstraints, "f" );

            CheckSatisfactions( playGrid.RowConstraints, "f" );
            CheckSatisfactions( playGrid.ColumnConstraints, "f" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation5()
        {
            var playGrid = CreatePlayGrid(
                "X"
                );

            CheckSatisfaction( playGrid.ColumnConstraints, "t" );
            CheckSatisfaction( playGrid.RowConstraints, "t" );

            CheckSatisfactions( playGrid.RowConstraints, "t" );
            CheckSatisfactions( playGrid.ColumnConstraints, "t" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation6()
        {
            var playGrid = CreatePlayGrid(
                "x.x"
                );

            OverwritePlayGrid( playGrid, "x.." );

            CheckSatisfaction( playGrid.ColumnConstraints, "ttf" );
            CheckSatisfaction( playGrid.RowConstraints, "f" );

            CheckSatisfactions( playGrid.RowConstraints, "tf" );
            CheckSatisfactions( playGrid.ColumnConstraints, "t", "", "f" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation7()
        {
            var playGrid = CreatePlayGrid( "X XX xxx.x" );

            CheckSatisfactions( playGrid.RowConstraints, "ttff" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation8()
        {
            var playGrid = CreatePlayGrid( "X XX x" );

            CheckSatisfactions( playGrid.RowConstraints, "ttf" );
        }

        [TestMethod]
        [TestCategory( "PlayGrid" )]
        public void ConstraintSatisfation9()
        {
            var playGrid = CreatePlayGrid( "X x" );

            CheckSatisfactions( playGrid.RowConstraints, "tf" );
        }

        private static void CheckSatisfaction( ISequence<PlayGridConstraints> constraints, string expected )
        {
            Assert.AreEqual( CreateBooleans( expected ), constraints.Map( c => c.IsSatisfied ) );
        }

        private static void CheckSatisfactions( ISequence<PlayGridConstraints> constraints, params string[] expecteds )
        {
            Assert.AreEqual( constraints.Length, expecteds.Length );

            foreach ( var i in constraints.Indices )
            {
                var actual = constraints[i];
                var expected = expecteds[i];

                Assert.AreEqual( CreateBooleans( expected ), actual.Values.Map( x => x.IsSatisfied ) );
            }
        }        
    }
}
