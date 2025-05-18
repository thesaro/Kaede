using Kaede.DbContexts;
using Kaede.DTOs;
using Kaede.Extensions;
using Kaede.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
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

        public async Task CreateAppointment(AppointmentDTO appointmentDTO)
        {
            var context = await _dbContextFactory.CreateDbContextAsync();

            var customer = await context.Customers.FirstOrDefaultAsync
                (c => c.FullName == appointmentDTO.CustomerDTO.FullName);

            if (User.TryEncodeUsername(appointmentDTO.BarberDTO.Username, out string? uHash))
            {
                var barber = await context.Users.FirstOrDefaultAsync(u => u.UsernameHash == uHash);
                var shopItem = await context.ShopItems.FirstOrDefaultAsync(i => i.Name == appointmentDTO.ShopItemDTO.Name);

                if (barber == null || shopItem == null || customer == null)
                {
                    throw new InvalidDTOException("AppointmentDTO does not have valid foreign models defined.");
                }
                else
                {
                    var appointment = new Appointment()
                    {
                        Customer = customer,
                        Barber = barber,
                        ShopItem = shopItem,
                        StartDate = appointmentDTO.StartDate,
                        EndDate = appointmentDTO.EndDate
                    };

                    await context.AddAsync(appointment);
                    await context.SaveChangesAsync();
                  
                }
            }
            else
                Log.Logger.Error("Unable to encode DTO username hash.");
        }

        public async Task<List<AppointmentDTO>> GetAllAppointments()
        {
            var context = await _dbContextFactory.CreateDbContextAsync();
            List<AppointmentDTO> appointments = await context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Barber)
                .Include(a => a.ShopItem)
                .Where(a => a != null)
                .Select(a => a!.MapToDTO()) 
                .ToListAsync();
            return appointments;
        }
    }
}
