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
            using var context = _dbContextFactory.CreateDbContext();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public Task<User> GetUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
