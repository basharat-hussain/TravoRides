using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Package;
using TravoRides.Application.DTOs.PackageRate;
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

        // =========================================================
        // PACKAGE CRUD
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Package?pageNumber={page}&pageSize={size}";

            var items =
                await _apiService.GetAllAsync<
                    ApiResponse<PagedResponse<PackageDTO>>>(url);

            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response =
                await _apiService.GetAsync<ApiResponse<PackageDTO>>(
                    $"api/Package/{id}");

            var item = response?.Data;

            if (item == null)
                return NotFound();

            return View(item);
        }


        // =========================================================
        // CREATE PACKAGE
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePackageRequest());
        }


        [HttpPost]
        public async Task<IActionResult> Create(
            CreatePackageRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[]
                {
                "False",
                "Validation Failed"
            };

                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(
                new StringContent(model.Title ?? string.Empty),
                nameof(model.Title));

            formData.Add(
                new StringContent(model.Price.ToString()),
                nameof(model.Price));

            formData.Add(
                new StringContent(model.Discount.ToString()),
                nameof(model.Discount));

            formData.Add(
                new StringContent(model.Itinerary ?? string.Empty),
                nameof(model.Itinerary));

            formData.Add(
                new StringContent(model.Inclusions ?? string.Empty),
                nameof(model.Inclusions));

            formData.Add(
                new StringContent(model.Distance.ToString()),
                nameof(model.Distance));

            formData.Add(
                new StringContent(model.Route ?? string.Empty),
                nameof(model.Route));

            formData.Add(
                new StringContent(model.PlacesCovered ?? string.Empty),
                nameof(model.PlacesCovered));

            formData.Add(
                new StringContent(model.Duration ?? string.Empty),
                nameof(model.Duration));


            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent =
                    new StreamContent(model.Image.OpenReadStream());

                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue(model.Image.ContentType);

                formData.Add(
                    imageContent,
                    nameof(model.Image),
                    model.Image.FileName);
            }


            await _apiService.PostAsync<ApiResponse<Guid>>(
                "api/Package",
                formData);

            response = new[]
            {
            "True",
            "Created successfully."
        };

            return Json(response);
        }


        // =========================================================
        // EDIT PACKAGE
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response =
                await _apiService.GetAsync<ApiResponse<PackageDTO>>(
                    $"api/Package/{id}");

            var item = response?.Data;

            if (item == null)
                return NotFound();

            var model = new UpdatePackageRequest
            {
                Id = item.Id,
                Title = item.Title,
                Price = item.Price,
                Discount = item.Discount,
                Distance = item.Distance,
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
        public async Task<IActionResult> Edit(
            Guid id,
            UpdatePackageRequest model)
        {
            var response = new string[] { };

            if (!ModelState.IsValid)
            {
                response = new[]
                {
                "False",
                "Validation Failed"
            };

                return Json(response);
            }

            using var formData = new MultipartFormDataContent();

            formData.Add(
                new StringContent(model.Title ?? string.Empty),
                nameof(model.Title));

            formData.Add(
                new StringContent(model.Price.ToString()),
                nameof(model.Price));

            formData.Add(
                new StringContent(model.Discount.ToString()),
                nameof(model.Discount));

            formData.Add(
                new StringContent(model.Itinerary ?? string.Empty),
                nameof(model.Itinerary));

            formData.Add(
                new StringContent(model.Inclusions ?? string.Empty),
                nameof(model.Inclusions));

            formData.Add(
                new StringContent(model.Distance.ToString()),
                nameof(model.Distance));

            formData.Add(
                new StringContent(model.Route ?? string.Empty),
                nameof(model.Route));

            formData.Add(
                new StringContent(model.PlacesCovered ?? string.Empty),
                nameof(model.PlacesCovered));

            formData.Add(
                new StringContent(model.Duration ?? string.Empty),
                nameof(model.Duration));


            if (model.Image != null && model.Image.Length > 0)
            {
                var imageContent =
                    new StreamContent(model.Image.OpenReadStream());

                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue(model.Image.ContentType);

                formData.Add(
                    imageContent,
                    nameof(model.Image),
                    model.Image.FileName);
            }


            await _apiService.PutAsync<ApiResponse<Guid>>(
                $"api/Package/{id}",
                formData);

            response = new[]
            {
            "True",
            "Updated successfully."
        };

            return Json(response);
        }


        // =========================================================
        // DELETE PACKAGE
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success =
                    await _apiService.DeleteAsync(
                        $"api/Package/{id}");

                if (!success)
                {
                    response = new[]
                    {
                    "False",
                    "Deletion failed"
                };

                    return Json(response);
                }

                response = new[]
                {
                "True",
                "Deleted successfully"
            };

                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[]
                {
                "False",
                ex.Message
            };

                return Json(response);
            }
        }


        // =========================================================
        // PACKAGE + CAB
        // =========================================================

        // ---------------------------------------------------------
        // GET ALL CABS ASSIGNED TO PACKAGE
        // API:
        // GET /api/Package/{packageId}/cabs
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetPackageCabs(
            Guid packageId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<List<PackageCabRateDTO>>>(
                    $"api/Package/{packageId}/cabs");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }


        // ---------------------------------------------------------
        // GET RATE OF ONE CAB FOR PACKAGE
        // API:
        // GET /api/Package/{packageId}/rates/{cabId}
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetPackageCabRate(
            Guid packageId,
            Guid cabId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<PackageCabRateDTO>>(
                    $"api/Package/{packageId}/rates/{cabId}");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }


        // ---------------------------------------------------------
        // ADD MULTIPLE CABS TO PACKAGE
        // API:
        // POST /api/Package/{packageId}/cabs
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> AddCabsToPackage(
            Guid packageId,
            AddCabsToPackageRequest model)
        {
            var response = new string[] { };

            try
            {
                if (model == null ||
                    model.Cabs == null ||
                    !model.Cabs.Any())
                {
                    response = new[]
                    {
                    "False",
                    "At least one cab must be selected."
                };

                    return Json(response);
                }

                var apiResponse =
                    await _apiService.PostAsync<
                        ApiResponse<object>>(
                        $"api/Package/{packageId}/cabs",
                        model);

                if (apiResponse == null || !apiResponse.IsSuccess)
                {
                    response = new[]
                    {
                    "False",
                    apiResponse?.Message ??
                    "Failed to add cabs."
                };

                    return Json(response);
                }

                response = new[]
                {
                "True",
                "Cabs added to package successfully."
            };

                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[]
                {
                "False",
                ex.Message
            };

                return Json(response);
            }
        }


        // ---------------------------------------------------------
        // UPDATE CAB RATE/DISCOUNT FOR PACKAGE
        // API:
        // PUT /api/Package/{packageId}/cabs/{cabId}
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> UpdatePackageCab(
            Guid packageId,
            Guid cabId,
            UpdatePackageCabRequest model)
        {
            var response = new string[] { };

            try
            {
                if (!ModelState.IsValid)
                {
                    response = new[]
                    {
                    "False",
                    "Validation Failed"
                };

                    return Json(response);
                }

                var apiResponse =
                    await _apiService.PutAsync<
                        ApiResponse<object>>(
                        $"api/Package/{packageId}/cabs/{cabId}",
                        model);

                if (apiResponse == null || !apiResponse.IsSuccess)
                {
                    response = new[]
                    {
                    "False",
                    apiResponse?.Message ??
                    "Failed to update package cab."
                };

                    return Json(response);
                }

                response = new[]
                {
                "True",
                "Package cab updated successfully."
            };

                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[]
                {
                "False",
                ex.Message
            };

                return Json(response);
            }
        }


        // ---------------------------------------------------------
        // REMOVE CAB FROM PACKAGE
        // API:
        // DELETE /api/Package/{packageId}/cabs/{cabId}
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> RemoveCabFromPackage(
            Guid packageId,
            Guid cabId)
        {
            var response = new string[] { };

            try
            {
                var success =
                    await _apiService.DeleteAsync(
                        $"api/Package/{packageId}/cabs/{cabId}");

                if (!success)
                {
                    response = new[]
                    {
                    "False",
                    "Failed to remove cab from package."
                };

                    return Json(response);
                }

                response = new[]
                {
                "True",
                "Cab removed from package successfully."
            };

                return Json(response);
            }
            catch (Exception ex)
            {
                response = new[]
                {
                "False",
                ex.Message
            };

                return Json(response);
            }
        }
    }

}
