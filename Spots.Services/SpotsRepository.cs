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
using Spots.Services.Helpers.ResourceParameters;

namespace Spots.Services
{
    public class SpotsRepository : ISpotsRepositroy
    {
        private readonly SpotsContext context;

        public SpotsRepository(SpotsContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        #region Category
        public void AddCategory(Category category)
        {
            if(category != null)
            {
                context.Add<Category>(category);
            }
        }
        public PagedList<Category> GetCategories(CategoryResourceParameters categoryParameters, string language)
        {
            if(categoryParameters == null)
            {
                throw new ArgumentNullException(nameof(categoryParameters));
            }

            var collection = context.Categories as IQueryable<Category>;



            #region Searching
            if (!string.IsNullOrWhiteSpace(categoryParameters.SearchQuery))
            {
                var searchQuery = categoryParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.Names.Where(n => n.Value.Contains(searchQuery)).Any());
            }

            #endregion

            #region Language
            var lang = language.ToLower().Trim();
            if (!string.IsNullOrWhiteSpace(language))
            {
                if (lang == "en" || lang == "ar")
                {
                    collection = collection.Include(c => c.Names.Where(n => n.Culture == language));
                }
                else
                {
                    collection = collection.Include(c => c.Names.Where(n => n.Culture == "en"));
                }
            }
            #endregion

            #region Filtering
            if (!string.IsNullOrWhiteSpace(categoryParameters.FilterQuery))
            {
                var filterQuery = categoryParameters.FilterQuery.ToLower().Trim();
                if(filterQuery == "level1")
                {
                    collection = collection.Where(c => c.CategoryId == null);
                }
                else if(filterQuery == "level2")
                {
                    collection = collection.Where(c => c.CategoryId == null)
                                           .Include(c => c.Categories)
                                           .ThenInclude(cc => cc.Names.Where(n => n.Culture == lang));
                }
                
            }
            #endregion

            collection = collection.OrderBy(c => c.SortOrder);
            if (categoryParameters.IncludeAll)
            {
                collection = collection.AsNoTracking();
            }
            collection = collection.AsSplitQuery();

            return PagedList<Category>.Create(collection, categoryParameters.PageNumber
                , categoryParameters.PageSize,categoryParameters.IncludeAll);
        }

        public Category GetCategoryById(Guid categoryId, bool includeVendors = false, bool includeSub = false, string language = "en")
        {
            var category =  context.Categories as IQueryable<Category>;
            //check that language has a valid format, if it's not valid use the default "en"
            var lang = language.ToLower().Trim();
            // check if language is ar or en then it's valid and perceed
            if(lang != "ar" && lang != "en")
            {
                //if it's not ar or en check if it's all, if not return default en
                lang = lang == "all" ? "%ar%en%" : "en";
            }
            category = category.Where(c => c.Id == categoryId)
                               .Include(c => c.Names.Where(n=>lang.Contains(n.Culture)));
            category = includeVendors? category.Include(c => c.Vendors):category;
            category = includeSub? category.Include(c => c.Categories)
                                           .ThenInclude(cc => cc.Names.Where(n => lang.Contains(n.Culture))) : category;
            category = category.AsSplitQuery();
            return category.FirstOrDefault();
        }

        public Category GetCategoryByName(string name)
        {
            var categoryId = context.Names.Where(n => n.Value == name).FirstOrDefault().Id;
            return context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();
        }

        public Category GetCategoryByNameAndSuperCategory(string name,string nameAR, Guid? superCategoryId)
        {
            return context.Categories.Where(c => c.Names.Where(n=>n.Value == name || n.Value == nameAR).Any() 
            && c.CategoryId == superCategoryId).FirstOrDefault();
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
            return context.Categories.Where(c => c.CategoryId == categoryId).Any();
        }

        #endregion

