using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace Cells
{
    public interface ISignal
    {
        void Send();
    }

    public class SignalFactory<T>
    {
        public SignalFactory( Cell<T> cell )
        {
            this.Cell = cell;
        }

        public SignalFactory( T initialCellContents = default( T ) )
            : this( Cells.Cell.Create<T>( initialCellContents ) )
        {
            // NOP
        }

        public Cell<T> Cell { get; }

        public ISignal CreateSignal( T value )
        {
            return new Signal( Cell, value );
        }

        private class Signal : ISignal
        {
            private readonly T value;

            private readonly Cell<T> cell;

            public Signal( Cell<T> cell, T value )
            {
                this.cell = cell;
                this.value = value;
            }

            public void Send()
            {
                cell.Value = value;
            }
        }
    }
}
