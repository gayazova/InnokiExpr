namespace AccessFilter
{
    public static class QueryArgumentT
    {
        public static Type T<T>(this IQueryable<T> query) => query.GetType().GetGenericArguments()[0];
    }
}
