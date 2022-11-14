using Newtonsoft.Json.Linq;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace AccessFilter
{
    public class SelectionModelMetaConverter
    {
        public IQueryable Original { get; private set; }

        public IQueryable<T> ConvertParams<T>(List<SelectionParameter> @params, IQueryable<T> query, Dictionary<Type, IEnumerable<object>> matchLists)
        {
            foreach (var param in @params)
            {
                query = ParseParam(query, param, matchLists);
            }

            return query;
        }



        private Type GetPropertyType<T>(string propName)
        {
            var propertiesList = propName.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var currentType = typeof(T);
            var iter = 0;
            while (iter < propertiesList.Length)
            { 
                var propInfo = currentType.GetProperties().FirstOrDefault(x => x.Name.EqualsIgnoreCase(propertiesList[iter]));
                if (propInfo != default)
                {
                    if (iter == propertiesList.Length - 1)
                    {
                        return propInfo.PropertyType;
                    }

                    var enumerable = propInfo.PropertyType.GetInterface("IEnumerable`1");
                    if (propInfo.PropertyType != typeof(string) && enumerable != null)
                    {
                        currentType = enumerable.GetGenericArguments()[0];
                    }
                    else
                    {
                        currentType = propInfo.PropertyType;
                    }
                }

                iter++;
            }
            return default;
        }

        public IQueryable<T> ParseParam<T>(IQueryable<T> query, SelectionParameter selectionParam, Dictionary<Type, IEnumerable<object>> matchLists)
        {
            var selectionIsDefault = selectionParam?.PropertyName == default
                                     && selectionParam?.Value == default
                                     && selectionParam?.Parameters?.Count == 0;

            if (selectionParam == default || selectionIsDefault)
                return query;

            var param = Expression.Parameter(query.T(), "m");

            var expr = BuildExpressionTree(selectionParam, param, query, matchLists);

            if (expr == default) return query;

            var lambda = Expression.Lambda(expr, param);

            var result = typeof(Queryable)
                .GetMethods()
                .FirstOrDefault(x => x.Name == "Where")
                .MakeGenericMethod(query.T())
                .Invoke(null, new object[] { query, lambda });

            return result.As<IQueryable<T>>();//.Where(lambda);
        }

        private Expression BuildExpressionTree<T>(
            SelectionParameter selectionParam,
            ParameterExpression parameterExpression,
            IQueryable<T> query,
            Dictionary<Type, IEnumerable<object>>? matchLists)
        {
            Expression expr = default;
            if (selectionParam.Parameters.Any())
            {
                var expressions = new List<Expression>();
                selectionParam.Parameters.ForEach(childRule =>
                {
                    var expression = BuildExpressionTree<T>(childRule, parameterExpression, query, matchLists);
                    expressions.Add(expression);
                });

                var expressionTree = expressions.Where(expression => expression != null).First();

                var counter = 1;
                while (counter < expressions.Count)
                {
                    expressionTree = selectionParam.Operator == SelectionOperator.Or
                                         ? Expression.Or(expressionTree, expressions[counter])
                                         : Expression.And(expressionTree, expressions[counter]);
                    counter++;
                }

                expr = expressionTree;
                return expr;
            }

            var propType = GetPropertyType<T>(selectionParam.PropertyName);
            if (propType == default)
            {
                if (typeof(Parameterful).IsAssignableFrom(query.T()))
                {
                    var parametersProp = Expression.Property(parameterExpression, nameof(Parameterful.Parameters));

                    var parameterValue = Expression.Property(parametersProp, "Item", Expression.Constant(selectionParam.PropertyName));

                    var equalsExpression = Expression.Equal(parameterValue, Expression.Constant(selectionParam.Value));

                    expr = equalsExpression;
                    return expr;
                }

                return default;
            }

            var propertyList = selectionParam.PropertyName.Split(".", StringSplitOptions.RemoveEmptyEntries);
            if (propertyList.Length > 1)
            {
                var enumerator = propertyList.AsEnumerable().GetEnumerator();
                return BuildNestedExpression<T>(parameterExpression, enumerator, selectionParam, propType, matchLists);
            }
            else
            {
                Expression prop = Expression.Property(parameterExpression, selectionParam.PropertyName);
                return BuildOperatorExpression<T>(expr, selectionParam, propType, prop, matchLists);
            }
        }

        private MethodInfo GetStringClassMethod(string methodName)
            => typeof(string)
            .GetMethods()
            .FirstOrDefault(m =>
                m.Name == methodName
                && m.GetParameters().FirstOrDefault()?.ParameterType == typeof(string));

        /// <summary>
        ///
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public MethodInfo EnumerableContains()
        {
            if (enumerableContains == default)
            {
                lock (enumerableContainsLock)
                {
                    if (enumerableContains == default)
                    {
                        enumerableContains = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "Contains" && m.GetParameters().Length == 2);
                    }
                }
            }

            return enumerableContains;
        }
        private MethodInfo enumerableContains;
        private object enumerableContainsLock = new object();

        private object ConvertJArrayToPropType<TProperty>(JArray array)
        {
            if (typeof(TProperty) != typeof(Guid))
                return array.Select(x => x.Value<TProperty>()).ToArray();

            var guidList = new List<Guid>();

            foreach (var jToken in array)
            {
                if (Guid.TryParse(jToken.ToString(), out var result))
                    guidList.Add(result);
            }

            return guidList;

        }

        private Expression BuildOperatorExpression<T>(
            Expression expr,
            SelectionParameter selectionParam,
            Type propType,
            Expression prop,
            Dictionary<Type, IEnumerable<object>> matchList)
        {
            object value = selectionParam.Value;

            if (Nullable.GetUnderlyingType(propType) != null)
            {
                 prop = Expression.Convert(prop, Nullable.GetUnderlyingType(propType));
            }

            if (propType != typeof(string) && selectionParam.Operator == SelectionOperator.Contains)
            {
                var valueType = selectionParam.Value.GetType();
                if (valueType.IsArray)
                    valueType = valueType.GetElementType();

                if (valueType == typeof(object))
                {
                    var list = typeof(List<>).MakeGenericType(propType).New();
                    if (value is IEnumerable @enum)
                    {
                        foreach (var item in @enum)
                        {
                            list.Call("Add", item);
                        }
                        value = list;
                    }
                }
                else if (valueType == typeof(JArray))
                {
                    value = this.CallGeneric(nameof(ConvertJArrayToPropType), new[] { propType }, value as JArray);
                }
                else
                {
                    value = selectionParam.Value;
                }
            }

            //TODO get 
            var mathProps = selectionParam.Value.ToString();
            var mathPropsList = mathProps.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var matchType = typeof(User);//.Assembly.GetType(propertiesList[0]); // Type.PropertyName -> Type
            var usersParam = Expression.Parameter(matchType, "uu");
            var listMatch = matchList[matchType];
            var usersProperty = MatchPropExpression(mathProps, usersParam);

            if (propType.IsNumericType())
            {
                switch (selectionParam.Operator)
                {
                    case SelectionOperator.GreaterThan:
                        expr = Expression.GreaterThan(prop, usersProperty);
                        break;
                    case SelectionOperator.GreaterThanOrEquals:
                        expr = Expression.GreaterThanOrEqual(prop, usersProperty);
                        break;
                    case SelectionOperator.LessThan:
                        expr = Expression.LessThan(prop, usersProperty);
                        break;
                    case SelectionOperator.LessThanOrEquals:
                        expr = Expression.LessThanOrEqual(prop, usersProperty);
                        break;
                    case SelectionOperator.Contains:
                        expr = Expression.Call(null, EnumerableContains().MakeGenericMethod(propType), usersProperty, prop);
                        break;
                    case SelectionOperator.Equals:
                        if (expr != null)
                        {
                            expr = Expression.Equal(expr, usersProperty);
                        }
                        break;
                }
            }

            if (propType == typeof(string))
            {
                var notNull = Expression.NotEqual(prop, Expression.Constant(default(string)));
                prop = Expression.Call(prop, "ToUpper", null);
                var userPropNotNull = Expression.NotEqual(usersProperty, Expression.Constant(default(string)));
                var exprValue = Expression.Call(usersProperty, "ToUpper", null);
                var andAlso = Expression.AndAlso(notNull, userPropNotNull);
                MethodInfo stringMethodInfo;
                switch (selectionParam.Operator)
                {
                    case SelectionOperator.Contains:
                        stringMethodInfo = GetStringClassMethod(nameof(string.Contains));
                        expr = Expression.AndAlso(andAlso, Expression.Call(prop, stringMethodInfo, exprValue));
                        break;
                    case SelectionOperator.NotContains:
                        stringMethodInfo = GetStringClassMethod(nameof(string.Contains));
                        var ctnExpr = Expression.AndAlso(andAlso, Expression.Call(prop, stringMethodInfo, exprValue));
                        expr = Expression.Not(ctnExpr);
                        break;
                    case SelectionOperator.StartsWith:
                        stringMethodInfo = GetStringClassMethod(nameof(string.StartsWith));
                        expr = Expression.AndAlso(andAlso, Expression.Call(prop, stringMethodInfo, exprValue));
                        break;
                    case SelectionOperator.EndsWith:
                        stringMethodInfo = GetStringClassMethod(nameof(string.EndsWith));
                        expr = Expression.AndAlso(andAlso, Expression.Call(prop, stringMethodInfo, exprValue));
                        break;
                    case SelectionOperator.Equals:
                        expr = Expression.AndAlso(andAlso, Expression.Equal(prop, exprValue));
                        break;
                    case SelectionOperator.NotEqual:
                        expr = Expression.AndAlso(andAlso, Expression.NotEqual(prop, exprValue));
                        break;
                }
            }

            if (propType == typeof(Guid))
            {
                switch (selectionParam.Operator)
                {
                    case SelectionOperator.Contains:
                        expr = Expression.Call(null, EnumerableContains().MakeGenericMethod(propType), usersProperty, prop);
                        break;
                }
            }

            if (expr == default)
            {
                switch (selectionParam.Operator)
                {
                    case SelectionOperator.Equals:
                        expr = Expression.Equal(prop, usersProperty);
                        break;
                    case SelectionOperator.NotEqual:
                        expr = Expression.NotEqual(prop, usersProperty);
                        break;
                }
            }

            var lambda = Expression.Lambda(expr, usersParam);
            var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { matchType }, Expression.Constant(listMatch));
            expr = Expression.Call(typeof(Queryable), "Any", new[] { typeof(User) }, queryable, lambda);
            return expr;
        }

        public Expression MatchPropExpression(string propName, ParameterExpression matchParameterExpression)
        {
            var propertiesList = propName.Split(".", StringSplitOptions.RemoveEmptyEntries);
            var iter = 1;
            Expression prop = default;
            while (iter < propertiesList.Length)
            {
                if (prop == default)
                {
                    prop = Expression.Property(matchParameterExpression, propertiesList[iter]);
                }
                else
                {
                    prop = Expression.Property(prop, propertiesList[iter]);
                }

                iter++;
            }
            return prop;
        }

        private Expression BuildNestedExpression<T>(
            Expression expression,
            IEnumerator<string> propertyEnumerator,
            SelectionParameter selectionParam,
            Type propType,
            Dictionary<Type, IEnumerable<object>> matchList)
        {
            while (propertyEnumerator.MoveNext())
            {
                var propertyName = propertyEnumerator.Current;
                var property = expression.Type.GetProperty(propertyName);
                expression = Expression.Property(expression, property);

                var propertyType = property.PropertyType;
                var enumerable = propertyType.GetInterface("IEnumerable`1");
                if (propertyType != typeof(string) && enumerable != null)
                {
                    var elementType = enumerable.GetGenericArguments()[0];
                    var predicateFnType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
                    var parameterExpress = Expression.Parameter(elementType, "bvc");
                    var body = BuildNestedExpression<T>(parameterExpress, propertyEnumerator, selectionParam, propType, matchList);
                    var predicate = Expression.Lambda(predicateFnType, body, parameterExpress);
                    var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { elementType }, expression);
                    var collectionMode = selectionParam.CollectionMode == CollectionMode.Any ? "Any" : "All";
                    var filterCall = Expression.Call(typeof(Queryable), collectionMode, new[] { elementType }, queryable, predicate);
                    if (selectionParam.CollectionMode == CollectionMode.Any)
                        return filterCall;
                    return Expression.And(Expression.Call(typeof(Queryable), "Any", new[] { elementType }, queryable), filterCall); // x => x.Collection.Any() && x.Collection.All(predicate);
                }
            }

            return BuildOperatorExpression<T>(expression, selectionParam, propType, expression, matchList);
        }

        public Expression Predicate<T>(SelectionParameter model)
        {
            IQueryable<T> query = Enumerable.Empty<T>().AsQueryable();
            var selectionIsDefault = model?.PropertyName == default
                                     && model?.Value == default
                                     && model?.Parameters?.Count == 0;
            if (model == default || selectionIsDefault)
                return default;
            var param = Expression.Parameter(typeof(T), "m");
            var expr = BuildExpressionTree(model, param, query, default);
            return expr;
        }
    }
}
