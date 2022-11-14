using System.Reflection;

namespace Patterns
{
    public class SingletonTester
    {
        public static bool IsSingleton(Func<object> func)
        {
            var singl = func();
            var type = singl.GetType();
            var ctr = type.GetConstructors(System.Reflection.BindingFlags.Public);
            return ctr.Count() == 0;
        }
    }
}
