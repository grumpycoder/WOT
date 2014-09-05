using System;
using System.Collections.Generic;
using System.Linq;

namespace WOT.Server
{
    public static class Extension
    {
        public static T Next<T>(this IEnumerable<T> list, T current)
        {
            try
            {
                if (current == null) return list.FirstOrDefault();

                return list.SkipWhile(x => !x.Equals(current)).Skip(1).First();
            }
            catch
            {
                return list.FirstOrDefault();
            }
        }

        public static T Previous<T>(this IEnumerable<T> list, T current)
        {
            try
            {
                if (current == null) return list.LastOrDefault();
                return list.TakeWhile(x => !x.Equals(current)).Last();
            }
            catch
            {
                return list.LastOrDefault();
            }
        }

        public static int ToInt(this double x)
        {
            return Convert.ToInt32(x);
        }
    }
}