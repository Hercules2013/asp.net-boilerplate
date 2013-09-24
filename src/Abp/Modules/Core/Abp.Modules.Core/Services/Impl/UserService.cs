using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Authorization;
using Abp.Exceptions;
using Abp.Modules.Core.Data.Repositories;
using Abp.Modules.Core.Entities;
using Abp.Modules.Core.Services.Dto;

namespace Abp.Modules.Core.Services.Impl
{
    /// <summary>
    /// Implementation of IUserService interface.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository questionRepository)
        {
            _userRepository = questionRepository;
        }

        public IList<UserDto> GetAllUsers()
        {
            return _userRepository.Query(q => q.ToList()).MapIList<User, UserDto>();
        }

        public UserDto GetUserOrNull(string emailAddress, string password)
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.EmailAddress == emailAddress && user.Password == password));
            return userEntity.MapTo<UserDto>();
        }

        public UserDto GetUser(int userId)
        {
            var userEntity = _userRepository.Query(q => q.FirstOrDefault(user => user.Id == userId));
            if (userEntity == null)
            {
                throw new ApplicationException("Can not find user with id = " + userId);
            }

            return userId.MapTo<UserDto>();
        }
    }
}