using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IO
{
    public interface ISerializer<T>
    {
        void Write( StreamWriter writer, T obj );

        T Read( StreamReader reader );
    }

    public abstract class ReaderBase
    {
        protected readonly StreamReader streamReader;

        protected ReaderBase( StreamReader streamReader )
        {
            if ( streamReader == null )
            {
                throw new ArgumentNullException( nameof( streamReader ) );
            }
            else
            {
                this.streamReader = streamReader;
            }
        }

        protected int ReadInteger()
        {
            return ReadIntegers( expectedCount: 1 )[0];
        }

        protected int[] ReadIntegers( int? expectedCount = null )
        {
            var line = streamReader.ReadLine();
            var ns = line.Split( ' ' ).Select( int.Parse ).ToArray();

            if ( expectedCount.HasValue && ns.Length != expectedCount.Value )
            {
                throw new IOException( $"Expected {expectedCount.Value} integers" );
            }
            else
            {
                return ns;
            }
        }

        protected string[] ReadLines( int count )
        {
            return Enumerable.Range( 0, count ).Select( _ => streamReader.ReadLine() ).ToArray();
        }
    }

    public abstract class WriterBase
    {
        protected readonly StreamWriter streamWriter;

        protected WriterBase( StreamWriter streamWriter )
        {
            if ( streamWriter == null )
            {
                throw new ArgumentNullException( nameof( streamWriter ) );
            }
            else
            {
                this.streamWriter = streamWriter;
            }
        }

        protected void Write( int[] ns )
        {
            streamWriter.WriteLine( string.Join( " ", ns.Select( n => n.ToString() ) ) );
        }
    }
}
