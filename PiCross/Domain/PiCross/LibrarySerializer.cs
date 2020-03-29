using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiCross;
using IO;

namespace PiCross
{
    internal class LibrarySerializer : ISerializer<InMemoryDatabase.PuzzleLibrary>
    {
        private readonly ISerializer<InMemoryDatabase.PuzzleLibraryEntry> libraryEntrySerializer;

        public LibrarySerializer()
        {
            libraryEntrySerializer = new LibraryEntrySerializer( new PuzzleSerializer() );
        }

        public void Write( StreamWriter writer, InMemoryDatabase.PuzzleLibrary obj )
        {
            new Writer( writer, obj, libraryEntrySerializer ).Write();
        }

        public InMemoryDatabase.PuzzleLibrary Read( StreamReader reader )
        {
            return new Reader( reader, libraryEntrySerializer ).Read();
        }

        private class Reader : ReaderBase
        {
            private readonly ISerializer<InMemoryDatabase.PuzzleLibraryEntry> libraryEntrySerializer;

            public Reader( StreamReader streamReader, ISerializer<InMemoryDatabase.PuzzleLibraryEntry> libraryEntrySerializer )
                : base( streamReader )
            {
                if ( libraryEntrySerializer == null )
                {
                    throw new ArgumentNullException( nameof( libraryEntrySerializer ) );
                }
                else
                {
                    this.libraryEntrySerializer = libraryEntrySerializer;
                }
            }

            public InMemoryDatabase.PuzzleLibrary Read()
            {
                var count = ReadInteger();
                var library = InMemoryDatabase.PuzzleLibrary.CreateEmpty();

                for ( var i = 0; i != count; ++i )
                {
                    var libraryEntry = libraryEntrySerializer.Read( streamReader );
                    library.Entries.Add( libraryEntry );
                }

                return library;
            }
        }

        private class Writer : WriterBase
        {
            private readonly ISerializer<InMemoryDatabase.PuzzleLibraryEntry> libraryEntrySerializer;

            private readonly InMemoryDatabase.PuzzleLibrary library;

            public Writer( StreamWriter streamWriter, InMemoryDatabase.PuzzleLibrary library, ISerializer<InMemoryDatabase.PuzzleLibraryEntry> libraryEntrySerializer )
                : base( streamWriter )
            {
                if ( library == null )
                {
                    throw new ArgumentNullException( nameof( library ) );
                }
                else if ( libraryEntrySerializer == null )
                {
                    throw new ArgumentNullException( nameof( libraryEntrySerializer ) );
                }
                else
                {
                    this.library = library;
                    this.libraryEntrySerializer = libraryEntrySerializer;
                }
            }

            public void Write()
            {
                streamWriter.WriteLine( library.Entries.Count );

                foreach ( var libraryEntry in library.Entries )
                {
                    libraryEntrySerializer.Write( streamWriter, (InMemoryDatabase.PuzzleLibraryEntry) libraryEntry );
                }
            }
        }
    }

    internal class LibraryEntrySerializer : ISerializer<InMemoryDatabase.PuzzleLibraryEntry>
    {
        private readonly ISerializer<Puzzle> puzzleSerializer;

        internal LibraryEntrySerializer( ISerializer<Puzzle> puzzleSerializer )
        {
            if ( puzzleSerializer == null )
            {
                throw new ArgumentNullException( nameof( puzzleSerializer ) );
            }
            else
            {
                this.puzzleSerializer = puzzleSerializer;
            }
        }

        public void Write( StreamWriter streamWriter, InMemoryDatabase.PuzzleLibraryEntry entry )
        {
            new Writer( streamWriter, entry, puzzleSerializer ).Write();
        }

        public InMemoryDatabase.PuzzleLibraryEntry Read( StreamReader streamReader )
        {
            return new Reader( streamReader, puzzleSerializer ).Read();
        }

        private class Writer : WriterBase
        {
            private readonly InMemoryDatabase.PuzzleLibraryEntry libraryEntry;

            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Writer( StreamWriter streamWriter, InMemoryDatabase.PuzzleLibraryEntry libraryEntry, ISerializer<Puzzle> puzzleSerializer )
                : base( streamWriter )
            {
                if ( libraryEntry == null )
                {
                    throw new ArgumentNullException( nameof( libraryEntry ) );
                }
                else if ( puzzleSerializer == null )
                {
                    throw new ArgumentNullException( nameof( puzzleSerializer ) );
                }
                else
                {
                    this.libraryEntry = libraryEntry;
                    this.puzzleSerializer = puzzleSerializer;
                }
            }

            internal void Write()
            {
                streamWriter.WriteLine( libraryEntry.UID );
                streamWriter.WriteLine( libraryEntry.Author );
                puzzleSerializer.Write( streamWriter, libraryEntry.Puzzle );
            }
        }

        private class Reader : ReaderBase
        {
            private readonly ISerializer<Puzzle> puzzleSerializer;

            internal Reader( StreamReader streamReader, ISerializer<Puzzle> puzzleSerializer ) : base( streamReader )
            {
                if ( puzzleSerializer == null )
                {
                    throw new ArgumentNullException( nameof( puzzleSerializer ) );
                }
                else
                {
                    this.puzzleSerializer = puzzleSerializer;
                }
            }

            internal InMemoryDatabase.PuzzleLibraryEntry Read()
            {
                var uid = ReadInteger();
                var author = streamReader.ReadLine();
                var puzzle = puzzleSerializer.Read( streamReader );

                return new InMemoryDatabase.PuzzleLibraryEntry( uid, puzzle, author );
            }
        }
    }
}
