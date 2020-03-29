using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_Generate
    {
        [TestMethod]
        public void Generate1()
        {
            Check( new int[] { }, "??", ".." );
        }

        [TestMethod]
        public void Generate2()
        {
            Check( new int[] { }, "..", ".." );
        }

        [TestMethod]
        public void Generate3()
        {
            Check( new int[] { }, "x." );
        }

        [TestMethod]
        public void Generate4()
        {
            Check( new int[] { 1 }, "??", "x.", ".x" );
        }

        [TestMethod]
        public void Generate5()
        {
            Check( new int[] { 1 }, "x?", "x." );
        }

        [TestMethod]
        public void Generate6()
        {
            Check( new int[] { 1 }, ".?", ".x" );
        }

        [TestMethod]
        public void Generate7()
        {
            Check( new int[] { 1 }, ".x", ".x" );
        }
        
        [TestMethod]
        public void Generate8()
        {
            Check( new int[] { 1 }, ".." );
        }

        [TestMethod]
        public void Generate9()
        {
            Check( new int[] { 2 }, "??", "xx" );
        }

        [TestMethod]
        public void Generate10()
        {
            Check( new int[] { 2 }, "xx", "xx" );
        }

        [TestMethod]
        public void Generate11()
        {
            Check( new int[] { 1, 1 }, "???", "x.x" );
        }

        [TestMethod]
        public void Generate12()
        {
            Check( new int[] { 2, 1 }, "???" );
        }

        [TestMethod]
        public void Generate13()
        {
            Check( new int[] { 1, 1 }, "????", "x.x.", "x..x", ".x.x" );
        }

        [TestMethod]
        public void Generate14()
        {
            Check( new int[] { 2, 1 }, "?????", "xx.x.", "xx..x", ".xx.x" );
        }

        [TestMethod]
        public void Generate15()
        {
            Check( new int[] { 2, 1 }, "???.?", "xx..x", ".xx.x" );
        }

        private void Check( int[] constraintValues, string compatibleWithString, params string[] expectedStrings )
        {
            var constraints = Constraints.FromValues( constraintValues );
            var compatibleWith = Sequence.FromString( compatibleWithString ).Map( Square.FromSymbol );
            var expected = expectedStrings.Select( s => Sequence.FromString( s ).Map( c => c == 'x' ) );
            var actual = CollectSlices( constraints, compatibleWith );

            foreach ( var x in expected )
            {
                Assert.IsTrue( actual.Contains( x ), "{0} missing", x );
            }

            foreach ( var x in actual )
            {
                Assert.IsTrue( expected.Contains( x ), "{0} is redundant", x );
            }
        }

        private ISet<ISequence<bool>> CollectSlices( Constraints constraints, ISequence<Square> compatibleWith )
        {
            var result = new HashSet<ISequence<bool>>();

            constraints.Generate( bs => result.Add( Sequence.FromItems( bs ) ), compatibleWith );

            return result;
        }
    }
}
