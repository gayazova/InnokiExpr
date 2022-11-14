namespace LinqToDb
{
    public class User
    {
        public long Id { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string NsiAccesses { get; set; }

        public string Email { get; set; }

        public string IPAddress { get; set; }

        public bool IsAdmin { get; set; }

        public string AuthToken { get; set; }

        public string Name { get; set; }

        public string DashboardIdToDisplay { get; set; }
    }
}
