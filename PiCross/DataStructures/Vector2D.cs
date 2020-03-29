using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructures
{
    public class Vector2D
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X-coordinate</param>
        /// <param name="y">Y-coordinate</param>
        [DebuggerStepThrough]
        public Vector2D( int x = 0, int y = 0 )
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// X-coordinate
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y-coordinate
        /// </summary>
        public int Y { get; }

        public override bool Equals( object obj )
        {
            return Equals( obj as Vector2D );
        }

        public bool Equals( Vector2D v )
        {
            return !object.ReferenceEquals( v, null ) && this.X == v.X && this.Y == v.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        /// <summary>
        /// Adds two vectors together.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>Sum of both vectors</returns>
        public static Vector2D operator +( Vector2D u, Vector2D v )
        {
            if ( u == null )
            {
                throw new ArgumentNullException( nameof( u ) );
            }
            else if ( v == null )
            {
                throw new ArgumentNullException( nameof( v ) );
            }
            else
            {
                var x = u.X + v.X;
                var y = u.Y + v.Y;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Negation.
        /// </summary>
        /// <param name="v">Vector</param>
        /// <returns>Negation of the given vector</returns>
        public static Vector2D operator -( Vector2D v )
        {
            if ( v == null )
            {
                throw new ArgumentNullException( nameof( v ) );
            }
            else
            {
                var x = -v.X;
                var y = -v.Y;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Subtraction.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>Difference</returns>
        public static Vector2D operator -( Vector2D u, Vector2D v )
        {
            if ( u == null )
            {
                throw new ArgumentNullException( nameof( u ) );
            }
            else if ( v == null )
            {
                throw new ArgumentNullException( nameof( v ) );
            }
            else
            {
                return u + (-v);
            }
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="v">Vector</param>
        /// <param name="factor">Factor</param>
        /// <returns>Product</returns>
        public static Vector2D operator *( Vector2D v, int factor )
        {
            if ( v == null )
            {
                throw new ArgumentNullException( nameof( v ) );
            }
            else
            {
                var x = v.X * factor;
                var y = v.Y * factor;

                return new Vector2D( x, y );
            }
        }

        /// <summary>
        /// Multiplication
        /// </summary>
        /// <param name="factor">Factor</param>
        /// <param name="v">Vector</param>
        /// <returns>Product</returns>
        public static Vector2D operator *( int factor, Vector2D v )
        {
            return v * factor;
        }

        /// <summary>
        /// Checks for equality. Null is considered equal to null.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>True if both vectors are equal, false otherwise.</returns>
        public static bool operator ==( Vector2D u, Vector2D v )
        {
            if ( object.ReferenceEquals( u, null ) )
            {
                return object.ReferenceEquals( v, null );
            }
            else
            {
                return u.Equals( v );
            }
        }

        /// <summary>
        /// Checks for inequality.
        /// </summary>
        /// <param name="u">Vector</param>
        /// <param name="v">Vector</param>
        /// <returns>True if vectors are not equal, false otherwise.</returns>
        public static bool operator !=( Vector2D u, Vector2D v )
        {
            return !(u == v);
        }
    }
}
