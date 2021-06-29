using Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Converter;
using UserProject.DTOs.Request;

namespace UserProject.Services.iplm
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;
        private UserConverter converter = new UserConverter();
        public UserService(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }
        public void Delete(long id)
        {
            this._userRepository.Delete(id);
            this._userRepository.SaveChange();
        }

        public IEnumerable<List<UserRequest>> GetAll()
        {
            var model = this._userRepository.GetAll();
            List<UserRequest> reqs = new List<UserRequest>();
            foreach(var item in model)
            {
                UserRequest req = new UserRequest();
                req = converter.toReq(item);
                reqs.Add(req);
            }

            yield return reqs;
        }

        public UserRequest GetById(long id)
        {
            return converter.toReq(this._userRepository.GetById(id));
        }

        public void Insert(UserRequest req)
        {
            this._userRepository.Insert(converter.toModel(req));
            this._userRepository.SaveChange();
        }

        public void Update(UserRequest req)
        {
            var model = this._userRepository.GetById(req.Id);
            converter.toModel(req, ref model);
            this._userRepository.Update(model);
            this._userRepository.SaveChange();
        }
    }
}
