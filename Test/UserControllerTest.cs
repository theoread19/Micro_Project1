using Domain.Logging;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using UserProject.Controllers;
using UserProject.DTOs.Request;
using UserProject.Services;

namespace Test
{
    [TestClass]
    public class UserControllerTest
    {
        private UserController _userController;
        private Mock<IUserService> _userService;
        private Mock<ILoggerManager> _logger;

        public UserControllerTest()
        {
            this._userService = new Mock<IUserService>();
            this._logger = new Mock<ILoggerManager>();
            this._userController = new UserController(this._userService.Object, this._logger.Object);
        }

        [TestMethod]
        public void Get_All_User_Test()
        {
            var result = this._userController.GetAllUser();

            this._userService.Verify(m => m.GetAll());
            result.Should().BeEquivalentTo(new List<UserRequest>());                        
        }

        [TestMethod]
        [DataRow(1)]
        public void Get_User_By_Id(long id)
        {
            this._userService.Setup(m => m.GetById(id)).Returns(new UserRequest());

            var result = this._userController.GetUserById(id);

            this._userService.Verify(m => m.GetById(id)); 
            result.Should().BeEquivalentTo(new UserRequest());
        }

        [TestMethod]
        public void Create_User_Test()
        {
            var model = new UserRequest();

            this._userController.CreateUser(model);

            this._userService.Verify(m => m.Insert(model));
        }

        [TestMethod]
        public void Update_User_Test()
        {
            var model = new UserRequest();

            this._userController.UpdateUser(model);

            this._userService.Verify(m => m.Update(model));
        }

        [TestMethod]
        [DataRow(1)]
        public void Delete_User_Test(long id)
        {
            this._userController.DeleteUser(id);

            this._userService.Verify(m => m.Delete(id));
        }
    }
}
