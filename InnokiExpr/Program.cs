public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("v16");
        while (true)
        {
            try
            {
                bool a = true;
                bool b = false;
                bool c = false;
                Console.WriteLine(a || b && c);
                Console.ReadKey();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private async Task Test()
    {
    }
}