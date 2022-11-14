using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessFilter
{
    public static class TypeCheckExtensions
    {
        public static T As<T>(this object obj)
        {
            if (obj == default)
            {
                return default;
            }

            if (obj is T tObj)
            {
                return tObj;
            }

            return default;
        }

        public static T As<T>(this object obj, T @defaultIfExists)
        {
            if (obj == default)
            {
                return default;
            }

            if (obj is T tObj)
            {
                return tObj;
            }

            return @defaultIfExists;
        }

        public static bool Is<TType>(this object obj)
        {
            return obj is TType;
        }

        public static bool Is<TType>(this object obj, out TType castResult)
        {
            if (obj is TType result)
            {
                castResult = result;
                return true;
            }

            castResult = default;
            return false;
        }

        public static bool IsNot<TType>(this object obj)
        {
            return !(obj is TType);
        }

        public static bool IsNot<TType>(this object obj, out TType castResult)
        {
            if (obj is TType result)
            {
                castResult = result;
                return false;
            }

            castResult = default;
            return true;
        }
    }
}
