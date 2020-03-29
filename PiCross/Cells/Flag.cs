using System.Diagnostics;

namespace Cells
{
    public class Flag
    {
        public static Flag<T> Create<T>( Cell<T> cell )
        {
            return new Flag<T>( cell );
        }
    }

    [DebuggerDisplay( "{Status}" )]
    public class Flag<T>
    {
        public Flag( Cell<T> cell )
        {
            cell.ValueChanged += () => { Status = true; };
        }

        public bool Status { get; set; }
    }
}
