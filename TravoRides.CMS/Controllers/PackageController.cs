using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class PackageController : Controller
    {
        private readonly IApiService _apiService;

        public PackageController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Package?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<PackageDTO>>>(url);
            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<PackageDTO>>($"api/Package/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePackageRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePackageRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Title ?? string.Empty), nameof(model.Title));
            formData.Add(new StringContent(model.Price.ToString() ?? string.Empty), nameof(model.Price));
            formData.Add(new StringContent(model.Discount.ToString() ?? string.Empty), nameof(model.Discount));
            formData.Add(new StringContent(model.Itinerary ?? string.Empty), nameof(model.Itinerary));
            formData.Add(new StringContent(model.Inclusions ?? string.Empty), nameof(model.Inclusions));
            formData.Add(new StringContent(model.Distance.ToString() ?? string.Empty), nameof(model.Distance));
            formData.Add(new StringContent(model.Route ?? string.Empty), nameof(model.Route));
            formData.Add(new StringContent(model.PlacesCovered ?? string.Empty), nameof(model.PlacesCovered));
            formData.Add(new StringContent(model.Duration ?? string.Empty), nameof(model.Duration));
           

            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.Image), model.Image.FileName);
            }

            await _apiService.PostAsync<ApiResponse<Guid>>("api/Package", formData);

            response = new[] { "True", "Created successfully." };
            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<PackageDTO>>($"api/Package/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var model = new UpdatePackageRequest
            {
                Id = item.Id,
                Title = item.Title,
                Price = item.Price,
                Discount = item.Discount,
                Distance= item.Distance,
                PlacesCovered = item.PlacesCovered,
                Itinerary = item.Itinerary,
                Route = item.Route,
                Inclusions = item.Inclusions,
                Duration = item.Duration,
                ImageUrl = item.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdatePackageRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Title ?? string.Empty), nameof(model.Title));
            formData.Add(new StringContent(model.Price.ToString() ?? string.Empty), nameof(model.Price));
            formData.Add(new StringContent(model.Discount.ToString() ?? string.Empty), nameof(model.Discount));
            formData.Add(new StringContent(model.Itinerary ?? string.Empty), nameof(model.Itinerary));
            formData.Add(new StringContent(model.Inclusions ?? string.Empty), nameof(model.Inclusions));
            formData.Add(new StringContent(model.Distance.ToString() ?? string.Empty), nameof(model.Distance));
            formData.Add(new StringContent(model.Route ?? string.Empty), nameof(model.Route));
            formData.Add(new StringContent(model.PlacesCovered ?? string.Empty), nameof(model.PlacesCovered));
            formData.Add(new StringContent(model.Duration ?? string.Empty), nameof(model.Duration));

            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.Image), model.Image.FileName);
            }

            await _apiService.PutAsync<ApiResponse<Guid>>($"api/Package/{id}", formData);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Package/{id}");
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
