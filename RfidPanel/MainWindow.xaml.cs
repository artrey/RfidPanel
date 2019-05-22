using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using RfidPanel.Models;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device _device;
        private readonly Storage _storage = new Storage(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "rfid.db")
        );

        public MainWindow()
        {
            InitializeComponent();

            //_device = Device.FindDevice(new Configuration { BaudRate = 115200 });
            //if (_device == null)
            //{
            //    MessageBox.Show($@"Не найдено устройство для считывания меток!{Environment.NewLine}Проверьте соединение.", "Ошибка!");
            //    Environment.Exit(-1);
            //}

            //_device.UidReceived += UidReceived;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //_device.UidReceived -= UidReceived;
            //_device.Close();
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
            UidReceived(null, "0D 12 43 F1");
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            HideErrorMessage();
        }

        private void UidReceived(object sender, string e)
        {
            var p = _storage.FindPerson(e);
            if (p == null)
            {
                UID.Text = e;
                SetErrorMessage("Сотрудник не найден в базе!");
            }
            else
            {
                HideErrorMessage();
                _storage.AddMark(p, DateTime.Now);
                DisplayPerson(p);
            }
        }

        public void DisplayPerson(Person p)
        {
            UID.Text = p.Uid;
            Bio.Text = p.Bio;
            Department.Text = p.Department;
            History.Items.Clear();
            foreach (var c in _storage.Checks(p))
            {
                History.Items.Add(c.Time);
            }
            //Photo.Source = LoadImageFromBytes(new byte[]{});
        }

        public static BitmapImage LoadImageFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();

                return image;
            }
        }
    }
}
