using SQLite;

namespace RfidPanel.Models
{
    public class Person
    {
        [PrimaryKey]
        public string Uid { get; set; }
        public string Bio { get; set; }
        public string Department { get; set; }
        public byte[] BinImage { get; set; }
    }
}
