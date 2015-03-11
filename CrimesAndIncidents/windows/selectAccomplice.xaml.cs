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
    /// Логика взаимодействия для selectAccomplice.xaml
    /// </summary>
    public partial class SelectAccomplice : Window
    {
        SqliteWorker sqlWorker;

        AccompliceList aChoosedList;
        AccompliceList aList;

        CollectionViewSource aChoosedView;

        public SelectAccomplice()
        {
            InitializeComponent();
        }

        public SelectAccomplice(SqliteWorker _sqlWorker, AccompliceList accompliceList)
        {
            InitializeComponent();

            lbChoosed.AutoGenerateColumns = false;
            lbChoosed.IsReadOnly = true;
            lbNotChoosed.AutoGenerateColumns = false;
            lbNotChoosed.IsReadOnly = true;

            sqlWorker = _sqlWorker;
            
            aList = new AccompliceList(
                DataWorker.getAccompliceList(
                    sqlWorker.selectData("SELECT R.shortName as rank, S.shortName as subUnit, SF.shortName as battalion, M.shortName as militaryUnit, A.*, R.priority " +
                        "FROM Accomplice A " +
                        "INNER JOIN SubUnit S ON S.idSubUnit = A.idSubUnit " +
                        "LEFT JOIN Rank R ON R.idRank = A.idRank " +
                        "LEFT JOIN SubUnit SF ON S.idFKSubUnit = SF.idSubUnit " +
                        "LEFT JOIN MilitaryUnit M ON M.idMilitaryUnit = S.idMilitaryUnit OR M.idMilitaryUnit = SF.idMilitaryUnit "+
                        "")));//сортировка хз по чему 
            if (accompliceList == null)
            {
                aChoosedList = new AccompliceList();
            }
            else
            {
                aChoosedList = accompliceList;
                for (int i = 0; i < accompliceList.values.Count; i++)
                {
                    aList.deleteById(accompliceList.values[i].Id);
                }
            }
            aChoosedView = new CollectionViewSource();
            aChoosedView.Source = aList.values;
            aChoosedView.Filter += collectView_Filter;

            lbChoosed.ItemsSource = aChoosedList.values;
            lbNotChoosed.ItemsSource = aChoosedView.View;

            txFilter.Focus();
        }

        private void collectView_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (e.Item as Accomplice).ShortName.ToLower().IndexOf(txFilter.Text.ToLower()) >= 0;
        }

        internal static AccompliceList getList(SqliteWorker sqlWorker, AccompliceList accompliceList)
        {
            SelectAccomplice wndS = new SelectAccomplice(sqlWorker, accompliceList);
            wndS.ShowDialog();
            return wndS.aChoosedList;
        }

        private void btnRight_Click_1(object sender, RoutedEventArgs e)
        {
            if (lbNotChoosed.SelectedItem != null)
            {
                Accomplice t = lbNotChoosed.SelectedItem as Accomplice;
                aList.values.Remove(t);
                aChoosedList.values.Add(t);
            }
        }

        private void btnLeft_Click_2(object sender, RoutedEventArgs e)
        {
            if (lbChoosed.SelectedItem != null)
            {
                Accomplice t = lbChoosed.SelectedItem as Accomplice;
                aChoosedList.values.Remove(t);
                aList.values.Add(t);
            }
        }

        private void btnOk_Click_3(object sender, RoutedEventArgs e)
        {
            if (aChoosedList.values.Count > 0)
                this.Close();
            else
                MessageBox.Show("Выберите как минимум одного участника!");
        }

        private void btnAddAccomplice_Click_1(object sender, RoutedEventArgs e)
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
                    aChoosedList.values.Add(newItem);
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void lbNotChoosed_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbNotChoosed.SelectedItem != null)
            {
                Accomplice t = lbNotChoosed.SelectedItem as Accomplice;
                aList.values.Remove(t);
                aChoosedList.values.Add(t);
            }
        }

        private void lbChoosed_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lbChoosed.SelectedItem != null)
            {
                Accomplice t = lbChoosed.SelectedItem as Accomplice;
                aChoosedList.values.Remove(t);
                aList.values.Add(t);
            }
        }

        private void txFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            aChoosedView.View.Refresh();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (lbChoosed.SelectedItem != null)
            {
                Accomplice s = AddAccomplice.getEditedAccomplice(lbChoosed.SelectedItem as Accomplice, sqlWorker);
                if (s != null)
                {
                    if (sqlWorker.updateAccomplice(s))
                    {
                        aChoosedList.update(s);
                        lbChoosed.Items.Refresh();
                    }
                    else
                        MessageBox.Show("Ошибка при изменении элемента");
                }
            }
        }



    }
}
