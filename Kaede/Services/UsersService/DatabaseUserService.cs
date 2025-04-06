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
            User newUser = User.FromDTO(userDTO);
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();
        }

        public async Task<UserDTO?> GetUser(string username)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Username == username);
            return user?.MapToDTO();
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
            var user = await context.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }

        public async Task ChangePassword(string username, string newPassword)
        {
            // TODO: fkn implement this
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new InvalidOperationException();
            }
            else
            {
                user.PasswordHash = User.HashPassword(newPassword);
                await context.SaveChangesAsync();
            }
        }

        public async Task<bool> ValidatePassword(string username, string password)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return false;
            // we interconnect logics here which is unclean but whatever
            return User.HashPassword(password) == user.PasswordHash;
        }
    }
}
