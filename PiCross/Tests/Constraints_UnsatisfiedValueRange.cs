using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_UnsatisfiedValueRange : TestBase
    {
        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange1()
        {
            var slice = CreateSlice( "x" );
            var constraints = CreateConstraints( 1 );
            var expected = Range.FromStartAndEndExclusive( 1, 1 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange2()
        {
            var slice = CreateSlice( "?" );
            var constraints = CreateConstraints( 1 );
            var expected = Range.FromStartAndEndExclusive( 0, 1 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange3()
        {
            var slice = CreateSlice( "x..x" );
            var constraints = CreateConstraints( 1, 1 );
            var expected = Range.FromStartAndEndExclusive( 2, 2 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange4()
        {
            var slice = CreateSlice( "x.?.x" );
            var constraints = CreateConstraints( 1, 1 );
            var expected = Range.FromStartAndEndExclusive( 1, 1 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange5()
        {
            var slice = CreateSlice( "...x.x" );
            var constraints = CreateConstraints( 1, 1, 1 );
            var expected = Range.FromStartAndEndExclusive( 2, 3 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange6()
        {
            var slice = CreateSlice( "x?x" );
            var constraints = CreateConstraints( 1, 1 );
            var expected = Range.FromStartAndEndExclusive( 0, 2 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange7()
        {
            var slice = CreateSlice( "x?x" );
            var constraints = CreateConstraints( 3 );
            var expected = Range.FromStartAndEndExclusive( 0, 1 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange8()
        {
            var slice = CreateSlice( "x.xx.?????" );
            var constraints = CreateConstraints( 1, 2, 3, 1 );
            var expected = Range.FromStartAndEndExclusive( 2, 4 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void UnsatisfiedValueRange9()
        {
            var slice = CreateSlice( "x........." );
            var constraints = CreateConstraints( 1 );
            var expected = Range.FromStartAndEndExclusive( 1, 1 );
            var actual = constraints.UnsatisfiedValueRange( slice );

            Assert.AreEqual( expected, actual );
        }
    }
}
