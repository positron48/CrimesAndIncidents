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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;

namespace CrimesAndIncidents
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqliteWorker sqlWorker;
        DataTable data;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                sqlWorker = new SqliteWorker("CrimesAndIncidents");
                data = sqlWorker.selectData("SELECT name, number FROM MilitaryUnit");
                crimesDataGrid.ItemsSource = data.DefaultView;
                crimesDataGrid.CanUserAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Во время закгрузки приложения возникли неполадки:\n" + ex.Message);
            }
        }

        private void MenuExit_Click_1(object sender, RoutedEventArgs e)
        {
            //запрос на сохранение изменений (если вносились)
            this.Close();
        }
    }
}
