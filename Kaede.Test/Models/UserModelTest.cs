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
using Kaede.DTOs;
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

            UserDTO u1DTO = new UserDTO
            {
                Username = "uname1",
                Password = "XXXXXXXX",
                Role = UserRole.Admin
            };

            UserDTO u2DTO = new UserDTO
            {
                Username = "uname2",
                Password = "XXXXXXXX",
                Role = UserRole.Barber
            };

            User.TryFromDTO(u1DTO, out User? u1);
            User.TryFromDTO(u2DTO, out User? u2);

            Assert.NotNull(u1);
            Assert.NotNull(u2);

            context.Users.Add(u1);
            context.Users.Add(u2);

            context.SaveChanges();
        }


        [Fact]
        public void CreateUser_WhenUsernameBadLength_ShouldFail()
        {
            UserDTO shortUserDTO = new UserDTO
            {
                Username = "u",
                Password = "XXXXXXXX",
                Role = UserRole.Admin
            };

            bool res = User.TryFromDTO(shortUserDTO, out User? shortUser);
            Assert.False(res);

            UserDTO longUserDTO = new UserDTO
            {
                Username = new string('u', 60),
                Password = "XXXXXXXX",
                Role = UserRole.Barber
            };

            res = User.TryFromDTO(longUserDTO, out User? longUser);
            Assert.False(res);
        }


        [Fact]
        public void CreateUser_WhenDuplicateUsernameProvided_ShouldFail()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            UserDTO u1DTO = new UserDTO
            {
                Username = "same_uname",
                Password = "XXXXXXXX",
                Role = UserRole.Admin
            };

            UserDTO u2DTO = new UserDTO
            {
                Username = "same_uname",
                Password = "XXXXXXXX",
                Role = UserRole.Barber
            };

            User.TryFromDTO(u1DTO, out User? u1);
            User.TryFromDTO(u2DTO, out User? u2);

            Assert.NotNull(u1);
            Assert.NotNull(u2);

            context.Users.Add(u1);
            context.Users.Add(u2);

            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
        }


    }
}
