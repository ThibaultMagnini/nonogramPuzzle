using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PiCross.Facade.IO;

namespace PiCross.Tests
{
    [TestClass]
    public class LibraryIOTests : TestBase
    {
        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved1_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "." )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved2_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "x" )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved3_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( ".." )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved4_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "xx" )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved5_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle(
                "xx",
                "xx" )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved6_ReadableFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle(
                "x....x..x",
                "x.xxx.x..",
                ".x.xx.x..",
                "x...x..xx" )
                );

            CheckReadableFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved1_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "." )
                );

            CheckCondensedFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved2_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "x" )
                );

            CheckCondensedFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved3_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( ".." )
                );

            CheckCondensedFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved4_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle( "xx" )
                );

            CheckCondensedFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved5_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle(
                "xx",
                "xx" )
                );

            CheckCondensedFormatIO( library );
        }

        [TestMethod]
        [TestCategory( "LibraryIO" )]
        public void Load_Saved6_CondensedFormat()
        {
            var library = CreateLibrary(
                CreatePuzzle(
                "x....x..x",
                "x.xxx.x..",
                ".x.xx.x..",
                "x...x..xx" )
                );

            CheckCondensedFormatIO( library );
        }

        private void CheckReadableFormatIO( ILibrary library )
        {
            var io = new LibraryIO( new ReadableFormat() );

            CheckIO( library, io );
        }

        private void CheckCondensedFormatIO( ILibrary library )
        {
            var io = new LibraryIO( new CondensedFormat() );

            CheckIO( library, io );
        }

        private static void CheckIO( ILibrary library, LibraryIO io )
        {
            using ( var memoryStream = new MemoryStream() )
            {
                io.Save( library, memoryStream );
                memoryStream.Seek( 0, SeekOrigin.Begin );
                var loaded = io.Load( memoryStream );

                Assert.AreEqual( library.Entries.Count, loaded.Entries.Count );

                for ( var i = 0; i != library.Entries.Count; ++i )
                {
                    var expected = library.Entries[i];
                    var actual = loaded.Entries[i];

                    Assert.AreEqual( expected, actual, string.Format( "Puzzle #{0} not saved/loaded correctly", i ) );
                }
            }
        }
    }
}
