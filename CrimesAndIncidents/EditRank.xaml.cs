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
    /// Логика взаимодействия для EditRank.xaml
    /// </summary>
    public partial class EditRank : Window
    {
        RankList list;
        SqliteWorker sqlWorker;

        public EditRank(RankList _list, SqliteWorker _sqlWorker)
        {
            InitializeComponent();
            list = _list;
            sqlWorker = _sqlWorker;
            
            dataGrid.AutoGenerateColumns = false;
            dataGrid.ItemsSource = list.values;
        }

        private void btnAddItem_Click(object sender, RoutedEventArgs e)
        {
            Rank newItem = AddRank.getRank();
            if (newItem != null)
            {
                //добавить в БД
                if (sqlWorker.addRank(list.newId(), newItem))
                {
                    newItem.Id = list.newId();
                    //если успешное добавление в БД
                    list.values.Add(newItem);
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот элемент?", "Подтвердите действие", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Rank c = dataGrid.SelectedItem as Rank;
                if (sqlWorker.deleteItemById("Rank", c.Id))
                    list.deleteById(c.Id);
                else
                    MessageBox.Show("Ошибка удаления элемента, возможно он уже используется.");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
            dataGrid.CancelEdit();
            dataGrid.Items.Refresh();
            for (int i = 0; i < list.values.Count; i++)
            {
                if (list.values[i].isChanged)
                {
                    if (!sqlWorker.updateRank(list.values[i]))
                        MessageBox.Show("Ошибка при обновлении элементов");
                }
            }
            this.Close();
        }
    }
}
