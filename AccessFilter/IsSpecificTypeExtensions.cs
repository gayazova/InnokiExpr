using System.Reflection;
using System.Runtime.CompilerServices;

namespace AccessFilter
{
    public static class IsSpecificTypeExtensions
    {
        public static bool IsPrimitive(this Type type) => type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(decimal);

        public static bool IsSimple(this Type type) => type.IsPrimitive() || type.IsNullablePrimitive() || type.IsEnum;

        public static bool IsNullablePrimitive(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != default && underlyingType.IsPrimitive();
        }

        public static bool IsNullable(this Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            return underlyingType != default || !type.IsValueType;
        }

        public static bool IsNumericType(this object o) => IsNumericType(o.GetType());

        public static bool IsNumericType(this Type o)
        {
            switch (Type.GetTypeCode(o))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
                return false;

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static bool IsDbCollection(this PropertyInfo property)
        {
            var goodType = IsICollection(property.PropertyType);
            return goodType && property.GetGetMethod().IsVirtual;
        }

        public static bool IsICollection(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>);
    }
}
