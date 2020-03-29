using System;

namespace Cells
{
    public class ReadonlyWrapper<T> : Cell<T>
    {
        private readonly Cell<T> wrappedCell;

        public ReadonlyWrapper( Cell<T> wrappedCell )
        {
            if ( wrappedCell == null )
            {
                throw new ArgumentNullException( nameof( wrappedCell ) );
            }
            else
            {
                this.wrappedCell = wrappedCell;

                wrappedCell.ValueChanged += NotifyObservers;
            }
        }

        public override T Value
        {
            get
            {
                return wrappedCell.Value;
            }
            set
            {
                throw new InvalidOperationException( "Cell is readonly" );
            }
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
