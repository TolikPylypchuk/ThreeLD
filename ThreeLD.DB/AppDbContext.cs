using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;

using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;

namespace ThreeLD.DB
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
        }

        static AppDbContext()
        {
            Database.SetInitializer(new DbInit());
        }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Preference> Preferences { get; set; }
    }
}
