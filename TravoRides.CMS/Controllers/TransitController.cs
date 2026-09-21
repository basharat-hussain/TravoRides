using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Transit;
using TravoRides.Application.DTOs.TransitRate;

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
        // =========================================================
        // EDIT Transit
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            // 1. Get Transit
            var TransitResponse =
                await _apiService.GetAsync<ApiResponse<TransitDTO>>(
                    $"api/Transit/{id}");

            var Transit = TransitResponse?.Data;

            if (Transit == null)
                return NotFound();

            // 2. Get ALL cabs from Cab API
            var cabResponse =
                await _apiService.GetAsync<ApiResponse<PagedResponse<CabDTO>>>(
                    "api/Cab?pageNumber=1&pageSize=100");

            var allCabs = cabResponse?.Data?.Items ?? new List<CabDTO>();

            // 3. Get cabs already added to this Transit
            var TransitCabsResponse =
                await _apiService.GetAsync<ApiResponse<List<TransitCabRateDTO>>>(
                    $"api/Transit/{id}/cabs");

            var TransitCabs = TransitCabsResponse?.Data
                ?? new List<TransitCabRateDTO>();

          


            // 6. Build MasterUpdate
            var model = new MasterUpdate
            {
                // Section 1 - Transit details
                UpdateTransit = new UpdateTransitRequest
                {
                    Id = Transit.Id,
                    Title = Transit.Title,
                    Price = Transit.Price,
                    Discount = Transit.Discount,
                    Description = Transit.Description,
                    ImageUrl = Transit.ImageUrl
                },

                // Section 2 - Add new cab
                TransitCabRequest = new TransitCabRequest(),

                // Available cabs for dropdown
                AvailableCabs = (List<CabDTO>)allCabs,

                // Section 3 - Already added cabs
                TransitCabRates = TransitCabs,

                // Section 4 - Edit existing Transit cab
                UpdateTransitCab = new UpdateTransitCabRequest()
            };

            return View(model);
        }

        // =========================================================
        // UPDATE Transit
        // =========================================================

        
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, MasterUpdate model)
        {
            if (model?.UpdateTransit == null)
            {
                return Json(new[]
                {
            "False",
            "Transit data is required."
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

            var Transit = model.UpdateTransit;

            using var formData = new MultipartFormDataContent();

            // Transit fields
            formData.Add(
                new StringContent(Transit.Title ?? string.Empty),
                nameof(Transit.Title));

            formData.Add(
                new StringContent(Transit.Price.ToString()),
                nameof(Transit.Price));

            formData.Add(
                new StringContent(Transit.Discount.ToString() ?? string.Empty),
                nameof(Transit.Discount));

            formData.Add(
               new StringContent(Transit.Description.ToString() ?? string.Empty),
               nameof(Transit.Description));
            // Image
            if (Transit.Image != null && Transit.Image.Length > 0)
            {
                var imageContent = new StreamContent(
                    Transit.Image.OpenReadStream());

                imageContent.Headers.ContentType =
                    new MediaTypeHeaderValue(Transit.Image.ContentType);

                formData.Add(
                    imageContent,
                    nameof(Transit.Image),
                    Transit.Image.FileName);
            }

            // Update Transit
            await _apiService.PutAsync<ApiResponse<Guid>>(
                $"api/Transit/{id}",
                formData);

            return Json(new[]
            {
        "True",
        "Updated successfully."
    });
        }


        // =========================================================
        // DELETE Transit
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success =
                    await _apiService.DeleteAsync(
                        $"api/Transit/{id}");

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
        // Transit + CAB
        // =========================================================


        // ---------------------------------------------------------
        // GET AVAILABLE CABS
        // API:
        // GET /api/Transit/{TransitId}/available-cabs
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetAvailableCabs(
            Guid TransitId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<List<CabDTO>>>(
                    $"api/Transit/{TransitId}/availableCabs");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }


        // ---------------------------------------------------------
        // GET ALL CABS ASSIGNED TO Transit
        // API:
        // GET /api/Transit/{TransitId}/cabs
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetTransitCabs(
            Guid TransitId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<List<TransitCabRateDTO>>>(
                    $"api/Transit/{TransitId}/cabs");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }


        // ---------------------------------------------------------
        // GET RATE OF ONE CAB FOR Transit
        // API:
        // GET /api/Transit/{TransitId}/rates/{cabId}
        // ---------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> GetTransitCabRate(
            Guid TransitId,
            Guid cabId)
        {
            var response =
                await _apiService.GetAsync<
                    ApiResponse<TransitCabRateDTO>>(
                    $"api/Transit/{TransitId}/rates/{cabId}");

            if (response?.Data == null)
                return NotFound();

            return Json(response.Data);
        }

        // =========================================================
        // API:
        // POST /api/Transit/{TransitId}/cabs
        // ADD ONE CAB TO Transit
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> AddCab(Guid TransitId, TransitCabRequest model)
        {
            try
            {
                if (model == null || model.CabId == Guid.Empty)
                {
                    return Json(new[]
                    {
                        "False",
                        "A valid cab must be selected."
                    });
                }
               
                var response = await _apiService.PostAsync<TransitCabRequest, ApiResponse<object>>(
                        $"api/Transit/{TransitId}/cabs", model);

                if (response == null || !response.IsSuccess)
                {
                    return Json(new[]
                    {
                        "False", response?.Message ?? "Failed to add cab."
                    });
                }

                return Json(new[]
                {
                    "True", "Cab added to Transit successfully."
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
        // PUT /api/Transit/{TransitId}/cabs/{cabId}
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> UpdateTransitCab(Guid TransitId, Guid cabId, UpdateTransitCabRequest model)
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
                    await _apiService.PutAsync<UpdateTransitCabRequest,ApiResponse<object>>($"api/Transit/{TransitId}/cabs/{cabId}",
                        model);


                if (apiResponse == null ||
                    !apiResponse.IsSuccess)
                {
                    response = new[]
                    {
                    "False",
                    apiResponse?.Message ??
                    "Failed to update Transit cab."
                };

                    return Json(response);
                }


                response = new[]
                {
                "True",
                "Transit cab updated successfully."
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
        // REMOVE CAB FROM Transit
        // API:
        // DELETE /api/Transit/{TransitId}/cabs/{cabId}
        // ---------------------------------------------------------

        [HttpPost]
        public async Task<IActionResult> RemoveCabFromTransit(Guid transitId, Guid cabId)
        {
           
            try
            {
                var success = await _apiService.DeleteAsync($"api/Transit/{transitId}/cabs/{cabId}");

                if (!success)
                {
                    return Json(new { isSuccess = false, message = "Failed to remove cab from Transit." });
                }

                return Json(new { isSuccess = true, message = "Cab removed from Transit successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

    }
}
