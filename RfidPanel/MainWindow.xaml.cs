using System;
using System.IO;
using System.Windows;
using RfidPanel.Models;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device _device = Device.FindDevice(
            new Configuration { BaudRate = 115200, UseTimeouts = true, ReadTimeout = 3000, WriteTimeout = 3000 });
        private readonly Storage _storage = new Storage(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rfid.db")
        );

        public MainWindow()
        {
            InitializeComponent();

            if (_device == null)
            {
                MessageBox.Show($@"Не найдено устройство для считывания меток!{Environment.NewLine}Проверьте соединение.", "Ошибка!");
                Environment.Exit(-1);
            }

            _device.UidReceived += UidReceived;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _device.UidReceived -= UidReceived;
            _device.Close();
        }

        private void SetErrorMessage(string mes)
        {
            Dispatcher.Invoke(() =>
            {
                Error.Text = mes;
            });
        }

        private void HideErrorMessage()
        {
            SetErrorMessage(string.Empty);
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            _device.UidReceived -= UidReceived;
            new AddWindow(_device, _storage).ShowDialog();
            var p = _storage.FindPerson(UID.Text);
            if (p != null) DisplayPerson(p);
            _device.UidReceived += UidReceived;
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            new RemoveWindow(_storage).ShowDialog();
        }

        private void UidReceived(object sender, string e)
        {
            if (string.IsNullOrEmpty(e)) return;

            var p = _storage.FindPerson(e);
            if (p == null)
            {
                p = new Person { Uid = e };
                SetErrorMessage("Сотрудник не найден в базе!");
            }
            else
            {
                HideErrorMessage();
                _storage.AddMark(p, DateTime.Now);
            }
            DisplayPerson(p);
        }

        public void DisplayPerson(Person p)
        {
            Dispatcher.Invoke(() =>
            {
                UID.Text = p.Uid;
                Bio.Text = p.Bio;
                Department.Text = p.Department;
                History.Items.Clear();
                foreach (var c in _storage.Checks(p))
                {
                    History.Items.Add(c.Time);
                }
                Photo.Source = Utils.LoadImageFromBytes(p.BinImage);
            });
        }
    }
}
