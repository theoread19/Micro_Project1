using Domain.Repository;
using Infrastructure.Kafka.Producer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserProject.DTOs.Converter;
using UserProject.DTOs.Request;
using UserProtoBufService;

namespace UserProject.Services.iplm
{
    public class UserService : IUserService
    {
        private ProducerConfigure _configure = new ProducerConfigure("message");
        private IUserRepository _userRepository;
        private readonly UserConverter _converter = new UserConverter();
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
                req = _converter.toReq(item);
                reqs.Add(req);
            }

            yield return reqs;
        }

        public UserRequest GetById(long id)
        {
            return _converter.toReq(this._userRepository.GetById(id));
        }

        public void Insert(UserRequest req)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }
            else
            {
                var model = this._userRepository.Insert(_converter.toModel(req));
                var data = new UserProtoReq
                {
                    UserId = model.Id,
                    Email = model.Email,
                    Fullname = model.Fullname
                };

                _configure.SendToKafka(data, "insert");
                this._userRepository.SaveChange();
            }
        }

        public void Update(UserRequest req)
        {
            if (req is null)
            {
                throw new ArgumentNullException(nameof(req));
            }
            else
            {
                var model = this._userRepository.GetById(req.Id);
                _converter.toModel(req, ref model);
                this._userRepository.Update(model);
                this._userRepository.SaveChange();
            }
        }

/*        public UserRequest? Authenticate(string username, string password)
        {
            var model = this._userRepository.GetByUsernameAndPassword(username, password);
            if (model != null) { 
                var user = this._converter.toReq(model);
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("secret");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                user.Token = tokenHandler.WriteToken(token);
                return user;                
            }
            // return null if user not found
            return null;
        }*/
    }
}
