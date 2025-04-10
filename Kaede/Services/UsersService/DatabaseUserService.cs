using Kaede.DbContexts;
using Kaede.DTOs;
using Kaede.Extensions;
using Kaede.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services.UsersService
{
    public class DatabaseUserService : IUserService
    {
        IDbContextFactory<KaedeDbContext> _dbContextFactory;

        public DatabaseUserService(IDbContextFactory<KaedeDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task CreateUser(UserDTO userDTO)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            if (User.TryFromDTO(userDTO, out User? newUser))
            {
                await context.Users.AddAsync(newUser!);
                await context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidDTOException("Invalid UserDTO");
            }
        }

        // needs to take the DbContext parameter as well so mutating the returned model
        // results in changes with respect to the calling/saving context
        private async Task<User?> GetUserModel(string username, KaedeDbContext context)
        {
            if (User.TryEncodeUsername(username, out string? uHash))
            {
                User? user = await context.Users.SingleOrDefaultAsync(u => u.UsernameHash == uHash);
                return user;
            }
            else 
                return null;
        }

        public async Task<UserDTO?> GetUser(string username)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return (await GetUserModel(username, context))?.MapToDTO();
        }

        public async Task<bool> HasAdmin()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Users.SingleOrDefaultAsync(u => u.Role == UserRole.Admin) != null;
        }

        public async Task<List<UserDTO>> GetBarbers()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return context.Users.Where(u => u.Role == UserRole.Barber)
                .Select(u => u.MapToDTO()).ToList();
        }

        public async Task RemoveUser(string username)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await GetUserModel(username, context);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task ChangePassword(string username, string newPassword)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await GetUserModel(username, context);
            if (user == null)
            {
                // TODO: do some actual error handlings
                throw new InvalidOperationException();
            }
            else
            {
                // this can be a SetPassword() method on user instead but this
                // is clearly a oneshot operation in the whole app so whatever
                user.PasswordHash = User.HashPassword(newPassword);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidatePassword(string username, string password)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await GetUserModel(username, context);
            if (user == null) return false;
            // we interconnect logics here which is unclean
            return User.HashPassword(password) == user.PasswordHash;
        }
    }
}
