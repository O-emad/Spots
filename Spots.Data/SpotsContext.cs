using Microsoft.EntityFrameworkCore;
using System;
using Spots.Domain;

namespace Spots.Data
{
    public class SpotsContext :DbContext
    {
        public SpotsContext()
        {

        }
        public SpotsContext(DbContextOptions<SpotsContext> dbContextOptions)
            :base(dbContextOptions)
        {
            
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Name> Names { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<VendorGallery> VendorGallery { get; set; }
        public DbSet<VendorVideo> VendorVideos { get; set; }


    }
}
