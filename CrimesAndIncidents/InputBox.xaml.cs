using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CrimesAndIncidents
{
    /// <summary>
    /// Логика взаимодействия для InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public string value;
        public InputBox()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.CanMinimize;
            txtBox.Focus();
        }

        public static string input(string header)
        {
            InputBox wndI = new InputBox();
            wndI.Title = header;
            wndI.ShowDialog();
            return wndI.value;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            value = txtBox.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
