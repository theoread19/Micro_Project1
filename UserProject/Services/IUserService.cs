using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Request;

namespace UserProject.Services
{
    public interface IUserService
    {
//        public UserRequest? Authenticate(string username, string password);
        public Task<List<UserRequest>> GetAllAsync();
        public UserRequest GetById(long id);
        public Task InsertByExcel(IFormFile file);
        public MemoryStream ExportDBToExcel(string nameFile);
        public void Insert(UserRequest req);
        public void Update(UserRequest req);
        public void Delete(long id);
    }
}
