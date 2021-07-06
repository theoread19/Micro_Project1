using Domain.Models;
using Infrastructure.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Request;

namespace UserProject.DTOs.Converter
{
    public class UserConverter
    {
        public UserRequest toReq(UserModel model)
        {
            UserRequest req = new UserRequest();
            req.Id = model.Id;
            req.Fullname = model.Fullname;
            req.Email = model.Email;
            return req;
        }

        public UserModel toModel(UserRequest req)
        {
            UserModel model = new UserModel();
            model.Fullname = req.Fullname;
            model.Email = req.Email;
            return model;
        }

        public UserModel toModel(UserRequest req, ref UserModel model)
        {
            model.Id = req.Id;
            model.Fullname = req.Fullname;
            model.Email = req.Email;
            return model;
        }

        public UserProtobuf toProto(UserRequest req)
        {
            var proto = new UserProtobuf();
            proto.Id = req.Id;
            proto.Fullname = req.Fullname;
            proto.Email = req.Email;
            return proto;
        }
    }
}
