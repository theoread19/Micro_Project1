using System;
using System.Collections.Generic;

#nullable disable

namespace IndentityServer4.Data
{
    public partial class UserTable
    {
        public long Id { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string Passsword { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}
