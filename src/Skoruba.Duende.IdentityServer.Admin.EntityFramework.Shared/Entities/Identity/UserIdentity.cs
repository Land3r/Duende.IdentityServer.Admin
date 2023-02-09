// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Identity;
using System;

namespace Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity
{
    public class UserIdentity<TKey> : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}