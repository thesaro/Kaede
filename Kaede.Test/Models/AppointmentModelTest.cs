using Kaede.DbContexts;
using Kaede.DTOs;
using Kaede.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kaede.Test.Models
{
    public class AppointmentModelTest
    {
        [Fact]
        public void CreateAppointment_WhenCorrect_ShouldPass()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteInMemOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            User b1 = new User
            {
                UsernameHash = "someuserwithhashidk",
                PasswordHash = "XXXXXXXXXXXXXXXXXXX",
                Role = UserRole.Barber
            };
            context.Users.Add(b1);
            context.SaveChanges();

            Customer c1 = new Customer { FullName = "c1", PhoneNumber = "1234567890" };
            context.Customers.Add(c1);
            context.SaveChanges();

            ShopItem i1 = new ShopItem
            {
                Name = "I1",
                Description = null,
                Price = 8M,
                Duration = TimeSpan.FromMinutes(30)
            };
            context.ShopItems.Add(i1);
            context.SaveChanges();

            Appointment ap = new Appointment
            {
                Customer = c1,
                Barber = b1,
                ShopItem = i1,
                StartDate = DateTime.Now - TimeSpan.FromMinutes(5),
                EndDate = DateTime.Now,
            };
            context.Appointments.Add(ap);
            context.SaveChanges();
        }
        [Fact]
        public void CreateAppointment_PastDate_ShouldFail()
        {
            using var context = new KaedeDbContext(Shared.CreateSqliteInMemOptions<KaedeDbContext>());
            context.Database.EnsureCreated();

            // First create required entities (Barber, Customer, ShopItem)
            User b1 = new User
            {
                UsernameHash = "someuserwithhash",
                PasswordHash = "XXXXXXXXXXXXXXXXXXX",
                Role = UserRole.Barber
            };
            context.Users.Add(b1);

            Customer c1 = new Customer { FullName = "testcustomer", PhoneNumber = "1234567890" };
            context.Customers.Add(c1);

            ShopItem i1 = new ShopItem
            {
                Name = "TestItem",
                Description = null,
                Price = 8M,
                Duration = TimeSpan.FromMinutes(30)
            };
            context.ShopItems.Add(i1);

            context.SaveChanges();

            // Now try to create appointment with past date
            Appointment ap = new Appointment
            {
                Customer = c1,
                Barber = b1,
                ShopItem = i1,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(-1).AddMinutes(30)
            };

            // This should throw validation exception
            var validationContext = new ValidationContext(ap);
            Assert.Throws<ValidationException>(() => Validator.ValidateObject(ap, validationContext, true));
        }
    }
}
