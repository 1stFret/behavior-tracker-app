using System;
using System.Collections.Generic;
using System.Text;
using SQLite;


namespace BehaviorTracker.Models
{
    public class BehaviorEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string? ChildName { get; set; }
        public string? BehaviorType { get; set; }
        public bool IsPositive { get; set; }
        public string? Notes { get; set; }
        public DateTime LoggedAt { get; set; }

    }
}
