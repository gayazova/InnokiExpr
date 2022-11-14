using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace AccessFilter
{
    public static class CallExtensions
    {
        /// <summary>
        /// Вызвать метод у объекта
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <returns>Результат вызова (если метод не void, иначе будет default)</returns>
        public static object Call(this object @object, string method, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo == default)
            {
                methodInfo = @object.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                if (methodInfo.GetParameters().IsEmpty())
                    return Expression.Lambda(Expression.Call(Expression.Constant(@object), methodInfo)).Compile().DynamicInvoke(argsObj);

                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(@from, methodInfo, @params);

                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj);
            }

            return default;
        }

        /// <summary>
        /// Вызвать метод у объекта
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="paramTypes">Типы параметров</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <returns>Результат вызова (если метод не void, иначе будет default)</returns>
        public static object Call(this object @object, string method, Type[] paramTypes, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo == default)
            {
                methodInfo = @object.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                if (methodInfo.GetParameters().IsEmpty())
                    return Expression.Lambda(Expression.Call(Expression.Constant(@object), methodInfo)).Compile().DynamicInvoke(argsObj);

                var from = Expression.Constant(@object);
                var @params = paramTypes.Select(Expression.Parameter).ToArray();
                var methodCall = Expression.Call(@from, methodInfo, @params);

                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj);
            }

            return default;
        }

        /// <summary>
        /// Вызвать статический метод у типа
        /// </summary>
        /// <param name="sourceType">Тип</param>
        /// <param name="method">Имя метода</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <returns>Результат вызова (если метод не void, иначе будет default)</returns>
        public static object CallStatic(this Type sourceType, string method, params object[] argsObj)
        {
            var methodInfo = sourceType.GetMethods().FirstOrDefault(m => m.Name == method);

            if (methodInfo == default)
            {
                methodInfo = sourceType
                    .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
                    .FirstOrDefault(m => m.Name == method);
            }

            if (methodInfo != default)
            {
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                if (methodInfo.ReturnType != typeof(void))
                {
                    {
                        return Expression.Lambda(Expression.Call(methodInfo, @params)).Compile().DynamicInvoke(argsObj);
                    }
                }
                else
                {
                    var expCall = Expression.Call(methodInfo, @params);
                    Expression.Lambda(expCall).Compile().DynamicInvoke(argsObj);
                }
            }

            return false;
        }

        /// <summary>
        /// Вызвать дженерик-метод у объекта
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="generics">Набор дженериков метода</param>
        /// <param name="argsObj">Набор аргументов</param>
        public static object CallGeneric(this object @object, string method, Type[] generics, params object[] argsObj)
        {
            var methodInfo = @object.GetType()
                .GetMethods()
                .Where(x => x.GetParameters().Length == argsObj.Length)
                .LastOrDefault(m => m.Name == method && m.IsGenericMethod);

            if (methodInfo == default)
            {
                methodInfo = @object.GetType()
                    .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(x => x.GetParameters().Length == argsObj.Length)
                    .LastOrDefault(m => m.Name == method && m.IsGenericMethod);
            }

            if (methodInfo != default)
            {
                methodInfo = methodInfo.MakeGenericMethod(generics);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();

                var methodCallExpression = Expression.Call(Expression.Constant(@object), methodInfo, @params);

                return Expression.Lambda(methodCallExpression, @params).Compile().DynamicInvoke(argsObj);
            }

            return default;
        }

        /// <summary>
        /// Вызвать дженерик-метод у объекта с результатом определенного типа
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="generics">Набор дженериков метода</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <typeparam name="TResult">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static TResult CallGeneric<TResult>(this object @object, string method, Type[] generics, params object[] argsObj) =>
            CallGeneric(@object, method, generics, argsObj).As<TResult>();

        /// <summary>
        /// Вызвать метод у объекта с определенным типом результата и
        /// значением по умолчанию на случай, если метод не будет найден
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="default">Значение результата по умолчанию</param>
        /// <param name="argsObj">Аргументы для вызова</param>
        /// <typeparam name="TResult">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static TResult Call<TResult>(this object @object, string method, TResult @default, params object[] argsObj)
        {
            var methodInfo = @object.GetType().GetMethods().FirstOrDefault(m => m.Name == method && m.GetParameters().Length == argsObj.Length);
            if (methodInfo == default)
            {
                methodInfo = @object.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(from, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj).As<TResult>();
            }

            return @default;
        }

        /// <summary>
        /// Вызывать метод у объекта с определенным результатом по умолчанию
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="methodInfo">Информация о методе</param>
        /// <param name="default">Результат по умолчанию</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <typeparam name="TResult">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static TResult Call<TResult>(this object @object, MethodInfo methodInfo, TResult @default, params object[] argsObj)
        {
            if (methodInfo != default)
            {
                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(from, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj).As<TResult>();
            }

            return @default;
        }

        /// <summary>
        /// Вызывать метод у объекта с определенным результатом по умолчанию
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="methodInfo">Информация о методе</param>
        /// <param name="default">Результат по умолчанию</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <typeparam name="TResult">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static object CallObj(this object @object, MethodInfo methodInfo, params object[] argsObj)
        {
            if (methodInfo != default)
            {
                if (@object is Type type)
                {
                    var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                    var methodCall = Expression.Call(methodInfo, @params);
                    Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj);
                }
                else
                {
                    var from = Expression.Constant(@object);
                    var @params = argsObj.Select(a => Expression.Parameter(a.GetType())).ToArray();
                    var methodCall = Expression.Call(from, methodInfo, @params);
                    return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj);
                }
            }

            return default;
        }

        /// <summary>
        /// Вызвать метод у объекта с определенным типом результата и
        /// значением по умолчанию на случай, если метод не будет найден
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="default">Значение результата по умолчанию</param>
        /// <param name="argsObj">Аргументы для вызова</param>
        /// <typeparam name="TResult">Тип результата вызова метода</typeparam>
        /// <typeparam name="TFrom">Класс, у которого искать метод для вызова</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static TResult Call<TResult, TFrom>(this object @object, string method, TResult @default, params object[] argsObj)
        {
            var methodInfo = typeof(TFrom).GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo == default)
            {
                methodInfo = typeof(TFrom).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).FirstOrDefault(m => m.Name == method);
            }
            if (methodInfo != default)
            {
                var from = Expression.Constant(@object);
                var @params = argsObj.Select(a => Expression.Parameter(a.GetType()));
                var methodCall = Expression.Call(from, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(argsObj).As<TResult>();
            }

            return @default;
        }

        /// <summary>
        /// Вызвать дженерик-метод у переданного типа с объектом в виде первого аргумента
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="sourceType">Класс, в котором ищется метод для вызова</param>
        /// <param name="generic">Набор дженериков</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <returns>Результат вызова метода</returns>
        public static object CallGenericExtension(this object @object, string method, Type sourceType, Type[] generic, params object[] argsObj)
        {
            var methodInfo = sourceType.GetMethods().FirstOrDefault(m => m.Name == method && m.GetGenericArguments().Length == generic.Length /*&& m.GetParameters().Length == argsObj.Length + 1*/);
            if (methodInfo != default)
            {
                var combinedArgs = new List<object> { @object };
                combinedArgs.AddRange(argsObj);
                methodInfo = methodInfo.MakeGenericMethod(generic);
                var @params = combinedArgs.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(null, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(combinedArgs.ToArray());
            }

            return default;
        }

        /// <summary>
        /// Вызвать метод у переданного типа с объектом в виде первого аргумента
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="sourceType">Класс, в котором ищется метод для вызова</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <returns>Результат вызова метода</returns>
        public static object CallExtension(this object @object, string method, Type sourceType, params object[] argsObj)
        {
            var methodInfo = sourceType.GetMethods().FirstOrDefault(m => m.Name == method);
            if (methodInfo != default)
            {
                var combinedArgs = new List<object> { @object };
                combinedArgs.AddRange(argsObj);
                var @params = combinedArgs.Select(a => Expression.Parameter(a.GetType())).ToArray();
                var methodCall = Expression.Call(null, methodInfo, @params);
                return Expression.Lambda(methodCall, @params).Compile().DynamicInvoke(combinedArgs.ToArray());
            }

            return default;
        }

        /// <summary>
        /// Вызвать метод у переданного типа с объектом в виде первого аргумента
        /// и определенным результатом вызова метода
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="sourceType">Класс, в котором ищется метод для вызова</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <typeparam name="T">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static T CallExtension<T>(this object @object, string method, Type sourceType, params object[] argsObj) =>
            @object.CallExtension(method, sourceType, argsObj).As<T>();

        /// <summary>
        /// Вызвать дженерик-метод у переданного типа с объектом в виде первого аргумента
        /// и определенным результатом вызова метода
        /// </summary>
        /// <param name="object">Объект</param>
        /// <param name="method">Имя метода</param>
        /// <param name="sourceType">Класс, в котором ищется метод для вызова</param>
        /// <param name="generic">Набор дженериков</param>
        /// <param name="argsObj">Набор аргументов</param>
        /// <typeparam name="T">Тип результата вызова метода</typeparam>
        /// <returns>Результат вызова метода</returns>
        public static T CallGenericExtension<T>(this object @object, string method, Type sourceType, Type[] generic, params object[] argsObj) =>
            @object.CallGenericExtension(method, sourceType, generic, argsObj).As<T>();
    }
}
