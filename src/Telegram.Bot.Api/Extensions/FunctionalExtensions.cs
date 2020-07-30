using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.Api.Extensions
{
    public static class FunctionalExtensions
    {

        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static bool Check<T>(this T item, Func<T, bool> predicate)
        {
            return predicate(item);
        }

        public static bool Continue<T>(this bool item, Func<bool, bool> predicate)
        {
            if (item)
            {
                return predicate(item);
            }
            return false;
        }
    }
}
