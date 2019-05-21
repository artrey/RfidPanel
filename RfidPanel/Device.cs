using System;
using System.IO.Ports;
using System.Text;

namespace RfidPanel
{
    public class Device
    {
        protected SerialPort _port;
        protected StringBuilder _buffer = new StringBuilder();

        public event EventHandler<string> UidReceived;

        public bool IsOpen { get => _port == null ? false : _port.IsOpen; }

        public void Open(Configuration config)
        {
            if (IsOpen) return;

            _port = new SerialPort(config.PortName, config.BaudRate);
            if (config.UseTimeouts)
            {
                _port.WriteTimeout = config.WriteTimeout;
                _port.ReadTimeout = config.ReadTimeout;
            }

            _port.DataReceived += DataReceived;

            _buffer.Clear();
            _port.Open();
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = sender as SerialPort;
            if (port == null) return;

            while (port.BytesToRead > 0)
            {
                char c = (char)port.ReadChar();
                if (c == '\n')
                {
                    var s = _buffer.ToString();
                    if (s.StartsWith("UID:")) UidReceived?.Invoke(sender, s.Substring(4).Trim());
                    _buffer.Clear();
                    continue;
                }
                _buffer.Append(c);
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                _port.Close();
                _port = null;
            }
        }
    }
}
