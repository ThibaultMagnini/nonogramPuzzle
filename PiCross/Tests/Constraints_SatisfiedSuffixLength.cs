using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_SatisfiedSuffixLength : TestBase
    {
        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest1()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "??x" );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 0;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest2()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "???" );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 0;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest3()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "?.x" );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest4()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "?.x." );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest5()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "?.xx." );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 0;

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void SatisfiedSuffixLengthTest6()
        {
            var constraints = CreateConstraints( 1, 1 );
            var slice = CreateSlice( "x?.x" );
            var actual = constraints.SatisfiedSuffixLength( slice );
            var expected = 1;

            Assert.AreEqual( expected, actual );
        }
    }
}
