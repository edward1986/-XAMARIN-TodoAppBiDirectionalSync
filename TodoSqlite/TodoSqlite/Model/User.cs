﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TodoSqlite.Model
{
    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public object email_verified_at { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    
}
