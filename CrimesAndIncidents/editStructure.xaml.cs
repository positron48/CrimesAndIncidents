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
    /// Логика взаимодействия для editStructure.xaml
    /// </summary>
    public partial class EditStructure : Window
    {
        SqliteWorker sqlWorker;
        MilitaryUnitList mList;
        SubUnitList bList;
        SubUnitList sList;

        public EditStructure(SqliteWorker _sqlWorker, MilitaryUnitList _mList)
        {
            InitializeComponent();

            sqlWorker = _sqlWorker;
            mList = _mList;

            lbMilitaryUnit.ItemsSource = mList.values;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lbMilitaryUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sList != null) sList.values.Clear();
            if (lbMilitaryUnit.SelectedItem != null)
            {
                int idMilitaryUnit = (lbMilitaryUnit.SelectedItem as MilitaryUnit).Id;
                bList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idMilitaryUnit = " + idMilitaryUnit)));
                lbBattalion.ItemsSource = bList.values;
            }
        }

        private void lbBattalion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null)
            {
                int idSubUnit = (lbBattalion.SelectedItem as SubUnit).Id;
                sList = new SubUnitList(DataWorker.getSubUnitList(sqlWorker.selectData("SELECT * FROM SubUnit WHERE idFKSubUnit = " + idSubUnit)));
                lbSubUnit.ItemsSource = sList.values;
            }
        }

        private void btnDeleteSubUnit_Click(object sender, RoutedEventArgs e)
        {
            if (lbSubUnit.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное подразделение?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo)==MessageBoxResult.Yes)
            {
                int idSubUnit = (lbSubUnit.SelectedItem as SubUnit).Id;
                if (sqlWorker.deleteItemById("SubUnit", idSubUnit))
                {
                    sList.deleteById(idSubUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }

        private void btnDeleteBattalion_Click(object sender, RoutedEventArgs e)
        {
            if (lbBattalion.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранное подразделение?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int idSubUnit = (lbBattalion.SelectedItem as SubUnit).Id;
                if (sqlWorker.deleteItemById("SubUnit", idSubUnit))
                {
                    bList.deleteById(idSubUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }

        private void btnDeleteMilitaryUnit_Click(object sender, RoutedEventArgs e)
        {
            if (lbMilitaryUnit.SelectedItem != null && MessageBox.Show("Вы действительно хотите удалить выбранную часть?", "Подтвердждение на удаление элемента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                int idMilitaryUnit = (lbMilitaryUnit.SelectedItem as MilitaryUnit).Id;
                if (sqlWorker.deleteItemById("MilitaryUnit", idMilitaryUnit))
                {
                    mList.deleteById(idMilitaryUnit);
                }
                else
                {
                    MessageBox.Show("При удалении элемента возникла ошибка, возможно, он уже используется");
                }
            }
        }
    }
}
