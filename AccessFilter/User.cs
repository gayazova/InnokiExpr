namespace AccessFilter
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Car> Cars { get; set; } = new List<Car>() { new Car { Id = 1, Name = "ereen", Model = "nice"} };
    }

    public class Car
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }
    }
}
