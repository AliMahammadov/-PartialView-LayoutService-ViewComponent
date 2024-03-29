﻿using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using Pronia.Models;
using System.Reflection;

namespace Pronia.DAL
{
    public class AppDbContext:DbContext
    {



        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }
        public DbSet<Category> categories { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Slider> Sliders{ get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<MehsulInformation> MehsulInformation { get; set; }
        public DbSet<MehsulSize> MehsulSize { get; set; }
        public DbSet<MehsulColor> MehsulColor { get; set; }
        public DbSet<MehsulImage> MehsulImage { get; set; }
          public DbSet<Color> Colors { get; set; }
        public DbSet<Mehsul> Mehsuls { get; set; }


    }
}
