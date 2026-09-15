
using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Interfaces;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Review;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.Application.DTOs.Enquirer;
using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.Application.DTOs.Subscription;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.SelfDrive;

namespace TravoRides.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(ICabService cabService, ICategoryService categoryService,IFeaturesMasterService featuresMasterService,
       ISelfDriveService selfDriveService, IPackageService packageService,ITransitService transitService,
        IReviewService reviewService,IQuoteService quoteService, ILatestThinkingService latestThinkingService, IEnquiryService EnquiryService,
        ISubscriptionService SubscriptionService) : ControllerBase
    {

        [HttpGet]
        ////[Authorize]
        public async Task<IActionResult> GetCount(CancellationToken cancellationToken)
        {
            var cabs = await cabService.GetAllAsync(new SearchCabRequest());
            var categories = await categoryService.GetAllAsync(new SearchCategoryRequest());

            var packages = await packageService.GetAllAsync(new SearchPackageRequest());

            var transits = await transitService.GetAllAsync(new SearchTransitRequest());

            var features = await featuresMasterService.GetAllAsync(new SearchFeatureMasterRequest());
            var selfDrives = await selfDriveService.GetAllAsync(new SearchSelfDriveRequest());
            var reviews = await reviewService.GetAllAsync(new SearchReviewRequest());
            var enquiries = await EnquiryService.GetAllAsync(new SearchEnquiryRequest());
            var latestThinkings = await latestThinkingService.GetAllAsync(new SearchLatestThinkingRequest());
            var subscribes = await SubscriptionService.GetAllAsync(new SearchSubscriptionRequest());
            var quotes = await SubscriptionService.GetAllAsync(new SearchSubscriptionRequest());
            return Ok(new ApiResponse<object>
            {
                IsSuccess = true,
                Message = "Dashboard Count fetched",
                Data = new { 
                             Cabs = cabs.TotalCount,
                             Categories = categories.TotalCount,
                             SelfDrives = selfDrives.TotalCount,
                            Packages = packages.TotalCount,
                            Transits = transits.TotalCount,
                            Features = features.TotalCount,
                            Reviews = reviews.TotalCount ,
                            Enquiry = enquiries.TotalCount,
                            LatestThinking = latestThinkings.TotalCount,
                            Subscribe = subscribes.TotalCount,
                            Quote = quotes.TotalCount }

            });
        }
    }
}
