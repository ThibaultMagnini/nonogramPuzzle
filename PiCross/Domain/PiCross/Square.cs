using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace PiCross
{
    /// <summary>
    /// Square would be defined as an enum in Java, but native C# enums have a different purpose so
    /// we fake Java enums. Square can take on three different values: UNKNOWN,
    /// FILLED and EMPTY. The meaning of these values becomes clear
    /// when considered in the context of puzzle solving: originally,
    /// the puzzle is fully UNKNOWN, and as more information is gathered,
    /// squares become FILLED or EMPTY. A fully solved puzzle does not contain
    /// any UNKNOWNs.
    /// </summary>
    public abstract class Square
    {
        /// <summary>
        /// Unknown square.
        /// </summary>
        public static readonly Square UNKNOWN = new Unknown();

        /// <summary>
        /// Filled square.
        /// </summary>
        public static readonly Square FILLED = new Filled();

        /// <summary>
        /// Empty square.
        /// </summary>
        public static readonly Square EMPTY = new Empty();

        /// <summary>
        /// Parses the given symbol. '.', 'x' and '?'
        /// correspond to EMPTY, FILLED and UNKNOWN, respectively.
        /// </summary>
        /// <param name="symbol">Char to be converted to Square.</param>
        /// <returns>Square corresponding to value.</returns>
        public static Square FromSymbol( char symbol )
        {
            if ( UNKNOWN.Symbol == symbol )
            {
                return UNKNOWN;
            }
            else if ( FILLED.Symbol == symbol )
            {
                return FILLED;
            }
            else if ( EMPTY.Symbol == symbol )
            {
                return EMPTY;
            }
            else
            {
                throw new ArgumentException( $"Unknown symbol: {symbol}" );
            }
        }

        /// <summary>
        /// Creates a Square from a bool. True gets transformed to FILLED, false to EMPTY.
        /// </summary>
        /// <param name="b">Bool.</param>
        /// <returns>Square.</returns>
        public static Square FromBool( bool b )
        {
            return b ? FILLED : EMPTY;
        }

        /// <summary>
        /// Parses a sequence of strings, resulting in a grid of squares.
        /// </summary>
        /// <param name="rows">Strings representing rows.</param>
        /// <returns>Grid.</returns>
        public static IGrid<Square> CreateGrid( params string[] rows )
        {
            return CreateGrid( Grid.CreateCharacterGrid( rows ) );
        }

        /// <summary>
        /// Transforms a grid of chars into a grid of squares.
        /// </summary>
        /// <param name="grid">Grid of chars.</param>
        /// <returns>Grid of squares.</returns>
        public static IGrid<Square> CreateGrid( IGrid<char> grid )
        {
            return grid.Map( Square.FromSymbol );
        }

        private Square()
        {
            // NOP
        }

        /// <summary>
        /// Checks whether a square is compatible with another.
        /// UNKNOWN is compatible with everything,
        /// FILLED is compatible with FILLED and UNKNOWN,
        /// EMPTY is compatible with EMPTY and UNKNOWN.
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public abstract bool CompatibleWith( Square that );

        /// <summary>
        /// Merges two Squares together.
        /// FILLED merged with FILLED yields FILLED.
        /// EMPTY merged with EMPTY yields EMPTY.
        /// All other combinations yield UNKNOWN.
        /// </summary>
        /// <param name="that">Square to merge with.</param>
        /// <returns>Merge result.</returns>
        public abstract Square Merge( Square that );

        /// <summary>
        /// Symbol for this square.
        /// Do NOT make use of this property to determine
        /// what kind of Square it is. Instead, use equality, e.g.,
        /// square == Square.UNKNOWN.
        /// </summary>
        public abstract char Symbol { get; }

        public override bool Equals( object obj )
        {
            return object.ReferenceEquals( this, obj );
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol.ToString();
        }

        /// <summary>
        /// Converts a Square to a bool.
        /// FILLED is converted to true,
        /// EMPTY to false. Casting UNKNOWN results in an exception.
        /// </summary>
        /// <param name="square">Square to be cast to bool.</param>
        /// <returns>Square.</returns>
        /// <exception cref="ArgumentNullException">
        /// When square is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when casting UNKNOWN to bool.
        /// </exception>
        public static explicit operator bool( Square square )
        {
            if ( square == null )
            {
                throw new ArgumentNullException( nameof( square ) );
            }
            else if ( square == UNKNOWN )
            {
                throw new ArgumentException( "Cannot convert UNKNOWN to bool" );
            }
            else if ( square == EMPTY )
            {
                return false;
            }
            else
            {
                Debug.Assert( square == FILLED );

                return true;
            }
        }

        private class Unknown : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return true;
            }

            public override Square Merge( Square that )
            {
                return UNKNOWN;
            }

            public override char Symbol => '?';
        }

        private class Filled : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return that != EMPTY;
            }

            public override Square Merge( Square that )
            {
                if ( that == this )
                {
                    return this;
                }
                else
                {
                    return UNKNOWN;
                }
            }

            public override char Symbol => 'x';
        }

        private class Empty : Square
        {
            public override bool CompatibleWith( Square that )
            {
                return that != FILLED;
            }

            public override Square Merge( Square that )
            {
                if ( that == this )
                {
                    return this;
                }
                else
                {
                    return UNKNOWN;
                }
            }

            public override char Symbol => '.';
        }
    }
}
