using Kaede.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaede.Services.AppointmentsService
{
    public interface IAppointmentService
    {
        Task<List<CustomerDTO>> GetAllCustomers();
        Task<CustomerDTO?> GetCustomerByName(string name);
        Task CreateCustomer(CustomerDTO customerDTO);
        Task CreateAppointment(AppointmentDTO appointmentDTO);

        Task<List<AppointmentDTO>> GetAllAppointments();

    }
}
