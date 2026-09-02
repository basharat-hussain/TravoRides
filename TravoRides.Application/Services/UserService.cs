using AutoMapper;
using TravoRiders.Application.Common.Exceptions;
using TravoRiders.Application.DTOs.Users;
using TravoRiders.Application.Interfaces.Services;
using TravoRides.Application.Interfaces;
using TravoRides.Application.Repositories;
using TravoRides.Domain.Entities;

namespace TravoRides.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserOnlyResponse> RegisterUserAsync(CreateUserRequst request)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
                throw new ConflictException("Email already exists.");
          

            var user = new User
            {
                Email = request.Email,
                 Role = request.Role,
                IsActive = true
            };
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserOnlyResponse>(user);
        }

        public async Task<UserProfileResponse> GetMyProfileAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ResourceNotFoundException("User not found.");
            }

            var userProfileResponse = _mapper.Map<UserProfileResponse>(user);

            return userProfileResponse;
        }
    }
}
