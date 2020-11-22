using System;
using System.Collections.Generic;
using System.Text;

namespace TodoSqlite.Model
{
    public class TokenModel
    {
            public User user { get; set; }
            public string access_token { get; set; }
    }
}
