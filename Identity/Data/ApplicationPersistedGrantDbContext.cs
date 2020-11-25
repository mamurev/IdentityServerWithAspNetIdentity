using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{

    public class ApplicationPersistedGrantDbContext : PersistedGrantDbContext<ApplicationPersistedGrantDbContext>
    {
        public ApplicationPersistedGrantDbContext(DbContextOptions<ApplicationPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Model.SetMaxIdentifierLength(30);
        }
    }
}
