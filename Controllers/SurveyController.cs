using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using PW.Models.ViewModels;

namespace PW.Controllers
{
    public class SurveyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SurveyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        // =========================
        // Survey List
        // =========================

        public IActionResult Surveys()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading survey list.");

                TempData["Error"] =
                    "Unable to load survey list.";

                return RedirectToAction("Index", "Home");
            }
        }

        // =========================
        // Survey Details
        // =========================

        public IActionResult Details(int id)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading survey details.");

                TempData["Error"] =
                    "Unable to load survey details.";

                return RedirectToAction(nameof(Surveys));
            }
        }

        [HttpGet]
        public IActionResult TakeSurvey()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading survey page.");

                TempData["Error"] =
                    "Unable to load survey.";

                return RedirectToAction(nameof(Surveys));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeSurvey(DASSRequestViewModel model)
        {
            try
            {
                Console.WriteLine("===== POST TakeSurvey =====");

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(x => x.Errors))
                    {
                        Console.WriteLine(error.ErrorMessage);
                    }

                    return View(model);
                }

                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var response = await client.PostAsJsonAsync(
                    "https://mentalhealthscreening.onrender.com/predict",
                    model);

                Console.WriteLine($"Status Code: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();

                    Console.WriteLine(error);

                    ModelState.AddModelError(
                        "",
                        "AI API returned an error.");

                    return View(model);
                }

                var prediction = await response.Content
                    .ReadFromJsonAsync<PredictionResponse>();

                if (prediction == null)
                {
                    ModelState.AddModelError(
                        "",
                        "Prediction result is empty.");

                    return View(model);
                }

                TempData["Success"] =
                    "Assessment completed successfully.";

                return RedirectToAction(
                    nameof(Result),
                    new
                    {
                        stress = prediction.Stress,
                        anxiety = prediction.Anxiety,
                        depression = prediction.Depression
                    });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex);

                ModelState.AddModelError(
                    "",
                    "Cannot connect to AI API. Please make sure FastAPI is running.");

                return View(model);
            }
            catch (TaskCanceledException ex)
            {
                Console.WriteLine(ex);

                ModelState.AddModelError(
                    "",
                    "The AI API request timed out.");

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                ModelState.AddModelError(
                    "",
                    "An unexpected error occurred. Please try again.");

                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Result(
            string stress,
            string anxiety,
            string depression)
        {
            try
            {
                SurveyResultViewModel model =
                    new SurveyResultViewModel
                    {
                        Stress = stress,
                        Anxiety = anxiety,
                        Depression = depression
                    };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error loading prediction result.");

                TempData["Error"] =
                    "Unable to display prediction result.";

                return RedirectToAction(nameof(TakeSurvey));
            }
        }
    }
}