using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.DTOs.Request
{
    public class MessageRequest
    {
        public long id { get; set; }
        public string content { get; set; }
        public string title { get; set; }
        public long senderId { get; set; }
        public long recipientId { get; set; }
        public DateTime createDate { get; set; }
    }
}
