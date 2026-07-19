using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PW.Models;

namespace PW.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        //----------------------------------------------------
        // Student List
        //----------------------------------------------------

        public async Task<IActionResult> Index()
        {
            try
            {
                var students = await _context.Students
                    .Include(x => x.User)
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                return View(students);
            }
            catch
            {
                TempData["Error"] = "Unable to load student list.";
                return View(new List<Student>());
            }
        }

        //----------------------------------------------------
        // Details
        //----------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var student = await _context.Students
                    .Include(x => x.User)
                    .Include(x => x.SurveyResults)
                    .Include(x => x.Appointments)
                    .Include(x => x.PsychologicalProfiles)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (student == null)
                    return NotFound();

                return View(student);
            }
            catch
            {
                TempData["Error"] = "Unable to load student.";
                return RedirectToAction(nameof(Index));
            }
        }

        //----------------------------------------------------
        // Edit
        //----------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var student = await _context.Students
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (student == null)
                    return NotFound();

                return View(student);
            }
            catch
            {
                TempData["Error"] = "Unable to load student.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(model);

                var student = await _context.Students
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (student == null)
                    return NotFound();

                student.Name = model.Name;
                student.StudentCode = model.StudentCode;
                student.Class = model.Class;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Student updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to update student.";
                return View(model);
            }
        }

        //----------------------------------------------------
        // Delete
        //----------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var student = await _context.Students.FindAsync(id);

                if (student == null)
                    return NotFound();

                _context.Students.Remove(student);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Student deleted successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Unable to delete student.";
                return RedirectToAction(nameof(Index));
            }
        }

        //----------------------------------------------------
        // Survey History
        //----------------------------------------------------

        public async Task<IActionResult> SurveyHistory(int id)
        {
            try
            {
                var history = await _context.SurveyResults
                    .Include(x => x.Survey)
                    .Where(x => x.StudentId == id)
                    .OrderByDescending(x => x.SubmittedAt)
                    .ToListAsync();

                ViewBag.StudentId = id;

                return View(history);
            }
            catch
            {
                TempData["Error"] = "Unable to load survey history.";
                return RedirectToAction(nameof(Index));
            }
        }

        //----------------------------------------------------
        // Appointment History
        //----------------------------------------------------

        public async Task<IActionResult> AppointmentHistory(int id)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .Where(x => x.StudentId == id)
                    .OrderByDescending(x => x.AppointmentDate)
                    .ToListAsync();

                ViewBag.StudentId = id;

                return View(appointments);
            }
            catch
            {
                TempData["Error"] = "Unable to load appointment history.";
                return RedirectToAction(nameof(Index));
            }
        }

        //----------------------------------------------------
        // Psychological Profile
        //----------------------------------------------------

        public async Task<IActionResult> PsychologicalProfile(int id)
        {
            try
            {
                var profiles = await _context.PsychologicalProfiles
                    .Include(x => x.Psychologist)
                    .Where(x => x.StudentId == id)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                ViewBag.StudentId = id;

                return View(profiles);
            }
            catch
            {
                TempData["Error"] = "Unable to load psychological profile.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}