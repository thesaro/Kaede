using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kaede;
using Kaede.DbContexts;
using Kaede.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaede.Test.Models
{
    public class UserModelTest
    {
        [Fact]
        public void CreateUser_WhenUsernameIsCorrect_ShouldPass()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            User u1 = new User
            {
                Username = "uname1",
                PasswordHash = "XXXXXXXX",
                Role = UserRole.Admin
            };

            User u2 = new User
            {
                Username = "uname2",
                PasswordHash = "XXXXXXXX",
                Role = UserRole.Barber
            };

            context.Users.Add(u1);
            context.Users.Add(u2);

            context.SaveChanges();
        }


        [Fact]
        public void CreateUser_WhenUsernameBadLength_ShouldFail()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            User shortUser = new User
            {
                Username = "u",
                PasswordHash = "XXXXXXXX",
                Role = UserRole.Admin
            };

            context.Users.Add(shortUser);

            Assert.Throws<ValidationException>(() => context.SaveChanges());

            User longUser = new User
            {
                Username = new string('u', 60),
                PasswordHash = "XXXXXXXX",
                Role = UserRole.Barber
            };

            context.Users.Add(longUser);

            Assert.Throws<ValidationException>(() => context.SaveChanges());
        }


        [Fact]
        public void CreateUser_WhenDuplicateUsernameProvided_ShouldFail()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            User u1 = new User
            {
                Username = "same_uname",
                PasswordHash = "X1",
                Role = UserRole.Admin
            };

            User u2 = new User
            {
                Username = "same_uname",
                PasswordHash = "X2",
                Role = UserRole.Barber
            };

            context.Users.Add(u1);
            context.Users.Add(u2);

            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
        }


    }
}
