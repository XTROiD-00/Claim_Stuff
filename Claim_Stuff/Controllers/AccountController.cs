using Claim_Stuff.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace Claim_Stuff.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            //creating an instance for the class sql_query
            sql_queries_register queries = new sql_queries_register();
            //call the method create_table to create the table
            queries.create_table();
            return View();
        }

        [HttpPost]
        public IActionResult Register(Register model)
        {
            if (ModelState.IsValid)
            {
                sql_queries_register get_values = new sql_queries_register();
                get_values.store_user(model.name, model.email, model.username, model.password, model.role);
                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Login");
            }
            return View(model);
        }
        //Get
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            sql_queries_register search_values = new sql_queries_register();
            var user = search_values.Login_user(username, password, role);

            if (user)
            {
                ViewBag.Username = username;
                ViewBag.Role = role;
                HttpContext.Session.SetString("role", role);
                if (role == "Lecturer")
                    return RedirectToAction("Lecturer");
                else if (role == "PC")
                    return RedirectToAction("ProgramCoordinator");
                else if (role == "PM")
                    return RedirectToAction("ProgramManager");
                else if (role == "HR")
                    return RedirectToAction("HR");
                else
                    return RedirectToAction("Login");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // Lecturer Dashboard
        public IActionResult Lecturer()
        {
            return View();
        }

        // Program Coordinator Dashboard
        public IActionResult ProgramCoordinator()
        {
            return View();
        }

        // Program Manager Dashboard
        public IActionResult ProgramManager()
        {
            return View();
        }

        public IActionResult HR()
        {
            return View();
        }
        // Logout
        public IActionResult Logout()
        {
            return View();
        }


    }
}
