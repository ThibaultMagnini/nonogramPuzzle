using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]
    public class Slice_Refine : TestBase
    {
        [TestMethod]
        [TestCategory( "Slice" )]
        public void Refine_UU_1()
        {
            var slice = CreateSlice( "??" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( "??" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Refine_UU_2()
        {
            var slice = CreateSlice( "??" );
            var constraints = CreateConstraints( 2 );
            var expected = CreateSlice( "xx" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Refine_UX_1()
        {
            var slice = CreateSlice( "?x" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( ".x" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Refine_EU_1()
        {
            var slice = CreateSlice( ".?" );
            var constraints = CreateConstraints( 1 );
            var expected = CreateSlice( ".x" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Refine_UUU_2()
        {
            var slice = CreateSlice( "???" );
            var constraints = CreateConstraints( 2 );
            var expected = CreateSlice( "?x?" );
            var actual = slice.Refine( constraints );

            Assert.AreEqual( expected, actual );
        }
    }
}
