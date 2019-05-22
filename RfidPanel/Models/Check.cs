using System;
using SQLite;

namespace RfidPanel.Models
{
    public class Check
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public DateTime Time { get; set; }

        [Indexed, NotNull]
        public string PersonUid { get; set; }
    }
}
