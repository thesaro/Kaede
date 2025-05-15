using Kaede.DbContexts;
using Kaede.DTOs;
using Kaede.Extensions;
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
            var customers = await context.Appointments
                .Include(a => a.Customer)
                .Where(a => a.Customer != null)
                .Select(a => a.Customer!.MapToDTO())
                .ToListAsync();
            return customers;
        }
    }
}
