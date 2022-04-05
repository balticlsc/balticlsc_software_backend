﻿using System;

namespace Baltic.Security.Entities
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public DateTime Stamp { get; set; }
    }
}