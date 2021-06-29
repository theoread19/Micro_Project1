using Domain.Models;
using Domain.Repository;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private UserDBContext Context;
        public UserRepository()
        {
            Context = new UserDBContext();
        }
        public void Delete(long Id)
        {
            var std = Context.UserTable.Find(Id);
            Context.Remove(std);
        }

        public List<UserModel> GetAll()
        {
            var model = Context.UserTable;
            return model.ToList();
        }

        public UserModel GetById(long Id)
        {
            return Context.UserTable.Find(Id);
        }

        public void Insert(UserModel userModel)
        {
            Context.UserTable.Add(userModel);
        }

        public void Update(UserModel userModel)
        {
            var std = Context.UserTable.Find(userModel.Id);
            Context.UserTable.Update(userModel);
        }

        public void SaveChange()
        {
            Context.SaveChanges();
        }
    }
}
