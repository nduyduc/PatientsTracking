using System.Threading.Tasks;
using FIT3077_Pre1975.Models;
using FIT3077_Pre1975.Services;
using Microsoft.AspNetCore.Mvc;

namespace FIT3077_Pre1975.Controllers
{
    /// <summary>
    /// Controller class for Practitioner Views (Login and Details)
    /// </summary>
    public class PractitionerController : Controller
    {

        // GET: /Practitioner/Login
        //
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Practitioner/Login
        //
        /// <summary>
        /// Handle Login event from View
        /// </summary>
        /// <param name="model"> submitted Login Model </param>
        /// <returns> Practitioner Detail View </returns>
        [HttpPost]
        public async Task<ActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Get new Practitioner from input Id
            Practitioner newPractitioner;
            newPractitioner = await FhirService.GetPractitioner(model.Id);
            if (newPractitioner != null)
            {
                AppContext.Practitioner = newPractitioner;
                AppContext.MonitorPatients = new PatientsList();
                
                // Notify observers of Practitioner
                AppContext.Practitioner.Notify();
                return Redirect("/Practitioner/");
            }
            else
            {
                TempData["ErrorMessage"] = "Practitioner Not Found. Please try again!";
                return View("Login");
            }
        }

        // GET: /Practitioner/
        //
        public IActionResult Index()
        {
            if (AppContext.Practitioner == null)
            {
                return Redirect("/Practitioner/Login/");
            }
            return View(AppContext.Practitioner); 
        }

    }
}