using Microsoft.EntityFrameworkCore;
using StudioMgn.Data;
using StudioMgn.Models;

namespace StudioMgn.Services
{
    public class AppointmentsService : Services.Service<Appointment, AppointmentsService>
    {
        public AppointmentsService(ILogger<AppointmentsService> logger, IDbContextFactory<ApplicationDbContext> dbContextFactory) : base(logger, dbContextFactory)
        {
        }
    }
}
