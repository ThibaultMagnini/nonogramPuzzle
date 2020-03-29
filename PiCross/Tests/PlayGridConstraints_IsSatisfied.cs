using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class PlayGridConstraints_IsSatisfied : TestBase
    {
        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied1()
        {
            var pgc = CreatePGConstraints( "???", 1, 1 );

            Check( pgc, "ff" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied2()
        {
            var pgc = CreatePGConstraints( "x.x", 1, 1 );

            Check( pgc, "tt" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied3()
        {
            var pgc = CreatePGConstraints( "x?x", 1, 1 );

            Check( pgc, "ff" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied4()
        {
            var pgc = CreatePGConstraints( "x?.x", 1, 1 );

            Check( pgc, "ft" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied5()
        {
            var pgc = CreatePGConstraints( "x.?.xx", 1, 2 );

            Check( pgc, "tt" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied6()
        {
            var pgc = CreatePGConstraints( "x.xxx.???.xx", 1, 3, 1, 2 );

            Check( pgc, "ttft" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied7()
        {
            var pgc = CreatePGConstraints( "x.x.x", 1, 1, 1 );

            Check( pgc, "ttt" );
        }

        [TestMethod]
        [TestCategory( "PlayGridConstraints" )]
        public void IsSatisfied8()
        {
            var pgc = CreatePGConstraints( "..x.x", 1, 1, 1 );

            Check( pgc, "ttf" );
        }

        private static PlayGridConstraints CreatePGConstraints( string str, params int[] constraints )
        {
            return new PlayGridConstraints( CreateSlice( str ), CreateConstraints( constraints ) );
        }

        private static void Check(PlayGridConstraints constraints, string expectedString)
        {
            var expected = CreateBooleans( expectedString );
            var actual = constraints.Values.Map( x => x.IsSatisfied );

            Assert.AreEqual( expected, actual );
        }
    }
}
