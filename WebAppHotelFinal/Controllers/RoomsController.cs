using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAppHotelFinal.Data;
using WebAppHotelFinal.Models;

namespace WebAppHotelFinal.Controllers
{
    public class RoomsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Rooms
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .AsNoTracking()
                .OrderBy(r => r.NumberRoom)
                .ToListAsync();

            return View(rooms);
        }

        // GET: Rooms/Details/5
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }
        [Authorize(Roles = "Admin")]
        
        // GET: Rooms/Create
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin")]
        
        // POST: Rooms/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(Room room)
        {
            if (!ModelState.IsValid) return View(room);

            bool exists = await _context.Rooms.AnyAsync(r => r.NumberRoom == room.NumberRoom);
            if (exists)
            {
                ModelState.AddModelError(nameof(Room.NumberRoom), "Вече има стая с този номер.");
                return View(room);
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Edit/5
        [Authorize(Roles = "Admin")]
       
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id) return NotFound();
            if (!ModelState.IsValid) return View(room);

            bool exists = await _context.Rooms.AnyAsync(r => r.NumberRoom == room.NumberRoom && r.Id != room.Id);
            if (exists)
            {
                ModelState.AddModelError(nameof(Room.NumberRoom), "Вече има стая с този номер.");
                return View(room);
            }

            _context.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Rooms/Delete/5
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var room = await _context.Rooms
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null) return NotFound();

            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return RedirectToAction(nameof(Index));

            bool hasReservations = await _context.Reservations.AnyAsync(r => r.RoomId == id);
            if (hasReservations)
            {
                ModelState.AddModelError("", "Стаята има резервации и не може да бъде изтрита.");
                return View("Delete", room);
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
