using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Transit;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class TransitController : Controller
    {
        private readonly IApiService _apiService;

        public TransitController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Transit?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<TransitDTO>>>(url);
            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<TransitDTO>>($"api/Transit/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateTransitRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTransitRequest model)
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
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
           


            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.Image), model.Image.FileName);
            }

            await _apiService.PostAsync<ApiResponse<Guid>>("api/Transit", formData);

            response = new[] { "True", "Created successfully." };
            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<TransitDTO>>($"api/Transit/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var model = new UpdateTransitRequest
            {
                Id = item.Id,
                Title = item.Title,
                Price = item.Price,
                Discount = item.Discount,
               Description = item.Description,
                ImageUrl = item.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateTransitRequest model)
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
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
           

            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent = new StreamContent(model.Image.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                formData.Add(imageContent, nameof(model.Image), model.Image.FileName);
            }

            await _apiService.PutAsync<ApiResponse<Guid>>($"api/Transit/{id}", formData);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Transit/{id}");
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

        public async Task<IActionResult> GetTransitRates(Guid id, Guid cabid)
        {
            var response = await _apiService.GetAsync<ApiResponse<TransitCabRateDTO>>(
                $"api/Transit/{id}/rates/{cabid}"
            );

            if (response == null || !response.IsSuccess)
            {
                return NotFound();
            }

            return Json(response.Data);
        }

        public async Task<IActionResult> GetCabsWithRates(Guid transitId)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<TransitCabRateDTO>>>(
                    $"api/Transit/{transitId}/cabs");

            return Json(response);
        }
    }
}
