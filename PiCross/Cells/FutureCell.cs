using System;
using System.Diagnostics;
using System.Threading;

namespace Cells
{
    [DebuggerDisplay( "{DebugProxy}" )]
    public class FutureCell<T> : Cell<T>
    {
        private volatile bool isBound;

        public FutureCell()
            : base( default( T ) )
        {
            isBound = false;
        }

        public override T Value
        {
            get
            {
                lock ( this )
                {
                    WaitForBinding();

                    return base.Value;
                }
            }
            set
            {
                lock ( this )
                {
                    if ( isBound )
                    {
                        throw new InvalidOperationException( "Future already bound" );
                    }
                    else
                    {
                        base.Value = value;

                        isBound = true;
                        Monitor.PulseAll( this );
                    }
                }
            }
        }

        private void WaitForBinding()
        {
            if ( !isBound )
            {
                Monitor.Wait( this );
            }
        }

        public override void Refresh()
        {
            throw new InvalidOperationException( "Cannot refresh futures" );
        }

        private T BaseValue { get { return base.Value; } }

        private string DebugString
        {
            get
            {
                if ( isBound )
                {
                    return base.Value.ToString();
                }
                else
                {
                    return "<unbound>";
                }
            }
        }

        private Proxy DebugProxy { get { return new Proxy( this ); } }

        private struct Proxy
        {
            private readonly FutureCell<T> cell;

            public Proxy( FutureCell<T> cell )
            {
                this.cell = cell;
            }

            public override string ToString()
            {
                return cell.DebugString;
            }
        }
    }
}
