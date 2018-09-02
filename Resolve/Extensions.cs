using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Resolve
{
    public static class Extensions
    {
        public static void AddRangeIgnoreNull<T>(this List<T> collection, IEnumerable<T> additions)
        {
            if (additions != null)
            {
                collection.AddRange(additions);
            }
        }
    }
}
