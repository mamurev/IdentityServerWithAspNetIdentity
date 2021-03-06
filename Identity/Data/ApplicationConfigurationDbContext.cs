﻿using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    public class ApplicationConfigurationDbContext : ConfigurationDbContext<ApplicationConfigurationDbContext>
    {
        public ApplicationConfigurationDbContext(DbContextOptions<ApplicationConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
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
