using System;
using System.Collections.Generic;

using Spots.Domain;

namespace Spots.Services
{
    public interface ISpotsRepositroy
    {
        IEnumerable<Category> GetCategories();
        Category GetCategoryById(Guid categoryId);
        Category GetCategoryByName(string name);
        void AddCategory(Category category);
        void UpdateCategory(Guid categoryId, Category category);
        void DeleteCategory(Category category);
        bool CategoryExists(Guid categoryId);
        bool IsSuperCategory(Guid categoryId);

        IEnumerable<Vendor> GetVendors();
        Vendor GetVendorById(Guid vendorId);
        Vendor GetVendorByName(string name);
        void AddVendor(Vendor vendor);
        void UpdateVendor(Guid vendorId, Vendor vendor);
        void DeleteVendor(Vendor vendor);
        bool VendorExists(Guid vendorId);

        bool Save();
    }
}
