using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace TodoSqlite.Model
{
    public class TodoModel
    {
        [PrimaryKey, AutoIncrement]
        public int cid { get; set; }
        public int id { get; set; }
        public string title { get; set; }
        public int isCompleted { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public bool Delete { get; set; }
        public bool Update { get; set; }
        public bool Store { get; set; }
    }
}
