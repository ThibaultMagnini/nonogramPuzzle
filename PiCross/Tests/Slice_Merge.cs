using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]   
    public class Slice_Merge : TestBase
    {
        [TestMethod]
        [TestCategory("Slice")]
        public void Merge_E_E()
        {
            Test( ".", ".", "." );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_X_X()
        {
            Test( "x", "x", "x" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_X_E()
        {
            Test( "x", ".", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_E_X()
        {
            Test( ".", "x", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_U_X()
        {
            Test( "?", "x", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_U_E()
        {
            Test( "?", ".", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_X_U()
        {
            Test( "x", "?", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_E_U()
        {
            Test( ".", "?", "?" );
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void Merge_XXXEEEUUU_XEUXEUXEU()
        {
            Test( "xxx...???", "x.?x.?x.?", "x???.????" );
        }

        private void Test(string sliceString1, string sliceString2, string expectedString)
        {
            var slice1 = CreateSlice( sliceString1 );
            var slice2 = CreateSlice( sliceString2 );
            var actual = slice1.Merge( slice2 );
            var expected = CreateSlice( expectedString );

            Assert.AreEqual( expected, actual );
        }
    }
}
