using AutoMapper;
using TravoRides.Application.Common.Exceptions;
using TravoRides.Application.DTOs.Category;
using TravoRides.Application.DTOs.Common;
using TravoRides.Application.DTOs.Enquirer;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Interfaces.Services;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class EnquiryService : IEnquiryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;

        public EnquiryService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService, IEmailTemplateService emailTemplateService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
        }
        public async Task<PagedResponse<EnquiryDTO>> GetAllAsync(SearchEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            if (request.PageNumber < 1) request.PageNumber = 1;
            if (request.PageSize < 1) request.PageSize = 10;
            if (request.PageSize > 100) request.PageSize = 100;

            var pagedResponse = await _unitOfWork.Enquiries
               .GetAllSearchAsync(
                   request.PageNumber,
                   request.PageSize,
                   request.Keyword,
                   cancellationToken);

            var categoryDtos = _mapper.Map<IEnumerable<EnquiryDTO>>(
                pagedResponse.Items);


            return new PagedResponse<EnquiryDTO>
            {
                Items = categoryDtos,
                PageNumber = pagedResponse.PageNumber,
                PageSize = pagedResponse.PageSize,
                TotalCount = pagedResponse.TotalCount,
                TotalPages = pagedResponse.TotalPages
            };

          
        }

        public async Task<EnquiryDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var Enquiry = await _unitOfWork.Enquiries
                .GetByIdAsync(id, cancellationToken);

            if (Enquiry == null)
                return null;

            return _mapper.Map<EnquiryDTO>(Enquiry);
        }

        public async Task<Guid> CreateAsync(CreateEnquiryRequest request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException(" name is required.");

            var Enquiry = new Enquiry
            {
                Name = request.Name.Trim(),
                Phone = request.Phone.Trim(),
                Subject = request.Subject.Trim(),
                Email = request.Email.Trim(),
                Message = request.Message.Trim()
               
            };

            await _unitOfWork.Enquiries.AddAsync(Enquiry, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Send a confirmation email to the enquirer using template
             var subject = "Thank you for your enquiry";

                var body = await _emailTemplateService.GetEnquiryConfirmationTemplateAsync(
                    request.Name,
                    request.Subject,
                    request.Message,
                    request.Phone
                  
                    );

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    await _emailService.SendEmailAsync(request.Email.Trim(), subject, body, true, cancellationToken);
                }
            return Enquiry.Id;
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var enquiry = await _unitOfWork.Enquiries
                .GetByIdAsync(id, cancellationToken);

            if (enquiry == null)
                throw new ResourceNotFoundException("Enquiry not found.");

            enquiry.IsDeleted = true;
            enquiry.ModifiedAt = DateTime.UtcNow;
            enquiry.ModifiedBy = "System"; // You

            _unitOfWork.Enquiries.Update(enquiry);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
