using System;
using System.Collections.Generic;

using Spots.Domain;

namespace Spots.Services
{
    public interface ISpotsRepositroy
    {
        IEnumerable<Category> GetCategories();
        Category GetCategoryById(Guid categoryId);
        void AddCategory(Category category);
        void UpdateCategory(Guid categoryId, Category category);
        void DeleteCategory(Category category);
        bool CategoryExists(Guid categoryId);

        IEnumerable<Vendor> GetVendors();
        Vendor GetVendorById(Guid vendorId);
        void AddVendor(Vendor vendor);
        void UpdateVendor(Guid vendorId, Vendor vendor);
        void DeleteVendor(Vendor vendor);
        bool VendorExists(Guid vendorId);

        bool Save();
    }
}
