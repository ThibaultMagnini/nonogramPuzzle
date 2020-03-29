using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cells
{
    public class DeferredCell<T> : Cell<T>
    {
        private bool dirty;

        public DeferredCell( T initialValue )
            : base( initialValue )
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
                if ( !Util.AreEqual( base.Value, value ) )
                {
                    base.Value = value;
                    dirty = true;
                }
            }
        }

        public void BroadcastChange()
        {
            if ( dirty )
            {
                dirty = false;

                NotifyObservers();
            }
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
