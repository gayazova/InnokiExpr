namespace Inheritance
{
    public class SqlProcessor
    {
        public static Dictionary<string, Dictionary<string, List<string>>> ToData<T>(IEnumerable<T> mainClasses)
        {
            var processedDict = new List<string>();
            var typeStack = new Stack<Type>();
            var currentType = typeof(T);
            while (currentType?.BaseType != null)
            {
                if (currentType != null)
                    typeStack.Push(currentType);
                currentType = currentType?.BaseType;
            }
            var dict = new Dictionary<string, Dictionary<string, List<string>>>();

            while(typeStack.Count > 0)
            {
                var type = typeStack.Pop();
                dict[type.Name] = new Dictionary<string, List<string>>();
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    if (property.Name == "Id" || processedDict.Contains(property.Name))
                        continue;
                    dict[type.Name][property.Name] = Values(mainClasses, property.Name);
                    processedDict.Add(property.Name);
                }
            }

            return dict;
        }

        private static List<string> Values<T>(IEnumerable<T> entities, string propertyName)
        {
            var result = new List<string>();
            foreach(var entity in entities)
            {
                var value = entity?.GetType()?.GetProperty(propertyName)?.GetValue(entity);
                result.Add(ToFormatString(value));
            }
            return result;
        }

        private static string ToFormatString(object fieldValue)
        {
            return fieldValue == null ? "null" : fieldValue.ToString(); //default
        }

    }
}
