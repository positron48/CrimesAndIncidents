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

        public EditDBList(string rusName, DBList list)
        {
            InitializeComponent();

            dataGrid.Columns[0].Header = rusName;
            dataGrid.AutoGenerateColumns = false;

            dblist = list;

            dataGrid.ItemsSource = dblist.values;
        }
    }
}
