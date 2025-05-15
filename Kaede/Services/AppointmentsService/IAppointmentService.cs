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
        Task CreateAppointment(AppointmentDTO appointmentDTO);
    }
}
