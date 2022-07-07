using System;
using System.Collections.Generic;
using System.Linq;
using Streamish.Models;
using Streamish.Repositories;

namespace Streamish.Tests.Mocks
{
    class InMemoryUserRepository : IUserProfileRepository
    {
        private readonly List<UserProfile> _data;

        public List<UserProfile> InternalData
        {
            get
            {
                return _data;
            }
        }

        public InMemoryUserRepository(List<UserProfile> startingData)
        {
            _data = startingData;
        }

        public void Add(UserProfile userProfile)
        {
            var lastUser = _data.Last();
            userProfile.Id = lastUser.Id + 1;
            _data.Add(userProfile);
        }

        public void Delete(int id)
        {
            var userToDelete = _data.FirstOrDefault(p => p.Id == id);
            if (userToDelete == null)
            {
                return;
            }

            _data.Remove(userToDelete);
        }

        public List<UserProfile> GetAll()
        {
            return _data;
        }

        public UserProfile GetById(int id)
        {
            return _data.FirstOrDefault(p => p.Id == id);
        }

        public void Update(UserProfile userProfile)
        {
            var currentUser = _data.FirstOrDefault(p => p.Id == userProfile.Id);
            if (currentUser == null)
            {
                return;
            }
            currentUser.Name = userProfile.Name;
            currentUser.Email = userProfile.Email;
            currentUser.ImageUrl = userProfile.ImageUrl;
            currentUser.DateCreated = userProfile.DateCreated;
        }

        public UserProfile GetUserByIdWithVideos(int id)
        {
            throw new NotImplementedException();
        }
    }
}