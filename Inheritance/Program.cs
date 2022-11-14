// See https://aka.ms/new-console-template for more information


using System.Text;

namespace Inheritance
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var list = MainClass.List();
            var dict = SqlProcessor.ToData(list);

            var ids = new List<string>() { "4, 5, 6" };

            var parentTable = true;
            var stringBuilder = new StringBuilder();
            foreach (var item in dict)
            {
                if (parentTable)
                {
                    var columnNames = $"({string.Join(",", item.Value.Keys)})";
                    var sql = Print(item.Value, list, parentTable);
                    Console.WriteLine(columnNames);
                    Console.WriteLine(sql);
                    parentTable = false;
                }
                else
                {
                    var columnList = item.Value.Keys.ToList();
                    columnList.Insert(0, "Id");
                    var columnNames = $"({string.Join(",", columnList)})";
                    var sql = Print(item.Value, list, parentTable);
                    Console.WriteLine(columnNames);
                    Console.WriteLine(sql);
                }
                Console.WriteLine();
            }

            Console.ReadLine();
            Console.WriteLine("Hello, World!");
        }

        private static string Print<T>(Dictionary<string, List<string>> info, IEnumerable<T> entities, bool ancesor)
        {
            var length = info.First().Value.Count();
            var arr = new List<string>[length];
            if (!ancesor)
            {
                for (int i = 0; i < length; i++)
                {
                    var entity = entities.ElementAt(i);
                    var id = entity.GetType().GetProperty("Id").GetValue(entity, null).ToString();
                    arr[i] = new List<string> { id };
                }
            }
            for (int i = 0; i < length; i++)
            {
                foreach (var (columnName, columnValues) in info)
                {
                    if (arr[i] == default)
                    {
                        arr[i] = new List<string> { columnValues[i] };
                    }
                    else
                    {
                        arr[i].Add(columnValues[i]);
                    }
                }
            }
            var valuesList = arr.Select(value => $"({string.Join(",", value)})").ToList();
            return string.Join(",", valuesList);
        }
    }
}
