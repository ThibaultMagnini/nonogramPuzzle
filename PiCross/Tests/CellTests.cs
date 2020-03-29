using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cells;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        [TestCategory( "Cell" )]
        public void Cell_EventFiredWhenValueChange()
        {
            var cell = CreateCell( 0 );
            var flag = Flag.Create( cell );

            Assert.IsFalse( flag.Status );
            cell.Value = 1;
            Assert.IsTrue( flag.Status );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Cell_NoEventFiredWhenValueChangeToSameOldValue()
        {
            var cell = CreateCell( 0 );
            var flag = Flag.Create( cell );

            Assert.IsFalse( flag.Status );
            cell.Value = 0;
            Assert.IsFalse( flag.Status );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Derived_Initialized()
        {
            var cell = CreateCell( 2 );
            var derived = CreateDerived( cell, x => x * x );

            Assert.AreEqual( 4, derived.Value );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void Derived_ChangesWithCell()
        {
            var cell = CreateCell( 2 );
            var derived = CreateDerived( cell, x => x * x );

            Assert.AreEqual( 4, derived.Value );
            cell.Value = 5;
            Assert.AreEqual( 25, derived.Value );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        public void WritableCell_CanWrite()
        {
            var cell = CreateCell( 2 );
            var derived = Cell.Derived( cell, x => x + 1, x => x - 1 );

            Assert.AreEqual( 2, cell.Value );
            Assert.AreEqual( 3, derived.Value );

            derived.Value = 5;

            Assert.AreEqual( 4, cell.Value );
            Assert.AreEqual( 5, derived.Value );
        }

        [TestMethod]
        [TestCategory( "Cell" )]
        [Timeout(1000)]
        public void Future()
        {
            var value = 5;
            var cell = CreateFuture<int>();
            var task = new Task( () => { Thread.Sleep( 100 ); cell.Value = value; } );

            task.Start();
            Thread.Sleep( 200 );
            Assert.AreEqual( value, cell.Value );
        }

        private static Cell<T> CreateCell<T>( T value )
        {
            return Cell.Create<T>( value );
        }

        private static Cell<R> CreateDerived<T, R>( Cell<T> cell, Func<T, R> function )
        {
            return Cell.Derived( cell, function );
        }

        private static FutureCell<T> CreateFuture<T>()
        {
            return (FutureCell<T>) Cell.CreateFuture<T>();
        }
    }
}
