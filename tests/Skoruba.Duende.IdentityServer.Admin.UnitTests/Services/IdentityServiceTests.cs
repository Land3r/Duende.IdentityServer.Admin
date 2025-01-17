﻿// Copyright (c) Jan Škoruba. All Rights Reserved.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Moq;
using Skoruba.AuditLogging.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Resources;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Services;
using Skoruba.Duende.IdentityServer.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Identity.Repositories;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.Duende.IdentityServer.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.Duende.IdentityServer.Admin.UnitTests.Mocks;
using Xunit;

namespace Skoruba.Duende.IdentityServer.Admin.UnitTests.Services
{
    public class IdentityServiceTests
    {
        public IdentityServiceTests()
        {
            var databaseName = Guid.NewGuid().ToString();

            _dbContextOptions = new DbContextOptionsBuilder<AdminIdentityDbContext<string>>()
                .UseInMemoryDatabase(databaseName)
                .Options;
        }

        private readonly DbContextOptions<AdminIdentityDbContext<string>> _dbContextOptions;

        private IIdentityRepository<UserIdentity<string>, UserIdentityRole<string>, string,
            UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
            UserIdentityUserToken<string>> GetIdentityRepository(AdminIdentityDbContext<string> dbContext,
            UserManager<UserIdentity<string>> userManager,
            RoleManager<UserIdentityRole<string>> roleManager,
            IMapper mapper)
        {
            return new IdentityRepository<AdminIdentityDbContext<string>, UserIdentity<string>, UserIdentityRole<string>, string,
                UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
                UserIdentityUserToken<string>>(dbContext, userManager, roleManager, mapper);
        }

        private IIdentityService<UserDto<string>, RoleDto<string>, UserIdentity<string>,
            UserIdentityRole<string>, string,
            UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
            UserIdentityUserToken<string>,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetIdentityService(IIdentityRepository<UserIdentity<string>, UserIdentityRole<string>, string, UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>, UserIdentityUserToken<string>> identityRepository,
            IIdentityServiceResources identityServiceResources,
            IMapper mapper, IAuditEventLogger auditEventLogger)
        {
            return new IdentityService<UserDto<string>, RoleDto<string>, UserIdentity<string>,
                UserIdentityRole<string>, string,
                UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
                UserIdentityUserToken<string>,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>(identityRepository, identityServiceResources, mapper, auditEventLogger);
        }

