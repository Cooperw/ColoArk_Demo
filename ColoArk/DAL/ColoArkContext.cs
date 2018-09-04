using ColoArk.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ColoArk.DAL
{
    public class ColoArkContext : IdentityDbContext<ApplicationUser>
    {
        public ColoArkContext(DbContextOptions<ColoArkContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<ColoArk.Models.User> ColoUsers { get; set; }
        public DbSet<Mailbox> Mailboxes { get; set; }
        public DbSet<GiveawayReceipt> GiveawayReceipts { get; set; }
        public DbSet<GiveawayDrop> GiveawayDrops { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //Begin Cascade Protection
            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            //End Cascade Protection

            //One-to-Many
            builder.Entity<User>()
                .HasMany<GiveawayReceipt>(c => c.GiveawayReceipts)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserID);
        }
    }
}
