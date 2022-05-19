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
    public class BookingController : Controller
    {
        private readonly HotelTasmaniaContext _context;

        public BookingController(HotelTasmaniaContext context)
        {
            _context = context;
        }

        // GET: Booking
        public async Task<IActionResult> Index()
        {
            var standard_rooms = await _context.Room.Where(r => r.RoomType.RoomTypeName == "Standard").Include(r => r.RoomType).ToListAsync();
            var deluxe_rooms = await  _context.Room.Where(r => r.RoomType.RoomTypeName == "Deluxe").Include(r => r.RoomType).ToListAsync();
            var suite_rooms = await _context.Room.Where(r => r.RoomType.RoomTypeName == "Suite").Include(r => r.RoomType).ToListAsync();

            dynamic model = new System.Dynamic.ExpandoObject();
            model.standard_rooms = standard_rooms;
            model.deluxe_rooms = deluxe_rooms;
            model.suite_rooms = suite_rooms;

            ViewData["Active"] = "Booking";
            return View(model);
        }

        // GET: Room Details
        public async Task<IActionResult> RoomDetail(int? id)
        {
            if (id == null)
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

            var reservations = await _context.Reservation.Where(r => r.RoomId == id).Include(r => r.Customer).ToListAsync();

            dynamic model = new System.Dynamic.ExpandoObject();
            model.room = room;
            model.reservations = reservations;

            return View(model);
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
    }
}