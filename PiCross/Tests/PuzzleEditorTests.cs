using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cells;
using DataStructures;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]
    public class PuzzleEditorTests : TestBase
    {
        [TestMethod]
        [TestCategory( "PuzzleEditor" )]
        public void Initialization()
        {
            var editor = CreatePuzzleEditor(
                "..x",
                "xxx",
                ".x."
                );

            Assert.AreEqual( 3, editor.Grid.Size.Width );
            Assert.AreEqual( 3, editor.Grid.Size.Height );

            Assert.AreEqual( false, editor.Grid[new Vector2D( 0, 0 )].IsFilled.Value );
            Assert.AreEqual( false, editor.Grid[new Vector2D( 1, 0 )].IsFilled.Value );
            Assert.AreEqual( true, editor.Grid[new Vector2D( 2, 0 )].IsFilled.Value );

            Assert.AreEqual( true, editor.Grid[new Vector2D( 0, 1 )].IsFilled.Value );
            Assert.AreEqual( true, editor.Grid[new Vector2D( 1, 1 )].IsFilled.Value );
            Assert.AreEqual( true, editor.Grid[new Vector2D( 2, 1 )].IsFilled.Value );

            Assert.AreEqual( false, editor.Grid[new Vector2D( 0, 2 )].IsFilled.Value );
            Assert.AreEqual( true, editor.Grid[new Vector2D( 1, 2 )].IsFilled.Value );
            Assert.AreEqual( false, editor.Grid[new Vector2D( 2, 2 )].IsFilled.Value );

            Assert.AreEqual( CreateConstraints( 1 ), editor.RowConstraints[0].Constraints.Value );
            Assert.AreEqual( CreateConstraints( 3 ), editor.RowConstraints[1].Constraints.Value );
            Assert.AreEqual( CreateConstraints( 1 ), editor.RowConstraints[2].Constraints.Value );

            Assert.AreEqual( CreateConstraints( 1 ), editor.ColumnConstraints[0].Constraints.Value );
            Assert.AreEqual( CreateConstraints( 2 ), editor.ColumnConstraints[1].Constraints.Value );
            Assert.AreEqual( CreateConstraints( 2 ), editor.ColumnConstraints[2].Constraints.Value );
        }

        [TestMethod]
        [TestCategory( "PuzzleEditor" )]
        public void SquareObserversGetNotifiedOnContentsChange()
        {
            var editor = CreatePuzzleEditor(
                "..x",
                "xxx",
                ".x."
                );

            var square = editor.Grid[new Vector2D( 0, 0 )];
            var flag = Flag.Create( square.IsFilled );

            Assert.IsFalse( flag.Status );
            square.IsFilled.Value = true;
            Assert.IsTrue( flag.Status );
        }

        [TestMethod]
        [TestCategory( "PuzzleEditor" )]
        public void SquareObserversDoNotGetNotifiedWhenChangedToSameValue()
        {
            var editor = CreatePuzzleEditor(
                "..x",
                "xxx",
                ".x."
                );

            var square = editor.Grid[new Vector2D( 0, 0 )];
            var flag = Flag.Create( square.IsFilled );

            Assert.IsFalse( flag.Status );
            square.IsFilled.Value = false;
            Assert.IsFalse( flag.Status );
        }

        [TestMethod]
        [TestCategory( "PuzzleEditor" )]
        public void RowConstraintsObserversGetNotifiedOnChange()
        {
            var editor = CreatePuzzleEditor(
                "..x",
                "xxx",
                ".x."
                );

            var square = editor.Grid[new Vector2D( 0, 0 )];
            var flag = Flag.Create( editor.RowConstraints[0].Constraints );

            Assert.IsFalse( flag.Status );
            square.IsFilled.Value = true;
            Assert.IsTrue( flag.Status );
        }

        [TestMethod]
        [TestCategory( "PuzzleEditor" )]
        public void ColumnConstraintsObserversGetNotifiedOnChange()
        {
            var editor = CreatePuzzleEditor(
                "..x",
                "xxx",
                ".x."
                );

            var square = editor.Grid[new Vector2D( 0, 0 )];
            var flag = Flag.Create( editor.ColumnConstraints[0].Constraints );

            Assert.IsFalse( flag.Status );
            square.IsFilled.Value = true;
            Assert.IsTrue( flag.Status );
        }
    }
}
