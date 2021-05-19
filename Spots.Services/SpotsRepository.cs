using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spots.Services.ResourceParameters;
using Spots.Data;
using Spots.Domain;
using Spots.Services.Helpers;
using Microsoft.EntityFrameworkCore;

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
        public PagedList<Category> GetCategories(IndexResourceParameters categoryParameters)
        {
            if(categoryParameters == null)
            {
                throw new ArgumentNullException(nameof(categoryParameters));
            }

            var collection = context.Categories as IQueryable<Category>;

            #region Filtering

            #endregion

            #region Searching
            if (!string.IsNullOrWhiteSpace(categoryParameters.SearchQuery))
            {
                var searchQuery = categoryParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery));
            }

            #endregion

            collection = collection.OrderBy(c => c.SortOrder);
            if (categoryParameters.IncludeAll)
            {
                collection = collection.AsNoTracking();
            }
            return PagedList<Category>.Create(collection, categoryParameters.PageNumber
                , categoryParameters.PageSize,categoryParameters.IncludeAll);
        }

        public Category GetCategoryById(Guid categoryId)
        {
            return context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public Category GetCategoryByName(string name)
        {
            
            return context.Categories.Where(c => c.Name == name).FirstOrDefault();
        }

        public Category GetCategoryByNameAndSuperCategory(string name, Guid superCategoryId)
        {
            return context.Categories.Where(c => c.Name == name 
            && c.SuperCategoryId == superCategoryId).FirstOrDefault();
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

        public bool IsSuperCategory(Guid categoryId)
        {
            return context.Categories.Where(c => c.SuperCategoryId == categoryId).Any();
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
            var vendor =  context.Vendors.Include(v => v.Categories).Where(v => v.Id == vendorId).FirstOrDefault();
            return vendor;
            //var vendor = context.Vendors.Where(v => v.Id == vendorId).FirstOrDefault();
            //vendor.Categories = GetCategoriesForVendor(vendorId).ToList();
            //return vendor;
        }

        public IEnumerable<Category> GetCategoriesForVendor(Guid vendorId)
        {
            return context.Categories.Where(c => c.Vendors.Where(v => v.Id == vendorId).FirstOrDefault().Id == vendorId);
        }

        public Vendor GetVendorByName(string name)
        {
            
            return context.Vendors.Where(v => v.Name == name).FirstOrDefault();
        }

        public PagedList<Vendor> GetVendors(IndexResourceParameters vendorParameters)
        {
            if (vendorParameters == null)
            {
                throw new ArgumentNullException(nameof(vendorParameters));
            }

            var collection = context.Vendors as IQueryable<Vendor>;

            #region Filtering

            #endregion

            #region Searching
            if (!string.IsNullOrWhiteSpace(vendorParameters.SearchQuery))
            {
                var searchQuery = vendorParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery));
            }

            #endregion

            collection = (IQueryable<Vendor>)collection.OrderBy(c => c.SortOrder);
      //          .Include(v=>v.Categories);

            return PagedList<Vendor>.Create(collection, vendorParameters.PageNumber
                , vendorParameters.PageSize,vendorParameters.IncludeAll);
        }
        public void UpdateVendor(Guid vendorId, Vendor vendor, IEnumerable<Category> categories)
        {
            var categoryWithVendor = context.Categories
                .Include(c => c.Vendors)
                .Where(c=>c.Vendors.Any(v=>v.Id == vendorId))
                .ToList();
            foreach(var category in categoryWithVendor)
            {
                category.Vendors.Remove(vendor);
            }
            var vend = GetVendorById(vendorId);
            foreach(var category in categories)
            {
                var cat = context.Categories.Where(c => c.Id == category.Id).FirstOrDefault();
                vend.Categories.Add(cat);
            }
        }
        public void DeleteVendor(Vendor vendor)
        {
            context.Remove(vendor);
        }

        

        public bool VendorExists(Guid vendorId)
        {
            return context.Vendors.Where(v => v.Id == vendorId).Any();
        }


        public IEnumerable<Offer> GetOffersForVendor(Guid vendorId)
        {
            return context.Offers.Where(o => o.VendorId == vendorId && o.OfferApproved == true);

        }

        public Offer GetOfferById(Guid vendorId, Guid offerId)
        {
            return context.Offers.Where(o => o.VendorId == vendorId &&
            o.OfferApproved == true && o.Id == offerId).FirstOrDefault();
        }

        public void AddOffer(Guid vendorId, Offer offer)
        {
            if(VendorExists(vendorId) && offer != null)
            {
                offer.VendorId = vendorId;
                if (context.Settings.FirstOrDefault().AutomaticOfferApproval)
                {
                    offer.OfferApproved = true;
                }
                else
                {
                    offer.OfferApproved = false;
                }
                context.Add<Offer>(offer);
            }
        }

        public void DeleteOffer(Offer offer)
        {
                context.Remove(offer);   
        }

        public bool OfferExists(Guid offerId)
        {
            return context.Offers.Where(o => o.Id == offerId).Any();
        }

        public bool Save()
        {
            return (context.SaveChanges() >= 0);
        }

        public IEnumerable<Review> GetReviewsForVendor(Guid vendorId)
        {
            return context.Reviews.Where(r => r.VendorId == vendorId);
        }

        public Review GetReviewById(Guid vendorId, Guid reviewId)
        {
            return context.Reviews.Where(r => r.VendorId == vendorId && r.Id == reviewId).FirstOrDefault();
        }

        public void AddReview(Guid vendorId, Review review)
        {
            if (VendorExists(vendorId) && review != null)
            {
                review.VendorId = vendorId;
                context.Add<Review>(review);
            }
        }

        public void UpdateReview(Guid vendorId, Guid reviewId, Review review)
        {

            review.Updated = true;
        }

        public void DeleteReview(Review review)
        {
            context.Remove(review);
        }

        public bool ReviewExists(Guid reviewId)
        {
            return context.Reviews.Where(r => r.Id == reviewId).Any();
        }


    }
}
