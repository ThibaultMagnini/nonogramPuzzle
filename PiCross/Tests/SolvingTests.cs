using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class SolvingTests : TestBase
    {
        [TestMethod]
        [TestCategory("SolverGrid")]
        public void Solve1()
        {
            Check( "x" );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve2()
        {
            Check( "." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve3()
        {
            Check( ".." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve4()
        {
            Check( ".x" );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve5()
        {
            Check( "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve6()
        {
            Check( 
                "..",
                ".." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve7()
        {
            Check(
                "x.",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve8()
        {
            Check(
                "..",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve9()
        {
            Check(
                "xx",
                "x." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve10()
        {
            Check(
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                "..........",
                ".........." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve11()
        {
            Check(
                "..........",
                ".xxxxxxxx.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".x......x.",
                ".xxxxxxxx.",
                ".........." );
        }

        [TestMethod]
        [TestCategory( "SolverGrid" )]
        public void Solve12()
        {
            Check(
                ".xxx..xx..",
                "..xxx.....",
                "....xxxx..",
                "......xx..",
                "..xxxxx...",
                ".x.x.xxx.x",
                ".xxx...xxx",
                "...xxxxx..",
                "......xxx.",
                ".........." );
        }

        [TestMethod]
        [TestCategory("SolverGrid")]
        public void IsAmbiguous()
        {
            AssertAmbiguous(
                ".x",
                "x."
                );
        }


        private static void Check(params string[] rows)
        {
            var editorGrid = EditorGrid.FromStrings( rows );
            var solverGrid = editorGrid.CreateSolverGrid();

            solverGrid.Refine();

            Assert.AreEqual( editorGrid.Squares.Size, solverGrid.Squares.Size );

            foreach ( var position in solverGrid.Squares.AllPositions )
            {
                var expected = editorGrid.Squares[position];
                var actual = solverGrid.Squares[position];

                Assert.AreEqual( expected, actual, "Unequal squares at {0}", position );
            }
        }

        private static void AssertAmbiguous(params string[] rows)
        {
            var solverGrid = CreateSolverGrid( rows );

            solverGrid.Refine();

            Assert.IsTrue( solverGrid.Squares.Items.Any( x => x == Square.UNKNOWN ) );
        }
    }
}
