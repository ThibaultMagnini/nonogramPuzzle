using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    public interface IGameData
    {
        IPuzzleLibrary PuzzleLibrary { get; }

        IPlayerLibrary PlayerDatabase { get; }
    }

    /// <summary>
    /// Player Library.
    /// </summary>
    public interface IPlayerLibrary
    {
        /// <summary>
        /// Retrieves the profile of the player with the specified name.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        /// <returns>Player's profile.</returns>
        IPlayerProfile this[string name] { get; }

        /// <summary>
        /// Creates a new player profile.
        /// </summary>
        /// <param name="name">Player name.</param>
        /// <returns>The newly created player profile.</returns>
        IPlayerProfile CreateNewProfile( string name );

        /// <summary>
        /// Checks if the specified name is valid.
        /// </summary>
        /// <param name="name">Name to be verified.</param>
        /// <returns>True if the name is valid, false otherwise.</returns>
        bool IsValidPlayerName( string name );

        /// <summary>
        /// Enumerates all the players' names.
        /// </summary>
        IList<string> PlayerNames { get; }
    }

    /// <summary>
    /// Player's profile.
    /// </summary>
    public interface IPlayerProfile
    {
        /// <summary>
        /// Fetches player-specific information about the given puzzle.
        /// </summary>
        /// <param name="libraryEntry">Puzzle.</param>
        /// <returns></returns>
        IPlayerPuzzleInformation this[IPuzzleLibraryEntry libraryEntry] { get; }

        /// <summary>
        /// Name of the player whose profile this is.
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// An IPlayerPuzzleInformation object contains
    /// the information for one specific player
    /// about one specific puzzle.
    /// </summary>
    public interface IPlayerPuzzleInformation
    {
        /// <summary>
        /// Best time the player has achieved for the puzzle.
        /// TimeSpan is value type like int, so it cannot be null.
        /// We can make it nullable though by using TimeSpan?
        /// If this property is null, the player hasn't solved
        /// the puzzle yet.
        /// </summary>
        TimeSpan? BestTime { get; set; }
    }

    public interface IPuzzleLibrary
    {
        /// <summary>
        /// Returns a list of all puzzles in the library.
        /// Note that this returns an IEnumerable, which
        /// offers limited functionality. Use
        /// its ToList() to turn it into a list (make sure
        /// using System.Linq is present at the top of your source file.)
        /// </summary>
        IEnumerable<IPuzzleLibraryEntry> Entries { get; }

        /// <summary>
        /// Adds a new puzzle to the library.
        /// </summary>
        /// <param name="puzzle">Puzzle.</param>
        /// <param name="author">Author of the puzzle.</param>
        /// <returns>Entry that has been added to the library.</returns>
        IPuzzleLibraryEntry Create( Puzzle puzzle, string author );
    }

    /// <summary>
    /// A IPuzzleLibraryEntry is an object that
    /// pairs up a Puzzle and its author.
    /// </summary>
    public interface IPuzzleLibraryEntry
    {
        /// <summary>
        /// Puzzle.
        /// </summary>
        Puzzle Puzzle { get; set; }

        /// <summary>
        /// Author of the puzzle.
        /// </summary>
        string Author { get; set; }
    }
}
