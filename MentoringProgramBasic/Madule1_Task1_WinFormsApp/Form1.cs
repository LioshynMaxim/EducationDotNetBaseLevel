using Madule1_Task1_ClassLibrary;

namespace Madule1_Task1_WinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            label.Text = TeskHelloHelper.SayHello(textBox.Text);
        }
    }
}
