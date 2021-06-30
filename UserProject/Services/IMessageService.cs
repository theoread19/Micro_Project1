using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.DTOs.Request;

namespace UserProject.Services
{
    public interface IMessageService
    {
        public List<MessageRequest> GetMessageBySenderId(long id);
        public void SendMessage(MessageRequest req);
        public void RemoveMessage(long id);

        public void ModifiyMessage(MessageRequest req);
    }
}
