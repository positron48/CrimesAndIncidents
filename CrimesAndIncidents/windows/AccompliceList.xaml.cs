using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
        CollectionViewSource coll;
        ObservableCollection<Crime> crimes;

        public Accomplices(AccompliceList al, SqliteWorker sqlWorker)
        {
            InitializeComponent();

            this.al = al;
            this.sqlWorker = sqlWorker;

            coll = new CollectionViewSource();
            coll.Source = al.values;
            coll.Filter += coll_Filter;

            dataGrid.CanUserAddRows = false;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.IsReadOnly = true;
            dataGrid.ItemsSource = coll.View;

            dgCrimes.CanUserAddRows = false;
            dgCrimes.AutoGenerateColumns = false;
        }

        void coll_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (((e.Item as Accomplice).ShortName.ToLower().IndexOf(txFilter.Text.ToLower()) >= 0));
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
                    coll.View.Refresh();
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
            if (dataGrid.SelectedItem != null)
            {
                Accomplice s = AddAccomplice.getEditedAccomplice(dataGrid.SelectedItem as Accomplice, sqlWorker);
                if (s != null)
                {
                    if (sqlWorker.updateAccomplice(s))
                    {
                        al.update(s);
                        dataGrid.Items.Refresh();
                    }
                    else
                        MessageBox.Show("Ошибка при изменении элемента");
                }
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            coll.View.Refresh();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                lbCrimes.Content = "Преступления и происшествия: " + 
                    (dataGrid.SelectedItem as Accomplice).Rank + " " + 
                    (dataGrid.SelectedItem as Accomplice).ShortName;
                crimes = DataWorker.getCrimes(
                   sqlWorker,
                   "",
                   "9999.99.99",
                   (dataGrid.SelectedItem as Accomplice).Id);
                dgCrimes.ItemsSource = crimes;
            }
        }

    }
}
