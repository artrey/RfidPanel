using System;
using SQLite;

namespace RfidPanel.Models
{
    public class Check
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Time { get; set; }
        [Indexed]
        public string PersonUid { get; set; }
    }
}
