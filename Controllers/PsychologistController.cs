using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using PW.Models.ViewModels;

namespace PW.Controllers
{
    public class PsychologistController : Controller
    {
        private readonly AppDbContext _context;

        public PsychologistController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================
        // Dashboard
        // ==========================
        public async Task<IActionResult> Dashboard()
        {
            var vm = new PsychologistDashboardViewModel();

            vm.TotalStudents =
                await _context.Students.CountAsync();

            vm.TodayAppointments =
                await _context.Appointments
                    .CountAsync(x =>
                        x.AppointmentDate.Date == DateTime.Today);

            vm.PendingAppointments =
                await _context.Appointments
                    .CountAsync(x =>
                        x.Status == AppointmentStatus.Pending);

            vm.HighRiskStudents = await _context.PsychologicalProfiles
                .CountAsync(x => x.Status == ProfileStatus.Risking);

            vm.HighRiskCases = await _context.PsychologicalProfiles
                .Include(x => x.Student)
                .Where(x => x.Status == ProfileStatus.Risking)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();

            vm.RecentAppointments =
                await _context.Appointments
                    .Include(x => x.Student)
                    .OrderByDescending(x => x.AppointmentDate)
                    .Take(5)
                    .ToListAsync();

            return View(vm);
        }

        // ==========================
        // Profile
        // ==========================
        public IActionResult CreateProfile()
        {
            var vm = new CreatePsychologicalProfileViewModel();

            vm.Students = _context.Students
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfile(CreatePsychologicalProfileViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Students = _context.Students
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    })
                    .ToList();

                return View(vm);
            }

            PsychologicalProfile profile = new()
            {
                StudentId = vm.StudentId,

                PsychologistId = 1, // TODO: Logged-in psychologist

                Status = vm.Status,

                Diagnosis = vm.Diagnosis,

                Notes = vm.Notes,

                CreatedAt = DateTime.Now
            };

            _context.PsychologicalProfiles.Add(profile);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                "Psychological profile created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // ==========================
        // Schedule
        // ==========================
        public async Task<IActionResult> Schedule(int psychologistId = 1)
        {
            var slots = await _context.ScheduleSlots
                .Include(x => x.Psychologist)
                .Include(x => x.Appointment)
                .Where(x => x.PsychologistId == psychologistId)
                .OrderBy(x => x.StartTime)
                .ToListAsync();

            return View(slots);
        }

        // ==========================
        // Appointment List
        // ==========================
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.Appointments
                .Include(x => x.Student)
                .Include(x => x.Psychologist)
                .Include(x => x.ScheduleSlot)
                .OrderByDescending(x => x.AppointmentDate)
                .ToListAsync();

            return View(appointments);
        }

        // ==========================
        // Appointment Details
        // ==========================

        public async Task<IActionResult> AppointmentDetails(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.Student)
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                    return NotFound();

                return View(appointment);
            }
            catch (Exception)
            {
                TempData["Error"] = "Unable to load appointment details.";

                return RedirectToAction(nameof(Appointments));
            }
        }

        // ==========================
        // Approve Appointment
        // ==========================
        public async Task<IActionResult> Approve(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                    return NotFound();

                appointment.Status = AppointmentStatus.Approved;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Booked;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment approved successfully.";
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Approve appointment failed.");

                TempData["Error"] =
                    "Unable to approve this appointment.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // ==========================
        // Complete Appointment
        // ==========================
        public async Task<IActionResult> Complete(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                    return NotFound();

                appointment.Status = AppointmentStatus.Completed;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Completed;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment marked as completed.";
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Complete appointment failed.");

                TempData["Error"] =
                    "Unable to complete this appointment.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // ==========================
        // Cancel Appointment
        // ==========================
        public async Task<IActionResult> Cancel(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                    return NotFound();

                appointment.Status = AppointmentStatus.Cancelled;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Available;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment cancelled successfully.";
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Cancel appointment failed.");

                TempData["Error"] =
                    "Unable to cancel this appointment.";
            }

            return RedirectToAction(nameof(Appointments));
        }

        // ==========================
        // Student Psychological Profile
        // ==========================
        public async Task<IActionResult> StudentProfile(int id)
        {
            if (id <= 0)
                return BadRequest();

            try
            {
                var student = await _context.Students
                    .Include(x => x.User)
                    .Include(x => x.PsychologicalProfiles)
                        .ThenInclude(x => x.Psychologist)
                    .Include(x => x.SurveyResults)
                        .ThenInclude(x => x.Survey)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (student == null)
                    return NotFound();

                return View(student);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading student profile.");

                TempData["Error"] =
                    "Unable to load the student profile.";

                return RedirectToAction("Index", "Home");
            }
        }
    }
}