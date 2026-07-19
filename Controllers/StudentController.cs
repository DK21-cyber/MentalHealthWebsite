using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PW.Models;
using PW.Models.ViewModels;

namespace PW.Controllers
{
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        //=================================================
        // Dashboard
        //=================================================

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .Include(x => x.User)
                    .Include(x => x.SurveyResults)
                    .Include(x => x.Appointments)
                    .Include(x => x.PsychologicalProfiles)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var vm = new StudentDashboardViewModel
                {
                    Student = student,
                    TotalSurveys = student.SurveyResults.Count,
                    TotalAppointments = student.Appointments.Count,
                    TotalProfiles = student.PsychologicalProfiles.Count
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load dashboard.";

                return RedirectToAction("Index", "Home");
            }
        }

        //=================================================
        // Profile
        //=================================================

        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var vm = new StudentProfileViewModel
                {
                    Id = student.Id,
                    Name = student.Name,
                    StudentCode = student.StudentCode,
                    Class = student.Class,
                    Email = student.User?.Email,
                    CreatedAt = student.User?.CreatedAt ?? DateTime.Now
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load profile.";

                return RedirectToAction(nameof(Index));
            }
        }

        //=================================================
        // Edit Profile
        //=================================================

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var vm = new EditStudentProfileViewModel
                {
                    Name = student.Name,
                    StudentCode = student.StudentCode,
                    Class = student.Class,
                    Email = student.User?.Email
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load profile.";

                return RedirectToAction(nameof(Profile));
            }
        }

        //=================================================
        // Edit Profile POST
        //=================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditStudentProfileViewModel vm)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(vm);

                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .Include(x => x.User)
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                student.Name = vm.Name;
                student.StudentCode = vm.StudentCode;
                student.Class = vm.Class;

                if (student.User != null)
                {
                    student.User.Email = vm.Email;
                    student.User.FullName = vm.Name;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Profile updated successfully.";

                return RedirectToAction(nameof(Profile));
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to update profile.";

                return View(vm);
            }
        }

        //=================================================
        // Survey History
        //=================================================

        public async Task<IActionResult> SurveyHistory()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var surveys = await _context.SurveyResults
                    .Include(x => x.Survey)
                    .Where(x => x.StudentId == student.Id)
                    .OrderByDescending(x => x.SubmittedAt)
                    .Select(x => new SurveyHistoryViewModel
                    {
                        Id = x.Id,
                        SurveyName = x.Survey != null
                                        ? x.Survey.Title
                                        : "DASS-21",

                        TotalScore = x.TotalScore,

                        RiskLevel = x.RiskLevel,

                        SubmittedAt = x.SubmittedAt
                    })
                    .ToListAsync();

                return View(surveys);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load survey history.";

                return RedirectToAction(nameof(Index));
            }
        }

        //=================================================
        // Survey Detail
        //=================================================

        public async Task<IActionResult> SurveyDetail(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var survey = await _context.SurveyResults
                    .Include(x => x.Survey)
                    .Include(x => x.Answers)
                        .ThenInclude(x => x.Question)
                    .Include(x => x.Answers)
                        .ThenInclude(x => x.QuestionOption)
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.StudentId == student.Id);

                if (survey == null)
                    return NotFound();

                var vm = new SurveyDetailViewModel
                {
                    Id = survey.Id,

                    SurveyName = survey.Survey != null
                                    ? survey.Survey.Title
                                    : "DASS-21",

                    TotalScore = survey.TotalScore,

                    RiskLevel = survey.RiskLevel,

                    ResultSummary = survey.ResultSummary,

                    SubmittedAt = survey.SubmittedAt
                };

                vm.Answers = survey.Answers
                    .Select(a => new SurveyAnswerViewModel
                    {
                        Question = a.Question != null
                                    ? a.Question.Content
                                    : "",

                        Answer = a.QuestionOption != null
                                    ? a.QuestionOption.OptionText
                                    : "",

                        Score = a.QuestionOption != null
                                    ? a.QuestionOption.Score
                                    : 0
                    })
                    .ToList();

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load survey details.";

                return RedirectToAction(nameof(SurveyHistory));
            }
        }

        //=================================================
        // Appointment History
        //=================================================

        public async Task<IActionResult> AppointmentHistory()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var appointments = await _context.Appointments
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .Where(x => x.StudentId == student.Id)
                    .OrderByDescending(x => x.AppointmentDate)
                    .Select(x => new AppointmentHistoryViewModel
                    {
                        Id = x.Id,

                        PsychologistName = x.Psychologist.Name,

                        Specialty = x.Psychologist.Specialty,

                        AppointmentDate = x.AppointmentDate,

                        Time = x.Time,

                        Status = x.Status
                    })
                    .ToListAsync();

                return View(appointments);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load appointment history.";

                return RedirectToAction(nameof(Index));
            }
        }

        //=================================================
        // Appointment Detail
        //=================================================

        public async Task<IActionResult> AppointmentDetail(int id)
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var appointment = await _context.Appointments
                    .Include(x => x.Student)
                    .Include(x => x.Psychologist)
                    .Include(x => x.ScheduleSlot)
                    .FirstOrDefaultAsync(x =>
                        x.Id == id &&
                        x.StudentId == student.Id);

                if (appointment == null)
                    return NotFound();

                var vm = new AppointmentDetailViewModel
                {
                    Id = appointment.Id,

                    StudentName = appointment.Student?.Name ?? "",

                    PsychologistName = appointment.Psychologist?.Name ?? "",

                    Specialty = appointment.Psychologist?.Specialty ?? "",

                    AppointmentDate = appointment.AppointmentDate,

                    Time = appointment.Time,

                    Description = appointment.Description,

                    Status = appointment.Status,

                    StartTime = appointment.ScheduleSlot?.StartTime ?? DateTime.MinValue,

                    EndTime = appointment.ScheduleSlot?.EndTime ?? DateTime.MinValue
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load appointment.";

                return RedirectToAction(nameof(AppointmentHistory));
            }
        }

        //=================================================
        // Psychological Profile
        //=================================================

        public async Task<IActionResult> PsychologicalProfile()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var student = await _context.Students
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (student == null)
                    return NotFound();

                var profile = await _context.PsychologicalProfiles
                    .Include(x => x.Student)
                    .Include(x => x.Psychologist)
                    .OrderByDescending(x => x.CreatedAt)
                    .FirstOrDefaultAsync(x => x.StudentId == student.Id);

                if (profile == null)
                {
                    TempData["Info"] =
                        "No psychological profile has been created yet.";

                    return View();
                }

                var vm = new PsychologicalProfileViewModel
                {
                    Id = profile.Id,

                    StudentName = profile.Student?.Name ?? "",

                    PsychologistName = profile.Psychologist?.Name ?? "",

                    Status = profile.Status,

                    Diagnosis = profile.Diagnosis,

                    Notes = profile.Notes,

                    CreatedAt = profile.CreatedAt
                };

                return View(vm);
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to load psychological profile.";

                return RedirectToAction(nameof(Index));
            }
        }

        //=================================================
        // Book Appointment
        //=================================================

        [HttpGet]
        public IActionResult BookAppointment()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                return RedirectToAction(
                    "Book",
                    "Appointment");
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to open the appointment booking page.";

                return RedirectToAction(nameof(Index));
            }
        }

        //=================================================
        // Take Survey
        //=================================================

        [HttpGet]
        public IActionResult TakeSurvey()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                return RedirectToAction(
                    "TakeSurvey",
                    "Survey");
            }
            catch (Exception)
            {
                TempData["Error"] =
                    "Unable to open the survey.";

                return RedirectToAction(nameof(Index));
            }
        }
    }
}