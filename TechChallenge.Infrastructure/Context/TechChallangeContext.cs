using Microsoft.EntityFrameworkCore;
using TechChallenge.Domain.Contact.Entity;

namespace TechChallenge.Infrastructure.Context
{
    public class TechChallangeContext : DbContext
    {
        public TechChallangeContext() : base()
        {
            // Database.Migrate();
        }

        public TechChallangeContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
        public DbSet<ContactEntity> Contact { get; set; }
    }
}
