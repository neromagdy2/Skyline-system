using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Airport_Managment_SYS.DataAccess;
using Airport_Managment_SYS.Models;
using Airport_Managment_SYS.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Airport_Managment_SYS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class PaymentsController : Controller
    {
        private readonly IRepository<Payment> _paymentRepository;

        public PaymentsController(IRepository<Payment> paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var payments = await _paymentRepository.GetAsync(
                p => !p.IsDeleted,
                new System.Linq.Expressions.Expression<System.Func<Payment, object>>[] { p => p.ApplicationUser },
                trackd: true
            );

            return View(payments);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payment = await _paymentRepository.GetOneAsync(
                m => m.Id == id,
                new System.Linq.Expressions.Expression<System.Func<Payment, object>>[] { p => p.ApplicationUser },
                trackd: true
            );

            if (payment == null) return NotFound();

            return View(payment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payment = await _paymentRepository.GetOneAsync(p => p.Id == id, trackd: true);

            if (payment != null)
            {
                payment.IsDeleted = true;
                _paymentRepository.Update(payment);
                await _paymentRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}