using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotelTasmania.Models;

    public class HotelTasmaniaContext : DbContext
    {
        public HotelTasmaniaContext (DbContextOptions<HotelTasmaniaContext> options)
            : base(options)
        {
        }

        public DbSet<HotelTasmania.Models.RoomType> RoomType { get; set; }

        public DbSet<HotelTasmania.Models.Room> Room { get; set; }

        public DbSet<HotelTasmania.Models.Customer> Customer { get; set; }
    }
