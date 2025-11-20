using Claim_Stuff.Models;
using Microsoft.AspNetCore.Mvc;

namespace Claim_Stuff.Controllers
{
    public class AccountController : Controller
    {
        // ---------------- REGISTER ----------------

        [HttpGet]
        public IActionResult Register()
        {
            // Create table on first load
            sql_queries_register queries = new sql_queries_register();
            queries.create_table();

            return View();
        }

        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (!ModelState.IsValid)
                return View(model);

            sql_queries_register db = new sql_queries_register();
            db.store_user(model.name, model.email, model.username, model.password, model.role);

            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("Login");
        }

        // ---------------- LOGIN ----------------

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            sql_queries_register db = new sql_queries_register();
            bool isValidUser = db.Login_user(username, password, role);

            if (!isValidUser)
            {
                ViewBag.Error = "Invalid username, password, or role.";
                return View();
            }

            // Store session info
            HttpContext.Session.SetString("Role", role);

            // Redirect based on role
            return role switch
            {
                "Lecturer" => RedirectToAction("Lecturer"),
                "PC" => RedirectToAction("ProgramCoordinator"),
                "PM" => RedirectToAction("ProgramManager"),
                "HR" => RedirectToAction("HR"),
                _ => RedirectToAction("Login")
            };
        }

        // ---------------- DASHBOARDS ----------------

        public IActionResult Lecturer()
        {
            return View();
        }

        public IActionResult ProgramCoordinator()
        {
            return View();
        }

        public IActionResult ProgramManager()
        {
            return View();
        }

        public IActionResult HR()
        {
            return View();
        }

        // ---------------- LOGOUT ----------------

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