        #region Vendor
        public void AddVendor(Vendor vendor)
        {
            if(vendor != null)
            {
                context.Add<Vendor>(vendor);
            }
        }
        public Vendor GetVendorById(Guid vendorId, bool includeOffer)
        {
            //var vendor = context.Vendors as IQueryable<Vendor>;
           var vendor =  context.Vendors.Include(v => v.Categories)
                                        .Include(v=>v.Follows)
                                        .Where(v => v.Id == vendorId);
            if (includeOffer)
            {
                vendor = vendor.Include(v => v.Offers).AsSplitQuery();
            }
            return vendor.FirstOrDefault();
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

        public PagedList<Vendor> GetVendors(VendorResourceParameters vendorParameters)
        {
            if (vendorParameters == null)
            {
                throw new ArgumentNullException(nameof(vendorParameters));
            }

            var collection = context.Vendors as IQueryable<Vendor>;

            #region Filtering
            if (!string.IsNullOrWhiteSpace(vendorParameters.FilterQuery))
            {
                var filterQuery = vendorParameters.FilterQuery.Trim();
                collection = collection.Where(c => c.OwnerId == filterQuery);
            }
            #endregion
            if (vendorParameters.CategoryId != Guid.Empty)
            {
                var categoryId = vendorParameters.CategoryId;
                collection = collection.Where(c => c.Categories.Where(category=>category.Id == categoryId).Any());
            }
            if (vendorParameters.Trusted)
            {
                var trusted = vendorParameters.Trusted;
                collection = collection.Where(v => v.Trusted == trusted) ;
            }

            #region Searching
            if (!string.IsNullOrWhiteSpace(vendorParameters.SearchQuery))
            {
                var searchQuery = vendorParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery)
                                                || c.Description.Contains(searchQuery));
            }

            #endregion

            collection = collection.Include(v=>v.Follows)
                                   .OrderBy(c => c.SortOrder);
      //          .Include(v=>v.Categories);

            return PagedList<Vendor>.Create(collection, vendorParameters.PageNumber
                , vendorParameters.PageSize,vendorParameters.IncludeAll);
        }
        public void UpdateVendor(Guid vendorId, Vendor vendor, IEnumerable<Category> categories)
        {
            //var categoryWithVendor = context.Categories
            //    .Include(c => c.Vendors)
            //    .Where(c=>c.Vendors.Any(v=>v.Id == vendorId))
            //    .ToList();
            //foreach(var category in categoryWithVendor)
            //{
            //    category.Vendors.Remove(vendor);
            //}
            //var vend = GetVendorById(vendorId,false);
            //foreach(var category in categories)
            //{
            //    var cat = context.Categories.Where(c => c.Id == category.Id).FirstOrDefault();
            //    vend.Categories.Add(cat);
            //}
        }
        public void DeleteVendor(Vendor vendor)
        {
            context.Remove(vendor);
        }

        public bool VendorExists(Guid vendorId)
        {
            return context.Vendors.Where(v => v.Id == vendorId).Any();
        }

        #endregion

        #region Offer

        public IEnumerable<Offer> GetOffersForVendor(Guid vendorId)
        {
            return context.Offers.Where(o => o.VendorId == vendorId);

        }
        public IEnumerable<Offer> GetPendingOffers()
        {
            return context.Offers.Where(o => o.OfferApproved == false);
        }

        public Offer GetSingleOfferById(Guid id)
        {
            return context.Offers.FirstOrDefault(o => o.Id == id);
        }

        public Offer GetOfferById(Guid vendorId, Guid offerId)
        {
            return context.Offers.Where(o => o.VendorId == vendorId && o.Id == offerId).FirstOrDefault();
        }

