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

            ViewData["Active"] = "Booking";
            return View(model);
        }

        // GET: Booking/NewCustomer
        public IActionResult NewCustomer(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _context.Room.Where(r => r.RoomId == id).Include(r => r.RoomType).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }

            ViewData["Active"] = "Booking";
            ViewData["RoomId"] = room.RoomId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewCustomer(int? id, [Bind("CustomerId,CustomerName,Address,PhoneNumber,Email,DateOfBirth")] Customer customer)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = _context.Room.Where(r => r.RoomId == id).Include(r => r.RoomType).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (_context.Customer.Any(c => c.Email == customer.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use");
                }
                else if (_context.Customer.Any(c => c.PhoneNumber == customer.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "Phone number is already in use");
                }
                else
                {
                    _context.Add(customer);
                await _context.SaveChangesAsync();
                ViewData["Active"] = "Booking";
                    return RedirectToAction("NewReservation", "Booking", new { RoomId = id, CustomerId = customer.CustomerId });
                }
            }

            ViewData["Active"] = "Booking";
            ViewData["RoomId"] = id;
            return View();
        }

        // GET: Booking/NewReservation
        [Route("Booking/NewReservation/{RoomId}/{CustomerId}")]
        public IActionResult NewReservation(int RoomId,int CustomerId){

            var room = _context.Room.Where(r => r.RoomId == RoomId).Include(r => r.RoomType).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }

            var customer = _context.Customer.Where(c => c.CustomerId == CustomerId).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }
            // model.Reservation = new Reservation();
            // model.Reservation.RoomId = RoomId;
            // model.Reservation.CustomerId = CustomerId;

            ViewData["Active"] = "Booking";
            ViewData["RoomId"] = RoomId;
            ViewData["RoomNumber"] = room.RoomNumber;
            ViewData["CustomerId"] = CustomerId;
            ViewData["CustomerName"] = customer.CustomerName;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Booking/NewReservation/{RoomId}/{CustomerId}")]
        public async Task<IActionResult> NewReservation(int RoomId, int CustomerId, [Bind("ReservationId,RoomId,CustomerId,CheckInDate,CheckOutDate")] Reservation reservation)
        {

            var room = _context.Room.Where(r => r.RoomId == RoomId).Include(r => r.RoomType).FirstOrDefault();
            if (room == null)
            {
                return NotFound();
            }

            var customer = _context.Customer.Where(c => c.CustomerId == CustomerId).FirstOrDefault();
            if (customer == null)
            {
                return NotFound();
            }

            // Check if the room is available
                room = await _context.Room.FirstOrDefaultAsync(r => r.RoomId == reservation.RoomId);

                // Check if the customer is already registered
                customer = await _context.Customer.FirstOrDefaultAsync(c => c.CustomerId == reservation.CustomerId);
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
                            // calculate total price
                            var totalPrice = calculateTotalPrice(reservation.RoomId, reservation.CheckInDate, reservation.CheckOutDate);
                            reservation.TotalPrice = totalPrice;
                            _context.Add(reservation);
                            await _context.SaveChangesAsync();
                            ViewData["Active"] = "Booking";
                            return RedirectToAction(nameof(RoomDetail), new { id = RoomId });
                        }
                        else
                        {
                            ModelState.AddModelError("CheckOutDate", "The room is not available to reserve.");
                        }
                    }
                }

            ViewData["Active"] = "Booking";
            ViewData["RoomId"] = RoomId;
            ViewData["RoomNumber"] = room.RoomNumber;
            ViewData["CustomerId"] = CustomerId;
            ViewData["CustomerName"] = customer.CustomerName;
            return View();
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

        // calculate total price
        public decimal calculateTotalPrice(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var room = _context.Room.Where(r => r.RoomId == roomId).Include(r => r.RoomType).FirstOrDefault();
            if (room == null)
            {
                return 0;
            }
            var totalPrice = room.PricePerNight * (checkOutDate - checkInDate).Days;
            return totalPrice;
        }
    }
}