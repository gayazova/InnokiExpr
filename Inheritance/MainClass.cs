namespace Inheritance
{
    public class MainClass : DadClass
    {
        public string MainName { get; set; }

        public static List<MainClass> List()
        {
            return new List<MainClass>()
            {
                new ()
                {
                    Id = 1, AncestorName = "AncestorName1", DadName = "DadName1", MainName = "MainMame1"
                },
                new ()
                {
                    Id = 2, AncestorName = "AncestorName2", DadName = "DadName2", MainName = "MainMame1"
                },
                new()
                {
                    Id = 3, AncestorName = "AncestorName3", DadName = "DadName3", MainName = "MainMame3"
                }
            };
        }
    }
}
