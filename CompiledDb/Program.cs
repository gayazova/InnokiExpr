using System.Linq.Expressions;
using Z.EntityFramework.Plus;

namespace CompiledDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var db = new AppContext())
            {
                var param = "Лондон";
                var citiesQueryWithParam = db.City.Where(x => x.Name == param);
                PrintWhere(citiesQueryWithParam.Expression, true);
                Console.WriteLine(citiesQueryWithParam.Expression.ToString());

                //var citiesQueryWithConst = db.City.Where(x => x.Name == "Лондон");
                //PrintWhere(citiesQueryWithConst.Expression);
                //Console.WriteLine(citiesQueryWithConst.Expression.ToString());

                //var citiesQueryWithGroupBy = db.City.GroupBy(x => x.Name).Select(x => x.First());
                //var exprWithGroupBy = citiesQueryWithGroupBy.Expression.ToString();
                //Console.WriteLine(exprWithGroupBy);

                //var citiesQieryWithJoin = db.City.Join(db.Salary, s => s.Name, c => c.Name, (city, salary) => city);
                //var exprWithJoin = citiesQieryWithJoin.Expression.ToString();
                //Console.WriteLine(exprWithJoin);
            }

            Console.ReadKey();
        }


        private static void PrintWhere(Expression expressionFromQuery, bool isParam = false)
        {
            var arguments = ((MethodCallExpression)expressionFromQuery).Arguments;
            var operand = ((UnaryExpression)arguments[1]).Operand;
            var lambdaOperand = (LambdaExpression)operand;
            var parameters = lambdaOperand.Parameters;
            foreach (var parameter in parameters)
            {
                Console.WriteLine(parameter);
            }
            var binaryBody = (BinaryExpression)lambdaOperand.Body;
            Console.WriteLine(binaryBody.Left);
            Console.WriteLine(binaryBody.Right);
            if (isParam)
            {
                var fieldExpression = ((MemberExpression)binaryBody.Right);
                //fieldExpression.Expression = Expression.Constant("Лондон");
                
            }
        }

        //private static readonly Func<AppContext, int, IEnumerable<City>> compiledQuery =
        //    EF.CompileQuery((AppContext ctx, int id) => ctx.City.Where(x => x.Id == id));


        //private static readonly Func<AppContext, int, IQueryable<City>> compiledQuery
        //    = CompiledQuery.Compile<AppContext, int, IQueryable<City>>((ctx, id) => ctx.City.Where(x => x.Id == id));

        //var id = 1;
        //var cityQuery = compiledQuery.Invoke(db, id);
        //var city = cityQuery.FirstOrDefault();

        //var test = citiesQuery.FromCache()
        //        var cach = Z.EntityFramework.Plus.QueryCacheManager.GetCacheKey(citiesQuery, new string[] { "tag1" });
    }
}