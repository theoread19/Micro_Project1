using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using ProjectCore.Logging;
using UserProject.Controllers;
using UserProject.DTOs.Request;
using UserProject.Services;

namespace Test
{
    [TestClass]
    public class UserControllerTest
    {
        private readonly UserController _userController;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IMessageService> _messageService;
        private readonly Mock<ILogger> _logger;

        public UserControllerTest()
        {
            this._userService = new Mock<IUserService>();
            this._logger = new Mock<ILogger>();
            this._messageService = new Mock<IMessageService>();
            this._userController = new UserController(this._userService.Object, this._logger.Object, this._messageService.Object);
        }

        [TestMethod]
        public void Get_All_User_Test()
        {
            var result = this._userController.GetAllUserAsync();

            this._userService.Verify(m => m.GetAllAsync());
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


        [TestMethod]
        [DataRow(2)]
        public void Get_Message_By_SenderId_Test(long id)
        {
            this._messageService.Setup(m => m.GetMessageBySenderId(id)).Returns(new List<MessageRequest>());

            var result = this._userController.GetMessagesBySenderId(id);

            this._messageService.Verify(m => m.GetMessageBySenderId(id));
            result.Should().BeEquivalentTo(new List<MessageRequest>());

        }

        [TestMethod]
        public void Send_Message_Test()
        {
            var req = new MessageRequest();

            this._userController.SendMessage(req);

            this._messageService.Verify(m => m.SendMessage(req));
        }

        [TestMethod]
        public void Modify_Message_Test()
        {
            var req = new MessageRequest();

            this._userController.ModifyMessage(req);

            this._messageService.Verify(m => m.ModifiyMessage(req));
        }

        [TestMethod]
        [DataRow(1)]
        public void Remove_Message_Test(long id)
        {
            this._userController.RemoveMessage(id);

            this._messageService.Verify(m => m.RemoveMessage(id));
        }
    }
}
