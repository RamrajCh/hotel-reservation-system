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
    public class ReservationController : Controller
    {
        private readonly HotelTasmaniaContext _context;

        public ReservationController(HotelTasmaniaContext context)
        {
            _context = context;
        }

        public bool isRoomAvailable(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var reservations = _context.Reservation.Where(r => r.RoomId == roomId);
            foreach (var reservation in reservations)
            {
                if (checkInDate >= reservation.CheckInDate && checkInDate <= reservation.CheckOutDate)
                {
                    return false;
                }
                if (checkOutDate >= reservation.CheckInDate && checkOutDate <= reservation.CheckOutDate)
                {
                    return false;
                }
            }
            return true;
        }

        // GET: Reservation
        public async Task<IActionResult> Index()
        {
            var hotelTasmaniaContext = _context.Reservation.Include(r => r.Customer).Include(r => r.Room);
            ViewData["Active"] = "Reservation";
            return View(await hotelTasmaniaContext.ToListAsync());
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            ViewData["Active"] = "Reservation";
            return View(reservation);
        }

        // GET: Reservation/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName");
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "RoomNumber");
            ViewData["Active"] = "Reservation";
            return View();
        }

        // POST: Reservation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReservationId,RoomId,CustomerId,CheckInDate,CheckOutDate,TotalPrice")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                // Check if the room is available
                var room = await _context.Room.FirstOrDefaultAsync(r => r.RoomId == reservation.RoomId);

                // Check if the customer is already registered
                var customer = await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == reservation.CustomerId);
                if (customer == null)
                {
                    ModelState.AddModelError("CustomerId", "The customer is not registered");
                }
                else
                {
                    // Check if the customer is already registered
                    var customerReservation = await _context.Reservation.FirstOrDefaultAsync(r => r.CustomerId == reservation.CustomerId);
                    if (customerReservation != null)
                    {
                        ModelState.AddModelError("CustomerId", "The customer is already registered");
                    }
                    else
                    {
                        // Check if the check out date is in past
                        if (isRoomAvailable(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate))
                        {
                            _context.Add(reservation);
                            await _context.SaveChangesAsync();
                            ViewData["Active"] = "Reservation";
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            ModelState.AddModelError("CheckOutDate", "The room is not available to reserve.");
                        }
                    }
                }
                
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", reservation.CustomerId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["Active"] = "Reservation";
            return View(reservation);
        }

        // GET: Reservation/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", reservation.CustomerId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["Active"] = "Reservation";
            return View(reservation);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReservationId,RoomId,CustomerId,CheckInDate,CheckOutDate,TotalPrice")] Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the room is available
                    var room = await _context.Room.FirstOrDefaultAsync(r => r.RoomId == reservation.RoomId);

                    // Check if the customer is already registered
                    var customer = await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == reservation.CustomerId);
                    if (customer == null)
                    {
                        ModelState.AddModelError("CustomerId", "The customer is not registered");
                    }
                    else
                    {
                        // Check if the customer is already registered
                        var customerReservation = await _context.Reservation.FirstOrDefaultAsync(r => r.CustomerId == reservation.CustomerId);
                        if (customerReservation != null)
                        {
                            ModelState.AddModelError("CustomerId", "The customer is already registered");
                        }
                        else
                        {
                            // Check if the check out date is in past
                            if (isRoomAvailable(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate))
                            {
                                _context.Update(reservation);
                                await _context.SaveChangesAsync();
                                ViewData["Active"] = "Reservation";
                                return RedirectToAction(nameof(Index));
                            }
                            else
                            {
                                ModelState.AddModelError("CheckOutDate", "The room is not available");
                            }
                        }
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.ReservationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["Active"] = "Reservation";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customer, "CustomerId", "CustomerName", reservation.CustomerId);
            ViewData["RoomId"] = new SelectList(_context.Room, "RoomId", "RoomNumber", reservation.RoomId);
            ViewData["Active"] = "Reservation";
            return View(reservation);
        }

        // GET: Reservation/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reservation == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Reservation";
            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'HotelTasmaniaContext.Reservation'  is null.");
            }
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
            }
            
            await _context.SaveChangesAsync();
            ViewData["Active"] = "Reservation";
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
          return _context.Reservation.Any(e => e.ReservationId == id);
        }
    }
}
