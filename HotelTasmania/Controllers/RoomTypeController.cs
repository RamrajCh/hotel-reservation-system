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
    public class RoomTypeController : Controller
    {
        private readonly HotelTasmaniaContext _context;

        public RoomTypeController(HotelTasmaniaContext context)
        {
            _context = context;
        }

        // GET: RoomType
        public async Task<IActionResult> Index()
        {
            ViewData["Active"] = "RoomType";
            return _context.RoomType != null ? 
                          View(await _context.RoomType.ToListAsync()) :
                          Problem("Entity set 'HotelTasmaniaContext.RoomType'  is null.");
        }

        // GET: RoomType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.RoomType == null)
            {
                return NotFound();
            }

            var roomType = await _context.RoomType
                .FirstOrDefaultAsync(m => m.RoomTypeId == id);
            if (roomType == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "RoomType";
            return View(roomType);
        }

        // GET: RoomType/Create
        public IActionResult Create()
        {
            ViewData["Active"] = "RoomType";
            return View();
        }

        // POST: RoomType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoomTypeId,RoomTypeName")] RoomType roomType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(roomType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Active"] = "RoomType";
            return View(roomType);
        }

        // GET: RoomType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.RoomType == null)
            {
                return NotFound();
            }

            var roomType = await _context.RoomType.FindAsync(id);
            if (roomType == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "RoomType";
            return View(roomType);
        }

        // POST: RoomType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RoomTypeId,RoomTypeName")] RoomType roomType)
        {
            if (id != roomType.RoomTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(roomType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomTypeExists(roomType.RoomTypeId))
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
            ViewData["Active"] = "RoomType";
            return View(roomType);
        }

        // GET: RoomType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.RoomType == null)
            {
                return NotFound();
            }

            var roomType = await _context.RoomType
                .FirstOrDefaultAsync(m => m.RoomTypeId == id);
            if (roomType == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "RoomType";
            return View(roomType);
        }

        // POST: RoomType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.RoomType == null)
            {
                return Problem("Entity set 'HotelTasmaniaContext.RoomType'  is null.");
            }
            var roomType = await _context.RoomType.FindAsync(id);
            if (roomType != null)
            {
                _context.RoomType.Remove(roomType);
            }
            
            await _context.SaveChangesAsync();
            ViewData["Active"] = "RoomType";
            return RedirectToAction(nameof(Index));
        }

        private bool RoomTypeExists(int id)
        {
            ViewData["Active"] = "RoomType";
            return (_context.RoomType?.Any(e => e.RoomTypeId == id)).GetValueOrDefault();
        }
    }
}
