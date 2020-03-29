using System;
using System.Collections.Generic;

namespace Cells
{
    public abstract class ManualCell<T> : Cell<T>
    {
        protected ManualCell( T initialValue )
            : base( initialValue )
        {
            // NOP
        }

        public override T Value
        {
            get
            {
                return ReadValue();
            }
            set
            {
                WriteValue( value );
            }
        }

        public bool IsDirty => !Util.AreEqual( ReadValue(), base.Value );

        public override void Refresh()
        {
            if ( IsDirty )
            {
                base.Value = ReadValue();

                NotifyObservers();
            }
        }

        protected abstract T ReadValue();

        protected abstract void WriteValue( T value );
    }

    public class ReadonlyManualCell<T> : Cell<T>
    {
        private readonly Func<T> function;

        public ReadonlyManualCell( Func<T> function )
            : base( function() )
        {
            this.function = function;
        }

        public override T Value
        {
            get
            {
                return function();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        public bool IsDirty => !Util.AreEqual( function(), base.Value );

        public override void Refresh()
        {
            if ( IsDirty )
            {
                base.Value = function();

                NotifyObservers();
            }
        }
    }

    internal class DirtyCellFactory<T, CELL>
        where CELL : ManualCell<T>
    {
        private readonly List<CELL> cells;

        private readonly Func<T, CELL> factory;

        public DirtyCellFactory( Func<T, CELL> factory )
        {
            if ( factory == null )
            {
                throw new ArgumentNullException( nameof( factory ) );
            }
            else
            {
                this.cells = new List<CELL>();
                this.factory = factory;
            }
        }

        public CELL CreateCell( T value )
        {
            var cell = factory( value );

            cells.Add( cell );

            return cell;
        }

        public void Clean()
        {
            foreach ( var cell in cells )
            {
                cell.Refresh();
            }
        }
    }
}
