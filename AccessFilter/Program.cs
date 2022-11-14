using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace AccessFilter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var users = new List<User>() { new User { Id = 1, Name = "Test" } }.ToArray() ;
            var dict = new Dictionary<Type, IEnumerable<object>>();
            dict[typeof(User)] = users;
            var selectionParams = new List<SelectionParameter>()
            {
                new SelectionParameter()
                {
                    Operator = SelectionOperator.Equals,
                    PropertyName = nameof(Document.Name),
                    Value = "User.Name"
                }
            };

            var test1 = (IEnumerable<User>)users; 
            var test = test1.AsQueryable<User>();
            
            var groupNames = new List<string>() { "main", "fam"};
            var values = new List<int>() { 1, 3, 4, 5 };
            var doc = new List<Document>()
                {
                    new Document()
                        {
                            DocId = 1,
                            Name = "main",
                            Value = 1001
                    },
                    new Document()
                        {
                            DocId = 2,
                            Name = "Test",
                            Value = 1002
                    },
                    new Document()
                        {
                            DocId = 3,
                            Name = null,
                            Value = 1003
                    }
                 }.AsQueryable();

            var tt = doc.Where(x => users.Any(uu => uu.Id == x.DocId));
            var asQu = users.Select(x => x.Name).ToList().AsQueryable();

            var exprConst = Expression.Constant(users);
            var userNames = users.Select(x => x.Name).ToArray();

            var matchParam = Expression.Parameter(typeof(User), "mp");
            var prop = MatchPropExpression("User.Name", matchParam);
            var lamda = Expression.Lambda(prop, matchParam);
            var select = EnumerableSelect();
            var selectGeneric = select.MakeGenericMethod(typeof(User), typeof(string));
            var callSelect = Expression.Call(null, selectGeneric, exprConst, lamda);
            var toList = EnumerableToList();
            var toListGeneric = toList.MakeGenericMethod(typeof(string));
            var callToList = Expression.Call(null, toListGeneric, callSelect);
            var lamdaExpr = Expression.Lambda(callToList).Compile().DynamicInvoke();
            var exprConst2 = Expression.Constant(lamdaExpr);

            var paramExpr = Expression.Parameter(typeof(string), "name");
            var docParam = Expression.Parameter(doc.T(), "qq");
            var docProp = Expression.Property(docParam, "Name");
            var lessExpression = Expression.Equal(paramExpr, docProp);
            var lamdaSec = Expression.Lambda(lessExpression, paramExpr);
            var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { typeof(string) }, exprConst2);
            var callExpresson = Expression.Call(typeof(Queryable), "Any", new[] { typeof(string) }, queryable, lamdaSec);
            var firstLamda = Expression.Lambda(callExpresson, docParam);
            var resultT = typeof(Queryable)
                .GetMethods()
                .FirstOrDefault(x => x.Name == "Where")
                .MakeGenericMethod(doc.T())
                .Invoke(null, new object[] { doc, firstLamda });
            var resultAs = resultT.As<IQueryable<Document>>();
            var listRes = resultAs.ToList();


            var check = doc.Where(dd => userNames.Any(un => un == dd.Name));


            var docResult = doc.Where(dd => users.Any(uu => uu.Id < dd.DocId) && users.Any(uu => uu.Cars.Any(pp => dd.Name == pp.Name)));

            var smc = new SelectionModelMetaConverter();
            try
            {
                var result = smc.ConvertParams(selectionParams, doc, dict);
                foreach(var res in result)
                {
                    Console.WriteLine($"{res.DocId} {res.Value} {res.Name}");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            Console.WriteLine("Hello, World!");
        }

        /// <summary>
        ///
        /// <para>
        /// [Кэшируемый]
        /// </para>
        /// </summary>
        public static MethodInfo EnumerableSelect()
        {
            if (enumerableSelect == default)
            {
                lock (enumerableDelectLock)
                {
                    if (enumerableSelect == default)
                    {
                        enumerableSelect = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "Select" && m.GetParameters().Length == 2);
                    }
                }
            }

            return enumerableSelect;
        }
        private static MethodInfo enumerableSelect;
        private static object enumerableDelectLock = new object();

        public static MethodInfo EnumerableToList()
        {
            if (enumerableToList == default)
            {
                lock (enumerableToListLock)
                {
                    if (enumerableToList == default)
                    {
                        enumerableToList = typeof(Enumerable).GetMethods().FirstOrDefault(m => m.Name == "ToList" && m.GetParameters().Length == 1);
                    }
                }
            }

            return enumerableToList;
        }
        private static MethodInfo enumerableToList;
        private static object enumerableToListLock = new object();

        public static Expression MatchPropExpression(string propName, ParameterExpression matchParameterExpression)
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

        //var docResult = doc.Where(dd => users.Any(uu => uu.Id < dd.DocId));
        //var propName = "User.Id";

        //var docParam = Expression.Parameter(doc.T(), "qq");
        //var docProp = Expression.Property(docParam, "DocId");
        //var propertiesList = propName.Split(".", StringSplitOptions.RemoveEmptyEntries);
        //var matchType = typeof(User);//.Assembly.GetType(propertiesList[0]); // Type.PropertyName -> Type
        //var usersParam = Expression.Parameter(matchType, "uu");
        //var list = dict[matchType];
        //var usersProperty = MatchPropExpression(propName, usersParam);
        //var lessExpr = Expression.LessThan(usersProperty, docProp);
        //var lambdaUser = Expression.Lambda(lessExpr, usersParam);
        //var queryableUser = Expression.Call(typeof(Queryable), "AsQueryable", new[] { typeof(User) }, Expression.Constant(list));
        //var callUserExpresson = Expression.Call(typeof(Queryable), "Any", new[] { typeof(User) }, queryableUser, lambdaUser);
        //var firstLamda = Expression.Lambda(callUserExpresson, docParam);


        ////var param = Expression.Parameter(doc.T(), "qq");
        ////var userParam = Expression.Parameter(typeof(User), "uu");
        ////var docProperty = Expression.Property(param, "DocId");
        ////var userProperty = Expression.Property(userParam, "Id");
        ////var lessExpression = Expression.LessThan(userProperty, docProperty);
        ////var lamdaSec = Expression.Lambda(lessExpression, userParam);
        ////var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { typeof(User) }, Expression.Constant(users));
        ////var callExpresson = Expression.Call(typeof(Queryable), "Any", new[] { typeof(User) }, queryable, lamdaSec);
        ////var firstLamda = Expression.Lambda(callExpresson, param);

        //var result = typeof(Queryable)
        //    .GetMethods()
        //    .FirstOrDefault(x => x.Name == "Where")
        //    .MakeGenericMethod(doc.T())
        //    .Invoke(null, new object[] { doc, firstLamda });
        //var resultAs = result.As<IQueryable<Document>>(); 
        //var listRes = resultAs.ToList();

        //private Expression GetConstantExpression<T>(string propName, IEnumerable<T> matchVals)
        //{
        //    var paramExpr = matchVals.Select()
        //}

        /*
             *  None (нет),
                GreaterThan (больше, чем),
                GreaterThanOrEquals (больше или равно),
                LessThan (меньше, чем),
                LessThanOrEquals (меньше или равно,
                Equals (равно),
                NotEqual (не равно),
                Contains (содержит, включает в себя),
                NotContains (не содержит, не включает в себя),
                StartsWith (начинается с),
                EndsWith (оканчивается).

        var greaterThan = doc.Where(dd => values.Any(val => dd.Value > val));
        var greaterThanEqual = doc.Where(dd => values.Any(val => dd.Value >= val));
        var lessThan = doc.Where(dd => values.Any(val => dd.Value < val));
        var lessThanEquals = doc.Where(dd => values.Any(val => dd.Value <= val));
        var notEquals = doc.Where(dd => values.Any(val => dd.Value != val));
        var equals = doc.Where(dd => groupNames.Contains(dd.Name));
      */
    }

    //private Expression BuildNestedExpression<T>(
    //        Expression expression,
    //        IEnumerator<string> propertyEnumerator,
    //        SelectionParameter selectionParam,
    //        Type propType,
    //        Dictionary<Type, IEnumerable<object>> matchList)
    //{
    //    while (propertyEnumerator.MoveNext())
    //    {
    //        var propertyName = propertyEnumerator.Current;
    //        var property = expression.Type.GetProperty(propertyName);
    //        expression = Expression.Property(expression, property);

    //        var propertyType = property.PropertyType;
    //        var enumerable = propertyType.GetInterface("IEnumerable`1");
    //        if (propertyType != typeof(string) && enumerable != null)
    //        {
    //            var elementType = enumerable.GetGenericArguments()[0];
    //            var predicateFnType = typeof(Func<,>).MakeGenericType(elementType, typeof(bool));
    //            var parameterExpress = Expression.Parameter(elementType, "bvc");
    //            var body = BuildNestedExpression<T>(parameterExpress, propertyEnumerator, selectionParam, propType, matchList);
    //            var predicate = Expression.Lambda(predicateFnType, body, parameterExpress);
    //            var queryable = Expression.Call(typeof(Queryable), "AsQueryable", new[] { elementType }, expression);
    //            var collectionMode = selectionParam.CollectionMode == CollectionMode.Any ? "Any" : "All";
    //            var filterCall = Expression.Call(typeof(Queryable), collectionMode, new[] { elementType }, queryable, predicate);
    //            if (selectionParam.CollectionMode == CollectionMode.Any)
    //                return filterCall;
    //            return Expression.And(Expression.Call(typeof(Queryable), "Any", new[] { elementType }, queryable), filterCall); // x => x.Collection.Any() && x.Collection.All(predicate);
    //        }
    //    }

    //    return BuildOperatorExpression<T>(expression, selectionParam, propType, expression, matchList);
    //}
}
