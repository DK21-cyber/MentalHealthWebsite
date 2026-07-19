using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using PW.Models.ViewModels;
using System.Security.Claims;

namespace PW.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        //=========================================
        // Load Dropdown Lists
        //=========================================

        private async Task LoadDropdowns(CreateAppointmentViewModel vm)
        {
            vm.Psychologists = await _context.Psychologists
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Name} ({x.Specialty})"
                })
                .ToListAsync();

            vm.ScheduleSlots = await _context.ScheduleSlots
                .Include(x => x.Psychologist)
                .Where(x => x.Status == SlotStatus.Available)
                .OrderBy(x => x.StartTime)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Psychologist.Name} | {x.StartTime:dd/MM/yyyy HH:mm} - {x.EndTime:HH:mm}"
                })
                .ToListAsync();
        }

        //
        //=========================================
        // BOOK (GET)
        //=========================================
        //

        [HttpGet]
        public async Task<IActionResult> Book()
        {
            try
            {
                var vm = new CreateAppointmentViewModel();

                await LoadDropdowns(vm);

                return View(vm);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading appointment page.");

                TempData["Error"] =
                    "Unable to load the appointment booking page.";

                return RedirectToAction("Index", "Home");
            }
        }

        //
        //=========================================
        // BOOK (POST)
        //=========================================
        //

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(CreateAppointmentViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadDropdowns(vm);
                    return View(vm);
                }

                //-------------------------------------------------------
                // Find selected schedule slot
                //-------------------------------------------------------

                var slot = await _context.ScheduleSlots
                    .Include(x => x.Psychologist)
                    .FirstOrDefaultAsync(x => x.Id == vm.ScheduleSlotId);

                if (slot == null)
                {
                    ModelState.AddModelError("", "The selected schedule slot does not exist.");

                    await LoadDropdowns(vm);

                    return View(vm);
                }

                //-------------------------------------------------------
                // Check slot status
                //-------------------------------------------------------

                if (slot.Status != SlotStatus.Available)
                {
                    ModelState.AddModelError("", "This schedule slot is no longer available.");

                    await LoadDropdowns(vm);

                    return View(vm);
                }

                //-------------------------------------------------------
                // TODO:
                // Replace by ASP.NET Identity
                //-------------------------------------------------------

                int studentId = 1;

                /*
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                {
                    TempData["Error"] = "Student account not found.";
                    return RedirectToAction("Login", "Account");
                }

                studentId = student.Id;
                */

                //-------------------------------------------------------
                // Prevent duplicate booking
                //-------------------------------------------------------

                bool existed = await _context.Appointments.AnyAsync(x =>
                    x.StudentId == studentId &&
                    x.ScheduleSlotId == vm.ScheduleSlotId &&
                    x.Status != AppointmentStatus.Cancelled);

                if (existed)
                {
                    ModelState.AddModelError("", "You have already booked this schedule.");

                    await LoadDropdowns(vm);

                    return View(vm);
                }

                //-------------------------------------------------------
                // Create appointment
                //-------------------------------------------------------

                Appointment appointment = new Appointment
                {
                    StudentId = studentId,

                    PsychologistId = slot.PsychologistId,

                    ScheduleSlotId = slot.Id,

                    AppointmentDate = slot.StartTime.Date,

                    Time = slot.StartTime,

                    Description = vm.Description,

                    Status = AppointmentStatus.Pending
                };

                //-------------------------------------------------------
                // Update slot
                //-------------------------------------------------------

                slot.Status = SlotStatus.Booked;

                slot.Notes = "Waiting for psychologist approval.";

                //-------------------------------------------------------
                // Save
                //-------------------------------------------------------

                _context.Appointments.Add(appointment);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Your appointment has been booked successfully.";

                return RedirectToAction(nameof(Schedule));
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "A database error occurred while booking the appointment.");

                await LoadDropdowns(vm);

                return View(vm);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error booking appointment.");

                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");

                await LoadDropdowns(vm);

                return View(vm);
            }
        }

        //=========================================
        // MY APPOINTMENTS
        //=========================================

        [HttpGet]
        public async Task<IActionResult> Schedule()
        {
            try
            {
                //-------------------------------------------------------
                // TEMP
                // Replace by Identity later
                //-------------------------------------------------------

                int studentId = 1;

                /*
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                {
                    TempData["Error"] = "Student account not found.";
                    return RedirectToAction("Login", "Account");
                }

                studentId = student.Id;
                */

                var appointments = await _context.Appointments
                    .Include(x => x.Student)
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .Where(x => x.StudentId == studentId)
                    .OrderByDescending(x => x.AppointmentDate)
                    .ThenByDescending(x => x.Time)
                    .ToListAsync();

                return View(appointments);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading appointments.");

                TempData["Error"] =
                    "Unable to load your appointments.";

                return RedirectToAction("Index", "Home");
            }
        }

        //=========================================
        // DETAILS
        //=========================================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.Student)
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                {
                    TempData["Error"] = "Appointment not found.";
                    return RedirectToAction(nameof(Schedule));
                }

                /*
                // Uncomment when using Identity

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return Unauthorized();

                if (appointment.StudentId != student.Id)
                    return Forbid();
                */

                return View(appointment);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error loading appointment details.");

                TempData["Error"] =
                    "Unable to load appointment details.";

                return RedirectToAction(nameof(Schedule));
            }
        }

        //=========================================
        // APPROVE
        //=========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                {
                    TempData["Error"] = "Appointment not found.";
                    return RedirectToAction(nameof(Schedule));
                }

                if (appointment.Status != AppointmentStatus.Pending)
                {
                    TempData["Error"] =
                        "Only pending appointments can be approved.";

                    return RedirectToAction(nameof(Schedule));
                }

                appointment.Status = AppointmentStatus.Approved;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Booked;
                    appointment.ScheduleSlot.Notes = "Approved by psychologist.";
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment approved successfully.";

                return RedirectToAction(nameof(Schedule));
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,"Approve Error");

                TempData["Error"] =
                    "Unable to approve appointment.";

                return RedirectToAction(nameof(Schedule));
            }
        }

        //=========================================
        // COMPLETE
        //=========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                {
                    TempData["Error"] = "Appointment not found.";
                    return RedirectToAction(nameof(Schedule));
                }

                if (appointment.Status != AppointmentStatus.Approved)
                {
                    TempData["Error"] =
                        "Only approved appointments can be completed.";

                    return RedirectToAction(nameof(Schedule));
                }

                appointment.Status = AppointmentStatus.Completed;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Completed;
                    appointment.ScheduleSlot.Notes = "Consultation completed.";
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment completed successfully.";

                return RedirectToAction(nameof(Schedule));
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,"Complete Error");

                TempData["Error"] =
                    "Unable to complete appointment.";

                return RedirectToAction(nameof(Schedule));
            }
        }

        //=========================================
        // CANCEL
        //=========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (appointment == null)
                {
                    TempData["Error"] = "Appointment not found.";
                    return RedirectToAction(nameof(Schedule));
                }

                if (appointment.Status == AppointmentStatus.Completed)
                {
                    TempData["Error"] =
                        "Completed appointments cannot be cancelled.";

                    return RedirectToAction(nameof(Schedule));
                }

                appointment.Status = AppointmentStatus.Cancelled;

                if (appointment.ScheduleSlot != null)
                {
                    appointment.ScheduleSlot.Status = SlotStatus.Available;
                    appointment.ScheduleSlot.Notes = null;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Appointment cancelled successfully.";

                return RedirectToAction(nameof(Schedule));
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex,"Cancel Error");

                TempData["Error"] =
                    "Unable to cancel appointment.";

                return RedirectToAction(nameof(Schedule));
            }
        }
    }
}