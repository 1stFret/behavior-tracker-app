using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace BehaviorTracker.Models
{
    public class BehaviorType
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool IsPositive { get; set; }
        public int ChildId { get; set; }

    }
}
