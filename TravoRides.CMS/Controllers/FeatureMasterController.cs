using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.FeaturesMaster;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class FeatureMasterController : Controller
    {
        private readonly IApiService _apiService;

        public FeatureMasterController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/FeatureMaster?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<FeaturesMasterDTO>>>(url);
            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<FeaturesMasterDTO>>($"api/FeatureMaster/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateFeaturesMasterRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateFeaturesMasterRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }


            var apiResponse = await _apiService.PostAsync<ApiResponse<Guid>>(
                   "api/FeatureMaster", model);


            if (apiResponse == null || !apiResponse.IsSuccess)
            {
                return Json(new[] { "False", apiResponse?.Message ?? "FeatureMaster creation failed." });
            }

            return Json(new[] { "True", apiResponse.Message ?? "FeatureMaster created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<FeaturesMasterDTO>>($"api/FeatureMaster/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var model = new UpdateFeaturesMasterRequest
            {
                Id = item.Id,
                Title = item.Title,
                Icon = item.Icon,
                Description = item.Description
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateFeaturesMasterRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            //var FeatureMaster = new UpdateFeatureMasterRequest
            //{
            //    Id = id,
            //    Name = model.Name,
            //    Description = model.Description
            //};

            await _apiService.PutAsync<ApiResponse<Guid>>($"api/FeatureMaster/{id}", model);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/FeatureMaster/{id}");
                if (!success)
                {
                    response = new[] { "False", "Deletion failed" };
                    return Json(response);
                }

                response = new[] { "True", "Deleted successfully" };
                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[] { "False", ex.Message };
                return Json(response);
            }
        }
    }
}
