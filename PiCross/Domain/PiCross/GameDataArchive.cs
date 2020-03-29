using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiCross
{
    internal interface IGameDataArchive : IDisposable
    {
        IList<int> PuzzleLibraryUIDs { get; }

        IList<string> PlayerNames { get; }

        InMemoryDatabase.PuzzleLibraryEntry ReadPuzzleLibraryEntry( int id );

        InMemoryDatabase.PlayerProfile ReadPlayerProfile( string playerName );

        void UpdateLibraryEntry( InMemoryDatabase.PuzzleLibraryEntry entry );

        void UpdatePlayerProfile( InMemoryDatabase.PlayerProfile playerProfile );
    }

    internal class GameDataArchive : IGameDataArchive
    {
        private readonly ZipArchive zipArchive;

        public GameDataArchive( ZipArchive zipArchive )
        {
            this.zipArchive = zipArchive;
        }

        private Puzzle ReadPuzzle( StreamReader streamReader )
        {
            return new PuzzleSerializer().Read( streamReader );
        }

        public IList<int> PuzzleLibraryUIDs
        {
            get
            {
                return (from entry in this.zipArchive.Entries
                        let uid = ExtractEntryID( entry.FullName )
                        where uid.HasValue
                        select uid.Value).ToList();
            }
        }

        public IList<string> PlayerNames
        {
            get
            {
                return (from entry in this.zipArchive.Entries
                        let playerName = ExtractPlayerName( entry.FullName )
                        where playerName != null
                        select playerName).ToList();
            }
        }

        public InMemoryDatabase.PuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            var path = GetLibraryEntryPath( id );

            using ( var reader = OpenZipArchiveEntryForReading( path ) )
            {
                var author = reader.ReadLine();
                var puzzle = ReadPuzzle( reader );

                return new InMemoryDatabase.PuzzleLibraryEntry( id, puzzle, author );
            }
        }

        public InMemoryDatabase.PlayerProfile ReadPlayerProfile( string playerName )
        {
            var path = GetPlayerProfilePath( playerName );

            using ( var reader = OpenZipArchiveEntryForReading( path ) )
            {
                var playerProfile = new InMemoryDatabase.PlayerProfile( playerName );
                var entryCount = int.Parse( reader.ReadLine() );

                for ( var i = 0; i != entryCount; ++i )
                {
                    var line = reader.ReadLine();
                    var parts = line.Split( ' ' );
                    var uid = int.Parse( parts[0] );
                    var bestTime = long.Parse( parts[1] );

                    playerProfile[uid].BestTime = TimeSpan.FromTicks( bestTime );
                }

                return playerProfile;
            }
        }

        public void UpdateLibraryEntry( InMemoryDatabase.PuzzleLibraryEntry entry )
        {
            var path = GetLibraryEntryPath( entry.UID );

            using ( var writer = OpenZipArchiveEntryForWriting( path ) )
            {
                writer.WriteLine( entry.Author );
                new PuzzleSerializer().Write( writer, entry.Puzzle );
            }
        }

        public void UpdatePlayerProfile( InMemoryDatabase.PlayerProfile playerProfile )
        {
            var path = GetPlayerProfilePath( playerProfile.Name );

            using ( var writer = OpenZipArchiveEntryForWriting( path ) )
            {
                var pairs = (from uid in playerProfile.EntryUIDs
                             let entry = playerProfile[uid]
                             where entry.BestTime.HasValue
                             select new { ID = uid, BestTime = entry.BestTime.Value }).ToList();

                writer.WriteLine( pairs.Count );

                foreach ( var pair in pairs )
                {
                    writer.WriteLine( "{0} {1}", pair.ID, pair.BestTime.Ticks );
                }
            }
        }

        private StreamReader OpenZipArchiveEntryForReading( string path )
        {
            return new StreamReader( OpenZipArchive( path ) );
        }

        private StreamWriter OpenZipArchiveEntryForWriting( string path )
        {
            return new StreamWriter( OpenZipArchive( path ) );
        }

        private Stream OpenZipArchive( string path )
        {
            return (zipArchive.GetEntry( path ) ?? CreateZipArchive( path )).Open();
        }

        private ZipArchiveEntry CreateZipArchive( string path )
        {
            return zipArchive.CreateEntry( path, CompressionLevel.Optimal );
        }

        private static string GetLibraryEntryPath( int id )
        {
            return $"library/entry{id.ToString().PadLeft( 5, '0' )}.txt";
        }

        private static string GetPlayerProfilePath( string playerName )
        {
            return $"players/{playerName}.txt";
        }

        private static int? ExtractEntryID( string filename )
        {
            var regex = new Regex( @"^library/entry(\d+)\.txt$" );
            var match = regex.Match( filename );

            if ( match.Success )
            {
                return int.Parse( match.Groups[1].Value );
            }
            else
            {
                return null;
            }
        }

        private static string ExtractPlayerName( string filename )
        {
            var regex = new Regex( @"^players/(.*)\.txt$" );
            var match = regex.Match( filename );

            if ( match.Success )
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }

        public void Dispose()
        {
            this.zipArchive.Dispose();
        }
    }

    internal class AutoCloseGameDataArchive : IGameDataArchive
    {
        private readonly string path;

        public AutoCloseGameDataArchive( string path )
        {
            this.path = path;
        }

        private ZipArchive OpenZipArchiveForReading()
        {
            return new ZipArchive( new FileStream( path, FileMode.Open, FileAccess.Read ), ZipArchiveMode.Read );
        }

        private ZipArchive OpenZipArchiveForWriting()
        {
            return new ZipArchive( new FileStream( path, FileMode.Open, FileAccess.ReadWrite ), ZipArchiveMode.Update );
        }

        private void WithReadOnlyZipArchive( Action<ZipArchive> action )
        {
            using ( var zipArchive = OpenZipArchiveForReading() )
            {
                action( zipArchive );
            }
        }

        private T WithReadOnlyZipArchive<T>( Func<ZipArchive, T> function )
        {
            using ( var zipArchive = OpenZipArchiveForReading() )
            {
                return function( zipArchive );
            }
        }

        private void WithWriteableZipArchive( Action<ZipArchive> action )
        {
            using ( var zipArchive = OpenZipArchiveForWriting() )
            {
                action( zipArchive );
            }
        }

        private T WithWriteableZipArchive<T>( Func<ZipArchive, T> function )
        {
            using ( var zipArchive = OpenZipArchiveForWriting() )
            {
                return function( zipArchive );
            }
        }

        private void WithReadOnlyArchive( Action<GameDataArchive> action )
        {
            WithReadOnlyZipArchive( archive => action( new GameDataArchive( archive ) ) );
        }

        private T WithReadOnlyArchive<T>( Func<GameDataArchive, T> function )
        {
            return WithReadOnlyZipArchive( archive => function( new GameDataArchive( archive ) ) );
        }

        private void WithWriteableArchive( Action<GameDataArchive> action )
        {
            WithWriteableZipArchive( archive => action( new GameDataArchive( archive ) ) );
        }

        private T WithWriteableArchive<T>( Func<GameDataArchive, T> function )
        {
            return WithWriteableZipArchive( archive => function( new GameDataArchive( archive ) ) );
        }

        public IList<int> PuzzleLibraryUIDs
        {
            get
            {
                return WithReadOnlyArchive( archive => archive.PuzzleLibraryUIDs );
            }
        }

        public IList<string> PlayerNames
        {
            get
            {
                return WithReadOnlyArchive( archive => archive.PlayerNames );
            }
        }

        public InMemoryDatabase.PuzzleLibraryEntry ReadPuzzleLibraryEntry( int id )
        {
            return WithReadOnlyArchive( archive => archive.ReadPuzzleLibraryEntry( id ) );
        }

        public InMemoryDatabase.PlayerProfile ReadPlayerProfile( string playerName )
        {
            return WithReadOnlyArchive( archive => archive.ReadPlayerProfile( playerName ) );
        }

        public void UpdateLibraryEntry( InMemoryDatabase.PuzzleLibraryEntry entry )
        {
            WithWriteableArchive( archive => archive.UpdateLibraryEntry( entry ) );
        }

        public void UpdatePlayerProfile( InMemoryDatabase.PlayerProfile playerProfile )
        {
            WithWriteableArchive( archive => archive.UpdatePlayerProfile( playerProfile ) );
        }

        public void Dispose()
        {
            // BOP
        }
    }
}
