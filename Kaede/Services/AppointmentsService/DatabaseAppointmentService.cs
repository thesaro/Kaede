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

namespace Kaede.Services.AppointmentsService
{
    public class DatabaseAppointmentService : IAppointmentService
    {
        IDbContextFactory<KaedeDbContext> _dbContextFactory;

        public DatabaseAppointmentService(IDbContextFactory<KaedeDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<CustomerDTO>> GetAllCustomers()
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            var customers = await context.Customers
                .Select(c => c.MapToDTO())
                .ToListAsync();
            return customers;
        }

        public async Task<CustomerDTO?> GetCustomerByName(string name)
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.FullName == name);
            return customer?.MapToDTO();
        }

        public async Task CreateCustomer(CustomerDTO customerDTO)
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            await context.Customers.AddAsync(Customer.FromDTO(customerDTO));
            await context.SaveChangesAsync();
        }
    }
}
