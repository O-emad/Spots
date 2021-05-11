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
            return context.Categories.ToList();
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
            return context.Vendors.Where(v => v.Id == vendorId).FirstOrDefault();
        }

        public Vendor GetVendorByName(string name)
        {
            return context.Vendors.Where(v => v.Name == name).FirstOrDefault();
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