        private IMapper GetMapper()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile<IdentityMapperProfile<UserDto<string>, RoleDto<string>, UserIdentity<string>, UserIdentityRole<string>, string,
                        UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
                        UserIdentityUserToken<string>,
                        UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                        UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>,
                        RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>>())
                .CreateMapper();
        }

        private UserManager<UserIdentity<string>> GetTestUserManager(AdminIdentityDbContext<string> context)
        {
            var testUserManager = IdentityMock.TestUserManager(new UserStore<UserIdentity<string>, UserIdentityRole<string>, AdminIdentityDbContext<string>, string, UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityUserToken<string>, UserIdentityRoleClaim<string>>(context, new IdentityErrorDescriber()));

            return testUserManager;
        }

        private RoleManager<UserIdentityRole<string>> GetTestRoleManager(AdminIdentityDbContext<string> context)
        {
            var testRoleManager = IdentityMock.TestRoleManager(new RoleStore<UserIdentityRole<string>, AdminIdentityDbContext<string>, string, UserIdentityUserRole<string>, UserIdentityRoleClaim<string>>(context, new IdentityErrorDescriber()));

            return testRoleManager;
        }

        private IIdentityService<UserDto<string>, RoleDto<string>, UserIdentity<string>,
            UserIdentityRole<string>, string,
            UserIdentityUserClaim<string>, UserIdentityUserRole<string>, UserIdentityUserLogin<string>, UserIdentityRoleClaim<string>,
            UserIdentityUserToken<string>,
            UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>,
            UserRolesDto<RoleDto<string>, string>,
            UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
            RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>> GetIdentityService(AdminIdentityDbContext<string> context)
        {
            var testUserManager = GetTestUserManager(context);
            var testRoleManager = GetTestRoleManager(context);
            var mapper = GetMapper();

            var identityRepository = GetIdentityRepository(context, testUserManager, testRoleManager, mapper);
            var localizerIdentityResource = new IdentityServiceResources();

            var auditLoggerMock = new Mock<IAuditEventLogger>();
            var auditLogger = auditLoggerMock.Object;

            var identityService = GetIdentityService(identityRepository, localizerIdentityResource, mapper, auditLogger);

            return identityService;
        }

        [Fact]
        public async Task AddUserAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);
            }
        }

        [Fact]
        public async Task DeleteUserProviderAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                var userProvider = IdentityMock.GenerateRandomUserProviders(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    newUserDto.Id);

                //Add new user login
                await context.UserLogins.AddAsync(userProvider);
                await context.SaveChangesAsync();

                //Get added user provider
                var addedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                addedUserProvider.Should().NotBeNull();

                var userProviderDto = IdentityDtoMock<string>.GenerateRandomUserProviders(userProvider.ProviderKey, userProvider.LoginProvider,
                    userProvider.UserId);

                await identityService.DeleteUserProvidersAsync(userProviderDto);

                //Get deleted user provider
                var deletedUserProvider = await context.UserLogins.Where(x => x.ProviderKey == userProvider.ProviderKey && x.LoginProvider == userProvider.LoginProvider).SingleOrDefaultAsync();
                deletedUserProvider.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserRoleAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                var userRoleDto = IdentityDtoMock<string>.GenerateRandomUserRole<RoleDto<string>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();

                userRole.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task DeleteUserRoleAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                var userRoleDto = IdentityDtoMock<string>.GenerateRandomUserRole<RoleDto<string>>(roleDto.Id, userDto.Id);

                await identityService.CreateUserRoleAsync(userRoleDto);

                //Get new role
                var userRole = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRole.Should().NotBeNull();

                await identityService.DeleteUserRoleAsync(userRoleDto);

                //Get deleted role
                var userRoleDeleted = await context.UserRoles.Where(x => x.RoleId == roleDto.Id && x.UserId == userDto.Id).SingleOrDefaultAsync();
                userRoleDeleted.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddUserClaimAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<string>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                newUserClaim.Should().BeEquivalentTo(userClaimDto);
            }
        }

        [Fact]
        public async Task DeleteUserClaimAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Generate random new user claim
                var userClaimDto = IdentityDtoMock<string>.GenerateRandomUserClaim(0, userDto.Id);

                await identityService.CreateUserClaimsAsync(userClaimDto);

                //Get new user claim
                var claim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                userClaimDto.ClaimId = claim.Id;

                var newUserClaim = await identityService.GetUserClaimAsync(userDto.Id.ToString(), claim.Id);

                //Assert new user claim
                newUserClaim.Should().BeEquivalentTo(userClaimDto);

                await identityService.DeleteUserClaimAsync(userClaimDto);

                //Get deleted user claim
                var deletedClaim = await context.UserClaims.Where(x => x.ClaimType == userClaimDto.ClaimType && x.ClaimValue == userClaimDto.ClaimValue).SingleOrDefaultAsync();
                deletedClaim.Should().BeNull();
            }
        }

        [Fact]
        public async Task UpdateUserAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Detached the added item
                context.Entry(user).State = EntityState.Detached;

                //Generete new user with added item id
                var userDtoForUpdate = IdentityDtoMock<string>.GenerateRandomUser(user.Id);

                //Update user
                await identityService.UpdateUserAsync(userDtoForUpdate);

                var updatedUser = await identityService.GetUserAsync(userDtoForUpdate.Id.ToString());

                //Assert updated user
                updatedUser.Should().BeEquivalentTo(userDtoForUpdate);
            }
        }

        [Fact]
        public async Task DeleteUserAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new user
                var userDto = IdentityDtoMock<string>.GenerateRandomUser();

                await identityService.CreateUserAsync(userDto);

                //Get new user
                var user = await context.Users.Where(x => x.UserName == userDto.UserName).SingleOrDefaultAsync();
                userDto.Id = user.Id;

                var newUserDto = await identityService.GetUserAsync(userDto.Id.ToString());

                //Assert new user
                newUserDto.Should().BeEquivalentTo(userDto);

                //Remove user
                await identityService.DeleteUserAsync(newUserDto.Id.ToString(), newUserDto);

                //Try Get Removed user
                var removeUser = await context.Users.Where(x => x.Id == user.Id)
                    .SingleOrDefaultAsync();

                //Assert removed user
                removeUser.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);
            }
        }

        [Fact]
        public async Task UpdateRoleAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Detached the added item
                context.Entry(role).State = EntityState.Detached;

                //Generete new role with added item id
                var roleDtoForUpdate = IdentityDtoMock<string>.GenerateRandomRole(role.Id);

                //Update role
                await identityService.UpdateRoleAsync(roleDtoForUpdate);

                var updatedRole = await identityService.GetRoleAsync(roleDtoForUpdate.Id.ToString());

                //Assert updated role
                updatedRole.Should().BeEquivalentTo(roleDtoForUpdate);
            }
        }

        [Fact]
        public async Task DeleteRoleAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Remove role
                await identityService.DeleteRoleAsync(newRoleDto);

                //Try Get Removed role
                var removeRole = await context.Roles.Where(x => x.Id == role.Id)
                    .SingleOrDefaultAsync();

                //Assert removed role
                removeRole.Should().BeNull();
            }
        }

        [Fact]
        public async Task AddRoleClaimAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                newRoleClaimDto.Should().BeEquivalentTo(roleClaimDto, options => options.Excluding(o => o.RoleName));
            }
        }

        [Fact]
        public async Task RemoveRoleClaimAsync()
        {
            using (var context = new AdminIdentityDbContext<string>(_dbContextOptions))
            {
                var identityService = GetIdentityService(context);

                //Generate random new role
                var roleDto = IdentityDtoMock<string>.GenerateRandomRole();

                await identityService.CreateRoleAsync(roleDto);

                //Get new role
                var role = await context.Roles.Where(x => x.Name == roleDto.Name).SingleOrDefaultAsync();
                roleDto.Id = role.Id;

                var newRoleDto = await identityService.GetRoleAsync(roleDto.Id.ToString());

                //Assert new role
                newRoleDto.Should().BeEquivalentTo(roleDto);

                //Generate random new role claim
                var roleClaimDto = IdentityDtoMock<string>.GenerateRandomRoleClaim(0, roleDto.Id);

                await identityService.CreateRoleClaimsAsync(roleClaimDto);

                //Get new role claim
                var roleClaim = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();
                roleClaimDto.ClaimId = roleClaim.Id;

                var newRoleClaimDto = await identityService.GetRoleClaimAsync(roleDto.Id.ToString(), roleClaimDto.ClaimId);

                //Assert new role
                newRoleClaimDto.Should().BeEquivalentTo(roleClaimDto, options => options.Excluding(o => o.RoleName));

                await identityService.DeleteRoleClaimAsync(roleClaimDto);

                var roleClaimToDelete = await context.RoleClaims.Where(x => x.ClaimType == roleClaimDto.ClaimType && x.ClaimValue == roleClaimDto.ClaimValue).SingleOrDefaultAsync();

                //Assert removed role claim
                roleClaimToDelete.Should().BeNull();
            }
        }
    }
}
