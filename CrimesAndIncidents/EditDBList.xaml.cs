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
    /// Логика взаимодействия для EditDBList.xaml
    /// </summary>
    public partial class EditDBList : Window
    {
        public DBList dblist;
        private SqliteWorker sqlWorker;
        public EditDBList(string rusName, DBList list, SqliteWorker _sqlWorker)
        {
            InitializeComponent();
            
            sqlWorker = _sqlWorker;
            dblist = list;

            dataGrid.Columns[0].Header = rusName;
            dataGrid.AutoGenerateColumns = false;
            dataGrid.ItemsSource = dblist.values;
        }

        private void btnAddItem_Click_1(object sender, RoutedEventArgs e)
        {
            string newItem = InputBox.input("Введите " + dataGrid.Columns[0].Header);
            if (newItem != "")
            {
                //добавить в БД
                if (sqlWorker.addInDBList(dblist.TableName, dblist.newId(), newItem))
                {
                    //если успешное добавление в БД
                    dblist.values.Add(new KeyValue(dblist.newId(), newItem));
                }
            }
        }
    }
}
