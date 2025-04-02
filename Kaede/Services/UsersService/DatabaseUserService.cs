using Kaede.DbContexts;
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

        public async Task CreateUser(User user)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<User?> GetUser(string username)
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Users.SingleOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> HasAdmin()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return await context.Users.SingleOrDefaultAsync(u => u.Role == UserRole.Admin) != null;
        }

        public async Task<List<User>> GetBarbers()
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            return context.Users.Where(u => u.Role == UserRole.Barber).ToList();
        }
    }
}
