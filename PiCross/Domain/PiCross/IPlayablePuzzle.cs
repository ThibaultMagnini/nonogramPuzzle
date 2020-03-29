using DataStructures;
using PiCross;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    /// <summary>
    /// Represents a playable puzzle.
    /// A playable puzzle offers the following functionality:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// A modifiable grid. Each square in the grid can take three values: UNKNOWN, FILLED and EMPTY (<see cref="Square" />.)
    /// The goal is for the player to fill in the grid correctly.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Row and colum constraints. These are all readonly.
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public interface IPlayablePuzzle
    {
        /// <summary>
        /// Grid of IPlayablePuzzleSquares.
        /// </summary>
        IGrid<IPlayablePuzzleSquare> Grid { get; }

        /// <summary>
        /// Column constraints.
        /// </summary>
        ISequence<IPlayablePuzzleConstraints> ColumnConstraints { get; }

        /// <summary>
        /// Row constraints.
        /// </summary>
        ISequence<IPlayablePuzzleConstraints> RowConstraints { get; }

        /// <summary>
        /// Contains true if the Grid contains no more unknown squares,
        /// and all squares are correct. This property is observable.
        /// </summary>
        Cell<bool> IsSolved { get; }

        /// <summary>
        /// Number of squares that are left unknown (i.e. have not been determined to be
        /// filled or empty.) This property is observable.
        /// </summary>
        Cell<int> UnknownCount { get; }

        /// <summary>
        /// True if there are unknowns left, false otherwise. Note:
        /// this does not correspond to a successfully solved puzzle.
        /// The player might have made mistakes. This property is observable.
        /// </summary>
        Cell<bool> ContainsUnknowns { get; }
    }

    public interface IPlayablePuzzleSquare
    {
        /// <summary>
        /// Contents of the square.
        /// </summary>
        Cell<Square> Contents { get; }

        /// <summary>
        /// Position of the square in the grid.
        /// </summary>
        Vector2D Position { get; }
    }

    public interface IPlayablePuzzleConstraints
    {
        /// <summary>
        /// Series of values.
        /// </summary>
        ISequence<IPlayablePuzzleConstraintsValue> Values { get; }

        /// <summary>
        /// Contains true if the corresponding row or column satisfies
        /// the pattern described by this object.
        /// </summary>
        Cell<bool> IsSatisfied { get; }
    }

    public interface IPlayablePuzzleConstraintsValue
    {
        /// <summary>
        /// Actual value.
        /// </summary>
        int Value { get; }

        /// <summary>
        /// Contains true if the corresponding row or column
        /// satisfies this particular value.
        /// </summary>
        Cell<bool> IsSatisfied { get; }
    }
}
