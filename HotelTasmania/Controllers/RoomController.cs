using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HotelTasmania.Models;

namespace HotelTasmania.Controllers
{
    public class RoomController : Controller
    {
        private readonly HotelTasmaniaContext _context;

        public RoomController(HotelTasmaniaContext context)
        {
            _context = context;
        }

        // GET: Room
        public async Task<IActionResult> Index()
        {
            var hotelTasmaniaContext = _context.Room.Include(r => r.RoomType);
            ViewData["Active"] = "Room";
            return View(await hotelTasmaniaContext.ToListAsync());
        }

        // GET: Room/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Room";
            return View(room);
        }

        // GET: Room/Create
        public IActionResult Create()
        {
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "RoomTypeId", "RoomTypeName");
            ViewData["Active"] = "Room";
            return View();
        }

        // POST: Room/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomId,RoomNumber,RoomTypeId,PricePerNight,Floor,NumberOfBeds")] Room room)
        {
            if (ModelState.IsValid)
            {
                // check if room number is unique
                if (_context.Room.Any(r => r.RoomNumber == room.RoomNumber))
                {
                    ModelState.AddModelError("RoomNumber", "Room number must be unique");
                }
                else
                {
                    _context.Add(room);
                    await _context.SaveChangesAsync();
                    ViewData["Active"] = "Room";
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "RoomTypeId", "RoomTypeName", room.RoomTypeId);
            ViewData["Active"] = "Room";
            return View(room);
        }

        // GET: Room/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "RoomTypeId", "RoomTypeName", room.RoomTypeId);
            ViewData["Active"] = "Room";
            return View(room);
        }

        // POST: Room/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomId,RoomNumber,RoomTypeId,PricePerNight,Floor,NumberOfBeds")] Room room)
        {
            if (id != room.RoomId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // check if room number is unique
                    if (_context.Room.Any(r => r.RoomNumber == room.RoomNumber && r.RoomId != room.RoomId))
                    {
                        ModelState.AddModelError("RoomNumber", "Room number must be unique");
                    }
                    else
                    {
                        _context.Update(room);
                        await _context.SaveChangesAsync();
                        ViewData["Active"] = "Room";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.RoomId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomTypeId"] = new SelectList(_context.RoomType, "RoomTypeId", "RoomTypeName", room.RoomTypeId);
            ViewData["Active"] = "Room";
            return View(room);
        }

        // GET: Room/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Room == null)
            {
                return NotFound();
            }

            var room = await _context.Room
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(m => m.RoomId == id);
            if (room == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Room";
            return View(room);
        }

        // POST: Room/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Room == null)
            {
                return Problem("Entity set 'HotelTasmaniaContext.Room'  is null.");
            }
            var room = await _context.Room.FindAsync(id);
            if (room != null)
            {
                _context.Room.Remove(room);
            }
            
            await _context.SaveChangesAsync();
            ViewData["Active"] = "Room";
            return RedirectToAction(nameof(Index));
        }

        private bool RoomExists(int id)
        {
          return _context.Room.Any(e => e.RoomId == id);
        }
    }
}
