using System;
using System.Linq;
using DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_Superposition
    {
        [TestMethod]
        public void Superposition1()
        {
            Check( new int[] { 1 }, "???", "???" );
        }

        [TestMethod]
        public void Superposition2()
        {
            Check( new int[] { 2 }, "???", "?x?" );
        }

        [TestMethod]
        public void Superposition3()
        {
            Check( new int[] { 1, 1 }, "???", "x.x" );
        }
        
        private void Check(int[] constraintValues, string compatibleWithString, string expectedString)
        {
            var constraints = Constraints.FromValues( constraintValues );
            var compatibleWith = Sequence.FromString( compatibleWithString ).Map( Square.FromSymbol );
            var expected = Sequence.FromString( expectedString ).Map( Square.FromSymbol );
            var actual = constraints.Superposition( compatibleWith );

            Assert.AreEqual( expected, actual );            
        }
    }
}
