using System;
using System.Collections.Generic;
using Spots.Services.ResourceParameters;
using Spots.Domain;
using Spots.Services.Helpers;

namespace Spots.Services
{
    public interface ISpotsRepositroy
    {
        PagedList<Category> GetCategories(IndexResourceParameters categoryParameters);
        Category GetCategoryById(Guid categoryId);
        Category GetCategoryByName(string name);
        Category GetCategoryByNameAndSuperCategory(string name, Guid superCategoryId);
        void AddCategory(Category category);
        void UpdateCategory(Guid categoryId, Category category);
        void DeleteCategory(Category category);
        bool CategoryExists(Guid categoryId);
        bool IsSuperCategory(Guid categoryId);

        PagedList<Vendor> GetVendors(IndexResourceParameters vendorParameters);
        Vendor GetVendorById(Guid vendorId);
        Vendor GetVendorByName(string name);
        void AddVendor(Vendor vendor);
        void UpdateVendor(Guid vendorId, Vendor vendor, IEnumerable<Category> categories);
        void DeleteVendor(Vendor vendor);
        bool VendorExists(Guid vendorId);

        IEnumerable<Offer> GetOffersForVendor(Guid vendorId);
        Offer GetOfferById(Guid vendorId, Guid offerId);
        void AddOffer(Guid vendorId, Offer offer);
        void DeleteOffer(Offer offer);
        bool OfferExists(Guid offerId);

        IEnumerable<Review> GetReviewsForVendor(Guid vendorId);
        Review GetReviewById(Guid vendorId, Guid reviewId);
        void AddReview(Guid vendorId, Review review);
        void UpdateReview(Guid vendorId, Guid reviewId, Review review);
        void DeleteReview(Review review);
        bool ReviewExists(Guid reviewId);

        PagedList<Ad> GetAds(IndexResourceParameters adParameters);
        Ad GetAdById(Guid id);
        public bool AdExists(Guid id);
        public void AddAd(Ad ad);
        public void DeleteAd(Ad ad);

        bool Save();
    }
}
