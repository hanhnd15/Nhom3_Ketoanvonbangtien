using Microsoft.AspNetCore.Mvc;
using MoneyManagementApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using OfficeOpenXml;

namespace MoneyManagementApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Transaction/
        public async Task<IActionResult> Index()
        {
            var transactions = await _context.Transactions.ToListAsync();
            return View(transactions);
        }

        // API: Lấy danh sách giao dịch theo ngày hoặc loại giao dịch
        [HttpGet]
        public async Task<IActionResult> GetTransactions(string type = "all", string date = "")
        {
            var query = _context.Transactions.AsQueryable();

            // Lọc theo ngày
            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
            {
                query = query.Where(t => t.TransactionDate.Date == parsedDate.Date);
            }

            // Lọc theo loại giao dịch (Thu/Chi)
            if (type != "all")
            {
                type = type.ToLower().Trim();
                query = query.Where(t => t.TransactionType.ToLower() == (type == "income" ? "thu" : "chi"));
            }

            var result = await query.Select(t => new
            {
                t.Id,
                Amount = t.Amount.ToString("C", new System.Globalization.CultureInfo("vi-VN")),
                TransactionType = t.TransactionType,
                t.Description,
                t.DebitAccount,
                t.CreditAccount,
                TransactionDate = t.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToListAsync();

            if (!result.Any())
            {
                return NotFound("Không có giao dịch nào phù hợp.");
            }

            return Json(result);
        }

        // GET: /Transaction/Add
        public IActionResult AddTransaction()
        {
            return View();
        }

        // POST: /Transaction/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTransaction(Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: /Transaction/Edit/{id}
        public async Task<IActionResult> EditTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: /Transaction/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTransaction(int id, Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(transaction);
        }

        // GET: /Transaction/Delete/{id}
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: /Transaction/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
