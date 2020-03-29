using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using PiCross;
using DataStructures;

namespace PiCross.Tests
{
    [TestClass]
    public class Constraints_GenerateSlices
    {
        [TestMethod]
        [TestCategory("Constraints")]
        public void GenerateSlices_Length_1_No_Constraints()
        {
            new Test( 1 ).Check( "." );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_5_No_Constraints()
        {
            new Test( 5 ).Check( "....." );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_1_Constraints_1()
        {
            new Test( 1, 1 ).Check( "x" );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_2_Constraints_1()
        {
            new Test( 2, 1 ).Check( "x.", ".x" );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_3_Constraints_1_1()
        {
            new Test( 3, 1, 1 ).Check( "x.x" );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_4_Constraints_1_1()
        {
            new Test( 4, 1, 1 ).Check( "x..x", "x.x.", ".x.x" );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_5_Constraints_1_1_1()
        {
            new Test( 5, 1, 1, 1 ).Check( "x.x.x" );
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_4_Constraints_1_1_1()
        {
            new Test( 4, 1, 1, 1 ).Check();
        }

        [TestMethod]
        [TestCategory( "Constraints" )]
        public void GenerateSlices_Length_4_Constraints_4_4()
        {
            new Test( 4, 4, 4 ).Check();
        }

        private class Test
        {
            private readonly List<Slice> sequences;

            public Test( int length, params int[] constraints )
            {
                sequences = Constraints.FromSequence( Sequence.FromItems( constraints ) ).GenerateSlices( length ).ToList();
            }

            private void CheckSingle( string str )
            {
                var slice = Slice.FromString( str );

                Assert.IsTrue( sequences.Contains( slice ), "{0} should appear", slice.ToString() );
                sequences.Remove( slice );
            }

            public void Check( params string[] ss )
            {
                foreach ( var s in ss )
                {
                    CheckSingle( s );
                }

                Done();
            }

            public void Done()
            {
                Assert.AreEqual( 0, sequences.Count );
            }
        }
    }
}
