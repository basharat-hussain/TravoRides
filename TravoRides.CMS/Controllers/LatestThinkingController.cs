using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.LatestThinking;
using TravoRides.CMS.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http.Headers;

namespace TravoRides.CMS.Controllers
{
    public class LatestThinkingController : Controller
    {
        private readonly IApiService _apiService;

        public LatestThinkingController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/LatestThinking?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<LatestThinkingDTO>>>(url);
            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<LatestThinkingDTO>>($"api/LatestThinking/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateLatestThinkingRequest());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateLatestThinkingRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Title ?? string.Empty), nameof(model.Title));
            formData.Add(new StringContent(model.Subtitle ?? string.Empty), nameof(model.Subtitle));
            formData.Add(new StringContent(model.ImageAltText ?? string.Empty), nameof(model.ImageAltText));
            formData.Add(new StringContent(model.Author ?? string.Empty), nameof(model.Author));
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
            formData.Add(new StringContent(model.MetaTitle ?? string.Empty), nameof(model.MetaTitle));
            formData.Add(new StringContent(model.MetaDescription ?? string.Empty), nameof(model.MetaDescription));
            formData.Add(new StringContent(model.Slug ?? string.Empty), nameof(model.Slug));
            formData.Add(new StringContent(model.CanonicalUrl ?? string.Empty), nameof(model.CanonicalUrl));
            formData.Add(new StringContent(model.Summary ?? string.Empty), nameof(model.Summary));
            formData.Add(new StringContent(model.KeyTakeaways ?? string.Empty), nameof(model.KeyTakeaways));
            formData.Add(new StringContent(model.PublishedOn?.ToString("o") ?? string.Empty), nameof(model.PublishedOn));

            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var imageContent = new StreamContent(model.ImageUrl.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageUrl.ContentType);
                formData.Add(imageContent, nameof(model.ImageUrl), model.ImageUrl.FileName);
            }

            await _apiService.PostAsync<ApiResponse<object>>("api/LatestThinking", formData);

            response = new[] { "True", "Created successfully." };
            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<LatestThinkingDTO>>($"api/LatestThinking/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();

            var model = new UpdateLatestThinkingRequest
            {
                Id = item.Id,
                Title = item.Title,
                Subtitle = item.Subtitle,
                ImageAltText = item.ImageAltText,
                Author = item.Author,
                Description = item.Description,
                MetaTitle = item.MetaTitle,
                MetaDescription = item.MetaDescription,
                Slug = item.Slug,
                CanonicalUrl = item.CanonicalUrl,
                Summary = item.Summary,
                KeyTakeaways = item.KeyTakeaways,
                PublishedOn = item.PublishedOn,
                ImageUrlUrl = item.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateLatestThinkingRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[] { "False", "Validation Failed" };
                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(model.Title ?? string.Empty), nameof(model.Title));
            formData.Add(new StringContent(model.Subtitle ?? string.Empty), nameof(model.Subtitle));
            formData.Add(new StringContent(model.ImageAltText ?? string.Empty), nameof(model.ImageAltText));
            formData.Add(new StringContent(model.Author ?? string.Empty), nameof(model.Author));
            formData.Add(new StringContent(model.Description ?? string.Empty), nameof(model.Description));
            formData.Add(new StringContent(model.MetaTitle ?? string.Empty), nameof(model.MetaTitle));
            formData.Add(new StringContent(model.MetaDescription ?? string.Empty), nameof(model.MetaDescription));
            formData.Add(new StringContent(model.Slug ?? string.Empty), nameof(model.Slug));
            formData.Add(new StringContent(model.CanonicalUrl ?? string.Empty), nameof(model.CanonicalUrl));
            formData.Add(new StringContent(model.Summary ?? string.Empty), nameof(model.Summary));
            formData.Add(new StringContent(model.KeyTakeaways ?? string.Empty), nameof(model.KeyTakeaways));
            formData.Add(new StringContent(model.PublishedOn?.ToString("o") ?? string.Empty), nameof(model.PublishedOn));

            if (model.ImageUrl != null && model.ImageUrl.Length > 0)
            {
                var imageContent = new StreamContent(model.ImageUrl.OpenReadStream());
                imageContent.Headers.ContentType = new MediaTypeHeaderValue(model.ImageUrl.ContentType);
                formData.Add(imageContent, nameof(model.ImageUrl), model.ImageUrl.FileName);
            }

            await _apiService.PutAsync<ApiResponse<object>>($"api/LatestThinking/{id}", formData);

            response = new[] { "True", "Updated successfully." };
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/LatestThinking/{id}");
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
