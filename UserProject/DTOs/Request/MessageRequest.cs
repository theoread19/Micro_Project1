using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.DTOs.Request
{
    public class MessageRequest
    {
        public long Id { get; set; }
        public string? Content { get; set; }
        public string? Title { get; set; }
        public long SenderId { get; set; }
        public long RecipientId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
