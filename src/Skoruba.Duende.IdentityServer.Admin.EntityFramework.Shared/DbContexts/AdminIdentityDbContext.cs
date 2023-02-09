// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Constants;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using System;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminIdentityDbContext<TKey> : IdentityDbContext<UserIdentity<TKey>, UserIdentityRole<TKey>, TKey, UserIdentityUserClaim<TKey>, UserIdentityUserRole<TKey>, UserIdentityUserLogin<TKey>, UserIdentityRoleClaim<TKey>, UserIdentityUserToken<TKey>>
        where TKey : IEquatable<TKey>
    {
        public AdminIdentityDbContext(DbContextOptions<AdminIdentityDbContext<TKey>> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            ConfigureIdentityContext(builder);
        }

        private void ConfigureIdentityContext(ModelBuilder builder)
        {
            builder.Entity<UserIdentityRole<TKey>>().ToTable(TableConsts.IdentityRoles);
            builder.Entity<UserIdentityRoleClaim<TKey>>().ToTable(TableConsts.IdentityRoleClaims);
            builder.Entity<UserIdentityUserRole<TKey>>().ToTable(TableConsts.IdentityUserRoles);

            builder.Entity<UserIdentity<TKey>>().ToTable(TableConsts.IdentityUsers);
            builder.Entity<UserIdentityUserLogin<TKey>>().ToTable(TableConsts.IdentityUserLogins);
            builder.Entity<UserIdentityUserClaim<TKey>>().ToTable(TableConsts.IdentityUserClaims);
            builder.Entity<UserIdentityUserToken<TKey>>().ToTable(TableConsts.IdentityUserTokens);
        }
    }
}