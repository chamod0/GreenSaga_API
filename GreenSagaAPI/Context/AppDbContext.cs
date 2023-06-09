﻿using GreenSagaAPI.Migrations;
using GreenSagaAPI.Models;
using GreenSagaAPI.Models.partial;
using Microsoft.EntityFrameworkCore;


namespace GreenSagaAPI.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) 
        {
        

        }
        public DbSet<User> Users { get; set; }
        public DbSet<cultivationProjects> Projects { get; set; }
        public DbSet<ProjecwithSupervisor> ProjecwithSupervisor { get; set; }
        public DbSet<TimeLineBox> TimeLineBox { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<cultivationProjects>().ToTable("Projects");
           
            modelBuilder.Entity<TimeLineBox>().ToTable("Timeline");

        }
    }
}
