using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PiCross.Tests
{
    [TestClass]
    public class AmbiguityCheckerTests : TestBase
    {
        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous1()
        {
            AssertUnambiguous( "." );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous2()
        {
            AssertUnambiguous( "x" );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous3()
        {
            AssertUnambiguous( "..." );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous4()
        {
            AssertUnambiguous( "x.." );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous5()
        {
            AssertUnambiguous(
                "...",
                "..."
                );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous6()
        {
            AssertUnambiguous(
                "x.x",
                ".x."
                );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Unambiguous7()
        {
            AssertUnambiguous(
                "x.x",
                ".x.",
                "..."
                );
        }

        [TestMethod]
        [TestCategory( "AmbiguityChecker" )]
        public void Ambiguous()
        {
            AssertAmbiguous(
                "x.",
                ".x"                
                );
        }
        
        private void AssertAmbiguous( params string[] rows )
        {
            AssertAmbiguity( true, rows );
        }

        private void AssertUnambiguous( params string[] rows )
        {
            AssertAmbiguity( false, rows );
        }

        private void AssertAmbiguity( bool expected, string[] rows )
        {
            var checker = CreateAmbiguityChecker( rows );
            checker.Resolve();

            Assert.AreEqual( expected, checker.IsAmbiguous );
        }
    }
}
