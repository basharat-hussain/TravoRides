using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.BookingDTO;
using TravoRides.Application.DTOs.Common;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class BookingController : Controller
    {
        private readonly IApiService _apiService;

        public BookingController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? pageNumber,
            int? pageSize,
            string? keyword,
            bool? isConfirmed,
            string? scope,
            string? quickFilter,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;
            var currentScope = string.IsNullOrWhiteSpace(scope) ? "new" : scope.Trim().ToLowerInvariant();

            var queryParams = new List<string>
            {
                $"pageNumber={page}",
                $"pageSize={size}",
                $"bookingScope={Uri.EscapeDataString(currentScope)}"
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryParams.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
            }

            if (isConfirmed.HasValue)
            {
                queryParams.Add($"isConfirmed={isConfirmed.Value}");
            }

            if (!string.IsNullOrWhiteSpace(quickFilter))
            {
                queryParams.Add($"quickFilter={Uri.EscapeDataString(quickFilter.Trim())}");
            }

            if (fromDate.HasValue)
            {
                queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            }

            if (toDate.HasValue)
            {
                queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");
            }

            var url = $"api/Booking?{string.Join("&", queryParams)}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<BookingDTO>>>(url);

            ViewBag.Keyword = keyword;
            ViewBag.IsConfirmed = isConfirmed;
            ViewBag.Scope = currentScope;
            ViewBag.QuickFilter = quickFilter?.Trim().ToLowerInvariant() ?? "";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<BookingDTO>>($"api/Booking/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost]
        public IActionResult Delete(Guid id)
        {
            return Json(new[] { "False", "Deleting bookings is not allowed." });
        }
    }
}

