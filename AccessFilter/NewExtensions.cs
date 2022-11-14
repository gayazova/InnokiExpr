using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AccessFilter
{
    public static class NewExtensions
    {
        /// <summary>
        /// Создать новый объект такого типа
        /// <para>
        /// Только вот не надо использовать это на типах <see cref="Type"/>
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="object"></param>
        /// <returns></returns>
        public static T New<T>(this object @object) => NewAs<T>(@object.GetType());

        /// <summary>
        /// Instantiate new object through expression tree with first ctor
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T New<T>(this Type type, params object[] argsObj)
            => New<T>(type, typeof(T).GetConstructors().FirstOrDefault(), argsObj);

        /// <summary>
        /// Instantiate new object through expression tree with first ctor
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static object New(this Type type, params object[] argsObj)
            => New<object>(type, type.GetConstructors().FirstOrDefault(), argsObj);

        public static object New(this Type type, bool onlyParameterLess, params object[] argsObj)
        {
            var ctors = type.GetConstructors();
            ConstructorInfo ctor = default;
            if (onlyParameterLess)
            {
                ctor = ctors.FirstOrDefault(c => c.GetParameters().Length == 0);
            }
            else
            {
                ctor = ctors.FirstOrDefault();
            }

            return New<object>(type, ctor, argsObj);
        }

        /// <summary>
        /// Instantiate new object through expression tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="ctor"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T New<T>(this Type type, ConstructorInfo ctor, params object[] argsObj)
        {
            if (ctor == default)
                return default;

            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func(argsObj);
        }

        /// <summary>
        /// Instantiate new object through expression tree
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="ctor"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T NewAs<T>(this Type type, int ctorCount, params object[] argsObj)
        {
            ConstructorInfo ctor = type.GetConstructors().ElementAtOrDefault(ctorCount - 1);

            if (ctor == default)
                return default;

            ParameterInfo[] par = ctor.GetParameters();
            Expression[] args = new Expression[par.Length];
            ParameterExpression param = Expression.Parameter(typeof(object[]));
            for (int i = 0; i != par.Length; ++i)
            {
                args[i] = Expression.Convert(Expression.ArrayIndex(param, Expression.Constant(i)), par[i].ParameterType);
            }
            var expression = Expression.Lambda<Func<object[], T>>(
                Expression.New(ctor, args), param
            );

            var func = expression.Compile();

            return func.Invoke(argsObj).As<T>();
        }

        /// <summary>
        /// Инстанциирует объект как object, а затем приводит к T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="argsObj"></param>
        /// <returns></returns>
        public static T NewAs<T>(this Type type, params object[] argsObj)
            => (T)New<object>(type, type.GetConstructors().FirstOrDefault(), argsObj);

        public static T NewDefaults<T>(this Type type)
        {
            var ctor = type.GetConstructors().FirstOrDefault();

            var args = ctor.GetParameters()
                .Select(x => x.ParameterType.Default())
                .ToArray();

            return (T)New<object>(type, ctor, args);
        }

        public static object Default(this Type type)
        {
            if (!___GetDefaultCache.TryGetValue(type, out var value))
            {
                value = Expression.Lambda(Expression.Default(type)).Compile().DynamicInvoke();
                ___GetDefaultCache.AddOrUpdate(type, value, (x, y) => value);
            }

            return value;
        }

        private static readonly ConcurrentDictionary<Type, object> ___GetDefaultCache = new ConcurrentDictionary<Type, object>();
    }
}
