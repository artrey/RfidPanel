using System.Windows;
using RfidPanel.Models;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for RemoveWindow.xaml
    /// </summary>
    public partial class RemoveWindow : Window
    {
        private readonly Storage _storage;

        public RemoveWindow(Storage storage)
        {
            InitializeComponent();

            _storage = storage;

            Table.ItemsSource = _storage.Persons();
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            var p = Table.SelectedItem as Person;
            if (p == null) return;
            _storage.RemovePerson(p);
            Table.ItemsSource = _storage.Persons();
        }
    }
}
