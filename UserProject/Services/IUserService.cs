using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Request;

namespace UserProject.Services
{
    public interface IUserService
    {
//        public UserRequest? Authenticate(string username, string password);
        public IEnumerable<List<UserRequest>> GetAll();
        public UserRequest GetById(long id);
        public void Insert(UserRequest req);
        public void Update(UserRequest req);
        public void Delete(long id);
    }
}
