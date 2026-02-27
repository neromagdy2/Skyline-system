using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Models;
using System.Security.Claims;

namespace Airport_Managment_SYS.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbcontext _context;

        public PaymentsController(ApplicationDbcontext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payments = _context.Payments
                .Where(p => p.ApplicationUserId == userId)
                .Include(p => p.ApplicationUser);

            return View(await payments.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payment = await _context.Payments
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (payment == null) return NotFound();

            return View(payment);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Total")] Payment payment)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            payment.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payment = await _context.Payments
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (payment == null) return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var payment = await _context.Payments
                .FirstOrDefaultAsync(m => m.Id == id && m.ApplicationUserId == userId);

            if (payment != null)
            {
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
}