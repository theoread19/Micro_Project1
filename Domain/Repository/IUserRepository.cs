using Domain.Models;
using ProjectCore.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Repository
{
    public interface IUserRepository : IRepository<UserModel>
    {
        /*        public List<UserModel> GetAll();
                public UserModel GetById(long Id);
                */
        public UserModel Insert(UserModel userModel);
        public UserModel? GetByUsernameAndPassword(string username, string password);
/*        public void Update(UserModel userModel);
        public void Delete(long id);
        public void SaveChange();*/
    }
}
