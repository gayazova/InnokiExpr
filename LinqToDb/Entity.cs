namespace LinqToDb
{
    public abstract class Entity<TId>
    {
        public TId Id { get; set; }
    }
}
