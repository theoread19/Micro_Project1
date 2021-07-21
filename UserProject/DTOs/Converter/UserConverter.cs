using Domain.Models;
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
            req.Username = model.Username;
            req.Role = model.Role;
            return req;
        }

        public UserModel toModel(UserRequest req)
        {
            UserModel model = new UserModel();
            model.Fullname = req.Fullname;
            model.Email = req.Email;
            model.Username = req.Username;
            model.Password = req.Password;
            model.Role = req.Role;
            return model;
        }

        public UserModel toModel(UserRequest req, ref UserModel model)
        {
            model.Id = req.Id;
            model.Fullname = req.Fullname;
            model.Email = req.Email;
            model.Role = req.Role;
            model.Role = req.Role;
            return model;
        }

    }
}
