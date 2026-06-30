using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppHotelFinal.Data;
using WebAppHotelFinal.Data.Domain;
using WebAppHotelFinal.Models;

namespace WebAppHotelFinal.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ReservationsController(
            ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ============================
        // INDEX
        // ============================
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Reservations
                    .AsNoTracking()
                    .Include(r => r.Room)
                    .Include(r => r.Client)
                    .OrderByDescending(r => r.DateIn)
                    .ToListAsync());
            }

            var userId = _userManager.GetUserId(User);
            var client = await _context.Clients
                .FirstOrDefaultAsync(c => c.AppUserId == userId);

            if (client == null)
                return Forbid();

            return View(await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r => r.Client)
                .Where(r => r.ClientId == client.Id)
                .OrderByDescending(r => r.DateIn)
                .ToListAsync());
        }

        // ============================
        // DETAILS
        // ============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                if (reservation.Client?.AppUserId != userId)
                    return Forbid();
            }

            return View(reservation);
        }

        // ============================
        // CREATE (GET)
        // ============================
        public async Task<IActionResult> Create()
        {
            await LoadDropDownsAsync(User.IsInRole("Admin"));

            return View(new Reservation
            {
                DateIn = DateTime.Today,
                DateOut = DateTime.Today.AddDays(1)
            });
        }

        // ============================
        // CREATE (POST)
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Reservation reservation)
        {
            ValidateDates(reservation);

            if (!User.IsInRole("Admin"))
            {
                var userId = _userManager.GetUserId(User);
                var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.AppUserId == userId);

                if (client == null)
                    return Forbid();

                reservation.ClientId = client.Id;
            }

            bool overlap = await _context.Reservations.AnyAsync(r =>
                r.RoomId == reservation.RoomId &&
                r.DateIn < reservation.DateOut &&
                reservation.DateIn < r.DateOut);

            if (overlap)
                ModelState.AddModelError("", "Стаята не е свободна за избрания период.");

            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(User.IsInRole("Admin"),
                    reservation.RoomId, reservation.ClientId);
                return View(reservation);
            }

            var room = await _context.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservation.RoomId);

            int nights = (reservation.DateOut.Date - reservation.DateIn.Date).Days;
            reservation.TotalPrice = nights * room.Price;

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // EDIT (ADMIN ONLY)
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            await LoadDropDownsAsync(true,
                reservation.RoomId, reservation.ClientId);

            return View(reservation);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Reservation reservation)
        {
            if (id != reservation.Id)
                return NotFound();

            ValidateDates(reservation);

            bool overlap = await _context.Reservations.AnyAsync(r =>
                r.Id != reservation.Id &&
                r.RoomId == reservation.RoomId &&
                r.DateIn < reservation.DateOut &&
                reservation.DateIn < r.DateOut);

            if (overlap)
                ModelState.AddModelError("", "Стаята не е свободна за избрания период.");

            if (!ModelState.IsValid)
            {
                await LoadDropDownsAsync(true,
                    reservation.RoomId, reservation.ClientId);
                return View(reservation);
            }

            var room = await _context.Rooms.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reservation.RoomId);

            int nights = (reservation.DateOut.Date - reservation.DateIn.Date).Days;
            reservation.TotalPrice = nights * room.Price;

            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // DELETE (ADMIN ONLY)
        // ============================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var reservation = await _context.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                .Include(r => r.Client)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return View(reservation);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================
        // HELPERS
        // ============================
        private void ValidateDates(Reservation reservation)
        {
            if (reservation.DateOut <= reservation.DateIn)
            {
                ModelState.AddModelError(nameof(Reservation.DateOut),
                    "Дата на освобождаване трябва да е след дата на настаняване.");
            }
        }

        private async Task LoadDropDownsAsync(
            bool includeClients,
            int? selectedRoomId = null,
            int? selectedClientId = null)
        {
            ViewData["RoomId"] = new SelectList(
                await _context.Rooms.AsNoTracking()
                    .OrderBy(r => r.NumberRoom)
                    .ToListAsync(),
                "Id", "NumberRoom", selectedRoomId);

            if (includeClients)
            {
                ViewData["ClientId"] = new SelectList(
                    await _context.Clients.AsNoTracking()
                        .OrderBy(c => c.FullName)
                        .ToListAsync(),
                    "Id", "FullName", selectedClientId);
            }
        }
    }
}
