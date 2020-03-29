using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;

namespace PiCross.Tests
{
    [TestClass]
    public class Slice_DeriveConstraints : TestBase
    {
        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_E()
        {
            new Test() { Slice = ".", Expected = CreateConstraints() };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_EE()
        {
            new Test() { Slice = "..", Expected = CreateConstraints() };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_X()
        {
            new Test() { Slice = "x", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_UX()
        {
            new Test() { Slice = ".x", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XU()
        {
            new Test() { Slice = "x.", Expected = CreateConstraints( 1 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XX()
        {
            new Test() { Slice = "xx", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_UXX()
        {
            new Test() { Slice = ".xx", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XXU()
        {
            new Test() { Slice = "xx.", Expected = CreateConstraints( 2 ) };
        }

        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XXEXX()
        {
            new Test() { Slice = "xx.xx", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XXEXXE()
        {
            new Test() { Slice = "xx.xx.", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_EXXEXX()
        {
            new Test() { Slice = ".xx.xx", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_EXXEXXE()
        {
            new Test() { Slice = ".xx.xx.", Expected = CreateConstraints( 2, 2 ) };
        }


        [TestMethod]
        [TestCategory( "Slice" )]
        public void DeriveConstraints_XXEEEXX()
        {
            new Test() { Slice = "xx...xx", Expected = CreateConstraints( 2, 2 ) };
        }

        private class Test
        {
            private Slice slice;

            private Constraints expected;

            public string Slice
            {
                set
                {
                    this.slice = CreateSlice( value );
                }
            }

            public Constraints Expected
            {
                set
                {
                    this.expected = value;
                }
            }

            public void Check()
            {
                var actual = slice.DeriveConstraints();

                Assert.AreEqual( expected, actual );
            }
        }
    }
}
