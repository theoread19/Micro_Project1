using Domain.Repository;
using Infrastructure.Kafka.Producer;
using Infrastructure.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Converter;
using UserProject.DTOs.Request;

namespace UserProject.Services.iplm
{
    public class UserService : IUserService
    {
        private ProducerConfigure configure = new ProducerConfigure("test1");
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
            var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize<UserProtobuf>(stream, converter.toProto(req));
            byte[] data =  stream.ToArray();
            configure.SendToKafka(data);
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
