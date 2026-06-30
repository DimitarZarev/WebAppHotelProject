using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppHotelFinal.Data;
using WebAppHotelFinal.Models;

namespace WebAppHotelFinal.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize != 10 && pageSize != 25 && pageSize != 50) pageSize = 10;

            IQueryable<Client> query = _context.Clients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.FullName.Contains(search));
            }

            int totalCount = await query.CountAsync();

            var clients = await query
                .OrderBy(c => c.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(clients);
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Client client)
        {
            if (!ModelState.IsValid) return View(client);

            // Admin-created clients do NOT have an identity user
            client.AppUserId = null;

            _context.Add(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (id != client.Id) return NotFound();
            if (!ModelState.IsValid) return View(client);

            try
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _context.Clients.AnyAsync(e => e.Id == client.Id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _context.Clients.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return RedirectToAction(nameof(Index));

            // Ако клиент има резервации - по желание: забрани изтриване
            bool hasReservations = await _context.Reservations.AnyAsync(r => r.ClientId == id);
            if (hasReservations)
            {
                ModelState.AddModelError("", "Клиентът има резервации и не може да бъде изтрит.");
                return View("Delete", client);
            }

            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
