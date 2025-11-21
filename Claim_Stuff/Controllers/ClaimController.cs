using Claim_Stuff.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Claim_Stuff.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly claim_query _query;

        public ClaimsController(claim_query query)
        {
            _query = query;
        }

        [HttpGet]
        public IActionResult Create()
        {
            _query.create_table();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Claims claim)
        {
            if (!ModelState.IsValid)
                return View(claim);

            if (claim.DocumentFile == null || claim.DocumentFile.Length == 0)
            {
                ModelState.AddModelError("DocumentFile", "Upload required");
                return View(claim);
            }

            var ext = Path.GetExtension(claim.DocumentFile.FileName).ToLower();
            var allowed = new[] { ".pdf", ".txt", ".docx", ".xlsx" };

            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("DocumentFile", "Invalid file type");
                return View(claim);
            }

            string uniqueFile = Guid.NewGuid() + "_" + claim.DocumentFile.FileName;
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, uniqueFile);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                claim.DocumentFile.CopyTo(stream);
            }

            claim.documentName = uniqueFile;
            claim.totalAmount = claim.hours * claim.rate;
            claim.status = "Pending";

            _query.store_claim(claim.month, claim.hours, claim.rate, claim.totalAmount, claim.documentName, claim.status);

            return RedirectToAction("MyClaims");
        }

        public IActionResult MyClaims()
        {
            ViewData["role"] = HttpContext.Session.GetString("role");
            List<Claims> claims = _query.get_all_claims();
            return View(claims);
        }
    }
}
