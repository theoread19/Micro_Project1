using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repository
{
    public interface IUserRepository
    {
        public List<UserModel> GetAll();
        public UserModel GetById(long Id);
        public void Insert(UserModel userModel);
        public void Update(UserModel userModel);
        public void Delete(long id);
        public void SaveChange();
    }
}
