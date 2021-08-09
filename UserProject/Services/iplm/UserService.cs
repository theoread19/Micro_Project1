using Domain.Repository;
using ExcelDataReader;
using Infrastructure.Kafka.Producer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserProject.DTOs.Converter;
using UserProject.DTOs.Request;
using UserProtoBufService;

namespace UserProject.Services.iplm
{
    public class UserService : IUserService
    {
        private ProducerConfigure _configure = new ProducerConfigure("message");
        private IUserRepository _userRepository;
        private IDistributedCache _distributedCache;
        private readonly UserConverter _converter = new UserConverter();
        public UserService(IUserRepository userRepository, IDistributedCache distributedCache)
        {
            this._distributedCache = distributedCache;
            this._userRepository = userRepository;
        }
        public void Delete(long id)
        {
            this._userRepository.Delete(id);
            this._userRepository.Save();
        }

        public async Task<List<UserRequest>> GetAllAsync()
        {
            var cacheKey = "userList";
            string serializedList;
            List<UserRequest> reqs = new List<UserRequest>();
            var redisList = await _distributedCache.GetAsync(cacheKey);
            if (redisList != null)
            {
                serializedList = Encoding.UTF8.GetString(redisList);
                reqs = JsonConvert.DeserializeObject<List<UserRequest>>(serializedList)!;
            }
            else
            {
                var model = this._userRepository.GetAll();
                //List<UserRequest> reqs = new List<UserRequest>();
                foreach (var item in model)
                {
                    UserRequest req = new UserRequest();
                    req = _converter.toReq(item);
                    reqs.Add(req);
                }
                serializedList = JsonConvert.SerializeObject(reqs);
                redisList = Encoding.UTF8.GetBytes(serializedList);
                var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
                await _distributedCache.SetAsync(cacheKey, redisList, options);
            }

            return reqs;
            /*            var model = this._userRepository.GetAll();
                        List<UserRequest> reqs = new List<UserRequest>();
                        foreach(var item in model)
                        {
                            UserRequest req = new UserRequest();
                            req = _converter.toReq(item);
                            reqs.Add(req);
                        }

                        yield return reqs;*/
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
                this._userRepository.Save();
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
                this._userRepository.Save();
            }
        }

        public async Task InsertByExcel(IFormFile file)
        {
            //check file not null
            if (file == null || file.Length == 0)
                throw new Exception("File Not Selected");

            //check if file is excel or not
            string fileExtension = Path.GetExtension(file.FileName);
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
                throw new Exception("File Not Compatible");

            var rootFolder = "Upload\\files\\test";

            //create Directory if not exist
            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), rootFolder);
            if (!Directory.Exists(rootFolder))
            {
                Directory.CreateDirectory(rootFolder);
            }

            var fileName = file.FileName;
            var filePath = Path.Combine(rootFolder, fileName);
            var fileLocation = new FileInfo(filePath);
            //upload file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            using (ExcelPackage package = new ExcelPackage(fileLocation))
            {
                //LicenseContext parameter must be set
                //For more details: https://epplussoftware.com/docs/5.1/articles/readme.html
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                //get first sheet
                //or Worksheets["sheet1"]
                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                int totalRows = workSheet.Dimension.Rows;

                var DataList = new List<UserRequest>();

                for (int i = 1; i <= totalRows; i++)
                {
                    var req = new UserRequest
                    {
                        Fullname = workSheet.Cells[i, 1].Value.ToString(),
                        Email = workSheet.Cells[i, 2].Value.ToString(),
                        Username = workSheet.Cells[i, 3].Value.ToString(),
                        Password = workSheet.Cells[i, 4].Value.ToString(),
                        Role = workSheet.Cells[i, 5].Value.ToString()
                    };
                    this.Insert(req);
                }
            }
            await Task.CompletedTask;
        }

        public MemoryStream ExportDBToExcel(string nameFile)
        {
            var table = this._userRepository.GetAll();
            var stream = new MemoryStream();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {


                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                workSheet.Cells.LoadFromCollection(table, true);
                package.Save();
            }
            stream.Position = 0;
            return stream;
        }
    }
}
