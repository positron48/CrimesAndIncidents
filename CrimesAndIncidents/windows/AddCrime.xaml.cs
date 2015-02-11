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
    /// Логика взаимодействия для AddCrime.xaml
    /// </summary>
    public partial class AddCrime : Window
    {
        SqliteWorker sqlWorker;

        AccompliceList accompliceList;
        MilitaryUnitList militaryList;
        ClauseList clauseList;
        DBList organList;
        DBList categoryList;

        Crime c = null;

        public AddCrime()
        {
            InitializeComponent();
        }

        public AddCrime(SqliteWorker _sqlWorker)
        {
            InitializeComponent();

            sqlWorker = _sqlWorker;

            organList = new DBList("Organ", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Organ")));
            categoryList = new DBList("Category", DataWorker.getList(sqlWorker.selectData("SELECT * FROM Category")));
            militaryList = new MilitaryUnitList(DataWorker.getMilitaryUnitList(sqlWorker.selectData("SELECT * FROM MilitaryUnit")));
            clauseList = new ClauseList(DataWorker.getClauseList(sqlWorker.selectData("SELECT * FROM Clause")));

            cbOrgan.ItemsSource = organList.values;
            cbMilitaryUnit.ItemsSource = militaryList.values;
            cbClause.ItemsSource = clauseList.values;
            lbCategoty.ItemsSource = categoryList.values;
        }

        internal static Crime gtNewCrime(SqliteWorker sqlWorker)
        {
            AddCrime wndCrime = new AddCrime(sqlWorker);
            wndCrime.ShowDialog();
            return wndCrime.c;
        }

        private void btnAddClause_Click(object sender, RoutedEventArgs e)
        {
            Clause newItem = AddClause.getClause();
            if (newItem != null)
            {
                //добавить в БД
                int id = sqlWorker.getNewId("Clause");
                if (sqlWorker.addClause(id, newItem))
                {
                    newItem.Id = id;
                    //если успешное добавление в БД
                    clauseList.values.Add(newItem);
                    cbClause.Items.Refresh();
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnAddOrgan_Click(object sender, RoutedEventArgs e)
        {
            string newItem = InputBox.input("Введите кем возбуждено дело");
            if (newItem != "")
            {
                int newId = sqlWorker.getNewId("Organ");
                //добавить в БД
                if (sqlWorker.addInDBList("Organ", newId, newItem))
                {
                    //если успешное добавление в БД
                    organList.values.Add(new KeyValue(newId, newItem));
                    cbOrgan.Items.Refresh();
                }
                else
                    MessageBox.Show("Ошибка при добавлении данных");
            }
        }

        private void btnAddAccomplice_Click(object sender, RoutedEventArgs e)
        {
            accompliceList = SelectAccomplice.getList(sqlWorker, accompliceList);
            lbAccomplice.ItemsSource = accompliceList.values;
        }
    }
}
