using System;
using System.Collections.Generic;
using System.Text;

namespace FmpAnalyzer
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// AllPositive
        /// </summary>
        /// <param name="listOfDouble"></param>
        /// <returns></returns>
        public static bool AllPositive(this List<double> listOfDouble)
        {
            foreach (var doubleValue in listOfDouble)
            {
                if (doubleValue <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Grows
        /// </summary>
        /// <param name="listOfDouble"></param>
        /// <returns>Returns listOfDouble.Count() - 1 if all elements grow.
        /// Returns 0 if no growth at all.</returns>
        public static int Grows(this List<double> listOfDouble)
        {
            return listOfDouble.GrowsOrDeclines((v0, v1) => v0 > v1);
        }

        /// <summary>
        /// listOfDouble
        /// </summary>
        /// <param name="listOfDouble"></param>
        /// <returns>Returns listOfDouble.Count() - 1 if all elements declines.
        /// Returns 0 if no decline at all.</returns>
        public static int Declines(this List<double> listOfDouble)
        {
            return listOfDouble.GrowsOrDeclines((v0, v1) => v0 < v1);
        }

        /// <summary>
        /// GrowsOrDeclines
        /// </summary>
        /// <param name="listOfDouble"></param>
        /// <param name="compareFunction"></param>
        /// <returns></returns>
        public static int GrowsOrDeclines(this List<double> listOfDouble, Func<double, double, bool> compareFunction)
        {
            int result = 0;

            for (int i = 1; i < listOfDouble.Count;  i++)
            {
                var v = listOfDouble[i];
                var vBefore = listOfDouble[i - 1];
                if (compareFunction(v, vBefore))
                {
                    result++;
                }
            }

            return result;
        }
    }
}