        public void AddOffer(Guid vendorId, Offer offer)
        {
            if(VendorExists(vendorId) && offer != null)
            {
                offer.VendorId = vendorId;
                if (GetSetting().AutomaticOfferApproval)
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

        #endregion

        #region Review

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
            if (review == null)
                return;
            var vendor = context.Vendors.FirstOrDefault(v => v.Id == vendorId);
            if (vendor == null)
                return;
            var totalStars = vendor.ReviewAverage * vendor.ReviewCount;
            vendor.ReviewCount++;
            totalStars += review.ReviewValue;
            vendor.ReviewAverage = totalStars / vendor.ReviewCount;
            review.VendorId = vendorId;
            context.Add<Review>(review);
        }

        public void UpdateReview(Guid vendorId, Guid reviewId, Review review)
        {

            review.Updated = true;
            var vendor = context.Vendors.FirstOrDefault(v => v.Id == vendorId);
            if (vendor == null)
                return;
            var originalReview = context.Reviews.FirstOrDefault(r => r.Id == reviewId);
            if (originalReview == null)
                return;
            var reviewValueChange = review.ReviewValue - originalReview.ReviewValue;
            var totalStars = vendor.ReviewAverage * vendor.ReviewCount;
            totalStars += reviewValueChange;
            vendor.ReviewAverage = totalStars / vendor.ReviewCount;
        }

        public void DeleteReview(Review review)
        {
            var vendor = context.Vendors.FirstOrDefault(v => v.Id == review.VendorId);
            if (vendor == null)
                return;
            var totalStars = vendor.ReviewAverage * vendor.ReviewCount;
            vendor.ReviewCount--;
            totalStars -= review.ReviewValue;
            vendor.ReviewAverage = totalStars / vendor.ReviewCount;
            context.Remove(review);
        }

        public bool ReviewExists(Guid reviewId)
        {
            return context.Reviews.Where(r => r.Id == reviewId).Any();
        }

        #endregion

        #region Ad

        public PagedList<Ad> GetAds(IndexResourceParameters adParameters)
        {
            if(adParameters == null)
            {
                throw new ArgumentNullException(nameof(adParameters));
            }

            var collection = context.Ads as IQueryable<Ad>;

            #region filtering

            #endregion

            #region searching
            if (!string.IsNullOrWhiteSpace(adParameters.SearchQuery))
            {
                var searchQuery = adParameters.SearchQuery.Trim();
                collection = collection.Where(c => c.Name.Contains(searchQuery));
            }
            #endregion

            collection = collection.OrderBy(c => c.SortOrder);

            return PagedList<Ad>.Create(collection, adParameters.PageNumber,
                adParameters.PageSize, adParameters.IncludeAll);
        }

        public Ad GetAdById(Guid id)
        {
            return context.Ads.Where(a => a.Id == id).FirstOrDefault();
        }

        public bool AdExists(Guid id)
        {
            return context.Ads.Where(a=>a.Id == id).Any();
        }

        public void AddAd(Ad ad)
        {
            if (ad != null)
            {
                context.Add<Ad>(ad);
            }
        }

        public void DeleteAd(Ad ad)
        {
            context.Remove(ad);
        }

        public void UpdateAd(Guid adId, Ad ad)
        {

        }
        #endregion

        #region Setting
        public Setting GetSetting()
        {
            if (!SettingExists())
            {
                CreateSetting();
            }
            return context.Settings.FirstOrDefault();
        }

        public void CreateSetting()
        {
            var setting = new Setting()
            {
                AutomaticOfferApproval = true
            };
            context.Add<Setting>(setting);
            context.SaveChanges();
        }
        public void DeleteSetting()
        {
            var setting = context.Settings.FirstOrDefault();
            context.Remove(setting);
            context.SaveChanges();
        }
        public void UpdateSetting()
        {

        }

        public bool SettingExists()
        {
            return context.Settings.Any();
        }
        #endregion
        public bool Save()
        {
            return (context.SaveChanges() >= 0);
        }

        #region Follow
        void ISpotsRepositroy.AddFollow(Follow follow)
        {
            context.Add<Follow>(follow);
        }

        void ISpotsRepositroy.DeleteFollow(Follow follow)
        {
            context.Remove(follow);
        }


        Follow ISpotsRepositroy.VendorIsFollowedByUser(Guid vendorId, Guid userId)
        {
            return context.Follows.Where(f => f.VendorId == vendorId && f.UserId == userId).FirstOrDefault();
        }
        #endregion

        #region Video
        List<VendorVideo> ISpotsRepositroy.GetVideosForVendor(Guid vendorId)
        {
            return context.VendorVideos.Where(v => v.VendorId == vendorId).ToList();
        }

        VendorVideo ISpotsRepositroy.GetVideoById(Guid id)
        {
            return context.VendorVideos.FirstOrDefault(v => v.Id == id);
        }

        void ISpotsRepositroy.AddVideo(Guid vendorId, VendorVideo video)
        {
            if (!VendorExists(vendorId))
                return;
            video.VendorId = vendorId;
            context.Add<VendorVideo>(video);
        }

        void ISpotsRepositroy.DeleteVideo(VendorVideo video)
        {
            context.Remove(video);
        }
        #endregion
        #region Gallery
        List<VendorGallery> ISpotsRepositroy.GetGalleriesForVendor(Guid vendorId)
        {
            return context.VendorGallery.Where(v => v.VendorId == vendorId).ToList();
        }

        VendorGallery ISpotsRepositroy.GetGalleryById(Guid id)
        {
            return context.VendorGallery.FirstOrDefault(v => v.Id == id);
        }

        void ISpotsRepositroy.AddGallery(Guid vendorId, VendorGallery gallery)
        {
            if (!VendorExists(vendorId))
                return;
            gallery.VendorId = vendorId;
            context.Add<VendorGallery>(gallery);
        }

        void ISpotsRepositroy.DeleteGallery(VendorGallery gallery)
        {
            context.Remove(gallery);
        }
        #endregion

    }
}
