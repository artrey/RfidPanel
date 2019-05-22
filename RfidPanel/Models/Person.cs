using SQLite;

namespace RfidPanel.Models
{
    public class Person
    {
        [PrimaryKey]
        public string Uid { get; set; }

        [NotNull]
        public string Bio { get; set; }

        [NotNull]
        public string Department { get; set; }

        public byte[] BinImage { get; set; }
    }
}
