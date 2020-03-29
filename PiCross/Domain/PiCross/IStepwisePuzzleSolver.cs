using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataStructures;

namespace PiCross
{
    public interface IStepwisePuzzleSolver
    {
        IGrid<Square> Grid { get; }

        bool IsSolved { get; }

        void Step();
    }
}
