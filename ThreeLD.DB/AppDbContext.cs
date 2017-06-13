using System.Data.Entity;

using Microsoft.AspNet.Identity.EntityFramework;

//using ThreeLD.DB.Infrastructure;
using ThreeLD.DB.Models;

namespace ThreeLD.DB
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext() : base("name=DefaultConnection")
        {
        }

        //static AppDbContext()
        //{
        //    Database.SetInitializer(new DbInit());
        //}

        public DbSet<Event> Events { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppDbContext>(null);

            modelBuilder.Entity<Notification>()
                        .HasRequired(n => n.User)
                        .WithMany(u => u.IncomingNotifications)
                        .HasForeignKey(n => n.To)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Notification>()
                        .HasRequired(n => n.Editor)
                        .WithMany(u => u.OutcomingNotifications)
                        .HasForeignKey(n => n.From)
                        .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }
    }
}
