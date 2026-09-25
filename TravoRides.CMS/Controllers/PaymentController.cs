using Microsoft.AspNetCore.Mvc;
using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Payment;
using TravoRides.CMS.Interface;

namespace TravoRides.CMS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IApiService _apiService;

        public PaymentController(IApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            int? pageNumber,
            int? pageSize,
            string? keyword,
            string? dateFilter,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var queryParams = new List<string>
            {
                $"pageNumber={page}",
                $"pageSize={size}"
            };

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                queryParams.Add($"keyword={Uri.EscapeDataString(keyword.Trim())}");
            }

            if (!string.IsNullOrWhiteSpace(dateFilter))
            {
                queryParams.Add($"dateFilter={Uri.EscapeDataString(dateFilter.Trim())}");
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryParams.Add($"status={Uri.EscapeDataString(status.Trim())}");
            }

            if (fromDate.HasValue)
            {
                queryParams.Add($"fromDate={fromDate.Value:yyyy-MM-dd}");
            }

            if (toDate.HasValue)
            {
                queryParams.Add($"toDate={toDate.Value:yyyy-MM-dd}");
            }

            var url = $"api/payments?{string.Join("&", queryParams)}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<PaymentDTO>>>(url);

            ViewBag.Keyword = keyword;
            ViewBag.DateFilter = dateFilter ?? "";
            ViewBag.Status = status ?? "";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<PaymentDTO>>($"api/payments/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }
    }
}

