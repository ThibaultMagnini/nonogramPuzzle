using System;

namespace Cells
{
    internal class ConstantCell<T> : Cell<T>
    {
        public ConstantCell( T value )
            : base( value )
        {
            // NOP
        }

        public override T Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                throw new InvalidOperationException( "Cannot change value of ConstantCell" );
            }
        }

        public override void Refresh()
        {
            // NOP
        }
    }
}
