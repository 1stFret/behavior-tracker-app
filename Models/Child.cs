using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace BehaviorTracker.Models
{
    [Table("Children")]
    public class Child
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}