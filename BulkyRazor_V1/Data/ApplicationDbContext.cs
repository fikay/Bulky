﻿using BulkyRazor_V1.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyRazor_V1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Rom-com", DisplayOrder = 1 },
                 new Category { Id = 2, Name = "Romance", DisplayOrder = 2 }
                ) ;

        }
    }
}
