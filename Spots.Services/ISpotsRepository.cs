﻿using System;
using System.Collections.Generic;
using Spots.Services.ResourceParameters;
using Spots.Domain;
using Spots.Services.Helpers;
using Spots.Services.Helpers.ResourceParameters;

namespace Spots.Services
{
    public interface ISpotsRepositroy
    {
        PagedList<Category> GetCategories(CategoryResourceParameters categoryParameters, string language);
        Category GetCategoryById(Guid categoryId, bool includeVendors = false, bool includeSub = false, string language = "en");
        Category GetCategoryByName(string name);
        Category GetCategoryByNameAndSuperCategory(string name, string nameAR, Guid? superCategoryId);
        void AddCategory(Category category);
        void UpdateCategory(Guid categoryId, Category category);
        void DeleteCategory(Category category);
        bool CategoryExists(Guid categoryId);
        bool IsSuperCategory(Guid categoryId);

        PagedList<Vendor> GetVendors(VendorResourceParameters vendorParameters);
        Vendor GetVendorById(Guid vendorId, bool includeOffer);
        Guid GetVendorIdByOwner(string ownerId);
        Vendor GetVendorByName(string name);
        void AddVendor(Vendor vendor);
        void UpdateVendor(Guid vendorId, Vendor vendor, IEnumerable<Category> categories);
        void DeleteVendor(Vendor vendor);
        bool VendorExists(Guid vendorId);

        IEnumerable<Offer> GetOffersForVendor(Guid vendorId);
        IEnumerable<Offer> GetPendingOffers();
        Offer GetSingleOfferById(Guid id);
        Offer GetOfferById(Guid vendorId, Guid offerId, bool includeOfferUses = false);
        void AddOffer(Guid vendorId, Offer offer);
        void DeleteOffer(Offer offer);
        bool OfferExists(Guid offerId);
        bool EligibleOfferUse(Guid offerId, string userSubject);
        void AddOfferUse(OfferUse offerUse);

        IEnumerable<Review> GetReviewsForVendor(Guid vendorId);
        Review GetReviewById(Guid vendorId, Guid reviewId);
        void AddReview(Guid vendorId, Review review);
        void UpdateReview(Guid vendorId, Guid reviewId, Review review);
        void DeleteReview(Review review);
        bool ReviewExists(Guid reviewId);

        PagedList<Ad> GetAds(IndexResourceParameters adParameters);
        Ad GetAdById(Guid id);
         bool AdExists(Guid id);
         void AddAd(Ad ad);
         void DeleteAd(Ad ad);
        void UpdateAd(Guid adId, Ad ad);

        void AddFollow(Follow follow);
        void DeleteFollow(Follow follow);
        Follow VendorIsFollowedByUser(Guid vendorId, Guid userId);

        List<VendorVideo> GetVideosForVendor(Guid vendorId);
        VendorVideo GetVideoById(Guid id);
        void AddVideo(Guid vendorId, VendorVideo video);
        void DeleteVideo(VendorVideo video);

        List<VendorGallery> GetGalleriesForVendor(Guid vendorId);
        VendorGallery GetGalleryById(Guid id);
        void AddGallery(Guid vendorId, VendorGallery gallery);
        void DeleteGallery(VendorGallery gallery);


        Setting GetSetting();
        void UpdateSetting();
        bool SettingExists();
        bool Save();


    }
}
