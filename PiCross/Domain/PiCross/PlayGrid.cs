using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cells;

namespace PiCross
{
    internal class PlayGrid
    {
        public PlayGrid( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints, IGrid<Square> squares )
        {
            if ( columnConstraints == null )
            {
                throw new ArgumentNullException( nameof( columnConstraints ) );
            }
            else if ( rowConstraints == null )
            {
                throw new ArgumentNullException( "rowConstraints " );
            }
            else if ( squares == null )
            {
                throw new ArgumentNullException( nameof( squares ) );
            }
            else if ( columnConstraints.Length != squares.Size.Width )
            {
                throw new ArgumentException( "Number of column constraints should be equal to grid width" );
            }
            else if ( rowConstraints.Length != squares.Size.Height )
            {
                throw new ArgumentException( "Number of row constraints should be equal to grid height" );
            }
            else
            {
                this.Squares = squares.Map( sqr => new Var<Square>( sqr ) ).Copy();

                this.ColumnConstraints = (from i in Squares.ColumnIndices
                                          let constraints = columnConstraints[i]
                                          let slice = new Slice( Squares.Column( i ).Map( var => var.Value ) )
                                          select new PlayGridConstraints( slice, constraints )).ToSequence();

                this.RowConstraints = (from i in Squares.RowIndices
                                       let constraints = rowConstraints[i]
                                       let slice = new Slice( Squares.Row( i ).Map( var => var.Value ) )
                                       select new PlayGridConstraints( slice, constraints )).ToSequence();
            }
        }

        public PlayGrid( ISequence<Constraints> columnConstraints, ISequence<Constraints> rowConstraints )
            : this( columnConstraints, rowConstraints, Grid.CreateVirtual( new Size( columnConstraints.Length, rowConstraints.Length ), _ => Square.UNKNOWN ) )
        {
            // NOP
        }

        public IGrid<IVar<Square>> Squares { get; }

        public ISequence<PlayGridConstraints> ColumnConstraints { get; }

        public ISequence<PlayGridConstraints> RowConstraints { get; }
    }

    internal class PlayGridConstraints
    {
        private readonly ISequence<PlayGridConstraintValue> values;

        private readonly Slice slice;

        private readonly Constraints constraints;

        public PlayGridConstraints( Slice slice, Constraints constraints )
        {
            this.slice = slice;
            this.constraints = constraints;

            this.values = Sequence.FromFunction( constraints.Values.Length,
                                                 i => new PlayGridConstraintValue( slice, constraints, i ) );
        }

        public ISequence<PlayGridConstraintValue> Values
        {
            get
            {
                return values;
            }
        }

        public bool IsSatisfied
        {
            get
            {
                return constraints.IsSatisfied( slice );
            }
        }
    }

    internal class PlayGridConstraintValue
    {
        private readonly Slice slice;

        private readonly Constraints constraints;

        private readonly int index;

        public PlayGridConstraintValue( Slice slice, Constraints constraints, int index )
        {
            this.slice = slice;
            this.constraints = constraints;
            this.index = index;
        }

        public bool IsSatisfied
        {
            get
            {
                return !constraints.UnsatisfiedValueRange( slice ).Contains( index );
            }
        }

        public int Value
        {
            get
            {
                return constraints.Values[index];
            }
        }
    }
}
