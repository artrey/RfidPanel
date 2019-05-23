using System;
using System.IO;
using System.Text;
using System.Windows;
using RfidPanel.Models;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // The RFID device. It's found automatically
        private readonly Device _device = Device.FindDevice(
            new Configuration { BaudRate = 115200, UseTimeouts = true, ReadTimeout = 3000, WriteTimeout = 3000 });

        // The storage class. It uses SQLite3 in backend
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

            // Event on new uid received from arduino + mfrc522
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
            // disable all events in main window while add window uses
            _device.UidReceived -= UidReceived;
            new AddWindow(_device, _storage).ShowDialog();
            // update current uid state
            UidReceived(null, UID.Text);
            // enable events
            _device.UidReceived += UidReceived;
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            new RemoveWindow(_storage).ShowDialog();
            // update current uid state
            UidReceived(null, UID.Text);
        }

        private void Report(object sender, RoutedEventArgs e)
        {
            // create OpenFileDialog 
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                // set filter for file extension and default file extension
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = ".csv",
            };

            // display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // get the selected file name and display in a Label 
            if (result.HasValue && result.Value)
            {
                // make report in csv
                var csv = new StringBuilder($@"ФИО;Отдел;Время{Environment.NewLine}");

                foreach (var check in _storage.Checks())
                {
                    var p = _storage.FindPerson(check.PersonUid);
                    csv.AppendLine($"\"{p.Bio}\";\"{p.Department}\";\"{check.Time}\"");
                }

                File.WriteAllText(dlg.FileName, csv.ToString());
            }
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
                // if event raised by device (not perform from code), add visit to person
                if (sender != null)
                {
                    _storage.AddMark(p, DateTime.Now);
                }
            }
            // show person info
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
