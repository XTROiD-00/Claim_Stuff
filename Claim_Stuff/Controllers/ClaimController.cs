using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using Claim_Stuff.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Claim_Stuff.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly claim_query _query;

        // Constructor Injection for claim_query
        public ClaimsController(claim_query query)
        {
            _query = query;
        }

        // GET: /Claims/Create
        [HttpGet]
        public IActionResult Create()
        {
            _query.create_table(); // Optional: Consider moving this to startup logic
            return View();
        }

        // POST: /Claims/Create
        [HttpPost]
        public IActionResult Create(Claims claim)
        {
            if (ModelState.IsValid)
            {
                // Check if a file was uploaded
                if (claim.DocumentFile != null && claim.DocumentFile.Length > 0)
                {
                    // Validate file extension
                    var extension = Path.GetExtension(claim.DocumentFile.FileName).ToLower();
                    var allowedExtensions = new[] { ".pdf", ".txt", ".docx", ".xlsx" };

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("DocumentFile", "Only PDF, TXT, DOCX, or XLSX files are allowed.");
                        return View(claim);
                    }

                    // Generate unique file name
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(claim.DocumentFile.FileName);

                    // Create the "Uploads" folder if it doesn’t exist
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Full file path
                    string filePath = Path.Combine(uploadPath, uniqueFileName);

                    // Save the file to server
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        claim.DocumentFile.CopyTo(stream);
                    }

                    // Save file name and other claim details in the database
                    claim.documentName = uniqueFileName;
                    claim.totalAmount = claim.hours * claim.rate;
                    claim.status = "Pending";

                    try
                    {
                        // Store claim in DB
                        _query.store_claim(claim.month, claim.hours, claim.rate, claim.totalAmount, claim.documentName, claim.status);
                        return RedirectToAction("MyClaims");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while saving your claim. Please try again.");
                        // Log error (you could use a logging framework here, such as Serilog or NLog)
                        Console.WriteLine(ex.Message);
                    }
                }
                else
                {
                    ModelState.AddModelError("DocumentFile", "Please upload a supporting document.");
                }
            }

            return View(claim);
        }

        // GET: /Claims/MyClaims
        [HttpGet]
        public IActionResult MyClaims()
        {
            Console.WriteLine("Gets role: " + HttpContext.Session.GetString("role"));
            ViewData["role"] = HttpContext.Session.GetString("role");

            // Fetch claims from the database
            List<Claims> claims = _query.get_all_claims();
            return View(claims);
        }

        // GET: /Claims/Details
        [HttpGet]
        public IActionResult Details(int id)
        {
            // Fetch a specific claim by ID
            var claim = _query.get_all_claims().FirstOrDefault(c => c.ClaimId == id);

            if (claim == null)
                return NotFound();

            return View(claim);
        }

        // POST: PreApprove (to update claim status)
        [HttpPost]
        public IActionResult PreApprove(int claimId, string status)
        {
            try
            {
                bool success = _query.update_claim_status(claimId, status);

                // Store success or failure message in TempData
                TempData["Message"] = success
                    ? "Claim status updated successfully!"
                    : "Error updating claim status.";

                return RedirectToAction("MyClaims");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error updating claim status: {ex.Message}";
                return RedirectToAction("MyClaims");
            }
        }

        // POST: Approve (final approval of the claim)
        [HttpPost]
        public IActionResult Approve(int claimId, string status)
        {
            try
            {
                bool success = _query.UpdateClaimStatusForFinalApproval(claimId, status);

                // Store success or failure message in TempData
                TempData["Message"] = success
                    ? "Claim final approval status updated successfully!"
                    : "Error updating final approval status.";

                return RedirectToAction("MyClaims");
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"Error updating final approval status: {ex.Message}";
                return RedirectToAction("MyClaims");
            }
        }

        // Report Page (can be expanded with logic)
        public IActionResult Report()
        {
            return View();
        }
    }
}
