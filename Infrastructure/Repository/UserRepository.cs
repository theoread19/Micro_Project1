using Domain.Models;
using Domain.Repository;
using Infrastructure.Data;
using ProjectCore.Repository.Impl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repository
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository 
    {
        private UserDBContext _context;
        public UserRepository() : base(new UserDBContext())
        {
            
            this._context = new UserDBContext();
        }

        public UserModel Insert(UserModel userModel)
        {
            this._context.UserTable.Add(userModel);
            this._context.SaveChanges();
            return userModel;
        }

        public UserModel? GetByUsernameAndPassword(string username, string password)
        {
            return this._context.UserTable.Where(m => m.Username == username && m.Password == password).FirstOrDefault();
        }
    }
}
