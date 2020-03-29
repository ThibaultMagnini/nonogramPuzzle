using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace Utility
{
    public class Chronometer
    {
        private DateTime lastTick;

        private bool started;

        private Cell<TimeSpan> totalTime;

        public Chronometer()
        {
            started = false;
            totalTime = Cell.Create( TimeSpan.Zero );
        }

        public void Start()
        {
            started = true;
            lastTick = DateTime.Now;
        }

        public void Pause()
        {
            started = false;
        }

        public void Tick()
        {
            if ( started )
            {
                var now = DateTime.Now;
                var delta = now - lastTick;
                lastTick = now;

                totalTime.Value += delta;
            }
        }

        public Cell<TimeSpan> TotalTime
        {
            get
            {
                return totalTime;
            }
        }
    }
}
