using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Category;
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
        public async Task<IActionResult> Index(
            int? pageNumber,
            int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url =
                $"api/Package?pageNumber={page}&pageSize={size}";

            var items =
                await _apiService.GetAllAsync<
                    ApiResponse<PagedResponse<PackageDTO>>>(url);

            return View(items);
        }


        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<PackageDTO>>(
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

            using var formData =
                new MultipartFormDataContent();

            formData.Add(
                new StringContent(
                    model.Title ?? string.Empty),
                nameof(model.Title));

            formData.Add(
                new StringContent(
                    model.Price.ToString()),
                nameof(model.Price));

            formData.Add(
                new StringContent(
                    model.Discount.ToString() ?? string.Empty),
                nameof(model.Discount));

            formData.Add(
                new StringContent(
                    model.Itinerary ?? string.Empty),
                nameof(model.Itinerary));

            formData.Add(
                new StringContent(
                    model.Inclusions ?? string.Empty),
                nameof(model.Inclusions));

            formData.Add(
                new StringContent(
                    model.Distance.ToString()),
                nameof(model.Distance));

            formData.Add(
                new StringContent(
                    model.Route ?? string.Empty),
                nameof(model.Route));

            formData.Add(
                new StringContent(
                    model.PlacesCovered ?? string.Empty),
                nameof(model.PlacesCovered));

            formData.Add(
                new StringContent(
                    model.Duration ?? string.Empty),
                nameof(model.Duration));


            if (model.Image != null &&
                model.Image.Length > 0)
            {
                var imageContent =
                    new StreamContent(
                        model.Image.OpenReadStream());

                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue(
                        model.Image.ContentType);

                formData.Add(
                    imageContent,
                    nameof(model.Image),
                    model.Image.FileName);
            }


            await _apiService.PostAsync<ApiResponse<object>>(
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
            // 1. Get package
            var packageResponse =
                await _apiService.GetAsync<ApiResponse<PackageDTO>>(
                    $"api/Package/{id}");

            var package = packageResponse?.Data;

            if (package == null)
                return NotFound();


            // 2. Get ALL cabs from Cab API
            var cabResponse =
                await _apiService.GetAsync<ApiResponse<PagedResponse<CabDTO>>>(
                    "api/Cab?pageNumber=1&pageSize=100");

            var allCabs =
                cabResponse?.Data?.Items
                ?? new List<CabDTO>();


            // 3. Get cabs already added to this package
            var packageCabsResponse =
                await _apiService.GetAsync<ApiResponse<List<PackageCabRateDTO>>>(
                    $"api/Package/{id}/cabs");

            var packageCabs =
                packageCabsResponse?.Data
                ?? new List<PackageCabRateDTO>();


            // 4. Build MasterUpdate
            var model = new MasterUpdate
            {
                // =====================================================
                // SECTION 1 - PACKAGE DETAILS
                // =====================================================

                UpdatePackage = new UpdatePackageRequest
                {
                    Id = package.Id,
                    Title = package.Title,
                    Price = package.Price,
                    Discount = package.Discount,
                    Itinerary = package.Itinerary,
                    Inclusions = package.Inclusions,
                    Distance = package.Distance,
                    Route = package.Route,
                    PlacesCovered = package.PlacesCovered,
                    Duration = package.Duration,
                    ImageUrl = package.ImageUrl
                },


                // =====================================================
                // SECTION 2 - ADD / UPDATE CAB
                // =====================================================

                PackageCabRequest = new PackageCabRequest(),

                // IMPORTANT:
                // Keep ALL cabs here so the dropdown has options.
                // This also allows an existing cab to be selected
                // when editing it.
                AvailableCabs = (List<CabDTO>)allCabs,


                // =====================================================
                // SECTION 3 - ALREADY ADDED CABS
                // =====================================================

                PackageCabRates = packageCabs,


                // =====================================================
                // SECTION 4 - UPDATE EXISTING PACKAGE CAB
                // =====================================================

                UpdatePackageCab = new UpdatePackageCabRequest()
            };

            return View(model);
        }

        // =========================================================
        // UPDATE PACKAGE
        // =========================================================


        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, MasterUpdate model)
        {
            if (model?.UpdatePackage == null)
            {
                return Json(new[]
                {
            "False",
            "Package data is required."
        });
            }

            if (!ModelState.IsValid)
            {
                return Json(new[]
                {
            "False",
            "Validation Failed"
        });
            }

            var package = model.UpdatePackage;

            using var formData = new MultipartFormDataContent();

            // Package fields
            formData.Add(
                new StringContent(package.Title ?? string.Empty),
                nameof(package.Title));

            formData.Add(
                new StringContent(package.Price.ToString()),
                nameof(package.Price));

            formData.Add(
                new StringContent(package.Discount.ToString() ?? string.Empty),
                nameof(package.Discount));

            formData.Add(
                new StringContent(package.Itinerary ?? string.Empty),
                nameof(package.Itinerary));

            formData.Add(
                new StringContent(package.Inclusions ?? string.Empty),
                nameof(package.Inclusions));

            formData.Add(
                new StringContent(package.Distance.ToString()),
                nameof(package.Distance));

            formData.Add(
                new StringContent(package.Route ?? string.Empty),
                nameof(package.Route));

            formData.Add(
                new StringContent(package.PlacesCovered ?? string.Empty),
                nameof(package.PlacesCovered));

            formData.Add(
                new StringContent(package.Duration ?? string.Empty),
                nameof(package.Duration));

            // Image
            if (package.Image != null && package.Image.Length > 0)
            {
                var imageContent = new StreamContent(
                    package.Image.OpenReadStream());

                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue(package.Image.ContentType);

                formData.Add(
                    imageContent,
                    nameof(package.Image),
                    package.Image.FileName);
            }

            // Update package
            await _apiService.PutAsync<ApiResponse<object>>(
                $"api/Package/{id}",
                formData);

            return Json(new[]
            {
        "True",
        "Updated successfully."
    });
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
        // GET AVAILABLE CABS
        // API:
        // GET /api/Package/{packageId}/available-cabs
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetAvailableCabs(
            Guid packageId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<List<CabDTO>>>(
                    $"api/Package/{packageId}/available-cabs");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }


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

        // =========================================================
        // API:
        // POST /api/Package/{packageId}/cabs
        // ADD ONE CAB TO PACKAGE
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> AddCab(  Guid packageId, PackageCabRequest model)
        {
            try
            {
                Console.WriteLine($"PackageId: {packageId}");
                Console.WriteLine($"Model: {model}");
                Console.WriteLine($"CabId: {model?.CabId}");
                if (model == null || model.CabId == Guid.Empty)
                {
                    return Json(new[]
                    {
                        "False",
                        "A valid cab must be selected."
                    });
                }

                var response = await _apiService.PostAsync<PackageCabRequest,ApiResponse<object>>(
                        $"api/Package/{packageId}/cabs", model);

                if (response == null || !response.IsSuccess)
                {
                    return Json(new[]
                    {
                        "False", response?.Message ?? "Failed to add cab."
                    });
                }

                return Json(new[]
                {
                    "True", "Cab added to package successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new[]
                {
                    "False", ex.Message
                });
            }
        }

        // ---------------------------------------------------------
        // UPDATE CAB RATE/DISCOUNT
        // API:
        // PUT /api/Package/{packageId}/cabs/{cabId}
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> UpdatePackageCab( Guid packageId, Guid cabId, UpdatePackageCabRequest model)
        {
            var response = new string[] { };

            try
            {
                if (!ModelState.IsValid)
                {
                    response = new[]
                    {
                    "False",  "Validation Failed"
                };

                    return Json(response);
                }


                var apiResponse =
                    await _apiService.PutAsync< UpdatePackageCabRequest,ApiResponse<object>>(  $"api/Package/{packageId}/cabs/{cabId}",
                        model);


                if (apiResponse == null ||
                    !apiResponse.IsSuccess)
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
        public async Task<IActionResult> RemoveCabFromPackage(Guid packageId, Guid cabId)
        {
            try
            {
                var success = await _apiService.DeleteAsync($"api/Package/{packageId}/cabs/{cabId}");

                if (!success)
                {
                    return Json(new { isSuccess = false, message = "Failed to remove cab from package." });
                }

                return Json(new { isSuccess = true, message = "Cab removed from package successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
