using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using RfidPanel.Models;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private readonly Device _device;
        private readonly Storage _storage;

        public AddWindow(Device device, Storage storage)
        {
            InitializeComponent();

            _device = device;
            _storage = storage;

            _device.UidReceived += UidReceived;
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _device.UidReceived -= UidReceived;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UID.Text) || UID.Text == "Приложите метку"
                || string.IsNullOrEmpty(Bio.Text) || string.IsNullOrEmpty(Department.Text))
            {
                MessageBox.Show("Поля не заполнены!", "Ошибка!");
                return;
            }

            var p = _storage.FindPerson(UID.Text);
            if (p != null)
            {
                MessageBox.Show($@"Метка уже используется сотрудником {p.Bio}!{Environment.NewLine}Используйте другую метку или удалите существующего сотрудника с данной меткой.", 
                    "Ошибка!");
                return;
            }

            _storage.Add(new Person
            {
                Uid = UID.Text,
                Bio = Bio.Text,
                Department = Department.Text,
                BinImage = File.Exists(FilePath.Content.ToString()) ? File.ReadAllBytes(FilePath.Content.ToString()) : null,
            });

            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            // create OpenFileDialog 
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                // set filter for file extension and default file extension
                Filter = "PNG Files (*.png)|*.png|JPEG/JPG Files (*.jpeg, *.jpg)|*.jpeg;*.jpg|BMP Files (*.bmp)|*.bmp",
                DefaultExt = ".png",
            };

            // display OpenFileDialog by calling ShowDialog method 
            var result = dlg.ShowDialog();

            // get the selected file name and display in a Label 
            if (result.HasValue && result.Value)
            {
                FilePath.Content = dlg.FileName;
                Photo.Source = Utils.LoadImageFromBytes(File.ReadAllBytes(dlg.FileName));
            }
        }

        private void UidReceived(object sender, string e)
        {
            Dispatcher.Invoke(() =>
            {
                UID.Text = e;
                UID.Foreground = Brushes.Black;
            });
        }
    }
}
