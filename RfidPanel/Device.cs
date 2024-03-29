﻿using System;
using System.IO.Ports;
using System.Text;

namespace RfidPanel
{
    public class Device
    {
        protected SerialPort _port;
        protected StringBuilder _buffer = new StringBuilder();

        public event EventHandler<string> UidReceived;

        public bool IsOpen => _port?.IsOpen ?? false;

        public bool Open(Configuration config)
        {
            if (IsOpen) return false;

            // create serial port instance for receiving uids
            _port = new SerialPort(config.PortName, config.BaudRate)
            {
                DtrEnable = true
            };

            if (config.UseTimeouts)
            {
                _port.WriteTimeout = config.WriteTimeout;
                _port.ReadTimeout = config.ReadTimeout;
            }
            
            _port.Open();

            // checking that port is valid (need get firmware version)
            bool valid;
            try
            {
                valid = _port.ReadLine().TrimStart().StartsWith("Firmware Version: 0x");
            }
            catch (TimeoutException)
            {
                valid = false;
            }

            if (!valid)
            {
                _port.Close();
                return false;
            }

            _buffer.Clear();
            _port.DataReceived += DataReceived;
            return true;
        }

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // event oocurs on receiving data from ID card
            if (!(sender is SerialPort port)) return;

            // read line by line
            while (port.BytesToRead > 0)
            {
                var c = (char)port.ReadChar();
                if (c == '\n')
                {
                    var s = _buffer.ToString();
                    // raise event if uid found
                    if (s.StartsWith("UID:")) UidReceived?.Invoke(sender, s.Substring(4).Trim());
                    _buffer.Clear();
                    continue;
                }
                _buffer.Append(c);
            }
        }

        public void Close()
        {
            if (!IsOpen) return;
            _port.Close();
            _port = null;
        }

        public static Device FindDevice(Configuration config)
        {
            // auto find arduino device (scanning all available ports)
            var device = new Device();
            foreach (var portName in SerialPort.GetPortNames())
            {
                config.PortName = portName;
                if (device.Open(config))
                {
                    return device;
                }
            }

            return null;
        }
    }
}
