using System;

namespace SuperGitProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                GitRepository.CreateBranch("Test1");
                GitRepository.Pull();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            Console.WriteLine("Hello, World!");
        }
    }
}