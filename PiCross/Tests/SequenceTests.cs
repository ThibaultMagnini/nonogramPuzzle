using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class SequenceTests
    {
        [TestMethod]
        [TestCategory( "Sequence" )]
        public void CreateEmpty()
        {
            var seq = Sequence.CreateEmpty<int>();

            Assert.AreEqual( 0, seq.Length );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Concatenation()
        {
            var xs = Sequence.FromItems( 1, 2, 3 );
            var ys = Sequence.FromItems( 4, 5 );
            var actual = xs.Concatenate( ys );
            var expected = Sequence.FromItems( 1, 2, 3, 4, 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void TakeWhile()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.TakeWhile( x => x < 4 );
            var expected = Sequence.FromItems( 1, 2, 3 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void DropWhile1()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.DropWhile( x => x < 4 );
            var expected = Sequence.FromItems( 4, 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void DropWhile2()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.DropWhile( x => x < 6 );
            var expected = Sequence.FromItems<int>();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Prefix( 2 );
            var expected = Sequence.FromItems( 1, 2 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Suffix( 2 );
            var expected = Sequence.FromItems( 3, 4, 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Reverse()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var actual = seq.Reverse();
            var expected = Sequence.FromItems( 5, 4, 3, 2, 1 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix_Full()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = seq;
            var actual = seq.Prefix( 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix_Full()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = seq;
            var actual = seq.Suffix( 0 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Prefix_Empty()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = Sequence.CreateEmpty<int>();
            var actual = seq.Prefix( 0 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Suffix_Empty()
        {
            var seq = Sequence.FromItems( 1, 2, 3, 4, 5 );
            var expected = Sequence.CreateEmpty<int>();
            var actual = seq.Suffix( 5 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Flatten1()
        {
            var seq = Sequence.FromItems( Sequence.FromItems( 1, 2, 3 ), Sequence.FromItems( 4, 5, 6 ), Sequence.FromItems( 7, 8, 9 ) );
            var expected = Sequence.FromItems( 1, 2, 3, 4, 5, 6, 7, 8, 9 );
            var actual = seq.Flatten();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Flatten2()
        {
            var seq = Sequence.Range( 1, 10 ).Map( x => Sequence.FromItems( x ) );
            var expected = Sequence.Range( 1, 10 );
            var actual = seq.Flatten();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void ToByte1()
        {
            var seq = Sequence.FromItems( true );
            var expected = (byte) 0x80;
            var actual = seq.ToByte();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void ToByte2()
        {
            var seq = Sequence.FromItems( false );
            var expected = (byte) 0x00;
            var actual = seq.ToByte();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void ToByte3()
        {
            var seq = Sequence.FromItems( true, false );
            var expected = (byte) 0x80;
            var actual = seq.ToByte();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void ToByte4()
        {
            var seq = Sequence.FromItems( true, true );
            var expected = (byte) 0xC0;
            var actual = seq.ToByte();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void ToByte5()
        {
            var seq = Sequence.FromItems( true, true, true, true );
            var expected = (byte) 0xF0;
            var actual = seq.ToByte();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void GroupBits1()
        {
            var seq = BoolSeq( 1, 1, 1, 1, 1, 1, 1, 1 );
            var expected = Sequence.FromItems<byte>( 0xFF );
            var actual = seq.GroupBits();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void GroupBits2()
        {
            var seq = BoolSeq( 0, 0, 0, 0, 0, 0, 0, 0 );
            var expected = Sequence.FromItems<byte>( 0x00 );
            var actual = seq.GroupBits();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void GroupBits3()
        {
            var seq = BoolSeq( 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0 );
            var expected = Sequence.FromItems<byte>( 0xFF, 0x00 );
            var actual = seq.GroupBits();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void GroupBits4()
        {
            var seq = BoolSeq( 1, 1 );
            var expected = Sequence.FromItems<byte>( 0xC0 );
            var actual = seq.GroupBits();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void GroupBits5()
        {
            var seq = BoolSeq( 0, 0, 0, 0, 1, 1, 1, 1, 1 );
            var expected = Sequence.FromItems<byte>( 0x0F, 0x80 );
            var actual = seq.GroupBits();

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Bits1()
        {
            var actual = Sequence.Bits( 0 );
            var expected = BoolSeq( 0, 0, 0, 0, 0, 0, 0, 0 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Bits2()
        {
            var actual = Sequence.Bits( 1 );
            var expected = BoolSeq( 0, 0, 0, 0, 0, 0, 0, 1 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Bits3()
        {
            var actual = Sequence.Bits( 2 );
            var expected = BoolSeq( 0, 0, 0, 0, 0, 0, 1, 0 );

            Assert.AreEqual( expected, actual );
        }

        [TestMethod]
        [TestCategory( "Sequence" )]
        public void Bits4()
        {
            var actual = Sequence.Bits( 0xFF );
            var expected = BoolSeq( 1, 1, 1, 1, 1, 1, 1, 1 );

            Assert.AreEqual( expected, actual );
        }

        private ISequence<bool> BoolSeq( params int[] bits )
        {
            return Sequence.FromItems( bits ).Map( x => x == 1 );
        }
    }
}
