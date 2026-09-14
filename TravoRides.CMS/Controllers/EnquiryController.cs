using TravoRides.Application.Common.Responses;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Enquirer;
using TravoRides.CMS.Interface;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TravoRides.CMS.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly IApiService _apiService;

        public EnquiryController(IApiService apiService)
        {
            _apiService = apiService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? pageNumber, int? pageSize)
        {
            var page = pageNumber ?? 1;
            var size = pageSize ?? 10;

            var url = $"api/Enquiry?pageNumber={page}&pageSize={size}";

            var items = await _apiService.GetAllAsync<ApiResponse<PagedResponse<EnquiryDTO>>>(url);
            return View(items);
        }
        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<EnquiryDTO>>($"api/Enquiry/{id}");
            var item = response?.Data;
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateEnquiryRequest());
        }
        [HttpPost]
        //public async Task<IActionResult> Create(EnquiryDTO model)
        //{
        //    var response = new string[] { };

        //    if (!ModelState.IsValid)
        //    {
        //        response = new[] { "False", "Validation Failed" };
        //        return Json(response);
        //    }

        //    await _apiService.PostAsync("api/Enquiry", model);

        //    response = new[] { "True", "Enquiry created successfully." };
        //    return Json(response);
        //}

      
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = new string[] { };

            try
            {
                var success = await _apiService.DeleteAsync($"api/Enquiry/{id}");
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
