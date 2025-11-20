using Microsoft.AspNetCore.Mvc;
using Claim_Stuff.Models;
using System.Text;

namespace Claim_Stuff.Controllers
{
    public class HRController : Controller
    {
        // HR Dashboard
        public IActionResult HR()
        {
            return View();
        }

        // Show ONLY Approved claims
        public IActionResult ApprovedClaims()
        {
            claim_query q = new claim_query();
            var claims = q.get_all_claims()
                          .Where(c => c.status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                          .ToList();

            return View(claims);
        }

        // Download Approved Claims as CSV (Excel opens CSV naturally)
        public FileResult DownloadApprovedClaimsCSV()
        {
            claim_query q = new claim_query();
            var claims = q.get_all_claims()
                          .Where(c => c.status.Equals("Approved", StringComparison.OrdinalIgnoreCase))
                          .ToList();

            var sb = new StringBuilder();

            // CSV HEADER
            sb.AppendLine("ClaimId,Month,Hours,Rate,TotalAmount,DocumentName,Status");

            // CSV ROWS
            foreach (var c in claims)
            {
                string docName = c.documentName?.Replace("\"", "\"\"") ?? "";

                sb.AppendLine(
                    $"{c.ClaimId}," +
                    $"{c.month}," +
                    $"{c.hours}," +
                    $"{c.rate}," +
                    $"{c.totalAmount}," +
                    $"\"{docName}\"," +
                    $"{c.status}"
                );
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "ApprovedClaimsReport.csv");
        }
    }
}
