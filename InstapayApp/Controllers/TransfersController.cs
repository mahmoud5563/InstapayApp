using InstapayApp.Data;
using InstapayApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class TransfersController : Controller
{
    private readonly AppDbContext _context;

    public TransfersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Transfers
    public async Task<IActionResult> Index()
    {
        var transfers = _context.Transactions
            .Include(t => t.FromCustomer)
            .Include(t => t.ToCustomer);

        return View(await transfers.ToListAsync());
    }

    // GET: Transfers/Create
    public IActionResult Create()
    {
        ViewData["FromCustomerId"] = new SelectList(_context.Customers, "Id", "Name");
        ViewData["ToCustomerId"] = new SelectList(_context.Customers, "Id", "Name");
        return View();
    }

    // POST: Transfers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FromCustomerId,ToCustomerId,Amount,Date")] Transaction transaction)
    {
        if (ModelState.IsValid)
        {
            var fromCustomer = await _context.Customers.FindAsync(transaction.FromCustomerId);
            var toCustomer = await _context.Customers.FindAsync(transaction.ToCustomerId);

            if (fromCustomer == null || toCustomer == null)
            {
                ModelState.AddModelError("", "Invalid customer information.");
                return View(transaction);
            }

            if (fromCustomer.Balance < transaction.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return View(transaction);
            }

            fromCustomer.Balance -= transaction.Amount;
            toCustomer.Balance += transaction.Amount;

            _context.Add(transaction);
            _context.Update(fromCustomer);
            _context.Update(toCustomer);

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                // Handle the exception (log it, show user-friendly message, etc.)
                ModelState.AddModelError("", "Unable to complete the transaction. Please try again later.");
                return View(transaction);
            }
        }

        ViewData["FromCustomerId"] = new SelectList(_context.Customers, "Id", "Name", transaction.FromCustomerId);
        ViewData["ToCustomerId"] = new SelectList(_context.Customers, "Id", "Name", transaction.ToCustomerId);
        return View(transaction);
    }

    // GET: Transfers/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaction = await _context.Transactions
            .Include(t => t.FromCustomer)
            .Include(t => t.ToCustomer)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (transaction == null)
        {
            return NotFound();
        }

        return View(transaction);
    }

    // POST: Transfers/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                var fromCustomer = await _context.Customers.FindAsync(transaction.FromCustomerId);
                var toCustomer = await _context.Customers.FindAsync(transaction.ToCustomerId);

                if (fromCustomer != null && toCustomer != null)
                {
                    fromCustomer.Balance += transaction.Amount;
                    toCustomer.Balance -= transaction.Amount;

                    _context.Update(fromCustomer);
                    _context.Update(toCustomer);

                    _context.Transactions.Remove(transaction);
                    await _context.SaveChangesAsync();
                }
            }
        }
        catch (DbUpdateException ex)
        {
            // Handle the exception (log it, show user-friendly message, etc.)
            ModelState.AddModelError("", "Unable to delete transaction. Please try again later.");
            return View("Delete", await _context.Transactions.FindAsync(id));
        }

        return RedirectToAction(nameof(Index));
    }
}
