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
    public class CustomerController : Controller
    {
        private readonly HotelTasmaniaContext _context;

        public CustomerController(HotelTasmaniaContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            ViewData["Active"] = "Customer";
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Customer";
            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            ViewData["Active"] = "Customer";
            return View();
        }

        // POST: Customer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerName,Address,PhoneNumber,Email,DateOfBirth")] Customer customer)
        {
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
                    ViewData["Active"] = "Customer";
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["Active"] = "Customer";
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Customer";
            return View(customer);
        }

        // POST: Customer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerName,Address,PhoneNumber,Email,DateOfBirth")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.Customer.Any(c => c.Email == customer.Email && c.CustomerId != customer.CustomerId))
                    {
                        ModelState.AddModelError("Email", "Email is already in use");
                    }
                    else if (_context.Customer.Any(c => c.PhoneNumber == customer.PhoneNumber && c.CustomerId != customer.CustomerId))
                    {
                        ModelState.AddModelError("PhoneNumber", "Phone number is already in use");
                    }
                    else
                    {
                        _context.Update(customer);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["Active"] = "Customer";
                return RedirectToAction(nameof(Index));
            }
            ViewData["Active"] = "Customer";
            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customer == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewData["Active"] = "Customer";
            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customer == null)
            {
                return Problem("Entity set 'HotelTasmaniaContext.Customer'  is null.");
            }
            var customer = await _context.Customer.FindAsync(id);
            if (customer != null)
            {
                _context.Customer.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            ViewData["Active"] = "Customer";
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.CustomerId == id);
        }
    }
}
