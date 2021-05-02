using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spots.Data;
using Spots.Domain;
namespace Spots.Services
{
    public class SpotsRepository : ISpotsRepositroy
    {
        private readonly SpotsContext context;

        public SpotsRepository(SpotsContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddCategory(Category category)
        {
            if(category != null)
            {
                context.Add<Category>(category);
            }
        }
        public IEnumerable<Category> GetCategories()
        {
            return context.Categories;
        }

        public Category GetCategoryById(Guid categoryId)
        {
            return context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public Category GetCategoryByName(string name)
        {
            
            return context.Categories.Where(c => c.Name == name).FirstOrDefault();
        }

        public void UpdateCategory(Guid categoryId, Category category)
        {
            //does nothing for now
        }
        public void DeleteCategory(Category category)
        {
            context.Remove(category);
        }
        public bool CategoryExists(Guid categoryId)
        {
            return context.Categories.Where(c => c.Id == categoryId).Any();
        }


        public void AddVendor(Vendor vendor)
        {
            if(vendor != null)
            {
                context.Add<Vendor>(vendor);
            }
        }
        public Vendor GetVendorById(Guid vendorId)
        {
            return context.Vendors.Where(v => v.Id == vendorId).FirstOrDefault();
        }

        public IEnumerable<Vendor> GetVendors()
        {
            return context.Vendors.OrderBy(v => v.SortOrder);
        }
        public void UpdateVendor(Guid vendorId, Vendor vendor)
        {
            //does nothing for now
        }
        public void DeleteVendor(Vendor vendor)
        {
            context.Remove(vendor);
        }

        

        public bool VendorExists(Guid vendorId)
        {
            return context.Vendors.Where(v => v.Id == vendorId).Any();
        }

        public bool Save()
        {
            return (context.SaveChanges() >= 0);
        }

        
    }
}
