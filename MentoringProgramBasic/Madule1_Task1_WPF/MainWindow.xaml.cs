using Madule1_Task1_ClassLibrary;
using System.Windows;

namespace Madule1_Task1_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            label.Content = TeskHelloHelper.SayHello(textBox.Text);
        }
    }
}