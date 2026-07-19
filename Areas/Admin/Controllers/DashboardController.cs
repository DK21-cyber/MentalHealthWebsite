using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using PW.Models.ViewModels;

namespace PW.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var model = new AdminDashboardViewModel
                {
                    TotalStudents = await _context.Students.CountAsync(),

                    TotalPsychologists = await _context.Psychologists.CountAsync(),

                    TotalAppointments = await _context.Appointments.CountAsync(),

                    HighRiskStudents = await _context.PsychologicalProfiles
                        .CountAsync(x => x.Status == ProfileStatus.Risking),

                    RecentAppointments = await _context.Appointments
                        .Include(x => x.Student)
                        .Include(x => x.Psychologist)
                        .OrderByDescending(x => x.AppointmentDate)
                        .Take(5)
                        .ToListAsync(),

                    RecentSurveyResults = await _context.SurveyResults
                        .Include(x => x.Student)
                        .OrderByDescending(x => x.SubmittedAt)
                        .Take(5)
                        .ToListAsync()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                // TODO: Log ex if you have a logger

                TempData["Error"] = "Unable to load dashboard.";

                return View(new AdminDashboardViewModel());
            }
        }
    }
}