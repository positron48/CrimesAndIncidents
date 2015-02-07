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
    /// Логика взаимодействия для Accomplices.xaml
    /// </summary>
    public partial class Accomplices : Window
    {
        private AccompliceList al;
        private SqliteWorker sqlWorker;

        public Accomplices(AccompliceList al, SqliteWorker sqlWorker)
        {
            InitializeComponent();

            this.al = al;
            this.sqlWorker = sqlWorker;
            
            dataGrid.CanUserAddRows = false;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.IsReadOnly = true;
            dataGrid.ItemsSource = al.values;

        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            Accomplice newItem = AddAccomplice.getAccomplice(sqlWorker);
            if (newItem != null)
            {
                //добавить в БД
                int id = sqlWorker.getNewId("Accomplice");
                if (sqlWorker.addAccomplice(id, newItem))
                {
                    newItem.Id = id;
                    //если успешное добавление в БД
                    al.values.Add(newItem);
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот элемент?", "Подтвердите действие", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Accomplice a = dataGrid.SelectedItem as Accomplice;
                if (sqlWorker.deleteItemById("Accomplice", a.Id))
                    al.deleteById(a.Id);
                else
                    MessageBox.Show("Ошибка удаления элемента, возможно он уже используется.");
            }
        }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
