using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RfidPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Device _device = new Device();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //_device.UidReceived += UidReceived;
            //_device.Open(new Configuration { PortName = "COM16", BaudRate = 115200 });
            UidReceived(null, "0D 12 43 F1");
            SetErrorMessage("sdgsdfgsdfg");
            UID.Text = "gdfgsg";
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //_device.UidReceived -= UidReceived;
            //_device.Close();
            HideErrorMessage();
        }

        private void UidReceived(object sender, string e)
        {
            //Dispatcher.Invoke(() => ListBox.Items.Add(e));            
        }
    }
}
