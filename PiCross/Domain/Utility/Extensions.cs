using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class DictionaryExtensions
    {
        public static bool EqualItems<K, V>( this IDictionary<K, V> x, IDictionary<K, V> y )
        {
            var xKeys = new HashSet<K>( x.Keys );
            var yKeys = new HashSet<K>( y.Keys );

            if ( !xKeys.SetEquals( yKeys ) )
            {
                return false;
            }
            else
            {
                return xKeys.All( key => x[key].Equals( y[key] ) );
            }
        }
    }
}
