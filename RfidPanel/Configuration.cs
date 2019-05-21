namespace RfidPanel
{
    public class Configuration
    {
        public string PortName { get; set; }
        public int BaudRate { get; set; }
        public bool UseTimeouts { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }
    }
}
